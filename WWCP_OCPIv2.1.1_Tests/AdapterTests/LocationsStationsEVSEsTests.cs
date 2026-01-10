/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.AdapterTests
{

    #region (class) DefaultData

    public class DefaultData(IChargingPool     ChargingPool1,
                             IChargingPool     ChargingPool2,
                             IChargingStation  ChargingStation1,
                             IChargingStation  ChargingStation2,
                             IChargingStation  ChargingStation3,
                             IEVSE             EVSE1,
                             IEVSE             EVSE2,
                             IEVSE             EVSE3,
                             IEVSE             EVSE4)
    {

        public IChargingPool     ChargingPool1       { get; set; } = ChargingPool1;
        public IChargingPool     ChargingPool2       { get; set; } = ChargingPool2;

        public IChargingStation  ChargingStation1    { get; set; } = ChargingStation1;
        public IChargingStation  ChargingStation2    { get; set; } = ChargingStation2;
        public IChargingStation  ChargingStation3    { get; set; } = ChargingStation3;

        public IEVSE             EVSE1               { get; set; } = EVSE1;
        public IEVSE             EVSE2               { get; set; } = EVSE2;
        public IEVSE             EVSE3               { get; set; } = EVSE3;
        public IEVSE             EVSE4               { get; set; } = EVSE4;

    }

    #endregion


    [TestFixture]
    public class LocationsStationsEVSEsTests : ACSOAdapterTests
    {

        #region (private) AddDefaultDevices()

        /// <summary>
        /// Add default devices.
        /// </summary>
        private async Task<DefaultData> AddDefaultDevices()
        {

            if (roamingNetwork  is not null &&
                graphDefinedCSO is not null &&
                csoAdapter      is not null &&
                commonAPI       is not null)
            {

                csoAdapter.CommonAPI.OnLocationAdded += async (location) => { };
                csoAdapter.CommonAPI.OnEVSEAdded     += async (evse)     => { };

                #region Add DE*GEF*POOL1

                var addChargingPoolResult1 = await graphDefinedCSO.AddChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL1"),
                                                 Name:                 I18NString.Create("Test pool #1"),
                                                 Description:          I18NString.Create("GraphDefined charging pool for tests #1"),

                                                 Address:              new Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.de, "Jena"),
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

                                                 InitialAdminStatus:   ChargingPoolAdminStatusType.Operational,
                                                 InitialStatus:        ChargingPoolStatusType.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                Assert.That(addChargingPoolResult1,               Is.Not.Null);
                Assert.That(addChargingPoolResult1.ChargingPool,  Is.Not.Null);

                var chargingPool1 = addChargingPoolResult1.ChargingPool!;

                #endregion

                #region Add DE*GEF*POOL2

                var addChargingPoolResult2 = await graphDefinedCSO.AddChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL2"),
                                                 Name:                 I18NString.Create("Test pool #2"),
                                                 Description:          I18NString.Create("GraphDefined charging pool for tests #2"),

                                                 Address:              new Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.de, "Jena"),
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

                                                 InitialAdminStatus:   ChargingPoolAdminStatusType.Operational,
                                                 InitialStatus:        ChargingPoolStatusType.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                Assert.That(addChargingPoolResult2,               Is.Not.Null);
                Assert.That(addChargingPoolResult2.ChargingPool,  Is.Not.Null);

                var chargingPool2 = addChargingPoolResult2.ChargingPool!;

                #endregion

                var allLocations1 = csoAdapter.CommonAPI.GetLocations().OrderBy(location => location.Id).ToArray();
                var time1_loc1_created = allLocations1.ElementAt(0).Created;
                var time1_loc2_created = allLocations1.ElementAt(1).Created;
                var time1_loc1_updated = allLocations1.ElementAt(0).LastUpdated;
                var time1_loc2_updated = allLocations1.ElementAt(1).LastUpdated;

                await Task.Delay(50);


                // OCPI v2.1.1 does not have stations!

                #region Add DE*GEF*STATION*1*A

                var addChargingStationResult1 = await chargingPool1.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*1*A"),
                                                    Name:                 I18NString.Create("Test station #1A"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #1A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusType.Operational,
                                                    InitialStatus:        ChargingStationStatusType.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                Assert.That(addChargingStationResult1,                  Is.Not.Null);
                Assert.That(addChargingStationResult1.ChargingStation,  Is.Not.Null);

                var chargingStation1 = addChargingStationResult1.ChargingStation!;

                #endregion

                #region Add DE*GEF*STATION*1*B

                var addChargingStationResult2 = await chargingPool1.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*1*B"),
                                                    Name:                 I18NString.Create("Test station #1B"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #1B"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusType.Operational,
                                                    InitialStatus:        ChargingStationStatusType.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                Assert.That(addChargingStationResult2,                  Is.Not.Null);
                Assert.That(addChargingStationResult2.ChargingStation,  Is.Not.Null);

                var chargingStation2 = addChargingStationResult2.ChargingStation!;

                #endregion

                #region Add DE*GEF*STATION*2*A

                var addChargingStationResult3 = await chargingPool2.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*2*A"),
                                                    Name:                 I18NString.Create("Test station #2A"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #2A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusType.Operational,
                                                    InitialStatus:        ChargingStationStatusType.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                Assert.That(addChargingStationResult3,                  Is.Not.Null);
                Assert.That(addChargingStationResult3.ChargingStation,  Is.Not.Null);

                var chargingStation3 = addChargingStationResult3.ChargingStation!;

                #endregion

                var allLocations2 = csoAdapter.CommonAPI.GetLocations().OrderBy(location => location.Id).ToArray();
                var time2_loc1_created = allLocations2.ElementAt(0).Created;
                var time2_loc2_created = allLocations2.ElementAt(1).Created;
                var time2_loc1_updated = allLocations2.ElementAt(0).LastUpdated;
                var time2_loc2_updated = allLocations2.ElementAt(1).LastUpdated;

                await Task.Delay(50);


                #region Add EVSE DE*GEF*EVSE*1*A*1

                var addEVSE1Result1 = await chargingStation1.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*1"),
                                          Name:                 I18NString.Create("Test EVSE #1A1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1A1"),

                                          InitialAdminStatus:   EVSEAdminStatusType.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.CHAdeMO
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.That(addEVSE1Result1,       Is.Not.Null);
                Assert.That(addEVSE1Result1.EVSE,  Is.Not.Null);

                var evse1 = addEVSE1Result1.EVSE!;

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*A*2

                var addEVSE1Result2 = await chargingStation1.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*2"),
                                          Name:                 I18NString.Create("Test EVSE #1A2"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1A2"),

                                          InitialAdminStatus:   EVSEAdminStatusType.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.Type2Outlet
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.That(addEVSE1Result2,       Is.Not.Null);
                Assert.That(addEVSE1Result2.EVSE,  Is.Not.Null);

                var evse2 = addEVSE1Result2.EVSE!;

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*B*1

                var addEVSE1Result3 = await chargingStation2.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*B*1"),
                                          Name:                 I18NString.Create("Test EVSE #1B1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1B1"),

                                          InitialAdminStatus:   EVSEAdminStatusType.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.TypeFSchuko
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.That(addEVSE1Result3,       Is.Not.Null);
                Assert.That(addEVSE1Result3.EVSE,  Is.Not.Null);

                var evse3 = addEVSE1Result3.EVSE!;

                #endregion

                #region Add EVSE DE*GEF*EVSE*2*A*1

                var addEVSE1Result4 = await chargingStation3.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*2*A*1"),
                                          Name:                 I18NString.Create("Test EVSE #2A1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #2A1"),

                                          InitialAdminStatus:   EVSEAdminStatusType.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.Type2Connector_CableAttached,
                                                                        CableAttached: true
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.That(addEVSE1Result4,       Is.Not.Null);
                Assert.That(addEVSE1Result4.EVSE,  Is.Not.Null);

                var evse4 = addEVSE1Result4.EVSE!;

                #endregion

                var allLocations3 = csoAdapter.CommonAPI.GetLocations().OrderBy(location => location.Id).ToArray();
                var time3_loc1_created = allLocations3.ElementAt(0).Created;
                var time3_loc2_created = allLocations3.ElementAt(1).Created;
                var time3_loc1_updated = allLocations3.ElementAt(0).LastUpdated;
                var time3_loc2_updated = allLocations3.ElementAt(1).LastUpdated;

                await Task.Delay(50);


                #region Validate, that locations had been sent to the OCPI module

                var allLocations  = csoAdapter.CommonAPI.GetLocations().ToArray();
                Assert.That(allLocations,         Is.Not.Null);
                Assert.That(allLocations.Length,  Is.EqualTo(2));

                #endregion

                #region Validate, that EVSEs had been sent to the OCPI module

                var allEVSEs      = csoAdapter.CommonAPI.GetLocations().SelectMany(location => location.EVSEs).ToArray();
                Assert.That(allEVSEs,         Is.Not.Null);
                Assert.That(allEVSEs.Length,  Is.EqualTo(4));

                #endregion

                #region Validate, that both locations have EVSEs

                if (csoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1!.Id.ToString()), out var location1) && location1 is not null)
                {
                    Assert.That(location1.EVSEs.Count(),  Is.EqualTo(3));
                }
                else
                    Assert.Fail("location1 was not found!");


                if (csoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool2!.Id.ToString()), out var location2) && location2 is not null)
                {
                    Assert.That(location2.EVSEs.Count(),  Is.EqualTo(1));
                }
                else
                    Assert.Fail("location2 was not found!");

                #endregion


                var x1_1c = time1_loc1_created.ToISO8601();
                var x2_1c = time2_loc1_created.ToISO8601();
                var x3_1c = time3_loc1_created.ToISO8601();

                var x1_2c = time1_loc2_created.ToISO8601();
                var x2_2c = time2_loc2_created.ToISO8601();
                var x3_2c = time3_loc2_created.ToISO8601();

                var x1_1u = time1_loc1_updated.ToISO8601();
                var x2_1u = time2_loc1_updated.ToISO8601();
                var x3_1u = time3_loc1_updated.ToISO8601();

                var x1_2u = time1_loc2_updated.ToISO8601();
                var x2_2u = time2_loc2_updated.ToISO8601();
                var x3_2u = time3_loc2_updated.ToISO8601();

                // Location1 Created time stamps do not change!
                Assert.That(time1_loc1_created, Is.EqualTo    (time2_loc1_created));
                Assert.That(time1_loc1_created, Is.EqualTo    (time3_loc1_created));

                // Location1 LastUpdate time stamps are in correct order!
                Assert.That(time1_loc1_updated, Is.EqualTo    (time2_loc1_updated));
                Assert.That(time3_loc1_updated, Is.GreaterThan(time1_loc1_updated));

                // Location2 Created time stamps do not change!
                Assert.That(time1_loc2_created, Is.EqualTo    (time2_loc2_created));
                Assert.That(time1_loc2_created, Is.EqualTo    (time3_loc2_created));

                // Location2 LastUpdate time stamps are in correct order!
                Assert.That(time1_loc2_updated, Is.EqualTo    (time2_loc2_updated));
                Assert.That(time3_loc2_updated, Is.GreaterThanOrEqualTo(time1_loc2_updated)); //ToDo: why "...OrEqualTo"?

                // Location1 is always older than Location2
                Assert.That(time1_loc2_created, Is.GreaterThan(time1_loc1_created));
                Assert.That(time1_loc2_updated, Is.GreaterThan(time1_loc1_updated));
                Assert.That(time2_loc2_updated, Is.GreaterThan(time2_loc1_updated));
               //Assert.That(time3_loc2_updated, Is.GreaterThanOrEqualTo(time3_loc1_updated)); //ToDo: why "...OrEqualTo"?


                return new DefaultData(
                           chargingPool1,
                           chargingPool2,
                           chargingStation1,
                           chargingStation2,
                           chargingStation3,
                           evse1,
                           evse2,
                           evse3,
                           evse4
                       );

            }

            throw new Exception("Missing roaming network, CSO, CSO adapter or common API!");

        }

        #endregion


        #region Add_ChargingLocationsAndEVSEs_Test1()

        /// <summary>
        /// Add WWCP charging locations, stations and EVSEs.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the OCPI module.
        /// </summary>
        [Test]
        public async Task Add_ChargingLocationsAndEVSEs_Test1()
        {

            if (roamingNetwork  is not null &&
                graphDefinedCSO is not null &&
                csoAdapter      is not null &&
                commonAPI       is not null)
            {

                csoAdapter.CommonAPI.OnLocationAdded += async (location) => { };
                csoAdapter.CommonAPI.OnEVSEAdded     += async (evse)     => { };

                await AddDefaultDevices();


                #region Validate, that locations had been sent to the OCPI module

                var allLocations  = csoAdapter.CommonAPI.GetLocations().ToArray();
                Assert.That(allLocations,         Is.Not.Null);
                Assert.That(allLocations.Length,  Is.EqualTo(2));

                #endregion

                #region Validate, that EVSEs had been sent to the OCPI module

                var allEVSEs      = csoAdapter.CommonAPI.GetLocations().SelectMany(location => location.EVSEs).ToArray();
                Assert.That(allEVSEs,         Is.Not.Null);
                Assert.That(allEVSEs.Length,  Is.EqualTo(4));

                #endregion


                #region Validate via HTTP (OpenData, no authorization)

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Connection = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {
                                Assert.That(jsonEVSEs1?.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2?.Count,  Is.EqualTo(1));
                            }

                        }

                    }

                }

                #endregion

                #region Validate via HTTP (with authorization)

                await commonAPI.AddRemoteParty(

                          CountryCode:         CountryCode.Parse("DE"),
                          PartyId:             Party_Id.   Parse("GDF"),
                          Role:                Role.EMSP,
                          BusinessDetails:     new BusinessDetails(
                                                   "GraphDefined EMSP"
                                               ),

                          Status:              PartyStatus.ENABLED,

                          LocalAccessToken:    AccessToken.Parse("1234xyz"),
                          LocalAccessStatus:   AccessStatus.ALLOWED

                      );

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.Parse("1234xyz");
                                                         requestBuilder.Connection     = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,                                                   Is.Not.Null);
                        Assert.That(jsonLocation2,                                                   Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,                                                  Is.Not.Null);
                            Assert.That(jsonEVSEs2,                                                  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {
                                Assert.That(jsonEVSEs1?.Count,                                       Is.EqualTo(3));
                                Assert.That(jsonEVSEs2?.Count,                                       Is.EqualTo(1));
                            }

                        }

                    }

                }

                #endregion


            }

        }

        #endregion


        #region Update_ChargingLocationData_Test1()

        /// <summary>
        /// Add WWCP charging pools, charging stations and EVSEs and update charging pool (static) data.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the OCPI module.
        /// </summary>
        [Test]
        public async Task Update_ChargingLocationData_Test1()
        {

            if (roamingNetwork  is not null &&
                graphDefinedCSO is not null &&
                csoAdapter      is not null)
            {

                var defaultDevices = await AddDefaultDevices();
                var chargingPool1     = defaultDevices.ChargingPool1;
                var chargingPool2     = defaultDevices.ChargingPool2;
                var chargingStation1  = defaultDevices.ChargingStation1;
                var chargingStation2  = defaultDevices.ChargingStation2;
                var chargingStation3  = defaultDevices.ChargingStation3;
                var evse1             = defaultDevices.EVSE1;
                var evse2             = defaultDevices.EVSE2;
                var evse3             = defaultDevices.EVSE3;
                var evse4             = defaultDevices.EVSE4;


                #region Validate via HTTP (OpenData, no authorization)

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Connection = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {
                                Assert.That(jsonEVSEs1?.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2?.Count,  Is.EqualTo(1));
                            }

                        }

                    }

                }

                #endregion

                #region Validate via HTTP (with authorization)

                await commonAPI!.AddRemoteParty(

                          CountryCode:         CountryCode.Parse("DE"),
                          PartyId:             Party_Id.Parse("GDF"),
                          Role:                Role.EMSP,
                          BusinessDetails:     new BusinessDetails(
                                                   "GraphDefined EMSP"
                                               ),

                          Status:              PartyStatus.ENABLED,

                          LocalAccessToken:    AccessToken.Parse("1234xyz"),
                          LocalAccessStatus:   AccessStatus.ALLOWED

                      );

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.Parse("1234xyz");
                                                         requestBuilder.Connection     = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {
                                Assert.That(jsonEVSEs1?.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2?.Count,  Is.EqualTo(1));
                            }

                        }

                    }

                }

                #endregion


                var a_p1ct1 = chargingPool1.Created;
                var a_p1lt1 = chargingPool1.LastChangeDate;

                var a_p2ct1 = chargingPool2.Created;
                var a_p2lt1 = chargingPool2.LastChangeDate;


                #region Update static charging pool data

                var updatedPoolProperties1     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedPoolProperties2     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedPoolProperties3     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedPoolProperties4     = new List<PropertyUpdateInfo<ChargingPool_Id>>();

                var updatedStationProperties1  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();
                var updatedStationProperties2  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();
                var updatedStationProperties3  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();
                var updatedStationProperties4  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();

                var updatedEVSEProperties1     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();
                var updatedEVSEProperties2     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();
                var updatedEVSEProperties3     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();
                var updatedEVSEProperties4     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();

                #region Subscribe charging pool events

                chargingPool1.OnPropertyChanged += (timestamp,
                                                     eventTrackingId,
                                                     chargingPool,
                                                     propertyName,
                                                     newValue,
                                                     oldValue,
                                                     dataSource) => {

                    updatedPoolProperties1.Add(new PropertyUpdateInfo<ChargingPool_Id>((chargingPool as IChargingPool)!.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                chargingPool1.OnDataChanged += (timestamp,
                                                 eventTrackingId,
                                                 chargingPool,
                                                 propertyName,
                                                 newValue,
                                                 oldValue,
                                                 dataSource) => {

                    updatedPoolProperties2.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingPoolDataChanged += (timestamp,
                                                              eventTrackingId,
                                                              chargingPool,
                                                              propertyName,
                                                              newValue,
                                                              oldValue,
                                                              dataSource) => {

                    updatedPoolProperties3.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                roamingNetwork.OnChargingPoolDataChanged  += (timestamp,
                                                              eventTrackingId,
                                                              chargingPool,
                                                              propertyName,
                                                              newValue,
                                                              oldValue,
                                                              dataSource) => {

                    updatedPoolProperties4.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue));
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

                    updatedStationProperties1.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>((chargingStation as IChargingStation)!.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                chargingStation1!.OnDataChanged += (timestamp,
                                                    eventTrackingId,
                                                    chargingStation,
                                                    propertyName,
                                                    newValue,
                                                    oldValue,
                                                    dataSource) => {

                    updatedStationProperties2.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingStationDataChanged += (timestamp,
                                                                 eventTrackingId,
                                                                 chargingStation,
                                                                 propertyName,
                                                                 newValue,
                                                                 oldValue,
                                                                 dataSource) => {

                    updatedStationProperties3.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                roamingNetwork.OnChargingStationDataChanged += (timestamp,
                                                                eventTrackingId,
                                                                chargingStation,
                                                                propertyName,
                                                                newValue,
                                                                oldValue,
                                                                dataSource) => {

                    updatedStationProperties4.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue));
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

                    updatedEVSEProperties1.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>((evse as IEVSE)!.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                evse1!.OnDataChanged += (timestamp,
                                         eventTrackingId,
                                         evse,
                                         propertyName,
                                         newValue,
                                         oldValue,
                                         dataSource) => {

                    updatedEVSEProperties2.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnEVSEDataChanged += (timestamp,
                                                      eventTrackingId,
                                                      evse,
                                                      propertyName,
                                                      newValue,
                                                      oldValue,
                                                      dataSource) => {

                    updatedEVSEProperties3.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                roamingNetwork.OnEVSEDataChanged += (timestamp,
                                                     eventTrackingId,
                                                     evse,
                                                     propertyName,
                                                     newValue,
                                                     oldValue,
                                                     dataSource) => {

                    updatedEVSEProperties4.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                #endregion

                var locationChanges           = new List<String>();
                var evseChanges               = new List<String>();

                csoAdapter.CommonAPI.OnLocationChanged += (location) => {
                    locationChanges.Add(location.ToJSON().ToString());
                    return Task.CompletedTask;
                };

                csoAdapter.CommonAPI.OnEVSEChanged     += (evse)     => {
                    evseChanges.    Add(evse.    ToJSON().ToString());
                    return Task.CompletedTask;
                };


                chargingPool1.Name.       Set(Languages.en, "Test pool #1 (updated)");
                //chargingPool1.Description.Set(Languages.en, "GraphDefined charging pool for tests #1 (updated)");

                await Task.Delay(300);

                // WWCP
                Assert.That(updatedPoolProperties1.Count,                                                       Is.EqualTo(1));
                Assert.That(updatedPoolProperties2.Count,                                                       Is.EqualTo(1));
                Assert.That(updatedPoolProperties3.Count,                                                       Is.EqualTo(1));
                Assert.That(updatedPoolProperties4.Count,                                                       Is.EqualTo(1));

                // OCPI
                Assert.That(locationChanges.       Count,                                                       Is.EqualTo(1));
                Assert.That(evseChanges.           Count,                                                       Is.EqualTo(0));

                Assert.That(graphDefinedCSO.GetChargingPoolById(chargingPool1.Id)!.Name       [Languages.en],  Is.EqualTo("Test pool #1 (updated)"                           ));
                //Assert.That(graphDefinedCSO.GetChargingPoolById(chargingPool1.Id)!.Description[Languages.en],  Is.EqualTo("GraphDefined charging pool for tests #1 (updated)"));

                csoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1.Id.ToString()), out var location);
                Assert.That(location!.Name,                                                                     Is.EqualTo("Test pool #1 (updated)"));
                //Assert.That(location!.Name,                                                                     Is.EqualTo("GraphDefined Charging Pool für Tests #1")); // Not mapped to OCPI!

                // .type
                // .coordinates
                // .opening_times
                // .last_updated

                await graphDefinedCSO.UpdateChargingPool(
                          chargingPool1.Id,
                          pool => pool.Address = new Address(

                                                     Street:              "Amselfeld",
                                                     PostalCode:          "07742",
                                                     City:                I18NString.Create(Languages.de, "NeuJena"),
                                                     Country:             Country.Germany,

                                                     HouseNumber:         "42",
                                                     FloorLevel:          null,
                                                     Region:              null,
                                                     PostalCodeSub:       null,
                                                     TimeZone:            null,
                                                     OfficialLanguages:   null,
                                                     Comment:             null,

                                                     CustomData:          null,
                                                     InternalData:        null

                                                 )
                      );

                Assert.That(updatedPoolProperties1.Count,  Is.EqualTo(2));
                Assert.That(updatedPoolProperties2.Count,  Is.EqualTo(2));
                Assert.That(updatedPoolProperties3.Count,  Is.EqualTo(2));
                Assert.That(updatedPoolProperties4.Count,  Is.EqualTo(2));

                Assert.That(graphDefinedCSO.GetChargingPoolById(chargingPool1.Id)!.Address!.Street,  Is.EqualTo("Amselfeld"));

                csoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1.Id.ToString()), out location);
                Assert.That(location!.City,                Is.EqualTo("NeuJena"));
                Assert.That(location!.PostalCode,          Is.EqualTo("07742"));




           //     evse1.Name.Set(Languages.en, "Test EVSE #1A1 (updated)");





                //var updateChargingPoolResult2 = await graphDefinedCSO.UpdateChargingPool()


                //var updateChargingPoolResult2 = await chargingPool2.Address = new Address(

                //                                                                  Street:             "Biberweg",
                //                                                                  PostalCode:         "07749",
                //                                                                  City:               I18NString.Create(Languages.de, "Jena"),
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

                //ClassicAssert.IsNotNull(addChargingPoolResult2);

                //var chargingPool2  = addChargingPoolResult2.ChargingPool;
                //ClassicAssert.IsNotNull(chargingPool2);

                #endregion


                var b_p1ct1 = chargingPool1.Created;
                var b_p1lt1 = chargingPool1.LastChangeDate;

                var b_p2ct1 = chargingPool2.Created;
                var b_p2lt1 = chargingPool2.LastChangeDate;

                #region Validate via HTTP (OpenData, no authorization)

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Connection = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {
                                Assert.That(jsonEVSEs1?.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2?.Count,  Is.EqualTo(1));
                            }

                        }

                    }

                }

                #endregion

                #region Validate via HTTP (with authorization)

                await commonAPI!.AddRemoteParty(

                          CountryCode:         CountryCode.Parse("DE"),
                          PartyId:             Party_Id.Parse("GDF"),
                          Role:                Role.EMSP,
                          BusinessDetails:     new BusinessDetails(
                                                   "GraphDefined EMSP"
                                               ),

                          Status:              PartyStatus.ENABLED,

                          LocalAccessToken:    AccessToken.Parse("1234xyz"),
                          LocalAccessStatus:   AccessStatus.ALLOWED

                      );

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.Parse("1234xyz");
                                                         requestBuilder.Connection     = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {
                                Assert.That(jsonEVSEs1?.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2?.Count,  Is.EqualTo(1));
                            }

                        }

                    }

                }

                #endregion

            }

        }

        #endregion

        #region Update_ChargingStation_GeoLocation_Test()

        /// <summary>
        /// Update the geo location of a WWCP charging station, which will update one or multiple OCPI EVSEs.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the OCPI module.
        /// </summary>
        [Test]
        public async Task Update_ChargingStation_GeoLocation_Test()
        {

            if (roamingNetwork  is not null &&
                graphDefinedCSO is not null &&
                csoAdapter      is not null)
            {

                var defaultDevices = await AddDefaultDevices();
                var chargingPool1     = defaultDevices.ChargingPool1;
                var chargingPool2     = defaultDevices.ChargingPool2;
                var chargingStation1  = defaultDevices.ChargingStation1;
                var chargingStation2  = defaultDevices.ChargingStation2;
                var chargingStation3  = defaultDevices.ChargingStation3;
                var evse1             = defaultDevices.EVSE1;
                var evse2             = defaultDevices.EVSE2;
                var evse3             = defaultDevices.EVSE3;
                var evse4             = defaultDevices.EVSE4;


                #region Validate via HTTP (OpenData, no authorization)

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Connection = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2.Count,  Is.EqualTo(1));

                                var evse_A1 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*1").First();
                                var evse_A2 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*2").First();

                                Assert.That(evse_A1["coordinates"]?["latitude"]?. Value<String>(),  Is.EqualTo("50.82000"));
                                Assert.That(evse_A1["coordinates"]?["longitude"]?.Value<String>(),  Is.EqualTo("11.52000"));

                                Assert.That(evse_A2["coordinates"]?["latitude"]?. Value<String>(),  Is.EqualTo("50.82000"));
                                Assert.That(evse_A2["coordinates"]?["longitude"]?.Value<String>(),  Is.EqualTo("11.52000"));

                            }

                        }

                    }

                }

                #endregion

                #region Validate via HTTP (with authorization)

                await commonAPI!.AddRemoteParty(

                          CountryCode:         CountryCode.Parse("DE"),
                          PartyId:             Party_Id.Parse("GDF"),
                          Role:                Role.EMSP,
                          BusinessDetails:     new BusinessDetails(
                                                   "GraphDefined EMSP"
                                               ),

                          Status:              PartyStatus.ENABLED,

                          LocalAccessToken:    AccessToken.Parse("1234xyz"),
                          LocalAccessStatus:   AccessStatus.ALLOWED

                      );

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.Parse("1234xyz");
                                                         requestBuilder.Connection     = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2.Count,  Is.EqualTo(1));

                                var evse_A1 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*1").First();
                                var evse_A2 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*2").First();

                                Assert.That(evse_A1["coordinates"]?["latitude"]?. Value<String>(),  Is.EqualTo("50.82000"));
                                Assert.That(evse_A1["coordinates"]?["longitude"]?.Value<String>(),  Is.EqualTo("11.52000"));

                                Assert.That(evse_A2["coordinates"]?["latitude"]?. Value<String>(),  Is.EqualTo("50.82000"));
                                Assert.That(evse_A2["coordinates"]?["longitude"]?.Value<String>(),  Is.EqualTo("11.52000"));

                            }

                        }

                    }

                }

                #endregion


                var a_p1ct1 = chargingPool1.Created;
                var a_p1lt1 = chargingPool1.LastChangeDate;

                var a_p2ct1 = chargingPool2.Created;
                var a_p2lt1 = chargingPool2.LastChangeDate;


                #region Update static EVSE data

                var updatedPoolProperties1     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedPoolProperties2     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedPoolProperties3     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedPoolProperties4     = new List<PropertyUpdateInfo<ChargingPool_Id>>();

                var updatedStationProperties1  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();
                var updatedStationProperties2  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();
                var updatedStationProperties3  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();
                var updatedStationProperties4  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();

                var updatedEVSEProperties1     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();
                var updatedEVSEProperties2     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();
                var updatedEVSEProperties3     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();
                var updatedEVSEProperties4     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();

                #region Subscribe charging pool events

                chargingPool1.OnPropertyChanged += (timestamp,
                                                     eventTrackingId,
                                                     chargingPool,
                                                     propertyName,
                                                     newValue,
                                                     oldValue,
                                                     dataSource) => {

                    updatedPoolProperties1.Add(new PropertyUpdateInfo<ChargingPool_Id>((chargingPool as IChargingPool)!.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                chargingPool1.OnDataChanged += (timestamp,
                                                 eventTrackingId,
                                                 chargingPool,
                                                 propertyName,
                                                 newValue,
                                                 oldValue,
                                                 dataSource) => {

                    updatedPoolProperties2.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingPoolDataChanged += (timestamp,
                                                              eventTrackingId,
                                                              chargingPool,
                                                              propertyName,
                                                              newValue,
                                                              oldValue,
                                                              dataSource) => {

                    updatedPoolProperties3.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                roamingNetwork.OnChargingPoolDataChanged  += (timestamp,
                                                              eventTrackingId,
                                                              chargingPool,
                                                              propertyName,
                                                              newValue,
                                                              oldValue,
                                                              dataSource) => {

                    updatedPoolProperties4.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue));
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

                    updatedStationProperties1.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>((chargingStation as IChargingStation)!.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                chargingStation1!.OnDataChanged += (timestamp,
                                                    eventTrackingId,
                                                    chargingStation,
                                                    propertyName,
                                                    newValue,
                                                    oldValue,
                                                    dataSource) => {

                    updatedStationProperties2.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingStationDataChanged += (timestamp,
                                                                 eventTrackingId,
                                                                 chargingStation,
                                                                 propertyName,
                                                                 newValue,
                                                                 oldValue,
                                                                 dataSource) => {

                    updatedStationProperties3.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                roamingNetwork.OnChargingStationDataChanged += (timestamp,
                                                                eventTrackingId,
                                                                chargingStation,
                                                                propertyName,
                                                                newValue,
                                                                oldValue,
                                                                dataSource) => {

                    updatedStationProperties4.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                #endregion

                var locationChanges           = new List<String>();
                var evseChanges               = new List<String>();

                csoAdapter.CommonAPI.OnLocationChanged += (location) => {
                    locationChanges.Add(location.ToJSON().ToString());
                    return Task.CompletedTask;
                };

                csoAdapter.CommonAPI.OnEVSEChanged     += (evse)     => {
                    evseChanges.    Add(evse.    ToJSON().ToString());
                    return Task.CompletedTask;
                };


                chargingStation1.GeoLocation = GeoCoordinate.Parse(40.257, 16.381);

                // WWCP
                Assert.That(updatedStationProperties1.Count,                                                             Is.EqualTo(1));
                Assert.That(updatedStationProperties2.Count,                                                             Is.EqualTo(1));
                Assert.That(updatedStationProperties3.Count,                                                             Is.EqualTo(1));
                Assert.That(updatedStationProperties4.Count,                                                             Is.EqualTo(1));

                Assert.That(updatedStationProperties1.First().ToString(),                                                Is.EqualTo("Update of 'DE*GEF*STATION*1*A'.'GeoLocation' 'Latitude = 50.82, Longitude = 11.52' -> 'Latitude = 40.257, Longitude = 16.381' !"));
                Assert.That(updatedStationProperties2.First().ToString(),                                                Is.EqualTo("Update of 'DE*GEF*STATION*1*A'.'GeoLocation' 'Latitude = 50.82, Longitude = 11.52' -> 'Latitude = 40.257, Longitude = 16.381' !"));
                Assert.That(updatedStationProperties3.First().ToString(),                                                Is.EqualTo("Update of 'DE*GEF*STATION*1*A'.'GeoLocation' 'Latitude = 50.82, Longitude = 11.52' -> 'Latitude = 40.257, Longitude = 16.381' !"));
                Assert.That(updatedStationProperties4.First().ToString(),                                                Is.EqualTo("Update of 'DE*GEF*STATION*1*A'.'GeoLocation' 'Latitude = 50.82, Longitude = 11.52' -> 'Latitude = 40.257, Longitude = 16.381' !"));

                Assert.That(roamingNetwork. GetChargingStationById(chargingStation1.Id)!.GeoLocation?.Latitude. Value,   Is.EqualTo(40.257));
                Assert.That(roamingNetwork. GetChargingStationById(chargingStation1.Id)!.GeoLocation?.Longitude.Value,   Is.EqualTo(16.381));

                Assert.That(graphDefinedCSO.GetChargingStationById(chargingStation1.Id)!.GeoLocation?.Latitude. Value,   Is.EqualTo(40.257));
                Assert.That(graphDefinedCSO.GetChargingStationById(chargingStation1.Id)!.GeoLocation?.Longitude.Value,   Is.EqualTo(16.381));

                Assert.That(chargingPool1.  GetChargingStationById(chargingStation1.Id)!.GeoLocation?.Latitude. Value,   Is.EqualTo(40.257));
                Assert.That(chargingPool1.  GetChargingStationById(chargingStation1.Id)!.GeoLocation?.Longitude.Value,   Is.EqualTo(16.381));

                Assert.That(evse1.ChargingStation!.                                      GeoLocation?.Latitude. Value,   Is.EqualTo(40.257));
                Assert.That(evse1.ChargingStation!.                                      GeoLocation?.Longitude.Value,   Is.EqualTo(16.381));


                // OCPI
                Assert.That(locationChanges.Count,                                                                       Is.EqualTo(1));
                Assert.That(evseChanges.    Count,                                                                       Is.EqualTo(2));

                if (csoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1.Id.ToString()), out var location) &&
                    location.TryGetEVSE(evse1.Id.ToOCPI_EVSEUId()!.Value, out var evse))
                {
                    Assert.That(evse.Coordinates?.Latitude. Value,                                                       Is.EqualTo(40.257));
                    Assert.That(evse.Coordinates?.Longitude.Value,                                                       Is.EqualTo(16.381));
                }

                #endregion


                var b_p1ct1 = chargingPool1.Created;
                var b_p1lt1 = chargingPool1.LastChangeDate;

                var b_p2ct1 = chargingPool2.Created;
                var b_p2lt1 = chargingPool2.LastChangeDate;

                #region Validate via HTTP (OpenData, no authorization)

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Connection = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2.Count,  Is.EqualTo(1));

                                var evse_A1 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*1").First();
                                var evse_A2 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*2").First();

                                Assert.That(evse_A1["coordinates"]?["latitude"]?. Value<String>(),  Is.EqualTo("40.25700"));
                                Assert.That(evse_A1["coordinates"]?["longitude"]?.Value<String>(),  Is.EqualTo("16.38100"));

                                Assert.That(evse_A2["coordinates"]?["latitude"]?. Value<String>(),  Is.EqualTo("40.25700"));
                                Assert.That(evse_A2["coordinates"]?["longitude"]?.Value<String>(),  Is.EqualTo("16.38100"));

                            }

                        }

                    }

                }

                #endregion

                #region Validate via HTTP (with authorization)

                await commonAPI!.AddRemoteParty(

                          CountryCode:         CountryCode.Parse("DE"),
                          PartyId:             Party_Id.Parse("GDF"),
                          Role:                Role.EMSP,
                          BusinessDetails:     new BusinessDetails(
                                                   "GraphDefined EMSP"
                                               ),

                          Status:              PartyStatus.ENABLED
                          ,
                          LocalAccessToken:    AccessToken.Parse("1234xyz"),
                          LocalAccessStatus:   AccessStatus.ALLOWED

                      );

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.Parse("1234xyz");
                                                         requestBuilder.Connection     = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2.Count,  Is.EqualTo(1));

                                var evse_A1 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*1").First();
                                var evse_A2 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*2").First();

                                Assert.That(evse_A1["coordinates"]?["latitude"]?. Value<String>(),  Is.EqualTo("40.25700"));
                                Assert.That(evse_A1["coordinates"]?["longitude"]?.Value<String>(),  Is.EqualTo("16.38100"));

                                Assert.That(evse_A2["coordinates"]?["latitude"]?. Value<String>(),  Is.EqualTo("40.25700"));
                                Assert.That(evse_A2["coordinates"]?["longitude"]?.Value<String>(),  Is.EqualTo("16.38100"));

                            }

                        }

                    }

                }

                #endregion

            }

        }

        #endregion


        #region Update_EVSEConnectorPlugType_Test()

        /// <summary>
        /// Update the plug type of the connector of an EVSE.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the OCPI module.
        /// </summary>
        [Test]
        public async Task Update_EVSEConnectorPlugType_Test()
        {

            if (roamingNetwork  is not null &&
                graphDefinedCSO is not null &&
                csoAdapter      is not null)
            {

                var defaultDevices = await AddDefaultDevices();
                var chargingPool1     = defaultDevices.ChargingPool1;
                var chargingPool2     = defaultDevices.ChargingPool2;
                var chargingStation1  = defaultDevices.ChargingStation1;
                var chargingStation2  = defaultDevices.ChargingStation2;
                var chargingStation3  = defaultDevices.ChargingStation3;
                var evse1             = defaultDevices.EVSE1;
                var evse2             = defaultDevices.EVSE2;
                var evse3             = defaultDevices.EVSE3;
                var evse4             = defaultDevices.EVSE4;


                #region Validate via HTTP (OpenData, no authorization)

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Connection = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2.Count,  Is.EqualTo(1));

                                var evse_A1 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*1").First();

                                Assert.That(evse_A1["connectors"]?.First()?["standard"]?.Value<String>(),  Is.EqualTo("CHADEMO"));

                            }

                        }

                    }

                }

                #endregion

                #region Validate via HTTP (with authorization)

                await commonAPI!.AddRemoteParty(

                          CountryCode:         CountryCode.Parse("DE"),
                          PartyId:             Party_Id.Parse("GDF"),
                          Role:                Role.EMSP,
                          BusinessDetails:     new BusinessDetails(
                                                   "GraphDefined EMSP"
                                               ),

                          Status:              PartyStatus.ENABLED,

                          LocalAccessToken:    AccessToken.Parse("1234xyz"),
                          LocalAccessStatus:   AccessStatus.ALLOWED

                      );

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.Parse("1234xyz");
                                                         requestBuilder.Connection     = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2.Count,  Is.EqualTo(1));

                                var evse_A1 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*1").First();

                                Assert.That(evse_A1["connectors"]?.First()?["standard"]?.Value<String>(),  Is.EqualTo("CHADEMO"));

                            }

                        }

                    }

                }

                #endregion


                var a_p1ct1 = chargingPool1.Created;
                var a_p1lt1 = chargingPool1.LastChangeDate;

                var a_p2ct1 = chargingPool2.Created;
                var a_p2lt1 = chargingPool2.LastChangeDate;


                #region Update static EVSE data

                var updatedPoolProperties1     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedPoolProperties2     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedPoolProperties3     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedPoolProperties4     = new List<PropertyUpdateInfo<ChargingPool_Id>>();

                var updatedStationProperties1  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();
                var updatedStationProperties2  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();
                var updatedStationProperties3  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();
                var updatedStationProperties4  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();

                var updatedEVSEProperties1     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();
                var updatedEVSEProperties2     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();
                var updatedEVSEProperties3     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();
                var updatedEVSEProperties4     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();

                #region Subscribe charging pool events

                chargingPool1.OnPropertyChanged += (timestamp,
                                                     eventTrackingId,
                                                     chargingPool,
                                                     propertyName,
                                                     newValue,
                                                     oldValue,
                                                     dataSource) => {

                    updatedPoolProperties1.Add(new PropertyUpdateInfo<ChargingPool_Id>((chargingPool as IChargingPool)!.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                chargingPool1.OnDataChanged += (timestamp,
                                                 eventTrackingId,
                                                 chargingPool,
                                                 propertyName,
                                                 newValue,
                                                 oldValue,
                                                 dataSource) => {

                    updatedPoolProperties2.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingPoolDataChanged += (timestamp,
                                                              eventTrackingId,
                                                              chargingPool,
                                                              propertyName,
                                                              newValue,
                                                              oldValue,
                                                              dataSource) => {

                    updatedPoolProperties3.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                roamingNetwork.OnChargingPoolDataChanged  += (timestamp,
                                                              eventTrackingId,
                                                              chargingPool,
                                                              propertyName,
                                                              newValue,
                                                              oldValue,
                                                              dataSource) => {

                    updatedPoolProperties4.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue));
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

                    updatedStationProperties1.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>((chargingStation as IChargingStation)!.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                chargingStation1!.OnDataChanged += (timestamp,
                                                    eventTrackingId,
                                                    chargingStation,
                                                    propertyName,
                                                    newValue,
                                                    oldValue,
                                                    dataSource) => {

                    updatedStationProperties2.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingStationDataChanged += (timestamp,
                                                                 eventTrackingId,
                                                                 chargingStation,
                                                                 propertyName,
                                                                 newValue,
                                                                 oldValue,
                                                                 dataSource) => {

                    updatedStationProperties3.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                roamingNetwork.OnChargingStationDataChanged += (timestamp,
                                                                eventTrackingId,
                                                                chargingStation,
                                                                propertyName,
                                                                newValue,
                                                                oldValue,
                                                                dataSource) => {

                    updatedStationProperties4.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue));
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

                    updatedEVSEProperties1.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>((evse as IEVSE)!.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                evse1!.OnDataChanged += (timestamp,
                                         eventTrackingId,
                                         evse,
                                         propertyName,
                                         newValue,
                                         oldValue,
                                         dataSource) => {

                    updatedEVSEProperties2.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnEVSEDataChanged += (timestamp,
                                                      eventTrackingId,
                                                      evse,
                                                      propertyName,
                                                      newValue,
                                                      oldValue,
                                                      dataSource) => {

                    updatedEVSEProperties3.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                roamingNetwork.OnEVSEDataChanged += (timestamp,
                                                     eventTrackingId,
                                                     evse,
                                                     propertyName,
                                                     newValue,
                                                     oldValue,
                                                     dataSource) => {

                    updatedEVSEProperties4.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                #endregion

                var locationChanges           = new List<String>();
                var evseChanges               = new List<String>();

                csoAdapter.CommonAPI.OnLocationChanged += (location) => {
                    locationChanges.Add(location.ToJSON().ToString());
                    return Task.CompletedTask;
                };

                csoAdapter.CommonAPI.OnEVSEChanged     += (evse)     => {
                    evseChanges.    Add(evse.    ToJSON().ToString());
                    return Task.CompletedTask;
                };


                evse1.ChargingConnectors.Set([ new ChargingConnector(
                                                   ChargingPlugTypes.TESLA_Roadster
                                               )]);

                Assert.That(updatedEVSEProperties1.Count,                                                             Is.EqualTo(1));
                Assert.That(updatedEVSEProperties2.Count,                                                             Is.EqualTo(1));
                Assert.That(updatedEVSEProperties3.Count,                                                             Is.EqualTo(1));
                Assert.That(updatedEVSEProperties4.Count,                                                             Is.EqualTo(1));

                //Assert.That(updatedEVSEProperties1.First().ToString(),                                                Is.EqualTo("Update of 'DE*GEF*STATION*1*A'.'GeoLocation' 'Latitude = 50.82, Longitude = 11.52' -> 'Latitude = 40.257, Longitude = 16.381' !"));
                //Assert.That(updatedEVSEProperties2.First().ToString(),                                                Is.EqualTo("Update of 'DE*GEF*STATION*1*A'.'GeoLocation' 'Latitude = 50.82, Longitude = 11.52' -> 'Latitude = 40.257, Longitude = 16.381' !"));
                //Assert.That(updatedEVSEProperties3.First().ToString(),                                                Is.EqualTo("Update of 'DE*GEF*STATION*1*A'.'GeoLocation' 'Latitude = 50.82, Longitude = 11.52' -> 'Latitude = 40.257, Longitude = 16.381' !"));
                //Assert.That(updatedEVSEProperties4.First().ToString(),                                                Is.EqualTo("Update of 'DE*GEF*STATION*1*A'.'GeoLocation' 'Latitude = 50.82, Longitude = 11.52' -> 'Latitude = 40.257, Longitude = 16.381' !"));

                Assert.That(roamingNetwork.  GetEVSEById(evse1.Id)!.ChargingConnectors.First().Plug,                  Is.EqualTo(ChargingPlugTypes.TESLA_Roadster));
                Assert.That(graphDefinedCSO. GetEVSEById(evse1.Id)!.ChargingConnectors.First().Plug,                  Is.EqualTo(ChargingPlugTypes.TESLA_Roadster));
                Assert.That(chargingPool1.   GetEVSEById(evse1.Id)!.ChargingConnectors.First().Plug,                  Is.EqualTo(ChargingPlugTypes.TESLA_Roadster));
                Assert.That(chargingStation1.GetEVSEById(evse1.Id)!.ChargingConnectors.First().Plug,                  Is.EqualTo(ChargingPlugTypes.TESLA_Roadster));


                if (csoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1.Id.ToString()), out var location) &&
                    location.TryGetEVSE(evse1.Id.ToOCPI_EVSEUId()!.Value, out var evse))
                {
                    Assert.That(evse.Connectors.First().Standard,                                                     Is.EqualTo(ConnectorType.TESLA_R));
                }

                #endregion


                var b_p1ct1 = chargingPool1.Created;
                var b_p1lt1 = chargingPool1.LastChangeDate;

                var b_p2ct1 = chargingPool2.Created;
                var b_p2lt1 = chargingPool2.LastChangeDate;

                #region Validate via HTTP (OpenData, no authorization)

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Connection = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2.Count,  Is.EqualTo(1));

                                var evse_A1 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*1").First();

                                Assert.That(evse_A1["connectors"]?.First()?["standard"]?.Value<String>(),  Is.EqualTo("TESLA_R"));

                            }

                        }

                    }

                }

                #endregion

                #region Validate via HTTP (with authorization)

                await commonAPI!.AddRemoteParty(

                          CountryCode:         CountryCode.Parse("DE"),
                          PartyId:             Party_Id.Parse("GDF"),
                          Role:                Role.EMSP,
                          BusinessDetails:     new BusinessDetails(
                                                   "GraphDefined EMSP"
                                               ),

                          Status:              PartyStatus.ENABLED,

                          LocalAccessToken:    AccessToken.Parse("1234xyz"),
                          LocalAccessStatus:   AccessStatus.ALLOWED

                      );

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.Parse("1234xyz");
                                                         requestBuilder.Connection     = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2.Count,  Is.EqualTo(1));

                                var evse_A1 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*1").First();

                                Assert.That(evse_A1["connectors"]?.First()?["standard"]?.Value<String>(),  Is.EqualTo("TESLA_R"));

                            }

                        }

                    }

                }

                #endregion

            }

        }

        #endregion



        #region Update_EVSEPhysicalReference_Test()

        /// <summary>
        /// Update the physical reference of an EVSE.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the OCPI module.
        /// </summary>
        [Test]
        public async Task Update_EVSEPhysicalReference_Test()
        {

            if (roamingNetwork  is not null &&
                graphDefinedCSO is not null &&
                csoAdapter      is not null)
            {

                var defaultDevices = await AddDefaultDevices();
                var chargingPool1     = defaultDevices.ChargingPool1;
                var chargingPool2     = defaultDevices.ChargingPool2;
                var chargingStation1  = defaultDevices.ChargingStation1;
                var chargingStation2  = defaultDevices.ChargingStation2;
                var chargingStation3  = defaultDevices.ChargingStation3;
                var evse1             = defaultDevices.EVSE1;
                var evse2             = defaultDevices.EVSE2;
                var evse3             = defaultDevices.EVSE3;
                var evse4             = defaultDevices.EVSE4;


                #region Validate via HTTP (OpenData, no authorization)

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Connection = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2.Count,  Is.EqualTo(1));

                                var evse_A1 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*1").First();

                                Assert.That(evse_A1["physical_reference"]?.Value<String>(),  Is.Null);

                            }

                        }

                    }

                }

                #endregion

                #region Validate via HTTP (with authorization)

                await commonAPI!.AddRemoteParty(

                          CountryCode:         CountryCode.Parse("DE"),
                          PartyId:             Party_Id.Parse("GDF"),
                          Role:                Role.EMSP,
                          BusinessDetails:     new BusinessDetails(
                                                   "GraphDefined EMSP"
                                               ),

                          Status:              PartyStatus.ENABLED,

                          LocalAccessToken:    AccessToken.Parse("1234xyz"),
                          LocalAccessStatus:   AccessStatus.ALLOWED

                      );

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.Parse("1234xyz");
                                                         requestBuilder.Connection     = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2.Count,  Is.EqualTo(1));

                                var evse_A1 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*1").First();

                                Assert.That(evse_A1["physical_reference"]?.Value<String>(),  Is.Null);

                            }

                        }

                    }

                }

                #endregion


                var a_p1ct1 = chargingPool1.Created;
                var a_p1lt1 = chargingPool1.LastChangeDate;

                var a_p2ct1 = chargingPool2.Created;
                var a_p2lt1 = chargingPool2.LastChangeDate;


                #region Update static EVSE data

                var updatedPoolProperties1     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedPoolProperties2     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedPoolProperties3     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedPoolProperties4     = new List<PropertyUpdateInfo<ChargingPool_Id>>();

                var updatedStationProperties1  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();
                var updatedStationProperties2  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();
                var updatedStationProperties3  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();
                var updatedStationProperties4  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();

                var updatedEVSEProperties1     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();
                var updatedEVSEProperties2     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();
                var updatedEVSEProperties3     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();
                var updatedEVSEProperties4     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();

                #region Subscribe charging pool events

                chargingPool1.OnPropertyChanged += (timestamp,
                                                     eventTrackingId,
                                                     chargingPool,
                                                     propertyName,
                                                     newValue,
                                                     oldValue,
                                                     dataSource) => {

                    updatedPoolProperties1.Add(new PropertyUpdateInfo<ChargingPool_Id>((chargingPool as IChargingPool)!.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                chargingPool1.OnDataChanged += (timestamp,
                                                 eventTrackingId,
                                                 chargingPool,
                                                 propertyName,
                                                 newValue,
                                                 oldValue,
                                                 dataSource) => {

                    updatedPoolProperties2.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingPoolDataChanged += (timestamp,
                                                              eventTrackingId,
                                                              chargingPool,
                                                              propertyName,
                                                              newValue,
                                                              oldValue,
                                                              dataSource) => {

                    updatedPoolProperties3.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                roamingNetwork.OnChargingPoolDataChanged  += (timestamp,
                                                              eventTrackingId,
                                                              chargingPool,
                                                              propertyName,
                                                              newValue,
                                                              oldValue,
                                                              dataSource) => {

                    updatedPoolProperties4.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue));
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

                    updatedStationProperties1.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>((chargingStation as IChargingStation)!.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                chargingStation1!.OnDataChanged += (timestamp,
                                                    eventTrackingId,
                                                    chargingStation,
                                                    propertyName,
                                                    newValue,
                                                    oldValue,
                                                    dataSource) => {

                    updatedStationProperties2.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingStationDataChanged += (timestamp,
                                                                 eventTrackingId,
                                                                 chargingStation,
                                                                 propertyName,
                                                                 newValue,
                                                                 oldValue,
                                                                 dataSource) => {

                    updatedStationProperties3.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                roamingNetwork.OnChargingStationDataChanged += (timestamp,
                                                                eventTrackingId,
                                                                chargingStation,
                                                                propertyName,
                                                                newValue,
                                                                oldValue,
                                                                dataSource) => {

                    updatedStationProperties4.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue));
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

                    updatedEVSEProperties1.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>((evse as IEVSE)!.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                evse1!.OnDataChanged += (timestamp,
                                         eventTrackingId,
                                         evse,
                                         propertyName,
                                         newValue,
                                         oldValue,
                                         dataSource) => {

                    updatedEVSEProperties2.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnEVSEDataChanged += (timestamp,
                                                      eventTrackingId,
                                                      evse,
                                                      propertyName,
                                                      newValue,
                                                      oldValue,
                                                      dataSource) => {

                    updatedEVSEProperties3.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                roamingNetwork.OnEVSEDataChanged += (timestamp,
                                                     eventTrackingId,
                                                     evse,
                                                     propertyName,
                                                     newValue,
                                                     oldValue,
                                                     dataSource) => {

                    updatedEVSEProperties4.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue));
                    return Task.CompletedTask;

                };

                #endregion

                var locationChanges           = new List<String>();
                var evseChanges               = new List<String>();

                csoAdapter.CommonAPI.OnLocationChanged += (location) => {
                    locationChanges.Add(location.ToJSON().ToString());
                    return Task.CompletedTask;
                };

                csoAdapter.CommonAPI.OnEVSEChanged     += (evse)     => {
                    evseChanges.    Add(evse.    ToJSON().ToString());
                    return Task.CompletedTask;
                };


                evse1.PhysicalReference = "E-V-S-E-#1";

                Assert.That(updatedEVSEProperties1.Count,                               Is.EqualTo(1));
                Assert.That(updatedEVSEProperties2.Count,                               Is.EqualTo(1));
                Assert.That(updatedEVSEProperties3.Count,                               Is.EqualTo(1));
                Assert.That(updatedEVSEProperties4.Count,                               Is.EqualTo(1));

                Assert.That(updatedEVSEProperties1.First().ToString(),                  Is.EqualTo("Update of 'DE*GEF*EVSE*1*A*1'.'PhysicalReference' '' -> 'E-V-S-E-#1' !"));
                Assert.That(updatedEVSEProperties2.First().ToString(),                  Is.EqualTo("Update of 'DE*GEF*EVSE*1*A*1'.'PhysicalReference' '' -> 'E-V-S-E-#1' !"));
                Assert.That(updatedEVSEProperties3.First().ToString(),                  Is.EqualTo("Update of 'DE*GEF*EVSE*1*A*1'.'PhysicalReference' '' -> 'E-V-S-E-#1' !"));
                Assert.That(updatedEVSEProperties4.First().ToString(),                  Is.EqualTo("Update of 'DE*GEF*EVSE*1*A*1'.'PhysicalReference' '' -> 'E-V-S-E-#1' !"));

                Assert.That(roamingNetwork.  GetEVSEById(evse1.Id)!.PhysicalReference,  Is.EqualTo("E-V-S-E-#1"));
                Assert.That(graphDefinedCSO. GetEVSEById(evse1.Id)!.PhysicalReference,  Is.EqualTo("E-V-S-E-#1"));
                Assert.That(chargingPool1.   GetEVSEById(evse1.Id)!.PhysicalReference,  Is.EqualTo("E-V-S-E-#1"));
                Assert.That(chargingStation1.GetEVSEById(evse1.Id)!.PhysicalReference,  Is.EqualTo("E-V-S-E-#1"));


                if (csoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1.Id.ToString()), out var location) &&
                    location.TryGetEVSE(evse1.Id.ToOCPI_EVSEUId()!.Value, out var evse))
                {
                    Assert.That(evse.PhysicalReference,                                 Is.EqualTo("E-V-S-E-#1"));
                }


                // CONNECTOR
                // .standard
                // .format
                // .power_type
                // .voltage
                // .amperage

                //Assert.That(updatedPoolProperties1.Count,  Is.EqualTo(2));
                //Assert.That(updatedPoolProperties2.Count,  Is.EqualTo(2));
                //Assert.That(updatedPoolProperties3.Count,  Is.EqualTo(2));
                //Assert.That(updatedPoolProperties4.Count,  Is.EqualTo(2));

                //Assert.That(graphDefinedCSO.GetChargingPoolById(chargingPool1.Id)!.Address!.Street,  Is.EqualTo("Amselfeld"));

                //csoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1.Id.ToString()), out location);
                //Assert.That(location!.City,                Is.EqualTo("NeuJena"));
                //Assert.That(location!.PostalCode,          Is.EqualTo("07742"));




                //evse1.Name.Set(Languages.en, "Test EVSE #1A1 (updated)");





                //var updateChargingPoolResult2 = await graphDefinedCSO.UpdateChargingPool()


                //var updateChargingPoolResult2 = await chargingPool2.Address = new Address(

                //                                                                  Street:             "Biberweg",
                //                                                                  PostalCode:         "07749",
                //                                                                  City:               I18NString.Create(Languages.de, "Jena"),
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

                //ClassicAssert.IsNotNull(addChargingPoolResult2);

                //var chargingPool2  = addChargingPoolResult2.ChargingPool;
                //ClassicAssert.IsNotNull(chargingPool2);

                #endregion


                var b_p1ct1 = chargingPool1.Created;
                var b_p1lt1 = chargingPool1.LastChangeDate;

                var b_p2ct1 = chargingPool2.Created;
                var b_p2lt1 = chargingPool2.LastChangeDate;

                #region Validate via HTTP (OpenData, no authorization)

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Connection = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2.Count,  Is.EqualTo(1));

                                var evse_A1 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*1").First();

                                Assert.That(evse_A1["physical_reference"]?.Value<String>(),  Is.EqualTo("E-V-S-E-#1"));

                            }

                        }

                    }

                }

                #endregion

                #region Validate via HTTP (with authorization)

                await commonAPI!.AddRemoteParty(

                          CountryCode:         CountryCode.Parse("DE"),
                          PartyId:             Party_Id.Parse("GDF"),
                          Role:                Role.EMSP,
                          BusinessDetails:     new BusinessDetails(
                                                   "GraphDefined EMSP"
                                               ),

                          Status:              PartyStatus.ENABLED,

                          LocalAccessToken:    AccessToken.Parse("1234xyz"),
                          LocalAccessStatus:   AccessStatus.ALLOWED

                      );

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.Parse("1234xyz");
                                                         requestBuilder.Connection     = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2.Count,  Is.EqualTo(1));

                                var evse_A1 = jsonEVSEs1.Where(evse => evse["evse_id"]?.Value<String>() == "DE*GEF*EVSE*1*A*1").First();

                                Assert.That(evse_A1["physical_reference"]?.Value<String>(),  Is.EqualTo("E-V-S-E-#1"));

                            }

                        }

                    }

                }

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

            if (roamingNetwork  is not null &&
                graphDefinedCSO is not null &&
                csoAdapter      is not null)
            {

                var defaultDevices = await AddDefaultDevices();
                var chargingPool1     = defaultDevices.ChargingPool1;
                var chargingPool2     = defaultDevices.ChargingPool2;
                var chargingStation1  = defaultDevices.ChargingStation1;
                var chargingStation2  = defaultDevices.ChargingStation2;
                var chargingStation3  = defaultDevices.ChargingStation3;
                var evse1             = defaultDevices.EVSE1;
                var evse2             = defaultDevices.EVSE2;
                var evse3             = defaultDevices.EVSE3;
                var evse4             = defaultDevices.EVSE4;


                var evse1_UId = evse1.Id.ToOCPI_EVSEUId();
                Assert.That(evse1_UId.HasValue,  Is.True);


                #region Validate via HTTP (OpenData, no authorization)

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Connection = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1?.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2?.Count,  Is.EqualTo(1));

                                foreach (var jsonEVSE in jsonEVSEs1!)
                                {
                                    if (jsonEVSE is JObject jobjectEVSE && jobjectEVSE["evse_id"]?.Value<String>() == evse1_UId!.ToString())
                                    {
                                        Assert.That(jobjectEVSE["status"]?.Value<String>(),  Is.EqualTo("AVAILABLE"));
                                    }
                                }

                            }

                        }

                    }

                }

                #endregion

                #region Validate via HTTP (with authorization)

                await commonAPI!.AddRemoteParty(

                          CountryCode:         CountryCode.Parse("DE"),
                          PartyId:             Party_Id.Parse("GDF"),
                          Role:                Role.EMSP,
                          BusinessDetails:     new BusinessDetails(
                                                   "GraphDefined EMSP"
                                               ),

                          Status:              PartyStatus.ENABLED,

                          LocalAccessToken:    AccessToken.Parse("1234xyz"),
                          LocalAccessStatus:   AccessStatus.ALLOWED

                      );

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.Parse("1234xyz");
                                                         requestBuilder.Connection     = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1?.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2?.Count,  Is.EqualTo(1));

                                foreach (var jsonEVSE in jsonEVSEs1!)
                                {
                                    if (jsonEVSE is JObject jobjectEVSE && jobjectEVSE["evse_id"]?.Value<String>() == evse1_UId!.ToString())
                                    {
                                        Assert.That(jobjectEVSE["status"]?.Value<String>(),  Is.EqualTo("AVAILABLE"));
                                    }
                                }

                            }

                        }

                    }

                }

                #endregion


                #region Subscribe WWCP EVSE events

                var updatedEVSEStatus1        = new List<EVSEStatusUpdate>();
                var updatedEVSEStatus2        = new List<EVSEStatusUpdate>();
                var updatedEVSEStatus3        = new List<EVSEStatusUpdate>();

                evse1.OnStatusChanged += (timestamp,
                                           eventTrackingId,
                                           evse,
                                           newEVSEStatus,
                                           oldEVSEStatus,
                                           dataSource) => {

                    updatedEVSEStatus1.Add(new EVSEStatusUpdate(evse.Id, newEVSEStatus, oldEVSEStatus, dataSource));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnEVSEStatusChanged += (timestamp,
                                                        eventTrackingId,
                                                        evse,
                                                        newEVSEStatus,
                                                        oldEVSEStatus,
                                                        dataSource) => {

                    updatedEVSEStatus2.Add(new EVSEStatusUpdate(evse.Id, newEVSEStatus, oldEVSEStatus, dataSource));
                    return Task.CompletedTask;

                };

                roamingNetwork.OnEVSEStatusChanged += (timestamp,
                                                       eventTrackingId,
                                                       evse,
                                                       newEVSEStatus,
                                                       oldEVSEStatus,
                                                       dataSource) => {

                    updatedEVSEStatus3.Add(new EVSEStatusUpdate(evse.Id, newEVSEStatus, oldEVSEStatus, dataSource));
                    return Task.CompletedTask;

                };

                #endregion

                #region Subscribe OCPI EVSE events

                var updatedOCPIEVSEStatus1 = new List<StatusType>();
                var updatedOCPIEVSEStatus2 = new List<StatusType>();

                csoAdapter.CommonAPI.OnEVSEChanged += (evse) => {

                    updatedOCPIEVSEStatus1.Add(evse.Status);
                    return Task.CompletedTask;

                };

                csoAdapter.CommonAPI.OnEVSEStatusChanged += (timestamp,
                                                             evse,
                                                             newEVSEStatus,
                                                             oldEVSEStatus) => {

                    updatedOCPIEVSEStatus2.Add(newEVSEStatus);
                    return Task.CompletedTask;

                };

                #endregion


                #region Update

                {
                    if (evse1_UId.HasValue &&
                        csoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1.Id.Suffix), out var location) &&
                        location.TryGetEVSE(evse1_UId.Value, out var ocpiEVSE))
                    {
                        Assert.That(ocpiEVSE.Status,  Is.EqualTo(StatusType.AVAILABLE));
                    }
                }


                evse1.SetStatus(EVSEStatusType.Charging);

                await Task.Delay(300);


                // WWCP
                Assert.That(updatedEVSEStatus1.    Count,                         Is.EqualTo(1));
                Assert.That(updatedEVSEStatus2.    Count,                         Is.EqualTo(1));
                Assert.That(updatedEVSEStatus3.    Count,                         Is.EqualTo(1));

                // OCPI
                Assert.That(updatedOCPIEVSEStatus1.Count,                         Is.EqualTo(1));
                Assert.That(updatedOCPIEVSEStatus2.Count,                         Is.EqualTo(1));

                Assert.That(graphDefinedCSO.GetEVSEById(evse1.Id)?.Status.Value,  Is.EqualTo(EVSEStatusType.Charging));

                {
                    if (evse1_UId.HasValue &&
                        csoAdapter.CommonAPI.TryGetLocation(Location_Id.Parse(chargingPool1.Id.Suffix), out var location) &&
                        location.TryGetEVSE(evse1_UId.Value, out var ocpiEVSE) && ocpiEVSE is not null)
                    {
                        Assert.That(ocpiEVSE.Status,  Is.EqualTo(StatusType.CHARGING));
                    }
                }

                #endregion


                #region Validate via HTTP (OpenData, no authorization)

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Connection = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1?.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2?.Count,  Is.EqualTo(1));

                                foreach (var jsonEVSE in jsonEVSEs1!)
                                {
                                    if (jsonEVSE is JObject jobjectEVSE && jobjectEVSE["evse_id"]?.Value<String>() == evse1_UId!.ToString())
                                    {
                                        Assert.That(jobjectEVSE["status"]?.Value<String>(),  Is.EqualTo("CHARGING"));
                                    }
                                }

                            }

                        }

                    }

                }

                #endregion

                #region Validate via HTTP (with authorization)

                await commonAPI!.AddRemoteParty(

                          CountryCode:         CountryCode.Parse("DE"),
                          PartyId:             Party_Id.Parse("GDF"),
                          Role:                Role.EMSP,
                          BusinessDetails:     new BusinessDetails(
                                                   "GraphDefined EMSP"
                                               ),

                          Status:              PartyStatus.ENABLED,

                          LocalAccessToken:    AccessToken.Parse("1234xyz"),
                          LocalAccessStatus:   AccessStatus.ALLOWED

                      );

                {

                    var httpResponse = await new HTTPSClient(remoteLocationsURL).
                                                 GET(remoteLocationsURL.Path,
                                                     RequestBuilder: requestBuilder => {
                                                         requestBuilder.Authorization  = HTTPTokenAuthentication.Parse("1234xyz");
                                                         requestBuilder.Connection     = ConnectionType.Close;
                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                         requestBuilder.Set("X-Request-ID",      "123");
                                                         requestBuilder.Set("X-Correlation-ID",  "123");
                                                     });

                    Assert.That(httpResponse,                                                        Is.Not.Null);
                    Assert.That(httpResponse.HTTPStatusCode.Code,                                    Is.EqualTo(200));

                    var ocpiResponse = JObject.Parse(httpResponse.HTTPBody?.ToUTF8String() ?? "{}");
                    Assert.That(ocpiResponse,                                                        Is.Not.Null);
                    Assert.That(ocpiResponse["status_code"]?.   Value<Int32>(),                      Is.EqualTo(1000));
                    Assert.That(ocpiResponse["status_message"]?.Value<String>(),                     Is.EqualTo("Hello world!"));
                    Assert.That(Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                    var jsonLocations = ocpiResponse["data"] as JArray;
                    Assert.That(jsonLocations,                                                       Is.Not.Null);
                    Assert.That(jsonLocations?.Count,                                                Is.EqualTo(2));

                    if (jsonLocations is not null)
                    {

                        var jsonLocation1 = jsonLocations[0] as JObject;
                        var jsonLocation2 = jsonLocations[1] as JObject;
                        Assert.That(jsonLocation1,  Is.Not.Null);
                        Assert.That(jsonLocation2,  Is.Not.Null);

                        if (jsonLocation1 is not null &&
                            jsonLocation2 is not null)
                        {

                            var jsonEVSEs1    = jsonLocation1["evses"] as JArray;
                            var jsonEVSEs2    = jsonLocation2["evses"] as JArray;
                            Assert.That(jsonEVSEs1,  Is.Not.Null);
                            Assert.That(jsonEVSEs2,  Is.Not.Null);

                            if (jsonEVSEs1 is not null &&
                                jsonEVSEs2 is not null)
                            {

                                Assert.That(jsonEVSEs1?.Count,  Is.EqualTo(3));
                                Assert.That(jsonEVSEs2?.Count,  Is.EqualTo(1));

                                foreach (var jsonEVSE in jsonEVSEs1!)
                                {
                                    if (jsonEVSE is JObject jobjectEVSE && jobjectEVSE["evse_id"]?.Value<String>() == evse1_UId!.ToString())
                                    {
                                        Assert.That(jobjectEVSE["status"]?.Value<String>(),  Is.EqualTo("CHARGING"));
                                    }
                                }

                            }

                        }

                    }

                }

                #endregion


            }

        }

        #endregion


    }

}
