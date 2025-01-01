/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using NUnit.Framework.Legacy;

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
                                    Volt.  ParseV(400),
                                    Ampere.ParseA(30),
                                    Tariff_Id.Parse("DE*GEF*T0001"),
                                    URL.Parse("https://open.charging.cloud/terms"),
                                    DateTime.Parse("2020-09-21T00:00:00Z")
                                ),
                                new Connector(
                                    Connector_Id.Parse("2"),
                                    ConnectorType.IEC_62196_T2_COMBO,
                                    ConnectorFormats.CABLE,
                                    PowerTypes.AC_3_PHASE,
                                    Volt.  ParseV(400),
                                    Ampere.ParseA(20),
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
                            new EnergyMeter(
                                Meter_Id.Parse("Meter0815"),
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
                            DateTime.Parse("2020-09-18T00:00:00Z")
                        );

            #endregion

            var json = EVSE1.ToJSON();

            ClassicAssert.AreEqual("DE*GEF*E*LOC0001*1",                     json["uid"].                                    Value<String>());
            ClassicAssert.AreEqual("AVAILABLE",                              json["status"].                                 Value<String>());
            ClassicAssert.AreEqual("1",                                      json["connectors"]          [0]["id"].          Value<String>());
            ClassicAssert.AreEqual("2",                                      json["connectors"]          [1]["id"].          Value<String>());
            ClassicAssert.AreEqual("DE*GEF*E*LOC0001*1",                     json["evse_id"].                                Value<String>());
            ClassicAssert.AreEqual("INOPERATIVE",                            json["status_schedule"]     [0]["status"].      Value<String>());
            ClassicAssert.AreEqual("2020-09-22T00:00:00.000Z",               json["status_schedule"]     [0]["period_begin"].Value<String>());
            ClassicAssert.AreEqual("2020-09-23T00:00:00.000Z",               json["status_schedule"]     [0]["period_end"].  Value<String>());
            ClassicAssert.AreEqual("OUTOFORDER",                             json["status_schedule"]     [1]["status"].      Value<String>());
            ClassicAssert.AreEqual("2020-12-30T00:00:00.000Z",               json["status_schedule"]     [1]["period_begin"].Value<String>());
            ClassicAssert.AreEqual("2020-12-31T00:00:00.000Z",               json["status_schedule"]     [1]["period_end"].  Value<String>());
            ClassicAssert.AreEqual("RFID_READER",                            json["capabilities"]        [0].                Value<String>());
            ClassicAssert.AreEqual("RESERVABLE",                             json["capabilities"]        [1].                Value<String>());
            ClassicAssert.AreEqual("1. Stock",                               json["floor_level"].                            Value<String>());
            ClassicAssert.AreEqual("10.10000",                               json["coordinates"]            ["latitude"].    Value<String>());
            ClassicAssert.AreEqual("20.20000",                               json["coordinates"]            ["longitude"].   Value<String>());
            ClassicAssert.AreEqual("Ladestation #1",                         json["physical_reference"].                     Value<String>());
            ClassicAssert.AreEqual("de",                                     json["directions"]          [0]["language"].    Value<String>());
            ClassicAssert.AreEqual("Bitte klingeln!",                        json["directions"]          [0]["text"].        Value<String>());
            ClassicAssert.AreEqual("en",                                     json["directions"]          [1]["language"].    Value<String>());
            ClassicAssert.AreEqual("Ken sent me!",                           json["directions"]          [1]["text"].        Value<String>());
            ClassicAssert.AreEqual("EV_ONLY",                                json["parking_restrictions"][0].                Value<String>());
            ClassicAssert.AreEqual("PLUGGED",                                json["parking_restrictions"][1].                Value<String>());
            ClassicAssert.AreEqual("http://example.com/pinguine.jpg",        json["images"]              [0]["url"].         Value<String>());
            ClassicAssert.AreEqual("http://example.com/kleine_pinguine.jpg", json["images"]              [0]["thumbnail"].   Value<String>());
            ClassicAssert.AreEqual("OPERATOR",                               json["images"]              [0]["category"].    Value<String>());
            ClassicAssert.AreEqual("jpeg",                                   json["images"]              [0]["type"].        Value<String>());
            ClassicAssert.AreEqual(100,                                      json["images"]              [0]["width"].       Value<UInt16>());
            ClassicAssert.AreEqual(150,                                      json["images"]              [0]["height"].      Value<UInt16>());
            ClassicAssert.AreEqual("http://example.com/wellensittiche.jpg",  json["images"]              [1]["url"].         Value<String>());
            ClassicAssert.AreEqual("2020-09-18T00:00:00.000Z",               json["last_updated"].                           Value<String>());

            if (EVSE.TryParse(json, out var evse2, out var errorResponse))
            {

                ClassicAssert.IsNotNull(evse2);
                ClassicAssert.IsNull   (errorResponse);

                ClassicAssert.AreEqual(EVSE1.UId,                       evse2.UId);
                ClassicAssert.AreEqual(EVSE1.Status,                    evse2.Status);
                ClassicAssert.AreEqual(EVSE1.Connectors,                evse2.Connectors);
                ClassicAssert.AreEqual(EVSE1.EVSEId,                    evse2.EVSEId);
                ClassicAssert.AreEqual(EVSE1.StatusSchedule,            evse2.StatusSchedule);
                ClassicAssert.AreEqual(EVSE1.Capabilities,              evse2.Capabilities);
                ClassicAssert.AreEqual(EVSE1.FloorLevel,                evse2.FloorLevel);
                ClassicAssert.AreEqual(EVSE1.Coordinates,               evse2.Coordinates);
                ClassicAssert.AreEqual(EVSE1.PhysicalReference,         evse2.PhysicalReference);
                ClassicAssert.AreEqual(EVSE1.Directions,                evse2.Directions);
                ClassicAssert.AreEqual(EVSE1.ParkingRestrictions,       evse2.ParkingRestrictions);
                ClassicAssert.AreEqual(EVSE1.Images,                    evse2.Images);
                ClassicAssert.AreEqual(EVSE1.LastUpdated.ToIso8601(),   evse2.LastUpdated.ToIso8601());

            }
            else
                ClassicAssert.Fail("Failed to parse EVSE!");

        }

        #endregion

    }

}
