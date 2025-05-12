/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, BookingLocation 2.0 (the "License");
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
    /// Extension methods for booking location identifications.
    /// </summary>
    public static class BookingLocationIdExtensions
    {

        /// <summary>
        /// Indicates whether this booking location identification is null or empty.
        /// </summary>
        /// <param name="BookingLocationId">A booking location identification.</param>
        public static Boolean IsNullOrEmpty(this BookingLocation_Id? BookingLocationId)
            => !BookingLocationId.HasValue || BookingLocationId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this booking location identification is NOT null or empty.
        /// </summary>
        /// <param name="BookingLocationId">A booking location identification.</param>
        public static Boolean IsNotNullOrEmpty(this BookingLocation_Id? BookingLocationId)
            => BookingLocationId.HasValue && BookingLocationId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a booking location.
    /// CiString(36)
    /// </summary>
    public readonly struct BookingLocation_Id : IId<BookingLocation_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this booking location identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this booking location identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the booking location identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new booking location identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a booking location identification.</param>
        private BookingLocation_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random booking location identification.
        /// </summary>
        public static BookingLocation_Id NewRandom

            => Parse(Guid.NewGuid().ToString());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a booking location identification.
        /// </summary>
        /// <param name="Text">A text representation of a booking location identification.</param>
        public static BookingLocation_Id Parse(String Text)
        {

            if (TryParse(Text, out var bookingLocationId))
                return bookingLocationId;

            throw new ArgumentException($"Invalid text representation of a booking location identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a booking location identification.
        /// </summary>
        /// <param name="Text">A text representation of a booking location identification.</param>
        public static BookingLocation_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var bookingLocationId))
                return bookingLocationId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out BookingLocationId)

        /// <summary>
        /// Try to parse the given text as a booking location identification.
        /// </summary>
        /// <param name="Text">A text representation of a booking location identification.</param>
        /// <param name="BookingLocationId">The parsed booking location identification.</param>
        public static Boolean TryParse(String Text, out BookingLocation_Id BookingLocationId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    BookingLocationId = new BookingLocation_Id(Text);
                    return true;
                }
                catch
                { }
            }

            BookingLocationId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this booking location identification.
        /// </summary>
        public BookingLocation_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (BookingLocationId1, BookingLocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingLocationId1">A booking location identification.</param>
        /// <param name="BookingLocationId2">Another booking location identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BookingLocation_Id BookingLocationId1,
                                           BookingLocation_Id BookingLocationId2)

            => BookingLocationId1.Equals(BookingLocationId2);

        #endregion

        #region Operator != (BookingLocationId1, BookingLocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingLocationId1">A booking location identification.</param>
        /// <param name="BookingLocationId2">Another booking location identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BookingLocation_Id BookingLocationId1,
                                           BookingLocation_Id BookingLocationId2)

            => !BookingLocationId1.Equals(BookingLocationId2);

        #endregion

        #region Operator <  (BookingLocationId1, BookingLocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingLocationId1">A booking location identification.</param>
        /// <param name="BookingLocationId2">Another booking location identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BookingLocation_Id BookingLocationId1,
                                          BookingLocation_Id BookingLocationId2)

            => BookingLocationId1.CompareTo(BookingLocationId2) < 0;

        #endregion

        #region Operator <= (BookingLocationId1, BookingLocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingLocationId1">A booking location identification.</param>
        /// <param name="BookingLocationId2">Another booking location identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BookingLocation_Id BookingLocationId1,
                                           BookingLocation_Id BookingLocationId2)

            => BookingLocationId1.CompareTo(BookingLocationId2) <= 0;

        #endregion

        #region Operator >  (BookingLocationId1, BookingLocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingLocationId1">A booking location identification.</param>
        /// <param name="BookingLocationId2">Another booking location identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BookingLocation_Id BookingLocationId1,
                                          BookingLocation_Id BookingLocationId2)

            => BookingLocationId1.CompareTo(BookingLocationId2) > 0;

        #endregion

        #region Operator >= (BookingLocationId1, BookingLocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingLocationId1">A booking location identification.</param>
        /// <param name="BookingLocationId2">Another booking location identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BookingLocation_Id BookingLocationId1,
                                           BookingLocation_Id BookingLocationId2)

            => BookingLocationId1.CompareTo(BookingLocationId2) >= 0;

        #endregion

        #endregion

        #region IComparable<BookingLocationId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two booking location identifications.
        /// </summary>
        /// <param name="Object">A booking location identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is BookingLocation_Id bookingLocationId
                   ? CompareTo(bookingLocationId)
                   : throw new ArgumentException("The given object is not a booking location identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BookingLocationId)

        /// <summary>
        /// Compares two booking location identifications.
        /// </summary>
        /// <param name="BookingLocationId">A booking location identification to compare with.</param>
        public Int32 CompareTo(BookingLocation_Id BookingLocationId)

            => String.Compare(InternalId,
                              BookingLocationId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<BookingLocationId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two booking location identifications for equality.
        /// </summary>
        /// <param name="Object">A booking location identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BookingLocation_Id bookingLocationId &&
                   Equals(bookingLocationId);

        #endregion

        #region Equals(BookingLocationId)

        /// <summary>
        /// Compares two booking location identifications for equality.
        /// </summary>
        /// <param name="BookingLocationId">A booking location identification to compare with.</param>
        public Boolean Equals(BookingLocation_Id BookingLocationId)

            => String.Equals(InternalId,
                             BookingLocationId.InternalId,
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
