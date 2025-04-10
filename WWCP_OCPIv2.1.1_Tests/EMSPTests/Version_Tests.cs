/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using NUnit.Framework;
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.EMSPTests
{

    [TestFixture]
    public class Version_Tests : ANodeTests
    {

        #region EMSP_GetVersion_RegisteredToken_Test()

        /// <summary>
        /// EMSP GetVersion with a registered access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_RegisteredToken_Test()
        {

            var graphDefinedCPO = emsp1EMSPAPI?.GetCPOClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetVersionDetails(Version.Id);

                // GET /ocpi/v2.1/versions/2.1.1 HTTP/1.1
                // Date:                            Sun, 30 Apr 2023 01:43:15 GMT
                // Accept:                          application/json; charset=utf-8;q=1
                // Host:                            localhost:3301
                // Authorization:                   Token emsp1_accessing_cpo++token
                // User-Agent:                      GraphDefined OCPI HTTP Client v1.0
                // X-Request-ID:                    4xAAKvW377Cd7C7rn2fMC5xb1WYKS8
                // X-Correlation-ID:                CWE5292n4bYbQ94M2p36GAdSnAS1b7

                // HTTP/1.1 200 OK
                // Date:                            Sun, 30 Apr 2023 01:43:15 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Vary:                            Accept
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  724
                // X-Request-ID:                    4xAAKvW377Cd7C7rn2fMC5xb1WYKS8
                // X-Correlation-ID:                CWE5292n4bYbQ94M2p36GAdSnAS1b7
                // 
                // {
                //     "data": {
                //         "version": "2.1.1",
                //         "endpoints": [
                //             {
                //                 "identifier":  "credentials",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/credentials"
                //             },
                //             {
                //                 "identifier":  "locations",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/cpo/locations"
                //             },
                //             {
                //                 "identifier":  "tariffs",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/cpo/tariffs"
                //             },
                //             {
                //                 "identifier":  "sessions",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/cpo/sessions"
                //             },
                //             {
                //                 "identifier":  "cdrs",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/cpo/cdrs"
                //             },
                //             {
                //                 "identifier":  "commands",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/cpo/commands"
                //             },
                //             {
                //                 "identifier":  "tokens",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/cpo/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-30T01:43:15.277Z"
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
                    ClassicAssert.AreEqual (7, endpoints.Count());

                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "credentials"));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "locations"));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "tariffs"));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "sessions"));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "cdrs"));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "commands"));
                    ClassicAssert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "tokens"));

                }

            }

        }

        #endregion

        #region EMSP_GetVersion_UnknownVersion_Test()

        /// <summary>
        /// EMSP GetVersion of an unknown OCPI version.
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


        #region EMSP_GetVersion_JSON_NoToken_Test()

        /// <summary>
        /// EMSP GetVersion JSON without an access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_JSON_NoToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1   = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL  = URL.Parse(JObject.Parse(response1.HTTPBodyAsUTF8String!)?["data"]?[0]?["url"]?.Value<String>()!);
                var response2   = await TestHelpers.GetJSONRequest(versionURL);

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (200,            response2.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                // {
                //   "data": {
                //     "version":   "2.1.1",
                //     "endpoints": [
                //       {
                //         "identifier":  "credentials",
                //         "url":         "http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/credentials"
                //       },
                //       {
                //         "identifier":  "locations",
                //         "url":         "http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/cpo/locations"
                //       }
                //     ]
                //   },
                //   "status_code":      1000,
                //   "status_message":  "Hello world!",
                //   "timestamp":       "2023-04-30T01:18:34.24Z"
                // }

                var json        = response2.Content;
                ClassicAssert.IsNotNull(json);

                ClassicAssert.AreEqual ("2.1.1",                                                  json["data"]?["version"]?.Value<String>());

                ClassicAssert.AreEqual (2,                                                        json["data"]?["endpoints"]?.Count());

                ClassicAssert.AreEqual ("credentials",                                            json["data"]?["endpoints"]?[0]?["identifier"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/credentials",     json["data"]?["endpoints"]?[0]?["url"]?.       Value<String>());

                ClassicAssert.AreEqual ("locations",                                              json["data"]?["endpoints"]?[1]?["identifier"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/cpo/locations",   json["data"]?["endpoints"]?[1]?["url"]?.       Value<String>());

                ClassicAssert.AreEqual (1000,                                                     json["status_code"]?.                          Value<UInt32>());
                ClassicAssert.AreEqual ("Hello world!",                                           json["status_message"]?.                       Value<String>());

                var timestamp   = json["timestamp"]?.Value<DateTime>();
                ClassicAssert.IsNotNull(timestamp);
                ClassicAssert.IsTrue   (Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region EMSP_GetVersion_JSON_UnknownToken_Test()

        /// <summary>
        /// EMSP GetVersion JSON using an unknown access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_JSON_UnknownToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1   = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL  = URL.Parse(JObject.Parse(response1.HTTPBodyAsUTF8String!)?["data"]?[0]?["url"]?.Value<String>()!);
                var response2   = await TestHelpers.GetJSONRequest(versionURL, UnknownToken);

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (200,            response2.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                // {
                //   "data": {
                //     "version":   "2.1.1",
                //     "endpoints": [
                //       {
                //         "identifier":  "credentials",
                //         "url":         "http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/credentials"
                //       },
                //       {
                //         "identifier":  "locations",
                //         "url":         "http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/cpo/locations"
                //       }
                //     ]
                //   },
                //   "status_code":      1000,
                //   "status_message":  "Hello world!",
                //   "timestamp":       "2023-04-30T01:18:34.24Z"
                // }

                var json        = response2.Content;
                ClassicAssert.IsNotNull(json);

                ClassicAssert.AreEqual ("2.1.1",                                                  json["data"]?["version"]?.Value<String>());

                ClassicAssert.AreEqual (2,                                                        json["data"]?["endpoints"]?.Count());

                ClassicAssert.AreEqual ("credentials",                                            json["data"]?["endpoints"]?[0]?["identifier"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/credentials",     json["data"]?["endpoints"]?[0]?["url"]?.       Value<String>());

                ClassicAssert.AreEqual ("locations",                                              json["data"]?["endpoints"]?[1]?["identifier"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/cpo/locations",   json["data"]?["endpoints"]?[1]?["url"]?.       Value<String>());

                ClassicAssert.AreEqual (1000,                                                     json["status_code"]?.                          Value<UInt32>());
                ClassicAssert.AreEqual ("Hello world!",                                           json["status_message"]?.                       Value<String>());

                var timestamp   = json["timestamp"]?.Value<DateTime>();
                ClassicAssert.IsNotNull(timestamp);
                ClassicAssert.IsTrue   (Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region EMSP_GetVersion_JSON_RegisteredToken_Test()

        /// <summary>
        /// EMSP GetVersion JSON using a registered access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_JSON_RegisteredToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1   = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value, "emsp1_accessing_cpo++token");
                var versionURL  = URL.Parse(JObject.Parse(response1.HTTPBodyAsUTF8String!)?["data"]?[0]?["url"]?.Value<String>()!);
                var response2   = await TestHelpers.GetJSONRequest(versionURL,              "emsp1_accessing_cpo++token");

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (200,            response2.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                // {
                //     "data": {
                //         "version": "2.1.1",
                //         "endpoints": [
                //             {
                //                 "identifier":  "credentials",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/credentials"
                //             },
                //             {
                //                 "identifier":  "locations",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/cpo/locations"
                //             },
                //             {
                //                 "identifier":  "tariffs",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/cpo/tariffs"
                //             },
                //             {
                //                 "identifier":  "sessions",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/cpo/sessions"
                //             },
                //             {
                //                 "identifier":  "cdrs",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/cpo/cdrs"
                //             },
                //             {
                //                 "identifier":  "commands",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/cpo/commands"
                //             },
                //             {
                //                 "identifier":  "tokens",
                //                 "url":         "http://localhost:3301/ocpi/v2.1/v2.1.1/cpo/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-30T01:43:15.277Z"
                // }

                var json        = response2.Content;
                ClassicAssert.IsNotNull(json);

                ClassicAssert.AreEqual ("2.1.1",                                                  json["data"]?["version"]?.Value<String>());

                ClassicAssert.AreEqual (7,                                                        json["data"]?["endpoints"]?.Count());

                ClassicAssert.AreEqual ("credentials",                                            json["data"]?["endpoints"]?[0]?["identifier"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/credentials",     json["data"]?["endpoints"]?[0]?["url"]?.       Value<String>());

                ClassicAssert.AreEqual ("locations",                                              json["data"]?["endpoints"]?[1]?["identifier"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/cpo/locations",   json["data"]?["endpoints"]?[1]?["url"]?.       Value<String>());

                ClassicAssert.AreEqual ("tariffs",                                                json["data"]?["endpoints"]?[2]?["identifier"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/cpo/tariffs",     json["data"]?["endpoints"]?[2]?["url"]?.       Value<String>());

                ClassicAssert.AreEqual ("sessions",                                               json["data"]?["endpoints"]?[3]?["identifier"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/cpo/sessions",    json["data"]?["endpoints"]?[3]?["url"]?.       Value<String>());

                ClassicAssert.AreEqual ("cdrs",                                                   json["data"]?["endpoints"]?[4]?["identifier"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/cpo/cdrs",        json["data"]?["endpoints"]?[4]?["url"]?.       Value<String>());

                ClassicAssert.AreEqual ("commands",                                               json["data"]?["endpoints"]?[5]?["identifier"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/cpo/commands",    json["data"]?["endpoints"]?[5]?["url"]?.       Value<String>());

                ClassicAssert.AreEqual ("tokens",                                                 json["data"]?["endpoints"]?[6]?["identifier"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/v2.1.1/cpo/tokens",      json["data"]?["endpoints"]?[6]?["url"]?.       Value<String>());

                ClassicAssert.AreEqual (1000,                                                     json["status_code"]?.                          Value<UInt32>());
                ClassicAssert.AreEqual ("Hello world!",                                           json["status_message"]?.                       Value<String>());

                var timestamp   = json["timestamp"]?.Value<DateTime>();
                ClassicAssert.IsNotNull(timestamp);
                ClassicAssert.IsTrue   (Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region EMSP_GetVersion_JSON_BlockedToken_Test()

        /// <summary>
        /// EMSP GetVersion JSON using a blocked access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_JSON_BlockedToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1   = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL  = URL.Parse(JObject.Parse(response1.HTTPBodyAsUTF8String!)?["data"]?[0]?["url"]?.Value<String>()!);
                var response2   = await TestHelpers.GetJSONRequest(versionURL, BlockedEMSPToken);

                // HTTP/1.1 403 Forbidden
                // Date:                            Sun, 30 Apr 2023 02:17:25 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  111
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "status_code":      2000,
                //     "status_message":  "Invalid or blocked access token!",
                //     "timestamp":       "2023-04-30T02:17:25.970Z"
                // }

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (403,            response2.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                var json        = response2.Content;
                ClassicAssert.IsNotNull(json);

                ClassicAssert.AreEqual (2000,                                json["status_code"]?.   Value<UInt32>());
                ClassicAssert.AreEqual ("Invalid or blocked access token!",  json["status_message"]?.Value<String>());

                var timestamp   = json["timestamp"]?.Value<DateTime>();
                ClassicAssert.IsNotNull(timestamp);
                ClassicAssert.IsTrue   (Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region EMSP_GetVersion_JSON_UnknownVersion_Test()

        /// <summary>
        /// EMSP GetVersion JSON of an unknown OCPI version.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_JSON_UnknownVersion_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1   = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL  = URL.Parse(JObject.Parse(response1.HTTPBodyAsUTF8String!)?["data"]?[0]?["url"]?.Value<String>()?.Replace("2.1.1", "0.7")!);
                var response2   = await TestHelpers.GetJSONRequest(versionURL);

                // GET /ocpi/v2.1/versions/0.7 HTTP/1.1
                // Date:                            Sun, 30 Apr 2023 02:33:26 GMT
                // Accept:                          application/json; charset=utf-8;q=1
                // Host:                            127.0.0.1:3301
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // HTTP/1.1 404 Not Found
                // Date:                            Sun, 30 Apr 2023 02:33:26 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  114
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "status_code":      2000,
                //     "status_message":  "This OCPI version is not supported!",
                //     "timestamp":       "2023-04-30T02:33:26.044Z"
                // }

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (404,            response2.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                var json        = response2.Content;
                ClassicAssert.IsNotNull(json);

                ClassicAssert.AreEqual (2000,                                    json["status_code"]?.   Value<UInt32>());
                ClassicAssert.AreEqual ("This OCPI version is not supported!",   json["status_message"]?.Value<String>());

                var timestamp   = json["timestamp"]?.Value<DateTime>();
                ClassicAssert.IsNotNull(timestamp);
                ClassicAssert.IsTrue   (Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion


        #region EMSP_GetVersion_HTML_NoToken_Test()

        /// <summary>
        /// EMSP GetVersion HTML without an access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_HTML_NoToken_Test()
        {

            ClassicAssert.IsNotNull(cpoWebAPI);

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1   = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL  = URL.Parse(JObject.Parse(response1.HTTPBodyAsUTF8String!)?["data"]?[0]?["url"]?.Value<String>()!);
                var response2   = await TestHelpers.GetHTMLRequest(versionURL);

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (200,            response2.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response2.HTTPBodyAsUTF8String;
                ClassicAssert.IsNotNull(html);
                ClassicAssert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region EMSP_GetVersion_HTML_UnknownToken_Test()

        /// <summary>
        /// EMSP GetVersion HTML with an unknown access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_HTML_UnknownToken_Test()
        {

            ClassicAssert.IsNotNull(cpoWebAPI);

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1   = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL  = URL.Parse(JObject.Parse(response1.HTTPBodyAsUTF8String!)?["data"]?[0]?["url"]?.Value<String>()!);
                var response2   = await TestHelpers.GetHTMLRequest(versionURL, UnknownToken);

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (200,            response2.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response2.HTTPBodyAsUTF8String;
                ClassicAssert.IsNotNull(html);
                ClassicAssert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region EMSP_GetVersion_HTML_RegisteredToken_Test()

        /// <summary>
        /// EMSP GetVersion HTML with a registered access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_HTML_RegisteredToken_Test()
        {

            ClassicAssert.IsNotNull(cpoWebAPI);

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1   = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value, "emsp1_accessing_cpo++token");
                var versionURL  = URL.Parse(JObject.Parse(response1.HTTPBodyAsUTF8String!)?["data"]?[0]?["url"]?.Value<String>()!);
                var response2   = await TestHelpers.GetHTMLRequest(versionURL,              "emsp1_accessing_cpo++token");

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (200,            response2.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response2.HTTPBodyAsUTF8String;
                ClassicAssert.IsNotNull(html);
                ClassicAssert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region EMSP_GetVersion_HTML_BlockedToken_Test()

        /// <summary>
        /// EMSP GetVersion HTML with a blocked access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_HTML_BlockedToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1   = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL  = URL.Parse(JObject.Parse(response1.HTTPBodyAsUTF8String!)?["data"]?[0]?["url"]?.Value<String>()!);
                var response2   = await TestHelpers.GetHTMLRequest(versionURL, BlockedEMSPToken);

                // HTTP/1.1 403 Forbidden
                // Date:                            Sun, 30 Apr 2023 02:17:25 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Vary:                            Accept
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  111
                // X-Request-ID:                    1234
                // X-Correlation-ID:                5678
                // 
                // {
                //     "status_code":      2000,
                //     "status_message":  "Invalid or blocked access token!",
                //     "timestamp":       "2023-04-30T02:17:25.970Z"
                // }

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (200,            response2.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response2.HTTPBodyAsUTF8String;
                ClassicAssert.IsNotNull(html);
                ClassicAssert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region EMSP_GetVersion_HTML_UnknownVersion_Test()

        /// <summary>
        /// EMSP GetVersion HTML of an unknown OCPI version.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_HTML_UnknownVersion_Test()
        {

            ClassicAssert.IsNotNull(cpoWebAPI);

            if (cpoVersionsAPIURL.HasValue)
            {

                var response1   = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);
                var versionURL  = URL.Parse(JObject.Parse(response1.HTTPBodyAsUTF8String!)?["data"]?[0]?["url"]?.Value<String>()?.Replace("2.1.1", "0.7")!);
                var response2   = await TestHelpers.GetHTMLRequest(versionURL);

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (200,            response2.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response2.HTTPBodyAsUTF8String;
                ClassicAssert.IsNotNull(html);
                ClassicAssert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion


    }

}
