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
    /// A session removed is still removed at the next start.
    /// </summary>
    /// <remarks>
    /// <para>
    /// RemoveSession wrote its line as "removeTariff", with the session. The
    /// next start read it as a tariff, which it is not, and passed over it:
    /// the session removed was there again. A line written so before still
    /// removes its session.
    /// </para>
    /// <para>
    /// The next start is a Common API made anew on the same directory, which
    /// reads the files back as a node's start does - see RemovedAtOnceTests.
    /// Nothing here listens: the HTTP servers are made and never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class SessionFileTests
    {

        #region Data

        private static readonly CountryCode     countryCode  = CountryCode.Parse("DE");
        private static readonly Party_Id        partyId      = Party_Id.   Parse("GEF");

        /// <summary>
        /// When the sessions started, a time of their own.
        /// </summary>
        private static readonly DateTimeOffset  start        = new (2026, 10, 1, 8, 0, 0, TimeSpan.Zero);

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


        #region ASessionRemovedIsGoneAtTheNextStart()

        [Test]
        public async Task ASessionRemovedIsGoneAtTheNextStart()
        {

            var removed  = ASession("SESSION0001");
            var kept     = ASession("SESSION0002");

            var added1   = await api.AddSession(removed);
            var added2   = await api.AddSession(kept);
            var removal  = await api.RemoveSession(removed);

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"The session to remove could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The session to keep could not be added: {added2.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The session could not be removed: {removal.ErrorResponse}");
            });

            var next     = await NextStart();

            Assert.Multiple(() => {

                Assert.That(next.GetSessions().Select(session => session.Id.ToString()), Is.EquivalentTo(new[] { "SESSION0002" }),
                            "The next start knows the session removed, or not the one kept.");

                Assert.That(CommandsIn(next.AssetsDBFileName), Does.Contain(CommonHTTPAPI.removeSession).And.Not.Contain(CommonHTTPAPI.removeTariff),
                            "The session removed is not written down as a session removed.");

            });

        }

        #endregion

        #region ASessionRemovedBeforeIsGoneAtTheNextStart()

        /// <summary>
        /// A session removed before, its line written as RemoveSession wrote
        /// it then - as "removeTariff" - is gone at the next start as well.
        /// </summary>
        [Test]
        public async Task ASessionRemovedBeforeIsGoneAtTheNextStart()
        {

            var removed  = ASession("SESSION0001");
            var kept     = ASession("SESSION0002");

            var added1   = await api.AddSession(removed);
            var added2   = await api.AddSession(kept);

            Assert.Multiple(() => {
                Assert.That(added1.IsSuccess, Is.True, $"The session to remove could not be added: {added1.ErrorResponse}");
                Assert.That(added2.IsSuccess, Is.True, $"The session to keep could not be added: {added2.ErrorResponse}");
            });

            await api.LogAsset(CommonHTTPAPI.removeTariff, removed.ToJSON(true, true), EventTracking_Id.New);

            Assert.That((await NextStart()).GetSessions().Select(session => session.Id.ToString()), Is.EquivalentTo(new[] { "SESSION0002" }),
                        "The next start knows the session removed before, or not the one kept.");

        }

        #endregion


        #region (private static) ASession(Id)

        /// <summary>
        /// A session of this Common API's own party, at a location of its own.
        /// </summary>
        private static Session ASession(String Id)

            => new (
                   CountryCode:  countryCode,
                   PartyId:      partyId,
                   Id:           Session_Id.Parse(Id),
                   Start:        start,
                   kWh:          WattHour.FromKWh(1.11M),
                   AuthId:       Auth_Id.Parse("1234"),
                   AuthMethod:   AuthMethods.AUTH_REQUEST,
                   Location:     new Location(
                                     countryCode,
                                     partyId,
                                     Location_Id. Parse("LOCATION0001"),
                                     LocationType.UNDERGROUND_GARAGE,
                                     "Biberweg 18",
                                     "Jena",
                                     "07749",
                                     Country.Germany,
                                     GeoCoordinate.Parse(10, 20)
                                 ),
                   Currency:     Currency.EUR,
                   Status:       SessionStatusTypes.ACTIVE,
                   LastUpdated:  start
               );

        #endregion

        #region (private static) CommandsIn(FileName)

        /// <summary>
        /// The commands of the lines in a file, in the order they are in it.
        /// </summary>
        private static IEnumerable<String> CommandsIn(String FileName)

            => File.ReadLines(FileName).
                    Where (line => line.StartsWith('{')).
                    Select(line => JObject.Parse(line).Properties().First().Name).
                    ToArray();

        #endregion

        #region (private) NextStart()

        /// <summary>
        /// The next start of a node: the Common API so far disposed - its assets
        /// go through a queue, and are all in their file once it is - and one
        /// made anew on the same directory, which reads back what it wrote.
        /// </summary>
        private async Task<CommonAPI> NextStart()
        {

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            return api;

        }

        #endregion

        #region (private) ACommonAPI()

        /// <summary>
        /// A Common API on this test's directory. Made anew, it reads back what
        /// the one before it wrote, as the next start of a node does.
        /// </summary>
        private CommonAPI ACommonAPI()

            => new (

                   OurBusinessDetails:  new BusinessDetails("GraphDefined CSO"),
                   OurCountryCode:      countryCode,
                   OurPartyId:          partyId,
                   OurRole:             Role.CPO,

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
