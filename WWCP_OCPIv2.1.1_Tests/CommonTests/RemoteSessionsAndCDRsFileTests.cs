/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Globalization;

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.CommonTests
{

    /// <summary>
    /// The sessions and the charge detail records two CPOs push to an EMSP
    /// are each one's own, also where they have the same id, and are there
    /// at the next start.
    /// </summary>
    /// <remarks>
    /// The Common API kept both by their id alone: the second CPO's session
    /// or charge detail record of an id replaced the first one's, and a
    /// PATCH or a removal of one CPO's could reach the other's.
    ///
    /// The next start is a Common API made anew on the same directory, which
    /// reads back what it wrote, as a node's start does. Nothing here listens:
    /// the HTTP server is made and never started.
    /// </remarks>
    [TestFixture]
    public class RemoteSessionsAndCDRsFileTests
    {

        #region Data

        private static readonly CountryCode     de          = CountryCode.Parse("DE");
        private static readonly Party_Id        cpoA        = Party_Id.Parse("AAA");
        private static readonly Party_Id        cpoB        = Party_Id.Parse("BBB");
        private static readonly Session_Id      sessionId   = Session_Id.Parse("SESSION0001");
        private static readonly CDR_Id          cdrId       = CDR_Id.Parse("CDR0001");

        /// <summary>
        /// When the sessions and the charge detail records were last updated:
        /// a time of their own, so that they do not read the clock.
        /// </summary>
        private static readonly DateTimeOffset  start       = new (2026, 10, 1, 8, 0, 0, TimeSpan.Zero);

        private String     directory  = default!;
        private CommonAPI  api        = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void SetUp()
        {

            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(directory);

            api        = ACommonAPI();

        }

        [TearDown]
        public async Task TearDown()
        {

            // Its queue written out, before its directory goes.
            await api.BaseAPI.DisposeAsync();

            try
            {
                Directory.Delete(directory, true);
            }
            catch (IOException)
            { }
            catch (UnauthorizedAccessException)
            { }

        }

        #endregion


        #region TheSessionsOfTwoCPOsOfOneIdAreKeptApartAndAreThereAtTheNextStart()

        [Test]
        public async Task TheSessionsOfTwoCPOsOfOneIdAreKeptApartAndAreThereAtTheNextStart()
        {

            var pushedA = await api.AddOrUpdateSession(ASession(cpoA, 1.11M));
            var pushedB = await api.AddOrUpdateSession(ASession(cpoB, 2.22M));

            Assert.Multiple(() => {
                Assert.That(pushedA.IsSuccess, Is.True, $"CPO A's session was refused: {pushedA.ErrorResponse}");
                Assert.That(pushedB.IsSuccess, Is.True, $"CPO B's session was refused: {pushedB.ErrorResponse}");
            });

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            Assert.Multiple(() => {

                Assert.That(api.GetSessions(de, cpoA).Select(session => $"{session.Id} {session.kWh.kWh.ToString(CultureInfo.InvariantCulture)}"), Is.EquivalentTo(new[] { "SESSION0001 1.11" }),
                            "At the next start CPO A's session is not as it was pushed.");

                Assert.That(api.GetSessions(de, cpoB).Select(session => $"{session.Id} {session.kWh.kWh.ToString(CultureInfo.InvariantCulture)}"), Is.EquivalentTo(new[] { "SESSION0001 2.22" }),
                            "At the next start CPO B's session is not as it was pushed.");

                Assert.That(api.GetSessions().Count(), Is.EqualTo(2), "The sessions in all are not both.");

                Assert.That(api.TryGetSession(de, cpoA, sessionId, out var a) ? a.kWh.kWh : 0, Is.EqualTo(1.11M));
                Assert.That(api.TryGetSession(de, cpoB, sessionId, out var b) ? b.kWh.kWh : 0, Is.EqualTo(2.22M));

            });

        }

        #endregion

        #region APatchOfOneCPOsSessionLeavesTheOthersOfTheSameId()

        [Test]
        public async Task APatchOfOneCPOsSessionLeavesTheOthersOfTheSameId()
        {

            await api.AddOrUpdateSession(ASession(cpoA, 1.11M));
            await api.AddOrUpdateSession(ASession(cpoB, 2.22M));

            var patched = await api.TryPatchSession(
                                    de,
                                    cpoB,
                                    sessionId,
                                    new JObject(
                                        new JProperty("status",        "COMPLETED"),
                                        new JProperty("last_updated",  (start + TimeSpan.FromHours(1)).ToISO8601())
                                    )
                                );

            Assert.Multiple(() => {
                Assert.That(patched.IsSuccess, Is.True, $"CPO B's session could not be patched: {patched.ErrorResponse}");
                Assert.That(api.TryGetSession(de, cpoA, sessionId, out var a) ? a.Status.ToString() : "none", Is.EqualTo("ACTIVE"),    "CPO A's session was patched with CPO B's.");
                Assert.That(api.TryGetSession(de, cpoB, sessionId, out var b) ? b.Status.ToString() : "none", Is.EqualTo("COMPLETED"), "CPO B's session was not patched.");
            });

        }

        #endregion

        #region ASessionRemovedIsOnlyThatPartys()

        [Test]
        public async Task ASessionRemovedIsOnlyThatPartys()
        {

            await api.AddOrUpdateSession(ASession(cpoA, 1.11M));
            await api.AddOrUpdateSession(ASession(cpoB, 2.22M));

            var removed = await api.RemoveSession(ASession(cpoA, 1.11M));

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            Assert.Multiple(() => {
                Assert.That(removed.IsSuccess,                          Is.True,  $"CPO A's session could not be removed: {removed.ErrorResponse}");
                Assert.That(api.SessionExists(de, cpoA, sessionId),     Is.False, "CPO A's session is still there.");
                Assert.That(api.SessionExists(de, cpoB, sessionId),     Is.True,  "CPO B's session went with CPO A's.");
            });

        }

        #endregion


        #region TheCDRsOfTwoCPOsOfOneIdAreKeptApartAndAreThereAtTheNextStart()

        [Test]
        public async Task TheCDRsOfTwoCPOsOfOneIdAreKeptApartAndAreThereAtTheNextStart()
        {

            var pushedA = await api.AddCDR(ACDR(cpoA, 1.11M));
            var pushedB = await api.AddCDR(ACDR(cpoB, 2.22M));

            Assert.Multiple(() => {
                Assert.That(pushedA.IsSuccess, Is.True, $"CPO A's charge detail record was refused: {pushedA.ErrorResponse}");
                Assert.That(pushedB.IsSuccess, Is.True, $"CPO B's charge detail record was refused: {pushedB.ErrorResponse}");
            });

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            Assert.Multiple(() => {

                Assert.That(api.GetCDRs(de, cpoA).Select(cdr => $"{cdr.Id} {cdr.TotalCost.ToString(CultureInfo.InvariantCulture)}"), Is.EquivalentTo(new[] { "CDR0001 1.11" }),
                            "At the next start CPO A's charge detail record is not as it was pushed.");

                Assert.That(api.GetCDRs(de, cpoB).Select(cdr => $"{cdr.Id} {cdr.TotalCost.ToString(CultureInfo.InvariantCulture)}"), Is.EquivalentTo(new[] { "CDR0001 2.22" }),
                            "At the next start CPO B's charge detail record is not as it was pushed.");

                Assert.That(api.GetCDRs().Count(), Is.EqualTo(2), "The charge detail records in all are not both.");

                Assert.That(api.TryGetCDR(de, cpoA, cdrId, out var a) ? a.TotalCost : 0, Is.EqualTo(1.11M));
                Assert.That(api.TryGetCDR(de, cpoB, cdrId, out var b) ? b.TotalCost : 0, Is.EqualTo(2.22M));

            });

        }

        #endregion

        #region APatchOfOneCPOsCDRLeavesTheOthersOfTheSameId()

        [Test]
        public async Task APatchOfOneCPOsCDRLeavesTheOthersOfTheSameId()
        {

            await api.AddCDR(ACDR(cpoA, 1.11M));
            await api.AddCDR(ACDR(cpoB, 2.22M));

            var patched = await api.TryPatchCDR(
                                    de,
                                    cpoB,
                                    cdrId,
                                    new JObject(
                                        new JProperty("total_cost",    3.33M),
                                        new JProperty("last_updated",  (start + TimeSpan.FromHours(1)).ToISO8601())
                                    )
                                );

            Assert.Multiple(() => {
                Assert.That(patched.IsSuccess, Is.True, $"CPO B's charge detail record could not be patched: {patched.ErrorResponse}");
                Assert.That(api.TryGetCDR(de, cpoA, cdrId, out var a) ? a.TotalCost : 0, Is.EqualTo(1.11M), "CPO A's charge detail record was patched with CPO B's.");
                Assert.That(api.TryGetCDR(de, cpoB, cdrId, out var b) ? b.TotalCost : 0, Is.EqualTo(3.33M), "CPO B's charge detail record was not patched.");
            });

        }

        #endregion

        #region ACDRRemovedIsOnlyThatPartys()

        [Test]
        public async Task ACDRRemovedIsOnlyThatPartys()
        {

            await api.AddCDR(ACDR(cpoA, 1.11M));
            await api.AddCDR(ACDR(cpoB, 2.22M));

            var removed = await api.RemoveCDR(ACDR(cpoA, 1.11M));

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            Assert.Multiple(() => {
                Assert.That(removed.IsSuccess,                  Is.True,  $"CPO A's charge detail record could not be removed: {removed.ErrorResponse}");
                Assert.That(api.CDRExists(de, cpoA, cdrId),     Is.False, "CPO A's charge detail record is still there.");
                Assert.That(api.CDRExists(de, cpoB, cdrId),     Is.True,  "CPO B's charge detail record went with CPO A's.");
            });

        }

        #endregion


        #region (private static) ALocation(PartyId) / ASession(PartyId, kWh) / ACDR(PartyId, TotalCost)

        /// <summary>
        /// A location of the given party.
        /// </summary>
        private static Location ALocation(Party_Id PartyId)

            => new (
                   de,
                   PartyId,
                   Location_Id.Parse("LOC0001"),
                   LocationType.PARKING_LOT,
                   "Biberweg 18",
                   "Jena",
                   "07749",
                   Country.Germany,
                   GeoCoordinate.Parse(50.9, 11.6),
                   LastUpdated: start
               );

        /// <summary>
        /// An active session of the given party, of the one session id of these
        /// tests, with the given energy, so that the two can be told apart.
        /// </summary>
        private static Session ASession(Party_Id PartyId, Decimal kWh)

            => new (
                   CountryCode:  de,
                   PartyId:      PartyId,
                   Id:           sessionId,
                   Start:        start,
                   kWh:          WattHour.FromKWh(kWh),
                   AuthId:       Auth_Id.Parse("1234"),
                   AuthMethod:   AuthMethods.AUTH_REQUEST,
                   Location:     ALocation(PartyId),
                   Currency:     Currency.EUR,
                   Status:       SessionStatusTypes.ACTIVE,
                   LastUpdated:  start
               );

        /// <summary>
        /// A charge detail record of the given party, of the one id of these
        /// tests, with the given total cost, so that the two can be told apart.
        /// </summary>
        private static CDR ACDR(Party_Id PartyId, Decimal TotalCost)

            => new (
                   CountryCode:      de,
                   PartyId:          PartyId,
                   Id:               cdrId,
                   Start:            start,
                   Stop:             start + TimeSpan.FromHours(1),
                   AuthId:           Auth_Id.Parse("1234"),
                   AuthMethod:       AuthMethods.AUTH_REQUEST,
                   Location:         ALocation(PartyId),
                   Currency:         Currency.EUR,
                   ChargingPeriods:  [
                                         ChargingPeriod.Create(
                                             start,
                                             [ CDRDimension.ENERGY(WattHour.FromKWh(1M)) ]
                                         )
                                     ],
                   TotalCost:        TotalCost,
                   TotalEnergy:      WattHour.FromKWh(1M),
                   TotalTime:        TimeSpan.FromHours(1),
                   LastUpdated:      start
               );

        #endregion

        #region (private) ACommonAPI()

        /// <summary>
        /// The Common API of an EMSP on this test's directory. Made anew, it
        /// reads back what the one before it wrote, as the next start of a
        /// node does.
        /// </summary>
        private CommonAPI ACommonAPI()

            => new (

                   OurBusinessDetails:  new BusinessDetails("GraphDefined EMSP"),
                   OurCountryCode:      CountryCode.Parse("DE"),
                   OurPartyId:          Party_Id.   Parse("GDF"),
                   OurRole:             Role.EMSP,

                   BaseAPI:             new CommonHTTPAPI(
                                          HTTPAPI:          new HTTPExtAPI(
                                                                HTTPServer: new HTTPServer(TCPPort: IPPort.Parse(3999))
                                                            ),
                                          OurBaseURL:       URL.Parse("http://127.0.0.1:3999/ocpi"),
                                          OurVersionsURL:   URL.Parse("http://127.0.0.1:3999/ocpi/versions"),
                                          RootPath:         HTTPPath.Parse("/ocpi"),
                                          DisableLogging:   true,
                                          LoggingPath:      directory
                                      ),

                   URLPathPrefix:       HTTPPath.Parse("/ocpi/v2.1.1"),
                   DatabaseFilePath:    directory,
                   DisableLogging:      true,
                   LoggingPath:         directory

               );

        #endregion

    }

}
