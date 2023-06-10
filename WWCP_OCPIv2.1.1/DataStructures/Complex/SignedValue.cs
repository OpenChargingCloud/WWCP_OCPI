﻿/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// A signed value consists of a start timestamp and a list of
    /// possible values that influence this period, for example:
    /// Amount of energy charged this period, maximum current during
    /// this period etc.
    /// </summary>
    public class SignedValue : IEquatable<SignedValue>
    {

        #region Properties

        /// <summary>
        /// Nature of the value, in other words, the event this value belongs to.
        /// </summary>
        [Mandatory]
        public SignedValueNature  Nature         { get; }

        /// <summary>
        /// The unencoded string of data. The format of the content depends on the EncodingMethod field.
        /// </summary>
        [Mandatory]
        public String             PlainData      { get; }

        /// <summary>
        /// Blob of signed data, base64 encoded. The format of the content depends on the EncodingMethod field.
        /// </summary>
        [Mandatory]
        public String             SignedData     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A signed value consists of a start timestamp and a
        /// list of possible values that influence this period.
        /// </summary>
        /// <param name="Nature">Nature of the value, in other words, the event this value belongs to.</param>
        /// <param name="PlainData">The unencoded string of data. The format of the content depends on the EncodingMethod field.</param>
        /// <param name="SignedData">Blob of signed data, base64 encoded. The format of the content depends on the EncodingMethod field.</param>
        public SignedValue(SignedValueNature  Nature,
                           String             PlainData,
                           String             SignedData)
        {

            this.Nature      = Nature;
            this.PlainData   = PlainData;
            this.SignedData  = SignedData;

        }

        #endregion


        #region (static) Parse   (JSON, CustomSignedValueParser = null)

        /// <summary>
        /// Parse the given JSON representation of a signed value.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSignedValueParser">A delegate to parse custom signed value JSON objects.</param>
        public static SignedValue Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<SignedValue>?  CustomSignedValueParser   = null)
        {

            if (TryParse(JSON,
                         out var signedValue,
                         out var errorResponse,
                         CustomSignedValueParser))
            {
                return signedValue!;
            }

            throw new ArgumentException("The given JSON representation of a signed value is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SignedValue, out ErrorResponse, CustomSignedValueParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a signed value.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SignedValue">The parsed signed value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject           JSON,
                                       out SignedValue?  SignedValue,
                                       out String?       ErrorResponse)

            => TryParse(JSON,
                        out SignedValue,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a signed value.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SignedValue">The parsed signed value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignedValueParser">A delegate to parse custom signed value JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       out SignedValue?                           SignedValue,
                                       out String?                                ErrorResponse,
                                       CustomJObjectParserDelegate<SignedValue>?  CustomSignedValueParser   = null)
        {

            try
            {

                SignedValue = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Nature        [mandatory]

                if (!JSON.ParseMandatory("nature",
                                         "signed value nature",
                                         SignedValueNature.TryParse,
                                         out SignedValueNature Nature,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PlainData     [mandatory]

                if (!JSON.ParseMandatoryText("plain_data",
                                             "plain data",
                                             out String PlainData,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse SignedData    [mandatory]

                if (!JSON.ParseMandatoryText("signed_data",
                                             "signed data",
                                             out String SignedData,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                SignedValue = new SignedValue(Nature,
                                              PlainData,
                                              SignedData);


                if (CustomSignedValueParser is not null)
                    SignedValue = CustomSignedValueParser(JSON,
                                                          SignedValue);

                return true;

            }
            catch (Exception e)
            {
                SignedValue    = default;
                ErrorResponse  = "The given JSON representation of a signed value is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignedValueSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignedValueSerializer">A delegate to serialize custom signed value JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedValue>? CustomSignedValueSerializer = null)
        {

            var json = JSONObject.Create(

                           new JProperty("nature",       Nature.ToString()),
                           new JProperty("plain_data",   PlainData),
                           new JProperty("signed_data",  SignedData)

                       );

            return CustomSignedValueSerializer is not null
                       ? CustomSignedValueSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public SignedValue Clone()

            => new (Nature.Clone,
                    new String(PlainData.ToCharArray()),
                    new String(SignedData.ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (SignedValue1, SignedValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedValue1">A signed value.</param>
        /// <param name="SignedValue2">Another signed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SignedValue SignedValue1,
                                           SignedValue SignedValue2)
        {

            if (Object.ReferenceEquals(SignedValue1, SignedValue2))
                return true;

            if (SignedValue1 is null || SignedValue2 is null)
                return false;

            return SignedValue1.Equals(SignedValue2);

        }

        #endregion

        #region Operator != (SignedValue1, SignedValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedValue1">A signed value.</param>
        /// <param name="SignedValue2">Another signed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SignedValue SignedValue1,
                                           SignedValue SignedValue2)

            => !(SignedValue1 == SignedValue2);

        #endregion

        #endregion

        #region IEquatable<SignedValue> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signed values for equality.
        /// </summary>
        /// <param name="Object">A signed value to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignedValue signedValue &&
                   Equals(signedValue);

        #endregion

        #region Equals(SignedValue)

        /// <summary>
        /// Compares two signed values for equality.
        /// </summary>
        /// <param name="SignedValue">A signed value to compare with.</param>
        public Boolean Equals(SignedValue? SignedValue)

            => SignedValue is not null &&

               Nature.    Equals(SignedValue.Nature)    &&
               PlainData. Equals(SignedValue.PlainData) &&
               SignedData.Equals(SignedValue.SignedData);

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

                return Nature.    GetHashCode() * 5 ^
                       PlainData. GetHashCode() * 3 ^
                       SignedData.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Nature,
                   " -> ",
                   PlainData. SubstringMax(10),
                   SignedData.SubstringMax(10)

               );

        #endregion

    }

}
