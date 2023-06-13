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

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_1_1.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSE identifications to OCPI EVSE unique identifications.
    /// </summary>
    /// <param name="EVSEId">A WWCP EVSE identification.</param>
    public delegate EVSE_UId                 WWCPEVSEId_2_EVSEUId_Delegate              (WWCP.EVSE_Id             EVSEId);

    /// <summary>
    /// A delegate which allows you to modify the convertion from OCPI EVSE unique identifications to WWCP EVSE identifications.
    /// </summary>
    /// <param name="EVSEId">An EVSE identification.</param>
    public delegate WWCP.EVSE_Id             EVSEUId_2_WWCPEVSEId_Delegate              (EVSE_UId                 EVSEUId);


    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSE identifications to OCPI EVSE identifications.
    /// </summary>
    /// <param name="EVSEId">A WWCP EVSE identification.</param>
    public delegate EVSE_Id                  WWCPEVSEId_2_EVSEId_Delegate               (WWCP.EVSE_Id             EVSEId);

    /// <summary>
    /// A delegate which allows you to modify the convertion from OCPI EVSE identifications to WWCP EVSE identifications.
    /// </summary>
    /// <param name="EVSEId">An EVSE identification.</param>
    public delegate WWCP.EVSE_Id             EVSEId_2_WWCPEVSEId_Delegate               (EVSE_Id                  EVSEId);


    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSEs to OCPI EVSEs.
    /// </summary>
    /// <param name="WWCPEVSE">A WWCP EVSE.</param>
    /// <param name="OCPIEVSE">An OCPI EVSE.</param>
    public delegate EVSE                     WWCPEVSE_2_EVSE_Delegate                   (WWCP.IEVSE               WWCPEVSE,
                                                                                         EVSE                     OCPIEVSE);

    /// <summary>
    /// A delegate which allows you to modify the convertion from OCPI EVSEs to WWCP EVSEs.
    /// </summary>
    /// <param name="WWCPEVSE">A WWCP EVSE.</param>
    public delegate WWCP.IEVSE               EVSE_2_WWCPEVSE_Delegate                   (EVSE                     EVSE);


    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSE status updates to OCPI EVSE status types.
    /// </summary>
    /// <param name="WWCPEVSEStatusUpdate">A WWCP EVSE status update.</param>
    /// <param name="OCPIStatusType">An OICP status type.</param>
    public delegate StatusType               WWCPEVSEStatusUpdate_2_StatusType_Delegate (WWCP.EVSEStatusUpdate    WWCPEVSEStatusUpdate,
                                                                                         StatusType               OCPIStatusType);

    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSE status updates to OCPI EVSE status types.
    /// </summary>
    /// <param name="OCPIStatusType">An OICP status type.</param>
    public delegate WWCP.EVSEStatusUpdate    StatusType_2_WWCPEVSEStatusUpdate_Delegate (StatusType               OCPIStatusType);


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

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Offline)
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


        #region ToOCPI(this OpeningTimes)

        public static Hours? ToOCPI(this OpeningTimes OpeningTimes)

            => OpeningTimes.IsOpen24Hours

                   ? Hours.TwentyFourSevenOpen(
                         OpeningTimes.ExceptionalOpenings.ToOCPI(),
                         OpeningTimes.ExceptionalClosings.ToOCPI()
                     )

                   : new Hours(
                         RegularHours:          OpeningTimes.RegularOpenings.Values.
                                                    SelectMany(regularHoursList => regularHoursList.Select(regularHours => new OCPI.RegularHours(
                                                                                                                               regularHours.DayOfWeek,
                                                                                                                               regularHours.PeriodBegin,
                                                                                                                               regularHours.PeriodEnd
                                                                                                                           ))),
                         ExceptionalOpenings:   OpeningTimes.ExceptionalOpenings.ToOCPI(),
                         ExceptionalClosings:   OpeningTimes.ExceptionalClosings.ToOCPI()
                     );


        public static OCPI.ExceptionalPeriod? ToOCPI(this org.GraphDefined.Vanaheimr.Illias.ExceptionalPeriod ExceptionalPeriod)

            => new OCPI.ExceptionalPeriod(
                   ExceptionalPeriod.Begin,
                   ExceptionalPeriod.End
               );

        public static IEnumerable<OCPI.ExceptionalPeriod> ToOCPI(this IEnumerable<org.GraphDefined.Vanaheimr.Illias.ExceptionalPeriod> ExceptionalPeriods)
        {

            var exceptionalPeriods = new List<OCPI.ExceptionalPeriod>();

            foreach (var exceptionalPeriod in ExceptionalPeriods)
            {

                var converted = exceptionalPeriod.ToOCPI();

                if (converted.HasValue)
                    exceptionalPeriods.Add(converted.Value);

            }

            return exceptionalPeriods;

        }

        #endregion


        #region ToOCPI_EVSEUId(this EVSEId)

        public static EVSE_UId? ToOCPI_EVSEUId(this WWCP.EVSE_Id               EVSEId,
                                               WWCPEVSEId_2_EVSEUId_Delegate?  CustomEVSEIdConverter   = null)

            => CustomEVSEIdConverter is not null
                   ? EVSE_UId.TryParse(CustomEVSEIdConverter(EVSEId).ToString())
                   : EVSE_UId.TryParse(EVSEId.ToString());

        public static EVSE_UId? ToOCPI_EVSEUId(this WWCP.EVSE_Id?              EVSEId,
                                               WWCPEVSEId_2_EVSEUId_Delegate?  CustomEVSEIdConverter   = null)

            => EVSEId.HasValue
                   ? EVSEId.Value.ToOCPI_EVSEUId(CustomEVSEIdConverter)
                   : null;

        #endregion

        #region ToOCPI_EVSEId (this EVSEId, CustomEVSEIdConverter = null)

        public static EVSE_Id? ToOCPI_EVSEId(this WWCP.EVSE_Id              EVSEId,
                                             WWCPEVSEId_2_EVSEId_Delegate?  CustomEVSEIdConverter   = null)

            => CustomEVSEIdConverter is not null
                   ? EVSE_Id.TryParse(CustomEVSEIdConverter(EVSEId).ToString())
                   : EVSE_Id.TryParse(EVSEId.ToString());

        public static EVSE_Id? ToOCPI_EVSEId(this WWCP.EVSE_Id?             EVSEId,
                                             WWCPEVSEId_2_EVSEId_Delegate?  CustomEVSEIdConverter   = null)

            => EVSEId.HasValue
                   ? EVSEId.Value.ToOCPI_EVSEId(CustomEVSEIdConverter)
                   : null;

        #endregion


        #region ToWWCP (this EMSPId)

        public static WWCP.EVSE_Id? ToWWCP(this EVSE_Id EVSEId)

            => WWCP.EVSE_Id.TryParse(EVSEId.ToString());

        public static WWCP.EVSE_Id? ToWWCP(this EVSE_Id? EVSEId)

            => EVSEId.HasValue
                   ? EVSEId.Value.ToWWCP()
                   : null;

        #endregion


        #region ToWWCP (this EMSPId)

        public static WWCP.EMobilityProvider_Id? ToWWCP(this EMSP_Id EMSPId)

            => WWCP.EMobilityProvider_Id.TryParse(EMSPId.ToString());

        public static WWCP.EMobilityProvider_Id? ToWWCP(this EMSP_Id? EMSPId)

            => EMSPId.HasValue
                   ? EMSPId.Value.ToWWCP()
                   : null;

        #endregion

        #region ToWWCP (this CPOId)

        public static WWCP.ChargingStationOperator_Id? ToWWCP(this CPO_Id CPOId)

            => WWCP.ChargingStationOperator_Id.TryParse(CPOId.ToString());

        public static WWCP.ChargingStationOperator_Id? ToWWCP(this CPO_Id? CPOId)

            => CPOId.HasValue
                   ? CPOId.Value.ToWWCP()
                   : null;

        #endregion


        #region ToOCPI_Capabilities(this ChargingStation)

        public static IEnumerable<Capability> ToOCPI_Capabilities(this WWCP.IChargingStation ChargingStation)
        {

            var capabilities = new HashSet<Capability>();

            if (ChargingStation.AuthenticationModes.Any(authenticationMode => authenticationMode is WWCP.AuthenticationModes.RFID)       ||
                ChargingStation.UIFeatures.    Contains(WWCP.UIFeatures.    RFID))
                capabilities.Add(Capability.RFID_READER);

            if (ChargingStation.AuthenticationModes.Any(authenticationMode => authenticationMode is WWCP.AuthenticationModes.CreditCard) ||
                ChargingStation.PaymentOptions.Contains(WWCP.PaymentOptions.CreditCard) ||
                ChargingStation.UIFeatures.    Contains(WWCP.UIFeatures.    CreditCard))
                capabilities.Add(Capability.CREDIT_CARD_PAYABLE);

            if (ChargingStation.AuthenticationModes.Any(authenticationMode => authenticationMode is WWCP.AuthenticationModes.DebitCard)  ||
                ChargingStation.PaymentOptions.Contains(WWCP.PaymentOptions.DebitCard) ||
                ChargingStation.UIFeatures.    Contains(WWCP.UIFeatures.    DebitCard))
                capabilities.Add(Capability.DEBIT_CARD_PAYABLE);

            if (ChargingStation.AuthenticationModes.Any(authenticationMode => authenticationMode is WWCP.AuthenticationModes.NFC)        ||
                ChargingStation.UIFeatures.    Contains(WWCP.UIFeatures.    NFC))
                capabilities.Add(Capability.CONTACTLESS_CARD_SUPPORT);

            if (ChargingStation.AuthenticationModes.Any(authenticationMode => authenticationMode is WWCP.AuthenticationModes.PINPAD)     ||
                ChargingStation.UIFeatures.    Contains(WWCP.UIFeatures.    Pinpad))
                capabilities.Add(Capability.PED_TERMINAL);

            if (ChargingStation.AuthenticationModes.Any(authenticationMode => authenticationMode is WWCP.AuthenticationModes.REMOTE))
                capabilities.Add(Capability.REMOTE_START_STOP_CAPABLE);

            // CHIP_CARD_SUPPORT (payment terminal that supports chip cards)



            if (ChargingStation.Features.Contains(WWCP.Features.Reservable))
                capabilities.Add(Capability.RESERVABLE);

            if (ChargingStation.Features.Contains(WWCP.Features.ChargingProfilesSupported))
                capabilities.Add(Capability.CHARGING_PROFILE_CAPABLE);

            if (ChargingStation.Features.Contains(WWCP.Features.ChargingPreferencesSupported))
                capabilities.Add(Capability.CHARGING_PREFERENCES_CAPABLE);

            if (ChargingStation.Features.Contains(WWCP.Features.TokenGroupsSupported))
                capabilities.Add(Capability.TOKEN_GROUP_CAPABLE);

            if (ChargingStation.Features.Contains(WWCP.Features.CSOUnlockSupported))
                capabilities.Add(Capability.UNLOCK_CAPABLE);


            return capabilities;

        }

        #endregion

        #region ToOCPI_Facilities(this Facilities)

        public static IEnumerable<Facilities> ToOCPI(this IEnumerable<WWCP.Facilities> Facilities)
        {

            var facilities = new HashSet<Facilities>();

            foreach (var facility in Facilities)
            {

                switch (facility.ToString())
                {

                    case "HOTEL":           facilities.Add(OCPIv2_1_1.Facilities.HOTEL);            break;
                    case "RESTAURANT":      facilities.Add(OCPIv2_1_1.Facilities.RESTAURANT);       break;
                    case "CAFE":            facilities.Add(OCPIv2_1_1.Facilities.CAFE);             break;
                    case "MALL":            facilities.Add(OCPIv2_1_1.Facilities.MALL);             break;
                    case "SUPERMARKET":     facilities.Add(OCPIv2_1_1.Facilities.SUPERMARKET);      break;
                    case "SPORT":           facilities.Add(OCPIv2_1_1.Facilities.SPORT);            break;
                    case "RECREATION_AREA": facilities.Add(OCPIv2_1_1.Facilities.RECREATION_AREA);  break;
                    case "NATURE":          facilities.Add(OCPIv2_1_1.Facilities.NATURE);           break;
                    case "MUSEUM":          facilities.Add(OCPIv2_1_1.Facilities.MUSEUM);           break;
                    case "BIKE_SHARING":    facilities.Add(OCPIv2_1_1.Facilities.BIKE_SHARING);     break;
                    case "BUS_STOP":        facilities.Add(OCPIv2_1_1.Facilities.BUS_STOP);         break;
                    case "TAXI_STAND":      facilities.Add(OCPIv2_1_1.Facilities.TAXI_STAND);       break;
                    case "TRAM_STOP":       facilities.Add(OCPIv2_1_1.Facilities.TRAM_STOP);        break;
                    case "METRO_STATION":   facilities.Add(OCPIv2_1_1.Facilities.METRO_STATION);    break;
                    case "TRAIN_STATION":   facilities.Add(OCPIv2_1_1.Facilities.TRAIN_STATION);    break;
                    case "AIRPORT":         facilities.Add(OCPIv2_1_1.Facilities.AIRPORT);          break;
                    case "PARKING_LOT":     facilities.Add(OCPIv2_1_1.Facilities.PARKING_LOT);      break;
                    case "CARPOOL_PARKING": facilities.Add(OCPIv2_1_1.Facilities.CARPOOL_PARKING);  break;
                    case "FUEL_STATION":    facilities.Add(OCPIv2_1_1.Facilities.FUEL_STATION);     break;
                    case "WIFI":            facilities.Add(OCPIv2_1_1.Facilities.WIFI);             break;

                }

            }

            return facilities;

        }

        #endregion


        #region ToOCPI(this ChargingPool,  ref Warnings, IncludeEVSEIds = null)

        public static Location? ToOCPI(this WWCP.IChargingPool                   ChargingPool,
                                       WWCPEVSEId_2_EVSEUId_Delegate?            CustomEVSEUIdConverter,
                                       WWCPEVSEId_2_EVSEId_Delegate?             CustomEVSEIdConverter,
                                       ref List<Warning>                         Warnings,
                                       WWCP.IncludeEVSEIdDelegate?               IncludeEVSEIds                = null,
                                       WWCP.IncludeChargingConnectorIdDelegate?  IncludeChargingConnectorIds   = null)
        {

            var location = ChargingPool.ToOCPI(CustomEVSEUIdConverter,
                                               CustomEVSEIdConverter,
                                               out var warnings,
                                               IncludeEVSEIds);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return location;

        }

        #endregion

        #region ToOCPI(this ChargingPool,  out Warnings, IncludeEVSEIds = null)

        public static Location? ToOCPI(this WWCP.IChargingPool                   ChargingPool,
                                       WWCPEVSEId_2_EVSEUId_Delegate?            CustomEVSEUIdConverter,
                                       WWCPEVSEId_2_EVSEId_Delegate?             CustomEVSEIdConverter,
                                       out IEnumerable<Warning>                  Warnings,
                                       WWCP.IncludeEVSEIdDelegate?               IncludeEVSEIds                = null,
                                       WWCP.IncludeChargingConnectorIdDelegate?  IncludeChargingConnectorIds   = null)
        {

            var includeEVSEIds  = IncludeEVSEIds ?? (evseId => true);
            var warnings        = new List<Warning>();

            if (ChargingPool.Operator is null)
            {
                warnings.Add(Warning.Create(Languages.en, "The given charging location must have a valid charging station operator!"));
                Warnings = warnings;
                return null;
            }

            var countryCode  = OCPI.CountryCode.TryParse(ChargingPool.Operator.Id.CountryCode.Alpha2Code);

            if (!countryCode.HasValue)
            {
                warnings.Add(Warning.Create(Languages.en, $"The given charging station operator identificaton '{ChargingPool.Id.OperatorId}' could not be converted to an OCPI country code!"));
                Warnings = warnings;
                return null;
            }

            var partyId      = OCPI.Party_Id.   TryParse(ChargingPool.Operator.Id.Suffix);

            if (!partyId.HasValue)
            {
                warnings.Add(Warning.Create(Languages.en, $"The given charging station operator identificaton '{ChargingPool.Id.OperatorId}' could not be converted to an OCPI party identification!"));
                Warnings = warnings;
                return null;
            }

            var locationId   = Location_Id.TryParse(ChargingPool.Id.ToString());

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

                    var ocpiEVSE = evse.ToOCPI(CustomEVSEUIdConverter,
                                               CustomEVSEIdConverter,
                                               IncludeChargingConnectorIds,
                                               evse.Status.Timestamp > evse.LastChange
                                                   ? evse.Status.Timestamp
                                                   : evse.LastChange,
                                               ref warnings);

                    if (ocpiEVSE is not null)
                        evses.Add(ocpiEVSE);
                    else
                    {

                    }

                }

                return new Location(

                           CountryCode:          countryCode.Value,
                           PartyId:              partyId    .Value,
                           Id:                   locationId .Value,
                           LocationType:         LocationType.ON_STREET, // ????
                           Address:              ChargingPool.Address.HouseNumber is not null
                                                     ? $"{ChargingPool.Address.Street} {ChargingPool.Address.HouseNumber}"
                                                     :    ChargingPool.Address.Street,
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
                                                     ChargingPool.Operator.Homepage,
                                                     ChargingPool.Operator.Logo.HasValue
                                                         ? new Image(
                                                               ChargingPool.Operator.Logo.Value,
                                                               ChargingPool.Operator.Logo.Value.Path.ToString().Substring(ChargingPool.Operator.Logo.Value.Path.LastIndexOf(".")+1).ToString().ToLower() switch {
                                                                   "gif"  => ImageFileType.gif,
                                                                   "png"  => ImageFileType.png,
                                                                   "svg"  => ImageFileType.svg,
                                                                   "jpeg" => ImageFileType.jpeg,
                                                                   "webp" => ImageFileType.webp,
                                                                   _      => ImageFileType.jpg
                                                               },
                                                               ImageCategory.OPERATOR
                                                           )
                                                         : null
                                                 ),
                           SubOperator:          null,
                           Owner:                null,
                           Facilities:           ChargingPool.Facilities.       ToOCPI(),
                           Timezone:             ChargingPool.Address.TimeZone?.ToString(),
                           OpeningTimes:         ChargingPool.OpeningTimes.     ToOCPI(),
                           ChargingWhenClosed:   ChargingPool.ChargingWhenClosed,
                           Images:               Array.Empty<Image>(),
                           EnergyMix:            null,

                           LastUpdated:          ChargingPool.LastChange

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create(Languages.en, $"Could not convert the given charging pool '{ChargingPool.Id}' to OCPI: {ex.Message}"));
                Warnings = warnings;
            }

            return null;

        }

        #endregion

        #region ToOCPI(this ChargingPools, ref Warnings, IncludeEVSEIds = null)

        public static IEnumerable<Location> ToOCPI(this IEnumerable<WWCP.ChargingPool>  ChargingPools,
                                                   WWCPEVSEId_2_EVSEUId_Delegate?       CustomEVSEUIdConverter,
                                                   WWCPEVSEId_2_EVSEId_Delegate?        CustomEVSEIdConverter,
                                                   ref List<Warning>                    Warnings,
                                                   WWCP.IncludeEVSEIdDelegate?          IncludeEVSEIds = null)
        {

            var locations = ChargingPools.ToOCPI(CustomEVSEUIdConverter,
                                                 CustomEVSEIdConverter,
                                                 out var warnings,
                                                 IncludeEVSEIds);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return locations;

        }

        #endregion

        #region ToOCPI(this ChargingPools, out Warnings, IncludeEVSEIds = null)

        public static IEnumerable<Location> ToOCPI(this IEnumerable<WWCP.ChargingPool>  ChargingPools,
                                                   WWCPEVSEId_2_EVSEUId_Delegate?       CustomEVSEUIdConverter,
                                                   WWCPEVSEId_2_EVSEId_Delegate?        CustomEVSEIdConverter,
                                                   out IEnumerable<Warning>             Warnings,
                                                   WWCP.IncludeEVSEIdDelegate?          IncludeEVSEIds = null)
        {

            var warnings   = new List<Warning>();
            var locations  = new HashSet<Location>();

            foreach (var chargingPool in ChargingPools)
            {

                try
                {

                    var chargingPool2 = chargingPool.ToOCPI(CustomEVSEUIdConverter,
                                                            CustomEVSEIdConverter,
                                                            out var warning,
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



        #region ToOCPI(this OpenSourceLicense)

        public static OpenSourceLicense ToOCPI(this org.GraphDefined.Vanaheimr.Hermod.OpenSourceLicense OpenSourceLicense)

            => new (OpenSourceLicense_Id.Parse(OpenSourceLicense.Id.ToString()),
                    OpenSourceLicense.URLs.ToArray());

        #endregion

        #region ToOCPI(this LegalStatus)

        public static LegalStatus ToOCPI(this WWCP.LegalStatus LegalStatus)

            => OCPI.LegalStatus.Parse(LegalStatus.ToString());

        #endregion

        #region ToOCPI(this TransparencySoftware)

        public static TransparencySoftware ToOCPI(this WWCP.TransparencySoftware TransparencySoftware)

            => new (TransparencySoftware.Name,
                    TransparencySoftware.Version,
                    TransparencySoftware.OpenSourceLicense.ToOCPI(),
                    TransparencySoftware.Vendor,
                    TransparencySoftware.Logo,
                    TransparencySoftware.HowToUse,
                    TransparencySoftware.MoreInformation,
                    TransparencySoftware.SourceCodeRepository);

        #endregion

        #region ToOCPI(this TransparencySoftwareStatus)

        public static TransparencySoftwareStatus ToOCPI(this WWCP.TransparencySoftwareStatus TransparencySoftwareStatus)

            => new (TransparencySoftwareStatus.TransparencySoftware.ToOCPI(),
                    TransparencySoftwareStatus.LegalStatus.         ToOCPI(),
                    TransparencySoftwareStatus.Certificate,
                    TransparencySoftwareStatus.CertificateIssuer,
                    TransparencySoftwareStatus.NotBefore,
                    TransparencySoftwareStatus.NotAfter);

        #endregion

        #region ToOCPI(this EnergyMeter)

        public static EnergyMeter ToOCPI(this WWCP.EnergyMeter EnergyMeter)

            => new (OCPI.Meter_Id.Parse(EnergyMeter.Id.ToString()),
                    EnergyMeter.Model,
                    EnergyMeter.ModelURL,
                    EnergyMeter.HardwareVersion,
                    EnergyMeter.FirmwareVersion,
                    EnergyMeter.Manufacturer,
                    EnergyMeter.ManufacturerURL,
                    EnergyMeter.PublicKeys.               Select(publicKey                  => PublicKey.Parse(publicKey. ToString())),
                    EnergyMeter.PublicKeyCertificateChain.HasValue ? CertificateChain.Parse(EnergyMeter.PublicKeyCertificateChain.Value.ToString()) : null,
                    EnergyMeter.TransparencySoftwares.    Select(transparencySoftwareStatus => transparencySoftwareStatus.ToOCPI()),
                    EnergyMeter.Description.ToOCPI(),
                    EnergyMeter.CustomData,
                    EnergyMeter.InternalData,
                    EnergyMeter.LastUpdate);

        #endregion


        #region ToOCPI(this EVSE,  ref Warnings)

        public static EVSE? ToOCPI(this WWCP.IEVSE                           EVSE,
                                   WWCPEVSEId_2_EVSEUId_Delegate?            CustomEVSEUIdConverter,
                                   WWCPEVSEId_2_EVSEId_Delegate?             CustomEVSEIdConverter,
                                   WWCP.IncludeChargingConnectorIdDelegate?  IncludeChargingConnectorIds,
                                   DateTime?                                 LastUpdate,
                                   ref List<Warning>                         Warnings)
        {

            var result = EVSE.ToOCPI(CustomEVSEUIdConverter,
                                     CustomEVSEIdConverter,
                                     IncludeChargingConnectorIds,
                                     LastUpdate,
                                     out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return result;

        }

        #endregion

        #region ToOCPI(this EVSE,  out Warnings)

        public static EVSE? ToOCPI(this WWCP.IEVSE                           EVSE,
                                   WWCPEVSEId_2_EVSEUId_Delegate?            CustomEVSEUIdConverter,
                                   WWCPEVSEId_2_EVSEId_Delegate?             CustomEVSEIdConverter,
                                   WWCP.IncludeChargingConnectorIdDelegate?  IncludeChargingConnectorIds,
                                   DateTime?                                 LastUpdate,
                                   out IEnumerable<Warning>                  Warnings)
        {

            var warnings = new List<Warning>();

            try
            {

                var evseUId = EVSE.Id.ToOCPI_EVSEUId(CustomEVSEUIdConverter);

                if (!evseUId.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE identificaton '{EVSE.Id}' could not be converted to an OCPI EVSE Unique identification!"));
                    Warnings = warnings;
                    return null;
                }


                var evseId  = EVSE.Id.ToOCPI_EVSEId(CustomEVSEIdConverter);

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

                var connectors = EVSE.ChargingConnectors.
                                      Where  (connector         => IncludeChargingConnectorIds?.Invoke(connector.Id) ?? true).
                                      Select (chargingConnector => chargingConnector.ToOCPI(EVSE, ref warnings)).
                                      Where  (connector         => connector is not null).
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
                           Capabilities:          EVSE.ChargingStation.ToOCPI_Capabilities(),
                           EnergyMeter:           EVSE.EnergyMeter?.ToOCPI(),
                           FloorLevel:            EVSE.ChargingStation.Address?.FloorLevel ?? EVSE.ChargingPool.Address?.FloorLevel,
                           Coordinates:           EVSE.ChargingStation.GeoLocation         ?? EVSE.ChargingPool.GeoLocation,
                           PhysicalReference:     EVSE.PhysicalReference                   ?? EVSE.ChargingStation.PhysicalReference,
                           Directions:            EVSE.ChargingStation.ArrivalInstructions.ToOCPI(),
                           ParkingRestrictions:   Array.Empty<ParkingRestrictions>(),
                           Images:                Array.Empty<Image>(),

                           LastUpdated:           LastUpdate ?? EVSE.LastChange

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create(Languages.en, $"Could not convert the given EVSE '{EVSE.Id}' to OCPI: {ex.Message}"));
                Warnings = warnings;
            }

            return null;

        }

        #endregion

        #region ToOCPI(this EVSEs, ref Warnings)

        public static IEnumerable<EVSE> ToOCPI(this IEnumerable<WWCP.IEVSE>              EVSEs,
                                               WWCPEVSEId_2_EVSEUId_Delegate?            CustomEVSEUIdConverter,
                                               WWCPEVSEId_2_EVSEId_Delegate?             CustomEVSEIdConverter,
                                               WWCP.IncludeChargingConnectorIdDelegate?  IncludeChargingConnectorIds,
                                               ref List<Warning>                         Warnings)
        {

            var evses = EVSEs.ToOCPI(CustomEVSEUIdConverter,
                                     CustomEVSEIdConverter,
                                     IncludeChargingConnectorIds,
                                     out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return evses;

        }

        #endregion

        #region ToOCPI(this EVSEs, out Warnings)

        public static IEnumerable<EVSE> ToOCPI(this IEnumerable<WWCP.IEVSE>              EVSEs,
                                               WWCPEVSEId_2_EVSEUId_Delegate?            CustomEVSEUIdConverter,
                                               WWCPEVSEId_2_EVSEId_Delegate?             CustomEVSEIdConverter,
                                               WWCP.IncludeChargingConnectorIdDelegate?  IncludeChargingConnectorIds,
                                               out IEnumerable<Warning>                  Warnings)
        {

            var warnings  = new List<Warning>();
            var evses     = new HashSet<EVSE>();

            foreach (var evse in EVSEs)
            {

                try
                {

                    var evse2 = evse.ToOCPI(CustomEVSEUIdConverter,
                                            CustomEVSEIdConverter,
                                            IncludeChargingConnectorIds,
                                            evse.Status.Timestamp > evse.LastChange
                                                ? evse.Status.Timestamp
                                                : evse.LastChange,
                                            out var warning);

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



        #region ToOCPI(this ChargingConnectorId)

        public static Connector_Id? ToOCPI(this WWCP.ChargingConnector_Id ChargingConnectorId)

            => Connector_Id.TryParse(ChargingConnectorId.ToString());

        public static Connector_Id? ToOCPI(this WWCP.ChargingConnector_Id? ChargingConnectorId)

            => ChargingConnectorId.HasValue
                   ? Connector_Id.TryParse(ChargingConnectorId.Value.ToString())
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

        public static ConnectorType? ToOCPI(this WWCP.ChargingPlugTypes PlugType)

            => PlugType switch {
                   //WWCP.PlugTypes.SmallPaddleInductive          => 
                   //WWCP.PlugTypes.LargePaddleInductive          => 
                   //WWCP.PlugTypes.AVCONConnector                => 
                   //WWCP.PlugTypes.TeslaConnector                => 
                   WWCP.ChargingPlugTypes.TESLA_Roadster                => ConnectorType.TESLA_R,
                   WWCP.ChargingPlugTypes.TESLA_ModelS                  => ConnectorType.TESLA_S,
                   //WWCP.PlugTypes.NEMA5_20                      => 
                   WWCP.ChargingPlugTypes.TypeEFrenchStandard           => ConnectorType.DOMESTIC_E,
                   WWCP.ChargingPlugTypes.TypeFSchuko                   => ConnectorType.DOMESTIC_F,
                   WWCP.ChargingPlugTypes.TypeGBritishStandard          => ConnectorType.DOMESTIC_G,
                   WWCP.ChargingPlugTypes.TypeJSwissStandard            => ConnectorType.DOMESTIC_J,
                   WWCP.ChargingPlugTypes.Type1Connector_CableAttached  => ConnectorType.IEC_62196_T1,
                   WWCP.ChargingPlugTypes.Type2Outlet                   => ConnectorType.IEC_62196_T2,
                   WWCP.ChargingPlugTypes.Type2Connector_CableAttached  => ConnectorType.IEC_62196_T2,
                   WWCP.ChargingPlugTypes.Type3Outlet                   => ConnectorType.IEC_62196_T3A,
                   WWCP.ChargingPlugTypes.IEC60309SinglePhase           => ConnectorType.IEC_60309_2_single_16,
                   WWCP.ChargingPlugTypes.IEC60309ThreePhase            => ConnectorType.IEC_60309_2_three_16,
                   WWCP.ChargingPlugTypes.CCSCombo1Plug_CableAttached   => ConnectorType.IEC_62196_T1_COMBO,
                   WWCP.ChargingPlugTypes.CCSCombo2Plug_CableAttached   => ConnectorType.IEC_62196_T2_COMBO,
                   WWCP.ChargingPlugTypes.CHAdeMO                       => ConnectorType.CHADEMO,
                   //WWCP.PlugTypes.CEE3                          => 
                   //WWCP.PlugTypes.CEE5                          => 
                   _                                            => null
               };

        public static ConnectorType? ToOCPI(this WWCP.ChargingPlugTypes? PlugType)

            => PlugType.HasValue
                   ? PlugType.Value.ToOCPI()
                   : null;

        #endregion


        #region ToOCPI(this ChargingConnector, EVSE, ref Warnings)

        public static Connector? ToOCPI(this WWCP.IChargingConnector  ChargingConnector,
                                        WWCP.IEVSE                    EVSE,
                                        ref List<Warning>             Warnings)
        {

            var result = ChargingConnector.ToOCPI(EVSE,
                                                  out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return result;

        }

        #endregion

        #region ToOCPI(this ChargingConnector, EVSE, out Warnings)

        public static Connector? ToOCPI(this WWCP.IChargingConnector  ChargingConnector,
                                        WWCP.IEVSE                    EVSE,
                                        out IEnumerable<Warning>      Warnings)
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

                var connectorId  = Connector_Id.Parse(ChargingConnector.Id.ToString());

                var powerType    = EVSE.CurrentType.ToOCPI();

                if (!powerType.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE current type '{EVSE.CurrentType}' could not be converted to an OCPI power type!"));
                    Warnings = warnings;
                    return null;
                }

                var standard     = ChargingConnector.Plug.ToOCPI();

                if (!standard.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given socket outlet plug '{ChargingConnector.Plug}' could not be converted to an OCPI connector standard!"));
                    Warnings = warnings;
                    return null;
                }


                Warnings = Array.Empty<Warning>();

                return new Connector(

                           Id:                      connectorId,
                           Standard:                standard.Value,
                           Format:                  ChargingConnector.CableAttached switch {
                                                        true  => ConnectorFormats.CABLE,
                                                        _     => ConnectorFormats.SOCKET
                                                    },
                           PowerType:               powerType.Value,
                           Voltage:                 (UInt16) (EVSE.AverageVoltage.HasValue && EVSE.AverageVoltage.Value != 0
                                                        ? ((Double) EVSE.AverageVoltage.Value / Math.Sqrt(3))   // 400 V between two conductors => 230 V between conductor and neutral (OCPI design flaw!)
                                                        : powerType.Value switch {
                                                              PowerTypes.AC_1_PHASE  => 230,
                                                              PowerTypes.AC_3_PHASE  => 230,                    // Line to neutral for AC_3_PHASE: https://github.com/ocpi/ocpi/blob/master/mod_locations.asciidoc#mod_locations_connector_object
                                                              PowerTypes.DC          => 400,
                                                              _                      => 0
                                                          }),
                           Amperage:                (UInt16) (EVSE.MaxCurrent.HasValue     && EVSE.MaxCurrent.Value != 0
                                                        ? EVSE.MaxCurrent.Value
                                                        : powerType.Value switch {
                                                              PowerTypes.AC_1_PHASE  => 16,
                                                              PowerTypes.AC_3_PHASE  => 16,
                                                              PowerTypes.DC          => 50,
                                                              _                      => 0
                                                          }),

                           //TariffId:              Via lookup table!
                           TermsAndConditionsURL:   EVSE.Operator?.TermsAndConditionsURL,

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

        public static AuthMethods? ToOCPI(this WWCP.AuthMethod AuthMethod)
        {

            if (AuthMethod == WWCP.AuthMethod.AUTH_REQUEST)
                return AuthMethods.AUTH_REQUEST;

            if (AuthMethod == WWCP.AuthMethod.RESERVE)
                return AuthMethods.AUTH_REQUEST;

            if (AuthMethod == WWCP.AuthMethod.REMOTESTART)
                return AuthMethods.AUTH_REQUEST;

            if (AuthMethod == WWCP.AuthMethod.WHITELIST)
                return AuthMethods.WHITELIST;

            return null;

        }

        public static AuthMethods? ToOCPI(this WWCP.AuthMethod? AuthMethod)

            => AuthMethod.HasValue
                   ? AuthMethod.Value.ToOCPI()
                   : null;

        #endregion

        #region ToWWCP(this AuthMethod)

        public static WWCP.AuthMethod? ToWWCP(this AuthMethods AuthMethod)
        {

            if (AuthMethod == AuthMethods.AUTH_REQUEST)
                return WWCP.AuthMethod.AUTH_REQUEST;

            if (AuthMethod == AuthMethods.WHITELIST)
                return WWCP.AuthMethod.WHITELIST;

            return null;

        }

        public static WWCP.AuthMethod? ToWWCP(this AuthMethods? AuthMethod)

            => AuthMethod.HasValue
                   ? AuthMethod.Value.ToWWCP()
                   : null;

        #endregion


        #region ToOCPI(this EnergyMeterId)

        public static OCPI.Meter_Id? ToOCPI(this WWCP.EnergyMeter_Id EnergyMeterId)

            => OCPI.Meter_Id.Parse(EnergyMeterId.ToString());

        public static OCPI.Meter_Id? ToOCPI(this WWCP.EnergyMeter_Id? EnergyMeterId)

            => EnergyMeterId.HasValue
                   ? EnergyMeterId.Value.ToOCPI()
                   : null;

        #endregion

        #region ToWWCP(this MeterId)

        public static WWCP.EnergyMeter_Id? ToWWCP(this Meter_Id MeterId)

            => WWCP.EnergyMeter_Id.Parse(MeterId.ToString());

        public static WWCP.EnergyMeter_Id? ToWWCP(this Meter_Id? MeterId)

            => MeterId.HasValue
                   ? MeterId.Value.ToWWCP()
                   : null;

        #endregion


        #region ToOCPI(this Time)

        public static Time? ToOCPI(this org.GraphDefined.Vanaheimr.Illias.Time Time)

            => OCPIv2_1_1.Time.FromHourMinSec(Time.Hour,
                                              Time.Minute,
                                              Time.Second ?? 0);

        #endregion


        #region ToOCPI(this ChargingPriceComponent,     ref Warnings)

        public static PriceComponent? ToOCPI(this WWCP.ChargingPriceComponent  ChargingPriceComponent,
                                             ref List<Warning>                 Warnings)
        {

            var result = ChargingPriceComponent.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return result;

        }

        #endregion

        #region ToOCPI(this ChargingPriceComponent,     out Warnings)

        public static PriceComponent? ToOCPI(this WWCP.ChargingPriceComponent  ChargingPriceComponent,
                                             out IEnumerable<Warning>          Warnings)
        {

            var warnings = new List<Warning>();

            try
            {

                Warnings = Array.Empty<Warning>();

                return new PriceComponent(

                           Type:       TariffDimension.Parse(ChargingPriceComponent.Type.ToString()),
                           Price:      ChargingPriceComponent.Price,
                           StepSize:   ChargingPriceComponent.StepSize

                       );;

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create(Languages.en, $"Could not convert the given charging price component to OCPI: {ex.Message}"));
                Warnings = warnings;
            }

            return null;

        }

        #endregion

        #region ToOCPI(this ChargingPriceComponents,    ref Warnings)

        public static IEnumerable<PriceComponent> ToOCPI(this IEnumerable<WWCP.ChargingPriceComponent>  ChargingPriceComponents,
                                                         ref List<Warning>                              Warnings)
        {

            var priceComponents = ChargingPriceComponents.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return priceComponents;

        }

        #endregion

        #region ToOCPI(this ChargingPriceComponents,    out Warnings)

        public static IEnumerable<PriceComponent> ToOCPI(this IEnumerable<WWCP.ChargingPriceComponent>  ChargingPriceComponents,
                                                             out IEnumerable<Warning>                   Warnings)
        {

            var warnings         = new List<Warning>();
            var priceComponents  = new HashSet<PriceComponent>();

            foreach (var chargingPriceComponent in ChargingPriceComponents)
            {

                try
                {

                    var priceComponent2 = chargingPriceComponent.ToOCPI(out var warning);

                    if (priceComponent2.HasValue)
                        priceComponents.Add(priceComponent2.Value);

                    if (warning is not null && warning.Any())
                        warnings.AddRange(warning);

                }
                catch (Exception ex)
                {
                    warnings.Add(Warning.Create(Languages.en, "Could not convert the given charging price component to OCPI: " + ex.Message));
                }

            }

            Warnings = warnings.ToArray();
            return priceComponents;

        }

        #endregion


        #region ToOCPI(this ChargingTariffRestriction,  ref Warnings)

        public static TariffRestrictions? ToOCPI(this WWCP.ChargingTariffRestriction  ChargingTariffRestriction,
                                                 ref List<Warning>                    Warnings)
        {

            var tariffRestrictions = ChargingTariffRestriction.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return tariffRestrictions;

        }

        #endregion

        #region ToOCPI(this ChargingTariffRestriction,  out Warnings)

        public static TariffRestrictions? ToOCPI(this WWCP.ChargingTariffRestriction  ChargingTariffRestriction,
                                                 out IEnumerable<Warning>             Warnings)
        {

            var warnings = new List<Warning>();

            try
            {

                Warnings = Array.Empty<Warning>();

                return new TariffRestrictions(

                           StartTime:     ChargingTariffRestriction.Time?.    StartTime?.ToOCPI(),
                           EndTime:       ChargingTariffRestriction.Time?.    EndTime?.  ToOCPI(),
                           StartDate:     ChargingTariffRestriction.Date?.    StartTime,
                           EndDate:       ChargingTariffRestriction.Date?.    EndTime,
                           MinkWh:        ChargingTariffRestriction.kWh?.     Min,
                           MaxkWh:        ChargingTariffRestriction.kWh?.     Max,
                           MinPower:      ChargingTariffRestriction.Power?.   Min,
                           MaxPower:      ChargingTariffRestriction.Power?.   Max,
                           MinDuration:   ChargingTariffRestriction.Duration?.Min,
                           MaxDuration:   ChargingTariffRestriction.Duration?.Max,
                           DayOfWeek:     ChargingTariffRestriction.DayOfWeek

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create(Languages.en, $"Could not convert the given charging tariff restrictions to OCPI: {ex.Message}"));
                Warnings = warnings;
            }

            return null;

        }

        #endregion

        #region ToOCPI(this ChargingTariffRestrictions, ref Warnings)

        public static IEnumerable<TariffRestrictions> ToOCPI(this IEnumerable<WWCP.ChargingTariffRestriction>  ChargingTariffRestrictions,
                                                             ref List<Warning>                                 Warnings)
        {

            var tariffRestrictions = ChargingTariffRestrictions.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return tariffRestrictions;

        }

        #endregion

        #region ToOCPI(this ChargingTariffRestrictions, out Warnings)

        public static IEnumerable<TariffRestrictions> ToOCPI(this IEnumerable<WWCP.ChargingTariffRestriction>  ChargingTariffRestrictions,
                                                             out IEnumerable<Warning>                          Warnings)
        {

            var warnings            = new List<Warning>();
            var tariffRestrictions  = new HashSet<TariffRestrictions>();

            foreach (var chargingTariffRestriction in ChargingTariffRestrictions)
            {

                try
                {

                    var tariffRestrictions2 = chargingTariffRestriction.ToOCPI(out var warning);

                    if (tariffRestrictions2 is not null)
                        tariffRestrictions.Add(tariffRestrictions2);

                    if (warning is not null && warning.Any())
                        warnings.AddRange(warning);

                }
                catch (Exception ex)
                {
                    warnings.Add(Warning.Create(Languages.en, "Could not convert the given charging tariff restriction to OCPI: " + ex.Message));
                }

            }

            Warnings = warnings.ToArray();
            return tariffRestrictions;

        }

        #endregion


        #region ToOCPI(this ChargingTariffElement,      ref Warnings)

        public static TariffElement? ToOCPI(this WWCP.ChargingTariffElement  ChargingTariffElement,
                                            ref List<Warning>                Warnings)
        {

            var tariffElements = ChargingTariffElement.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return tariffElements;

        }

        #endregion

        #region ToOCPI(this ChargingTariffElement,      out Warnings)

        public static TariffElement? ToOCPI(this WWCP.ChargingTariffElement  ChargingTariffElement,
                                            out IEnumerable<Warning>         Warnings)
        {

            var warnings = new List<Warning>();

            try
            {

                var priceComponents = ChargingTariffElement.ChargingPriceComponents.
                                          Select (chargingPriceComponent => chargingPriceComponent.ToOCPI(ref warnings)).
                                          Where  (priceComponent         => priceComponent is not null).
                                          Cast<PriceComponent>().
                                          ToArray();

                if (!priceComponents.Any())
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given charging price components could not be converted to OCPI price components!"));
                    Warnings = warnings;
                    return null;
                }


                var tariffRestrictions = ChargingTariffElement.ChargingTariffRestrictions.
                                             Select (chargingTariffRestriction => chargingTariffRestriction.ToOCPI(ref warnings)).
                                             Where  (tariffRestriction         => tariffRestriction is not null).
                                             Cast<TariffRestrictions>().
                                             ToArray();

                if (!tariffRestrictions.Any())
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given charging tariff restrictions could not be converted to OCPI tariff restrictions!"));
                    Warnings = warnings;
                    return null;
                }


                Warnings = Array.Empty<Warning>();

                return new TariffElement(
                           PriceComponents:     priceComponents,
                           TariffRestrictions:  tariffRestrictions
                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create(Languages.en, $"Could not convert the given charging tariff element to OCPI: {ex.Message}"));
                Warnings = warnings;
            }

            return null;

        }

        #endregion

        #region ToOCPI(this ChargingTariffElements,     ref Warnings)

        public static IEnumerable<TariffElement> ToOCPI(this IEnumerable<WWCP.ChargingTariffElement>  ChargingTariffElements,
                                                        ref List<Warning>                             Warnings)
        {

            var tariffElements = ChargingTariffElements.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return tariffElements;

        }

        #endregion

        #region ToOCPI(this ChargingTariffElements,     out Warnings)

        public static IEnumerable<TariffElement> ToOCPI(this IEnumerable<WWCP.ChargingTariffElement>  ChargingTariffElements,
                                                        out IEnumerable<Warning>                      Warnings)
        {

            var warnings        = new List<Warning>();
            var tariffElements  = new HashSet<TariffElement>();

            foreach (var chargingTariff in ChargingTariffElements)
            {

                try
                {

                    var tariffElement2 = chargingTariff.ToOCPI(out var warning);

                    if (tariffElement2.HasValue)
                        tariffElements.Add(tariffElement2.Value);

                    if (warning is not null && warning.Any())
                        warnings.AddRange(warning);

                }
                catch (Exception ex)
                {
                    warnings.Add(Warning.Create(Languages.en, "Could not convert the given charging tariff element to OCPI: " + ex.Message));
                }

            }

            Warnings = warnings.ToArray();
            return tariffElements;

        }

        #endregion


        #region ToOCPI(this ChargingTariff,             ref Warnings)

        public static Tariff? ToOCPI(this WWCP.IChargingTariff  ChargingTariff,
                                     ref List<Warning>          Warnings)
        {

            var tariff = ChargingTariff.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return tariff;

        }

        #endregion

        #region ToOCPI(this ChargingTariff,             out Warnings)

        public static Tariff? ToOCPI(this WWCP.IChargingTariff  ChargingTariff,
                                     out IEnumerable<Warning>   Warnings)
        {

            var warnings = new List<Warning>();

            try
            {

                var tariffElements = ChargingTariff.TariffElements.
                                         Select (chargingTariffElement => chargingTariffElement.ToOCPI(ref warnings)).
                                         Where  (tariffElement         => tariffElement is not null).
                                         Cast<TariffElement>().
                                         ToArray();

                if (!tariffElements.Any())
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given charging tariff elements could not be converted to OCPI tariff elements!"));
                    Warnings = warnings;
                    return null;
                }


                Warnings = Array.Empty<Warning>();

                return new Tariff(

                           CountryCode:      OCPI.CountryCode.Parse(ChargingTariff.Id.OperatorId.CountryCode.Alpha2Code),
                           PartyId:          OCPI.Party_Id.   Parse(ChargingTariff.Id.OperatorId.Suffix),
                           Id:               OCPI.Tariff_Id.  Parse(ChargingTariff.Id.ToString()),
                           Currency:         OCPI.Currency.   Parse(ChargingTariff.Currency.ISOCode),
                           TariffElements:   tariffElements,

                           TariffAltText:    ChargingTariff.Description.ToOCPI(),
                           TariffAltURL:     ChargingTariff.TariffURL,
                           EnergyMix:        null, //ChargingTariff.EnergyMix.ToOCPI(),

                           LastUpdated:      ChargingTariff.LastChange

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create(Languages.en, $"Could not convert the given charging tariff '{ChargingTariff.Id}' to OCPI: {ex.Message}"));
                Warnings = warnings;
            }

            return null;

        }

        #endregion

        #region ToOCPI(this ChargingTariffs,            ref Warnings)

        public static IEnumerable<Tariff> ToOCPI(this IEnumerable<WWCP.IChargingTariff>  ChargingTariffs,
                                                 ref List<Warning>                       Warnings)
        {

            var tariffs = ChargingTariffs.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return tariffs;

        }

        #endregion

        #region ToOCPI(this ChargingTariffs,            out Warnings)

        public static IEnumerable<Tariff> ToOCPI(this IEnumerable<WWCP.IChargingTariff>  ChargingTariffs,
                                                 out IEnumerable<Warning>                Warnings)
        {

            var warnings  = new List<Warning>();
            var tariffs   = new HashSet<Tariff>();

            foreach (var chargingTariff in ChargingTariffs)
            {

                try
                {

                    var tariff2 = chargingTariff.ToOCPI(out var warning);

                    if (tariff2 is not null)
                        tariffs.Add(tariff2);

                    if (warning is not null && warning.Any())
                        warnings.AddRange(warning);

                }
                catch (Exception ex)
                {
                    warnings.Add(Warning.Create(Languages.en, $"Could not convert the given charging tariff '{chargingTariff.Id}' to OCPI: " + ex.Message));
                }

            }

            Warnings = warnings.ToArray();
            return tariffs;

        }

        #endregion



        #region ToOCPI(this ChargeDetailRecord, out Warnings)

        public static CDR? ToOCPI(this WWCP.ChargeDetailRecord    ChargeDetailRecord,
                                  WWCPEVSEId_2_EVSEUId_Delegate?  CustomEVSEUIdConverter,
                                  WWCPEVSEId_2_EVSEId_Delegate?   CustomEVSEIdConverter,
                                  GetTariffIds2_Delegate?         GetTariffIdsDelegate,
                                  EMSP_Id?                        EMSPId,
                                  Func<Tariff_Id, Tariff?>        TariffGetter,
                                  out IEnumerable<Warning>        Warnings)
        {

            var warnings  = new List<Warning>();
            var cdr       = ChargeDetailRecord.ToOCPI(CustomEVSEUIdConverter,
                                                      CustomEVSEIdConverter,
                                                      GetTariffIdsDelegate,
                                                      EMSPId,
                                                      TariffGetter,
                                                      ref warnings);

            Warnings = warnings;

            return cdr;

        }

        #endregion

        #region ToOCPI(this ChargeDetailRecord, ref Warnings)

        public static CDR? ToOCPI(this WWCP.ChargeDetailRecord    ChargeDetailRecord,
                                  WWCPEVSEId_2_EVSEUId_Delegate?  CustomEVSEUIdConverter,
                                  WWCPEVSEId_2_EVSEId_Delegate?   CustomEVSEIdConverter,
                                  GetTariffIds2_Delegate?         GetTariffIdsDelegate,
                                  EMSP_Id?                        EMSPId,
                                  Func<Tariff_Id, Tariff?>        TariffGetter,
                                  ref List<Warning>               Warnings)
        {

            try
            {

                if (ChargeDetailRecord is null)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The given charge detail record must not be null!"));
                    return null;
                }

                if (ChargeDetailRecord.ChargingStationOperator is null)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The given charge detail record must have a valid charging station operator!"));
                    return null;
                }

                if (!ChargeDetailRecord.SessionTime.EndTime.HasValue)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The session endtime of the given charge detail record must not be null!"));
                    return null;
                }

                if (!ChargeDetailRecord.SessionTime.Duration.HasValue)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The session time duration of the given charge detail record must not be null!"));
                    return null;
                }

                Auth_Id? authId = null;

                if (ChargeDetailRecord.AuthenticationStart is not null)
                {

                    if      (ChargeDetailRecord.AuthenticationStart.AuthToken.                  HasValue)
                        authId = Auth_Id.Parse(ChargeDetailRecord.AuthenticationStart.AuthToken.                  Value.ToString());

                    else if (ChargeDetailRecord.AuthenticationStart.RemoteIdentification.       HasValue)
                        authId = Auth_Id.Parse(ChargeDetailRecord.AuthenticationStart.RemoteIdentification.       Value.ToString());

                    else if (ChargeDetailRecord.AuthenticationStart.PlugAndChargeIdentification.HasValue)
                        authId = Auth_Id.Parse(ChargeDetailRecord.AuthenticationStart.PlugAndChargeIdentification.Value.ToString());

                    else if (ChargeDetailRecord.AuthenticationStart.QRCodeIdentification.       HasValue)
                        authId = Auth_Id.Parse(ChargeDetailRecord.AuthenticationStart.QRCodeIdentification.       Value.ToString());

                    else if (ChargeDetailRecord.AuthenticationStart.PIN.                        HasValue)
                        authId = Auth_Id.Parse(ChargeDetailRecord.AuthenticationStart.PIN.                        Value.ToString());

                    else if (ChargeDetailRecord.AuthenticationStart.PublicKey.                  HasValue)
                        authId = Auth_Id.Parse(ChargeDetailRecord.AuthenticationStart.PublicKey.                  Value.ToString());

                }

                if (authId is null)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The authentication identification used for starting of the given charge detail record must not be null!"));
                    return null;
                }

                if (!ChargeDetailRecord.AuthMethodStart.HasValue)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The authentication (verification) method used for starting of the given charge detail record must not be null!"));
                    return null;
                }

                var authMethod = ChargeDetailRecord.AuthMethodStart.ToOCPI();

                if (!authMethod.HasValue)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The authentication (verification) method used for starting of the given charge detail record is invalid!"));
                    return null;
                }

                if (ChargeDetailRecord.EVSE is null)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The EVSE of the given charge detail record must not be null!"));
                    return null;
                }

                if (ChargeDetailRecord.ChargingStation is null)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The charging station of the given charge detail record must not be null!"));
                    return null;
                }

                if (ChargeDetailRecord.ChargingPool is null)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The charging pool of the given charge detail record must not be null!"));
                    return null;
                }

                var filteredLocation = ChargeDetailRecord.ChargingPool.ToOCPI(CustomEVSEUIdConverter,
                                                                              CustomEVSEIdConverter,
                                                                              ref Warnings,
                                                                              evseId => evseId == ChargeDetailRecord.EVSE.Id);

                if (filteredLocation is null)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The charging location of the given charge detail record could not be calculated!"));
                    return null;
                }

                var chargingPeriods = new List<ChargingPeriod>();

                foreach (var energyMeteringValue in ChargeDetailRecord.EnergyMeteringValues)
                {
                    chargingPeriods.Add(
                        new ChargingPeriod(
                            energyMeteringValue.Timestamp,
                            new[] {
                                new CDRDimension(
                                    CDRDimensionType.ENERGY,
                                    energyMeteringValue.Value
                                )
                            }
                        )
                    );
                }

                if (!ChargeDetailRecord.ChargingPrice.HasValue)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The charging price of the given charge detail record must not be null!"));
                    return null;
                }

                if (ChargeDetailRecord.ChargingPrice.Value.Currency is null)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The currency of the charging price of the given charge detail record must not be null!"));
                    return null;
                }

                if (!ChargeDetailRecord.ConsumedEnergy.HasValue)
                {
                    Warnings.Add(Warning.Create(Languages.en, "The consumed energy of the given charge detail record must not be null!"));
                    return null;
                }

                if (ChargeDetailRecord.EnergyMeteringValues.Count() < 2)
                {
                    Warnings.Add(Warning.Create(Languages.en, "At least two energy metering values are expected!"));
                    return null;
                }


                var tariffIds  = GetTariffIdsDelegate?.Invoke(CountryCode.Parse(ChargeDetailRecord.ChargingStationOperator.Id.CountryCode.Alpha2Code),
                                                              Party_Id.   Parse(ChargeDetailRecord.ChargingStationOperator.Id.Suffix),
                                                              filteredLocation.Id,
                                                              filteredLocation.EVSEs.First().UId,
                                                              filteredLocation.EVSEs.First().Connectors.First().Id,
                                                              EMSPId);

                //var tariffIds  = filteredLocation.EVSEs.First().
                //                                        Select(connector => connector.TariffId).
                //                                        Where (tariffId  => tariffId is not null).
                //                                        Cast<Tariff_Id>();

                var tariffs    = tariffIds?.Select (tariffId => TariffGetter?.Invoke(tariffId))?.
                                            Where  (tariff   => tariff is not null)?.
                                            Cast<Tariff>()?.
                                            ToArray()
                                 ?? Array.Empty<Tariff>();

                return new CDR(

                           CountryCode:             CountryCode.Parse(ChargeDetailRecord.ChargingStationOperator.Id.CountryCode.Alpha2Code),
                           PartyId:                 Party_Id.   Parse(ChargeDetailRecord.ChargingStationOperator.Id.Suffix),
                           Id:                      CDR_Id.     Parse(ChargeDetailRecord.Id.ToString()),
                           Start:                   ChargeDetailRecord.SessionTime.StartTime,
                           Stop:                    ChargeDetailRecord.SessionTime.EndTime.Value,
                           AuthId:                  authId.    Value,
                           AuthMethod:              authMethod.Value,
                           Location:                filteredLocation,   //ToDo: Might still have not required connectors!
                           Currency:                OCPI.Currency.   Parse(ChargeDetailRecord.ChargingPrice.Value.Currency.ISOCode),
                           ChargingPeriods:         chargingPeriods,
                           TotalCost:               ChargeDetailRecord.ChargingPrice.       Value.Total,
                           TotalEnergy:             ChargeDetailRecord.ConsumedEnergy.      Value,
                           TotalTime:               ChargeDetailRecord.SessionTime.Duration.Value,

                           MeterId:                 ChargeDetailRecord.EnergyMeterId.ToOCPI(),
                           EnergyMeter:             null,
                           TransparencySoftwares:   null,
                           Tariffs:                 tariffs,
                           SignedData:              null,
                           TotalParkingTime:        null,
                           Remark:                  null,

                           Created:                 ChargeDetailRecord.Created,
                           LastUpdated:             ChargeDetailRecord.LastChange

                       );

            }
            catch (Exception ex)
            {
                Warnings.Add(Warning.Create(Languages.en, "Could not convert the given charge detail record to OCPI: " + ex.Message));
            }

            return null;

        }

        #endregion


    }

}
