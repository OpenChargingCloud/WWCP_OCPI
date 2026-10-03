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

            Assert.That(result,        Is.True, errorResponse);
            Assert.That(parsedToken,   Is.Not.Null);
            Assert.That(errorResponse, Is.Null);

            if (parsedToken is not null)
            {

                Assert.That(parsedToken.Id,                      Is.EqualTo(Token_Id. Parse("012345678")));
                Assert.That(parsedToken.Type,                    Is.EqualTo(TokenType.RFID));
                Assert.That(parsedToken.AuthId,                  Is.EqualTo(Auth_Id.  Parse("DE8ACC12E46L89")));
                Assert.That(parsedToken.VisualNumber,            Is.EqualTo("DF000-2001-8999"));
                Assert.That(parsedToken.Issuer,                  Is.EqualTo("TheNewMotion"));
                Assert.That(parsedToken.IsValid,                 Is.EqualTo(true));
                Assert.That(parsedToken.WhitelistType,           Is.EqualTo(WhitelistType.ALLOWED));
                Assert.That(parsedToken.LastUpdated.ToISO8601(), Is.EqualTo("2015-06-29T22:39:09.000Z"));

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
                             WhitelistType.NEVER,
                             "RFID:0815",
                             Languages.de,
                             DateTime.Parse("2020-09-21T00:00:00.000Z")
                         );

            var json = token1.ToJSON();

            Assert.That(json["uid"]?.                             Value<String>(),  Is.EqualTo("Token0001"));
            Assert.That(json["type"]?.                            Value<String>(),  Is.EqualTo("RFID"));
            Assert.That(json["auth_id"]?.                         Value<String>(),  Is.EqualTo("0815"));
            Assert.That(json["visual_number"]?.                   Value<String>(),  Is.EqualTo("RFID:0815"));
            Assert.That(json["issuer"]?.                          Value<String>(),  Is.EqualTo("GraphDefined GmbH"));
            Assert.That(json["valid"]?.                           Value<Boolean>(), Is.EqualTo(true));
            Assert.That(json["whitelist"]?.                       Value<String>(),  Is.EqualTo("NEVER"));
            Assert.That(json["language"]?.                        Value<String>(),  Is.EqualTo("de"));
            Assert.That(json["last_updated"]?.                    Value<String>(),  Is.EqualTo("2020-09-21T00:00:00.000Z"));


            var result = Token.TryParse(json,
                                        out var token2,
                                        out var errorResponse,
                                        CountryCode.Parse("DE"),
                                        Party_Id.   Parse("GDF"));

            Assert.That(result,        Is.True, errorResponse);
            Assert.That(token2,        Is.Not.Null);
            Assert.That(errorResponse, Is.Null);

            if (token2 is not null)
            {

                Assert.That(token2.Id,                      Is.EqualTo(token1.Id));
                Assert.That(token2.Type,                    Is.EqualTo(token1.Type));
                Assert.That(token2.AuthId,                  Is.EqualTo(token1.AuthId));
                Assert.That(token2.Issuer,                  Is.EqualTo(token1.Issuer));
                Assert.That(token2.IsValid,                 Is.EqualTo(token1.IsValid));
                Assert.That(token2.WhitelistType,           Is.EqualTo(token1.WhitelistType));
                Assert.That(token2.VisualNumber,            Is.EqualTo(token1.VisualNumber));
                Assert.That(token2.UILanguage,              Is.EqualTo(token1.UILanguage));
                Assert.That(token2.LastUpdated.ToISO8601(), Is.EqualTo(token1.LastUpdated.ToISO8601()));

            }

        }

        #endregion


    }

}
