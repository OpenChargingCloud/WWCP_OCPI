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
        private static readonly POI.ParkingOperator_Id              PAR       = POI.ParkingOperator_Id.        Parse("DE*PAR");
        private static readonly POI.ParkingSpace_Id                 GS1       = POI.ParkingSpace_Id.           Parse("GEF-1");
        private static readonly POI.ParkingSpace_Id                 PS1       = POI.ParkingSpace_Id.           Parse("PAR-1");

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

        private static POI.ChargingTariff ATariff(I18NString? Description = null)

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
                                                                                         DaysOfWeek:  [ DayOfWeek.Sunday ])),
                        new POI.ChargingTariffElement([ new POI.ChargingPriceComponent(POI.ChargingDimensionTypes.TIME, 2, StepSize: 60) ],
                                                      new POI.ChargingTariffRestrictions(Reservation: POI.ChargingReservationRestriction.RESERVATION))
                    ],
                    I18NString.Create(Languages.en, "Standard"),
                    Description,
                    TariffType:  POI.ChargingTariffType.REGULAR,
                    MinPrice:    new POI.ChargingPriceLimit(2.5m, 2.98m),
                    MaxPrice:    new POI.ChargingPriceLimit(50),
                    ValidFrom:   DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
                    ValidUntil:  DateTimeOffset.Parse("2027-01-01T00:00:00Z"),
                    Created:     Created);

        private static POI.EVSE AnEVSE(Int32    MaxPower     = 22000,
                                       Boolean  OwnTariffs   = true)

            => new (E1,
                    CurrentTypes:        POI.CurrentTypes.AC_ThreePhases,
                    MaxVoltage:          Volt.  FromV(400),
                    MaxCurrent:          Ampere.FromA(32),
                    MaxPower:            Watt.  FromW(MaxPower),
                    ChargingConnectors:  [ new POI.ChargingConnector(POI.ChargingConnector_Id.Parse("1"),
                                                                     POI.ChargingConnectorType.IEC_62196_T2,
                                                                     Format:                 POI.ChargingConnectorFormat.CABLE,
                                                                     CableLength:            Meter.   From_m (5),
                                                                     CableWeight:            Kilogram.FromKG(2.5m),
                                                                     CableManagementSystem:  true,
                                                                     Standards:              [ POI.Standard.PAS1899_2022 ]) ],
                    ChargingTariffIds:   OwnTariffs ? [ T1 ] : null,
                    ReachDistance:             Meter.From_m(0.6m),
                    OperationTimeout:          TimeSpan.FromSeconds(90),
                    ExtendedOperationTimeout:  true,
                    Standards:                 [ POI.Standard.PAS1899_2022 ],
                    Created:             Created);

        private static POI.ChargingPool APool(Int32           MaxPower         = 22000,
                                              POI.EnergyMix?  EnergyMix        = null,
                                              I18NString?     Description      = null,
                                              String          Name             = "Biberweg",
                                              Boolean         OwnTariffs       = true,
                                              Boolean         FeaturesByGroup  = false)

            => new (P1,
                    I18NString.Create(Languages.en, Name),
                    Description:          Description,
                    Address:              new Address("Biberweg", "07749", I18NString.Create(Languages.de, "Jena"), Country.Germany, HouseNumber: "18"),
                    GeoLocation:          GeoCoordinate.Parse(50.92, 11.58),
                    ParkingType:          POI.ParkingType.PARKING_LOT,
                    TimeZone:             Time_Zone.Parse("Europe/Berlin"),
                    Facilities:           FeaturesByGroup ? null : [ POI.Facility.CAFE ],
                    EnergyMix:            EnergyMix,
                    AuthenticationModes:  [ POI.AuthenticationMode.RFID(), POI.AuthenticationMode.REMOTE ],
                    Services:                  [ POI.LocationService.ACCESSIBLE_CHARGING, POI.LocationService.ASSISTANCE ],
                    AssistanceServiceDetails:  I18NString.Create(Languages.en, "Ring at the kiosk"),
                    Standards:                 [ POI.Standard.PAS1899_2022 ],
                    ChargingStations:     [ new POI.ChargingStation(S1,
                                                                    Features:         FeaturesByGroup ? null : [ POI.ChargingStationFeature.Reservable ],
                                                                    ParkingSpaceIds:  [ GS1, PS1 ],
                                                                    EVSEs:     [ AnEVSE(MaxPower, OwnTariffs) ],
                                                                    Created:   Created) ],
                    Created:              Created);

        private static POI.ParkingSpace AParkingSpace()

            => new (GS1,
                    PhysicalReference:         "1",
                    VehicleTypes:              [ POI.VehicleType.PERSONAL_VEHICLE ],
                    MaxVehicleHeight:          Meter.From_m(2.1m),
                    Length:                    Meter.From_m(5),
                    Width:                     Meter.From_m(3.5m),
                    EVSEPosition:              POI.EVSEPosition.LEFT,
                    Direction:                 POI.ParkingDirection.PERPENDICULAR,
                    Surface:                   POI.ParkingSurface.ASPHALT,
                    TimeLimit:                 TimeSpan.FromHours(4),
                    Roofed:                    true,
                    Illuminated:               true,
                    Standards:                 [ POI.Standard.PAS1899_2022 ],
                    Created:                   Created);

        private static POI.RoamingNetwork ANetwork(Boolean WithPools        = true,
                                                   Boolean WithPOI3Only     = false,
                                                   Boolean FeaturesByGroup  = false)

            => new POI.RoamingNetwork(POI.RoamingNetwork_Id.Parse("rn1"),
                                      Created:                   Created,
                                      ChargingStationOperators:  [ new POI.ChargingStationOperator(GEF,
                                                                                                   Name:            I18NString.Create(Languages.en, "GraphDefined"),
                                                                                                   Homepage:        URL.Parse("https://graphdefined.com"),
                                                                                                   ChargingTariffs: WithPools ? [ ATariff(WithPOI3Only ? I18NString.Create(Languages.de, "Der Standardtarif") : null) ] : null,
                                                                                                   ChargingPools:   WithPools ? [ APool(Description: WithPOI3Only ? I18NString.Create(Languages.de, "Hinter dem Haus") : null,
                                                                                                                                       OwnTariffs:  !WithPOI3Only,
                                                                                                                                       FeaturesByGroup: FeaturesByGroup) ] : null,
                                                                                                   ChargingStationGroups: WithPools && FeaturesByGroup ? [ new POI.ChargingStationGroup(POI.ChargingStationGroup_Id.Parse("DE*GEF*SG1"),
                                                                                                                                                                                       ChargingStationIds:  [ S1 ],
                                                                                                                                                                                       PhysicalReference:   "A",
                                                                                                                                                                                       Features:            [ POI.ChargingStationFeature.Reservable ],
                                                                                                                                                                                       Created:             Created) ] : null,
                                                                                                   ChargingPoolGroups:    WithPools && FeaturesByGroup ? [ new POI.ChargingPoolGroup(POI.ChargingPoolGroup_Id.Parse("DE*GEF*PG1"),
                                                                                                                                                                                 ChargingPoolIds:  [ P1 ],
                                                                                                                                                                                 Facilities:       [ POI.Facility.CAFE ],
                                                                                                                                                                                 Created:          Created) ] : null,
                                                                                                   EVSEGroups:      WithPools && WithPOI3Only ? [ new POI.EVSEGroup(POI.EVSEGroup_Id.Parse(GEF, "1"), EVSEIds: [ E1 ], ChargingTariffId: T1, Created: Created) ] : null,
                                                                                                   ParkingSpaces:   WithPools ? [ AParkingSpace() ] : null,
                                                                                                   Created:         Created) ],
                                      ParkingOperators:          [ new POI.ParkingOperator(PAR, Created: Created, ParkingSpaces: [ new POI.ParkingSpace(PS1, Created: Created) ]) ]).WithKeys();

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
                Assert.That(location.Services,                    Is.EquivalentTo(new[] { LocationService.ACCESSIBLE_CHARGING, LocationService.ASSISTANCE }));
                Assert.That(location.ParkingPlaces.Select(parking => parking.Id.ToString()),  Is.EqualTo(new[] { "GEF-1", "PAR-1" }));
                Assert.That(location.ParkingPlaces.First().ParkingSpaceLength?.m,             Is.EqualTo(5));
                Assert.That(location.ParkingPlaces.First().Direction,                         Is.EqualTo(ParkingDirection.PERPENDICULAR));
                Assert.That(location.ParkingPlaces.First().Lighting,                          Is.True);
                Assert.That(location.ParkingPlaces.First().RestrictedToType,                  Is.False, "Unknown in POI3, required by OCPI.");
                Assert.That(evse.Parking.Select(parking => parking.ParkingId.ToString()),     Is.EqualTo(new[] { "GEF-1", "PAR-1" }));
                Assert.That(location.AssistanceServiceDetails,    Is.EqualTo("Ring at the kiosk"));
                Assert.That(location.Standards,                   Is.EqualTo(new[] { Standard.PAS1899_2022 }));
                Assert.That(evse.ReachDistance?.cm,               Is.EqualTo(60));
                Assert.That(evse.OperationTimeout,                Is.EqualTo(TimeSpan.FromSeconds(90)));
                Assert.That(evse.ExtendedOperationTimeout,        Is.True);
                Assert.That(evse.Standards,                       Is.EqualTo(new[] { Standard.PAS1899_2022 }));
                Assert.That(connector.CableLength?.m,             Is.EqualTo(5));
                Assert.That(connector.CableWeight?.Value,         Is.EqualTo(2.5m));
                Assert.That(connector.CableManagementSystem,      Is.True);

                Assert.That(evse.UId.ToString(),                  Is.EqualTo("DE*GEF*E1*1"));
                Assert.That(evse.EVSEId?.ToString(),              Is.EqualTo("DE*GEF*E1*1"));
                Assert.That(evse.Status,                          Is.EqualTo(StatusType.AVAILABLE));
                Assert.That(evse.Capabilities,                    Is.EquivalentTo(new[] { Capability.RFID_READER, Capability.REMOTE_START_STOP_CAPABLE, Capability.RESERVABLE }));

                Assert.That(connector.Id.ToString(),              Is.EqualTo("1"));
                Assert.That(connector.Standard,                   Is.EqualTo(ConnectorType.IEC_62196_T2));
                Assert.That(connector.Format,                     Is.EqualTo(ConnectorFormats.CABLE));
                Assert.That(connector.PowerType,                  Is.EqualTo(PowerTypes.AC_3_PHASE));
                Assert.That(connector.MaxVoltage.Value,           Is.EqualTo(400));
                Assert.That(connector.MaxAmperage.Value,          Is.EqualTo(32));
                Assert.That(connector.MaxElectricPower?.Value,    Is.EqualTo(22000));
                Assert.That(connector.TariffIds.Select(tariffId => tariffId.ToString()),  Is.EqualTo(new[] { "1" }));

            });

        }

        #endregion

        #region (private static) ABrand(Id, Name, Homepage, Logo) / ABrandedNetwork(PoolBrands)

        private static POI.Brand ABrand(String   Id,
                                        String   Name,
                                        String?  Homepage  = null,
                                        String?  Logo      = null)

            => new (POI.Brand_Id.Parse(Id),
                    new ImmutableI18NString(I18NString.Create(Languages.en, Name)),
                    Logo:      Logo     is not null ? URL.Parse(Logo)     : null,
                    Homepage:  Homepage is not null ? URL.Parse(Homepage) : null);

        /// <summary>
        /// Brands on every level, each meaning something of its own: the operator a white label of "white-charge",
        /// the station and the EVSE also appearing under "station-charge" and "evse-charge"; the pool under the given ones.
        /// </summary>
        private static POI.RoamingNetwork ABrandedNetwork(params POI.Brand[] PoolBrands)

            => new POI.RoamingNetwork(POI.RoamingNetwork_Id.Parse("rn1"),
                                      Created:                   Created,
                                      ChargingStationOperators:  [ new POI.ChargingStationOperator(GEF,
                                                                                                   Name:           I18NString.Create(Languages.en, "GraphDefined"),
                                                                                                   Brands:         [ ABrand("white-charge", "White Charge", "https://white.example") ],
                                                                                                   ChargingPools:  [ new POI.ChargingPool(P1,
                                                                                                                                          I18NString.Create(Languages.en, "Biberweg"),
                                                                                                                                          Address:           new Address("Biberweg", "07749", I18NString.Create(Languages.de, "Jena"), Country.Germany, HouseNumber: "18"),
                                                                                                                                          GeoLocation:       GeoCoordinate.Parse(50.92, 11.58),
                                                                                                                                          TimeZone:          Time_Zone.Parse("Europe/Berlin"),
                                                                                                                                          Brands:            PoolBrands,
                                                                                                                                          ChargingStations:  [ new POI.ChargingStation(S1,
                                                                                                                                                                                       Brands:   [ ABrand("station-charge", "Station Charge") ],
                                                                                                                                                                                       EVSEs:    [ new POI.EVSE(E1,
                                                                                                                                                                                                                CurrentTypes:        POI.CurrentTypes.AC_ThreePhases,
                                                                                                                                                                                                                MaxVoltage:          Volt.  FromV(400),
                                                                                                                                                                                                                MaxCurrent:          Ampere.FromA(32),
                                                                                                                                                                                                                MaxPower:            Watt.  FromW(22000),
                                                                                                                                                                                                                ChargingConnectors:  [ new POI.ChargingConnector(POI.ChargingConnector_Id.Parse("1"),
                                                                                                                                                                                                                                                                 POI.ChargingConnectorType.IEC_62196_T2) ],
                                                                                                                                                                                                                Brands:              [ ABrand("evse-charge", "EVSE Charge") ],
                                                                                                                                                                                                                Created:             Created) ],
                                                                                                                                                                                       Created:  Created) ],
                                                                                                                                          Created:           Created) ],
                                                                                                   Created:        Created) ]).WithKeys();

        #endregion

        #region ThePoolsFirstBrandIsTheSubOperator()

        [Test]
        public void ThePoolsFirstBrandIsTheSubOperator()
        {

            var network   = ABrandedNetwork(ABrand("zeta-charge",  "Zeta Charge"),
                                            ABrand("city-charge",  "City Charge", "https://city.example", "https://city.example/logo.png"));
            var warnings  = new List<Warning>();
            var location  = network.GetChargingStationOperatorById(GEF)!.ChargingPools.Single().ToOCPI(network, warnings);

            Assert.That(location, Is.Not.Null, String.Join(" ", warnings.Select(warning => warning.Text.FirstText())));

            Assert.Multiple(() => {

                // The first in the order of the identifications, whatever order they were given in.
                Assert.That(location!.SubOperator?.Name,              Is.EqualTo("City Charge"));
                Assert.That(location.SubOperator?.Website?.ToString(), Is.EqualTo("https://city.example"));
                Assert.That(location.SubOperator?.Logo?.URL.ToString(), Is.EqualTo("https://city.example/logo.png"));
                Assert.That(location.SubOperator?.Logo?.Category,     Is.EqualTo(ImageCategory.NETWORK));

                // The operator stays the operator: its white label is not the location's suboperator.
                Assert.That(location.Operator?.Name,                  Is.EqualTo("GraphDefined"));

            });

        }

        #endregion

        #region APoolWithoutBrandsHasNoSubOperatorWhateverTheOtherLevelsHave()

        [Test]
        public void APoolWithoutBrandsHasNoSubOperatorWhateverTheOtherLevelsHave()
        {

            var network   = ABrandedNetwork();
            var warnings  = new List<Warning>();
            var location  = network.GetChargingStationOperatorById(GEF)!.ChargingPools.Single().ToOCPI(network, warnings);

            Assert.That(location, Is.Not.Null, String.Join(" ", warnings.Select(warning => warning.Text.FirstText())));

            // Neither the operator's white label nor the brands of its station or EVSE: every level its own.
            Assert.That(location!.SubOperator, Is.Null);

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
                Assert.That(tariff.TariffType,                                       Is.EqualTo(TariffType.REGULAR));
                Assert.That(tariff.TaxIncluded,                                      Is.EqualTo(TaxIncluded.No), "POI3 prices exclude VAT.");
                Assert.That(tariff.MinPrice,                                         Is.EqualTo(new PriceLimit(2.5m, 2.98m)));
                Assert.That(tariff.MaxPrice,                                         Is.EqualTo(new PriceLimit(50, null)));
                Assert.That(tariff.Start,                                            Is.EqualTo(DateTimeOffset.Parse("2026-01-01T00:00:00Z")));
                Assert.That(tariff.End,                                              Is.EqualTo(DateTimeOffset.Parse("2027-01-01T00:00:00Z")));
                Assert.That(tariff.TariffElements.ElementAt(2).TariffRestrictions?.Reservation,
                                                                                     Is.EqualTo(ReservationRestrictions.RESERVATION));
                Assert.That(timed.TariffRestrictions?.MinPower,                      Is.EqualTo(3.7m));
                Assert.That(timed.TariffRestrictions?.MinkWh,                        Is.EqualTo(5m));
                Assert.That(timed.TariffRestrictions?.MaxDuration,                   Is.EqualTo(TimeSpan.FromHours(1)));
                Assert.That(timed.PriceComponents.First().StepSize,                  Is.EqualTo(60));
                Assert.That(JToken.DeepEquals(expected, actual),                     Is.True, $"{expected}\n{actual}");
                Assert.That(warnings,                                                Is.Empty);
            });

        }

        #endregion

        #region PricesIncludingTaxesBecomeNetPrices()

        [Test]
        public void PricesIncludingTaxesBecomeNetPrices()
        {

            Tariff ATariffOf(TaxIncluded TaxIncluded)

                => new (CountryCode.Parse("DE"), Party_Id.Parse("GEF"), Tariff_Id.Parse("1"), Currency.EUR,
                        [ new TariffElement(new PriceComponent(TariffDimension.ENERGY, 0.4641m, 19, 1),
                                            new PriceComponent(TariffDimension.FLAT,   1,       null, 1)) ],
                        TaxIncluded,
                        Created:      Created,
                        LastUpdated:  T0);

            var warnings  = new List<Warning>();
            var gross     = ATariffOf(TaxIncluded.Yes).ToPOI3(GEF, Languages.en, warnings)!;
            var net       = ATariffOf(TaxIncluded.No). ToPOI3(GEF, Languages.en, warnings)!;
            var untaxed   = ATariffOf(TaxIncluded.NA). ToPOI3(GEF, Languages.en, warnings)!;

            Assert.Multiple(() => {

                // Including 19 % VAT; without any VAT given, none applies.
                Assert.That(gross.  Elements[0].PriceComponents.Select(component => (component.Price, component.VAT)),
                            Is.EqualTo(new (Decimal, Decimal?)[] { (0.39m,   19), (1, null) }));

                // Excluding VAT, as POI3 prices are.
                Assert.That(net.    Elements[0].PriceComponents.Select(component => (component.Price, component.VAT)),
                            Is.EqualTo(new (Decimal, Decimal?)[] { (0.4641m, 19), (1, null) }));

                // No taxes apply.
                Assert.That(untaxed.Elements[0].PriceComponents.Select(component => (component.Price, component.VAT)),
                            Is.EqualTo(new (Decimal, Decimal?)[] { (0.4641m, 19), (1, 0) }));

                Assert.That(warnings,  Is.Empty);

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

                // The parking place of no parking operator is one of the operator; the one of a parking operator is named.
                Assert.That(imported.ParkingSpaces.Select(space => space.Id),         Is.EqualTo(new[] { GS1 }));
                Assert.That(imported.ParkingSpaces.Single().Width?.m,                 Is.EqualTo(3.5m));
                Assert.That(imported.ParkingSpaces.Single().EVSEPosition,             Is.EqualTo(POI.EVSEPosition.LEFT));
                Assert.That(imported.ChargingStations.Single().ParkingSpaceIds,       Is.EqualTo(new[] { GS1, PS1 }));
                Assert.That(history.Head.RoamingNetwork.ParkingOperators.Single().ParkingSpaces.Single().ToJSON().ToString(),
                            Is.EqualTo(new POI.ParkingSpace(PS1, Created: Created).ToJSON().ToString()));
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

        #region WhatDoesNotMapIsToldAsWarnings()

        [Test]
        public void WhatDoesNotMapIsToldAsWarnings()
        {

            var network          = ANetwork();
            var op               = network.GetChargingStationOperatorById(GEF)!;
            var pool             = op.ChargingPools.Single();

            // A tariff with a dimension OCPI 2.3.0 does not have is not published.
            var tariffWarnings   = new List<Warning>();
            var minCurrent       = new POI.ChargingTariff(T1,
                                                          Currency.EUR,
                                                          [ new POI.ChargingTariffElement([ new POI.ChargingPriceComponent(POI.ChargingDimensionTypes.MIN_CURRENT, 1, StepSize: 1) ]) ],
                                                          Created: Created).ToOCPI(tariffWarnings);

            // A tariff type POI3 does not know is left out; a minimum price above the maximum refuses the tariff.
            var ocpiTariff       = ATariff().ToOCPI([])!;
            var typeWarnings     = new List<Warning>();
            var unknownType      = new Tariff(ocpiTariff.CountryCode, ocpiTariff.PartyId, ocpiTariff.Id, ocpiTariff.Currency, ocpiTariff.TariffElements, TaxIncluded.NA,
                                              TariffType: TariffType.Parse("PROFILE_NIGHT"), Created: Created, LastUpdated: T0).ToPOI3(GEF, Languages.en, typeWarnings);
            var limitWarnings    = new List<Warning>();
            var minAboveMax      = new Tariff(ocpiTariff.CountryCode, ocpiTariff.PartyId, ocpiTariff.Id, ocpiTariff.Currency, ocpiTariff.TariffElements, TaxIncluded.NA,
                                              MinPrice: new PriceLimit(10, null), MaxPrice: new PriceLimit(5, null), Created: Created, LastUpdated: T0).ToPOI3(GEF, Languages.en, limitWarnings);

            // A connector with a plug type OCPI does not know is left out.
            var plugWarnings     = new List<Warning>();
            var evse             = new POI.EVSE(E1,
                                                CurrentTypes:        POI.CurrentTypes.AC_ThreePhases,
                                                ChargingConnectors:  [ new POI.ChargingConnector(POI.ChargingConnector_Id.Parse("1"), POI.ChargingConnectorType.IEC_62196_T2),
                                                                       new POI.ChargingConnector(POI.ChargingConnector_Id.Parse("2"), POI.ChargingConnectorType.Parse("NO_SUCH_PLUG")) ],
                                                Created:             Created);
            var ocpiEVSE         = evse.ToOCPI(pool.ChargingStations.Single(), pool, op, plugWarnings);

            // A location identification that is no charging pool suffix is refused.
            var idWarnings       = new List<Warning>();
            var location         = pool.ToOCPI(network, [])!;
            var badLocation      = new Location(location.CountryCode, location.PartyId, Location_Id.Parse("A-1"), true, location.Address, location.City,
                                                location.Country, location.Coordinates, location.TimeZone, Created: Created, LastUpdated: T0);

            Assert.Multiple(() => {

                Assert.That(minCurrent,                                            Is.Null);
                Assert.That(tariffWarnings.Single().Text.FirstText(),              Does.Contain("MIN_CURRENT"));

                Assert.That(unknownType?.TariffType,                               Is.Null);
                Assert.That(typeWarnings.Single().Text.FirstText(),                Does.Contain("PROFILE_NIGHT"));
                Assert.That(minAboveMax,                                           Is.Null);
                Assert.That(limitWarnings.Single().Text.FirstText(),               Does.Contain("must not exceed its maximum price"));

                Assert.That(ocpiEVSE?.Connectors.Select(connector => connector.Id.ToString()), Is.EqualTo(new[] { "1" }));
                Assert.That(plugWarnings.Single().Text.FirstText(),                Does.Contain("NO_SUCH_PLUG"));

                Assert.That(badLocation.ToPOI3(GEF, null, Languages.en, idWarnings), Is.Null);
                Assert.That(idWarnings.Single().Text.FirstText(),                  Does.Contain("A-1"));

                Assert.That(POI3Mapper.ToPOI3Operator(CountryCode.Parse("DE"), Party_Id.Parse("GE")), Is.Null);

            });

        }

        #endregion


        #region ThePublisherRemovesWhatIsGone()

        [Test]
        public async Task ThePublisherRemovesWhatIsGone()
        {

            var history    = POI.RoamingNetworkHistory.Start(ANetwork());
            var errors     = new List<String>();

            await using var publisher = new POI3ToOCPIPublisher(history, api);
            publisher.OnError += (sender, description, warnings) => { lock (errors) errors.Add(description); };

            await publisher.StartAsync();

            var locationId = Location_Id.Parse("1");
            var tariffId   = Tariff_Id.  Parse("1");

            // A removed EVSE...
            var head = history.Head.RoamingNetwork;
            Assert.That(history.Commit(head.CreateSignedChangeSet([ POI.RoamingNetworkChange.Remove(E1) ], CreatedAt: T0)).IsCommitted);

            Assert.That(await Eventually(() => api.TryGetLocation(Party, locationId, out var location) && !location.EVSEs.Any()),
                        Is.True, "The removed EVSE is still an EVSE of the location.");

            // ...and a removed charging tariff, no longer named by the EVSE.
            head = history.Head.RoamingNetwork;
            Assert.That(history.Commit(head.CreateSignedChangeSet([ POI.RoamingNetworkChange.Remove(T1) ], CreatedAt: T0.AddMinutes(1))).IsCommitted);

            Assert.That(await Eventually(() => !api.TryGetTariff(Party, tariffId, out _)),
                        Is.True, "The removed charging tariff is still a tariff.");

            Assert.That(api.TryGetLocation(Party, locationId, out _), Is.True);

            lock (errors)
                Assert.That(errors, Is.Empty);

        }

        #endregion

        #region ThePublisherRemovesWhatWasRemovedWhileItWasStopped()

        [Test]
        public async Task ThePublisherRemovesWhatWasRemovedWhileItWasStopped()
        {

            var history    = POI.RoamingNetworkHistory.Start(ANetwork());
            var errors     = new List<String>();
            var locationId = Location_Id.Parse("1");
            var tariffId   = Tariff_Id.  Parse("1");

            await using (var publisher = new POI3ToOCPIPublisher(history, api))
                await publisher.StartAsync();

            Assert.That(api.TryGetLocation(Party, locationId, out _), Is.True);

            // Removed while no publisher runs...
            var head = history.Head.RoamingNetwork;
            Assert.That(history.Commit(head.CreateSignedChangeSet([ POI.RoamingNetworkChange.Remove(P1) ], CreatedAt: T0)).IsCommitted);

            // ...is removed by the next one at its start; what is still in the head stays.
            await using var next = new POI3ToOCPIPublisher(history, api);
            next.OnError += (sender, description, warnings) => { lock (errors) errors.Add(description); };

            await next.StartAsync();

            Assert.Multiple(() => {
                Assert.That(api.TryGetLocation(Party, locationId, out _),  Is.False, "The charging pool removed while no publisher ran is still a location.");
                Assert.That(api.TryGetTariff  (Party, tariffId,   out _),  Is.True);
                Assert.That(errors,                                         Is.Empty);
            });

        }

        #endregion

        #region TheImporterRemovesWhatOCPIRemoves()

        [Test]
        public async Task TheImporterRemovesWhatOCPIRemoves()
        {

            var history    = POI.RoamingNetworkHistory.Start(ANetwork(WithPools: false));
            var errors     = new List<String>();

            await using var importer = new OCPIToPOI3Importer(api, history, changeSet => changeSet.SignedByAll(history.Head.RoamingNetwork));
            importer.OnError += (sender, description, warnings) => {
                lock (errors)
                    errors.Add(description + " " + String.Join(" ", warnings.Select(warning => warning.Text.FirstText())));
            };

            importer.Start();

            var source     = ANetwork();
            var tariff     = source.GetChargingStationOperatorById(GEF)!.ChargingTariffs.Single().ToOCPI([])!;
            var location   = source.GetChargingStationOperatorById(GEF)!.ChargingPools.  Single().ToOCPI(source, [])!;

            Assert.That((await api.AddOrUpdateTariff  (tariff)).  IsSuccess);
            Assert.That((await api.AddOrUpdateLocation(location)).IsSuccess);

            POI.ChargingStationOperator Operator()
                => history.Head.RoamingNetwork.GetChargingStationOperatorById(GEF)!;

            Assert.That(await Eventually(() => Operator().EVSEs.Any()), Is.True, String.Join("\n", errors));

            // An EVSE stored as removed...
            Assert.That(api.TryGetLocation(Party, location.Id, out var stored), Is.True);

            var removed    = new EVSE(stored!.EVSEs.Single().UId,
                                      StatusType.REMOVED,
                                      stored.EVSEs.Single().Connectors,
                                      stored.EVSEs.Single().EVSEId,
                                      LastUpdated: T0);

            Assert.That((await api.AddOrUpdateEVSE(stored, removed)).IsSuccess);

            Assert.That(await Eventually(() => !Operator().EVSEs.Any()), Is.True, "The removed EVSE was not removed. " + String.Join("\n", errors));

            // ...a removed location...
            Assert.That((await api.RemoveLocation(Party, location.Id)).IsSuccess);

            Assert.That(await Eventually(() => !Operator().ChargingPools.Any()), Is.True, "The removed location was not removed. " + String.Join("\n", errors));

            // ...and a removed tariff.
            Assert.That((await api.RemoveTariff(Party, tariff.Id)).IsSuccess);

            Assert.That(await Eventually(() => !Operator().ChargingTariffs.Any()), Is.True, "The removed tariff was not removed. " + String.Join("\n", errors));

            lock (errors)
                Assert.That(errors, Is.Empty);

        }

        #endregion

        #region ARoundTripCommitsNothing()

        [Test]
        public async Task ARoundTripCommitsNothing()
        {

            // What a POI3 network publishes, imported back into it: nothing changed, nothing is committed,
            // although OCPI joins the house number with the street, has no language of the city, one text for
            // both the name and the description of a tariff, and tariffs of connectors only, not of EVSE groups.
            var network    = ANetwork(WithPOI3Only: true);
            var history    = POI.RoamingNetworkHistory.Start(network);
            var revision   = history.Head.Commit.Revision;
            var errors     = new List<String>();

            await using var importer = new OCPIToPOI3Importer(api, history, changeSet => changeSet.SignedByAll(history.Head.RoamingNetwork));
            importer.OnError += (sender, description, warnings) => {
                lock (errors)
                    errors.Add(description + " " + String.Join(" ", warnings.Select(warning => warning.Text.FirstText())));
            };

            importer.Start();

            var warnings   = new List<Warning>();
            var tariff     = network.GetChargingStationOperatorById(GEF)!.ChargingTariffs.Single().ToOCPI(warnings)!;
            var location   = network.GetChargingStationOperatorById(GEF)!.ChargingPools.  Single().ToOCPI(network, warnings)!;

            await importer.ImportAsync(tariff);
            await importer.ImportAsync(location);

            Assert.Multiple(() => {
                Assert.That(location.EVSEs.Single().Connectors.Single().TariffIds, Is.EqualTo(new[] { tariff.Id }));
                Assert.That(warnings,                                              Is.Empty);
                Assert.That(errors,                                                Is.Empty);
                Assert.That(history.Head.Commit.Revision,                          Is.EqualTo(revision),
                            String.Join(", ", history.Head.Commit.ChangeSet?.Changes.Select(change => $"{change.Kind} {change.EntityId} {change.PropertyName}") ?? []));
            });

        }

        #endregion

        #region WhatGroupsGiveIsPublishedAndImportedBackUnchanged()

        [Test]
        public async Task WhatGroupsGiveIsPublishedAndImportedBackUnchanged()
        {

            // The charging station is reservable and labelled, the charging pool has a cafe, by their groups only.
            var history    = POI.RoamingNetworkHistory.Start(ANetwork(FeaturesByGroup: true));
            var revision   = history.Head.Commit.Revision;
            var errors     = new List<String>();

            await using (var publisher = new POI3ToOCPIPublisher(history, api))
            {

                publisher.OnError += (sender, description, warnings) => { lock (errors) errors.Add(description); };

                await publisher.StartAsync();

            }

            Assert.That(api.TryGetLocation(Party, Location_Id.Parse("1"), out var location),  Is.True);

            // Imported back: what the location tells alike stays in the group.
            await using (var importer = new OCPIToPOI3Importer(api, history, changeSet => changeSet.SignedByAll(history.Head.RoamingNetwork)))
            {

                importer.OnError += (sender, description, warnings) => { lock (errors) errors.Add(description); };
                importer.Start();

                await importer.ImportAsync(location!);

            }

            var op = history.Head.RoamingNetwork.GetChargingStationOperatorById(GEF)!;

            Assert.Multiple(() => {
                Assert.That(location!.EVSEs.Single().Capabilities,                 Does.Contain(Capability.RESERVABLE));
                Assert.That(location.EVSEs.Single().PhysicalReference,             Is.EqualTo("A"));
                Assert.That(location.Facilities,                                   Does.Contain(Facility.CAFE));
                Assert.That(op.ChargingPools.Single().Facilities,                  Is.Empty);
                Assert.That(errors,                                                Is.Empty);
                Assert.That(history.Head.Commit.Revision,                          Is.EqualTo(revision),
                            String.Join(", ", history.Head.Commit.ChangeSet?.Changes.Select(change => $"{change.Kind} {change.EntityId} {change.PropertyName}") ?? []));
                Assert.That(op.ChargingStations.Single().Features,                 Is.Empty);
                Assert.That(op.ChargingStations.Single().PhysicalReference,        Is.Null);
            });

        }

        #endregion

        #region AnImportKeepsWhatOnlyPOI3Knows()

        [Test]
        public async Task AnImportKeepsWhatOnlyPOI3Knows()
        {

            var network    = ANetwork(WithPOI3Only: true);
            var history    = POI.RoamingNetworkHistory.Start(network);
            var revision   = history.Head.Commit.Revision;
            var errors     = new List<String>();

            await using var importer = new OCPIToPOI3Importer(api, history, changeSet => changeSet.SignedByAll(history.Head.RoamingNetwork));
            importer.OnError += (sender, description, warnings) => {
                lock (errors)
                    errors.Add(description + " " + String.Join(" ", warnings.Select(warning => warning.Text.FirstText())));
            };

            importer.Start();

            // A location renamed, and with less power.
            var changed    = APool(MaxPower: 11000, Name: "Biberweg Nord").ToOCPI(network, [], T0)!;

            await importer.ImportAsync(changed);

            var op         = history.Head.RoamingNetwork.GetChargingStationOperatorById(GEF)!;
            var pool       = op.ChargingPools.Single();

            Assert.Multiple(() => {

                Assert.That(errors,                                                Is.Empty);
                Assert.That(history.Head.Commit.Revision,                          Is.EqualTo(revision + 1));
                Assert.That(history.Head.Commit.ChangeSet?.Changes.Select(change => change.PropertyName),
                            Is.EquivalentTo(new[] { "name", "maxPower" }));

                Assert.That(pool.Name?.FirstText(),                                Is.EqualTo("Biberweg Nord"));
                Assert.That(op.EVSEs.Single().MaxPower?.Value,                     Is.EqualTo(11000));

                Assert.That(pool.Description?.FirstText(),                         Is.EqualTo("Hinter dem Haus"));
                Assert.That(pool.Address?.Street,                                  Is.EqualTo("Biberweg"));
                Assert.That(pool.Address?.HouseNumber,                             Is.EqualTo("18"));
                Assert.That(pool.Address?.City.FirstText(),                        Is.EqualTo("Jena"));
                Assert.That(pool.AuthenticationModes.Length,                       Is.EqualTo(2));
                Assert.That(op.ChargingStations.Single().Features,                 Is.EqualTo(new[] { POI.ChargingStationFeature.Reservable }));

            });

        }

        #endregion


    }

}
