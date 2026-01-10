/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Calendar 2.0 (the "License");
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Extension methods for calendar identifications.
    /// </summary>
    public static class CalendarIdExtensions
    {

        /// <summary>
        /// Indicates whether this calendar identification is null or empty.
        /// </summary>
        /// <param name="CalendarId">A calendar identification.</param>
        public static Boolean IsNullOrEmpty(this Calendar_Id? CalendarId)
            => !CalendarId.HasValue || CalendarId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this calendar identification is NOT null or empty.
        /// </summary>
        /// <param name="CalendarId">A calendar identification.</param>
        public static Boolean IsNotNullOrEmpty(this Calendar_Id? CalendarId)
            => CalendarId.HasValue && CalendarId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a calendar.
    /// CiString(36)
    /// </summary>
    public readonly struct Calendar_Id : IId<Calendar_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this calendar identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this calendar identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the calendar identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new calendar identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a calendar identification.</param>
        private Calendar_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random calendar identification.
        /// </summary>
        public static Calendar_Id NewRandom

            => Parse(Guid.NewGuid().ToString());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a calendar identification.
        /// </summary>
        /// <param name="Text">A text representation of a calendar identification.</param>
        public static Calendar_Id Parse(String Text)
        {

            if (TryParse(Text, out var calendarId))
                return calendarId;

            throw new ArgumentException($"Invalid text representation of a calendar identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a calendar identification.
        /// </summary>
        /// <param name="Text">A text representation of a calendar identification.</param>
        public static Calendar_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var calendarId))
                return calendarId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CalendarId)

        /// <summary>
        /// Try to parse the given text as a calendar identification.
        /// </summary>
        /// <param name="Text">A text representation of a calendar identification.</param>
        /// <param name="CalendarId">The parsed calendar identification.</param>
        public static Boolean TryParse(String Text, out Calendar_Id CalendarId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CalendarId = new Calendar_Id(Text);
                    return true;
                }
                catch
                { }
            }

            CalendarId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this calendar identification.
        /// </summary>
        public Calendar_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (CalendarId1, CalendarId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CalendarId1">A calendar identification.</param>
        /// <param name="CalendarId2">Another calendar identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Calendar_Id CalendarId1,
                                           Calendar_Id CalendarId2)

            => CalendarId1.Equals(CalendarId2);

        #endregion

        #region Operator != (CalendarId1, CalendarId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CalendarId1">A calendar identification.</param>
        /// <param name="CalendarId2">Another calendar identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Calendar_Id CalendarId1,
                                           Calendar_Id CalendarId2)

            => !CalendarId1.Equals(CalendarId2);

        #endregion

        #region Operator <  (CalendarId1, CalendarId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CalendarId1">A calendar identification.</param>
        /// <param name="CalendarId2">Another calendar identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Calendar_Id CalendarId1,
                                          Calendar_Id CalendarId2)

            => CalendarId1.CompareTo(CalendarId2) < 0;

        #endregion

        #region Operator <= (CalendarId1, CalendarId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CalendarId1">A calendar identification.</param>
        /// <param name="CalendarId2">Another calendar identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Calendar_Id CalendarId1,
                                           Calendar_Id CalendarId2)

            => CalendarId1.CompareTo(CalendarId2) <= 0;

        #endregion

        #region Operator >  (CalendarId1, CalendarId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CalendarId1">A calendar identification.</param>
        /// <param name="CalendarId2">Another calendar identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Calendar_Id CalendarId1,
                                          Calendar_Id CalendarId2)

            => CalendarId1.CompareTo(CalendarId2) > 0;

        #endregion

        #region Operator >= (CalendarId1, CalendarId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CalendarId1">A calendar identification.</param>
        /// <param name="CalendarId2">Another calendar identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Calendar_Id CalendarId1,
                                           Calendar_Id CalendarId2)

            => CalendarId1.CompareTo(CalendarId2) >= 0;

        #endregion

        #endregion

        #region IComparable<CalendarId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two calendar identifications.
        /// </summary>
        /// <param name="Object">A calendar identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Calendar_Id calendarId
                   ? CompareTo(calendarId)
                   : throw new ArgumentException("The given object is not a calendar identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CalendarId)

        /// <summary>
        /// Compares two calendar identifications.
        /// </summary>
        /// <param name="CalendarId">A calendar identification to compare with.</param>
        public Int32 CompareTo(Calendar_Id CalendarId)

            => String.Compare(InternalId,
                              CalendarId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CalendarId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two calendar identifications for equality.
        /// </summary>
        /// <param name="Object">A calendar identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Calendar_Id calendarId &&
                   Equals(calendarId);

        #endregion

        #region Equals(CalendarId)

        /// <summary>
        /// Compares two calendar identifications for equality.
        /// </summary>
        /// <param name="CalendarId">A calendar identification to compare with.</param>
        public Boolean Equals(Calendar_Id CalendarId)

            => String.Equals(InternalId,
                             CalendarId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
