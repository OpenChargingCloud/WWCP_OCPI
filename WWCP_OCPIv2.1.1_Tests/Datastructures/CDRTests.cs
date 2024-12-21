/*
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
using System.Linq;

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

                               [
                                   new ChargingPeriod(
                                       DateTime.Parse("2020-04-12T18:21:49Z"),
                                       [
                                           CDRDimension.Create(
                                               CDRDimensionType.ENERGY,
                                               1.33M
                                           )
                                       ]
                                   ),
                                   new ChargingPeriod(
                                       DateTime.Parse("2020-04-12T18:21:50Z"),
                                       [
                                           CDRDimension.Create(
                                               CDRDimensionType.TIME,
                                               5.12M
                                           )
                                       ]
                                   )
                               ],

                               // Total cost
                               10.00M,

                               // Total Energy
                               WattHour.              ParseKWh(50.00M),

                               // Total time
                               TimeSpan.              FromMinutes(30),

                               null,    // Costs
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
                                   [
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
                                   ]
                               ),
                               null,

                               [
                                   new Tariff(
                                       CountryCode.Parse("DE"),
                                       Party_Id.   Parse("GEF"),
                                       Tariff_Id.  Parse("TARIFF0001"),
                                       OCPI.Currency.EUR,
                                       [
                                           new TariffElement(
                                               [
                                                   PriceComponent.ChargingTime(
                                                       2.00M,
                                                       TimeSpan.FromSeconds(300)
                                                   )
                                               ],
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
                                                   [
                                                       DayOfWeek.Monday,
                                                       DayOfWeek.Tuesday
                                                   ]
                                               )
                                           )
                                       ],
                                       [
                                           new DisplayText(Languages.de, "Hallo Welt!"),
                                           new DisplayText(Languages.en, "Hello world!"),
                                       ],
                                       URL.Parse("https://open.charging.cloud"),
                                       new EnergyMix(
                                           true,
                                           [
                                               new EnergySource(
                                                   EnergySourceCategory.SOLAR,
                                                   80
                                               ),
                                               new EnergySource(
                                                   EnergySourceCategory.WIND,
                                                   20
                                               )
                                           ],
                                           [
                                               new EnvironmentalImpact(
                                                   EnvironmentalImpactCategory.CARBON_DIOXIDE,
                                                   0.1
                                               )
                                           ],
                                           "Stadtwerke Jena-Ost",
                                           "New Green Deal"
                                       ),
                                       DateTime.Parse("2020-09-22").ToUniversalTime()
                                   )
                               ],

                               new SignedData(
                                   EncodingMethod.GraphDefined,
                                   [
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
                                   ],
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

            var jsonCDR = cdr1.ToJSON();

            //ClassicAssert.AreEqual("DE",                          JSON["country_code"]?.Value<String>());
            //ClassicAssert.AreEqual("GEF",                         JSON["party_id"]?.    Value<String>());
            ClassicAssert.AreEqual("CDR0001",                     jsonCDR["id"]?.          Value<String>());


            if (CDR.TryParse(jsonCDR,
                             out var parsedCDR,
                             out var errorResponse,
                             CountryCode.Parse("DE"),
                             Party_Id.Parse("GEF"))) {

                ClassicAssert.IsNotNull(parsedCDR);
                ClassicAssert.IsNull   (errorResponse);

                ClassicAssert.AreEqual (cdr1.CountryCode,              parsedCDR.CountryCode);
                ClassicAssert.AreEqual (cdr1.PartyId,                  parsedCDR.PartyId);
                ClassicAssert.AreEqual (cdr1.Id,                       parsedCDR.Id);

                ClassicAssert.AreEqual (cdr1.Start.ToIso8601(),        parsedCDR.Start.ToIso8601());
                ClassicAssert.AreEqual (cdr1.Stop. ToIso8601(),        parsedCDR.Stop.  ToIso8601());
                ClassicAssert.AreEqual (cdr1.AuthId,                   parsedCDR.AuthId);
                ClassicAssert.AreEqual (cdr1.AuthMethod,               parsedCDR.AuthMethod);
                ClassicAssert.IsTrue   (cdr1.Location.Equals(parsedCDR.Location));
                ClassicAssert.AreEqual (cdr1.Currency,                 parsedCDR.Currency);
                ClassicAssert.AreEqual (cdr1.ChargingPeriods,          parsedCDR.ChargingPeriods);
                ClassicAssert.AreEqual (cdr1.TotalCost,                parsedCDR.TotalCost);
                ClassicAssert.AreEqual (cdr1.TotalEnergy,              parsedCDR.TotalEnergy);
                ClassicAssert.AreEqual (cdr1.TotalTime,                parsedCDR.TotalTime);

                ClassicAssert.AreEqual (cdr1.MeterId,                  parsedCDR.MeterId);
                ClassicAssert.AreEqual (cdr1.EnergyMeter,              parsedCDR.EnergyMeter);
                ClassicAssert.AreEqual (cdr1.TransparencySoftwares,    parsedCDR.TransparencySoftwares);
                ClassicAssert.AreEqual (cdr1.Tariffs,                  parsedCDR.Tariffs);
                ClassicAssert.AreEqual (cdr1.SignedData,               parsedCDR.SignedData);
                ClassicAssert.AreEqual (cdr1.TotalParkingTime,         parsedCDR.TotalParkingTime);
                ClassicAssert.AreEqual (cdr1.Remark,                   parsedCDR.Remark);

                ClassicAssert.AreEqual (cdr1.LastUpdated.ToIso8601(),  parsedCDR.LastUpdated.ToIso8601());

            }

            else
                Assert.Fail(errorResponse);

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

            ClassicAssert.IsTrue   (result, errorResponse!);
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

                                             InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                             InitialStatus:        ChargingPoolStatusTypes.Available

                                         );

            ClassicAssert.IsNotNull(addChargingPoolResult1);

            var chargingPool1 = addChargingPoolResult1.ChargingPool;
            ClassicAssert.IsNotNull(chargingPool1);

            #endregion

            #region Add DE*GEF*STATION*1*A

            var addChargingStationResult1 = await chargingPool1!.AddChargingStation(

                                                Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*1*A"),
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
                                      InitialStatus:        EVSEStatusType.Available,

                                      ChargingConnectors:   new[] {
                                                                new ChargingConnector(
                                                                    Id:              ChargingConnector_Id.Parse("1"),
                                                                    Plug:            ChargingPlugTypes.Type2Outlet,
                                                                    Lockable:        true,
                                                                    CableAttached:   true,
                                                                    CableLength:     Meter.ParseM(4)
                                                                )
                                                            }

                                  );

            ClassicAssert.IsNotNull(addEVSE1Result1);

            var evse1     = addEVSE1Result1.EVSE!;
            ClassicAssert.IsNotNull(evse1);

            #endregion

            #region Add EVSE DE*GEF*EVSE*1*A*2

            var addEVSE1Result2 = await chargingStation1!.AddEVSE(

                                      Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*2"),
                                      Name:                 I18NString.Create("Test EVSE #1A2"),
                                      Description:          I18NString.Create("GraphDefined EVSE for tests #1A2"),

                                      InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                      InitialStatus:        EVSEStatusType.Available,

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
                                                                  20.00M,
                                                                   0.50M
                                                              ),

                              AuthenticationStart:            LocalAuthentication.FromAuthToken(AuthenticationToken.NewRandom7Bytes),
                              //AuthenticationStop
                              AuthMethodStart:                AuthMethod.AUTH_REQUEST,
                              //AuthMethodStop
                              ProviderIdStart:                EMobilityProvider_Id. Parse("DE-GDF"),
                              //ProviderIdStop

                              //EMPRoamingProvider
                              CSORoamingProviderIdStart:      CSORoamingProvider_Id.Parse("Hubject"),

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
                                                                  new EnergyMeteringValue(startTime,                             WattHour.ParseKWh( 0), EnergyMeteringValueTypes.Start),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(1),   WattHour.ParseKWh( 5), EnergyMeteringValueTypes.Intermediate),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(31),  WattHour.ParseKWh(10), EnergyMeteringValueTypes.Intermediate),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(61),  WattHour.ParseKWh(15), EnergyMeteringValueTypes.TariffChange),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(91),  WattHour.ParseKWh(20), EnergyMeteringValueTypes.Intermediate),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(119), WattHour.ParseKWh(22), EnergyMeteringValueTypes.Intermediate),
                                                                  new EnergyMeteringValue(startTime + TimeSpan.FromMinutes(120), WattHour.ParseKWh(23), EnergyMeteringValueTypes.Stop)
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
            ClassicAssert.AreEqual (23, wwcpCDR.ConsumedEnergy!.Value.kWh);

            var ocpiCDR = wwcpCDR.ToOCPI(CustomEVSEUIdConverter:   null,
                                         CustomEVSEIdConverter:    null,
                                         GetTariffIdsDelegate:     (cpoCountryCode,
                                                                    cpoPartyId,
                                                                    locationId,
                                                                    evseUId,
                                                                    connectorId,
                                                                    emspId) => [ Tariff_Id.Parse("ct1") ],
                                         EMSPId:                   null,
                                         TariffGetter:             (tariffId, startTimestamp, c) => new Tariff(
                                                                                                        CountryCode:     CountryCode.Parse("DE"),
                                                                                                        PartyId:         Party_Id.   Parse("GEF"),
                                                                                                        Id:              Tariff_Id.  Parse("ct1"),
                                                                                                        Currency:        OCPI.Currency.EUR,
                                                                                                        TariffElements:  [
                                                                                                                             new TariffElement(
                                                                                                                                 PriceComponent.Energy(
                                                                                                                                     Price:     0.44m,
                                                                                                                                     StepSize:  1000
                                                                                                                                 ),
                                                                                                                                 PriceComponent.ChargingTime(
                                                                                                                                     Price:     5.04m,
                                                                                                                                     Duration:  TimeSpan.FromMinutes(1)
                                                                                                                                 ),
                                                                                                                                 PriceComponent.FlatRate(
                                                                                                                                     Price:     0.3m
                                                                                                                                 )
                                                                                                                             )
                                                                                                                         ]
                                                                                                    ),
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
            ClassicAssert.AreEqual (wwcpCDR.EnergyMeteringValues.Count() - 1,   ocpiCDR!.ChargingPeriods.Count());

            var energyMeteringValue1  = wwcpCDR!.EnergyMeteringValues.ElementAt(0);
            var energyMeteringValue2  = wwcpCDR!.EnergyMeteringValues.ElementAt(1);
            var energyMeteringValue3  = wwcpCDR!.EnergyMeteringValues.ElementAt(2);
            var energyMeteringValue4  = wwcpCDR!.EnergyMeteringValues.ElementAt(3);
            var energyMeteringValue5  = wwcpCDR!.EnergyMeteringValues.ElementAt(4);
            var energyMeteringValue6  = wwcpCDR!.EnergyMeteringValues.ElementAt(5);
            var energyMeteringValue7  = wwcpCDR!.EnergyMeteringValues.ElementAt(6);

            ClassicAssert.IsNotNull(energyMeteringValue1);
            ClassicAssert.IsNotNull(energyMeteringValue2);
            ClassicAssert.IsNotNull(energyMeteringValue3);
            ClassicAssert.IsNotNull(energyMeteringValue4);
            ClassicAssert.IsNotNull(energyMeteringValue5);
            ClassicAssert.IsNotNull(energyMeteringValue6);
            ClassicAssert.IsNotNull(energyMeteringValue7);

            var chargingPeriod1       = ocpiCDR!.ChargingPeriods.ElementAt(0);
            var chargingPeriod2       = ocpiCDR!.ChargingPeriods.ElementAt(1);
            var chargingPeriod3       = ocpiCDR!.ChargingPeriods.ElementAt(2);
            var chargingPeriod4       = ocpiCDR!.ChargingPeriods.ElementAt(3);
            var chargingPeriod5       = ocpiCDR!.ChargingPeriods.ElementAt(4);
            var chargingPeriod6       = ocpiCDR!.ChargingPeriods.ElementAt(5);

            ClassicAssert.IsNotNull(chargingPeriod1);
            ClassicAssert.IsNotNull(chargingPeriod2);
            ClassicAssert.IsNotNull(chargingPeriod3);
            ClassicAssert.IsNotNull(chargingPeriod4);
            ClassicAssert.IsNotNull(chargingPeriod5);
            ClassicAssert.IsNotNull(chargingPeriod6);

            ClassicAssert.AreEqual (energyMeteringValue1.Timestamp.ToIso8601(),                                chargingPeriod1.StartTimestamp.      ToIso8601());
            ClassicAssert.AreEqual (energyMeteringValue2.Timestamp.ToIso8601(),                                chargingPeriod2.StartTimestamp.      ToIso8601());
            ClassicAssert.AreEqual (energyMeteringValue3.Timestamp.ToIso8601(),                                chargingPeriod3.StartTimestamp.      ToIso8601());
            ClassicAssert.AreEqual (energyMeteringValue4.Timestamp.ToIso8601(),                                chargingPeriod4.StartTimestamp.      ToIso8601());
            ClassicAssert.AreEqual (energyMeteringValue5.Timestamp.ToIso8601(),                                chargingPeriod5.StartTimestamp.      ToIso8601());
            ClassicAssert.AreEqual (energyMeteringValue6.Timestamp.ToIso8601(),                                chargingPeriod6.StartTimestamp.      ToIso8601());
            ClassicAssert.AreEqual (energyMeteringValue7.Timestamp.ToIso8601(),                                chargingPeriod6.StopTimestamp!.Value.ToIso8601());

            ClassicAssert.IsNotNull(chargingPeriod1.Dimensions);
            ClassicAssert.IsNotNull(chargingPeriod2.Dimensions);
            ClassicAssert.IsNotNull(chargingPeriod3.Dimensions);
            ClassicAssert.IsNotNull(chargingPeriod4.Dimensions);
            ClassicAssert.IsNotNull(chargingPeriod5.Dimensions);
            ClassicAssert.IsNotNull(chargingPeriod6.Dimensions);

            ClassicAssert.IsNotNull(chargingPeriod1.Dimensions.FirstOrDefault());
            ClassicAssert.IsNotNull(chargingPeriod2.Dimensions.FirstOrDefault());
            ClassicAssert.IsNotNull(chargingPeriod3.Dimensions.FirstOrDefault());
            ClassicAssert.IsNotNull(chargingPeriod4.Dimensions.FirstOrDefault());
            ClassicAssert.IsNotNull(chargingPeriod5.Dimensions.FirstOrDefault());
            ClassicAssert.IsNotNull(chargingPeriod6.Dimensions.FirstOrDefault());

            ClassicAssert.AreEqual (CDRDimensionType.ENERGY,                                                   chargingPeriod1.Dimensions.First().Type);
            ClassicAssert.AreEqual (CDRDimensionType.ENERGY,                                                   chargingPeriod2.Dimensions.First().Type);
            ClassicAssert.AreEqual (CDRDimensionType.ENERGY,                                                   chargingPeriod3.Dimensions.First().Type);
            ClassicAssert.AreEqual (CDRDimensionType.ENERGY,                                                   chargingPeriod4.Dimensions.First().Type);
            ClassicAssert.AreEqual (CDRDimensionType.ENERGY,                                                   chargingPeriod5.Dimensions.First().Type);
            ClassicAssert.AreEqual (CDRDimensionType.ENERGY,                                                   chargingPeriod6.Dimensions.First().Type);

            ClassicAssert.AreEqual (energyMeteringValue2.WattHours.kWh,                                        chargingPeriod1.Dimensions.First().Volume);
            ClassicAssert.AreEqual (energyMeteringValue3.WattHours.kWh - energyMeteringValue2.WattHours.kWh,   chargingPeriod2.Dimensions.First().Volume);
            ClassicAssert.AreEqual (energyMeteringValue4.WattHours.kWh - energyMeteringValue3.WattHours.kWh,   chargingPeriod3.Dimensions.First().Volume);
            ClassicAssert.AreEqual (energyMeteringValue5.WattHours.kWh - energyMeteringValue4.WattHours.kWh,   chargingPeriod4.Dimensions.First().Volume);
            ClassicAssert.AreEqual (energyMeteringValue6.WattHours.kWh - energyMeteringValue5.WattHours.kWh,   chargingPeriod5.Dimensions.First().Volume);
            ClassicAssert.AreEqual (energyMeteringValue7.WattHours.kWh - energyMeteringValue6.WattHours.kWh,   chargingPeriod6.Dimensions.First().Volume);

            ClassicAssert.AreEqual (wwcpCDR.ChargingPrice?.Base + wwcpCDR.ChargingPrice?.VAT,                  ocpiCDR!.TotalCost);
            ClassicAssert.AreEqual (wwcpCDR.ConsumedEnergy!.Value,                                             ocpiCDR!.TotalEnergy);
            ClassicAssert.AreEqual (wwcpCDR.Duration.Value,                                                    ocpiCDR!.TotalTime);

            ClassicAssert.AreEqual (wwcpCDR.EnergyMeterId!.Value.ToString(),                                   ocpiCDR!.MeterId.ToString());

            //Tariffs
            //SignedData
            //TotalParkingTime
            //Remark

            //LastUpdated

        }

        #endregion


        #region SplittCDRIntoChargingPeriods_Test01()

        /// <summary>
        /// Augment an OCPI charge detail record with charging tariff information.
        /// </summary>
        [Test]
        public static void SplittCDRIntoChargingPeriods_Test01()
        {

            var now = Timestamp.Now;

            #region CDR 1     (2 hours duration, 10 kWh!)

            var start1           = now    - TimeSpan.FromHours(3);
            var stop1            = start1 + TimeSpan.FromHours(2);

            var meteringValues1  = new[] {
                                       new Timestamped<WattHour>(start1, WattHour.ParseKWh( 1)),
                                       new Timestamped<WattHour>(stop1,  WattHour.ParseKWh(11))
                                   };

            var cdr1             = new CDR(
                                       CountryCode:         CountryCode.Parse("DE"),
                                       PartyId:             Party_Id.   Parse("GEF"),
                                       Id:                  CDR_Id.     Parse("cdr1"),
                                       Start:               start1,
                                       Stop:                stop1,
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
                                       ChargingPeriods:     [
                                                                new ChargingPeriod(
                                                                    start1,
                                                                    [ CDRDimension.ENERGY(1) ]
                                                                )
                                                            ],
                                       TotalCost:           0,
                                       TotalEnergy:         WattHour.Zero,
                                       TotalTime:           TimeSpan.Zero
                                   );

            #endregion

            #region CDR 2     (3 hours duration, 18 kWh!)   [EdgeCase for tariff 2!]

            var start2           = now    - TimeSpan.FromHours(6);
            var stop2            = start2 + TimeSpan.FromHours(3);

            var meteringValues2  = new[] {
                                       new Timestamped<WattHour>(start2, WattHour.ParseKWh( 2)),
                                       new Timestamped<WattHour>(stop2,  WattHour.ParseKWh(20))
                                   };

            var cdr2             = new CDR(
                                       CountryCode:         CountryCode.Parse("DE"),
                                       PartyId:             Party_Id.   Parse("GEF"),
                                       Id:                  CDR_Id.     Parse("cdr2"),
                                       Start:               start2,
                                       Stop:                stop2,
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
                                       ChargingPeriods:     [
                                                                new ChargingPeriod(
                                                                    start2,
                                                                    [ CDRDimension.ENERGY(2) ]
                                                                )
                                                            ],
                                       TotalCost:           0,
                                       TotalEnergy:         WattHour.Zero,
                                       TotalTime:           TimeSpan.Zero
                                   );

            #endregion

            #region CDR 3     (7 hours duration, 20 kWh!)

            var start3           = now    - TimeSpan.FromHours(9);
            var stop3            = start3 + TimeSpan.FromHours(7);

            var meteringValues3  = new[] {
                                       new Timestamped<WattHour>(start3, WattHour.ParseKWh( 2)),
                                       new Timestamped<WattHour>(stop3,  WattHour.ParseKWh(22))
                                   };

            var cdr3             = new CDR(
                                       CountryCode:         CountryCode.Parse("DE"),
                                       PartyId:             Party_Id.   Parse("GEF"),
                                       Id:                  CDR_Id.     Parse("cdr2"),
                                       Start:               start3,
                                       Stop:                stop3,
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
                                       ChargingPeriods:     [
                                                                new ChargingPeriod(
                                                                    start3,
                                                                    [ CDRDimension.ENERGY(2) ]
                                                                )
                                                            ],
                                       TotalCost:           0,
                                       TotalEnergy:         WattHour.Zero,
                                       TotalTime:           TimeSpan.Zero
                                   );

            #endregion


            #region Tariff 1  (one tariff element!)

            var tariff1          = new Tariff(
                                       CountryCode:         CountryCode.Parse("DE"),
                                       PartyId:             Party_Id.   Parse("GEF"),
                                       Id:                  Tariff_Id.  Parse("tariff1"),
                                       Currency:            OCPI.Currency.EUR,
                                       TariffElements:      [
                                                                new TariffElement(
                                                                    PriceComponent.Energy(
                                                                        Price:     0.44m,
                                                                        StepSize:  1000
                                                                    ),
                                                                    PriceComponent.ChargingTime(
                                                                        Price:     5.04m,
                                                                        Duration:  TimeSpan.FromMinutes(1)
                                                                    ),
                                                                    PriceComponent.FlatRate(
                                                                        Price:     0.3m
                                                                    )
                                                                )
                                                            ]
                                   );

            #endregion

            #region Tariff 2  (two tariff elements, switching after 3 hours!)

            var tariff2          = new Tariff(
                                       CountryCode:         CountryCode.Parse("DE"),
                                       PartyId:             Party_Id.   Parse("GEF"),
                                       Id:                  Tariff_Id.  Parse("tariff2"),
                                       Currency:            OCPI.Currency.EUR,
                                       TariffElements:      [
                                                                new TariffElement(
                                                                    [
                                                                        PriceComponent.Energy(
                                                                            Price:     0.44m,
                                                                            StepSize:  1000
                                                                        ),
                                                                        PriceComponent.ChargingTime(
                                                                            Price:     0m,
                                                                            Duration:  TimeSpan.FromMinutes(1)
                                                                        ),
                                                                        PriceComponent.FlatRate(
                                                                            Price:     0.3m
                                                                        )
                                                                    ],
                                                                    new TariffRestrictions(
                                                                        MaxDuration:   TimeSpan.FromHours(3)
                                                                    )
                                                                ),
                                                                new TariffElement(
                                                                    PriceComponent.Energy(
                                                                        Price:     0.44m,
                                                                        StepSize:  1000
                                                                    ),
                                                                    PriceComponent.ChargingTime(
                                                                        Price:     5.04m,
                                                                        Duration:  TimeSpan.FromMinutes(1)
                                                                    )
                                                                )
                                                            ]
                                   );

            // {
            //   "id":              "tariff2",
            //   "currency":        "EUR",
            //   "elements": [
            //     {
            //       "price_components": [
            //         {
            //           "type":        "ENERGY",
            //           "price":        0.44,
            //           "step_size":    1000
            //         },
            //         {
            //           "type":        "TIME",
            //           "price":        0.0,
            //           "step_size":    60
            //         },
            //         {
            //           "type":        "FLAT",
            //           "price":        0.3,
            //           "step_size":    1
            //         }
            //       ],
            //       "restrictions": {
            //         "max_duration":   10800
            //       }
            //     },
            //     {
            //       "price_components": [
            //         {
            //           "type":        "ENERGY",
            //           "price":        0.44,
            //           "step_size":    1000
            //         },
            //         {
            //           "type":        "TIME",
            //           "price":        5.04,
            //           "step_size":    60
            //         }
            //       ]
            //     }
            //   ],
            //   "created":         "2024-05-14T18:16:11.369Z",
            //   "last_updated":    "2024-05-14T18:16:11.369Z"
            // }

            #endregion


            #region Validate cdr1 with tariff1  (2h,   1 tariff element!)

            var cdr1WithTariff1 = cdr1.SplittIntoChargingPeriods(meteringValues1, [tariff1]);

            Assert.Multiple(() => {

                Assert.That(cdr1WithTariff1,                                                   Is.Not.Null);
                Assert.That(cdr1WithTariff1.Start.ToIso8601(),                                 Is.EqualTo( start1.  ToIso8601()));
                Assert.That(cdr1WithTariff1.Stop. ToIso8601(),                                 Is.EqualTo( stop1.   ToIso8601()));
                Assert.That(cdr1WithTariff1.TotalTime,                                         Is.EqualTo( TimeSpan.FromHours(2)));
                Assert.That(cdr1WithTariff1.TotalChargingTime,                                 Is.EqualTo( TimeSpan.FromHours(2)));
                Assert.That(cdr1WithTariff1.TotalEnergy.kWh,                                   Is.EqualTo( 10m));
                Assert.That(cdr1WithTariff1.TotalParkingTime,                                  Is.EqualTo( TimeSpan.Zero));
                Assert.That(cdr1WithTariff1.Currency,                                          Is.EqualTo( OCPI.Currency.EUR));
                Assert.That(cdr1WithTariff1.TotalCost,                                         Is.EqualTo( 14.78m)); // €

                Assert.That(cdr1WithTariff1.ChargingPeriods.Count(),                           Is.EqualTo(1));


                // 1st charging period
                var chargingPeriod1 = cdr1WithTariff1.ChargingPeriods.First();
                Assert.That(chargingPeriod1,                                                   Is.Not.Null);

                Assert.That(chargingPeriod1.StartTimestamp.                     ToIso8601(),   Is.EqualTo( cdr1WithTariff1.Start.                         ToIso8601()));
                Assert.That(chargingPeriod1.StartMeteringValue!.Value.Timestamp.ToIso8601(),   Is.EqualTo( cdr1WithTariff1.Start.                         ToIso8601()));
                Assert.That(chargingPeriod1.StartMeteringValue!.Value.WattHours.kWh,           Is.EqualTo(  1m));

                Assert.That(chargingPeriod1.Duration,                                          Is.EqualTo( TimeSpan.FromHours(2)));

                Assert.That(chargingPeriod1.StopTimestamp!.     Value.          ToIso8601(),   Is.EqualTo((cdr1WithTariff1.Start + TimeSpan.FromHours(2)).ToIso8601()));
                Assert.That(chargingPeriod1.StopMeteringValue!. Value.Timestamp.ToIso8601(),   Is.EqualTo((cdr1WithTariff1.Start + TimeSpan.FromHours(2)).ToIso8601()));
                Assert.That(chargingPeriod1.StopMeteringValue!. Value.WattHours.kWh,           Is.EqualTo( 11m));

                Assert.That(chargingPeriod1.Energy.kWh,                                        Is.EqualTo( 10m));
                Assert.That(chargingPeriod1.EnergyPrice,                                       Is.EqualTo( 0.44m));
                Assert.That(chargingPeriod1.EnergyStepSize,                                    Is.EqualTo( 1000));

                Assert.That(chargingPeriod1.Duration,                                          Is.EqualTo( TimeSpan.FromHours(2)));
                Assert.That(chargingPeriod1.TimePrice,                                         Is.EqualTo( 5.04m));
                Assert.That(chargingPeriod1.TimeStepSize,                                      Is.EqualTo( 60));

                Assert.That(chargingPeriod1.PowerAverage.kW,                                   Is.EqualTo( 5m));

            });

            #endregion

            #region Validate cdr2 with tariff1  (3h,   1 tariff element!)

            var cdr2WithTariff1 = cdr2.SplittIntoChargingPeriods(meteringValues2, [tariff1]);

            Assert.Multiple(() => {

                Assert.That(cdr2WithTariff1,                                                   Is.Not.Null);
                Assert.That(cdr2WithTariff1.Start.ToIso8601(),                                 Is.EqualTo( start2.  ToIso8601()));
                Assert.That(cdr2WithTariff1.Stop. ToIso8601(),                                 Is.EqualTo( stop2.   ToIso8601()));
                Assert.That(cdr2WithTariff1.TotalTime,                                         Is.EqualTo( TimeSpan.FromHours(3)));
                Assert.That(cdr2WithTariff1.TotalChargingTime,                                 Is.EqualTo( TimeSpan.FromHours(3)));
                Assert.That(cdr2WithTariff1.TotalEnergy.kWh,                                   Is.EqualTo( 18m));
                Assert.That(cdr2WithTariff1.TotalParkingTime,                                  Is.EqualTo( TimeSpan.Zero));
                Assert.That(cdr2WithTariff1.Currency,                                          Is.EqualTo( OCPI.Currency.EUR));
                Assert.That(cdr2WithTariff1.TotalCost,                                         Is.EqualTo( 23.34m)); // €

                Assert.That(cdr2WithTariff1.ChargingPeriods.Count(),                           Is.EqualTo(1));


                // 1st charging period
                var chargingPeriod1 = cdr2WithTariff1.ChargingPeriods.First();
                Assert.That(chargingPeriod1,                                                   Is.Not.Null);

                Assert.That(chargingPeriod1.StartTimestamp.                     ToIso8601(),   Is.EqualTo( cdr2WithTariff1.Start.                         ToIso8601()));
                Assert.That(chargingPeriod1.StartMeteringValue!.Value.Timestamp.ToIso8601(),   Is.EqualTo( cdr2WithTariff1.Start.                         ToIso8601()));
                Assert.That(chargingPeriod1.StartMeteringValue!.Value.WattHours.kWh,           Is.EqualTo(  2m));

                Assert.That(chargingPeriod1.Duration,                                          Is.EqualTo( TimeSpan.FromHours(3)));

                Assert.That(chargingPeriod1.StopTimestamp!.     Value.          ToIso8601(),   Is.EqualTo((cdr2WithTariff1.Start + TimeSpan.FromHours(3)).ToIso8601()));
                Assert.That(chargingPeriod1.StopMeteringValue!. Value.Timestamp.ToIso8601(),   Is.EqualTo((cdr2WithTariff1.Start + TimeSpan.FromHours(3)).ToIso8601()));
                Assert.That(chargingPeriod1.StopMeteringValue!. Value.WattHours.kWh,           Is.EqualTo( 20m));

                Assert.That(chargingPeriod1.Energy.kWh,                                        Is.EqualTo( 18m));
                Assert.That(chargingPeriod1.EnergyPrice,                                       Is.EqualTo( 0.44m));
                Assert.That(chargingPeriod1.EnergyStepSize,                                    Is.EqualTo( 1000));

                Assert.That(chargingPeriod1.Duration,                                          Is.EqualTo( TimeSpan.FromHours(3)));
                Assert.That(chargingPeriod1.TimePrice,                                         Is.EqualTo( 5.04m));
                Assert.That(chargingPeriod1.TimeStepSize,                                      Is.EqualTo( 60));

                Assert.That(chargingPeriod1.PowerAverage.kW,                                   Is.EqualTo( 6m));

            });

            #endregion

            #region Validate cdr3 with tariff1  (7h,   1 tariff element!)

            var cdr3WithTariff1 = cdr3.SplittIntoChargingPeriods(meteringValues3, [tariff1]);

            Assert.Multiple(() => {

                Assert.That(cdr3WithTariff1,                                                   Is.Not.Null);
                Assert.That(cdr3WithTariff1.Start.ToIso8601(),                                 Is.EqualTo( start3.  ToIso8601()));
                Assert.That(cdr3WithTariff1.Stop. ToIso8601(),                                 Is.EqualTo( stop3.   ToIso8601()));
                Assert.That(cdr3WithTariff1.TotalTime,                                         Is.EqualTo( TimeSpan.FromHours(7)));
                Assert.That(cdr3WithTariff1.TotalChargingTime,                                 Is.EqualTo( TimeSpan.FromHours(7)));
                Assert.That(cdr3WithTariff1.TotalEnergy.kWh,                                   Is.EqualTo( 20m));
                Assert.That(cdr3WithTariff1.TotalParkingTime,                                  Is.EqualTo( TimeSpan.Zero));
                Assert.That(cdr3WithTariff1.Currency,                                          Is.EqualTo( OCPI.Currency.EUR));
                Assert.That(cdr3WithTariff1.TotalCost,                                         Is.EqualTo( 44.38m)); // €

                Assert.That(cdr3WithTariff1.ChargingPeriods.Count(),                           Is.EqualTo(1));


                // 1st charging period
                var chargingPeriod1 = cdr3WithTariff1.ChargingPeriods.First();
                Assert.That(chargingPeriod1,                                                   Is.Not.Null);

                Assert.That(chargingPeriod1.StartTimestamp.                     ToIso8601(),   Is.EqualTo( cdr3WithTariff1.Start.                         ToIso8601()));
                Assert.That(chargingPeriod1.StartMeteringValue!.Value.Timestamp.ToIso8601(),   Is.EqualTo( cdr3WithTariff1.Start.                         ToIso8601()));
                Assert.That(chargingPeriod1.StartMeteringValue!.Value.WattHours.kWh,           Is.EqualTo(  2m));

                Assert.That(chargingPeriod1.Duration,                                          Is.EqualTo( TimeSpan.FromHours(7)));

                Assert.That(chargingPeriod1.StopTimestamp!.     Value.          ToIso8601(),   Is.EqualTo((cdr3WithTariff1.Start + TimeSpan.FromHours(7)).ToIso8601()));
                Assert.That(chargingPeriod1.StopMeteringValue!. Value.Timestamp.ToIso8601(),   Is.EqualTo((cdr3WithTariff1.Start + TimeSpan.FromHours(7)).ToIso8601()));
                Assert.That(chargingPeriod1.StopMeteringValue!. Value.WattHours.kWh,           Is.EqualTo( 22m));

                Assert.That(chargingPeriod1.Energy.kWh,                                        Is.EqualTo( 20m));
                Assert.That(chargingPeriod1.EnergyPrice,                                       Is.EqualTo( 0.44m));
                Assert.That(chargingPeriod1.EnergyStepSize,                                    Is.EqualTo( 1000));

                Assert.That(chargingPeriod1.Duration,                                          Is.EqualTo( TimeSpan.FromHours(7)));
                Assert.That(chargingPeriod1.TimePrice,                                         Is.EqualTo( 5.04m));
                Assert.That(chargingPeriod1.TimeStepSize,                                      Is.EqualTo( 60));

                Assert.That(chargingPeriod1.PowerAverage.kW,                                   Is.EqualTo( 2.8571428571428571428571428571m));

            });

            #endregion


            #region Validate cdr1 with tariff2  (2h, 2-1 tariff elements!)

            var cdr1WithTariff2 = cdr1.SplittIntoChargingPeriods(meteringValues1, [tariff2]);

            // {
            //   "id": "cdr1",
            //   "start_date_time": "2024-05-14T20:28:55.445Z",
            //   "stop_date_time": "2024-05-14T22:28:55.445Z",
            //   "auth_id": "4p31E5U3b3nptp2xvGQ696Q6Qx7pGn",
            //   "auth_method": "AUTH_REQUEST",
            //   "location": {
            //     "id": "8U6Ap6rbQj3AAjMK6C11MYU63z1Kj9",
            //     "type": "ON_STREET",
            //     "address": "Biberweg 18",
            //     "city": "Jena",
            //     "postal_code": "07749",
            //     "country": "DEU",
            //     "coordinates": {
            //       "latitude": "50.928365",
            //       "longitude": "11.589986"
            //     },
            //     "time_zone": null,
            //     "created": "2024-05-14T23:28:55.471Z",
            //     "last_updated": "2024-05-14T23:28:55.471Z"
            //   },
            //   "currency": "EUR",
            //   "tariffs": [
            //     {
            //       "id": "tariff2",
            //       "currency": "EUR",
            //       "elements": [
            //         {
            //           "price_components": [
            //             {
            //               "type": "ENERGY",
            //               "price": 0.44,
            //               "step_size": 1000
            //             },
            //             {
            //               "type": "TIME",
            //               "price": 0.0,
            //               "step_size": 60
            //             },
            //             {
            //               "type": "FLAT",
            //               "price": 0.3,
            //               "step_size": 1
            //             }
            //           ],
            //           "restrictions": {
            //             "max_duration": 10800
            //           }
            //         },
            //         {
            //           "price_components": [
            //             {
            //               "type": "ENERGY",
            //               "price": 0.44,
            //               "step_size": 1000
            //             },
            //             {
            //               "type": "TIME",
            //               "price": 5.04,
            //               "step_size": 60
            //             }
            //           ]
            //         }
            //       ],
            //       "created": "2024-05-14T23:28:55.510Z",
            //       "last_updated": "2024-05-14T23:28:55.510Z"
            //     }
            //   ],
            //   "charging_periods": [
            //     {
            //       "start_date_time": "2024-05-14T20:28:55.445Z",
            //       "dimensions": [
            //         {
            //           "type": "ENERGY",
            //           "volume": 10.0
            //         }
            //       ]
            //     }
            //   ],
            //   "total_cost":               4.70,
            //   "total_energy":            10.0,
            //   "total_time":               2.0,
            //   "total_parking_time":       0.0,
            //   "create":                  "2024-05-14T23:28:55.490Z",
            //   "last_updated":            "2024-05-14T23:28:55.490Z"
            // }

            Assert.Multiple(() => {

                Assert.That(cdr1WithTariff2,                                                   Is.Not.Null);
                Assert.That(cdr1WithTariff2.Start.ToIso8601(),                                 Is.EqualTo( start1.  ToIso8601()));
                Assert.That(cdr1WithTariff2.Stop. ToIso8601(),                                 Is.EqualTo( stop1.   ToIso8601()));
                Assert.That(cdr1WithTariff2.TotalTime,                                         Is.EqualTo( TimeSpan.FromHours(2)));
                Assert.That(cdr1WithTariff2.TotalChargingTime,                                 Is.EqualTo( TimeSpan.FromHours(2)));
                Assert.That(cdr1WithTariff2.TotalEnergy.kWh,                                   Is.EqualTo( 10m));
                Assert.That(cdr1WithTariff2.TotalParkingTime,                                  Is.EqualTo( TimeSpan.Zero));
                Assert.That(cdr1WithTariff2.Currency,                                          Is.EqualTo( OCPI.Currency.EUR));
                Assert.That(cdr1WithTariff2.TotalCost,                                         Is.EqualTo( 4.7m)); // €

                Assert.That(cdr1WithTariff2.ChargingPeriods.Count(),                           Is.EqualTo(1));


                // 1st charging period
                var chargingPeriod1 = cdr1WithTariff2.ChargingPeriods.First();
                Assert.That(chargingPeriod1,                                                   Is.Not.Null);

                Assert.That(chargingPeriod1.StartTimestamp.                     ToIso8601(),   Is.EqualTo( cdr1WithTariff2.Start.                         ToIso8601()));
                Assert.That(chargingPeriod1.StartMeteringValue!.Value.Timestamp.ToIso8601(),   Is.EqualTo( cdr1WithTariff2.Start.                         ToIso8601()));
                Assert.That(chargingPeriod1.StartMeteringValue!.Value.WattHours.kWh,           Is.EqualTo(  1m));

                Assert.That(chargingPeriod1.Duration,                                          Is.EqualTo( TimeSpan.FromHours(2)));

                Assert.That(chargingPeriod1.StopTimestamp!.     Value.          ToIso8601(),   Is.EqualTo((cdr1WithTariff2.Start + TimeSpan.FromHours(2)).ToIso8601()));
                Assert.That(chargingPeriod1.StopMeteringValue!. Value.Timestamp.ToIso8601(),   Is.EqualTo((cdr1WithTariff2.Start + TimeSpan.FromHours(2)).ToIso8601()));
                Assert.That(chargingPeriod1.StopMeteringValue!. Value.WattHours.kWh,           Is.EqualTo( 11m));

                Assert.That(chargingPeriod1.Energy.kWh,                                        Is.EqualTo( 10m));
                Assert.That(chargingPeriod1.EnergyPrice,                                       Is.EqualTo( 0.44m));
                Assert.That(chargingPeriod1.EnergyStepSize,                                    Is.EqualTo( 1000));

                Assert.That(chargingPeriod1.Duration,                                          Is.EqualTo( TimeSpan.FromHours(2)));
                Assert.That(chargingPeriod1.TimePrice,                                         Is.EqualTo( 0m));
                Assert.That(chargingPeriod1.TimeStepSize,                                      Is.EqualTo( 0));

                Assert.That(chargingPeriod1.PowerAverage.kW,                                   Is.EqualTo( 5m));

            });

            #endregion

            #region Validate cdr2 with tariff2  (3h, 2-1 tariff elements!)

            var cdr2WithTariff2 = cdr2.SplittIntoChargingPeriods(meteringValues2, [tariff2]);

            // {
            //   "id": "cdr2",
            //   "start_date_time": "2024-05-14T17:23:13.138Z",
            //   "stop_date_time": "2024-05-14T20:23:13.138Z",
            //   "auth_id": "65W78E3f726EvYtMn7U929jj6jQ823",
            //   "auth_method": "AUTH_REQUEST",
            //   "location": {
            //     "id": "2hYf3574U7K5j3j1S2WGb35G47M91b",
            //     "type": "ON_STREET",
            //     "address": "Biberweg 18",
            //     "city": "Jena",
            //     "postal_code": "07749",
            //     "country": "DEU",
            //     "coordinates": {
            //       "latitude": "50.928365",
            //       "longitude": "11.589986"
            //     },
            //     "time_zone": null,
            //     "created": "2024-05-14T23:23:13.187Z",
            //     "last_updated": "2024-05-14T23:23:13.187Z"
            //   },
            //   "currency": "EUR",
            //   "tariffs": [
            //     {
            //       "id": "tariff2",
            //       "currency": "EUR",
            //       "elements": [
            //         {
            //           "price_components": [
            //             {
            //               "type": "ENERGY",
            //               "price": 0.44,
            //               "step_size": 1000
            //             },
            //             {
            //               "type": "TIME",
            //               "price": 0.0,
            //               "step_size": 60
            //             },
            //             {
            //               "type": "FLAT",
            //               "price": 0.3,
            //               "step_size": 1
            //             }
            //           ],
            //           "restrictions": {
            //             "max_duration": 10800
            //           }
            //         },
            //         {
            //           "price_components": [
            //             {
            //               "type": "ENERGY",
            //               "price": 0.44,
            //               "step_size": 1000
            //             },
            //             {
            //               "type": "TIME",
            //               "price": 5.04,
            //               "step_size": 60
            //             }
            //           ]
            //         }
            //       ],
            //       "created": "2024-05-14T23:23:13.200Z",
            //       "last_updated": "2024-05-14T23:23:13.200Z"
            //     }
            //   ],
            //   "charging_periods": [
            //     {
            //       "start_date_time": "2024-05-14T17:23:13.138Z",
            //       "dimensions": [
            //         {
            //           "type": "ENERGY",
            //           "volume": 18.0
            //         }
            //       ]
            //     }
            //   ],
            //   "total_cost": 8.22,
            //   "total_energy": 18.0,
            //   "total_time": 3.0,
            //   "total_parking_time": 0.0,
            //   "create": "2024-05-14T23:23:13.188Z",
            //   "last_updated": "2024-05-14T23:23:13.188Z"
            // }

            Assert.Multiple(() => {

                Assert.That(cdr2WithTariff2,                                                   Is.Not.Null);
                Assert.That(cdr2WithTariff2.Start.ToIso8601(),                                 Is.EqualTo( start2.  ToIso8601()));
                Assert.That(cdr2WithTariff2.Stop. ToIso8601(),                                 Is.EqualTo( stop2.   ToIso8601()));
                Assert.That(cdr2WithTariff2.TotalTime,                                         Is.EqualTo( TimeSpan.FromHours(3)));
                Assert.That(cdr2WithTariff2.TotalChargingTime,                                 Is.EqualTo( TimeSpan.FromHours(3)));
                Assert.That(cdr2WithTariff2.TotalEnergy.kWh,                                   Is.EqualTo( 18m));
                Assert.That(cdr2WithTariff2.TotalParkingTime,                                  Is.EqualTo( TimeSpan.Zero));
                Assert.That(cdr2WithTariff2.Currency,                                          Is.EqualTo( OCPI.Currency.EUR));
                Assert.That(cdr2WithTariff2.TotalCost,                                         Is.EqualTo( 8.22m)); // €

                Assert.That(cdr2WithTariff2.ChargingPeriods.Count(),                           Is.EqualTo(1));


                // 1st charging period
                var chargingPeriod1 = cdr2WithTariff2.ChargingPeriods.First();
                Assert.That(chargingPeriod1,                                                   Is.Not.Null);

                Assert.That(chargingPeriod1.StartTimestamp.                     ToIso8601(),   Is.EqualTo( cdr2WithTariff2.Start.                         ToIso8601()));
                Assert.That(chargingPeriod1.StartMeteringValue!.Value.Timestamp.ToIso8601(),   Is.EqualTo( cdr2WithTariff2.Start.                         ToIso8601()));
                Assert.That(chargingPeriod1.StartMeteringValue!.Value.WattHours.kWh,           Is.EqualTo(  2m));

                Assert.That(chargingPeriod1.Duration,                                          Is.EqualTo( TimeSpan.FromHours(3)));

                Assert.That(chargingPeriod1.StopTimestamp!.     Value.          ToIso8601(),   Is.EqualTo((cdr2WithTariff2.Start + TimeSpan.FromHours(3)).ToIso8601()));
                Assert.That(chargingPeriod1.StopMeteringValue!. Value.Timestamp.ToIso8601(),   Is.EqualTo((cdr2WithTariff2.Start + TimeSpan.FromHours(3)).ToIso8601()));
                Assert.That(chargingPeriod1.StopMeteringValue!. Value.WattHours.kWh,           Is.EqualTo( 20m));

                Assert.That(chargingPeriod1.Energy.kWh,                                        Is.EqualTo( 18m));
                Assert.That(chargingPeriod1.EnergyPrice,                                       Is.EqualTo( 0.44m));
                Assert.That(chargingPeriod1.EnergyStepSize,                                    Is.EqualTo( 1000));

                Assert.That(chargingPeriod1.Duration,                                          Is.EqualTo( TimeSpan.FromHours(3)));
                Assert.That(chargingPeriod1.TimePrice,                                         Is.EqualTo( 0m));
                Assert.That(chargingPeriod1.TimeStepSize,                                      Is.EqualTo( 0));

                Assert.That(chargingPeriod1.PowerAverage.kW,                                   Is.EqualTo( 6m));

            });

            #endregion

            #region Validate cdr3 with tariff2  (7h,   2 tariff elements!)

            var cdr3WithTariff2 = cdr3.SplittIntoChargingPeriods(meteringValues3, [tariff2]);

            // {
            //   "id":                  "cdr2",
            //   "start_date_time":     "2024-05-14T14:23:13.138Z",
            //   "stop_date_time":      "2024-05-14T21:23:13.138Z",
            //   "auth_id":             "485hGpMz73Q3hvC7Q11CCnxnY5U32C",
            //   "auth_method":         "AUTH_REQUEST",
            //   "location": {
            //     "id": "jU112pGMp4p8h35z33Wv4796xdrxrb",
            //     "type": "ON_STREET",
            //     "address": "Biberweg 18",
            //     "city": "Jena",
            //     "postal_code": "07749",
            //     "country": "DEU",
            //     "coordinates": {
            //       "latitude": "50.928365",
            //       "longitude": "11.589986"
            //     },
            //     "time_zone": null,
            //     "created": "2024-05-14T23:23:13.188Z",
            //     "last_updated": "2024-05-14T23:23:13.188Z"
            //   },
            //   "currency": "EUR",
            //   "tariffs": [
            //     {
            //       "id": "tariff2",
            //       "currency": "EUR",
            //       "elements": [
            //         {
            //           "price_components": [
            //             {
            //               "type": "ENERGY",
            //               "price": 0.44,
            //               "step_size": 1000
            //             },
            //             {
            //               "type": "TIME",
            //               "price": 0.0,
            //               "step_size": 60
            //             },
            //             {
            //               "type": "FLAT",
            //               "price": 0.3,
            //               "step_size": 1
            //             }
            //           ],
            //           "restrictions": {
            //             "max_duration": 10800
            //           }
            //         },
            //         {
            //           "price_components": [
            //             {
            //               "type": "ENERGY",
            //               "price": 0.44,
            //               "step_size": 1000
            //             },
            //             {
            //               "type": "TIME",
            //               "price": 5.04,
            //               "step_size": 60
            //             }
            //           ]
            //         }
            //       ],
            //       "created": "2024-05-14T23:23:13.200Z",
            //       "last_updated": "2024-05-14T23:23:13.200Z"
            //     }
            //   ],
            //   "charging_periods": [
            //     {
            //       "start_date_time": "2024-05-14T14:23:13.138Z",
            //       "dimensions": [
            //         {
            //           "type": "ENERGY",
            //           "volume": 8.571428571428571428571428572
            //         }
            //       ]
            //     },
            //     {
            //       "start_date_time": "2024-05-14T17:23:13.138Z",
            //       "dimensions": [
            //         {
            //           "type": "ENERGY",
            //           "volume": 11.428571428571428571428571428
            //         },
            //         {
            //           "type": "TIME",
            //           "volume": 4.0
            //         }
            //       ]
            //     }
            //   ],
            //   "total_cost": 29.26,
            //   "total_energy": 20.000000000000000000000000000,
            //   "total_time": 7.0,
            //   "total_parking_time": 0.0,
            //   "create": "2024-05-14T23:23:13.188Z",
            //   "last_updated": "2024-05-14T23:23:13.188Z"
            // }

            Assert.Multiple(() => {

                Assert.That(cdr3WithTariff2,                                                   Is.Not.Null);
                Assert.That(cdr3WithTariff2.Start.ToIso8601(),                                 Is.EqualTo( start3.  ToIso8601()));
                Assert.That(cdr3WithTariff2.Stop. ToIso8601(),                                 Is.EqualTo( stop3.   ToIso8601()));
                Assert.That(cdr3WithTariff2.TotalTime,                                         Is.EqualTo( TimeSpan.FromHours(7)));
                Assert.That(cdr3WithTariff2.TotalChargingTime,                                 Is.EqualTo( TimeSpan.FromHours(7)));
                Assert.That(cdr3WithTariff2.TotalEnergy.kWh,                                   Is.EqualTo( 20m));
                Assert.That(cdr3WithTariff2.TotalParkingTime,                                  Is.EqualTo( TimeSpan.Zero));
                Assert.That(cdr3WithTariff2.Currency,                                          Is.EqualTo( OCPI.Currency.EUR));
                Assert.That(cdr3WithTariff2.TotalCost,                                         Is.EqualTo( 29.26m)); // €

                Assert.That(cdr3WithTariff2.CostDetails?.BilledEnergy.kWh,                     Is.EqualTo( 20m));
                Assert.That(cdr3WithTariff2.CostDetails?.BilledTime,                           Is.EqualTo( new TimeSpan(4, 00, 00)));
                Assert.That(cdr3WithTariff2.CostDetails?.TotalEnergyCost,                      Is.EqualTo(  8.80m).Within(0.001m));
                Assert.That(cdr3WithTariff2.CostDetails?.TotalTimeCost,                        Is.EqualTo( 20.16m).Within(0.001m));


                Assert.That(cdr3WithTariff2.ChargingPeriods.Count(),                           Is.EqualTo(2));

                // 1st charging period
                var chargingPeriod1 = cdr3WithTariff2.ChargingPeriods.ElementAt(0);
                Assert.That(chargingPeriod1,                                                   Is.Not.Null);

                Assert.That(chargingPeriod1.StartTimestamp.                     ToIso8601(),   Is.EqualTo( cdr3WithTariff2.Start.                         ToIso8601()));
                Assert.That(chargingPeriod1.StartMeteringValue!.Value.Timestamp.ToIso8601(),   Is.EqualTo( cdr3WithTariff2.Start.                         ToIso8601()));
                Assert.That(chargingPeriod1.StartMeteringValue!.Value.WattHours.kWh,           Is.EqualTo(  2m));

                Assert.That(chargingPeriod1.Duration,                                          Is.EqualTo( TimeSpan.FromHours(3)));

                Assert.That(chargingPeriod1.StopTimestamp!.     Value.          ToIso8601(),   Is.EqualTo((cdr3WithTariff2.Start + TimeSpan.FromHours(3)).ToIso8601()));
                Assert.That(chargingPeriod1.StopMeteringValue!. Value.Timestamp.ToIso8601(),   Is.EqualTo((cdr3WithTariff2.Start + TimeSpan.FromHours(3)).ToIso8601()));
                Assert.That(chargingPeriod1.StopMeteringValue!. Value.WattHours.kWh,           Is.EqualTo( 10.571428571428571428571428572m));

                Assert.That(chargingPeriod1.Energy.kWh,                                        Is.EqualTo( 8.571428571428571428571428572m));
                Assert.That(chargingPeriod1.EnergyPrice,                                       Is.EqualTo( 0.44m));
                Assert.That(chargingPeriod1.EnergyStepSize,                                    Is.EqualTo( 1000));

                Assert.That(chargingPeriod1.Duration,                                          Is.EqualTo( TimeSpan.FromHours(3)));
                Assert.That(chargingPeriod1.TimePrice,                                         Is.EqualTo( 0m));
                Assert.That(chargingPeriod1.TimeStepSize,                                      Is.EqualTo( 0));

                Assert.That(chargingPeriod1.PowerAverage.kW,                                   Is.EqualTo( 2.8571428571428571428571428573m));


                // 2nd charging period
                var chargingPeriod2 = cdr3WithTariff2.ChargingPeriods.ElementAt(1);
                Assert.That(chargingPeriod2,                                                   Is.Not.Null);

                Assert.That(chargingPeriod2.StartTimestamp.                     ToIso8601(),   Is.EqualTo((cdr3WithTariff2.Start + TimeSpan.FromHours(3)).ToIso8601()));
                Assert.That(chargingPeriod2.StartMeteringValue!.Value.Timestamp.ToIso8601(),   Is.EqualTo((cdr3WithTariff2.Start + TimeSpan.FromHours(3)).ToIso8601()));
                Assert.That(chargingPeriod2.StartMeteringValue!.Value.WattHours.kWh,           Is.EqualTo( 10.571428571428571428571428572m));

                Assert.That(chargingPeriod2.Duration,                                          Is.EqualTo( TimeSpan.FromHours(4)));

                Assert.That(chargingPeriod2.StopTimestamp!.     Value.          ToIso8601(),   Is.EqualTo( cdr3WithTariff2.Stop.                          ToIso8601()));
                Assert.That(chargingPeriod2.StopMeteringValue!. Value.Timestamp.ToIso8601(),   Is.EqualTo( cdr3WithTariff2.Stop.                          ToIso8601()));
                Assert.That(chargingPeriod2.StopMeteringValue!. Value.WattHours.kWh,           Is.EqualTo( 22m));

                Assert.That(chargingPeriod2.Energy.kWh,                                        Is.EqualTo( 11.428571428571428571428571428m));
                Assert.That(chargingPeriod2.EnergyPrice,                                       Is.EqualTo( 0.44m));
                Assert.That(chargingPeriod2.EnergyStepSize,                                    Is.EqualTo( 1000));

                Assert.That(chargingPeriod2.Duration,                                          Is.EqualTo( TimeSpan.FromHours(4)));
                Assert.That(chargingPeriod2.TimePrice,                                         Is.EqualTo( 5.04m));
                Assert.That(chargingPeriod2.TimeStepSize,                                      Is.EqualTo( 60));

                Assert.That(chargingPeriod1.PowerAverage.kW,                                   Is.EqualTo( 2.8571428571428571428571428573m));

            });

            #endregion

        }

        #endregion


        #region SplittCDRIntoChargingPeriods_Test02()

        /// <summary>
        /// Augment an OCPI charge detail record with charging tariff information.
        /// </summary>
        [Test]
        public static void SplittCDRIntoChargingPeriods_Test02()
        {

            var now = Timestamp.Now;

            // SessionStart/-Stop == ChargingStart-/Stop!

            #region CDR

            var start1          = DateTime.Parse("2024-04-14T19:26:20.000Z");
            var stop1           = DateTime.Parse("2024-04-15T06:36:36.000Z");

            var meteringValues  = new[] {
                                      new Timestamped<WattHour>(start1, WattHour.ParseKWh(    0m)),
                                      new Timestamped<WattHour>(stop1,  WattHour.ParseKWh(59.51m))
                                  };

            var cdr             = new CDR(
                                      CountryCode:         CountryCode.Parse("DE"),
                                      PartyId:             Party_Id.   Parse("GEF"),
                                      Id:                  CDR_Id.     Parse("cdr1"),
                                      Start:               start1,
                                      Stop:                stop1,
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
                                                               Coordinates:    GeoCoordinate.FromLatLng(50.928365, 11.589986),
                                                               EVSEs:          [
                                                                                   new EVSE(
                                                                                       UId:          EVSE_UId.Parse("DE*GEF*E1234*1"),
                                                                                       Status:       StatusType.CHARGING,
                                                                                       Connectors:   [
                                                                                                         new Connector(
                                                                                                             Id:         Connector_Id.Parse("1"),
                                                                                                             Standard:   ConnectorType.IEC_62196_T2,
                                                                                                             Format:     ConnectorFormats.SOCKET,
                                                                                                             PowerType:  PowerTypes.AC_3_PHASE,
                                                                                                             Voltage:    Volt.  ParseV(230),
                                                                                                             Amperage:   Ampere.ParseA(32)
                                                                                                         )
                                                                                                     ]
                                                                                   )
                                                                               ]
                                                           ),
                                      Currency:            OCPI.Currency.EUR,
                                      ChargingPeriods:     [
                                                               new ChargingPeriod(
                                                                   start1,
                                                                   [ CDRDimension.ENERGY(1) ]
                                                               )
                                                           ],
                                      TotalCost:           0,
                                      TotalEnergy:         WattHour.Zero,
                                      TotalTime:           TimeSpan.Zero
                                  );

            #endregion

            #region Tariff

            var tariff          = new Tariff(
                                      CountryCode:         CountryCode.Parse("DE"),
                                      PartyId:             Party_Id.   Parse("GEF"),
                                      Id:                  Tariff_Id.  Parse("tariff1"),
                                      Currency:            OCPI.Currency.EUR,
                                      TariffElements:      [
                                                               new TariffElement(
                                                                   [
                                                                       PriceComponent.Energy(
                                                                           Price:     0.44m,
                                                                           StepSize:  1000
                                                                       ),
                                                                       PriceComponent.ChargingTime(
                                                                           Price:     0m,
                                                                           Duration:  TimeSpan.FromMinutes(1)
                                                                       ),
                                                                       PriceComponent.FlatRate(
                                                                           Price:     0.3m
                                                                       )
                                                                   ],
                                                                   new TariffRestrictions(
                                                                       MaxDuration:   TimeSpan.FromHours(3)
                                                                   )
                                                               ),
                                                               new TariffElement(
                                                                   PriceComponent.Energy(
                                                                       Price:     0.44m,
                                                                       StepSize:  1000
                                                                   ),
                                                                   PriceComponent.ChargingTime(
                                                                       Price:     5.04m,
                                                                       Duration:  TimeSpan.FromMinutes(1)
                                                                   )
                                                               )
                                                           ]
                                  );

            #endregion


            var cdrWithTariff   = cdr.SplittIntoChargingPeriods(meteringValues, [tariff]);


        }

        #endregion

        #region SplittCDRIntoChargingPeriods_Test03()

        /// <summary>
        /// Augment an OCPI charge detail record with charging tariff information.
        /// </summary>
        [Test]
        public static void SplittCDRIntoChargingPeriods_Test03()
        {

            var now = Timestamp.Now;

            // SessionStart/-Stop != ChargingStart-/Stop!

            #region CDR

            var sessionStart    = DateTime.Parse("2024-04-14T19:25:45.000Z");
            var chargingStart   = DateTime.Parse("2024-04-14T19:26:20.000Z");
            var chargingStop    = DateTime.Parse("2024-04-15T06:36:36.000Z");
            var sessionStop     = DateTime.Parse("2024-04-15T06:40:02.000Z");

            var meteringValues  = new[] {
                                      new Timestamped<WattHour>(chargingStart, WattHour.ParseKWh(    0m)),
                                      new Timestamped<WattHour>(chargingStop,  WattHour.ParseKWh(59.51m))
                                  };

            var cdr             = new CDR(
                                      CountryCode:         CountryCode.Parse("DE"),
                                      PartyId:             Party_Id.   Parse("GEF"),
                                      Id:                  CDR_Id.     Parse("cdr1"),
                                      Start:               sessionStart,
                                      Stop:                sessionStop,
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
                                                               Coordinates:    GeoCoordinate.FromLatLng(50.928365, 11.589986),
                                                               EVSEs:          [
                                                                                   new EVSE(
                                                                                       UId:          EVSE_UId.Parse("DE*GEF*E1234*1"),
                                                                                       Status:       StatusType.CHARGING,
                                                                                       Connectors:   [
                                                                                                         new Connector(
                                                                                                             Id:         Connector_Id.Parse("1"),
                                                                                                             Standard:   ConnectorType.IEC_62196_T2,
                                                                                                             Format:     ConnectorFormats.SOCKET,
                                                                                                             PowerType:  PowerTypes.AC_3_PHASE,
                                                                                                             Voltage:    Volt.  ParseV(230),
                                                                                                             Amperage:   Ampere.ParseA(32)
                                                                                                         )
                                                                                                     ]
                                                                                   )
                                                                               ]
                                                           ),
                                      Currency:            OCPI.Currency.EUR,
                                      ChargingPeriods:     [
                                                               new ChargingPeriod(
                                                                   chargingStart,
                                                                   [ CDRDimension.ENERGY(1) ]
                                                               )
                                                           ],
                                      TotalCost:           0,
                                      TotalEnergy:         WattHour.Zero,
                                      TotalTime:           TimeSpan.Zero
                                  );

            #endregion

            #region Tariff

            var tariff          = new Tariff(
                                      CountryCode:         CountryCode.Parse("DE"),
                                      PartyId:             Party_Id.   Parse("GEF"),
                                      Id:                  Tariff_Id.  Parse("tariff1"),
                                      Currency:            OCPI.Currency.EUR,
                                      TariffElements:      [
                                                               new TariffElement(
                                                                   [
                                                                       PriceComponent.Energy(
                                                                           Price:     0.44m,
                                                                           StepSize:  1000
                                                                       ),
                                                                       PriceComponent.ChargingTime(
                                                                           Price:     0m,
                                                                           Duration:  TimeSpan.FromMinutes(1)
                                                                       ),
                                                                       PriceComponent.FlatRate(
                                                                           Price:     0.3m
                                                                       )
                                                                   ],
                                                                   new TariffRestrictions(
                                                                       MaxDuration:   TimeSpan.FromHours(3)
                                                                   )
                                                               ),
                                                               new TariffElement(
                                                                   PriceComponent.Energy(
                                                                       Price:     0.44m,
                                                                       StepSize:  1000
                                                                   ),
                                                                   PriceComponent.ChargingTime(
                                                                       Price:     5.04m,
                                                                       Duration:  TimeSpan.FromMinutes(1)
                                                                   )
                                                               )
                                                           ]
                                  );

            #endregion


            var cdrWithTariff   = cdr.SplittIntoChargingPeriods(meteringValues, [tariff]);


            Assert.That(cdrWithTariff.ChargingPeriods.Count(), Is.EqualTo(4));

            // Waiting for start of charging
            var cp1 = cdrWithTariff.ChargingPeriods.ElementAt(0);

            Assert.That(cp1.StartTimestamp,                        Is.EqualTo(sessionStart));
            Assert.That(cp1.StopTimestamp,                         Is.EqualTo(chargingStart));
            Assert.That(cp1.StartMeteringValue,                    Is.Null);
            Assert.That(cp1.StopMeteringValue,                     Is.EqualTo(MeteringValue.Measured(chargingStart, WattHour.ParseKWh(0m))));

            Assert.That(cp1.Energy,                                Is.EqualTo(WattHour.ParseKWh(0)));
            Assert.That(cp1.EnergyPrice,                           Is.EqualTo(0));
            Assert.That(cp1.EnergyStepSize,                        Is.EqualTo(0));
            Assert.That(cp1.PowerAverage,                          Is.EqualTo(Watt.    ParseKW (0)));

            Assert.That(cp1.Duration,                              Is.EqualTo(chargingStart - sessionStart));
            Assert.That(cp1.TotalDuration,                         Is.EqualTo(chargingStart - sessionStart));
            Assert.That(cp1.TimePrice,                             Is.EqualTo(0));
            Assert.That(cp1.TimeStepSize,                          Is.EqualTo(0));

            Assert.That(cp1.Dimensions.     Count,                 Is.EqualTo(0));

            Assert.That(cp1.PriceComponents.Count,                 Is.EqualTo(3));
            Assert.That(cp1.PriceComponents.ElementAt(0).Key,      Is.EqualTo(TariffDimension.ENERGY));
            Assert.That(cp1.PriceComponents.ElementAt(1).Key,      Is.EqualTo(TariffDimension.TIME));
            Assert.That(cp1.PriceComponents.ElementAt(2).Key,      Is.EqualTo(TariffDimension.FLAT));


            // Charging
            var cp2 = cdrWithTariff.ChargingPeriods.ElementAt(1);

            Assert.That(cp2.StartTimestamp,                        Is.EqualTo(chargingStart));
            Assert.That(cp2.StopTimestamp,                         Is.EqualTo(sessionStart  + new TimeSpan(3, 00, 00)));
            Assert.That(cp2.StartMeteringValue,                    Is.EqualTo(MeteringValue.Measured(chargingStart, WattHour.ParseKWh(0m))));
            Assert.That(cp2.StopMeteringValue!.Value.Timestamp,    Is.EqualTo(chargingStart + new TimeSpan(2, 59, 25)));
            Assert.That(cp2.StopMeteringValue!.Value.WattHours,    Is.EqualTo(WattHour.ParseWh(15929.608862144420131291028447M)));

            Assert.That(cp2.Energy,                                Is.EqualTo(WattHour.ParseWh(15929.608862144420131291028447M)));
            Assert.That(cp2.EnergyPrice,                           Is.EqualTo(0.44M));
            Assert.That(cp2.EnergyStepSize,                        Is.EqualTo(1000));
            Assert.That(cp2.PowerAverage,                          Is.EqualTo(Watt.    ParseW (5327.1334792122498704672427971M)));

            Assert.That(cp2.Duration,                              Is.EqualTo(new TimeSpan(2, 59, 25)));
            Assert.That(cp2.TotalDuration,                         Is.EqualTo(new TimeSpan(3, 00, 00)));
            Assert.That(cp2.TimePrice,                             Is.EqualTo(0));
            Assert.That(cp2.TimeStepSize,                          Is.EqualTo(0));

            Assert.That(cp2.Dimensions.     Count,                 Is.EqualTo(1));
            Assert.That(cp2.Dimensions.First().Type,               Is.EqualTo(CDRDimensionType.ENERGY));
            Assert.That(cp2.Dimensions.First().Volume,             Is.EqualTo(15.929608862144420131291028447M)); // KWh

            Assert.That(cp2.PriceComponents.Count,                 Is.EqualTo(3));
            Assert.That(cp2.PriceComponents.ElementAt(0).Key,      Is.EqualTo(TariffDimension.ENERGY));
            Assert.That(cp2.PriceComponents.ElementAt(1).Key,      Is.EqualTo(TariffDimension.TIME));
            Assert.That(cp2.PriceComponents.ElementAt(2).Key,      Is.EqualTo(TariffDimension.FLAT));


            // Additional blocking fee applies
            var cp3 = cdrWithTariff.ChargingPeriods.ElementAt(2);

            Assert.That(cp3.StartTimestamp,                        Is.EqualTo(sessionStart  + new TimeSpan(3, 00, 00)));
            Assert.That(cp3.StopTimestamp,                         Is.EqualTo(chargingStop));
            Assert.That(cp3.StartMeteringValue,                    Is.EqualTo(MeteringValue.Imputed(
                                                                                  chargingStart + new TimeSpan(2, 59, 25),
                                                                                  WattHour.ParseWh(15929.608862144420131291028447M)
                                                                              )));
            Assert.That(cp3.StopMeteringValue,                     Is.EqualTo(MeteringValue.Measured(chargingStop, WattHour.ParseKWh(59.51m))));

            Assert.That(cp3.Energy,                                Is.EqualTo(WattHour.ParseWh(43580.391137855579868708971553M)));
            Assert.That(cp3.EnergyPrice,                           Is.EqualTo(0.44M));
            Assert.That(cp3.EnergyStepSize,                        Is.EqualTo(1000));
            Assert.That(cp3.PowerAverage,                          Is.EqualTo(Watt.    ParseW (5327.1334792122494881718378492M)));

            Assert.That(cp3.Duration,                              Is.EqualTo(new TimeSpan( 8, 10, 51)));
            Assert.That(cp3.TotalDuration,                         Is.EqualTo(new TimeSpan(11, 10, 51)));
            Assert.That(cp3.TimePrice,                             Is.EqualTo(5.04M));
            Assert.That(cp3.TimeStepSize,                          Is.EqualTo(60));

            Assert.That(cp3.Dimensions.     Count,                 Is.EqualTo(2));
            Assert.That(cp3.Dimensions.ElementAt(0).Type,          Is.EqualTo(CDRDimensionType.ENERGY));
            Assert.That(cp3.Dimensions.ElementAt(0).Volume,        Is.EqualTo(43.580391137855579868708971553M)); // KWh
            Assert.That(cp3.Dimensions.ElementAt(1).Type,          Is.EqualTo(CDRDimensionType.TIME));
            Assert.That(cp3.Dimensions.ElementAt(1).Volume,        Is.EqualTo( 8.18083333333334M));              // hours

            Assert.That(cp3.PriceComponents.Count,                 Is.EqualTo(2));
            Assert.That(cp3.PriceComponents.ElementAt(0).Key,      Is.EqualTo(TariffDimension.ENERGY));
            Assert.That(cp3.PriceComponents.ElementAt(1).Key,      Is.EqualTo(TariffDimension.TIME));


            // Waiting for plug out, after end of charging
            var cp4 = cdrWithTariff.ChargingPeriods.ElementAt(3);

            Assert.That(cp4.StartTimestamp,                        Is.EqualTo(chargingStop));
            Assert.That(cp4.StopTimestamp,                         Is.EqualTo(sessionStop));
            Assert.That(cp4.StartMeteringValue,                    Is.EqualTo(MeteringValue.Measured(chargingStop, WattHour.ParseKWh(59.51m))));
            Assert.That(cp4.StopMeteringValue,                     Is.Null);

            Assert.That(cp4.Energy,                                Is.EqualTo(WattHour.ParseKWh(0)));
            Assert.That(cp4.EnergyPrice,                           Is.EqualTo(0));
            Assert.That(cp4.EnergyStepSize,                        Is.EqualTo(0));
            Assert.That(cp4.PowerAverage,                          Is.EqualTo(Watt.Zero));

            Assert.That(cp4.Duration,                              Is.EqualTo(sessionStop - chargingStop));
            Assert.That(cp4.TotalDuration,                         Is.EqualTo(sessionStop - sessionStart));
            Assert.That(cp4.TimePrice,                             Is.EqualTo(5.04M));
            Assert.That(cp4.TimeStepSize,                          Is.EqualTo(60));

            Assert.That(cp4.Dimensions.     Count,                 Is.EqualTo(1));
            Assert.That(cp4.Dimensions.ElementAt(0).Type,          Is.EqualTo(CDRDimensionType.TIME));
            Assert.That(((Double) cp4.Dimensions.ElementAt(0).Volume) - (sessionStop - chargingStop).TotalHours, Is.LessThan(0.00001));

            Assert.That(cp4.PriceComponents.Count,                 Is.EqualTo(2));
            Assert.That(cp4.PriceComponents.ElementAt(0).Key,      Is.EqualTo(TariffDimension.ENERGY));
            Assert.That(cp4.PriceComponents.ElementAt(1).Key,      Is.EqualTo(TariffDimension.TIME));



            Assert.That(cdrWithTariff.TotalTime,         Is.EqualTo(sessionStop - sessionStart));
            Assert.That(cdrWithTariff.TotalTime,         Is.EqualTo(cp1.Duration + cp2.Duration + cp3.Duration + cp4.Duration));
            Assert.That(cdrWithTariff.TotalChargingTime, Is.EqualTo(               cp2.Duration + cp3.Duration));


        }

        #endregion


    }

}
