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

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.EMSPTests
{

    /// <summary>
    /// A CPO asks EMSP #1 to authorize a token of EMSP #1's.
    /// </summary>
    /// <remarks>
    /// OCPI says of the type of the token: "Default if omitted: RFID". The
    /// EMSP compared the type of its token with no type at all where a CPO
    /// left it out, and every token was unknown to it.
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


        #region (private) Authorize(Type)

        /// <summary>
        /// EMSP #1 has an RFID token, which the CPO asks it to authorize -
        /// with the given type, or none.
        /// </summary>
        private async Task<OCPIResponse<AuthorizationInfo>> Authorize(TokenType? Type)
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
                                  AllowedType.ALLOWED
                              );

            Assert.That(added.IsSuccess, Is.True, $"EMSP #1 could not add its token: {added.ErrorResponse}");

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
