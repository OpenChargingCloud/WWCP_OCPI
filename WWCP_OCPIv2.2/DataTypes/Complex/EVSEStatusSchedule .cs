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
    public readonly struct EVSEStatusSchedule : IEquatable<EVSEStatusSchedule>,
                                                IComparable<EVSEStatusSchedule>,
                                                IComparable
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


        #region Operator overloading

        #region Operator == (EVSEStatusSchedule1, EVSEStatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatusSchedule1">An EVSE status schedule.</param>
        /// <param name="EVSEStatusSchedule2">Another EVSE status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVSEStatusSchedule EVSEStatusSchedule1,
                                           EVSEStatusSchedule EVSEStatusSchedule2)

            => EVSEStatusSchedule1.Equals(EVSEStatusSchedule2);

        #endregion

        #region Operator != (EVSEStatusSchedule1, EVSEStatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatusSchedule1">An EVSE status schedule.</param>
        /// <param name="EVSEStatusSchedule2">Another EVSE status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVSEStatusSchedule EVSEStatusSchedule1,
                                           EVSEStatusSchedule EVSEStatusSchedule2)

            => !(EVSEStatusSchedule1 == EVSEStatusSchedule2);

        #endregion

        #region Operator <  (EVSEStatusSchedule1, EVSEStatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatusSchedule1">An EVSE status schedule.</param>
        /// <param name="EVSEStatusSchedule2">Another EVSE status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EVSEStatusSchedule EVSEStatusSchedule1,
                                          EVSEStatusSchedule EVSEStatusSchedule2)

            => EVSEStatusSchedule1.CompareTo(EVSEStatusSchedule2) < 0;

        #endregion

        #region Operator <= (EVSEStatusSchedule1, EVSEStatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatusSchedule1">An EVSE status schedule.</param>
        /// <param name="EVSEStatusSchedule2">Another EVSE status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EVSEStatusSchedule EVSEStatusSchedule1,
                                           EVSEStatusSchedule EVSEStatusSchedule2)

            => !(EVSEStatusSchedule1 > EVSEStatusSchedule2);

        #endregion

        #region Operator >  (EVSEStatusSchedule1, EVSEStatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatusSchedule1">An EVSE status schedule.</param>
        /// <param name="EVSEStatusSchedule2">Another EVSE status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EVSEStatusSchedule EVSEStatusSchedule1,
                                          EVSEStatusSchedule EVSEStatusSchedule2)

            => EVSEStatusSchedule1.CompareTo(EVSEStatusSchedule2) > 0;

        #endregion

        #region Operator >= (EVSEStatusSchedule1, EVSEStatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatusSchedule1">An EVSE status schedule.</param>
        /// <param name="EVSEStatusSchedule2">Another EVSE status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EVSEStatusSchedule EVSEStatusSchedule1,
                                           EVSEStatusSchedule EVSEStatusSchedule2)

            => !(EVSEStatusSchedule1 < EVSEStatusSchedule2);

        #endregion

        #endregion

        #region IComparable<EVSEStatusSchedule> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is EVSEStatusSchedule EVSEStatusSchedule
                   ? CompareTo(EVSEStatusSchedule)
                   : throw new ArgumentException("The given object is not a EVSE status schedule!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EVSEStatusSchedule)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatusSchedule">An object to compare with.</param>
        public Int32 CompareTo(EVSEStatusSchedule EVSEStatusSchedule)
        {

            var c = Begin.     CompareTo(EVSEStatusSchedule.Begin);

            if (c == 0)
                c = EVSEStatus.CompareTo(EVSEStatusSchedule.EVSEStatus);

            if (c == 0)
                c = End.HasValue && EVSEStatusSchedule.End.HasValue
                        ? End.Value.CompareTo(EVSEStatusSchedule.End.Value)
                        : 0;

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EVSEStatusSchedule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is EVSEStatusSchedule EVSEStatusSchedule &&
                   Equals(EVSEStatusSchedule);

        #endregion

        #region Equals(EVSEStatusSchedule)

        /// <summary>
        /// Compares two EVSE status schedules for equality.
        /// </summary>
        /// <param name="EVSEStatusSchedule">An EVSE status schedule to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EVSEStatusSchedule EVSEStatusSchedule)

            => Begin.     Equals(EVSEStatusSchedule.Begin)      &&
               EVSEStatus.Equals(EVSEStatusSchedule.EVSEStatus) &&

               ((!End.HasValue && !EVSEStatusSchedule.End.HasValue) ||
                 (End.HasValue &&  EVSEStatusSchedule.End.HasValue && End.Value.Equals(EVSEStatusSchedule.End.Value)));

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

                return Begin.     GetHashCode() * 5 ^
                       EVSEStatus.GetHashCode() * 3 ^

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
                             EVSEStatus);

        #endregion

    }

}
