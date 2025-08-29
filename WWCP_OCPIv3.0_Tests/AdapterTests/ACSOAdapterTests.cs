/*
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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv3_0.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.UnitTests
{

    /// <summary>
    /// OCPI v3.0 adapter test defaults.
    /// </summary>
    public abstract class ACSOAdapterTests
    {

        #region Data

        protected  RoamingNetwork?            roamingNetwork;
        protected  HTTPTestServerX?           httpServer;
        protected  CommonAPI?                 commonAPI;
        protected  CPOAPI?                    cpoAPI;
        protected  OCPICSOAdapter?            csoAdapter;
        protected  IChargingStationOperator?  graphDefinedCSO;

        protected  URL                        remoteLocationsURL = URL.Parse("http://127.0.0.1:3473/ocpi/v3.0/locations");

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

            Assert.That(roamingNetwork, Is.Not.Null);

            httpServer       = new HTTPTestServerX(
                                   TCPPort:                   IPPort.Parse(3473)
                               );

            Assert.That(httpServer,  Is.Not.Null);

            var ocpiBaseAPI  = new CommonHTTPAPI(

                                   OurBaseURL:                URL.Parse("http://127.0.0.1:3473/ocpi/v3.0/"),
                                   OurVersionsURL:            URL.Parse("http://127.0.0.1:3473/ocpi/v3.0/versions"),
                                   HTTPServer:                httpServer,
                                   AdditionalURLPathPrefix:   null,
                                   //KeepRemovedEVSEs:          null,
                                   LocationsAsOpenData:       true,
                                   AllowDowngrades:           null,
                                   //Disable_RootServices:      false,

                  //                 HTTPHostname:              null,
                                   ExternalDNSName:           null,
                                   HTTPServiceName:           null,
                                   BasePath:                  null,

                                   //URLPathPrefix:             HTTPPath.Parse("/ocpi"),
                                   RootPath:                  HTTPPath.Parse("/ocpi"),
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

            Assert.That(ocpiBaseAPI,  Is.Not.Null);


            commonAPI        = new CommonAPI(

                                   //OurBaseURL:                          URL.Parse("http://127.0.0.1:3473/ocpi/v3.0/"),
                                   //OurVersionsURL:                      URL.Parse("http://127.0.0.1:3473/ocpi/v3.0/versions"),
                                   OurCredentialRoles:                  [
                                                                            new CredentialsRole(
                                                                                PartyId:          Party_Idv3.Parse("DEGEF"),
                                                                                Role:             Role.CPO,
                                                                                BusinessDetails:  new BusinessDetails(
                                                                                                      "GraphDefined CSO",
                                                                                                      URL.Parse("https://www.graphdefined.com/cso")
                                                                                                  )
                                                                            )
                                                                        ],
                                   DefaultPartyId:                      Party_Idv3.Parse("DEGEF"),

                                   BaseAPI:                             ocpiBaseAPI,

                                   AdditionalURLPathPrefix:             null,
                                   KeepRemovedEVSEs:                    null,
                                   LocationsAsOpenData:                 true,
                                   AllowDowngrades:                     null,
                                   Disable_RootServices:                false,

                                   HTTPHostname:                        null,
                                   ExternalDNSName:                     null,
                                   HTTPServiceName:                     null,
                                   BasePath:                            null,

                                   URLPathPrefix:                       HTTPPath.Parse("/ocpi/v3.0/"),
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
                                   LogfileCreator:                      null

                               );

            Assert.That(commonAPI, Is.Not.Null);


            cpoAPI           = new CPOAPI(

                                   CommonAPI:                           commonAPI,
                                   AllowDowngrades:                     null,

                                   HTTPHostname:                        null,
                                   ExternalDNSName:                     null,
                                   HTTPServiceName:                     null,
                                   BasePath:                            null,

                                   URLPathPrefix:                       HTTPPath.Parse("/ocpi/v3.0/"),
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
                                   LogfileCreator:                      null

                               );

            Assert.That(cpoAPI, Is.Not.Null);


            csoAdapter       = roamingNetwork.CreateOCPIv3_0_CSOAdapter(

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

            Assert.That(csoAdapter, Is.Not.Null);


            graphDefinedCSO  = roamingNetwork.CreateChargingStationOperator(
                                   Id:                  ChargingStationOperator_Id.Parse("DE*GEF"),
                                   Name:                I18NString.Create("GraphDefined CSO"),
                                   Description:         I18NString.Create("GraphDefined CSO Services"),
                                   InitialAdminStatus:  ChargingStationOperatorAdminStatusTypes.Operational,
                                   InitialStatus:       ChargingStationOperatorStatusTypes.Available
                               ).Result.ChargingStationOperator;

            Assert.That(graphDefinedCSO, Is.Not.Null);


        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public async Task ShutdownEachTest()
        {
            if (httpServer is not null)
                await httpServer.Stop();
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
