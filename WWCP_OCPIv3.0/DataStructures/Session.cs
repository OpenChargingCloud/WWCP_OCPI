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

using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv3_0;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A charging session.
    /// </summary>
    public class Session : APartyIssuedObject<Session_Id>,
                           IEquatable<Session>,
                           IComparable<Session>,
                           IComparable
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of locations.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/session");

        #endregion

        #region Properties

        /// <summary>
        /// The timestamp when the session became ACTIVE in the charging station.
        /// When the session is still PENDING, this field SHALL be set to the time the session was created at the charging station.
        /// When a session goes from PENDING to ACTIVE, this field SHALL be updated to the moment the session went to ACTIVE in the charging station.
        /// </summary>
        [Mandatory]
        public   DateTimeOffset                           Start                        { get; }

        /// <summary>
        /// The timestamp when the session was completed/finished, charging might have finished before the session ends,
        /// for example: EV is full, but parking cost also has to be paid.
        /// </summary>
        [Optional]
        public   DateTimeOffset?                          End                          { get; }

        /// <summary>
        /// How many kWh of energy were transferred through the EVSE into the vehicle.
        /// </summary>
        [Mandatory]
        public   WattHour                                 Energy                       { get; }

        /// <summary>
        /// Token used to start this charging session, including all the relevant information to identify the unique token.
        /// </summary>
        [Mandatory]
        public   CDRToken                                 CDRToken                     { get; }

        /// <summary>
        /// Method used for authorization. This might change during a session.This can happen for example when the session was
        /// started with a reservation according to use case Reserve an EVSE at a location.
        /// Initially the authorization method will be COMMAND which changes to WHITELIST when the driver arrives and starts
        /// charging using a Token that is whitelisted.
        /// </summary>
        [Mandatory]
        public   AuthMethod                               AuthMethod                   { get; }

        /// <summary>
        /// Reference to the authorization given by the eMSP. When the eMSP provided an authorization_reference
        /// in either: realtime authorization, Start a Session or Reserve an EVSE at a Location this field SHALL
        /// contain the same value. When different authorization_reference values have been given by the eMSP that
        /// are relevant to this Session, the last given value SHALL be used here.
        /// </summary>
        [Optional]
        public   AuthorizationReference?                  AuthorizationReference       { get; }

        /// <summary>
        /// The identification of the location of this CPO, on which the charging session is/was happening.
        /// </summary>
        [Mandatory]
        public   Location_Id                              LocationId                   { get; }

        /// <summary>
        /// The Connector that the Session happened on. This is allowed to be unset if and only if the session
        /// is created for a reservation for which no EVSE has been assigned yet.
        /// </summary>
        [Optional]
        public   SessionConnector?                        Connector                    { get; }

        /// <summary>
        /// The ISO 4217 code of the currency used for this session.
        /// </summary>
        [Mandatory]
        public   Currency                                 Currency                     { get; }

        /// <summary>
        /// The optional enumeration of charging periods that can be used to calculate and verify the total cost.
        /// </summary>
        [Optional]
        public   IEnumerable<ChargingPeriod>              ChargingPeriods              { get; }

        /// <summary>
        /// The ID of the Tariff Association that was used to look up the Tariff of this Session.
        /// When the session is free, the ID of a Tariff Association for a Free of Charge tariff is to be given in this field.
        /// </summary>
        [Mandatory]
        public   TariffAssociation_Id                     TariffAssociationId          { get; }

        /// <summary>
        /// The ID of the Tariff that was used to compute what this Session costs.
        /// When the session is free, the ID of a Free of Charge tariff is to be given in this field.
        /// </summary>
        [Mandatory]
        public   Tariff_Id                                TariffId                     { get; }

        /// <summary>
        /// The total costs of the session in the specified currency. This is the price that the eMSP will
        /// have to pay to the CPO. A total_cost of 0.00 means free of charge. When omitted, i.e. no price
        /// information is given in the Session object, it does not imply the session is/was free of charge.
        /// </summary>
        [Optional]
        public   Price?                                   TotalCosts                   { get; }

        /// <summary>
        /// The status of the session.
        /// </summary>
        [Mandatory]
        public   SessionStatus                            Status                       { get; }

        /// <summary>
        /// The optional identification of the energy meter.
        /// </summary>
        [Optional]
        public   EnergyMeter_Id?                          EnergyMeterId                { get; }

        /// <summary>
        /// The optional energy meter used for this session.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined, VE.Eichrecht)]
        public   EnergyMeter<EVSE>?                       EnergyMeter                  { get; }

        /// <summary>
        /// The enumeration of valid transparency softwares which can be used to validate
        /// the singed charging session and metering data.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined, VE.Eichrecht)]
        public   IEnumerable<TransparencySoftwareStatus>  TransparencySoftwares        { get; }

        /// <summary>
        /// The timestamp when this session was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public   DateTimeOffset                           Created                      { get; }

        /// <summary>
        /// The timestamp when this session was last updated (or created).
        /// </summary>
        [Mandatory]
        public   DateTimeOffset                           LastUpdated                  { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this charging session.
        /// </summary>
        public   String                                   ETag                         { get; private set; }

        #endregion

        #region Constructor(s)

        #region Session(...)

        /// <summary>
        /// Create a new session.
        /// </summary>
        /// <param name="PartyId">The party identification of the party that issued this session.</param>
        /// <param name="Id">An identification of the session within the party.</param>
        /// <param name="VersionId">The version identification of the session.</param>
        /// 
        /// <param name="Start">A timestamp when the session became active.</param>
        /// <param name="Energy">An amount of kWhs that had been charged.</param>
        /// <param name="CDRToken">An authentication token used to start this charging session, including all relevant information to identify the unique token.</param>
        /// <param name="AuthMethod">A method used for authentication.</param>
        /// <param name="LocationId">The identification of the location at which the charging session is/was happening.</param>
        /// <param name="Currency">An ISO 4217 code of the currency used for this session.</param>
        /// <param name="TariffAssociationId">The identification of the tariff association that was used to look up the tariff of this session.</param>
        /// <param name="TariffId">The identification of the tariff that was used to compute what this session costs.</param>
        /// <param name="Status">A status of the session.</param>
        /// 
        /// <param name="End">An optional timestamp when the session was completed.</param>
        /// <param name="AuthorizationReference">The optional reference to the authorization given by the eMSP. When the eMSP provided an authorization_reference in either: real-time authorization or StartSession, this field SHALL contain the same value.</param>
        /// <param name="Connector">The optional connector that the session happened on.</param>
        /// <param name="ChargingPeriods">An optional enumeration of charging periods that can be used to calculate and verify the total cost.</param>
        /// <param name="TotalCosts">The total costs of the session in the specified currency. This is the price that the eMSP will have to pay to the CPO. A total_cost of 0.00 means free of charge. When omitted, i.e. no price information is given in the Session object, it does not imply the session is/was free of charge.</param>
        /// <param name="EnergyMeterId">The optional identification of the kWh energy meter.</param>
        /// <param name="EnergyMeter">The optional energy meter used for this session.</param>
        /// <param name="TransparencySoftwares">An optional enumeration of valid transparency softwares which can be used to validate the signed charging session and metering data.</param>
        /// 
        /// <param name="Created">An optional timestamp when this charging session was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this charging session was last updated (or created).</param>
        /// 
        /// <param name="CustomSessionSerializer">A delegate to serialize custom session JSON objects.</param>
        /// <param name="CustomCDRTokenSerializer">A delegate to serialize custom charge detail record token JSON objects.</param>
        /// <param name="CustomSessionConnectorSerializer">A delegate to serialize custom session connector JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        public Session(Party_Idv3                                          PartyId,
                       Session_Id                                          Id,
                       UInt64                                              VersionId,

                       DateTimeOffset                                      Start,
                       WattHour                                            Energy,
                       CDRToken                                            CDRToken,
                       AuthMethod                                          AuthMethod,
                       Location_Id                                         LocationId,
                       Currency                                            Currency,
                       TariffAssociation_Id                                TariffAssociationId,
                       Tariff_Id                                           TariffId,
                       SessionStatus                                       Status,

                       DateTimeOffset?                                     End                                = null,
                       AuthorizationReference?                             AuthorizationReference             = null,
                       SessionConnector?                                   Connector                          = null,
                       IEnumerable<ChargingPeriod>?                        ChargingPeriods                    = null,
                       Price?                                              TotalCosts                         = null,
                       EnergyMeter_Id?                                     EnergyMeterId                      = null,
                       EnergyMeter<EVSE>?                                  EnergyMeter                        = null,
                       IEnumerable<TransparencySoftwareStatus>?            TransparencySoftwares              = null,

                       DateTimeOffset?                                     Created                            = null,
                       DateTimeOffset?                                     LastUpdated                        = null,

                       CustomJObjectSerializerDelegate<Session>?           CustomSessionSerializer            = null,
                       CustomJObjectSerializerDelegate<CDRToken>?          CustomCDRTokenSerializer           = null,
                       CustomJObjectSerializerDelegate<SessionConnector>?  CustomSessionConnectorSerializer   = null,
                       CustomJObjectSerializerDelegate<ChargingPeriod>?    CustomChargingPeriodSerializer     = null,
                       CustomJObjectSerializerDelegate<CDRDimension>?      CustomCDRDimensionSerializer       = null,
                       CustomJObjectSerializerDelegate<Price>?             CustomPriceSerializer              = null)

            : this(null,
                   PartyId,
                   Id,
                   VersionId,

                   Start,
                   Energy,
                   CDRToken,
                   AuthMethod,
                   LocationId,
                   Currency,
                   TariffAssociationId,
                   TariffId,
                   Status,

                   End,
                   AuthorizationReference,
                   Connector,
                   ChargingPeriods,
                   TotalCosts,
                   EnergyMeterId,
                   EnergyMeter,
                   TransparencySoftwares,

                   Created,
                   LastUpdated,

                   CustomSessionSerializer,
                   CustomCDRTokenSerializer,
                   CustomSessionConnectorSerializer,
                   CustomChargingPeriodSerializer,
                   CustomCDRDimensionSerializer,
                   CustomPriceSerializer)

        { }

        #endregion

        #region (internal) Session(CommonAPI, ...)

        /// <summary>
        /// Create a new session.
        /// </summary>
        /// <param name="CommonAPI">The OCPI Common API hosting this session.</param>
        /// <param name="PartyId">The party identification of the party that issued this session.</param>
        /// <param name="Id">An identification of the session within the party.</param>
        /// <param name="VersionId">The version identification of the session.</param>
        /// 
        /// <param name="Start">A timestamp when the session became active.</param>
        /// <param name="Energy">An amount of kWhs that had been charged.</param>
        /// <param name="CDRToken">An authentication token used to start this charging session, including all relevant information to identify the unique token.</param>
        /// <param name="AuthMethod">A method used for authentication.</param>
        /// <param name="LocationId">The identification of the location at which the charging session is/was happening.</param>
        /// <param name="Currency">An ISO 4217 code of the currency used for this session.</param>
        /// <param name="TariffAssociationId">The identification of the tariff association that was used to look up the tariff of this session.</param>
        /// <param name="TariffId">The identification of the tariff that was used to compute what this session costs.</param>
        /// <param name="Status">A status of the session.</param>
        /// 
        /// <param name="End">An optional timestamp when the session was completed.</param>
        /// <param name="AuthorizationReference">The optional reference to the authorization given by the eMSP. When the eMSP provided an authorization_reference in either: real-time authorization or StartSession, this field SHALL contain the same value.</param>
        /// <param name="Connector">The optional connector that the session happened on.</param>
        /// <param name="ChargingPeriods">An optional enumeration of charging periods that can be used to calculate and verify the total cost.</param>
        /// <param name="TotalCosts">The total costs of the session in the specified currency. This is the price that the eMSP will have to pay to the CPO. A total_cost of 0.00 means free of charge. When omitted, i.e. no price information is given in the Session object, it does not imply the session is/was free of charge.</param>
        /// <param name="EnergyMeterId">The optional identification of the kWh energy meter.</param>
        /// <param name="EnergyMeter">The optional energy meter used for this session.</param>
        /// <param name="TransparencySoftwares">An optional enumeration of valid transparency softwares which can be used to validate the signed charging session and metering data.</param>
        /// 
        /// <param name="Created">An optional timestamp when this charging session was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this charging session was last updated (or created).</param>
        /// 
        /// <param name="CustomSessionSerializer">A delegate to serialize custom session JSON objects.</param>
        /// <param name="CustomCDRTokenSerializer">A delegate to serialize custom charge detail record token JSON objects.</param>
        /// <param name="CustomSessionConnectorSerializer">A delegate to serialize custom session connector JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        internal Session(CommonAPI?                                          CommonAPI,
                         Party_Idv3                                          PartyId,
                         Session_Id                                          Id,
                         UInt64                                              VersionId,

                         DateTimeOffset                                      Start,
                         WattHour                                            Energy,
                         CDRToken                                            CDRToken,
                         AuthMethod                                          AuthMethod,
                         Location_Id                                         LocationId,
                         Currency                                            Currency,
                         TariffAssociation_Id                                TariffAssociationId,
                         Tariff_Id                                           TariffId,
                         SessionStatus                                       Status,

                         DateTimeOffset?                                     End                                = null,
                         AuthorizationReference?                             AuthorizationReference             = null,
                         SessionConnector?                                   Connector                          = null,
                         IEnumerable<ChargingPeriod>?                        ChargingPeriods                    = null,
                         Price?                                              TotalCosts                         = null,
                         EnergyMeter_Id?                                     EnergyMeterId                      = null,
                         EnergyMeter<EVSE>?                                  EnergyMeter                        = null,
                         IEnumerable<TransparencySoftwareStatus>?            TransparencySoftwares              = null,

                         DateTimeOffset?                                     Created                            = null,
                         DateTimeOffset?                                     LastUpdated                        = null,

                         CustomJObjectSerializerDelegate<Session>?           CustomSessionSerializer            = null,
                         CustomJObjectSerializerDelegate<CDRToken>?          CustomCDRTokenSerializer           = null,
                         CustomJObjectSerializerDelegate<SessionConnector>?  CustomSessionConnectorSerializer   = null,
                         CustomJObjectSerializerDelegate<ChargingPeriod>?    CustomChargingPeriodSerializer     = null,
                         CustomJObjectSerializerDelegate<CDRDimension>?      CustomCDRDimensionSerializer       = null,
                         CustomJObjectSerializerDelegate<Price>?             CustomPriceSerializer              = null)

            : base(CommonAPI,
                   PartyId,
                   Id,
                   VersionId)

        {

            this.Start                   = Start;
            this.Energy                  = Energy;
            this.CDRToken                = CDRToken;
            this.AuthMethod              = AuthMethod;
            this.LocationId              = LocationId;
            this.Currency                = Currency;
            this.TariffAssociationId     = TariffAssociationId;
            this.TariffId                = TariffId;
            this.Status                  = Status;

            this.End                     = End;
            this.AuthorizationReference  = AuthorizationReference;
            this.Connector               = Connector;
            this.ChargingPeriods         = ChargingPeriods?.      Distinct() ?? [];
            this.TotalCosts              = TotalCosts;
            this.EnergyMeterId           = EnergyMeterId;
            this.EnergyMeter             = EnergyMeter;
            this.TransparencySoftwares   = TransparencySoftwares?.Distinct() ?? [];

            this.Created                 = Created                           ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated             = LastUpdated                       ?? Created     ?? Timestamp.Now;

            this.ETag                    = CalcSHA256Hash(CustomSessionSerializer,
                                                          CustomCDRTokenSerializer,
                                                          CustomSessionConnectorSerializer,
                                                          CustomChargingPeriodSerializer,
                                                          CustomCDRDimensionSerializer,
                                                          CustomPriceSerializer);

            unchecked
            {

                hashCode = this.PartyId.                GetHashCode()       * 79 ^
                           this.Id.                     GetHashCode()       * 73 ^
                           this.VersionId.              GetHashCode()       * 71 ^

                           this.Start.                  GetHashCode()       * 67 ^
                           this.Energy.                 GetHashCode()       * 61 ^
                           this.CDRToken.               GetHashCode()       * 59 ^
                           this.AuthMethod.             GetHashCode()       * 53 ^
                           this.LocationId.             GetHashCode()       * 47 ^
                           this.Currency.               GetHashCode()       * 43 ^
                           this.TariffAssociationId.    GetHashCode()       * 41 ^
                           this.TariffId.               GetHashCode()       * 37 ^
                           this.Status.                 GetHashCode()       * 31 ^

                          (this.End?.                   GetHashCode() ?? 0) * 29 ^
                          (this.AuthorizationReference?.GetHashCode() ?? 0) * 23 ^
                          (this.Connector?.             GetHashCode() ?? 0) * 19 ^
                           this.ChargingPeriods.        CalcHashCode()      * 17 ^
                          (this.TotalCosts?.            GetHashCode() ?? 0) * 13 ^
                          (this.EnergyMeterId?.         GetHashCode() ?? 0) * 11 ^
                          (this.EnergyMeter?.           GetHashCode() ?? 0) *  7 ^
                           this.TransparencySoftwares.  CalcHashCode()      *  5 ^

                           this.Created.                GetHashCode()       *  3 ^
                           this.LastUpdated.            GetHashCode();

            }

        }

        #endregion

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of a session.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="SessionIdURL">An optional charging session identification, e.g. from the HTTP URL.</param>
        /// <param name="VersionIdURL">An optional version identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomSessionParser">A delegate to parse custom session JSON objects.</param>
        public static Session Parse(JObject                                JSON,
                                    Party_Idv3?                            PartyIdURL            = null,
                                    Session_Id?                            SessionIdURL          = null,
                                    UInt64?                                VersionIdURL          = null,
                                    CustomJObjectParserDelegate<Session>?  CustomSessionParser   = null)
        {

            if (TryParse(JSON,
                         out var session,
                         out var errorResponse,
                         PartyIdURL,
                         SessionIdURL,
                         VersionIdURL,
                         CustomSessionParser))
            {
                return session;
            }

            throw new ArgumentException("The given JSON representation of a session is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Session, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a session.
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
        /// Try to parse the given JSON representation of a session.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Session">The parsed charging session.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="SessionIdURL">An optional charging session identification, e.g. from the HTTP URL.</param>
        /// <param name="VersionIdURL">An optional version identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomSessionParser">A delegate to parse custom session JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out Session?      Session,
                                       [NotNullWhen(false)] out String?       ErrorResponse,
                                       Party_Idv3?                            PartyIdURL            = null,
                                       Session_Id?                            SessionIdURL          = null,
                                       UInt64?                                VersionIdURL          = null,
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

                #region Parse PartyId                   [optional]

                if (JSON.ParseOptional("party_id",
                                       "party identification",
                                       Party_Idv3.TryParse,
                                       out Party_Idv3? PartyIdBody,
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

                #region Parse VersionId                 [optional]

                if (JSON.ParseOptional("version",
                                       "version identification",
                                       out UInt64? VersionIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!VersionIdURL.HasValue && !VersionIdBody.HasValue)
                {
                    ErrorResponse = "The version identification is missing!";
                    return false;
                }

                if (VersionIdURL.HasValue && VersionIdBody.HasValue && VersionIdURL.Value != VersionIdBody.Value)
                {
                    ErrorResponse = "The optional version identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion


                #region Parse Start                     [mandatory]

                if (!JSON.ParseMandatory("start_date_time",
                                         "start timestamp",
                                         out DateTimeOffset Start,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse End                       [optional]

                if (JSON.ParseOptional("end_date_time",
                                       "end timestamp",
                                       out DateTimeOffset? End,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Energy                    [mandatory]

                if (!JSON.ParseMandatory("energy",
                                         "charged energy kWh",
                                         WattHour.TryParse,
                                         out WattHour Energy,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CDRToken                  [mandatory]

                if (!JSON.ParseMandatoryJSON("cdr_token",
                                             "charge detail record token",
                                             OCPIv3_0.CDRToken.TryParse,
                                             out CDRToken CDRToken,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthMethod                [mandatory]

                if (!JSON.ParseMandatory("auth_method",
                                         "authentication method",
                                         OCPIv3_0.AuthMethod.TryParse,
                                         out AuthMethod AuthMethod,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthorizationReference    [optional]

                if (JSON.ParseOptional("authorization_reference",
                                       "authorization reference",
                                       OCPIv3_0.AuthorizationReference.TryParse,
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

                #region Parse Connector                 [optional]

                if (!JSON.ParseOptionalJSON("connector",
                                            "session connector",
                                            SessionConnector.TryParse,
                                            out SessionConnector Connector,
                                            out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MeterId                   [optional]

                if (JSON.ParseOptional("meter_id",
                                       "meter identification",
                                       EnergyMeter_Id.TryParse,
                                       out EnergyMeter_Id? MeterId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EnergyMeter               [optional]

                if (JSON.ParseOptionalJSON("energy_meter",
                                           "energy meter",
                                           EnergyMeter<EVSE>.TryParse,
                                           out EnergyMeter<EVSE>? EnergyMeter,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TransparencySoftwares     [optional]

                if (JSON.ParseOptionalJSON("transparency_softwares",
                                           "transparency softwares",
                                           TransparencySoftwareStatus.TryParse,
                                           out IEnumerable<TransparencySoftwareStatus> TransparencySoftwares,
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

                #region Parse TariffAssociationId       [mandatory]

                if (!JSON.ParseMandatory("tariff_association_id",
                                         "tariff association identification",
                                         TariffAssociation_Id.TryParse,
                                         out TariffAssociation_Id TariffAssociationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TariffId                  [mandatory]

                if (!JSON.ParseMandatory("tariff_id",
                                         "tariff identification",
                                         Tariff_Id.TryParse,
                                         out Tariff_Id TariffId,
                                         out ErrorResponse))
                {
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
                                         SessionStatus.TryParse,
                                         out SessionStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse Created                   [optional, NonStandard]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTimeOffset? Created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated               [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTimeOffset LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Session = new Session(

                              null,
                              PartyIdBody   ?? PartyIdURL!.  Value,
                              SessionIdBody ?? SessionIdURL!.Value,
                              VersionIdBody ?? VersionIdURL!.Value,

                              Start,
                              Energy,
                              CDRToken,
                              AuthMethod,
                              LocationId,
                              currency,
                              TariffAssociationId,
                              TariffId,
                              Status,

                              End,
                              AuthorizationReference,
                              Connector,
                              ChargingPeriods,
                              TotalCosts,
                              MeterId,
                              EnergyMeter,
                              TransparencySoftwares,

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
                ErrorResponse  = "The given JSON representation of a session is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSessionSerializer = null, CustomCDRTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeOwnerInformation">Whether to include optional owner information.</param>
        /// <param name="IncludeVersionInformation">Whether to include version information.</param>
        /// <param name="IncludeCreatedTimestamp">Whether to include the created timestamp.</param>
        /// <param name="IncludeExtensions">Whether to include optional data model extensions.</param>
        /// <param name="CustomSessionSerializer">A delegate to serialize custom session JSON objects.</param>
        /// <param name="CustomCDRTokenSerializer">A delegate to serialize custom charge detail record token JSON objects.</param>
        /// <param name="CustomSessionConnectorSerializer">A delegate to serialize custom session connector JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        public JObject ToJSON(Boolean                                             IncludeOwnerInformation            = true,
                              Boolean                                             IncludeVersionInformation          = true,
                              Boolean                                             IncludeCreatedTimestamp            = true,
                              Boolean                                             IncludeExtensions                  = true,
                              CustomJObjectSerializerDelegate<Session>?           CustomSessionSerializer            = null,
                              CustomJObjectSerializerDelegate<CDRToken>?          CustomCDRTokenSerializer           = null,
                              CustomJObjectSerializerDelegate<SessionConnector>?  CustomSessionConnectorSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingPeriod>?    CustomChargingPeriodSerializer     = null,
                              CustomJObjectSerializerDelegate<CDRDimension>?      CustomCDRDimensionSerializer       = null,
                              CustomJObjectSerializerDelegate<Price>?             CustomPriceSerializer              = null)
        {

            var json = JSONObject.Create(

                           IncludeOwnerInformation
                               ? new JProperty("party_id",                  PartyId.               ToString())
                               : null,

                                 new JProperty("id",                        Id.                    ToString()),

                           IncludeVersionInformation
                               ? new JProperty("version",                   VersionId.             ToString())
                               : null,


                                 new JProperty("start_date_time",           Start.                 ToISO8601()),

                           End.HasValue
                               ? new JProperty("end_date_time",             End.             Value.ToISO8601())
                               : null,

                                 new JProperty("energy",                    Energy.Value),

                                 new JProperty("cdr_token",                 CDRToken.              ToJSON(CustomCDRTokenSerializer)),
                                 new JProperty("auth_method",               AuthMethod.            ToString()),

                           AuthorizationReference.HasValue
                               ? new JProperty("authorization_reference",   AuthorizationReference.ToString())
                               : null,

                                 new JProperty("location_id",               LocationId.            ToString()),

                           Connector.HasValue
                               ? new JProperty("connector",                 Connector.Value.       ToJSON(CustomSessionConnectorSerializer))
                               : null,

                           EnergyMeterId.HasValue
                               ? new JProperty("meter_id",                  EnergyMeterId.               ToString())
                               : null,

                                 new JProperty("currency",                  Currency.              ISOCode),

                           ChargingPeriods.Any()
                               ? new JProperty("charging_periods",          new JArray(ChargingPeriods.Select(chargingPeriod => chargingPeriod.ToJSON(CustomChargingPeriodSerializer,
                                                                                                                                                      CustomCDRDimensionSerializer))))
                               : null,

                                 new JProperty("tariff_association_id",     TariffAssociationId.   ToString()),
                                 new JProperty("tariff_id",                 TariffId.              ToString()),

                           TotalCosts.HasValue
                               ? new JProperty("total_cost",                TotalCosts.      Value.ToJSON(CustomPriceSerializer))
                               : null,

                                 new JProperty("status",                    Status.                ToString()),


                           IncludeCreatedTimestamp
                               ? new JProperty("created",                   Created.               ToISO8601())
                               : null,

                                 new JProperty("last_updated",              LastUpdated.           ToISO8601())

                       );

            return CustomSessionSerializer is not null
                       ? CustomSessionSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Session Clone()

            => new(

                   CommonAPI,
                   PartyId.                Clone(),
                   Id.                     Clone(),
                   VersionId,

                   Start,
                   Energy.                 Clone(),
                   CDRToken.               Clone(),
                   AuthMethod.             Clone(),
                   LocationId.             Clone(),
                   Currency.               Clone(),
                   TariffAssociationId.    Clone(),
                   TariffId.               Clone(),
                   Status.                 Clone(),

                   End,
                   AuthorizationReference?.Clone(),
                   Connector?.             Clone(),
                   ChargingPeriods.        Select(chargingPeriod       => chargingPeriod.      Clone()).ToArray(),
                   TotalCosts?.            Clone(),
                   EnergyMeterId?.               Clone(),
                   EnergyMeter?.           Clone(),
                   TransparencySoftwares.  Select(transparencySoftware => transparencySoftware.Clone()).ToArray(),

                   Created,
                   LastUpdated

               );

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject  JSON,
                                                     JObject  Patch,
                                                     EventTracking_Id EventTrackingId)
        {

            foreach (var property in Patch)
            {

                if      (property.Key == "country_code")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'country code' of a session is not allowed!");

                else if (property.Key == "party_id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'party identification' of a session is not allowed!");

                else if (property.Key == "id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'identification' of a session is not allowed!");

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
                                                       "Invalid JSON merge patch of a session: " + errorResponse);

            }

        }

        #endregion


        #region CalcSHA256Hash(CustomSessionSerializer = null, CustomCDRTokenSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this charging session in HEX.
        /// </summary>
        /// <param name="CustomSessionSerializer">A delegate to serialize custom session JSON objects.</param>
        /// <param name="CustomCDRTokenSerializer">A delegate to serialize custom charge detail record token JSON objects.</param>
        /// <param name="CustomSessionConnectorSerializer">A delegate to serialize custom session connector JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        public String CalcSHA256Hash(CustomJObjectSerializerDelegate<Session>?           CustomSessionSerializer            = null,
                                     CustomJObjectSerializerDelegate<CDRToken>?          CustomCDRTokenSerializer           = null,
                                     CustomJObjectSerializerDelegate<SessionConnector>?  CustomSessionConnectorSerializer   = null,
                                     CustomJObjectSerializerDelegate<ChargingPeriod>?    CustomChargingPeriodSerializer     = null,
                                     CustomJObjectSerializerDelegate<CDRDimension>?      CustomCDRDimensionSerializer       = null,
                                     CustomJObjectSerializerDelegate<Price>?             CustomPriceSerializer              = null)
        {

            ETag = SHA256.HashData(
                       ToJSON(
                           true,
                           true,
                           true,
                           true,
                           CustomSessionSerializer,
                           CustomCDRTokenSerializer,
                           CustomSessionConnectorSerializer,
                           CustomChargingPeriodSerializer,
                           CustomCDRDimensionSerializer,
                           CustomPriceSerializer
                       ).ToUTF8Bytes()
                   ).ToBase64();

            return ETag;

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
                   : throw new ArgumentException("The given object is not a session!",
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

            var c = PartyId.            CompareTo(Session.PartyId);

            if (c == 0)
                c = Id.                 CompareTo(Session.Id);

            if (c == 0)
                c = VersionId.          CompareTo(Session.VersionId);


            if (c == 0)
                c = Created.            CompareTo(Session.Created);

            if (c == 0)
                c = LastUpdated.        CompareTo(Session.LastUpdated);


            if (c == 0)
                c = Start.              CompareTo(Session.Start);

            if (c == 0)
                c = Energy.             CompareTo(Session.Energy);

            if (c == 0)
                c = CDRToken.           CompareTo(Session.CDRToken);

            if (c == 0)
                c = AuthMethod.         CompareTo(Session.AuthMethod);

            if (c == 0)
                c = LocationId.         CompareTo(Session.LocationId);

            if (c == 0)
                c = Currency.           CompareTo(Session.Currency);

            if (c == 0)
                c = TariffAssociationId.CompareTo(Session.TariffAssociationId);

            if (c == 0)
                c = TariffId.           CompareTo(Session.TariffId);


            if (c == 0)
                c = Status.             CompareTo(Session.Status);

            if (c == 0)
                c = Created.            CompareTo(Session.Created);

            if (c == 0)
                c = LastUpdated.        CompareTo(Session.LastUpdated);

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

               PartyId.                Equals(Session.PartyId)                 &&
               Id.                     Equals(Session.Id)                      &&
               VersionId.              Equals(Session.VersionId)               &&

               Start.                  Equals(Session.Start)                   &&
               Energy.                 Equals(Session.Energy)                  &&
               CDRToken.               Equals(Session.CDRToken)                &&
               AuthMethod.             Equals(Session.AuthMethod)              &&
               LocationId.             Equals(Session.LocationId)              &&
               Currency.               Equals(Session.Currency)                &&
               TariffAssociationId.    Equals(Session.TariffAssociationId)     &&
               TariffId.               Equals(Session.TariffId)                &&

               Status.                 Equals(Session.Status)                  &&
               Created.    ToISO8601().Equals(Session.Created.    ToISO8601()) &&
               LastUpdated.ToISO8601().Equals(Session.LastUpdated.ToISO8601()) &&

            ((!End.                   HasValue    && !Session.End.                   HasValue)    ||
              (End.                   HasValue    &&  Session.End.                   HasValue    && End.                   Value.Equals(Session.End.                   Value))) &&

            ((!AuthorizationReference.HasValue    && !Session.AuthorizationReference.HasValue)    ||
              (AuthorizationReference.HasValue    &&  Session.AuthorizationReference.HasValue    && AuthorizationReference.Value.Equals(Session.AuthorizationReference.Value))) &&

            ((!EnergyMeterId.               HasValue    && !Session.EnergyMeterId.               HasValue)    ||
              (EnergyMeterId.               HasValue    &&  Session.EnergyMeterId.               HasValue    && EnergyMeterId.               Value.Equals(Session.EnergyMeterId.               Value))) &&

            ((!TotalCosts.            HasValue    && !Session.TotalCosts.            HasValue)    ||
              (TotalCosts.            HasValue    &&  Session.TotalCosts.            HasValue    && TotalCosts.            Value.Equals(Session.TotalCosts.            Value))) &&

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

                   "location: ",  LocationId.ToString(),  ", ",
                   "connector: ", Connector?. ToString() ?? "",  ", ",

                   Energy, " kWh, ",

                   TotalCosts.HasValue
                       ? TotalCosts.Value.ToString() + " " + Currency.ToString() + ", "
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


        #region ToBuilder(NewSessionId = null, NewVersionId = null)

        /// <summary>
        /// Return a builder for this charging session.
        /// </summary>
        /// <param name="NewSessionId">An optional new charging session identification.</param>
        /// <param name="NewVersionId">An optional new version identification.</param>
        public Builder ToBuilder(Session_Id?  NewSessionId   = null,
                                 UInt64?      NewVersionId   = null)

            => new (

                   CommonAPI,
                   PartyId,
                   NewSessionId ?? Id,
                   NewVersionId ?? VersionId,

                   Start,
                   Energy,
                   CDRToken,
                   AuthMethod,
                   LocationId,
                   Currency,
                   TariffAssociationId,
                   TariffId,
                   Status,

                   End,
                   AuthorizationReference,
                   Connector,
                   ChargingPeriods,
                   TotalCosts,
                   EnergyMeterId,
                   EnergyMeter,
                   TransparencySoftwares,

                   Created,
                   LastUpdated

               );

        #endregion

        #region (class) Builder

        /// <summary>
        /// A charging session builder.
        /// </summary>
        public class Builder : ABuilder
        {

            #region Properties

            /// <summary>
            /// The timestamp when the session became ACTIVE in the charging station.
            /// When the session is still PENDING, this field SHALL be set to the time the session was created at the charging station.
            /// When a session goes from PENDING to ACTIVE, this field SHALL be updated to the moment the session went to ACTIVE in the charging station.
            /// </summary>
            [Mandatory]
            public   DateTimeOffset?                          Start                        { get; set; }

            /// <summary>
            /// The timestamp when the session was completed/finished, charging might have finished before the session ends,
            /// for example: EV is full, but parking cost also has to be paid.
            /// </summary>
            [Optional]
            public   DateTimeOffset?                          End                          { get; set; }

            /// <summary>
            /// How many kWh of energy were transferred through the EVSE into the vehicle.
            /// </summary>
            [Mandatory]
            public   WattHour?                                Energy                       { get; set; }

            /// <summary>
            /// Token used to start this charging session, including all the relevant information to identify the unique token.
            /// </summary>
            [Mandatory]
            public   CDRToken?                                CDRToken                     { get; set; }

            /// <summary>
            /// Method used for authorization. This might change during a session.This can happen for example when the session was
            /// started with a reservation according to use case Reserve an EVSE at a location.
            /// Initially the authorization method will be COMMAND which changes to WHITELIST when the driver arrives and starts
            /// charging using a Token that is whitelisted.
            /// </summary>
            [Mandatory]
            public   AuthMethod?                              AuthMethod                   { get; set; }

            /// <summary>
            /// Reference to the authorization given by the eMSP. When the eMSP provided an authorization_reference
            /// in either: realtime authorization, Start a Session or Reserve an EVSE at a Location this field SHALL
            /// contain the same value. When different authorization_reference values have been given by the eMSP that
            /// are relevant to this Session, the last given value SHALL be used here.
            /// </summary>
            [Optional]
            public   AuthorizationReference?                  AuthorizationReference       { get; set; }

            /// <summary>
            /// The identification of the location of this CPO, on which the charging session is/was happening.
            /// </summary>
            [Mandatory]
            public   Location_Id?                             LocationId                   { get; set; }

            /// <summary>
            /// The Connector that the Session happened on. This is allowed to be unset if and only if the session
            /// is created for a reservation for which no EVSE has been assigned yet.
            /// </summary>
            [Optional]
            public   SessionConnector?                        Connector                    { get; set; }

            /// <summary>
            /// The ISO 4217 code of the currency used for this session.
            /// </summary>
            [Mandatory]
            public   Currency?                                Currency                     { get; set; }

            /// <summary>
            /// The optional enumeration of charging periods that can be used to calculate and verify the total cost.
            /// </summary>
            [Optional]
            public   List<ChargingPeriod>                     ChargingPeriods              { get; }

            /// <summary>
            /// The ID of the Tariff Association that was used to look up the Tariff of this Session.
            /// When the session is free, the ID of a Tariff Association for a Free of Charge tariff is to be given in this field.
            /// </summary>
            [Mandatory]
            public   TariffAssociation_Id?                    TariffAssociationId          { get; set; }

            /// <summary>
            /// The ID of the Tariff that was used to compute what this Session costs.
            /// When the session is free, the ID of a Free of Charge tariff is to be given in this field.
            /// </summary>
            [Mandatory]
            public   Tariff_Id?                               TariffId                     { get; set; }

            /// <summary>
            /// The total costs of the session in the specified currency. This is the price that the eMSP will
            /// have to pay to the CPO. A total_cost of 0.00 means free of charge. When omitted, i.e. no price
            /// information is given in the Session object, it does not imply the session is/was free of charge.
            /// </summary>
            [Optional]
            public   Price?                                   TotalCosts                   { get; set; }

            /// <summary>
            /// The status of the session.
            /// </summary>
            [Mandatory]
            public   SessionStatus?                           Status                       { get; set; }

            /// <summary>
            /// The optional identification of the energy meter.
            /// </summary>
            [Optional]
            public   EnergyMeter_Id?                          EnergyMeterId                { get; set; }

            /// <summary>
            /// The optional energy meter used for this session.
            /// </summary>
            [Optional, VendorExtension(VE.GraphDefined, VE.Eichrecht)]
            public   EnergyMeter<EVSE>?                       EnergyMeter                  { get; set; }

            /// <summary>
            /// The enumeration of valid transparency softwares which can be used to validate
            /// the singed charging session and metering data.
            /// </summary>
            [Optional, VendorExtension(VE.GraphDefined, VE.Eichrecht)]
            public   HashSet<TransparencySoftwareStatus>      TransparencySoftwares        { get; }

            /// <summary>
            /// The timestamp when this session was created.
            /// </summary>
            [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
            public   DateTimeOffset?                          Created                      { get; set; }

            /// <summary>
            /// The timestamp when this session was last updated (or created).
            /// </summary>
            [Mandatory]
            public   DateTimeOffset?                          LastUpdated                  { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new charging session builder.
            /// </summary>
            /// <param name="CommonAPI">The OCPI Common API hosting this session.</param>
            /// <param name="PartyId">The party identification of the party that issued this session.</param>
            /// <param name="Id">An identification of the session within the party.</param>
            /// <param name="VersionId">The version identification of the session.</param>
            /// 
            /// <param name="Start">A timestamp when the session became active.</param>
            /// <param name="Energy">An amount of kWhs that had been charged.</param>
            /// <param name="CDRToken">An authentication token used to start this charging session, including all relevant information to identify the unique token.</param>
            /// <param name="AuthMethod">A method used for authentication.</param>
            /// <param name="LocationId">The identification of the location at which the charging session is/was happening.</param>
            /// <param name="Currency">An ISO 4217 code of the currency used for this session.</param>
            /// <param name="TariffAssociationId">The identification of the tariff association that was used to look up the tariff of this session.</param>
            /// <param name="TariffId">The identification of the tariff that was used to compute what this session costs.</param>
            /// <param name="Status">A status of the session.</param>
            /// 
            /// <param name="End">An optional timestamp when the session was completed.</param>
            /// <param name="AuthorizationReference">The optional reference to the authorization given by the eMSP. When the eMSP provided an authorization_reference in either: real-time authorization or StartSession, this field SHALL contain the same value.</param>
            /// <param name="Connector">The optional connector that the session happened on.</param>
            /// <param name="ChargingPeriods">An optional enumeration of charging periods that can be used to calculate and verify the total cost.</param>
            /// <param name="TotalCosts">The total costs of the session in the specified currency. This is the price that the eMSP will have to pay to the CPO. A total_cost of 0.00 means free of charge. When omitted, i.e. no price information is given in the Session object, it does not imply the session is/was free of charge.</param>
            /// <param name="EnergyMeterId">The optional identification of the kWh energy meter.</param>
            /// <param name="EnergyMeter">The optional energy meter used for this session.</param>
            /// <param name="TransparencySoftwares">An optional enumeration of valid transparency softwares which can be used to validate the signed charging session and metering data.</param>
            /// 
            /// <param name="Created">An optional timestamp when this charging session was created.</param>
            /// <param name="LastUpdated">An optional timestamp when this charging session was last updated (or created).</param>
            internal Builder(CommonAPI?                                CommonAPI                = null,
                             Party_Idv3?                               PartyId                  = null,
                             Session_Id?                               Id                       = null,
                             UInt64?                                   VersionId                = null,

                             DateTimeOffset?                           Start                    = null,
                             WattHour?                                 Energy                   = null,
                             CDRToken?                                 CDRToken                 = null,
                             AuthMethod?                               AuthMethod               = null,
                             Location_Id?                              LocationId               = null,
                             Currency?                                 Currency                 = null,
                             TariffAssociation_Id?                     TariffAssociationId      = null,
                             Tariff_Id?                                TariffId                 = null,
                             SessionStatus?                            Status                   = null,

                             DateTimeOffset?                           End                      = null,
                             AuthorizationReference?                   AuthorizationReference   = null,
                             SessionConnector?                         Connector                = null,
                             IEnumerable<ChargingPeriod>?              ChargingPeriods          = null,
                             Price?                                    TotalCosts               = null,
                             EnergyMeter_Id?                           EnergyMeterId            = null,
                             EnergyMeter<EVSE>?                        EnergyMeter              = null,
                             IEnumerable<TransparencySoftwareStatus>?  TransparencySoftwares    = null,

                             DateTimeOffset?                           Created                  = null,
                             DateTimeOffset?                           LastUpdated              = null)

                : base(CommonAPI,
                       PartyId,
                       Id,
                       VersionId)

            {

                this.Start                   = Start;
                this.Energy                  = Energy;
                this.CDRToken                = CDRToken;
                this.AuthMethod              = AuthMethod;
                this.LocationId              = LocationId;
                this.Currency                = Currency;
                this.TariffAssociationId     = TariffAssociationId;
                this.TariffId                = TariffId;
                this.Status                  = Status;

                this.End                     = End;
                this.AuthorizationReference  = AuthorizationReference;
                this.Connector               = Connector;
                this.ChargingPeriods         = ChargingPeriods       is not null ? new List<ChargingPeriod>(ChargingPeriods) : [];
                this.TotalCosts              = TotalCosts;
                this.EnergyMeterId           = EnergyMeterId;
                this.EnergyMeter             = EnergyMeter;
                this.TransparencySoftwares   = TransparencySoftwares is not null ? new HashSet<TransparencySoftwareStatus>(TransparencySoftwares) : [];

                this.Created                 = Created               ?? LastUpdated;
                this.LastUpdated             = LastUpdated           ?? Created;

            }

            #endregion

            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the charging session.
            /// </summary>
            public static implicit operator Session?(Builder? Builder)

                => Builder?.ToImmutable(out _);


            /// <summary>
            /// Return an immutable version of the charging session.
            /// </summary>
            /// <param name="Warnings"></param>
            public Session? ToImmutable(out IEnumerable<Warning> Warnings)
            {

                var warnings = new List<Warning>();

                //if (!PartyId.    HasValue)
                //    throw new ArgumentNullException(nameof(PartyId),    "The party identification of the charging station must not be null or empty!");

                //if (!Id.         HasValue)
                //    throw new ArgumentNullException(nameof(Id),         "The charging station identification must not be null or empty!");

                //if (!VersionId.  HasValue)
                //    throw new ArgumentNullException(nameof(VersionId),  "The version identification of the charging station must not be null or empty!");

                if (!PartyId.  HasValue)
                    warnings.Add(Warning.Create("The party identification of the charging station must not be null or empty!"));

                if (!Id.       HasValue)
                    warnings.Add(Warning.Create("The charging station identification must not be null or empty!"));

                if (!VersionId.HasValue)
                    warnings.Add(Warning.Create("The version identification of the charging station must not be null or empty!"));

                Warnings = warnings;

                return warnings.Count != 0

                           ? null

                           : new Session(

                                 null,
                                 PartyId.            Value,
                                 Id.                 Value,
                                 VersionId.          Value,

                                 Start.              Value,
                                 Energy.             Value,
                                 CDRToken.           Value,
                                 AuthMethod.         Value,
                                 LocationId.         Value,
                                 Currency,
                                 TariffAssociationId.Value,
                                 TariffId.           Value,
                                 Status.             Value,

                                 End,
                                 AuthorizationReference,
                                 Connector,
                                 ChargingPeriods,
                                 TotalCosts,
                                 EnergyMeterId,
                                 EnergyMeter,
                                 TransparencySoftwares,

                                 Created     ?? Timestamp.Now,
                                 LastUpdated ?? Timestamp.Now

                             );

            }

            #endregion

        }

        #endregion

    }

}
