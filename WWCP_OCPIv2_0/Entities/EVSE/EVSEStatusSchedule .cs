/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

#region Usings

using System;

#endregion

namespace org.GraphDefined.WWCP.OCPIv2_0
{

    /// <summary>
    /// This type is used to schedule EVSE status periods in the future.
    /// </summary>
    public struct EVSEStatusSchedule
    {

        #region Properties

        #region Begin

        private readonly DateTime _Begin;

        /// <summary>
        /// Begin of the scheduled period.
        /// </summary>
        public DateTime Begin
        {
            get
            {
                return _Begin;
            }
        }

        #endregion

        #region End

        private readonly DateTime? _End;

        /// <summary>
        /// Optional end of the scheduled period.
        /// </summary>
        public DateTime? End
        {
            get
            {
                return _End;
            }
        }

        #endregion

        #region EVSEStatus

        private readonly EVSEStatusType _EVSEStatus;

        /// <summary>
        /// EVSE status value during the scheduled period.
        /// </summary>
        public EVSEStatusType EVSEStatus
        {
            get
            {
                return _EVSEStatus;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// This type is used to schedule EVSE status periods in the future.
        /// </summary>
        /// <param name="Begin">Begin of the scheduled period.</param>
        /// <param name="End">Optional end of the scheduled period.</param>
        /// <param name="EVSEStatus">EVSE status value during the scheduled period.</param>
        public EVSEStatusSchedule(DateTime        Begin,
                                  DateTime?       End,
                                  EVSEStatusType  EVSEStatus)
        {

            this._Begin       = Begin;
            this._End         = End;
            this._EVSEStatus  = EVSEStatus;

        }

        #endregion

    }

}
