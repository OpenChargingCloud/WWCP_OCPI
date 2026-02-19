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

using Newtonsoft.Json.Linq;

using NUnit.Framework;
using NUnit.Framework.Legacy;

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

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
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
                //         "version":  "2.3.0",
                //         "url":      "http://127.0.0.1:7235/versions/2.3.0"
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
                ClassicAssert.AreEqual (Version_Id.Parse("2.3.0"),     version.Id);
                ClassicAssert.AreEqual (emsp1VersionsAPIURL.Value + "2.3.0",  version.URL);

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

                var result1 = await cpoCommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Role.EMSP);
                ClassicAssert.IsTrue(result1);

                var result2 = await cpoCommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    [
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Role.       EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             ],
                                        LocalAccessInfos:    [
                                                                 new LocalAccessInfo(
                                                                     AccessToken.Parse("aaaaaa"),
                                                                     AccessStatus.ALLOWED
                                                                 )
                                                             ],
                                        RemoteAccessInfos:   [
                                                                 new RemoteAccessInfo(
                                                                     AccessToken:        AccessToken.Parse("bbbbbb"),
                                                                     VersionsURL:        emsp1VersionsAPIURL.Value,
                                                                     VersionIds:         [ Version_Id.Parse("2.3.0") ],
                                                                     SelectedVersionId:  Version_Id.Parse("2.3.0"),
                                                                     Status:             RemoteAccessStatus.ONLINE
                                                                 )
                                                             ],
                                        Status:              PartyStatus.ENABLED
                                    );

                ClassicAssert.IsTrue(result2.IsSuccess);

            }

            #endregion

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
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
                //         "version":  "2.3.0",
                //         "url":      "http://127.0.0.1:7235/versions/2.3.0"
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
                ClassicAssert.AreEqual (Version_Id.Parse("2.3.0"),    version.Id);
                ClassicAssert.AreEqual (emsp1VersionsAPIURL.Value + "2.3.0", version.URL);

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

            await cpoCommonAPI.  RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Role.EMSP);
            await emsp1CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"), Role.CPO);

            var addEMSPResult = await cpoCommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    [
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Role.       EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             ],
                                        LocalAccessInfos:    [
                                                                 new LocalAccessInfo(
                                                                     AccessToken.Parse("xxxxxx"),
                                                                     AccessStatus.ALLOWED
                                                                 )
                                                             ],
                                        RemoteAccessInfos:   [
                                                                 new RemoteAccessInfo(
                                                                     AccessToken:        AccessToken.Parse("yyyyyy"),
                                                                     VersionsURL:        emsp1VersionsAPIURL.Value,
                                                                     VersionIds:         [ Version_Id.Parse("2.3.0") ],
                                                                     SelectedVersionId:  Version_Id.Parse("2.3.0"),
                                                                     Status:             RemoteAccessStatus.ONLINE
                                                                 )
                                                             ],
                                        Status:              PartyStatus.ENABLED
                                    );

            ClassicAssert.IsTrue(addEMSPResult.IsSuccess);


            var addCPOResult = await emsp1CommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GEF_CPO"),
                                        CredentialsRoles:    [
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GEF"),
                                                                     Role:                Role.       CPO,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined CPO Services")
                                                                 )
                                                             ],
                                        LocalAccessInfos:    [
                                                                 new LocalAccessInfo(
                                                                     AccessToken.Parse("yyyyyy"),
                                                                     AccessStatus.BLOCKED
                                                                 )
                                                             ],
                                        RemoteAccessInfos:   [
                                                                 new RemoteAccessInfo(
                                                                     AccessToken:        AccessToken.Parse("xxxxxx"),
                                                                     VersionsURL:        emsp1VersionsAPIURL.Value,
                                                                     VersionIds:         [ Version_Id.Parse("2.3.0") ],
                                                                     SelectedVersionId:  Version_Id.Parse("2.3.0"),
                                                                     Status:             RemoteAccessStatus.ONLINE
                                                                 )
                                                             ],
                                        Status:              PartyStatus.ENABLED
                                    );

            ClassicAssert.IsTrue(addCPOResult.IsSuccess);

            #endregion

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
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

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetVersionDetails(Version_Id.Parse("2.3.0"));

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
                //ClassicAssert.AreEqual(Version_Id.Parse("2.3.0"), endpoints.Id);
                //ClassicAssert.AreEqual(emspVersionsAPIURL + "2.3.0", endpoints.URL);


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

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetCredentials();

                // GET /2.3.0/credentials HTTP/1.1
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
                ClassicAssert.AreEqual("yyyyyy",                             credentials.Token.                            ToString());
                ClassicAssert.AreEqual("http://127.0.0.1:7235/versions",     credentials.URL.                              ToString());
                ClassicAssert.AreEqual("DE",                                 credentials.Roles.First().PartyId.CountryCode.ToString());
                ClassicAssert.AreEqual("GDF",                                credentials.Roles.First().PartyId.PartyId.      ToString());

                var businessDetails = credentials.Roles.First().BusinessDetails;
                ClassicAssert.IsNotNull(businessDetails);
                ClassicAssert.AreEqual("GraphDefined EMSP Services",         businessDetails.Name);
                ClassicAssert.AreEqual("https://www.graphdefined.com/emsp",  businessDetails.Website.                      ToString());

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

            await cpoCommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Role.EMSP);

            var result = await cpoCommonAPI.AddRemoteParty(
                Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    [
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Role.       EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             ],
                LocalAccessInfos:    [
                                         new LocalAccessInfo(
                                             AccessToken.Parse("aaaaaa"),
                                             AccessStatus.ALLOWED
                                         )
                                     ],
                RemoteAccessInfos:   [
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("bbbbbb"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         [ Version_Id.Parse("2.3.0") ],
                                             SelectedVersionId:  Version_Id.Parse("2.3.0"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     ],
                Status:              PartyStatus.ENABLED
            );

            #endregion

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
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
                ClassicAssert.AreEqual("<any>",                              credentials.Token.                            ToString());
                ClassicAssert.AreEqual("http://127.0.0.1:7235/versions",     credentials.URL.                              ToString());
                ClassicAssert.AreEqual("DE",                                 credentials.Roles.First().PartyId.CountryCode.ToString());
                ClassicAssert.AreEqual("GDF",                                credentials.Roles.First().PartyId.PartyId.      ToString());

                var businessDetails = credentials.Roles.First().BusinessDetails;
                ClassicAssert.IsNotNull(businessDetails);
                ClassicAssert.AreEqual("GraphDefined EMSP Services",         businessDetails.Name);
                ClassicAssert.AreEqual("https://www.graphdefined.com/emsp",  businessDetails.Website.                      ToString());

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

            await cpoCommonAPI.  RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Role.EMSP);
            await emsp1CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"), Role.CPO);

            var addEMSPResult = await cpoCommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    [
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Role.       EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             ],
                LocalAccessInfos:    [
                                         new LocalAccessInfo(
                                             AccessToken.Parse("xxxxxx"),
                                             AccessStatus.ALLOWED
                                         )
                                     ],
                RemoteAccessInfos:   [
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("yyyyyy"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         [ Version_Id.Parse("2.3.0") ],
                                             SelectedVersionId:  Version_Id.Parse("2.3.0"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     ],
                Status:              PartyStatus.ENABLED
            );

            ClassicAssert.IsTrue(addEMSPResult.IsSuccess);


            var addCPOResult = await emsp1CommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GEF_CPO"),
                                        CredentialsRoles:    [
                                                                 new CredentialsRole(
                                                                     CountryCode:      CountryCode.Parse("DE"),
                                                                     PartyId:          Party_Id.   Parse("GEF"),
                                                                     Role:             Role.       CPO,
                                                                     BusinessDetails:  new BusinessDetails("GraphDefined CPO Services")
                                                                 )
                                                             ],
                LocalAccessInfos:    [
                                         new LocalAccessInfo(
                                             AccessToken.Parse("yyyyyy"),
                                             AccessStatus.BLOCKED
                                         )
                                     ],
                RemoteAccessInfos:   [
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("xxxxxx"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         [ Version_Id.Parse("2.3.0") ],
                                             SelectedVersionId:  Version_Id.Parse("2.3.0"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     ],
                Status:              PartyStatus.ENABLED
            );

            ClassicAssert.IsTrue(addCPOResult.IsSuccess);

            #endregion

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
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

            await cpoCommonAPI.  RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Role.EMSP);
            await emsp1CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"), Role.CPO);

            var addEMSPResult = await cpoCommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    [
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Role.       EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             ],
                LocalAccessInfos:    [
                                         new LocalAccessInfo(
                                             AccessToken.Parse("xxxxxx"),
                                             AccessStatus.ALLOWED
                                         )
                                     ],
                RemoteAccessInfos:   [
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("yyyyyy"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         [ Version_Id.Parse("2.3.0") ],
                                             SelectedVersionId:  Version_Id.Parse("2.3.0"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     ],
                Status:              PartyStatus.ENABLED
            );

            ClassicAssert.IsTrue(addEMSPResult.IsSuccess);


            var addCPOResult = await emsp1CommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GEF_CPO"),
                                        CredentialsRoles:    [
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GEF"),
                                                                     Role:                Role.       CPO,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined CPO Services")
                                                                 )
                                                             ],
                LocalAccessInfos:    [
                                         new LocalAccessInfo(
                                             AccessToken.Parse("yyyyyy"),
                                             AccessStatus.BLOCKED
                                         )
                                     ],
                RemoteAccessInfos:   [
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("xxxxxx"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         [ Version_Id.Parse("2.3.0") ],
                                             SelectedVersionId:  Version_Id.Parse("2.3.0"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     ],
                Status:              PartyStatus.ENABLED
            );

            ClassicAssert.IsTrue(addCPOResult.IsSuccess);

            #endregion

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetCredentials(Version_Id.Parse("2.3.0"));

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

            await cpoCommonAPI.  RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Role.EMSP);
            await emsp1CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"), Role.CPO);

            var addEMSPResult = await cpoCommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    [
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Role.       EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             ],
                LocalAccessInfos:    [
                                         new LocalAccessInfo(
                                             AccessToken.Parse("xxxxxx"),
                                             AccessStatus.ALLOWED
                                         )
                                     ],
                RemoteAccessInfos:   [
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("yyyyyy"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         [ Version_Id.Parse("2.3.0") ],
                                             SelectedVersionId:  Version_Id.Parse("2.3.0"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     ],
                Status:              PartyStatus.ENABLED
            );

            ClassicAssert.IsTrue(addEMSPResult.IsSuccess);


            var addCPOResult = await emsp1CommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GEF_CPO"),
                                        CredentialsRoles:    [
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GEF"),
                                                                     Role:                Role.       CPO,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined CPO Services")
                                                                 )
                                                             ],
                LocalAccessInfos:    [
                                         new LocalAccessInfo(
                                             AccessToken.Parse("yyyyyy"),
                                             AccessStatus.BLOCKED
                                         )
                                     ],
                RemoteAccessInfos:   [
                                         new RemoteAccessInfo(
                                             AccessToken:        AccessToken.Parse("xxxxxx"),
                                             VersionsURL:        emsp1VersionsAPIURL.Value,
                                             VersionIds:         [ Version_Id.Parse("2.3.0") ],
                                             SelectedVersionId:  Version_Id.Parse("2.3.0"),
                                             Status:             RemoteAccessStatus.ONLINE
                                         )
                                     ],
                Status:              PartyStatus.ENABLED
            );

            ClassicAssert.IsTrue(addCPOResult.IsSuccess);

            #endregion

            var httpResponse = await TestHelpers.JSONRequest(URL.Parse("http://127.0.0.1:7235/2.3.0/credentials"),
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

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
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
                                                               [
                                                                   new CredentialsRole(
                                                                       CountryCode.Parse("DE"),
                                                                       Party_Id.   Parse("EXP"),
                                                                       Role.       CPO,
                                                                       new BusinessDetails(
                                                                           "Example Org",
                                                                           URL.Parse("http://example.org")
                                                                       )
                                                                   )
                                                               ]
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

                var result1 = await cpoCommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Role.EMSP);

                ClassicAssert.IsTrue(result1);


                var result2 = await cpoCommonAPI.AddRemoteParty(
                                        Id:                  RemoteParty_Id.Parse("DE-GDF_EMSP"),
                                        CredentialsRoles:    [
                                                                 new CredentialsRole(
                                                                     CountryCode:         CountryCode.Parse("DE"),
                                                                     PartyId:             Party_Id.   Parse("GDF"),
                                                                     Role:                Role.       EMSP,
                                                                     BusinessDetails:     new BusinessDetails("GraphDefined EMSP Services")
                                                                 )
                                                             ],
                                        LocalAccessInfos:    [
                                                                 new LocalAccessInfo(
                                                                     AccessToken.Parse("aaaaaa"),
                                                                     AccessStatus.ALLOWED
                                                                 )
                                                             ],
                                        RemoteAccessInfos:   [
                                                                 new RemoteAccessInfo(
                                                                     AccessToken:        AccessToken.Parse("bbbbbb"),
                                                                     VersionsURL:        emsp1VersionsAPIURL.Value,
                                                                     VersionIds:         [ Version_Id.Parse("2.3.0") ],
                                                                     SelectedVersionId:  Version_Id.Parse("2.3.0"),
                                                                     Status:             RemoteAccessStatus.ONLINE
                                                                 )
                                                             ],
                                        Status:              PartyStatus.ENABLED
                                    );

                ClassicAssert.IsTrue(result2.IsSuccess);

            }

            #endregion

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
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
                                                               [
                                                                   new CredentialsRole(
                                                                       CountryCode.Parse("DE"),
                                                                       Party_Id.   Parse("EXP"),
                                                                       Role.       CPO,
                                                                       new BusinessDetails(
                                                                           "Example Org",
                                                                           URL.Parse("http://example.org")
                                                                       )
                                                                   )
                                                               ]
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

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
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


    }

}
