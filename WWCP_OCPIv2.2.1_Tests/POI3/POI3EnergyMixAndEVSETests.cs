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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1.POI3;

using POI = cloud.charging.open.protocols.WWCP.POI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.POI3
{

    /// <summary>
    /// Environmental impacts of energy mixes, and the floor level and the accepted e-mobility providers of EVSEs,
    /// between POI3 and OCPI 2.2.1, which has no accepted service providers.
    /// </summary>
    [TestFixture]
    public class POI3EnergyMixAndEVSETests
    {

        #region Data

        private static readonly DateTimeOffset                  Created  = DateTimeOffset.Parse("2026-01-01T00:00:00Z");

        private static readonly POI.ChargingStationOperator_Id  GEF      = POI.ChargingStationOperator_Id.Parse("DE*GEF");
        private static readonly POI.ChargingTariff_Id           T1       = POI.ChargingTariff_Id.         Parse("DE*GEF*T1");
        private static readonly POI.EVSE_Id                     E1       = POI.EVSE_Id.                   Parse("DE*GEF*E1*1");

        #endregion


        #region EnvironmentalImpactsMapBothWaysInGramsPerKWh()

        [Test]
        public void EnvironmentalImpactsMapBothWaysInGramsPerKWh()
        {

            var tariff    = new POI.ChargingTariff(T1,
                                                   Currency.EUR,
                                                   [ new POI.ChargingTariffElement([ new POI.ChargingPriceComponent(POI.ChargingDimensionTypes.ENERGY, 0.39m, VAT: 19, StepSize: 1) ]) ],
                                                   EnergyMix:  new POI.EnergyMix([ new PercentageOf<POI.EnergySourceCategory>(POI.EnergySourceCategory.Wind, 100) ],
                                                                                 [ new POI.EnvironmentalImpactAmount(POI.EnvironmentalImpact.CarbonDioxide, 12.5m),
                                                                                   new POI.EnvironmentalImpactAmount(POI.EnvironmentalImpact.NuclearWaste,  0),
                                                                                   new POI.EnvironmentalImpactAmount(POI.EnvironmentalImpact.Parse("Methane"), 3) ],
                                                                                 ImmutableI18NString.Create(Languages.en, "Supplier"),
                                                                                 ImmutableI18NString.Create(Languages.en, "Wind")),
                                                   Created:    Created);

            var warnings  = new List<Warning>();
            var ocpi      = tariff.ToOCPI(warnings)!;
            var impacts   = ocpi.EnergyMix!.EnvironmentalImpacts.ToArray();

            // OCPI tells what an EMSP sends back: an impact of its own is refused, a negative one as well.
            var received  = new Tariff(ocpi.CountryCode,
                                       ocpi.PartyId,
                                       ocpi.Id,
                                       ocpi.Currency,
                                       ocpi.TariffElements,
                                       EnergyMix:    new EnergyMix(true,
                                                                   ocpi.EnergyMix.EnergySources,
                                                                   [ .. impacts, new EnvironmentalImpact(EnvironmentalImpactCategory.Parse("METHANE"), 1) ],
                                                                   ocpi.EnergyMix.SupplierName,
                                                                   ocpi.EnergyMix.EnergyProductName),
                                       Created:      ocpi.Created,
                                       LastUpdated:  ocpi.LastUpdated);

            var back      = new List<Warning>();
            var imported  = received.ToPOI3(GEF, Languages.en, back)!;

            Assert.Multiple(() => {

                Assert.That(impacts.Select(impact => (impact.Category.ToString(), impact.Amount)),
                            Is.EqualTo(new[] { ("CARBON_DIOXIDE", 12.5), ("NUCLEAR_WASTE", 0.0) }));
                Assert.That(warnings.Single().Text.FirstText(),                     Does.Contain("'Methane'"));

                Assert.That(imported.EnergyMix!.EnvironmentalImpacts.Select(impact => (impact.Category, impact.GramsPerKWh)),
                            Is.EqualTo(new[] { (POI.EnvironmentalImpact.CarbonDioxide, 12.5m), (POI.EnvironmentalImpact.NuclearWaste, 0m) }));
                Assert.That(back.Single().Text.FirstText(),                         Does.Contain("'METHANE'"));

            });

        }

        #endregion

        #region FloorLevelMapsBothWays()

        [Test]
        public void FloorLevelMapsBothWays()
        {

            var network   = new POI.RoamingNetwork(POI.RoamingNetwork_Id.Parse("rn1"),
                                                   Created:                   Created,
                                                   ChargingStationOperators:  [ new POI.ChargingStationOperator(GEF,
                                                                                                                Created:        Created,
                                                                                                                ChargingPools:  [ new POI.ChargingPool(POI.ChargingPool_Id.Parse("DE*GEF*P1"),
                                                                                                                                                       Created:           Created,
                                                                                                                                                       ChargingStations:  [ new POI.ChargingStation(POI.ChargingStation_Id.Parse("DE*GEF*S1"),
                                                                                                                                                                                                    Created:  Created,
                                                                                                                                                                                                    EVSEs:    [ new POI.EVSE(E1,
                                                                                                                                                                                                                             ChargingConnectors:            [ new POI.ChargingConnector(POI.ChargingConnector_Id.Parse("1"), POI.ChargingConnectorType.IEC_62196_T2) ],
                                                                                                                                                                                                                             FloorLevel:                    "-1",
                                                                                                                                                                                                                             AcceptedEMobilityProviderIds:  [ POI.EMobilityProvider_Id.Parse("DE*GDF"), POI.EMobilityProvider_Id.Parse("DE-ABC") ],
                                                                                                                                                                                                                             Created:                       Created) ]) ]) ]) ]);

            var cso       = network.GetChargingStationOperatorById(GEF)!;
            var pool      = cso.ChargingPools.Single();
            var station   = pool.ChargingStations.Single();
            var warnings  = new List<Warning>();
            var ocpi      = station.EVSEs.Single().ToOCPI(station, pool, cso, warnings)!;

            // As an EMSP sends it back.
            var received  = new EVSE(ocpi.UId,
                                     ocpi.Status,
                                     ocpi.Connectors,
                                     ocpi.EVSEId,
                                     FloorLevel:   ocpi.FloorLevel,
                                     LastUpdated:  ocpi.LastUpdated);

            var back      = new List<Warning>();
            var imported  = received.ToPOI3(GEF, Created, back)!;

            Assert.Multiple(() => {

                Assert.That(warnings,                                                         Is.Empty);
                Assert.That(ocpi.FloorLevel,                                                  Is.EqualTo("-1"));

                Assert.That(imported.FloorLevel,                                              Is.EqualTo("-1"));
                Assert.That(imported.AcceptedEMobilityProviderIds,                            Is.Empty, "OCPI 2.2.1 tells none.");
                Assert.That(back,                                                             Is.Empty);

            });

        }

        #endregion

    }

}
