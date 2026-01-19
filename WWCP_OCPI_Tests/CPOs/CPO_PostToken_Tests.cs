/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI.UnitTests;

#endregion

namespace cloud.charging.open.protocols.OCPI.CPO.UnitTests
{

    [TestFixture]
    public class CPO_PostToken_Tests()

        : A_2CPOs2EMSPs_TestDefaults()

    {

        #region CPO1_PostToken_OCPIv2_1_1_Test()

        /// <summary>
        /// CPO1_PostToken_OCPIv2_1_1_Test.
        /// </summary>
        [Test]
        public async Task CPO1_PostToken_OCPIv2_1_1_Test()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_1_1?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var rfidUID   = Token_Id.NewRandom7;
                var response  = await graphDefinedEMSP1.PostToken(
                                          rfidUID,
                                          TokenType.RFID,
                                          new OCPIv2_1_1.LocationReference(
                                              Location_Id.Parse("DE*GDF*P*LOC0001"),
                                              [
                                                  EVSE_UId.Parse("DE*GDF*E*LOC0001*1"),
                                                  EVSE_UId.Parse("DE*GDF*E*LOC0001*2")
                                              ]
                                          )
                                      );

                // HTTP/1.1 201 Created
                // Date:                          Wed, 28 Dec 2022 23:20:07 GMT
                // Location:                      /2.2/emsp/cdrs/CDR0001
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

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.StatusCode,                                            Is.EqualTo(2004), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Unknown token!"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(404),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                //if (response.Data is not null)
                //{
                //}

            }

        }

        #endregion

        #region CPO1_PostToken_OCPIv2_2_1_Test()

        /// <summary>
        /// CPO1_PostToken_OCPIv2_2_1_Test.
        /// </summary>
        [Test]
        public async Task CPO1_PostToken_OCPIv2_2_1_Test()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_2_1?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var rfidUID   = Token_Id.NewRandom7;
                var response  = await graphDefinedEMSP1.PostToken(
                                          rfidUID,
                                          TokenType.RFID,
                                          new OCPIv2_2_1.LocationReference(
                                              Location_Id.Parse("DE*GDF*P*LOC0001"),
                                              [
                                                  EVSE_UId.Parse("DE*GDF*E*LOC0001*1"),
                                                  EVSE_UId.Parse("DE*GDF*E*LOC0001*2")
                                              ]
                                          )
                                      );

                // HTTP/1.1 201 Created
                // Date:                          Wed, 28 Dec 2022 23:20:07 GMT
                // Location:                      /2.2/emsp/cdrs/CDR0001
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

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.StatusCode,                                            Is.EqualTo(2004), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Unknown token!"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(404),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {
                }

            }

        }

        #endregion

        #region CPO1_PostToken_OCPIv2_3_0_Test()

        /// <summary>
        /// CPO1_PostToken_OCPIv2_3_0_Test.
        /// </summary>
        [Test]
        public async Task CPO1_PostToken_OCPIv2_3_0_Test()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_3_0?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var rfidUID   = Token_Id.NewRandom7;
                var response  = await graphDefinedEMSP1.PostToken(
                                          rfidUID,
                                          TokenType.RFID,
                                          new OCPIv2_3_0.LocationReference(
                                              Location_Id.Parse("DE*GDF*P*LOC0001"),
                                              [
                                                  EVSE_UId.Parse("DE*GDF*E*LOC0001*1"),
                                                  EVSE_UId.Parse("DE*GDF*E*LOC0001*2")
                                              ]
                                          )
                                      );

                // HTTP/1.1 201 Created
                // Date:                          Wed, 28 Dec 2022 23:20:07 GMT
                // Location:                      /2.2/emsp/cdrs/CDR0001
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

                Assert.That(response,                                                       Is.Not.Null);
                Assert.That(response.StatusCode,                                            Is.EqualTo(2004), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Unknown token!"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(404),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {
                }

            }

        }

        #endregion

    }

}
