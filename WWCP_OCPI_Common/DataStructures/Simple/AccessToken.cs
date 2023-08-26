/*
 * Copyright (c) 2015-2023 GraphDefinedGmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for access tokens.
    /// </summary>
    public static class AccessTokenExtensions
    {

        /// <summary>
        /// Indicates whether this access token is null or empty.
        /// </summary>
        /// <param name="AccessToken">An access token.</param>
        public static Boolean IsNullOrEmpty(this AccessToken? AccessToken)
            => !AccessToken.HasValue || AccessToken.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this access token is NOT null or empty.
        /// </summary>
        /// <param name="AccessToken">An access token.</param>
        public static Boolean IsNotNullOrEmpty(this AccessToken? AccessToken)
            => AccessToken.HasValue && AccessToken.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The access token.
    /// </summary>
    public readonly struct AccessToken : IId<AccessToken>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this access token is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this access token is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the access token.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new access token based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an access token.</param>
        private AccessToken(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom(Length = 50)

        /// <summary>
        /// Create a new random access token.
        /// </summary>
        /// <param name="Length">The expected length of the access token.</param>
        public static AccessToken NewRandom(Byte Length = 50)

            => new (RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as an access token.
        /// </summary>
        /// <param name="Text">A text representation of an access token.</param>
        public static AccessToken Parse(String Text)
        {

            if (TryParse(Text, out var accessToken))
                return accessToken;

            throw new ArgumentException($"Invalid text representation of an access token: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as an access token.
        /// </summary>
        /// <param name="Text">A text representation of an access token.</param>
        public static AccessToken? TryParse(String Text)
        {

            if (TryParse(Text, out var accessToken))
                return accessToken;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out AccessToken)

        /// <summary>
        /// Try to parse the given text as an access token.
        /// </summary>
        /// <param name="Text">A text representation of an access token.</param>
        /// <param name="AccessToken">The parsed access token.</param>
        public static Boolean TryParse(String Text, out AccessToken AccessToken)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    AccessToken = new AccessToken(Text);
                    return true;
                }
                catch
                { }
            }

            AccessToken = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this access token.
        /// </summary>
        public AccessToken Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (AccessToken1, AccessToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessToken1">An access token.</param>
        /// <param name="AccessToken2">Another access token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AccessToken AccessToken1,
                                           AccessToken AccessToken2)

            => AccessToken1.Equals(AccessToken2);

        #endregion

        #region Operator != (AccessToken1, AccessToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessToken1">An access token.</param>
        /// <param name="AccessToken2">Another access token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AccessToken AccessToken1,
                                           AccessToken AccessToken2)

            => !AccessToken1.Equals(AccessToken2);

        #endregion

        #region Operator <  (AccessToken1, AccessToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessToken1">An access token.</param>
        /// <param name="AccessToken2">Another access token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AccessToken AccessToken1,
                                          AccessToken AccessToken2)

            => AccessToken1.CompareTo(AccessToken2) < 0;

        #endregion

        #region Operator <= (AccessToken1, AccessToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessToken1">An access token.</param>
        /// <param name="AccessToken2">Another access token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AccessToken AccessToken1,
                                           AccessToken AccessToken2)

            => AccessToken1.CompareTo(AccessToken2) <= 0;

        #endregion

        #region Operator >  (AccessToken1, AccessToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessToken1">An access token.</param>
        /// <param name="AccessToken2">Another access token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AccessToken AccessToken1,
                                          AccessToken AccessToken2)

            => AccessToken1.CompareTo(AccessToken2) > 0;

        #endregion

        #region Operator >= (AccessToken1, AccessToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessToken1">An access token.</param>
        /// <param name="AccessToken2">Another access token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AccessToken AccessToken1,
                                           AccessToken AccessToken2)

            => AccessToken1.CompareTo(AccessToken2) >= 0;

        #endregion

        #endregion

        #region IComparable<AccessToken> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two access tokens.
        /// </summary>
        /// <param name="Object">An access token to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AccessToken accessToken
                   ? CompareTo(accessToken)
                   : throw new ArgumentException("The given object is not an access token!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AccessToken)

        /// <summary>
        /// Compares two access tokens.
        /// </summary>
        /// <param name="AccessToken">An access token to compare with.</param>
        public Int32 CompareTo(AccessToken AccessToken)

            => String.Compare(InternalId,
                              AccessToken.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<AccessToken> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two access tokens for equality.
        /// </summary>
        /// <param name="Object">An access token to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AccessToken accessToken &&
                   Equals(accessToken);

        #endregion

        #region Equals(AccessToken)

        /// <summary>
        /// Compares two access tokens for equality.
        /// </summary>
        /// <param name="AccessToken">An access token to compare with.</param>
        public Boolean Equals(AccessToken AccessToken)

            => String.Equals(InternalId,
                             AccessToken.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

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
