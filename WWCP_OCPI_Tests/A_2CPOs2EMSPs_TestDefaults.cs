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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPI;

using cloud.charging.open.protocols.OCPIv2_1_1;
using cloud.charging.open.protocols.OCPIv2_1_1.HTTP;
using cloud.charging.open.protocols.OCPIv2_1_1.WebAPI;

using cloud.charging.open.protocols.OCPIv2_2_1;
using cloud.charging.open.protocols.OCPIv2_2_1.HTTP;
using cloud.charging.open.protocols.OCPIv2_2_1.WebAPI;

using cloud.charging.open.protocols.OCPIv2_3;
using cloud.charging.open.protocols.OCPIv2_3.HTTP;
using cloud.charging.open.protocols.OCPIv2_3.WebAPI;

using cloud.charging.open.protocols.OCPIv3_0;
using cloud.charging.open.protocols.OCPIv3_0.HTTP;
using cloud.charging.open.protocols.OCPIv3_0.WebAPI;

#endregion

namespace cloud.charging.open.protocols.OCPI.UnitTests
{

    public static class TestHelpers
    {

        #region OptionsRequest     (RemoteURL, Token = null)

        public static async Task<HTTPResponse> OptionsRequest(URL      RemoteURL,
                                                              String?  Token = null)

            => await new HTTPClient(RemoteURL).
                         Execute(client => client.CreateRequest(HTTPMethod.OPTIONS,
                                                                RemoteURL.Path,
                                                                RequestBuilder: requestBuilder => {

                                                                    if (Token is not null && Token.IsNotNullOrEmpty())
                                                                        requestBuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                    requestBuilder.Set("X-Request-ID",      "1234");
                                                                    requestBuilder.Set("X-Correlation-ID",  "5678");

                                                                })).
                         ConfigureAwait(false);

        #endregion


        #region GetJSONRequest     (RemoteURL, Token = null)

        public static async Task<HTTPResponse<JObject>> GetJSONRequest(URL      RemoteURL,
                                                                       String?  Token = null)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     RemoteURL.Path,
                                                                                     RequestBuilder: requestBuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestBuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                         requestBuilder.Set("X-Request-ID",      "1234");
                                                                                         requestBuilder.Set("X-Correlation-ID",  "5678");

                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<JObject>(httpResponse,
                                             JObject.Parse(httpResponse.HTTPBodyAsUTF8String ?? "{}"));

        }

        #endregion

        #region GetJSONArrayRequest(RemoteURL, Token = null)

