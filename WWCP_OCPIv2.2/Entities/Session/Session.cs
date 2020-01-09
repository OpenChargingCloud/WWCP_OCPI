/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing software
 * distributed under the License is distributed on an "AS IS" BASIS
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPIv2_2
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

        #region Start

        private readonly DateTime _Start;

        /// <summary>
        /// The time when the session became active.
        /// </summary>
        [Mandatory]
        public DateTime Start
        {
            get
            {
                return _Start;
            }
        }

        #endregion

        #region End

        private DateTime? _End;

        /// <summary>
        /// The time when the session is completed.
        /// </summary>
        [Optional]
        public DateTime? End
        {

            get
            {
                return _End;
            }

            set
            {

                if (_End != value)
                    SetProperty(ref _End, value);

            }

        }

        #endregion

        #region kWh

        private readonly Decimal _kWh;

        /// <summary>
        /// How many kWh are charged.
        /// </summary>
        [Mandatory]
        public decimal kWh
        {
            get
            {
                return _kWh;
            }
        }

        #endregion

        #region AuthId

        private readonly Auth_Id _AuthId;

        /// <summary>
        /// An id provided by the authentication used so that the eMSP knows to which driver the session belongs.
        /// </summary>
        [Mandatory]
        public Auth_Id AuthId
        {
            get
            {
                return _AuthId;
            }
        }

        #endregion

        #region AuthMethod

        private readonly AuthMethodType _AuthMethod;

        /// <summary>
        /// Method used for authentication.
        /// </summary>
        [Mandatory]
        public AuthMethodType AuthMethod
        {
            get
            {
                return _AuthMethod;
            }
        }

        #endregion

        #region Location

        private readonly Location _Location;

        /// <summary>
        /// The location where this session took place.
        /// </summary>
        [Mandatory]
        public Location Location
        {
            get
            {
                return _Location;
            }
        }

        #endregion

        #region EVSE

        private readonly EVSE _EVSE;

        /// <summary>
        /// The EVSE that was used for this session.
        /// </summary>
        [Mandatory]
        public EVSE EVSE
        {
            get
            {
                return _EVSE;
            }
        }

        #endregion

        #region ConnectorId

        private readonly Connector_Id _ConnectorId;

        /// <summary>
        /// Connector ID of the connector used at the EVSE.
        /// </summary>
        [Mandatory]
        public Connector_Id ConnectorId
        {
            get
            {
                return _ConnectorId;
            }
        }

        #endregion

        #region MeterId

        private readonly Meter_Id _MeterId;

        /// <summary>
        /// Optional identification of the kWh energy meter.
        /// </summary>
        [Optional]
        public Meter_Id MeterId
        {
            get
            {
                return _MeterId;
            }
        }

        #endregion

        #region Currency

        private readonly Currency _Currency;

        /// <summary>
        /// ISO 4217 code of the currency used for this session.
        /// </summary>
        [Mandatory]
        public Currency Currency
        {
            get
            {
                return _Currency;
            }
        }

        #endregion

        #region ChargingPeriods

        private readonly IEnumerable<ChargingPeriod> _ChargingPeriods;

        /// <summary>
        /// An optional enumeration of charging periods that can be used to calculate and verify the total cost.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingPeriod> ChargingPeriods
        {
            get
            {
                return _ChargingPeriods;
            }
        }

        #endregion

        #region TotalCosts

        private Decimal _TotalCosts;

        /// <summary>
        /// The total cost (excluding VAT) of the session in the specified currency.
        /// This is the price that the eMSP will have to pay to the CPO.
        /// </summary>
        [Mandatory]
        public Decimal TotalCosts
        {

            get
            {
                return _TotalCosts;
            }

            set
            {

                if (_TotalCosts != value)
                    SetProperty(ref _TotalCosts, value);

            }

        }

        #endregion

        #region Status

        private SessionStatusType _Status;

        /// <summary>
        /// The total cost (excluding VAT) of the session in the specified currency.
        /// This is the price that the eMSP will have to pay to the CPO.
        /// </summary>
        [Mandatory]
        public SessionStatusType Status
        {

            get
            {
                return _Status;
            }

            set
            {

                if (_Status != value)
                    SetProperty(ref _Status, value);

            }

        }

        #endregion

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
                       AuthMethodType               AuthMethod,
                       Location                     Location,
                       EVSE                         EVSE,
                       Connector_Id                 ConnectorId,
                       Meter_Id                     MeterId,
                       Currency                     Currency,
                       IEnumerable<ChargingPeriod>  ChargingPeriods,
                       Decimal                      TotalCosts,
                       SessionStatusType            Status)

            : base(Id)

        {

            #region Initial checks

            if (AuthId      == null)
                throw new ArgumentNullException("AuthId",       "The given parameter must not be null!");

            if (Location    == null)
                throw new ArgumentNullException("Location",     "The given parameter must not be null!");

            if (EVSE        == null)
                throw new ArgumentNullException("EVSE",         "The given parameter must not be null!");

            if (ConnectorId == null)
                throw new ArgumentNullException("ConnectorId",  "The given parameter must not be null!");

            if (MeterId     == null)
                throw new ArgumentNullException("MeterId",      "The given parameter must not be null!");

            if (Currency    == null)
                throw new ArgumentNullException("Currency",     "The given parameter must not be null!");

            #endregion

            #region Init data and properties

            this._Start            = Start;
            this._End              = End;
            this._kWh              = kWh;
            this._AuthId           = AuthId;
            this._AuthMethod       = AuthMethod;
            this._Location         = Location;
            this._EVSE             = EVSE;
            this._ConnectorId      = ConnectorId;
            this._MeterId          = MeterId;
            this._Currency         = Currency;
            this._ChargingPeriods  = ChargingPeriods;
            this._TotalCosts       = TotalCosts;
            this._Status           = Status;

            #endregion

        }

        #endregion


        #region IComparable<Session> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an Session.
            var Session = Object as Session;
            if ((Object) Session == null)
                throw new ArgumentException("The given object is not an Session!");

            return CompareTo(Session);

        }

        #endregion

        #region CompareTo(Session)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Session">An Session to compare with.</param>
        public Int32 CompareTo(Session Session)
        {

            if ((Object) Session == null)
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
        {

            if (Object == null)
                return false;

            // Check if the given object is an Session.
            var Session = Object as Session;
            if ((Object) Session == null)
                return false;

            return this.Equals(Session);

        }

        #endregion

        #region Equals(Session)

        /// <summary>
        /// Compares two Sessions for equality.
        /// </summary>
        /// <param name="Session">An Session to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Session Session)
        {

            if ((Object) Session == null)
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
