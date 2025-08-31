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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.MobilityProvider;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests
{

    /// <summary>
    /// Roaming test defaults for a charging station operator connected
    /// to two e-mobility providers via OCPI v2.1.1.
    /// </summary>
    public abstract class ARoamingTests : ANodeTests
    {

        #region Data

        protected  RoamingNetwork?            csoRoamingNetwork;
        protected  IChargingStationOperator?  graphDefinedCSO;

        protected  VirtualEMobilityProvider?  graphDefinedEMP1Local;
        protected  RoamingNetwork?            emp1RoamingNetwork;
        protected  IEMobilityProvider?        graphDefinedEMP1;
        protected  EMobilityProviderAPI?      graphDefinedEMP1API;

        protected  VirtualEMobilityProvider?  graphDefinedEMP2Local;
        protected  RoamingNetwork?            emp2RoamingNetwork;
        protected  IEMobilityProvider?        graphDefinedEMP2;
        protected  EMobilityProviderAPI?      graphDefinedEMP2API;

        protected  VirtualSmartPhone?         ahzfPhone;
        protected  EVehicle?                  ahzfCar;

        #endregion

        #region Constructor(s)

        public ARoamingTests()
        {

        }

        #endregion


        #region SetupEachTest()

        [SetUp]
        public override async Task SetupEachTest()
        {

            await base.SetupEachTest();

            #region Create cso/emp1/emp2 roaming network

            csoRoamingNetwork    = new RoamingNetwork(
                                       Id:                  RoamingNetwork_Id.Parse("test_cso"),
                                       Name:                I18NString.Create("CSO EV Roaming Test Network"),
                                       Description:         I18NString.Create("The EV roaming test network at the charging station operator"),
                                       InitialAdminStatus:  RoamingNetworkAdminStatusTypes.Operational,
                                       InitialStatus:       RoamingNetworkStatusTypes.Available
                                   );

            emp1RoamingNetwork   = new RoamingNetwork(
                                       Id:                  RoamingNetwork_Id.Parse("test_emp1"),
                                       Name:                I18NString.Create("EV Roaming Test Network EMP1"),
                                       Description:         I18NString.Create("The EV roaming test network at the 1st e-mobility provider"),
                                       InitialAdminStatus:  RoamingNetworkAdminStatusTypes.Operational,
                                       InitialStatus:       RoamingNetworkStatusTypes.Available
                                   );

            emp2RoamingNetwork   = new RoamingNetwork(
                                       Id:                  RoamingNetwork_Id.Parse("test_emp2"),
                                       Name:                I18NString.Create("EV Roaming Test Network EMP2"),
                                       Description:         I18NString.Create("The EV roaming test network at the 2nd e-mobility provider"),
                                       InitialAdminStatus:  RoamingNetworkAdminStatusTypes.Operational,
                                       InitialStatus:       RoamingNetworkStatusTypes.Available
                                   );

            ClassicAssert.IsNotNull(csoRoamingNetwork);
            ClassicAssert.IsNotNull(emp1RoamingNetwork);
            ClassicAssert.IsNotNull(emp2RoamingNetwork);

            #endregion

            #region Create graphDefinedCSO / graphDefinedEMP1 / graphDefinedEMP2

            var csoResult          = await csoRoamingNetwork.CreateChargingStationOperator(
                                                                 Id:                   ChargingStationOperator_Id.Parse("DE*GEF"),
                                                                 Name:                 I18NString.Create("GraphDefined CSO"),
                                                                 Description:          I18NString.Create("GraphDefined CSO Services"),
                                                                 InitialAdminStatus:   ChargingStationOperatorAdminStatusTypes.Operational,
                                                                 InitialStatus:        ChargingStationOperatorStatusTypes.Available
                                                             );

            ClassicAssert.IsTrue   (csoResult.Result == org.GraphDefined.Vanaheimr.Illias.CommandResult.Success);
            ClassicAssert.IsNotNull(csoResult.ChargingStationOperator);

            graphDefinedCSO        = csoResult.ChargingStationOperator;



            var emp1result         = await emp1RoamingNetwork.CreateEMobilityProvider(

                                                                  Id:                               EMobilityProvider_Id.Parse("DE-GDF"),
                                                                  Name:                             I18NString.Create("GraphDefined EMP #1"),
                                                                  Description:                      I18NString.Create("GraphDefined EMP #1 Services"),
                                                                  InitialAdminStatus:               EMobilityProviderAdminStatusTypes.Operational,
                                                                  InitialStatus:                    EMobilityProviderStatusTypes.Available,

                                                                  RemoteEMobilityProviderCreator:   eMobilityProvider => new VirtualEMobilityProvider(
                                                                                                                             EMobilityProvider_Id.Parse("DE-GDF"),
                                                                                                                             eMobilityProvider.RoamingNetwork
                                                                                                                         )

                                                              );

            ClassicAssert.IsTrue   (emp1result.Result == org.GraphDefined.Vanaheimr.Illias.CommandResult.Success);
            ClassicAssert.IsNotNull(emp1result.EMobilityProvider);

            graphDefinedEMP1       = emp1result.EMobilityProvider;
            graphDefinedEMP1Local  = graphDefinedEMP1?.RemoteEMobilityProvider as VirtualEMobilityProvider;


            var emp2result         = await emp2RoamingNetwork.CreateEMobilityProvider(

                                                                  Id:                               EMobilityProvider_Id.Parse("DE-GD2"),
                                                                  Name:                             I18NString.Create("GraphDefined EMP #2"),
                                                                  Description:                      I18NString.Create("GraphDefined EMP #2 Services"),
                                                                  InitialAdminStatus:               EMobilityProviderAdminStatusTypes.Operational,
                                                                  InitialStatus:                    EMobilityProviderStatusTypes.Available,

                                                                  RemoteEMobilityProviderCreator:   eMobilityProvider => new VirtualEMobilityProvider(
                                                                                                                             EMobilityProvider_Id.Parse("DE-GD2"),
                                                                                                                             eMobilityProvider.RoamingNetwork
                                                                                                                         )

                                                              );

            ClassicAssert.IsTrue   (emp2result.Result == org.GraphDefined.Vanaheimr.Illias.CommandResult.Success);
            ClassicAssert.IsNotNull(emp2result.EMobilityProvider);

            graphDefinedEMP2       = emp2result.EMobilityProvider;
            graphDefinedEMP2Local  = graphDefinedEMP2?.RemoteEMobilityProvider as VirtualEMobilityProvider;
            graphDefinedEMP2Local?.StartAPI(HTTPServerPort: IPPort.Parse(3501));

            #endregion

            #region Create cpo/emsp1/emsp2 adapter

            ClassicAssert.IsNotNull(cpoCPOAPI);
            ClassicAssert.IsNotNull(emsp1EMSPAPI);
            ClassicAssert.IsNotNull(emsp2EMSPAPI);

            if (cpoCPOAPI    is not null &&
                emsp1EMSPAPI is not null &&
                emsp2EMSPAPI is not null)
            {

                cpoAdapter           = csoRoamingNetwork.CreateOCPIv2_1_1_CSOAdapter(

                                           Id:                                  CSORoamingProvider_Id.Parse("OCPIv2.1_CSO_" + this.csoRoamingNetwork.Id),
                                           Name:                                I18NString.Create(Languages.de, "OCPI v2.1 CSO"),
                                           Description:                         I18NString.Create(Languages.de, "OCPI v2.1 CSO Roaming"),

                                           CPOAPI:                              cpoCPOAPI,

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
                                           DisableSendChargeDetailRecords:      false

                                       );

                emsp1Adapter          = emp1RoamingNetwork.CreateOCPIv2_1_EMPAdapter(

                                           Id:                                  EMPRoamingProvider_Id.Parse("OCPIv2.1_EMP1_" + this.emp1RoamingNetwork.Id),
                                           Name:                                I18NString.Create(Languages.de, "OCPI v2.1 EMP1"),
                                           Description:                         I18NString.Create(Languages.de, "OCPI v2.1 EMP1 Roaming"),

                                           EMSPAPI:                             emsp1EMSPAPI,

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
                                           DisableSendChargeDetailRecords:      false

                                       );

                emsp2Adapter          = emp2RoamingNetwork.CreateOCPIv2_1_EMPAdapter(

                                           Id:                                  EMPRoamingProvider_Id.Parse("OCPIv2.1_EMP2_" + this.emp1RoamingNetwork.Id),
                                           Name:                                I18NString.Create(Languages.de, "OCPI v2.1 EMP2"),
                                           Description:                         I18NString.Create(Languages.de, "OCPI v2.1 EMP2 Roaming"),

                                           EMSPAPI:                             emsp2EMSPAPI,

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
                                           DisableSendChargeDetailRecords:      false

                                       );

                ClassicAssert.IsNotNull(cpoAdapter);
                ClassicAssert.IsNotNull(emsp1Adapter);
                ClassicAssert.IsNotNull(emsp2Adapter);

            }

            #endregion


            ahzfPhone = new VirtualSmartPhone();
            ahzfPhone?.Connect(URL.Parse("http://127.0.0.1:3501"));

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public override async Task ShutdownEachTest()
        {

            await base.ShutdownEachTest();

        }

        #endregion


    }

}
