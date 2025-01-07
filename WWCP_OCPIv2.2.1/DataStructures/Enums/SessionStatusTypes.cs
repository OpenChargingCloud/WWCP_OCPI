/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// The status of a charging session.
    /// </summary>
    public enum SessionStatusTypes
    {

        /// <summary>
        /// The session is accepted and active.
        /// </summary>
        ACTIVE,

        /// <summary>
        /// The session has been finished successfully.
        /// No more modifications will be made to the session using this state.
        /// </summary>
        COMPLETED,

        /// <summary>
        /// The session using this state is declared invalid and will not be billed.
        /// </summary>
        INVALID,

        /// <summary>
        /// The session is pending, it has not yet started. Not all pre-conditions are met.
        /// This is the initial state. The session might never become an active session.
        /// </summary>
        PENDING,

        /// <summary>
        /// The session is started due to a reservation, charging has not yet started.
        /// The session might never become an active session.
        /// </summary>
        RESERVATION

    }

}
