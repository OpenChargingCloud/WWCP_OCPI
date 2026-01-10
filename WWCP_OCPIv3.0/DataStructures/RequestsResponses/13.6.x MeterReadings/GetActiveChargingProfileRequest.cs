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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A GetActiveChargingProfile request.
    /// </summary>
    public class GetActiveChargingProfileRequest : AAsyncRequest<GetActiveChargingProfileRequest>,
                                                   IEquatable<GetActiveChargingProfileRequest>,
                                                   IComparable<GetActiveChargingProfileRequest>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/meterreadings/getActiveChargingProfileRequestRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The unique ID that identifies the Charging Profile for which an
        /// Active Charging Profile is requested to the CPO.
        /// </summary>
        [Mandatory]
        public ChargingProfile_Id  ChargingProfileId    { get; }

        /// <summary>
        /// Length of the requested ActiveChargingProfile in seconds.
        /// </summary>
        [Mandatory]
        public TimeSpan            Duration             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetActiveChargingProfile request.
        /// </summary>
        /// <param name="CallbackId">An identifier to relate a later asynchronous response to this request.</param>
        /// 
        /// <param name="ChargingProfileId">The unique ID that identifies the Charging Profile for which an Active Charging Profile is requested to the CPO.</param>
        /// <param name="Duration">Length of the requested ActiveChargingProfile in seconds.</param>
        public GetActiveChargingProfileRequest(String              CallbackId,

                                               ChargingProfile_Id  ChargingProfileId,
                                               TimeSpan            Duration)

            : base(CallbackId)

        {

            this.ChargingProfileId  = ChargingProfileId;
            this.Duration           = Duration;

            unchecked
            {

                hashCode = this.ChargingProfileId.GetHashCode() * 3 ^
                           this.Duration.         GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetActiveChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomGetActiveChargingProfileRequestParser">A delegate to parse custom GetActiveChargingProfile request JSON objects.</param>
        public static GetActiveChargingProfileRequest Parse(JObject                                                        JSON,
                                                            CustomJObjectParserDelegate<GetActiveChargingProfileRequest>?  CustomGetActiveChargingProfileRequestParser   = null)
        {

            if (TryParse(JSON,
                         out var getActiveChargingProfileRequest,
                         out var errorResponse,
                         CustomGetActiveChargingProfileRequestParser))
            {
                return getActiveChargingProfileRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetActiveChargingProfile request is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out GetActiveChargingProfileRequest, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a GetActiveChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="GetActiveChargingProfileRequest">The parsed GetActiveChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       [NotNullWhen(true)]  out GetActiveChargingProfileRequest?  GetActiveChargingProfileRequest,
                                       [NotNullWhen(false)] out String?                           ErrorResponse)

            => TryParse(JSON,
                        out GetActiveChargingProfileRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a GetActiveChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="GetActiveChargingProfileRequest">The parsed GetActiveChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetActiveChargingProfileRequestParser">A delegate to parse custom GetActiveChargingProfile request JSON objects.</param>
        public static Boolean TryParse(JObject                                                        JSON,
                                       [NotNullWhen(true)]  out GetActiveChargingProfileRequest?      GetActiveChargingProfileRequest,
                                       [NotNullWhen(false)] out String?                               ErrorResponse,
                                       CustomJObjectParserDelegate<GetActiveChargingProfileRequest>?  CustomGetActiveChargingProfileRequestParser)
        {

            try
            {

                GetActiveChargingProfileRequest = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CallbackId           [mandatory]

                if (!JSON.ParseMandatoryText("callback_id",
                                             "callback identification",
                                             out String? CallbackId,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


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

                #region Parse Duration             [mandatory]

                if (!JSON.ParseMandatory("duration",
                                         "duration",
                                         out TimeSpan Duration,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                GetActiveChargingProfileRequest = new GetActiveChargingProfileRequest(

                                                      CallbackId,

                                                      ChargingProfileId,
                                                      Duration

                                                  );


                if (CustomGetActiveChargingProfileRequestParser is not null)
                    GetActiveChargingProfileRequest = CustomGetActiveChargingProfileRequestParser(JSON,
                                                                                                  GetActiveChargingProfileRequest);

                return true;

            }
            catch (Exception e)
            {
                GetActiveChargingProfileRequest  = default;
                ErrorResponse                    = "The given JSON representation of a GetActiveChargingProfile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetActiveChargingProfileRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetActiveChargingProfileRequestSerializer">A delegate to serialize custom GetActiveChargingProfile requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetActiveChargingProfileRequest>? CustomGetActiveChargingProfileRequestSerializer = null)
        {

            var json = ToJSON(
                           JSONObject.Create(

                               new JProperty("charging_profile_id",   ChargingProfileId.ToString()),
                               new JProperty("duration",              (UInt32) Duration.TotalSeconds)

                           )
                       );

            return CustomGetActiveChargingProfileRequestSerializer is not null
                       ? CustomGetActiveChargingProfileRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this GetActiveChargingProfile request.
        /// </summary>
        public GetActiveChargingProfileRequest Clone()

            => new (

                   CallbackId.       CloneString(),

                   ChargingProfileId.Clone(),
                   Duration

               );

        #endregion


        #region Operator overloading

        #region Operator == (GetActiveChargingProfileRequest1, GetActiveChargingProfileRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetActiveChargingProfileRequest1">A GetActiveChargingProfile request.</param>
        /// <param name="GetActiveChargingProfileRequest2">Another GetActiveChargingProfile request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (GetActiveChargingProfileRequest GetActiveChargingProfileRequest1,
                                           GetActiveChargingProfileRequest GetActiveChargingProfileRequest2)

            => GetActiveChargingProfileRequest1.Equals(GetActiveChargingProfileRequest2);

        #endregion

        #region Operator != (GetActiveChargingProfileRequest1, GetActiveChargingProfileRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetActiveChargingProfileRequest1">A GetActiveChargingProfile request.</param>
        /// <param name="GetActiveChargingProfileRequest2">Another GetActiveChargingProfile request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (GetActiveChargingProfileRequest GetActiveChargingProfileRequest1,
                                           GetActiveChargingProfileRequest GetActiveChargingProfileRequest2)

            => !GetActiveChargingProfileRequest1.Equals(GetActiveChargingProfileRequest2);

        #endregion

        #region Operator <  (GetActiveChargingProfileRequest1, GetActiveChargingProfileRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetActiveChargingProfileRequest1">A GetActiveChargingProfile request.</param>
        /// <param name="GetActiveChargingProfileRequest2">Another GetActiveChargingProfile request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (GetActiveChargingProfileRequest GetActiveChargingProfileRequest1,
                                          GetActiveChargingProfileRequest GetActiveChargingProfileRequest2)

            => GetActiveChargingProfileRequest1.CompareTo(GetActiveChargingProfileRequest2) < 0;

        #endregion

        #region Operator <= (GetActiveChargingProfileRequest1, GetActiveChargingProfileRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetActiveChargingProfileRequest1">A GetActiveChargingProfile request.</param>
        /// <param name="GetActiveChargingProfileRequest2">Another GetActiveChargingProfile request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (GetActiveChargingProfileRequest GetActiveChargingProfileRequest1,
                                           GetActiveChargingProfileRequest GetActiveChargingProfileRequest2)

            => GetActiveChargingProfileRequest1.CompareTo(GetActiveChargingProfileRequest2) <= 0;

        #endregion

        #region Operator >  (GetActiveChargingProfileRequest1, GetActiveChargingProfileRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetActiveChargingProfileRequest1">A GetActiveChargingProfile request.</param>
        /// <param name="GetActiveChargingProfileRequest2">Another GetActiveChargingProfile request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (GetActiveChargingProfileRequest GetActiveChargingProfileRequest1,
                                          GetActiveChargingProfileRequest GetActiveChargingProfileRequest2)

            => GetActiveChargingProfileRequest1.CompareTo(GetActiveChargingProfileRequest2) > 0;

        #endregion

        #region Operator >= (GetActiveChargingProfileRequest1, GetActiveChargingProfileRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetActiveChargingProfileRequest1">A GetActiveChargingProfile request.</param>
        /// <param name="GetActiveChargingProfileRequest2">Another GetActiveChargingProfile request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (GetActiveChargingProfileRequest GetActiveChargingProfileRequest1,
                                           GetActiveChargingProfileRequest GetActiveChargingProfileRequest2)

            => GetActiveChargingProfileRequest1.CompareTo(GetActiveChargingProfileRequest2) >= 0;

        #endregion

        #endregion

        #region IComparable<GetActiveChargingProfileRequest> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two GetActiveChargingProfile requests.
        /// </summary>
        /// <param name="Object">A GetActiveChargingProfile request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is GetActiveChargingProfileRequest getActiveChargingProfileRequest
                   ? CompareTo(getActiveChargingProfileRequest)
                   : throw new ArgumentException("The given object is not a GetActiveChargingProfile request object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(GetActiveChargingProfileRequest)

        /// <summary>
        /// Compares two GetActiveChargingProfile requests.
        /// </summary>
        /// <param name="GetActiveChargingProfileRequest">A GetActiveChargingProfile request to compare with.</param>
        public Int32 CompareTo(GetActiveChargingProfileRequest? GetActiveChargingProfileRequest)
        {

            if (GetActiveChargingProfileRequest is null)
                throw new ArgumentNullException(nameof(GetActiveChargingProfileRequest), "The given GetActiveChargingProfile request object must not be null!");

            var c = ChargingProfileId.CompareTo(GetActiveChargingProfileRequest.ChargingProfileId);

            if (c == 0)
                c = Duration.         CompareTo(GetActiveChargingProfileRequest.Duration);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<GetActiveChargingProfileRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetActiveChargingProfile requests for equality.
        /// </summary>
        /// <param name="Object">A GetActiveChargingProfile request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetActiveChargingProfileRequest getActiveChargingProfileRequest &&
                   Equals(getActiveChargingProfileRequest);

        #endregion

        #region Equals(GetActiveChargingProfileRequest)

        /// <summary>
        /// Compares two GetActiveChargingProfile requests for equality.
        /// </summary>
        /// <param name="GetActiveChargingProfileRequest">A GetActiveChargingProfile request to compare with.</param>
        public Boolean Equals(GetActiveChargingProfileRequest? GetActiveChargingProfileRequest)

            => GetActiveChargingProfileRequest is not null &&

               ChargingProfileId.Equals(GetActiveChargingProfileRequest.ChargingProfileId) &&
               Duration.         Equals(GetActiveChargingProfileRequest.Duration);

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

            => $"{ChargingProfileId} with {Duration.TotalSeconds} secs.";

        #endregion

    }

}
