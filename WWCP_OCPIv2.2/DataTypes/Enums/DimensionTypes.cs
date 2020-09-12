/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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
    /// The authentication method used.
    /// </summary>
    public enum DimensionTypes
    {

        /// <summary>
        /// defined in kWh, default division is 1 Wh
        /// </summary>
        ENERGY,

        /// <summary>
        /// flat fee, no unit
        /// </summary>
        FLAT,

        /// <summary>
        /// defined in A (Ampere), Maximum current
        /// </summary>
        MAX_CURRENT,

        /// <summary>
        /// defined in A (Ampere), Minimum current
        /// </summary>
        MIN_CURRENT,

        /// <summary>
        /// time not charging: defined in hours, default division is 1 second
        /// </summary>
        PARKING_TIME,

        /// <summary>
        /// time charging: defined in hours, default division is 1 second
        /// </summary>
        TIME

    }

}
