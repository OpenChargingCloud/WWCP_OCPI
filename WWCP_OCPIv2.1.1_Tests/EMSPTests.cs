/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Aegir;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests
{

    [TestFixture]
    public class EMSPTests : ANodeTests
    {

        #region EMSP_GetVersions_Test()

        /// <summary>
        /// EMSP GetVersions Test 01.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_Test()
        {

            var graphDefinedCPO = emspWebAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetVersions();

                // GET /versions HTTP/1.1
                // Date:                          Sun, 25 Dec 2022 23:16:30 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7134
                // Authorization:                 Token xxxxxx
                // User-Agent:                    GraphDefined OCPI HTTP Client v1.0
                // X-Request-ID:                  Wz8f2E9YKMj7Wdx6pQ3YjbQhd86EYG
                // X-Correlation-ID:              2AYdzYY6zK6Y59hS5bn3n8A3Kt7C6x

                // HTTP/1.1 200 OK
                // Date:                          Sun, 25 Dec 2022 23:16:31 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Vary:                          Accept
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                165
                // X-Request-ID:                  Wz8f2E9YKMj7Wdx6pQ3YjbQhd86EYG
                // X-Correlation-ID:              2AYdzYY6zK6Y59hS5bn3n8A3Kt7C6x
                // 
                // {
                //     "data": [{
                //         "version":  "2.1.1",
                //         "url":      "http://127.0.0.1:7134/versions/2.1.1"
                //     }],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-25T23:16:31.228Z"
                // }

                Assert.IsNotNull(response);
                Assert.AreEqual (200,             response.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response.StatusCode);
                Assert.AreEqual ("Hello world!",  response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var versions = response.Data;
                Assert.IsNotNull(versions);
                Assert.AreEqual (1, response.Data.Count());

                var version = versions.First();
                Assert.AreEqual (Version_Id.Parse("2.1.1"),    version.Id);
                Assert.AreEqual (cpoVersionsAPIURL + "2.1.1",  version.URL);

            }

        }

        #endregion


        #region EMSP_GetVersion_Test()

        /// <summary>
        /// EMSP GetVersion Test 01.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_Test()
        {

            var graphDefinedCPO = emspWebAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response1 = await graphDefinedCPO.GetVersions();
                var response2 = await graphDefinedCPO.GetVersionDetails(Version_Id.Parse("2.1.1"));

                // GET /versions/2.1.1 HTTP/1.1
                // Date:                          Mon, 26 Dec 2022 00:36:20 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7134
                // Authorization:                 Token xxxxxx
                // User-Agent:                    GraphDefined OCPI HTTP Client v1.0
                // X-Request-ID:                  21Sj4CSUttCvUE7t8YdttY31YA1nzx
                // X-Correlation-ID:              jGrrQ7pEb541dCnUx12SEp3KG88YjG

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 00:36:21 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Vary:                          Accept
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                647
                // X-Request-ID:                  21Sj4CSUttCvUE7t8YdttY31YA1nzx
                // X-Correlation-ID:              jGrrQ7pEb541dCnUx12SEp3KG88YjG
                // 
                // {
                //     "data": {
                //         "version": "2.1.1",
                //         "endpoints": [
                //             {
                //                 "identifier":  "credentials",
                //                 "url":         "http://127.0.0.1:7134/2.1.1/credentials"
                //             },
                //             {
                //                 "identifier":  "locations",
                //                 "url":         "http://127.0.0.1:7134/2.1.1/cpo/locations"
                //             },
                //             {
                //                 "identifier":  "tariffs",
                //                 "url":         "http://127.0.0.1:7134/2.1.1/cpo/tariffs"
                //             },
                //             {
                //                 "identifier":  "sessions",
                //                 "url":         "http://127.0.0.1:7134/2.1.1/cpo/sessions"
                //             },
                //             {
                //                 "identifier":  "cdrs",
                //                 "url":         "http://127.0.0.1:7134/2.1.1/cpo/cdrs"
                //             },
                //             {
                //                 "identifier":  "commands",
                //                 "url":         "http://127.0.0.1:7134/2.1.1/cpo/commands"
                //             },
                //             {
                //                 "identifier":  "tokens",
                //                 "url":         "http://127.0.0.1:7134/2.1.1/cpo/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-26T00:36:21.259Z"
                // }

                Assert.IsNotNull(response2);
                Assert.AreEqual (200,             response2.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response2.StatusCode);
                Assert.AreEqual ("Hello world!",  response2.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var versionDetail = response2.Data;
                Assert.IsNotNull(versionDetail);

                var endpoints = versionDetail.Endpoints;
                Assert.AreEqual (7, endpoints.Count());
                //Assert.AreEqual(Version_Id.Parse("2.1.1"), endpoints.Id);
                //Assert.AreEqual(cpoVersionsAPIURL + "2.1.1", endpoints.URL);


            }

        }

        #endregion


        #region EMSP_GetCredentials_Test()

        /// <summary>
        /// EMSP GetCredentials Test 01.
        /// </summary>
        [Test]
        public async Task EMSP_GetCredentials_Test()
        {

            var graphDefinedCPO = emspWebAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response1 = await graphDefinedCPO.GetVersions();
                var response2 = await graphDefinedCPO.GetCredentials();

                // GET /2.1.1/credentials HTTP/1.1
                // Date:                          Mon, 26 Dec 2022 10:29:48 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7134
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
                //        "url":           "http://127.0.0.1:7134/versions",
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

                Assert.IsNotNull(response2);
                Assert.AreEqual (200,             response2.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response2.StatusCode);
                Assert.AreEqual ("Hello world!",  response2.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var credentials = response2.Data;
                Assert.IsNotNull(credentials);
                Assert.AreEqual("xxxxxx",                            credentials.    Token.      ToString());
                Assert.AreEqual("http://127.0.0.1:7134/versions",    credentials.    URL.        ToString());
                Assert.AreEqual("DE",                                credentials.    CountryCode.ToString());
                Assert.AreEqual("GEF",                               credentials.    PartyId.    ToString());

                var businessDetails = credentials.BusinessDetails;
                Assert.IsNotNull(businessDetails);
                Assert.AreEqual("GraphDefined CPO Services",         businessDetails.Name);
                Assert.AreEqual("https://www.graphdefined.com/cpo",  businessDetails.Website.    ToString());

            }

        }

        #endregion


    }

}
