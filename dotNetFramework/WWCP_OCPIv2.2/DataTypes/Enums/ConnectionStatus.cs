﻿/*
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
    /// The connection status of a party.
    /// </summary>
    public enum ConnectionStatus
    {

        /// <summary>
        /// Party is connected.
        /// </summary>
        CONNECTED,

        /// <summary>
        /// Party is currently not connected.
        /// </summary>
        OFFLINE,

        /// <summary>
        /// Connection to this party is planned, but has never been connected.
        /// </summary>
        PLANNED,

        /// <summary>
        /// Party is now longer active, will never connect anymore.
        /// </summary>
        SUSPENDED

    }

}
