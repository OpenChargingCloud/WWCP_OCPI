/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The command requested.
    /// </summary>
    public enum CommandTypes
    {

        /// <summary>
        /// Request the charge point to cancel a specific reservation.
        /// </summary>
        CANCEL_RESERVATION,

        /// <summary>
        /// Request the charge point to reserve a (specific) EVSE for a Token for a certain time, starting now.
        /// </summary>
        RESERVE_NOW,

        /// <summary>
        /// Request the charge point to start a transaction on the given EVSE/Connector.
        /// </summary>
        START_SESSION,

        /// <summary>
        /// Request the charge point to stop an ongoing session.
        /// </summary>
        STOP_SESSION,

        /// <summary>
        /// Request the charge point to unlock the connector (if applicable). This functionality is for help desk operators only!
        /// </summary>
        UNLOCK_CONNECTOR

    }

}
