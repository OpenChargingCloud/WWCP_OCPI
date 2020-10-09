/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// A Tariff Object consists of a list of one or more TariffElements,
    /// these elements can be used to create complex Tariff structures.
    /// When the list of elements contains more then 1 element, then the
    /// first tariff in the list with matching restrictions will be used.
    /// </summary>
    public class Tariff : IHasId<Tariff_Id>,
                          IEquatable<Tariff>,
                          IComparable<Tariff>,
                          IComparable
    {

        #region Properties

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the CPO that 'owns' this session.
        /// </summary>
        [Optional]
        public CountryCode                      CountryCode             { get; }

        /// <summary>
        /// The Id of the CPO that 'owns' this session (following the ISO-15118 standard).
        /// </summary>
        [Optional]
        public Party_Id                         PartyId                 { get; }

        /// <summary>
        /// The identification of the tariff within the CPOs platform (and suboperator platforms). 
        /// </summary>
        [Mandatory]
        public Tariff_Id                        Id                      { get; }

        /// <summary>
        /// ISO 4217 code of the currency used for this tariff.
        /// </summary>
        [Mandatory]
        public Currency                         Currency                { get; }

        /// <summary>
        /// Defines the type of the tariff. This allows for distinction in case of given
        /// charging preferences. When omitted, this tariff is valid for all charging sessions.
        /// </summary>
        [Optional]
        public TariffTypes?                     TariffType              { get; }

        /// <summary>
        /// Multi-language alternative tariff info text.
        /// </summary>
        [Optional]
        public IEnumerable<DisplayText>         TariffAltText           { get; }

        /// <summary>
        /// URL to a web page that contains an explanation of the tariff information
        /// in human readable form.
        /// </summary>
        [Optional]
        public String                           TariffAltUrl            { get; }

        /// <summary>
        /// When this field is set, a Charging Session with this tariff will at least cost
        /// this amount. This is different from a FLAT fee (Start Tariff, Transaction Fee),
        /// as a FLAT fee is a fixed amount that has to be paid for any Charging Session.
        /// A minimum price indicates that when the cost of a charging session is lower
        /// than this amount, the cost of the charging session will be equal to this amount.
        /// </summary>
        [Optional]
        public Price?                           MinPrice                { get; }

        /// <summary>
        /// When this field is set, a charging session with this tariff will NOT cost more
        /// than this amount.
        /// </summary>
        [Optional]
        public Price?                           MaxPrice                { get; }

        /// <summary>
        /// An enumeration of tariff elements.
        /// </summary>
        [Mandatory]
        public IEnumerable<TariffElement>       TariffElements          { get; }

        /// <summary>
        /// The time when this tariff becomes active, in UTC, time_zone field of the
        /// charging location can be used to convert to local time. Typically used for
        /// a new tariff that is already given with the charging location, before it
        /// becomes active.
        /// </summary>
        [Optional]
        public DateTime?                        Start                   { get; }

        /// <summary>
        /// The time after which this tariff is no longer valid, in UTC, time_zone field
        /// if the charging location can be used to convert to local time. Typically used
        /// when this tariff is going to be replaced with a different tariff in the near
        /// future.
        /// </summary>
        [Optional]
        public DateTime?                        End                     { get; }

        /// <summary>
        /// Details on the energy supplied with this tariff.
        /// </summary>
        [Optional]
        public EnergyMix                        EnergyMix               { get;  }

        /// <summary>
        /// Timestamp when this tariff was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                         LastUpdated             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new chrging tariff.
        /// </summary>
        public Tariff(CountryCode                 CountryCode,
                      Party_Id                    PartyId,
                      Tariff_Id                   Id ,
                      Currency                    Currency,
                      IEnumerable<TariffElement>  TariffElements,

                      TariffTypes?                TariffType      = null,
                      IEnumerable<DisplayText>    TariffAltText   = null,
                      String                      TariffAltUrl    = null,
                      Price?                      MinPrice        = null,
                      Price?                      MaxPrice        = null,
                      DateTime?                   Start           = null,
                      DateTime?                   End             = null,
                      EnergyMix                   EnergyMix       = null,
                      DateTime?                   LastUpdated     = null)

        {

            #region Initial checks

            if (!TariffElements.SafeAny())
                throw new ArgumentNullException(nameof(TariffElements),  "The given enumeration of tariff elements must not be null or empty!");

            #endregion

            this.CountryCode      = CountryCode;
            this.PartyId          = PartyId;
            this.Id               = Id;
            this.Currency         = Currency;
            this.TariffElements   = TariffElements.Distinct();

            this.TariffType       = TariffType;
            this.TariffAltText    = TariffAltText?.Distinct() ?? new DisplayText[0];
            this.TariffAltUrl     = TariffAltUrl;
            this.MinPrice         = MinPrice;
            this.MaxPrice         = MaxPrice;
            this.Start            = Start;
            this.End              = End;
            this.EnergyMix        = EnergyMix;

            this.LastUpdated      = LastUpdated ?? DateTime.Now;

        }

        #endregion


        #region (static) Parse   (JSON, TariffIdURL = null, CustomTariffParser = null)

        /// <summary>
        /// Parse the given JSON representation of a tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffIdURL">An optional tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTariffParser">A delegate to parse custom tariff JSON objects.</param>
        public static Tariff Parse(JObject                              JSON,
                                   CountryCode?                         CountryCodeURL       = null,
                                   Party_Id?                            PartyIdURL           = null,
                                   Tariff_Id?                           TariffIdURL          = null,
                                   CustomJObjectParserDelegate<Tariff>  CustomTariffParser   = null)
        {

            if (TryParse(JSON,
                         out Tariff  tariff,
                         out String  ErrorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         TariffIdURL,
                         CustomTariffParser))
            {
                return tariff;
            }

            throw new ArgumentException("The given JSON representation of a tariff is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, TariffIdURL = null, CustomTariffParser = null)

        /// <summary>
        /// Parse the given text representation of a tariff.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="TariffIdURL">An optional tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTariffParser">A delegate to parse custom tariff JSON objects.</param>
        public static Tariff Parse(String                               Text,
                                   CountryCode?                         CountryCodeURL       = null,
                                   Party_Id?                            PartyIdURL           = null,
                                   Tariff_Id?                           TariffIdURL          = null,
                                   CustomJObjectParserDelegate<Tariff>  CustomTariffParser   = null)
        {

            if (TryParse(Text,
                         out Tariff  tariff,
                         out String  ErrorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         TariffIdURL,
                         CustomTariffParser))
            {
                return tariff;
            }

            throw new ArgumentException("The given text representation of a tariff is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out Tariff, out ErrorResponse, TariffIdURL = null, CustomTariffParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Tariff">The parsed tariff.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject     JSON,
                                       out Tariff  Tariff,
                                       out String  ErrorResponse)

            => TryParse(JSON,
                        out Tariff,
                        out ErrorResponse,
                        null,
                        null,
                        null,
                        null);

        /// <summary>
        /// Try to parse the given JSON representation of a tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Tariff">The parsed tariff.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="TariffIdURL">An optional tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTariffParser">A delegate to parse custom tariff JSON objects.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       out Tariff                           Tariff,
                                       out String                           ErrorResponse,
                                       CountryCode?                         CountryCodeURL       = null,
                                       Party_Id?                            PartyIdURL           = null,
                                       Tariff_Id?                           TariffIdURL          = null,
                                       CustomJObjectParserDelegate<Tariff>  CustomTariffParser   = null)
        {

            try
            {

                Tariff = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CountryCode           [optional]

                if (JSON.ParseOptionalStruct("country_code",
                                             "country code",
                                             CountryCode.TryParse,
                                             out CountryCode? CountryCodeBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!CountryCodeURL.HasValue && !CountryCodeBody.HasValue)
                {
                    ErrorResponse = "The country code is missing!";
                    return false;
                }

                if (CountryCodeURL.HasValue && CountryCodeBody.HasValue && CountryCodeURL.Value != CountryCodeBody.Value)
                {
                    ErrorResponse = "The optional country code given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse PartyIdURL            [optional]

                if (JSON.ParseOptionalStruct("party_id",
                                             "party identification",
                                             Party_Id.TryParse,
                                             out Party_Id? PartyIdBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!PartyIdURL.HasValue && !PartyIdBody.HasValue)
                {
                    ErrorResponse = "The party identification is missing!";
                    return false;
                }

                if (PartyIdURL.HasValue && PartyIdBody.HasValue && PartyIdURL.Value != PartyIdBody.Value)
                {
                    ErrorResponse = "The optional party identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Id                    [optional]

                if (JSON.ParseOptionalStruct("id",
                                             "tariff identification",
                                             Tariff_Id.TryParse,
                                             out Tariff_Id? TariffIdBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!TariffIdURL.HasValue && !TariffIdBody.HasValue)
                {
                    ErrorResponse = "The tariff identification is missing!";
                    return false;
                }

                if (TariffIdURL.HasValue && TariffIdBody.HasValue && TariffIdURL.Value != TariffIdBody.Value)
                {
                    ErrorResponse = "The optional tariff identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Publish               [mandatory]

                if (!JSON.ParseMandatory("publish",
                                         "publish",
                                         out Boolean Publish,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Address               [mandatory]

                if (!JSON.ParseMandatoryText("address",
                                             "address",
                                             out String Address,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse City                  [mandatory]

                if (!JSON.ParseMandatoryText("city",
                                             "city",
                                             out String City,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Country               [mandatory]

                if (!JSON.ParseMandatoryText("country",
                                             "country",
                                             out String Country,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Coordinates           [mandatory]

                //if (!JSON.ParseMandatoryJSON("coordinates",
                //                             "geo coordinates",
                //                             GeoCoordinate.TryParse,
                //                             out GeoCoordinate Coordinates,
                //                             out ErrorResponse))
                //{
                //    return false;
                //}

                #endregion

                #region Parse TimeZone              [mandatory]

                if (!JSON.ParseMandatoryText("time_zone",
                                             "time zone",
                                             out String TimeZone,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse PublishTokenTypes     [optional]

                if (JSON.ParseOptionalJSON("publish_allowed_to",
                                           "publish allowed to",
                                           PublishTokenType.TryParse,
                                           out IEnumerable<PublishTokenType> PublishTokenTypes,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Name                  [optional]

                var Name = JSON.GetString("name");

                #endregion

                #region Parse PostalCode            [optional]

                var PostalCode = JSON.GetString("postal_code");

                #endregion

                #region Parse State                 [optional]

                var State = JSON.GetString("state");

                #endregion

                #region Parse RelatedTariffs      [optional]

                //if (JSON.ParseOptionalJSON("related_tariffs",
                //                           "related tariffs",
                //                           AdditionalGeoTariff.TryParse,
                //                           out IEnumerable<AdditionalGeoTariff> RelatedTariffs,
                //                           out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                #endregion

                #region Parse ParkingType           [optional]

                if (JSON.ParseOptionalEnum("parking_type",
                                           "parking type",
                                           out ParkingTypes? ParkingType,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse EVSEs                 [optional]

                if (JSON.ParseOptionalJSON("evses",
                                           "evses",
                                           EVSE.TryParse,
                                           out IEnumerable<EVSE> EVSEs,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Directions            [optional]

                if (JSON.ParseOptionalJSON("directions",
                                           "multi-language directions",
                                           DisplayText.TryParse,
                                           out IEnumerable<DisplayText> Directions,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Operator              [optional]

                if (JSON.ParseOptionalJSON("operator",
                                           "operator",
                                           BusinessDetails.TryParse,
                                           out BusinessDetails Operator,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Suboperator           [optional]

                if (JSON.ParseOptionalJSON("suboperator",
                                           "suboperator",
                                           BusinessDetails.TryParse,
                                           out BusinessDetails Suboperator,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Owner                 [optional]

                if (JSON.ParseOptionalJSON("owner",
                                           "owner",
                                           BusinessDetails.TryParse,
                                           out BusinessDetails Owner,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Facilities            [optional]

                if (JSON.ParseOptionalEnums("facilities",
                                            "facilities",
                                            out IEnumerable<Facilities> Facilities,
                                            out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse OpeningTimes          [optional]

                if (JSON.ParseOptionalJSON("opening_times",
                                           "opening times",
                                           Hours.TryParse,
                                           out Hours OpeningTimes,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse ChargingWhenClosed    [optional]

                if (JSON.ParseOptional("charging_when_closed",
                                       "charging when closed",
                                       out Boolean? ChargingWhenClosed,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Images                [optional]

                if (JSON.ParseOptionalJSON("images",
                                           "images",
                                           Image.TryParse,
                                           out IEnumerable<Image> Images,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse EnergyMix             [optional]

                if (JSON.ParseOptionalJSON("energy_mix",
                                           "energy mix",
                                           OCPIv2_2.EnergyMix.TryParse,
                                           out EnergyMix EnergyMix,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                #region Parse LastUpdated           [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                //Tariff = new Tariff(CountryCodeBody ?? CountryCodeURL.Value,
                //                        PartyIdBody     ?? PartyIdURL.Value,
                //                        TariffIdBody  ?? TariffIdURL.Value,
                //                        Publish,
                //                        Address?.   Trim(),
                //                        City?.      Trim(),
                //                        Country?.   Trim(),
                //                        Coordinates,
                //                        TimeZone?.  Trim(),

                //                        PublishTokenTypes,
                //                        Name?.      Trim(),
                //                        PostalCode?.Trim(),
                //                        State?.     Trim(),
                //                        RelatedTariffs?.Distinct(),
                //                        ParkingType,
                //                        EVSEs?.           Distinct(),
                //                        Directions?.      Distinct(),
                //                        Operator,
                //                        Suboperator,
                //                        Owner,
                //                        Facilities?.      Distinct(),
                //                        OpeningTimes,
                //                        ChargingWhenClosed,
                //                        Images?.          Distinct(),
                //                        EnergyMix,

                //                        LastUpdated);

                Tariff = null;

                if (CustomTariffParser != null)
                    Tariff = CustomTariffParser(JSON,
                                                Tariff);

                return true;

            }
            catch (Exception e)
            {
                Tariff         = default;
                ErrorResponse  = "The given JSON representation of a tariff is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out Tariff, out ErrorResponse, TariffIdURL = null, CustomTariffParser = null)

        /// <summary>
        /// Try to parse the given text representation of a tariff.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="Tariff">The parsed tariff.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="TariffIdURL">An optional tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTariffParser">A delegate to parse custom tariff JSON objects.</param>
        public static Boolean TryParse(String                               Text,
                                       out Tariff                           Tariff,
                                       out String                           ErrorResponse,
                                       CountryCode?                         CountryCodeURL       = null,
                                       Party_Id?                            PartyIdURL           = null,
                                       Tariff_Id?                           TariffIdURL          = null,
                                       CustomJObjectParserDelegate<Tariff>  CustomTariffParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out Tariff,
                                out ErrorResponse,
                                CountryCodeURL,
                                PartyIdURL,
                                TariffIdURL,
                                CustomTariffParser);

            }
            catch (Exception e)
            {
                Tariff      = null;
                ErrorResponse  = "The given text representation of a tariff is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffSerializer = null, CustomEVSESerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>

        public JObject ToJSON(CustomJObjectSerializerDelegate<Tariff> CustomTariffSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("country_code",                    CountryCode.ToString()),
                           new JProperty("party_id",                        PartyId.    ToString()),
                           new JProperty("id",                              Id.         ToString()),






                           new JProperty("last_updated",                    LastUpdated.ToIso8601())

                       );

            return CustomTariffSerializer != null
                       ? CustomTariffSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Tariff Tariff1,
                                           Tariff Tariff2)
        {

            if (Object.ReferenceEquals(Tariff1, Tariff2))
                return true;

            if (Tariff1 is null || Tariff2 is null)
                return false;

            return Tariff1.Equals(Tariff2);

        }

        #endregion

        #region Operator != (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Tariff Tariff1,
                                           Tariff Tariff2)

            => !(Tariff1 == Tariff2);

        #endregion

        #region Operator <  (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Tariff Tariff1,
                                          Tariff Tariff2)

            => Tariff1 is null
                   ? throw new ArgumentNullException(nameof(Tariff1), "The given tariff must not be null!")
                   : Tariff1.CompareTo(Tariff2) < 0;

        #endregion

        #region Operator <= (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Tariff Tariff1,
                                           Tariff Tariff2)

            => !(Tariff1 > Tariff2);

        #endregion

        #region Operator >  (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Tariff Tariff1,
                                          Tariff Tariff2)

            => Tariff1 is null
                   ? throw new ArgumentNullException(nameof(Tariff1), "The given tariff must not be null!")
                   : Tariff1.CompareTo(Tariff2) > 0;

        #endregion

        #region Operator >= (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Tariff Tariff1,
                                           Tariff Tariff2)

            => !(Tariff1 < Tariff2);

        #endregion

        #endregion

        #region IComparable<Tariff> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Tariff tariff
                   ? CompareTo(tariff)
                   : throw new ArgumentException("The given object is not a charging tariff!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Tariff)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff">An Tariff to compare with.</param>
        public Int32 CompareTo(Tariff Tariff)

            => Tariff is null
                   ? throw new ArgumentNullException(nameof(Tariff), "The given charging tariff must not be null!")
                   : Id.CompareTo(Tariff.Id);

        #endregion

        #endregion

        #region IEquatable<Tariff> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Tariff tariff &&
                   Equals(tariff);

        #endregion

        #region Equals(Tariff)

        /// <summary>
        /// Compares two Tariffs for equality.
        /// </summary>
        /// <param name="Tariff">An Tariff to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Tariff Tariff)

            => !(Tariff is null) &&
                   Id.Equals(Tariff.Id);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Id.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()

            => Id.ToString();

        #endregion

    }

}
