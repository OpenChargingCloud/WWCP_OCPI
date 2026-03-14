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

            if (cpo1CommonAPI_v2_1_1 is null)
            {
                Assert.Fail("cpo1CommonAPI_v2_1_1 is null!");
                return;
            }



            cpo1CommonHTTPAPI.HTTPBaseAPI.HTTPServer.AddPipeline(
                new OCPIv2_1_1.OCPIRewritePipeline(cpo1CommonAPI_v2_1_1)
            );


            var headerName               = "X-Custom-Header";
            var randomValue              = RandomExtensions.RandomString(16);
            var onGetVersionsRequests    = new List<String>();

            var emsp1RemoteParty_AtCPO1  = cpo1CommonHTTPAPI.GetRemoteParty(
                                               RemoteParty_Id.From(
                                                   emsp1CommonAPI_v2_1_1.OurCountryCode,
                                                   emsp1CommonAPI_v2_1_1.OurPartyId,
                                                   Role.EMSP
                                               )
                                           );

            emsp1RemoteParty_AtCPO1?.LocalAccessInfos.FirstOrDefault()?.IN = new HTTPModifiers(
                                                                                 RequestModifier:  request => {
                                                                                                       var requestBuilder = request.AsBuilder();
                                                                                                       requestBuilder.Set(headerName, randomValue);
                                                                                                       return requestBuilder;
                                                                                                   }
                                                                             );

            cpo1CommonHTTPAPI.OnGetVersionsRequest.Add((timestamp,
                                                        httpAPI,
                                                        ocpiRequest,
                                                        cancellationToken) => {

                if (ocpiRequest.HTTPRequest.TryGetHeaderField(headerName, out var headerValue))
                    onGetVersionsRequests.Add(headerValue?.ToString() ?? "-");

                return Task.CompletedTask;

            });


            var graphDefinedCPO1 = emsp1CommonAPI_v2_1_1.GetCommonClient(
                                       RemoteVersionsURL:  cpo1CommonHTTPAPI.OurVersionsURL
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersions();

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
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


            var headerName               = "X-Custom-Header";
            var randomValue              = RandomExtensions.RandomString(16);
            var onGetVersionsRequests    = new List<String>();

            var emsp1RemoteParty_AtCPO1  = cpo1CommonHTTPAPI.GetRemoteParty(
                                               RemoteParty_Id.From(
                                                   emsp1CommonAPI_v2_2_1.DefaultPartyId,
                                                   Role.EMSP
                                               )
                                           );

            emsp1RemoteParty_AtCPO1?.LocalAccessInfos.FirstOrDefault()?.IN = new HTTPModifiers(
                                                                                 RequestModifier:  request => {
                                                                                                       var requestBuilder = request.AsBuilder();
                                                                                                       requestBuilder.Set(headerName, randomValue);
                                                                                                       return requestBuilder;
                                                                                                   }
                                                                             );

            cpo1CommonHTTPAPI.OnGetVersionsRequest.Add((timestamp,
                                                        httpAPI,
                                                        ocpiRequest,
                                                        cancellationToken) => {

                if (ocpiRequest.HTTPRequest.TryGetHeaderField(headerName, out var headerValue))
                    onGetVersionsRequests.Add(headerValue?.ToString() ?? "-");

                return Task.CompletedTask;

            });


            var graphDefinedCPO1 = emsp1EMSPAPI_v2_2_1?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersions();

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(onGetVersionsRequests.First(), Is.EqualTo(randomValue));

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

            if (cpo1CommonHTTPAPI is null)
            {
                Assert.Fail("cpo1CommonHTTPAPI is null!");
                return;
            }

            if (cpo1CommonAPI_v2_3_0 is null)
            {
                Assert.Fail("cpo1CommonAPI_v2_3_0 is null!");
                return;
            }

            if (emsp1CommonAPI_v2_3_0 is null)
            {
                Assert.Fail("emsp1CommonAPI_v2_3_0 is null!");
                return;
            }


            cpo1CommonHTTPAPI.HTTPBaseAPI.HTTPServer.AddPipeline(
                new OCPIv2_3_0.OCPIRewritePipeline(cpo1CommonAPI_v2_3_0)
            );


            var headerName               = "X-Custom-Header";
            var randomValue              = RandomExtensions.RandomString(16);
            var onGetVersionsRequests    = new List<String>();

            var emsp1RemoteParty_AtCPO1  = cpo1CommonHTTPAPI.GetRemoteParty(
                                               RemoteParty_Id.From(
                                                   emsp1CommonAPI_v2_3_0.DefaultPartyId,
                                                   Role.EMSP
                                               )
                                           );

            emsp1RemoteParty_AtCPO1?.LocalAccessInfos.FirstOrDefault()?.IN = new HTTPModifiers(
                                                                                 RequestModifier:  request => {
                                                                                                       var requestBuilder = request.AsBuilder();
                                                                                                       requestBuilder.Set(headerName, randomValue);
                                                                                                       return requestBuilder;
                                                                                                   }
                                                                             );

            cpo1CommonHTTPAPI.OnGetVersionsRequest.Add((timestamp,
                                                        httpAPI,
                                                        ocpiRequest,
                                                        cancellationToken) => {

                if (ocpiRequest.HTTPRequest.TryGetHeaderField(headerName, out var headerValue))
                    onGetVersionsRequests.Add(headerValue?.ToString() ?? "-");

                return Task.CompletedTask;

            });


            var graphDefinedCPO1 = emsp1EMSPAPI_v2_3_0?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetVersions();

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(onGetVersionsRequests.First(), Is.EqualTo(randomValue));

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




        #region GetLocations_v2_1_1_fromCPO1_Test1()

        /// <summary>
        /// CPO #1 OCPI v2.1.1 Locations as Open Data!
        /// </summary>
        [Test]
        public async Task GetLocations_v2_1_1_fromCPO1_Test1()
        {

            if (cpo1CommonHTTPAPI is null)
            {
                Assert.Fail("cpo1CommonHTTPAPI is null!");
                return;
            }

            if (cpo1CommonAPI_v2_1_1 is null)
            {
                Assert.Fail("cpo1CommonAPI_v2_1_1 is null!");
                return;
            }

            if (cpo1CPOAPI_v2_1_1 is null)
            {
                Assert.Fail("cpo1CPOAPI_v2_1_1 is null!");
                return;
            }

            if (emsp1CommonAPI_v2_1_1 is null)
            {
                Assert.Fail("emsp1CommonAPI_v2_1_1 is null!");
                return;
            }


            cpo1CommonHTTPAPI.HTTPBaseAPI.HTTPServer.AddPipeline(
                new OCPIv2_1_1.OCPIRewritePipeline(cpo1CommonAPI_v2_1_1)
            );


            var headerName                      = "X-Custom-Header";
            var randomValue                     = RandomExtensions.RandomString(16);
            var onGetLocationsHTTPRequests      = new List<String>();

            var emsp1RemoteParty_AtCPO1_v2_1_1  = cpo1CommonAPI_v2_1_1.GetRemoteParty(
                                                      RemoteParty_Id.From(
                                                          emsp1CommonAPI_v2_1_1.OurCountryCode,
                                                          emsp1CommonAPI_v2_1_1.OurPartyId,
                                                          Role.EMSP
                                                      )
                                                  );

            emsp1RemoteParty_AtCPO1_v2_1_1?.LocalAccessInfos.FirstOrDefault()?.IN = new HTTPModifiers(
                                                                                        RequestModifier:  request => {
                                                                                                              var requestBuilder = request.AsBuilder();
                                                                                                              requestBuilder.Set(headerName, randomValue);
                                                                                                              return requestBuilder;
                                                                                                          }
                                                                                    );

            cpo1CPOAPI_v2_1_1.OnGetLocationsHTTPRequest.Add(
                (timestamp,
                 httpAPI,
                 ocpiRequest,
                 cancellationToken) => {

                     if (ocpiRequest.HTTPRequest.TryGetHeaderField(headerName, out var headerValue))
                         onGetLocationsHTTPRequests.Add(headerValue?.ToString() ?? "-");

                     return Task.CompletedTask;

                 }
            );


            var graphDefinedCPO1 = emsp1EMSPAPI_v2_1_1?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetLocations();

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(onGetLocationsHTTPRequests.First(), Is.EqualTo(randomValue));

            }

        }

        #endregion


        #region GetLocations_v2_2_1_fromCPO1_Test1()

        /// <summary>
        /// CPO #1 OCPI v2.2.1 Locations as Open Data!
        /// </summary>
        [Test]
        public async Task GetLocations_v2_2_1_fromCPO1_Test1()
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

            if (cpo1CPOAPI_v2_2_1 is null)
            {
                Assert.Fail("cpo1CPOAPI_v2_2_1 is null!");
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


            var headerName                      = "X-Custom-Header";
            var randomValue                     = RandomExtensions.RandomString(16);
            var onGetLocationsHTTPRequests      = new List<String>();

            var emsp1RemoteParty_AtCPO1_v2_2_1  = cpo1CommonAPI_v2_2_1.GetRemoteParty(
                                                      RemoteParty_Id.From(
                                                          emsp1CommonAPI_v2_2_1.DefaultPartyId,
                                                          Role.EMSP
                                                      )
                                                  );

            emsp1RemoteParty_AtCPO1_v2_2_1?.LocalAccessInfos.FirstOrDefault()?.IN = new HTTPModifiers(
                                                                                        RequestModifier:  request => {
                                                                                                              var requestBuilder = request.AsBuilder();
                                                                                                              requestBuilder.Set(headerName, randomValue);
                                                                                                              return requestBuilder;
                                                                                                          }
                                                                                    );

            cpo1CPOAPI_v2_2_1.HTTPEvents.OnGetLocationsHTTPRequest.Add(
                (timestamp,
                 httpAPI,
                 ocpiRequest,
                 cancellationToken) => {

                     if (ocpiRequest.HTTPRequest.TryGetHeaderField(headerName, out var headerValue))
                         onGetLocationsHTTPRequests.Add(headerValue?.ToString() ?? "-");

                     return Task.CompletedTask;

                 }
            );


            var graphDefinedCPO1 = emsp1EMSPAPI_v2_2_1?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetLocations();

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(onGetLocationsHTTPRequests.First(), Is.EqualTo(randomValue));

            }

        }

        #endregion

        #region GetLocations_v2_3_0_fromCPO1_Test1()

        /// <summary>
        /// CPO #1 OCPI v2.3.0 Locations as Open Data!
        /// </summary>
        [Test]
        public async Task GetLocations_v2_3_0_fromCPO1_Test1()
        {

            if (cpo1CommonHTTPAPI is null)
            {
                Assert.Fail("cpo1CommonHTTPAPI is null!");
                return;
            }

            if (cpo1CommonAPI_v2_3_0 is null)
            {
                Assert.Fail("cpo1CommonAPI_v2_3_0 is null!");
                return;
            }

            if (cpo1CPOAPI_v2_3_0 is null)
            {
                Assert.Fail("cpo1CPOAPI_v2_3_0 is null!");
                return;
            }

            if (emsp1CommonAPI_v2_3_0 is null)
            {
                Assert.Fail("emsp1CommonAPI_v2_3_0 is null!");
                return;
            }


            cpo1CommonHTTPAPI.HTTPBaseAPI.HTTPServer.AddPipeline(
                new OCPIv2_3_0.OCPIRewritePipeline(cpo1CommonAPI_v2_3_0)
            );


            var headerName                      = "X-Custom-Header";
            var randomValue                     = RandomExtensions.RandomString(16);
            var onGetLocationsHTTPRequests      = new List<String>();

            var emsp1RemoteParty_AtCPO1_v2_3_0  = cpo1CommonAPI_v2_3_0.GetRemoteParty(
                                                      RemoteParty_Id.From(
                                                          emsp1CommonAPI_v2_3_0.DefaultPartyId,
                                                          Role.EMSP
                                                      )
                                                  );

            emsp1RemoteParty_AtCPO1_v2_3_0?.LocalAccessInfos.FirstOrDefault()?.IN = new HTTPModifiers(
                                                                                        RequestModifier:  request => {
                                                                                                              var requestBuilder = request.AsBuilder();
                                                                                                              requestBuilder.Set(headerName, randomValue);
                                                                                                              return requestBuilder;
                                                                                                          }
                                                                                    );

            cpo1CPOAPI_v2_3_0.HTTPEvents.OnGetLocationsHTTPRequest.Add(
                (timestamp,
                 httpAPI,
                 ocpiRequest,
                 cancellationToken) => {

                     if (ocpiRequest.HTTPRequest.TryGetHeaderField(headerName, out var headerValue))
                         onGetLocationsHTTPRequests.Add(headerValue?.ToString() ?? "-");

                     return Task.CompletedTask;

                 }
            );


            var graphDefinedCPO1 = emsp1EMSPAPI_v2_3_0?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GEF")
                                   );

            Assert.That(graphDefinedCPO1, Is.Not.Null);

            if (graphDefinedCPO1 is not null)
            {

                var response = await graphDefinedCPO1.GetLocations();

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(200),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                Assert.That(onGetLocationsHTTPRequests.First(), Is.EqualTo(randomValue));

            }

        }

        #endregion


    }

}
