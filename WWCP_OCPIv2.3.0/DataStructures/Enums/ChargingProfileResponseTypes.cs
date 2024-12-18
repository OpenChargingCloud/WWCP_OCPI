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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Response to the ChargingProfile request from the eMSP to the CPO.
    /// </summary>
    public enum ChargingProfileResponseTypes
    {

        /// <summary>
        /// ChargingProfile request accepted by the CPO, request will be forwarded to the EVSE.
        /// </summary>
        ACCEPTED,

        /// <summary>
        /// The ChargingProfiles not supported by this CPO, Charge Point, EVSE etc.
        /// </summary>
        NOT_SUPPORTED,

        /// <summary>
        /// ChargingProfile request rejected by the CPO. (Session might not be from a customer of the eMSP that send this request)
        /// </summary>
        REJECTED,

        /// <summary>
        /// ChargingProfile request rejected by the CPO, requests are send more often then allowed.
        /// </summary>
        TOO_OFTEN,

        /// <summary>
        /// The Session in the requested command is not known by this CPO.
        /// </summary>
        UNKNOWN_SESSION

    }

}
