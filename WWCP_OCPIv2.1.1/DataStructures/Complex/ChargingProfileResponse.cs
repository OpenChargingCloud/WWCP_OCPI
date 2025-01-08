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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// A charging profile response.
    /// </summary>
    public class ChargingProfileResponse : IEquatable<ChargingProfileResponse>,
                                           IComparable<ChargingProfileResponse>,
                                           IComparable
    {

        #region Properties

        /// <summary>
        /// Response from the CPO on the charging profile request.
        /// </summary>
        [Mandatory]
        public ChargingProfileResponseTypes  Result     { get; }

        /// <summary>
        /// Timeout for this charging profile in seconds. When the Result is not received within
        /// this timeout, the eMSP can assume that the message might never be send.
        /// </summary>
        [Mandatory]
        public TimeSpan                      Timeout    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new charging profile response.
        /// </summary>
        /// <param name="Result">Response from the CPO on the charging profile request.</param>
        /// <param name="Timeout">Timeout for this charging profile in seconds. When the Result is not received within this timeout, the eMSP can assume that the message might never be send.</param>
        public ChargingProfileResponse(ChargingProfileResponseTypes  Result,
                                       TimeSpan                      Timeout)
        {

            this.Result   = Result;
            this.Timeout  = Timeout;

        }

        #endregion


        #region (static) Parse   (JSON, CustomChargingProfileResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging profile response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomChargingProfileResponseParser">A delegate to parse custom charging profile response JSON objects.</param>
        public static ChargingProfileResponse Parse(JObject                                               JSON,
                                                    CustomJObjectParserDelegate<ChargingProfileResponse>  CustomChargingProfileResponseParser   = null)
        {

            if (TryParse(JSON,
                         out ChargingProfileResponse  chargingProfileResponse,
                         out String                   ErrorResponse,
                         CustomChargingProfileResponseParser))
            {
                return chargingProfileResponse;
            }

            throw new ArgumentException("The given JSON representation of a charging profile response is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomChargingProfileResponseParser = null)

        /// <summary>
        /// Parse the given text representation of a charging profile response.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomChargingProfileResponseParser">A delegate to parse custom charging profile response JSON objects.</param>
        public static ChargingProfileResponse Parse(String                                                Text,
                                                    CustomJObjectParserDelegate<ChargingProfileResponse>  CustomChargingProfileResponseParser   = null)
        {

            if (TryParse(Text,
                         out ChargingProfileResponse  chargingProfileResponse,
                         out String                   ErrorResponse,
                         CustomChargingProfileResponseParser))
            {
                return chargingProfileResponse;
            }

            throw new ArgumentException("The given text representation of a charging profile response is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomChargingProfileResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charging profile response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomChargingProfileResponseParser">A delegate to parse custom charging profile response JSON objects.</param>
        public static ChargingProfileResponse? TryParse(JObject                                               JSON,
                                                        CustomJObjectParserDelegate<ChargingProfileResponse>  CustomChargingProfileResponseParser   = null)
        {

            if (TryParse(JSON,
                         out ChargingProfileResponse  chargingProfileResponse,
                         out String                   ErrorResponse,
                         CustomChargingProfileResponseParser))
            {
                return chargingProfileResponse;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomChargingProfileResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charging profile response.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomChargingProfileResponseParser">A delegate to parse custom charging profile response JSON objects.</param>
        public static ChargingProfileResponse? TryParse(String                                                Text,
                                                        CustomJObjectParserDelegate<ChargingProfileResponse>  CustomChargingProfileResponseParser   = null)
        {

            if (TryParse(Text,
                         out ChargingProfileResponse  chargingProfileResponse,
                         out String                   ErrorResponse,
                         CustomChargingProfileResponseParser))
            {
                return chargingProfileResponse;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingProfileResponse, out ErrorResponse, CustomChargingProfileResponseParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging profile response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingProfileResponse">The parsed charging profile response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                      JSON,
                                       out ChargingProfileResponse  ChargingProfileResponse,
                                       out String                   ErrorResponse)

            => TryParse(JSON,
                        out ChargingProfileResponse,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging profile response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingProfileResponse">The parsed charging profile response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingProfileResponseParser">A delegate to parse custom charging profile response JSON objects.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       out ChargingProfileResponse                           ChargingProfileResponse,
                                       out String                                            ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingProfileResponse>  CustomChargingProfileResponseParser   = null)
        {

            try
            {

                ChargingProfileResponse = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Result      [mandatory]

                if (!JSON.ParseMandatoryEnum("result",
                                             "charging profile response",
                                             out ChargingProfileResponseTypes Result,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Timeout     [mandatory]

                if (!JSON.ParseMandatory("timeout",
                                         "charging profile timeout",
                                         out TimeSpan Timeout,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ChargingProfileResponse = new ChargingProfileResponse(Result,
                                                                      Timeout);

                if (CustomChargingProfileResponseParser is not null)
                    ChargingProfileResponse = CustomChargingProfileResponseParser(JSON,
                                                                                  ChargingProfileResponse);

                return true;

            }
            catch (Exception e)
            {
                ChargingProfileResponse  = default;
                ErrorResponse    = "The given JSON representation of a charging profile response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out ChargingProfileResponse, out ErrorResponse, CustomChargingProfileResponseParser = null)

        /// <summary>
        /// Try to parse the given text representation of a charging profile response.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="ChargingProfileResponse">The parsed charging profileResponse.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingProfileResponseParser">A delegate to parse custom charging profile response JSON objects.</param>
        public static Boolean TryParse(String                                                Text,
                                       out ChargingProfileResponse                           ChargingProfileResponse,
                                       out String                                            ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingProfileResponse>  CustomChargingProfileResponseParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out ChargingProfileResponse,
                                out ErrorResponse,
                                CustomChargingProfileResponseParser);

            }
            catch (Exception e)
            {
                ChargingProfileResponse  = default;
                ErrorResponse            = "The given text representation of a charging profile response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChargingProfileResponseSerializer = null, CustomDisplayTextSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingProfileResponseSerializer">A delegate to serialize custom charging profile response JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingProfileResponse>?  CustomChargingProfileResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<DisplayText>?              CustomDisplayTextSerializer               = null)
        {

            var json = JSONObject.Create(
                           new JProperty("result",   Result.ToString()),
                           new JProperty("timeout",  (UInt32) Timeout.TotalSeconds)
                       );

            return CustomChargingProfileResponseSerializer is not null
                       ? CustomChargingProfileResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingProfileResponse1, ChargingProfileResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileResponse1">A charging profile response.</param>
        /// <param name="ChargingProfileResponse2">Another charging profile response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingProfileResponse ChargingProfileResponse1,
                                           ChargingProfileResponse ChargingProfileResponse2)
        {

            if (Object.ReferenceEquals(ChargingProfileResponse1, ChargingProfileResponse2))
                return true;

            if (ChargingProfileResponse1 is null || ChargingProfileResponse2 is null)
                return false;

            return ChargingProfileResponse1.Equals(ChargingProfileResponse2);

        }

        #endregion

        #region Operator != (ChargingProfileResponse1, ChargingProfileResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileResponse1">A charging profile response.</param>
        /// <param name="ChargingProfileResponse2">Another charging profile response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingProfileResponse ChargingProfileResponse1,
                                           ChargingProfileResponse ChargingProfileResponse2)

            => !(ChargingProfileResponse1 == ChargingProfileResponse2);

        #endregion

        #region Operator <  (ChargingProfileResponse1, ChargingProfileResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileResponse1">A charging profile response.</param>
        /// <param name="ChargingProfileResponse2">Another charging profile response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingProfileResponse ChargingProfileResponse1,
                                          ChargingProfileResponse ChargingProfileResponse2)

            => ChargingProfileResponse1 is null
                   ? throw new ArgumentNullException(nameof(ChargingProfileResponse1), "The given charging profile response must not be null!")
                   : ChargingProfileResponse1.CompareTo(ChargingProfileResponse2) < 0;

        #endregion

        #region Operator <= (ChargingProfileResponse1, ChargingProfileResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileResponse1">A charging profile response.</param>
        /// <param name="ChargingProfileResponse2">Another charging profile response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingProfileResponse ChargingProfileResponse1,
                                           ChargingProfileResponse ChargingProfileResponse2)

            => !(ChargingProfileResponse1 > ChargingProfileResponse2);

        #endregion

        #region Operator >  (ChargingProfileResponse1, ChargingProfileResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileResponse1">A charging profile response.</param>
        /// <param name="ChargingProfileResponse2">Another charging profile response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingProfileResponse ChargingProfileResponse1,
                                          ChargingProfileResponse ChargingProfileResponse2)

            => ChargingProfileResponse1 is null
                   ? throw new ArgumentNullException(nameof(ChargingProfileResponse1), "The given charging profile response must not be null!")
                   : ChargingProfileResponse1.CompareTo(ChargingProfileResponse2) > 0;

        #endregion

        #region Operator >= (ChargingProfileResponse1, ChargingProfileResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileResponse1">A charging profile response.</param>
        /// <param name="ChargingProfileResponse2">Another charging profile response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingProfileResponse ChargingProfileResponse1,
                                           ChargingProfileResponse ChargingProfileResponse2)

            => !(ChargingProfileResponse1 < ChargingProfileResponse2);

        #endregion

        #endregion

        #region IComparable<ChargingProfileResponse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is ChargingProfileResponse chargingProfileResponse
                   ? CompareTo(chargingProfileResponse)
                   : throw new ArgumentException("The given object is not a charging profile response!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingProfileResponse)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileResponse">An object to compare with.</param>
        public Int32 CompareTo(ChargingProfileResponse ChargingProfileResponse)
        {

            if (ChargingProfileResponse is null)
                throw new ArgumentNullException(nameof(ChargingProfileResponse), "The given charging profile response must not be null!");

            var c = Result.CompareTo(ChargingProfileResponse.Result);

            if (c == 0)
                c = Timeout.CompareTo(ChargingProfileResponse.Timeout);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ChargingProfileResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ChargingProfileResponse chargingProfileResponse &&
                   Equals(chargingProfileResponse);

        #endregion

        #region Equals(ChargingProfileResponse)

        /// <summary>
        /// Compares two charging profile responses for equality.
        /// </summary>
        /// <param name="ChargingProfileResponse">A charging profile response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargingProfileResponse ChargingProfileResponse)
        {

            if (ChargingProfileResponse is null)
                throw new ArgumentNullException(nameof(ChargingProfileResponse), "The given charging profile response must not be null!");

            return Result. Equals(ChargingProfileResponse.Result) &&
                   Timeout.Equals(ChargingProfileResponse.Timeout);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Result. GetHashCode() * 3 ^
                       Timeout.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Result, " / ", Timeout.TotalSeconds, " seconds");

        #endregion

    }

}
