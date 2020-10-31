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
    /// Unit tests for connector.
    /// https://github.com/ocpi/ocpi/blob/master/mod_locations.asciidoc#mod_locations_connector_object
    /// </summary>
    [TestFixture]
    public static class ConnectorTests
    {

        #region Connector_SerializeDeserialize_Test01()

        /// <summary>
        /// Connector serialize, deserialize and compare test.
        /// </summary>
        [Test]
        public static void Connector_SerializeDeserialize_Test01()
        {

            var ConnectorA = new Connector(
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
                                 DateTime.Parse("2020-09-21T00:00:00Z")
                             );

            var JSON = ConnectorA.ToJSON();

            Assert.AreEqual("1",                                 JSON["id"                  ].   Value<String>());
            Assert.AreEqual("IEC_62196_T2",                      JSON["standard"            ].   Value<String>());
            Assert.AreEqual("SOCKET",                            JSON["format"              ].   Value<String>());
            Assert.AreEqual("AC_3_PHASE",                        JSON["power_type"          ].   Value<String>());
            Assert.AreEqual(400,                                 JSON["max_voltage"         ].   Value<UInt16>());
            Assert.AreEqual(30,                                  JSON["max_amperage"        ].   Value<UInt16>());
            Assert.AreEqual(12,                                  JSON["max_electric_power"  ].   Value<UInt16>());
            Assert.AreEqual("DE*GEF*T0001",                      JSON["tariff_ids"          ][0].Value<String>());
            Assert.AreEqual("DE*GEF*T0002",                      JSON["tariff_ids"          ][1].Value<String>());
            Assert.AreEqual("Public money, public code!",        JSON["terms_and_conditions"].   Value<String>());
            Assert.AreEqual("2020-09-21T00:00:00.000Z",          JSON["last_updated"        ].   Value<String>());

            Assert.IsTrue(Connector.TryParse(JSON, out Connector ConnectorB, out String ErrorResponse));
            Assert.IsNull(ErrorResponse);

            Assert.AreEqual(ConnectorA.Id,                       ConnectorB.Id);
            Assert.AreEqual(ConnectorA.Standard,                 ConnectorB.Standard);
            Assert.AreEqual(ConnectorA.Format,                   ConnectorB.Format);
            Assert.AreEqual(ConnectorA.PowerType,                ConnectorB.PowerType);
            Assert.AreEqual(ConnectorA.MaxVoltage,               ConnectorB.MaxVoltage);
            Assert.AreEqual(ConnectorA.MaxAmperage,              ConnectorB.MaxAmperage);
            Assert.AreEqual(ConnectorA.MaxElectricPower,         ConnectorB.MaxElectricPower);
            Assert.AreEqual(ConnectorA.TariffIds,                ConnectorB.TariffIds);
            Assert.AreEqual(ConnectorA.TermsAndConditions,       ConnectorB.TermsAndConditions);
            Assert.AreEqual(ConnectorA.LastUpdated.ToIso8601(),  ConnectorB.LastUpdated.ToIso8601());

        }

        #endregion

    }

}
