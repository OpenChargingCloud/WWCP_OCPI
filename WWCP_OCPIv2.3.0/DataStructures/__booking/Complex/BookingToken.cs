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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// A booking token.
    /// </summary>
    public class BookingToken : IEquatable<BookingToken>,
                                IComparable<BookingToken>,
                                IComparable
    {

        #region Properties

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this booking token.
        /// </summary>
        [Mandatory]
        public CountryCode  CountryCode     { get; }

        /// <summary>
        /// The identification of the charge point operator that 'owns' this booking token
        /// (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id     PartyId         { get; }

        /// <summary>
        /// The unique identification by which this token can be identified.
        /// </summary>
        [Mandatory]
        public Token_Id     UID             { get; }

        /// <summary>
        /// The type of the token.
        /// </summary>
        [Mandatory]
        public TokenType    Type            { get; }

        /// <summary>
        /// Uniquely identifies the EV driver contract token within the eMSP’s
        /// platform (and suboperator platforms).
        /// </summary>
        [Mandatory]
        public Contract_Id  ContractId      { get; }

        /// <summary>
        /// Only possible if it is part as one of the LocationAccess options in the BookingTerms.
        /// </summary>
        [Optional]
        public String?      LicensePlate    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A booking token consists of a start timestamp and a
        /// list of possible values that influence this period.
        /// </summary>
        /// <param name="CountryCode">An ISO-3166 alpha-2 country code of the charge point operator that 'owns' this booking token.</param>
        /// <param name="PartyId">The identification of the charge point operator that 'owns' this booking token (following the ISO-15118 standard).</param>
        /// <param name="UID">The unique identification by which this token can be identified.</param>
        /// <param name="TokenType">The type of the token.</param>
        /// <param name="ContractId">Uniquely identifies the EV driver contract token within the eMSP’s platform (and suboperator platforms).</param>
        /// <param name="LicensePlate">Only possible if it is part as one of the LocationAccess options in the BookingTerms.</param></param>
        public BookingToken(CountryCode  CountryCode,
                            Party_Id     PartyId,
                            Token_Id     UID,
                            TokenType    TokenType,
                            Contract_Id  ContractId,
                            String?      LicensePlate   = null)
        {

            this.CountryCode   = CountryCode;
            this.PartyId       = PartyId;
            this.UID           = UID;
            this.Type          = TokenType;
            this.ContractId    = ContractId;
            this.LicensePlate  = LicensePlate;

            unchecked
            {

                hashCode = this.CountryCode.  GetHashCode() * 13 ^
                           this.PartyId.      GetHashCode() * 11 ^
                           this.UID.          GetHashCode() *  7 ^
                           this.Type.         GetHashCode() *  5 ^
                           this.ContractId.   GetHashCode() *  3 ^
                           this.LicensePlate?.GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomBookingTokenParser = null)

        /// <summary>
        /// Parse the given JSON representation of a booking token token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomBookingTokenParser">A delegate to parse custom booking token token JSON objects.</param>
        public static BookingToken Parse(JObject                                 JSON,
                                     CustomJObjectParserDelegate<BookingToken>?  CustomBookingTokenParser   = null)
        {

            if (TryParse(JSON,
                         out var bookingToken,
                         out var errorResponse,
                         CustomBookingTokenParser))
            {
                return bookingToken;
            }

            throw new ArgumentException("The given JSON representation of a booking token token is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out BookingToken, out ErrorResponse, CustomBookingTokenParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a booking token token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="BookingToken">The parsed booking token token.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out BookingToken?  BookingToken,
                                       [NotNullWhen(false)] out String?        ErrorResponse)

            => TryParse(JSON,
                        out BookingToken,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a booking token token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="BookingToken">The parsed booking token token.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBookingTokenParser">A delegate to parse custom booking token token JSON objects.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out BookingToken?      BookingToken,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomJObjectParserDelegate<BookingToken>?  CustomBookingTokenParser   = null)
        {

            try
            {

                BookingToken = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CountryCode    [mandatory]

                if (!JSON.ParseMandatory("country_code",
                                         "country code",
                                         CountryCode.TryParse,
                                         out CountryCode countryCode,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PartyIdURL     [mandatory]

                if (!JSON.ParseMandatory("party_id",
                                         "party identification",
                                         Party_Id.TryParse,
                                         out Party_Id partyId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse UID            [mandatory]

                if (!JSON.ParseMandatory("uid",
                                         "token identification",
                                         Token_Id.TryParse,
                                         out Token_Id UID,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TokenType      [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "token type",
                                         TokenType.TryParse,
                                         out TokenType tokenType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ContractId     [mandatory]

                if (!JSON.ParseMandatory("contract_id",
                                         "contract identification",
                                         Contract_Id.TryParse,
                                         out Contract_Id contractId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                BookingToken = new BookingToken(
                               countryCode,
                               partyId,
                               UID,
                               tokenType,
                               contractId
                           );


                if (CustomBookingTokenParser is not null)
                    BookingToken = CustomBookingTokenParser(JSON,
                                                    BookingToken);

                return true;

            }
            catch (Exception e)
            {
                BookingToken       = default;
                ErrorResponse  = "The given JSON representation of a booking token token is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBookingTokenSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBookingTokenSerializer">A delegate to serialize custom booking token token JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BookingToken>? CustomBookingTokenSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("country_code",    CountryCode.ToString()),
                                 new JProperty("party_id",        PartyId.    ToString()),
                                 new JProperty("uid",             UID.        ToString()),
                                 new JProperty("type",            Type.       ToString()),
                                 new JProperty("contract_id",     ContractId. ToString()),

                           LicensePlate.IsNotNullOrEmpty()
                               ? new JProperty("license_plate",   LicensePlate)
                               : null

                       );

            return CustomBookingTokenSerializer is not null
                       ? CustomBookingTokenSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this booking token.
        /// </summary>
        public BookingToken Clone()

            => new (
                   CountryCode.  Clone(),
                   PartyId.      Clone(),
                   UID.          Clone(),
                   Type.         Clone(),
                   ContractId.   Clone(),
                   LicensePlate?.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (BookingToken1, BookingToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingToken1">A booking token.</param>
        /// <param name="BookingToken2">Another booking token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BookingToken? BookingToken1,
                                           BookingToken? BookingToken2)
        {

            if (Object.ReferenceEquals(BookingToken1, BookingToken2))
                return true;

            if (BookingToken1 is null || BookingToken2 is null)
                return false;

            return BookingToken1.Equals(BookingToken2);

        }

        #endregion

        #region Operator != (BookingToken1, BookingToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingToken1">A booking token.</param>
        /// <param name="BookingToken2">Another booking token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BookingToken? BookingToken1,
                                           BookingToken? BookingToken2)

            => !(BookingToken1 == BookingToken2);

        #endregion

        #region Operator <  (BookingToken1, BookingToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingToken1">A booking token.</param>
        /// <param name="BookingToken2">Another booking token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BookingToken? BookingToken1,
                                          BookingToken? BookingToken2)

            => BookingToken1 is null
                   ? throw new ArgumentNullException(nameof(BookingToken1), "The given booking token must not be null!")
                   : BookingToken1.CompareTo(BookingToken2) < 0;

        #endregion

        #region Operator <= (BookingToken1, BookingToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingToken1">A booking token.</param>
        /// <param name="BookingToken2">Another booking token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BookingToken? BookingToken1,
                                           BookingToken? BookingToken2)

            => !(BookingToken1 > BookingToken2);

        #endregion

        #region Operator >  (BookingToken1, BookingToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingToken1">A booking token.</param>
        /// <param name="BookingToken2">Another booking token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BookingToken? BookingToken1,
                                          BookingToken? BookingToken2)

            => BookingToken1 is null
                   ? throw new ArgumentNullException(nameof(BookingToken1), "The given booking token must not be null!")
                   : BookingToken1.CompareTo(BookingToken2) > 0;

        #endregion

        #region Operator >= (BookingToken1, BookingToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingToken1">A booking token.</param>
        /// <param name="BookingToken2">Another booking token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BookingToken? BookingToken1,
                                           BookingToken? BookingToken2)

            => !(BookingToken1 < BookingToken2);

        #endregion

        #endregion

        #region IComparable<BookingToken> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two booking tokens.
        /// </summary>
        /// <param name="Object">A booking token to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is BookingToken bookingToken
                   ? CompareTo(bookingToken)
                   : throw new ArgumentException("The given object is not a booking token!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BookingToken)

        /// <summary>s
        /// Compares two booking tokens.
        /// </summary>
        /// <param name="Object">A booking token to compare with.</param>
        public Int32 CompareTo(BookingToken? BookingToken)
        {

            if (BookingToken is null)
                throw new ArgumentNullException(nameof(BookingToken), "The given booking token must not be null!");

            var c = CountryCode. CompareTo(BookingToken.CountryCode);

            if (c == 0)
                c = PartyId.     CompareTo(BookingToken.PartyId);

            if (c == 0)
                c = UID.         CompareTo(BookingToken.UID);

            if (c == 0)
                c = Type.        CompareTo(BookingToken.Type);

            if (c == 0)
                c = ContractId.  CompareTo(BookingToken.ContractId);

            if (c == 0 && LicensePlate is not null && BookingToken.LicensePlate is not null)
                c = LicensePlate.CompareTo(BookingToken.LicensePlate);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<BookingToken> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two booking tokens for equality.
        /// </summary>
        /// <param name="Object">A booking token to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BookingToken bookingToken &&
                   Equals(bookingToken);

        #endregion

        #region Equals(BookingToken)

        /// <summary>
        /// Compares two booking tokens for equality.
        /// </summary>
        /// <param name="BookingToken">A booking token to compare with.</param>
        public Boolean Equals(BookingToken? BookingToken)

            => BookingToken is not null &&

               CountryCode.Equals(BookingToken.CountryCode) &&
               PartyId.    Equals(BookingToken.PartyId)     &&
               UID.        Equals(BookingToken.UID)         &&
               Type.       Equals(BookingToken.Type)        &&
               ContractId. Equals(BookingToken.ContractId)  &&

             ((LicensePlate is     null && BookingToken.LicensePlate is     null)  ||
              (LicensePlate is not null && BookingToken.LicensePlate is not null && LicensePlate.Equals(BookingToken.LicensePlate)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   $"{CountryCode}-{PartyId}*{UID} ({Type}) => {ContractId}",

                   LicensePlate.IsNotNullOrEmpty()
                       ? $" [{LicensePlate}]"
                       : ""

               );

        #endregion

    }

}
