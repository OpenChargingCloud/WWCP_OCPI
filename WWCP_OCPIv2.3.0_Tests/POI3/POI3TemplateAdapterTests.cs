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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0.POI3;
using cloud.charging.open.protocols.WWCP.POI.tests;

using POI = cloud.charging.open.protocols.WWCP.POI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.POI3
{

    /// <summary>
    /// A POI3 charging pool whose address and facilities, a charging station whose parking space, and a parking space whose
    /// roof and length only their templates give: the OCPI location has them, as they are at runtime, and imported back
    /// they stay in their templates.
    /// </summary>
    [TestFixture]
    public class POI3TemplateAdapterTests
    {

        #region Data

        private static readonly DateTimeOffset                  Created  = DateTimeOffset.Parse("2026-01-01T00:00:00Z");
        private static readonly DateTimeOffset                  T0       = DateTimeOffset.Parse("2026-10-17T12:00:00Z");

        private static readonly POI.ChargingStationOperator_Id  GEF      = POI.ChargingStationOperator_Id.Parse("DE*GEF");
        private static readonly POI.ChargingPool_Id             P1       = POI.ChargingPool_Id.           Parse("DE*GEF*P1");
        private static readonly POI.ChargingStation_Id          S1       = POI.ChargingStation_Id.        Parse("DE*GEF*S1");
        private static readonly POI.ParkingSpace_Id             Space1   = POI.ParkingSpace_Id.           Parse("PAR-1");
        private static readonly POI.ParkingSpace_Id             Space2   = POI.ParkingSpace_Id.           Parse("PAR-2");

        private static readonly Party_Idv3                      Party    = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));

        private String     directory  = default!;
        private CommonAPI  api        = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void SetUp()
        {
            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_POI3_TemplateAdapterTests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(directory);
            api        = new CommonAPI(
                             OurPartyData:      [ new PartyData(Party, Role.CPO, new BusinessDetails("GraphDefined CSO")) ],
                             DefaultPartyId:    Party,
                             BaseAPI:           new CommonHTTPAPI(
                                                    HTTPAPI:         new HTTPExtAPI(HTTPServer: new HTTPServer(TCPPort: IPPort.Parse(3996))),
                                                    OurBaseURL:      URL.Parse("http://127.0.0.1:3996/ocpi"),
                                                    OurVersionsURL:  URL.Parse("http://127.0.0.1:3996/ocpi/versions"),
                                                    RootPath:        HTTPPath.Parse("/ocpi"),
                                                    DisableLogging:  true,
                                                    LoggingPath:     directory
                                                ),
                             URLPathPrefix:     HTTPPath.Parse("/ocpi/v2.3.0"),
                             DatabaseFilePath:  directory,
                             DisableLogging:    true,
                             LoggingPath:       directory
                         );
        }

        [TearDown]
        public async Task TearDown()
        {

            await api.BaseAPI.DisposeAsync();

            try
            {
                Directory.Delete(directory, true);
            }
            catch (IOException)
            { }
            catch (UnauthorizedAccessException)
            { }

        }

        #endregion

        #region NewNetwork(StationTemplate = false, PoolFacilities = null)

        /// <summary>
        /// A charging pool whose address and the facility CAFE its template gives, and a parking space whose roof and length
        /// its template gives; with a station template, the charging station is given a second parking space by it.
        /// </summary>
        private static POI.RoamingNetwork NewNetwork(Boolean                     StationTemplate  = false,
                                                     IEnumerable<POI.Facility>?  PoolFacilities   = null)

            => new POI.RoamingNetwork(POI.RoamingNetwork_Id.Parse("rn1"),
                   Created:                   Created,
                   ChargingStationOperators:  [ new POI.ChargingStationOperator(GEF,
                                                    Created:                Created,
                                                    ChargingPools:          [ new POI.ChargingPool(P1,
                                                                                                   GeoLocation:       GeoCoordinate.Parse(50.92, 11.58),
                                                                                                   Facilities:        PoolFacilities,
                                                                                                   TemplateIds:       [ POI.ChargingPoolTemplate_Id.Parse("DE*GEF*PT1") ],
                                                                                                   Created:           Created,
                                                                                                   ChargingStations:  [ new POI.ChargingStation(S1,
                                                                                                                                                ParkingSpaceIds:  [ Space1 ],
                                                                                                                                                TemplateIds:      StationTemplate ? [ POI.ChargingStationTemplate_Id.Parse("DE*GEF*ST1") ] : null,
                                                                                                                                                Created:          Created,
                                                                                                                                                EVSEs:            [ new POI.EVSE(POI.EVSE_Id.Parse("DE*GEF*E1*1"),
                                                                                                                                                                                 CurrentTypes:        POI.CurrentTypes.AC_ThreePhases,
                                                                                                                                                                                 ChargingConnectors:  [ new POI.ChargingConnector(POI.ChargingConnector_Id.Parse("1"), POI.ChargingConnectorType.IEC_62196_T2) ],
                                                                                                                                                                                 Created:             Created) ]) ]) ],
                                                    ChargingPoolTemplates:  [ new POI.ChargingPoolTemplate(POI.ChargingPoolTemplate_Id.Parse("DE*GEF*PT1"),
                                                                                                        Address:          new Address("Biberweg", "07749", I18NString.Create(Languages.de, "Jena"), Country.Germany, HouseNumber: "18"),
                                                                                                        Facilities:       [ POI.Facility.CAFE ],
                                                                                                        Created:          Created) ],
                                                    ChargingStationTemplates:  StationTemplate
                                                                                ? [ new POI.ChargingStationTemplate(POI.ChargingStationTemplate_Id.Parse("DE*GEF*ST1"),
                                                                                                                 ParkingSpaceIds:     [ Space2 ],
                                                                                                                 Created:             Created) ]
                                                                                : null) ],
                   ParkingOperators:          [ new POI.ParkingOperator(POI.ParkingOperator_Id.Parse("DE*PAR"),
                                                    Created:             Created,
                                                    ParkingSpaces:          [ new POI.ParkingSpace(Space1, TemplateIds: [ POI.ParkingSpaceTemplate_Id.Parse("level-1") ], Created: Created),
                                                                              new POI.ParkingSpace(Space2, Created: Created) ],
                                                    ParkingSpaceTemplates:  [ new POI.ParkingSpaceTemplate(POI.ParkingSpaceTemplate_Id.Parse("level-1"),
                                                                                                     Roofed:           true,
                                                                                                     Length:           Meter.From_m(5),
                                                                                                     Created:          Created) ]) ]).WithKeys();

        #endregion

        #region (private static) Eventually(Condition)

        private static async Task<Boolean> Eventually(Func<Boolean> Condition)
        {

            for (var i = 0; i < 200; i++)
            {
                if (Condition())
                    return true;
                await Task.Delay(25);
            }

            return Condition();

        }

        #endregion


        #region WhatTemplatesGiveIsInTheLocation()

        [Test]
        public void WhatTemplatesGiveIsInTheLocation()
        {

            var network   = NewNetwork();
            var gef       = network.GetChargingStationOperatorById(GEF)!;
            var warnings  = new List<Warning>();

            // As it is given, without the address its template gives it, it is no location.
            var stored    = gef.ChargingPools.Single().ToOCPI(network, warnings);

            // As it is at runtime, as the publisher publishes it.
            var location  = gef.EffectiveChargingPools.Single().ToOCPI(network, warnings);

            Assert.Multiple(() => {
                Assert.That(stored,                                                 Is.Null);
                Assert.That(location,                                               Is.Not.Null, String.Join(", ", warnings.Select(warning => warning.Text.FirstText())));
                Assert.That(location!.Address,                                      Does.Contain("Biberweg"));
                Assert.That(location.Facilities.Select(facility => facility.ToString()),  Is.EqualTo(new[] { "CAFE" }));
                Assert.That(location.ParkingPlaces.Single().Id.ToString(),          Is.EqualTo("PAR-1"));
                Assert.That(location.ParkingPlaces.Single().Roofed,                 Is.True);
                Assert.That(location.ParkingPlaces.Single().ParkingSpaceLength?.m,  Is.EqualTo(5));
            });

        }

        #endregion

        #region WhatTemplatesGiveStaysInTheTemplatesOnImport()

        /// <summary>
        /// Imported back as it is published, the location commits nothing, though the parking of its EVSE names the
        /// parking space the template of its charging station gives it. With a facility added at OCPI, the pool takes only
        /// that one as its own: the one of its template stays in its template, and goes with it.
        /// </summary>
        [Test]
        public async Task WhatTemplatesGiveStaysInTheTemplatesOnImport()
        {

            var network   = NewNetwork(StationTemplate: true);
            var history   = POI.RoamingNetworkHistory.Start(network);
            var revision  = history.Head.Commit.Revision;
            var errors    = new List<String>();

            await using var importer = new OCPIToPOI3Importer(api, history, changeSet => changeSet.SignedByAll(history.Head.RoamingNetwork));
            importer.OnError += (sender, description, warnings) => {
                lock (errors)
                    errors.Add(description + " " + String.Join(" ", warnings.Select(warning => warning.Text.FirstText())));
            };

            importer.Start();

            var warnings  = new List<Warning>();
            var published = network.GetChargingStationOperatorById(GEF)!.EffectiveChargingPools.Single().ToOCPI(network, warnings)!;

            Assert.That(published.EVSEs.Single().Parking.Select(parking => parking.ParkingId.ToString()),  Is.EquivalentTo(new[] { "PAR-1", "PAR-2" }));

            await importer.ImportAsync(published);

            Assert.That(history.Head.Commit.Revision, Is.EqualTo(revision), "Imported back as published, nothing is to be committed. " + String.Join("\n", errors));

            // What a CPO pushes: a facility of the pool's own added.
            var source    = NewNetwork(StationTemplate: true, PoolFacilities: [ POI.Facility.SUPERMARKET ]);
            var added     = source.GetChargingStationOperatorById(GEF)!.EffectiveChargingPools.Single().ToOCPI(source, warnings, T0)!;

            await importer.ImportAsync(added);

            POI.ChargingStationOperator GEFOf() => history.Head.RoamingNetwork.GetChargingStationOperatorById(GEF)!;

            Assert.That(await Eventually(() => GEFOf().ChargingPools.Single().Facilities.Any()), Is.True, String.Join("\n", errors));

            Assert.Multiple(() => {
                Assert.That(GEFOf().ChargingPools.Single().Facilities,                    Is.EqualTo(new[] { POI.Facility.SUPERMARKET }));
                Assert.That(GEFOf().EffectiveChargingPools.Single().Facilities,           Is.EquivalentTo(new[] { POI.Facility.SUPERMARKET, POI.Facility.CAFE }));
                Assert.That(GEFOf().ChargingPools.Single().Address,                       Is.Null);
                Assert.That(GEFOf().ChargingStations.Single().ParkingSpaceIds,            Is.EqualTo(new[] { Space1 }));
            });

            lock (errors)
                Assert.That(errors, Is.Empty);

        }

        #endregion

        #region ARemovedEVSEMayHaveTheEVSEIdOfItsSuccessor()

        /// <summary>
        /// A CPO replaces an EVSE: the old one stays REMOVED, with the EVSE ID the new one has now, as OCPI allows. The
        /// location is imported, here with its new name, and the REMOVED one is gone.
        /// </summary>
        [Test]
        public async Task ARemovedEVSEMayHaveTheEVSEIdOfItsSuccessor()
        {

            var network    = NewNetwork();
            var history    = POI.RoamingNetworkHistory.Start(network);
            var revision   = history.Head.Commit.Revision;

            await using var importer = new OCPIToPOI3Importer(api, history, changeSet => changeSet.SignedByAll(history.Head.RoamingNetwork));

            importer.Start();

            var published  = network.GetChargingStationOperatorById(GEF)!.EffectiveChargingPools.Single().ToOCPI(network, [], T0)!;
            var evse       = published.EVSEs.Single();
            var json       = published.ToJSON();

            json["name"]   = "Biberweg Nord";
            ((JArray) json["evses"]!).Add(new EVSE(EVSE_UId.Parse("old"), StatusType.REMOVED, evse.Connectors, evse.EVSEId, LastUpdated: T0).ToJSON());

            var location   = Location.Parse(json);

            Assert.That(location.EVSEs.Count(), Is.EqualTo(2));

            Assert.That(async () => await importer.ImportAsync(location), Throws.Nothing);

            var pool       = history.Head.RoamingNetwork.GetChargingStationOperatorById(GEF)!.ChargingPools.Single();

            Assert.Multiple(() => {
                Assert.That(history.Head.Commit.Revision,                     Is.EqualTo(revision + 1));
                Assert.That(pool.Name.FirstText(),                            Is.EqualTo("Biberweg Nord"));
                Assert.That(pool.EVSEs.Select(evse => evse.Id.ToString()),    Is.EqualTo(new[] { "DE*GEF*E1*1" }));
                Assert.That(pool.ChargingStations.Single().ParkingSpaceIds,   Is.EqualTo(new[] { Space1 }));
            });

        }

        #endregion

    }

}
