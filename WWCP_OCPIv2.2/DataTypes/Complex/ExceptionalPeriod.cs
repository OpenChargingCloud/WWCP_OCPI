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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// Specifies one exceptional period for opening or access hours.
    /// </summary>
    public readonly struct ExceptionalPeriod : IEquatable<ExceptionalPeriod>,
                                               IComparable<ExceptionalPeriod>,
                                               IComparable
    {

        #region Properties

        /// <summary>
        /// Begin of the opening or access hours exception.
        /// </summary>
        public DateTime Begin    { get; }

        /// <summary>
        /// End of the opening or access hours exception.
        /// </summary>
        public DateTime End      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new exceptional period for opening or access hours.
        /// </summary>
        /// <param name="Begin">Begin of the opening or access hours exception.</param>
        /// <param name="End">End of the opening or access hours exception.</param>
        public ExceptionalPeriod(DateTime  Begin,
                                 DateTime  End)
        {

            this.Begin  = Begin;
            this.End    = End;

        }

        #endregion


        public JObject ToJSON()
        {

            var JSON = JSONObject.Create(
                           new JProperty("period_begin", Begin.ToIso8601()),
                           new JProperty("period_end",   End.  ToIso8601())
                       );

            return JSON;

        }


        #region Operator overloading

        #region Operator == (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period for opening or access hours.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ExceptionalPeriod ExceptionalPeriod1,
                                           ExceptionalPeriod ExceptionalPeriod2)

            => ExceptionalPeriod1.Equals(ExceptionalPeriod2);

        #endregion

        #region Operator != (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period for opening or access hours.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ExceptionalPeriod ExceptionalPeriod1,
                                           ExceptionalPeriod ExceptionalPeriod2)

            => !(ExceptionalPeriod1 == ExceptionalPeriod2);

        #endregion

        #region Operator <  (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period for opening or access hours.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ExceptionalPeriod ExceptionalPeriod1,
                                          ExceptionalPeriod ExceptionalPeriod2)

            => ExceptionalPeriod1.CompareTo(ExceptionalPeriod2) < 0;

        #endregion

        #region Operator <= (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period for opening or access hours.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ExceptionalPeriod ExceptionalPeriod1,
                                           ExceptionalPeriod ExceptionalPeriod2)

            => !(ExceptionalPeriod1 > ExceptionalPeriod2);

        #endregion

        #region Operator >  (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period for opening or access hours.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ExceptionalPeriod ExceptionalPeriod1,
                                          ExceptionalPeriod ExceptionalPeriod2)

            => ExceptionalPeriod1.CompareTo(ExceptionalPeriod2) > 0;

        #endregion

        #region Operator >= (ExceptionalPeriod1, ExceptionalPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod1">An exceptional period for opening or access hours.</param>
        /// <param name="ExceptionalPeriod2">Another exceptional period for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ExceptionalPeriod ExceptionalPeriod1,
                                           ExceptionalPeriod ExceptionalPeriod2)

            => !(ExceptionalPeriod1 < ExceptionalPeriod2);

        #endregion

        #endregion

        #region IComparable<ExceptionalPeriod> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is ExceptionalPeriod exceptionalPeriod
                   ? CompareTo(exceptionalPeriod)
                   : throw new ArgumentException("The given object is not an exceptional period!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ExceptionalPeriod)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ExceptionalPeriod">An object to compare with.</param>
        public Int32 CompareTo(ExceptionalPeriod ExceptionalPeriod)
        {

            var c = Begin.CompareTo(ExceptionalPeriod.Begin);

            if (c == 0)
                c = End.CompareTo(ExceptionalPeriod.End);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ExceptionalPeriod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ExceptionalPeriod exceptionalPeriod &&
                   Equals(exceptionalPeriod);

        #endregion

        #region Equals(ExceptionalPeriod)

        /// <summary>
        /// Compares two ExceptionalPeriods for equality.
        /// </summary>
        /// <param name="ExceptionalPeriod">A ExceptionalPeriod to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ExceptionalPeriod ExceptionalPeriod)

            => Begin.Equals(ExceptionalPeriod.Begin) &&
               End.  Equals(ExceptionalPeriod.End);

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

                return Begin.GetHashCode() * 3 ^
                       End.  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Begin,
                             " - ",
                             End);

        #endregion

    }

}
