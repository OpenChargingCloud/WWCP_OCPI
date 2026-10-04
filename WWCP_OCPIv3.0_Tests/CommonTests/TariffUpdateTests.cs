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

namespace cloud.charging.open.protocols.OCPIv3_0.UnitTests.CommonTests
{

    /// <summary>
    /// A tariff patched is the new one afterwards, also at the next start.
    /// </summary>
    /// <remarks>
    /// TryPatchTariff handed its ConcurrentDictionary the patched tariff as
    /// the one to compare with, so nothing was stored, and wrote down the
    /// tariff as it was before: the patch said it was done, and was nowhere.
    ///
    /// The next start is a Common API made anew on the same directory, which
    /// reads back what it wrote, as a node's start does. Nothing here listens:
    /// the HTTP server is made and never started.
    /// </remarks>
    [TestFixture]
    public class TariffUpdateTests
    {

        #region Data

        private static readonly Party_Idv3      ourPartyId   = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));
        private static readonly Tariff_Id       tariffId     = Tariff_Id.Parse("TARIFF0001");

        /// <summary>
        /// When the tariff was last updated before the patch: a time of its
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


        #region ATariffPatchedIsTheNewOneAlsoAtTheNextStart()

        [Test]
        public async Task ATariffPatchedIsTheNewOneAlsoAtTheNextStart()
        {

            var tariff  = ATariff();

            var party   = await api.AddParty(ourPartyId, Role.CPO, new BusinessDetails("GraphDefined CSO"));
            var added   = await api.AddOrUpdateTariff(tariff);

            Assert.Multiple(() => {
                Assert.That(party.IsSuccess, Is.True, $"Our party could not be added: {party.ErrorResponse}");
                Assert.That(added.IsSuccess, Is.True, $"The tariff could not be added: {added.ErrorResponse}");
            });

            var patched = await api.TryPatchTariff(
                                    tariff,
                                    new JObject(
                                        new JProperty("currency",      "USD"),
                                        new JProperty("last_updated",  (start + TimeSpan.FromHours(1)).ToISO8601())
                                    )
                                );

            var now     = api.TryGetTariff(ourPartyId, tariffId, out var tariffNow) ? tariffNow.Currency.ISOCode : "none";

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            Assert.Multiple(() => {
                Assert.That(patched.IsSuccess, Is.True, $"The tariff could not be patched: {patched.ErrorResponse}");
                Assert.That(now, Is.EqualTo("USD"), "The tariff patched is still the old one.");
                Assert.That(api.TryGetTariff(ourPartyId, tariffId, out var tariffNext) ? tariffNext.Currency.ISOCode : "none", Is.EqualTo("USD"),
                            "At the next start the tariff patched is the old one.");
            });

        }

        #endregion


        #region (private static) ATariff()

        /// <summary>
        /// The one tariff of these tests, in euros.
        /// </summary>
        private static Tariff ATariff()

            => new (
                   PartyId:         ourPartyId,
                   Id:              tariffId,
                   VersionId:       1,
                   Currency:        Currency.EUR,
                   TariffElements:  [
                                        new TariffElement(
                                            PriceComponent.ChargingTime(
                                                2.5M,
                                                0.1M,
                                                TimeSpan.FromSeconds(300)
                                            )
                                        )
                                    ],
                   LastUpdated:     start
               );

        #endregion

        #region (private) ACommonAPI()

        /// <summary>
        /// The Common API of a CPO on this test's directory. Made anew, it
        /// reads back what the one before it wrote, as the next start of a
        /// node does.
        /// </summary>
        private CommonAPI ACommonAPI()

            => new (

                   OurCredentialRoles:  [
                                            new CredentialsRole(
                                                ourPartyId,
                                                Role.CPO,
                                                new BusinessDetails("GraphDefined CSO")
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

                   URLPathPrefix:       HTTPPath.Parse("/ocpi/v3.0"),
                   DatabaseFilePath:    directory,
                   DisableLogging:      true,
                   LoggingPath:         directory

               );

        #endregion

    }

}
