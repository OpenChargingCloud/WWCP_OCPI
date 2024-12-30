/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Reservation 2.0 (the "License");
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for reservation identifications.
    /// </summary>
    public static class ReservationIdExtensions
    {

        /// <summary>
        /// Indicates whether this reservation identification is null or empty.
        /// </summary>
        /// <param name="ReservationId">A reservation identification.</param>
        public static Boolean IsNullOrEmpty(this Reservation_Id? ReservationId)
            => !ReservationId.HasValue || ReservationId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this reservation identification is NOT null or empty.
        /// </summary>
        /// <param name="ReservationId">A reservation identification.</param>
        public static Boolean IsNotNullOrEmpty(this Reservation_Id? ReservationId)
            => ReservationId.HasValue && ReservationId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a reservation.
    /// CiString(36)
    /// </summary>
    public readonly struct Reservation_Id : IId<Reservation_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this reservation identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this reservation identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the reservation identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new reservation identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a reservation identification.</param>
        private Reservation_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random reservation identification.
        /// </summary>
        public static Reservation_Id NewRandom

            => Parse(Guid.NewGuid().ToString());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a reservation identification.
        /// </summary>
        /// <param name="Text">A text representation of a reservation identification.</param>
        public static Reservation_Id Parse(String Text)
        {

            if (TryParse(Text, out var reservationId))
                return reservationId;

            throw new ArgumentException($"Invalid text representation of a reservation identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a reservation identification.
        /// </summary>
        /// <param name="Text">A text representation of a reservation identification.</param>
        public static Reservation_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var reservationId))
                return reservationId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ReservationId)

        /// <summary>
        /// Try to parse the given text as a reservation identification.
        /// </summary>
        /// <param name="Text">A text representation of a reservation identification.</param>
        /// <param name="ReservationId">The parsed reservation identification.</param>
        public static Boolean TryParse(String Text, out Reservation_Id ReservationId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ReservationId = new Reservation_Id(Text);
                    return true;
                }
                catch
                { }
            }

            ReservationId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this reservation identification.
        /// </summary>
        public Reservation_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">A reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Reservation_Id ReservationId1,
                                           Reservation_Id ReservationId2)

            => ReservationId1.Equals(ReservationId2);

        #endregion

        #region Operator != (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">A reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Reservation_Id ReservationId1,
                                           Reservation_Id ReservationId2)

            => !ReservationId1.Equals(ReservationId2);

        #endregion

        #region Operator <  (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">A reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Reservation_Id ReservationId1,
                                          Reservation_Id ReservationId2)

            => ReservationId1.CompareTo(ReservationId2) < 0;

        #endregion

        #region Operator <= (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">A reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Reservation_Id ReservationId1,
                                           Reservation_Id ReservationId2)

            => ReservationId1.CompareTo(ReservationId2) <= 0;

        #endregion

        #region Operator >  (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">A reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Reservation_Id ReservationId1,
                                          Reservation_Id ReservationId2)

            => ReservationId1.CompareTo(ReservationId2) > 0;

        #endregion

        #region Operator >= (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">A reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Reservation_Id ReservationId1,
                                           Reservation_Id ReservationId2)

            => ReservationId1.CompareTo(ReservationId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ReservationId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two reservation identifications.
        /// </summary>
        /// <param name="Object">A reservation identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Reservation_Id reservationId
                   ? CompareTo(reservationId)
                   : throw new ArgumentException("The given object is not a reservation identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ReservationId)

        /// <summary>
        /// Compares two reservation identifications.
        /// </summary>
        /// <param name="ReservationId">A reservation identification to compare with.</param>
        public Int32 CompareTo(Reservation_Id ReservationId)

            => String.Compare(InternalId,
                              ReservationId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ReservationId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two reservation identifications for equality.
        /// </summary>
        /// <param name="Object">A reservation identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Reservation_Id reservationId &&
                   Equals(reservationId);

        #endregion

        #region Equals(ReservationId)

        /// <summary>
        /// Compares two reservation identifications for equality.
        /// </summary>
        /// <param name="ReservationId">A reservation identification to compare with.</param>
        public Boolean Equals(Reservation_Id ReservationId)

            => String.Equals(InternalId,
                             ReservationId.InternalId,
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
