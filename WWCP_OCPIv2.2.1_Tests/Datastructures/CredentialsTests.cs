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

            Assert.That(JSON["token"].                                            Value<String>(), Is.EqualTo("4285n43805fng38"));
            Assert.That(JSON["url"].                                              Value<String>(), Is.EqualTo("http://open.charging.cloud/versions"));

            Assert.That(JSON["roles"][0]["country_code"].                         Value<String>(), Is.EqualTo("DE"));
            Assert.That(JSON["roles"][0]["party_id"].                             Value<String>(), Is.EqualTo("GEF"));
            Assert.That(JSON["roles"][0]["role"].                                 Value<String>(), Is.EqualTo("CPO"));
            Assert.That(JSON["roles"][0]["business_details"]["name"].             Value<String>(), Is.EqualTo("Open Charging Cloud CPO"));
            Assert.That(JSON["roles"][0]["business_details"]["website"].          Value<String>(), Is.EqualTo("http://cpo.charging.cloud"));
            Assert.That(JSON["roles"][0]["business_details"]["logo"]["url"].      Value<String>(), Is.EqualTo("http://cpo.charging.cloud/logo"));
            Assert.That(JSON["roles"][0]["business_details"]["logo"]["thumbnail"].Value<String>(), Is.EqualTo("http://cpo.charging.cloud/logo_small"));
            Assert.That(JSON["roles"][0]["business_details"]["logo"]["category"]. Value<String>(), Is.EqualTo("OPERATOR"));
            Assert.That(JSON["roles"][0]["business_details"]["logo"]["type"].     Value<String>(), Is.EqualTo("svg"));
            Assert.That(JSON["roles"][0]["business_details"]["logo"]["width"].    Value<UInt16>(), Is.EqualTo(1000));
            Assert.That(JSON["roles"][0]["business_details"]["logo"]["height"].   Value<UInt16>(), Is.EqualTo(1500));

            Assert.That(JSON["roles"][1]["country_code"].                         Value<String>(), Is.EqualTo("DE"));
            Assert.That(JSON["roles"][1]["party_id"].                             Value<String>(), Is.EqualTo("GDF"));
            Assert.That(JSON["roles"][1]["role"].                                 Value<String>(), Is.EqualTo("EMSP"));
            Assert.That(JSON["roles"][1]["business_details"]["name"].             Value<String>(), Is.EqualTo("Open Charging Cloud EMSP"));
            Assert.That(JSON["roles"][1]["business_details"]["website"].          Value<String>(), Is.EqualTo("http://emsp.charging.cloud"));
            Assert.That(JSON["roles"][1]["business_details"]["logo"]["url"].      Value<String>(), Is.EqualTo("http://emsp.charging.cloud/logo"));
            Assert.That(JSON["roles"][1]["business_details"]["logo"]["thumbnail"].Value<String>(), Is.EqualTo("http://emsp.charging.cloud/logo_small"));
            Assert.That(JSON["roles"][1]["business_details"]["logo"]["category"]. Value<String>(), Is.EqualTo("NETWORK"));
            Assert.That(JSON["roles"][1]["business_details"]["logo"]["type"].     Value<String>(), Is.EqualTo("png"));
            Assert.That(JSON["roles"][1]["business_details"]["logo"]["width"].    Value<UInt16>(), Is.EqualTo(2000));
            Assert.That(JSON["roles"][1]["business_details"]["logo"]["height"].   Value<UInt16>(), Is.EqualTo(3000));


            Assert.That(Credentials.TryParse(JSON, out Credentials Credentials2, out String ErrorResponse), Is.True);
            Assert.That(ErrorResponse,                                                                      Is.Null);

            Assert.That(Credentials2.Token, Is.EqualTo(Credentials1.Token));
            Assert.That(Credentials2.URL,   Is.EqualTo(Credentials1.URL));
            Assert.That(Credentials2.Roles, Is.EqualTo(Credentials1.Roles));

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

            Assert.That(Credentials.TryParse(JObject.Parse(JSON), out var parsedCredentials, out var errorResponse), Is.True);
            Assert.That(errorResponse,                                                                               Is.Null);

            Assert.That(parsedCredentials.Token,                              Is.EqualTo(AccessToken.Parse("ebf3b399-779f-4497-9b9d-ac6ad3cc44d2")));
            Assert.That(parsedCredentials.URL,                                Is.EqualTo(URL.        Parse("https://example.com/ocpi/versions/")));
            Assert.That(parsedCredentials.Roles.First().PartyId.CountryCode,  Is.EqualTo(CountryCode.Parse("NL")));
            Assert.That(parsedCredentials.Roles.First().PartyId.PartyId,      Is.EqualTo(Party_Id.   Parse("EXA")));
            Assert.That(parsedCredentials.Roles.First().Role,                 Is.EqualTo(Role.CPO));
            Assert.That(parsedCredentials.Roles.First().BusinessDetails.Name, Is.EqualTo("Example Operator"));

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

            Assert.That(Credentials.TryParse(JObject.Parse(JSON), out var parsedCredentials, out var errorResponse), Is.True);
            Assert.That(errorResponse,                                                                               Is.Null);

            Assert.That(parsedCredentials.Token, Is.EqualTo(AccessToken.Parse("9e80a9c4-28be-11e9-b210-d663bd873d93")));
            Assert.That(parsedCredentials.URL,   Is.EqualTo(URL.        Parse("https://ocpi.example.com/versions/")));

            Assert.That(parsedCredentials.Roles.        First().PartyId.CountryCode,  Is.EqualTo(CountryCode.Parse("NL")));
            Assert.That(parsedCredentials.Roles.        First().PartyId.PartyId,      Is.EqualTo(Party_Id.   Parse("EXA")));
            Assert.That(parsedCredentials.Roles.        First().Role,                 Is.EqualTo(Role.CPO));
            Assert.That(parsedCredentials.Roles.        First().BusinessDetails.Name, Is.EqualTo("Example Operator"));

            // Note: Same CountryCode and PartyId, but different roles: Is this really a good idea?
            Assert.That(parsedCredentials.Roles.Skip(1).First().PartyId.CountryCode,  Is.EqualTo(CountryCode.Parse("NL")));
            Assert.That(parsedCredentials.Roles.Skip(1).First().PartyId.PartyId,      Is.EqualTo(Party_Id.   Parse("EXA")));
            Assert.That(parsedCredentials.Roles.Skip(1).First().Role,                 Is.EqualTo(Role.EMSP));
            Assert.That(parsedCredentials.Roles.Skip(1).First().BusinessDetails.Name, Is.EqualTo("Example Provider"));

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

            Assert.That(Credentials.TryParse(JObject.Parse(JSON), out var parsedCredentials, out var errorResponse), Is.True);
            Assert.That(errorResponse,                                                                               Is.Null);

            Assert.That(parsedCredentials.Token, Is.EqualTo(AccessToken.Parse("9e80ae10-28be-11e9-b210-d663bd873d93")));
            Assert.That(parsedCredentials.URL,   Is.EqualTo(URL.        Parse("https://example.com/ocpi/versions/")));

            Assert.That(parsedCredentials.Roles.First().PartyId.CountryCode,            Is.EqualTo(CountryCode.Parse("NL")));
            Assert.That(parsedCredentials.Roles.First().PartyId.PartyId,                Is.EqualTo(Party_Id.   Parse("EXA")));
            Assert.That(parsedCredentials.Roles.First().Role,                           Is.EqualTo(Role.CPO));
            Assert.That(parsedCredentials.Roles.First().BusinessDetails.Logo.URL,       Is.EqualTo(URL.        Parse("https://example.com/img/logo.jpg")));
            Assert.That(parsedCredentials.Roles.First().BusinessDetails.Logo.Thumbnail, Is.EqualTo(URL.        Parse("https://example.com/img/logo_thumb.jpg")));
            Assert.That(parsedCredentials.Roles.First().BusinessDetails.Logo.Category,  Is.EqualTo(ImageCategory.OPERATOR));
            Assert.That(parsedCredentials.Roles.First().BusinessDetails.Logo.Type,      Is.EqualTo(ImageFileType.jpeg));
            Assert.That(parsedCredentials.Roles.First().BusinessDetails.Logo.Width,     Is.EqualTo(512));
            Assert.That(parsedCredentials.Roles.First().BusinessDetails.Logo.Height,    Is.EqualTo(512));
            Assert.That(parsedCredentials.Roles.First().BusinessDetails.Website,        Is.EqualTo(URL.        Parse("http://example.com")));

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

            Assert.That(Credentials.TryParse(JObject.Parse(JSON), out var parsedCredentials, out var errorResponse), Is.True);
            Assert.That(errorResponse,                                                                               Is.Null);

            Assert.That(parsedCredentials.Token, Is.EqualTo(AccessToken.Parse("9e80aca8-28be-11e9-b210-d663bd873d93")));
            Assert.That(parsedCredentials.URL,   Is.EqualTo(URL.        Parse("https://ocpi.example.com/versions/")));

            Assert.That(parsedCredentials.Roles.        First().PartyId.CountryCode,  Is.EqualTo(CountryCode.Parse("NL")));
            Assert.That(parsedCredentials.Roles.        First().PartyId.PartyId,      Is.EqualTo(Party_Id.   Parse("EXO")));
            Assert.That(parsedCredentials.Roles.        First().Role,                 Is.EqualTo(Role.CPO));
            Assert.That(parsedCredentials.Roles.        First().BusinessDetails.Name, Is.EqualTo("Excellent Operator"));

            Assert.That(parsedCredentials.Roles.Skip(1).First().PartyId.CountryCode,  Is.EqualTo(CountryCode.Parse("NL")));
            Assert.That(parsedCredentials.Roles.Skip(1).First().PartyId.PartyId,      Is.EqualTo(Party_Id.   Parse("PFC")));
            Assert.That(parsedCredentials.Roles.Skip(1).First().Role,                 Is.EqualTo(Role.CPO));
            Assert.That(parsedCredentials.Roles.Skip(1).First().BusinessDetails.Name, Is.EqualTo("Plug Flex Charging"));

            Assert.That(parsedCredentials.Roles.Skip(2).First().PartyId.CountryCode,  Is.EqualTo(CountryCode.Parse("NL")));
            Assert.That(parsedCredentials.Roles.Skip(2).First().PartyId.PartyId,      Is.EqualTo(Party_Id.   Parse("CGP")));
            Assert.That(parsedCredentials.Roles.Skip(2).First().Role,                 Is.EqualTo(Role.CPO));
            Assert.That(parsedCredentials.Roles.Skip(2).First().BusinessDetails.Name, Is.EqualTo("Charging Green Power"));

        }

        #endregion


    }

}
