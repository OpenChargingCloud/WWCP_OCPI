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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

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

                // GET /ocpi/versions/2.1.1 HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3401
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Authorization:                 Token cpo1_accessing_emsp1++token
                // Connection:                    close
                // X-Request-ID:                  S1Q64pQ9nf24C3tA64xjhvWnz983jt
                // X-Correlation-ID:              945f2bEj63npK7fMSU48UdfnCt8722

                // HTTP/1.1 200 OK
                // Date:                          Fri, 10 Jan 2025 06:01:52 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Allow:                         OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Vary:                          Accept
                // Server:                        GraphDefined HTTP API
                // Access-Control-Allow-Origin:   *
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                695
                // X-Request-ID:                  S1Q64pQ9nf24C3tA64xjhvWnz983jt
                // X-Correlation-ID:              945f2bEj63npK7fMSU48UdfnCt8722
                // 
                // {
                //     "data": {
                //         "version": "2.1.1",
                //         "endpoints": [
                //             {
                //                 "identifier": "credentials",
                //                 "url":        "http://localhost:3401/ocpi/v2.1.1/credentials"
                //             },
                //             {
                //                 "identifier": "locations",
                //                 "url":        "http://localhost:3401/ocpi/v2.1.1/emsp/locations"
                //             },
                //             {
                //                 "identifier": "tariffs",
                //                 "url":        "http://localhost:3401/ocpi/v2.1.1/emsp/tariffs"
                //             },
                //             {
                //                 "identifier": "sessions",
                //                 "url":        "http://localhost:3401/ocpi/v2.1.1/emsp/sessions"
                //             },
                //             {
                //                 "identifier": "cdrs",
                //                 "url":        "http://localhost:3401/ocpi/v2.1.1/emsp/cdrs"
                //             },
                //             {
                //                 "identifier": "commands",
                //                 "url":        "http://localhost:3401/ocpi/v2.1.1/emsp/commands"
                //             },
                //             {
                //                 "identifier": "tokens",
                //                 "url":        "http://localhost:3401/ocpi/v2.1.1/emsp/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "timestamp":      "2025-01-10T06:01:52.727Z"
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

                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Credentials].URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/credentials")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Locations].  URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/locations")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Tariffs].    URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/tariffs")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Sessions].   URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/sessions")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.CDRs].       URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/cdrs")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Commands].   URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/commands")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Tokens].     URL,                Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.1.1/emsp/tokens")));

                }

            }

        }

        #endregion

        #region CPO1_GetVersionDetails_v2_2_1_fromEMSP1_viaOCPIv2_2_1__RegisteredToken_Test1()

        /// <summary>
        /// CPO #1 asks EMSP #1 for its OCPI versions via OCPI v2.2.1!
        /// </summary>
        [Test]
        public async Task CPO1_GetVersionDetails_v2_2_1_fromEMSP1_viaOCPIv2_2_1__RegisteredToken_Test1()
        {

            var graphDefinedEMSP1 = cpo1CommonAPI_v2_2_1?.GetCPOClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersionDetails(OCPIv2_2_1.Version.Id);

                // GET /ocpi/versions/2.2.1 HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3401
                // User-Agent:                    GraphDefined OCPI v2.2.1 Common Client
                // Authorization:                 Token Y3BvMV9hY2Nlc3NpbmdfZW1zcDErK3Rva2Vu
                // Connection:                    close
                // X-Request-ID:                  xQfW89475Sx3pMvUv56Eb6x1nd5b4M
                // X-Correlation-ID:              jt45vS3E9AK1r3p5EEb927UUnx51U8

                // HTTP/1.1 200 OK
                // Date:                          Fri, 10 Jan 2025 05:54:40 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Allow:                         OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Vary:                          Accept
                // Server:                        GraphDefined HTTP API
                // Access-Control-Allow-Origin:   *
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                1029
                // X-Request-ID:                  xQfW89475Sx3pMvUv56Eb6x1nd5b4M
                // X-Correlation-ID:              jt45vS3E9AK1r3p5EEb927UUnx51U8
                // 
                // {
                //     "data": {
                //         "version": "2.2.1",
                //         "endpoints": [
                //             {
                //                 "identifier": "credentials",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/credentials"
                //             },
                //             {
                //                 "identifier": "credentials",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/credentials"
                //             },
                //             {
                //                 "identifier": "locations",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/emsp/locations"
                //             },
                //             {
                //                 "identifier": "tariffs",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/emsp/tariffs"
                //             },
                //             {
                //                 "identifier": "sessions",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/emsp/sessions"
                //             },
                //             {
                //                 "identifier": "chargingprofiles",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/cpo/chargingprofiles"
                //             },
                //             {
                //                 "identifier": "cdrs",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/emsp/cdrs"
                //             },
                //             {
                //                 "identifier": "commands",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/emsp/commands"
                //             },
                //             {
                //                 "identifier": "tokens",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/emsp/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "timestamp":      "2025-01-10T05:54:40.844Z"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {

                    Assert.That(response.Data.VersionId,                                        Is.EqualTo(OCPIv2_2_1.Version.Id));

                    var endpoints = response.Data.Endpoints.
                                        GroupBy     (endpoint => endpoint.Identifier).
                                        ToDictionary(group    => group.Key,
                                                     group    => group.ToList());

                    Assert.That(endpoints,                                                      Is.Not.Null);
                    Assert.That(endpoints.Count,                                                Is.EqualTo(8));

                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Credentials].     First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.2.1/credentials")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Locations].       First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.2.1/emsp/locations")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tariffs].         First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.2.1/emsp/tariffs")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Sessions].        First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.2.1/emsp/sessions")));
                    //Assert.That(endpoints[OCPIv2_2_1.Module_Id.ChargingProfiles].First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.2.1/emsp/chargingProfiles")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.CDRs].            First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.2.1/emsp/cdrs")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Commands].        First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.2.1/emsp/commands")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tokens].          First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.2.1/emsp/tokens")));

                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Credentials].     Any(endpoint => endpoint.Role == OCPIv2_2_1.InterfaceRoles.SENDER),    Is.True);
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Credentials].     Any(endpoint => endpoint.Role == OCPIv2_2_1.InterfaceRoles.RECEIVER),  Is.True);

                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Locations].       First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tariffs].         First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Sessions].        First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.ChargingProfiles].First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.CDRs].            First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Commands].        First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tokens].          First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.SENDER));

                }

            }

        }

        #endregion

        #region CPO1_GetVersionDetails_v2_3_fromEMSP1_viaOCPIv2_3__RegisteredToken_Test1()

        /// <summary>
        /// CPO #1 asks EMSP #1 for its OCPI versions via OCPI v2.3!
        /// </summary>
        [Test]
        public async Task CPO1_GetVersionDetails_v2_3_fromEMSP1_viaOCPIv2_3__RegisteredToken_Test1()
        {

            var graphDefinedEMSP1 = cpo1CommonAPI_v2_3?.GetCPOClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersionDetails(OCPIv2_3.Version.Id);

                // GET /ocpi/versions/2.3 HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3401
                // User-Agent:                    GraphDefined OCPI v2.2.1 Common Client
                // Authorization:                 Token Y3BvMV9hY2Nlc3NpbmdfZW1zcDErK3Rva2Vu
                // Connection:                    close
                // X-Request-ID:                  xQfW89475Sx3pMvUv56Eb6x1nd5b4M
                // X-Correlation-ID:              jt45vS3E9AK1r3p5EEb927UUnx51U8

                // HTTP/1.1 200 OK
                // Date:                          Fri, 10 Jan 2025 05:54:40 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Allow:                         OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Vary:                          Accept
                // Server:                        GraphDefined HTTP API
                // Access-Control-Allow-Origin:   *
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                1029
                // X-Request-ID:                  xQfW89475Sx3pMvUv56Eb6x1nd5b4M
                // X-Correlation-ID:              jt45vS3E9AK1r3p5EEb927UUnx51U8
                // 
                // {
                //     "data": {
                //         "version": "2.3",
                //         "endpoints": [
                //             {
                //                 "identifier": "credentials",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/credentials"
                //             },
                //             {
                //                 "identifier": "credentials",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/credentials"
                //             },
                //             {
                //                 "identifier": "locations",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/emsp/locations"
                //             },
                //             {
                //                 "identifier": "tariffs",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/emsp/tariffs"
                //             },
                //             {
                //                 "identifier": "sessions",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/emsp/sessions"
                //             },
                //             {
                //                 "identifier": "chargingprofiles",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/cpo/chargingprofiles"
                //             },
                //             {
                //                 "identifier": "cdrs",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/emsp/cdrs"
                //             },
                //             {
                //                 "identifier": "commands",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/emsp/commands"
                //             },
                //             {
                //                 "identifier": "tokens",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3401/ocpi/v2.2.1/emsp/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "timestamp":      "2025-01-10T05:54:40.844Z"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {

                    Assert.That(response.Data.VersionId,                                        Is.EqualTo(OCPIv2_3.Version.Id));

                    var endpoints = response.Data.Endpoints.
                                        GroupBy     (endpoint => endpoint.Identifier).
                                        ToDictionary(group    => group.Key,
                                                     group    => group.ToList());

                    Assert.That(endpoints,                                                      Is.Not.Null);
                    Assert.That(endpoints.Count,                                                Is.EqualTo(8));

                    Assert.That(endpoints[OCPIv2_3.Module_Id.Credentials].     First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.3/credentials")));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Locations].       First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.3/emsp/locations")));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Tariffs].         First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.3/emsp/tariffs")));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Sessions].        First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.3/emsp/sessions")));
                    //Assert.That(endpoints[OCPIv2_3.Module_Id.ChargingProfiles].First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.3/emsp/chargingProfiles")));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.CDRs].            First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.3/emsp/cdrs")));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Commands].        First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.3/emsp/commands")));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Tokens].          First().URL,   Is.EqualTo(URL.Parse("http://localhost:3401/ocpi/v2.3/emsp/tokens")));

                    Assert.That(endpoints[OCPIv2_3.Module_Id.Credentials].     Any(endpoint => endpoint.Role == OCPIv2_3.InterfaceRoles.SENDER),    Is.True);
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Credentials].     Any(endpoint => endpoint.Role == OCPIv2_3.InterfaceRoles.RECEIVER),  Is.True);

                    Assert.That(endpoints[OCPIv2_3.Module_Id.Locations].       First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Tariffs].         First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Sessions].        First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.ChargingProfiles].First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.CDRs].            First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Commands].        First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Tokens].          First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.SENDER));

                }

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

        #region EMSP1_GetVersionDetails_v2_2_1_fromCPO1_viaOCPIv2_2_1__RegisteredToken_Test1()

        /// <summary>
        /// EMSP #1 asks CPO #1 for its OCPI v2.2.1 version details via OCPI v2.2.1!
        /// </summary>
        [Test]
        public async Task EMSP1_GetVersionDetails_v2_2_1_fromCPO1_viaOCPIv2_2_1__RegisteredToken_Test1()
        {

            var graphDefinedCPO1 = emsp1CommonAPI_v2_2_1?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersionDetails(OCPIv2_2_1.Version.Id);

                // GET /ocpi/versions/2.2.1 HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3301
                // User-Agent:                    GraphDefined OCPI v2.2.1 Common Client
                // Authorization:                 Token ZW1zcDFfYWNjZXNzaW5nX2NwbzErK3Rva2Vu
                // Connection:                    close
                // X-Request-ID:                  jbr78Y997pQ55E28xWvxjS7A459hUQ
                // X-Correlation-ID:              8xAn1v14p217MAE9GCtWESK1h9nA1x

                // HTTP/1.1 200 OK
                // Date:                          Fri, 10 Jan 2025 06:42:42 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Allow:                         OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Vary:                          Accept
                // Server:                        GraphDefined HTTP API
                // Access-Control-Allow-Origin:   *
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                1019
                // X-Request-ID:                  jbr78Y997pQ55E28xWvxjS7A459hUQ
                // X-Correlation-ID:              8xAn1v14p217MAE9GCtWESK1h9nA1x
                // 
                // {
                //     "data": {
                //         "version": "2.2.1",
                //         "endpoints": [
                //             {
                //                 "identifier": "credentials",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.2.1/credentials"
                //             },
                //             {
                //                 "identifier": "credentials",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3301/ocpi/v2.2.1/credentials"
                //             },
                //             {
                //                 "identifier": "locations",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.2.1/cpo/locations"
                //             },
                //             {
                //                 "identifier": "tariffs",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.2.1/cpo/tariffs"
                //             },
                //             {
                //                 "identifier": "sessions",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.2.1/cpo/sessions"
                //             },
                //             {
                //                 "identifier": "chargingprofiles",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.2.1/cpo/chargingprofiles"
                //             },
                //             {
                //                 "identifier": "cdrs",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.2.1/cpo/cdrs"
                //             },
                //             {
                //                 "identifier": "commands",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3301/ocpi/v2.2.1/cpo/commands"
                //             },
                //             {
                //                 "identifier": "tokens",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3301/ocpi/v2.2.1/cpo/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "timestamp":      "2025-01-10T06:42:42.201Z"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {

                    Assert.That(response.Data.VersionId,                                        Is.EqualTo(OCPIv2_2_1.Version.Id));

                    var endpoints = response.Data.Endpoints.
                                        GroupBy     (endpoint => endpoint.Identifier).
                                        ToDictionary(group    => group.Key,
                                                     group    => group.ToList());

                    Assert.That(endpoints,                                                      Is.Not.Null);
                    Assert.That(endpoints.Count,                                                Is.EqualTo(8));

                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Credentials].     First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/credentials")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Locations].       First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/locations")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tariffs].         First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/tariffs")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Sessions].        First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/sessions")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.ChargingProfiles].First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/chargingProfiles")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.CDRs].            First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/cdrs")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Commands].        First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/commands")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tokens].          First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/tokens")));

                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Credentials].     Any(endpoint => endpoint.Role == OCPIv2_2_1.InterfaceRoles.SENDER),    Is.True);
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Credentials].     Any(endpoint => endpoint.Role == OCPIv2_2_1.InterfaceRoles.RECEIVER),  Is.True);

                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Locations].       First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tariffs].         First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Sessions].        First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.SENDER));
                    //Assert.That(endpoints[OCPIv2_2_1.Module_Id.ChargingProfiles].First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.CDRs].            First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Commands].        First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tokens].          First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.RECEIVER));

                }


                //Assert.That(response.Request, Is.Not.Null);


            }

        }

        #endregion

        #region EMSP1_GetVersionDetails_v2_3_fromCPO1_viaOCPIv2_3__RegisteredToken_Test1()

        /// <summary>
        /// EMSP #1 asks CPO #1 for its OCPI v2.3 version details via OCPI v2.3!
        /// </summary>
        [Test]
        public async Task EMSP1_GetVersionDetails_v2_3_fromCPO1_viaOCPIv2_3__RegisteredToken_Test1()
        {

            var graphDefinedCPO1 = emsp1CommonAPI_v2_3?.GetEMSPClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersionDetails(OCPIv2_3.Version.Id);

                // GET /ocpi/versions/2.3 HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3301
                // User-Agent:                    GraphDefined OCPI v2.3 Common Client
                // Authorization:                 Token ZW1zcDFfYWNjZXNzaW5nX2NwbzErK3Rva2Vu
                // Connection:                    close
                // X-Request-ID:                  jbr78Y997pQ55E28xWvxjS7A459hUQ
                // X-Correlation-ID:              8xAn1v14p217MAE9GCtWESK1h9nA1x

                // HTTP/1.1 200 OK
                // Date:                          Fri, 10 Jan 2025 06:42:42 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Allow:                         OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Vary:                          Accept
                // Server:                        GraphDefined HTTP API
                // Access-Control-Allow-Origin:   *
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                1019
                // X-Request-ID:                  jbr78Y997pQ55E28xWvxjS7A459hUQ
                // X-Correlation-ID:              8xAn1v14p217MAE9GCtWESK1h9nA1x
                // 
                // {
                //     "data": {
                //         "version": "2.3",
                //         "endpoints": [
                //             {
                //                 "identifier": "credentials",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.3/credentials"
                //             },
                //             {
                //                 "identifier": "credentials",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3301/ocpi/v2.3/credentials"
                //             },
                //             {
                //                 "identifier": "locations",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.3/cpo/locations"
                //             },
                //             {
                //                 "identifier": "tariffs",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.3/cpo/tariffs"
                //             },
                //             {
                //                 "identifier": "sessions",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.3/cpo/sessions"
                //             },
                //             {
                //                 "identifier": "chargingprofiles",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.3/cpo/chargingprofiles"
                //             },
                //             {
                //                 "identifier": "cdrs",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.3/cpo/cdrs"
                //             },
                //             {
                //                 "identifier": "commands",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3301/ocpi/v2.3/cpo/commands"
                //             },
                //             {
                //                 "identifier": "tokens",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3301/ocpi/v2.3/cpo/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "timestamp":      "2025-01-10T06:42:42.201Z"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {

                    Assert.That(response.Data.VersionId,                                        Is.EqualTo(OCPIv2_3.Version.Id));

                    var endpoints = response.Data.Endpoints.
                                        GroupBy     (endpoint => endpoint.Identifier).
                                        ToDictionary(group    => group.Key,
                                                     group    => group.ToList());

                    Assert.That(endpoints,                                                      Is.Not.Null);
                    Assert.That(endpoints.Count,                                                Is.EqualTo(8));

                    Assert.That(endpoints[OCPIv2_3.Module_Id.Credentials].     First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.3/credentials")));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Locations].       First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.3/cpo/locations")));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Tariffs].         First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.3/cpo/tariffs")));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Sessions].        First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.3/cpo/sessions")));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.ChargingProfiles].First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.3/cpo/chargingProfiles")));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.CDRs].            First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.3/cpo/cdrs")));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Commands].        First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.3/cpo/commands")));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Tokens].          First().URL,   Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.3/cpo/tokens")));

                    Assert.That(endpoints[OCPIv2_3.Module_Id.Credentials].     Any(endpoint => endpoint.Role == OCPIv2_3.InterfaceRoles.SENDER),    Is.True);
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Credentials].     Any(endpoint => endpoint.Role == OCPIv2_3.InterfaceRoles.RECEIVER),  Is.True);

                    Assert.That(endpoints[OCPIv2_3.Module_Id.Locations].       First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Tariffs].         First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Sessions].        First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.SENDER));
                    //Assert.That(endpoints[OCPIv2_3.Module_Id.ChargingProfiles].First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.CDRs].            First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Commands].        First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_3.Module_Id.Tokens].          First().Role,  Is.EqualTo(OCPIv2_3.InterfaceRoles.RECEIVER));

                }


                //Assert.That(response.Request, Is.Not.Null);


            }

        }

        #endregion


    }

}
