/*
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

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// An active charging profile result.
    /// </summary>
    public readonly struct ActiveChargingProfileResult : IEquatable<ActiveChargingProfileResult>,
                                                         IComparable<ActiveChargingProfileResult>,
                                                         IComparable
    {

        #region Properties

        /// <summary>
        /// The EVSE will indicate if it was able to process the request for the ActiveChargingProfile.
        /// </summary>
        [Mandatory]
        public ChargingProfileResultTypes  Result     { get; }

        /// <summary>
        /// The requested ActiveChargingProfile, if the result field is set to: ACCEPTED
        /// </summary>
        [Optional]
        public ActiveChargingProfile?      Profile    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new active charging profile result.
        /// </summary>
        /// <param name="Result">The EVSE will indicate if it was able to process the request for the ActiveChargingProfile.</param>
        /// <param name="Profile">The requested ActiveChargingProfile, if the result field is set to: ACCEPTED</param>
        public ActiveChargingProfileResult(ChargingProfileResultTypes  Result,
                                           ActiveChargingProfile?      Profile   = null)
        {

            this.Result   = Result;
            this.Profile  = Profile;

        }

        #endregion


        #region (static) Parse   (JSON, CustomActiveChargingProfileResultParser = null)

        /// <summary>
        /// Parse the given JSON representation of an active charging profile result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomActiveChargingProfileResultParser">A delegate to parse custom active charging profile result JSON objects.</param>
        public static ActiveChargingProfileResult Parse(JObject                                                    JSON,
                                                        CustomJObjectParserDelegate<ActiveChargingProfileResult>?  CustomActiveChargingProfileResultParser   = null)
        {

            if (TryParse(JSON,
                         out var activeChargingProfileResult,
                         out var errorResponse,
                         CustomActiveChargingProfileResultParser))
            {
                return activeChargingProfileResult!;
            }

            throw new ArgumentException("The given JSON representation of an active charging profile result is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ActiveChargingProfileResult, out ErrorResponse, CustomActiveChargingProfileResultParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an active charging profile result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ActiveChargingProfileResult">The parsed active charging profile result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                          JSON,
                                       out ActiveChargingProfileResult  ActiveChargingProfileResult,
                                       out String?                      ErrorResponse)

            => TryParse(JSON,
                        out ActiveChargingProfileResult,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an active charging profile result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ActiveChargingProfileResult">The parsed active charging profile result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomActiveChargingProfileResultParser">A delegate to parse custom active charging profile result JSON objects.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       out ActiveChargingProfileResult                            ActiveChargingProfileResult,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<ActiveChargingProfileResult>?  CustomActiveChargingProfileResultParser   = null)
        {

            try
            {

                ActiveChargingProfileResult = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Result     [mandatory]

                if (!JSON.ParseMandatoryEnum("result",
                                             "charging profile result",
                                             out ChargingProfileResultTypes Result,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Profile    [optional]

                if (JSON.ParseOptionalJSON("profile",
                                           "charging profile timeout",
                                           ActiveChargingProfile.TryParse,
                                           out ActiveChargingProfile? Profile,
                                           out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ActiveChargingProfileResult = new ActiveChargingProfileResult(Result,
                                                                              Profile);

                if (CustomActiveChargingProfileResultParser is not null)
                    ActiveChargingProfileResult = CustomActiveChargingProfileResultParser(JSON,
                                                                                          ActiveChargingProfileResult);

                return true;

            }
            catch (Exception e)
            {
                ActiveChargingProfileResult  = default;
                ErrorResponse                = "The given JSON representation of an active charging profile result is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomActiveChargingProfileResultSerializer = null, CustomActiveChargingProfileSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomActiveChargingProfileResultSerializer">A delegate to serialize custom active charging profile result JSON objects.</param>
        /// <param name="CustomActiveChargingProfileSerializer">A delegate to serialize custom active charging profile JSON objects.</param>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profile JSON objects.</param>
        /// <param name="CustomChargingProfilePeriodSerializer">A delegate to serialize custom charging profile period JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ActiveChargingProfileResult>?  CustomActiveChargingProfileResultSerializer   = null,
                              CustomJObjectSerializerDelegate<ActiveChargingProfile>?        CustomActiveChargingProfileSerializer         = null,
                              CustomJObjectSerializerDelegate<ChargingProfile>?              CustomChargingProfileSerializer               = null,
                              CustomJObjectSerializerDelegate<ChargingProfilePeriod>?        CustomChargingProfilePeriodSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("result",   Result. ToString()),

                           Profile is not null
                               ? new JProperty("profile",  Profile.ToJSON  (CustomActiveChargingProfileSerializer,
                                                                            CustomChargingProfileSerializer,
                                                                            CustomChargingProfilePeriodSerializer))
                               : null

                       );

            return CustomActiveChargingProfileResultSerializer is not null
                       ? CustomActiveChargingProfileResultSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ActiveChargingProfileResult1, ActiveChargingProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ActiveChargingProfileResult1">An active charging profile result.</param>
        /// <param name="ActiveChargingProfileResult2">Another active charging profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ActiveChargingProfileResult ActiveChargingProfileResult1,
                                           ActiveChargingProfileResult ActiveChargingProfileResult2)

            => ActiveChargingProfileResult1.Equals(ActiveChargingProfileResult2);

        #endregion

        #region Operator != (ActiveChargingProfileResult1, ActiveChargingProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ActiveChargingProfileResult1">An active charging profile result.</param>
        /// <param name="ActiveChargingProfileResult2">Another active charging profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ActiveChargingProfileResult ActiveChargingProfileResult1,
                                           ActiveChargingProfileResult ActiveChargingProfileResult2)

            => !ActiveChargingProfileResult1.Equals(ActiveChargingProfileResult2);

        #endregion

        #region Operator <  (ActiveChargingProfileResult1, ActiveChargingProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ActiveChargingProfileResult1">An active charging profile result.</param>
        /// <param name="ActiveChargingProfileResult2">Another active charging profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ActiveChargingProfileResult ActiveChargingProfileResult1,
                                          ActiveChargingProfileResult ActiveChargingProfileResult2)

            => ActiveChargingProfileResult1.CompareTo(ActiveChargingProfileResult2) < 0;

        #endregion

        #region Operator <= (ActiveChargingProfileResult1, ActiveChargingProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ActiveChargingProfileResult1">An active charging profile result.</param>
        /// <param name="ActiveChargingProfileResult2">Another active charging profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ActiveChargingProfileResult ActiveChargingProfileResult1,
                                           ActiveChargingProfileResult ActiveChargingProfileResult2)

            => ActiveChargingProfileResult1.CompareTo(ActiveChargingProfileResult2) <= 0;

        #endregion

        #region Operator >  (ActiveChargingProfileResult1, ActiveChargingProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ActiveChargingProfileResult1">An active charging profile result.</param>
        /// <param name="ActiveChargingProfileResult2">Another active charging profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ActiveChargingProfileResult ActiveChargingProfileResult1,
                                          ActiveChargingProfileResult ActiveChargingProfileResult2)

            => ActiveChargingProfileResult1.CompareTo(ActiveChargingProfileResult2) > 0;

        #endregion

        #region Operator >= (ActiveChargingProfileResult1, ActiveChargingProfileResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ActiveChargingProfileResult1">An active charging profile result.</param>
        /// <param name="ActiveChargingProfileResult2">Another active charging profile result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ActiveChargingProfileResult ActiveChargingProfileResult1,
                                           ActiveChargingProfileResult ActiveChargingProfileResult2)

            => ActiveChargingProfileResult1.CompareTo(ActiveChargingProfileResult2) >= 0;

        #endregion

        #endregion

        #region IComparable<ActiveChargingProfileResult> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two active charging profile results.
        /// </summary>
        /// <param name="Object">An active charging profile result to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ActiveChargingProfileResult activeChargingProfileResult
                   ? CompareTo(activeChargingProfileResult)
                   : throw new ArgumentException("The given object is not an active charging profile result!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ActiveChargingProfileResult)

        /// <summary>
        /// Compares two active charging profile results.
        /// </summary>
        /// <param name="ActiveChargingProfileResult">An active charging profile result to compare with.</param>
        public Int32 CompareTo(ActiveChargingProfileResult ActiveChargingProfileResult)
        {

            var c = Result.CompareTo(ActiveChargingProfileResult.Result);

            // Profile

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ActiveChargingProfileResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two active charging profile results for equality.
        /// </summary>
        /// <param name="Object">An active charging profile result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ActiveChargingProfileResult activeChargingProfileResult &&
                   Equals(activeChargingProfileResult);

        #endregion

        #region Equals(ActiveChargingProfileResult)

        /// <summary>
        /// Compares two active charging profile results for equality.
        /// </summary>
        /// <param name="ActiveChargingProfileResult">An active charging profile result to compare with.</param>
        public Boolean Equals(ActiveChargingProfileResult ActiveChargingProfileResult)

            => Result. Equals(ActiveChargingProfileResult.Result) &&

             ((Profile is     null && ActiveChargingProfileResult.Profile is     null) ||
              (Profile is not null && ActiveChargingProfileResult.Profile is not null && Profile.Equals(ActiveChargingProfileResult.Profile)));

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

                return Result.  GetHashCode() * 3 ^
                       Profile?.GetHashCode() ?? 0;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Result,

                   Profile is not null
                       ? ", " + Profile
                       : ""

               );

        #endregion

    }

}
