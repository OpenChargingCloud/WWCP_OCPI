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
    /// This type is used to schedule EVSE/connector status periods in the future.
    /// </summary>
    public readonly struct StatusSchedule : IEquatable<StatusSchedule>,
                                            IComparable<StatusSchedule>,
                                            IComparable
    {

        #region Properties

        /// <summary>
        /// Begin of the scheduled period.
        /// </summary>
        [Mandatory]
        public DateTime     Begin     { get; }

        /// <summary>
        /// Optional end of the scheduled period.
        /// </summary>
        [Optional]
        public DateTime?    End       { get; }

        /// <summary>
        /// EVSE status value during the scheduled period.
        /// </summary>
        [Mandatory]
        public StatusTypes  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// This type is used to schedule EVSE status periods in the future.
        /// </summary>
        /// <param name="Status">EVSE status value during the scheduled period.</param>
        /// <param name="Begin">Begin of the scheduled period.</param>
        /// <param name="End">Optional end of the scheduled period.</param>
        public StatusSchedule(StatusTypes  Status,
                              DateTime     Begin,
                              DateTime?    End   = null)
        {

            this.Status  = Status;
            this.Begin   = Begin;
            this.End     = End;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StatusSchedule1, StatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule1">A status schedule.</param>
        /// <param name="StatusSchedule2">Another status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StatusSchedule StatusSchedule1,
                                           StatusSchedule StatusSchedule2)

            => StatusSchedule1.Equals(StatusSchedule2);

        #endregion

        #region Operator != (StatusSchedule1, StatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule1">A status schedule.</param>
        /// <param name="StatusSchedule2">Another status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (StatusSchedule StatusSchedule1,
                                           StatusSchedule StatusSchedule2)

            => !(StatusSchedule1 == StatusSchedule2);

        #endregion

        #region Operator <  (StatusSchedule1, StatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule1">A status schedule.</param>
        /// <param name="StatusSchedule2">Another status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (StatusSchedule StatusSchedule1,
                                          StatusSchedule StatusSchedule2)

            => StatusSchedule1.CompareTo(StatusSchedule2) < 0;

        #endregion

        #region Operator <= (StatusSchedule1, StatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule1">A status schedule.</param>
        /// <param name="StatusSchedule2">Another status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (StatusSchedule StatusSchedule1,
                                           StatusSchedule StatusSchedule2)

            => !(StatusSchedule1 > StatusSchedule2);

        #endregion

        #region Operator >  (StatusSchedule1, StatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule1">A status schedule.</param>
        /// <param name="StatusSchedule2">Another status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (StatusSchedule StatusSchedule1,
                                          StatusSchedule StatusSchedule2)

            => StatusSchedule1.CompareTo(StatusSchedule2) > 0;

        #endregion

        #region Operator >= (StatusSchedule1, StatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule1">A status schedule.</param>
        /// <param name="StatusSchedule2">Another status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (StatusSchedule StatusSchedule1,
                                           StatusSchedule StatusSchedule2)

            => !(StatusSchedule1 < StatusSchedule2);

        #endregion

        #endregion

        #region IComparable<StatusSchedule> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is StatusSchedule statusSchedule
                   ? CompareTo(statusSchedule)
                   : throw new ArgumentException("The given object is not a status schedule!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(StatusSchedule)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule">An object to compare with.</param>
        public Int32 CompareTo(StatusSchedule StatusSchedule)
        {

            var c = Begin. CompareTo(StatusSchedule.Begin);

            if (c == 0)
                c = Status.CompareTo(StatusSchedule.Status);

            if (c == 0)
                c = End.HasValue && StatusSchedule.End.HasValue
                        ? End.Value.CompareTo(StatusSchedule.End.Value)
                        : 0;

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<StatusSchedule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is StatusSchedule statusSchedule &&
                   Equals(statusSchedule);

        #endregion

        #region Equals(StatusSchedule)

        /// <summary>
        /// Compares two status schedules for equality.
        /// </summary>
        /// <param name="StatusSchedule">A status schedule to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(StatusSchedule StatusSchedule)

            => Begin. Equals(StatusSchedule.Begin)  &&
               Status.Equals(StatusSchedule.Status) &&

               ((!End.HasValue && !StatusSchedule.End.HasValue) ||
                 (End.HasValue &&  StatusSchedule.End.HasValue && End.Value.Equals(StatusSchedule.End.Value)));

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

                return Begin. GetHashCode() * 5 ^
                       Status.GetHashCode() * 3 ^

                       (End.HasValue
                            ? End.Value.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Begin,
                             " -> ",
                             End.HasValue
                                 ? End.Value.ToString()
                                 : "...",
                             " : ",
                             Status);

        #endregion

    }

}
