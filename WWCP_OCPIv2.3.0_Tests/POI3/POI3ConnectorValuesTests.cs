/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0.POI3;

using POI = cloud.charging.open.protocols.WWCP.POI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.POI3
{

    /// <summary>
    /// The current types, maximum voltage, current and power and the tariffs of OCPI connectors: of the POI3 charging
    /// connector, else of its EVSE, e.g. a CHAdeMO connector of 50 kW beside a CCS one of 150 kW.
    /// </summary>
    [TestFixture]
    public class POI3ConnectorValuesTests
    {

        #region Data

        private static readonly DateTimeOffset                  Created  = DateTimeOffset.Parse("2026-01-01T00:00:00Z");

        private static readonly POI.ChargingStationOperator_Id  GEF      = POI.ChargingStationOperator_Id.Parse("DE*GEF");
        private static readonly POI.ChargingTariff_Id           Fast     = POI.ChargingTariff_Id.         Parse("DE*GEF*TFAST");
        private static readonly POI.ChargingTariff_Id           Normal   = POI.ChargingTariff_Id.         Parse("DE*GEF*TNORMAL");
        private static readonly POI.EVSE_Id                     E1       = POI.EVSE_Id.                   Parse("DE*GEF*E1*1");

        #endregion

        #region (private static) ATariff(Id) / ANetwork(Connectors, CurrentTypes)

        private static POI.ChargingTariff ATariff(POI.ChargingTariff_Id Id)

            => new (Id,
                    Currency.EUR,
                    [ new POI.ChargingTariffElement([ new POI.ChargingPriceComponent(POI.ChargingDimensionTypes.ENERGY, 0.59m, VAT: 19, StepSize: 1) ]) ],
                    Created: Created);

        /// <summary>
        /// A CCS cable as its EVSE is, DC of 150 kW with the fast tariff, and a CHAdeMO one of 50 kW, 500 V and 125 A with
        /// the normal tariff.
        /// </summary>
        private static readonly POI.ChargingConnector[] CCSAndCHAdeMO = [
            new (POI.ChargingConnector_Id.Parse("1"), POI.ChargingConnectorType.IEC_62196_T2_COMBO, Format: POI.ChargingConnectorFormat.CABLE),
            new (POI.ChargingConnector_Id.Parse("2"), POI.ChargingConnectorType.CHAdeMO,            Format: POI.ChargingConnectorFormat.CABLE,
                 MaxVoltage:         Volt.  FromV(500),
                 MaxCurrent:         Ampere.FromA(125),
                 MaxPower:           Watt.  FromW(50000),
                 ChargingTariffIds:  [ Normal ])
        ];

        private static POI.RoamingNetwork ANetwork(IEnumerable<POI.ChargingConnector>  Connectors,
                                                   POI.CurrentTypes                    CurrentTypes = POI.CurrentTypes.DC)

            => new (POI.RoamingNetwork_Id.Parse("rn1"),
                    Created:                   Created,
                    ChargingStationOperators:  [ new POI.ChargingStationOperator(GEF,
                                                                                 Created:          Created,
                                                                                 ChargingTariffs:  [ ATariff(Fast), ATariff(Normal) ],
                                                                                 ChargingPools:    [ new POI.ChargingPool(POI.ChargingPool_Id.Parse("DE*GEF*P1"),
                                                                                                                          Created:           Created,
                                                                                                                          ChargingStations:  [ new POI.ChargingStation(POI.ChargingStation_Id.Parse("DE*GEF*S1"),
                                                                                                                                                                       Created:  Created,
                                                                                                                                                                       EVSEs:    [ new POI.EVSE(E1,
                                                                                                                                                                                                CurrentTypes:        CurrentTypes,
                                                                                                                                                                                                MaxVoltage:          Volt.  FromV(920),
                                                                                                                                                                                                MaxCurrent:          Ampere.FromA(200),
                                                                                                                                                                                                MaxPower:            Watt.  FromW(150000),
                                                                                                                                                                                                ChargingTariffIds:   [ Fast ],
                                                                                                                                                                                                ChargingConnectors:  Connectors,
                                                                                                                                                                                                Created:             Created) ]) ]) ]) ]);

        private static EVSE ToOCPI(POI.RoamingNetwork Network, ICollection<Warning> Warnings)
        {
            var cso      = Network.GetChargingStationOperatorById(GEF)!;
            var pool     = cso.ChargingPools.Single();
            var station  = pool.ChargingStations.Single();
            return station.EVSEs.Single().ToOCPI(station, pool, cso, Warnings)!;
        }

        #endregion


        #region EachConnectorTellsItsOwnValuesElseThoseOfItsEVSE()

        [Test]
        public void EachConnectorTellsItsOwnValuesElseThoseOfItsEVSE()
        {

            var warnings  = new List<Warning>();
            var evse      = ToOCPI(ANetwork(CCSAndCHAdeMO), warnings);
            var ccs       = evse.Connectors.Single(connector => connector.Id.ToString() == "1");
            var chademo   = evse.Connectors.Single(connector => connector.Id.ToString() == "2");

            Assert.Multiple(() => {

                Assert.That(warnings,                                                    Is.Empty);

                Assert.That(ccs.PowerType,                                               Is.EqualTo(PowerTypes.DC));
                Assert.That(ccs.MaxVoltage.Value,                                        Is.EqualTo(920));
                Assert.That(ccs.MaxAmperage.Value,                                       Is.EqualTo(200));
                Assert.That(ccs.MaxElectricPower?.Value,                                 Is.EqualTo(150000));
                Assert.That(ccs.TariffIds.Select(tariffId => tariffId.ToString()),        Is.EqualTo(new[] { "FAST" }));

                Assert.That(chademo.PowerType,                                           Is.EqualTo(PowerTypes.DC));
                Assert.That(chademo.MaxVoltage.Value,                                    Is.EqualTo(500));
                Assert.That(chademo.MaxAmperage.Value,                                   Is.EqualTo(125));
                Assert.That(chademo.MaxElectricPower?.Value,                             Is.EqualTo(50000));
                Assert.That(chademo.TariffIds.Select(tariffId => tariffId.ToString()),    Is.EqualTo(new[] { "NORMAL" }));

            });

        }

        #endregion

        #region WhatAConnectorTellsOtherThanItsEVSEIsItsOwn()

        [Test]
        public void WhatAConnectorTellsOtherThanItsEVSEIsItsOwn()
        {

            var network   = ANetwork(CCSAndCHAdeMO);
            var warnings  = new List<Warning>();
            var imported  = ToOCPI(network, warnings).ToPOI3(GEF, Created, warnings)!;
            var original  = network.GetChargingStationOperatorById(GEF)!.EVSEs.Single();

            Assert.Multiple(() => {

                Assert.That(warnings,                                                    Is.Empty);

                // The highest limits, the current types of all, and the tariffs most connectors have: the CCS one's,
                // the first of the two.
                Assert.That(imported.MaxVoltage?.Value,                                  Is.EqualTo(920));
                Assert.That(imported.MaxCurrent?.Value,                                  Is.EqualTo(200));
                Assert.That(imported.MaxPower?.Value,                                    Is.EqualTo(150000));
                Assert.That(imported.CurrentTypes,                                       Is.EqualTo(POI.CurrentTypes.DC));
                Assert.That(imported.ChargingTariffIds,                                  Is.EqualTo(new[] { Fast }));

                // So the connectors are as they were.
                Assert.That(JToken.DeepEquals(new JArray(imported.ChargingConnectors.Select(connector => connector.ToJSON())),
                                              new JArray(original.ChargingConnectors.Select(connector => connector.ToJSON()))),
                            Is.True);

            });

        }

        #endregion

        #region ConnectorsOfACAndDCAreOfTheirOwnCurrentTypes()

        [Test]
        public void ConnectorsOfACAndDCAreOfTheirOwnCurrentTypes()
        {

            // An EVSE of both: a Type 2 cable of AC, 22 kW, and a CCS connector of DC.
            var network   = ANetwork([ new (POI.ChargingConnector_Id.Parse("1"), POI.ChargingConnectorType.IEC_62196_T2, Format: POI.ChargingConnectorFormat.CABLE,
                                            CurrentTypes: POI.CurrentTypes.AC_ThreePhases, MaxVoltage: Volt.FromV(400), MaxCurrent: Ampere.FromA(32), MaxPower: Watt.FromW(22000)),
                                       new (POI.ChargingConnector_Id.Parse("2"), POI.ChargingConnectorType.IEC_62196_T2_COMBO,
                                            CurrentTypes: POI.CurrentTypes.DC) ],
                                     POI.CurrentTypes.AC_ThreePhases | POI.CurrentTypes.DC);

            var warnings  = new List<Warning>();
            var evse      = ToOCPI(network, warnings);
            var imported  = evse.ToPOI3(GEF, Created, warnings)!;

            Assert.Multiple(() => {

                Assert.That(evse.Connectors.Select(connector => connector.PowerType),    Is.EqualTo(new[] { PowerTypes.AC_3_PHASE, PowerTypes.DC }));
                Assert.That(evse.Connectors.First().MaxElectricPower?.Value,              Is.EqualTo(22000));

                Assert.That(imported.CurrentTypes,                                        Is.EqualTo(POI.CurrentTypes.AC_ThreePhases | POI.CurrentTypes.DC));
                Assert.That(imported.ChargingConnectors.Select(connector => connector.CurrentTypes),
                            Is.EqualTo(new POI.CurrentTypes?[] { POI.CurrentTypes.AC_ThreePhases, POI.CurrentTypes.DC }));
                Assert.That(imported.ChargingConnectors.Last().MaxPower,                  Is.Null, "The DC one has the 150 kW of the EVSE.");
                Assert.That(warnings,                                                     Is.Empty);

            });

        }

        #endregion

    }

}
