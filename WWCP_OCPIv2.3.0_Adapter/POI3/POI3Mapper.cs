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

            var brand = ChargingPool.BrandIds.
                            Select(brandId => RoamingNetwork.TryGetBrandById(brandId, out var found) ? found : null).
                            FirstOrDefault(found => found is not null);

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
                       EnergyMix:           ChargingPool.EnergyMix?.ToOCPI(),
                       HelpPhone:           ChargingPool.HotlinePhoneNumber ?? chargingStationOperator.HotlinePhoneNumber,

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

            var tariffIds   = TariffIdsOf(EVSE, ChargingStation, ChargingPool, Operator).ToArray();
            var connectors  = new List<Connector>();

            foreach (var chargingConnector in EVSE.ChargingConnectors)
            {
                var connector = chargingConnector.ToOCPI(EVSE, tariffIds, Operator, Warnings);
                if (connector is not null)
                    connectors.Add(connector);
            }

            if (connectors.Count == 0)
                return null;

            var lastUpdated = new[] { EVSE.LastChangeDate, EVSE.Status.Timestamp, EVSE.AdminStatus.Timestamp, NotBefore ?? DateTimeOffset.MinValue }.Max();

            return new EVSE(

                       UId:                  uid.Value,
                       Status:               ToOCPIStatus(EVSE.Status.Value, EVSE.AdminStatus.Value),
                       Connectors:           connectors,
                       EVSEId:               EVSE_Id.TryParse(EVSE.Id.ToString()),
                       Capabilities:         CapabilitiesOf(ChargingStation, ChargingPool),
                       Coordinates:          EVSE.GeoLocation ?? ChargingStation.GeoLocation ?? ChargingPool.GeoLocation,
                       PhysicalReference:    EVSE.PhysicalReference ?? ChargingStation.PhysicalReference,
                       ParkingRestrictions:  ParkingRestrictionsOf(ChargingPool),
                       Images:               EVSE.Images.Concat(ChargingStation.Images).Select(ToOCPI).Distinct(),

                       Created:              EVSE.Created,
                       LastUpdated:          lastUpdated

                   );

        }

        #endregion

        #region ToOCPI(ChargingConnector, EVSE, TariffIds, Operator, Warnings)

        /// <summary>
        /// The OCPI connector of the given charging connector: its plug type, and the current type and maximum voltage,
        /// current and power of its EVSE.
        /// </summary>
        public static Connector? ToOCPI(this POI.ChargingConnector      ChargingConnector,
                                        POI.EVSE                        EVSE,
                                        IEnumerable<Tariff_Id>          TariffIds,
                                        POI.ChargingStationOperator     Operator,
                                        ICollection<Warning>            Warnings)
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
                       Format:                 ConnectorFormats.SOCKET,
                       PowerType:              powerType,
                       MaxVoltage:             EVSE.MaxVoltage ?? (isDC ? Volt.  FromV(400) : Volt.  FromV(230)),
                       MaxAmperage:            EVSE.MaxCurrent ?? (isDC ? Ampere.FromA(50)  : Ampere.FromA(16)),
                       MaxElectricPower:       EVSE.MaxPower,
                       TariffIds:              TariffIds,
                       TermsAndConditionsURL:  Operator.TermsAndConditionsURL,
                       Created:                EVSE.Created,
                       LastUpdated:            EVSE.LastChangeDate
                   );

        }

        #endregion

        #region ToOCPI(ChargingTariff, Warnings)

        /// <summary>
        /// The OCPI tariff of the given charging tariff, or null when it has price components OCPI 2.3.0 does not know.
        /// </summary>
        public static Tariff? ToOCPI(this POI.ChargingTariff  ChargingTariff,
                                     ICollection<Warning>     Warnings,
                                     DateTimeOffset?          NotBefore   = null)
        {

            var tariffId = ChargingTariff.Id.ToOCPITariffId();

            if (!tariffId.HasValue)
            {
                Warnings.Add(Warning.Create($"The charging tariff identification '{ChargingTariff.Id}' is no OCPI tariff identification!"));
                return null;
            }

            var elements = new List<TariffElement>();

            foreach (var element in ChargingTariff.Elements)
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
                       TaxIncluded:    TaxIncluded.NA,
                       TariffAltText:  ToDisplayTexts(ChargingTariff.Name).Concat(ToDisplayTexts(ChargingTariff.Description)),
                       TariffAltURL:   ChargingTariff.TariffURL,
                       EnergyMix:      ChargingTariff.EnergyMix?.ToOCPI(),
                       Created:        ChargingTariff.Created,
                       LastUpdated:    NotBefore.HasValue && NotBefore.Value > ChargingTariff.LastChangeDate
                                           ? NotBefore.Value
                                           : ChargingTariff.LastChangeDate
                   );

        }

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
                   DayOfWeek:    Restrictions.DaysOfWeek
               );

        #endregion


        #region ToPOI3(Location, Operator, Existing, Language, Warnings)

        /// <summary>
        /// The charging pool of the given OCPI location, as JSON: the existing one, if any, with what OCPI says about it.
        /// What only POI3 knows is kept; EVSEs stay in their charging stations, new ones go into the first charging station
        /// of the pool, or into a new one named after the location.
        /// </summary>
        /// <param name="Location">An OCPI location.</param>
        /// <param name="OperatorId">The operator of the location.</param>
        /// <param name="Existing">The JSON of the existing charging pool, if any.</param>
        /// <param name="Language">The language of texts OCPI gives without one.</param>
        /// <param name="Warnings">Where to tell what could not be mapped.</param>
        public static JObject? ToPOI3(this Location                    Location,
                                      POI.ChargingStationOperator_Id   OperatorId,
                                      JObject?                         Existing,
                                      Languages                        Language,
                                      ICollection<Warning>             Warnings)
        {

            var poolId = ToPOI3PoolId(OperatorId, Location.Id);

            if (!poolId.HasValue)
            {
                Warnings.Add(Warning.Create($"The location identification '{Location.Id}' is no charging pool identification!"));
                return null;
            }

            #region The charging pool as OCPI tells it

            var mappedEVSEs = new List<POI.EVSE>();

            foreach (var evse in Location.EVSEs)
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

            var capabilities = Location.EVSEs.SelectMany(evse => evse.Capabilities).ToHashSet();

            var mapped    = new POI.ChargingPool(
                                Id:                   poolId.Value,
                                Name:                 Location.Name is not null ? I18NString.Create(Language, Location.Name) : null,
                                Address:              address,
                                GeoLocation:          Location.Coordinates,
                                ParkingType:          Location.ParkingType.HasValue ? POI.ParkingType.TryParse(Location.ParkingType.Value.ToString()) : null,
                                HotlinePhoneNumber:   Location.HelpPhone,
                                OpeningTimes:         Location.OpeningTimes is not null ? new ImmutableOpeningTimes(Location.OpeningTimes.ToPOI3()) : null,
                                EnergyMix:            Location.EnergyMix?.ToPOI3(Language),
                                Images:               Location.Images.Select(ToPOI3),
                                TimeZone:             Time_Zone.TryParse(Location.TimeZone),
                                Facilities:           Location.Facilities.Select(facility => POI.Facility.TryParse(facility.ToString())).
                                                                          Where (facility => facility.HasValue).
                                                                          Select(facility => facility!.Value),
                                PaymentOptions:       PaymentOptionsOf(capabilities),
                                AuthenticationModes:  AuthenticationModesOf(capabilities),
                                ArrivalInstructions:  ToI18NString(Location.Directions),
                                ChargingWhenClosed:   Location.ChargingWhenClosed,
                                ParkingRestrictions:  Location.EVSEs.SelectMany(evse => evse.ParkingRestrictions).
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
                                                         Features:    capabilities.Contains(Capability.RESERVABLE) ? [ POI.ChargingStationFeature.Reservable ] : null,
                                                         Created:     Location.Created,
                                                         LastChange:  Location.LastUpdated
                                                     ).ToJSON()
                                                 );

                return mapped;

            }

            #endregion

            #region Existing: what OCPI tells, onto it

            var desired = (JObject) Existing.DeepClone();

            Overlay(desired, mapped, PoolProperties);

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
                        Overlay(evseJSON, mappedEVSE, EVSEProperties);
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

                connectors.Add(new POI.ChargingConnector(POI.ChargingConnector_Id.Parse(connector.Id.ToString()), type.Value));

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

            var tariffIds = EVSE.Connectors.SelectMany(connector => connector.TariffIds).
                                            Select    (tariffId  => ToPOI3TariffId(OperatorId, tariffId)).
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
                       Created:             EVSE.Created,
                       LastChange:          EVSE.LastUpdated
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
                                                                                           priceComponent.Price,
                                                                                           priceComponent.VAT,
                                                                                           priceComponent.Type.ToString() == "FLAT" ? null : priceComponent.StepSize
                                                                                       )).ToArray();

                var restrictions    = element.TariffRestrictions;

                if (restrictions?.Reservation is not null)
                    Warnings.Add(Warning.Create($"The reservation restriction of the tariff '{Tariff.Id}' has no counterpart and is left out!"));

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
                                           DaysOfWeek:   restrictions.DayOfWeek
                                       )
                             ));

            }

            if (Tariff.TariffType.HasValue || Tariff.MinPrice.HasValue || Tariff.MaxPrice.HasValue || Tariff.Start.HasValue || Tariff.End.HasValue)
                Warnings.Add(Warning.Create($"The tariff type, minimum and maximum price, start and end of the tariff '{Tariff.Id}' have no counterpart and are left out!"));

            return new POI.ChargingTariff(
                       Id:           tariffId.Value,
                       Currency:     Tariff.Currency,
                       Elements:     elements,
                       Name:         ToI18NString(Tariff.TariffAltText),
                       TariffURL:    Tariff.TariffAltURL,
                       EnergyMix:    Tariff.EnergyMix?.ToPOI3(Language),
                       Created:      Tariff.Created,
                       LastChange:   Tariff.LastUpdated
                   );

        }

        #endregion


        #region (internal) PoolProperties / EVSEProperties

        /// <summary>
        /// The properties of a charging pool OCPI tells: imported onto an existing charging pool, the others are kept.
        /// </summary>
        internal static readonly String[] PoolProperties = [
            "name", "address", "geoLocation", "timeZone", "parkingType", "hotline", "openingTimes", "energyMix", "images",
            "facilities", "arrivalInstructions", "chargingWhenClosed", "relatedLocations"
        ];

        /// <summary>
        /// The properties of an EVSE OCPI tells: imported onto an existing EVSE, the others are kept.
        /// </summary>
        internal static readonly String[] EVSEProperties = [
            "physicalReference", "currentTypes", "maxVoltage", "maxCurrent", "maxPower", "chargingConnectors",
            "chargingTariffIds", "images", "geoLocation"
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

        private static EnergyMix ToOCPI(this POI.EnergyMix EnergyMix)
        {

            var sources = EnergyMix.EnergySources.
                              Select(source => (Category: EnergySourceCategory.TryParse(source.Value.ToString()), source.Percent)).
                              Where (source => source.Category.HasValue).
                              Select(source => new EnergySource(source.Category!.Value, source.Percent)).
                              ToArray();

            var green   = sources.Length > 0 &&
                          sources.All(source => source.Source == EnergySourceCategory.GENERAL_GREEN ||
                                                source.Source == EnergySourceCategory.SOLAR         ||
                                                source.Source == EnergySourceCategory.WIND          ||
                                                source.Source == EnergySourceCategory.WATER);

            return new EnergyMix(green,
                                 sources,
                                 null,
                                 FirstText(EnergyMix.SupplierName),
                                 FirstText(EnergyMix.ProductName));

        }

        private static POI.EnergyMix ToPOI3(this EnergyMix EnergyMix, Languages Language)

            => new (EnergyMix.EnergySources.
                        Select(source => (Category: POI.EnergySourceCategory.TryParse(source.Source.ToString()), source.Percentage)).
                        Where (source => source.Category.HasValue).
                        Select(source => new PercentageOf<POI.EnergySourceCategory>(source.Category!.Value, (Single) source.Percentage)),
                    [],
                    new ImmutableI18NString(I18NString.Create(Language, EnergyMix.SupplierName      ?? "")),
                    new ImmutableI18NString(I18NString.Create(Language, EnergyMix.EnergyProductName ?? "")));

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
        /// The tariffs of an EVSE: its own, and those of the EVSE, charging station and charging pool groups it belongs to.
        /// </summary>
        private static IEnumerable<Tariff_Id> TariffIdsOf(POI.EVSE                     EVSE,
                                                          POI.ChargingStation          ChargingStation,
                                                          POI.ChargingPool             ChargingPool,
                                                          POI.ChargingStationOperator  Operator)
        {

            var tariffIds = new List<POI.ChargingTariff_Id>(EVSE.ChargingTariffIds);

            tariffIds.AddRange(Operator.EVSEGroups.           Where(group => group.ChargingTariffId.HasValue && group.EVSEIds.           Contains(EVSE.Id)).           Select(group => group.ChargingTariffId!.Value));
            tariffIds.AddRange(Operator.ChargingStationGroups.Where(group => group.ChargingTariffId.HasValue && group.ChargingStationIds.Contains(ChargingStation.Id)).Select(group => group.ChargingTariffId!.Value));
            tariffIds.AddRange(Operator.ChargingPoolGroups.   Where(group => group.ChargingTariffId.HasValue && group.ChargingPoolIds.   Contains(ChargingPool.Id)).   Select(group => group.ChargingTariffId!.Value));

            return tariffIds.Distinct().
                             Select(tariffId => tariffId.ToOCPITariffId()).
                             Where (tariffId => tariffId.HasValue).
                             Select(tariffId => tariffId!.Value);

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
