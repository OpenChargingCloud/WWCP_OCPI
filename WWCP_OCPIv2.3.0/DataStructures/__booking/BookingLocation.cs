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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Booking Location
    /// </summary>
    public class BookingLocation : IEquatable<BookingLocation>,
                                   IComparable<BookingLocation>,
                                   IComparable
    {

        #region Properties

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this booking location.
        /// </summary>
        [Mandatory]
        public CountryCode                          CountryCode               { get; }

        /// <summary>
        /// The identification of the charge point operator that 'owns' this booking location
        /// (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id                             PartyId                   { get; }

        /// <summary>
        /// The unique identification of the booking location within the CPO platform.
        /// </summary>
        [Mandatory]
        public BookingLocation_Id                   BookingLocationId         { get; }

        /// <summary>
        /// Location identification of the location of this CPO, on which the reservation can be made.
        /// </summary>
        [Mandatory]
        public Location_Id                          LocationId                { get; }

        /// <summary>
        /// The optional enumeration of bookable EVSEs at this location on which the reservation will be made.
        /// Allowed to be set to: #NA when no EVSE yet assigned to the driver.
        /// This reference will be provided in the relevant Booking and/or CDR.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE_UId>                EVSEUIds                  { get; }

        /// <summary>
        /// The optional enumeration of parking specification that can be booked by drivers
        /// that want to charge at this location.
        /// </summary>
        [Optional]
        public IEnumerable<BookableParkingOptions>  BookableParkingOptions    { get; }

        /// <summary>
        /// The number of charging stations that are bookable at this location and if this is required.
        /// </summary>
        [Optional]
        public Bookable?                            Bookable                  { get; }

        /// <summary>
        /// The optional enumeration of tariff identifications.
        /// </summary>
        [Optional]
        public IEnumerable<Tariff_Id>               TariffIds                 { get; }

        /// <summary>
        /// The optional enumeration of booking terms for booking at this location.
        /// </summary>
        [Optional]
        public IEnumerable<BookingTerms>            BookingTerms              { get; }

        /// <summary>
        /// The optional enumeration of calendars to display the availability on this location.
        /// </summary>
        [Optional]
        public IEnumerable<Calendar>                Calendar                  { get; }


        /// <summary>
        /// The timestamp when this booking location was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public   DateTimeOffset                     Created                   { get; }

        /// <summary>
        /// The timestamp for the last bookingLocation change has been made.
        /// </summary>
        [Mandatory]
        public   DateTimeOffset                     LastUpdated               { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this booking location used as HTTP ETag.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined)]
        public   String                             ETag                      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new booking location.
        /// </summary>
        /// <param name="CountryCode">The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this booking location.</param>
        /// <param name="PartyId">The identification of the charge point operator that 'owns' this booking location (following the ISO-15118 standard).</param>
        /// <param name="BookingLocationId">The unique identification of the booking location within the CPO platform.</param>
        /// <param name="LocationId">The location identification of the location of this CPO, on which the reservation can be made.</param>
        /// <param name="EVSEUIds">An optional enumeration of bookable EVSEs at this location on which the reservation will be made.</param>
        /// <param name="BookableParkingOptions">An optional enumeration of parking specification that can be booked by drivers that want to charge at this location.</param>
        /// <param name="Bookable">An optional number of charging stations that are bookable at this location and if this is required.</param>
        /// <param name="TariffIds">An optional enumeration of tariff identifications.</param>
        /// <param name="BookingTerms">An optional enumeration of booking terms for booking at this location.</param>
        /// <param name="Calendar">An optional enumeration of calendars to display the availability on this location.</param>
        /// 
        /// <param name="Created">An optional timestamp when this charge detail record was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this charge detail record was last updated (or created).</param>
        public BookingLocation(CountryCode                           CountryCode,
                               Party_Id                              PartyId,
                               BookingLocation_Id                    BookingLocationId,
                               Location_Id                           LocationId,
                               IEnumerable<EVSE_UId>?                EVSEUIds,
                               IEnumerable<BookableParkingOptions>?  BookableParkingOptions   = null,
                               Bookable?                             Bookable                 = null,
                               IEnumerable<Tariff_Id>?               TariffIds                = null,
                               IEnumerable<BookingTerms>?            BookingTerms             = null,
                               IEnumerable<Calendar>?                Calendar                 = null,

                               DateTimeOffset?                       Created                  = null,
                               DateTimeOffset?                       LastUpdated              = null,
                               String?                               ETag                     = null)

        {

            this.CountryCode               = CountryCode;
            this.PartyId                   = PartyId;
            this.BookingLocationId         = BookingLocationId;
            this.LocationId                = LocationId;
            this.EVSEUIds                  = EVSEUIds?.              Distinct() ?? [];
            this.BookableParkingOptions    = BookableParkingOptions?.Distinct() ?? [];
            this.Bookable                  = Bookable;
            this.TariffIds                 = TariffIds?.             Distinct() ?? [];
            this.BookingTerms              = BookingTerms?.          Distinct() ?? [];
            this.Calendar                  = Calendar?.              Distinct() ?? [];

            var created                    = Created     ?? LastUpdated ?? Timestamp.Now;
            this.Created                   = created;
            this.LastUpdated               = LastUpdated ?? created;

            this.ETag                      = ETag ?? SHA256.HashData(
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

                hashCode = this.CountryCode.           GetHashCode()       * 37 ^
                           this.PartyId.               GetHashCode()       * 31 ^
                           this.BookingLocationId.     GetHashCode()       * 29 ^
                           this.LocationId.            GetHashCode()       * 23 ^
                           this.EVSEUIds.              CalcHashCode()      * 19 ^
                           this.BookableParkingOptions.CalcHashCode()      * 17^
                          (this.Bookable?.             GetHashCode() ?? 0) * 13 ^
                           this.TariffIds.             CalcHashCode()      * 11 ^
                           this.BookingTerms.          CalcHashCode()      *  7 ^
                           this.Calendar.              CalcHashCode()      *  5 ^
                           this.Created.               GetHashCode()       *  3 ^
                           this.LastUpdated.           GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ..., CustomBookingLocationParser = null)

        /// <summary>
        /// Parse the given JSON representation of a booking location.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="BookingLocationIdURL">An optional booking location identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomBookingLocationParser">A delegate to parse custom bookingLocation JSON objects.</param>
        public static BookingLocation Parse(JObject                                        JSON,
                                            CountryCode?                                   CountryCodeURL                = null,
                                            Party_Id?                                      PartyIdURL                    = null,
                                            BookingLocation_Id?                            BookingLocationIdURL          = null,
                                            CustomJObjectParserDelegate<BookingLocation>?  CustomBookingLocationParser   = null)
        {

            if (TryParse(JSON,
                         out var bookingLocation,
                         out var errorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         BookingLocationIdURL,
                         CustomBookingLocationParser))
            {
                return bookingLocation;
            }

            throw new ArgumentException("The given JSON representation of a booking location is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out BookingLocation, out ErrorResponse, ..., CustomBookingLocationParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a booking location.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="BookingLocation">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="BookingLocationIdURL">An optional booking location identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomBookingLocationParser">A delegate to parse custom bookingLocation JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out BookingLocation?      BookingLocation,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CountryCode?                                   CountryCodeURL                = null,
                                       Party_Id?                                      PartyIdURL                    = null,
                                       BookingLocation_Id?                            BookingLocationIdURL          = null,
                                       CustomJObjectParserDelegate<BookingLocation>?  CustomBookingLocationParser   = null)
        {

            try
            {

                BookingLocation = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

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

                #region Parse BookingLocationId         [optional]

                if (JSON.ParseOptional("id",
                                       "booking location identification",
                                       BookingLocation_Id.TryParse,
                                       out BookingLocation_Id? bookingLocationIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!BookingLocationIdURL.HasValue && !bookingLocationIdBody.HasValue)
                {
                    ErrorResponse = "The booking location identification is missing!";
                    return false;
                }

                if (BookingLocationIdURL.HasValue && bookingLocationIdBody.HasValue && BookingLocationIdURL.Value != bookingLocationIdBody.Value)
                {
                    ErrorResponse = "The optional booking location identification given within the JSON body does not match the one given in the URL!";
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

                #region Parse EVSEUIds                  [optional]

                if (JSON.ParseOptionalHashSet("evse_uid",
                                              "EVSE unique identifications",
                                              EVSE_UId.TryParse,
                                              out HashSet<EVSE_UId> evseUIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse BookableParkingOptions    [optional]

                if (JSON.ParseOptionalHashSet("bookable_parking_options",
                                              "bookable parking options",
                                              OCPIv2_3_0.BookableParkingOptions.TryParse,
                                              out HashSet<BookableParkingOptions> bookableParkingOptions,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Bookable                  [optional]

                if (JSON.ParseOptionalJSON("bookable",
                                           "bookable",
                                           Bookable.TryParse,
                                           out Bookable? bookable,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TariffIds                 [optional]

                if (JSON.ParseOptionalHashSet("tariff_id",
                                              "tariff identifications",
                                              Tariff_Id.TryParse,
                                              out HashSet<Tariff_Id> tariffIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse BookingTerms              [optional]

                if (JSON.ParseOptionalHashSet("booking_terms",
                                              "booking terms",
                                              OCPIv2_3_0.BookingTerms.TryParse,
                                              out HashSet<BookingTerms> bookingTerms,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Calendar                  [optional]

                if (JSON.ParseOptionalHashSet("calendar",
                                              "calendar",
                                              OCPIv2_3_0.Calendar.TryParse,
                                              out HashSet<Calendar> calendar,
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


                BookingLocation = new BookingLocation(

                                      countryCodeBody       ?? CountryCodeURL!.      Value,
                                      partyIdBody           ?? PartyIdURL!.          Value,
                                      bookingLocationIdBody ?? BookingLocationIdURL!.Value,
                                      locationId,
                                      evseUIds,
                                      bookableParkingOptions,
                                      bookable,
                                      tariffIds,
                                      bookingTerms,
                                      calendar,

                                      created,
                                      lastUpdated

                                  );

                if (CustomBookingLocationParser is not null)
                    BookingLocation = CustomBookingLocationParser(JSON,
                                                                  BookingLocation);

                return true;

            }
            catch (Exception e)
            {
                BookingLocation  = default;
                ErrorResponse    = "The given JSON representation of a booking location is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBookingLocationSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBookingLocationSerializer">A delegate to serialize custom bookingLocation JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BookingLocation>? CustomBookingLocationSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("country_code",               CountryCode.      ToString()),
                                 new JProperty("party_id",                   PartyId.          ToString()),
                                 new JProperty("id",                         BookingLocationId.ToString()),
                                 new JProperty("location_id",                LocationId.       ToString()),

                           EVSEUIds.Any()
                               ? new JProperty("evse_uid",                   new JArray(EVSEUIds.              Select(evseUId               => evseUId.              ToString())))
                               : null,

                           BookableParkingOptions.Any()
                               ? new JProperty("bookable_parking_options",   new JArray(BookableParkingOptions.Select(bookableParkingOption => bookableParkingOption.ToString())))
                               : null,

                           Bookable is not null
                               ? new JProperty("bookable",                   Bookable.         ToJSON())
                               : null,

                           TariffIds.Any()
                               ? new JProperty("tariff_id",                  new JArray(TariffIds.             Select(tariffId              => tariffId.             ToString())))
                               : null,

                           BookingTerms.Any()
                               ? new JProperty("booking_terms",              new JArray(BookingTerms.          Select(bookingTerm           => bookingTerm.          ToString())))
                               : null,

                           Calendar.Any()
                               ? new JProperty("calendar",                   new JArray(Calendar.              Select(calendar              => calendar.             ToString())))
                               : null,

                                 new JProperty("created",                    Created.    ToISO8601()),
                                 new JProperty("last_updated",               LastUpdated.ToISO8601())

                       );

            return CustomBookingLocationSerializer is not null
                       ? CustomBookingLocationSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this booking location.
        /// </summary>
        public BookingLocation Clone()

            => new (

                   CountryCode.           Clone(),
                   PartyId.               Clone(),
                   BookingLocationId.     Clone(),
                   LocationId.            Clone(),
                   EVSEUIds.              Select(evseUIds              => evseUIds.             Clone()),
                   BookableParkingOptions.Select(bookableParkingOption => bookableParkingOption.Clone()),
                   Bookable?.             Clone(),
                   TariffIds.             Select(tariffId              => tariffId.             Clone()),
                   BookingTerms.          Select(bookingTerm           => bookingTerm.          Clone()),
                   Calendar.              Select(calendar              => calendar.             Clone()),

                   Created,
                   LastUpdated,
                   ETag.                  CloneString()

               );

        #endregion


        #region Operator overloading

        #region Operator == (BookingLocation1, BookingLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingLocation1">A booking location.</param>
        /// <param name="BookingLocation2">Another booking location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BookingLocation? BookingLocation1,
                                           BookingLocation? BookingLocation2)
        {

            if (Object.ReferenceEquals(BookingLocation1, BookingLocation2))
                return true;

            if (BookingLocation1 is null || BookingLocation2 is null)
                return false;

            return BookingLocation1.Equals(BookingLocation2);

        }

        #endregion

        #region Operator != (BookingLocation1, BookingLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingLocation1">A booking location.</param>
        /// <param name="BookingLocation2">Another booking location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BookingLocation? BookingLocation1,
                                           BookingLocation? BookingLocation2)

            => !(BookingLocation1 == BookingLocation2);

        #endregion

        #region Operator <  (BookingLocation1, BookingLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingLocation1">A booking location.</param>
        /// <param name="BookingLocation2">Another booking location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BookingLocation? BookingLocation1,
                                          BookingLocation? BookingLocation2)

            => BookingLocation1 is null
                   ? throw new ArgumentNullException(nameof(BookingLocation1), "The given bookingLocation must not be null!")
                   : BookingLocation1.CompareTo(BookingLocation2) < 0;

        #endregion

        #region Operator <= (BookingLocation1, BookingLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingLocation1">A booking location.</param>
        /// <param name="BookingLocation2">Another booking location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BookingLocation? BookingLocation1,
                                           BookingLocation? BookingLocation2)

            => !(BookingLocation1 > BookingLocation2);

        #endregion

        #region Operator >  (BookingLocation1, BookingLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingLocation1">A booking location.</param>
        /// <param name="BookingLocation2">Another booking location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BookingLocation? BookingLocation1,
                                          BookingLocation? BookingLocation2)

            => BookingLocation1 is null
                   ? throw new ArgumentNullException(nameof(BookingLocation1), "The given bookingLocation must not be null!")
                   : BookingLocation1.CompareTo(BookingLocation2) > 0;

        #endregion

        #region Operator >= (BookingLocation1, BookingLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BookingLocation1">A booking location.</param>
        /// <param name="BookingLocation2">Another booking location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BookingLocation? BookingLocation1,
                                           BookingLocation? BookingLocation2)

            => !(BookingLocation1 < BookingLocation2);

        #endregion

        #endregion

        #region IComparable<BookingLocation> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two booking locations.
        /// </summary>
        /// <param name="Object">A booking location to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is BookingLocation bookingLocation
                   ? CompareTo(bookingLocation)
                   : throw new ArgumentException("The given object is not a booking location!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BookingLocation)

        /// <summary>s
        /// Compares two booking locations.
        /// </summary>
        /// <param name="Object">A booking location to compare with.</param>
        public Int32 CompareTo(BookingLocation? BookingLocation)
        {

            if (BookingLocation is null)
                throw new ArgumentNullException(nameof(BookingLocation), "The given bookingLocation must not be null!");

            var c = CountryCode.      CompareTo(BookingLocation.CountryCode);

            if (c == 0)
                c = PartyId.          CompareTo(BookingLocation.PartyId);

            if (c == 0)
                c = BookingLocationId.CompareTo(BookingLocation.BookingLocationId);

            if (c == 0)
                c = LocationId.       CompareTo(BookingLocation.LocationId);

            if (c == 0)
                c = Created.          CompareTo(BookingLocation.Created);

            if (c == 0)
                c = LastUpdated.      CompareTo(BookingLocation.LastUpdated);


            if (c == 0 && Bookable is not null && BookingLocation.Bookable is not null)
                c = Bookable.CompareTo(BookingLocation.Bookable);


            if (c == 0)
                c = EVSEUIds.              Order().AggregateWith(",").CompareTo(BookingLocation.EVSEUIds.              Order().AggregateWith(","));

            if (c == 0)
                c = BookableParkingOptions.Order().AggregateWith(",").CompareTo(BookingLocation.BookableParkingOptions.Order().AggregateWith(","));

            if (c == 0)
                c = TariffIds.             Order().AggregateWith(",").CompareTo(BookingLocation.TariffIds.             Order().AggregateWith(","));

            if (c == 0)
                c = BookingTerms.          Order().AggregateWith(",").CompareTo(BookingLocation.BookingTerms.          Order().AggregateWith(","));

            if (c == 0)
                c = Calendar.              Order().AggregateWith(",").CompareTo(BookingLocation.Calendar.              Order().AggregateWith(","));

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<BookingLocation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two booking locations for equality.
        /// </summary>
        /// <param name="Object">BookingLocation to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BookingLocation bookingLocation &&
                   Equals(bookingLocation);

        #endregion

        #region Equals(BookingLocation)

        /// <summary>
        /// Compares two booking locations for equality.
        /// </summary>
        /// <param name="BookingLocation">BookingLocation to compare with.</param>
        public Boolean Equals(BookingLocation? BookingLocation)

            => BookingLocation is not null &&

               CountryCode.      Equals(BookingLocation.CountryCode)       &&
               PartyId.          Equals(BookingLocation.PartyId)           &&
               BookingLocationId.Equals(BookingLocation.BookingLocationId) &&
               LocationId.       Equals(BookingLocation.LocationId)        &&

               Created.          Equals(BookingLocation.Created)           &&
               LastUpdated.      Equals(BookingLocation.LastUpdated)       &&

               EVSEUIds.              Order().SequenceEqual(BookingLocation.EVSEUIds.              Order()) &&
               BookableParkingOptions.Order().SequenceEqual(BookingLocation.BookableParkingOptions.Order()) &&
               TariffIds.             Order().SequenceEqual(BookingLocation.TariffIds.             Order()) &&
               BookingTerms.          Order().SequenceEqual(BookingLocation.BookingTerms.          Order()) &&
               Calendar.              Order().SequenceEqual(BookingLocation.Calendar.              Order()) &&

             ((Bookable is null     && BookingLocation.Bookable is null) ||
              (Bookable is not null && BookingLocation.Bookable is not null && Bookable.Equals(BookingLocation.Bookable)));

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

                   $"{CountryCode}*{PartyId} / {BookingLocationId} / {LocationId}",

                   EVSEUIds.Any()
                       ? $", EVSE UIds: '{EVSEUIds.AggregateWith(", ")}'"
                       : ""

               );

        #endregion

    }

}
