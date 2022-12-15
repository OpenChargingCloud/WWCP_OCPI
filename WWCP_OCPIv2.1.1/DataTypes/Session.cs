/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// A charging session.
    /// </summary>
    public class Session : IHasId<Session_Id>,
                           IEquatable<Session>,
                           IComparable<Session>,
                           IComparable
    {

        #region Data

        private readonly Object patchLock = new Object();

        #endregion

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
        /// An optional energy meter, e.g. for the German calibration law.
        /// </summary>
        [Optional, NonStandard]
        public EnergyMeter                         EnergyMeter                  { get; }

        /// <summary>
        /// An enumeration of transparency softwares which can be used to validate the charging session data.
        /// </summary>
        [Optional, NonStandard]
        public IEnumerable<TransparencySoftware>   TransparencySoftwares        { get; }

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

        /// <summary>
        /// The SHA256 hash of the JSON representation of this charging session.
        /// </summary>
        public String                              SHA256Hash                   { get; private set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging session.
        /// </summary>
        public Session(CountryCode                        CountryCode,
                       Party_Id                           PartyId,
                       Session_Id                         Id,
                       DateTime                           Start,
                       Decimal                            kWh,
                       CDRToken                           CDRToken,
                       AuthMethods                        AuthMethod,
                       Location_Id                        LocationId,
                       EVSE_UId                           EVSEUId,
                       Connector_Id                       ConnectorId,
                       Currency                           Currency,
                       SessionStatusTypes                 Status,

                       DateTime?                          End                      = null,
                       AuthorizationReference?            AuthorizationReference   = null,
                       Meter_Id?                          MeterId                  = null,
                       EnergyMeter                        EnergyMeter              = null,
                       IEnumerable<TransparencySoftware>  TransparencySoftwares    = null,
                       IEnumerable<ChargingPeriod>        ChargingPeriods          = null,
                       Price?                             TotalCosts               = null,

                       DateTime?                          LastUpdated              = null)

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
            this.EnergyMeter              = EnergyMeter;
            this.TransparencySoftwares    = TransparencySoftwares;
            this.ChargingPeriods          = ChargingPeriods;
            this.TotalCosts               = TotalCosts;

            this.LastUpdated              = LastUpdated ?? DateTime.Now;

            CalcSHA256Hash();

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

                if (JSON.ParseOptional("country_code",
                                       "country code",
                                       CountryCode.TryParse,
                                       out CountryCode? CountryCodeBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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

                if (JSON.ParseOptional("party_id",
                                       "party identification",
                                       Party_Id.TryParse,
                                       out Party_Id? PartyIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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

                if (JSON.ParseOptional("id",
                                       "session identification",
                                       Session_Id.TryParse,
                                       out Session_Id? SessionIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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
                    if (ErrorResponse is not null)
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

                if (!JSON.ParseMandatoryJSON("cdr_token",
                                             "charge detail record token",
                                             OCPIv2_1_1.CDRToken.TryParse,
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

                if (JSON.ParseOptional("authorization_reference",
                                       "authorization reference",
                                       OCPIv2_1_1.AuthorizationReference.TryParse,
                                       out AuthorizationReference? AuthorizationReference,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EnergyMeter               [optional]

                if (JSON.ParseOptionalJSON("energy_meter",
                                           "energy meter",
                                           OCPIv2_1_1.EnergyMeter.TryParse,
                                           out EnergyMeter EnergyMeter,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TransparencySoftwares     [optional]

                if (JSON.ParseOptionalJSON("transparency_softwares",
                                           "transparency softwares",
                                           TransparencySoftware.TryParse,
                                           out IEnumerable<TransparencySoftware> TransparencySoftwares,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Currency                  [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         OCPIv2_1_1.Currency.TryParse,
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
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalCosts                [optional]

                if (JSON.ParseOptionalJSON("total_cost",
                                           "total costs",
                                           Price.TryParse,
                                           out Price? TotalCosts,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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
                                      EnergyMeter,
                                      TransparencySoftwares,
                                      ChargingPeriods,
                                      TotalCosts,

                                      LastUpdated);


                if (CustomSessionParser is not null)
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
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Session>               CustomSessionSerializer                = null,
                              CustomJObjectSerializerDelegate<CDRToken>              CustomCDRTokenSerializer               = null,
                              CustomJObjectSerializerDelegate<EnergyMeter>           CustomEnergyMeterSerializer            = null,
                              CustomJObjectSerializerDelegate<TransparencySoftware>  CustomTransparencySoftwareSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingPeriod>        CustomChargingPeriodSerializer         = null,
                              CustomJObjectSerializerDelegate<CDRDimension>          CustomCDRDimensionSerializer           = null,
                              CustomJObjectSerializerDelegate<Price>                 CustomPriceSerializer                  = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("country_code",                   CountryCode.           ToString()),
                           new JProperty("party_id",                       PartyId.               ToString()),
                           new JProperty("id",                             Id.                    ToString()),

                           new JProperty("start_date_time",                Start.                 ToIso8601()),

                           End.HasValue
                               ? new JProperty("end_date_time",            End.             Value.ToIso8601())
                               : null,

                           new JProperty("kwh",                            kWh),

                           new JProperty("cdr_token",                      CDRToken.ToJSON(CustomCDRTokenSerializer)),
                           new JProperty("auth_method",                    AuthMethod.            ToString()),

                           AuthorizationReference.HasValue
                               ? new JProperty("authorization_reference",  AuthorizationReference.ToString())
                               : null,

                           new JProperty("location_id",                    LocationId.            ToString()),
                           new JProperty("evse_uid",                       EVSEUId.               ToString()),
                           new JProperty("connector_id",                   ConnectorId.           ToString()),

                           MeterId.HasValue
                               ? new JProperty("meter_id",                 MeterId.               ToString())
                               : null,

                           EnergyMeter != null
                               ? new JProperty("energy_meter",             EnergyMeter.           ToJSON(CustomEnergyMeterSerializer))
                               : null,

                           TransparencySoftwares.SafeAny()
                               ? new JProperty("transparency_softwares",   new JArray(TransparencySoftwares.Select(software       => software.      ToJSON(CustomTransparencySoftwareSerializer))))
                               : null,

                           new JProperty("currency",                       Currency.              ToString()),

                           ChargingPeriods.SafeAny()
                               ? new JProperty("charging_periods",         new JArray(ChargingPeriods.      Select(chargingPeriod => chargingPeriod.ToJSON(CustomChargingPeriodSerializer,
                                                                                                                                                           CustomCDRDimensionSerializer))))
                               : null,

                           TotalCosts.HasValue
                               ? new JProperty("total_cost",               TotalCosts.      Value.ToJSON(CustomPriceSerializer))
                               : null,

                           new JProperty("status",                         Status.                ToString()),


                           new JProperty("last_updated",                   LastUpdated.           ToIso8601())

                       );

            return CustomSessionSerializer is not null
                       ? CustomSessionSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject  JSON,
                                                     JObject  Patch)
        {

            foreach (var property in Patch)
            {

                if      (property.Key == "country_code")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'country code' of a charging session is not allowed!");

                else if (property.Key == "party_id")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'party identification' of a charging session is not allowed!");

                else if (property.Key == "id")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'identification' of a charging session is not allowed!");

                else if (property.Value is null)
                    JSON.Remove(property.Key);

                else if (property.Value is JObject subObject)
                {

                    if (JSON.ContainsKey(property.Key))
                    {

                        if (JSON[property.Key] is JObject oldSubObject)
                        {

                            //ToDo: Perhaps use a more generic JSON patch here!
                            // PatchObject.Apply(ToJSON(), EVSEPatch),
                            var patchResult = TryPrivatePatch(oldSubObject, subObject);

                            if (patchResult.IsSuccess)
                                JSON[property.Key] = patchResult.PatchedData;

                        }

                        else
                            JSON[property.Key] = subObject;

                    }

                    else
                        JSON.Add(property.Key, subObject);

                }

                //else if (property.Value is JArray subArray)
                //{
                //}

                else
                    JSON[property.Key] = property.Value;

            }

            return PatchResult<JObject>.Success(JSON);

        }

        #endregion

        #region TryPatch(SessionPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representaion of this charging session.
        /// </summary>
        /// <param name="SessionPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<Session> TryPatch(JObject  SessionPatch,
                                             Boolean  AllowDowngrades = false)
        {

            if (SessionPatch == null)
                return PatchResult<Session>.Failed(this,
                                                   "The given charging session patch must not be null!");

            lock (patchLock)
            {

                if (SessionPatch["last_updated"] is null)
                    SessionPatch["last_updated"] = Timestamp.Now.ToIso8601();

                else if (AllowDowngrades == false &&
                        SessionPatch["last_updated"].Type == JTokenType.Date &&
                       (SessionPatch["last_updated"].Value<DateTime>().ToIso8601().CompareTo(LastUpdated.ToIso8601()) < 1))
                {
                    return PatchResult<Session>.Failed(this,
                                                       "The 'lastUpdated' timestamp of the charging session patch must be newer then the timestamp of the existing charging session!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), SessionPatch);


                if (patchResult.IsFailed)
                    return PatchResult<Session>.Failed(this,
                                                       patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out Session  PatchedSession,
                             out String   ErrorResponse))
                {

                    return PatchResult<Session>.Success(PatchedSession,
                                                        ErrorResponse);

                }

                else
                    return PatchResult<Session>.Failed(this,
                                                       "Invalid JSON merge patch of a charging session: " + ErrorResponse);

            }

        }

        #endregion


        #region CalcSHA256Hash(CustomSessionSerializer = null, CustomCDRTokenSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this charging session in HEX.
        /// </summary>
        /// <param name="CustomSessionSerializer">A delegate to serialize custom session JSON objects.</param>
        /// <param name="CustomCDRTokenSerializer">A delegate to serialize custom charge detail record token JSON objects.</param>
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        public String CalcSHA256Hash(CustomJObjectSerializerDelegate<Session>               CustomSessionSerializer                = null,
                                     CustomJObjectSerializerDelegate<CDRToken>              CustomCDRTokenSerializer               = null,
                                     CustomJObjectSerializerDelegate<EnergyMeter>           CustomEnergyMeterSerializer            = null,
                                     CustomJObjectSerializerDelegate<TransparencySoftware>  CustomTransparencySoftwareSerializer   = null,
                                     CustomJObjectSerializerDelegate<ChargingPeriod>        CustomChargingPeriodSerializer         = null,
                                     CustomJObjectSerializerDelegate<CDRDimension>          CustomCDRDimensionSerializer           = null,
                                     CustomJObjectSerializerDelegate<Price>                 CustomPriceSerializer                  = null)
        {

            using (var SHA256 = new SHA256Managed())
            {

                return SHA256Hash = "0x" + SHA256.ComputeHash(Encoding.Unicode.GetBytes(
                                                                  ToJSON(CustomSessionSerializer,
                                                                         CustomCDRTokenSerializer,
                                                                         CustomEnergyMeterSerializer,
                                                                         CustomTransparencySoftwareSerializer,
                                                                         CustomChargingPeriodSerializer,
                                                                         CustomCDRDimensionSerializer,
                                                                         CustomPriceSerializer).
                                                                  ToString(Newtonsoft.Json.Formatting.None)
                                                              )).
                                                  Select(value => String.Format("{0:x2}", value)).
                                                  Aggregate();

            }

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
        /// Get a text representation of this object.
        /// </summary>
        public override String ToString()

            => Id.ToString();

        #endregion

    }

}
