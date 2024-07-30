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
    public class Locations_Tests : ANodeTests
    {

        #region CPO_PutLocation_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_PutLocation_Test()
        {

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                #region PUT Location

                var response3 = await graphDefinedEMSP.PutLocation(new Location(
                                                                       CountryCode.Parse("DE"),
                                                                       Party_Id.   Parse("GEF"),
                                                                       Location_Id.Parse("LOC0001"),
                                                                       LocationType.PARKING_LOT,
                                                                       "Biberweg 18",
                                                                       "Jena",
                                                                       "07749",
                                                                       Country.Germany,
                                                                       GeoCoordinate.Parse(10, 20),
                                                                       "Location 0001",
                                                                       [
                                                                           new AdditionalGeoLocation(
                                                                               Latitude. Parse(11),
                                                                               Longitude.Parse(22),
                                                                               Name: DisplayText.Create(Languages.de, "Postkasten")
                                                                           )
                                                                       ],
                                                                       new[] {
                                                                           new EVSE(
                                                                               EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                                                               StatusType.AVAILABLE,
                                                                               [
                                                                                   new Connector(
                                                                                       Connector_Id.Parse("1"),
                                                                                       ConnectorType.IEC_62196_T2,
                                                                                       ConnectorFormats.SOCKET,
                                                                                       PowerTypes.AC_3_PHASE,
                                                                                       Volt.  ParseV(400),
                                                                                       Ampere.ParseA(30),
                                                                                       Tariff_Id.Parse("DE*GEF*T0001"),
                                                                                       URL.Parse("https://open.charging.cloud/terms"),
                                                                                       DateTime.Parse("2020-09-21")
                                                                                   ),
                                                                                   new Connector(
                                                                                       Connector_Id.Parse("2"),
                                                                                       ConnectorType.IEC_62196_T2_COMBO,
                                                                                       ConnectorFormats.CABLE,
                                                                                       PowerTypes.AC_3_PHASE,
                                                                                       Volt.  ParseV(400),
                                                                                       Ampere.ParseA(20),
                                                                                       Tariff_Id.Parse("DE*GEF*T0003"),
                                                                                       URL.Parse("https://open.charging.cloud/terms"),
                                                                                       DateTime.Parse("2020-09-22")
                                                                                   )
                                                                               ],
                                                                               EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                                                               [
                                                                                   new StatusSchedule(
                                                                                       StatusType.INOPERATIVE,
                                                                                       DateTime.Parse("2020-09-23"),
                                                                                       DateTime.Parse("2020-09-24")
                                                                                   ),
                                                                                   new StatusSchedule(
                                                                                       StatusType.OUTOFORDER,
                                                                                       DateTime.Parse("2020-12-30"),
                                                                                       DateTime.Parse("2020-12-31")
                                                                                   )
                                                                               ],
                                                                               [
                                                                                   Capability.RFID_READER,
                                                                                   Capability.RESERVABLE
                                                                               ],

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

                                                                               "1. Stock",
                                                                               GeoCoordinate.Parse(10.1, 20.2),
                                                                               "Ladestation #1",
                                                                               new[] {
                                                                                   DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                                                                   DisplayText.Create(Languages.en, "Ken sent me!")
                                                                               },
                                                                               new[] {
                                                                                   ParkingRestrictions.EV_ONLY,
                                                                                   ParkingRestrictions.PLUGGED
                                                                               },
                                                                               new[] {
                                                                                   new Image(
                                                                                       URL.Parse("http://example.com/pinguine.jpg"),
                                                                                       ImageFileType.jpeg,
                                                                                       ImageCategory.OPERATOR,
                                                                                       100,
                                                                                       150,
                                                                                       URL.Parse("http://example.com/kleine_pinguine.jpg")
                                                                                   )
                                                                               },
                                                                               DateTime.Parse("2020-09-22")
                                                                           )
                                                                       },
                                                                       new[] {
                                                                           new DisplayText(Languages.de, "Hallo Welt!"),
                                                                           new DisplayText(Languages.en, "Hello world!")
                                                                       },
                                                                       new BusinessDetails(
                                                                           "Open Charging Cloud",
                                                                           URL.Parse("https://open.charging.cloud"),
                                                                           new Image(
                                                                               URL.Parse("http://open.charging.cloud/logo.svg"),
                                                                               ImageFileType.svg,
                                                                               ImageCategory.OPERATOR,
                                                                               1000,
                                                                               1500,
                                                                               URL.Parse("http://open.charging.cloud/logo_small.svg")
                                                                           )
                                                                       ),
                                                                       new BusinessDetails(
                                                                           "GraphDefined GmbH",
                                                                           URL.Parse("https://www.graphdefined.com"),
                                                                           new Image(
                                                                               URL.Parse("http://www.graphdefined.com/logo.png"),
                                                                               ImageFileType.png,
                                                                               ImageCategory.OPERATOR,
                                                                               2000,
                                                                               3000,
                                                                               URL.Parse("http://www.graphdefined.com/logo_small.png")
                                                                           )
                                                                       ),
                                                                       new BusinessDetails(
                                                                           "Achim Friedland",
                                                                           URL.Parse("https://ahzf.de"),
                                                                           new Image(
                                                                               URL.Parse("http://ahzf.de/logo.gif"),
                                                                               ImageFileType.gif,
                                                                               ImageCategory.OWNER,
                                                                               3000,
                                                                               4500,
                                                                               URL.Parse("http://ahzf.de/logo_small.gif")
                                                                           )
                                                                       ),
                                                                       new[] {
                                                                           Facilities.CAFE
                                                                       },
                                                                       "Europe/Berlin",
                                                                       new Hours(
                                                                           new[] {
                                                                               new OCPI.RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                                                               new OCPI.RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                                                               new OCPI.RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                                                               new OCPI.RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                                                               new OCPI.RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                                                           },
                                                                           new[] {
                                                                               new OCPI.ExceptionalPeriod(
                                                                                   DateTime.Parse("2020-09-21T00:00:00Z"),
                                                                                   DateTime.Parse("2020-09-22T00:00:00Z")
                                                                               )
                                                                           },
                                                                           new[] {
                                                                               new OCPI.ExceptionalPeriod(
                                                                                   DateTime.Parse("2020-12-24T00:00:00Z"),
                                                                                   DateTime.Parse("2020-12-26T00:00:00Z")
                                                                               )
                                                                           }
                                                                       ),
                                                                       false,
                                                                       new[] {
                                                                           new Image(
                                                                               URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                                                               ImageFileType.jpeg,
                                                                               ImageCategory.LOCATION,
                                                                               200,
                                                                               400,
                                                                               URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                                                           )
                                                                       },
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
                                                                       LastUpdated: DateTime.Parse("2020-09-21T00:00:00Z").ToUniversalTime()
                                                                   ));

                #endregion

                // HTTP/1.1 201 Created
                // Date:                          Wed, 28 Dec 2022 22:54:40 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, PUT, PATCH, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Last-Modified:                 2020-09-21T00:00:00.000Z
                // ETag:                          Q8R8UfdvHYd/jE3j+FjO0r84MiRmqt/8+PXxZ+Z43sg=
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                4932
                // X-Request-ID:                  G62h4r32Yd9rrQS579jWGpf49rS2Wz
                // X-Correlation-ID:              3xE3Y25Atv8633zYz3U6Qr945457pt
                // 
                // {
                //     "data":            {"id":"LOC0001","location_type":"PARKING_LOT","name":"Location 0001","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"related_locations":[{"latitude":"11","longitude":"22","name":{"language":"de","text":"Postkasten"}}],"evses":[{"uid":"DE*GEF*E*LOC0001*1","evse_id":"DE*GEF*E*LOC0001*1","status":"AVAILABLE","status_schedule":[{"period_begin":"2020-09-22T22:00:00.000Z","period_end":"2020-09-23T22:00:00.000Z","status":"INOPERATIVE"},{"period_begin":"2020-12-29T23:00:00.000Z","period_end":"2020-12-30T23:00:00.000Z","status":"OUTOFORDER"}],"capabilities":["RFID_READER","RESERVABLE"],"connectors":[{"id":"1","standard":"IEC_62196_T2","format":"SOCKET","power_type":"AC_3_PHASE","voltage":400,"amperage":30,"terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-20T22:00:00.000Z"},{"id":"2","standard":"IEC_62196_T2_COMBO","format":"CABLE","power_type":"AC_3_PHASE","voltage":400,"amperage":20,"terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-21T22:00:00.000Z"}],"energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","vendor":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}]},"floor_level":"1. Stock","coordinates":{"latitude":"10.10000","longitude":"20.20000"},"physical_reference":"Ladestation #1","directions":[{"language":"de","text":"Bitte klingeln!"},{"language":"en","text":"Ken sent me!"}],"parking_restrictions":["EV_ONLY","PLUGGED"],"images":[{"url":"http://example.com/pinguine.jpg","category":"OPERATOR","type":"jpeg","thumbnail":"http://example.com/kleine_pinguine.jpg","width":100,"height":150}],"last_updated":"2020-09-21T22:00:00.000Z"}],"directions":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"operator":{"name":"Open Charging Cloud","website":"https://open.charging.cloud","logo":{"url":"http://open.charging.cloud/logo.svg","category":"OPERATOR","type":"svg","thumbnail":"http://open.charging.cloud/logo_small.svg","width":1000,"height":1500}},"suboperator":{"name":"GraphDefined GmbH","website":"https://www.graphdefined.com","logo":{"url":"http://www.graphdefined.com/logo.png","category":"OPERATOR","type":"png","thumbnail":"http://www.graphdefined.com/logo_small.png","width":2000,"height":3000}},"owner":{"name":"Achim Friedland","website":"https://ahzf.de","logo":{"url":"http://ahzf.de/logo.gif","category":"OWNER","type":"gif","thumbnail":"http://ahzf.de/logo_small.gif","width":3000,"height":4500}},"facilities":["CAFE"],"time_zone":"Europe/Berlin","opening_times":{"twentyfourseven":false,"regular_hours":[{"weekday":1,"period_begin":"08:00","period_end":"15:00"},{"weekday":2,"period_begin":"09:00","period_end":"16:00"},{"weekday":3,"period_begin":"10:00","period_end":"17:00"},{"weekday":4,"period_begin":"11:00","period_end":"18:00"},{"weekday":5,"period_begin":"12:00","period_end":"19:00"}],"exceptional_openings":[{"period_begin":"2020-09-21T00:00:00.000Z","period_end":"2020-09-22T00:00:00.000Z"}],"exceptional_closings":[{"period_begin":"2020-12-24T00:00:00.000Z","period_end":"2020-12-26T00:00:00.000Z"}]},"charging_when_closed":false,"images":[{"url":"http://open.charging.cloud/locations/location0001.jpg","category":"LOCATION","type":"jpeg","thumbnail":"http://open.charging.cloud/locations/location0001s.jpg","width":200,"height":400}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T00:00:00.000Z"},
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-28T22:54:40.784Z"
                // }

                ClassicAssert.IsNotNull(response3);
                ClassicAssert.AreEqual (201,            response3.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,           response3.StatusCode);
                ClassicAssert.AreEqual ("Hello world!", response3.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now - response3.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region CPO_PutEVSE_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_PutEVSE_Test()
        {

            var graphDefinedEMSP = cpoCommonAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            ClassicAssert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                #region Put Location and EVSE

                var response3            = await graphDefinedEMSP.PutLocation(new Location(
                                                                                  CountryCode.Parse("DE"),
                                                                                  Party_Id.   Parse("GEF"),
                                                                                  Location_Id.Parse("LOC0001"),
                                                                                  LocationType.PARKING_LOT,
                                                                                  "Biberweg 18",
                                                                                  "Jena",
                                                                                  "07749",
                                                                                  Country.Germany,
                                                                                  GeoCoordinate.Parse(10, 20),
                                                                                  "Location 0001",
                                                                                  new[] {
                                                                                      new AdditionalGeoLocation(
                                                                                          Latitude.Parse(11),
                                                                                          Longitude.Parse(22),
                                                                                          Name: DisplayText.Create(Languages.de, "Postkasten")
                                                                                      )
                                                                                  },
                                                                                  new[] {
                                                                                      new EVSE(
                                                                                          EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                                                                          StatusType.AVAILABLE,
                                                                                          new[] {
                                                                                              new Connector(
                                                                                                  Connector_Id.Parse("1"),
                                                                                                  ConnectorType.IEC_62196_T2,
                                                                                                  ConnectorFormats.SOCKET,
                                                                                                  PowerTypes.AC_3_PHASE,
                                                                                                  Volt.  ParseV(400),
                                                                                                  Ampere.ParseA(30),
                                                                                                  Tariff_Id.Parse("DE*GEF*T0001"),
                                                                                                  URL.Parse("https://open.charging.cloud/terms"),
                                                                                                  DateTime.Parse("2020-09-21")
                                                                                              ),
                                                                                              new Connector(
                                                                                                  Connector_Id.Parse("2"),
                                                                                                  ConnectorType.IEC_62196_T2_COMBO,
                                                                                                  ConnectorFormats.CABLE,
                                                                                                  PowerTypes.AC_3_PHASE,
                                                                                                  Volt.  ParseV(400),
                                                                                                  Ampere.ParseA(20),
                                                                                                  Tariff_Id.Parse("DE*GEF*T0003"),
                                                                                                  URL.Parse("https://open.charging.cloud/terms"),
                                                                                                  DateTime.Parse("2020-09-22")
                                                                                              )
                                                                                          },
                                                                                          EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                                                                          new[] {
                                                                                              new StatusSchedule(
                                                                                                  StatusType.INOPERATIVE,
                                                                                                  DateTime.Parse("2020-09-23"),
                                                                                                  DateTime.Parse("2020-09-24")
                                                                                              ),
                                                                                              new StatusSchedule(
                                                                                                  StatusType.OUTOFORDER,
                                                                                                  DateTime.Parse("2020-12-30"),
                                                                                                  DateTime.Parse("2020-12-31")
                                                                                              )
                                                                                          },
                                                                                          new[] {
                                                                                              Capability.RFID_READER,
                                                                                              Capability.RESERVABLE
                                                                                          },

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

                                                                                          "1. Stock",
                                                                                          GeoCoordinate.Parse(10.1, 20.2),
                                                                                          "Ladestation #1",
                                                                                          new[] {
                                                                                              DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                                                                              DisplayText.Create(Languages.en, "Ken sent me!")
                                                                                          },
                                                                                          new[] {
                                                                                              ParkingRestrictions.EV_ONLY,
                                                                                              ParkingRestrictions.PLUGGED
                                                                                          },
                                                                                          new[] {
                                                                                              new Image(
                                                                                                  URL.Parse("http://example.com/pinguine.jpg"),
                                                                                                  ImageFileType.jpeg,
                                                                                                  ImageCategory.OPERATOR,
                                                                                                  100,
                                                                                                  150,
                                                                                                  URL.Parse("http://example.com/kleine_pinguine.jpg")
                                                                                              )
                                                                                          },
                                                                                          DateTime.Parse("2020-09-22")
                                                                                      )
                                                                                  },
                                                                                  new[] {
                                                                                      new DisplayText(Languages.de, "Hallo Welt!"),
                                                                                      new DisplayText(Languages.en, "Hello world!")
                                                                                  },
                                                                                  new BusinessDetails(
                                                                                      "Open Charging Cloud",
                                                                                      URL.Parse("https://open.charging.cloud"),
                                                                                      new Image(
                                                                                          URL.Parse("http://open.charging.cloud/logo.svg"),
                                                                                          ImageFileType.svg,
                                                                                          ImageCategory.OPERATOR,
                                                                                          1000,
                                                                                          1500,
                                                                                          URL.Parse("http://open.charging.cloud/logo_small.svg")
                                                                                      )
                                                                                  ),
                                                                                  new BusinessDetails(
                                                                                      "GraphDefined GmbH",
                                                                                      URL.Parse("https://www.graphdefined.com"),
                                                                                      new Image(
                                                                                          URL.Parse("http://www.graphdefined.com/logo.png"),
                                                                                          ImageFileType.png,
                                                                                          ImageCategory.OPERATOR,
                                                                                          2000,
                                                                                          3000,
                                                                                          URL.Parse("http://www.graphdefined.com/logo_small.png")
                                                                                      )
                                                                                  ),
                                                                                  new BusinessDetails(
                                                                                      "Achim Friedland",
                                                                                      URL.Parse("https://ahzf.de"),
                                                                                      new Image(
                                                                                          URL.Parse("http://ahzf.de/logo.gif"),
                                                                                          ImageFileType.gif,
                                                                                          ImageCategory.OWNER,
                                                                                          3000,
                                                                                          4500,
                                                                                          URL.Parse("http://ahzf.de/logo_small.gif")
                                                                                      )
                                                                                  ),
                                                                                  new[] {
                                                                                      Facilities.CAFE
                                                                                  },
                                                                                  "Europe/Berlin",
                                                                                  new Hours(
                                                                                      new[] {
                                                                                          new OCPI.RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                                                                          new OCPI.RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                                                                          new OCPI.RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                                                                          new OCPI.RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                                                                          new OCPI.RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                                                                      },
                                                                                      new[] {
                                                                                          new OCPI.ExceptionalPeriod(
                                                                                              DateTime.Parse("2020-09-21T00:00:00Z"),
                                                                                              DateTime.Parse("2020-09-22T00:00:00Z")
                                                                                          )
                                                                                      },
                                                                                      new[] {
                                                                                          new OCPI.ExceptionalPeriod(
                                                                                              DateTime.Parse("2020-12-24T00:00:00Z"),
                                                                                              DateTime.Parse("2020-12-26T00:00:00Z")
                                                                                          )
                                                                                      }
                                                                                  ),
                                                                                  false,
                                                                                  new[] {
                                                                                      new Image(
                                                                                          URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                                                                          ImageFileType.jpeg,
                                                                                          ImageCategory.LOCATION,
                                                                                          200,
                                                                                          400,
                                                                                          URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                                                                      )
                                                                                  },
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
                                                                                  LastUpdated: DateTime.Parse("2020-09-21T00:00:00Z").ToUniversalTime()
                                                                              ));

                var response4            = await graphDefinedEMSP.PutEVSE(new EVSE(
                                                                              EVSE_UId.Parse("DE*GEF*E*LOC0001*2"),
                                                                              StatusType.AVAILABLE,
                                                                              new[] {
                                                                                  new Connector(
                                                                                      Connector_Id.Parse("1"),
                                                                                      ConnectorType.CHADEMO,
                                                                                      ConnectorFormats.SOCKET,
                                                                                      PowerTypes.DC,
                                                                                      Volt.  ParseV(400),
                                                                                      Ampere.ParseA(30),
                                                                                      Tariff_Id.Parse("DE*GEF*T0003"),
                                                                                      URL.Parse("https://open.charging.cloud/terms"),
                                                                                      DateTime.Parse("2020-09-21")
                                                                                  ),
                                                                                  new Connector(
                                                                                      Connector_Id.Parse("2"),
                                                                                      ConnectorType.CHADEMO,
                                                                                      ConnectorFormats.CABLE,
                                                                                      PowerTypes.DC,
                                                                                      Volt.  ParseV(400),
                                                                                      Ampere.ParseA(20),
                                                                                      Tariff_Id.Parse("DE*GEF*T0004"),
                                                                                      URL.Parse("https://open.charging.cloud/terms"),
                                                                                      DateTime.Parse("2021-11-13")
                                                                                  )
                                                                              },
                                                                              EVSE_Id.Parse("DE*GEF*E*LOC0001*2"),
                                                                              new[] {
                                                                                  new StatusSchedule(
                                                                                      StatusType.INOPERATIVE,
                                                                                      DateTime.Parse("2021-11-23"),
                                                                                      DateTime.Parse("2021-11-24")
                                                                                  ),
                                                                                  new StatusSchedule(
                                                                                      StatusType.OUTOFORDER,
                                                                                      DateTime.Parse("2021-10-30"),
                                                                                      DateTime.Parse("2021-10-31")
                                                                                  )
                                                                              },
                                                                              new[] {
                                                                                  Capability.RFID_READER,
                                                                                  Capability.RESERVABLE
                                                                              },

                                                                              // OCPI Computer Science Extensions
                                                                              new EnergyMeter(
                                                                                  Meter_Id.Parse("Meter0815b"),
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

                                                                              "2. Stock",
                                                                              GeoCoordinate.Parse(10.1, 20.2),
                                                                              "Ladestation #2",
                                                                              new[] {
                                                                                  DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                                                                  DisplayText.Create(Languages.en, "Dave sent me!")
                                                                              },
                                                                              new[] {
                                                                                  ParkingRestrictions.EV_ONLY,
                                                                                  ParkingRestrictions.PLUGGED
                                                                              },
                                                                              new[] {
                                                                                  new Image(
                                                                                      URL.Parse("http://example.com/pinguine.jpg"),
                                                                                      ImageFileType.jpeg,
                                                                                      ImageCategory.OPERATOR,
                                                                                      100,
                                                                                      150,
                                                                                      URL.Parse("http://example.com/kleine_pinguine.jpg")
                                                                                  )
                                                                              },
                                                                              LastUpdated: DateTime.Parse("2020-09-22")
                                                                          ),
                                                                          Location_Id.Parse("LOC0001"));

                #endregion

                // HTTP/1.1 201 Created
                // Date:                          Wed, 28 Dec 2022 22:54:40 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, PUT, PATCH, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Last-Modified:                 2020-09-21T00:00:00.000Z
                // ETag:                          Q8R8UfdvHYd/jE3j+FjO0r84MiRmqt/8+PXxZ+Z43sg=
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                4932
                // X-Request-ID:                  G62h4r32Yd9rrQS579jWGpf49rS2Wz
                // X-Correlation-ID:              3xE3Y25Atv8633zYz3U6Qr945457pt
                // 
                // {
                //     "data":            {"id":"LOC0001","location_type":"PARKING_LOT","name":"Location 0001","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"related_locations":[{"latitude":"11","longitude":"22","name":{"language":"de","text":"Postkasten"}}],"evses":[{"uid":"DE*GEF*E*LOC0001*1","evse_id":"DE*GEF*E*LOC0001*1","status":"AVAILABLE","status_schedule":[{"period_begin":"2020-09-22T22:00:00.000Z","period_end":"2020-09-23T22:00:00.000Z","status":"INOPERATIVE"},{"period_begin":"2020-12-29T23:00:00.000Z","period_end":"2020-12-30T23:00:00.000Z","status":"OUTOFORDER"}],"capabilities":["RFID_READER","RESERVABLE"],"connectors":[{"id":"1","standard":"IEC_62196_T2","format":"SOCKET","power_type":"AC_3_PHASE","voltage":400,"amperage":30,"terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-20T22:00:00.000Z"},{"id":"2","standard":"IEC_62196_T2_COMBO","format":"CABLE","power_type":"AC_3_PHASE","voltage":400,"amperage":20,"terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-21T22:00:00.000Z"}],"energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","vendor":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":"GPL3","vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}]},"floor_level":"1. Stock","coordinates":{"latitude":"10.10000","longitude":"20.20000"},"physical_reference":"Ladestation #1","directions":[{"language":"de","text":"Bitte klingeln!"},{"language":"en","text":"Ken sent me!"}],"parking_restrictions":["EV_ONLY","PLUGGED"],"images":[{"url":"http://example.com/pinguine.jpg","category":"OPERATOR","type":"jpeg","thumbnail":"http://example.com/kleine_pinguine.jpg","width":100,"height":150}],"last_updated":"2020-09-21T22:00:00.000Z"}],"directions":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"operator":{"name":"Open Charging Cloud","website":"https://open.charging.cloud","logo":{"url":"http://open.charging.cloud/logo.svg","category":"OPERATOR","type":"svg","thumbnail":"http://open.charging.cloud/logo_small.svg","width":1000,"height":1500}},"suboperator":{"name":"GraphDefined GmbH","website":"https://www.graphdefined.com","logo":{"url":"http://www.graphdefined.com/logo.png","category":"OPERATOR","type":"png","thumbnail":"http://www.graphdefined.com/logo_small.png","width":2000,"height":3000}},"owner":{"name":"Achim Friedland","website":"https://ahzf.de","logo":{"url":"http://ahzf.de/logo.gif","category":"OWNER","type":"gif","thumbnail":"http://ahzf.de/logo_small.gif","width":3000,"height":4500}},"facilities":["CAFE"],"time_zone":"Europe/Berlin","opening_times":{"twentyfourseven":false,"regular_hours":[{"weekday":1,"period_begin":"08:00","period_end":"15:00"},{"weekday":2,"period_begin":"09:00","period_end":"16:00"},{"weekday":3,"period_begin":"10:00","period_end":"17:00"},{"weekday":4,"period_begin":"11:00","period_end":"18:00"},{"weekday":5,"period_begin":"12:00","period_end":"19:00"}],"exceptional_openings":[{"period_begin":"2020-09-21T00:00:00.000Z","period_end":"2020-09-22T00:00:00.000Z"}],"exceptional_closings":[{"period_begin":"2020-12-24T00:00:00.000Z","period_end":"2020-12-26T00:00:00.000Z"}]},"charging_when_closed":false,"images":[{"url":"http://open.charging.cloud/locations/location0001.jpg","category":"LOCATION","type":"jpeg","thumbnail":"http://open.charging.cloud/locations/location0001s.jpg","width":200,"height":400}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T00:00:00.000Z"},
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-28T22:54:40.784Z"
                // }

                ClassicAssert.IsNotNull(response4);
                ClassicAssert.AreEqual (201,             response4.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,            response4.StatusCode);
                ClassicAssert.AreEqual ("Hello world!",  response4.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now -  response4.Timestamp < TimeSpan.FromSeconds(10));

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion


    }

}
