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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.CPOTests
{

    [TestFixture]
    public class CDRs_Tests : ANodeTests
    {

        #region CPO_PostCDR_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_PostCDR_Test()
        {

            var graphDefinedEMSP1 = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP1);

            if (graphDefinedEMSP1 is not null)
            {

                #region POST CDR

                var response = await graphDefinedEMSP1.PostCDR(new CDR(
                                                                   CountryCode.Parse("DE"),
                                                                   Party_Id.   Parse("GEF"),
                                                                   CDR_Id.     Parse("CDR0001"),
                                                                   DateTime.   Parse("2020-04-12T18:20:19Z").ToUniversalTime(),
                                                                   DateTime.   Parse("2020-04-12T22:20:19Z").ToUniversalTime(),
                                                                   Auth_Id.    Parse("1234"),
                                                                   AuthMethods.AUTH_REQUEST,
                                                                   new Location(
                                                                       CountryCode.Parse("DE"),
                                                                       Party_Id.   Parse("GEF"),
                                                                       Location_Id.Parse("LOC0001"),
                                                                       LocationType.UNDERGROUND_GARAGE,
                                                                       "Biberweg 18",
                                                                       "Jena",
                                                                       "07749",
                                                                       Country.Germany,
                                                                       GeoCoordinate.Parse(10, 20)
                                                                   ),
                                                                   OCPI.Currency.EUR,

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
                                                                   TimeSpan.FromMinutes(30),
                                                                   Meter_Id.Parse("Meter0815"),

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

                                                                   LastUpdated: DateTime.Parse("2020-09-12").ToUniversalTime()

                                                               ));

                #endregion

                // HTTP/1.1 201 Created
                // Date:                          Wed, 28 Dec 2022 23:20:07 GMT
                // Location:                      /2.1.1/emsp/cdrs/CDR0001
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

                Assert.IsNotNull(response);
                Assert.AreEqual (201,             response.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response.StatusCode);
                Assert.AreEqual ("Hello world!",  response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

                Assert.IsNotNull(response.RequestId);
                Assert.IsNotNull(response.CorrelationId);
                Assert.IsNotNull(response.Data);

                if (response.Data is not null)
                {

                    Assert.IsTrue(response.Data.Id == CDR_Id.Parse("CDR0001"));

                    //Assert.AreEqual (1, response.Data.First().CDRElements.Count());
                    //Assert.AreEqual (2, response.Data.First().CDRAltText. Count());

                }

                //Assert.IsNotNull(response.Request);

            }

        }

        #endregion


    }

}
