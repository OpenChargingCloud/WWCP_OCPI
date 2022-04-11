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
    /// The capabilities of an EVSE.
    /// </summary>
    public enum CapabilityTypes
    {

        /// <summary>
        /// The EVSE supports charging profiles.
        /// </summary>
        CHARGING_PROFILE_CAPABLE,

        /// <summary>
        /// The EVSE supports charging preferences.
        /// </summary>
        CHARGING_PREFERENCES_CAPABLE,

        /// <summary>
        /// EVSE has a payment terminal that supports chip cards.
        /// </summary>
        CHIP_CARD_SUPPORT,

        /// <summary>
        /// EVSE has a payment terminal that supports contactless cards.
        /// </summary>
        CONTACTLESS_CARD_SUPPORT,

        /// <summary>
        /// EVSE has a payment terminal that makes it possible to pay for charging using a credit card.
        /// </summary>
        CREDIT_CARD_PAYABLE,

        /// <summary>
        /// EVSE has a payment terminal that makes it possible to pay for charging using a debit card.
        /// </summary>
        DEBIT_CARD_PAYABLE,

        /// <summary>
        /// EVSE has a payment terminal with a pin-code entry device.
        /// </summary>
        PED_TERMINAL,

        /// <summary>
        /// The EVSE can remotely be started/stopped.
        /// </summary>
        REMOTE_START_STOP_CAPABLE,

        /// <summary>
        /// The EVSE can be reserved.
        /// </summary>
        RESERVABLE,

        /// <summary>
        /// Charging at this EVSE can be authorized with a RFID token.
        /// </summary>
        RFID_READER,

        /// <summary>
        /// This EVSE supports token groups, two or more tokens work as one, so that a session can be
        /// started with one token and stopped with another (handy when a card and key-fob are given
        /// to the EV-driver).
        /// </summary>
        TOKEN_GROUP_CAPABLE,

        /// <summary>
        /// Connectors have mechanical lock that can be requested by the eMSP to be unlocked.
        /// </summary>
        UNLOCK_CAPABLE

    }

}
