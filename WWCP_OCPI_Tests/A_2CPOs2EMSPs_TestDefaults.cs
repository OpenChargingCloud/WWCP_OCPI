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
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPI.WebAPI;

using cloud.charging.open.protocols.OCPIv2_1_1;
using cloud.charging.open.protocols.OCPIv2_2_1;
using cloud.charging.open.protocols.OCPIv2_3_0;
using cloud.charging.open.protocols.OCPIv3_0;

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
    public abstract class A_2CPOs2EMSPs_TestDefaults(TimeSpan? TOTPValidityTime = null)
    {

        #region Data

        private   readonly DNSClient  DNSClient          = new (SearchForIPv6DNSServers: false);

        private   readonly TimeSpan?  TOTPValidityTime   = TOTPValidityTime;

        protected const    String     UnknownToken       = "UnknownUnknownUnknownToken";

        protected const    String     BlockedToken       = "blocked-token";
        protected const    String     BlockedCPOToken    = "blocked-cpo";
        protected const    String     BlockedEMSPToken   = "blocked-emsp";

        #region CPO #1

        public          HTTPTestServerX?                                               cpo1HTTPServer;
        protected       HTTPExtAPIX?                                                   cpo1HTTPAPI;
        public          URL?                                                           cpo1VersionsAPIURL;
        protected       CommonHTTPAPI?                                                 cpo1CommonHTTPAPI;
        protected       CommonWebAPI?                                                  cpo1CommonWebAPI;

        protected       OCPIv2_1_1.CommonAPI?                                          cpo1CommonAPI_v2_1_1;
        protected       OCPIv2_1_1.WebAPI.OCPIWebAPI?                                  cpo1WebAPI_v2_1_1;
        protected       OCPIv2_1_1.CPOAPI?                                             cpo1CPOAPI_v2_1_1;
        protected       OCPIv2_1_1.OCPICSOAdapter?                                     cpo1Adapter_v2_1_1;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_1_1.OCPIRequest>   cpo1APIRequestLogs_v2_1_1  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_1_1.OCPIResponse>  cpo1APIResponseLogs_v2_1_1 = [];

        protected       OCPIv2_2_1.CommonAPI?                                          cpo1CommonAPI_v2_2_1;
        protected       OCPIv2_2_1.WebAPI.OCPIWebAPI?                                  cpo1WebAPI_v2_2_1;
        protected       OCPIv2_2_1.CPOAPI?                                             cpo1CPOAPI_v2_2_1;
        protected       OCPIv2_2_1.OCPICSOAdapter?                                     cpo1Adapter_v2_2_1;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_2_1.OCPIRequest>   cpo1APIRequestLogs_v2_2_1  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_2_1.OCPIResponse>  cpo1APIResponseLogs_v2_2_1 = [];

        protected       OCPIv2_3_0.CommonAPI?                                          cpo1CommonAPI_v2_3_0;
        protected       OCPIv2_3_0.WebAPI.OCPIWebAPI?                                  cpo1WebAPI_v2_3_0;
        protected       OCPIv2_3_0.CPOAPI?                                             cpo1CPOAPI_v2_3_0;
        protected       OCPIv2_3_0.OCPICSOAdapter?                                     cpo1Adapter_v2_3_0;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_3_0.OCPIRequest>   cpo1APIRequestLogs_v2_3_0  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_3_0.OCPIResponse>  cpo1APIResponseLogs_v2_3_0 = [];

        protected       OCPIv3_0.CommonAPI?                                            cpo1CommonAPI_v3_0;
        protected       OCPIv3_0.WebAPI.OCPIWebAPI?                                    cpo1WebAPI_v3_0;
        protected       OCPIv3_0.CPOAPI?                                               cpo1CPOAPI_v3_0;
        protected       OCPIv3_0.OCPICSOAdapter?                                       cpo1Adapter_v3_0;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv3_0.OCPIRequest>     cpo1APIRequestLogs_v3_0  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv3_0.OCPIResponse>    cpo1APIResponseLogs_v3_0 = [];

        protected const String                                                         cpo1_accessing_emsp1__token   = "cpo1_accessing_emsp1++token";
        protected const String                                                         cpo1_accessing_emsp2__token   = "cpo1_accessing_emsp2++token";

        #endregion

        #region CPO #2

        public          HTTPTestServerX?                                               cpo2HTTPServer;
        protected       HTTPExtAPIX?                                                   cpo2HTTPAPI;
        public          URL?                                                           cpo2VersionsAPIURL;
        protected       CommonHTTPAPI?                                                 cpo2CommonHTTPAPI;
        protected       CommonWebAPI?                                                  cpo2CommonWebAPI;

        protected       OCPIv2_1_1.CommonAPI?                                          cpo2CommonAPI_v2_1_1;
        protected       OCPIv2_1_1.WebAPI.OCPIWebAPI?                                  cpo2WebAPI_v2_1_1;
        protected       OCPIv2_1_1.CPOAPI?                                             cpo2CPOAPI_v2_1_1;
        protected       OCPIv2_1_1.OCPICSOAdapter?                                     cpo2Adapter_v2_1_1;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_1_1.OCPIRequest>   cpo2APIRequestLogs_v2_1_1  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_1_1.OCPIResponse>  cpo2APIResponseLogs_v2_1_1 = [];

        protected       OCPIv2_2_1.CommonAPI?                                          cpo2CommonAPI_v2_2_1;
        protected       OCPIv2_2_1.WebAPI.OCPIWebAPI?                                  cpo2WebAPI_v2_2_1;
        protected       OCPIv2_2_1.CPOAPI?                                             cpo2CPOAPI_v2_2_1;
        protected       OCPIv2_2_1.OCPICSOAdapter?                                     cpo2Adapter_v2_2_1;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_2_1.OCPIRequest>   cpo2APIRequestLogs_v2_2_1  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_2_1.OCPIResponse>  cpo2APIResponseLogs_v2_2_1 = [];

        protected       OCPIv2_3_0.CommonAPI?                                          cpo2CommonAPI_v2_3_0;
        protected       OCPIv2_3_0.WebAPI.OCPIWebAPI?                                  cpo2WebAPI_v2_3_0;
        protected       OCPIv2_3_0.CPOAPI?                                             cpo2CPOAPI_v2_3_0;
        protected       OCPIv2_3_0.OCPICSOAdapter?                                     cpo2Adapter_v2_3_0;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_3_0.OCPIRequest>   cpo2APIRequestLogs_v2_3_0  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_3_0.OCPIResponse>  cpo2APIResponseLogs_v2_3_0 = [];

        protected       OCPIv3_0.CommonAPI?                                            cpo2CommonAPI_v3_0;
        protected       OCPIv3_0.WebAPI.OCPIWebAPI?                                    cpo2WebAPI_v3_0;
        protected       OCPIv3_0.CPOAPI?                                               cpo2CPOAPI_v3_0;
        protected       OCPIv3_0.OCPICSOAdapter?                                       cpo2Adapter_v3_0;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv3_0.OCPIRequest>     cpo2APIRequestLogs_v3_0  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv3_0.OCPIResponse>    cpo2APIResponseLogs_v3_0 = [];

        protected const String                                                         cpo2_accessing_emsp1__token   = "cpo2_accessing_emsp1++token";
        protected const String                                                         cpo2_accessing_emsp2__token   = "cpo2_accessing_emsp2++token";

        #endregion

        #region EMSP #1

        public          HTTPTestServerX?                                               emsp1HTTPServer;
        protected       HTTPExtAPIX?                                                   emsp1HTTPAPI;
        public          URL?                                                           emsp1VersionsAPIURL;
        protected       CommonHTTPAPI?                                                 emsp1CommonHTTPAPI;
        protected       CommonWebAPI?                                                  emsp1CommonWebAPI;

        protected       OCPIv2_1_1.CommonAPI?                                          emsp1CommonAPI_v2_1_1;
        protected       OCPIv2_1_1.WebAPI.OCPIWebAPI?                                  emsp1WebAPI_v2_1_1;
        protected       OCPIv2_1_1.EMSPAPI?                                            emsp1EMSPAPI_v2_1_1;
        protected       OCPIv2_1_1.OCPIEMPAdapter?                                     emsp1Adapter_v2_1_1;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_1_1.OCPIRequest>   emsp1APIRequestLogs_v2_1_1  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_1_1.OCPIResponse>  emsp1APIResponseLogs_v2_1_1 = [];

        protected       OCPIv2_2_1.CommonAPI?                                          emsp1CommonAPI_v2_2_1;
        protected       OCPIv2_2_1.WebAPI.OCPIWebAPI?                                  emsp1WebAPI_v2_2_1;
        protected       OCPIv2_2_1.EMSPAPI?                                            emsp1EMSPAPI_v2_2_1;
        protected       OCPIv2_2_1.OCPIEMPAdapter?                                     emsp1Adapter_v2_2_1;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_2_1.OCPIRequest>   emsp1APIRequestLogs_v2_2_1  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_2_1.OCPIResponse>  emsp1APIResponseLogs_v2_2_1 = [];

        protected       OCPIv2_3_0.CommonAPI?                                          emsp1CommonAPI_v2_3_0;
        protected       OCPIv2_3_0.WebAPI.OCPIWebAPI?                                  emsp1WebAPI_v2_3_0;
        protected       OCPIv2_3_0.EMSPAPI?                                            emsp1EMSPAPI_v2_3_0;
        protected       OCPIv2_3_0.OCPIEMPAdapter?                                     emsp1Adapter_v2_3_0;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_3_0.OCPIRequest>   emsp1APIRequestLogs_v2_3_0  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_3_0.OCPIResponse>  emsp1APIResponseLogs_v2_3_0 = [];

        protected       OCPIv3_0.CommonAPI?                                            emsp1CommonAPI_v3_0;
        protected       OCPIv3_0.WebAPI.OCPIWebAPI?                                    emsp1WebAPI_v3_0;
        protected       OCPIv3_0.EMSPAPI?                                              emsp1EMSPAPI_v3_0;
        protected       OCPIv3_0.OCPIEMPAdapter?                                       emsp1Adapter_v3_0;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv3_0.OCPIRequest>     emsp1APIRequestLogs_v3_0  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv3_0.OCPIResponse>    emsp1APIResponseLogs_v3_0 = [];

        protected const String                                                         emsp1_accessing_cpo1__token   = "emsp1_accessing_cpo1++token";
        protected const String                                                         emsp2_accessing_cpo1__token   = "emsp2_accessing_cpo2++token";

        #endregion

        #region EMSP #2

        public          HTTPTestServerX?                                               emsp2HTTPServer;
        protected       HTTPExtAPIX?                                                   emsp2HTTPAPI;
        public          URL?                                                           emsp2VersionsAPIURL;
        protected       CommonHTTPAPI?                                                 emsp2CommonHTTPAPI;
        protected       CommonWebAPI?                                                  emsp2CommonWebAPI;

        protected       OCPIv2_1_1.CommonAPI?                                          emsp2CommonAPI_v2_1_1;
        protected       OCPIv2_1_1.WebAPI.OCPIWebAPI?                                  emsp2WebAPI_v2_1_1;
        protected       OCPIv2_1_1.EMSPAPI?                                            emsp2EMSPAPI_v2_1_1;
        protected       OCPIv2_1_1.OCPIEMPAdapter?                                     emsp2Adapter_v2_1_1;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_1_1.OCPIRequest>   emsp2APIRequestLogs_v2_1_1  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_1_1.OCPIResponse>  emsp2APIResponseLogs_v2_1_1 = [];

        protected       OCPIv2_2_1.CommonAPI?                                          emsp2CommonAPI_v2_2_1;
        protected       OCPIv2_2_1.WebAPI.OCPIWebAPI?                                  emsp2WebAPI_v2_2_1;
        protected       OCPIv2_2_1.EMSPAPI?                                            emsp2EMSPAPI_v2_2_1;
        protected       OCPIv2_2_1.OCPIEMPAdapter?                                     emsp2Adapter_v2_2_1;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_2_1.OCPIRequest>   emsp2APIRequestLogs_v2_2_1  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_2_1.OCPIResponse>  emsp2APIResponseLogs_v2_2_1 = [];

        protected       OCPIv2_3_0.CommonAPI?                                          emsp2CommonAPI_v2_3_0;
        protected       OCPIv2_3_0.WebAPI.OCPIWebAPI?                                  emsp2WebAPI_v2_3_0;
        protected       OCPIv2_3_0.EMSPAPI?                                            emsp2EMSPAPI_v2_3_0;
        protected       OCPIv2_3_0.OCPIEMPAdapter?                                     emsp2Adapter_v2_3_0;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_3_0.OCPIRequest>   emsp2APIRequestLogs_v2_3_0  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv2_3_0.OCPIResponse>  emsp2APIResponseLogs_v2_3_0 = [];

        protected       OCPIv3_0.CommonAPI?                                            emsp2CommonAPI_v3_0;
        protected       OCPIv3_0.WebAPI.OCPIWebAPI?                                    emsp2WebAPI_v3_0;
        protected       OCPIv3_0.EMSPAPI?                                              emsp2EMSPAPI_v3_0;
        protected       OCPIv3_0.OCPIEMPAdapter?                                       emsp2Adapter_v3_0;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv3_0.OCPIRequest>     emsp2APIRequestLogs_v3_0  = [];
        protected       ConcurrentDictionary<DateTimeOffset, OCPIv3_0.OCPIResponse>    emsp2APIResponseLogs_v3_0 = [];

        protected const String                                                         emsp1_accessing_cpo2__token   = "emsp1_accessing_cpo1++token";
        protected const String                                                         emsp2_accessing_cpo2__token   = "emsp2_accessing_cpo2++token";

        #endregion

        #endregion


        #region (private) checkFile(FileName, Checker)

        /// <summary>
        /// Most (log) files are written async in the background, thus test might run faster,
        /// than those background processes really write the files. When we want to verify,
        /// that something was written to a file, we might have to retry a few times and
        /// meanwhile wait a little bit.
        /// </summary>
        /// <param name="FileName">The file to be checked.</param>
        /// <param name="Checker">A delegate to check the file contents.</param>
        private static async Task<Boolean> checkFile(String                   FileName,
                                                     Func<String[], Boolean>  Checker)
        {

            var retries = 100;
            while (retries > 0)
            {

                try
                {

                    var lines = await File.ReadAllLinesAsync(FileName);

                    if (Checker(lines))
                        return true;

                }
                catch
                { }

                retries--;
                await Task.Delay(100);

            }

            return false;

        }

        #endregion


        #region SetupOnce()

        [OneTimeSetUp]
        public async virtual Task SetupOnce()
        {

            #region Create cpo1/cpo2/emsp1/emsp2 HTTP Servers

            cpo1HTTPServer           = new HTTPTestServerX(
                                           TCPPort:    IPPort.Parse(3301),
                                           DNSClient:  DNSClient,
                                           AutoStart:  true
                                       );

            cpo2HTTPServer           = new HTTPTestServerX(
                                           TCPPort:    IPPort.Parse(3302),
                                           DNSClient:  DNSClient,
                                           AutoStart:  true
                                       );

            emsp1HTTPServer          = new HTTPTestServerX(
                                           TCPPort:    IPPort.Parse(3401),
                                           DNSClient:  DNSClient,
                                           AutoStart:  true
                                       );

            emsp2HTTPServer          = new HTTPTestServerX(
                                           TCPPort:    IPPort.Parse(3402),
                                           DNSClient:  DNSClient,
                                           AutoStart:  true
                                       );

            Assert.That(cpo1HTTPServer,   Is.Not.Null);
            Assert.That(cpo2HTTPServer,   Is.Not.Null);

            Assert.That(emsp1HTTPServer,  Is.Not.Null);
            Assert.That(emsp2HTTPServer,  Is.Not.Null);

            #endregion

            #region Create cpo1/cpo2/emsp1/emsp2 HTTP APIs

            cpo1HTTPAPI          = new HTTPExtAPIX(
                                       HTTPServer:  cpo1HTTPServer
                                   );

            cpo2HTTPAPI          = new HTTPExtAPIX(
                                       HTTPServer:  cpo2HTTPServer
                                   );

            emsp1HTTPAPI         = new HTTPExtAPIX(
                                       HTTPServer:  emsp1HTTPServer
                                   );

            emsp2HTTPAPI         = new HTTPExtAPIX(
                                       HTTPServer:  emsp2HTTPServer
                                   );

            Assert.That(cpo1HTTPAPI,   Is.Not.Null);
            Assert.That(cpo2HTTPAPI,   Is.Not.Null);
            Assert.That(emsp1HTTPAPI,  Is.Not.Null);
            Assert.That(emsp2HTTPAPI,  Is.Not.Null);

            #endregion


            #region Create cpo1/cpo2/emsp1/emsp2 OCPI Common HTTP APIs

            var loggingPath = Path.Combine(AppContext.BaseDirectory, "default", "OCPI");

            #region logfileCreator

            static String logfileCreator(String         loggingPath2,
                                         IRemoteParty?  remoteParty,
                                         String         context,
                                         String         logfileName)

                => String.Concat(
                       loggingPath2, Path.DirectorySeparatorChar,
                       context.IsNotNullOrEmpty() ? context + Path.DirectorySeparatorChar : "",
                       logfileName, "_",
                       Timestamp.Now.Year, "-",
                       Timestamp.Now.Month.ToString("D2"),
                       ".log"
                   );

            #endregion


            #region CPO #1

            cpo1CommonHTTPAPI  = new CommonHTTPAPI(

                                     HTTPAPI:                     cpo1HTTPAPI,
                                     OurBaseURL:                  URL.Parse($"http://127.0.0.1:{cpo1HTTPServer.TCPPort}/ocpi"),
                                     OurVersionsURL:              URL.Parse($"http://127.0.0.1:{cpo1HTTPServer.TCPPort}/ocpi/versions"),

                                     Hostnames:                   null,
                                     RootPath:                    HTTPPath.Parse("/ocpi"),
                                     HTTPContentTypes:            null,
                                     Description:                 null,

                                     BasePath:                    null,

                                     ExternalDNSName:             null,
                                     HTTPServerName:              null,
                                     HTTPServiceName:             null,

                                     APIVersionHash:              null,
                                     APIVersionHashes:            null,

                                     APIRobotEMailAddress:        null,
                                     APIRobotGPGPassphrase:       null,
                                     SMTPClient:                  null,

                                     AdditionalURLPathPrefix:     null,
                                     LocationsAsOpenData:         true,
                                     TariffsAsOpenData:           true,
                                     AllowDowngrades:             null,

                                     IsDevelopment:               null,
                                     DevelopmentServers:          null,
                                     //SkipURLTemplates:            null,
                                     DatabaseFileName:            null,
                                     DisableNotifications:        null,

                                     DisableLogging:              false,
                                     LoggingContext:              "CommonHTTPAPI",
                                     LoggingPath:                 Path.Combine(loggingPath, "cpo1"),
                                     LogfileName:                 null,
                                     LogfileCreator:              logfileCreator

                                 );

            cpo1CommonHTTPAPI.Logger?.Debug("all", LogTargets.Debug);

            #endregion

            #region CPO #2

            cpo2CommonHTTPAPI  = new CommonHTTPAPI(

                                     HTTPAPI:                   cpo2HTTPAPI,
                                     OurBaseURL:                URL.Parse("http://127.0.0.1:3202/ocpi"),
                                     OurVersionsURL:            URL.Parse("http://127.0.0.1:3202/ocpi/versions"),

                                     Hostnames:                 null,
                                     RootPath:                  HTTPPath.Parse("/ocpi"),
                                     HTTPContentTypes:          null,
                                     Description:               null,

                                     BasePath:                  null,

                                     ExternalDNSName:           null,
                                     HTTPServerName:            null,
                                     HTTPServiceName:           null,

                                     APIVersionHash:            null,
                                     APIVersionHashes:          null,

                                     APIRobotEMailAddress:      null,
                                     APIRobotGPGPassphrase:     null,
                                     SMTPClient:                null,

                                     AdditionalURLPathPrefix:   null,
                                     LocationsAsOpenData:       true,
                                     TariffsAsOpenData:         true,
                                     AllowDowngrades:           null,

                                     IsDevelopment:             null,
                                     DevelopmentServers:        null,
                                     //SkipURLTemplates:          null,
                                     DatabaseFileName:          null,
                                     DisableNotifications:      null,

                                     DisableLogging:            false,
                                     LoggingContext:            "CommonHTTPAPI",
                                     LoggingPath:               Path.Combine(loggingPath, "cpo2"),
                                     LogfileName:               null,
                                     LogfileCreator:            logfileCreator

                                 );

            cpo2CommonHTTPAPI.Logger?.Debug("all", LogTargets.Debug);

            #endregion

            #region EMSP #1

            emsp1CommonHTTPAPI = new CommonHTTPAPI(

                                     HTTPAPI:                   emsp1HTTPAPI,
                                     OurBaseURL:                URL.Parse("http://127.0.0.1:3401/ocpi"),
                                     OurVersionsURL:            URL.Parse("http://127.0.0.1:3401/ocpi/versions"),

                                     Hostnames:                 null,
                                     RootPath:                  HTTPPath.Parse("/ocpi"),
                                     HTTPContentTypes:          null,
                                     Description:               null,

                                     BasePath:                  null,

                                     ExternalDNSName:           null,
                                     HTTPServerName:            null,
                                     HTTPServiceName:           null,

                                     APIVersionHash:            null,
                                     APIVersionHashes:          null,

                                     APIRobotEMailAddress:      null,
                                     APIRobotGPGPassphrase:     null,
                                     SMTPClient:                null,

                                     AdditionalURLPathPrefix:   null,
                                     LocationsAsOpenData:       true,
                                     TariffsAsOpenData:         true,
                                     AllowDowngrades:           null,

                                     IsDevelopment:             null,
                                     DevelopmentServers:        null,
                                     //SkipURLTemplates:          null,
                                     DatabaseFileName:          null,
                                     DisableNotifications:      null,

                                     DisableLogging:            false,
                                     LoggingContext:            "CommonHTTPAPI",
                                     LoggingPath:               Path.Combine(loggingPath, "emsp1"),
                                     LogfileName:               null,
                                     LogfileCreator:            logfileCreator

                                 );

            emsp1CommonHTTPAPI.Logger?.Debug("all", LogTargets.Debug);

            #endregion

            #region EMSP #2

            emsp2CommonHTTPAPI = new CommonHTTPAPI(

                                     HTTPAPI:                   emsp2HTTPAPI,
                                     OurBaseURL:                URL.Parse("http://127.0.0.1:3402/ocpi"),
                                     OurVersionsURL:            URL.Parse("http://127.0.0.1:3402/ocpi/versions"),

                                     Hostnames:                 null,
                                     RootPath:                  HTTPPath.Parse("/ocpi"),
                                     HTTPContentTypes:          null,
                                     Description:               null,

                                     BasePath:                  null,

                                     ExternalDNSName:           null,
                                     HTTPServerName:            null,
                                     HTTPServiceName:           null,

                                     APIVersionHash:            null,
                                     APIVersionHashes:          null,

                                     APIRobotEMailAddress:      null,
                                     APIRobotGPGPassphrase:     null,
                                     SMTPClient:                null,

                                     AdditionalURLPathPrefix:   null,
                                     LocationsAsOpenData:       true,
                                     TariffsAsOpenData:         true,
                                     AllowDowngrades:           null,

                                     IsDevelopment:             null,
                                     DevelopmentServers:        null,
                                     //SkipURLTemplates:          null,
                                     DatabaseFileName:          null,
                                     DisableNotifications:      null,

                                     DisableLogging:            false,
                                     LoggingContext:            "CommonHTTPAPI",
                                     LoggingPath:               Path.Combine(loggingPath, "emsp2"),
                                     LogfileName:               null,
                                     LogfileCreator:            logfileCreator

                                 );

            emsp2CommonHTTPAPI.Logger?.Debug("all", LogTargets.Debug);

            #endregion

            Assert.That(cpo1CommonHTTPAPI,   Is.Not.Null);
            Assert.That(cpo2CommonHTTPAPI,   Is.Not.Null);
            Assert.That(emsp1CommonHTTPAPI,  Is.Not.Null);
            Assert.That(emsp2CommonHTTPAPI,  Is.Not.Null);

            #endregion

            #region Create cpo1/cpo2/emsp1/emsp2 OCPI Common APIs

            // Clean up log and database directories...
            foreach (var filePath in Directory.GetFiles(
                                         Path.Combine(
                                             AppContext.BaseDirectory,
                                             HTTPAPI.DefaultHTTPAPI_LoggingPath
                                         ),
                                         $"GraphDefined_OCPI*.log"
                                     ))
            {
                File.Delete(filePath);
            }


            #region CPO #1

            cpo1CommonAPI_v2_1_1  = new OCPIv2_1_1.CommonAPI(

                                        OurBusinessDetails:                  new BusinessDetails(
                                                                                 $"GraphDefined OCPI {OCPIv2_1_1.Version.String} CPO #1 Services",
                                                                                 URL.Parse("https://www.graphdefined.com/cpo1")
                                                                             ),
                                        OurCountryCode:                      CountryCode.Parse("DE"),
                                        OurPartyId:                          Party_Id.   Parse("GEF"),
                                        OurRole:                             Role.       CPO,

                                        BaseAPI:                             cpo1CommonHTTPAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,

                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       null,
                                        APIVersionHashes:                    null,

                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_RemoteParties_CPO1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_Assets_CPO1.log",

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      false,
                                        LoggingPath:                         Path.Combine(loggingPath, "cpo1"),
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_CPO1.log",
                                        LogfileCreator:                      null

                                    );

            cpo1CommonAPI_v2_1_1.Logger?.Debug("all", LogTargets.Debug);


            cpo1CommonAPI_v2_2_1  = new OCPIv2_2_1.CommonAPI(

                                        OurPartyData:                        [
                                                                                 new OCPIv2_2_1.PartyData(
                                                                                     Party_Idv3.From(
                                                                                         CountryCode.Parse("DE"),
                                                                                         Party_Id.   Parse("GEF")
                                                                                     ),
                                                                                     Role.CPO,
                                                                                     new BusinessDetails(
                                                                                         $"GraphDefined OCPI {OCPIv2_2_1.Version.String} CPO #1 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/cpo1")
                                                                                     )
                                                                                 )
                                                                             ],
                                        DefaultPartyId:                      Party_Idv3.From(
                                                                                 CountryCode.Parse("DE"),
                                                                                 Party_Id.   Parse("GEF")
                                                                             ),

                                        BaseAPI:                             cpo1CommonHTTPAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,

                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       null,
                                        APIVersionHashes:                    null,

                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_RemoteParties_CPO1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_Assets_CPO1.log",

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      false,
                                        LoggingPath:                         Path.Combine(loggingPath, "cpo1"),
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_CPO1.log",
                                        LogfileCreator:                      null

                                    );

            cpo1CommonAPI_v2_2_1.Logger?.Debug("all", LogTargets.Debug);


            cpo1CommonAPI_v2_3_0  = new OCPIv2_3_0.CommonAPI(

                                        OurPartyData:                        [
                                                                                 new OCPIv2_3_0.PartyData(
                                                                                     Party_Idv3.From(
                                                                                         CountryCode.Parse("DE"),
                                                                                         Party_Id.   Parse("GEF")
                                                                                     ),
                                                                                     Role.CPO,
                                                                                     new BusinessDetails(
                                                                                         $"GraphDefined OCPI {OCPIv2_2_1.Version.String} CPO #1 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/cpo1")
                                                                                     )
                                                                                 )
                                                                             ],
                                        DefaultPartyId:                      Party_Idv3.From(
                                                                                 CountryCode.Parse("DE"),
                                                                                 Party_Id.   Parse("GEF")
                                                                             ),

                                        BaseAPI:                             cpo1CommonHTTPAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,

                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       null,
                                        APIVersionHashes:                    null,

                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_3_0.Version.String}_RemoteParties_CPO1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_3_0.Version.String}_Assets_CPO1.log",

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      false,
                                        LoggingPath:                         Path.Combine(loggingPath, "cpo1"),
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_3_0.Version.String}_CPO1.log",
                                        LogfileCreator:                      null

                                    );

            cpo1CommonAPI_v2_3_0.Logger?.Debug("all", LogTargets.Debug);


            #endregion

            #region CPO #2

            cpo2CommonAPI_v2_1_1  = new OCPIv2_1_1.CommonAPI(

                                        OurBusinessDetails:                  new BusinessDetails(
                                                                                 $"GraphDefined OCPI {OCPIv2_1_1.Version.String} CPO #2 Services",
                                                                                 URL.Parse("https://www.graphdefined.com/cpo2")
                                                                             ),
                                        OurCountryCode:                      CountryCode.Parse("DE"),
                                        OurPartyId:                          Party_Id.   Parse("GE2"),
                                        OurRole:                             Role.       CPO,

                                        BaseAPI:                             cpo2CommonHTTPAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,

                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       null,//HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_RemoteParties_CPO2.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_Assets_CPO2.log",

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      false,
                                        LoggingPath:                         Path.Combine(loggingPath, "cpo2"),
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_CPO2.log",
                                        LogfileCreator:                      null

                                    );

            cpo2CommonAPI_v2_1_1.Logger?.Debug("all", LogTargets.Debug);


            cpo2CommonAPI_v2_2_1  = new OCPIv2_2_1.CommonAPI(

                                        OurPartyData:                        [
                                                                                 new OCPIv2_2_1.PartyData(
                                                                                     Party_Idv3.From(
                                                                                         CountryCode.Parse("DE"),
                                                                                         Party_Id.   Parse("GE2")
                                                                                     ),
                                                                                     Role.CPO,
                                                                                     new BusinessDetails(
                                                                                         $"GraphDefined OCPI {OCPIv2_2_1.Version.String} CPO #2 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/cpo2")
                                                                                     ),
                                                                                     AllowDowngrades: true
                                                                                 )
                                                                             ],
                                        DefaultPartyId:                      Party_Idv3.From(
                                                                                 CountryCode.Parse("DE"),
                                                                                 Party_Id.   Parse("GE2")
                                                                             ),

                                        BaseAPI:                             cpo2CommonHTTPAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,

                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       null,//HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_RemoteParties_CPO2.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_Assets_CPO2.log",

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      false,
                                        LoggingPath:                         Path.Combine(loggingPath, "cpo2"),
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_CPO2.log",
                                        LogfileCreator:                      null

                                    );

            cpo2CommonAPI_v2_2_1.Logger?.Debug("all", LogTargets.Debug);


            cpo2CommonAPI_v2_3_0  = new OCPIv2_3_0.CommonAPI(

                                        OurPartyData:                        [
                                                                                 new OCPIv2_3_0.PartyData(
                                                                                     Party_Idv3.From(
                                                                                         CountryCode.Parse("DE"),
                                                                                         Party_Id.   Parse("GE2")
                                                                                     ),
                                                                                     Role.CPO,
                                                                                     new BusinessDetails(
                                                                                         $"GraphDefined OCPI {OCPIv2_2_1.Version.String} CPO #2 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/cpo2")
                                                                                     ),
                                                                                     AllowDowngrades: true
                                                                                 )
                                                                             ],
                                        DefaultPartyId:                      Party_Idv3.From(
                                                                                 CountryCode.Parse("DE"),
                                                                                 Party_Id.   Parse("GE2")
                                                                             ),

                                        BaseAPI:                             cpo2CommonHTTPAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,

                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       null,//HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_3_0.Version.String}_RemoteParties_CPO2.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_3_0.Version.String}_Assets_CPO2.log",

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      false,
                                        LoggingPath:                         Path.Combine(loggingPath, "cpo2"),
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_3_0.Version.String}_CPO2.log",
                                        LogfileCreator:                      null

                                    );

            cpo1CommonAPI_v2_3_0.Logger?.Debug("all", LogTargets.Debug);

            #endregion

            #region EMSP #1

            emsp1CommonAPI_v2_1_1 = new OCPIv2_1_1.CommonAPI(

                                        OurBusinessDetails:                  new BusinessDetails(
                                                                                 $"GraphDefined OCPI {OCPIv2_1_1.Version.String} EMSP #1 Services",
                                                                                 URL.Parse("https://www.graphdefined.com/emsp1")
                                                                             ),
                                        OurCountryCode:                      CountryCode.Parse("DE"),
                                        OurPartyId:                          Party_Id.   Parse("GDF"),
                                        OurRole:                             Role.       EMSP,

                                        BaseAPI:                             emsp1CommonHTTPAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,

                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       null,//HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_RemoteParties_EMSP1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_Assets_EMSP1.log",

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      false,
                                        LoggingPath:                         Path.Combine(loggingPath, "emsp1"),
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_EMSP1.log",
                                        LogfileCreator:                      null

                                    );

            emsp1CommonAPI_v2_1_1.Logger?.Debug("all", LogTargets.Debug);


            emsp1CommonAPI_v2_2_1 = new OCPIv2_2_1.CommonAPI(

                                        OurPartyData:                        [
                                                                                 new OCPIv2_2_1.PartyData(
                                                                                     Party_Idv3.From(
                                                                                         CountryCode.Parse("DE"),
                                                                                         Party_Id.   Parse("GDF")
                                                                                     ),
                                                                                     Role.EMSP,
                                                                                     new BusinessDetails(
                                                                                         $"GraphDefined OCPI {OCPIv2_2_1.Version.String} EMSP #1 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/emsp1")
                                                                                     ),
                                                                                     AllowDowngrades: true
                                                                                 )
                                                                             ],
                                        DefaultPartyId:                      Party_Idv3.From(
                                                                                 CountryCode.Parse("DE"),
                                                                                 Party_Id.   Parse("GDF")
                                                                             ),

                                        BaseAPI:                             emsp1CommonHTTPAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,

                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       null,//HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_RemoteParties_EMSP1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_Assets_EMSP1.log",

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      false,
                                        LoggingPath:                         Path.Combine(loggingPath, "emsp1"),
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_EMSP1.log",
                                        LogfileCreator:                      null

                                    );

            emsp1CommonAPI_v2_2_1.Logger?.Debug("all", LogTargets.Debug);


            emsp1CommonAPI_v2_3_0 = new OCPIv2_3_0.CommonAPI(

                                        OurPartyData:                        [
                                                                                 new OCPIv2_3_0.PartyData(
                                                                                     Party_Idv3.From(
                                                                                         CountryCode.Parse("DE"),
                                                                                         Party_Id.   Parse("GDF")
                                                                                     ),
                                                                                     Role.EMSP,
                                                                                     new BusinessDetails(
                                                                                         $"GraphDefined OCPI {OCPIv2_3_0.Version.String} EMSP #1 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/emsp1")
                                                                                     ),
                                                                                     AllowDowngrades: true
                                                                                 )
                                                                             ],
                                        DefaultPartyId:                      Party_Idv3.From(
                                                                                 CountryCode.Parse("DE"),
                                                                                 Party_Id.   Parse("GDF")
                                                                             ),

                                        BaseAPI:                             emsp1CommonHTTPAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,

                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       null,//HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_3_0.Version.String}_RemoteParties_EMSP1.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_3_0.Version.String}_Assets_EMSP1.log",

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      false,
                                        LoggingPath:                         Path.Combine(loggingPath, "emsp1"),
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_3_0.Version.String}_EMSP1.log",
                                        LogfileCreator:                      null

                                    );

            emsp1CommonAPI_v2_3_0.Logger?.Debug("all", LogTargets.Debug);

            #endregion

            #region EMSP #2

            emsp2CommonAPI_v2_1_1 = new OCPIv2_1_1.CommonAPI(

                                        OurBusinessDetails:                  new BusinessDetails(
                                                                                 $"GraphDefined OCPI {OCPIv2_1_1.Version.String} EMSP #2 Services",
                                                                                 URL.Parse("https://www.graphdefined.com/emsp2")
                                                                             ),
                                        OurCountryCode:                      CountryCode.Parse("DE"),
                                        OurPartyId:                          Party_Id.   Parse("GD2"),
                                        OurRole:                             Role.       EMSP,

                                        BaseAPI:                             emsp2CommonHTTPAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,

                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       null,//HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_RemoteParties_EMSP2.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_Assets_EMSP2.log",

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      false,
                                        LoggingPath:                         Path.Combine(loggingPath, "emsp2"),
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_1_1.Version.String}_EMSP2.log",
                                        LogfileCreator:                      null

                                    );

            emsp2CommonAPI_v2_1_1.Logger?.Debug("all", LogTargets.Debug);


            emsp2CommonAPI_v2_2_1 = new OCPIv2_2_1.CommonAPI(

                                        OurPartyData:                        [
                                                                                 new OCPIv2_2_1.PartyData(
                                                                                     Party_Idv3.From(
                                                                                         CountryCode.Parse("DE"),
                                                                                         Party_Id.   Parse("GD2")
                                                                                     ),
                                                                                     Role.EMSP,
                                                                                     new BusinessDetails(
                                                                                         $"GraphDefined OCPI {OCPIv2_2_1.Version.String} EMSP #2 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/emsp2")
                                                                                     ),
                                                                                     AllowDowngrades: true
                                                                                 )
                                                                             ],
                                        DefaultPartyId:                      Party_Idv3.From(
                                                                                 CountryCode.Parse("DE"),
                                                                                 Party_Id.   Parse("GD2")
                                                                             ),

                                        BaseAPI:                             emsp2CommonHTTPAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,

                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       null,//HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_RemoteParties_EMSP2.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_Assets_EMSP2.log",

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      false,
                                        LoggingPath:                         Path.Combine(loggingPath, "emsp2"),
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_2_1.Version.String}_EMSP2.log",
                                        LogfileCreator:                      null

                                    );

            emsp2CommonAPI_v2_2_1.Logger?.Debug("all", LogTargets.Debug);


            emsp2CommonAPI_v2_3_0 = new OCPIv2_3_0.CommonAPI(

                                        OurPartyData:                        [
                                                                                 new OCPIv2_3_0.PartyData(
                                                                                     Party_Idv3.From(
                                                                                         CountryCode.Parse("DE"),
                                                                                         Party_Id.   Parse("GD2")
                                                                                     ),
                                                                                     Role.EMSP,
                                                                                     new BusinessDetails(
                                                                                         $"GraphDefined OCPI {OCPIv2_3_0.Version.String} EMSP #2 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/emsp2")
                                                                                     ),
                                                                                     AllowDowngrades: true
                                                                                 )
                                                                             ],
                                        DefaultPartyId:                      Party_Idv3.From(
                                                                                 CountryCode.Parse("DE"),
                                                                                 Party_Id.   Parse("GD2")
                                                                             ),

                                        BaseAPI:                             emsp2CommonHTTPAPI,

                                        AdditionalURLPathPrefix:             null,
                                        KeepRemovedEVSEs:                    null,

                                        ExternalDNSName:                     null,
                                        HTTPServiceName:                     null,
                                        BasePath:                            null,

                                        URLPathPrefix:                       null,//HTTPPath.Parse("/ocpi"),
                                        APIVersionHashes:                    null,

                                        DatabaseFilePath:                    null,
                                        RemotePartyDBFileName:               $"GraphDefined_OCPI{OCPIv2_3_0.Version.String}_RemoteParties_EMSP2.log",
                                        AssetsDBFileName:                    $"GraphDefined_OCPI{OCPIv2_3_0.Version.String}_Assets_EMSP2.log",

                                        IsDevelopment:                       null,
                                        DevelopmentServers:                  null,
                                        DisableLogging:                      false,
                                        LoggingPath:                         Path.Combine(loggingPath, "emsp2"),
                                        LogfileName:                         $"GraphDefined_OCPI{OCPIv2_3_0.Version.String}_EMSP2.log",
                                        LogfileCreator:                      null

                                    );

            emsp2CommonAPI_v2_3_0.Logger?.Debug("all", LogTargets.Debug);

            #endregion

            Assert.That(cpo1CommonAPI_v2_1_1,   Is.Not.Null);
            Assert.That(cpo1CommonAPI_v2_2_1,   Is.Not.Null);
            Assert.That(cpo1CommonAPI_v2_3_0,   Is.Not.Null);

            Assert.That(cpo2CommonAPI_v2_1_1,   Is.Not.Null);
            Assert.That(cpo2CommonAPI_v2_2_1,   Is.Not.Null);
            Assert.That(cpo2CommonAPI_v2_3_0,   Is.Not.Null);

            Assert.That(emsp1CommonAPI_v2_1_1,  Is.Not.Null);
            Assert.That(emsp1CommonAPI_v2_2_1,  Is.Not.Null);
            Assert.That(emsp1CommonAPI_v2_3_0,  Is.Not.Null);

            Assert.That(emsp2CommonAPI_v2_1_1,  Is.Not.Null);
            Assert.That(emsp2CommonAPI_v2_2_1,  Is.Not.Null);
            Assert.That(emsp2CommonAPI_v2_3_0,  Is.Not.Null);

            #endregion

            #region Create cpo1/cpo2 CPOAPIs & emsp1/emsp2 EMPAPIs

            #region CPO #1

            cpo1CPOAPI_v2_1_1    = new OCPIv2_1_1.CPOAPI(

                                       CommonAPI:                           cpo1CommonAPI_v2_1_1,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            cpo1CPOAPI_v2_1_1.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                        (loggingPath, context, logEventName, request) => {
                                                            cpo1APIRequestLogs_v2_1_1. TryAdd(Timestamp.Now, request);
                                                            return Task.CompletedTask;
                                                        });

            cpo1CPOAPI_v2_1_1.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                        (loggingPath, context, logEventName, request, response) => {
                                                            cpo1APIResponseLogs_v2_1_1.TryAdd(Timestamp.Now, response);
                                                            return Task.CompletedTask;
                                                        });

            cpo1CPOAPI_v2_1_1.Logger?.Debug("all", LogTargets.Debug);


            cpo1CPOAPI_v2_2_1    = new OCPIv2_2_1.CPOAPI(

                                       CommonAPI:                           cpo1CommonAPI_v2_2_1,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            cpo1CPOAPI_v2_2_1.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                        (loggingPath, context, logEventName, request) => {
                                                            cpo1APIRequestLogs_v2_2_1. TryAdd(Timestamp.Now, request);
                                                            return Task.CompletedTask;
                                                        });

            cpo1CPOAPI_v2_2_1.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                        (loggingPath, context, logEventName, request, response) => {
                                                            cpo1APIResponseLogs_v2_2_1.TryAdd(Timestamp.Now, response);
                                                            return Task.CompletedTask;
                                                        });

            cpo1CPOAPI_v2_2_1.Logger?.Debug("all", LogTargets.Debug);


            cpo1CPOAPI_v2_3_0    = new OCPIv2_3_0.CPOAPI(

                                       CommonAPI:                           cpo1CommonAPI_v2_3_0,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            cpo1CPOAPI_v2_3_0.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                        (loggingPath, context, logEventName, request) => {
                                                            cpo1APIRequestLogs_v2_3_0. TryAdd(Timestamp.Now, request);
                                                            return Task.CompletedTask;
                                                        });

            cpo1CPOAPI_v2_3_0.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                        (loggingPath, context, logEventName, request, response) => {
                                                            cpo1APIResponseLogs_v2_3_0.TryAdd(Timestamp.Now, response);
                                                            return Task.CompletedTask;
                                                        });

            cpo1CPOAPI_v2_3_0.Logger?.Debug("all", LogTargets.Debug);

            #endregion

            #region CPO #2

            cpo2CPOAPI_v2_1_1    = new OCPIv2_1_1.CPOAPI(

                                       CommonAPI:                           cpo2CommonAPI_v2_1_1,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            cpo2CPOAPI_v2_1_1.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                        (loggingPath, context, logEventName, request) => {
                                                            cpo2APIRequestLogs_v2_1_1. TryAdd(Timestamp.Now, request);
                                                            return Task.CompletedTask;
                                                        });

            cpo2CPOAPI_v2_1_1.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                        (loggingPath, context, logEventName, request, response) => {
                                                            cpo2APIResponseLogs_v2_1_1.TryAdd(Timestamp.Now, response);
                                                            return Task.CompletedTask;
                                                        });

            cpo2CPOAPI_v2_1_1.Logger?.Debug("all", LogTargets.Debug);


            cpo2CPOAPI_v2_2_1    = new OCPIv2_2_1.CPOAPI(

                                       CommonAPI:                           cpo2CommonAPI_v2_2_1,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            cpo2CPOAPI_v2_2_1.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                        (loggingPath, context, logEventName, request) => {
                                                            cpo2APIRequestLogs_v2_2_1. TryAdd(Timestamp.Now, request);
                                                            return Task.CompletedTask;
                                                        });

            cpo2CPOAPI_v2_2_1.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                        (loggingPath, context, logEventName, request, response) => {
                                                            cpo2APIResponseLogs_v2_2_1.TryAdd(Timestamp.Now, response);
                                                            return Task.CompletedTask;
                                                        });

            cpo2CPOAPI_v2_2_1.Logger?.Debug("all", LogTargets.Debug);


            cpo2CPOAPI_v2_3_0    = new OCPIv2_3_0.CPOAPI(

                                       CommonAPI:                           cpo2CommonAPI_v2_3_0,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            cpo2CPOAPI_v2_3_0.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                        (loggingPath, context, logEventName, request) => {
                                                            cpo2APIRequestLogs_v2_3_0. TryAdd(Timestamp.Now, request);
                                                            return Task.CompletedTask;
                                                        });

            cpo2CPOAPI_v2_3_0.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                        (loggingPath, context, logEventName, request, response) => {
                                                            cpo2APIResponseLogs_v2_3_0.TryAdd(Timestamp.Now, response);
                                                            return Task.CompletedTask;
                                                        });

            cpo2CPOAPI_v2_3_0.Logger?.Debug("all", LogTargets.Debug);

            #endregion

            #region EMSP #1

            emsp1EMSPAPI_v2_1_1 = new OCPIv2_1_1.EMSPAPI(

                                       CommonAPI:                           emsp1CommonAPI_v2_1_1,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            emsp1EMSPAPI_v2_1_1.Logger?.Debug("all", LogTargets.Debug);


            emsp1EMSPAPI_v2_2_1 = new OCPIv2_2_1.EMSPAPI(

                                       CommonAPI:                           emsp1CommonAPI_v2_2_1,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            emsp1EMSPAPI_v2_2_1.Logger?.Debug("all", LogTargets.Debug);


            emsp1EMSPAPI_v2_3_0 = new OCPIv2_3_0.EMSPAPI(

                                       CommonAPI:                           emsp1CommonAPI_v2_3_0,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            emsp1EMSPAPI_v2_3_0.Logger?.Debug("all", LogTargets.Debug);

            #endregion

            #region EMSP #2

            emsp2EMSPAPI_v2_1_1 = new OCPIv2_1_1.EMSPAPI(

                                       CommonAPI:                           emsp2CommonAPI_v2_1_1,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            emsp2EMSPAPI_v2_1_1.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request) => {
                                                              emsp2APIRequestLogs_v2_1_1. TryAdd(Timestamp.Now, request);
                                                              return Task.CompletedTask;
                                                          });

            emsp2EMSPAPI_v2_1_1.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request, response) => {
                                                              emsp2APIResponseLogs_v2_1_1.TryAdd(Timestamp.Now, response);
                                                              return Task.CompletedTask;
                                                          });

            emsp2EMSPAPI_v2_1_1.Logger?.Debug("all", LogTargets.Debug);


            emsp2EMSPAPI_v2_2_1 = new OCPIv2_2_1.EMSPAPI(

                                       CommonAPI:                           emsp2CommonAPI_v2_2_1,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            emsp2EMSPAPI_v2_2_1.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request) => {
                                                              emsp2APIRequestLogs_v2_2_1. TryAdd(Timestamp.Now, request);
                                                              return Task.CompletedTask;
                                                          });

            emsp2EMSPAPI_v2_2_1.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request, response) => {
                                                              emsp2APIResponseLogs_v2_2_1.TryAdd(Timestamp.Now, response);
                                                              return Task.CompletedTask;
                                                          });

            emsp2EMSPAPI_v2_2_1.Logger?.Debug("all", LogTargets.Debug);


            emsp2EMSPAPI_v2_3_0 = new OCPIv2_3_0.EMSPAPI(

                                       CommonAPI:                           emsp2CommonAPI_v2_3_0,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       null,
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            emsp2EMSPAPI_v2_3_0.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request) => {
                                                              emsp2APIRequestLogs_v2_3_0. TryAdd(Timestamp.Now, request);
                                                              return Task.CompletedTask;
                                                          });

            emsp2EMSPAPI_v2_3_0.Logger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request, response) => {
                                                              emsp2APIResponseLogs_v2_3_0.TryAdd(Timestamp.Now, response);
                                                              return Task.CompletedTask;
                                                          });

            emsp2EMSPAPI_v2_3_0.Logger?.Debug("all", LogTargets.Debug);

            #endregion

            Assert.That(cpo1CPOAPI_v2_1_1,    Is.Not.Null);
            Assert.That(cpo1CPOAPI_v2_2_1,    Is.Not.Null);
            Assert.That(cpo1CPOAPI_v2_3_0,    Is.Not.Null);

            Assert.That(cpo2CPOAPI_v2_1_1,    Is.Not.Null);
            Assert.That(cpo2CPOAPI_v2_2_1,    Is.Not.Null);
            Assert.That(cpo2CPOAPI_v2_3_0,    Is.Not.Null);

            Assert.That(emsp1EMSPAPI_v2_1_1,  Is.Not.Null);
            Assert.That(emsp1EMSPAPI_v2_2_1,  Is.Not.Null);
            Assert.That(emsp1EMSPAPI_v2_3_0,  Is.Not.Null);

            Assert.That(emsp2EMSPAPI_v2_1_1,  Is.Not.Null);
            Assert.That(emsp2EMSPAPI_v2_2_1,  Is.Not.Null);
            Assert.That(emsp2EMSPAPI_v2_3_0,  Is.Not.Null);

            #endregion


            #region Create cpo1/cpo2/emsp1/emsp2 OCPI (Common) Web APIs

            #region CPO #1

            cpo1CommonWebAPI   = new CommonWebAPI(

                                     cpo1CommonHTTPAPI,

                                     OverlayURLPathPrefix:  null,
                                     APIURLPathPrefix:      HTTPPath.Parse("api"),
                                     WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                     BasePath:              null,

                                     Description:           null,

                                     HTTPRealm:             "GraphDefined OCPI CPO #1 WebAPI",
                                     HTTPLogins:            [
                                                                new KeyValuePair<String, String>("a", "b")
                                                            ],

                                     ExternalDNSName:       null,
                                     HTTPServerName:        null,
                                     HTTPServiceName:       null,
                                     APIVersionHash:        null,
                                     APIVersionHashes:      null,

                                     IsDevelopment:         null,
                                     DevelopmentServers:    null,
                                     DisableNotifications:  null,
                                     DisableLogging:        null,
                                     LoggingPath:           null,
                                     LogfileName:           null,
                                     LogfileCreator:        null

                                 );

            cpo1WebAPI_v2_1_1  = new OCPIv2_1_1.WebAPI.OCPIWebAPI(

                                     cpo1CommonWebAPI,
                                     cpo1CommonAPI_v2_1_1,

                                     APIURLPathPrefix:      HTTPPath.Parse("api"),
                                     WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                     BasePath:              null

                                 );

            cpo1WebAPI_v2_2_1  = new OCPIv2_2_1.WebAPI.OCPIWebAPI(

                                     cpo1CommonWebAPI,
                                     cpo1CommonAPI_v2_2_1,

                                     APIURLPathPrefix:      HTTPPath.Parse("api"),
                                     WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                     BasePath:              null

                                 );

            cpo1WebAPI_v2_3_0  = new OCPIv2_3_0.WebAPI.OCPIWebAPI(

                                     cpo1CommonWebAPI,
                                     cpo1CommonAPI_v2_3_0,

                                     APIURLPathPrefix:      HTTPPath.Parse("api"),
                                     WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                     BasePath:              null

                                 );

            #endregion

            #region CPO #2

            cpo2CommonWebAPI   = new CommonWebAPI(

                                     cpo2CommonHTTPAPI,

                                     OverlayURLPathPrefix:  null,
                                     APIURLPathPrefix:      HTTPPath.Parse("api"),
                                     WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                     BasePath:              null,

                                     Description:           null,

                                     HTTPRealm:             "GraphDefined OCPI CPO #2 WebAPI",
                                     HTTPLogins:            [
                                                                new KeyValuePair<String, String>("a", "b")
                                                            ],

                                     ExternalDNSName:       null,
                                     HTTPServerName:        null,
                                     HTTPServiceName:       null,
                                     APIVersionHash:        null,
                                     APIVersionHashes:      null,

                                     IsDevelopment:         null,
                                     DevelopmentServers:    null,
                                     DisableNotifications:  null,
                                     DisableLogging:        null,
                                     LoggingPath:           null,
                                     LogfileName:           null,
                                     LogfileCreator:        null

                                 );

            cpo2WebAPI_v2_1_1  = new OCPIv2_1_1.WebAPI.OCPIWebAPI(

                                     cpo2CommonWebAPI,
                                     cpo2CommonAPI_v2_1_1,

                                     APIURLPathPrefix:      HTTPPath.Parse("api"),
                                     WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                     BasePath:              null

                                 );

            cpo2WebAPI_v2_2_1  = new OCPIv2_2_1.WebAPI.OCPIWebAPI(

                                     cpo2CommonWebAPI,
                                     cpo2CommonAPI_v2_2_1,

                                     APIURLPathPrefix:      HTTPPath.Parse("api"),
                                     WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                     BasePath:              null

                                 );

            cpo2WebAPI_v2_3_0  = new OCPIv2_3_0.WebAPI.OCPIWebAPI(

                                     cpo2CommonWebAPI,
                                     cpo2CommonAPI_v2_3_0,

                                     APIURLPathPrefix:      HTTPPath.Parse("api"),
                                     WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                     BasePath:              null

                                 );

            #endregion

            #region EMSP #1

            emsp1CommonWebAPI   = new CommonWebAPI(

                                      emsp1CommonHTTPAPI,

                                      OverlayURLPathPrefix:  null,
                                      APIURLPathPrefix:      HTTPPath.Parse("api"),
                                      WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                      BasePath:              null,

                                      Description:           null,

                                      HTTPRealm:             "GraphDefined OCPI EMSP #1 WebAPI",
                                      HTTPLogins:            [
                                                                 new KeyValuePair<String, String>("a", "b")
                                                             ],

                                      ExternalDNSName:       null,
                                      HTTPServerName:        null,
                                      HTTPServiceName:       null,
                                      APIVersionHash:        null,
                                      APIVersionHashes:      null,

                                      IsDevelopment:         null,
                                      DevelopmentServers:    null,
                                      DisableNotifications:  null,
                                      DisableLogging:        null,
                                      LoggingPath:           null,
                                      LogfileName:           null,
                                      LogfileCreator:        null

                                  );

            emsp1WebAPI_v2_1_1  = new OCPIv2_1_1.WebAPI.OCPIWebAPI(

                                      emsp1CommonWebAPI,
                                      emsp1CommonAPI_v2_1_1,

                                      APIURLPathPrefix:      HTTPPath.Parse("api"),
                                      WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                      BasePath:              null

                                  );

            emsp1WebAPI_v2_2_1  = new OCPIv2_2_1.WebAPI.OCPIWebAPI(

                                      emsp1CommonWebAPI,
                                      emsp1CommonAPI_v2_2_1,

                                      APIURLPathPrefix:      HTTPPath.Parse("api"),
                                      WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                      BasePath:              null

                                  );

            emsp1WebAPI_v2_3_0  = new OCPIv2_3_0.WebAPI.OCPIWebAPI(

                                      emsp1CommonWebAPI,
                                      emsp1CommonAPI_v2_3_0,

                                      APIURLPathPrefix:      HTTPPath.Parse("api"),
                                      WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                      BasePath:              null

                                  );

            #endregion

            #region EMSP #2

            emsp2CommonWebAPI   = new CommonWebAPI(

                                      emsp2CommonHTTPAPI,

                                      OverlayURLPathPrefix:  null,
                                      APIURLPathPrefix:      HTTPPath.Parse("api"),
                                      WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                      BasePath:              null,

                                      Description:           null,

                                      HTTPRealm:             "GraphDefined OCPI EMSP #2 WebAPI",
                                      HTTPLogins:            [
                                                                 new KeyValuePair<String, String>("a", "b")
                                                             ],

                                      ExternalDNSName:       null,
                                      HTTPServerName:        null,
                                      HTTPServiceName:       null,
                                      APIVersionHash:        null,
                                      APIVersionHashes:      null,

                                      IsDevelopment:         null,
                                      DevelopmentServers:    null,
                                      DisableNotifications:  null,
                                      DisableLogging:        null,
                                      LoggingPath:           null,
                                      LogfileName:           null,
                                      LogfileCreator:        null

                                  );

            emsp2WebAPI_v2_1_1  = new OCPIv2_1_1.WebAPI.OCPIWebAPI(

                                      emsp2CommonWebAPI,
                                      emsp2CommonAPI_v2_1_1,

                                      APIURLPathPrefix:      HTTPPath.Parse("api"),
                                      WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                      BasePath:              null

                                  );

            emsp2WebAPI_v2_2_1  = new OCPIv2_2_1.WebAPI.OCPIWebAPI(

                                      emsp2CommonWebAPI,
                                      emsp2CommonAPI_v2_2_1,

                                      APIURLPathPrefix:      HTTPPath.Parse("api"),
                                      WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                      BasePath:              null

                                  );

            emsp2WebAPI_v2_3_0  = new OCPIv2_3_0.WebAPI.OCPIWebAPI(

                                      emsp2CommonWebAPI,
                                      emsp2CommonAPI_v2_3_0,

                                      APIURLPathPrefix:      HTTPPath.Parse("api"),
                                      WebAPIURLPathPrefix:   HTTPPath.Parse("webapi"),
                                      BasePath:              null

                                  );

            #endregion

            Assert.That(cpo1CommonWebAPI,    Is.Not.Null);
            Assert.That(cpo1WebAPI_v2_1_1,   Is.Not.Null);
            Assert.That(cpo1WebAPI_v2_2_1,   Is.Not.Null);
            Assert.That(cpo1WebAPI_v2_3_0,   Is.Not.Null);

            Assert.That(cpo2CommonWebAPI,    Is.Not.Null);
            Assert.That(cpo2WebAPI_v2_1_1,   Is.Not.Null);
            Assert.That(cpo2WebAPI_v2_2_1,   Is.Not.Null);
            Assert.That(cpo2WebAPI_v2_3_0,   Is.Not.Null);

            Assert.That(emsp1CommonWebAPI,   Is.Not.Null);
            Assert.That(emsp1WebAPI_v2_1_1,  Is.Not.Null);
            Assert.That(emsp1WebAPI_v2_2_1,  Is.Not.Null);
            Assert.That(emsp1WebAPI_v2_3_0,  Is.Not.Null);

            Assert.That(emsp2CommonWebAPI,   Is.Not.Null);
            Assert.That(emsp2WebAPI_v2_1_1,  Is.Not.Null);
            Assert.That(emsp2WebAPI_v2_2_1,  Is.Not.Null);
            Assert.That(emsp2WebAPI_v2_3_0,  Is.Not.Null);

            #endregion


            #region Define Client Configurations

            //cpo1CommonAPI. BaseAPI.ClientConfigurations.Description = (remotePartyId) => I18NString.Create($"CPO #1 Client for {remotePartyId}");

            //cpo2CommonAPI. BaseAPI.ClientConfigurations.Description = (remotePartyId) => I18NString.Create($"CPO #2 Client for {remotePartyId}");

            //emsp1CommonAPI.BaseAPI.ClientConfigurations.Description = (remotePartyId) => I18NString.Create($"EMSP #1 Client for {remotePartyId}");

            //emsp2CommonAPI.BaseAPI.ClientConfigurations.Description = (remotePartyId) => I18NString.Create($"EMSP #2 Client for {remotePartyId}");

            #endregion

            #region Define and connect Remote Parties

            #region CPO #1 -> EMSP #1

            Assert.That(
                await emsp1CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                          CountryCode:                 cpo1CommonAPI_v2_1_1.OurCountryCode,
                          PartyId:                     cpo1CommonAPI_v2_1_1.OurPartyId,
                          Role:                        Role.CPO,
                          BusinessDetails:             cpo1CommonAPI_v2_1_1.OurBusinessDetails,

                          AccessToken:                 AccessToken.Parse(cpo1_accessing_emsp1__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(emsp1_accessing_cpo1__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_2_1.Version.Id,
                          AccessTokenBase64Encoding:   false,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await emsp1CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           cpo1CommonAPI_v2_2_1.DefaultPartyId,
                                                           Role.CPO
                                                       ),
                          CredentialsRoles:            [
                                                           cpo1CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                       ],
                          AccessToken:                 AccessToken.Parse(cpo1_accessing_emsp1__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(emsp1_accessing_cpo1__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_2_1.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await emsp1CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           cpo1CommonAPI_v2_3_0.DefaultPartyId,
                                                           Role.CPO
                                                       ),
                          CredentialsRoles:            [
                                                           cpo1CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                       ],
                          AccessToken:                 AccessToken.Parse(cpo1_accessing_emsp1__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(emsp1_accessing_cpo1__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_3_0.Version.Id ],
                          SelectedVersionId:           OCPIv2_3_0.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            #endregion

            #region CPO #1 -> EMSP #2

            Assert.That(
                await emsp2CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                          CountryCode:                 cpo1CommonAPI_v2_1_1.OurCountryCode,
                          PartyId:                     cpo1CommonAPI_v2_1_1.OurPartyId,
                          Role:                        Role.CPO,
                          BusinessDetails:             cpo1CommonAPI_v2_1_1.OurBusinessDetails,

                          AccessToken:                 AccessToken.Parse(cpo1_accessing_emsp2__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(emsp2_accessing_cpo1__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_2_1.Version.Id,
                          AccessTokenBase64Encoding:   false,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await emsp2CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           cpo1CommonAPI_v2_2_1.DefaultPartyId,
                                                           Role.CPO
                                                       ),
                          CredentialsRoles:            [
                                                           cpo1CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                       ],
                          AccessToken:                 AccessToken.Parse(cpo1_accessing_emsp2__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(emsp2_accessing_cpo1__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_2_1.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await emsp2CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           cpo1CommonAPI_v2_3_0.DefaultPartyId,
                                                           Role.CPO
                                                       ),
                          CredentialsRoles:            [
                                                           cpo1CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                       ],
                          AccessToken:                 AccessToken.Parse(cpo1_accessing_emsp2__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(emsp2_accessing_cpo1__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_3_0.Version.Id ],
                          SelectedVersionId:           OCPIv2_3_0.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            #endregion


            #region CPO #2 -> EMSP #1

            Assert.That(
                await emsp1CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                          CountryCode:                 cpo2CommonAPI_v2_1_1.OurCountryCode,
                          PartyId:                     cpo2CommonAPI_v2_1_1.OurPartyId,
                          Role:                        Role.CPO,
                          BusinessDetails:             cpo2CommonAPI_v2_1_1.OurBusinessDetails,

                          AccessToken:                 AccessToken.Parse(cpo2_accessing_emsp1__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(emsp1_accessing_cpo2__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo2HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_2_1.Version.Id,
                          AccessTokenBase64Encoding:   false,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await emsp1CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           cpo2CommonAPI_v2_2_1.DefaultPartyId,
                                                           Role.CPO
                                                       ),
                          CredentialsRoles:            [
                                                           cpo2CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                       ],
                          AccessToken:                 AccessToken.Parse(cpo2_accessing_emsp1__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(emsp1_accessing_cpo2__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo2HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_2_1.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await emsp1CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           cpo2CommonAPI_v2_3_0.DefaultPartyId,
                                                           Role.CPO
                                                       ),
                          CredentialsRoles:            [
                                                           cpo2CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                       ],
                          AccessToken:                 AccessToken.Parse(cpo2_accessing_emsp1__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(emsp1_accessing_cpo2__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo2HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_3_0.Version.Id ],
                          SelectedVersionId:           OCPIv2_3_0.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            #endregion

            #region CPO #2 -> EMSP #2

            Assert.That(
                await emsp2CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                          CountryCode:                 cpo2CommonAPI_v2_1_1.OurCountryCode,
                          PartyId:                     cpo2CommonAPI_v2_1_1.OurPartyId,
                          Role:                        Role.CPO,
                          BusinessDetails:             cpo2CommonAPI_v2_1_1.OurBusinessDetails,

                          AccessToken:                 AccessToken.Parse(cpo2_accessing_emsp2__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(emsp2_accessing_cpo2__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo2HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_2_1.Version.Id,
                          AccessTokenBase64Encoding:   false,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await emsp2CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           cpo2CommonAPI_v2_2_1.DefaultPartyId,
                                                           Role.CPO
                                                       ),
                          CredentialsRoles:            [
                                                           cpo2CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                       ],
                          AccessToken:                 AccessToken.Parse(cpo2_accessing_emsp2__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(emsp2_accessing_cpo2__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo2HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_2_1.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await emsp2CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           cpo2CommonAPI_v2_3_0.DefaultPartyId,
                                                           Role.CPO
                                                       ),
                          CredentialsRoles:            [
                                                           cpo2CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                       ],
                          AccessToken:                 AccessToken.Parse(cpo2_accessing_emsp2__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(emsp2_accessing_cpo2__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo2HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_3_0.Version.Id ],
                          SelectedVersionId:           OCPIv2_3_0.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            #endregion


            #region EMSP #1 -> CPO #1

            Assert.That(
                await cpo1CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                          CountryCode:                 emsp1CommonAPI_v2_1_1.OurCountryCode,
                          PartyId:                     emsp1CommonAPI_v2_1_1.OurPartyId,
                          Role:                        Role.EMSP,
                          BusinessDetails:             emsp1CommonAPI_v2_1_1.OurBusinessDetails,

                          AccessToken:                 AccessToken.Parse(emsp1_accessing_cpo1__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(cpo1_accessing_emsp1__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_1_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_1_1.Version.Id,
                          AccessTokenBase64Encoding:   false,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await cpo1CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           emsp1CommonAPI_v2_2_1.DefaultPartyId,
                                                           Role.EMSP
                                                       ),
                          CredentialsRoles:            [
                                                           emsp1CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                       ],

                          AccessToken:                 AccessToken.Parse(emsp1_accessing_cpo1__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(cpo1_accessing_emsp1__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_2_1.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await cpo1CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           emsp1CommonAPI_v2_3_0.DefaultPartyId,
                                                           Role.EMSP
                                                       ),
                          CredentialsRoles:            [
                                                           emsp1CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                       ],

                          AccessToken:                 AccessToken.Parse(emsp1_accessing_cpo1__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(cpo1_accessing_emsp1__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_3_0.Version.Id ],
                          SelectedVersionId:           OCPIv2_3_0.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            #endregion

            #region EMSP #2 -> CPO #1

            Assert.That(
                await cpo1CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                          CountryCode:                 emsp2CommonAPI_v2_1_1.OurCountryCode,
                          PartyId:                     emsp2CommonAPI_v2_1_1.OurPartyId,
                          Role:                        Role.EMSP,
                          BusinessDetails:             emsp2CommonAPI_v2_1_1.OurBusinessDetails,

                          AccessToken:                 AccessToken.Parse(emsp2_accessing_cpo1__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(cpo1_accessing_emsp2__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp2HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_1_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_1_1.Version.Id,
                          AccessTokenBase64Encoding:   false,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await cpo1CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           emsp2CommonAPI_v2_2_1.DefaultPartyId,
                                                           Role.EMSP
                                                       ),
                          CredentialsRoles:            [
                                                           emsp2CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                       ],

                          AccessToken:                 AccessToken.Parse(emsp2_accessing_cpo1__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(cpo1_accessing_emsp2__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp2HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_2_1.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await cpo1CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           emsp2CommonAPI_v2_3_0.DefaultPartyId,
                                                           Role.EMSP
                                                       ),
                          CredentialsRoles:            [
                                                           emsp2CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                       ],

                          AccessToken:                 AccessToken.Parse(emsp2_accessing_cpo1__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(cpo1_accessing_emsp2__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp2HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_3_0.Version.Id ],
                          SelectedVersionId:           OCPIv2_3_0.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            #endregion


            #region EMSP #1 -> CPO #2

            Assert.That(
                await cpo2CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                          CountryCode:                 emsp1CommonAPI_v2_1_1.OurCountryCode,
                          PartyId:                     emsp1CommonAPI_v2_1_1.OurPartyId,
                          Role:                        Role.EMSP,
                          BusinessDetails:             emsp1CommonAPI_v2_1_1.OurBusinessDetails,

                          AccessToken:                 AccessToken.Parse(emsp1_accessing_cpo2__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(cpo2_accessing_emsp1__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_1_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_1_1.Version.Id,
                          AccessTokenBase64Encoding:   false,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await cpo2CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           emsp1CommonAPI_v2_2_1.DefaultPartyId,
                                                           Role.EMSP
                                                       ),
                          CredentialsRoles:            [
                                                           emsp1CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                       ],

                          AccessToken:                 AccessToken.Parse(emsp1_accessing_cpo2__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(cpo2_accessing_emsp1__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_2_1.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await cpo2CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           emsp1CommonAPI_v2_3_0.DefaultPartyId,
                                                           Role.EMSP
                                                       ),
                          CredentialsRoles:            [
                                                           emsp1CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                       ],

                          AccessToken:                 AccessToken.Parse(emsp1_accessing_cpo2__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(cpo2_accessing_emsp1__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_3_0.Version.Id ],
                          SelectedVersionId:           OCPIv2_3_0.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            #endregion

            #region EMSP #2 -> CPO #2

            Assert.That(
                await cpo2CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                          CountryCode:                 emsp2CommonAPI_v2_1_1.OurCountryCode,
                          PartyId:                     emsp2CommonAPI_v2_1_1.OurPartyId,
                          Role:                        Role.EMSP,
                          BusinessDetails:             emsp2CommonAPI_v2_1_1.OurBusinessDetails,

                          AccessToken:                 AccessToken.Parse(emsp2_accessing_cpo2__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(cpo2_accessing_emsp2__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp2HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_1_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_1_1.Version.Id,
                          AccessTokenBase64Encoding:   false,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await cpo2CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           emsp2CommonAPI_v2_2_1.DefaultPartyId,
                                                           Role.EMSP
                                                       ),
                          CredentialsRoles:            [
                                                           emsp2CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                       ],

                          AccessToken:                 AccessToken.Parse(emsp2_accessing_cpo2__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(cpo2_accessing_emsp2__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp2HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_2_1.Version.Id ],
                          SelectedVersionId:           OCPIv2_2_1.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            Assert.That(
                await cpo2CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                          Id:                          RemoteParty_Id.Parse(
                                                           emsp2CommonAPI_v2_3_0.DefaultPartyId,
                                                           Role.EMSP
                                                       ),
                          CredentialsRoles:            [
                                                           emsp2CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                       ],

                          AccessToken:                 AccessToken.Parse(emsp2_accessing_cpo2__token),
                          AccessStatus:                AccessStatus.ALLOWED,
                          LocalTOTP_ValidityTime:      TOTPValidityTime,

                          RemoteAccessToken:           AccessToken.Parse(cpo2_accessing_emsp2__token),
                          RemoteTOTP_ValidityTime:     TOTPValidityTime,
                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp2HTTPServer.TCPPort}/ocpi/versions"),
                          RemoteVersionIds:            [ OCPIv2_3_0.Version.Id ],
                          SelectedVersionId:           OCPIv2_3_0.Version.Id,
                          AccessTokenBase64Encoding:   true,
                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                          PartyStatus:                 PartyStatus.ENABLED
                      ),
                Is.True
            );

            #endregion


            Assert.That(cpo1CommonAPI_v2_1_1. RemoteParties.Count(),  Is.EqualTo(2));
            Assert.That(cpo1CommonAPI_v2_2_1. RemoteParties.Count(),  Is.EqualTo(2));
            Assert.That(cpo1CommonAPI_v2_3_0. RemoteParties.Count(),  Is.EqualTo(2));

            Assert.That(cpo2CommonAPI_v2_1_1. RemoteParties.Count(),  Is.EqualTo(2));
            Assert.That(cpo2CommonAPI_v2_2_1. RemoteParties.Count(),  Is.EqualTo(2));
            Assert.That(cpo2CommonAPI_v2_3_0. RemoteParties.Count(),  Is.EqualTo(2));

            Assert.That(emsp1CommonAPI_v2_1_1.RemoteParties.Count(),  Is.EqualTo(2));
            Assert.That(emsp1CommonAPI_v2_2_1.RemoteParties.Count(),  Is.EqualTo(2));
            Assert.That(emsp1CommonAPI_v2_3_0.RemoteParties.Count(),  Is.EqualTo(2));

            Assert.That(emsp2CommonAPI_v2_1_1.RemoteParties.Count(),  Is.EqualTo(2));
            Assert.That(emsp2CommonAPI_v2_2_1.RemoteParties.Count(),  Is.EqualTo(2));
            Assert.That(emsp2CommonAPI_v2_3_0.RemoteParties.Count(),  Is.EqualTo(2));


            // Check the RemoteParty database files...
            Assert.That(await checkFile(cpo1CommonAPI_v2_1_1. RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
            Assert.That(await checkFile(cpo1CommonAPI_v2_1_1. RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
            Assert.That(await checkFile(cpo1CommonAPI_v2_1_1. RemotePartyDBFileName, lines => lines.Length == 2), Is.True);

            Assert.That(await checkFile(cpo2CommonAPI_v2_1_1. RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
            Assert.That(await checkFile(cpo2CommonAPI_v2_1_1. RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
            Assert.That(await checkFile(cpo2CommonAPI_v2_1_1. RemotePartyDBFileName, lines => lines.Length == 2), Is.True);

            Assert.That(await checkFile(emsp1CommonAPI_v2_1_1.RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
            Assert.That(await checkFile(emsp1CommonAPI_v2_1_1.RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
            Assert.That(await checkFile(emsp1CommonAPI_v2_1_1.RemotePartyDBFileName, lines => lines.Length == 2), Is.True);

            Assert.That(await checkFile(emsp2CommonAPI_v2_1_1.RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
            Assert.That(await checkFile(emsp2CommonAPI_v2_1_1.RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
            Assert.That(await checkFile(emsp2CommonAPI_v2_1_1.RemotePartyDBFileName, lines => lines.Length == 2), Is.True);

            #endregion

            #region Define blocked Access Tokens and blocked Remote Parties

            #region Blocked Tokens

            await cpo1CommonAPI_v2_1_1.AddAccessToken(
                      AccessToken.Parse(BlockedToken),
                      AccessStatus.BLOCKED
                  );

            await cpo2CommonAPI_v2_1_1.AddAccessToken(
                      AccessToken.Parse(BlockedToken),
                      AccessStatus.BLOCKED
                  );

            await emsp1CommonAPI_v2_1_1.AddAccessToken(
                      AccessToken.Parse(BlockedToken),
                      AccessStatus.BLOCKED
                  );

            await emsp2CommonAPI_v2_1_1.AddAccessToken(
                      AccessToken.Parse(BlockedToken),
                      AccessStatus.BLOCKED
                  );

            #endregion


            await cpo1CommonAPI_v2_1_1.AddRemoteParty(
                      CountryCode:       CountryCode.Parse("XX"),
                      PartyId:           Party_Id.   Parse("BLE"),
                      Role:              Role.EMSP,
                      BusinessDetails:   new BusinessDetails(
                                             "Blocked EMSP"
                                         ),
                      AccessToken:       AccessToken.Parse(BlockedEMSPToken),
                      AccessStatus:      AccessStatus.BLOCKED,
                      PartyStatus:       PartyStatus. ENABLED
                  );


            await emsp1CommonAPI_v2_1_1.AddRemoteParty(
                      CountryCode:       CountryCode.Parse("XX"),
                      PartyId:           Party_Id.   Parse("BLC"),
                      Role:              Role.EMSP,
                      BusinessDetails:   new BusinessDetails(
                                             "Blocked CPO"
                                         ),
                      AccessToken:       AccessToken.Parse(BlockedCPOToken),
                      AccessStatus:      AccessStatus.BLOCKED,
                      PartyStatus:       PartyStatus. ENABLED
                  );



            //ClassicAssert.AreEqual(3, cpoCommonAPI.  RemoteParties.Count());
            //ClassicAssert.AreEqual(2, emsp1CommonAPI.RemoteParties.Count());
            //ClassicAssert.AreEqual(2, emsp2CommonAPI.RemoteParties.Count());

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

            if (cpo1HTTPServer is not null)
                await cpo1HTTPServer. Stop();

            if (cpo2HTTPServer is not null)
                await cpo2HTTPServer. Stop();

            if (emsp1HTTPServer is not null)
                await emsp1HTTPServer.Stop();

            if (emsp2HTTPServer is not null)
                await emsp2HTTPServer.Stop();


            // CPO #1
            if (cpo1CommonAPI_v2_1_1 is not null)
                File.Delete(cpo1CommonAPI_v2_1_1.RemotePartyDBFileName);

            if (cpo1CommonAPI_v2_2_1 is not null)
                File.Delete(cpo1CommonAPI_v2_2_1.RemotePartyDBFileName);

            if (cpo1CommonAPI_v2_3_0 is not null)
                File.Delete(cpo1CommonAPI_v2_3_0.RemotePartyDBFileName);

            if (cpo1CommonAPI_v3_0   is not null)
                File.Delete(cpo1CommonAPI_v3_0.  RemotePartyDBFileName);


            // CPO #2
            if (cpo2CommonAPI_v2_1_1 is not null)
                File.Delete(cpo2CommonAPI_v2_1_1.RemotePartyDBFileName);

            if (cpo2CommonAPI_v2_2_1 is not null)
                File.Delete(cpo2CommonAPI_v2_2_1.RemotePartyDBFileName);

            if (cpo2CommonAPI_v2_3_0 is not null)
                File.Delete(cpo2CommonAPI_v2_3_0.RemotePartyDBFileName);

            if (cpo2CommonAPI_v3_0   is not null)
                File.Delete(cpo2CommonAPI_v3_0.  RemotePartyDBFileName);


            // EMSP #1
            if (emsp1CommonAPI_v2_1_1 is not null)
                File.Delete(emsp1CommonAPI_v2_1_1.RemotePartyDBFileName);

            if (emsp1CommonAPI_v2_2_1 is not null)
                File.Delete(emsp1CommonAPI_v2_2_1.RemotePartyDBFileName);

            if (emsp1CommonAPI_v2_3_0 is not null)
                File.Delete(emsp1CommonAPI_v2_3_0.RemotePartyDBFileName);

            if (emsp1CommonAPI_v3_0   is not null)
                File.Delete(emsp1CommonAPI_v3_0.  RemotePartyDBFileName);


            // EMSP #2
            if (emsp2CommonAPI_v2_1_1 is not null)
                File.Delete(emsp2CommonAPI_v2_1_1.RemotePartyDBFileName);

            if (emsp2CommonAPI_v2_2_1 is not null)
                File.Delete(emsp2CommonAPI_v2_2_1.RemotePartyDBFileName);

            if (emsp2CommonAPI_v2_3_0 is not null)
                File.Delete(emsp2CommonAPI_v2_3_0.RemotePartyDBFileName);

            if (emsp2CommonAPI_v3_0   is not null)
                File.Delete(emsp2CommonAPI_v3_0.  RemotePartyDBFileName);

        }

        #endregion


    }

}
