﻿/*
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
    public class GetVersions_OpenData_Tests : A_2CPOs2EMSPs_TestDefaults
    {

        #region GetVersions_v2_1_1_fromCPO1_Test1()

        /// <summary>
        /// CPO #1 OCPI v2.1.1 versions as Open Data!
        /// </summary>
        [Test]
        public async Task GetVersions_v2_1_1_fromCPO1_Test1()
        {

            if (cpo1BaseAPI is null)
            {
                Assert.Fail("cpo1BaseAPI is null!");
                return;
            }

            var graphDefinedCPO1 = new OCPIv2_1_1.HTTP.CommonClient(
                                       cpo1BaseAPI.OurVersionsURL
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                          application/json; charset=utf-8; q=1
                // Host:                            127.0.0.1:3301
                // User-Agent:                      GraphDefined OCPI v2.1.1 CommonClient
                // Connection:                      close
                // X-Request-ID:                    64UC9vv67xSj258nz4YprUGth3W514
                // X-Correlation-ID:                rCtE9d3G1U72bdW7ArtUht9d8fj8Y1

                // HTTP/1.1 200 OK
                // Date:                            Fri, 10 Jan 2025 22:49:05 GMT
                // Server:                          GraphDefined OCPI HTTP API v0.1
                // Access-Control-Allow-Origin:     *
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Allow:                           OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  267
                // Connection:                      close
                // Vary:                            Accept
                // X-Request-ID:                    64UC9vv67xSj258nz4YprUGth3W514
                // X-Correlation-ID:                rCtE9d3G1U72bdW7ArtUht9d8fj8Y1
                // 
                // {
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "data": [
                //         {
                //             "version": "2.1.1",
                //             "url":     "http://localhost:3301/ocpi/versions/2.1.1"
                //         },
                //         {
                //             "version": "2.2.1",
                //             "url":     "http://localhost:3301/ocpi/versions/2.2.1"
                //         },
                //         {
                //             "version": "2.3",
                //             "url":     "http://localhost:3301/ocpi/versions/2.3"
                //         }
                //     ]
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                var versions = response.Data?.OrderBy(version => version.Id).ToArray();
                Assert.That(versions,                                                       Is.Not.Null);
                Assert.That(response.Data?.Count(),                                         Is.EqualTo(3));

                var version2_1_1 = versions?.ElementAt(0);
                Assert.That(version2_1_1?.Id == OCPIv2_1_1.Version.Id, Is.True);
                Assert.That(URL.Parse("http://localhost:3301/ocpi/versions/2.1.1") == version2_1_1?.URL, Is.True);

                var version2_2_1 = versions?.ElementAt(1);
                Assert.That(version2_2_1?.Id == OCPIv2_2_1.Version.Id, Is.True);
                Assert.That(URL.Parse("http://localhost:3301/ocpi/versions/2.2.1") == version2_2_1?.URL, Is.True);

                var version2_3   = versions?.ElementAt(2);
                Assert.That(version2_3?.  Id == OCPIv2_3_0.Version.Id, Is.True);
                Assert.That(URL.Parse("http://localhost:3301/ocpi/versions/2.3")   == version2_3?.  URL, Is.True);

            }

        }

        #endregion

        #region GetVersions_v2_2_1_fromCPO1_Test1()

        /// <summary>
        /// CPO #1 OCPI v2.2.1 versions as Open Data!
        /// </summary>
        [Test]
        public async Task GetVersions_v2_2_1_fromCPO1_Test1()
        {

            if (cpo1BaseAPI is null)
            {
                Assert.Fail("cpo1BaseAPI is null!");
                return;
            }

            var graphDefinedCPO1 = new OCPIv2_2_1.HTTP.CommonClient(
                                       cpo1BaseAPI.OurVersionsURL
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                          application/json; charset=utf-8; q=1
                // Host:                            127.0.0.1:3301
                // User-Agent:                      GraphDefined OCPI v2.2.1 CommonClient
                // Connection:                      close
                // X-Request-ID:                    64UC9vv67xSj258nz4YprUGth3W514
                // X-Correlation-ID:                rCtE9d3G1U72bdW7ArtUht9d8fj8Y1

                // HTTP/1.1 200 OK
                // Date:                            Fri, 10 Jan 2025 22:49:05 GMT
                // Server:                          GraphDefined OCPI HTTP API v0.1
                // Access-Control-Allow-Origin:     *
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Allow:                           OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  267
                // Connection:                      close
                // Vary:                            Accept
                // X-Request-ID:                    64UC9vv67xSj258nz4YprUGth3W514
                // X-Correlation-ID:                rCtE9d3G1U72bdW7ArtUht9d8fj8Y1
                // 
                // {
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "data": [
                //         {
                //             "version": "2.1.1",
                //             "url":     "http://localhost:3301/ocpi/versions/2.1.1"
                //         },
                //         {
                //             "version": "2.2.1",
                //             "url":     "http://localhost:3301/ocpi/versions/2.2.1"
                //         },
                //         {
                //             "version": "2.3",
                //             "url":     "http://localhost:3301/ocpi/versions/2.3"
                //         }
                //     ]
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                var versions = response.Data?.OrderBy(version => version.Id).ToArray();
                Assert.That(versions,                                                       Is.Not.Null);
                Assert.That(response.Data?.Count(),                                         Is.EqualTo(3));

                var version2_1_1 = versions?.ElementAt(0);
                Assert.That(version2_1_1?.Id == OCPIv2_1_1.Version.Id, Is.True);
                Assert.That(URL.Parse("http://localhost:3301/ocpi/versions/2.1.1") == version2_1_1?.URL, Is.True);

                var version2_2_1 = versions?.ElementAt(1);
                Assert.That(version2_2_1?.Id == OCPIv2_2_1.Version.Id, Is.True);
                Assert.That(URL.Parse("http://localhost:3301/ocpi/versions/2.2.1") == version2_2_1?.URL, Is.True);

                var version2_3   = versions?.ElementAt(2);
                Assert.That(version2_3?.  Id == OCPIv2_3_0.Version.Id, Is.True);
                Assert.That(URL.Parse("http://localhost:3301/ocpi/versions/2.3")   == version2_3?.  URL, Is.True);

            }

        }

        #endregion

        #region GetVersions_v2_3_fromCPO1_Test1()

        /// <summary>
        /// CPO #1 OCPI v2.3 versions as Open Data!
        /// </summary>
        [Test]
        public async Task GetVersions_v2_3_fromCPO1_Test1()
        {

            if (cpo1BaseAPI is null)
            {
                Assert.Fail("cpo1BaseAPI is null!");
                return;
            }

            var graphDefinedCPO1 = new OCPIv2_3_0.HTTP.CommonClient(
                                       cpo1BaseAPI.OurVersionsURL
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                          application/json; charset=utf-8; q=1
                // Host:                            127.0.0.1:3301
                // User-Agent:                      GraphDefined OCPI v2.3 CommonClient
                // Connection:                      close
                // X-Request-ID:                    64UC9vv67xSj258nz4YprUGth3W514
                // X-Correlation-ID:                rCtE9d3G1U72bdW7ArtUht9d8fj8Y1

                // HTTP/1.1 200 OK
                // Date:                            Fri, 10 Jan 2025 22:49:05 GMT
                // Server:                          GraphDefined OCPI HTTP API v0.1
                // Access-Control-Allow-Origin:     *
                // Access-Control-Allow-Methods:    OPTIONS, GET
                // Allow:                           OPTIONS, GET
                // Access-Control-Allow-Headers:    Authorization
                // Content-Type:                    application/json; charset=utf-8
                // Content-Length:                  267
                // Connection:                      close
                // Vary:                            Accept
                // X-Request-ID:                    64UC9vv67xSj258nz4YprUGth3W514
                // X-Correlation-ID:                rCtE9d3G1U72bdW7ArtUht9d8fj8Y1
                // 
                // {
                //     "status_code":     1000,
                //     "status_message": "Hello world!",
                //     "data": [
                //         {
                //             "version": "2.1.1",
                //             "url":     "http://localhost:3301/ocpi/versions/2.1.1"
                //         },
                //         {
                //             "version": "2.2.1",
                //             "url":     "http://localhost:3301/ocpi/versions/2.2.1"
                //         },
                //         {
                //             "version": "2.3",
                //             "url":     "http://localhost:3301/ocpi/versions/2.3"
                //         }
                //     ]
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200));
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                var versions = response.Data?.OrderBy(version => version.Id).ToArray();
                Assert.That(versions,                                                       Is.Not.Null);
                Assert.That(response.Data?.Count(),                                         Is.EqualTo(3));

                var version2_1_1 = versions?.ElementAt(0);
                Assert.That(version2_1_1?.Id == OCPIv2_1_1.Version.Id, Is.True);
                Assert.That(URL.Parse("http://localhost:3301/ocpi/versions/2.1.1") == version2_1_1?.URL, Is.True);

                var version2_2_1 = versions?.ElementAt(1);
                Assert.That(version2_2_1?.Id == OCPIv2_2_1.Version.Id, Is.True);
                Assert.That(URL.Parse("http://localhost:3301/ocpi/versions/2.2.1") == version2_2_1?.URL, Is.True);

                var version2_3   = versions?.ElementAt(2);
                Assert.That(version2_3?.  Id == OCPIv2_3_0.Version.Id, Is.True);
                Assert.That(URL.Parse("http://localhost:3301/ocpi/versions/2.3")   == version2_3?.  URL, Is.True);

            }

        }

        #endregion


        // Note: EMSP OpenData is probably not really useful!


    }

}
