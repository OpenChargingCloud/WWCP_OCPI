/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
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

        #region Data

        private readonly Object patchLock = new();

        #endregion

        #region Properties

        /// <summary>
        /// The parent CommonAPI of this charging location.
        /// </summary>
        internal CommonAPI?                          CommonAPI                   { get; set; }

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this charge detail record.
        /// </summary>
        [Mandatory]
        public   CountryCode                         CountryCode                 { get; }

        /// <summary>
        /// The identification of the charge point operator that 'owns' this charge detail record
        /// (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public   Party_Id                            PartyId                     { get; }

        /// <summary>
        /// The identification of the charge detail record within the charge point operator's platform
        /// (and suboperator platforms).
        /// CiString(39)
        /// </summary>
        [Mandatory]
        public   CDR_Id                              Id                          { get; }

        /// <summary>
        /// The start timestamp of the charging session, or in-case of a reservation
        /// (before the start of a session) the start of the reservation.
        /// </summary>
        [Mandatory]
        public   DateTime                            Start                       { get; }

        /// <summary>
        /// The timestamp when the session was completed/finished.
        /// Charging might have finished before the session ends, for example:
        /// EV is full, but parking cost also has to be paid.
        /// </summary>
        [Mandatory]
        public   DateTime                            End                         { get; }

        /// <summary>
        /// The optional unique identification of the charging session.
        /// Is only allowed to be omitted when the CPO has not implemented the sessions module or this
        /// charge detail record is the result of a reservation that never became a charging session.
        /// </summary>
        [Optional]
        public   Session_Id?                         SessionId                   { get; }

        /// <summary>
        /// The token used to start this charging session, includes all relevant information
        /// to identify the unique token.
        /// </summary>
        [Mandatory]
        public   CDRToken                            CDRToken                    { get; }

        /// <summary>
        /// The authentication method used.
        /// </summary>
        [Mandatory]
        public   AuthMethods                         AuthMethod                  { get; }

        /// <summary>
        /// The optional reference to the authorization given by the eMSP.
        /// When the eMSP provided an authorization_reference in either:
        /// real-time authorization or StartSession, this field SHALL contain the same value.
        /// When different authorization_reference values have been given by the eMSP that
        /// are relevant to this session, the last given value SHALL be used here.
        /// </summary>
        [Optional]
        public   AuthorizationReference?             AuthorizationReference      { get; }

        /// <summary>
        /// The location where the charging session took place, including only the relevant
        /// EVSE and connector.
        /// </summary>
        [Mandatory]
        public   CDRLocation                         Location                    { get; }

        /// <summary>
        /// The optional identification of the energy meter.
        /// </summary>
        [Optional]
        public   Meter_Id?                           MeterId                     { get; }

        /// <summary>
        /// The optional energy meter.
        /// </summary>
        [Optional, NonStandard]
        public   EnergyMeter?                        EnergyMeter                 { get; }

        /// <summary>
        /// The enumeration of valid transparency softwares which can be used to validate
        /// the singed charging session and metering data.
        /// </summary>
        [Optional, NonStandard]
        public   IEnumerable<TransparencySoftware>   TransparencySoftwares        { get; }

        /// <summary>
        /// The ISO 4217 code of the currency used for this charge detail record.
        /// </summary>
        [Mandatory]
        public   OCPI.Currency                       Currency                    { get; }

        /// <summary>
        /// The enumeration of relevant charging tariffs.
        /// </summary>
        [Optional]
        public   IEnumerable<Tariff>                 Tariffs                     { get; }

        /// <summary>
        /// The enumeration of charging periods that make up this charging session.
        /// A session consist of 1 or more periodes with, each period has a
        /// different relevant charging tariff.
        /// </summary>
        [Mandatory]
        public   IEnumerable<ChargingPeriod>         ChargingPeriods             { get; }

        /// <summary>
        /// The optional signed metering data that belongs to this charging session.
        /// </summary>
        [Optional]
        public   SignedData?                         SignedData                  { get; }

        /// <summary>
        /// The total sum of all the costs of this transaction in the specified currency.
        /// </summary>
        [Mandatory]
        public   Price                               TotalCosts                  { get; }

        /// <summary>
        /// The optional total sum of all the costs of this transaction in the specified currency.
        /// </summary>
        [Optional]
        public   Price?                              TotalFixedCosts             { get; }

        /// <summary>
        /// The total energy charged in kWh.
        /// </summary>
        [Mandatory]
        public   Decimal                             TotalEnergy                 { get; }

        /// <summary>
        /// The optional total sum of all the cost of all the energy used, in the specified currency.
        /// </summary>
        [Optional]
        public   Price?                              TotalEnergyCost             { get; }

        /// <summary>
        /// The total duration of the charging session, including the duration of charging and not charging.
        /// </summary>
        [Mandatory]
        public   TimeSpan                            TotalTime                   { get; }

        /// <summary>
        /// The optional total sum of all the cost related to duration of charging during this transaction,
        /// in the specified currency.
        /// </summary>
        [Optional]
        public   Price?                              TotalTimeCost               { get; }

        /// <summary>
        /// The optional total duration of the charging session where the EV was not charging
        /// (no energy was transferred between EVSE and EV).
        /// </summary>
        [Optional]
        public   TimeSpan?                           TotalParkingTime            { get; }

        /// <summary>
        /// The optional total duration of the charging session where the EV was not charging
        /// (no energy was transferred between EVSE and EV).
        /// </summary>
        [Optional]
        public   TimeSpan                            TotalChargingTime
            => TotalTime - (TotalParkingTime ?? TimeSpan.Zero);

        /// <summary>
        /// The optional total sum of all the cost related to parking of this transaction,
        /// including fixed price components, in the specified currency.
        /// </summary>
        [Optional]
        public   Price?                              TotalParkingCost            { get; }

        /// <summary>
        /// The optional total sum of all the cost related to a reservation of a charge point,
        /// including fixed price components, in the specified currency.
        /// </summary>
        [Optional]
        public   Price?                              TotalReservationCost        { get; }

        /// <summary>
        /// The optional remark can be used to provide addition human
        /// readable information to the charge detail record, for example a
        /// reason why a transaction was stopped.
        /// </summary>
        [Optional]
        public   String?                             Remark                      { get; }

        /// <summary>
        /// The optional invoice reference identification can be used to reference an invoice,
        /// that will later be send for this charge detail record.
        /// Might even group multiple charge detail records that will be on the same invoice.
        /// </summary>
        [Optional]
        public   InvoiceReference_Id?                InvoiceReferenceId          { get; }

        /// <summary>
        /// The optional indication, that this charge detail record is a "credit CDR".
        /// When set to true the field credit_reference_id needs to be set as well.
        /// </summary>
        [Optional]
        public   Boolean?                            Credit                      { get; }

        /// <summary>
        /// The optional credit reference identification is required to be set for a "credit CDR".
        /// This SHALL contain the identification of the charge detail record for which this is a "credit CDR".
        /// </summary>
        [Optional]
        public   CreditReference_Id?                 CreditReferenceId           { get; }

        /// <summary>
        /// When set to true, this charge detail record is for a charging session using the home charger
        /// of the EV driver for which the energy cost needs to be financial compensated to the EV driver.
        /// </summary>
        [Optional]
        public   Boolean?                            HomeChargingCompensation    { get; }

        /// <summary>
        /// The timestamp when this charge detail record was created.
        /// </summary>
        [Mandatory, NonStandard("Pagination")]
        public   DateTime                            Created                     { get; }

        /// <summary>
        /// The timestamp when this charge detail record was last updated (or created).
        /// </summary>
        [Mandatory]
        public   DateTime                            LastUpdated                 { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this charge detail record.
        /// </summary>
        public   String                              ETag                        { get; }

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
        /// <param name="CDRToken">The token used to start this charging session, includes all relevant information to identify the unique token.</param>
        /// <param name="AuthMethod">The authentication method used.</param>
        /// <param name="Location">The location where the charging session took place, including only the relevant EVSE and connector.</param>
        /// <param name="Currency">The ISO 4217 code of the currency used for this charge detail record.</param>
        /// <param name="ChargingPeriods">The enumeration of charging periods that make up this charging session. A session consist of 1 or more periodes with, each period has a different relevant charging tariff.</param>
        /// <param name="TotalCosts">The total sum of all the costs of this transaction in the specified currency.</param>
        /// <param name="TotalEnergy">The total energy charged in kWh.</param>
        /// <param name="TotalTime">The total duration of the charging session, including the duration of charging and not charging.</param>
        /// 
        /// <param name="SessionId">The optional unique identification of the charging session. Is only allowed to be omitted when the CPO has not implemented the sessions module or this charge detail record is the result of a reservation that never became a charging session.</param>
        /// <param name="AuthorizationReference">The optional reference to the authorization given by the eMSP.</param>
        /// <param name="MeterId">The optional identification of the energy meter.</param>
        /// <param name="EnergyMeter">The optional energy meter.</param>
        /// <param name="TransparencySoftwares">The enumeration of valid transparency softwares which can be used to validate the singed charging session and metering data.</param>
        /// <param name="Tariffs">The enumeration of relevant charging tariffs.</param>
        /// <param name="SignedData">The optional signed metering data that belongs to this charging session.</param>
        /// <param name="TotalFixedCosts">The optional total sum of all the costs of this transaction in the specified currency.</param>
        /// <param name="TotalEnergyCost">The optional total sum of all the cost of all the energy used, in the specified currency.</param>
        /// <param name="TotalTimeCost">The optional total sum of all the cost related to duration of charging during this transaction, in the specified currency.</param>
        /// <param name="TotalParkingTime">The optional total duration of the charging session where the EV was not charging (no energy was transferred between EVSE and EV).</param>
        /// <param name="TotalParkingCost">The optional total sum of all the cost related to parking of this transaction, including fixed price components, in the specified currency.</param>
        /// <param name="TotalReservationCost">The optional total sum of all the cost related to a reservation of a charge point, including fixed price components, in the specified currency.</param>
        /// <param name="Remark">The optional remark can be used to provide addition human readable information to the charge detail record, for example a reason why a transaction was stopped.</param>
        /// <param name="InvoiceReferenceId">The optional invoice reference identification can be used to reference an invoice, that will later be send for this charge detail record. Might even group multiple charge detail records that will be on the same invoice.</param>
        /// <param name="Credit">The optional indication, that this charge detail record is a "credit CDR". When set to true the field credit_reference_id needs to be set as well.</param>
        /// <param name="CreditReferenceId">The optional credit reference identification is required to be set for a "credit CDR". This SHALL contain the identification of the charge detail record for which this is a "credit CDR".</param>
        /// <param name="HomeChargingCompensation">When set to true, this charge detail record is for a charging session using the home charger of the EV driver for which the energy cost needs to be financial compensated to the EV driver.</param>
        /// 
        /// <param name="Created">An optional timestamp when this charge detail record was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this charge detail record was last updated (or created).</param>
        /// 
        /// <param name="CustomCDRSerializer">A delegate to serialize custom charge detail record JSON objects.</param>
        /// <param name="CustomCDRTokenSerializer">A delegate to serialize custom charge detail record token JSON objects.</param>
        /// <param name="CustomCDRLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomSignedDataSerializer">A delegate to serialize custom signed data JSON objects.</param>
        /// <param name="CustomSignedValueSerializer">A delegate to serialize custom signed value JSON objects.</param>
        public CDR(CountryCode                                             CountryCode,
                   Party_Id                                                PartyId,
                   CDR_Id                                                  Id,
                   DateTime                                                Start,
                   DateTime                                                End,
                   CDRToken                                                CDRToken,
                   AuthMethods                                             AuthMethod,
                   CDRLocation                                             Location,
                   OCPI.Currency                                           Currency,
                   IEnumerable<ChargingPeriod>                             ChargingPeriods,
                   Price                                                   TotalCosts,
                   Decimal                                                 TotalEnergy,
                   TimeSpan                                                TotalTime,

                   Session_Id?                                             SessionId                              = null,
                   AuthorizationReference?                                 AuthorizationReference                 = null,
                   Meter_Id?                                               MeterId                                = null,
                   EnergyMeter?                                            EnergyMeter                            = null,
                   IEnumerable<TransparencySoftware>?                      TransparencySoftwares                  = null,
                   IEnumerable<Tariff>?                                    Tariffs                                = null,
                   SignedData?                                             SignedData                             = null,
                   Price?                                                  TotalFixedCosts                        = null,
                   Price?                                                  TotalEnergyCost                        = null,
                   Price?                                                  TotalTimeCost                          = null,
                   TimeSpan?                                               TotalParkingTime                       = null,
                   Price?                                                  TotalParkingCost                       = null,
                   Price?                                                  TotalReservationCost                   = null,
                   String?                                                 Remark                                 = null,
                   InvoiceReference_Id?                                    InvoiceReferenceId                     = null,
                   Boolean?                                                Credit                                 = null,
                   CreditReference_Id?                                     CreditReferenceId                      = null,
                   Boolean?                                                HomeChargingCompensation               = null,

                   DateTime?                                               Created                                = null,
                   DateTime?                                               LastUpdated                            = null,
                   CustomJObjectSerializerDelegate<CDR>?                   CustomCDRSerializer                    = null,
                   CustomJObjectSerializerDelegate<CDRToken>?              CustomCDRTokenSerializer               = null,
                   CustomJObjectSerializerDelegate<CDRLocation>?           CustomCDRLocationSerializer            = null,
                   CustomJObjectSerializerDelegate<EnergyMeter>?           CustomEnergyMeterSerializer            = null,
                   CustomJObjectSerializerDelegate<TransparencySoftware>?  CustomTransparencySoftwareSerializer   = null,
                   CustomJObjectSerializerDelegate<Tariff>?                CustomTariffSerializer                 = null,
                   CustomJObjectSerializerDelegate<DisplayText>?           CustomDisplayTextSerializer            = null,
                   CustomJObjectSerializerDelegate<Price>?                 CustomPriceSerializer                  = null,
                   CustomJObjectSerializerDelegate<TariffElement>?         CustomTariffElementSerializer          = null,
                   CustomJObjectSerializerDelegate<PriceComponent>?        CustomPriceComponentSerializer         = null,
                   CustomJObjectSerializerDelegate<TariffRestrictions>?    CustomTariffRestrictionsSerializer     = null,
                   CustomJObjectSerializerDelegate<EnergyMix>?             CustomEnergyMixSerializer              = null,
                   CustomJObjectSerializerDelegate<EnergySource>?          CustomEnergySourceSerializer           = null,
                   CustomJObjectSerializerDelegate<EnvironmentalImpact>?   CustomEnvironmentalImpactSerializer    = null,
                   CustomJObjectSerializerDelegate<ChargingPeriod>?        CustomChargingPeriodSerializer         = null,
                   CustomJObjectSerializerDelegate<CDRDimension>?          CustomCDRDimensionSerializer           = null,
                   CustomJObjectSerializerDelegate<SignedData>?            CustomSignedDataSerializer             = null,
                   CustomJObjectSerializerDelegate<SignedValue>?           CustomSignedValueSerializer            = null)

        {

            if (!ChargingPeriods.Any())
                throw new ArgumentNullException(nameof(ChargingPeriods),  "The given enumeration of charging periods must not be null or empty!");

            this.CountryCode               = CountryCode;
            this.PartyId                   = PartyId;
            this.Id                        = Id;
            this.Start                     = Start;
            this.End                       = End;
            this.CDRToken                  = CDRToken;
            this.AuthMethod                = AuthMethod;
            this.Location                  = Location;
            this.Currency                  = Currency;
            this.ChargingPeriods           = ChargingPeriods       ?? Array.Empty<ChargingPeriod>();
            this.TotalCosts                = TotalCosts;
            this.TotalEnergy               = TotalEnergy;
            this.TotalTime                 = TotalTime;

            this.SessionId                 = SessionId;
            this.AuthorizationReference    = AuthorizationReference;
            this.MeterId                   = MeterId;
            this.EnergyMeter               = EnergyMeter;
            this.TransparencySoftwares     = TransparencySoftwares ?? Array.Empty<TransparencySoftware>();
            this.Tariffs                   = Tariffs               ?? Array.Empty<Tariff>();
            this.SignedData                = SignedData;
            this.TotalFixedCosts           = TotalFixedCosts;
            this.TotalEnergyCost           = TotalEnergyCost;
            this.TotalTimeCost             = TotalTimeCost;
            this.TotalParkingTime          = TotalParkingTime;
            this.TotalParkingCost          = TotalParkingCost;
            this.TotalReservationCost      = TotalReservationCost;
            this.Remark                    = Remark;
            this.InvoiceReferenceId        = InvoiceReferenceId;
            this.Credit                    = Credit;
            this.CreditReferenceId         = CreditReferenceId;
            this.HomeChargingCompensation  = HomeChargingCompensation;

            this.Created                   = Created               ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated               = LastUpdated           ?? Created     ?? Timestamp.Now;

            this.ETag                      = SHA256.HashData(ToJSON(CustomCDRSerializer,
                                                                                CustomCDRTokenSerializer,
                                                                                CustomCDRLocationSerializer,
                                                                                CustomEnergyMeterSerializer,
                                                                                CustomTransparencySoftwareSerializer,
                                                                                CustomTariffSerializer,
                                                                                CustomDisplayTextSerializer,
                                                                                CustomPriceSerializer,
                                                                                CustomTariffElementSerializer,
                                                                                CustomPriceComponentSerializer,
                                                                                CustomTariffRestrictionsSerializer,
                                                                                CustomEnergyMixSerializer,
                                                                                CustomEnergySourceSerializer,
                                                                                CustomEnvironmentalImpactSerializer,
                                                                                CustomChargingPeriodSerializer,
                                                                                CustomCDRDimensionSerializer,
                                                                                CustomSignedDataSerializer,
                                                                                CustomSignedValueSerializer).ToUTF8Bytes()).ToBase64();

            unchecked
            {

                hashCode = this.CountryCode.               GetHashCode()        * 137 ^
                           this.PartyId.                   GetHashCode()        * 131 ^
                           this.Id.                        GetHashCode()        * 127 ^
                           this.Start.                     GetHashCode()        * 113 ^
                           this.End.                       GetHashCode()        * 109 ^
                           this.CDRToken.                  GetHashCode()        * 107 ^
                           this.AuthMethod.                GetHashCode()        * 103 ^
                           this.Location.                  GetHashCode()        * 101 ^
                           this.Currency.                  GetHashCode()        *  97 ^
                           this.ChargingPeriods.           CalcHashCode()       *  89 ^
                           this.Tariffs.                   CalcHashCode()       *  83 ^
                           this.TotalCosts.                GetHashCode()        *  79 ^
                           this.TotalEnergy.               GetHashCode()        *  73 ^
                           this.TotalTime.                 GetHashCode()        *  71 ^
                           this.Created.                   GetHashCode()        *  67 ^
                           this.LastUpdated.               GetHashCode()        *  61 ^

                           (this.SessionId?.               GetHashCode()  ?? 0) *  59 ^
                           (this.AuthorizationReference?.  GetHashCode()  ?? 0) *  53 ^
                           (this.MeterId?.                 GetHashCode()  ?? 0) *  47 ^
                           (this.EnergyMeter?.             GetHashCode()  ?? 0) *  43 ^
                            this.TransparencySoftwares.    CalcHashCode()       *  41 ^
                           (this.SignedData?.              GetHashCode()  ?? 0) *  37 ^
                           (this.TotalFixedCosts?.         GetHashCode()  ?? 0) *  31 ^
                           (this.TotalEnergyCost?.         GetHashCode()  ?? 0) *  29 ^
                           (this.TotalTimeCost?.           GetHashCode()  ?? 0) *  23 ^
                           (this.TotalParkingTime?.        GetHashCode()  ?? 0) *  19 ^
                           (this.TotalParkingCost?.        GetHashCode()  ?? 0) *  17 ^
                           (this.TotalReservationCost?.    GetHashCode()  ?? 0) *  13 ^
                           (this.Remark?.                  GetHashCode()  ?? 0) *  11 ^
                           (this.InvoiceReferenceId?.      GetHashCode()  ?? 0) *   7 ^
                           (this.Credit?.                  GetHashCode()  ?? 0) *   5 ^
                           (this.CreditReferenceId?.       GetHashCode()  ?? 0) *   3 ^
                            this.HomeChargingCompensation?.GetHashCode()  ?? 0;

            }

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

                #region Parse CountryCode                 [optional]

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

                #region Parse PartyIdURL                  [optional]

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

                #region Parse Id                          [optional]

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

                #region Parse Start                       [mandatory]

                if (!JSON.ParseMandatory("start_date_time",
                                         "start timestamp",
                                         out DateTime Start,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse End                         [mandatory]

                if (!JSON.ParseMandatory("end_date_time",
                                         "end timestamp",
                                         out DateTime End,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse SessionId                   [optional]

                if (JSON.ParseOptional("session_id",
                                       "session identification",
                                       Session_Id.TryParse,
                                       out Session_Id? SessionId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse CDRToken                    [mandatory]

                if (!JSON.ParseMandatoryJSON("cdr_token",
                                             "charge detail record token",
                                             OCPIv2_2_1.CDRToken.TryParse,
                                             out CDRToken CDRToken,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthMethod                  [mandatory]

                if (!JSON.ParseMandatoryEnum("auth_method",
                                             "authentication method",
                                             out AuthMethods AuthMethod,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthorizationReference      [optional]

                if (JSON.ParseOptional("authorization_reference",
                                       "authorization reference",
                                       OCPIv2_2_1.AuthorizationReference.TryParse,
                                       out AuthorizationReference? AuthorizationReference,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Location                    [mandatory]

                if (!JSON.ParseMandatoryJSON("cdr_location",
                                             "charge detail record location",
                                             CDRLocation.TryParse,
                                             out CDRLocation? Location,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (Location is null)
                    return false;

                #endregion

                #region Parse MeterId                     [optional]

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

                #region Parse EnergyMeter                 [optional]

                if (JSON.ParseOptionalJSON("energy_meter",
                                           "energy meter",
                                           OCPI.EnergyMeter.TryParse,
                                           out EnergyMeter EnergyMeter,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TransparencySoftwares       [optional]

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

                #region Parse Currency                    [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         OCPI.Currency.TryParse,
                                         out OCPI.Currency Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Tariffs                     [optional]

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

                #region Parse ChargingPeriods             [mandatory]

                if (!JSON.ParseMandatoryJSON("charging_periods",
                                             "charging periods",
                                             ChargingPeriod.TryParse,
                                             out IEnumerable<ChargingPeriod> ChargingPeriods,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse SignedData                  [optional]

                if (JSON.ParseOptionalJSON("signed_data",
                                           "signed data",
                                           OCPIv2_2_1.SignedData.TryParse,
                                           out SignedData SignedData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalCosts                  [mandatory]

                if (!JSON.ParseMandatoryJSON("total_cost",
                                             "total costs",
                                             Price.TryParse,
                                             out Price TotalCosts,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TotalFixedCosts             [optional]

                if (JSON.ParseOptionalJSON("total_fixed_cost",
                                           "total fixed costs",
                                           Price.TryParse,
                                           out Price? TotalFixedCosts,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalEnergy                 [mandatory]

                if (!JSON.ParseMandatory("total_energy",
                                         "total energy",
                                         out Decimal TotalEnergy,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TotalEnergyCost             [optional]

                if (JSON.ParseOptionalJSON("total_energy_cost",
                                           "total energy cost",
                                           Price.TryParse,
                                           out Price? TotalEnergyCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalTime                   [mandatory]

                if (!JSON.ParseMandatory("total_time",
                                         "total time",
                                         out Double totalTimeHours,
                                         out ErrorResponse))
                {
                    return false;
                }

                var TotalTimeHours = TimeSpan.FromHours(totalTimeHours);

                #endregion

                #region Parse TotalTimeCost               [optional]

                if (JSON.ParseOptionalJSON("total_time_cost",
                                           "total time cost",
                                           Price.TryParse,
                                           out Price? TotalTimeCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalParkingTime            [optional]

                if (JSON.ParseOptional("total_parking_time",
                                       "total parking time",
                                       out Double? totalParkingTimeHours,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var TotalParkingTimeHours = totalParkingTimeHours.HasValue
                                                ? new TimeSpan?(TimeSpan.FromHours(totalParkingTimeHours.Value))
                                                : null;

                #endregion

                #region Parse TotalParkingCost            [optional]

                if (JSON.ParseOptionalJSON("total_parking_cost",
                                           "total parking cost",
                                           Price.TryParse,
                                           out Price? TotalParkingCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalReservationCost        [optional]

                if (JSON.ParseOptionalJSON("total_reservation_cost",
                                           "total reservation cost",
                                           Price.TryParse,
                                           out Price? TotalReservationCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Remark                            [optional]

                var Remark = JSON.GetString("remark");

                #endregion

                #region Parse InvoiceReferenceId          [optional]

                if (JSON.ParseOptional("invoice_reference_id",
                                       "invoice reference identification",
                                       InvoiceReference_Id.TryParse,
                                       out InvoiceReference_Id? InvoiceReferenceId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Credit                            [optional]

                if (JSON.ParseOptional("credit",
                                       "credit charge detail record",
                                       out Boolean? Credit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse CreditReferenceId           [optional]

                if (JSON.ParseOptional("credit_reference_id",
                                       "credit reference identification",
                                       CreditReference_Id.TryParse,
                                       out CreditReference_Id? CreditReferenceId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse HomeChargingCompensation    [optional]

                if (JSON.ParseOptional("home_charging_compensation",
                                       "home charging compensation",
                                       out Boolean? HomeChargingCompensation,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse Created                     [optional, NonStandard]

                if (!JSON.ParseOptional("created",
                                        "created",
                                        out DateTime? Created,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated                 [mandatory]

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
                              CDRToken,
                              AuthMethod,
                              Location,
                              Currency,
                              ChargingPeriods,
                              TotalCosts,
                              TotalEnergy,
                              TotalTimeHours,

                              SessionId,
                              AuthorizationReference,
                              MeterId,
                              EnergyMeter,
                              TransparencySoftwares,
                              Tariffs,
                              SignedData,
                              TotalFixedCosts,
                              TotalEnergyCost,
                              TotalTimeCost,
                              TotalParkingTimeHours,
                              TotalParkingCost,
                              TotalReservationCost,
                              Remark,
                              InvoiceReferenceId,
                              Credit,
                              CreditReferenceId,
                              HomeChargingCompensation,

                              Created,
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

        #region ToJSON(CustomCDRSerializer = null, CustomCDRTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCDRSerializer">A delegate to serialize custom charge detail record JSON objects.</param>
        /// <param name="CustomCDRTokenSerializer">A delegate to serialize custom charge detail record token JSON objects.</param>
        /// <param name="CustomCDRLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomSignedDataSerializer">A delegate to serialize custom signed data JSON objects.</param>
        /// <param name="CustomSignedValueSerializer">A delegate to serialize custom signed value JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CDR>?                   CustomCDRSerializer                    = null,
                              CustomJObjectSerializerDelegate<CDRToken>?              CustomCDRTokenSerializer               = null,
                              CustomJObjectSerializerDelegate<CDRLocation>?           CustomCDRLocationSerializer            = null,
                              CustomJObjectSerializerDelegate<EnergyMeter>?           CustomEnergyMeterSerializer            = null,
                              CustomJObjectSerializerDelegate<TransparencySoftware>?  CustomTransparencySoftwareSerializer   = null,
                              CustomJObjectSerializerDelegate<Tariff>?                CustomTariffSerializer                 = null,
                              CustomJObjectSerializerDelegate<DisplayText>?           CustomDisplayTextSerializer            = null,
                              CustomJObjectSerializerDelegate<Price>?                 CustomPriceSerializer                  = null,
                              CustomJObjectSerializerDelegate<TariffElement>?         CustomTariffElementSerializer          = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?        CustomPriceComponentSerializer         = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?    CustomTariffRestrictionsSerializer     = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?             CustomEnergyMixSerializer              = null,
                              CustomJObjectSerializerDelegate<EnergySource>?          CustomEnergySourceSerializer           = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?   CustomEnvironmentalImpactSerializer    = null,
                              CustomJObjectSerializerDelegate<ChargingPeriod>?        CustomChargingPeriodSerializer         = null,
                              CustomJObjectSerializerDelegate<CDRDimension>?          CustomCDRDimensionSerializer           = null,
                              CustomJObjectSerializerDelegate<SignedData>?            CustomSignedDataSerializer             = null,
                              CustomJObjectSerializerDelegate<SignedValue>?           CustomSignedValueSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("country_code",                 CountryCode.                   ToString()),
                                 new JProperty("party_id",                     PartyId.                       ToString()),
                                 new JProperty("id",                           Id.                            ToString()),
                                 new JProperty("start_date_time",              Start.                         ToIso8601()),
                                 new JProperty("end_date_time",                End.                           ToIso8601()),

                           SessionId.HasValue
                               ? new JProperty("session_id",                   SessionId.               Value.ToString())
                               : null,

                                 new JProperty("cdr_token",                    CDRToken.                      ToJSON(CustomCDRTokenSerializer)),
                                 new JProperty("auth_method",                  AuthMethod.                    ToString()),

                           AuthorizationReference.HasValue
                               ? new JProperty("authorization_reference",      AuthorizationReference.  Value.ToString())
                               : null,

                                 new JProperty("cdr_location",                 Location.                      ToJSON(CustomCDRLocationSerializer)),

                           MeterId.HasValue
                               ? new JProperty("meter_id",                     MeterId.                 Value.ToString())
                               : null,

                           EnergyMeter is not null
                               ? new JProperty("energy_meter",                 EnergyMeter.                   ToJSON(CustomEnergyMeterSerializer))
                               : null,

                           TransparencySoftwares.Any()
                               ? new JProperty("transparency_softwares",       new JArray(TransparencySoftwares.Select(transparencySoftware => transparencySoftware.ToJSON(CustomTransparencySoftwareSerializer))))
                               : null,

                                 new JProperty("currency",                     Currency.                      ToString()),

                           Tariffs.Any()
                               ? new JProperty("tariffs",                      new JArray(Tariffs.                Select(tariff               => tariff.              ToJSON(CustomTariffSerializer,
                                                                                                                                                                             CustomDisplayTextSerializer,
                                                                                                                                                                             CustomPriceSerializer,
                                                                                                                                                                             CustomTariffElementSerializer,
                                                                                                                                                                             CustomPriceComponentSerializer,
                                                                                                                                                                             CustomTariffRestrictionsSerializer,
                                                                                                                                                                             CustomEnergyMixSerializer,
                                                                                                                                                                             CustomEnergySourceSerializer,
                                                                                                                                                                             CustomEnvironmentalImpactSerializer))))
                               : null,

                           ChargingPeriods.Any()
                               ? new JProperty("charging_periods",             new JArray(ChargingPeriods.        Select(chargingPeriod       => chargingPeriod.      ToJSON(CustomChargingPeriodSerializer,
                                                                                                                                                                             CustomCDRDimensionSerializer))))
                               : null,

                           SignedData is not null
                               ? new JProperty("signed_data",                  SignedData.                    ToJSON(CustomSignedDataSerializer,
                                                                                                                     CustomSignedValueSerializer))
                               : null,

                                 new JProperty("total_cost",                   TotalCosts.                    ToJSON(CustomPriceSerializer)),

                           TotalFixedCosts.HasValue
                               ? new JProperty("total_fixed_cost",             TotalFixedCosts.         Value.ToJSON(CustomPriceSerializer))
                               : null,

                                 new JProperty("total_energy",                 TotalEnergy),

                           TotalEnergyCost.HasValue
                               ? new JProperty("total_energy_cost",            TotalEnergyCost.         Value.ToJSON(CustomPriceSerializer))
                               : null,

                                 new JProperty("total_time",                   TotalTime.                     TotalHours),

                           TotalTimeCost.HasValue
                               ? new JProperty("total_time_cost",              TotalTimeCost.           Value.ToJSON(CustomPriceSerializer))
                               : null,

                           TotalParkingTime.HasValue
                               ? new JProperty("total_parking_time",           TotalParkingTime.        Value.TotalHours)
                               : null,

                           TotalParkingCost.HasValue
                               ? new JProperty("total_parking_cost",           TotalParkingCost.        Value.ToJSON(CustomPriceSerializer))
                               : null,

                           TotalReservationCost.HasValue
                               ? new JProperty("total_reservation_cost",       TotalReservationCost.    Value.ToJSON(CustomPriceSerializer))
                               : null,

                           Remark.IsNotNullOrEmpty()
                               ? new JProperty("remark",                       Remark)
                               : null,

                           InvoiceReferenceId.HasValue
                               ? new JProperty("invoice_reference_id",         InvoiceReferenceId.      Value.ToString())
                               : null,

                           Credit.HasValue
                               ? new JProperty("credit",                       Credit.                  Value)
                               : null,

                           CreditReferenceId.HasValue
                               ? new JProperty("credit_reference_id",          CreditReferenceId.       Value.ToString())
                               : null,

                           HomeChargingCompensation.HasValue
                               ? new JProperty("home_charging_compensation",   HomeChargingCompensation.Value)
                               : null,

                                 new JProperty("create",                       Created.                       ToIso8601()),
                                 new JProperty("last_updated",                 LastUpdated.                   ToIso8601())

                       );

            return CustomCDRSerializer is not null
                       ? CustomCDRSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public CDR Clone()

            => new (CountryCode.Clone,
                    PartyId.    Clone,
                    Id.         Clone,
                    Start,
                    End,
                    CDRToken.   Clone(),
                    AuthMethod,
                    Location.   Clone(),
                    Currency.   Clone,
                    ChargingPeriods.      Select(chargingPeriod       => chargingPeriod.      Clone()).ToArray(),
                    TotalCosts. Clone(),
                    TotalEnergy,
                    TotalTime,

                    SessionId?.             Clone,
                    AuthorizationReference?.Clone,
                    MeterId?.               Clone,
                    EnergyMeter?.           Clone(),
                    TransparencySoftwares.Select(transparencySoftware => transparencySoftware.Clone()).ToArray(),
                    Tariffs.              Select(tariff               => tariff.              Clone()).ToArray(),
                    SignedData?.            Clone(),
                    TotalFixedCosts?.       Clone(),
                    TotalEnergyCost?.       Clone(),
                    TotalTimeCost?.         Clone(),
                    TotalParkingTime,
                    TotalParkingCost?.      Clone(),
                    TotalReservationCost?.  Clone(),
                    Remark is not null ? new String(Remark.ToCharArray()) : null,
                    InvoiceReferenceId?.    Clone,
                    Credit,
                    CreditReferenceId?.     Clone,
                    HomeChargingCompensation,

                    Created,
                    LastUpdated);

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject  JSON,
                                                     JObject  Patch)
        {

            foreach (var property in Patch)
            {

                if      (property.Key == "country_code")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'country code' of a charge detail record is not allowed!");

                else if (property.Key == "party_id")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'party identification' of a charge detail record is not allowed!");

                else if (property.Key == "id")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'identification' of a charge detail record is not allowed!");

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

        #region TryPatch(CDRPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representaion of this charge detail record.
        /// </summary>
        /// <param name="CDRPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<CDR> TryPatch(JObject  CDRPatch,
                                            Boolean  AllowDowngrades = false)
        {

            if (CDRPatch is null)
                return PatchResult<CDR>.Failed(this,
                                                  "The given charge detail record patch must not be null!");

            lock (patchLock)
            {

                if (CDRPatch["last_updated"] is null)
                    CDRPatch["last_updated"] = Timestamp.Now.ToIso8601();

                else if (AllowDowngrades == false &&
                        CDRPatch["last_updated"].Type == JTokenType.Date &&
                       (CDRPatch["last_updated"].Value<DateTime>().ToIso8601().CompareTo(LastUpdated.ToIso8601()) < 1))
                {
                    return PatchResult<CDR>.Failed(this,
                                                      "The 'lastUpdated' timestamp of the charge detail record patch must be newer then the timestamp of the existing charge detail record!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), CDRPatch);


                if (patchResult.IsFailed)
                    return PatchResult<CDR>.Failed(this,
                                                      patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out var patchedCDR,
                             out var errorResponse))
                {

                    return PatchResult<CDR>.Success(patchedCDR,
                                                       errorResponse);

                }

                else
                    return PatchResult<CDR>.Failed(this,
                                                      "Invalid JSON merge patch of a charge detail record: " + errorResponse);

            }

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
        public static Boolean operator == (CDR? CDR1,
                                           CDR? CDR2)
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
        public static Boolean operator != (CDR? CDR1,
                                           CDR? CDR2)

            => !(CDR1 == CDR2);

        #endregion

        #region Operator <  (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDR? CDR1,
                                          CDR? CDR2)

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
        public static Boolean operator <= (CDR? CDR1,
                                           CDR? CDR2)

            => !(CDR1 > CDR2);

        #endregion

        #region Operator >  (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDR? CDR1,
                                          CDR? CDR2)

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
        public static Boolean operator >= (CDR? CDR1,
                                           CDR? CDR2)

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
        /// <param name="Object">A charge detail record to compare with.</param>
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
                c = CDRToken.   CompareTo(CDR.CDRToken);

            if (c == 0)
                c = AuthMethod. CompareTo(CDR.AuthMethod);

            if (c == 0)
                c = Currency.   CompareTo(CDR.Currency);

            if (c == 0)
                c = TotalCosts. CompareTo(CDR.TotalCosts);

            if (c == 0)
                c = TotalEnergy.CompareTo(CDR.TotalEnergy);

            if (c == 0)
                c = TotalTime.  CompareTo(CDR.TotalTime);

            if (c == 0)
                c = Created.    CompareTo(CDR.Created);

            if (c == 0)
                c = LastUpdated.CompareTo(CDR.LastUpdated);

            // Location,
            // ChargingPeriods,
            // 
            // SessionId
            // AuthorizationReference
            // MeterId
            // EnergyMeter
            // TransparencySoftwares
            // Tariffs
            // SignedData
            // TotalFixedCosts
            // TotalEnergyCost
            // TotalTimeCost
            // TotalParkingTime
            // TotalParkingCost
            // TotalReservationCost
            // Remark
            // InvoiceReferenceId
            // Credit
            // CreditReferenceId
            // HomeChargingCompensation

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
               CDRToken.               Equals(CDR.CDRToken)                &&
               AuthMethod.             Equals(CDR.AuthMethod)              &&
               Location.               Equals(CDR.Location)                &&
               Currency.               Equals(CDR.Currency)                &&
               TotalCosts.             Equals(CDR.TotalCosts)              &&
               TotalEnergy.            Equals(CDR.TotalEnergy)             &&
               TotalTime.              Equals(CDR.TotalTime)               &&
               Created.    ToIso8601().Equals(CDR.Created.    ToIso8601()) &&
               LastUpdated.ToIso8601().Equals(CDR.LastUpdated.ToIso8601()) &&

            ((!SessionId.               HasValue    && !CDR.SessionId.               HasValue)  ||
              (SessionId.               HasValue    &&  CDR.SessionId.               HasValue    && SessionId.               Value.Equals(CDR.SessionId.               Value))) &&

            ((!AuthorizationReference.  HasValue    && !CDR.AuthorizationReference.  HasValue)  ||
              (AuthorizationReference.  HasValue    &&  CDR.AuthorizationReference.  HasValue    && AuthorizationReference.  Value.Equals(CDR.AuthorizationReference.  Value))) &&

            ((!MeterId.                 HasValue    && !CDR.MeterId.                 HasValue)  ||
              (MeterId.                 HasValue    &&  CDR.MeterId.                 HasValue    && MeterId.                 Value.Equals(CDR.MeterId.                 Value))) &&

            ((!TotalFixedCosts.         HasValue    && !CDR.TotalFixedCosts.         HasValue)  ||
              (TotalFixedCosts.         HasValue    &&  CDR.TotalFixedCosts.         HasValue    && TotalFixedCosts.         Value.Equals(CDR.TotalFixedCosts.         Value))) &&

            ((!TotalEnergyCost.         HasValue    && !CDR.TotalEnergyCost.         HasValue)  ||
              (TotalEnergyCost.         HasValue    &&  CDR.TotalEnergyCost.         HasValue    && TotalEnergyCost.         Value.Equals(CDR.TotalEnergyCost.         Value))) &&

            ((!TotalTimeCost.           HasValue    && !CDR.TotalTimeCost.           HasValue)  ||
              (TotalTimeCost.           HasValue    &&  CDR.TotalTimeCost.           HasValue    && TotalTimeCost.           Value.Equals(CDR.TotalTimeCost.           Value))) &&

            ((!TotalParkingTime.        HasValue    && !CDR.TotalParkingTime.        HasValue)  ||
              (TotalParkingTime.        HasValue    &&  CDR.TotalParkingTime.        HasValue    && TotalParkingTime.        Value.Equals(CDR.TotalParkingTime.        Value))) &&

            ((!TotalParkingCost.        HasValue    && !CDR.TotalParkingCost.        HasValue)  ||
              (TotalParkingCost.        HasValue    &&  CDR.TotalParkingCost.        HasValue    && TotalParkingCost.        Value.Equals(CDR.TotalParkingCost.        Value))) &&

            ((!TotalReservationCost.    HasValue    && !CDR.TotalReservationCost.    HasValue)  ||
              (TotalReservationCost.    HasValue    &&  CDR.TotalReservationCost.    HasValue    && TotalReservationCost.    Value.Equals(CDR.TotalReservationCost.    Value))) &&

            ((!InvoiceReferenceId.      HasValue    && !CDR.InvoiceReferenceId.      HasValue)  ||
              (InvoiceReferenceId.      HasValue    &&  CDR.InvoiceReferenceId.      HasValue    && InvoiceReferenceId.      Value.Equals(CDR.InvoiceReferenceId.      Value))) &&

            ((!Credit.                  HasValue    && !CDR.Credit.                  HasValue)  ||
              (Credit.                  HasValue    &&  CDR.Credit.                  HasValue    && Credit.                  Value.Equals(CDR.Credit.                  Value))) &&

            ((!CreditReferenceId.       HasValue    && !CDR.CreditReferenceId.       HasValue)  ||
              (CreditReferenceId.       HasValue    &&  CDR.CreditReferenceId.       HasValue    && CreditReferenceId.       Value.Equals(CDR.CreditReferenceId.       Value))) &&

            ((!HomeChargingCompensation.HasValue    && !CDR.HomeChargingCompensation.HasValue)    ||
              (HomeChargingCompensation.HasValue    &&  CDR.HomeChargingCompensation.HasValue    && HomeChargingCompensation.Value.Equals(CDR.HomeChargingCompensation.Value))) &&


             ((Remark                   is     null &&  CDR.Remark                   is     null) ||
              (Remark                   is not null &&  CDR.Remark                   is not null && Remark.                        Equals(CDR.Remark)))                         &&

             ((EnergyMeter              is     null &&  CDR.EnergyMeter              is     null) ||
              (EnergyMeter              is not null &&  CDR.EnergyMeter              is not null && EnergyMeter.                   Equals(CDR.EnergyMeter)))                    &&

             ((TransparencySoftwares    is     null &&  CDR.TransparencySoftwares    is     null) ||
              (TransparencySoftwares    is not null &&  CDR.TransparencySoftwares    is not null && TransparencySoftwares.         Equals(CDR.TransparencySoftwares)))          &&

             ((SignedData               is     null &&  CDR.SignedData               is     null) ||
              (SignedData               is not null &&  CDR.SignedData               is not null && SignedData.                    Equals(CDR.SignedData)))                     &&

               ChargingPeriods.Count().Equals(CDR.ChargingPeriods.Count()) &&
               ChargingPeriods.Count().Equals(CDR.ChargingPeriods.Count()) &&

               Tariffs.All(data => CDR.Tariffs.Contains(data)) &&
               Tariffs.All(data => CDR.Tariffs.Contains(data));

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

                   Id,                       " (",
                   CountryCode,              "-",
                   PartyId,                  ") ",
                   Start.      ToIso8601(),  ", ",
                   End.        ToIso8601(),  ", ",
                   CDRToken.   ToString(),   ", ",
                   AuthMethod. ToString(),   ", ",
                   Location.   ToString(),   ", ",

                   TotalCosts. ToString(),   " ",
                   Currency.   ToString(),   ", ",
                   TotalEnergy.ToString(),   " kWh, ",
                   TotalTime.  ToString(),   " h, ",

                   // TotalFixedCosts
                   // TotalEnergyCost
                   // TotalTimeCost
                   // TotalParkingTime
                   // TotalParkingCost
                   // TotalReservationCost
                   // InvoiceReferenceId
                   // Credit
                   // CreditReferenceId
                   // HomeChargingCompensation

                   TotalParkingTime.HasValue
                       ? TotalParkingTime.ToString() + " h parking, "
                       : "",

                   ChargingPeriods.Count(), " charging period(s), ",

                   Tariffs.Any()
                       ? Tariffs.Count() + " tariff(s), "
                       : "",

                   // SessionId
                   // AuthorizationReference

                   MeterId.HasValue
                       ? "meter id: " + MeterId.Value.ToString() + ", "
                       : "",

                   // EnergyMeter
                   // TransparencySoftwares
                   // SignedData

                   Remark is not null
                       ? "remark: " + Remark + ", "
                       : "",

                   LastUpdated.ToIso8601()

               );

        #endregion


    }

}
