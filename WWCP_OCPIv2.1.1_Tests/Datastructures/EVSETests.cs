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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests.Datastructures
{

    /// <summary>
    /// Unit tests for EVSEs.
    /// https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/mod_locations.md#32-evse-object
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
                            [
                                new Connector(
                                    Connector_Id.Parse("1"),
                                    ConnectorType.IEC_62196_T2,
                                    ConnectorFormats.SOCKET,
                                    PowerTypes.AC_3_PHASE,
                                    Volt.  FromV(400),
                                    Ampere.FromA(30),
                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                    URL.Parse("https://open.charging.cloud/terms"),
                                    DateTime.Parse("2020-09-21T00:00:00Z")
                                ),
                                new Connector(
                                    Connector_Id.Parse("2"),
                                    ConnectorType.IEC_62196_T2_COMBO,
                                    ConnectorFormats.CABLE,
                                    PowerTypes.AC_3_PHASE,
                                    Volt.  FromV(400),
                                    Ampere.FromA(20),
                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                    URL.Parse("https://open.charging.cloud/terms"),
                                    DateTime.Parse("2020-09-21T00:00:00Z")
                                )
                            ],
                            EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                            [
                                new StatusSchedule(
                                    StatusType.INOPERATIVE,
                                    DateTime.Parse("2020-09-22T00:00:00.000Z"),
                                    DateTime.Parse("2020-09-23T00:00:00.000Z")
                                ),
                                new StatusSchedule(
                                    StatusType.OUTOFORDER,
                                    DateTime.Parse("2020-12-30T00:00:00.000Z"),
                                    DateTime.Parse("2020-12-31T00:00:00.000Z")
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
                                ParkingRestriction.EV_ONLY,
                                ParkingRestriction.PLUGGED
                            ],
                            [
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
                            ],
                            null,
                            null,
                            DateTime.Parse("2020-09-18T00:00:00Z")
                        );

            #endregion

            var json = EVSE1.ToJSON();

            Assert.That(json["uid"].                                    Value<String>(), Is.EqualTo("DE*GEF*E*LOC0001*1"));
            Assert.That(json["status"].                                 Value<String>(), Is.EqualTo("AVAILABLE"));
            Assert.That(json["connectors"]          [0]["id"].          Value<String>(), Is.EqualTo("1"));
            Assert.That(json["connectors"]          [1]["id"].          Value<String>(), Is.EqualTo("2"));
            Assert.That(json["evse_id"].                                Value<String>(), Is.EqualTo("DE*GEF*E*LOC0001*1"));
            Assert.That(json["status_schedule"]     [0]["status"].      Value<String>(), Is.EqualTo("INOPERATIVE"));
            Assert.That(json["status_schedule"]     [0]["period_begin"].Value<String>(), Is.EqualTo("2020-09-22T00:00:00.000Z"));
            Assert.That(json["status_schedule"]     [0]["period_end"].  Value<String>(), Is.EqualTo("2020-09-23T00:00:00.000Z"));
            Assert.That(json["status_schedule"]     [1]["status"].      Value<String>(), Is.EqualTo("OUTOFORDER"));
            Assert.That(json["status_schedule"]     [1]["period_begin"].Value<String>(), Is.EqualTo("2020-12-30T00:00:00.000Z"));
            Assert.That(json["status_schedule"]     [1]["period_end"].  Value<String>(), Is.EqualTo("2020-12-31T00:00:00.000Z"));
            Assert.That(json["capabilities"]        [0].                Value<String>(), Is.EqualTo("RFID_READER"));
            Assert.That(json["capabilities"]        [1].                Value<String>(), Is.EqualTo("RESERVABLE"));
            Assert.That(json["floor_level"].                            Value<String>(), Is.EqualTo("1. Stock"));
            Assert.That(json["coordinates"]            ["latitude"].    Value<String>(), Is.EqualTo("10.10000"));
            Assert.That(json["coordinates"]            ["longitude"].   Value<String>(), Is.EqualTo("20.20000"));
            Assert.That(json["physical_reference"].                     Value<String>(), Is.EqualTo("Ladestation #1"));
            Assert.That(json["directions"]          [0]["language"].    Value<String>(), Is.EqualTo("de"));
            Assert.That(json["directions"]          [0]["text"].        Value<String>(), Is.EqualTo("Bitte klingeln!"));
            Assert.That(json["directions"]          [1]["language"].    Value<String>(), Is.EqualTo("en"));
            Assert.That(json["directions"]          [1]["text"].        Value<String>(), Is.EqualTo("Ken sent me!"));
            Assert.That(json["parking_restrictions"][0].                Value<String>(), Is.EqualTo("EV_ONLY"));
            Assert.That(json["parking_restrictions"][1].                Value<String>(), Is.EqualTo("PLUGGED"));
            Assert.That(json["images"]              [0]["url"].         Value<String>(), Is.EqualTo("http://example.com/pinguine.jpg"));
            Assert.That(json["images"]              [0]["thumbnail"].   Value<String>(), Is.EqualTo("http://example.com/kleine_pinguine.jpg"));
            Assert.That(json["images"]              [0]["category"].    Value<String>(), Is.EqualTo("OPERATOR"));
            Assert.That(json["images"]              [0]["type"].        Value<String>(), Is.EqualTo("jpeg"));
            Assert.That(json["images"]              [0]["width"].       Value<UInt16>(), Is.EqualTo(100));
            Assert.That(json["images"]              [0]["height"].      Value<UInt16>(), Is.EqualTo(150));
            Assert.That(json["images"]              [1]["url"].         Value<String>(), Is.EqualTo("http://example.com/wellensittiche.jpg"));
            Assert.That(json["last_updated"].                           Value<String>(), Is.EqualTo("2020-09-18T00:00:00.000Z"));

            if (EVSE.TryParse(json, out var evse2, out var errorResponse))
            {

                Assert.That(evse2,         Is.Not.Null);
                Assert.That(errorResponse, Is.Null);

                Assert.That(evse2.UId,                     Is.EqualTo(EVSE1.UId));
                Assert.That(evse2.Status,                  Is.EqualTo(EVSE1.Status));
                Assert.That(evse2.Connectors,              Is.EqualTo(EVSE1.Connectors));
                Assert.That(evse2.EVSEId,                  Is.EqualTo(EVSE1.EVSEId));
                Assert.That(evse2.StatusSchedule,          Is.EqualTo(EVSE1.StatusSchedule));
                Assert.That(evse2.Capabilities,            Is.EqualTo(EVSE1.Capabilities));
                Assert.That(evse2.FloorLevel,              Is.EqualTo(EVSE1.FloorLevel));
                Assert.That(evse2.Coordinates,             Is.EqualTo(EVSE1.Coordinates));
                Assert.That(evse2.PhysicalReference,       Is.EqualTo(EVSE1.PhysicalReference));
                Assert.That(evse2.Directions,              Is.EqualTo(EVSE1.Directions));
                Assert.That(evse2.ParkingRestrictions,     Is.EqualTo(EVSE1.ParkingRestrictions));
                Assert.That(evse2.Images,                  Is.EqualTo(EVSE1.Images));
                Assert.That(evse2.LastUpdated.ToISO8601(), Is.EqualTo(EVSE1.LastUpdated.ToISO8601()));

            }
            else
                Assert.Fail("Failed to parse EVSE!");

        }

        #endregion

    }

}
