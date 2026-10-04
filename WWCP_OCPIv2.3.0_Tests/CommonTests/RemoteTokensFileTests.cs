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
    /// The tokens an EMSP pushes to a CPO are the EMSP's, apart from the CPO's
    /// own parties, and are there at the next start - one EMSP's apart from
    /// another's.
    /// </summary>
    /// <remarks>
    /// The next start is a Common API made anew on the same directory, which
    /// reads back the remote parties first and the assets after them, as a
    /// node's start does. Nothing here listens: the HTTP server is made and
    /// never started.
    /// </remarks>
    [TestFixture]
    public class RemoteTokensFileTests
    {

        #region Data

        private static readonly Party_Idv3      ourPartyId  = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));
        private static readonly Party_Idv3      emspAId     = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("AAA"));
        private static readonly Party_Idv3      emspBId     = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("BBB"));

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


        #region TheTokensOfTwoEMSPsAreKeptApartAndAreThereAtTheNextStart()

        [Test]
        public async Task TheTokensOfTwoEMSPsAreKeptApartAndAreThereAtTheNextStart()
        {

            var registeredA  = await api.AddRemoteParty(RemoteParty_Id.Parse("DE-AAA_EMSP"), RolesOf(emspAId), AccessToken.NewRandom());
            var registeredB  = await api.AddRemoteParty(RemoteParty_Id.Parse("DE-BBB_EMSP"), RolesOf(emspBId), AccessToken.NewRandom());

            // The same uid at both: tokens are a party's, not the CPO's.
            var pushedA      = await api.AddOrUpdateToken(AToken(emspAId, "0123456789ABCDEF"), AllowedType.ALLOWED);
            var pushedB      = await api.AddOrUpdateToken(AToken(emspBId, "0123456789ABCDEF"), AllowedType.BLOCKED);

            Assert.Multiple(() => {
                Assert.That(registeredA.IsSuccess, Is.True, $"EMSP A could not be registered: {registeredA.ErrorResponse}");
                Assert.That(registeredB.IsSuccess, Is.True, $"EMSP B could not be registered: {registeredB.ErrorResponse}");
                Assert.That(pushedA.    IsSuccess, Is.True, $"EMSP A's token was refused: {pushedA.ErrorResponse}");
                Assert.That(pushedB.    IsSuccess, Is.True, $"EMSP B's token was refused: {pushedB.ErrorResponse}");
            });

            await api.BaseAPI.DisposeAsync();

            api = ACommonAPI();

            Assert.Multiple(() => {

                Assert.That(api.GetTokenStatus(emspAId).Select(status => $"{status.Token.Id} {status.Status}"), Is.EquivalentTo(new[] { "0123456789ABCDEF ALLOWED" }),
                            "At the next start EMSP A's token is not as it was pushed.");

                Assert.That(api.GetTokenStatus(emspBId).Select(status => $"{status.Token.Id} {status.Status}"), Is.EquivalentTo(new[] { "0123456789ABCDEF BLOCKED" }),
                            "At the next start EMSP B's token is not as it was pushed.");

                Assert.That(api.GetTokenStatus().Count(), Is.EqualTo(2),
                            "The tokens in all are not both.");

                Assert.That(api.Parties.Select(party => party.Id), Is.EquivalentTo(new[] { ourPartyId }),
                            "At the next start an EMSP is one of the CPO's own parties.");

            });

        }

        #endregion

        #region APatchOfOneEMSPsTokenLeavesTheOthersOfTheSameUid()

        /// <summary>
        /// A patch is an update, and an update found the token it replaced
        /// unequal to itself - Token.Equals compared the country code and the
        /// party id with the uid - so no token could be updated at all.
        /// </summary>
        [Test]
        public async Task APatchOfOneEMSPsTokenLeavesTheOthersOfTheSameUid()
        {

            await api.AddRemoteParty(RemoteParty_Id.Parse("DE-AAA_EMSP"), RolesOf(emspAId), AccessToken.NewRandom());
            await api.AddRemoteParty(RemoteParty_Id.Parse("DE-BBB_EMSP"), RolesOf(emspBId), AccessToken.NewRandom());

            await api.AddOrUpdateToken(AToken(emspAId, "0123456789ABCDEF"), AllowedType.ALLOWED);
            await api.AddOrUpdateToken(AToken(emspBId, "0123456789ABCDEF"), AllowedType.ALLOWED);

            var patched = await api.TryPatchToken(
                                    emspBId,
                                    Token_Id.Parse("0123456789ABCDEF"),
                                    new Newtonsoft.Json.Linq.JObject(
                                        new Newtonsoft.Json.Linq.JProperty("valid",         false),
                                        new Newtonsoft.Json.Linq.JProperty("last_updated",  (start + TimeSpan.FromHours(1)).ToISO8601())
                                    )
                                );

            Assert.Multiple(() => {
                Assert.That(patched.IsSuccess,                                          Is.True, $"EMSP B's token could not be patched: {patched.ErrorResponse}");
                Assert.That(api.GetTokenStatus(emspAId).Single().Token.IsValid,         Is.True, "EMSP A's token was patched with EMSP B's.");
                Assert.That(api.GetTokenStatus(emspBId).Single().Token.IsValid,         Is.False, "EMSP B's token was not patched.");
            });

        }

        #endregion

        #region ATokenOfAPartyNobodyRegisteredIsRefused()

        [Test]
        public async Task ATokenOfAPartyNobodyRegisteredIsRefused()
        {

            var pushed = await api.AddOrUpdateToken(AToken(emspAId, "0123456789ABCDEF"), AllowedType.ALLOWED);

            Assert.Multiple(() => {
                Assert.That(pushed.IsSuccess,              Is.False, "The token of a party nobody registered was taken.");
                Assert.That(api.GetTokenStatus(emspAId),   Is.Empty, "The token of a party nobody registered is kept.");
            });

        }

        #endregion


        #region (private static) RolesOf(PartyId) / AToken(PartyId, TokenId)

        /// <summary>
        /// The roles of an EMSP.
        /// </summary>
        private static IEnumerable<CredentialsRole> RolesOf(Party_Idv3 PartyId)

            => [
                   new CredentialsRole(
                       PartyId.CountryCode,
                       PartyId.PartyId,
                       Role.EMSP,
                       new BusinessDetails($"EMSP {PartyId.PartyId}")
                   )
               ];

        /// <summary>
        /// An RFID token of the given party.
        /// </summary>
        private static Token AToken(Party_Idv3 PartyId, String TokenId)

            => new (
                   PartyId.CountryCode,
                   PartyId.PartyId,
                   Token_Id.Parse(TokenId),
                   TokenType.RFID,
                   Contract_Id.Parse($"{PartyId.CountryCode}-{PartyId.PartyId}-C12345678-X"),
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
