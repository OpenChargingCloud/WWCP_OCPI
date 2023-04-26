﻿/*
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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPIv2_2_1.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests
{

    public static class TestHelpers
    {

        public static async Task<HTTPResponse<JObject>> JSONRequest(URL     RemoteURL,
                                                                    String  Token)
        {

            var httpResponse  = await new HTTPClient(RemoteURL).
                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     RemoteURL.Path,
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization  = new HTTPTokenAuthentication(Token);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      "1234");
                                                                                         requestbuilder.Set("X-Correlation-ID",  "5678");
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
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization  = new HTTPTokenAuthentication(Token);
                                                                                         requestbuilder.ContentType    = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content        = JSON.ToUTF8Bytes();
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      "1234");
                                                                                         requestbuilder.Set("X-Correlation-ID",  "5678");
                                                                                     })).
                                              ConfigureAwait(false);

            return new HTTPResponse<JObject>(httpResponse,
                                             JObject.Parse(httpResponse.HTTPBodyAsUTF8String));

        }


    }


    /// <summary>
    /// OCPI v2.2 Node test defaults.
    /// </summary>
    public abstract class ANodeTests
    {

        #region Data

        public     URL?             cpoVersionsAPIURL;
        public     URL?             emsp1VersionsAPIURL;
        public     URL?             emsp2VersionsAPIURL;

        protected  HTTPAPI?         cpoHTTPAPI;
        protected  CommonAPI?       cpoCommonAPI;
        protected  CPOAPI?          cpoCPOAPI;
        protected  OCPICSOAdapter?  cpoAdapter;

        protected  HTTPAPI?         emsp1HTTPAPI;
        protected  CommonAPI?       emsp1CommonAPI;
        protected  EMSPAPI?         emsp1EMSPAPI;
        protected  OCPIEMPAdapter?  emsp1Adapter;

        protected  HTTPAPI?         emsp2HTTPAPI;
        protected  CommonAPI?       emsp2CommonAPI;
        protected  EMSPAPI?         emsp2EMSPAPI;
        protected  OCPIEMPAdapter?  emsp2Adapter;

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
        public virtual void SetupEachTest()
        {

            Timestamp.Reset();

            #region Create cpo/emp1/emp2 HTTP API

            cpoHTTPAPI           = new HTTPAPI(
                                       HTTPServerPort:                      IPPort.Parse(3301),
                                       Autostart:                           true
                                   );

            emsp1HTTPAPI          = new HTTPAPI(
                                       HTTPServerPort:                      IPPort.Parse(3401),
                                       Autostart:                           true
                                   );

            emsp2HTTPAPI          = new HTTPAPI(
                                       HTTPServerPort:                      IPPort.Parse(3402),
                                       Autostart:                           true
                                   );

            Assert.IsNotNull(cpoHTTPAPI);
            Assert.IsNotNull(emsp1HTTPAPI);
            Assert.IsNotNull(emsp2HTTPAPI);

            #endregion

            #region Create cpo/emsp1/emsp2 OCPI Common API

            cpoVersionsAPIURL    = URL.Parse("http://127.0.0.1:3301/ocpi/v2.2/versions");

            cpoCommonAPI         = new CommonAPI(

                                       OurVersionsURL:                      cpoVersionsAPIURL.Value,
                                       OurCredentialRoles:                  new[] {
                                                                                new CredentialsRole(
                                                                                     CountryCode.Parse("DE"),
                                                                                     Party_Id.   Parse("GEF"),
                                                                                     Roles.      CPO,
                                                                                     new BusinessDetails(
                                                                                         "GraphDefined CSO Services",
                                                                                         URL.Parse("https://www.graphdefined.com/cso")
                                                                                     )
                                                                                )
                                                                            },
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

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2"),
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


            emsp1VersionsAPIURL  = URL.Parse("http://127.0.0.1:3401/ocpi/v2.2/versions");

            emsp1CommonAPI       = new CommonAPI(

                                       OurVersionsURL:                      emsp1VersionsAPIURL.Value,
                                       OurCredentialRoles:                  new[] {
                                                                                new CredentialsRole(
                                                                                     CountryCode.Parse("DE"),
                                                                                     Party_Id.   Parse("GDF"),
                                                                                     Roles.      EMSP,
                                                                                     new BusinessDetails(
                                                                                         "GraphDefined EMSP #1 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/emsp1")
                                                                                     )
                                                                                )
                                                                            },
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

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2"),
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


            emsp2VersionsAPIURL  = URL.Parse("http://127.0.0.1:3402/ocpi/v2.2/versions");

            emsp2CommonAPI       = new CommonAPI(

                                       OurVersionsURL:                      emsp2VersionsAPIURL.Value,
                                       OurCredentialRoles:                  new[] {
                                                                                new CredentialsRole(
                                                                                     CountryCode.Parse("DE"),
                                                                                     Party_Id.   Parse("GD2"),
                                                                                     Roles.      EMSP,
                                                                                     new BusinessDetails(
                                                                                         "GraphDefined EMSP #2 Services",
                                                                                         URL.Parse("https://www.graphdefined.com/emsp2")
                                                                                     )
                                                                                )
                                                                            },
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

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2"),
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

            Assert.IsNotNull(cpoVersionsAPIURL);
            Assert.IsNotNull(emsp1VersionsAPIURL);
            Assert.IsNotNull(emsp2VersionsAPIURL);

            Assert.IsNotNull(cpoCommonAPI);
            Assert.IsNotNull(emsp1CommonAPI);
            Assert.IsNotNull(emsp2CommonAPI);

            #endregion

            #region Create cpo CPO API / emsp1 EMP API / emsp2 EMP API

            cpoCPOAPI            = new CPOAPI(

                                       CommonAPI:                           cpoCommonAPI,
                                       DefaultCountryCode:                  cpoCommonAPI.OurCredentialRoles.First().CountryCode,
                                       DefaultPartyId:                      cpoCommonAPI.OurCredentialRoles.First().PartyId,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2/v2.2.1/cpo"),
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
                                       DefaultCountryCode:                  emsp1CommonAPI.OurCredentialRoles.First().CountryCode,
                                       DefaultPartyId:                      emsp1CommonAPI.OurCredentialRoles.First().PartyId,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2/v2.2.1/emsp"),
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
                                       DefaultCountryCode:                  emsp2CommonAPI.OurCredentialRoles.First().CountryCode,
                                       DefaultPartyId:                      emsp2CommonAPI.OurCredentialRoles.First().PartyId,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2/v2.2.1/emsp"),
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

            #region Add Remote Parties

            cpoCommonAPI.AddRemoteParty  (CountryCode:                 emsp1CommonAPI.OurCredentialRoles.First().CountryCode,
                                          PartyId:                     emsp1CommonAPI.OurCredentialRoles.First().PartyId,
                                          Role:                        Roles.EMSP,
                                          BusinessDetails:             emsp1CommonAPI.OurCredentialRoles.First().BusinessDetails,

                                          AccessToken:                 AccessToken.Parse("cso-2-emp1:token"),
                                          AccessStatus:                AccessStatus.ALLOWED,

                                          RemoteAccessToken:           AccessToken.Parse("emp1-2-cso:token"),
                                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.2/versions"),
                                          RemoteVersionIds:            null,
                                          AccessTokenBase64Encoding:   true,
                                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                                          PartyStatus:                 PartyStatus.ENABLED);

            cpoCommonAPI.AddRemoteParty  (CountryCode:                 emsp2CommonAPI.OurCredentialRoles.First().CountryCode,
                                          PartyId:                     emsp2CommonAPI.OurCredentialRoles.First().PartyId,
                                          Role:                        Roles.EMSP,
                                          BusinessDetails:             emsp2CommonAPI.OurCredentialRoles.First().BusinessDetails,
                                          AccessToken:                 AccessToken.Parse("cso-2-emp2:token"),
                                          AccessStatus:                AccessStatus.ALLOWED,
                                          RemoteAccessToken:           AccessToken.Parse("emp2-2-cso:token"),
                                          RemoteVersionsURL:           URL.Parse($"http://localhost:{emsp2HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.2/versions"),
                                          RemoteVersionIds:            null,
                                          AccessTokenBase64Encoding:   true,
                                          RemoteStatus:                RemoteAccessStatus.ONLINE,
                                          PartyStatus:                 PartyStatus.ENABLED);



            emsp1CommonAPI.AddRemoteParty(CountryCode:                 cpoCommonAPI.OurCredentialRoles.First().CountryCode,
                                          PartyId:                     cpoCommonAPI.OurCredentialRoles.First().PartyId,
                                          Role:                        Roles.CPO,
                                          BusinessDetails:             cpoCommonAPI.OurCredentialRoles.First().BusinessDetails,

                                          AccessToken:                 AccessToken.Parse("emp1-2-cso:token"),
                                          AccessStatus:                AccessStatus.ALLOWED,

                                          RemoteAccessToken:           AccessToken.Parse("cso-2-emp1:token"),
                                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpoHTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.2/versions"),
                                          RemoteVersionIds:            null,
                                          AccessTokenBase64Encoding:   true,
                                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                                          PartyStatus:                 PartyStatus.ENABLED);


            emsp2CommonAPI.AddRemoteParty(CountryCode:                 cpoCommonAPI.OurCredentialRoles.First().CountryCode,
                                          PartyId:                     cpoCommonAPI.OurCredentialRoles.First().PartyId,
                                          Role:                        Roles.CPO,
                                          BusinessDetails:             cpoCommonAPI.OurCredentialRoles.First().BusinessDetails,

                                          AccessToken:                 AccessToken.Parse("emp2-2-cso:token"),
                                          AccessStatus:                AccessStatus.ALLOWED,

                                          RemoteAccessToken:           AccessToken.Parse("cso-2-emp2:token"),
                                          RemoteVersionsURL:           URL.Parse($"http://localhost:{cpoHTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.2/versions"),
                                          RemoteVersionIds:            null,
                                          AccessTokenBase64Encoding:   true,
                                          RemoteStatus:                RemoteAccessStatus.ONLINE,

                                          PartyStatus:                 PartyStatus.ENABLED);


            Assert.AreEqual(2, cpoCommonAPI.  RemoteParties.Count());
            Assert.AreEqual(1, emsp1CommonAPI.RemoteParties.Count());
            Assert.AreEqual(1, emsp2CommonAPI.RemoteParties.Count());

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