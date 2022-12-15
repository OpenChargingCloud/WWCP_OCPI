/*
 * Copyright (c) 2015-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The status of an EVSE/connector.
    /// </summary>
    public enum StatusTypes
    {

        /// <summary>
        /// No status information available (also used when offline).
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// The EVSE/Connector is able to start a new charging session.
        /// </summary>
        AVAILABLE,

        /// <summary>
        /// The EVSE/Connector is not accessible because of a physical barrier, i.e. a car.
        /// </summary>
        BLOCKED,

        /// <summary>
        /// The EVSE/Connector is in use.
        /// </summary>
        CHARGING,

        /// <summary>
        /// The EVSE/Connector is not yet active or it is no longer available (deleted).
        /// </summary>
        INOPERATIVE,

        /// <summary>
        /// The EVSE/Connector is currently out of order.
        /// </summary>
        OUTOFORDER,

        /// <summary>
        /// The EVSE/Connector is planned, will be operating soon.
        /// </summary>
        PLANNED,

        /// <summary>
        /// The EVSE/Connector was discontinued/removed.
        /// </summary>
        REMOVED,

        /// <summary>
        /// The EVSE/Connector is reserved for a particular EV driver and is unavailable for other drivers.
        /// </summary>
        RESERVED

    }

}
