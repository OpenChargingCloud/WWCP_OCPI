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

using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Booking
    /// </summary>
    public class Booking : IEquatable<Booking>,
                           IComparable<Booking>,
                           IComparable
    {

        #region Properties

        /// <summary>
        /// The parent CommonAPI of this booking.
        /// </summary>
        internal CommonAPI?                       CommonAPI                 { get; set; }

        /// <summary>
        /// ID for the CPO side
        /// </summary>
        [Mandatory]
        public Booking_Id                         Id                        { get; }

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this booking request.
        /// </summary>
        [Mandatory]
        public CountryCode                        CountryCode               { get; }

        /// <summary>
        /// The identification of the charge point operator that 'owns' this booking request
        /// (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id                           PartyId                   { get; }

        /// <summary>
        /// Request identification determined by the requesting party.
        /// The same request identification SHALL be used for all edits on booking.
        /// </summary>
        [Mandatory]
        public Request_Id                         RequestId                 { get; }

        /// <summary>
        /// Selected parking specification to charge at this Location.
        /// </summary>
        [Optional]
        public BookableParkingOptions?            BookableParkingOption     { get; }

        /// <summary>
        /// Reference to the parking id, can be later assigned by the CPO based on the bookable parking option.
        /// </summary>
        [Optional]
        public Parking_Id?                        ParkingId                 { get; }

        /// <summary>
        /// Location identification of the location of this CPO, on which the reservation can be made.
        /// </summary>
        [Mandatory]
        public Location_Id                        LocationId                { get; }

        /// <summary>
        /// EVSE.uid of the EVSE of this Location on which the reservation will be made.
        /// Allowed to be set to: #NA when no EVSE yet assigned to the driver.
        /// </summary>
        [Optional]
        public EVSE_UId?                          EVSEUId                   { get; }

        /// <summary>
        /// The enumeration of tokens that can be used to utilise the booking.
        /// </summary>
        [Mandatory]
        public IEnumerable<BookingToken>          BookingTokens             { get; }

        /// <summary>
        /// The enumeration of tariff identifications relevant for this booking.
        /// </summary>
        [Mandatory]
        public IEnumerable<Tariff_Id>             TariffIds                 { get; }

        /// <summary>
        /// The timeslot for this booking.
        /// </summary>
        [Mandatory]
        public TimeSlot                           Period                    { get; }

        /// <summary>
        /// The current state of the reservation.
        /// </summary>
        [Mandatory]
        public ReservationStatus                  ReservationStatus         { get; }

        /// <summary>
        /// Whether the booking was canceled, why and by whom.
        /// </summary>
        [Optional]
        public Cancellation?                      Canceled                  { get; }

        /// <summary>
        /// The required license, plate or access code or nothing if the location is open.
        /// </summary>
        [Optional]
        public IEnumerable<AccessMethod>          AccessMethods             { get; }

        /// <summary>
        /// Authorization reference for the relevant charging session and booking.
        /// </summary>
        [Mandatory]
        public AuthorizationReference             AuthorizationReference    { get; }

        /// <summary>
        /// The accepted booking terms.
        /// </summary>
        [Mandatory]
        public BookingTerms                       BookingTerms              { get; }

        /// <summary>
        /// All the requests made for this booking.
        /// </summary>
        [Mandatory]
        public IEnumerable<BookingRequestStatus>  BookingRequestStatus      { get; }

        /// <summary>
        /// The timestamp when this booking was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public   DateTimeOffset                   Created                   { get; }

        /// <summary>
        /// The timestamp for the last booking change has been made.
        /// </summary>
        [Mandatory]
        public   DateTimeOffset                   LastUpdated               { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this booking used as HTTP ETag.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined)]
        public   String                           ETag                      { get; }

        #endregion

        #region Constructor(s)

        public Booking(Booking_Id                          Id,
                       CountryCode                         CountryCode,
                       Party_Id                            PartyId,
                       Request_Id                          RequestId,
                       Location_Id                         LocationId,
                       IEnumerable<BookingToken>           BookingTokens,
                       IEnumerable<Tariff_Id>              TariffIds,
                       TimeSlot                            Period,
                       ReservationStatus                   ReservationStatus,
                       AuthorizationReference              AuthorizationReference,
                       BookingTerms                        BookingTerms,

                       BookableParkingOptions?             BookableParkingOption   = null,
                       Parking_Id?                         ParkingId               = null,
                       EVSE_UId?                           EVSEUId                 = null,
                       Cancellation?                       Canceled                = null,
                       IEnumerable<AccessMethod>?          AccessMethods           = null,
                       IEnumerable<BookingRequestStatus>?  BookingRequestStatus    = null,

                       DateTimeOffset?                     Created                 = null,
                       DateTimeOffset?                     LastUpdated             = null,
                       String?                             ETag                    = null)

        {

            this.Id                      = Id;
            this.CountryCode             = CountryCode;
            this.PartyId                 = PartyId;
            this.RequestId               = RequestId;
            this.LocationId              = LocationId;
            this.BookingTokens           = BookingTokens.        Distinct();
            this.TariffIds               = TariffIds.            Distinct();
            this.Period                  = Period;
            this.ReservationStatus       = ReservationStatus;
            this.AuthorizationReference  = AuthorizationReference;
            this.BookingTerms            = BookingTerms;

            this.BookableParkingOption   = BookableParkingOption;
            this.ParkingId               = ParkingId;
            this.EVSEUId                 = EVSEUId;
            this.Canceled                = Canceled;
            this.AccessMethods           = AccessMethods?.       Distinct() ?? [];
            this.BookingRequestStatus    = BookingRequestStatus?.Distinct() ?? [];

            var created                  = Created     ?? LastUpdated ?? Timestamp.Now;
            this.Created                 = created;
            this.LastUpdated             = LastUpdated ?? created;

            this.ETag                    = ETag ?? SHA256.HashData(
                                                       ToJSON(
                                                           //CustomCDRSerializer,
                                                           //CustomCDRTokenSerializer,
                                                           //CustomCDRLocationSerializer,
                                                           //CustomEVSEEnergyMeterSerializer,
                                                           //CustomTransparencySoftwareSerializer,
                                                           //CustomTariffSerializer,
                                                           //CustomDisplayTextSerializer,
                                                           //CustomPriceSerializer,
                                                           //CustomPriceLimitSerializer,
                                                           //CustomTariffElementSerializer,
                                                           //CustomPriceComponentSerializer,
                                                           //CustomTaxAmountSerializer,
                                                           //CustomTariffRestrictionsSerializer,
                                                           //CustomEnergyMixSerializer,
                                                           //CustomEnergySourceSerializer,
                                                           //CustomEnvironmentalImpactSerializer,
                                                           //CustomChargingPeriodSerializer,
                                                           //CustomCDRDimensionSerializer,
                                                           //CustomSignedDataSerializer,
                                                           //CustomSignedValueSerializer
                                                       ).ToUTF8Bytes()
                                                   ).ToBase64();

            unchecked
            {

                hashCode = this.Id.                    GetHashCode()        * 67 ^
                           this.CountryCode.           GetHashCode()        * 51 ^
                           this.PartyId.               GetHashCode()        * 59 ^
                           this.RequestId.             GetHashCode()        * 53 ^
                           this.LocationId.            GetHashCode()        * 47 ^
                           this.BookingTokens.         CalcHashCode()       * 43 ^
                           this.TariffIds.             CalcHashCode()       * 41 ^
                           this.Period.                GetHashCode()        * 37 ^
                           this.ReservationStatus.     GetHashCode()        * 31 ^
                           this.AuthorizationReference.GetHashCode()        * 29 ^
                           this.BookingTerms.          GetHashCode()        * 23 ^

                          (this.BookableParkingOption?.GetHashCode()  ?? 0) * 19 ^
                          (this.ParkingId?.            GetHashCode()  ?? 0) * 17 ^
                          (this.EVSEUId?.              GetHashCode()  ?? 0) * 13 ^
                          (this.Canceled?.             GetHashCode()  ?? 0) * 11 ^
                           this.AccessMethods.         CalcHashCode()       *  7 ^
                           this.BookingRequestStatus.  CalcHashCode()       *  5 ^

                           this.Created.               GetHashCode()        *  3 ^
                           this.LastUpdated.           GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ..., CustomBookingParser = null)

        /// <summary>
        /// Parse the given JSON representation of a booking.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="RequestIdURL">An optional request identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomBookingParser">A delegate to parse custom booking JSON objects.</param>
        public static Booking Parse(JObject                                JSON,
                                    CountryCode?                           CountryCodeURL        = null,
                                    Party_Id?                              PartyIdURL            = null,
                                    Request_Id?                            RequestIdURL          = null,
                                    CustomJObjectParserDelegate<Booking>?  CustomBookingParser   = null)
        {

            if (TryParse(JSON,
                         out var booking,
                         out var errorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         RequestIdURL,
                         CustomBookingParser))
            {
                return booking;
            }

            throw new ArgumentException("The given JSON representation of a booking is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Booking, out ErrorResponse, ..., CustomBookingParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a booking.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Booking">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="RequestIdURL">An optional request identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomBookingParser">A delegate to parse custom booking JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out Booking?      Booking,
                                       [NotNullWhen(false)] out String?       ErrorResponse,
                                       CountryCode?                           CountryCodeURL        = null,
                                       Party_Id?                              PartyIdURL            = null,
                                       Request_Id?                            RequestIdURL          = null,
                                       CustomJObjectParserDelegate<Booking>?  CustomBookingParser   = null)
        {

            try
            {

                Booking = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                        [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "booking identification",
                                         Booking_Id.TryParse,
                                         out Booking_Id id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CountryCode               [optional]

                if (JSON.ParseOptional("country_code",
                                       "country code",
                                       CountryCode.TryParse,
                                       out CountryCode? countryCodeBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!CountryCodeURL.HasValue && !countryCodeBody.HasValue)
                {
                    ErrorResponse = "The country code is missing!";
                    return false;
                }

                if (CountryCodeURL.HasValue && countryCodeBody.HasValue && CountryCodeURL.Value != countryCodeBody.Value)
                {
                    ErrorResponse = "The optional country code given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse PartyIdURL                [optional]

                if (JSON.ParseOptional("party_id",
                                       "party identification",
                                       Party_Id.TryParse,
                                       out Party_Id? partyIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!PartyIdURL.HasValue && !partyIdBody.HasValue)
                {
                    ErrorResponse = "The party identification is missing!";
                    return false;
                }

                if (PartyIdURL.HasValue && partyIdBody.HasValue && PartyIdURL.Value != partyIdBody.Value)
                {
                    ErrorResponse = "The optional party identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse RequestId                 [optional]

                if (JSON.ParseOptional("request_id",
                                       "request identification",
                                       Request_Id.TryParse,
                                       out Request_Id? requestIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!RequestIdURL.HasValue && !requestIdBody.HasValue)
                {
                    ErrorResponse = "The Request identification is missing!";
                    return false;
                }

                if (RequestIdURL.HasValue && requestIdBody.HasValue && RequestIdURL.Value != requestIdBody.Value)
                {
                    ErrorResponse = "The optional Request identification given within the JSON body does not match the one given in the URL!";
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

                #region Parse BookingTokens             [mandatory]

                if (!JSON.ParseMandatoryHashSet("booking_tokens",
                                                "booking tokens",
                                                BookingToken.TryParse,
                                                out HashSet<BookingToken> bookingTokens,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TariffIds                 [mandatory]

                if (!JSON.ParseMandatoryHashSet("tariff_id",
                                                "tariff identifications",
                                                Tariff_Id.TryParse,
                                                out HashSet<Tariff_Id> tariffIds,
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

                #region Parse ReservationStatus         [mandatory]

                if (!JSON.ParseMandatory("reservation_status",
                                         "reservation status",
                                         ReservationStatus.TryParse,
                                         out ReservationStatus reservationStatus,
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

                #region Parse BookingTerms              [mandatory]

                if (!JSON.ParseMandatoryJSON("booking_terms",
                                             "booking terms",
                                             BookingTerms.TryParse,
                                             out BookingTerms? bookingTerms,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse BookableParkingOptions    [optional]

                if (JSON.ParseOptionalJSON("bookable_parking_option",
                                           "bookable parking option",
                                           BookableParkingOptions.TryParse,
                                           out BookableParkingOptions? bookableParkingOption,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ParkingId                 [optional]

                if (JSON.ParseOptional("parking_id",
                                       "parking identification",
                                       Parking_Id.TryParse,
                                       out Parking_Id? parkingId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EVSEUId                   [optional]

                if (JSON.ParseOptional("evse_uid",
                                       "EVSE unique identification",
                                       EVSE_UId.TryParse,
                                       out EVSE_UId? evseUId,
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

                #region Parse AccessMethods             [optional]

                if (JSON.ParseOptionalHashSet("access_methods",
                                              "access methods",
                                              AccessMethod.TryParse,
                                              out HashSet<AccessMethod> accessMethods,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse BookingRequestStatus      [optional]

                if (JSON.ParseOptionalHashSet("booking_requests",
                                              "booking requests",
                                              OCPIv2_3_0.BookingRequestStatus.TryParse,
                                              out HashSet<BookingRequestStatus> bookingRequestStatus,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse Created                   [optional, VendorExtension]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTimeOffset? created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated               [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTimeOffset lastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Booking = new Booking(

                              id,
                              countryCodeBody ?? CountryCodeURL!.Value,
                              partyIdBody     ?? PartyIdURL!.    Value,
                              requestIdBody   ?? RequestIdURL!.  Value,
                              locationId,
                              bookingTokens,
                              tariffIds,
                              period,
                              reservationStatus,
                              authorizationReference,
                              bookingTerms,

                              bookableParkingOption,
                              parkingId,
                              evseUId,
                              canceled,
                              accessMethods,
                              bookingRequestStatus,

                              created,
                              lastUpdated

                          );

                if (CustomBookingParser is not null)
                    Booking = CustomBookingParser(JSON,
                                                  Booking);

                return true;

            }
            catch (Exception e)
            {
                Booking        = default;
                ErrorResponse  = "The given JSON representation of a booking is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBookingSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBookingSerializer">A delegate to serialize custom booking JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Booking>? CustomBookingSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                        Id.                            ToString()),
                                 new JProperty("country_code",              CountryCode.                   ToString()),
                                 new JProperty("party_id",                  PartyId.                       ToString()),
                                 new JProperty("request_id",                RequestId.                     ToString()),
                                 new JProperty("location_id",               LocationId.                    ToString()),
                                 new JProperty("booking_tokens",            new JArray(BookingTokens.Select(bookingToken => bookingToken.ToJSON()))),
                                 new JProperty("tariff_id",                 new JArray(TariffIds.    Select(tariffId     => tariffId.    ToString()))),
                                 new JProperty("period",                    Period.                        ToJSON()),
                                 new JProperty("reservation_status",        ReservationStatus.             ToString()),
                                 new JProperty("authorization_reference",   AuthorizationReference.        ToString()),
                                 new JProperty("booking_terms",             BookingTerms.                  ToJSON()),

                           BookableParkingOption is not null
                               ? new JProperty("bookable_parking_option",   BookableParkingOption.         ToJSON())
                               : null,

                           ParkingId.HasValue
                               ? new JProperty("parking_id",                ParkingId.               Value.ToString())
                               : null,

                           EVSEUId.HasValue
                               ? new JProperty("evse_uid",                  EVSEUId.                 Value.ToString())
                               : null,

                           Canceled is not null
                               ? new JProperty("canceled",                  Canceled.                      ToJSON())
                               : null,

                           AccessMethods.Any()
                               ? new JProperty("access_methods",            new JArray(AccessMethods.       Select(accessMethod         => accessMethod.        ToJSON())))
                               : null,

                           BookingRequestStatus is not null
                               ? new JProperty("booking_requests",          new JArray(BookingRequestStatus.Select(bookingRequestStatus => bookingRequestStatus.ToJSON())))
                               : null,

                                 new JProperty("created",                   Created.    ToISO8601()),
                                 new JProperty("last_updated",              LastUpdated.ToISO8601())

                       );

            return CustomBookingSerializer is not null
                       ? CustomBookingSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this booking.
        /// </summary>
        public Booking Clone()

            => new (
                   Id.                    Clone(),
                   CountryCode.           Clone(),
                   PartyId.               Clone(),
                   RequestId.             Clone(),
                   LocationId.            Clone(),
                   BookingTokens.  Select(bookingToken   => bookingToken.  Clone()),
                   TariffIds.      Select(tariffId       => tariffId.      Clone()),
                   Period.                Clone(),
                   ReservationStatus.     Clone(),
                   AuthorizationReference.Clone(),
                   BookingTerms.          Clone(),

                   BookableParkingOption?.Clone(),
                   ParkingId?.            Clone(),
                   EVSEUId?.              Clone(),
                   Canceled?.             Clone(),
                   AccessMethods.  Select(accessMethod   => accessMethod.  Clone()),
                   BookingRequestStatus.Select(bookingRequest => bookingRequest.Clone()),
                   Created,
                   LastUpdated,
                   ETag.                  CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (Booking1, Booking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Booking1">A booking.</param>
        /// <param name="Booking2">Another booking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Booking? Booking1,
                                           Booking? Booking2)
        {

            if (Object.ReferenceEquals(Booking1, Booking2))
                return true;

            if (Booking1 is null || Booking2 is null)
                return false;

            return Booking1.Equals(Booking2);

        }

        #endregion

        #region Operator != (Booking1, Booking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Booking1">A booking.</param>
        /// <param name="Booking2">Another booking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Booking? Booking1,
                                           Booking? Booking2)

            => !(Booking1 == Booking2);

        #endregion

        #region Operator <  (Booking1, Booking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Booking1">A booking.</param>
        /// <param name="Booking2">Another booking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Booking? Booking1,
                                          Booking? Booking2)

            => Booking1 is null
                   ? throw new ArgumentNullException(nameof(Booking1), "The given booking must not be null!")
                   : Booking1.CompareTo(Booking2) < 0;

        #endregion

        #region Operator <= (Booking1, Booking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Booking1">A booking.</param>
        /// <param name="Booking2">Another booking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Booking? Booking1,
                                           Booking? Booking2)

            => !(Booking1 > Booking2);

        #endregion

        #region Operator >  (Booking1, Booking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Booking1">A booking.</param>
        /// <param name="Booking2">Another booking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Booking? Booking1,
                                          Booking? Booking2)

            => Booking1 is null
                   ? throw new ArgumentNullException(nameof(Booking1), "The given booking must not be null!")
                   : Booking1.CompareTo(Booking2) > 0;

        #endregion

        #region Operator >= (Booking1, Booking2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Booking1">A booking.</param>
        /// <param name="Booking2">Another booking.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Booking? Booking1,
                                           Booking? Booking2)

            => !(Booking1 < Booking2);

        #endregion

        #endregion

        #region IComparable<Booking> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two bookings.
        /// </summary>
        /// <param name="Object">A booking to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Booking booking
                   ? CompareTo(booking)
                   : throw new ArgumentException("The given object is not a booking!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Booking)

        /// <summary>s
        /// Compares two bookings.
        /// </summary>
        /// <param name="Object">A booking to compare with.</param>
        public Int32 CompareTo(Booking? Booking)
        {

            if (Booking is null)
                throw new ArgumentNullException(nameof(Booking), "The given booking must not be null!");

            var c = Id.         CompareTo(Booking.Id);

            //if (c == 0)
            //    c = BeginFrom.  CompareTo(Booking.BeginFrom);

            //if (c == 0)
            //    c = EndBefore.  CompareTo(Booking.EndBefore);

            //if (c == 0)
            //    c = LastUpdated.CompareTo(Booking.LastUpdated);

            //if (c == 0)
            //    c = AvailableTimeSlots.Order().AggregateWith(",").CompareTo(Booking.AvailableTimeSlots.Order().AggregateWith(","));

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Booking> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two bookings for equality.
        /// </summary>
        /// <param name="Object">Booking to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Booking booking &&
                   Equals(booking);

        #endregion

        #region Equals(Booking)

        /// <summary>
        /// Compares two bookings for equality.
        /// </summary>
        /// <param name="Booking">Booking to compare with.</param>
        public Boolean Equals(Booking? Booking)

            => Booking is not null &&

               Id.                        Equals       (Booking.Id)          ;
            //   BeginFrom.                 Equals       (Booking.BeginFrom)   &&
            //   EndBefore.                 Equals       (Booking.EndBefore)   &&
            //   AvailableTimeSlots.Order().SequenceEqual(Booking.AvailableTimeSlots.Order()) &&
            //   LastUpdated.               Equals       (Booking.LastUpdated) &&

            //((!StepSize.HasValue && !Booking.StepSize.HasValue) ||
            //  (StepSize.HasValue &&  Booking.StepSize.HasValue && StepSize.Value.Equals(Booking.StepSize.Value)));

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

                   $"{Id}: {CountryCode}*{PartyId} / {RequestId}",

                   EVSEUId.HasValue
                       ? ", EVSE UId: " + EVSEUId.Value
                       : "",

                   Canceled is not null
                       ? $", canceled by '{Canceled.WhoCanceled}' because of '{Canceled.CancellationReason}'"
                       : ""

               );

        #endregion

    }

}
