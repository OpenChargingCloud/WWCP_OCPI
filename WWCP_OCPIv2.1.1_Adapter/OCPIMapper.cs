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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSE identifications to OCPI EVSE identifications.
    /// </summary>
    /// <param name="EVSEId">A WWCP EVSE identification.</param>
    public delegate EVSE_Id     WWCPEVSEId_2_EVSEId_Delegate               (WWCP.EVSE_Id           EVSEId);

    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSEs to OCPI EVSEs.
    /// </summary>
    /// <param name="WWCPEVSE">A WWCP EVSE.</param>
    /// <param name="OCPIEVSE">An OCPI EVSE.</param>
    public delegate EVSE        WWCPEVSE_2_EVSE_Delegate                   (WWCP.IEVSE             WWCPEVSE,
                                                                            EVSE                   OCPIEVSE);

    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSE status updates to OCPI EVSE status types.
    /// </summary>
    /// <param name="WWCPEVSEStatusUpdate">A WWCP EVSE status update.</param>
    /// <param name="OCPIStatusType">An OICP status type.</param>
    public delegate StatusType  WWCPEVSEStatusUpdate_2_StatusType_Delegate (WWCP.EVSEStatusUpdate  WWCPEVSEStatusUpdate,
                                                                            StatusType             OCPIStatusType);

    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP charge detail records to OICP charge detail records.
    /// </summary>
    /// <param name="WWCPChargeDetailRecord">A WWCP charge detail record.</param>
    /// <param name="OCIPCDR">An OICP charge detail record.</param>
    public delegate CDR         WWCPChargeDetailRecord_2_CDR_Delegate      (WWCP.ChargeDetailRecord  WWCPChargeDetailRecord,
                                                                            CDR                      OCIPCDR);


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

        #region AsOCPIEVSEStatus(this EVSEStatus)

        /// <summary>
        /// Convert an OCPI v2.0 EVSE status into a corresponding WWCP EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">An OCPI v2.0 EVSE status.</param>
        /// <returns>The corresponding WWCP EVSE status.</returns>
        public static StatusType AsOCPIEVSEStatus(this WWCP.EVSEStatusTypes EVSEStatus)
        {

            //case WWCP.EVSEStatusTypes.Planned:
            //    return OCPIv2_2.EVSEStatusType.Planned;

            //case WWCP.EVSEStatusTypes.InDeployment:
            //    return OCPIv2_2.EVSEStatusType.Planned;

            if (EVSEStatus == WWCP.EVSEStatusTypes.Available)
                return StatusType.AVAILABLE;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Charging)
                return StatusType.CHARGING;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Error)
                return StatusType.OUTOFORDER;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.OutOfService)
                return StatusType.INOPERATIVE;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Offline)
                return StatusType.UNKNOWN;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Reserved)
                return StatusType.RESERVED;

            //case WWCP.EVSEStatusTypes.Private:
            //    return OCPIv2_2.EVSEStatusType.Unknown;

            //else if (EVSEStatus == WWCP.EVSEStatusTypes.UnknownEVSE)
            //    return StatusType.REMOVED;

            else
                return StatusType.UNKNOWN;

        }

        #endregion


        #region ToOCPI(this ChargingPool, out warnings)

        public static Location? ToOCPI(this WWCP.IChargingPool   ChargingPool,
                                       out IEnumerable<Warning>  Warnings)
        {

            var warnings = new List<Warning>();

            if (ChargingPool.Operator is null)
            {
                warnings.Add(Warning.Create(Languages.en, "The given charging location must have a valid charging station operator!"));
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

                return new Location(

                           CountryCode:          CountryCode.Parse(ChargingPool.Id.OperatorId.CountryCode.Alpha2Code),
                           PartyId:              Party_Id.   Parse(ChargingPool.Id.OperatorId.Suffix),
                           Id:                   Location_Id.Parse(ChargingPool.Id.Suffix),
                           LocationType:         LocationType.ON_STREET, // ????
                           Address:              String.Concat(ChargingPool.Address.Street, " ", ChargingPool.Address.HouseNumber),
                           City:                 ChargingPool.Address.City.FirstText(),
                           PostalCode:           ChargingPool.Address.PostalCode,
                           Country:              ChargingPool.Address.Country,
                           Coordinates:          ChargingPool.GeoLocation.Value,

                           Name:                 ChargingPool.Name.FirstText(),
                           RelatedLocations:     Array.Empty<AdditionalGeoLocation>(),
                           EVSEs:                Array.Empty<EVSE>(),
                           Directions:           Array.Empty<DisplayText>(),
                           Operator:             new BusinessDetails(
                                                     ChargingPool.Operator.Name.FirstText()
                                                 ),
                           SubOperator:          null,
                           Owner:                null,
                           Facilities:           Array.Empty<Facilities>(),
                           Timezone:             ChargingPool.Address.TimeZone?.ToString(),
                           OpeningTimes:         null,
                           ChargingWhenClosed:   null,
                           Images:               Array.Empty<Image>(),
                           EnergyMix:            null

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

        #region ToOCPI(this ChargingPools, out warnings)

        public static IEnumerable<Location> ToOCPI(this IEnumerable<WWCP.ChargingPool>  ChargingPools,
                                                   out IEnumerable<Warning>             Warnings)
        {

            var warnings   = new List<Warning>();
            var locations  = new HashSet<Location>();

            foreach (var chargingPool in ChargingPools)
            {

                try
                {

                    var chargingPool2 = chargingPool.ToOCPI(out var warning);

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


        #region ToOCPI(this EVSE, out warnings)

        public static EVSE? ToOCPI(this WWCP.IEVSE           EVSE,
                                   out IEnumerable<Warning>  Warnings)
        {

            var warnings = new List<Warning>();

            try
            {

                var evseUId = EVSE_UId.TryParse(EVSE.ToString());

                if (!evseUId.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE identificaton '{EVSE.Id}' could not be converted to an OCPI EVSE Unique identification!"));
                    Warnings = warnings;
                    return null;
                }


                var evseId  = EVSE_Id.TryParse(EVSE.ToString());

                if (!evseId.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE identificaton '{EVSE.Id}' could not be converted to an OCPI EVSE identification!"));
                    Warnings = warnings;
                    return null;
                }


                if (EVSE.ChargingStation is null)
                {
                    warnings.Add(Warning.Create(Languages.en, "The given EVSE must have a valid charging station!"));
                    Warnings = warnings;
                    return null;
                }


                Warnings = Array.Empty<Warning>();

                return new EVSE(

                           UId:                   evseUId.Value,
                           Status:                StatusType.AVAILABLE,
                           Connectors:            Array.Empty<Connector>(),

                           EVSEId:                evseId,
                           StatusSchedule:        Array.Empty<StatusSchedule>(),
                           Capabilities:          Array.Empty<Capability>(),
                           EnergyMeter:           null,
                           FloorLevel:            "",
                           Coordinates:           null,
                           PhysicalReference:     "",
                           Directions:            Array.Empty<DisplayText>(),
                           ParkingRestrictions:   Array.Empty<ParkingRestrictions>(),
                           Images:                Array.Empty<Image>(),

                           LastUpdated:           Timestamp.Now

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

        #region ToOCPI(this EVSEs)

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


    }

}
