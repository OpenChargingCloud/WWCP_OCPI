﻿/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using NUnit.Framework.Legacy;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPI;

//using EnergyMeter                = cloud.charging.open.protocols.OCPI.EnergyMeter;
//using TransparencySoftwareStatus = cloud.charging.open.protocols.OCPI.TransparencySoftwareStatus;

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
                               OCPI.Currency.EUR,

                               new[] {
                                   ChargingPeriod.Create(
                                       DateTime.Parse("2020-04-12T18:21:49Z"),
                                       new[] {
                                           CDRDimension.Create(
                                               CDRDimensionType.ENERGY,
                                               1.33M
                                           )
                                       }
                                   ),
                                   ChargingPeriod.Create(
                                       DateTime.Parse("2020-04-12T18:21:50Z"),
                                       new[] {
                                           CDRDimension.Create(
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
                               new OCPI.EnergyMeter(
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
                                       new OCPI.TransparencySoftwareStatus(
                                           new OCPI.TransparencySoftware(
                                               "Chargy Transparency Software Desktop Application",
                                               "v1.00",
                                               OpenSourceLicense.AGPL3,
                                               "GraphDefined GmbH",
                                               URL.Parse("https://open.charging.cloud/logo.svg"),
                                               URL.Parse("https://open.charging.cloud/Chargy/howto"),
                                               URL.Parse("https://open.charging.cloud/Chargy"),
                                               URL.Parse("https://github.com/OpenChargingCloud/ChargyDesktopApp")
                                           ),
                                           OCPI.LegalStatus.GermanCalibrationLaw,
                                           "cert",
                                           "German PTB",
                                           NotBefore: DateTime.Parse("2019-04-01T00:00:00.000Z").ToUniversalTime(),
                                           NotAfter:  DateTime.Parse("2030-01-01T00:00:00.000Z").ToUniversalTime()
                                       ),
                                       new OCPI.TransparencySoftwareStatus(
                                           new OCPI.TransparencySoftware(
                                               "Chargy Transparency Software Mobile Application",
                                               "v1.00",
                                               OpenSourceLicense.AGPL3,
                                               "GraphDefined GmbH",
                                               URL.Parse("https://open.charging.cloud/logo.svg"),
                                               URL.Parse("https://open.charging.cloud/Chargy/howto"),
                                               URL.Parse("https://open.charging.cloud/Chargy"),
                                               URL.Parse("https://github.com/OpenChargingCloud/ChargyMobileApp")
                                           ),
                                           OCPI.LegalStatus.ForInformationOnly,
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
                                       OCPI.Currency.EUR,
                                       new[] {
                                           new TariffElement(
                                               new[] {
                                                   PriceComponent.ChargingTime(
                                                       2.00M,
                                                       TimeSpan.FromSeconds(300)
                                                   )
                                               },
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
                                                   new[] {
                                                       DayOfWeek.Monday,
                                                       DayOfWeek.Tuesday
                                                   }
                                               )
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

            //ClassicAssert.AreEqual("DE",                          JSON["country_code"]?.Value<String>());
            //ClassicAssert.AreEqual("GEF",                         JSON["party_id"]?.    Value<String>());
            ClassicAssert.AreEqual("CDR0001",                     JSON["id"]?.          Value<String>());


            ClassicAssert.IsTrue(CDR.TryParse(JSON,
                                       out var parsedCDR,
                                       out var errorResponse,
                                       CountryCode.Parse("DE"),
                                       Party_Id.   Parse("GEF")));

            ClassicAssert.IsNotNull(parsedCDR);
            ClassicAssert.IsNull   (errorResponse);

            ClassicAssert.AreEqual (cdr1.CountryCode,              parsedCDR!.CountryCode);
            ClassicAssert.AreEqual (cdr1.PartyId,                  parsedCDR!.PartyId);
            ClassicAssert.AreEqual (cdr1.Id,                       parsedCDR!.Id);

            ClassicAssert.AreEqual (cdr1.Start.ToIso8601(),        parsedCDR!.Start.ToIso8601());
            ClassicAssert.AreEqual (cdr1.Stop.  ToIso8601(),        parsedCDR!.Stop.  ToIso8601());
            ClassicAssert.AreEqual (cdr1.AuthId,                   parsedCDR!.AuthId);
            ClassicAssert.AreEqual (cdr1.AuthMethod,               parsedCDR!.AuthMethod);
            ClassicAssert.IsTrue   (cdr1.Location.Equals(parsedCDR!.Location));
            ClassicAssert.AreEqual (cdr1.Currency,                 parsedCDR!.Currency);
            ClassicAssert.AreEqual (cdr1.ChargingPeriods,          parsedCDR!.ChargingPeriods);
            ClassicAssert.AreEqual (cdr1.TotalCost,                parsedCDR!.TotalCost);
            ClassicAssert.AreEqual (cdr1.TotalEnergy,              parsedCDR!.TotalEnergy);
            ClassicAssert.AreEqual (cdr1.TotalTime,                parsedCDR!.TotalTime);

            ClassicAssert.AreEqual (cdr1.MeterId,                  parsedCDR!.MeterId);
            ClassicAssert.AreEqual (cdr1.EnergyMeter,              parsedCDR!.EnergyMeter);
            ClassicAssert.AreEqual (cdr1.TransparencySoftwares,    parsedCDR!.TransparencySoftwares);
            ClassicAssert.AreEqual (cdr1.Tariffs,                  parsedCDR!.Tariffs);
            ClassicAssert.AreEqual (cdr1.SignedData,               parsedCDR!.SignedData);
            ClassicAssert.AreEqual (cdr1.TotalParkingTime,         parsedCDR!.TotalParkingTime);
            ClassicAssert.AreEqual (cdr1.Remark,                   parsedCDR!.Remark);

            ClassicAssert.AreEqual (cdr1.LastUpdated.ToIso8601(),  parsedCDR!.LastUpdated.ToIso8601());

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
                           ""stop_date_time"": ""2015-06-29T23:37:32Z"",
                           ""auth_id"": ""012345678"",
                           ""auth_method"": ""WHITELIST"",
                           ""location"": {
                             ""id"": ""LOC1"",
                             ""type"": ""UNDERGROUND_GARAGE"",
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

            var result = CDR.TryParse(JObject.Parse(JSON),
                                      out var parsedCDR,
                                      out var errorResponse,
                                      CountryCode.Parse("DE"),
                                      Party_Id.   Parse("GEF"));

            ClassicAssert.IsTrue   (result, errorResponse);
            ClassicAssert.IsNotNull(parsedCDR);
            ClassicAssert.IsNull   (errorResponse);

            ClassicAssert.AreEqual (CDR_Id.Parse("12345"),                                 parsedCDR!.Id);
            //ClassicAssert.AreEqual(true,                                                  parsedCDR.Publish);
            //ClassicAssert.AreEqual(cdr1.Start.    ToIso8601(),                            parsedCDR.Start.    ToIso8601());
            //ClassicAssert.AreEqual(cdr1.End.Value.ToIso8601(),                            parsedCDR.End.Value.ToIso8601());
            //ClassicAssert.AreEqual(cdr1.kWh,                                              parsedCDR.kWh);
            //ClassicAssert.AreEqual(cdr1.CDRToken,                                         parsedCDR.CDRToken);
            //ClassicAssert.AreEqual(cdr1.AuthMethod,                                       parsedCDR.AuthMethod);
            //ClassicAssert.AreEqual(cdr1.AuthorizationReference,                           parsedCDR.AuthorizationReference);
            //ClassicAssert.AreEqual(cdr1.CDRId,                                            parsedCDR.CDRId);
            //ClassicAssert.AreEqual(cdr1.EVSEUId,                                          parsedCDR.EVSEUId);
            //ClassicAssert.AreEqual(cdr1.ConnectorId,                                      parsedCDR.ConnectorId);
            //ClassicAssert.AreEqual(cdr1.MeterId,                                          parsedCDR.MeterId);
            //ClassicAssert.AreEqual(cdr1.EnergyMeter,                                      parsedCDR.EnergyMeter);
            //ClassicAssert.AreEqual(cdr1.TransparencySoftwares,                            parsedCDR.TransparencySoftwares);
            //ClassicAssert.AreEqual(cdr1.Currency,                                         parsedCDR.Currency);
            //ClassicAssert.AreEqual(cdr1.ChargingPeriods,                                  parsedCDR.ChargingPeriods);
            //ClassicAssert.AreEqual(cdr1.TotalCosts,                                       parsedCDR.TotalCosts);
            //ClassicAssert.AreEqual(cdr1.Status,                                           parsedCDR.Status);
            //ClassicAssert.AreEqual(cdr1.LastUpdated.ToIso8601(),                          parsedCDR.LastUpdated.ToIso8601());

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
                          I18NString.Create("GraphDefined CSO")
                      );

            #endregion

            #region Add DE*GEF*POOL1

            var addChargingPoolResult1 = await cso.AddChargingPool(

                                             Id:                   ChargingPool_Id.Parse("DE*GEF*POOL1"),
                                             Name:                 I18NString.Create("Test pool #1"),
                                             Description:          I18NString.Create("GraphDefined charging pool for tests #1"),

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

            ClassicAssert.IsNotNull(addChargingPoolResult1);

            var chargingPool1 = addChargingPoolResult1.ChargingPool;
            ClassicAssert.IsNotNull(chargingPool1);

            #endregion

            #region Add DE*GEF*STATION*1*A

            var addChargingStationResult1 = await chargingPool1!.AddChargingStation(

                                                Id:                   ChargingStation_Id.Parse("DE*GEF*STATION*1*A"),
                                                Name:                 I18NString.Create("Test station #1A"),
                                                Description:          I18NString.Create("GraphDefined charging station for tests #1A"),

                                                GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                InitialStatus:        ChargingStationStatusTypes.Available

                                            );

            ClassicAssert.IsNotNull(addChargingStationResult1);

            var chargingStation1  = addChargingStationResult1.ChargingStation;
            ClassicAssert.IsNotNull(chargingStation1);

            #endregion

            #region Add EVSE DE*GEF*EVSE*1*A*1

            var addEVSE1Result1 = await chargingStation1!.AddEVSE(

                                      Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*1"),
                                      Name:                 I18NString.Create("Test EVSE #1A1"),
                                      Description:          I18NString.Create("GraphDefined EVSE for tests #1A1"),

                                      InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                      InitialStatus:        EVSEStatusTypes.Available,

                                      ChargingConnectors:   new[] {
                                                                new ChargingConnector(
                                                                    Id:              ChargingConnector_Id.Parse("1"),
                                                                    Plug:            ChargingPlugTypes.Type2Outlet,
                                                                    Lockable:        true,
                                                                    CableAttached:   true,
                                                                    CableLength:     Meter.Parse(4)
                                                                )
                                                            }

                                  );

            ClassicAssert.IsNotNull(addEVSE1Result1);

            var evse1     = addEVSE1Result1.EVSE;
            ClassicAssert.IsNotNull(evse1);

            #endregion

            #region Add EVSE DE*GEF*EVSE*1*A*2

            var addEVSE1Result2 = await chargingStation1!.AddEVSE(

                                      Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*2"),
                                      Name:                 I18NString.Create("Test EVSE #1A2"),
                                      Description:          I18NString.Create("GraphDefined EVSE for tests #1A2"),

                                      InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                      InitialStatus:        EVSEStatusTypes.Available,

                                      ChargingConnectors:   new[] {
                                                                new ChargingConnector(
                                                                    Id:              ChargingConnector_Id.Parse("2"),
                                                                    Plug:            ChargingPlugTypes.TypeFSchuko,
                                                                    Lockable:        false,
                                                                    CableAttached:   false
                                                                )
                                                            }

                                  );

            ClassicAssert.IsNotNull(addEVSE1Result2);

            var evse2     = addEVSE1Result2.EVSE;
            ClassicAssert.IsNotNull(evse2);

            #endregion


            var startTime = Timestamp.Now - TimeSpan.FromHours(3);


            var wwcpCDR = new ChargeDetailRecord(

                              Id:                             ChargeDetailRecord_Id.NewRandom(),
                              SessionId:                      ChargingSession_Id.   NewRandom(),
                              SessionTime:                    new StartEndDateTime(
                                                                  startTime,
                                                                  startTime + TimeSpan.FromHours(2)
                                                              ),
                              //Duration                      // automagic!

                              ChargingConnector:              evse1.ChargingConnectors.First(),
                              //EVSE:                         // automagic!
                              //EVSEId                        // automagic!
                              //ChargingStation               // automagic!
                              //ChargingStationId             // automagic!
                              //ChargingPool                  // automagic!
                              //ChargingPoolId                // automagic!
                              //ChargingStationOperator       // automagic!
                              //ChargingStationOperatorId     // automagic!

                              ChargingProduct:                ChargingProduct.AC3,
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
                              CSORoamingProviderIdStart:           CSORoamingProvider_Id.Parse("Hubject"),

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
                              EnergyMeteringValues:           [
                                                                  new EnergyMeteringValue(startTime,                              0, EnergyMeteringValueTypes.Start),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(1),    5, EnergyMeteringValueTypes.Intermediate),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(31),  10, EnergyMeteringValueTypes.Intermediate),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(61),  15, EnergyMeteringValueTypes.TariffChange),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(91),  20, EnergyMeteringValueTypes.Intermediate),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(119), 22, EnergyMeteringValueTypes.Intermediate),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(120), 23, EnergyMeteringValueTypes.Stop)
                                                              ]
                              //ConsumedEnergy                // automagic!
                              //ConsumedEnergyFee

                              //CustomData
                              //InternalData

                              //PublicKey
                              //Signatures

                          );


            ClassicAssert.IsTrue   (wwcpCDR.Duration.         HasValue);
            ClassicAssert.AreEqual (2.0, wwcpCDR.Duration!.Value.TotalHours);

            ClassicAssert.IsNotNull(wwcpCDR.EVSE);
            ClassicAssert.IsTrue   (wwcpCDR.EVSEId.           HasValue);
            ClassicAssert.IsNotNull(wwcpCDR.ChargingStation);
            ClassicAssert.IsTrue   (wwcpCDR.ChargingStationId.HasValue);
            ClassicAssert.IsNotNull(wwcpCDR.ChargingPool);
            ClassicAssert.IsTrue   (wwcpCDR.ChargingPoolId.   HasValue);

            ClassicAssert.IsTrue   (wwcpCDR.ConsumedEnergy.   HasValue);
            ClassicAssert.AreEqual (17, wwcpCDR.ConsumedEnergy!.Value);

            var ocpiCDR = wwcpCDR.ToOCPI(CustomEVSEUIdConverter:  null,
                                         CustomEVSEIdConverter:   null,
                                         GetTariffIdsDelegate:    null,
                                         EMSPId:                  null,
                                         TariffGetter:            null,
                                         out var warnings);
            ClassicAssert.AreEqual (0, warnings.Count(), 0, warnings.FirstOrDefault()?.Text.FirstText());

            ClassicAssert.IsNotNull(ocpiCDR);
            ClassicAssert.AreEqual ("DE",                                       ocpiCDR!.CountryCode.ToString());
            ClassicAssert.AreEqual ("GEF",                                      ocpiCDR!.PartyId.    ToString());
            ClassicAssert.AreEqual (wwcpCDR.Id.ToString(),                      ocpiCDR!.Id.         ToString());
            ClassicAssert.AreEqual (wwcpCDR.SessionTime.StartTime,              ocpiCDR!.Start);
            ClassicAssert.AreEqual (wwcpCDR.SessionTime.EndTime!.Value,         ocpiCDR!.Stop);
            //AuthId
            ClassicAssert.AreEqual (wwcpCDR.AuthMethodStart.ToOCPI(),           ocpiCDR!.AuthMethod);

            ClassicAssert.IsNotNull(ocpiCDR.Location);
            ClassicAssert.IsNotNull(ocpiCDR.Location.EVSEs);
            ClassicAssert.AreEqual (1,                                          ocpiCDR!.Location.EVSEs.Count());
            ClassicAssert.AreEqual (wwcpCDR.EVSEId!.Value.ToString(),           ocpiCDR!.Location.EVSEs.First().EVSEId!.Value.ToString());

            ClassicAssert.AreEqual (wwcpCDR.ChargingPrice?.Currency?.ISOCode,   ocpiCDR!.Currency.ToString());

            ClassicAssert.IsNotNull(ocpiCDR.ChargingPeriods);
            ClassicAssert.AreEqual (wwcpCDR.EnergyMeteringValues.Count(),       ocpiCDR!.ChargingPeriods.Count());

            var energyMeteringValue1  = wwcpCDR!.EnergyMeteringValues.ElementAt(0);
            var energyMeteringValue2  = wwcpCDR!.EnergyMeteringValues.ElementAt(1);
            var energyMeteringValue3  = wwcpCDR!.EnergyMeteringValues.ElementAt(2);
            var energyMeteringValue4  = wwcpCDR!.EnergyMeteringValues.ElementAt(3);
            var energyMeteringValue5  = wwcpCDR!.EnergyMeteringValues.ElementAt(4);

            ClassicAssert.IsNotNull(energyMeteringValue1);
            ClassicAssert.IsNotNull(energyMeteringValue2);
            ClassicAssert.IsNotNull(energyMeteringValue3);
            ClassicAssert.IsNotNull(energyMeteringValue4);
            ClassicAssert.IsNotNull(energyMeteringValue5);

            var chargingPeriod1       = ocpiCDR!.ChargingPeriods.ElementAt(0);
            var chargingPeriod2       = ocpiCDR!.ChargingPeriods.ElementAt(1);
            var chargingPeriod3       = ocpiCDR!.ChargingPeriods.ElementAt(2);
            var chargingPeriod4       = ocpiCDR!.ChargingPeriods.ElementAt(3);
            var chargingPeriod5       = ocpiCDR!.ChargingPeriods.ElementAt(4);

            ClassicAssert.IsNotNull(chargingPeriod1);
            ClassicAssert.IsNotNull(chargingPeriod2);
            ClassicAssert.IsNotNull(chargingPeriod3);
            ClassicAssert.IsNotNull(chargingPeriod4);
            ClassicAssert.IsNotNull(chargingPeriod5);

            ClassicAssert.AreEqual (energyMeteringValue1.Timestamp.ToIso8601(),                 chargingPeriod1.StartTimestamp.ToIso8601());
            ClassicAssert.AreEqual (energyMeteringValue2.Timestamp.ToIso8601(),                 chargingPeriod2.StartTimestamp.ToIso8601());
            ClassicAssert.AreEqual (energyMeteringValue3.Timestamp.ToIso8601(),                 chargingPeriod3.StartTimestamp.ToIso8601());
            ClassicAssert.AreEqual (energyMeteringValue4.Timestamp.ToIso8601(),                 chargingPeriod4.StartTimestamp.ToIso8601());
            ClassicAssert.AreEqual (energyMeteringValue5.Timestamp.ToIso8601(),                 chargingPeriod5.StartTimestamp.ToIso8601());

            ClassicAssert.IsNotNull(chargingPeriod1.Dimensions);
            ClassicAssert.IsNotNull(chargingPeriod2.Dimensions);
            ClassicAssert.IsNotNull(chargingPeriod3.Dimensions);
            ClassicAssert.IsNotNull(chargingPeriod4.Dimensions);
            ClassicAssert.IsNotNull(chargingPeriod5.Dimensions);

            ClassicAssert.IsNotNull(chargingPeriod1.Dimensions.FirstOrDefault());
            ClassicAssert.IsNotNull(chargingPeriod2.Dimensions.FirstOrDefault());
            ClassicAssert.IsNotNull(chargingPeriod3.Dimensions.FirstOrDefault());
            ClassicAssert.IsNotNull(chargingPeriod4.Dimensions.FirstOrDefault());
            ClassicAssert.IsNotNull(chargingPeriod5.Dimensions.FirstOrDefault());

            ClassicAssert.AreEqual (CDRDimensionType.ENERGY,                                    chargingPeriod1.Dimensions.First().Type);
            ClassicAssert.AreEqual (CDRDimensionType.ENERGY,                                    chargingPeriod2.Dimensions.First().Type);
            ClassicAssert.AreEqual (CDRDimensionType.ENERGY,                                    chargingPeriod3.Dimensions.First().Type);
            ClassicAssert.AreEqual (CDRDimensionType.ENERGY,                                    chargingPeriod4.Dimensions.First().Type);
            ClassicAssert.AreEqual (CDRDimensionType.ENERGY,                                    chargingPeriod5.Dimensions.First().Type);

            ClassicAssert.AreEqual (energyMeteringValue1.Value,                                 chargingPeriod1.Dimensions.First().Volume);
            ClassicAssert.AreEqual (energyMeteringValue2.Value,                                 chargingPeriod2.Dimensions.First().Volume);
            ClassicAssert.AreEqual (energyMeteringValue3.Value,                                 chargingPeriod3.Dimensions.First().Volume);
            ClassicAssert.AreEqual (energyMeteringValue4.Value,                                 chargingPeriod4.Dimensions.First().Volume);
            ClassicAssert.AreEqual (energyMeteringValue5.Value,                                 chargingPeriod5.Dimensions.First().Volume);

            ClassicAssert.AreEqual (wwcpCDR.ChargingPrice?.Base + wwcpCDR.ChargingPrice?.VAT,   ocpiCDR!.TotalCost);
            ClassicAssert.AreEqual (wwcpCDR.ConsumedEnergy!.Value,                              ocpiCDR!.TotalEnergy);
            ClassicAssert.AreEqual (wwcpCDR.Duration.Value,                                     ocpiCDR!.TotalTime);

            ClassicAssert.AreEqual (wwcpCDR.EnergyMeterId!.Value.ToString(),                    ocpiCDR!.MeterId.ToString());

            //Tariffs
            //SignedData
            //TotalParkingTime
            //Remark

            //LastUpdated

        }

        #endregion


        #region AugmentCDRWithTariffinformation_Test01()

        /// <summary>
        /// Augment an OCPI charge detail record with charging tariff information.
        /// </summary>
        [Test]
        public static async Task AugmentCDRWithTariffinformation_Test01()
        {

            var now        = Timestamp.Now;
            var startTime  = now - TimeSpan.FromHours(3);
            var stopTime   = now;

            var cdrIn      = new CDR(
                                 CountryCode:         CountryCode.Parse("DE"),
                                 PartyId:             Party_Id.   Parse("GEF"),
                                 Id:                  CDR_Id.     NewRandom(),
                                 Start:               startTime,
                                 Stop:                stopTime,
                                 AuthId:              Auth_Id.    NewRandom(),
                                 AuthMethod:          AuthMethods.AUTH_REQUEST,
                                 Location:            new Location(
                                                          CountryCode:    CountryCode.Parse("DE"),
                                                          PartyId:        Party_Id.   Parse("GEF"),
                                                          Id:             Location_Id.NewRandom(),
                                                          LocationType:   LocationType.ON_STREET,
                                                          Address:        "Biberweg 18",
                                                          City:           "Jena",
                                                          PostalCode:     "07749",
                                                          Country:        Country.Germany,
                                                          Coordinates:    GeoCoordinate.FromLatLng(50.928365, 11.589986)
                                                      ),
                                 Currency:            OCPI.Currency.EUR,
                                 ChargingPeriods:     Array.Empty<ChargingPeriod>(),
                                 TotalCost:           0,
                                 TotalEnergy:         0,
                                 TotalTime:           TimeSpan.Zero
                             );

            var tariff     = new Tariff(
                                 CountryCode:         CountryCode.Parse("DE"),
                                 PartyId:             Party_Id.   Parse("GEF"),
                                 Id:                  Tariff_Id.  NewRandom(),
                                 Currency:            OCPI.Currency.EUR,
                                 TariffElements:      new[] {
                                                          new TariffElement(
                                                              PriceComponent.Energy(
                                                                  Price:     0.5m,
                                                                  StepSize:  1
                                                              ),
                                                              PriceComponent.ChargingTime(
                                                                  Price:     0.25m,
                                                                  Duration:  TimeSpan.FromMinutes(1)
                                                              )
                                                          )
                                                  } 
                             );


            var cdrOut     = cdrIn.AugemntCDRWithTariff(tariff);


        }

        #endregion


    }

}
