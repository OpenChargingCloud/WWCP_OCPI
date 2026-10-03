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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.Datastructures
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
                             WhitelistType.NEVER,
                             "RFID:0815",
                             Group_Id.Parse("G1234"),
                             Languages.de,
                             ProfileType.FAST,
                             new EnergyContract(
                                 "Stadtwerke Jena-Ost",
                                 EnergyContract_Id.Parse("GDF012324")
                             ),
                             DateTime.Parse("2020-09-21T00:00:00.000Z")
                         );

            var JSON = Token1.ToJSON();

            Assert.That(JSON["country_code"].                    Value<String>(),  Is.EqualTo("DE"));
            Assert.That(JSON["party_id"].                        Value<String>(),  Is.EqualTo("GEF"));
            Assert.That(JSON["uid"].                             Value<String>(),  Is.EqualTo("Token0001"));
            Assert.That(JSON["type"].                            Value<String>(),  Is.EqualTo("RFID"));
            Assert.That(JSON["contract_id"].                     Value<String>(),  Is.EqualTo("0815"));
            Assert.That(JSON["visual_number"].                   Value<String>(),  Is.EqualTo("RFID:0815"));
            Assert.That(JSON["issuer"].                          Value<String>(),  Is.EqualTo("GraphDefined GmbH"));
            Assert.That(JSON["group_id"].                        Value<String>(),  Is.EqualTo("G1234"));
            Assert.That(JSON["valid"].                           Value<Boolean>(), Is.EqualTo(true));
            Assert.That(JSON["whitelist"].                       Value<String>(),  Is.EqualTo("NEVER"));
            Assert.That(JSON["language"].                        Value<String>(),  Is.EqualTo("de"));
            Assert.That(JSON["default_profile_type"].            Value<String>(),  Is.EqualTo("FAST"));
            Assert.That(JSON["energy_contract"]["supplier_name"].Value<String>(),  Is.EqualTo("Stadtwerke Jena-Ost"));
            Assert.That(JSON["energy_contract"]["contract_id"].  Value<String>(),  Is.EqualTo("GDF012324"));
            Assert.That(JSON["last_updated"].                    Value<String>(),  Is.EqualTo("2020-09-21T00:00:00.000Z"));

            Assert.That(Token.TryParse(JSON, out var Token2, out var errorResponse), Is.True);
            Assert.That(errorResponse,                                               Is.Null);

            Assert.That(Token2.CountryCode,             Is.EqualTo(Token1.CountryCode));
            Assert.That(Token2.PartyId,                 Is.EqualTo(Token1.PartyId));
            Assert.That(Token2.Id,                      Is.EqualTo(Token1.Id));
            Assert.That(Token2.Type,                    Is.EqualTo(Token1.Type));
            Assert.That(Token2.ContractId,              Is.EqualTo(Token1.ContractId));
            Assert.That(Token2.Issuer,                  Is.EqualTo(Token1.Issuer));
            Assert.That(Token2.IsValid,                 Is.EqualTo(Token1.IsValid));
            Assert.That(Token2.WhitelistType,           Is.EqualTo(Token1.WhitelistType));
            Assert.That(Token2.VisualNumber,            Is.EqualTo(Token1.VisualNumber));
            Assert.That(Token2.GroupId,                 Is.EqualTo(Token1.GroupId));
            Assert.That(Token2.UILanguage,              Is.EqualTo(Token1.UILanguage));
            Assert.That(Token2.DefaultProfile,          Is.EqualTo(Token1.DefaultProfile));
            Assert.That(Token2.EnergyContract,          Is.EqualTo(Token1.EnergyContract));
            Assert.That(Token2.LastUpdated.ToISO8601(), Is.EqualTo(Token1.LastUpdated.ToISO8601()));

        }

        #endregion


        #region Token_DeserializeGitHub_Test01()

        /// <summary>
        /// Tries to deserialize a token example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.3.0.1-bugfixes/examples/token_put_example.json
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
            Assert.That(result,        Is.True, errorResponse);
            Assert.That(parsedToken,   Is.Not.Null);
            Assert.That(errorResponse, Is.Null);

            Assert.That(parsedToken.CountryCode,             Is.EqualTo(CountryCode.Parse("NL")));
            Assert.That(parsedToken.PartyId,                 Is.EqualTo(Party_Id.   Parse("TNM")));
            Assert.That(parsedToken.Id,                      Is.EqualTo(Token_Id.   Parse("012345678")));
            Assert.That(parsedToken.Type,                    Is.EqualTo(TokenType.  RFID));
            Assert.That(parsedToken.ContractId,              Is.EqualTo(Contract_Id.Parse("NL8ACC12E46L89")));
            Assert.That(parsedToken.VisualNumber,            Is.EqualTo("DF000-2001-8999-1"));
            Assert.That(parsedToken.Issuer,                  Is.EqualTo("TheNewMotion"));
            Assert.That(parsedToken.GroupId,                 Is.EqualTo(Group_Id.   Parse("DF000-2001-8999")));
            Assert.That(parsedToken.IsValid,                 Is.EqualTo(true));
            Assert.That(parsedToken.WhitelistType,           Is.EqualTo(WhitelistType.ALWAYS));
            Assert.That(parsedToken.LastUpdated.ToISO8601(), Is.EqualTo(DateTime.Parse("2015-06-29T22:39:09Z").ToISO8601()));

        }

        #endregion

        #region Token_DeserializeGitHub_Test02()

        /// <summary>
        /// Tries to deserialize a token example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.3.0.1-bugfixes/examples/token_put_example.json
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
            Assert.That(result,        Is.True, errorResponse);
            Assert.That(parsedToken,   Is.Not.Null);
            Assert.That(errorResponse, Is.Null);

            Assert.That(parsedToken.CountryCode,   Is.EqualTo(CountryCode.Parse("NL")));
            Assert.That(parsedToken.PartyId,       Is.EqualTo(Party_Id.   Parse("TNM")));
            Assert.That(parsedToken.Id,            Is.EqualTo(Token_Id.   Parse("012345678")));
            Assert.That(parsedToken.Type,          Is.EqualTo(TokenType.  RFID));
            Assert.That(parsedToken.ContractId,    Is.EqualTo(Contract_Id.Parse("NL8ACC12E46L89")));
            Assert.That(parsedToken.VisualNumber,  Is.EqualTo("DF000-2001-8999-1"));
            Assert.That(parsedToken.Issuer,        Is.EqualTo("TheNewMotion"));
            Assert.That(parsedToken.GroupId,       Is.EqualTo(Group_Id.   Parse("DF000-2001-8999")));
            Assert.That(parsedToken.IsValid,       Is.EqualTo(true));
            Assert.That(parsedToken.WhitelistType, Is.EqualTo(WhitelistType.ALWAYS));
            //ClassicAssert.AreEqual(Languages.it,                                        parsedToken.UILanguage);
            //ClassicAssert.AreEqual(ProfileTypes.GREEN,                                  parsedToken.DefaultProfile);
            //ClassicAssert.AreEqual("Greenpeace Energy eG",                              parsedToken.EnergyContract.Value.SupplierName);
            //ClassicAssert.AreEqual(EnergyContract_Id.Parse("0123456789"),               parsedToken.EnergyContract.Value.ContractId);
            Assert.That(parsedToken.LastUpdated.ToISO8601(), Is.EqualTo(DateTime.Parse("2018-12-10T17:25:10Z").ToISO8601()));

        }

        #endregion

        #region Token_DeserializeGitHub_Test03()

        /// <summary>
        /// Tries to deserialize a token example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.3.0.1-bugfixes/examples/token_example_1_app_user.json
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
            Assert.That(result,        Is.True, errorResponse);
            Assert.That(parsedToken,   Is.Not.Null);
            Assert.That(errorResponse, Is.Null);

            Assert.That(parsedToken.CountryCode,             Is.EqualTo(CountryCode.Parse("DE")));
            Assert.That(parsedToken.PartyId,                 Is.EqualTo(Party_Id.   Parse("TNM")));
            Assert.That(parsedToken.Id,                      Is.EqualTo(Token_Id.   Parse("bdf21bce-fc97-11e8-8eb2-f2801f1b9fd1")));
            Assert.That(parsedToken.Type,                    Is.EqualTo(TokenType.  APP_USER));
            Assert.That(parsedToken.ContractId,              Is.EqualTo(Contract_Id.Parse("DE8ACC12E46L89")));
            Assert.That(parsedToken.Issuer,                  Is.EqualTo("TheNewMotion"));
            Assert.That(parsedToken.IsValid,                 Is.EqualTo(true));
            Assert.That(parsedToken.WhitelistType,           Is.EqualTo(WhitelistType.ALLOWED));
            Assert.That(parsedToken.LastUpdated.ToISO8601(), Is.EqualTo(DateTime.Parse("2018-12-10T17:16:15Z").ToISO8601()));

        }

        #endregion


    }

}
