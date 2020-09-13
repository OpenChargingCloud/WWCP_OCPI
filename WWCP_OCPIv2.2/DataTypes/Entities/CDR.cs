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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The CDR object describes the charging session and its costs,
    /// how these costs are composed, etc.
    /// </summary>
    public class CDR : IHasId<CDR_Id>,
                       IEquatable<CDR>,
                       IComparable<CDR>,
                       IComparable
    {

        #region Properties

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the CPO that 'owns' this charge detail record.
        /// </summary>
        [Optional]
        public CountryCode                  CountryCode                 { get; }

        /// <summary>
        /// The Id of the CPO that 'owns' this charge detail record (following the ISO-15118 standard).
        /// </summary>
        [Optional]
        public Party_Id                     PartyId                     { get; }

        /// <summary>
        /// The identification of the charge detail record within the CPOs platform (and suboperator platforms). 
        /// </summary>
        [Mandatory]
        public CDR_Id                       Id                          { get; }


        /// <summary>
        /// Start timestamp of the charging session, or in-case of a reservation
        /// (before the start of a session) the start of the reservation.
        /// </summary>
        [Mandatory]
        public DateTime                     Start                       { get; }

        /// <summary>
        /// The timestamp when the session was completed/finished, charging might
        /// have finished before the session ends, for example:
        /// EV is full, but parking cost also has to be paid.
        /// </summary>
        [Mandatory]
        public DateTime                     End                         { get; }

        /// <summary>
        /// Unique ID of the Session for which this CDR is sent. Is only allowed to be omitted
        /// when the CPO has not implemented the sessions module or this charge detail record
        /// is the result of a reservation that never became a charging session, thus no OCPI session.
        /// </summary>
        [Optional]
        public Session_Id?                  SessionId                   { get; }

        /// <summary>
        /// Token used to start this charging session, includes all the relevant information
        /// to identify the unique token.
        /// </summary>
        [Mandatory]
        public CDRToken                     CDRToken                    { get; }

        /// <summary>
        /// Method used for authentication.
        /// </summary>
        [Mandatory]
        public AuthMethods                  AuthMethod                  { get; }

        /// <summary>
        /// Reference to the authorization given by the eMSP. When the eMSP provided an
        /// authorization_reference in either: real-time authorization or StartSession,
        /// this field SHALL contain the same value. When different authorization_reference
        /// values have been given by the eMSP that are relevant to this Session, the last
        /// given value SHALL be used here.
        /// </summary>
        public AuthorizationReference?      AuthorizationReference      { get; }

        /// <summary>
        /// Location where the charging session took place, including only the relevant
        /// EVSE and connector.
        /// </summary>
        [Mandatory]
        public CDRLocation                  Location                    { get; }

        /// <summary>
        /// Optional identification of the kWh energy meter.
        /// </summary>
        [Optional]
        public Meter_Id?                    MeterId                     { get; }

        /// <summary>
        /// ISO 4217 code of the currency used for this CDR.
        /// </summary>
        [Mandatory]
        public Currency                     Currency                    { get; }

        /// <summary>
        /// Enumeration of relevant tariffs.
        /// </summary>
        [Optional]
        public IEnumerable<Tariff>          Tariffs                     { get; }

        /// <summary>
        /// Enumeration of charging periods that make up this charging session.
        /// A session consist of 1 or more periodes with, each period has a
        /// different relevant Tariff.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingPeriod>  ChargingPeriods             { get; }

        /// <summary>
        /// Signed data that belongs to this charging session.
        /// </summary>
        [Optional]
        public SignedData?                  SignedData                  { get; }

        /// <summary>
        /// Total sum of all the costs of this transaction in the specified currency.
        /// </summary>
        [Mandatory]
        public Price                        TotalCosts                  { get; }

        /// <summary>
        /// Total sum of all the costs of this transaction in the specified currency.
        /// </summary>
        [Optional]
        public Price?                       TotalFixedCosts             { get; }

        /// <summary>
        /// Total energy charged, in kWh.
        /// </summary>
        [Mandatory]
        public Decimal                      TotalEnergy                 { get; }

        /// <summary>
        /// Total sum of all the cost of all the energy used, in the specified currency.
        /// </summary>
        [Optional]
        public Price?                       TotalEnergyCost             { get; }


        /// <summary>
        /// Total duration of the charging session (including the duration of charging and not charging), in hours.
        /// </summary>
        [Mandatory]
        public Decimal                      TotalTime                   { get; }

        /// <summary>
        /// Total sum of all the cost related to duration of charging during this transaction, in the specified currency.
        /// </summary>
        [Optional]
        public Price?                       TotalTimeCost               { get; }


        /// <summary>
        /// Total duration of the charging session where the EV was not charging (no energy was transferred between EVSE and EV), in hours.
        /// </summary>
        [Optional]
        public Decimal?                     TotalParkingTime            { get; }

        /// <summary>
        /// Total duration of the charging session where the EV was not charging (no energy was transferred between EVSE and EV), in hours.
        /// </summary>
        [Optional]
        public Decimal                      TotalChargingTime
            => TotalTime - (TotalParkingTime ?? 0);

        /// <summary>
        /// Total sum of all the cost related to parking of this transaction, including fixed price components, in the specified currency.
        /// </summary>
        [Optional]
        public Price?                       TotalParkingCost            { get; }

        /// <summary>
        /// Total sum of all the cost related to a reservation of a Charge Point, including fixed price components, in the specified currency.
        /// </summary>
        [Optional]
        public Price?                       TotalReservationCost        { get; }

        /// <summary>
        /// Optional remark, can be used to provide addition human
        /// readable information to the charge detail record, for example:
        /// reason why a transaction was stopped.
        /// </summary>
        [Optional]
        public String                       Remark                      { get; }

        /// <summary>
        /// This field can be used to reference an invoice, that will later be send for this CDR. Making it easier to link a CDR to a given invoice. Maybe even group CDRs that will be on the same invoice.
        /// </summary>
        [Optional]
        public InvoiceReference_Id?         InvoiceReferenceId          { get; }


        /// <summary>
        /// When set to true, this is a Credit CDR, and the field credit_reference_id needs to be set as well.
        /// </summary>
        [Optional]
        public Boolean?                     Credit                      { get; }

        /// <summary>
        /// Is required to be set for a Credit CDR. This SHALL contain the id of the CDR for which this is a Credit CDR.
        /// </summary>
        [Optional]
        public CreditReference_Id?          CreditReferenceId           { get; }


        /// <summary>
        /// Timestamp when this charge detail record was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                     LastUpdated                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge detail record describing the charging session and its costs,
        /// how these costs are composed, etc.
        /// </summary>
        public CDR(CountryCode                  CountryCode,
                   Party_Id                     PartyId,
                   CDR_Id                       Id,
                   DateTime                     Start,
                   DateTime                     End,
                   CDRToken                     CDRToken,
                   AuthMethods                  AuthMethod,
                   CDRLocation                  Location,
                   Currency                     Currency,
                   IEnumerable<ChargingPeriod>  ChargingPeriods,
                   Price                        TotalCosts,
                   Decimal                      TotalEnergy,
                   Decimal                      TotalTime,

                   Session_Id?                  SessionId                = null,
                   AuthorizationReference?      AuthorizationReference   = null,
                   Meter_Id?                    MeterId                  = null,
                   IEnumerable<Tariff>          Tariffs                  = null,
                   SignedData?                  SignedData               = null,
                   Price?                       TotalFixedCosts          = null,
                   Price?                       TotalEnergyCost          = null,
                   Price?                       TotalTimeCost            = null,
                   Decimal?                     TotalParkingTime         = null,
                   Price?                       TotalParkingCost         = null,
                   Price?                       TotalReservationCost     = null,
                   String                       Remark                   = null,
                   InvoiceReference_Id?         InvoiceReferenceId       = null,
                   Boolean?                     Credit                   = null,
                   CreditReference_Id?          CreditReferenceId        = null,

                   DateTime?                    LastUpdated              = null)

        {

            #region Initial checks

            if (!ChargingPeriods.SafeAny())
                throw new ArgumentNullException(nameof(ChargingPeriods),  "The given enumeration of charging periods must not be null or empty!");

            #endregion

            this.CountryCode              = CountryCode;
            this.PartyId                  = PartyId;
            this.Id                       = Id;
            this.Start                    = Start;
            this.End                      = End;
            this.CDRToken                 = CDRToken;
            this.AuthMethod               = AuthMethod;
            this.Location                 = Location ?? throw new ArgumentNullException(nameof(Location),  "The given charging location must not be null!");
            this.Currency                 = Currency;
            this.ChargingPeriods          = ChargingPeriods;
            this.TotalCosts               = TotalCosts;
            this.TotalEnergy              = TotalEnergy;
            this.TotalTime                = TotalTime;

            this.SessionId                = SessionId;
            this.AuthorizationReference   = AuthorizationReference;
            this.MeterId                  = MeterId;
            this.Tariffs                  = Tariffs;
            this.SignedData               = SignedData;
            this.TotalFixedCosts          = TotalFixedCosts;
            this.TotalEnergyCost          = TotalEnergyCost;
            this.TotalTimeCost            = TotalTimeCost;
            this.TotalParkingTime         = TotalParkingTime;
            this.TotalParkingCost         = TotalParkingCost;
            this.TotalReservationCost     = TotalReservationCost;
            this.Remark                   = Remark;
            this.InvoiceReferenceId       = InvoiceReferenceId;
            this.Credit                   = Credit;
            this.CreditReferenceId        = CreditReferenceId;

            this.LastUpdated              = LastUpdated ?? DateTime.Now;

        }

        #endregion


        #region IComparable<CDR> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is CDR cdr
                   ? CompareTo(cdr)
                   : throw new ArgumentException("The given object is not a charge detail record!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDR)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR">An CDR to compare with.</param>
        public Int32 CompareTo(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR),  "The given CDR must not be null!");

            return Id.CompareTo(CDR.Id);

        }

        #endregion

        #endregion

        #region IEquatable<CDR> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is CDR cdr &&
                   Equals(cdr);

        #endregion

        #region Equals(CDR)

        /// <summary>
        /// Compares two CDRs for equality.
        /// </summary>
        /// <param name="CDR">An CDR to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CDR CDR)
        {

            if (CDR is null)
                return false;

            return Id.Equals(CDR.Id);

        }

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
