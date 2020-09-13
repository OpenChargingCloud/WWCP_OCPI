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
using System.Collections.Generic;
using System.Linq;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// A charging period consists of a start timestamp and a list of
    /// possible values that influence this period, for example:
    /// Amount of energy charged this period, maximum current during
    /// this period etc.
    /// </summary>
    public readonly struct ChargingPeriod : IEquatable<ChargingPeriod>,
                                            IComparable<ChargingPeriod>,
                                            IComparable
    {

        #region Properties

        /// <summary>
        /// Start timestamp of the charging period.
        /// This period ends when a next period starts,
        /// the last period ends when the session ends.
        /// </summary>
        public DateTime                   Start         { get; }

        /// <summary>
        /// List of relevant values for this charging period.
        /// </summary>
        public IEnumerable<CDRDimension>  Dimensions    { get; }

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

            if (!Dimensions.SafeAny())
                throw new ArgumentNullException(nameof(Dimensions), "The given enumeration of relevant values for this charging period must not be null or empty!");

            #endregion

            this.Start       = Start;
            this.Dimensions  = Dimensions.Distinct();

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingPeriod ChargingPeriod1,
                                           ChargingPeriod ChargingPeriod2)

            => ChargingPeriod1.Equals(ChargingPeriod2);

        #endregion

        #region Operator != (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingPeriod ChargingPeriod1,
                                           ChargingPeriod ChargingPeriod2)

            => !(ChargingPeriod1 == ChargingPeriod2);

        #endregion

        #region Operator <  (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingPeriod ChargingPeriod1,
                                          ChargingPeriod ChargingPeriod2)

            => ChargingPeriod1.CompareTo(ChargingPeriod2) < 0;

        #endregion

        #region Operator <= (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingPeriod ChargingPeriod1,
                                           ChargingPeriod ChargingPeriod2)

            => !(ChargingPeriod1 > ChargingPeriod2);

        #endregion

        #region Operator >  (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingPeriod ChargingPeriod1,
                                          ChargingPeriod ChargingPeriod2)

            => ChargingPeriod1.CompareTo(ChargingPeriod2) > 0;

        #endregion

        #region Operator >= (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingPeriod ChargingPeriod1,
                                           ChargingPeriod ChargingPeriod2)

            => !(ChargingPeriod1 < ChargingPeriod2);

        #endregion

        #endregion

        #region IComparable<ChargingPeriod> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is ChargingPeriod chargingPeriod
                   ? CompareTo(chargingPeriod)
                   : throw new ArgumentException("The given object is not a charging period!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingPeriod)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod">An object to compare with.</param>
        public Int32 CompareTo(ChargingPeriod ChargingPeriod)

            => Start.CompareTo(ChargingPeriod.Start);

        #endregion

        #endregion

        #region IEquatable<ChargingPeriod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ChargingPeriod ChargingPeriod &&
                   Equals(ChargingPeriod);

        #endregion

        #region Equals(ChargingPeriod)

        /// <summary>
        /// Compares two charging periods for equality.
        /// </summary>
        /// <param name="ChargingPeriod">A charging period to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargingPeriod ChargingPeriod)

            => Start.             Equals(ChargingPeriod.Start)              &&
               Dimensions.Count().Equals(ChargingPeriod.Dimensions.Count()) &&
               Dimensions.All(dimension => ChargingPeriod.Dimensions.Contains(dimension));

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
                return Start.GetHashCode() * 3 ^
                       Dimensions.Aggregate(0, (hashCode, dimension) => hashCode ^ dimension.GetHashCode());
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Start,
                             " -> ",
                             Dimensions.OrderBy(dimension => dimension.Type).
                                        AggregateWith(", "));

        #endregion

    }

}
