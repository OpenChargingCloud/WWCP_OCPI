/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests
{

    /// <summary>
    /// Roaming test defaults for a charging station operator connected
    /// to two e-mobility providers via OCPI v2.2.1.
    /// </summary>
    public abstract class ARoamingTests : ANodeTests
    {

        #region Data

        protected  RoamingNetwork?            csoRoamingNetwork;
        protected  IChargingStationOperator?  graphDefinedCSO;

        protected  IEMobilityProvider?        graphDefinedEMP;
        protected  RoamingNetwork?            emp1RoamingNetwork;
        protected  IEMobilityProvider?        graphDefinedEMP_remote;

        protected  RoamingNetwork?            emp2RoamingNetwork;
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

            #region Create graphDefinedCSO / graphDefinedEMP / exampleEMP

            var result = await csoRoamingNetwork.CreateChargingStationOperator(
                                                     Id:                                  ChargingStationOperator_Id.Parse("DE*GEF"),
                                                     Name:                                I18NString.Create("GraphDefined CSO"),
                                                     Description:                         I18NString.Create("GraphDefined CSO Services"),
                                                     InitialAdminStatus:                  ChargingStationOperatorAdminStatusTypes.Operational,
                                                     InitialStatus:                       ChargingStationOperatorStatusTypes.Available
                                                 );

            ClassicAssert.IsTrue   (result.Result == org.GraphDefined.Vanaheimr.Illias.CommandResult.Success);
            ClassicAssert.IsNotNull(result.ChargingStationOperator);

            graphDefinedCSO = result.ChargingStationOperator;


            //graphDefinedEMP_remote  = csoRoamingNetwork.CreateEMobilityProvider(
            //                              Id:                                  EMobilityProvider_Id.Parse("DE*GDF"),
            //                              Name:                                I18NString.Create("GraphDefined EMP"),
            //                              Description:                         I18NString.Create("GraphDefined EMP Services"),
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
            //                              Name:                                I18NString.Create("GraphDefined EMP"),
            //                              Description:                         I18NString.Create("GraphDefined EMP Services"),
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
            //                          Name:                                I18NString.Create("GraphDefined EMP"),
            //                          Description:                         I18NString.Create("GraphDefined EMP Services"),
            //                          InitialAdminStatus:                  EMobilityProviderAdminStatusTypes.Operational,
            //                          InitialStatus:                       EMobilityProviderStatusTypes.Available
            //                      ).Result.EMobilityProvider;






            //exampleEMP          = emp2RoamingNetwork.CreateEMobilityProvider(
            //                          Id:                                  EMobilityProvider_Id.Parse("DE*EMP"),
            //                          Name:                                I18NString.Create("example EMP"),
            //                          Description:                         I18NString.Create("example EMP Services"),
            //                          InitialAdminStatus:                  EMobilityProviderAdminStatusTypes.Operational,
            //                          InitialStatus:                       EMobilityProviderStatusTypes.Available
            //                      ).Result.EMobilityProvider;






            //ClassicAssert.IsNotNull(graphDefinedCSO);
            //ClassicAssert.IsNotNull(graphDefinedEMP);
            //ClassicAssert.IsNotNull(exampleEMP);

            #endregion

            #region Create cpo/emsp1/emsp2 adapter

            ClassicAssert.IsNotNull(cpoCommonAPI);
            ClassicAssert.IsNotNull(emsp1CommonAPI);
            ClassicAssert.IsNotNull(emsp2CommonAPI);

            if (cpoCommonAPI   is not null &&
                emsp1CommonAPI is not null &&
                emsp2CommonAPI is not null)
            {

                cpoAdapter           = csoRoamingNetwork.CreateOCPIv2_2_1_CSOAdapter(

                                           Id:                                  CSORoamingProvider_Id.Parse("OCPIv2.2.1_CSO_" + this.csoRoamingNetwork.Id),
                                           Name:                                I18NString.Create(Languages.de, "OCPI v2.2.1 CSO"),
                                           Description:                         I18NString.Create(Languages.de, "OCPI v2.2.1 CSO Roaming"),

                                           CommonAPI:                           cpoCommonAPI,

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

                emsp1Adapter         = emp1RoamingNetwork.CreateOCPIv2_2_1_EMPAdapter(

                                           Id:                                  EMPRoamingProvider_Id.Parse("OCPIv2.2.1_EMP1_" + this.emp1RoamingNetwork.Id),
                                           Name:                                I18NString.Create(Languages.de, "OCPI v2.2.1 EMP1"),
                                           Description:                         I18NString.Create(Languages.de, "OCPI v2.2.1 EMP1 Roaming"),

                                           CommonAPI:                           emsp1CommonAPI,

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

                emsp2Adapter         = emp2RoamingNetwork.CreateOCPIv2_2_1_EMPAdapter(

                                           Id:                                  EMPRoamingProvider_Id.Parse("OCPIv2.2.1_EMP2_" + this.emp1RoamingNetwork.Id),
                                           Name:                                I18NString.Create(Languages.de, "OCPI v2.2.1 EMP2"),
                                           Description:                         I18NString.Create(Languages.de, "OCPI v2.2.1 EMP2 Roaming"),

                                           CommonAPI:                           emsp2CommonAPI,

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

                ClassicAssert.IsNotNull(cpoAdapter);
                ClassicAssert.IsNotNull(emsp1Adapter);
                ClassicAssert.IsNotNull(emsp2Adapter);

            }

            #endregion

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public override async Task ShutdownEachTest()
        {

            await base.ShutdownEachTest();

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
