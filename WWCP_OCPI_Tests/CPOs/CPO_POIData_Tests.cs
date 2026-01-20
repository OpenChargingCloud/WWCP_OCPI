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
    public class CPO_POIData_Tests()

        : A_2CPOs2EMSPs_TestDefaults()

    {

        #region Location1_v2_1_1

        private static OCPIv2_1_1.Location Location1_v2_1_1

            => new (
                   CountryCode.Parse("DE"),
                   Party_Id.   Parse("GEF"),
                   Location_Id.Parse("LOC0001"),
                   OCPIv2_1_1.LocationType.PARKING_LOT,
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
                   [
                       new OCPIv2_1_1.EVSE(
                           EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                           OCPIv2_1_1.StatusType.AVAILABLE,
                           [
                               new OCPIv2_1_1.Connector(
                                   Connector_Id.Parse("1"),
                                   OCPIv2_1_1.ConnectorType.IEC_62196_T2,
                                   OCPIv2_1_1.ConnectorFormats.SOCKET,
                                   OCPIv2_1_1.PowerTypes.AC_3_PHASE,
                                   Volt.  ParseV(400),
                                   Ampere.ParseA(30),
                                   Tariff_Id.Parse("DE*GEF*T0001"),
                                   URL.Parse("https://open.charging.cloud/terms"),
                                   DateTime.Parse("2020-09-21")
                               ),
                               new OCPIv2_1_1.Connector(
                                   Connector_Id.Parse("2"),
                                   OCPIv2_1_1.ConnectorType.IEC_62196_T2_COMBO,
                                   OCPIv2_1_1.ConnectorFormats.CABLE,
                                   OCPIv2_1_1.PowerTypes.AC_3_PHASE,
                                   Volt.  ParseV(400),
                                   Ampere.ParseA(20),
                                   Tariff_Id.Parse("DE*GEF*T0003"),
                                   URL.Parse("https://open.charging.cloud/terms"),
                                   DateTime.Parse("2020-09-22")
                               )
                           ],
                           EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                           [
                               new OCPIv2_1_1.StatusSchedule(
                                   OCPIv2_1_1.StatusType.INOPERATIVE,
                                   DateTime.Parse("2020-09-23"),
                                   DateTime.Parse("2020-09-24")
                               ),
                               new OCPIv2_1_1.StatusSchedule(
                                   OCPIv2_1_1.StatusType.OUTOFORDER,
                                   DateTime.Parse("2020-12-30"),
                                   DateTime.Parse("2020-12-31")
                               )
                           ],
                           [
                               OCPIv2_1_1.Capability.RFID_READER,
                               OCPIv2_1_1.Capability.RESERVABLE
                           ],

                           // OCPI Computer Science Extensions
                           new EnergyMeter<OCPIv2_1_1.EVSE>(
                               EnergyMeter_Id.Parse("Meter0815"),
                               "EnergyMeter Model #1",
                               null,
                               "hw. v1.80",
                               "fw. v1.20",
                               "Energy Metering Services",
                               null,
                               null,
                               null,
                               [
                                   new TransparencySoftwareStatus(
                                       new TransparencySoftware(
                                           "Chargy Transparency Software Desktop Application",
                                           "v1.00",
                                           SoftwareLicense.AGPL3,
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
                                           SoftwareLicense.AGPL3,
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
                               ]
                           ),

                           "1. Stock",
                           GeoCoordinate.Parse(10.1, 20.2),
                           "Ladestation #1",
                           [
                               DisplayText.Create(Languages.de, "Bitte klingeln!"),
                               DisplayText.Create(Languages.en, "Ken sent me!")
                           ],
                           [
                               OCPIv2_1_1.ParkingRestriction.EV_ONLY,
                               OCPIv2_1_1.ParkingRestriction.PLUGGED
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
                           null,
                           null,
                           DateTime.Parse("2020-09-22")
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
                   [
                       OCPIv2_1_1.Facility.CAFE
                   ],
                   "Europe/Berlin",
                   new Hours(
                       [
                           new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                           new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                           new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                           new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                           new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                       ],
                       [
                           new ExceptionalPeriod(
                               DateTime.Parse("2020-09-21T00:00:00Z"),
                               DateTime.Parse("2020-09-22T00:00:00Z")
                           )
                       ],
                       [
                           new ExceptionalPeriod(
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
                   new OCPIv2_1_1.EnergyMix(
                       true,
                       [
                           new OCPIv2_1_1.EnergySource(
                               OCPIv2_1_1.EnergySourceCategory.SOLAR,
                               80
                           ),
                           new OCPIv2_1_1.EnergySource(
                               OCPIv2_1_1.EnergySourceCategory.WIND,
                               20
                           )
                       ],
                       [
                           new OCPIv2_1_1.EnvironmentalImpact(
                               OCPIv2_1_1.EnvironmentalImpactCategory.CARBON_DIOXIDE,
                               0.1
                           )
                       ],
                       "Stadtwerke Jena-Ost",
                       "New Green Deal"
                   ),

                   LastUpdated: DateTime.Parse("2020-09-21T00:00:00Z").ToUniversalTime()

               );

        #endregion

        #region Location1_v2_2_1

        private static OCPIv2_2_1.Location Location1_v2_2_1

            => new (

                   CountryCode.Parse("DE"),
                   Party_Id.   Parse("GEF"),
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
                   OCPIv2_2_1.ParkingType.PARKING_LOT,

                   [
                       new OCPIv2_2_1.EVSE(
                           EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                           OCPIv2_2_1.StatusType.AVAILABLE,
                           [
                               new OCPIv2_2_1.Connector(
                                   Connector_Id.Parse("1"),
                                   OCPIv2_2_1.ConnectorType.IEC_62196_T2,
                                   OCPIv2_2_1.ConnectorFormats.SOCKET,
                                   OCPIv2_2_1.PowerTypes.AC_3_PHASE,
                                   Volt.  ParseV(400),
                                   Ampere.ParseA(30),
                                   Watt.  ParseW(12),
                                   [
                                       Tariff_Id.Parse("DE*GEF*T0001"),
                                       Tariff_Id.Parse("DE*GEF*T0002")
                                   ],
                                   URL.Parse("https://open.charging.cloud/terms"),
                                   DateTime.Parse("2020-09-21")
                               ),
                               new OCPIv2_2_1.Connector(
                                   Connector_Id.Parse("2"),
                                   OCPIv2_2_1.ConnectorType.IEC_62196_T2_COMBO,
                                   OCPIv2_2_1.ConnectorFormats.CABLE,
                                   OCPIv2_2_1.PowerTypes.AC_3_PHASE,
                                   Volt.  ParseV(400),
                                   Ampere.ParseA(20),
                                   Watt.  ParseW(8),
                                   [
                                       Tariff_Id.Parse("DE*GEF*T0003"),
                                       Tariff_Id.Parse("DE*GEF*T0004")
                                   ],
                                   URL.Parse("https://open.charging.cloud/terms"),
                                   DateTime.Parse("2020-09-22")
                               )
                           ],
                           EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                           [
                               new OCPIv2_2_1.StatusSchedule(
                                   OCPIv2_2_1.StatusType.INOPERATIVE,
                                   DateTime.Parse("2020-09-23"),
                                   DateTime.Parse("2020-09-24")
                               ),
                               new OCPIv2_2_1.StatusSchedule(
                                   OCPIv2_2_1.StatusType.OUTOFORDER,
                                   DateTime.Parse("2020-12-30"),
                                   DateTime.Parse("2020-12-31")
                               )
                           ],
                           [
                               OCPIv2_2_1.Capability.RFID_READER,
                               OCPIv2_2_1.Capability.RESERVABLE
                           ],

                           // OCPI Computer Science Extensions
                           new EnergyMeter<OCPIv2_2_1.EVSE>(
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
                               OCPIv2_2_1.ParkingRestriction.EV_ONLY,
                               OCPIv2_2_1.ParkingRestriction.PLUGGED
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
                           LastUpdated: DateTime.Parse("2020-09-22")
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
                   [
                       OCPIv2_2_1.Facility.CAFE
                   ],
                   new Hours(
                       [
                           new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                           new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                           new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                           new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                           new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                       ],
                       [
                           new ExceptionalPeriod(
                               DateTime.Parse("2020-09-21T00:00:00Z"),
                               DateTime.Parse("2020-09-22T00:00:00Z")
                           )
                       ],
                       [
                           new ExceptionalPeriod(
                               DateTime.Parse("2020-12-24T00:00:00Z"),
                               DateTime.Parse("2020-12-26T00:00:00Z")
                           )
                       ]
                   ),
                   ChargingWhenClosed: false,
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
                           new OCPIv2_2_1.EnergySource(
                               OCPIv2_2_1.EnergySourceCategory.SOLAR,
                               80
                           ),
                           new OCPIv2_2_1.EnergySource(
                               OCPIv2_2_1.EnergySourceCategory.WIND,
                               20
                           )
                       ],
                       [
                           new OCPIv2_2_1.EnvironmentalImpact(
                               OCPIv2_2_1.EnvironmentalImpactCategory.CARBON_DIOXIDE,
                               0.1
                           )
                       ],
                       "Stadtwerke Jena-Ost",
                       "New Green Deal"
                   ),

                   Created: DateTime.Parse("2020-09-21T00:00:00Z")

               );

        #endregion

        #region Location1_v2_3_0

        private static OCPIv2_3_0.Location Location1_v2_3_0

            => new (

                   CountryCode.Parse("DE"),
                   Party_Id.   Parse("GEF"),
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
                   OCPIv2_3_0.ParkingType.PARKING_LOT,
                   [
                       new OCPIv2_3_0.EVSE(
                           EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                           OCPIv2_3_0.StatusType.AVAILABLE,
                           [
                               new OCPIv2_3_0.Connector(
                                   Connector_Id.Parse("1"),
                                   OCPIv2_3_0.ConnectorType.IEC_62196_T2,
                                   OCPIv2_3_0.ConnectorFormats.SOCKET,
                                   OCPIv2_3_0.PowerTypes.AC_3_PHASE,
                                   Volt.  ParseV(400),
                                   Ampere.ParseA(30),
                                   Watt.  ParseW(12),
                                   [
                                       Tariff_Id.Parse("DE*GEF*T0001"),
                                       Tariff_Id.Parse("DE*GEF*T0002")
                                   ],
                                   URL.Parse("https://open.charging.cloud/terms"),
                                   [ OCPIv2_3_0.ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                                   DateTime.Parse("2020-09-21"),
                                   DateTime.Parse("2020-09-21")
                               ),
                               new OCPIv2_3_0.Connector(
                                   Connector_Id.Parse("2"),
                                   OCPIv2_3_0.ConnectorType.IEC_62196_T2_COMBO,
                                   OCPIv2_3_0.ConnectorFormats.CABLE,
                                   OCPIv2_3_0.PowerTypes.AC_3_PHASE,
                                   Volt.  ParseV(400),
                                   Ampere.ParseA(20),
                                   Watt.  ParseW(8),
                                   [
                                       Tariff_Id.Parse("DE*GEF*T0003"),
                                       Tariff_Id.Parse("DE*GEF*T0004")
                                   ],
                                   URL.Parse("https://open.charging.cloud/terms"),
                                   [ OCPIv2_3_0.ConnectorCapability.ISO_15118_20_PLUG_AND_CHARGE ],
                                   DateTime.Parse("2020-09-22"),
                                   DateTime.Parse("2020-09-22")
                               )
                           ],


                           EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                           [
                               new OCPIv2_3_0.StatusSchedule(
                                   OCPIv2_3_0.StatusType.INOPERATIVE,
                                   DateTime.Parse("2020-09-23"),
                                   DateTime.Parse("2020-09-24")
                               ),
                               new OCPIv2_3_0.StatusSchedule(
                                   OCPIv2_3_0.StatusType.OUTOFORDER,
                                   DateTime.Parse("2020-12-30"),
                                   DateTime.Parse("2020-12-31")
                               )
                           ],
                           [
                               OCPIv2_3_0.Capability.RFID_READER,
                               OCPIv2_3_0.Capability.RESERVABLE
                           ],

                           // OCPI Computer Science Extensions
                           new EnergyMeter<OCPIv2_3_0.EVSE>(
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
                           [ OCPIv2_3_0.ParkingRestriction.CUSTOMERS ],
                           [
                               new OCPIv2_3_0.EVSEParking(
                                   Parking_Id.Parse("1"),
                                   OCPIv2_3_0.EVSEPosition.CENTER)
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
                           Created:     DateTime.Parse("2020-09-22"),
                           LastUpdated: DateTime.Parse("2020-09-22")
                       )
                   ],
                   [
                       new OCPIv2_3_0.Parking(
                           Id:                      Parking_Id.Parse("1"),
                           VehicleTypes:            [ OCPIv2_3_0.VehicleType.PERSONAL_VEHICLE ],
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

                   [ OCPIv2_3_0.Facility.CAFE ],
                   new Hours(
                       [
                           new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                           new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                           new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                           new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                           new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                       ],
                       [
                           new ExceptionalPeriod(
                               DateTime.Parse("2020-09-21T00:00:00Z"),
                               DateTime.Parse("2020-09-22T00:00:00Z")
                           )
                       ],
                       [
                           new ExceptionalPeriod(
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
                           new OCPIv2_3_0.EnergySource(
                               OCPIv2_3_0.EnergySourceCategory.SOLAR,
                               80
                           ),
                           new OCPIv2_3_0.EnergySource(
                               OCPIv2_3_0.EnergySourceCategory.WIND,
                               20
                           )
                       ],
                       [
                           new OCPIv2_3_0.EnvironmentalImpact(
                               OCPIv2_3_0.EnvironmentalImpactCategory.CARBON_DIOXIDE,
                               0.1
                           )
                       ],
                       "Stadtwerke Jena-Ost",
                       "New Green Deal"
                   ),

                   Created: DateTime.Parse("2020-09-21T00:00:00Z")

               );

        #endregion


        #region EVSE1_v2_1_1

        private static OCPIv2_1_1.EVSE EVSE1_v2_1_1

            => new (

                   EVSE_UId.Parse("DE*GEF*E*LOC0001*2"),
                   OCPIv2_1_1.StatusType.AVAILABLE,
                   [
                       new OCPIv2_1_1.Connector(
                           Connector_Id.Parse("1"),
                           OCPIv2_1_1.ConnectorType.CHADEMO,
                           OCPIv2_1_1.ConnectorFormats.SOCKET,
                           OCPIv2_1_1.PowerTypes.DC,
                           Volt.  ParseV(400),
                           Ampere.ParseA(30),
                           Tariff_Id.Parse("DE*GEF*T0003"),
                           URL.Parse("https://open.charging.cloud/terms"),
                           DateTime.Parse("2020-09-21")
                       ),
                       new OCPIv2_1_1.Connector(
                           Connector_Id.Parse("2"),
                           OCPIv2_1_1.ConnectorType.CHADEMO,
                           OCPIv2_1_1.ConnectorFormats.CABLE,
                           OCPIv2_1_1.PowerTypes.DC,
                           Volt.  ParseV(400),
                           Ampere.ParseA(20),
                           Tariff_Id.Parse("DE*GEF*T0004"),
                           URL.Parse("https://open.charging.cloud/terms"),
                           DateTime.Parse("2021-11-13")
                       )
                   ],
                   EVSE_Id.Parse("DE*GEF*E*LOC0001*2"),
                   [
                       new OCPIv2_1_1.StatusSchedule(
                           OCPIv2_1_1.StatusType.INOPERATIVE,
                           DateTime.Parse("2021-11-23"),
                           DateTime.Parse("2021-11-24")
                       ),
                       new OCPIv2_1_1.StatusSchedule(
                           OCPIv2_1_1.StatusType.OUTOFORDER,
                           DateTime.Parse("2021-10-30"),
                           DateTime.Parse("2021-10-31")
                       )
                   ],
                   [
                       OCPIv2_1_1.Capability.RFID_READER,
                       OCPIv2_1_1.Capability.RESERVABLE
                   ],

                   // OCPI Computer Science Extensions
                   new EnergyMeter<OCPIv2_1_1.EVSE>(
                       EnergyMeter_Id.Parse("Meter0815b"),
                       "EnergyMeter Model #1",
                       null,
                       "hw. v1.80",
                       "fw. v1.20",
                       "Energy Metering Services",
                       null,
                       null,
                       null,
                       [
                           new TransparencySoftwareStatus(
                               new TransparencySoftware(
                                   "Chargy Transparency Software Desktop Application",
                                   "v1.00",
                                   SoftwareLicense.AGPL3,
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
                                   SoftwareLicense.AGPL3,
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
                       ]
                   ),

                   "2. Stock",
                   GeoCoordinate.Parse(10.1, 20.2),
                   "Ladestation #2",
                   [
                       DisplayText.Create(Languages.de, "Bitte klingeln!"),
                       DisplayText.Create(Languages.en, "Dave sent me!")
                   ],
                   [
                       OCPIv2_1_1.ParkingRestriction.EV_ONLY,
                       OCPIv2_1_1.ParkingRestriction.PLUGGED
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

                   LastUpdated: DateTime.Parse("2020-09-22")

               );

        #endregion

        #region EVSE1_v2_2_1

        private static OCPIv2_2_1.EVSE EVSE1_v2_2_1

            => new (

                   EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                   OCPIv2_2_1.StatusType.AVAILABLE,
                   [
                       new OCPIv2_2_1.Connector(
                           Connector_Id.Parse("1"),
                           OCPIv2_2_1.ConnectorType.IEC_62196_T2,
                           OCPIv2_2_1.ConnectorFormats.SOCKET,
                           OCPIv2_2_1.PowerTypes.AC_3_PHASE,
                           Volt.  ParseV(400),
                           Ampere.ParseA(30),
                           Watt.  ParseW(12),
                           [
                               Tariff_Id.Parse("DE*GEF*T0001"),
                               Tariff_Id.Parse("DE*GEF*T0002")
                           ],
                           URL.Parse("https://open.charging.cloud/terms"),
                           DateTime.Parse("2020-09-21")
                       ),
                       new OCPIv2_2_1.Connector(
                           Connector_Id.Parse("2"),
                           OCPIv2_2_1.ConnectorType.IEC_62196_T2_COMBO,
                           OCPIv2_2_1.ConnectorFormats.CABLE,
                           OCPIv2_2_1.PowerTypes.AC_3_PHASE,
                           Volt.  ParseV(400),
                           Ampere.ParseA(20),
                           Watt.  ParseW(8),
                           [
                               Tariff_Id.Parse("DE*GEF*T0003"),
                               Tariff_Id.Parse("DE*GEF*T0004")
                           ],
                           URL.Parse("https://open.charging.cloud/terms"),
                           DateTime.Parse("2020-09-22")
                       )
                   ],
                   EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                   [
                       new OCPIv2_2_1.StatusSchedule(
                           OCPIv2_2_1.StatusType.INOPERATIVE,
                           DateTime.Parse("2020-09-23"),
                           DateTime.Parse("2020-09-24")
                       ),
                       new OCPIv2_2_1.StatusSchedule(
                           OCPIv2_2_1.StatusType.OUTOFORDER,
                           DateTime.Parse("2020-12-30"),
                           DateTime.Parse("2020-12-31")
                       )
                   ],
                   [
                       OCPIv2_2_1.Capability.RFID_READER,
                       OCPIv2_2_1.Capability.RESERVABLE
                   ],

                   // OCPI Computer Science Extensions
                   new EnergyMeter<OCPIv2_2_1.EVSE>(
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
                       OCPIv2_2_1.ParkingRestriction.EV_ONLY,
                       OCPIv2_2_1.ParkingRestriction.PLUGGED
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

                   LastUpdated: DateTime.Parse("2020-09-22")

               );

        #endregion

        #region EVSE1_v2_3_0

        private static OCPIv2_3_0.EVSE EVSE1_v2_3_0

            => new (

                   EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                   OCPIv2_3_0.StatusType.AVAILABLE,
                   [
                       new OCPIv2_3_0.Connector(
                           Connector_Id.Parse("1"),
                           OCPIv2_3_0.ConnectorType.IEC_62196_T2,
                           OCPIv2_3_0.ConnectorFormats.SOCKET,
                           OCPIv2_3_0.PowerTypes.AC_3_PHASE,
                           Volt.  ParseV(400),
                           Ampere.ParseA(30),
                           Watt.  ParseW(12),
                           [
                               Tariff_Id.Parse("DE*GEF*T0001"),
                               Tariff_Id.Parse("DE*GEF*T0002")
                           ],
                           URL.Parse("https://open.charging.cloud/terms"),
                           [ OCPIv2_3_0.ConnectorCapability.ISO_15118_2_PLUG_AND_CHARGE ],
                           DateTime.Parse("2020-09-21"),
                           DateTime.Parse("2020-09-21")
                       ),
                       new OCPIv2_3_0.Connector(
                           Connector_Id.Parse("2"),
                           OCPIv2_3_0.ConnectorType.IEC_62196_T2_COMBO,
                           OCPIv2_3_0.ConnectorFormats.CABLE,
                           OCPIv2_3_0.PowerTypes.AC_3_PHASE,
                           Volt.  ParseV(400),
                           Ampere.ParseA(20),
                           Watt.  ParseW(8),
                           [
                               Tariff_Id.Parse("DE*GEF*T0003"),
                               Tariff_Id.Parse("DE*GEF*T0004")
                           ],
                           URL.Parse("https://open.charging.cloud/terms"),
                           [ OCPIv2_3_0.ConnectorCapability.ISO_15118_20_PLUG_AND_CHARGE ],
                           DateTime.Parse("2020-09-22"),
                           DateTime.Parse("2020-09-22")
                       )
                   ],

                   EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                   [
                       new OCPIv2_3_0.StatusSchedule(
                           OCPIv2_3_0.StatusType.INOPERATIVE,
                           DateTime.Parse("2020-09-23"),
                           DateTime.Parse("2020-09-24")
                       ),
                       new OCPIv2_3_0.StatusSchedule(
                           OCPIv2_3_0.StatusType.OUTOFORDER,
                           DateTime.Parse("2020-12-30"),
                           DateTime.Parse("2020-12-31")
                       )
                   ],
                   [
                       OCPIv2_3_0.Capability.RFID_READER,
                       OCPIv2_3_0.Capability.RESERVABLE
                   ],

                   // OCPI Computer Science Extensions
                   new EnergyMeter<OCPIv2_3_0.EVSE>(
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
                   [ OCPIv2_3_0.ParkingRestriction.CUSTOMERS ],
                   [
                       new OCPIv2_3_0.EVSEParking(
                           Parking_Id.Parse("1"),
                           OCPIv2_3_0.EVSEPosition.CENTER
                       )
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
                   [EMSP_Id.Parse("DE*GDF")],
                   Created:     DateTime.Parse("2020-09-22"),
                   LastUpdated: DateTime.Parse("2020-09-22")

               );

        #endregion



        #region CPO1_PutLocation_OCPIv2_1_1_Test1()

        /// <summary>
        /// CPO1_PutLocation_OCPIv2_1_1_Test.
        /// </summary>
        [Test]
        public async Task CPO1_PutLocation_OCPIv2_1_1_Test1()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_1_1?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.PutLocation(
                                         Location1_v2_1_1
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
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(201),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                //if (response.Data is not null)
                //{
                //}

            }

        }

        #endregion

        #region CPO1_PutLocation_OCPIv2_2_1_Test1()

        /// <summary>
        /// CPO1_PutLocation_OCPIv2_2_1_Test.
        /// </summary>
        [Test]
        public async Task CPO1_PutLocation_OCPIv2_2_1_Test1()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_2_1?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.PutLocation(
                                         Location1_v2_2_1
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
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(201),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {
                }

            }

        }

        #endregion

        #region CPO1_PutLocation_OCPIv2_3_0_Test1()

        /// <summary>
        /// CPO1_PutLocation_OCPIv2_3_0_Test.
        /// </summary>
        [Test]
        public async Task CPO1_PutLocation_OCPIv2_3_0_Test1()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_3_0?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response = await graphDefinedEMSP1.PutLocation(
                                         Location1_v2_3_0
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
                Assert.That(response.StatusCode,                                            Is.EqualTo(1000), response.StatusMessage);
                Assert.That(response.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(201),  response.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response.Data is not null)
                {
                }

            }

        }

        #endregion


        #region CPO1_PutLocationAndEVSE_OCPIv2_1_1_Test1()

        /// <summary>
        /// CPO1_PutLocationAndEVSE_OCPIv2_1_1_Test1.
        /// </summary>
        [Test]
        public async Task CPO1_PutLocationAndEVSE_OCPIv2_1_1_Test1()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_1_1?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response1 = await graphDefinedEMSP1.PutLocation(
                                          Location1_v2_1_1
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

                Assert.That(response1,                                                       Is.Not.Null);
                Assert.That(response1.StatusCode,                                            Is.EqualTo(1000), response1.StatusMessage);
                Assert.That(response1.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response1.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(201),  response1.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response1.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response1.Data is not null)
                {
                }


                var response2 = await graphDefinedEMSP1.PutEVSE(
                                          EVSE1_v2_1_1,
                                          Location1_v2_1_1.Id
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

                Assert.That(response2,                                                       Is.Not.Null);
                Assert.That(response2.StatusCode,                                            Is.EqualTo(1000), response2.StatusMessage);
                Assert.That(response2.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response2.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(201),  response2.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response2.Data is not null)
                {
                }

            }

        }

        #endregion

        #region CPO1_PutLocationAndEVSE_OCPIv2_2_1_Test1()

        /// <summary>
        /// CPO1_PutLocationAndEVSE_OCPIv2_2_1_Test1.
        /// </summary>
        [Test]
        public async Task CPO1_PutLocationAndEVSE_OCPIv2_2_1_Test1()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_2_1?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response1 = await graphDefinedEMSP1.PutLocation(
                                          Location1_v2_2_1
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

                Assert.That(response1,                                                       Is.Not.Null);
                Assert.That(response1.StatusCode,                                            Is.EqualTo(1000), response1.StatusMessage);
                Assert.That(response1.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response1.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(201),  response1.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response1.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response1.Data is not null)
                {
                }


                var response2 = await graphDefinedEMSP1.PutEVSE(
                                          EVSE1_v2_2_1,
                                          CountryCode.Parse("DE"),
                                          Party_Id.   Parse("GEF"),
                                          Location1_v2_2_1.Id
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

                Assert.That(response2,                                                       Is.Not.Null);
                Assert.That(response2.StatusCode,                                            Is.EqualTo(1000), response2.StatusMessage);
                Assert.That(response2.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response2.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(201),  response2.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response2.Data is not null)
                {
                }

            }

        }

        #endregion

        #region CPO1_PutLocationAndEVSE_OCPIv2_3_0_Test1()

        /// <summary>
        /// CPO1_PutLocationAndEVSE_OCPIv2_3_0_Test1.
        /// </summary>
        [Test]
        public async Task CPO1_PutLocationAndEVSE_OCPIv2_3_0_Test1()
        {

            var graphDefinedEMSP1 = cpo1CPOAPI_v2_3_0?.GetEMSPClient(
                                        CountryCode: CountryCode.Parse("DE"),
                                        PartyId:     Party_Id.   Parse("GDF")
                                    );

            Assert.That(graphDefinedEMSP1, Is.Not.Null);

            if (graphDefinedEMSP1 is not null)
            {

                var response1 = await graphDefinedEMSP1.PutLocation(
                                          Location1_v2_3_0
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

                Assert.That(response1,                                                       Is.Not.Null);
                Assert.That(response1.StatusCode,                                            Is.EqualTo(1000), response1.StatusMessage);
                Assert.That(response1.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response1.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(201),  response1.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response1.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response1.Data is not null)
                {
                }


                var response2 = await graphDefinedEMSP1.PutEVSE(
                                          EVSE1_v2_3_0,
                                          CountryCode.Parse("DE"),
                                          Party_Id.   Parse("GEF"),
                                          Location1_v2_3_0.Id
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

                Assert.That(response2,                                                       Is.Not.Null);
                Assert.That(response2.StatusCode,                                            Is.EqualTo(1000), response2.StatusMessage);
                Assert.That(response2.StatusMessage,                                         Is.EqualTo("Hello world!"));
                Assert.That(response2.HTTPResponse?.HTTPStatusCode.Code,                     Is.EqualTo(201),  response2.HTTPResponse?.HTTPBodyAsUTF8String);
                Assert.That(Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10),  Is.True);

                if (response2.Data is not null)
                {
                }

            }

        }

        #endregion


    }

}
