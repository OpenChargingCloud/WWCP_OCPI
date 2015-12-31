/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPI_2_0
{

    /// <summary>
    /// The status of an EVSE.
    /// </summary>
    public enum EVSEStatusType
    {

        /// <summary>
        /// No status information available.
        /// </summary>
        Unknown         = 0,

        /// <summary>
        /// The EVSE is able to start a new charging session.
        /// </summary>
        Available       = 1,

        /// <summary>
        /// The EVSE not accessible because of a physical barrier, i.e. a car.
        /// </summary>
        Blocked         = 2,

        /// <summary>
        /// The EVSE is in use.
        /// </summary>
        Charging        = 3,

        /// <summary>
        /// The EVSE is not yet active or it is no longer available (deleted).
        /// </summary>
        Inoperative     = 4,

        /// <summary>
        /// The EVSE is currently out of order.
        /// </summary>
        OutOfOrder      = 5,

        /// <summary>
        /// The EVSE is planned, will be operating soon
        /// </summary>
        Planned         = 6,

        /// <summary>
        /// The EVSE/charge point is discontinued/removed.
        /// </summary>
        Removed         = 7,

        /// <summary>
        /// The EVSE is reserved for a particular EV driver and is unavailable for other drivers.
        /// </summary>
        Reserved        = 8

    }

}
