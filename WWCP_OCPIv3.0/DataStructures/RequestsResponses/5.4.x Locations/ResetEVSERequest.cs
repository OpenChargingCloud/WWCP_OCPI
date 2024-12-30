/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A ResetEVSE request.
    /// </summary>
    public class ResetEVSERequest : AAsyncRequest<ResetEVSERequest>,
                                    IEquatable<ResetEVSERequest>,
                                    IComparable<ResetEVSERequest>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/locations/resetEVSERequest");

        #endregion

        #region Properties

        /// <summary>
        /// The ID of the Location at which a device is to be reset.
        /// </summary>
        [Mandatory]
        public Location_Id  LocationId    { get; }

        /// <summary>
        /// The value of the uid field of the EVSE of this Location for which the device is requested to be reset.
        /// </summary>
        [Mandatory]
        public EVSE_UId     EVSEUId       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ResetEVSE request.
        /// </summary>
        /// <param name="CallbackId">An identifier to relate a later asynchronous response to this request.</param>
        /// 
        /// <param name="LocationId">The ID of the Location at which a device is to be reset.</param>
        /// <param name="EVSEUId">The value of the uid field of the EVSE of this Location for which the device is requested to be reset.</param>
        public ResetEVSERequest(String       CallbackId,

                                Location_Id  LocationId,
                                EVSE_UId     EVSEUId)

            : base(CallbackId)

        {

            this.LocationId  = LocationId;
            this.EVSEUId     = EVSEUId;

            unchecked
            {

                hashCode = this.CallbackId.GetHashCode() * 5 ^
                           this.LocationId.GetHashCode() * 3 ^
                           this.EVSEUId.   GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of a ResetEVSE request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomResetEVSERequestParser">A delegate to parse custom ResetEVSE request JSON objects.</param>
        public static ResetEVSERequest Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<ResetEVSERequest>?  CustomResetEVSERequestParser   = null)
        {

            if (TryParse(JSON,
                         out var resetEVSERequest,
                         out var errorResponse,
                         CustomResetEVSERequestParser))
            {
                return resetEVSERequest;
            }

            throw new ArgumentException("The given JSON representation of a ResetEVSE request is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ResetEVSERequest, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a ResetEVSE request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ResetEVSERequest">The parsed ResetEVSE request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out ResetEVSERequest?  ResetEVSERequest,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out ResetEVSERequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a ResetEVSE request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ResetEVSERequest">The parsed ResetEVSE request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomResetEVSERequestParser">A delegate to parse custom ResetEVSE request JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out ResetEVSERequest?      ResetEVSERequest,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<ResetEVSERequest>?  CustomResetEVSERequestParser)
        {

            try
            {

                ResetEVSERequest = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CallbackId    [mandatory]

                if (!JSON.ParseMandatoryText("callback_id",
                                             "callback identification",
                                             out String? CallbackId,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse LocationId    [mandatory]

                if (!JSON.ParseMandatory("location_id",
                                         "location identification",
                                         Location_Id.TryParse,
                                         out Location_Id LocationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEUId       [mandatory]

                if (!JSON.ParseMandatory("evse_uid",
                                         "EVSE unique identification",
                                         EVSE_UId.TryParse,
                                         out EVSE_UId EVSEUId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ResetEVSERequest = new ResetEVSERequest(

                                       CallbackId,

                                       LocationId,
                                       EVSEUId

                                   );


                if (CustomResetEVSERequestParser is not null)
                    ResetEVSERequest = CustomResetEVSERequestParser(JSON,
                                                                    ResetEVSERequest);

                return true;

            }
            catch (Exception e)
            {
                ResetEVSERequest  = default;
                ErrorResponse     = "The given JSON representation of a ResetEVSE request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomResetEVSERequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomResetEVSERequestSerializer">A delegate to serialize custom ResetEVSE requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ResetEVSERequest>? CustomResetEVSERequestSerializer = null)
        {

            var json = ToJSON(
                           JSONObject.Create(
                               new JProperty("location_id",  LocationId.ToString()),
                               new JProperty("evse_uid",     EVSEUId.   ToString())
                           )
                       );

            return CustomResetEVSERequestSerializer is not null
                       ? CustomResetEVSERequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this ResetEVSE request.
        /// </summary>
        public ResetEVSERequest Clone()

            => new (

                   CallbackId.CloneString(),

                   LocationId.Clone(),
                   EVSEUId.   Clone()

               );

        #endregion


        #region Operator overloading

        #region Operator == (ResetEVSERequest1, ResetEVSERequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetEVSERequest1">A ResetEVSE request.</param>
        /// <param name="ResetEVSERequest2">Another ResetEVSE request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ResetEVSERequest ResetEVSERequest1,
                                           ResetEVSERequest ResetEVSERequest2)

            => ResetEVSERequest1.Equals(ResetEVSERequest2);

        #endregion

        #region Operator != (ResetEVSERequest1, ResetEVSERequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetEVSERequest1">A ResetEVSE request.</param>
        /// <param name="ResetEVSERequest2">Another ResetEVSE request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ResetEVSERequest ResetEVSERequest1,
                                           ResetEVSERequest ResetEVSERequest2)

            => !ResetEVSERequest1.Equals(ResetEVSERequest2);

        #endregion

        #region Operator <  (ResetEVSERequest1, ResetEVSERequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetEVSERequest1">A ResetEVSE request.</param>
        /// <param name="ResetEVSERequest2">Another ResetEVSE request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ResetEVSERequest ResetEVSERequest1,
                                          ResetEVSERequest ResetEVSERequest2)

            => ResetEVSERequest1.CompareTo(ResetEVSERequest2) < 0;

        #endregion

        #region Operator <= (ResetEVSERequest1, ResetEVSERequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetEVSERequest1">A ResetEVSE request.</param>
        /// <param name="ResetEVSERequest2">Another ResetEVSE request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ResetEVSERequest ResetEVSERequest1,
                                           ResetEVSERequest ResetEVSERequest2)

            => ResetEVSERequest1.CompareTo(ResetEVSERequest2) <= 0;

        #endregion

        #region Operator >  (ResetEVSERequest1, ResetEVSERequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetEVSERequest1">A ResetEVSE request.</param>
        /// <param name="ResetEVSERequest2">Another ResetEVSE request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ResetEVSERequest ResetEVSERequest1,
                                          ResetEVSERequest ResetEVSERequest2)

            => ResetEVSERequest1.CompareTo(ResetEVSERequest2) > 0;

        #endregion

        #region Operator >= (ResetEVSERequest1, ResetEVSERequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetEVSERequest1">A ResetEVSE request.</param>
        /// <param name="ResetEVSERequest2">Another ResetEVSE request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ResetEVSERequest ResetEVSERequest1,
                                           ResetEVSERequest ResetEVSERequest2)

            => ResetEVSERequest1.CompareTo(ResetEVSERequest2) >= 0;

        #endregion

        #endregion

        #region IComparable<ResetEVSERequest> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two ResetEVSE requests.
        /// </summary>
        /// <param name="Object">A ResetEVSE request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ResetEVSERequest resetEVSERequest
                   ? CompareTo(resetEVSERequest)
                   : throw new ArgumentException("The given object is not a ResetEVSE request object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ResetEVSERequest)

        /// <summary>
        /// Compares two ResetEVSE requests.
        /// </summary>
        /// <param name="ResetEVSERequest">A ResetEVSE request to compare with.</param>
        public Int32 CompareTo(ResetEVSERequest? ResetEVSERequest)
        {

            if (ResetEVSERequest is null)
                throw new ArgumentNullException(nameof(ResetEVSERequest), "The given ResetEVSE request object must not be null!");

            var c = LocationId.CompareTo(ResetEVSERequest.LocationId);

            if (c == 0)
                c = EVSEUId.   CompareTo(ResetEVSERequest.EVSEUId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ResetEVSERequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ResetEVSE requests for equality.
        /// </summary>
        /// <param name="Object">A ResetEVSE request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ResetEVSERequest resetEVSERequest &&
                   Equals(resetEVSERequest);

        #endregion

        #region Equals(ResetEVSERequest)

        /// <summary>
        /// Compares two ResetEVSE requests for equality.
        /// </summary>
        /// <param name="ResetEVSERequest">A ResetEVSE request to compare with.</param>
        public Boolean Equals(ResetEVSERequest? ResetEVSERequest)

            => ResetEVSERequest is not null &&

               LocationId.Equals(ResetEVSERequest.LocationId) &&
               EVSEUId.   Equals(ResetEVSERequest.EVSEUId);

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

            => $"{LocationId}/{EVSEUId}";

        #endregion

    }

}
