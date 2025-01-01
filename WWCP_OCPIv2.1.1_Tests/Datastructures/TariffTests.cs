/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.Datastructures
{

    /// <summary>
    /// Charging tariffs tests.
    /// https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_tariffs.md
    /// </summary>
    [TestFixture]
    public static class TariffTests
    {

        // Simple Tariff example 2 euro per hour
        // https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_tariffs.md#simple-tariff-example-2-euro-per-hour

        // {
        //     "id": "12",
        //     "currency": "EUR",
        //     "elements": [{
        //         "price_components": [{
        //             "type": "TIME",
        //             "price": 2.00,
        //             "step_size": 300
        //         }]
        //     }],
        //     "last_updated": "2015-06-29T20:39:09Z"
        // }


        // Simple Tariff example with alternative multi language text
        // https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_tariffs.md#simple-tariff-example-2-euro-per-hour

        // {
        //     "id": "12",
        //     "currency": "EUR",
        //     "tariff_alt_text": [{
        //         "language": "en",
        //         "text": "2 euro p/hour"
        //     }, {
        //         "language": "nl",
        //         "text": "2 euro p/uur"
        //     }],
        //     "elements": [{
        //         "price_components": [{
        //             "type": "TIME",
        //             "price": 2.00,
        //             "step_size": 300
        //         }]
        //     }],
        //     "last_updated": "2015-06-29T20:39:09Z"
        // }


        // Simple Tariff example with alternative URL
        // https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_tariffs.md#simple-tariff-example-with-alternative-url

        // {
        //     "id": "12",
        //     "currency": "EUR",
        //     "tariff_alt_url": "https://company.com/tariffs/12",
        //     "elements": [{
        //         "price_components": [{
        //             "type": "TIME",
        //             "price": 2.00,
        //             "step_size": 300
        //         }]
        //     }],
        //     "last_updated": "2015-06-29T20:39:09Z"
        // }


        // Complex Tariff example
        // https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_tariffs.md#complex-tariff-example

        // {
        //     "id": "11",
        //     "currency": "EUR",
        //     "tariff_alt_url": "https://company.com/tariffs/11",
        //     "elements": [{
        //         "price_components": [{
        //             "type": "FLAT",
        //             "price": 2.50,
        //             "step_size": 1
        //         }]
        //     }, {
        //         "price_components": [{
        //             "type": "TIME",
        //             "price": 1.00,
        //             "step_size": 900
        //         }],
        //         "restrictions": {
        //             "max_power": 32.00
        //         }
        //     }, {
        //         "price_components": [{
        //             "type": "TIME",
        //             "price": 2.00,
        //             "step_size": 600
        //         }],
        //         "restrictions": {
        //             "min_power": 32.00,
        //             "day_of_week": ["MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY"]
        //         }
        //     }, {
        //         "price_components": [{
        //             "type": "TIME",
        //             "price": 1.25,
        //             "step_size": 600
        //         }],
        //         "restrictions": {
        //             "min_power": 32.00,
        //             "day_of_week": ["SATURDAY", "SUNDAY"]
        //         }
        //     }, {
        //         "price_components": [{
        //             "type": "PARKING_TIME",
        //             "price": 5.00,
        //             "step_size": 300
        //         }],
        //         "restrictions": {
        //             "start_time": "09:00",
        //             "end_time": "18:00",
        //             "day_of_week": ["MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY"]
        //         }
        //     }, {
        //         "price_components": [{
        //             "type": "PARKING_TIME",
        //             "price": 6.00,
        //             "step_size": 300
        //         }],
        //         "restrictions": {
        //             "start_time": "10:00",
        //             "end_time": "17:00",
        //             "day_of_week": ["SATURDAY"]
        //         }
        //     }],
        //     "last_updated": "2015-06-29T20:39:09Z"
        // }


        // Free of Charge Tariff example
        // https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_tariffs.md#free-of-charge-tariff-example

        // {
        //     "id": "12",
        //     "currency": "EUR",
        //     "elements": [{
        //         "price_components": [{
        //             "type": "FLAT",
        //             "price": 0.00,
        //             "step_size": 0
        //         }]
        //     }],
        //     "last_updated": "2015-06-29T20:39:09Z"
        // }


        #region Tariff_SerializeDeserialize_Test01()

        /// <summary>
        /// Tariff serialize, deserialize and compare test.
        /// </summary>
        [Test]
        public static void Tariff_SerializeDeserialize_Test01()
        {

            #region Define tariff1

            var tariff1 = new Tariff(
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
                          );

            #endregion


            var json = tariff1.ToJSON();

            ClassicAssert.AreEqual("TARIFF0001",   json["id"]?.Value<String>());


            if (Tariff.TryParse(json,
                                out var tariff2,
                                out var errorResponse,
                                tariff1.CountryCode,
                                tariff1.PartyId))
            {

                ClassicAssert.IsNotNull(tariff2);
                ClassicAssert.IsNull   (errorResponse);

                if (tariff2 is not null)
                {

                    ClassicAssert.AreEqual(tariff1.Id,                        tariff2.Id);
                    ClassicAssert.AreEqual(tariff1.Currency,                  tariff2.Currency);
                    ClassicAssert.AreEqual(tariff1.TariffElements,            tariff2.TariffElements);
                    ClassicAssert.AreEqual(tariff1.TariffAltText,             tariff2.TariffAltText);
                    ClassicAssert.AreEqual(tariff1.TariffAltURL,              tariff2.TariffAltURL);
                    ClassicAssert.AreEqual(tariff1.EnergyMix,                 tariff2.EnergyMix);

                    ClassicAssert.AreEqual(tariff1.LastUpdated.ToIso8601(),   tariff2.LastUpdated.ToIso8601());

                }

            }
            else
                ClassicAssert.Fail("Error: " + errorResponse);

        }

        #endregion


    }

}
