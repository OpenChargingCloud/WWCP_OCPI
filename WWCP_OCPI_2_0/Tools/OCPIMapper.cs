/*
 * Copyright (c) 2015 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

namespace org.GraphDefined.WWCP.OCPI_2_0.Tools
{

    /// <summary>
    /// Helper methods to map OCPI v2.0 data type values to
    /// WWCP data type values and vice versa.
    /// </summary>
    public static class OCPIMapper
    {

        #region AsWWCPEVSEStatus(this EVSEStatus)

        /// <summary>
        /// Convert an OCPI v2.0 EVSE status into a corresponding WWCP EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">An OCPI v2.0 EVSE status.</param>
        /// <returns>The corresponding WWCP EVSE status.</returns>
        public static WWCP.EVSEStatusType AsWWCPEVSEStatus(this OCPI_2_0.EVSEStatusType EVSEStatus)
        {

            switch (EVSEStatus)
            {

                case OCPI_2_0.EVSEStatusType.Available:
                    return WWCP.EVSEStatusType.Available;

                case OCPI_2_0.EVSEStatusType.Blocked:
                    return WWCP.EVSEStatusType.OutOfService;

                case OCPI_2_0.EVSEStatusType.Charging:
                    return WWCP.EVSEStatusType.Occupied;

                case OCPI_2_0.EVSEStatusType.Inoperative:
                    return WWCP.EVSEStatusType.OutOfService;

                case OCPI_2_0.EVSEStatusType.OutOfOrder:
                    return WWCP.EVSEStatusType.Faulted;

                case OCPI_2_0.EVSEStatusType.Planned:
                    return WWCP.EVSEStatusType.Planned;

                case OCPI_2_0.EVSEStatusType.Removed:
                    return WWCP.EVSEStatusType.EvseNotFound;

                case OCPI_2_0.EVSEStatusType.Reserved:
                    return WWCP.EVSEStatusType.Reserved;

                default:
                    return WWCP.EVSEStatusType.Unspecified;

            }

        }

        #endregion

        #region AsOCPIEVSEStatus(this EVSEStatus)

        /// <summary>
        /// Convert an OCPI v2.0 EVSE status into a corresponding WWCP EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">An OCPI v2.0 EVSE status.</param>
        /// <returns>The corresponding WWCP EVSE status.</returns>
        public static OCPI_2_0.EVSEStatusType AsOCPIEVSEStatus(this WWCP.EVSEStatusType EVSEStatus)
        {

            switch (EVSEStatus)
            {

                case WWCP.EVSEStatusType.Planned:
                    return OCPI_2_0.EVSEStatusType.Planned;

                case WWCP.EVSEStatusType.InDeployment:
                    return OCPI_2_0.EVSEStatusType.Planned;

                case WWCP.EVSEStatusType.Available:
                    return OCPI_2_0.EVSEStatusType.Available;

                case WWCP.EVSEStatusType.Occupied:
                    return OCPI_2_0.EVSEStatusType.Charging;

                case WWCP.EVSEStatusType.Faulted:
                    return OCPI_2_0.EVSEStatusType.OutOfOrder;

                case WWCP.EVSEStatusType.OutOfService:
                    return OCPI_2_0.EVSEStatusType.Inoperative;

                case WWCP.EVSEStatusType.Offline:
                    return OCPI_2_0.EVSEStatusType.Unknown;

                case WWCP.EVSEStatusType.Reserved:
                    return OCPI_2_0.EVSEStatusType.Reserved;

                case WWCP.EVSEStatusType.Other:
                    return OCPI_2_0.EVSEStatusType.Unknown;

                case WWCP.EVSEStatusType.EvseNotFound:
                    return OCPI_2_0.EVSEStatusType.Removed;

                default:
                    return OCPI_2_0.EVSEStatusType.Unknown;

            }

        }

        #endregion

    }

}
