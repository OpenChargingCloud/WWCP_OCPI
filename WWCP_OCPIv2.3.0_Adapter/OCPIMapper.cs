/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// A delegate which allows you to modify the conversion from WWCP EVSEs to OCPI EVSEs.
    /// </summary>
    /// <param name="WWCPEVSE">A WWCP EVSE.</param>
    /// <param name="OCPIEVSE">An OCPI EVSE.</param>
    public delegate EVSE                     WWCPEVSE_2_EVSE_Delegate                   (WWCP.IEVSE               WWCPEVSE,
                                                                                         EVSE                     OCPIEVSE);

    /// <summary>
    /// A delegate which allows you to modify the conversion from OCPI EVSEs to WWCP EVSEs.
    /// </summary>
    /// <param name="WWCPEVSE">A WWCP EVSE.</param>
    public delegate WWCP.IEVSE               EVSE_2_WWCPEVSE_Delegate                   (EVSE                     EVSE);


    /// <summary>
    /// A delegate which allows you to modify the conversion from WWCP EVSE status updates to OCPI EVSE status types.
    /// </summary>
    /// <param name="WWCPEVSEStatusUpdate">A WWCP EVSE status update.</param>
    /// <param name="OCPIStatusType">An OICP status type.</param>
    public delegate StatusType               WWCPEVSEStatusUpdate_2_StatusType_Delegate (WWCP.EVSEStatusUpdate    WWCPEVSEStatusUpdate,
                                                                                         StatusType               OCPIStatusType);

    /// <summary>
    /// A delegate which allows you to modify the conversion from WWCP EVSE status updates to OCPI EVSE status types.
    /// </summary>
    /// <param name="OCPIStatusType">An OICP status type.</param>
    public delegate WWCP.EVSEStatusUpdate    StatusType_2_WWCPEVSEStatusUpdate_Delegate (StatusType               OCPIStatusType);


    /// <summary>
    /// A delegate which allows you to modify the conversion from WWCP charge detail records to OICP charge detail records.
    /// </summary>
    /// <param name="WWCPChargeDetailRecord">A WWCP charge detail record.</param>
    /// <param name="OCIPCDR">An OICP charge detail record.</param>
    public delegate CDR                      WWCPChargeDetailRecord_2_CDR_Delegate      (WWCP.ChargeDetailRecord  WWCPChargeDetailRecord,
                                                                                         CDR                      OCIPCDR);

    /// <summary>
    /// A delegate which allows you to modify the conversion from OCPI charge detail records to WWCP charge detail records.
    /// </summary>
    /// <param name="OCIPCDR">An OICP charge detail record.</param>
    public delegate WWCP.ChargeDetailRecord  CDR_2_WWCPChargeDetailRecord_Delegate      (CDR                      OCIPCDR);



    /// <summary>
    /// Helper methods to map OCPI data structures to
    /// WWCP data structures and vice versa.
    /// </summary>
    public static class OCPIMapper
    {

        public const String OCPI_EVSEUId = "OCPI.EVSEUId";


        #region AsWWCPEVSEStatus(this EVSEStatus)

        /// <summary>
        /// Convert the given OCPI EVSE/connector status into a corresponding WWCP EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">An OCPI EVSE/connector status.</param>
        public static WWCP.EVSEStatusType AsWWCPEVSEStatus(this StatusType EVSEStatus)
        {

            if (EVSEStatus == StatusType.AVAILABLE)
                return WWCP.EVSEStatusType.Available;

            if (EVSEStatus == StatusType.BLOCKED)
                return WWCP.EVSEStatusType.OutOfService;

            if (EVSEStatus == StatusType.CHARGING)
                return WWCP.EVSEStatusType.Charging;

            if (EVSEStatus == StatusType.INOPERATIVE)
                return WWCP.EVSEStatusType.OutOfService;

            if (EVSEStatus == StatusType.OUTOFORDER)
                return WWCP.EVSEStatusType.Error;

            //if (EVSEStatus == StatusType.PLANNED)
            //    return WWCP.EVSEStatusTypes.Planned;

            //if (EVSEStatus == StatusType.REMOVED)
            //    return WWCP.EVSEStatusTypes.Removed;

            if (EVSEStatus == StatusType.RESERVED)
                return WWCP.EVSEStatusType.Reserved;

            return WWCP.EVSEStatusType.Unspecified;

        }

        #endregion

        #region ToOCPI(this EVSEStatus)

        /// <summary>
        /// Convert a WWCP EVSE status into OCPI EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">A WWCP EVSE status.</param>
        public static StatusType ToOCPI(this WWCP.EVSEStatusType EVSEStatus)
        {

            if      (EVSEStatus == WWCP.EVSEStatusType.Available)
                return StatusType.AVAILABLE;

            else if (EVSEStatus == WWCP.EVSEStatusType.Blocked)
                return StatusType.BLOCKED;

            else if (EVSEStatus == WWCP.EVSEStatusType.Charging)
                return StatusType.CHARGING;

            else if (EVSEStatus == WWCP.EVSEStatusType.OutOfService)
                return StatusType.INOPERATIVE;

            else if (EVSEStatus == WWCP.EVSEStatusType.Offline)
                return StatusType.INOPERATIVE;

            else if (EVSEStatus == WWCP.EVSEStatusType.Error)
                return StatusType.OUTOFORDER;

            else if (EVSEStatus == WWCP.EVSEStatusType.InDeployment)
                return StatusType.PLANNED;

            else if (EVSEStatus == WWCP.EVSEStatusType.Removed)
                return StatusType.REMOVED;

            else if (EVSEStatus == WWCP.EVSEStatusType.Reserved)
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

            => new (
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



            if (ChargingStation.Features.Contains(WWCP.ChargingStationFeature.Reservable))
                capabilities.Add(Capability.RESERVABLE);

            if (ChargingStation.Features.Contains(WWCP.ChargingStationFeature.ChargingProfilesSupported))
                capabilities.Add(Capability.CHARGING_PROFILE_CAPABLE);

            if (ChargingStation.Features.Contains(WWCP.ChargingStationFeature.ChargingPreferencesSupported))
                capabilities.Add(Capability.CHARGING_PREFERENCES_CAPABLE);

            if (ChargingStation.Features.Contains(WWCP.ChargingStationFeature.TokenGroupsSupported))
                capabilities.Add(Capability.TOKEN_GROUP_CAPABLE);

            if (ChargingStation.Features.Contains(WWCP.ChargingStationFeature.CSOUnlockSupported))
                capabilities.Add(Capability.UNLOCK_CAPABLE);


            return capabilities;

        }

        #endregion

        #region ToOCPI             (this Facilities)

        public static IEnumerable<Facility> ToOCPI(this IEnumerable<WWCP.Facility> Facilities)
        {

            var facilities = new HashSet<Facility>();

            foreach (var facility in Facilities)
            {

                switch (facility.ToString())
                {

                    case "HOTEL":           facilities.Add(OCPIv2_3_0.Facility.HOTEL);            break;
                    case "RESTAURANT":      facilities.Add(OCPIv2_3_0.Facility.RESTAURANT);       break;
                    case "CAFE":            facilities.Add(OCPIv2_3_0.Facility.CAFE);             break;
                    case "MALL":            facilities.Add(OCPIv2_3_0.Facility.MALL);             break;
                    case "SUPERMARKET":     facilities.Add(OCPIv2_3_0.Facility.SUPERMARKET);      break;
                    case "SPORT":           facilities.Add(OCPIv2_3_0.Facility.SPORT);            break;
                    case "RECREATION_AREA": facilities.Add(OCPIv2_3_0.Facility.RECREATION_AREA);  break;
                    case "NATURE":          facilities.Add(OCPIv2_3_0.Facility.NATURE);           break;
                    case "MUSEUM":          facilities.Add(OCPIv2_3_0.Facility.MUSEUM);           break;
                    case "BIKE_SHARING":    facilities.Add(OCPIv2_3_0.Facility.BIKE_SHARING);     break;
                    case "BUS_STOP":        facilities.Add(OCPIv2_3_0.Facility.BUS_STOP);         break;
                    case "TAXI_STAND":      facilities.Add(OCPIv2_3_0.Facility.TAXI_STAND);       break;
                    case "TRAM_STOP":       facilities.Add(OCPIv2_3_0.Facility.TRAM_STOP);        break;
                    case "METRO_STATION":   facilities.Add(OCPIv2_3_0.Facility.METRO_STATION);    break;
                    case "TRAIN_STATION":   facilities.Add(OCPIv2_3_0.Facility.TRAIN_STATION);    break;
                    case "AIRPORT":         facilities.Add(OCPIv2_3_0.Facility.AIRPORT);          break;
                    case "PARKING_LOT":     facilities.Add(OCPIv2_3_0.Facility.PARKING_LOT);      break;
                    case "CARPOOL_PARKING": facilities.Add(OCPIv2_3_0.Facility.CARPOOL_PARKING);  break;
                    case "FUEL_STATION":    facilities.Add(OCPIv2_3_0.Facility.FUEL_STATION);     break;
                    case "WIFI":            facilities.Add(OCPIv2_3_0.Facility.WIFI);             break;

                }

            }

            return facilities;

        }

        #endregion


        public static ParkingType ToOICP(this WWCP.ParkingType Parkingtype)
        {

            if (Parkingtype == WWCP.ParkingType.UNKNOWN)
                return ParkingType.UNKNOWN;

            if (Parkingtype == WWCP.ParkingType.ALONG_MOTORWAY)
                return ParkingType.ALONG_MOTORWAY;

            if (Parkingtype == WWCP.ParkingType.PARKING_GARAGE)
                return ParkingType.PARKING_GARAGE;

            if (Parkingtype == WWCP.ParkingType.PARKING_LOT)
                return ParkingType.PARKING_LOT;

            if (Parkingtype == WWCP.ParkingType.ON_DRIVEWAY)
                return ParkingType.ON_DRIVEWAY;

            if (Parkingtype == WWCP.ParkingType.ON_STREET)
                return ParkingType.ON_STREET;

            if (Parkingtype == WWCP.ParkingType.UNDERGROUND_GARAGE)
                return ParkingType.UNDERGROUND_GARAGE;

            if (Parkingtype == WWCP.ParkingType.OTHER)
                return ParkingType.UNKNOWN;

            throw new ArgumentException("Invalid parking type!");

        }

        public static Location_Id? ToOCPI(this WWCP.ChargingPool_Id              ChargingPoolId,
                                          ChargingPoolId_2_LocationId_Delegate?  CustomChargingPoolIdConverter = null)

            => CustomChargingPoolIdConverter is not null
                   ? CustomChargingPoolIdConverter(ChargingPoolId)
                   : Location_Id.TryParse(ChargingPoolId.Suffix);


        #region ToOCPI(this ChargingPool,  ref Warnings, IncludeEVSEIds = null)

        public static Location? ToOCPI(this WWCP.IChargingPool                  ChargingPool,
                                       ChargingPoolId_2_LocationId_Delegate?    CustomChargingPoolIdConverter,
                                       WWCPEVSEId_2_EVSEUId_Delegate?           CustomEVSEUIdConverter,
                                       WWCPEVSEId_2_EVSEId_Delegate?            CustomEVSEIdConverter,
                                       WWCP.IncludeEVSEIdDelegate               IncludeEVSEIds,
                                       WWCP.IncludeChargingConnectorIdDelegate  IncludeChargingConnectorIds,
                                       ref List<Warning>                        Warnings)
        {

            var location = ChargingPool.ToOCPI(
                               CustomChargingPoolIdConverter,
                               CustomEVSEUIdConverter,
                               CustomEVSEIdConverter,
                               IncludeEVSEIds,
                               IncludeChargingConnectorIds,
                               out var warnings
                           );

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return location;

        }

        #endregion

        #region ToOCPI(this ChargingPool,  out Warnings, IncludeEVSEIds = null)

        public static Location? ToOCPI(this WWCP.IChargingPool                  ChargingPool,
                                       ChargingPoolId_2_LocationId_Delegate?    CustomChargingPoolIdConverter,
                                       WWCPEVSEId_2_EVSEUId_Delegate?           CustomEVSEUIdConverter,
                                       WWCPEVSEId_2_EVSEId_Delegate?            CustomEVSEIdConverter,
                                       WWCP.IncludeEVSEIdDelegate               IncludeEVSEIds,
                                       WWCP.IncludeChargingConnectorIdDelegate  IncludeChargingConnectorIds,
                                       out IEnumerable<Warning>                 Warnings)
        {

            var includeEVSEIds  = IncludeEVSEIds ?? (evseId => true);
            var warnings        = new List<Warning>();

            if (ChargingPool.Operator is null)
            {
                warnings.Add(Warning.Create("The given charging location must have a valid charging station operator!"));
                Warnings = warnings;
                return null;
            }

            var locationId   = ChargingPool.Id.ToOCPI(CustomChargingPoolIdConverter);

            if (!locationId.HasValue)
            {
                warnings.Add(Warning.Create($"The given charging pool identification '{ChargingPool.Id}' could not be converted to an OCPI location identification!"));
                Warnings = warnings;
                return null;
            }

            if (ChargingPool.Address is null)
            {
                warnings.Add(Warning.Create("The given charging location must have a valid address!"));
                Warnings = warnings;
                return null;
            }

            if (ChargingPool.GeoLocation is null)
            {
                warnings.Add(Warning.Create("The given charging location must have a valid geo location!"));
                Warnings = warnings;
                return null;
            }

            try
            {

                Warnings = [];

                var evses = new List<EVSE>();

                foreach (var evse in ChargingPool.SelectMany(station => station.EVSEs).
                                                  Where     (evse    => includeEVSEIds(evse.Id)))
                {

                    var ocpiEVSE = evse.ToOCPI(
                                       CustomEVSEUIdConverter,
                                       CustomEVSEIdConverter,
                                       connectorId => true,
                                       evse.Status.Timestamp > evse.LastChangeDate
                                           ? evse.Status.Timestamp
                                           : evse.LastChangeDate,
                                       ref warnings
                                   );

                    if (ocpiEVSE is not null)
                        evses.Add(ocpiEVSE);

                }

                var subOperators = new List<BusinessDetails>();

                foreach (var brand in ChargingPool.Brands.Concat(ChargingPool.SelectMany(station => station.Brands)).Distinct().OrderBy(brand => brand.Name.FirstText()))
                {
                    subOperators.Add(
                        new BusinessDetails(
                            brand.Name.FirstText()
                        )
                    );
                }

                return new Location(

                           CountryCode:          CountryCode.Parse(ChargingPool.Id.OperatorId.CountryCode.Alpha2Code),
                           PartyId:              Party_Id.   Parse(ChargingPool.Id.OperatorId.Suffix),
                           Id:                   locationId.Value,
                           Publish:              true,
                           Address:              ChargingPool.Address.HouseNumber.IsNotNullOrEmpty()
                                                     ? $"{ChargingPool.Address.Street} {ChargingPool.Address.HouseNumber}"
                                                     :    ChargingPool.Address.Street,
                           City:                 ChargingPool.Address.City.FirstText(),
                           Country:              ChargingPool.Address.Country,
                           Coordinates:          ChargingPool.GeoLocation.Value,
                           TimeZone:             ChargingPool.Address.TimeZone?.ToString() ?? "UTC",

                           PublishAllowedTo:     null,
                           Name:                 ChargingPool.Name.FirstText(),
                           PostalCode:           ChargingPool.Address.PostalCode,
                           State:                null,
                           RelatedLocations:     [],
                           ParkingType:          ChargingPool.ParkingType?.ToOICP(),
                           EVSEs:                evses,
                           Directions:           [],
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
                           SubOperator:          subOperators.FirstOrDefault(),
                           Owner:                null,
                           Facilities:           ChargingPool.Facilities.  ToOCPI(),
                           OpeningTimes:         ChargingPool.OpeningTimes.ToOCPI(),
                           ChargingWhenClosed:   ChargingPool.ChargingWhenClosed,
                           Images:               null,
                           EnergyMix:            null,

                           CustomData:           ChargingPool.CustomData,
                           InternalData:         ChargingPool.InternalData,

                           LastUpdated:          ChargingPool.LastChangeDate

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create($"Could not convert the given charging pool '{ChargingPool.Id}' to OCPI: {ex.Message}"));
                Warnings = warnings;
            }

            return null;

        }

        #endregion

        #region ToOCPI(this ChargingPools, ref Warnings, IncludeEVSEIds = null)

        public static IEnumerable<Location> ToOCPI(this IEnumerable<WWCP.ChargingPool>      ChargingPools,
                                                   ChargingPoolId_2_LocationId_Delegate?    CustomChargingPoolIdConverter,
                                                   WWCPEVSEId_2_EVSEUId_Delegate?           CustomEVSEUIdConverter,
                                                   WWCPEVSEId_2_EVSEId_Delegate?            CustomEVSEIdConverter,
                                                   WWCP.IncludeEVSEIdDelegate               IncludeEVSEIds,
                                                   WWCP.IncludeChargingConnectorIdDelegate  IncludeChargingConnectorIds,
                                                   ref List<Warning>                        Warnings)
        {

            var locations = ChargingPools.ToOCPI(CustomChargingPoolIdConverter,
                                                 CustomEVSEUIdConverter,
                                                 CustomEVSEIdConverter,
                                                 IncludeEVSEIds,
                                                 IncludeChargingConnectorIds,
                                                 out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return locations;

        }

        #endregion

        #region ToOCPI(this ChargingPools, out Warnings, IncludeEVSEIds = null)

        public static IEnumerable<Location> ToOCPI(this IEnumerable<WWCP.ChargingPool>      ChargingPools,
                                                   ChargingPoolId_2_LocationId_Delegate?    CustomChargingPoolIdConverter,
                                                   WWCPEVSEId_2_EVSEUId_Delegate?           CustomEVSEUIdConverter,
                                                   WWCPEVSEId_2_EVSEId_Delegate?            CustomEVSEIdConverter,
                                                   WWCP.IncludeEVSEIdDelegate               IncludeEVSEIds,
                                                   WWCP.IncludeChargingConnectorIdDelegate  IncludeChargingConnectorIds,
                                                   out IEnumerable<Warning>                 Warnings)
        {

            var warnings   = new List<Warning>();
            var locations  = new HashSet<Location>();

            foreach (var chargingPool in ChargingPools)
            {

                try
                {

                    var chargingPool2 = chargingPool.ToOCPI(CustomChargingPoolIdConverter,
                                                            CustomEVSEUIdConverter,
                                                            CustomEVSEIdConverter,
                                                            IncludeEVSEIds,
                                                            IncludeChargingConnectorIds,
                                                            out var warning);

                    if (chargingPool2 is not null)
                        locations.Add(chargingPool2);

                    if (warning is not null && warning.Any())
                        warnings.AddRange(warning);

                }
                catch (Exception ex)
                {
                    warnings.Add(Warning.Create($"Could not convert the given charging pool '{chargingPool.Id}' to OCPI: " + ex.Message));
                }

            }

            Warnings = warnings.ToArray();
            return locations;

        }

        #endregion



        #region ToOCPI(this OpenSourceLicense)

        public static SoftwareLicense ToOCPI(this org.GraphDefined.Vanaheimr.Hermod.OpenSourceLicense OpenSourceLicense)

            => new (SoftwareLicense_Id.Parse(OpenSourceLicense.Id.ToString()),
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

        public static EnergyMeter<EVSE> ToOCPI(this WWCP.EnergyMeter EnergyMeter)

            => new (
                   EnergyMeter_Id.Parse(EnergyMeter.Id.ToString()),
                   EnergyMeter.Model,
                   EnergyMeter.ModelURL,
                   EnergyMeter.HardwareVersion,
                   EnergyMeter.FirmwareVersion,
                   EnergyMeter.Manufacturer,
                   EnergyMeter.ManufacturerURL,
                   EnergyMeter.PublicKeys.               Select(publicKey                  => PublicKey.Parse(publicKey. ToString())),
                   EnergyMeter.PublicKeyCertificateChain.HasValue ? CertificateChain.Parse(EnergyMeter.PublicKeyCertificateChain.Value.ToString()) : null,
                   EnergyMeter.TransparencySoftware.    Select(transparencySoftwareStatus => transparencySoftwareStatus.ToOCPI()),
                   EnergyMeter.Description.ToOCPI(),
                   EnergyMeter.Created,
                   EnergyMeter.LastChangeDate,
                   EnergyMeter.CustomData,
                   EnergyMeter.InternalData
               );

        #endregion


        #region ToOCPI(this EVSE,  ref Warnings)

        public static EVSE? ToOCPI(this WWCP.IEVSE                           EVSE,
                                   WWCPEVSEId_2_EVSEUId_Delegate?            CustomEVSEUIdConverter,
                                   WWCPEVSEId_2_EVSEId_Delegate?             CustomEVSEIdConverter,
                                   WWCP.IncludeChargingConnectorIdDelegate?  IncludeChargingConnectorIds,
                                   DateTimeOffset?                           LastUpdate,
                                   ref List<Warning>                         Warnings)
        {

            var evse = EVSE.ToOCPI(
                           CustomEVSEUIdConverter,
                           CustomEVSEIdConverter,
                           IncludeChargingConnectorIds,
                           LastUpdate,
                           out var warnings
                       );

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return evse;

        }

        #endregion

        #region ToOCPI(this EVSE,  out Warnings)

        public static EVSE? ToOCPI(this WWCP.IEVSE                           EVSE,
                                   WWCPEVSEId_2_EVSEUId_Delegate?            CustomEVSEUIdConverter,
                                   WWCPEVSEId_2_EVSEId_Delegate?             CustomEVSEIdConverter,
                                   WWCP.IncludeChargingConnectorIdDelegate?  IncludeChargingConnectorIds,
                                   DateTimeOffset?                           LastUpdate,
                                   out IEnumerable<Warning>                  Warnings)
        {

            var warnings = new List<Warning>();

            try
            {

                EVSE_UId? evseUId = null;

                // Favour the OCPI EVSE Unique identification over the WWCP EVSE identification!
                if (EVSE_UId.TryParse(EVSE.CustomData?[OCPI_EVSEUId]?.Value<String>() ?? "", out var uid))
                {
                    evseUId = uid;
                }
                else
                {

                    evseUId = EVSE.Id.ToOCPI_EVSEUId(CustomEVSEUIdConverter);

                    if (!evseUId.HasValue)
                    {
                        warnings.Add(Warning.Create($"The given EVSE identification '{EVSE.Id}' could not be converted to an OCPI EVSE Unique identification!"));
                        Warnings = warnings;
                        return null;
                    }

                }


                var evseId = EVSE.Id.ToOCPI_EVSEId(CustomEVSEIdConverter);

                if (!evseId.HasValue)
                {
                    warnings.Add(Warning.Create($"The given EVSE identification '{EVSE.Id}' could not be converted to an OCPI EVSE identification!"));
                    Warnings = warnings;
                    return null;
                }


                if (EVSE.ChargingStation is null)
                {
                    warnings.Add(Warning.Create($"The given EVSE '{EVSE.Id}' must have a valid charging station!"));
                    Warnings = warnings;
                    return null;
                }

                if (EVSE.ChargingPool is null)
                {
                    warnings.Add(Warning.Create($"The given EVSE '{EVSE.Id}' must have a valid charging pool!"));
                    Warnings = warnings;
                    return null;
                }

                var connectors   = EVSE.ChargingConnectors.
                                        Where  (connector         => IncludeChargingConnectorIds?.Invoke(connector.Id) ?? true).
                                        Select (chargingConnector => chargingConnector.ToOCPI(EVSE, ref warnings)).
                                        Where  (connector         => connector is not null).
                                        Cast<Connector>().
                                        ToArray();

                if (connectors.Length == 0)
                {
                    warnings.Add(Warning.Create($"The given EVSE socket outlets could not be converted to OCPI connectors!"));
                    Warnings = warnings;
                    return null;
                }


                Warnings = [];

                return new EVSE(

                           UId:                   evseUId.Value,
                           Status:                EVSE.Status.Value.ToOCPI(),
                           Connectors:            connectors,

                           EVSEId:                evseId,
                           StatusSchedule:        [],
                           Capabilities:          EVSE.ChargingStation.ToOCPI_Capabilities(),
                           EnergyMeter:           EVSE.EnergyMeter?.   ToOCPI(),
                           FloorLevel:            EVSE.ChargingStation.Address?.FloorLevel ?? EVSE.ChargingPool.Address?.FloorLevel,
                           Coordinates:           EVSE.ChargingStation.GeoLocation         ?? EVSE.ChargingPool.GeoLocation,
                           PhysicalReference:     EVSE.PhysicalReference                   ?? EVSE.ChargingStation.PhysicalReference,
                           Directions:            EVSE.ChargingStation.ArrivalInstructions.ToOCPI(),
                           Parking:               null,
                           Images:                [],

                           CustomData:            EVSE.CustomData,
                           InternalData:          EVSE.InternalData,

                           LastUpdated:           LastUpdate ?? EVSE.LastChangeDate

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create($"Could not convert the given EVSE '{EVSE.Id}' to OCPI: {ex.Message}"));
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

            var evses = EVSEs.ToOCPI(
                            CustomEVSEUIdConverter,
                            CustomEVSEIdConverter,
                            IncludeChargingConnectorIds,
                            out var warnings
                        );

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

                    var evse2 = evse.ToOCPI(
                                    CustomEVSEUIdConverter,
                                    CustomEVSEIdConverter,
                                    IncludeChargingConnectorIds,
                                    evse.Status.Timestamp > evse.LastChangeDate
                                        ? evse.Status.Timestamp
                                        : evse.LastChangeDate,
                                    out var warning
                                );

                    if (evse2 is not null)
                        evses.Add(evse2);

                    if (warning is not null && warning.Any())
                        warnings.AddRange(warning);

                }
                catch (Exception ex)
                {
                    warnings.Add(Warning.Create($"Could not convert the given EVSE '{evse.Id}' to OCPI: " + ex.Message));
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
                   _                                 => null,
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
                   _                                            => null,
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

            var connector = ChargingConnector.ToOCPI(EVSE, out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return connector;

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
                    warnings.Add(Warning.Create($"The given EVSE must not be null!"));
                    Warnings = warnings;
                    return null;
                }

                var connectorId  = Connector_Id.Parse(ChargingConnector.Id.ToString());
                var powerType    = EVSE.CurrentType.ToOCPI();

                if (!powerType.HasValue)
                {
                    warnings.Add(Warning.Create($"The given EVSE current type '{EVSE.CurrentType}' could not be converted to an OCPI power type!"));
                    Warnings = warnings;
                    return null;
                }

                var standard     = ChargingConnector.Plug.ToOCPI();

                if (!standard.HasValue)
                {
                    warnings.Add(Warning.Create($"The given socket outlet plug '{ChargingConnector.Plug}' could not be converted to an OCPI connector standard!"));
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

                           //MaxVoltage:              (UInt16) (EVSE.RMSVoltage.HasValue
                           //                             ? ((Double) EVSE.RMSVoltage.Value / Math.Sqrt(3))   // 400 V between two conductors => 230 V between conductor and neutral (OCPI design flaw!)
                           //                             : powerType.Value switch {
                           //                                   PowerTypes.AC_1_PHASE  => 230,
                           //                                   PowerTypes.AC_3_PHASE  => 230,                    // Line to neutral for AC_3_PHASE: https://github.com/ocpi/ocpi/blob/master/mod_locations.asciidoc#mod_locations_connector_object
                           //                                   PowerTypes.DC          => 400,
                           //                                   _                      => 0
                           //                               }),

                           MaxVoltage:              EVSE.MaxVoltage.HasValue && EVSE.MaxVoltage.Value.Value != 0
                                                        ? powerType.Value switch {
                                                              PowerTypes.AC_1_PHASE  => EVSE.MaxVoltage.Value,
                                                                                                              // 400 V between two conductors => 230 V between conductor and neutral (OCPI design flaw!)
                                                              PowerTypes.AC_3_PHASE  => EVSE.MaxVoltage.Value,//Volt.ParseV(EVSE.MaxVoltage.Value.Value / ((Decimal) Math.Sqrt(3))),
                                                              _                      => EVSE.MaxVoltage.Value
                                                          }
                                                        : powerType.Value switch {
                                                              PowerTypes.AC_1_PHASE  => Volt.ParseV(230),
                                                              PowerTypes.AC_3_PHASE  => Volt.ParseV(230),  // Line to neutral for AC_3_PHASE: https://github.com/ocpi/ocpi/blob/master/mod_locations.asciidoc#mod_locations_connector_object
                                                              PowerTypes.DC          => Volt.ParseV(400),
                                                              _                      => Volt.ParseV(0)
                                                          },

                           MaxAmperage:             EVSE.MaxCurrent     ?? powerType.Value switch {
                                                        PowerTypes.AC_1_PHASE  => Ampere.ParseA(16),
                                                        PowerTypes.AC_3_PHASE  => Ampere.ParseA(16),
                                                        PowerTypes.DC          => Ampere.ParseA(50),
                                                        _                      => Ampere.ParseA(0)
                                                    },
                           MaxElectricPower:        EVSE.MaxPower,

                           //TariffId:              Via lookup table!
                           TermsAndConditionsURL:   EVSE.Operator?.TermsAndConditionsURL,

                           LastUpdated:             EVSE.LastChangeDate

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create($"Could not convert the given socket outlet to OCPI: " + ex.Message));
                Warnings = warnings;
            }

            return null;

        }

        #endregion


        #region ToOCPI(this AuthMethod)

        public static AuthMethod? ToOCPI(this WWCP.AuthMethod AuthMethod)
        {

            if (AuthMethod == WWCP.AuthMethod.AUTH_REQUEST)
                return OCPIv2_3_0.AuthMethod.AUTH_REQUEST;

            if (AuthMethod == WWCP.AuthMethod.RESERVE)
                return OCPIv2_3_0.AuthMethod.COMMAND;

            if (AuthMethod == WWCP.AuthMethod.REMOTESTART)
                return OCPIv2_3_0.AuthMethod.COMMAND;

            if (AuthMethod == WWCP.AuthMethod.WHITELIST)
                return OCPIv2_3_0.AuthMethod.WHITELIST;

            return null;

        }

        public static AuthMethod? ToOCPI(this WWCP.AuthMethod? AuthMethod)

            => AuthMethod.HasValue
                   ? AuthMethod.Value.ToOCPI()
                   : null;

        #endregion

        #region ToWWCP(this AuthMethod)

        public static WWCP.AuthMethod? ToWWCP(this AuthMethod AuthMethod)
        {

            if (AuthMethod == AuthMethod.AUTH_REQUEST)
                return WWCP.AuthMethod.AUTH_REQUEST;

            if (AuthMethod == AuthMethod.WHITELIST)
                return WWCP.AuthMethod.WHITELIST;

            return null;

        }

        public static WWCP.AuthMethod? ToWWCP(this AuthMethod? AuthMethod)

            => AuthMethod.HasValue
                   ? AuthMethod.Value.ToWWCP()
                   : null;

        #endregion


        #region ToOCPI(this EnergyMeterId)

        public static EnergyMeter_Id? ToOCPI(this WWCP.EnergyMeter_Id EnergyMeterId)

            => EnergyMeter_Id.Parse(EnergyMeterId.ToString());

        public static EnergyMeter_Id? ToOCPI(this WWCP.EnergyMeter_Id? EnergyMeterId)

            => EnergyMeterId.HasValue
                   ? EnergyMeterId.Value.ToOCPI()
                   : null;

        #endregion

        #region ToWWCP(this MeterId)

        public static WWCP.EnergyMeter_Id? ToWWCP(this EnergyMeter_Id MeterId)

            => WWCP.EnergyMeter_Id.Parse(MeterId.ToString());

        public static WWCP.EnergyMeter_Id? ToWWCP(this EnergyMeter_Id? MeterId)

            => MeterId.HasValue
                   ? MeterId.Value.ToWWCP()
                   : null;

        #endregion


        #region ToOCPI(this ChargeDetailRecord, out Warnings)

        public static CDR? ToOCPI(this WWCP.ChargeDetailRecord           ChargeDetailRecord,
                                  ChargingPoolId_2_LocationId_Delegate?  CustomChargingPoolIdConverter,
                                  WWCPEVSEId_2_EVSEUId_Delegate?         CustomEVSEUIdConverter,
                                  WWCPEVSEId_2_EVSEId_Delegate?          CustomEVSEIdConverter,
                                  GetTariffIds_Delegate?                 GetTariffIdsDelegate,
                                  out IEnumerable<Warning>               Warnings)
        {

            var warnings  = new List<Warning>();
            var cdr       = ChargeDetailRecord.ToOCPI(CustomChargingPoolIdConverter,
                                                      CustomEVSEUIdConverter,
                                                      CustomEVSEIdConverter,
                                                      GetTariffIdsDelegate,
                                                      ref warnings);

            Warnings = warnings;

            return cdr;

        }

        #endregion

        #region ToOCPI(this ChargeDetailRecord, ref Warnings)

        public static CDR? ToOCPI(this WWCP.ChargeDetailRecord           ChargeDetailRecord,
                                  ChargingPoolId_2_LocationId_Delegate?  CustomChargingPoolIdConverter,
                                  WWCPEVSEId_2_EVSEUId_Delegate?         CustomEVSEUIdConverter,
                                  WWCPEVSEId_2_EVSEId_Delegate?          CustomEVSEIdConverter,
                                  GetTariffIds_Delegate?                 GetTariffIdsDelegate,
                                  ref List<Warning>                      Warnings)
        {

            try
            {

                if (ChargeDetailRecord is null)
                {
                    Warnings.Add(Warning.Create("The given charge detail record must not be null!"));
                    return null;
                }

                if (ChargeDetailRecord.ChargingStationOperator is null)
                {
                    Warnings.Add(Warning.Create("The given charge detail record must have a valid charging station operator!"));
                    return null;
                }

                if (!ChargeDetailRecord.SessionTime.EndTime.HasValue)
                {
                    Warnings.Add(Warning.Create("The session endtime of the given charge detail record must not be null!"));
                    return null;
                }

                if (!ChargeDetailRecord.SessionTime.Duration.HasValue)
                {
                    Warnings.Add(Warning.Create("The session time duration of the given charge detail record must not be null!"));
                    return null;
                }

                if (ChargeDetailRecord.AuthenticationStart?.AuthMethod.HasValue == false)
                {
                    Warnings.Add(Warning.Create("The authentication (verification) method used for starting of the given charge detail record must not be null!"));
                    return null;
                }

                var authMethod = ChargeDetailRecord.AuthenticationStart?.AuthMethod.ToOCPI();
                if (!authMethod.HasValue)
                {
                    Warnings.Add(Warning.Create("The authentication (verification) method used for starting of the given charge detail record is invalid!"));
                    return null;
                }

                if (ChargeDetailRecord.ChargingConnector is null)
                {
                    Warnings.Add("The Connector of the given charge detail record must not be null!".ToWarning());
                    return null;
                }

                if (ChargeDetailRecord.EVSE is null)
                {
                    Warnings.Add(Warning.Create("The EVSE of the given charge detail record must not be null!"));
                    return null;
                }

                if (ChargeDetailRecord.ChargingStation is null)
                {
                    Warnings.Add(Warning.Create("The charging station of the given charge detail record must not be null!"));
                    return null;
                }

                if (ChargeDetailRecord.ChargingPool is null)
                {
                    Warnings.Add(Warning.Create("The charging pool of the given charge detail record must not be null!"));
                    return null;
                }

                var filteredLocation = ChargeDetailRecord.ChargingPool.ToOCPI(
                                           CustomChargingPoolIdConverter,
                                           CustomEVSEUIdConverter,
                                           CustomEVSEIdConverter,
                                           evseId      => evseId      == ChargeDetailRecord.EVSE.Id,
                                           connectorId => connectorId == ChargeDetailRecord.ChargingConnector.Id,
                                           ref Warnings
                                       );

                if (filteredLocation is null)
                {
                    Warnings.Add(Warning.Create("The charging location of the given charge detail record could not be calculated!"));
                    return null;
                }

                var chargingPeriods = new List<ChargingPeriod>();

                foreach (var energyMeteringValue in ChargeDetailRecord.EnergyMeteringValues)
                {
                    chargingPeriods.Add(
                        ChargingPeriod.Create(
                            energyMeteringValue.Timestamp,
                            [
                                CDRDimension.Create(
                                    CDRDimensionType.ENERGY,
                                    energyMeteringValue.WattHours.kWh
                                )
                            ]
                        )
                    );
                }

                if (!ChargeDetailRecord.ChargingPrice.HasValue)
                {
                    Warnings.Add(Warning.Create("The charging price of the given charge detail record must not be null!"));
                    return null;
                }

                if (ChargeDetailRecord.ChargingPrice.Value.Currency is null)
                {
                    Warnings.Add(Warning.Create("The currency of the charging price of the given charge detail record must not be null!"));
                    return null;
                }

                if (!ChargeDetailRecord.ConsumedEnergy.HasValue)
                {
                    Warnings.Add(Warning.Create("The consumed energy of the given charge detail record must not be null!"));
                    return null;
                }

                if (ChargeDetailRecord.EnergyMeteringValues.Count() < 2)
                {
                    Warnings.Add(Warning.Create("At least two energy metering values are expected!"));
                    return null;
                }

                return new CDR(

                           CountryCode:                CountryCode.Parse(ChargeDetailRecord.ChargingStationOperator.Id.CountryCode.Alpha2Code),
                           PartyId:                    Party_Id.   Parse(ChargeDetailRecord.ChargingStationOperator.Id.Suffix),
                           Id:                         CDR_Id.     Parse(ChargeDetailRecord.Id.ToString()),
                           Start:                      ChargeDetailRecord.SessionTime.StartTime,
                           End:                        ChargeDetailRecord.SessionTime.EndTime. Value,
                           CDRToken:                   new CDRToken(
                                                           CountryCode:   CountryCode.Parse(ChargeDetailRecord.ChargingStationOperator.Id.CountryCode.Alpha2Code),
                                                           PartyId:       Party_Id.   Parse(ChargeDetailRecord.ChargingStationOperator.Id.Suffix),
                                                           UID:           Token_Id.Parse("123"),    //ToDo: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                                           TokenType:     TokenType.RFID,           //ToDo: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                                           ContractId:    Contract_Id.Parse("123")  //ToDo: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                                       ),
                           AuthMethod:                 authMethod.Value,
                           Location:                   new CDRLocation(          //ToDo: Might still have not required connectors!
                                                           Id:                   filteredLocation.Id,
                                                           Address:              filteredLocation.Address,
                                                           City:                 filteredLocation.City,
                                                           Country:              filteredLocation.Country,
                                                           Coordinates:          filteredLocation.Coordinates,
                                                           EVSEUId:              filteredLocation.EVSEUIds.First(),
                                                           EVSEId:               filteredLocation.EVSEIds. First(),
                                                           ConnectorId:          filteredLocation.EVSEs.First().Connectors.First().Id,          //ToDo: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                                           ConnectorStandard:    filteredLocation.EVSEs.First().Connectors.First().Standard,    //ToDo: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                                           ConnectorFormat:      filteredLocation.EVSEs.First().Connectors.First().Format,      //ToDo: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                                           ConnectorPowerType:   filteredLocation.EVSEs.First().Connectors.First().PowerType,   //ToDo: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                                                           Name:                 filteredLocation.Name,
                                                           PostalCode:           filteredLocation.PostalCode,
                                                           State:                filteredLocation.State
                                                       ),
                           Currency:                   ChargeDetailRecord.ChargingPrice.Value.Currency,
                           ChargingPeriods:            chargingPeriods,
                           TotalCosts:                 new Price(
                                                           BeforeTaxes: ChargeDetailRecord.ChargingPrice.Value.Base
                                                       ),
                           TotalEnergy:                ChargeDetailRecord.ConsumedEnergy.      Value,
                           TotalTime:                  ChargeDetailRecord.SessionTime.Duration.Value,

                           SessionId:                  null,
                           AuthorizationReference:     null,
                           EnergyMeterId:                    ChargeDetailRecord.EnergyMeterId.ToOCPI(),
                           EnergyMeter:                null,
                           TransparencySoftware:      null,
                           Tariffs:                    (IEnumerable<Tariff>?) (GetTariffIdsDelegate?.Invoke(
                                                           ChargeDetailRecord.ChargingStationOperatorId,
                                                           ChargeDetailRecord.ChargingPoolId,
                                                           ChargeDetailRecord.ChargingStationId,
                                                           ChargeDetailRecord.EVSEId,
                                                           ChargeDetailRecord.ChargingConnectorId,
                                                           ChargeDetailRecord.ProviderIdStart
                                                       )),
                           SignedData:                 null,
                           TotalFixedCosts:            null,
                           TotalEnergyCost:            null,
                           TotalTimeCost:              null,
                           TotalParkingTime:           null,
                           TotalParkingCost:           null,
                           TotalReservationCost:       null,
                           Remark:                     null,
                           InvoiceReferenceId:         null,
                           Credit:                     null,
                           CreditReferenceId:          null,
                           HomeChargingCompensation:   null,

                           LastUpdated:                ChargeDetailRecord.LastChangeDate// Timestamp.Now

                       );

            }
            catch (Exception ex)
            {
                Warnings.Add(Warning.Create("Could not convert the given charge detail record to OCPI: " + ex.Message));
            }

            return null;

        }

        #endregion


    }

}
