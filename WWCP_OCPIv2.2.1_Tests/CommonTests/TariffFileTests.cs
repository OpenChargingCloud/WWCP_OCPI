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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.CommonTests
{

    /// <summary>
    /// A tariff removed is still removed at the next start.
    /// </summary>
    /// <remarks>
    /// <para>
    /// RemoveTariff writes down every version of the tariff it removed, as an
    /// array, and the next start looked for an object only: the tariff
    /// removed was there again.
    /// </para>
    /// <para>
    /// The next start is a Common API made anew on the same directory, which
    /// reads the files back as a node's start does - see RemovedAtOnceTests.
    /// Nothing here listens: the HTTP servers are made and never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class TariffFileTests
    {

        #region Data

        private static readonly CountryCode     countryCode  = CountryCode.Parse("DE");
        private static readonly Party_Id        partyId      = Party_Id.   Parse("GEF");
        private static readonly Party_Idv3      ourPartyId   = Party_Idv3. From(countryCode, partyId);

        /// <summary>
        /// When the tariffs were last updated: a time of their own, so that
        /// none of them reads the clock.
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


        #region ATariffRemovedIsGoneAtTheNextStart()

        [Test]
        public async Task ATariffRemovedIsGoneAtTheNextStart()
        {

            var removed  = ATariff("TARIFF0001");
            var kept     = ATariff("TARIFF0002");

            var added1   = await api.AddTariff(removed);
            var added2   = await api.AddTariff(kept);
            var removal  = await api.RemoveTariff(ourPartyId, removed.Id);

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"The tariff to remove could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The tariff to keep could not be added: {added2.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The tariff could not be removed: {removal.ErrorResponse}");
            });

            Assert.That(IdsOf((await NextStart()).GetTariffs()), Is.EquivalentTo(new[] { "TARIFF0002" }),
                        "The next start knows the tariff removed, or not the one kept.");

        }

        #endregion

        #region EveryTariffMatchedIsGoneAtTheNextStart()

        /// <summary>
        /// Every tariff a filter matched is gone at the next start, and the one
        /// it did not match is there.
        /// </summary>
        [Test]
        public async Task EveryTariffMatchedIsGoneAtTheNextStart()
        {

            var added1   = await api.AddTariff(ATariff("TARIFF0001"));
            var added2   = await api.AddTariff(ATariff("TARIFF0002"));
            var added3   = await api.AddTariff(ATariff("TARIFF0003"));
            var removal  = await api.RemoveAllTariffs((Tariff tariff) => tariff.Id.ToString() != "TARIFF0003");

            Assert.Multiple(() => {
                Assert.That(added1. IsSuccess, Is.True, $"The first tariff could not be added: {added1.ErrorResponse}");
                Assert.That(added2. IsSuccess, Is.True, $"The second tariff could not be added: {added2.ErrorResponse}");
                Assert.That(added3. IsSuccess, Is.True, $"The third tariff could not be added: {added3.ErrorResponse}");
                Assert.That(removal.IsSuccess, Is.True, $"The tariffs matched could not be removed: {removal.ErrorResponse}");
            });

            Assert.That(IdsOf((await NextStart()).GetTariffs()), Is.EquivalentTo(new[] { "TARIFF0003" }),
                        "The next start knows a tariff matched and removed, or not the one kept.");

        }

        #endregion


        #region (private static) ATariff(Id) / IdsOf(Tariffs)

        /// <summary>
        /// A tariff of this Common API's own party.
        /// </summary>
        private static Tariff ATariff(String Id)

            => new (
                   CountryCode:     countryCode,
                   PartyId:         partyId,
                   Id:              Tariff_Id.Parse(Id),
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
        /// The identifications of the given tariffs, as text.
        /// </summary>
        private static IEnumerable<String> IdsOf(IEnumerable<Tariff> Tariffs)

            => Tariffs.Select(tariff => tariff.Id.ToString()).ToArray();

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

                   OurPartyData:        [
                                            new PartyData(
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

                   URLPathPrefix:       HTTPPath.Parse("/ocpi/v2.2.1"),
                   DatabaseFilePath:    directory,
                   DisableLogging:      true,
                   LoggingPath:         directory

               );

        #endregion

    }

}
