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

using System.Text;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

using POI = cloud.charging.open.protocols.WWCP.POI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.POI3
{

    /// <summary>
    /// The mapping between the charging infrastructure and tariffs of a POI3 roaming network and OCPI 2.3.0:
    /// charging pools and locations, charging stations and EVSEs, charging connectors and connectors,
    /// charging tariffs and tariffs, and the status of EVSEs. See README.md for what does not map.
    /// </summary>
    public static class POI3Mapper
    {

        #region Identifications

        /// <summary>
        /// The OCPI party of the given operator: DE*GEF is the country code DE and the party GEF.
        /// </summary>
        public static Party_Idv3 ToOCPIParty(this POI.ChargingStationOperator_Id OperatorId)

            => OperatorId.ToOCPI_PartyIdv3();

        /// <summary>
        /// The operator of the given OCPI party, unless its party identification has not the three characters an operator has.
        /// </summary>
        public static POI.ChargingStationOperator_Id? ToPOI3Operator(CountryCode  CountryCode,
                                                                    Party_Id     PartyId)

            => POI.ChargingStationOperator_Id.TryParse($"{CountryCode}*{PartyId}");

        /// <summary>
        /// The location of the given charging pool: DE*GEF*P1 is the location 1.
        /// </summary>
        public static Location_Id? ToOCPILocationId(this POI.ChargingPool_Id ChargingPoolId)

            => Location_Id.TryParse(ChargingPoolId.Suffix);

        /// <summary>
        /// The charging pool of the given location of the given operator, unless the location identification does not fit one.
        /// </summary>
        public static POI.ChargingPool_Id? ToPOI3PoolId(POI.ChargingStationOperator_Id  OperatorId,
                                                       Location_Id                     LocationId)

            => POI.ChargingPool_Id.TryParse(OperatorId, LocationId.ToString());

        /// <summary>
        /// The tariff of the given charging tariff: DE*GEF*T1 is the tariff 1.
        /// </summary>
        public static Tariff_Id? ToOCPITariffId(this POI.ChargingTariff_Id ChargingTariffId)

            => Tariff_Id.TryParse(ChargingTariffId.Suffix);

        /// <summary>
        /// The charging tariff of the given tariff of the given operator, unless the tariff identification does not fit one.
        /// </summary>
        public static POI.ChargingTariff_Id? ToPOI3TariffId(POI.ChargingStationOperator_Id  OperatorId,
                                                           Tariff_Id                       TariffId)

            => POI.ChargingTariff_Id.TryParse(OperatorId, TariffId.ToString());

        /// <summary>
        /// Between the tariff of a charging tariff and the hash of new prices of it: no charging tariff identification has it.
        /// </summary>
        public const Char PricesSeparator = '~';

        /// <summary>
        /// The tariff of the given charging tariff with the given new prices of its price status: DE*GEF*T1 with prices
        /// is the tariff 1~{the first 8 hex digits of the SHA-256 of their elements}, the same for the same prices.
        /// Without prices, the tariff of the charging tariff.
        /// </summary>
        public static Tariff_Id? ToOCPITariffId(this POI.ChargingTariff_Id     ChargingTariffId,
                                                POI.ChargingTariffPrices?      Prices)

            => Prices is null
                   ? ChargingTariffId.ToOCPITariffId()
                   : Tariff_Id.TryParse($"{ChargingTariffId.Suffix}{PricesSeparator}{PricesHash(Prices.Elements)}");

        /// <summary>
        /// Whether the given tariff is one of a charging tariff with new prices (see ToOCPITariffId(Prices)).
        /// </summary>
        public static Boolean HasPrices(this Tariff_Id TariffId)

            => TariffId.ToString().LastIndexOf(PricesSeparator) > 0;

        /// <summary>
        /// The charging tariff of the given tariff of the given operator, also of one with new prices: 1~… is DE*GEF*T1.
        /// </summary>
        public static POI.ChargingTariff_Id? ToPOI3TariffIdOf(POI.ChargingStationOperator_Id  OperatorId,
                                                             Tariff_Id                       TariffId)
        {

            var text       = TariffId.ToString();
            var separator  = text.LastIndexOf(PricesSeparator);

            return separator > 0
                       ? POI.ChargingTariff_Id.TryParse(OperatorId, text[..separator])
                       : ToPOI3TariffId(OperatorId, TariffId);

        }

        private static String PricesHash(IEnumerable<POI.ChargingTariffElement> Elements)

            => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(ElementsText(Elements))))[..8];

        /// <summary>
        /// The text of the given charging tariff elements: their compact JSON, equal for equal elements.
        /// </summary>
        public static String ElementsText(IEnumerable<POI.ChargingTariffElement> Elements)

            => new JArray(Elements.Select(element => element.ToJSON())).ToString(Newtonsoft.Json.Formatting.None);

        #endregion

        #region Status

        /// <summary>
        /// The OCPI status of an EVSE: its admin status where it is not in operation, its status otherwise.
        /// </summary>
        public static StatusType ToOCPIStatus(POI.EVSEStatusType       Status,
                                              POI.EVSEAdminStatusType  AdminStatus)

            => AdminStatus.ToString().ToLowerInvariant() switch {
                   "planned"       => StatusType.PLANNED,
                   "indeployment"  => StatusType.PLANNED,
                   "deleted"       => StatusType.REMOVED,
                   "outofservice"  => StatusType.INOPERATIVE,
                   "blocked"       => StatusType.BLOCKED,
                   _               => ToOCPIStatus(Status)
               };

        /// <summary>
        /// The OCPI status of the given EVSE status.
        /// </summary>
        public static StatusType ToOCPIStatus(this POI.EVSEStatusType Status)

            => Status.ToString().ToLowerInvariant() switch {
                   "available"         => StatusType.AVAILABLE,
                   "waitingforplugin"  => StatusType.AVAILABLE,
                   "reserved"          => StatusType.RESERVED,
                   "charging"          => StatusType.CHARGING,
                   "pluggedin"         => StatusType.CHARGING,
                   "occupied"          => StatusType.CHARGING,
                   "blocked"           => StatusType.BLOCKED,
                   "offline"           => StatusType.INOPERATIVE,
                   "unavailable"       => StatusType.INOPERATIVE,
                   "inoperative"       => StatusType.INOPERATIVE,
                   "outofservice"      => StatusType.INOPERATIVE,
                   "doornotclosed"     => StatusType.INOPERATIVE,
                   "error"             => StatusType.OUTOFORDER,
                   "faulted"           => StatusType.OUTOFORDER,
                   "outoforder"        => StatusType.OUTOFORDER,
                   "indeployment"      => StatusType.PLANNED,
                   "removed"           => StatusType.REMOVED,
                   _                   => StatusType.UNKNOWN
               };

        /// <summary>
        /// The EVSE status of the given OCPI status.
        /// </summary>
        public static POI.EVSEStatusType ToPOI3Status(this StatusType Status)

            => POI.EVSEStatusType.Parse(Status.ToString() switch {
                   "AVAILABLE"    => "available",
                   "BLOCKED"      => "blocked",
                   "CHARGING"     => "charging",
                   "INOPERATIVE"  => "inoperative",
                   "OUTOFORDER"   => "outOfOrder",
                   "PLANNED"      => "inDeployment",
                   "REMOVED"      => "removed",
                   "RESERVED"     => "reserved",
                   _              => "unknown"
               });

        #endregion


        #region ToOCPI(ChargingPool, RoamingNetwork, Warnings)

        /// <summary>
        /// The OCPI location of the given charging pool with its EVSEs, or null when it has no address or
        /// geo location, which OCPI requires.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="RoamingNetwork">The roaming network of the charging pool: for its operator, brands and groups.</param>
        /// <param name="Warnings">Where to tell what could not be mapped.</param>
        /// <param name="NotBefore">The earliest 'last updated' of the location, e.g. after the one published before.</param>
        public static Location? ToOCPI(this POI.ChargingPool    ChargingPool,
                                       POI.RoamingNetwork       RoamingNetwork,
                                       ICollection<Warning>     Warnings,
                                       DateTimeOffset?          NotBefore   = null)
        {

            if (!RoamingNetwork.TryGetChargingStationOperatorById(ChargingPool.Id.OperatorId, out var chargingStationOperator))
            {
                Warnings.Add(Warning.Create($"The operator of the charging pool '{ChargingPool.Id}' is unknown!"));
                return null;
            }

            var locationId = ChargingPool.Id.ToOCPILocationId();

            if (!locationId.HasValue)
            {
                Warnings.Add(Warning.Create($"The charging pool identification '{ChargingPool.Id}' is no OCPI location identification!"));
                return null;
            }

            if (ChargingPool.Address is null)
            {
                Warnings.Add(Warning.Create($"The charging pool '{ChargingPool.Id}' has no address, which an OCPI location requires!"));
                return null;
            }

            if (!ChargingPool.GeoLocation.HasValue)
            {
                Warnings.Add(Warning.Create($"The charging pool '{ChargingPool.Id}' has no geo location, which an OCPI location requires!"));
                return null;
            }

            var party         = ChargingPool.Id.OperatorId.ToOCPIParty();
            var evses         = new List<EVSE>();
            var lastUpdated   = ChargingPool.LastChangeDate;

            foreach (var chargingStation in ChargingPool.ChargingStations)
            {

                if (chargingStation.LastChangeDate > lastUpdated)
                    lastUpdated = chargingStation.LastChangeDate;

                foreach (var evse in chargingStation.EVSEs)
                {

                    var ocpiEVSE = evse.ToOCPI(chargingStation, ChargingPool, chargingStationOperator, Warnings);

                    if (ocpiEVSE is not null)
                    {
                        evses.Add(ocpiEVSE);
                        if (ocpiEVSE.LastUpdated > lastUpdated)
                            lastUpdated = ocpiEVSE.LastUpdated;
                    }

                }

            }

            if (NotBefore.HasValue && NotBefore.Value > lastUpdated)
                lastUpdated = NotBefore.Value;

            // The first brand the pool also appears under: its own, not its operator's white label.
            var brand = ChargingPool.Brands.FirstOrDefault();

            return new Location(

                       CountryCode:         party.CountryCode,
                       PartyId:             party.PartyId,
                       Id:                  locationId.Value,
                       Publish:             true,
                       Address:             ChargingPool.Address.HouseNumber.IsNotNullOrEmpty()
                                                ? $"{ChargingPool.Address.Street} {ChargingPool.Address.HouseNumber}"
                                                :    ChargingPool.Address.Street,
                       City:                ChargingPool.Address.City.FirstText(),
                       Country:             ChargingPool.Address.Country,
                       Coordinates:         ChargingPool.GeoLocation.Value,
                       TimeZone:            ChargingPool.TimeZone?.ToString() ?? ChargingPool.Address.TimeZone?.ToString() ?? "UTC",

                       Name:                FirstText(ChargingPool.Name),
                       PostalCode:          ChargingPool.Address.PostalCode,
                       State:               ChargingPool.Address.Region,
                       RelatedLocations:    ChargingPool.RelatedLocations.Select(relatedLocation => new OCPI.AdditionalGeoLocation(
                                                                                                         relatedLocation.GeoLocation,
                                                                                                         relatedLocation.Name is not null && relatedLocation.Name.Any()
                                                                                                             ? ToDisplayTexts(relatedLocation.Name).First()
                                                                                                             : null
                                                                                                     )),
                       ParkingType:         ChargingPool.ParkingType.HasValue ? ChargingPool.ParkingType.Value.ToOICP() : null,
                       EVSEs:               evses,
                       Directions:          ToDisplayTexts(ChargingPool.ArrivalInstructions),
                       Operator:            new BusinessDetails(
                                                FirstText(chargingStationOperator.Name) ?? chargingStationOperator.Id.ToString(),
                                                chargingStationOperator.Homepage,
                                                chargingStationOperator.Logo.HasValue
                                                    ? new Image(chargingStationOperator.Logo.Value, ImageFileTypeOf(chargingStationOperator.Logo.Value), ImageCategory.OPERATOR)
                                                    : null
                                            ),
                       SubOperator:         brand is not null
                                                ? new BusinessDetails(
                                                      FirstText(brand.Name) ?? brand.Id.ToString(),
                                                      brand.Homepage,
                                                      brand.Logo.HasValue
                                                          ? new Image(brand.Logo.Value, ImageFileTypeOf(brand.Logo.Value), ImageCategory.NETWORK)
                                                          : null
                                                  )
                                                : null,
                       Facilities:          ChargingPool.Facilities.ToOCPI(),
                       OpeningTimes:        ChargingPool.OpeningTimes is not null
                                                ? ((OpeningTimes) ChargingPool.OpeningTimes).ToOCPI()
                                                : null,
                       ChargingWhenClosed:  ChargingPool.ChargingWhenClosed,
                       Images:              ChargingPool.Images.Select(ToOCPI),
                       ParkingPlaces:       ParkingSpacesOf(ChargingPool, RoamingNetwork).Select(ToOCPI),
                       EnergyMix:           ChargingPool.EnergyMix?.ToOCPI($"charging pool '{ChargingPool.Id}'", Warnings),
                       HelpPhone:           ChargingPool.HotlinePhoneNumber ?? chargingStationOperator.HotlinePhoneNumber,
                       Services:                  ChargingPool.Services. Select(service  => LocationService.Parse(service. ToString())),
                       AssistanceServiceDetails:  FirstText(ChargingPool.AssistanceServiceDetails),
                       Standards:                 ChargingPool.Standards.Select(standard => Standard.       Parse(standard.ToString())),

                       Created:             ChargingPool.Created,
                       LastUpdated:         lastUpdated

                   );

        }

        #endregion

        #region ToOCPI(EVSE, ChargingStation, ChargingPool, Operator, Warnings)

        /// <summary>
        /// The OCPI EVSE of the given EVSE of the given charging station, or null when it has no charging connectors,
        /// which OCPI requires.
        /// </summary>
        public static EVSE? ToOCPI(this POI.EVSE                     EVSE,
                                   POI.ChargingStation               ChargingStation,
                                   POI.ChargingPool                  ChargingPool,
                                   POI.ChargingStationOperator       Operator,
                                   ICollection<Warning>              Warnings,
                                   DateTimeOffset?                   NotBefore   = null)
        {

            var uid = EVSE_UId.TryParse(EVSE.Id.ToString());

            if (!uid.HasValue)
            {
                Warnings.Add(Warning.Create($"The EVSE identification '{EVSE.Id}' is no OCPI EVSE identification!"));
                return null;
            }

            if (EVSE.ChargingConnectors.Length == 0)
            {
                Warnings.Add(Warning.Create($"The EVSE '{EVSE.Id}' has no charging connectors, which an OCPI EVSE requires!"));
                return null;
            }

            // The tariffs with the prices in effect: since then, the connectors name other tariffs.
            var prices          = PricesOf(EVSE, ChargingStation, ChargingPool, Operator);
            var pricesChanged   = prices.Select(price => price.Timestamp).Append(EVSE.LastChangeDate).Max();
            var tariffIds       = TariffIdsOf(EVSE, prices, Operator).ToArray();
            var connectors      = new List<Connector>();

            foreach (var chargingConnector in EVSE.ChargingConnectors)
            {
                var connector = chargingConnector.ToOCPI(EVSE, tariffIds, Operator, Warnings, pricesChanged);
                if (connector is not null)
                    connectors.Add(connector);
            }

            if (connectors.Count == 0)
                return null;

            var lastUpdated = new[] { pricesChanged, EVSE.Status.Timestamp, EVSE.AdminStatus.Timestamp, NotBefore ?? DateTimeOffset.MinValue }.Max();

            return new EVSE(

                       UId:                  uid.Value,
                       Status:               ToOCPIStatus(EVSE.Status.Value, EVSE.AdminStatus.Value),
                       StatusSchedule:       StatusScheduleOf(EVSE),
                       Connectors:           connectors,
                       EVSEId:               EVSE_Id.TryParse(EVSE.Id.ToString()),
                       Capabilities:         CapabilitiesOf(ChargingStation, ChargingPool),
                       Coordinates:          EVSE.GeoLocation ?? ChargingStation.GeoLocation ?? ChargingPool.GeoLocation,
                       PhysicalReference:    EVSE.PhysicalReference ?? ChargingStation.PhysicalReference,
                       ParkingRestrictions:  ParkingRestrictionsOf(ChargingPool),
                       Parking:              ChargingStation.ParkingSpaceIds.Select(parkingSpaceId => new EVSEParking(Parking_Id.Parse(parkingSpaceId.ToString()))),
                       Images:               EVSE.Images.Concat(ChargingStation.Images).Select(ToOCPI).Distinct(),

                       Created:              EVSE.Created,
                       LastUpdated:          lastUpdated,

                       ReachDistance:             EVSE.ReachDistance,
                       OperationTimeout:          EVSE.OperationTimeout,
                       ExtendedOperationTimeout:  EVSE.ExtendedOperationTimeout,
                       Standards:                 EVSE.Standards.Select(standard => Standard.Parse(standard.ToString())),

                       FloorLevel:                EVSE.FloorLevel,
                       AcceptedServiceProviders:  ToOCPI(EVSE.AcceptedEMobilityProviderIds, EVSE.Id, Warnings)

                   );

        }

        #endregion

        #region ToOCPI(ChargingConnector, EVSE, TariffIds, Operator, Warnings, LastUpdated = null)

        /// <summary>
        /// The OCPI connector of the given charging connector: its plug type, and the current type and maximum voltage,
        /// current and power of its EVSE.
        /// </summary>
        /// <param name="LastUpdated">Its 'last updated', when later than the last change of the EVSE, e.g. new prices of its tariffs.</param>
        public static Connector? ToOCPI(this POI.ChargingConnector      ChargingConnector,
                                        POI.EVSE                        EVSE,
                                        IEnumerable<Tariff_Id>          TariffIds,
                                        POI.ChargingStationOperator     Operator,
                                        ICollection<Warning>            Warnings,
                                        DateTimeOffset?                 LastUpdated   = null)
        {

            var connectorId  = Connector_Id.TryParse(ChargingConnector.Id.ToString());
            ConnectorType? standard;

            try
            {
                standard = ChargingConnector.Type.ToOCPI();
            }
            catch
            {
                standard = null;
            }

            if (!connectorId.HasValue || !standard.HasValue)
            {
                Warnings.Add(Warning.Create($"The charging connector '{ChargingConnector.Id}' of the EVSE '{EVSE.Id}' has no OCPI identification or plug type ('{ChargingConnector.Type}')!"));
                return null;
            }

            var isDC         = EVSE.CurrentTypes.HasFlag(POI.CurrentTypes.DC);
            var powerType    = isDC                                              ? PowerTypes.DC
                             : EVSE.CurrentTypes.HasFlag(POI.CurrentTypes.AC_ThreePhases) ? PowerTypes.AC_3_PHASE
                             : EVSE.CurrentTypes.HasFlag(POI.CurrentTypes.AC_OnePhase)    ? PowerTypes.AC_1_PHASE
                             : PowerTypes.UNKNOWN;

            return new Connector(
                       Id:                     connectorId.Value,
                       Standard:               standard.Value,
                       Format:                 ChargingConnector.Format == POI.ChargingConnectorFormat.CABLE ? ConnectorFormats.CABLE : ConnectorFormats.SOCKET,
                       PowerType:              powerType,
                       MaxVoltage:             EVSE.MaxVoltage ?? (isDC ? Volt.  FromV(400) : Volt.  FromV(230)),
                       MaxAmperage:            EVSE.MaxCurrent ?? (isDC ? Ampere.FromA(50)  : Ampere.FromA(16)),
                       MaxElectricPower:       EVSE.MaxPower,
                       TariffIds:              TariffIds,
                       TermsAndConditionsURL:  Operator.TermsAndConditionsURL,
                       Created:                EVSE.Created,
                       LastUpdated:            LastUpdated > EVSE.LastChangeDate ? LastUpdated.Value : EVSE.LastChangeDate,
                       CableLength:            ChargingConnector.CableLength,
                       CableWeight:            ChargingConnector.CableWeight,
                       CableManagementSystem:  ChargingConnector.CableManagementSystem,
                       Standards:              ChargingConnector.Standards.Select(standard => Standard.Parse(standard.ToString())),
                       Images:                 ChargingConnector.Images.Select(ToOCPI)
                   );

        }

        #endregion

        #region ToOCPI(ChargingTariff, Warnings, NotBefore = null, Prices = null)

        /// <summary>
        /// The OCPI tariff of the given charging tariff, or null when it has price components OCPI 2.3.0 does not know.
        /// With new prices of it, e.g. of the price status of a charging pool, a tariff of its own: their elements
        /// instead of the tariff's, the rest as the tariff has it (see ToOCPITariffId(Prices)).
        /// </summary>
        /// <param name="NotBefore">The earliest 'last updated' of the tariff, e.g. after the one published before.</param>
        /// <param name="Prices">New prices of the charging tariff, with since when.</param>
        public static Tariff? ToOCPI(this POI.ChargingTariff                    ChargingTariff,
                                     ICollection<Warning>                       Warnings,
                                     DateTimeOffset?                            NotBefore   = null,
                                     Timestamped<POI.ChargingTariffPrices>?     Prices      = null)
        {

            if (Prices.HasValue && Prices.Value.Value.ChargingTariffId != ChargingTariff.Id)
                throw new ArgumentException($"The prices of the charging tariff '{Prices.Value.Value.ChargingTariffId}' are not those of '{ChargingTariff.Id}'!", nameof(Prices));

            var tariffId = ChargingTariff.Id.ToOCPITariffId(Prices?.Value);

            if (!tariffId.HasValue)
            {
                Warnings.Add(Warning.Create($"The charging tariff identification '{ChargingTariff.Id}' is no OCPI tariff identification!"));
                return null;
            }

            var elements     = new List<TariffElement>();
            var lastUpdated  = new[] { ChargingTariff.LastChangeDate, Prices?.Timestamp ?? DateTimeOffset.MinValue, NotBefore ?? DateTimeOffset.MinValue }.Max();

            foreach (var element in Prices?.Value.Elements ?? ChargingTariff.Elements)
            {

                var priceComponents = new List<PriceComponent>();

                foreach (var priceComponent in element.PriceComponents)
                {

                    var dimension = priceComponent.Type switch {
                                        POI.ChargingDimensionTypes.ENERGY        => TariffDimension.ENERGY,
                                        POI.ChargingDimensionTypes.FLAT          => TariffDimension.FLAT,
                                        POI.ChargingDimensionTypes.TIME          => TariffDimension.TIME,
                                        POI.ChargingDimensionTypes.PARKING_TIME  => TariffDimension.PARKING_TIME,
                                        _                                        => (TariffDimension?) null
                                    };

                    if (!dimension.HasValue)
                    {
                        Warnings.Add(Warning.Create($"The price component '{priceComponent.Type}' of the charging tariff '{ChargingTariff.Id}' has no OCPI 2.3.0 dimension!"));
                        return null;
                    }

                    priceComponents.Add(new PriceComponent(dimension.Value,
                                                           priceComponent.Price,
                                                           priceComponent.VAT,
                                                           (UInt32) Math.Max(1, priceComponent.StepSize ?? 1)));

                }

                elements.Add(new TariffElement(priceComponents, element.Restrictions?.ToOCPI()));

            }

            var party = ChargingTariff.Id.OperatorId.ToOCPIParty();

            return new Tariff(
                       CountryCode:    party.CountryCode,
                       PartyId:        party.PartyId,
                       Id:             tariffId.Value,
                       Currency:       ChargingTariff.Currency,
                       TariffElements: elements,
                       // POI3 prices exclude VAT.
                       TaxIncluded:    TaxIncluded.No,
                       TariffType:     ChargingTariff.TariffType.HasValue ? TariffType.Parse(ChargingTariff.TariffType.Value.ToString()) : null,
                       TariffAltText:  ToDisplayTexts(ChargingTariff.Name).Concat(ToDisplayTexts(ChargingTariff.Description)),
                       TariffAltURL:   ChargingTariff.TariffURL,
                       MinPrice:       ChargingTariff.MinPrice?.ToOCPI(),
                       MaxPrice:       ChargingTariff.MaxPrice?.ToOCPI(),
                       Start:          ChargingTariff.ValidFrom,
                       End:            ChargingTariff.ValidUntil,
                       EnergyMix:      ChargingTariff.EnergyMix?.ToOCPI($"charging tariff '{ChargingTariff.Id}'", Warnings),
                       Created:        ChargingTariff.Created,
                       LastUpdated:    lastUpdated
                   );

        }

        /// <summary>
        /// The price excluding VAT of the given OCPI price: one including taxes is divided by its VAT, if any applies,
        /// and rounded to six decimal places; one excluding taxes, or without any, is as it is.
        /// </summary>
        public static Decimal NetPrice(Decimal      Price,
                                       Decimal?     VAT,
                                       TaxIncluded  TaxIncluded)

            => TaxIncluded == TaxIncluded.Yes && VAT > 0
                   ? Math.Round(Price / (1 + VAT.Value / 100), 6)
                   : Price;

        /// <summary>
        /// The OCPI price limit of the given price limit: before taxes is excluding VAT, after taxes including it.
        /// </summary>
        public static PriceLimit ToOCPI(this POI.ChargingPriceLimit PriceLimit)

            => new (PriceLimit.ExcludingVAT,
                    PriceLimit.IncludingVAT);

        /// <summary>
        /// The price limit of the given OCPI price limit: before taxes is excluding VAT, after taxes including it.
        /// </summary>
        public static POI.ChargingPriceLimit ToPOI3(this PriceLimit PriceLimit)

            => new (PriceLimit.BeforeTaxes,
                    PriceLimit.AfterTaxes);

        /// <summary>
        /// The OCPI restrictions of the given charging tariff restrictions: energy in Wh becomes kWh, power in W becomes kW.
        /// </summary>
        public static TariffRestrictions ToOCPI(this POI.ChargingTariffRestrictions Restrictions)

            => new (
                   StartTime:    Restrictions.StartTime.HasValue ? Time.FromHourMin((Byte) Restrictions.StartTime.Value.Hour, (Byte) Restrictions.StartTime.Value.Minute) : null,
                   EndTime:      Restrictions.EndTime.  HasValue ? Time.FromHourMin((Byte) Restrictions.EndTime.  Value.Hour, (Byte) Restrictions.EndTime.  Value.Minute) : null,
                   StartDate:    Restrictions.StartDate.HasValue ? new DateTimeOffset(Restrictions.StartDate.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero) : null,
                   EndDate:      Restrictions.EndDate.  HasValue ? new DateTimeOffset(Restrictions.EndDate.  Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero) : null,
                   MinkWh:       Restrictions.MinEnergy / 1000,
                   MaxkWh:       Restrictions.MaxEnergy / 1000,
                   MinCurrent:   Restrictions.MinCurrent,
                   MaxCurrent:   Restrictions.MaxCurrent,
                   MinPower:     Restrictions.MinPower  / 1000,
                   MaxPower:     Restrictions.MaxPower  / 1000,
                   MinDuration:  Restrictions.MinDuration.HasValue ? TimeSpan.FromSeconds((Double) Restrictions.MinDuration.Value) : null,
                   MaxDuration:  Restrictions.MaxDuration.HasValue ? TimeSpan.FromSeconds((Double) Restrictions.MaxDuration.Value) : null,
                   DayOfWeek:    Restrictions.DaysOfWeek,
                   Reservation:  Restrictions.Reservation switch {
                                     POI.ChargingReservationRestriction.RESERVATION          => ReservationRestrictions.RESERVATION,
                                     POI.ChargingReservationRestriction.RESERVATION_EXPIRES  => ReservationRestrictions.RESERVATION_EXPIRES,
                                     _                                                       => null
                                 }
               );

        #endregion


        #region ToPOI3(Location, Operator, Existing, Language, Warnings, Unchanged = null, Effective = null)

        /// <summary>
        /// The charging pool of the given OCPI location, as JSON: the existing one, if any, with what OCPI says about it.
        /// What only POI3 knows is kept, and so is what OCPI tells as it would tell the existing charging pool, e.g. a house
        /// number apart from its street. EVSEs stay in their charging stations, new ones go into the first charging station
        /// of the pool, or into a new one named after the location. EVSEs whose status is REMOVED are gone.
        /// What it tells of the pool and its EVSEs as their groups give it stays in their groups: see
        /// POIGroupMembers.KeepWhatGroupsGive(…).
        /// </summary>
        /// <param name="Location">An OCPI location.</param>
        /// <param name="OperatorId">The operator of the location.</param>
        /// <param name="Existing">The JSON of the existing charging pool, if any.</param>
        /// <param name="Language">The language of texts OCPI gives without one.</param>
        /// <param name="Warnings">Where to tell what could not be mapped.</param>
        /// <param name="Unchanged">The existing charging pool as OCPI tells it, if any: what the location tells alike is kept as it is.</param>
        /// <param name="Effective">The JSON of the existing charging pool as it is at runtime, its effective one, with the
        /// properties its groups and those of its EVSEs give them, if any.</param>
        public static JObject? ToPOI3(this Location                    Location,
                                      POI.ChargingStationOperator_Id   OperatorId,
                                      JObject?                         Existing,
                                      Languages                        Language,
                                      ICollection<Warning>             Warnings,
                                      Location?                        Unchanged   = null,
                                      JObject?                         Effective   = null)
        {

            var poolId = ToPOI3PoolId(OperatorId, Location.Id);

            if (!poolId.HasValue)
            {
                Warnings.Add(Warning.Create($"The location identification '{Location.Id}' is no charging pool identification!"));
                return null;
            }

            #region The charging pool as OCPI tells it

            var evses        = Location.EVSEs.Where(evse => evse.Status != StatusType.REMOVED).ToArray();
            var mappedEVSEs  = new List<POI.EVSE>();

            foreach (var evse in evses)
            {
                var mappedEVSE = evse.ToPOI3(OperatorId, Location.LastUpdated, Warnings);
                if (mappedEVSE is not null)
                    mappedEVSEs.Add(mappedEVSE);
            }

            var address   = new Address(
                                Street:      Location.Address,
                                PostalCode:  Location.PostalCode ?? "",
                                City:        I18NString.Create(Language, Location.City),
                                Country:     Location.Country,
                                Region:      Location.State
                            );

            var capabilities = evses.SelectMany(evse => evse.Capabilities).ToHashSet();

            var mapped    = new POI.ChargingPool(
                                Id:                   poolId.Value,
                                Name:                 Location.Name is not null ? I18NString.Create(Language, Location.Name) : null,
                                Address:              address,
                                GeoLocation:          Location.Coordinates,
                                ParkingType:          Location.ParkingType.HasValue ? POI.ParkingType.TryParse(Location.ParkingType.Value.ToString()) : null,
                                HotlinePhoneNumber:   Location.HelpPhone,
                                OpeningTimes:         Location.OpeningTimes is not null ? new ImmutableOpeningTimes(Location.OpeningTimes.ToPOI3()) : null,
                                EnergyMix:            Location.EnergyMix?.ToPOI3(Language, $"location '{Location.Id}'", Warnings),
                                Images:               Location.Images.Select(ToPOI3),
                                TimeZone:             Time_Zone.TryParse(Location.TimeZone),
                                Facilities:           Location.Facilities.Select(facility => POI.Facility.TryParse(facility.ToString())).
                                                                          Where (facility => facility.HasValue).
                                                                          Select(facility => facility!.Value),
                                PaymentOptions:       PaymentOptionsOf(capabilities),
                                AuthenticationModes:  AuthenticationModesOf(capabilities),
                                ArrivalInstructions:  ToI18NString(Location.Directions),
                                ChargingWhenClosed:   Location.ChargingWhenClosed,
                                ParkingRestrictions:  evses.SelectMany(evse => evse.ParkingRestrictions).
                                                                     Select    (restriction => restriction.ToString() == "TAXIS" ? "TAXI" : restriction.ToString()).
                                                                     Select    (text => POI.ParkingRestrictionGroup.TryParse(text, out var group) ? group : (POI.ParkingRestrictionGroup?) null).
                                                                     Where     (group => group.HasValue).
                                                                     Select    (group => group!.Value).
                                                                     Distinct(),
                                RelatedLocations:     Location.RelatedLocations.Select(relatedLocation => new POI.AdditionalGeoLocation(
                                                                                                              relatedLocation.GeoLocation,
                                                                                                              relatedLocation.Name.HasValue
                                                                                                                  ? new ImmutableI18NString(I18NString.Create(relatedLocation.Name.Value.Language, relatedLocation.Name.Value.Text))
                                                                                                                  : null
                                                                                                          )),
                                Services:                  Location.Services. Select(service  => POI.LocationService.Parse(service. ToString())),
                                AssistanceServiceDetails:  Location.AssistanceServiceDetails.IsNotNullOrEmpty() ? I18NString.Create(Language, Location.AssistanceServiceDetails) : null,
                                Standards:                 Location.Standards.Select(standard => POI.Standard.       Parse(standard.ToString())),
                                Created:              Location.Created,
                                LastChange:           Location.LastUpdated
                            ).ToJSON();

            #endregion

            #region New: with one charging station, named after the location

            if (Existing is null)
            {

                var stationId = POI.ChargingStation_Id.TryParse(poolId.Value, "");

                if (!stationId.HasValue)
                {
                    Warnings.Add(Warning.Create($"The location identification '{Location.Id}' is no charging station identification!"));
                    return null;
                }

                if (mappedEVSEs.Count > 0)
                    mapped["chargingStations"] = new JArray(
                                                     new POI.ChargingStation(
                                                         Id:          stationId.Value,
                                                         EVSEs:       mappedEVSEs,
                                                         Features:    FeaturesOf(capabilities),
                                                         Created:     Location.Created,
                                                         LastChange:  Location.LastUpdated
                                                     ).ToJSON()
                                                 );

                return mapped;

            }

            #endregion

            #region Existing: what OCPI tells, onto it

            var desired          = (JObject) Existing.DeepClone();

            var unchanged        = Unchanged?.ToPOI3(OperatorId, null, Language, []);
            var unchangedEVSEs   = (unchanged?["chargingStations"] as JArray ?? []).
                                       OfType<JObject>().
                                       SelectMany(station => (station["EVSEs"] as JArray ?? []).OfType<JObject>()).
                                       ToDictionary(evse => evse["@id"]?.Value<String>() ?? "");

            Overlay(desired, mapped, PoolProperties.Where(property => !SameValue(unchanged, mapped, property)));

            if (Effective is not null)
                POI.POIGroupMembers.KeepWhatGroupsGive(desired, Existing, Effective, PoolProperties);

            var effectiveEVSEs   = (Effective?["chargingStations"] as JArray ?? []).
                                       OfType<JObject>().
                                       SelectMany(station => (station["EVSEs"] as JArray ?? []).OfType<JObject>()).
                                       ToDictionary(evse => evse["@id"]?.Value<String>() ?? "");

            var stations     = ArrayOf(desired, "chargingStations");

            var wanted       = mappedEVSEs.ToDictionary(evse => evse.Id.ToString(), evse => evse.ToJSON());
            var placed       = new HashSet<String>();

            foreach (var station in stations.OfType<JObject>())
            {

                var stationEVSEs = station["EVSEs"] as JArray ?? [];

                foreach (var evseJSON in stationEVSEs.OfType<JObject>().ToArray())
                {

                    var evseId = evseJSON["@id"]?.Value<String>() ?? "";

                    if (wanted.TryGetValue(evseId, out var mappedEVSE))
                    {

                        var storedEVSE     = (JObject) evseJSON.DeepClone();
                        var unchangedEVSE  = unchangedEVSEs.GetValueOrDefault(evseId);

                        Overlay(evseJSON, mappedEVSE, EVSEProperties.Where(property => !SameValue(unchangedEVSE, mappedEVSE, property)));

                        if (effectiveEVSEs.GetValueOrDefault(evseId) is JObject effectiveEVSE)
                            POI.POIGroupMembers.KeepWhatGroupsGive(evseJSON, storedEVSE, effectiveEVSE, EVSEProperties);

                        placed.Add(evseId);

                    }

                    // Gone at OCPI.
                    else
                        evseJSON.Remove();

                }

            }

            var newEVSEs = wanted.Where(pair => !placed.Contains(pair.Key)).Select(pair => pair.Value).ToArray();

            if (newEVSEs.Length > 0)
            {

                if (stations.FirstOrDefault() is JObject firstStation)
                {
                    var stationEVSEs = ArrayOf(firstStation, "EVSEs");
                    foreach (var newEVSE in newEVSEs)
                        stationEVSEs.Add(newEVSE);
                }

                else if (POI.ChargingStation_Id.TryParse(poolId.Value, "") is POI.ChargingStation_Id stationId)
                {
                    var station = new POI.ChargingStation(Id: stationId, Created: Location.Created, LastChange: Location.LastUpdated).ToJSON();
                    station["EVSEs"] = new JArray(newEVSEs);
                    stations.Add(station);
                }

            }

            ImportCapabilities(desired, Existing, Effective, Location, Unchanged);

            return desired;

            #endregion

        }

        #endregion

        #region ToPOI3(EVSE, OperatorId, Timestamp, Warnings)

        /// <summary>
        /// The EVSE of the given OCPI EVSE: its identification from its evse_id, else its uid; its current type and
        /// maximum voltage, current and power from its connectors; its charging tariffs from the tariffs of its connectors.
        /// </summary>
        public static POI.EVSE? ToPOI3(this EVSE                        EVSE,
                                       POI.ChargingStationOperator_Id   OperatorId,
                                       DateTimeOffset                   Timestamp,
                                       ICollection<Warning>             Warnings)
        {

            var evseId = POI.EVSE_Id.TryParse(EVSE.EVSEId?.ToString() ?? EVSE.UId.ToString());

            if (!evseId.HasValue || evseId.Value.OperatorId != OperatorId)
            {
                Warnings.Add(Warning.Create($"The EVSE '{EVSE.EVSEId?.ToString() ?? EVSE.UId.ToString()}' has no EVSE identification of the operator '{OperatorId}'!"));
                return null;
            }

            var connectors   = new List<POI.ChargingConnector>();
            var currentTypes = POI.CurrentTypes.Unspecified;

            foreach (var connector in EVSE.Connectors)
            {

                var type = connector.Standard.ToWWCP();

                if (!type.HasValue)
                {
                    Warnings.Add(Warning.Create($"The connector standard '{connector.Standard}' of the EVSE '{evseId}' has no plug type!"));
                    continue;
                }

                // A socket has no cable of its own.
                var isCable = connector.Format == ConnectorFormats.CABLE;

                if (!isCable && (connector.CableLength.HasValue || connector.CableWeight.HasValue || connector.CableManagementSystem.HasValue))
                    Warnings.Add(Warning.Create($"The connector '{connector.Id}' of the EVSE '{evseId}' is a socket: its cable is left out!"));

                connectors.Add(new POI.ChargingConnector(POI.ChargingConnector_Id.Parse(connector.Id.ToString()),
                                                         type.Value,
                                                         Format:                 isCable ? POI.ChargingConnectorFormat.CABLE : POI.ChargingConnectorFormat.SOCKET,
                                                         CableLength:            isCable && connector.CableLength?.m     > 0 ? connector.CableLength : null,
                                                         CableWeight:            isCable && connector.CableWeight?.Value > 0 ? connector.CableWeight : null,
                                                         CableManagementSystem:  isCable ? connector.CableManagementSystem : null,
                                                         Standards:              connector.Standards.Select(standard => POI.Standard.Parse(standard.ToString())),
                                                         Images:                 connector.Images.Select(ToPOI3)));

                currentTypes |= connector.PowerType switch {
                                    PowerTypes.DC          => POI.CurrentTypes.DC,
                                    PowerTypes.AC_3_PHASE  => POI.CurrentTypes.AC_ThreePhases,
                                    PowerTypes.AC_1_PHASE  => POI.CurrentTypes.AC_OnePhase,
                                    PowerTypes.AC_2_PHASE or
                                    PowerTypes.AC_2_PHASE_SPLIT
                                                           => POI.CurrentTypes.AC_OnePhase,
                                    _                      => POI.CurrentTypes.Unspecified
                                };

            }

            if (connectors.Count == 0)
                return null;

            // A tariff with new prices is its charging tariff: the prices are runtime data.
            var tariffIds = EVSE.Connectors.SelectMany(connector => connector.TariffIds).
                                            Select    (tariffId  => ToPOI3TariffIdOf(OperatorId, tariffId)).
                                            Where     (tariffId  => tariffId.HasValue).
                                            Select    (tariffId  => tariffId!.Value).
                                            Distinct();

            return new POI.EVSE(
                       Id:                  evseId.Value,
                       PhysicalReference:   EVSE.PhysicalReference,
                       CurrentTypes:        currentTypes,
                       MaxVoltage:          EVSE.Connectors.Max(connector => connector.MaxVoltage),
                       MaxCurrent:          EVSE.Connectors.Max(connector => connector.MaxAmperage),
                       MaxPower:            EVSE.Connectors.Select(connector => connector.MaxElectricPower).Where(power => power.HasValue).Max(),
                       ChargingConnectors:  connectors,
                       ChargingTariffIds:   tariffIds,
                       Images:              EVSE.Images.Select(ToPOI3),
                       GeoLocation:         EVSE.Coordinates,
                       InitialStatus:       new Timestamped<POI.EVSEStatusType>(EVSE.LastUpdated, EVSE.Status.ToPOI3Status()),
                       ReachDistance:             EVSE.ReachDistance?.m >= 0 ? EVSE.ReachDistance : null,
                       OperationTimeout:          EVSE.OperationTimeout >= TimeSpan.Zero ? EVSE.OperationTimeout : null,
                       ExtendedOperationTimeout:  EVSE.ExtendedOperationTimeout,
                       Standards:                 EVSE.Standards.Select(standard => POI.Standard.Parse(standard.ToString())),
                       FloorLevel:                EVSE.FloorLevel,
                       AcceptedEMobilityProviderIds:  ToPOI3(EVSE.AcceptedServiceProviders, evseId.Value, Warnings).Distinct(),
                       Created:             EVSE.Created,
                       LastChange:          EVSE.LastUpdated
                   );

        }

        #endregion

        #region ParkingSpacesOf(ChargingPool, RoamingNetwork)

        /// <summary>
        /// The parking spaces the charging stations of the given charging pool name, of parking operators or of charging
        /// station operators, each once, in the order of their identifications: as they are at runtime, with the properties
        /// their parking space groups give them.
        /// </summary>
        public static IEnumerable<POI.ParkingSpace> ParkingSpacesOf(POI.ChargingPool    ChargingPool,
                                                                    POI.RoamingNetwork  RoamingNetwork)

            => ChargingPool.ChargingStations.SelectMany(chargingStation => chargingStation.ParkingSpaceIds).
                                             Distinct().
                                             OrderBy   (parkingSpaceId  => parkingSpaceId.ToString(), StringComparer.Ordinal).
                                             Select    (parkingSpaceId  => RoamingNetwork.TryGetEffectiveParkingSpaceById(parkingSpaceId, out var parkingSpace) ? parkingSpace : null).
                                             Where     (parkingSpace    => parkingSpace is not null).
                                             Select    (parkingSpace    => parkingSpace!);

        #endregion

        #region ToOCPI(ParkingSpace) / ToPOI3(Parking, Created, LastChange)

        /// <summary>
        /// The OCPI parking of the given parking space: what OCPI requires and POI3 does not know is false.
        /// </summary>
        public static Parking ToOCPI(this POI.ParkingSpace ParkingSpace)

            => new (Id:                     Parking_Id.Parse(ParkingSpace.Id.ToString()),
                    VehicleTypes:           ParkingSpace.VehicleTypes.Select(vehicleType => VehicleType.Parse(vehicleType.ToString())),
                    RestrictedToType:       ParkingSpace.RestrictedToVehicleTypes ?? false,
                    ReservationRequired:    ParkingSpace.ReservationRequired      ?? false,
                    PhysicalReference:      ParkingSpace.PhysicalReference,
                    MaxVehicleWeight:       ParkingSpace.MaxVehicleWeight,
                    MaxVehicleHeight:       ParkingSpace.MaxVehicleHeight,
                    MaxVehicleLength:       ParkingSpace.MaxVehicleLength,
                    MaxVehicleWidth:        ParkingSpace.MaxVehicleWidth,
                    ParkingSpaceLength:     ParkingSpace.Length,
                    ParkingSpaceWidth:      ParkingSpace.Width,
                    DangerousGoodsAllowed:  ParkingSpace.DangerousGoodsAllowed,
                    EVSEPosition:           ParkingSpace.EVSEPosition.HasValue ? EVSEPosition.    Parse(ParkingSpace.EVSEPosition.Value.ToString()) : null,
                    Direction:              ParkingSpace.Direction.   HasValue ? ParkingDirection.Parse(ParkingSpace.Direction.   Value.ToString()) : null,
                    DriveThrough:           ParkingSpace.DriveThrough,
                    TimeLimit:              ParkingSpace.TimeLimit,
                    Roofed:                 ParkingSpace.Roofed,
                    Images:                 ParkingSpace.Images.Select(ToOCPI),
                    Lighting:               ParkingSpace.Illuminated,
                    RefrigerationOutlet:    ParkingSpace.RefrigerationOutlet,
                    Standards:              ParkingSpace.Standards.Select(standard => Standard.Parse(standard.ToString())),
                    APDSReference:          ParkingSpace.APDSReference,
                    ProtectedArea:          ParkingSpace.ProtectedArea,
                    Surface:                ParkingSpace.Surface.     HasValue ? Surface.         Parse(ParkingSpace.Surface.     Value.ToString()) : null,
                    Slope:                  ParkingSpace.Slope.       HasValue ? Slope.           Parse(ParkingSpace.Slope.       Value.ToString()) : null);

        /// <summary>
        /// The parking space of the given OCPI parking, or null when its identification is none of a parking space.
        /// Measures that are not positive, and values POI3 does not know, are left out.
        /// </summary>
        public static POI.ParkingSpace? ToPOI3(this Parking          Parking,
                                               DateTimeOffset        Created,
                                               DateTimeOffset        LastChange,
                                               ICollection<Warning>  Warnings)
        {

            if (!POI.ParkingSpace_Id.TryParse(Parking.Id.ToString(), out var parkingSpaceId))
            {
                Warnings.Add(Warning.Create($"The parking identification '{Parking.Id}' is no parking space identification!"));
                return null;
            }

            static TEnum? EnumOf<TEnum>(Object? Value) where TEnum : struct, Enum

                => Value is not null && Enum.TryParse<TEnum>(Value.ToString(), ignoreCase: false, out var value) && Enum.IsDefined(value)
                       ? value
                       : null;

            static Meter? Positive(Meter? Meter)
                => Meter?.m > 0 ? Meter : null;

            return new POI.ParkingSpace(
                       parkingSpaceId,
                       PhysicalReference:         Parking.PhysicalReference,
                       VehicleTypes:              Parking.VehicleTypes.Select(vehicleType => POI.VehicleType.Parse(vehicleType.ToString())),
                       MaxVehicleWeight:          Parking.MaxVehicleWeight?.Value > 0 ? Parking.MaxVehicleWeight : null,
                       MaxVehicleHeight:          Positive(Parking.MaxVehicleHeight),
                       MaxVehicleLength:          Positive(Parking.MaxVehicleLength),
                       MaxVehicleWidth:           Positive(Parking.MaxVehicleWidth),
                       Length:                    Positive(Parking.ParkingSpaceLength),
                       Width:                     Positive(Parking.ParkingSpaceWidth),
                       EVSEPosition:              EnumOf<POI.EVSEPosition>    (Parking.EVSEPosition),
                       Direction:                 EnumOf<POI.ParkingDirection>(Parking.Direction),
                       Surface:                   EnumOf<POI.ParkingSurface>  (Parking.Surface),
                       Slope:                     EnumOf<POI.ParkingSlope>    (Parking.Slope),
                       TimeLimit:                 Parking.TimeLimit >= TimeSpan.Zero ? Parking.TimeLimit : null,
                       RestrictedToVehicleTypes:  Parking.RestrictedToType,
                       DriveThrough:              Parking.DriveThrough,
                       ReservationRequired:       Parking.ReservationRequired,
                       Roofed:                    Parking.Roofed,
                       Illuminated:               Parking.Lighting,
                       RefrigerationOutlet:       Parking.RefrigerationOutlet,
                       DangerousGoodsAllowed:     Parking.DangerousGoodsAllowed,
                       ProtectedArea:             Parking.ProtectedArea,
                       APDSReference:             Parking.APDSReference,
                       Standards:                 Parking.Standards.Select(standard => POI.Standard.Parse(standard.ToString())),
                       Images:                    Parking.Images.Select(ToPOI3),
                       Created:                   Created,
                       LastChange:                LastChange
                   );

        }

        #endregion

        #region ToPOI3(Tariff, OperatorId, Language, Warnings)

        /// <summary>
        /// The charging tariff of the given OCPI tariff: energy in kWh becomes Wh, power in kW becomes W.
        /// </summary>
        public static POI.ChargingTariff? ToPOI3(this Tariff                      Tariff,
                                                 POI.ChargingStationOperator_Id   OperatorId,
                                                 Languages                        Language,
                                                 ICollection<Warning>             Warnings)
        {

            var tariffId = ToPOI3TariffId(OperatorId, Tariff.Id);

            if (!tariffId.HasValue)
            {
                Warnings.Add(Warning.Create($"The tariff identification '{Tariff.Id}' is no charging tariff identification!"));
                return null;
            }

            var elements = Tariff.ToPOI3Elements();

            POI.ChargingTariffType? tariffType = null;

            if (Tariff.TariffType.HasValue)
            {
                if (Enum.TryParse<POI.ChargingTariffType>(Tariff.TariffType.Value.ToString(), ignoreCase: false, out var type) &&
                    Enum.IsDefined(type))
                {
                    tariffType = type;
                }
                else
                    Warnings.Add(Warning.Create($"The tariff type '{Tariff.TariffType}' of the tariff '{Tariff.Id}' has no counterpart and is left out!"));
            }

            var minPrice = Tariff.MinPrice?.ToPOI3();
            var maxPrice = Tariff.MaxPrice?.ToPOI3();

            try
            {
                return new POI.ChargingTariff(
                           Id:           tariffId.Value,
                           Currency:     Tariff.Currency,
                           Elements:     elements,
                           Name:         ToI18NString(Tariff.TariffAltText),
                           TariffURL:    Tariff.TariffAltURL,
                           EnergyMix:    Tariff.EnergyMix?.ToPOI3(Language, $"tariff '{Tariff.Id}'", Warnings),
                           TariffType:   tariffType,
                           MinPrice:     minPrice,
                           MaxPrice:     maxPrice,
                           ValidFrom:    Tariff.Start,
                           ValidUntil:   Tariff.End,
                           Created:      Tariff.Created,
                           LastChange:   Tariff.LastUpdated
                       );
            }
            // E.g. a minimum price above the maximum price.
            catch (ArgumentException e)
            {
                Warnings.Add(Warning.Create(e.Message));
                return null;
            }

        }

        #endregion

        #region ToPOI3Elements(Tariff)

        /// <summary>
        /// The charging tariff elements of the given OCPI tariff: prices excluding VAT, energy in Wh, power in W.
        /// </summary>
        public static IReadOnlyList<POI.ChargingTariffElement> ToPOI3Elements(this Tariff Tariff)
        {

            var elements = new List<POI.ChargingTariffElement>();

            foreach (var element in Tariff.TariffElements)
            {

                var priceComponents = element.PriceComponents.Select(priceComponent => new POI.ChargingPriceComponent(
                                                                                           priceComponent.Type.ToString() switch {
                                                                                               "ENERGY"        => POI.ChargingDimensionTypes.ENERGY,
                                                                                               "TIME"          => POI.ChargingDimensionTypes.TIME,
                                                                                               "PARKING_TIME"  => POI.ChargingDimensionTypes.PARKING_TIME,
                                                                                               _               => POI.ChargingDimensionTypes.FLAT
                                                                                           },
                                                                                           NetPrice(priceComponent.Price, priceComponent.VAT, Tariff.TaxIncluded),
                                                                                           // No VAT applies: none given, or none at all.
                                                                                           priceComponent.VAT ?? (Tariff.TaxIncluded == TaxIncluded.NA ? 0 : null),
                                                                                           priceComponent.Type.ToString() == "FLAT" ? null : priceComponent.StepSize
                                                                                       )).ToArray();

                var restrictions    = element.TariffRestrictions;

                elements.Add(new POI.ChargingTariffElement(
                                 priceComponents,
                                 restrictions is null
                                     ? null
                                     : new POI.ChargingTariffRestrictions(
                                           StartTime:    restrictions.StartTime.HasValue ? new TimeOnly(restrictions.StartTime.Value.Hour, restrictions.StartTime.Value.Minute) : null,
                                           EndTime:      restrictions.EndTime.  HasValue ? new TimeOnly(restrictions.EndTime.  Value.Hour, restrictions.EndTime.  Value.Minute) : null,
                                           StartDate:    restrictions.StartDate.HasValue ? DateOnly.FromDateTime(restrictions.StartDate.Value.UtcDateTime) : null,
                                           EndDate:      restrictions.EndDate.  HasValue ? DateOnly.FromDateTime(restrictions.EndDate.  Value.UtcDateTime) : null,
                                           MinEnergy:    restrictions.MinkWh * 1000,
                                           MaxEnergy:    restrictions.MaxkWh * 1000,
                                           MinCurrent:   restrictions.MinCurrent,
                                           MaxCurrent:   restrictions.MaxCurrent,
                                           MinPower:     restrictions.MinPower * 1000,
                                           MaxPower:     restrictions.MaxPower * 1000,
                                           MinDuration:  restrictions.MinDuration.HasValue ? (Decimal) restrictions.MinDuration.Value.TotalSeconds : null,
                                           MaxDuration:  restrictions.MaxDuration.HasValue ? (Decimal) restrictions.MaxDuration.Value.TotalSeconds : null,
                                           DaysOfWeek:   restrictions.DayOfWeek,
                                           Reservation:  restrictions.Reservation switch {
                                                             ReservationRestrictions.RESERVATION          => POI.ChargingReservationRestriction.RESERVATION,
                                                             ReservationRestrictions.RESERVATION_EXPIRES  => POI.ChargingReservationRestriction.RESERVATION_EXPIRES,
                                                             _                                            => null
                                                         }
                                       )
                             ));

            }

            return elements;

        }

        #endregion


        #region (internal) ParkingSpaceProperties

        /// <summary>
        /// The properties of a parking space OCPI tells: imported onto an existing parking space of a charging station
        /// operator, the others are kept.
        /// </summary>
        internal static readonly String[] ParkingSpaceProperties = [
            "physicalReference", "vehicleTypes", "restrictedToVehicleTypes", "maxVehicleWeight", "maxVehicleHeight",
            "maxVehicleLength", "maxVehicleWidth", "length", "width", "evsePosition", "direction", "surface", "slope",
            "timeLimit", "driveThrough", "reservationRequired", "roofed", "illuminated", "refrigerationOutlet",
            "dangerousGoodsAllowed", "protectedArea", "apdsReference", "standards", "images"
        ];

        #endregion

        #region (internal) PoolProperties / EVSEProperties

        /// <summary>
        /// The properties of a charging pool OCPI tells: imported onto an existing charging pool, the others are kept.
        /// </summary>
        internal static readonly String[] PoolProperties = [
            "name", "address", "geoLocation", "timeZone", "parkingType", "hotline", "openingTimes", "energyMix", "images",
            "facilities", "arrivalInstructions", "chargingWhenClosed", "relatedLocations", "services", "assistanceServiceDetails",
            "standards"
        ];

        /// <summary>
        /// The properties of an EVSE OCPI tells: imported onto an existing EVSE, the others are kept.
        /// </summary>
        internal static readonly String[] EVSEProperties = [
            "physicalReference", "currentTypes", "maxVoltage", "maxCurrent", "maxPower", "chargingConnectors",
            "chargingTariffIds", "images", "geoLocation", "reachDistance", "operationTimeout", "extendedOperationTimeout", "standards",
            "floorLevel", "acceptedEMobilityProviderIds"
        ];

        /// <summary>
        /// The array of the given property, made when there is none. Not one assigned again: a token with a parent
        /// is copied when it is assigned, and changes to it would go nowhere.
        /// </summary>
        internal static JArray ArrayOf(JObject  Parent,
                                       String   Property)
        {

            if (Parent[Property] is JArray array)
                return array;

            array = [];
            Parent[Property] = array;
            return array;

        }

        /// <summary>
        /// Whether the given property is the same in both, also when neither has it; never without the first.
        /// </summary>
        internal static Boolean SameValue(JObject?  JSON1,
                                          JObject   JSON2,
                                          String    Property)

            => JSON1 is not null &&
               JToken.DeepEquals(JSON1[Property], JSON2[Property]);

        /// <summary>
        /// Set the given properties of the target to those of the source; one the source does not have is removed.
        /// </summary>
        internal static void Overlay(JObject                Target,
                                     JObject                Source,
                                     IEnumerable<String>    Properties)
        {
            foreach (var property in Properties)
            {
                if (Source[property] is JToken value)
                    Target[property] = value.DeepClone();
                else
                    Target.Remove(property);
            }
        }

        #endregion

        #region (private) Helpers

        private static String? FirstText(IEnumerable<I18NPair>? Text)

            => Text?.Select(pair => pair.Text).FirstOrDefault(text => text.IsNotNullOrEmpty());

        private static IEnumerable<DisplayText> ToDisplayTexts(IEnumerable<I18NPair>? Text)

            => Text?.Where (pair => pair.Text.IsNotNullOrEmpty()).
                     Select(pair => new DisplayText(pair.Language, pair.Text)).
                     ToArray() ?? [];

        private static I18NString? ToI18NString(IEnumerable<DisplayText> Texts)
        {

            var text = I18NString.Empty;

            foreach (var displayText in Texts)
                text.Set(displayText.Language, displayText.Text);

            return text.IsNullOrEmpty() ? null : text;

        }

        private static ImageFileType ImageFileTypeOf(URL URL)

            => URL.ToString()[(URL.ToString().LastIndexOf('.') + 1)..].ToLowerInvariant() switch {
                   "gif"   => ImageFileType.gif,
                   "png"   => ImageFileType.png,
                   "svg"   => ImageFileType.svg,
                   "jpeg"  => ImageFileType.jpeg,
                   "webp"  => ImageFileType.webp,
                   _       => ImageFileType.jpg
               };

        private static Image ToOCPI(POI.Image Image)

            => new (Image.URL,
                    Image.Type == HTTPContentType.Image.PNG  ? ImageFileType.png
                  : Image.Type == HTTPContentType.Image.GIF  ? ImageFileType.gif
                  : Image.Type == HTTPContentType.Image.SVG  ? ImageFileType.svg
                  : Image.Type == HTTPContentType.Image.WEBP ? ImageFileType.webp
                  : ImageFileType.jpeg,
                    ImageCategory.TryParse(Image.Category.ToString()) ?? ImageCategory.OTHER,
                    Image.Width,
                    Image.Height,
                    Image.Thumbnail);

        private static POI.Image ToPOI3(Image Image)

            => new (Image.URL,
                    Image.Type == ImageFileType.png  ? HTTPContentType.Image.PNG
                  : Image.Type == ImageFileType.gif  ? HTTPContentType.Image.GIF
                  : Image.Type == ImageFileType.svg  ? HTTPContentType.Image.SVG
                  : Image.Type == ImageFileType.webp ? HTTPContentType.Image.WEBP
                  : HTTPContentType.Image.JPEG,
                    POI.ImageCategory.TryParse(Image.Category.ToString()) ?? POI.ImageCategory.OTHER,
                    Image.Width,
                    Image.Height,
                    Image.Thumbnail);

        /// <summary>
        /// The OCPI energy source categories of the POI3 ones: waste and unknown have none.
        /// </summary>
        private static readonly Dictionary<POI.EnergySourceCategory, EnergySourceCategory> ocpiEnergySourceCategories = new() {
            { POI.EnergySourceCategory.Gas,            EnergySourceCategory.GAS            },
            { POI.EnergySourceCategory.Coal,           EnergySourceCategory.COAL           },
            { POI.EnergySourceCategory.StoneCoal,      EnergySourceCategory.COAL           },
            { POI.EnergySourceCategory.Oil,            EnergySourceCategory.GENERAL_FOSSIL },
            { POI.EnergySourceCategory.OtherFossil,    EnergySourceCategory.GENERAL_FOSSIL },
            { POI.EnergySourceCategory.NuclearFision,  EnergySourceCategory.NUCLEAR        },
            { POI.EnergySourceCategory.NuclearFusion,  EnergySourceCategory.NUCLEAR        },
            { POI.EnergySourceCategory.Sun,            EnergySourceCategory.SOLAR          },
            { POI.EnergySourceCategory.Wind,           EnergySourceCategory.WIND           },
            { POI.EnergySourceCategory.Geothermal,     EnergySourceCategory.GENERAL_GREEN  },
            { POI.EnergySourceCategory.Biogas,         EnergySourceCategory.GENERAL_GREEN  },
            { POI.EnergySourceCategory.OtherGreen,     EnergySourceCategory.GENERAL_GREEN  }
        };

        /// <summary>
        /// The POI3 energy source categories of the OCPI ones: POI3 has no water, which becomes other green.
        /// </summary>
        private static readonly Dictionary<EnergySourceCategory, POI.EnergySourceCategory> poi3EnergySourceCategories = new() {
            { EnergySourceCategory.GAS,             POI.EnergySourceCategory.Gas           },
            { EnergySourceCategory.COAL,            POI.EnergySourceCategory.Coal          },
            { EnergySourceCategory.GENERAL_FOSSIL,  POI.EnergySourceCategory.OtherFossil   },
            { EnergySourceCategory.NUCLEAR,         POI.EnergySourceCategory.NuclearFision },
            { EnergySourceCategory.SOLAR,           POI.EnergySourceCategory.Sun           },
            { EnergySourceCategory.WIND,            POI.EnergySourceCategory.Wind          },
            { EnergySourceCategory.WATER,           POI.EnergySourceCategory.OtherGreen    },
            { EnergySourceCategory.GENERAL_GREEN,   POI.EnergySourceCategory.OtherGreen    }
        };

        /// <summary>
        /// The OCPI environmental impact categories of the POI3 ones, both in g/kWh.
        /// </summary>
        private static readonly Dictionary<POI.EnvironmentalImpact, EnvironmentalImpactCategory> ocpiEnvironmentalImpacts = new() {
            { POI.EnvironmentalImpact.CarbonDioxide,  EnvironmentalImpactCategory.CARBON_DIOXIDE },
            { POI.EnvironmentalImpact.NuclearWaste,   EnvironmentalImpactCategory.NUCLEAR_WASTE  }
        };

        /// <param name="Of">What the energy mix belongs to, for warnings, e.g. "charging pool 'DE*GEF*P1'".</param>
        private static EnergyMix ToOCPI(this POI.EnergyMix    EnergyMix,
                                        String                Of,
                                        ICollection<Warning>? Warnings   = null)
        {

            var sources = new List<EnergySource>();

            foreach (var source in EnergyMix.EnergySources)
            {
                if (ocpiEnergySourceCategories.TryGetValue(source.Value, out var category))
                    sources.Add(new EnergySource(category, source.Percent));
                else
                    Warnings?.Add(Warning.Create($"The energy source '{source.Value}' of the {Of} has no OCPI counterpart and is left out!"));
            }

            var green   = sources.Count > 0 &&
                          sources.All(source => source.Source == EnergySourceCategory.GENERAL_GREEN ||
                                                source.Source == EnergySourceCategory.SOLAR         ||
                                                source.Source == EnergySourceCategory.WIND          ||
                                                source.Source == EnergySourceCategory.WATER);

            var impacts = new List<EnvironmentalImpact>();

            foreach (var impact in EnergyMix.EnvironmentalImpacts)
            {
                if (ocpiEnvironmentalImpacts.TryGetValue(impact.Category, out var category))
                    impacts.Add(new EnvironmentalImpact(category, (Double) impact.GramsPerKWh));
                else
                    Warnings?.Add(Warning.Create($"The environmental impact '{impact.Category}' of the {Of} has no OCPI counterpart and is left out!"));
            }

            return new EnergyMix(green,
                                 sources,
                                 impacts,
                                 FirstText(EnergyMix.SupplierName),
                                 FirstText(EnergyMix.ProductName));

        }

        /// <param name="Of">What the energy mix belongs to, for warnings, e.g. "location 'P1'".</param>
        private static POI.EnergyMix ToPOI3(this EnergyMix        EnergyMix,
                                            Languages             Language,
                                            String                Of,
                                            ICollection<Warning>? Warnings   = null)
        {

            var sources = new List<PercentageOf<POI.EnergySourceCategory>>();

            foreach (var source in EnergyMix.EnergySources)
            {
                if (poi3EnergySourceCategories.TryGetValue(source.Source, out var category))
                    sources.Add(new PercentageOf<POI.EnergySourceCategory>(category, (Single) source.Percentage));
                else
                    Warnings?.Add(Warning.Create($"The energy source '{source.Source}' of the {Of} has no POI3 counterpart and is left out!"));
            }

            var impacts = new List<POI.EnvironmentalImpactAmount>();

            foreach (var impact in EnergyMix.EnvironmentalImpacts)
            {

                var category = ocpiEnvironmentalImpacts.FirstOrDefault(entry => entry.Value == impact.Category).Key;

                if (category.IsNullOrEmpty)
                    Warnings?.Add(Warning.Create($"The environmental impact '{impact.Category}' of the {Of} has no POI3 counterpart and is left out!"));

                else if (!Double.IsFinite(impact.Amount) || impact.Amount < 0 || impacts.Any(known => known.Category == category))
                    Warnings?.Add(Warning.Create($"The environmental impact '{impact.Category}' of the {Of} is negative, no number or given twice, and is left out!"));

                else
                    impacts.Add(new POI.EnvironmentalImpactAmount(category, (Decimal) impact.Amount));

            }

            return new (sources,
                        impacts,
                        new ImmutableI18NString(I18NString.Create(Language, EnergyMix.SupplierName      ?? "")),
                        new ImmutableI18NString(I18NString.Create(Language, EnergyMix.EnergyProductName ?? "")));

        }

        private static OpeningTimes ToPOI3(this Hours Hours)
        {

            var openingTimes = Hours.IsTwentyFourSevenOpen
                                   ? OpeningTimes.Open24Hours
                                   : new OpeningTimes();

            foreach (var regularHours in Hours.RegularHours)
                openingTimes.AddRegularOpening(regularHours.Weekday, regularHours.Begin, regularHours.End);

            foreach (var exceptionalOpening in Hours.ExceptionalOpenings)
                openingTimes.AddExceptionalOpening(exceptionalOpening.Begin, exceptionalOpening.End);

            foreach (var exceptionalClosing in Hours.ExceptionalClosings)
                openingTimes.AddExceptionalClosing(exceptionalClosing.Begin, exceptionalClosing.End);

            return openingTimes;

        }

        /// <summary>
        /// The tariffs of an EVSE as it is at runtime: its own, else the one of its most specific groups, of the EVSE,
        /// charging station or charging pool groups it belongs to (ChargingStationOperator.EffectiveEVSE(…)).
        /// </summary>
        /// <summary>
        /// The OCPI eMSPs of the given e-mobility providers: DE-GDF is DE-GDF.
        /// </summary>
        private static IEnumerable<EMSP_Id> ToOCPI(IEnumerable<POI.EMobilityProvider_Id>  EMobilityProviderIds,
                                                   POI.EVSE_Id                            EVSEId,
                                                   ICollection<Warning>                   Warnings)
        {
            foreach (var eMobilityProviderId in EMobilityProviderIds)
            {
                if (EMSP_Id.TryParse($"{eMobilityProviderId.CountryCode.Alpha2Code}-{eMobilityProviderId.Suffix}", out var emspId))
                    yield return emspId;
                else
                    Warnings.Add(Warning.Create($"The e-mobility provider '{eMobilityProviderId}' the EVSE '{EVSEId}' accepts is no OCPI eMSP and is left out!"));
            }
        }

        /// <summary>
        /// The e-mobility providers of the given OCPI eMSPs, unless their party has not the three characters of a provider.
        /// </summary>
        private static IEnumerable<POI.EMobilityProvider_Id> ToPOI3(IEnumerable<EMSP_Id>  EMSPIds,
                                                                    POI.EVSE_Id           EVSEId,
                                                                    ICollection<Warning>  Warnings)
        {
            foreach (var emspId in EMSPIds)
            {
                if (POI.EMobilityProvider_Id.TryParse(emspId.ToString(), out var eMobilityProviderId))
                    yield return eMobilityProviderId;
                else
                    Warnings.Add(Warning.Create($"The eMSP '{emspId}' the EVSE '{EVSEId}' accepts is no e-mobility provider and is left out!"));
            }
        }

        private static IEnumerable<Tariff_Id> TariffIdsOf(POI.EVSE                                                EVSE,
                                                          IReadOnlyList<Timestamped<POI.ChargingTariffPrices>>    Prices,
                                                          POI.ChargingStationOperator                             Operator)

            => Operator.EffectiveEVSE(EVSE).ChargingTariffIds.
                   Select(tariffId => tariffId.ToOCPITariffId(Prices.FirstOrDefault(price => price.Value.ChargingTariffId == tariffId).Value)).
                   Where (tariffId => tariffId.HasValue).
                   Select(tariffId => tariffId!.Value);

        /// <summary>
        /// The new prices in effect at the given EVSE of the charging tariffs it takes (see ChargingPrices.Effective),
        /// with since when: the most specific price status naming a tariff, the EVSE's before its station's before its pool's.
        /// </summary>
        public static IReadOnlyList<Timestamped<POI.ChargingTariffPrices>> PricesOf(POI.EVSE                     EVSE,
                                                                                    POI.ChargingStation          ChargingStation,
                                                                                    POI.ChargingPool             ChargingPool,
                                                                                    POI.ChargingStationOperator  Operator)
        {

            var tariffIds = Operator.EffectiveEVSE(EVSE).ChargingTariffIds.ToHashSet();

            return [.. POI.ChargingPrices.Effective(ChargingPool, ChargingStation, EVSE).
                           Where(price => tariffIds.Contains(price.Value.ChargingTariffId)) ];

        }

        /// <summary>
        /// The OCPI capabilities of an EVSE: from the authentication modes, payment options and UI features of its
        /// charging pool, and the features of its charging station.
        /// </summary>
        private static IEnumerable<Capability> CapabilitiesOf(POI.ChargingStation  ChargingStation,
                                                              POI.ChargingPool     ChargingPool)
        {

            var capabilities  = new HashSet<Capability>();
            var modes         = ChargingPool.AuthenticationModes.Select(mode => mode.Type).ToHashSet();

            if (modes.Contains(POI.AuthenticationMode.RFIDType)        || ChargingPool.UIFeatures.Contains(POI.UIFeatures.RFID))
                capabilities.Add(Capability.RFID_READER);

            if (modes.Contains(POI.AuthenticationMode.CreditCardType)  || ChargingPool.PaymentOptions.Contains(POI.PaymentOptions.CreditCard) || ChargingPool.UIFeatures.Contains(POI.UIFeatures.CreditCard))
                capabilities.Add(Capability.CREDIT_CARD_PAYABLE);

            if (modes.Contains(POI.AuthenticationMode.DebitCardType)   || ChargingPool.PaymentOptions.Contains(POI.PaymentOptions.DebitCard)  || ChargingPool.UIFeatures.Contains(POI.UIFeatures.DebitCard))
                capabilities.Add(Capability.DEBIT_CARD_PAYABLE);

            if (modes.Contains(POI.AuthenticationMode.NFCType)         || ChargingPool.UIFeatures.Contains(POI.UIFeatures.NFC))
                capabilities.Add(Capability.CONTACTLESS_CARD_SUPPORT);

            if (modes.Contains(POI.AuthenticationMode.PINPADType)      || ChargingPool.UIFeatures.Contains(POI.UIFeatures.Pinpad))
                capabilities.Add(Capability.PED_TERMINAL);

            if (modes.Contains(POI.AuthenticationMode.REMOTEType))
                capabilities.Add(Capability.REMOTE_START_STOP_CAPABLE);

            if (ChargingStation.Features.Contains(POI.ChargingStationFeature.Reservable))
                capabilities.Add(Capability.RESERVABLE);

            if (ChargingStation.Features.Contains(POI.ChargingStationFeature.ChargingProfilesSupported))
                capabilities.Add(Capability.CHARGING_PROFILE_CAPABLE);

            if (ChargingStation.Features.Contains(POI.ChargingStationFeature.ChargingPreferencesSupported))
                capabilities.Add(Capability.CHARGING_PREFERENCES_CAPABLE);

            if (ChargingStation.Features.Contains(POI.ChargingStationFeature.TokenGroupsSupported))
                capabilities.Add(Capability.TOKEN_GROUP_CAPABLE);

            if (ChargingStation.Features.Contains(POI.ChargingStationFeature.CSOUnlockSupported))
                capabilities.Add(Capability.UNLOCK_CAPABLE);

            return capabilities;

        }

        /// <summary>
        /// The capabilities an EVSE takes from its charging pool, with the authentication modes, payment options and user
        /// interface features giving them.
        /// </summary>
        private static readonly (Capability Capability, String[] ModeTypes, POI.PaymentOptions[] PaymentOptions, POI.UIFeatures[] UIFeatures)[] poolCapabilities = [
            (Capability.RFID_READER,                [ POI.AuthenticationMode.RFIDType       ], [],                                [ POI.UIFeatures.RFID       ]),
            (Capability.REMOTE_START_STOP_CAPABLE,  [ POI.AuthenticationMode.REMOTEType     ], [],                                []),
            (Capability.CREDIT_CARD_PAYABLE,        [ POI.AuthenticationMode.CreditCardType ], [ POI.PaymentOptions.CreditCard ], [ POI.UIFeatures.CreditCard ]),
            (Capability.DEBIT_CARD_PAYABLE,         [ POI.AuthenticationMode.DebitCardType  ], [ POI.PaymentOptions.DebitCard  ], [ POI.UIFeatures.DebitCard  ]),
            (Capability.CONTACTLESS_CARD_SUPPORT,   [ POI.AuthenticationMode.NFCType        ], [],                                [ POI.UIFeatures.NFC        ]),
            (Capability.PED_TERMINAL,               [ POI.AuthenticationMode.PINPADType     ], [],                                [ POI.UIFeatures.Pinpad     ])
        ];

        /// <summary>
        /// The capabilities an EVSE takes from the features of its charging station.
        /// </summary>
        private static readonly (Capability Capability, POI.ChargingStationFeature Feature)[] stationCapabilities = [
            (Capability.RESERVABLE,                    POI.ChargingStationFeature.Reservable),
            (Capability.CHARGING_PROFILE_CAPABLE,      POI.ChargingStationFeature.ChargingProfilesSupported),
            (Capability.CHARGING_PREFERENCES_CAPABLE,  POI.ChargingStationFeature.ChargingPreferencesSupported),
            (Capability.TOKEN_GROUP_CAPABLE,           POI.ChargingStationFeature.TokenGroupsSupported),
            (Capability.UNLOCK_CAPABLE,                POI.ChargingStationFeature.CSOUnlockSupported)
        ];

        /// <summary>
        /// The properties of a charging pool its capabilities tell.
        /// </summary>
        private static readonly String[] poolCapabilityProperties = [ "authenticationModes", "paymentOptions", "uiFeatures" ];

        private static IEnumerable<POI.ChargingStationFeature> FeaturesOf(IReadOnlySet<Capability> Capabilities)

            => stationCapabilities.Where (entry => Capabilities.Contains(entry.Capability)).
                                   Select(entry => entry.Feature);

        #region (private static) ImportCapabilities(Desired, Existing, Effective, Location, Unchanged)

        /// <summary>
        /// What the capabilities of the EVSEs of the given location tell other than those of the existing charging pool
        /// as OCPI tells it, onto the pool and its charging stations: a capability told now adds the authentication mode
        /// and payment option, or the feature of the station, giving it; one no longer told removes everything giving it.
        /// Everything else stays, e.g. an RFID authentication mode with its card types, or a QR code. What a group gives
        /// stays in the group.
        /// </summary>
        private static void ImportCapabilities(JObject    Desired,
                                               JObject    Existing,
                                               JObject?   Effective,
                                               Location   Location,
                                               Location?  Unchanged)
        {

            static Dictionary<String, HashSet<Capability>> CapabilitiesByEVSE(IEnumerable<EVSE> EVSEs)
            {

                var byEVSE = new Dictionary<String, HashSet<Capability>>();

                foreach (var evse in EVSEs.Where(evse => evse.Status != StatusType.REMOVED))
                {

                    var key = POI.EVSE_Id.TryParse(evse.EVSEId?.ToString() ?? evse.UId.ToString())?.ToString() ?? "";

                    if (!byEVSE.TryGetValue(key, out var capabilities))
                        byEVSE[key] = capabilities = [];

                    capabilities.UnionWith(evse.Capabilities);

                }

                return byEVSE;

            }

            var told    = CapabilitiesByEVSE(Location.EVSEs);
            var before  = CapabilitiesByEVSE(Unchanged?.EVSEs ?? []);

            #region The charging pool: what all its EVSEs tell

            var toldOfPool    = told.  Values.SelectMany(capabilities => capabilities).ToHashSet();
            var beforeOfPool  = before.Values.SelectMany(capabilities => capabilities).ToHashSet();
            var added         = poolCapabilities.Where(entry =>  toldOfPool.Contains(entry.Capability) && !beforeOfPool.Contains(entry.Capability)).ToArray();
            var removed       = poolCapabilities.Where(entry => !toldOfPool.Contains(entry.Capability) &&  beforeOfPool.Contains(entry.Capability)).ToArray();

            if (added.Length > 0 || removed.Length > 0)
            {

                // As it is at runtime, with what its groups give it.
                var source           = Effective ?? Existing;
                var newCapabilities  = added.Select(entry => entry.Capability).ToHashSet();

                var removedModes     = removed.SelectMany(entry => entry.ModeTypes).ToHashSet();
                var removedOptions   = removed.SelectMany(entry => entry.PaymentOptions).Select(option  => option. ToString()).ToHashSet();
                var removedUI        = removed.SelectMany(entry => entry.UIFeatures).    Select(feature => feature.ToString()).ToHashSet();

                var modes            = (source["authenticationModes"] as JArray ?? []).
                                           Where (mode => !removedModes.Contains(mode["type"]?.Value<String>() ?? "")).
                                           Select(mode => mode.DeepClone()).
                                           Concat(AuthenticationModesOf(newCapabilities).Select(mode => mode.ToJSON())).
                                           ToArray();

                var options          = (source["paymentOptions"] as JArray ?? []).
                                           Values<String>().OfType<String>().
                                           Where (option => !removedOptions.Contains(option)).
                                           Concat(PaymentOptionsOf(newCapabilities).Select(option => option.ToString())).
                                           Distinct().
                                           Order (StringComparer.Ordinal).
                                           ToArray();

                var ui               = (source["uiFeatures"] as JArray ?? []).
                                           Values<String>().OfType<String>().
                                           Where (feature => !removedUI.Contains(feature)).
                                           ToArray();

                void Set(String Property, JToken[] Values)
                {
                    if (Values.Length > 0)
                        Desired[Property] = new JArray(Values);
                    else
                        Desired.Remove(Property);
                }

                Set("authenticationModes",  modes);
                Set("paymentOptions",       [.. options.Select(option  => (JToken) option)]);
                Set("uiFeatures",           [.. ui.     Select(feature => (JToken) feature)]);

                if (Effective is not null)
                    POI.POIGroupMembers.KeepWhatGroupsGive(Desired, Existing, Effective, poolCapabilityProperties);

            }

            #endregion

            #region Its charging stations: what their EVSEs tell

            static Dictionary<String, JObject> StationsOf(JObject? Pool)

                => (Pool?["chargingStations"] as JArray ?? []).
                       OfType<JObject>().
                       GroupBy (station => station["@id"]?.Value<String>() ?? "").
                       ToDictionary(group => group.Key, group => group.First());

            var storedStations     = StationsOf(Existing);
            var effectiveStations  = StationsOf(Effective);

            foreach (var station in (Desired["chargingStations"] as JArray ?? []).OfType<JObject>())
            {

                var evseIds          = (station["EVSEs"] as JArray ?? []).OfType<JObject>().Select(evse => evse["@id"]?.Value<String>() ?? "").ToArray();
                var toldOfStation    = evseIds.SelectMany(evseId => told.  GetValueOrDefault(evseId) ?? []).ToHashSet();
                var beforeOfStation  = evseIds.SelectMany(evseId => before.GetValueOrDefault(evseId) ?? []).ToHashSet();

                var addedFeatures    = stationCapabilities.Where (entry =>  toldOfStation.Contains(entry.Capability) && !beforeOfStation.Contains(entry.Capability)).
                                                           Select(entry => entry.Feature.ToString()).
                                                           ToArray();

                var removedFeatures  = stationCapabilities.Where (entry => !toldOfStation.Contains(entry.Capability) &&  beforeOfStation.Contains(entry.Capability)).
                                                           Select(entry => entry.Feature.ToString()).
                                                           ToHashSet();

                if (addedFeatures.Length == 0 && removedFeatures.Count == 0)
                    continue;

                var stationId         = station["@id"]?.Value<String>() ?? "";
                var effectiveStation  = effectiveStations.GetValueOrDefault(stationId);

                // As it is at runtime, with what its groups give it.
                var features          = ((effectiveStation ?? station)["features"] as JArray ?? []).
                                            Values<String>().
                                            OfType<String>().
                                            Where (feature => !removedFeatures.Contains(feature)).
                                            Concat(addedFeatures).
                                            Distinct().
                                            Order (StringComparer.Ordinal).
                                            ToArray();

                if (features.Length > 0)
                    station["features"] = new JArray(features);
                else
                    station.Remove("features");

                if (effectiveStation is not null)
                    POI.POIGroupMembers.KeepWhatGroupsGive(station, storedStations.GetValueOrDefault(stationId), effectiveStation, [ "features" ]);

            }

            #endregion

        }

        #endregion

        #region (private static) StatusScheduleOf(EVSE)

        /// <summary>
        /// The planned status of the given EVSE as OCPI tells it: from its planned status and admin status, the periods
        /// of another status than its current one, each until the next planned change, the last one open. A return to the
        /// current status is the end of the period before it.
        /// </summary>
        private static IEnumerable<StatusSchedule> StatusScheduleOf(POI.EVSE EVSE)
        {

            var now       = Timestamp.Now;
            var statuses  = EVSE.StatusSchedule().     OrderBy(status => status.Timestamp).ToArray();
            var admin     = EVSE.AdminStatusSchedule().OrderBy(status => status.Timestamp).ToArray();
            var changes   = statuses.Select(status => status.Timestamp).
                                Concat(admin.Select(status => status.Timestamp)).
                                Where (timestamp => timestamp > now).
                                Distinct().
                                Order().
                                ToArray();

            if (changes.Length == 0)
                return [];

            var current   = ToOCPIStatus(EVSE.Status.Value, EVSE.AdminStatus.Value);
            var periods   = new List<(DateTimeOffset Begin, StatusType Status)>();
            var previous  = current;

            foreach (var change in changes)
            {

                var status = ToOCPIStatus(statuses.LastOrDefault(entry => entry.Timestamp <= change, EVSE.Status).     Value,
                                          admin.   LastOrDefault(entry => entry.Timestamp <= change, EVSE.AdminStatus).Value);

                if (status != previous)
                {
                    periods.Add((change, status));
                    previous = status;
                }

            }

            var schedule = new List<StatusSchedule>();

            for (var i = 0; i < periods.Count; i++)
                if (periods[i].Status != current)
                    schedule.Add(new StatusSchedule(periods[i].Status,
                                                    periods[i].Begin,
                                                    i + 1 < periods.Count ? periods[i + 1].Begin : null));

            return schedule;

        }

        #endregion

        private static IEnumerable<POI.AuthenticationMode> AuthenticationModesOf(IReadOnlySet<Capability> Capabilities)
        {

            var modes = new List<POI.AuthenticationMode>();

            if (Capabilities.Contains(Capability.RFID_READER))                modes.Add(POI.AuthenticationMode.RFID());
            if (Capabilities.Contains(Capability.REMOTE_START_STOP_CAPABLE))  modes.Add(POI.AuthenticationMode.REMOTE);
            if (Capabilities.Contains(Capability.CREDIT_CARD_PAYABLE))        modes.Add(POI.AuthenticationMode.CreditCard);
            if (Capabilities.Contains(Capability.DEBIT_CARD_PAYABLE))         modes.Add(POI.AuthenticationMode.DebitCard);
            if (Capabilities.Contains(Capability.CONTACTLESS_CARD_SUPPORT))   modes.Add(POI.AuthenticationMode.NFC);
            if (Capabilities.Contains(Capability.PED_TERMINAL))               modes.Add(POI.AuthenticationMode.PINPAD);

            return modes;

        }

        private static IEnumerable<POI.PaymentOptions> PaymentOptionsOf(IReadOnlySet<Capability> Capabilities)
        {

            var options = new List<POI.PaymentOptions>();

            if (Capabilities.Contains(Capability.CREDIT_CARD_PAYABLE))  options.Add(POI.PaymentOptions.CreditCard);
            if (Capabilities.Contains(Capability.DEBIT_CARD_PAYABLE))   options.Add(POI.PaymentOptions.DebitCard);

            return options;

        }

        private static IEnumerable<ParkingRestriction> ParkingRestrictionsOf(POI.ChargingPool ChargingPool)

            => ChargingPool.ParkingRestrictions.
                   Select(group => group.ToString() == "TAXI" ? "TAXIS" : group.ToString()).
                   Select(text  => ParkingRestriction.TryParse(text)).
                   Where (restriction => restriction.HasValue).
                   Select(restriction => restriction!.Value).
                   Distinct();

        #endregion

    }

}
