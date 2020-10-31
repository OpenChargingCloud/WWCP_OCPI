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
using System.Collections.Generic;

using NUnit.Framework;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Aegir;

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

            var LocationA = new Location(CountryCode.Parse("DE"),
                                         Party_Id.Parse("GEF"),
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
                                                         "Public money, public code!",
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
                                                         "More public money, more public code!",
                                                         DateTime.Parse("2020-09-22")
                                                     )
                                                 },
                                                 EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                                 new StatusSchedule[] {
                                                     new StatusSchedule(
                                                         StatusTypes.INOPERATIVE,
                                                         DateTime.Parse("2020-09-23"),
                                                         DateTime.Parse("2020-09-24")
                                                     )
                                                 },
                                                 new CapabilityTypes[] {
                                                     CapabilityTypes.RFID_READER,
                                                     CapabilityTypes.RESERVABLE
                                                 },
                                                 "1. Stock",
                                                 GeoCoordinate.Parse(10.1, 20.2),
                                                 "Ladestation #1",
                                                 DisplayText.Create(Languages.de, "Ken sent me!"),
                                                 new ParkingRestrictions[] {
                                                     ParkingRestrictions.EV_ONLY
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
                                                     DateTime.Parse("2020-12-22"),
                                                     DateTime.Parse("2020-12-22")
                                                 )
                                             },
                                             new ExceptionalPeriod[] {
                                                 new ExceptionalPeriod(
                                                     DateTime.Parse("2020-12-22"),
                                                     DateTime.Parse("2020-12-22")
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
                                         new EnergyMix(true,
                                                       new EnergySource[] {
                                                           new EnergySource(
                                                               EnergySourceCategories.SOLAR,
                                                               80
                                                           ),
                                                           new EnergySource(
                                                               EnergySourceCategories.WIND,
                                                               20
                                                           )
                                                       } as IEnumerable<EnergySource>,
                                                       new EnvironmentalImpact[] {
                                                           new EnvironmentalImpact(
                                                               EnvironmentalImpactCategories.CARBON_DIOXIDE,
                                                               0.1
                                                           )
                                                       } as IEnumerable<EnvironmentalImpact>,
                                                       "Stadtwerke Jena-Ost",
                                                       "Bio 3000"),
                                         DateTime.Parse("2020-09-12")
                                     );

            var JSON = LocationA.ToJSON();

            Assert.AreEqual("DE",                               JSON["country_code"].Value<String>());
            Assert.AreEqual("GEF",                              JSON["party_id"].    Value<String>());
            Assert.AreEqual("LOC0001",                          JSON["id"].          Value<String>());

            Assert.IsTrue(Location.TryParse(JSON, out Location LocationB, out String ErrorResponse));
            Assert.IsNull(ErrorResponse);

            Assert.AreEqual(LocationA.CountryCode,              LocationB.CountryCode);
            Assert.AreEqual(LocationA.PartyId,                  LocationB.PartyId);
            Assert.AreEqual(LocationA.Id,                       LocationB.Id);

            Assert.AreEqual(LocationA.Publish,                  LocationB.Publish);
            Assert.AreEqual(LocationA.Address,                  LocationB.Address);
            Assert.AreEqual(LocationA.City,                     LocationB.City);
            Assert.AreEqual(LocationA.Country,                  LocationB.Country);
            Assert.AreEqual(LocationA.Coordinates,              LocationB.Coordinates);
            Assert.AreEqual(LocationA.Timezone,                 LocationB.Timezone);

            Assert.AreEqual(LocationA.PublishAllowedTo,         LocationB.PublishAllowedTo);
            Assert.AreEqual(LocationA.Name,                     LocationB.Name);
            Assert.AreEqual(LocationA.PostalCode,               LocationB.PostalCode);
            Assert.AreEqual(LocationA.State,                    LocationB.State);
            Assert.AreEqual(LocationA.RelatedLocations,         LocationB.RelatedLocations);
            Assert.AreEqual(LocationA.ParkingType,              LocationB.ParkingType);
            //Assert.AreEqual(LocationA.EVSEs,                    LocationB.EVSEs);
            Assert.AreEqual(LocationA.Directions,               LocationB.Directions);
            Assert.AreEqual(LocationA.Operator,                 LocationB.Operator);
            Assert.AreEqual(LocationA.SubOperator,              LocationB.SubOperator);
            Assert.AreEqual(LocationA.Owner,                    LocationB.Owner);
            Assert.AreEqual(LocationA.Facilities,               LocationB.Facilities);
            //Assert.AreEqual(LocationA.OpeningTimes,             LocationB.OpeningTimes);
            Assert.AreEqual(LocationA.ChargingWhenClosed,       LocationB.ChargingWhenClosed);
            Assert.AreEqual(LocationA.Images,                   LocationB.Images);
            Assert.AreEqual(LocationA.EnergyMix,                LocationB.EnergyMix);

            Assert.AreEqual(LocationA.LastUpdated.ToIso8601(),  LocationB.LastUpdated.ToIso8601());

        }

        #endregion

    }

}
