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

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests
{

    /// <summary>
    /// An EMSP pushes one of its tokens to the CPO, which keeps it as the
    /// EMSP's - and stays a CPO.
    /// </summary>
    /// <remarks>
    /// The CPO filed a token only under one of its own parties: an EMSP it was
    /// peered with could push none, unless the application made the EMSP a
    /// party of the CPO's own - which made the CPO say it was an EMSP too, in
    /// its credentials and its version details.
    /// </remarks>
    [TestFixture]
    public class PushedTokensTests : ANodeTests
    {

        #region Data

        private static readonly Party_Idv3 cpoId   = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));
        private static readonly Party_Idv3 emsp1Id = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GDF"));

        #endregion


        #region ATokenAnEMSPPushesIsKeptAsItsOwn()

        [Test]
        public async Task ATokenAnEMSPPushesIsKeptAsItsOwn()
        {

            var response = await TheCPO().PutToken(AToken(emsp1Id, "0123456789ABCDEF"));

            Assert.Multiple(() => {

                Assert.That(response.StatusCode.Value, Is.EqualTo(1000), response.StatusMessage);

                Assert.That(cpoCommonAPI!.GetTokenStatus(emsp1Id).Select(status => status.Token.Id.ToString()), Is.EquivalentTo(new[] { "0123456789ABCDEF" }),
                            "The CPO does not keep the token as the EMSP's.");

                Assert.That(cpoCommonAPI.GetTokenStatus().Select(status => status.Token.Id.ToString()), Does.Contain("0123456789ABCDEF"),
                            "The CPO's tokens in all do not have it.");

                Assert.That(cpoCommonAPI.Parties.Select(party => party.Id), Is.EquivalentTo(new[] { cpoId }),
                            "The EMSP became one of the CPO's own parties.");

            });

        }

        #endregion

        #region ATokenOfAPartyNobodyRegisteredIsNotKept()

        [Test]
        public async Task ATokenOfAPartyNobodyRegisteredIsNotKept()
        {

            var stranger  = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("XYZ"));

            var response  = await TheCPO().PutToken(AToken(stranger, "FEDCBA9876543210"));

            Assert.Multiple(() => {
                Assert.That(response.StatusCode.Value,             Is.Not.EqualTo(1000), "The token of a party nobody registered was taken.");
                Assert.That(cpoCommonAPI!.GetTokenStatus(stranger), Is.Empty,           "The token of a party nobody registered is kept.");
            });

        }

        #endregion


        #region (private) TheCPO() / AToken(PartyId, TokenId)

        /// <summary>
        /// EMSP #1's client of the CPO.
        /// </summary>
        private EMSP.HTTP.EMSP2CPO_HTTPClient TheCPO()
        {

            var client = emsp1EMSPAPI?.GetCPOClient(
                             CountryCode: cpoId.CountryCode,
                             PartyId:     cpoId.PartyId
                         );

            Assert.That(client, Is.Not.Null);

            return client!;

        }

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
                   LastUpdated: new DateTimeOffset(2026, 10, 1, 8, 0, 0, TimeSpan.Zero)
               );

        #endregion

    }

}
