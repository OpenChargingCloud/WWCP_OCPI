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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.Datastructures
{

    /// <summary>
    /// Unit tests for locations.
    /// https://github.com/ocpi/ocpi/blob/master/mod_locations.asciidoc
    /// </summary>
    [TestFixture]
    public static class LocationTests
    {

        #region Location_SerializeDeserialize_Test01()

        /// <summary>
        /// Location serialize, deserialize and compare test.
        /// </summary>
        [Test]
        public static void Location_SerializeDeserialize_Test01()
        {

            #region Define Location1

            var Location1 = new Location(
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (30),
                                                Watt.  FromKW(12),
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (20),
                                                Watt.  FromKW(8),
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
                                        [ ParkingRestriction.CUSTOMERS ],
                                        [ new EVSEParking(Parking_Id.Parse("1"), EVSEPosition.CENTER) ],
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
                                [ Facility.CAFE ],
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
                            );

            #endregion

            var JSON = Location1.ToJSON();

            Assert.That(JSON["country_code"].Value<String>(), Is.EqualTo("DE"));
            Assert.That(JSON["party_id"].    Value<String>(), Is.EqualTo("GEF"));
            Assert.That(JSON["id"].          Value<String>(), Is.EqualTo("LOC0001"));

            Assert.That(Location.TryParse(JSON, out var Location2, out var ErrorResponse), Is.True);
            Assert.That(ErrorResponse,                                                     Is.Null);

            Assert.That(Location2.CountryCode, Is.EqualTo(Location1.CountryCode));
            Assert.That(Location2.PartyId,     Is.EqualTo(Location1.PartyId));
            Assert.That(Location2.Id,          Is.EqualTo(Location1.Id));

            Assert.That(Location2.Publish,     Is.EqualTo(Location1.Publish));
            Assert.That(Location2.Address,     Is.EqualTo(Location1.Address));
            Assert.That(Location2.City,        Is.EqualTo(Location1.City));
            Assert.That(Location2.Country,     Is.EqualTo(Location1.Country));
            Assert.That(Location2.Coordinates, Is.EqualTo(Location1.Coordinates));
            Assert.That(Location2.TimeZone,    Is.EqualTo(Location1.TimeZone));

            Assert.That(Location2.PublishAllowedTo, Is.EqualTo(Location1.PublishAllowedTo));
            Assert.That(Location2.Name,             Is.EqualTo(Location1.Name));
            Assert.That(Location2.PostalCode,       Is.EqualTo(Location1.PostalCode));
            Assert.That(Location2.State,            Is.EqualTo(Location1.State));
            Assert.That(Location2.RelatedLocations, Is.EqualTo(Location1.RelatedLocations));
            Assert.That(Location2.ParkingType,      Is.EqualTo(Location1.ParkingType));
            //ClassicAssert.AreEqual(LocationA.EVSEs,                    LocationB.EVSEs);
            Assert.That(Location2.Directions,  Is.EqualTo(Location1.Directions));
            Assert.That(Location2.Operator,    Is.EqualTo(Location1.Operator));
            Assert.That(Location2.SubOperator, Is.EqualTo(Location1.SubOperator));
            Assert.That(Location2.Owner,       Is.EqualTo(Location1.Owner));
            Assert.That(Location2.Facilities,  Is.EqualTo(Location1.Facilities));
            //ClassicAssert.AreEqual(LocationA.OpeningTimes,             LocationB.OpeningTimes);
            Assert.That(Location2.ChargingWhenClosed, Is.EqualTo(Location1.ChargingWhenClosed));
            Assert.That(Location2.Images,             Is.EqualTo(Location1.Images));
            Assert.That(Location2.EnergyMix,          Is.EqualTo(Location1.EnergyMix));

            Assert.That(Location2.LastUpdated.ToISO8601(), Is.EqualTo(Location1.LastUpdated.ToISO8601()));

        }

        #endregion


        #region Location_PATCH_CountryCode()

        /// <summary>
        /// Try to PATCH the country code of a location.
        /// </summary>
        [Test]
        public static void Location_PATCH_CountryCode()
        {

            #region Define Location1

            var Location1 = new Location(
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (30),
                                                Watt.  FromKW(12),
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (20),
                                                Watt.  FromKW(8),
                                                [
                                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                                    Tariff_Id.Parse("DE*GEF*T0004")
                                                ],
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                [ ConnectorCapability.ISO_15118_20_PLUG_AND_CHARGE ],
                                                DateTime.Parse("2020-09-21"),
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
                                        [ ParkingRestriction.CUSTOMERS ],
                                        [ new EVSEParking(Parking_Id.Parse("1"), EVSEPosition.CENTER) ],
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
                                [ Facility.CAFE ],
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
                                Created: DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""country_code"": ""FR"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.False);
            Assert.That(patchResult.IsFailed,      Is.True);
            Assert.That(patchResult.ErrorResponse, Is.Not.Null);
            Assert.That(patchResult.ErrorResponse, Is.EqualTo("Patching the 'country code' of a charging location is not allowed!"));
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Location_Id.Parse("LOC0001")));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-09-21T00:00:00.000Z"));

        }

        #endregion

        #region Location_PATCH_PartyId()

        /// <summary>
        /// Try to PATCH the party identification of a location.
        /// </summary>
        [Test]
        public static void Location_PATCH_PartyId()
        {

            #region Define Location1

            var Location1 = new Location(
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (30),
                                                Watt.  FromKW(12),
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (20),
                                                Watt.  FromKW(8),
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
                                        [ ParkingRestriction.CUSTOMERS ],
                                        [ new EVSEParking(Parking_Id.Parse("1"), EVSEPosition.CENTER) ],
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
                                [ Facility.CAFE ],
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
                                Created: DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""party_id"": ""GDF"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.False);
            Assert.That(patchResult.IsFailed,      Is.True);
            Assert.That(patchResult.ErrorResponse, Is.Not.Null);
            Assert.That(patchResult.ErrorResponse, Is.EqualTo("Patching the 'party identification' of a charging location is not allowed!"));
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Location_Id.Parse("LOC0001")));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-09-21T00:00:00.000Z"));

        }

        #endregion

        #region Location_PATCH_LocationId()

        /// <summary>
        /// Try to PATCH the location identification.
        /// </summary>
        [Test]
        public static void Location_PATCH_LocationId()
        {

            #region Define Location1

            var Location1 = new Location(
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (30),
                                                Watt.  FromKW(12),
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (20),
                                                Watt.  FromKW(8),
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
                                        [ ParkingRestriction.CUSTOMERS ],
                                        [ new EVSEParking(Parking_Id.Parse("1"), EVSEPosition.CENTER) ],
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
                                [ Facility.CAFE ],
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
                                Created: DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""id"": ""2"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.False);
            Assert.That(patchResult.IsFailed,      Is.True);
            Assert.That(patchResult.ErrorResponse, Is.Not.Null);
            Assert.That(patchResult.ErrorResponse, Is.EqualTo("Patching the 'identification' of a charging location is not allowed!"));
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Location_Id.Parse("LOC0001")));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-09-21T00:00:00.000Z"));

        }

        #endregion

        #region Location_PATCH_minimal()

        /// <summary>
        /// Minimal location PATCH test.
        /// </summary>
        [Test]
        public static void Location_PATCH_minimal()
        {

            #region Define Location1

            var Location1 = new Location(
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (30),
                                                Watt.  FromKW(12),
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (20),
                                                Watt.  FromKW(8),
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
                                        [ ParkingRestriction.CUSTOMERS ],
                                        [ new EVSEParking(Parking_Id.Parse("1"), EVSEPosition.CENTER) ],
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
                                [ Facility.CAFE ],
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
                                Created: DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""name"": ""Location 0001a"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,          Is.EqualTo(Location_Id.Parse("LOC0001")));
            Assert.That(patchResult.PatchedData.Name,        Is.EqualTo("Location 0001a"));
            Assert.That(patchResult.PatchedData.LastUpdated, Is.Not.EqualTo(DateTimeOffset.Parse("2020-09-21T00:00:00Z")));

            Assert.That(Timestamp.Now - patchResult.PatchedData.LastUpdated < TimeSpan.FromSeconds(5), Is.True);

        }

        #endregion

        #region Location_PATCH_withLastUpdated()

        /// <summary>
        /// Minimal location PATCH test, but with last_updated parameter.
        /// </summary>
        [Test]
        public static void Location_PATCH_withLastUpdated()
        {

            #region Define Location1

            var Location1 = new Location(
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (30),
                                                Watt.  FromKW(12),
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (20),
                                                Watt.  FromKW(8),
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
                                        [ ParkingRestriction.CUSTOMERS ],
                                        [ new EVSEParking(Parking_Id.Parse("1"), EVSEPosition.CENTER) ],
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
                                [ Facility.CAFE ],
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
                                Created: DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""name"": ""Location 0001a"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Location_Id.Parse("LOC0001")));
            Assert.That(patchResult.PatchedData.Name,                    Is.EqualTo("Location 0001a"));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-10-15T00:00:00.000Z"));

        }

        #endregion

        #region Location_PATCH_FacilitiesArray()

        /// <summary>
        /// Try to PATCH the facilities array of a location.
        /// </summary>
        [Test]
        public static void Location_PATCH_FacilitiesArray()
        {

            #region Define Location1

            var Location1 = new Location(
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (30),
                                                Watt.  FromKW(12),
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (20),
                                                Watt.  FromKW(8),
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
                                        [ ParkingRestriction.CUSTOMERS ],
                                        [ new EVSEParking(Parking_Id.Parse("1"), EVSEPosition.CENTER) ],
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
                                [ Facility.CAFE ],
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
                                Created: DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""facilities"": [ ""CAFE"", ""AIRPORT"" ], ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                         Is.EqualTo(Location_Id.Parse("LOC0001")));
            Assert.That(patchResult.PatchedData.Name,                       Is.EqualTo("Location 0001"));
            Assert.That(patchResult.PatchedData.Facilities.        Count(), Is.EqualTo(2));
            Assert.That(patchResult.PatchedData.Facilities.        First(), Is.EqualTo(Facility.Parse("CAFE")));
            Assert.That(patchResult.PatchedData.Facilities.Skip(1).First(), Is.EqualTo(Facility.Parse("AIRPORT")));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(),    Is.EqualTo("2020-10-15T00:00:00.000Z"));

        }

        #endregion

        #region Location_PATCH_RemoveFacilitiesArray()

        /// <summary>
        /// Try to remove the facilities array of a location via PATCH.
        /// </summary>
        [Test]
        public static void Location_PATCH_RemoveFacilitiesArray()
        {

            #region Define Location1

            var Location1 = new Location(
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (30),
                                                Watt.  FromKW(12),
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (20),
                                                Watt.  FromKW(8),
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
                                        [ ParkingRestriction.CUSTOMERS ],
                                        [ new EVSEParking(Parking_Id.Parse("1"), EVSEPosition.CENTER) ],
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
                                [ Facility.CAFE ],
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
                                Created: DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""facilities"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Location_Id.Parse("LOC0001")));
            Assert.That(patchResult.PatchedData.Name,                    Is.EqualTo("Location 0001"));
            Assert.That(patchResult.PatchedData.Facilities. Count(),     Is.EqualTo(0));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-10-15T00:00:00.000Z"));

        }

        #endregion

        #region Location_PATCH_RemoveName()

        /// <summary>
        /// Try to remove the 'name' of a location via PATCH.
        /// </summary>
        [Test]
        public static void Location_PATCH_RemoveName()
        {

            #region Define Location1

            var Location1 = new Location(
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (30),
                                                Watt.  FromKW(12),
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (20),
                                                Watt.  FromKW(8),
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
                                        [ ParkingRestriction.CUSTOMERS ],
                                        [ new EVSEParking(Parking_Id.Parse("1"), EVSEPosition.CENTER) ],
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
                                [ Facility.CAFE ],
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
                                Created: DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""name"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.That(patchResult.IsSuccess,     Is.True);
            Assert.That(patchResult.IsFailed,      Is.False);
            Assert.That(patchResult.ErrorResponse, Is.Null);
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Location_Id.Parse("LOC0001")));
            Assert.That(patchResult.PatchedData.Name.IsNullOrEmpty(),    Is.True);
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-10-15T00:00:00.000Z"));

        }

        #endregion

        #region Location_PATCH_InvalidEVSESPatch()

        /// <summary>
        /// Invalid EVSEs PATCH of a location.
        /// </summary>
        [Test]
        public static void Location_PATCH_InvalidEVSESPatch()
        {

            #region Define Location1

            var Location1 = new Location(
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (30),
                                                Watt.  FromKW(12),
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (20),
                                                Watt.  FromKW(8),
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
                                        [ ParkingRestriction.CUSTOMERS ],
                                        [ new EVSEParking(Parking_Id.Parse("1"), EVSEPosition.CENTER) ],
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
                                [ Facility.CAFE ],
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
                                Created: DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""evses"": ""I-N-V-A-L-I-D!"" }"));

            Assert.That(patchResult.IsSuccess,     Is.False);
            Assert.That(patchResult.IsFailed,      Is.True);
            Assert.That(patchResult.ErrorResponse, Is.Not.Null);
            Assert.That(patchResult.ErrorResponse, Is.EqualTo("Patching the 'evses' array of a charging location is not allowed!"));
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Location_Id.Parse("LOC0001")));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-09-21T00:00:00.000Z"));

        }

        #endregion

        #region Location_PATCH_InvalidOperatorPatch()

        /// <summary>
        /// Invalid operator PATCH of a location.
        /// </summary>
        [Test]
        public static void Location_PATCH_InvalidOperatorPatch()
        {

            #region Define Location1

            var Location1 = new Location(
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (30),
                                                Watt.  FromKW(12),
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (20),
                                                Watt.  FromKW(8),
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
                                        [ ParkingRestriction.CUSTOMERS ],
                                        [ new EVSEParking(Parking_Id.Parse("1"), EVSEPosition.CENTER) ],
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
                                [ Facility.CAFE ],
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
                                Created: DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""operator"": ""I-N-V-A-L-I-D!"" }"));

            Assert.That(patchResult.IsSuccess,     Is.False);
            Assert.That(patchResult.IsFailed,      Is.True);
            Assert.That(patchResult.ErrorResponse, Is.Not.Null);
            Assert.That(patchResult.ErrorResponse, Is.EqualTo("Invalid JSON merge patch of a charging location: Invalid operator!"));
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Location_Id.Parse("LOC0001")));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-09-21T00:00:00.000Z"));

        }

        #endregion

        #region Location_PATCH_InvalidLastUpdatedPatch()

        /// <summary>
        /// Invalid 'last_updated' PATCH of a location.
        /// </summary>
        [Test]
        public static void Location_PATCH_InvalidLastUpdatedPatch()
        {

            #region Define Location1

            var Location1 = new Location(
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (30),
                                                Watt.  FromKW(12),
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
                                                Volt.  FromV(400),
                                                Ampere.FromA (20),
                                                Watt.  FromKW(8),
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
                                        [ ParkingRestriction.CUSTOMERS ],
                                        [ new EVSEParking(Parking_Id.Parse("1"), EVSEPosition.CENTER) ],
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
                                [ Facility.CAFE ],
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
                                Created: DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""last_updated"": ""I-N-V-A-L-I-D!"" }"));

            Assert.That(patchResult.IsSuccess,     Is.False);
            Assert.That(patchResult.IsFailed,      Is.True);
            Assert.That(patchResult.ErrorResponse, Is.Not.Null);
            Assert.That(patchResult.ErrorResponse, Is.EqualTo("Invalid JSON merge patch of a charging location: Invalid 'last updated'!"));
            Assert.That(patchResult.PatchedData,   Is.Not.Null);

            Assert.That(patchResult.PatchedData.Id,                      Is.EqualTo(Location_Id.Parse("LOC0001")));
            Assert.That(patchResult.PatchedData.LastUpdated.ToISO8601(), Is.EqualTo("2020-09-21T00:00:00.000Z"));

        }

        #endregion



        #region Location_DeserializeGitHub_Test01()

        /// <summary>
        /// Tries to deserialize a location example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.3.0-bugfixes/examples/location_example.json
        /// </summary>
        [Test]
        public static void Location_DeserializeGitHub_Test01()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"": ""BE"",
                           ""party_id"": ""BEC"",
                           ""id"": ""LOC1"",
                           ""publish"": true,
                           ""name"": ""Gent Zuid"",
                           ""address"": ""F.Rooseveltlaan 3A"",
                           ""city"": ""Gent"",
                           ""postal_code"": ""9000"",
                           ""country"": ""BEL"",
                           ""coordinates"": {
                             ""latitude"": ""51.047599"",
                             ""longitude"": ""3.729944""
                           },
                           ""parking_type"": ""ON_STREET"",
                           ""evses"": [{
                             ""uid"": ""3256"",
                             ""evse_id"": ""BE*BEC*E041503001"",
                             ""status"": ""AVAILABLE"",
                             ""status_schedule"": [],
                             ""capabilities"": [
                               ""RESERVABLE""
                             ],
                             ""connectors"": [{
                               ""id"": ""1"",
                               ""standard"": ""IEC_62196_T2"",
                               ""format"": ""CABLE"",
                               ""power_type"": ""AC_3_PHASE"",
                               ""max_voltage"": 220,
                               ""max_amperage"": 16,
                               ""tariff_ids"": [""11""],
                               ""last_updated"": ""2015-03-16T10:10:02Z""
                             }, {
                               ""id"": ""2"",
                               ""standard"": ""IEC_62196_T2"",
                               ""format"": ""SOCKET"",
                               ""power_type"": ""AC_3_PHASE"",
                               ""max_voltage"": 220,
                               ""max_amperage"": 16,
                               ""tariff_ids"": [""13""],
                               ""last_updated"": ""2015-03-18T08:12:01Z""
                             }],
                             ""physical_reference"": ""1"",
                             ""floor_level"": ""-1"",
                             ""last_updated"": ""2015-06-28T08:12:01Z""
                           }, {
                             ""uid"": ""3257"",
                             ""evse_id"": ""BE*BEC*E041503002"",
                             ""status"": ""RESERVED"",
                             ""capabilities"": [
                               ""RESERVABLE""
                             ],
                             ""connectors"": [{
                               ""id"": ""1"",
                               ""standard"": ""IEC_62196_T2"",
                               ""format"": ""SOCKET"",
                               ""power_type"": ""AC_3_PHASE"",
                               ""max_voltage"": 220,
                               ""max_amperage"": 16,
                               ""tariff_ids"": [""12""],
                               ""last_updated"": ""2015-06-29T20:39:09Z""
                             }],
                             ""physical_reference"": ""2"",
                             ""floor_level"": ""-2"",
                             ""last_updated"": ""2015-06-29T20:39:09Z""
                           }],
                           ""operator"": {
                             ""name"": ""BeCharged""
                           },
                           ""time_zone"": ""Europe/Brussels"",
                           ""last_updated"": ""2015-06-29T20:39:09Z""
                         }";

            #endregion

            var result = Location.TryParse(JObject.Parse(JSON), out var parsedLocation, out var ErrorResponse);
            Assert.That(ErrorResponse, Is.Null);
            Assert.That(result,        Is.True);

            Assert.That(parsedLocation.CountryCode, Is.EqualTo(CountryCode.Parse("BE")));
            Assert.That(parsedLocation.PartyId,     Is.EqualTo(Party_Id.   Parse("BEC")));
            Assert.That(parsedLocation.Id,          Is.EqualTo(Location_Id.Parse("LOC1")));
            Assert.That(parsedLocation.Publish,     Is.EqualTo(true));
            //ClassicAssert.AreEqual(Location1.Start.    ToISO8601(),                            parsedLocation.Start.    ToISO8601());
            //ClassicAssert.AreEqual(Location1.End.Value.ToISO8601(),                            parsedLocation.End.Value.ToISO8601());
            //ClassicAssert.AreEqual(Location1.kWh,                                              parsedLocation.kWh);
            //ClassicAssert.AreEqual(Location1.CDRToken,                                         parsedLocation.CDRToken);
            //ClassicAssert.AreEqual(Location1.AuthMethod,                                       parsedLocation.AuthMethod);
            //ClassicAssert.AreEqual(Location1.AuthorizationReference,                           parsedLocation.AuthorizationReference);
            //ClassicAssert.AreEqual(Location1.LocationId,                                       parsedLocation.LocationId);
            //ClassicAssert.AreEqual(Location1.EVSEUId,                                          parsedLocation.EVSEUId);
            //ClassicAssert.AreEqual(Location1.ConnectorId,                                      parsedLocation.ConnectorId);
            //ClassicAssert.AreEqual(Location1.MeterId,                                          parsedLocation.MeterId);
            //ClassicAssert.AreEqual(Location1.EnergyMeter,                                      parsedLocation.EnergyMeter);
            //ClassicAssert.AreEqual(Location1.TransparencySoftware,                            parsedLocation.TransparencySoftware);
            //ClassicAssert.AreEqual(Location1.Currency,                                         parsedLocation.Currency);
            //ClassicAssert.AreEqual(Location1.ChargingPeriods,                                  parsedLocation.ChargingPeriods);
            //ClassicAssert.AreEqual(Location1.TotalCosts,                                       parsedLocation.TotalCosts);
            //ClassicAssert.AreEqual(Location1.Status,                                           parsedLocation.Status);
            //ClassicAssert.AreEqual(Location1.LastUpdated.ToISO8601(),                          parsedLocation.LastUpdated.ToISO8601());

        }

        #endregion

        #region Location_DeserializeGitHub_Test02()

        /// <summary>
        /// Tries to deserialize a location example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.3.0-bugfixes/examples/location_example_parking_garage_opening_hours.json
        /// </summary>
        [Test]
        public static void Location_DeserializeGitHub_Test02()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"": ""SE"",
                           ""party_id"": ""EVC"",
                           ""id"": ""cbb0df21-d17d-40ba-a4aa-dc588c8f98cb"",
                           ""publish"": true,
                           ""name"": ""P-Huset Leonard"",
                           ""address"": ""Claesgatan 6"",
                           ""city"": ""Malmö"",
                           ""postal_code"": ""214 26"",
                           ""country"": ""SWE"",
                           ""coordinates"": {
                             ""latitude"": ""55.590325"",
                             ""longitude"": ""13.008307""
                           },
                           ""parking_type"": ""PARKING_GARAGE"",
                           ""evses"": [{
                             ""uid"": ""eccb8dd9-4189-433e-b100-cc0945dd17dc"",
                             ""evse_id"": ""SE*EVC*E000000123"",
                             ""status"": ""AVAILABLE"",
                             ""connectors"": [{
                               ""id"": ""1"",
                               ""standard"": ""IEC_62196_T2"",
                               ""format"": ""SOCKET"",
                               ""power_type"": ""AC_3_PHASE"",
                               ""max_voltage"": 230,
                               ""max_amperage"": 32,
                               ""last_updated"": ""2017-03-07T02:21:22Z""
                             }],
                             ""last_updated"": ""2017-03-07T02:21:22Z""
                           }],
                           ""time_zone"": ""Europe/Stockholm"",
                           ""opening_times"": {
                             ""twentyfourseven"": false,
                             ""regular_hours"": [{
                               ""weekday"": 1,
                               ""period_begin"": ""07:00"",
                               ""period_end"": ""18:00""
                             }, {
                               ""weekday"": 2,
                               ""period_begin"": ""07:00"",
                               ""period_end"": ""18:00""
                             },{
                               ""weekday"": 3,
                               ""period_begin"": ""07:00"",
                               ""period_end"": ""18:00""
                             },{
                               ""weekday"": 4,
                               ""period_begin"": ""07:00"",
                               ""period_end"": ""18:00""
                             },{
                               ""weekday"": 5,
                               ""period_begin"": ""07:00"",
                               ""period_end"": ""18:00""
                             },{
                               ""weekday"": 6,
                               ""period_begin"": ""07:00"",
                               ""period_end"": ""18:00""
                             },{
                               ""weekday"": 7,
                               ""period_begin"": ""07:00"",
                               ""period_end"": ""18:00""
                             }]
                           },
                           ""charging_when_closed"": true,
                           ""last_updated"": ""2017-03-07T02:21:22Z""
                         }";

            #endregion

            var result = Location.TryParse(JObject.Parse(JSON), out var parsedLocation, out var ErrorResponse);
            Assert.That(ErrorResponse, Is.Null);
            Assert.That(result,        Is.True);

            Assert.That(parsedLocation.CountryCode, Is.EqualTo(CountryCode.Parse("SE")));
            Assert.That(parsedLocation.PartyId,     Is.EqualTo(Party_Id.   Parse("EVC")));
            Assert.That(parsedLocation.Id,          Is.EqualTo(Location_Id.Parse("cbb0df21-d17d-40ba-a4aa-dc588c8f98cb")));
            Assert.That(parsedLocation.Publish,     Is.EqualTo(true));
            //ClassicAssert.AreEqual(Location1.Start.    ToISO8601(),                            parsedLocation.Start.    ToISO8601());
            //ClassicAssert.AreEqual(Location1.End.Value.ToISO8601(),                            parsedLocation.End.Value.ToISO8601());
            //ClassicAssert.AreEqual(Location1.kWh,                                              parsedLocation.kWh);
            //ClassicAssert.AreEqual(Location1.CDRToken,                                         parsedLocation.CDRToken);
            //ClassicAssert.AreEqual(Location1.AuthMethod,                                       parsedLocation.AuthMethod);
            //ClassicAssert.AreEqual(Location1.AuthorizationReference,                           parsedLocation.AuthorizationReference);
            //ClassicAssert.AreEqual(Location1.LocationId,                                       parsedLocation.LocationId);
            //ClassicAssert.AreEqual(Location1.EVSEUId,                                          parsedLocation.EVSEUId);
            //ClassicAssert.AreEqual(Location1.ConnectorId,                                      parsedLocation.ConnectorId);
            //ClassicAssert.AreEqual(Location1.MeterId,                                          parsedLocation.MeterId);
            //ClassicAssert.AreEqual(Location1.EnergyMeter,                                      parsedLocation.EnergyMeter);
            //ClassicAssert.AreEqual(Location1.TransparencySoftware,                            parsedLocation.TransparencySoftware);
            //ClassicAssert.AreEqual(Location1.Currency,                                         parsedLocation.Currency);
            //ClassicAssert.AreEqual(Location1.ChargingPeriods,                                  parsedLocation.ChargingPeriods);
            //ClassicAssert.AreEqual(Location1.TotalCosts,                                       parsedLocation.TotalCosts);
            //ClassicAssert.AreEqual(Location1.Status,                                           parsedLocation.Status);
            //ClassicAssert.AreEqual(Location1.LastUpdated.ToISO8601(),                          parsedLocation.LastUpdated.ToISO8601());

        }

        #endregion

        #region Location_DeserializeGitHub_Test03()

        /// <summary>
        /// Tries to deserialize a location example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.3.0-bugfixes/examples/location_example_uc2_destination_charger.json
        /// </summary>
        [Test]
        public static void Location_DeserializeGitHub_Test03()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"": ""NL"",
                           ""party_id"": ""ALF"",
                           ""id"": ""3e7b39c2-10d0-4138-a8b3-8509a25f9920"",
                           ""publish"": true,
                           ""name"": ""ihomer"",
                           ""address"": ""Tamboerijn 7"",
                           ""city"": ""Etten-Leur"",
                           ""postal_code"": ""4876 BS"",
                           ""country"": ""NLD"",
                           ""coordinates"": {
                             ""latitude"": ""51.562787"",
                             ""longitude"": ""4.638975""
                           },
                           ""parking_type"": ""PARKING_LOT"",
                           ""evses"": [{
                             ""uid"": ""fd855359-bc81-47bb-bb89-849ae3dac89e"",
                             ""evse_id"": ""NL*ALF*E000000001"",
                             ""status"": ""AVAILABLE"",
                             ""connectors"": [{
                               ""id"": ""1"",
                               ""standard"": ""IEC_62196_T2"",
                               ""format"": ""SOCKET"",
                               ""power_type"": ""AC_3_PHASE"",
                               ""max_voltage"": 220,
                               ""max_amperage"": 16,
                               ""last_updated"": ""2019-07-01T12:12:11Z""
                             }],
                             ""parking_restrictions"": ""CUSTOMERS"",
                             ""last_updated"": ""2019-07-01T12:12:11Z""
                           }],
                           ""time_zone"": ""Europe/Amsterdam"",
                           ""last_updated"": ""2019-07-01T12:12:11Z""
                         }";

            #endregion

            var result = Location.TryParse(JObject.Parse(JSON), out var parsedLocation, out var ErrorResponse);
            Assert.That(ErrorResponse, Is.Null);
            Assert.That(result,        Is.True);

            Assert.That(parsedLocation.CountryCode, Is.EqualTo(CountryCode.Parse("NL")));
            Assert.That(parsedLocation.PartyId,     Is.EqualTo(Party_Id.   Parse("ALF")));
            Assert.That(parsedLocation.Id,          Is.EqualTo(Location_Id.Parse("3e7b39c2-10d0-4138-a8b3-8509a25f9920")));
            Assert.That(parsedLocation.Publish,     Is.EqualTo(true));
            //ClassicAssert.AreEqual(Location1.Start.    ToISO8601(),                            parsedLocation.Start.    ToISO8601());
            //ClassicAssert.AreEqual(Location1.End.Value.ToISO8601(),                            parsedLocation.End.Value.ToISO8601());
            //ClassicAssert.AreEqual(Location1.kWh,                                              parsedLocation.kWh);
            //ClassicAssert.AreEqual(Location1.CDRToken,                                         parsedLocation.CDRToken);
            //ClassicAssert.AreEqual(Location1.AuthMethod,                                       parsedLocation.AuthMethod);
            //ClassicAssert.AreEqual(Location1.AuthorizationReference,                           parsedLocation.AuthorizationReference);
            //ClassicAssert.AreEqual(Location1.LocationId,                                       parsedLocation.LocationId);
            //ClassicAssert.AreEqual(Location1.EVSEUId,                                          parsedLocation.EVSEUId);
            //ClassicAssert.AreEqual(Location1.ConnectorId,                                      parsedLocation.ConnectorId);
            //ClassicAssert.AreEqual(Location1.MeterId,                                          parsedLocation.MeterId);
            //ClassicAssert.AreEqual(Location1.EnergyMeter,                                      parsedLocation.EnergyMeter);
            //ClassicAssert.AreEqual(Location1.TransparencySoftware,                            parsedLocation.TransparencySoftware);
            //ClassicAssert.AreEqual(Location1.Currency,                                         parsedLocation.Currency);
            //ClassicAssert.AreEqual(Location1.ChargingPeriods,                                  parsedLocation.ChargingPeriods);
            //ClassicAssert.AreEqual(Location1.TotalCosts,                                       parsedLocation.TotalCosts);
            //ClassicAssert.AreEqual(Location1.Status,                                           parsedLocation.Status);
            //ClassicAssert.AreEqual(Location1.LastUpdated.ToISO8601(),                          parsedLocation.LastUpdated.ToISO8601());

        }

        #endregion

        #region Location_DeserializeGitHub_Test04()

        /// <summary>
        /// Tries to deserialize a location example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.3.0-bugfixes/examples/location_example_uc3_destination_charger_not_published.json
        /// </summary>
        [Test]
        public static void Location_DeserializeGitHub_Test04()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"": ""NL"",
                           ""party_id"": ""ALF"",
                           ""id"": ""3e7b39c2-10d0-4138-a8b3-8509a25f9920"",
                           ""publish"": false,
                           ""name"": ""ihomer"",
                           ""address"": ""Tamboerijn 7"",
                           ""city"": ""Etten-Leur"",
                           ""postal_code"": ""4876 BS"",
                           ""country"": ""NLD"",
                           ""coordinates"": {
                             ""latitude"": ""51.562787"",
                             ""longitude"": ""4.638975""
                           },
                           ""evses"": [{
                             ""uid"": ""fd855359-bc81-47bb-bb89-849ae3dac89e"",
                             ""evse_id"": ""NL*ALF*E000000001"",
                             ""status"": ""AVAILABLE"",
                             ""connectors"": [{
                               ""id"": ""1"",
                               ""standard"": ""IEC_62196_T2"",
                               ""format"": ""SOCKET"",
                               ""power_type"": ""AC_3_PHASE"",
                               ""max_voltage"": 220,
                               ""max_amperage"": 16,
                               ""last_updated"": ""2019-07-01T12:12:11Z""
                             }],
                             ""parking_restrictions"": ""CUSTOMERS"",
                             ""last_updated"": ""2019-07-01T12:12:11Z""
                           }],
                           ""time_zone"": ""Europe/Amsterdam"",
                           ""last_updated"": ""2019-07-01T12:12:11Z""
                         }";

            #endregion

            var result = Location.TryParse(JObject.Parse(JSON), out var parsedLocation, out var ErrorResponse);
            Assert.That(ErrorResponse, Is.Null);
            Assert.That(result,        Is.True);

            Assert.That(parsedLocation.CountryCode, Is.EqualTo(CountryCode.Parse("NL")));
            Assert.That(parsedLocation.PartyId,     Is.EqualTo(Party_Id.   Parse("ALF")));
            Assert.That(parsedLocation.Id,          Is.EqualTo(Location_Id.Parse("3e7b39c2-10d0-4138-a8b3-8509a25f9920")));
            Assert.That(parsedLocation.Publish,     Is.EqualTo(false));
            //ClassicAssert.AreEqual(Location1.Start.    ToISO8601(),                            parsedLocation.Start.    ToISO8601());
            //ClassicAssert.AreEqual(Location1.End.Value.ToISO8601(),                            parsedLocation.End.Value.ToISO8601());
            //ClassicAssert.AreEqual(Location1.kWh,                                              parsedLocation.kWh);
            //ClassicAssert.AreEqual(Location1.CDRToken,                                         parsedLocation.CDRToken);
            //ClassicAssert.AreEqual(Location1.AuthMethod,                                       parsedLocation.AuthMethod);
            //ClassicAssert.AreEqual(Location1.AuthorizationReference,                           parsedLocation.AuthorizationReference);
            //ClassicAssert.AreEqual(Location1.LocationId,                                       parsedLocation.LocationId);
            //ClassicAssert.AreEqual(Location1.EVSEUId,                                          parsedLocation.EVSEUId);
            //ClassicAssert.AreEqual(Location1.ConnectorId,                                      parsedLocation.ConnectorId);
            //ClassicAssert.AreEqual(Location1.MeterId,                                          parsedLocation.MeterId);
            //ClassicAssert.AreEqual(Location1.EnergyMeter,                                      parsedLocation.EnergyMeter);
            //ClassicAssert.AreEqual(Location1.TransparencySoftware,                            parsedLocation.TransparencySoftware);
            //ClassicAssert.AreEqual(Location1.Currency,                                         parsedLocation.Currency);
            //ClassicAssert.AreEqual(Location1.ChargingPeriods,                                  parsedLocation.ChargingPeriods);
            //ClassicAssert.AreEqual(Location1.TotalCosts,                                       parsedLocation.TotalCosts);
            //ClassicAssert.AreEqual(Location1.Status,                                           parsedLocation.Status);
            //ClassicAssert.AreEqual(Location1.LastUpdated.ToISO8601(),                          parsedLocation.LastUpdated.ToISO8601());

        }

        #endregion

        #region Location_DeserializeGitHub_Test05()

        /// <summary>
        /// Tries to deserialize a location example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.3.0-bugfixes/examples/location_example_uc3_destination_charger_not_published.json
        /// </summary>
        [Test]
        public static void Location_DeserializeGitHub_Test05()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"": ""NL"",
                           ""party_id"": ""ALL"",
                           ""id"": ""f76c2e0c-a6ef-4f67-bf23-6a187e5ca0e0"",
                           ""publish"": false,
                           ""publish_allowed_to"": [{
                             ""visual_number"": ""12345-67"",
                             ""issuer"": ""NewMotion""
                           }, {
                             ""visual_number"": ""0055375624"",
                             ""issuer"": ""ANWB""
                           }, {
                             ""uid"": ""12345678905880"",
                             ""type"": ""RFID""
                           }],
                           ""name"": ""Water State"",
                           ""address"": ""Taco van der Veenplein 12"",
                           ""city"": ""Leeuwarden"",
                           ""postal_code"": ""8923 EM"",
                           ""country"": ""NLD"",
                           ""coordinates"": {
                             ""latitude"": ""53.213763"",
                             ""longitude"": ""5.804638""
                           },
                           ""parking_type"": ""UNDERGROUND_GARAGE"",
                           ""evses"": [{
                             ""uid"": ""8c1b3487-61ac-40a7-a367-21eee99dbd90"",
                             ""evse_id"": ""NL*ALL*EGO0000013"",
                             ""status"": ""AVAILABLE"",
                             ""connectors"": [{
                               ""id"": ""1"",
                               ""standard"": ""IEC_62196_T2"",
                               ""format"": ""SOCKET"",
                               ""power_type"": ""AC_3_PHASE"",
                               ""max_voltage"": 230,
                               ""max_amperage"": 16,
                               ""last_updated"": ""2019-09-27T00:19:45Z""
                             }],
                             ""last_updated"": ""2019-09-27T00:19:45Z""
                           }],
                           ""time_zone"": ""Europe/Amsterdam"",
                           ""last_updated"": ""2019-09-27T00:19:45Z""
                         }";

            #endregion

            var result = Location.TryParse(JObject.Parse(JSON), out var parsedLocation, out var ErrorResponse);
            Assert.That(ErrorResponse, Is.Null);
            Assert.That(result,        Is.True);

            Assert.That(parsedLocation.CountryCode, Is.EqualTo(CountryCode.Parse("NL")));
            Assert.That(parsedLocation.PartyId,     Is.EqualTo(Party_Id.   Parse("ALL")));
            Assert.That(parsedLocation.Id,          Is.EqualTo(Location_Id.Parse("f76c2e0c-a6ef-4f67-bf23-6a187e5ca0e0")));
            Assert.That(parsedLocation.Publish,     Is.EqualTo(false));
            //ClassicAssert.AreEqual(Location1.Start.    ToISO8601(),                            parsedLocation.Start.    ToISO8601());
            //ClassicAssert.AreEqual(Location1.End.Value.ToISO8601(),                            parsedLocation.End.Value.ToISO8601());
            //ClassicAssert.AreEqual(Location1.kWh,                                              parsedLocation.kWh);
            //ClassicAssert.AreEqual(Location1.CDRToken,                                         parsedLocation.CDRToken);
            //ClassicAssert.AreEqual(Location1.AuthMethod,                                       parsedLocation.AuthMethod);
            //ClassicAssert.AreEqual(Location1.AuthorizationReference,                           parsedLocation.AuthorizationReference);
            //ClassicAssert.AreEqual(Location1.LocationId,                                       parsedLocation.LocationId);
            //ClassicAssert.AreEqual(Location1.EVSEUId,                                          parsedLocation.EVSEUId);
            //ClassicAssert.AreEqual(Location1.ConnectorId,                                      parsedLocation.ConnectorId);
            //ClassicAssert.AreEqual(Location1.MeterId,                                          parsedLocation.MeterId);
            //ClassicAssert.AreEqual(Location1.EnergyMeter,                                      parsedLocation.EnergyMeter);
            //ClassicAssert.AreEqual(Location1.TransparencySoftware,                            parsedLocation.TransparencySoftware);
            //ClassicAssert.AreEqual(Location1.Currency,                                         parsedLocation.Currency);
            //ClassicAssert.AreEqual(Location1.ChargingPeriods,                                  parsedLocation.ChargingPeriods);
            //ClassicAssert.AreEqual(Location1.TotalCosts,                                       parsedLocation.TotalCosts);
            //ClassicAssert.AreEqual(Location1.Status,                                           parsedLocation.Status);
            //ClassicAssert.AreEqual(Location1.LastUpdated.ToISO8601(),                          parsedLocation.LastUpdated.ToISO8601());

        }

        #endregion

        #region Location_DeserializeGitHub_Test06()

        /// <summary>
        /// Tries to deserialize a location example from GitHub.
        /// https://github.com/ocpi/ocpi/blob/release-2.3.0-bugfixes/examples/location_example_uc5_home_charge_point.json
        /// </summary>
        [Test]
        public static void Location_DeserializeGitHub_Test06()
        {

            #region Define JSON

            var JSON = @"{
                           ""country_code"": ""DE"",
                           ""party_id"": ""ALL"",
                           ""id"": ""a5295927-09b9-4a71-b4b9-a5fffdfa0b77"",
                           ""publish"": false,
                           ""publish_allowed_to"": [{
                             ""visual_number"": ""0123456-99"",
                             ""issuer"": ""MoveMove""
                           }],
                           ""address"": ""Krautwigstraße 283A"",
                           ""city"": ""Köln"",
                           ""postal_code"": ""50931"",
                           ""country"": ""DEU"",
                           ""coordinates"": {
                             ""latitude"": ""50.931826"",
                             ""longitude"": ""6.964043""
                           },
                           ""parking_type"": ""ON_DRIVEWAY"",
                           ""evses"": [{
                             ""uid"": ""4534ad5f-45be-428b-bfd0-fa489dda932d"",
                             ""evse_id"": ""DE*ALL*EGO0000001"",
                             ""status"": ""AVAILABLE"",
                             ""connectors"": [{
                               ""id"": ""1"",
                               ""standard"": ""IEC_62196_T2"",
                               ""format"": ""SOCKET"",
                               ""power_type"": ""AC_1_PHASE"",
                               ""max_voltage"": 230,
                               ""max_amperage"": 8,
                               ""last_updated"": ""2019-04-05T17:17:56Z""
                             }],
                             ""last_updated"": ""2019-04-05T17:17:56Z""
                           }],
                           ""time_zone"": ""Europe/Berlin"",
                           ""last_updated"": ""2019-04-05T17:17:56Z""
                         }";

            #endregion

            var result = Location.TryParse(JObject.Parse(JSON), out var parsedLocation, out var ErrorResponse);
            Assert.That(ErrorResponse, Is.Null);
            Assert.That(result,        Is.True);

            Assert.That(parsedLocation.CountryCode, Is.EqualTo(CountryCode.Parse("DE")));
            Assert.That(parsedLocation.PartyId,     Is.EqualTo(Party_Id.   Parse("ALL")));
            Assert.That(parsedLocation.Id,          Is.EqualTo(Location_Id.Parse("a5295927-09b9-4a71-b4b9-a5fffdfa0b77")));
            Assert.That(parsedLocation.Publish,     Is.EqualTo(false));
            //ClassicAssert.AreEqual(Location1.Start.    ToISO8601(),                            parsedLocation.Start.    ToISO8601());
            //ClassicAssert.AreEqual(Location1.End.Value.ToISO8601(),                            parsedLocation.End.Value.ToISO8601());
            //ClassicAssert.AreEqual(Location1.kWh,                                              parsedLocation.kWh);
            //ClassicAssert.AreEqual(Location1.CDRToken,                                         parsedLocation.CDRToken);
            //ClassicAssert.AreEqual(Location1.AuthMethod,                                       parsedLocation.AuthMethod);
            //ClassicAssert.AreEqual(Location1.AuthorizationReference,                           parsedLocation.AuthorizationReference);
            //ClassicAssert.AreEqual(Location1.LocationId,                                       parsedLocation.LocationId);
            //ClassicAssert.AreEqual(Location1.EVSEUId,                                          parsedLocation.EVSEUId);
            //ClassicAssert.AreEqual(Location1.ConnectorId,                                      parsedLocation.ConnectorId);
            //ClassicAssert.AreEqual(Location1.MeterId,                                          parsedLocation.MeterId);
            //ClassicAssert.AreEqual(Location1.EnergyMeter,                                      parsedLocation.EnergyMeter);
            //ClassicAssert.AreEqual(Location1.TransparencySoftware,                            parsedLocation.TransparencySoftware);
            //ClassicAssert.AreEqual(Location1.Currency,                                         parsedLocation.Currency);
            //ClassicAssert.AreEqual(Location1.ChargingPeriods,                                  parsedLocation.ChargingPeriods);
            //ClassicAssert.AreEqual(Location1.TotalCosts,                                       parsedLocation.TotalCosts);
            //ClassicAssert.AreEqual(Location1.Status,                                           parsedLocation.Status);
            //ClassicAssert.AreEqual(Location1.LastUpdated.ToISO8601(),                          parsedLocation.LastUpdated.ToISO8601());

        }

        #endregion

    }

}
