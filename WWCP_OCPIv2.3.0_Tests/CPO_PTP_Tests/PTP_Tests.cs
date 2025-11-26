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

using Newtonsoft.Json.Linq;

using NUnit.Framework;
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.CPO_PTP
{

    [TestFixture]
    public class PTP_Tests : ACPO_PTP_Tests
    {

        #region GetVersions_FromCPO_Test()

        /// <summary>
        /// The PTP will fetch the OCPI Versions from the CPO.
        /// </summary>
        [Test]
        public async Task GetVersions_FromCPO_Test()
        {

            var graphDefinedCPO = ptpAPI?.GetCPOClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.That(graphDefinedCPO,  Is.Not.Null);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          127.0.0.1:3801
                // Authorization:                 Token cHRwLTItY3BvOnRva2Vu
                // User-Agent:                    GraphDefined OCPI v2.3.0 Common Client
                // Connection:                    close
                // X-Request-ID:                  f21C5K7hC1bA5drA8zr7b797U531fQ
                // X-Correlation-ID:              M1hd7pfY8UhYxnAY8MQC8W3SQ46fEG

                // HTTP/1.1 200 OK
                // Date:                          Wed, 12 Mar 2025 15:58:57 GMT
                // Server:                        GraphDefined OCPI Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Allow:                         OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                131
                // Connection:                    close
                // Vary:                          Accept
                // X-Request-ID:                  f21C5K7hC1bA5drA8zr7b797U531fQ
                // X-Correlation-ID:              M1hd7pfY8UhYxnAY8MQC8W3SQ46fEG
                // 
                // {
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "data": [
                //         {
                //             "version":  "2.3.0",
                //             "url":      "http://localhost:3801/ocpi/versions/2.3.0"
                //         }
                //     ]
                // }

                Assert.That(response,                                                                            Is.Not.Null);

                Assert.That(response.HTTPResponse?.HTTPRequest,                                                  Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPRequest?.Accept,                                          Is.EqualTo(AcceptTypes.FromHTTPContentTypes(HTTPContentType.Application.JSON_UTF8)));
                Assert.That(response.HTTPResponse?.HTTPRequest?.Host,                                            Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPRequest?.Authorization?.ToString()?.StartsWith("Token"),  Is.True);
                Assert.That(response.HTTPResponse?.HTTPRequest?.UserAgent,                                       Is.EqualTo("GraphDefined OCPI v2.3.0 Common Client"));
                Assert.That(response.HTTPResponse?.HTTPRequest?.Connection,                                      Is.EqualTo(ConnectionType.Close));
                Assert.That(response.HTTPResponse?.HTTPRequest?.GetHeaderField("X-Request-ID"),                  Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPRequest?.GetHeaderField("X-Correlation-ID"),              Is.Not.Null);

                Assert.That(response.HTTPResponse,                                                               Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                                          Is.EqualTo(200));
                Assert.That(response.HTTPResponse?.Date,                                                         Is.Not.Null);
                Assert.That(response.HTTPResponse?.Server,                                                       Is.EqualTo("GraphDefined OCPI Common HTTP API"));
                Assert.That(response.HTTPResponse?.AccessControlAllowOrigin,                                     Is.EqualTo("*"));
                Assert.That(response.HTTPResponse?.AccessControlAllowMethods,                                    Is.EqualTo([ "OPTIONS", "GET" ]));
                Assert.That(response.HTTPResponse?.AccessControlAllowHeaders,                                    Is.EqualTo([ "Authorization" ]));
                Assert.That(response.HTTPResponse?.Allow,                                                        Is.EqualTo([ HTTPMethod.OPTIONS, HTTPMethod.GET ]));
                Assert.That(response.HTTPResponse?.ContentType,                                                  Is.EqualTo(HTTPContentType.Application.JSON_UTF8));
                Assert.That(response.HTTPResponse?.GetHeaderField("X-Request-ID"),                               Is.EqualTo(response.HTTPResponse?.HTTPRequest?.GetHeaderField("X-Request-ID")));
                Assert.That(response.HTTPResponse?.GetHeaderField("X-Correlation-ID"),                           Is.EqualTo(response.HTTPResponse?.HTTPRequest?.GetHeaderField("X-Correlation-ID")));

                Assert.That(response.StatusCode,                                                                 Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                                              Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now -  response.Timestamp,                                                 Is.LessThan(TimeSpan.FromSeconds(10)));

                //Assert.That(response.Request,  Is.Not.Null);

                Assert.That(response.Data,                                                                       Is.Not.Null);
                Assert.That(response?.Data?.Count(),                                                             Is.EqualTo(1));
                Assert.That(response?.Data?.First().Id,                                                          Is.EqualTo(Version_Id.Parse("2.3.0")));
                Assert.That(response?.Data?.First().URL,                                                         Is.EqualTo(cpoAPI?.CommonAPI.BaseAPI.OurVersionsURL + "2.3.0"));

            }

        }

        #endregion


        #region GetVersionDetails_FromCPO_Test()

        /// <summary>
        /// The PTP will fetch the OCPI VersionDetails from the CPO.
        /// </summary>
        [Test]
        public async Task GetVersionDetails_FromCPO_Test()
        {

            var graphDefinedCPO = ptpAPI?.GetCPOClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.That(graphDefinedCPO,  Is.Not.Null);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetVersionDetails(Version_Id.Parse("2.3.0"));

                // GET /versions/2.3.0 HTTP/1.1
                // Date:                          Mon, 26 Dec 2022 00:36:20 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7235
                // Authorization:                 Token yyyyyy
                // User-Agent:                    GraphDefined OCPI HTTP Client v1.0
                // X-Request-ID:                  MpG9fA2Mjr89K16r82phMA18r83CS9
                // X-Correlation-ID:              S1p1rKhhh96vEK8A8t84Sht382KE8f

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 00:36:21 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Vary:                          Accept
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                653
                // X-Request-ID:                  MpG9fA2Mjr89K16r82phMA18r83CS9
                // X-Correlation-ID:              S1p1rKhhh96vEK8A8t84Sht382KE8f
                // 
                // {
                //     "data": {
                //         "version": "2.3.0",
                //         "endpoints": [
                //             {
                //                 "identifier":  "credentials",
                //                 "url":         "http://127.0.0.1:7235/2.3.0/credentials"
                //             },
                //             {
                //                 "identifier":  "locations",
                //                 "url":         "http://127.0.0.1:7235/2.3.0/emsp/locations"
                //             },
                //             {
                //                 "identifier":  "tariffs",
                //                 "url":         "http://127.0.0.1:7235/2.3.0/emsp/tariffs"
                //             },
                //             {
                //                 "identifier":  "sessions",
                //                 "url":         "http://127.0.0.1:7235/2.3.0/emsp/sessions"
                //             },
                //             {
                //                 "identifier":  "cdrs",
                //                 "url":         "http://127.0.0.1:7235/2.3.0/emsp/cdrs"
                //             },
                //             {
                //                 "identifier":  "commands",
                //                 "url":         "http://127.0.0.1:7235/2.3.0/emsp/commands"
                //             },
                //             {
                //                 "identifier":  "tokens",
                //                 "url":         "http://127.0.0.1:7235/2.3.0/emsp/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-26T00:36:21.259Z"
                // }

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,             response.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,            response.StatusCode);
                ClassicAssert.AreEqual ("Hello world!",  response.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

                var versionDetail = response.Data;
                ClassicAssert.IsNotNull(versionDetail);

                var endpoints = versionDetail.Endpoints;
                ClassicAssert.AreEqual (7, endpoints.Count());
                //ClassicAssert.AreEqual(Version_Id.Parse("2.3.0"), endpoints.Id);
                //ClassicAssert.AreEqual(emspVersionsAPIURL + "2.3.0", endpoints.URL);


            }

        }

        #endregion


    }

}
