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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.UnitTests.Datastructures
{

    /// <summary>
    /// What the OCPI Accessibility Extension 1.0.0 adds to the locations module of
    /// OCPI 3.0 is written and read back as it was: the parking places, the
    /// assistance services and the standards of a location; the reach distance, the
    /// timeouts and the standards of a charging station; the cable, the images and
    /// the standards of a connector; the protected area, surface and slope of a
    /// parking; the EVSE parking - a reference to a parking place of the location
    /// with the position of the EVSE at it and the access level between them.
    /// https://evroaming.org/wp-content/uploads/2026/01/e_accessibility_extension-1.0.0.pdf
    /// </summary>
    /// <remarks>
    /// An EVSE of 3.0 embedded one parking of its own; it refers to the parking
    /// places of its location as in 2.3.0 now, and the parking's evse_position is
    /// the EVSE parking's (Achim's decisions). The parking space's length and width
    /// are parking_space_length and _width, as the extension has them.
    /// </remarks>
    [TestFixture]
    public class AccessibilityExtensionTests
    {

        #region Data

        private static readonly Party_Idv3      partyId  = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));
        private static readonly DateTimeOffset  start    = new (2026, 10, 1, 8, 0, 0, TimeSpan.Zero);

        #endregion


        #region AParkingIsWrittenAndReadWithWhatTheExtensionAdds()

        [Test]
        public void AParkingIsWrittenAndReadWithWhatTheExtensionAdds()
        {

            var parking = AParking();
            var json    = parking.ToJSON();

            Assert.Multiple(() => {
                Assert.That(json["id"]?.Value<String>(),                      Is.EqualTo("P1"));
                Assert.That(json["parking_space_width"]?.Value<Decimal>(),    Is.EqualTo(3.6M));
                Assert.That(json["parking_space_length"]?.Value<Decimal>(),   Is.EqualTo(6M));
                Assert.That(json.ContainsKey("parking_bay_width"),            Is.False, "The old name is written.");
                Assert.That(json.ContainsKey("evse_position"),                Is.False, "The position is the EVSE parking's.");
                Assert.That(json["protected_area"]?.Value<Boolean>(),         Is.True);
                Assert.That(json["surface"]?.Value<String>(),                 Is.EqualTo("ASPHALT"));
                Assert.That(json["slope"]?.Value<String>(),                   Is.EqualTo("FLAT"));
            });

            Assert.That(Parking.TryParse(json, out var again, out var errorResponse), Is.True, errorResponse);
            Assert.That(again, Is.EqualTo(parking), "A parking read from what it wrote is another one.");

        }

        #endregion

        #region AnEVSERefersToTheParkingOfItsLocation()

        [Test]
        public void AnEVSERefersToTheParkingOfItsLocation()
        {

            var evse = AnEVSE();
            var json = evse.ToJSON();

            Assert.Multiple(() => {
                Assert.That(json["parking"] is JArray,                                Is.True, "The parking is not a list of EVSE parkings.");
                Assert.That(json["parking"]?[0]?["parking_id"]?.Value<String>(),      Is.EqualTo("P1"));
                Assert.That(json["parking"]?[0]?["evse_position"]?.Value<String>(),   Is.EqualTo("LEFT"));
                Assert.That(json["parking"]?[0]?["access_level"]?.Value<String>(),    Is.EqualTo("SAME_LEVEL"));
            });

            Assert.That(EVSE.TryParse(json, out var again, out var errorResponse), Is.True, errorResponse);

            var link = again!.Parking.Single();

            Assert.Multiple(() => {
                Assert.That(link.ParkingId,     Is.EqualTo(Parking_Id.Parse("P1")));
                Assert.That(link.EVSEPosition,  Is.EqualTo(EVSEPosition.Parse("LEFT")));
                Assert.That(link.AccessLevel,   Is.EqualTo(AccessLevel.SAME_LEVEL));
            });

        }

        #endregion

        #region AConnectorIsWrittenAndReadWithWhatTheExtensionAdds()

        [Test]
        public void AConnectorIsWrittenAndReadWithWhatTheExtensionAdds()
        {

            var connector = AConnector();
            var json      = connector.ToJSON();

            Assert.Multiple(() => {
                Assert.That(json["cable_weight"]?.Value<Decimal>(),             Is.EqualTo(2.5M));
                Assert.That(json["cable_management_system"]?.Value<Boolean>(),  Is.True);
                Assert.That(json["standards"]?.ToObject<String[]>(),            Is.EqualTo(new[] { "PAS 1899" }));
                Assert.That(json["images"]?[0]?["category"]?.Value<String>(),   Is.EqualTo("CONNECTOR"));
            });

            Assert.That(Connector.TryParse(json, out var again, out var errorResponse), Is.True, errorResponse);

            Assert.Multiple(() => {
                Assert.That(again!.CableWeight?.Value,       Is.EqualTo(2.5M));
                Assert.That(again.CableManagementSystem,     Is.True);
                Assert.That(again.Standards.Select(standard => standard.ToString()), Is.EqualTo(new[] { "PAS 1899" }));
                Assert.That(again.Images.Single().Category,  Is.EqualTo(ImageCategory.CONNECTOR));
            });

        }

        #endregion

        #region AChargingStationWritesWhatTheExtensionAdds()

        /// <summary>
        /// Written only: a charging station is read with its EVSEs from "connectors"
        /// and writes them as "evse", so it does not read back what it wrote - which
        /// is not the extension's, and is left as it is.
        /// </summary>
        [Test]
        public void AChargingStationWritesWhatTheExtensionAdds()
        {

            var station = new ChargingStation(
                              ChargingStation_Id.Parse("CS1"),
                              [ AnEVSE() ],
                              ReachDistance:             Meter.Parse_m("0.6"),
                              OperationTimeout:          TimeSpan.FromSeconds(120),
                              ExtendedOperationTimeout:  true,
                              Standards:                 [ Standard.Parse("PAS 1899") ]
                          );

            var json = station.ToJSON();

            Assert.Multiple(() => {
                Assert.That(json["reach_distance"]?.Value<Decimal>(),               Is.EqualTo(0.6M));
                Assert.That(json["operation_timeout"]?.Value<Decimal>(),            Is.EqualTo(120M));
                Assert.That(json["extended_operation_timeout"]?.Value<Boolean>(),   Is.True);
                Assert.That(json["standards"]?.ToObject<String[]>(),                Is.EqualTo(new[] { "PAS 1899" }));
            });

        }

        #endregion

        #region ALocationIsWrittenAndReadWithWhatTheExtensionAdds()

        [Test]
        public void ALocationIsWrittenAndReadWithWhatTheExtensionAdds()
        {

            var location = new Location(
                               partyId,
                               Location_Id.Parse("LOC0001"),
                               1,
                               true,
                               "Europe/Berlin",
                               Services:                  [ LocationService.ACCESSIBLE_TOILETS, LocationService.EMERGENCY_ASSISTANCE, LocationService.PERIMETER_FENCE ],
                               LastUpdated:               start,
                               ParkingPlaces:             [ AParking() ],
                               AssistanceServiceDetails:  "Call +49 3641 0000, 8:00 to 20:00, staff will plug in the cable.",
                               Standards:                 [ Standard.Parse("PAS 1899") ]
                           );

            var json = location.ToJSON();

            Assert.Multiple(() => {
                Assert.That(json["services"]?.ToObject<String[]>(),                     Is.EquivalentTo(new[] { "ACCESSIBLE_TOILETS", "EMERGENCY_ASSISTANCE", "PERIMETER_FENCE" }));
                Assert.That(json["parking_places"]?[0]?["id"]?.Value<String>(),         Is.EqualTo("P1"));
                Assert.That(json["assistance_service_details"]?.Value<String>(),        Is.EqualTo("Call +49 3641 0000, 8:00 to 20:00, staff will plug in the cable."));
                Assert.That(json["standards"]?.ToObject<String[]>(),                    Is.EqualTo(new[] { "PAS 1899" }));
            });

            Assert.That(Location.TryParse(json, out var again, out var errorResponse), Is.True, errorResponse);

            Assert.Multiple(() => {
                Assert.That(again!.Services,                    Is.EquivalentTo(location.Services));
                Assert.That(again.ParkingPlaces.Single(),       Is.EqualTo(AParking()));
                Assert.That(again.AssistanceServiceDetails,     Is.EqualTo(location.AssistanceServiceDetails));
                Assert.That(again.Standards,                    Is.EquivalentTo(location.Standards));
            });

        }

        #endregion

        #region TheLocationServicesAreTheSpecifications()

        /// <summary>
        /// ASSISTANCE was declared as FLASSISTANCEAT, PERIMETER_FENCE was missing, and
        /// TIME - a dimension of a tariff - was one.
        /// </summary>
        [Test]
        public void TheLocationServicesAreTheSpecifications()
        {

            var known = typeof(LocationService).
                            GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).
                            Where (property => property.PropertyType == typeof(LocationService)).
                            Select(property => property.Name).
                            ToArray();

            Assert.That(known, Is.EquivalentTo(new[] {
                                   "ACCESSIBLE_CHARGING", "ACCESSIBLE_TOILETS", "ASSISTANCE", "CAMERA_SURVEILLANCE",
                                   "EMERGENCY_CALL", "EMERGENCY_ASSISTANCE", "PERIMETER_FENCE"
                               }));

            Assert.That(known.Select(name => ((LocationService) typeof(LocationService).GetProperty(name)!.GetValue(null)!).ToString()),
                        Is.EquivalentTo(known), "A service is called otherwise than it is written.");

        }

        #endregion


        #region (private static) AParking() / AConnector() / AnEVSE()

        private static Parking AParking()

            => new (
                   Parking_Id.Parse("P1"),
                   [ VehicleType.Parse("DISABLED") ],
                   ParkingDirection.Parse("PARALLEL"),
                   false,
                   false,
                   ParkingSpaceLength:  Meter.Parse_m("6"),
                   ParkingSpaceWidth:   Meter.Parse_m("3.6"),
                   ProtectedArea:       true,
                   Surface:             Surface.ASPHALT,
                   Slope:               Slope.FLAT
               );

        private static Connector AConnector()

            => new (
                   Connector_Id.Parse("1"),
                   ConnectorType.Parse("IEC_62196_T2_COMBO"),
                   ConnectorFormats.CABLE,
                   PowerTypes.DC,
                   Volt.  FromV(920),
                   Ampere.FromA(200),
                   LastUpdated:            start,
                   CableWeight:            Kilogram.FromKG(2.5M),
                   CableManagementSystem:  true,
                   Images:                 [ new Image(URL.Parse("https://example.org/connector.jpg"), ImageFileType.jpeg, ImageCategory.CONNECTOR) ],
                   Standards:              [ Standard.Parse("PAS 1899") ]
               );

        private static EVSE AnEVSE()

            => new (
                   EVSE_UId.Parse("DE*GEF*E0001*1"),
                   PresenceStatus.PRESENT,
                   [ AConnector() ],
                   [ new EVSEParking(Parking_Id.Parse("P1"), EVSEPosition.Parse("LEFT"), AccessLevel.SAME_LEVEL) ],
                   LastUpdated: start
               );

        #endregion

    }

}
