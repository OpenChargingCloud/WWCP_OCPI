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
    /// The CDR object describes the Charging Session and its costs.
    /// How these costs are build up etc.
    /// </summary>
    public class CDR : AEMobilityEntity<CDR_Id>,
                       IEquatable<CDR>, IComparable<CDR>, IComparable
    {

        #region Properties

        /// <summary>
        /// The time when the CDR became active.
        /// </summary>
        [Mandatory]
        public DateTime Start { get; }

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

        /// <summary>
        /// An id provided by the authentication used so that the eMSP knows to which driver the CDR belongs.
        /// </summary>
        [Mandatory]
        public Auth_Id AuthId { get; }

        /// <summary>
        /// Method used for authentication.
        /// </summary>
        [Mandatory]
        public AuthMethodTypes AuthMethod { get; }

        /// <summary>
        /// List of relevant values for this charging period.
        /// </summary>
        public IEnumerable<CDRDimension> Dimensions { get; }

        /// <summary>
        /// The location where this CDR took place.
        /// </summary>
        [Mandatory]
        public Location Location { get; }

        /// <summary>
        /// The EVSE that was used for this CDR.
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
        /// ISO 4217 code of the currency used for this CDR.
        /// </summary>
        [Mandatory]
        public Currency Currency { get; }

        /// <summary>
        /// Enumeration of relevant tariffs.
        /// </summary>
        public IEnumerable<Tariff> Tariffs { get; }

        /// <summary>
        /// Enumeration of charging periods that make up this charging session.
        /// A session consist of 1 or more periodes with, each period has a
        /// different relevant Tariff.
        /// </summary>
        public IEnumerable<ChargingPeriod> ChargingPeriods { get; }

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
                   AuthMethodTypes               AuthMethod,
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

            if (!Dimensions.SafeAny())
                throw new ArgumentNullException(nameof(Dimensions),       "The given enumeration must not be null or empty!");

            if (!ChargingPeriods.SafeAny())
                throw new ArgumentNullException(nameof(ChargingPeriods),  "The given enumeration must not be null or empty!");

            #endregion

            this.Start            = Start;
            this._End              = End;
            this.AuthId           = AuthId;
            this.AuthMethod       = AuthMethod;
            this.Dimensions       = Dimensions;
            this.Location         = Location ?? throw new ArgumentNullException(nameof(Location),  "The given parameter must not be null!");
            this.EVSE             = EVSE     ?? throw new ArgumentNullException(nameof(EVSE),      "The given parameter must not be null!");
            this.ConnectorId      = ConnectorId;
            this.MeterId          = MeterId;
            this.Currency         = Currency ?? throw new ArgumentNullException(nameof(Currency),  "The given parameter must not be null!");
            this.Tariffs          = Tariffs;
            this.ChargingPeriods  = ChargingPeriods;
            this._TotalCosts       = TotalCosts;
            this._Remark           = Remark ?? new I18NString();

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