        public static async Task<HTTPResponse<JArray>> GetJSONArrayRequest(URL      RemoteURL,
                                                                           String?  Token = null)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     RemoteURL.Path,
                                                                                     RequestBuilder: requestBuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestBuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                         requestBuilder.Set("X-Request-ID",      "1234");
                                                                                         requestBuilder.Set("X-Correlation-ID",  "5678");

                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<JArray>(httpResponse,
                                            JArray.Parse(httpResponse.HTTPBodyAsUTF8String ?? "[]"));

        }

        #endregion

        #region GetHTMLRequest     (RemoteURL, Token = null)

        public static async Task<HTTPResponse<String>> GetHTMLRequest(URL      RemoteURL,
                                                                      String?  Token = null)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     RemoteURL.Path,
                                                                                     RequestBuilder: requestBuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestBuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestBuilder.Accept.Add(HTTPContentType.Text.HTML_UTF8);
                                                                                         requestBuilder.Set("X-Request-ID",      "1234");
                                                                                         requestBuilder.Set("X-Correlation-ID",  "5678");

                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<String>(httpResponse,
                                            httpResponse.HTTPBodyAsUTF8String ?? "");

        }

        #endregion

        #region GetTextRequest     (RemoteURL, Token = null)

        public static async Task<HTTPResponse<String>> GetTextRequest(URL      RemoteURL,
                                                                      String?  Token = null)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     RemoteURL.Path,
                                                                                     RequestBuilder: requestBuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestBuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestBuilder.Accept.Add(HTTPContentType.Text.PLAIN);
                                                                                         requestBuilder.Set("X-Request-ID",      "1234");
                                                                                         requestBuilder.Set("X-Correlation-ID",  "5678");

                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<String>(httpResponse,
                                            httpResponse.HTTPBodyAsUTF8String ?? "");

        }

        #endregion


        #region DeleteJSONRequest  (RemoteURL, Token = null)

        public static async Task<HTTPResponse<JObject>> DeleteJSONRequest(URL      RemoteURL,
                                                                          String?  Token = null)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(HTTPMethod.DELETE,
                                                                                     RemoteURL.Path,
                                                                                     RequestBuilder: requestBuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestBuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                         requestBuilder.Set("X-Request-ID",      "1234");
                                                                                         requestBuilder.Set("X-Correlation-ID",  "5678");

                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<JObject>(httpResponse,
                                             JObject.Parse(httpResponse.HTTPBodyAsUTF8String ?? "{}"));

        }

        #endregion


        #region PutJSONRequest     (RemoteURL, JSON, Token = null)

        public static async Task<HTTPResponse<JObject>> PutJSONRequest(URL      RemoteURL,
                                                                       JObject  JSON,
                                                                       String?  Token = null)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(HTTPMethod.PUT,
                                                                                     RemoteURL.Path,
                                                                                     RequestBuilder: requestBuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestBuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestBuilder.ContentType    = HTTPContentType.Application.JSON_UTF8;
                                                                                         requestBuilder.Content        = JSON.ToUTF8Bytes();
                                                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                         requestBuilder.Set("X-Request-ID",      "1234");
                                                                                         requestBuilder.Set("X-Correlation-ID",  "5678");

                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<JObject>(httpResponse,
                                             JObject.Parse(httpResponse.HTTPBodyAsUTF8String ?? "{}"));

        }

        #endregion

        #region PostJSONRequest    (RemoteURL, JSON, Token = null)

        public static async Task<HTTPResponse<JObject>> PostJSONRequest(URL     RemoteURL,
                                                                       JObject  JSON,
                                                                       String?  Token = null)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                     RemoteURL.Path,
                                                                                     RequestBuilder: requestBuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestBuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestBuilder.ContentType    = HTTPContentType.Application.JSON_UTF8;
                                                                                         requestBuilder.Content        = JSON.ToUTF8Bytes();
                                                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                         requestBuilder.Set("X-Request-ID",      "1234");
                                                                                         requestBuilder.Set("X-Correlation-ID",  "5678");

                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<JObject>(httpResponse,
                                             JObject.Parse(httpResponse.HTTPBodyAsUTF8String ?? "{}"));

        }

        #endregion

    }


    /// <summary>
    /// OCPI test defaults for tests with 2 CPOs and 2 EMSPs.
    /// </summary>
    public abstract class A_2CPOs2EMSPs_TestDefaults
    {

        #region Data

        #region CPO #1

        protected       HTTPAPI?                                                      cpo1HTTPAPI;
        public          URL?                                                          cpo1VersionsAPIURL;
        protected       CommonBaseAPI?                                                cpo1BaseAPI;

        protected       OCPIv2_1_1.HTTP.CommonAPI?                                    cpo1CommonAPI_v2_1_1;
        protected       OCPIv2_1_1.WebAPI.OCPIWebAPI?                                 cpo1WebAPI_v2_1_1;
        protected       OCPIv2_1_1.HTTP.CPOAPI?                                       cpo1CPOAPI_v2_1_1;
        protected       OCPIv2_1_1.OCPICSOAdapter?                                    cpo1Adapter_v2_1_1;
        protected       ConcurrentDictionary<DateTime, OCPIv2_1_1.HTTP.OCPIRequest>   cpo1APIRequestLogs_v2_1_1  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv2_1_1.OCPIResponse>       cpo1APIResponseLogs_v2_1_1 = [];

        protected       OCPIv2_2_1.HTTP.CommonAPI?                                    cpo1CommonAPI_v2_2_1;
        protected       OCPIv2_2_1.WebAPI.OCPIWebAPI?                                 cpo1WebAPI_v2_2_1;
        protected       OCPIv2_2_1.HTTP.CPOAPI?                                       cpo1CPOAPI_v2_2_1;
        protected       OCPIv2_2_1.OCPICSOAdapter?                                    cpo1Adapter_v2_2_1;
        protected       ConcurrentDictionary<DateTime, OCPIv2_2_1.HTTP.OCPIRequest>   cpo1APIRequestLogs_v2_2_1  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv2_2_1.OCPIResponse>       cpo1APIResponseLogs_v2_2_1 = [];

        protected       OCPIv2_3.HTTP.CommonAPI?                                      cpo1CommonAPI_v2_3;
        protected       OCPIv2_3.WebAPI.OCPIWebAPI?                                   cpo1WebAPI_v2_3;
        protected       OCPIv2_3.HTTP.CPOAPI?                                         cpo1CPOAPI_v2_3;
        protected       OCPIv2_3.OCPICSOAdapter?                                      cpo1Adapter_v2_3;
        protected       ConcurrentDictionary<DateTime, OCPIv2_3.HTTP.OCPIRequest>     cpo1APIRequestLogs_v2_3  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv2_3.OCPIResponse>         cpo1APIResponseLogs_v2_3 = [];

        protected       OCPIv3_0.HTTP.CommonAPI?                                      cpo1CommonAPI_v3_0;
        protected       OCPIv3_0.WebAPI.OCPIWebAPI?                                   cpo1WebAPI_v3_0;
        protected       OCPIv3_0.HTTP.CPOAPI?                                         cpo1CPOAPI_v3_0;
        protected       OCPIv3_0.OCPICSOAdapter?                                      cpo1Adapter_v3_0;
        protected       ConcurrentDictionary<DateTime, OCPIv3_0.HTTP.OCPIRequest>     cpo1APIRequestLogs_v3_0  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv3_0.OCPIResponse>         cpo1APIResponseLogs_v3_0 = [];

        protected const String                                                        cpo1_accessing_emsp1__token   = "cpo1_accessing_emsp1++token";
        protected const String                                                        cpo1_accessing_emsp2__token   = "cpo1_accessing_emsp2++token";

        #endregion

        #region CPO #2

        protected       HTTPAPI?                                                      cpo2HTTPAPI;
        public          URL?                                                          cpo2VersionsAPIURL;
        protected       CommonBaseAPI?                                                cpo2BaseAPI;

        protected       OCPIv2_1_1.HTTP.CommonAPI?                                    cpo2CommonAPI_v2_1_1;
        protected       OCPIv2_1_1.WebAPI.OCPIWebAPI?                                 cpo2WebAPI_v2_1_1;
        protected       OCPIv2_1_1.HTTP.CPOAPI?                                       cpo2CPOAPI_v2_1_1;
        protected       OCPIv2_1_1.OCPICSOAdapter?                                    cpo2Adapter_v2_1_1;
        protected       ConcurrentDictionary<DateTime, OCPIv2_1_1.HTTP.OCPIRequest>   cpo2APIRequestLogs_v2_1_1  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv2_1_1.OCPIResponse>       cpo2APIResponseLogs_v2_1_1 = [];

        protected       OCPIv2_2_1.HTTP.CommonAPI?                                    cpo2CommonAPI_v2_2_1;
        protected       OCPIv2_2_1.WebAPI.OCPIWebAPI?                                 cpo2WebAPI_v2_2_1;
        protected       OCPIv2_2_1.HTTP.CPOAPI?                                       cpo2CPOAPI_v2_2_1;
        protected       OCPIv2_2_1.OCPICSOAdapter?                                    cpo2Adapter_v2_2_1;
        protected       ConcurrentDictionary<DateTime, OCPIv2_2_1.HTTP.OCPIRequest>   cpo2APIRequestLogs_v2_2_1  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv2_2_1.OCPIResponse>       cpo2APIResponseLogs_v2_2_1 = [];

        protected       OCPIv2_3.HTTP.CommonAPI?                                    cpo2CommonAPI_v2_3;
        protected       OCPIv2_3.WebAPI.OCPIWebAPI?                                 cpo2WebAPI_v2_3;
        protected       OCPIv2_3.HTTP.CPOAPI?                                       cpo2CPOAPI_v2_3;
        protected       OCPIv2_3.OCPICSOAdapter?                                    cpo2Adapter_v2_3;
        protected       ConcurrentDictionary<DateTime, OCPIv2_3.HTTP.OCPIRequest>   cpo2APIRequestLogs_v2_3  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv2_3.OCPIResponse>       cpo2APIResponseLogs_v2_3 = [];

        protected       OCPIv3_0.HTTP.CommonAPI?                                      cpo2CommonAPI_v3_0;
        protected       OCPIv3_0.WebAPI.OCPIWebAPI?                                   cpo2WebAPI_v3_0;
        protected       OCPIv3_0.HTTP.CPOAPI?                                         cpo2CPOAPI_v3_0;
        protected       OCPIv3_0.OCPICSOAdapter?                                      cpo2Adapter_v3_0;
        protected       ConcurrentDictionary<DateTime, OCPIv3_0.HTTP.OCPIRequest>     cpo2APIRequestLogs_v3_0  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv3_0.OCPIResponse>         cpo2APIResponseLogs_v3_0 = [];

        protected const String                                                        cpo2_accessing_emsp1__token   = "cpo2_accessing_emsp1++token";
        protected const String                                                        cpo2_accessing_emsp2__token   = "cpo2_accessing_emsp2++token";

        #endregion

        #region EMSP #1

        protected       HTTPAPI?                                                      emsp1HTTPAPI;
        public          URL?                                                          emsp1VersionsAPIURL;
        protected       CommonBaseAPI?                                                emsp1BaseAPI;

        protected       OCPIv2_1_1.HTTP.CommonAPI?                                    emsp1CommonAPI_v2_1_1;
        protected       OCPIv2_1_1.WebAPI.OCPIWebAPI?                                 emsp1WebAPI_v2_1_1;
        protected       OCPIv2_1_1.HTTP.EMSPAPI?                                      emsp1EMSPAPI_v2_1_1;
        protected       OCPIv2_1_1.OCPIEMPAdapter?                                    emsp1Adapter_v2_1_1;
        protected       ConcurrentDictionary<DateTime, OCPIv2_1_1.HTTP.OCPIRequest>   emsp1APIRequestLogs_v2_1_1  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv2_1_1.OCPIResponse>       emsp1APIResponseLogs_v2_1_1 = [];

        protected       OCPIv2_2_1.HTTP.CommonAPI?                                    emsp1CommonAPI_v2_2_1;
        protected       OCPIv2_2_1.WebAPI.OCPIWebAPI?                                 emsp1WebAPI_v2_2_1;
        protected       OCPIv2_2_1.HTTP.EMSPAPI?                                      emsp1EMSPAPI_v2_2_1;
        protected       OCPIv2_2_1.OCPIEMPAdapter?                                    emsp1Adapter_v2_2_1;
        protected       ConcurrentDictionary<DateTime, OCPIv2_2_1.HTTP.OCPIRequest>   emsp1APIRequestLogs_v2_2_1  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv2_2_1.OCPIResponse>       emsp1APIResponseLogs_v2_2_1 = [];

        protected       OCPIv2_3.HTTP.CommonAPI?                                      emsp1CommonAPI_v2_3;
        protected       OCPIv2_3.WebAPI.OCPIWebAPI?                                   emsp1WebAPI_v2_3;
        protected       OCPIv2_3.HTTP.EMSPAPI?                                        emsp1EMSPAPI_v2_3;
        protected       OCPIv2_3.OCPIEMPAdapter?                                      emsp1Adapter_v2_3;
        protected       ConcurrentDictionary<DateTime, OCPIv2_3.HTTP.OCPIRequest>     emsp1APIRequestLogs_v2_3  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv2_3.OCPIResponse>         emsp1APIResponseLogs_v2_3 = [];

        protected       OCPIv3_0.HTTP.CommonAPI?                                      emsp1CommonAPI_v3_0;
        protected       OCPIv3_0.WebAPI.OCPIWebAPI?                                   emsp1WebAPI_v3_0;
        protected       OCPIv3_0.HTTP.EMSPAPI?                                        emsp1EMSPAPI_v3_0;
        protected       OCPIv3_0.OCPIEMPAdapter?                                      emsp1Adapter_v3_0;
        protected       ConcurrentDictionary<DateTime, OCPIv3_0.HTTP.OCPIRequest>     emsp1APIRequestLogs_v3_0  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv3_0.OCPIResponse>         emsp1APIResponseLogs_v3_0 = [];

        protected const String                                                        emsp1_accessing_cpo1__token   = "emsp1_accessing_cpo1++token";
        protected const String                                                        emsp2_accessing_cpo1__token   = "emsp2_accessing_cpo2++token";

        #endregion

        #region EMSP #2

        protected       HTTPAPI?                                                      emsp2HTTPAPI;
        public          URL?                                                          emsp2VersionsAPIURL;
        protected       CommonBaseAPI?                                                emsp2BaseAPI;

        protected       OCPIv2_1_1.HTTP.CommonAPI?                                    emsp2CommonAPI_v2_1_1;
        protected       OCPIv2_1_1.WebAPI.OCPIWebAPI?                                 emsp2WebAPI_v2_1_1;
        protected       OCPIv2_1_1.HTTP.EMSPAPI?                                      emsp2EMSPAPI_v2_1_1;
        protected       OCPIv2_1_1.OCPIEMPAdapter?                                    emsp2Adapter_v2_1_1;
        protected       ConcurrentDictionary<DateTime, OCPIv2_1_1.HTTP.OCPIRequest>   emsp2APIRequestLogs_v2_1_1  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv2_1_1.OCPIResponse>       emsp2APIResponseLogs_v2_1_1 = [];

        protected       OCPIv2_2_1.HTTP.CommonAPI?                                    emsp2CommonAPI_v2_2_1;
        protected       OCPIv2_2_1.WebAPI.OCPIWebAPI?                                 emsp2WebAPI_v2_2_1;
        protected       OCPIv2_2_1.HTTP.EMSPAPI?                                      emsp2EMSPAPI_v2_2_1;
        protected       OCPIv2_2_1.OCPIEMPAdapter?                                    emsp2Adapter_v2_2_1;
        protected       ConcurrentDictionary<DateTime, OCPIv2_2_1.HTTP.OCPIRequest>   emsp2APIRequestLogs_v2_2_1  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv2_2_1.OCPIResponse>       emsp2APIResponseLogs_v2_2_1 = [];

        protected       OCPIv2_3.HTTP.CommonAPI?                                      emsp2CommonAPI_v2_3;
        protected       OCPIv2_3.WebAPI.OCPIWebAPI?                                   emsp2WebAPI_v2_3;
        protected       OCPIv2_3.HTTP.EMSPAPI?                                        emsp2EMSPAPI_v2_3;
        protected       OCPIv2_3.OCPIEMPAdapter?                                      emsp2Adapter_v2_3;
        protected       ConcurrentDictionary<DateTime, OCPIv2_3.HTTP.OCPIRequest>     emsp2APIRequestLogs_v2_3  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv2_3.OCPIResponse>         emsp2APIResponseLogs_v2_3 = [];

        protected       OCPIv3_0.HTTP.CommonAPI?                                      emsp2CommonAPI_v3_0;
        protected       OCPIv3_0.WebAPI.OCPIWebAPI?                                   emsp2WebAPI_v3_0;
        protected       OCPIv3_0.HTTP.EMSPAPI?                                        emsp2EMSPAPI_v3_0;
        protected       OCPIv3_0.OCPIEMPAdapter?                                      emsp2Adapter_v3_0;
        protected       ConcurrentDictionary<DateTime, OCPIv3_0.HTTP.OCPIRequest>     emsp2APIRequestLogs_v3_0  = [];
        protected       ConcurrentDictionary<DateTime, OCPIv3_0.OCPIResponse>         emsp2APIResponseLogs_v3_0 = [];

        protected const String                                                        emsp1_accessing_cpo2__token   = "emsp1_accessing_cpo1++token";
        protected const String                                                        emsp2_accessing_cpo2__token   = "emsp2_accessing_cpo2++token";

        #endregion

        protected const String  UnknownToken       = "UnknownUnknownUnknownToken";

        protected const String  BlockedCPOToken    = "blocked-cpo";
        protected const String  BlockedEMSPToken   = "blocked-emsp";

        #endregion

        #region Constructor(s)

        public A_2CPOs2EMSPs_TestDefaults()
        {

        }

        #endregion


        #region SetupOnce()

        [OneTimeSetUp]
        public async virtual Task SetupOnce()
        {

            #region Create cpo1/cpo2/emsp1/emsp2 HTTP APIs

            cpo1HTTPAPI           = new HTTPAPI(
                                        HTTPServerPort:  IPPort.Parse(3301),
                                        AutoStart:       true
                                    );

            cpo2HTTPAPI           = new HTTPAPI(
                                        HTTPServerPort:  IPPort.Parse(3302),
                                        AutoStart:       true
                                    );

            emsp1HTTPAPI          = new HTTPAPI(
                                        HTTPServerPort:  IPPort.Parse(3401),
                                        AutoStart:       true
                                    );

            emsp2HTTPAPI          = new HTTPAPI(
                                        HTTPServerPort:  IPPort.Parse(3402),
                                        AutoStart:       true
                                    );

            Assert.That(cpo1HTTPAPI,   Is.Not.Null);
            Assert.That(cpo2HTTPAPI,   Is.Not.Null);

            Assert.That(emsp1HTTPAPI,  Is.Not.Null);
            Assert.That(emsp2HTTPAPI,  Is.Not.Null);

            #endregion

            #region Create cpo1/cpo2/emsp1/emsp2 OCPI Base APIs

            #region CPO #1

            cpo1BaseAPI  = new CommonBaseAPI(

                               OurBaseURL:                  URL.Parse($"http://127.0.0.1:{cpo1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi"),
                               OurVersionsURL:              URL.Parse($"http://127.0.0.1:{cpo1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/versions"),
                               HTTPServer:                  cpo1HTTPAPI.HTTPServer,

                               AdditionalURLPathPrefix:     null,
                               LocationsAsOpenData:         true,
                               AllowDowngrades:             null,

                               HTTPHostname:                null,
                               ExternalDNSName:             null,
                               HTTPServiceName:             null,
                               BasePath:                    null,

                               URLPathPrefix:               HTTPPath.Parse("/ocpi"),
                               APIVersionHashes:            null,

                               DisableMaintenanceTasks:     true,
                               MaintenanceInitialDelay:     null,
                               MaintenanceEvery:            null,

                               DisableWardenTasks:          true,
                               WardenInitialDelay:          null,
                               WardenCheckEvery:            null,

                               IsDevelopment:               null,
                               DevelopmentServers:          null,
                               DisableLogging:              null,
                               LoggingContext:              null,
                               LoggingPath:                 null,
                               LogfileName:                 null,
                               LogfileCreator:              null,
                               AutoStart:                   true

                           );

            #endregion

            #region CPO #2

            cpo2BaseAPI  = new CommonBaseAPI(

                               OurBaseURL:                URL.Parse("http://127.0.0.1:3202/ocpi"),
                               OurVersionsURL:            URL.Parse("http://127.0.0.1:3202/ocpi/versions"),

                               HTTPServer:                cpo2HTTPAPI.HTTPServer,
                               AdditionalURLPathPrefix:   null,
                               //KeepRemovedEVSEs:          null,
                               LocationsAsOpenData:       true,
                               AllowDowngrades:           null,

                               HTTPHostname:              null,
                               ExternalDNSName:           null,
                               HTTPServiceName:           null,
                               BasePath:                  null,

                               URLPathPrefix:             HTTPPath.Parse("/ocpi"),
                               APIVersionHashes:          null,

                               DisableMaintenanceTasks:   null,
                               MaintenanceInitialDelay:   null,
                               MaintenanceEvery:          null,

                               DisableWardenTasks:        null,
                               WardenInitialDelay:        null,
                               WardenCheckEvery:          null,

                               IsDevelopment:             null,
                               DevelopmentServers:        null,
                               DisableLogging:            null,
                               LoggingContext:            null,
                               LoggingPath:               null,
                               LogfileName:               null,
                               LogfileCreator:            null

                           );

            #endregion

            #region EMSP #1

            emsp1BaseAPI = new CommonBaseAPI(

                               OurBaseURL:                URL.Parse("http://127.0.0.1:3401/ocpi"),
                               OurVersionsURL:            URL.Parse("http://127.0.0.1:3401/ocpi/versions"),

                               HTTPServer:                emsp1HTTPAPI.HTTPServer,
                               AdditionalURLPathPrefix:   null,
                               //KeepRemovedEVSEs:          null,
                               LocationsAsOpenData:       true,
                               AllowDowngrades:           null,

                               HTTPHostname:              null,
                               ExternalDNSName:           null,
                               HTTPServiceName:           null,
                               BasePath:                  null,

                               URLPathPrefix:             HTTPPath.Parse("/ocpi"),
                               APIVersionHashes:          null,

                               DisableMaintenanceTasks:   null,
                               MaintenanceInitialDelay:   null,
                               MaintenanceEvery:          null,

                               DisableWardenTasks:        null,
                               WardenInitialDelay:        null,
                               WardenCheckEvery:          null,

                               IsDevelopment:             null,
                               DevelopmentServers:        null,
                               DisableLogging:            null,
                               LoggingContext:            null,
                               LoggingPath:               null,
                               LogfileName:               null,
                               LogfileCreator:            null

                           );

            #endregion

            #region EMSP #2

            emsp2BaseAPI = new CommonBaseAPI(

                               OurBaseURL:                URL.Parse("http://127.0.0.1:3402/ocpi"),
                               OurVersionsURL:            URL.Parse("http://127.0.0.1:3402/ocpi/versions"),

                               HTTPServer:                emsp2HTTPAPI.HTTPServer,
                               AdditionalURLPathPrefix:   null,
                               //KeepRemovedEVSEs:          null,
                               LocationsAsOpenData:       true,
                               AllowDowngrades:           null,

                               HTTPHostname:              null,
                               ExternalDNSName:           null,
                               HTTPServiceName:           null,
                               BasePath:                  null,

                               URLPathPrefix:             HTTPPath.Parse("/ocpi"),
                               APIVersionHashes:          null,

                               DisableMaintenanceTasks:   null,
                               MaintenanceInitialDelay:   null,
                               MaintenanceEvery:          null,

                               DisableWardenTasks:        null,
                               WardenInitialDelay:        null,
                               WardenCheckEvery:          null,

                               IsDevelopment:             null,
                               DevelopmentServers:        null,
                               DisableLogging:            null,
                               LoggingContext:            null,
                               LoggingPath:               null,
                               LogfileName:               null,
                               LogfileCreator:            null

                           );

            #endregion

            Assert.That(cpo1BaseAPI,   Is.Not.Null);
            Assert.That(cpo2BaseAPI,   Is.Not.Null);

            Assert.That(emsp1BaseAPI,  Is.Not.Null);
            Assert.That(emsp2BaseAPI,  Is.Not.Null);

            #endregion

            #region Create cpo1/cpo2/emsp1/emsp2 OCPI v2.1.1 Common APIs

            // Clean up log and databade directories...
            foreach (var filePath in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory,
                                                                     HTTPAPI.DefaultHTTPAPI_LoggingPath),
                                                        $"GraphDefined_OCPI*.log"))
            {
                File.Delete(filePath);
            }


            #region CPO #1

            cpo1CommonAPI_v2_1_1  = new OCPIv2_1_1.HTTP.CommonAPI(

                                        OurBusinessDetails:                  new BusinessDetails(
                                                                                 "GraphDefined OCPI v2.1.1 CPO #1 Services",
                                                                                 URL.Parse("https://www.graphdefined.com/cpo1")
                                                                             ),
                                        OurCountryCode:                      CountryCode.Parse("DE"),
                                        OurPartyId:                          Party_Id.   Parse("GEF"),
                                        OurRole:                             Role.      CPO,

                                        BaseAPI:                             cpo1BaseAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,
                                        LocationsAsOpenData:                 true,
                                        AllowDowngrades:                     null,

                                        HTTPHostname:                        null,
                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DisableMaintenanceTasks:             null,
                                        MaintenanceInitialDelay:             null,
                                        MaintenanceEvery:                    null,

                                        DisableWardenTasks:                  null,
                                        WardenInitialDelay:                  null,
                                        WardenCheckEvery:                    null,

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      null,
                                        LoggingPath:                         null,
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_CPO1.log",
                                        LogfileCreator:                      null,
                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_RemoteParties_CPO1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_Assets_CPO1.log",
                                        AutoStart:                           false

                                    );

            cpo1CommonAPI_v2_2_1  = new OCPIv2_2_1.HTTP.CommonAPI(

                                        OurCredentialRoles:                  [
                                                                                 new OCPIv2_2_1.CredentialsRole(
                                                                                     CountryCode:       CountryCode.Parse("DE"),
                                                                                     PartyId:           Party_Id.   Parse("GEF"),
                                                                                     Role:              Role.CPO,
                                                                                     BusinessDetails:   new BusinessDetails(
                                                                                                            "GraphDefined OCPI v2.2.1 CPO #1 Services",
                                                                                                            URL.Parse("https://www.graphdefined.com/cpo1")
                                                                                                        ),
                                                                                     AllowDowngrades:   true
                                                                                 )
                                                                             ],
                                        DefaultCountryCode:                  CountryCode.Parse("DE"),
                                        DefaultPartyId:                      Party_Id.   Parse("GEF"),

                                        BaseAPI:                             cpo1BaseAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,
                                        LocationsAsOpenData:                 true,
                                        AllowDowngrades:                     null,

                                        HTTPHostname:                        null,
                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DisableMaintenanceTasks:             null,
                                        MaintenanceInitialDelay:             null,
                                        MaintenanceEvery:                    null,

                                        DisableWardenTasks:                  null,
                                        WardenInitialDelay:                  null,
                                        WardenCheckEvery:                    null,

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      null,
                                        LoggingPath:                         null,
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_CPO1.log",
                                        LogfileCreator:                      null,
                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_RemoteParties_CPO1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_Assets_CPO1.log",
                                        AutoStart:                           false

                                    );

            cpo1CommonAPI_v2_3  = new OCPIv2_3.HTTP.CommonAPI(

                                        OurCredentialRoles:                  [
                                                                                 new OCPIv2_3.CredentialsRole(
                                                                                     CountryCode:       CountryCode.Parse("DE"),
                                                                                     PartyId:           Party_Id.   Parse("GEF"),
                                                                                     Role:              Role.CPO,
                                                                                     BusinessDetails:   new BusinessDetails(
                                                                                                            "GraphDefined OCPI v2.3.0 CPO #1 Services",
                                                                                                            URL.Parse("https://www.graphdefined.com/cpo1")
                                                                                                        ),
                                                                                     AllowDowngrades:   true
                                                                                 )
                                                                             ],
                                        DefaultCountryCode:                  CountryCode.Parse("DE"),
                                        DefaultPartyId:                      Party_Id.   Parse("GEF"),

                                        BaseAPI:                             cpo1BaseAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,
                                        LocationsAsOpenData:                 true,
                                        AllowDowngrades:                     null,

                                        HTTPHostname:                        null,
                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DisableMaintenanceTasks:             null,
                                        MaintenanceInitialDelay:             null,
                                        MaintenanceEvery:                    null,

                                        DisableWardenTasks:                  null,
                                        WardenInitialDelay:                  null,
                                        WardenCheckEvery:                    null,

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      null,
                                        LoggingPath:                         null,
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_3.Version.String}_CPO1.log",
                                        LogfileCreator:                      null,
                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_3.Version.String}_RemoteParties_CPO1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_3.Version.String}_Assets_CPO1.log",
                                        AutoStart:                           false

                                    );

            #endregion

            #region CPO #2

            cpo2CommonAPI_v2_1_1  = new OCPIv2_1_1.HTTP.CommonAPI(

                                        OurBusinessDetails:                  new BusinessDetails(
                                                                                 "GraphDefined OCPI v2.1.1 CPO #2 Services",
                                                                                 URL.Parse("https://www.graphdefined.com/cpo2")
                                                                             ),
                                        OurCountryCode:                      CountryCode.Parse("DE"),
                                        OurPartyId:                          Party_Id.   Parse("GE2"),
                                        OurRole:                             Role.      CPO,

                                        BaseAPI:                             cpo2BaseAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,
                                        LocationsAsOpenData:                 true,
                                        AllowDowngrades:                     null,

                                        HTTPHostname:                        null,
                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DisableMaintenanceTasks:             null,
                                        MaintenanceInitialDelay:             null,
                                        MaintenanceEvery:                    null,

                                        DisableWardenTasks:                  null,
                                        WardenInitialDelay:                  null,
                                        WardenCheckEvery:                    null,

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      null,
                                        LoggingPath:                         null,
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_CPO2.log",
                                        LogfileCreator:                      null,
                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_RemoteParties_CPO2.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_Assets_CPO2.log",
                                        AutoStart:                           false

                                    );

            cpo2CommonAPI_v2_2_1  = new OCPIv2_2_1.HTTP.CommonAPI(

                                        OurCredentialRoles:                  [
                                                                                 new OCPIv2_2_1.CredentialsRole(
                                                                                     CountryCode:       CountryCode.Parse("DE"),
                                                                                     PartyId:           Party_Id.   Parse("GEF"),
                                                                                     Role:              Role.CPO,
                                                                                     BusinessDetails:   new BusinessDetails(
                                                                                                            "GraphDefined OCPI v2.2.1 CPO #2 Services",
                                                                                                            URL.Parse("https://www.graphdefined.com/cpo2")
                                                                                                        ),
                                                                                     AllowDowngrades:   true
                                                                                 )
                                                                             ],
                                        DefaultCountryCode:                  CountryCode.Parse("DE"),
                                        DefaultPartyId:                      Party_Id.   Parse("GEF"),

                                        BaseAPI:                             cpo2BaseAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,
                                        LocationsAsOpenData:                 true,
                                        AllowDowngrades:                     null,

                                        HTTPHostname:                        null,
                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DisableMaintenanceTasks:             null,
                                        MaintenanceInitialDelay:             null,
                                        MaintenanceEvery:                    null,

                                        DisableWardenTasks:                  null,
                                        WardenInitialDelay:                  null,
                                        WardenCheckEvery:                    null,

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      null,
                                        LoggingPath:                         null,
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_CPO2.log",
                                        LogfileCreator:                      null,
                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_RemoteParties_CPO1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_Assets_CPO1.log",
                                        AutoStart:                           false

                                    );

            cpo2CommonAPI_v2_3  = new OCPIv2_3.HTTP.CommonAPI(

                                        OurCredentialRoles:                  [
                                                                                 new OCPIv2_3.CredentialsRole(
                                                                                     CountryCode:       CountryCode.Parse("DE"),
                                                                                     PartyId:           Party_Id.   Parse("GEF"),
                                                                                     Role:              Role.CPO,
                                                                                     BusinessDetails:   new BusinessDetails(
                                                                                                            "GraphDefined OCPI v2.3.0 CPO #2 Services",
                                                                                                            URL.Parse("https://www.graphdefined.com/cpo2")
                                                                                                        ),
                                                                                     AllowDowngrades:   true
                                                                                 )
                                                                             ],
                                        DefaultCountryCode:                  CountryCode.Parse("DE"),
                                        DefaultPartyId:                      Party_Id.   Parse("GEF"),

                                        BaseAPI:                             cpo2BaseAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,
                                        LocationsAsOpenData:                 true,
                                        AllowDowngrades:                     null,

                                        HTTPHostname:                        null,
                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DisableMaintenanceTasks:             null,
                                        MaintenanceInitialDelay:             null,
                                        MaintenanceEvery:                    null,

                                        DisableWardenTasks:                  null,
                                        WardenInitialDelay:                  null,
                                        WardenCheckEvery:                    null,

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      null,
                                        LoggingPath:                         null,
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_3.Version.String}_CPO2.log",
                                        LogfileCreator:                      null,
                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_3.Version.String}_RemoteParties_CPO2.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_3.Version.String}_Assets_CPO2.log",
                                        AutoStart:                           false

                                    );

            #endregion

            #region EMSP #1

            emsp1CommonAPI_v2_1_1 = new OCPIv2_1_1.HTTP.CommonAPI(

                                        OurBusinessDetails:                  new BusinessDetails(
                                                                                 "GraphDefined OCPI v2.1.1 EMSP #1 Services",
                                                                                 URL.Parse("https://www.graphdefined.com/emsp1")
                                                                             ),
                                        OurCountryCode:                      CountryCode.Parse("DE"),
                                        OurPartyId:                          Party_Id.   Parse("GDF"),
                                        OurRole:                             Role.      EMSP,

                                        BaseAPI:                             emsp1BaseAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,
                                        LocationsAsOpenData:                 true,
                                        AllowDowngrades:                     null,

                                        HTTPHostname:                        null,
                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DisableMaintenanceTasks:             null,
                                        MaintenanceInitialDelay:             null,
                                        MaintenanceEvery:                    null,

                                        DisableWardenTasks:                  null,
                                        WardenInitialDelay:                  null,
                                        WardenCheckEvery:                    null,

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      null,
                                        LoggingPath:                         null,
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_EMSP1.log",
                                        LogfileCreator:                      null,
                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_RemoteParties_EMSP1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_Assets_EMSP1.log",
                                        AutoStart:                           false

                                    );

            emsp1CommonAPI_v2_2_1 = new OCPIv2_2_1.HTTP.CommonAPI(

                                        OurCredentialRoles:                  [
                                                                                 new OCPIv2_2_1.CredentialsRole(
                                                                                     CountryCode:       CountryCode.Parse("DE"),
                                                                                     PartyId:           Party_Id.   Parse("GDF"),
                                                                                     Role:              Role.EMSP,
                                                                                     BusinessDetails:   new BusinessDetails(
                                                                                                            "GraphDefined OCPI v2.2.1 EMSP #1 Services",
                                                                                                            URL.Parse("https://www.graphdefined.com/emsp1")
                                                                                                        ),
                                                                                     AllowDowngrades:   true
                                                                                 )
                                                                             ],
                                        DefaultCountryCode:                  CountryCode.Parse("DE"),
                                        DefaultPartyId:                      Party_Id.   Parse("GDF"),

                                        BaseAPI:                             emsp1BaseAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,
                                        LocationsAsOpenData:                 true,
                                        AllowDowngrades:                     null,

                                        HTTPHostname:                        null,
                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DisableMaintenanceTasks:             null,
                                        MaintenanceInitialDelay:             null,
                                        MaintenanceEvery:                    null,

                                        DisableWardenTasks:                  null,
                                        WardenInitialDelay:                  null,
                                        WardenCheckEvery:                    null,

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      null,
                                        LoggingPath:                         null,
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_EMSP1.log",
                                        LogfileCreator:                      null,
                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_RemoteParties_EMSP1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_Assets_EMSP1.log",
                                        AutoStart:                           false

                                    );

            emsp1CommonAPI_v2_3 = new OCPIv2_3.HTTP.CommonAPI(

                                        OurCredentialRoles:                  [
                                                                                 new OCPIv2_3.CredentialsRole(
                                                                                     CountryCode:       CountryCode.Parse("DE"),
                                                                                     PartyId:           Party_Id.   Parse("GDF"),
                                                                                     Role:              Role.EMSP,
                                                                                     BusinessDetails:   new BusinessDetails(
                                                                                                            "GraphDefined OCPI v2.3 EMSP #1 Services",
                                                                                                            URL.Parse("https://www.graphdefined.com/emsp1")
                                                                                                        ),
                                                                                     AllowDowngrades:   true
                                                                                 )
                                                                             ],
                                        DefaultCountryCode:                  CountryCode.Parse("DE"),
                                        DefaultPartyId:                      Party_Id.   Parse("GDF"),

                                        BaseAPI:                             emsp1BaseAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,
                                        LocationsAsOpenData:                 true,
                                        AllowDowngrades:                     null,

                                        HTTPHostname:                        null,
                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DisableMaintenanceTasks:             null,
                                        MaintenanceInitialDelay:             null,
                                        MaintenanceEvery:                    null,

                                        DisableWardenTasks:                  null,
                                        WardenInitialDelay:                  null,
                                        WardenCheckEvery:                    null,

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      null,
                                        LoggingPath:                         null,
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_3.Version.String}_EMSP1.log",
                                        LogfileCreator:                      null,
                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_3.Version.String}_RemoteParties_EMSP1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_3.Version.String}_Assets_EMSP1.log",
                                        AutoStart:                           false

                                    );

            #endregion

            #region EMSP #2

            emsp2CommonAPI_v2_1_1 = new OCPIv2_1_1.HTTP.CommonAPI(

                                        OurBusinessDetails:                  new BusinessDetails(
                                                                                 "GraphDefined EMSP #2 Services",
                                                                                 URL.Parse("https://www.graphdefined.com/emsp2")
                                                                             ),
                                        OurCountryCode:                      CountryCode.Parse("DE"),
                                        OurPartyId:                          Party_Id.   Parse("GD2"),
                                        OurRole:                             Role.      EMSP,

                                        BaseAPI:                             emsp2BaseAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,
                                        LocationsAsOpenData:                 true,
                                        AllowDowngrades:                     null,

                                        HTTPHostname:                        null,
                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DisableMaintenanceTasks:             null,
                                        MaintenanceInitialDelay:             null,
                                        MaintenanceEvery:                    null,

                                        DisableWardenTasks:                  null,
                                        WardenInitialDelay:                  null,
                                        WardenCheckEvery:                    null,

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      null,
                                        LoggingPath:                         null,
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_EMSP2.log",
                                        LogfileCreator:                      null,
                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_RemoteParties_EMSP2.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_Assets_EMSP2.log",
                                        AutoStart:                           false

                                    );

            emsp2CommonAPI_v2_2_1 = new OCPIv2_2_1.HTTP.CommonAPI(

                                        OurCredentialRoles:                  [
                                                                                 new OCPIv2_2_1.CredentialsRole(
                                                                                     CountryCode:       CountryCode.Parse("DE"),
                                                                                     PartyId:           Party_Id.   Parse("GDF"),
                                                                                     Role:              Role.EMSP,
                                                                                     BusinessDetails:   new BusinessDetails(
                                                                                                            "GraphDefined OCPI v2.2.1 EMSP #2 Services",
                                                                                                            URL.Parse("https://www.graphdefined.com/emsp2")
                                                                                                        ),
                                                                                     AllowDowngrades:   true
                                                                                 )
                                                                             ],
                                        DefaultCountryCode:                  CountryCode.Parse("DE"),
                                        DefaultPartyId:                      Party_Id.   Parse("GDF"),

                                        BaseAPI:                             emsp2BaseAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,
                                        LocationsAsOpenData:                 true,
                                        AllowDowngrades:                     null,

                                        HTTPHostname:                        null,
                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DisableMaintenanceTasks:             null,
                                        MaintenanceInitialDelay:             null,
                                        MaintenanceEvery:                    null,

                                        DisableWardenTasks:                  null,
                                        WardenInitialDelay:                  null,
                                        WardenCheckEvery:                    null,

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      null,
                                        LoggingPath:                         null,
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_EMSP1.log",
                                        LogfileCreator:                      null,
                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_RemoteParties_EMSP1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_Assets_EMSP1.log",
                                        AutoStart:                           false

                                    );

            emsp2CommonAPI_v2_3 = new OCPIv2_3.HTTP.CommonAPI(

                                        OurCredentialRoles:                  [
                                                                                 new OCPIv2_3.CredentialsRole(
                                                                                     CountryCode:       CountryCode.Parse("DE"),
                                                                                     PartyId:           Party_Id.   Parse("GDF"),
                                                                                     Role:              Role.EMSP,
                                                                                     BusinessDetails:   new BusinessDetails(
                                                                                                            "GraphDefined OCPI v2.3 EMSP #2 Services",
                                                                                                            URL.Parse("https://www.graphdefined.com/emsp2")
                                                                                                        ),
                                                                                     AllowDowngrades:   true
                                                                                 )
                                                                             ],
                                        DefaultCountryCode:                  CountryCode.Parse("DE"),
                                        DefaultPartyId:                      Party_Id.   Parse("GDF"),

                                        BaseAPI:                             emsp2BaseAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,
                                        LocationsAsOpenData:                 true,
                                        AllowDowngrades:                     null,

                                        HTTPHostname:                        null,
                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DisableMaintenanceTasks:             null,
                                        MaintenanceInitialDelay:             null,
                                        MaintenanceEvery:                    null,

                                        DisableWardenTasks:                  null,
                                        WardenInitialDelay:                  null,
                                        WardenCheckEvery:                    null,

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      null,
                                        LoggingPath:                         null,
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_3.Version.String}_EMSP1.log",
                                        LogfileCreator:                      null,
                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_3.Version.String}_RemoteParties_EMSP1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_3.Version.String}_Assets_EMSP1.log",
                                        AutoStart:                           false

                                    );

            #endregion

            Assert.That(cpo1CommonAPI_v2_1_1,   Is.Not.Null);
            Assert.That(cpo1CommonAPI_v2_2_1,   Is.Not.Null);
            Assert.That(cpo1CommonAPI_v2_3,     Is.Not.Null);

            Assert.That(cpo2CommonAPI_v2_1_1,   Is.Not.Null);
            Assert.That(cpo2CommonAPI_v2_2_1,   Is.Not.Null);
            Assert.That(cpo2CommonAPI_v2_3,     Is.Not.Null);

            Assert.That(emsp1CommonAPI_v2_1_1,  Is.Not.Null);
            Assert.That(emsp1CommonAPI_v2_2_1,  Is.Not.Null);
            Assert.That(emsp1CommonAPI_v2_3,    Is.Not.Null);

            Assert.That(emsp2CommonAPI_v2_1_1,  Is.Not.Null);
            Assert.That(emsp2CommonAPI_v2_2_1,  Is.Not.Null);
            Assert.That(emsp2CommonAPI_v2_3,    Is.Not.Null);

            #endregion

            #region Create cpo1/cpo2/emsp1/emsp2 OCPI WebAPIs

            cpo1WebAPI_v2_1_1    = new OCPIv2_1_1.WebAPI.OCPIWebAPI(
                                       CommonAPI:                           cpo1CommonAPI_v2_1_1,
                                       HTTPServer:                          cpo1HTTPAPI.HTTPServer,
                                       OverlayURLPathPrefix:                HTTPPath.Parse("/ocpi/v2.1"),
                                       APIURLPathPrefix:                    HTTPPath.Parse("/ocpi/v2.1/api"),
                                       WebAPIURLPathPrefix:                 HTTPPath.Parse("/ocpi/v2.1/webapi"),
                                       BasePath:                            HTTPPath.Parse("/ocpi/v2.1"),
                                       HTTPRealm:                           "GraphDefined OCPI CPO #1 WebAPI",
                                       HTTPLogins:                          [
                                                                                new KeyValuePair<String, String>("a", "b")
                                                                            ]
                                   );

            //emsp1WebAPI          = new OCPIWebAPI(
            //                           HTTPServer:                          emsp1HTTPAPI.HTTPServer,
            //                           CommonAPI:                           emsp1CommonAPI,
            //                           OverlayURLPathPrefix:                HTTPPath.Parse("/ocpi/v2.1"),
            //                           APIURLPathPrefix:                    HTTPPath.Parse("/ocpi/v2.1/api"),
            //                           WebAPIURLPathPrefix:                 HTTPPath.Parse("/ocpi/v2.1/webapi"),
            //                           BasePath:                            HTTPPath.Parse("/ocpi/v2.1"),
            //                           HTTPRealm:                           "GraphDefined OCPI EMSP #1 WebAPI",
            //                           HTTPLogins:                          [
            //                                                                    new KeyValuePair<String, String>("c", "d")
            //                                                                ]
            //                       );

            //emsp2WebAPI          = new OCPIWebAPI(
            //                           HTTPServer:                          emsp2HTTPAPI.HTTPServer,
            //                           CommonAPI:                           emsp2CommonAPI,
            //                           OverlayURLPathPrefix:                HTTPPath.Parse("/ocpi/v2.1"),
            //                           APIURLPathPrefix:                    HTTPPath.Parse("/ocpi/v2.1/api"),
            //                           WebAPIURLPathPrefix:                 HTTPPath.Parse("/ocpi/v2.1/webapi"),
            //                           BasePath:                            HTTPPath.Parse("/ocpi/v2.1"),
            //                           HTTPRealm:                           "GraphDefined OCPI EMSP #2 WebAPI",
            //                           HTTPLogins:                          [
            //                                                                    new KeyValuePair<String, String>("e", "f")
            //                                                                ]
            //                       );

            Assert.That(cpo1WebAPI_v2_1_1,  Is.Not.Null);
            //ClassicAssert.IsNotNull(cpoWebAPI);
            //ClassicAssert.IsNotNull(emsp1WebAPI);
            //ClassicAssert.IsNotNull(emsp2WebAPI);

            #endregion

            #region Create cpo1/cpo2 CPOAPIs & emsp1/emsp2 EMPAPIs

            cpo1CPOAPI_v2_1_1    = new OCPIv2_1_1.HTTP.CPOAPI(

                                       CommonAPI:                           cpo1CommonAPI_v2_1_1,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1/v2.1.1/cpo"),
                                       APIVersionHashes:                    null,

                                       DisableMaintenanceTasks:             null,
                                       MaintenanceInitialDelay:             null,
                                       MaintenanceEvery:                    null,

                                       DisableWardenTasks:                  null,
                                       WardenInitialDelay:                  null,
                                       WardenCheckEvery:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null,
                                       AutoStart:                           false

                                   );

            //emsp1EMSPAPI         = new EMSPAPI(

            //                           CommonAPI:                           emsp1CommonAPI,
            //                           AllowDowngrades:                     null,

            //                           HTTPHostname:                        null,
            //                           ExternalDNSName:                     null,
            //                           HTTPServiceName:                     null,
            //                           BasePath:                            null,

            //                           URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1/v2.1.1/emsp"),
            //                           APIVersionHashes:                    null,

            //                           DisableMaintenanceTasks:             null,
            //                           MaintenanceInitialDelay:             null,
            //                           MaintenanceEvery:                    null,

            //                           DisableWardenTasks:                  null,
            //                           WardenInitialDelay:                  null,
            //                           WardenCheckEvery:                    null,

            //                           IsDevelopment:                       null,
            //                           DevelopmentServers:                  null,
            //                           DisableLogging:                      null,
            //                           LoggingPath:                         null,
            //                           LogfileName:                         null,
            //                           LogfileCreator:                      null,
            //                           AutoStart:                           false

            //                       );

            //emsp2EMSPAPI         = new EMSPAPI(

            //                           CommonAPI:                           emsp2CommonAPI,
            //                           AllowDowngrades:                     null,

            //                           HTTPHostname:                        null,
            //                           ExternalDNSName:                     null,
            //                           HTTPServiceName:                     null,
            //                           BasePath:                            null,

            //                           URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1/v2.1.1/emsp"),
            //                           APIVersionHashes:                    null,

            //                           DisableMaintenanceTasks:             null,
            //                           MaintenanceInitialDelay:             null,
            //                           MaintenanceEvery:                    null,

            //                           DisableWardenTasks:                  null,
            //                           WardenInitialDelay:                  null,
            //                           WardenCheckEvery:                    null,

            //                           IsDevelopment:                       null,
            //                           DevelopmentServers:                  null,
            //                           DisableLogging:                      null,
            //                           LoggingPath:                         null,
            //                           LogfileName:                         null,
            //                           LogfileCreator:                      null,
            //                           AutoStart:                           false

            //                       );

            Assert.That(cpo1CPOAPI_v2_1_1,  Is.Not.Null);
            //ClassicAssert.IsNotNull(emsp1EMSPAPI);
            //ClassicAssert.IsNotNull(emsp2EMSPAPI);

            #endregion


            #region Define and connect Remote Parties

            #region CPO #1 -> EMSP #1

            await emsp1CommonAPI_v2_1_1.AddRemoteParty(CountryCode:                 cpo1CommonAPI_v2_1_1.OurCountryCode,
                                                       PartyId:                     cpo1CommonAPI_v2_1_1.OurPartyId,
                                                       Role:                        Role.CPO,
                                                       BusinessDetails:             cpo1CommonAPI_v2_1_1.OurBusinessDetails,

                                                       AccessToken:                 AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                       AccessStatus:                AccessStatus.ALLOWED,

                                                       RemoteAccessToken:           AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                       RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/versions"),
                                                       RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                                                       SelectedVersionId:           OCPIv2_2_1.Version.Id,
                                                       AccessTokenBase64Encoding:   false,
                                                       RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                       PartyStatus:                 PartyStatus.ENABLED);

            await emsp1CommonAPI_v2_2_1.AddRemoteParty(Id:                          RemoteParty_Id.Parse(
                                                                                        cpo1CommonAPI_v2_2_1.DefaultCountryCode,
                                                                                        cpo1CommonAPI_v2_2_1.DefaultPartyId,
                                                                                        Role.CPO
                                                                                    ),
                                                       CredentialsRoles:            [
                                                                                        cpo1CommonAPI_v2_2_1.OurCredentialRoles.First()
                                                                                    ],
                                                       AccessToken:                 AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                       AccessStatus:                AccessStatus.ALLOWED,

                                                       RemoteAccessToken:           AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                       RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/versions"),
                                                       RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                                                       SelectedVersionId:           OCPIv2_2_1.Version.Id,
                                                       AccessTokenBase64Encoding:   true,
                                                       RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                       PartyStatus:                 PartyStatus.ENABLED);

            await emsp1CommonAPI_v2_3.  AddRemoteParty(Id:                          RemoteParty_Id.Parse(
                                                                                        cpo1CommonAPI_v2_3.DefaultCountryCode,
                                                                                        cpo1CommonAPI_v2_3.DefaultPartyId,
                                                                                        Role.CPO
                                                                                    ),
                                                       CredentialsRoles:            [
                                                                                        cpo1CommonAPI_v2_3.OurCredentialRoles.First()
                                                                                    ],
                                                       AccessToken:                 AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                       AccessStatus:                AccessStatus.ALLOWED,

                                                       RemoteAccessToken:           AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                       RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/versions"),
                                                       RemoteVersionIds:            [ OCPIv2_3.Version.Id ],
                                                       SelectedVersionId:           OCPIv2_3.Version.Id,
                                                       AccessTokenBase64Encoding:   true,
                                                       RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                       PartyStatus:                 PartyStatus.ENABLED);

            #endregion

            #region CPO #1 -> EMSP #2

            await emsp2CommonAPI_v2_1_1.AddRemoteParty(CountryCode:                 cpo1CommonAPI_v2_1_1.OurCountryCode,
                                                       PartyId:                     cpo1CommonAPI_v2_1_1.OurPartyId,
                                                       Role:                        Role.CPO,
                                                       BusinessDetails:             cpo1CommonAPI_v2_1_1.OurBusinessDetails,

                                                       AccessToken:                 AccessToken.Parse(cpo1_accessing_emsp2__token),
                                                       AccessStatus:                AccessStatus.ALLOWED,

                                                       RemoteAccessToken:           AccessToken.Parse(emsp2_accessing_cpo1__token),
                                                       RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/versions"),
                                                       RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                                                       SelectedVersionId:           OCPIv2_2_1.Version.Id,
                                                       AccessTokenBase64Encoding:   false,
                                                       RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                       PartyStatus:                 PartyStatus.ENABLED);

            await emsp2CommonAPI_v2_2_1.AddRemoteParty(Id:                          RemoteParty_Id.Parse(
                                                                                        cpo1CommonAPI_v2_2_1.DefaultCountryCode,
                                                                                        cpo1CommonAPI_v2_2_1.DefaultPartyId,
                                                                                        Role.CPO
                                                                                    ),
                                                       CredentialsRoles:            [
                                                                                        cpo1CommonAPI_v2_2_1.OurCredentialRoles.First()
                                                                                    ],
                                                       AccessToken:                 AccessToken.Parse(cpo1_accessing_emsp2__token),
                                                       AccessStatus:                AccessStatus.ALLOWED,

                                                       RemoteAccessToken:           AccessToken.Parse(emsp2_accessing_cpo1__token),
                                                       RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/versions"),
                                                       RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                                                       SelectedVersionId:           OCPIv2_2_1.Version.Id,
                                                       AccessTokenBase64Encoding:   true,
                                                       RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                       PartyStatus:                 PartyStatus.ENABLED);

            await emsp2CommonAPI_v2_3.  AddRemoteParty(Id:                          RemoteParty_Id.Parse(
                                                                                        cpo1CommonAPI_v2_3.DefaultCountryCode,
                                                                                        cpo1CommonAPI_v2_3.DefaultPartyId,
                                                                                        Role.CPO
                                                                                    ),
                                                       CredentialsRoles:            [
                                                                                        cpo1CommonAPI_v2_3.OurCredentialRoles.First()
                                                                                    ],
                                                       AccessToken:                 AccessToken.Parse(cpo1_accessing_emsp2__token),
                                                       AccessStatus:                AccessStatus.ALLOWED,

                                                       RemoteAccessToken:           AccessToken.Parse(emsp2_accessing_cpo1__token),
                                                       RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/versions"),
                                                       RemoteVersionIds:            [ OCPIv2_3.Version.Id ],
                                                       SelectedVersionId:           OCPIv2_3.Version.Id,
                                                       AccessTokenBase64Encoding:   true,
                                                       RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                       PartyStatus:                 PartyStatus.ENABLED);

            #endregion


            #region EMSP #1 -> CPO #1

            await cpo1CommonAPI_v2_1_1.AddRemoteParty (CountryCode:                 emsp1CommonAPI_v2_1_1.OurCountryCode,
                                                       PartyId:                     emsp1CommonAPI_v2_1_1.OurPartyId,
                                                       Role:                        Role.EMSP,
                                                       BusinessDetails:             emsp1CommonAPI_v2_1_1.OurBusinessDetails,

                                                       AccessToken:                 AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                       AccessStatus:                AccessStatus.ALLOWED,

                                                       RemoteAccessToken:           AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                       RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/versions"),
                                                       RemoteVersionIds:            [ OCPIv2_1_1.Version.Id ],
                                                       SelectedVersionId:           OCPIv2_1_1.Version.Id,
                                                       AccessTokenBase64Encoding:   false,
                                                       RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                       PartyStatus:                 PartyStatus.ENABLED);

            await cpo1CommonAPI_v2_2_1.AddRemoteParty (Id:                          RemoteParty_Id.Parse(
                                                                                        emsp1CommonAPI_v2_2_1.DefaultCountryCode,
                                                                                        emsp1CommonAPI_v2_2_1.DefaultPartyId,
                                                                                        Role.EMSP
                                                                                    ),
                                                       CredentialsRoles:            [
                                                                                        emsp1CommonAPI_v2_2_1.OurCredentialRoles.First()
                                                                                        //new OCPIv2_2_1.CredentialsRole(
                                                                                        //    CountryCode:       emsp1CommonAPI_v2_2_1.OurCountryCode,
                                                                                        //    PartyId:           emsp1CommonAPI_v2_2_1.OurPartyId,
                                                                                        //    Role:              Role.EMSP,
                                                                                        //    BusinessDetails:   emsp1CommonAPI_v2_2_1.OurBusinessDetails,
                                                                                        //    AllowDowngrades:   true
                                                                                        //)
                                                                                    ],

                                                       AccessToken:                 AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                       AccessStatus:                AccessStatus.ALLOWED,

                                                       RemoteAccessToken:           AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                       RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/versions"),
                                                       RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                                                       SelectedVersionId:           OCPIv2_2_1.Version.Id,
                                                       AccessTokenBase64Encoding:   true,
                                                       RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                       PartyStatus:                 PartyStatus.ENABLED);

            await cpo1CommonAPI_v2_3.  AddRemoteParty (Id:                          RemoteParty_Id.Parse(
                                                                                        emsp1CommonAPI_v2_3.DefaultCountryCode,
                                                                                        emsp1CommonAPI_v2_3.DefaultPartyId,
                                                                                        Role.EMSP
                                                                                    ),
                                                       CredentialsRoles:            [
                                                                                        emsp1CommonAPI_v2_3.OurCredentialRoles.First()
                                                                                        //new OCPIv2_3.CredentialsRole(
                                                                                        //    CountryCode:       emsp1CommonAPI_v2_3.OurCountryCode,
                                                                                        //    PartyId:           emsp1CommonAPI_v2_3.OurPartyId,
                                                                                        //    Role:              Role.EMSP,
                                                                                        //    BusinessDetails:   emsp1CommonAPI_v2_3.OurBusinessDetails,
                                                                                        //    AllowDowngrades:   true
                                                                                        //)
                                                                                    ],

                                                       AccessToken:                 AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                       AccessStatus:                AccessStatus.ALLOWED,

                                                       RemoteAccessToken:           AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                       RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/versions"),
                                                       RemoteVersionIds:            [ OCPIv2_3.Version.Id ],
                                                       SelectedVersionId:           OCPIv2_3.Version.Id,
                                                       AccessTokenBase64Encoding:   true,
                                                       RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                       PartyStatus:                 PartyStatus.ENABLED);

            #endregion


            //await emsp2CommonAPI.AddRemoteParty(CountryCode:                 cpoCommonAPI.OurCountryCode,
            //                                    PartyId:                     cpoCommonAPI.OurPartyId,
            //                                    Role:                        Role.CPO,
            //                                    BusinessDetails:             cpoCommonAPI.OurBusinessDetails,

            //                                    AccessToken:                 AccessToken.Parse(cpo_accessing_emsp2__token),
            //                                    AccessStatus:                AccessStatus.ALLOWED,

            //                                    RemoteAccessToken:           AccessToken.Parse(emsp2_accessing_cpo__token),
            //                                    RemoteVersionsURL:           URL.Parse($"http://localhost:{cpoHTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.1/versions"),
            //                                    RemoteVersionIds:            [ Version.Id ],
            //                                    SelectedVersionId:           Version.Id,
            //                                    AccessTokenBase64Encoding:   false,
            //                                    RemoteStatus:                RemoteAccessStatus.ONLINE,

            //                                    PartyStatus:                 PartyStatus.ENABLED);


            Assert.That(cpo1CommonAPI_v2_1_1.RemoteParties.Count(),  Is.EqualTo(1));
            Assert.That(cpo1CommonAPI_v2_2_1.RemoteParties.Count(),  Is.EqualTo(1));

            Assert.That(File.ReadAllLines(cpo1CommonAPI_v2_1_1.RemotePartyDBFileName).Length,  Is.EqualTo(1));
            Assert.That(File.ReadAllLines(cpo1CommonAPI_v2_2_1.RemotePartyDBFileName).Length,  Is.EqualTo(1));

            //ClassicAssert.AreEqual(2, File.ReadAllLines(cpoCommonAPI.  BaseAPI.RemotePartyDBFileName).Length);
            //ClassicAssert.AreEqual(1, File.ReadAllLines(emsp1CommonAPI.BaseAPI.RemotePartyDBFileName).Length);
            //ClassicAssert.AreEqual(1, File.ReadAllLines(emsp2CommonAPI.BaseAPI.RemotePartyDBFileName).Length);

            #endregion

            #region Define blocked Remote Parties

            //await cpoCommonAPI.AddRemoteParty  (CountryCode:       CountryCode.Parse("XX"),
            //                                    PartyId:           Party_Id.   Parse("BLE"),
            //                                    Role:              Role.EMSP,
            //                                    BusinessDetails:   new BusinessDetails(
            //                                                           "Blocked EMSP"
            //                                                       ),
            //                                    AccessToken:       AccessToken.Parse(BlockedEMSPToken),
            //                                    AccessStatus:      AccessStatus.BLOCKED,
            //                                    PartyStatus:       PartyStatus. ENABLED);

            //await emsp1CommonAPI.AddRemoteParty(CountryCode:       CountryCode.Parse("XX"),
            //                                    PartyId:           Party_Id.   Parse("BLC"),
            //                                    Role:              Role.CPO,
            //                                    BusinessDetails:   new BusinessDetails(
            //                                                           "Blocked CPO"
            //                                                       ),
            //                                    AccessToken:       AccessToken.Parse(BlockedCPOToken),
            //                                    AccessStatus:      AccessStatus.BLOCKED,
            //                                    PartyStatus:       PartyStatus. ENABLED);

            //await emsp2CommonAPI.AddRemoteParty(CountryCode:       CountryCode.Parse("XX"),
            //                                    PartyId:           Party_Id.   Parse("BLC"),
            //                                    Role:              Role.CPO,
            //                                    BusinessDetails:   new BusinessDetails(
            //                                                           "Blocked CPO"
            //                                                       ),
            //                                    AccessToken:       AccessToken.Parse(BlockedCPOToken),
            //                                    AccessStatus:      AccessStatus.BLOCKED,
            //                                    PartyStatus:       PartyStatus. ENABLED);


            //ClassicAssert.AreEqual(3, cpoCommonAPI.  RemoteParties.Count());
            //ClassicAssert.AreEqual(2, emsp1CommonAPI.RemoteParties.Count());
            //ClassicAssert.AreEqual(2, emsp2CommonAPI.RemoteParties.Count());

            #endregion

            #region Defined API loggers

            //// CPO
            //cpoAPIRequestLogs     = new ConcurrentDictionary<DateTime, OCPIRequest>();
            //cpoAPIResponseLogs    = new ConcurrentDictionary<DateTime, OCPIResponse>();

            //cpoCPOAPI.CPOAPILogger?.RegisterLogTarget(LogTargets.Debug,
            //                                          (loggingPath, context, logEventName, request) => {
            //                                              cpoAPIRequestLogs. TryAdd(Timestamp.Now, request);
            //                                              return Task.CompletedTask;
            //                                          });

            //cpoCPOAPI.CPOAPILogger?.RegisterLogTarget(LogTargets.Debug,
            //                                          (loggingPath, context, logEventName, request, response) => {
            //                                              cpoAPIResponseLogs.TryAdd(Timestamp.Now, response);
            //                                              return Task.CompletedTask;
            //                                          });

            //cpoCPOAPI.CPOAPILogger?.Debug("all", LogTargets.Debug);

            //cpoCommonAPI.BaseAPI.ClientConfigurations.Description = (remotePartyId) => I18NString.Create($"CPO Client for {remotePartyId}");



            //// EMSP #1
            //emsp1APIRequestLogs   = new ConcurrentDictionary<DateTime, OCPIRequest>();
            //emsp1APIResponseLogs  = new ConcurrentDictionary<DateTime, OCPIResponse>();

            //emsp1EMSPAPI.EMSPAPILogger?.RegisterLogTarget(LogTargets.Debug,
            //                                              (loggingPath, context, logEventName, request) => {
            //                                                  emsp1APIRequestLogs. TryAdd(Timestamp.Now, request);
            //                                                  return Task.CompletedTask;
            //                                              });

            //emsp1EMSPAPI.EMSPAPILogger?.RegisterLogTarget(LogTargets.Debug,
            //                                              (loggingPath, context, logEventName, request, response) => {
            //                                                  emsp1APIResponseLogs.TryAdd(Timestamp.Now, response);
            //                                                  return Task.CompletedTask;
            //                                              });

            //emsp1EMSPAPI.EMSPAPILogger?.Debug("all", LogTargets.Debug);

            //emsp1CommonAPI.BaseAPI.ClientConfigurations.Description = (remotePartyId) => I18NString.Create($"EMSP #1 Client for {remotePartyId}");



            //// EMSP #2
            //emsp2APIRequestLogs   = new ConcurrentDictionary<DateTime, OCPIRequest>();
            //emsp2APIResponseLogs  = new ConcurrentDictionary<DateTime, OCPIResponse>();

            //emsp2EMSPAPI.EMSPAPILogger?.RegisterLogTarget(LogTargets.Debug,
            //                                              (loggingPath, context, logEventName, request) => {
            //                                                  emsp2APIRequestLogs. TryAdd(Timestamp.Now, request);
            //                                                  return Task.CompletedTask;
            //                                              });

            //emsp2EMSPAPI.EMSPAPILogger?.RegisterLogTarget(LogTargets.Debug,
            //                                              (loggingPath, context, logEventName, request, response) => {
            //                                                  emsp2APIResponseLogs.TryAdd(Timestamp.Now, response);
            //                                                  return Task.CompletedTask;
            //                                              });

            //emsp2EMSPAPI.EMSPAPILogger?.Debug("all", LogTargets.Debug);

            //emsp2CommonAPI.BaseAPI.ClientConfigurations.Description = (remotePartyId) => I18NString.Create($"EMSP #2 Client for {remotePartyId}");

            #endregion

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public async virtual Task SetupEachTest()
        {

            Timestamp.Reset();

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public async virtual Task ShutdownEachTest()
        {



        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public async virtual Task ShutdownOnce()
        {

            if (cpo1HTTPAPI is not null)
                await cpo1HTTPAPI. Shutdown();

            if (cpo2HTTPAPI is not null)
                await cpo2HTTPAPI. Shutdown();

            if (emsp1HTTPAPI is not null)
                await emsp1HTTPAPI.Shutdown();

            if (emsp2HTTPAPI is not null)
                await emsp2HTTPAPI.Shutdown();


            // CPO #1
            if (cpo1CommonAPI_v2_1_1 is not null)
                File.Delete(cpo1CommonAPI_v2_1_1.RemotePartyDBFileName);

            if (cpo1CommonAPI_v2_2_1 is not null)
                File.Delete(cpo1CommonAPI_v2_2_1.RemotePartyDBFileName);

            if (cpo1CommonAPI_v2_3 is not null)
                File.Delete(cpo1CommonAPI_v2_3.RemotePartyDBFileName);

            if (cpo1CommonAPI_v3_0   is not null)
                File.Delete(cpo1CommonAPI_v3_0.  RemotePartyDBFileName);


            // CPO #2
            if (cpo2CommonAPI_v2_1_1 is not null)
                File.Delete(cpo2CommonAPI_v2_1_1.RemotePartyDBFileName);

            if (cpo2CommonAPI_v2_2_1 is not null)
                File.Delete(cpo2CommonAPI_v2_2_1.RemotePartyDBFileName);

            if (cpo2CommonAPI_v2_3 is not null)
                File.Delete(cpo2CommonAPI_v2_3.RemotePartyDBFileName);

            if (cpo2CommonAPI_v3_0   is not null)
                File.Delete(cpo2CommonAPI_v3_0.  RemotePartyDBFileName);


            // EMSP #1
            if (emsp1CommonAPI_v2_1_1 is not null)
                File.Delete(emsp1CommonAPI_v2_1_1.RemotePartyDBFileName);

            if (emsp1CommonAPI_v2_2_1 is not null)
                File.Delete(emsp1CommonAPI_v2_2_1.RemotePartyDBFileName);

            if (emsp1CommonAPI_v2_3 is not null)
                File.Delete(emsp1CommonAPI_v2_3.RemotePartyDBFileName);

            if (emsp1CommonAPI_v3_0   is not null)
                File.Delete(emsp1CommonAPI_v3_0.  RemotePartyDBFileName);


            // EMSP #2
            if (emsp2CommonAPI_v2_1_1 is not null)
                File.Delete(emsp2CommonAPI_v2_1_1.RemotePartyDBFileName);

            if (emsp2CommonAPI_v2_2_1 is not null)
                File.Delete(emsp2CommonAPI_v2_2_1.RemotePartyDBFileName);

            if (emsp2CommonAPI_v2_3 is not null)
                File.Delete(emsp2CommonAPI_v2_3.RemotePartyDBFileName);

            if (emsp2CommonAPI_v3_0   is not null)
                File.Delete(emsp2CommonAPI_v3_0.  RemotePartyDBFileName);

        }

        #endregion


    }

}
