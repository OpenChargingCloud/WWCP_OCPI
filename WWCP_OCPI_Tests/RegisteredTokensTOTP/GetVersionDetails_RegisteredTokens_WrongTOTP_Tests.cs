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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPI.UnitTests
{

    /// <summary>
    /// Testing the OCPI GetVersionDetails method(s) using registered OCPI tokens
    /// and WRONG time-based one-time passwords (TOTPs).
    /// </summary>
    [TestFixture]
    public class GetVersionDetails_RegisteredTokens_WrongTOTP_Tests()

        : A_2CPOs2EMSPs_TestDefaults(

              // This will enable additional TOTP authentication!
              TOTPValidityTime:   TimeSpan.FromSeconds(30)

          )

    {

        #region CPO1_GetVersionDetails_v2_1_1_fromEMSP1_viaOCPIv2_1_1__RegisteredToken_Test1()

        /// <summary>
        /// CPO #1 asking EMSP #1 for its OCPI version details via OCPI v2.1.1!
        /// </summary>
        [Test]
        public async Task CPO1_GetVersionDetails_v2_1_1_fromEMSP1_viaOCPIv2_1_1__RegisteredToken_Test1()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_1_1?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersionDetails(OCPIv2_1_1.Version.Id);

                // GET /ocpi/versions/2.1.1 HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3501
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Authorization:                 Token cpo1_accessing_emsp1++token
                // Connection:                    close
                // X-Request-ID:                  S1Q64pQ9nf24C3tA64xjhvWnz983jt
                // X-Correlation-ID:              945f2bEj63npK7fMSU48UdfnCt8722

                // HTTP/1.1 200 OK
                // Date:                          Fri, 10 Jan 2025 06:01:52 GMT
                // Server:                        GraphDefined OCPI v2.1.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
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
                //                 "url":        "http://localhost:3501/ocpi/v2.1.1/credentials"
                //             },
                //             {
                //                 "identifier": "locations",
                //                 "url":        "http://localhost:3501/ocpi/v2.1.1/emsp/locations"
                //             },
                //             {
                //                 "identifier": "tariffs",
                //                 "url":        "http://localhost:3501/ocpi/v2.1.1/emsp/tariffs"
                //             },
                //             {
                //                 "identifier": "sessions",
                //                 "url":        "http://localhost:3501/ocpi/v2.1.1/emsp/sessions"
                //             },
                //             {
                //                 "identifier": "cdrs",
                //                 "url":        "http://localhost:3501/ocpi/v2.1.1/emsp/cdrs"
                //             },
                //             {
                //                 "identifier": "commands",
                //                 "url":        "http://localhost:3501/ocpi/v2.1.1/emsp/commands"
                //             },
                //             {
                //                 "identifier": "tokens",
                //                 "url":        "http://localhost:3501/ocpi/v2.1.1/emsp/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "timestamp":      "2025-01-10T06:01:52.727Z"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {

                    Assert.That(response.Data.VersionId,                                        Is.EqualTo(OCPIv2_1_1.Version.Id));

                    var endpoints = response.Data.Endpoints.ToDictionary(endpoint => endpoint.Identifier);

                    Assert.That(endpoints,                                                      Is.Not.Null);
                    Assert.That(endpoints.Count,                                                Is.EqualTo(7));

                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Credentials].URL,                Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.1.1/credentials")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Locations].  URL,                Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.1.1/emsp/locations")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Tariffs].    URL,                Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.1.1/emsp/tariffs")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Sessions].   URL,                Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.1.1/emsp/sessions")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.CDRs].       URL,                Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.1.1/emsp/cdrs")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Commands].   URL,                Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.1.1/emsp/commands")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Tokens].     URL,                Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.1.1/emsp/tokens")));

                }


                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP,                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP?.Length,               Is.EqualTo(TOTPLength));
                Assert.That(response.HTTPResponse?.HTTPRequest?.ToString(),                 Contains.Substring($"TOTP: {response.HTTPResponse?.HTTPRequest?.TOTP}"));

            }

        }

        #endregion

        #region CPO1_GetVersionDetails_v2_2_1_fromEMSP1_viaOCPIv2_2_1__RegisteredToken_Test1()

        /// <summary>
        /// CPO #1 asking EMSP #1 for its OCPI version details via OCPI v2.2.1!
        /// </summary>
        [Test]
        public async Task CPO1_GetVersionDetails_v2_2_1_fromEMSP1_viaOCPIv2_2_1__RegisteredToken_Test1()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_2_1?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersionDetails(OCPIv2_2_1.Version.Id);

                // GET /ocpi/versions/2.2.1 HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3501
                // User-Agent:                    GraphDefined OCPI v2.2.1 Common Client
                // Authorization:                 Token Y3BvMV9hY2Nlc3NpbmdfZW1zcDErK3Rva2Vu
                // Connection:                    close
                // X-Request-ID:                  xQfW89475Sx3pMvUv56Eb6x1nd5b4M
                // X-Correlation-ID:              jt45vS3E9AK1r3p5EEb927UUnx51U8

                // HTTP/1.1 200 OK
                // Date:                          Fri, 10 Jan 2025 05:54:40 GMT
                // Server:                        GraphDefined OCPI v2.2.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
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
                //                 "url":        "http://localhost:3501/ocpi/v2.2.1/credentials"
                //             },
                //             {
                //                 "identifier": "credentials",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3501/ocpi/v2.2.1/credentials"
                //             },
                //             {
                //                 "identifier": "locations",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3501/ocpi/v2.2.1/emsp/locations"
                //             },
                //             {
                //                 "identifier": "tariffs",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3501/ocpi/v2.2.1/emsp/tariffs"
                //             },
                //             {
                //                 "identifier": "sessions",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3501/ocpi/v2.2.1/emsp/sessions"
                //             },
                //             {
                //                 "identifier": "chargingprofiles",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3501/ocpi/v2.2.1/emsp/chargingprofiles"
                //             },
                //             {
                //                 "identifier": "cdrs",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3501/ocpi/v2.2.1/emsp/cdrs"
                //             },
                //             {
                //                 "identifier": "commands",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3501/ocpi/v2.2.1/emsp/commands"
                //             },
                //             {
                //                 "identifier": "tokens",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3501/ocpi/v2.2.1/emsp/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "timestamp":      "2025-01-10T05:54:40.844Z"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200),  response.HTTPResponse?.HTTPBodyAsUTF8String);
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

                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Credentials].     First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.2.1/credentials")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Locations].       First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.2.1/emsp/locations")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tariffs].         First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.2.1/emsp/tariffs")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Sessions].        First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.2.1/emsp/sessions")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.ChargingProfiles].First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.2.1/emsp/chargingProfiles")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.CDRs].            First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.2.1/emsp/cdrs")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Commands].        First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.2.1/emsp/commands")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tokens].          First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.2.1/emsp/tokens")));

                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Credentials].     First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Locations].       First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tariffs].         First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Sessions].        First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.ChargingProfiles].First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.CDRs].            First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Commands].        First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tokens].          First().Role,  Is.EqualTo(OCPIv2_2_1.InterfaceRoles.SENDER));

                }


                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP,                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP?.Length,               Is.EqualTo(TOTPLength));
                Assert.That(response.HTTPResponse?.HTTPRequest?.ToString(),                 Contains.Substring($"TOTP: {response.HTTPResponse?.HTTPRequest?.TOTP}"));

            }

        }

        #endregion

        #region CPO1_GetVersionDetails_v2_3_0_fromEMSP1_viaOCPIv2_3_0__RegisteredToken_Test1()

        /// <summary>
        /// CPO #1 asking EMSP #1 for its OCPI version details via OCPI v2.3!
        /// </summary>
        [Test]
        public async Task CPO1_GetVersionDetails_v2_3_0_fromEMSP1_viaOCPIv2_3_0__RegisteredToken_Test1()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_3_0?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersionDetails(OCPIv2_3_0.Version.Id);

                // GET /ocpi/versions/2.3.0 HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3501
                // User-Agent:                    GraphDefined OCPI v2.3.0 Common Client
                // Authorization:                 Token Y3BvMV9hY2Nlc3NpbmdfZW1zcDErK3Rva2Vu
                // Connection:                    close
                // X-Request-ID:                  xQfW89475Sx3pMvUv56Eb6x1nd5b4M
                // X-Correlation-ID:              jt45vS3E9AK1r3p5EEb927UUnx51U8

                // HTTP/1.1 200 OK
                // Date:                          Fri, 10 Jan 2025 05:54:40 GMT
                // Server:                        GraphDefined OCPI v2.3.0 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                1029
                // X-Request-ID:                  xQfW89475Sx3pMvUv56Eb6x1nd5b4M
                // X-Correlation-ID:              jt45vS3E9AK1r3p5EEb927UUnx51U8
                // 
                // {
                //     "data": {
                //         "version": "2.3.0",
                //         "endpoints": [
                //             {
                //                 "identifier": "credentials",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3501/ocpi/v2.3.0/credentials"
                //             },
                //             {
                //                 "identifier": "credentials",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3501/ocpi/v2.3.0/credentials"
                //             },
                //             {
                //                 "identifier": "locations",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3501/ocpi/v2.3.0/emsp/locations"
                //             },
                //             {
                //                 "identifier": "tariffs",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3501/ocpi/v2.3.0/emsp/tariffs"
                //             },
                //             {
                //                 "identifier": "sessions",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3501/ocpi/v2.3.0/emsp/sessions"
                //             },
                //             {
                //                 "identifier": "chargingprofiles",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3501/ocpi/v2.3.0/emsp/chargingprofiles"
                //             },
                //             {
                //                 "identifier": "cdrs",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3501/ocpi/v2.3.0/emsp/cdrs"
                //             },
                //             {
                //                 "identifier": "commands",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3501/ocpi/v2.3.0/emsp/commands"
                //             },
                //             {
                //                 "identifier": "tokens",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3501/ocpi/v2.3.0/emsp/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "timestamp":      "2025-01-10T05:54:40.844Z"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {

                    Assert.That(response.Data.VersionId,                                        Is.EqualTo(OCPIv2_3_0.Version.Id));

                    var endpoints = response.Data.Endpoints.
                                        GroupBy     (endpoint => endpoint.Identifier).
                                        ToDictionary(group    => group.Key,
                                                     group    => group.ToList());

                    Assert.That(endpoints,                                                      Is.Not.Null);
                    Assert.That(endpoints.Count,                                                Is.EqualTo(8));

                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Credentials].     First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.3.0/credentials")));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Locations].       First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.3.0/emsp/locations")));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Tariffs].         First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.3.0/emsp/tariffs")));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Sessions].        First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.3.0/emsp/sessions")));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.ChargingProfiles].First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.3.0/emsp/chargingProfiles")));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.CDRs].            First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.3.0/emsp/cdrs")));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Commands].        First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.3.0/emsp/commands")));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Tokens].          First().URL,   Is.EqualTo(URL.Parse("http://localhost:3501/ocpi/v2.3.0/emsp/tokens")));

                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Credentials].     First().Role,  Is.EqualTo(OCPIv2_3_0.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Locations].       First().Role,  Is.EqualTo(OCPIv2_3_0.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Tariffs].         First().Role,  Is.EqualTo(OCPIv2_3_0.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Sessions].        First().Role,  Is.EqualTo(OCPIv2_3_0.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.ChargingProfiles].First().Role,  Is.EqualTo(OCPIv2_3_0.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.CDRs].            First().Role,  Is.EqualTo(OCPIv2_3_0.InterfaceRoles.RECEIVER));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Commands].        First().Role,  Is.EqualTo(OCPIv2_3_0.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Tokens].          First().Role,  Is.EqualTo(OCPIv2_3_0.InterfaceRoles.SENDER));

                }


                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP,                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP?.Length,               Is.EqualTo(TOTPLength));
                Assert.That(response.HTTPResponse?.HTTPRequest?.ToString(),                 Contains.Substring($"TOTP: {response.HTTPResponse?.HTTPRequest?.TOTP}"));

            }

        }

        #endregion


        #region EMSP1_GetVersionDetails_v2_1_1_fromCPO1_viaOCPIv2_1_1__RegisteredToken_Test1()

        /// <summary>
        /// EMSP #1 asks CPO #1 for its OCPI v2.1.1 version details details via OCPI v2.1.1!
        /// </summary>
        [Test]
        public async Task EMSP1_GetVersionDetails_v2_1_1_fromCPO1_viaOCPIv2_1_1__RegisteredToken_Test1()
        {

            var graphDefinedCPO1 = emsp1EMSPAPI_v2_1_1?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersionDetails(OCPIv2_1_1.Version.Id);

                // GET /ocpi/versions/2.1.1 HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3301
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Authorization:                 Token emsp1_accessing_cpo1++token
                // Connection:                    close
                // X-Request-ID:                  1tEnWAzn2CME5dSr673Q7UM2v6152n
                // X-Correlation-ID:              t3bh9f2bUbzjGzGCQjx3SjU3d1G8xd

                // HTTP/1.1 200 OK
                // Date:                          Wed, 08 Jan 2025 12:35:51 GMT
                // Server:                        GraphDefined OCPI v2.1.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                689
                // X-Request-ID:                  1tEnWAzn2CME5dSr673Q7UM2v6152n
                // X-Correlation-ID:              t3bh9f2bUbzjGzGCQjx3SjU3d1G8xd
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
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200),  response.HTTPResponse?.HTTPBodyAsUTF8String);
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


                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP,                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP?.Length,               Is.EqualTo(TOTPLength));
                Assert.That(response.HTTPResponse?.HTTPRequest?.ToString(),                 Contains.Substring($"TOTP: {response.HTTPResponse?.HTTPRequest?.TOTP}"));

            }

        }

        #endregion

        #region EMSP1_GetVersionDetails_v2_2_1_fromCPO1_viaOCPIv2_2_1__RegisteredToken_Test1()

        /// <summary>
        /// EMSP #1 asks CPO #1 for its OCPI v2.2.1 version details details via OCPI v2.2.1!
        /// </summary>
        [Test]
        public async Task EMSP1_GetVersionDetails_v2_2_1_fromCPO1_viaOCPIv2_2_1__RegisteredToken_Test1()
        {

            var graphDefinedCPO1 = emsp1EMSPAPI_v2_2_1?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                // Manipulate the TOTP config to always generate a wrong TOTP!
                graphDefinedCPO1.NewHTTPClient.TOTPConfig = new TOTPConfig(
                                                                "brokenbrokenbroken",
                                                                TimeSpan.FromHours(5),
                                                                18
                                                            );

                var response = await graphDefinedCPO1.GetVersionDetails(OCPIv2_2_1.Version.Id);

                // GET /ocpi/versions/2.2.1 HTTP/1.1



                // Already the pre-fetch failed!

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3301
                // User-Agent:                    GraphDefined OCPI v2.2.1 Common Client
                // Authorization:                 Token ZW1zcDFfYWNjZXNzaW5nX2NwbzErK3Rva2Vu
                // TOTP:                          aHZf4iiYoJmrA0tn7k
                // X-Request-ID:                  rYA9bf16vb4K1jUpj29xKQ497x56th
                // X-Correlation-ID:              5KSbUxv4M59jU5G75xWzKGfAGpGpYU
                // Connection:                    keep-alive
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                0

                // HTTP/1.1 401 Unauthorized
                // Date:                          Fri, 16 Jan 2026 10:05:00 GMT
                // Server:                        HTTP Service
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Headers:  Authorization
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                123
                // Connection:                    keep-alive
                // Vary:                          Accept
                // 
                // {
                //     "status_code":      2000,
                //     "status_message":  "Invalid Time-based One-Time Password (TOTP)!",
                //     "timestamp":       "2026-01-16T10:05:00.605Z"
                // }


                // Server:                        GraphDefined OCPI v2.2.1 Common HTTP API
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Allow:                         OPTIONS, GET
                // Content-Length:                1019
                // X-Request-ID:                  jbr78Y997pQ55E28xWvxjS7A459hUQ
                // X-Correlation-ID:              8xAn1v14p217MAE9GCtWESK1h9nA1x

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(401),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(2000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Invalid Time-based One-Time Password (TOTP)!"));
                Assert.That(response.Data,                                                  Is.Null);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP,                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP?.Length,               Is.EqualTo(18));
                Assert.That(response.HTTPResponse?.HTTPRequest?.ToString(),                 Contains.Substring($"TOTP: "));

            }

        }

        #endregion

        #region EMSP1_GetVersionDetails_v2_3_0_fromCPO1_viaOCPIv2_3_0__RegisteredToken_Test1()

        /// <summary>
        /// EMSP #1 asks CPO #1 for its OCPI v2.3 version details details via OCPI v2.3!
        /// </summary>
        [Test]
        public async Task EMSP1_GetVersionDetails_v2_3_0_fromCPO1_viaOCPIv2_3_0__RegisteredToken_Test1()
        {

            var graphDefinedCPO1 = emsp1EMSPAPI_v2_3_0?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response1 = await graphDefinedCPO1.GetVersions();

                // Manipulate the TOTP config to always generate a wrong TOTP!
                graphDefinedCPO1.NewHTTPClient.TOTPConfig = new TOTPConfig(
                                                                "brokenbrokenbroken",
                                                                TimeSpan.FromHours(5),
                                                                18
                                                            );

                var response = await graphDefinedCPO1.GetVersionDetails(OCPIv2_3_0.Version.Id);

                // GET /ocpi/versions/2.3.0 HTTP/1.1


                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(401),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(2000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Invalid Time-based One-Time Password (TOTP)!"));
                Assert.That(response.Data,                                                  Is.Null);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP,                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP?.Length,               Is.EqualTo(18));
                Assert.That(response.HTTPResponse?.HTTPRequest?.ToString(),                 Contains.Substring($"TOTP: "));

            }

        }

        #endregion


        #region EMSP1_GetVersionDetails_v2_3_0_fromCPO1_viaOCPIv2_3_0__RegisteredToken_Test2()

        /// <summary>
        /// EMSP #1 asks CPO #1 for its OCPI v2.3 version details details via OCPI v2.3!
        /// </summary>
        [Test]
        public async Task EMSP1_GetVersionDetails_v2_3_0_fromCPO1_viaOCPIv2_3_0__RegisteredToken_Test2()
        {

            var graphDefinedCPO1 = emsp1EMSPAPI_v2_3_0?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                // Manipulate the TOTP config to always generate a wrong TOTP!
                graphDefinedCPO1.NewHTTPClient.TOTPConfig = new TOTPConfig(
                                                                "brokenbrokenbroken",
                                                                TimeSpan.FromHours(5),
                                                                18
                                                            );

                var response = await graphDefinedCPO1.GetVersionDetails(OCPIv2_3_0.Version.Id);

                // GET /ocpi/versions/2.3.0 HTTP/1.1


                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(401),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(2000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Invalid Time-based One-Time Password (TOTP)!"));
                Assert.That(response.Data,                                                  Is.Null);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP,                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPRequest?.TOTP?.Length,               Is.EqualTo(18));
                Assert.That(response.HTTPResponse?.HTTPRequest?.ToString(),                 Contains.Substring($"TOTP: "));

            }

        }

        #endregion


    }

}
