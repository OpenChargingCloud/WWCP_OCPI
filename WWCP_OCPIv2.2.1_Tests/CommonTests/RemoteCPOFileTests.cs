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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.CommonTests
{

    /// <summary>
    /// The remote CPOs of an EMSP and their assets - locations, tariffs,
    /// sessions, charge detail records - are there at the next start as they
    /// were last written down.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The EMSP wrote all of them to the file of the assets of its Common
    /// API, and nothing read them back: they were gone at every start. A
    /// remote CPO was written with its identification only. And the remote
    /// CDRs were written with the Common API's own commands: removing every
    /// remote CDR removed every CDR of the Common API's own at the next
    /// start.
    /// </para>
    /// <para>
    /// The hub's HUB_HTTPAPI keeps its remote CPOs the same way, through the
    /// same RemoteCPOAssets, and cannot be made at all as Hermod stands - see
    /// the roaming hub's OCPIv2_2_1.cs - so these tests go through the EMSP.
    /// </para>
    /// <para>
    /// The next start is a Common API and an EMSP API made anew on the same
    /// directory, which read the files back as a node's start does - see
    /// RemovedAtOnceTests. Nothing here listens: the HTTP servers are made and
    /// never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class RemoteCPOFileTests
    {

        #region Data

        private static readonly CountryCode     countryCode   = CountryCode.Parse("DE");
        private static readonly Party_Idv3      ourPartyId    = Party_Idv3.From(countryCode, Party_Id.Parse("GEF"));
        private static readonly Party_Idv3      remoteCPOId   = Party_Idv3.From(countryCode, Party_Id.Parse("XYZ"));

        /// <summary>
        /// When things began and were last updated: a time of their own, so
        /// that none of them reads the clock.
        /// </summary>
        private static readonly DateTimeOffset  start         = new (2026, 10, 1, 8, 0, 0, TimeSpan.Zero);

        private String        directory  = default!;
        private CommonAPI     api        = default!;
        private EMSP_HTTPAPI  emsp       = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public async Task SetUp()
        {

            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(directory);

            (api, emsp) = ANode();

            var cpo    = await emsp.AddRemoteCPO(remoteCPOId, Role.CPO, new BusinessDetails("XYZ Charging"));

            Assert.That(cpo.IsSuccess, Is.True, $"The remote CPO could not be added: {cpo.ErrorResponse}");

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


        #region ARemoteCPOAddedIsThereAtTheNextStart()

        [Test]
        public async Task ARemoteCPOAddedIsThereAtTheNextStart()
        {

            var next = await NextStart();

            Assert.That(next.RemoteCPOs.Select(AsText), Is.EquivalentTo(new[] { AsText(new PartyData(remoteCPOId, Role.CPO, new BusinessDetails("XYZ Charging"))) }),
                        "The next start does not know the remote CPO as it was added.");

        }

        #endregion

        #region ARemoteLocationAddedIsThereAtTheNextStart()

        [Test]
        public async Task ARemoteLocationAddedIsThereAtTheNextStart()
        {

            var location  = ALocation("LOCATION0001");
            var added     = await emsp.AddRemoteLocation(location);

            Assert.That(added.IsSuccess, Is.True, $"The remote location could not be added: {added.ErrorResponse}");

            Assert.That((await NextStart()).GetRemoteLocations(remoteCPOId).Select(AsJSON), Is.EquivalentTo(new[] { AsJSON(location) }),
                        "The next start does not know the remote location added.");

        }

        #endregion

        #region ARemoteLocationRemovedIsGoneAtTheNextStart()

        [Test]
        public async Task ARemoteLocationRemovedIsGoneAtTheNextStart()
        {

            var removed  = ALocation("LOCATION0001");
            var kept     = ALocation("LOCATION0002");

            var added1   = await emsp.AddRemoteLocation(removed);
            var added2   = await emsp.AddRemoteLocation(kept);
            var removal  = await emsp.RemoveRemoteLocation(remoteCPOId, removed.Id);

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"The remote location to remove could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The remote location to keep could not be added: {added2.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The remote location could not be removed: {removal.ErrorResponse}");
            });

            Assert.That((await NextStart()).GetRemoteLocations(remoteCPOId).Select(AsJSON), Is.EquivalentTo(new[] { AsJSON(kept) }),
                        "The next start knows the remote location removed, or not the one kept.");

        }

        #endregion

        #region ARemoteTariffAddedIsThereAtTheNextStart()

        [Test]
        public async Task ARemoteTariffAddedIsThereAtTheNextStart()
        {

            var tariff  = ATariff();
            var added   = await emsp.AddRemoteTariff(tariff);

            Assert.That(added.IsSuccess, Is.True, $"The remote tariff could not be added: {added.ErrorResponse}");

            Assert.That((await NextStart()).GetRemoteTariffs(remoteCPOId).Select(AsJSON), Is.EquivalentTo(new[] { AsJSON(tariff) }),
                        "The next start does not know the remote tariff added.");

        }

        #endregion

        #region ARemoteSessionAddedIsThereAtTheNextStart()

        [Test]
        public async Task ARemoteSessionAddedIsThereAtTheNextStart()
        {

            var session  = ASession();
            var added    = await emsp.AddRemoteSession(session);

            Assert.That(added.IsSuccess, Is.True, $"The remote session could not be added: {added.ErrorResponse}");

            Assert.That((await NextStart()).GetRemoteSessions(remoteCPOId).Select(AsJSON), Is.EquivalentTo(new[] { AsJSON(session) }),
                        "The next start does not know the remote session added.");

        }

        #endregion

        #region ARemoteCDRAddedIsThereAtTheNextStart()

        [Test]
        public async Task ARemoteCDRAddedIsThereAtTheNextStart()
        {

            var cdr    = ACDR(remoteCPOId, "CDR0001");
            var added  = await emsp.AddRemoteCDR(cdr);

            Assert.That(added.IsSuccess, Is.True, $"The remote CDR could not be added: {added.ErrorResponse}");

            Assert.That((await NextStart()).GetRemoteCDRs(remoteCPOId).Select(AsJSON), Is.EquivalentTo(new[] { AsJSON(cdr) }),
                        "The next start does not know the remote CDR added.");

        }

        #endregion

        #region ARemoteCDRWrittenAsBeforeIsThereAtTheNextStart()

        /// <summary>
        /// A remote CDR written with the Common API's own command, as the EMSP
        /// wrote it before, is there at the next start as well.
        /// </summary>
        [Test]
        public async Task ARemoteCDRWrittenAsBeforeIsThereAtTheNextStart()
        {

            var cdr = ACDR(remoteCPOId, "CDR0001");

            await api.LogAsset(CommonHTTPAPI.addChargeDetailRecord, cdr.ToJSON(), EventTracking_Id.New);

            Assert.That((await NextStart()).GetRemoteCDRs(remoteCPOId).Select(AsJSON), Is.EquivalentTo(new[] { AsJSON(cdr) }),
                        "The next start does not know the remote CDR written as before.");

        }

        #endregion

        #region RemovingEveryRemoteCDRLeavesTheOwnCDRsAlone()

        /// <summary>
        /// Every remote CDR removed at once is gone at the next start - and the
        /// Common API's own CDRs are still there.
        /// </summary>
        [Test]
        public async Task RemovingEveryRemoteCDRLeavesTheOwnCDRsAlone()
        {

            var ours    = ACDR(ourPartyId,  "CDR0001");
            var remote  = ACDR(remoteCPOId, "CDR0002");

            var added1  = await api. AddCDR      (ours);
            var added2  = await emsp.AddRemoteCDR(remote);
            var removal = await emsp.RemoveAllRemoteCDRs();

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"Our CDR could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The remote CDR could not be added: {added2.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The remote CDRs could not be removed: {removal.ErrorResponse}");
            });

            var next = await NextStart();

            Assert.Multiple(() => {
                Assert.That(api. GetCDRs().     Select(AsJSON), Is.EquivalentTo(new[] { AsJSON(ours) }), "The next start does not know our own CDR.");
                Assert.That(next.GetRemoteCDRs().Select(AsJSON), Is.Empty,                               "The next start knows a remote CDR removed at once.");
            });

        }

        #endregion


        #region (private static) ALocation / ATariff / ASession / ACDR

        /// <summary>
        /// A location of the remote CPO.
        /// </summary>
        private static Location ALocation(String Id)

            => new (
                   CountryCode:  remoteCPOId.CountryCode,
                   PartyId:      remoteCPOId.PartyId,
                   Id:           Location_Id.Parse(Id),
                   Publish:      true,
                   Address:      "Biberweg 18",
                   City:         "Jena",
                   Country:      Country.Germany,
                   Coordinates:  GeoCoordinate.Parse(50.9, 11.6),
                   TimeZone:     "Europe/Berlin",
                   LastUpdated:  start
               );

        /// <summary>
        /// A tariff of the remote CPO. Its prices have no zeros at their ends,
        /// which a price read back does not keep: JSON compares them as text.
        /// </summary>
        private static Tariff ATariff()

            => new (
                   CountryCode:     remoteCPOId.CountryCode,
                   PartyId:         remoteCPOId.PartyId,
                   Id:              Tariff_Id.Parse("TARIFF0001"),
                   Currency:        Currency.EUR,
                   TariffElements:  [
                                        new TariffElement(
                                            [
                                                PriceComponent.ChargingTime(
                                                    2.5M,
                                                    0.1M,
                                                    TimeSpan.FromSeconds(300)
                                                )
                                            ]
                                        )
                                    ],
                   LastUpdated:     start
               );

        /// <summary>
        /// A session at a location of the remote CPO.
        /// </summary>
        private static Session ASession()

            => new (
                   CountryCode:  remoteCPOId.CountryCode,
                   PartyId:      remoteCPOId.PartyId,
                   Id:           Session_Id.Parse("SESSION0001"),
                   Start:        start,
                   kWh:          WattHour.FromKWh(1.11M),
                   CDRToken:     ACDRToken(),
                   AuthMethod:   AuthMethod.AUTH_REQUEST,
                   LocationId:   Location_Id. Parse("LOCATION0001"),
                   EVSEUId:      EVSE_UId.    Parse("EVSE0001"),
                   ConnectorId:  Connector_Id.Parse("1"),
                   Currency:     Currency.EUR,
                   Status:       SessionStatusType.ACTIVE,
                   LastUpdated:  start
               );

        /// <summary>
        /// A charge detail record of the given party, its costs without zeros
        /// at their ends - see ATariff.
        /// </summary>
        private static CDR ACDR(Party_Idv3 PartyId, String Id)

            => new (
                   CountryCode:      PartyId.CountryCode,
                   PartyId:          PartyId.PartyId,
                   Id:               CDR_Id.Parse(Id),
                   Start:            start,
                   End:              start + TimeSpan.FromMinutes(30),
                   CDRToken:         ACDRToken(),
                   AuthMethod:       AuthMethod.AUTH_REQUEST,
                   Location:         new CDRLocation(
                                         Location_Id.     Parse("LOCATION0001"),
                                         "Biberweg 18",
                                         "Jena",
                                         Country.Germany,
                                         GeoCoordinate.   Parse(50.9, 11.6),
                                         EVSE_UId.        Parse("EVSE0001"),
                                         EVSE_Id.         Parse("DE*XYZ*E0001*1"),
                                         Connector_Id.    Parse("1"),
                                         ConnectorType.   IEC_62196_T2,
                                         ConnectorFormats.SOCKET,
                                         PowerTypes.      AC_3_PHASE,
                                         "Charging Station",
                                         "07749"
                                     ),
                   Currency:         Currency.EUR,
                   ChargingPeriods:  [
                                         ChargingPeriod.Create(
                                             start,
                                             [ CDRDimension.Create(CDRDimensionType.ENERGY, 1.33M) ]
                                         )
                                     ],
                   TotalCosts:       new Price(10.5M, 11.6M),
                   TotalEnergy:      WattHour.FromKWh(1.33M),
                   TotalTime:        TimeSpan.FromMinutes(30),
                   LastUpdated:      start
               );

        /// <summary>
        /// The token a session or a CDR was charged with.
        /// </summary>
        private static CDRToken ACDRToken()

            => new (
                   ourPartyId.CountryCode,
                   ourPartyId.PartyId,
                   Token_Id.   Parse("TOKEN0001"),
                   TokenType.  RFID,
                   Contract_Id.Parse("DE-GEF-C12345678-X")
               );

        #endregion

        #region (private static) AsJSON / AsText

        private static String AsJSON(Location Location)  => Location.ToJSON().ToString(Newtonsoft.Json.Formatting.None);
        private static String AsJSON(Tariff   Tariff)    => Tariff.  ToJSON().ToString(Newtonsoft.Json.Formatting.None);
        private static String AsJSON(Session  Session)   => Session. ToJSON().ToString(Newtonsoft.Json.Formatting.None);
        private static String AsJSON(CDR      CDR)       => CDR.     ToJSON().ToString(Newtonsoft.Json.Formatting.None);

        /// <summary>
        /// All a party says, as one text.
        /// </summary>
        private static String AsText(PartyData Party)

            => $"{Party.Id} {Party.Role} {Party.AllowDowngrades} {Party.BusinessDetails.ToJSON().ToString(Newtonsoft.Json.Formatting.None)}";

        #endregion

        #region (private) NextStart() / ANode()

        /// <summary>
        /// The next start of a node: the Common API so far disposed - its assets
        /// go through a queue, and are all in their file once it is - and a
        /// Common API and an EMSP API made anew on the same directory, which
        /// read back what they wrote. The new EMSP API is returned.
        /// </summary>
        private async Task<EMSP_HTTPAPI> NextStart()
        {

            await api.BaseAPI.DisposeAsync();

            (api, emsp) = ANode();

            return emsp;

        }

        /// <summary>
        /// A Common API and an EMSP API on this test's directory. Made anew,
        /// they read back what the ones before them wrote, as the next start
        /// of a node does.
        /// </summary>
        private (CommonAPI, EMSP_HTTPAPI) ANode()
        {

            var commonAPI = new CommonAPI(

                                OurPartyData:      [
                                                       new PartyData(
                                                           ourPartyId,
                                                           Role.EMSP,
                                                           new BusinessDetails("GraphDefined EMSP")
                                                       )
                                                   ],
                                DefaultPartyId:    ourPartyId,

                                BaseAPI:           new CommonHTTPAPI(
                                                       HTTPAPI:          new HTTPExtAPI(
                                                                             HTTPServer: new HTTPServer(TCPPort: IPPort.Parse(3999))
                                                                         ),
                                                       OurBaseURL:       URL.Parse("http://127.0.0.1:3999/ocpi"),
                                                       OurVersionsURL:   URL.Parse("http://127.0.0.1:3999/ocpi/versions"),
                                                       RootPath:         HTTPPath.Parse("/ocpi"),
                                                       DisableLogging:   true,
                                                       LoggingPath:      directory
                                                   ),

                                URLPathPrefix:     HTTPPath.Parse("/ocpi/v2.2.1"),
                                DatabaseFilePath:  directory,
                                DisableLogging:    true,
                                LoggingPath:       directory

                            );

            return (commonAPI,
                    new EMSP_HTTPAPI(
                        CommonAPI:       commonAPI,
                        DisableLogging:  true,
                        LoggingPath:     directory
                    ));

        }

        #endregion

    }

}
