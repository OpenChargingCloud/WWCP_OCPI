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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.Datastructures
{

    /// <summary>
    /// What the OCPI Accessibility Extension 1.0.0 adds to the locations module of
    /// OCPI 2.3.0 is read and written back as it came: services, the details of
    /// the assistance services and the standards of a location; the reach distance,
    /// the timeouts and the standards of an EVSE; the cable, the standards and the images of a
    /// connector; the
    /// protected area, surface and slope of a parking; the access level between an
    /// EVSE and its parking; images of a connector or of the parking.
    /// https://evroaming.org/wp-content/uploads/2026/01/e_accessibility_extension-1.0.0.pdf
    /// </summary>
    [TestFixture]
    public class AccessibilityExtensionTests
    {

        #region Data

        /// <summary>
        /// A location with everything the extension adds, as a CPO would send it.
        /// </summary>
        private const String LocationJSON = """
            {
                "country_code":  "DE",
                "party_id":      "GEF",
                "id":            "LOC0001",
                "publish":       true,
                "address":       "Biberweg 18",
                "city":          "Jena",
                "postal_code":   "07749",
                "country":       "DEU",
                "coordinates":   { "latitude": "50.90000", "longitude": "11.60000" },
                "time_zone":     "Europe/Berlin",
                "services":      [ "ACCESSIBLE_TOILETS", "ASSISTANCE", "EMERGENCY_ASSISTANCE" ],
                "assistance_service_details": "Call +49 3641 0000, 8:00 to 20:00, staff will plug in the cable.",
                "standards":     [ "PAS 1899" ],
                "images": [
                    { "url": "https://example.org/parking.jpg", "category": "PARKING", "type": "jpeg" }
                ],
                "parking_places": [
                    {
                        "id":                    "P1",
                        "vehicle_types":         [ "DISABLED" ],
                        "restricted_to_type":    false,
                        "reservation_required":  false,
                        "parking_space_width":   360,
                        "protected_area":        true,
                        "surface":               "ASPHALT",
                        "slope":                 "FLAT"
                    }
                ],
                "evses": [
                    {
                        "uid":                         "DE*GEF*E0001*1",
                        "status":                      "AVAILABLE",
                        "reach_distance":              60,
                        "operation_timeout":           120,
                        "extended_operation_timeout":  true,
                        "standards":                   [ "PAS 1899" ],
                        "parking": [
                            { "parking_id": "P1", "evse_position": "LEFT", "access_level": "ACCESSIBLE_TRANSITION" }
                        ],
                        "connectors": [
                            {
                                "id":                       "1",
                                "standard":                 "IEC_62196_T2_COMBO",
                                "format":                   "CABLE",
                                "power_type":               "DC",
                                "max_voltage":              920,
                                "max_amperage":             200,
                                "cable_length":             500,
                                "cable_weight":             2.5,
                                "cable_management_system":  true,
                                "standards":                [ "PAS 1899" ],
                                "images": [
                                    { "url": "https://example.org/socket.jpg", "category": "CONNECTOR", "type": "jpeg" }
                                ],
                                "last_updated":             "2026-10-01T08:00:00Z"
                            }
                        ],
                        "images": [
                            { "url": "https://example.org/connector.jpg", "category": "CONNECTOR", "type": "jpeg" }
                        ],
                        "last_updated": "2026-10-01T08:00:00Z"
                    }
                ],
                "last_updated": "2026-10-01T08:00:00Z"
            }
            """;

        #endregion


        #region ALocationTakesWhatTheExtensionAdds()

        [Test]
        public void ALocationTakesWhatTheExtensionAdds()
        {

            Assert.That(Location.TryParse(JObject.Parse(LocationJSON), out var location, out var errorResponse), Is.True, errorResponse);

            var evse      = location!.EVSEs.Single();
            var connector = evse.Connectors.Single();
            var parking   = location.ParkingPlaces.Single();
            var link      = evse.Parking.Single();

            Assert.Multiple(() => {

                Assert.That(location.Services.Select(service => service.ToString()), Is.EquivalentTo(new[] { "ACCESSIBLE_TOILETS", "ASSISTANCE", "EMERGENCY_ASSISTANCE" }));
                Assert.That(location.Services,                         Does.Contain(LocationService.EMERGENCY_ASSISTANCE));
                Assert.That(location.AssistanceServiceDetails,         Is.EqualTo("Call +49 3641 0000, 8:00 to 20:00, staff will plug in the cable."));
                Assert.That(location.Standards.Select(standard => standard.ToString()), Is.EqualTo(new[] { "PAS 1899" }));
                Assert.That(location.Images.Single().Category,         Is.EqualTo(ImageCategory.PARKING));

                Assert.That(evse.ReachDistance?.cm,                    Is.EqualTo(60));
                Assert.That(evse.OperationTimeout,                     Is.EqualTo(TimeSpan.FromSeconds(120)));
                Assert.That(evse.ExtendedOperationTimeout,             Is.True);
                Assert.That(evse.Standards.Select(standard => standard.ToString()), Is.EqualTo(new[] { "PAS 1899" }));
                Assert.That(evse.Images.Single().Category,             Is.EqualTo(ImageCategory.CONNECTOR));

                Assert.That(link.EVSEPosition,                         Is.EqualTo(EVSEPosition.LEFT));
                Assert.That(link.AccessLevel,                          Is.EqualTo(AccessLevel.ACCESSIBLE_TRANSITION));

                Assert.That(connector.CableLength?.cm,                 Is.EqualTo(500));
                Assert.That(connector.CableWeight?.Value,              Is.EqualTo(2.5M));
                Assert.That(connector.CableManagementSystem,           Is.True);
                Assert.That(connector.Standards.Select(standard => standard.ToString()), Is.EqualTo(new[] { "PAS 1899" }));
                Assert.That(connector.Images.Single().Category,        Is.EqualTo(ImageCategory.CONNECTOR));

                Assert.That(parking.ProtectedArea,                     Is.True);
                Assert.That(parking.Surface,                           Is.EqualTo(Surface.ASPHALT));
                Assert.That(parking.Slope,                             Is.EqualTo(Slope.FLAT));

            });

        }

        #endregion

        #region ALocationWritesBackWhatTheExtensionAdds()

        /// <summary>
        /// Read and written again, everything the extension adds is in the JSON as
        /// it came - and a location read from what it wrote equals the first one.
        /// </summary>
        [Test]
        public void ALocationWritesBackWhatTheExtensionAdds()
        {

            Assert.That(Location.TryParse(JObject.Parse(LocationJSON), out var location, out var errorResponse), Is.True, errorResponse);

            var json      = location!.ToJSON();
            var given     = JObject.Parse(LocationJSON);
            var evse      = json["evses"]![0]!;
            var connector = evse["connectors"]![0]!;
            var parking   = json["parking_places"]![0]!;

            Assert.Multiple(() => {

                Assert.That(JToken.DeepEquals(json["services"],                    given["services"]),                    Is.True, "services");
                Assert.That(JToken.DeepEquals(json["assistance_service_details"],  given["assistance_service_details"]),  Is.True, "assistance_service_details");
                Assert.That(JToken.DeepEquals(json["standards"],                   given["standards"]),                   Is.True, "standards");

                Assert.That(evse["reach_distance"]?.Value<Decimal>(),               Is.EqualTo(60M));
                Assert.That(evse["operation_timeout"]?.Value<Decimal>(),            Is.EqualTo(120M));
                Assert.That(evse["extended_operation_timeout"]?.Value<Boolean>(),   Is.True);
                Assert.That(evse["standards"]?.ToObject<String[]>(),                Is.EqualTo(new[] { "PAS 1899" }));
                Assert.That(evse["parking"]?[0]?["access_level"]?.Value<String>(),  Is.EqualTo("ACCESSIBLE_TRANSITION"));
                Assert.That(evse["images"]?[0]?["category"]?.Value<String>(),       Is.EqualTo("CONNECTOR"));

                Assert.That(connector["cable_length"]?.Value<Decimal>(),            Is.EqualTo(500M));
                Assert.That(connector["cable_weight"]?.Value<Decimal>(),            Is.EqualTo(2.5M));
                Assert.That(connector["cable_management_system"]?.Value<Boolean>(), Is.True);
                Assert.That(connector["standards"]?.ToObject<String[]>(),           Is.EqualTo(new[] { "PAS 1899" }));
                Assert.That(connector["images"]?[0]?["category"]?.Value<String>(),  Is.EqualTo("CONNECTOR"));

                Assert.That(parking["protected_area"]?.Value<Boolean>(),            Is.True);
                Assert.That(parking["surface"]?.Value<String>(),                    Is.EqualTo("ASPHALT"));
                Assert.That(parking["slope"]?.Value<String>(),                      Is.EqualTo("FLAT"));

            });

            Assert.That(Location.TryParse(json, out var again, out errorResponse), Is.True, errorResponse);
            Assert.That(again, Is.EqualTo(location), "A location read from what it wrote is another one.");

        }

        #endregion

        #region AnEVSEParkingWithoutAPositionHasNone()

        /// <summary>
        /// An EVSE parking without an evse_position was read into a position that
        /// was there and empty: written back, it said "evse_position": "".
        /// </summary>
        [Test]
        public void AnEVSEParkingWithoutAPositionHasNone()
        {

            Assert.That(EVSEParking.TryParse(JObject.Parse("""{ "parking_id": "P1" }"""), out var parking, out var errorResponse), Is.True, errorResponse);

            Assert.Multiple(() => {
                Assert.That(parking!.EVSEPosition,                     Is.Null, "A position that was not given is there.");
                Assert.That(parking.ToJSON().ContainsKey("evse_position"), Is.False, "A position that was not given is written.");
            });

        }

        #endregion

        #region UnknownValuesOfTheOpenEnumsAreKept()

        /// <summary>
        /// The extension's enums are open: a value not known here is read and written back.
        /// </summary>
        [Test]
        public void UnknownValuesOfTheOpenEnumsAreKept()
        {

            Assert.That(EVSEParking.TryParse(JObject.Parse("""{ "parking_id": "P1", "access_level": "LIFT" }"""), out var parking, out var errorResponse), Is.True, errorResponse);

            Assert.That(parking!.ToJSON()["access_level"]?.Value<String>(), Is.EqualTo("LIFT"));

        }

        #endregion

    }

}
