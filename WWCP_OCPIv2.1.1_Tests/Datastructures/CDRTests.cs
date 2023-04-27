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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.Datastructures
{

    /// <summary>
    /// Unit tests for charge detail records.
    /// https://github.com/ocpi/ocpi/blob/master/mod_cdrs.asciidoc
    /// </summary>
    [TestFixture]
    public static class CDRTests
    {

        #region CDR_SerializeDeserialize_Test01()

        /// <summary>
        /// Charge Detail Record serialize, deserialize and compare test.
        /// </summary>
        [Test]
        public static void CDR_SerializeDeserialize_Test01()
        {

            #region Defined CDR1

            var cdr1 = new CDR(CountryCode.Parse("DE"),
                               Party_Id.   Parse("GEF"),
                               CDR_Id.     Parse("CDR0001"),
                               DateTime.   Parse("2020-04-12T18:20:19Z").ToUniversalTime(),
                               DateTime.   Parse("2020-04-12T22:20:19Z").ToUniversalTime(),
                               Auth_Id.    Parse("1234"),
                               AuthMethods.AUTH_REQUEST,
                               new Location(
                                   CountryCode. Parse("DE"),
                                   Party_Id.    Parse("GEF"),
                                   Location_Id. Parse("LOC0001"),
                                   LocationType.UNDERGROUND_GARAGE,
                                   "Biberweg 18",
                                   "Jena",
                                   "07749",
                                   Country.Germany,
                                   GeoCoordinate.   Parse(10, 20)
                               ),
                               Currency.EUR,

                               new[] {
                                   new ChargingPeriod(
                                       DateTime.Parse("2020-04-12T18:21:49Z"),
                                       new[] {
                                           new CDRDimension(
                                               CDRDimensionType.ENERGY,
                                               1.33M
                                           )
                                       }
                                   ),
                                   new ChargingPeriod(
                                       DateTime.Parse("2020-04-12T18:21:50Z"),
                                       new[] {
                                           new CDRDimension(
                                               CDRDimensionType.TIME,
                                               5.12M
                                           )
                                       }
                                   )
                               },

                               // Total cost
                               10.00M,

                               // Total Energy
                               50.00M,

                               // Total time
                               TimeSpan.              FromMinutes(30),
                               Meter_Id.              Parse("Meter0815"),

                               // OCPI Computer Science Extensions
                               new EnergyMeter(
                                   Meter_Id.Parse("Meter0815"),
                                   "EnergyMeter Model #1",
                                   null,
                                   "hw. v1.80",
                                   "fw. v1.20",
                                   "Energy Metering Services",
                                   null,
                                   null,
                                   null,
                                   new[] {
                                       new TransparencySoftwareStatus(
                                           new TransparencySoftware(
                                               "Chargy Transparency Software Desktop Application",
                                               "v1.00",
                                               OpenSourceLicense.AGPL3,
                                               "GraphDefined GmbH",
                                               URL.Parse("https://open.charging.cloud/logo.svg"),
                                               URL.Parse("https://open.charging.cloud/Chargy/howto"),
                                               URL.Parse("https://open.charging.cloud/Chargy"),
                                               URL.Parse("https://github.com/OpenChargingCloud/ChargyDesktopApp")
                                           ),
                                           LegalStatus.GermanCalibrationLaw,
                                           "cert",
                                           "German PTB",
                                           NotBefore: DateTime.Parse("2019-04-01T00:00:00.000Z").ToUniversalTime(),
                                           NotAfter:  DateTime.Parse("2030-01-01T00:00:00.000Z").ToUniversalTime()
                                       ),
                                       new TransparencySoftwareStatus(
                                           new TransparencySoftware(
                                               "Chargy Transparency Software Mobile Application",
                                               "v1.00",
                                               OpenSourceLicense.AGPL3,
                                               "GraphDefined GmbH",
                                               URL.Parse("https://open.charging.cloud/logo.svg"),
                                               URL.Parse("https://open.charging.cloud/Chargy/howto"),
                                               URL.Parse("https://open.charging.cloud/Chargy"),
                                               URL.Parse("https://github.com/OpenChargingCloud/ChargyMobileApp")
                                           ),
                                           LegalStatus.ForInformationOnly,
                                           "no cert",
                                           "GraphDefined",
                                           NotBefore: DateTime.Parse("2019-04-01T00:00:00.000Z").ToUniversalTime(),
                                           NotAfter:  DateTime.Parse("2030-01-01T00:00:00.000Z").ToUniversalTime()
                                       )
                                   }
                               ),
                               null,

                               new[] {
                                   new Tariff(
                                       CountryCode.Parse("DE"),
                                       Party_Id.   Parse("GEF"),
                                       Tariff_Id.  Parse("TARIFF0001"),
                                       Currency.EUR,
                                       new[] {
                                           new TariffElement(
                                               new[] {
                                                   PriceComponent.ChargingTime(
                                                       TimeSpan.FromSeconds(300),
                                                       2.00M
                                                   )
                                               },
                                               new[] {
                                                   new TariffRestrictions(
                                                       Time.FromHourMin(08,00),       // Start time
                                                       Time.FromHourMin(18,00),       // End time
                                                       DateTime.Parse("2020-12-01"),  // Start timestamp
                                                       DateTime.Parse("2020-12-31"),  // End timestamp
                                                       1.12M,                         // MinkWh
                                                       5.67M,                         // MaxkWh
                                                       1.49M,                         // MinPower
                                                       9.91M,                         // MaxPower
                                                       TimeSpan.FromMinutes(10),      // MinDuration
                                                       TimeSpan.FromMinutes(30),      // MaxDuration
                                                       new DayOfWeek[] {
                                                           DayOfWeek.Monday,
                                                           DayOfWeek.Tuesday
                                                       }
                                                   )
                                               }
                                           )
                                       },
                                       new[] {
                                           new DisplayText(Languages.de, "Hallo Welt!"),
                                           new DisplayText(Languages.en, "Hello world!"),
                                       },
                                       URL.Parse("https://open.charging.cloud"),
                                       new EnergyMix(
                                           true,
                                           new[] {
                                               new EnergySource(
                                                   EnergySourceCategory.SOLAR,
                                                   80
                                               ),
                                               new EnergySource(
                                                   EnergySourceCategory.WIND,
                                                   20
                                               )
                                           },
                                           new[] {
                                               new EnvironmentalImpact(
                                                   EnvironmentalImpactCategory.CARBON_DIOXIDE,
                                                   0.1
                                               )
                                           },
                                           "Stadtwerke Jena-Ost",
                                           "New Green Deal"
                                       ),
                                       DateTime.Parse("2020-09-22").ToUniversalTime()
                                   )
                               },

                               new SignedData(
                                   EncodingMethod.GraphDefined,
                                   new[] {
                                       new SignedValue(
                                           SignedValueNature.START,
                                           "PlainStartValue",
                                           "SignedStartValue"
                                       ),
                                       new SignedValue(
                                           SignedValueNature.INTERMEDIATE,
                                           "PlainIntermediateValue",
                                           "SignedIntermediateValue"
                                       ),
                                       new SignedValue(
                                           SignedValueNature.END,
                                           "PlainEndValue",
                                           "SignedEndValue"
                                       )
                                   },
                                   1,     // Encoding method version
                                   null,  // Public key
                                   URL.Parse("https://open.charging.cloud/pools/1/stations/1/evse/1/publicKey")
                               ),

                               // Total Parking Time
                               TimeSpan.FromMinutes(120),

                               "Remark!",

                               DateTime.Parse("2020-09-12").ToUniversalTime()

                           );

            #endregion

            var JSON = cdr1.ToJSON();

            //Assert.AreEqual("DE",                          JSON["country_code"]?.Value<String>());
            //Assert.AreEqual("GEF",                         JSON["party_id"]?.    Value<String>());
            Assert.AreEqual("CDR0001",                     JSON["id"]?.          Value<String>());


            Assert.IsTrue(CDR.TryParse(JSON,
                                       out var parsedCDR,
                                       out var errorResponse,
                                       CountryCode.Parse("DE"),
                                       Party_Id.   Parse("GEF")));

            Assert.IsNotNull(parsedCDR);
            Assert.IsNull   (errorResponse);

            Assert.AreEqual (cdr1.CountryCode,              parsedCDR!.CountryCode);
            Assert.AreEqual (cdr1.PartyId,                  parsedCDR!.PartyId);
            Assert.AreEqual (cdr1.Id,                       parsedCDR!.Id);

            Assert.AreEqual (cdr1.Start.ToIso8601(),        parsedCDR!.Start.ToIso8601());
            Assert.AreEqual (cdr1.End.  ToIso8601(),        parsedCDR!.End.  ToIso8601());
            Assert.AreEqual (cdr1.AuthId,                   parsedCDR!.AuthId);
            Assert.AreEqual (cdr1.AuthMethod,               parsedCDR!.AuthMethod);
            Assert.IsTrue   (cdr1.Location.Equals(parsedCDR!.Location));
            Assert.AreEqual (cdr1.Currency,                 parsedCDR!.Currency);
            Assert.AreEqual (cdr1.ChargingPeriods,          parsedCDR!.ChargingPeriods);
            Assert.AreEqual (cdr1.TotalCost,                parsedCDR!.TotalCost);
            Assert.AreEqual (cdr1.TotalEnergy,              parsedCDR!.TotalEnergy);
            Assert.AreEqual (cdr1.TotalTime,                parsedCDR!.TotalTime);

            Assert.AreEqual (cdr1.MeterId,                  parsedCDR!.MeterId);
            Assert.AreEqual (cdr1.EnergyMeter,              parsedCDR!.EnergyMeter);
            Assert.AreEqual (cdr1.TransparencySoftwares,    parsedCDR!.TransparencySoftwares);
            Assert.AreEqual (cdr1.Tariffs,                  parsedCDR!.Tariffs);
            Assert.AreEqual (cdr1.SignedData,               parsedCDR!.SignedData);
            Assert.AreEqual (cdr1.TotalParkingTime,         parsedCDR!.TotalParkingTime);
            Assert.AreEqual (cdr1.Remark,                   parsedCDR!.Remark);

            Assert.AreEqual (cdr1.LastUpdated.ToIso8601(),  parsedCDR!.LastUpdated.ToIso8601());

        }

        #endregion


        #region CDR_DeserializeGitHub_Test01()

        /// <summary>
        /// Tries to deserialize a charge detail record example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_cdrs.md#example-of-a-cdr
        /// </summary>
        [Test]
        public static void CDR_DeserializeGitHub_Test01()
        {

            #region Define JSON

            var JSON = @"{
                           ""id"": ""12345"",
                           ""start_date_time"": ""2015-06-29T21:39:09Z"",
                           ""end_date_time"": ""2015-06-29T23:37:32Z"",
                           ""auth_id"": ""012345678"",
                           ""auth_method"": ""WHITELIST"",
                           ""location"": {
                             ""id"": ""LOC1"",
                             ""location_type"": ""UNDERGROUND_GARAGE"",
                             ""address"": ""F.Rooseveltlaan 3A"",
                             ""city"": ""Gent"",
                             ""postal_code"": ""9000"",
                             ""country"": ""BEL"",
                             ""coordinates"": {
                               ""latitude"": ""3.729944"",
                               ""longitude"": ""51.047599""
                             },
                             ""evses"": [{
                                 ""uid"": ""3256"",
                                 ""evse_id"": ""BE-BEC-E041503003"",
                                 ""status"": ""AVAILABLE"",
                                 ""connectors"": [{
                                     ""id"": ""1"",
                                     ""standard"": ""IEC_62196_T2"",
                                     ""format"": ""SOCKET"",
                                     ""power_type"": ""AC_1_PHASE"",
                                     ""voltage"": 230,
                                     ""amperage"": 64,
                                     ""tariff_id"": ""11"",
                                     ""last_updated"": ""2015-06-29T21:39:01Z""
                                 }],
                                 ""last_updated"": ""2015-06-29T21:39:01Z""
                             }],
                             ""last_updated"": ""2015-06-29T21:39:01Z""
                           },
                           ""currency"": ""EUR"",
                           ""tariffs"": [{
                             ""id"": ""12"",
                             ""currency"": ""EUR"",
                             ""elements"": [{
                               ""price_components"": [{
                                 ""type"": ""TIME"",
                                 ""price"": 2.00,
                                 ""vat"": 10.0,
                                 ""step_size"": 300
                               }]
                             }],
                             ""last_updated"": ""2015-02-02T14:15:01Z""
                           }],
                           ""charging_periods"": [{
                             ""start_date_time"": ""2015-06-29T21:39:09Z"",
                             ""dimensions"": [{
                               ""type"": ""TIME"",
                               ""volume"": 1.973
                             }],
                             ""tariff_id"": ""12""
                           }],
                           ""total_cost"": 4.00,
                           ""total_energy"": 15.342,
                           ""total_time"": 1.973,
                           ""meter_id"": ""metr123"",
                           ""remark"": ""CDR_DeserializeGitHub_Test01()"",
                           ""last_updated"": ""2015-06-29T22:01:13Z""
                         }";

            #endregion

            Assert.IsTrue(CDR.TryParse(JObject.Parse(JSON),
                                       out var parsedCDR,
                                       out var errorResponse,
                                       CountryCode.Parse("DE"),
                                       Party_Id.   Parse("GEF")));

            Assert.IsNotNull(parsedCDR);
            Assert.IsNull   (errorResponse);

            Assert.AreEqual (CDR_Id.Parse("12345"),                                 parsedCDR!.Id);
            //Assert.AreEqual(true,                                                  parsedCDR.Publish);
            //Assert.AreEqual(cdr1.Start.    ToIso8601(),                            parsedCDR.Start.    ToIso8601());
            //Assert.AreEqual(cdr1.End.Value.ToIso8601(),                            parsedCDR.End.Value.ToIso8601());
            //Assert.AreEqual(cdr1.kWh,                                              parsedCDR.kWh);
            //Assert.AreEqual(cdr1.CDRToken,                                         parsedCDR.CDRToken);
            //Assert.AreEqual(cdr1.AuthMethod,                                       parsedCDR.AuthMethod);
            //Assert.AreEqual(cdr1.AuthorizationReference,                           parsedCDR.AuthorizationReference);
            //Assert.AreEqual(cdr1.CDRId,                                            parsedCDR.CDRId);
            //Assert.AreEqual(cdr1.EVSEUId,                                          parsedCDR.EVSEUId);
            //Assert.AreEqual(cdr1.ConnectorId,                                      parsedCDR.ConnectorId);
            //Assert.AreEqual(cdr1.MeterId,                                          parsedCDR.MeterId);
            //Assert.AreEqual(cdr1.EnergyMeter,                                      parsedCDR.EnergyMeter);
            //Assert.AreEqual(cdr1.TransparencySoftwares,                            parsedCDR.TransparencySoftwares);
            //Assert.AreEqual(cdr1.Currency,                                         parsedCDR.Currency);
            //Assert.AreEqual(cdr1.ChargingPeriods,                                  parsedCDR.ChargingPeriods);
            //Assert.AreEqual(cdr1.TotalCosts,                                       parsedCDR.TotalCosts);
            //Assert.AreEqual(cdr1.Status,                                           parsedCDR.Status);
            //Assert.AreEqual(cdr1.LastUpdated.ToIso8601(),                          parsedCDR.LastUpdated.ToIso8601());

        }

        #endregion


        #region CDR_WWCP_2_OCPI_Test01()

        /// <summary>
        /// WWCP CDR to OCPI CDR conversion test.
        /// </summary>
        [Test]
        public static async Task CDR_WWCP_2_OCPI_Test01()
        {

            #region Add DE*GEF

            var cso = new ChargingStationOperator(
                          ChargingStationOperator_Id.Parse("DE*GEF"),
                          new RoamingNetwork(
                              RoamingNetwork_Id.Parse("test")
                          ),
                          I18NString.Create(Languages.en, "GraphDefined CSO")
                      );

            #endregion

            #region Add DE*GEF*POOL1

            var addChargingPoolResult1 = await cso.CreateChargingPool(

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
                                             InitialStatus:        ChargingPoolStatusTypes.Available

                                         );

            Assert.IsNotNull(addChargingPoolResult1);

            var chargingPool1 = addChargingPoolResult1.ChargingPool;
            Assert.IsNotNull(chargingPool1);

            #endregion

            #region Add DE*GEF*STATION*1*A

            var addChargingStationResult1 = await chargingPool1!.CreateChargingStation(

                                                Id:                   ChargingStation_Id.Parse("DE*GEF*STATION*1*A"),
                                                Name:                 I18NString.Create(Languages.en, "Test station #1A"),
                                                Description:          I18NString.Create(Languages.en, "GraphDefined charging station for tests #1A"),

                                                GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                InitialStatus:        ChargingStationStatusTypes.Available

                                            );

            Assert.IsNotNull(addChargingStationResult1);

            var chargingStation1  = addChargingStationResult1.ChargingStation;
            Assert.IsNotNull(chargingStation1);

            #endregion

            #region Add EVSE DE*GEF*EVSE*1*A*1

            var addEVSE1Result1 = await chargingStation1!.CreateEVSE(

                                      Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*1"),
                                      Name:                 I18NString.Create(Languages.en, "Test EVSE #1A1"),
                                      Description:          I18NString.Create(Languages.en, "GraphDefined EVSE for tests #1A1"),

                                      InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                      InitialStatus:        EVSEStatusTypes.Available,

                                      ChargingConnectors:        new ChargingConnector[] {
                                                                new ChargingConnector(
                                                                    Id:              ChargingConnector_Id.Parse("1"),
                                                                    Plug:            ChargingPlugTypes.Type2Outlet,
                                                                    Lockable:        true,
                                                                    CableAttached:   true,
                                                                    CableLength:     4
                                                                )
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

                                      ChargingConnectors:   new ChargingConnector[] {
                                                                new ChargingConnector(
                                                                    Id:              ChargingConnector_Id.Parse("2"),
                                                                    Plug:            ChargingPlugTypes.TypeFSchuko,
                                                                    Lockable:        false,
                                                                    CableAttached:   false
                                                                )
                                                            }

                                  );

            Assert.IsNotNull(addEVSE1Result2);

            var evse2     = addEVSE1Result2.EVSE;
            Assert.IsNotNull(evse2);

            #endregion


            var startTime = Timestamp.Now - TimeSpan.FromHours(3);


            var wwcpCDR = new ChargeDetailRecord(

                              Id:                             ChargeDetailRecord_Id.NewRandom,
                              SessionId:                      ChargingSession_Id.   NewRandom,
                              SessionTime:                    new StartEndDateTime(
                                                                  startTime,
                                                                  startTime + TimeSpan.FromHours(2)
                                                              ),
                              //Duration                      // automagic!

                              EVSE:                           evse1!,
                              //EVSEId                        // automagic!
                              //ChargingStation               // automagic!
                              //ChargingStationId             // automagic!
                              //ChargingPool                  // automagic!
                              //ChargingPoolId                // automagic!
                              //ChargingStationOperator       // automagic!
                              //ChargingStationOperatorId     // automagic!

                              ChargingProduct:                new ChargingProduct(
                                                                  ChargingProduct_Id.AC3
                                                              ),
                              ChargingPrice:                  Price.EURO(
                                                                  21.34M,
                                                                   1.00M
                                                              ),

                              AuthenticationStart:            LocalAuthentication.FromAuthToken(AuthenticationToken.NewRandom7Bytes),
                              //AuthenticationStop
                              AuthMethodStart:                AuthMethod.AUTH_REQUEST,
                              //AuthMethodStop
                              ProviderIdStart:                EMobilityProvider_Id.Parse("DE-GDF"),
                              //ProviderIdStop

                              //EMPRoamingProvider
                              EMPRoamingProviderId:           EMPRoamingProvider_Id.Parse("Hubject"),

                              //Reservation
                              //ReservationId
                              //ReservationTime
                              //ReservationFee

                              //ParkingSpaceId
                              //ParkingTime
                              //ParkingFee

                              //EnergyMeterId:                // automagic!
                              EnergyMeter:                    new WWCP.EnergyMeter(
                                                                  EnergyMeter_Id.Parse("12345678")
                                                              ),
                              EnergyMeteringValues:           new EnergyMeteringValue[] {
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(1),    5),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(31),  10),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(61),  15),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(91),  20),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(119), 22),
                                                              }
                              //ConsumedEnergy                // automagic!
                              //ConsumedEnergyFee

                              //CustomData
                              //InternalData

                              //PublicKey
                              //Signatures

                          );


            Assert.IsTrue   (wwcpCDR.Duration.         HasValue);
            Assert.AreEqual (2.0, wwcpCDR.Duration!.Value.TotalHours);

            Assert.IsNotNull(wwcpCDR.EVSE);
            Assert.IsTrue   (wwcpCDR.EVSEId.           HasValue);
            Assert.IsNotNull(wwcpCDR.ChargingStation);
            Assert.IsTrue   (wwcpCDR.ChargingStationId.HasValue);
            Assert.IsNotNull(wwcpCDR.ChargingPool);
            Assert.IsTrue   (wwcpCDR.ChargingPoolId.   HasValue);

            Assert.IsTrue   (wwcpCDR.ConsumedEnergy.   HasValue);
            Assert.AreEqual (17, wwcpCDR.ConsumedEnergy!.Value);

            var ocpiCDR = wwcpCDR.ToOCPI(null, null, out var warnings);
            Assert.AreEqual (0, warnings.Count());

            Assert.IsNotNull(ocpiCDR);
            Assert.AreEqual ("DE",                                       ocpiCDR!.CountryCode.ToString());
            Assert.AreEqual ("GEF",                                      ocpiCDR!.PartyId.    ToString());
            Assert.AreEqual (wwcpCDR.Id.ToString(),                      ocpiCDR!.Id.         ToString());
            Assert.AreEqual (wwcpCDR.SessionTime.StartTime,              ocpiCDR!.Start);
            Assert.AreEqual (wwcpCDR.SessionTime.EndTime!.Value,         ocpiCDR!.End);
            //AuthId
            Assert.AreEqual (wwcpCDR.AuthMethodStart.ToOCPI(),           ocpiCDR!.AuthMethod);

            Assert.IsNotNull(ocpiCDR.Location);
            Assert.IsNotNull(ocpiCDR.Location.EVSEs);
            Assert.AreEqual (1,                                          ocpiCDR!.Location.EVSEs.Count());
            Assert.AreEqual (wwcpCDR.EVSEId!.Value.ToString(),           ocpiCDR!.Location.EVSEs.First().EVSEId!.Value.ToString());

            Assert.AreEqual (wwcpCDR.ChargingPrice?.Currency?.ISOCode,   ocpiCDR!.Currency.ToString());

            Assert.IsNotNull(ocpiCDR.ChargingPeriods);
            Assert.AreEqual (wwcpCDR.EnergyMeteringValues.Count(),       ocpiCDR!.ChargingPeriods.Count());

            var energyMeteringValue1  = wwcpCDR!.EnergyMeteringValues.ElementAt(0);
            var energyMeteringValue2  = wwcpCDR!.EnergyMeteringValues.ElementAt(1);
            var energyMeteringValue3  = wwcpCDR!.EnergyMeteringValues.ElementAt(2);
            var energyMeteringValue4  = wwcpCDR!.EnergyMeteringValues.ElementAt(3);
            var energyMeteringValue5  = wwcpCDR!.EnergyMeteringValues.ElementAt(4);

            Assert.IsNotNull(energyMeteringValue1);
            Assert.IsNotNull(energyMeteringValue2);
            Assert.IsNotNull(energyMeteringValue3);
            Assert.IsNotNull(energyMeteringValue4);
            Assert.IsNotNull(energyMeteringValue5);

            var chargingPeriod1       = ocpiCDR!.ChargingPeriods.ElementAt(0);
            var chargingPeriod2       = ocpiCDR!.ChargingPeriods.ElementAt(1);
            var chargingPeriod3       = ocpiCDR!.ChargingPeriods.ElementAt(2);
            var chargingPeriod4       = ocpiCDR!.ChargingPeriods.ElementAt(3);
            var chargingPeriod5       = ocpiCDR!.ChargingPeriods.ElementAt(4);

            Assert.IsNotNull(chargingPeriod1);
            Assert.IsNotNull(chargingPeriod2);
            Assert.IsNotNull(chargingPeriod3);
            Assert.IsNotNull(chargingPeriod4);
            Assert.IsNotNull(chargingPeriod5);

            Assert.AreEqual(energyMeteringValue1.Timestamp.ToIso8601(),   chargingPeriod1.StartTimestamp.ToIso8601());
            Assert.AreEqual(energyMeteringValue2.Timestamp.ToIso8601(),   chargingPeriod2.StartTimestamp.ToIso8601());
            Assert.AreEqual(energyMeteringValue3.Timestamp.ToIso8601(),   chargingPeriod3.StartTimestamp.ToIso8601());
            Assert.AreEqual(energyMeteringValue4.Timestamp.ToIso8601(),   chargingPeriod4.StartTimestamp.ToIso8601());
            Assert.AreEqual(energyMeteringValue5.Timestamp.ToIso8601(),   chargingPeriod5.StartTimestamp.ToIso8601());

            Assert.IsNotNull(chargingPeriod1.Dimensions);
            Assert.IsNotNull(chargingPeriod2.Dimensions);
            Assert.IsNotNull(chargingPeriod3.Dimensions);
            Assert.IsNotNull(chargingPeriod4.Dimensions);
            Assert.IsNotNull(chargingPeriod5.Dimensions);

            Assert.IsNotNull(chargingPeriod1.Dimensions.FirstOrDefault());
            Assert.IsNotNull(chargingPeriod2.Dimensions.FirstOrDefault());
            Assert.IsNotNull(chargingPeriod3.Dimensions.FirstOrDefault());
            Assert.IsNotNull(chargingPeriod4.Dimensions.FirstOrDefault());
            Assert.IsNotNull(chargingPeriod5.Dimensions.FirstOrDefault());

            Assert.AreEqual(CDRDimensionType.ENERGY,                      chargingPeriod1.Dimensions.First().Type);
            Assert.AreEqual(CDRDimensionType.ENERGY,                      chargingPeriod2.Dimensions.First().Type);
            Assert.AreEqual(CDRDimensionType.ENERGY,                      chargingPeriod3.Dimensions.First().Type);
            Assert.AreEqual(CDRDimensionType.ENERGY,                      chargingPeriod4.Dimensions.First().Type);
            Assert.AreEqual(CDRDimensionType.ENERGY,                      chargingPeriod5.Dimensions.First().Type);

            Assert.AreEqual(energyMeteringValue1.Value,                   chargingPeriod1.Dimensions.First().Volume);
            Assert.AreEqual(energyMeteringValue2.Value,                   chargingPeriod2.Dimensions.First().Volume);
            Assert.AreEqual(energyMeteringValue3.Value,                   chargingPeriod3.Dimensions.First().Volume);
            Assert.AreEqual(energyMeteringValue4.Value,                   chargingPeriod4.Dimensions.First().Volume);
            Assert.AreEqual(energyMeteringValue5.Value,                   chargingPeriod5.Dimensions.First().Volume);


            Assert.AreEqual (wwcpCDR.ChargingPrice!.Value,                ocpiCDR!.TotalCost);
            Assert.AreEqual (wwcpCDR.ConsumedEnergy!.Value,               ocpiCDR!.TotalEnergy);
            Assert.AreEqual (wwcpCDR.Duration.Value,                      ocpiCDR!.TotalTime);

            Assert.AreEqual (wwcpCDR.EnergyMeterId!.Value.ToString(),     ocpiCDR!.MeterId.ToString());

            //Tariffs
            //SignedData
            //TotalParkingTime
            //Remark

            //LastUpdated

        }

        #endregion

    }

}
