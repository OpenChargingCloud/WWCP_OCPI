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

using NUnit.Framework;
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_1_1.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.Networking;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests
{

    /// <summary>
    /// OCPI v2.1.1 adapter test defaults.
    /// </summary>
    public abstract class ACSOAdapterTests
    {

        #region Data

        protected  RoamingNetwork?            roamingNetwork;
        protected  HTTPAPI?                   httpAPI;
        protected  CommonAPI?                 commonAPI;
        protected  CPOAPI?                    cpoAPI;
        protected  OCPICSOAdapter?            csoAdapter;
        protected  IChargingStationOperator?  graphDefinedCSO;

        #endregion

        #region Constructor(s)

        public ACSOAdapterTests()
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

            roamingNetwork   = new RoamingNetwork(
                                   Id:                                  RoamingNetwork_Id.Parse("test"),
                                   Name:                                I18NString.Create("EV Roaming Test Network"),
                                   Description:                         I18NString.Create("An EV roaming test network"),
                                   InitialAdminStatus:                  RoamingNetworkAdminStatusTypes.Operational,
                                   InitialStatus:                       RoamingNetworkStatusTypes.     Available,
                                   RoamingNetworkInfos:                 [

                                                                            //new RoamingNetworkInfo(
                                                                            //    TrackerId:            Tracker_Id.Parse("1"),
                                                                            //    NodeId:               Node_Id.Parse("de-bd-gw-01"),
                                                                            //    IncomingURL:          "",
                                                                            //    ExpiredAfter:         Timestamp.Now,
                                                                            //
                                                                            //    RoamingNetworkId:     this.RoamingNetworkId,
                                                                            //    priority:             10,
                                                                            //    weight:               10,
                                                                            //    hostname:             null,
                                                                            //    IPAddress:            IPv4Address.Parse("127.0.0.1"),
                                                                            //    port:                 IPPort.Parse(4001),
                                                                            //    transport:            TransportTypes.TCP,
                                                                            //    URLPrefix:            "",
                                                                            //    contentType:          HTTPContentType.Application.JSONLD_UTF8,
                                                                            //    protocolType:         ProtocolTypes.WWCP,
                                                                            //    PublicKeys:           null)

                                                                            //new RoamingNetworkInfo(
                                                                            //    TrackerId:            Tracker_Id.Parse("1"),
                                                                            //    NodeId:               Node_Id.Parse("de-bd-gw-02"),
                                                                            //    IncomingURL:          "",
                                                                            //    ExpiredAfter:         Timestamp.Now,

                                                                            //    RoamingNetworkId:     RoamingNetwork_Id.Parse("Prod"),
                                                                            //    priority:             10,
                                                                            //    weight:               10,
                                                                            //    hostname:             null,
                                                                            //    IPAddress:            IPv4Address.Parse("127.0.0.1"),
                                                                            //    port:                 IPPort.Parse(4002),
                                                                            //    transport:            TransportTypes.TCP,
                                                                            //    URLPrefix:            "",
                                                                            //    contentType:          HTTPContentType.Application.JSONLD_UTF8,
                                                                            //    protocolType:         ProtocolTypes.WWCP,
                                                                            //    PublicKeys:           null)

                                                                        ]
                               );

            ClassicAssert.IsNotNull(roamingNetwork);


            httpAPI          = new HTTPAPI(
                                   HTTPServerPort:                      IPPort.Parse(3473),
                                   AutoStart:                           true
                               );

            ClassicAssert.IsNotNull(httpAPI);


            var ocpiBaseAPI = new CommonBaseAPI(

                                  OurBaseURL:                URL.Parse("http://127.0.0.1:3473/ocpi/v2.1"),
                                  OurVersionsURL:            URL.Parse("http://127.0.0.1:3473/ocpi/v2.1/versions"),
                                  HTTPServer:                httpAPI.HTTPServer,
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

            ClassicAssert.IsNotNull(ocpiBaseAPI);


            commonAPI        = new CommonAPI(

                                   //OurBaseURL:                          URL.Parse("http://127.0.0.1:3473/ocpi/v2.1"),
                                   //OurVersionsURL:                      URL.Parse("http://127.0.0.1:3473/ocpi/v2.1/versions"),
                                   OurBusinessDetails:                  new BusinessDetails(
                                                                            "GraphDefined CSO",
                                                                            URL.Parse("https://www.graphdefined.com/cso")
                                                                        ),
                                   OurCountryCode:                      CountryCode.Parse("DE"),
                                   OurPartyId:                          Party_Id.   Parse("GEF"),
                                   OurRole:                             Role.       CPO,

                                   BaseAPI:                             ocpiBaseAPI,

                                   AdditionalURLPathPrefix:             null,
                                   KeepRemovedEVSEs:                    null,
                                   LocationsAsOpenData:                 true,
                                   AllowDowngrades:                     null,

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
                                   LogfileName:                         null,
                                   LogfileCreator:                      null,
                                   AutoStart:                           false

                               );

            ClassicAssert.IsNotNull(commonAPI);


            cpoAPI           = new CPOAPI(

                                   CommonAPI:                           commonAPI,
                                   AllowDowngrades:                     null,

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
                                   LogfileName:                         null,
                                   LogfileCreator:                      null,
                                   AutoStart:                           false

                               );

            ClassicAssert.IsNotNull(cpoAPI);


            csoAdapter       = roamingNetwork.CreateOCPIv2_1_1_CSOAdapter(

                                   Id:                                  CSORoamingProvider_Id.Parse("OCPIv2.1_CSO_" + this.roamingNetwork.Id),
                                   Name:                                I18NString.Create(Languages.de, "OCPI v2.1 CSO"),
                                   Description:                         I18NString.Create(Languages.de, "OCPI v2.1 CSO Roaming"),

                                   CommonAPI:                           commonAPI,

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
                                   DisableAuthentication:               true,
                                   DisableSendChargeDetailRecords:      true

                               );

            ClassicAssert.IsNotNull(csoAdapter);


            graphDefinedCSO  = roamingNetwork.CreateChargingStationOperator(
                                   Id:                  ChargingStationOperator_Id.Parse("DE*GEF"),
                                   Name:                I18NString.Create("GraphDefined CSO"),
                                   Description:         I18NString.Create("GraphDefined CSO Services"),
                                   InitialAdminStatus:  ChargingStationOperatorAdminStatusTypes.Operational,
                                   InitialStatus:       ChargingStationOperatorStatusTypes.Available
                               ).Result.ChargingStationOperator;

            ClassicAssert.IsNotNull(graphDefinedCSO);

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public void ShutdownEachTest()
        {
            httpAPI?.Shutdown();
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
