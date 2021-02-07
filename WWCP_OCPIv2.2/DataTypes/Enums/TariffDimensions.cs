/*
 * Copyright (c) 2015-2021 GraphDefined GmbH
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
    /// Tariff dimensions.
    /// </summary>
    public enum TariffDimensions
    {

        /// <summary>
        /// Defined in kWh, step_size multiplier: 1 Wh.
        /// </summary>
        ENERGY,

        /// <summary>
        /// Flat fee without unit for step_size.
        /// </summary>
        FLAT,

        /// <summary>
        /// Time not charging: defined in hours, step_size multiplier: 1 second.
        /// </summary>
        PARKING_TIME,

        /// <summary>
        /// Time charging: defined in hours, step_size multiplier: 1 second.
        /// Can also be used in combination with a RESERVATION restriction to describe
        /// the price of the reservation time.
        /// </summary>
        TIME

    }

}
