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

            var EVSE1 = new EVSE(
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
                            } as IEnumerable<Connector>,
                            EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                            new StatusSchedule[] {
                                new StatusSchedule(
                                    StatusTypes.INOPERATIVE,
                                    DateTime.Parse("2020-09-23"),
                                    DateTime.Parse("2020-09-24")
                                )
                            } as IEnumerable<StatusSchedule>,
                            new CapabilityTypes[] {
                                CapabilityTypes.RFID_READER,
                                CapabilityTypes.RESERVABLE
                            } as IEnumerable<CapabilityTypes>,
                            "1. Stock",
                            GeoCoordinate.Parse(10.1, 20.2),
                            "Ladestation #1",
                            DisplayText.Create(Languages.de, "Ken sent me!"),
                            new ParkingRestrictions[] {
                                ParkingRestrictions.EV_ONLY
                            } as IEnumerable<ParkingRestrictions>,
                            new Image[] {
                                new Image(
                                    URL.Parse("http://example.com/pinguine.jpg"),
                                    ImageFileType.jpeg,
                                    ImageCategories.OPERATOR,
                                    100,
                                    150,
                                    URL.Parse("http://example.com/kleine_pinguine.jpg")
                                )
                            } as IEnumerable<Image>,
                            DateTime.Parse("2020-09-22")
                        );

            var JSON = EVSE1.ToJSON();

            //Assert.AreEqual("DE",                           JSON["country_code"].Value<String>());
            //Assert.AreEqual("GEF",                          JSON["party_id"].    Value<String>());
            //Assert.AreEqual("LOC0001",                      JSON["id"].          Value<String>());

            Assert.IsTrue(EVSE.TryParse(JSON, out EVSE EVSE2, out String ErrorResponse));
            Assert.IsNull(ErrorResponse);

            Assert.AreEqual(EVSE1.UId,                      EVSE2.UId);
            Assert.AreEqual(EVSE1.Status,                   EVSE2.Status);
            Assert.AreEqual(EVSE1.Connectors,               EVSE2.Connectors);

            Assert.AreEqual(EVSE1.EVSEId,                   EVSE2.EVSEId);
            //Assert.AreEqual(EVSE1.StatusSchedule,           EVSE2.StatusSchedule);
            Assert.AreEqual(EVSE1.Capabilities,             EVSE2.Capabilities);
            Assert.AreEqual(EVSE1.FloorLevel,               EVSE2.FloorLevel);
            Assert.AreEqual(EVSE1.Coordinates,              EVSE2.Coordinates);
            Assert.AreEqual(EVSE1.PhysicalReference,        EVSE2.PhysicalReference);
            Assert.AreEqual(EVSE1.Directions,               EVSE2.Directions);
            Assert.AreEqual(EVSE1.ParkingRestrictions,      EVSE2.ParkingRestrictions);
            Assert.AreEqual(EVSE1.Images,                   EVSE2.Images);

            Assert.AreEqual(EVSE1.LastUpdated.ToIso8601(),  EVSE2.LastUpdated.ToIso8601());

        }

        #endregion

    }

}
