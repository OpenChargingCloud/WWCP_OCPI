/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A token status.
    /// </summary>
    public class TokenStatus : IEquatable<TokenStatus>,
                               IComparable<TokenStatus>,
                               IComparable
    {

        #region Properties

        /// <summary>
        /// The name of the energy supplier for this token.
        /// </summary>
        [Mandatory]
        public Token               Token                { get; }

        /// <summary>
        /// The optional contract identification at the energy supplier, that belongs
        /// to the owner of this token.
        /// </summary>
        [Mandatory]
        public AllowedType         Status               { get; }

        /// <summary>
        /// The reference to location details.
        /// </summary>
        [Optional]
        public LocationReference?  LocationReference    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new token status for an EV driver.
        /// </summary>
        /// <param name="Token">The name of the energy supplier for this token.</param>
        /// <param name="Status">The optional contract identification at the energy supplier, that belongs to the owner of this token.</param>
        /// <param name="LocationReference">A reference to location details.</param>
        public TokenStatus(Token               Token,
                           AllowedType         Status,
                           LocationReference?  LocationReference   = null)
        {

            this.Token              = Token;
            this.Status             = Status;
            this.LocationReference  = LocationReference;


            unchecked
            {

                hashCode = this.Token.             GetHashCode() * 5 ^
                           this.Status.            GetHashCode() * 3 ^
                           this.LocationReference?.GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomTokenStatusParser = null)

        /// <summary>
        /// Parse the given JSON representation of a token status.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTokenStatusParser">A delegate to parse custom token status JSON objects.</param>
        public static TokenStatus Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<TokenStatus>?  CustomTokenStatusParser   = null)
        {

            if (TryParse(JSON,
                         out var tokenStatus,
                         out var errorResponse,
                         CustomTokenStatusParser))
            {
                return tokenStatus;
            }

            throw new ArgumentException("The given JSON representation of a token status is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TokenStatus, out ErrorResponse, CustomTokenStatusParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a token status.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TokenStatus">The parsed token status.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out TokenStatus?  TokenStatus,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out TokenStatus,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a token status.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TokenStatus">The parsed token status.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTokenStatusParser">A delegate to parse custom token status JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out TokenStatus?      TokenStatus,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<TokenStatus>?  CustomTokenStatusParser   = null)
        {

            try
            {

                TokenStatus = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Token                [mandatory]

                if (!JSON.ParseMandatoryJSON("token",
                                             "token",
                                             Token.TryParse,
                                             out Token? token,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Status               [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "token status",
                                         AllowedType.TryParse,
                                         out AllowedType status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse LocationReference    [optional]

                if (JSON.ParseOptionalEnum("locationReference",
                                           "location reference",
                                           out LocationReference? locationReference,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                TokenStatus = new TokenStatus(
                                  token,
                                  status,
                                  locationReference
                              );


                if (CustomTokenStatusParser is not null)
                    TokenStatus = CustomTokenStatusParser(JSON,
                                                          TokenStatus);

                return true;

            }
            catch (Exception e)
            {
                TokenStatus    = default;
                ErrorResponse  = "The given JSON representation of a token status is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTokenStatusSerializer = null, CustomLocationReferenceSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTokenStatusSerializer">A delegate to serialize custom token status JSON objects.</param>
        /// <param name="CustomTokenSerializer">A delegate to serialize custom token JSON objects.</param>
        /// <param name="CustomEnergyContractSerializer">A delegate to serialize custom energy contract JSON objects.</param>
        /// <param name="CustomLocationReferenceSerializer">A delegate to serialize custom location reference JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TokenStatus>?        CustomTokenStatusSerializer         = null,
                              CustomJObjectSerializerDelegate<Token>?              CustomTokenSerializer               = null,
                              CustomJObjectSerializerDelegate<EnergyContract>?     CustomEnergyContractSerializer      = null,
                              CustomJObjectSerializerDelegate<LocationReference>?  CustomLocationReferenceSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("token",               Token. ToJSON(true,
                                                                                    true,
                                                                                    true,
                                                                                    true,
                                                                                    CustomTokenSerializer,
                                                                                    CustomEnergyContractSerializer)),

                                 new JProperty("status",              Status.ToString()),

                           LocationReference.HasValue
                               ? new JProperty("locationReference",   LocationReference.Value.ToJSON(CustomLocationReferenceSerializer))
                               : null

                       );

            return CustomTokenStatusSerializer is not null
                       ? CustomTokenStatusSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this token status.
        /// </summary>
        public TokenStatus Clone()

            => new (
                   Token.             Clone(),
                   Status.            Clone(),
                   LocationReference?.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (TokenStatus1, TokenStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenStatus1">A token status.</param>
        /// <param name="TokenStatus2">Another token status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TokenStatus TokenStatus1,
                                           TokenStatus TokenStatus2)
        {

            if (Object.ReferenceEquals(TokenStatus1, TokenStatus2))
                return true;

            if (TokenStatus1 is null || TokenStatus2 is null)
                return false;

            return TokenStatus1.Equals(TokenStatus2);

        }

        #endregion

        #region Operator != (TokenStatus1, TokenStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenStatus1">A token status.</param>
        /// <param name="TokenStatus2">Another token status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TokenStatus TokenStatus1,
                                           TokenStatus TokenStatus2)

            => !(TokenStatus1 == TokenStatus2);

        #endregion

        #region Operator <  (TokenStatus1, TokenStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenStatus1">A token status.</param>
        /// <param name="TokenStatus2">Another token status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TokenStatus TokenStatus1,
                                          TokenStatus TokenStatus2)

            => TokenStatus1 is null
                   ? throw new ArgumentNullException(nameof(TokenStatus1), "The given token status must not be null!")
                   : TokenStatus1.CompareTo(TokenStatus2) < 0;

        #endregion

        #region Operator <= (TokenStatus1, TokenStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenStatus1">A token status.</param>
        /// <param name="TokenStatus2">Another token status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TokenStatus TokenStatus1,
                                           TokenStatus TokenStatus2)

            => !(TokenStatus1 > TokenStatus2);

        #endregion

        #region Operator >  (TokenStatus1, TokenStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenStatus1">A token status.</param>
        /// <param name="TokenStatus2">Another token status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TokenStatus TokenStatus1,
                                          TokenStatus TokenStatus2)

            => TokenStatus1 is null
                   ? throw new ArgumentNullException(nameof(TokenStatus1), "The given token status must not be null!")
                   : TokenStatus1.CompareTo(TokenStatus2) > 0;

        #endregion

        #region Operator >= (TokenStatus1, TokenStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TokenStatus1">A token status.</param>
        /// <param name="TokenStatus2">Another token status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TokenStatus TokenStatus1,
                                           TokenStatus TokenStatus2)

            => !(TokenStatus1 < TokenStatus2);

        #endregion

        #endregion

        #region IComparable<TokenStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two token status.
        /// </summary>
        /// <param name="Object">A token status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TokenStatus tokenStatus
                   ? CompareTo(tokenStatus)
                   : throw new ArgumentException("The given object is not a token status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TokenStatus)

        /// <summary>
        /// Compares two token status.
        /// </summary>
        /// <param name="TokenStatus">A token status to compare with.</param>
        public Int32 CompareTo(TokenStatus? TokenStatus)
        {

            if (TokenStatus is null)
                throw new ArgumentNullException(nameof(TokenStatus), "The given token status must not be null!");

            var c = Status.                 CompareTo(TokenStatus.Status);

            if (c == 0)
                c = Token.                  CompareTo(TokenStatus.Token);

            if (c == 0 && LocationReference.HasValue && TokenStatus.LocationReference.HasValue)
                c = LocationReference.Value.CompareTo(TokenStatus.LocationReference.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<TokenStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two token status for equality.
        /// </summary>
        /// <param name="Object">A token status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TokenStatus tokenStatus &&
                   Equals(tokenStatus);

        #endregion

        #region Equals(TokenStatus)

        /// <summary>
        /// Compares two token status for equality.
        /// </summary>
        /// <param name="TokenStatus">A token status to compare with.</param>
        public Boolean Equals(TokenStatus? TokenStatus)

            => TokenStatus is not null &&

               Status.           Equals(TokenStatus.Status) &&
               Token.            Equals(TokenStatus.Token)  &&
               LocationReference.Equals(TokenStatus.LocationReference);

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

                   $"{Token.Id} ({Status})",

                   LocationReference.HasValue
                       ? " at " + LocationReference.Value
                       : ""

               );

        #endregion


    }

}
