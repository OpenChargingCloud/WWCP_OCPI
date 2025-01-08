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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The signed data consists of a start timestamp and a list of
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
        public EncodingMethod            EncodingMethod           { get; }

        /// <summary>
        /// The enumeration of signed values.
        /// </summary>
        [Mandatory]
        public IEnumerable<SignedValue>  SignedValues             { get; }

        /// <summary>
        /// Version of the encoding method (when applicable).
        /// </summary>
        [Optional]
        public Int32?                    EncodingMethodVersion    { get; }

        /// <summary>
        /// The public key used to sign the data, base64 encoded.
        /// </summary>
        [Optional]
        public PublicKey?                PublicKey                { get; }

        /// <summary>
        /// URL that can be shown to an electric vehicle driver.
        /// This URL gives the EV driver the possibility to verify the signed data of a charging session.
        /// </summary>
        [Optional]
        public URL?                      URL                      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new signed data.
        /// </summary>
        /// <param name="EncodingMethod">The name of the encoding used in the SignedData field.</param>
        /// <param name="SignedValues">An enumeration of signed values.</param>
        /// <param name="EncodingMethodVersion">Version of the encoding method (when applicable).</param>
        /// <param name="PublicKey">The public key used to sign the data, base64 encoded.</param>
        /// <param name="URL">URL that can be shown to an electric vehicle driver.</param>
        public SignedData(EncodingMethod            EncodingMethod,
                          IEnumerable<SignedValue>  SignedValues,
                          Int32?                    EncodingMethodVersion   = null,
                          PublicKey?                PublicKey               = null,
                          URL?                      URL                     = null)
        {

            if (!SignedValues.Any())
                throw new ArgumentNullException(nameof(SignedValues), "The given enumeration of signed values must not be null or empty!");

            this.EncodingMethod          = EncodingMethod;
            this.SignedValues            = SignedValues.Distinct();
            this.EncodingMethodVersion   = EncodingMethodVersion;
            this.PublicKey               = PublicKey;
            this.URL                     = URL;

            unchecked
            {

                hashCode = this.EncodingMethod.        GetHashCode()       * 11 ^
                           this.SignedValues.          CalcHashCode()      *  7 ^
                          (this.EncodingMethodVersion?.GetHashCode() ?? 0) *  5 ^
                          (this.PublicKey?.            GetHashCode() ?? 0) *  3 ^
                           this.URL?.                  GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomSignedDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of signed data.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSignedDataParser">A delegate to parse custom signed data JSON objects.</param>
        public static SignedData Parse(JObject                                   JSON,
                                       CustomJObjectParserDelegate<SignedData>?  CustomSignedDataParser   = null)
        {

            if (TryParse(JSON,
                         out var signedData,
                         out var errorResponse,
                         CustomSignedDataParser))
            {
                return signedData;
            }

            throw new ArgumentException("The given JSON representation of signed data is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SignedData, out ErrorResponse, CustomSignedDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of signed data.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SignedData">The parsed signed data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       [NotNullWhen(true)]  out SignedData?  SignedData,
                                       [NotNullWhen(false)] out String?      ErrorResponse)

            => TryParse(JSON,
                        out SignedData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of signed data.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SignedData">The parsed signed data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignedDataParser">A delegate to parse custom signed data JSON objects.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out SignedData?      SignedData,
                                       [NotNullWhen(false)] out String?          ErrorResponse,
                                       CustomJObjectParserDelegate<SignedData>?  CustomSignedDataParser   = null)
        {

            try
            {

                SignedData = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse EncodingMethod           [mandatory]

                if (!JSON.ParseMandatory("encoding_method",
                                         "encoding method",
                                         OCPIv2_1_1.EncodingMethod.TryParse,
                                         out EncodingMethod EncodingMethod,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EncodingMethodVersion    [optional]

                if (JSON.ParseOptional("encoding_method_version",
                                       "encoding method version",
                                       out Int32? EncodingMethodVersion,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PublicKey                [optional]

                if (JSON.ParseOptional("public_key",
                                       "public key",
                                       OCPI.PublicKey.TryParse,
                                       out PublicKey? PublicKey,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse SignedValues             [mandatory]

                if (!JSON.ParseMandatoryHashSet("signed_values",
                                                "signed values",
                                                SignedValue.TryParse,
                                                out HashSet<SignedValue> SignedValues,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse URL                      [optional]

                if (JSON.ParseOptional("url",
                                       "url",
                                       org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.TryParse,
                                       out URL? URL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SignedData = new SignedData(
                                 EncodingMethod,
                                 SignedValues,
                                 EncodingMethodVersion,
                                 PublicKey,
                                 URL
                             );


                if (CustomSignedDataParser is not null)
                    SignedData = CustomSignedDataParser(JSON,
                                                        SignedData);

                return true;

            }
            catch (Exception e)
            {
                SignedData     = default;
                ErrorResponse  = "The given JSON representation of signed data is invalid: " + e.Message;
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
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedData>?   CustomSignedDataSerializer    = null,
                              CustomJObjectSerializerDelegate<SignedValue>?  CustomSignedValueSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("encoding_method",           EncodingMethod.       ToString()),

                           EncodingMethodVersion.HasValue
                               ? new JProperty("encoding_method_version",   EncodingMethodVersion.ToString())
                               : null,

                           PublicKey.HasValue
                               ? new JProperty("public_key",                PublicKey.            ToString())
                               : null,

                           SignedValues.SafeAny()
                               ? new JProperty("signed_values",             new JArray(SignedValues.Select(signedValue => signedValue.ToJSON(CustomSignedValueSerializer))))
                               : null,

                           URL.HasValue
                               ? new JProperty("url",                       URL.            Value.ToString())
                               : null

                       );

            return CustomSignedDataSerializer is not null
                       ? CustomSignedDataSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this signed data.
        /// </summary>
        public SignedData Clone()

            => new (
                   EncodingMethod.Clone(),
                   SignedValues.  Select(signedValue => signedValue.Clone()),
                   EncodingMethodVersion,
                   PublicKey?.    Clone(),
                   URL?.          Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (SignedData1, SignedData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedData1">Signed data.</param>
        /// <param name="SignedData2">Other signed data.</param>
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
        /// <param name="SignedData1">Signed data.</param>
        /// <param name="SignedData2">Other signed data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SignedData SignedData1,
                                           SignedData SignedData2)

            => !(SignedData1 == SignedData2);

        #endregion

        #endregion

        #region IEquatable<SignedData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signed data for equality.
        /// </summary>
        /// <param name="Object">Signed data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignedData signedData &&
                   Equals(signedData);

        #endregion

        #region Equals(SignedData)

        /// <summary>
        /// Compares two signed data for equality.
        /// </summary>
        /// <param name="SignedData">Signed data to compare with.</param>
        public Boolean Equals(SignedData? SignedData)

            => SignedData is not null &&

               EncodingMethod.Equals(SignedData.EncodingMethod) &&

            ((!EncodingMethodVersion.HasValue    && !SignedData.EncodingMethodVersion.HasValue) ||
              (EncodingMethodVersion.HasValue    &&  SignedData.EncodingMethodVersion.HasValue && EncodingMethodVersion.Value.Equals(SignedData.EncodingMethodVersion.Value))) &&

            ((!PublicKey.            HasValue    && !SignedData.PublicKey.            HasValue) ||
              (PublicKey.            HasValue    &&  SignedData.PublicKey.            HasValue && PublicKey.            Value.Equals(SignedData.PublicKey.            Value))) &&

            ((!URL.                  HasValue    && !SignedData.URL.                  HasValue) ||
              (URL.                  HasValue    &&  SignedData.URL.                  HasValue && URL.                  Value.Equals(SignedData.URL.                  Value))) &&

               SignedValues.Count().Equals(SignedData.SignedValues.Count()) &&
               SignedValues.All(signedValue => SignedData.SignedValues.Contains(signedValue));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   EncodingMethod,

                   EncodingMethodVersion.HasValue
                       ? " (v" + EncodingMethodVersion.Value + ")"
                       : "",

                   " ", SignedValues.Count() + " signed value(s)",

                   PublicKey.HasValue
                       ? ", public key: " + PublicKey.Value.ToString().SubstringMax(20)
                       : "",

                   URL.      HasValue
                       ? ", URL: "        + URL.      Value.ToString().SubstringMax(20)
                       : ""

               );

        #endregion

    }

}
