/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPIv2_1
{

    /// <summary>
    /// The CDR object describes the Charging Session and its costs.
    /// How these costs are build up etc.
    /// </summary>
    public class CDR : AEMobilityEntity<CDR_Id>,
                       IEquatable<CDR>, IComparable<CDR>, IComparable
    {

        #region Properties

        #region Start

        private readonly DateTime _Start;

        /// <summary>
        /// The time when the CDR became active.
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
        /// The time when the CDR is completed.
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

        #region AuthId

        private readonly Auth_Id _AuthId;

        /// <summary>
        /// An id provided by the authentication used so that the eMSP knows to which driver the CDR belongs.
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

        #region Dimensions

        private readonly IEnumerable<CDRDimension> _Dimensions;

        /// <summary>
        /// List of relevant values for this charging period.
        /// </summary>
        public IEnumerable<CDRDimension> Dimensions
        {
            get
            {
                return _Dimensions;
            }
        }

        #endregion

        #region Location

        private readonly Location _Location;

        /// <summary>
        /// The location where this CDR took place.
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
        /// The EVSE that was used for this CDR.
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
        /// ISO 4217 code of the currency used for this CDR.
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

        #region Tariffs

        private readonly IEnumerable<Tariff> _Tariffs;

        /// <summary>
        /// Enumeration of relevant tariffs.
        /// </summary>
        public IEnumerable<Tariff> Tariffs
        {
            get
            {
                return _Tariffs;
            }
        }

        #endregion

        #region ChargingPeriods

        private readonly IEnumerable<ChargingPeriod> _ChargingPeriods;

        /// <summary>
        /// Enumeration of charging periods that make up this charging session.
        /// A session consist of 1 or more periodes with, each period has a
        /// different relevant Tariff.
        /// </summary>
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
        /// The total cost (excluding VAT) of the CDR in the specified currency.
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

        #region Remark

        private I18NString _Remark;

        /// <summary>
        /// Optional remark, can be used to provide addition human
        /// readable information to the CDR, for example:
        /// reason why a transaction was stopped.
        /// </summary>
        [Optional]
        public I18NString Remark
        {

            get
            {
                return _Remark;
            }

            set
            {

                if (_Remark != value)
                    SetProperty(ref _Remark, value);

            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The CDR object describes the Charging Session and its costs.
        /// How these costs are build up etc.
        /// </summary>
        /// <param name="Id">Uniquely identifies the CDR within the CPOs platform (and suboperator platforms).</param>
        /// <param name="Start">The time when the CDR became active.</param>
        /// <param name="End">The time when the CDR is completed.</param>
        /// <param name="AuthId">An id provided by the authentication used so that the eMSP knows to which driver the CDR belongs.</param>
        /// <param name="AuthMethod">Method used for authentication.</param>
        /// <param name="Dimensions">List of relevant values for this charging period.</param>
        /// <param name="Location">The location where this CDR took place.</param>
        /// <param name="EVSE">The EVSE that was used for this CDR.</param>
        /// <param name="ConnectorId">Connector ID of the connector used at the EVSE.</param>
        /// <param name="MeterId">Optional identification of the kWh meter.</param>
        /// <param name="Currency">ISO 4217 code of the currency used for this CDR.</param>
        /// <param name="Tariffs">Enumeration of relevant tariffs.</param>
        /// <param name="ChargingPeriods">Enumeration of charging periods that make up this charging session. A session consist of 1 or more periodes with, each period has a different relevant Tariff.</param>
        /// <param name="TotalCosts">The total cost (excluding VAT) of the CDR in the specified currency. This is the price that the eMSP will have to pay to the CPO.</param>
        /// <param name="Remark">Optional remark, can be used to provide addition human readable information to the CDR, for example: reason why a transaction was stopped.</param>
        public CDR(CDR_Id                       Id,
                   DateTime                     Start,
                   DateTime?                    End,
                   Auth_Id                      AuthId,
                   AuthMethodType               AuthMethod,
                   IEnumerable<CDRDimension>    Dimensions,
                   Location                     Location,
                   EVSE                         EVSE,
                   Connector_Id                 ConnectorId,
                   Meter_Id                     MeterId,
                   Currency                     Currency,
                   IEnumerable<Tariff>          Tariffs,
                   IEnumerable<ChargingPeriod>  ChargingPeriods,
                   Decimal                      TotalCosts,
                   I18NString                   Remark)

            : base(Id)

        {

            #region Initial checks

            if (AuthId      == null)
                throw new ArgumentNullException("AuthId",           "The given parameter must not be null!");

            if (Dimensions == null)
                throw new ArgumentNullException("Dimensions",       "The given parameter must not be null!");

            if (!Dimensions.Any())
                throw new ArgumentNullException("Dimensions",       "The given enumeration must not be empty!");

            if (Location    == null)
                throw new ArgumentNullException("Location",         "The given parameter must not be null!");

            if (EVSE        == null)
                throw new ArgumentNullException("EVSE",             "The given parameter must not be null!");

            if (ConnectorId == null)
                throw new ArgumentNullException("ConnectorId",      "The given parameter must not be null!");

            if (MeterId     == null)
                throw new ArgumentNullException("MeterId",          "The given parameter must not be null!");

            if (Currency    == null)
                throw new ArgumentNullException("Currency",         "The given parameter must not be null!");

            if (ChargingPeriods == null)
                throw new ArgumentNullException("ChargingPeriods",  "The given parameter must not be null!");

            if (!ChargingPeriods.Any())
                throw new ArgumentNullException("ChargingPeriods",  "The given enumeration must not be empty!");

            #endregion

            #region Init data and properties

            this._Start            = Start;
            this._End              = End;
            this._AuthId           = AuthId;
            this._AuthMethod       = AuthMethod;
            this._Dimensions       = Dimensions;
            this._Location         = Location;
            this._EVSE             = EVSE;
            this._ConnectorId      = ConnectorId;
            this._MeterId          = MeterId;
            this._Currency         = Currency;
            this._Tariffs          = Tariffs;
            this._ChargingPeriods  = ChargingPeriods;
            this._TotalCosts       = TotalCosts;
            this._Remark           = Remark != null ? Remark : new I18NString();

            #endregion

        }

        #endregion


        #region IComparable<CDR> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an CDR.
            var CDR = Object as CDR;
            if ((Object) CDR == null)
                throw new ArgumentException("The given object is not an CDR!");

            return CompareTo(CDR);

        }

        #endregion

        #region CompareTo(CDR)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR">An CDR to compare with.</param>
        public Int32 CompareTo(CDR CDR)
        {

            if ((Object) CDR == null)
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
        {

            if (Object == null)
                return false;

            // Check if the given object is an CDR.
            var CDR = Object as CDR;
            if ((Object) CDR == null)
                return false;

            return this.Equals(CDR);

        }

        #endregion

        #region Equals(CDR)

        /// <summary>
        /// Compares two CDRs for equality.
        /// </summary>
        /// <param name="CDR">An CDR to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CDR CDR)
        {

            if ((Object) CDR == null)
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
