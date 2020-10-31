/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;

using NUnit.Framework;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.UnitTests
{

    /// <summary>
    /// Unit tests for tokens.
    /// https://github.com/ocpi/ocpi/blob/master/mod_tokens.asciidoc
    /// </summary>
    [TestFixture]
    public static class TokenTests
    {

        #region Token_SerializeDeserialize_Test01()

        /// <summary>
        /// Token serialize, deserialize and compare test.
        /// </summary>
        [Test]
        public static void Token_SerializeDeserialize_Test01()
        {

            var Token1 = new Token(
                             CountryCode.Parse("DE"),
                             Party_Id.   Parse("GEF"),
                             Token_Id.   Parse("Token0001"),
                             TokenTypes. RFID,
                             Contract_Id.Parse("0815"),
                             "GraphDefined GmbH",
                             true,
                             WhitelistTypes.NEVER,
                             "RFID:0815",
                             Group_Id.Parse("G1234"),
                             Languages.de,
                             ProfileTypes.FAST,
                             new EnergyContract(
                                 "Stadtwerke Jena-Ost",
                                 EnergyContract_Id.Parse("GDF012324")
                             ),
                             DateTime.Parse("2020-09-21T00:00:00.000Z")
                         );

            var JSON = Token1.ToJSON();

            Assert.AreEqual("DE",                            JSON["country_code"].                    Value<String>());
            Assert.AreEqual("GEF",                           JSON["party_id"].                        Value<String>());
            Assert.AreEqual("Token0001",                     JSON["uid"].                             Value<String>());
            Assert.AreEqual("RFID",                          JSON["type"].                            Value<String>());
            Assert.AreEqual("0815",                          JSON["contract_id"].                     Value<String>());
            Assert.AreEqual("RFID:0815",                     JSON["visual_number"].                   Value<String>());
            Assert.AreEqual("GraphDefined GmbH",             JSON["issuer"].                          Value<String>());
            Assert.AreEqual("G1234",                         JSON["group_id"].                        Value<String>());
            Assert.AreEqual(true,                            JSON["valid"].                           Value<Boolean>());
            Assert.AreEqual("NEVER",                         JSON["whitelist"].                       Value<String>());
            Assert.AreEqual("de",                            JSON["language"].                        Value<String>());
            Assert.AreEqual("FAST",                          JSON["default_profile_type"].            Value<String>());
            Assert.AreEqual("Stadtwerke Jena-Ost",           JSON["energy_contract"]["supplier_name"].Value<String>());
            Assert.AreEqual("GDF012324",                     JSON["energy_contract"]["contract_id"].  Value<String>());
            Assert.AreEqual("2020-09-21T00:00:00.000Z",      JSON["last_updated"].                    Value<String>());

            Assert.IsTrue(Token.TryParse(JSON, out Token Token2, out String ErrorResponse));
            Assert.IsNull(ErrorResponse);

            Assert.AreEqual(Token1.CountryCode,              Token2.CountryCode);
            Assert.AreEqual(Token1.PartyId,                  Token2.PartyId);
            Assert.AreEqual(Token1.Id,                       Token2.Id);
            Assert.AreEqual(Token1.Type,                     Token2.Type);
            Assert.AreEqual(Token1.ContractId,               Token2.ContractId);
            Assert.AreEqual(Token1.Issuer,                   Token2.Issuer);
            Assert.AreEqual(Token1.IsValid,                  Token2.IsValid);
            Assert.AreEqual(Token1.WhitelistType,            Token2.WhitelistType);
            Assert.AreEqual(Token1.VisualNumber,             Token2.VisualNumber);
            Assert.AreEqual(Token1.GroupId,                  Token2.GroupId);
            Assert.AreEqual(Token1.UILanguage,               Token2.UILanguage);
            Assert.AreEqual(Token1.DefaultProfile,           Token2.DefaultProfile);
            Assert.AreEqual(Token1.EnergyContract,           Token2.EnergyContract);
            Assert.AreEqual(Token1.LastUpdated.ToIso8601(),  Token2.LastUpdated.ToIso8601());

        }

        #endregion

    }

}
