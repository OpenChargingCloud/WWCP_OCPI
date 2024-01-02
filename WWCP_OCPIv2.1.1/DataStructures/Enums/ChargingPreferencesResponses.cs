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
    /// Different smart charging response types.
    /// </summary>
    public enum ChargingPreferencesResponses
    {

        /// <summary>
        /// Charging Preferences accepted, EVSE will try to accomplish them, although this is no guarantee that they will be fulfilled.
        /// </summary>
        ACCEPTED,

        /// <summary>
        /// CPO requires departure_time to be able to perform Charging Preference based Smart Charging.
        /// </summary>
        DEPARTURE_REQUIRED,

        /// <summary>
        /// CPO requires energy_need to be able to perform Charging Preference based Smart Charging.
        /// </summary>
        ENERGY_NEED_REQUIRED,

        /// <summary>
        /// Charging Preferences contain a demand that the EVSE knows it cannot fulfill.
        /// </summary>
        NOT_POSSIBLE,

        /// <summary>
        /// profile_type contains a value that is not supported by the EVSE.
        /// </summary>
        PROFILE_TYPE_NOT_SUPPORTED

    }

}
