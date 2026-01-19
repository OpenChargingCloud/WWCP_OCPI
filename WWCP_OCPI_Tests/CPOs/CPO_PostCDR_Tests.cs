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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI.UnitTests;

#endregion

namespace cloud.charging.open.protocols.OCPI.CPO.UnitTests
{

    [TestFixture]
    public class CPO_PostCDR_Tests()

        : A_2CPOs2EMSPs_TestDefaults()

    {

        #region CPO1_PostCDR_OCPIv2_1_1_Test()

        /// <summary>
        /// CPO1_PostCDR_OCPIv2_1_1_Test.
        /// </summary>
        [Test]
        public async Task CPO1_PostCDR_OCPIv2_1_1_Test()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_1_1?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.PostCDR(
                                         new OCPIv2_1_1.CDR(
                                             CountryCode.Parse("DE"),
                                             Party_Id.   Parse("GEF"),
                                             CDR_Id.     Parse("CDR0001"),
                                             DateTime.   Parse("2020-04-12T18:20:19Z").ToUniversalTime(),
                                             DateTime.   Parse("2020-04-12T22:20:19Z").ToUniversalTime(),
                                             Auth_Id.    Parse("1234"),
                                             OCPIv2_1_1.AuthMethods.AUTH_REQUEST,
                                             new OCPIv2_1_1.Location(
                                                 CountryCode.Parse("DE"),
                                                 Party_Id.   Parse("GEF"),
                                                 Location_Id.Parse("LOC0001"),
                                                 OCPIv2_1_1.LocationType.UNDERGROUND_GARAGE,
                                                 "Biberweg 18",
                                                 "Jena",
                                                 "07749",
                                                 Country.Germany,
                                                 GeoCoordinate.Parse(10, 20)
                                             ),
                                             Currency.EUR,

                                             [
                                                 new OCPIv2_1_1.ChargingPeriod(
                                                     DateTime.Parse("2020-04-12T18:21:49Z"),
                                                     [
                                                         OCPIv2_1_1.CDRDimension.Create(
                                                             OCPIv2_1_1.CDRDimensionType.ENERGY,
                                                             1.33M
                                                         )
                                                     ]
                                                 ),
                                                 new OCPIv2_1_1.ChargingPeriod(
                                                     DateTime.Parse("2020-04-12T18:21:50Z"),
                                                     [
                                                         OCPIv2_1_1.CDRDimension.Create(
                                                             OCPIv2_1_1.CDRDimensionType.TIME,
                                                             5.12M
                                                         )
                                                     ]
                                                 )
                                             ],

                                             // Total cost
                                             10.00M,

                                             // Total Energy
                                             WattHour.ParseKWh(50.00M),

                                             // Total time
                                             TimeSpan.FromMinutes(30),

                                             null,   // Costs
                                             EnergyMeter_Id.Parse("Meter0815"),

                                             // OCPI Computer Science Extensions
                                             new EnergyMeter<OCPIv2_1_1.EVSE>(
                                                 EnergyMeter_Id.Parse("Meter0815"),
                                                 "EnergyMeter Model #1",
                                                 null,
                                                 "hw. v1.80",
                                                 "fw. v1.20",
                                                 "Energy Metering Services",
                                                 null,
                                                 null,
                                                 null,
                                                 [
                                                     new TransparencySoftwareStatus(
                                                         new TransparencySoftware(
                                                             "Chargy Transparency Software Desktop Application",
                                                             "v1.00",
                                                             SoftwareLicense.AGPL3,
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
                                                             SoftwareLicense.AGPL3,
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
                                                 ]
                                             ),
                                             null,

                                             [
                                                 new OCPIv2_1_1.Tariff(
                                                     CountryCode.Parse("DE"),
                                                     Party_Id.   Parse("GEF"),
                                                     Tariff_Id.  Parse("TARIFF0001"),
                                                     Currency.EUR,
                                                     [
                                                         new OCPIv2_1_1.TariffElement(
                                                             [
                                                                 OCPIv2_1_1.PriceComponent.ChargingTime(
                                                                     2.00M,
                                                                     TimeSpan.FromSeconds(300)
                                                                 )
                                                             ],
                                                             new OCPIv2_1_1.TariffRestrictions(
                                                                 OCPIv2_1_1.Time.FromHourMin(08,00),       // Start time
                                                                 OCPIv2_1_1.Time.FromHourMin(18,00),       // End time
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
                                                         new DisplayText(Languages.en, "Hello world!")
                                                     ],
                                                     URL.Parse("https://open.charging.cloud"),
                                                     new OCPIv2_1_1.EnergyMix(
                                                         true,
                                                         [
                                                             new OCPIv2_1_1.EnergySource(
                                                                 OCPIv2_1_1.EnergySourceCategory.SOLAR,
                                                                 80
                                                             ),
                                                             new OCPIv2_1_1.EnergySource(
                                                                 OCPIv2_1_1.EnergySourceCategory.WIND,
                                                                 20
                                                             )
                                                         ],
                                                         [
                                                             new OCPIv2_1_1.EnvironmentalImpact(
                                                                 OCPIv2_1_1.EnvironmentalImpactCategory.CARBON_DIOXIDE,
                                                                 0.1
                                                             )
                                                         ],
                                                         "Stadtwerke Jena-Ost",
                                                         "New Green Deal"
                                                     ),
                                                     DateTime.Parse("2020-09-22").ToUniversalTime()
                                                 )
                                             ],

                                             new OCPIv2_1_1.SignedData(
                                                 OCPIv2_1_1.EncodingMethod.GraphDefined,
                                                 [
                                                     new OCPIv2_1_1.SignedValue(
                                                         OCPIv2_1_1.SignedValueNature.START,
                                                         "PlainStartValue",
                                                         "SignedStartValue"
                                                     ),
                                                     new OCPIv2_1_1.SignedValue(
                                                         OCPIv2_1_1.SignedValueNature.INTERMEDIATE,
                                                         "PlainIntermediateValue",
                                                         "SignedIntermediateValue"
                                                     ),
                                                     new OCPIv2_1_1.SignedValue(
                                                         OCPIv2_1_1.SignedValueNature.END,
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

                                             LastUpdated: DateTime.Parse("2020-09-12").ToUniversalTime()

                                         )
                                     );

                // HTTP/1.1 201 Created
                // Date:                          Wed, 28 Dec 2022 23:20:07 GMT
                // Location:                      /2.2/emsp/cdrs/CDR0001
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Last-Modified:                 2020-09-11T22:00:00.000Z
                // ETag:                          ckAWY1kW3wi8MR04zps+WfXAFDVTrsYlHSnRn0nPJU8=
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                3436
                // X-Request-ID:                  dt88p12Mn313hQxf3t5UKpCjMKY37d
                // X-Correlation-ID:              2284ht8G82pE4Sxpn4KUj2t5tAS13j
                // 
                // {"data":{"id":"CDR0001","start_date_time":"2020-04-12T18:20:19.000Z","end_date_time":"2020-04-12T22:20:19.000Z","auth_id":"1234","auth_method":"AUTH_REQUEST","location":{"id":"LOC0001","location_type":"UNDERGROUND_GARAGE","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"time_zone":null,"last_updated":"2022-12-28T23:19:49.740Z"},"meter_id":"Meter0815","energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","vendor":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}]},"signed_data":{"encoding_method":"GraphDefined","encoding_method_version":"1","signed_values":[{"nature":"START","plain_data":"PlainStartValue","signed_data":"SignedStartValue"},{"nature":"INTERMEDIATE","plain_data":"PlainIntermediateValue","signed_data":"SignedIntermediateValue"},{"nature":"END","plain_data":"PlainEndValue","signed_data":"SignedEndValue"}],"url":"https://open.charging.cloud/pools/1/stations/1/evse/1/publicKey"},"currency":"EUR","tariffs":[{"id":"TARIFF0001","currency":"EUR","tariff_alt_text":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"tariff_alt_url":"https://open.charging.cloud","elements":[{"price_components":[{"type":"TIME","price":2.0,"step_size":300}],"restrictions":[{"start_time":"08:00","end_time":"18:00","start_date":"2020-12-01","end_date":"2020-12-31","min_kwh":1.12,"max_kwh":5.67,"min_power":1.49,"max_power":9.91,"min_duration":600.0,"max_duration":1800.0,"day_of_week":["MONDAY","TUESDAY"]}]}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T22:00:00.000Z"}],"charging_periods":[{"start_date_time":"2020-04-12T18:21:49.000Z","dimensions":[{"type":"ENERGY","volume":1.33}]},{"start_date_time":"2020-04-12T18:21:50.000Z","dimensions":[{"type":"TIME","volume":5.12}]}],"total_cost":10.0,"total_energy":50.0,"total_time":0.5,"total_parking_time":2.0,"remark":"Remark!","last_updated":"2020-09-11T22:00:00.000Z"},"status_code":1000,"status_message":"Hello world!","timestamp":"2022-12-28T23:20:07.358Z"}

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(201),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                //if (response.Data is not null)
                //{
                //}

            }

        }

        #endregion

        #region CPO1_PostCDR_OCPIv2_2_1_Test()

        /// <summary>
        /// CPO1_PostCDR_OCPIv2_2_1_Test.
        /// </summary>
        [Test]
        public async Task CPO1_PostCDR_OCPIv2_2_1_Test()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_2_1?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.PostCDR(
                                         new OCPIv2_2_1.CDR(

                                             CountryCode.Parse("DE"),
                                             Party_Id.   Parse("GEF"),
                                             CDR_Id.     Parse("CDR0001"),
                                             DateTime.   Parse("2020-04-12T18:20:19Z").ToUniversalTime(),
                                             DateTime.   Parse("2020-04-12T22:20:19Z").ToUniversalTime(),
                                             new OCPIv2_2_1.CDRToken(
                                                 CountryCode.Parse("DE"),
                                                 Party_Id.   Parse("GEF"),
                                                 Token_Id.   Parse("1234"),
                                                 TokenType. RFID,
                                                 Contract_Id.Parse("C1234")
                                             ),
                                             OCPIv2_2_1.AuthMethod.AUTH_REQUEST,
                                             new OCPIv2_2_1.CDRLocation(
                                                 Location_Id.     Parse("LOC0001"),
                                                 "Biberweg 18",
                                                 "Jena",
                                                 Country.Germany,
                                                 GeoCoordinate.   Parse(10, 20),
                                                 EVSE_UId.        Parse("DE*GEF*E*LOC0001*1"),
                                                 EVSE_Id.         Parse("DE*GEF*E*LOC0001*1"),
                                                 Connector_Id.    Parse("1"),
                                                 OCPIv2_2_1.ConnectorType.   IEC_62196_T2,
                                                 OCPIv2_2_1.ConnectorFormats.SOCKET,
                                                 OCPIv2_2_1.PowerTypes.      AC_3_PHASE,
                                                 "Name?",
                                                 "07749"
                                             ),
                                             Currency.EUR,

                                             [
                                                 OCPIv2_2_1.ChargingPeriod.Create(
                                                     DateTime.Parse("2020-04-12T18:21:49Z").ToUniversalTime(),
                                                     [
                                                         OCPIv2_2_1.CDRDimension.Create(
                                                             OCPIv2_2_1.CDRDimensionType.ENERGY,
                                                             1.33M
                                                         )
                                                     ],
                                                     Tariff_Id.Parse("DE*GEF*T0001")
                                                 ),
                                                 OCPIv2_2_1.ChargingPeriod.Create(
                                                     DateTime.Parse("2020-04-12T18:21:50Z").ToUniversalTime(),
                                                     [
                                                         OCPIv2_2_1.CDRDimension.Create(
                                                             OCPIv2_2_1.CDRDimensionType.TIME,
                                                             5.12M
                                                         )
                                                     ],
                                                     Tariff_Id.Parse("DE*GEF*T0002")
                                                 )
                                             ],

                                             // Total costs
                                             new OCPIv2_2_1.Price(
                                                 10.00,
                                                 11.60
                                             ),

                                             // Total Energy
                                             WattHour.ParseKWh(50.00M),

                                             // Total time
                                             TimeSpan.              FromMinutes(30),

                                             Session_Id.            Parse("0815"),
                                             OCPIv2_2_1.AuthorizationReference.Parse("Auth0815"),
                                             EnergyMeter_Id.              Parse("Meter0815"),

                                             // OCPI Computer Science Extensions
                                             new EnergyMeter<OCPIv2_2_1.EVSE>(
                                                 EnergyMeter_Id.Parse("Meter0815"),
                                                 "EnergyMeter Model #1",
                                                 null,
                                                 "hw. v1.80",
                                                 "fw. v1.20",
                                                 "Energy Metering Services",
                                                 null,
                                                 null,
                                                 null,
                                                 [
                                                     new TransparencySoftwareStatus(
                                                         new TransparencySoftware(
                                                             "Chargy Transparency Software Desktop Application",
                                                             "v1.00",
                                                             SoftwareLicense.AGPL3,
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
                                                             SoftwareLicense.AGPL3,
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
                                                 ]
                                             ),
                                             null,

                                             [
                                                 new OCPIv2_2_1.Tariff(
                                                     CountryCode.Parse("DE"),
                                                     Party_Id.   Parse("GEF"),
                                                     Tariff_Id.  Parse("TARIFF0001"),
                                                     Currency.EUR,
                                                     [
                                                         new OCPIv2_2_1.TariffElement(
                                                             [
                                                                 OCPIv2_2_1.PriceComponent.ChargingTime(
                                                                     2.00M,
                                                                     0.10M,
                                                                     TimeSpan.FromSeconds(300)
                                                                 )
                                                             ],
                                                             new OCPIv2_2_1.TariffRestrictions(
                                                                 OCPIv2_2_1.Time.FromHourMin(08,00),       // Start time
                                                                 OCPIv2_2_1.Time.FromHourMin(18,00),       // End time
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
                                                                 [
                                                                     DayOfWeek.Monday,
                                                                     DayOfWeek.Tuesday
                                                                 ],
                                                                 OCPIv2_2_1.ReservationRestrictions.RESERVATION
                                                             )
                                                         )
                                                     ],
                                                     OCPIv2_2_1.TariffType.PROFILE_GREEN,
                                                     [
                                                         new DisplayText(Languages.de, "Hallo Welt!"),
                                                         new DisplayText(Languages.en, "Hello world!"),
                                                     ],
                                                     URL.Parse("https://open.charging.cloud"),
                                                     new OCPIv2_2_1.Price( // Min Price
                                                         1.10,
                                                         1.26
                                                     ),
                                                     new OCPIv2_2_1.Price( // Max Price
                                                         2.20,
                                                         2.52
                                                     ),
                                                     DateTime.Parse("2020-12-01").ToUniversalTime(), // Start timestamp
                                                     DateTime.Parse("2020-12-31").ToUniversalTime(), // End timestamp
                                                     new OCPIv2_2_1.EnergyMix(
                                                         true,
                                                         [
                                                             new OCPIv2_2_1.EnergySource(
                                                                 OCPIv2_2_1.EnergySourceCategory.SOLAR,
                                                                 80
                                                             ),
                                                             new OCPIv2_2_1.EnergySource(
                                                                 OCPIv2_2_1.EnergySourceCategory.WIND,
                                                                 20
                                                             )
                                                         ],
                                                         [
                                                             new OCPIv2_2_1.EnvironmentalImpact(
                                                                 OCPIv2_2_1.EnvironmentalImpactCategory.CARBON_DIOXIDE,
                                                                 0.1
                                                             )
                                                         ],
                                                         "Stadtwerke Jena-Ost",
                                                         "New Green Deal"
                                                     ),
                                                     DateTime.Parse("2020-09-22").ToUniversalTime()
                                                 )
                                             ],

                                             new OCPIv2_2_1.SignedData(
                                                 OCPIv2_2_1.EncodingMethod.GraphDefined,
                                                 [
                                                     new OCPIv2_2_1.SignedValue(
                                                         OCPIv2_2_1.SignedValueNature.START,
                                                         "PlainStartValue",
                                                         "SignedStartValue"
                                                     ),
                                                     new OCPIv2_2_1.SignedValue(
                                                         OCPIv2_2_1.SignedValueNature.INTERMEDIATE,
                                                         "PlainIntermediateValue",
                                                         "SignedIntermediateValue"
                                                     ),
                                                     new OCPIv2_2_1.SignedValue(
                                                         OCPIv2_2_1.SignedValueNature.END,
                                                         "PlainEndValue",
                                                         "SignedEndValue"
                                                     )
                                                 ],
                                                 1,     // Encoding method version
                                                 null,  // Public key
                                                 URL.Parse("https://open.charging.cloud/pools/1/stations/1/evse/1/publicKey")
                                             ),

                                             // Total Fixed Costs
                                             new OCPIv2_2_1.Price(
                                                 20.00,
                                                 23.10
                                             ),

                                             // Total Energy Cost
                                             new OCPIv2_2_1.Price(
                                                 20.00,
                                                 23.10
                                             ),

                                             // Total Time Cost
                                             new OCPIv2_2_1.Price(
                                                 20.00,
                                                 23.10
                                             ),

                                             // Total Parking Time
                                             TimeSpan.FromMinutes(120),

                                             // Total Parking Cost
                                             new OCPIv2_2_1.Price(
                                                 20.00,
                                                 23.10
                                             ),

                                             // Total Reservation Cost
                                             new OCPIv2_2_1.Price(
                                                 20.00,
                                                 23.10
                                             ),

                                             "Remark!",
                                             OCPIv2_2_1.InvoiceReference_Id.Parse("Invoice:0815"),
                                             true, // IsCredit
                                             OCPIv2_2_1.CreditReference_Id. Parse("Credit:0815"),
                                             false,

                                             DateTimeOffset.Parse("2026-01-17")

                                         )
                                     );

                // HTTP/1.1 201 Created
                // Date:                          Wed, 28 Dec 2022 23:20:07 GMT
                // Location:                      /2.2/emsp/cdrs/CDR0001
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Last-Modified:                 2020-09-11T22:00:00.000Z
                // ETag:                          ckAWY1kW3wi8MR04zps+WfXAFDVTrsYlHSnRn0nPJU8=
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                3436
                // X-Request-ID:                  dt88p12Mn313hQxf3t5UKpCjMKY37d
                // X-Correlation-ID:              2284ht8G82pE4Sxpn4KUj2t5tAS13j
                // 
                // {"data":{"id":"CDR0001","start_date_time":"2020-04-12T18:20:19.000Z","end_date_time":"2020-04-12T22:20:19.000Z","auth_id":"1234","auth_method":"AUTH_REQUEST","location":{"id":"LOC0001","location_type":"UNDERGROUND_GARAGE","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"time_zone":null,"last_updated":"2022-12-28T23:19:49.740Z"},"meter_id":"Meter0815","energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","vendor":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}]},"signed_data":{"encoding_method":"GraphDefined","encoding_method_version":"1","signed_values":[{"nature":"START","plain_data":"PlainStartValue","signed_data":"SignedStartValue"},{"nature":"INTERMEDIATE","plain_data":"PlainIntermediateValue","signed_data":"SignedIntermediateValue"},{"nature":"END","plain_data":"PlainEndValue","signed_data":"SignedEndValue"}],"url":"https://open.charging.cloud/pools/1/stations/1/evse/1/publicKey"},"currency":"EUR","tariffs":[{"id":"TARIFF0001","currency":"EUR","tariff_alt_text":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"tariff_alt_url":"https://open.charging.cloud","elements":[{"price_components":[{"type":"TIME","price":2.0,"step_size":300}],"restrictions":[{"start_time":"08:00","end_time":"18:00","start_date":"2020-12-01","end_date":"2020-12-31","min_kwh":1.12,"max_kwh":5.67,"min_power":1.49,"max_power":9.91,"min_duration":600.0,"max_duration":1800.0,"day_of_week":["MONDAY","TUESDAY"]}]}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T22:00:00.000Z"}],"charging_periods":[{"start_date_time":"2020-04-12T18:21:49.000Z","dimensions":[{"type":"ENERGY","volume":1.33}]},{"start_date_time":"2020-04-12T18:21:50.000Z","dimensions":[{"type":"TIME","volume":5.12}]}],"total_cost":10.0,"total_energy":50.0,"total_time":0.5,"total_parking_time":2.0,"remark":"Remark!","last_updated":"2020-09-11T22:00:00.000Z"},"status_code":1000,"status_message":"Hello world!","timestamp":"2022-12-28T23:20:07.358Z"}

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(201),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {
                }

            }

        }

        #endregion

        #region CPO1_PostCDR_OCPIv2_3_0_Test()

        /// <summary>
        /// CPO1_PostCDR_OCPIv2_3_0_Test.
        /// </summary>
        [Test]
        public async Task CPO1_PostCDR_OCPIv2_3_0_Test()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_3_0?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.PostCDR(
                                         new OCPIv2_3_0.CDR(

                                             CountryCode.Parse("DE"),
                                             Party_Id.   Parse("GEF"),
                                             CDR_Id.     Parse("CDR0001"),
                                             DateTime.   Parse("2020-04-12T18:20:19Z").ToUniversalTime(),
                                             DateTime.   Parse("2020-04-12T22:20:19Z").ToUniversalTime(),
                                             new OCPIv2_3_0.CDRToken(
                                                 CountryCode.Parse("DE"),
                                                 Party_Id.   Parse("GEF"),
                                                 Token_Id.   Parse("1234"),
                                                 TokenType. RFID,
                                                 Contract_Id.Parse("C1234")
                                             ),
                                             OCPIv2_3_0.AuthMethod.AUTH_REQUEST,
                                             new OCPIv2_3_0.CDRLocation(
                                                 Location_Id.     Parse("LOC0001"),
                                                 "Biberweg 18",
                                                 "Jena",
                                                 Country.Germany,
                                                 GeoCoordinate.   Parse(10, 20),
                                                 EVSE_UId.        Parse("DE*GEF*E*LOC0001*1"),
                                                 EVSE_Id.         Parse("DE*GEF*E*LOC0001*1"),
                                                 Connector_Id.    Parse("1"),
                                                 OCPIv2_3_0.ConnectorType.   IEC_62196_T2,
                                                 OCPIv2_3_0.ConnectorFormats.SOCKET,
                                                 OCPIv2_3_0.PowerTypes.      AC_3_PHASE,
                                                 "Name?",
                                                 "07749"
                                             ),
                                             Currency.EUR,

                                             [
                                                 OCPIv2_3_0.ChargingPeriod.Create(
                                                     DateTime.Parse("2020-04-12T18:21:49Z").ToUniversalTime(),
                                                     [
                                                         OCPIv2_3_0.CDRDimension.Create(
                                                             OCPIv2_3_0.CDRDimensionType.ENERGY,
                                                             1.33M
                                                         )
                                                     ],
                                                     Tariff_Id.Parse("DE*GEF*T0001")
                                                 ),
                                                 OCPIv2_3_0.ChargingPeriod.Create(
                                                     DateTime.Parse("2020-04-12T18:21:50Z").ToUniversalTime(),
                                                     [
                                                         OCPIv2_3_0.CDRDimension.Create(
                                                             OCPIv2_3_0.CDRDimensionType.TIME,
                                                             5.12M
                                                         )
                                                     ],
                                                     Tariff_Id.Parse("DE*GEF*T0002")
                                                 )
                                             ],

                                             // Total costs
                                             new OCPIv2_3_0.Price(
                                                 10.00m,
                                                 [ new OCPIv2_3_0.TaxAmount("VAT", 11.60m) ]
                                             ),

                                             // Total Energy
                                             WattHour.ParseKWh(50.00M),

                                             // Total time
                                             TimeSpan.              FromMinutes(30),

                                             Session_Id.            Parse("0815"),
                                             OCPIv2_3_0.AuthorizationReference.Parse("Auth0815"),
                                             EnergyMeter_Id.              Parse("Meter0815"),

                                             // OCPI Computer Science Extensions
                                             new EnergyMeter<OCPIv2_3_0.EVSE>(
                                                 EnergyMeter_Id.Parse("Meter0815"),
                                                 "EnergyMeter Model #1",
                                                 null,
                                                 "hw. v1.80",
                                                 "fw. v1.20",
                                                 "Energy Metering Services",
                                                 null,
                                                 null,
                                                 null,
                                                 [
                                                     new TransparencySoftwareStatus(
                                                         new TransparencySoftware(
                                                             "Chargy Transparency Software Desktop Application",
                                                             "v1.00",
                                                             SoftwareLicense.AGPL3,
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
                                                             SoftwareLicense.AGPL3,
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
                                                 ]
                                             ),
                                             null,

                                             [
                                                 new OCPIv2_3_0.Tariff(
                                                     CountryCode.Parse("DE"),
                                                     Party_Id.   Parse("GEF"),
                                                     Tariff_Id.  Parse("TARIFF0001"),
                                                     Currency.EUR,
                                                     [
                                                         new OCPIv2_3_0.TariffElement(
                                                             [
                                                                 OCPIv2_3_0.PriceComponent.ChargingTime(
                                                                     2.00M,
                                                                     0.10M,
                                                                     TimeSpan.FromSeconds(300)
                                                                 )
                                                             ],
                                                             new OCPIv2_3_0.TariffRestrictions(
                                                                 OCPIv2_3_0.Time.FromHourMin(08,00),       // Start time
                                                                 OCPIv2_3_0.Time.FromHourMin(18,00),       // End time
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
                                                                 [
                                                                     DayOfWeek.Monday,
                                                                     DayOfWeek.Tuesday
                                                                 ],
                                                                 OCPIv2_3_0.ReservationRestrictions.RESERVATION
                                                             )
                                                         )
                                                     ],
                                                     OCPIv2_3_0.TaxIncluded.Yes,
                                                     OCPIv2_3_0.TariffType.PROFILE_GREEN,
                                                     [
                                                         new DisplayText(Languages.de, "Hallo Welt!"),
                                                         new DisplayText(Languages.en, "Hello world!"),
                                                     ],
                                                     URL.Parse("https://open.charging.cloud"),
                                                     new OCPIv2_3_0.PriceLimit( // Min Price
                                                         1.10m,
                                                         1.26m
                                                     ),
                                                     new OCPIv2_3_0.PriceLimit( // Max Price
                                                         2.20m,
                                                         2.52m
                                                     ),
                                                     DateTime.Parse("2020-12-01").ToUniversalTime(), // Start timestamp
                                                     DateTime.Parse("2020-12-31").ToUniversalTime(), // End timestamp
                                                     new OCPIv2_3_0.EnergyMix(
                                                         true,
                                                         [
                                                             new OCPIv2_3_0.EnergySource(
                                                                 OCPIv2_3_0.EnergySourceCategory.SOLAR,
                                                                 80
                                                             ),
                                                             new OCPIv2_3_0.EnergySource(
                                                                 OCPIv2_3_0.EnergySourceCategory.WIND,
                                                                 20
                                                             )
                                                         ],
                                                         [
                                                             new OCPIv2_3_0.EnvironmentalImpact(
                                                                 OCPIv2_3_0.EnvironmentalImpactCategory.CARBON_DIOXIDE,
                                                                 0.1
                                                             )
                                                         ],
                                                         "Stadtwerke Jena-Ost",
                                                         "New Green Deal"
                                                     ),
                                                     DateTime.Parse("2020-09-22").ToUniversalTime()
                                                 )
                                             ],

                                             new OCPIv2_3_0.SignedData(
                                                 OCPIv2_3_0.EncodingMethod.GraphDefined,
                                                 [
                                                     new OCPIv2_3_0.SignedValue(
                                                         OCPIv2_3_0.SignedValueNature.START,
                                                         "PlainStartValue",
                                                         "SignedStartValue"
                                                     ),
                                                     new OCPIv2_3_0.SignedValue(
                                                         OCPIv2_3_0.SignedValueNature.INTERMEDIATE,
                                                         "PlainIntermediateValue",
                                                         "SignedIntermediateValue"
                                                     ),
                                                     new OCPIv2_3_0.SignedValue(
                                                         OCPIv2_3_0.SignedValueNature.END,
                                                         "PlainEndValue",
                                                         "SignedEndValue"
                                                     )
                                                 ],
                                                 1,     // Encoding method version
                                                 null,  // Public key
                                                 URL.Parse("https://open.charging.cloud/pools/1/stations/1/evse/1/publicKey")
                                             ),

                                             // Total Fixed Costs
                                             new OCPIv2_3_0.Price(
                                                 20.00m,
                                                 [new OCPIv2_3_0.TaxAmount("VAT", 23.10m)]
                                             ),

                                             // Total Energy Cost
                                             new OCPIv2_3_0.Price(
                                                 20.00m,
                                                 [new OCPIv2_3_0.TaxAmount("VAT", 23.10m)]
                                             ),

                                             // Total Time Cost
                                             new OCPIv2_3_0.Price(
                                                 20.00m,
                                                 [new OCPIv2_3_0.TaxAmount("VAT", 23.10m)]
                                             ),

                                             // Total Parking Time
                                             TimeSpan.FromMinutes(120),

                                             // Total Parking Cost
                                             new OCPIv2_3_0.Price(
                                                 20.00m,
                                                 [new OCPIv2_3_0.TaxAmount("VAT", 23.10m)]
                                             ),

                                             // Total Reservation Cost
                                             new OCPIv2_3_0.Price(
                                                 20.00m,
                                                 [new OCPIv2_3_0.TaxAmount("VAT", 23.10m)]
                                             ),

                                             "Remark!",
                                             OCPIv2_3_0.InvoiceReference_Id.Parse("Invoice:0815"),
                                             true, // IsCredit
                                             OCPIv2_3_0.CreditReference_Id. Parse("Credit:0815"),
                                             false,

                                             DateTimeOffset.Parse("2026-01-17")

                                         )
                                     );

                // HTTP/1.1 201 Created
                // Date:                          Wed, 28 Dec 2022 23:20:07 GMT
                // Location:                      /2.2/emsp/cdrs/CDR0001
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Last-Modified:                 2020-09-11T22:00:00.000Z
                // ETag:                          ckAWY1kW3wi8MR04zps+WfXAFDVTrsYlHSnRn0nPJU8=
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                3436
                // X-Request-ID:                  dt88p12Mn313hQxf3t5UKpCjMKY37d
                // X-Correlation-ID:              2284ht8G82pE4Sxpn4KUj2t5tAS13j
                // 
                // {"data":{"id":"CDR0001","start_date_time":"2020-04-12T18:20:19.000Z","end_date_time":"2020-04-12T22:20:19.000Z","auth_id":"1234","auth_method":"AUTH_REQUEST","location":{"id":"LOC0001","location_type":"UNDERGROUND_GARAGE","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"time_zone":null,"last_updated":"2022-12-28T23:19:49.740Z"},"meter_id":"Meter0815","energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","vendor":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}]},"signed_data":{"encoding_method":"GraphDefined","encoding_method_version":"1","signed_values":[{"nature":"START","plain_data":"PlainStartValue","signed_data":"SignedStartValue"},{"nature":"INTERMEDIATE","plain_data":"PlainIntermediateValue","signed_data":"SignedIntermediateValue"},{"nature":"END","plain_data":"PlainEndValue","signed_data":"SignedEndValue"}],"url":"https://open.charging.cloud/pools/1/stations/1/evse/1/publicKey"},"currency":"EUR","tariffs":[{"id":"TARIFF0001","currency":"EUR","tariff_alt_text":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"tariff_alt_url":"https://open.charging.cloud","elements":[{"price_components":[{"type":"TIME","price":2.0,"step_size":300}],"restrictions":[{"start_time":"08:00","end_time":"18:00","start_date":"2020-12-01","end_date":"2020-12-31","min_kwh":1.12,"max_kwh":5.67,"min_power":1.49,"max_power":9.91,"min_duration":600.0,"max_duration":1800.0,"day_of_week":["MONDAY","TUESDAY"]}]}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T22:00:00.000Z"}],"charging_periods":[{"start_date_time":"2020-04-12T18:21:49.000Z","dimensions":[{"type":"ENERGY","volume":1.33}]},{"start_date_time":"2020-04-12T18:21:50.000Z","dimensions":[{"type":"TIME","volume":5.12}]}],"total_cost":10.0,"total_energy":50.0,"total_time":0.5,"total_parking_time":2.0,"remark":"Remark!","last_updated":"2020-09-11T22:00:00.000Z"},"status_code":1000,"status_message":"Hello world!","timestamp":"2022-12-28T23:20:07.358Z"}

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(201),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {
                }

            }

        }

        #endregion

    }

}
