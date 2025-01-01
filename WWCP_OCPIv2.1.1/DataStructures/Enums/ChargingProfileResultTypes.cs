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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Result of a ChargingProfile request that the EVSE sends via the CPO to the eMSP.
    /// </summary>
    public enum ChargingProfileResultTypes
    {

        /// <summary>
        /// No Charging Profile(s) were found by the EVSE matching the request.
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// ChargingProfile request accepted by the EVSE.
        /// </summary>
        ACCEPTED,

        /// <summary>
        /// ChargingProfile request rejected by the EVSE.
        /// </summary>
        REJECTED

    }

}
