/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Result of the command that was send to the charge point.
    /// </summary>
    public enum CommandResultTypes
    {

        /// <summary>
        /// Command request accepted by the charge point.
        /// </summary>
        ACCEPTED,

        /// <summary>
        /// The Reservation has been canceled by the CPO.
        /// </summary>
        CANCELED_RESERVATION,

        /// <summary>
        /// EVSE is currently occupied, another session is ongoing. Cannot start a new session.
        /// </summary>
        EVSE_OCCUPIED,

        /// <summary>
        /// EVSE is currently inoperative or faulted.
        /// </summary>
        EVSE_INOPERATIVE,

        /// <summary>
        /// Execution of the command failed at the charge point.
        /// </summary>
        FAILED,

        /// <summary>
        /// The requested command is not supported by this charge point, EVSE etc.
        /// </summary>
        NOT_SUPPORTED,

        /// <summary>
        /// Command request rejected by the charge point.
        /// </summary>
        REJECTED,

        /// <summary>
        /// Command request timeout, no response received from the charge point in a reasonable time.
        /// </summary>
        TIMEOUT,

        /// <summary>
        /// The Reservation in the requested command is not known by this charge point.
        /// </summary>
        UNKNOWN_RESERVATION

    }

}
