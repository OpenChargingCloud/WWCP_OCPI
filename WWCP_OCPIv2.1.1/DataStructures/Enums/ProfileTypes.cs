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
    /// Different smart charging profile types.
    /// </summary>
    public enum ProfileTypes
    {

        /// <summary>
        /// Driver wants to use the cheapest charging profile possible.
        /// </summary>
        CHEAP,

        /// <summary>
        /// Driver wants his EV charged as quickly as possible and is willing
        /// to pay a premium for this, if needed.
        /// </summary>
        FAST,

        /// <summary>
        /// Driver wants his EV charged with as much regenerative (green) energy as possible.
        /// </summary>
        GREEN,

        /// <summary>
        /// Driver does not have special preferences.
        /// </summary>
        REGULAR

    }

}
