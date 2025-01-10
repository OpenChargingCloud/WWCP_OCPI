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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPI.UnitTests
{

    /// <summary>
    /// Testing the OCPI GetVersionDetails method(s).
    /// </summary>
    [TestFixture]
    public class GetVersionDetails_Tests : A_2CPOs2EMSPs_TestDefaults
    {

        #region CPO1_GetVersionDetails_v2_1_1_fromEMSP1_viaOCPIv2_1_1__RegisteredToken_Test1()

        /// <summary>
        /// CPO #1 asks EMSP #1 for its OCPI versions via OCPI v2.1.1!
        /// </summary>
        [Test]
        public async Task CPO1_GetVersionDetails_v2_1_1_fromEMSP1_viaOCPIv2_1_1__RegisteredToken_Test1()
        {

            var graphDefinedEMSP1 = cpo1CommonAPI_v2_1_1?.GetCPOClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersionDetails(OCPIv2_1_1.Version.Id);

                // GET /ocpi/versions HTTP/1.1
                // Accept:                          application/json; charset=utf-8; q=1
                // Host:                            localhost:3401
                // User-Agent:                      GraphDefined OCPI v2.1.1 CommonClient
                // Authorization:                   Token cpo1_accessing_emsp1++token
                // Connection:                      close
                // X-Request-ID:                    v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:                Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj

                // HTTP/1.1 200 OK
                // Date:                            Wed, 08 Jan 2025 03:26:44 GMT
                // Server:                          GraphDefined OCPI HTTP API v0.1
                // Access-Control-Allow-Origin:     *
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Allow:                           OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  267
                // Connection:                      close
                // Vary:                            Accept
                // X-Request-ID:                    v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:                Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj
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
                //             "version": "2.3",
                //             "url": "http://localhost:3401/ocpi/versions/2.3"
                //         }
                //     ]
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                //var versions = response.Data?.OrderBy(version => version.Id).ToArray();
                //Assert.That(versions,                                                       Is.Not.Null);
                //Assert.That(response.Data?.Count(),                                         Is.EqualTo(3));

                //var version2_1_1 = versions?.ElementAt(0);
                //Assert.That(version2_1_1?.Id == OCPIv2_1_1.Version.Id, Is.True);
                //Assert.That(URL.Parse("http://localhost:3401/ocpi/versions/2.1.1") == version2_1_1?.URL, Is.True);

                //var version2_2_1 = versions?.ElementAt(1);
                //Assert.That(version2_2_1?.Id == OCPIv2_2_1.Version.Id, Is.True);
                //Assert.That(URL.Parse("http://localhost:3401/ocpi/versions/2.2.1") == version2_2_1?.URL, Is.True);

                //var version2_3   = versions?.ElementAt(2);
                //Assert.That(version2_3?.  Id == OCPIv2_3.Version.Id, Is.True);
                //Assert.That(URL.Parse("http://localhost:3401/ocpi/versions/2.3")   == version2_3?.  URL, Is.True);


                //Assert.That(response.Request, Is.Not.Null);


               
                var response3 = await graphDefinedEMSP1.GetVersionDetails(OCPIv2_2_1.Version.Id);
                var response4 = await graphDefinedEMSP1.GetVersionDetails(OCPIv2_3.Version.Id);

            }

        }

        #endregion

        #region CPO1_GetVersionDetails_v2_1_1_fromEMSP1_viaOCPIv2_2_1__RegisteredToken_Test1()

        /// <summary>
        /// CPO #1 asks EMSP #1 for its OCPI versions via OCPI v2.2.1!
        /// </summary>
        [Test]
        public async Task CPO1_GetVersionDetails_v2_1_1_fromEMSP1_viaOCPIv2_2_1__RegisteredToken_Test1()
        {

            var graphDefinedEMSP1 = cpo1CommonAPI_v2_2_1?.GetCPOClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersionDetails(OCPIv2_1_1.Version.Id);

                // GET /ocpi/versions HTTP/1.1
                // Accept:                          application/json; charset=utf-8; q=1
                // Host:                            localhost:3401
                // User-Agent:                      GraphDefined OCPI v2.1.1 CommonClient
                // Authorization:                   Token cpo1_accessing_emsp1++token
                // Connection:                      close
                // X-Request-ID:                    v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:                Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj

                // HTTP/1.1 200 OK
                // Date:                            Wed, 08 Jan 2025 03:26:44 GMT
                // Server:                          GraphDefined OCPI HTTP API v0.1
                // Access-Control-Allow-Origin:     *
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Allow:                           OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  267
                // Connection:                      close
                // Vary:                            Accept
                // X-Request-ID:                    v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:                Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj
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
                //             "version": "2.3",
                //             "url": "http://localhost:3401/ocpi/versions/2.3"
                //         }
                //     ]
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                //var versions = response.Data?.OrderBy(version => version.Id).ToArray();
                //Assert.That(versions,                                                       Is.Not.Null);
                //Assert.That(response.Data?.Count(),                                         Is.EqualTo(3));

                //var version2_1_1 = versions?.ElementAt(0);
                //Assert.That(version2_1_1?.Id == OCPIv2_1_1.Version.Id, Is.True);
                //Assert.That(URL.Parse("http://localhost:3401/ocpi/versions/2.1.1") == version2_1_1?.URL, Is.True);

                //var version2_2_1 = versions?.ElementAt(1);
                //Assert.That(version2_2_1?.Id == OCPIv2_2_1.Version.Id, Is.True);
                //Assert.That(URL.Parse("http://localhost:3401/ocpi/versions/2.2.1") == version2_2_1?.URL, Is.True);

                //var version2_3   = versions?.ElementAt(2);
                //Assert.That(version2_3?.  Id == OCPIv2_3.Version.Id, Is.True);
                //Assert.That(URL.Parse("http://localhost:3401/ocpi/versions/2.3")   == version2_3?.  URL, Is.True);


                //Assert.That(response.Request, Is.Not.Null);


                //var response2 = await graphDefinedEMSP1.GetVersionDetails(OCPIv2_1_1.Version.Id);
                //var response3 = await graphDefinedEMSP1.GetVersionDetails(OCPIv2_2_1.Version.Id);
                //var response4 = await graphDefinedEMSP1.GetVersionDetails(OCPIv2_3.Version.Id);

            }

        }

        #endregion



        #region EMSP1_GetVersionDetails_v2_1_1_fromCPO1_viaOCPIv2_1_1__RegisteredToken_Test1()

        /// <summary>
        /// EMSP #1 asks CPO #1 for its OCPI v2.1.1 version details via OCPI v2.1.1!
        /// </summary>
        [Test]
        public async Task EMSP1_GetVersionDetails_v2_1_1_fromCPO1_viaOCPIv2_1_1__RegisteredToken_Test1()
        {

            var graphDefinedCPO1 = emsp1CommonAPI_v2_1_1?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersionDetails(OCPIv2_1_1.Version.Id);

                // GET /ocpi/versions/2.1.1 HTTP/1.1
                // Accept:                          application/json; charset=utf-8; q=1
                // Host:                            localhost:3301
                // User-Agent:                      GraphDefined OCPI v2.1.1 CommonClient
                // Authorization:                   Token emsp1_accessing_cpo1++token
                // Connection:                      close
                // X-Request-ID:                    1tEnWAzn2CME5dSr673Q7UM2v6152n
                // X-Correlation-ID:                t3bh9f2bUbzjGzGCQjx3SjU3d1G8xd

                // HTTP/1.1 200 OK
                // Date:                            Wed, 08 Jan 2025 12:35:51 GMT
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Allow:                           OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Vary:                            Accept
                // Server:                          GraphDefined HTTP API
                // Access-Control-Allow-Origin:     *
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  689
                // X-Request-ID:                    1tEnWAzn2CME5dSr673Q7UM2v6152n
                // X-Correlation-ID:                t3bh9f2bUbzjGzGCQjx3SjU3d1G8xd
                // 
                // {
                //     "data": {
                //         "version": "2.1.1",
                //         "endpoints": [
                //             {
                //                 "identifier": "credentials",
                //                 "url": "http://localhost:3301/ocpi/v2.1.1/credentials"
                //             },
                //             {
                //                 "identifier": "locations",
                //                 "url": "http://localhost:3301/ocpi/v2.1.1/cpo/locations"
                //             },
                //             {
                //                 "identifier": "tariffs",
                //                 "url": "http://localhost:3301/ocpi/v2.1.1/cpo/tariffs"
                //             },
                //             {
                //                 "identifier": "sessions",
                //                 "url": "http://localhost:3301/ocpi/v2.1.1/cpo/sessions"
                //             },
                //             {
                //                 "identifier": "cdrs",
                //                 "url": "http://localhost:3301/ocpi/v2.1.1/cpo/cdrs"
                //             },
                //             {
                //                 "identifier": "commands",
                //                 "url": "http://localhost:3301/ocpi/v2.1.1/cpo/commands"
                //             },
                //             {
                //                 "identifier": "tokens",
                //                 "url": "http://localhost:3301/ocpi/v2.1.1/cpo/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "timestamp":      "2025-01-08T12:35:51.859Z"
                // }


                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {

                    Assert.That(response.Data.VersionId,                                        Is.EqualTo(OCPIv2_1_1.Version.Id));

                    var endpoints = response.Data.Endpoints.ToDictionary(endpoint => endpoint.Identifier);

                    Assert.That(endpoints,                                                      Is.Not.Null);
                    Assert.That(endpoints.Count,                                                Is.EqualTo(7));

                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Credentials].URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.1.1/credentials")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Locations].  URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.1.1/cpo/locations")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Tariffs].    URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.1.1/cpo/tariffs")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Sessions].   URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.1.1/cpo/sessions")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.CDRs].       URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.1.1/cpo/cdrs")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Commands].   URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.1.1/cpo/commands")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Tokens].     URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.1.1/cpo/tokens")));

                }


                //Assert.That(response.Request, Is.Not.Null);


            }

        }

        #endregion

        #region EMSP1_GetVersionDetails_v2_1_1_fromCPO1_viaOCPIv2_2_1__RegisteredToken_Test1()

        /// <summary>
        /// EMSP #1 asks CPO #1 for its OCPI v2.1.1 version details via OCPI v2.2.1!
        /// </summary>
        [Test]
        public async Task EMSP1_GetVersionDetails_v2_1_1_fromCPO1_viaOCPIv2_2_1__RegisteredToken_Test1()
        {

            var graphDefinedCPO1 = emsp1CommonAPI_v2_2_1?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersionDetails(OCPIv2_1_1.Version.Id);

                // GET /ocpi/versions HTTP/1.1
                // Accept:                          application/json; charset=utf-8; q=1
                // Host:                            localhost:3301
                // User-Agent:                      GraphDefined OCPI v2.1.1 CommonClient
                // Authorization:                   Token cpo1_accessing_emsp1++token
                // Connection:                      close
                // X-Request-ID:                    v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:                Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj

                // HTTP/1.1 200 OK
                // Date:                            Wed, 08 Jan 2025 03:26:44 GMT
                // Server:                          GraphDefined OCPI HTTP API v0.1
                // Access-Control-Allow-Origin:     *
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Allow:                           OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  267
                // Connection:                      close
                // Vary:                            Accept
                // X-Request-ID:                    v42pQKn544dEbSE94nh4hhSCjWjWn6
                // X-Correlation-ID:                Mx3G3355W6bQKM98M5W5Gz9SdUG5Cj
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
                //             "version": "2.3",
                //             "url": "http://localhost:3301/ocpi/versions/2.3"
                //         }
                //     ]
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {

                    Assert.That(response.Data.VersionId,                                        Is.EqualTo(OCPIv2_1_1.Version.Id));

                    var endpoints = response.Data.Endpoints.ToDictionary(endpoint => endpoint.Identifier);

                    Assert.That(endpoints,                                                      Is.Not.Null);
                    Assert.That(endpoints.Count,                                                Is.EqualTo(7));

                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Credentials].URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/credentials")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Locations].  URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/locations")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tariffs].    URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/tariffs")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Sessions].   URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/sessions")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.CDRs].       URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/cdrs")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Commands].   URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/commands")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tokens].     URL,                    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/tokens")));

                }


                //Assert.That(response.Request, Is.Not.Null);


            }

        }

        #endregion



    }

}
