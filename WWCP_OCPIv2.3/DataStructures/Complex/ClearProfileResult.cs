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
    /// A clear profile result.
    /// </summary>
    public readonly struct ClearProfileResult : IEquatable<ClearProfileResult>,
                                                IComparable<ClearProfileResult>,
                                                IComparable
    {

        #region Properties

        /// <summary>
        /// The ClearProfileResult object is send by the CPO to the given response_url in a POST request.
        /// It contains the result of the DELETE (ClearProfile) request send by the eMSP.
        /// </summary>
        [Mandatory]
        public ChargingProfileResultTypes  Result    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new clear profile result.
        /// </summary>
        /// <param name="Result">The ClearProfileResult object is send by the CPO to the given response_url in a POST request. It contains the result of the DELETE (ClearProfile) request send by the eMSP.</param>
        public ClearProfileResult(ChargingProfileResultTypes Result)
        {

            this.Result = Result;
 
        }

        #endregion


        #region (static) Parse   (JSON, CustomClearProfileResultParser = null)

        /// <summary>
        /// Parse the given JSON representation of a clear profile result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomClearProfileResultParser">A delegate to parse custom  clear profile result JSON objects.</param>
        public static ClearProfileResult Parse(JObject                                           JSON,
                                               CustomJObjectParserDelegate<ClearProfileResult>?  CustomClearProfileResultParser   = null)
        {

            if (TryParse(JSON,
                         out var ClearProfileResult,
                         out var errorResponse,
                         CustomClearProfileResultParser))
            {
                return ClearProfileResult;
            }

            throw new ArgumentException("The given JSON representation of a clear profile result is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomClearProfileResultParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a clear profile result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomClearProfileResultParser">A delegate to parse custom  clear profile result JSON objects.</param>
        public static ClearProfileResult? TryParse(JObject                                           JSON,
                                                   CustomJObjectParserDelegate<ClearProfileResult>?  CustomClearProfileResultParser   = null)
        {

            if (TryParse(JSON,
                         out var ClearProfileResult,
                         out var errorResponse,
                         CustomClearProfileResultParser))
            {
                return ClearProfileResult;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out ClearProfileResult, out ErrorResponse, CustomClearProfileResultParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a clear profile result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ClearProfileResult">The parsed  clear profile result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                 JSON,
                                       out ClearProfileResult  ClearProfileResult,
                                       out String?             ErrorResponse)

            => TryParse(JSON,
                        out ClearProfileResult,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a clear profile result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ClearProfileResult">The parsed  clear profile result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearProfileResultParser">A delegate to parse custom  clear profile result JSON objects.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       out ClearProfileResult                            ClearProfileResult,
                                       out String?                                       ErrorResponse,
                                       CustomJObjectParserDelegate<ClearProfileResult>?  CustomClearProfileResultParser   = null)
        {

            try
            {

                ClearProfileResult = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Result    [mandatory]

                if (!JSON.ParseMandatoryEnum("result",
                                             "clear profile result",
                                             out ChargingProfileResultTypes Result,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ClearProfileResult = new ClearProfileResult(Result);

                if (CustomClearProfileResultParser is not null)
                    ClearProfileResult = CustomClearProfileResultParser(JSON,
                                                                        ClearProfileResult);

                return true;

            }
            catch (Exception e)
            {
                ClearProfileResult  = default;
                ErrorResponse       = "The given JSON representation of a clear profile result is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearProfileResultSerializer = null, CustomChargingProfileSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearProfileResultSerializer">A delegate to serialize custom  clear profile result JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearProfileResult>? CustomClearProfileResultSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("result",  Result.ToString())
                       );

            return CustomClearProfileResultSerializer is not null
                       ? CustomClearProfileResultSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearProfileResult1, ClearProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearProfileResult1">A clear profile result.</param>
        /// <param name="ClearProfileResult2">Another clear profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ClearProfileResult ClearProfileResult1,
                                           ClearProfileResult ClearProfileResult2)

            => ClearProfileResult1.Equals(ClearProfileResult2);

        #endregion

        #region Operator != (ClearProfileResult1, ClearProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearProfileResult1">A clear profile result.</param>
        /// <param name="ClearProfileResult2">Another clear profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ClearProfileResult ClearProfileResult1,
                                           ClearProfileResult ClearProfileResult2)

            => !ClearProfileResult1.Equals(ClearProfileResult2);

        #endregion

        #region Operator <  (ClearProfileResult1, ClearProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearProfileResult1">A clear profile result.</param>
        /// <param name="ClearProfileResult2">Another clear profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ClearProfileResult ClearProfileResult1,
                                          ClearProfileResult ClearProfileResult2)

            => ClearProfileResult1.CompareTo(ClearProfileResult2) < 0;

        #endregion

        #region Operator <= (ClearProfileResult1, ClearProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearProfileResult1">A clear profile result.</param>
        /// <param name="ClearProfileResult2">Another clear profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ClearProfileResult ClearProfileResult1,
                                           ClearProfileResult ClearProfileResult2)

            => ClearProfileResult1.CompareTo(ClearProfileResult2) <= 0;

        #endregion

        #region Operator >  (ClearProfileResult1, ClearProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearProfileResult1">A clear profile result.</param>
        /// <param name="ClearProfileResult2">Another clear profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ClearProfileResult ClearProfileResult1,
                                          ClearProfileResult ClearProfileResult2)

            => ClearProfileResult1.CompareTo(ClearProfileResult2) > 0;

        #endregion

        #region Operator >= (ClearProfileResult1, ClearProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearProfileResult1">A clear profile result.</param>
        /// <param name="ClearProfileResult2">Another clear profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ClearProfileResult ClearProfileResult1,
                                           ClearProfileResult ClearProfileResult2)

            => ClearProfileResult1.CompareTo(ClearProfileResult2) >= 0;

        #endregion

        #endregion

        #region IComparable<ClearProfileResult> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two 'clear profile' results.
        /// </summary>
        /// <param name="Object">A 'clear profile' result to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ClearProfileResult clearProfileResult
                   ? CompareTo(clearProfileResult)
                   : throw new ArgumentException("The given object is not a clear profile result!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ClearProfileResult)

        /// <summary>
        /// Compares two 'clear profile' results.
        /// </summary>
        /// <param name="ClearProfileResult">A 'clear profile' result to compare with.</param>
        public Int32 CompareTo(ClearProfileResult ClearProfileResult)

            => Result.CompareTo(ClearProfileResult.Result);

        #endregion

        #endregion

        #region IEquatable<ClearProfileResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two 'clear profile' results for equality.
        /// </summary>
        /// <param name="Object">A 'clear profile' result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearProfileResult clearProfileResult &&
                   Equals(clearProfileResult);

        #endregion

        #region Equals(ClearProfileResult)

        /// <summary>
        /// Compares two 'clear profile' results for equality.
        /// </summary>
        /// <param name="ClearProfileResult">A 'clear profile' result to compare with.</param>
        public Boolean Equals(ClearProfileResult ClearProfileResult)

            => Result.Equals(ClearProfileResult.Result);

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
