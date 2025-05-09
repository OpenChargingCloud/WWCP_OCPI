/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using NUnit.Framework.Legacy;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.Datastructures
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
                             TokenType. RFID,
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

            ClassicAssert.AreEqual("DE",                            JSON["country_code"].                    Value<String>());
            ClassicAssert.AreEqual("GEF",                           JSON["party_id"].                        Value<String>());
            ClassicAssert.AreEqual("Token0001",                     JSON["uid"].                             Value<String>());
            ClassicAssert.AreEqual("RFID",                          JSON["type"].                            Value<String>());
            ClassicAssert.AreEqual("0815",                          JSON["contract_id"].                     Value<String>());
            ClassicAssert.AreEqual("RFID:0815",                     JSON["visual_number"].                   Value<String>());
            ClassicAssert.AreEqual("GraphDefined GmbH",             JSON["issuer"].                          Value<String>());
            ClassicAssert.AreEqual("G1234",                         JSON["group_id"].                        Value<String>());
            ClassicAssert.AreEqual(true,                            JSON["valid"].                           Value<Boolean>());
            ClassicAssert.AreEqual("NEVER",                         JSON["whitelist"].                       Value<String>());
            ClassicAssert.AreEqual("de",                            JSON["language"].                        Value<String>());
            ClassicAssert.AreEqual("FAST",                          JSON["default_profile_type"].            Value<String>());
            ClassicAssert.AreEqual("Stadtwerke Jena-Ost",           JSON["energy_contract"]["supplier_name"].Value<String>());
            ClassicAssert.AreEqual("GDF012324",                     JSON["energy_contract"]["contract_id"].  Value<String>());
            ClassicAssert.AreEqual("2020-09-21T00:00:00.000Z",      JSON["last_updated"].                    Value<String>());

            ClassicAssert.IsTrue(Token.TryParse(JSON, out var Token2, out var errorResponse));
            ClassicAssert.IsNull(errorResponse);

            ClassicAssert.AreEqual(Token1.CountryCode,              Token2.CountryCode);
            ClassicAssert.AreEqual(Token1.PartyId,                  Token2.PartyId);
            ClassicAssert.AreEqual(Token1.Id,                       Token2.Id);
            ClassicAssert.AreEqual(Token1.Type,                     Token2.Type);
            ClassicAssert.AreEqual(Token1.ContractId,               Token2.ContractId);
            ClassicAssert.AreEqual(Token1.Issuer,                   Token2.Issuer);
            ClassicAssert.AreEqual(Token1.IsValid,                  Token2.IsValid);
            ClassicAssert.AreEqual(Token1.WhitelistType,            Token2.WhitelistType);
            ClassicAssert.AreEqual(Token1.VisualNumber,             Token2.VisualNumber);
            ClassicAssert.AreEqual(Token1.GroupId,                  Token2.GroupId);
            ClassicAssert.AreEqual(Token1.UILanguage,               Token2.UILanguage);
            ClassicAssert.AreEqual(Token1.DefaultProfile,           Token2.DefaultProfile);
            ClassicAssert.AreEqual(Token1.EnergyContract,           Token2.EnergyContract);
            ClassicAssert.AreEqual(Token1.LastUpdated.ToISO8601(),  Token2.LastUpdated.ToISO8601());

        }

        #endregion


        #region Token_DeserializeGitHub_Test01()

        /// <summary>
        /// Tries to deserialize a token example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2.1-bugfixes/examples/token_put_example.json
        /// </summary>
        [Test]
        public static void Token_DeserializeGitHub_Test01()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"":    ""NL"",
                           ""party_id"":        ""TNM"",
                           ""uid"":             ""012345678"",
                           ""type"":            ""RFID"",
                           ""contract_id"":     ""NL8ACC12E46L89"",
                           ""visual_number"":   ""DF000-2001-8999-1"",
                           ""issuer"":          ""TheNewMotion"",
                           ""group_id"":        ""DF000-2001-8999"",
                           ""valid"":             true,
                           ""whitelist"":       ""ALWAYS"",
                           ""last_updated"":    ""2015-06-29T22:39:09Z""
                         }";

            #endregion

            var result = Token.TryParse(JObject.Parse(JSON), out var parsedToken, out var errorResponse);
            ClassicAssert.IsTrue   (result, errorResponse);
            ClassicAssert.IsNotNull(parsedToken);
            ClassicAssert.IsNull   (errorResponse);

            ClassicAssert.AreEqual(CountryCode.Parse("NL"),                             parsedToken.CountryCode);
            ClassicAssert.AreEqual(Party_Id.   Parse("TNM"),                            parsedToken.PartyId);
            ClassicAssert.AreEqual(Token_Id.   Parse("012345678"),                      parsedToken.Id);
            ClassicAssert.AreEqual(TokenType.  RFID,                                    parsedToken.Type);
            ClassicAssert.AreEqual(Contract_Id.Parse("NL8ACC12E46L89"),                 parsedToken.ContractId);
            ClassicAssert.AreEqual("DF000-2001-8999-1",                                 parsedToken.VisualNumber);
            ClassicAssert.AreEqual("TheNewMotion",                                      parsedToken.Issuer);
            ClassicAssert.AreEqual(Group_Id.   Parse("DF000-2001-8999"),                parsedToken.GroupId);
            ClassicAssert.AreEqual(true,                                                parsedToken.IsValid);
            ClassicAssert.AreEqual(WhitelistTypes.ALWAYS,                               parsedToken.WhitelistType);
            ClassicAssert.AreEqual(DateTime.Parse("2015-06-29T22:39:09Z").ToISO8601(),  parsedToken.LastUpdated.ToISO8601());

        }

        #endregion

        #region Token_DeserializeGitHub_Test02()

        /// <summary>
        /// Tries to deserialize a token example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2.1-bugfixes/examples/token_put_example.json
        /// </summary>
        [Test]
        public static void Token_DeserializeGitHub_Test02()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"":          ""NL"",
                           ""party_id"":              ""TNM"",
                           ""uid"":                   ""012345678"",
                           ""type"":                  ""RFID"",
                           ""contract_id"":           ""NL8ACC12E46L89"",
                           ""visual_number"":         ""DF000-2001-8999-1"",
                           ""issuer"":                ""TheNewMotion"",
                           ""group_id"":              ""DF000-2001-8999"",
                           ""valid"":                   true,
                           ""whitelist"":             ""ALWAYS"",
                           //""language"":              ""it"",
                           //""default_profile_type"":  ""GREEN"",
                           //""energy_contract"": {
                           //  ""supplier_name"":       ""Greenpeace Energy eG"",
                           //  ""contract_id"":         ""0123456789""
                           //},
                           ""last_updated"":          ""2018-12-10T17:25:10Z""
                         }";

            #endregion

            var result = Token.TryParse(JObject.Parse(JSON), out var parsedToken, out var errorResponse);
            ClassicAssert.IsTrue   (result, errorResponse);
            ClassicAssert.IsNotNull(parsedToken);
            ClassicAssert.IsNull   (errorResponse);

            ClassicAssert.AreEqual(CountryCode.Parse("NL"),                             parsedToken.CountryCode);
            ClassicAssert.AreEqual(Party_Id.   Parse("TNM"),                            parsedToken.PartyId);
            ClassicAssert.AreEqual(Token_Id.   Parse("012345678"),                      parsedToken.Id);
            ClassicAssert.AreEqual(TokenType.  RFID,                                    parsedToken.Type);
            ClassicAssert.AreEqual(Contract_Id.Parse("NL8ACC12E46L89"),                 parsedToken.ContractId);
            ClassicAssert.AreEqual("DF000-2001-8999-1",                                 parsedToken.VisualNumber);
            ClassicAssert.AreEqual("TheNewMotion",                                      parsedToken.Issuer);
            ClassicAssert.AreEqual(Group_Id.   Parse("DF000-2001-8999"),                parsedToken.GroupId);
            ClassicAssert.AreEqual(true,                                                parsedToken.IsValid);
            ClassicAssert.AreEqual(WhitelistTypes.ALWAYS,                               parsedToken.WhitelistType);
            //ClassicAssert.AreEqual(Languages.it,                                        parsedToken.UILanguage);
            //ClassicAssert.AreEqual(ProfileTypes.GREEN,                                  parsedToken.DefaultProfile);
            //ClassicAssert.AreEqual("Greenpeace Energy eG",                              parsedToken.EnergyContract.Value.SupplierName);
            //ClassicAssert.AreEqual(EnergyContract_Id.Parse("0123456789"),               parsedToken.EnergyContract.Value.ContractId);
            ClassicAssert.AreEqual(DateTime.Parse("2018-12-10T17:25:10Z").ToISO8601(),  parsedToken.LastUpdated.ToISO8601());

        }

        #endregion

        #region Token_DeserializeGitHub_Test03()

        /// <summary>
        /// Tries to deserialize a token example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2.1-bugfixes/examples/token_example_1_app_user.json
        /// </summary>
        [Test]
        public static void Token_DeserializeGitHub_Test03()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"":  ""DE"",
                           ""party_id"":      ""TNM"",
                           ""uid"":           ""bdf21bce-fc97-11e8-8eb2-f2801f1b9fd1"",
                           ""type"":          ""APP_USER"",
                           ""contract_id"":   ""DE8ACC12E46L89"",
                           ""issuer"":        ""TheNewMotion"",
                           ""valid"":           true,
                           ""whitelist"":     ""ALLOWED"",
                           ""last_updated"":  ""2018-12-10T17:16:15Z""
                         }";

            #endregion

            var result = Token.TryParse(JObject.Parse(JSON), out var parsedToken, out var errorResponse);
            ClassicAssert.IsTrue   (result, errorResponse);
            ClassicAssert.IsNotNull(parsedToken);
            ClassicAssert.IsNull   (errorResponse);

            ClassicAssert.AreEqual(CountryCode.Parse("DE"),                                    parsedToken.CountryCode);
            ClassicAssert.AreEqual(Party_Id.   Parse("TNM"),                                   parsedToken.PartyId);
            ClassicAssert.AreEqual(Token_Id.   Parse("bdf21bce-fc97-11e8-8eb2-f2801f1b9fd1"),  parsedToken.Id);
            ClassicAssert.AreEqual(TokenType.  APP_USER,                                       parsedToken.Type);
            ClassicAssert.AreEqual(Contract_Id.Parse("DE8ACC12E46L89"),                        parsedToken.ContractId);
            ClassicAssert.AreEqual("TheNewMotion",                                             parsedToken.Issuer);
            ClassicAssert.AreEqual(true,                                                       parsedToken.IsValid);
            ClassicAssert.AreEqual(WhitelistTypes.ALLOWED,                                     parsedToken.WhitelistType);
            ClassicAssert.AreEqual(DateTime.Parse("2018-12-10T17:16:15Z").ToISO8601(),         parsedToken.LastUpdated.ToISO8601());

        }

        #endregion


    }

}
