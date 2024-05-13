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
                                        CountryCode:  CountryCode.Parse("DE"),
                                        PartyId:      Party_Id.   Parse("GDF")
                                    );

            ClassicAssert.IsNotNull(graphDefinedEMSP1);

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
                                                                               CDRDimension.Create(
                                                                                   CDRDimensionType.ENERGY,
                                                                                   1.33M
                                                                               )
                                                                           }
                                                                       ),
                                                                       new ChargingPeriod(
                                                                           DateTime.Parse("2020-04-12T18:21:50Z"),
                                                                           new[] {
                                                                               CDRDimension.Create(
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
                                                                                           2.00M,
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
                                                                                       1.49M,                         // MinPower
                                                                                       9.91M,                         // MaxPower
                                                                                       TimeSpan.FromMinutes(10),      // MinDuration
                                                                                       TimeSpan.FromMinutes(30),      // MaxDuration
                                                                                       new[] {
                                                                                           DayOfWeek.Monday,
                                                                                           DayOfWeek.Tuesday
                                                                                       }
                                                                                   )
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
                // Location:                      https://127.0.0.1/ocpi/emsp/2.1.1/cdrs/e5295528-6286-4d02-a2b4-39d82b3b6ab6

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (201,             response.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,            response.StatusCode);
                ClassicAssert.AreEqual ("Hello world!",  response.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

                ClassicAssert.IsNotNull(response.RequestId);
                ClassicAssert.IsNotNull(response.CorrelationId);
                ClassicAssert.IsNotNull(response.HTTPLocation);
                //ClassicAssert.IsTrue(response.Location == URL.Parse("https://127.0.0.1/ocpi/emsp/2.1.1/cdrs/e5295528-6286-4d02-a2b4-39d82b3b6ab6"));

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion


    }

}
