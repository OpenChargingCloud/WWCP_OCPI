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

using System.Text;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The charge detail record describes the charging session and its costs,
    /// how these costs are composed, etc.
    /// </summary>
    public class CDR : IHasId<CDR_Id>,
                       IEquatable<CDR>,
                       IComparable<CDR>,
                       IComparable
    {

        #region Properties

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this charge detail record.
        /// </summary>
        [Mandatory]
        public CountryCode                         CountryCode                 { get; }

        /// <summary>
        /// The identification of the charge point operator that 'owns' this charge detail record
        /// (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id                            PartyId                     { get; }

        /// <summary>
        /// The identification of the charge detail record within the charge point operator's platform
        /// (and suboperator platforms).
        /// CiString(39)
        /// </summary>
        [Mandatory]
        public CDR_Id                              Id                          { get; }

        /// <summary>
        /// The start timestamp of the charging session, or in-case of a reservation
        /// (before the start of a session) the start of the reservation.
        /// </summary>
        [Mandatory]
        public DateTime                            Start                       { get; }

        /// <summary>
        /// The timestamp when the session was completed/finished.
        /// Charging might have finished before the session ends, for example:
        /// EV is full, but parking cost also has to be paid.
        /// </summary>
        [Mandatory]
        public DateTime                            End                         { get; }

        /// <summary>
        /// The reference to a token, identified by the auth_id field of the token.
        /// </summary>
        [Mandatory]
        public Auth_Id                             AuthId                      { get; }

        /// <summary>
        /// The authentication method used.
        /// </summary>
        [Mandatory]
        public AuthMethods                         AuthMethod                  { get; }

        /// <summary>
        /// The location where the charging session took place, including only the relevant
        /// EVSE and connector.
        /// </summary>
        [Mandatory]
        public Location                            Location                    { get; }

        /// <summary>
        /// The optional identification of the energy meter.
        /// </summary>
        [Optional]
        public Meter_Id?                           MeterId                     { get; }

        /// <summary>
        /// The optional energy meter.
        /// </summary>
        [Optional, NonStandard]
        public EnergyMeter?                        EnergyMeter                 { get; }

        /// <summary>
        /// The enumeration of valid transparency softwares which can be used to validate
        /// the singed charging session and metering data.
        /// </summary>
        [Optional, NonStandard]
        public IEnumerable<TransparencySoftware>   TransparencySoftwares        { get; }

        /// <summary>
        /// The ISO 4217 code of the currency used for this charge detail record.
        /// </summary>
        [Mandatory]
        public Currency                            Currency                    { get; }

        /// <summary>
        /// The enumeration of relevant charging tariffs.
        /// </summary>
        [Optional]
        public IEnumerable<Tariff>                 Tariffs                     { get; }

        /// <summary>
        /// The enumeration of charging periods that make up this charging session.
        /// A session consist of 1 or more periodes with, each period has a
        /// different relevant charging tariff.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingPeriod>         ChargingPeriods             { get; }

        /// <summary>
        /// The optional signed metering data that belongs to this charging session.
        /// (Backported from OCPI v2.2)
        /// </summary>
        [Optional, NonStandard]
        public SignedData?                         SignedData                  { get; }

        /// <summary>
        /// The total cost (excluding VAT) of this transaction.
        /// </summary>
        [Mandatory]
        public Decimal                             TotalCost                  { get; }

        /// <summary>
        /// The total energy charged in kWh.
        /// </summary>
        [Mandatory]
        public Decimal                             TotalEnergy                 { get; }

        /// <summary>
        /// The total duration of the charging session, including the duration of charging and not charging.
        /// </summary>
        [Mandatory]
        public TimeSpan                            TotalTime                   { get; }

        /// <summary>
        /// The optional total duration of the charging session where the EV was not charging
        /// (no energy was transferred between EVSE and EV).
        /// </summary>
        [Optional]
        public TimeSpan?                           TotalParkingTime            { get; }

        /// <summary>
        /// The optional total duration of the charging session where the EV was not charging
        /// (no energy was transferred between EVSE and EV).
        /// </summary>
        [Optional]
        public TimeSpan                            TotalChargingTime
            => TotalTime - (TotalParkingTime ?? TimeSpan.Zero);

        /// <summary>
        /// The optional remark can be used to provide addition human
        /// readable information to the charge detail record, for example a
        /// reason why a transaction was stopped.
        /// </summary>
        [Optional]
        public String?                             Remark                      { get; }

        /// <summary>
        /// The timestamp when this charge detail record was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                            LastUpdated                 { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this charge detail record.
        /// </summary>
        public String                              ETag                        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge detail record.
        /// </summary>
        /// <param name="CountryCode">An ISO-3166 alpha-2 country code of the charge point operator that 'owns' this charge detail record.</param>
        /// <param name="PartyId">An identification of the charge point operator that 'owns' this charge detail record (following the ISO-15118 standard).</param>
        /// <param name="Id">An identification of the charge detail record within the charge point operator's platform (and suboperator platforms).</param>
        /// <param name="Start">The start timestamp of the charging session, or in-case of a reservation (before the start of a session) the start of the reservation.</param>
        /// <param name="End">The timestamp when the session was completed/finished. Charging might have finished before the session ends, for example: EV is full, but parking cost also has to be paid.</param>
        /// <param name="AuthId">The reference to a token, identified by the auth_id field of the token.</param>
        /// <param name="Location">The location where the charging session took place, including only the relevant EVSE and connector.</param>
        /// <param name="AuthMethod">The authentication method used.</param>
        /// <param name="Currency">The ISO 4217 code of the currency used for this charge detail record.</param>
        /// <param name="ChargingPeriods">The enumeration of charging periods that make up this charging session. A session consist of 1 or more periodes with, each period has a different relevant charging tariff.</param>
        /// <param name="TotalCost">The total cost (excluding VAT) of this transaction.</param>
        /// <param name="TotalEnergy">The total energy charged in kWh.</param>
        /// <param name="TotalTime">The total duration of the charging session, including the duration of charging and not charging.</param>
        /// 
        /// <param name="MeterId">The optional identification of the energy meter.</param>
        /// <param name="EnergyMeter">The optional energy meter.</param>
        /// <param name="TransparencySoftwares">The enumeration of valid transparency softwares which can be used to validate the singed charging session and metering data.</param>
        /// <param name="Tariffs">The enumeration of relevant charging tariffs.</param>
        /// <param name="SignedData">The optional signed metering data that belongs to this charging session.</param>
        /// <param name="TotalParkingTime">The optional total duration of the charging session where the EV was not charging (no energy was transferred between EVSE and EV).</param>
        /// <param name="Remark">The optional remark can be used to provide addition human readable information to the charge detail record, for example a reason why a transaction was stopped.</param>
        /// 
        /// <param name="LastUpdated">The timestamp when this charge detail record was last updated (or created).</param>
        /// <param name="CustomCDRSerializer">A delegate to serialize custom charge detail record JSON objects.</param>
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomSignedDataSerializer">A delegate to serialize custom signed data JSON objects.</param>
        /// <param name="CustomSignedValueSerializer">A delegate to serialize custom signed value JSON objects.</param>
        public CDR(CountryCode                                                   CountryCode,
                   Party_Id                                                      PartyId,
                   CDR_Id                                                        Id,
                   DateTime                                                      Start,
                   DateTime                                                      End,
                   Auth_Id                                                       AuthId,
                   AuthMethods                                                   AuthMethod,
                   Location                                                      Location,
                   Currency                                                      Currency,
                   IEnumerable<ChargingPeriod>                                   ChargingPeriods,
                   Decimal                                                       TotalCost,
                   Decimal                                                       TotalEnergy,
                   TimeSpan                                                      TotalTime,

                   Meter_Id?                                                     MeterId                                      = null,
                   EnergyMeter?                                                  EnergyMeter                                  = null,
                   IEnumerable<TransparencySoftware>?                            TransparencySoftwares                        = null,
                   IEnumerable<Tariff>?                                          Tariffs                                      = null,
                   SignedData?                                                   SignedData                                   = null,
                   TimeSpan?                                                     TotalParkingTime                             = null,
                   String?                                                       Remark                                       = null,

                   DateTime?                                                     LastUpdated                                  = null,
                   CustomJObjectSerializerDelegate<CDR>?                         CustomCDRSerializer                          = null,
                   CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                     = null,
                   CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer        = null,
                   CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                   CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                   CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                   CustomJObjectSerializerDelegate<EnergyMeter>?                 CustomEnergyMeterSerializer                  = null,
                   CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                   CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                   CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                   CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer              = null,
                   CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                        = null,
                   CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null,
                   CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                    = null,
                   CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                 = null,
                   CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer          = null,
                   CustomJObjectSerializerDelegate<Tariff>?                      CustomTariffSerializer                       = null,
                   CustomJObjectSerializerDelegate<TariffElement>?               CustomTariffElementSerializer                = null,
                   CustomJObjectSerializerDelegate<PriceComponent>?              CustomPriceComponentSerializer               = null,
                   CustomJObjectSerializerDelegate<TariffRestrictions>?          CustomTariffRestrictionsSerializer           = null,
                   CustomJObjectSerializerDelegate<ChargingPeriod>?              CustomChargingPeriodSerializer               = null,
                   CustomJObjectSerializerDelegate<CDRDimension>?                CustomCDRDimensionSerializer                 = null,
                   CustomJObjectSerializerDelegate<SignedData>?                  CustomSignedDataSerializer                   = null,
                   CustomJObjectSerializerDelegate<SignedValue>?                 CustomSignedValueSerializer                  = null)

        {

            if (!ChargingPeriods.Any())
                throw new ArgumentNullException(nameof(ChargingPeriods),  "The given enumeration of charging periods must not be null or empty!");

            this.CountryCode              = CountryCode;
            this.PartyId                  = PartyId;
            this.Id                       = Id;
            this.Start                    = Start;
            this.End                      = End;
            this.AuthId                   = AuthId;
            this.AuthMethod               = AuthMethod;
            this.Location                 = Location;
            this.Currency                 = Currency;
            this.ChargingPeriods          = ChargingPeriods       ?? Array.Empty<ChargingPeriod>();
            this.TotalCost                = TotalCost;
            this.TotalEnergy              = TotalEnergy;
            this.TotalTime                = TotalTime;

            this.MeterId                  = MeterId;
            this.EnergyMeter              = EnergyMeter;
            this.TransparencySoftwares    = TransparencySoftwares ?? Array.Empty<TransparencySoftware>();
            this.Tariffs                  = Tariffs               ?? Array.Empty<Tariff>();
            this.SignedData               = SignedData;
            this.TotalParkingTime         = TotalParkingTime;
            this.Remark                   = Remark;

            this.LastUpdated              = LastUpdated           ?? Timestamp.Now;

            this.ETag                     = SHA256.Create().ComputeHash(ToJSON(true,
                                                                               CustomCDRSerializer,
                                                                               CustomLocationSerializer,
                                                                               CustomAdditionalGeoLocationSerializer,
                                                                               CustomEVSESerializer,
                                                                               CustomStatusScheduleSerializer,
                                                                               CustomConnectorSerializer,
                                                                               CustomEnergyMeterSerializer,
                                                                               CustomTransparencySoftwareStatusSerializer,
                                                                               CustomTransparencySoftwareSerializer,
                                                                               CustomDisplayTextSerializer,
                                                                               CustomBusinessDetailsSerializer,
                                                                               CustomHoursSerializer,
                                                                               CustomImageSerializer,
                                                                               CustomEnergyMixSerializer,
                                                                               CustomEnergySourceSerializer,
                                                                               CustomEnvironmentalImpactSerializer,
                                                                               CustomTariffSerializer,
                                                                               CustomTariffElementSerializer,
                                                                               CustomPriceComponentSerializer,
                                                                               CustomTariffRestrictionsSerializer,
                                                                               CustomChargingPeriodSerializer,
                                                                               CustomCDRDimensionSerializer,
                                                                               CustomSignedDataSerializer,
                                                                               CustomSignedValueSerializer).ToUTF8Bytes()).ToBase64();

        }

        #endregion


        #region (static) Parse   (JSON, CountryCodeURL = null, PartyIdURL = null, CDRIdURL = null, CustomCDRParser = null)

        /// <summary>
        /// Parse the given JSON representation of a CDR.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="CDRIdURL">An optional charge detail record identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomCDRParser">A delegate to parse custom charge detail record JSON objects.</param>
        public static CDR Parse(JObject                            JSON,
                                CountryCode?                       CountryCodeURL    = null,
                                Party_Id?                          PartyIdURL        = null,
                                CDR_Id?                            CDRIdURL          = null,
                                CustomJObjectParserDelegate<CDR>?  CustomCDRParser   = null)
        {

            if (TryParse(JSON,
                         out var CDR,
                         out var errorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         CDRIdURL,
                         CustomCDRParser))
            {
                return CDR!;
            }

            throw new ArgumentException("The given JSON representation of a charge detail record is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CDR, out ErrorResponse, CDRIdURL = null, CustomCDRParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a CDR.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDR">The parsed CDR.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject      JSON,
                                       out CDR?     CDR,
                                       out String?  ErrorResponse)

            => TryParse(JSON,
                        out CDR,
                        out ErrorResponse,
                        null,
                        null,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a CDR.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDR">The parsed CDR.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="CDRIdURL">An optional CDR identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomCDRParser">A delegate to parse custom CDR JSON objects.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       out CDR?                           CDR,
                                       out String?                        ErrorResponse,
                                       CountryCode?                       CountryCodeURL    = null,
                                       Party_Id?                          PartyIdURL        = null,
                                       CDR_Id?                            CDRIdURL          = null,
                                       CustomJObjectParserDelegate<CDR>?  CustomCDRParser   = null)
        {

            try
            {

                CDR = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CountryCode               [optional, internal]

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

                #region Parse PartyIdURL                [optional, internal]

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
                                       "CDR identification",
                                       CDR_Id.TryParse,
                                       out CDR_Id? CDRIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!CDRIdURL.HasValue && !CDRIdBody.HasValue)
                {
                    ErrorResponse = "The CDR identification is missing!";
                    return false;
                }

                if (CDRIdURL.HasValue && CDRIdBody.HasValue && CDRIdURL.Value != CDRIdBody.Value)
                {
                    ErrorResponse = "The optional CDR identification given within the JSON body does not match the one given in the URL!";
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

                #region Parse End                       [mandatory]

                if (!JSON.ParseMandatory("end_date_time",
                                         "end timestamp",
                                         out DateTime End,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthId                    [mandatory]

                if (!JSON.ParseMandatory("auth_id",
                                         "authentication identification",
                                         Auth_Id.TryParse,
                                         out Auth_Id AuthId,
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

                #region Parse Location                  [mandatory]

                if (!JSON.ParseMandatoryJSON("location",
                                             "charge detail record location",
                                             OCPIv2_1_1.Location.TryParse,
                                             out Location? Location,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (Location is null)
                    return false;

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

                #region Parse SignedData                [optional]

                if (JSON.ParseOptionalJSON("signed_data",
                                           "signed data",
                                           OCPIv2_1_1.SignedData.TryParse,
                                           out SignedData SignedData,
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

                #region Parse Tariffs                   [optional]

                if (JSON.ParseOptionalJSON("tariffs",
                                           "tariffs",
                                           Tariff.TryParse,
                                           out IEnumerable<Tariff> Tariffs,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ChargingPeriods           [mandatory]

                if (!JSON.ParseMandatoryJSON("charging_periods",
                                             "charging periods",
                                             ChargingPeriod.TryParse,
                                             out IEnumerable<ChargingPeriod> ChargingPeriods,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TotalCosts                [mandatory]

                if (!JSON.ParseMandatory("total_cost",
                                         "total costs",
                                         out Decimal TotalCosts,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TotalEnergy               [mandatory]

                if (!JSON.ParseMandatory("total_energy",
                                         "total energy",
                                         out Decimal TotalEnergy,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TotalTime                 [mandatory]

                if (!JSON.ParseMandatory("total_time",
                                         "total time",
                                         out Double totalTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                var TotalTime = TimeSpan.FromHours(totalTime);

                #endregion

                #region Parse TotalParkingTime          [optional]

                if (JSON.ParseOptional("total_parking_time",
                                       "total parking time",
                                       out Double? totalParkingTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var TotalParkingTime = totalParkingTime.HasValue
                                           ? new TimeSpan?(TimeSpan.FromHours(totalParkingTime.Value))
                                           : null;

                #endregion

                #region Remark                          [optional]

                var Remark = JSON.GetString("remark");

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


                CDR = new CDR(CountryCodeBody ?? CountryCodeURL!.Value,
                              PartyIdBody     ?? PartyIdURL!.    Value,
                              CDRIdBody       ?? CDRIdURL!.      Value,
                              Start,
                              End,
                              AuthId,
                              AuthMethod,
                              Location,
                              Currency,
                              ChargingPeriods,
                              TotalCosts,
                              TotalEnergy,
                              TotalTime,

                              MeterId,
                              EnergyMeter,
                              TransparencySoftwares,
                              Tariffs,
                              SignedData,
                              TotalParkingTime,
                              Remark,

                              LastUpdated);


                if (CustomCDRParser is not null)
                    CDR = CustomCDRParser(JSON,
                                          CDR);

                return true;

            }
            catch (Exception e)
            {
                CDR            = default;
                ErrorResponse  = "The given JSON representation of a CDR is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCDRSerializer = null, CustomEnergyMeterSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCDRSerializer">A delegate to serialize custom charge detail record JSON objects.</param>
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomSignedDataSerializer">A delegate to serialize custom signed data JSON objects.</param>
        /// <param name="CustomSignedValueSerializer">A delegate to serialize custom signed value JSON objects.</param>
        public JObject ToJSON(Boolean                                                       IncludeOwnerInformation                      = false,
                              CustomJObjectSerializerDelegate<CDR>?                         CustomCDRSerializer                          = null,
                              CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                     = null,
                              CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer        = null,
                              CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                              CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                              CustomJObjectSerializerDelegate<EnergyMeter>?                 CustomEnergyMeterSerializer                  = null,
                              CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                              CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                              CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer              = null,
                              CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                        = null,
                              CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                    = null,
                              CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                 = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer          = null,
                              CustomJObjectSerializerDelegate<Tariff>?                      CustomTariffSerializer                       = null,
                              CustomJObjectSerializerDelegate<TariffElement>?               CustomTariffElementSerializer                = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?              CustomPriceComponentSerializer               = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?          CustomTariffRestrictionsSerializer           = null,
                              CustomJObjectSerializerDelegate<ChargingPeriod>?              CustomChargingPeriodSerializer               = null,
                              CustomJObjectSerializerDelegate<CDRDimension>?                CustomCDRDimensionSerializer                 = null,
                              CustomJObjectSerializerDelegate<SignedData>?                  CustomSignedDataSerializer                   = null,
                              CustomJObjectSerializerDelegate<SignedValue>?                 CustomSignedValueSerializer                  = null)
        {

            var JSON = JSONObject.Create(

                           IncludeOwnerInformation
                               ? new JProperty("country_code",              CountryCode.                 ToString())
                               : null,

                           IncludeOwnerInformation
                               ? new JProperty("party_id",                  PartyId.                     ToString())
                               : null,

                                 new JProperty("id",                        Id.                          ToString()),
                                 new JProperty("start_date_time",           Start.                       ToIso8601()),
                                 new JProperty("end_date_time",             End.                         ToIso8601()),
                                 new JProperty("auth_id",                   AuthId.                      ToString()),
                                 new JProperty("auth_method",               AuthMethod.                  ToString()),
                                 new JProperty("location",                  Location.                    ToJSON(false,
                                                                                                                CustomLocationSerializer,
                                                                                                                CustomAdditionalGeoLocationSerializer,
                                                                                                                CustomEVSESerializer,
                                                                                                                CustomStatusScheduleSerializer,
                                                                                                                CustomConnectorSerializer,
                                                                                                                CustomEnergyMeterSerializer,
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
                               ? new JProperty("meter_id",                  MeterId.               Value.ToString())
                               : null,

                           EnergyMeter is not null
                               ? new JProperty("energy_meter",              EnergyMeter.                 ToJSON(CustomEnergyMeterSerializer))
                               : null,

                           SignedData is not null
                               ? new JProperty("signed_data",               SignedData.                  ToJSON(CustomSignedDataSerializer,
                                                                                                                CustomSignedValueSerializer))
                               : null,

                           TransparencySoftwares.Any()
                               ? new JProperty("transparency_softwares",    new JArray(TransparencySoftwares.Select(transparencySoftware => transparencySoftware.ToJSON(CustomTransparencySoftwareSerializer))))
                               : null,

                                 new JProperty("currency",                  Currency.                    ToString()),

                           Tariffs.Any()
                               ? new JProperty("tariffs",                   new JArray(Tariffs.              Select(tariff               => tariff.              ToJSON(false,
                                                                                                                                                                        CustomTariffSerializer,
                                                                                                                                                                        CustomDisplayTextSerializer,
                                                                                                                                                                        CustomTariffElementSerializer,
                                                                                                                                                                        CustomPriceComponentSerializer,
                                                                                                                                                                        CustomTariffRestrictionsSerializer,
                                                                                                                                                                        CustomEnergyMixSerializer,
                                                                                                                                                                        CustomEnergySourceSerializer,
                                                                                                                                                                        CustomEnvironmentalImpactSerializer))))
                               : null,

                           ChargingPeriods.Any()
                               ? new JProperty("charging_periods",          new JArray(ChargingPeriods.      Select(chargingPeriod       => chargingPeriod.      ToJSON(CustomChargingPeriodSerializer,
                                                                                                                                                                        CustomCDRDimensionSerializer))))
                               : null,

                                 new JProperty("total_cost",                TotalCost),
                                 new JProperty("total_energy",              TotalEnergy),
                                 new JProperty("total_time",                TotalTime.                   TotalHours),

                           TotalParkingTime.HasValue
                               ? new JProperty("total_parking_time",        TotalParkingTime.      Value.TotalHours)
                               : null,

                           Remark.IsNotNullOrEmpty()
                               ? new JProperty("remark",                    Remark)
                               : null,

                                 new JProperty("last_updated",              LastUpdated.                 ToIso8601())

                       );

            return CustomCDRSerializer is not null
                       ? CustomCDRSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDR CDR1,
                                           CDR CDR2)
        {

            if (Object.ReferenceEquals(CDR1, CDR2))
                return true;

            if (CDR1 is null || CDR2 is null)
                return false;

            return CDR1.Equals(CDR2);

        }

        #endregion

        #region Operator != (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDR CDR1,
                                           CDR CDR2)

            => !(CDR1 == CDR2);

        #endregion

        #region Operator <  (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDR CDR1,
                                          CDR CDR2)

            => CDR1 is null
                   ? throw new ArgumentNullException(nameof(CDR1), "The given charge detail record must not be null!")
                   : CDR1.CompareTo(CDR2) < 0;

        #endregion

        #region Operator <= (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDR CDR1,
                                           CDR CDR2)

            => !(CDR1 > CDR2);

        #endregion

        #region Operator >  (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDR CDR1,
                                          CDR CDR2)

            => CDR1 is null
                   ? throw new ArgumentNullException(nameof(CDR1), "The given charge detail record must not be null!")
                   : CDR1.CompareTo(CDR2) > 0;

        #endregion

        #region Operator >= (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDR CDR1,
                                           CDR CDR2)

            => !(CDR1 < CDR2);

        #endregion

        #endregion

        #region IComparable<CDR> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two charge detail records.
        /// </summary>
        /// <param name="Object">A charge detail record to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CDR cdr
                   ? CompareTo(cdr)
                   : throw new ArgumentException("The given object is not a charge detail record!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDR)

        /// <summary>s
        /// Compares two charge detail records.
        /// </summary>
        /// <param name="CDR">A charge detail record to compare with.</param>
        public Int32 CompareTo(CDR? CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            var c = CountryCode.CompareTo(CDR.CountryCode);

            if (c == 0)
                c = PartyId.    CompareTo(CDR.PartyId);

            if (c == 0)
                c = Id.         CompareTo(CDR.Id);

            if (c == 0)
                c = Start.      CompareTo(CDR.Start);

            if (c == 0)
                c = End.        CompareTo(CDR.End);

            if (c == 0)
                c = AuthId.     CompareTo(CDR.AuthId);

            if (c == 0)
                c = AuthMethod. CompareTo(CDR.AuthMethod);

            if (c == 0)
                c = Currency.   CompareTo(CDR.Currency);

            if (c == 0)
                c = TotalCost.  CompareTo(CDR.TotalCost);

            if (c == 0)
                c = TotalEnergy.CompareTo(CDR.TotalEnergy);

            if (c == 0)
                c = TotalTime.  CompareTo(CDR.TotalTime);

            if (c == 0)
                c = LastUpdated.CompareTo(CDR.LastUpdated);

            // Location,
            // ChargingPeriods,
            // 
            // MeterId                   
            // EnergyMeter               
            // TransparencySoftwares     
            // Tariffs                   
            // SignedData                
            // TotalParkingTime          
            // Remark                    

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<CDR> Members

        #region Equals(Object)

        /// <summary>s
        /// Compares two charge detail records for equality.
        /// </summary>
        /// <param name="Object">A charge detail record to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CDR cdr &&
                   Equals(cdr);

        #endregion

        #region Equals(CDR)

        /// <summary>s
        /// Compares two charge detail records for equality.
        /// </summary>
        /// <param name="CDR">A charge detail record to compare with.</param>
        public Boolean Equals(CDR? CDR)

            => CDR is not null &&

               CountryCode.            Equals(CDR.CountryCode)             &&
               PartyId.                Equals(CDR.PartyId)                 &&
               Id.                     Equals(CDR.Id)                      &&
               Start.                  Equals(CDR.Start)                   &&
               End.                    Equals(CDR.End)                     &&
               AuthId.                 Equals(CDR.AuthId)                  &&
               AuthMethod.             Equals(CDR.AuthMethod)              &&
               Location.               Equals(CDR.Location)                &&
               Currency.               Equals(CDR.Currency)                &&
               TotalCost.              Equals(CDR.TotalCost)               &&
               TotalEnergy.            Equals(CDR.TotalEnergy)             &&
               TotalTime.              Equals(CDR.TotalTime)               &&
               LastUpdated.ToIso8601().Equals(CDR.LastUpdated.ToIso8601()) &&

            ((!MeterId.              HasValue    && !CDR.MeterId.              HasValue)    ||
              (MeterId.              HasValue    &&  CDR.MeterId.              HasValue    && MeterId.        Value.Equals(CDR.MeterId.        Value)))  &&

             ((EnergyMeter           is     null &&  CDR.EnergyMeter           is     null) ||
              (EnergyMeter           is not null &&  CDR.EnergyMeter           is not null && EnergyMeter.          Equals(CDR.EnergyMeter)))            &&

             ((TransparencySoftwares is     null &&  CDR.TransparencySoftwares is     null) ||
              (TransparencySoftwares is not null &&  CDR.TransparencySoftwares is not null && TransparencySoftwares.Equals(CDR.TransparencySoftwares)))  &&

             ((SignedData            is     null &&  CDR.SignedData            is     null) ||
              (SignedData            is not null &&  CDR.SignedData            is not null && SignedData.           Equals(CDR.SignedData)))             &&

            ((!TotalParkingTime.     HasValue    && !CDR.TotalParkingTime.     HasValue)    ||
              (TotalParkingTime.     HasValue    &&  CDR.TotalParkingTime.     HasValue && TotalParkingTime.Value.  Equals(CDR.TotalParkingTime.Value))) &&

             ((Remark                is     null &&  CDR.Remark                is     null) ||
              (Remark                is not null &&  CDR.Remark                is not null && Remark.               Equals(CDR.Remark)))                 &&

               ChargingPeriods.Count().Equals(CDR.ChargingPeriods.Count()) &&
               ChargingPeriods.Count().Equals(CDR.ChargingPeriods.Count()) &&

               Tariffs.All(data => CDR.Tariffs.Contains(data)) &&
               Tariffs.All(data => CDR.Tariffs.Contains(data));

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

                return CountryCode.            GetHashCode()        * 73 ^
                       PartyId.                GetHashCode()        * 71 ^
                       Id.                     GetHashCode()        * 67 ^
                       Start.                  GetHashCode()        * 61 ^
                       End.                    GetHashCode()        * 59 ^
                       AuthId.                 GetHashCode()        * 53 ^
                       AuthMethod.             GetHashCode()        * 47 ^
                       Location.               GetHashCode()        * 43 ^
                       Currency.               GetHashCode()        * 41 ^
                       ChargingPeriods.        CalcHashCode()       * 37 ^
                       Tariffs.                CalcHashCode()       * 31 ^
                       TotalCost.              GetHashCode()        * 29 ^
                       TotalEnergy.            GetHashCode()        * 23 ^
                       TotalTime.              GetHashCode()        * 19 ^
                       LastUpdated.            GetHashCode()        * 17 ^

                       (MeterId?.              GetHashCode()  ?? 0) * 13 ^
                       (EnergyMeter?.          GetHashCode()  ?? 0) * 11 ^
                       (TransparencySoftwares?.CalcHashCode() ?? 0) *  7 ^
                       (SignedData?.           GetHashCode()  ?? 0) *  5 ^
                       (TotalParkingTime?.     GetHashCode()  ?? 0) *  3 ^
                        Remark?.               GetHashCode()  ?? 0;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,                       " (",
                   CountryCode,              "-",
                   PartyId,                  ") ",
                   Start.      ToIso8601(),  ", ",
                   End.        ToIso8601(),  ", ",
                   AuthId.     ToString(),   ", ",
                   AuthMethod. ToString(),   ", ",
                   Location.   ToString(),   ", ",

                   TotalCost.  ToString(),   " ",
                   Currency.   ToString(),   ", ",
                   TotalEnergy.ToString(),   " kWh, ",
                   TotalTime.  ToString(),   " h, ",

                   TotalParkingTime.HasValue
                       ? TotalParkingTime.ToString() + " h parking, "
                       : "",

                   ChargingPeriods.Count(), " charging period(s), ",

                   Tariffs.Any()
                       ? Tariffs.Count() + " tariff(s), "
                       : "",

                   MeterId.HasValue
                       ? "meter id: " + MeterId.Value.ToString() + ", "
                       : "",

                   Remark is not null
                       ? "remark: " + Remark + ", "
                       : "",

                   LastUpdated.ToIso8601()

                   // EnergyMeter
                   // TransparencySoftwares
                   // SignedData

               );

        #endregion

    }

}
