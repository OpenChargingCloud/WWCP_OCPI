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

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.CommonTests
{

    /// <summary>
    /// The tokens two EMSPs push to a CPO are each one's own, also where they
    /// have the same uid, and are there at the next start.
    /// </summary>
    /// <remarks>
    /// The Common API kept its tokens by their uid alone: the second EMSP's
    /// token of a uid replaced the first one's, and a PATCH of one EMSP's
    /// token could change the other's.
    ///
    /// The next start is a Common API made anew on the same directory, which
    /// reads back what it wrote, as a node's start does. Nothing here listens:
    /// the HTTP server is made and never started.
    /// </remarks>
    [TestFixture]
    public class RemoteTokensFileTests
    {

        #region Data

        private static readonly CountryCode     de          = CountryCode.Parse("DE");
        private static readonly Party_Id        emspA       = Party_Id.Parse("AAA");
        private static readonly Party_Id        emspB       = Party_Id.Parse("BBB");
        private static readonly Token_Id        uid         = Token_Id.Parse("0123456789ABCDEF");

        /// <summary>
        /// When the tokens were last updated: a time of their own, so that
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


        #region TheTokensOfTwoEMSPsOfOneUidAreKeptApartAndAreThereAtTheNextStart()

        [Test]
        public async Task TheTokensOfTwoEMSPsOfOneUidAreKeptApartAndAreThereAtTheNextStart()
        {

            var pushedA = await api.AddOrUpdateToken(AToken(emspA), AllowedType.ALLOWED);
            var pushedB = await api.AddOrUpdateToken(AToken(emspB), AllowedType.BLOCKED);

            Assert.Multiple(() => {
                Assert.That(pushedA.IsSuccess, Is.True, $"EMSP A's token was refused: {pushedA.ErrorResponse}");
                Assert.That(pushedB.IsSuccess, Is.True, $"EMSP B's token was refused: {pushedB.ErrorResponse}");
            });

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            Assert.Multiple(() => {

                Assert.That(api.GetTokenStatus(de, emspA).Select(status => $"{status.Token.Id} {status.Status}"), Is.EquivalentTo(new[] { "0123456789ABCDEF ALLOWED" }),
                            "At the next start EMSP A's token is not as it was pushed.");

                Assert.That(api.GetTokenStatus(de, emspB).Select(status => $"{status.Token.Id} {status.Status}"), Is.EquivalentTo(new[] { "0123456789ABCDEF BLOCKED" }),
                            "At the next start EMSP B's token is not as it was pushed.");

                Assert.That(api.GetTokenStatus().Count(), Is.EqualTo(2), "The tokens in all are not both.");

                Assert.That(api.TryGetTokenStatus(de, emspA, uid, out var a) ? a.Status.ToString() : "none", Is.EqualTo("ALLOWED"));
                Assert.That(api.TryGetTokenStatus(de, emspB, uid, out var b) ? b.Status.ToString() : "none", Is.EqualTo("BLOCKED"));

            });

        }

        #endregion

        #region APatchOfOneEMSPsTokenLeavesTheOthersOfTheSameUid()

        [Test]
        public async Task APatchOfOneEMSPsTokenLeavesTheOthersOfTheSameUid()
        {

            await api.AddOrUpdateToken(AToken(emspA), AllowedType.ALLOWED);
            await api.AddOrUpdateToken(AToken(emspB), AllowedType.ALLOWED);

            var patched = await api.TryPatchToken(
                                    de,
                                    emspB,
                                    uid,
                                    new JObject(
                                        new JProperty("valid",         false),
                                        new JProperty("last_updated",  (start + TimeSpan.FromHours(1)).ToISO8601())
                                    )
                                );

            Assert.Multiple(() => {
                Assert.That(patched.IsSuccess, Is.True, $"EMSP B's token could not be patched: {patched.ErrorResponse}");
                Assert.That(api.TryGetTokenStatus(de, emspA, uid, out var a) && a.Token.IsValid,  Is.True,  "EMSP A's token was patched with EMSP B's.");
                Assert.That(api.TryGetTokenStatus(de, emspB, uid, out var b) && !b.Token.IsValid, Is.True,  "EMSP B's token was not patched.");
            });

        }

        #endregion

        #region ATokenRemovedIsOnlyThatPartys()

        [Test]
        public async Task ATokenRemovedIsOnlyThatPartys()
        {

            await api.AddOrUpdateToken(AToken(emspA), AllowedType.ALLOWED);
            await api.AddOrUpdateToken(AToken(emspB), AllowedType.ALLOWED);

            var removed = await api.RemoveToken(AToken(emspA));

            Assert.Multiple(() => {
                Assert.That(removed.IsSuccess,                    Is.True,  $"EMSP A's token could not be removed: {removed.ErrorResponse}");
                Assert.That(api.TokenExists(de, emspA, uid),      Is.False, "EMSP A's token is still there.");
                Assert.That(api.TokenExists(de, emspB, uid),      Is.True,  "EMSP B's token went with EMSP A's.");
            });

        }

        #endregion


        #region (private static) AToken(PartyId)

        /// <summary>
        /// An RFID token of the given party, of the one uid of these tests.
        /// </summary>
        private static Token AToken(Party_Id PartyId)

            => new (
                   de,
                   PartyId,
                   uid,
                   TokenType.RFID,
                   Auth_Id.Parse($"DE-{PartyId}-C12345678"),
                   "GraphDefined CA",
                   true,
                   WhitelistType.NEVER,
                   LastUpdated: start
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

                   OurBusinessDetails:  new BusinessDetails("GraphDefined CSO"),
                   OurCountryCode:      CountryCode.Parse("DE"),
                   OurPartyId:          Party_Id.   Parse("GEF"),
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
