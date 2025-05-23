﻿/*
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

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// A session.
    /// </summary>
    public class Session : IHasId<Session_Id>,
                           IEquatable<Session>,
                           IComparable<Session>,
                           IComparable
    {

        #region Data

        private readonly Lock patchLock = new();

        #endregion

        #region Properties

        /// <summary>
        /// The parent CommonAPI of this charging location.
        /// </summary>
        internal CommonAPI?                          CommonAPI                    { get; set; }

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this session.
        /// </summary>
        [Mandatory]
        public   CountryCode                         CountryCode                  { get; }

        /// <summary>
        /// The identification of the charge point operator that 'owns' this session (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public   Party_Id                            PartyId                      { get; }

        /// <summary>
        /// The identification of the session within the charge point operator's platform (and suboperator platforms).
        /// </summary>
        [Mandatory]
        public   Session_Id                          Id                           { get; }

        /// <summary>
        /// The time when the session became active.
        /// </summary>
        [Mandatory]
        public   DateTime                            Start                        { get; }

        /// <summary>
        /// The time when the session is completed.
        /// </summary>
        [Optional]
        public   DateTime?                           End                          { get; }

#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The amount of kWhs that had been charged.
        /// </summary>
        [Mandatory]
        public   WattHour                            kWh                          { get; }
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// The authentication token used to start this charging session, including all
        /// relevant information to identify the unique token.
        /// </summary>
        [Mandatory]
        public   CDRToken                            CDRToken                     { get; }

        /// <summary>
        /// The method used for authentication.
        /// </summary>
        [Mandatory]
        public   AuthMethod                          AuthMethod                   { get; }

        /// <summary>
        /// The optional reference to the authorization given by the eMSP.
        /// When the eMSP provided an authorization_reference in either: real-time authorization
        /// or StartSession, this field SHALL contain the same value.
        /// </summary>
        [Optional]
        public   AuthorizationReference?             AuthorizationReference       { get; }

        /// <summary>
        /// The identification of the location at which the charging session is/was happening.
        /// </summary>
        [Mandatory]
        public   Location_Id                         LocationId                   { get; }

        /// <summary>
        /// The unique internal identification of the EVSE at which the charging session is/was happening.
        /// </summary>
        [Mandatory]
        public   EVSE_UId                            EVSEUId                      { get; }

        /// <summary>
        /// The unique identification of the connector at which the charging session is/was happening.
        /// </summary>
        [Mandatory]
        public   Connector_Id                        ConnectorId                  { get; }

        /// <summary>
        /// The optional identification of the kWh energy meter.
        /// </summary>
        [Optional]
        public   EnergyMeter_Id?                     EnergyMeterId                { get; }

        /// <summary>
        /// The ISO 4217 code of the currency used for this session.
        /// </summary>
        [Mandatory]
        public   Currency                            Currency                     { get; }

        /// <summary>
        /// The optional enumeration of charging periods that can be used to calculate and verify the total cost.
        /// </summary>
        [Optional]
        public   IEnumerable<ChargingPeriod>         ChargingPeriods              { get; }

        /// <summary>
        /// The total costs of the session in the specified currency. This is the price that the eMSP will
        /// have to pay to the CPO. A total_cost of 0.00 means free of charge. When omitted, i.e. no price
        /// information is given in the Session object, it does not imply the session is/was free of charge.
        /// </summary>
        [Optional]
        public   Price?                              TotalCosts                   { get; }

        /// <summary>
        /// The status of the session.
        /// </summary>
        [Mandatory]
        public   SessionStatusType                   Status                       { get; }

        /// <summary>
        /// The timestamp when this session was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public   DateTime                            Created                      { get; }

        /// <summary>
        /// The timestamp when this session was last updated (or created).
        /// </summary>
        [Mandatory]
        public   DateTime                            LastUpdated                  { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this session used as HTTP ETag.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined)]
        public   String                              ETag                         { get; private set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new session.
        /// </summary>
        /// <param name="CountryCode">An ISO-3166 alpha-2 country code of the charge point operator that 'owns' this session.</param>
        /// <param name="PartyId">An identification of the charge point operator that 'owns' this session (following the ISO-15118 standard).</param>
        /// <param name="Id">An unique identification of the session within the CPOs platform (and suboperator platforms).</param>
        /// <param name="Start">A timestamp when the session became active.</param>
        /// <param name="kWh">An amount of kWhs that had been charged.</param>
        /// <param name="CDRToken">An authentication token used to start this charging session, including all relevant information to identify the unique token.</param>
        /// <param name="AuthMethod">A method used for authentication.</param>
        /// <param name="AuthorizationReference">The optional reference to the authorization given by the eMSP. When the eMSP provided an authorization_reference in either: real-time authorization or StartSession, this field SHALL contain the same value.</param>
        /// <param name="LocationId">The identification of the location at which the charging session is/was happening.</param>
        /// <param name="EVSEUId">An unique internal identification of the EVSE at which the charging session is/was happening.</param>
        /// <param name="ConnectorId">An unique identification of the connector at which the charging session is/was happening.</param>
        /// <param name="Currency">An ISO 4217 code of the currency used for this session.</param>
        /// <param name="Status">A status of the session.</param>
        /// 
        /// <param name="End">An optional timestamp when the session was completed.</param>
        /// <param name="EnergyMeterId">The optional identification of the kWh energy meter.</param>
        /// <param name="ChargingPeriods">An optional enumeration of charging periods that can be used to calculate and verify the total cost.</param>
        /// <param name="TotalCosts">The total costs of the session in the specified currency. This is the price that the eMSP will have to pay to the CPO. A total_cost of 0.00 means free of charge. When omitted, i.e. no price information is given in the Session object, it does not imply the session is/was free of charge.</param>
        /// 
        /// <param name="Created">An optional timestamp when this charging session was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this charging session was last updated (or created).</param>
        /// 
        /// <param name="CustomSessionSerializer">A delegate to serialize custom session JSON objects.</param>
        /// <param name="CustomCDRTokenSerializer">A delegate to serialize custom charge detail record token JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        public Session(CountryCode                                       CountryCode,
                       Party_Id                                          PartyId,
                       Session_Id                                        Id,
                       DateTime                                          Start,
                       WattHour                                          kWh,
                       CDRToken                                          CDRToken,
                       AuthMethod                                        AuthMethod,
                       Location_Id                                       LocationId,
                       EVSE_UId                                          EVSEUId,
                       Connector_Id                                      ConnectorId,
                       Currency                                          Currency,
                       SessionStatusType                                 Status,

                       DateTime?                                         End                              = null,
                       AuthorizationReference?                           AuthorizationReference           = null,
                       EnergyMeter_Id?                                   EnergyMeterId                    = null,
                       IEnumerable<ChargingPeriod>?                      ChargingPeriods                  = null,
                       Price?                                            TotalCosts                       = null,

                       DateTime?                                         Created                          = null,
                       DateTime?                                         LastUpdated                      = null,
                       String?                                           ETag                             = null,

                       CustomJObjectSerializerDelegate<Session>?         CustomSessionSerializer          = null,
                       CustomJObjectSerializerDelegate<CDRToken>?        CustomCDRTokenSerializer         = null,
                       CustomJObjectSerializerDelegate<ChargingPeriod>?  CustomChargingPeriodSerializer   = null,
                       CustomJObjectSerializerDelegate<CDRDimension>?    CustomCDRDimensionSerializer     = null,
                       CustomJObjectSerializerDelegate<Price>?           CustomPriceSerializer            = null)

        {

            this.CountryCode             = CountryCode;
            this.PartyId                 = PartyId;
            this.Id                      = Id;
            this.Start                   = Start;
            this.kWh                     = kWh;
            this.CDRToken                = CDRToken;
            this.AuthMethod              = AuthMethod;
            this.LocationId              = LocationId;
            this.EVSEUId                 = EVSEUId;
            this.ConnectorId             = ConnectorId;
            this.Currency                = Currency;
            this.Status                  = Status;

            this.End                     = End;
            this.AuthorizationReference  = AuthorizationReference;
            this.EnergyMeterId           = EnergyMeterId;
            this.ChargingPeriods         = ChargingPeriods?.Distinct() ?? [];
            this.TotalCosts              = TotalCosts;

            var created                  = Created     ?? LastUpdated ?? Timestamp.Now;
            this.Created                 = created;
            this.LastUpdated             = LastUpdated ?? created;

            this.ETag                    = ETag ?? CalcSHA256Hash(
                                                       CustomSessionSerializer,
                                                       CustomCDRTokenSerializer,
                                                       CustomChargingPeriodSerializer,
                                                       CustomCDRDimensionSerializer,
                                                       CustomPriceSerializer
                                                   );

            unchecked
            {

                hashCode = this.CountryCode.            GetHashCode()       * 67 ^
                           this.PartyId.                GetHashCode()       * 61 ^
                           this.Id.                     GetHashCode()       * 59 ^
                           this.Start.                  GetHashCode()       * 53 ^
                           this.kWh.                    GetHashCode()       * 47 ^
                           this.CDRToken.               GetHashCode()       * 43 ^
                           this.AuthMethod.             GetHashCode()       * 41 ^
                           this.LocationId.             GetHashCode()       * 37 ^
                           this.EVSEUId.                GetHashCode()       * 31 ^
                           this.ConnectorId.            GetHashCode()       * 29 ^
                           this.Currency.               GetHashCode()       * 23 ^
                           this.Status.                 GetHashCode()       * 19 ^
                           this.Created.                GetHashCode()       * 17 ^
                           this.LastUpdated.            GetHashCode()       * 13 ^

                          (this.End?.                   GetHashCode() ?? 0) * 11 ^
                          (this.AuthorizationReference?.GetHashCode() ?? 0) *  7 ^
                          (this.EnergyMeterId?.         GetHashCode() ?? 0) *  5 ^
                           this.ChargingPeriods.        GetHashCode()       *  3 ^
                           this.TotalCosts?.            GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CountryCodeURL= null, PartyIdURL= null, SessionIdURL = null, CustomSessionParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging session.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="SessionIdURL">An optional charging session identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomSessionParser">A delegate to parse custom session JSON objects.</param>
        public static Session Parse(JObject                                JSON,
                                    CountryCode?                           CountryCodeURL        = null,
                                    Party_Id?                              PartyIdURL            = null,
                                    Session_Id?                            SessionIdURL          = null,
                                    CustomJObjectParserDelegate<Session>?  CustomSessionParser   = null)
        {

            if (TryParse(JSON,
                         out var session,
                         out var errorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         SessionIdURL,
                         CustomSessionParser))
            {
                return session;
            }

            throw new ArgumentException("The given JSON representation of a charging session is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Session, out ErrorResponse, SessionIdURL = null, CustomSessionParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging session.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Session">The parsed charging session.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       [NotNullWhen(true)]  out Session?  Session,
                                       [NotNullWhen(false)] out String?   ErrorResponse)

            => TryParse(JSON,
                        out Session,
                        out ErrorResponse,
                        null,
                        null,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging session.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Session">The parsed charging session.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="SessionIdURL">An optional charging session identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomSessionParser">A delegate to parse custom session JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out Session?      Session,
                                       [NotNullWhen(false)] out String?       ErrorResponse,
                                       CountryCode?                           CountryCodeURL        = null,
                                       Party_Id?                              PartyIdURL            = null,
                                       Session_Id?                            SessionIdURL          = null,
                                       CustomJObjectParserDelegate<Session>?  CustomSessionParser   = null)
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
                                       "charging session identification",
                                       Session_Id.TryParse,
                                       out Session_Id? SessionIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!SessionIdURL.HasValue && !SessionIdBody.HasValue)
                {
                    ErrorResponse = "The charging session identification is missing!";
                    return false;
                }

                if (SessionIdURL.HasValue && SessionIdBody.HasValue && SessionIdURL.Value != SessionIdBody.Value)
                {
                    ErrorResponse = "The optional charging session identification given within the JSON body does not match the one given in the URL!";
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
                                         WattHour.TryParseKWh,
                                         out WattHour KWh,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CDRToken                  [mandatory]

                if (!JSON.ParseMandatoryJSON("cdr_token",
                                             "charge detail record token",
                                             CDRToken.TryParse,
                                             out CDRToken cdrToken,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthMethod                [mandatory]

                if (!JSON.ParseMandatory("auth_method",
                                         "authentication method",
                                         AuthMethod.TryParse,
                                         out AuthMethod authMethod,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthorizationReference    [optional]

                if (JSON.ParseOptional("authorization_reference",
                                       "authorization reference",
                                       OCPIv2_3_0.AuthorizationReference.TryParse,
                                       out AuthorizationReference? authorizationReference,
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

                #region Parse EnergyMeterId             [optional]

                if (JSON.ParseOptional("meter_id",
                                       "meter identification",
                                       EnergyMeter_Id.TryParse,
                                       out EnergyMeter_Id? energyMeterId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Currency                  [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         Currency.TryParse,
                                         out Currency currency,
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

                if (!JSON.ParseMandatory("status",
                                         "session status",
                                         SessionStatusType.TryParse,
                                         out SessionStatusType status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse Created                   [optional, VendorExtension]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTime? Created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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


                Session = new Session(

                              CountryCodeBody ?? CountryCodeURL!.Value,
                              PartyIdBody     ?? PartyIdURL!.    Value,
                              SessionIdBody   ?? SessionIdURL!.  Value,
                              Start,
                              KWh,
                              cdrToken,
                              authMethod,
                              LocationId,
                              EVSEUId,
                              ConnectorId,
                              currency,
                              status,

                              End,
                              authorizationReference,
                              energyMeterId,
                              ChargingPeriods,
                              TotalCosts,

                              Created,
                              LastUpdated

                          );


                if (CustomSessionParser is not null)
                    Session = CustomSessionParser(JSON,
                                                  Session);

                return true;

            }
            catch (Exception e)
            {
                Session        = default;
                ErrorResponse  = "The given JSON representation of a charging session is invalid: " + e.Message;
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
        /// <param name="CustomTaxAmountSerializer">A delegate to serialize custom tax amount JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Session>?         CustomSessionSerializer          = null,
                              CustomJObjectSerializerDelegate<CDRToken>?        CustomCDRTokenSerializer         = null,
                              CustomJObjectSerializerDelegate<ChargingPeriod>?  CustomChargingPeriodSerializer   = null,
                              CustomJObjectSerializerDelegate<CDRDimension>?    CustomCDRDimensionSerializer     = null,
                              CustomJObjectSerializerDelegate<Price>?           CustomPriceSerializer            = null,
                              CustomJObjectSerializerDelegate<TaxAmount>?       CustomTaxAmountSerializer        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("country_code",              CountryCode.           ToString()),
                                 new JProperty("party_id",                  PartyId.               ToString()),
                                 new JProperty("id",                        Id.                    ToString()),

                                 new JProperty("start_date_time",           Start.                 ToISO8601()),

                           End.HasValue
                               ? new JProperty("end_date_time",             End.             Value.ToISO8601())
                               : null,

                                 new JProperty("kwh",                       kWh.kWh),

                                 new JProperty("cdr_token",                 CDRToken.              ToJSON(CustomCDRTokenSerializer)),
                                 new JProperty("auth_method",               AuthMethod.            ToString()),

                           AuthorizationReference.HasValue
                               ? new JProperty("authorization_reference",   AuthorizationReference.ToString())
                               : null,

                                 new JProperty("location_id",               LocationId.            ToString()),
                                 new JProperty("evse_uid",                  EVSEUId.               ToString()),
                                 new JProperty("connector_id",              ConnectorId.           ToString()),

                           EnergyMeterId.HasValue
                               ? new JProperty("meter_id",                  EnergyMeterId.         ToString())
                               : null,

                                 new JProperty("currency",                  Currency.              ISOCode),

                           ChargingPeriods.SafeAny()
                               ? new JProperty("charging_periods",          new JArray(ChargingPeriods.Select(chargingPeriod => chargingPeriod.ToJSON(CustomChargingPeriodSerializer,
                                                                                                                                                      CustomCDRDimensionSerializer))))
                               : null,

                           TotalCosts is not null
                               ? new JProperty("total_cost",                TotalCosts.            ToJSON(CustomPriceSerializer,
                                                                                                          CustomTaxAmountSerializer))
                               : null,

                                 new JProperty("status",                    Status.                ToString()),


                                 new JProperty("created",                   Created.               ToISO8601()),
                                 new JProperty("last_updated",              LastUpdated.           ToISO8601())

                       );

            return CustomSessionSerializer is not null
                       ? CustomSessionSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this session.
        /// </summary>
        public Session Clone()

            => new (

                   CountryCode.            Clone(),
                   PartyId.                Clone(),
                   Id.                     Clone(),
                   Start,
                   kWh,
                   CDRToken.               Clone(),
                   AuthMethod,
                   LocationId.             Clone(),
                   EVSEUId.                Clone(),
                   ConnectorId.            Clone(),
                   Currency.               Clone(),
                   Status,

                   End,
                   AuthorizationReference?.Clone(),
                   EnergyMeterId?.         Clone(),
                   ChargingPeriods.        Select(chargingPeriod => chargingPeriod.Clone()),
                   TotalCosts,

                   Created,
                   LastUpdated,
                   ETag.                   CloneString()

               );

        #endregion


        #region Update(AdditionalConsumption = null, NewStatus = null, SessionEnd = null, NewTotalCosts = null, AdditionalChargingPeriods = null)

        /// <summary>
        /// Update the session with the given parameters.
        /// </summary>
        /// <param name="AdditionalConsumption">An optional amount of additional energy consumption.</param>
        /// <param name="NewStatus">An optional new status of the session.</param>
        /// <param name="SessionEnd">An optional new end timestamp of the session.</param>
        /// <param name="NewTotalCosts">An optional new total costs of the session.</param>
        /// <param name="AdditionalChargingPeriods">Optional additional charging periods.</param>
        public Session Update(WattHour?                     AdditionalConsumption       = null,
                              SessionStatusType?            NewStatus                   = null,
                              DateTime?                     SessionEnd                  = null,
                              Price?                        NewTotalCosts               = null,
                              IEnumerable<ChargingPeriod>?  AdditionalChargingPeriods   = null)

            => AdditionalConsumption     is null &&
               NewStatus                 is null &&
               SessionEnd                is null &&
               NewTotalCosts             is null &&
               AdditionalChargingPeriods is null

                   ? this
                   : new (

                         CountryCode:              CountryCode,
                         PartyId:                  PartyId,
                         Id:                       Id,
                         Start:                    Start,
                         kWh:                      AdditionalConsumption.HasValue
                                                       ? kWh + AdditionalConsumption.Value
                                                       : kWh,
                         CDRToken:                 CDRToken,
                         AuthMethod:               AuthMethod,
                         LocationId:               LocationId,
                         EVSEUId:                  EVSEUId,
                         ConnectorId:              ConnectorId,
                         Currency:                 Currency,
                         Status:                   NewStatus ?? Status,

                         End:                      SessionEnd,
                         AuthorizationReference:   AuthorizationReference,
                         EnergyMeterId:            EnergyMeterId,
                         ChargingPeriods:          AdditionalChargingPeriods?.Count() > 0
                                                       ? ChargingPeriods.Concat(AdditionalChargingPeriods)
                                                       : ChargingPeriods,
                         TotalCosts:               NewTotalCosts ?? TotalCosts,

                         Created:                  null,
                         LastUpdated:              null

                     );

        #endregion

        #region Complete(SessionEndTimestamp, AdditionalConsumption = null, NewTotalCosts = null, AdditionalChargingPeriods = null)

        /// <summary>
        /// Update the session with the given parameters and set the status to 'completed'.
        /// </summary>
        /// <param name="SessionEndTimestamp">The timestamp when the session is completed.</param>
        /// <param name="AdditionalConsumption">An optional amount of additional energy consumption.</param>
        /// <param name="NewTotalCosts">An optional new total costs of the session.</param>
        /// <param name="AdditionalChargingPeriods">Optional additional charging periods.</param>
        public Session Complete(DateTime                      SessionEndTimestamp,
                                WattHour?                     AdditionalConsumption       = null,
                                Price?                        NewTotalCosts               = null,
                                IEnumerable<ChargingPeriod>?  AdditionalChargingPeriods   = null)

            => new (

                   CountryCode:              CountryCode,
                   PartyId:                  PartyId,
                   Id:                       Id,
                   Start:                    Start,
                   kWh:                      AdditionalConsumption.HasValue
                                                 ? kWh + AdditionalConsumption.Value
                                                 : kWh,
                   CDRToken:                 CDRToken,
                   AuthMethod:               AuthMethod,
                   LocationId:               LocationId,
                   EVSEUId:                  EVSEUId,
                   ConnectorId:              ConnectorId,
                   Currency:                 Currency,
                   Status:                   SessionStatusType.COMPLETED,

                   End:                      SessionEndTimestamp,
                   AuthorizationReference:   AuthorizationReference,
                   EnergyMeterId:            EnergyMeterId,
                   ChargingPeriods:          AdditionalChargingPeriods?.Count() > 0
                                                 ? ChargingPeriods.Concat(AdditionalChargingPeriods)
                                                 : ChargingPeriods,
                   TotalCosts:               NewTotalCosts ?? TotalCosts,

                   Created:                  null,
                   LastUpdated:              null

               );

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject  JSON,
                                                     JObject  Patch,
                                                     EventTracking_Id  EventTrackingId)
        {

            foreach (var property in Patch)
            {

                if      (property.Key == "country_code")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'country code' of a charging session is not allowed!");

                else if (property.Key == "party_id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'party identification' of a charging session is not allowed!");

                else if (property.Key == "id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
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
                            var patchResult = TryPrivatePatch(oldSubObject, subObject, EventTrackingId);

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

            return PatchResult<JObject>.Success(EventTrackingId, JSON);

        }

        #endregion

        #region TryPatch(SessionPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representation of this charging session.
        /// </summary>
        /// <param name="SessionPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<Session> TryPatch(JObject           SessionPatch,
                                             Boolean           AllowDowngrades   = false,
                                             EventTracking_Id? EventTrackingId   = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (SessionPatch is null)
                return PatchResult<Session>.Failed(EventTrackingId, this,
                                                   "The given charging session patch must not be null!");

            lock (patchLock)
            {

                if (SessionPatch["last_updated"] is null)
                    SessionPatch["last_updated"] = Timestamp.Now.ToISO8601();

                else if (AllowDowngrades == false &&
                        SessionPatch["last_updated"].Type == JTokenType.Date &&
                       (SessionPatch["last_updated"].Value<DateTime>().ToISO8601().CompareTo(LastUpdated.ToISO8601()) < 1))
                {
                    return PatchResult<Session>.Failed(EventTrackingId, this,
                                                       "The 'lastUpdated' timestamp of the charging session patch must be newer then the timestamp of the existing charging session!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), SessionPatch, EventTrackingId);


                if (patchResult.IsFailed)
                    return PatchResult<Session>.Failed(EventTrackingId, this,
                                                       patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out var patchedSession,
                             out var errorResponse))
                {

                    return PatchResult<Session>.Success(EventTrackingId, patchedSession,
                                                        errorResponse);

                }

                else
                    return PatchResult<Session>.Failed(EventTrackingId, this,
                                                       "Invalid JSON merge patch of a charging session: " + errorResponse);

            }

        }

        #endregion


        #region CalcSHA256Hash(CustomSessionSerializer = null, CustomCDRTokenSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this charging session in HEX.
        /// </summary>
        /// <param name="CustomSessionSerializer">A delegate to serialize custom session JSON objects.</param>
        /// <param name="CustomCDRTokenSerializer">A delegate to serialize custom charge detail record token JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        public String CalcSHA256Hash(CustomJObjectSerializerDelegate<Session>?         CustomSessionSerializer          = null,
                                     CustomJObjectSerializerDelegate<CDRToken>?        CustomCDRTokenSerializer         = null,
                                     CustomJObjectSerializerDelegate<ChargingPeriod>?  CustomChargingPeriodSerializer   = null,
                                     CustomJObjectSerializerDelegate<CDRDimension>?    CustomCDRDimensionSerializer     = null,
                                     CustomJObjectSerializerDelegate<Price>?           CustomPriceSerializer            = null)
        {

            this.ETag = SHA256.HashData(ToJSON(CustomSessionSerializer,
                                                           CustomCDRTokenSerializer,
                                                           CustomChargingPeriodSerializer,
                                                           CustomCDRDimensionSerializer,
                                                           CustomPriceSerializer).ToUTF8Bytes()).ToBase64();

            return this.ETag;

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
        public static Boolean operator == (Session? Session1,
                                           Session? Session2)
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
        public static Boolean operator != (Session? Session1,
                                           Session? Session2)

            => !(Session1 == Session2);

        #endregion

        #region Operator <  (Session1, Session2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Session1">A charging session.</param>
        /// <param name="Session2">Another charging session.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Session? Session1,
                                          Session? Session2)

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
        public static Boolean operator <= (Session? Session1,
                                           Session? Session2)

            => !(Session1 > Session2);

        #endregion

        #region Operator >  (Session1, Session2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Session1">A charging session.</param>
        /// <param name="Session2">Another charging session.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Session? Session1,
                                          Session? Session2)

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
        public static Boolean operator >= (Session? Session1,
                                           Session? Session2)

            => !(Session1 < Session2);

        #endregion

        #endregion

        #region IComparable<Session> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging sessions.
        /// </summary>
        /// <param name="Object">A charging session to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Session session
                   ? CompareTo(session)
                   : throw new ArgumentException("The given object is not a charging session!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Session)

        /// <summary>
        /// Compares two charging sessions.
        /// </summary>
        /// <param name="Session">A charging session to compare with.</param>
        public Int32 CompareTo(Session? Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given charging session must not be null!");

            var c = CountryCode.CompareTo(Session.CountryCode);

            if (c == 0)
                c = PartyId.    CompareTo(Session.PartyId);

            if (c == 0)
                c = Id.         CompareTo(Session.Id);

            if (c == 0)
                c = Start.      CompareTo(Session.Start);

            if (c == 0)
                c = LocationId. CompareTo(Session.LocationId);

            if (c == 0)
                c = EVSEUId.    CompareTo(Session.EVSEUId);

            if (c == 0)
                c = ConnectorId.CompareTo(Session.ConnectorId);

            if (c == 0)
                c = kWh.        CompareTo(Session.kWh);

            if (c == 0)
                c = CDRToken.   CompareTo(Session.CDRToken);

            if (c == 0)
                c = AuthMethod. CompareTo(Session.AuthMethod);

            if (c == 0)
                c = Currency.   CompareTo(Session.Currency);

            if (c == 0)
                c = Status.     CompareTo(Session.Status);

            if (c == 0)
                c = Created.    CompareTo(Session.Created);

            if (c == 0)
                c = LastUpdated.CompareTo(Session.LastUpdated);

            // End
            // AuthorizationReference
            // MeterId
            // ChargingPeriods
            // TotalCost

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Session> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging sessions for equality.
        /// </summary>
        /// <param name="Object">A charging session to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Session session &&
                   Equals(session);

        #endregion

        #region Equals(Session)

        /// <summary>
        /// Compares two charging sessions for equality.
        /// </summary>
        /// <param name="Session">A charging session to compare with.</param>
        public Boolean Equals(Session? Session)

            => Session is not null &&

               CountryCode.            Equals(Session.CountryCode)             &&
               PartyId.                Equals(Session.PartyId)                 &&
               Id.                     Equals(Session.Id)                      &&
               Start.                  Equals(Session.Start)                   &&
               kWh.                    Equals(Session.kWh)                     &&
               CDRToken.               Equals(Session.CDRToken)                &&
               AuthMethod.             Equals(Session.AuthMethod)              &&
               LocationId.             Equals(Session.LocationId)              &&
               EVSEUId.                Equals(Session.EVSEUId)                 &&
               ConnectorId.            Equals(Session.ConnectorId)             &&
               Currency.               Equals(Session.Currency)                &&
               Status.                 Equals(Session.Status)                  &&
               Created.    ToISO8601().Equals(Session.Created.    ToISO8601()) &&
               LastUpdated.ToISO8601().Equals(Session.LastUpdated.ToISO8601()) &&

            ((!End.                   HasValue    && !Session.End.                   HasValue) ||
              (End.                   HasValue    &&  Session.End.                   HasValue    && End.                   Value.Equals(Session.End.                   Value))) &&

            ((!AuthorizationReference.HasValue    && !Session.AuthorizationReference.HasValue) ||
              (AuthorizationReference.HasValue    &&  Session.AuthorizationReference.HasValue    && AuthorizationReference.Value.Equals(Session.AuthorizationReference.Value))) &&

            ((!EnergyMeterId.               HasValue    && !Session.EnergyMeterId.               HasValue) ||
              (EnergyMeterId.               HasValue    &&  Session.EnergyMeterId.               HasValue    && EnergyMeterId.               Value.Equals(Session.EnergyMeterId.               Value))) &&

             ((TotalCosts             is null     &&  Session.TotalCosts             is null)  ||
              (TotalCosts             is not null &&  Session.TotalCosts             is not null && TotalCosts.                  Equals(Session.TotalCosts                  ))) &&

               ChargingPeriods.Count().Equals(Session.ChargingPeriods.Count())     &&
               ChargingPeriods.All(chargingPeriod => Session.ChargingPeriods.Contains(chargingPeriod));

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

                   Id,                      " (",
                   CountryCode,             "-",
                   PartyId,                 ", ",
                   Status,                  "), ",

                   Start.ToISO8601(),

                   End.HasValue
                       ? " - " + End.Value.ToISO8601() + ", "
                       : ", ",

                   CDRToken.ToString(), " (",
                   AuthMethod,          "), ",

                   AuthorizationReference.HasValue
                       ? "auth ref: " + AuthorizationReference.Value.ToString() + ", "
                       : "",

                   "location: ",  LocationId. ToString(),  ", ",
                   "evse: ",      EVSEUId.    ToString(),  ", ",
                   "connector: ", ConnectorId.ToString(),  ", ",

                   kWh,                     " kWh, ",

                   TotalCosts is not null
                       ? TotalCosts.ToString() + " " + Currency.ToString() + ", "
                       : "",

                   ChargingPeriods.Any()
                       ? ChargingPeriods.Count() + " charging period(s), "
                       : "",

                   EnergyMeterId.HasValue
                       ? "meter: " + EnergyMeterId.Value.ToString() + ", "
                       : "",

                   "last updated: " + LastUpdated.ToISO8601()

               );

        #endregion


    }

}
