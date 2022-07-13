/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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
    /// The tariff type.
    /// </summary>
    public enum TariffTypes
    {

        /// <summary>
        /// Used to describe that a tariff is valid when ad-hoc payment is used at the charging station
        /// (for example: Debit or credit card payment terminal).
        /// </summary>
        AD_HOC_PAYMENT,

        /// <summary>
        /// Used to describe that a tariff is valid when charging preference:
        /// CHEAP is set for the charging session.
        /// </summary>
        PROFILE_CHEAP,

        /// <summary>
        /// Used to describe that a tariff is valid when charging preference:
        /// FAST is set for the charging session.
        /// </summary>
        PROFILE_FAST,

        /// <summary>
        /// Used to describe that a tariff is valid when charging preference:
        /// GREEN is set for the charging session.
        /// </summary>
        PROFILE_GREEN,

        /// <summary>
        /// Used to describe that a tariff is valid when using an RFID, without
        /// any charging preference, or when Charging Preference:
        /// REGULAR is set for the charging session.
        /// </summary>
        REGULAR

    }

}
