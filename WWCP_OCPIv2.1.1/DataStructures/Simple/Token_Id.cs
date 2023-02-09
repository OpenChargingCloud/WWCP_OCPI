/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Token 2.0 (the "License");
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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Extension methods for token identifications.
    /// </summary>
    public static class TokenIdExtensions
    {

        /// <summary>
        /// Indicates whether this token identification is null or empty.
        /// </summary>
        /// <param name="TokenId">A token identification.</param>
        public static Boolean IsNullOrEmpty(this Token_Id? TokenId)
            => !TokenId.HasValue || TokenId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this token identification is NOT null or empty.
        /// </summary>
        /// <param name="TokenId">A token identification.</param>
        public static Boolean IsNotNullOrEmpty(this Token_Id? TokenId)
            => TokenId.HasValue && TokenId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a token.
    /// CiString(36)
    /// </summary>
    public readonly struct Token_Id : IId<Token_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this token identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this token identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the token identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new token identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a token identification.</param>
        private Token_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random token identification.
        /// </summary>
        public static Token_Id NewRandom

            => Parse(Guid.NewGuid().ToString());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a token identification.
        /// </summary>
        /// <param name="Text">A text representation of a token identification.</param>
        public static Token_Id Parse(String Text)
        {

            if (TryParse(Text, out var tokenId))
                return tokenId;

            throw new ArgumentException("Invalid text representation of a token identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a token identification.
        /// </summary>
        /// <param name="Text">A text representation of a token identification.</param>
        public static Token_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var tokenId))
                return tokenId;

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

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    TokenId = new Token_Id(Text);
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

            => new (
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

            => !TokenId1.Equals(TokenId2);

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

            => TokenId1.CompareTo(TokenId2) <= 0;

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

            => TokenId1.CompareTo(TokenId2) >= 0;

        #endregion

        #endregion

        #region IComparable<TokenId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two token identifications.
        /// </summary>
        /// <param name="Object">A token identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Token_Id tokenId
                   ? CompareTo(tokenId)
                   : throw new ArgumentException("The given object is not a token identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TokenId)

        /// <summary>
        /// Compares two token identifications.
        /// </summary>
        /// <param name="TokenId">A token identification to compare with.</param>
        public Int32 CompareTo(Token_Id TokenId)

            => String.Compare(InternalId,
                              TokenId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TokenId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two token identifications for equality.
        /// </summary>
        /// <param name="Object">A token identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Token_Id tokenId &&
                   Equals(tokenId);

        #endregion

        #region Equals(TokenId)

        /// <summary>
        /// Compares two token identifications for equality.
        /// </summary>
        /// <param name="TokenId">A token identification to compare with.</param>
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
