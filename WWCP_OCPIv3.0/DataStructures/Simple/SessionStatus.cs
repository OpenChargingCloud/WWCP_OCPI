/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extension methods for session statuss.
    /// </summary>
    public static class SessionStatusExtensions
    {

        /// <summary>
        /// Indicates whether this session status is null or empty.
        /// </summary>
        /// <param name="SessionStatus">A session status.</param>
        public static Boolean IsNullOrEmpty(this SessionStatus? SessionStatus)
            => !SessionStatus.HasValue || SessionStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this session status is NOT null or empty.
        /// </summary>
        /// <param name="SessionStatus">A session status.</param>
        public static Boolean IsNotNullOrEmpty(this SessionStatus? SessionStatus)
            => SessionStatus.HasValue && SessionStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a session status.
    /// </summary>
    public readonly struct SessionStatus : IId<SessionStatus>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this session status is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this session status is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the session status.
        /// </summary>
        public UInt64 Length
            => (UInt64)InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new session status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a session status.</param>
        private SessionStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a session status.
        /// </summary>
        /// <param name="Text">A text representation of a session status.</param>
        public static SessionStatus Parse(String Text)
        {

            if (TryParse(Text, out var sessionStatus))
                return sessionStatus;

            throw new ArgumentException($"Invalid text representation of a session status: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a session status.
        /// </summary>
        /// <param name="Text">A text representation of a session status.</param>
        public static SessionStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var sessionStatus))
                return sessionStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out SessionStatus)

        /// <summary>
        /// Try to parse the given text as a session status.
        /// </summary>
        /// <param name="Text">A text representation of a session status.</param>
        /// <param name="SessionStatus">The parsed session status.</param>
        public static Boolean TryParse(String Text, out SessionStatus SessionStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    SessionStatus = new SessionStatus(Text);
                    return true;
                }
                catch
                { }
            }

            SessionStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this session status.
        /// </summary>
        public SessionStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The session has been accepted and is active. All pre-conditions were met:
        /// Communication between EV and EVSE (for example: cable plugged in correctly),
        /// EV or driver is authorized. EV is being charged, or can be charged.
        /// Energy is, or is not, being transfered.
        /// </summary>
        public static SessionStatus  ACTIVE         { get; }
            = new ("ACTIVE");

        /// <summary>
        /// The session has been finished successfully.
        /// No more modifications will be made to the session object using this state.
        /// </summary>
        public static SessionStatus  COMPLETED      { get; }
            = new ("COMPLETED");

        /// <summary>
        /// The session object using this state is declared invalid and will not be billed.
        /// </summary>
        public static SessionStatus  INVALID        { get; }
            = new ("INVALID");

        /// <summary>
        /// The session is pending, it has not yet started. Not all pre-conditions are met.
        /// This is the initial state. The session might never become an _active_ session.
        /// </summary>
        public static SessionStatus  PENDING        { get; }
            = new ("PENDING");

        /// <summary>
        /// The session is started due to a reservation, charging has not yet started.
        /// The session might never become an active session.
        /// </summary>
        public static SessionStatus  RESERVATION    { get; }
            = new ("RESERVATION");

        #endregion


        #region Operator overloading

        #region Operator == (SessionStatus1, SessionStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionStatus1">A session status.</param>
        /// <param name="SessionStatus2">Another session status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator ==(SessionStatus SessionStatus1,
                                           SessionStatus SessionStatus2)

            => SessionStatus1.Equals(SessionStatus2);

        #endregion

        #region Operator != (SessionStatus1, SessionStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionStatus1">A session status.</param>
        /// <param name="SessionStatus2">Another session status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator !=(SessionStatus SessionStatus1,
                                           SessionStatus SessionStatus2)

            => !SessionStatus1.Equals(SessionStatus2);

        #endregion

        #region Operator <  (SessionStatus1, SessionStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionStatus1">A session status.</param>
        /// <param name="SessionStatus2">Another session status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <(SessionStatus SessionStatus1,
                                          SessionStatus SessionStatus2)

            => SessionStatus1.CompareTo(SessionStatus2) < 0;

        #endregion

        #region Operator <= (SessionStatus1, SessionStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionStatus1">A session status.</param>
        /// <param name="SessionStatus2">Another session status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <=(SessionStatus SessionStatus1,
                                           SessionStatus SessionStatus2)

            => SessionStatus1.CompareTo(SessionStatus2) <= 0;

        #endregion

        #region Operator >  (SessionStatus1, SessionStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionStatus1">A session status.</param>
        /// <param name="SessionStatus2">Another session status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >(SessionStatus SessionStatus1,
                                          SessionStatus SessionStatus2)

            => SessionStatus1.CompareTo(SessionStatus2) > 0;

        #endregion

        #region Operator >= (SessionStatus1, SessionStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionStatus1">A session status.</param>
        /// <param name="SessionStatus2">Another session status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >=(SessionStatus SessionStatus1,
                                           SessionStatus SessionStatus2)

            => SessionStatus1.CompareTo(SessionStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<SessionStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two session statuss.
        /// </summary>
        /// <param name="Object">A session status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SessionStatus sessionStatus
                   ? CompareTo(sessionStatus)
                   : throw new ArgumentException("The given object is not a session status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SessionStatus)

        /// <summary>
        /// Compares two session statuss.
        /// </summary>
        /// <param name="SessionStatus">A session status to compare with.</param>
        public Int32 CompareTo(SessionStatus SessionStatus)

            => String.Compare(InternalId,
                              SessionStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<SessionStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two session statuss for equality.
        /// </summary>
        /// <param name="Object">A session status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SessionStatus sessionStatus &&
                   Equals(sessionStatus);

        #endregion

        #region Equals(SessionStatus)

        /// <summary>
        /// Compares two session statuss for equality.
        /// </summary>
        /// <param name="SessionStatus">A session status to compare with.</param>
        public Boolean Equals(SessionStatus SessionStatus)

            => String.Equals(InternalId,
                             SessionStatus.InternalId,
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
