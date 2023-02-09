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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.UnitTests
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
                               new CDRToken(
                                   CountryCode.Parse("DE"),
                                   Party_Id.   Parse("GEF"),
                                   Token_Id.   Parse("1234"),
                                   TokenType. RFID,
                                   Contract_Id.Parse("C1234")
                               ),
                               AuthMethods.AUTH_REQUEST,
                               new CDRLocation(
                                   Location_Id.     Parse("LOC0001"),
                                   "Biberweg 18",
                                   "Jena",
                                   Country.Germany,
                                   GeoCoordinate.   Parse(10, 20),
                                   EVSE_UId.        Parse("DE*GEF*E*LOC0001*1"),
                                   EVSE_Id.         Parse("DE*GEF*E*LOC0001*1"),
                                   Connector_Id.    Parse("1"),
                                   ConnectorType.   IEC_62196_T2,
                                   ConnectorFormats.SOCKET,
                                   PowerTypes.      AC_3_PHASE,
                                   "Name?",
                                   "07749"
                               ),
                               Currency.EUR,

                               new ChargingPeriod[] {
                                   new ChargingPeriod(
                                       DateTime.Parse("2020-04-12T18:21:49Z").ToUniversalTime(),
                                       new CDRDimension[] {
                                           new CDRDimension(
                                               CDRDimensionType.ENERGY,
                                               1.33M
                                           )
                                       },
                                       Tariff_Id.Parse("DE*GEF*T0001")
                                   ),
                                   new ChargingPeriod(
                                       DateTime.Parse("2020-04-12T18:21:50Z").ToUniversalTime(),
                                       new CDRDimension[] {
                                           new CDRDimension(
                                               CDRDimensionType.TIME,
                                               5.12M
                                           )
                                       },
                                       Tariff_Id.Parse("DE*GEF*T0002")
                                   )
                               },

                               // Total costs
                               new Price(
                                   10.00,
                                   11.60
                               ),

                               // Total Energy
                               50.00M,

                               // Total time
                               TimeSpan.              FromMinutes(30),

                               Session_Id.            Parse("0815"),
                               AuthorizationReference.Parse("Auth0815"),
                               Meter_Id.              Parse("Meter0815"),

                               // OCPI Computer Science Extensions
                               new EnergyMeter(
                                   Meter_Id.Parse("Meter0815"),
                                   "EnergyMeter Model #1",
                                   "hw. v1.80",
                                   "fw. v1.20",
                                   "Energy Metering Services",
                                   null,
                                   null,
                                   new TransparencySoftwareStatus[] {
                                       new TransparencySoftwareStatus(
                                           new TransparencySoftware(
                                               "Chargy Transparency Software Desktop Application",
                                               "v1.00",
                                               OpenSourceLicenses.GPL3,
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
                                               OpenSourceLicenses.GPL3,
                                               "GraphDefined GmbH",
                                               URL.Parse("https://open.charging.cloud/logo.svg"),
                                               URL.Parse("https://open.charging.cloud/Chargy/howto"),
                                               URL.Parse("https://open.charging.cloud/Chargy"),
                                               URL.Parse("https://github.com/OpenChargingCloud/ChargyMobileApp")
                                           ),
                                           LegalStatus.ForInformationOnly,
                                           "no cert",
                                           "GraphDefiend",
                                           NotBefore: DateTime.Parse("2019-04-01T00:00:00.000Z").ToUniversalTime(),
                                           NotAfter:  DateTime.Parse("2030-01-01T00:00:00.000Z").ToUniversalTime()
                                       )
                                   }
                               ),
                               null,

                               new Tariff[] {
                                   new Tariff(
                                       CountryCode.Parse("DE"),
                                       Party_Id.   Parse("GEF"),
                                       Tariff_Id.  Parse("TARIFF0001"),
                                       Currency.EUR,
                                       new TariffElement[] {
                                           new TariffElement(
                                               new PriceComponent[] {
                                                   PriceComponent.ChargingTime(
                                                       TimeSpan.FromSeconds(300),
                                                       2.00M,
                                                       0.10M
                                                   )
                                               },
                                               new TariffRestrictions [] {
                                                   new TariffRestrictions(
                                                       Time.FromHourMin(08,00),       // Start time
                                                       Time.FromHourMin(18,00),       // End time
                                                       DateTime.Parse("2020-12-01"),  // Start timestamp
                                                       DateTime.Parse("2020-12-31"),  // End timestamp
                                                       1.12M,                         // MinkWh
                                                       5.67M,                         // MaxkWh
                                                       1.34M,                         // MinCurrent
                                                       8.89M,                         // MaxCurrent
                                                       1.49M,                         // MinPower
                                                       9.91M,                         // MaxPower
                                                       TimeSpan.FromMinutes(10),      // MinDuration
                                                       TimeSpan.FromMinutes(30),      // MaxDuration
                                                       new DayOfWeek[] {
                                                           DayOfWeek.Monday,
                                                           DayOfWeek.Tuesday
                                                       },
                                                       ReservationRestrictions.RESERVATION
                                                   )
                                               }
                                           )
                                       },
                                       TariffTypes.PROFILE_GREEN,
                                       new DisplayText[] {
                                           new DisplayText(Languages.de, "Hallo Welt!"),
                                           new DisplayText(Languages.en, "Hello world!"),
                                       },
                                       URL.Parse("https://open.charging.cloud"),
                                       new Price( // Min Price
                                           1.10,
                                           1.26
                                       ),
                                       new Price( // Max Price
                                           2.20,
                                           2.52
                                       ),
                                       DateTime.Parse("2020-12-01").ToUniversalTime(), // Start timestamp
                                       DateTime.Parse("2020-12-31").ToUniversalTime(), // End timestamp
                                       new EnergyMix(
                                           true,
                                           new EnergySource[] {
                                               new EnergySource(
                                                   EnergySourceCategory.SOLAR,
                                                   80
                                               ),
                                               new EnergySource(
                                                   EnergySourceCategory.WIND,
                                                   20
                                               )
                                           },
                                           new EnvironmentalImpact[] {
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
                                   EncodingMethod.GraphDefiened,
                                   new SignedValue[] {
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
                                   "https://open.charging.cloud/pools/1/stations/1/evse/1/publicKey"
                               ),

                               // Total Fixed Costs
                               new Price(
                                   20.00,
                                   23.10
                               ),

                               // Total Energy Cost
                               new Price(
                                   20.00,
                                   23.10
                               ),

                               // Total Time Cost
                               new Price(
                                   20.00,
                                   23.10
                               ),

                               // Total Parking Time
                               TimeSpan.FromMinutes(120),

                               // Total Parking Cost
                               new Price(
                                   20.00,
                                   23.10
                               ),

                               // Total Reservation Cost
                               new Price(
                                   20.00,
                                   23.10
                               ),

                               "Remark!",
                               InvoiceReference_Id.Parse("Invoice:0815"),
                               true, // IsCredit
                               CreditReference_Id.Parse("Credit:0815"),
                               false,

                               DateTime.Parse("2020-09-12").ToUniversalTime()

                           );

            #endregion

            var JSON = cdr1.ToJSON();

            Assert.AreEqual("DE",                          JSON["country_code"].Value<String>());
            Assert.AreEqual("GEF",                         JSON["party_id"].    Value<String>());
            Assert.AreEqual("CDR0001",                     JSON["id"].          Value<String>());


            Assert.IsTrue(CDR.TryParse(JSON, out var cdr2, out var errorResponse));
            Assert.IsNull(errorResponse);

            Assert.AreEqual(cdr1.CountryCode,              cdr2.CountryCode);
            Assert.AreEqual(cdr1.PartyId,                  cdr2.PartyId);
            Assert.AreEqual(cdr1.Id,                       cdr2.Id);

            Assert.AreEqual(cdr1.Start.ToIso8601(),        cdr2.Start.ToIso8601());
            Assert.AreEqual(cdr1.End.  ToIso8601(),        cdr2.End.  ToIso8601());
            Assert.AreEqual(cdr1.CDRToken,                 cdr2.CDRToken);
            Assert.AreEqual(cdr1.AuthMethod,               cdr2.AuthMethod);
            Assert.AreEqual(cdr1.Location,                 cdr2.Location);
            Assert.AreEqual(cdr1.Currency,                 cdr2.Currency);
            Assert.AreEqual(cdr1.ChargingPeriods,          cdr2.ChargingPeriods);
            Assert.AreEqual(cdr1.TotalCosts,               cdr2.TotalCosts);
            Assert.AreEqual(cdr1.TotalEnergy,              cdr2.TotalEnergy);
            Assert.AreEqual(cdr1.TotalTime,                cdr2.TotalTime);

            Assert.AreEqual(cdr1.SessionId,                cdr2.SessionId);
            Assert.AreEqual(cdr1.AuthorizationReference,   cdr2.AuthorizationReference);
            Assert.AreEqual(cdr1.MeterId,                  cdr2.MeterId);
            Assert.AreEqual(cdr1.EnergyMeter,              cdr2.EnergyMeter);
            Assert.AreEqual(cdr1.TransparencySoftwares,    cdr2.TransparencySoftwares);
            Assert.AreEqual(cdr1.Tariffs,                  cdr2.Tariffs);
            Assert.AreEqual(cdr1.SignedData,               cdr2.SignedData);
            Assert.AreEqual(cdr1.TotalFixedCosts,          cdr2.TotalFixedCosts);
            Assert.AreEqual(cdr1.TotalEnergyCost,          cdr2.TotalEnergyCost);
            Assert.AreEqual(cdr1.TotalTimeCost,            cdr2.TotalTimeCost);
            Assert.AreEqual(cdr1.TotalParkingTime,         cdr2.TotalParkingTime);
            Assert.AreEqual(cdr1.TotalParkingCost,         cdr2.TotalParkingCost);
            Assert.AreEqual(cdr1.TotalReservationCost,     cdr2.TotalReservationCost);
            Assert.AreEqual(cdr1.Remark,                   cdr2.Remark);
            Assert.AreEqual(cdr1.InvoiceReferenceId,       cdr2.InvoiceReferenceId);
            Assert.AreEqual(cdr1.Credit,                   cdr2.Credit);
            Assert.AreEqual(cdr1.CreditReferenceId,        cdr2.CreditReferenceId);

            Assert.AreEqual(cdr1.LastUpdated.ToIso8601(),  cdr2.LastUpdated.ToIso8601());

        }

        #endregion


        #region CDR_DeserializeGitHub_Test01()

        /// <summary>
        /// Tries to deserialize a charge detail record example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/examples/cdr_example.json
        /// </summary>
        [Test]
        public static void CDR_DeserializeGitHub_Test01()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"": ""BE"",
                           ""party_id"": ""BEC"",
                           ""id"": ""12345"",
                           ""start_date_time"": ""2015-06-29T21:39:09Z"",
                           ""end_date_time"": ""2015-06-29T23:37:32Z"",
                           ""cdr_token"": {
                             ""uid"": ""012345678"",
                             ""type"": ""RFID"",
                             ""contract_id"": ""DE8ACC12E46L89""
                           },
                           ""auth_method"": ""WHITELIST"",
                           ""cdr_location"": {
                             ""id"": ""LOC1"",
                             ""name"": ""Gent Zuid"",
                             ""address"": ""F.Rooseveltlaan 3A"",
                             ""city"": ""Gent"",
                             ""postal_code"": ""9000"",
                             ""country"": ""BEL"",
                             ""coordinates"": {
                               ""latitude"": ""3.729944"",
                               ""longitude"": ""51.047599""
                             },
                             ""evse_uid"": ""3256"",
                             ""evse_id"": ""BE*BEC*E041503003"",
                             ""connector_id"": ""1"",
                             ""connector_standard"": ""IEC_62196_T2"",
                             ""connector_format"": ""SOCKET"",
                             ""connector_power_type"": ""AC_1_PHASE""
                           },
                           ""currency"": ""EUR"",
                           ""tariffs"": [{
                             ""country_code"": ""BE"",
                             ""party_id"": ""BEC"",
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
                           ""total_cost"": {
                             ""excl_vat"": 4.00,
                             ""incl_vat"": 4.40
                           },
                           ""total_energy"": 15.342,
                           ""total_time"": 1.973,
                           ""total_time_cost"": {
                             ""excl_vat"": 4.00,
                             ""incl_vat"": 4.40
                           },
                           ""last_updated"": ""2015-06-29T22:01:13Z""
                         }";

            #endregion

            Assert.IsTrue(CDR.TryParse(JObject.Parse(JSON), out var parsedCDR, out var errorResponse));
            Assert.IsNull(errorResponse);

            Assert.AreEqual(CountryCode.Parse("BE"),                               parsedCDR.CountryCode);
            Assert.AreEqual(Party_Id.   Parse("BEC"),                              parsedCDR.PartyId);
            Assert.AreEqual(CDR_Id.Parse("12345"),                                 parsedCDR.Id);
            //Assert.AreEqual(true,                                                  parsedCDR.Publish);
            //Assert.AreEqual(CDR1.Start.    ToIso8601(),                            parsedCDR.Start.    ToIso8601());
            //Assert.AreEqual(CDR1.End.Value.ToIso8601(),                            parsedCDR.End.Value.ToIso8601());
            //Assert.AreEqual(CDR1.kWh,                                              parsedCDR.kWh);
            //Assert.AreEqual(CDR1.CDRToken,                                         parsedCDR.CDRToken);
            //Assert.AreEqual(CDR1.AuthMethod,                                       parsedCDR.AuthMethod);
            //Assert.AreEqual(CDR1.AuthorizationReference,                           parsedCDR.AuthorizationReference);
            //Assert.AreEqual(CDR1.CDRId,                                            parsedCDR.CDRId);
            //Assert.AreEqual(CDR1.EVSEUId,                                          parsedCDR.EVSEUId);
            //Assert.AreEqual(CDR1.ConnectorId,                                      parsedCDR.ConnectorId);
            //Assert.AreEqual(CDR1.MeterId,                                          parsedCDR.MeterId);
            //Assert.AreEqual(CDR1.EnergyMeter,                                      parsedCDR.EnergyMeter);
            //Assert.AreEqual(CDR1.TransparencySoftwares,                            parsedCDR.TransparencySoftwares);
            //Assert.AreEqual(CDR1.Currency,                                         parsedCDR.Currency);
            //Assert.AreEqual(CDR1.ChargingPeriods,                                  parsedCDR.ChargingPeriods);
            //Assert.AreEqual(CDR1.TotalCosts,                                       parsedCDR.TotalCosts);
            //Assert.AreEqual(CDR1.Status,                                           parsedCDR.Status);
            //Assert.AreEqual(CDR1.LastUpdated.ToIso8601(),                          parsedCDR.LastUpdated.ToIso8601());

        }

        #endregion


    }

}
