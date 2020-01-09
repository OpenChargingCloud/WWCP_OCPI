/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPIv2_2
{

    /// <summary>
    /// The status of a charging session.
    /// </summary>
    public enum SessionStatusType
    {

        /// <summary>
        /// The session is pending and has not yet started. This is the initial state.
        /// </summary>
        PENDING,

        /// <summary>
        /// The session is accepted and active.
        /// </summary>
        ACTIVE,

        /// <summary>
        /// The session has finished succesfully.
        /// </summary>
        COMPLETED,

        /// <summary>
        /// The session is declared invalid and will not be billed.
        /// </summary>
        INVALID

    }

}
