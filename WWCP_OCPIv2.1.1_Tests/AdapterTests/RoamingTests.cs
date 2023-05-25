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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Aegir;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.RoamingTests.CSO
{

    /// <summary>
    /// OCPI tests in combination with a WWCP roaming network.
    /// Will test the WWCP-to-OCPI adapters.
    /// </summary>
    [TestFixture]
    public class RoamingTests : ARoamingTests
    {

        #region Add_ChargingLocationsAndEVSEs_Test1()

        /// <summary>
        /// Add WWCP charging locations, stations and EVSEs.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the OCPI module.
        /// </summary>
        [Test]
        public async Task Add_ChargingLocationsAndEVSEs_Test1()
        {

            if (graphDefinedCSO is not null &&
                cpoAdapter      is not null &&
                emsp1Adapter    is not null &&
                emsp2Adapter    is not null)
            {

                #region Add DE*GEF*POOL1

                // Will call OCPICSOAdapter.AddStaticData(ChargingPool, ...)!
                var addChargingPoolResult1 = await graphDefinedCSO.CreateChargingPool(

                                                 Id: ChargingPool_Id.Parse("DE*GEF*POOL1"),
                                                 Name: I18NString.Create(Languages.en, "Test pool #1"),
                                                 Description: I18NString.Create(Languages.en, "GraphDefined charging pool for tests #1"),

                                                 Address: new Address(

                                                                           Street: "Biberweg",
                                                                           PostalCode: "07749",
                                                                           City: I18NString.Create(Languages.de, "Jena"),
                                                                           Country: Country.Germany,

                                                                           HouseNumber: "18",
                                                                           FloorLevel: null,
                                                                           Region: null,
                                                                           PostalCodeSub: null,
                                                                           TimeZone: Time_Zone.TryParse("CET"),
                                                                           OfficialLanguages: null,
                                                                           Comment: null,

                                                                           CustomData: null,
                                                                           InternalData: null

                                                                       ),
                                                 GeoLocation: GeoCoordinate.Parse(50.93, 11.63),

                                                 OpeningTimes: null,
                                                 ChargingWhenClosed: true,

                                                 //EnergyMix
                                                 //RelatedLocations
                                                 //Directions
                                                 //Facilities
                                                 //Images
                                                 //LocationType

                                                 InitialAdminStatus: ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus: ChargingPoolStatusTypes.Available,

                                                 Configurator: chargingPool =>
                                                 {
                                                 }

                                             );

                Assert.IsNotNull(addChargingPoolResult1);
                Assert.IsTrue(addChargingPoolResult1.IsSuccess);

                var chargingPool1 = addChargingPoolResult1.ChargingPool;
                Assert.IsNotNull(chargingPool1);

                if (chargingPool1 is not null)
                {

                    var ocpiLocation1 = cpoAdapter.CommonAPI.GetLocations().FirstOrDefault(location => location.Name == chargingPool1.Name.FirstText());
                    Assert.IsNotNull(ocpiLocation1);

                    if (ocpiLocation1 is not null)
                    {

                        Assert.AreEqual($"{chargingPool1.Address?.Street} {chargingPool1.Address?.HouseNumber}", ocpiLocation1.Address);
                        Assert.AreEqual(chargingPool1.Address?.City.FirstText(), ocpiLocation1.City);
                        Assert.AreEqual(chargingPool1.Address?.PostalCode, ocpiLocation1.PostalCode);
                        Assert.AreEqual(chargingPool1.Address?.TimeZone?.ToString(), ocpiLocation1.Timezone);

                        Assert.AreEqual(chargingPool1.GeoLocation, ocpiLocation1.Coordinates);

                        Assert.AreEqual(chargingPool1.ChargingWhenClosed, ocpiLocation1.ChargingWhenClosed);

                    }

                }

                Assert.AreEqual(1, cpoAdapter.CommonAPI.GetLocations().Count());

                #endregion

                #region Add DE*GEF*POOL2

                // Will call OCPICSOAdapter.AddStaticData(ChargingPool, ...)!
                var addChargingPoolResult2 = await graphDefinedCSO.CreateChargingPool(

                                                 Id: ChargingPool_Id.Parse("DE*GEF*POOL2"),
                                                 Name: I18NString.Create(Languages.en, "Test pool #2"),
                                                 Description: I18NString.Create(Languages.en, "GraphDefined charging pool for tests #2"),

                                                 Address: new Address(

                                                                           Street: "Biber Weg",
                                                                           PostalCode: "07748",
                                                                           City: I18NString.Create(Languages.de, "Neu-Jena"),
                                                                           Country: Country.Germany,

                                                                           HouseNumber: "18b",
                                                                           FloorLevel: null,
                                                                           Region: null,
                                                                           PostalCodeSub: null,
                                                                           TimeZone: null,
                                                                           OfficialLanguages: null,
                                                                           Comment: null,

                                                                           CustomData: null,
                                                                           InternalData: null

                                                                       ),
                                                 GeoLocation: GeoCoordinate.Parse(50.94, 11.64),

                                                 OpeningTimes: null,
                                                 ChargingWhenClosed: false,

                                                 InitialAdminStatus: ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus: ChargingPoolStatusTypes.Available,

                                                 Configurator: chargingPool =>
                                                 {
                                                 }

                                             );

                Assert.IsNotNull(addChargingPoolResult2);
                Assert.IsTrue(addChargingPoolResult2.IsSuccess);

                var chargingPool2 = addChargingPoolResult2.ChargingPool;
                Assert.IsNotNull(chargingPool2);

                if (chargingPool2 is not null)
                {

                    var ocpiLocation2 = cpoAdapter.CommonAPI.GetLocations().FirstOrDefault(location => location.Name == chargingPool2.Name.FirstText());
                    Assert.IsNotNull(ocpiLocation2);

                    if (ocpiLocation2 is not null)
                    {

                        Assert.AreEqual($"{chargingPool2.Address?.Street} {chargingPool2.Address?.HouseNumber}", ocpiLocation2.Address);
                        Assert.AreEqual(chargingPool2.Address?.City.FirstText(), ocpiLocation2.City);
                        Assert.AreEqual(chargingPool2.Address?.PostalCode, ocpiLocation2.PostalCode);
                        Assert.AreEqual(chargingPool2.Address?.TimeZone?.ToString(), ocpiLocation2.Timezone);

                        Assert.AreEqual(chargingPool2.GeoLocation, ocpiLocation2.Coordinates);

                    }

                }

                Assert.AreEqual(2, cpoAdapter.CommonAPI.GetLocations().Count());

                #endregion


                // OCPI does not have stations!

                #region Add DE*GEF*STATION*1*A

                var addChargingStationResult1 = await chargingPool1!.CreateChargingStation(

                                                    Id: ChargingStation_Id.Parse("DE*GEF*STATION*1*A"),
                                                    Name: I18NString.Create(Languages.en, "Test station #1A"),
                                                    Description: I18NString.Create(Languages.en, "GraphDefined charging station for tests #1A"),

                                                    GeoLocation: GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus: ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus: ChargingStationStatusTypes.Available,

                                                    Configurator: chargingStation =>
                                                    {
                                                    }

                                                );

                Assert.IsNotNull(addChargingStationResult1);
                Assert.IsTrue(addChargingStationResult1.IsSuccess);

                var chargingStation1 = addChargingStationResult1.ChargingStation;
                Assert.IsNotNull(chargingStation1);

                #endregion

                #region Add DE*GEF*STATION*1*B

                var addChargingStationResult2 = await chargingPool1!.CreateChargingStation(

                                                    Id: ChargingStation_Id.Parse("DE*GEF*STATION*1*B"),
                                                    Name: I18NString.Create(Languages.en, "Test station #1B"),
                                                    Description: I18NString.Create(Languages.en, "GraphDefined charging station for tests #1B"),

                                                    GeoLocation: GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus: ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus: ChargingStationStatusTypes.Available,

                                                    Configurator: chargingStation =>
                                                    {
                                                    }

                                                );

                Assert.IsNotNull(addChargingStationResult2);
                Assert.IsTrue(addChargingStationResult2.IsSuccess);

                var chargingStation2 = addChargingStationResult2.ChargingStation;
                Assert.IsNotNull(chargingStation2);

                #endregion

                #region Add DE*GEF*STATION*2*A

                var addChargingStationResult3 = await chargingPool2!.CreateChargingStation(

                                                    Id: ChargingStation_Id.Parse("DE*GEF*STATION*2*A"),
                                                    Name: I18NString.Create(Languages.en, "Test station #2A"),
                                                    Description: I18NString.Create(Languages.en, "GraphDefined charging station for tests #2A"),

                                                    GeoLocation: GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus: ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus: ChargingStationStatusTypes.Available,

                                                    Configurator: chargingStation =>
                                                    {
                                                    }

                                                );

                Assert.IsNotNull(addChargingStationResult3);
                Assert.IsTrue(addChargingStationResult3.IsSuccess);

                var chargingStation3 = addChargingStationResult3.ChargingStation;
                Assert.IsNotNull(chargingStation3);

                #endregion


                #region Add EVSE DE*GEF*EVSE*1*A*1

                // Will call OCPICSOAdapter.AddStaticData(EVSE, ...)!
                var addEVSE1Result1 = await chargingStation1!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*1"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #1A1"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #1A1"),

                                          ChargingConnectors:   new[] {
                                                                      new ChargingConnector(
                                                                          Id:             ChargingConnector_Id.Parse(1),
                                                                          Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                          Lockable:       true,
                                                                          CableAttached:  true,
                                                                          CableLength:    Meter.Parse(3.0)
                                                                      )
                                                                  },

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result1);
                Assert.IsTrue(addEVSE1Result1.IsSuccess);

                var evse1 = addEVSE1Result1.EVSE;
                Assert.IsNotNull(evse1);

                if (evse1 is not null)
                {

                    var ocpiLocation1 = cpoAdapter.CommonAPI.GetLocations().FirstOrDefault(location => location.Name == chargingPool1.Name.FirstText());
                    Assert.IsNotNull(ocpiLocation1);

                    var ocpiEVSE1 = ocpiLocation1!.FirstOrDefault(evse => evse.UId.ToString() == evse1.Id.ToString());
                    Assert.IsNotNull(ocpiEVSE1);

                    if (ocpiEVSE1 is not null)
                    {

                        Assert.AreEqual(ocpiEVSE1.EVSEId.ToString(), evse1.Id.ToString());

                    }

                }

                Assert.AreEqual(1, cpoAdapter.CommonAPI.GetLocations().SelectMany(location => location.EVSEs).Count());

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*A*2

                var addEVSE1Result2 = await chargingStation1!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*2"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #1A2"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #1A2"),

                                          ChargingConnectors:   new[] {
                                                                      new ChargingConnector(
                                                                          Id:             ChargingConnector_Id.Parse(1),
                                                                          Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                          Lockable:       true,
                                                                          CableAttached:  true,
                                                                          CableLength:    Meter.Parse(3.0)
                                                                      )
                                                                  },

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result2);

                var evse2 = addEVSE1Result2.EVSE;
                Assert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*B*1

                var addEVSE1Result3 = await chargingStation2!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*B*1"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #1B1"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #1B1"),

                                          ChargingConnectors:   new[] {
                                                                      new ChargingConnector(
                                                                          Id:             ChargingConnector_Id.Parse(1),
                                                                          Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                          Lockable:       true,
                                                                          CableAttached:  true,
                                                                          CableLength:    Meter.Parse(3.0)
                                                                      )
                                                                  },

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result3);

                var evse3 = addEVSE1Result3.EVSE;
                Assert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*2*A*1

                var addEVSE1Result4 = await chargingStation3!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*2*A*1"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #2A1"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #2A1"),

                                          ChargingConnectors:   new[] {
                                                                      new ChargingConnector(
                                                                          Id:             ChargingConnector_Id.Parse(1),
                                                                          Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                          Lockable:       true,
                                                                          CableAttached:  true,
                                                                          CableLength:    Meter.Parse(3.0)
                                                                      )
                                                                  },

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result4);

                var evse4 = addEVSE1Result4.EVSE;
                Assert.IsNotNull(evse4);

                #endregion


                //ToDo: There seems to be a timing issue!
                await Task.Delay(500);


                #region Validate, that locations had been sent to the CPO OCPI module

                var allLocationsAtCPO = cpoAdapter.CommonAPI.GetLocations().ToArray();
                Assert.IsNotNull(allLocationsAtCPO);
                Assert.AreEqual(2, allLocationsAtCPO.Length);

                #endregion

                #region Validate, that EVSEs had been sent to the CPO OCPI module

                var allEVSEsAtCPO = cpoAdapter.CommonAPI.GetLocations().SelectMany(location => location.EVSEs).ToArray();
                Assert.IsNotNull(allEVSEsAtCPO);
                Assert.AreEqual(4, allEVSEsAtCPO.Length);

                #endregion

                #region Validate, that both CPO locations have EVSEs

                if (cpoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1.Id.ToString()), out var location1) && location1 is not null)
                    Assert.AreEqual(3, location1.EVSEs.Count());
                else
                    Assert.Fail("location1 was not found!");


                if (cpoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool2.Id.ToString()), out var location2) && location2 is not null)
                    Assert.AreEqual(1, location2.EVSEs.Count());
                else
                    Assert.Fail("location2 was not found!");

                #endregion


                #region Validate, that EVSEs had been sent to the 1st e-mobility provider

                //var allEVSEsAtEMSP1  = emsp1Adapter.CommonAPI.GetLocations().SelectMany(location => location.EVSEs).ToArray();
                //Assert.IsNotNull(allEVSEsAtEMSP1);
                //Assert.AreEqual (4, allEVSEsAtEMSP1.Length);

                #endregion

                #region Validate, that EVSEs had been sent to the 2nd e-mobility provider

                //var allEVSEsAtEMSP2  = emsp2Adapter.CommonAPI.GetLocations().SelectMany(location => location.EVSEs).ToArray();
                //Assert.IsNotNull(allEVSEsAtEMSP2);
                //Assert.AreEqual (4, allEVSEsAtEMSP2.Length);

                #endregion


            }
            else
                Assert.Fail("Failed precondition(s)!");

        }

        #endregion

        #region Update_ChargingLocationsAndEVSEs_Test1()

        /// <summary>
        /// Add WWCP charging locations, stations and EVSEs and update their static data.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the OCPI module.
        /// </summary>
        [Test]
        public async Task Update_ChargingLocationsAndEVSEs_Test1()
        {

            if (csoRoamingNetwork  is not null &&
                emp1RoamingNetwork is not null &&
                emp2RoamingNetwork is not null &&

                cpoAdapter         is not null &&

                graphDefinedCSO    is not null)
            {

                #region Add DE*GEF*POOL1

                var addChargingPoolResult1 = await graphDefinedCSO.CreateChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL1"),
                                                 Name:                 I18NString.Create(Languages.en, "Test pool #1"),
                                                 Description:          I18NString.Create(Languages.en, "GraphDefined charging pool for tests #1"),

                                                 Address:              new Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.da, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                Assert.IsNotNull(addChargingPoolResult1);

                var chargingPool1  = addChargingPoolResult1.ChargingPool;
                Assert.IsNotNull(chargingPool1);

                #endregion

                #region Add DE*GEF*POOL2

                var addChargingPoolResult2 = await graphDefinedCSO.CreateChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL2"),
                                                 Name:                 I18NString.Create(Languages.en, "Test pool #2"),
                                                 Description:          I18NString.Create(Languages.en, "GraphDefined charging pool for tests #2"),

                                                 Address:              new Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.da, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                Assert.IsNotNull(addChargingPoolResult2);

                var chargingPool2  = addChargingPoolResult2.ChargingPool;
                Assert.IsNotNull(chargingPool2);

                #endregion


                // OCPI does not have stations!

                #region Add DE*GEF*STATION*1*A

                var addChargingStationResult1 = await chargingPool1!.CreateChargingStation(

                                                    Id:                   ChargingStation_Id.Parse("DE*GEF*STATION*1*A"),
                                                    Name:                 I18NString.Create(Languages.en, "Test station #1A"),
                                                    Description:          I18NString.Create(Languages.en, "GraphDefined charging station for tests #1A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                Assert.IsNotNull(addChargingStationResult1);

                var chargingStation1  = addChargingStationResult1.ChargingStation;
                Assert.IsNotNull(chargingStation1);

                #endregion

                #region Add DE*GEF*STATION*1*B

                var addChargingStationResult2 = await chargingPool1!.CreateChargingStation(

                                                    Id:                   ChargingStation_Id.Parse("DE*GEF*STATION*1*B"),
                                                    Name:                 I18NString.Create(Languages.en, "Test station #1B"),
                                                    Description:          I18NString.Create(Languages.en, "GraphDefined charging station for tests #1B"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                Assert.IsNotNull(addChargingStationResult2);

                var chargingStation2  = addChargingStationResult2.ChargingStation;
                Assert.IsNotNull(chargingStation2);

                #endregion

                #region Add DE*GEF*STATION*2*A

                var addChargingStationResult3 = await chargingPool2!.CreateChargingStation(

                                                    Id:                   ChargingStation_Id.Parse("DE*GEF*STATION*2*A"),
                                                    Name:                 I18NString.Create(Languages.en, "Test station #2A"),
                                                    Description:          I18NString.Create(Languages.en, "GraphDefined charging station for tests #2A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                Assert.IsNotNull(addChargingStationResult3);

                var chargingStation3  = addChargingStationResult3.ChargingStation;
                Assert.IsNotNull(chargingStation3);

                #endregion


                #region Add EVSE DE*GEF*EVSE*1*A*1

                var addEVSE1Result1 = await chargingStation1!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*1"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #1A1"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #1A1"),

                                          ChargingConnectors:   new[] {
                                                                      new ChargingConnector(
                                                                          Id:             ChargingConnector_Id.Parse(1),
                                                                          Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                          Lockable:       true,
                                                                          CableAttached:  true,
                                                                          CableLength:    Meter.Parse(3.0)
                                                                      )
                                                                  },

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result1);

                var evse1     = addEVSE1Result1.EVSE;
                Assert.IsNotNull(evse1);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*A*2

                var addEVSE1Result2 = await chargingStation1!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*2"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #1A2"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #1A2"),

                                          ChargingConnectors:   new[] {
                                                                      new ChargingConnector(
                                                                          Id:             ChargingConnector_Id.Parse(1),
                                                                          Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                          Lockable:       true,
                                                                          CableAttached:  true,
                                                                          CableLength:    Meter.Parse(3.0)
                                                                      )
                                                                  },

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result2);

                var evse2     = addEVSE1Result2.EVSE;
                Assert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*B*1

                var addEVSE1Result3 = await chargingStation2!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*B*1"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #1B1"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #1B1"),

                                          ChargingConnectors:   new[] {
                                                                      new ChargingConnector(
                                                                          Id:             ChargingConnector_Id.Parse(1),
                                                                          Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                          Lockable:       true,
                                                                          CableAttached:  true,
                                                                          CableLength:    Meter.Parse(3.0)
                                                                      )
                                                                  },

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result3);

                var evse3     = addEVSE1Result3.EVSE;
                Assert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*2*A*1

                var addEVSE1Result4 = await chargingStation3!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*2*A*1"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #2A1"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #2A1"),

                                          ChargingConnectors:   new[] {
                                                                      new ChargingConnector(
                                                                          Id:             ChargingConnector_Id.Parse(1),
                                                                          Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                          Lockable:       true,
                                                                          CableAttached:  true,
                                                                          CableLength:    Meter.Parse(3.0)
                                                                      )
                                                                  },

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result4);

                var evse4     = addEVSE1Result4.EVSE;
                Assert.IsNotNull(evse4);

                #endregion


                //ToDo: There seems to be a timing issue!
                await Task.Delay(500);


                #region Validate, that locations had been sent to the CPO OCPI module

                var allLocations  = cpoAdapter.CommonAPI.GetLocations().ToArray();
                Assert.IsNotNull(allLocations);
                Assert.AreEqual (2, allLocations.Length);

                #endregion

                #region Validate, that EVSEs had been sent to the CPO OCPI module

                var allEVSEs      = cpoAdapter.CommonAPI.GetLocations().SelectMany(location => location.EVSEs).ToArray();
                Assert.IsNotNull(allEVSEs);
                Assert.AreEqual (4, allEVSEs.Length);

                #endregion

                #region Validate, that both locations have EVSEs

                if (cpoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1.Id.ToString()), out var location1) && location1 is not null)
                    Assert.AreEqual(3, location1.EVSEs.Count());
                else
                    Assert.Fail("location1 was not found!");


                if (cpoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool2.Id.ToString()), out var location2) && location2 is not null)
                    Assert.AreEqual(1, location2.EVSEs.Count());
                else
                    Assert.Fail("location2 was not found!");

                #endregion



                #region Update Add DE*GEF*POOL2, DE*GEF*STATION*2*A, DE*GEF*POOL2

                var updatedPoolProperties     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedStationProperties  = new List<PropertyUpdateInfo<ChargingStation_Id>>();
                var updatedEVSEProperties     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();

                #region Subscribe charging pool events

                chargingPool1!.OnPropertyChanged += (timestamp,
                                                     eventTrackingId,
                                                     chargingPool,
                                                     propertyName,
                                                     newValue,
                                                     oldValue,
                                                     dataSource) => {

                    updatedPoolProperties.Add(new PropertyUpdateInfo<ChargingPool_Id>((chargingPool as IChargingPool)!.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                chargingPool1!.OnDataChanged += (timestamp,
                                                 eventTrackingId,
                                                 chargingPool,
                                                 propertyName,
                                                 newValue,
                                                 oldValue,
                                                 dataSource) => {

                    updatedPoolProperties.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingPoolDataChanged += (timestamp,
                                                              eventTrackingId,
                                                              chargingPool,
                                                              propertyName,
                                                              newValue,
                                                              oldValue,
                                                              dataSource) => {

                    updatedPoolProperties.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                csoRoamingNetwork.OnChargingPoolDataChanged  += (timestamp,
                                                              eventTrackingId,
                                                              chargingPool,
                                                              propertyName,
                                                              newValue,
                                                              oldValue,
                                                              dataSource) => {

                    updatedPoolProperties.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                #endregion

                #region Subscribe charging station events

                chargingStation1!.OnPropertyChanged += (timestamp,
                                                        eventTrackingId,
                                                        chargingStation,
                                                        propertyName,
                                                        newValue,
                                                        oldValue,
                                                        dataSource) => {

                    updatedStationProperties.Add(new PropertyUpdateInfo<ChargingStation_Id>((chargingStation as IChargingStation)!.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                chargingStation1!.OnDataChanged += (timestamp,
                                                    eventTrackingId,
                                                    chargingStation,
                                                    propertyName,
                                                    newValue,
                                                    oldValue,
                                                    dataSource) => {

                    updatedStationProperties.Add(new PropertyUpdateInfo<ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingStationDataChanged += (timestamp,
                                                                 eventTrackingId,
                                                                 chargingStation,
                                                                 propertyName,
                                                                 newValue,
                                                                 oldValue,
                                                                 dataSource) => {

                    updatedStationProperties.Add(new PropertyUpdateInfo<ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                csoRoamingNetwork.OnChargingStationDataChanged += (timestamp,
                                                                   eventTrackingId,
                                                                   chargingStation,
                                                                   propertyName,
                                                                   newValue,
                                                                   oldValue,
                                                                   dataSource) => {

                    updatedStationProperties.Add(new PropertyUpdateInfo<ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                #endregion

                #region Subscribe EVSE events

                evse1!.OnPropertyChanged += (timestamp,
                                             eventTrackingId,
                                             evse,
                                             propertyName,
                                             newValue,
                                             oldValue,
                                             dataSource) => {

                    updatedEVSEProperties.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>((evse as IEVSE)!.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                evse1!.OnDataChanged += (timestamp,
                                         eventTrackingId,
                                         evse,
                                         propertyName,
                                         newValue,
                                         oldValue,
                                         dataSource) => {

                    updatedEVSEProperties.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnEVSEDataChanged += (timestamp,
                                                      eventTrackingId,
                                                      evse,
                                                      propertyName,
                                                      newValue,
                                                      oldValue,
                                                      dataSource) => {

                    updatedEVSEProperties.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                csoRoamingNetwork.OnEVSEDataChanged += (timestamp,
                                                        eventTrackingId,
                                                        evse,
                                                        propertyName,
                                                        newValue,
                                                        oldValue,
                                                        dataSource) => {

                    updatedEVSEProperties.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                #endregion

                cpoAdapter.CommonAPI.OnLocationChanged += async (location) => { };
                cpoAdapter.CommonAPI.OnEVSEChanged     += async (evse)     => { };


                chargingPool1!.Name.       Set(Languages.en, "Test pool #1 (updated)");
                chargingPool1!.Description.Set(Languages.en, "GraphDefined charging pool for tests #1 (updated)");

                Assert.AreEqual(8, updatedPoolProperties.Count);
                Assert.AreEqual("Test pool #1 (updated)",                             graphDefinedCSO.GetChargingPoolById(chargingPool1!.Id)!.Name       [Languages.en]);
                Assert.AreEqual("GraphDefined charging pool for tests #1 (updated)",  graphDefinedCSO.GetChargingPoolById(chargingPool1!.Id)!.Description[Languages.en]);


                //ToDo: There seems to be a timing issue!
                await Task.Delay(500);


                cpoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1!.Id.ToString()), out var location);
                Assert.AreEqual("Test pool #1 (updated)",                             location.Name);
                //Assert.AreEqual("GraphDefined Charging Pool für Tests #1",            location!.Name); // Not mapped to OCPI!


                evse1.Name.Set(Languages.en, "Test EVSE #1A1 (updated)");















                //var updateChargingPoolResult2 = await graphDefinedCSO.UpdateChargingPool()


                //var updateChargingPoolResult2 = await chargingPool2.Address = new Address(

                //                                                                  Street:             "Biberweg",
                //                                                                  PostalCode:         "07749",
                //                                                                  City:               I18NString.Create(Languages.da, "Jena"),
                //                                                                  Country:            Country.Germany,

                //                                                                  HouseNumber:        "18",
                //                                                                  FloorLevel:         null,
                //                                                                  Region:             null,
                //                                                                  PostalCodeSub:      null,
                //                                                                  TimeZone:           null,
                //                                                                  OfficialLanguages:  null,
                //                                                                  Comment:            null,

                //                                                                  CustomData:         null,
                //                                                                  InternalData:       null

                //                                                              );

                //Assert.IsNotNull(addChargingPoolResult2);

                //var chargingPool2  = addChargingPoolResult2.ChargingPool;
                //Assert.IsNotNull(chargingPool2);

                #endregion



            }
            else
                Assert.Fail("Failed precondition(s)!");

        }

        #endregion

        #region Update_EVSEStatus_Test1()

        /// <summary>
        /// Add WWCP charging locations, stations and EVSEs and update the EVSE status.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the OCPI module.
        /// </summary>
        [Test]
        public async Task Update_EVSEStatus_Test1()
        {

            if (csoRoamingNetwork  is not null &&
                emp1RoamingNetwork is not null &&
                emp2RoamingNetwork is not null &&

                graphDefinedCSO    is not null)
            {

                #region Add DE*GEF*POOL1

                var addChargingPoolResult1 = await graphDefinedCSO.CreateChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL1"),
                                                 Name:                 I18NString.Create(Languages.en, "Test pool #1"),
                                                 Description:          I18NString.Create(Languages.en, "GraphDefined charging pool for tests #1"),

                                                 Address:              new Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.da, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                Assert.IsNotNull(addChargingPoolResult1);

                var chargingPool1  = addChargingPoolResult1.ChargingPool;
                Assert.IsNotNull(chargingPool1);

                #endregion

                #region Add DE*GEF*POOL2

                var addChargingPoolResult2 = await graphDefinedCSO.CreateChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL2"),
                                                 Name:                 I18NString.Create(Languages.en, "Test pool #2"),
                                                 Description:          I18NString.Create(Languages.en, "GraphDefined charging pool for tests #2"),

                                                 Address:              new Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.da, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                Assert.IsNotNull(addChargingPoolResult2);

                var chargingPool2  = addChargingPoolResult2.ChargingPool;
                Assert.IsNotNull(chargingPool2);

                #endregion


                // OCPI does not have stations!

                #region Add DE*GEF*STATION*1*A

                var addChargingStationResult1 = await chargingPool1!.CreateChargingStation(

                                                    Id:                   ChargingStation_Id.Parse("DE*GEF*STATION*1*A"),
                                                    Name:                 I18NString.Create(Languages.en, "Test station #1A"),
                                                    Description:          I18NString.Create(Languages.en, "GraphDefined charging station for tests #1A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                Assert.IsNotNull(addChargingStationResult1);

                var chargingStation1  = addChargingStationResult1.ChargingStation;
                Assert.IsNotNull(chargingStation1);

                #endregion

                #region Add DE*GEF*STATION*1*B

                var addChargingStationResult2 = await chargingPool1!.CreateChargingStation(

                                                    Id:                   ChargingStation_Id.Parse("DE*GEF*STATION*1*B"),
                                                    Name:                 I18NString.Create(Languages.en, "Test station #1B"),
                                                    Description:          I18NString.Create(Languages.en, "GraphDefined charging station for tests #1B"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                Assert.IsNotNull(addChargingStationResult2);

                var chargingStation2  = addChargingStationResult2.ChargingStation;
                Assert.IsNotNull(chargingStation2);

                #endregion

                #region Add DE*GEF*STATION*2*A

                var addChargingStationResult3 = await chargingPool2!.CreateChargingStation(

                                                    Id:                   ChargingStation_Id.Parse("DE*GEF*STATION*2*A"),
                                                    Name:                 I18NString.Create(Languages.en, "Test station #2A"),
                                                    Description:          I18NString.Create(Languages.en, "GraphDefined charging station for tests #2A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                Assert.IsNotNull(addChargingStationResult3);

                var chargingStation3  = addChargingStationResult3.ChargingStation;
                Assert.IsNotNull(chargingStation3);

                #endregion


                #region Add EVSE DE*GEF*EVSE*1*A*1

                var addEVSE1Result1 = await chargingStation1!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*1"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #1A1"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #1A1"),

                                          ChargingConnectors:   new[] {
                                                                      new ChargingConnector(
                                                                          Id:             ChargingConnector_Id.Parse(1),
                                                                          Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                          Lockable:       true,
                                                                          CableAttached:  true,
                                                                          CableLength:    Meter.Parse(3.0)
                                                                      )
                                                                  },

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result1);

                var evse1     = addEVSE1Result1.EVSE;
                Assert.IsNotNull(evse1);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*A*2

                var addEVSE1Result2 = await chargingStation1!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*2"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #1A2"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #1A2"),

                                          ChargingConnectors:   new[] {
                                                                      new ChargingConnector(
                                                                          Id:             ChargingConnector_Id.Parse(1),
                                                                          Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                          Lockable:       true,
                                                                          CableAttached:  true,
                                                                          CableLength:    Meter.Parse(3.0)
                                                                      )
                                                                  },

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result2);

                var evse2     = addEVSE1Result2.EVSE;
                Assert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*B*1

                var addEVSE1Result3 = await chargingStation2!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*B*1"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #1B1"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #1B1"),

                                          ChargingConnectors:   new[] {
                                                                      new ChargingConnector(
                                                                          Id:             ChargingConnector_Id.Parse(1),
                                                                          Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                          Lockable:       true,
                                                                          CableAttached:  true,
                                                                          CableLength:    Meter.Parse(3.0)
                                                                      )
                                                                  },

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result3);

                var evse3     = addEVSE1Result3.EVSE;
                Assert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*2*A*1

                var addEVSE1Result4 = await chargingStation3!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*2*A*1"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #2A1"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #2A1"),

                                          ChargingConnectors:   new[] {
                                                                      new ChargingConnector(
                                                                          Id:             ChargingConnector_Id.Parse(1),
                                                                          Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                          Lockable:       true,
                                                                          CableAttached:  true,
                                                                          CableLength:    Meter.Parse(3.0)
                                                                      )
                                                                  },

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result4);

                var evse4     = addEVSE1Result4.EVSE;
                Assert.IsNotNull(evse4);

                #endregion


                //ToDo: There seems to be a timing issue!
                await Task.Delay(500);


                #region Validate, that locations had been sent to the CPO OCPI module

                var allLocations  = cpoAdapter.CommonAPI.GetLocations().ToArray();
                Assert.IsNotNull(allLocations);
                Assert.AreEqual (2, allLocations.Length);

                #endregion

                #region Validate, that EVSEs had been sent to the CPO OCPI module

                var allEVSEs      = cpoAdapter.CommonAPI.GetLocations().SelectMany(location => location.EVSEs).ToArray();
                Assert.IsNotNull(allEVSEs);
                Assert.AreEqual (4, allEVSEs.Length);

                #endregion

                #region Validate, that both locations have EVSEs

                if (cpoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1.Id.ToString()), out var location1) && location1 is not null)
                    Assert.AreEqual(3, location1.EVSEs.Count());
                else
                    Assert.Fail("location1 was not found!");


                if (cpoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool2.Id.ToString()), out var location2) && location2 is not null)
                    Assert.AreEqual(1, location2.EVSEs.Count());
                else
                    Assert.Fail("location2 was not found!");

                #endregion



                var evse1_UId = evse1!.Id.ToOCPI_EVSEUId();
                Assert.IsTrue(evse1_UId.HasValue);

                #region Subscribe WWCP EVSE events

                var updatedEVSEStatus         = new List<EVSEStatusUpdate>();

                evse1!.OnStatusChanged += (timestamp,
                                           eventTrackingId,
                                           evse,
                                           newValue,
                                           oldValue,
                                           dataSource) => {

                    updatedEVSEStatus.Add(new EVSEStatusUpdate(evse.Id, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnEVSEStatusChanged += (timestamp,
                                                        eventTrackingId,
                                                        evse,
                                                        newEVSEStatus,
                                                        oldEVSEStatus,
                                                        dataSource) => {

                    updatedEVSEStatus.Add(new EVSEStatusUpdate(evse.Id, newEVSEStatus, oldEVSEStatus, dataSource));
                    return Task.CompletedTask;

                };

                csoRoamingNetwork.OnEVSEStatusChanged += (timestamp,
                                                          eventTrackingId,
                                                          evse,
                                                          newEVSEStatus,
                                                          oldEVSEStatus,
                                                          dataSource) => {

                    updatedEVSEStatus.Add(new EVSEStatusUpdate(evse.Id, newEVSEStatus, oldEVSEStatus, dataSource));
                    return Task.CompletedTask;

                };

                #endregion

                #region Subscribe OCPI EVSE events

                var updatedOCPIEVSEStatus = new List<StatusType>();

                cpoAdapter.CommonAPI.OnEVSEChanged += (evse) => {

                    updatedOCPIEVSEStatus.Add(evse.Status);
                    return Task.CompletedTask;

                };

                cpoAdapter.CommonAPI.OnEVSEStatusChanged += (timestamp,
                                                             evse,
                                                             newEVSEStatus,
                                                             oldEVSEStatus) => {

                    updatedOCPIEVSEStatus.Add(newEVSEStatus);
                    return Task.CompletedTask;

                };

                #endregion

                #region Update

                {
                    if (evse1_UId.HasValue &&
                        cpoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1!.Id.Suffix), out var location) && location is not null &&
                        location.TryGetEVSE(evse1_UId.Value, out var ocpiEVSE) && ocpiEVSE is not null)
                    {
                        Assert.AreEqual(StatusType.AVAILABLE, ocpiEVSE.Status);
                    }
                }



                evse1.SetStatus(EVSEStatusTypes.Charging);


                //ToDo: There seems to be a timing issue!
                await Task.Delay(500);


                Assert.AreEqual(3, updatedEVSEStatus.    Count);
                Assert.AreEqual(2, updatedOCPIEVSEStatus.Count);

                Assert.AreEqual(EVSEStatusTypes.Charging,  graphDefinedCSO.GetEVSEById(evse1!.Id).Status.Value);

                {
                    if (evse1_UId.HasValue &&
                        cpoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1!.Id.Suffix), out var location) && location is not null &&
                        location.TryGetEVSE(evse1_UId.Value, out var ocpiEVSE) && ocpiEVSE is not null)
                    {
                        Assert.AreEqual(StatusType.CHARGING, ocpiEVSE.Status);
                    }
                }

                #endregion


            }
            else
                Assert.Fail("Failed precondition(s)!");

        }

        #endregion


        #region AuthStart_Test1()

        /// <summary>
        /// Add WWCP charging locations, stations and EVSEs and update the EVSE status.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the OCPI module.
        /// </summary>
        [Test]
        public async Task AuthStart_Test1()
        {

            if (csoRoamingNetwork  is not null &&
                emp1RoamingNetwork is not null &&
                emp2RoamingNetwork is not null &&

                cpoCPOAPI          is not null &&
                emsp1EMSPAPI        is not null &&
                emsp2EMSPAPI        is not null &&

                graphDefinedCSO    is not null)
                //graphDefinedEMP    is not null &&
                //exampleEMP         is not null)
            {

                #region Add DE*GEF*POOL1

                var addChargingPoolResult1 = await graphDefinedCSO.CreateChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL1"),
                                                 Name:                 I18NString.Create(Languages.en, "Test pool #1"),
                                                 Description:          I18NString.Create(Languages.en, "GraphDefined charging pool for tests #1"),

                                                 Address:              new Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.da, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                Assert.IsNotNull(addChargingPoolResult1);

                var chargingPool1  = addChargingPoolResult1.ChargingPool;
                Assert.IsNotNull(chargingPool1);

                #endregion

                #region Add DE*GEF*POOL2

                var addChargingPoolResult2 = await graphDefinedCSO.CreateChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL2"),
                                                 Name:                 I18NString.Create(Languages.en, "Test pool #2"),
                                                 Description:          I18NString.Create(Languages.en, "GraphDefined charging pool for tests #2"),

                                                 Address:              new Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.da, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                Assert.IsNotNull(addChargingPoolResult2);

                var chargingPool2  = addChargingPoolResult2.ChargingPool;
                Assert.IsNotNull(chargingPool2);

                #endregion


                // OCPI does not have stations!

                #region Add DE*GEF*STATION*1*A

                var addChargingStationResult1 = await chargingPool1!.CreateChargingStation(

                                                    Id:                   ChargingStation_Id.Parse("DE*GEF*STATION*1*A"),
                                                    Name:                 I18NString.Create(Languages.en, "Test station #1A"),
                                                    Description:          I18NString.Create(Languages.en, "GraphDefined charging station for tests #1A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                Assert.IsNotNull(addChargingStationResult1);

                var chargingStation1  = addChargingStationResult1.ChargingStation;
                Assert.IsNotNull(chargingStation1);

                #endregion

                #region Add DE*GEF*STATION*1*B

                var addChargingStationResult2 = await chargingPool1!.CreateChargingStation(

                                                    Id:                   ChargingStation_Id.Parse("DE*GEF*STATION*1*B"),
                                                    Name:                 I18NString.Create(Languages.en, "Test station #1B"),
                                                    Description:          I18NString.Create(Languages.en, "GraphDefined charging station for tests #1B"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                Assert.IsNotNull(addChargingStationResult2);

                var chargingStation2  = addChargingStationResult2.ChargingStation;
                Assert.IsNotNull(chargingStation2);

                #endregion

                #region Add DE*GEF*STATION*2*A

                var addChargingStationResult3 = await chargingPool2!.CreateChargingStation(

                                                    Id:                   ChargingStation_Id.Parse("DE*GEF*STATION*2*A"),
                                                    Name:                 I18NString.Create(Languages.en, "Test station #2A"),
                                                    Description:          I18NString.Create(Languages.en, "GraphDefined charging station for tests #2A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                Assert.IsNotNull(addChargingStationResult3);

                var chargingStation3  = addChargingStationResult3.ChargingStation;
                Assert.IsNotNull(chargingStation3);

                #endregion


                #region Add EVSE DE*GEF*EVSE*1*A*1

                var addEVSE1Result1 = await chargingStation1!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*1"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #1A1"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #1A1"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result1);

                var evse1     = addEVSE1Result1.EVSE;
                Assert.IsNotNull(evse1);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*A*2

                var addEVSE1Result2 = await chargingStation1!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*2"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #1A2"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #1A2"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result2);

                var evse2     = addEVSE1Result2.EVSE;
                Assert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*B*1

                var addEVSE1Result3 = await chargingStation2!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*B*1"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #1B1"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #1B1"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result3);

                var evse3     = addEVSE1Result3.EVSE;
                Assert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*2*A*1

                var addEVSE1Result4 = await chargingStation3!.CreateEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*2*A*1"),
                                          Name:                 I18NString.Create(Languages.en, "Test EVSE #2A1"),
                                          Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #2A1"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusTypes.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.IsNotNull(addEVSE1Result4);

                var evse4     = addEVSE1Result4.EVSE;
                Assert.IsNotNull(evse4);

                #endregion


                emsp1EMSPAPI.OnRFIDAuthToken += async (countryCode,
                                                       partyId,
                                                       tokenId,
                                                       locationReference) => {

                    return tokenId.ToString() == "11223344"

                               ? new AuthorizationInfo(
                                     Allowed:       AllowedType.ALLOWED,
                                     Location:      locationReference,
                                     Info:          DisplayText.Create(Languages.en, "Hello world!"),
                                     RemoteParty:   null,
                                     Runtime:       TimeSpan.FromMilliseconds(23.5)
                                 )

                               : new AuthorizationInfo(
                                     Allowed:       AllowedType.NOT_ALLOWED,
                                     Location:      locationReference,
                                     Info:          DisplayText.Create(Languages.en, "Go away!"),
                                     RemoteParty:   null,
                                     Runtime:       TimeSpan.FromMilliseconds(42)
                                 );

                };

                emsp2EMSPAPI.OnRFIDAuthToken += async (countryCode,
                                                       partyId,
                                                       tokenId,
                                                       locationReference) => {

                    return tokenId.ToString() == "55667788"

                               ? new AuthorizationInfo(
                                     Allowed:       AllowedType.ALLOWED,
                                     Location:      locationReference,
                                     Info:          DisplayText.Create(Languages.en, "Hello world!"),
                                     RemoteParty:   null,
                                     Runtime:       TimeSpan.FromMilliseconds(23.5)
                                 )

                               : new AuthorizationInfo(
                                     Allowed:       AllowedType.NOT_ALLOWED,
                                     Location:      locationReference,
                                     Info:          DisplayText.Create(Languages.en, "Go away!"),
                                     RemoteParty:   null,
                                     Runtime:       TimeSpan.FromMilliseconds(42)
                                 );

                };


                var authStartResult1 = await csoRoamingNetwork.AuthorizeStart(
                                                 LocalAuthentication: LocalAuthentication.FromAuthToken(AuthenticationToken.NewRandom7Bytes),
                                                 ChargingLocation:    ChargingLocation.   FromEVSEId   (evse1!.Id),
                                                 ChargingProduct:     ChargingProduct.    FromId       (ChargingProduct_Id.Parse("AC1"))
                                             );

                var authStartResult2 = await csoRoamingNetwork.AuthorizeStart(
                                                 LocalAuthentication: LocalAuthentication.FromAuthToken(AuthenticationToken.Parse("11223344")),
                                                 ChargingLocation:    ChargingLocation.   FromEVSEId   (evse1!.Id),
                                                 ChargingProduct:     ChargingProduct.    FromId       (ChargingProduct_Id.Parse("AC1"))
                                             );

                var authStartResult3 = await csoRoamingNetwork.AuthorizeStart(
                                                 LocalAuthentication: LocalAuthentication.FromAuthToken(AuthenticationToken.Parse("55667788")),
                                                 ChargingLocation:    ChargingLocation.   FromEVSEId   (evse1!.Id),
                                                 ChargingProduct:     ChargingProduct.    FromId       (ChargingProduct_Id.Parse("AC1"))
                                             );

                Assert.AreEqual(AuthStartResultTypes.NotAuthorized, authStartResult1.Result);
                Assert.AreEqual(AuthStartResultTypes.Authorized,    authStartResult2.Result);
                Assert.AreEqual(AuthStartResultTypes.Authorized,    authStartResult3.Result);

            }
            else
                Assert.Fail("Failed precondition(s)!");

        }

        #endregion


    }

}
