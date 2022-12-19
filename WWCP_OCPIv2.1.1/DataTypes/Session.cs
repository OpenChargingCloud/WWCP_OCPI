/*
 * Copyright (c) 2015-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
        /// The unique identification of the session within the CPOs platform (and suboperator platforms).
        /// </summary>
        [Mandatory]
        public Session_Id                          Id                           { get; }

        /// <summary>
        /// The timestamp when the session became active.
        /// </summary>
        [Mandatory]
        public DateTime                            Start                        { get; }

        /// <summary>
        /// The optional timestamp when the session was completed.
        /// </summary>
        [Optional]
        public DateTime?                           End                          { get; }

#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The amount of kWhs that had been charged.
        /// </summary>
        [Mandatory]
        public Decimal                             kWh                          { get; }
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// The authentication identification.
        /// </summary>
        [Mandatory]
        public Auth_Id                             AuthId                       { get; }

        /// <summary>
        /// The method used for authentication.
        /// </summary>
        [Mandatory]
        public AuthMethods                         AuthMethod                   { get; }

        /// <summary>
        /// The location where this session took place, including only the relevant EVSE and connector.
        /// </summary>
        [Mandatory]
        public Location                            Location                     { get; }

        /// <summary>
        /// The optional identification of the kWh energy meter.
        /// </summary>
        [Optional]
        public Meter_Id?                           MeterId                      { get; }

        /// <summary>
        /// The ISO 4217 code of the currency used for this session.
        /// </summary>
        [Mandatory]
        public Currency                            Currency                     { get; }

        /// <summary>
        /// The optional enumeration of charging periods that can be used to calculate and verify the total cost.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingPeriod>         ChargingPeriods              { get; }

        /// <summary>
        /// The total cost (excluding VAT) of the session in the specified currency.
        /// This is the price that the eMSP will have to pay to the charge point operator.
        /// </summary>
        [Optional]
        public Decimal?                            TotalCost                    { get; }

        /// <summary>
        /// The status of the session.
        /// </summary>
        [Mandatory]
        public SessionStatusTypes                  Status                       { get; }

        /// <summary>
        /// The timestamp when this session was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                            LastUpdated                  { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this charging session.
        /// </summary>
        public String                              ETag                         { get; private set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging session.
        /// </summary>
        /// <param name="Id">An unique identification of the session within the CPOs platform (and suboperator platforms).</param>
        /// <param name="Start">A timestamp when the session became active.</param>
        /// <param name="kWh">An amount of kWhs that had been charged.</param>
        /// <param name="AuthId">An authentication identification.</param>
        /// <param name="AuthMethod">A method used for authentication.</param>
        /// <param name="Location">A location where this session took place, including only the relevant EVSE and connector.</param>
        /// <param name="Currency">An ISO 4217 code of the currency used for this session.</param>
        /// <param name="Status">A status of the session.</param>
        /// 
        /// <param name="End">An optional timestamp when the session was completed.</param>
        /// <param name="MeterId">The optional identification of the kWh energy meter.</param>
        /// <param name="ChargingPeriods">An optional enumeration of charging periods that can be used to calculate and verify the total cost.</param>
        /// <param name="TotalCost">The total cost (excluding VAT) of the session in the specified currency. This is the price that the eMSP will have to pay to the charge point operator.</param>
        /// 
        /// <param name="LastUpdated">A timestamp when this session was last updated (or created).</param>
        /// <param name="CustomSessionSerializer">A delegate to serialize custom session JSON objects.</param>
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        public Session(Session_Id                                               Id,
                       DateTime                                                 Start,
                       Decimal                                                  kWh,
                       Auth_Id                                                  AuthId,
                       AuthMethods                                              AuthMethod,
                       Location                                                 Location,
                       Currency                                                 Currency,
                       SessionStatusTypes                                       Status,

                       DateTime?                                                End                                     = null,
                       Meter_Id?                                                MeterId                                 = null,
                       IEnumerable<ChargingPeriod>?                             ChargingPeriods                         = null,
                       Decimal?                                                 TotalCost                               = null,

                       DateTime?                                                LastUpdated                             = null,
                       CustomJObjectSerializerDelegate<Session>?                CustomSessionSerializer                 = null,
                       CustomJObjectSerializerDelegate<Location>?               CustomLocationSerializer                = null,
                       CustomJObjectSerializerDelegate<AdditionalGeoLocation>?  CustomAdditionalGeoLocationSerializer   = null,
                       CustomJObjectSerializerDelegate<EVSE>?                   CustomEVSESerializer                    = null,
                       CustomJObjectSerializerDelegate<StatusSchedule>?         CustomStatusScheduleSerializer          = null,
                       CustomJObjectSerializerDelegate<Connector>?              CustomConnectorSerializer               = null,
                       CustomJObjectSerializerDelegate<DisplayText>?            CustomDisplayTextSerializer             = null,
                       CustomJObjectSerializerDelegate<BusinessDetails>?        CustomBusinessDetailsSerializer         = null,
                       CustomJObjectSerializerDelegate<Hours>?                  CustomHoursSerializer                   = null,
                       CustomJObjectSerializerDelegate<Image>?                  CustomImageSerializer                   = null,
                       CustomJObjectSerializerDelegate<EnergyMix>?              CustomEnergyMixSerializer               = null,
                       CustomJObjectSerializerDelegate<EnergySource>?           CustomEnergySourceSerializer            = null,
                       CustomJObjectSerializerDelegate<EnvironmentalImpact>?    CustomEnvironmentalImpactSerializer     = null,
                       CustomJObjectSerializerDelegate<ChargingPeriod>?         CustomChargingPeriodSerializer          = null,
                       CustomJObjectSerializerDelegate<CDRDimension>?           CustomCDRDimensionSerializer            = null)

        {

            this.Id               = Id;
            this.Start            = Start;
            this.kWh              = kWh;
            this.AuthId           = AuthId;
            this.AuthMethod       = AuthMethod;
            this.Location         = Location;
            this.Currency         = Currency;
            this.Status           = Status;

            this.End              = End;
            this.MeterId          = MeterId;
            this.ChargingPeriods  = ChargingPeriods?.Distinct() ?? Array.Empty<ChargingPeriod>();
            this.TotalCost        = TotalCost;

            this.LastUpdated      = LastUpdated ?? Timestamp.Now;

            this.ETag             = CalcSHA256Hash(CustomSessionSerializer,
                                                   CustomLocationSerializer,
                                                   CustomAdditionalGeoLocationSerializer,
                                                   CustomEVSESerializer,
                                                   CustomStatusScheduleSerializer,
                                                   CustomConnectorSerializer,
                                                   CustomDisplayTextSerializer,
                                                   CustomBusinessDetailsSerializer,
                                                   CustomHoursSerializer,
                                                   CustomImageSerializer,
                                                   CustomEnergyMixSerializer,
                                                   CustomEnergySourceSerializer,
                                                   CustomEnvironmentalImpactSerializer,
                                                   CustomChargingPeriodSerializer,
                                                   CustomCDRDimensionSerializer);

        }

        #endregion


        #region (static) Parse   (JSON, SessionIdURL = null, CustomSessionParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging session.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SessionIdURL">An optional charging session identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomSessionParser">A delegate to parse custom session JSON objects.</param>
        public static Session Parse(JObject                                JSON,
                                    Session_Id?                            SessionIdURL          = null,
                                    CustomJObjectParserDelegate<Session>?  CustomSessionParser   = null)
        {

            if (TryParse(JSON,
                         out var session,
                         out var errorResponse,
                         SessionIdURL,
                         CustomSessionParser))
            {
                return session!;
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
        public static Boolean TryParse(JObject       JSON,
                                       out Session?  Session,
                                       out String?   ErrorResponse)

            => TryParse(JSON,
                        out Session,
                        out ErrorResponse,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging session.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Session">The parsed charging session.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="SessionIdURL">An optional charging session identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomSessionParser">A delegate to parse custom session JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       out Session?                           Session,
                                       out String?                            ErrorResponse,
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

                #region Parse Id                       [optional]

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

                #region Parse Start                    [mandatory]

                if (!JSON.ParseMandatory("start_date_time",
                                         "start timestamp",
                                         out DateTime Start,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse End                      [optional]

                if (JSON.ParseOptional("end_date_time",
                                       "end timestamp",
                                       out DateTime? End,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse KWh                      [mandatory]

                if (!JSON.ParseMandatory("kwh",
                                         "charged kWh",
                                         out Decimal KWh,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthId                   [mandatory]

                if (!JSON.ParseMandatory("auth_id",
                                         "authorization identification",
                                         Auth_Id.TryParse,
                                         out Auth_Id AuthId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthMethod               [mandatory]

                if (!JSON.ParseMandatoryEnum("auth_method",
                                             "authentication method",
                                             out AuthMethods AuthMethod,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Location                 [mandatory]

                if (!JSON.ParseMandatoryJSON("location",
                                             "location",
                                             OCPIv2_1_1.Location.TryParse,
                                             out Location? Location,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (Location is null)
                    return false;

                #endregion

                #region Parse MeterId                  [optional]

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

                #region Parse Currency                 [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         OCPIv2_1_1.Currency.TryParse,
                                         out Currency Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ChargingPeriods          [optional]

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

                #region Parse TotalCost                [optional]

                if (JSON.ParseOptional("total_cost",
                                       "total cost",
                                       out Decimal? TotalCost,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Status                   [mandatory]

                if (!JSON.ParseMandatoryEnum("status",
                                             "session status",
                                             out SessionStatusTypes Status,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse LastUpdated              [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Session = new Session(SessionIdBody ?? SessionIdURL!.Value,
                                      Start,
                                      KWh,
                                      AuthId,
                                      AuthMethod,
                                      Location,
                                      Currency,
                                      Status,

                                      End,
                                      MeterId,
                                      ChargingPeriods,
                                      TotalCost,

                                      LastUpdated);


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
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Session>?                CustomSessionSerializer                 = null,
                              CustomJObjectSerializerDelegate<Location>?               CustomLocationSerializer                = null,
                              CustomJObjectSerializerDelegate<AdditionalGeoLocation>?  CustomAdditionalGeoLocationSerializer   = null,
                              CustomJObjectSerializerDelegate<EVSE>?                   CustomEVSESerializer                    = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>?         CustomStatusScheduleSerializer          = null,
                              CustomJObjectSerializerDelegate<Connector>?              CustomConnectorSerializer               = null,
                              CustomJObjectSerializerDelegate<DisplayText>?            CustomDisplayTextSerializer             = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?        CustomBusinessDetailsSerializer         = null,
                              CustomJObjectSerializerDelegate<Hours>?                  CustomHoursSerializer                   = null,
                              CustomJObjectSerializerDelegate<Image>?                  CustomImageSerializer                   = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?              CustomEnergyMixSerializer               = null,
                              CustomJObjectSerializerDelegate<EnergySource>?           CustomEnergySourceSerializer            = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?    CustomEnvironmentalImpactSerializer     = null,
                              CustomJObjectSerializerDelegate<ChargingPeriod>?         CustomChargingPeriodSerializer          = null,
                              CustomJObjectSerializerDelegate<CDRDimension>?           CustomCDRDimensionSerializer            = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("id",                             Id.                    ToString()),

                           new JProperty("start_date_time",                Start.                 ToIso8601()),

                           End.HasValue
                               ? new JProperty("end_date_time",            End.             Value.ToIso8601())
                               : null,

                           new JProperty("kwh",                            kWh),

                           new JProperty("auth_id",                        AuthId.                ToString()),
                           new JProperty("auth_method",                    AuthMethod.            ToString()),

                           new JProperty("location",                       Location.              ToJSON(CustomLocationSerializer,
                                                                                                         CustomAdditionalGeoLocationSerializer,
                                                                                                         CustomEVSESerializer,
                                                                                                         CustomStatusScheduleSerializer,
                                                                                                         CustomConnectorSerializer,
                                                                                                         CustomDisplayTextSerializer,
                                                                                                         CustomBusinessDetailsSerializer,
                                                                                                         CustomHoursSerializer,
                                                                                                         CustomImageSerializer,
                                                                                                         CustomEnergyMixSerializer,
                                                                                                         CustomEnergySourceSerializer,
                                                                                                         CustomEnvironmentalImpactSerializer)),

                           MeterId.HasValue
                               ? new JProperty("meter_id",                 MeterId.               ToString())
                               : null,

                           new JProperty("currency",                       Currency.              ToString()),

                           ChargingPeriods.Any()
                               ? new JProperty("charging_periods",         new JArray(ChargingPeriods.      Select(chargingPeriod => chargingPeriod.ToJSON(CustomChargingPeriodSerializer,
                                                                                                                                                           CustomCDRDimensionSerializer))))
                               : null,

                           TotalCost.HasValue
                               ? new JProperty("total_cost",               TotalCost.      Value)
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

            if (SessionPatch is null)
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
                             out var patchedSession,
                             out var errorResponse))
                {

                    return PatchResult<Session>.Success(patchedSession,
                                                        errorResponse);

                }

                else
                    return PatchResult<Session>.Failed(this,
                                                       "Invalid JSON merge patch of a charging session: " + errorResponse);

            }

        }

        #endregion


        #region CalcSHA256Hash(CustomSessionSerializer = null, CustomCDRTokenSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this charging session in HEX.
        /// </summary>
        /// <param name="CustomSessionSerializer">A delegate to serialize custom session JSON objects.</param>
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        public String CalcSHA256Hash(CustomJObjectSerializerDelegate<Session>?                CustomSessionSerializer                 = null,
                                     CustomJObjectSerializerDelegate<Location>?               CustomLocationSerializer                = null,
                                     CustomJObjectSerializerDelegate<AdditionalGeoLocation>?  CustomAdditionalGeoLocationSerializer   = null,
                                     CustomJObjectSerializerDelegate<EVSE>?                   CustomEVSESerializer                    = null,
                                     CustomJObjectSerializerDelegate<StatusSchedule>?         CustomStatusScheduleSerializer          = null,
                                     CustomJObjectSerializerDelegate<Connector>?              CustomConnectorSerializer               = null,
                                     CustomJObjectSerializerDelegate<DisplayText>?            CustomDisplayTextSerializer             = null,
                                     CustomJObjectSerializerDelegate<BusinessDetails>?        CustomBusinessDetailsSerializer         = null,
                                     CustomJObjectSerializerDelegate<Hours>?                  CustomHoursSerializer                   = null,
                                     CustomJObjectSerializerDelegate<Image>?                  CustomImageSerializer                   = null,
                                     CustomJObjectSerializerDelegate<EnergyMix>?              CustomEnergyMixSerializer               = null,
                                     CustomJObjectSerializerDelegate<EnergySource>?           CustomEnergySourceSerializer            = null,
                                     CustomJObjectSerializerDelegate<EnvironmentalImpact>?    CustomEnvironmentalImpactSerializer     = null,
                                     CustomJObjectSerializerDelegate<ChargingPeriod>?         CustomChargingPeriodSerializer          = null,
                                     CustomJObjectSerializerDelegate<CDRDimension>?           CustomCDRDimensionSerializer            = null)
        {

            this.ETag = SHA256.Create().ComputeHash(ToJSON(CustomSessionSerializer,
                                                           CustomLocationSerializer,
                                                           CustomAdditionalGeoLocationSerializer,
                                                           CustomEVSESerializer,
                                                           CustomStatusScheduleSerializer,
                                                           CustomConnectorSerializer,
                                                           CustomDisplayTextSerializer,
                                                           CustomBusinessDetailsSerializer,
                                                           CustomHoursSerializer,
                                                           CustomImageSerializer,
                                                           CustomEnergyMixSerializer,
                                                           CustomEnergySourceSerializer,
                                                           CustomEnvironmentalImpactSerializer,
                                                           CustomChargingPeriodSerializer,
                                                           CustomCDRDimensionSerializer).ToUTF8Bytes()).ToBase64();

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

            var c = Id.         CompareTo(Session.Id);

            if (c == 0)
                c = Start.      CompareTo(Session.Start);

            if (c == 0)
                c = kWh.        CompareTo(Session.kWh);

            if (c == 0)
                c = AuthId.     CompareTo(Session.AuthId);

            if (c == 0)
                c = AuthMethod. CompareTo(Session.AuthMethod);

            if (c == 0)
                c = Currency.   CompareTo(Session.Currency);

            if (c == 0)
                c = Status.     CompareTo(Session.Status);

            if (c == 0)
                c = LastUpdated.CompareTo(Session.LastUpdated);

            // Location,
            // 
            // End
            // MeterId
            // EnergyMeter
            // TransparencySoftwares
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

               Id.                     Equals(Session.Id)                      &&
               Start.                  Equals(Session.Start)                   &&
               kWh.                    Equals(Session.kWh)                     &&
               AuthId.                 Equals(Session.AuthId)                  &&
               AuthMethod.             Equals(Session.AuthMethod)              &&
               Location.               Equals(Session.Location)                &&
               Currency.               Equals(Session.Currency)                &&
               Status.                 Equals(Session.Status)                  &&
               LastUpdated.ToIso8601().Equals(Session.LastUpdated.ToIso8601()) &&

            ((!End.      HasValue && !Session.End.      HasValue) ||
              (End.      HasValue &&  Session.End.      HasValue && End.      Value.Equals(Session.End.      Value))) &&

            ((!MeterId.  HasValue && !Session.MeterId.  HasValue) ||
              (MeterId.  HasValue &&  Session.MeterId.  HasValue && MeterId.  Value.Equals(Session.MeterId.  Value))) &&

            ((!TotalCost.HasValue && !Session.TotalCost.HasValue) ||
              (TotalCost.HasValue &&  Session.TotalCost.HasValue && TotalCost.Value.Equals(Session.TotalCost.Value))) &&

               ChargingPeriods.Count().Equals(Session.ChargingPeriods.Count()) &&
               ChargingPeriods.All(chargingPeriod => Session.ChargingPeriods.Contains(chargingPeriod));

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Id.              GetHashCode()       * 47 ^
                       Start.           GetHashCode()       * 43 ^
                       kWh.             GetHashCode()       * 41 ^
                       AuthId.          GetHashCode()       * 37 ^
                       AuthMethod.      GetHashCode()       * 31 ^
                       Location.        GetHashCode()       * 29 ^
                       Currency.        GetHashCode()       * 23 ^
                       Status.          GetHashCode()       * 19 ^
                       LastUpdated.     GetHashCode()       * 17 ^

                      (End?.            GetHashCode() ?? 0) * 13 ^
                      (MeterId?.        GetHashCode() ?? 0) * 11 ^
                      (ChargingPeriods?.GetHashCode() ?? 0) *  3 ^
                       TotalCost?.      GetHashCode() ?? 0;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,                      " (",
                   Status,                  "), ",

                   Start.ToIso8601(),

                   End.HasValue
                       ? " - " + End.Value.ToIso8601() + ", "
                       : ", ",

                   AuthId,                  " (",
                   AuthMethod,              "), ",

                   "location: ",
                   Location.Id.ToString(),  ", ",

                   kWh,                     " kWh, ",

                   TotalCost.HasValue
                       ? TotalCost.Value.ToString() + " " + Currency.ToString() + ", "
                       : "",

                   ChargingPeriods.Any()
                       ? ChargingPeriods.Count() + " charging period(s), "
                       : "",

                   MeterId.HasValue
                       ? "meter: " + MeterId.Value.ToString() + ", "
                       : "",

                   // EnergyMeter
                   // TransparencySoftwares

                   "last updated: " + LastUpdated.ToIso8601()

               );

        #endregion

    }

}
