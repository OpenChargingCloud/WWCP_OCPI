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
    /// Charge detail record dimensions.
    /// </summary>
    public enum CDRDimensions
    {

        /// <summary>
        /// Average charging current during this ChargingPeriod: defined in A (Ampere). When negative, the current is flowing from the EV to the grid.
        /// </summary>
        CURRENT,        // Session only!

        /// <summary>
        /// Total amount of energy (dis-)charged during this ChargingPeriod: defined in kWh. When negative, more energy was feed into the grid then charged into the EV. Default step_size is 1.
        /// </summary>
        ENERGY,

        /// <summary>
        /// Total amount of energy feed back into the grid: defined in kWh.
        /// </summary>
        ENERGY_EXPORT,  // Session only!

        /// <summary>
        /// Total amount of energy charged, defined in kWh.
        /// </summary>
        ENERGY_IMPORT,  // Session only!

        /// <summary>
        /// Sum of the maximum current over all phases, reached during this ChargingPeriod: defined in A (Ampere).
        /// </summary>
        MAX_CURRENT,

        /// <summary>
        /// Sum of the minimum current over all phases, reached during this ChargingPeriod, when negative, current has flowed from the EV to the grid. Defined in A (Ampere).
        /// </summary>
        MIN_CURRENT,

        /// <summary>
        /// Maximum power reached during this ChargingPeriod: defined in kW (Kilowatt).
        /// </summary>
        MAX_POWER,

        /// <summary>
        /// Minimum power reached during this ChargingPeriod: defined in kW (Kilowatt), when negative, the power has flowed from the EV to the grid.
        /// </summary>
        MIN_POWER,

        /// <summary>
        /// Time during this ChargingPeriod not charging: defined in hours, default step_size multiplier is 1 second.
        /// </summary>
        PARKING_TIME,

        /// <summary>
        /// Average power during this ChargingPeriod: defined in kW (Kilowatt). When negative, the power is flowing from the EV to the grid.
        /// </summary>
        POWER,          // Session only!

        /// <summary>
        /// Time during this ChargingPeriod Charge Point has been reserved and not yet been in use for this customer: defined in hours, default step_size multiplier is 1 second.
        /// </summary>
        RESERVATION_TIME,

        /// <summary>
        /// Current state of charge of the EV, in percentage, values allowed: 0 to 100. See note below.
        /// </summary>
        STATE_OF_CHARGE,           // Session only!

        /// <summary>
        /// Time charging during this ChargingPeriod: defined in hours, default step_size multiplier is 1 second.
        /// </summary>
        TIME

    }

}
