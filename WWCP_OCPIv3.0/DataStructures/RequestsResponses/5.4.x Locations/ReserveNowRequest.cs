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
    /// A ReserveNow request.
    /// </summary>
    public class ReserveNowRequest : AAsyncRequest<ReserveNowRequest>,
                                     IEquatable<ReserveNowRequest>,
                                     IComparable<ReserveNowRequest>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/locations/reserveNowRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The Token for which Party Y should reserve an EVSE.
        /// </summary>
        [Mandatory]
        public Token                    Token                     { get; }

        /// <summary>
        /// The point in time at which this reservation ends.
        /// </summary>
        [Mandatory]
        public DateTime                 ExpiryDate                { get; }

        /// <summary>
        /// An identifier for this reservation.
        /// </summary>
        [Mandatory]
        public Reservation_Id           ReservationId             { get; }

        /// <summary>
        /// Location ID of the Location (belonging to the CPO this request is sent to)
        /// for which to reserve an EVSE.
        /// </summary>
        [Mandatory]
        public Location_Id              LocationId                { get; }

        /// <summary>
        /// Optional EVSE UID of the EVSE to reserve if a specific EVSE has to be reserved.
        /// </summary>
        [Optional]
        public EVSE_UId?                EVSEUId                   { get; }

        /// <summary>
        /// Reference to the authorization given by the eMSP.
        /// When given, this reference will be provided in the Session and/or CDR that
        /// may eventually be transferred from Party Y to Party X as a result of this
        /// reservation request.
        /// </summary>
        [Optional]
        public AuthorizationReference?  AuthorizationReference    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ReserveNow request.
        /// </summary>
        /// <param name="CallbackId">An identifier to relate a later asynchronous response to this request.</param>
        /// 
        /// <param name="Token">The Token for which Party Y should reserve an EVSE.</param>
        /// <param name="ExpiryDate">The point in time at which this reservation ends.</param>
        /// <param name="ReservationId">An identifier for this reservation.</param>
        /// <param name="LocationId">Location ID of the Location (belonging to the CPO this request is sent to) for which to reserve an EVSE.</param>
        /// <param name="EVSEUId">Optional EVSE UID of the EVSE to reserve if a specific EVSE has to be reserved.</param>
        /// <param name="AuthorizationReference">Reference to the authorization given by the eMSP.</param>
        public ReserveNowRequest(String                   CallbackId,

                                 Token                    Token,
                                 DateTime                 ExpiryDate,
                                 Reservation_Id           ReservationId,
                                 Location_Id              LocationId,
                                 EVSE_UId?                EVSEUId                  = null,
                                 AuthorizationReference?  AuthorizationReference   = null)

            : base(CallbackId)

        {

            this.Token                   = Token;
            this.ExpiryDate              = ExpiryDate;
            this.ReservationId           = ReservationId;
            this.LocationId              = LocationId;
            this.EVSEUId                 = EVSEUId;
            this.AuthorizationReference  = AuthorizationReference;

            unchecked
            {

                hashCode = this.CallbackId.             GetHashCode()       * 17 ^
                           this.Token.                  GetHashCode()       * 13 ^
                           this.ExpiryDate.             GetHashCode()       * 11 ^
                           this.ReservationId.          GetHashCode()       *  7 ^
                           this.LocationId.             GetHashCode()       *  5 ^
                          (this.EVSEUId?.               GetHashCode() ?? 0) *  3 ^
                           this.AuthorizationReference?.GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of a ReserveNow request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomReserveNowRequestParser">A delegate to parse custom ReserveNow request JSON objects.</param>
        public static ReserveNowRequest Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<ReserveNowRequest>?  CustomReserveNowRequestParser   = null)
        {

            if (TryParse(JSON,
                         out var reserveNowRequest,
                         out var errorResponse,
                         CustomReserveNowRequestParser))
            {
                return reserveNowRequest;
            }

            throw new ArgumentException("The given JSON representation of a ReserveNow request is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ReserveNowRequest, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a ReserveNow request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ReserveNowRequest">The parsed ReserveNow request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out ReserveNowRequest?  ReserveNowRequest,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out ReserveNowRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a ReserveNow request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ReserveNowRequest">The parsed ReserveNow request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomReserveNowRequestParser">A delegate to parse custom ReserveNow request JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out ReserveNowRequest?      ReserveNowRequest,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<ReserveNowRequest>?  CustomReserveNowRequestParser)
        {

            try
            {

                ReserveNowRequest = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CallbackId                [mandatory]

                if (!JSON.ParseMandatoryText("callback_id",
                                             "callback identification",
                                             out String? CallbackId,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse Token                     [mandatory]

                if (!JSON.ParseMandatoryJSON("token",
                                             "authentication token",
                                             OCPIv3_0.Token.TryParse,
                                             out Token? Token,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ExpiryDate                [mandatory]

                if (!JSON.ParseMandatory("expiry_date",
                                         "expiry date",
                                         out DateTime ExpiryDate,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ReservationId             [mandatory]

                if (!JSON.ParseMandatory("reservation_id",
                                         "reservation identification",
                                         Reservation_Id.TryParse,
                                         out Reservation_Id ReservationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse LocationId                [mandatory]

                if (!JSON.ParseMandatory("location_id",
                                         "location identification",
                                         Location_Id.TryParse,
                                         out Location_Id LocationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEUId                   [optional]

                if (JSON.ParseOptional("evse_uid",
                                       "EVSE unique identification",
                                       EVSE_UId.TryParse,
                                       out EVSE_UId? EVSEUId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse AuthorizationReference    [optional]

                if (JSON.ParseOptional("authorization_reference",
                                       "authorization reference",
                                       OCPIv3_0.AuthorizationReference.TryParse,
                                       out AuthorizationReference? AuthorizationReference,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ReserveNowRequest = new ReserveNowRequest(

                                        CallbackId,

                                        Token,
                                        ExpiryDate,
                                        ReservationId,
                                        LocationId,
                                        EVSEUId,
                                        AuthorizationReference

                                    );


                if (CustomReserveNowRequestParser is not null)
                    ReserveNowRequest = CustomReserveNowRequestParser(JSON,
                                                                      ReserveNowRequest);

                return true;

            }
            catch (Exception e)
            {
                ReserveNowRequest  = default;
                ErrorResponse      = "The given JSON representation of a ReserveNow request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomReserveNowRequestSerializer = null, CustomTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReserveNowRequestSerializer">A delegate to serialize custom ReserveNow requests.</param>
        /// <param name="CustomTokenSerializer">A delegate to serialize custom token JSON objects.</param>
        /// <param name="CustomEnergyContractSerializer">A delegate to serialize custom energy contract JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ReserveNowRequest>?  CustomReserveNowRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Token>?              CustomTokenSerializer               = null,
                              CustomJObjectSerializerDelegate<EnergyContract>?     CustomEnergyContractSerializer      = null)
        {

            var json = ToJSON(
                           JSONObject.Create(

                                     new JProperty("token",                     Token.                       ToJSON(true,
                                                                                                                    true,
                                                                                                                    true,
                                                                                                                    true,
                                                                                                                    CustomTokenSerializer,
                                                                                                                    CustomEnergyContractSerializer)),
                                     new JProperty("expiry_date",               ExpiryDate.                  ToIso8601()),
                                     new JProperty("reservation_id",            ReservationId.               ToString()),
                                     new JProperty("location_id",               LocationId.                  ToString()),

                               EVSEUId.HasValue
                                   ? new JProperty("evse_uid",                  EVSEUId.               Value.ToString())
                                   : null,

                               AuthorizationReference.HasValue
                                   ? new JProperty("authorization_reference",   AuthorizationReference.Value.ToString())
                                   : null

                           )
                       );

            return CustomReserveNowRequestSerializer is not null
                       ? CustomReserveNowRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this ReserveNow request.
        /// </summary>
        public ReserveNowRequest Clone()

            => new (

                   CallbackId.             CloneString(),

                   Token.                  Clone(),
                   ExpiryDate,
                   ReservationId.          Clone(),
                   LocationId.             Clone(),
                   EVSEUId?.               Clone(),
                   AuthorizationReference?.Clone()

               );

        #endregion


        #region Operator overloading

        #region Operator == (ReserveNowRequest1, ReserveNowRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReserveNowRequest1">A ReserveNow request.</param>
        /// <param name="ReserveNowRequest2">Another ReserveNow request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ReserveNowRequest ReserveNowRequest1,
                                           ReserveNowRequest ReserveNowRequest2)

            => ReserveNowRequest1.Equals(ReserveNowRequest2);

        #endregion

        #region Operator != (ReserveNowRequest1, ReserveNowRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReserveNowRequest1">A ReserveNow request.</param>
        /// <param name="ReserveNowRequest2">Another ReserveNow request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ReserveNowRequest ReserveNowRequest1,
                                           ReserveNowRequest ReserveNowRequest2)

            => !ReserveNowRequest1.Equals(ReserveNowRequest2);

        #endregion

        #region Operator <  (ReserveNowRequest1, ReserveNowRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReserveNowRequest1">A ReserveNow request.</param>
        /// <param name="ReserveNowRequest2">Another ReserveNow request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ReserveNowRequest ReserveNowRequest1,
                                          ReserveNowRequest ReserveNowRequest2)

            => ReserveNowRequest1.CompareTo(ReserveNowRequest2) < 0;

        #endregion

        #region Operator <= (ReserveNowRequest1, ReserveNowRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReserveNowRequest1">A ReserveNow request.</param>
        /// <param name="ReserveNowRequest2">Another ReserveNow request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ReserveNowRequest ReserveNowRequest1,
                                           ReserveNowRequest ReserveNowRequest2)

            => ReserveNowRequest1.CompareTo(ReserveNowRequest2) <= 0;

        #endregion

        #region Operator >  (ReserveNowRequest1, ReserveNowRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReserveNowRequest1">A ReserveNow request.</param>
        /// <param name="ReserveNowRequest2">Another ReserveNow request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ReserveNowRequest ReserveNowRequest1,
                                          ReserveNowRequest ReserveNowRequest2)

            => ReserveNowRequest1.CompareTo(ReserveNowRequest2) > 0;

        #endregion

        #region Operator >= (ReserveNowRequest1, ReserveNowRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReserveNowRequest1">A ReserveNow request.</param>
        /// <param name="ReserveNowRequest2">Another ReserveNow request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ReserveNowRequest ReserveNowRequest1,
                                           ReserveNowRequest ReserveNowRequest2)

            => ReserveNowRequest1.CompareTo(ReserveNowRequest2) >= 0;

        #endregion

        #endregion

        #region IComparable<ReserveNowRequest> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two ReserveNow requests.
        /// </summary>
        /// <param name="Object">A ReserveNow request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ReserveNowRequest reserveNowRequest
                   ? CompareTo(reserveNowRequest)
                   : throw new ArgumentException("The given object is not a ReserveNow request object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ReserveNowRequest)

        /// <summary>
        /// Compares two ReserveNow requests.
        /// </summary>
        /// <param name="ReserveNowRequest">A ReserveNow request to compare with.</param>
        public Int32 CompareTo(ReserveNowRequest? ReserveNowRequest)
        {

            if (ReserveNowRequest is null)
                throw new ArgumentNullException(nameof(ReserveNowRequest), "The given ReserveNow request object must not be null!");

            var c = ExpiryDate.CompareTo(ReserveNowRequest.ExpiryDate);

            if (c == 0)
                c = ReservationId.CompareTo(ReserveNowRequest.ReservationId);

            if (c == 0)
                c = LocationId.CompareTo(ReserveNowRequest.LocationId);

            if (c == 0 && EVSEUId.HasValue && ReserveNowRequest.EVSEUId.HasValue)
                c = EVSEUId.Value.CompareTo(ReserveNowRequest.EVSEUId.Value);

            if (c == 0 && AuthorizationReference.HasValue && ReserveNowRequest.AuthorizationReference.HasValue)
                c = AuthorizationReference.Value.CompareTo(ReserveNowRequest.AuthorizationReference.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ReserveNowRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ReserveNow requests for equality.
        /// </summary>
        /// <param name="Object">A ReserveNow request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReserveNowRequest reserveNowRequest &&
                   Equals(reserveNowRequest);

        #endregion

        #region Equals(ReserveNowRequest)

        /// <summary>
        /// Compares two ReserveNow requests for equality.
        /// </summary>
        /// <param name="ReserveNowRequest">A ReserveNow request to compare with.</param>
        public Boolean Equals(ReserveNowRequest? ReserveNowRequest)

            => ReserveNowRequest is not null &&

               Token.        Equals(ReserveNowRequest.Token)         &&
               ExpiryDate.   Equals(ReserveNowRequest.ExpiryDate)    &&
               ReservationId.Equals(ReserveNowRequest.ReservationId) &&
               LocationId.   Equals(ReserveNowRequest.LocationId)    &&

            ((!EVSEUId.               HasValue && !ReserveNowRequest.EVSEUId.               HasValue) ||
              (EVSEUId.               HasValue &&  ReserveNowRequest.EVSEUId.               HasValue && EVSEUId.               Value.Equals(ReserveNowRequest.EVSEUId.Value))) &&

            ((!AuthorizationReference.HasValue && !ReserveNowRequest.AuthorizationReference.HasValue) ||
              (AuthorizationReference.HasValue &&  ReserveNowRequest.AuthorizationReference.HasValue && AuthorizationReference.Value.Equals(ReserveNowRequest.AuthorizationReference.Value)));

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

            => String.Concat(

                   $"{ReservationId} for {Token.Id} @ {LocationId}",

                   EVSEUId.HasValue
                       ? $"/{EVSEUId}"
                       : ""

               );

        #endregion

    }

}
