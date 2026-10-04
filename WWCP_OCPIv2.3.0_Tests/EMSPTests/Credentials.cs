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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.EMSPTests
{

    [TestFixture]
    public class Credentials : ANodeTests
    {

        #region EMSP_GetCredentials_Test1()

        /// <summary>
        /// EMSP GetCredentials Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetCredentials_Test1()
        {

            var graphDefinedCPO = emsp1EMSPAPI?.GetCPOClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.That(graphDefinedCPO, Is.Not.Null);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetCredentials();

                // GET /2.3.0.1/credentials HTTP/1.1
                // Date:                          Mon, 26 Dec 2022 10:29:48 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
                // Authorization:                 Token xxxxxx
                // X-Request-ID:                  27ExnKK8fK3vhQvYA8dY7A2rx9KUtC
                // X-Correlation-ID:              325WrvpzKh6f238Mb4Ex17v1612365

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 10:29:49 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                296
                // X-Request-ID:                  27ExnKK8fK3vhQvYA8dY7A2rx9KUtC
                // X-Correlation-ID:              325WrvpzKh6f238Mb4Ex17v1612365
                // 
                // {
                //    "data": {
                //        "token":         "cso-2-emp1:token",
                //        "url":           "http://127.0.0.1:3301/ocpi/v2.3.0/versions",
                //        "business_details": {
                //            "name":           "GraphDefined CSO Services",
                //            "website":        "https://www.graphdefined.com/cso"
                //        },
                //        "country_code":  "DE",
                //        "party_id":      "GEF"
                //    },
                //    "status_code":      1000,
                //    "status_message":  "Hello world!",
                //    "timestamp":       "2022-12-26T10:29:49.143Z"
                //}

                Assert.That(response,                                                      Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                    Is.EqualTo(200));
                Assert.That(response.StatusCode.Value,                                     Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                        Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10), Is.True);

                var credentials = response.Data;
                Assert.That(credentials, Is.Not.Null);

                if (credentials is not null)
                {

                    Assert.That(credentials.    Token.                            ToString(),   Is.EqualTo("cso-2-emp1:token"));
                    Assert.That(credentials.    URL.                              ToString(),   Is.EqualTo(cpoVersionsAPIURL!.Value.ToString()));
                    Assert.That(credentials.    Roles.First().PartyId.CountryCode.ToString(),   Is.EqualTo("DE"));
                    Assert.That(credentials.    Roles.First().PartyId.PartyId.      ToString(), Is.EqualTo("GEF"));

                    var businessDetails = credentials.Roles.First().BusinessDetails;
                    Assert.That(businessDetails,                                              Is.Not.Null);
                    Assert.That(businessDetails.Name,                                         Is.EqualTo("GraphDefined CSO Services"));
                    Assert.That(businessDetails.Website.                          ToString(), Is.EqualTo("https://www.graphdefined.com/cso"));

                }

            }

        }

        #endregion

        #region EMSP_GetCredentials_Test2()

        /// <summary>
        /// EMSP GetCredentials Test 2.
        /// </summary>
        [Test]
        public async Task EMSP_GetCredentials_Test2()
        {

            var graphDefinedCPO = emsp2EMSPAPI?.GetCPOClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.That(graphDefinedCPO, Is.Not.Null);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetCredentials();

                // GET /2.3.0.1/credentials HTTP/1.1
                // Date:                          Mon, 26 Dec 2022 10:29:48 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
                // Authorization:                 Token xxxxxx
                // X-Request-ID:                  27ExnKK8fK3vhQvYA8dY7A2rx9KUtC
                // X-Correlation-ID:              325WrvpzKh6f238Mb4Ex17v1612365

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 10:29:49 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                296
                // X-Request-ID:                  27ExnKK8fK3vhQvYA8dY7A2rx9KUtC
                // X-Correlation-ID:              325WrvpzKh6f238Mb4Ex17v1612365
                // 
                // {
                //    "data": {
                //        "token":         "xxxxxx",
                //        "url":           "http://127.0.0.1:7234/versions",
                //        "business_details": {
                //            "name":           "GraphDefined CPO Services",
                //            "website":        "https://www.graphdefined.com/cpo"
                //        },
                //        "country_code":  "DE",
                //        "party_id":      "GEF"
                //    },
                //    "status_code":      1000,
                //    "status_message":  "Hello world!",
                //    "timestamp":       "2022-12-26T10:29:49.143Z"
                //}

                Assert.That(response,                                                      Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                    Is.EqualTo(200));
                Assert.That(response.StatusCode.Value,                                     Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                        Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10), Is.True);

                var credentials = response.Data;
                Assert.That(credentials, Is.Not.Null);

                if (credentials is not null)
                {

                    Assert.That(credentials.    Token.                            ToString(),   Is.EqualTo("cso-2-emp2:token"));
                    Assert.That(credentials.    URL.                              ToString(),   Is.EqualTo(cpoVersionsAPIURL!.Value.ToString()));
                    Assert.That(credentials.    Roles.First().PartyId.CountryCode.ToString(),   Is.EqualTo("DE"));
                    Assert.That(credentials.    Roles.First().PartyId.PartyId.      ToString(), Is.EqualTo("GEF"));

                    var businessDetails = credentials.Roles.First().BusinessDetails;
                    Assert.That(businessDetails,                                              Is.Not.Null);
                    Assert.That(businessDetails.Name,                                         Is.EqualTo("GraphDefined CSO Services"));
                    Assert.That(businessDetails.Website.                          ToString(), Is.EqualTo("https://www.graphdefined.com/cso"));

                }

            }

        }

        #endregion


    }

}
