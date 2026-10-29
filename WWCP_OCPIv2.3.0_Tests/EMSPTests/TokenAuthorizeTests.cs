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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.EMSPTests
{

    /// <summary>
    /// A CPO asks EMSP #1 to authorize a token of EMSP #1's.
    /// </summary>
    /// <remarks>
    /// OCPI says of the type of the token: "Default if omitted: RFID". The
    /// EMSP compared the type of its token with no type at all where a CPO
    /// left it out, and every token was unknown to it. And what EMSP #1 allows
    /// from its own tokens, a filter of its own may take back.
    /// </remarks>
    [TestFixture]
    public class TokenAuthorizeTests : ANodeTests
    {

        #region Data

        private static readonly Token_Id tokenId = Token_Id.Parse("11223344");

        #endregion


        #region AnRFIDTokenIsAuthorizedWhereItsTypeIsLeftOut()

        [Test]
        public async Task AnRFIDTokenIsAuthorizedWhereItsTypeIsLeftOut()
        {

            var response = await Authorize(null);

            Assert.Multiple(() => {
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code, Is.EqualTo(200),  response.StatusMessage);
                Assert.That(response.StatusCode.Value,                  Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.Data?.Allowed,                     Is.EqualTo(AllowedType.ALLOWED));
            });

        }

        #endregion

        #region AnRFIDTokenIsAuthorizedAsAnRFIDToken()

        [Test]
        public async Task AnRFIDTokenIsAuthorizedAsAnRFIDToken()
        {

            var response = await Authorize(TokenType.RFID);

            Assert.Multiple(() => {
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code, Is.EqualTo(200),  response.StatusMessage);
                Assert.That(response.StatusCode.Value,                  Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.Data?.Allowed,                     Is.EqualTo(AllowedType.ALLOWED));
            });

        }

        #endregion

        #region AnRFIDTokenIsUnknownAsAnAppUser()

        [Test]
        public async Task AnRFIDTokenIsUnknownAsAnAppUser()
        {

            var response = await Authorize(TokenType.APP_USER);

            Assert.Multiple(() => {
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code, Is.EqualTo(404));
                Assert.That(response.StatusMessage,                     Is.EqualTo("Unknown token!"));
            });

        }

        #endregion

        #region AFilterTakesAnAuthorizationBack()

        /// <summary>
        /// A filter on the authorizations EMSP #1 answers from its own tokens
        /// takes one back - asked with the token, and with the party asked.
        /// </summary>
        [Test]
        public async Task AFilterTakesAnAuthorizationBack()
        {

            TokenStatus? asked    = null;
            Party_Idv3?  askedTo  = null;

            var response = await Authorize(TokenType.RFID,
                                            Filter: (from, to, tokenStatus, location, cancellationToken) => {
                                                asked    = tokenStatus;
                                                askedTo  = to;
                                                return Task.FromResult<AllowedType?>(AllowedType.NOT_ALLOWED);
                                            });

            Assert.Multiple(() => {
                Assert.That(response.StatusCode.Value,  Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.Data?.Allowed,     Is.EqualTo(AllowedType.NOT_ALLOWED), "The filter did not take the authorization back.");
                Assert.That(asked?.Token.Id,            Is.EqualTo(tokenId));
                Assert.That(asked?.Status,              Is.EqualTo(AllowedType.ALLOWED));
                Assert.That(askedTo,                    Is.Not.Null);
            });

        }

        #endregion

        #region AFilterThatSaysNothingLeavesItAsItIs()

        /// <summary>
        /// A filter that answers null leaves the authorization as the token
        /// has it.
        /// </summary>
        [Test]
        public async Task AFilterThatSaysNothingLeavesItAsItIs()
        {

            var response = await Authorize(TokenType.RFID,
                                           Filter: (from, to, tokenStatus, location, cancellationToken) => Task.FromResult<AllowedType?>(null));

            Assert.That(response.Data?.Allowed, Is.EqualTo(AllowedType.ALLOWED), response.StatusMessage);

        }

        #endregion

        #region AFilterNeverAllowsWhatTheTokenDoesNot()

        /// <summary>
        /// A blocked token stays blocked whatever a filter says: a filter
        /// narrows, and is not even asked where there is nothing to narrow.
        /// </summary>
        [Test]
        public async Task AFilterNeverAllowsWhatTheTokenDoesNot()
        {

            var asked = false;

            var response = await Authorize(TokenType.RFID,
                                           Status: AllowedType.BLOCKED,
                                           Filter: (from, to, tokenStatus, location, cancellationToken) => {
                                               asked = true;
                                               return Task.FromResult<AllowedType?>(AllowedType.ALLOWED);
                                           });

            Assert.Multiple(() => {
                Assert.That(response.Data?.Allowed,  Is.EqualTo(AllowedType.BLOCKED), "A filter allowed a blocked token.");
                Assert.That(asked,                   Is.False, "A filter was asked of a token that is not allowed anyway.");
            });

        }

        #endregion

        #region AFilterThatFailsAllowsNothing()

        /// <summary>
        /// A filter that throws allows nothing.
        /// </summary>
        [Test]
        public async Task AFilterThatFailsAllowsNothing()
        {

            var response = await Authorize(TokenType.RFID,
                                           Filter: (from, to, tokenStatus, location, cancellationToken) => throw new InvalidOperationException("The filter's own store is down."));

            Assert.That(response.Data?.Allowed, Is.EqualTo(AllowedType.NOT_ALLOWED), response.StatusMessage);

        }

        #endregion


        #region (private) Authorize(Type, Status = ALLOWED, Filter = null)

        /// <summary>
        /// EMSP #1 has an RFID token, which the CPO asks it to authorize -
        /// with the given type, or none.
        /// </summary>
        private async Task<OCPIResponse<AuthorizationInfo>> Authorize(TokenType?                                       Type,
                                                                      AllowedType?                                     Status   = null,
                                                                      EMSP_HTTPAPI.OnLocalTokenAuthorizationDelegate?  Filter   = null)
        {

            var added = await emsp1CommonAPI!.AddToken(
                                  new Token(
                                      CountryCode.Parse("DE"),
                                      Party_Id.   Parse("GDF"),
                                      tokenId,
                                      TokenType.RFID,
                                      Contract_Id.Parse("DE-GDF-C12345678-X"),
                                      "GraphDefined CA",
                                      true,
                                      WhitelistType.NEVER
                                  ),
                                  Status ?? AllowedType.ALLOWED
                              );

            Assert.That(added.IsSuccess, Is.True, $"EMSP #1 could not add its token: {added.ErrorResponse}");

            if (Filter is not null)
                emsp1EMSPAPI!.OnLocalTokenAuthorization += Filter;

            var emsp = cpoCPOAPI?.GetEMSPClient(
                           CountryCode: CountryCode.Parse("DE"),
                           PartyId:     Party_Id.   Parse("GDF")
                       );

            Assert.That(emsp, Is.Not.Null);

            return await emsp!.PostToken(tokenId, Type);

        }

        #endregion

    }

}
