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
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.WWCP.OCPIv2_1
{

    /// <summary>
    /// Opening and access hours for the location.
    /// </summary>
    public class Hours
    {

        private readonly RegularHours[] _RegularHours;

        #region Properties

        #region IsTwentyFourSevenOpen

        private readonly Boolean _IsTwentyFourSevenOpen;

        /// <summary>
        /// True to represent 24 hours per day and 7 days per week, except the given exceptions.
        /// </summary>
        public Boolean IsTwentyFourSevenOpen
        {
            get
            {
                return _IsTwentyFourSevenOpen;
            }
        }

        #endregion

        #region RegularHours

        //private readonly RegularHours _RegularHours;

        ///// <summary>
        ///// Regular hours, weekday based. Should not be set for representing 24/7 as this is the most common case.
        ///// </summary>
        //public RegularHours RegularHours
        //{
        //    get
        //    {
        //        return _RegularHours;
        //    }
        //}

        #endregion

        #region ExceptionalOpenings

        private readonly ExceptionalPeriod _ExceptionalOpenings;

        /// <summary>
        /// Exceptions for specified calendar dates, time-range based.
        /// Periods the station is operating/accessible.
        /// Additional to regular hours. May overlap regular rules.
        /// </summary>
        public ExceptionalPeriod ExceptionalOpenings
        {
            get
            {
                return _ExceptionalOpenings;
            }
        }

        #endregion

        #region ExceptionalClosings

        private readonly ExceptionalPeriod _ExceptionalClosings;

        /// <summary>
        /// Exceptions for specified calendar dates, time-range based.
        /// Periods the station is not operating/accessible.
        /// Overwriting regularHours and exceptionalOpenings.
        /// Should not overlap exceptionalOpenings.
        /// </summary>
        public ExceptionalPeriod ExceptionalClosings
        {
            get
            {
                return _ExceptionalClosings;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region Hours(ExceptionalOpenings = null, ExceptionalClosings = null)

        /// <summary>
        /// Create a new hours class representing 24 hours per day and 7 days per week, except the given exceptions.
        /// </summary>
        /// <param name="ExceptionalOpenings">Exceptions for specified calendar dates, time-range based. Periods the station is operating/accessible. Additional to regular hours. May overlap regular rules.</param>
        /// <param name="ExceptionalClosings">Exceptions for specified calendar dates, time-range based. Periods the station is not operating/accessible. Overwriting regularHours and exceptionalOpenings. Should not overlap exceptionalOpenings.</param>
        private Hours(ExceptionalPeriod  ExceptionalOpenings = null,
                      ExceptionalPeriod  ExceptionalClosings = null)
        {

            this._IsTwentyFourSevenOpen  = true;
            this._ExceptionalOpenings    = ExceptionalOpenings;
            this._ExceptionalClosings    = ExceptionalClosings;

        }

        #endregion

        #region Hours(RegularHours, ExceptionalOpenings = null, ExceptionalClosings = null)

        /// <summary>
        /// Create a new hours class having regular hours, weekday based.
        /// Should not be used for representing 24/7 as this is the most common case.
        /// </summary>
        /// <param name="RegularHours">Regular hours, weekday based. Should not be set for representing 24/7 as this is the most common case.</param>
        /// <param name="ExceptionalOpenings">Exceptions for specified calendar dates, time-range based. Periods the station is operating/accessible. Additional to regular hours. May overlap regular rules.</param>
        /// <param name="ExceptionalClosings">Exceptions for specified calendar dates, time-range based. Periods the station is not operating/accessible. Overwriting regularHours and exceptionalOpenings. Should not overlap exceptionalOpenings.</param>
        public Hours()//RegularHours       RegularHours,
                     //ExceptionalPeriod  ExceptionalOpenings = null,
                     //ExceptionalPeriod  ExceptionalClosings = null)
        {

            this._IsTwentyFourSevenOpen  = false;
            this._RegularHours           = new RegularHours[7];
            this._ExceptionalOpenings    = ExceptionalOpenings;
            this._ExceptionalClosings    = ExceptionalClosings;

        }

        #endregion

        #endregion

        public Hours Set(DayOfWeek  Weekday,
                         HourMin    Begin,
                         HourMin    End)
        {

            _RegularHours[(int) Weekday] = new RegularHours(Weekday, Begin, End);

            return this;

        }



        #region (static) TwentyFourSevenOpen(ExceptionalOpenings = null, ExceptionalClosings = null)

        /// <summary>
        /// Create a new hours class representing 24 hours per day and 7 days per week, except the given exceptions.
        /// </summary>
        /// <param name="ExceptionalOpenings">Exceptions for specified calendar dates, time-range based. Periods the station is operating/accessible. Additional to regular hours. May overlap regular rules.</param>
        /// <param name="ExceptionalClosings">Exceptions for specified calendar dates, time-range based. Periods the station is not operating/accessible. Overwriting regularHours and exceptionalOpenings. Should not overlap exceptionalOpenings.</param>
        public static Hours TwentyFourSevenOpen(ExceptionalPeriod ExceptionalOpenings  = null,
                                                ExceptionalPeriod ExceptionalClosings  = null)
        {

            return new Hours(ExceptionalOpenings, ExceptionalClosings);

        }

        #endregion


        public override string ToString()
        {
            return base.ToString();
        }


    }

}
