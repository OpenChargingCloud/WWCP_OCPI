/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI.UnitTests
{

    /// <summary>
    /// Testing the OCPI GetVersionDetails method(s) using registered OCPI tokens.
    /// </summary>
    [TestFixture]
    public class GetVersions_RegisteredTokens_ClientCertificates_Tests : A_2CPOs2EMSPs_TestDefaults
    {

        #region Constructor(s)

        public GetVersions_RegisteredTokens_ClientCertificates_Tests()

              // This will enable client certificates for TLS authentication!
            : base(ECCAlgorithm: "secp256r1")

        {

        }

        #endregion


        #region CPO1_GetVersions_fromEMSP1_viaOCPIv2_1_1__RegisteredToken_Test1()

        /// <summary>
        /// CPO #1 asking EMSP #1 for its OCPI versions via OCPI v2.1.1!
        /// </summary>
        [Test]
        public async Task CPO1_GetVersions_fromEMSP1_viaOCPIv2_1_1__RegisteredToken_Test1()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_1_1?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3401
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Authorization:                 Token cpo1_accessing_emsp1++token
                // Connection:                    close
                // X-Request-ID:                  v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:              Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj

                // HTTP/1.1 200 OK
                // Date:                          Wed, 08 Jan 2025 03:26:44 GMT
                // Server:                        GraphDefined OCPI v2.1.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                271
                // X-Request-ID:                  v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:              Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj
                // 
                // {
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "data": [
                //         {
                //             "version": "2.1.1",
                //             "url": "http://localhost:3401/ocpi/versions/2.1.1"
                //         },
                //         {
                //             "version": "2.2.1",
                //             "url": "http://localhost:3401/ocpi/versions/2.2.1"
                //         },
                //         {
                //             "version": "2.3.0",
                //             "url": "http://localhost:3401/ocpi/versions/2.3.0"
                //         }
                //     ]
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200), response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                var versions = response.Data?.OrderBy(version => version.Id).ToArray();
                Assert.That(versions,                                                       Is.Not.Null);
                Assert.That(response.Data?.Count(),                                         Is.EqualTo(3));

                var version2_1_1 = versions?.ElementAt(0);
                Assert.That(version2_1_1?.Id == OCPIv2_1_1.Version.Id, Is.True);
                Assert.That(version2_1_1?.URL.ToString(), Is.EqualTo("http://localhost:3401/ocpi/versions/2.1.1"));

                var version2_2_1 = versions?.ElementAt(1);
                Assert.That(version2_2_1?.Id == OCPIv2_2_1.Version.Id, Is.True);
                Assert.That(version2_2_1?.URL.ToString(), Is.EqualTo("http://localhost:3401/ocpi/versions/2.2.1"));

                var version2_3_0 = versions?.ElementAt(2);
                Assert.That(version2_3_0?.Id == OCPIv2_3_0.Version.Id, Is.True);
                Assert.That(version2_3_0?.URL.ToString(), Is.EqualTo("http://localhost:3401/ocpi/versions/2.3.0"));


                //Assert.That(response.Request, Is.Not.Null);


            }

        }

        #endregion

        #region CPO1_GetVersions_fromEMSP1_viaOCPIv2_2_1__RegisteredToken_Test1()

        /// <summary>
        /// CPO #1 asking EMSP #1 for its OCPI versions via OCPI v2.2.1!
        /// </summary>
        [Test]
        public async Task CPO1_GetVersions_fromEMSP1_viaOCPIv2_2_1__RegisteredToken_Test1()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_2_1?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3401
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Authorization:                 Token cpo1_accessing_emsp1++token
                // Connection:                    close
                // X-Request-ID:                  v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:              Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj

                // HTTP/1.1 200 OK
                // Date:                          Wed, 08 Jan 2025 03:26:44 GMT
                // Server:                        GraphDefined OCPI v2.2.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                271
                // X-Request-ID:                  v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:              Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj
                // 
                // {
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "data": [
                //         {
                //             "version": "2.1.1",
                //             "url": "http://localhost:3401/ocpi/versions/2.1.1"
                //         },
                //         {
                //             "version": "2.2.1",
                //             "url": "http://localhost:3401/ocpi/versions/2.2.1"
                //         },
                //         {
                //             "version": "2.3.0",
                //             "url": "http://localhost:3401/ocpi/versions/2.3.0"
                //         }
                //     ]
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200), response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                var versions = response.Data?.OrderBy(version => version.Id).ToArray();
                Assert.That(versions,                                                       Is.Not.Null);
                Assert.That(response.Data?.Count(),                                         Is.EqualTo(3));

                var version2_1_1 = versions?.ElementAt(0);
                Assert.That(version2_1_1?.Id == OCPIv2_1_1.Version.Id, Is.True);
                Assert.That(version2_1_1?.URL.ToString(), Is.EqualTo("http://localhost:3401/ocpi/versions/2.1.1"));

                var version2_2_1 = versions?.ElementAt(1);
                Assert.That(version2_2_1?.Id == OCPIv2_2_1.Version.Id, Is.True);
                Assert.That(version2_2_1?.URL.ToString(), Is.EqualTo("http://localhost:3401/ocpi/versions/2.2.1"));

                var version2_3_0 = versions?.ElementAt(2);
                Assert.That(version2_3_0?.Id == OCPIv2_3_0.Version.Id, Is.True);
                Assert.That(version2_3_0?.URL.ToString(), Is.EqualTo("http://localhost:3401/ocpi/versions/2.3.0"));


                //Assert.That(response.Request, Is.Not.Null);


            }

        }

        #endregion

        #region CPO1_GetVersions_fromEMSP1_viaOCPIv2_3_0__RegisteredToken_Test1()

        /// <summary>
        /// CPO #1 asking EMSP #1 for its OCPI versions via OCPI v2.3!
        /// </summary>
        [Test]
        public async Task CPO1_GetVersions_fromEMSP1_viaOCPIv2_3_0__RegisteredToken_Test1()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_3_0?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3401
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Authorization:                 Token cpo1_accessing_emsp1++token
                // Connection:                    close
                // X-Request-ID:                  v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:              Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj

                // HTTP/1.1 200 OK
                // Date:                          Wed, 08 Jan 2025 03:26:44 GMT
                // Server:                        GraphDefined OCPI v2.3.0 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                271
                // X-Request-ID:                  v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:              Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj
                // 
                // {
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "data": [
                //         {
                //             "version": "2.1.1",
                //             "url": "http://localhost:3401/ocpi/versions/2.1.1"
                //         },
                //         {
                //             "version": "2.2.1",
                //             "url": "http://localhost:3401/ocpi/versions/2.2.1"
                //         },
                //         {
                //             "version": "2.3.0",
                //             "url": "http://localhost:3401/ocpi/versions/2.3.0"
                //         }
                //     ]
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200), response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                var versions = response.Data?.OrderBy(version => version.Id).ToArray();
                Assert.That(versions,                                                       Is.Not.Null);
                Assert.That(response.Data?.Count(),                                         Is.EqualTo(3));

                var version2_1_1 = versions?.ElementAt(0);
                Assert.That(version2_1_1?.Id == OCPIv2_1_1.Version.Id, Is.True);
                Assert.That(version2_1_1?.URL.ToString(), Is.EqualTo("http://localhost:3401/ocpi/versions/2.1.1"));

                var version2_2_1 = versions?.ElementAt(1);
                Assert.That(version2_2_1?.Id == OCPIv2_2_1.Version.Id, Is.True);
                Assert.That(version2_2_1?.URL.ToString(), Is.EqualTo("http://localhost:3401/ocpi/versions/2.2.1"));

                var version2_3_0 = versions?.ElementAt(2);
                Assert.That(version2_3_0?.Id == OCPIv2_3_0.Version.Id, Is.True);
                Assert.That(version2_3_0?.URL.ToString(), Is.EqualTo("http://localhost:3401/ocpi/versions/2.3.0"));


                //Assert.That(response.Request, Is.Not.Null);


            }

        }

        #endregion


        #region EMSP1_GetVersions_fromCPO1_viaOCPIv2_1_1__RegisteredToken_Test1()

        /// <summary>
        /// EMSP #1 asks CPO #1 for its OCPI versions via OCPI v2.1.1!
        /// </summary>
        [Test]
        public async Task EMSP1_GetVersions_fromCPO1_viaOCPIv2_1_1__RegisteredToken_Test1()
        {

            var graphDefinedCPO1 = emsp1EMSPAPI_v2_1_1?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3301
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Authorization:                 Token cpo1_accessing_emsp1++token
                // Connection:                    close
                // X-Request-ID:                  v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:              Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj

                // HTTP/1.1 200 OK
                // Date:                          Wed, 08 Jan 2025 03:26:44 GMT
                // Server:                        GraphDefined OCPI v2.1.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                271
                // X-Request-ID:                  v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:              Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj
                // 
                // {
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "data": [
                //         {
                //             "version": "2.1.1",
                //             "url": "http://localhost:3301/ocpi/versions/2.1.1"
                //         },
                //         {
                //             "version": "2.2.1",
                //             "url": "http://localhost:3301/ocpi/versions/2.2.1"
                //         },
                //         {
                //             "version": "2.3.0",
                //             "url": "http://localhost:3301/ocpi/versions/2.3.0"
                //         }
                //     ]
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200), response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                var versions = response.Data?.OrderBy(version => version.Id).ToArray();
                Assert.That(versions,                                                       Is.Not.Null);
                Assert.That(response.Data?.Count(),                                         Is.EqualTo(3));

                var version2_1_1 = versions?.ElementAt(0);
                Assert.That(version2_1_1?.Id == OCPIv2_1_1.Version.Id, Is.True);
                Assert.That(version2_1_1?.URL.ToString(), Is.EqualTo("http://localhost:3301/ocpi/versions/2.1.1"));

                var version2_2_1 = versions?.ElementAt(1);
                Assert.That(version2_2_1?.Id == OCPIv2_2_1.Version.Id, Is.True);
                Assert.That(version2_2_1?.URL.ToString(), Is.EqualTo("http://localhost:3301/ocpi/versions/2.2.1"));

                var version2_3_0 = versions?.ElementAt(2);
                Assert.That(version2_3_0?.Id == OCPIv2_3_0.Version.Id, Is.True);
                Assert.That(version2_3_0?.URL.ToString(), Is.EqualTo("http://localhost:3301/ocpi/versions/2.3.0"));

            }

        }

        #endregion

        #region EMSP1_GetVersions_fromCPO1_viaOCPIv2_2_1__RegisteredToken_Test1()

        /// <summary>
        /// EMSP #1 asks CPO #1 for its OCPI versions via OCPI v2.2.1!
        /// </summary>
        [Test]
        public async Task EMSP1_GetVersions_fromCPO1_viaOCPIv2_2_1__RegisteredToken_Test1()
        {

            var graphDefinedCPO1 = emsp1EMSPAPI_v2_2_1?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            //cpo1CPOAPI_v2_2_1.HTTPBaseAPI.HTTPBaseAPI.HTTPServer.ClientCertificateValidator = (Sender,
            //                                                                                   Certificate,
            //                                                                                   CertificateChain,
            //                                                                                   TLSServer,
            //                                                                                   PolicyErrors) => {

            //    return (true, []);

            //};


            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3301
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Authorization:                 Token cpo1_accessing_emsp1++token
                // Connection:                    close
                // X-Request-ID:                  v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:              Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj

                // HTTP/1.1 200 OK
                // Date:                          Wed, 08 Jan 2025 03:26:44 GMT
                // Server:                        GraphDefined OCPI v2.2.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                271
                // X-Request-ID:                  v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:              Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj
                // 
                // {
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "data": [
                //         {
                //             "version": "2.1.1",
                //             "url": "http://localhost:3301/ocpi/versions/2.1.1"
                //         },
                //         {
                //             "version": "2.2.1",
                //             "url": "http://localhost:3301/ocpi/versions/2.2.1"
                //         },
                //         {
                //             "version": "2.3.0",
                //             "url": "http://localhost:3301/ocpi/versions/2.3.0"
                //         }
                //     ]
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200), response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                var versions = response.Data?.OrderBy(version => version.Id).ToArray();
                Assert.That(versions,                                                       Is.Not.Null);
                Assert.That(response.Data?.Count(),                                         Is.EqualTo(3));

                var version2_1_1 = versions?.ElementAt(0);
                Assert.That(version2_1_1?.Id == OCPIv2_1_1.Version.Id, Is.True);
                Assert.That(version2_1_1?.URL.ToString(), Is.EqualTo("http://localhost:3301/ocpi/versions/2.1.1"));

                var version2_2_1 = versions?.ElementAt(1);
                Assert.That(version2_2_1?.Id == OCPIv2_2_1.Version.Id, Is.True);
                Assert.That(version2_2_1?.URL.ToString(), Is.EqualTo("http://localhost:3301/ocpi/versions/2.2.1"));

                var version2_3_0 = versions?.ElementAt(2);
                Assert.That(version2_3_0?.Id == OCPIv2_3_0.Version.Id, Is.True);
                Assert.That(version2_3_0?.URL.ToString(), Is.EqualTo("http://localhost:3301/ocpi/versions/2.3.0"));

            }

        }

        #endregion

        #region EMSP1_GetVersions_fromCPO1_viaOCPIv2_3_0__RegisteredToken_Test1()

        /// <summary>
        /// EMSP #1 asks CPO #1 for its OCPI versions via OCPI v2.3!
        /// </summary>
        [Test]
        public async Task EMSP1_GetVersions_fromCPO1_viaOCPIv2_3_0__RegisteredToken_Test1()
        {

            var graphDefinedCPO1 = emsp1EMSPAPI_v2_3_0?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3301
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Authorization:                 Token cpo1_accessing_emsp1++token
                // Connection:                    close
                // X-Request-ID:                  v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:              Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj

                // HTTP/1.1 200 OK
                // Date:                          Wed, 08 Jan 2025 03:26:44 GMT
                // Server:                        GraphDefined OCPI v2.3.0 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                271
                // X-Request-ID:                  v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:              Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj
                // 
                // {
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "data": [
                //         {
                //             "version": "2.1.1",
                //             "url": "http://localhost:3301/ocpi/versions/2.1.1"
                //         },
                //         {
                //             "version": "2.2.1",
                //             "url": "http://localhost:3301/ocpi/versions/2.2.1"
                //         },
                //         {
                //             "version": "2.3.0",
                //             "url": "http://localhost:3301/ocpi/versions/2.3.0"
                //         }
                //     ]
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200), response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                var versions = response.Data?.OrderBy(version => version.Id).ToArray();
                Assert.That(versions,                                                       Is.Not.Null);
                Assert.That(response.Data?.Count(),                                         Is.EqualTo(3));

                var version2_1_1 = versions?.ElementAt(0);
                Assert.That(version2_1_1?.Id == OCPIv2_1_1.Version.Id, Is.True);
                Assert.That(version2_1_1?.URL.ToString(), Is.EqualTo("http://localhost:3301/ocpi/versions/2.1.1"));

                var version2_2_1 = versions?.ElementAt(1);
                Assert.That(version2_2_1?.Id == OCPIv2_2_1.Version.Id, Is.True);
                Assert.That(version2_2_1?.URL.ToString(), Is.EqualTo("http://localhost:3301/ocpi/versions/2.2.1"));

                var version2_3_0 = versions?.ElementAt(2);
                Assert.That(version2_3_0?.Id == OCPIv2_3_0.Version.Id, Is.True);
                Assert.That(version2_3_0?.URL.ToString(), Is.EqualTo("http://localhost:3301/ocpi/versions/2.3.0"));

            }

        }

        #endregion


    }

}
