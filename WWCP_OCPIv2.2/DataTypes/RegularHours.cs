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

#region Usings

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPIv2_2
{

    /// <summary>
    /// Regular recurring operation or access hours.
    /// </summary>
    public struct RegularHours : IEquatable<RegularHours>
    {

        #region Properties

        #region Weekday

        private readonly DayOfWeek _Weekday;

        /// <summary>
        /// Day of the week, from Monday till Sunday.
        /// </summary>
        public DayOfWeek Weekday
        {
            get
            {
                return _Weekday;
            }
        }

        #endregion

        #region Begin

        private readonly HourMin _Begin;

        /// <summary>
        /// Begin of the regular period given in hours and minutes. Must be in 24h format.
        /// </summary>
        public HourMin Begin
        {
            get
            {
                return _Begin;
            }
        }

        #endregion

        #region End

        private readonly HourMin _End;

        /// <summary>
        /// End of the regular period, syntax as for period_begin. Must be later than the begin.
        /// </summary>
        public HourMin End
        {
            get
            {
                return _End;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new specification of a regular recurring operation or access hours.
        /// </summary>
        /// <param name="Weekday">Day of the week, from Monday till Sunday.</param>
        /// <param name="Begin">Begin of the regular period given in hours and minutes. Must be in 24h format.</param>
        /// <param name="End">End of the regular period, syntax as for period_begin. Must be later than the begin.</param>
        public RegularHours(DayOfWeek  Weekday,
                            HourMin    Begin,
                            HourMin    End)
        {

            #region Initial checks

            if (Begin > End)
                throw new ArgumentException("Begin time must be before the end time!");

            #endregion

            this._Weekday  = Weekday;
            this._Begin    = Begin;
            this._End      = End;

        }

        #endregion


        #region IEquatable<RegularHours> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is an RegularHours.
            if (!(Object is RegularHours))
                return false;

            return this.Equals((RegularHours) Object);

        }

        #endregion

        #region Equals(RegularHours)

        /// <summary>
        /// Compares two RegularHourss for equality.
        /// </summary>
        /// <param name="RegularHours">A RegularHours to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(RegularHours RegularHours)
        {

            if ((Object) RegularHours == null)
                return false;

            return _Weekday.Equals(RegularHours._Weekday) &&
                   _Begin.  Equals(RegularHours._Begin)   &&
                   _End.    Equals(RegularHours._End);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                return _Weekday.GetHashCode() * 23 ^ _Begin.GetHashCode() * 17 ^ _End.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {

            if (_Weekday == DayOfWeek.Sunday && _Begin.Hour == 0 && _Begin.Minute == 0 && _End.Hour == 0 && _End.Minute == 0)
                return "";

            return String.Concat(_Weekday.ToString(), "s from ", _Begin.ToString(), " to ", _End.ToString());

        }

        #endregion

    }

}
