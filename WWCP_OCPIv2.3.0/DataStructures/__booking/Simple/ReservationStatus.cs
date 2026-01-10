/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
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
    /// Extension methods for reservation status.
    /// </summary>
    public static class ReservationStatusExtensions
    {

        /// <summary>
        /// Indicates whether this reservation status is null or empty.
        /// </summary>
        /// <param name="ReservationStatus">A reservation status.</param>
        public static Boolean IsNullOrEmpty(this ReservationStatus? ReservationStatus)
            => !ReservationStatus.HasValue || ReservationStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this reservation status is NOT null or empty.
        /// </summary>
        /// <param name="ReservationStatus">A reservation status.</param>
        public static Boolean IsNotNullOrEmpty(this ReservationStatus? ReservationStatus)
            => ReservationStatus.HasValue && ReservationStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The reservation status.
    /// </summary>
    public readonly struct ReservationStatus : IId<ReservationStatus>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this reservation status is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this reservation status is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the reservation status.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new reservation status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a reservation status.</param>
        private ReservationStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a reservation status.
        /// </summary>
        /// <param name="Text">A text representation of a reservation status.</param>
        public static ReservationStatus Parse(String Text)
        {

            if (TryParse(Text, out var reservationStatus))
                return reservationStatus;

            throw new ArgumentException($"Invalid text representation of a reservation status: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a reservation status.
        /// </summary>
        /// <param name="Text">A text representation of a reservation status.</param>
        public static ReservationStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var reservationStatus))
                return reservationStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ReservationStatus)

        /// <summary>
        /// Try to parse the given text as a reservation status.
        /// </summary>
        /// <param name="Text">A text representation of a reservation status.</param>
        /// <param name="ReservationStatus">The parsed reservation status.</param>
        public static Boolean TryParse(String Text, out ReservationStatus ReservationStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ReservationStatus = new ReservationStatus(Text);
                    return true;
                }
                catch
                { }
            }

            ReservationStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this reservation status.
        /// </summary>
        public ReservationStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Booking request pending processing by the CPO.
        /// </summary>
        public static ReservationStatus  PENDING      { get; }
            = new ("PENDING");

        /// <summary>
        /// Booking request accepted by the CPO.
        /// </summary>
        public static ReservationStatus  RESERVED     { get; }
            = new ("RESERVED");

        /// <summary>
        /// Booking canceled.
        /// </summary>
        public static ReservationStatus  CANCELED     { get; }
            = new ("CANCELED");

        /// <summary>
        /// Request for booking failed (error).
        /// </summary>
        public static ReservationStatus  FAILED       { get; }
            = new ("FAILED");

        /// <summary>
        /// Booking was not fulfilled because no one showed up, within start time found in the booking terms.
        /// </summary>
        public static ReservationStatus  NO_SHOW      { get; }
            = new ("NO_SHOW");

        /// <summary>
        /// The Booking is fulfilled, fulfilled means that the session is started with the communicated token before the expiry moment has passed.
        /// </summary>
        public static ReservationStatus  FULFILLED    { get; }
            = new ("FULFILLED");

        /// <summary>
        /// Booking req is rejected after processing by the CPO (e.g., requested time slot unavailable).
        /// </summary>
        public static ReservationStatus  REJECTED     { get; }
            = new ("REJECTED");

        /// <summary>
        /// Any other status / unknown status.
        /// </summary>
        public static ReservationStatus  UNKNOWN      { get; }
            = new ("UNKNOWN");

        #endregion


        #region Operator overloading

        #region Operator == (ReservationStatus1, ReservationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationStatus1">A reservation status.</param>
        /// <param name="ReservationStatus2">Another reservation status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ReservationStatus ReservationStatus1,
                                           ReservationStatus ReservationStatus2)

            => ReservationStatus1.Equals(ReservationStatus2);

        #endregion

        #region Operator != (ReservationStatus1, ReservationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationStatus1">A reservation status.</param>
        /// <param name="ReservationStatus2">Another reservation status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ReservationStatus ReservationStatus1,
                                           ReservationStatus ReservationStatus2)

            => !ReservationStatus1.Equals(ReservationStatus2);

        #endregion

        #region Operator <  (ReservationStatus1, ReservationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationStatus1">A reservation status.</param>
        /// <param name="ReservationStatus2">Another reservation status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ReservationStatus ReservationStatus1,
                                          ReservationStatus ReservationStatus2)

            => ReservationStatus1.CompareTo(ReservationStatus2) < 0;

        #endregion

        #region Operator <= (ReservationStatus1, ReservationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationStatus1">A reservation status.</param>
        /// <param name="ReservationStatus2">Another reservation status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ReservationStatus ReservationStatus1,
                                           ReservationStatus ReservationStatus2)

            => ReservationStatus1.CompareTo(ReservationStatus2) <= 0;

        #endregion

        #region Operator >  (ReservationStatus1, ReservationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationStatus1">A reservation status.</param>
        /// <param name="ReservationStatus2">Another reservation status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ReservationStatus ReservationStatus1,
                                          ReservationStatus ReservationStatus2)

            => ReservationStatus1.CompareTo(ReservationStatus2) > 0;

        #endregion

        #region Operator >= (ReservationStatus1, ReservationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationStatus1">A reservation status.</param>
        /// <param name="ReservationStatus2">Another reservation status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ReservationStatus ReservationStatus1,
                                           ReservationStatus ReservationStatus2)

            => ReservationStatus1.CompareTo(ReservationStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<ReservationStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two reservation status.
        /// </summary>
        /// <param name="Object">A reservation status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ReservationStatus reservationStatus
                   ? CompareTo(reservationStatus)
                   : throw new ArgumentException("The given object is not a reservation status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ReservationStatus)

        /// <summary>
        /// Compares two reservation status.
        /// </summary>
        /// <param name="ReservationStatus">A reservation status to compare with.</param>
        public Int32 CompareTo(ReservationStatus ReservationStatus)

            => String.Compare(InternalId,
                              ReservationStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ReservationStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two reservation status for equality.
        /// </summary>
        /// <param name="Object">A reservation status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReservationStatus reservationStatus &&
                   Equals(reservationStatus);

        #endregion

        #region Equals(ReservationStatus)

        /// <summary>
        /// Compares two reservation status for equality.
        /// </summary>
        /// <param name="ReservationStatus">A reservation status to compare with.</param>
        public Boolean Equals(ReservationStatus ReservationStatus)

            => String.Equals(InternalId,
                             ReservationStatus.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToUpper().GetHashCode() ?? 0;

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
