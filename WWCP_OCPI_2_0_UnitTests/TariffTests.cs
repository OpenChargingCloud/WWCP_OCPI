/*
 * Copyright (c) 2015 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
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

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.WWCP.OCPI_2_0;

using org.GraphDefined.Vanaheimr.Illias;
using NUnit.Framework;

#endregion

namespace org.GraphDefined.WWCP.OCPI_2_0_UnitTests
{

#if __MonoCS__
#else

    /// <summary>
    /// Unit tests for OCPI v2.0 tariffs.
    /// https://github.com/ocpi/ocpi/blob/master/mod_tariffs.md#54-tariffrestrictions-class
    /// </summary>
    [TestFixture]
    public static class TariffTests
    {

        //FixMe: 1) The type of the pricecomponent is serialized as lowercase, why?
        //       2) The price of the pricecomponent is serialized as a string, why?

        #region Tariff0001()

        /// <summary>
        /// Simple Tariff example 2 euro per hour.
        /// </summary>
        [Test]
        public static void Tariff0001()
        {

            var tariff    = new Tariff(Tariff_Id.Parse("12"),
                                       Currency.EUR,
                                       TariffElements: Enumeration.Create(
                                                           new TariffElement(
                                                               new PriceComponent(DimensionType.TIME, 2.00M, 300)
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

            var tariff    = new Tariff(Tariff_Id.Parse("12"),
                                       Currency.EUR,
                                       TariffText: I18NString.Create(Languages.en, "2 euro p/hour").
                                                                 Add(Languages.nl, "2 euro p/uur"),
                                       TariffElements: Enumeration.Create(
                                                           new TariffElement(
                                                               new PriceComponent(DimensionType.TIME, 2.00M, 300)
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

            var tariff    = new Tariff(Tariff_Id.Parse("12"),
                                       Currency.EUR,
                                       TariffUrl:      new Uri("https://company.com/tariffs/12"),
                                       TariffElements: Enumeration.Create(
                                                           new TariffElement(
                                                               PriceComponent.ChargingTime(2.00M, TimeSpan.FromSeconds(300))
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

            var tariff    = new Tariff(Tariff_Id.Parse("11"),
                                       Currency.EUR,
                                       TariffUrl:      new Uri("https://company.com/tariffs/11"),
                                       TariffElements: new List<TariffElement>() {

                                                           // 2.50 euro start tariff
                                                           new TariffElement(
                                                               PriceComponent.FlatRate(2.50M)
                                                           ),

                                                           // 1.00 euro per hour charging tariff for less than 32A (paid per 15 minutes)
                                                           new TariffElement(
                                                               PriceComponent.ChargingTime(1.00M, TimeSpan.FromSeconds(900)),
                                                               TariffRestriction.MaxPower(32M)
                                                           ),

                                                           // 2.00 euro per hour charging tariff for more than 32A on weekdays (paid per 10 minutes)
                                                           new TariffElement(
                                                               PriceComponent.ChargingTime(2.00M, TimeSpan.FromSeconds(600)),
                                                               new TariffRestriction(Power:      DecimalMinMax.FromMin(32M),
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
                                                               PriceComponent.ChargingTime(1.25M, TimeSpan.FromSeconds(600)),
                                                               new TariffRestriction(Power:      DecimalMinMax.FromMin(32M),
                                                                                     DayOfWeek:  Enumeration.Create(
                                                                                                     DayOfWeek.Saturday,
                                                                                                     DayOfWeek.Sunday
                                                                                                 ))
                                                           ),


                                                           // Parking on weekdays: between 09:00 and 18:00: 5 euro(paid per 5 minutes)
                                                           new TariffElement(
                                                               PriceComponent.ParkingTime(5M, TimeSpan.FromSeconds(300)),
                                                               new TariffRestriction(Time:       TimeRange.From(9).To(18),
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
                                                               PriceComponent.ParkingTime(6M, TimeSpan.FromSeconds(300)),
                                                               new TariffRestriction(Time:       TimeRange.From(10).To(17),
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

#endif

}
