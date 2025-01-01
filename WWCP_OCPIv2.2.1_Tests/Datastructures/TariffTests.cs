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

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.Datastructures
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
                              OCPI.Currency.EUR,
                              new TariffElement[] {
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
                              TariffType.PROFILE_GREEN,
                              new[] {
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
                              DateTime.Parse("2020-09-22")
                          );

            var JSON = TariffA.ToJSON();

            ClassicAssert.AreEqual("DE",                             JSON["country_code"].Value<String>());
            ClassicAssert.AreEqual("GEF",                            JSON["party_id"].    Value<String>());
            ClassicAssert.AreEqual("TARIFF0001",                     JSON["id"].          Value<String>());

            ClassicAssert.IsTrue(Tariff.TryParse(JSON, out Tariff TariffB, out String ErrorResponse));
            ClassicAssert.IsNull(ErrorResponse);

            ClassicAssert.AreEqual(TariffA.CountryCode,              TariffB.CountryCode);
            ClassicAssert.AreEqual(TariffA.PartyId,                  TariffB.PartyId);
            ClassicAssert.AreEqual(TariffA.Id,                       TariffB.Id);
            ClassicAssert.AreEqual(TariffA.Currency,                 TariffB.Currency);
            ClassicAssert.AreEqual(TariffA.TariffElements,           TariffB.TariffElements);

            ClassicAssert.AreEqual(TariffA.TariffType,               TariffB.TariffType);
            ClassicAssert.AreEqual(TariffA.TariffAltText,            TariffB.TariffAltText);
            ClassicAssert.AreEqual(TariffA.TariffAltURL,             TariffB.TariffAltURL);
            ClassicAssert.AreEqual(TariffA.MinPrice,                 TariffB.MinPrice);
            ClassicAssert.AreEqual(TariffA.MaxPrice,                 TariffB.MaxPrice);
            ClassicAssert.AreEqual(TariffA.Start,                    TariffB.Start);
            ClassicAssert.AreEqual(TariffA.End,                      TariffB.End);
            ClassicAssert.AreEqual(TariffA.EnergyMix,                TariffB.EnergyMix);

            ClassicAssert.AreEqual(TariffA.LastUpdated.ToIso8601(),  TariffB.LastUpdated.ToIso8601());

        }

        #endregion



    }

}
