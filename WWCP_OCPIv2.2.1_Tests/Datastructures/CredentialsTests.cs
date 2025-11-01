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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.Datastructures
{

    /// <summary>
    /// Unit tests for credentials.
    /// https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/credentials.asciidoc
    /// </summary>
    [TestFixture]
    public static class CredentialsTests
    {

        #region Credentials_SerializeDeserialize_Test01()

        /// <summary>
        /// Credentials serialize, deserialize and compare test.
        /// </summary>
        [Test]
        public static void Credentials_SerializeDeserialize_Test01()
        {

            #region Defined Credentials1

            var Credentials1 = new Credentials(

                                   AccessToken.Parse("4285n43805fng38"),
                                   URL.        Parse("http://open.charging.cloud/versions"),

                                   new CredentialsRole[] {

                                       new CredentialsRole(
                                           CountryCode.Parse("DE"),
                                           Party_Id.Parse("GEF"),
                                           Role.CPO,
                                           new BusinessDetails(
                                               "Open Charging Cloud CPO",
                                               URL.Parse("http://cpo.charging.cloud"),
                                               new Image(
                                                   URL.Parse("http://cpo.charging.cloud/logo"),
                                                   ImageFileType.svg,
                                                   ImageCategory.OPERATOR,
                                                   1000,
                                                   1500,
                                                   URL.Parse("http://cpo.charging.cloud/logo_small")
                                               )
                                           )
                                       ),

                                       new CredentialsRole(
                                           CountryCode.Parse("DE"),
                                           Party_Id.Parse("GDF"),
                                           Role.EMSP,
                                           new BusinessDetails(
                                               "Open Charging Cloud EMSP",
                                               URL.Parse("http://emsp.charging.cloud"),
                                               new Image(
                                                   URL.Parse("http://emsp.charging.cloud/logo"),
                                                   ImageFileType.png,
                                                   ImageCategory.NETWORK,
                                                   2000,
                                                   3000,
                                                   URL.Parse("http://emsp.charging.cloud/logo_small")
                                               )
                                           )
                                       )

                                   }
                               );

            #endregion

            var JSON = Credentials1.ToJSON();

            ClassicAssert.AreEqual("4285n43805fng38",                        JSON["token"].                                            Value<String>());
            ClassicAssert.AreEqual("http://open.charging.cloud/versions",    JSON["url"].                                              Value<String>());

            ClassicAssert.AreEqual("DE",                                     JSON["roles"][0]["country_code"].                         Value<String>());
            ClassicAssert.AreEqual("GEF",                                    JSON["roles"][0]["party_id"].                             Value<String>());
            ClassicAssert.AreEqual("CPO",                                    JSON["roles"][0]["role"].                                 Value<String>());
            ClassicAssert.AreEqual("Open Charging Cloud CPO",                JSON["roles"][0]["business_details"]["name"].             Value<String>());
            ClassicAssert.AreEqual("http://cpo.charging.cloud",              JSON["roles"][0]["business_details"]["website"].          Value<String>());
            ClassicAssert.AreEqual("http://cpo.charging.cloud/logo",         JSON["roles"][0]["business_details"]["logo"]["url"].      Value<String>());
            ClassicAssert.AreEqual("http://cpo.charging.cloud/logo_small",   JSON["roles"][0]["business_details"]["logo"]["thumbnail"].Value<String>());
            ClassicAssert.AreEqual("OPERATOR",                               JSON["roles"][0]["business_details"]["logo"]["category"]. Value<String>());
            ClassicAssert.AreEqual("svg",                                    JSON["roles"][0]["business_details"]["logo"]["type"].     Value<String>());
            ClassicAssert.AreEqual(1000,                                     JSON["roles"][0]["business_details"]["logo"]["width"].    Value<UInt16>());
            ClassicAssert.AreEqual(1500,                                     JSON["roles"][0]["business_details"]["logo"]["height"].   Value<UInt16>());

            ClassicAssert.AreEqual("DE",                                     JSON["roles"][1]["country_code"].                         Value<String>());
            ClassicAssert.AreEqual("GDF",                                    JSON["roles"][1]["party_id"].                             Value<String>());
            ClassicAssert.AreEqual("EMSP",                                   JSON["roles"][1]["role"].                                 Value<String>());
            ClassicAssert.AreEqual("Open Charging Cloud EMSP",               JSON["roles"][1]["business_details"]["name"].             Value<String>());
            ClassicAssert.AreEqual("http://emsp.charging.cloud",             JSON["roles"][1]["business_details"]["website"].          Value<String>());
            ClassicAssert.AreEqual("http://emsp.charging.cloud/logo",        JSON["roles"][1]["business_details"]["logo"]["url"].      Value<String>());
            ClassicAssert.AreEqual("http://emsp.charging.cloud/logo_small",  JSON["roles"][1]["business_details"]["logo"]["thumbnail"].Value<String>());
            ClassicAssert.AreEqual("NETWORK",                                JSON["roles"][1]["business_details"]["logo"]["category"]. Value<String>());
            ClassicAssert.AreEqual("png",                                    JSON["roles"][1]["business_details"]["logo"]["type"].     Value<String>());
            ClassicAssert.AreEqual(2000,                                     JSON["roles"][1]["business_details"]["logo"]["width"].    Value<UInt16>());
            ClassicAssert.AreEqual(3000,                                     JSON["roles"][1]["business_details"]["logo"]["height"].   Value<UInt16>());


            ClassicAssert.IsTrue(Credentials.TryParse(JSON, out Credentials Credentials2, out String ErrorResponse));
            ClassicAssert.IsNull(ErrorResponse);

            ClassicAssert.AreEqual(Credentials1.Token,                       Credentials2.Token);
            ClassicAssert.AreEqual(Credentials1.URL,                         Credentials2.URL);
            ClassicAssert.AreEqual(Credentials1.Roles,                       Credentials2.Roles);

        }

        #endregion


        #region Credentials_DeserializeGitHub_Test01()

        /// <summary>
        /// Tries to deserialize a charge detail record example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/examples/credentials_example.json
        /// </summary>
        [Test]
        public static void Credentials_DeserializeGitHub_Test01()
        {

            #region Define JSON

            var JSON = @"{
                           ""token"": ""ebf3b399-779f-4497-9b9d-ac6ad3cc44d2"",
                           ""url"":   ""https://example.com/ocpi/versions/"",
                           ""roles"": [{
                               ""role"":         ""CPO"",
                               ""party_id"":     ""EXA"",
                               ""country_code"": ""NL"",
                               ""business_details"": {
                                   ""name"":     ""Example Operator""
                               }
                           }]
                         }";

            #endregion

            ClassicAssert.IsTrue(Credentials.TryParse(JObject.Parse(JSON), out var parsedCredentials, out var errorResponse));
            ClassicAssert.IsNull(errorResponse);

            ClassicAssert.AreEqual(AccessToken.Parse("ebf3b399-779f-4497-9b9d-ac6ad3cc44d2"),  parsedCredentials.Token);
            ClassicAssert.AreEqual(URL.        Parse("https://example.com/ocpi/versions/"),    parsedCredentials.URL);
            ClassicAssert.AreEqual(CountryCode.Parse("NL"),                                    parsedCredentials.Roles.First().PartyId.CountryCode);
            ClassicAssert.AreEqual(Party_Id.   Parse("EXA"),                                   parsedCredentials.Roles.First().PartyId.Party);
            ClassicAssert.AreEqual(Role.CPO,                                                   parsedCredentials.Roles.First().Role);
            ClassicAssert.AreEqual("Example Operator",                                         parsedCredentials.Roles.First().BusinessDetails.Name);

        }

        #endregion

        #region Credentials_DeserializeGitHub_Test02()

        /// <summary>
        /// Tries to deserialize a charge detail record example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/examples/credentials_example2.json
        /// </summary>
        [Test]
        public static void Credentials_DeserializeGitHub_Test02()
        {

            #region Define JSON

            var JSON = @"{
                           ""token"":  ""9e80a9c4-28be-11e9-b210-d663bd873d93"",
                           ""url"":    ""https://ocpi.example.com/versions/"",
                           ""roles"": [{
                               ""role"":          ""CPO"",
                               ""party_id"":      ""EXA"",
                               ""country_code"":  ""NL"",
                               ""business_details"": {
                                   ""name"":      ""Example Operator""
                               }
                           }, {
                               ""role"":          ""EMSP"",
                               ""party_id"":      ""EXA"",
                               ""country_code"":  ""NL"",
                               ""business_details"": {
                                   ""name"":      ""Example Provider""
                               }
                           }]
                         }";

            #endregion

            ClassicAssert.IsTrue(Credentials.TryParse(JObject.Parse(JSON), out var parsedCredentials, out var errorResponse));
            ClassicAssert.IsNull(errorResponse);

            ClassicAssert.AreEqual(AccessToken.Parse("9e80a9c4-28be-11e9-b210-d663bd873d93"),  parsedCredentials.Token);
            ClassicAssert.AreEqual(URL.        Parse("https://ocpi.example.com/versions/"),    parsedCredentials.URL);

            ClassicAssert.AreEqual(CountryCode.Parse("NL"),                                    parsedCredentials.Roles.        First().PartyId.CountryCode);
            ClassicAssert.AreEqual(Party_Id.   Parse("EXA"),                                   parsedCredentials.Roles.        First().PartyId.Party);
            ClassicAssert.AreEqual(Role.CPO,                                                   parsedCredentials.Roles.        First().Role);
            ClassicAssert.AreEqual("Example Operator",                                         parsedCredentials.Roles.        First().BusinessDetails.Name);

            // Note: Same CountryCode and PartyId, but different roles: Is this really a good idea?
            ClassicAssert.AreEqual(CountryCode.Parse("NL"),                                    parsedCredentials.Roles.Skip(1).First().PartyId.CountryCode);
            ClassicAssert.AreEqual(Party_Id.   Parse("EXA"),                                   parsedCredentials.Roles.Skip(1).First().PartyId.Party);
            ClassicAssert.AreEqual(Role.EMSP,                                                  parsedCredentials.Roles.Skip(1).First().Role);
            ClassicAssert.AreEqual("Example Provider",                                         parsedCredentials.Roles.Skip(1).First().BusinessDetails.Name);

        }

        #endregion

        #region Credentials_DeserializeGitHub_Test03()

        /// <summary>
        /// Tries to deserialize a charge detail record example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/examples/credentials_example3.json
        /// </summary>
        [Test]
        public static void Credentials_DeserializeGitHub_Test03()
        {

            #region Define JSON

            var JSON = @"{
                           ""token"":  ""9e80ae10-28be-11e9-b210-d663bd873d93"",
                           ""url"":    ""https://example.com/ocpi/versions/"",
                           ""roles"": [{
                               ""role"":              ""CPO"",
                               ""party_id"":          ""EXA"",
                               ""country_code"":      ""NL"",
                               ""business_details"": {
                                   ""name"":          ""Example Operator"",
                                   ""logo"": {
                                       ""url"":       ""https://example.com/img/logo.jpg"",
                                       ""thumbnail"": ""https://example.com/img/logo_thumb.jpg"",
                                       ""category"":  ""OPERATOR"",
                                       ""type"":      ""jpeg"",
                                       ""width"":     512,
                                       ""height"":    512
                                   },
                                   ""website"":       ""http://example.com""
                               }
                           }]
                         }";

            #endregion

            ClassicAssert.IsTrue(Credentials.TryParse(JObject.Parse(JSON), out var parsedCredentials, out var errorResponse));
            ClassicAssert.IsNull(errorResponse);

            ClassicAssert.AreEqual(AccessToken.Parse("9e80ae10-28be-11e9-b210-d663bd873d93"),    parsedCredentials.Token);
            ClassicAssert.AreEqual(URL.        Parse("https://example.com/ocpi/versions/"),      parsedCredentials.URL);

            ClassicAssert.AreEqual(CountryCode.Parse("NL"),                                      parsedCredentials.Roles.First().PartyId.CountryCode);
            ClassicAssert.AreEqual(Party_Id.   Parse("EXA"),                                     parsedCredentials.Roles.First().PartyId.Party);
            ClassicAssert.AreEqual(Role.CPO,                                                     parsedCredentials.Roles.First().Role);
            ClassicAssert.AreEqual(URL.        Parse("https://example.com/img/logo.jpg"),        parsedCredentials.Roles.First().BusinessDetails.Logo.URL);
            ClassicAssert.AreEqual(URL.        Parse("https://example.com/img/logo_thumb.jpg"),  parsedCredentials.Roles.First().BusinessDetails.Logo.Thumbnail);
            ClassicAssert.AreEqual(ImageCategory.OPERATOR,                                       parsedCredentials.Roles.First().BusinessDetails.Logo.Category);
            ClassicAssert.AreEqual(ImageFileType.jpeg,                                           parsedCredentials.Roles.First().BusinessDetails.Logo.Type);
            ClassicAssert.AreEqual(512,                                                          parsedCredentials.Roles.First().BusinessDetails.Logo.Width);
            ClassicAssert.AreEqual(512,                                                          parsedCredentials.Roles.First().BusinessDetails.Logo.Height);
            ClassicAssert.AreEqual(URL.        Parse("http://example.com"),                      parsedCredentials.Roles.First().BusinessDetails.Website);

        }

        #endregion

        #region Credentials_DeserializeGitHub_Test04()

        /// <summary>
        /// Tries to deserialize a charge detail record example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/examples/credentials_example4.json
        /// </summary>
        [Test]
        public static void Credentials_DeserializeGitHub_Test04()
        {

            #region Define JSON

            var JSON = @"{
                           ""token"":  ""9e80aca8-28be-11e9-b210-d663bd873d93"",
                           ""url"":    ""https://ocpi.example.com/versions/"",
                           ""roles"": [{
                               ""role"":         ""CPO"",
                               ""party_id"":     ""EXO"",
                               ""country_code"": ""NL"",
                               ""business_details"": {
                                   ""name"":     ""Excellent Operator""
                               }
                           }, {
                               ""role"":         ""CPO"",
                               ""party_id"":     ""PFC"",
                               ""country_code"": ""NL"",
                               ""business_details"": {
                                   ""name"":     ""Plug Flex Charging""
                               }
                           }, {
                               ""role"":         ""CPO"",
                               ""party_id"":     ""CGP"",
                               ""country_code"": ""NL"",
                               ""business_details"": {
                                   ""name"":     ""Charging Green Power""
                               }
                           }]
                         }";

            #endregion

            ClassicAssert.IsTrue(Credentials.TryParse(JObject.Parse(JSON), out var parsedCredentials, out var errorResponse));
            ClassicAssert.IsNull(errorResponse);

            ClassicAssert.AreEqual(AccessToken.Parse("9e80aca8-28be-11e9-b210-d663bd873d93"),  parsedCredentials.Token);
            ClassicAssert.AreEqual(URL.        Parse("https://ocpi.example.com/versions/"),    parsedCredentials.URL);

            ClassicAssert.AreEqual(CountryCode.Parse("NL"),                                    parsedCredentials.Roles.        First().PartyId.CountryCode);
            ClassicAssert.AreEqual(Party_Id.   Parse("EXO"),                                   parsedCredentials.Roles.        First().PartyId.Party);
            ClassicAssert.AreEqual(Role.CPO,                                                   parsedCredentials.Roles.        First().Role);
            ClassicAssert.AreEqual("Excellent Operator",                                       parsedCredentials.Roles.        First().BusinessDetails.Name);

            ClassicAssert.AreEqual(CountryCode.Parse("NL"),                                    parsedCredentials.Roles.Skip(1).First().PartyId.CountryCode);
            ClassicAssert.AreEqual(Party_Id.   Parse("PFC"),                                   parsedCredentials.Roles.Skip(1).First().PartyId.Party);
            ClassicAssert.AreEqual(Role.CPO,                                                   parsedCredentials.Roles.Skip(1).First().Role);
            ClassicAssert.AreEqual("Plug Flex Charging",                                       parsedCredentials.Roles.Skip(1).First().BusinessDetails.Name);

            ClassicAssert.AreEqual(CountryCode.Parse("NL"),                                    parsedCredentials.Roles.Skip(2).First().PartyId.CountryCode);
            ClassicAssert.AreEqual(Party_Id.   Parse("CGP"),                                   parsedCredentials.Roles.Skip(2).First().PartyId.Party);
            ClassicAssert.AreEqual(Role.CPO,                                                   parsedCredentials.Roles.Skip(2).First().Role);
            ClassicAssert.AreEqual("Charging Green Power",                                     parsedCredentials.Roles.Skip(2).First().BusinessDetails.Name);

        }

        #endregion


    }

}
