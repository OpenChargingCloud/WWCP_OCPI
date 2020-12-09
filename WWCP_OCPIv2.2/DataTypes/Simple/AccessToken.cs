/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// An access token.
    /// </summary>
    public readonly struct AccessToken : IId<AccessToken>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        private static readonly Random random = new Random(DateTime.Now.Millisecond);

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty

            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the access token.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new access token.
        /// </summary>
        /// <param name="String">The string representation of the access token.</param>
        private AccessToken(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Random  (Length)

        /// <summary>
        /// Create a new random access token.
        /// </summary>
        /// <param name="Length">The expected length of the access token.</param>
        public static AccessToken Random(Byte Length = 30)

            => new AccessToken(random.RandomString(Length));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an access token.
        /// </summary>
        /// <param name="Text">A text representation of an access token.</param>
        public static AccessToken Parse(String Text)
        {

            if (TryParse(Text, out AccessToken locationId))
                return locationId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an access token must not be null or empty!");

            throw new ArgumentException("The given text representation of an access token is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an access token.
        /// </summary>
        /// <param name="Text">A text representation of an access token.</param>
        public static AccessToken? TryParse(String Text)
        {

            if (TryParse(Text, out AccessToken locationId))
                return locationId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out AccessToken)

        /// <summary>
        /// Try to parse the given text as an access token.
        /// </summary>
        /// <param name="Text">A text representation of an access token.</param>
        /// <param name="AccessToken">The parsed access token.</param>
        public static Boolean TryParse(String Text, out AccessToken AccessToken)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    AccessToken = new AccessToken(Text.Trim());
                    return true;
                }
                catch (Exception)
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

            => new AccessToken(
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

            => !(AccessToken1 == AccessToken2);

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

            => !(AccessToken1 > AccessToken2);

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

            => !(AccessToken1 < AccessToken2);

        #endregion

        #endregion

        #region IComparable<AccessToken> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is AccessToken locationId
                   ? CompareTo(locationId)
                   : throw new ArgumentException("The given object is not an access token!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AccessToken)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessToken">An object to compare with.</param>
        public Int32 CompareTo(AccessToken AccessToken)

            => String.Compare(InternalId,
                              AccessToken.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<AccessToken> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is AccessToken locationId &&
                   Equals(locationId);

        #endregion

        #region Equals(AccessToken)

        /// <summary>
        /// Compares two access tokens for equality.
        /// </summary>
        /// <param name="AccessToken">An access token to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(AccessToken AccessToken)

            => String.Equals(InternalId,
                             AccessToken.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
