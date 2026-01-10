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

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A ClearChargingProfile request.
    /// </summary>
    public class ClearChargingProfileRequest
    {

        #region Properties

        /// <summary>
        /// The unique ID that identifies the Charging Profile that is to be cleared.
        /// </summary>
        public ChargingProfile_Id  ChargingProfileId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearChargingProfile request.
        /// </summary>
        /// <param name="ChargingProfileId">The unique ID that identifies the Charging Profile that is to be cleared.</param>
        public ClearChargingProfileRequest(ChargingProfile_Id ChargingProfileId)
        {
            this.ChargingProfileId = ChargingProfileId;
        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON as a ClearChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomClearChargingProfileRequestParser">A delegate to parse custom ClearChargingProfile request JSON objects.</param>
        public static ClearChargingProfileRequest Parse(JObject                                                    JSON,
                                                        CustomJObjectParserDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestParser   = null)
        {

            if (TryParse(JSON,
                         out var clearChargingProfileRequest,
                         out var errorResponse,
                         CustomClearChargingProfileRequestParser))
            {
                return clearChargingProfileRequest;
            }

            throw new ArgumentException("The given JSON representation of a ClearChargingProfile request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ClearChargingProfileRequest, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ClearChargingProfileRequest">The parsed ClearChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       [NotNullWhen(true)]  out ClearChargingProfileRequest?  ClearChargingProfileRequest,
                                       [NotNullWhen(false)] out String?                       ErrorResponse)

            => TryParse(JSON,
                        out ClearChargingProfileRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text as a ClearChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ClearChargingProfileRequest">The parsed ClearChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearChargingProfileRequestParser">A delegate to parse custom ClearChargingProfile request JSON objects.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       [NotNullWhen(true)]  out ClearChargingProfileRequest?      ClearChargingProfileRequest,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       CustomJObjectParserDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestParser)
        {

            try
            {

                ClearChargingProfileRequest = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ChargingProfileId    [mandatory]

                if (!JSON.ParseMandatory("charging_profile_id",
                                         "charging profile identification",
                                         ChargingProfile_Id.TryParse,
                                         out ChargingProfile_Id ChargingProfileId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ClearChargingProfileRequest = new ClearChargingProfileRequest(
                                                  ChargingProfileId
                                              );


                if (CustomClearChargingProfileRequestParser is not null)
                    ClearChargingProfileRequest = CustomClearChargingProfileRequestParser(JSON,
                                                                                          ClearChargingProfileRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearChargingProfileRequest  = null;
                ErrorResponse                = "The given JSON representation of a ClearChargingProfile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearChargingProfileRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearChargingProfileRequestSerializer">A delegate to serialize custom subscription cancellation requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearChargingProfileRequest>? CustomClearChargingProfileRequestSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("charging_profile_id",   ChargingProfileId.ToString())
                       );

            return CustomClearChargingProfileRequestSerializer is not null
                       ? CustomClearChargingProfileRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this ClearChargingProfile request.
        /// </summary>
        public ClearChargingProfileRequest Clone()

            => new (
                   ChargingProfileId.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A ClearChargingProfile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another ClearChargingProfile request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ClearChargingProfileRequest ClearChargingProfileRequest1,
                                           ClearChargingProfileRequest ClearChargingProfileRequest2)

            => ClearChargingProfileRequest1.Equals(ClearChargingProfileRequest2);

        #endregion

        #region Operator != (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A ClearChargingProfile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another ClearChargingProfile request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ClearChargingProfileRequest ClearChargingProfileRequest1,
                                           ClearChargingProfileRequest ClearChargingProfileRequest2)

            => !ClearChargingProfileRequest1.Equals(ClearChargingProfileRequest2);

        #endregion

        #region Operator <  (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A ClearChargingProfile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another ClearChargingProfile request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ClearChargingProfileRequest ClearChargingProfileRequest1,
                                          ClearChargingProfileRequest ClearChargingProfileRequest2)

            => ClearChargingProfileRequest1.CompareTo(ClearChargingProfileRequest2) < 0;

        #endregion

        #region Operator <= (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A ClearChargingProfile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another ClearChargingProfile request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ClearChargingProfileRequest ClearChargingProfileRequest1,
                                           ClearChargingProfileRequest ClearChargingProfileRequest2)

            => ClearChargingProfileRequest1.CompareTo(ClearChargingProfileRequest2) <= 0;

        #endregion

        #region Operator >  (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A ClearChargingProfile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another ClearChargingProfile request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ClearChargingProfileRequest ClearChargingProfileRequest1,
                                          ClearChargingProfileRequest ClearChargingProfileRequest2)

            => ClearChargingProfileRequest1.CompareTo(ClearChargingProfileRequest2) > 0;

        #endregion

        #region Operator >= (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A ClearChargingProfile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another ClearChargingProfile request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ClearChargingProfileRequest ClearChargingProfileRequest1,
                                           ClearChargingProfileRequest ClearChargingProfileRequest2)

            => ClearChargingProfileRequest1.CompareTo(ClearChargingProfileRequest2) >= 0;

        #endregion

        #endregion

        #region IComparable<ClearChargingProfileRequest> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two ClearChargingProfile requests.
        /// </summary>
        /// <param name="Object">A ClearChargingProfile request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ClearChargingProfileRequest clearChargingProfileRequest
                   ? CompareTo(clearChargingProfileRequest)
                   : throw new ArgumentException("The given object is not a ClearChargingProfile request!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ClearChargingProfileRequest)

        /// <summary>
        /// Compares two ClearChargingProfile requests.
        /// </summary>
        /// <param name="ClearChargingProfileRequest">A ClearChargingProfile request to compare with.</param>
        public Int32 CompareTo(ClearChargingProfileRequest? ClearChargingProfileRequest)
        {

            if (ClearChargingProfileRequest is null)
                throw new ArgumentNullException(nameof(ClearChargingProfileRequest), "The given ClearChargingProfile request must not be null!");

            return ChargingProfileId.CompareTo(ClearChargingProfileRequest.ChargingProfileId);

        }

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfileRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearChargingProfile requests for equality.
        /// </summary>
        /// <param name="Object">A ClearChargingProfile request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearChargingProfileRequest clearChargingProfileRequest &&
                   Equals(clearChargingProfileRequest);

        #endregion

        #region Equals(ClearChargingProfileRequest)

        /// <summary>
        /// Compares two ClearChargingProfile requests for equality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest">A ClearChargingProfile request to compare with.</param>
        public Boolean Equals(ClearChargingProfileRequest? ClearChargingProfileRequest)

            => ClearChargingProfileRequest is not null &&

               ChargingProfileId.Equals(ClearChargingProfileRequest.ChargingProfileId);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => ChargingProfileId.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"Clear charging profile '{ChargingProfileId}'";

        #endregion

    }

}
