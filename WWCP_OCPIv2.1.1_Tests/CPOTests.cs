/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPIv2_1_1.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests
{

    [TestFixture]
    public class CPOTests : ANodeTests
    {

        #region CPO_GetVersions_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_Test()
        {

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

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
                //         "version":  "2.1.1",
                //         "url":      "http://127.0.0.1:7135/versions/2.1.1"
                //     }],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-25T23:16:31.228Z"
                // }

                Assert.IsNotNull(response);
                Assert.AreEqual(1000, response.StatusCode);
                Assert.AreEqual("Hello world!", response.StatusMessage);
                Assert.IsTrue(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var versions = response.Data;
                Assert.IsNotNull(versions);
                Assert.AreEqual(1, response.Data.Count());

                var version = versions.First();
                Assert.AreEqual(Version_Id.Parse("2.1.1"),    version.Id);
                Assert.AreEqual(emspVersionsAPIURL + "2.1.1", version.URL);

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

            cpoWebAPI.CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"));

            var result = cpoWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GDF"),
                Role:               Roles.      EMSP,
                BusinessDetails:    new BusinessDetails("GraphDefined EMSP Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("aaaaaa"),
                                            AccessStatus.ALLOWED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("bbbbbb"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            #endregion

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

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
                //         "version":  "2.1.1",
                //         "url":      "http://127.0.0.1:7135/versions/2.1.1"
                //     }],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-25T23:16:31.228Z"
                // }

                Assert.IsNotNull(response);
                Assert.AreEqual(1000, response.StatusCode);
                Assert.AreEqual("Hello world!", response.StatusMessage);
                Assert.IsTrue(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var versions = response.Data;
                Assert.IsNotNull(versions);
                Assert.AreEqual(1, response.Data.Count());

                var version = versions.First();
                Assert.AreEqual(Version_Id.Parse("2.1.1"),    version.Id);
                Assert.AreEqual(emspVersionsAPIURL + "2.1.1", version.URL);

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

            cpoWebAPI. CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"));
            emspWebAPI.CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));

            var addEMSPResult = cpoWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GDF"),
                Role:               Roles.      EMSP,
                BusinessDetails:    new BusinessDetails("GraphDefined EMSP Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("xxxxxx"),
                                            AccessStatus.ALLOWED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("yyyyyy"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            Assert.IsTrue(addEMSPResult);


            var addCPOResult = emspWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GEF"),
                Role:               Roles.      CPO,
                BusinessDetails:    new BusinessDetails("GraphDefined CPO Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("yyyyyy"),
                                            AccessStatus.BLOCKED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("xxxxxx"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            Assert.IsTrue(addCPOResult);

            #endregion

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

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

                Assert.IsNotNull(response);
                Assert.AreEqual(403,                                 response.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual(2000,                                response.StatusCode);
                Assert.AreEqual("Invalid or blocked access token!",  response.StatusMessage);
                Assert.IsTrue  (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var versions = response.Data;
                Assert.IsNotNull(versions);
                Assert.AreEqual(0, versions.Count());

            }

        }

        #endregion


        #region CPO_GetVersion_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetVersion_Test()
        {

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetVersionDetails(Version_Id.Parse("2.1.1"));

                // HTTP/1.1 200 OK
                // Content-Type:   application/json;charset=utf-8
                // Content-Length: 224
                // Connection:     keep-alive
                // Date:           Wed, 14 Dec 2022 18:59:05 GMT
                // 
                // {
                //     "data": [{
                //         "version":  "2.1.1",
                //         "url":      "http://127.0.0.1:7135/versions/2.1.1"
                //     }],
                //     "timestamp":    "2022-12-14T18:59:05Z",
                //     "status_code":   1000
                // }

                Assert.IsNotNull(response2);
                Assert.AreEqual(1000, response2.StatusCode);
                Assert.AreEqual("Hello world!", response2.StatusMessage);
                Assert.IsTrue(Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);


                var versionDetail = response2.Data;
                Assert.IsNotNull(versionDetail);

                var endpoints = versionDetail.Endpoints;
                Assert.AreEqual(7, endpoints.Count());
                //Assert.AreEqual(Version_Id.Parse("2.1.1"), endpoints.Id);
                //Assert.AreEqual(emspVersionsAPIURL + "2.1.1", endpoints.URL);


            }

        }

        #endregion


    }

}
