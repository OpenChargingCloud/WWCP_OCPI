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

#region Usings

using System;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// This type is used to schedule EVSE status periods in the future.
    /// </summary>
    public struct EVSEStatusSchedule
    {

        #region Properties

        /// <summary>
        /// Begin of the scheduled period.
        /// </summary>
        public DateTime         Begin         { get; }

        /// <summary>
        /// Optional end of the scheduled period.
        /// </summary>
        public DateTime?        End           { get; }

        /// <summary>
        /// EVSE status value during the scheduled period.
        /// </summary>
        public EVSEStatusTypes  EVSEStatus    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// This type is used to schedule EVSE status periods in the future.
        /// </summary>
        /// <param name="EVSEStatus">EVSE status value during the scheduled period.</param>
        /// <param name="Begin">Begin of the scheduled period.</param>
        /// <param name="End">Optional end of the scheduled period.</param>
        public EVSEStatusSchedule(EVSEStatusTypes  EVSEStatus,
                                  DateTime         Begin,
                                  DateTime?        End = null)
        {

            this.EVSEStatus  = EVSEStatus;
            this.Begin       = Begin;
            this.End         = End;

        }

        #endregion

    }

}
