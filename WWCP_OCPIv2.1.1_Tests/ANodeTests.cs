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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using NUnit.Framework;
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_1_1.WebAPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests
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
    /// OCPI v2.1.1 Node test defaults.
    /// </summary>
    public abstract class ANodeTests
    {

        #region Data

        protected       HTTPTestServerX?                                     cpoHTTPServer;
        protected       HTTPTestServerX?                                     emsp1HTTPServer;
        protected       HTTPTestServerX?                                     emsp2HTTPServer;

        protected       HTTPExtAPIX?                                         cpoHTTPAPI;
        protected       HTTPExtAPIX?                                         emsp1HTTPAPI;
        protected       HTTPExtAPIX?                                         emsp2HTTPAPI;

  //      protected       HTTPAPI?                                             cpoHTTPAPI;
        protected       CommonAPI?                                           cpoCommonAPI;
        protected       OCPIWebAPI?                                          cpoWebAPI;
        protected       CPO_HTTPAPI?                                         cpoCPOAPI;
        protected       OCPICSOAdapter?                                      cpoAdapter;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIRequest>    cpoAPIRequestLogs;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIResponse>   cpoAPIResponseLogs;

   //     protected       HTTPAPI?                                             emsp1HTTPAPI;
        protected       CommonAPI?                                           emsp1CommonAPI;
        protected       OCPIWebAPI?                                          emsp1WebAPI;
        protected       EMSP_HTTPAPI?                                        emsp1EMSPAPI;
        protected       OCPIEMPAdapter?                                      emsp1Adapter;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIRequest>    emsp1APIRequestLogs;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIResponse>   emsp1APIResponseLogs;

   //     protected       HTTPAPI?                                             emsp2HTTPAPI;
        protected       CommonAPI?                                           emsp2CommonAPI;
        protected       OCPIWebAPI?                                          emsp2WebAPI;
        protected       EMSP_HTTPAPI?                                        emsp2EMSPAPI;
        protected       OCPIEMPAdapter?                                      emsp2Adapter;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIRequest>    emsp2APIRequestLogs;
        protected       ConcurrentDictionary<DateTimeOffset, OCPIResponse>   emsp2APIResponseLogs;

        public          URL?                                                 cpoVersionsAPIURL;
        public          URL?                                                 emsp1VersionsAPIURL;
        public          URL?                                                 emsp2VersionsAPIURL;

        protected const String                                               cpo_accessing_emsp1__token  = "cpo_accessing_emsp1++token";
        protected const String                                               cpo_accessing_emsp2__token  = "cpo_accessing_emsp2++token";

        protected const String                                               emsp1_accessing_cpo__token  = "emsp1_accessing_cpo++token";
        protected const String                                               emsp2_accessing_cpo__token  = "emsp2_accessing_cpo++token";

        protected const String                                               UnknownToken                = "UnknownUnknownUnknownToken";

        protected const String                                               BlockedCPOToken             = "blocked-cpo";
        protected const String                                               BlockedEMSPToken            = "blocked-emsp";

        #endregion

        #region Constructor(s)

        public ANodeTests()
        {

            //this.EVSEDataRecords      = new Dictionary<Operator_Id, HashSet<EVSEDataRecord>>();
            //this.EVSEStatusRecords    = new Dictionary<Operator_Id, HashSet<EVSEStatusRecord>>();
            //this.PricingProductData   = new Dictionary<Operator_Id, HashSet<PricingProductDataRecord>>();
            //this.EVSEPricings         = new Dictionary<Operator_Id, HashSet<EVSEPricing>>();
            //this.ChargeDetailRecords  = new Dictionary<Operator_Id, HashSet<ChargeDetailRecord>>();

        }

        #endregion


        #region SetupOnce()

        [OneTimeSetUp]
        public void SetupOnce()
        {

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public async virtual Task SetupEachTest()
        {

            Timestamp.Reset();

            #region Create cpo/emsp1/emsp2 HTTP Servers

            cpoHTTPServer         = new HTTPTestServerX(
                                        TCPPort:         IPPort.Parse(3301)
                                    );

            emsp1HTTPServer       = new HTTPTestServerX(
                                        TCPPort:         IPPort.Parse(3401)
                                    );

            emsp2HTTPServer       = new HTTPTestServerX(
                                        TCPPort:         IPPort.Parse(3402)
                                    );

            #endregion

            #region Create cpo/emsp1/emsp2 HTTP APIs

            cpoHTTPAPI            = new HTTPExtAPIX(
                                        HTTPServer:      cpoHTTPServer
                                    );

            emsp1HTTPAPI          = new HTTPExtAPIX(
                                        HTTPServer:      emsp1HTTPServer
                                    );

            emsp2HTTPAPI          = new HTTPExtAPIX(
                                        HTTPServer:      emsp2HTTPServer
                                    );

            #endregion

            var ocpiBaseAPI = new CommonHTTPAPI(

                                  HTTPAPI:                   cpoHTTPAPI,
                                  OurBaseURL:                URL.Parse("http://127.0.0.1:3201/ocpi"),
                                  OurVersionsURL:            URL.Parse("http://127.0.0.1:3201/ocpi/versions"),
                                  //OurBusinessDetails:        new OCPI.BusinessDetails(
                                  //                               "GraphDefined OCPI Services",
                                  //                               URL.Parse("https://www.graphdefined.com/ocpi")
                                  //                           ),
                                  //OurCountryCode:            OCPI.CountryCode.Parse("DE"),
                                  //OurPartyId:                OCPI.Party_Id.   Parse("BDO"),
                                  //OurRole:                   OCPI.Role.       CPO,

                                  AdditionalURLPathPrefix:   null,
                                  //KeepRemovedEVSEs:          null,
                                  LocationsAsOpenData:       true,
                                  AllowDowngrades:           null,

                                  ExternalDNSName:           null,
                                  HTTPServiceName:           null,
                                  BasePath:                  null,

                                  RootPath:                  HTTPPath.Parse("/ocpi"),
                                  APIVersionHashes:          null,

                                  IsDevelopment:             null,
                                  DevelopmentServers:        null,
                                  DisableLogging:            null,
                                  LoggingContext:            null,
                                  LoggingPath:               null,
                                  LogfileName:               null,
                                  LogfileCreator:            null

                              );

            ClassicAssert.IsNotNull(ocpiBaseAPI);

            var commonWebAPI = new OCPI.WebAPI.CommonWebAPI(
                                   ocpiBaseAPI
                                  // HTTPServer: cpoHTTPAPI.HTTPServer
                               );


            #region Create cpo/emsp1/emsp2 OCPI Common API

            // Clean up log and database directories...
            foreach (var filePath in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory,
                                                                     HTTPAPI.DefaultHTTPAPI_LoggingPath),
                                                        $"GraphDefined_OCPI{Version.String}_*.log"))
            {
                File.Delete(filePath);
            }


            //cpoVersionsAPIURL    = URL.Parse("http://127.0.0.1:3301/ocpi/v2.1/versions");

            cpoCommonAPI         = new CommonAPI(

                                       //OurBaseURL:                          URL.Parse("http://127.0.0.1:3301/ocpi/v2.1"),
                                       //OurVersionsURL:                      ocpiBaseAPI.OurVersionsURL,
                                       OurBusinessDetails:                  new BusinessDetails(
                                                                                "GraphDefined CSO Services",
                                                                                URL.Parse("https://www.graphdefined.com/cso")
                                                                            ),
                                       OurCountryCode:                      CountryCode.Parse("DE"),
                                       OurPartyId:                          Party_Id.   Parse("GEF"),
                                       OurRole:                             Role.       CPO,

                                       BaseAPI:                             ocpiBaseAPI,
                                       //HTTPServer:                          cpoHTTPServer,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         $"GraphDefined_OCPI{Version.String}_CSO.log",
                                       LogfileCreator:                      null,
                                       DatabaseFilePath:                    null,
                                       RemotePartyDBFileName:               $"GraphDefined_OCPI{Version.String}_RemoteParties_CPO.log",
                                       AssetsDBFileName:                    $"GraphDefined_OCPI{Version.String}_Assets_CPO.log"

                                   );


            emsp1VersionsAPIURL  = URL.Parse("http://127.0.0.1:3401/ocpi/v2.1/versions");

            emsp1CommonAPI       = new CommonAPI(

                                       //OurBaseURL:                          URL.Parse("http://127.0.0.1:3401/ocpi/v2.1"),
                                       //OurVersionsURL:                      emsp1VersionsAPIURL.Value,
                                       OurBusinessDetails:                  new BusinessDetails(
                                                                                "GraphDefined EMSP #1 Services",
                                                                                URL.Parse("https://www.graphdefined.com/emsp1")
                                                                            ),
                                       OurCountryCode:                      CountryCode.Parse("DE"),
                                       OurPartyId:                          Party_Id.   Parse("GDF"),
                                       OurRole:                             Role.       EMSP,

                                       BaseAPI:                             ocpiBaseAPI,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         $"GraphDefined_OCPI{Version.String}_EMSP1.log",
                                       LogfileCreator:                      null,
                                       DatabaseFilePath:                    null,
                                       RemotePartyDBFileName:               $"GraphDefined_OCPI{Version.String}_RemoteParties_EMSP1.log",
                                       AssetsDBFileName:                    $"GraphDefined_OCPI{Version.String}_Assets_EMSP1.log"

                                   );


            emsp2VersionsAPIURL  = URL.Parse("http://127.0.0.1:3402/ocpi/v2.1/versions");

            emsp2CommonAPI       = new CommonAPI(

                                       //OurBaseURL:                          URL.Parse("http://127.0.0.1:3402/ocpi/v2.1"),
                                       //OurVersionsURL:                      emsp2VersionsAPIURL.Value,
                                       OurBusinessDetails:                  new BusinessDetails(
                                                                                "GraphDefined EMSP #2 Services",
                                                                                URL.Parse("https://www.graphdefined.com/emsp2")
                                                                            ),
                                       OurCountryCode:                      CountryCode.Parse("DE"),
                                       OurPartyId:                          Party_Id.   Parse("GD2"),
                                       OurRole:                             Role.       EMSP,

                                       BaseAPI:                             ocpiBaseAPI,
                                       //HTTPServer:                          emsp2HTTPServer,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         $"GraphDefined_OCPI{Version.String}_EMSP2.log",
                                       LogfileCreator:                      null,
                                       DatabaseFilePath:                    null,
                                       RemotePartyDBFileName:               $"GraphDefined_OCPI{Version.String}_RemoteParties_EMSP2.log",
                                       AssetsDBFileName:                    $"GraphDefined_OCPI{Version.String}_Assets_EMSP2.log"

                                   );

            ClassicAssert.IsNotNull(cpoVersionsAPIURL);
            ClassicAssert.IsNotNull(emsp1VersionsAPIURL);
            ClassicAssert.IsNotNull(emsp2VersionsAPIURL);

            ClassicAssert.IsNotNull(cpoCommonAPI);
            ClassicAssert.IsNotNull(emsp1CommonAPI);
            ClassicAssert.IsNotNull(emsp2CommonAPI);

            #endregion

            #region Create cpo/emsp1/emsp2 OCPI WebAPI

            cpoWebAPI            = new OCPIWebAPI(
                                       CommonWebAPI:                        new OCPI.WebAPI.CommonWebAPI(
                                                                                ocpiBaseAPI,
                                                                                //HTTPServer:             cpoHTTPAPI.HTTPServer,
                                                                                OverlayURLPathPrefix:   HTTPPath.Parse("/ocpi/v2.1.1")
                                                                            ),
                                       CommonAPI:                           cpoCommonAPI,
                                       APIURLPathPrefix:                    HTTPPath.Parse("/ocpi/v2.1.1/api"),
                                       WebAPIURLPathPrefix:                 HTTPPath.Parse("/ocpi/v2.1.1/webapi"),
                                       BasePath:                            HTTPPath.Parse("/ocpi/v2.1.1")
                                   );

            emsp1WebAPI          = new OCPIWebAPI(
                                       CommonWebAPI:                        new OCPI.WebAPI.CommonWebAPI(
                                                                                ocpiBaseAPI,
                                                                                //HTTPServer:             emsp1HTTPAPI.HTTPServer,
                                                                                OverlayURLPathPrefix:   HTTPPath.Parse("/ocpi/v2.1.1")
                                                                            ),
                                       CommonAPI:                           emsp1CommonAPI,
                                       APIURLPathPrefix:                    HTTPPath.Parse("/ocpi/v2.1.1/api"),
                                       WebAPIURLPathPrefix:                 HTTPPath.Parse("/ocpi/v2.1.1/webapi"),
                                       BasePath:                            HTTPPath.Parse("/ocpi/v2.1.1")
                                   );

            emsp2WebAPI          = new OCPIWebAPI(
                                       CommonWebAPI:                        new OCPI.WebAPI.CommonWebAPI(
                                                                                ocpiBaseAPI,
                                                                                //HTTPServer:             emsp2HTTPAPI.HTTPServer,
                                                                                OverlayURLPathPrefix:   HTTPPath.Parse("/ocpi/v2.1.1")
                                                                            ),
                                       CommonAPI:                           emsp2CommonAPI,
                                       APIURLPathPrefix:                    HTTPPath.Parse("/ocpi/v2.1.1/api"),
                                       WebAPIURLPathPrefix:                 HTTPPath.Parse("/ocpi/v2.1.1/webapi"),
                                       BasePath:                            HTTPPath.Parse("/ocpi/v2.1.1")
                                   );

            ClassicAssert.IsNotNull(cpoWebAPI);
            ClassicAssert.IsNotNull(emsp1WebAPI);
            ClassicAssert.IsNotNull(emsp2WebAPI);

            #endregion

            #region Create cpo CPO API & emsp1/emsp2 EMP API

            cpoCPOAPI            = new CPO_HTTPAPI(

                                       CommonAPI:                           cpoCommonAPI,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1/v2.1.1/cpo"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            emsp1EMSPAPI         = new EMSP_HTTPAPI(

                                       CommonAPI:                           emsp1CommonAPI,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1/v2.1.1/emsp"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            emsp2EMSPAPI         = new EMSP_HTTPAPI(

                                       CommonAPI:                           emsp2CommonAPI,
                                       AllowDowngrades:                     null,

                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1/v2.1.1/emsp"),
                                       APIVersionHashes:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null

                                   );

            ClassicAssert.IsNotNull(cpoCPOAPI);
            ClassicAssert.IsNotNull(emsp1EMSPAPI);
            ClassicAssert.IsNotNull(emsp2EMSPAPI);

            #endregion

            #region Define and connect Remote Parties

            await cpoCommonAPI.AddRemoteParty  (CountryCode:                       emsp1CommonAPI.OurCountryCode,
                                                PartyId:                           emsp1CommonAPI.OurPartyId,
                                                Role:                              Role.EMSP,
                                                BusinessDetails:                   emsp1CommonAPI.OurBusinessDetails,

                                                LocalAccessToken:                  AccessToken.Parse(emsp1_accessing_cpo__token),
                                                LocalAccessStatus:                 AccessStatus.ALLOWED,

                                                RemoteAccessToken:                 AccessToken.Parse(cpo_accessing_emsp1__token),
                                                RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp1HTTPAPI.HTTPServer.TCPPort}/ocpi/v2.1/versions"),
                                                RemoteVersionIds:                  [ Version.Id ],
                                                SelectedVersionId:                 Version.Id,
                                                RemoteAccessTokenBase64Encoding:   false,
                                                RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                Status:                            PartyStatus.ENABLED);

            await cpoCommonAPI.AddRemoteParty  (CountryCode:                       emsp2CommonAPI.OurCountryCode,
                                                PartyId:                           emsp2CommonAPI.OurPartyId,
                                                Role:                              Role.EMSP,
                                                BusinessDetails:                   emsp2CommonAPI.OurBusinessDetails,
                                                LocalAccessToken:                  AccessToken.Parse(emsp2_accessing_cpo__token),
                                                LocalAccessStatus:                 AccessStatus.ALLOWED,
                                                RemoteAccessToken:                 AccessToken.Parse(cpo_accessing_emsp2__token),
                                                RemoteVersionsURL:                 URL.Parse($"http://localhost:{emsp2HTTPAPI.HTTPServer.TCPPort}/ocpi/v2.1/versions"),
                                                RemoteVersionIds:                  [ Version.Id ],
                                                SelectedVersionId:                 Version.Id,
                                                RemoteAccessTokenBase64Encoding:   false,
                                                RemoteStatus:                      RemoteAccessStatus.ONLINE,
                                                Status:                            PartyStatus.ENABLED);



            await emsp1CommonAPI.AddRemoteParty(CountryCode:                       cpoCommonAPI.OurCountryCode,
                                                PartyId:                           cpoCommonAPI.OurPartyId,
                                                Role:                              Role.CPO,
                                                BusinessDetails:                   cpoCommonAPI.OurBusinessDetails,

                                                LocalAccessToken:                  AccessToken.Parse(cpo_accessing_emsp1__token),
                                                LocalAccessStatus:                 AccessStatus.ALLOWED,

                                                RemoteAccessToken:                 AccessToken.Parse(emsp1_accessing_cpo__token),
                                                RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpoHTTPAPI.HTTPServer.TCPPort}/ocpi/v2.1/versions"),
                                                RemoteVersionIds:                  [ Version.Id ],
                                                SelectedVersionId:                 Version.Id,
                                                RemoteAccessTokenBase64Encoding:   false,
                                                RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                Status:                            PartyStatus.ENABLED);


            await emsp2CommonAPI.AddRemoteParty(CountryCode:                       cpoCommonAPI.OurCountryCode,
                                                PartyId:                           cpoCommonAPI.OurPartyId,
                                                Role:                              Role.CPO,
                                                BusinessDetails:                   cpoCommonAPI.OurBusinessDetails,

                                                LocalAccessToken:                  AccessToken.Parse(cpo_accessing_emsp2__token),
                                                LocalAccessStatus:                 AccessStatus.ALLOWED,

                                                RemoteAccessToken:                 AccessToken.Parse(emsp2_accessing_cpo__token),
                                                RemoteVersionsURL:                 URL.Parse($"http://localhost:{cpoHTTPAPI.HTTPServer.TCPPort}/ocpi/v2.1/versions"),
                                                RemoteVersionIds:                  [ Version.Id ],
                                                SelectedVersionId:                 Version.Id,
                                                RemoteAccessTokenBase64Encoding:   false,
                                                RemoteStatus:                      RemoteAccessStatus.ONLINE,

                                                Status:                            PartyStatus.ENABLED);


            ClassicAssert.AreEqual(2, cpoCommonAPI.  RemoteParties.Count());
            ClassicAssert.AreEqual(1, emsp1CommonAPI.RemoteParties.Count());
            ClassicAssert.AreEqual(1, emsp2CommonAPI.RemoteParties.Count());

            ClassicAssert.AreEqual(2, File.ReadAllLines(cpoCommonAPI.  RemotePartyDBFileName).Length);
            ClassicAssert.AreEqual(1, File.ReadAllLines(emsp1CommonAPI.RemotePartyDBFileName).Length);
            ClassicAssert.AreEqual(1, File.ReadAllLines(emsp2CommonAPI.RemotePartyDBFileName).Length);

            #endregion

            #region Define blocked Remote Parties

            await cpoCommonAPI.AddRemoteParty  (CountryCode:         CountryCode.Parse("XX"),
                                                PartyId:             Party_Id.   Parse("BLE"),
                                                Role:                Role.EMSP,
                                                BusinessDetails:     new BusinessDetails(
                                                                         "Blocked EMSP"
                                                                     ),
                                                LocalAccessToken:    AccessToken.Parse(BlockedEMSPToken),
                                                LocalAccessStatus:   AccessStatus.BLOCKED,
                                                Status:              PartyStatus. ENABLED);

            await emsp1CommonAPI.AddRemoteParty(CountryCode:         CountryCode.Parse("XX"),
                                                PartyId:             Party_Id.   Parse("BLC"),
                                                Role:                Role.CPO,
                                                BusinessDetails:     new BusinessDetails(
                                                                         "Blocked CPO"
                                                                     ),
                                                LocalAccessToken:    AccessToken.Parse(BlockedCPOToken),
                                                LocalAccessStatus:   AccessStatus.BLOCKED,
                                                Status:              PartyStatus. ENABLED);

            await emsp2CommonAPI.AddRemoteParty(CountryCode:         CountryCode.Parse("XX"),
                                                PartyId:             Party_Id.   Parse("BLC"),
                                                Role:                Role.CPO,
                                                BusinessDetails:     new BusinessDetails(
                                                                         "Blocked CPO"
                                                                     ),
                                                LocalAccessToken:    AccessToken.Parse(BlockedCPOToken),
                                                LocalAccessStatus:   AccessStatus.BLOCKED,
                                                Status:              PartyStatus. ENABLED);


            ClassicAssert.AreEqual(3, cpoCommonAPI.  RemoteParties.Count());
            ClassicAssert.AreEqual(2, emsp1CommonAPI.RemoteParties.Count());
            ClassicAssert.AreEqual(2, emsp2CommonAPI.RemoteParties.Count());

            #endregion

            #region Defined API loggers

            // CPO
            cpoAPIRequestLogs     = new ConcurrentDictionary<DateTimeOffset, OCPIRequest>();
            cpoAPIResponseLogs    = new ConcurrentDictionary<DateTimeOffset, OCPIResponse>();

            cpoCPOAPI.HTTPLogger?.RegisterLogTarget(LogTargets.Debug,
                                                      (loggingPath, context, logEventName, request) => {
                                                          cpoAPIRequestLogs. TryAdd(Timestamp.Now, request);
                                                          return Task.CompletedTask;
                                                      });

            cpoCPOAPI.HTTPLogger?.RegisterLogTarget(LogTargets.Debug,
                                                      (loggingPath, context, logEventName, request, response) => {
                                                          cpoAPIResponseLogs.TryAdd(Timestamp.Now, response);
                                                          return Task.CompletedTask;
                                                      });

            cpoCPOAPI.HTTPLogger?.Debug("all", LogTargets.Debug);

            cpoCommonAPI.BaseAPI.ClientConfigurations.Description = (remotePartyId) => I18NString.Create($"CPO Client for {remotePartyId}");



            // EMSP #1
            emsp1APIRequestLogs   = new ConcurrentDictionary<DateTimeOffset, OCPIRequest>();
            emsp1APIResponseLogs  = new ConcurrentDictionary<DateTimeOffset, OCPIResponse>();

            emsp1EMSPAPI.HTTPLogger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request) => {
                                                              emsp1APIRequestLogs. TryAdd(Timestamp.Now, request);
                                                              return Task.CompletedTask;
                                                          });

            emsp1EMSPAPI.HTTPLogger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request, response) => {
                                                              emsp1APIResponseLogs.TryAdd(Timestamp.Now, response);
                                                              return Task.CompletedTask;
                                                          });

            emsp1EMSPAPI.HTTPLogger?.Debug("all", LogTargets.Debug);

            emsp1CommonAPI.BaseAPI.ClientConfigurations.Description = (remotePartyId) => I18NString.Create($"EMSP #1 Client for {remotePartyId}");



            // EMSP #2
            emsp2APIRequestLogs   = new ConcurrentDictionary<DateTimeOffset, OCPIRequest>();
            emsp2APIResponseLogs  = new ConcurrentDictionary<DateTimeOffset, OCPIResponse>();

            emsp2EMSPAPI.HTTPLogger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request) => {
                                                              emsp2APIRequestLogs. TryAdd(Timestamp.Now, request);
                                                              return Task.CompletedTask;
                                                          });

            emsp2EMSPAPI.HTTPLogger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request, response) => {
                                                              emsp2APIResponseLogs.TryAdd(Timestamp.Now, response);
                                                              return Task.CompletedTask;
                                                          });

            emsp2EMSPAPI.HTTPLogger?.Debug("all", LogTargets.Debug);

            emsp2CommonAPI.BaseAPI.ClientConfigurations.Description = (remotePartyId) => I18NString.Create($"EMSP #2 Client for {remotePartyId}");

            #endregion

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public virtual async Task ShutdownEachTest()
        {

            if (cpoHTTPServer   is not null)
                await cpoHTTPServer.  Stop();

            if (emsp1HTTPServer is not null)
                await emsp1HTTPServer.Stop();

            if (emsp2HTTPServer is not null)
                await emsp2HTTPServer.Stop();


            if (cpoCommonAPI is not null)
                File.Delete(Path.Combine(cpoCommonAPI.  LoggingPath, cpoCommonAPI.  LogfileName));

            if (emsp1CommonAPI is not null)
                File.Delete(Path.Combine(emsp1CommonAPI.LoggingPath, emsp1CommonAPI.LogfileName));

            if (emsp2CommonAPI is not null)
                File.Delete(Path.Combine(emsp2CommonAPI.LoggingPath, emsp2CommonAPI.LogfileName));

        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public void ShutdownOnce()
        {

        }

        #endregion


    }

}
