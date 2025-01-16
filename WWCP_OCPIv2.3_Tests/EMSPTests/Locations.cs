/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using NUnit.Framework;
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.EMSPTests
{

    [TestFixture]
    public class Locations : ANodeTests
    {

        #region EMSP_GetLocations_Test1()

        /// <summary>
        /// EMSP GetLocations Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetLocations_Test1()
        {

            var graphDefinedCPO = emsp1CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                #region Define Location1

                await cpoCommonAPI.AddLocation(
                          new Location(
                              CountryCode.Parse("DE"),
                              Party_Id.Parse("GEF"),
                              Location_Id.Parse("LOC0001"),
                              true,
                              "Biberweg 18",
                              "Jena",
                              Country.Germany,
                              GeoCoordinate.Parse(10, 20),
                              "Europe/Berlin",
                              null,
                              "Location 0001",
                              "07749",
                              "Thüringen",
                              [
                                  new AdditionalGeoLocation(
                                      Latitude.Parse(11),
                                      Longitude.Parse(22),
                                      Name: DisplayText.Create(Languages.de, "Postkasten")
                                  )
                              ],
                              ParkingType.PARKING_LOT,
                              [
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
                                              Watt.  ParseW(12),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0001"),
                                                  Tariff_Id.Parse("DE*GEF*T0002")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-21"),
                                              DateTime.Parse("2020-09-21")
                                          ),
                                          new Connector(
                                              Connector_Id.Parse("2"),
                                              ConnectorType.IEC_62196_T2_COMBO,
                                              ConnectorFormats.CABLE,
                                              PowerTypes.AC_3_PHASE,
                                              Volt.  ParseV(400),
                                              Ampere.ParseA(20),
                                              Watt.  ParseW(8),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0003"),
                                                  Tariff_Id.Parse("DE*GEF*T0004")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_20_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-22"),
                                              DateTime.Parse("2020-09-22")
                                          )
                                      ],
                                      [ Parking_Id.Parse("1") ],
                                      [ VehicleType.PERSONAL_VEHICLE ],

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
                                      new EnergyMeter<EVSE>(
                                          EnergyMeter_Id.Parse("Meter0815"),
                                          "EnergyMeter Model #1",
                                          null,
                                          "hw. v1.80",
                                          "fw. v1.20",
                                          "Energy Metering Services",
                                          null,
                                          null,
                                          null
                                      ),

                                      "1. Stock",
                                      GeoCoordinate.Parse(10.1, 20.2),
                                      "Ladestation #1",
                                      [
                                          DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                          DisplayText.Create(Languages.en, "Ken sent me!")
                                      ],
                                      [
                                          new Image(
                                              URL.Parse("http://example.com/pinguine.jpg"),
                                              ImageFileType.jpeg,
                                              ImageCategory.OPERATOR,
                                              100,
                                              150,
                                              URL.Parse("http://example.com/kleine_pinguine.jpg")
                                          )
                                      ],
                                      [ EMSP_Id.Parse("DE*GDF") ],
                                      DateTime.Parse("2020-09-22"),
                                      DateTime.Parse("2020-09-22")
                                  )
                              ],
                              [
                                  new Parking(
                                      Id:                      Parking_Id.Parse("1"),
                                      VehicleTypes:            [ VehicleType.PERSONAL_VEHICLE ],
                                      RestrictedToType:        false,
                                      ReservationRequired:     false,
                                      MaxVehicleWeight:        null,
                                      MaxVehicleHeight:        null,
                                      MaxVehicleLength:        null,
                                      MaxVehicleWidth:         null,
                                      ParkingSpaceLength:      null,
                                      ParkingSpaceWidth:       null,
                                      DangerousGoodsAllowed:   null,
                                      EVSEPosition:            null,
                                      Direction:               null,
                                      DriveThrough:            null,
                                      ParkingRestrictions:     null,
                                      TimeLimit:               null,
                                      Roofed:                  null,
                                      Images:                  null,
                                      Lighting:                null,
                                      RefrigerationOutlet:     null,
                                      Standards:               null,
                                      APDSReference:           null
                                  )
                              ],
                              [
                                  new DisplayText(Languages.de, "Hallo Welt!"),
                                  new DisplayText(Languages.en, "Hello world!")
                              ],
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
                              [ Facilities.CAFE ],
                              new Hours(
                                  [
                                      new OCPI.RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-09-21T00:00:00Z"),
                                          DateTime.Parse("2020-09-22T00:00:00Z")
                                      )
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-12-24T00:00:00Z"),
                                          DateTime.Parse("2020-12-26T00:00:00Z")
                                      )
                                  ]
                              ),
                              false,
                              [
                                  new Image(
                                      URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                      ImageFileType.jpeg,
                                      ImageCategory.LOCATION,
                                      200,
                                      400,
                                      URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                  )
                              ],
                              new(
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
                              Created: DateTime.Parse("2020-09-21T00:00:00Z")
                          ),
                          SkipNotifications: true
                      );

                #endregion


                var response = await graphDefinedCPO.GetLocations();

                // GET /2.2.1/cpo/locations HTTP/1.1
                // Date:                          Tue, 18 Apr 2023 03:41:25 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
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

                    ClassicAssert.IsTrue   (response.Data.First().EVSEs.First().Connectors.First().TariffIds.Contains(Tariff_Id.Parse("DE*GEF*T0001")));

                    ClassicAssert.AreEqual (1, response.Data.Count());

                }

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region EMSP_GetLocations_Test2()

        /// <summary>
        /// EMSP GetLocations Test 2.
        /// </summary>
        [Test]
        public async Task EMSP_GetLocations_Test2()
        {

            var graphDefinedCPO = emsp2CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                #region Define Location1

                await cpoCommonAPI.AddLocation(
                          new Location(
                              CountryCode.Parse("DE"),
                              Party_Id.Parse("GEF"),
                              Location_Id.Parse("LOC0001"),
                              true,
                              "Biberweg 18",
                              "Jena",
                              Country.Germany,
                              GeoCoordinate.Parse(10, 20),
                              "Europe/Berlin",
                              null,
                              "Location 0001",
                              "07749",
                              "Thüringen",
                              [
                                  new AdditionalGeoLocation(
                                      Latitude.Parse(11),
                                      Longitude.Parse(22),
                                      Name: DisplayText.Create(Languages.de, "Postkasten")
                                  )
                              ],
                              ParkingType.PARKING_LOT,
                              [
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
                                              Watt.  ParseW(12),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0001"),
                                                  Tariff_Id.Parse("DE*GEF*T0002")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-21"),
                                              DateTime.Parse("2020-09-21")
                                          ),
                                          new Connector(
                                              Connector_Id.Parse("2"),
                                              ConnectorType.IEC_62196_T2_COMBO,
                                              ConnectorFormats.CABLE,
                                              PowerTypes.AC_3_PHASE,
                                              Volt.  ParseV(400),
                                              Ampere.ParseA(20),
                                              Watt.  ParseW(8),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0003"),
                                                  Tariff_Id.Parse("DE*GEF*T0004")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_20_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-22"),
                                              DateTime.Parse("2020-09-22")
                                          )
                                      ],
                                      [ Parking_Id.Parse("1") ],
                                      [ VehicleType.PERSONAL_VEHICLE ],

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
                                      new EnergyMeter<EVSE>(
                                          EnergyMeter_Id.Parse("Meter0815"),
                                          "EnergyMeter Model #1",
                                          null,
                                          "hw. v1.80",
                                          "fw. v1.20",
                                          "Energy Metering Services",
                                          null,
                                          null,
                                          null
                                      ),

                                      "1. Stock",
                                      GeoCoordinate.Parse(10.1, 20.2),
                                      "Ladestation #1",
                                      [
                                          DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                          DisplayText.Create(Languages.en, "Ken sent me!")
                                      ],
                                      [
                                          new Image(
                                              URL.Parse("http://example.com/pinguine.jpg"),
                                              ImageFileType.jpeg,
                                              ImageCategory.OPERATOR,
                                              100,
                                              150,
                                              URL.Parse("http://example.com/kleine_pinguine.jpg")
                                          )
                                      ],
                                      [ EMSP_Id.Parse("DE*GDF") ],
                                      DateTime.Parse("2020-09-22"),
                                      DateTime.Parse("2020-09-22")
                                  )
                              ],
                              [
                                  new Parking(
                                      Id:                      Parking_Id.Parse("1"),
                                      VehicleTypes:            [ VehicleType.PERSONAL_VEHICLE ],
                                      RestrictedToType:        false,
                                      ReservationRequired:     false,
                                      MaxVehicleWeight:        null,
                                      MaxVehicleHeight:        null,
                                      MaxVehicleLength:        null,
                                      MaxVehicleWidth:         null,
                                      ParkingSpaceLength:      null,
                                      ParkingSpaceWidth:       null,
                                      DangerousGoodsAllowed:   null,
                                      EVSEPosition:            null,
                                      Direction:               null,
                                      DriveThrough:            null,
                                      ParkingRestrictions:     null,
                                      TimeLimit:               null,
                                      Roofed:                  null,
                                      Images:                  null,
                                      Lighting:                null,
                                      RefrigerationOutlet:     null,
                                      Standards:               null,
                                      APDSReference:           null
                                  )
                              ],
                              [
                                  new DisplayText(Languages.de, "Hallo Welt!"),
                                  new DisplayText(Languages.en, "Hello world!")
                              ],
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
                              [ Facilities.CAFE ],
                              new Hours(
                                  [
                                      new OCPI.RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-09-21T00:00:00Z"),
                                          DateTime.Parse("2020-09-22T00:00:00Z")
                                      )
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-12-24T00:00:00Z"),
                                          DateTime.Parse("2020-12-26T00:00:00Z")
                                      )
                                  ]
                              ),
                              false,
                              [
                                  new Image(
                                      URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                      ImageFileType.jpeg,
                                      ImageCategory.LOCATION,
                                      200,
                                      400,
                                      URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                  )
                              ],
                              new(
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
                              Created: DateTime.Parse("2020-09-21T00:00:00Z")
                          ),
                          SkipNotifications: true
                      );

                #endregion


                var response = await graphDefinedCPO.GetLocations();

                // GET /2.2.1/cpo/locations HTTP/1.1
                // Date:                          Tue, 18 Apr 2023 03:41:25 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
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

                    ClassicAssert.IsTrue   (response.Data.First().EVSEs.First().Connectors.First().TariffIds.Contains(Tariff_Id.Parse("DE*GEF*T0001")));

                    ClassicAssert.AreEqual (1, response.Data.Count());

                }

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region EMSP_GetLocations_TariffDelegate_Test1()

        /// <summary>
        /// EMSP GetLocations using a tariff delegate, Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetLocations_TariffDelegate_Test1()
        {

            var graphDefinedCPO = emsp1CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                cpoCommonAPI.GetTariffIdsDelegate += (CPOCountryCode,
                                                             CPOPartyId,
                                                             Location,
                                                             EVSEUId,
                                                             ConnectorId,
                                                             EMPId) => {

                    if (EMPId.ToString() == "DE-GDF")
                        return [ Tariff_Id.Parse("AC1") ];

                    // Will be called during a lot of internal calculations!
                    else
                        return [];

                };

                #region Define Location1

                await cpoCommonAPI.AddLocation(
                          new Location(
                              CountryCode.Parse("DE"),
                              Party_Id.Parse("GEF"),
                              Location_Id.Parse("LOC0001"),
                              true,
                              "Biberweg 18",
                              "Jena",
                              Country.Germany,
                              GeoCoordinate.Parse(10, 20),
                              "Europe/Berlin",
                              null,
                              "Location 0001",
                              "07749",
                              "Thüringen",
                              [
                                  new AdditionalGeoLocation(
                                      Latitude.Parse(11),
                                      Longitude.Parse(22),
                                      Name: DisplayText.Create(Languages.de, "Postkasten")
                                  )
                              ],
                              ParkingType.PARKING_LOT,
                              [
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
                                              Watt.  ParseW(12),
                                              null,
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-21"),
                                              DateTime.Parse("2020-09-21")
                                          ),
                                          new Connector(
                                              Connector_Id.Parse("2"),
                                              ConnectorType.IEC_62196_T2_COMBO,
                                              ConnectorFormats.CABLE,
                                              PowerTypes.AC_3_PHASE,
                                              Volt.  ParseV(400),
                                              Ampere.ParseA(20),
                                              Watt.  ParseW(8),
                                              null,
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_20_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-22"),
                                              DateTime.Parse("2020-09-22")
                                          )
                                      ],
                                      [ Parking_Id.Parse("1") ],
                                      [ VehicleType.PERSONAL_VEHICLE ],

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
                                      new EnergyMeter<EVSE>(
                                          EnergyMeter_Id.Parse("Meter0815"),
                                          "EnergyMeter Model #1",
                                          null,
                                          "hw. v1.80",
                                          "fw. v1.20",
                                          "Energy Metering Services",
                                          null,
                                          null,
                                          null
                                      ),

                                      "1. Stock",
                                      GeoCoordinate.Parse(10.1, 20.2),
                                      "Ladestation #1",
                                      [
                                          DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                          DisplayText.Create(Languages.en, "Ken sent me!")
                                      ],
                                      [
                                          new Image(
                                              URL.Parse("http://example.com/pinguine.jpg"),
                                              ImageFileType.jpeg,
                                              ImageCategory.OPERATOR,
                                              100,
                                              150,
                                              URL.Parse("http://example.com/kleine_pinguine.jpg")
                                          )
                                      ],
                                      [ EMSP_Id.Parse("DE*GDF") ],
                                      DateTime.Parse("2020-09-22"),
                                      DateTime.Parse("2020-09-22")
                                  )
                              ],
                              [
                                  new Parking(
                                      Id:                      Parking_Id.Parse("1"),
                                      VehicleTypes:            [ VehicleType.PERSONAL_VEHICLE ],
                                      RestrictedToType:        false,
                                      ReservationRequired:     false,
                                      MaxVehicleWeight:        null,
                                      MaxVehicleHeight:        null,
                                      MaxVehicleLength:        null,
                                      MaxVehicleWidth:         null,
                                      ParkingSpaceLength:      null,
                                      ParkingSpaceWidth:       null,
                                      DangerousGoodsAllowed:   null,
                                      EVSEPosition:            null,
                                      Direction:               null,
                                      DriveThrough:            null,
                                      ParkingRestrictions:     null,
                                      TimeLimit:               null,
                                      Roofed:                  null,
                                      Images:                  null,
                                      Lighting:                null,
                                      RefrigerationOutlet:     null,
                                      Standards:               null,
                                      APDSReference:           null
                                  )
                              ],
                              [
                                  new DisplayText(Languages.de, "Hallo Welt!"),
                                  new DisplayText(Languages.en, "Hello world!")
                              ],
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
                              [ Facilities.CAFE ],
                              new Hours(
                                  [
                                      new OCPI.RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-09-21T00:00:00Z"),
                                          DateTime.Parse("2020-09-22T00:00:00Z")
                                      )
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-12-24T00:00:00Z"),
                                          DateTime.Parse("2020-12-26T00:00:00Z")
                                      )
                                  ]
                              ),
                              false,
                              [
                                  new Image(
                                      URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                      ImageFileType.jpeg,
                                      ImageCategory.LOCATION,
                                      200,
                                      400,
                                      URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                  )
                              ],
                              new(
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
                              Created: DateTime.Parse("2020-09-21T00:00:00Z")
                          ),
                          SkipNotifications: true
                      );

                #endregion


                var response = await graphDefinedCPO.GetLocations();

                // GET /2.2.1/cpo/locations HTTP/1.1
                // Date:                          Tue, 18 Apr 2023 03:41:25 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
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
                ClassicAssert.AreEqual (200,             response.HTTPResponse?.HTTPStatusCode.Code);
                ClassicAssert.AreEqual (1000,            response.StatusCode);
                ClassicAssert.AreEqual ("Hello world!",  response.StatusMessage);
                ClassicAssert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

                ClassicAssert.IsNotNull(response.RequestId);
                ClassicAssert.IsNotNull(response.CorrelationId);
                ClassicAssert.IsNotNull(response.Data);

                if (response.Data is not null)
                {

                    ClassicAssert.IsTrue   (response.Data.First().EVSEs.First().Connectors.First().TariffIds.Contains(Tariff_Id.Parse("AC1")));

                    ClassicAssert.AreEqual (1, response.Data.Count());

                }

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion


        #region EMSP_GetLocation_Test1()

        /// <summary>
        /// EMSP GetLocation Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetLocation_Test1()
        {

            var graphDefinedCPO = emsp1CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                #region Define Location1

                await cpoCommonAPI.AddLocation(
                          new Location(
                              CountryCode.Parse("DE"),
                              Party_Id.Parse("GEF"),
                              Location_Id.Parse("LOC0001"),
                              true,
                              "Biberweg 18",
                              "Jena",
                              Country.Germany,
                              GeoCoordinate.Parse(10, 20),
                              "Europe/Berlin",
                              null,
                              "Location 0001",
                              "07749",
                              "Thüringen",
                              [
                                  new AdditionalGeoLocation(
                                      Latitude.Parse(11),
                                      Longitude.Parse(22),
                                      Name: DisplayText.Create(Languages.de, "Postkasten")
                                  )
                              ],
                              ParkingType.PARKING_LOT,
                              [
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
                                              Watt.  ParseW(12),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0001"),
                                                  Tariff_Id.Parse("DE*GEF*T0002")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-21"),
                                              DateTime.Parse("2020-09-21")
                                          ),
                                          new Connector(
                                              Connector_Id.Parse("2"),
                                              ConnectorType.IEC_62196_T2_COMBO,
                                              ConnectorFormats.CABLE,
                                              PowerTypes.AC_3_PHASE,
                                              Volt.  ParseV(400),
                                              Ampere.ParseA(20),
                                              Watt.  ParseW(8),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0003"),
                                                  Tariff_Id.Parse("DE*GEF*T0004")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_20_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-22"),
                                              DateTime.Parse("2020-09-22")
                                          )
                                      ],
                                      [ Parking_Id.Parse("1") ],
                                      [ VehicleType.PERSONAL_VEHICLE ],

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
                                      new EnergyMeter<EVSE>(
                                          EnergyMeter_Id.Parse("Meter0815"),
                                          "EnergyMeter Model #1",
                                          null,
                                          "hw. v1.80",
                                          "fw. v1.20",
                                          "Energy Metering Services",
                                          null,
                                          null,
                                          null
                                      ),

                                      "1. Stock",
                                      GeoCoordinate.Parse(10.1, 20.2),
                                      "Ladestation #1",
                                      [
                                          DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                          DisplayText.Create(Languages.en, "Ken sent me!")
                                      ],
                                      [
                                          new Image(
                                              URL.Parse("http://example.com/pinguine.jpg"),
                                              ImageFileType.jpeg,
                                              ImageCategory.OPERATOR,
                                              100,
                                              150,
                                              URL.Parse("http://example.com/kleine_pinguine.jpg")
                                          )
                                      ],
                                      [ EMSP_Id.Parse("DE*GDF") ],
                                      DateTime.Parse("2020-09-22"),
                                      DateTime.Parse("2020-09-22")
                                  )
                              ],
                              [
                                  new Parking(
                                      Id:                      Parking_Id.Parse("1"),
                                      VehicleTypes:            [ VehicleType.PERSONAL_VEHICLE ],
                                      RestrictedToType:        false,
                                      ReservationRequired:     false,
                                      MaxVehicleWeight:        null,
                                      MaxVehicleHeight:        null,
                                      MaxVehicleLength:        null,
                                      MaxVehicleWidth:         null,
                                      ParkingSpaceLength:      null,
                                      ParkingSpaceWidth:       null,
                                      DangerousGoodsAllowed:   null,
                                      EVSEPosition:            null,
                                      Direction:               null,
                                      DriveThrough:            null,
                                      ParkingRestrictions:     null,
                                      TimeLimit:               null,
                                      Roofed:                  null,
                                      Images:                  null,
                                      Lighting:                null,
                                      RefrigerationOutlet:     null,
                                      Standards:               null,
                                      APDSReference:           null
                                  )
                              ],
                              [
                                  new DisplayText(Languages.de, "Hallo Welt!"),
                                  new DisplayText(Languages.en, "Hello world!")
                              ],
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
                              [ Facilities.CAFE ],
                              new Hours(
                                  [
                                      new OCPI.RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-09-21T00:00:00Z"),
                                          DateTime.Parse("2020-09-22T00:00:00Z")
                                      )
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-12-24T00:00:00Z"),
                                          DateTime.Parse("2020-12-26T00:00:00Z")
                                      )
                                  ]
                              ),
                              false,
                              [
                                  new Image(
                                      URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                      ImageFileType.jpeg,
                                      ImageCategory.LOCATION,
                                      200,
                                      400,
                                      URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                  )
                              ],
                              new(
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
                              Created: DateTime.Parse("2020-09-21T00:00:00Z")
                          ),
                          SkipNotifications: true
                      );

                #endregion


                var response = await graphDefinedCPO.GetLocation(Location_Id.Parse("LOC0001"));

                // GET /2.2.1/cpo/locations HTTP/1.1
                // Date:                          Tue, 18 Apr 2023 03:41:25 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
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

                    ClassicAssert.IsTrue   (response.Data.EVSEs.First().Connectors.First().TariffIds.Contains(Tariff_Id.Parse("DE*GEF*T0001")));

                    ClassicAssert.AreEqual (1,  response.Data.Count());

                }

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region EMSP_GetLocation_Test2()

        /// <summary>
        /// EMSP GetLocation Test 2.
        /// </summary>
        [Test]
        public async Task EMSP_GetLocation_Test2()
        {

            var graphDefinedCPO = emsp2CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                #region Define Location1

                await cpoCommonAPI.AddLocation(
                          new Location(
                              CountryCode.Parse("DE"),
                              Party_Id.Parse("GEF"),
                              Location_Id.Parse("LOC0001"),
                              true,
                              "Biberweg 18",
                              "Jena",
                              Country.Germany,
                              GeoCoordinate.Parse(10, 20),
                              "Europe/Berlin",
                              null,
                              "Location 0001",
                              "07749",
                              "Thüringen",
                              [
                                  new AdditionalGeoLocation(
                                      Latitude.Parse(11),
                                      Longitude.Parse(22),
                                      Name: DisplayText.Create(Languages.de, "Postkasten")
                                  )
                              ],
                              ParkingType.PARKING_LOT,
                              [
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
                                              Watt.  ParseW(12),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0001"),
                                                  Tariff_Id.Parse("DE*GEF*T0002")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-21"),
                                              DateTime.Parse("2020-09-21")
                                          ),
                                          new Connector(
                                              Connector_Id.Parse("2"),
                                              ConnectorType.IEC_62196_T2_COMBO,
                                              ConnectorFormats.CABLE,
                                              PowerTypes.AC_3_PHASE,
                                              Volt.  ParseV(400),
                                              Ampere.ParseA(20),
                                              Watt.  ParseW(8),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0003"),
                                                  Tariff_Id.Parse("DE*GEF*T0004")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_20_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-22"),
                                              DateTime.Parse("2020-09-22")
                                          )
                                      ],
                                      [ Parking_Id.Parse("1") ],
                                      [ VehicleType.PERSONAL_VEHICLE ],

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
                                      new EnergyMeter<EVSE>(
                                          EnergyMeter_Id.Parse("Meter0815"),
                                          "EnergyMeter Model #1",
                                          null,
                                          "hw. v1.80",
                                          "fw. v1.20",
                                          "Energy Metering Services",
                                          null,
                                          null,
                                          null
                                      ),

                                      "1. Stock",
                                      GeoCoordinate.Parse(10.1, 20.2),
                                      "Ladestation #1",
                                      [
                                          DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                          DisplayText.Create(Languages.en, "Ken sent me!")
                                      ],
                                      [
                                          new Image(
                                              URL.Parse("http://example.com/pinguine.jpg"),
                                              ImageFileType.jpeg,
                                              ImageCategory.OPERATOR,
                                              100,
                                              150,
                                              URL.Parse("http://example.com/kleine_pinguine.jpg")
                                          )
                                      ],
                                      [ EMSP_Id.Parse("DE*GDF") ],
                                      DateTime.Parse("2020-09-22"),
                                      DateTime.Parse("2020-09-22")
                                  )
                              ],
                              [
                                  new Parking(
                                      Id:                      Parking_Id.Parse("1"),
                                      VehicleTypes:            [ VehicleType.PERSONAL_VEHICLE ],
                                      RestrictedToType:        false,
                                      ReservationRequired:     false,
                                      MaxVehicleWeight:        null,
                                      MaxVehicleHeight:        null,
                                      MaxVehicleLength:        null,
                                      MaxVehicleWidth:         null,
                                      ParkingSpaceLength:      null,
                                      ParkingSpaceWidth:       null,
                                      DangerousGoodsAllowed:   null,
                                      EVSEPosition:            null,
                                      Direction:               null,
                                      DriveThrough:            null,
                                      ParkingRestrictions:     null,
                                      TimeLimit:               null,
                                      Roofed:                  null,
                                      Images:                  null,
                                      Lighting:                null,
                                      RefrigerationOutlet:     null,
                                      Standards:               null,
                                      APDSReference:           null
                                  )
                              ],
                              [
                                  new DisplayText(Languages.de, "Hallo Welt!"),
                                  new DisplayText(Languages.en, "Hello world!")
                              ],
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
                              [ Facilities.CAFE ],
                              new Hours(
                                  [
                                      new OCPI.RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-09-21T00:00:00Z"),
                                          DateTime.Parse("2020-09-22T00:00:00Z")
                                      )
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-12-24T00:00:00Z"),
                                          DateTime.Parse("2020-12-26T00:00:00Z")
                                      )
                                  ]
                              ),
                              false,
                              [
                                  new Image(
                                      URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                      ImageFileType.jpeg,
                                      ImageCategory.LOCATION,
                                      200,
                                      400,
                                      URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                  )
                              ],
                              new(
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
                              Created: DateTime.Parse("2020-09-21T00:00:00Z")
                          ),
                          SkipNotifications: true
                      );

                #endregion


                var response = await graphDefinedCPO.GetLocation(Location_Id.Parse("LOC0001"));

                // GET /2.2.1/cpo/locations HTTP/1.1
                // Date:                          Tue, 18 Apr 2023 03:41:25 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
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

                    ClassicAssert.IsTrue   (response.Data.EVSEs.First().Connectors.First().TariffIds.Contains(Tariff_Id.Parse("DE*GEF*T0001")));

                    ClassicAssert.AreEqual (1,  response.Data.Count());

                }

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion


        #region EMSP_GetEVSE_Test1()

        /// <summary>
        /// EMSP GetEVSE Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetEVSE_Test1()
        {

            var graphDefinedCPO = emsp1CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                #region Define Location1

                await cpoCommonAPI.AddLocation(
                          new Location(
                              CountryCode.Parse("DE"),
                              Party_Id.Parse("GEF"),
                              Location_Id.Parse("LOC0001"),
                              true,
                              "Biberweg 18",
                              "Jena",
                              Country.Germany,
                              GeoCoordinate.Parse(10, 20),
                              "Europe/Berlin",
                              null,
                              "Location 0001",
                              "07749",
                              "Thüringen",
                              [
                                  new AdditionalGeoLocation(
                                      Latitude.Parse(11),
                                      Longitude.Parse(22),
                                      Name: DisplayText.Create(Languages.de, "Postkasten")
                                  )
                              ],
                              ParkingType.PARKING_LOT,
                              [
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
                                              Watt.  ParseW(12),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0001"),
                                                  Tariff_Id.Parse("DE*GEF*T0002")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-21"),
                                              DateTime.Parse("2020-09-21")
                                          ),
                                          new Connector(
                                              Connector_Id.Parse("2"),
                                              ConnectorType.IEC_62196_T2_COMBO,
                                              ConnectorFormats.CABLE,
                                              PowerTypes.AC_3_PHASE,
                                              Volt.  ParseV(400),
                                              Ampere.ParseA(20),
                                              Watt.  ParseW(8),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0003"),
                                                  Tariff_Id.Parse("DE*GEF*T0004")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_20_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-22"),
                                              DateTime.Parse("2020-09-22")
                                          )
                                      ],
                                      [ Parking_Id.Parse("1") ],
                                      [ VehicleType.PERSONAL_VEHICLE ],

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
                                      new EnergyMeter<EVSE>(
                                          EnergyMeter_Id.Parse("Meter0815"),
                                          "EnergyMeter Model #1",
                                          null,
                                          "hw. v1.80",
                                          "fw. v1.20",
                                          "Energy Metering Services",
                                          null,
                                          null,
                                          null
                                      ),

                                      "1. Stock",
                                      GeoCoordinate.Parse(10.1, 20.2),
                                      "Ladestation #1",
                                      [
                                          DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                          DisplayText.Create(Languages.en, "Ken sent me!")
                                      ],
                                      [
                                          new Image(
                                              URL.Parse("http://example.com/pinguine.jpg"),
                                              ImageFileType.jpeg,
                                              ImageCategory.OPERATOR,
                                              100,
                                              150,
                                              URL.Parse("http://example.com/kleine_pinguine.jpg")
                                          )
                                      ],
                                      [ EMSP_Id.Parse("DE*GDF") ],
                                      DateTime.Parse("2020-09-22"),
                                      DateTime.Parse("2020-09-22")
                                  )
                              ],
                              [
                                  new Parking(
                                      Id:                      Parking_Id.Parse("1"),
                                      VehicleTypes:            [ VehicleType.PERSONAL_VEHICLE ],
                                      RestrictedToType:        false,
                                      ReservationRequired:     false,
                                      MaxVehicleWeight:        null,
                                      MaxVehicleHeight:        null,
                                      MaxVehicleLength:        null,
                                      MaxVehicleWidth:         null,
                                      ParkingSpaceLength:      null,
                                      ParkingSpaceWidth:       null,
                                      DangerousGoodsAllowed:   null,
                                      EVSEPosition:            null,
                                      Direction:               null,
                                      DriveThrough:            null,
                                      ParkingRestrictions:     null,
                                      TimeLimit:               null,
                                      Roofed:                  null,
                                      Images:                  null,
                                      Lighting:                null,
                                      RefrigerationOutlet:     null,
                                      Standards:               null,
                                      APDSReference:           null
                                  )
                              ],
                              [
                                  new DisplayText(Languages.de, "Hallo Welt!"),
                                  new DisplayText(Languages.en, "Hello world!")
                              ],
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
                              [ Facilities.CAFE ],
                              new Hours(
                                  [
                                      new OCPI.RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-09-21T00:00:00Z"),
                                          DateTime.Parse("2020-09-22T00:00:00Z")
                                      )
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-12-24T00:00:00Z"),
                                          DateTime.Parse("2020-12-26T00:00:00Z")
                                      )
                                  ]
                              ),
                              false,
                              [
                                  new Image(
                                      URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                      ImageFileType.jpeg,
                                      ImageCategory.LOCATION,
                                      200,
                                      400,
                                      URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                  )
                              ],
                              new(
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
                              Created: DateTime.Parse("2020-09-21T00:00:00Z")
                          ),
                          SkipNotifications: true
                      );

                #endregion


                var response = await graphDefinedCPO.GetEVSE(Location_Id.Parse("LOC0001"),
                                                             EVSE_UId.   Parse("DE*GEF*E*LOC0001*1"));

                // GET /ocpi/v2.1/v2.1.1/cpo/locations/LOC0001 HTTP/1.1
                // Date:                           Wed, 26 Apr 2023 05:07:21 GMT
                // Accept:                         application/json; charset=utf-8;q=1
                // Host:                           localhost:3301
                // Authorization:                  Token cso-2-emp1:token
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

                    ClassicAssert.IsTrue   (response.Data.Connectors.First().TariffIds.Contains(Tariff_Id.Parse("DE*GEF*T0001")));

                    ClassicAssert.AreEqual (2,  response.Data.Count());  // 2 Connectors!

                }

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region EMSP_GetEVSE_Test2()

        /// <summary>
        /// EMSP GetEVSE Test 2.
        /// </summary>
        [Test]
        public async Task EMSP_GetEVSE_Test2()
        {

            var graphDefinedCPO = emsp2CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                #region Define Location1

                await cpoCommonAPI.AddLocation(
                          new Location(
                              CountryCode.Parse("DE"),
                              Party_Id.Parse("GEF"),
                              Location_Id.Parse("LOC0001"),
                              true,
                              "Biberweg 18",
                              "Jena",
                              Country.Germany,
                              GeoCoordinate.Parse(10, 20),
                              "Europe/Berlin",
                              null,
                              "Location 0001",
                              "07749",
                              "Thüringen",
                              [
                                  new AdditionalGeoLocation(
                                      Latitude.Parse(11),
                                      Longitude.Parse(22),
                                      Name: DisplayText.Create(Languages.de, "Postkasten")
                                  )
                              ],
                              ParkingType.PARKING_LOT,
                              [
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
                                              Watt.  ParseW(12),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0001"),
                                                  Tariff_Id.Parse("DE*GEF*T0002")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-21"),
                                              DateTime.Parse("2020-09-21")
                                          ),
                                          new Connector(
                                              Connector_Id.Parse("2"),
                                              ConnectorType.IEC_62196_T2_COMBO,
                                              ConnectorFormats.CABLE,
                                              PowerTypes.AC_3_PHASE,
                                              Volt.  ParseV(400),
                                              Ampere.ParseA(20),
                                              Watt.  ParseW(8),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0003"),
                                                  Tariff_Id.Parse("DE*GEF*T0004")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_20_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-22"),
                                              DateTime.Parse("2020-09-22")
                                          )
                                      ],
                                      [ Parking_Id.Parse("1") ],
                                      [ VehicleType.PERSONAL_VEHICLE ],

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
                                      new EnergyMeter<EVSE>(
                                          EnergyMeter_Id.Parse("Meter0815"),
                                          "EnergyMeter Model #1",
                                          null,
                                          "hw. v1.80",
                                          "fw. v1.20",
                                          "Energy Metering Services",
                                          null,
                                          null,
                                          null
                                      ),

                                      "1. Stock",
                                      GeoCoordinate.Parse(10.1, 20.2),
                                      "Ladestation #1",
                                      [
                                          DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                          DisplayText.Create(Languages.en, "Ken sent me!")
                                      ],
                                      [
                                          new Image(
                                              URL.Parse("http://example.com/pinguine.jpg"),
                                              ImageFileType.jpeg,
                                              ImageCategory.OPERATOR,
                                              100,
                                              150,
                                              URL.Parse("http://example.com/kleine_pinguine.jpg")
                                          )
                                      ],
                                      [ EMSP_Id.Parse("DE*GDF") ],
                                      DateTime.Parse("2020-09-22"),
                                      DateTime.Parse("2020-09-22")
                                  )
                              ],
                              [
                                  new Parking(
                                      Id:                      Parking_Id.Parse("1"),
                                      VehicleTypes:            [ VehicleType.PERSONAL_VEHICLE ],
                                      RestrictedToType:        false,
                                      ReservationRequired:     false,
                                      MaxVehicleWeight:        null,
                                      MaxVehicleHeight:        null,
                                      MaxVehicleLength:        null,
                                      MaxVehicleWidth:         null,
                                      ParkingSpaceLength:      null,
                                      ParkingSpaceWidth:       null,
                                      DangerousGoodsAllowed:   null,
                                      EVSEPosition:            null,
                                      Direction:               null,
                                      DriveThrough:            null,
                                      ParkingRestrictions:     null,
                                      TimeLimit:               null,
                                      Roofed:                  null,
                                      Images:                  null,
                                      Lighting:                null,
                                      RefrigerationOutlet:     null,
                                      Standards:               null,
                                      APDSReference:           null
                                  )
                              ],
                              [
                                  new DisplayText(Languages.de, "Hallo Welt!"),
                                  new DisplayText(Languages.en, "Hello world!")
                              ],
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
                              [ Facilities.CAFE ],
                              new Hours(
                                  [
                                      new OCPI.RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-09-21T00:00:00Z"),
                                          DateTime.Parse("2020-09-22T00:00:00Z")
                                      )
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-12-24T00:00:00Z"),
                                          DateTime.Parse("2020-12-26T00:00:00Z")
                                      )
                                  ]
                              ),
                              false,
                              [
                                  new Image(
                                      URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                      ImageFileType.jpeg,
                                      ImageCategory.LOCATION,
                                      200,
                                      400,
                                      URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                  )
                              ],
                              new(
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
                              Created: DateTime.Parse("2020-09-21T00:00:00Z")
                          ),
                          SkipNotifications: true
                      );

                #endregion


                var response = await graphDefinedCPO.GetEVSE(Location_Id.Parse("LOC0001"),
                                                             EVSE_UId.   Parse("DE*GEF*E*LOC0001*1"));

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

                    ClassicAssert.IsTrue   (response.Data.Connectors.First().TariffIds.Contains(Tariff_Id.Parse("DE*GEF*T0001")));

                    ClassicAssert.AreEqual (2,  response.Data.Count());  // 2 Connectors!

                }

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion


        #region EMSP_GetConnector_Test1()

        /// <summary>
        /// EMSP GetConnector Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetConnector_Test1()
        {

            var graphDefinedCPO = emsp1CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                #region Define Location1

                await cpoCommonAPI.AddLocation(
                          new Location(
                              CountryCode.Parse("DE"),
                              Party_Id.Parse("GEF"),
                              Location_Id.Parse("LOC0001"),
                              true,
                              "Biberweg 18",
                              "Jena",
                              Country.Germany,
                              GeoCoordinate.Parse(10, 20),
                              "Europe/Berlin",
                              null,
                              "Location 0001",
                              "07749",
                              "Thüringen",
                              [
                                  new AdditionalGeoLocation(
                                      Latitude.Parse(11),
                                      Longitude.Parse(22),
                                      Name: DisplayText.Create(Languages.de, "Postkasten")
                                  )
                              ],
                              ParkingType.PARKING_LOT,
                              [
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
                                              Watt.  ParseW(12),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0001"),
                                                  Tariff_Id.Parse("DE*GEF*T0002")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-21"),
                                              DateTime.Parse("2020-09-21")
                                          ),
                                          new Connector(
                                              Connector_Id.Parse("2"),
                                              ConnectorType.IEC_62196_T2_COMBO,
                                              ConnectorFormats.CABLE,
                                              PowerTypes.AC_3_PHASE,
                                              Volt.  ParseV(400),
                                              Ampere.ParseA(20),
                                              Watt.  ParseW(8),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0003"),
                                                  Tariff_Id.Parse("DE*GEF*T0004")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_20_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-22"),
                                              DateTime.Parse("2020-09-22")
                                          )
                                      ],
                                      [ Parking_Id.Parse("1") ],
                                      [ VehicleType.PERSONAL_VEHICLE ],

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
                                      new EnergyMeter<EVSE>(
                                          EnergyMeter_Id.Parse("Meter0815"),
                                          "EnergyMeter Model #1",
                                          null,
                                          "hw. v1.80",
                                          "fw. v1.20",
                                          "Energy Metering Services",
                                          null,
                                          null,
                                          null
                                      ),

                                      "1. Stock",
                                      GeoCoordinate.Parse(10.1, 20.2),
                                      "Ladestation #1",
                                      [
                                          DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                          DisplayText.Create(Languages.en, "Ken sent me!")
                                      ],
                                      [
                                          new Image(
                                              URL.Parse("http://example.com/pinguine.jpg"),
                                              ImageFileType.jpeg,
                                              ImageCategory.OPERATOR,
                                              100,
                                              150,
                                              URL.Parse("http://example.com/kleine_pinguine.jpg")
                                          )
                                      ],
                                      [ EMSP_Id.Parse("DE*GDF") ],
                                      DateTime.Parse("2020-09-22"),
                                      DateTime.Parse("2020-09-22")
                                  )
                              ],
                              [
                                  new Parking(
                                      Id:                      Parking_Id.Parse("1"),
                                      VehicleTypes:            [ VehicleType.PERSONAL_VEHICLE ],
                                      RestrictedToType:        false,
                                      ReservationRequired:     false,
                                      MaxVehicleWeight:        null,
                                      MaxVehicleHeight:        null,
                                      MaxVehicleLength:        null,
                                      MaxVehicleWidth:         null,
                                      ParkingSpaceLength:      null,
                                      ParkingSpaceWidth:       null,
                                      DangerousGoodsAllowed:   null,
                                      EVSEPosition:            null,
                                      Direction:               null,
                                      DriveThrough:            null,
                                      ParkingRestrictions:     null,
                                      TimeLimit:               null,
                                      Roofed:                  null,
                                      Images:                  null,
                                      Lighting:                null,
                                      RefrigerationOutlet:     null,
                                      Standards:               null,
                                      APDSReference:           null
                                  )
                              ],
                              [
                                  new DisplayText(Languages.de, "Hallo Welt!"),
                                  new DisplayText(Languages.en, "Hello world!")
                              ],
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
                              [ Facilities.CAFE ],
                              new Hours(
                                  [
                                      new OCPI.RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-09-21T00:00:00Z"),
                                          DateTime.Parse("2020-09-22T00:00:00Z")
                                      )
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-12-24T00:00:00Z"),
                                          DateTime.Parse("2020-12-26T00:00:00Z")
                                      )
                                  ]
                              ),
                              false,
                              [
                                  new Image(
                                      URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                      ImageFileType.jpeg,
                                      ImageCategory.LOCATION,
                                      200,
                                      400,
                                      URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                  )
                              ],
                              new(
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
                              Created: DateTime.Parse("2020-09-21T00:00:00Z")
                          ),
                          SkipNotifications: true
                      );

                #endregion


                var response = await graphDefinedCPO.GetConnector(Location_Id. Parse("LOC0001"),
                                                                  EVSE_UId.    Parse("DE*GEF*E*LOC0001*1"),
                                                                  Connector_Id.Parse("1"));

                // GET /ocpi/v2.1/v2.1.1/cpo/locations/LOC0001 HTTP/1.1
                // Date:                           Wed, 26 Apr 2023 05:07:21 GMT
                // Accept:                         application/json; charset=utf-8;q=1
                // Host:                           localhost:3301
                // Authorization:                  Token cso-2-emp1:token
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

                    ClassicAssert.IsTrue(response.Data.TariffIds.Contains(Tariff_Id.Parse("DE*GEF*T0001")));

                }

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region EMSP_GetConnector_Test2()

        /// <summary>
        /// EMSP GetConnector Test 2.
        /// </summary>
        [Test]
        public async Task EMSP_GetConnector_Test2()
        {

            var graphDefinedCPO = emsp2CommonAPI?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            ClassicAssert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null &&
                cpoCommonAPI    is not null)
            {

                #region Define Location1

                await cpoCommonAPI.AddLocation(
                          new Location(
                              CountryCode.Parse("DE"),
                              Party_Id.Parse("GEF"),
                              Location_Id.Parse("LOC0001"),
                              true,
                              "Biberweg 18",
                              "Jena",
                              Country.Germany,
                              GeoCoordinate.Parse(10, 20),
                              "Europe/Berlin",
                              null,
                              "Location 0001",
                              "07749",
                              "Thüringen",
                              [
                                  new AdditionalGeoLocation(
                                      Latitude.Parse(11),
                                      Longitude.Parse(22),
                                      Name: DisplayText.Create(Languages.de, "Postkasten")
                                  )
                              ],
                              ParkingType.PARKING_LOT,
                              [
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
                                              Watt.  ParseW(12),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0001"),
                                                  Tariff_Id.Parse("DE*GEF*T0002")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-21"),
                                              DateTime.Parse("2020-09-21")
                                          ),
                                          new Connector(
                                              Connector_Id.Parse("2"),
                                              ConnectorType.IEC_62196_T2_COMBO,
                                              ConnectorFormats.CABLE,
                                              PowerTypes.AC_3_PHASE,
                                              Volt.  ParseV(400),
                                              Ampere.ParseA(20),
                                              Watt.  ParseW(8),
                                              [
                                                  Tariff_Id.Parse("DE*GEF*T0003"),
                                                  Tariff_Id.Parse("DE*GEF*T0004")
                                              ],
                                              URL.Parse("https://open.charging.cloud/terms"),
                                              [ ConnectorCapability.ISO_15118_20_PLUG_AND_CHARGE ],
                                              DateTime.Parse("2020-09-22"),
                                              DateTime.Parse("2020-09-22")
                                          )
                                      ],
                                      [ Parking_Id.Parse("1") ],
                                      [ VehicleType.PERSONAL_VEHICLE ],

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
                                      new EnergyMeter<EVSE>(
                                          EnergyMeter_Id.Parse("Meter0815"),
                                          "EnergyMeter Model #1",
                                          null,
                                          "hw. v1.80",
                                          "fw. v1.20",
                                          "Energy Metering Services",
                                          null,
                                          null,
                                          null
                                      ),

                                      "1. Stock",
                                      GeoCoordinate.Parse(10.1, 20.2),
                                      "Ladestation #1",
                                      [
                                          DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                          DisplayText.Create(Languages.en, "Ken sent me!")
                                      ],
                                      [
                                          new Image(
                                              URL.Parse("http://example.com/pinguine.jpg"),
                                              ImageFileType.jpeg,
                                              ImageCategory.OPERATOR,
                                              100,
                                              150,
                                              URL.Parse("http://example.com/kleine_pinguine.jpg")
                                          )
                                      ],
                                      [ EMSP_Id.Parse("DE*GDF") ],
                                      DateTime.Parse("2020-09-22"),
                                      DateTime.Parse("2020-09-22")
                                  )
                              ],
                              [
                                  new Parking(
                                      Id:                      Parking_Id.Parse("1"),
                                      VehicleTypes:            [ VehicleType.PERSONAL_VEHICLE ],
                                      RestrictedToType:        false,
                                      ReservationRequired:     false,
                                      MaxVehicleWeight:        null,
                                      MaxVehicleHeight:        null,
                                      MaxVehicleLength:        null,
                                      MaxVehicleWidth:         null,
                                      ParkingSpaceLength:      null,
                                      ParkingSpaceWidth:       null,
                                      DangerousGoodsAllowed:   null,
                                      EVSEPosition:            null,
                                      Direction:               null,
                                      DriveThrough:            null,
                                      ParkingRestrictions:     null,
                                      TimeLimit:               null,
                                      Roofed:                  null,
                                      Images:                  null,
                                      Lighting:                null,
                                      RefrigerationOutlet:     null,
                                      Standards:               null,
                                      APDSReference:           null
                                  )
                              ],
                              [
                                  new DisplayText(Languages.de, "Hallo Welt!"),
                                  new DisplayText(Languages.en, "Hello world!")
                              ],
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
                              [ Facilities.CAFE ],
                              new Hours(
                                  [
                                      new OCPI.RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                      new OCPI.RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-09-21T00:00:00Z"),
                                          DateTime.Parse("2020-09-22T00:00:00Z")
                                      )
                                  ],
                                  [
                                      new OCPI.ExceptionalPeriod(
                                          DateTime.Parse("2020-12-24T00:00:00Z"),
                                          DateTime.Parse("2020-12-26T00:00:00Z")
                                      )
                                  ]
                              ),
                              false,
                              [
                                  new Image(
                                      URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                      ImageFileType.jpeg,
                                      ImageCategory.LOCATION,
                                      200,
                                      400,
                                      URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                  )
                              ],
                              new(
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
                              Created: DateTime.Parse("2020-09-21T00:00:00Z")
                          ),
                          SkipNotifications: true
                      );

                #endregion


                var response = await graphDefinedCPO.GetConnector(Location_Id. Parse("LOC0001"),
                                                                  EVSE_UId.    Parse("DE*GEF*E*LOC0001*1"),
                                                                  Connector_Id.Parse("1"));

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

                    ClassicAssert.IsTrue(response.Data.TariffIds.Contains(Tariff_Id.Parse("DE*GEF*T0001")));

                }

                //ClassicAssert.IsNotNull(response.Request);

            }

        }

        #endregion


    }

}
