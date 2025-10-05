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

#endregion

namespace cloud.charging.open.protocols.OCPI.UnitTests
{

    /// <summary>
    /// Testing the OCPI GetVersionDetails method(s) using blocked OCPI tokens.
    /// </summary>
    [TestFixture]
    public class GetVersions_BlockedTokens_Tests : A_2CPOs2EMSPs_TestDefaults
    {

        #region GetVersions_v2_1_1_fromCPO1_Test1()

        /// <summary>
        /// Trying to fetch CPO #1 OCPI v2.1.1 versions via a blocked OCPI access token!
        /// </summary>
        [Test]
        public async Task GetVersions_v2_1_1_fromCPO1_Test1()
        {

            if (cpo1CommonHTTPAPI is null)
            {
                Assert.Fail("cpo1BaseAPI is null!");
                return;
            }

            var graphDefinedCPO1 = new OCPIv2_1_1.CommonClient(
                                       VersionsURL:  cpo1CommonHTTPAPI.OurVersionsURL,
                                       AccessToken:  BlockedToken
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          127.0.0.1:3301
                // Authorization:                 Token blocked-token
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Connection:                    close
                // X-Request-ID:                  Sz6v65hz6U3r936vfGd845ftSbxtt4
                // X-Correlation-ID:              1jS4bY9KYU22GU7dWS5x331QjC7E2S

                // HTTP/1.1 200 OK
                // Date:                          Sun, 12 Jan 2025 12:02:52 GMT
                // Server:                        GraphDefined OCPI v2.1.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                72
                // X-Request-ID:                  Sz6v65hz6U3r936vfGd845ftSbxtt4
                // X-Correlation-ID:              1jS4bY9KYU22GU7dWS5x331QjC7E2S
                // 
                // {
                //     "status_code":     2000,
                //     "status_message": "Invalid or blocked access token!"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200)); //ToDo: Would be better to have a 401 here! But does the OCPI specification allow this?
                Assert.That(response.StatusCode,                                            Is.EqualTo(2000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Invalid or blocked access token!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(response.Data,                                                  Is.Not.Null);
                Assert.That(response.Data?.Any(),                                           Is.False);

            }

        }

        #endregion

        #region GetVersions_v2_1_1_fromEMSP1_Test1()

        /// <summary>
        /// Trying to fetch EMSP #1 OCPI v2.1.1 versions via a blocked OCPI access token!
        /// </summary>
        [Test]
        public async Task GetVersions_v2_1_1_fromEMSP1_Test1()
        {

            if (emsp1CommonHTTPAPI is null)
            {
                Assert.Fail("emsp1BaseAPI is null!");
                return;
            }

            var graphDefinedEMSP1 = new OCPIv2_1_1.CommonClient(
                                        VersionsURL:  emsp1CommonHTTPAPI.OurVersionsURL,
                                        AccessToken:  BlockedToken
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          127.0.0.1:3301
                // Authorization:                 Token blocked-token
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Connection:                    close
                // X-Request-ID:                  Sz6v65hz6U3r936vfGd845ftSbxtt4
                // X-Correlation-ID:              1jS4bY9KYU22GU7dWS5x331QjC7E2S

                // HTTP/1.1 200 OK
                // Date:                          Sun, 12 Jan 2025 12:02:52 GMT
                // Server:                        GraphDefined OCPI v2.1.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                72
                // X-Request-ID:                  Sz6v65hz6U3r936vfGd845ftSbxtt4
                // X-Correlation-ID:              1jS4bY9KYU22GU7dWS5x331QjC7E2S
                // 
                // {
                //     "status_code":     2000,
                //     "status_message": "Invalid or blocked access token!"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200)); //ToDo: Would be better to have a 401 here! But does the OCPI specification allow this?
                Assert.That(response.StatusCode,                                            Is.EqualTo(2000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Invalid or blocked access token!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(response.Data,                                                  Is.Not.Null);
                Assert.That(response.Data?.Any(),                                           Is.False);

            }

        }

        #endregion


        #region GetVersions_v2_2_1_fromCPO1_Test1()

        /// <summary>
        /// Trying to fetch CPO #1 OCPI v2.2.1 versions via a blocked OCPI access token!
        /// </summary>
        [Test]
        public async Task GetVersions_v2_2_1_fromCPO1_Test1()
        {

            if (cpo1CommonHTTPAPI is null)
            {
                Assert.Fail("cpo1BaseAPI is null!");
                return;
            }

            var graphDefinedCPO1 = new OCPIv2_2_1.CommonClient(
                                       VersionsURL:  cpo1CommonHTTPAPI.OurVersionsURL,
                                       AccessToken:  BlockedToken
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          127.0.0.1:3301
                // Authorization:                 Token blocked-token
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Connection:                    close
                // X-Request-ID:                  Sz6v65hz6U3r936vfGd845ftSbxtt4
                // X-Correlation-ID:              1jS4bY9KYU22GU7dWS5x331QjC7E2S

                // HTTP/1.1 200 OK
                // Date:                          Sun, 12 Jan 2025 12:02:52 GMT
                // Server:                        GraphDefined OCPI v2.2.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                72
                // X-Request-ID:                  Sz6v65hz6U3r936vfGd845ftSbxtt4
                // X-Correlation-ID:              1jS4bY9KYU22GU7dWS5x331QjC7E2S
                // 
                // {
                //     "status_code":     2000,
                //     "status_message": "Invalid or blocked access token!"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200)); //ToDo: Would be better to have a 401 here! But does the OCPI specification allow this?
                Assert.That(response.StatusCode,                                            Is.EqualTo(2000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Invalid or blocked access token!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(response.Data,                                                  Is.Not.Null);
                Assert.That(response.Data?.Any(),                                           Is.False);

            }

        }

        #endregion

        #region GetVersions_v2_2_1_fromEMSP1_Test1()

        /// <summary>
        /// Trying to fetch EMSP #1 OCPI v2.2.1 versions via a blocked OCPI access token!
        /// </summary>
        [Test]
        public async Task GetVersions_v2_2_1_fromEMSP1_Test1()
        {

            if (emsp1CommonHTTPAPI is null)
            {
                Assert.Fail("emsp1BaseAPI is null!");
                return;
            }

            var graphDefinedEMSP1 = new OCPIv2_2_1.CommonClient(
                                        VersionsURL:  emsp1CommonHTTPAPI.OurVersionsURL,
                                        AccessToken:  BlockedToken
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          127.0.0.1:3301
                // Authorization:                 Token blocked-token
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Connection:                    close
                // X-Request-ID:                  Sz6v65hz6U3r936vfGd845ftSbxtt4
                // X-Correlation-ID:              1jS4bY9KYU22GU7dWS5x331QjC7E2S

                // HTTP/1.1 200 OK
                // Date:                          Sun, 12 Jan 2025 12:02:52 GMT
                // Server:                        GraphDefined OCPI v2.2.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                72
                // X-Request-ID:                  Sz6v65hz6U3r936vfGd845ftSbxtt4
                // X-Correlation-ID:              1jS4bY9KYU22GU7dWS5x331QjC7E2S
                // 
                // {
                //     "status_code":     2000,
                //     "status_message": "Invalid or blocked access token!"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200)); //ToDo: Would be better to have a 401 here! But does the OCPI specification allow this?
                Assert.That(response.StatusCode,                                            Is.EqualTo(2000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Invalid or blocked access token!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(response.Data,                                                  Is.Not.Null);
                Assert.That(response.Data?.Any(),                                           Is.False);

            }

        }

        #endregion


        #region GetVersions_v2_3_0_fromCPO1_Test1()

        /// <summary>
        /// Trying to fetch CPO #1 OCPI v2.3.0 versions via a blocked OCPI access token!
        /// </summary>
        [Test]
        public async Task GetVersions_v2_3_0_fromCPO1_Test1()
        {

            if (cpo1CommonHTTPAPI is null)
            {
                Assert.Fail("cpo1BaseAPI is null!");
                return;
            }

            var graphDefinedCPO1 = new OCPIv2_3_0.CommonClient(
                                       VersionsURL:  cpo1CommonHTTPAPI.OurVersionsURL,
                                       AccessToken:  BlockedToken
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          127.0.0.1:3301
                // Authorization:                 Token blocked-token
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Connection:                    close
                // X-Request-ID:                  Sz6v65hz6U3r936vfGd845ftSbxtt4
                // X-Correlation-ID:              1jS4bY9KYU22GU7dWS5x331QjC7E2S

                // HTTP/1.1 200 OK
                // Date:                          Sun, 12 Jan 2025 12:02:52 GMT
                // Server:                        GraphDefined OCPI v2.3.0 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                72
                // X-Request-ID:                  Sz6v65hz6U3r936vfGd845ftSbxtt4
                // X-Correlation-ID:              1jS4bY9KYU22GU7dWS5x331QjC7E2S
                // 
                // {
                //     "status_code":     2000,
                //     "status_message": "Invalid or blocked access token!"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200)); //ToDo: Would be better to have a 401 here! But does the OCPI specification allow this?
                Assert.That(response.StatusCode,                                            Is.EqualTo(2000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Invalid or blocked access token!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(response.Data,                                                  Is.Not.Null);
                Assert.That(response.Data?.Any(),                                           Is.False);

            }

        }

        #endregion

        #region GetVersions_v2_3_0_fromEMSP1_Test1()

        /// <summary>
        /// Trying to fetch EMSP #1 OCPI v2.3.0 versions via a blocked OCPI access token!
        /// </summary>
        [Test]
        public async Task GetVersions_v2_3_0_fromEMSP1_Test1()
        {

            if (emsp1CommonHTTPAPI is null)
            {
                Assert.Fail("emsp1BaseAPI is null!");
                return;
            }

            var graphDefinedEMSP1 = new OCPIv2_3_0.CommonClient(
                                        VersionsURL:  emsp1CommonHTTPAPI.OurVersionsURL,
                                        AccessToken:  BlockedToken
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          127.0.0.1:3301
                // Authorization:                 Token blocked-token
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Connection:                    close
                // X-Request-ID:                  Sz6v65hz6U3r936vfGd845ftSbxtt4
                // X-Correlation-ID:              1jS4bY9KYU22GU7dWS5x331QjC7E2S

                // HTTP/1.1 200 OK
                // Date:                          Sun, 12 Jan 2025 12:02:52 GMT
                // Server:                        GraphDefined OCPI v2.3.0 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                72
                // X-Request-ID:                  Sz6v65hz6U3r936vfGd845ftSbxtt4
                // X-Correlation-ID:              1jS4bY9KYU22GU7dWS5x331QjC7E2S
                // 
                // {
                //     "status_code":     2000,
                //     "status_message": "Invalid or blocked access token!"
                // }

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200)); //ToDo: Would be better to have a 401 here! But does the OCPI specification allow this?
                Assert.That(response.StatusCode,                                            Is.EqualTo(2000));
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Invalid or blocked access token!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(response.Data,                                                  Is.Not.Null);
                Assert.That(response.Data?.Any(),                                           Is.False);

            }

        }

        #endregion


    }

}
