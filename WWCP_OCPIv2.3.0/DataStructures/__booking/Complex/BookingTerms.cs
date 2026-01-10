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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Booking Terms.
    /// </summary>
    public class BookingTerms : IEquatable<BookingTerms>,
                                IComparable<BookingTerms>,
                                IComparable
    {

        #region Properties

        /// <summary>
        /// Optional indication whether charging using a booking requires to authenticate via RFID card at the charging station.
        /// </summary>
        [Optional]
        public Boolean?                     RFIDAuthRequired              { get; }

        /// <summary>
        /// Optional indication whether any token within the same token group can be used for using the booking.
        /// </summary>
        [Optional]
        public Boolean?                     TokenGroupsSupported          { get; }

        /// <summary>
        /// Optional indication whether charging using a booking is possible through remote start.
        /// </summary>
        [Optional]
        public Boolean?                     RemoteAuthSupported           { get; }

        /// <summary>
        /// The enumeration of location access methods.
        /// </summary>
        [Mandatory]
        public IEnumerable<LocationAccess>  SupportedAccessMethods        { get; }

        /// <summary>
        /// Time before the scheduled start of the booking during which changes are still allowed.
        /// </summary>
        [Mandatory]
        public TimeSpan                     ChangeUntil                   { get; }

        /// <summary>
        /// Time before the scheduled start of the booking during which cancellation is still allowed.
        /// </summary>
        [Mandatory]
        public TimeSpan                     CancelUntil                   { get; }

        /// <summary>
        /// Optional indication whether changes are allowed.
        /// </summary>
        [Optional]
        public Boolean?                     ChangeNotAllowed              { get; }

        /// <summary>
        /// Optional indication whether an early start of the session is allowed/possible.
        /// </summary>
        [Optional]
        public Boolean?                     EarlyStartAllowed             { get; }

        /// <summary>
        /// The optional time before the scheduled start when early access is permitted.
        /// </summary>
        [Optional]
        public TimeSpan?                    EarlyStartTime                { get; }

        /// <summary>
        /// The optional time after the scheduled start that it is considered
        /// a no show and booking is released. No timeout if unspecified.
        /// </summary>
        [Optional]
        public TimeSpan?                    NoShowTimeout                 { get; }

        /// <summary>
        /// Optional indication whether the CPO will charge a no show fee.
        /// The amount of the fee can be defined in the booking_terms URL.
        /// Will also be in the Tariff part of the BookingLocation.
        /// </summary>
        [Optional]
        public Boolean?                     NoShowFee                     { get; }

        /// <summary>
        /// Optional indication whether the charging session can be longer than requested in the booking.
        /// </summary>
        [Optional]
        public Boolean?                     LateStopAllowed               { get; }

        /// <summary>
        /// The optional time after the scheduled stop charging is still allowed/possible.
        /// </summary>
        [Optional]
        public TimeSpan?                    LateStopTime                  { get; }

        /// <summary>
        /// Optional indication whether it is possible to use the same RFID Token for multiple concurrent bookings.
        /// </summary>
        [Optional]
        public Boolean?                     OverlappingBookingsAllowed    { get; }

        /// <summary>
        /// The optional URL to CPO’s booking terms.
        /// </summary>
        [Optional]
        public URL?                         BookingTermsURL               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new Booking Terms.
        /// </summary>
        /// <param name="SupportedAccessMethods">The enumeration of location access methods.</param>
        /// <param name="ChangeUntil">Time before the scheduled start of the booking during which changes are still allowed.</param>
        /// <param name="CancelUntil">Time before the scheduled start of the booking during which cancellation is still allowed.</param>
        /// <param name="ChangeNotAllowed">Optional indication whether changes are allowed.</param>
        /// <param name="EarlyStartAllowed">Optional indication whether an early start of the session is allowed/possible.</param>
        /// <param name="EarlyStartTime">Optional time before the scheduled start when early access is permitted.</param>
        /// <param name="NoShowTimeout">Optional time after the scheduled start that it is considered a no show and booking is released. No timeout if unspecified.</param>
        /// <param name="NoShowFee">Optional indication whether the CPO will charge a no show fee. The amount of the fee can be defined in the booking_terms URL. Will also be in the Tariff part of the BookingLocation.</param>
        /// <param name="LateStopAllowed">Optional indication whether the charging session can be longer than requested in the booking.</param>
        /// <param name="LateStopTime">Optional time after the scheduled stop charging is still allowed/possible.</param>
        /// <param name="OverlappingBookingsAllowed">Optional indication whether it is possible to use the same RFID Token for multiple concurrent bookings.</param>
        /// <param name="RFIDAuthRequired">Optional indication whether charging using a booking requires to authenticate via RFID card at the charging station.</param>
        /// <param name="TokenGroupsSupported">Optional indication whether any token within the same token group can be used for using the booking.</param>
        /// <param name="RemoteAuthSupported">Optional indication whether charging using a booking is possible through remote start.</param>
        /// <param name="BookingTermsURL">Optional URL to CPO’s booking terms.</param>
        public BookingTerms(IEnumerable<LocationAccess>  SupportedAccessMethods,
                            TimeSpan                     ChangeUntil,
                            TimeSpan                     CancelUntil,
                            Boolean?                     ChangeNotAllowed             = null,
                            Boolean?                     EarlyStartAllowed            = null,
                            TimeSpan?                    EarlyStartTime               = null,
                            TimeSpan?                    NoShowTimeout                = null,
                            Boolean?                     NoShowFee                    = null,
                            Boolean?                     LateStopAllowed              = null,
                            TimeSpan?                    LateStopTime                 = null,
                            Boolean?                     OverlappingBookingsAllowed   = null,
                            Boolean?                     RFIDAuthRequired             = null,
                            Boolean?                     TokenGroupsSupported         = null,
                            Boolean?                     RemoteAuthSupported          = null,
                            URL?                         BookingTermsURL              = null)
        {

            this.SupportedAccessMethods      = SupportedAccessMethods.Distinct();
            this.ChangeUntil                 = ChangeUntil;
            this.CancelUntil                 = CancelUntil;
            this.ChangeNotAllowed            = ChangeNotAllowed;
            this.EarlyStartAllowed           = EarlyStartAllowed;
            this.EarlyStartTime              = EarlyStartTime;
            this.NoShowTimeout               = NoShowTimeout;
            this.NoShowFee                   = NoShowFee;
            this.LateStopAllowed             = LateStopAllowed;
            this.LateStopTime                = LateStopTime;
            this.OverlappingBookingsAllowed  = OverlappingBookingsAllowed;
            this.RFIDAuthRequired            = RFIDAuthRequired;
            this.TokenGroupsSupported        = TokenGroupsSupported;
            this.RemoteAuthSupported         = RemoteAuthSupported;
            this.BookingTermsURL             = BookingTermsURL;

            unchecked
            {

                hashCode = this.SupportedAccessMethods.     GetHashCode()       * 47 ^
                           this.ChangeUntil.                GetHashCode()       * 43 ^
                           this.CancelUntil.                GetHashCode()       * 41 ^
                          (this.ChangeNotAllowed?.          GetHashCode() ?? 0) * 37 ^
                          (this.EarlyStartAllowed?.         GetHashCode() ?? 0) * 31 ^
                          (this.EarlyStartTime?.            GetHashCode() ?? 0) * 29 ^
                          (this.NoShowTimeout?.             GetHashCode() ?? 0) * 23 ^
                          (this.NoShowFee?.                 GetHashCode() ?? 0) * 19 ^
                          (this.LateStopAllowed?.           GetHashCode() ?? 0) * 17 ^
                          (this.LateStopTime?.              GetHashCode() ?? 0) * 13 ^
                          (this.OverlappingBookingsAllowed?.GetHashCode() ?? 0) * 11 ^
                          (this.RFIDAuthRequired?.          GetHashCode() ?? 0) *  7 ^
                          (this.TokenGroupsSupported?.      GetHashCode() ?? 0) *  5 ^
                          (this.RemoteAuthSupported?.       GetHashCode() ?? 0) *  3 ^
                           this.BookingTermsURL?.           GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomBookingTermsParser = null)

        /// <summary>
        /// Parse the given JSON representation of Booking Terms.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomBookingTermsParser">A delegate to parse custom Booking Terms JSON objects.</param>
        public static BookingTerms Parse(JObject                                     JSON,
                                         CustomJObjectParserDelegate<BookingTerms>?  CustomBookingTermsParser   = null)
        {

            if (TryParse(JSON,
                         out var bookingTerms,
                         out var errorResponse,
                         CustomBookingTermsParser))
            {
                return bookingTerms;
            }

            throw new ArgumentException("The given JSON representation of Booking Terms is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out BookingTerms, out ErrorResponse, CustomBookingTermsParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of Booking Terms.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="BookingTerms">The parsed Booking Terms.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out BookingTerms?  BookingTerms,
                                       [NotNullWhen(false)] out String?        ErrorResponse)

            => TryParse(JSON,
                        out BookingTerms,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of Booking Terms.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="BookingTerms">The parsed Booking Terms.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBookingTermsParser">A delegate to parse custom Booking Terms JSON objects.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out BookingTerms?      BookingTerms,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomJObjectParserDelegate<BookingTerms>?  CustomBookingTermsParser   = null)
        {

            try
            {

                BookingTerms = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse SupportedAccessMethods        [mandatory]

                if (!JSON.ParseMandatoryHashSet("supported_access_methods",
                                                "supported access methods",
                                                LocationAccess.TryParse,
                                                out HashSet<LocationAccess> supportedAccessMethods,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ChangeUntil                   [mandatory]

                if (!JSON.ParseMandatory("change_until_minutes",
                                         "change until minutes",
                                         TimeSpanExtensions.TryParseMinutes,
                                         out TimeSpan changeUntil,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CancelUntil                   [mandatory]

                if (!JSON.ParseMandatory("cancel_until_minutes",
                                         "cancel until minutes",
                                         TimeSpanExtensions.TryParseMinutes,
                                         out TimeSpan cancelUntil,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ChangeNotAllowed              [optional]

                if (JSON.ParseOptional("change_not_allowed",
                                       "change not allowed",
                                       out Boolean? changeNotAllowed,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EarlyStartAllowed             [optional]

                if (JSON.ParseOptional("early_start_allowed",
                                       "early start allowed",
                                       out Boolean? earlyStartAllowed,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EarlyStartTime                [optional]

                if (JSON.ParseOptional("early_start_time",
                                       "early start time",
                                       TimeSpanExtensions.TryParseMinutes,
                                       out TimeSpan? earlyStartTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NoShowTimeout                 [optional]

                if (JSON.ParseOptional("noshow_timeout",
                                       "noshow timeout",
                                       TimeSpanExtensions.TryParseMinutes,
                                       out TimeSpan? noShowTimeout,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NoShowFee                     [optional]

                if (JSON.ParseOptional("noshow_fee",
                                       "no show fee",
                                       out Boolean? noShowFee,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LateStopAllowed               [optional]

                if (JSON.ParseOptional("late_stop_allowed",
                                       "late stop allowed",
                                       out Boolean? lateStopAllowed,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LateStopTime                  [optional]

                if (JSON.ParseOptional("late_stop_time",
                                       "late stop time",
                                       TimeSpanExtensions.TryParseMinutes,
                                       out TimeSpan? lateStopTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse OverlappingBookingsAllowed    [optional]

                if (JSON.ParseOptional("overlapping_bookings_allowed",
                                       "overlapping bookings allowed",
                                       out Boolean? overlappingBookingsAllowed,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse RFIDAuthRequired              [optional]

                if (JSON.ParseOptional("RFID_auth_required",
                                       "RFID auth required",
                                       out Boolean? rfidAuthRequired,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TokenGroupsSupported          [optional]

                if (JSON.ParseOptional("token_groups_supported",
                                       "token groups supported",
                                       out Boolean? tokenGroupsSupported,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse RemoteAuthSupported           [optional]

                if (JSON.ParseOptional("remote_auth_supported",
                                       "remote auth supported",
                                       out Boolean? remoteAuthSupported,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse BookingTermsURL               [optional]

                if (JSON.ParseOptional("booking_terms",
                                       "booking terms URL",
                                       URL.TryParse,
                                       out URL? bookingTermsURL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                BookingTerms = new BookingTerms(
                                   supportedAccessMethods,
                                   changeUntil,
                                   cancelUntil,
                                   changeNotAllowed,
                                   earlyStartAllowed,
                                   earlyStartTime,
                                   noShowTimeout,
                                   noShowFee,
                                   lateStopAllowed,
                                   lateStopTime,
                                   overlappingBookingsAllowed,
                                   rfidAuthRequired,
                                   tokenGroupsSupported,
                                   remoteAuthSupported,
                                   bookingTermsURL
                               );


                if (CustomBookingTermsParser is not null)
                    BookingTerms = CustomBookingTermsParser(JSON,
                                                            BookingTerms);

                return true;

            }
            catch (Exception e)
            {
                BookingTerms   = default;
                ErrorResponse  = "The given JSON representation of Booking Terms is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBookingTermsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBookingTermsSerializer">A delegate to serialize custom Booking Terms JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BookingTerms>? CustomBookingTermsSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("supported_access_methods",       new JArray(SupportedAccessMethods.Select(locationAccess => locationAccess.ToString()))),
                                 new JProperty("change_until_minutes",           ChangeUntil.TotalMinutes),
                                 new JProperty("cancel_until_minutes",           CancelUntil.TotalMinutes),

                           ChangeNotAllowed.          HasValue
                               ? new JProperty("change_not_allowed",             ChangeNotAllowed.          Value)
                               : null,

                           EarlyStartAllowed.         HasValue
                               ? new JProperty("early_start_allowed",            EarlyStartAllowed.         Value)
                               : null,

                           EarlyStartTime.            HasValue
                               ? new JProperty("early_start_time",               EarlyStartTime.            Value.TotalMinutes)
                               : null,

                           NoShowTimeout.             HasValue
                               ? new JProperty("noshow_timeout",                 NoShowTimeout.             Value.TotalMinutes)
                               : null,

                           NoShowFee.                 HasValue
                               ? new JProperty("noshow_fee",                     NoShowFee.                 Value)
                               : null,

                           LateStopAllowed.           HasValue
                               ? new JProperty("late_stop_allowed",              LateStopAllowed.           Value)
                               : null,

                           LateStopTime.              HasValue
                               ? new JProperty("late_stop_time",                 LateStopTime.              Value.TotalMinutes)
                               : null,

                           OverlappingBookingsAllowed.HasValue
                               ? new JProperty("overlapping_bookings_allowed",   OverlappingBookingsAllowed.Value)
                               : null,

                           RFIDAuthRequired.          HasValue
                               ? new JProperty("RFID_auth_required",             RFIDAuthRequired.          Value)
                               : null,

                           TokenGroupsSupported.      HasValue
                               ? new JProperty("token_groups_supported",         TokenGroupsSupported.      Value)
                               : null,

                           RemoteAuthSupported.       HasValue
                               ? new JProperty("remote_auth_supported",          RemoteAuthSupported.       Value)
                               : null,

                           BookingTermsURL.           HasValue
                               ? new JProperty("booking_terms",                  BookingTermsURL.           Value.ToString())
                               : null

                       );

            return CustomBookingTermsSerializer is not null
                       ? CustomBookingTermsSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone these Booking Terms.
        /// </summary>
        public BookingTerms Clone()

            => new (
                   SupportedAccessMethods.Select(locationAccess => locationAccess.Clone()),
                   ChangeUntil,
                   CancelUntil,
                   ChangeNotAllowed,
                   EarlyStartAllowed,
                   EarlyStartTime,
                   NoShowTimeout,
                   NoShowFee,
                   LateStopAllowed,
                   LateStopTime,
                   OverlappingBookingsAllowed,
                   RFIDAuthRequired,
                   TokenGroupsSupported,
                   RemoteAuthSupported,
                   BookingTermsURL?.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (BookingTerms1, BookingTerms2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingTerms1">A Booking Terms.</param>
        /// <param name="BookingTerms2">Another Booking Terms.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BookingTerms? BookingTerms1,
                                           BookingTerms? BookingTerms2)
        {

            if (Object.ReferenceEquals(BookingTerms1, BookingTerms2))
                return true;

            if (BookingTerms1 is null || BookingTerms2 is null)
                return false;

            return BookingTerms1.Equals(BookingTerms2);

        }

        #endregion

        #region Operator != (BookingTerms1, BookingTerms2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingTerms1">A Booking Terms.</param>
        /// <param name="BookingTerms2">Another Booking Terms.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BookingTerms? BookingTerms1,
                                           BookingTerms? BookingTerms2)

            => !(BookingTerms1 == BookingTerms2);

        #endregion

        #region Operator <  (BookingTerms1, BookingTerms2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingTerms1">A Booking Terms.</param>
        /// <param name="BookingTerms2">Another Booking Terms.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BookingTerms? BookingTerms1,
                                          BookingTerms? BookingTerms2)

            => BookingTerms1 is null
                   ? throw new ArgumentNullException(nameof(BookingTerms1), "The given Booking Terms must not be null!")
                   : BookingTerms1.CompareTo(BookingTerms2) < 0;

        #endregion

        #region Operator <= (BookingTerms1, BookingTerms2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingTerms1">A Booking Terms.</param>
        /// <param name="BookingTerms2">Another Booking Terms.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BookingTerms? BookingTerms1,
                                           BookingTerms? BookingTerms2)

            => !(BookingTerms1 > BookingTerms2);

        #endregion

        #region Operator >  (BookingTerms1, BookingTerms2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingTerms1">A Booking Terms.</param>
        /// <param name="BookingTerms2">Another Booking Terms.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BookingTerms? BookingTerms1,
                                          BookingTerms? BookingTerms2)

            => BookingTerms1 is null
                   ? throw new ArgumentNullException(nameof(BookingTerms1), "The given Booking Terms must not be null!")
                   : BookingTerms1.CompareTo(BookingTerms2) > 0;

        #endregion

        #region Operator >= (BookingTerms1, BookingTerms2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingTerms1">A Booking Terms.</param>
        /// <param name="BookingTerms2">Another Booking Terms.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BookingTerms? BookingTerms1,
                                           BookingTerms? BookingTerms2)

            => !(BookingTerms1 < BookingTerms2);

        #endregion

        #endregion

        #region IComparable<BookingTerms> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two Booking Termss.
        /// </summary>
        /// <param name="Object">A Booking Terms to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is BookingTerms bookingTerms
                   ? CompareTo(bookingTerms)
                   : throw new ArgumentException("The given object is not Booking Terms!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BookingTerms)

        /// <summary>s
        /// Compares two Booking Termss.
        /// </summary>
        /// <param name="Object">A Booking Terms to compare with.</param>
        public Int32 CompareTo(BookingTerms? BookingTerms)
        {

            if (BookingTerms is null)
                throw new ArgumentNullException(nameof(BookingTerms), "The given Booking Terms must not be null!");

            var c = SupportedAccessMethods.Order().Select(locationAccess => locationAccess.ToString()).AggregateWith(",").CompareTo(
                        BookingTerms.SupportedAccessMethods.Order().Select(locationAccess => locationAccess.ToString()).AggregateWith(",")
                    );

            if (c == 0)
                c = ChangeUntil.CompareTo(BookingTerms.ChangeUntil);

            if (c == 0)
                c = CancelUntil.CompareTo(BookingTerms.CancelUntil);

            if (c == 0 && ChangeNotAllowed.          HasValue && BookingTerms.ChangeNotAllowed.          HasValue)
                c = ChangeNotAllowed.          Value.CompareTo(BookingTerms.ChangeNotAllowed.          Value);

            if (c == 0 && EarlyStartAllowed.         HasValue && BookingTerms.EarlyStartAllowed.         HasValue)
                c = EarlyStartAllowed.         Value.CompareTo(BookingTerms.EarlyStartAllowed.         Value);

            if (c == 0 && EarlyStartTime.            HasValue && BookingTerms.EarlyStartTime.            HasValue)
                c = EarlyStartTime.            Value.CompareTo(BookingTerms.EarlyStartTime.            Value);

            if (c == 0 && NoShowTimeout.             HasValue && BookingTerms.NoShowTimeout.             HasValue)
                c = NoShowTimeout.             Value.CompareTo(BookingTerms.NoShowTimeout.             Value);

            if (c == 0 && NoShowFee.                 HasValue && BookingTerms.NoShowFee.                 HasValue)
                c = NoShowFee.                 Value.CompareTo(BookingTerms.NoShowFee.                 Value);

            if (c == 0 && LateStopAllowed.           HasValue && BookingTerms.LateStopAllowed.           HasValue)
                c = LateStopAllowed.           Value.CompareTo(BookingTerms.LateStopAllowed.           Value);

            if (c == 0 && LateStopTime.              HasValue && BookingTerms.LateStopTime.              HasValue)
                c = LateStopTime.              Value.CompareTo(BookingTerms.LateStopTime.              Value);

            if (c == 0 && OverlappingBookingsAllowed.HasValue && BookingTerms.OverlappingBookingsAllowed.HasValue)
                c = OverlappingBookingsAllowed.Value.CompareTo(BookingTerms.OverlappingBookingsAllowed.Value);

            if (c == 0 && RFIDAuthRequired.          HasValue && BookingTerms.RFIDAuthRequired.          HasValue)
                c = RFIDAuthRequired.          Value.CompareTo(BookingTerms.RFIDAuthRequired.          Value);

            if (c == 0 && TokenGroupsSupported.      HasValue && BookingTerms.TokenGroupsSupported.      HasValue)
                c = TokenGroupsSupported.      Value.CompareTo(BookingTerms.TokenGroupsSupported.      Value);

            if (c == 0 && RemoteAuthSupported.       HasValue && BookingTerms.RemoteAuthSupported.       HasValue)
                c = RemoteAuthSupported.       Value.CompareTo(BookingTerms.RemoteAuthSupported.       Value);

            if (c == 0 && BookingTermsURL.           HasValue && BookingTerms.BookingTermsURL.           HasValue)
                c = BookingTermsURL.           Value.CompareTo(BookingTerms.BookingTermsURL.           Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<BookingTerms> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Booking Termss for equality.
        /// </summary>
        /// <param name="Object">A Booking Terms to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BookingTerms bookingTerms &&
                   Equals(bookingTerms);

        #endregion

        #region Equals(BookingTerms)

        /// <summary>
        /// Compares two Booking Termss for equality.
        /// </summary>
        /// <param name="BookingTerms">A Booking Terms to compare with.</param>
        public Boolean Equals(BookingTerms? BookingTerms)

            => BookingTerms is not null &&

               SupportedAccessMethods.Order().SequenceEqual(BookingTerms.SupportedAccessMethods.Order()) &&
               ChangeUntil.Equals(BookingTerms.ChangeUntil) &&
               CancelUntil.Equals(BookingTerms.CancelUntil) &&

            ((!ChangeNotAllowed.          HasValue && !BookingTerms.ChangeNotAllowed.          HasValue) ||
              (ChangeNotAllowed.          HasValue &&  BookingTerms.ChangeNotAllowed.          HasValue && ChangeNotAllowed.          Value.Equals(BookingTerms.ChangeNotAllowed.          Value))) &&

            ((!EarlyStartAllowed.         HasValue && !BookingTerms.EarlyStartAllowed.         HasValue) ||
              (EarlyStartAllowed.         HasValue &&  BookingTerms.EarlyStartAllowed.         HasValue && EarlyStartAllowed.         Value.Equals(BookingTerms.EarlyStartAllowed.         Value))) &&

            ((!EarlyStartTime.            HasValue && !BookingTerms.EarlyStartTime.            HasValue) ||
              (EarlyStartTime.            HasValue &&  BookingTerms.EarlyStartTime.            HasValue && EarlyStartTime.            Value.Equals(BookingTerms.EarlyStartTime.            Value))) &&

            ((!NoShowTimeout.             HasValue && !BookingTerms.NoShowTimeout.             HasValue) ||
              (NoShowTimeout.             HasValue &&  BookingTerms.NoShowTimeout.             HasValue && NoShowTimeout.             Value.Equals(BookingTerms.NoShowTimeout.             Value))) &&

            ((!NoShowFee.                 HasValue && !BookingTerms.NoShowFee.                 HasValue) ||
              (NoShowFee.                 HasValue &&  BookingTerms.NoShowFee.                 HasValue && NoShowFee.                 Value.Equals(BookingTerms.NoShowFee.                 Value))) &&

            ((!LateStopAllowed.           HasValue && !BookingTerms.LateStopAllowed.           HasValue) ||
              (LateStopAllowed.           HasValue &&  BookingTerms.LateStopAllowed.           HasValue && LateStopAllowed.           Value.Equals(BookingTerms.LateStopAllowed.           Value))) &&

            ((!LateStopTime.              HasValue && !BookingTerms.LateStopTime.              HasValue) ||
              (LateStopTime.              HasValue &&  BookingTerms.LateStopTime.              HasValue && LateStopTime.              Value.Equals(BookingTerms.LateStopTime.              Value))) &&

            ((!OverlappingBookingsAllowed.HasValue && !BookingTerms.OverlappingBookingsAllowed.HasValue) ||
              (OverlappingBookingsAllowed.HasValue &&  BookingTerms.OverlappingBookingsAllowed.HasValue && OverlappingBookingsAllowed.Value.Equals(BookingTerms.OverlappingBookingsAllowed.Value))) &&

            ((!RFIDAuthRequired.          HasValue && !BookingTerms.RFIDAuthRequired.          HasValue) ||
              (RFIDAuthRequired.          HasValue &&  BookingTerms.RFIDAuthRequired.          HasValue && RFIDAuthRequired.          Value.Equals(BookingTerms.RFIDAuthRequired.          Value))) &&

            ((!TokenGroupsSupported.      HasValue && !BookingTerms.TokenGroupsSupported.      HasValue) ||
              (TokenGroupsSupported.      HasValue &&  BookingTerms.TokenGroupsSupported.      HasValue && TokenGroupsSupported.      Value.Equals(BookingTerms.TokenGroupsSupported.      Value))) &&

            ((!RemoteAuthSupported.       HasValue && !BookingTerms.RemoteAuthSupported.       HasValue) ||
              (RemoteAuthSupported.       HasValue &&  BookingTerms.RemoteAuthSupported.       HasValue && RemoteAuthSupported.       Value.Equals(BookingTerms.RemoteAuthSupported.       Value))) &&

            ((!BookingTermsURL.           HasValue && !BookingTerms.BookingTermsURL.           HasValue) ||
              (BookingTermsURL.           HasValue &&  BookingTerms.BookingTermsURL.           HasValue && BookingTermsURL.           Value.Equals(BookingTerms.BookingTermsURL.           Value)));

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

                   $"{Math.Round(ChangeUntil.TotalMinutes, 2)} / {Math.Round(CancelUntil.TotalMinutes, 2)}: '{SupportedAccessMethods.AggregateWith(", ")}'",

                   ChangeNotAllowed.HasValue
                       ? ChangeNotAllowed.Value
                             ? ", no changes allowed"
                             : ", changes allowed"
                       : "",

                   BookingTermsURL.HasValue
                       ? $", URL: {BookingTermsURL.Value}"
                       : ""

               );

        #endregion

    }

}
