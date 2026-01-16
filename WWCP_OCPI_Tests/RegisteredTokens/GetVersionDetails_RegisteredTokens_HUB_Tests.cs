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

#endregion

namespace cloud.charging.open.protocols.OCPI.UnitTests
{

    /// <summary>
    /// Testing the OCPI GetVersionDetails method(s) using registered OCPI tokens.
    /// </summary>
    [TestFixture]
    public class GetVersionDetails_RegisteredTokens_HUB_Tests()

        : A_2CPOs2EMSPs_TestDefaults(
              UseHUBCommunication:   true
          )

    {

        #region HUB1_GetVersionDetails_v2_2_1_fromCPO1_viaOCPIv2_2_1__RegisteredToken_Test1()

        /// <summary>
        /// HUB #1 asks CPO #1 for its OCPI v2.2.1 version details details via OCPI v2.2.1!
        /// </summary>
        [Test]
        public async Task HUB1_GetVersionDetails_v2_2_1_fromCPO1_viaOCPIv2_2_1__RegisteredToken_Test1()
        {

            var graphDefinedCPO1 = hub1HUBAPI_v2_2_1?.GetCPOClient(
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
                // Server:                        GraphDefined OCPI v2.2.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
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
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
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

    }

}
