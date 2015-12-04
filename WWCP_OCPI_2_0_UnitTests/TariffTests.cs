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

        [Test]
        public static void Tariff0001()
        {

            var tariff    = new Tariff(Tariff_Id.Parse("12"),
                                       Currency.EUR,
                                       TariffElements: new List<TariffElement>() {
                                                           new TariffElement(
                                                               new PriceComponent(DimensionType.TIME, 2.00M, 300)
                                                           )
                                                       }
                                      );

            var expected  = new JObject(new JProperty("id",        "12"),
                                        new JProperty("currency",  "EUR"),
                                        new JProperty("elements",  new JArray(
                                            new JObject(new JProperty("price_components", new JArray(
                                                new JObject(
                                                    new JProperty("type",      "TIME"),
                                                    new JProperty("price",     "2.00"),
                                                    new JProperty("step_size", 300)
                                            ))))
                                       )));

            Assert.AreEqual(expected.ToString(), tariff.ToJSON().ToString());

        }

        #endregion

        #region Tariff0002()

        [Test]
        public static void Tariff0002()
        {

            var tariff    = new Tariff(Tariff_Id.Parse("12"),
                                       Currency.EUR,
                                       TariffText: I18NString.Create(Languages.en, "2 euro p/hour").
                                                                 Add(Languages.nl, "2 euro p/uur"),
                                       TariffElements: new List<TariffElement>() {
                                                           new TariffElement(
                                                               new PriceComponent(DimensionType.TIME, 2.00M, 300)
                                                           )
                                                       }
                                      );

            var expected  = new JObject(new JProperty("id",        "12"),
                                        new JProperty("currency",  "EUR"),
                                        new JProperty("tariff_alt_text", new JArray(
                                            new JObject(new JProperty("language", "en"), new JProperty("text", "2 euro p/hour")),
                                            new JObject(new JProperty("language", "nl"), new JProperty("text", "2 euro p/uur"))
                                            )),
                                        new JProperty("elements",  new JArray(
                                            new JObject(new JProperty("price_components", new JArray(
                                                new JObject(
                                                    new JProperty("type",      "TIME"),
                                                    new JProperty("price",     "2.00"),
                                                    new JProperty("step_size", 300)
                                            ))))
                                       )));

            Assert.AreEqual(expected.ToString(), tariff.ToJSON().ToString());

        }

        #endregion

    }

}
