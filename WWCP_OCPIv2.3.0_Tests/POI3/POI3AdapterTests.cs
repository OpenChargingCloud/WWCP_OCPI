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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0.POI3;
using cloud.charging.open.protocols.WWCP.POI.tests;

using POI = cloud.charging.open.protocols.WWCP.POI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.UnitTests.POI3
{

    /// <summary>
    /// A POI3 roaming network as OCPI 2.3.0 locations and tariffs, and back.
    /// </summary>
    [TestFixture]
    public class POI3AdapterTests
    {

        #region Data

        private static readonly DateTimeOffset                      Created   = DateTimeOffset.Parse("2026-01-01T00:00:00Z");
        private static readonly DateTimeOffset                      T0        = DateTimeOffset.Parse("2026-10-12T12:00:00Z");

        private static readonly POI.ChargingStationOperator_Id      GEF       = POI.ChargingStationOperator_Id.Parse("DE*GEF");
        private static readonly POI.ChargingPool_Id                 P1        = POI.ChargingPool_Id.           Parse("DE*GEF*P1");
        private static readonly POI.ChargingStation_Id              S1        = POI.ChargingStation_Id.        Parse("DE*GEF*S1");
        private static readonly POI.EVSE_Id                         E1        = POI.EVSE_Id.                   Parse("DE*GEF*E1*1");
        private static readonly POI.ChargingTariff_Id               T1        = POI.ChargingTariff_Id.         Parse("DE*GEF*T1");

        private static readonly Party_Idv3                          Party     = Party_Idv3.From(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));

        private String     directory  = default!;
        private CommonAPI  api        = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void SetUp()
        {
            directory  = Path.Combine(Path.GetTempPath(), $"WWCP_OCPI_POI3_Tests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(directory);
            api        = ACommonAPI();
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

        #region (private) ACommonAPI()

        private CommonAPI ACommonAPI()

            => new (
                   OurPartyData:      [ new PartyData(Party, Role.CPO, new BusinessDetails("GraphDefined CSO")) ],
                   DefaultPartyId:    Party,
                   BaseAPI:           new CommonHTTPAPI(
                                          HTTPAPI:         new HTTPExtAPI(HTTPServer: new HTTPServer(TCPPort: IPPort.Parse(3999))),
                                          OurBaseURL:      URL.Parse("http://127.0.0.1:3999/ocpi"),
                                          OurVersionsURL:  URL.Parse("http://127.0.0.1:3999/ocpi/versions"),
                                          RootPath:        HTTPPath.Parse("/ocpi"),
                                          DisableLogging:  true,
                                          LoggingPath:     directory
                                      ),
                   URLPathPrefix:     HTTPPath.Parse("/ocpi/v2.3.0"),
                   DatabaseFilePath:  directory,
                   DisableLogging:    true,
                   LoggingPath:       directory
               );

        #endregion

        #region (private static) ATariff() / AnEVSE(MaxPower) / APool(MaxPower, EnergyMix) / ANetwork(WithPools)

        private static POI.ChargingTariff ATariff()

            => new (T1,
                    Currency.EUR,
                    [
                        new POI.ChargingTariffElement([ new POI.ChargingPriceComponent(POI.ChargingDimensionTypes.ENERGY, 0.39m, VAT: 19, StepSize: 1) ]),
                        new POI.ChargingTariffElement([ new POI.ChargingPriceComponent(POI.ChargingDimensionTypes.PARKING_TIME, 6, StepSize: 60),
                                                        new POI.ChargingPriceComponent(POI.ChargingDimensionTypes.FLAT,         1) ],
                                                      new POI.ChargingTariffRestrictions(StartTime:   new TimeOnly(22, 0),
                                                                                         EndTime:     new TimeOnly(6, 30),
                                                                                         MinEnergy:   5000,
                                                                                         MinPower:    3700,
                                                                                         MaxDuration: 3600,
                                                                                         DaysOfWeek:  [ DayOfWeek.Sunday ]))
                    ],
                    I18NString.Create(Languages.en, "Standard"),
                    Created: Created);

        private static POI.EVSE AnEVSE(Int32 MaxPower = 22000)

            => new (E1,
                    CurrentTypes:        POI.CurrentTypes.AC_ThreePhases,
                    MaxVoltage:          Volt.  FromV(400),
                    MaxCurrent:          Ampere.FromA(32),
                    MaxPower:            Watt.  FromW(MaxPower),
                    ChargingConnectors:  [ new POI.ChargingConnector(POI.ChargingConnector_Id.Parse("1"), POI.ChargingConnectorType.IEC_62196_T2) ],
                    ChargingTariffIds:   [ T1 ],
                    Created:             Created);

        private static POI.ChargingPool APool(Int32           MaxPower    = 22000,
                                              POI.EnergyMix?  EnergyMix   = null)

            => new (P1,
                    I18NString.Create(Languages.en, "Biberweg"),
                    Address:              new Address("Biberweg", "07749", I18NString.Create(Languages.de, "Jena"), Country.Germany, HouseNumber: "18"),
                    GeoLocation:          GeoCoordinate.Parse(50.92, 11.58),
                    ParkingType:          POI.ParkingType.PARKING_LOT,
                    TimeZone:             Time_Zone.Parse("Europe/Berlin"),
                    Facilities:           [ POI.Facility.CAFE ],
                    EnergyMix:            EnergyMix,
                    AuthenticationModes:  [ POI.AuthenticationMode.RFID(), POI.AuthenticationMode.REMOTE ],
                    ChargingStations:     [ new POI.ChargingStation(S1,
                                                                    Features:  [ POI.ChargingStationFeature.Reservable ],
                                                                    EVSEs:     [ AnEVSE(MaxPower) ],
                                                                    Created:   Created) ],
                    Created:              Created);

        private static POI.RoamingNetwork ANetwork(Boolean WithPools = true)

            => new POI.RoamingNetwork(POI.RoamingNetwork_Id.Parse("rn1"),
                                      Created:                   Created,
                                      ChargingStationOperators:  [ new POI.ChargingStationOperator(GEF,
                                                                                                   Name:            I18NString.Create(Languages.en, "GraphDefined"),
                                                                                                   Homepage:        URL.Parse("https://graphdefined.com"),
                                                                                                   ChargingTariffs: WithPools ? [ ATariff() ] : null,
                                                                                                   ChargingPools:   WithPools ? [ APool()   ] : null,
                                                                                                   Created:         Created) ]).WithKeys();

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


        #region APoolBecomesALocation()

        [Test]
        public void APoolBecomesALocation()
        {

            var network   = ANetwork();
            var warnings  = new List<Warning>();
            var location  = network.GetChargingStationOperatorById(GEF)!.ChargingPools.Single().ToOCPI(network, warnings);

            Assert.That(location, Is.Not.Null, String.Join(" ", warnings.Select(warning => warning.Text.FirstText())));

            var evse      = location!.EVSEs.Single();
            var connector = evse.Connectors.Single();

            Assert.Multiple(() => {

                Assert.That(location.CountryCode.ToString(),      Is.EqualTo("DE"));
                Assert.That(location.PartyId.ToString(),          Is.EqualTo("GEF"));
                Assert.That(location.Id.ToString(),               Is.EqualTo("1"));
                Assert.That(location.Name,                        Is.EqualTo("Biberweg"));
                Assert.That(location.Address,                     Is.EqualTo("Biberweg 18"));
                Assert.That(location.City,                        Is.EqualTo("Jena"));
                Assert.That(location.PostalCode,                  Is.EqualTo("07749"));
                Assert.That(location.TimeZone,                    Is.EqualTo("Europe/Berlin"));
                Assert.That(location.ParkingType,                 Is.EqualTo(ParkingType.PARKING_LOT));
                Assert.That(location.Facilities,                  Is.EqualTo(new[] { Facility.CAFE }));
                Assert.That(location.Operator?.Name,              Is.EqualTo("GraphDefined"));

                Assert.That(evse.UId.ToString(),                  Is.EqualTo("DE*GEF*E1*1"));
                Assert.That(evse.EVSEId?.ToString(),              Is.EqualTo("DE*GEF*E1*1"));
                Assert.That(evse.Status,                          Is.EqualTo(StatusType.AVAILABLE));
                Assert.That(evse.Capabilities,                    Is.EquivalentTo(new[] { Capability.RFID_READER, Capability.REMOTE_START_STOP_CAPABLE, Capability.RESERVABLE }));

                Assert.That(connector.Id.ToString(),              Is.EqualTo("1"));
                Assert.That(connector.Standard,                   Is.EqualTo(ConnectorType.IEC_62196_T2));
                Assert.That(connector.Format,                     Is.EqualTo(ConnectorFormats.SOCKET));
                Assert.That(connector.PowerType,                  Is.EqualTo(PowerTypes.AC_3_PHASE));
                Assert.That(connector.MaxVoltage.Value,           Is.EqualTo(400));
                Assert.That(connector.MaxAmperage.Value,          Is.EqualTo(32));
                Assert.That(connector.MaxElectricPower?.Value,    Is.EqualTo(22000));
                Assert.That(connector.TariffIds.Select(tariffId => tariffId.ToString()),  Is.EqualTo(new[] { "1" }));

            });

        }

        #endregion

        #region AnEnergyMixMapsBothWays()

        [Test]
        public void AnEnergyMixMapsBothWays()
        {

            var energyMix  = new POI.EnergyMix([ new PercentageOf<POI.EnergySourceCategory>(POI.EnergySourceCategory.Sun,  60),
                                                 new PercentageOf<POI.EnergySourceCategory>(POI.EnergySourceCategory.Wind, 40) ],
                                               [],
                                               new ImmutableI18NString(I18NString.Create(Languages.en, "Stadtwerke Jena")),
                                               new ImmutableI18NString(I18NString.Create(Languages.en, "Ökostrom")));

            var network    = ANetwork();
            var warnings   = new List<Warning>();
            var location   = APool(EnergyMix: energyMix).ToOCPI(network, warnings);

            Assert.That(location?.EnergyMix, Is.Not.Null, String.Join(" ", warnings.Select(warning => warning.Text.FirstText())));

            var back       = location!.ToPOI3(GEF, null, Languages.en, warnings);

            Assert.That(back?["energyMix"], Is.InstanceOf<JObject>(), String.Join(" ", warnings.Select(warning => warning.Text.FirstText())));

            var backMix    = POI.EnergyMix.Parse((JObject) back!["energyMix"]!);

            Assert.Multiple(() => {
                Assert.That(location.EnergyMix!.IsGreenEnergy,                                                    Is.True);
                Assert.That(location.EnergyMix.EnergySources.Select(source => (source.Source, source.Percentage)), Is.EquivalentTo(new[] { (EnergySourceCategory.SOLAR, 60.0),
                                                                                                                                        (EnergySourceCategory.WIND,  40.0) }));
                Assert.That(backMix.EnergySources.Select(source => (source.Value, source.Percent)),                 Is.EquivalentTo(new[] { (POI.EnergySourceCategory.Sun,  60f),
                                                                                                                                        (POI.EnergySourceCategory.Wind, 40f) }));
                Assert.That(warnings,                                                                              Is.Empty);
            });

        }

        #endregion

        #region ATariffBecomesAnOCPITariffAndBack()

        [Test]
        public void ATariffBecomesAnOCPITariffAndBack()
        {

            var warnings  = new List<Warning>();
            var tariff    = ATariff().ToOCPI(warnings);

            Assert.That(tariff, Is.Not.Null);

            var timed     = tariff!.TariffElements.ElementAt(1);
            var back      = tariff.ToPOI3(GEF, Languages.en, warnings);

            var expected  = ATariff().ToJSON();
            var actual    = back!.ToJSON();

            foreach (var json in new[] { expected, actual })
            {
                json.Remove("created");
                json.Remove("lastChange");
            }

            Assert.Multiple(() => {
                Assert.That(tariff.Id.ToString(),                                    Is.EqualTo("1"));
                Assert.That(tariff.Currency,                                         Is.EqualTo(Currency.EUR));
                Assert.That(timed.TariffRestrictions?.MinPower,                      Is.EqualTo(3.7m));
                Assert.That(timed.TariffRestrictions?.MinkWh,                        Is.EqualTo(5m));
                Assert.That(timed.TariffRestrictions?.MaxDuration,                   Is.EqualTo(TimeSpan.FromHours(1)));
                Assert.That(timed.PriceComponents.First().StepSize,                  Is.EqualTo(60));
                Assert.That(JToken.DeepEquals(expected, actual),                     Is.True, $"{expected}\n{actual}");
                Assert.That(warnings,                                                Is.Empty);
            });

        }

        #endregion

        #region StatusMapsBothWays()

        [Test]
        public void StatusMapsBothWays()
        {
            Assert.Multiple(() => {
                Assert.That(POI3Mapper.ToOCPIStatus(POI.EVSEStatusType.Charging,  POI.EVSEAdminStatusType.Operational),  Is.EqualTo(StatusType.CHARGING));
                Assert.That(POI3Mapper.ToOCPIStatus(POI.EVSEStatusType.Available, POI.EVSEAdminStatusType.Planned),      Is.EqualTo(StatusType.PLANNED));
                Assert.That(POI3Mapper.ToOCPIStatus(POI.EVSEStatusType.Available, POI.EVSEAdminStatusType.OutOfService), Is.EqualTo(StatusType.INOPERATIVE));
                Assert.That(StatusType.OUTOFORDER.ToPOI3Status().ToOCPIStatus(),                                         Is.EqualTo(StatusType.OUTOFORDER));
                Assert.That(StatusType.RESERVED.  ToPOI3Status().ToOCPIStatus(),                                         Is.EqualTo(StatusType.RESERVED));
            });
        }

        #endregion


        #region ThePublisherKeepsTheCommonAPIUpToDate()

        [Test]
        public async Task ThePublisherKeepsTheCommonAPIUpToDate()
        {

            var history    = POI.RoamingNetworkHistory.Start(ANetwork());
            var errors     = new List<String>();

            await using var publisher = new POI3ToOCPIPublisher(history, api);
            publisher.OnError += (sender, description, warnings) => { lock (errors) errors.Add(description); };

            await publisher.StartAsync();

            var locationId = Location_Id.Parse("1");

            Assert.Multiple(() => {
                Assert.That(api.TryGetLocation(Party, locationId, out var location),  Is.True);
                Assert.That(location?.EVSEs.Single().Status,                          Is.EqualTo(StatusType.AVAILABLE));
                Assert.That(api.TryGetTariff(Party, Tariff_Id.Parse("1"), out _),     Is.True);
            });

            // A change of the content...
            var head = history.Head.RoamingNetwork;
            Assert.That(history.Commit(head.CreateSignedChangeSet([ POI.RoamingNetworkChange.UpdateProperty(head.GetChargingStationOperatorById(GEF)!.EVSEs.Single(), "maxPower", null, 11000) ],
                                                                  CreatedAt: T0)).IsCommitted);

            Assert.That(await Eventually(() => api.TryGetLocation(Party, locationId, out var location) &&
                                               location.EVSEs.Single().Connectors.Single().MaxElectricPower?.Value == 11000),
                        Is.True, "The changed maximum power was not published.");

            // ...and of the status.
            history.RuntimeStatus.TryUpdate(POI.POIRuntimeStatusKind.Status, nameof(POI.EVSE), E1.ToString(), "charging", T0.AddMinutes(1), out _, out _);

            Assert.That(await Eventually(() => api.TryGetLocation(Party, locationId, out var location) &&
                                               location.EVSEs.Single().Status == StatusType.CHARGING),
                        Is.True, "The changed status was not published.");

            // A removed charging pool.
            head = history.Head.RoamingNetwork;
            Assert.That(history.Commit(head.CreateSignedChangeSet([ POI.RoamingNetworkChange.Remove(P1) ], CreatedAt: T0.AddMinutes(2))).IsCommitted);

            Assert.That(await Eventually(() => !api.TryGetLocation(Party, locationId, out _)),
                        Is.True, "The removed charging pool is still a location.");

            lock (errors)
                Assert.That(errors, Is.Empty);

        }

        #endregion

        #region TheImporterCommitsWhatTheCommonAPIReceives()

        [Test]
        public async Task TheImporterCommitsWhatTheCommonAPIReceives()
        {

            var history    = POI.RoamingNetworkHistory.Start(ANetwork(WithPools: false));
            var errors     = new List<String>();

            await using var importer = new OCPIToPOI3Importer(api, history, changeSet => changeSet.SignedByAll(history.Head.RoamingNetwork));
            importer.OnError += (sender, description, warnings) => {
                lock (errors)
                    errors.Add(description + " " + String.Join(" ", warnings.Select(warning => warning.Text.FirstText())));
            };

            importer.Start();

            // What a CPO would push: the OCPI data of another POI3 network.
            var source     = ANetwork();
            var warnings   = new List<Warning>();
            var tariff     = source.GetChargingStationOperatorById(GEF)!.ChargingTariffs.Single().ToOCPI(warnings)!;
            var location   = source.GetChargingStationOperatorById(GEF)!.ChargingPools.  Single().ToOCPI(source, warnings)!;

            Assert.That((await api.AddOrUpdateTariff  (tariff)).  IsSuccess);
            Assert.That((await api.AddOrUpdateLocation(location)).IsSuccess);

            POI.EVSE? Imported()
                => history.Head.RoamingNetwork.TryGetChargingStationOperatorById(GEF, out var op) ? op.EVSEs.FirstOrDefault() : null;

            Assert.That(await Eventually(() => Imported() is not null), Is.True, String.Join("\n", errors));

            var imported   = history.Head.RoamingNetwork.GetChargingStationOperatorById(GEF)!;

            Assert.Multiple(() => {
                Assert.That(imported.ChargingTariffs.Single().Id,                     Is.EqualTo(T1));
                Assert.That(imported.ChargingPools.Single().Id,                       Is.EqualTo(P1));
                Assert.That(imported.ChargingPools.Single().Address?.Street,          Is.EqualTo("Biberweg 18"));
                Assert.That(imported.ChargingStations.Single().Id,                    Is.EqualTo(S1));
                Assert.That(imported.EVSEs.Single().Id,                               Is.EqualTo(E1));
                Assert.That(imported.EVSEs.Single().MaxPower?.Value,                  Is.EqualTo(22000));
                Assert.That(imported.EVSEs.Single().ChargingTariffIds,                Is.EqualTo(new[] { T1 }));
            });

            // A changed location: only what changed is committed.
            var commits    = history.Head.Commit.Revision;
            var changed    = APool(MaxPower: 11000).ToOCPI(source, warnings, T0)!;

            Assert.That((await api.AddOrUpdateLocation(changed)).IsSuccess);

            Assert.That(await Eventually(() => Imported()?.MaxPower?.Value == 11000), Is.True, String.Join("\n", errors));

            var changeSet  = history.Head.Commit.ChangeSet!;

            Assert.Multiple(() => {
                Assert.That(history.Head.Commit.Revision,                             Is.EqualTo(commits + 1));
                Assert.That(changeSet.Changes.Select(change => change.PropertyName),  Is.EqualTo(new[] { "maxPower" }));
            });

            // A changed status.
            var storedLocation = api.TryGetLocation(Party, Location_Id.Parse("1"), out var current) ? current : null;
            var charging       = new EVSE(storedLocation!.EVSEs.Single().UId,
                                          StatusType.CHARGING,
                                          storedLocation.EVSEs.Single().Connectors,
                                          storedLocation.EVSEs.Single().EVSEId,
                                          LastUpdated: T0.AddMinutes(5));

            Assert.That((await api.AddOrUpdateEVSE(storedLocation, charging)).IsSuccess);

            Assert.That(await Eventually(() => history.RuntimeStatus.TryGet(POI.POIRuntimeStatusKind.Status, nameof(POI.EVSE), E1.ToString(), out var status) &&
                                               status.Value == "charging"),
                        Is.True, String.Join("\n", errors));

            lock (errors)
                Assert.That(errors, Is.Empty);

        }

        #endregion

    }

}
