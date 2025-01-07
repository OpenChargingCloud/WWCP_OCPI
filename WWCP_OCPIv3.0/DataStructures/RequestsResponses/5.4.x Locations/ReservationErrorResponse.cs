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

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A reservation error response.
    /// </summary>
    public class ReservationErrorResponse : AAsyncResponse<ReserveNowRequest, ReservationErrorResponse>,
                                            IEquatable<ReservationErrorResponse>,
                                            IComparable<ReservationErrorResponse>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/locations/reservationErrorResponse");

        #endregion

        #region Properties

        /// <summary>
        /// An error code that signals why the reservation request failed.
        /// </summary>
        [Mandatory]
        public ReservationStatus  Status     { get; }

        /// <summary>
        /// Human-readable description of the reason for the status (if one can be provided),
        /// multiple languages can be provided.
        /// </summary>
        [Optional]
        public DisplayTexts?      Message    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new reservation error response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="ResultType">The completion status of the requested asynchronous remote procedure call.</param>
        /// 
        /// <param name="Status">An error code that signals why the reservation request failed.</param>
        /// <param name="Message">Human-readable description of the reason for the status (if one can be provided), multiple languages can be provided.</param>
        public ReservationErrorResponse(ReserveNowRequest  Request,
                                        AsyncResultType    ResultType,

                                        ReservationStatus  Status,
                                        DisplayTexts?      Message = null)

            : base(Request,
                   ResultType)

        {

            this.Status   = Status;
            this.Message  = Message;

            unchecked
            {

                hashCode = this.Request.   GetHashCode() * 7 ^
                           this.ResultType.GetHashCode() * 5 ^
                           this.Status.    GetHashCode() * 3 ^
                           this.Message?.  GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (Request, JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of a reservation error response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomReservationErrorResponseParser">A delegate to parse custom reservation error response JSON objects.</param>
        public static ReservationErrorResponse Parse(ReserveNowRequest                                       Request,
                                                     JObject                                                 JSON,
                                                     CustomJObjectParserDelegate<ReservationErrorResponse>?  CustomReservationErrorResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var reservationErrorResponse,
                         out var errorResponse,
                         CustomReservationErrorResponseParser))
            {
                return reservationErrorResponse;
            }

            throw new ArgumentException("The given JSON representation of a reservation error response is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ReservationErrorResponse, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a reservation error response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ReservationErrorResponse">The parsed reservation error response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(ReserveNowRequest                                   Request,
                                       JObject                                             JSON,
                                       [NotNullWhen(true)]  out ReservationErrorResponse?  ReservationErrorResponse,
                                       [NotNullWhen(false)] out String?                    ErrorResponse)

            => TryParse(Request,
                        JSON,
                        out ReservationErrorResponse,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a reservation error response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ReservationErrorResponse">The parsed reservation error response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomReservationErrorResponseParser">A delegate to parse custom reservation error response JSON objects.</param>
        public static Boolean TryParse(ReserveNowRequest                                       Request,
                                       JObject                                                 JSON,
                                       [NotNullWhen(true)]  out ReservationErrorResponse?      ReservationErrorResponse,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       CustomJObjectParserDelegate<ReservationErrorResponse>?  CustomReservationErrorResponseParser)
        {

            try
            {
                ReservationErrorResponse = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ResultType    [mandatory]

                if (!JSON.ParseMandatory("result_type",
                                         "result type",
                                         AsyncResultType.TryParse,
                                         out AsyncResultType ResultType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Payload       [mandatory]

                if (JSON["payload"] is not JObject payload)
                {
                    ErrorResponse = "The async response did not contain a payload!";
                    return false;
                }

                #endregion

                #region Parse Error         [mandatory]

                if (JSON["error"] is not JObject error)
                {
                    ErrorResponse = "The async response did not contain an error payload!";
                    return false;
                }

                #endregion


                #region Parse Status        [mandatory]

                if (!error.ParseMandatory("status",
                                          "reservation status",
                                          ReservationStatus.TryParse,
                                          out ReservationStatus Status,
                                          out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Message       [optional]

                if (error.ParseOptionalJSONArray("message",
                                                 "error message",
                                                 DisplayTexts.TryParse,
                                                 out DisplayTexts? Message,
                                                 out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ReservationErrorResponse = new ReservationErrorResponse(

                                               Request,
                                               ResultType,

                                               Status,
                                               Message

                                           );


                if (CustomReservationErrorResponseParser is not null)
                    ReservationErrorResponse = CustomReservationErrorResponseParser(JSON,
                                                                    ReservationErrorResponse);

                return true;

            }
            catch (Exception e)
            {
                ReservationErrorResponse  = default;
                ErrorResponse     = "The given JSON representation of a reservation error response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomReservationErrorResponseSerializer = null, CustomTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReservationErrorResponseSerializer">A delegate to serialize custom reservation error responses.</param>
        /// <param name="CustomTokenSerializer">A delegate to serialize custom token JSON objects.</param>
        /// <param name="CustomEnergyContractSerializer">A delegate to serialize custom energy contract JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ReservationErrorResponse>?  CustomReservationErrorResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Token>?              CustomTokenSerializer               = null,
                              CustomJObjectSerializerDelegate<EnergyContract>?     CustomEnergyContractSerializer      = null)
        {

            var json = ToJSON(
                           Payload:  null,
                           Error:    JSONObject.Create(

                                               new JProperty("status",    Status. ToString()),

                                         Message is not null && Message.Any()
                                             ? new JProperty("message",   Message.ToJSON())
                                             : null

                                     )
                       );

            return CustomReservationErrorResponseSerializer is not null
                       ? CustomReservationErrorResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this reservation error response.
        /// </summary>
        public ReservationErrorResponse Clone()

            => new (

                   Request.   Clone(),
                   ResultType.Clone(),

                   Status.    Clone(),
                   Message?.  Clone()

               );

        #endregion


        #region Operator overloading

        #region Operator == (ReservationErrorResponse1, ReservationErrorResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationErrorResponse1">A reservation error response.</param>
        /// <param name="ReservationErrorResponse2">Another reservation error response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ReservationErrorResponse ReservationErrorResponse1,
                                           ReservationErrorResponse ReservationErrorResponse2)

            => ReservationErrorResponse1.Equals(ReservationErrorResponse2);

        #endregion

        #region Operator != (ReservationErrorResponse1, ReservationErrorResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationErrorResponse1">A reservation error response.</param>
        /// <param name="ReservationErrorResponse2">Another reservation error response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ReservationErrorResponse ReservationErrorResponse1,
                                           ReservationErrorResponse ReservationErrorResponse2)

            => !ReservationErrorResponse1.Equals(ReservationErrorResponse2);

        #endregion

        #region Operator <  (ReservationErrorResponse1, ReservationErrorResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationErrorResponse1">A reservation error response.</param>
        /// <param name="ReservationErrorResponse2">Another reservation error response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ReservationErrorResponse ReservationErrorResponse1,
                                          ReservationErrorResponse ReservationErrorResponse2)

            => ReservationErrorResponse1.CompareTo(ReservationErrorResponse2) < 0;

        #endregion

        #region Operator <= (ReservationErrorResponse1, ReservationErrorResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationErrorResponse1">A reservation error response.</param>
        /// <param name="ReservationErrorResponse2">Another reservation error response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ReservationErrorResponse ReservationErrorResponse1,
                                           ReservationErrorResponse ReservationErrorResponse2)

            => ReservationErrorResponse1.CompareTo(ReservationErrorResponse2) <= 0;

        #endregion

        #region Operator >  (ReservationErrorResponse1, ReservationErrorResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationErrorResponse1">A reservation error response.</param>
        /// <param name="ReservationErrorResponse2">Another reservation error response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ReservationErrorResponse ReservationErrorResponse1,
                                          ReservationErrorResponse ReservationErrorResponse2)

            => ReservationErrorResponse1.CompareTo(ReservationErrorResponse2) > 0;

        #endregion

        #region Operator >= (ReservationErrorResponse1, ReservationErrorResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationErrorResponse1">A reservation error response.</param>
        /// <param name="ReservationErrorResponse2">Another reservation error response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ReservationErrorResponse ReservationErrorResponse1,
                                           ReservationErrorResponse ReservationErrorResponse2)

            => ReservationErrorResponse1.CompareTo(ReservationErrorResponse2) >= 0;

        #endregion

        #endregion

        #region IComparable<ReservationErrorResponse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two reservation error responses.
        /// </summary>
        /// <param name="Object">A reservation error response to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ReservationErrorResponse reservationErrorResponse
                   ? CompareTo(reservationErrorResponse)
                   : throw new ArgumentException("The given object is not a reservation error response!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ReservationErrorResponse)

        /// <summary>
        /// Compares two reservation error responses.
        /// </summary>
        /// <param name="ReservationErrorResponse">A reservation error response to compare with.</param>
        public Int32 CompareTo(ReservationErrorResponse? ReservationErrorResponse)
        {

            if (ReservationErrorResponse is null)
                throw new ArgumentNullException(nameof(ReservationErrorResponse), "The given reservation error response must not be null!");

            var c = ResultType.CompareTo(ReservationErrorResponse.ResultType);

            if (c == 0)
                c = Status.    CompareTo(ReservationErrorResponse.Status);

            if (c == 0)
                c = Message?.  CompareTo(ReservationErrorResponse.Message) ?? (ReservationErrorResponse.Message is null ? 0 : -1);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ReservationErrorResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two reservation error responses for equality.
        /// </summary>
        /// <param name="Object">A reservation error response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReservationErrorResponse reservationErrorResponse &&
                   Equals(reservationErrorResponse);

        #endregion

        #region Equals(ReservationErrorResponse)

        /// <summary>
        /// Compares two reservation error responses for equality.
        /// </summary>
        /// <param name="ReservationErrorResponse">A reservation error response to compare with.</param>
        public Boolean Equals(ReservationErrorResponse? ReservationErrorResponse)

            => ReservationErrorResponse is not null &&

               Status.    Equals(ReservationErrorResponse.Status)     &&
               ResultType.Equals(ReservationErrorResponse.ResultType) &&

             ((Message is     null && ReservationErrorResponse.Message is     null) ||
              (Message is not null && ReservationErrorResponse.Message is not null && Message.Equals(ReservationErrorResponse.Message)));

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

                   $"{ResultType} / {Status}",

                   Message is not null && Message.Any()
                       ? $", {Message.First()}"
                       : ""

               );

        #endregion

    }

}
