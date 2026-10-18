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

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1.POI3;
using cloud.charging.open.protocols.WWCP.POI.tests;

using POI = cloud.charging.open.protocols.WWCP.POI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests.POI3
{

    /// <summary>
    /// The price status of POI3, new prices of charging tariffs at runtime, as OCPI 2.2.1 tariffs of their own, and back.
    /// </summary>
    [TestFixture]
    public class POI3PriceStatusTests
    {

        #region Data

        private static readonly DateTimeOffset                      Created   = DateTimeOffset.Parse("2026-01-01T00:00:00Z");
        private static readonly DateTimeOffset                      T0        = DateTimeOffset.Parse("2026-10-12T12:00:00Z");

        private static readonly POI.ChargingStationOperator_Id      GEF       = POI.ChargingStationOperator_Id.Parse("DE*GEF");
        private static readonly POI.ChargingPool_Id                 P1        = POI.ChargingPool_Id.           Parse("DE*GEF*P1");
        private static readonly POI.ChargingStation_Id              S1        = POI.ChargingStation_Id.        Parse("DE*GEF*S1");
        private static readonly POI.EVSE_Id                         E1        = POI.EVSE_Id.                   Parse("DE*GEF*E1*1");
        private static readonly POI.EVSE_Id                         E2        = POI.EVSE_Id.                   Parse("DE*GEF*E1*2");
        private static readonly POI.ChargingTariff_Id               T1        = POI.ChargingTariff_Id.         Parse("DE*GEF*T1");

        private static readonly Party_Idv3                          Party     = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));
        private static readonly Location_Id                         Location1 = Location_Id.Parse("1");
        private static readonly Tariff_Id                           Tariff1   = Tariff_Id.  Parse("1");

        private String     directory  = default!;
        private CommonAPI  api        = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void SetUp()
        {
            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_POI3_PriceStatusTests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(directory);
            api        = new CommonAPI(
                             OurPartyData:      [ new PartyData(Party, Role.CPO, new BusinessDetails("GraphDefined CSO")) ],
                             DefaultPartyId:    Party,
                             BaseAPI:           new CommonHTTPAPI(
                                                    HTTPAPI:         new HTTPExtAPI(HTTPServer: new HTTPServer(TCPPort: IPPort.Parse(3977))),
                                                    OurBaseURL:      URL.Parse("http://127.0.0.1:3977/ocpi"),
                                                    OurVersionsURL:  URL.Parse("http://127.0.0.1:3977/ocpi/versions"),
                                                    RootPath:        HTTPPath.Parse("/ocpi"),
                                                    DisableLogging:  true,
                                                    LoggingPath:     directory
                                                ),
                             URLPathPrefix:     HTTPPath.Parse("/ocpi/v2.2.1"),
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

        #region (private static) Elements(Price) / Prices(Price) / ATariff(Price, Name) / ANetwork()

        private static POI.ChargingTariffElement[] Elements(Decimal Price)

            => [ new POI.ChargingTariffElement([ new POI.ChargingPriceComponent(POI.ChargingDimensionTypes.ENERGY, Price, VAT: 19, StepSize: 1) ]) ];

        private static POI.ChargingTariffPrices Prices(Decimal Price)

            => new (T1, Elements(Price));

        private static String PriceStatus(params Decimal[] Prices)

            => new POI.ChargingPrices(Prices.Select(POI3PriceStatusTests.Prices)).Text;

        private static POI.ChargingTariff ATariff(Decimal  Price  = 0.39m,
                                                  String   Name   = "Standard")

            => new (T1,
                    Currency.EUR,
                    Elements(Price),
                    I18NString.Create(Languages.en, Name),
                    Created: Created);

        private static POI.EVSE AnEVSE(POI.EVSE_Id Id)

            => new (Id,
                    CurrentTypes:        POI.CurrentTypes.AC_ThreePhases,
                    MaxVoltage:          Volt.  FromV(400),
                    MaxCurrent:          Ampere.FromA(32),
                    MaxPower:            Watt.  FromW(22000),
                    ChargingConnectors:  [ new POI.ChargingConnector(POI.ChargingConnector_Id.Parse("1"), POI.ChargingConnectorType.IEC_62196_T2) ],
                    ChargingTariffIds:   [ T1 ],
                    Created:             Created);

        private static POI.RoamingNetwork ANetwork()

            => new POI.RoamingNetwork(POI.RoamingNetwork_Id.Parse("rn1"),
                                      Created:                   Created,
                                      ChargingStationOperators:  [ new POI.ChargingStationOperator(GEF,
                                                                                                   Name:             I18NString.Create(Languages.en, "GraphDefined"),
                                                                                                   ChargingTariffs:  [ ATariff() ],
                                                                                                   ChargingPools:    [ new POI.ChargingPool(P1,
                                                                                                                                            I18NString.Create(Languages.en, "Biberweg"),
                                                                                                                                            Address:           new Address("Biberweg", "07749", I18NString.Create(Languages.de, "Jena"), Country.Germany, HouseNumber: "18"),
                                                                                                                                            GeoLocation:       GeoCoordinate.Parse(50.92, 11.58),
                                                                                                                                            TimeZone:          Time_Zone.Parse("Europe/Berlin"),
                                                                                                                                            ChargingStations:  [ new POI.ChargingStation(S1, EVSEs: [ AnEVSE(E1), AnEVSE(E2) ], Created: Created) ],
                                                                                                                                            Created:           Created) ],
                                                                                                   Created:          Created) ]).WithKeys();

        #endregion

        #region (private static) Eventually(Condition) / TariffIdsOf(Location) / PriceOf(Tariff) / PricesOf(PriceStatus)

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

        private static String[] TariffIdsOf(Location Location)

            => [.. Location.EVSEs.OrderBy(evse => evse.UId.ToString()).Select(evse => String.Join(",", evse.Connectors.SelectMany(connector => connector.TariffIds)))];

        private static Decimal PriceOf(Tariff Tariff)

            => Tariff.TariffElements.Single().PriceComponents.Single().Price;

        private static Decimal[] PricesOf(Timestamped<POI.ChargingPrices>? PriceStatus)

            => [.. PriceStatus?.Value.Prices.Select(price => price.Elements.Single().PriceComponents.Single().Price) ?? []];

        #endregion


        #region PricesInEffectAreTariffsOfTheirOwn()

        [Test]
        public void PricesInEffectAreTariffsOfTheirOwn()
        {

            var network          = ANetwork();
            var chargingPool     = network.GetChargingStationOperatorById(GEF)!.ChargingPools.Single();
            var evse2            = chargingPool.ChargingStations.Single().EVSEs.Single(evse => evse.Id == E2);
            var chargingTariff   = network.GetChargingStationOperatorById(GEF)!.ChargingTariffs.Single();
            var warnings         = new List<Warning>();

            // The pool's prices, and other ones of the second EVSE.
            chargingPool.SetPriceStatus(new POI.ChargingPrices([ Prices(0.29m) ]), T0);
            evse2.       SetPriceStatus(new POI.ChargingPrices([ Prices(0.49m) ]), T0.AddMinutes(1));

            var location         = chargingPool.ToOCPI(network, warnings)!;
            var poolTariffId     = T1.ToOCPITariffId(Prices(0.29m))!.Value;
            var evseTariffId     = T1.ToOCPITariffId(Prices(0.49m))!.Value;

            var tariff           = chargingTariff.ToOCPI(warnings)!;
            var poolTariff       = chargingTariff.ToOCPI(warnings, Prices: new Timestamped<POI.ChargingTariffPrices>(T0, Prices(0.29m)))!;

            Assert.Multiple(() => {

                Assert.That(warnings,                                         Is.Empty);

                // Of the tariff 1, the first 8 hex digits of the hash of the prices: the same for the same prices.
                Assert.That(poolTariffId.ToString(),                          Does.Match("^1~[0-9a-f]{8}$"));
                Assert.That(poolTariffId,                                     Is.EqualTo(T1.ToOCPITariffId(Prices(0.29m))));
                Assert.That(evseTariffId,                                     Is.Not.EqualTo(poolTariffId));
                Assert.That(poolTariffId.HasPrices(),                         Is.True);
                Assert.That(Tariff1.HasPrices(),                              Is.False);
                Assert.That(POI3Mapper.ToPOI3TariffIdOf(GEF, poolTariffId),   Is.EqualTo(T1));
                Assert.That(POI3Mapper.ToPOI3TariffIdOf(GEF, Tariff1),        Is.EqualTo(T1));

                // The EVSEs name the tariffs with the prices in effect at them, since then.
                Assert.That(TariffIdsOf(location),                            Is.EqualTo(new[] { poolTariffId.ToString(), evseTariffId.ToString() }));
                Assert.That(location.EVSEs.Single(evse => evse.UId.ToString() == E2.ToString()).LastUpdated,  Is.EqualTo(T0.AddMinutes(1)));
                Assert.That(location.LastUpdated,                             Is.EqualTo(T0.AddMinutes(1)));

                // The tariff as it is, and the one with the prices of the pool.
                Assert.That(tariff.Id,                                        Is.EqualTo(Tariff1));
                Assert.That(PriceOf(tariff),                                  Is.EqualTo(0.39m));
                Assert.That(poolTariff.Id,                                    Is.EqualTo(poolTariffId));
                Assert.That(PriceOf(poolTariff),                              Is.EqualTo(0.29m));
                Assert.That(poolTariff.TariffAltText.Single().Text,           Is.EqualTo("Standard"));
                Assert.That(poolTariff.LastUpdated,                           Is.EqualTo(T0));

            });

        }

        #endregion

        #region ThePublisherPublishesNewPricesAsTariffsOfTheirOwn()

        [Test]
        public async Task ThePublisherPublishesNewPricesAsTariffsOfTheirOwn()
        {

            var history  = POI.RoamingNetworkHistory.Start(ANetwork());
            var errors   = new List<String>();

            await using var publisher = new POI3ToOCPIPublisher(history, api);
            publisher.OnError += (sender, description, warnings) => { lock (errors) errors.Add(description); };

            await publisher.StartAsync();

            Assert.That(api.TryGetLocation(Party, Location1, out var location) ? TariffIdsOf(location) : null,
                        Is.EqualTo(new[] { "1", "1" }));

            // New prices of the charging pool: a tariff of their own, which its EVSEs name.
            var pricedId = T1.ToOCPITariffId(Prices(0.29m))!.Value;

            Assert.That(history.RuntimeStatus.TryUpdate(POI.POIRuntimeStatusKind.PriceStatus, nameof(POI.ChargingPool), P1.ToString(), PriceStatus(0.29m), T0, out _, out var errorResponse),
                        Is.Not.EqualTo(POI.POIStatusUpdateResult.Refused), errorResponse);

            Assert.That(await Eventually(() => api.TryGetLocation(Party, Location1, out var location) &&
                                               TariffIdsOf(location).SequenceEqual([ pricedId.ToString(), pricedId.ToString() ])),
                        Is.True, "The new prices were not published.");

            Assert.Multiple(() => {
                Assert.That(api.TryGetTariff(Party, pricedId, out var pricedTariff) ? PriceOf(pricedTariff) : 0,  Is.EqualTo(0.29m));
                Assert.That(api.TryGetTariff(Party, Tariff1,  out var tariff)       ? PriceOf(tariff)       : 0,  Is.EqualTo(0.39m));
            });

            // The prices of the tariff again: the EVSEs name it, and the tariff of the new prices is gone.
            Assert.That(history.RuntimeStatus.TryUpdate(POI.POIRuntimeStatusKind.PriceStatus, nameof(POI.ChargingPool), P1.ToString(), PriceStatus(), T0.AddMinutes(1), out _, out errorResponse),
                        Is.Not.EqualTo(POI.POIStatusUpdateResult.Refused), errorResponse);

            Assert.That(await Eventually(() => api.TryGetLocation(Party, Location1, out var location) &&
                                               TariffIdsOf(location).SequenceEqual([ "1", "1" ]) &&
                                               !api.TryGetTariff(Party, pricedId, out _)),
                        Is.True, "The prices of the tariff were not published again.");

            lock (errors)
                Assert.That(errors, Is.Empty);

        }

        #endregion

        #region TheImporterTakesNewPricesAsPriceStatus()

        [Test]
        public async Task TheImporterTakesNewPricesAsPriceStatus()
        {

            var network   = ANetwork();
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
            var location  = network.GetChargingStationOperatorById(GEF)!.ChargingPools.Single().ToOCPI(network, warnings)!;

            Assert.That((await api.AddOrUpdateTariff  (ATariff().ToOCPI(warnings)!)).IsSuccess);
            Assert.That((await api.AddOrUpdateLocation(location)).                   IsSuccess);

            POI.ChargingPool  Pool()     => history.Head.RoamingNetwork.GetChargingStationOperatorById(GEF)!.ChargingPools.Single();
            POI.ChargingTariff Tariff()  => history.Head.RoamingNetwork.GetChargingStationOperatorById(GEF)!.ChargingTariffs.Single();

            // What a CPO pushes: only new prices of the tariff.
            var cheaper   = ATariff(0.29m).ToOCPI(warnings, T0)!;
            Assert.That((await api.AddOrUpdateTariff(cheaper)).IsSuccess);
            await importer.ImportAsync(cheaper);

            Assert.Multiple(() => {
                Assert.That(history.Head.Commit.Revision,                                      Is.EqualTo(revision), "New prices only must not be committed.");
                Assert.That(Tariff().Elements.Single().PriceComponents.Single().Price,         Is.EqualTo(0.39m));
                Assert.That(PricesOf(Pool().PriceStatus),                                      Is.EqualTo(new[] { 0.29m }));
                Assert.That(Pool().ChargingStations.Single().EVSEs.Select(evse => evse.PriceStatus.HasValue),  Is.All.False);
            });

            // The prices of the tariff again.
            var again     = ATariff(0.39m).ToOCPI(warnings, T0.AddMinutes(1))!;
            Assert.That((await api.AddOrUpdateTariff(again)).IsSuccess);
            await importer.ImportAsync(again);

            Assert.Multiple(() => {
                Assert.That(history.Head.Commit.Revision,                                      Is.EqualTo(revision));
                Assert.That(PricesOf(Pool().PriceStatus),                                      Is.Empty);
            });

            // A new name with new prices: committed as a whole.
            var renamed   = ATariff(0.35m, "Basic").ToOCPI(warnings, T0.AddMinutes(2))!;
            Assert.That((await api.AddOrUpdateTariff(renamed)).IsSuccess);
            await importer.ImportAsync(renamed);

            Assert.Multiple(() => {
                Assert.That(history.Head.Commit.Revision,                                      Is.EqualTo(revision + 1));
                Assert.That(Tariff().Name?.FirstText(),                                        Is.EqualTo("Basic"));
                Assert.That(Tariff().Elements.Single().PriceComponents.Single().Price,         Is.EqualTo(0.35m));
                Assert.That(PricesOf(Pool().PriceStatus),                                      Is.Empty);
                Assert.That(warnings,                                                          Is.Empty);
            });

            lock (errors)
                Assert.That(errors, Is.Empty);

        }

        #endregion

        #region PublishedPricesImportedBackChangeNothing()

        [Test]
        public async Task PublishedPricesImportedBackChangeNothing()
        {

            // A hub publishing and importing the same history: what it publishes comes back.
            var history   = POI.RoamingNetworkHistory.Start(ANetwork());
            var revision  = history.Head.Commit.Revision;
            var errors    = new List<String>();

            history.RuntimeStatus.TryUpdate(POI.POIRuntimeStatusKind.PriceStatus, nameof(POI.ChargingPool), P1.ToString(), PriceStatus(0.29m), T0,               out _, out _);
            history.RuntimeStatus.TryUpdate(POI.POIRuntimeStatusKind.PriceStatus, nameof(POI.EVSE),         E2.ToString(), PriceStatus(0.49m), T0.AddMinutes(1), out _, out _);

            void OnError(Object Sender, String Description, IEnumerable<Warning> Warnings)
            {
                lock (errors)
                    errors.Add(Description + " " + String.Join(" ", Warnings.Select(warning => warning.Text.FirstText())));
            }

            await using var importer  = new OCPIToPOI3Importer(api, history, changeSet => changeSet.SignedByAll(history.Head.RoamingNetwork));
            importer.OnError += OnError;
            importer.Start();

            await using var publisher = new POI3ToOCPIPublisher(history, api);
            publisher.OnError += OnError;
            await publisher.StartAsync();

            Assert.That(api.TryGetLocation(Party, Location1, out var location), Is.True);

            // After everything the CommonAPI told the importer.
            await importer.ImportAsync(location!);

            POI.ChargingPool Pool() => history.Head.RoamingNetwork.GetChargingStationOperatorById(GEF)!.ChargingPools.Single();

            Assert.Multiple(() => {
                Assert.That(TariffIdsOf(location!),                                            Is.EqualTo(new[] { T1.ToOCPITariffId(Prices(0.29m)).ToString(), T1.ToOCPITariffId(Prices(0.49m)).ToString() }));
                Assert.That(history.Head.Commit.Revision,                                      Is.EqualTo(revision));
                Assert.That(PricesOf(Pool().PriceStatus),                                      Is.EqualTo(new[] { 0.29m }));
                Assert.That(PricesOf(Pool().ChargingStations.Single().EVSEs.Single(evse => evse.Id == E1).PriceStatus),  Is.Empty);
                Assert.That(PricesOf(Pool().ChargingStations.Single().EVSEs.Single(evse => evse.Id == E2).PriceStatus),  Is.EqualTo(new[] { 0.49m }));
                Assert.That(Pool().PriceStatus?.Timestamp,                                     Is.EqualTo(T0), "The price status of the pool was set again.");
            });

            lock (errors)
                Assert.That(errors, Is.Empty);

        }

        #endregion

    }

}
