/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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

using NUnit.Framework;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests
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
                             Token_Id.   Parse("Token0001"),
                             TokenTypes. RFID,
                             Auth_Id.Parse("0815"),
                             "GraphDefined GmbH",
                             true,
                             WhitelistTypes.NEVER,
                             "RFID:0815",
                             Languages.de,
                             DateTime.Parse("2020-09-21T00:00:00.000Z")
                         );

            var JSON = Token1.ToJSON();

            Assert.AreEqual("Token0001",                     JSON["uid"].                             Value<String>());
            Assert.AreEqual("RFID",                          JSON["type"].                            Value<String>());
            Assert.AreEqual("0815",                          JSON["auth_id"].                         Value<String>());
            Assert.AreEqual("RFID:0815",                     JSON["visual_number"].                   Value<String>());
            Assert.AreEqual("GraphDefined GmbH",             JSON["issuer"].                          Value<String>());
            Assert.AreEqual(true,                            JSON["valid"].                           Value<Boolean>());
            Assert.AreEqual("NEVER",                         JSON["whitelist"].                       Value<String>());
            Assert.AreEqual("de",                            JSON["language"].                        Value<String>());
            Assert.AreEqual("2020-09-21T00:00:00.000Z",      JSON["last_updated"].                    Value<String>());

            Assert.IsTrue(Token.TryParse(JSON, out Token Token2, out String ErrorResponse));
            Assert.IsNull(ErrorResponse);

            Assert.AreEqual(Token1.Id,                       Token2.Id);
            Assert.AreEqual(Token1.Type,                     Token2.Type);
            Assert.AreEqual(Token1.AuthId,                   Token2.AuthId);
            Assert.AreEqual(Token1.Issuer,                   Token2.Issuer);
            Assert.AreEqual(Token1.IsValid,                  Token2.IsValid);
            Assert.AreEqual(Token1.WhitelistType,            Token2.WhitelistType);
            Assert.AreEqual(Token1.VisualNumber,             Token2.VisualNumber);
            Assert.AreEqual(Token1.UILanguage,               Token2.UILanguage);
            Assert.AreEqual(Token1.LastUpdated.ToIso8601(),  Token2.LastUpdated.ToIso8601());

        }

        #endregion


        #region Token_DeserializeGitHub_Test01()

        /// <summary>
        /// Tries to deserialize a token example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/examples/token_put_example.json
        /// </summary>
        [Test]
        public static void Token_DeserializeGitHub_Test01()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"":    ""DE"",
                           ""party_id"":        ""TNM"",
                           ""uid"":             ""012345678"",
                           ""type"":            ""RFID"",
                           ""contract_id"":     ""DE8ACC12E46L89"",
                           ""visual_number"":   ""DF000-2001-8999-1"",
                           ""issuer"":          ""TheNewMotion"",
                           ""group_id"":        ""DF000-2001-8999"",
                           ""valid"":             true,
                           ""whitelist"":       ""ALWAYS"",
                           ""last_updated"":    ""2015-06-29T22:39:09Z""
                         }";

            #endregion

            Assert.IsTrue(Token.TryParse(JObject.Parse(JSON), out var parsedToken, out var errorResponse));
            Assert.IsNull(errorResponse);

            Assert.AreEqual(Token_Id.   Parse("012345678"),                      parsedToken.Id);
            Assert.AreEqual(TokenTypes. RFID,                                    parsedToken.Type);
            Assert.AreEqual(Auth_Id.Parse("DE8ACC12E46L89"),                     parsedToken.AuthId);
            Assert.AreEqual("DF000-2001-8999-1",                                 parsedToken.VisualNumber);
            Assert.AreEqual("TheNewMotion",                                      parsedToken.Issuer);
            Assert.AreEqual(true,                                                parsedToken.IsValid);
            Assert.AreEqual(WhitelistTypes.ALWAYS,                               parsedToken.WhitelistType);
            Assert.AreEqual(DateTime.Parse("2015-06-29T22:39:09Z").ToIso8601(),  parsedToken.LastUpdated.ToIso8601());

        }

        #endregion

        #region Token_DeserializeGitHub_Test02()

        /// <summary>
        /// Tries to deserialize a token example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/examples/token_put_example.json
        /// </summary>
        [Test]
        public static void Token_DeserializeGitHub_Test02()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"":          ""DE"",
                           ""party_id"":              ""TNM"",
                           ""uid"":                   ""12345678905880"",
                           ""type"":                  ""RFID"",
                           ""contract_id"":           ""DE8ACC12E46L89"",
                           ""visual_number"":         ""DF000-2001-8999-1"",
                           ""issuer"":                ""TheNewMotion"",
                           ""group_id"":              ""DF000-2001-8999"",
                           ""valid"":                   true,
                           ""whitelist"":             ""ALLOWED"",
                           ""language"":              ""it"",
                           ""default_profile_type"":  ""GREEN"",
                           ""energy_contract"": {
                             ""supplier_name"":       ""Greenpeace Energy eG"",
                             ""contract_id"":         ""0123456789""
                           },
                           ""last_updated"":          ""2018-12-10T17:25:10Z""
                         }";

            #endregion

            Assert.IsTrue(Token.TryParse(JObject.Parse(JSON), out var parsedToken, out var errorResponse));
            Assert.IsNull(errorResponse);

            Assert.AreEqual(Token_Id.   Parse("12345678905880"),                 parsedToken.Id);
            Assert.AreEqual(TokenTypes. RFID,                                    parsedToken.Type);
            Assert.AreEqual(Auth_Id.Parse("DE8ACC12E46L89"),                     parsedToken.AuthId);
            Assert.AreEqual("DF000-2001-8999-1",                                 parsedToken.VisualNumber);
            Assert.AreEqual("TheNewMotion",                                      parsedToken.Issuer);
            Assert.AreEqual(true,                                                parsedToken.IsValid);
            Assert.AreEqual(WhitelistTypes.ALLOWED,                              parsedToken.WhitelistType);
            Assert.AreEqual(Languages.it,                                        parsedToken.UILanguage);
            Assert.AreEqual(DateTime.Parse("2018-12-10T17:25:10Z").ToIso8601(),  parsedToken.LastUpdated.ToIso8601());

        }

        #endregion

        #region Token_DeserializeGitHub_Test03()

        /// <summary>
        /// Tries to deserialize a token example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/examples/token_example_1_app_user.json
        /// </summary>
        [Test]
        public static void Token_DeserializeGitHub_Test03()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"":  ""DE"",
                           ""party_id"":      ""TNM"",
                           ""uid"":           ""bdf21bce-fc97-11e8-8eb2-f2801f1b9fd1"",
                           ""type"":          ""RFID"",
                           ""contract_id"":   ""DE8ACC12E46L89"",
                           ""issuer"":        ""TheNewMotion"",
                           ""valid"":           true,
                           ""whitelist"":     ""ALLOWED"",
                           ""last_updated"":  ""2018-12-10T17:16:15Z""
                         }";

            #endregion

            Assert.IsTrue(Token.TryParse(JObject.Parse(JSON), out var parsedToken, out var errorResponse));
            Assert.IsNull(errorResponse);

            Assert.AreEqual(Token_Id.   Parse("bdf21bce-fc97-11e8-8eb2-f2801f1b9fd1"),  parsedToken.Id);
            Assert.AreEqual(TokenTypes. RFID,                                           parsedToken.Type);
            Assert.AreEqual(Auth_Id.Parse("DE8ACC12E46L89"),                            parsedToken.AuthId);
            Assert.AreEqual("TheNewMotion",                                             parsedToken.Issuer);
            Assert.AreEqual(true,                                                       parsedToken.IsValid);
            Assert.AreEqual(WhitelistTypes.ALLOWED,                                     parsedToken.WhitelistType);
            Assert.AreEqual(DateTime.Parse("2018-12-10T17:16:15Z").ToIso8601(),         parsedToken.LastUpdated.ToIso8601());

        }

        #endregion


    }

}
