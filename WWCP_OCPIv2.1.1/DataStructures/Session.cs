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
using cloud.charging.open.protocols.OCPIv2_1_1.HTTP;

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
        /// The unique identification of the session within the CPOs platform (and suboperator platforms).
        /// </summary>
        [Mandatory]
        public   Session_Id                          Id                           { get; }

        /// <summary>
        /// The timestamp when the session became active.
        /// </summary>
        [Mandatory]
        public   DateTime                            Start                        { get; }

        /// <summary>
        /// The optional timestamp when the session was completed.
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
        /// The authentication identification.
        /// </summary>
        [Mandatory]
        public   Auth_Id                             AuthId                       { get; }

        /// <summary>
        /// The method used for authentication.
        /// </summary>
        [Mandatory]
        public   AuthMethods                         AuthMethod                   { get; }

        /// <summary>
        /// The location where this session took place, including only the relevant EVSE and connector.
        /// </summary>
        [Mandatory]
        public   Location                            Location                     { get; }

        /// <summary>
        /// The optional identification of the kWh energy meter.
        /// </summary>
        [Optional]
        public   EnergyMeter_Id?                     MeterId                      { get; }

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
        /// The total cost (excluding VAT) of the session in the specified currency.
        /// This is the price that the eMSP will have to pay to the charge point operator.
        /// </summary>
        [Optional]
        public   Decimal?                            TotalCost                    { get; }

        /// <summary>
        /// The status of the session.
        /// </summary>
        [Mandatory]
        public   SessionStatusTypes                  Status                       { get; }

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
        /// The SHA256 hash of the JSON representation of this charging session.
        /// </summary>
        public   String                              ETag                         { get; private set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging session.
        /// </summary>
        /// <param name="CountryCode">An ISO-3166 alpha-2 country code of the charge point operator that 'owns' this session.</param>
        /// <param name="PartyId">An identification of the charge point operator that 'owns' this session (following the ISO-15118 standard).</param>
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
        /// <param name="EnergyMeterId">The optional identification of the kWh energy meter.</param>
        /// <param name="ChargingPeriods">An optional enumeration of charging periods that can be used to calculate and verify the total cost.</param>
        /// <param name="TotalCost">The total cost (excluding VAT) of the session in the specified currency. This is the price that the eMSP will have to pay to the charge point operator.</param>
        /// 
        /// <param name="Created">An optional timestamp when this charging session was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this charging session was last updated (or created).</param>
        /// 
        /// <param name="CustomSessionSerializer">A delegate to serialize custom session JSON objects.</param>
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom EVSE energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        public Session(CountryCode                                                   CountryCode,
                       Party_Id                                                      PartyId,
                       Session_Id                                                    Id,
                       DateTime                                                      Start,
                       WattHour                                                      kWh,
                       Auth_Id                                                       AuthId,
                       AuthMethods                                                   AuthMethod,
                       Location                                                      Location,
                       Currency                                                      Currency,
                       SessionStatusTypes                                            Status,

                       DateTime?                                                     End                                          = null,
                       EnergyMeter_Id?                                               EnergyMeterId                                = null,
                       IEnumerable<ChargingPeriod>?                                  ChargingPeriods                              = null,
                       Decimal?                                                      TotalCost                                    = null,

                       DateTime?                                                     Created                                      = null,
                       DateTime?                                                     LastUpdated                                  = null,
                       EMSP_Id?                                                      EMSPId                                       = null,

                       CustomJObjectSerializerDelegate<Session>?                     CustomSessionSerializer                      = null,
                       CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                     = null,
                       CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer        = null,
                       CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                       CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                       CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                       CustomJObjectSerializerDelegate<EnergyMeter<Location>>?       CustomLocationEnergyMeterSerializer          = null,
                       CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                       CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                       CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                       CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                       CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer              = null,
                       CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                        = null,
                       CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null,
                       CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                    = null,
                       CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                 = null,
                       CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer          = null,
                       CustomJObjectSerializerDelegate<ChargingPeriod>?              CustomChargingPeriodSerializer               = null,
                       CustomJObjectSerializerDelegate<CDRDimension>?                CustomCDRDimensionSerializer                 = null)

        {

            this.CountryCode      = CountryCode;
            this.PartyId          = PartyId;
            this.Id               = Id;
            this.Start            = Start;
            this.kWh              = kWh;
            this.AuthId           = AuthId;
            this.AuthMethod       = AuthMethod;
            this.Location         = Location;
            this.Currency         = Currency;
            this.Status           = Status;

            this.End              = End;
            this.MeterId          = EnergyMeterId;
            this.ChargingPeriods  = ChargingPeriods?.Distinct() ?? [];
            this.TotalCost        = TotalCost;

            this.Created          = Created                     ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated      = LastUpdated                 ?? Created     ?? Timestamp.Now;

            this.ETag             = CalcSHA256Hash(EMSPId,
                                                   CustomSessionSerializer,
                                                   CustomLocationSerializer,
                                                   CustomAdditionalGeoLocationSerializer,
                                                   CustomEVSESerializer,
                                                   CustomStatusScheduleSerializer,
                                                   CustomConnectorSerializer,
                                                   CustomLocationEnergyMeterSerializer,
                                                   CustomEVSEEnergyMeterSerializer,
                                                   CustomTransparencySoftwareStatusSerializer,
                                                   CustomTransparencySoftwareSerializer,
                                                   CustomDisplayTextSerializer,
                                                   CustomBusinessDetailsSerializer,
                                                   CustomHoursSerializer,
                                                   CustomImageSerializer,
                                                   CustomEnergyMixSerializer,
                                                   CustomEnergySourceSerializer,
                                                   CustomEnvironmentalImpactSerializer,
                                                   CustomChargingPeriodSerializer,
                                                   CustomCDRDimensionSerializer);

            unchecked
            {

                hashCode = this.CountryCode.    GetHashCode()       * 53 ^
                           this.PartyId.        GetHashCode()       * 47 ^
                           this.Id.             GetHashCode()       * 43 ^
                           this.Start.          GetHashCode()       * 41 ^
                           this.kWh.            GetHashCode()       * 37 ^
                           this.AuthId.         GetHashCode()       * 31 ^
                           this.AuthMethod.     GetHashCode()       * 29 ^
                           this.Location.       GetHashCode()       * 23 ^
                           this.Currency.       GetHashCode()       * 19 ^
                           this.Status.         GetHashCode()       * 17 ^
                           this.Created.        GetHashCode()       * 13 ^
                           this.LastUpdated.    GetHashCode()       * 11 ^

                          (this.End?.           GetHashCode() ?? 0) *  7 ^
                          (this.MeterId?.       GetHashCode() ?? 0) *  5 ^
                           this.ChargingPeriods.CalcHashCode()      *  3 ^
                           this.TotalCost?.     GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CountryCodeURL = null, PartyIdURL = null, SessionIdURL = null, CustomSessionParser = null)

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

                #region Parse CountryCode              [optional, internal]

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

                #region Parse PartyIdURL               [optional, internal]

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

                if (!JSON.ParseMandatory("start_datetime",
                                         "start timestamp",
                                         out DateTime Start,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse End                      [optional]

                if (JSON.ParseOptional("end_datetime",
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
                                         WattHour.TryParseKWh,
                                         out WattHour KWh,
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

                var locationJSON = JSON["location"];

                if (locationJSON is not JObject locationJSONObject)
                    return false;

                if (!OCPIv2_1_1.Location.TryParse(locationJSONObject,
                                                  out Location? Location,
                                                  out ErrorResponse,
                                                  CountryCodeBody ?? CountryCodeURL!.Value,
                                                  PartyIdBody     ?? PartyIdURL!.    Value,
                                                  null) ||
                    Location is null)
                {
                    return false;
                }

                //if (!JSON.ParseMandatoryJSON("location",
                //                             "location",
                //                             json2 => 
                //                             out Location? Location,
                //                             out ErrorResponse) ||
                //     Location is null)
                //{
                //    return false;
                //}

                #endregion

                #region Parse MeterId                  [optional]

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

                #region Parse Currency                 [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         Currency.TryParse,
                                         out Currency currency,
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


                #region Parse Created                  [optional, NonStandard]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTime? Created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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


                Session = new Session(

                              CountryCodeBody ?? CountryCodeURL!.Value,
                              PartyIdBody     ?? PartyIdURL!.    Value,
                              SessionIdBody   ?? SessionIdURL!.  Value,
                              Start,
                              KWh,
                              AuthId,
                              AuthMethod,
                              Location,
                              currency,
                              Status,

                              End,
                              MeterId,
                              ChargingPeriods,
                              TotalCost,

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

        #region ToJSON(IncludeOwnerInformation = false, CustomSessionSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeOwnerInformation">Include optional owner information.</param>
        /// <param name="IncludeEnergyMeter">Whether to include the energy meter.</param>
        /// <param name="EMSPId">The optional EMSP identification, e.g. for including the right charging tariff.</param>
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
        public JObject ToJSON(Boolean                                                       IncludeOwnerInformation                      = false,
                              Boolean                                                       IncludeEnergyMeter                           = false,
                              EMSP_Id?                                                      EMSPId                                       = null,
                              CustomJObjectSerializerDelegate<Session>?                     CustomSessionSerializer                      = null,
                              CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                     = null,
                              CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer        = null,
                              CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                              CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                              CustomJObjectSerializerDelegate<EnergyMeter<Location>>?       CustomLocationEnergyMeterSerializer          = null,
                              CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                              CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                              CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                              CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer              = null,
                              CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                        = null,
                              CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                    = null,
                              CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                 = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer          = null,
                              CustomJObjectSerializerDelegate<ChargingPeriod>?              CustomChargingPeriodSerializer               = null,
                              CustomJObjectSerializerDelegate<CDRDimension>?                CustomCDRDimensionSerializer                 = null)
        {

            var json = JSONObject.Create(

                           IncludeOwnerInformation
                               ? new JProperty("country_code",                   CountryCode.           ToString())
                               : null,

                           IncludeOwnerInformation
                               ? new JProperty("party_id",                       PartyId.               ToString())
                               : null,

                                 new JProperty("id",                             Id.                    ToString()),

                                 new JProperty("start_datetime",                 Start.                 ToISO8601()),

                           End.HasValue
                               ? new JProperty("end_datetime",                   End.             Value.ToISO8601())
                               : null,

                                 new JProperty("kwh",                            kWh.kWh),

                                 new JProperty("auth_id",                        AuthId.                ToString()),
                                 new JProperty("auth_method",                    AuthMethod.            ToString()),

                                 new JProperty("location",                       Location.              ToJSON(false,
                                                                                                               IncludeEnergyMeter,
                                                                                                               EMSPId,
                                                                                                               CustomLocationSerializer,
                                                                                                               CustomAdditionalGeoLocationSerializer,
                                                                                                               CustomEVSESerializer,
                                                                                                               CustomStatusScheduleSerializer,
                                                                                                               CustomConnectorSerializer,
                                                                                                               CustomLocationEnergyMeterSerializer,
                                                                                                               CustomEVSEEnergyMeterSerializer,
                                                                                                               CustomTransparencySoftwareStatusSerializer,
                                                                                                               CustomTransparencySoftwareSerializer,
                                                                                                               CustomDisplayTextSerializer,
                                                                                                               CustomBusinessDetailsSerializer,
                                                                                                               CustomHoursSerializer,
                                                                                                               CustomImageSerializer,
                                                                                                               CustomEnergyMixSerializer,
                                                                                                               CustomEnergySourceSerializer,
                                                                                                               CustomEnvironmentalImpactSerializer)),

                           MeterId.HasValue
                               ? new JProperty("meter_id",                       MeterId.               ToString())
                               : null,

                                 new JProperty("currency",                       Currency.              ISOCode),

                           ChargingPeriods.Any()
                               ? new JProperty("charging_periods",               new JArray(ChargingPeriods.      Select(chargingPeriod => chargingPeriod.ToJSON(CustomChargingPeriodSerializer,
                                                                                                                                                                 CustomCDRDimensionSerializer))))
                               : null,

                           TotalCost.HasValue
                               ? new JProperty("total_cost",                     TotalCost.      Value)
                               : null,

                                 new JProperty("status",                         Status.                ToString()),


                                 new JProperty("created",                        Created.               ToISO8601()),
                                 new JProperty("last_updated",                   LastUpdated.           ToISO8601())

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

            => new (
                   CountryCode.Clone(),
                   PartyId.    Clone(),
                   Id.         Clone(),
                   Start,
                   kWh,
                   AuthId.     Clone(),
                   AuthMethod,
                   Location.   Clone(),
                   Currency.   Clone(),
                   Status,

                   End,
                   MeterId?.   Clone(),
                   ChargingPeriods.Select(chargingPeriod => chargingPeriod.Clone()).ToArray(),
                   TotalCost,

                   Created,
                   LastUpdated
               );

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject           JSON,
                                                     JObject           Patch,
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

            if (!SessionPatch.HasValues)
                return PatchResult<Session>.Failed(EventTrackingId,
                                                   this,
                                                   "The given charging session patch must not be null or empty!");

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
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        public String CalcSHA256Hash(EMSP_Id?                                                      EMSPId                                       = null,
                                     CustomJObjectSerializerDelegate<Session>?                     CustomSessionSerializer                      = null,
                                     CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                     = null,
                                     CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer        = null,
                                     CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                                     CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                                     CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                                     CustomJObjectSerializerDelegate<EnergyMeter<Location>>?       CustomLocationEnergyMeterSerializer          = null,
                                     CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer              = null,
                                     CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                                     CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                                     CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                                     CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer              = null,
                                     CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                        = null,
                                     CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null,
                                     CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                    = null,
                                     CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                 = null,
                                     CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer          = null,
                                     CustomJObjectSerializerDelegate<ChargingPeriod>?              CustomChargingPeriodSerializer               = null,
                                     CustomJObjectSerializerDelegate<CDRDimension>?                CustomCDRDimensionSerializer                 = null)
        {

            ETag = SHA256.HashData(
                       ToJSON(
                           true,
                           true,
                           EMSPId,
                           CustomSessionSerializer,
                           CustomLocationSerializer,
                           CustomAdditionalGeoLocationSerializer,
                           CustomEVSESerializer,
                           CustomStatusScheduleSerializer,
                           CustomConnectorSerializer,
                           CustomLocationEnergyMeterSerializer,
                           CustomEVSEEnergyMeterSerializer,
                           CustomTransparencySoftwareStatusSerializer,
                           CustomTransparencySoftwareSerializer,
                           CustomDisplayTextSerializer,
                           CustomBusinessDetailsSerializer,
                           CustomHoursSerializer,
                           CustomImageSerializer,
                           CustomEnergyMixSerializer,
                           CustomEnergySourceSerializer,
                           CustomEnvironmentalImpactSerializer,
                           CustomChargingPeriodSerializer,
                           CustomCDRDimensionSerializer
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
                c = Created.    CompareTo(Session.Created);

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

               CountryCode.            Equals(Session.CountryCode)             &&
               PartyId.                Equals(Session.PartyId)                 &&
               Id.                     Equals(Session.Id)                      &&
               Start.                  Equals(Session.Start)                   &&
               kWh.                    Equals(Session.kWh)                     &&
               AuthId.                 Equals(Session.AuthId)                  &&
               AuthMethod.             Equals(Session.AuthMethod)              &&
               Location.               Equals(Session.Location)                &&
               Currency.               Equals(Session.Currency)                &&
               Status.                 Equals(Session.Status)                  &&
               Created.    ToISO8601().Equals(Session.Created.    ToISO8601()) &&
               LastUpdated.ToISO8601().Equals(Session.LastUpdated.ToISO8601()) &&

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

                   "last updated: " + LastUpdated.ToISO8601()

               );

        #endregion


    }

}
