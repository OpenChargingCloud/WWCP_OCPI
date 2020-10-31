/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;
using System.Collections.Generic;

using NUnit.Framework;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.UnitTests
{

    /// <summary>
    /// Unit tests for charging tariffs.
    /// https://github.com/ocpi/ocpi/blob/master/mod_tariffs.asciidoc
    /// </summary>
    [TestFixture]
    public static class TariffTests
    {

        #region Tariff_SerializeDeserialize_Test01()

        /// <summary>
        /// Tariff serialize, deserialize and compare test.
        /// </summary>
        [Test]
        public static void Tariff_SerializeDeserialize_Test01()
        {

            var TariffA = new Tariff(
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
                                              ReservationRestrictionTypes.RESERVATION
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
                              DateTime.Parse("2020-12-01"), // Start timestamp
                              DateTime.Parse("2020-12-31"), // End timestamp
                              new EnergyMix(
                                  true,
                                  new EnergySource[] {
                                      new EnergySource(
                                          EnergySourceCategories.SOLAR,
                                          80
                                      ),
                                      new EnergySource(
                                          EnergySourceCategories.WIND,
                                          20
                                      )
                                  },
                                  new EnvironmentalImpact[] {
                                      new EnvironmentalImpact(
                                          EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                          0.1
                                      )
                                  },
                                  "Stadtwerke Jena-Ost",
                                  "New Green Deal"
                              ),
                              DateTime.Parse("2020-09-22")
                          );

            var JSON = TariffA.ToJSON();

            Assert.AreEqual("DE",                             JSON["country_code"].Value<String>());
            Assert.AreEqual("GEF",                            JSON["party_id"].    Value<String>());
            Assert.AreEqual("TARIFF0001",                     JSON["id"].          Value<String>());

            Assert.IsTrue(Tariff.TryParse(JSON, out Tariff TariffB, out String ErrorResponse));
            Assert.IsNull(ErrorResponse);

            Assert.AreEqual(TariffA.CountryCode,              TariffB.CountryCode);
            Assert.AreEqual(TariffA.PartyId,                  TariffB.PartyId);
            Assert.AreEqual(TariffA.Id,                       TariffB.Id);
            Assert.AreEqual(TariffA.Currency,                 TariffB.Currency);
            Assert.AreEqual(TariffA.TariffElements,           TariffB.TariffElements);

            Assert.AreEqual(TariffA.TariffType,               TariffB.TariffType);
            Assert.AreEqual(TariffA.TariffAltText,            TariffB.TariffAltText);
            Assert.AreEqual(TariffA.TariffAltURL,             TariffB.TariffAltURL);
            Assert.AreEqual(TariffA.MinPrice,                 TariffB.MinPrice);
            Assert.AreEqual(TariffA.MaxPrice,                 TariffB.MaxPrice);
            Assert.AreEqual(TariffA.Start,                    TariffB.Start);
            Assert.AreEqual(TariffA.End,                      TariffB.End);
            Assert.AreEqual(TariffA.EnergyMix,                TariffB.EnergyMix);

            Assert.AreEqual(TariffA.LastUpdated.ToIso8601(),  TariffB.LastUpdated.ToIso8601());

        }

        #endregion


        #region Tariff0001()

        /// <summary>
        /// Simple Tariff example 2 euro per hour.
        /// </summary>
        [Test]
        public static void Tariff0001()
        {

            var tariff    = new Tariff(
                                CountryCode.Parse("DE"),
                                Party_Id.   Parse("GEF"),
                                Tariff_Id.  Parse("12"),
                                Currency.   EUR,
                                TariffElements:  Enumeration.Create(
                                                     new TariffElement(
                                                         new PriceComponent(
                                                             TariffDimensions.TIME,
                                                             2.00M,
                                                             0.10M,
                                                             300
                                                         )
                                                     )
                                                 )
                            );

            var expected  = new JObject(new JProperty("id",        "12"),
                                        new JProperty("currency",  "EUR"),
                                        new JProperty("elements",  new JArray(
                                            new JObject(
                                                new JProperty("price_components", new JArray(
                                                    new JObject(
                                                        new JProperty("type",      "TIME"),
                                                        new JProperty("price",     "2.00"),
                                                        new JProperty("step_size", 300)
                                                )))
                                            )
                                       )));

            Assert.AreEqual(expected.ToString(), tariff.ToJSON().ToString());

        }

        #endregion

        #region Tariff0002()

        /// <summary>
        /// Simple Tariff example with alternative multi language text.
        /// </summary>
        [Test]
        public static void Tariff0002()
        {

            var tariff    = new Tariff(
                                CountryCode.Parse("DE"),
                                Party_Id.   Parse("GEF"),
                                Tariff_Id.  Parse("12"),
                                Currency.   EUR,
                                TariffAltText:   new DisplayText[] {
                                                     new DisplayText(Languages.en, "2 euro p/hour"),
                                                     new DisplayText(Languages.nl, "2 euro p/uur")
                                                 },
                                TariffElements:  Enumeration.Create(
                                                     new TariffElement(
                                                         new PriceComponent(
                                                             TariffDimensions.TIME,
                                                             2.00M,
                                                             0.10M,
                                                             300
                                                         )
                                                     )
                                                 )
                            );

            var expected  = new JObject(new JProperty("id",        "12"),
                                        new JProperty("currency",  "EUR"),
                                        new JProperty("tariff_alt_text", new JArray(
                                            new JObject(new JProperty("language", "en"), new JProperty("text", "2 euro p/hour")),
                                            new JObject(new JProperty("language", "nl"), new JProperty("text", "2 euro p/uur"))
                                            )),
                                        new JProperty("elements",  new JArray(
                                            new JObject(
                                                new JProperty("price_components", new JArray(
                                                    new JObject(
                                                        new JProperty("type",      "TIME"),
                                                        new JProperty("price",     "2.00"),
                                                        new JProperty("step_size", 300)
                                                )))
                                            )
                                       )));

            Assert.AreEqual(expected.ToString(), tariff.ToJSON().ToString());

        }

        #endregion

        #region Tariff0003()

        /// <summary>
        /// Simple Tariff example with alternative URL.
        /// </summary>
        [Test]
        public static void Tariff0003()
        {

            var tariff    = new Tariff(
                                CountryCode.Parse("DE"),
                                Party_Id.   Parse("GEF"),
                                Tariff_Id.  Parse("12"),
                                Currency.   EUR,
                                TariffAltURL:    URL.Parse("https://company.com/tariffs/12"),
                                TariffElements:  Enumeration.Create(
                                                     new TariffElement(
                                                         PriceComponent.ChargingTime(
                                                             TimeSpan.FromSeconds(300),
                                                             2.00M
                                                         )
                                                     )
                                                 )
                               );

            var expected  = new JObject(new JProperty("id",             "12"),
                                        new JProperty("currency",       "EUR"),
                                        new JProperty("tariff_alt_url", "https://company.com/tariffs/12"),
                                        new JProperty("elements",  new JArray(
                                            new JObject(
                                                new JProperty("price_components", new JArray(
                                                    new JObject(
                                                        new JProperty("type",      "TIME"),
                                                        new JProperty("price",     "2.00"),
                                                        new JProperty("step_size", 300)
                                                )))
                                            )
                                       )));

            Assert.AreEqual(expected.ToString(), tariff.ToJSON().ToString());

        }

        #endregion

        #region Tariff0004()

        /// <summary>
        /// Complex Tariff example
        /// 
        /// 2.50 euro start tariff 1.00 euro per hour charging tariff for less
        /// than 32A (paid per 15 minutes) 2.00 euro per hour charging tariff for
        /// more then 32A on weekdays (paid per 10 minutes) 1.25 euro per hour
        /// charging tariff for more then 32A during the weekend (paid per 10
        /// minutes)
        /// 
        /// Parking:
        ///   Weekdays: between 09:00 and 18:00: 5 euro (paid per 5 minutes)
        ///   Saturday: between 10:00 and 17:00: 6 euro (paid per 5 minutes)
        /// </summary>
        [Test]
        public static void Tariff0004()
        {

            var tariff    = new Tariff(
                                CountryCode.Parse("DE"),
                                Party_Id.   Parse("GEF"),
                                Tariff_Id.  Parse("12"),
                                Currency.   EUR,
                                TariffAltURL:    URL.Parse("https://company.com/tariffs/11"),
                                TariffElements:  new List<TariffElement>() {

                                                     // 2.50 euro start tariff
                                                     new TariffElement(
                                                         PriceComponent.FlatRate(2.50M)
                                                     ),

                                                     // 1.00 euro per hour charging tariff for less than 32A (paid per 15 minutes)
                                                     new TariffElement(
                                                         PriceComponent.ChargingTime(TimeSpan.FromSeconds(900), 1.00M),
                                                         new TariffRestrictions(MaxPower: 32M)
                                                     ),

                                                     // 2.00 euro per hour charging tariff for more than 32A on weekdays (paid per 10 minutes)
                                                     new TariffElement(
                                                         PriceComponent.ChargingTime(TimeSpan.FromSeconds(600), 2.00M),
                                                         new TariffRestrictions(MinPower:   32M,
                                                                                DayOfWeek:  Enumeration.Create(
                                                                                                DayOfWeek.Monday,
                                                                                                DayOfWeek.Tuesday,
                                                                                                DayOfWeek.Wednesday,
                                                                                                DayOfWeek.Thursday,
                                                                                                DayOfWeek.Friday
                                                                                            ))
                                                     ),

                                                     // 1.25 euro per hour charging tariff for more then 32A during the weekend (paid per 10 minutes)
                                                     new TariffElement(
                                                         PriceComponent.ChargingTime(TimeSpan.FromSeconds(600), 1.25M),
                                                         new TariffRestrictions(MinPower:   32M,
                                                                                DayOfWeek:  Enumeration.Create(
                                                                                                DayOfWeek.Saturday,
                                                                                                DayOfWeek.Sunday
                                                                                            ))
                                                     ),


                                                     // Parking on weekdays: between 09:00 and 18:00: 5 euro(paid per 5 minutes)
                                                     new TariffElement(
                                                         PriceComponent.ParkingTime(TimeSpan.FromSeconds(300), 5M),
                                                         new TariffRestrictions(StartTime:  Time.FromHour(9),
                                                                                EndTime:    Time.FromHour(18),
                                                                                DayOfWeek:  Enumeration.Create(
                                                                                                DayOfWeek.Monday,
                                                                                                DayOfWeek.Tuesday,
                                                                                                DayOfWeek.Wednesday,
                                                                                                DayOfWeek.Thursday,
                                                                                                DayOfWeek.Friday
                                                                                            ))
                                                     ),

                                                     // Parking on saturday: between 10:00 and 17:00: 6 euro (paid per 5 minutes)
                                                     new TariffElement(
                                                         PriceComponent.ParkingTime(TimeSpan.FromSeconds(300), 6M),
                                                         new TariffRestrictions(StartTime:  Time.FromHour(10),
                                                                                EndTime:    Time.FromHour(17),
                                                                                DayOfWeek:  new DayOfWeek[] {
                                                                                                DayOfWeek.Saturday
                                                                                            })
                                                     )

                                                 }
                            );

            var expected  = new JObject(new JProperty("id",             "11"),
                                        new JProperty("currency",       "EUR"),
                                        new JProperty("tariff_alt_url", "https://company.com/tariffs/11"),
                                        new JProperty("elements",  new JArray(

                                            // 2.50 euro start tariff
                                            new JObject(
                                                new JProperty("price_components", new JArray(
                                                    new JObject(
                                                        new JProperty("type",      "FLAT"),
                                                        new JProperty("price",     "2.50"),
                                                        new JProperty("step_size", 1)
                                                    )))
                                            ),


                                            // 1.00 euro per hour charging tariff for less than 32A (paid per 15 minutes)
                                            new JObject(
                                                new JProperty("price_components", new JArray(
                                                new JObject(
                                                    new JProperty("type",      "TIME"),
                                                    new JProperty("price",     "1.00"),
                                                    new JProperty("step_size", 900)
                                                ))),
                                                new JProperty("restrictions", new JArray(
                                                    new JObject(
                                                        new JProperty("max_power", "32.00")
                                                    )
                                                ))
                                            ),

                                            // 2.00 euro per hour charging tariff for more than 32A on weekdays (paid per 10 minutes)
                                            new JObject(
                                                new JProperty("price_components", new JArray(
                                                new JObject(
                                                    new JProperty("type",      "TIME"),
                                                    new JProperty("price",     "2.00"),
                                                    new JProperty("step_size", 600)
                                                ))),
                                                new JProperty("restrictions", new JArray(
                                                    new JObject(
                                                        new JProperty("min_power",   "32.00"),
                                                        new JProperty("day_of_week", new JArray("MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY"))
                                                    )
                                                ))
                                            ),

                                            // 1.25 euro per hour charging tariff for more then 32A during the weekend (paid per 10 minutes)
                                            new JObject(
                                                new JProperty("price_components", new JArray(
                                                new JObject(
                                                    new JProperty("type",      "TIME"),
                                                    new JProperty("price",     "1.25"),
                                                    new JProperty("step_size", 600)
                                                ))),
                                                new JProperty("restrictions", new JArray(
                                                    new JObject(
                                                        new JProperty("min_power",   "32.00"),
                                                        new JProperty("day_of_week", new JArray("SATURDAY", "SUNDAY"))
                                                    )
                                                ))
                                            ),

                                            // Parking on weekdays: between 09:00 and 18:00: 5 euro(paid per 5 minutes)
                                            new JObject(
                                                new JProperty("price_components", new JArray(
                                                new JObject(
                                                    new JProperty("type",       "PARKING_TIME"),
                                                    new JProperty("price",      "5.00"),
                                                    new JProperty("step_size",  300)
                                                ))),
                                                new JProperty("restrictions", new JArray(
                                                    new JObject(
                                                        new JProperty("start_time",   "09:00"),
                                                        new JProperty("end_time",     "18:00"),
                                                        new JProperty("day_of_week",  new JArray("MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY"))
                                                    )
                                                ))
                                            ),

                                            // Parking on saturday: between 10:00 and 17:00: 6 euro (paid per 5 minutes)
                                            new JObject(
                                                new JProperty("price_components", new JArray(
                                                new JObject(
                                                    new JProperty("type",       "PARKING_TIME"),
                                                    new JProperty("price",      "6.00"),
                                                    new JProperty("step_size",  300)
                                                ))),
                                                new JProperty("restrictions", new JArray(
                                                    new JObject(
                                                        new JProperty("start_time",   "10:00"),
                                                        new JProperty("end_time",     "17:00"),
                                                        new JProperty("day_of_week",  new JArray("SATURDAY"))
                                                    )
                                                ))
                                            )

                                       )));

            Assert.AreEqual(expected.ToString(), tariff.ToJSON().ToString());

        }

        #endregion

    }

}
