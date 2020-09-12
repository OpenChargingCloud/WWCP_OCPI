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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// Regular recurring operation or access hours.
    /// </summary>
    public readonly struct RegularHours : IEquatable<RegularHours>
    {

        #region Properties

        /// <summary>
        /// Day of the week, from Monday till Sunday.
        /// </summary>
        public DayOfWeek  Weekday    { get; }

        /// <summary>
        /// Begin of the regular period given in hours and minutes. Must be in 24h format.
        /// </summary>
        public HourMin    Begin      { get; }

        /// <summary>
        /// End of the regular period, syntax as for period_begin. Must be later than the begin.
        /// </summary>
        public HourMin    End        { get; }

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

            if (Begin >= End)
                throw new ArgumentException("Begin time must be before the end time!");

            #endregion

            this.Weekday  = Weekday;
            this.Begin    = Begin;
            this.End      = End;

        }

        #endregion


        #region Operator overloading

        #region Operator == (RegularHours1, RegularHours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegularHours1">A specification of a regular recurring operation or access hours.</param>
        /// <param name="RegularHours2">Another specification of a regular recurring operation or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RegularHours RegularHours1,
                                           RegularHours RegularHours2)

            => RegularHours1.Equals(RegularHours2);

        #endregion

        #region Operator != (RegularHours1, RegularHours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegularHours1">A specification of a regular recurring operation or access hours.</param>
        /// <param name="RegularHours2">Another specification of a regular recurring operation or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RegularHours RegularHours1,
                                           RegularHours RegularHours2)

            => !(RegularHours1 == RegularHours2);

        #endregion

        #endregion

        #region IEquatable<RegularHours> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is RegularHours RegularHours &&
                   Equals(RegularHours);

        #endregion

        #region Equals(RegularHours)

        /// <summary>
        /// Compares two RegularHourss for equality.
        /// </summary>
        /// <param name="RegularHours">A RegularHours to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(RegularHours RegularHours)

            => Weekday.Equals(RegularHours.Weekday) &&
               Begin.  Equals(RegularHours.Begin)   &&
               End.    Equals(RegularHours.End);

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
                return Weekday.GetHashCode() * 5 ^
                       Begin.  GetHashCode() * 3 ^
                       End.    GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Weekday.ToString(),
                             "s from ",
                             Begin.  ToString(),
                             " to ",
                             End.    ToString());

        #endregion

    }

}
