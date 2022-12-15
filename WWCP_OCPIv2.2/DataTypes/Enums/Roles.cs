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
    /// The role of a party.
    /// </summary>
    public enum Roles
    {

        OpenData,

        /// <summary>
        /// Charge point operator.
        /// </summary>
        CPO,

        /// <summary>
        /// eMobility service provider.
        /// </summary>
        EMSP,

        /// <summary>
        /// Hub.
        /// </summary>
        HUB,

        /// <summary>
        /// National Access Point: National database with all location information of a country.
        /// </summary>
        NAP,

        /// <summary>
        /// Navigation Service Provider: Like an eMSP, but probably only interested in location information.
        /// </summary>
        NSP,

        /// <summary>
        /// Other.
        /// </summary>
        OTHER,

        /// <summary>
        /// Smart charging service provider.
        /// </summary>
        SCSP

    }

}
