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
    /// The charging session.
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
                       Connector_Id             ConnectorId,
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
            this.ConnectorId              = ConnectorId;
            this.MeterId                  = MeterId;
            this.Currency                 = Currency;
            this.Status                   = Status;

            this.End                      = End;
            this.AuthorizationReference   = AuthorizationReference;
            this.TotalCosts               = TotalCosts;

            this.LastUpdated              = LastUpdated ?? DateTime.Now;

        }

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
                throw new ArgumentNullException(nameof(Session),  "The given charging session must not be null!");

            return Id.CompareTo(Session.Id);

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
        {

            if (Session is null)
                return false;

            return Id.Equals(Session.Id);

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
