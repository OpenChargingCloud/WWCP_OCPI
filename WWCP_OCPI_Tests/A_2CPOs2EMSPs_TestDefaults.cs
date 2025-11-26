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
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Crypto;

using BCx509 = Org.BouncyCastle.X509;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.PKI;
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
    public abstract class A_2CPOs2EMSPs_TestDefaults(

        // This will enable additional TOTP authentication!
        TimeSpan?  TOTPValidityTime        = null,

        // One of those will enable client certificates for TLS authentication!
        UInt16?    RSASize                 = null,
        String?    ECCAlgorithm            = null,
        String?    MLDSAAlgorithm          = null,

        Boolean?   AutoWireRemoteParties   = true)

    {

        #region Data

        private   readonly DNSClient    DNSClient                             = new (SearchForIPv6DNSServers: false);

        private   readonly TimeSpan?    TOTPValidityTime                      = TOTPValidityTime;

        protected const    String       UnknownToken                          = "UnknownUnknownUnknownToken";

        protected readonly AccessToken  BlockedToken                          = AccessToken.Parse("blocked-token");
        protected readonly AccessToken  BlockedCPOToken                       = AccessToken.Parse("blocked-cpo");
        protected readonly AccessToken  BlockedEMSPToken                      = AccessToken.Parse("blocked-emsp");

        private   readonly Boolean      AutoWireRemoteParties                 = AutoWireRemoteParties ?? true;


        #region PKI

        protected          OCPI_PKI?                  PKI;

        private   readonly UInt16?                    RSASize                 = RSASize;
        private   readonly String?                    ECCAlgorithm            = ECCAlgorithm;
        private   readonly String?                    MLDSAAlgorithm          = MLDSAAlgorithm;

        protected          AsymmetricCipherKeyPair?   rootCAKeyPair;
        protected          BCx509.X509Certificate?    rootCACertificate;
        protected          X509Certificate2?          rootCACertificate2;

        protected          AsymmetricCipherKeyPair?   serverCAKeyPair;
        protected          BCx509.X509Certificate?    serverCACertificate;
        protected          X509Certificate2?          serverCACertificate2;

        protected          AsymmetricCipherKeyPair?   clientCAKeyPair;
        protected          BCx509.X509Certificate?    clientCACertificate;
        protected          X509Certificate2?          clientCACertificate2;

        #endregion


        #region CPO #1

        public          HTTPTestServerX?                                               cpo1HTTPServer;
        protected       HTTPExtAPIX?                                                   cpo1HTTPAPI;
        protected       AsymmetricCipherKeyPair?                                       cpo1TLSServerKeyPair;
        protected       AsymmetricCipherKeyPair?                                       cpo1emsp1TLSClientKeyPair;
        protected       AsymmetricCipherKeyPair?                                       cpo1emsp2TLSClientKeyPair;
        protected       Pkcs10CertificationRequest?                                    cpo1TLSServerCSR;
        protected       Pkcs10CertificationRequest?                                    cpo1emsp1TLSClientCSR;
        protected       Pkcs10CertificationRequest?                                    cpo1emsp2TLSClientCSR;
        protected       X509Certificate2?                                              cpo1TLSServerCertificate;
        protected       X509Certificate2?                                              cpo1emsp1TLSClientCertificate;
        protected       X509Certificate2?                                              cpo1emsp2TLSClientCertificate;

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
        protected       AsymmetricCipherKeyPair?                                       cpo2TLSServerKeyPair;
        protected       AsymmetricCipherKeyPair?                                       cpo2emsp1TLSClientKeyPair;
        protected       AsymmetricCipherKeyPair?                                       cpo2emsp2TLSClientKeyPair;
        protected       Pkcs10CertificationRequest?                                    cpo2TLSServerCSR;
        protected       Pkcs10CertificationRequest?                                    cpo2emsp1TLSClientCSR;
        protected       Pkcs10CertificationRequest?                                    cpo2emsp2TLSClientCSR;
        protected       X509Certificate2?                                              cpo2TLSServerCertificate;
        protected       X509Certificate2?                                              cpo2emsp1TLSClientCertificate;
        protected       X509Certificate2?                                              cpo2emsp2TLSClientCertificate;

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
        protected       AsymmetricCipherKeyPair?                                       emsp1TLSServerKeyPair;
        protected       AsymmetricCipherKeyPair?                                       emsp1cpo1TLSClientKeyPair;
        protected       AsymmetricCipherKeyPair?                                       emsp1cpo2TLSClientKeyPair;
        protected       Pkcs10CertificationRequest?                                    emsp1TLSServerCSR;
        protected       Pkcs10CertificationRequest?                                    emsp1cpo1TLSClientCSR;
        protected       Pkcs10CertificationRequest?                                    emsp1cpo2TLSClientCSR;
        protected       X509Certificate2?                                              emsp1TLSServerCertificate;
        protected       X509Certificate2?                                              emsp1cpo1TLSClientCertificate;
        protected       X509Certificate2?                                              emsp1cpo2TLSClientCertificate;

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
        protected       AsymmetricCipherKeyPair?                                       emsp2TLSServerKeyPair;
        protected       AsymmetricCipherKeyPair?                                       emsp2cpo1TLSClientKeyPair;
        protected       AsymmetricCipherKeyPair?                                       emsp2cpo2TLSClientKeyPair;
        protected       Pkcs10CertificationRequest?                                    emsp2TLSServerCSR;
        protected       Pkcs10CertificationRequest?                                    emsp2emsp1TLSClientCSR;
        protected       Pkcs10CertificationRequest?                                    emsp2emsp2TLSClientCSR;
        protected       X509Certificate2?                                              emsp2TLSServerCertificate;
        protected       X509Certificate2?                                              emsp2cpo1TLSClientCertificate;
        protected       X509Certificate2?                                              emsp2cpo2TLSClientCertificate;

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


        #region (private) CheckFile(FileName, Checker)

        /// <summary>
        /// Most (log) files are written async in the background, thus test might run faster,
        /// than those background processes really write the files. When we want to verify,
        /// that something was written to a file, we might have to retry a few times and
        /// meanwhile wait a little bit.
        /// </summary>
        /// <param name="FileName">The file to be checked.</param>
        /// <param name="Checker">A delegate to check the file contents.</param>
        private static async Task<Boolean> CheckFile(String                   FileName,
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

            #region Generate PKI

            if (RSASize        is not null ||
                ECCAlgorithm   is not null ||
                MLDSAAlgorithm is not null)
            {

                PKI = new OCPI_PKI();

                #region Generate RootCA

                if      (RSASize        is not null)
                    rootCAKeyPair    = PKI.GenerateRSAKeyPair  (RSASize);

                else if (ECCAlgorithm   is not null)
                    rootCAKeyPair    = PKI.GenerateECCKeyPair  (ECCAlgorithm);

                else if (MLDSAAlgorithm is not null)
                    rootCAKeyPair    = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);


                rootCACertificate    = rootCAKeyPair is not null
                                           ? PKIFactory.CreateRootCACertificate(
                                                 "OCPI Test RootCA",
                                                 rootCAKeyPair,
                                                 TimeSpan.FromDays(7)
                                             )
                                           : null;

                rootCACertificate2   = rootCACertificate?.ToDotNet2(rootCAKeyPair?.Private);

                #endregion

                #region Generate ServerCA

                if      (RSASize        is not null)
                    serverCAKeyPair   = PKI.GenerateRSAKeyPair  (RSASize);

                else if (ECCAlgorithm   is not null)
                    serverCAKeyPair   = PKI.GenerateECCKeyPair  (ECCAlgorithm);

                else if (MLDSAAlgorithm is not null)
                    serverCAKeyPair   = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);


                serverCACertificate   = rootCAKeyPair is not null && rootCACertificate is not null && serverCAKeyPair is not null
                                          ? PKIFactory.CreateIntermediateCA(
                                                "OCPI Test ServerCA",
                                                serverCAKeyPair.Public,
                                                rootCAKeyPair.  Private,
                                                rootCACertificate,
                                                TimeSpan.FromDays(7)
                                            )
                                          : null;

                serverCACertificate2  = rootCACertificate?.ToDotNet2(rootCAKeyPair?.Private);

                #endregion

                #region Generate ClientCA

                if      (RSASize        is not null)
                    clientCAKeyPair   = PKI.GenerateRSAKeyPair  (RSASize);

                else if (ECCAlgorithm   is not null)
                    clientCAKeyPair   = PKI.GenerateECCKeyPair  (ECCAlgorithm);

                else if (MLDSAAlgorithm is not null)
                    clientCAKeyPair   = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);


                clientCACertificate   = rootCAKeyPair is not null && rootCACertificate is not null && clientCAKeyPair is not null
                                          ? PKIFactory.CreateIntermediateCA(
                                                "OCPI Test ClientCA",
                                                clientCAKeyPair.Public,
                                                rootCAKeyPair.  Private,
                                                rootCACertificate,
                                                TimeSpan.FromDays(7)
                                            )
                                          : null;

                clientCACertificate2  = rootCACertificate?.ToDotNet2(rootCAKeyPair?.Private);

                #endregion


                #region Generate CPO and EMSP TLS KeyPairs

                #region CPO #1

                if      (RSASize        is not null)
                {
                    cpo1TLSServerKeyPair       = PKI.GenerateRSAKeyPair(RSASize);
                    cpo1emsp1TLSClientKeyPair  = PKI.GenerateRSAKeyPair(RSASize);
                    cpo1emsp2TLSClientKeyPair  = PKI.GenerateRSAKeyPair(RSASize);
                }

                else if (ECCAlgorithm   is not null)
                {
                    cpo1TLSServerKeyPair       = PKI.GenerateECCKeyPair(ECCAlgorithm);
                    cpo1emsp1TLSClientKeyPair  = PKI.GenerateECCKeyPair(ECCAlgorithm);
                    cpo1emsp2TLSClientKeyPair  = PKI.GenerateECCKeyPair(ECCAlgorithm);
                }

                else if (MLDSAAlgorithm is not null)
                {
                    cpo1TLSServerKeyPair       = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);
                    cpo1emsp1TLSClientKeyPair  = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);
                    cpo1emsp2TLSClientKeyPair  = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);
                }

                #endregion

                #region CPO #2

                if      (RSASize        is not null)
                {
                    cpo2TLSServerKeyPair       = PKI.GenerateRSAKeyPair(RSASize);
                    cpo2emsp1TLSClientKeyPair  = PKI.GenerateRSAKeyPair(RSASize);
                    cpo2emsp2TLSClientKeyPair  = PKI.GenerateRSAKeyPair(RSASize);
                }

                else if (ECCAlgorithm   is not null)
                {
                    cpo2TLSServerKeyPair       = PKI.GenerateECCKeyPair(ECCAlgorithm);
                    cpo2emsp1TLSClientKeyPair  = PKI.GenerateECCKeyPair(ECCAlgorithm);
                    cpo2emsp2TLSClientKeyPair  = PKI.GenerateECCKeyPair(ECCAlgorithm);
                }

                else if (MLDSAAlgorithm is not null)
                {
                    cpo2TLSServerKeyPair       = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);
                    cpo2emsp1TLSClientKeyPair  = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);
                    cpo2emsp2TLSClientKeyPair  = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);
                }

                #endregion

                #region EMSP #1

                if (RSASize is not null)
                {
                    emsp1TLSServerKeyPair      = PKI.GenerateRSAKeyPair(RSASize);
                    emsp1cpo1TLSClientKeyPair  = PKI.GenerateRSAKeyPair(RSASize);
                    emsp1cpo2TLSClientKeyPair  = PKI.GenerateRSAKeyPair(RSASize);
                }

                else if (ECCAlgorithm is not null)
                {
                    emsp1TLSServerKeyPair      = PKI.GenerateECCKeyPair(ECCAlgorithm);
                    emsp1cpo1TLSClientKeyPair  = PKI.GenerateECCKeyPair(ECCAlgorithm);
                    emsp1cpo2TLSClientKeyPair  = PKI.GenerateECCKeyPair(ECCAlgorithm);
                }

                else if (MLDSAAlgorithm is not null)
                {
                    emsp1TLSServerKeyPair      = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);
                    emsp1cpo1TLSClientKeyPair  = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);
                    emsp1cpo2TLSClientKeyPair  = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);
                }

                #endregion

                #region EMSP #2

                if (RSASize is not null)
                {
                    emsp2TLSServerKeyPair      = PKI.GenerateRSAKeyPair(RSASize);
                    emsp2cpo1TLSClientKeyPair  = PKI.GenerateRSAKeyPair(RSASize);
                    emsp2cpo2TLSClientKeyPair  = PKI.GenerateRSAKeyPair(RSASize);
                }

                else if (ECCAlgorithm is not null)
                {
                    emsp2TLSServerKeyPair      = PKI.GenerateECCKeyPair(ECCAlgorithm);
                    emsp2cpo1TLSClientKeyPair  = PKI.GenerateECCKeyPair(ECCAlgorithm);
                    emsp2cpo2TLSClientKeyPair  = PKI.GenerateECCKeyPair(ECCAlgorithm);
                }

                else if (MLDSAAlgorithm is not null)
                {
                    emsp2TLSServerKeyPair      = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);
                    emsp2cpo1TLSClientKeyPair  = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);
                    emsp2cpo2TLSClientKeyPair  = PKI.GenerateMLDSAKeyPair(MLDSAAlgorithm);
                }

                #endregion

                #endregion

                #region Generate CPO and EMSP TLS Certificate Signing Requests

                #region CPO #1

                cpo1TLSServerCSR       = cpo1TLSServerKeyPair is not null
                                             ? PKI.GenerateServerCSR(
                                                   PublicKey:            cpo1TLSServerKeyPair.Public,
                                                   KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                   PartyIds:             [ "DEGEF" ],
                                                   SubCPOIds:            [ "DE*GEF" ],
                                                   NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                   NotAfter:             Timestamp.Now + TimeSpan.FromDays(2),
                                                   CommonName:          $"GraphDefined OCPI CPO #1",
                                                   Organization:         "GraphDefined GmbH",
                                                   OrganizationalUnit:   "CPO Services",
                                                   EMailAddress:         "roaming-cpo@charging.cloud",
                                                   TelephoneNumber:      "+49 1234 56789012",
                                                   PostalCode:           "07749",
                                                   Locality:             "Jena",
                                                   Country:              "DE",
                                                   Description:         $"Main CPO #1 OCPI CSR..."
                                               ).ToCSR(cpo1TLSServerKeyPair)
                                             : null;

                // Towards EMSP #1 Client CSR
                cpo1emsp1TLSClientCSR  = cpo1emsp1TLSClientKeyPair is not null
                                             ? PKI.GenerateClientCSR(
                                                   PublicKey:            cpo1emsp1TLSClientKeyPair.Public,
                                                   KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                   PartyIds:             [ "DEGEF" ],
                                                   SubCPOIds:            [ "DE*GEF" ],
                                                   NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                   NotAfter:             Timestamp.Now + TimeSpan.FromDays(2),
                                                   CommonName:          $"GraphDefined OCPI CPO #1 to EMSP #1",
                                                   Organization:         "GraphDefined GmbH",
                                                   OrganizationalUnit:   "CPO Services",
                                                   EMailAddress:         "roaming-cpo@charging.cloud",
                                                   TelephoneNumber:      "+49 1234 56789012",
                                                   PostalCode:           "07749",
                                                   Locality:             "Jena",
                                                   Country:              "DE",
                                                   Description:         $"Main CPO #1 to EMSP #1 OCPI CSR..."
                                               ).ToCSR(cpo1emsp1TLSClientKeyPair)
                                             : null;

                // Towards EMSP #2 Client CSR
                cpo1emsp2TLSClientCSR  = cpo1emsp2TLSClientKeyPair is not null
                                             ? PKI.GenerateClientCSR(
                                                   PublicKey:            cpo1emsp2TLSClientKeyPair.Public,
                                                   KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                   PartyIds:             [ "DEGEF" ],
                                                   SubCPOIds:            [ "DE*GEF" ],
                                                   NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                   NotAfter:             Timestamp.Now + TimeSpan.FromDays(2),
                                                   CommonName:          $"GraphDefined OCPI CPO #1 to EMSP #2",
                                                   Organization:         "GraphDefined GmbH",
                                                   OrganizationalUnit:   "CPO Services",
                                                   EMailAddress:         "roaming-cpo@charging.cloud",
                                                   TelephoneNumber:      "+49 1234 56789012",
                                                   PostalCode:           "07749",
                                                   Locality:             "Jena",
                                                   Country:              "DE",
                                                   Description:         $"Main CPO #1 to EMSP #2 OCPI CSR..."
                                               ).ToCSR(cpo1emsp2TLSClientKeyPair)
                                             : null;

                #endregion

                #region CPO #2

                cpo2TLSServerCSR       = cpo2TLSServerKeyPair is not null
                                             ? PKI.GenerateServerCSR(
                                                   PublicKey:            cpo2TLSServerKeyPair.Public,
                                                   KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                   PartyIds:             [ "DEGEF" ],
                                                   SubCPOIds:            [ "DE*GEF" ],
                                                   NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                   NotAfter:             Timestamp.Now + TimeSpan.FromDays(2),
                                                   CommonName:          $"GraphDefined OCPI CPO #2",
                                                   Organization:         "GraphDefined GmbH",
                                                   OrganizationalUnit:   "CPO Services",
                                                   EMailAddress:         "roaming-cpo@charging.cloud",
                                                   TelephoneNumber:      "+49 1234 56789012",
                                                   PostalCode:           "07749",
                                                   Locality:             "Jena",
                                                   Country:              "DE",
                                                   Description:         $"Main CPO #2 OCPI CSR..."
                                               ).ToCSR(cpo2TLSServerKeyPair)
                                             : null;

                // Towards EMSP #2 Client CSR
                cpo2emsp1TLSClientCSR  = cpo2emsp1TLSClientKeyPair is not null
                                             ? PKI.GenerateClientCSR(
                                                   PublicKey:            cpo2emsp1TLSClientKeyPair.Public,
                                                   KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                   PartyIds:             [ "DEGEF" ],
                                                   SubCPOIds:            [ "DE*GEF" ],
                                                   NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                   NotAfter:             Timestamp.Now + TimeSpan.FromDays(2),
                                                   CommonName:          $"GraphDefined OCPI CPO #2 to EMSP #2",
                                                   Organization:         "GraphDefined GmbH",
                                                   OrganizationalUnit:   "CPO Services",
                                                   EMailAddress:         "roaming-cpo@charging.cloud",
                                                   TelephoneNumber:      "+49 1234 56789012",
                                                   PostalCode:           "07749",
                                                   Locality:             "Jena",
                                                   Country:              "DE",
                                                   Description:         $"Main CPO #2 to EMSP #2 OCPI CSR..."
                                               ).ToCSR(cpo2emsp1TLSClientKeyPair)
                                             : null;

                // Towards EMSP #2 Client CSR
                cpo2emsp2TLSClientCSR  = cpo2emsp2TLSClientKeyPair is not null
                                             ? PKI.GenerateClientCSR(
                                                   PublicKey:            cpo2emsp2TLSClientKeyPair.Public,
                                                   KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                   PartyIds:             [ "DEGEF" ],
                                                   SubCPOIds:            [ "DE*GEF" ],
                                                   NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                   NotAfter:             Timestamp.Now + TimeSpan.FromDays(2),
                                                   CommonName:          $"GraphDefined OCPI CPO #2 to EMSP #2",
                                                   Organization:         "GraphDefined GmbH",
                                                   OrganizationalUnit:   "CPO Services",
                                                   EMailAddress:         "roaming-cpo@charging.cloud",
                                                   TelephoneNumber:      "+49 1234 56789012",
                                                   PostalCode:           "07749",
                                                   Locality:             "Jena",
                                                   Country:              "DE",
                                                   Description:         $"Main CPO #2 to EMSP #2 OCPI CSR..."
                                               ).ToCSR(cpo2emsp2TLSClientKeyPair)
                                             : null;

                #endregion

                #region EMSP #1

                emsp1TLSServerCSR      = emsp1TLSServerKeyPair is not null
                                                       ? PKI.GenerateServerCSR(
                                                             PublicKey:            emsp1TLSServerKeyPair.Public,
                                                             KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                             PartyIds:             [ "DEGDF" ],
                                                             SubEMSPIds:           [ "DE*GDF" ],
                                                             NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                             NotAfter:             Timestamp.Now + TimeSpan.FromDays(2),
                                                             CommonName:          $"GraphDefined OCPI EMSP #1",
                                                             Organization:         "GraphDefined GmbH",
                                                             OrganizationalUnit:   "EMSP Services",
                                                             EMailAddress:         "roaming-emsp@charging.cloud",
                                                             TelephoneNumber:      "+49 1234 56789012",
                                                             PostalCode:           "07749",
                                                             Locality:             "Jena",
                                                             Country:              "DE",
                                                             Description:         $"Main EMSP #1 OCPI CSR..."
                                                         ).ToCSR(emsp1TLSServerKeyPair)
                                                       : null;

                // Towards CPO #1 Client CSRs
                emsp1cpo1TLSClientCSR  = emsp1cpo1TLSClientKeyPair is not null
                                                        ? PKI.GenerateClientCSR(
                                                              PublicKey:            emsp1cpo1TLSClientKeyPair.Public,
                                                              KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                              PartyIds:             [ "DEGEF" ],
                                                              SubCPOIds:            [ "DE*GEF" ],
                                                              NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                              NotAfter:             Timestamp.Now + TimeSpan.FromDays(2),
                                                              CommonName:          $"GraphDefined OCPI {OCPIv2_1_1.Version.String} EMSP #1 to CPO #1",
                                                              Organization:         "GraphDefined GmbH",
                                                              OrganizationalUnit:   "EMSP Services",
                                                              EMailAddress:         "roaming-emsp@charging.cloud",
                                                              TelephoneNumber:      "+49 1234 56789012",
                                                              PostalCode:           "07749",
                                                              Locality:             "Jena",
                                                              Country:              "DE",
                                                              Description:         $"Main EMSP #1 to CPO #1 OCPI {OCPIv2_1_1.Version.String} CSR..."
                                                          ).ToCSR(emsp1cpo1TLSClientKeyPair)
                                                        : null;

                // Towards CPO #2 Client CSRs
                emsp1cpo2TLSClientCSR  = emsp1cpo2TLSClientKeyPair is not null
                                                        ? PKI.GenerateClientCSR(
                                                              PublicKey:            emsp1cpo2TLSClientKeyPair.Public,
                                                              KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                              PartyIds:             [ "DEGEF" ],
                                                              SubCPOIds:            [ "DE*GEF" ],
                                                              NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                              NotAfter:             Timestamp.Now + TimeSpan.FromDays(2),
                                                              CommonName:          $"GraphDefined OCPI {OCPIv2_1_1.Version.String} EMSP #1 to CPO #2",
                                                              Organization:         "GraphDefined GmbH",
                                                              OrganizationalUnit:   "EMSP Services",
                                                              EMailAddress:         "roaming-emsp@charging.cloud",
                                                              TelephoneNumber:      "+49 1234 56789012",
                                                              PostalCode:           "07749",
                                                              Locality:             "Jena",
                                                              Country:              "DE",
                                                              Description:         $"Main EMSP #1 to CPO #2 OCPI {OCPIv2_1_1.Version.String} CSR..."
                                                          ).ToCSR(emsp1cpo2TLSClientKeyPair)
                                                        : null;

                #endregion

                #region EMSP #2

                emsp1TLSServerCSR      = emsp1TLSServerKeyPair is not null
                                                       ? PKI.GenerateServerCSR(
                                                             PublicKey:            emsp1TLSServerKeyPair.Public,
                                                             KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                             PartyIds:             [ "DEGDF" ],
                                                             SubEMSPIds:           [ "DE*GDF" ],
                                                             NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                             NotAfter:             Timestamp.Now + TimeSpan.FromDays(2),
                                                             CommonName:          $"GraphDefined OCPI EMSP #2",
                                                             Organization:         "GraphDefined GmbH",
                                                             OrganizationalUnit:   "EMSP Services",
                                                             EMailAddress:         "roaming-emsp@charging.cloud",
                                                             TelephoneNumber:      "+49 1234 56789012",
                                                             PostalCode:           "07749",
                                                             Locality:             "Jena",
                                                             Country:              "DE",
                                                             Description:         $"Main EMSP #2 OCPI CSR..."
                                                         ).ToCSR(emsp1TLSServerKeyPair)
                                                       : null;

                // Towards CPO #2 Client CSRs
                emsp1cpo2TLSClientCSR  = emsp1cpo2TLSClientKeyPair is not null
                                                        ? PKI.GenerateClientCSR(
                                                              PublicKey:            emsp1cpo2TLSClientKeyPair.Public,
                                                              KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                              PartyIds:             [ "DEGEF" ],
                                                              SubCPOIds:            [ "DE*GEF" ],
                                                              NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                              NotAfter:             Timestamp.Now + TimeSpan.FromDays(2),
                                                              CommonName:          $"GraphDefined OCPI {OCPIv2_1_1.Version.String} EMSP #2 to CPO #2",
                                                              Organization:         "GraphDefined GmbH",
                                                              OrganizationalUnit:   "EMSP Services",
                                                              EMailAddress:         "roaming-emsp@charging.cloud",
                                                              TelephoneNumber:      "+49 1234 56789012",
                                                              PostalCode:           "07749",
                                                              Locality:             "Jena",
                                                              Country:              "DE",
                                                              Description:         $"Main EMSP #2 to CPO #2 OCPI {OCPIv2_1_1.Version.String} CSR..."
                                                          ).ToCSR(emsp1cpo2TLSClientKeyPair)
                                                        : null;

                // Towards CPO #2 Client CSRs
                emsp1cpo2TLSClientCSR  = emsp1cpo2TLSClientKeyPair is not null
                                                        ? PKI.GenerateClientCSR(
                                                              PublicKey:            emsp1cpo2TLSClientKeyPair.Public,
                                                              KeySerialNumber:      UUIDv7.Generate().ToString(),
                                                              PartyIds:             [ "DEGEF" ],
                                                              SubCPOIds:            [ "DE*GEF" ],
                                                              NotBefore:            Timestamp.Now - TimeSpan.FromDays(1),
                                                              NotAfter:             Timestamp.Now + TimeSpan.FromDays(2),
                                                              CommonName:          $"GraphDefined OCPI {OCPIv2_1_1.Version.String} EMSP #2 to CPO #2",
                                                              Organization:         "GraphDefined GmbH",
                                                              OrganizationalUnit:   "EMSP Services",
                                                              EMailAddress:         "roaming-emsp@charging.cloud",
                                                              TelephoneNumber:      "+49 1234 56789012",
                                                              PostalCode:           "07749",
                                                              Locality:             "Jena",
                                                              Country:              "DE",
                                                              Description:         $"Main EMSP #2 to CPO #2 OCPI {OCPIv2_1_1.Version.String} CSR..."
                                                          ).ToCSR(emsp1cpo2TLSClientKeyPair)
                                                        : null;

                #endregion

                #endregion

                #region Generate CPO and EMSP TLS Certificates

                var serverCertificateIssuerName  = serverCACertificate?.SubjectDN;
                var serverCAPrivateKey           = serverCAKeyPair?.    Private;

                var clientCertificateIssuerName  = clientCACertificate?.SubjectDN;
                var clientCAPrivateKey           = clientCAKeyPair?.    Private;

                if (serverCertificateIssuerName is not null &&
                    serverCAPrivateKey          is not null &&
                    clientCertificateIssuerName is not null &&
                    clientCAPrivateKey          is not null)
                {

                    #region CPO #1

                    cpo1TLSServerCertificate       = cpo1TLSServerCSR              is not null &&
                                                     cpo1TLSServerKeyPair?.Private is not null

                                                         ? PKI.SignServerCertificate(
                                                               cpo1TLSServerCSR,
                                                               serverCertificateIssuerName,
                                                               serverCAPrivateKey
                                                           )?.ToDotNet2(cpo1TLSServerKeyPair.Private)

                                                         : null;

                    // Towards EMSP #1 Client Certificates
                    cpo1emsp1TLSClientCertificate  = cpo1emsp1TLSClientCSR              is not null &&
                                                     cpo1emsp1TLSClientKeyPair?.Private is not null

                                                         ? PKI.SignClientCertificate(
                                                               cpo1emsp1TLSClientCSR,
                                                               clientCertificateIssuerName,
                                                               clientCAPrivateKey
                                                           )?.ToDotNet2(cpo1emsp1TLSClientKeyPair.Private)

                                                         : null;

                    // Towards EMSP #2 Client Certificates
                    cpo1emsp2TLSClientCertificate  = cpo1emsp2TLSClientCSR              is not null &&
                                                     cpo1emsp2TLSClientKeyPair?.Private is not null

                                                         ? PKI.SignClientCertificate(
                                                               cpo1emsp2TLSClientCSR,
                                                               clientCertificateIssuerName,
                                                               clientCAPrivateKey
                                                           )?.ToDotNet2(cpo1emsp2TLSClientKeyPair.Private)

                                                         : null;

                    #endregion

                    #region CPO #2

                    cpo2TLSServerCertificate       = cpo2TLSServerCSR              is not null &&
                                                     cpo2TLSServerKeyPair?.Private is not null

                                                         ? PKI.SignServerCertificate(
                                                               cpo2TLSServerCSR,
                                                               serverCertificateIssuerName,
                                                               serverCAPrivateKey
                                                           )?.ToDotNet2(cpo2TLSServerKeyPair.Private)

                                                         : null;

                    // Towards EMSP #2 Client Certificates
                    cpo2emsp1TLSClientCertificate  = cpo2emsp1TLSClientCSR              is not null &&
                                                     cpo2emsp1TLSClientKeyPair?.Private is not null

                                                         ? PKI.SignClientCertificate(
                                                               cpo2emsp1TLSClientCSR,
                                                               clientCertificateIssuerName,
                                                               clientCAPrivateKey
                                                           )?.ToDotNet2(cpo2emsp1TLSClientKeyPair.Private)

                                                         : null;

                    // Towards EMSP #2 Client Certificates
                    cpo2emsp2TLSClientCertificate  = cpo2emsp2TLSClientCSR              is not null &&
                                                     cpo2emsp2TLSClientKeyPair?.Private is not null

                                                         ? PKI.SignClientCertificate(
                                                               cpo2emsp2TLSClientCSR,
                                                               clientCertificateIssuerName,
                                                               clientCAPrivateKey
                                                           )?.ToDotNet2(cpo2emsp2TLSClientKeyPair.Private)

                                                         : null;

                    #endregion

                    #region EMSP #1

                    emsp1TLSServerCertificate      = emsp1TLSServerCSR              is not null &&
                                                     emsp1TLSServerKeyPair?.Private is not null

                                                         ? PKI.SignServerCertificate(
                                                               emsp1TLSServerCSR,
                                                               serverCertificateIssuerName,
                                                               serverCAPrivateKey
                                                           )?.ToDotNet2(emsp1TLSServerKeyPair.Private)

                                                         : null;

                    // Towards EMSP #1 Client Certificates
                    emsp1cpo1TLSClientCertificate  = emsp1cpo1TLSClientCSR              is not null &&
                                                     emsp1cpo1TLSClientKeyPair?.Private is not null

                                                         ? PKI.SignClientCertificate(
                                                               emsp1cpo1TLSClientCSR,
                                                               clientCertificateIssuerName,
                                                               clientCAPrivateKey
                                                           )?.ToDotNet2(emsp1cpo1TLSClientKeyPair.Private)

                                                         : null;

                    // Towards EMSP #2 Client Certificates
                    emsp1cpo2TLSClientCertificate  = emsp1cpo2TLSClientCSR              is not null &&
                                                     emsp1cpo2TLSClientKeyPair?.Private is not null

                                                         ? PKI.SignClientCertificate(
                                                               emsp1cpo2TLSClientCSR,
                                                               clientCertificateIssuerName,
                                                               clientCAPrivateKey
                                                           )?.ToDotNet2(emsp1cpo2TLSClientKeyPair.Private)

                                                         : null;

                    #endregion

                    #region EMSP #2

                    emsp1TLSServerCertificate      = emsp1TLSServerCSR              is not null &&
                                                     emsp1TLSServerKeyPair?.Private is not null

                                                         ? PKI.SignServerCertificate(
                                                               emsp1TLSServerCSR,
                                                               serverCertificateIssuerName,
                                                               serverCAPrivateKey
                                                           )?.ToDotNet2(emsp1TLSServerKeyPair.Private)

                                                         : null;

                    // Towards EMSP #2 Client Certificates
                    emsp1cpo2TLSClientCertificate  = emsp1cpo2TLSClientCSR              is not null &&
                                                     emsp1cpo2TLSClientKeyPair?.Private is not null

                                                         ? PKI.SignClientCertificate(
                                                               emsp1cpo2TLSClientCSR,
                                                               clientCertificateIssuerName,
                                                               clientCAPrivateKey
                                                           )?.ToDotNet2(emsp1cpo2TLSClientKeyPair.Private)

                                                         : null;

                    // Towards EMSP #2 Client Certificates
                    emsp1cpo2TLSClientCertificate  = emsp1cpo2TLSClientCSR              is not null &&
                                                     emsp1cpo2TLSClientKeyPair?.Private is not null

                                                         ? PKI.SignClientCertificate(
                                                               emsp1cpo2TLSClientCSR,
                                                               clientCertificateIssuerName,
                                                               clientCAPrivateKey
                                                           )?.ToDotNet2(emsp1cpo2TLSClientKeyPair.Private)

                                                         : null;

                    #endregion

                }

                #endregion

            }

            #endregion


            #region Create cpo1/cpo2/emsp1/emsp2 HTTP Servers

            cpo1HTTPServer           = new HTTPTestServerX(
                                           TCPPort:                      IPPort.Parse(3301),
                                           DNSClient:                    DNSClient,
                                           ServerCertificateSelector:    cpo1TLSServerCertificate is not null
                                                                             ? (tcpServer, tcpClient) => {
                                                                                   return cpo2TLSServerCertificate!;
                                                                               }
                                                                             : null,
                                           ClientCertificateValidator:   emsp1cpo1TLSClientCertificate is not null &&
                                                                         emsp2cpo1TLSClientCertificate is not null
                                                                             ? (sender,
                                                                               clientCertificate,
                                                                               clientCertificateChain,
                                                                               tlsServer,
                                                                               policyErrors) => {

                                                                                   if (clientCertificate is null)
                                                                                       return (true, [ "The client certificate is null, anyway we proceed... :)" ]);

                                                                                   var chainReport = PKIFactory.ValidateClientChain(
                                                                                                         clientCertificate,
                                                                                                         clientCACertificate2!,
                                                                                                         rootCACertificate2!
                                                                                                     );

                                                                                   return (chainReport.IsValid,
                                                                                           chainReport.Status.Select(chainStatus => chainStatus.Status.ToString()));

                                                                               }
                                                                             : null,
                                           LocalCertificateSelector:     cpo1TLSServerCertificate is not null
                                                                             ? (sender,
                                                                                targetHost,
                                                                                localCertificates,
                                                                                remoteCertificate,
                                                                                acceptableIssuers) => {
                                                                                    return cpo2TLSServerCertificate!;
                                                                               }
                                                                             : null,
                                           AutoStart:                    true
                                       );

            cpo2HTTPServer           = new HTTPTestServerX(
                                           TCPPort:                      IPPort.Parse(3302),
                                           DNSClient:                    DNSClient,
                                           ServerCertificateSelector:    cpo2TLSServerCertificate is not null
                                                                             ? (tcpServer, tcpClient) => {
                                                                                   return cpo2TLSServerCertificate!;
                                                                               }
                                                                             : null,
                                           ClientCertificateValidator:   emsp1cpo2TLSClientCertificate is not null &&
                                                                         emsp2cpo2TLSClientCertificate is not null
                                                                             ? (sender,
                                                                               clientCertificate,
                                                                               clientCertificateChain,
                                                                               tlsServer,
                                                                               policyErrors) => {

                                                                                   if (clientCertificate is null)
                                                                                       return (true, [ "The client certificate is null, anyway we proceed... :)" ]);

                                                                                   var chainReport = PKIFactory.ValidateClientChain(
                                                                                                         clientCertificate,
                                                                                                         clientCACertificate2!,
                                                                                                         rootCACertificate2!
                                                                                                     );

                                                                                   return (chainReport.IsValid,
                                                                                           chainReport.Status.Select(chainStatus => chainStatus.Status.ToString()));

                                                                               }
                                                                             : null,
                                           LocalCertificateSelector:     cpo2TLSServerCertificate is not null
                                                                             ? (sender,
                                                                                targetHost,
                                                                                localCertificates,
                                                                                remoteCertificate,
                                                                                acceptableIssuers) => {
                                                                                    return cpo2TLSServerCertificate!;
                                                                               }
                                                                             : null,
                                           AutoStart:                    true
                                       );

            emsp1HTTPServer          = new HTTPTestServerX(
                                           TCPPort:                      IPPort.Parse(3401),
                                           DNSClient:                    DNSClient,
                                           ServerCertificateSelector:    (tcpServer, tcpClient) => {
                                                                             return emsp1TLSServerCertificate!;
                                                                         },
                                           ClientCertificateValidator:   emsp1cpo1TLSClientCertificate is not null &&
                                                                         emsp2cpo1TLSClientCertificate is not null
                                                                             ? (sender,
                                                                                clientCertificate,
                                                                                clientCertificateChain,
                                                                                tlsServer,
                                                                                policyErrors) => {

                                                                                    if (clientCertificate is null)
                                                                                        return (true, [ "The client certificate is null, anyway we proceed... :)" ]);

                                                                                    var chainReport = PKIFactory.ValidateClientChain(
                                                                                                          clientCertificate,
                                                                                                          clientCACertificate2!,
                                                                                                          rootCACertificate2!
                                                                                                      );

                                                                                    return (chainReport.IsValid,
                                                                                            chainReport.Status.Select(chainStatus => chainStatus.Status.ToString()));

                                                                               }
                                                                             : null,
                                           LocalCertificateSelector:     (sender,
                                                                          targetHost,
                                                                          localCertificates,
                                                                          remoteCertificate,
                                                                          acceptableIssuers) => {
                                                                              return emsp1TLSServerCertificate!;
                                                                         },
                                           AutoStart:                    true
                                       );

            emsp2HTTPServer          = new HTTPTestServerX(
                                           TCPPort:                      IPPort.Parse(3402),
                                           DNSClient:                    DNSClient,
                                           ServerCertificateSelector:    (tcpServer, tcpClient) => {
                                                                             return emsp2TLSServerCertificate!;
                                                                         },
                                           ClientCertificateValidator:   (sender,
                                                                          clientCertificate,
                                                                          clientCertificateChain,
                                                                          tlsServer,
                                                                          policyErrors) => {

                                                                              if (clientCertificate is null)
                                                                                  return (true, [ "The client certificate is null, anyway we proceed... :)" ]);

                                                                              var chainReport = PKIFactory.ValidateClientChain(
                                                                                                    clientCertificate,
                                                                                                    clientCACertificate2!,
                                                                                                    rootCACertificate2!
                                                                                                );

                                                                              return (chainReport.IsValid,
                                                                                      chainReport.Status.Select(chainStatus => chainStatus.Status.ToString()));

                                                                         },
                                           LocalCertificateSelector:     (sender,
                                                                          targetHost,
                                                                          localCertificates,
                                                                          remoteCertificate,
                                                                          acceptableIssuers) => {
                                                                              return emsp2TLSServerCertificate!;
                                                                         },
                                           AutoStart:                    true
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
                                     OurBaseURL:                  URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi"),
                                     OurVersionsURL:              URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),

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
                                     OurBaseURL:                URL.Parse("http://localhost:3202/ocpi"),
                                     OurVersionsURL:            URL.Parse("http://localhost:3202/ocpi/versions"),

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
                                     OurBaseURL:                URL.Parse("http://localhost:3401/ocpi"),
                                     OurVersionsURL:            URL.Parse("http://localhost:3401/ocpi/versions"),

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
                                     OurBaseURL:                URL.Parse("http://localhost:3402/ocpi"),
                                     OurVersionsURL:            URL.Parse("http://localhost:3402/ocpi/versions"),

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

            if (AutoWireRemoteParties)
            {

                #region CPO #1 -> EMSP #1

                var emsp1_2_cpo1_v2_1_1 = await emsp1CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                                                    CountryCode:                       cpo1CommonAPI_v2_1_1.OurCountryCode,
                                                    PartyId:                           cpo1CommonAPI_v2_1_1.OurPartyId,
                                                    Role:                              Role.CPO,
                                                    BusinessDetails:                   cpo1CommonAPI_v2_1_1.OurBusinessDetails,

                                                    LocalAccessToken:                  AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp1__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    false,

                                                    RemoteAccessToken:                 AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo1__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   false,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(emsp1_2_cpo1_v2_1_1.IsSuccess,  Is.True);
                Assert.That(emsp1_2_cpo1_v2_1_1.Data,       Is.Not.Null);


                var emsp1_2_cpo1_v2_2_1 = await emsp1CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           cpo1CommonAPI_v2_2_1.DefaultPartyId,
                                                                                           Role.CPO
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           cpo1CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                                                       ],
                                                    LocalAccessToken:                  AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp1__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo1__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(emsp1_2_cpo1_v2_2_1.IsSuccess,  Is.True);
                Assert.That(emsp1_2_cpo1_v2_2_1.Data,       Is.Not.Null);


                var emsp1_2_cpo1_v2_3_0 = await emsp1CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           cpo1CommonAPI_v2_3_0.DefaultPartyId,
                                                                                           Role.CPO
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           cpo1CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                                                       ],
                                                    LocalAccessToken:                  AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp1__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo1__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_3_0.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_3_0.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(emsp1_2_cpo1_v2_3_0.IsSuccess,  Is.True);
                Assert.That(emsp1_2_cpo1_v2_3_0.Data,       Is.Not.Null);

                #endregion

                #region CPO #1 -> EMSP #2

                var emsp2_2_cpo1_v2_1_1 = await emsp2CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                                                    CountryCode:                       cpo1CommonAPI_v2_1_1.OurCountryCode,
                                                    PartyId:                           cpo1CommonAPI_v2_1_1.OurPartyId,
                                                    Role:                              Role.CPO,
                                                    BusinessDetails:                   cpo1CommonAPI_v2_1_1.OurBusinessDetails,

                                                    LocalAccessToken:                  AccessToken.Parse(cpo1_accessing_emsp2__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp2__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    false,

                                                    RemoteAccessToken:                 AccessToken.Parse(emsp2_accessing_cpo1__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp2_accessing_cpo1__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   false,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(emsp2_2_cpo1_v2_1_1.IsSuccess,  Is.True);
                Assert.That(emsp2_2_cpo1_v2_1_1.Data,       Is.Not.Null);


                var emsp2_2_cpo1_v2_2_1 = await emsp2CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           cpo1CommonAPI_v2_2_1.DefaultPartyId,
                                                                                           Role.CPO
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           cpo1CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                                                       ],
                                                    LocalAccessToken:                  AccessToken.Parse(cpo1_accessing_emsp2__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp2__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(emsp2_accessing_cpo1__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp2_accessing_cpo1__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(emsp2_2_cpo1_v2_2_1.IsSuccess,  Is.True);
                Assert.That(emsp2_2_cpo1_v2_2_1.Data,       Is.Not.Null);


                var emsp2_2_cpo1_v2_3_0 = await emsp2CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           cpo1CommonAPI_v2_3_0.DefaultPartyId,
                                                                                           Role.CPO
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           cpo1CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                                                       ],
                                                    LocalAccessToken:                  AccessToken.Parse(cpo1_accessing_emsp2__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp2__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(emsp2_accessing_cpo1__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp2_accessing_cpo1__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo1HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_3_0.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_3_0.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(emsp2_2_cpo1_v2_3_0.IsSuccess,  Is.True);
                Assert.That(emsp2_2_cpo1_v2_3_0.Data,       Is.Not.Null);

                #endregion


                #region CPO #2 -> EMSP #1

                var emsp1_2_cpo2_v2_1_1 = await emsp1CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                                                    CountryCode:                       cpo2CommonAPI_v2_1_1.OurCountryCode,
                                                    PartyId:                           cpo2CommonAPI_v2_1_1.OurPartyId,
                                                    Role:                              Role.CPO,
                                                    BusinessDetails:                   cpo2CommonAPI_v2_1_1.OurBusinessDetails,

                                                    LocalAccessToken:                  AccessToken.Parse(cpo2_accessing_emsp1__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo2_accessing_emsp1__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    false,

                                                    RemoteAccessToken:                 AccessToken.Parse(emsp1_accessing_cpo2__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo2__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo2HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   false,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(emsp1_2_cpo2_v2_1_1.IsSuccess,  Is.True);
                Assert.That(emsp1_2_cpo2_v2_1_1.Data,       Is.Not.Null);


                var emsp1_2_cpo2_v2_2_1 = await emsp1CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           cpo2CommonAPI_v2_2_1.DefaultPartyId,
                                                                                           Role.CPO
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           cpo2CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                                                       ],
                                                    LocalAccessToken:                  AccessToken.Parse(cpo2_accessing_emsp1__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo2_accessing_emsp1__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(emsp1_accessing_cpo2__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo2__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo2HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(emsp1_2_cpo2_v2_2_1.IsSuccess,  Is.True);
                Assert.That(emsp1_2_cpo2_v2_2_1.Data,       Is.Not.Null);


                var emsp1_2_cpo2_v2_3_0 = await emsp1CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           cpo2CommonAPI_v2_3_0.DefaultPartyId,
                                                                                           Role.CPO
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           cpo2CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                                                       ],
                                                    LocalAccessToken:                  AccessToken.Parse(cpo2_accessing_emsp1__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo2_accessing_emsp1__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(emsp1_accessing_cpo2__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo2__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo2HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_3_0.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_3_0.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(emsp1_2_cpo2_v2_3_0.IsSuccess,  Is.True);
                Assert.That(emsp1_2_cpo2_v2_3_0.Data,       Is.Not.Null);

                #endregion

                #region CPO #2 -> EMSP #2

                var emsp2_2_cpo2_v2_1_1 = await emsp2CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                                                    CountryCode:                       cpo2CommonAPI_v2_1_1.OurCountryCode,
                                                    PartyId:                           cpo2CommonAPI_v2_1_1.OurPartyId,
                                                    Role:                              Role.CPO,
                                                    BusinessDetails:                   cpo2CommonAPI_v2_1_1.OurBusinessDetails,

                                                    LocalAccessToken:                  AccessToken.Parse(cpo2_accessing_emsp2__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo2_accessing_emsp2__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    false,

                                                    RemoteAccessToken:                 AccessToken.Parse(emsp2_accessing_cpo2__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp2_accessing_cpo2__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo2HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   false,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(emsp2_2_cpo2_v2_1_1.IsSuccess,  Is.True);
                Assert.That(emsp2_2_cpo2_v2_1_1.Data,       Is.Not.Null);


                var emsp2_2_cpo2_v2_2_1 = await emsp2CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           cpo2CommonAPI_v2_2_1.DefaultPartyId,
                                                                                           Role.CPO
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           cpo2CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                                                       ],
                                                    LocalAccessToken:                  AccessToken.Parse(cpo2_accessing_emsp2__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo2_accessing_emsp2__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(emsp2_accessing_cpo2__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp2_accessing_cpo2__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo2HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(emsp2_2_cpo2_v2_2_1.IsSuccess,  Is.True);
                Assert.That(emsp2_2_cpo2_v2_2_1.Data,       Is.Not.Null);


                var emsp2_2_cpo2_v2_3_0 = await emsp2CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           cpo2CommonAPI_v2_3_0.DefaultPartyId,
                                                                                           Role.CPO
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           cpo2CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                                                       ],
                                                    LocalAccessToken:                  AccessToken.Parse(cpo2_accessing_emsp2__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(cpo2_accessing_emsp2__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(emsp2_accessing_cpo2__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(emsp2_accessing_cpo2__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpo2HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_3_0.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_3_0.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(emsp2_2_cpo2_v2_3_0.IsSuccess,  Is.True);
                Assert.That(emsp2_2_cpo2_v2_3_0.Data,       Is.Not.Null);

                #endregion


                #region EMSP #1 -> CPO #1

                var cpo1_2_emsp1_v2_1_1 = await cpo1CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                                                    CountryCode:                       emsp1CommonAPI_v2_1_1.OurCountryCode,
                                                    PartyId:                           emsp1CommonAPI_v2_1_1.OurPartyId,
                                                    Role:                              Role.EMSP,
                                                    BusinessDetails:                   emsp1CommonAPI_v2_1_1.OurBusinessDetails,

                                                    LocalAccessToken:                  AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo1__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    false,

                                                    RemoteAccessToken:                 AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp1__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_1_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_1_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   false,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(cpo1_2_emsp1_v2_1_1.IsSuccess,  Is.True);
                Assert.That(cpo1_2_emsp1_v2_1_1.Data,       Is.Not.Null);


                var cpo1_2_emsp1_v2_2_1 = await cpo1CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           emsp1CommonAPI_v2_2_1.DefaultPartyId,
                                                                                           Role.EMSP
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           emsp1CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                                                       ],

                                                    LocalAccessToken:                  AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo1__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp1__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(cpo1_2_emsp1_v2_2_1.IsSuccess,  Is.True);
                Assert.That(cpo1_2_emsp1_v2_2_1.Data,       Is.Not.Null);


                var cpo1_2_emsp1_v2_3_0 = await cpo1CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           emsp1CommonAPI_v2_3_0.DefaultPartyId,
                                                                                           Role.EMSP
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           emsp1CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                                                       ],
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    LocalAccessToken:                  AccessToken.Parse(emsp1_accessing_cpo1__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo1__token, TOTPValidityTime) : null,

                                                    RemoteAccessToken:                 AccessToken.Parse(cpo1_accessing_emsp1__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp1__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_3_0.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_3_0.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(cpo1_2_emsp1_v2_3_0.IsSuccess,  Is.True);
                Assert.That(cpo1_2_emsp1_v2_3_0.Data,       Is.Not.Null);

                #endregion

                #region EMSP #2 -> CPO #1

                var cpo1_2_emsp2_v2_1_1 = await cpo1CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                                                    CountryCode:                       emsp2CommonAPI_v2_1_1.OurCountryCode,
                                                    PartyId:                           emsp2CommonAPI_v2_1_1.OurPartyId,
                                                    Role:                              Role.EMSP,
                                                    BusinessDetails:                   emsp2CommonAPI_v2_1_1.OurBusinessDetails,

                                                    LocalAccessToken:                  AccessToken.Parse(emsp2_accessing_cpo1__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp2_accessing_cpo1__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    false,

                                                    RemoteAccessToken:                 AccessToken.Parse(cpo1_accessing_emsp2__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp2__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp2HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_1_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_1_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   false,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(cpo1_2_emsp2_v2_1_1.IsSuccess,  Is.True);
                Assert.That(cpo1_2_emsp2_v2_1_1.Data,       Is.Not.Null);


                var cpo1_2_emsp2_v2_2_1 = await cpo1CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           emsp2CommonAPI_v2_2_1.DefaultPartyId,
                                                                                           Role.EMSP
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           emsp2CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                                                       ],

                                                    LocalAccessToken:                  AccessToken.Parse(emsp2_accessing_cpo1__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp2_accessing_cpo1__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(cpo1_accessing_emsp2__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp2__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp2HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(cpo1_2_emsp2_v2_2_1.IsSuccess,  Is.True);
                Assert.That(cpo1_2_emsp2_v2_2_1.Data,       Is.Not.Null);


                var cpo1_2_emsp2_v2_3_0 = await cpo1CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           emsp2CommonAPI_v2_3_0.DefaultPartyId,
                                                                                           Role.EMSP
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           emsp2CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                                                       ],

                                                    LocalAccessToken:                  AccessToken.Parse(emsp2_accessing_cpo1__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp2_accessing_cpo1__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(cpo1_accessing_emsp2__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo1_accessing_emsp2__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp2HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_3_0.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_3_0.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(cpo1_2_emsp2_v2_3_0.IsSuccess,  Is.True);
                Assert.That(cpo1_2_emsp2_v2_3_0.Data,       Is.Not.Null);

                #endregion


                #region EMSP #1 -> CPO #2

                var cpo2_2_emsp1_v2_1_1 = await cpo2CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                                                    CountryCode:                       emsp1CommonAPI_v2_1_1.OurCountryCode,
                                                    PartyId:                           emsp1CommonAPI_v2_1_1.OurPartyId,
                                                    Role:                              Role.EMSP,
                                                    BusinessDetails:                   emsp1CommonAPI_v2_1_1.OurBusinessDetails,

                                                    LocalAccessToken:                  AccessToken.Parse(emsp1_accessing_cpo2__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo2__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    false,

                                                    RemoteAccessToken:                 AccessToken.Parse(cpo2_accessing_emsp1__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo2_accessing_emsp1__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_1_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_1_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   false,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(cpo2_2_emsp1_v2_1_1.IsSuccess,  Is.True);
                Assert.That(cpo2_2_emsp1_v2_1_1.Data,       Is.Not.Null);


                var cpo2_2_emsp1_v2_2_1 = await cpo2CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           emsp1CommonAPI_v2_2_1.DefaultPartyId,
                                                                                           Role.EMSP
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           emsp1CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                                                       ],

                                                    LocalAccessToken:                  AccessToken.Parse(emsp1_accessing_cpo2__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo2__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(cpo2_accessing_emsp1__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo2_accessing_emsp1__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(cpo2_2_emsp1_v2_2_1.IsSuccess,  Is.True);
                Assert.That(cpo2_2_emsp1_v2_2_1.Data,       Is.Not.Null);


                var cpo2_2_emsp1_v2_3_0 = await cpo2CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           emsp1CommonAPI_v2_3_0.DefaultPartyId,
                                                                                           Role.EMSP
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           emsp1CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                                                       ],

                                                    LocalAccessToken:                  AccessToken.Parse(emsp1_accessing_cpo2__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp1_accessing_cpo2__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(cpo2_accessing_emsp1__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo2_accessing_emsp1__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp1HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_3_0.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_3_0.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(cpo2_2_emsp1_v2_3_0.IsSuccess,  Is.True);
                Assert.That(cpo2_2_emsp1_v2_3_0.Data,       Is.Not.Null);

                #endregion

                #region EMSP #2 -> CPO #2

                var cpo2_2_emsp2_v2_1_1 = await cpo2CommonAPI_v2_1_1.AddRemotePartyIfNotExists(
                                                    CountryCode:                       emsp2CommonAPI_v2_1_1.OurCountryCode,
                                                    PartyId:                           emsp2CommonAPI_v2_1_1.OurPartyId,
                                                    Role:                              Role.EMSP,
                                                    BusinessDetails:                   emsp2CommonAPI_v2_1_1.OurBusinessDetails,

                                                    LocalAccessToken:                  AccessToken.Parse(emsp2_accessing_cpo2__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp2_accessing_cpo2__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    false,

                                                    RemoteAccessToken:                 AccessToken.Parse(cpo2_accessing_emsp2__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo2_accessing_emsp2__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp2HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_1_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_1_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   false,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(cpo2_2_emsp2_v2_1_1.IsSuccess,  Is.True);
                Assert.That(cpo2_2_emsp2_v2_1_1.Data,       Is.Not.Null);


                var cpo2_2_emsp2_v2_2_1 = await cpo2CommonAPI_v2_2_1.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           emsp2CommonAPI_v2_2_1.DefaultPartyId,
                                                                                           Role.EMSP
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           emsp2CommonAPI_v2_2_1.Parties.First().ToCredentialsRole()
                                                                                       ],

                                                    LocalAccessToken:                  AccessToken.Parse(emsp2_accessing_cpo2__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp2_accessing_cpo2__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(cpo2_accessing_emsp2__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo2_accessing_emsp2__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp2HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_2_1.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_2_1.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(cpo2_2_emsp2_v2_2_1.IsSuccess,  Is.True);
                Assert.That(cpo2_2_emsp2_v2_2_1.Data,       Is.Not.Null);


                var cpo2_2_emsp2_v2_3_0 = await cpo2CommonAPI_v2_3_0.AddRemotePartyIfNotExists(
                                                    Id:                                RemoteParty_Id.Parse(
                                                                                           emsp2CommonAPI_v2_3_0.DefaultPartyId,
                                                                                           Role.EMSP
                                                                                       ),
                                                    CredentialsRoles:                  [
                                                                                           emsp2CommonAPI_v2_3_0.Parties.First().ToCredentialsRole()
                                                                                       ],

                                                    LocalAccessToken:                  AccessToken.Parse(emsp2_accessing_cpo2__token),
                                                    LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                    LocalTOTPConfig:                   TOTPValidityTime.HasValue ? new TOTPConfig(emsp2_accessing_cpo2__token, TOTPValidityTime) : null,
                                                    LocalAccessTokenBase64Encoding:    true,

                                                    RemoteAccessToken:                 AccessToken.Parse(cpo2_accessing_emsp2__token),
                                                    RemoteTOTPConfig:                  TOTPValidityTime.HasValue ? new TOTPConfig(cpo2_accessing_emsp2__token, TOTPValidityTime) : null,
                                                    RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp2HTTPServer.TCPPort}/ocpi/versions"),
                                                    RemoteVersionIds:                  [ OCPIv2_3_0.Version.Id ],
                                                    SelectedVersionId:                 OCPIv2_3_0.Version.Id,
                                                    RemoteAccessTokenBase64Encoding:   true,
                                                    RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                    ClientCertificates:                cpo1emsp1TLSClientKeyPair is not null
                                                                                           ? null
                                                                                           : null,

                                                    Status:                            PartyStatus.ENABLED
                                                );

                Assert.That(cpo2_2_emsp2_v2_3_0.IsSuccess,  Is.True);
                Assert.That(cpo2_2_emsp2_v2_3_0.Data,       Is.Not.Null);

                #endregion


                #region Validate Remote Parties

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

                #endregion

                #region Check the RemoteParty database files

                Assert.That(await CheckFile(cpo1CommonAPI_v2_1_1. RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
                Assert.That(await CheckFile(cpo1CommonAPI_v2_1_1. RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
                Assert.That(await CheckFile(cpo1CommonAPI_v2_1_1. RemotePartyDBFileName, lines => lines.Length == 2), Is.True);

                Assert.That(await CheckFile(cpo2CommonAPI_v2_1_1. RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
                Assert.That(await CheckFile(cpo2CommonAPI_v2_1_1. RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
                Assert.That(await CheckFile(cpo2CommonAPI_v2_1_1. RemotePartyDBFileName, lines => lines.Length == 2), Is.True);

                Assert.That(await CheckFile(emsp1CommonAPI_v2_1_1.RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
                Assert.That(await CheckFile(emsp1CommonAPI_v2_1_1.RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
                Assert.That(await CheckFile(emsp1CommonAPI_v2_1_1.RemotePartyDBFileName, lines => lines.Length == 2), Is.True);

                Assert.That(await CheckFile(emsp2CommonAPI_v2_1_1.RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
                Assert.That(await CheckFile(emsp2CommonAPI_v2_1_1.RemotePartyDBFileName, lines => lines.Length == 2), Is.True);
                Assert.That(await CheckFile(emsp2CommonAPI_v2_1_1.RemotePartyDBFileName, lines => lines.Length == 2), Is.True);

                #endregion


                #region Define blocked Remote Parties

                await cpo1CommonAPI_v2_1_1.AddRemoteParty(
                            CountryCode:         CountryCode.Parse("XX"),
                            PartyId:             Party_Id.   Parse("BLE"),
                            Role:                Role.EMSP,
                            BusinessDetails:     new BusinessDetails(
                                                    "Blocked EMSP"
                                                ),
                            LocalAccessToken:    BlockedEMSPToken,
                            LocalAccessStatus:   AccessStatus.BLOCKED,
                            Status:              PartyStatus. ENABLED
                        );


                await emsp1CommonAPI_v2_1_1.AddRemoteParty(
                            CountryCode:         CountryCode.Parse("XX"),
                            PartyId:             Party_Id.   Parse("BLC"),
                            Role:                Role.EMSP,
                            BusinessDetails:     new BusinessDetails(
                                                    "Blocked CPO"
                                                ),
                            LocalAccessToken:    BlockedCPOToken,
                            LocalAccessStatus:   AccessStatus.BLOCKED,
                            Status:              PartyStatus. ENABLED
                        );

                #endregion

                #region Define blocked Access Tokens

                await cpo1CommonAPI_v2_1_1.AddAccessToken(
                          BlockedToken,
                          AccessStatus.BLOCKED
                      );

                await cpo2CommonAPI_v2_1_1.AddAccessToken(
                          BlockedToken,
                          AccessStatus.BLOCKED
                      );

                await emsp1CommonAPI_v2_1_1.AddAccessToken(
                          BlockedToken,
                          AccessStatus.BLOCKED
                      );

                await emsp2CommonAPI_v2_1_1.AddAccessToken(
                          BlockedToken,
                          AccessStatus.BLOCKED
                      );

                #endregion

            }

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

            #region CPO #1

            if (cpo1HTTPServer is not null)
                await cpo1HTTPServer. Stop();


            if (cpo1CommonAPI_v2_1_1 is not null)
                File.Delete(cpo1CommonAPI_v2_1_1.RemotePartyDBFileName);

            if (cpo1CommonAPI_v2_2_1 is not null)
                File.Delete(cpo1CommonAPI_v2_2_1.RemotePartyDBFileName);

            if (cpo1CommonAPI_v2_3_0 is not null)
                File.Delete(cpo1CommonAPI_v2_3_0.RemotePartyDBFileName);

            if (cpo1CommonAPI_v3_0   is not null)
                File.Delete(cpo1CommonAPI_v3_0.  RemotePartyDBFileName);

            #endregion

            #region CPO #2

            if (cpo2HTTPServer is not null)
                await cpo2HTTPServer. Stop();


            if (cpo2CommonAPI_v2_1_1 is not null)
                File.Delete(cpo2CommonAPI_v2_1_1.RemotePartyDBFileName);

            if (cpo2CommonAPI_v2_2_1 is not null)
                File.Delete(cpo2CommonAPI_v2_2_1.RemotePartyDBFileName);

            if (cpo2CommonAPI_v2_3_0 is not null)
                File.Delete(cpo2CommonAPI_v2_3_0.RemotePartyDBFileName);

            if (cpo2CommonAPI_v3_0   is not null)
                File.Delete(cpo2CommonAPI_v3_0.  RemotePartyDBFileName);

            #endregion

            #region EMSP #1

            if (emsp1HTTPServer is not null)
                await emsp1HTTPServer.Stop();


            if (emsp1CommonAPI_v2_1_1 is not null)
                File.Delete(emsp1CommonAPI_v2_1_1.RemotePartyDBFileName);

            if (emsp1CommonAPI_v2_2_1 is not null)
                File.Delete(emsp1CommonAPI_v2_2_1.RemotePartyDBFileName);

            if (emsp1CommonAPI_v2_3_0 is not null)
                File.Delete(emsp1CommonAPI_v2_3_0.RemotePartyDBFileName);

            if (emsp1CommonAPI_v3_0   is not null)
                File.Delete(emsp1CommonAPI_v3_0.  RemotePartyDBFileName);

            #endregion

            #region EMSP #2

            if (emsp2HTTPServer is not null)
                await emsp2HTTPServer.Stop();


            if (emsp2CommonAPI_v2_1_1 is not null)
                File.Delete(emsp2CommonAPI_v2_1_1.RemotePartyDBFileName);

            if (emsp2CommonAPI_v2_2_1 is not null)
                File.Delete(emsp2CommonAPI_v2_2_1.RemotePartyDBFileName);

            if (emsp2CommonAPI_v2_3_0 is not null)
                File.Delete(emsp2CommonAPI_v2_3_0.RemotePartyDBFileName);

            if (emsp2CommonAPI_v3_0   is not null)
                File.Delete(emsp2CommonAPI_v3_0.  RemotePartyDBFileName);

            #endregion

        }

        #endregion


    }

}
