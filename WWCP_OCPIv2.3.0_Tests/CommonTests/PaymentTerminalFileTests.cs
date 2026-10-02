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

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.CommonTests
{

    /// <summary>
    /// A payment terminal is there at the next start as it was last written
    /// down: added or changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Every change of a payment terminal was written to the file of the
    /// assets, and the next start passed over every one of them: no payment
    /// terminal was there any more once the process had ended.
    /// </para>
    /// <para>
    /// The next start is a Common API made anew on the same directory, which
    /// reads the files back as a node's start does - see RemovedAtOnceTests.
    /// Nothing here listens: the HTTP servers are made and never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class PaymentTerminalFileTests
    {

        #region Data

        private static readonly CountryCode     countryCode  = CountryCode.Parse("DE");
        private static readonly Party_Id        partyId      = Party_Id.   Parse("GEF");
        private static readonly Party_Idv3      ourPartyId   = Party_Idv3. From(countryCode, partyId);

        /// <summary>
        /// When the payment terminals were last updated, unless said otherwise:
        /// a time of their own, so that none of them reads the clock.
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


        #region APaymentTerminalAddedIsThereAtTheNextStart()

        [Test]
        public async Task APaymentTerminalAddedIsThereAtTheNextStart()
        {

            var terminal  = ATerminal("Biberweg 18");

            var created   = await api.AddOrUpdatePaymentTerminal(terminal);

            Assert.That(created.WasCreated, Is.True, $"The payment terminal was not created: {created.ErrorResponse}");

            Assert.That(AsJSON((await NextStart()).GetPaymentTerminals()), Is.EquivalentTo(AsJSON([ terminal ])),
                        "The next start does not know the payment terminal added.");

        }

        #endregion

        #region APaymentTerminalAddedOrUpdatedIsThereAtTheNextStartAsLastWritten()

        [Test]
        public async Task APaymentTerminalAddedOrUpdatedIsThereAtTheNextStartAsLastWritten()
        {

            var terminal  = ATerminal("Biberweg 18");
            var changed   = ATerminal("Biberweg 19", terminal.LastUpdated + TimeSpan.FromHours(1));

            var created   = await api.AddOrUpdatePaymentTerminal(terminal);
            var updated   = await api.AddOrUpdatePaymentTerminal(changed);

            Assert.Multiple(() => {
                Assert.That(created.WasCreated, Is.True, $"The payment terminal was not created: {created.ErrorResponse}");
                Assert.That(updated.WasUpdated, Is.True, $"The payment terminal was not updated: {updated.ErrorResponse}");
            });

            Assert.That(AsJSON((await NextStart()).GetPaymentTerminals()), Is.EquivalentTo(AsJSON([ changed ])),
                        "The next start does not know the payment terminal as it was last written.");

        }

        #endregion

        #region APaymentTerminalUpdatedIsThereAtTheNextStartAsUpdated()

        [Test]
        public async Task APaymentTerminalUpdatedIsThereAtTheNextStartAsUpdated()
        {

            var terminal  = ATerminal("Biberweg 18");
            var changed   = ATerminal("Biberweg 19", terminal.LastUpdated + TimeSpan.FromHours(1));

            var created   = await api.AddOrUpdatePaymentTerminal(terminal);
            var updated   = await api.UpdatePaymentTerminal     (changed);

            Assert.Multiple(() => {
                Assert.That(created.WasCreated, Is.True, $"The payment terminal was not created: {created.ErrorResponse}");
                Assert.That(updated.IsSuccess,  Is.True, $"The payment terminal could not be updated: {updated.ErrorResponse}");
            });

            Assert.That(AsJSON((await NextStart()).GetPaymentTerminals()), Is.EquivalentTo(AsJSON([ changed ])),
                        "The next start does not know the payment terminal as it was updated.");

        }

        #endregion


        #region (private static) ATerminal(Address, LastUpdated = null)

        /// <summary>
        /// A payment terminal of this Common API's own party, which it keeps
        /// payment terminals for, at the given address.
        /// </summary>
        private static Terminal ATerminal(String           Address,
                                          DateTimeOffset?  LastUpdated   = null)

            => new (
                   Id:           Terminal_Id.Parse("TERMINAL0001"),
                   CountryCode:  countryCode,
                   PartyId:      partyId,
                   Address:      Address,
                   City:         "Jena",
                   PostalCode:   "07749",
                   LastUpdated:  LastUpdated ?? start
               );

        #endregion

        #region (private static) AsJSON(Terminals)

        /// <summary>
        /// Payment terminals as their JSON, compared with all they say.
        /// </summary>
        private static IEnumerable<String> AsJSON(IEnumerable<Terminal> Terminals)

            => Terminals.Select(terminal => terminal.ToJSON().ToString(Newtonsoft.Json.Formatting.None));

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

                   OurPartyData:      [
                                          new PartyData(
                                              ourPartyId,
                                              Role.CPO,
                                              new BusinessDetails("GraphDefined CSO")
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

                   URLPathPrefix:     HTTPPath.Parse("/ocpi/v2.3.0"),
                   DatabaseFilePath:  directory,
                   DisableLogging:    true,
                   LoggingPath:       directory

               );

        #endregion

    }

}
