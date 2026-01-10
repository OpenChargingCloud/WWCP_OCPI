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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// A booking request.
    /// </summary>
    public class BookingRequest : IEquatable<BookingRequest>,
                                  IComparable<BookingRequest>,
                                  IComparable
    {

        #region Properties

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this booking request.
        /// </summary>
        [Mandatory]
        public CountryCode              CountryCode               { get; }

        /// <summary>
        /// The identification of the charge point operator that 'owns' this booking request
        /// (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id                 PartyId                   { get; }

        /// <summary>
        /// Request identification determined by the requesting party.
        /// The same request identification SHALL be used for all edits on booking.
        /// </summary>
        [Mandatory]
        public Request_Id               RequestId                 { get; }

        /// <summary>
        /// Optional selected parking specification for charging at this Location.
        /// </summary>
        [Optional]
        public BookableParkingOptions?  BookableParkingOptions    { get; }

        /// <summary>
        /// Location identification of the location object of this CPO, on which the reservation can be made.
        /// </summary>
        [Mandatory]
        public Location_Id              LocationId                { get; }

        /// <summary>
        /// Optional UID of a bookable EVSE. Only possible if stated in the bookable location.
        /// </summary>
        [Optional]
        public EVSE_UId?                EVSEUId                   { get; }

        /// <summary>
        /// The enumeration of tokens that can be used to utilise the booking.
        /// </summary>
        [Mandatory]
        public IEnumerable<Token_Id>    Tokens                    { get; }

        /// <summary>
        /// The timeslot for this booking.
        /// </summary>
        [Mandatory]
        public TimeSlot                 Period                    { get; }

        /// <summary>
        /// Authorization reference for the relevant session and charge detail record.
        /// </summary>
        [Mandatory]
        public AuthorizationReference   AuthorizationReference    { get; }

        /// <summary>
        /// The power requested for the reservation in kW.
        /// If it isn’t the maximum available the CPO can relocate the extra to another session.
        /// </summary>
        [Optional]
        public Watt?                    PowerRequired             { get; }

        /// <summary>
        /// To set when requesting to cancel the booking.
        /// </summary>
        [Optional]
        public Cancellation?            Canceled                  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new booking request.
        /// </summary>
        /// <param name="CountryCode">An ISO-3166 alpha-2 country code of the charge point operator that 'owns' this booking request.</param>
        /// <param name="PartyId">An identification of the charge point operator that 'owns' this booking request (following the ISO-15118 standard).</param>
        /// <param name="RequestId">An request identification determined by the requesting party. The same request identification SHALL be used for all edits on booking.</param>
        /// <param name="LocationId">A location identification of the location object of this CPO, on which the reservation can be made.</param>
        /// <param name="Tokens">An enumeration of tokens that can be used to utilise the booking.</param>
        /// <param name="Period">A timeslot for this booking.</param>
        /// <param name="AuthorizationReference">An authorization reference for the relevant session and charge detail record.</param>
        /// <param name="BookableParkingOptions">An optional selected parking specification for charging at this Location.</param>
        /// <param name="EVSEUId">An optional UID of a bookable EVSE. Only possible if stated in the bookable location.</param>
        /// <param name="PowerRequired">An optional power requested for the reservation in kW. If it isn’t the maximum available the CPO can relocate the extra to another session.</param>
        /// <param name="Canceled">An optional cancellation request.</param>
        public BookingRequest(CountryCode              CountryCode,
                              Party_Id                 PartyId,
                              Request_Id               RequestId,
                              Location_Id              LocationId,
                              IEnumerable<Token_Id>    Tokens,
                              TimeSlot                 Period,
                              AuthorizationReference   AuthorizationReference,
                              BookableParkingOptions?  BookableParkingOptions   = null,
                              EVSE_UId?                EVSEUId                  = null,
                              Watt?                    PowerRequired            = null,
                              Cancellation?            Canceled                 = null)
        {

            this.CountryCode             = CountryCode;
            this.PartyId                 = PartyId;
            this.RequestId               = RequestId;
            this.LocationId              = LocationId;
            this.Tokens                  = Tokens.Distinct();
            this.Period                  = Period;
            this.AuthorizationReference  = AuthorizationReference;
            this.BookableParkingOptions  = BookableParkingOptions;
            this.EVSEUId                 = EVSEUId;
            this.PowerRequired           = PowerRequired;
            this.Canceled                = Canceled;

            unchecked
            {

                hashCode = this.CountryCode.            GetHashCode()       * 37 ^
                           this.PartyId.                GetHashCode()       * 31 ^
                           this.RequestId.              GetHashCode()       * 29 ^
                           this.LocationId.             GetHashCode()       * 23 ^
                           this.Tokens.                 CalcHashCode()      * 19 ^
                           this.Period.                 GetHashCode()       * 17 ^
                           this.AuthorizationReference. GetHashCode()       * 13 ^
                          (this.BookableParkingOptions?.GetHashCode() ?? 0) *  7 ^
                          (this.EVSEUId?.               GetHashCode() ?? 0) *  5 ^
                          (this.PowerRequired?.         GetHashCode() ?? 0) *  3 ^
                           this.Canceled?.              GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomBookingRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a booking request request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomBookingRequestParser">A delegate to parse custom booking request request JSON objects.</param>
        public static BookingRequest Parse(JObject                                       JSON,
                                           CustomJObjectParserDelegate<BookingRequest>?  CustomBookingRequestParser   = null)
        {

            if (TryParse(JSON,
                         out var bookingRequest,
                         out var errorResponse,
                         CustomBookingRequestParser))
            {
                return bookingRequest;
            }

            throw new ArgumentException("The given JSON representation of a booking request request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out BookingRequest, out ErrorResponse, CustomBookingRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a booking request request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="BookingRequest">The parsed booking request request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out BookingRequest?  BookingRequest,
                                       [NotNullWhen(false)] out String?          ErrorResponse)

            => TryParse(JSON,
                        out BookingRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a booking request request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="BookingRequest">The parsed booking request request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBookingRequestParser">A delegate to parse custom booking request request JSON objects.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out BookingRequest?      BookingRequest,
                                       [NotNullWhen(false)] out String?              ErrorResponse,
                                       CustomJObjectParserDelegate<BookingRequest>?  CustomBookingRequestParser   = null)
        {

            try
            {

                BookingRequest = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CountryCode               [mandatory]

                if (!JSON.ParseMandatory("country_code",
                                         "country code",
                                         CountryCode.TryParse,
                                         out CountryCode countryCode,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PartyIdURL                [mandatory]

                if (!JSON.ParseMandatory("party_id",
                                         "party identification",
                                         Party_Id.TryParse,
                                         out Party_Id partyId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse RequestId                 [mandatory]

                if (!JSON.ParseMandatory("request_id",
                                         "request identification",
                                         Request_Id.TryParse,
                                         out Request_Id requestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse LocationId                [mandatory]

                if (!JSON.ParseMandatory("location_id",
                                         "location identification",
                                         Location_Id.TryParse,
                                         out Location_Id locationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Tokens                    [mandatory]

                if (!JSON.ParseMandatoryHashSet("tokens",
                                                "tokens",
                                                Token_Id.TryParse,
                                                out HashSet<Token_Id> tokens,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Period                    [mandatory]

                if (!JSON.ParseMandatoryJSON("period",
                                             "period",
                                             TimeSlot.TryParse,
                                             out TimeSlot? period,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthorizationReference    [mandatory]

                if (!JSON.ParseMandatory("authorization_reference",
                                         "authorization reference",
                                         AuthorizationReference.TryParse,
                                         out AuthorizationReference authorizationReference,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse BookableParkingOptions    [optional]

                if (JSON.ParseOptionalJSON("bookable_parking_option",
                                           "bookable parking option",
                                           BookableParkingOptions.TryParse,
                                           out BookableParkingOptions? bookableParkingOptions,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EVSEUId                   [optional]

                if (JSON.ParseOptional("evse_uid",
                                       "EVSE UId",
                                       EVSE_UId.TryParse,
                                       out EVSE_UId? evseUId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PowerRequired             [optional]

                if (JSON.ParseOptional("power_required",
                                       "power required",
                                       Watt.TryParseKW,
                                       out Watt powerRequired,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Canceled                  [optional]

                if (JSON.ParseOptionalJSON("canceled",
                                           "canceled",
                                           Cancellation.TryParse,
                                           out Cancellation? canceled,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                BookingRequest = new BookingRequest(
                                     countryCode,
                                     partyId,
                                     requestId,
                                     locationId,
                                     tokens,
                                     period,
                                     authorizationReference,
                                     bookableParkingOptions,
                                     evseUId,
                                     powerRequired,
                                     canceled
                                 );


                if (CustomBookingRequestParser is not null)
                    BookingRequest = CustomBookingRequestParser(JSON,
                                                                BookingRequest);

                return true;

            }
            catch (Exception e)
            {
                BookingRequest  = default;
                ErrorResponse   = "The given JSON representation of a booking request request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBookingRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBookingRequestSerializer">A delegate to serialize custom booking request request JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BookingRequest>? CustomBookingRequestSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("country_code",              CountryCode.           ToString()),
                                 new JProperty("party_id",                  PartyId.               ToString()),
                                 new JProperty("request_id",                RequestId.             ToString()),
                                 new JProperty("location_id",               LocationId.            ToString()),
                                 new JProperty("tokens",                    new JArray(Tokens.Select(token => token.ToString()))),
                                 new JProperty("period",                    Period.                ToJSON()),
                                 new JProperty("authorization_reference",   AuthorizationReference.ToString()),

                           BookableParkingOptions is not null
                               ? new JProperty("bookable_parking_option",   BookableParkingOptions.ToString())
                               : null,

                           EVSEUId.HasValue
                               ? new JProperty("evse_uid",                  EVSEUId.               ToString())
                               : null,

                           PowerRequired.HasValue
                               ? new JProperty("power_required",            PowerRequired.         Value.Value)
                               : null,

                           Canceled is not null
                               ? new JProperty("canceled",                  Canceled.              ToJSON())
                               : null

                       );

            return CustomBookingRequestSerializer is not null
                       ? CustomBookingRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this booking request.
        /// </summary>
        public BookingRequest Clone()

            => new (
                   CountryCode.            Clone(),
                   PartyId.                Clone(),
                   RequestId.              Clone(),
                   LocationId.             Clone(),
                   Tokens.Select(token => token.Clone()),
                   Period.                 Clone(),
                   AuthorizationReference. Clone(),
                   BookableParkingOptions?.Clone(),
                   EVSEUId?.               Clone(),
                   PowerRequired?.         Clone(),
                   Canceled?.              Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (BookingRequest1, BookingRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingRequest1">A booking request.</param>
        /// <param name="BookingRequest2">Another booking request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BookingRequest? BookingRequest1,
                                           BookingRequest? BookingRequest2)
        {

            if (Object.ReferenceEquals(BookingRequest1, BookingRequest2))
                return true;

            if (BookingRequest1 is null || BookingRequest2 is null)
                return false;

            return BookingRequest1.Equals(BookingRequest2);

        }

        #endregion

        #region Operator != (BookingRequest1, BookingRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingRequest1">A booking request.</param>
        /// <param name="BookingRequest2">Another booking request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BookingRequest? BookingRequest1,
                                           BookingRequest? BookingRequest2)

            => !(BookingRequest1 == BookingRequest2);

        #endregion

        #region Operator <  (BookingRequest1, BookingRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingRequest1">A booking request.</param>
        /// <param name="BookingRequest2">Another booking request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BookingRequest? BookingRequest1,
                                          BookingRequest? BookingRequest2)

            => BookingRequest1 is null
                   ? throw new ArgumentNullException(nameof(BookingRequest1), "The given booking request must not be null!")
                   : BookingRequest1.CompareTo(BookingRequest2) < 0;

        #endregion

        #region Operator <= (BookingRequest1, BookingRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingRequest1">A booking request.</param>
        /// <param name="BookingRequest2">Another booking request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BookingRequest? BookingRequest1,
                                           BookingRequest? BookingRequest2)

            => !(BookingRequest1 > BookingRequest2);

        #endregion

        #region Operator >  (BookingRequest1, BookingRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingRequest1">A booking request.</param>
        /// <param name="BookingRequest2">Another booking request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BookingRequest? BookingRequest1,
                                          BookingRequest? BookingRequest2)

            => BookingRequest1 is null
                   ? throw new ArgumentNullException(nameof(BookingRequest1), "The given booking request must not be null!")
                   : BookingRequest1.CompareTo(BookingRequest2) > 0;

        #endregion

        #region Operator >= (BookingRequest1, BookingRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingRequest1">A booking request.</param>
        /// <param name="BookingRequest2">Another booking request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BookingRequest? BookingRequest1,
                                           BookingRequest? BookingRequest2)

            => !(BookingRequest1 < BookingRequest2);

        #endregion

        #endregion

        #region IComparable<BookingRequest> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two booking requests.
        /// </summary>
        /// <param name="Object">A booking request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is BookingRequest bookingRequest
                   ? CompareTo(bookingRequest)
                   : throw new ArgumentException("The given object is not a booking request!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BookingRequest)

        /// <summary>s
        /// Compares two booking requests.
        /// </summary>
        /// <param name="Object">A booking request to compare with.</param>
        public Int32 CompareTo(BookingRequest? BookingRequest)
        {

            if (BookingRequest is null)
                throw new ArgumentNullException(nameof(BookingRequest), "The given booking request must not be null!");

            var c = CountryCode.           CompareTo(BookingRequest.CountryCode);

            if (c == 0)
                c = PartyId.               CompareTo(BookingRequest.PartyId);

            if (c == 0)
                c = RequestId.             CompareTo(BookingRequest.RequestId);

            if (c == 0)
                c = LocationId.            CompareTo(BookingRequest.LocationId);

            if (c == 0)
                c = Tokens.Order().AggregateWith(",").CompareTo(BookingRequest.Tokens.Order().AggregateWith(","));

            if (c == 0)
                c = Period.                CompareTo(BookingRequest.Period);

            if (c == 0)
                c = AuthorizationReference.CompareTo(BookingRequest.AuthorizationReference);

            if (c == 0 && BookableParkingOptions is not null && BookingRequest.BookableParkingOptions is not null)
                c = BookableParkingOptions.CompareTo(BookingRequest.AuthorizationReference);

            if (c == 0 && EVSEUId.  HasValue                 && BookingRequest.EVSEUId.HasValue)
                c = EVSEUId.Value.         CompareTo(BookingRequest.EVSEUId.Value);

            if (c == 0 && PowerRequired.HasValue             && BookingRequest.PowerRequired.HasValue)
                c = PowerRequired.Value.   CompareTo(BookingRequest.PowerRequired.Value);

            if (c == 0 && Canceled is not null               && BookingRequest.Canceled is not null)
                c = Canceled.              CompareTo(BookingRequest.Canceled);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<BookingRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two booking requests for equality.
        /// </summary>
        /// <param name="Object">A booking request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BookingRequest bookingRequest &&
                   Equals(bookingRequest);

        #endregion

        #region Equals(BookingRequest)

        /// <summary>
        /// Compares two booking requests for equality.
        /// </summary>
        /// <param name="BookingRequest">A booking request to compare with.</param>
        public Boolean Equals(BookingRequest? BookingRequest)

            => BookingRequest is not null &&

               CountryCode.           Equals       (BookingRequest.CountryCode)            &&
               PartyId.               Equals       (BookingRequest.PartyId)                &&
               RequestId.             Equals       (BookingRequest.RequestId)              &&
               LocationId.            Equals       (BookingRequest.LocationId)             &&
               Tokens.Order().        SequenceEqual(BookingRequest.Tokens.Order())         &&
               Period.                Equals       (BookingRequest.Period)                 &&
               AuthorizationReference.Equals       (BookingRequest.AuthorizationReference) &&

             ((BookableParkingOptions is     null &&  BookingRequest.BookableParkingOptions is     null) ||
              (BookableParkingOptions is not null &&  BookingRequest.BookableParkingOptions is not null && BookableParkingOptions.Equals(BookingRequest.BookableParkingOptions))) &&

            ((!EVSEUId.               HasValue    && !BookingRequest.EVSEUId.               HasValue)    ||
              (EVSEUId.               HasValue    &&  BookingRequest.EVSEUId.               HasValue    && EVSEUId.         Value.Equals(BookingRequest.EVSEUId.      Value))) &&

            ((!PowerRequired.         HasValue    && !BookingRequest.PowerRequired.         HasValue)    ||
              (PowerRequired.         HasValue    &&  BookingRequest.PowerRequired.         HasValue    && PowerRequired.   Value.Equals(BookingRequest.PowerRequired.Value))) &&

             ((Canceled               is     null &&  BookingRequest.Canceled               is     null) ||
              (Canceled               is not null &&  BookingRequest.Canceled               is not null && Canceled.              Equals(BookingRequest.Canceled)));

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

                   $"{CountryCode}-{PartyId}*{RequestId}"

                   //LicensePlate.IsNotNullOrEmpty()
                   //    ? $" [{LicensePlate}]"
                   //    : ""

               );

        #endregion

    }

}
