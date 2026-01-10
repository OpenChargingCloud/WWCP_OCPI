/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
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

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.Datastructures
{

    /// <summary>
    /// Charging tokens tests.
    /// https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_tokens.md
    /// </summary>
    [TestFixture]
    public static class TokenTests
    {

        #region Token_DeserializeGitHub_Test()

        /// <summary>
        /// Tries to deserialize a token example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_tokens.md#example
        /// </summary>
        [Test]
        public static void Token_DeserializeGitHub_Test()
        {

            #region Define JSON

            var tokenJSON = @"{
                                ""uid"":             ""012345678"",
                                ""type"":            ""RFID"",
                                ""auth_id"":         ""DE8ACC12E46L89"",
                                ""visual_number"":   ""DF000-2001-8999"",
                                ""issuer"":          ""TheNewMotion"",
                                ""valid"":             true,
                                ""whitelist"":       ""ALLOWED"",
                                ""last_updated"":    ""2015-06-29T22:39:09Z""
                              }";

            #endregion

            var result = Token.TryParse(JObject.Parse(tokenJSON),
                                        out var parsedToken,
                                        out var errorResponse,
                                        CountryCode.Parse("DE"),
                                        Party_Id.   Parse("TNM"));

            ClassicAssert.IsTrue   (result, errorResponse);
            ClassicAssert.IsNotNull(parsedToken);
            ClassicAssert.IsNull   (errorResponse);

            if (parsedToken is not null)
            {

                ClassicAssert.AreEqual(Token_Id. Parse("012345678"),        parsedToken.Id);
                ClassicAssert.AreEqual(TokenType.RFID,                      parsedToken.Type);
                ClassicAssert.AreEqual(Auth_Id.  Parse("DE8ACC12E46L89"),   parsedToken.AuthId);
                ClassicAssert.AreEqual("DF000-2001-8999",                   parsedToken.VisualNumber);
                ClassicAssert.AreEqual("TheNewMotion",                      parsedToken.Issuer);
                ClassicAssert.AreEqual(true,                                parsedToken.IsValid);
                ClassicAssert.AreEqual(WhitelistTypes.ALLOWED,              parsedToken.WhitelistType);
                ClassicAssert.AreEqual("2015-06-29T22:39:09.000Z",          parsedToken.LastUpdated.ToISO8601());

            }

        }

        #endregion


        #region Token_SerializeDeserialize_Test01()

        /// <summary>
        /// Token serialize, deserialize and compare test.
        /// </summary>
        [Test]
        public static void Token_SerializeDeserialize_Test01()
        {

            var token1 = new Token(
                             CountryCode.Parse("DE"),
                             Party_Id.   Parse("GDF"),
                             Token_Id.   Parse("Token0001"),
                             TokenType.  RFID,
                             Auth_Id.    Parse("0815"),
                             "GraphDefined GmbH",
                             true,
                             WhitelistTypes.NEVER,
                             "RFID:0815",
                             Languages.de,
                             DateTime.Parse("2020-09-21T00:00:00.000Z")
                         );

            var json = token1.ToJSON();

            ClassicAssert.AreEqual("Token0001",                     json["uid"]?.                             Value<String>());
            ClassicAssert.AreEqual("RFID",                          json["type"]?.                            Value<String>());
            ClassicAssert.AreEqual("0815",                          json["auth_id"]?.                         Value<String>());
            ClassicAssert.AreEqual("RFID:0815",                     json["visual_number"]?.                   Value<String>());
            ClassicAssert.AreEqual("GraphDefined GmbH",             json["issuer"]?.                          Value<String>());
            ClassicAssert.AreEqual(true,                            json["valid"]?.                           Value<Boolean>());
            ClassicAssert.AreEqual("NEVER",                         json["whitelist"]?.                       Value<String>());
            ClassicAssert.AreEqual("de",                            json["language"]?.                        Value<String>());
            ClassicAssert.AreEqual("2020-09-21T00:00:00.000Z",      json["last_updated"]?.                    Value<String>());


            var result = Token.TryParse(json,
                                        out var token2,
                                        out var errorResponse,
                                        CountryCode.Parse("DE"),
                                        Party_Id.   Parse("GDF"));

            ClassicAssert.IsTrue   (result, errorResponse);
            ClassicAssert.IsNotNull(token2);
            ClassicAssert.IsNull   (errorResponse);

            if (token2 is not null)
            {

                ClassicAssert.AreEqual(token1.Id,                       token2.Id);
                ClassicAssert.AreEqual(token1.Type,                     token2.Type);
                ClassicAssert.AreEqual(token1.AuthId,                   token2.AuthId);
                ClassicAssert.AreEqual(token1.Issuer,                   token2.Issuer);
                ClassicAssert.AreEqual(token1.IsValid,                  token2.IsValid);
                ClassicAssert.AreEqual(token1.WhitelistType,            token2.WhitelistType);
                ClassicAssert.AreEqual(token1.VisualNumber,             token2.VisualNumber);
                ClassicAssert.AreEqual(token1.UILanguage,               token2.UILanguage);
                ClassicAssert.AreEqual(token1.LastUpdated.ToISO8601(),  token2.LastUpdated.ToISO8601());

            }

        }

        #endregion


    }

}
