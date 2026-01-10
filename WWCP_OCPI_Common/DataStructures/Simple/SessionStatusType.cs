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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for SessionStatusTypes.
    /// </summary>
    public static class SessionStatusTypeExtensions
    {

        /// <summary>
        /// Indicates whether this SessionStatusType is null or empty.
        /// </summary>
        /// <param name="SessionStatusType">A SessionStatusType.</param>
        public static Boolean IsNullOrEmpty(this SessionStatusType? SessionStatusType)
            => !SessionStatusType.HasValue || SessionStatusType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this SessionStatusType is NOT null or empty.
        /// </summary>
        /// <param name="SessionStatusType">A SessionStatusType.</param>
        public static Boolean IsNotNullOrEmpty(this SessionStatusType? SessionStatusType)
            => SessionStatusType.HasValue && SessionStatusType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A SessionStatusType.
    /// </summary>
    public readonly struct SessionStatusType : IId<SessionStatusType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this SessionStatusType is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this SessionStatusType is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the SessionStatusType.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SessionStatusType based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a SessionStatusType.</param>
        private SessionStatusType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a SessionStatusType.
        /// </summary>
        /// <param name="Text">A text representation of a SessionStatusType.</param>
        public static SessionStatusType Parse(String Text)
        {

            if (TryParse(Text, out var sessionStatusType))
                return sessionStatusType;

            throw new ArgumentException($"Invalid text representation of a SessionStatusType: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a SessionStatusType.
        /// </summary>
        /// <param name="Text">A text representation of a SessionStatusType.</param>
        public static SessionStatusType? TryParse(String Text)
        {

            if (TryParse(Text, out var sessionStatusType))
                return sessionStatusType;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out SessionStatusType)

        /// <summary>
        /// Try to parse the given text as a SessionStatusType.
        /// </summary>
        /// <param name="Text">A text representation of a SessionStatusType.</param>
        /// <param name="SessionStatusType">The parsed SessionStatusType.</param>
        public static Boolean TryParse(String Text, out SessionStatusType SessionStatusType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    SessionStatusType = new SessionStatusType(Text);
                    return true;
                }
                catch
                { }
            }

            SessionStatusType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this SessionStatusType.
        /// </summary>
        public SessionStatusType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The session is accepted and active.
        /// </summary>
        public static SessionStatusType  ACTIVE         { get; }
            = new ("ACTIVE");

        /// <summary>
        /// The session has been finished successfully.
        /// No more modifications will be made to the session using this state.
        /// </summary>
        public static SessionStatusType  COMPLETED      { get; }
            = new ("COMPLETED");

        /// <summary>
        /// The session using this state is declared invalid and will not be billed.
        /// </summary>
        public static SessionStatusType  INVALID        { get; }
            = new ("INVALID");

        /// <summary>
        /// The session is pending, it has not yet started. Not all pre-conditions are met.
        /// This is the initial state. The session might never become an active session.
        /// </summary>
        public static SessionStatusType  PENDING        { get; }
            = new ("PENDING");

        /// <summary>
        /// The session is started due to a reservation, charging has not yet started.
        /// The session might never become an active session.
        /// </summary>
        public static SessionStatusType  RESERVATION    { get; }
            = new ("RESERVATION");

        #endregion


        #region Operator overloading

        #region Operator == (SessionStatusType1, SessionStatusType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionStatusType1">A SessionStatusType.</param>
        /// <param name="SessionStatusType2">Another SessionStatusType.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SessionStatusType SessionStatusType1,
                                           SessionStatusType SessionStatusType2)

            => SessionStatusType1.Equals(SessionStatusType2);

        #endregion

        #region Operator != (SessionStatusType1, SessionStatusType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionStatusType1">A SessionStatusType.</param>
        /// <param name="SessionStatusType2">Another SessionStatusType.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SessionStatusType SessionStatusType1,
                                           SessionStatusType SessionStatusType2)

            => !SessionStatusType1.Equals(SessionStatusType2);

        #endregion

        #region Operator <  (SessionStatusType1, SessionStatusType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionStatusType1">A SessionStatusType.</param>
        /// <param name="SessionStatusType2">Another SessionStatusType.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SessionStatusType SessionStatusType1,
                                          SessionStatusType SessionStatusType2)

            => SessionStatusType1.CompareTo(SessionStatusType2) < 0;

        #endregion

        #region Operator <= (SessionStatusType1, SessionStatusType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionStatusType1">A SessionStatusType.</param>
        /// <param name="SessionStatusType2">Another SessionStatusType.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SessionStatusType SessionStatusType1,
                                           SessionStatusType SessionStatusType2)

            => SessionStatusType1.CompareTo(SessionStatusType2) <= 0;

        #endregion

        #region Operator >  (SessionStatusType1, SessionStatusType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionStatusType1">A SessionStatusType.</param>
        /// <param name="SessionStatusType2">Another SessionStatusType.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SessionStatusType SessionStatusType1,
                                          SessionStatusType SessionStatusType2)

            => SessionStatusType1.CompareTo(SessionStatusType2) > 0;

        #endregion

        #region Operator >= (SessionStatusType1, SessionStatusType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionStatusType1">A SessionStatusType.</param>
        /// <param name="SessionStatusType2">Another SessionStatusType.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SessionStatusType SessionStatusType1,
                                           SessionStatusType SessionStatusType2)

            => SessionStatusType1.CompareTo(SessionStatusType2) >= 0;

        #endregion

        #endregion

        #region IComparable<SessionStatusType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two SessionStatusTypes.
        /// </summary>
        /// <param name="Object">A SessionStatusType to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SessionStatusType sessionStatusType
                   ? CompareTo(sessionStatusType)
                   : throw new ArgumentException("The given object is not a SessionStatusType!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SessionStatusType)

        /// <summary>
        /// Compares two SessionStatusTypes.
        /// </summary>
        /// <param name="SessionStatusType">A SessionStatusType to compare with.</param>
        public Int32 CompareTo(SessionStatusType SessionStatusType)

            => String.Compare(InternalId,
                              SessionStatusType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<SessionStatusType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SessionStatusTypes for equality.
        /// </summary>
        /// <param name="Object">A SessionStatusType to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SessionStatusType sessionStatusType &&
                   Equals(sessionStatusType);

        #endregion

        #region Equals(SessionStatusType)

        /// <summary>
        /// Compares two SessionStatusTypes for equality.
        /// </summary>
        /// <param name="SessionStatusType">A SessionStatusType to compare with.</param>
        public Boolean Equals(SessionStatusType SessionStatusType)

            => String.Equals(InternalId,
                             SessionStatusType.InternalId,
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
