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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Extension methods for token types.
    /// </summary>
    public static class TokenTypeExtensions
    {

        /// <summary>
        /// Indicates whether this token type is null or empty.
        /// </summary>
        /// <param name="TokenType">A token type.</param>
        public static Boolean IsNullOrEmpty(this TokenType? TokenType)
            => !TokenType.HasValue || TokenType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this token type is NOT null or empty.
        /// </summary>
        /// <param name="TokenType">A token type.</param>
        public static Boolean IsNotNullOrEmpty(this TokenType? TokenType)
            => TokenType.HasValue && TokenType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a meter.
    /// string(255)
    /// </summary>
    public readonly struct TokenType : IId<TokenType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this token type is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this token type is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the token type.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new token type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a token type.</param>
        private TokenType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a token type.
        /// </summary>
        /// <param name="Text">A text representation of a token type.</param>
        public static TokenType Parse(String Text)
        {

            if (TryParse(Text, out var tokenType))
                return tokenType;

            throw new ArgumentException($"Invalid text representation of a token type: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a token type.
        /// </summary>
        /// <param name="Text">A text representation of a token type.</param>
        public static TokenType? TryParse(String Text)
        {

            if (TryParse(Text, out var tokenType))
                return tokenType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TokenType)

        /// <summary>
        /// Try to parse the given text as a token type.
        /// </summary>
        /// <param name="Text">A text representation of a token type.</param>
        /// <param name="TokenType">The parsed token type.</param>
        public static Boolean TryParse(String Text, out TokenType TokenType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    TokenType = new TokenType(Text);
                    return true;
                }
                catch
                { }
            }

            TokenType = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this token type.
        /// </summary>
        public TokenType Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// A one time use token identification generated by a server or app.
        /// The eMSP uses this to bind a charging ssession to a customer, probably an app user.
        /// </summary>
        public static TokenType AD_HOC_USER    { get; }
            = new ("AD_HOC_USER");

        /// <summary>
        /// A token identification generated by a server or app to identify a user of an app.
        /// The same user uses the same token for every/multiple charging sessions.
        /// </summary>
        public static TokenType APP_USER       { get; }
            = new ("APP_USER");

        /// <summary>
        /// An EMAID. EMAIDs are used as Tokens when the Charging Station and the vehicle are
        /// using ISO 15118 for communication.
        /// </summary>
        public static TokenType EMAID          { get; }
            = new ("EMAID");

        /// <summary>
        /// Other type of token.
        /// </summary>
        public static TokenType OTHER          { get; }
            = new ("OTHER");

        /// <summary>
        /// RFID token (UID).
        /// </summary>
        public static TokenType RFID           { get; }
            = new ("RFID");

        #endregion


        #region Operator overloading

        #region Operator == (TokenType1, TokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenType1">A token type.</param>
        /// <param name="TokenType2">Another token type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TokenType TokenType1,
                                           TokenType TokenType2)

            => TokenType1.Equals(TokenType2);

        #endregion

        #region Operator != (TokenType1, TokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenType1">A token type.</param>
        /// <param name="TokenType2">Another token type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TokenType TokenType1,
                                           TokenType TokenType2)

            => !TokenType1.Equals(TokenType2);

        #endregion

        #region Operator <  (TokenType1, TokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenType1">A token type.</param>
        /// <param name="TokenType2">Another token type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TokenType TokenType1,
                                          TokenType TokenType2)

            => TokenType1.CompareTo(TokenType2) < 0;

        #endregion

        #region Operator <= (TokenType1, TokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenType1">A token type.</param>
        /// <param name="TokenType2">Another token type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TokenType TokenType1,
                                           TokenType TokenType2)

            => TokenType1.CompareTo(TokenType2) <= 0;

        #endregion

        #region Operator >  (TokenType1, TokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenType1">A token type.</param>
        /// <param name="TokenType2">Another token type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TokenType TokenType1,
                                          TokenType TokenType2)

            => TokenType1.CompareTo(TokenType2) > 0;

        #endregion

        #region Operator >= (TokenType1, TokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenType1">A token type.</param>
        /// <param name="TokenType2">Another token type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TokenType TokenType1,
                                           TokenType TokenType2)

            => TokenType1.CompareTo(TokenType2) >= 0;

        #endregion

        #endregion

        #region IComparable<TokenType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two token types.
        /// </summary>
        /// <param name="Object">A token type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TokenType tokenType
                   ? CompareTo(tokenType)
                   : throw new ArgumentException("The given object is not a token type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TokenType)

        /// <summary>
        /// Compares two token types.
        /// </summary>
        /// <param name="TokenType">A token type to compare with.</param>
        public Int32 CompareTo(TokenType TokenType)

            => String.Compare(InternalId,
                              TokenType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TokenType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two token types for equality.
        /// </summary>
        /// <param name="Object">A token type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TokenType tokenType &&
                   Equals(tokenType);

        #endregion

        #region Equals(TokenType)

        /// <summary>
        /// Compares two token types for equality.
        /// </summary>
        /// <param name="TokenType">A token type to compare with.</param>
        public Boolean Equals(TokenType TokenType)

            => String.Equals(InternalId,
                             TokenType.InternalId,
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
