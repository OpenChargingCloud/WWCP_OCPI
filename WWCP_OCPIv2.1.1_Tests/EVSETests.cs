/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests
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
                            new Connector[] {
                                new Connector(
                                    Connector_Id.Parse("1"),
                                    ConnectorType.IEC_62196_T2,
                                    ConnectorFormats.SOCKET,
                                    PowerTypes.AC_3_PHASE,
                                    400,
                                    30,
                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                    URL.Parse("https://open.charging.cloud/terms"),
                                    DateTime.Parse("2020-09-21T00:00:00Z")
                                ),
                                new Connector(
                                    Connector_Id.Parse("2"),
                                    ConnectorType.IEC_62196_T2_COMBO,
                                    ConnectorFormats.CABLE,
                                    PowerTypes.AC_3_PHASE,
                                    400,
                                    20,
                                    Tariff_Id.Parse("DE*GEF*T0003"),
                                    URL.Parse("https://open.charging.cloud/terms"),
                                    DateTime.Parse("2020-09-21T00:00:00Z")
                                )
                            },
                            EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                            new StatusSchedule[] {
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
                            },
                            new Capability[] {
                                Capability.RFID_READER,
                                Capability.RESERVABLE
                            },

                            // OCPI Computer Science Extensions
                            new EnergyMeter(
                                Meter_Id.Parse("Meter0815"),
                                "EnergyMeter Model #1",
                                "hw. v1.80",
                                "fw. v1.20",
                                "Energy Metering Services",
                                null,
                                null,
                                new TransparencySoftwareStatus[] {
                                    new TransparencySoftwareStatus(
                                        new TransparencySoftware(
                                            "Chargy Transparency Software Desktop Application",
                                            "v1.00",
                                            OpenSourceLicenses.GPL3,
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
                                            OpenSourceLicenses.GPL3,
                                            "GraphDefined GmbH",
                                            URL.Parse("https://open.charging.cloud/logo.svg"),
                                            URL.Parse("https://open.charging.cloud/Chargy/howto"),
                                            URL.Parse("https://open.charging.cloud/Chargy"),
                                            URL.Parse("https://github.com/OpenChargingCloud/ChargyMobileApp")
                                        ),
                                        LegalStatus.ForInformationOnly,
                                        "no cert",
                                        "GraphDefiend",
                                        NotBefore: DateTime.Parse("2019-04-01T00:00:00.000Z").ToUniversalTime(),
                                        NotAfter:  DateTime.Parse("2030-01-01T00:00:00.000Z").ToUniversalTime()
                                    )
                                }
                            ),

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
                            DateTime.Parse("2020-09-18T00:00:00Z")
                        );

            #endregion

            var JSON = EVSE1.ToJSON();

            Assert.AreEqual("DE*GEF*E*LOC0001*1",                     JSON["uid"].                                    Value<String>());
            Assert.AreEqual("AVAILABLE",                              JSON["status"].                                 Value<String>());
            Assert.AreEqual("1",                                      JSON["connectors"]          [0]["id"].          Value<String>());
            Assert.AreEqual("2",                                      JSON["connectors"]          [1]["id"].          Value<String>());
            Assert.AreEqual("DE*GEF*E*LOC0001*1",                     JSON["evse_id"].                                Value<String>());
            Assert.AreEqual("INOPERATIVE",                            JSON["status_schedule"]     [0]["status"].      Value<String>());
            Assert.AreEqual("2020-09-22T00:00:00.000Z",               JSON["status_schedule"]     [0]["period_begin"].Value<String>());
            Assert.AreEqual("2020-09-23T00:00:00.000Z",               JSON["status_schedule"]     [0]["period_end"].  Value<String>());
            Assert.AreEqual("OUTOFORDER",                             JSON["status_schedule"]     [1]["status"].      Value<String>());
            Assert.AreEqual("2020-12-30T00:00:00.000Z",               JSON["status_schedule"]     [1]["period_begin"].Value<String>());
            Assert.AreEqual("2020-12-31T00:00:00.000Z",               JSON["status_schedule"]     [1]["period_end"].  Value<String>());
            Assert.AreEqual("RFID_READER",                            JSON["capabilities"]        [0].                Value<String>());
            Assert.AreEqual("RESERVABLE",                             JSON["capabilities"]        [1].                Value<String>());
            Assert.AreEqual("1. Stock",                               JSON["floor_level"].                            Value<String>());
            Assert.AreEqual("10.10000",                               JSON["coordinates"]            ["latitude"].    Value<String>());
            Assert.AreEqual("20.20000",                               JSON["coordinates"]            ["longitude"].   Value<String>());
            Assert.AreEqual("Ladestation #1",                         JSON["physical_reference"].                     Value<String>());
            Assert.AreEqual("de",                                     JSON["directions"]          [0]["language"].    Value<String>());
            Assert.AreEqual("Bitte klingeln!",                        JSON["directions"]          [0]["text"].        Value<String>());
            Assert.AreEqual("en",                                     JSON["directions"]          [1]["language"].    Value<String>());
            Assert.AreEqual("Ken sent me!",                           JSON["directions"]          [1]["text"].        Value<String>());
            Assert.AreEqual("EV_ONLY",                                JSON["parking_restrictions"][0].                Value<String>());
            Assert.AreEqual("PLUGGED",                                JSON["parking_restrictions"][1].                Value<String>());
            Assert.AreEqual("http://example.com/pinguine.jpg",        JSON["images"]              [0]["url"].         Value<String>());
            Assert.AreEqual("http://example.com/kleine_pinguine.jpg", JSON["images"]              [0]["thumbnail"].   Value<String>());
            Assert.AreEqual("OPERATOR",                               JSON["images"]              [0]["category"].    Value<String>());
            Assert.AreEqual("jpeg",                                   JSON["images"]              [0]["type"].        Value<String>());
            Assert.AreEqual(100,                                      JSON["images"]              [0]["width"].       Value<UInt16>());
            Assert.AreEqual(150,                                      JSON["images"]              [0]["height"].      Value<UInt16>());
            Assert.AreEqual("http://example.com/wellensittiche.jpg",  JSON["images"]              [1]["url"].         Value<String>());
            Assert.AreEqual("2020-09-18T00:00:00.000Z",               JSON["last_updated"].                           Value<String>());

            Assert.IsTrue(EVSE.TryParse(JSON, out EVSE EVSE2, out String ErrorResponse));
            Assert.IsNull(ErrorResponse);

            Assert.AreEqual(EVSE1.UId,                                EVSE2.UId);
            Assert.AreEqual(EVSE1.Status,                             EVSE2.Status);
            Assert.AreEqual(EVSE1.Connectors,                         EVSE2.Connectors);
            Assert.AreEqual(EVSE1.EVSEId,                             EVSE2.EVSEId);
            Assert.AreEqual(EVSE1.StatusSchedule,                     EVSE2.StatusSchedule);
            Assert.AreEqual(EVSE1.Capabilities,                       EVSE2.Capabilities);
            Assert.AreEqual(EVSE1.FloorLevel,                         EVSE2.FloorLevel);
            Assert.AreEqual(EVSE1.Coordinates,                        EVSE2.Coordinates);
            Assert.AreEqual(EVSE1.PhysicalReference,                  EVSE2.PhysicalReference);
            Assert.AreEqual(EVSE1.Directions,                         EVSE2.Directions);
            Assert.AreEqual(EVSE1.ParkingRestrictions,                EVSE2.ParkingRestrictions);
            Assert.AreEqual(EVSE1.Images,                             EVSE2.Images);
            Assert.AreEqual(EVSE1.LastUpdated.ToIso8601(),            EVSE2.LastUpdated.ToIso8601());

        }

        #endregion

    }

}
