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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// A booking request status.
    /// </summary>
    public class BookingRequestStatus : IEquatable<BookingRequestStatus>,
                                        IComparable<BookingRequestStatus>,
                                        IComparable
    {

        #region Properties

        /// <summary>
        /// The current state of the booking request.
        /// </summary>
        [Mandatory]
        public RequestStatus   RequestStatus      { get; }

        /// <summary>
        /// The booking request that was received.
        /// </summary>
        [Mandatory]
        public BookingRequest  BookingRequest     { get; }

        /// <summary>
        /// Timestamp for when the request was received.
        /// </summary>
        [Mandatory]
        public DateTimeOffset  RequestReceived    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new booking request status.
        /// </summary>
        /// <param name="RequestStatus">A request status.</param>
        /// <param name="BookingRequest">A booking request.</param>
        /// <param name="RequestReceived">A request received timestamp.</param>
        public BookingRequestStatus(RequestStatus   RequestStatus,
                                    BookingRequest  BookingRequest,
                                    DateTimeOffset  RequestReceived)
        {

            this.RequestStatus    = RequestStatus;
            this.BookingRequest   = BookingRequest;
            this.RequestReceived  = RequestReceived;

            unchecked
            {

                hashCode = this.RequestStatus.  GetHashCode() *  5 ^
                           this.BookingRequest. GetHashCode() *  3 ^
                           this.RequestReceived.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomBookingRequestStatusParser = null)

        /// <summary>
        /// Parse the given JSON representation of a booking request status token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomBookingRequestStatusParser">A delegate to parse custom booking request status token JSON objects.</param>
        public static BookingRequestStatus Parse(JObject                                             JSON,
                                                 CustomJObjectParserDelegate<BookingRequestStatus>?  CustomBookingRequestStatusParser   = null)
        {

            if (TryParse(JSON,
                         out var bookingRequestStatus,
                         out var errorResponse,
                         CustomBookingRequestStatusParser))
            {
                return bookingRequestStatus;
            }

            throw new ArgumentException("The given JSON representation of a booking request status token is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out BookingRequestStatus, out ErrorResponse, CustomBookingRequestStatusParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a booking request status token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="BookingRequestStatus">The parsed booking request status token.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out BookingRequestStatus?  BookingRequestStatus,
                                       [NotNullWhen(false)] out String?                ErrorResponse)

            => TryParse(JSON,
                        out BookingRequestStatus,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a booking request status token.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="BookingRequestStatus">The parsed booking request status token.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBookingRequestStatusParser">A delegate to parse custom booking request status token JSON objects.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       [NotNullWhen(true)]  out BookingRequestStatus?      BookingRequestStatus,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       CustomJObjectParserDelegate<BookingRequestStatus>?  CustomBookingRequestStatusParser   = null)
        {

            try
            {

                BookingRequestStatus = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse RequestStatus      [mandatory]

                if (!JSON.ParseMandatory("request_status",
                                         "request status",
                                         RequestStatus.TryParse,
                                         out RequestStatus requestStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse BookingRequest     [mandatory]

                if (!JSON.ParseMandatoryJSON("booking_request",
                                             "booking request",
                                             BookingRequest.TryParse,
                                             out BookingRequest? bookingRequest,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse RequestReceived    [mandatory]

                if (!JSON.ParseMandatory("request_received",
                                         "booking request received",
                                         out DateTimeOffset requestReceived,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                BookingRequestStatus = new BookingRequestStatus(
                                           requestStatus,
                                           bookingRequest,
                                           requestReceived
                                       );


                if (CustomBookingRequestStatusParser is not null)
                    BookingRequestStatus = CustomBookingRequestStatusParser(JSON,
                                                                            BookingRequestStatus);

                return true;

            }
            catch (Exception e)
            {
                BookingRequestStatus  = default;
                ErrorResponse         = "The given JSON representation of a booking request status token is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBookingRequestStatusSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBookingRequestStatusSerializer">A delegate to serialize custom booking request status token JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BookingRequestStatus>? CustomBookingRequestStatusSerializer = null)
        {

            var json = JSONObject.Create(

                           new JProperty("request_status",     RequestStatus.   ToString()),
                           new JProperty("booking_request",    BookingRequest.  ToJSON()),
                           new JProperty("request_received",   RequestReceived. ToISO8601())

                       );

            return CustomBookingRequestStatusSerializer is not null
                       ? CustomBookingRequestStatusSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this booking request status.
        /// </summary>
        public BookingRequestStatus Clone()

            => new (
                   RequestStatus. Clone(),
                   BookingRequest.Clone(),
                   RequestReceived
               );

        #endregion


        #region Operator overloading

        #region Operator == (BookingRequestStatus1, BookingRequestStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingRequestStatus1">A booking request status.</param>
        /// <param name="BookingRequestStatus2">Another booking request status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BookingRequestStatus? BookingRequestStatus1,
                                           BookingRequestStatus? BookingRequestStatus2)
        {

            if (Object.ReferenceEquals(BookingRequestStatus1, BookingRequestStatus2))
                return true;

            if (BookingRequestStatus1 is null || BookingRequestStatus2 is null)
                return false;

            return BookingRequestStatus1.Equals(BookingRequestStatus2);

        }

        #endregion

        #region Operator != (BookingRequestStatus1, BookingRequestStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingRequestStatus1">A booking request status.</param>
        /// <param name="BookingRequestStatus2">Another booking request status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BookingRequestStatus? BookingRequestStatus1,
                                           BookingRequestStatus? BookingRequestStatus2)

            => !(BookingRequestStatus1 == BookingRequestStatus2);

        #endregion

        #region Operator <  (BookingRequestStatus1, BookingRequestStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingRequestStatus1">A booking request status.</param>
        /// <param name="BookingRequestStatus2">Another booking request status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BookingRequestStatus? BookingRequestStatus1,
                                          BookingRequestStatus? BookingRequestStatus2)

            => BookingRequestStatus1 is null
                   ? throw new ArgumentNullException(nameof(BookingRequestStatus1), "The given booking request status must not be null!")
                   : BookingRequestStatus1.CompareTo(BookingRequestStatus2) < 0;

        #endregion

        #region Operator <= (BookingRequestStatus1, BookingRequestStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingRequestStatus1">A booking request status.</param>
        /// <param name="BookingRequestStatus2">Another booking request status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BookingRequestStatus? BookingRequestStatus1,
                                           BookingRequestStatus? BookingRequestStatus2)

            => !(BookingRequestStatus1 > BookingRequestStatus2);

        #endregion

        #region Operator >  (BookingRequestStatus1, BookingRequestStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingRequestStatus1">A booking request status.</param>
        /// <param name="BookingRequestStatus2">Another booking request status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BookingRequestStatus? BookingRequestStatus1,
                                          BookingRequestStatus? BookingRequestStatus2)

            => BookingRequestStatus1 is null
                   ? throw new ArgumentNullException(nameof(BookingRequestStatus1), "The given booking request status must not be null!")
                   : BookingRequestStatus1.CompareTo(BookingRequestStatus2) > 0;

        #endregion

        #region Operator >= (BookingRequestStatus1, BookingRequestStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingRequestStatus1">A booking request status.</param>
        /// <param name="BookingRequestStatus2">Another booking request status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BookingRequestStatus? BookingRequestStatus1,
                                           BookingRequestStatus? BookingRequestStatus2)

            => !(BookingRequestStatus1 < BookingRequestStatus2);

        #endregion

        #endregion

        #region IComparable<BookingRequestStatus> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two booking request statuss.
        /// </summary>
        /// <param name="Object">A booking request status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is BookingRequestStatus bookingRequestStatus
                   ? CompareTo(bookingRequestStatus)
                   : throw new ArgumentException("The given object is not a booking request status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BookingRequestStatus)

        /// <summary>s
        /// Compares two booking request statuss.
        /// </summary>
        /// <param name="Object">A booking request status to compare with.</param>
        public Int32 CompareTo(BookingRequestStatus? BookingRequestStatus)
        {

            if (BookingRequestStatus is null)
                throw new ArgumentNullException(nameof(BookingRequestStatus), "The given booking request status must not be null!");

            var c = RequestStatus.  CompareTo(BookingRequestStatus.RequestStatus);

            if (c == 0)
                c = RequestReceived.CompareTo(BookingRequestStatus.RequestReceived);

            if (c == 0)
                c = BookingRequest. CompareTo(BookingRequestStatus.BookingRequest);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<BookingRequestStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two booking request statuss for equality.
        /// </summary>
        /// <param name="Object">A booking request status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BookingRequestStatus bookingRequestStatus &&
                   Equals(bookingRequestStatus);

        #endregion

        #region Equals(BookingRequestStatus)

        /// <summary>
        /// Compares two booking request statuss for equality.
        /// </summary>
        /// <param name="BookingRequestStatus">A booking request status to compare with.</param>
        public Boolean Equals(BookingRequestStatus? BookingRequestStatus)

            => BookingRequestStatus is not null &&

               RequestStatus.  Equals(BookingRequestStatus.RequestStatus)   &&
               RequestReceived.Equals(BookingRequestStatus.RequestReceived) &&
               BookingRequest. Equals(BookingRequestStatus.BookingRequest);

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

            => $"{RequestStatus} at {RequestReceived}: {BookingRequest}";

        #endregion

    }

}
