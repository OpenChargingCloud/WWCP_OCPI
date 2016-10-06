/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

namespace org.GraphDefined.WWCP.OCPIv2_1
{

    /// <summary>
    /// The capabilities of an EVSE.
    /// </summary>
    public enum CapabilityType
    {

        /// <summary>
        /// Unspecified capabilities.
        /// </summary>
        Unspecified                 = 0,

        /// <summary>
        /// The EVSE supports charging profiles. Sending Charging Profiles is not yet supported by OCPI.
        /// </summary>
        CHARGING_PROFILE_CAPABLE    = 1,

        /// <summary>
        /// Charging at this EVSE can be payed with credit card.
        /// </summary>
        CREDIT_CARD_PAYABLE         = 2,

        /// <summary>
        /// The EVSE can be reserved.
        /// </summary>
        RESERVABLE                  = 3,

        /// <summary>
        /// Charging at this EVSE can be authorized with a RFID token
        /// </summary>
        RFID_READER                 = 4

    }

}
