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
using System.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The session object describes one charging session in a roaming scenario.
    /// The object is virtual and lives between the operator's and provider's systems.
    /// Each of the two parties hold a inheritance of this virtual object.
    /// </summary>
    public class Session : AEMobilityEntity<Session_Id>,
                           IEquatable<Session>, IComparable<Session>, IComparable
    {

        #region Properties

        /// <summary>
        /// The time when the session became active.
        /// </summary>
        [Mandatory]
        public DateTime Start { get; }

        /// <summary>
        /// The time when the session is completed.
        /// </summary>
        [Optional]
        public DateTime? End { get; }

        /// <summary>
        /// How many kWh are charged.
        /// </summary>
        [Mandatory]
        public Decimal kWh { get; }

        /// <summary>
        /// An id provided by the authentication used so that the eMSP knows to which driver the session belongs.
        /// </summary>
        [Mandatory]
        public Auth_Id AuthId { get; }

        /// <summary>
        /// Method used for authentication.
        /// </summary>
        [Mandatory]
        public AuthMethodTypes AuthMethod { get; }

        /// <summary>
        /// The location where this session took place.
        /// </summary>
        [Mandatory]
        public Location Location { get; }

        /// <summary>
        /// The EVSE that was used for this session.
        /// </summary>
        [Mandatory]
        public EVSE EVSE { get; }

        /// <summary>
        /// Connector ID of the connector used at the EVSE.
        /// </summary>
        [Mandatory]
        public Connector_Id ConnectorId { get; }

        /// <summary>
        /// Optional identification of the kWh energy meter.
        /// </summary>
        [Optional]
        public Meter_Id MeterId { get; }

        /// <summary>
        /// ISO 4217 code of the currency used for this session.
        /// </summary>
        [Mandatory]
        public Currency Currency { get; }

        /// <summary>
        /// An optional enumeration of charging periods that can be used to calculate and verify the total cost.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingPeriod> ChargingPeriods { get; }

        /// <summary>
        /// The total cost (excluding VAT) of the session in the specified currency.
        /// This is the price that the eMSP will have to pay to the CPO.
        /// </summary>
        [Mandatory]
        public Decimal TotalCosts { get; }

        /// <summary>
        /// The total cost (excluding VAT) of the session in the specified currency.
        /// This is the price that the eMSP will have to pay to the CPO.
        /// </summary>
        [Mandatory]
        public SessionStatusTypes Status { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The Session object describes the Session and its properties
        /// where a group of EVSEs that belong together are installed.
        /// </summary>
        /// <param name="Id">Uniquely identifies the Session within the CPOs platform (and suboperator platforms).</param>
        /// <param name="Start">The time when the session became active.</param>
        /// <param name="End">The time when the session is completed.</param>
        /// <param name="kWh">How many kWh are charged.</param>
        /// <param name="AuthId">An id provided by the authentication used so that the eMSP knows to which driver the session belongs.</param>
        /// <param name="AuthMethod">Method used for authentication.</param>
        /// <param name="Location">The location where this session took place.</param>
        /// <param name="EVSE">The EVSE that was used for this session.</param>
        /// <param name="ConnectorId">Connector ID of the connector used at the EVSE.</param>
        /// <param name="MeterId">Optional identification of the kWh meter.</param>
        /// <param name="Currency">ISO 4217 code of the currency used for this session.</param>
        /// <param name="ChargingPeriods">An optional enumeration of charging periods that can be used to calculate and verify the total cost.</param>
        /// <param name="TotalCosts">The total cost (excluding VAT) of the session in the specified currency. This is the price that the eMSP will have to pay to the CPO.</param>
        /// <param name="Status">The status of the session.</param>
        public Session(Session_Id                   Id,
                       DateTime                     Start,
                       DateTime?                    End,
                       Decimal                      kWh,
                       Auth_Id                      AuthId,
                       AuthMethodTypes              AuthMethod,
                       Location                     Location,
                       EVSE                         EVSE,
                       Connector_Id                 ConnectorId,
                       Meter_Id                     MeterId,
                       Currency                     Currency,
                       IEnumerable<ChargingPeriod>  ChargingPeriods,
                       Decimal                      TotalCosts,
                       SessionStatusTypes           Status)

            : base(Id)

        {

            this.Start            = Start;
            this.End              = End;
            this.kWh              = kWh;
            this.AuthId           = AuthId;
            this.AuthMethod       = AuthMethod;
            this.Location         = Location ?? throw new ArgumentNullException(nameof(Location),  "The given parameter must not be null!");
            this.EVSE             = EVSE     ?? throw new ArgumentNullException(nameof(EVSE),      "The given parameter must not be null!");
            this.ConnectorId      = ConnectorId;
            this.MeterId          = MeterId;
            this.Currency         = Currency ?? throw new ArgumentNullException(nameof(Currency),  "The given parameter must not be null!");
            this.ChargingPeriods  = ChargingPeriods;
            this.TotalCosts       = TotalCosts;
            this.Status           = Status;

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
                throw new ArgumentNullException(nameof(Session),  "The given session must not be null!");

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
