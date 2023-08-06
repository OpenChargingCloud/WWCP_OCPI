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
                                   Name:                                I18NString.Create(Languages.en, "EV Roaming Test Network"),
                                   Description:                         I18NString.Create(Languages.en, "An EV roaming test network"),
                                   InitialAdminStatus:                  RoamingNetworkAdminStatusTypes.Operational,
                                   InitialStatus:                       RoamingNetworkStatusTypes.     Available,
                                   RoamingNetworkInfos:                 new RoamingNetworkInfo[] {

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
                                                                            //    contentType:          HTTPContentType.JSONLD_UTF8,
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
                                                                            //    contentType:          HTTPContentType.JSONLD_UTF8,
                                                                            //    protocolType:         ProtocolTypes.WWCP,
                                                                            //    PublicKeys:           null)

                                                                        }
                               );

            Assert.IsNotNull(roamingNetwork);


            httpAPI          = new HTTPAPI(
                                   HTTPServerPort:                      IPPort.Parse(3473),
                                   AutoStart:                           true
                               );

            Assert.IsNotNull(httpAPI);


            commonAPI        = new CommonAPI(

                                   OurBaseURL:                          URL.Parse("http://127.0.0.1:3473/ocpi/v2.1"),
                                   OurVersionsURL:                      URL.Parse("http://127.0.0.1:3473/ocpi/v2.1/versions"),
                                   OurBusinessDetails:                  new BusinessDetails(
                                                                            "GraphDefined CSO",
                                                                            URL.Parse("https://www.graphdefined.com/cso")
                                                                        ),
                                   OurCountryCode:                      CountryCode.Parse("DE"),
                                   OurPartyId:                          Party_Id.   Parse("GEF"),
                                   OurRole:                             Roles.      CPO,

                                   HTTPServer:                          httpAPI.HTTPServer,

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
                                   LogfileName:                         null,
                                   LogfileCreator:                      null,
                                   AutoStart:                           false

                               );

            Assert.IsNotNull(commonAPI);


            cpoAPI           = new CPOAPI(

                                   CommonAPI:                           commonAPI,
                                   DefaultCountryCode:                  commonAPI.OurCountryCode,
                                   DefaultPartyId:                      commonAPI.OurPartyId,
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

            Assert.IsNotNull(cpoAPI);


            csoAdapter       = roamingNetwork.CreateOCPIv2_1_1_CSOAdapter(

                                   Id:                                  EMPRoamingProvider_Id.Parse("OCPIv2.1_CSO_" + this.roamingNetwork.Id),
                                   Name:                                I18NString.Create(Languages.de, "OCPI v2.1 CSO"),
                                   Description:                         I18NString.Create(Languages.de, "OCPI v2.1 CSO Roaming"),

                                   CommonAPI:                           commonAPI,
                                   DefaultCountryCode:                  CountryCode.Parse("DE"),
                                   DefaultPartyId:                      Party_Id.   Parse("GEF"),

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

            Assert.IsNotNull(csoAdapter);


            graphDefinedCSO  = roamingNetwork.CreateChargingStationOperator(
                                   Id:                  ChargingStationOperator_Id.Parse("DE*GEF"),
                                   Name:                I18NString.Create(Languages.en, "GraphDefined CSO"),
                                   Description:         I18NString.Create(Languages.en, "GraphDefined CSO Services"),
                                   InitialAdminStatus:  ChargingStationOperatorAdminStatusTypes.Operational,
                                   InitialStatus:       ChargingStationOperatorStatusTypes.Available
                               ).Result.ChargingStationOperator;

            Assert.IsNotNull(graphDefinedCSO);

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
