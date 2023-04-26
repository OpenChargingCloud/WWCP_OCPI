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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.RoamingTests
{

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

            if (csoRoamingNetwork  is not null &&
                emp1RoamingNetwork is not null &&
                emp2RoamingNetwork is not null &&

                graphDefinedCSO    is not null &&
                graphDefinedEMP    is not null &&
                exampleEMP         is not null)
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


                #region Validate, that locations had been sent to the e-mobility providers

                //var allLocations  = cpoCommonAPI.GetLocations().ToArray();
                //Assert.IsNotNull(allLocations);
                //Assert.AreEqual (2, allLocations.Length);

                #endregion

                #region Validate, that EVSEs had been sent to the e-mobility providers

                //var allEVSEs      = cpoCommonAPI.GetLocations().SelectMany(location => location.EVSEs).ToArray();
                //Assert.IsNotNull(allEVSEs);
                //Assert.AreEqual (4, allEVSEs.Length);

                #endregion

            }

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

                graphDefinedCSO    is not null &&
                graphDefinedEMP    is not null &&
                exampleEMP         is not null)
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



                #region Validate, that locations had been sent to the OCPI module

                var allLocations  = cpoCommonAPI.GetLocations().ToArray();
                Assert.IsNotNull(allLocations);
                Assert.AreEqual (2, allLocations.Length);

                #endregion

                #region Validate, that EVSEs had been sent to the OCPI module

                var allEVSEs      = cpoCommonAPI.GetLocations().SelectMany(location => location.EVSEs).ToArray();
                Assert.IsNotNull(allEVSEs);
                Assert.AreEqual (4, allEVSEs.Length);

                #endregion

                #region Validate, that both locations have EVSEs

                if (cpoCommonAPI.TryGetLocation(CountryCode.Parse(chargingPool1.Operator?.Id.CountryCode.Alpha2Code ?? ""),
                                                Party_Id.   Parse(chargingPool1.Operator?.Id.Suffix                 ?? ""),
                                                Location_Id.Parse(chargingPool1.Id.Suffix),
                                                out var location1) &&
                    location1 is not null)
                {

                    Assert.AreEqual(3, location1.EVSEs.Count());

                }
                else
                    Assert.Fail("location1 was not found!");


                if (cpoCommonAPI.TryGetLocation(CountryCode.Parse(chargingPool2.Operator?.Id.CountryCode.Alpha2Code ?? ""),
                                                Party_Id.   Parse(chargingPool2.Operator?.Id.Suffix                 ?? ""),
                                                Location_Id.Parse(chargingPool2.Id.Suffix),
                                                out var location2) &&
                    location2 is not null)
                {

                    Assert.AreEqual(1, location2.EVSEs.Count());

                }
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
                                                     oldValue,
                                                     newValue) => {

                    updatedPoolProperties.Add(new PropertyUpdateInfo<ChargingPool_Id>((chargingPool as IChargingPool)!.Id, propertyName, oldValue, newValue));
                    return Task.CompletedTask;

                };

                chargingPool1!.OnDataChanged += (timestamp,
                                                 eventTrackingId,
                                                 chargingPool,
                                                 propertyName,
                                                 oldValue,
                                                 newValue) => {

                    updatedPoolProperties.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, oldValue, newValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingPoolDataChanged += (timestamp,
                                                              eventTrackingId,
                                                              chargingPool,
                                                              propertyName,
                                                              oldValue,
                                                              newValue) => {

                    updatedPoolProperties.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, oldValue, newValue));
                    return Task.CompletedTask;

                };

                csoRoamingNetwork.OnChargingPoolDataChanged  += (timestamp,
                                                              eventTrackingId,
                                                              chargingPool,
                                                              propertyName,
                                                              oldValue,
                                                              newValue) => {

                    updatedPoolProperties.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, oldValue, newValue));
                    return Task.CompletedTask;

                };

                #endregion

                #region Subscribe charging station events

                chargingStation1!.OnPropertyChanged += (timestamp,
                                                        eventTrackingId,
                                                        chargingStation,
                                                        propertyName,
                                                        oldValue,
                                                        newValue) => {

                    updatedStationProperties.Add(new PropertyUpdateInfo<ChargingStation_Id>((chargingStation as IChargingStation)!.Id, propertyName, oldValue, newValue));
                    return Task.CompletedTask;

                };

                chargingStation1!.OnDataChanged += (timestamp,
                                                    eventTrackingId,
                                                    chargingStation,
                                                    propertyName,
                                                    oldValue,
                                                    newValue) => {

                    updatedStationProperties.Add(new PropertyUpdateInfo<ChargingStation_Id>(chargingStation.Id, propertyName, oldValue, newValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingStationDataChanged += (timestamp,
                                                                 eventTrackingId,
                                                                 chargingStation,
                                                                 propertyName,
                                                                 oldValue,
                                                                 newValue) => {

                    updatedStationProperties.Add(new PropertyUpdateInfo<ChargingStation_Id>(chargingStation.Id, propertyName, oldValue, newValue));
                    return Task.CompletedTask;

                };

                csoRoamingNetwork.OnChargingStationDataChanged += (timestamp,
                                                                eventTrackingId,
                                                                chargingStation,
                                                                propertyName,
                                                                oldValue,
                                                                newValue) => {

                    updatedStationProperties.Add(new PropertyUpdateInfo<ChargingStation_Id>(chargingStation.Id, propertyName, oldValue, newValue));
                    return Task.CompletedTask;

                };

                #endregion

                #region Subscribe EVSE events

                evse1!.OnPropertyChanged += (timestamp,
                                             eventTrackingId,
                                             evse,
                                             propertyName,
                                             oldValue,
                                             newValue) => {

                    updatedEVSEProperties.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>((evse as IEVSE)!.Id, propertyName, oldValue, newValue));
                    return Task.CompletedTask;

                };

                evse1!.OnDataChanged += (timestamp,
                                         eventTrackingId,
                                         evse,
                                         propertyName,
                                         oldValue,
                                         newValue) => {

                    updatedEVSEProperties.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, oldValue, newValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnEVSEDataChanged += (timestamp,
                                                      eventTrackingId,
                                                      evse,
                                                      propertyName,
                                                      oldValue,
                                                      newValue) => {

                    updatedEVSEProperties.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, oldValue, newValue));
                    return Task.CompletedTask;

                };

                csoRoamingNetwork.OnEVSEDataChanged += (timestamp,
                                                     eventTrackingId,
                                                     evse,
                                                     propertyName,
                                                     oldValue,
                                                     newValue) => {

                    updatedEVSEProperties.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, oldValue, newValue));
                    return Task.CompletedTask;

                };

                #endregion

                cpoCommonAPI.OnLocationChanged += async (location) => { };
                cpoCommonAPI.OnEVSEChanged     += async (evse)     => { };


                chargingPool1!.Name.       Set(Languages.en, "Test pool #1 (updated)");
                chargingPool1!.Description.Set(Languages.en, "GraphDefined charging pool for tests #1 (updated)");

                Assert.AreEqual(8, updatedPoolProperties.Count);
                Assert.AreEqual("Test pool #1 (updated)",                             graphDefinedCSO.GetChargingPoolById(chargingPool1!.Id)!.Name       [Languages.en]);
                Assert.AreEqual("GraphDefined charging pool for tests #1 (updated)",  graphDefinedCSO.GetChargingPoolById(chargingPool1!.Id)!.Description[Languages.en]);

                cpoCommonAPI.TryGetLocation(CountryCode.Parse(chargingPool1.Operator?.Id.CountryCode.Alpha2Code ?? ""),
                                            Party_Id.   Parse(chargingPool1.Operator?.Id.Suffix                 ?? ""),
                                            Location_Id.Parse(chargingPool1!.Id.Suffix),
                                            out var location);

                Assert.AreEqual("Test pool #1 (updated)",                             location!.Name);
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

                graphDefinedCSO    is not null &&
                graphDefinedEMP    is not null &&
                exampleEMP         is not null)
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


                var evse1_UId = evse1!.Id.ToOCPI_EVSEUId();
                Assert.IsTrue(evse1_UId.HasValue);


                #region Validate, that locations had been sent to the OCPI module

                var allLocations  = cpoCommonAPI.GetLocations().ToArray();
                Assert.IsNotNull(allLocations);
                Assert.AreEqual (2, allLocations.Length);

                #endregion

                #region Validate, that EVSEs had been sent to the OCPI module

                var allEVSEs      = cpoCommonAPI.GetLocations().SelectMany(location => location.EVSEs).ToArray();
                Assert.IsNotNull(allEVSEs);
                Assert.AreEqual (4, allEVSEs.Length);

                #endregion

                #region Validate, that both locations have EVSEs

                if (cpoCommonAPI.TryGetLocation(CountryCode.Parse(chargingPool1.Operator?.Id.CountryCode.Alpha2Code ?? ""),
                                                        Party_Id.   Parse(chargingPool1.Operator?.Id.Suffix                 ?? ""),
                                                        Location_Id.Parse(chargingPool1.Id.Suffix),
                                                        out var location1) &&
                    location1 is not null)
                {

                    Assert.AreEqual(3, location1.EVSEs.Count());

                }
                else
                    Assert.Fail("location1 was not found!");


                if (cpoCommonAPI.TryGetLocation(CountryCode.Parse(chargingPool2.Operator?.Id.CountryCode.Alpha2Code ?? ""),
                                                        Party_Id.   Parse(chargingPool2.Operator?.Id.Suffix                 ?? ""),
                                                        Location_Id.Parse(chargingPool2.Id.Suffix),
                                                        out var location2) &&
                    location2 is not null)
                {

                    Assert.AreEqual(1, location2.EVSEs.Count());

                }
                else
                    Assert.Fail("location2 was not found!");

                #endregion



                #region Subscribe WWCP EVSE events

                var updatedEVSEStatus         = new List<EVSEStatusUpdate>();

                evse1!.OnStatusChanged += (timestamp,
                                           eventTrackingId,
                                           evse,
                                           oldEVSEStatus,
                                           newEVSEStatus) => {

                    updatedEVSEStatus.Add(new EVSEStatusUpdate(evse.Id, oldEVSEStatus, newEVSEStatus));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnEVSEStatusChanged += (timestamp,
                                                        eventTrackingId,
                                                        evse,
                                                        oldEVSEStatus,
                                                        newEVSEStatus) => {

                    updatedEVSEStatus.Add(new EVSEStatusUpdate(evse.Id, oldEVSEStatus, newEVSEStatus));
                    return Task.CompletedTask;

                };

                csoRoamingNetwork.OnEVSEStatusChanged += (timestamp,
                                                       eventTrackingId,
                                                       evse,
                                                       oldEVSEStatus,
                                                       newEVSEStatus) => {

                    updatedEVSEStatus.Add(new EVSEStatusUpdate(evse.Id, oldEVSEStatus, newEVSEStatus));
                    return Task.CompletedTask;

                };

                #endregion

                #region Subscribe OCPI EVSE events

                var updatedOCPIEVSEStatus = new List<StatusType>();

                cpoCommonAPI.OnEVSEChanged += (evse) => {

                    updatedOCPIEVSEStatus.Add(evse.Status);
                    return Task.CompletedTask;

                };

                cpoCommonAPI.OnEVSEStatusChanged += (timestamp,
                                                             evse,
                                                             oldEVSEStatus,
                                                             newEVSEStatus) => {

                    updatedOCPIEVSEStatus.Add(newEVSEStatus);
                    return Task.CompletedTask;

                };

                #endregion

                #region Update

                {
                    if (evse1_UId.HasValue &&
                        cpoCommonAPI.TryGetLocation(CountryCode.Parse(chargingPool1.Operator?.Id.CountryCode.Alpha2Code ?? ""),
                                                            Party_Id.   Parse(chargingPool1.Operator?.Id.Suffix                 ?? ""),
                                                            Location_Id.Parse(chargingPool1!.Id.Suffix),
                                                            out var location) &&
                        location is not null &&
                        location.TryGetEVSE(evse1_UId.Value, out var ocpiEVSE) && ocpiEVSE is not null)
                    {
                        Assert.AreEqual(StatusType.AVAILABLE, ocpiEVSE.Status);
                    }
                }



                evse1.SetStatus(EVSEStatusTypes.Charging);



                Assert.AreEqual(3, updatedEVSEStatus.    Count);
                Assert.AreEqual(2, updatedOCPIEVSEStatus.Count);

                Assert.AreEqual(EVSEStatusTypes.Charging,  graphDefinedCSO.GetEVSEById(evse1!.Id)?.Status.Value);

                {
                    if (evse1_UId.HasValue &&
                        cpoCommonAPI.TryGetLocation(CountryCode.Parse(chargingPool1.Operator?.Id.CountryCode.Alpha2Code ?? ""),
                                                            Party_Id.   Parse(chargingPool1.Operator?.Id.Suffix                 ?? ""),
                                                            Location_Id.Parse(chargingPool1!.Id.Suffix),
                                                            out var location) &&
                        location is not null &&
                        location.TryGetEVSE(evse1_UId.Value, out var ocpiEVSE) && ocpiEVSE is not null)
                    {
                        Assert.AreEqual(StatusType.CHARGING, ocpiEVSE.Status);
                    }
                }

                #endregion


            }

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
                emsp1EMSPAPI       is not null &&
                emsp2EMSPAPI       is not null &&

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


                emsp1EMSPAPI.OnRFIDAuthToken += async (From_CountryCode,
                                                       From_PartyId,
                                                       To_CountryCode,
                                                       To_PartyId,
                                                       tokenId,
                                                       locationReference) => {

                    return tokenId.ToString() == "11223344"

                               ? new AuthorizationInfo(
                                     Allowed:                  AllowedType.ALLOWED,
                                     Token:                    new Token(
                                                                   CountryCode:      To_CountryCode,
                                                                   PartyId:          To_PartyId,
                                                                   Id:               tokenId,
                                                                   Type:             TokenType.RFID,
                                                                   ContractId:       Contract_Id.Parse("DE-GDF-C12345678-X"),
                                                                   Issuer:           "GraphDefined CA",
                                                                   IsValid:          true,
                                                                   WhitelistType:    WhitelistTypes.NEVER,
                                                                   VisualNumber:     null,
                                                                   GroupId:          null,
                                                                   UILanguage:       null,
                                                                   DefaultProfile:   null,
                                                                   EnergyContract:   null,
                                                                   LastUpdated:      null
                                                               ),
                                     Location:                 locationReference,
                                     AuthorizationReference:   null,
                                     Info:                     DisplayText.Create(Languages.en, "Hello world!"),
                                     RemoteParty:              null,
                                     Runtime:                  TimeSpan.FromMilliseconds(23.5)
                                 )

                               : new AuthorizationInfo(
                                     Allowed:                  AllowedType.NOT_ALLOWED,
                                     Token:                    new Token(
                                                                   CountryCode:      To_CountryCode,
                                                                   PartyId:          To_PartyId,
                                                                   Id:               tokenId,
                                                                   Type:             TokenType.RFID,
                                                                   ContractId:       Contract_Id.Parse("DE-GDF-C12345678-X"),
                                                                   Issuer:           "GraphDefined CA",
                                                                   IsValid:          true,
                                                                   WhitelistType:    WhitelistTypes.NEVER,
                                                                   VisualNumber:     null,
                                                                   GroupId:          null,
                                                                   UILanguage:       null,
                                                                   DefaultProfile:   null,
                                                                   EnergyContract:   null,
                                                                   LastUpdated:      null
                                                               ),
                                     Location:                 locationReference,
                                     AuthorizationReference:   null,
                                     Info:                     DisplayText.Create(Languages.en, "Go away!"),
                                     RemoteParty:              null,
                                     Runtime:                  TimeSpan.FromMilliseconds(42)
                                 );

                };

                emsp2EMSPAPI.OnRFIDAuthToken += async (From_CountryCode,
                                                       From_PartyId,
                                                       To_CountryCode,
                                                       To_PartyId,
                                                       tokenId,
                                                       locationReference) => {

                    return tokenId.ToString() == "55667788"

                               ? new AuthorizationInfo(
                                     Allowed:                  AllowedType.ALLOWED,
                                     Token:                    new Token(
                                                                   CountryCode:      To_CountryCode,
                                                                   PartyId:          To_PartyId,
                                                                   Id:               tokenId,
                                                                   Type:             TokenType.RFID,
                                                                   ContractId:       Contract_Id.Parse("DE-GDF-C56781234-X"),
                                                                   Issuer:           "GraphDefined CA",
                                                                   IsValid:          true,
                                                                   WhitelistType:    WhitelistTypes.NEVER,
                                                                   VisualNumber:     null,
                                                                   GroupId:          null,
                                                                   UILanguage:       null,
                                                                   DefaultProfile:   null,
                                                                   EnergyContract:   null,
                                                                   LastUpdated:      null
                                                               ),
                                     Location:                 locationReference,
                                     AuthorizationReference:   null,
                                     Info:                     DisplayText.Create(Languages.en, "Hello world!"),
                                     RemoteParty:              null,
                                     Runtime:                  TimeSpan.FromMilliseconds(23.5)
                                 )

                               : new AuthorizationInfo(
                                     Allowed:                  AllowedType.NOT_ALLOWED,
                                     Token:                    new Token(
                                                                   CountryCode:      To_CountryCode,
                                                                   PartyId:          To_PartyId,
                                                                   Id:               tokenId,
                                                                   Type:             TokenType.RFID,
                                                                   ContractId:       Contract_Id.Parse("DE-GDF-C56781234-X"),
                                                                   Issuer:           "GraphDefined CA",
                                                                   IsValid:          true,
                                                                   WhitelistType:    WhitelistTypes.NEVER,
                                                                   VisualNumber:     null,
                                                                   GroupId:          null,
                                                                   UILanguage:       null,
                                                                   DefaultProfile:   null,
                                                                   EnergyContract:   null,
                                                                   LastUpdated:      null
                                                               ),
                                     Location:                 locationReference,
                                     AuthorizationReference:   null,
                                     Info:                     DisplayText.Create(Languages.en, "Go away!"),
                                     RemoteParty:              null,
                                     Runtime:                  TimeSpan.FromMilliseconds(42)
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

        }

        #endregion


    }

}