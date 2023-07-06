/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
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
using cloud.charging.open.protocols.OCPIv2_1_1.HTTP;
using cloud.charging.open.protocols.OCPIv2_1_1.WebAPI;

using social.OpenData.UsersAPI;

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
                                                                requestbuilder => {

                                                                    if (Token is not null && Token.IsNotNullOrEmpty())
                                                                        requestbuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                    requestbuilder.Set("X-Request-ID",      "1234");
                                                                    requestbuilder.Set("X-Correlation-ID",  "5678");

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
                                                                                     requestbuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestbuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      "1234");
                                                                                         requestbuilder.Set("X-Correlation-ID",  "5678");

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
                                                                                     requestbuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestbuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      "1234");
                                                                                         requestbuilder.Set("X-Correlation-ID",  "5678");

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
                                                                                     requestbuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestbuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestbuilder.Accept.Add(HTTPContentType.HTML_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      "1234");
                                                                                         requestbuilder.Set("X-Correlation-ID",  "5678");

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
                                                                                     requestbuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestbuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestbuilder.Accept.Add(HTTPContentType.TEXT_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      "1234");
                                                                                         requestbuilder.Set("X-Correlation-ID",  "5678");

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
                                                                                     requestbuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestbuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      "1234");
                                                                                         requestbuilder.Set("X-Correlation-ID",  "5678");

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
                                                                                     requestbuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestbuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestbuilder.ContentType    = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content        = JSON.ToUTF8Bytes();
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      "1234");
                                                                                         requestbuilder.Set("X-Correlation-ID",  "5678");

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
                                                                                     requestbuilder => {

                                                                                         if (Token is not null && Token.IsNotNullOrEmpty())
                                                                                             requestbuilder.Authorization = HTTPTokenAuthentication.Parse(Token);

                                                                                         requestbuilder.ContentType    = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content        = JSON.ToUTF8Bytes();
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      "1234");
                                                                                         requestbuilder.Set("X-Correlation-ID",  "5678");

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

        public          URL?                                           cpoVersionsAPIURL;
        public          URL?                                           emsp1VersionsAPIURL;
        public          URL?                                           emsp2VersionsAPIURL;

        protected       HTTPAPI?                                       cpoHTTPAPI;
        protected       CommonAPI?                                     cpoCommonAPI;
        protected       OCPIWebAPI?                                    cpoWebAPI;
        protected       CPOAPI?                                        cpoCPOAPI;
        protected       OCPICSOAdapter?                                cpoAdapter;
        protected       ConcurrentDictionary<DateTime, OCPIRequest>    cpoAPIRequestLogs;
        protected       ConcurrentDictionary<DateTime, OCPIResponse>   cpoAPIResponseLogs;

        protected       HTTPAPI?                                       emsp1HTTPAPI;
        protected       CommonAPI?                                     emsp1CommonAPI;
        protected       OCPIWebAPI?                                    emsp1WebAPI;
        protected       EMSPAPI?                                       emsp1EMSPAPI;
        protected       OCPIEMPAdapter?                                emsp1Adapter;
        protected       ConcurrentDictionary<DateTime, OCPIRequest>    emsp1APIRequestLogs;
        protected       ConcurrentDictionary<DateTime, OCPIResponse>   emsp1APIResponseLogs;

        protected       HTTPAPI?                                       emsp2HTTPAPI;
        protected       CommonAPI?                                     emsp2CommonAPI;
        protected       OCPIWebAPI?                                    emsp2WebAPI;
        protected       EMSPAPI?                                       emsp2EMSPAPI;
        protected       OCPIEMPAdapter?                                emsp2Adapter;
        protected       ConcurrentDictionary<DateTime, OCPIRequest>    emsp2APIRequestLogs;
        protected       ConcurrentDictionary<DateTime, OCPIResponse>   emsp2APIResponseLogs;

        protected const String                                         cpo_accessing_emsp1__token    = "cpo_accessing_emsp1++token";
        protected const String                                         cpo_accessing_emsp2__token    = "cpo_accessing_emsp2++token";

        protected const String                                         emsp1_accessing_cpo__token    = "emsp1_accessing_cpo++token";
        protected const String                                         emsp2_accessing_cpo__token    = "emsp2_accessing_cpo++token";

        protected const String                                         UnknownToken                  = "UnknownUnknownUnknownToken";

        protected const String                                         BlockedCPOToken               = "blocked-cpo";
        protected const String                                         BlockedEMSPToken              = "blocked-emsp";

        //protected readonly Dictionary<Operator_Id, HashSet<EVSEDataRecord>>            EVSEDataRecords;
        //protected readonly Dictionary<Operator_Id, HashSet<EVSEStatusRecord>>          EVSEStatusRecords;
        //protected readonly Dictionary<Operator_Id, HashSet<PricingProductDataRecord>>  PricingProductData;
        //protected readonly Dictionary<Operator_Id, HashSet<EVSEPricing>>               EVSEPricings;
        //protected readonly Dictionary<Operator_Id, HashSet<ChargeDetailRecord>>        ChargeDetailRecords;

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

            #region Create cpo/emsp1/emsp2 HTTP API

            cpoHTTPAPI            = new HTTPAPI(
                                        HTTPServerPort:  IPPort.Parse(3301),
                                        Autostart:       true
                                    );

            emsp1HTTPAPI          = new HTTPAPI(
                                        HTTPServerPort:  IPPort.Parse(3401),
                                        Autostart:       true
                                    );

            emsp2HTTPAPI          = new HTTPAPI(
                                        HTTPServerPort:  IPPort.Parse(3402),
                                        Autostart:       true
                                    );

            Assert.IsNotNull(cpoHTTPAPI);
            Assert.IsNotNull(emsp1HTTPAPI);
            Assert.IsNotNull(emsp2HTTPAPI);

            #endregion

            #region Create cpo/emsp1/emsp2 OCPI Common API

            // Clean up log and databade directories...
            foreach (var filePath in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory,
                                                                     HTTPAPI.DefaultHTTPAPI_LoggingPath),
                                                        $"GraphDefined_OCPI{Version.String}_*.log"))
            {
                File.Delete(filePath);
            }


            cpoVersionsAPIURL    = URL.Parse("http://127.0.0.1:3301/ocpi/v2.1/versions");

            cpoCommonAPI         = new CommonAPI(

                                       OurBaseURL:                          URL.Parse("http://127.0.0.1:3301/ocpi/v2.1"),
                                       OurVersionsURL:                      cpoVersionsAPIURL.Value,
                                       OurBusinessDetails:                  new BusinessDetails(
                                                                                "GraphDefined CSO Services",
                                                                                URL.Parse("https://www.graphdefined.com/cso")
                                                                            ),
                                       OurCountryCode:                      CountryCode.Parse("DE"),
                                       OurPartyId:                          Party_Id.   Parse("GEF"),
                                       OurRole:                             Roles.      CPO,

                                       HTTPServer:                          cpoHTTPAPI.HTTPServer,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,
                                       LocationsAsOpenData:                 true,
                                       AllowDowngrades:                     null,
                                       Disable_RootServices:                false,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1"),
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
                                       LogfileName:                         $"GraphDefined_OCPI{Version.String}_CSO.log",
                                       LogfileCreator:                      null,
                                       DatabaseFilePath:                    null,
                                       RemotePartyDBFileName:               $"GraphDefined_OCPI{Version.String}_RemoteParties_CPO.log",
                                       AssetsDBFileName:                    $"GraphDefined_OCPI{Version.String}_Assets_CPO.log",
                                       Autostart:                           false

                                   );


            emsp1VersionsAPIURL  = URL.Parse("http://127.0.0.1:3401/ocpi/v2.1/versions");

            emsp1CommonAPI       = new CommonAPI(

                                       OurBaseURL:                          URL.Parse("http://127.0.0.1:3401/ocpi/v2.1"),
                                       OurVersionsURL:                      emsp1VersionsAPIURL.Value,
                                       OurBusinessDetails:                  new BusinessDetails(
                                                                                "GraphDefined EMSP #1 Services",
                                                                                URL.Parse("https://www.graphdefined.com/emsp1")
                                                                            ),
                                       OurCountryCode:                      CountryCode.Parse("DE"),
                                       OurPartyId:                          Party_Id.   Parse("GDF"),
                                       OurRole:                             Roles.      EMSP,

                                       HTTPServer:                          emsp1HTTPAPI.HTTPServer,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,
                                       LocationsAsOpenData:                 true,
                                       AllowDowngrades:                     null,
                                       Disable_RootServices:                false,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1"),
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
                                       LogfileName:                         $"GraphDefined_OCPI{Version.String}_EMSP1.log",
                                       LogfileCreator:                      null,
                                       DatabaseFilePath:                    null,
                                       RemotePartyDBFileName:               $"GraphDefined_OCPI{Version.String}_RemoteParties_EMSP1.log",
                                       AssetsDBFileName:                    $"GraphDefined_OCPI{Version.String}_Assets_EMSP1.log",
                                       Autostart:                           false

                                   );


            emsp2VersionsAPIURL  = URL.Parse("http://127.0.0.1:3402/ocpi/v2.1/versions");

            emsp2CommonAPI       = new CommonAPI(

                                       OurBaseURL:                          URL.Parse("http://127.0.0.1:3402/ocpi/v2.1"),
                                       OurVersionsURL:                      emsp2VersionsAPIURL.Value,
                                       OurBusinessDetails:                  new BusinessDetails(
                                                                                "GraphDefined EMSP #2 Services",
                                                                                URL.Parse("https://www.graphdefined.com/emsp2")
                                                                            ),
                                       OurCountryCode:                      CountryCode.Parse("DE"),
                                       OurPartyId:                          Party_Id.   Parse("GD2"),
                                       OurRole:                             Roles.      EMSP,

                                       HTTPServer:                          emsp2HTTPAPI.HTTPServer,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,
                                       LocationsAsOpenData:                 true,
                                       AllowDowngrades:                     null,
                                       Disable_RootServices:                false,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1"),
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
                                       LogfileName:                         $"GraphDefined_OCPI{Version.String}_EMSP2.log",
                                       LogfileCreator:                      null,
                                       DatabaseFilePath:                    null,
                                       RemotePartyDBFileName:               $"GraphDefined_OCPI{Version.String}_RemoteParties_EMSP2.log",
                                       AssetsDBFileName:                    $"GraphDefined_OCPI{Version.String}_Assets_EMSP2.log",
                                       Autostart:                           false

                                   );

            Assert.IsNotNull(cpoVersionsAPIURL);
            Assert.IsNotNull(emsp1VersionsAPIURL);
            Assert.IsNotNull(emsp2VersionsAPIURL);

            Assert.IsNotNull(cpoCommonAPI);
            Assert.IsNotNull(emsp1CommonAPI);
            Assert.IsNotNull(emsp2CommonAPI);

            #endregion

            #region Create cpo/emsp1/emsp2 OCPI WebAPI

            cpoWebAPI            = new OCPIWebAPI(
                                       HTTPServer:                          cpoHTTPAPI.HTTPServer,
                                       CommonAPI:                           cpoCommonAPI,
                                       OverlayURLPathPrefix:                HTTPPath.Parse("/ocpi/v2.1"),
                                       WebAPIURLPathPrefix:                 HTTPPath.Parse("/ocpi/v2.1/webapi"),
                                       BasePath:                            HTTPPath.Parse("/ocpi/v2.1"),
                                       HTTPRealm:                           "GraphDefined OCPI CPO WebAPI",
                                       HTTPLogins:                          new[] {
                                                                                new KeyValuePair<String, String>("a", "b")
                                                                            },
                                       HTMLTemplate:                        null,
                                       RequestTimeout:                      null
                                   );

            emsp1WebAPI          = new OCPIWebAPI(
                                       HTTPServer:                          emsp1HTTPAPI.HTTPServer,
                                       CommonAPI:                           emsp1CommonAPI,
                                       OverlayURLPathPrefix:                HTTPPath.Parse("/ocpi/v2.1"),
                                       WebAPIURLPathPrefix:                 HTTPPath.Parse("/ocpi/v2.1/webapi"),
                                       BasePath:                            HTTPPath.Parse("/ocpi/v2.1"),
                                       HTTPRealm:                           "GraphDefined OCPI EMSP #1 WebAPI",
                                       HTTPLogins:                          new[] {
                                                                                new KeyValuePair<String, String>("c", "d")
                                                                            },
                                       HTMLTemplate:                        null,
                                       RequestTimeout:                      null
                                   );

            emsp2WebAPI          = new OCPIWebAPI(
                                       HTTPServer:                          emsp2HTTPAPI.HTTPServer,
                                       CommonAPI:                           emsp2CommonAPI,
                                       OverlayURLPathPrefix:                HTTPPath.Parse("/ocpi/v2.1"),
                                       WebAPIURLPathPrefix:                 HTTPPath.Parse("/ocpi/v2.1/webapi"),
                                       BasePath:                            HTTPPath.Parse("/ocpi/v2.1"),
                                       HTTPRealm:                           "GraphDefined OCPI EMSP #2 WebAPI",
                                       HTTPLogins:                          new[] {
                                                                                new KeyValuePair<String, String>("e", "f")
                                                                            },
                                       HTMLTemplate:                        null,
                                       RequestTimeout:                      null
                                   );

            Assert.IsNotNull(cpoWebAPI);
            Assert.IsNotNull(emsp1WebAPI);
            Assert.IsNotNull(emsp2WebAPI);

            #endregion

            #region Create cpo CPOAPI & emsp1/emsp2 EMPAPI

            cpoCPOAPI            = new CPOAPI(

                                       CommonAPI:                           cpoCommonAPI,
                                       DefaultCountryCode:                  cpoCommonAPI.OurCountryCode,
                                       DefaultPartyId:                      cpoCommonAPI.OurPartyId,
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
                                       Autostart:                           false

                                   );

            emsp1EMSPAPI         = new EMSPAPI(

                                       CommonAPI:                           emsp1CommonAPI,
                                       DefaultCountryCode:                  emsp1CommonAPI.OurCountryCode,
                                       DefaultPartyId:                      emsp1CommonAPI.OurPartyId,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1/v2.1.1/emsp"),
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
                                       Autostart:                           false

                                   );

            emsp2EMSPAPI         = new EMSPAPI(

                                       CommonAPI:                           emsp2CommonAPI,
                                       DefaultCountryCode:                  emsp2CommonAPI.OurCountryCode,
                                       DefaultPartyId:                      emsp2CommonAPI.OurPartyId,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1/v2.1.1/emsp"),
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
                                       Autostart:                           false

                                   );

            Assert.IsNotNull(cpoCPOAPI);
            Assert.IsNotNull(emsp1EMSPAPI);
            Assert.IsNotNull(emsp2EMSPAPI);

            #endregion

            #region Define and connect Remote Parties

            await cpoCommonAPI.AddRemoteParty  (CountryCode:                 emsp1CommonAPI.OurCountryCode,
                                                PartyId:                     emsp1CommonAPI.OurPartyId,
                                                Role:                        Roles.EMSP,
                                                BusinessDetails:             emsp1CommonAPI.OurBusinessDetails,

                                                AccessToken:                 AccessToken.Parse(emsp1_accessing_cpo__token),
                                                AccessStatus:                AccessStatus.ALLOWED,

                                                RemoteAccessToken:           AccessToken.Parse(cpo_accessing_emsp1__token),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.1/versions"),
                                                RemoteVersionIds:            new[] { Version.Id },
                                                SelectedVersionId:           Version.Id,
                                                AccessTokenBase64Encoding:   false,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                PartyStatus:                 PartyStatus.ENABLED);

            await cpoCommonAPI.AddRemoteParty  (CountryCode:                 emsp2CommonAPI.OurCountryCode,
                                                PartyId:                     emsp2CommonAPI.OurPartyId,
                                                Role:                        Roles.EMSP,
                                                BusinessDetails:             emsp2CommonAPI.OurBusinessDetails,
                                                AccessToken:                 AccessToken.Parse(emsp2_accessing_cpo__token),
                                                AccessStatus:                AccessStatus.ALLOWED,
                                                RemoteAccessToken:           AccessToken.Parse(cpo_accessing_emsp2__token),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp2HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.1/versions"),
                                                RemoteVersionIds:            new[] { Version.Id },
                                                SelectedVersionId:           Version.Id,
                                                AccessTokenBase64Encoding:   false,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,
                                                PartyStatus:                 PartyStatus.ENABLED);



            await emsp1CommonAPI.AddRemoteParty(CountryCode:                 cpoCommonAPI.OurCountryCode,
                                                PartyId:                     cpoCommonAPI.OurPartyId,
                                                Role:                        Roles.CPO,
                                                BusinessDetails:             cpoCommonAPI.OurBusinessDetails,

                                                AccessToken:                 AccessToken.Parse(cpo_accessing_emsp1__token),
                                                AccessStatus:                AccessStatus.ALLOWED,

                                                RemoteAccessToken:           AccessToken.Parse(emsp1_accessing_cpo__token),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{cpoHTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.1/versions"),
                                                RemoteVersionIds:            new[] { Version.Id },
                                                SelectedVersionId:           Version.Id,
                                                AccessTokenBase64Encoding:   false,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                PartyStatus:                 PartyStatus.ENABLED);


            await emsp2CommonAPI.AddRemoteParty(CountryCode:                 cpoCommonAPI.OurCountryCode,
                                                PartyId:                     cpoCommonAPI.OurPartyId,
                                                Role:                        Roles.CPO,
                                                BusinessDetails:             cpoCommonAPI.OurBusinessDetails,

                                                AccessToken:                 AccessToken.Parse(cpo_accessing_emsp2__token),
                                                AccessStatus:                AccessStatus.ALLOWED,

                                                RemoteAccessToken:           AccessToken.Parse(emsp2_accessing_cpo__token),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{cpoHTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.1/versions"),
                                                RemoteVersionIds:            new[] { Version.Id },
                                                SelectedVersionId:           Version.Id,
                                                AccessTokenBase64Encoding:   false,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                PartyStatus:                 PartyStatus.ENABLED);


            Assert.AreEqual(2, cpoCommonAPI.  RemoteParties.Count());
            Assert.AreEqual(1, emsp1CommonAPI.RemoteParties.Count());
            Assert.AreEqual(1, emsp2CommonAPI.RemoteParties.Count());

            Assert.AreEqual(2, File.ReadAllLines(cpoCommonAPI.  RemotePartyDBFileName).Length);
            Assert.AreEqual(1, File.ReadAllLines(emsp1CommonAPI.RemotePartyDBFileName).Length);
            Assert.AreEqual(1, File.ReadAllLines(emsp2CommonAPI.RemotePartyDBFileName).Length);

            #endregion

            #region Define blocked Remote Parties

            await cpoCommonAPI.AddRemoteParty  (CountryCode:       CountryCode.Parse("XX"),
                                                PartyId:           Party_Id.   Parse("BLE"),
                                                Role:              Roles.EMSP,
                                                BusinessDetails:   new BusinessDetails(
                                                                       "Blocked EMSP"
                                                                   ),
                                                AccessToken:       AccessToken.Parse(BlockedEMSPToken),
                                                AccessStatus:      AccessStatus.BLOCKED,
                                                PartyStatus:       PartyStatus. ENABLED);

            await emsp1CommonAPI.AddRemoteParty(CountryCode:       CountryCode.Parse("XX"),
                                                PartyId:           Party_Id.   Parse("BLC"),
                                                Role:              Roles.CPO,
                                                BusinessDetails:   new BusinessDetails(
                                                                       "Blocked CPO"
                                                                   ),
                                                AccessToken:       AccessToken.Parse(BlockedCPOToken),
                                                AccessStatus:      AccessStatus.BLOCKED,
                                                PartyStatus:       PartyStatus. ENABLED);

            await emsp2CommonAPI.AddRemoteParty(CountryCode:       CountryCode.Parse("XX"),
                                                PartyId:           Party_Id.   Parse("BLC"),
                                                Role:              Roles.CPO,
                                                BusinessDetails:   new BusinessDetails(
                                                                       "Blocked CPO"
                                                                   ),
                                                AccessToken:       AccessToken.Parse(BlockedCPOToken),
                                                AccessStatus:      AccessStatus.BLOCKED,
                                                PartyStatus:       PartyStatus. ENABLED);


            Assert.AreEqual(3, cpoCommonAPI.  RemoteParties.Count());
            Assert.AreEqual(2, emsp1CommonAPI.RemoteParties.Count());
            Assert.AreEqual(2, emsp2CommonAPI.RemoteParties.Count());

            #endregion

            #region Defined API loggers

            // CPO
            cpoAPIRequestLogs     = new ConcurrentDictionary<DateTime, OCPIRequest>();
            cpoAPIResponseLogs    = new ConcurrentDictionary<DateTime, OCPIResponse>();

            cpoCPOAPI.CPOAPILogger?.RegisterLogTarget(LogTargets.Debug,
                                                      (loggingPath, context, logEventName, request) => {
                                                          cpoAPIRequestLogs. TryAdd(Timestamp.Now, request);
                                                          return Task.CompletedTask;
                                                      });

            cpoCPOAPI.CPOAPILogger?.RegisterLogTarget(LogTargets.Debug,
                                                      (loggingPath, context, logEventName, request, response) => {
                                                          cpoAPIResponseLogs.TryAdd(Timestamp.Now, response);
                                                          return Task.CompletedTask;
                                                      });

            cpoCPOAPI.CPOAPILogger?.Debug("all", LogTargets.Debug);

            cpoCommonAPI.ClientConfigurations.Description = (remotePartyId) => $"CPO Client for {remotePartyId}";



            // EMSP #1
            emsp1APIRequestLogs   = new ConcurrentDictionary<DateTime, OCPIRequest>();
            emsp1APIResponseLogs  = new ConcurrentDictionary<DateTime, OCPIResponse>();

            emsp1EMSPAPI.EMSPAPILogger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request) => {
                                                              emsp1APIRequestLogs. TryAdd(Timestamp.Now, request);
                                                              return Task.CompletedTask;
                                                          });

            emsp1EMSPAPI.EMSPAPILogger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request, response) => {
                                                              emsp1APIResponseLogs.TryAdd(Timestamp.Now, response);
                                                              return Task.CompletedTask;
                                                          });

            emsp1EMSPAPI.EMSPAPILogger?.Debug("all", LogTargets.Debug);

            emsp1CommonAPI.ClientConfigurations.Description = (remotePartyId) => $"EMSP #1 Client for {remotePartyId}";



            // EMSP #2
            emsp2APIRequestLogs   = new ConcurrentDictionary<DateTime, OCPIRequest>();
            emsp2APIResponseLogs  = new ConcurrentDictionary<DateTime, OCPIResponse>();

            emsp2EMSPAPI.EMSPAPILogger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request) => {
                                                              emsp2APIRequestLogs. TryAdd(Timestamp.Now, request);
                                                              return Task.CompletedTask;
                                                          });

            emsp2EMSPAPI.EMSPAPILogger?.RegisterLogTarget(LogTargets.Debug,
                                                          (loggingPath, context, logEventName, request, response) => {
                                                              emsp2APIResponseLogs.TryAdd(Timestamp.Now, response);
                                                              return Task.CompletedTask;
                                                          });

            emsp2EMSPAPI.EMSPAPILogger?.Debug("all", LogTargets.Debug);

            emsp2CommonAPI.ClientConfigurations.Description = (remotePartyId) => $"EMSP #2 Client for {remotePartyId}";

            #endregion

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public virtual void ShutdownEachTest()
        {

            cpoHTTPAPI?.  Shutdown();
            emsp1HTTPAPI?.Shutdown();
            emsp2HTTPAPI?.Shutdown();

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
