/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests
{

    [TestFixture]
    public class CPOTests : ANodeTests
    {

        #region CPO_GetVersions_Test()

        /// <summary>
        /// CPO GetVersions Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_Test()
        {

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response = await graphDefinedEMSP.GetVersions();

                // GET /versions HTTP/1.1
                // Date:                          Sun, 25 Dec 2022 23:16:30 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7235
                // Authorization:                 Token xxxxxx
                // User-Agent:                    GraphDefined OCPI HTTP Client v1.0
                // X-Request-ID:                  nM7djM37h56hQz8t8hKMznnhGYj3CK
                // X-Correlation-ID:              53YKxAnt2zM9bGp2AvjK6t83txbCK3

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
                //         "version":  "2.2",
                //         "url":      "http://127.0.0.1:7235/versions/2.2"
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

                //ClassicAssert.IsNotNull(response.Request);

                var versions = response.Data;
                ClassicAssert.IsNotNull(versions);
                ClassicAssert.AreEqual (1, response.Data.Count());

                var version = versions.First();
                ClassicAssert.AreEqual (Version_Id.Parse("2.2"),     version.Id);
                ClassicAssert.AreEqual (emsp1VersionsAPIURL.Value + "2.2",  version.URL);

            }

        }

        #endregion

        #region CPO_GetVersions_UnknownToken_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_UnknownToken_Test()
        {

            #region Change Access Token

            ClassicAssert.IsNotNull(cpoCommonAPI);
            ClassicAssert.IsNotNull(emsp1VersionsAPIURL);

            if (cpoCommonAPI is not null &&
                emsp1VersionsAPIURL.HasValue)
            {

                var result1 = await cpoCommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Roles.EMSP);
                ClassicAssert.IsTrue(result1);

                var result2 = await cpoCommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    new[] {
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Roles.      EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             },
                                        LocalAccessInfos:    new[] {
                                                                 new LocalAccessInfo(
                                                                     AccessToken.Parse("aaaaaa"),
                                                                     AccessStatus.ALLOWED
                                                                 )
                                                             },
                                        RemoteAccessInfos:   new[] {
                                                                 new RemoteAccessInfo(
                                                                     AccessToken:        AccessToken.Parse("bbbbbb"),
                                                                     VersionsURL:        emsp1VersionsAPIURL.Value,
                                                                     VersionIds:         new Version_Id[] {
                                                                                             Version_Id.Parse("2.2")
                                                                                         },
                                                                     SelectedVersionId:  Version_Id.Parse("2.2"),
                                                                     Status:             RemoteAccessStatus.ONLINE
                                                                 )
                                                             },
                                        Status:              PartyStatus.ENABLED
                                    );

                ClassicAssert.IsTrue(result2);

            }

            #endregion

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response = await graphDefinedEMSP.GetVersions();

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
                //         "version":  "2.2",
                //         "url":      "http://127.0.0.1:7235/versions/2.2"
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

                //ClassicAssert.IsNotNull(response.Request);

                var versions = response.Data;
                ClassicAssert.IsNotNull(versions);
                ClassicAssert.AreEqual (1, response.Data.Count());

                var version = versions.First();
                ClassicAssert.AreEqual (Version_Id.Parse("2.2"),    version.Id);
                ClassicAssert.AreEqual (emsp1VersionsAPIURL.Value + "2.2", version.URL);

            }

        }

        #endregion

        #region CPO_GetVersions_BlockedToken_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_BlockedToken_Test()
        {

            #region Block Access Token

            await cpoCommonAPI.  RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Roles.EMSP);
            await emsp1CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"), Roles.CPO);

            var addEMSPResult = await cpoCommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    new[] {
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Roles.      EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             },
                                        LocalAccessInfos:    new[] {
                                                                 new LocalAccessInfo(
                                                                     AccessToken.Parse("xxxxxx"),
                                                                     AccessStatus.ALLOWED
                                                                 )
                                                             },
                                        RemoteAccessInfos:   new[] {
                                                                 new RemoteAccessInfo(
                                                                     AccessToken:        AccessToken.Parse("yyyyyy"),
                                                                     VersionsURL:        emsp1VersionsAPIURL.Value,
                                                                     VersionIds:         new Version_Id[] {
                                                                                             Version_Id.Parse("2.2")
                                                                                         },
                                                                     SelectedVersionId:  Version_Id.Parse("2.2"),
                                                                     Status:             RemoteAccessStatus.ONLINE
                                                                 )
                                                             },
                                        Status:              PartyStatus.ENABLED
                                    );

            ClassicAssert.IsTrue(addEMSPResult);


            var addCPOResult = await emsp1CommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GEF_CPO"),
                                        CredentialsRoles:    new[] {
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GEF"),
                                                                     Role:                Roles.      CPO,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined CPO Services")
                                                                 )
                                                             },
                                        LocalAccessInfos:    new[] {
                                                                 new LocalAccessInfo(
                                                                     AccessToken.Parse("yyyyyy"),
                                                                     AccessStatus.BLOCKED
                                                                 )
                                                             },
                                        RemoteAccessInfos:   new[] {
                                                                 new RemoteAccessInfo(
                                                                     AccessToken:        AccessToken.Parse("xxxxxx"),
                                                                     VersionsURL:        emsp1VersionsAPIURL.Value,
                                                                     VersionIds:         new Version_Id[] {
                                                                                             Version_Id.Parse("2.2")
                                                                                         },
                                                                     SelectedVersionId:  Version_Id.Parse("2.2"),
                                                                     Status:             RemoteAccessStatus.ONLINE
                                                                 )
                                                             },
                                        Status:              PartyStatus.ENABLED
                                    );

            ClassicAssert.IsTrue(addCPOResult);

            #endregion

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response = await graphDefinedEMSP.GetVersions();

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

                //ClassicAssert.IsNotNull(response.Request);

                var versions = response.Data;
                ClassicAssert.IsNotNull(versions);
                ClassicAssert.AreEqual (0, versions.Count());

            }

        }

        #endregion


        #region CPO_GetVersion_Test()

        /// <summary>
        /// CPO GetVersion Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetVersion_Test()
        {

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetVersionDetails(Version_Id.Parse("2.2"));

                // GET /versions/2.2 HTTP/1.1
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
                //         "version": "2.2",
                //         "endpoints": [
                //             {
                //                 "identifier":  "credentials",
                //                 "url":         "http://127.0.0.1:7235/2.2/credentials"
                //             },
                //             {
                //                 "identifier":  "locations",
                //                 "url":         "http://127.0.0.1:7235/2.2/emsp/locations"
                //             },
                //             {
                //                 "identifier":  "tariffs",
                //                 "url":         "http://127.0.0.1:7235/2.2/emsp/tariffs"
                //             },
                //             {
                //                 "identifier":  "sessions",
                //                 "url":         "http://127.0.0.1:7235/2.2/emsp/sessions"
                //             },
                //             {
                //                 "identifier":  "cdrs",
                //                 "url":         "http://127.0.0.1:7235/2.2/emsp/cdrs"
                //             },
                //             {
                //                 "identifier":  "commands",
                //                 "url":         "http://127.0.0.1:7235/2.2/emsp/commands"
                //             },
                //             {
                //                 "identifier":  "tokens",
                //                 "url":         "http://127.0.0.1:7235/2.2/emsp/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-26T00:36:21.259Z"
                // }

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (200,             response2.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,            response2.StatusCode);
                ClassicAssert.AreEqual ("Hello world!",  response2.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

                var versionDetail = response2.Data;
                ClassicAssert.IsNotNull(versionDetail);

                var endpoints = versionDetail.Endpoints;
                ClassicAssert.AreEqual (7, endpoints.Count());
                //ClassicAssert.AreEqual(Version_Id.Parse("2.2"), endpoints.Id);
                //ClassicAssert.AreEqual(emspVersionsAPIURL + "2.2", endpoints.URL);


            }

        }

        #endregion


        #region CPO_GetCredentials_Test()

        /// <summary>
        /// CPO GetCredentials Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetCredentials_Test()
        {

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetCredentials();

                // GET /2.2/credentials HTTP/1.1
                // Date:                          Mon, 26 Dec 2022 10:29:48 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7235
                // Authorization:                 Token yyyyyy
                // X-Request-ID:                  7AYph123pWAUt7j1Ad3n1jh1G279xG
                // X-Correlation-ID:              jhz1GGj3j83SE7Wrf42p8hM82rM3A3

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 10:29:49 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                296
                // X-Request-ID:                  7AYph123pWAUt7j1Ad3n1jh1G279xG
                // X-Correlation-ID:              jhz1GGj3j83SE7Wrf42p8hM82rM3A3
                // 
                // {
                //    "data": {
                //        "token":         "yyyyyy",
                //        "url":           "http://127.0.0.1:7235/versions",
                //        "business_details": {
                //            "name":           "GraphDefined EMSP Services",
                //            "website":        "https://www.graphdefined.com/emsp"
                //        },
                //        "country_code":  "DE",
                //        "party_id":      "GDF"
                //    },
                //    "status_code":      1000,
                //    "status_message":  "Hello world!",
                //    "timestamp":       "2022-12-26T10:29:49.143Z"
                //}

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (200,             response2.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,            response2.StatusCode);
                ClassicAssert.AreEqual ("Hello world!",  response2.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

                var credentials = response2.Data;
                ClassicAssert.IsNotNull(credentials);
                ClassicAssert.AreEqual("yyyyyy",                             credentials.Token.                    ToString());
                ClassicAssert.AreEqual("http://127.0.0.1:7235/versions",     credentials.URL.                      ToString());
                ClassicAssert.AreEqual("DE",                                 credentials.Roles.First().CountryCode.ToString());
                ClassicAssert.AreEqual("GDF",                                credentials.Roles.First().PartyId.    ToString());

                var businessDetails = credentials.Roles.First().BusinessDetails;
                ClassicAssert.IsNotNull(businessDetails);
                ClassicAssert.AreEqual("GraphDefined EMSP Services",         businessDetails.Name);
                ClassicAssert.AreEqual("https://www.graphdefined.com/emsp",  businessDetails.Website.    ToString());

            }

        }

        #endregion

        #region CPO_GetCredentials_UnknownToken_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetCredentials_UnknownToken_Test()
        {

            #region Change Access Token

            await cpoCommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Roles.EMSP);

            var result = await cpoCommonAPI.AddRemoteParty(
                Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    new[] {
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Roles.      EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             },
                LocalAccessInfos:    new[] {
                                         new LocalAccessInfo(
                                             AccessToken.Parse("aaaaaa"),
                                             AccessStatus.ALLOWED
                                         )
                                     },
                RemoteAccessInfos:   new[] {
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("bbbbbb"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         new Version_Id[] {
                                                                     Version_Id.Parse("2.2")
                                                                 },
                                             SelectedVersionId:  Version_Id.Parse("2.2"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     },
                Status:              PartyStatus.ENABLED
            );

            #endregion

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetCredentials();

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 15:14:30 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                296
                // X-Request-ID:                  7AYph123pWAUt7j1Ad3n1jh1G279xG
                // X-Correlation-ID:              jhz1GGj3j83SE7Wrf42p8hM82rM3A3
                // 
                // {
                //    "data": {
                //        "token":         "<any>",
                //        "url":           "http://127.0.0.1:7235/versions",
                //        "business_details": {
                //            "name":           "GraphDefined EMSP Services",
                //            "website":        "https://www.graphdefined.com/emsp"
                //        },
                //        "country_code":  "DE",
                //        "party_id":      "GDF"
                //    },
                //    "status_code":      1000,
                //    "status_message":  "Hello world!",
                //    "timestamp":       "2022-12-26T15:14:30.143Z"
                //}

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (200,             response2.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,            response2.StatusCode);
                ClassicAssert.AreEqual ("Hello world!",  response2.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

                var credentials = response2.Data;
                ClassicAssert.IsNotNull(credentials);
                ClassicAssert.AreEqual("<any>",                              credentials.Token.                    ToString());
                ClassicAssert.AreEqual("http://127.0.0.1:7235/versions",     credentials.URL.                      ToString());
                ClassicAssert.AreEqual("DE",                                 credentials.Roles.First().CountryCode.ToString());
                ClassicAssert.AreEqual("GDF",                                credentials.Roles.First().PartyId.    ToString());

                var businessDetails = credentials.Roles.First().BusinessDetails;
                ClassicAssert.IsNotNull(businessDetails);
                ClassicAssert.AreEqual("GraphDefined EMSP Services",         businessDetails.Name);
                ClassicAssert.AreEqual("https://www.graphdefined.com/emsp",  businessDetails.Website.    ToString());

            }

        }

        #endregion

        #region CPO_GetCredentials_BlockedToken1_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetCredentials_BlockedToken1_Test()
        {

            #region Block Access Token

            await cpoCommonAPI.  RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Roles.EMSP);
            await emsp1CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"), Roles.CPO);

            var addEMSPResult = await cpoCommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    new[] {
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Roles.      EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             },
                LocalAccessInfos:    new[] {
                                         new LocalAccessInfo(
                                             AccessToken.Parse("xxxxxx"),
                                             AccessStatus.ALLOWED
                                         )
                                     },
                RemoteAccessInfos:   new[] {
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("yyyyyy"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         new Version_Id[] {
                                                                     Version_Id.Parse("2.2")
                                                                 },
                                             SelectedVersionId:  Version_Id.Parse("2.2"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     },
                Status:              PartyStatus.ENABLED
            );

            ClassicAssert.IsTrue(addEMSPResult);


            var addCPOResult = await emsp1CommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GEF_CPO"),
                                        CredentialsRoles:    new[] {
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GEF"),
                                                                     Role:                Roles.      CPO,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined CPO Services")
                                                                 )
                                                             },
                LocalAccessInfos:    new[] {
                                         new LocalAccessInfo(
                                             AccessToken.Parse("yyyyyy"),
                                             AccessStatus.BLOCKED
                                         )
                                     },
                RemoteAccessInfos:   new[] {
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("xxxxxx"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         new Version_Id[] {
                                                                     Version_Id.Parse("2.2")
                                                                 },
                                             SelectedVersionId:  Version_Id.Parse("2.2"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     },
                Status:              PartyStatus.ENABLED
            );

            ClassicAssert.IsTrue(addCPOResult);

            #endregion

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetCredentials();

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.IsNull   (response2.HTTPResponse);
                ClassicAssert.AreEqual (-1,                         response2.StatusCode);
                ClassicAssert.AreEqual ("No versionId available!",  response2.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

                var credentials = response2.Data;
                ClassicAssert.IsNull(credentials);
                //ClassicAssert.AreEqual("<any>",                              credentials.    Token.      ToString());
                //ClassicAssert.AreEqual("http://127.0.0.1:7235/versions",     credentials.    URL.        ToString());
                //ClassicAssert.AreEqual("DE",                                 credentials.    CountryCode.ToString());
                //ClassicAssert.AreEqual("GDF",                                credentials.    PartyId.    ToString());

                //var businessDetails = credentials.BusinessDetails;
                //ClassicAssert.IsNotNull(businessDetails);
                //ClassicAssert.AreEqual("GraphDefined EMSP Services",         businessDetails.Name);
                //ClassicAssert.AreEqual("https://www.graphdefined.com/emsp",  businessDetails.Website.    ToString());

            }

        }

        #endregion

        #region CPO_GetCredentials_BlockedToken2_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetCredentials_BlockedToken2_Test()
        {

            #region Block Access Token

            await cpoCommonAPI.  RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Roles.EMSP);
            await emsp1CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"), Roles.CPO);

            var addEMSPResult = await cpoCommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    new[] {
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Roles.      EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             },
                LocalAccessInfos:    new[] {
                                         new LocalAccessInfo(
                                             AccessToken.Parse("xxxxxx"),
                                             AccessStatus.ALLOWED
                                         )
                                     },
                RemoteAccessInfos:   new[] {
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("yyyyyy"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         new Version_Id[] {
                                                                     Version_Id.Parse("2.2")
                                                                 },
                                             SelectedVersionId:  Version_Id.Parse("2.2"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     },
                Status:              PartyStatus.ENABLED
            );

            ClassicAssert.IsTrue(addEMSPResult);


            var addCPOResult = await emsp1CommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GEF_CPO"),
                                        CredentialsRoles:    new[] {
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GEF"),
                                                                     Role:                Roles.      CPO,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined CPO Services")
                                                                 )
                                                             },
                LocalAccessInfos:    new[] {
                                         new LocalAccessInfo(
                                             AccessToken.Parse("yyyyyy"),
                                             AccessStatus.BLOCKED
                                         )
                                     },
                RemoteAccessInfos:   new[] {
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("xxxxxx"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         new Version_Id[] {
                                                                     Version_Id.Parse("2.2")
                                                                 },
                                             SelectedVersionId:  Version_Id.Parse("2.2"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     },
                Status:              PartyStatus.ENABLED
            );

            ClassicAssert.IsTrue(addCPOResult);

            #endregion

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetCredentials(Version_Id.Parse("2.2"));

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.IsNull   (response2.HTTPResponse);
                ClassicAssert.AreEqual (-1,                          response2.StatusCode);
                ClassicAssert.AreEqual ("No remote URL available!",  response2.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

                var credentials = response2.Data;
                ClassicAssert.IsNull(credentials);

            }

        }

        #endregion

        #region CPO_GetCredentials_BlockedToken3_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetCredentials_BlockedToken3_Test()
        {

            #region Block Access Token

            await cpoCommonAPI.  RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Roles.EMSP);
            await emsp1CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"), Roles.CPO);

            var addEMSPResult = await cpoCommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    new[] {
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Roles.      EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             },
                LocalAccessInfos:    new[] {
                                         new LocalAccessInfo(
                                             AccessToken.Parse("xxxxxx"),
                                             AccessStatus.ALLOWED
                                         )
                                     },
                RemoteAccessInfos:   new[] {
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("yyyyyy"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         new Version_Id[] {
                                                                     Version_Id.Parse("2.2")
                                                                 },
                                             SelectedVersionId:  Version_Id.Parse("2.2"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     },
                Status:              PartyStatus.ENABLED
            );

            ClassicAssert.IsTrue(addEMSPResult);


            var addCPOResult = await emsp1CommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GEF_CPO"),
                                        CredentialsRoles:    new[] {
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GEF"),
                                                                     Role:                Roles.      CPO,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined CPO Services")
                                                                 )
                                                             },
                LocalAccessInfos:    new[] {
                                         new LocalAccessInfo(
                                             AccessToken.Parse("yyyyyy"),
                                             AccessStatus.BLOCKED
                                         )
                                     },
                RemoteAccessInfos:   new[] {
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("xxxxxx"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         new Version_Id[] {
                                                                     Version_Id.Parse("2.2")
                                                                 },
                                             SelectedVersionId:  Version_Id.Parse("2.2"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     },
                Status:              PartyStatus.ENABLED
            );

            ClassicAssert.IsTrue(addCPOResult);

            #endregion

            var httpResponse = await TestHelpers.JSONRequest(URL.Parse("http://127.0.0.1:7235/2.2/credentials"),
                                                             "yyyyyy");

            // HTTP/1.1 403 Forbidden
            // Date:                          Mon, 26 Dec 2022 15:43:44 GMT
            // Access-Control-Allow-Methods:  OPTIONS, GET
            // Access-Control-Allow-Headers:  Authorization
            // Server:                        GraphDefined Hermod HTTP Server v1.0
            // Access-Control-Allow-Origin:   *
            // Connection:                    close
            // Content-Type:                  application/json; charset=utf-8
            // Content-Length:                111
            // X-Request-ID:                  1234
            // X-Correlation-ID:              5678
            // 
            // {
            //     "status_code":      2000,
            //     "status_message":  "Invalid or blocked access token!",
            //     "timestamp":       "2022-12-26T15:43:44.533Z"
            // }

            ClassicAssert.IsNotNull(httpResponse);
            ClassicAssert.AreEqual (403,  httpResponse.HTTPStatusCode.Code);

            var jsonResponse = JObject.Parse(httpResponse.HTTPBodyAsUTF8String);
            ClassicAssert.IsNotNull(jsonResponse);

            ClassicAssert.AreEqual (2000,                                jsonResponse["status_code"]?.   Value<Int32>());
            ClassicAssert.AreEqual ("Invalid or blocked access token!",  jsonResponse["status_message"]?.Value<String>());
            ClassicAssert.IsTrue   (Timestamp.Now - jsonResponse["timestamp"]?.Value<DateTime>() < TimeSpan.FromSeconds(10));

            //ClassicAssert.IsNotNull(response.Request);

        }

        #endregion


        #region CPO_PutCredentials_NotYetRegistered_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_PutCredentials_NotYetRegistered_Test()
        {

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response2 = await graphDefinedEMSP.PutCredentials(
                                                           new Credentials(
                                                               AccessToken.Parse("nnnnnn"),
                                                               URL.Parse("http://example.org/versions"),
                                                               new[] {
                                                                   new CredentialsRole(
                                                                       CountryCode.Parse("DE"),
                                                                       Party_Id.   Parse("EXP"),
                                                                       Roles.      CPO,
                                                                       new BusinessDetails(
                                                                           "Example Org",
                                                                           URL.Parse("http://example.org")
                                                                       )
                                                                   )
                                                               }
                                                           )
                                                       );

                // HTTP/1.1 405 Method Not Allowed
                // Date:                          Mon, 26 Dec 2022 15:29:55 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                151
                // X-Request-ID:                  zd4A4h2Kp6vY28nYQC1j616bd6569d
                // X-Correlation-ID:              An625Y7Yv1Un5K8AS33G2pUCbpCpjC
                // 
                // {
                //     "status_code":     2000,
                //     "status_message": "You need to be registered before trying to invoke this protected method!",
                //     "timestamp":      "2022-12-26T15:29:55.424Z"
                // }

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (405,                                                                         response2.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (2000,                                                                        response2.StatusCode);
                ClassicAssert.AreEqual ("You need to be registered before trying to invoke this protected method!",  response2.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

                ClassicAssert.IsNull   (response2.Data);

            }

        }

        #endregion

        #region CPO_PutCredentials_UnknownToken_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_PutCredentials_UnknownToken_Test()
        {

            #region Change Access Token

            if (cpoCommonAPI is not null)
            {

                var result1 = await cpoCommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Roles.EMSP);

                ClassicAssert.IsTrue(result1);


                var result2 = await cpoCommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    new[] {
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Roles.      EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             },
                                        LocalAccessInfos:    new[] {
                                                                 new LocalAccessInfo(
                                                                     AccessToken.Parse("aaaaaa"),
                                                                     AccessStatus.ALLOWED
                                                                 )
                                                             },
                                        RemoteAccessInfos:   new[] {
                                                                 new RemoteAccessInfo(
                                                                     AccessToken:        AccessToken.Parse("bbbbbb"),
                                                                     VersionsURL:        emsp1VersionsAPIURL.Value,
                                                                     VersionIds:         new Version_Id[] {
                                                                                             Version_Id.Parse("2.2")
                                                                                         },
                                                                     SelectedVersionId:  Version_Id.Parse("2.2"),
                                                                     Status:             RemoteAccessStatus.ONLINE
                                                                 )
                                                             },
                                        Status:              PartyStatus.ENABLED
                                    );

                ClassicAssert.IsTrue(result2);

            }

            #endregion

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.PutCredentials(
                                                           new Credentials(
                                                               AccessToken.Parse("nnnnnn"),
                                                               URL.Parse("http://example.org/versions"),
                                                               new[] {
                                                                   new CredentialsRole(
                                                                       CountryCode.Parse("DE"),
                                                                       Party_Id.   Parse("EXP"),
                                                                       Roles.      CPO,
                                                                       new BusinessDetails(
                                                                           "Example Org",
                                                                           URL.Parse("http://example.org")
                                                                       )
                                                                   )
                                                               }
                                                           )
                                                       );

                // HTTP/1.1 405 Method Not Allowed
                // Date:                          Mon, 26 Dec 2022 15:29:55 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                151
                // X-Request-ID:                  zd4A4h2Kp6vY28nYQC1j616bd6569d
                // X-Correlation-ID:              An625Y7Yv1Un5K8AS33G2pUCbpCpjC
                // 
                // {
                //     "status_code":     2000,
                //     "status_message": "You need to be registered before trying to invoke this protected method!",
                //     "timestamp":      "2022-12-26T15:29:55.424Z"
                // }

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (405,                                                                         response2.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (2000,                                                                        response2.StatusCode);
                ClassicAssert.AreEqual ("You need to be registered before trying to invoke this protected method!",  response2.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

                ClassicAssert.IsNull   (response2.Data);

            }

        }

        #endregion


        #region CPO_Register_RR_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_Register_RR_Test()
        {

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var remoteAccessInfoOld  = cpoCommonAPI?.  RemoteParties.First().RemoteAccessInfos.First();
                var accessInfoOld        = emsp1CommonAPI?.RemoteParties.First().LocalAccessInfos. First();

                var response1            = await graphDefinedEMSP.GetVersions();
                var response2            = await graphDefinedEMSP.Register();

                // HTTP/1.1 200 OK
                // Date:                          Tue, 27 Dec 2022 09:16:14 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                340
                // X-Request-ID:                  vzY6jY44Gjf995rAC14487f3K1Wpr5
                // X-Correlation-ID:              Ev63U91239t523E1Q9xb41fA3dCzC4
                // 
                // {
                //     "data": {
                //         "token":         "tM1bM71zW39f9W46WM5K9W46h6rYtdYfK1nS46rjA6Cf9ffY9h",
                //         "url":           "http://127.0.0.1:7235/versions",
                //         "business_details": {
                //             "name":            "GraphDefined EMSP Services",
                //             "website":         "https://www.graphdefined.com/emsp"
                //         },
                //         "country_code":    "DE",
                //         "party_id":        "GDF"
                //     },
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-27T09:16:14.632Z"
                // }

                ClassicAssert.IsNotNull(response2);
                ClassicAssert.AreEqual (200,             response2.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,            response2.StatusCode);
                ClassicAssert.AreEqual ("Hello world!",  response2.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

                var remoteAccessInfoNew  = cpoCommonAPI?.  RemoteParties.First().RemoteAccessInfos.First();
                ClassicAssert.IsNotNull  (remoteAccessInfoNew);
                ClassicAssert.AreNotEqual(remoteAccessInfoOld?.AccessToken.ToString(),  remoteAccessInfoNew?.AccessToken.ToString());

                var accessInfoNew        = emsp1CommonAPI?.RemoteParties.First().LocalAccessInfos. First();

            }

        }

        #endregion


        #region CPO_PutLocation_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_PutLocation_Test()
        {

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1            = await graphDefinedEMSP.GetVersions();
                var response2            = await graphDefinedEMSP.Register();
                var response3            = await graphDefinedEMSP.PutLocation(new Location(
                                                                                  CountryCode.Parse("DE"),
                                                                                  Party_Id.Parse("GEF"),
                                                                                  Location_Id.Parse("LOC0001"),
                                                                                  true,
                                                                                  "Biberweg 18",
                                                                                  "Jena",
                                                                                  Country.Germany,
                                                                                  GeoCoordinate.Parse(10, 20),
                                                                                  "Europe/Berlin",
                                                                                  null,
                                                                                  "Location 0001",
                                                                                  "07749",
                                                                                  "Thüringen",
                                                                                  new[] {
                                                                                      new AdditionalGeoLocation(
                                                                                          Latitude.Parse(11),
                                                                                          Longitude.Parse(22),
                                                                                          Name: DisplayText.Create(Languages.de, "Postkasten")
                                                                                      )
                                                                                  },
                                                                                  ParkingType.PARKING_LOT,
                                                                                  new[] {
                                                                                      new EVSE(
                                                                                          EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                                                                          StatusType.AVAILABLE,
                                                                                          new[] {
                                                                                              new Connector(
                                                                                                  Connector_Id.Parse("1"),
                                                                                                  ConnectorType.IEC_62196_T2,
                                                                                                  ConnectorFormats.SOCKET,
                                                                                                  PowerTypes.AC_3_PHASE,
                                                                                                  Volt.  ParseV(400),
                                                                                                  Ampere.ParseA(30),
                                                                                                  Watt.  ParseW(12),
                                                                                                  new[] {
                                                                                                      Tariff_Id.Parse("DE*GEF*T0001"),
                                                                                                      Tariff_Id.Parse("DE*GEF*T0002")
                                                                                                  },
                                                                                                  URL.Parse("https://open.charging.cloud/terms"),
                                                                                                  DateTime.Parse("2020-09-21")
                                                                                              ),
                                                                                              new Connector(
                                                                                                  Connector_Id.Parse("2"),
                                                                                                  ConnectorType.IEC_62196_T2_COMBO,
                                                                                                  ConnectorFormats.CABLE,
                                                                                                  PowerTypes.AC_3_PHASE,
                                                                                                  Volt.  ParseV(400),
                                                                                                  Ampere.ParseA(20),
                                                                                                  Watt.  ParseW(8),
                                                                                                  new[] {
                                                                                                      Tariff_Id.Parse("DE*GEF*T0003"),
                                                                                                      Tariff_Id.Parse("DE*GEF*T0004")
                                                                                                  },
                                                                                                  URL.Parse("https://open.charging.cloud/terms"),
                                                                                                  DateTime.Parse("2020-09-22")
                                                                                              )
                                                                                          },
                                                                                          EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                                                                          new[] {
                                                                                              new StatusSchedule(
                                                                                                  StatusType.INOPERATIVE,
                                                                                                  DateTime.Parse("2020-09-23"),
                                                                                                  DateTime.Parse("2020-09-24")
                                                                                              ),
                                                                                              new StatusSchedule(
                                                                                                  StatusType.OUTOFORDER,
                                                                                                  DateTime.Parse("2020-12-30"),
                                                                                                  DateTime.Parse("2020-12-31")
                                                                                              )
                                                                                          },
                                                                                          new[] {
                                                                                              Capability.RFID_READER,
                                                                                              Capability.RESERVABLE
                                                                                          },

                                                                                          // OCPI Computer Science Extensions
                                                                                          new EnergyMeter(
                                                                                              Meter_Id.Parse("Meter0815"),
                                                                                              "EnergyMeter Model #1",
                                                                                              null,
                                                                                              "hw. v1.80",
                                                                                              "fw. v1.20",
                                                                                              "Energy Metering Services",
                                                                                              null,
                                                                                              null,
                                                                                              null
                                                                                          ),

                                                                                          "1. Stock",
                                                                                          GeoCoordinate.Parse(10.1, 20.2),
                                                                                          "Ladestation #1",
                                                                                          new[] {
                                                                                              DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                                                                              DisplayText.Create(Languages.en, "Ken sent me!")
                                                                                          },
                                                                                          new[] {
                                                                                              ParkingRestriction.EV_ONLY,
                                                                                              ParkingRestriction.PLUGGED
                                                                                          },
                                                                                          new[] {
                                                                                              new Image(
                                                                                                  URL.Parse("http://example.com/pinguine.jpg"),
                                                                                                  ImageFileType.jpeg,
                                                                                                  ImageCategory.OPERATOR,
                                                                                                  100,
                                                                                                  150,
                                                                                                  URL.Parse("http://example.com/kleine_pinguine.jpg")
                                                                                              )
                                                                                          },
                                                                                          DateTime.Parse("2020-09-22")
                                                                                      )
                                                                                  },
                                                                                  new[] {
                                                                                      new DisplayText(Languages.de, "Hallo Welt!"),
                                                                                      new DisplayText(Languages.en, "Hello world!")
                                                                                  },
                                                                                  new BusinessDetails(
                                                                                      "Open Charging Cloud",
                                                                                      URL.Parse("https://open.charging.cloud"),
                                                                                      new Image(
                                                                                          URL.Parse("http://open.charging.cloud/logo.svg"),
                                                                                          ImageFileType.svg,
                                                                                          ImageCategory.OPERATOR,
                                                                                          1000,
                                                                                          1500,
                                                                                          URL.Parse("http://open.charging.cloud/logo_small.svg")
                                                                                      )
                                                                                  ),
                                                                                  new BusinessDetails(
                                                                                      "GraphDefined GmbH",
                                                                                      URL.Parse("https://www.graphdefined.com"),
                                                                                      new Image(
                                                                                          URL.Parse("http://www.graphdefined.com/logo.png"),
                                                                                          ImageFileType.png,
                                                                                          ImageCategory.OPERATOR,
                                                                                          2000,
                                                                                          3000,
                                                                                          URL.Parse("http://www.graphdefined.com/logo_small.png")
                                                                                      )
                                                                                  ),
                                                                                  new BusinessDetails(
                                                                                      "Achim Friedland",
                                                                                      URL.Parse("https://ahzf.de"),
                                                                                      new Image(
                                                                                          URL.Parse("http://ahzf.de/logo.gif"),
                                                                                          ImageFileType.gif,
                                                                                          ImageCategory.OWNER,
                                                                                          3000,
                                                                                          4500,
                                                                                          URL.Parse("http://ahzf.de/logo_small.gif")
                                                                                      )
                                                                                  ),
                                                                                  new[] {
                                                                                      Facilities.CAFE
                                                                                  },
                                                                                  new Hours(
                                                                                      new[] {
                                                                                          new OCPI.RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                                                                          new OCPI.RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                                                                          new OCPI.RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                                                                          new OCPI.RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                                                                          new OCPI.RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                                                                      },
                                                                                      new[] {
                                                                                          new OCPI.ExceptionalPeriod(
                                                                                              DateTime.Parse("2020-09-21T00:00:00Z"),
                                                                                              DateTime.Parse("2020-09-22T00:00:00Z")
                                                                                          )
                                                                                      },
                                                                                      new[] {
                                                                                          new OCPI.ExceptionalPeriod(
                                                                                              DateTime.Parse("2020-12-24T00:00:00Z"),
                                                                                              DateTime.Parse("2020-12-26T00:00:00Z")
                                                                                          )
                                                                                      }
                                                                                  ),
                                                                                  false,
                                                                                  new[] {
                                                                                      new Image(
                                                                                          URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                                                                          ImageFileType.jpeg,
                                                                                          ImageCategory.LOCATION,
                                                                                          200,
                                                                                          400,
                                                                                          URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                                                                      )
                                                                                  },
                                                                                  new(
                                                                                      true,
                                                                                      new[] {
                                                                                          new EnergySource(
                                                                                              EnergySourceCategory.SOLAR,
                                                                                              80
                                                                                          ),
                                                                                          new EnergySource(
                                                                                              EnergySourceCategory.WIND,
                                                                                              20
                                                                                          )
                                                                                      },
                                                                                      new[] {
                                                                                          new EnvironmentalImpact(
                                                                                              EnvironmentalImpactCategory.CARBON_DIOXIDE,
                                                                                              0.1
                                                                                          )
                                                                                      },
                                                                                      "Stadtwerke Jena-Ost",
                                                                                      "New Green Deal"
                                                                                  ),
                                                                                  DateTime.Parse("2020-09-21T00:00:00Z")
                                                                              ));

                // HTTP/1.1 201 Created
                // Date:                          Wed, 28 Dec 2022 22:54:40 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, PUT, PATCH, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Last-Modified:                 2020-09-21T00:00:00.000Z
                // ETag:                          Q8R8UfdvHYd/jE3j+FjO0r84MiRmqt/8+PXxZ+Z43sg=
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                4932
                // X-Request-ID:                  G62h4r32Yd9rrQS579jWGpf49rS2Wz
                // X-Correlation-ID:              3xE3Y25Atv8633zYz3U6Qr945457pt
                // 
                // {
                //     "data":            {"id":"LOC0001","location_type":"PARKING_LOT","name":"Location 0001","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"related_locations":[{"latitude":"11","longitude":"22","name":{"language":"de","text":"Postkasten"}}],"evses":[{"uid":"DE*GEF*E*LOC0001*1","evse_id":"DE*GEF*E*LOC0001*1","status":"AVAILABLE","status_schedule":[{"period_begin":"2020-09-22T22:00:00.000Z","period_end":"2020-09-23T22:00:00.000Z","status":"INOPERATIVE"},{"period_begin":"2020-12-29T23:00:00.000Z","period_end":"2020-12-30T23:00:00.000Z","status":"OUTOFORDER"}],"capabilities":["RFID_READER","RESERVABLE"],"connectors":[{"id":"1","standard":"IEC_62196_T2","format":"SOCKET","power_type":"AC_3_PHASE","voltage":400,"amperage":30,"terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-20T22:00:00.000Z"},{"id":"2","standard":"IEC_62196_T2_COMBO","format":"CABLE","power_type":"AC_3_PHASE","voltage":400,"amperage":20,"terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-21T22:00:00.000Z"}],"energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","vendor":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}]},"floor_level":"1. Stock","coordinates":{"latitude":"10.10000","longitude":"20.20000"},"physical_reference":"Ladestation #1","directions":[{"language":"de","text":"Bitte klingeln!"},{"language":"en","text":"Ken sent me!"}],"parking_restrictions":["EV_ONLY","PLUGGED"],"images":[{"url":"http://example.com/pinguine.jpg","category":"OPERATOR","type":"jpeg","thumbnail":"http://example.com/kleine_pinguine.jpg","width":100,"height":150}],"last_updated":"2020-09-21T22:00:00.000Z"}],"directions":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"operator":{"name":"Open Charging Cloud","website":"https://open.charging.cloud","logo":{"url":"http://open.charging.cloud/logo.svg","category":"OPERATOR","type":"svg","thumbnail":"http://open.charging.cloud/logo_small.svg","width":1000,"height":1500}},"suboperator":{"name":"GraphDefined GmbH","website":"https://www.graphdefined.com","logo":{"url":"http://www.graphdefined.com/logo.png","category":"OPERATOR","type":"png","thumbnail":"http://www.graphdefined.com/logo_small.png","width":2000,"height":3000}},"owner":{"name":"Achim Friedland","website":"https://ahzf.de","logo":{"url":"http://ahzf.de/logo.gif","category":"OWNER","type":"gif","thumbnail":"http://ahzf.de/logo_small.gif","width":3000,"height":4500}},"facilities":["CAFE"],"time_zone":"Europe/Berlin","opening_times":{"twentyfourseven":false,"regular_hours":[{"weekday":1,"period_begin":"08:00","period_end":"15:00"},{"weekday":2,"period_begin":"09:00","period_end":"16:00"},{"weekday":3,"period_begin":"10:00","period_end":"17:00"},{"weekday":4,"period_begin":"11:00","period_end":"18:00"},{"weekday":5,"period_begin":"12:00","period_end":"19:00"}],"exceptional_openings":[{"period_begin":"2020-09-21T00:00:00.000Z","period_end":"2020-09-22T00:00:00.000Z"}],"exceptional_closings":[{"period_begin":"2020-12-24T00:00:00.000Z","period_end":"2020-12-26T00:00:00.000Z"}]},"charging_when_closed":false,"images":[{"url":"http://open.charging.cloud/locations/location0001.jpg","category":"LOCATION","type":"jpeg","thumbnail":"http://open.charging.cloud/locations/location0001s.jpg","width":200,"height":400}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T00:00:00.000Z"},
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-28T22:54:40.784Z"
                // }

                ClassicAssert.IsNotNull(response3);
                ClassicAssert.AreEqual (201,             response3.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,            response3.StatusCode);
                ClassicAssert.AreEqual ("Hello world!",  response3.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now -  response3.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region CPO_PutEVSE_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_PutEVSE_Test()
        {

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1            = await graphDefinedEMSP.GetVersions();
                var response2            = await graphDefinedEMSP.Register();
                var response3            = await graphDefinedEMSP.PutLocation(new Location(
                                                                                  CountryCode.Parse("DE"),
                                                                                  Party_Id.   Parse("GEF"),
                                                                                  Location_Id.Parse("LOC0001"),
                                                                                  true,
                                                                                  "Biberweg 18",
                                                                                  "Jena",
                                                                                  Country.Germany,
                                                                                  GeoCoordinate.Parse(10, 20),
                                                                                  "Europe/Berlin",
                                                                                  null,
                                                                                  "Location 0001",
                                                                                  "07749",
                                                                                  "Thüringen",
                                                                                  new[] {
                                                                                      new AdditionalGeoLocation(
                                                                                          Latitude.Parse(11),
                                                                                          Longitude.Parse(22),
                                                                                          Name: DisplayText.Create(Languages.de, "Postkasten")
                                                                                      )
                                                                                  },
                                                                                  ParkingType.PARKING_LOT,
                                                                                  new[] {
                                                                                      new EVSE(
                                                                                          EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                                                                          StatusType.AVAILABLE,
                                                                                          new[] {
                                                                                              new Connector(
                                                                                                  Connector_Id.Parse("1"),
                                                                                                  ConnectorType.IEC_62196_T2,
                                                                                                  ConnectorFormats.SOCKET,
                                                                                                  PowerTypes.AC_3_PHASE,
                                                                                                  Volt.  ParseV(400),
                                                                                                  Ampere.ParseA(30),
                                                                                                  Watt.  ParseW(12),
                                                                                                  new[] {
                                                                                                      Tariff_Id.Parse("DE*GEF*T0001"),
                                                                                                      Tariff_Id.Parse("DE*GEF*T0002")
                                                                                                  },
                                                                                                  URL.Parse("https://open.charging.cloud/terms"),
                                                                                                  DateTime.Parse("2020-09-21")
                                                                                              ),
                                                                                              new Connector(
                                                                                                  Connector_Id.Parse("2"),
                                                                                                  ConnectorType.IEC_62196_T2_COMBO,
                                                                                                  ConnectorFormats.CABLE,
                                                                                                  PowerTypes.AC_3_PHASE,
                                                                                                  Volt.  ParseV(400),
                                                                                                  Ampere.ParseA(20),
                                                                                                  Watt.  ParseW(8),
                                                                                                  new[] {
                                                                                                      Tariff_Id.Parse("DE*GEF*T0003"),
                                                                                                      Tariff_Id.Parse("DE*GEF*T0004")
                                                                                                  },
                                                                                                  URL.Parse("https://open.charging.cloud/terms"),
                                                                                                  DateTime.Parse("2020-09-22")
                                                                                              )
                                                                                          },
                                                                                          EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                                                                          new[] {
                                                                                              new StatusSchedule(
                                                                                                  StatusType.INOPERATIVE,
                                                                                                  DateTime.Parse("2020-09-23"),
                                                                                                  DateTime.Parse("2020-09-24")
                                                                                              ),
                                                                                              new StatusSchedule(
                                                                                                  StatusType.OUTOFORDER,
                                                                                                  DateTime.Parse("2020-12-30"),
                                                                                                  DateTime.Parse("2020-12-31")
                                                                                              )
                                                                                          },
                                                                                          new[] {
                                                                                              Capability.RFID_READER,
                                                                                              Capability.RESERVABLE
                                                                                          },

                                                                                          // OCPI Computer Science Extensions
                                                                                          new EnergyMeter(
                                                                                              Meter_Id.Parse("Meter0815"),
                                                                                              "EnergyMeter Model #1",
                                                                                              null,
                                                                                              "hw. v1.80",
                                                                                              "fw. v1.20",
                                                                                              "Energy Metering Services",
                                                                                              null,
                                                                                              null,
                                                                                              null
                                                                                          ),

                                                                                          "1. Stock",
                                                                                          GeoCoordinate.Parse(10.1, 20.2),
                                                                                          "Ladestation #1",
                                                                                          new[] {
                                                                                              DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                                                                              DisplayText.Create(Languages.en, "Ken sent me!")
                                                                                          },
                                                                                          new[] {
                                                                                              ParkingRestriction.EV_ONLY,
                                                                                              ParkingRestriction.PLUGGED
                                                                                          },
                                                                                          new[] {
                                                                                              new Image(
                                                                                                  URL.Parse("http://example.com/pinguine.jpg"),
                                                                                                  ImageFileType.jpeg,
                                                                                                  ImageCategory.OPERATOR,
                                                                                                  100,
                                                                                                  150,
                                                                                                  URL.Parse("http://example.com/kleine_pinguine.jpg")
                                                                                              )
                                                                                          },
                                                                                          DateTime.Parse("2020-09-22")
                                                                                      )
                                                                                  },
                                                                                  new[] {
                                                                                      new DisplayText(Languages.de, "Hallo Welt!"),
                                                                                      new DisplayText(Languages.en, "Hello world!")
                                                                                  },
                                                                                  new BusinessDetails(
                                                                                      "Open Charging Cloud",
                                                                                      URL.Parse("https://open.charging.cloud"),
                                                                                      new Image(
                                                                                          URL.Parse("http://open.charging.cloud/logo.svg"),
                                                                                          ImageFileType.svg,
                                                                                          ImageCategory.OPERATOR,
                                                                                          1000,
                                                                                          1500,
                                                                                          URL.Parse("http://open.charging.cloud/logo_small.svg")
                                                                                      )
                                                                                  ),
                                                                                  new BusinessDetails(
                                                                                      "GraphDefined GmbH",
                                                                                      URL.Parse("https://www.graphdefined.com"),
                                                                                      new Image(
                                                                                          URL.Parse("http://www.graphdefined.com/logo.png"),
                                                                                          ImageFileType.png,
                                                                                          ImageCategory.OPERATOR,
                                                                                          2000,
                                                                                          3000,
                                                                                          URL.Parse("http://www.graphdefined.com/logo_small.png")
                                                                                      )
                                                                                  ),
                                                                                  new BusinessDetails(
                                                                                      "Achim Friedland",
                                                                                      URL.Parse("https://ahzf.de"),
                                                                                      new Image(
                                                                                          URL.Parse("http://ahzf.de/logo.gif"),
                                                                                          ImageFileType.gif,
                                                                                          ImageCategory.OWNER,
                                                                                          3000,
                                                                                          4500,
                                                                                          URL.Parse("http://ahzf.de/logo_small.gif")
                                                                                      )
                                                                                  ),
                                                                                  new[] {
                                                                                      Facilities.CAFE
                                                                                  },
                                                                                  new Hours(
                                                                                      new[] {
                                                                                          new OCPI.RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                                                                          new OCPI.RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                                                                          new OCPI.RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                                                                          new OCPI.RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                                                                          new OCPI.RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                                                                      },
                                                                                      new[] {
                                                                                          new OCPI.ExceptionalPeriod(
                                                                                              DateTime.Parse("2020-09-21T00:00:00Z"),
                                                                                              DateTime.Parse("2020-09-22T00:00:00Z")
                                                                                          )
                                                                                      },
                                                                                      new[] {
                                                                                          new OCPI.ExceptionalPeriod(
                                                                                              DateTime.Parse("2020-12-24T00:00:00Z"),
                                                                                              DateTime.Parse("2020-12-26T00:00:00Z")
                                                                                          )
                                                                                      }
                                                                                  ),
                                                                                  false,
                                                                                  new[] {
                                                                                      new Image(
                                                                                          URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                                                                          ImageFileType.jpeg,
                                                                                          ImageCategory.LOCATION,
                                                                                          200,
                                                                                          400,
                                                                                          URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                                                                      )
                                                                                  },
                                                                                  new(
                                                                                      true,
                                                                                      new[] {
                                                                                          new EnergySource(
                                                                                              EnergySourceCategory.SOLAR,
                                                                                              80
                                                                                          ),
                                                                                          new EnergySource(
                                                                                              EnergySourceCategory.WIND,
                                                                                              20
                                                                                          )
                                                                                      },
                                                                                      new[] {
                                                                                          new EnvironmentalImpact(
                                                                                              EnvironmentalImpactCategory.CARBON_DIOXIDE,
                                                                                              0.1
                                                                                          )
                                                                                      },
                                                                                      "Stadtwerke Jena-Ost",
                                                                                      "New Green Deal"
                                                                                  ),
                                                                                  DateTime.Parse("2020-09-21T00:00:00Z")
                                                                              ));

                var response4            = await graphDefinedEMSP.PutEVSE(new EVSE(
                                                                              EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                                                              StatusType.AVAILABLE,
                                                                              new[] {
                                                                                  new Connector(
                                                                                      Connector_Id.Parse("1"),
                                                                                      ConnectorType.IEC_62196_T2,
                                                                                      ConnectorFormats.SOCKET,
                                                                                      PowerTypes.AC_3_PHASE,
                                                                                      Volt.  ParseV(400),
                                                                                      Ampere.ParseA(30),
                                                                                      Watt.  ParseW(12),
                                                                                      new[] {
                                                                                          Tariff_Id.Parse("DE*GEF*T0001"),
                                                                                          Tariff_Id.Parse("DE*GEF*T0002")
                                                                                      },
                                                                                      URL.Parse("https://open.charging.cloud/terms"),
                                                                                      DateTime.Parse("2020-09-21")
                                                                                  ),
                                                                                  new Connector(
                                                                                      Connector_Id.Parse("2"),
                                                                                      ConnectorType.IEC_62196_T2_COMBO,
                                                                                      ConnectorFormats.CABLE,
                                                                                      PowerTypes.AC_3_PHASE,
                                                                                      Volt.  ParseV(400),
                                                                                      Ampere.ParseA(20),
                                                                                      Watt.  ParseW(8),
                                                                                      new[] {
                                                                                          Tariff_Id.Parse("DE*GEF*T0003"),
                                                                                          Tariff_Id.Parse("DE*GEF*T0004")
                                                                                      },
                                                                                      URL.Parse("https://open.charging.cloud/terms"),
                                                                                      DateTime.Parse("2020-09-22")
                                                                                  )
                                                                              },
                                                                              EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                                                              new[] {
                                                                                  new StatusSchedule(
                                                                                      StatusType.INOPERATIVE,
                                                                                      DateTime.Parse("2020-09-23"),
                                                                                      DateTime.Parse("2020-09-24")
                                                                                  ),
                                                                                  new StatusSchedule(
                                                                                      StatusType.OUTOFORDER,
                                                                                      DateTime.Parse("2020-12-30"),
                                                                                      DateTime.Parse("2020-12-31")
                                                                                  )
                                                                              },
                                                                              new[] {
                                                                                  Capability.RFID_READER,
                                                                                  Capability.RESERVABLE
                                                                              },

                                                                              // OCPI Computer Science Extensions
                                                                              new EnergyMeter(
                                                                                  Meter_Id.Parse("Meter0815"),
                                                                                  "EnergyMeter Model #1",
                                                                                  null,
                                                                                  "hw. v1.80",
                                                                                  "fw. v1.20",
                                                                                  "Energy Metering Services",
                                                                                  null,
                                                                                  null,
                                                                                  null
                                                                              ),

                                                                              "1. Stock",
                                                                              GeoCoordinate.Parse(10.1, 20.2),
                                                                              "Ladestation #1",
                                                                              new[] {
                                                                                  DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                                                                  DisplayText.Create(Languages.en, "Ken sent me!")
                                                                              },
                                                                              new[] {
                                                                                  ParkingRestriction.EV_ONLY,
                                                                                  ParkingRestriction.PLUGGED
                                                                              },
                                                                              new[] {
                                                                                  new Image(
                                                                                      URL.Parse("http://example.com/pinguine.jpg"),
                                                                                      ImageFileType.jpeg,
                                                                                      ImageCategory.OPERATOR,
                                                                                      100,
                                                                                      150,
                                                                                      URL.Parse("http://example.com/kleine_pinguine.jpg")
                                                                                  )
                                                                              },
                                                                              DateTime.Parse("2020-09-22")
                                                                          ),
                                                                          CountryCode.Parse("DE"),
                                                                          Party_Id.   Parse("GEF"),
                                                                          Location_Id.Parse("LOC0001"));

                // HTTP/1.1 201 Created
                // Date:                          Wed, 28 Dec 2022 22:54:40 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, PUT, PATCH, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Last-Modified:                 2020-09-21T00:00:00.000Z
                // ETag:                          Q8R8UfdvHYd/jE3j+FjO0r84MiRmqt/8+PXxZ+Z43sg=
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                4932
                // X-Request-ID:                  G62h4r32Yd9rrQS579jWGpf49rS2Wz
                // X-Correlation-ID:              3xE3Y25Atv8633zYz3U6Qr945457pt
                // 
                // {
                //     "data":            {"id":"LOC0001","location_type":"PARKING_LOT","name":"Location 0001","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"related_locations":[{"latitude":"11","longitude":"22","name":{"language":"de","text":"Postkasten"}}],"evses":[{"uid":"DE*GEF*E*LOC0001*1","evse_id":"DE*GEF*E*LOC0001*1","status":"AVAILABLE","status_schedule":[{"period_begin":"2020-09-22T22:00:00.000Z","period_end":"2020-09-23T22:00:00.000Z","status":"INOPERATIVE"},{"period_begin":"2020-12-29T23:00:00.000Z","period_end":"2020-12-30T23:00:00.000Z","status":"OUTOFORDER"}],"capabilities":["RFID_READER","RESERVABLE"],"connectors":[{"id":"1","standard":"IEC_62196_T2","format":"SOCKET","power_type":"AC_3_PHASE","voltage":400,"amperage":30,"terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-20T22:00:00.000Z"},{"id":"2","standard":"IEC_62196_T2_COMBO","format":"CABLE","power_type":"AC_3_PHASE","voltage":400,"amperage":20,"terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-21T22:00:00.000Z"}],"energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","vendor":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}]},"floor_level":"1. Stock","coordinates":{"latitude":"10.10000","longitude":"20.20000"},"physical_reference":"Ladestation #1","directions":[{"language":"de","text":"Bitte klingeln!"},{"language":"en","text":"Ken sent me!"}],"parking_restrictions":["EV_ONLY","PLUGGED"],"images":[{"url":"http://example.com/pinguine.jpg","category":"OPERATOR","type":"jpeg","thumbnail":"http://example.com/kleine_pinguine.jpg","width":100,"height":150}],"last_updated":"2020-09-21T22:00:00.000Z"}],"directions":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"operator":{"name":"Open Charging Cloud","website":"https://open.charging.cloud","logo":{"url":"http://open.charging.cloud/logo.svg","category":"OPERATOR","type":"svg","thumbnail":"http://open.charging.cloud/logo_small.svg","width":1000,"height":1500}},"suboperator":{"name":"GraphDefined GmbH","website":"https://www.graphdefined.com","logo":{"url":"http://www.graphdefined.com/logo.png","category":"OPERATOR","type":"png","thumbnail":"http://www.graphdefined.com/logo_small.png","width":2000,"height":3000}},"owner":{"name":"Achim Friedland","website":"https://ahzf.de","logo":{"url":"http://ahzf.de/logo.gif","category":"OWNER","type":"gif","thumbnail":"http://ahzf.de/logo_small.gif","width":3000,"height":4500}},"facilities":["CAFE"],"time_zone":"Europe/Berlin","opening_times":{"twentyfourseven":false,"regular_hours":[{"weekday":1,"period_begin":"08:00","period_end":"15:00"},{"weekday":2,"period_begin":"09:00","period_end":"16:00"},{"weekday":3,"period_begin":"10:00","period_end":"17:00"},{"weekday":4,"period_begin":"11:00","period_end":"18:00"},{"weekday":5,"period_begin":"12:00","period_end":"19:00"}],"exceptional_openings":[{"period_begin":"2020-09-21T00:00:00.000Z","period_end":"2020-09-22T00:00:00.000Z"}],"exceptional_closings":[{"period_begin":"2020-12-24T00:00:00.000Z","period_end":"2020-12-26T00:00:00.000Z"}]},"charging_when_closed":false,"images":[{"url":"http://open.charging.cloud/locations/location0001.jpg","category":"LOCATION","type":"jpeg","thumbnail":"http://open.charging.cloud/locations/location0001s.jpg","width":200,"height":400}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T00:00:00.000Z"},
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-28T22:54:40.784Z"
                // }

                ClassicAssert.IsNotNull(response4);
                ClassicAssert.AreEqual (201,             response4.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,            response4.StatusCode);
                ClassicAssert.AreEqual ("Hello world!",  response4.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now -  response4.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion



        #region CPO_PostCDR_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_PostCDR_Test()
        {

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1            = await graphDefinedEMSP.GetVersions();
                var response2            = await graphDefinedEMSP.Register();
                var response3            = await graphDefinedEMSP.PostCDR(new CDR(
                                                                              CountryCode.Parse("DE"),
                                                                              Party_Id.   Parse("GEF"),
                                                                              CDR_Id.     Parse("CDR0001"),
                                                                              DateTime.   Parse("2020-04-12T18:20:19Z").ToUniversalTime(),
                                                                              DateTime.   Parse("2020-04-12T22:20:19Z").ToUniversalTime(),
                                                                              new CDRToken(
                                                                                  CountryCode.Parse("DE"),
                                                                                  Party_Id.   Parse("GEF"),
                                                                                  Token_Id.   Parse("1234"),
                                                                                  TokenType. RFID,
                                                                                  Contract_Id.Parse("C1234")
                                                                              ),
                                                                              AuthMethods.AUTH_REQUEST,
                                                                              new CDRLocation(
                                                                                  Location_Id.     Parse("LOC0001"),
                                                                                  "Biberweg 18",
                                                                                  "Jena",
                                                                                  Country.Germany,
                                                                                  GeoCoordinate.   Parse(10, 20),
                                                                                  EVSE_UId.        Parse("DE*GEF*E*LOC0001*1"),
                                                                                  EVSE_Id.         Parse("DE*GEF*E*LOC0001*1"),
                                                                                  Connector_Id.    Parse("1"),
                                                                                  ConnectorType.   IEC_62196_T2,
                                                                                  ConnectorFormats.SOCKET,
                                                                                  PowerTypes.      AC_3_PHASE,
                                                                                  "Name?",
                                                                                  "07749"
                                                                              ),
                                                                              OCPI.Currency.EUR,

                                                                              new ChargingPeriod[] {
                                                                                  ChargingPeriod.Create(
                                                                                      DateTime.Parse("2020-04-12T18:21:49Z").ToUniversalTime(),
                                                                                      new[] {
                                                                                          CDRDimension.Create(
                                                                                              CDRDimensionType.ENERGY,
                                                                                              1.33M
                                                                                          )
                                                                                      },
                                                                                      Tariff_Id.Parse("DE*GEF*T0001")
                                                                                  ),
                                                                                  ChargingPeriod.Create(
                                                                                      DateTime.Parse("2020-04-12T18:21:50Z").ToUniversalTime(),
                                                                                      new[] {
                                                                                          CDRDimension.Create(
                                                                                              CDRDimensionType.TIME,
                                                                                              5.12M
                                                                                          )
                                                                                      },
                                                                                      Tariff_Id.Parse("DE*GEF*T0002")
                                                                                  )
                                                                              },

                                                                              // Total costs
                                                                              new Price(
                                                                                  10.00m,
                                                                                  [ new TaxAmount("VAT", 11.60m) ]
                                                                              ),

                                                                              // Total Energy
                                                                              WattHour.ParseKWh(50.00M),

                                                                              // Total time
                                                                              TimeSpan.              FromMinutes(30),

                                                                              Session_Id.            Parse("0815"),
                                                                              AuthorizationReference.Parse("Auth0815"),
                                                                              Meter_Id.              Parse("Meter0815"),

                                                                              // OCPI Computer Science Extensions
                                                                              new EnergyMeter(
                                                                                  Meter_Id.Parse("Meter0815"),
                                                                                  "EnergyMeter Model #1",
                                                                                  null,
                                                                                  "hw. v1.80",
                                                                                  "fw. v1.20",
                                                                                  "Energy Metering Services",
                                                                                  null,
                                                                                  null,
                                                                                  null,
                                                                                  new[] {
                                                                                      new TransparencySoftwareStatus(
                                                                                          new TransparencySoftware(
                                                                                              "Chargy Transparency Software Desktop Application",
                                                                                              "v1.00",
                                                                                              SoftwareLicense.AGPL3,
                                                                                              "GraphDefined GmbH",
                                                                                              URL.Parse("https://open.charging.cloud/logo.svg"),
                                                                                              URL.Parse("https://open.charging.cloud/Chargy/howto"),
                                                                                              URL.Parse("https://open.charging.cloud/Chargy"),
                                                                                              URL.Parse("https://github.com/OpenChargingCloud/ChargyDesktopApp")
                                                                                          ),
                                                                                          LegalStatus.GermanCalibrationLaw,
                                                                                          "cert",
                                                                                          "German PTB",
                                                                                          NotBefore: DateTime.Parse("2019-04-01T00:00:00.000Z").ToUniversalTime(),
                                                                                          NotAfter:  DateTime.Parse("2030-01-01T00:00:00.000Z").ToUniversalTime()
                                                                                      ),
                                                                                      new TransparencySoftwareStatus(
                                                                                          new TransparencySoftware(
                                                                                              "Chargy Transparency Software Mobile Application",
                                                                                              "v1.00",
                                                                                              SoftwareLicense.AGPL3,
                                                                                              "GraphDefined GmbH",
                                                                                              URL.Parse("https://open.charging.cloud/logo.svg"),
                                                                                              URL.Parse("https://open.charging.cloud/Chargy/howto"),
                                                                                              URL.Parse("https://open.charging.cloud/Chargy"),
                                                                                              URL.Parse("https://github.com/OpenChargingCloud/ChargyMobileApp")
                                                                                          ),
                                                                                          LegalStatus.ForInformationOnly,
                                                                                          "no cert",
                                                                                          "GraphDefined",
                                                                                          NotBefore: DateTime.Parse("2019-04-01T00:00:00.000Z").ToUniversalTime(),
                                                                                          NotAfter:  DateTime.Parse("2030-01-01T00:00:00.000Z").ToUniversalTime()
                                                                                      )
                                                                                  }
                                                                              ),
                                                                              null,

                                                                              new[] {
                                                                                  new Tariff(
                                                                                      CountryCode.Parse("DE"),
                                                                                      Party_Id.   Parse("GEF"),
                                                                                      Tariff_Id.  Parse("TARIFF0001"),
                                                                                      OCPI.Currency.EUR,
                                                                                      new[] {
                                                                                          new TariffElement(
                                                                                              new[] {
                                                                                                  PriceComponent.ChargingTime(
                                                                                                      2.00M,
                                                                                                      0.10M,
                                                                                                      TimeSpan.FromSeconds(300)
                                                                                                  )
                                                                                              },
                                                                                              new TariffRestrictions(
                                                                                                  Time.FromHourMin(08,00),       // Start time
                                                                                                  Time.FromHourMin(18,00),       // End time
                                                                                                  DateTime.Parse("2020-12-01"),  // Start timestamp
                                                                                                  DateTime.Parse("2020-12-31"),  // End timestamp
                                                                                                  1.12M,                         // MinkWh
                                                                                                  5.67M,                         // MaxkWh
                                                                                                  1.34M,                         // MinCurrent
                                                                                                  8.89M,                         // MaxCurrent
                                                                                                  1.49M,                         // MinPower
                                                                                                  9.91M,                         // MaxPower
                                                                                                  TimeSpan.FromMinutes(10),      // MinDuration
                                                                                                  TimeSpan.FromMinutes(30),      // MaxDuration
                                                                                                  new DayOfWeek[] {
                                                                                                      DayOfWeek.Monday,
                                                                                                      DayOfWeek.Tuesday
                                                                                                  },
                                                                                                  ReservationRestrictions.RESERVATION
                                                                                              )
                                                                                          )
                                                                                      },
                                                                                      TaxIncluded.Yes,
                                                                                      TariffType.PROFILE_GREEN,
                                                                                      new[] {
                                                                                          new DisplayText(Languages.de, "Hallo Welt!"),
                                                                                          new DisplayText(Languages.en, "Hello world!"),
                                                                                      },
                                                                                      URL.Parse("https://open.charging.cloud"),
                                                                                      new PriceLimit( // Min Price
                                                                                          1.10m,
                                                                                          1.26m
                                                                                      ),
                                                                                      new PriceLimit( // Max Price
                                                                                          2.20m,
                                                                                          2.52m
                                                                                      ),
                                                                                      DateTime.Parse("2020-12-01").ToUniversalTime(), // Start timestamp
                                                                                      DateTime.Parse("2020-12-31").ToUniversalTime(), // End timestamp
                                                                                      new EnergyMix(
                                                                                          true,
                                                                                          new[] {
                                                                                              new EnergySource(
                                                                                                  EnergySourceCategory.SOLAR,
                                                                                                  80
                                                                                              ),
                                                                                              new EnergySource(
                                                                                                  EnergySourceCategory.WIND,
                                                                                                  20
                                                                                              )
                                                                                          },
                                                                                          new[] {
                                                                                              new EnvironmentalImpact(
                                                                                                  EnvironmentalImpactCategory.CARBON_DIOXIDE,
                                                                                                  0.1
                                                                                              )
                                                                                          },
                                                                                          "Stadtwerke Jena-Ost",
                                                                                          "New Green Deal"
                                                                                      ),
                                                                                      DateTime.Parse("2020-09-22").ToUniversalTime()
                                                                                  )
                                                                              },

                                                                              new SignedData(
                                                                                  EncodingMethod.GraphDefined,
                                                                                  new[] {
                                                                                      new SignedValue(
                                                                                          SignedValueNature.START,
                                                                                          "PlainStartValue",
                                                                                          "SignedStartValue"
                                                                                      ),
                                                                                      new SignedValue(
                                                                                          SignedValueNature.INTERMEDIATE,
                                                                                          "PlainIntermediateValue",
                                                                                          "SignedIntermediateValue"
                                                                                      ),
                                                                                      new SignedValue(
                                                                                          SignedValueNature.END,
                                                                                          "PlainEndValue",
                                                                                          "SignedEndValue"
                                                                                      )
                                                                                  },
                                                                                  1,     // Encoding method version
                                                                                  null,  // Public key
                                                                                  URL.Parse("https://open.charging.cloud/pools/1/stations/1/evse/1/publicKey")
                                                                              ),

                                                                              // Total Fixed Costs
                                                                              new Price(
                                                                                  20.00m,
                                                                                  [new TaxAmount("VAT", 23.10m)]
                                                                              ),

                                                                              // Total Energy Cost
                                                                              new Price(
                                                                                  20.00m,
                                                                                  [new TaxAmount("VAT", 23.10m)]
                                                                              ),

                                                                              // Total Time Cost
                                                                              new Price(
                                                                                  20.00m,
                                                                                  [new TaxAmount("VAT", 23.10m)]
                                                                              ),

                                                                              // Total Parking Time
                                                                              TimeSpan.FromMinutes(120),

                                                                              // Total Parking Cost
                                                                              new Price(
                                                                                  20.00m,
                                                                                  [new TaxAmount("VAT", 23.10m)]
                                                                              ),

                                                                              // Total Reservation Cost
                                                                              new Price(
                                                                                  20.00m,
                                                                                  [new TaxAmount("VAT", 23.10m)]
                                                                              ),

                                                                              "Remark!",
                                                                              InvoiceReference_Id.Parse("Invoice:0815"),
                                                                              true, // IsCredit
                                                                              CreditReference_Id.Parse("Credit:0815"),
                                                                              false,

                                                                              DateTime.Parse("2020-09-12").ToUniversalTime()

                                                                          ));

                // HTTP/1.1 201 Created
                // Date:                          Wed, 28 Dec 2022 23:20:07 GMT
                // Location:                      /2.2/emsp/cdrs/CDR0001
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Last-Modified:                 2020-09-11T22:00:00.000Z
                // ETag:                          ckAWY1kW3wi8MR04zps+WfXAFDVTrsYlHSnRn0nPJU8=
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                3436
                // X-Request-ID:                  dt88p12Mn313hQxf3t5UKpCjMKY37d
                // X-Correlation-ID:              2284ht8G82pE4Sxpn4KUj2t5tAS13j
                // 
                // {"data":{"id":"CDR0001","start_date_time":"2020-04-12T18:20:19.000Z","end_date_time":"2020-04-12T22:20:19.000Z","auth_id":"1234","auth_method":"AUTH_REQUEST","location":{"id":"LOC0001","location_type":"UNDERGROUND_GARAGE","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"time_zone":null,"last_updated":"2022-12-28T23:19:49.740Z"},"meter_id":"Meter0815","energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","vendor":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}]},"signed_data":{"encoding_method":"GraphDefined","encoding_method_version":"1","signed_values":[{"nature":"START","plain_data":"PlainStartValue","signed_data":"SignedStartValue"},{"nature":"INTERMEDIATE","plain_data":"PlainIntermediateValue","signed_data":"SignedIntermediateValue"},{"nature":"END","plain_data":"PlainEndValue","signed_data":"SignedEndValue"}],"url":"https://open.charging.cloud/pools/1/stations/1/evse/1/publicKey"},"currency":"EUR","tariffs":[{"id":"TARIFF0001","currency":"EUR","tariff_alt_text":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"tariff_alt_url":"https://open.charging.cloud","elements":[{"price_components":[{"type":"TIME","price":2.0,"step_size":300}],"restrictions":[{"start_time":"08:00","end_time":"18:00","start_date":"2020-12-01","end_date":"2020-12-31","min_kwh":1.12,"max_kwh":5.67,"min_power":1.49,"max_power":9.91,"min_duration":600.0,"max_duration":1800.0,"day_of_week":["MONDAY","TUESDAY"]}]}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T22:00:00.000Z"}],"charging_periods":[{"start_date_time":"2020-04-12T18:21:49.000Z","dimensions":[{"type":"ENERGY","volume":1.33}]},{"start_date_time":"2020-04-12T18:21:50.000Z","dimensions":[{"type":"TIME","volume":5.12}]}],"total_cost":10.0,"total_energy":50.0,"total_time":0.5,"total_parking_time":2.0,"remark":"Remark!","last_updated":"2020-09-11T22:00:00.000Z"},"status_code":1000,"status_message":"Hello world!","timestamp":"2022-12-28T23:20:07.358Z"}

                ClassicAssert.IsNotNull(response3);
                ClassicAssert.AreEqual (201,             response3.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,            response3.StatusCode);
                ClassicAssert.AreEqual ("Hello world!",  response3.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now -  response3.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion



    }

}
