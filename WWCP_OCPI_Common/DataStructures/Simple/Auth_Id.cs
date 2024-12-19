/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Auth 2.0 (the "License");
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
    /// Extension methods for authentication credential identifications.
    /// </summary>
    public static class AuthIdExtensions
    {

        /// <summary>
        /// Indicates whether this authentication credential identification is null or empty.
        /// </summary>
        /// <param name="AuthId">An authentication credential identification.</param>
        public static Boolean IsNullOrEmpty(this Auth_Id? AuthId)
            => !AuthId.HasValue || AuthId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this authentication credential identification is NOT null or empty.
        /// </summary>
        /// <param name="AuthId">An authentication credential identification.</param>
        public static Boolean IsNotNullOrEmpty(this Auth_Id? AuthId)
            => AuthId.HasValue && AuthId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of an authentication credential.
    /// string(36)
    /// </summary>
    public readonly struct Auth_Id : IId<Auth_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this authentication credential identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this authentication credential identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the authentication credential identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authentication credential identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an authentication credential identification.</param>
        private Auth_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom(Length = 20)

        /// <summary>
        /// Create a new random authentication credential identification.
        /// </summary>
        /// <param name="Length">The expected length of the authentication credential identification.</param>
        public static Auth_Id NewRandom(Byte Length = 30)

            => new (RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as an authentication credential identification.
        /// </summary>
        /// <param name="Text">A text representation of an authentication credential identification.</param>
        public static Auth_Id Parse(String Text)
        {

            if (TryParse(Text, out var authId))
                return authId;

            throw new ArgumentException($"Invalid text representation of an authentication credential identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as an authentication credential identification.
        /// </summary>
        /// <param name="Text">A text representation of an authentication credential identification.</param>
        public static Auth_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var authId))
                return authId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out AuthId)

        /// <summary>
        /// Try to parse the given text as an authentication credential identification.
        /// </summary>
        /// <param name="Text">A text representation of an authentication credential identification.</param>
        /// <param name="AuthId">The parsed authentication credential identification.</param>
        public static Boolean TryParse(String Text, out Auth_Id AuthId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    AuthId = new Auth_Id(Text);
                    return true;
                }
                catch
                { }
            }

            AuthId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this authentication credential identification.
        /// </summary>
        public Auth_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">An authentication credential identification.</param>
        /// <param name="AuthId2">Another authentication credential identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Auth_Id AuthId1,
                                           Auth_Id AuthId2)

            => AuthId1.Equals(AuthId2);

        #endregion

        #region Operator != (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">An authentication credential identification.</param>
        /// <param name="AuthId2">Another authentication credential identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Auth_Id AuthId1,
                                           Auth_Id AuthId2)

            => !AuthId1.Equals(AuthId2);

        #endregion

        #region Operator <  (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">An authentication credential identification.</param>
        /// <param name="AuthId2">Another authentication credential identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Auth_Id AuthId1,
                                          Auth_Id AuthId2)

            => AuthId1.CompareTo(AuthId2) < 0;

        #endregion

        #region Operator <= (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">An authentication credential identification.</param>
        /// <param name="AuthId2">Another authentication credential identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Auth_Id AuthId1,
                                           Auth_Id AuthId2)

            => AuthId1.CompareTo(AuthId2) <= 0;

        #endregion

        #region Operator >  (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">An authentication credential identification.</param>
        /// <param name="AuthId2">Another authentication credential identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Auth_Id AuthId1,
                                          Auth_Id AuthId2)

            => AuthId1.CompareTo(AuthId2) > 0;

        #endregion

        #region Operator >= (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">An authentication credential identification.</param>
        /// <param name="AuthId2">Another authentication credential identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Auth_Id AuthId1,
                                           Auth_Id AuthId2)

            => AuthId1.CompareTo(AuthId2) >= 0;

        #endregion

        #endregion

        #region IComparable<AuthId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two authentication credential identifications.
        /// </summary>
        /// <param name="Object">An authentication credential identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Auth_Id authId
                   ? CompareTo(authId)
                   : throw new ArgumentException("The given object is not an authentication credential identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AuthId)

        /// <summary>
        /// Compares two authentication credential identifications.
        /// </summary>
        /// <param name="AuthId">An authentication credential identification to compare with.</param>
        public Int32 CompareTo(Auth_Id AuthId)

            => String.Compare(InternalId,
                              AuthId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<AuthId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two authentication credential identifications for equality.
        /// </summary>
        /// <param name="Object">An authentication credential identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Auth_Id authId &&
                   Equals(authId);

        #endregion

        #region Equals(AuthId)

        /// <summary>
        /// Compares two authentication credential identifications for equality.
        /// </summary>
        /// <param name="AuthId">An authentication credential identification to compare with.</param>
        public Boolean Equals(Auth_Id AuthId)

            => String.Equals(InternalId,
                             AuthId.InternalId,
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
