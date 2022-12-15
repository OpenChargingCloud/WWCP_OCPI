/*
 * Copyright (c) 2015-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Allowed types of tokens.
    /// </summary>
    public enum AllowedTypes
    {

        /// <summary>
        /// This token is allowed to charge (at this location).
        /// </summary>
        ALLOWED,

        /// <summary>
        /// This token is blocked.
        /// </summary>
        BLOCKED,

        /// <summary>
        /// This token has expired.
        /// </summary>
        EXPIRED,

        /// <summary>
        /// This token belongs to an account that has not enough credits to charge (at the given location).
        /// </summary>
        NO_CREDIT,

        /// <summary>
        /// Token is valid, but is not allowed to charge at the given location.
        /// </summary>
        NOT_ALLOWED

    }

}
