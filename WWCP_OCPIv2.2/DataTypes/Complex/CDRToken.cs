/*
 * Copyright (c) 2015-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
//using System.Management.Instrumentation;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// CDR token.
    /// </summary>
    public readonly struct CDRToken : IEquatable<CDRToken>,
                                      IComparable<CDRToken>,
                                      IComparable
    {

        #region Properties

        /// <summary>
        /// The unique identification by which this token can be identified.
        /// </summary>
        [Mandatory]
        public Token_Id     UID           { get; }

        /// <summary>
        /// The type of the token.
        /// </summary>
        [Mandatory]
        public TokenTypes   TokenType     { get; }

        /// <summary>
        /// Uniquely identifies the EV driver contract token within the eMSP’s
        /// platform (and suboperator platforms).
        /// </summary>
        [Mandatory]
        public Contract_Id  ContractId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A charging period consists of a start timestamp and a
        /// list of possible values that influence this period.
        /// </summary>
        /// <param name="UID">The unique identification by which this token can be identified.</param>
        /// <param name="TokenType">The type of the token.</param>
        /// <param name="ContractId">Uniquely identifies the EV driver contract token within the eMSP’s platform (and suboperator platforms).</param>
        public CDRToken(Token_Id     UID,
                        TokenTypes   TokenType,
                        Contract_Id  ContractId)
        {

            this.UID         = UID;
            this.TokenType   = TokenType;
            this.ContractId  = ContractId;

        }

        #endregion


        #region (static) Parse   (JSON, CustomCDRTokenParser = null)

        /// <summary>
        /// Parse the given JSON representation of an charge detail record token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCDRTokenParser">A delegate to parse custom charge detail record token JSON objects.</param>
        public static CDRToken Parse(JObject                                JSON,
                                     CustomJObjectParserDelegate<CDRToken>  CustomCDRTokenParser   = null)
        {

            if (TryParse(JSON,
                         out CDRToken  cdrToken,
                         out String    ErrorResponse,
                         CustomCDRTokenParser))
            {
                return cdrToken;
            }

            throw new ArgumentException("The given JSON representation of an charge detail record token is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomCDRTokenParser = null)

        /// <summary>
        /// Parse the given text representation of an charge detail record token.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomCDRTokenParser">A delegate to parse custom charge detail record token JSON objects.</param>
        public static CDRToken Parse(String                                 Text,
                                     CustomJObjectParserDelegate<CDRToken>  CustomCDRTokenParser   = null)
        {

            if (TryParse(Text,
                         out CDRToken  cdrToken,
                         out String    ErrorResponse,
                         CustomCDRTokenParser))
            {
                return cdrToken;
            }

            throw new ArgumentException("The given text representation of an charge detail record token is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomCDRTokenParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charge detail record token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCDRTokenParser">A delegate to parse custom charge detail record token JSON objects.</param>
        public static CDRToken? TryParse(JObject                                JSON,
                                         CustomJObjectParserDelegate<CDRToken>  CustomCDRTokenParser   = null)
        {

            if (TryParse(JSON,
                         out CDRToken  cdrToken,
                         out String    ErrorResponse,
                         CustomCDRTokenParser))
            {
                return cdrToken;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomCDRTokenParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charge detail record token.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomCDRTokenParser">A delegate to parse custom charge detail record token JSON objects.</param>
        public static CDRToken? TryParse(String                                 Text,
                                         CustomJObjectParserDelegate<CDRToken>  CustomCDRTokenParser   = null)
        {

            if (TryParse(Text,
                         out CDRToken  cdrToken,
                         out String    ErrorResponse,
                         CustomCDRTokenParser))
            {
                return cdrToken;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out CDRToken, out ErrorResponse, CustomCDRTokenParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an charge detail record token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDRToken">The parsed charge detail record token.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject       JSON,
                                       out CDRToken  CDRToken,
                                       out String    ErrorResponse)

            => TryParse(JSON,
                        out CDRToken,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an charge detail record token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDRToken">The parsed charge detail record token.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCDRTokenParser">A delegate to parse custom charge detail record token JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       out CDRToken                           CDRToken,
                                       out String                             ErrorResponse,
                                       CustomJObjectParserDelegate<CDRToken>  CustomCDRTokenParser   = null)
        {

            try
            {

                CDRToken = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse UID               [mandatory]

                if (!JSON.ParseMandatory("uid",
                                         "token identification",
                                         Token_Id.TryParse,
                                         out Token_Id UID,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TokenType         [mandatory]

                if (!JSON.ParseMandatoryEnum("type",
                                             "token type",
                                             out TokenTypes TokenType,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ContractId        [mandatory]

                if (!JSON.ParseMandatory("contract_id",
                                         "contract identification",
                                         Contract_Id.TryParse,
                                         out Contract_Id ContractId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                CDRToken = new CDRToken(UID,
                                        TokenType,
                                        ContractId);


                if (CustomCDRTokenParser is not null)
                    CDRToken = CustomCDRTokenParser(JSON,
                                                    CDRToken);

                return true;

            }
            catch (Exception e)
            {
                CDRToken       = default;
                ErrorResponse  = "The given JSON representation of an charge detail record token is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out CDRToken, out ErrorResponse, CustomCDRTokenParser = null)

        /// <summary>
        /// Try to parse the given text representation of an cdrToken.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CDRToken">The parsed cdrToken.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCDRTokenParser">A delegate to parse custom charge detail record token JSON objects.</param>
        public static Boolean TryParse(String                                 Text,
                                       out CDRToken                           CDRToken,
                                       out String                             ErrorResponse,
                                       CustomJObjectParserDelegate<CDRToken>  CustomCDRTokenParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out CDRToken,
                                out ErrorResponse,
                                CustomCDRTokenParser);

            }
            catch (Exception e)
            {
                CDRToken       = default;
                ErrorResponse  = "The given text representation of an charge detail record token is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCDRTokenSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCDRTokenSerializer">A delegate to serialize custom charge detail record token JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CDRToken> CustomCDRTokenSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("uid",          UID.       ToString()),
                           new JProperty("type",         TokenType. ToString()),
                           new JProperty("contract_id",  ContractId.ToString())

                       );

            return CustomCDRTokenSerializer is not null
                       ? CustomCDRTokenSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CDRToken1, CDRToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken1">A charging period.</param>
        /// <param name="CDRToken2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDRToken CDRToken1,
                                           CDRToken CDRToken2)

            => CDRToken1.Equals(CDRToken2);

        #endregion

        #region Operator != (CDRToken1, CDRToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken1">A charging period.</param>
        /// <param name="CDRToken2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDRToken CDRToken1,
                                           CDRToken CDRToken2)

            => !(CDRToken1 == CDRToken2);

        #endregion

        #region Operator <  (CDRToken1, CDRToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken1">A charging period.</param>
        /// <param name="CDRToken2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDRToken CDRToken1,
                                          CDRToken CDRToken2)

            => CDRToken1.CompareTo(CDRToken2) < 0;

        #endregion

        #region Operator <= (CDRToken1, CDRToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken1">A charging period.</param>
        /// <param name="CDRToken2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDRToken CDRToken1,
                                           CDRToken CDRToken2)

            => !(CDRToken1 > CDRToken2);

        #endregion

        #region Operator >  (CDRToken1, CDRToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken1">A charging period.</param>
        /// <param name="CDRToken2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDRToken CDRToken1,
                                          CDRToken CDRToken2)

            => CDRToken1.CompareTo(CDRToken2) > 0;

        #endregion

        #region Operator >= (CDRToken1, CDRToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken1">A charging period.</param>
        /// <param name="CDRToken2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDRToken CDRToken1,
                                           CDRToken CDRToken2)

            => !(CDRToken1 < CDRToken2);

        #endregion

        #endregion

        #region IComparable<CDRToken> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is CDRToken token
                   ? CompareTo(token)
                   : throw new ArgumentException("The given object is not a charging period!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDRToken)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRToken">An object to compare with.</param>
        public Int32 CompareTo(CDRToken CDRToken)
        {

            var c = UID.       CompareTo(CDRToken.UID);

            if (c == 0)
                c = TokenType. CompareTo(CDRToken.TokenType);

            if (c == 0)
                c = ContractId.CompareTo(CDRToken.ContractId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<CDRToken> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is CDRToken token &&
                   Equals(token);

        #endregion

        #region Equals(CDRToken)

        /// <summary>
        /// Compares two charging periods for equality.
        /// </summary>
        /// <param name="CDRToken">A charging period to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CDRToken CDRToken)

            => UID.       Equals(CDRToken.UID)       &&
               TokenType. Equals(CDRToken.TokenType) &&
               ContractId.Equals(CDRToken.ContractId);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return UID.       GetHashCode() * 5 ^
                       TokenType. GetHashCode() * 3 ^
                       ContractId.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(UID,
                             " (", TokenType, ") => ",
                             ContractId);

        #endregion

    }

}
