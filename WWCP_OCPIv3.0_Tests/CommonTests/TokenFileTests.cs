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

namespace cloud.charging.open.protocols.OCPIv3_0.UnitTests.CommonTests
{

    /// <summary>
    /// A token is there at the next start as it was added.
    /// </summary>
    /// <remarks>
    /// <para>
    /// No token came back from the file of the assets: Token.TryParse asked
    /// for "valid", which a token of 3.0 neither has nor writes - its
    /// validity is valid_from and valid_until. And it read valid_until as an
    /// enumeration, which a time is not, so that a token with an end could
    /// not be read at all.
    /// </para>
    /// <para>
    /// The next start is a Common API made anew on the same directory, which
    /// reads the files back as a node's start does - see PartyFileTests.
    /// Nothing here listens: the HTTP servers are made and never started.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class TokenFileTests
    {

        #region Data

        private static readonly Party_Idv3      ourPartyId  = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));

        /// <summary>
        /// When the tokens begin and were last updated: a time of their own,
        /// so that none of them reads the clock.
        /// </summary>
        private static readonly DateTimeOffset  start       = new (2026, 10, 1, 8, 0, 0, TimeSpan.Zero);

        private String     directory  = default!;
        private CommonAPI  api        = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public async Task SetUp()
        {

            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_Tests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(directory);

            api        = ACommonAPI();

            // A token is filed under its party, and in 3.0 no party is given
            // at the start: every one is added, and is there at the next
            // start since 2bf8aa31.
            var party  = await api.AddParty(ourPartyId, Role.EMSP, new BusinessDetails("GraphDefined EMSP"));

            Assert.That(party.IsSuccess, Is.True, $"Our party could not be added: {party.ErrorResponse}");

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


        #region ATokenAddedIsThereAtTheNextStart()

        [Test]
        public async Task ATokenAddedIsThereAtTheNextStart()
        {

            var token  = AToken();

            var added  = await api.AddToken(token);

            Assert.That(added.IsSuccess, Is.True, $"The token could not be added: {added.ErrorResponse}");

            Assert.That(AsJSON((await NextStart()).GetTokens(ourPartyId)), Is.EquivalentTo(AsJSON([ token ])),
                        "The next start does not know the token added.");

        }

        #endregion

        #region ATokenWithAnEndIsThereAtTheNextStart()

        [Test]
        public async Task ATokenWithAnEndIsThereAtTheNextStart()
        {

            var token  = AToken(ValidUntil: start + TimeSpan.FromDays(365));

            var added  = await api.AddToken(token);

            Assert.That(added.IsSuccess, Is.True, $"The token could not be added: {added.ErrorResponse}");

            Assert.That(AsJSON((await NextStart()).GetTokens(ourPartyId)), Is.EquivalentTo(AsJSON([ token ])),
                        "The next start does not know the token with an end.");

        }

        #endregion


        #region (private static) AToken(ValidUntil = null)

        /// <summary>
        /// A token of our party, valid from the start on - and until the given
        /// time, if one is given.
        /// </summary>
        private static Token AToken(DateTimeOffset? ValidUntil = null)

            => new (
                   PartyId:      ourPartyId,
                   Id:           Token_Id.   Parse("TOKEN0001"),
                   VersionId:    1,
                   Type:         TokenType.RFID,
                   ContractId:   Contract_Id.Parse("DE-GEF-C12345678-X"),
                   Issuer:       "GraphDefined CA",
                   ValidFrom:    start,
                   Whitelist:    WhitelistType.NEVER,
                   ValidUntil:   ValidUntil,
                   LastUpdated:  start
               );

        #endregion

        #region (private static) AsJSON(Tokens)

        /// <summary>
        /// Tokens as their JSON, compared with all they say.
        /// </summary>
        private static IEnumerable<String> AsJSON(IEnumerable<Token> Tokens)

            => Tokens.Select(token => token.ToJSON().ToString(Newtonsoft.Json.Formatting.None));

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

                   OurCredentialRoles:  [
                                            new CredentialsRole(
                                                ourPartyId,
                                                Role.EMSP,
                                                new BusinessDetails("GraphDefined EMSP")
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
