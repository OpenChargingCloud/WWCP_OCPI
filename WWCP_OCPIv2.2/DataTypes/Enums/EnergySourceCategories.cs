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
    /// Categories of energy sources.
    /// </summary>
    public enum EnergySourceCategories
    {

        /// <summary>
        /// Nuclear power sources.
        /// </summary>
        NUCLEAR,

        /// <summary>
        /// All kinds of fossil power sources.
        /// </summary>
        GENERAL_FOSSIL,

        /// <summary>
        /// Fossil power from coal.
        /// </summary>
        COAL,

        /// <summary>
        /// Fossil power from gas.
        /// </summary>
        GAS,

        /// <summary>
        /// All kinds of regenerative power sources.
        /// </summary>
        GENERAL_GREEN,

        /// <summary>
        /// Regenerative power from PV.
        /// </summary>
        SOLAR,

        /// <summary>
        /// Regenerative power from wind turbines.
        /// </summary>
        WIND,

        /// <summary>
        /// Regenerative power from water turbines.
        /// </summary>
        WATER

    }

}
