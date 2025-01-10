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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3
{

    /// <summary>
    /// A charging profile result.
    /// </summary>
    public readonly struct ChargingProfileResult : IEquatable<ChargingProfileResult>,
                                                   IComparable<ChargingProfileResult>,
                                                   IComparable
    {

        #region Properties

        /// <summary>
        /// The EVSE will indicate if it was able to process the new/updated charging profile.
        /// </summary>
        [Mandatory]
        public ChargingProfileResultTypes  Result    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new charging profile result.
        /// </summary>
        /// <param name="Result">The EVSE will indicate if it was able to process the new/updated charging profile.</param>
        public ChargingProfileResult(ChargingProfileResultTypes  Result)
        {

            this.Result = Result;
 
        }

        #endregion


        #region (static) Parse   (JSON, CustomChargingProfileResultParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging profile result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomChargingProfileResultParser">A delegate to parse custom  charging profile result JSON objects.</param>
        public static ChargingProfileResult Parse(JObject                                              JSON,
                                                  CustomJObjectParserDelegate<ChargingProfileResult>?  CustomChargingProfileResultParser   = null)
        {

            if (TryParse(JSON,
                         out var ChargingProfileResult,
                         out var errorResponse,
                         CustomChargingProfileResultParser))
            {
                return ChargingProfileResult;
            }

            throw new ArgumentException("The given JSON representation of a charging profile result is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomChargingProfileResultParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charging profile result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomChargingProfileResultParser">A delegate to parse custom  charging profile result JSON objects.</param>
        public static ChargingProfileResult? TryParse(JObject                                              JSON,
                                                      CustomJObjectParserDelegate<ChargingProfileResult>?  CustomChargingProfileResultParser   = null)
        {

            if (TryParse(JSON,
                         out var ChargingProfileResult,
                         out var errorResponse,
                         CustomChargingProfileResultParser))
            {
                return ChargingProfileResult;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingProfileResult, out ErrorResponse, CustomChargingProfileResultParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging profile result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingProfileResult">The parsed  charging profile result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                    JSON,
                                       out ChargingProfileResult  ChargingProfileResult,
                                       out String?                ErrorResponse)

            => TryParse(JSON,
                        out ChargingProfileResult,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging profile result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingProfileResult">The parsed  charging profile result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingProfileResultParser">A delegate to parse custom  charging profile result JSON objects.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       out ChargingProfileResult                            ChargingProfileResult,
                                       out String?                                          ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingProfileResult>?  CustomChargingProfileResultParser   = null)
        {

            try
            {

                ChargingProfileResult = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Result    [mandatory]

                if (!JSON.ParseMandatoryEnum("result",
                                             "charging profile result",
                                             out ChargingProfileResultTypes Result,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ChargingProfileResult = new ChargingProfileResult(Result);

                if (CustomChargingProfileResultParser is not null)
                    ChargingProfileResult = CustomChargingProfileResultParser(JSON,
                                                                              ChargingProfileResult);

                return true;

            }
            catch (Exception e)
            {
                ChargingProfileResult  = default;
                ErrorResponse          = "The given JSON representation of a charging profile result is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChargingProfileResultSerializer = null, CustomChargingProfileSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingProfileResultSerializer">A delegate to serialize custom  charging profile result JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingProfileResult>? CustomChargingProfileResultSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("result",  Result.ToString())
                       );

            return CustomChargingProfileResultSerializer is not null
                       ? CustomChargingProfileResultSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingProfileResult1, ChargingProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileResult1">A charging profile result.</param>
        /// <param name="ChargingProfileResult2">Another charging profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingProfileResult ChargingProfileResult1,
                                           ChargingProfileResult ChargingProfileResult2)

            => ChargingProfileResult1.Equals(ChargingProfileResult2);

        #endregion

        #region Operator != (ChargingProfileResult1, ChargingProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileResult1">A charging profile result.</param>
        /// <param name="ChargingProfileResult2">Another charging profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingProfileResult ChargingProfileResult1,
                                           ChargingProfileResult ChargingProfileResult2)

            => !ChargingProfileResult1.Equals(ChargingProfileResult2);

        #endregion

        #region Operator <  (ChargingProfileResult1, ChargingProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileResult1">A charging profile result.</param>
        /// <param name="ChargingProfileResult2">Another charging profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingProfileResult ChargingProfileResult1,
                                          ChargingProfileResult ChargingProfileResult2)

            => ChargingProfileResult1.CompareTo(ChargingProfileResult2) < 0;

        #endregion

        #region Operator <= (ChargingProfileResult1, ChargingProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileResult1">A charging profile result.</param>
        /// <param name="ChargingProfileResult2">Another charging profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingProfileResult ChargingProfileResult1,
                                           ChargingProfileResult ChargingProfileResult2)

            => ChargingProfileResult1.CompareTo(ChargingProfileResult2) <= 0;

        #endregion

        #region Operator >  (ChargingProfileResult1, ChargingProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileResult1">A charging profile result.</param>
        /// <param name="ChargingProfileResult2">Another charging profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingProfileResult ChargingProfileResult1,
                                          ChargingProfileResult ChargingProfileResult2)

            => ChargingProfileResult1.CompareTo(ChargingProfileResult2) > 0;

        #endregion

        #region Operator >= (ChargingProfileResult1, ChargingProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileResult1">A charging profile result.</param>
        /// <param name="ChargingProfileResult2">Another charging profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingProfileResult ChargingProfileResult1,
                                           ChargingProfileResult ChargingProfileResult2)

            => ChargingProfileResult1.CompareTo(ChargingProfileResult2) >= 0;

        #endregion

        #endregion

        #region IComparable<ChargingProfileResult> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two 'charging profile' results.
        /// </summary>
        /// <param name="Object">A 'charging profile' result to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingProfileResult chargingProfileResult
                   ? CompareTo(chargingProfileResult)
                   : throw new ArgumentException("The given object is not a charging profile result!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingProfileResult)

        /// <summary>
        /// Compares two 'charging profile' results.
        /// </summary>
        /// <param name="ChargingProfileResult">A 'charging profile' result to compare with.</param>
        public Int32 CompareTo(ChargingProfileResult ChargingProfileResult)

            => Result.CompareTo(ChargingProfileResult.Result);

        #endregion

        #endregion

        #region IEquatable<ChargingProfileResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two 'charging profile' results for equality.
        /// </summary>
        /// <param name="ChargingProfileResult">A 'charging profile' result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingProfileResult chargingProfileResult &&
                   Equals(chargingProfileResult);

        #endregion

        #region Equals(ChargingProfileResult)

        /// <summary>
        /// Compares two 'charging profile' results for equality.
        /// </summary>
        /// <param name="ChargingProfileResult">A 'charging profile' result to compare with.</param>
        public Boolean Equals(ChargingProfileResult ChargingProfileResult)

            => Result.Equals(ChargingProfileResult.Result);

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

                return Result.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Result.ToString();

        #endregion

    }

}
