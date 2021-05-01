/*
 * Copyright (c) 2015--2021 GraphDefined GmbH
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

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



    }

}
