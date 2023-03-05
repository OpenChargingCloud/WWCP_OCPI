/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
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

using cloud.charging.open.protocols.OCPIv2_1_1.HTTP;
using cloud.charging.open.protocols.WWCP;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSE identifications to OCPI EVSE identifications.
    /// </summary>
    /// <param name="EVSEId">A WWCP EVSE identification.</param>
    public delegate EVSE_Id                  WWCPEVSEId_2_EVSEId_Delegate               (WWCP.EVSE_Id           EVSEId);

    /// <summary>
    /// A delegate which allows you to modify the convertion from OCPI EVSE identifications to WWCP EVSE identifications.
    /// </summary>
    /// <param name="EVSEId">An EVSE identification.</param>
    public delegate WWCP.EVSE_Id             EVSEId_2_WWCPEVSEId_Delegate               (EVSE_Id                EVSEId);


    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSEs to OCPI EVSEs.
    /// </summary>
    /// <param name="WWCPEVSE">A WWCP EVSE.</param>
    /// <param name="OCPIEVSE">An OCPI EVSE.</param>
    public delegate EVSE                     WWCPEVSE_2_EVSE_Delegate                   (WWCP.IEVSE             WWCPEVSE,
                                                                                         EVSE                   OCPIEVSE);

    /// <summary>
    /// A delegate which allows you to modify the convertion from OCPI EVSEs to WWCP EVSEs.
    /// </summary>
    /// <param name="WWCPEVSE">A WWCP EVSE.</param>
    public delegate WWCP.IEVSE               EVSE_2_WWCPEVSE_Delegate                   (EVSE                   EVSE);


    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSE status updates to OCPI EVSE status types.
    /// </summary>
    /// <param name="WWCPEVSEStatusUpdate">A WWCP EVSE status update.</param>
    /// <param name="OCPIStatusType">An OICP status type.</param>
    public delegate StatusType               WWCPEVSEStatusUpdate_2_StatusType_Delegate (WWCP.EVSEStatusUpdate  WWCPEVSEStatusUpdate,
                                                                                         StatusType             OCPIStatusType);

    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSE status updates to OCPI EVSE status types.
    /// </summary>
    /// <param name="OCPIStatusType">An OICP status type.</param>
    public delegate WWCP.EVSEStatusUpdate    StatusType_2_WWCPEVSEStatusUpdate_Delegate (StatusType             OCPIStatusType);


    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP charge detail records to OICP charge detail records.
    /// </summary>
    /// <param name="WWCPChargeDetailRecord">A WWCP charge detail record.</param>
    /// <param name="OCIPCDR">An OICP charge detail record.</param>
    public delegate CDR                      WWCPChargeDetailRecord_2_CDR_Delegate      (WWCP.ChargeDetailRecord  WWCPChargeDetailRecord,
                                                                                         CDR                      OCIPCDR);

    /// <summary>
    /// A delegate which allows you to modify the convertion from OCPI charge detail records to WWCP charge detail records.
    /// </summary>
    /// <param name="OCIPCDR">An OICP charge detail record.</param>
    public delegate WWCP.ChargeDetailRecord  CDR_2_WWCPChargeDetailRecord_Delegate      (CDR                      OCIPCDR);



    /// <summary>
    /// Helper methods to map OCPI data structures to
    /// WWCP data structures and vice versa.
    /// </summary>
    public static class OCPIMapper
    {

        #region AsWWCPEVSEStatus(this EVSEStatus)

        /// <summary>
        /// Convert the given OCPI EVSE/connector status into a corresponding WWCP EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">An OCPI EVSE/connector status.</param>
        public static WWCP.EVSEStatusTypes AsWWCPEVSEStatus(this StatusType EVSEStatus)
        {

            if (EVSEStatus == StatusType.AVAILABLE)
                return WWCP.EVSEStatusTypes.Available;

            if (EVSEStatus == StatusType.BLOCKED)
                return WWCP.EVSEStatusTypes.OutOfService;

            if (EVSEStatus == StatusType.CHARGING)
                return WWCP.EVSEStatusTypes.Charging;

            if (EVSEStatus == StatusType.INOPERATIVE)
                return WWCP.EVSEStatusTypes.OutOfService;

            if (EVSEStatus == StatusType.OUTOFORDER)
                return WWCP.EVSEStatusTypes.Error;

            //if (EVSEStatus == StatusType.PLANNED)
            //    return WWCP.EVSEStatusTypes.Planned;

            //if (EVSEStatus == StatusType.REMOVED)
            //    return WWCP.EVSEStatusTypes.Removed;

            if (EVSEStatus == StatusType.RESERVED)
                return WWCP.EVSEStatusTypes.Reserved;

            return WWCP.EVSEStatusTypes.Unspecified;

        }

        #endregion

        #region ToOCPI(this EVSEStatus)

        /// <summary>
        /// Convert a WWCP EVSE status into OCPI EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">A WWCP EVSE status.</param>
        public static StatusType ToOCPI(this WWCP.EVSEStatusTypes EVSEStatus)
        {

            if      (EVSEStatus == WWCP.EVSEStatusTypes.Available)
                return StatusType.AVAILABLE;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Blocked)
                return StatusType.BLOCKED;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Charging)
                return StatusType.CHARGING;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.OutOfService)
                return StatusType.INOPERATIVE;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Error)
                return StatusType.OUTOFORDER;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.InDeployment)
                return StatusType.PLANNED;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Removed)
                return StatusType.REMOVED;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Reserved)
                return StatusType.RESERVED;

            else
                return StatusType.UNKNOWN;

        }

        #endregion


        #region ToOCPI(this I18NString)

        public static IEnumerable<DisplayText> ToOCPI(this I18NString I18NString)

            => I18NString.Select(text => new DisplayText(text.Language,
                                                         text.Text));

        #endregion

        #region ToWWCP(this DisplayTexts)

        public static I18NString ToOCPI(this IEnumerable<DisplayText> DisplayTexts)
        {

            var i18nString = I18NString.Empty;

            foreach (var displayText in DisplayTexts)
                i18nString.Set(displayText.Language,
                               displayText.Text);

            return i18nString;

        }

        #endregion


        #region ToOCPI_EVSEUId(this EVSEId)

        public static EVSE_UId? ToOCPI_EVSEUId(this WWCP.EVSE_Id EVSEId)

            => EVSE_UId.TryParse(EVSEId.ToString());

        public static EVSE_UId? ToOCPI_EVSEUId(this WWCP.EVSE_Id? EVSEId)

            => EVSEId.HasValue
                   ? EVSE_UId.TryParse(EVSEId.Value.ToString())
                   : null;

        #endregion


        #region ToOCPI_EVSEId(this EVSEId)

        public static EVSE_Id? ToOCPI_EVSEId(this WWCP.EVSE_Id EVSEId)

            => EVSE_Id.TryParse(EVSEId.ToString());

        public static EVSE_Id? ToOCPI_EVSEId(this WWCP.EVSE_Id? EVSEId)

            => EVSEId.HasValue
                   ? EVSE_Id.TryParse(EVSEId.Value.ToString())
                   : null;

        #endregion


        #region ToOCPI(this ChargingPool,  ref Warnings, IncludeEVSEIds = null)

        public static Location? ToOCPI(this WWCP.IChargingPool  ChargingPool,
                                       ref List<Warning>        Warnings,
                                       IncludeEVSEIdDelegate?   IncludeEVSEIds = null)
        {

            var location = ChargingPool.ToOCPI(out var warnings,
                                               IncludeEVSEIds);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return location;

        }

        #endregion

        #region ToOCPI(this ChargingPool,  out Warnings, IncludeEVSEIds = null)

        public static Location? ToOCPI(this WWCP.IChargingPool   ChargingPool,
                                       out IEnumerable<Warning>  Warnings,
                                       IncludeEVSEIdDelegate?    IncludeEVSEIds = null)
        {

            var includeEVSEIds  = IncludeEVSEIds ?? (evseId => true);
            var warnings        = new List<Warning>();

            if (ChargingPool.Operator is null)
            {
                warnings.Add(Warning.Create(Languages.en, "The given charging location must have a valid charging station operator!"));
                Warnings = warnings;
                return null;
            }

            var countryCode  = CountryCode.TryParse(ChargingPool.Operator.Id.CountryCode.Alpha2Code);

            if (!countryCode.HasValue)
            {
                warnings.Add(Warning.Create(Languages.en, $"The given charging station operator identificaton '{ChargingPool.Id.OperatorId}' could not be converted to an OCPI country code!"));
                Warnings = warnings;
                return null;
            }

            var partyId      = Party_Id.   TryParse(ChargingPool.Operator.Id.Suffix);

            if (!partyId.HasValue)
            {
                warnings.Add(Warning.Create(Languages.en, $"The given charging station operator identificaton '{ChargingPool.Id.OperatorId}' could not be converted to an OCPI party identification!"));
                Warnings = warnings;
                return null;
            }

            var locationId   = Location_Id.TryParse(ChargingPool.Id.Suffix);

            if (!locationId.HasValue)
            {
                warnings.Add(Warning.Create(Languages.en, $"The given charging pool identificaton '{ChargingPool.Id}' could not be converted to an OCPI location identification!"));
                Warnings = warnings;
                return null;
            }

            if (ChargingPool.Address is null)
            {
                warnings.Add(Warning.Create(Languages.en, "The given charging location must have a valid address!"));
                Warnings = warnings;
                return null;
            }

            if (ChargingPool.GeoLocation is null)
            {
                warnings.Add(Warning.Create(Languages.en, "The given charging location must have a valid geo location!"));
                Warnings = warnings;
                return null;
            }

            try
            {

                Warnings = Array.Empty<Warning>();

                var evses = new List<EVSE>();

                foreach (var evse in ChargingPool.SelectMany(station => station.EVSEs).
                                                  Where     (evse    => includeEVSEIds(evse.Id)))
                {

                    var ocpiEVSE = evse.ToOCPI(ref warnings);

                    if (ocpiEVSE is not null)
                        evses.Add(ocpiEVSE);

                }

                return new Location(

                           CountryCode:          countryCode.Value,
                           PartyId:              partyId    .Value,
                           Id:                   locationId .Value,
                           LocationType:         LocationType.ON_STREET, // ????
                           Address:              ChargingPool.Address.HouseNumber is not null
                                                     ? $"{ChargingPool.Address.Street} {ChargingPool.Address.HouseNumber}"
                                                     : ChargingPool.Address.Street,
                           City:                 ChargingPool.Address.City.FirstText(),
                           PostalCode:           ChargingPool.Address.PostalCode,
                           Country:              ChargingPool.Address.Country,
                           Coordinates:          ChargingPool.GeoLocation.Value,

                           Name:                 ChargingPool.Name.FirstText(),
                           RelatedLocations:     Array.Empty<AdditionalGeoLocation>(),
                           EVSEs:                evses,
                           Directions:           Array.Empty<DisplayText>(),
                           Operator:             new BusinessDetails(
                                                     ChargingPool.Operator.Name.FirstText(),
                                                     ChargingPool.Operator.Homepage
                                                 ),
                           SubOperator:          null,
                           Owner:                null,
                           Facilities:           Array.Empty<Facilities>(),
                           Timezone:             ChargingPool.Address.TimeZone?.ToString(),
                           OpeningTimes:         null,
                           ChargingWhenClosed:   null,
                           Images:               Array.Empty<Image>(),
                           EnergyMix:            null,

                           LastUpdated:          Timestamp.Now

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create(Languages.en, $"Could not convert the given charging pool '{ChargingPool.Id}' to OCPI: " + ex.Message));
                Warnings = warnings;
            }

            return null;

        }

        #endregion

        #region ToOCPI(this ChargingPools, ref Warnings, IncludeEVSEIds = null)

        public static IEnumerable<Location> ToOCPI(this IEnumerable<WWCP.ChargingPool>  ChargingPools,
                                                   ref List<Warning>                    Warnings,
                                                   IncludeEVSEIdDelegate?               IncludeEVSEIds = null)
        {

            var locations = ChargingPools.ToOCPI(out var warnings,
                                                 IncludeEVSEIds);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return locations;

        }

        #endregion

        #region ToOCPI(this ChargingPools, out Warnings, IncludeEVSEIds = null)

        public static IEnumerable<Location> ToOCPI(this IEnumerable<WWCP.ChargingPool>  ChargingPools,
                                                   out IEnumerable<Warning>             Warnings,
                                                   IncludeEVSEIdDelegate?               IncludeEVSEIds = null)
        {

            var warnings   = new List<Warning>();
            var locations  = new HashSet<Location>();

            foreach (var chargingPool in ChargingPools)
            {

                try
                {

                    var chargingPool2 = chargingPool.ToOCPI(out var warning,
                                                            IncludeEVSEIds);

                    if (chargingPool2 is not null)
                        locations.Add(chargingPool2);

                    if (warning is not null && warning.Any())
                        warnings.AddRange(warning);

                }
                catch (Exception ex)
                {
                    warnings.Add(Warning.Create(Languages.en, $"Could not convert the given charging pool '{chargingPool.Id}' to OCPI: " + ex.Message));
                }

            }

            Warnings = warnings.ToArray();
            return locations;

        }

        #endregion


        #region ToOCPI(this EVSE,  ref Warnings)

        public static EVSE? ToOCPI(this WWCP.IEVSE    EVSE,
                                   ref List<Warning>  Warnings)
        {

            var result = EVSE.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return result;

        }

        #endregion

        #region ToOCPI(this EVSE,  out Warnings)

        public static EVSE? ToOCPI(this WWCP.IEVSE           EVSE,
                                   out IEnumerable<Warning>  Warnings)
        {

            var warnings = new List<Warning>();

            try
            {

                var evseUId = EVSE.Id.ToOCPI_EVSEUId();

                if (!evseUId.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE identificaton '{EVSE.Id}' could not be converted to an OCPI EVSE Unique identification!"));
                    Warnings = warnings;
                    return null;
                }


                var evseId  = EVSE.Id.ToOCPI_EVSEId();

                if (!evseId.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE identificaton '{EVSE.Id}' could not be converted to an OCPI EVSE identification!"));
                    Warnings = warnings;
                    return null;
                }


                if (EVSE.ChargingStation is null)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE '{EVSE.Id}' must have a valid charging station!"));
                    Warnings = warnings;
                    return null;
                }

                if (EVSE.ChargingPool is null)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE '{EVSE.Id}' must have a valid charging pool!"));
                    Warnings = warnings;
                    return null;
                }

                var connectors = EVSE.SocketOutlets.
                                      Select (socketOutlet => socketOutlet.ToOCPI(EVSE, ref warnings)).
                                      Where  (connector    => connector is not null).
                                      Cast<Connector>().
                                      ToArray();

                if (!connectors.Any())
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE socket outlets could not be converted to OCPI connectors!"));
                    Warnings = warnings;
                    return null;
                }


                Warnings = Array.Empty<Warning>();

                return new EVSE(

                           UId:                   evseUId.Value,
                           Status:                EVSE.Status.Value.ToOCPI(),
                           Connectors:            connectors,

                           EVSEId:                evseId,
                           StatusSchedule:        Array.Empty<StatusSchedule>(),
                           Capabilities:          Array.Empty<Capability>(),
                           EnergyMeter:           null,
                           FloorLevel:            EVSE.ChargingStation.Address?.FloorLevel ?? EVSE.ChargingPool.Address?.FloorLevel,
                           Coordinates:           EVSE.ChargingStation.GeoLocation         ?? EVSE.ChargingPool.GeoLocation,
                           PhysicalReference:     EVSE.PhysicalReference                   ?? EVSE.ChargingStation.PhysicalReference,
                           Directions:            EVSE.ChargingStation.ArrivalInstructions.ToOCPI(),
                           ParkingRestrictions:   Array.Empty<ParkingRestrictions>(),
                           Images:                Array.Empty<Image>(),

                           LastUpdated:           EVSE.LastChange

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create(Languages.en, $"Could not convert the given EVSE '{EVSE.Id}' to OCPI: " + ex.Message));
                Warnings = warnings;
            }

            return null;

        }

        #endregion

        #region ToOCPI(this EVSEs, ref Warnings)

        public static IEnumerable<EVSE> ToOCPI(this IEnumerable<WWCP.IEVSE>  EVSEs,
                                               ref List<Warning>             Warnings)
        {

            var evses = EVSEs.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return evses;

        }

        #endregion

        #region ToOCPI(this EVSEs, out Warnings)

        public static IEnumerable<EVSE> ToOCPI(this IEnumerable<WWCP.IEVSE>  EVSEs,
                                               out IEnumerable<Warning>      Warnings)
        {

            var warnings  = new List<Warning>();
            var evses     = new HashSet<EVSE>();

            foreach (var evse in EVSEs)
            {

                try
                {

                    var evse2 = evse.ToOCPI(out var warning);

                    if (evse2 is not null)
                        evses.Add(evse2);

                    if (warning is not null && warning.Any())
                        warnings.AddRange(warning);

                }
                catch (Exception ex)
                {
                    warnings.Add(Warning.Create(Languages.en, $"Could not convert the given EVSE '{evse.Id}' to OCPI: " + ex.Message));
                }

            }

            Warnings = warnings.ToArray();
            return evses;

        }

        #endregion



        #region ToOCPI(this SocketOutletId)

        public static Connector_Id? ToOCPI(this WWCP.SocketOutlet_Id SocketOutletId)

            => Connector_Id.TryParse(SocketOutletId.ToString());

        public static Connector_Id? ToOCPI(this WWCP.SocketOutlet_Id? SocketOutletId)

            => SocketOutletId.HasValue
                   ? Connector_Id.TryParse(SocketOutletId.Value.ToString())
                   : null;

        #endregion


        #region ToOCPI(this CurrentType)

        public static PowerTypes? ToOCPI(this WWCP.CurrentTypes CurrentType)

            => CurrentType switch {
                   WWCP.CurrentTypes.AC_OnePhase     => PowerTypes.AC_1_PHASE,
                   WWCP.CurrentTypes.AC_ThreePhases  => PowerTypes.AC_3_PHASE,
                   WWCP.CurrentTypes.DC              => PowerTypes.DC,
                   _                                 => null
               };

        public static PowerTypes? ToOCPI(this WWCP.CurrentTypes? CurrentType)

            => CurrentType.HasValue
                   ? CurrentType.Value.ToOCPI()
                   : null;

        #endregion


        #region ToOCPI(this PlugType)

        public static ConnectorType? ToOCPI(this WWCP.PlugTypes PlugType)

            => PlugType switch {
                   //WWCP.PlugTypes.SmallPaddleInductive          => 
                   //WWCP.PlugTypes.LargePaddleInductive          => 
                   //WWCP.PlugTypes.AVCONConnector                => 
                   //WWCP.PlugTypes.TeslaConnector                => 
                   WWCP.PlugTypes.TESLA_Roadster                => ConnectorType.TESLA_R,
                   WWCP.PlugTypes.TESLA_ModelS                  => ConnectorType.TESLA_S,
                   //WWCP.PlugTypes.NEMA5_20                      => 
                   WWCP.PlugTypes.TypeEFrenchStandard           => ConnectorType.DOMESTIC_E,
                   WWCP.PlugTypes.TypeFSchuko                   => ConnectorType.DOMESTIC_F,
                   WWCP.PlugTypes.TypeGBritishStandard          => ConnectorType.DOMESTIC_G,
                   WWCP.PlugTypes.TypeJSwissStandard            => ConnectorType.DOMESTIC_J,
                   WWCP.PlugTypes.Type1Connector_CableAttached  => ConnectorType.IEC_62196_T1,
                   WWCP.PlugTypes.Type2Outlet                   => ConnectorType.IEC_62196_T2,
                   WWCP.PlugTypes.Type2Connector_CableAttached  => ConnectorType.IEC_62196_T2,
                   WWCP.PlugTypes.Type3Outlet                   => ConnectorType.IEC_62196_T3A,
                   WWCP.PlugTypes.IEC60309SinglePhase           => ConnectorType.IEC_60309_2_single_16,
                   WWCP.PlugTypes.IEC60309ThreePhase            => ConnectorType.IEC_60309_2_three_16,
                   WWCP.PlugTypes.CCSCombo1Plug_CableAttached   => ConnectorType.IEC_62196_T1_COMBO,
                   WWCP.PlugTypes.CCSCombo2Plug_CableAttached   => ConnectorType.IEC_62196_T2_COMBO,
                   WWCP.PlugTypes.CHAdeMO                       => ConnectorType.CHADEMO,
                   //WWCP.PlugTypes.CEE3                          => 
                   //WWCP.PlugTypes.CEE5                          => 
                   _                                            => null
               };

        public static ConnectorType? ToOCPI(this WWCP.PlugTypes? PlugType)

            => PlugType.HasValue
                   ? PlugType.Value.ToOCPI()
                   : null;

        #endregion


        #region ToOCPI(this SocketOutlet, EVSE,  ref Warnings)

        public static Connector? ToOCPI(this WWCP.SocketOutlet  SocketOutlet,
                                        WWCP.IEVSE              EVSE,
                                        ref List<Warning>       Warnings)
        {

            var result = SocketOutlet.ToOCPI(EVSE, out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return result;

        }

        #endregion

        #region ToOCPI(this SocketOutlet, EVSE,  out Warnings)

        public static Connector? ToOCPI(this WWCP.SocketOutlet    SocketOutlet,
                                        WWCP.IEVSE                EVSE,
                                        out IEnumerable<Warning>  Warnings)
        {

            var warnings = new List<Warning>();

            try
            {

                if (EVSE is null)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE must not be null!"));
                    Warnings = warnings;
                    return null;
                }

                var connectorId  = Connector_Id.Parse(SocketOutlet.Id.HasValue
                                                          ? SocketOutlet.Id.Value.ToString()
                                                          : "1");

                var powerType    = EVSE.CurrentType.ToOCPI();

                if (!powerType.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE current type '{EVSE.CurrentType}' could not be converted to an OCPI power type!"));
                    Warnings = warnings;
                    return null;
                }

                var standard     = SocketOutlet.Plug.ToOCPI();

                if (!standard.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given socket outlet plug '{SocketOutlet.Plug}' could not be converted to an OCPI connector standard!"));
                    Warnings = warnings;
                    return null;
                }


                Warnings = Array.Empty<Warning>();

                return new Connector(

                           Id:                      connectorId,
                           Standard:                standard.Value,
                           Format:                  SocketOutlet.CableAttached switch {
                                                        true  => ConnectorFormats.CABLE,
                                                        _     => ConnectorFormats.SOCKET
                                                    },
                           PowerType:               powerType.Value,
                           Voltage:                 (UInt16) (EVSE.AverageVoltage ?? powerType.Value switch {
                                                        PowerTypes.AC_1_PHASE  => 240,
                                                        PowerTypes.AC_3_PHASE  => 400,
                                                        PowerTypes.DC          => 0,
                                                        _                      => 0
                                                    }),
                           Amperage:                (UInt16) (EVSE.MaxCurrent     ?? powerType.Value switch {
                                                        PowerTypes.AC_1_PHASE  => 16,
                                                        PowerTypes.AC_3_PHASE  => 16,
                                                        PowerTypes.DC          => 50,
                                                        _                      => 0
                                                    }),

                           TariffId:                null,
                           TermsAndConditionsURL:   null,

                           LastUpdated:             EVSE.LastChange

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create(Languages.en, $"Could not convert the given socket outlet to OCPI: " + ex.Message));
                Warnings = warnings;
            }

            return null;

        }

        #endregion


        #region ToOCPI(this AuthMethod)

        public static AuthMethods? ToOCPI(this AuthMethod AuthMethod)
        {

            if (AuthMethod == AuthMethod.AUTH_REQUEST)
                return AuthMethods.AUTH_REQUEST;

            if (AuthMethod == AuthMethod.WHITELIST)
                return AuthMethods.WHITELIST;

            return null;

        }

        public static AuthMethods? ToOCPI(this AuthMethod? AuthMethod)

            => AuthMethod.HasValue
                   ? AuthMethod.Value.ToOCPI()
                   : null;

        #endregion

        #region ToWWCP(this AuthMethod)

        public static AuthMethod? ToWWCP(this AuthMethods AuthMethod)
        {

            if (AuthMethod == AuthMethods.AUTH_REQUEST)
                return WWCP.AuthMethod.AUTH_REQUEST;

            if (AuthMethod == AuthMethods.WHITELIST)
                return WWCP.AuthMethod.WHITELIST;

            return null;

        }

        public static AuthMethod? ToWWCP(this AuthMethods? AuthMethod)

            => AuthMethod.HasValue
                   ? AuthMethod.Value.ToWWCP()
                   : null;

        #endregion


        #region ToOCPI(this EnergyMeterId)

        public static Meter_Id? ToOCPI(this EnergyMeter_Id EnergyMeterId)

            => Meter_Id.Parse(EnergyMeterId.ToString());

        public static Meter_Id? ToOCPI(this EnergyMeter_Id? EnergyMeterId)

            => EnergyMeterId.HasValue
                   ? EnergyMeterId.Value.ToOCPI()
                   : null;

        #endregion

        #region ToWWCP(this MeterId)

        public static EnergyMeter_Id? ToWWCP(this Meter_Id MeterId)

            => EnergyMeter_Id.Parse(MeterId.ToString());

        public static EnergyMeter_Id? ToWWCP(this Meter_Id? MeterId)

            => MeterId.HasValue
                   ? MeterId.Value.ToWWCP()
                   : null;

        #endregion


        #region ToOCPI(this ChargeDetailRecord, ref Warnings)

        public static CDR? ToOCPI(this WWCP.ChargeDetailRecord  ChargeDetailRecord,
                                  ref List<Warning>             Warnings)
        {

            var result = ChargeDetailRecord.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return result;

        }

        #endregion

        #region ToOCPI(this ChargeDetailRecord, out Warnings)

        public static CDR? ToOCPI(this WWCP.ChargeDetailRecord  ChargeDetailRecord,
                                  out IEnumerable<Warning>      Warnings)
        {

            var warnings = new List<Warning>();

            try
            {

                if (ChargeDetailRecord is null)
                {
                    warnings.Add(Warning.Create(Languages.en, "The given charge detail record must not be null!"));
                    Warnings = warnings;
                    return null;
                }

                if (ChargeDetailRecord.ChargingStationOperator is null)
                {
                    warnings.Add(Warning.Create(Languages.en, "The given charge detail record must have a valid charging station operator!"));
                    Warnings = warnings;
                    return null;
                }

                if (!ChargeDetailRecord.SessionTime.EndTime.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, "The session endtime of the given charge detail record must not be null!"));
                    Warnings = warnings;
                    return null;
                }

                if (!ChargeDetailRecord.SessionTime.Duration.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, "The session time duration of the given charge detail record must not be null!"));
                    Warnings = warnings;
                    return null;
                }

                if (!ChargeDetailRecord.AuthMethodStart.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, "The authentication (verification) method used for starting of the given charge detail record must not be null!"));
                    Warnings = warnings;
                    return null;
                }

                var authMethod = ChargeDetailRecord.AuthMethodStart.ToOCPI();

                if (!authMethod.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, "The authentication (verification) method used for starting of the given charge detail record is invalid!"));
                    Warnings = warnings;
                    return null;
                }

                if (ChargeDetailRecord.EVSE is null)
                {
                    warnings.Add(Warning.Create(Languages.en, "The EVSE of the given charge detail record must not be null!"));
                    Warnings = warnings;
                    return null;
                }

                if (ChargeDetailRecord.ChargingStation is null)
                {
                    warnings.Add(Warning.Create(Languages.en, "The charging station of the given charge detail record must not be null!"));
                    Warnings = warnings;
                    return null;
                }

                if (ChargeDetailRecord.ChargingPool is null)
                {
                    warnings.Add(Warning.Create(Languages.en, "The charging pool of the given charge detail record must not be null!"));
                    Warnings = warnings;
                    return null;
                }

                var filteredLocation = ChargeDetailRecord.ChargingPool.ToOCPI(ref warnings,
                                                                              evseId => evseId == ChargeDetailRecord.EVSE.Id);

                if (filteredLocation is null)
                {
                    warnings.Add(Warning.Create(Languages.en, "The charging location of the given charge detail record could not be calculated!"));
                    Warnings = warnings;
                    return null;
                }

                if (ChargeDetailRecord.Currency is null)
                {
                    warnings.Add(Warning.Create(Languages.en, "The currency of the given charge detail record must not be null!"));
                    Warnings = warnings;
                    return null;
                }

                var chargingPeriods = new List<ChargingPeriod>();

                foreach (var energyMeteringValue in ChargeDetailRecord.EnergyMeteringValues)
                {
                    chargingPeriods.Add(
                        new ChargingPeriod(
                            energyMeteringValue.Timestamp,
                            new CDRDimension[] {
                                new CDRDimension(
                                    CDRDimensionType.ENERGY_EXPORT,
                                    energyMeteringValue.Value
                                )
                            }
                        )
                    );
                }

                if (!ChargeDetailRecord.ChargingPrice.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, "The charging price of the given charge detail record must not be null!"));
                    Warnings = warnings;
                    return null;
                }

                if (!ChargeDetailRecord.ConsumedEnergy.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, "The consumed energy of the given charge detail record must not be null!"));
                    Warnings = warnings;
                    return null;
                }

                if (ChargeDetailRecord.EnergyMeteringValues.Count() < 2)
                {
                    warnings.Add(Warning.Create(Languages.en, "At least two energy metering values are expected!"));
                    Warnings = warnings;
                    return null;
                }

                Warnings = Array.Empty<Warning>();

                return new CDR(

                           CountryCode:             CountryCode.Parse(ChargeDetailRecord.ChargingStationOperator.Id.CountryCode.Alpha2Code),
                           PartyId:                 Party_Id.   Parse(ChargeDetailRecord.ChargingStationOperator.Id.Suffix),
                           Id:                      CDR_Id.     Parse(ChargeDetailRecord.Id.ToString()),
                           Start:                   ChargeDetailRecord.SessionTime.StartTime,
                           End:                     ChargeDetailRecord.SessionTime.EndTime. Value,
                           AuthId:                  Auth_Id.    Parse(ChargeDetailRecord.SessionId.ToString()),
                           AuthMethod:              authMethod.Value,
                           Location:                filteredLocation,   //ToDo: Might still have not required connectors!
                           Currency:                Currency.Parse(ChargeDetailRecord.Currency.ISOCode),
                           ChargingPeriods:         chargingPeriods,
                           TotalCost:               ChargeDetailRecord.ChargingPrice.       Value,
                           TotalEnergy:             ChargeDetailRecord.ConsumedEnergy.      Value,
                           TotalTime:               ChargeDetailRecord.SessionTime.Duration.Value,

                           MeterId:                 ChargeDetailRecord.EnergyMeterId.ToOCPI(),
                           EnergyMeter:             null,
                           TransparencySoftwares:   null,
                           Tariffs:                 null,
                           SignedData:              null,
                           TotalParkingTime:        null,
                           Remark:                  null,

                           LastUpdated:             Timestamp.Now

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create(Languages.en, "Could not convert the given charge detail record to OCPI: " + ex.Message));
                Warnings = warnings;
            }

            return null;

        }

        #endregion


    }

}
