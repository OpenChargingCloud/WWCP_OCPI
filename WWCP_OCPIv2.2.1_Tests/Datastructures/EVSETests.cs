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

using NUnit.Framework;
using NUnit.Framework.Legacy;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.Datastructures
{

    /// <summary>
    /// Unit tests for EVSEs.
    /// https://github.com/ocpi/ocpi/blob/master/mod_locations.asciidoc#mod_locations_evse_object
    /// </summary>
    [TestFixture]
    public static class EVSETests
    {

        #region EVSE_SerializeDeserialize_Test01()

        /// <summary>
        /// EVSE serialize, deserialize and compare test.
        /// </summary>
        [Test]
        public static void EVSE_SerializeDeserialize_Test01()
        {

            #region Define EVSE1

            var EVSE1 = new EVSE(
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
                                    Watt.  ParseW(12),
                                    new[] {
                                        Tariff_Id.Parse("DE*GEF*T0001"),
                                        Tariff_Id.Parse("DE*GEF*T0002")
                                    },
                                    URL.Parse("https://open.charging.cloud/terms"),
                                    DateTime.Parse("2020-09-21T00:00:00Z").ToUniversalTime()
                                ),
                                new Connector(
                                    Connector_Id.Parse("2"),
                                    ConnectorType.IEC_62196_T2_COMBO,
                                    ConnectorFormats.CABLE,
                                    PowerTypes.AC_3_PHASE,
                                    Volt.  ParseV(400),
                                    Ampere.ParseA(20),
                                    Watt.  ParseW(8),
                                    new[] {
                                        Tariff_Id.Parse("DE*GEF*T0003"),
                                        Tariff_Id.Parse("DE*GEF*T0004")
                                    },
                                    URL.Parse("https://open.charging.cloud/terms"),
                                    DateTime.Parse("2020-09-21T00:00:00Z").ToUniversalTime()
                                )
                            },
                            EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                            new[] {
                                new StatusSchedule(
                                    StatusType.INOPERATIVE,
                                    DateTime.Parse("2020-09-22T00:00:00.000Z").ToUniversalTime(),
                                    DateTime.Parse("2020-09-23T00:00:00.000Z").ToUniversalTime()
                                ),
                                new StatusSchedule(
                                    StatusType.OUTOFORDER,
                                    DateTime.Parse("2020-12-30T00:00:00.000Z").ToUniversalTime(),
                                    DateTime.Parse("2020-12-31T00:00:00.000Z").ToUniversalTime()
                                )
                            },
                            new[] {
                                Capability.RFID_READER,
                                Capability.RESERVABLE
                            },

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
                                null,
                                new TransparencySoftwareStatus[] {
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
                                ParkingRestriction.EV_ONLY,
                                ParkingRestriction.PLUGGED
                            },
                            new[] {
                                new Image(
                                    URL.Parse("http://example.com/pinguine.jpg"),
                                    ImageFileType.jpeg,
                                    ImageCategory.OPERATOR,
                                    100,
                                    150,
                                    URL.Parse("http://example.com/kleine_pinguine.jpg")
                                ),
                                new Image(
                                    URL.Parse("http://example.com/wellensittiche.jpg"),
                                    ImageFileType.png,
                                    ImageCategory.ENTRANCE,
                                    200,
                                    300,
                                    URL.Parse("http://example.com/kleine_wellensittiche.jpg")
                                )
                            },
                            DateTime.Parse("2020-09-18T00:00:00Z").ToUniversalTime()
                        );

            #endregion

            var JSON = EVSE1.ToJSON();

            ClassicAssert.AreEqual("DE*GEF*E*LOC0001*1",                     JSON["uid"].                                    Value<String>());
            ClassicAssert.AreEqual("AVAILABLE",                              JSON["status"].                                 Value<String>());
            ClassicAssert.AreEqual("1",                                      JSON["connectors"]          [0]["id"].          Value<String>());
            ClassicAssert.AreEqual("2",                                      JSON["connectors"]          [1]["id"].          Value<String>());
            ClassicAssert.AreEqual("DE*GEF*E*LOC0001*1",                     JSON["evse_id"].                                Value<String>());
            ClassicAssert.AreEqual("INOPERATIVE",                            JSON["status_schedule"]     [0]["status"].      Value<String>());
            ClassicAssert.AreEqual("2020-09-22T00:00:00.000Z",               JSON["status_schedule"]     [0]["period_begin"].Value<String>());
            ClassicAssert.AreEqual("2020-09-23T00:00:00.000Z",               JSON["status_schedule"]     [0]["period_end"].  Value<String>());
            ClassicAssert.AreEqual("OUTOFORDER",                             JSON["status_schedule"]     [1]["status"].      Value<String>());
            ClassicAssert.AreEqual("2020-12-30T00:00:00.000Z",               JSON["status_schedule"]     [1]["period_begin"].Value<String>());
            ClassicAssert.AreEqual("2020-12-31T00:00:00.000Z",               JSON["status_schedule"]     [1]["period_end"].  Value<String>());
            ClassicAssert.AreEqual("RFID_READER",                            JSON["capabilities"]        [0].                Value<String>());
            ClassicAssert.AreEqual("RESERVABLE",                             JSON["capabilities"]        [1].                Value<String>());
            ClassicAssert.AreEqual("1. Stock",                               JSON["floor_level"].                            Value<String>());
            ClassicAssert.AreEqual("10.10000",                               JSON["coordinates"]            ["latitude"].    Value<String>());
            ClassicAssert.AreEqual("20.20000",                               JSON["coordinates"]            ["longitude"].   Value<String>());
            ClassicAssert.AreEqual("Ladestation #1",                         JSON["physical_reference"].                     Value<String>());
            ClassicAssert.AreEqual("de",                                     JSON["directions"]          [0]["language"].    Value<String>());
            ClassicAssert.AreEqual("Bitte klingeln!",                        JSON["directions"]          [0]["text"].        Value<String>());
            ClassicAssert.AreEqual("en",                                     JSON["directions"]          [1]["language"].    Value<String>());
            ClassicAssert.AreEqual("Ken sent me!",                           JSON["directions"]          [1]["text"].        Value<String>());
            ClassicAssert.AreEqual("EV_ONLY",                                JSON["parking_restrictions"][0].                Value<String>());
            ClassicAssert.AreEqual("PLUGGED",                                JSON["parking_restrictions"][1].                Value<String>());
            ClassicAssert.AreEqual("http://example.com/pinguine.jpg",        JSON["images"]              [0]["url"].         Value<String>());
            ClassicAssert.AreEqual("http://example.com/kleine_pinguine.jpg", JSON["images"]              [0]["thumbnail"].   Value<String>());
            ClassicAssert.AreEqual("OPERATOR",                               JSON["images"]              [0]["category"].    Value<String>());
            ClassicAssert.AreEqual("jpeg",                                   JSON["images"]              [0]["type"].        Value<String>());
            ClassicAssert.AreEqual(100,                                      JSON["images"]              [0]["width"].       Value<UInt16>());
            ClassicAssert.AreEqual(150,                                      JSON["images"]              [0]["height"].      Value<UInt16>());
            ClassicAssert.AreEqual("http://example.com/wellensittiche.jpg",  JSON["images"]              [1]["url"].         Value<String>());
            ClassicAssert.AreEqual("2020-09-18T00:00:00.000Z",               JSON["last_updated"].                           Value<String>());

            ClassicAssert.IsTrue(EVSE.TryParse(JSON, out var evse2, out var errorResponse));
            ClassicAssert.IsNull(errorResponse);

            ClassicAssert.AreEqual(EVSE1.UId,                                evse2.UId);
            ClassicAssert.AreEqual(EVSE1.Status,                             evse2.Status);
            ClassicAssert.AreEqual(EVSE1.Connectors,                         evse2.Connectors);
            ClassicAssert.IsTrue  (EVSE1.Connectors.        First().Equals(evse2.Connectors.        First()));
            ClassicAssert.IsTrue  (EVSE1.Connectors.Skip(1).First().Equals(evse2.Connectors.Skip(1).First()));
            ClassicAssert.AreEqual(EVSE1.EVSEId,                             evse2.EVSEId);
            ClassicAssert.AreEqual(EVSE1.StatusSchedule,                     evse2.StatusSchedule);
            ClassicAssert.AreEqual(EVSE1.Capabilities,                       evse2.Capabilities);
            ClassicAssert.AreEqual(EVSE1.FloorLevel,                         evse2.FloorLevel);
            ClassicAssert.AreEqual(EVSE1.Coordinates,                        evse2.Coordinates);
            ClassicAssert.AreEqual(EVSE1.PhysicalReference,                  evse2.PhysicalReference);
            ClassicAssert.AreEqual(EVSE1.Directions,                         evse2.Directions);
            ClassicAssert.AreEqual(EVSE1.ParkingRestrictions,                evse2.ParkingRestrictions);
            ClassicAssert.AreEqual(EVSE1.Images,                             evse2.Images);
            ClassicAssert.AreEqual(EVSE1.LastUpdated.ToIso8601(),            evse2.LastUpdated.ToIso8601());

        }

        #endregion

    }

}
