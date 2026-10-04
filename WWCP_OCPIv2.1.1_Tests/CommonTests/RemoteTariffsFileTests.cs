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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.CommonTests
{

    /// <summary>
    /// The tariffs two CPOs push to an EMSP are each one's own, also where
    /// they have the same id, and are there at the next start.
    /// </summary>
    /// <remarks>
    /// The Common API kept its tariffs by their id alone: the second CPO's
    /// tariff of an id replaced the first one's, and a PATCH or a removal of
    /// one CPO's tariff could reach the other's.
    ///
    /// The next start is a Common API made anew on the same directory, which
    /// reads back what it wrote, as a node's start does. Nothing here listens:
    /// the HTTP server is made and never started.
    /// </remarks>
    [TestFixture]
    public class RemoteTariffsFileTests
    {

        #region Data

        private static readonly CountryCode     de          = CountryCode.Parse("DE");
        private static readonly Party_Id        cpoA        = Party_Id.Parse("AAA");
        private static readonly Party_Id        cpoB        = Party_Id.Parse("BBB");
        private static readonly Tariff_Id       id          = Tariff_Id.Parse("TARIFF0001");

        /// <summary>
        /// When the tariffs were last updated: a time of their own, so that
        /// they do not read the clock.
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


        #region TheTariffsOfTwoCPOsOfOneIdAreKeptApartAndAreThereAtTheNextStart()

        [Test]
        public async Task TheTariffsOfTwoCPOsOfOneIdAreKeptApartAndAreThereAtTheNextStart()
        {

            var pushedA = await api.AddOrUpdateTariff(ATariff(cpoA, 1.11M));
            var pushedB = await api.AddOrUpdateTariff(ATariff(cpoB, 2.22M));

            Assert.Multiple(() => {
                Assert.That(pushedA.IsSuccess, Is.True, $"CPO A's tariff was refused: {pushedA.ErrorResponse}");
                Assert.That(pushedB.IsSuccess, Is.True, $"CPO B's tariff was refused: {pushedB.ErrorResponse}");
            });

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            Assert.Multiple(() => {

                Assert.That(api.GetTariffs(de, cpoA).Select(tariff => $"{tariff.Id} {PriceOf(tariff)}"), Is.EquivalentTo(new[] { "TARIFF0001 1.11" }),
                            "At the next start CPO A's tariff is not as it was pushed.");

                Assert.That(api.GetTariffs(de, cpoB).Select(tariff => $"{tariff.Id} {PriceOf(tariff)}"), Is.EquivalentTo(new[] { "TARIFF0001 2.22" }),
                            "At the next start CPO B's tariff is not as it was pushed.");

                Assert.That(api.GetTariffs().Count(), Is.EqualTo(2), "The tariffs in all are not both.");

                Assert.That(api.TryGetTariff(de, cpoA, id, out var a) ? PriceOf(a) : "none", Is.EqualTo("1.11"));
                Assert.That(api.TryGetTariff(de, cpoB, id, out var b) ? PriceOf(b) : "none", Is.EqualTo("2.22"));

            });

        }

        #endregion

        #region APatchOfOneCPOsTariffLeavesTheOthersOfTheSameId()

        [Test]
        public async Task APatchOfOneCPOsTariffLeavesTheOthersOfTheSameId()
        {

            await api.AddOrUpdateTariff(ATariff(cpoA, 1.11M));
            await api.AddOrUpdateTariff(ATariff(cpoB, 2.22M));

            var patched = await api.TryPatchTariff(
                                    de,
                                    cpoB,
                                    id,
                                    new JObject(
                                        new JProperty("currency",      "USD"),
                                        new JProperty("last_updated",  (start + TimeSpan.FromHours(1)).ToISO8601())
                                    )
                                );

            Assert.Multiple(() => {
                Assert.That(patched.IsSuccess, Is.True, $"CPO B's tariff could not be patched: {patched.ErrorResponse}");
                Assert.That(api.TryGetTariff(de, cpoA, id, out var a) ? a.Currency.ISOCode : "none", Is.EqualTo("EUR"), "CPO A's tariff was patched with CPO B's.");
                Assert.That(api.TryGetTariff(de, cpoB, id, out var b) ? b.Currency.ISOCode : "none", Is.EqualTo("USD"), "CPO B's tariff was not patched.");
            });

        }

        #endregion

        #region ATariffRemovedIsOnlyThatPartys()

        [Test]
        public async Task ATariffRemovedIsOnlyThatPartys()
        {

            await api.AddOrUpdateTariff(ATariff(cpoA, 1.11M));
            await api.AddOrUpdateTariff(ATariff(cpoB, 2.22M));

            var removed = await api.RemoveTariff(ATariff(cpoA, 1.11M));

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            Assert.Multiple(() => {
                Assert.That(removed.IsSuccess,                  Is.True,  $"CPO A's tariff could not be removed: {removed.ErrorResponse}");
                Assert.That(api.TariffExists(de, cpoA, id),     Is.False, "CPO A's tariff is still there.");
                Assert.That(api.TariffExists(de, cpoB, id),     Is.True,  "CPO B's tariff went with CPO A's.");
            });

        }

        #endregion


        #region (private static) ATariff(PartyId, Price) / PriceOf(Tariff)

        /// <summary>
        /// A tariff of the given party, of the one id of these tests, with the
        /// given price, so that the two can be told apart.
        /// </summary>
        private static Tariff ATariff(Party_Id PartyId, Decimal Price)

            => new (
                   CountryCode:     de,
                   PartyId:         PartyId,
                   Id:              id,
                   Currency:        Currency.EUR,
                   TariffElements:  [
                                        new TariffElement(
                                            [
                                                PriceComponent.ChargingTime(
                                                    Price,
                                                    TimeSpan.FromSeconds(300)
                                                )
                                            ]
                                        )
                                    ],
                   LastUpdated:     start
               );

        /// <summary>
        /// The price of the given tariff, the same in every culture.
        /// </summary>
        private static String PriceOf(Tariff Tariff)

            => Tariff.TariffElements.First().PriceComponents.First().Price.ToString(CultureInfo.InvariantCulture);

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
