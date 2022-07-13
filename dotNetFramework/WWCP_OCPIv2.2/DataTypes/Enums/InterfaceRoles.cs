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

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The interface role of a party.
    /// </summary>
    public enum InterfaceRoles
    {

        /// <summary>
        /// Sender Interface implementation. Interface implemented by the owner of data, so the Receiver can Pull information from the data Sender/owner.
        /// </summary>
        SENDER,

        /// <summary>
        /// Receiver Interface implementation. Interface implemented by the receiver of data, so the Sender/owner can Push information to the Receiver.
        /// </summary>
        RECEIVER

    }

}
