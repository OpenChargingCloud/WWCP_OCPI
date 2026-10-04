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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.CommonTests
{

    /// <summary>
    /// A tariff put again, updated or patched is the new one afterwards - in
    /// the Common API, and in what an EMSP and a hub keep of a remote CPO.
    /// </summary>
    /// <remarks>
    /// All of them called TimeRangeDictionary.TryUpdate as a
    /// ConcurrentDictionary's, (key, new, existing); it takes the version to
    /// replace first. The existing tariff was put in its own place: no update
    /// changed anything, though every one of them said it had.
    ///
    /// Nothing here listens: the HTTP servers are made and never started.
    /// </remarks>
    [TestFixture]
    public class TariffUpdateTests
    {

        #region Data

        private static readonly Party_Idv3      ourPartyId   = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));
        private static readonly Party_Idv3      remoteCPOId  = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("XYZ"));
        private static readonly RemoteParty_Id  remoteId     = RemoteParty_Id.Parse("DE-XYZ_CPO");
        private static readonly Tariff_Id       tariffId     = Tariff_Id.Parse("TARIFF0001");

        /// <summary>
        /// When the first version of the tariff was last updated: a time of its
        /// own, so that it does not read the clock.
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

        }

        [TearDown]
        public async Task TearDown()
        {

            // Its queue written out, before its directory goes.
            if (api is not null)
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


        #region ATariffPutAgainIsTheNewOne()

        [Test]
        public async Task ATariffPutAgainIsTheNewOne()
        {

            api = ACommonAPI(Role.CPO);

            await api.AddOrUpdateTariff(ATariff(ourPartyId, Currency.EUR, start));

            var updated = await api.AddOrUpdateTariff(ATariff(ourPartyId, Currency.USD, start + TimeSpan.FromHours(1)));

            Assert.Multiple(() => {
                Assert.That(updated.IsSuccess, Is.True, $"The tariff could not be put again: {updated.ErrorResponse}");
                Assert.That(api.TryGetTariff(ourPartyId, tariffId, out var tariff) ? tariff.Currency.ISOCode : "none", Is.EqualTo("USD"),
                            "The tariff put again is still the old one.");
            });

        }

        #endregion

        #region ATariffUpdatedIsTheNewOne()

        [Test]
        public async Task ATariffUpdatedIsTheNewOne()
        {

            api = ACommonAPI(Role.CPO);

            await api.AddOrUpdateTariff(ATariff(ourPartyId, Currency.EUR, start));

            var updated = await api.UpdateTariff(ATariff(ourPartyId, Currency.USD, start + TimeSpan.FromHours(1)));

            Assert.Multiple(() => {
                Assert.That(updated.IsSuccess, Is.True, $"The tariff could not be updated: {updated.ErrorResponse}");
                Assert.That(api.TryGetTariff(ourPartyId, tariffId, out var tariff) ? tariff.Currency.ISOCode : "none", Is.EqualTo("USD"),
                            "The tariff updated is still the old one.");
            });

        }

        #endregion

        #region ATariffPatchedIsTheNewOne()

        [Test]
        public async Task ATariffPatchedIsTheNewOne()
        {

            api = ACommonAPI(Role.CPO);

            await api.AddOrUpdateTariff(ATariff(ourPartyId, Currency.EUR, start));

            var patched = await api.TryPatchTariff(
                                    ourPartyId,
                                    tariffId,
                                    new JObject(
                                        new JProperty("currency",      "USD"),
                                        new JProperty("last_updated",  (start + TimeSpan.FromHours(1)).ToISO8601())
                                    )
                                );

            Assert.Multiple(() => {
                Assert.That(patched.IsSuccess, Is.True, $"The tariff could not be patched: {patched.ErrorResponse}");
                Assert.That(api.TryGetTariff(ourPartyId, tariffId, out var tariff) ? tariff.Currency.ISOCode : "none", Is.EqualTo("USD"),
                            "The tariff patched is still the old one.");
            });

        }

        #endregion


        #region ARemoteCPOsTariffPutAgainIsTheNewOneAtAnEMSP()

        [Test]
        public async Task ARemoteCPOsTariffPutAgainIsTheNewOneAtAnEMSP()
        {

            api = ACommonAPI(Role.EMSP);

            var emsp = new EMSP_HTTPAPI(
                           CommonAPI:       api,
                           DisableLogging:  true,
                           LoggingPath:     directory
                       );

            await api.AddRemoteParty(remoteId, RolesOfTheRemoteCPO, AccessToken.NewRandom());
            await emsp.AddOrUpdateRemoteTariff(ATariff(remoteCPOId, Currency.EUR, start));

            var updated = await emsp.AddOrUpdateRemoteTariff(ATariff(remoteCPOId, Currency.USD, start + TimeSpan.FromHours(1)));

            Assert.Multiple(() => {
                Assert.That(updated.IsSuccess, Is.True, $"The remote tariff could not be put again: {updated.ErrorResponse}");
                Assert.That(emsp.TryGetRemoteTariff(remoteCPOId, tariffId, out var tariff) ? tariff.Currency.ISOCode : "none", Is.EqualTo("USD"),
                            "The remote tariff put again is still the old one.");
            });

        }

        #endregion

        #region ARemoteCPOsTariffUpdatedIsTheNewOneAtAnEMSP()

        [Test]
        public async Task ARemoteCPOsTariffUpdatedIsTheNewOneAtAnEMSP()
        {

            api = ACommonAPI(Role.EMSP);

            var emsp = new EMSP_HTTPAPI(
                           CommonAPI:       api,
                           DisableLogging:  true,
                           LoggingPath:     directory
                       );

            await api.AddRemoteParty(remoteId, RolesOfTheRemoteCPO, AccessToken.NewRandom());
            await emsp.AddOrUpdateRemoteTariff(ATariff(remoteCPOId, Currency.EUR, start));

            var updated = await emsp.UpdateRemoteTariff(ATariff(remoteCPOId, Currency.USD, start + TimeSpan.FromHours(1)));

            Assert.Multiple(() => {
                Assert.That(updated.IsSuccess, Is.True, $"The remote tariff could not be updated: {updated.ErrorResponse}");
                Assert.That(emsp.TryGetRemoteTariff(remoteCPOId, tariffId, out var tariff) ? tariff.Currency.ISOCode : "none", Is.EqualTo("USD"),
                            "The remote tariff updated is still the old one.");
            });

        }

        #endregion


        #region ARemoteCPOsTariffPutAgainIsTheNewOneAtAHub()

        [Test]
        public async Task ARemoteCPOsTariffPutAgainIsTheNewOneAtAHub()
        {

            api = ACommonAPI(Role.HUB);

            var hub = new HUB_HTTPAPI(
                          api,
                          DisableLogging:  true,
                          LoggingPath:     directory
                      );

            await hub.AddRemoteCPO(new PartyData(remoteCPOId, Role.CPO, new BusinessDetails("XYZ Charging")));
            await hub.AddOrUpdateRemoteTariff(ATariff(remoteCPOId, Currency.EUR, start));

            var updated = await hub.AddOrUpdateRemoteTariff(ATariff(remoteCPOId, Currency.USD, start + TimeSpan.FromHours(1)));

            Assert.Multiple(() => {
                Assert.That(updated.IsSuccess, Is.True, $"The remote tariff could not be put again: {updated.ErrorResponse}");
                Assert.That(hub.TryGetRemoteTariff(remoteCPOId, tariffId, out var tariff) ? tariff.Currency.ISOCode : "none", Is.EqualTo("USD"),
                            "The remote tariff put again is still the old one.");
            });

        }

        #endregion

        #region ARemoteCPOsTariffUpdatedIsTheNewOneAtAHub()

        [Test]
        public async Task ARemoteCPOsTariffUpdatedIsTheNewOneAtAHub()
        {

            api = ACommonAPI(Role.HUB);

            var hub = new HUB_HTTPAPI(
                          api,
                          DisableLogging:  true,
                          LoggingPath:     directory
                      );

            await hub.AddRemoteCPO(new PartyData(remoteCPOId, Role.CPO, new BusinessDetails("XYZ Charging")));
            await hub.AddOrUpdateRemoteTariff(ATariff(remoteCPOId, Currency.EUR, start));

            var updated = await hub.UpdateRemoteTariff(ATariff(remoteCPOId, Currency.USD, start + TimeSpan.FromHours(1)));

            Assert.Multiple(() => {
                Assert.That(updated.IsSuccess, Is.True, $"The remote tariff could not be updated: {updated.ErrorResponse}");
                Assert.That(hub.TryGetRemoteTariff(remoteCPOId, tariffId, out var tariff) ? tariff.Currency.ISOCode : "none", Is.EqualTo("USD"),
                            "The remote tariff updated is still the old one.");
            });

        }

        #endregion


        #region (private static) RolesOfTheRemoteCPO / ATariff(PartyId, Currency, LastUpdated)

        /// <summary>
        /// The roles of the remote CPO.
        /// </summary>
        private static IEnumerable<CredentialsRole> RolesOfTheRemoteCPO

            => [
                   new CredentialsRole(
                       remoteCPOId.CountryCode,
                       remoteCPOId.PartyId,
                       Role.CPO,
                       new BusinessDetails("XYZ Charging")
                   )
               ];

        /// <summary>
        /// The one tariff of these tests, of the given party, in the given
        /// currency, so that its versions can be told apart.
        /// </summary>
        private static Tariff ATariff(Party_Idv3 PartyId, Currency Currency, DateTimeOffset LastUpdated)

            => new (
                   CountryCode:     PartyId.CountryCode,
                   PartyId:         PartyId.PartyId,
                   Id:              tariffId,
                   Currency:        Currency,
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
                   LastUpdated:     LastUpdated
               );

        #endregion

        #region (private) ACommonAPI(Role)

        /// <summary>
        /// A Common API of a node of the given role on this test's directory.
        /// </summary>
        private CommonAPI ACommonAPI(Role Role)

            => new (

                   OurPartyData:        [
                                            new PartyData(
                                                ourPartyId,
                                                Role,
                                                new BusinessDetails("GraphDefined")
                                            )
                                        ],
                   DefaultPartyId:      ourPartyId,

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

                   URLPathPrefix:       HTTPPath.Parse("/ocpi/v2.2.1"),
                   DatabaseFilePath:    directory,
                   DisableLogging:      true,
                   LoggingPath:         directory

               );

        #endregion

    }

}
