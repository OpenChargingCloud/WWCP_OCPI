/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The unique identification of a token.
    /// </summary>
    public readonly struct Token_Id : IId<Token_Id>
    {

        #region Data

        // CiString(3)

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the token identification.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new token identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the token identification.</param>
        private Token_Id(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a token identification.
        /// </summary>
        /// <param name="Text">A text representation of a token identification.</param>
        public static Token_Id Parse(String Text)
        {

            if (TryParse(Text, out Token_Id locationId))
                return locationId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a token identification must not be null or empty!");

            throw new ArgumentException("The given text representation of a token identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a token identification.
        /// </summary>
        /// <param name="Text">A text representation of a token identification.</param>
        public static Token_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Token_Id locationId))
                return locationId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TokenId)

        /// <summary>
        /// Try to parse the given text as a token identification.
        /// </summary>
        /// <param name="Text">A text representation of a token identification.</param>
        /// <param name="TokenId">The parsed token identification.</param>
        public static Boolean TryParse(String Text, out Token_Id TokenId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    TokenId = new Token_Id(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            TokenId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this token identification.
        /// </summary>
        public Token_Id Clone

            => new Token_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (TokenId1, TokenId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenId1">A token identification.</param>
        /// <param name="TokenId2">Another token identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Token_Id TokenId1,
                                           Token_Id TokenId2)

            => TokenId1.Equals(TokenId2);

        #endregion

        #region Operator != (TokenId1, TokenId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenId1">A token identification.</param>
        /// <param name="TokenId2">Another token identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Token_Id TokenId1,
                                           Token_Id TokenId2)

            => !(TokenId1 == TokenId2);

        #endregion

        #region Operator <  (TokenId1, TokenId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenId1">A token identification.</param>
        /// <param name="TokenId2">Another token identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Token_Id TokenId1,
                                          Token_Id TokenId2)

            => TokenId1.CompareTo(TokenId2) < 0;

        #endregion

        #region Operator <= (TokenId1, TokenId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenId1">A token identification.</param>
        /// <param name="TokenId2">Another token identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Token_Id TokenId1,
                                           Token_Id TokenId2)

            => !(TokenId1 > TokenId2);

        #endregion

        #region Operator >  (TokenId1, TokenId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenId1">A token identification.</param>
        /// <param name="TokenId2">Another token identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Token_Id TokenId1,
                                          Token_Id TokenId2)

            => TokenId1.CompareTo(TokenId2) > 0;

        #endregion

        #region Operator >= (TokenId1, TokenId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenId1">A token identification.</param>
        /// <param name="TokenId2">Another token identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Token_Id TokenId1,
                                           Token_Id TokenId2)

            => !(TokenId1 < TokenId2);

        #endregion

        #endregion

        #region IComparable<TokenId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Token_Id locationId
                   ? CompareTo(locationId)
                   : throw new ArgumentException("The given object is not a token identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TokenId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenId">An object to compare with.</param>
        public Int32 CompareTo(Token_Id TokenId)

            => String.Compare(InternalId,
                              TokenId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TokenId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Token_Id locationId &&
                   Equals(locationId);

        #endregion

        #region Equals(TokenId)

        /// <summary>
        /// Compares two token identifications for equality.
        /// </summary>
        /// <param name="TokenId">An token identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Token_Id TokenId)

            => String.Equals(InternalId,
                             TokenId.InternalId,
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
