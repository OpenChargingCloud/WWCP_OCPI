/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Session 2.0 (the "License");
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

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// Extension methods for session identifications.
    /// </summary>
    public static class SessionIdExtensions
    {

        /// <summary>
        /// Indicates whether this session identification is null or empty.
        /// </summary>
        /// <param name="SessionId">A session identification.</param>
        public static Boolean IsNullOrEmpty(this Session_Id? SessionId)
            => !SessionId.HasValue || SessionId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this session identification is NOT null or empty.
        /// </summary>
        /// <param name="SessionId">A session identification.</param>
        public static Boolean IsNotNullOrEmpty(this Session_Id? SessionId)
            => SessionId.HasValue && SessionId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a session.
    /// CiString(36)
    /// </summary>
    public readonly struct Session_Id : IId<Session_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this session identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this session identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the session identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new session identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a session identification.</param>
        private Session_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random session identification.
        /// </summary>
        public static Session_Id NewRandom

            => Parse(Guid.NewGuid().ToString());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a session identification.
        /// </summary>
        /// <param name="Text">A text representation of a session identification.</param>
        public static Session_Id Parse(String Text)
        {

            if (TryParse(Text, out var sessionId))
                return sessionId;

            throw new ArgumentException("Invalid text representation of a session identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a session identification.
        /// </summary>
        /// <param name="Text">A text representation of a session identification.</param>
        public static Session_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var sessionId))
                return sessionId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out SessionId)

        /// <summary>
        /// Try to parse the given text as a session identification.
        /// </summary>
        /// <param name="Text">A text representation of a session identification.</param>
        /// <param name="SessionId">The parsed session identification.</param>
        public static Boolean TryParse(String Text, out Session_Id SessionId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    SessionId = new Session_Id(Text);
                    return true;
                }
                catch (Exception)
                { }
            }

            SessionId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this session identification.
        /// </summary>
        public Session_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (SessionId1, SessionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionId1">A session identification.</param>
        /// <param name="SessionId2">Another session identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Session_Id SessionId1,
                                           Session_Id SessionId2)

            => SessionId1.Equals(SessionId2);

        #endregion

        #region Operator != (SessionId1, SessionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionId1">A session identification.</param>
        /// <param name="SessionId2">Another session identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Session_Id SessionId1,
                                           Session_Id SessionId2)

            => !SessionId1.Equals(SessionId2);

        #endregion

        #region Operator <  (SessionId1, SessionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionId1">A session identification.</param>
        /// <param name="SessionId2">Another session identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Session_Id SessionId1,
                                          Session_Id SessionId2)

            => SessionId1.CompareTo(SessionId2) < 0;

        #endregion

        #region Operator <= (SessionId1, SessionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionId1">A session identification.</param>
        /// <param name="SessionId2">Another session identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Session_Id SessionId1,
                                           Session_Id SessionId2)

            => SessionId1.CompareTo(SessionId2) <= 0;

        #endregion

        #region Operator >  (SessionId1, SessionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionId1">A session identification.</param>
        /// <param name="SessionId2">Another session identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Session_Id SessionId1,
                                          Session_Id SessionId2)

            => SessionId1.CompareTo(SessionId2) > 0;

        #endregion

        #region Operator >= (SessionId1, SessionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionId1">A session identification.</param>
        /// <param name="SessionId2">Another session identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Session_Id SessionId1,
                                           Session_Id SessionId2)

            => SessionId1.CompareTo(SessionId2) >= 0;

        #endregion

        #endregion

        #region IComparable<SessionId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two session identifications.
        /// </summary>
        /// <param name="Object">A session identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Session_Id sessionId
                   ? CompareTo(sessionId)
                   : throw new ArgumentException("The given object is not a session identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SessionId)

        /// <summary>
        /// Compares two session identifications.
        /// </summary>
        /// <param name="SessionId">A session identification to compare with.</param>
        public Int32 CompareTo(Session_Id SessionId)

            => String.Compare(InternalId,
                              SessionId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<SessionId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two session identifications for equality.
        /// </summary>
        /// <param name="Object">A session identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Session_Id sessionId &&
                   Equals(sessionId);

        #endregion

        #region Equals(SessionId)

        /// <summary>
        /// Compares two session identifications for equality.
        /// </summary>
        /// <param name="SessionId">A session identification to compare with.</param>
        public Boolean Equals(Session_Id SessionId)

            => String.Equals(InternalId,
                             SessionId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

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
