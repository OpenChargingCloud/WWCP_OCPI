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
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// A charging session.
    /// </summary>
    public class Session : IHasId<Session_Id>,
                           IEquatable<Session>,
                           IComparable<Session>,
                           IComparable
    {

        #region Properties

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the CPO that 'owns' this session.
        /// </summary>
        [Optional]
        public CountryCode                         CountryCode                  { get; }

        /// <summary>
        /// The Id of the CPO that 'owns' this session (following the ISO-15118 standard).
        /// </summary>
        [Optional]
        public Party_Id                            PartyId                      { get; }

        /// <summary>
        /// The identification of the session within the CPOs platform (and suboperator platforms). 
        /// </summary>
        [Mandatory]
        public Session_Id                          Id                           { get; }

        /// <summary>
        /// The time when the session became active.
        /// </summary>
        [Mandatory]
        public DateTime                            Start                        { get; }

        /// <summary>
        /// The time when the session is completed.
        /// </summary>
        [Optional]
        public DateTime?                           End                          { get; }

        /// <summary>
        /// How many kWh are charged.
        /// </summary>
        [Mandatory]
        public Decimal                             kWh                          { get; }

        /// <summary>
        /// 
        /// </summary>
        [Mandatory]
        public CDRToken                            CDRToken                     { get; }

        /// <summary>
        /// Method used for authentication.
        /// </summary>
        [Mandatory]
        public AuthMethods                         AuthMethod                   { get; }

        /// <summary>
        /// Reference to the authorization given by the eMSP. When the eMSP provided an
        /// authorization_reference in either: real-time authorization or StartSession,
        /// this field SHALL contain the same value. 
        /// </summary>
        [Optional]
        public AuthorizationReference?             AuthorizationReference       { get; }

        /// <summary>
        /// Identification of the location of this CPO, on which the charging session is/was happening.
        /// </summary>
        [Mandatory]
        public Location_Id                         LocationId                   { get; }

        /// <summary>
        /// The UID of the EVSE of this Location on which the charging session is/was happening.
        /// </summary>
        [Mandatory]
        public EVSE_UId                            EVSEUId                      { get; }

        /// <summary>
        /// Identification of the connector of this location the charging session is/was happening.
        /// </summary>
        [Mandatory]
        public Session_Id                        SessionId                  { get; }

        /// <summary>
        /// Optional identification of the kWh energy meter.
        /// </summary>
        [Optional]
        public Meter_Id                            MeterId                      { get; }

        /// <summary>
        /// ISO 4217 code of the currency used for this session.
        /// </summary>
        [Mandatory]
        public Currency                            Currency                     { get; }

        /// <summary>
        /// An optional enumeration of charging periods that can be used to calculate and verify
        /// the total cost.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingPeriod>         ChargingPeriods              { get; }

        /// <summary>
        /// The total cost (excluding VAT) of the session in the specified currency.
        /// This is the price that the eMSP will have to pay to the CPO.
        /// </summary>
        [Optional]
        public Decimal?                            TotalCosts                   { get; }

        /// <summary>
        /// The total cost (excluding VAT) of the session in the specified currency.
        /// This is the price that the eMSP will have to pay to the CPO.
        /// </summary>
        [Mandatory]
        public SessionStatusTypes                  Status                       { get; }

        /// <summary>
        /// Timestamp when this session was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                            LastUpdated                  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging session.
        /// </summary>
        public Session(CountryCode              CountryCode,
                       Party_Id                 PartyId,
                       Session_Id               Id,
                       DateTime                 Start,
                       Decimal                  kWh,
                       CDRToken                 CDRToken,
                       AuthMethods              AuthMethod,
                       Location_Id              LocationId,
                       EVSE_UId                 EVSEUId,
                       Session_Id             SessionId,
                       Meter_Id                 MeterId,
                       Currency                 Currency,
                       SessionStatusTypes       Status,

                       DateTime?                End                      = null,
                       AuthorizationReference?  AuthorizationReference   = null,
                       Decimal?                 TotalCosts               = null,

                       DateTime?                LastUpdated              = null)

        {

            this.CountryCode              = CountryCode;
            this.PartyId                  = PartyId;
            this.Id                       = Id;
            this.Start                    = Start;
            this.kWh                      = kWh;
            this.CDRToken                 = CDRToken;
            this.AuthMethod               = AuthMethod;
            this.LocationId               = LocationId;
            this.EVSEUId                  = EVSEUId;
            this.SessionId              = SessionId;
            this.MeterId                  = MeterId;
            this.Currency                 = Currency;
            this.Status                   = Status;

            this.End                      = End;
            this.AuthorizationReference   = AuthorizationReference;
            this.TotalCosts               = TotalCosts;

            this.LastUpdated              = LastUpdated ?? DateTime.Now;

        }

        #endregion


        #region (static) Parse   (JSON, SessionIdURL = null, CustomSessionParser = null)

        /// <summary>
        /// Parse the given JSON representation of a session.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SessionIdURL">An optional session identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomSessionParser">A delegate to parse custom session JSON objects.</param>
        public static Session Parse(JObject                               JSON,
                                    CountryCode?                          CountryCodeURL        = null,
                                    Party_Id?                             PartyIdURL            = null,
                                    Session_Id?                           SessionIdURL          = null,
                                    CustomJObjectParserDelegate<Session>  CustomSessionParser   = null)
        {

            if (TryParse(JSON,
                         out Session  session,
                         out String   ErrorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         SessionIdURL,
                         CustomSessionParser))
            {
                return session;
            }

            throw new ArgumentException("The given JSON representation of a session is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, SessionIdURL = null, CustomSessionParser = null)

        /// <summary>
        /// Parse the given text representation of a session.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="SessionIdURL">An optional session identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomSessionParser">A delegate to parse custom session JSON objects.</param>
        public static Session Parse(String                                Text,
                                    CountryCode?                          CountryCodeURL        = null,
                                    Party_Id?                             PartyIdURL            = null,
                                    Session_Id?                           SessionIdURL          = null,
                                    CustomJObjectParserDelegate<Session>  CustomSessionParser   = null)
        {

            if (TryParse(Text,
                         out Session  session,
                         out String   ErrorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         SessionIdURL,
                         CustomSessionParser))
            {
                return session;
            }

            throw new ArgumentException("The given text representation of a session is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out Session, out ErrorResponse, SessionIdURL = null, CustomSessionParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a session.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Session">The parsed session.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject      JSON,
                                       out Session  Session,
                                       out String   ErrorResponse)

            => TryParse(JSON,
                        out Session,
                        out ErrorResponse,
                        null,
                        null,
                        null,
                        null);

        /// <summary>
        /// Try to parse the given JSON representation of a session.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Session">The parsed session.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="SessionIdURL">An optional session identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomSessionParser">A delegate to parse custom session JSON objects.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       out Session                           Session,
                                       out String                            ErrorResponse,
                                       CountryCode?                          CountryCodeURL        = null,
                                       Party_Id?                             PartyIdURL            = null,
                                       Session_Id?                           SessionIdURL          = null,
                                       CustomJObjectParserDelegate<Session>  CustomSessionParser   = null)
        {

            try
            {

                Session = default;

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
                                             "session identification",
                                             Session_Id.TryParse,
                                             out Session_Id? SessionIdBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!SessionIdURL.HasValue && !SessionIdBody.HasValue)
                {
                    ErrorResponse = "The session identification is missing!";
                    return false;
                }

                if (SessionIdURL.HasValue && SessionIdBody.HasValue && SessionIdURL.Value != SessionIdBody.Value)
                {
                    ErrorResponse = "The optional session identification given within the JSON body does not match the one given in the URL!";
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

                #region Parse RelatedSessions      [optional]

                //if (JSON.ParseOptionalJSON("related_sessions",
                //                           "related sessions",
                //                           AdditionalGeoSession.TryParse,
                //                           out IEnumerable<AdditionalGeoSession> RelatedSessions,
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


                //Session = new Session(CountryCodeBody ?? CountryCodeURL.Value,
                //                        PartyIdBody     ?? PartyIdURL.Value,
                //                        SessionIdBody  ?? SessionIdURL.Value,
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
                //                        RelatedSessions?.Distinct(),
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

                Session = null;

                if (CustomSessionParser != null)
                    Session = CustomSessionParser(JSON,
                                                  Session);

                return true;

            }
            catch (Exception e)
            {
                Session        = default;
                ErrorResponse  = "The given JSON representation of a session is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out Session, out ErrorResponse, SessionIdURL = null, CustomSessionParser = null)

        /// <summary>
        /// Try to parse the given text representation of a session.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="Session">The parsed session.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="SessionIdURL">An optional session identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomSessionParser">A delegate to parse custom session JSON objects.</param>
        public static Boolean TryParse(String                                Text,
                                       out Session                           Session,
                                       out String                            ErrorResponse,
                                       CountryCode?                          CountryCodeURL        = null,
                                       Party_Id?                             PartyIdURL            = null,
                                       Session_Id?                           SessionIdURL          = null,
                                       CustomJObjectParserDelegate<Session>  CustomSessionParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out Session,
                                out ErrorResponse,
                                CountryCodeURL,
                                PartyIdURL,
                                SessionIdURL,
                                CustomSessionParser);

            }
            catch (Exception e)
            {
                Session        = null;
                ErrorResponse  = "The given text representation of a session is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSessionSerializer = null, CustomEVSESerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSessionSerializer">A delegate to serialize custom session JSON objects.</param>
        /// <param name="CustomPublishTokenTypeSerializer">A delegate to serialize custom publish token type JSON objects.</param>
        /// <param name="CustomAdditionalGeoSessionSerializer">A delegate to serialize custom additional geo session JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Session>                CustomSessionSerializer                 = null,
                              CustomJObjectSerializerDelegate<PublishTokenType>       CustomPublishTokenTypeSerializer        = null,
                              CustomJObjectSerializerDelegate<AdditionalGeoLocation>  CustomAdditionalGeoSessionSerializer    = null,
                              CustomJObjectSerializerDelegate<EVSE>                   CustomEVSESerializer                    = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>         CustomStatusScheduleSerializer          = null,
                              CustomJObjectSerializerDelegate<Connector>              CustomConnectorSerializer               = null,
                              CustomJObjectSerializerDelegate<DisplayText>            CustomDisplayTextSerializer             = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>        CustomBusinessDetailsSerializer         = null,
                              CustomJObjectSerializerDelegate<Hours>                  CustomHoursSerializer                   = null,
                              CustomJObjectSerializerDelegate<Image>                  CustomImageSerializer                   = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("country_code",                    CountryCode.ToString()),
                           new JProperty("party_id",                        PartyId.    ToString()),
                           new JProperty("id",                              Id.         ToString()),
                           //new JProperty("publish",                         Publish),

                           //Publish == false && PublishAllowedTo.SafeAny()
                           //    ? new JProperty("publish_allowed_to",        new JArray(PublishAllowedTo.Select(publishAllowedTo => publishAllowedTo.ToJSON(CustomPublishTokenTypeSerializer))))
                           //    : null,

                           //Name.IsNotNullOrEmpty()
                           //    ? new JProperty("name",                      Name)
                           //    : null,

                           //new JProperty("address",                         Address),
                           //new JProperty("city",                            City),

                           //PostalCode.IsNotNullOrEmpty()
                           //    ? new JProperty("postal_code",               PostalCode)
                           //    : null,

                           //State.IsNotNullOrEmpty()
                           //    ? new JProperty("state",                     State)
                           //    : null,

                           //new JProperty("country",                         Country),
                           //new JProperty("coordinates",                     new JObject(
                           //                                                     new JProperty("latitude",  Coordinates.Latitude. Value.ToString()),
                           //                                                     new JProperty("longitude", Coordinates.Longitude.Value.ToString())
                           //                                                 )),

                           //RelatedSessions.SafeAny()
                           //    ? new JProperty("related_sessions",         new JArray(RelatedSessions.Select(session => session.ToJSON(CustomAdditionalGeoSessionSerializer))))
                           //    : null,

                           //ParkingType.HasValue
                           //    ? new JProperty("parking_type",              ParkingType.Value.ToString())
                           //    : null,

                           //EVSEs.SafeAny()
                           //    ? new JProperty("evses",                     new JArray(EVSEs.Select(evse => evse.ToJSON(CustomEVSESerializer,
                           //                                                                                             CustomStatusScheduleSerializer,
                           //                                                                                             CustomConnectorSerializer))))
                           //    : null,

                           //Directions.SafeAny()
                           //    ? new JProperty("directions",                new JArray(Directions.Select(evse => evse.ToJSON(CustomDisplayTextSerializer))))
                           //    : null,

                           //Operator != null
                           //    ? new JProperty("operator",                  Operator.   ToJSON(CustomBusinessDetailsSerializer))
                           //    : null,

                           //SubOperator != null
                           //    ? new JProperty("suboperator",               SubOperator.ToJSON(CustomBusinessDetailsSerializer))
                           //    : null,

                           //Owner != null
                           //    ? new JProperty("owner",                     Owner.      ToJSON(CustomBusinessDetailsSerializer))
                           //    : null,

                           //Facilities.SafeAny()
                           //    ? new JProperty("facilities",                new JArray(Facilities.Select(facility => facility.ToString())))
                           //    : null,

                           //new JProperty("time_zone",                       Timezone),

                           //OpeningTimes != null
                           //    ? new JProperty("opening_times",             OpeningTimes.ToJSON(CustomHoursSerializer))
                           //    : null,

                           //ChargingWhenClosed.HasValue
                           //    ? new JProperty("charging_when_closed",      ChargingWhenClosed.Value)
                           //    : null,

                           //Images.SafeAny()
                           //    ? new JProperty("images",                    new JArray(Images.Select(image => image.ToJSON(CustomImageSerializer))))
                           //    : null,

                           //EnergyMix != null
                           //    ? new JProperty("energy_mix",                EnergyMix.ToJSON())
                           //    : null,

                           new JProperty("last_updated",                    LastUpdated.ToIso8601())

                       );

            return CustomSessionSerializer != null
                       ? CustomSessionSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Patch(SessionPatch)

        public Session Patch(JObject SessionPatch)
        {

            return this;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Session1, Session2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Session1">A charging session.</param>
        /// <param name="Session2">Another charging session.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Session Session1,
                                           Session Session2)
        {

            if (Object.ReferenceEquals(Session1, Session2))
                return true;

            if (Session1 is null || Session2 is null)
                return false;

            return Session1.Equals(Session2);

        }

        #endregion

        #region Operator != (Session1, Session2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Session1">A charging session.</param>
        /// <param name="Session2">Another charging session.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Session Session1,
                                           Session Session2)

            => !(Session1 == Session2);

        #endregion

        #region Operator <  (Session1, Session2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Session1">A charging session.</param>
        /// <param name="Session2">Another charging session.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Session Session1,
                                          Session Session2)

            => Session1 is null
                   ? throw new ArgumentNullException(nameof(Session1), "The given charging session must not be null!")
                   : Session1.CompareTo(Session2) < 0;

        #endregion

        #region Operator <= (Session1, Session2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Session1">A charging session.</param>
        /// <param name="Session2">Another charging session.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Session Session1,
                                           Session Session2)

            => !(Session1 > Session2);

        #endregion

        #region Operator >  (Session1, Session2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Session1">A charging session.</param>
        /// <param name="Session2">Another charging session.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Session Session1,
                                          Session Session2)

            => Session1 is null
                   ? throw new ArgumentNullException(nameof(Session1), "The given charging session must not be null!")
                   : Session1.CompareTo(Session2) > 0;

        #endregion

        #region Operator >= (Session1, Session2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Session1">A charging session.</param>
        /// <param name="Session2">Another charging session.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Session Session1,
                                           Session Session2)

            => !(Session1 < Session2);

        #endregion

        #endregion

        #region IComparable<Session> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Session session
                   ? CompareTo(session)
                   : throw new ArgumentException("The given object is not a charging session!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Session)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Session">An Session to compare with.</param>
        public Int32 CompareTo(Session Session)

            => Session is null
                   ? throw new ArgumentNullException(nameof(Session), "The given charging session must not be null!")
                   : Id.CompareTo(Session.Id);

        #endregion

        #endregion

        #region IEquatable<Session> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Session session &&
                   Equals(session);

        #endregion

        #region Equals(Session)

        /// <summary>
        /// Compares two Sessions for equality.
        /// </summary>
        /// <param name="Session">An Session to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Session Session)

            => !(Session is null) &&
                   Id.Equals(Session.Id);

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
