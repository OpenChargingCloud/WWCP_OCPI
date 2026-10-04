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

            Assert.That(graphDefinedEMSP, Is.Not.Null);

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

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response.StatusCode.Value,                                      Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10), Is.True);

                //ClassicAssert.IsNotNull(response.Request);

                var versions = response.Data;
                Assert.That(versions,              Is.Not.Null);
                Assert.That(response.Data.Count(), Is.EqualTo(1));

                var version = versions.First();
                Assert.That(version.Id,  Is.EqualTo(Version_Id.Parse("2.3.0")));
                Assert.That(version.URL.ToString(), Does.EndWith($":{emsp1HTTPServer!.TCPPort}/ocpi/versions/{Version.Id}"));

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

            Assert.That(cpoCommonAPI,        Is.Not.Null);
            Assert.That(emsp1VersionsAPIURL, Is.Not.Null);

            if (cpoCommonAPI is not null &&
                emsp1VersionsAPIURL.HasValue)
            {

                var result1 = await cpoCommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"), Role.EMSP);
                Assert.That(result1, Is.True);

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

                Assert.That(result2.IsSuccess, Is.True);

            }

            #endregion

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.That(graphDefinedEMSP, Is.Not.Null);

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

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response.StatusCode.Value,                                      Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10), Is.True);

                //ClassicAssert.IsNotNull(response.Request);

                var versions = response.Data;
                Assert.That(versions,              Is.Not.Null);
                Assert.That(response.Data.Count(), Is.EqualTo(1));

                var version = versions.First();
                Assert.That(version.Id,  Is.EqualTo(Version_Id.Parse("2.3.0")));
                Assert.That(version.URL.ToString(), Does.EndWith($":{emsp1HTTPServer!.TCPPort}/ocpi/versions/{Version.Id}"));

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

            Assert.That(addEMSPResult.IsSuccess, Is.True);


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

            Assert.That(addCPOResult.IsSuccess, Is.True);

            // The versions list asks the token list of the base API, where a
            // token is blocked as such - as GetVersions_BlockedTokens_Tests do.
            await emsp1CommonAPI.AddAccessToken(AccessToken.Parse("yyyyyy"), AccessStatus.BLOCKED);

            #endregion

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.That(graphDefinedEMSP, Is.Not.Null);

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

                Assert.That(response,                                                      Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                    Is.EqualTo(200));  // as GetVersions_BlockedTokens_Tests have it - a 401 would be better, if OCPI allows it
                Assert.That(response.StatusCode.Value,                                     Is.EqualTo(2000));
                Assert.That(response.StatusMessage,                                        Is.EqualTo("Invalid or blocked access token!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10), Is.True);

                //ClassicAssert.IsNotNull(response.Request);

                var versions = response.Data;
                Assert.That(versions,         Is.Not.Null);
                Assert.That(versions.Count(), Is.EqualTo(0));

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

            Assert.That(graphDefinedEMSP, Is.Not.Null);

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

                Assert.That(response2,                                                       Is.Not.Null);
                Assert.That(response2.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response2.StatusCode.Value,                                      Is.EqualTo(1000));
                Assert.That(response2.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10), Is.True);

                //ClassicAssert.IsNotNull(response.Request);

                var versionDetail = response2.Data;
                Assert.That(versionDetail, Is.Not.Null);

                var endpoints = versionDetail.Endpoints;
                Assert.That(endpoints.Count(), Is.EqualTo(8));
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

            Assert.That(graphDefinedEMSP, Is.Not.Null);

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

                Assert.That(response2,                                                       Is.Not.Null);
                Assert.That(response2.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response2.StatusCode.Value,                                      Is.EqualTo(1000));
                Assert.That(response2.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10), Is.True);

                //ClassicAssert.IsNotNull(response.Request);

                var credentials = response2.Data;
                Assert.That(credentials,                                                Is.Not.Null);
                Assert.That(credentials.Token.                            ToString(),   Is.EqualTo("emp1-2-cso:token"));
                Assert.That(credentials.URL.                              ToString(),   Is.EqualTo(emsp1VersionsAPIURL!.Value.ToString()));
                Assert.That(credentials.Roles.First().PartyId.CountryCode.ToString(),   Is.EqualTo("DE"));
                Assert.That(credentials.Roles.First().PartyId.PartyId.      ToString(), Is.EqualTo("GDF"));

                var businessDetails = credentials.Roles.First().BusinessDetails;
                Assert.That(businessDetails,                                          Is.Not.Null);
                Assert.That(businessDetails.Name,                                     Is.EqualTo("GraphDefined EMSP #1 Services"));
                Assert.That(businessDetails.Website.                      ToString(), Is.EqualTo("https://www.graphdefined.com/emsp1"));

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

            Assert.That(graphDefinedEMSP, Is.Not.Null);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetCredentials();

                // A token EMSP #1 does not know: its credentials are not given
                // to anybody it does not know.
                Assert.That(response2,                                                       Is.Not.Null);
                Assert.That(response2.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(401));
                Assert.That(response2.StatusCode.Value,                                      Is.EqualTo(2000));
                Assert.That(response2.StatusMessage,                                         Is.EqualTo("Unknown access token!"));
                Assert.That(response2.Data,                                                  Is.Null);

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

            Assert.That(addEMSPResult.IsSuccess, Is.True);


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

            Assert.That(addCPOResult.IsSuccess, Is.True);

            #endregion

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.That(graphDefinedEMSP, Is.Not.Null);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetCredentials();

                Assert.That(response2,                                                      Is.Not.Null);
                Assert.That(response2.HTTPResponse,                                         Is.Null);
                Assert.That(response2.StatusCode.Value,                                     Is.EqualTo(-1));
                Assert.That(response2.StatusMessage,                                        Is.EqualTo("No remote URL available!"));
                Assert.That(Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10), Is.True);

                //ClassicAssert.IsNotNull(response.Request);

                var credentials = response2.Data;
                Assert.That(credentials, Is.Null);
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

            Assert.That(addEMSPResult.IsSuccess, Is.True);


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

            Assert.That(addCPOResult.IsSuccess, Is.True);

            #endregion

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.That(graphDefinedEMSP, Is.Not.Null);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetCredentials(Version_Id.Parse("2.3.0"));

                Assert.That(response2,                                                      Is.Not.Null);
                Assert.That(response2.HTTPResponse,                                         Is.Null);
                Assert.That(response2.StatusCode.Value,                                     Is.EqualTo(-1));
                Assert.That(response2.StatusMessage,                                        Is.EqualTo("No remote URL available!"));
                Assert.That(Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10), Is.True);

                //ClassicAssert.IsNotNull(response.Request);

                var credentials = response2.Data;
                Assert.That(credentials, Is.Null);

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

            Assert.That(addEMSPResult.IsSuccess, Is.True);


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

            Assert.That(addCPOResult.IsSuccess, Is.True);

            #endregion

            var httpResponse = await TestHelpers.JSONRequest(CredentialsURL(emsp1CommonAPI!, emsp1HTTPServer!),
                                                             "eXl5eXl5");  // "yyyyyy", BASE64-encoded as the local access keeps it

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

            Assert.That(httpResponse,                     Is.Not.Null);
            Assert.That(httpResponse.HTTPStatusCode.Code, Is.EqualTo(403));

            var jsonResponse = JObject.Parse(httpResponse.HTTPBodyAsUTF8String);
            Assert.That(jsonResponse, Is.Not.Null);

            Assert.That(jsonResponse["status_code"]?.   Value<Int32>(),                                          Is.EqualTo(2000));
            Assert.That(jsonResponse["status_message"]?.Value<String>(),                                         Is.EqualTo("Invalid or blocked access token!"));
            Assert.That(Timestamp.Now - jsonResponse["timestamp"]?.Value<DateTime>() < TimeSpan.FromSeconds(10), Is.True);

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

            // Not yet registered: EMSP #1 knows the CPO by its token, and has not
            // been told where the CPO's versions are.
            await emsp1CommonAPI!.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"), Role.CPO);

            var notRegistered = await emsp1CommonAPI.AddRemoteParty(
                                          Id:                RemoteParty_Id.Parse("DE-GEF_CPO"),
                                          CredentialsRoles:  [
                                                                 new CredentialsRole(
                                                                     CountryCode:      CountryCode.Parse("DE"),
                                                                     PartyId:          Party_Id.   Parse("GEF"),
                                                                     Role:             Role.       CPO,
                                                                     BusinessDetails:  new BusinessDetails("GraphDefined CSO Services")
                                                                 )
                                                             ],
                                          LocalAccessInfos:  [
                                                                 new LocalAccessInfo(
                                                                     AccessToken.Parse("emp1-2-cso:token"),
                                                                     AccessStatus.ALLOWED
                                                                 )
                                                             ],
                                          RemoteAccessInfos: [],
                                          Status:            PartyStatus.ENABLED
                                      );

            Assert.That(notRegistered.IsSuccess, Is.True);

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.That(graphDefinedEMSP, Is.Not.Null);

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

                Assert.That(response2,                                                      Is.Not.Null);
                Assert.That(response2.HTTPResponse?.HTTPStatusCode.Code,                    Is.EqualTo(405));
                Assert.That(response2.StatusCode.Value,                                     Is.EqualTo(2000));
                Assert.That(response2.StatusMessage,                                        Is.EqualTo("The given access token 'emp1-2-cso:token' is not yet registered!"));
                Assert.That(Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10), Is.True);

                //ClassicAssert.IsNotNull(response.Request);

                Assert.That(response2.Data, Is.Null);

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

                Assert.That(result1, Is.True);


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

                Assert.That(result2.IsSuccess, Is.True);

            }

            #endregion

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.That(graphDefinedEMSP, Is.Not.Null);

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

                Assert.That(response2,                                                      Is.Not.Null);
                Assert.That(response2.HTTPResponse?.HTTPStatusCode.Code,                    Is.EqualTo(405));
                Assert.That(response2.StatusCode.Value,                                     Is.EqualTo(2000));
                Assert.That(response2.StatusMessage,                                        Is.EqualTo("You need to be registered before trying to invoke this protected method!"));
                Assert.That(Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10), Is.True);

                //ClassicAssert.IsNotNull(response.Request);

                Assert.That(response2.Data, Is.Null);

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

            // Not yet registered: EMSP #1 knows the CPO by its token, and has not
            // been told where the CPO's versions are. A registration is what
            // tells it - and what gives both sides new tokens.
            await emsp1CommonAPI!.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"), Role.CPO);

            var notRegistered = await emsp1CommonAPI.AddRemoteParty(
                                          Id:                RemoteParty_Id.Parse("DE-GEF_CPO"),
                                          CredentialsRoles:  [
                                                                 new CredentialsRole(
                                                                     CountryCode:      CountryCode.Parse("DE"),
                                                                     PartyId:          Party_Id.   Parse("GEF"),
                                                                     Role:             Role.       CPO,
                                                                     BusinessDetails:  new BusinessDetails("GraphDefined CSO Services")
                                                                 )
                                                             ],
                                          LocalAccessInfos:  [
                                                                 new LocalAccessInfo(
                                                                     AccessToken.Parse("emp1-2-cso:token"),
                                                                     AccessStatus.ALLOWED
                                                                 )
                                                             ],
                                          RemoteAccessInfos: [],
                                          Status:            PartyStatus.ENABLED
                                      );

            Assert.That(notRegistered.IsSuccess, Is.True);

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.That(graphDefinedEMSP, Is.Not.Null);

            if (graphDefinedEMSP is not null)
            {

                var remoteAccessInfoOld  = cpoCommonAPI?.GetRemoteParty(RemoteParty_Id.Parse("DE-GDF_EMSP"))?.RemoteAccessInfos.First();
                Assert.That(remoteAccessInfoOld,                         Is.Not.Null);

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

                Assert.That(response2,                                                       Is.Not.Null);
                Assert.That(response2.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response2.StatusCode.Value,                                      Is.EqualTo(1000));
                Assert.That(response2.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10), Is.True);

                //ClassicAssert.IsNotNull(response.Request);

                var remoteAccessInfoNew  = cpoCommonAPI?.GetRemoteParty(RemoteParty_Id.Parse("DE-GDF_EMSP"))?.RemoteAccessInfos.First();
                Assert.That(remoteAccessInfoNew,                         Is.Not.Null);
                Assert.That(remoteAccessInfoNew?.AccessToken.ToString(), Is.Not.EqualTo(remoteAccessInfoOld?.AccessToken.ToString()));

                // EMSP #1 now knows where the CPO's versions are.
                var cpoAtEMSP1           = emsp1CommonAPI?.GetRemoteParty(RemoteParty_Id.Parse("DE-GEF_CPO"));
                Assert.That(cpoAtEMSP1?.RemoteAccessInfos.Count(),       Is.EqualTo(1));

            }

        }

        #endregion

        #region CPO_Register_AlreadyRegistered_Test()

        /// <summary>
        /// A party that is registered already registers once more: OCPI wants
        /// a 405 for that - a registered party renews its credentials by PUT.
        /// </summary>
        [Test]
        public async Task CPO_Register_AlreadyRegistered_Test()
        {

            var graphDefinedEMSP = cpoCPOAPI?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.That(graphDefinedEMSP, Is.Not.Null);

            if (graphDefinedEMSP is not null)
            {

                var remoteAccessInfoOld  = cpoCommonAPI?.GetRemoteParty(RemoteParty_Id.Parse("DE-GDF_EMSP"))?.RemoteAccessInfos.First();
                Assert.That(remoteAccessInfoOld,                         Is.Not.Null);

                var response             = await graphDefinedEMSP.Register();

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(405));
                Assert.That(response.StatusCode.Value,                                      Is.EqualTo(2000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("The given access token 'emp1-2-cso:token' is already registered!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10), Is.True);

                // Refused: the CPO still calls EMSP #1 with the token it had.
                var remoteAccessInfoNew  = cpoCommonAPI?.GetRemoteParty(RemoteParty_Id.Parse("DE-GDF_EMSP"))?.RemoteAccessInfos.First();
                Assert.That(remoteAccessInfoNew?.AccessToken.ToString(), Is.EqualTo(remoteAccessInfoOld?.AccessToken.ToString()));

            }

        }

        #endregion


    }

}
