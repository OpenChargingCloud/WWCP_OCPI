﻿/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv3_0.HTTP;
using System.Net.Http.Headers;
using cloud.charging.open.protocols.WWCP.Virtual;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.UnitTests
{

    public static class TestHelpers
    {

        public static async Task<HTTPResponse<JObject>> JSONRequest(URL     RemoteURL,
                                                                    String  Token)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     RemoteURL.Path,
                                                                                     RequestBuilder: requestBuilder => {
                                                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.ParseHTTPHeader(Token);
                                                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                         requestBuilder.Set("X-Request-ID",      "1234");
                                                                                         requestBuilder.Set("X-Correlation-ID",  "5678");
                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<JObject>(httpResponse,
                                             JObject.Parse(httpResponse.HTTPBodyAsUTF8String));

        }

        public static async Task<HTTPResponse<JObject>> JSONRequest(HTTPMethod  Method,
                                                                    URL         RemoteURL,
                                                                    String      Token,
                                                                    JObject     JSON)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(Method,
                                                                                     RemoteURL.Path,
                                                                                     RequestBuilder: requestBuilder => {
                                                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.ParseHTTPHeader(Token);
                                                                                         requestBuilder.ContentType    = HTTPContentType.Application.JSON_UTF8;
                                                                                         requestBuilder.Content        = JSON.ToUTF8Bytes();
                                                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                         requestBuilder.Set("X-Request-ID",      "1234");
                                                                                         requestBuilder.Set("X-Correlation-ID",  "5678");
                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<JObject>(httpResponse,
                                             JObject.Parse(httpResponse.HTTPBodyAsUTF8String));

        }


    }


    /// <summary>
    /// OCPI v3.0 test defaults for tests with 2 CPOs and 2 EMSPs.
    /// </summary>
    public abstract class A_2CPOs2EMSPs_TestDefaults
    {

        #region Data

        public     UInt16           cpo1TCPPort    = 3301;
        public     UInt16           cpo2TCPPort    = 3302;
        public     UInt16           emsp1TCPPort   = 3401;
        public     UInt16           emsp2TCPPort   = 3402;

        public     URL?             cpo1VersionsAPIURL;
        public     URL?             cpo2VersionsAPIURL;
        public     URL?             emsp1VersionsAPIURL;
        public     URL?             emsp2VersionsAPIURL;

        protected  CommonBaseAPI?   cpo1BaseAPI;
        protected  CommonBaseAPI?   cpo2BaseAPI;
        protected  CommonBaseAPI?   emsp1BaseAPI;
        protected  CommonBaseAPI?   emsp2BaseAPI;

        protected  HTTPAPI?         cpo1HTTPAPI;
        protected  CommonAPI?       cpo1CommonAPI;
        protected  CPOAPI?          cpo1CPOAPI;
        protected  OCPICSOAdapter?  cpo1Adapter;

        protected  HTTPAPI?         cpo2HTTPAPI;
        protected  CommonAPI?       cpo2CommonAPI;
        protected  CPOAPI?          cpo2CPOAPI;
        protected  OCPICSOAdapter?  cpo2Adapter;

        protected  HTTPAPI?         emsp1HTTPAPI;
        protected  CommonAPI?       emsp1CommonAPI;
        protected  EMSPAPI?         emsp1EMSPAPI;
        protected  OCPIEMPAdapter?  emsp1Adapter;

        protected  HTTPAPI?         emsp2HTTPAPI;
        protected  CommonAPI?       emsp2CommonAPI;
        protected  EMSPAPI?         emsp2EMSPAPI;
        protected  OCPIEMPAdapter?  emsp2Adapter;

        #endregion

        #region Constructor(s)

        public A_2CPOs2EMSPs_TestDefaults()
        {

            Timestamp.Reset();

        }

        #endregion


        #region SetupOCPINetwork()

        public async Task SetupOCPINetwork()
        {

            #region Create cpo1/cpo2/emsp1/emsp2 HTTP API

            cpo1HTTPAPI          = new HTTPAPI(
                                       HTTPServerPort:                      IPPort.Parse(cpo1TCPPort),
                                       AutoStart:                           true
                                   );

            cpo2HTTPAPI          = new HTTPAPI(
                                       HTTPServerPort:                      IPPort.Parse(cpo2TCPPort),
                                       AutoStart:                           true
                                   );

            emsp1HTTPAPI         = new HTTPAPI(
                                       HTTPServerPort:                      IPPort.Parse(emsp1TCPPort),
                                       AutoStart:                           true
                                   );

            emsp2HTTPAPI         = new HTTPAPI(
                                       HTTPServerPort:                      IPPort.Parse(emsp2TCPPort),
                                       AutoStart:                           true
                                   );

            Assert.That(cpo1HTTPAPI,   Is.Not.Null);
            Assert.That(cpo2HTTPAPI,   Is.Not.Null);
            Assert.That(emsp1HTTPAPI,  Is.Not.Null);
            Assert.That(emsp2HTTPAPI,  Is.Not.Null);

            #endregion

            #region Create cpo1/cpo2/emsp1/emsp2 OCPI Base APIs

            #region CPO #1

            cpo1BaseAPI = new CommonBaseAPI(

                               OurBaseURL:                URL.Parse("http://127.0.0.1:3201/ocpi"),
                               OurVersionsURL:            URL.Parse("http://127.0.0.1:3201/ocpi/versions"),

                               HTTPServer:                cpo1HTTPAPI.HTTPServer,
                               AdditionalURLPathPrefix:   null,
                               //KeepRemovedEVSEs:          null,
                               LocationsAsOpenData:       true,
                               AllowDowngrades:           null,
                               //Disable_RootServices:      false,

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

            #region CPO #2

            cpo2BaseAPI  = new CommonBaseAPI(

                               OurBaseURL:                URL.Parse("http://127.0.0.1:3202/ocpi"),
                               OurVersionsURL:            URL.Parse("http://127.0.0.1:3202/ocpi/versions"),

                               HTTPServer:                cpo2HTTPAPI.HTTPServer,
                               AdditionalURLPathPrefix:   null,
                               //KeepRemovedEVSEs:          null,
                               LocationsAsOpenData:       true,
                               AllowDowngrades:           null,
                               //Disable_RootServices:      false,

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
                               //Disable_RootServices:      false,

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
                               //Disable_RootServices:      false,

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

            Assert.That(cpo2BaseAPI,   Is.Not.Null);
            Assert.That(cpo2BaseAPI,   Is.Not.Null);
            Assert.That(emsp1BaseAPI,  Is.Not.Null);
            Assert.That(emsp2BaseAPI,  Is.Not.Null);

            #endregion

            #region Create CPO #1  Common/CPO API

            cpo1VersionsAPIURL   = URL.Parse($"http://127.0.0.1:{cpo1TCPPort}/ocpi/v3.0/versions");

            cpo1CommonAPI        = new CommonAPI(

                                       //OurBaseURL:                          URL.Parse($"http://127.0.0.1:{cpo1TCPPort}/ocpi/v3.0"),
                                       //OurVersionsURL:                      cpo1VersionsAPIURL.Value,
                                       OurCredentialRoles:                  [
                                                                                new CredentialsRole(
                                                                                     Party_Idv3.Parse("DEGEF"),
                                                                                     Role.CPO,
                                                                                     new BusinessDetails(
                                                                                         "GraphDefined CPO #1 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/cpo1")
                                                                                     )
                                                                                )
                                                                            ],
                                       DefaultPartyId:                      Party_Idv3.Parse("DEGEF"),
                                       BaseAPI:                             cpo1BaseAPI,
                                       //HTTPServer:                          cpo1HTTPAPI.HTTPServer,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,
                                       LocationsAsOpenData:                 true,
                                       AllowDowngrades:                     null,
                                       Disable_RootServices:                false,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v3.0"),
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

            cpo1CPOAPI            = new CPOAPI(

                                       CommonAPI:                           cpo1CommonAPI,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v3.0/cpo"),
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

            Assert.That(cpo1CommonAPI, Is.Not.Null);
            Assert.That(cpo1CPOAPI,    Is.Not.Null);

            #endregion

            #region Create CPO #2  Common/CPO API

            cpo2VersionsAPIURL   = URL.Parse($"http://127.0.0.1:{cpo2TCPPort}/ocpi/v3.0/versions");

            cpo2CommonAPI        = new CommonAPI(

                                       OurBaseURL:                          URL.Parse($"http://127.0.0.1:{cpo2TCPPort}/ocpi/v3.0"),
                                       OurVersionsURL:                      cpo1VersionsAPIURL.Value,
                                       OurCredentialRoles:                  [
                                                                                new CredentialsRole(
                                                                                     Party_Idv3.Parse("DEGE2"),
                                                                                     Role.CPO,
                                                                                     new BusinessDetails(
                                                                                         "GraphDefined CPO #2 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/cpo2")
                                                                                     )
                                                                                )
                                                                            ],
                                       DefaultPartyId:                      Party_Idv3.Parse("DEGE2"),
                                       BaseAPI:                             cpo2BaseAPI,
                                       //HTTPServer:                          cpo2HTTPAPI.HTTPServer,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,
                                       LocationsAsOpenData:                 true,
                                       AllowDowngrades:                     null,
                                       Disable_RootServices:                false,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v3.0"),
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

            cpo2CPOAPI            = new CPOAPI(

                                       CommonAPI:                           cpo2CommonAPI,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v3.0/cpo"),
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

            Assert.That(cpo2CommonAPI, Is.Not.Null);
            Assert.That(cpo2CPOAPI,    Is.Not.Null);

            #endregion

            #region Create EMSP #1 Common/EMSP API

            emsp1VersionsAPIURL  = URL.Parse($"http://127.0.0.1:{emsp1TCPPort}/ocpi/v3.0/versions");

            emsp1CommonAPI       = new CommonAPI(

                                       OurBaseURL:                          URL.Parse($"http://127.0.0.1:{emsp1TCPPort}/ocpi/v3.0"),
                                       OurVersionsURL:                      emsp1VersionsAPIURL.Value,
                                       OurCredentialRoles:                  [
                                                                                new CredentialsRole(
                                                                                     Party_Idv3.Parse("DEGDF"),
                                                                                     Role.EMSP,
                                                                                     new BusinessDetails(
                                                                                         "GraphDefined EMSP #1 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/emsp1")
                                                                                     )
                                                                                )
                                                                            ],
                                       DefaultPartyId:                      Party_Idv3.Parse("DEGDF"),
                                       BaseAPI:                             emsp1BaseAPI,
                                       //HTTPServer:                          emsp1HTTPAPI.HTTPServer,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,
                                       LocationsAsOpenData:                 true,
                                       AllowDowngrades:                     null,
                                       Disable_RootServices:                false,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v3.0"),
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

            emsp1EMSPAPI         = new EMSPAPI(

                                       CommonAPI:                           emsp1CommonAPI,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v3.0/emsp"),
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

            Assert.That(emsp1CommonAPI, Is.Not.Null);
            Assert.That(emsp1EMSPAPI,   Is.Not.Null);

            #endregion

            #region Create EMSP #2 Common/EMSP API

            emsp2VersionsAPIURL  = URL.Parse($"http://127.0.0.1:{emsp2TCPPort}/ocpi/v3.0/versions");

            emsp2CommonAPI       = new CommonAPI(

                                       OurBaseURL:                          URL.Parse($"http://127.0.0.1:{emsp2TCPPort}/ocpi/v3.0"),
                                       OurVersionsURL:                      emsp2VersionsAPIURL.Value,
                                       OurCredentialRoles:                  [
                                                                                new CredentialsRole(
                                                                                     Party_Idv3.Parse("DEGD2"),
                                                                                     Role.EMSP,
                                                                                     new BusinessDetails(
                                                                                         "GraphDefined EMSP #2 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/emsp2")
                                                                                     )
                                                                                )
                                                                            ],
                                       DefaultPartyId:                      Party_Idv3.Parse("DEGD2"),
                                       BaseAPI:                             emsp2BaseAPI,
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

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v3.0"),
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

            emsp2EMSPAPI         = new EMSPAPI(

                                       CommonAPI:                           emsp2CommonAPI,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v3.0/emsp"),
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

            Assert.That(emsp2CommonAPI, Is.Not.Null);
            Assert.That(emsp2EMSPAPI,   Is.Not.Null);

            #endregion


            #region Add CPO #1's  Remote Parties

            await cpo1CommonAPI.AddRemoteParty (Id:                          RemoteParty_Id.Parse(
                                                                                 emsp1CommonAPI.Parties.First().Id,
                                                                                 emsp1CommonAPI.Parties.First().Role
                                                                             ),
                                                CredentialsRoles:            [
                                                                                 new CredentialsRole(
                                                                                     PartyId:          emsp1CommonAPI.Parties.First().Id,
                                                                                     Role:             Role.EMSP,
                                                                                     BusinessDetails:  emsp1CommonAPI.Parties.First().BusinessDetails,
                                                                                     AllowDowngrades:  false
                                                                                 )
                                                                             ],

                                                AccessToken:                 AccessToken.Parse("cpo1-to-emsp1:token"),
                                                AccessStatus:                AccessStatus.ALLOWED,

                                                RemoteAccessToken:           AccessToken.Parse("emsp1-to-cpo1:token"),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v3.0/versions"),
                                                RemoteVersionIds:            null,
                                                AccessTokenBase64Encoding:   true,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                PartyStatus:                 PartyStatus.ENABLED);

            await cpo1CommonAPI.AddRemoteParty (Id:                          RemoteParty_Id.Parse(
                                                                                 emsp2CommonAPI.Parties.First().Id,
                                                                                 emsp2CommonAPI.Parties.First().Role
                                                                             ),
                                                CredentialsRoles:            [
                                                                                 new CredentialsRole(
                                                                                     PartyId:          emsp2CommonAPI.Parties.First().Id,
                                                                                     Role:             Role.EMSP,
                                                                                     BusinessDetails:  emsp2CommonAPI.Parties.First().BusinessDetails,
                                                                                     AllowDowngrades:  false
                                                                                 )
                                                                             ],

                                                AccessToken:                 AccessToken.Parse("cpo1-to-emsp2:token"),
                                                AccessStatus:                AccessStatus.ALLOWED,
                                                RemoteAccessToken:           AccessToken.Parse("emsp2-to-cpo1:token"),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp2HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v3.0/versions"),
                                                RemoteVersionIds:            null,
                                                AccessTokenBase64Encoding:   true,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,
                                                PartyStatus:                 PartyStatus.ENABLED);

            Assert.That(cpo1CommonAPI.RemoteParties.Count(), Is.EqualTo(2));

            #endregion

            #region Add CPO #2's  Remote Parties

            await cpo2CommonAPI.AddRemoteParty (Id:                          RemoteParty_Id.Parse(
                                                                                 emsp1CommonAPI.Parties.First().Id,
                                                                                 emsp1CommonAPI.Parties.First().Role
                                                                             ),
                                                CredentialsRoles:            [
                                                                                 new CredentialsRole(
                                                                                     PartyId:          emsp1CommonAPI.Parties.First().Id,
                                                                                     Role:             Role.EMSP,
                                                                                     BusinessDetails:  emsp1CommonAPI.Parties.First().BusinessDetails,
                                                                                     AllowDowngrades:  false
                                                                                 )
                                                                             ],

                                                AccessToken:                 AccessToken.Parse("cpo2-to-emsp1:token"),
                                                AccessStatus:                AccessStatus.ALLOWED,

                                                RemoteAccessToken:           AccessToken.Parse("emsp1-to-cpo2:token"),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1TCPPort}/ocpi/v3.0/versions"),
                                                RemoteVersionIds:            null,
                                                AccessTokenBase64Encoding:   true,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                PartyStatus:                 PartyStatus.ENABLED);

            await cpo2CommonAPI.AddRemoteParty (Id:                          RemoteParty_Id.Parse(
                                                                                 emsp2CommonAPI.Parties.First().Id,
                                                                                 emsp2CommonAPI.Parties.First().Role
                                                                             ),
                                                CredentialsRoles:            [
                                                                                 new CredentialsRole(
                                                                                     PartyId:          emsp2CommonAPI.Parties.First().Id,
                                                                                     Role:             Role.EMSP,
                                                                                     BusinessDetails:  emsp2CommonAPI.Parties.First().BusinessDetails,
                                                                                     AllowDowngrades:  false
                                                                                 )
                                                                             ],

                                                AccessToken:                 AccessToken.Parse("cpo2-to-emsp2:token"),
                                                AccessStatus:                AccessStatus.ALLOWED,
                                                RemoteAccessToken:           AccessToken.Parse("emsp2-to-cpo2:token"),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp2TCPPort}/ocpi/v3.0/versions"),
                                                RemoteVersionIds:            null,
                                                AccessTokenBase64Encoding:   true,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,
                                                PartyStatus:                 PartyStatus.ENABLED);

            Assert.That(cpo2CommonAPI.RemoteParties.Count(), Is.EqualTo(2));

            #endregion

            #region Add EMSP #1's Remote Parties

            await emsp1CommonAPI.AddRemoteParty(Id:                          RemoteParty_Id.Parse(
                                                                                 cpo1CommonAPI.Parties.First().Id,
                                                                                 cpo1CommonAPI.Parties.First().Role
                                                                             ),
                                                CredentialsRoles:            [
                                                                                 new CredentialsRole(
                                                                                     PartyId:          cpo1CommonAPI.Parties.First().Id,
                                                                                     Role:             Role.CPO,
                                                                                     BusinessDetails:  cpo1CommonAPI.Parties.First().BusinessDetails,
                                                                                     AllowDowngrades:  false
                                                                                 )
                                                                             ],

                                                AccessToken:                 AccessToken.Parse("emsp1-to-cpo1:token"),
                                                AccessStatus:                AccessStatus.ALLOWED,

                                                RemoteAccessToken:           AccessToken.Parse("cpo1-to-emsp1:token"),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1TCPPort}/ocpi/v3.0/versions"),
                                                RemoteVersionIds:            null,
                                                AccessTokenBase64Encoding:   true,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                PartyStatus:                 PartyStatus.ENABLED);

            await emsp1CommonAPI.AddRemoteParty(Id:                          RemoteParty_Id.Parse(
                                                                                 cpo2CommonAPI.Parties.First().Id,
                                                                                 cpo2CommonAPI.Parties.First().Role
                                                                             ),
                                                CredentialsRoles:            [
                                                                                 new CredentialsRole(
                                                                                     PartyId:          cpo2CommonAPI.Parties.First().Id,
                                                                                     Role:             Role.CPO,
                                                                                     BusinessDetails:  cpo2CommonAPI.Parties.First().BusinessDetails,
                                                                                     AllowDowngrades:  false
                                                                                 )
                                                                             ],

                                                AccessToken:                 AccessToken.Parse("emsp1-to-cpo2:token"),
                                                AccessStatus:                AccessStatus.ALLOWED,

                                                RemoteAccessToken:           AccessToken.Parse("cpo2-to-emsp1:token"),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo2TCPPort}/ocpi/v3.0/versions"),
                                                RemoteVersionIds:            null,
                                                AccessTokenBase64Encoding:   true,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                PartyStatus:                 PartyStatus.ENABLED);

            Assert.That(emsp1CommonAPI.RemoteParties.Count(), Is.EqualTo(2));

            #endregion

            #region Add EMSP #1's Remote Parties

            await emsp2CommonAPI.AddRemoteParty(Id:                          RemoteParty_Id.Parse(
                                                                                 cpo1CommonAPI.Parties.First().Id,
                                                                                 cpo1CommonAPI.Parties.First().Role
                                                                             ),
                                                CredentialsRoles:            [
                                                                                 new CredentialsRole(
                                                                                     PartyId:          cpo1CommonAPI.Parties.First().Id,
                                                                                     Role:             Role.CPO,
                                                                                     BusinessDetails:  cpo1CommonAPI.Parties.First().BusinessDetails,
                                                                                     AllowDowngrades:  false
                                                                                 )
                                                                             ],

                                                AccessToken:                 AccessToken.Parse("emsp2-to-cpo1:token"),
                                                AccessStatus:                AccessStatus.ALLOWED,

                                                RemoteAccessToken:           AccessToken.Parse("cpo1-to-emsp2:token"),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo1TCPPort}/ocpi/v3.0/versions"),
                                                RemoteVersionIds:            null,
                                                AccessTokenBase64Encoding:   true,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                PartyStatus:                 PartyStatus.ENABLED);

            await emsp2CommonAPI.AddRemoteParty(Id:                          RemoteParty_Id.Parse(
                                                                                 cpo2CommonAPI.Parties.First().Id,
                                                                                 cpo2CommonAPI.Parties.First().Role
                                                                             ),
                                                CredentialsRoles:            [
                                                                                 new CredentialsRole(
                                                                                     PartyId:          cpo2CommonAPI.Parties.First().Id,
                                                                                     Role:             Role.CPO,
                                                                                     BusinessDetails:  cpo2CommonAPI.Parties.First().BusinessDetails,
                                                                                     AllowDowngrades:  false
                                                                                 )
                                                                             ],

                                                AccessToken:                 AccessToken.Parse("emsp2-to-cpo2:token"),
                                                AccessStatus:                AccessStatus.ALLOWED,

                                                RemoteAccessToken:           AccessToken.Parse("cpo2-to-emsp2:token"),
                                                RemoteVersionsURL:           URL.Parse($"http://localhost:{cpo2TCPPort}/ocpi/v3.0/versions"),
                                                RemoteVersionIds:            null,
                                                AccessTokenBase64Encoding:   true,
                                                RemoteStatus:                RemoteAccessStatus.ONLINE,

                                                PartyStatus:                 PartyStatus.ENABLED);

            Assert.That(emsp2CommonAPI.RemoteParties.Count(), Is.EqualTo(2));

            #endregion


        }

        #endregion


        #region SetupOnce()

        [OneTimeSetUp]
        public virtual async Task SetupOnce()
        {

            await SetupOCPINetwork();

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public virtual async Task SetupEachTest()
        {

            

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public virtual async Task ShutdownEachTest()
        {

            

        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public virtual async Task ShutdownOnce()
        {

            cpo1HTTPAPI? .Shutdown();
            cpo2HTTPAPI?. Shutdown();
            emsp1HTTPAPI?.Shutdown();
            emsp2HTTPAPI?.Shutdown();

        }

        #endregion


    }

}
