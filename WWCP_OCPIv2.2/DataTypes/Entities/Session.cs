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
using System.Linq;
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
        public Connector_Id                        ConnectorId                  { get; }

        /// <summary>
        /// Optional identification of the kWh energy meter.
        /// </summary>
        [Optional]
        public Meter_Id?                           MeterId                      { get; }

        /// <summary>
        /// The optional public key of the energy meter.
        /// </summary>
        [Optional, NonStandard]
        public PublicKey?                          MeterPublicKey               { get; }

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
        public Price?                              TotalCosts                   { get; }

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
        public Session(CountryCode                  CountryCode,
                       Party_Id                     PartyId,
                       Session_Id                   Id,
                       DateTime                     Start,
                       Decimal                      kWh,
                       CDRToken                     CDRToken,
                       AuthMethods                  AuthMethod,
                       Location_Id                  LocationId,
                       EVSE_UId                     EVSEUId,
                       Connector_Id                 ConnectorId,
                       Currency                     Currency,
                       SessionStatusTypes           Status,

                       DateTime?                    End                      = null,
                       AuthorizationReference?      AuthorizationReference   = null,
                       Meter_Id?                    MeterId                  = null,
                       PublicKey?                   MeterPublicKey           = null,
                       IEnumerable<ChargingPeriod>  ChargingPeriods          = null,
                       Price?                       TotalCosts               = null,

                       DateTime?                    LastUpdated              = null)

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
            this.ConnectorId              = ConnectorId;
            this.Currency                 = Currency;
            this.Status                   = Status;

            this.End                      = End;
            this.AuthorizationReference   = AuthorizationReference;
            this.MeterId                  = MeterId;
            this.MeterPublicKey           = MeterPublicKey;
            this.ChargingPeriods          = ChargingPeriods;
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

                #region Parse CountryCode               [optional]

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

                #region Parse PartyIdURL                [optional]

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

                #region Parse Id                        [optional]

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

                #region Parse Start                     [mandatory]

                if (!JSON.ParseMandatory("start_date_time",
                                         "start timestamp",
                                         out DateTime Start,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse End                       [optional]

                if (JSON.ParseOptional("end_date_time",
                                       "end timestamp",
                                       out DateTime? End,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse KWh                       [mandatory]

                if (!JSON.ParseMandatory("kwh",
                                         "charged kWh",
                                         out Decimal KWh,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CDRToken                  [mandatory]

                if (!JSON.ParseMandatory("cdr_token",
                                         "charge detail record token",
                                         CDRToken.TryParse,
                                         out CDRToken CDRToken,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthMethod                [mandatory]

                if (!JSON.ParseMandatoryEnum("auth_method",
                                             "authentication method",
                                             out AuthMethods AuthMethod,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthorizationReference    [optional]

                if (JSON.ParseOptionalStruct("authorization_reference",
                                             "authorization reference",
                                             OCPIv2_2.AuthorizationReference.TryParse,
                                             out AuthorizationReference? AuthorizationReference,
                                             out ErrorResponse))
                {
                    if (ErrorResponse != null)
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

                #region Parse EVSEUId                   [mandatory]

                if (!JSON.ParseMandatory("evse_uid",
                                         "EVSE identification",
                                         EVSE_UId.TryParse,
                                         out EVSE_UId EVSEUId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ConnectorId               [mandatory]

                if (!JSON.ParseMandatory("connector_id",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MeterId                   [optional]

                if (JSON.ParseOptional("meter_id",
                                       "meter identification",
                                       Meter_Id.TryParse,
                                       out Meter_Id? MeterId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse MeterPublicKey            [optional]

                if (JSON.ParseOptional("public_key",
                                       "public key",
                                       PublicKey.TryParse,
                                       out PublicKey? MeterPublicKey,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse Currency                  [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         OCPIv2_2.Currency.TryParse,
                                         out Currency Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ChargingPeriods           [optional]

                if (JSON.ParseOptionalJSON("charging_periods",
                                           "charging periods",
                                           ChargingPeriod.TryParse,
                                           out IEnumerable<ChargingPeriod> ChargingPeriods,
                                           out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse TotalCosts                [mandatory]

                if (!JSON.ParseMandatory("total_cost",
                                         "total costs",
                                         OCPIv2_2.Price.TryParse,
                                         out Price TotalCosts,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Status                    [mandatory]

                if (!JSON.ParseMandatoryEnum("status",
                                             "session status",
                                             out SessionStatusTypes Status,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse LastUpdated               [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Session = new Session(CountryCodeBody ?? CountryCodeURL.Value,
                                      PartyIdBody     ?? PartyIdURL.    Value,
                                      SessionIdBody   ?? SessionIdURL.  Value,
                                      Start,
                                      KWh,
                                      CDRToken,
                                      AuthMethod,
                                      LocationId,
                                      EVSEUId,
                                      ConnectorId,
                                      Currency,
                                      Status,

                                      End,
                                      AuthorizationReference,
                                      MeterId,
                                      MeterPublicKey,
                                      ChargingPeriods,
                                      TotalCosts,

                                      LastUpdated);


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

        #region ToJSON(CustomSessionSerializer = null, CustomCDRTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSessionSerializer">A delegate to serialize custom session JSON objects.</param>
        /// <param name="CustomCDRTokenSerializer">A delegate to serialize custom charge detail record token JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Session>         CustomSessionSerializer          = null,
                              CustomJObjectSerializerDelegate<CDRToken>        CustomCDRTokenSerializer         = null,
                              CustomJObjectSerializerDelegate<ChargingPeriod>  CustomChargingPeriodSerializer   = null,
                              CustomJObjectSerializerDelegate<CDRDimension>    CustomCDRDimensionSerializer     = null,
                              CustomJObjectSerializerDelegate<Price>           CustomPriceSerializer            = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("country_code",             CountryCode.           ToString()),
                           new JProperty("party_id",                 PartyId.               ToString()),
                           new JProperty("id",                       Id.                    ToString()),

                           new JProperty("start_date_time",          Start.                 ToIso8601()),

                           End.HasValue
                               ? new JProperty("end_date_time",      End.             Value.ToIso8601())
                               : null,

                           new JProperty("kwh",                      kWh),

                           new JProperty("cdr_token",                CDRToken.ToJSON(CustomCDRTokenSerializer)),
                           new JProperty("auth_method",              AuthMethod.            ToString()),
                           new JProperty("authorization_reference",  AuthorizationReference.ToString()),
                           new JProperty("location_id",              LocationId.            ToString()),
                           new JProperty("evse_uid",                 EVSEUId.               ToString()),
                           new JProperty("connector_id",             ConnectorId.           ToString()),

                           MeterId.HasValue
                               ? new JProperty("meter_id",           MeterId.               ToString())
                               : null,

                           MeterPublicKey.HasValue
                               ? new JProperty("meter_public_key",   MeterPublicKey.        ToString())
                               : null,

                           new JProperty("currency",                 Currency.              ToString()),

                           ChargingPeriods.SafeAny()
                               ? new JProperty("charging_periods",   new JArray(ChargingPeriods.Select(chargingPeriod => chargingPeriod.ToJSON(CustomChargingPeriodSerializer,
                                                                                                                                               CustomCDRDimensionSerializer))))
                               : null,

                           TotalCosts.HasValue
                               ? new JProperty("total_cost",         TotalCosts.      Value.ToJSON(CustomPriceSerializer))
                               : null,

                           new JProperty("status",                   Status.                ToString()),


                           new JProperty("last_updated",             LastUpdated.           ToIso8601())

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
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given charging session must not be null!");

            var c = Id.CompareTo(Session.Id);

            if (c == 0)
                c = LastUpdated.CompareTo(Session.LastUpdated);

            return c;

        }

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

               Id.         Equals(Session.Id) &&
               LastUpdated.Equals(Session.LastUpdated);

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
