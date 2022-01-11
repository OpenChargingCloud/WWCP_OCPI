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
    /// Defines when authorization of a Token by the CPO is allowed.
    /// </summary>
    public enum WhitelistTypes
    {

        /// <summary>
        /// Token always has to be whitelisted, realtime authorization is not possible/allowed.
        /// CPO shall always allow any use of this Token.
        /// </summary>
        ALWAYS,

        /// <summary>
        /// It is allowed to whitelist the token, realtime authorization is also allowed.
        /// The CPO may choose which version of authorization to use.
        /// </summary>
        ALLOWED,

        /// <summary>
        /// In normal situations realtime authorization shall be used. But when the CPO cannot
        /// get a response from the eMSP (communication between CPO and eMSP is offline),
        /// the CPO shall allow this Token to be used.
        /// </summary>
        ALLOWED_OFFLINE,

        /// <summary>
        /// Whitelisting is forbidden, only realtime authorization is allowed. CPO shall always
        /// send a realtime authorization for any use of this Token to the eMSP.
        /// </summary>
        NEVER

    }

}
