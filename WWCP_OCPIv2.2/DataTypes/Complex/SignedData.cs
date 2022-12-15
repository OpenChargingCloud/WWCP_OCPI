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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// A charging period consists of a start timestamp and a list of
    /// possible values that influence this period, for example:
    /// Amount of energy charged this period, maximum current during
    /// this period etc.
    /// </summary>
    public class SignedData : IEquatable<SignedData>
    {

        #region Properties

        /// <summary>
        /// The name of the encoding used in the SignedData field.
        /// This is the name given to the encoding by a company or group of companies.
        /// </summary>
        [Mandatory]
        public EncodingMethod            EncodingMethod             { get; }

        /// <summary>
        /// The enumeration of signed values.
        /// </summary>
        [Mandatory]
        public IEnumerable<SignedValue>  SignedValues               { get; }

        /// <summary>
        /// Version of the encoding method (when applicable).
        /// </summary>
        [Optional]
        public Int32?                    EncodingMethodVersion      { get; }

        /// <summary>
        /// The public key used to sign the data, base64 encoded.
        /// </summary>
        [Optional]
        public PublicKey?                PublicKey                  { get; }

        /// <summary>
        /// URL that can be shown to an electric vehicle driver.
        /// This URL gives the EV driver the possibility to verify the signed data of a charging session.
        /// </summary>
        [Optional]
        public String                    URL                        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A charging period consists of a start timestamp and an enumeration of possible values that influence this period.
        /// </summary>
        /// <param name="EncodingMethod">The name of the encoding used in the SignedData field.</param>
        /// <param name="SignedValues">An enumeration of signed values.</param>
        /// 
        /// <param name="EncodingMethodVersion">Version of the encoding method (when applicable).</param>
        /// <param name="PublicKey">The public key used to sign the data, base64 encoded.</param>
        /// <param name="URL">URL that can be shown to an electric vehicle driver.</param>
        public SignedData(EncodingMethod            EncodingMethod,
                          IEnumerable<SignedValue>  SignedValues,

                          Int32?                    EncodingMethodVersion   = null,
                          PublicKey?                PublicKey               = null,
                          String                    URL                     = null)
        {

            if (!SignedValues.SafeAny())
                throw new ArgumentNullException(nameof(SignedValues), "The given enumeration of signed values must not be null or empty!");

            this.EncodingMethod          = EncodingMethod;
            this.SignedValues            = SignedValues.Distinct();
            this.EncodingMethodVersion   = EncodingMethodVersion;
            this.PublicKey               = PublicKey;
            this.URL                     = URL;

        }

        #endregion


        #region (static) Parse   (JSON, CustomSignedDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of a signed data.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSignedDataParser">A delegate to parse custom signed data JSON objects.</param>
        public static SignedData Parse(JObject                                  JSON,
                                       CustomJObjectParserDelegate<SignedData>  CustomSignedDataParser   = null)
        {

            if (TryParse(JSON,
                         out SignedData  signedData,
                         out String      ErrorResponse,
                         CustomSignedDataParser))
            {
                return signedData;
            }

            throw new ArgumentException("The given JSON representation of a signed data is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomSignedDataParser = null)

        /// <summary>
        /// Parse the given text representation of a signed data.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomSignedDataParser">A delegate to parse custom signed data JSON objects.</param>
        public static SignedData Parse(String                                   Text,
                                       CustomJObjectParserDelegate<SignedData>  CustomSignedDataParser   = null)
        {

            if (TryParse(Text,
                         out SignedData  signedData,
                         out String      ErrorResponse,
                         CustomSignedDataParser))
            {
                return signedData;
            }

            throw new ArgumentException("The given text representation of a signed data is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out SignedData, out ErrorResponse, CustomSignedDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a signed data.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SignedData">The parsed signed data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject         JSON,
                                       out SignedData  SignedData,
                                       out String      ErrorResponse)

            => TryParse(JSON,
                        out SignedData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a signed data.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SignedData">The parsed signed data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignedDataParser">A delegate to parse custom signed data JSON objects.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       out SignedData                           SignedData,
                                       out String                               ErrorResponse,
                                       CustomJObjectParserDelegate<SignedData>  CustomSignedDataParser   = null)
        {

            try
            {

                SignedData = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse EncodingMethod            [mandatory]

                if (!JSON.ParseMandatory("encoding_method",
                                         "encoding method",
                                         OCPIv2_2.EncodingMethod.TryParse,
                                         out EncodingMethod EncodingMethod,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EncodingMethodVersion     [optional]

                if (JSON.ParseOptional("encoding_method_version",
                                       "encoding method version",
                                       out Int32? EncodingMethodVersion,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse PublicKey                 [optional]

                if (JSON.ParseOptional("public_key",
                                       "public key",
                                       OCPIv2_2.PublicKey.TryParse,
                                       out PublicKey? PublicKey,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse SignedValues              [mandatory]

                if (!JSON.ParseMandatoryJSON("signed_values",
                                             "signed values",
                                             SignedValue.TryParse,
                                             out IEnumerable<SignedValue> SignedValues,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse URL                       [optional]

                var URL = JSON.GetString("url");

                #endregion


                SignedData = new SignedData(EncodingMethod,
                                            SignedValues,
                                            EncodingMethodVersion,
                                            PublicKey,
                                            URL);


                if (CustomSignedDataParser is not null)
                    SignedData = CustomSignedDataParser(JSON,
                                                        SignedData);

                return true;

            }
            catch (Exception e)
            {
                SignedData     = default;
                ErrorResponse  = "The given JSON representation of a signed data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out SignedData, out ErrorResponse, CustomSignedDataParser = null)

        /// <summary>
        /// Try to parse the given text representation of an signedData.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="SignedData">The parsed signedData.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignedDataParser">A delegate to parse custom signed data JSON objects.</param>
        public static Boolean TryParse(String                                   Text,
                                       out SignedData                           SignedData,
                                       out String                               ErrorResponse,
                                       CustomJObjectParserDelegate<SignedData>  CustomSignedDataParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out SignedData,
                                out ErrorResponse,
                                CustomSignedDataParser);

            }
            catch (Exception e)
            {
                SignedData     = default;
                ErrorResponse  = "The given text representation of a signed data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignedDataSerializer = null, CustomSignedValueSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignedDataSerializer">A delegate to serialize custom signed data JSON objects.</param>
        /// <param name="CustomSignedValueSerializer">A delegate to serialize custom signed value JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedData>   CustomSignedDataSerializer    = null,
                              CustomJObjectSerializerDelegate<SignedValue>  CustomSignedValueSerializer   = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("encoding_method",                EncodingMethod.       ToString()),

                           EncodingMethodVersion.HasValue
                               ? new JProperty("encoding_method_version",  EncodingMethodVersion.ToString())
                               : null,

                           PublicKey.HasValue
                               ? new JProperty("public_key",               PublicKey.            ToString())
                               : null,

                           SignedValues.SafeAny()
                               ? new JProperty("signed_values",            new JArray(SignedValues.Select(signedValue => signedValue.ToJSON(CustomSignedValueSerializer))))
                               : null,

                           URL.IsNotNullOrEmpty()
                               ? new JProperty("url",                      URL)
                               : null

                       );

            return CustomSignedDataSerializer is not null
                       ? CustomSignedDataSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SignedData1, SignedData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedData1">A charging period.</param>
        /// <param name="SignedData2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SignedData SignedData1,
                                           SignedData SignedData2)
        {

            if (Object.ReferenceEquals(SignedData1, SignedData2))
                return true;

            if (SignedData1 is null || SignedData2 is null)
                return false;

            return SignedData1.Equals(SignedData2);

        }

        #endregion

        #region Operator != (SignedData1, SignedData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedData1">A charging period.</param>
        /// <param name="SignedData2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SignedData SignedData1,
                                           SignedData SignedData2)

            => !(SignedData1 == SignedData2);

        #endregion

        #endregion

        #region IEquatable<SignedData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is SignedData signedData &&
                   Equals(signedData);

        #endregion

        #region Equals(SignedData)

        /// <summary>
        /// Compares two charging periods for equality.
        /// </summary>
        /// <param name="SignedData">A charging period to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(SignedData SignedData)

            => !(SignedData is null) &&

               EncodingMethod.Equals(SignedData.EncodingMethod) &&

               SignedValues.Count().Equals(SignedData.SignedValues.Count()) &&
               SignedValues.All(signedValue => SignedData.SignedValues.Contains(signedValue)) &&

               ((!EncodingMethodVersion.HasValue        && !SignedData.EncodingMethodVersion.HasValue)        || (EncodingMethodVersion.HasValue           && SignedData.EncodingMethodVersion.HasValue           && EncodingMethodVersion.Value.Equals(SignedData.EncodingMethodVersion.Value))) &&
               ((!PublicKey.            HasValue        && !SignedData.PublicKey.            HasValue)        || (PublicKey.            HasValue           && SignedData.PublicKey.            HasValue           && PublicKey.            Value.Equals(SignedData.PublicKey.            Value))) &&
               ((!URL.                  IsNullOrEmpty() && !SignedData.URL.                  IsNullOrEmpty()) || (URL.                  IsNotNullOrEmpty() && SignedData.URL.                  IsNotNullOrEmpty() && URL.                        Equals(SignedData.URL)));

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

                return EncodingMethod.GetHashCode() * 7 ^
                       SignedValues.Aggregate(0, (hashCode, value) => hashCode ^ value.GetHashCode()) ^

                       (EncodingMethodVersion.HasValue
                            ? EncodingMethodVersion.Value.GetHashCode() * 5
                            : 0) ^

                       (PublicKey.HasValue
                            ? PublicKey.GetHashCode() * 3
                            : 0) ^

                       (URL.IsNotNullOrEmpty()
                            ? URL.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(EncodingMethod,
                             EncodingMethodVersion.HasValue
                                 ? " (v" + EncodingMethodVersion.Value + ")"
                                 : "",
                             " ", SignedValues.Count() + " signed values");

        #endregion

    }

}
