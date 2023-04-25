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

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPIv2_2_1.HTTP;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.Networking;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests
{

    /// <summary>
    /// Roaming test defaults for a charging station operator connected
    /// to two e-mobility providers via OCPI v2.2.1.
    /// </summary>
    public abstract class ARoamingTests
    {

        #region Data

        protected  RoamingNetwork?            csoRoamingNetwork;
        protected  HTTPAPI?                   csoHTTPAPI;
        protected  CommonAPI?                 csoCommonAPI;
        protected  CPOAPI?                    csoCPOAPI;
        protected  OCPICSOAdapter?            csoAdapter;
        protected  IChargingStationOperator?  graphDefinedCSO;

        protected  RoamingNetwork?            emp1RoamingNetwork;
        protected  HTTPAPI?                   emp1HTTPAPI;
        protected  CommonAPI?                 emp1CommonAPI;
        protected  EMSPAPI?                   emp1EMSPAPI;
        protected  OCPIEMPAdapter?            emp1Adapter;
        protected  IEMobilityProvider?        graphDefinedEMP;
        protected  IEMobilityProvider?        graphDefinedEMP_remote;

        protected  RoamingNetwork?            emp2RoamingNetwork;
        protected  HTTPAPI?                   emp2HTTPAPI;
        protected  CommonAPI?                 emp2CommonAPI;
        protected  EMSPAPI?                   emp2EMSPAPI;
        protected  OCPIEMPAdapter?            emp2Adapter;
        protected  IEMobilityProvider?        exampleEMP;
        protected  IEMobilityProvider?        exampleEMP_remote;

        #endregion

        #region Constructor(s)

        public ARoamingTests()
        {

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
        public void SetupEachTest()
        {

            Timestamp.Reset();

            #region Create cso/emp1/emp2 roaming network

            csoRoamingNetwork = new RoamingNetwork(
                                       Id:                                  RoamingNetwork_Id.Parse("test_cso"),
                                       Name:                                I18NString.Create(Languages.en, "CSO EV Roaming Test Network"),
                                       Description:                         I18NString.Create(Languages.en, "The EV roaming test network at the charging station operator"),
                                       InitialAdminStatus:                  RoamingNetworkAdminStatusTypes.Operational,
                                       InitialStatus:                       RoamingNetworkStatusTypes.Available
                                   );

            emp1RoamingNetwork   = new RoamingNetwork(
                                       Id:                                  RoamingNetwork_Id.Parse("test_emp1"),
                                       Name:                                I18NString.Create(Languages.en, "EV Roaming Test Network EMP1"),
                                       Description:                         I18NString.Create(Languages.en, "The EV roaming test network at the 1st e-mobility provider"),
                                       InitialAdminStatus:                  RoamingNetworkAdminStatusTypes.Operational,
                                       InitialStatus:                       RoamingNetworkStatusTypes.Available
                                   );

            emp2RoamingNetwork   = new RoamingNetwork(
                                       Id:                                  RoamingNetwork_Id.Parse("test_emp2"),
                                       Name:                                I18NString.Create(Languages.en, "EV Roaming Test Network EMP2"),
                                       Description:                         I18NString.Create(Languages.en, "The EV roaming test network at the 2nd e-mobility provider"),
                                       InitialAdminStatus:                  RoamingNetworkAdminStatusTypes.Operational,
                                       InitialStatus:                       RoamingNetworkStatusTypes.Available
                                   );

            Assert.IsNotNull(csoRoamingNetwork);
            Assert.IsNotNull(emp1RoamingNetwork);
            Assert.IsNotNull(emp2RoamingNetwork);

            #endregion

            #region Create cso/emp1/emp2 HTTP API

            csoHTTPAPI           = new HTTPAPI(
                                       HTTPServerPort:                      IPPort.Parse(3301),
                                       Autostart:                           true
                                   );

            emp1HTTPAPI          = new HTTPAPI(
                                       HTTPServerPort:                      IPPort.Parse(3401),
                                       Autostart:                           true
                                   );

            emp2HTTPAPI          = new HTTPAPI(
                                       HTTPServerPort:                      IPPort.Parse(3402),
                                       Autostart:                           true
                                   );

            Assert.IsNotNull(csoHTTPAPI);
            Assert.IsNotNull(emp1HTTPAPI);
            Assert.IsNotNull(emp2HTTPAPI);

            #endregion

            #region Create cso/emp1/emp2 OCPI Common API

            csoCommonAPI         = new CommonAPI(

                                       OurVersionsURL:                      URL.Parse("http://127.0.0.1:3301/ocpi/v2.2/versions"),
                                       OurCredentialRoles:                  new[] {
                                                                                new CredentialsRole(
                                                                                    CountryCode:       CountryCode.Parse("DE"),
                                                                                    PartyId:           Party_Id.   Parse("GEF"),
                                                                                    Role:              Roles.CPO,
                                                                                    BusinessDetails:   new BusinessDetails(
                                                                                                           "GraphDefiend CSO",
                                                                                                           URL.Parse("http://www.graphdefiend.com")
                                                                                                       )
                                                                                )
                                                                            },
                                       HTTPServer:                          csoHTTPAPI.HTTPServer,

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
                                       //APIVersionHashes:                    null,

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

            emp1CommonAPI        = new CommonAPI(

                                       OurVersionsURL:                      URL.Parse("http://127.0.0.1:3401/ocpi/v2.2/versions"),
                                       OurCredentialRoles:                  new[] {
                                                                                new CredentialsRole(
                                                                                    CountryCode:       CountryCode.Parse("DE"),
                                                                                    PartyId:           Party_Id.   Parse("GDF"),
                                                                                    Role:              Roles.EMSP,
                                                                                    BusinessDetails:   new BusinessDetails(
                                                                                                           "GraphDefiend EMP",
                                                                                                           URL.Parse("http://www.graphdefiend.com")
                                                                                                       )
                                                                                )
                                                                            },
                                       HTTPServer:                          emp1HTTPAPI.HTTPServer,

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

            emp2CommonAPI        = new CommonAPI(

                                       OurVersionsURL:                      URL.Parse("http://127.0.0.1:3402/ocpi/v2.2/versions"),
                                       OurCredentialRoles:                  new[] {
                                                                                new CredentialsRole(
                                                                                    CountryCode:       CountryCode.Parse("DE"),
                                                                                    PartyId:           Party_Id.   Parse("EMP"),
                                                                                    Role:              Roles.EMSP,
                                                                                    BusinessDetails:   new BusinessDetails(
                                                                                                           "GraphDefiend EMP",
                                                                                                           URL.Parse("http://www.example.org")
                                                                                                       )
                                                                                )
                                                                            },
                                       HTTPServer:                          emp2HTTPAPI.HTTPServer,

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

            Assert.IsNotNull(csoCommonAPI);
            Assert.IsNotNull(emp1CommonAPI);
            Assert.IsNotNull(emp2CommonAPI);

            #endregion

            #region Create cso CPO API / emp1 EMP API / emp2 EMP API

            csoCPOAPI            = new CPOAPI(

                                       CommonAPI:                           csoCommonAPI,
                                       DefaultCountryCode:                  csoCommonAPI.OurCredentialRoles.First().CountryCode,
                                       DefaultPartyId:                      csoCommonAPI.OurCredentialRoles.First().PartyId,
                                       AllowDowngrades:                     null,

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

            emp1EMSPAPI          = new EMSPAPI(

                                       CommonAPI:                           emp1CommonAPI,
                                       DefaultCountryCode:                  emp1CommonAPI.OurCredentialRoles.First().CountryCode,
                                       DefaultPartyId:                      emp1CommonAPI.OurCredentialRoles.First().PartyId,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2/2.2.1/emsp"),
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

            emp2EMSPAPI          = new EMSPAPI(

                                       CommonAPI:                           emp2CommonAPI,
                                       DefaultCountryCode:                  emp2CommonAPI.OurCredentialRoles.First().CountryCode,
                                       DefaultPartyId:                      emp2CommonAPI.OurCredentialRoles.First().PartyId,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.2/2.2.1/emsp"),
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

            Assert.IsNotNull(csoCPOAPI);
            Assert.IsNotNull(emp1EMSPAPI);
            Assert.IsNotNull(emp2EMSPAPI);

            #endregion

            #region Create cso/emp1/emp2 adapter

            csoAdapter           = csoRoamingNetwork.CreateOCPIv2_2_1_CSOAdapter(

                                       Id:                                  EMPRoamingProvider_Id.Parse("OCPIv2.2.1_CSO_" + this.csoRoamingNetwork.Id),
                                       Name:                                I18NString.Create(Languages.de, "OCPI v2.2.1 CSO"),
                                       Description:                         I18NString.Create(Languages.de, "OCPI v2.2.1 CSO Roaming"),

                                       CommonAPI:                           csoCommonAPI,
                                       DefaultCountryCode:                  csoCommonAPI.OurCredentialRoles.First().CountryCode,
                                       DefaultPartyId:                      csoCommonAPI.OurCredentialRoles.First().PartyId,

                                       CustomEVSEIdConverter:               null,
                                       CustomEVSEConverter:                 null,
                                       CustomEVSEStatusUpdateConverter:     null,
                                       CustomChargeDetailRecordConverter:   null,

                                       IncludeEVSEIds:                      null,
                                       IncludeEVSEs:                        null,
                                       IncludeChargingPoolIds:              null,
                                       IncludeChargingPools:                null,
                                       ChargeDetailRecordFilter:            null,

                                       ServiceCheckEvery:                   null,
                                       StatusCheckEvery:                    null,
                                       CDRCheckEvery:                       null,

                                       DisablePushData:                     true,
                                       DisablePushStatus:                   true,
                                       DisableAuthentication:               false,
                                       DisableSendChargeDetailRecords:      true

                                   );

            emp1Adapter          = emp1RoamingNetwork.CreateOCPIv2_2_1_EMPAdapter(

                                       Id:                                  CSORoamingProvider_Id.Parse("OCPIv2.2.1_EMP1_" + this.emp1RoamingNetwork.Id),
                                       Name:                                I18NString.Create(Languages.de, "OCPI v2.2.1 EMP1"),
                                       Description:                         I18NString.Create(Languages.de, "OCPI v2.2.1 EMP1 Roaming"),

                                       CommonAPI:                           emp1CommonAPI,
                                       DefaultCountryCode:                  emp1CommonAPI.OurCredentialRoles.First().CountryCode,
                                       DefaultPartyId:                      emp1CommonAPI.OurCredentialRoles.First().PartyId,

                                       CustomEVSEIdConverter:               null,
                                       CustomEVSEConverter:                 null,
                                       CustomEVSEStatusUpdateConverter:     null,
                                       CustomChargeDetailRecordConverter:   null,

                                       IncludeEVSEIds:                      null,
                                       IncludeEVSEs:                        null,
                                       IncludeChargingPoolIds:              null,
                                       IncludeChargingPools:                null,
                                       ChargeDetailRecordFilter:            null,

                                       ServiceCheckEvery:                   null,
                                       StatusCheckEvery:                    null,
                                       CDRCheckEvery:                       null,

                                       DisablePushData:                     true,
                                       DisablePushStatus:                   true,
                                       DisableAuthentication:               false,
                                       DisableSendChargeDetailRecords:      true

                                   );

            emp2Adapter          = emp2RoamingNetwork.CreateOCPIv2_2_1_EMPAdapter(

                                       Id:                                  CSORoamingProvider_Id.Parse("OCPIv2.2.1_EMP2_" + this.emp1RoamingNetwork.Id),
                                       Name:                                I18NString.Create(Languages.de, "OCPI v2.2.1 EMP2"),
                                       Description:                         I18NString.Create(Languages.de, "OCPI v2.2.1 EMP2 Roaming"),

                                       CommonAPI:                           emp2CommonAPI,
                                       DefaultCountryCode:                  emp2CommonAPI.OurCredentialRoles.First().CountryCode,
                                       DefaultPartyId:                      emp2CommonAPI.OurCredentialRoles.First().PartyId,

                                       CustomEVSEIdConverter:               null,
                                       CustomEVSEConverter:                 null,
                                       CustomEVSEStatusUpdateConverter:     null,
                                       CustomChargeDetailRecordConverter:   null,

                                       IncludeEVSEIds:                      null,
                                       IncludeEVSEs:                        null,
                                       IncludeChargingPoolIds:              null,
                                       IncludeChargingPools:                null,
                                       ChargeDetailRecordFilter:            null,

                                       ServiceCheckEvery:                   null,
                                       StatusCheckEvery:                    null,
                                       CDRCheckEvery:                       null,

                                       DisablePushData:                     true,
                                       DisablePushStatus:                   true,
                                       DisableAuthentication:               false,
                                       DisableSendChargeDetailRecords:      true

                                   );

            Assert.IsNotNull(csoAdapter);
            Assert.IsNotNull(emp1Adapter);
            Assert.IsNotNull(emp2Adapter);

            #endregion

            #region Create graphDefinedCSO / graphDefinedEMP / exampleEMP

            graphDefinedCSO         = csoRoamingNetwork.CreateChargingStationOperator(
                                          Id:                                  ChargingStationOperator_Id.Parse("DE*GEF"),
                                          Name:                                I18NString.Create(Languages.en, "GraphDefined CSO"),
                                          Description:                         I18NString.Create(Languages.en, "GraphDefined CSO Services"),
                                          InitialAdminStatus:                  ChargingStationOperatorAdminStatusTypes.Operational,
                                          InitialStatus:                       ChargingStationOperatorStatusTypes.Available
                                      ).Result.ChargingStationOperator;


            //graphDefinedEMP_remote  = csoRoamingNetwork.CreateEMobilityProvider(
            //                              Id:                                  EMobilityProvider_Id.Parse("DE*GDF"),
            //                              Name:                                I18NString.Create(Languages.en, "GraphDefined EMP"),
            //                              Description:                         I18NString.Create(Languages.en, "GraphDefined EMP Services"),
            //                              InitialAdminStatus:                  EMobilityProviderAdminStatusTypes.Operational,
            //                              InitialStatus:                       EMobilityProviderStatusTypes.Available,
            //                              RemoteEMobilityProviderCreator:      (eMobilityProvider) => {

            //                                                                       var empAdapter = new OCPIEMPAdapter(
            //                                                                                            Id:                 CSORoamingProvider_Id.Parse($"{emp1CommonAPI.OurCountryCode}-{emp1CommonAPI.OurPartyId}"),
            //                                                                                            Name:               I18NString.           Create(Languages.en, emp1CommonAPI.OurBusinessDetails.Name),
            //                                                                                            Description:        I18NString.           Create(Languages.en, emp1CommonAPI.OurBusinessDetails.Name + "_description"),
            //                                                                                            RoamingNetwork:     emp1RoamingNetwork,
            //                                                                                            CommonAPI:          emp1CommonAPI,
            //                                                                                            DefaultCountryCode: emp1CommonAPI.OurCountryCode,
            //                                                                                            DefaultPartyId:     emp1CommonAPI.OurPartyId
            //                                                                                        );

            //                                                                       // IRemoteEMobilityProvider
            //                                                                       return empAdapter;

            //                                                                   }
            //                          ).Result.EMobilityProvider;

            //var graphDefMAP_remote  = csoRoamingNetwork.CreateEMPRoamingProvider(
            //                              Id:                                  EMPRoamingProvider_Id.Parse("DE*GDF"),
            //                              Name:                                I18NString.Create(Languages.en, "GraphDefined EMP"),
            //                              Description:                         I18NString.Create(Languages.en, "GraphDefined EMP Services"),
            //                              InitialAdminStatus:                  EMobilityProviderAdminStatusTypes.Operational,
            //                              InitialStatus:                       EMobilityProviderStatusTypes.Available,
            //                              RemoteEMobilityProviderCreator:      (eMobilityProvider) => {

            //                                                                       var empAdapter = new OCPIEMPAdapter(
            //                                                                                            Id:                 CSORoamingProvider_Id.Parse($"{emp1CommonAPI.OurCountryCode}-{emp1CommonAPI.OurPartyId}"),
            //                                                                                            Name:               I18NString.           Create(Languages.en, emp1CommonAPI.OurBusinessDetails.Name),
            //                                                                                            Description:        I18NString.           Create(Languages.en, emp1CommonAPI.OurBusinessDetails.Name + "_description"),
            //                                                                                            RoamingNetwork:     emp1RoamingNetwork,
            //                                                                                            CommonAPI:          emp1CommonAPI,
            //                                                                                            DefaultCountryCode: emp1CommonAPI.OurCountryCode,
            //                                                                                            DefaultPartyId:     emp1CommonAPI.OurPartyId
            //                                                                                        );

            //                                                                       // IRemoteEMobilityProvider
            //                                                                       return empAdapter;

            //                                                                   }
            //                          ).Result.EMobilityProvider;








            //graphDefinedEMP     = emp1RoamingNetwork.CreateEMobilityProvider(
            //                          Id:                                  EMobilityProvider_Id.Parse("DE*GDF"),
            //                          Name:                                I18NString.Create(Languages.en, "GraphDefined EMP"),
            //                          Description:                         I18NString.Create(Languages.en, "GraphDefined EMP Services"),
            //                          InitialAdminStatus:                  EMobilityProviderAdminStatusTypes.Operational,
            //                          InitialStatus:                       EMobilityProviderStatusTypes.Available
            //                      ).Result.EMobilityProvider;






            //exampleEMP          = emp2RoamingNetwork.CreateEMobilityProvider(
            //                          Id:                                  EMobilityProvider_Id.Parse("DE*EMP"),
            //                          Name:                                I18NString.Create(Languages.en, "example EMP"),
            //                          Description:                         I18NString.Create(Languages.en, "example EMP Services"),
            //                          InitialAdminStatus:                  EMobilityProviderAdminStatusTypes.Operational,
            //                          InitialStatus:                       EMobilityProviderStatusTypes.Available
            //                      ).Result.EMobilityProvider;






            //Assert.IsNotNull(graphDefinedCSO);
            //Assert.IsNotNull(graphDefinedEMP);
            //Assert.IsNotNull(exampleEMP);

            #endregion

            #region Add Remote Parties

            csoAdapter!. AddRemoteParty(CountryCode:                 emp1CommonAPI.OurCredentialRoles.First().CountryCode,
                                        PartyId:                     emp1CommonAPI.OurCredentialRoles.First().PartyId,
                                        Role:                        Roles.EMSP,
                                        BusinessDetails:             emp1CommonAPI.OurCredentialRoles.First().BusinessDetails,

                                        AccessToken:                 AccessToken.Parse("cso-2-emp1:token"),
                                        AccessStatus:                AccessStatus.ALLOWED,

                                        RemoteAccessToken:           AccessToken.Parse("emp1-2-cso:token"),
                                        RemoteVersionsURL:           URL.Parse($"http://localhost:{emp1HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.2/versions"),
                                        RemoteVersionIds:            null,
                                        AccessTokenBase64Encoding:   true,
                                        RemoteStatus:                RemoteAccessStatus.ONLINE,

                                        PartyStatus:                 PartyStatus.ENABLED);

            csoAdapter!. AddRemoteParty(CountryCode:                 emp2CommonAPI.OurCredentialRoles.First().CountryCode,
                                        PartyId:                     emp2CommonAPI.OurCredentialRoles.First().PartyId,
                                        Role:                        Roles.EMSP,
                                        BusinessDetails:             emp2CommonAPI.OurCredentialRoles.First().BusinessDetails,
                                        AccessToken:                 AccessToken.Parse("cso-2-emp2:token"),
                                        AccessStatus:                AccessStatus.ALLOWED,
                                        RemoteAccessToken:           AccessToken.Parse("emp2-2-cso:token"),
                                        RemoteVersionsURL:           URL.Parse($"http://localhost:{emp2HTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.2/versions"),
                                        RemoteVersionIds:            null,
                                        AccessTokenBase64Encoding:   true,
                                        RemoteStatus:                RemoteAccessStatus.ONLINE,
                                        PartyStatus:                 PartyStatus.ENABLED);



            emp1Adapter!.AddRemoteParty(CountryCode:                 csoCommonAPI.OurCredentialRoles.First().CountryCode,
                                        PartyId:                     csoCommonAPI.OurCredentialRoles.First().PartyId,
                                        Role:                        Roles.CPO,
                                        BusinessDetails:             csoCommonAPI.OurCredentialRoles.First().BusinessDetails,

                                        AccessToken:                 AccessToken.Parse("emp1-2-cso:token"),
                                        AccessStatus:                AccessStatus.ALLOWED,

                                        RemoteAccessToken:           AccessToken.Parse("cso-2-emp1:token"),
                                        RemoteVersionsURL:           URL.Parse($"http://localhost:{csoHTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.2/versions"),
                                        RemoteVersionIds:            null,
                                        AccessTokenBase64Encoding:   true,
                                        RemoteStatus:                RemoteAccessStatus.ONLINE,

                                        PartyStatus:                 PartyStatus.ENABLED);


            emp2Adapter!.AddRemoteParty(CountryCode:                 csoCommonAPI.OurCredentialRoles.First().CountryCode,
                                        PartyId:                     csoCommonAPI.OurCredentialRoles.First().PartyId,
                                        Role:                        Roles.CPO,
                                        BusinessDetails:             csoCommonAPI.OurCredentialRoles.First().BusinessDetails,

                                        AccessToken:                 AccessToken.Parse("emp2-2-cso:token"),
                                        AccessStatus:                AccessStatus.ALLOWED,

                                        RemoteAccessToken:           AccessToken.Parse("cso-2-emp2:token"),
                                        RemoteVersionsURL:           URL.Parse($"http://localhost:{csoHTTPAPI.HTTPServer.IPPorts.First()}/ocpi/v2.2/versions"),
                                        RemoteVersionIds:            null,
                                        AccessTokenBase64Encoding:   true,
                                        RemoteStatus:                RemoteAccessStatus.ONLINE,

                                        PartyStatus:                 PartyStatus.ENABLED);


            Assert.AreEqual(2, csoCommonAPI. RemoteParties.Count());
            Assert.AreEqual(1, emp1CommonAPI.RemoteParties.Count());
            Assert.AreEqual(1, emp2CommonAPI.RemoteParties.Count());

            #endregion

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public void ShutdownEachTest()
        {

            csoHTTPAPI?.Shutdown();
            emp1HTTPAPI?.Shutdown();
            emp2HTTPAPI?.Shutdown();

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
