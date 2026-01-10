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
    /// Testing the OCPI GetVersionDetails method(s) using HTTP Request Modifiers.
    /// </summary>
    [TestFixture]
    public class GetVersions_UsingRequestModifiers_Tests : A_2CPOs2EMSPs_TestDefaults
    {

        #region GetVersions_v2_1_1_fromCPO1_Test1()

        /// <summary>
        /// CPO #1 OCPI v2.1.1 versions as Open Data!
        /// </summary>
        [Test]
        public async Task GetVersions_v2_1_1_fromCPO1_Test1()
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

                var response = await graphDefinedCPO1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          127.0.0.1:3301
                // User-Agent:                    GraphDefined OCPI v2.1.1 CommonClient
                // Connection:                    close
                // X-Request-ID:                  64UC9vv67xSj258nz4YprUGth3W514
                // X-Correlation-ID:              rCtE9d3G1U72bdW7ArtUht9d8fj8Y1

                // HTTP/1.1 200 OK
                // Date:                          Fri, 10 Jan 2025 22:49:05 GMT
                // Server:                        GraphDefined OCPI v2.1.1 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                271
                // X-Request-ID:                  64UC9vv67xSj258nz4YprUGth3W514
                // X-Correlation-ID:              rCtE9d3G1U72bdW7ArtUht9d8fj8Y1
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
                //             "version": "2.3.0",
                //             "url":     "http://localhost:3301/ocpi/versions/2.3.0"
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

        #region GetVersions_v2_2_1_fromCPO1_Test1()

        /// <summary>
        /// CPO #1 OCPI v2.2.1 versions as Open Data!
        /// </summary>
        [Test]
        public async Task GetVersions_v2_2_1_fromCPO1_Test1()
        {

            if (cpo1CommonHTTPAPI is null)
            {
                Assert.Fail("cpo1CommonHTTPAPI is null!");
                return;
            }

            if (cpo1CommonAPI_v2_2_1 is null)
            {
                Assert.Fail("cpo1CommonAPI_v2_2_1 is null!");
                return;
            }

            if (emsp1CommonAPI_v2_2_1 is null)
            {
                Assert.Fail("emsp1CommonAPI_v2_2_1 is null!");
                return;
            }


            cpo1CommonHTTPAPI.HTTPBaseAPI.HTTPServer.AddPipeline(
                new OCPIv2_2_1.OCPIRewritePipeline(cpo1CommonAPI_v2_2_1)
            );


            var emsp1RemoteParty_AtCPO1         = cpo1CommonHTTPAPI.GetRemoteParty(
                                                      RemoteParty_Id.From(
                                                          emsp1CommonAPI_v2_2_1.DefaultPartyId,
                                                          Role.EMSP
                                                      )
                                                  );

            emsp1RemoteParty_AtCPO1?.IN         = new RemoteParty.IOModifiers() {
                                                      RequestModifier = request => {
                                                          var requestBuilder = request.AsBuilder();
                                                          requestBuilder.Set("Accept", "application/xml");
                                                          return requestBuilder;
                                                      }
                                                      //ResponseModifier = response =>
                                                      //{
                                                      //    // Modify the HTTP response here
                                                      //    // For example, log the response
                                                      //    Console.WriteLine("Received response with status code: " + response.HTTPStatusCode);
                                                      //    return response;
                                                      //}
                                                  };


            var emsp1RemoteParty_AtCPO1_v2_2_1  = cpo1CommonAPI_v2_2_1.GetRemoteParty(
                                                      RemoteParty_Id.From(
                                                          emsp1CommonAPI_v2_2_1.DefaultPartyId,
                                                          Role.EMSP
                                                      )
                                                  );

            emsp1RemoteParty_AtCPO1_v2_2_1?.IN  = new RemoteParty.IOModifiers() {
                                                      RequestModifier = request => {
                                                          var requestBuilder = request.AsBuilder();
                                                          requestBuilder.Set("X-Custom-Header", "CustomValue");
                                                          return requestBuilder;
                                                      }
                                                      //ResponseModifier = response =>
                                                      //{
                                                      //    // Modify the HTTP response here
                                                      //    // For example, log the response
                                                      //    Console.WriteLine("Received response with status code: " + response.HTTPStatusCode);
                                                      //    return response;
                                                      //}
                                                  };


            if (cpo1CommonHTTPAPI is null)
            {
                Assert.Fail("cpo1CommonHTTPAPI is null!");
                return;
            }




            var graphDefinedCPO1 = emsp1EMSPAPI_v2_2_1?.GetCPOClient(
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

        #region GetVersions_v2_3_0_fromCPO1_Test1()

        /// <summary>
        /// CPO #1 OCPI v2.3.0 versions as Open Data!
        /// </summary>
        [Test]
        public async Task GetVersions_v2_3_0_fromCPO1_Test1()
        {

            if (emsp1CommonAPI_v2_3_0 is null)
            {
                Assert.Fail("emsp1CommonAPI_v2_3_0 is null!");
                return;
            }

            if (cpo1CommonHTTPAPI is null)
            {
                Assert.Fail("cpo1BaseAPI is null!");
                return;
            }

            var graphDefinedCPO1 = emsp1CommonAPI_v2_3_0.GetCommonClient(
                                       RemoteVersionsURL:  cpo1CommonHTTPAPI.OurVersionsURL
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersions();

                // GET /ocpi/versions HTTP/1.1
                // Accept:                        application/json; charset=utf-8; q=1
                // Host:                          127.0.0.1:3301
                // User-Agent:                    GraphDefined OCPI v2.3 CommonClient
                // Connection:                    close
                // X-Request-ID:                  64UC9vv67xSj258nz4YprUGth3W514
                // X-Correlation-ID:              rCtE9d3G1U72bdW7ArtUht9d8fj8Y1

                // HTTP/1.1 200 OK
                // Date:                          Fri, 10 Jan 2025 22:49:05 GMT
                // Server:                        GraphDefined OCPI v2.3.0 Common HTTP API
                // Access-Control-Allow-Origin:   *
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Allow:                         OPTIONS, GET
                // Vary:                          Accept
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                271
                // X-Request-ID:                  64UC9vv67xSj258nz4YprUGth3W514
                // X-Correlation-ID:              rCtE9d3G1U72bdW7ArtUht9d8fj8Y1
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
                //             "version": "2.3.0",
                //             "url":     "http://localhost:3301/ocpi/versions/2.3.0"
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
