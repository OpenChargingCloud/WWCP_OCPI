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
using System.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// Opening and access hours.
    /// </summary>
    public class Hours : IEquatable<Hours>
    {

        #region Data

        private readonly HashSet<RegularHours>       _RegularHours;
        private readonly HashSet<ExceptionalPeriod>  _ExceptionalOpenings;
        private readonly HashSet<ExceptionalPeriod>  _ExceptionalClosings;

        #endregion

        #region Properties

        /// <summary>
        /// True to represent 24 hours per day and 7 days per week, except the given exceptions.
        /// </summary>
        public Boolean                         IsTwentyFourSevenOpen    { get; }

        /// <summary>
        /// Regular hours, weekday based. Should not be set for representing 24/7 as this is the most common case.
        /// </summary>
        public IEnumerable<RegularHours>       RegularHours
            => _RegularHours;

        /// <summary>
        /// Exceptions for specified calendar dates, time-range based.
        /// Periods the station is operating/accessible.
        /// Additional to regular hours. May overlap regular rules.
        /// </summary>
        public IEnumerable<ExceptionalPeriod>  ExceptionalOpenings
            => _ExceptionalOpenings;

        /// <summary>
        /// Exceptions for specified calendar dates, time-range based.
        /// Periods the station is not operating/accessible.
        /// Overwriting regularHours and exceptionalOpenings.
        /// Should not overlap exceptionalOpenings.
        /// </summary>
        public IEnumerable<ExceptionalPeriod>  ExceptionalClosings
            => _ExceptionalClosings;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new hours class having regular hours, weekday based.
        /// Should not be used for representing 24/7 as this is the most common case.
        /// </summary>
        /// <param name="RegularHours">Regular hours, weekday based. Should not be set for representing 24/7 as this is the most common case.</param>
        /// <param name="ExceptionalOpenings">Exceptions for specified calendar dates, time-range based. Periods the station is operating/accessible. Additional to regular hours. May overlap regular rules.</param>
        /// <param name="ExceptionalClosings">Exceptions for specified calendar dates, time-range based. Periods the station is not operating/accessible. Overwriting regularHours and exceptionalOpenings. Should not overlap exceptionalOpenings.</param>
        public Hours(IEnumerable<RegularHours>       RegularHours          = null,
                     IEnumerable<ExceptionalPeriod>  ExceptionalOpenings   = null,
                     IEnumerable<ExceptionalPeriod>  ExceptionalClosings   = null)
        {

            this.IsTwentyFourSevenOpen  = false;
            this._RegularHours          = RegularHours.       SafeAny() ? new HashSet<RegularHours>     (RegularHours)        : new HashSet<RegularHours>();
            this._ExceptionalOpenings   = ExceptionalOpenings.SafeAny() ? new HashSet<ExceptionalPeriod>(ExceptionalOpenings) : new HashSet<ExceptionalPeriod>();
            this._ExceptionalClosings   = ExceptionalClosings.SafeAny() ? new HashSet<ExceptionalPeriod>(ExceptionalClosings) : new HashSet<ExceptionalPeriod>();

        }

        #endregion


        #region (static) TwentyFourSevenOpen(ExceptionalOpenings = null, ExceptionalClosings = null)

        /// <summary>
        /// Create a new hours class representing 24 hours per day and 7 days per week, except the given exceptions.
        /// </summary>
        /// <param name="ExceptionalOpenings">Exceptions for specified calendar dates, time-range based. Periods the station is operating/accessible. Additional to regular hours. May overlap regular rules.</param>
        /// <param name="ExceptionalClosings">Exceptions for specified calendar dates, time-range based. Periods the station is not operating/accessible. Overwriting regularHours and exceptionalOpenings. Should not overlap exceptionalOpenings.</param>
        public static Hours TwentyFourSevenOpen(IEnumerable<ExceptionalPeriod>  ExceptionalOpenings   = null,
                                                IEnumerable<ExceptionalPeriod>  ExceptionalClosings   = null)

            => new Hours(null,
                         ExceptionalOpenings,
                         ExceptionalClosings);

        #endregion


        #region Operator overloading

        #region Operator == (Hours1, Hours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hours1">A specification of opening and access hours.</param>
        /// <param name="Hours2">Another specification of opening and access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Hours Hours1,
                                           Hours Hours2)

            => Hours1.Equals(Hours2);

        #endregion

        #region Operator != (Hours1, Hours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hours1">A specification of opening and access hours.</param>
        /// <param name="Hours2">Another specification of opening and access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Hours Hours1,
                                           Hours Hours2)

            => !(Hours1 == Hours2);

        #endregion

        #endregion

        #region IEquatable<Hours> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Hours Hours &&
                   Equals(Hours);

        #endregion

        #region Equals(Hours)

        /// <summary>
        /// Compares two Hourss for equality.
        /// </summary>
        /// <param name="Hours">A Hours to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Hours Hours)

            => !(Hours is null)                                                                          &&
               IsTwentyFourSevenOpen.     Equals(Hours.IsTwentyFourSevenOpen)                            &&
               _RegularHours.       Count.Equals(Hours._RegularHours)                                    &&
               _ExceptionalOpenings.Count.Equals(Hours._ExceptionalOpenings)                             &&
               _ExceptionalClosings.Count.Equals(Hours._ExceptionalClosings)                             &&
               _RegularHours.       Any(regularHour => Hours._RegularHours.Contains(regularHour))        &&
               _ExceptionalOpenings.All(regularHour => Hours._ExceptionalOpenings.Contains(regularHour)) &&
               _ExceptionalClosings.All(regularHour => Hours._ExceptionalClosings.Contains(regularHour));

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
                return IsTwentyFourSevenOpen.GetHashCode() * 3 ^
                       _RegularHours.       Aggregate(0, (hashCode, dimension) => hashCode ^ dimension.GetHashCode()) ^
                       _ExceptionalOpenings.Aggregate(0, (hashCode, period)    => hashCode ^ period.   GetHashCode()) ^
                       _ExceptionalClosings.Aggregate(0, (hashCode, period)    => hashCode ^ period.   GetHashCode());
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(IsTwentyFourSevenOpen ? "24/7" : "...");

        #endregion


    }

}
