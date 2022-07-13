﻿/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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

using System;
using System.Linq;
using System.Collections.Generic;

using WWCP = org.GraphDefined.WWCP;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// 
    /// </summary>
    public static class OCPIMapper
    {

        #region AsWWCPEVSEStatus(this EVSEStatus)

        /// <summary>
        /// Convert an OCPI v2.0 EVSE status into a corresponding WWCP EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">An OCPI v2.0 EVSE status.</param>
        /// <returns>The corresponding WWCP EVSE status.</returns>
        public static WWCP.EVSEStatusTypes AsWWCPEVSEStatus(this OCPIv2_2.StatusTypes EVSEStatus)
        {

            switch (EVSEStatus)
            {

                case OCPIv2_2.StatusTypes.AVAILABLE:
                    return WWCP.EVSEStatusTypes.Available;

                case OCPIv2_2.StatusTypes.BLOCKED:
                    return WWCP.EVSEStatusTypes.OutOfService;

                case OCPIv2_2.StatusTypes.CHARGING:
                    return WWCP.EVSEStatusTypes.Charging;

                case OCPIv2_2.StatusTypes.INOPERATIVE:
                    return WWCP.EVSEStatusTypes.OutOfService;

                case OCPIv2_2.StatusTypes.OUTOFORDER:
                    return WWCP.EVSEStatusTypes.Faulted;

                //case OCPIv2_2.EVSEStatusType.Planned:
                //    return WWCP.EVSEStatusTypes.Planned;

                case OCPIv2_2.StatusTypes.REMOVED:
                    return WWCP.EVSEStatusTypes.UnknownEVSE;

                case OCPIv2_2.StatusTypes.RESERVED:
                    return WWCP.EVSEStatusTypes.Reserved;

                default:
                    return WWCP.EVSEStatusTypes.Unspecified;

            }

        }

        #endregion

        #region AsOCPIEVSEStatus(this EVSEStatus)

        /// <summary>
        /// Convert an OCPI v2.0 EVSE status into a corresponding WWCP EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">An OCPI v2.0 EVSE status.</param>
        /// <returns>The corresponding WWCP EVSE status.</returns>
        public static OCPIv2_2.StatusTypes AsOCPIEVSEStatus(this WWCP.EVSEStatusTypes EVSEStatus)
        {

            switch (EVSEStatus)
            {

                //case WWCP.EVSEStatusTypes.Planned:
                //    return OCPIv2_2.EVSEStatusType.Planned;

                //case WWCP.EVSEStatusTypes.InDeployment:
                //    return OCPIv2_2.EVSEStatusType.Planned;

                case WWCP.EVSEStatusTypes.Available:
                    return OCPIv2_2.StatusTypes.AVAILABLE;

                case WWCP.EVSEStatusTypes.Charging:
                    return OCPIv2_2.StatusTypes.CHARGING;

                case WWCP.EVSEStatusTypes.Faulted:
                    return OCPIv2_2.StatusTypes.OUTOFORDER;

                case WWCP.EVSEStatusTypes.OutOfService:
                    return OCPIv2_2.StatusTypes.INOPERATIVE;

                case WWCP.EVSEStatusTypes.Offline:
                    return OCPIv2_2.StatusTypes.UNKNOWN;

                case WWCP.EVSEStatusTypes.Reserved:
                    return OCPIv2_2.StatusTypes.RESERVED;

                //case WWCP.EVSEStatusTypes.Private:
                //    return OCPIv2_2.EVSEStatusType.Unknown;

                case WWCP.EVSEStatusTypes.UnknownEVSE:
                    return OCPIv2_2.StatusTypes.REMOVED;

                default:
                    return OCPIv2_2.StatusTypes.UNKNOWN;

            }

        }

        #endregion


        public static IEnumerable<Location> ToOCPI(this IEnumerable<WWCP.ChargingPool>  ChargingPools)

            => ChargingPools.SafeSelect(pool => {

                                            try
                                            {

                                                return new Location(CountryCode.Parse("DE"),
                                                                    Party_Id.Parse("GEF"),
                                                                    Location_Id.Parse("..."),
                                                                    true,
                                                                    pool.Address.Street + pool.Address.HouseNumber,
                                                                    pool.Address.City.FirstText(),
                                                                    pool.Address.Country.Alpha3Code,
                                                                    pool.GeoLocation ?? default,
                                                                    "timezone",
                                                                    null,
                                                                    "name",
                                                                    pool.Address.PostalCode,
                                                                    pool.Address.Country.CountryName.FirstText(),
                                                                    null,
                                                                    null,
                                                                    new EVSE[0],
                                                                    new DisplayText[] { new DisplayText(Languages.en, "directions") },
                                                                    new BusinessDetails(pool.Operator.Name.FirstText(),
                                                                                        URL.Parse(pool.Operator.Homepage),
                                                                                        null),
                                                                    null,
                                                                    null,
                                                                    new Facilities[0],
                                                                    null,
                                                                    null,
                                                                    null,
                                                                    null,
                                                                    DateTime.UtcNow);

                                            }
                                            catch (Exception e)
                                            {
                                                return null;
                                            }
                                        }).

                             Where(pool => pool != null);

    }

}
