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
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.EMSPTests
{

    [TestFixture]
    public class Versions : ANodeTests
    {

        #region EMSP_GetVersions_Test()

        /// <summary>
        /// EMSP GetVersions Test.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_Test()
        {

            var graphDefinedCPO = emsp1EMSPAPI?.GetCPOClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetVersions();

                // GET /versions HTTP/1.1
                // Date:                          Sun, 25 Dec 2022 23:16:30 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
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
                //         "version":  "2.3.0.1",
                //         "url":      "http://127.0.0.1:7234/versions/2.3.0.1"
                //     }],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-25T23:16:31.228Z"
                // }

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,           response.StatusCode);
                ClassicAssert.AreEqual ("Hello world!", response.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var versions = response.Data;
                ClassicAssert.IsNotNull(versions);
                ClassicAssert.AreEqual (1, response.Data?.Count());

                var version  = versions?.First();
                ClassicAssert.IsTrue   (version?.Id == Version.Id);
                ClassicAssert.IsTrue   (URL.Parse("http://localhost:3301/ocpi/v2.3.0/versions/2.3.0.1") == version?.URL);

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

            var graphDefinedCPO = emsp1EMSPAPI?.GetCPOClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetVersionDetails(Version.Id);

                // GET /versions/2.3.0.1 HTTP/1.1
                // Date:                          Mon, 26 Dec 2022 00:36:20 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
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
                // Content-Length:                965
                // X-Request-ID:                  21Sj4CSUttCvUE7t8YdttY31YA1nzx
                // X-Correlation-ID:              jGrrQ7pEb541dCnUx12SEp3KG88YjG
                // 
                // {
                //     "data": {
                //         "version": "2.3.0.1",
                //         "endpoints": [
                //             {
                //                 "identifier":  "credentials",
                //                 "role":        "SENDER",
                //                 "url":         "http://127.0.0.1:7234/2.3.0.1/credentials"
                //             },
                //             {
                //                 "identifier":  "credentials",
                //                 "role":        "RECEIVER",
                //                 "url":         "http://127.0.0.1:7234/2.3.0.1/credentials"
                //             },
                //             {
                //                 "identifier":  "locations",
                //                 "role":        "SENDER",
                //                 "url":         "http://127.0.0.1:7234/2.3.0.1/cpo/locations"
                //             },
                //             {
                //                 "identifier":  "tariffs",
                //                 "role":        "SENDER",
                //                 "url":         "http://127.0.0.1:7234/2.3.0.1/cpo/tariffs"
                //             },
                //             {
                //                 "identifier":  "sessions",
                //                 "role":        "SENDER",
                //                 "url":         "http://127.0.0.1:7234/2.3.0.1/cpo/sessions"
                //             },
                //             {
                //                 "identifier":  "chargingprofiles",
                //                 "role":        "SENDER",
                //                 "url":         "http://127.0.0.1:7234/2.3.0.1/cpo/chargingprofiles"
                //             },
                //             {
                //                 "identifier":  "cdrs",
                //                 "role":        "SENDER",
                //                 "url":         "http://127.0.0.1:7234/2.3.0.1/cpo/cdrs"
                //             },
                //             {
                //                 "identifier":  "commands",
                //                 "role":        "RECEIVER",
                //                 "url":         "http://127.0.0.1:7234/2.3.0.1/cpo/commands"
                //             },
                //             {
                //                 "identifier":  "tokens",
                //                 "role":        "RECEIVER",
                //                 "url":         "http://127.0.0.1:7234/2.3.0.1/cpo/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-26T00:36:21.228Z"
                // }

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,           response.StatusCode);
                ClassicAssert.AreEqual ("Hello world!", response.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var versionDetail  = response.Data;
                ClassicAssert.IsNotNull(versionDetail);

                if (versionDetail is not null)
                {

                    var versionId      = versionDetail.VersionId;
                    ClassicAssert.IsTrue   (versionId == Version.Id);

                    var endpoints      = versionDetail.Endpoints;
                    ClassicAssert.AreEqual (9, endpoints.Count());

                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "credentials"      && endpoint.Role == InterfaceRoles.SENDER));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "credentials"      && endpoint.Role == InterfaceRoles.RECEIVER));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "locations"        && endpoint.Role == InterfaceRoles.SENDER));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "tariffs"          && endpoint.Role == InterfaceRoles.SENDER));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "sessions"         && endpoint.Role == InterfaceRoles.SENDER));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "chargingprofiles" && endpoint.Role == InterfaceRoles.SENDER));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "cdrs"             && endpoint.Role == InterfaceRoles.SENDER));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "commands"         && endpoint.Role == InterfaceRoles.RECEIVER));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "tokens"           && endpoint.Role == InterfaceRoles.RECEIVER));

                }

            }

        }

        #endregion

        #region EMSP_GetVersion_UnknownVersion_Test()

        /// <summary>
        /// EMSP GetVersion Unknown Version Test.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_UnknownVersion_Test()
        {

            var graphDefinedCPO = emsp1EMSPAPI?.GetCPOClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetVersionDetails(Version_Id.Parse("0.7"));

                // GET /versions/0.7 HTTP/1.1
                // Date:              Mon, 26 Dec 2022 00:36:20 GMT
                // Accept:            application/json; charset=utf-8;q=1
                // Host:              127.0.0.1:7234
                // Authorization:     Token xxxxxx
                // User-Agent:        GraphDefined OCPI HTTP Client v1.0
                // X-Request-ID:      21Sj4CSUttCvUE7t8YdttY31YA1nzx
                // X-Correlation-ID:  jGrrQ7pEb541dCnUx12SEp3KG88YjG

                ClassicAssert.IsNotNull(response);

                ClassicAssert.AreEqual (-1,                                response.StatusCode); // local error!
                ClassicAssert.AreEqual ("Unknown version identification!",  response.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                // There is not HTTP response, as this is a local error!
                ClassicAssert.IsNull   (response.HTTPResponse);
                ClassicAssert.IsNull   (response.RequestId);
                ClassicAssert.IsNull   (response.CorrelationId);

            }

        }

        #endregion

        #region EMSP_GetVersion_UnknownVersion_viaHTTP_Test()

        /// <summary>
        /// EMSP GetVersion Unknown Version via HTTP Test.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_UnknownVersion_viaHTTP_Test()
        {

            var graphDefinedCPO = emsp1EMSPAPI?.GetCPOClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var httpResponse = await TestHelpers.JSONRequest(URL.Parse("http://127.0.0.1:3301/ocpi/v2.3.0/versions/v0.7"),
                                                                 "xxxxxx");

                // GET /ocpi/v2.3.0/versions/v0.7 HTTP/1.1
                // Date:                          Sat, 22 Apr 2023 11:54:54 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7235
                // Authorization:                 Token xxxxxx
                // X-Request-ID:                  1234
                // X-Correlation-ID:              5678

                // HTTP/1.1 404 Not Found
                // Date:                          Sat, 22 Apr 2023 11:54:54 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                114
                // X-Request-ID:                  1234
                // X-Correlation-ID:              5678
                // 
                // {
                //     "status_code":      2000,
                //     "status_message":  "This OCPI version is not supported!",
                //     "timestamp":       "2023-04-22T11:54:54.800Z"
                // }

                ClassicAssert.IsNotNull(httpResponse);
                ClassicAssert.IsTrue   (httpResponse.ContentLength > 0);

                var response = OCPIResponse.Parse(httpResponse,
                                                  Request_Id.    Parse("12340"),
                                                  Correlation_Id.Parse("56780"));

                ClassicAssert.AreEqual (2000,                                   response.StatusCode);
                ClassicAssert.AreEqual ("This OCPI version is not supported!",  response.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                ClassicAssert.IsNotNull(response.HTTPResponse);

            }

        }

        #endregion


    }

}
