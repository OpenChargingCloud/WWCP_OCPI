/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;
using System.Linq;

using NUnit.Framework;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.UnitTests
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
                                "Deutschland",
                                GeoCoordinate.Parse(10, 20),
                                "Europe/Berlin",
                                null,
                                "Location 0001",
                                "07749",
                                "Thüringen",
                                new AdditionalGeoLocation[] {
                                    new AdditionalGeoLocation(
                                        Latitude.Parse(11),
                                        Longitude.Parse(22),
                                        Name: DisplayText.Create(Languages.de, "Postkasten")
                                    )
                                },
                                ParkingTypes.PARKING_LOT,
                                new EVSE[] {
                                    new EVSE(
                                        EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                        StatusTypes.AVAILABLE,
                                        new Connector[] {
                                            new Connector(
                                                Connector_Id.Parse("1"),
                                                ConnectorTypes.IEC_62196_T2,
                                                ConnectorFormats.SOCKET,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                30,
                                                12,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                                    Tariff_Id.Parse("DE*GEF*T0002")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-21")
                                            ),
                                            new Connector(
                                                Connector_Id.Parse("2"),
                                                ConnectorTypes.IEC_62196_T2_COMBO,
                                                ConnectorFormats.CABLE,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                20,
                                                8,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                                    Tariff_Id.Parse("DE*GEF*T0004")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-22")
                                            )
                                        },
                                        EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                        new StatusSchedule[] {
                                            new StatusSchedule(
                                                StatusTypes.INOPERATIVE,
                                                DateTime.Parse("2020-09-23"),
                                                DateTime.Parse("2020-09-24")
                                            ),
                                            new StatusSchedule(
                                                StatusTypes.OUTOFORDER,
                                                DateTime.Parse("2020-12-30"),
                                                DateTime.Parse("2020-12-31")
                                            )
                                        },
                                        new CapabilityTypes[] {
                                            CapabilityTypes.RFID_READER,
                                            CapabilityTypes.RESERVABLE
                                        },
                                        "1. Stock",
                                        GeoCoordinate.Parse(10.1, 20.2),
                                        "Ladestation #1",
                                        new DisplayText[] {
                                            DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                            DisplayText.Create(Languages.en, "Ken sent me!")
                                        },
                                        new ParkingRestrictions[] {
                                            ParkingRestrictions.EV_ONLY,
                                            ParkingRestrictions.PLUGGED
                                        },
                                        new Image[] {
                                            new Image(
                                                URL.Parse("http://example.com/pinguine.jpg"),
                                                ImageFileType.jpeg,
                                                ImageCategories.OPERATOR,
                                                100,
                                                150,
                                                URL.Parse("http://example.com/kleine_pinguine.jpg")
                                            )
                                        },
                                        DateTime.Parse("2020-09-22")
                                    )
                                },
                                new DisplayText[] {
                                    new DisplayText(Languages.de, "Hallo Welt!"),
                                    new DisplayText(Languages.en, "Hello world!")
                                },
                                new BusinessDetails(
                                    "Open Charging Cloud",
                                    URL.Parse("https://open.charging.cloud"),
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/logo.svg"),
                                        ImageFileType.svg,
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OWNER,
                                        3000,
                                        4500,
                                        URL.Parse("http://ahzf.de/logo_small.gif")
                                    )
                                ),
                                new Facilities[] {
                                    Facilities.CAFE
                                },
                                new Hours(
                                    new RegularHours[] {
                                        new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                        new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                        new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                        new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                        new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-09-21T00:00:00Z"),
                                            DateTime.Parse("2020-09-22T00:00:00Z")
                                        )
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-12-24T00:00:00Z"),
                                            DateTime.Parse("2020-12-26T00:00:00Z")
                                        )
                                    }
                                ),
                                false,
                                new Image[] {
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                        ImageFileType.jpeg,
                                        ImageCategories.LOCATION,
                                        200,
                                        400,
                                        URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                    )
                                },
                                new EnergyMix(
                                    true,
                                    new EnergySource[] {
                                        new EnergySource(
                                            EnergySourceCategories.SOLAR,
                                            80
                                        ),
                                        new EnergySource(
                                            EnergySourceCategories.WIND,
                                            20
                                        )
                                    },
                                    new EnvironmentalImpact[] {
                                        new EnvironmentalImpact(
                                            EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                            0.1
                                        )
                                    },
                                    "Stadtwerke Jena-Ost",
                                    "New Green Deal"
                                ),
                                DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var JSON = Location1.ToJSON();

            Assert.AreEqual("DE",                               JSON["country_code"].Value<String>());
            Assert.AreEqual("GEF",                              JSON["party_id"].    Value<String>());
            Assert.AreEqual("LOC0001",                          JSON["id"].          Value<String>());

            Assert.IsTrue(Location.TryParse(JSON, out Location Location2, out String ErrorResponse));
            Assert.IsNull(ErrorResponse);

            Assert.AreEqual(Location1.CountryCode,              Location2.CountryCode);
            Assert.AreEqual(Location1.PartyId,                  Location2.PartyId);
            Assert.AreEqual(Location1.Id,                       Location2.Id);

            Assert.AreEqual(Location1.Publish,                  Location2.Publish);
            Assert.AreEqual(Location1.Address,                  Location2.Address);
            Assert.AreEqual(Location1.City,                     Location2.City);
            Assert.AreEqual(Location1.Country,                  Location2.Country);
            Assert.AreEqual(Location1.Coordinates,              Location2.Coordinates);
            Assert.AreEqual(Location1.Timezone,                 Location2.Timezone);

            Assert.AreEqual(Location1.PublishAllowedTo,         Location2.PublishAllowedTo);
            Assert.AreEqual(Location1.Name,                     Location2.Name);
            Assert.AreEqual(Location1.PostalCode,               Location2.PostalCode);
            Assert.AreEqual(Location1.State,                    Location2.State);
            Assert.AreEqual(Location1.RelatedLocations,         Location2.RelatedLocations);
            Assert.AreEqual(Location1.ParkingType,              Location2.ParkingType);
            //Assert.AreEqual(LocationA.EVSEs,                    LocationB.EVSEs);
            Assert.AreEqual(Location1.Directions,               Location2.Directions);
            Assert.AreEqual(Location1.Operator,                 Location2.Operator);
            Assert.AreEqual(Location1.SubOperator,              Location2.SubOperator);
            Assert.AreEqual(Location1.Owner,                    Location2.Owner);
            Assert.AreEqual(Location1.Facilities,               Location2.Facilities);
            //Assert.AreEqual(LocationA.OpeningTimes,             LocationB.OpeningTimes);
            Assert.AreEqual(Location1.ChargingWhenClosed,       Location2.ChargingWhenClosed);
            Assert.AreEqual(Location1.Images,                   Location2.Images);
            Assert.AreEqual(Location1.EnergyMix,                Location2.EnergyMix);

            Assert.AreEqual(Location1.LastUpdated.ToIso8601(),  Location2.LastUpdated.ToIso8601());

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
                                "Deutschland",
                                GeoCoordinate.Parse(10, 20),
                                "Europe/Berlin",
                                null,
                                "Location 0001",
                                "07749",
                                "Thüringen",
                                new AdditionalGeoLocation[] {
                                    new AdditionalGeoLocation(
                                        Latitude.Parse(11),
                                        Longitude.Parse(22),
                                        Name: DisplayText.Create(Languages.de, "Postkasten")
                                    )
                                },
                                ParkingTypes.PARKING_LOT,
                                new EVSE[] {
                                    new EVSE(
                                        EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                        StatusTypes.AVAILABLE,
                                        new Connector[] {
                                            new Connector(
                                                Connector_Id.Parse("1"),
                                                ConnectorTypes.IEC_62196_T2,
                                                ConnectorFormats.SOCKET,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                30,
                                                12,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                                    Tariff_Id.Parse("DE*GEF*T0002")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-21")
                                            ),
                                            new Connector(
                                                Connector_Id.Parse("2"),
                                                ConnectorTypes.IEC_62196_T2_COMBO,
                                                ConnectorFormats.CABLE,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                20,
                                                8,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                                    Tariff_Id.Parse("DE*GEF*T0004")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-22")
                                            )
                                        },
                                        EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                        new StatusSchedule[] {
                                            new StatusSchedule(
                                                StatusTypes.INOPERATIVE,
                                                DateTime.Parse("2020-09-23"),
                                                DateTime.Parse("2020-09-24")
                                            ),
                                            new StatusSchedule(
                                                StatusTypes.OUTOFORDER,
                                                DateTime.Parse("2020-12-30"),
                                                DateTime.Parse("2020-12-31")
                                            )
                                        },
                                        new CapabilityTypes[] {
                                            CapabilityTypes.RFID_READER,
                                            CapabilityTypes.RESERVABLE
                                        },
                                        "1. Stock",
                                        GeoCoordinate.Parse(10.1, 20.2),
                                        "Ladestation #1",
                                        new DisplayText[] {
                                            DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                            DisplayText.Create(Languages.en, "Ken sent me!")
                                        },
                                        new ParkingRestrictions[] {
                                            ParkingRestrictions.EV_ONLY,
                                            ParkingRestrictions.PLUGGED
                                        },
                                        new Image[] {
                                            new Image(
                                                URL.Parse("http://example.com/pinguine.jpg"),
                                                ImageFileType.jpeg,
                                                ImageCategories.OPERATOR,
                                                100,
                                                150,
                                                URL.Parse("http://example.com/kleine_pinguine.jpg")
                                            )
                                        },
                                        DateTime.Parse("2020-09-22")
                                    )
                                },
                                new DisplayText[] {
                                    new DisplayText(Languages.de, "Hallo Welt!"),
                                    new DisplayText(Languages.en, "Hello world!")
                                },
                                new BusinessDetails(
                                    "Open Charging Cloud",
                                    URL.Parse("https://open.charging.cloud"),
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/logo.svg"),
                                        ImageFileType.svg,
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OWNER,
                                        3000,
                                        4500,
                                        URL.Parse("http://ahzf.de/logo_small.gif")
                                    )
                                ),
                                new Facilities[] {
                                    Facilities.CAFE
                                },
                                new Hours(
                                    new RegularHours[] {
                                        new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                        new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                        new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                        new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                        new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-09-21T00:00:00Z"),
                                            DateTime.Parse("2020-09-22T00:00:00Z")
                                        )
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-12-24T00:00:00Z"),
                                            DateTime.Parse("2020-12-26T00:00:00Z")
                                        )
                                    }
                                ),
                                false,
                                new Image[] {
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                        ImageFileType.jpeg,
                                        ImageCategories.LOCATION,
                                        200,
                                        400,
                                        URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                    )
                                },
                                new EnergyMix(
                                    true,
                                    new EnergySource[] {
                                        new EnergySource(
                                            EnergySourceCategories.SOLAR,
                                            80
                                        ),
                                        new EnergySource(
                                            EnergySourceCategories.WIND,
                                            20
                                        )
                                    },
                                    new EnvironmentalImpact[] {
                                        new EnvironmentalImpact(
                                            EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                            0.1
                                        )
                                    },
                                    "Stadtwerke Jena-Ost",
                                    "New Green Deal"
                                ),
                                DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""country_code"": ""FR"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsFalse  (patchResult.IsSuccess);
            Assert.IsTrue   (patchResult.IsFailed);
            Assert.IsNotNull(patchResult.ErrorResponse);
            Assert.AreEqual ("Patching the 'country code' of a location is not allowed!",  patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Location_Id.Parse("LOC0001"),                                 patchResult.PatchedData.Id);
            Assert.AreEqual ("2020-09-21T00:00:00.000Z",                                   patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                "Deutschland",
                                GeoCoordinate.Parse(10, 20),
                                "Europe/Berlin",
                                null,
                                "Location 0001",
                                "07749",
                                "Thüringen",
                                new AdditionalGeoLocation[] {
                                    new AdditionalGeoLocation(
                                        Latitude.Parse(11),
                                        Longitude.Parse(22),
                                        Name: DisplayText.Create(Languages.de, "Postkasten")
                                    )
                                },
                                ParkingTypes.PARKING_LOT,
                                new EVSE[] {
                                    new EVSE(
                                        EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                        StatusTypes.AVAILABLE,
                                        new Connector[] {
                                            new Connector(
                                                Connector_Id.Parse("1"),
                                                ConnectorTypes.IEC_62196_T2,
                                                ConnectorFormats.SOCKET,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                30,
                                                12,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                                    Tariff_Id.Parse("DE*GEF*T0002")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-21")
                                            ),
                                            new Connector(
                                                Connector_Id.Parse("2"),
                                                ConnectorTypes.IEC_62196_T2_COMBO,
                                                ConnectorFormats.CABLE,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                20,
                                                8,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                                    Tariff_Id.Parse("DE*GEF*T0004")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-22")
                                            )
                                        },
                                        EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                        new StatusSchedule[] {
                                            new StatusSchedule(
                                                StatusTypes.INOPERATIVE,
                                                DateTime.Parse("2020-09-23"),
                                                DateTime.Parse("2020-09-24")
                                            ),
                                            new StatusSchedule(
                                                StatusTypes.OUTOFORDER,
                                                DateTime.Parse("2020-12-30"),
                                                DateTime.Parse("2020-12-31")
                                            )
                                        },
                                        new CapabilityTypes[] {
                                            CapabilityTypes.RFID_READER,
                                            CapabilityTypes.RESERVABLE
                                        },
                                        "1. Stock",
                                        GeoCoordinate.Parse(10.1, 20.2),
                                        "Ladestation #1",
                                        new DisplayText[] {
                                            DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                            DisplayText.Create(Languages.en, "Ken sent me!")
                                        },
                                        new ParkingRestrictions[] {
                                            ParkingRestrictions.EV_ONLY,
                                            ParkingRestrictions.PLUGGED
                                        },
                                        new Image[] {
                                            new Image(
                                                URL.Parse("http://example.com/pinguine.jpg"),
                                                ImageFileType.jpeg,
                                                ImageCategories.OPERATOR,
                                                100,
                                                150,
                                                URL.Parse("http://example.com/kleine_pinguine.jpg")
                                            )
                                        },
                                        DateTime.Parse("2020-09-22")
                                    )
                                },
                                new DisplayText[] {
                                    new DisplayText(Languages.de, "Hallo Welt!"),
                                    new DisplayText(Languages.en, "Hello world!")
                                },
                                new BusinessDetails(
                                    "Open Charging Cloud",
                                    URL.Parse("https://open.charging.cloud"),
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/logo.svg"),
                                        ImageFileType.svg,
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OWNER,
                                        3000,
                                        4500,
                                        URL.Parse("http://ahzf.de/logo_small.gif")
                                    )
                                ),
                                new Facilities[] {
                                    Facilities.CAFE
                                },
                                new Hours(
                                    new RegularHours[] {
                                        new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                        new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                        new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                        new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                        new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-09-21T00:00:00Z"),
                                            DateTime.Parse("2020-09-22T00:00:00Z")
                                        )
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-12-24T00:00:00Z"),
                                            DateTime.Parse("2020-12-26T00:00:00Z")
                                        )
                                    }
                                ),
                                false,
                                new Image[] {
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                        ImageFileType.jpeg,
                                        ImageCategories.LOCATION,
                                        200,
                                        400,
                                        URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                    )
                                },
                                new EnergyMix(
                                    true,
                                    new EnergySource[] {
                                        new EnergySource(
                                            EnergySourceCategories.SOLAR,
                                            80
                                        ),
                                        new EnergySource(
                                            EnergySourceCategories.WIND,
                                            20
                                        )
                                    },
                                    new EnvironmentalImpact[] {
                                        new EnvironmentalImpact(
                                            EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                            0.1
                                        )
                                    },
                                    "Stadtwerke Jena-Ost",
                                    "New Green Deal"
                                ),
                                DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""party_id"": ""GDF"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsFalse  (patchResult.IsSuccess);
            Assert.IsTrue   (patchResult.IsFailed);
            Assert.IsNotNull(patchResult.ErrorResponse);
            Assert.AreEqual ("Patching the 'party identification' of a location is not allowed!",  patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Location_Id.Parse("LOC0001"),                                   patchResult.PatchedData.Id);
            Assert.AreEqual ("2020-09-21T00:00:00.000Z",                                     patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                "Deutschland",
                                GeoCoordinate.Parse(10, 20),
                                "Europe/Berlin",
                                null,
                                "Location 0001",
                                "07749",
                                "Thüringen",
                                new AdditionalGeoLocation[] {
                                    new AdditionalGeoLocation(
                                        Latitude.Parse(11),
                                        Longitude.Parse(22),
                                        Name: DisplayText.Create(Languages.de, "Postkasten")
                                    )
                                },
                                ParkingTypes.PARKING_LOT,
                                new EVSE[] {
                                    new EVSE(
                                        EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                        StatusTypes.AVAILABLE,
                                        new Connector[] {
                                            new Connector(
                                                Connector_Id.Parse("1"),
                                                ConnectorTypes.IEC_62196_T2,
                                                ConnectorFormats.SOCKET,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                30,
                                                12,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                                    Tariff_Id.Parse("DE*GEF*T0002")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-21")
                                            ),
                                            new Connector(
                                                Connector_Id.Parse("2"),
                                                ConnectorTypes.IEC_62196_T2_COMBO,
                                                ConnectorFormats.CABLE,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                20,
                                                8,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                                    Tariff_Id.Parse("DE*GEF*T0004")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-22")
                                            )
                                        },
                                        EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                        new StatusSchedule[] {
                                            new StatusSchedule(
                                                StatusTypes.INOPERATIVE,
                                                DateTime.Parse("2020-09-23"),
                                                DateTime.Parse("2020-09-24")
                                            ),
                                            new StatusSchedule(
                                                StatusTypes.OUTOFORDER,
                                                DateTime.Parse("2020-12-30"),
                                                DateTime.Parse("2020-12-31")
                                            )
                                        },
                                        new CapabilityTypes[] {
                                            CapabilityTypes.RFID_READER,
                                            CapabilityTypes.RESERVABLE
                                        },
                                        "1. Stock",
                                        GeoCoordinate.Parse(10.1, 20.2),
                                        "Ladestation #1",
                                        new DisplayText[] {
                                            DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                            DisplayText.Create(Languages.en, "Ken sent me!")
                                        },
                                        new ParkingRestrictions[] {
                                            ParkingRestrictions.EV_ONLY,
                                            ParkingRestrictions.PLUGGED
                                        },
                                        new Image[] {
                                            new Image(
                                                URL.Parse("http://example.com/pinguine.jpg"),
                                                ImageFileType.jpeg,
                                                ImageCategories.OPERATOR,
                                                100,
                                                150,
                                                URL.Parse("http://example.com/kleine_pinguine.jpg")
                                            )
                                        },
                                        DateTime.Parse("2020-09-22")
                                    )
                                },
                                new DisplayText[] {
                                    new DisplayText(Languages.de, "Hallo Welt!"),
                                    new DisplayText(Languages.en, "Hello world!")
                                },
                                new BusinessDetails(
                                    "Open Charging Cloud",
                                    URL.Parse("https://open.charging.cloud"),
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/logo.svg"),
                                        ImageFileType.svg,
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OWNER,
                                        3000,
                                        4500,
                                        URL.Parse("http://ahzf.de/logo_small.gif")
                                    )
                                ),
                                new Facilities[] {
                                    Facilities.CAFE
                                },
                                new Hours(
                                    new RegularHours[] {
                                        new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                        new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                        new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                        new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                        new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-09-21T00:00:00Z"),
                                            DateTime.Parse("2020-09-22T00:00:00Z")
                                        )
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-12-24T00:00:00Z"),
                                            DateTime.Parse("2020-12-26T00:00:00Z")
                                        )
                                    }
                                ),
                                false,
                                new Image[] {
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                        ImageFileType.jpeg,
                                        ImageCategories.LOCATION,
                                        200,
                                        400,
                                        URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                    )
                                },
                                new EnergyMix(
                                    true,
                                    new EnergySource[] {
                                        new EnergySource(
                                            EnergySourceCategories.SOLAR,
                                            80
                                        ),
                                        new EnergySource(
                                            EnergySourceCategories.WIND,
                                            20
                                        )
                                    },
                                    new EnvironmentalImpact[] {
                                        new EnvironmentalImpact(
                                            EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                            0.1
                                        )
                                    },
                                    "Stadtwerke Jena-Ost",
                                    "New Green Deal"
                                ),
                                DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""id"": ""2"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsFalse  (patchResult.IsSuccess);
            Assert.IsTrue   (patchResult.IsFailed);
            Assert.IsNotNull(patchResult.ErrorResponse);
            Assert.AreEqual ("Patching the 'identification' of a location is not allowed!",  patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Location_Id.Parse("LOC0001"),                                   patchResult.PatchedData.Id);
            Assert.AreEqual ("2020-09-21T00:00:00.000Z",                                     patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                "Deutschland",
                                GeoCoordinate.Parse(10, 20),
                                "Europe/Berlin",
                                null,
                                "Location 0001",
                                "07749",
                                "Thüringen",
                                new AdditionalGeoLocation[] {
                                    new AdditionalGeoLocation(
                                        Latitude.Parse(11),
                                        Longitude.Parse(22),
                                        Name: DisplayText.Create(Languages.de, "Postkasten")
                                    )
                                },
                                ParkingTypes.PARKING_LOT,
                                new EVSE[] {
                                    new EVSE(
                                        EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                        StatusTypes.AVAILABLE,
                                        new Connector[] {
                                            new Connector(
                                                Connector_Id.Parse("1"),
                                                ConnectorTypes.IEC_62196_T2,
                                                ConnectorFormats.SOCKET,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                30,
                                                12,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                                    Tariff_Id.Parse("DE*GEF*T0002")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-21")
                                            ),
                                            new Connector(
                                                Connector_Id.Parse("2"),
                                                ConnectorTypes.IEC_62196_T2_COMBO,
                                                ConnectorFormats.CABLE,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                20,
                                                8,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                                    Tariff_Id.Parse("DE*GEF*T0004")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-22")
                                            )
                                        },
                                        EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                        new StatusSchedule[] {
                                            new StatusSchedule(
                                                StatusTypes.INOPERATIVE,
                                                DateTime.Parse("2020-09-23"),
                                                DateTime.Parse("2020-09-24")
                                            ),
                                            new StatusSchedule(
                                                StatusTypes.OUTOFORDER,
                                                DateTime.Parse("2020-12-30"),
                                                DateTime.Parse("2020-12-31")
                                            )
                                        },
                                        new CapabilityTypes[] {
                                            CapabilityTypes.RFID_READER,
                                            CapabilityTypes.RESERVABLE
                                        },
                                        "1. Stock",
                                        GeoCoordinate.Parse(10.1, 20.2),
                                        "Ladestation #1",
                                        new DisplayText[] {
                                            DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                            DisplayText.Create(Languages.en, "Ken sent me!")
                                        },
                                        new ParkingRestrictions[] {
                                            ParkingRestrictions.EV_ONLY,
                                            ParkingRestrictions.PLUGGED
                                        },
                                        new Image[] {
                                            new Image(
                                                URL.Parse("http://example.com/pinguine.jpg"),
                                                ImageFileType.jpeg,
                                                ImageCategories.OPERATOR,
                                                100,
                                                150,
                                                URL.Parse("http://example.com/kleine_pinguine.jpg")
                                            )
                                        },
                                        DateTime.Parse("2020-09-22")
                                    )
                                },
                                new DisplayText[] {
                                    new DisplayText(Languages.de, "Hallo Welt!"),
                                    new DisplayText(Languages.en, "Hello world!")
                                },
                                new BusinessDetails(
                                    "Open Charging Cloud",
                                    URL.Parse("https://open.charging.cloud"),
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/logo.svg"),
                                        ImageFileType.svg,
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OWNER,
                                        3000,
                                        4500,
                                        URL.Parse("http://ahzf.de/logo_small.gif")
                                    )
                                ),
                                new Facilities[] {
                                    Facilities.CAFE
                                },
                                new Hours(
                                    new RegularHours[] {
                                        new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                        new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                        new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                        new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                        new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-09-21T00:00:00Z"),
                                            DateTime.Parse("2020-09-22T00:00:00Z")
                                        )
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-12-24T00:00:00Z"),
                                            DateTime.Parse("2020-12-26T00:00:00Z")
                                        )
                                    }
                                ),
                                false,
                                new Image[] {
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                        ImageFileType.jpeg,
                                        ImageCategories.LOCATION,
                                        200,
                                        400,
                                        URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                    )
                                },
                                new EnergyMix(
                                    true,
                                    new EnergySource[] {
                                        new EnergySource(
                                            EnergySourceCategories.SOLAR,
                                            80
                                        ),
                                        new EnergySource(
                                            EnergySourceCategories.WIND,
                                            20
                                        )
                                    },
                                    new EnvironmentalImpact[] {
                                        new EnvironmentalImpact(
                                            EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                            0.1
                                        )
                                    },
                                    "Stadtwerke Jena-Ost",
                                    "New Green Deal"
                                ),
                                DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""name"": ""Location 0001a"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsTrue   (patchResult.IsSuccess);
            Assert.IsFalse  (patchResult.IsFailed);
            Assert.IsNull   (patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual   (Location_Id.Parse("LOC0001"),            patchResult.PatchedData.Id);
            Assert.AreEqual   ("Location 0001a",                        patchResult.PatchedData.Name);
            Assert.AreNotEqual(DateTime.Parse("2020-09-21T00:00:00Z"),  patchResult.PatchedData.LastUpdated);

            Assert.IsTrue     (DateTime.UtcNow - patchResult.PatchedData.LastUpdated < TimeSpan.FromSeconds(5));

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
                                "Deutschland",
                                GeoCoordinate.Parse(10, 20),
                                "Europe/Berlin",
                                null,
                                "Location 0001",
                                "07749",
                                "Thüringen",
                                new AdditionalGeoLocation[] {
                                    new AdditionalGeoLocation(
                                        Latitude.Parse(11),
                                        Longitude.Parse(22),
                                        Name: DisplayText.Create(Languages.de, "Postkasten")
                                    )
                                },
                                ParkingTypes.PARKING_LOT,
                                new EVSE[] {
                                    new EVSE(
                                        EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                        StatusTypes.AVAILABLE,
                                        new Connector[] {
                                            new Connector(
                                                Connector_Id.Parse("1"),
                                                ConnectorTypes.IEC_62196_T2,
                                                ConnectorFormats.SOCKET,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                30,
                                                12,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                                    Tariff_Id.Parse("DE*GEF*T0002")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-21")
                                            ),
                                            new Connector(
                                                Connector_Id.Parse("2"),
                                                ConnectorTypes.IEC_62196_T2_COMBO,
                                                ConnectorFormats.CABLE,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                20,
                                                8,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                                    Tariff_Id.Parse("DE*GEF*T0004")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-22")
                                            )
                                        },
                                        EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                        new StatusSchedule[] {
                                            new StatusSchedule(
                                                StatusTypes.INOPERATIVE,
                                                DateTime.Parse("2020-09-23"),
                                                DateTime.Parse("2020-09-24")
                                            ),
                                            new StatusSchedule(
                                                StatusTypes.OUTOFORDER,
                                                DateTime.Parse("2020-12-30"),
                                                DateTime.Parse("2020-12-31")
                                            )
                                        },
                                        new CapabilityTypes[] {
                                            CapabilityTypes.RFID_READER,
                                            CapabilityTypes.RESERVABLE
                                        },
                                        "1. Stock",
                                        GeoCoordinate.Parse(10.1, 20.2),
                                        "Ladestation #1",
                                        new DisplayText[] {
                                            DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                            DisplayText.Create(Languages.en, "Ken sent me!")
                                        },
                                        new ParkingRestrictions[] {
                                            ParkingRestrictions.EV_ONLY,
                                            ParkingRestrictions.PLUGGED
                                        },
                                        new Image[] {
                                            new Image(
                                                URL.Parse("http://example.com/pinguine.jpg"),
                                                ImageFileType.jpeg,
                                                ImageCategories.OPERATOR,
                                                100,
                                                150,
                                                URL.Parse("http://example.com/kleine_pinguine.jpg")
                                            )
                                        },
                                        DateTime.Parse("2020-09-22")
                                    )
                                },
                                new DisplayText[] {
                                    new DisplayText(Languages.de, "Hallo Welt!"),
                                    new DisplayText(Languages.en, "Hello world!")
                                },
                                new BusinessDetails(
                                    "Open Charging Cloud",
                                    URL.Parse("https://open.charging.cloud"),
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/logo.svg"),
                                        ImageFileType.svg,
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OWNER,
                                        3000,
                                        4500,
                                        URL.Parse("http://ahzf.de/logo_small.gif")
                                    )
                                ),
                                new Facilities[] {
                                    Facilities.CAFE
                                },
                                new Hours(
                                    new RegularHours[] {
                                        new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                        new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                        new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                        new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                        new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-09-21T00:00:00Z"),
                                            DateTime.Parse("2020-09-22T00:00:00Z")
                                        )
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-12-24T00:00:00Z"),
                                            DateTime.Parse("2020-12-26T00:00:00Z")
                                        )
                                    }
                                ),
                                false,
                                new Image[] {
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                        ImageFileType.jpeg,
                                        ImageCategories.LOCATION,
                                        200,
                                        400,
                                        URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                    )
                                },
                                new EnergyMix(
                                    true,
                                    new EnergySource[] {
                                        new EnergySource(
                                            EnergySourceCategories.SOLAR,
                                            80
                                        ),
                                        new EnergySource(
                                            EnergySourceCategories.WIND,
                                            20
                                        )
                                    },
                                    new EnvironmentalImpact[] {
                                        new EnvironmentalImpact(
                                            EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                            0.1
                                        )
                                    },
                                    "Stadtwerke Jena-Ost",
                                    "New Green Deal"
                                ),
                                DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""name"": ""Location 0001a"", ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsTrue   (patchResult.IsSuccess);
            Assert.IsFalse  (patchResult.IsFailed);
            Assert.IsNull   (patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Location_Id.Parse("LOC0001"),  patchResult.PatchedData.Id);
            Assert.AreEqual ("Location 0001a",              patchResult.PatchedData.Name);
            Assert.AreEqual ("2020-10-15T00:00:00.000Z",    patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                "Deutschland",
                                GeoCoordinate.Parse(10, 20),
                                "Europe/Berlin",
                                null,
                                "Location 0001",
                                "07749",
                                "Thüringen",
                                new AdditionalGeoLocation[] {
                                    new AdditionalGeoLocation(
                                        Latitude.Parse(11),
                                        Longitude.Parse(22),
                                        Name: DisplayText.Create(Languages.de, "Postkasten")
                                    )
                                },
                                ParkingTypes.PARKING_LOT,
                                new EVSE[] {
                                    new EVSE(
                                        EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                        StatusTypes.AVAILABLE,
                                        new Connector[] {
                                            new Connector(
                                                Connector_Id.Parse("1"),
                                                ConnectorTypes.IEC_62196_T2,
                                                ConnectorFormats.SOCKET,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                30,
                                                12,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                                    Tariff_Id.Parse("DE*GEF*T0002")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-21")
                                            ),
                                            new Connector(
                                                Connector_Id.Parse("2"),
                                                ConnectorTypes.IEC_62196_T2_COMBO,
                                                ConnectorFormats.CABLE,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                20,
                                                8,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                                    Tariff_Id.Parse("DE*GEF*T0004")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-22")
                                            )
                                        },
                                        EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                        new StatusSchedule[] {
                                            new StatusSchedule(
                                                StatusTypes.INOPERATIVE,
                                                DateTime.Parse("2020-09-23"),
                                                DateTime.Parse("2020-09-24")
                                            ),
                                            new StatusSchedule(
                                                StatusTypes.OUTOFORDER,
                                                DateTime.Parse("2020-12-30"),
                                                DateTime.Parse("2020-12-31")
                                            )
                                        },
                                        new CapabilityTypes[] {
                                            CapabilityTypes.RFID_READER,
                                            CapabilityTypes.RESERVABLE
                                        },
                                        "1. Stock",
                                        GeoCoordinate.Parse(10.1, 20.2),
                                        "Ladestation #1",
                                        new DisplayText[] {
                                            DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                            DisplayText.Create(Languages.en, "Ken sent me!")
                                        },
                                        new ParkingRestrictions[] {
                                            ParkingRestrictions.EV_ONLY,
                                            ParkingRestrictions.PLUGGED
                                        },
                                        new Image[] {
                                            new Image(
                                                URL.Parse("http://example.com/pinguine.jpg"),
                                                ImageFileType.jpeg,
                                                ImageCategories.OPERATOR,
                                                100,
                                                150,
                                                URL.Parse("http://example.com/kleine_pinguine.jpg")
                                            )
                                        },
                                        DateTime.Parse("2020-09-22")
                                    )
                                },
                                new DisplayText[] {
                                    new DisplayText(Languages.de, "Hallo Welt!"),
                                    new DisplayText(Languages.en, "Hello world!")
                                },
                                new BusinessDetails(
                                    "Open Charging Cloud",
                                    URL.Parse("https://open.charging.cloud"),
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/logo.svg"),
                                        ImageFileType.svg,
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OWNER,
                                        3000,
                                        4500,
                                        URL.Parse("http://ahzf.de/logo_small.gif")
                                    )
                                ),
                                new Facilities[] {
                                    Facilities.CAFE
                                },
                                new Hours(
                                    new RegularHours[] {
                                        new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                        new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                        new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                        new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                        new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-09-21T00:00:00Z"),
                                            DateTime.Parse("2020-09-22T00:00:00Z")
                                        )
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-12-24T00:00:00Z"),
                                            DateTime.Parse("2020-12-26T00:00:00Z")
                                        )
                                    }
                                ),
                                false,
                                new Image[] {
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                        ImageFileType.jpeg,
                                        ImageCategories.LOCATION,
                                        200,
                                        400,
                                        URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                    )
                                },
                                new EnergyMix(
                                    true,
                                    new EnergySource[] {
                                        new EnergySource(
                                            EnergySourceCategories.SOLAR,
                                            80
                                        ),
                                        new EnergySource(
                                            EnergySourceCategories.WIND,
                                            20
                                        )
                                    },
                                    new EnvironmentalImpact[] {
                                        new EnvironmentalImpact(
                                            EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                            0.1
                                        )
                                    },
                                    "Stadtwerke Jena-Ost",
                                    "New Green Deal"
                                ),
                                DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""facilities"": [ ""CAFE"", ""AIRPORT"" ], ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsTrue   (patchResult.IsSuccess);
            Assert.IsFalse  (patchResult.IsFailed);
            Assert.IsNull   (patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Location_Id.Parse("LOC0001"),  patchResult.PatchedData.Id);
            Assert.AreEqual ("Location 0001",               patchResult.PatchedData.Name);
            Assert.AreEqual (2,                             patchResult.PatchedData.Facilities.        Count());
            Assert.AreEqual (Facilities.Parse("CAFE"),      patchResult.PatchedData.Facilities.        First());
            Assert.AreEqual (Facilities.Parse("AIRPORT"),   patchResult.PatchedData.Facilities.Skip(1).First());
            Assert.AreEqual ("2020-10-15T00:00:00.000Z",    patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                "Deutschland",
                                GeoCoordinate.Parse(10, 20),
                                "Europe/Berlin",
                                null,
                                "Location 0001",
                                "07749",
                                "Thüringen",
                                new AdditionalGeoLocation[] {
                                    new AdditionalGeoLocation(
                                        Latitude.Parse(11),
                                        Longitude.Parse(22),
                                        Name: DisplayText.Create(Languages.de, "Postkasten")
                                    )
                                },
                                ParkingTypes.PARKING_LOT,
                                new EVSE[] {
                                    new EVSE(
                                        EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                        StatusTypes.AVAILABLE,
                                        new Connector[] {
                                            new Connector(
                                                Connector_Id.Parse("1"),
                                                ConnectorTypes.IEC_62196_T2,
                                                ConnectorFormats.SOCKET,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                30,
                                                12,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                                    Tariff_Id.Parse("DE*GEF*T0002")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-21")
                                            ),
                                            new Connector(
                                                Connector_Id.Parse("2"),
                                                ConnectorTypes.IEC_62196_T2_COMBO,
                                                ConnectorFormats.CABLE,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                20,
                                                8,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                                    Tariff_Id.Parse("DE*GEF*T0004")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-22")
                                            )
                                        },
                                        EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                        new StatusSchedule[] {
                                            new StatusSchedule(
                                                StatusTypes.INOPERATIVE,
                                                DateTime.Parse("2020-09-23"),
                                                DateTime.Parse("2020-09-24")
                                            ),
                                            new StatusSchedule(
                                                StatusTypes.OUTOFORDER,
                                                DateTime.Parse("2020-12-30"),
                                                DateTime.Parse("2020-12-31")
                                            )
                                        },
                                        new CapabilityTypes[] {
                                            CapabilityTypes.RFID_READER,
                                            CapabilityTypes.RESERVABLE
                                        },
                                        "1. Stock",
                                        GeoCoordinate.Parse(10.1, 20.2),
                                        "Ladestation #1",
                                        new DisplayText[] {
                                            DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                            DisplayText.Create(Languages.en, "Ken sent me!")
                                        },
                                        new ParkingRestrictions[] {
                                            ParkingRestrictions.EV_ONLY,
                                            ParkingRestrictions.PLUGGED
                                        },
                                        new Image[] {
                                            new Image(
                                                URL.Parse("http://example.com/pinguine.jpg"),
                                                ImageFileType.jpeg,
                                                ImageCategories.OPERATOR,
                                                100,
                                                150,
                                                URL.Parse("http://example.com/kleine_pinguine.jpg")
                                            )
                                        },
                                        DateTime.Parse("2020-09-22")
                                    )
                                },
                                new DisplayText[] {
                                    new DisplayText(Languages.de, "Hallo Welt!"),
                                    new DisplayText(Languages.en, "Hello world!")
                                },
                                new BusinessDetails(
                                    "Open Charging Cloud",
                                    URL.Parse("https://open.charging.cloud"),
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/logo.svg"),
                                        ImageFileType.svg,
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OWNER,
                                        3000,
                                        4500,
                                        URL.Parse("http://ahzf.de/logo_small.gif")
                                    )
                                ),
                                new Facilities[] {
                                    Facilities.CAFE
                                },
                                new Hours(
                                    new RegularHours[] {
                                        new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                        new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                        new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                        new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                        new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-09-21T00:00:00Z"),
                                            DateTime.Parse("2020-09-22T00:00:00Z")
                                        )
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-12-24T00:00:00Z"),
                                            DateTime.Parse("2020-12-26T00:00:00Z")
                                        )
                                    }
                                ),
                                false,
                                new Image[] {
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                        ImageFileType.jpeg,
                                        ImageCategories.LOCATION,
                                        200,
                                        400,
                                        URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                    )
                                },
                                new EnergyMix(
                                    true,
                                    new EnergySource[] {
                                        new EnergySource(
                                            EnergySourceCategories.SOLAR,
                                            80
                                        ),
                                        new EnergySource(
                                            EnergySourceCategories.WIND,
                                            20
                                        )
                                    },
                                    new EnvironmentalImpact[] {
                                        new EnvironmentalImpact(
                                            EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                            0.1
                                        )
                                    },
                                    "Stadtwerke Jena-Ost",
                                    "New Green Deal"
                                ),
                                DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""facilities"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsTrue   (patchResult.IsSuccess);
            Assert.IsFalse  (patchResult.IsFailed);
            Assert.IsNull   (patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Location_Id.Parse("LOC0001"),  patchResult.PatchedData.Id);
            Assert.AreEqual ("Location 0001",               patchResult.PatchedData.Name);
            Assert.AreEqual (0,                             patchResult.PatchedData.Facilities. Count());
            Assert.AreEqual ("2020-10-15T00:00:00.000Z",    patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                "Deutschland",
                                GeoCoordinate.Parse(10, 20),
                                "Europe/Berlin",
                                null,
                                "Location 0001",
                                "07749",
                                "Thüringen",
                                new AdditionalGeoLocation[] {
                                    new AdditionalGeoLocation(
                                        Latitude.Parse(11),
                                        Longitude.Parse(22),
                                        Name: DisplayText.Create(Languages.de, "Postkasten")
                                    )
                                },
                                ParkingTypes.PARKING_LOT,
                                new EVSE[] {
                                    new EVSE(
                                        EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                        StatusTypes.AVAILABLE,
                                        new Connector[] {
                                            new Connector(
                                                Connector_Id.Parse("1"),
                                                ConnectorTypes.IEC_62196_T2,
                                                ConnectorFormats.SOCKET,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                30,
                                                12,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                                    Tariff_Id.Parse("DE*GEF*T0002")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-21")
                                            ),
                                            new Connector(
                                                Connector_Id.Parse("2"),
                                                ConnectorTypes.IEC_62196_T2_COMBO,
                                                ConnectorFormats.CABLE,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                20,
                                                8,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                                    Tariff_Id.Parse("DE*GEF*T0004")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-22")
                                            )
                                        },
                                        EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                        new StatusSchedule[] {
                                            new StatusSchedule(
                                                StatusTypes.INOPERATIVE,
                                                DateTime.Parse("2020-09-23"),
                                                DateTime.Parse("2020-09-24")
                                            ),
                                            new StatusSchedule(
                                                StatusTypes.OUTOFORDER,
                                                DateTime.Parse("2020-12-30"),
                                                DateTime.Parse("2020-12-31")
                                            )
                                        },
                                        new CapabilityTypes[] {
                                            CapabilityTypes.RFID_READER,
                                            CapabilityTypes.RESERVABLE
                                        },
                                        "1. Stock",
                                        GeoCoordinate.Parse(10.1, 20.2),
                                        "Ladestation #1",
                                        new DisplayText[] {
                                            DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                            DisplayText.Create(Languages.en, "Ken sent me!")
                                        },
                                        new ParkingRestrictions[] {
                                            ParkingRestrictions.EV_ONLY,
                                            ParkingRestrictions.PLUGGED
                                        },
                                        new Image[] {
                                            new Image(
                                                URL.Parse("http://example.com/pinguine.jpg"),
                                                ImageFileType.jpeg,
                                                ImageCategories.OPERATOR,
                                                100,
                                                150,
                                                URL.Parse("http://example.com/kleine_pinguine.jpg")
                                            )
                                        },
                                        DateTime.Parse("2020-09-22")
                                    )
                                },
                                new DisplayText[] {
                                    new DisplayText(Languages.de, "Hallo Welt!"),
                                    new DisplayText(Languages.en, "Hello world!")
                                },
                                new BusinessDetails(
                                    "Open Charging Cloud",
                                    URL.Parse("https://open.charging.cloud"),
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/logo.svg"),
                                        ImageFileType.svg,
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OWNER,
                                        3000,
                                        4500,
                                        URL.Parse("http://ahzf.de/logo_small.gif")
                                    )
                                ),
                                new Facilities[] {
                                    Facilities.CAFE
                                },
                                new Hours(
                                    new RegularHours[] {
                                        new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                        new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                        new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                        new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                        new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-09-21T00:00:00Z"),
                                            DateTime.Parse("2020-09-22T00:00:00Z")
                                        )
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-12-24T00:00:00Z"),
                                            DateTime.Parse("2020-12-26T00:00:00Z")
                                        )
                                    }
                                ),
                                false,
                                new Image[] {
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                        ImageFileType.jpeg,
                                        ImageCategories.LOCATION,
                                        200,
                                        400,
                                        URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                    )
                                },
                                new EnergyMix(
                                    true,
                                    new EnergySource[] {
                                        new EnergySource(
                                            EnergySourceCategories.SOLAR,
                                            80
                                        ),
                                        new EnergySource(
                                            EnergySourceCategories.WIND,
                                            20
                                        )
                                    },
                                    new EnvironmentalImpact[] {
                                        new EnvironmentalImpact(
                                            EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                            0.1
                                        )
                                    },
                                    "Stadtwerke Jena-Ost",
                                    "New Green Deal"
                                ),
                                DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""name"": null, ""last_updated"": ""2020-10-15T00:00:00Z"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsTrue   (patchResult.IsSuccess);
            Assert.IsFalse  (patchResult.IsFailed);
            Assert.IsNull   (patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Location_Id.Parse("LOC0001"),  patchResult.PatchedData.Id);
            Assert.IsTrue   (patchResult.PatchedData.Name.IsNullOrEmpty());
            Assert.AreEqual ("2020-10-15T00:00:00.000Z",    patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                "Deutschland",
                                GeoCoordinate.Parse(10, 20),
                                "Europe/Berlin",
                                null,
                                "Location 0001",
                                "07749",
                                "Thüringen",
                                new AdditionalGeoLocation[] {
                                    new AdditionalGeoLocation(
                                        Latitude.Parse(11),
                                        Longitude.Parse(22),
                                        Name: DisplayText.Create(Languages.de, "Postkasten")
                                    )
                                },
                                ParkingTypes.PARKING_LOT,
                                new EVSE[] {
                                    new EVSE(
                                        EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                        StatusTypes.AVAILABLE,
                                        new Connector[] {
                                            new Connector(
                                                Connector_Id.Parse("1"),
                                                ConnectorTypes.IEC_62196_T2,
                                                ConnectorFormats.SOCKET,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                30,
                                                12,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                                    Tariff_Id.Parse("DE*GEF*T0002")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-21")
                                            ),
                                            new Connector(
                                                Connector_Id.Parse("2"),
                                                ConnectorTypes.IEC_62196_T2_COMBO,
                                                ConnectorFormats.CABLE,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                20,
                                                8,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                                    Tariff_Id.Parse("DE*GEF*T0004")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-22")
                                            )
                                        },
                                        EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                        new StatusSchedule[] {
                                            new StatusSchedule(
                                                StatusTypes.INOPERATIVE,
                                                DateTime.Parse("2020-09-23"),
                                                DateTime.Parse("2020-09-24")
                                            ),
                                            new StatusSchedule(
                                                StatusTypes.OUTOFORDER,
                                                DateTime.Parse("2020-12-30"),
                                                DateTime.Parse("2020-12-31")
                                            )
                                        },
                                        new CapabilityTypes[] {
                                            CapabilityTypes.RFID_READER,
                                            CapabilityTypes.RESERVABLE
                                        },
                                        "1. Stock",
                                        GeoCoordinate.Parse(10.1, 20.2),
                                        "Ladestation #1",
                                        new DisplayText[] {
                                            DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                            DisplayText.Create(Languages.en, "Ken sent me!")
                                        },
                                        new ParkingRestrictions[] {
                                            ParkingRestrictions.EV_ONLY,
                                            ParkingRestrictions.PLUGGED
                                        },
                                        new Image[] {
                                            new Image(
                                                URL.Parse("http://example.com/pinguine.jpg"),
                                                ImageFileType.jpeg,
                                                ImageCategories.OPERATOR,
                                                100,
                                                150,
                                                URL.Parse("http://example.com/kleine_pinguine.jpg")
                                            )
                                        },
                                        DateTime.Parse("2020-09-22")
                                    )
                                },
                                new DisplayText[] {
                                    new DisplayText(Languages.de, "Hallo Welt!"),
                                    new DisplayText(Languages.en, "Hello world!")
                                },
                                new BusinessDetails(
                                    "Open Charging Cloud",
                                    URL.Parse("https://open.charging.cloud"),
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/logo.svg"),
                                        ImageFileType.svg,
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OWNER,
                                        3000,
                                        4500,
                                        URL.Parse("http://ahzf.de/logo_small.gif")
                                    )
                                ),
                                new Facilities[] {
                                    Facilities.CAFE
                                },
                                new Hours(
                                    new RegularHours[] {
                                        new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                        new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                        new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                        new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                        new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-09-21T00:00:00Z"),
                                            DateTime.Parse("2020-09-22T00:00:00Z")
                                        )
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-12-24T00:00:00Z"),
                                            DateTime.Parse("2020-12-26T00:00:00Z")
                                        )
                                    }
                                ),
                                false,
                                new Image[] {
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                        ImageFileType.jpeg,
                                        ImageCategories.LOCATION,
                                        200,
                                        400,
                                        URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                    )
                                },
                                new EnergyMix(
                                    true,
                                    new EnergySource[] {
                                        new EnergySource(
                                            EnergySourceCategories.SOLAR,
                                            80
                                        ),
                                        new EnergySource(
                                            EnergySourceCategories.WIND,
                                            20
                                        )
                                    },
                                    new EnvironmentalImpact[] {
                                        new EnvironmentalImpact(
                                            EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                            0.1
                                        )
                                    },
                                    "Stadtwerke Jena-Ost",
                                    "New Green Deal"
                                ),
                                DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""evses"": ""I-N-V-A-L-I-D!"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsFalse  (patchResult.IsSuccess);
            Assert.IsTrue   (patchResult.IsFailed);
            Assert.IsNotNull(patchResult.ErrorResponse);
            Assert.AreEqual ("Invalid JSON merge patch for 'evses' array of a location: JSON property 'evses' is not an array!",  patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Location_Id.Parse("LOC0001"),                                                                        patchResult.PatchedData.Id);
            Assert.AreEqual ("2020-09-21T00:00:00.000Z",                                                                          patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                "Deutschland",
                                GeoCoordinate.Parse(10, 20),
                                "Europe/Berlin",
                                null,
                                "Location 0001",
                                "07749",
                                "Thüringen",
                                new AdditionalGeoLocation[] {
                                    new AdditionalGeoLocation(
                                        Latitude.Parse(11),
                                        Longitude.Parse(22),
                                        Name: DisplayText.Create(Languages.de, "Postkasten")
                                    )
                                },
                                ParkingTypes.PARKING_LOT,
                                new EVSE[] {
                                    new EVSE(
                                        EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                        StatusTypes.AVAILABLE,
                                        new Connector[] {
                                            new Connector(
                                                Connector_Id.Parse("1"),
                                                ConnectorTypes.IEC_62196_T2,
                                                ConnectorFormats.SOCKET,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                30,
                                                12,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                                    Tariff_Id.Parse("DE*GEF*T0002")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-21")
                                            ),
                                            new Connector(
                                                Connector_Id.Parse("2"),
                                                ConnectorTypes.IEC_62196_T2_COMBO,
                                                ConnectorFormats.CABLE,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                20,
                                                8,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                                    Tariff_Id.Parse("DE*GEF*T0004")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-22")
                                            )
                                        },
                                        EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                        new StatusSchedule[] {
                                            new StatusSchedule(
                                                StatusTypes.INOPERATIVE,
                                                DateTime.Parse("2020-09-23"),
                                                DateTime.Parse("2020-09-24")
                                            ),
                                            new StatusSchedule(
                                                StatusTypes.OUTOFORDER,
                                                DateTime.Parse("2020-12-30"),
                                                DateTime.Parse("2020-12-31")
                                            )
                                        },
                                        new CapabilityTypes[] {
                                            CapabilityTypes.RFID_READER,
                                            CapabilityTypes.RESERVABLE
                                        },
                                        "1. Stock",
                                        GeoCoordinate.Parse(10.1, 20.2),
                                        "Ladestation #1",
                                        new DisplayText[] {
                                            DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                            DisplayText.Create(Languages.en, "Ken sent me!")
                                        },
                                        new ParkingRestrictions[] {
                                            ParkingRestrictions.EV_ONLY,
                                            ParkingRestrictions.PLUGGED
                                        },
                                        new Image[] {
                                            new Image(
                                                URL.Parse("http://example.com/pinguine.jpg"),
                                                ImageFileType.jpeg,
                                                ImageCategories.OPERATOR,
                                                100,
                                                150,
                                                URL.Parse("http://example.com/kleine_pinguine.jpg")
                                            )
                                        },
                                        DateTime.Parse("2020-09-22")
                                    )
                                },
                                new DisplayText[] {
                                    new DisplayText(Languages.de, "Hallo Welt!"),
                                    new DisplayText(Languages.en, "Hello world!")
                                },
                                new BusinessDetails(
                                    "Open Charging Cloud",
                                    URL.Parse("https://open.charging.cloud"),
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/logo.svg"),
                                        ImageFileType.svg,
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OWNER,
                                        3000,
                                        4500,
                                        URL.Parse("http://ahzf.de/logo_small.gif")
                                    )
                                ),
                                new Facilities[] {
                                    Facilities.CAFE
                                },
                                new Hours(
                                    new RegularHours[] {
                                        new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                        new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                        new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                        new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                        new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-09-21T00:00:00Z"),
                                            DateTime.Parse("2020-09-22T00:00:00Z")
                                        )
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-12-24T00:00:00Z"),
                                            DateTime.Parse("2020-12-26T00:00:00Z")
                                        )
                                    }
                                ),
                                false,
                                new Image[] {
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                        ImageFileType.jpeg,
                                        ImageCategories.LOCATION,
                                        200,
                                        400,
                                        URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                    )
                                },
                                new EnergyMix(
                                    true,
                                    new EnergySource[] {
                                        new EnergySource(
                                            EnergySourceCategories.SOLAR,
                                            80
                                        ),
                                        new EnergySource(
                                            EnergySourceCategories.WIND,
                                            20
                                        )
                                    },
                                    new EnvironmentalImpact[] {
                                        new EnvironmentalImpact(
                                            EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                            0.1
                                        )
                                    },
                                    "Stadtwerke Jena-Ost",
                                    "New Green Deal"
                                ),
                                DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""operator"": ""I-N-V-A-L-I-D!"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsFalse  (patchResult.IsSuccess);
            Assert.IsTrue   (patchResult.IsFailed);
            Assert.IsNotNull(patchResult.ErrorResponse);
            Assert.AreEqual ("Invalid JSON merge patch of a location: Invalid operator!",  patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Location_Id.Parse("LOC0001"),                                 patchResult.PatchedData.Id);
            Assert.AreEqual ("2020-09-21T00:00:00.000Z",                                   patchResult.PatchedData.LastUpdated.ToIso8601());

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
                                "Deutschland",
                                GeoCoordinate.Parse(10, 20),
                                "Europe/Berlin",
                                null,
                                "Location 0001",
                                "07749",
                                "Thüringen",
                                new AdditionalGeoLocation[] {
                                    new AdditionalGeoLocation(
                                        Latitude.Parse(11),
                                        Longitude.Parse(22),
                                        Name: DisplayText.Create(Languages.de, "Postkasten")
                                    )
                                },
                                ParkingTypes.PARKING_LOT,
                                new EVSE[] {
                                    new EVSE(
                                        EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                        StatusTypes.AVAILABLE,
                                        new Connector[] {
                                            new Connector(
                                                Connector_Id.Parse("1"),
                                                ConnectorTypes.IEC_62196_T2,
                                                ConnectorFormats.SOCKET,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                30,
                                                12,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                                    Tariff_Id.Parse("DE*GEF*T0002")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-21")
                                            ),
                                            new Connector(
                                                Connector_Id.Parse("2"),
                                                ConnectorTypes.IEC_62196_T2_COMBO,
                                                ConnectorFormats.CABLE,
                                                PowerTypes.AC_3_PHASE,
                                                400,
                                                20,
                                                8,
                                                new Tariff_Id[] {
                                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                                    Tariff_Id.Parse("DE*GEF*T0004")
                                                },
                                                URL.Parse("https://open.charging.cloud/terms"),
                                                DateTime.Parse("2020-09-22")
                                            )
                                        },
                                        EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                        new StatusSchedule[] {
                                            new StatusSchedule(
                                                StatusTypes.INOPERATIVE,
                                                DateTime.Parse("2020-09-23"),
                                                DateTime.Parse("2020-09-24")
                                            ),
                                            new StatusSchedule(
                                                StatusTypes.OUTOFORDER,
                                                DateTime.Parse("2020-12-30"),
                                                DateTime.Parse("2020-12-31")
                                            )
                                        },
                                        new CapabilityTypes[] {
                                            CapabilityTypes.RFID_READER,
                                            CapabilityTypes.RESERVABLE
                                        },
                                        "1. Stock",
                                        GeoCoordinate.Parse(10.1, 20.2),
                                        "Ladestation #1",
                                        new DisplayText[] {
                                            DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                            DisplayText.Create(Languages.en, "Ken sent me!")
                                        },
                                        new ParkingRestrictions[] {
                                            ParkingRestrictions.EV_ONLY,
                                            ParkingRestrictions.PLUGGED
                                        },
                                        new Image[] {
                                            new Image(
                                                URL.Parse("http://example.com/pinguine.jpg"),
                                                ImageFileType.jpeg,
                                                ImageCategories.OPERATOR,
                                                100,
                                                150,
                                                URL.Parse("http://example.com/kleine_pinguine.jpg")
                                            )
                                        },
                                        DateTime.Parse("2020-09-22")
                                    )
                                },
                                new DisplayText[] {
                                    new DisplayText(Languages.de, "Hallo Welt!"),
                                    new DisplayText(Languages.en, "Hello world!")
                                },
                                new BusinessDetails(
                                    "Open Charging Cloud",
                                    URL.Parse("https://open.charging.cloud"),
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/logo.svg"),
                                        ImageFileType.svg,
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OPERATOR,
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
                                        ImageCategories.OWNER,
                                        3000,
                                        4500,
                                        URL.Parse("http://ahzf.de/logo_small.gif")
                                    )
                                ),
                                new Facilities[] {
                                    Facilities.CAFE
                                },
                                new Hours(
                                    new RegularHours[] {
                                        new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                        new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                        new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                        new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                        new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-09-21T00:00:00Z"),
                                            DateTime.Parse("2020-09-22T00:00:00Z")
                                        )
                                    },
                                    new ExceptionalPeriod[] {
                                        new ExceptionalPeriod(
                                            DateTime.Parse("2020-12-24T00:00:00Z"),
                                            DateTime.Parse("2020-12-26T00:00:00Z")
                                        )
                                    }
                                ),
                                false,
                                new Image[] {
                                    new Image(
                                        URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                        ImageFileType.jpeg,
                                        ImageCategories.LOCATION,
                                        200,
                                        400,
                                        URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                    )
                                },
                                new EnergyMix(
                                    true,
                                    new EnergySource[] {
                                        new EnergySource(
                                            EnergySourceCategories.SOLAR,
                                            80
                                        ),
                                        new EnergySource(
                                            EnergySourceCategories.WIND,
                                            20
                                        )
                                    },
                                    new EnvironmentalImpact[] {
                                        new EnvironmentalImpact(
                                            EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                            0.1
                                        )
                                    },
                                    "Stadtwerke Jena-Ost",
                                    "New Green Deal"
                                ),
                                DateTime.Parse("2020-09-21T00:00:00Z")
                            );

            #endregion

            var patchResult = Location1.TryPatch(JObject.Parse(@"{ ""last_updated"": ""I-N-V-A-L-I-D!"" }"));

            Assert.IsNotNull(patchResult);
            Assert.IsFalse  (patchResult.IsSuccess);
            Assert.IsTrue   (patchResult.IsFailed);
            Assert.IsNotNull(patchResult.ErrorResponse);
            Assert.AreEqual ("Invalid JSON merge patch of a location: Invalid 'last updated'!",  patchResult.ErrorResponse);
            Assert.IsNotNull(patchResult.PatchedData);

            Assert.AreEqual (Location_Id.Parse("LOC0001"),                                       patchResult.PatchedData.Id);
            Assert.AreEqual ("2020-09-21T00:00:00.000Z",                                         patchResult.PatchedData.LastUpdated.ToIso8601());

        }

        #endregion


    }

}
