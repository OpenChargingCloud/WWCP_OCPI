﻿/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.EMSPTests
{

    [TestFixture]
    public class Tariffs_Tests : ANodeTests
    {

        #region EMSP_GetTariffs_Test1()

        /// <summary>
        /// EMSP EMSP_GetTariffs Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetTariffs_Test1()
        {

            var graphDefinedCPO = emsp1EMSPAPI?.GetCPOClient(
                                      CountryCode:  CountryCode.Parse("DE"),
                                      PartyId:      Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                #region Define Tariff1

                await cpoCommonAPI.AddTariff(new Tariff(
                                                 CountryCode.Parse("DE"),
                                                 Party_Id.   Parse("GEF"),
                                                 Tariff_Id.  Parse("TARIFF0001"),
                                                 Currency.EUR,
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
                                                 LastUpdated: DateTime.Parse("2020-09-22").ToUniversalTime()
                                             ),
                                             SkipNotifications: true
                                         );

                #endregion

                var response = await graphDefinedCPO.GetTariffs();

                // GET /2.1.1/cpo/locations HTTP/1.1
                // Date:                          Tue, 18 Apr 2023 03:41:25 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7134
                // Authorization:                 Token xxxxxx
                // Connection:                    close
                // X-Request-ID:                  1pKzWGQQd7EhYr9t7zf8dA7njt7W9h
                // X-Correlation-ID:              z5EKhQU7nK9UhYCMnCU4869zt5G882

                // HTTP/1.1 200 OK
                // Date:                          Tue, 18 Apr 2023 03:41:28 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // X-Total-Count:                 1
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                5494
                // X-Request-ID:                  1pKzWGQQd7EhYr9t7zf8dA7njt7W9h
                // X-Correlation-ID:              z5EKhQU7nK9UhYCMnCU4869zt5G882
                // 
                // {
                //     "data":            [{"id":"LOC0001","location_type":"PARKING_LOT","name":"Location 0001","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"related_locations":[{"latitude":"11","longitude":"22","name":{"language":"de","text":"Postkasten"}}],"evses":[{"uid":"DE*GEF*E*LOC0001*1","evse_id":"DE*GEF*E*LOC0001*1","status":"AVAILABLE","status_schedule":[{"period_begin":"2020-09-22T22:00:00.000Z","period_end":"2020-09-23T22:00:00.000Z","status":"INOPERATIVE"},{"period_begin":"2020-12-29T23:00:00.000Z","period_end":"2020-12-30T23:00:00.000Z","status":"OUTOFORDER"}],"capabilities":["RFID_READER","RESERVABLE"],"connectors":[{"id":"1","standard":"IEC_62196_T2","format":"SOCKET","power_type":"AC_3_PHASE","voltage":400,"amperage":30,"tariff_id":"DE*GEF*T0001","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-20T22:00:00.000Z"},{"id":"2","standard":"IEC_62196_T2_COMBO","format":"CABLE","power_type":"AC_3_PHASE","voltage":400,"amperage":20,"tariff_id":"DE*GEF*T0003","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-21T22:00:00.000Z"}],"energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","manufacturer":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","certificate_issuer":"German PTB","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","certificate_issuer":"GraphDefined","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}],"last_updated":"2023-04-18T03:41:24.847Z"},"floor_level":"1. Stock","coordinates":{"latitude":"10.10000","longitude":"20.20000"},"physical_reference":"Ladestation #1","directions":[{"language":"de","text":"Bitte klingeln!"},{"language":"en","text":"Ken sent me!"}],"parking_restrictions":["EV_ONLY","PLUGGED"],"images":[{"url":"http://example.com/pinguine.jpg","category":"OPERATOR","type":"jpeg","thumbnail":"http://example.com/kleine_pinguine.jpg","width":100,"height":150}],"last_updated":"2020-09-21T22:00:00.000Z"}],"directions":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"operator":{"name":"Open Charging Cloud","website":"https://open.charging.cloud","logo":{"url":"http://open.charging.cloud/logo.svg","category":"OPERATOR","type":"svg","thumbnail":"http://open.charging.cloud/logo_small.svg","width":1000,"height":1500}},"suboperator":{"name":"GraphDefined GmbH","website":"https://www.graphdefined.com","logo":{"url":"http://www.graphdefined.com/logo.png","category":"OPERATOR","type":"png","thumbnail":"http://www.graphdefined.com/logo_small.png","width":2000,"height":3000}},"owner":{"name":"Achim Friedland","website":"https://ahzf.de","logo":{"url":"http://ahzf.de/logo.gif","category":"OWNER","type":"gif","thumbnail":"http://ahzf.de/logo_small.gif","width":3000,"height":4500}},"facilities":["CAFE"],"time_zone":"Europe/Berlin","opening_times":{"twentyfourseven":false,"regular_hours":[{"weekday":1,"period_begin":"08:00","period_end":"15:00"},{"weekday":2,"period_begin":"09:00","period_end":"16:00"},{"weekday":3,"period_begin":"10:00","period_end":"17:00"},{"weekday":4,"period_begin":"11:00","period_end":"18:00"},{"weekday":5,"period_begin":"12:00","period_end":"19:00"}],"exceptional_openings":[{"period_begin":"2020-09-21T00:00:00.000Z","period_end":"2020-09-22T00:00:00.000Z"}],"exceptional_closings":[{"period_begin":"2020-12-24T00:00:00.000Z","period_end":"2020-12-26T00:00:00.000Z"}]},"charging_when_closed":false,"images":[{"url":"http://open.charging.cloud/locations/location0001.jpg","category":"LOCATION","type":"jpeg","thumbnail":"http://open.charging.cloud/locations/location0001s.jpg","width":200,"height":400}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T00:00:00.000Z"}],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-18T03:41:28.838Z"
                // }

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,           response.StatusCode);
                ClassicAssert.AreEqual ("Hello world!", response.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                ClassicAssert.IsNotNull(response.RequestId);
                ClassicAssert.IsNotNull(response.CorrelationId);
                ClassicAssert.IsNotNull(response.Data);

                if (response.Data is not null)
                {

                    ClassicAssert.AreEqual (1, response.Data.Count());

                    ClassicAssert.IsTrue   (response.Data.First().Id == Tariff_Id.Parse("TARIFF0001"));

                    ClassicAssert.AreEqual (1, response.Data.First().TariffElements.Count());
                    ClassicAssert.AreEqual (2, response.Data.First().TariffAltText. Count());

                }

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region EMSP_GetTariffs_Test2()

        /// <summary>
        /// EMSP EMSP_GetTariffs Test 2.
        /// </summary>
        [Test]
        public async Task EMSP_GetTariffs_Test2()
        {

            var graphDefinedCPO = emsp2EMSPAPI?.GetCPOClient(
                                      CountryCode:  CountryCode.Parse("DE"),
                                      PartyId:      Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                #region Define Tariff1

                await cpoCommonAPI.AddTariff(new Tariff(
                                                 CountryCode.Parse("DE"),
                                                 Party_Id.   Parse("GEF"),
                                                 Tariff_Id.  Parse("TARIFF0001"),
                                                 Currency.EUR,
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
                                                 LastUpdated: DateTime.Parse("2020-09-22").ToUniversalTime()
                                             ),
                                             SkipNotifications: true
                                         );

                #endregion

                var response = await graphDefinedCPO.GetTariffs();

                // GET /2.1.1/cpo/locations HTTP/1.1
                // Date:                          Tue, 18 Apr 2023 03:41:25 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7134
                // Authorization:                 Token xxxxxx
                // Connection:                    close
                // X-Request-ID:                  1pKzWGQQd7EhYr9t7zf8dA7njt7W9h
                // X-Correlation-ID:              z5EKhQU7nK9UhYCMnCU4869zt5G882

                // HTTP/1.1 200 OK
                // Date:                          Tue, 18 Apr 2023 03:41:28 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // X-Total-Count:                 1
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                5494
                // X-Request-ID:                  1pKzWGQQd7EhYr9t7zf8dA7njt7W9h
                // X-Correlation-ID:              z5EKhQU7nK9UhYCMnCU4869zt5G882
                // 
                // {
                //     "data":            [{"id":"LOC0001","location_type":"PARKING_LOT","name":"Location 0001","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"related_locations":[{"latitude":"11","longitude":"22","name":{"language":"de","text":"Postkasten"}}],"evses":[{"uid":"DE*GEF*E*LOC0001*1","evse_id":"DE*GEF*E*LOC0001*1","status":"AVAILABLE","status_schedule":[{"period_begin":"2020-09-22T22:00:00.000Z","period_end":"2020-09-23T22:00:00.000Z","status":"INOPERATIVE"},{"period_begin":"2020-12-29T23:00:00.000Z","period_end":"2020-12-30T23:00:00.000Z","status":"OUTOFORDER"}],"capabilities":["RFID_READER","RESERVABLE"],"connectors":[{"id":"1","standard":"IEC_62196_T2","format":"SOCKET","power_type":"AC_3_PHASE","voltage":400,"amperage":30,"tariff_id":"DE*GEF*T0001","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-20T22:00:00.000Z"},{"id":"2","standard":"IEC_62196_T2_COMBO","format":"CABLE","power_type":"AC_3_PHASE","voltage":400,"amperage":20,"tariff_id":"DE*GEF*T0003","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-21T22:00:00.000Z"}],"energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","manufacturer":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","certificate_issuer":"German PTB","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","certificate_issuer":"GraphDefined","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}],"last_updated":"2023-04-18T03:41:24.847Z"},"floor_level":"1. Stock","coordinates":{"latitude":"10.10000","longitude":"20.20000"},"physical_reference":"Ladestation #1","directions":[{"language":"de","text":"Bitte klingeln!"},{"language":"en","text":"Ken sent me!"}],"parking_restrictions":["EV_ONLY","PLUGGED"],"images":[{"url":"http://example.com/pinguine.jpg","category":"OPERATOR","type":"jpeg","thumbnail":"http://example.com/kleine_pinguine.jpg","width":100,"height":150}],"last_updated":"2020-09-21T22:00:00.000Z"}],"directions":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"operator":{"name":"Open Charging Cloud","website":"https://open.charging.cloud","logo":{"url":"http://open.charging.cloud/logo.svg","category":"OPERATOR","type":"svg","thumbnail":"http://open.charging.cloud/logo_small.svg","width":1000,"height":1500}},"suboperator":{"name":"GraphDefined GmbH","website":"https://www.graphdefined.com","logo":{"url":"http://www.graphdefined.com/logo.png","category":"OPERATOR","type":"png","thumbnail":"http://www.graphdefined.com/logo_small.png","width":2000,"height":3000}},"owner":{"name":"Achim Friedland","website":"https://ahzf.de","logo":{"url":"http://ahzf.de/logo.gif","category":"OWNER","type":"gif","thumbnail":"http://ahzf.de/logo_small.gif","width":3000,"height":4500}},"facilities":["CAFE"],"time_zone":"Europe/Berlin","opening_times":{"twentyfourseven":false,"regular_hours":[{"weekday":1,"period_begin":"08:00","period_end":"15:00"},{"weekday":2,"period_begin":"09:00","period_end":"16:00"},{"weekday":3,"period_begin":"10:00","period_end":"17:00"},{"weekday":4,"period_begin":"11:00","period_end":"18:00"},{"weekday":5,"period_begin":"12:00","period_end":"19:00"}],"exceptional_openings":[{"period_begin":"2020-09-21T00:00:00.000Z","period_end":"2020-09-22T00:00:00.000Z"}],"exceptional_closings":[{"period_begin":"2020-12-24T00:00:00.000Z","period_end":"2020-12-26T00:00:00.000Z"}]},"charging_when_closed":false,"images":[{"url":"http://open.charging.cloud/locations/location0001.jpg","category":"LOCATION","type":"jpeg","thumbnail":"http://open.charging.cloud/locations/location0001s.jpg","width":200,"height":400}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T00:00:00.000Z"}],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-18T03:41:28.838Z"
                // }

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,           response.StatusCode);
                ClassicAssert.AreEqual ("Hello world!", response.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                ClassicAssert.IsNotNull(response.RequestId);
                ClassicAssert.IsNotNull(response.CorrelationId);
                ClassicAssert.IsNotNull(response.Data);

                if (response.Data is not null)
                {

                    ClassicAssert.AreEqual (1, response.Data.Count());

                    ClassicAssert.IsTrue   (response.Data.First().Id == Tariff_Id.Parse("TARIFF0001"));

                    ClassicAssert.AreEqual (1, response.Data.First().TariffElements.Count());
                    ClassicAssert.AreEqual (2, response.Data.First().TariffAltText. Count());

                }

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region EMSP_GetTariffs_Versioned_Test1()

        /// <summary>
        /// EMSP EMSP_GetTariffs Versioned Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetTariffs_Versioned_Test1()
        {

            var graphDefinedCPO = emsp2EMSPAPI?.GetCPOClient(
                                      CountryCode:  CountryCode.Parse("DE"),
                                      PartyId:      Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                // Tariff 1a
                var tariffSwitchTimestamp1 = DateTime.Parse("2024-01-01").ToUniversalTime();
                // Tariff 1b
                var tariffSwitchTimestamp2 = DateTime.Parse("2024-04-01").ToUniversalTime();
                // Tariff 1c

                #region Define Tariff 1a

                var addResult1 = await cpoCommonAPI.AddTariff(
                                           new Tariff(
                                               CountryCode.Parse("DE"),
                                               Party_Id.   Parse("GEF"),
                                               Tariff_Id.  Parse("TARIFF0001"),
                                               Currency.EUR,
                                               new[] {
                                                   new TariffElement(
                                                       new[] {
                                                           PriceComponent.ChargingTime(
                                                               1.00M,
                                                               TimeSpan.FromSeconds(300)
                                                           )
                                                       },
                                                       new TariffRestrictions(
                                                           Time.FromHourMin(08,00),                         // Start time
                                                           Time.FromHourMin(18,00),                         // End time
                                                           DateTime.Parse("2020-12-01").ToUniversalTime(),  // Start timestamp
                                                           DateTime.Parse("2020-12-31").ToUniversalTime(),  // End timestamp
                                                           1.12M,                                           // MinkWh
                                                           5.67M,                                           // MaxkWh
                                                           1.49M,                                           // MinPower
                                                           9.91M,                                           // MaxPower
                                                           TimeSpan.FromMinutes(10),                        // MinDuration
                                                           TimeSpan.FromMinutes(30),                        // MaxDuration
                                                           new[] {
                                                               DayOfWeek.Monday,
                                                               DayOfWeek.Tuesday
                                                           }
                                                       )
                                                   )
                                               },
                                               new[] {
                                                   new DisplayText(Languages.de, "Tariff 1a!"),
                                                   new DisplayText(Languages.en, "Tariff 1a!"),
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
                                               NotAfter:     tariffSwitchTimestamp1,
                                               LastUpdated:  DateTime.Parse("2024-02-10").ToUniversalTime()
                                           ),
                                           SkipNotifications: true
                                       );

                #endregion

                #region Define Tariff 1b

                var addResult2 = await cpoCommonAPI.AddTariff(
                                           new Tariff(
                                               CountryCode.Parse("DE"),
                                               Party_Id.   Parse("GEF"),
                                               Tariff_Id.  Parse("TARIFF0001"),
                                               Currency.EUR,
                                               new[] {
                                                   new TariffElement(
                                                       new[] {
                                                           PriceComponent.ChargingTime(
                                                               2.00M,
                                                               TimeSpan.FromSeconds(300)
                                                           )
                                                       },
                                                       new TariffRestrictions(
                                                           Time.FromHourMin(08,00),                         // Start time
                                                           Time.FromHourMin(18,00),                         // End time
                                                           DateTime.Parse("2020-12-01").ToUniversalTime(),  // Start timestamp
                                                           DateTime.Parse("2020-12-31").ToUniversalTime(),  // End timestamp
                                                           1.12M,                                           // MinkWh
                                                           5.67M,                                           // MaxkWh
                                                           1.49M,                                           // MinPower
                                                           9.91M,                                           // MaxPower
                                                           TimeSpan.FromMinutes(10),                        // MinDuration
                                                           TimeSpan.FromMinutes(30),                        // MaxDuration
                                                           new[] {
                                                               DayOfWeek.Monday,
                                                               DayOfWeek.Tuesday
                                                           }
                                                       )
                                                   )
                                               },
                                               new[] {
                                                   new DisplayText(Languages.de, "Tariff 1b!"),
                                                   new DisplayText(Languages.en, "Tariff 1b!"),
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
                                               NotBefore:    tariffSwitchTimestamp1,
                                               NotAfter:     tariffSwitchTimestamp2,
                                               LastUpdated:  DateTime.Parse("2024-02-10").ToUniversalTime()
                                           ),
                                           SkipNotifications: true
                                       );

                #endregion

                #region Define Tariff 1c

                var addResult3 = await cpoCommonAPI.AddTariff(
                                           new Tariff(
                                               CountryCode.Parse("DE"),
                                               Party_Id.   Parse("GEF"),
                                               Tariff_Id.  Parse("TARIFF0001"),
                                               Currency.EUR,
                                               new[] {
                                                   new TariffElement(
                                                       new[] {
                                                           PriceComponent.ChargingTime(
                                                               3.00M,
                                                               TimeSpan.FromSeconds(300)
                                                           )
                                                       },
                                                       new TariffRestrictions(
                                                           Time.FromHourMin(08,00),                         // Start time
                                                           Time.FromHourMin(18,00),                         // End time
                                                           DateTime.Parse("2020-12-01").ToUniversalTime(),  // Start timestamp
                                                           DateTime.Parse("2020-12-31").ToUniversalTime(),  // End timestamp
                                                           1.12M,                                           // MinkWh
                                                           5.67M,                                           // MaxkWh
                                                           1.49M,                                           // MinPower
                                                           9.91M,                                           // MaxPower
                                                           TimeSpan.FromMinutes(10),                        // MinDuration
                                                           TimeSpan.FromMinutes(30),                        // MaxDuration
                                                           new[] {
                                                               DayOfWeek.Monday,
                                                               DayOfWeek.Tuesday
                                                           }
                                                       )
                                                   )
                                               },
                                               new[] {
                                                   new DisplayText(Languages.de, "Tariff 1c!"),
                                                   new DisplayText(Languages.en, "Tariff 1c!"),
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
                                               NotBefore:    tariffSwitchTimestamp2,
                                               LastUpdated:  DateTime.Parse("2024-02-10").ToUniversalTime()
                                           ),
                                           SkipNotifications: true
                                       );

                #endregion


                #region Get tariff at timestamp before first tariff switch

                var response1 = await graphDefinedCPO.GetTariffs(Timestamp: DateTime.Parse("2023-12-30").ToUniversalTime());

                Assert.Multiple(() => {

                    Assert.That(response1, Is.Not.Null);
                    if (response1 is not null)
                    {

                        Assert.That(response1.HTTPResponse?.HTTPStatusCode.Code,                      Is.EqualTo(200));
                        Assert.That(response1.StatusCode,                                             Is.EqualTo(1000));
                        Assert.That(response1.StatusMessage,                                          Is.EqualTo("Hello world!"));
                        Assert.That(Timestamp.Now - response1.Timestamp < TimeSpan.FromSeconds(10),   Is.True);

                        Assert.That(response1.RequestId,                                              Is.Not.Null);
                        Assert.That(response1.CorrelationId,                                          Is.Not.Null);

                        Assert.That(response1.Data, Is.Not.Null);
                        if (response1.Data is not null)
                        {

                            Assert.That(response1.Data.Count(),                      Is.EqualTo(1));

                            var firstTariff = response1.Data.FirstOrDefault();
                            if (firstTariff is not null)
                            {

                                Assert.That(firstTariff.Id,                           Is.EqualTo(Tariff_Id.Parse("TARIFF0001")));

                                Assert.That(firstTariff.TariffAltText. Count(),       Is.EqualTo(2));
                                Assert.That(firstTariff.TariffAltText.First().Text,   Is.EqualTo("Tariff 1a!"));

                                Assert.That(firstTariff.TariffElements.Count(),       Is.EqualTo(1));
                                var tariffElement1 = firstTariff.TariffElements.First();
                                //if (tariffElement1 is not null)
                                //{

                                    Assert.That(tariffElement1.PriceComponents.Count(),   Is.EqualTo(1));
                                    var priceComponent1 = tariffElement1.PriceComponents.First();

                                    //if (priceComponent1 is not null)
                                    //{

                                        Assert.That(priceComponent1.Price,   Is.EqualTo(1.00M));

                                    //}

                                //}

                            }

                        }

                        //Assert.That(response.Request, Is.Not.Null);

                    }

                });

                #endregion

                #region Get tariff at current timestamp

                var response2 = await graphDefinedCPO.GetTariffs();

                // GET /2.1.1/cpo/locations HTTP/1.1
                // Date:                          Tue, 18 Apr 2023 03:41:25 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7134
                // Authorization:                 Token xxxxxx
                // Connection:                    close
                // X-Request-ID:                  1pKzWGQQd7EhYr9t7zf8dA7njt7W9h
                // X-Correlation-ID:              z5EKhQU7nK9UhYCMnCU4869zt5G882

                // HTTP/1.1 200 OK
                // Date:                          Tue, 18 Apr 2023 03:41:28 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // X-Total-Count:                 1
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                5494
                // X-Request-ID:                  1pKzWGQQd7EhYr9t7zf8dA7njt7W9h
                // X-Correlation-ID:              z5EKhQU7nK9UhYCMnCU4869zt5G882
                // 
                // {
                //     "data":            [{"id":"LOC0001","location_type":"PARKING_LOT","name":"Location 0001","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"related_locations":[{"latitude":"11","longitude":"22","name":{"language":"de","text":"Postkasten"}}],"evses":[{"uid":"DE*GEF*E*LOC0001*1","evse_id":"DE*GEF*E*LOC0001*1","status":"AVAILABLE","status_schedule":[{"period_begin":"2020-09-22T22:00:00.000Z","period_end":"2020-09-23T22:00:00.000Z","status":"INOPERATIVE"},{"period_begin":"2020-12-29T23:00:00.000Z","period_end":"2020-12-30T23:00:00.000Z","status":"OUTOFORDER"}],"capabilities":["RFID_READER","RESERVABLE"],"connectors":[{"id":"1","standard":"IEC_62196_T2","format":"SOCKET","power_type":"AC_3_PHASE","voltage":400,"amperage":30,"tariff_id":"DE*GEF*T0001","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-20T22:00:00.000Z"},{"id":"2","standard":"IEC_62196_T2_COMBO","format":"CABLE","power_type":"AC_3_PHASE","voltage":400,"amperage":20,"tariff_id":"DE*GEF*T0003","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-21T22:00:00.000Z"}],"energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","manufacturer":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","certificate_issuer":"German PTB","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","certificate_issuer":"GraphDefined","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}],"last_updated":"2023-04-18T03:41:24.847Z"},"floor_level":"1. Stock","coordinates":{"latitude":"10.10000","longitude":"20.20000"},"physical_reference":"Ladestation #1","directions":[{"language":"de","text":"Bitte klingeln!"},{"language":"en","text":"Ken sent me!"}],"parking_restrictions":["EV_ONLY","PLUGGED"],"images":[{"url":"http://example.com/pinguine.jpg","category":"OPERATOR","type":"jpeg","thumbnail":"http://example.com/kleine_pinguine.jpg","width":100,"height":150}],"last_updated":"2020-09-21T22:00:00.000Z"}],"directions":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"operator":{"name":"Open Charging Cloud","website":"https://open.charging.cloud","logo":{"url":"http://open.charging.cloud/logo.svg","category":"OPERATOR","type":"svg","thumbnail":"http://open.charging.cloud/logo_small.svg","width":1000,"height":1500}},"suboperator":{"name":"GraphDefined GmbH","website":"https://www.graphdefined.com","logo":{"url":"http://www.graphdefined.com/logo.png","category":"OPERATOR","type":"png","thumbnail":"http://www.graphdefined.com/logo_small.png","width":2000,"height":3000}},"owner":{"name":"Achim Friedland","website":"https://ahzf.de","logo":{"url":"http://ahzf.de/logo.gif","category":"OWNER","type":"gif","thumbnail":"http://ahzf.de/logo_small.gif","width":3000,"height":4500}},"facilities":["CAFE"],"time_zone":"Europe/Berlin","opening_times":{"twentyfourseven":false,"regular_hours":[{"weekday":1,"period_begin":"08:00","period_end":"15:00"},{"weekday":2,"period_begin":"09:00","period_end":"16:00"},{"weekday":3,"period_begin":"10:00","period_end":"17:00"},{"weekday":4,"period_begin":"11:00","period_end":"18:00"},{"weekday":5,"period_begin":"12:00","period_end":"19:00"}],"exceptional_openings":[{"period_begin":"2020-09-21T00:00:00.000Z","period_end":"2020-09-22T00:00:00.000Z"}],"exceptional_closings":[{"period_begin":"2020-12-24T00:00:00.000Z","period_end":"2020-12-26T00:00:00.000Z"}]},"charging_when_closed":false,"images":[{"url":"http://open.charging.cloud/locations/location0001.jpg","category":"LOCATION","type":"jpeg","thumbnail":"http://open.charging.cloud/locations/location0001s.jpg","width":200,"height":400}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T00:00:00.000Z"}],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-18T03:41:28.838Z"
                // }

                Assert.Multiple(() => {

                    Assert.That(response2, Is.Not.Null);
                    if (response2 is not null)
                    {

                        Assert.That(response2.HTTPResponse?.HTTPStatusCode.Code,                      Is.EqualTo(200));
                        Assert.That(response2.StatusCode,                                             Is.EqualTo(1000));
                        Assert.That(response2.StatusMessage,                                          Is.EqualTo("Hello world!"));
                        Assert.That(Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10),   Is.True);

                        Assert.That(response2.RequestId,                                              Is.Not.Null);
                        Assert.That(response2.CorrelationId,                                          Is.Not.Null);

                        Assert.That(response2.Data, Is.Not.Null);
                        if (response2.Data is not null)
                        {

                            Assert.That(response2.Data.Count(),                      Is.EqualTo(1));

                            var firstTariff = response2.Data.FirstOrDefault();
                            if (firstTariff is not null)
                            {

                                Assert.That(firstTariff.Id,                           Is.EqualTo(Tariff_Id.Parse("TARIFF0001")));

                                Assert.That(firstTariff.TariffAltText. Count(),       Is.EqualTo(2));
                                Assert.That(firstTariff.TariffAltText.First().Text,   Is.EqualTo("Tariff 1b!"));

                                Assert.That(firstTariff.TariffElements.Count(),       Is.EqualTo(1));
                                var tariffElement1 = firstTariff.TariffElements.First();
                                //if (tariffElement1 is not null)
                                //{

                                    Assert.That(tariffElement1.PriceComponents.Count(),   Is.EqualTo(1));
                                    var priceComponent1 = tariffElement1.PriceComponents.First();

                                    //if (priceComponent1 is not null)
                                    //{

                                        Assert.That(priceComponent1.Price,   Is.EqualTo(2.00M));

                                    //}

                                //}

                            }

                        }

                        //Assert.That(response.Request, Is.Not.Null);

                    }

                });

                #endregion

                #region Get tariff at timestamp after second tariff switch

                var response3 = await graphDefinedCPO.GetTariffs(Timestamp: DateTime.Parse("2024-05-03").ToUniversalTime());

                Assert.Multiple(() => {

                    Assert.That(response3, Is.Not.Null);
                    if (response3 is not null)
                    {

                        Assert.That(response3.HTTPResponse?.HTTPStatusCode.Code,                      Is.EqualTo(200));
                        Assert.That(response3.StatusCode,                                             Is.EqualTo(1000));
                        Assert.That(response3.StatusMessage,                                          Is.EqualTo("Hello world!"));
                        Assert.That(Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10),   Is.True);

                        Assert.That(response3.RequestId,                                              Is.Not.Null);
                        Assert.That(response3.CorrelationId,                                          Is.Not.Null);

                        Assert.That(response3.Data, Is.Not.Null);
                        if (response3.Data is not null)
                        {

                            Assert.That(response3.Data.Count(),                      Is.EqualTo(1));

                            var firstTariff = response3.Data.FirstOrDefault();
                            if (firstTariff is not null)
                            {

                                Assert.That(firstTariff.Id,                           Is.EqualTo(Tariff_Id.Parse("TARIFF0001")));

                                Assert.That(firstTariff.TariffAltText. Count(),       Is.EqualTo(2));
                                Assert.That(firstTariff.TariffAltText.First().Text,   Is.EqualTo("Tariff 1c!"));

                                Assert.That(firstTariff.TariffElements.Count(),       Is.EqualTo(1));
                                var tariffElement1 = firstTariff.TariffElements.First();
                                //if (tariffElement1 is not null)
                                //{

                                    Assert.That(tariffElement1.PriceComponents.Count(),   Is.EqualTo(1));
                                    var priceComponent1 = tariffElement1.PriceComponents.First();

                                    //if (priceComponent1 is not null)
                                    //{

                                        Assert.That(priceComponent1.Price,   Is.EqualTo(3.00M));

                                    //}

                                //}

                            }

                        }

                        //Assert.That(response.Request, Is.Not.Null);

                    }

                });

                #endregion

            }

        }

        #endregion


        #region EMSP_GetTariff_Test1()

        /// <summary>
        /// EMSP GetTariff Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetTariff_Test1()
        {

            var graphDefinedCPO = emsp1EMSPAPI?.GetCPOClient(
                                      CountryCode:  CountryCode.Parse("DE"),
                                      PartyId:      Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                #region Define Tariff1

                await cpoCommonAPI.AddTariff(new Tariff(
                                                 CountryCode.Parse("DE"),
                                                 Party_Id.   Parse("GEF"),
                                                 Tariff_Id.  Parse("TARIFF0001"),
                                                 Currency.EUR,
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
                                                 LastUpdated: DateTime.Parse("2020-09-22").ToUniversalTime()
                                             ),
                                             SkipNotifications: true
                                         );

                #endregion

                var response = await graphDefinedCPO.GetTariff(Tariff_Id.Parse("TARIFF0001"));

                // GET /ocpi/v2.1/v2.1.1/cpo/locations/LOC0001 HTTP/1.1
                // Date:                           Wed, 26 Apr 2023 05:07:21 GMT
                // Accept:                         application/json; charset=utf-8;q=1
                // Host:                           localhost:3301
                // Authorization:                  Token emsp1_accessing_cpo++token
                // Connection:                     close
                // X-Request-ID:                   5UAb53YK9533x9r1vAGG8G22K5M861
                // X-Correlation-ID:               4M12E4Q4SMn8zMpxUxAz2d22b6n89j

                // HTTP/1.1 200 OK
                // Date:                           Wed, 26 Apr 2023 05:07:21 GMT
                // Server:                         GraphDefined OCPI CPO HTTP API v0.1
                // Access-Control-Allow-Methods:   OPTIONS, GET
                // Access-Control-Allow-Headers:   Authorization
                // Last-Modified:                  2020-09-21T00:00:00.000Z
                // ETag:                           9MRnEsmK/723od/Sw+67veAuGM+F6EVFGA2EaKrSg7E=
                // Access-Control-Allow-Origin:    *
                // Vary:                           Accept
                // Connection:                     close
                // Content-Type:                   application/json; charset=utf-8
                // Content-Length:                 5492
                // X-Request-ID:                   5UAb53YK9533x9r1vAGG8G22K5M861
                // X-Correlation-ID:               4M12E4Q4SMn8zMpxUxAz2d22b6n89j
                // 
                // {
                //     "data":            {"id":"LOC0001","location_type":"PARKING_LOT","name":"Location 0001","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"related_locations":[{"latitude":"11","longitude":"22","name":{"language":"de","text":"Postkasten"}}],"evses":[{"uid":"DE*GEF*E*LOC0001*1","evse_id":"DE*GEF*E*LOC0001*1","status":"AVAILABLE","status_schedule":[{"period_begin":"2020-09-22T22:00:00.000Z","period_end":"2020-09-23T22:00:00.000Z","status":"INOPERATIVE"},{"period_begin":"2020-12-29T23:00:00.000Z","period_end":"2020-12-30T23:00:00.000Z","status":"OUTOFORDER"}],"capabilities":["RFID_READER","RESERVABLE"],"connectors":[{"id":"1","standard":"IEC_62196_T2","format":"SOCKET","power_type":"AC_3_PHASE","voltage":400,"amperage":30,"tariff_id":"DE*GEF*T0001","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-20T22:00:00.000Z"},{"id":"2","standard":"IEC_62196_T2_COMBO","format":"CABLE","power_type":"AC_3_PHASE","voltage":400,"amperage":20,"tariff_id":"DE*GEF*T0003","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-21T22:00:00.000Z"}],"energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","manufacturer":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","certificate_issuer":"German PTB","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","certificate_issuer":"GraphDefined","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}],"last_updated":"2023-04-18T03:41:24.847Z"},"floor_level":"1. Stock","coordinates":{"latitude":"10.10000","longitude":"20.20000"},"physical_reference":"Ladestation #1","directions":[{"language":"de","text":"Bitte klingeln!"},{"language":"en","text":"Ken sent me!"}],"parking_restrictions":["EV_ONLY","PLUGGED"],"images":[{"url":"http://example.com/pinguine.jpg","category":"OPERATOR","type":"jpeg","thumbnail":"http://example.com/kleine_pinguine.jpg","width":100,"height":150}],"last_updated":"2020-09-21T22:00:00.000Z"}],"directions":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"operator":{"name":"Open Charging Cloud","website":"https://open.charging.cloud","logo":{"url":"http://open.charging.cloud/logo.svg","category":"OPERATOR","type":"svg","thumbnail":"http://open.charging.cloud/logo_small.svg","width":1000,"height":1500}},"suboperator":{"name":"GraphDefined GmbH","website":"https://www.graphdefined.com","logo":{"url":"http://www.graphdefined.com/logo.png","category":"OPERATOR","type":"png","thumbnail":"http://www.graphdefined.com/logo_small.png","width":2000,"height":3000}},"owner":{"name":"Achim Friedland","website":"https://ahzf.de","logo":{"url":"http://ahzf.de/logo.gif","category":"OWNER","type":"gif","thumbnail":"http://ahzf.de/logo_small.gif","width":3000,"height":4500}},"facilities":["CAFE"],"time_zone":"Europe/Berlin","opening_times":{"twentyfourseven":false,"regular_hours":[{"weekday":1,"period_begin":"08:00","period_end":"15:00"},{"weekday":2,"period_begin":"09:00","period_end":"16:00"},{"weekday":3,"period_begin":"10:00","period_end":"17:00"},{"weekday":4,"period_begin":"11:00","period_end":"18:00"},{"weekday":5,"period_begin":"12:00","period_end":"19:00"}],"exceptional_openings":[{"period_begin":"2020-09-21T00:00:00.000Z","period_end":"2020-09-22T00:00:00.000Z"}],"exceptional_closings":[{"period_begin":"2020-12-24T00:00:00.000Z","period_end":"2020-12-26T00:00:00.000Z"}]},"charging_when_closed":false,"images":[{"url":"http://open.charging.cloud/locations/location0001.jpg","category":"LOCATION","type":"jpeg","thumbnail":"http://open.charging.cloud/locations/location0001s.jpg","width":200,"height":400}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T00:00:00.000Z"},
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-26T05:07:21.108Z"}
                // }

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,           response.StatusCode);
                ClassicAssert.AreEqual ("Hello world!", response.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                ClassicAssert.IsNotNull(response.RequestId);
                ClassicAssert.IsNotNull(response.CorrelationId);
                ClassicAssert.IsNotNull(response.Data);

                if (response.Data is not null)
                {

                    ClassicAssert.IsTrue   (response.Data.Id == Tariff_Id.Parse("TARIFF0001"));

                    ClassicAssert.AreEqual (1, response.Data.TariffElements.Count());
                    ClassicAssert.AreEqual (2, response.Data.TariffAltText. Count());


                }

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region EMSP_GetTariff_Test2()

        /// <summary>
        /// EMSP GetTariff Test 2.
        /// </summary>
        [Test]
        public async Task EMSP_GetTariff_Test2()
        {

            var graphDefinedCPO = emsp2EMSPAPI?.GetCPOClient(
                                      CountryCode:  CountryCode.Parse("DE"),
                                      PartyId:      Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                #region Define Tariff1

                await cpoCommonAPI.AddTariff(new Tariff(
                                                 CountryCode.Parse("DE"),
                                                 Party_Id.   Parse("GEF"),
                                                 Tariff_Id.  Parse("TARIFF0001"),
                                                 Currency.EUR,
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
                                                 LastUpdated: DateTime.Parse("2020-09-22").ToUniversalTime()
                                             ),
                                             SkipNotifications: true
                                         );

                #endregion

                var response = await graphDefinedCPO.GetTariff(Tariff_Id.Parse("TARIFF0001"));

                // GET /ocpi/v2.1/v2.1.1/cpo/locations/LOC0001 HTTP/1.1
                // Date:                           Wed, 26 Apr 2023 05:07:21 GMT
                // Accept:                         application/json; charset=utf-8;q=1
                // Host:                           localhost:3301
                // Authorization:                  Token emsp1_accessing_cpo++token
                // Connection:                     close
                // X-Request-ID:                   5UAb53YK9533x9r1vAGG8G22K5M861
                // X-Correlation-ID:               4M12E4Q4SMn8zMpxUxAz2d22b6n89j

                // HTTP/1.1 200 OK
                // Date:                           Wed, 26 Apr 2023 05:07:21 GMT
                // Server:                         GraphDefined OCPI CPO HTTP API v0.1
                // Access-Control-Allow-Methods:   OPTIONS, GET
                // Access-Control-Allow-Headers:   Authorization
                // Last-Modified:                  2020-09-21T00:00:00.000Z
                // ETag:                           9MRnEsmK/723od/Sw+67veAuGM+F6EVFGA2EaKrSg7E=
                // Access-Control-Allow-Origin:    *
                // Vary:                           Accept
                // Connection:                     close
                // Content-Type:                   application/json; charset=utf-8
                // Content-Length:                 5492
                // X-Request-ID:                   5UAb53YK9533x9r1vAGG8G22K5M861
                // X-Correlation-ID:               4M12E4Q4SMn8zMpxUxAz2d22b6n89j
                // 
                // {
                //     "data":            {"id":"LOC0001","location_type":"PARKING_LOT","name":"Location 0001","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"related_locations":[{"latitude":"11","longitude":"22","name":{"language":"de","text":"Postkasten"}}],"evses":[{"uid":"DE*GEF*E*LOC0001*1","evse_id":"DE*GEF*E*LOC0001*1","status":"AVAILABLE","status_schedule":[{"period_begin":"2020-09-22T22:00:00.000Z","period_end":"2020-09-23T22:00:00.000Z","status":"INOPERATIVE"},{"period_begin":"2020-12-29T23:00:00.000Z","period_end":"2020-12-30T23:00:00.000Z","status":"OUTOFORDER"}],"capabilities":["RFID_READER","RESERVABLE"],"connectors":[{"id":"1","standard":"IEC_62196_T2","format":"SOCKET","power_type":"AC_3_PHASE","voltage":400,"amperage":30,"tariff_id":"DE*GEF*T0001","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-20T22:00:00.000Z"},{"id":"2","standard":"IEC_62196_T2_COMBO","format":"CABLE","power_type":"AC_3_PHASE","voltage":400,"amperage":20,"tariff_id":"DE*GEF*T0003","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-21T22:00:00.000Z"}],"energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","manufacturer":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","certificate_issuer":"German PTB","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","certificate_issuer":"GraphDefined","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}],"last_updated":"2023-04-18T03:41:24.847Z"},"floor_level":"1. Stock","coordinates":{"latitude":"10.10000","longitude":"20.20000"},"physical_reference":"Ladestation #1","directions":[{"language":"de","text":"Bitte klingeln!"},{"language":"en","text":"Ken sent me!"}],"parking_restrictions":["EV_ONLY","PLUGGED"],"images":[{"url":"http://example.com/pinguine.jpg","category":"OPERATOR","type":"jpeg","thumbnail":"http://example.com/kleine_pinguine.jpg","width":100,"height":150}],"last_updated":"2020-09-21T22:00:00.000Z"}],"directions":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"operator":{"name":"Open Charging Cloud","website":"https://open.charging.cloud","logo":{"url":"http://open.charging.cloud/logo.svg","category":"OPERATOR","type":"svg","thumbnail":"http://open.charging.cloud/logo_small.svg","width":1000,"height":1500}},"suboperator":{"name":"GraphDefined GmbH","website":"https://www.graphdefined.com","logo":{"url":"http://www.graphdefined.com/logo.png","category":"OPERATOR","type":"png","thumbnail":"http://www.graphdefined.com/logo_small.png","width":2000,"height":3000}},"owner":{"name":"Achim Friedland","website":"https://ahzf.de","logo":{"url":"http://ahzf.de/logo.gif","category":"OWNER","type":"gif","thumbnail":"http://ahzf.de/logo_small.gif","width":3000,"height":4500}},"facilities":["CAFE"],"time_zone":"Europe/Berlin","opening_times":{"twentyfourseven":false,"regular_hours":[{"weekday":1,"period_begin":"08:00","period_end":"15:00"},{"weekday":2,"period_begin":"09:00","period_end":"16:00"},{"weekday":3,"period_begin":"10:00","period_end":"17:00"},{"weekday":4,"period_begin":"11:00","period_end":"18:00"},{"weekday":5,"period_begin":"12:00","period_end":"19:00"}],"exceptional_openings":[{"period_begin":"2020-09-21T00:00:00.000Z","period_end":"2020-09-22T00:00:00.000Z"}],"exceptional_closings":[{"period_begin":"2020-12-24T00:00:00.000Z","period_end":"2020-12-26T00:00:00.000Z"}]},"charging_when_closed":false,"images":[{"url":"http://open.charging.cloud/locations/location0001.jpg","category":"LOCATION","type":"jpeg","thumbnail":"http://open.charging.cloud/locations/location0001s.jpg","width":200,"height":400}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T00:00:00.000Z"},
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-26T05:07:21.108Z"}
                // }

                ClassicAssert.IsNotNull(response);
                ClassicAssert.AreEqual (200,            response.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,           response.StatusCode);
                ClassicAssert.AreEqual ("Hello world!", response.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                ClassicAssert.IsNotNull(response.RequestId);
                ClassicAssert.IsNotNull(response.CorrelationId);
                ClassicAssert.IsNotNull(response.Data);

                if (response.Data is not null)
                {

                    ClassicAssert.IsTrue   (response.Data.Id == Tariff_Id.Parse("TARIFF0001"));

                    ClassicAssert.AreEqual (1, response.Data.TariffElements.Count());
                    ClassicAssert.AreEqual (2, response.Data.TariffAltText. Count());


                }

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region EMSP_GetTariff_Versioned_Test1()

        /// <summary>
        /// EMSP GetTariff versioned Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetTariff_Versioned_Test1()
        {

            var graphDefinedCPO = emsp1EMSPAPI?.GetCPOClient(
                                      CountryCode:  CountryCode.Parse("DE"),
                                      PartyId:      Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                // Tariff 1a
                var tariffSwitchTimestamp1 = DateTime.Parse("2024-01-01").ToUniversalTime();
                // Tariff 1b
                var tariffSwitchTimestamp2 = DateTime.Parse("2024-04-01").ToUniversalTime();
                // Tariff 1c

                #region Define Tariff 1a

                var addResult1 = await cpoCommonAPI.AddTariff(
                                           new Tariff(
                                               CountryCode.Parse("DE"),
                                               Party_Id.   Parse("GEF"),
                                               Tariff_Id.  Parse("TARIFF0001"),
                                               Currency.EUR,
                                               new[] {
                                                   new TariffElement(
                                                       new[] {
                                                           PriceComponent.ChargingTime(
                                                               1.00M,
                                                               TimeSpan.FromSeconds(300)
                                                           )
                                                       },
                                                       new TariffRestrictions(
                                                           Time.FromHourMin(08,00),                         // Start time
                                                           Time.FromHourMin(18,00),                         // End time
                                                           DateTime.Parse("2020-12-01").ToUniversalTime(),  // Start timestamp
                                                           DateTime.Parse("2020-12-31").ToUniversalTime(),  // End timestamp
                                                           1.12M,                                           // MinkWh
                                                           5.67M,                                           // MaxkWh
                                                           1.49M,                                           // MinPower
                                                           9.91M,                                           // MaxPower
                                                           TimeSpan.FromMinutes(10),                        // MinDuration
                                                           TimeSpan.FromMinutes(30),                        // MaxDuration
                                                           new[] {
                                                               DayOfWeek.Monday,
                                                               DayOfWeek.Tuesday
                                                           }
                                                       )
                                                   )
                                               },
                                               new[] {
                                                   new DisplayText(Languages.de, "Tariff 1a!"),
                                                   new DisplayText(Languages.en, "Tariff 1a!"),
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
                                               NotAfter:     tariffSwitchTimestamp1,
                                               LastUpdated:  DateTime.Parse("2024-02-10").ToUniversalTime()
                                           ),
                                           SkipNotifications: true
                                       );

                #endregion

                #region Define Tariff 1b

                var addResult2 = await cpoCommonAPI.AddTariff(
                                           new Tariff(
                                               CountryCode.Parse("DE"),
                                               Party_Id.   Parse("GEF"),
                                               Tariff_Id.  Parse("TARIFF0001"),
                                               Currency.EUR,
                                               new[] {
                                                   new TariffElement(
                                                       new[] {
                                                           PriceComponent.ChargingTime(
                                                               2.00M,
                                                               TimeSpan.FromSeconds(300)
                                                           )
                                                       },
                                                       new TariffRestrictions(
                                                           Time.FromHourMin(08,00),                         // Start time
                                                           Time.FromHourMin(18,00),                         // End time
                                                           DateTime.Parse("2020-12-01").ToUniversalTime(),  // Start timestamp
                                                           DateTime.Parse("2020-12-31").ToUniversalTime(),  // End timestamp
                                                           1.12M,                                           // MinkWh
                                                           5.67M,                                           // MaxkWh
                                                           1.49M,                                           // MinPower
                                                           9.91M,                                           // MaxPower
                                                           TimeSpan.FromMinutes(10),                        // MinDuration
                                                           TimeSpan.FromMinutes(30),                        // MaxDuration
                                                           new[] {
                                                               DayOfWeek.Monday,
                                                               DayOfWeek.Tuesday
                                                           }
                                                       )
                                                   )
                                               },
                                               new[] {
                                                   new DisplayText(Languages.de, "Tariff 1b!"),
                                                   new DisplayText(Languages.en, "Tariff 1b!"),
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
                                               NotBefore:    tariffSwitchTimestamp1,
                                               NotAfter:     tariffSwitchTimestamp2,
                                               LastUpdated:  DateTime.Parse("2024-02-10").ToUniversalTime()
                                           ),
                                           SkipNotifications: true
                                       );

                #endregion

                #region Define Tariff 1c

                var addResult3 = await cpoCommonAPI.AddTariff(
                                           new Tariff(
                                               CountryCode.Parse("DE"),
                                               Party_Id.   Parse("GEF"),
                                               Tariff_Id.  Parse("TARIFF0001"),
                                               Currency.EUR,
                                               new[] {
                                                   new TariffElement(
                                                       new[] {
                                                           PriceComponent.ChargingTime(
                                                               3.00M,
                                                               TimeSpan.FromSeconds(300)
                                                           )
                                                       },
                                                       new TariffRestrictions(
                                                           Time.FromHourMin(08,00),                         // Start time
                                                           Time.FromHourMin(18,00),                         // End time
                                                           DateTime.Parse("2020-12-01").ToUniversalTime(),  // Start timestamp
                                                           DateTime.Parse("2020-12-31").ToUniversalTime(),  // End timestamp
                                                           1.12M,                                           // MinkWh
                                                           5.67M,                                           // MaxkWh
                                                           1.49M,                                           // MinPower
                                                           9.91M,                                           // MaxPower
                                                           TimeSpan.FromMinutes(10),                        // MinDuration
                                                           TimeSpan.FromMinutes(30),                        // MaxDuration
                                                           new[] {
                                                               DayOfWeek.Monday,
                                                               DayOfWeek.Tuesday
                                                           }
                                                       )
                                                   )
                                               },
                                               new[] {
                                                   new DisplayText(Languages.de, "Tariff 1c!"),
                                                   new DisplayText(Languages.en, "Tariff 1c!"),
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
                                               NotBefore:    tariffSwitchTimestamp2,
                                               LastUpdated:  DateTime.Parse("2024-02-10").ToUniversalTime()
                                           ),
                                           SkipNotifications: true
                                       );

                #endregion


                #region Get tariff at timestamp before first tariff switch

                var response1 = await graphDefinedCPO.GetTariff(
                                                          TariffId:   Tariff_Id.Parse("TARIFF0001"),
                                                          Timestamp:  DateTime. Parse("2023-12-30").ToUniversalTime()
                                                      );

                Assert.Multiple(() => {

                    Assert.That(response1, Is.Not.Null);
                    if (response1 is not null)
                    {

                        Assert.That(response1.HTTPResponse?.HTTPStatusCode.Code,                      Is.EqualTo(200));
                        Assert.That(response1.StatusCode,                                             Is.EqualTo(1000));
                        Assert.That(response1.StatusMessage,                                          Is.EqualTo("Hello world!"));
                        Assert.That(Timestamp.Now - response1.Timestamp < TimeSpan.FromSeconds(10),   Is.True);

                        Assert.That(response1.RequestId,                                              Is.Not.Null);
                        Assert.That(response1.CorrelationId,                                          Is.Not.Null);

                        Assert.That(response1.Data, Is.Not.Null);
                        if (response1.Data is not null)
                        {

                            Assert.That(response1.Data.Id,                           Is.EqualTo(Tariff_Id.Parse("TARIFF0001")));

                            Assert.That(response1.Data.TariffAltText. Count(),       Is.EqualTo(2));
                            Assert.That(response1.Data.TariffAltText.First().Text,   Is.EqualTo("Tariff 1a!"));

                            Assert.That(response1.Data.TariffElements.Count(),       Is.EqualTo(1));
                            var tariffElement1 = response1.Data.TariffElements.First();
                            //if (tariffElement1 is not null)
                            //{

                                Assert.That(tariffElement1.PriceComponents.Count(),   Is.EqualTo(1));
                                var priceComponent1 = tariffElement1.PriceComponents.First();

                                //if (priceComponent1 is not null)
                                //{

                                    Assert.That(priceComponent1.Price,   Is.EqualTo(1.00M));

                                //}

                            //}

                        }

                        //Assert.That(response.Request, Is.Not.Null);

                    }

                });

                #endregion

                #region Get tariff at current timestamp

                var response2 = await graphDefinedCPO.GetTariff(Tariff_Id.Parse("TARIFF0001"));

                // GET /ocpi/v2.1/v2.1.1/cpo/locations/LOC0001 HTTP/1.1
                // Date:                           Wed, 26 Apr 2023 05:07:21 GMT
                // Accept:                         application/json; charset=utf-8;q=1
                // Host:                           localhost:3301
                // Authorization:                  Token emsp1_accessing_cpo++token
                // Connection:                     close
                // X-Request-ID:                   5UAb53YK9533x9r1vAGG8G22K5M861
                // X-Correlation-ID:               4M12E4Q4SMn8zMpxUxAz2d22b6n89j

                // HTTP/1.1 200 OK
                // Date:                           Wed, 26 Apr 2023 05:07:21 GMT
                // Server:                         GraphDefined OCPI CPO HTTP API v0.1
                // Access-Control-Allow-Methods:   OPTIONS, GET
                // Access-Control-Allow-Headers:   Authorization
                // Last-Modified:                  2020-09-21T00:00:00.000Z
                // ETag:                           9MRnEsmK/723od/Sw+67veAuGM+F6EVFGA2EaKrSg7E=
                // Access-Control-Allow-Origin:    *
                // Vary:                           Accept
                // Connection:                     close
                // Content-Type:                   application/json; charset=utf-8
                // Content-Length:                 5492
                // X-Request-ID:                   5UAb53YK9533x9r1vAGG8G22K5M861
                // X-Correlation-ID:               4M12E4Q4SMn8zMpxUxAz2d22b6n89j
                // 
                // {
                //     "data":            {"id":"LOC0001","location_type":"PARKING_LOT","name":"Location 0001","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"related_locations":[{"latitude":"11","longitude":"22","name":{"language":"de","text":"Postkasten"}}],"evses":[{"uid":"DE*GEF*E*LOC0001*1","evse_id":"DE*GEF*E*LOC0001*1","status":"AVAILABLE","status_schedule":[{"period_begin":"2020-09-22T22:00:00.000Z","period_end":"2020-09-23T22:00:00.000Z","status":"INOPERATIVE"},{"period_begin":"2020-12-29T23:00:00.000Z","period_end":"2020-12-30T23:00:00.000Z","status":"OUTOFORDER"}],"capabilities":["RFID_READER","RESERVABLE"],"connectors":[{"id":"1","standard":"IEC_62196_T2","format":"SOCKET","power_type":"AC_3_PHASE","voltage":400,"amperage":30,"tariff_id":"DE*GEF*T0001","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-20T22:00:00.000Z"},{"id":"2","standard":"IEC_62196_T2_COMBO","format":"CABLE","power_type":"AC_3_PHASE","voltage":400,"amperage":20,"tariff_id":"DE*GEF*T0003","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-21T22:00:00.000Z"}],"energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","manufacturer":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","certificate_issuer":"German PTB","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","certificate_issuer":"GraphDefined","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}],"last_updated":"2023-04-18T03:41:24.847Z"},"floor_level":"1. Stock","coordinates":{"latitude":"10.10000","longitude":"20.20000"},"physical_reference":"Ladestation #1","directions":[{"language":"de","text":"Bitte klingeln!"},{"language":"en","text":"Ken sent me!"}],"parking_restrictions":["EV_ONLY","PLUGGED"],"images":[{"url":"http://example.com/pinguine.jpg","category":"OPERATOR","type":"jpeg","thumbnail":"http://example.com/kleine_pinguine.jpg","width":100,"height":150}],"last_updated":"2020-09-21T22:00:00.000Z"}],"directions":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"operator":{"name":"Open Charging Cloud","website":"https://open.charging.cloud","logo":{"url":"http://open.charging.cloud/logo.svg","category":"OPERATOR","type":"svg","thumbnail":"http://open.charging.cloud/logo_small.svg","width":1000,"height":1500}},"suboperator":{"name":"GraphDefined GmbH","website":"https://www.graphdefined.com","logo":{"url":"http://www.graphdefined.com/logo.png","category":"OPERATOR","type":"png","thumbnail":"http://www.graphdefined.com/logo_small.png","width":2000,"height":3000}},"owner":{"name":"Achim Friedland","website":"https://ahzf.de","logo":{"url":"http://ahzf.de/logo.gif","category":"OWNER","type":"gif","thumbnail":"http://ahzf.de/logo_small.gif","width":3000,"height":4500}},"facilities":["CAFE"],"time_zone":"Europe/Berlin","opening_times":{"twentyfourseven":false,"regular_hours":[{"weekday":1,"period_begin":"08:00","period_end":"15:00"},{"weekday":2,"period_begin":"09:00","period_end":"16:00"},{"weekday":3,"period_begin":"10:00","period_end":"17:00"},{"weekday":4,"period_begin":"11:00","period_end":"18:00"},{"weekday":5,"period_begin":"12:00","period_end":"19:00"}],"exceptional_openings":[{"period_begin":"2020-09-21T00:00:00.000Z","period_end":"2020-09-22T00:00:00.000Z"}],"exceptional_closings":[{"period_begin":"2020-12-24T00:00:00.000Z","period_end":"2020-12-26T00:00:00.000Z"}]},"charging_when_closed":false,"images":[{"url":"http://open.charging.cloud/locations/location0001.jpg","category":"LOCATION","type":"jpeg","thumbnail":"http://open.charging.cloud/locations/location0001s.jpg","width":200,"height":400}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T00:00:00.000Z"},
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-26T05:07:21.108Z"}
                // }

                Assert.Multiple(() => {

                    Assert.That(response2, Is.Not.Null);
                    if (response2 is not null)
                    {

                        Assert.That(response2.HTTPResponse?.HTTPStatusCode.Code,                      Is.EqualTo(200));
                        Assert.That(response2.StatusCode,                                             Is.EqualTo(1000));
                        Assert.That(response2.StatusMessage,                                          Is.EqualTo("Hello world!"));
                        Assert.That(Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10),   Is.True);

                        Assert.That(response2.RequestId,                                              Is.Not.Null);
                        Assert.That(response2.CorrelationId,                                          Is.Not.Null);

                        Assert.That(response2.Data, Is.Not.Null);
                        if (response2.Data is not null)
                        {

                            Assert.That(response2.Data.Id,                           Is.EqualTo(Tariff_Id.Parse("TARIFF0001")));

                            Assert.That(response2.Data.TariffAltText. Count(),       Is.EqualTo(2));
                            Assert.That(response2.Data.TariffAltText.First().Text,   Is.EqualTo("Tariff 1b!"));

                            Assert.That(response2.Data.TariffElements.Count(),       Is.EqualTo(1));
                            var tariffElement1 = response2.Data.TariffElements.First();
                            //if (tariffElement1 is not null)
                            //{

                                Assert.That(tariffElement1.PriceComponents.Count(),   Is.EqualTo(1));
                                var priceComponent1 = tariffElement1.PriceComponents.First();

                                //if (priceComponent1 is not null)
                                //{

                                    Assert.That(priceComponent1.Price,   Is.EqualTo(2.00M));

                                //}

                            //}

                        }

                        //Assert.That(response.Request, Is.Not.Null);

                    }

                });

                #endregion

                #region Get tariff at timestamp after second tariff switch

                var response3 = await graphDefinedCPO.GetTariff(
                                                          TariffId:   Tariff_Id.Parse("TARIFF0001"),
                                                          Timestamp:  DateTime. Parse("2024-05-03").ToUniversalTime()
                                                      );

                Assert.Multiple(() => {

                    Assert.That(response3, Is.Not.Null);
                    if (response3 is not null)
                    {

                        Assert.That(response3.HTTPResponse?.HTTPStatusCode.Code,                      Is.EqualTo(200));
                        Assert.That(response3.StatusCode,                                             Is.EqualTo(1000));
                        Assert.That(response3.StatusMessage,                                          Is.EqualTo("Hello world!"));
                        Assert.That(Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10),   Is.True);

                        Assert.That(response3.RequestId,                                              Is.Not.Null);
                        Assert.That(response3.CorrelationId,                                          Is.Not.Null);

                        Assert.That(response3.Data, Is.Not.Null);
                        if (response3.Data is not null)
                        {

                            Assert.That(response3.Data.Id,                           Is.EqualTo(Tariff_Id.Parse("TARIFF0001")));

                            Assert.That(response3.Data.TariffAltText. Count(),       Is.EqualTo(2));
                            Assert.That(response3.Data.TariffAltText.First().Text,   Is.EqualTo("Tariff 1c!"));

                            Assert.That(response3.Data.TariffElements.Count(),       Is.EqualTo(1));
                            var tariffElement1 = response3.Data.TariffElements.First();
                            //if (tariffElement1 is not null)
                            //{

                                Assert.That(tariffElement1.PriceComponents.Count(),   Is.EqualTo(1));
                                var priceComponent1 = tariffElement1.PriceComponents.First();

                                //if (priceComponent1 is not null)
                                //{

                                    Assert.That(priceComponent1.Price,   Is.EqualTo(3.00M));

                                //}

                            //}

                        }

                        //Assert.That(response.Request, Is.Not.Null);

                    }

                });

                #endregion

            }

        }

        #endregion


    }

}
