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
    /// Testing the OCPI GetVersionDetails method(s) using NO OCPI tokens (Open Data Access).
    /// </summary>
    [TestFixture]
    public class GetVersionDetails_OpenData_Tests : A_2CPOs2EMSPs_TestDefaults
    {

        #region GetVersionDetails_v2_1_1_fromCPO1_Test1()

        /// <summary>
        /// CPO #1 Open Data for its OCPI v2.1.1 version details!
        /// </summary>
        [Test]
        public async Task GetVersionDetails_v2_1_1_fromCPO1_Test1()
        {

            if (emsp1CommonAPI_v2_1_1 is null)
            {
                Assert.Fail("emsp1CommonAPI_v2_1_1 is null!");
                return;
            }

            if (cpo1CommonHTTPAPI is null)
            {
                Assert.Fail("cpo1CommonHTTPAPI is null!");
                return;
            }

            var graphDefinedCPO1 = emsp1CommonAPI_v2_1_1.GetCommonClient(
                                       RemoteVersionsURL:  cpo1CommonHTTPAPI.OurVersionsURL
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersionDetails(OCPIv2_1_1.Version.Id);

                // GET /ocpi/versions/2.1.1 HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3301
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Connection:                    close
                // X-Request-ID:                  S183YEKK6jSp7471vhnEv39r22zp3U
                // X-Correlation-ID:              8W7x6MKE8f1M2CQY1379pAUWt79QM3

                // HTTP/1.1 200 OK
                // Date:                          Fri, 10 Jan 2025 22:20:12 GMT
                // Server:                        GraphDefined OCPI v2.1.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                298
                // X-Request-ID:                  S183YEKK6jSp7471vhnEv39r22zp3U
                // X-Correlation-ID:              8W7x6MKE8f1M2CQY1379pAUWt79QM3
                // 
                // {
                //     "data": {
                //         "version": "2.1.1",
                //         "endpoints": [
                //             {
                //                 "identifier": "credentials",
                //                 "url":        "http://localhost:3301/ocpi/v2.1.1/credentials"
                //             },
                //             {
                //                 "identifier": "locations",
                //                 "url":        "http://localhost:3301/ocpi/v2.1.1/cpo/locations"
                //             },
                //             {
                //                 "identifier": "tariffs",
                //                 "url":        "http://localhost:3301/ocpi/v2.1.1/cpo/tariffs"
                //             }
                //         ]
                //     },
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "timestamp":      "2025-01-10T22:20:12.270Z"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200), response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {

                    Assert.That(response.Data.VersionId,                                    Is.EqualTo(OCPIv2_1_1.Version.Id));

                    var endpoints = response.Data.Endpoints.ToDictionary(endpoint => endpoint.Identifier);

                    Assert.That(endpoints,                                                  Is.Not.Null);
                    Assert.That(endpoints.Count,                                            Is.EqualTo(3));

                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Credentials].URL,            Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.1.1/credentials")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Locations].  URL,            Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.1.1/cpo/locations")));
                    Assert.That(endpoints[OCPIv2_1_1.Module_Id.Tariffs].    URL,            Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.1.1/cpo/tariffs")));

                }

            }

        }

        #endregion

        #region GetVersionDetails_v2_2_1_fromCPO1_Test1()

        /// <summary>
        /// CPO #1 Open Data for its OCPI v2.2.1 version details!
        /// </summary>
        [Test]
        public async Task GetVersionDetails_v2_2_1_fromCPO1_Test1()
        {

            if (emsp1CommonAPI_v2_2_1 is null)
            {
                Assert.Fail("emsp1CommonAPI_v2_2_1 is null!");
                return;
            }

            if (cpo1CommonHTTPAPI is null)
            {
                Assert.Fail("cpo1CommonHTTPAPI is null!");
                return;
            }

            var graphDefinedCPO1 = emsp1CommonAPI_v2_2_1.GetCommonClient(
                                       RemoteVersionsURL:  cpo1CommonHTTPAPI.OurVersionsURL
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersionDetails(OCPIv2_2_1.Version.Id);

                // GET /ocpi/versions/2.2.1 HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3301
                // User-Agent:                    GraphDefined HTTP Client
                // Connection:                    close
                // X-Request-ID:                  W5d451v324A1p7rSQY9fYhCC4MS46h
                // X-Correlation-ID:              88fn5bK8pp12b89UQf216t475EQ5r5

                // HTTP/1.1 200 OK
                // Date:                          Fri, 10 Jan 2025 22:32:16 GMT
                // Server:                        GraphDefined OCPI v2.2.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                431
                // X-Request-ID:                  W5d451v324A1p7rSQY9fYhCC4MS46h
                // X-Correlation-ID:              88fn5bK8pp12b89UQf216t475EQ5r5
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
                //             }
                //         ]
                //     },
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "timestamp":      "2025-01-10T22:32:16.547Z"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200), response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {

                    Assert.That(response.Data.VersionId,                                    Is.EqualTo(OCPIv2_2_1.Version.Id));

                    var endpoints = response.Data.Endpoints.
                                        GroupBy     (endpoint => endpoint.Identifier).
                                        ToDictionary(group    => group.Key,
                                                     group    => group.ToList());

                    Assert.That(endpoints,                                                  Is.Not.Null);
                    Assert.That(endpoints.Count,                                            Is.EqualTo(3));

                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Credentials].First().URL,    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/credentials")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Locations].  First().URL,    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/locations")));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tariffs].    First().URL,    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.2.1/cpo/tariffs")));

                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Credentials].Any(endpoint => endpoint.Role == OCPIv2_2_1.InterfaceRoles.SENDER),    Is.True);
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Credentials].Any(endpoint => endpoint.Role == OCPIv2_2_1.InterfaceRoles.RECEIVER),  Is.True); //ToDo: Does this make sense for OpenData access?

                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Locations].  First().Role,   Is.EqualTo(OCPIv2_2_1.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_2_1.Module_Id.Tariffs].    First().Role,   Is.EqualTo(OCPIv2_2_1.InterfaceRoles.SENDER));

                }

            }

        }

        #endregion

        #region GetVersionDetails_v2_3_0_fromCPO1_Test1()

        /// <summary>
        /// CPO #1 Open Data for its OCPI v2.3.0 version details!
        /// </summary>
        [Test]
        public async Task GetVersionDetails_v2_3_0_fromCPO1_Test1()
        {

            if (emsp1CommonAPI_v2_3_0 is null)
            {
                Assert.Fail("emsp1CommonAPI_v2_3_0 is null!");
                return;
            }

            if (cpo1CommonHTTPAPI is null)
            {
                Assert.Fail("cpo1CommonHTTPAPI is null!");
                return;
            }

            var graphDefinedCPO1 = emsp1CommonAPI_v2_3_0.GetCommonClient(
                                       RemoteVersionsURL:  cpo1CommonHTTPAPI.OurVersionsURL
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersionDetails(OCPIv2_3_0.Version.Id);

                // GET /ocpi/versions/2.3.0 HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          localhost:3301
                // User-Agent:                    GraphDefined HTTP Client
                // Connection:                    close
                // X-Request-ID:                  3tCbKAb188ChzfKfK353dY7r8Ez1MQ
                // X-Correlation-ID:              GAtGb22dSt3fAGp31S7WQU1d8YKQ4v

                // HTTP/1.1 200 OK
                // Date:                          Fri, 10 Jan 2025 22:35:21 GMT
                // Server:                        GraphDefined OCPI v2.3.0 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                526
                // X-Request-ID:                  3tCbKAb188ChzfKfK353dY7r8Ez1MQ
                // X-Correlation-ID:              GAtGb22dSt3fAGp31S7WQU1d8YKQ4v
                // 
                // {
                //     "data": {
                //         "version": "2.3.0",
                //         "endpoints": [
                //             {
                //                 "identifier": "credentials",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.3.0/credentials"
                //             },
                //             {
                //                 "identifier": "credentials",
                //                 "role":       "RECEIVER",
                //                 "url":        "http://localhost:3301/ocpi/v2.3.0/credentials"
                //             },
                //             {
                //                 "identifier": "locations",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.3.0/cpo/locations"
                //             },
                //             {
                //                 "identifier": "tariffs",
                //                 "role":       "SENDER",
                //                 "url":        "http://localhost:3301/ocpi/v2.3.0/cpo/tariffs"
                //             }
                //         ]
                //     },
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "timestamp":      "2025-01-10T22:35:21.015Z"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200), response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {

                    Assert.That(response.Data.VersionId,                                    Is.EqualTo(OCPIv2_3_0.Version.Id));

                    var endpoints = response.Data.Endpoints.
                                        GroupBy     (endpoint => endpoint.Identifier).
                                        ToDictionary(group    => group.Key,
                                                     group    => group.ToList());

                    Assert.That(endpoints,                                                  Is.Not.Null);
                    Assert.That(endpoints.Count,                                            Is.EqualTo(3));

                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Credentials].First().URL,    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.3.0/credentials")));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Locations].  First().URL,    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.3.0/cpo/locations")));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Tariffs].    First().URL,    Is.EqualTo(URL.Parse("http://localhost:3301/ocpi/v2.3.0/cpo/tariffs"))); //ToDo: Should AdHoc tariffs be available on default?

                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Credentials].Any(endpoint => endpoint.Role == OCPIv2_3_0.InterfaceRoles.SENDER),    Is.True);
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Credentials].Any(endpoint => endpoint.Role == OCPIv2_3_0.InterfaceRoles.RECEIVER),  Is.True); //ToDo: Does this make sense for OpenData access?

                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Locations].  First().Role,   Is.EqualTo(OCPIv2_3_0.InterfaceRoles.SENDER));
                    Assert.That(endpoints[OCPIv2_3_0.Module_Id.Tariffs].    First().Role,   Is.EqualTo(OCPIv2_3_0.InterfaceRoles.SENDER));

                }

            }

        }

        #endregion


        // Note: EMSP OpenData is probably not really useful!


    }

}
