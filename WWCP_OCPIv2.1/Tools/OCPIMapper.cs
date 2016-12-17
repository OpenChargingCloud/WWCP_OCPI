/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

namespace org.GraphDefined.WWCP.OCPIv2_1.Tools
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
        public static WWCP.EVSEStatusType AsWWCPEVSEStatus(this OCPIv2_1.EVSEStatusType EVSEStatus)
        {

            switch (EVSEStatus)
            {

                case OCPIv2_1.EVSEStatusType.Available:
                    return WWCP.EVSEStatusType.Available;

                case OCPIv2_1.EVSEStatusType.Blocked:
                    return WWCP.EVSEStatusType.OutOfService;

                case OCPIv2_1.EVSEStatusType.Charging:
                    return WWCP.EVSEStatusType.Charging;

                case OCPIv2_1.EVSEStatusType.Inoperative:
                    return WWCP.EVSEStatusType.OutOfService;

                case OCPIv2_1.EVSEStatusType.OutOfOrder:
                    return WWCP.EVSEStatusType.Faulted;

                case OCPIv2_1.EVSEStatusType.Planned:
                    return WWCP.EVSEStatusType.Planned;

                case OCPIv2_1.EVSEStatusType.Removed:
                    return WWCP.EVSEStatusType.UnknownEVSE;

                case OCPIv2_1.EVSEStatusType.Reserved:
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
        public static OCPIv2_1.EVSEStatusType AsOCPIEVSEStatus(this WWCP.EVSEStatusType EVSEStatus)
        {

            switch (EVSEStatus)
            {

                case WWCP.EVSEStatusType.Planned:
                    return OCPIv2_1.EVSEStatusType.Planned;

                case WWCP.EVSEStatusType.InDeployment:
                    return OCPIv2_1.EVSEStatusType.Planned;

                case WWCP.EVSEStatusType.Available:
                    return OCPIv2_1.EVSEStatusType.Available;

                case WWCP.EVSEStatusType.Charging:
                    return OCPIv2_1.EVSEStatusType.Charging;

                case WWCP.EVSEStatusType.Faulted:
                    return OCPIv2_1.EVSEStatusType.OutOfOrder;

                case WWCP.EVSEStatusType.OutOfService:
                    return OCPIv2_1.EVSEStatusType.Inoperative;

                case WWCP.EVSEStatusType.Offline:
                    return OCPIv2_1.EVSEStatusType.Unknown;

                case WWCP.EVSEStatusType.Reserved:
                    return OCPIv2_1.EVSEStatusType.Reserved;

                case WWCP.EVSEStatusType.Private:
                    return OCPIv2_1.EVSEStatusType.Unknown;

                case WWCP.EVSEStatusType.UnknownEVSE:
                    return OCPIv2_1.EVSEStatusType.Removed;

                default:
                    return OCPIv2_1.EVSEStatusType.Unknown;

            }

        }

        #endregion

    }

}
