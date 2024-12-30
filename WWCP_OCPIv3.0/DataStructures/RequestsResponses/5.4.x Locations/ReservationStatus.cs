/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
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
    /// A reservation status.
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
        /// The Reservation has been canceled by the CPO.
        /// </summary>
        public static ReservationStatus  CANCELED_RESERVATION    { get; }
            = new ("CANCELED_RESERVATION");

        /// <summary>
        /// Reservation for the requested EVSE requires remote communication to a charging device and this remote communication link is not available.
        /// </summary>
        public static ReservationStatus  DEVICE_OFFLINE          { get; }
            = new ("DEVICE_OFFLINE");

        /// <summary>
        /// EVSE is currently occupied, another session is ongoing. Cannot start a new session.
        /// </summary>
        public static ReservationStatus  EVSE_OCCUPIED           { get; }
            = new ("EVSE_OCCUPIED");

        /// <summary>
        /// EVSE is currently inoperative or faulted.
        /// </summary>
        public static ReservationStatus  EVSE_INOPERATIVE        { get; }
            = new ("EVSE_INOPERATIVE");

        /// <summary>
        /// Reservations are not supported by the receiving Party or on the Location for which one was requested.
        /// </summary>
        public static ReservationStatus  NOT_SUPPORTED           { get; }
            = new ("NOT_SUPPORTED");

        /// <summary>
        /// The Reservation in the requested command is not known by the receiving Party.
        /// </summary>
        public static ReservationStatus  UNKNOWN_RESERVATION     { get; }
            = new ("UNKNOWN_RESERVATION");

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
