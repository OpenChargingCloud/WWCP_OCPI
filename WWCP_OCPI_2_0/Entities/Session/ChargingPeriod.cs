/*
 * Copyright (c) 2015 GraphDefined GmbH
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
using System.Linq;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.WWCP.OCPI_2_0
{

    /// <summary>
    /// A charging period consists of a start timestamp and a list of
    /// possible values that influence this period, for example:
    /// Amount of energy charged this period, maximum current during
    /// this period etc.
    /// </summary>
    public struct ChargingPeriod
    {

        #region Properties

        #region Start

        private readonly DateTime _Start;

        /// <summary>
        /// Start timestamp of the charging period.
        /// This period ends when a next period starts,
        /// the last period ends when the session ends.
        /// </summary>
        public DateTime Start
        {
            get
            {
                return _Start;
            }
        }

        #endregion

        #region Dimensions

        private readonly IEnumerable<CDRDimension> _Dimensions;

        /// <summary>
        /// List of relevant values for this charging period.
        /// </summary>
        public IEnumerable<CDRDimension> Dimensions
        {
            get
            {
                return _Dimensions;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A charging period consists of a start timestamp and a
        /// list of possible values that influence this period.
        /// </summary>
        /// <param name="Start">Start timestamp of the charging period.</param>
        /// <param name="Dimensions">List of relevant values for this charging period.</param>
        public ChargingPeriod(DateTime                   Start,
                              IEnumerable<CDRDimension>  Dimensions)
        {

            #region Initial checks

            if (Dimensions == null)
                throw new ArgumentNullException("Dimensions", "The given parameter must not be null!");

            if (!Dimensions.Any())
                throw new ArgumentNullException("Dimensions", "The given enumeration must not be empty!");

            #endregion

            this._Start       = Start;
            this._Dimensions  = Dimensions;

        }

        #endregion

    }

}
