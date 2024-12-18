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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.Datastructures
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

            var cdr1 = new CDR(
                           CountryCode.Parse("DE"),
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
                           OCPI.Currency.EUR,

                           new ChargingPeriod[] {
                               ChargingPeriod.Create(
                                   DateTime.Parse("2020-04-12T18:21:49Z").ToUniversalTime(),
                                   new[] {
                                       CDRDimension.Create(
                                           CDRDimensionType.ENERGY,
                                           1.33M
                                       )
                                   },
                                   Tariff_Id.Parse("DE*GEF*T0001")
                               ),
                               ChargingPeriod.Create(
                                   DateTime.Parse("2020-04-12T18:21:50Z").ToUniversalTime(),
                                   new[] {
                                       CDRDimension.Create(
                                           CDRDimensionType.TIME,
                                           5.12M
                                       )
                                   },
                                   Tariff_Id.Parse("DE*GEF*T0002")
                               )
                           },

                           // Total costs
                           new Price(
                               10.00m,
                               [new TaxAmount("VAT", 11.60m)]
                           ),

                           // Total Energy
                           WattHour.ParseKWh(50.00M),

                           // Total time
                           TimeSpan.              FromMinutes(30),

                           Session_Id.            Parse("0815"),
                           AuthorizationReference.Parse("Auth0815"),
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
                                   OCPI.Currency.EUR,
                                   new[] {
                                       new TariffElement(
                                           new[] {
                                               PriceComponent.ChargingTime(
                                                   2.00M,
                                                   0.10M,
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
                                       )
                                   },
                                   TaxIncluded.Yes,
                                   TariffType.PROFILE_GREEN,
                                   new[] {
                                       new DisplayText(Languages.de, "Hallo Welt!"),
                                       new DisplayText(Languages.en, "Hello world!"),
                                   },
                                   URL.Parse("https://open.charging.cloud"),
                                   new PriceLimit( // Min Price
                                       1.10m,
                                       1.26m
                                   ),
                                   new PriceLimit( // Max Price
                                       2.20m,
                                       2.52m
                                   ),
                                   DateTime.Parse("2020-12-01").ToUniversalTime(), // Start timestamp
                                   DateTime.Parse("2020-12-31").ToUniversalTime(), // End timestamp
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

                           // Total Fixed Costs
                           new Price(
                               20.00m,
                               [new TaxAmount("VAT", 23.10m)]
                           ),

                           // Total Energy Cost
                           new Price(
                               20.00m,
                               [new TaxAmount("VAT", 23.10m)]
                           ),

                           // Total Time Cost
                           new Price(
                               20.00m,
                               [new TaxAmount("VAT", 23.10m)]
                           ),

                           // Total Parking Time
                           TimeSpan.FromMinutes(120),

                           // Total Parking Cost
                           new Price(
                               20.00m,
                               [new TaxAmount("VAT", 23.10m)]
                           ),

                           // Total Reservation Cost
                           new Price(
                               20.00m,
                               [new TaxAmount("VAT", 23.10m)]
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

            ClassicAssert.AreEqual("DE",                          JSON["country_code"].Value<String>());
            ClassicAssert.AreEqual("GEF",                         JSON["party_id"].    Value<String>());
            ClassicAssert.AreEqual("CDR0001",                     JSON["id"].          Value<String>());


            ClassicAssert.IsTrue(CDR.TryParse(JSON, out var cdr2, out var errorResponse));
            ClassicAssert.IsNull(errorResponse);

            ClassicAssert.AreEqual(cdr1.CountryCode,              cdr2.CountryCode);
            ClassicAssert.AreEqual(cdr1.PartyId,                  cdr2.PartyId);
            ClassicAssert.AreEqual(cdr1.Id,                       cdr2.Id);

            ClassicAssert.AreEqual(cdr1.Start.ToIso8601(),        cdr2.Start.ToIso8601());
            ClassicAssert.AreEqual(cdr1.End.  ToIso8601(),        cdr2.End.  ToIso8601());
            ClassicAssert.AreEqual(cdr1.CDRToken,                 cdr2.CDRToken);
            ClassicAssert.AreEqual(cdr1.AuthMethod,               cdr2.AuthMethod);
            ClassicAssert.AreEqual(cdr1.Location,                 cdr2.Location);
            ClassicAssert.AreEqual(cdr1.Currency,                 cdr2.Currency);
            ClassicAssert.AreEqual(cdr1.ChargingPeriods,          cdr2.ChargingPeriods);
            ClassicAssert.AreEqual(cdr1.TotalCosts,               cdr2.TotalCosts);
            ClassicAssert.AreEqual(cdr1.TotalEnergy,              cdr2.TotalEnergy);
            ClassicAssert.AreEqual(cdr1.TotalTime,                cdr2.TotalTime);

            ClassicAssert.AreEqual(cdr1.SessionId,                cdr2.SessionId);
            ClassicAssert.AreEqual(cdr1.AuthorizationReference,   cdr2.AuthorizationReference);
            ClassicAssert.AreEqual(cdr1.MeterId,                  cdr2.MeterId);
            ClassicAssert.AreEqual(cdr1.EnergyMeter,              cdr2.EnergyMeter);
            ClassicAssert.AreEqual(cdr1.TransparencySoftwares,    cdr2.TransparencySoftwares);
            ClassicAssert.AreEqual(cdr1.Tariffs,                  cdr2.Tariffs);
            ClassicAssert.AreEqual(cdr1.SignedData,               cdr2.SignedData);
            ClassicAssert.AreEqual(cdr1.TotalFixedCosts,          cdr2.TotalFixedCosts);
            ClassicAssert.AreEqual(cdr1.TotalEnergyCost,          cdr2.TotalEnergyCost);
            ClassicAssert.AreEqual(cdr1.TotalTimeCost,            cdr2.TotalTimeCost);
            ClassicAssert.AreEqual(cdr1.TotalParkingTime,         cdr2.TotalParkingTime);
            ClassicAssert.AreEqual(cdr1.TotalParkingCost,         cdr2.TotalParkingCost);
            ClassicAssert.AreEqual(cdr1.TotalReservationCost,     cdr2.TotalReservationCost);
            ClassicAssert.AreEqual(cdr1.Remark,                   cdr2.Remark);
            ClassicAssert.AreEqual(cdr1.InvoiceReferenceId,       cdr2.InvoiceReferenceId);
            ClassicAssert.AreEqual(cdr1.Credit,                   cdr2.Credit);
            ClassicAssert.AreEqual(cdr1.CreditReferenceId,        cdr2.CreditReferenceId);

            ClassicAssert.AreEqual(cdr1.LastUpdated.ToIso8601(),  cdr2.LastUpdated.ToIso8601());

        }

        #endregion


        #region CDR_DeserializeGitHub_Test01()

        /// <summary>
        /// Tries to deserialize a charge detail record example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.2.1-bugfixes/examples/cdr_example.json
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
                             ""country_code"": ""DE"",
                             ""party_id"": ""TNM"",
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

            var result = CDR.TryParse(JObject.Parse(JSON), out var parsedCDR, out var errorResponse);
            ClassicAssert.IsTrue   (result, errorResponse);
            ClassicAssert.IsNotNull(parsedCDR);
            ClassicAssert.IsNull   (errorResponse);

            ClassicAssert.AreEqual(CountryCode.Parse("BE"),                               parsedCDR.CountryCode);
            ClassicAssert.AreEqual(Party_Id.   Parse("BEC"),                              parsedCDR.PartyId);
            ClassicAssert.AreEqual(CDR_Id.Parse("12345"),                                 parsedCDR.Id);
            //ClassicAssert.AreEqual(true,                                                  parsedCDR.Publish);
            //ClassicAssert.AreEqual(CDR1.Start.    ToIso8601(),                            parsedCDR.Start.    ToIso8601());
            //ClassicAssert.AreEqual(CDR1.End.Value.ToIso8601(),                            parsedCDR.End.Value.ToIso8601());
            //ClassicAssert.AreEqual(CDR1.kWh,                                              parsedCDR.kWh);
            //ClassicAssert.AreEqual(CDR1.CDRToken,                                         parsedCDR.CDRToken);
            //ClassicAssert.AreEqual(CDR1.AuthMethod,                                       parsedCDR.AuthMethod);
            //ClassicAssert.AreEqual(CDR1.AuthorizationReference,                           parsedCDR.AuthorizationReference);
            //ClassicAssert.AreEqual(CDR1.CDRId,                                            parsedCDR.CDRId);
            //ClassicAssert.AreEqual(CDR1.EVSEUId,                                          parsedCDR.EVSEUId);
            //ClassicAssert.AreEqual(CDR1.ConnectorId,                                      parsedCDR.ConnectorId);
            //ClassicAssert.AreEqual(CDR1.MeterId,                                          parsedCDR.MeterId);
            //ClassicAssert.AreEqual(CDR1.EnergyMeter,                                      parsedCDR.EnergyMeter);
            //ClassicAssert.AreEqual(CDR1.TransparencySoftwares,                            parsedCDR.TransparencySoftwares);
            //ClassicAssert.AreEqual(CDR1.Currency,                                         parsedCDR.Currency);
            //ClassicAssert.AreEqual(CDR1.ChargingPeriods,                                  parsedCDR.ChargingPeriods);
            //ClassicAssert.AreEqual(CDR1.TotalCosts,                                       parsedCDR.TotalCosts);
            //ClassicAssert.AreEqual(CDR1.Status,                                           parsedCDR.Status);
            //ClassicAssert.AreEqual(CDR1.LastUpdated.ToIso8601(),                          parsedCDR.LastUpdated.ToIso8601());

        }

        #endregion


    }

}
