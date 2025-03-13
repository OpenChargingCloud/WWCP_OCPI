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

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.CPOTests
{

    [TestFixture]
    public class Versions_Tests : ANodeTests
    {

        #region CPO_GetVersions_RegisteredToken_Test1()

        /// <summary>
        /// CPO GetVersions Test (EMSP 1).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_RegisteredToken_Test1()
        {

            var graphDefinedEMSP1 = cpoCPOAPI?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            ClassicAssert.IsNotNull(graphDefinedEMSP1);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersions();

                // GET /ocpi/v2.1/versions HTTP/1.1
                // Date:                            Sun, 30 Apr 2023 01:29:27 GMT
                // Accept:                          application/json; charset=utf-8;q=1
                // Host:                            localhost:3401
                // Authorization:                   Token cpo_accessing_emsp1++token
                // User-Agent:                      GraphDefined OCPI HTTP Client v1.0
                // X-Request-ID:                    bjxW4W594CMfSnWCz5v74Q24UKtp92
                // X-Correlation-ID:                SM5WpMtE22h3d2p8vSz9hGSC4W3n12

                // HTTP/1.1 200 OK
                // Date:                            Sun, 30 Apr 2023 01:29:27 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Vary:                            Accept
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  175
                // X-Request-ID:                    bjxW4W594CMfSnWCz5v74Q24UKtp92
                // X-Correlation-ID:                SM5WpMtE22h3d2p8vSz9hGSC4W3n12
                // 
                // {
                //     "data": [{
                //         "version":  "2.1.1",
                //         "url":      "http://localhost:3401/ocpi/v2.1/versions/2.1.1"
                //     }],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-30T01:29:27.228Z"
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
                ClassicAssert.IsTrue   (URL.Parse("http://localhost:3401/ocpi/v2.1/versions/2.1.1") == version?.URL);

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region CPO_GetVersions_RegisteredToken_Test2()

        /// <summary>
        /// CPO GetVersions Test (EMSP 2).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_RegisteredToken_Test2()
        {

            var graphDefinedEMSP2 = cpoCPOAPI?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GD2")
                                    );

            ClassicAssert.IsNotNull(graphDefinedEMSP2);

            if (graphDefinedEMSP2 is not null)
            {

                var response = await graphDefinedEMSP2.GetVersions();

                // GET /ocpi/v2.1/versions HTTP/1.1
                // Date:                            Sun, 30 Apr 2023 01:35:47 GMT
                // Accept:                          application/json; charset=utf-8;q=1
                // Host:                            localhost:3402
                // Authorization:                   Token cpo_accessing_emsp2++token
                // User-Agent:                      GraphDefined OCPI HTTP Client v1.0
                // X-Request-ID:                    Yx9xWSSG45bdE6jMAj9pWA6ES2jnr2
                // X-Correlation-ID:                52f41UGdWn681QU9rEtK69Ef9bhE9p

                // HTTP/1.1 200 OK
                // Date:                            Sun, 30 Apr 2023 01:35:47 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Vary:                            Accept
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  175
                // X-Request-ID:                    Yx9xWSSG45bdE6jMAj9pWA6ES2jnr2
                // X-Correlation-ID:                52f41UGdWn681QU9rEtK69Ef9bhE9p
                // 
                // {
                //     "data": [{
                //         "version":  "2.1.1",
                //         "url":      "http://localhost:3402/ocpi/v2.1/versions/2.1.1"
                //     }],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-30T01:35:47.800Z"
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
                ClassicAssert.IsTrue   (URL.Parse("http://localhost:3402/ocpi/v2.1/versions/2.1.1") == version?.URL);

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region CPO_GetVersions_UnknownToken_Test1()

        /// <summary>
        /// CPO GetVersions using an unknown access token (EMSP 1).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_UnknownToken_Test1()
        {

            if (cpoCommonAPI is not null &&
                emsp1VersionsAPIURL.HasValue)
            {

                #region Change Access Token

                await cpoCommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"),
                                                     Party_Id.   Parse("GDF"),
                                                     Role.       EMSP);

                var result = await cpoCommonAPI.AddRemoteParty(
                    CountryCode:         CountryCode.Parse("DE"),
                    PartyId:             Party_Id.   Parse("GDF"),
                    Role:                Role.       EMSP,
                    BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services"),
                    LocalAccessInfos:    new[] {
                                             new LocalAccessInfo(
                                                 AccessToken.Parse(UnknownToken),
                                                 AccessStatus.ALLOWED
                                             )
                                         },
                    RemoteAccessInfos:   new[] {
                                             new RemoteAccessInfo(
                                                 AccessToken:        AccessToken.Parse(UnknownToken),
                                                 VersionsURL:        emsp1VersionsAPIURL.Value,
                                                 VersionIds:         new Version_Id[] {
                                                                         Version_Id.Parse("2.1.1")
                                                                     },
                                                 SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                                 Status:             RemoteAccessStatus.ONLINE
                                             )
                                         },
                    Status:              PartyStatus.ENABLED
                );

                #endregion

                var graphDefinedEMSP1 = cpoCPOAPI?.GetEMSPClient(
                                            CountryCode: CountryCode.Parse("DE"),
                                            PartyId:     Party_Id.   Parse("GDF")
                                        );

                ClassicAssert.IsNotNull(graphDefinedEMSP1);

                if (graphDefinedEMSP1 is not null)
                {

                    var response = await graphDefinedEMSP1.GetVersions();

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
                    // X-Request-ID:                  nM7djM37h56hQz8t8hKMznnhGYj3CK
                    // X-Correlation-ID:              53YKxAnt2zM9bGp2AvjK6t83txbCK3
                    // 
                    // {
                    //     "data": [{
                    //         "version":  "2.1.1",
                    //         "url":      "http://127.0.0.1:7135/versions/2.1.1"
                    //     }],
                    //     "status_code":      1000,
                    //     "status_message":  "Hello world!",
                    //     "timestamp":       "2022-12-25T23:16:31.228Z"
                    // }

                    ClassicAssert.IsNotNull(response);
                    ClassicAssert.AreEqual (200,             response.HTTPResponse?.HTTPStatusCode.Code);
                    ClassicAssert.AreEqual (1000,            response.StatusCode);
                    ClassicAssert.AreEqual ("Hello world!",  response.StatusMessage);
                    ClassicAssert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

                    var versions = response.Data;
                    ClassicAssert.IsNotNull(versions);
                    ClassicAssert.AreEqual (1, response.Data?.Count());

                    var version = versions?.First();
                    ClassicAssert.IsNotNull(version);
                    ClassicAssert.AreEqual (Version_Id.Parse("2.1.1"),                         version?.Id);
                    ClassicAssert.AreEqual ("http://127.0.0.1:3401/ocpi/v2.1/versions/2.1.1",  version?.URL.ToString());

                    //ClassicAssert.IsNotNull(response.Request);

                }

            }

        }

        #endregion

        #region CPO_GetVersions_BlockedToken_Test1()

        /// <summary>
        /// CPO GetVersions using a blocked access token (EMSP 1).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_BlockedToken_Test1()
        {

            if (cpoCommonAPI   is not null &&
                emsp1CommonAPI is not null &&
                emsp1VersionsAPIURL.HasValue)
            {

                #region Block Access Token

                await cpoCommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"),
                                                     Party_Id.   Parse("GDF"),
                                                     Role.       EMSP);

                var addEMSPResult = await cpoCommonAPI.AddRemoteParty(
                    CountryCode:         CountryCode.Parse("DE"),
                    PartyId:             Party_Id.   Parse("GDF"),
                    Role:                Role.       EMSP,
                    BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services"),
                    LocalAccessInfos:    new[] {
                                             new LocalAccessInfo(
                                                 AccessToken.Parse(BlockedEMSPToken),
                                                 AccessStatus.ALLOWED
                                             )
                                         },
                    RemoteAccessInfos:   new[] {
                                             new RemoteAccessInfo(
                                                 AccessToken:        AccessToken.Parse(BlockedCPOToken),
                                                 VersionsURL:        emsp1VersionsAPIURL.Value,
                                                 VersionIds:         new[] {
                                                                         Version_Id.Parse("2.1.1")
                                                                     },
                                                 SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                                 Status:             RemoteAccessStatus.ONLINE
                                             )
                                         },
                    Status:              PartyStatus.ENABLED
                );

                ClassicAssert.IsTrue(addEMSPResult);

                #endregion

                var graphDefinedEMSP1 = cpoCPOAPI?.GetEMSPClient(
                                            CountryCode: CountryCode.Parse("DE"),
                                            PartyId:     Party_Id.   Parse("GDF")
                                        );

                ClassicAssert.IsNotNull(graphDefinedEMSP1);

                if (graphDefinedEMSP1 is not null)
                {

                    var response = await graphDefinedEMSP1.GetVersions();

                    // HTTP/1.1 403 Forbidden
                    // Date:                          Sun, 25 Dec 2022 22:44:01 GMT
                    // Access-Control-Allow-Methods:  OPTIONS, GET
                    // Access-Control-Allow-Headers:  Authorization
                    // Server:                        GraphDefined Hermod HTTP Server v1.0
                    // Access-Control-Allow-Origin:   *
                    // Connection:                    close
                    // Content-Type:                  application/json; charset=utf-8
                    // Content-Length:                111
                    // X-Request-ID:                  KtUbA39Ad22pQ2Qt1MEShtEUrpfS14
                    // X-Correlation-ID:              1ppv79Yj5QU69E2j9vG4z6GSb46r8z
                    // 
                    // {
                    //     "status_code":     2000,
                    //     "status_message": "Invalid or blocked access token!",
                    //     "timestamp":      "2022-12-25T22:44:01.747Z"
                    // }

                    ClassicAssert.IsNotNull(response);
                    ClassicAssert.AreEqual (403,                                 response.HTTPResponse?.HTTPStatusCode.Code);
                    ClassicAssert.AreEqual (2000,                                response.StatusCode);
                    ClassicAssert.AreEqual ("Invalid or blocked access token!",  response.StatusMessage);
                    ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                    var versions = response.Data;
                    ClassicAssert.IsNotNull(versions);
                    ClassicAssert.AreEqual (0, versions?.Count());

                    //ClassicAssert.IsNotNull(response.Request);

                }

            }

        }

        #endregion


        #region CPO_GetVersions_JSON_NoToken_Test1()

        /// <summary>
        /// CPO GetVersions JSON without an access token (EMSP 1).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_JSON_NoToken_Test1()
        {

            if (emsp1VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetJSONRequest(emsp1VersionsAPIURL.Value);

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                // {
                //   "data": [
                //     {
                //       "version":  "2.1.1",
                //       "url":      "http://127.0.0.1:3401/ocpi/v2.1/versions/2.1.1"
                //     }
                //   ],
                //   "status_code":      1000,
                //   "status_message":  "Hello world!",
                //   "timestamp":       "2023-04-30T01:01:47.224Z"
                // }

                var json       = response.Content;
                ClassicAssert.IsNotNull(json);

                ClassicAssert.AreEqual ("2.1.1",                                            json["data"]?[0]?["version"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3401/ocpi/v2.1/versions/2.1.1",   json["data"]?[0]?["url"]?.    Value<String>());
                ClassicAssert.AreEqual (1000,                                               json["status_code"]?.         Value<UInt32>());
                ClassicAssert.AreEqual ("Hello world!",                                     json["status_message"]?.      Value<String>());

                var timestamp  = json["timestamp"]?.Value<DateTime>();
                ClassicAssert.IsNotNull(timestamp);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region CPO_GetVersions_JSON_NoToken_Test2()

        /// <summary>
        /// CPO GetVersions JSON without an access token (EMSP 2).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_JSON_NoToken_Test2()
        {

            if (emsp2VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetJSONRequest(emsp2VersionsAPIURL.Value);

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                // {
                //   "data": [
                //     {
                //       "version":  "2.1.1",
                //       "url":      "http://127.0.0.1:3402/ocpi/v2.1/versions/2.1.1"
                //     }
                //   ],
                //   "status_code":      1000,
                //   "status_message":  "Hello world!",
                //   "timestamp":       "2023-04-30T01:01:47.224Z"
                // }

                var json       = response.Content;
                ClassicAssert.IsNotNull(json);

                ClassicAssert.AreEqual ("2.1.1",                                            json["data"]?[0]?["version"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3402/ocpi/v2.1/versions/2.1.1",   json["data"]?[0]?["url"]?.    Value<String>());
                ClassicAssert.AreEqual (1000,                                               json["status_code"]?.         Value<UInt32>());
                ClassicAssert.AreEqual ("Hello world!",                                     json["status_message"]?.      Value<String>());

                var timestamp  = json["timestamp"]?.Value<DateTime>();
                ClassicAssert.IsNotNull(timestamp);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region CPO_GetVersions_JSON_UnknownToken_Test1()

        /// <summary>
        /// CPO GetVersions JSON using an unknown access token (EMSP 1).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_JSON_UnknownToken_Test1()
        {

            if (emsp1VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetJSONRequest(emsp1VersionsAPIURL.Value, UnknownToken);

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                // {
                //   "data": [
                //     {
                //       "version":  "2.1.1",
                //       "url":      "http://127.0.0.1:3401/ocpi/v2.1/versions/2.1.1"
                //     }
                //   ],
                //   "status_code":      1000,
                //   "status_message":  "Hello world!",
                //   "timestamp":       "2023-04-30T01:01:47.224Z"
                // }

                var json       = response.Content;
                ClassicAssert.IsNotNull(json);

                ClassicAssert.AreEqual ("2.1.1",                                            json["data"]?[0]?["version"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3401/ocpi/v2.1/versions/2.1.1",   json["data"]?[0]?["url"]?.    Value<String>());
                ClassicAssert.AreEqual (1000,                                               json["status_code"]?.         Value<UInt32>());
                ClassicAssert.AreEqual ("Hello world!",                                     json["status_message"]?.      Value<String>());

                var timestamp  = json["timestamp"]?.Value<DateTime>();
                ClassicAssert.IsNotNull(timestamp);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region CPO_GetVersions_JSON_UnknownToken_Test2()

        /// <summary>
        /// CPO GetVersions JSON using an unknown access token (EMSP 2).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_JSON_UnknownToken_Test2()
        {

            if (emsp2VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetJSONRequest(emsp2VersionsAPIURL.Value, UnknownToken);

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                // {
                //   "data": [
                //     {
                //       "version":  "2.1.1",
                //       "url":      "http://127.0.0.1:3402/ocpi/v2.1/versions/2.1.1"
                //     }
                //   ],
                //   "status_code":      1000,
                //   "status_message":  "Hello world!",
                //   "timestamp":       "2023-04-30T01:01:47.224Z"
                // }

                var json       = response.Content;
                ClassicAssert.IsNotNull(json);

                ClassicAssert.AreEqual ("2.1.1",                                            json["data"]?[0]?["version"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3402/ocpi/v2.1/versions/2.1.1",   json["data"]?[0]?["url"]?.    Value<String>());
                ClassicAssert.AreEqual (1000,                                               json["status_code"]?.         Value<UInt32>());
                ClassicAssert.AreEqual ("Hello world!",                                     json["status_message"]?.      Value<String>());

                var timestamp  = json["timestamp"]?.Value<DateTime>();
                ClassicAssert.IsNotNull(timestamp);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region CPO_GetVersions_JSON_RegisteredToken_Test1()

        /// <summary>
        /// CPO GetVersions JSON using a registered access token (EMSP 1).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_JSON_RegisteredToken_Test1()
        {

            if (emsp1VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetJSONRequest(emsp1VersionsAPIURL.Value, "cpo_accessing_emsp1++token");

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                // {
                //   "data": [
                //     {
                //       "version":  "2.1.1",
                //       "url":      "http://127.0.0.1:3401/ocpi/v2.1/versions/2.1.1"
                //     }
                //   ],
                //   "status_code":      1000,
                //   "status_message":  "Hello world!",
                //   "timestamp":       "2023-04-30T01:01:47.224Z"
                // }

                var json       = response.Content;
                ClassicAssert.IsNotNull(json);

                ClassicAssert.AreEqual ("2.1.1",                                            json["data"]?[0]?["version"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3401/ocpi/v2.1/versions/2.1.1",   json["data"]?[0]?["url"]?.    Value<String>());
                ClassicAssert.AreEqual (1000,                                               json["status_code"]?.         Value<UInt32>());
                ClassicAssert.AreEqual ("Hello world!",                                     json["status_message"]?.      Value<String>());

                var timestamp  = json["timestamp"]?.Value<DateTime>();
                ClassicAssert.IsNotNull(timestamp);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region CPO_GetVersions_JSON_RegisteredToken_Test2()

        /// <summary>
        /// CPO GetVersions JSON using a registered access token (EMSP 2).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_JSON_RegisteredToken_Test2()
        {

            if (emsp2VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetJSONRequest(emsp2VersionsAPIURL.Value, "cpo_accessing_emsp2++token");

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                // {
                //   "data": [
                //     {
                //       "version":  "2.1.1",
                //       "url":      "http://127.0.0.1:3402/ocpi/v2.1/versions/2.1.1"
                //     }
                //   ],
                //   "status_code":      1000,
                //   "status_message":  "Hello world!",
                //   "timestamp":       "2023-04-30T01:01:47.224Z"
                // }

                var json       = response.Content;
                ClassicAssert.IsNotNull(json);

                ClassicAssert.AreEqual ("2.1.1",                                            json["data"]?[0]?["version"]?.Value<String>());
                ClassicAssert.AreEqual ("http://127.0.0.1:3402/ocpi/v2.1/versions/2.1.1",   json["data"]?[0]?["url"]?.    Value<String>());
                ClassicAssert.AreEqual (1000,                                               json["status_code"]?.         Value<UInt32>());
                ClassicAssert.AreEqual ("Hello world!",                                     json["status_message"]?.      Value<String>());

                var timestamp  = json["timestamp"]?.Value<DateTime>();
                ClassicAssert.IsNotNull(timestamp);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region CPO_GetVersions_JSON_BlockedToken_Test1()

        /// <summary>
        /// CPO GetVersions JSON using a blocked access token (EMSP 1).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_JSON_BlockedToken_Test1()
        {

            if (emsp1VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetJSONRequest(emsp1VersionsAPIURL.Value, BlockedCPOToken);

                // HTTP/1.1 403 Forbidden
                // Date:                            Sun, 30 Apr 2023 03:02:50 GMT
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
                //     "timestamp":       "2023-04-30T03:02:50.567Z"
                // }

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (403,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var json       = response.Content;
                ClassicAssert.IsNotNull(json);

                ClassicAssert.AreEqual (2000,                                json["status_code"]?.   Value<UInt32>());
                ClassicAssert.AreEqual ("Invalid or blocked access token!",  json["status_message"]?.Value<String>());

                var timestamp  = json["timestamp"]?.Value<DateTime>();
                ClassicAssert.IsNotNull(timestamp);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region CPO_GetVersions_JSON_BlockedToken_Test2()

        /// <summary>
        /// CPO GetVersions JSON using a blocked access token (EMSP 2).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_JSON_BlockedToken_Test2()
        {

            if (emsp2VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetJSONRequest(emsp2VersionsAPIURL.Value, BlockedCPOToken);

                // HTTP/1.1 403 Forbidden
                // Date:                            Sun, 30 Apr 2023 03:02:50 GMT
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
                //     "timestamp":       "2023-04-30T03:02:50.567Z"
                // }

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (403,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var json       = response.Content;
                ClassicAssert.IsNotNull(json);

                ClassicAssert.AreEqual (2000,                                json["status_code"]?.   Value<UInt32>());
                ClassicAssert.AreEqual ("Invalid or blocked access token!",  json["status_message"]?.Value<String>());

                var timestamp  = json["timestamp"]?.Value<DateTime>();
                ClassicAssert.IsNotNull(timestamp);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion


        #region CPO_GetVersions_HTML_NoToken_Test1()

        /// <summary>
        /// CPO GetVersions HTML without an access token (EMSP 1).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_HTML_NoToken_Test1()
        {

            ClassicAssert.IsNotNull(emsp1WebAPI);

            if (emsp1VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetHTMLRequest(emsp1VersionsAPIURL.Value);

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response.HTTPBodyAsUTF8String;
                ClassicAssert.IsNotNull(html);
                ClassicAssert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region CPO_GetVersions_HTML_NoToken_Test2()

        /// <summary>
        /// CPO GetVersions HTML without an access token (EMSP 2).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_HTML_NoToken_Test2()
        {

            ClassicAssert.IsNotNull(emsp2WebAPI);

            if (emsp2VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetHTMLRequest(emsp2VersionsAPIURL.Value);

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response.HTTPBodyAsUTF8String;
                ClassicAssert.IsNotNull(html);
                ClassicAssert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region CPO_GetVersions_HTML_UnknownToken_Test1()

        /// <summary>
        /// CPO GetVersions HTML using an unknown access token (EMSP 1).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_HTML_UnknownToken_Test1()
        {

            ClassicAssert.IsNotNull(emsp1WebAPI);

            if (emsp1VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetHTMLRequest(emsp1VersionsAPIURL.Value, UnknownToken);

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response.HTTPBodyAsUTF8String;
                ClassicAssert.IsNotNull(html);
                ClassicAssert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region CPO_GetVersions_HTML_UnknownToken_Test2()

        /// <summary>
        /// CPO GetVersions HTML using an unknown access token (EMSP 2).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_HTML_UnknownToken_Test2()
        {

            ClassicAssert.IsNotNull(emsp2WebAPI);

            if (emsp2VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetHTMLRequest(emsp2VersionsAPIURL.Value, UnknownToken);

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response.HTTPBodyAsUTF8String;
                ClassicAssert.IsNotNull(html);
                ClassicAssert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region CPO_GetVersions_HTML_RegisteredToken_Test1()

        /// <summary>
        /// CPO GetVersions HTML using a registered access token (EMSP 1).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_HTML_RegisteredToken_Test1()
        {

            ClassicAssert.IsNotNull(emsp1WebAPI);

            if (emsp1VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetHTMLRequest(emsp1VersionsAPIURL.Value, "cpo_accessing_emsp1++token");

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response.HTTPBodyAsUTF8String;
                ClassicAssert.IsNotNull(html);
                ClassicAssert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region CPO_GetVersions_HTML_RegisteredToken_Test2()

        /// <summary>
        /// CPO GetVersions HTML using a registered access token (EMSP 2).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_HTML_RegisteredToken_Test2()
        {

            ClassicAssert.IsNotNull(emsp2WebAPI);

            if (emsp2VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetHTMLRequest(emsp2VersionsAPIURL.Value, "cpo_accessing_emsp2++token");

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response.HTTPBodyAsUTF8String;
                ClassicAssert.IsNotNull(html);
                ClassicAssert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region CPO_GetVersions_HTML_BlockedToken_Test1()

        /// <summary>
        /// CPO GetVersions HTML using a blocked access token (EMSP 1).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_HTML_BlockedToken_Test1()
        {

            ClassicAssert.IsNotNull(emsp1WebAPI);

            if (emsp1VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetHTMLRequest(emsp1VersionsAPIURL.Value, BlockedCPOToken);

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response.HTTPBodyAsUTF8String;
                ClassicAssert.IsNotNull(html);
                ClassicAssert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region CPO_GetVersions_HTML_BlockedToken_Test2()

        /// <summary>
        /// CPO GetVersions HTML using a blocked access token (EMSP 2).
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_HTML_BlockedToken_Test2()
        {

            ClassicAssert.IsNotNull(emsp2WebAPI);

            if (emsp2VersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetHTMLRequest(emsp2VersionsAPIURL.Value, BlockedCPOToken);

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPStatusCode.Code);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response.HTTPBodyAsUTF8String;
                ClassicAssert.IsNotNull(html);
                ClassicAssert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion


    }

}
