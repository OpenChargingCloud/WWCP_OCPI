/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
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

using NUnit.Framework;

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
                              Currency.EUR,
                              [
                                  new TariffElement(
                                      [
                                          PriceComponent.ChargingTime(
                                              2.00M,
                                              0.10M,
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
                                          ReservationRestrictions.RESERVATION
                                      )
                                  )
                              ],
                              TariffType.PROFILE_GREEN,
                              [
                                  new DisplayText(Languages.de, "Hallo Welt!"),
                                  new DisplayText(Languages.en, "Hello world!"),
                              ],
                              URL.Parse("https://open.charging.cloud"),
                              new Price( // Min Price
                                  1.10m,
                                  1.26m
                              ),
                              new Price( // Max Price
                                  2.20m,
                                  2.52m
                              ),
                              DateTime.Parse("2020-12-01"), // Start timestamp
                              DateTime.Parse("2020-12-31"), // End timestamp
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
                              DateTime.Parse("2020-09-22")
                          );

            var JSON = TariffA.ToJSON();

            Assert.That(JSON["country_code"].Value<String>(), Is.EqualTo("DE"));
            Assert.That(JSON["party_id"].    Value<String>(), Is.EqualTo("GEF"));
            Assert.That(JSON["id"].          Value<String>(), Is.EqualTo("TARIFF0001"));

            Assert.That(Tariff.TryParse(JSON, out var TariffB, out var ErrorResponse), Is.True);
            Assert.That(ErrorResponse,                                                 Is.Null);

            Assert.That(TariffB.CountryCode,    Is.EqualTo(TariffA.CountryCode));
            Assert.That(TariffB.PartyId,        Is.EqualTo(TariffA.PartyId));
            Assert.That(TariffB.Id,             Is.EqualTo(TariffA.Id));
            Assert.That(TariffB.Currency,       Is.EqualTo(TariffA.Currency));
            Assert.That(TariffB.TariffElements, Is.EqualTo(TariffA.TariffElements));

            Assert.That(TariffB.TariffType,    Is.EqualTo(TariffA.TariffType));
            Assert.That(TariffB.TariffAltText, Is.EqualTo(TariffA.TariffAltText));
            Assert.That(TariffB.TariffAltURL,  Is.EqualTo(TariffA.TariffAltURL));
            Assert.That(TariffB.MinPrice,      Is.EqualTo(TariffA.MinPrice));
            Assert.That(TariffB.MaxPrice,      Is.EqualTo(TariffA.MaxPrice));
            Assert.That(TariffB.Start,         Is.EqualTo(TariffA.Start));
            Assert.That(TariffB.End,           Is.EqualTo(TariffA.End));
            Assert.That(TariffB.EnergyMix,     Is.EqualTo(TariffA.EnergyMix));

            Assert.That(TariffB.LastUpdated.ToISO8601(), Is.EqualTo(TariffA.LastUpdated.ToISO8601()));

        }

        #endregion



    }

}
