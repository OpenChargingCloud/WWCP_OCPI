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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.EMSPTests
{

    [TestFixture]
    public class Versions_Tests : ANodeTests
    {

        #region EMSP_GetVersions_RegisteredToken_Test1()

        /// <summary>
        /// EMSP GetVersions using a registered access token (EMSP 1).
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_RegisteredToken_Test1()
        {

            var graphDefinedCPO = emsp1CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetVersions();

                // GET /ocpi/v2.1/versions HTTP/1.1
                // Date:                            Sun, 30 Apr 2023 01:38:42 GMT
                // Accept:                          application/json; charset=utf-8;q=1
                // Host:                            localhost:3301
                // Authorization:                   Token emsp1_accessing_cpo::token
                // User-Agent:                      GraphDefined OCPI HTTP Client v1.0
                // X-Request-ID:                    43EKp122t15Ad3hh1vxEj4Qvtht1hM
                // X-Correlation-ID:                f1Qr44hnzYd2tWAKrjdjhU15CvW943

                // HTTP/1.1 200 OK
                // Date:                            Sun, 30 Apr 2023 01:38:42 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Vary:                            Accept
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Connection:                      close
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  175
                // X-Request-ID:                    43EKp122t15Ad3hh1vxEj4Qvtht1hM
                // X-Correlation-ID:                f1Qr44hnzYd2tWAKrjdjhU15CvW943
                // 
                // {
                //     "data": [{
                //         "version":  "2.1.1",
                //         "url":      "http://localhost:3301/ocpi/v2.1/versions/2.1.1"
                //     }],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-30T01:38:42.483Z"
                // }

                Assert.IsNotNull(response);
                Assert.AreEqual (200,            response.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,           response.StatusCode);
                Assert.AreEqual ("Hello world!", response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var versions = response.Data;
                Assert.IsNotNull(versions);
                Assert.AreEqual (1, response.Data?.Count());

                var version  = versions?.First();
                Assert.IsTrue   (version?.Id == Version.Id);
                Assert.IsTrue   (URL.Parse("http://localhost:3301/ocpi/v2.1/versions/2.1.1") == version?.URL);

                //Assert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region EMSP_GetVersions_UnknownToken_Test1()

        /// <summary>
        /// EMSP GetVersions using an unknown access token (EMSP 1).
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_UnknownToken_Test1()
        {

            if (cpoCommonAPI   is not null &&
                emsp1CommonAPI is not null &&
                cpoVersionsAPIURL.HasValue)
            {

                #region Change Access Token

                var removeResult    = emsp1CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"),
                                                                       Party_Id.   Parse("GEF"));

                Assert.IsTrue(removeResult);

                var updateCPOResult = emsp1CommonAPI.AddRemoteParty(
                    CountryCode:         CountryCode.Parse("DE"),
                    PartyId:             Party_Id.   Parse("GEF"),
                    Role:                Roles.      CPO,
                    BusinessDetails:     new BusinessDetails("GraphDefined CPO Services"),
                    LocalAccessInfos:    new[] {
                                             new LocalAccessInfo(
                                                 AccessToken.Parse(UnknownToken),
                                                 AccessStatus.ALLOWED,
                                                 CountryCode.Parse("DE"),
                                                 Party_Id.   Parse("GEF"),
                                                 Roles.      CPO
                                             )
                                         },
                    RemoteAccessInfos:   new[] {
                                             new RemoteAccessInfo(
                                                 AccessToken:        AccessToken.Parse(UnknownToken),
                                                 VersionsURL:        cpoVersionsAPIURL.Value,
                                                 VersionIds:         new[] {
                                                                         Version_Id.Parse("2.1.1")
                                                                     },
                                                 SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                                 Status:             RemoteAccessStatus.ONLINE
                                             )
                                         },
                    Status:              PartyStatus.ENABLED
                );

                Assert.IsTrue(updateCPOResult);

                #endregion

                var graphDefinedCPO = emsp1CommonAPI?.GetEMSPClient(
                                          CountryCode: CountryCode.Parse("DE"),
                                          PartyId:     Party_Id.   Parse("GEF")
                                      );

                Assert.IsNotNull(graphDefinedCPO);

                if (graphDefinedCPO is not null)
                {

                    var response = await graphDefinedCPO.GetVersions();

                    // GET /ocpi/v2.1/versions HTTP/1.1
                    // Date:                            Sun, 30 Apr 2023 01:38:42 GMT
                    // Accept:                          application/json; charset=utf-8;q=1
                    // Host:                            127.0.0.1:3301
                    // Authorization:                   Token UnknownUnknownUnknownToken
                    // User-Agent:                      GraphDefined OCPI HTTP Client v1.0
                    // X-Request-ID:                    43EKp122t15Ad3hh1vxEj4Qvtht1hM
                    // X-Correlation-ID:                f1Qr44hnzYd2tWAKrjdjhU15CvW943

                    // HTTP/1.1 200 OK
                    // Date:                            Sun, 30 Apr 2023 01:38:42 GMT
                    // Access-Control-Allow-Methods:    OPTIONS, GET
                    // Access-Control-Allow-Headers:    Authorization
                    // Vary:                            Accept
                    // Server:                          GraphDefined HTTP API
                    // Access-Control-Allow-Origin:     *
                    // Connection:                      close
                    // Content-Type:                    application/json; charset=utf-8
                    // Content-Length:                  175
                    // X-Request-ID:                    43EKp122t15Ad3hh1vxEj4Qvtht1hM
                    // X-Correlation-ID:                f1Qr44hnzYd2tWAKrjdjhU15CvW943
                    // 
                    // {
                    //     "data": [{
                    //         "version":  "2.1.1",
                    //         "url":      "http://127.0.0.1:3301/ocpi/v2.1/versions/2.1.1"
                    //     }],
                    //     "status_code":      1000,
                    //     "status_message":  "Hello world!",
                    //     "timestamp":       "2023-04-30T01:38:42.483Z"
                    // }

                    Assert.IsNotNull(response);
                    Assert.AreEqual (200,            response.HTTPResponse?.HTTPStatusCode.Code);
                    Assert.AreEqual (1000,           response.StatusCode);
                    Assert.AreEqual ("Hello world!", response.StatusMessage);
                    Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                    var versions = response.Data;
                    Assert.IsNotNull(versions);
                    Assert.AreEqual (1, response.Data?.Count());

                    var version  = versions?.First();
                    Assert.IsTrue   (version?.Id == Version.Id);
                    Assert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/versions/2.1.1", version?.URL.ToString());

                    //Assert.IsNotNull(response.Request);

                }

            }

        }

        #endregion

        #region EMSP_GetVersions_BlockedToken_Test1()

        /// <summary>
        /// EMSP GetVersion using a blocked access token (EMSP 1).
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_BlockedToken_Test1()
        {

            if (cpoCommonAPI   is not null &&
                emsp1CommonAPI is not null &&
                emsp1VersionsAPIURL.HasValue)
            {

                #region Block Access Token

                emsp1CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));

                var addCPOResult = emsp1CommonAPI.AddRemoteParty(
                    CountryCode:         CountryCode.Parse("DE"),
                    PartyId:             Party_Id.   Parse("GEF"),
                    Role:                Roles.      CPO,
                    BusinessDetails:     new BusinessDetails("GraphDefined CPO Services"),
                    LocalAccessInfos:    new[] {
                                             new LocalAccessInfo(
                                                 AccessToken.Parse(BlockedEMSPToken),
                                                 AccessStatus.BLOCKED,
                                                 CountryCode.Parse("DE"),
                                                 Party_Id.   Parse("GEF"),
                                                 Roles.      CPO
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

                Assert.IsTrue(addCPOResult);

                #endregion

                var graphDefinedCPO = emsp1CommonAPI?.GetEMSPClient(
                                          CountryCode: CountryCode.Parse("DE"),
                                          PartyId:     Party_Id.   Parse("GEF")
                                      );

                Assert.IsNotNull(graphDefinedCPO);

                if (graphDefinedCPO is not null)
                {

                    var response = await graphDefinedCPO.GetVersions();

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

                    Assert.IsNotNull(response);
                    Assert.AreEqual (403,                                 response.HTTPResponse?.HTTPStatusCode.Code);
                    Assert.AreEqual (2000,                                response.StatusCode);
                    Assert.AreEqual ("Invalid or blocked access token!",  response.StatusMessage);
                    Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                    var versions = response.Data;
                    Assert.IsNotNull(versions);
                    Assert.AreEqual (0, versions?.Count());

                    //Assert.IsNotNull(response.Request);

                }

            }

        }

        #endregion


        #region EMSP_GetVersions_JSON_NoToken_Test()

        /// <summary>
        /// EMSP GetVersions JSON without an access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_JSON_NoToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value);

                Assert.IsNotNull(response);
                Assert.AreEqual (200,            response.HTTPStatusCode.Code);
                Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                // {
                //   "data": [
                //     {
                //       "version":  "2.1.1",
                //       "url":      "http://127.0.0.1:3301/ocpi/v2.1/versions/2.1.1"
                //     }
                //   ],
                //   "status_code":      1000,
                //   "status_message":  "Hello world!",
                //   "timestamp":       "2023-04-30T01:01:47.224Z"
                // }

                var json       = response.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual ("2.1.1",                                            json["data"]?[0]?["version"]?.Value<String>());
                Assert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/versions/2.1.1",   json["data"]?[0]?["url"]?.    Value<String>());
                Assert.AreEqual (1000,                                               json["status_code"]?.         Value<UInt32>());
                Assert.AreEqual ("Hello world!",                                     json["status_message"]?.      Value<String>());

                var timestamp  = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue   (Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region EMSP_GetVersions_JSON_UnknownToken_Test()

        /// <summary>
        /// EMSP GetVersion JSON using an unknown access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_JSON_UnknownToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value, UnknownToken);

                // {
                //   "data": [
                //     {
                //       "version":  "2.1.1",
                //       "url":      "http://127.0.0.1:3301/ocpi/v2.1/versions/2.1.1"
                //     }
                //   ],
                //   "status_code":      1000,
                //   "status_message":  "Hello world!",
                //   "timestamp":       "2023-04-30T01:01:47.224Z"
                // }

                var json       = response.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual ("2.1.1",                                            json["data"]?[0]?["version"]?.Value<String>());
                Assert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/versions/2.1.1",   json["data"]?[0]?["url"]?.    Value<String>());
                Assert.AreEqual (1000,                                               json["status_code"]?.         Value<UInt32>());
                Assert.AreEqual ("Hello world!",                                     json["status_message"]?.      Value<String>());

                var timestamp  = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue   (Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region EMSP_GetVersions_JSON_RegisteredToken_Test()

        /// <summary>
        /// EMSP GetVersion JSON using a registered access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_JSON_RegisteredToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value, "emsp1_accessing_cpo::token");

                // {
                //   "data": [
                //     {
                //       "version":  "2.1.1",
                //       "url":      "http://127.0.0.1:3301/ocpi/v2.1/versions/2.1.1"
                //     }
                //   ],
                //   "status_code":      1000,
                //   "status_message":  "Hello world!",
                //   "timestamp":       "2023-04-30T01:01:47.224Z"
                // }

                var json       = response.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual ("2.1.1",                                            json["data"]?[0]?["version"]?.Value<String>());
                Assert.AreEqual ("http://127.0.0.1:3301/ocpi/v2.1/versions/2.1.1",   json["data"]?[0]?["url"]?.    Value<String>());
                Assert.AreEqual (1000,                                               json["status_code"]?.         Value<UInt32>());
                Assert.AreEqual ("Hello world!",                                     json["status_message"]?.      Value<String>());

                var timestamp  = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue   (Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion

        #region EMSP_GetVersions_JSON_BlockedToken_Test()

        /// <summary>
        /// EMSP GetVersion JSON using a blocked access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_JSON_BlockedToken_Test()
        {

            if (cpoVersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetJSONRequest(cpoVersionsAPIURL.Value, BlockedEMSPToken);

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

                Assert.IsNotNull(response);
                Assert.AreEqual (403,            response.HTTPStatusCode.Code);
                Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var json       = response.Content;
                Assert.IsNotNull(json);

                Assert.AreEqual (2000,                                json["status_code"]?.   Value<UInt32>());
                Assert.AreEqual ("Invalid or blocked access token!",  json["status_message"]?.Value<String>());

                var timestamp  = json["timestamp"]?.Value<DateTime>();
                Assert.IsNotNull(timestamp);
                Assert.IsTrue   (Timestamp.Now - timestamp < TimeSpan.FromSeconds(10));

            }

        }

        #endregion


        #region EMSP_GetVersions_HTML_NoToken_Test()

        /// <summary>
        /// EMSP GetVersions HTML without an access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_HTML_NoToken_Test()
        {

            Assert.IsNotNull(cpoWebAPI);

            if (cpoVersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetHTMLRequest(cpoVersionsAPIURL.Value);

                Assert.IsNotNull(response);
                Assert.AreEqual (200,            response.HTTPStatusCode.Code);
                Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response.HTTPBodyAsUTF8String;
                Assert.IsNotNull(html);
                Assert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region EMSP_GetVersions_HTML_UnknownToken_Test()

        /// <summary>
        /// EMSP GetVersions HTML using an unknown access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_HTML_UnknownToken_Test()
        {

            Assert.IsNotNull(cpoWebAPI);

            if (cpoVersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetHTMLRequest(cpoVersionsAPIURL.Value, UnknownToken);

                Assert.IsNotNull(response);
                Assert.AreEqual (200,            response.HTTPStatusCode.Code);
                Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response.HTTPBodyAsUTF8String;
                Assert.IsNotNull(html);
                Assert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region EMSP_GetVersions_HTML_RegisteredToken_Test()

        /// <summary>
        /// EMSP GetVersions HTML using a registered access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_HTML_RegisteredToken_Test()
        {

            Assert.IsNotNull(cpoWebAPI);

            if (cpoVersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetHTMLRequest(cpoVersionsAPIURL.Value, "emsp1_accessing_cpo::token");

                Assert.IsNotNull(response);
                Assert.AreEqual (200,            response.HTTPStatusCode.Code);
                Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response.HTTPBodyAsUTF8String;
                Assert.IsNotNull(html);
                Assert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion

        #region EMSP_GetVersions_HTML_BlockedToken_Test()

        /// <summary>
        /// EMSP GetVersions HTML using a blocked access token.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_HTML_BlockedToken_Test()
        {

            Assert.IsNotNull(cpoWebAPI);

            if (cpoVersionsAPIURL.HasValue)
            {

                var response  = await TestHelpers.GetHTMLRequest(cpoVersionsAPIURL.Value, BlockedEMSPToken);

                Assert.IsNotNull(response);
                Assert.AreEqual (200,            response.HTTPStatusCode.Code);
                Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                var html      = response.HTTPBodyAsUTF8String;
                Assert.IsNotNull(html);
                Assert.IsTrue   (html?.Length > 0);

            }

        }

        #endregion


    }

}
