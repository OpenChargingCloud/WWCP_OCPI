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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Unit in which a charging profile is defined.
    /// </summary>
    public enum ChargingRateUnits
    {

        /// <summary>
        /// Watts (power)
        /// This is the TOTAL allowed charging power. If used for AC Charging, the phase current should be calculated via:
        /// Current per phase = Power / (Line Voltage * Number of Phases). The "Line Voltage" used in the calculation is
        /// the Line to Neutral Voltage (VLN). In Europe and Asia VLN is typically 220V or 230V and the corresponding Line
        /// to Line Voltage (VLL) is 380V and 400V. The "Number of Phases" is the numberPhases from the ChargingProfilePeriod.
        /// It is usually more convenient to use this for DC charging. Note that if numberPhases in a ChargingProfilePeriod
        /// is absent, 3 SHALL be assumed.
        /// </summary>
        W,

        /// <summary>
        /// Amperes (current)
        /// The amount of Ampere per phase, not the sum of all phases. It is usually more convenient to use this for AC charging.
        /// </summary>
        A

    }

}
