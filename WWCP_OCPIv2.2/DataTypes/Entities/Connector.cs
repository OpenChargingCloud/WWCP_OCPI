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

using org.GraphDefined.Vanaheimr.Illias;
using System;
using System.Collections.Generic;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// A connector is the socket or cable available for the EV to make use of.
    /// </summary>
    public class Connector : IHasId<Connector_Id>,
                             IEquatable<Connector>,
                             IComparable<Connector>,
                             IComparable
    {

        #region Properties

        /// <summary>
        /// Identifier of the connector within the EVSE.
        /// Two connectors may have the same id as long as they do not belong to the same EVSE object.
        /// </summary>
        [Mandatory]
        public Connector_Id            Id                    { get; }

        /// <summary>
        /// The standard of the installed connector.
        /// </summary>
        [Mandatory]
        public ConnectorTypes          Standard              { get; }

        /// <summary>
        /// The format (socket/cable) of the installed connector.
        /// </summary>
        [Mandatory]
        public ConnectorFormats        Format                { get; }

        /// <summary>
        /// The type of powert at the connector.
        /// </summary>
        [Mandatory]
        public PowerTypes              PowerType             { get; }

        /// <summary>
        /// Voltage of the connector (line to neutral for AC_3_PHASE), in volt [V].
        /// </summary>
        [Mandatory]
        public UInt16                  MaxVoltage            { get; }

        /// <summary>
        /// Maximum amperage of the connector, in ampere [A].
        /// </summary>
        [Mandatory]
        public UInt16                  MaxAmperage           { get; }

        /// <summary>
        /// Maximum electric power that can be delivered by this connector, in Watts (W).
        /// When the maximum electric power is lower than the calculated value from voltage
        /// and amperage, this value should be set.
        /// </summary>
        [Optional]
        public UInt16?                 MaxElectricPower      { get; }

        /// <summary>
        /// Identifiers of the currently valid charging tariffs. Multiple tariffs are possible,
        /// but only one of each Tariff.type can be active at the same time. Tariffs with the
        /// same type are only allowed if they are not active at the same time: start_date_time
        /// and end_date_time period not overlapping.
        /// </summary>
        [Optional]
        public IEnumerable<Tariff_Id>  TariffIds             { get; }

        /// <summary>
        /// Optional URL to the operator's terms and conditions.
        /// </summary>
        [Optional]
        public String                  TermsAndConditions    { get; }

        /// <summary>
        /// Timestamp when this connector was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                LastUpdated           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A connector is the socket or cable available for the EV to make use of.
        /// </summary>
        public Connector(Connector_Id            Id,
                         ConnectorTypes          Standard,
                         ConnectorFormats        Format,
                         PowerTypes              PowerType,
                         UInt16                  MaxVoltage,
                         UInt16                  MaxAmperage,

                         UInt16?                 MaxElectricPower     = null,
                         IEnumerable<Tariff_Id>  TariffIds            = null,
                         String                  TermsAndConditions   = null,

                         DateTime?               LastUpdated          = null)
        {

            this.Id                   = Id;
            this.Standard             = Standard;
            this.Format               = Format;
            this.PowerType            = PowerType;
            this.MaxVoltage           = MaxVoltage;
            this.MaxAmperage          = MaxAmperage;

            this.MaxElectricPower     = MaxElectricPower;
            this.TariffIds            = TariffIds   ?? new Tariff_Id[0];
            this.TermsAndConditions   = TermsAndConditions;

            this.LastUpdated          = LastUpdated ?? DateTime.Now;

        }

        #endregion


        #region IComparable<Connector> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Connector connector
                   ? CompareTo(connector)
                   : throw new ArgumentException("The given object is not a connector!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Connector)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Connector">An Connector to compare with.</param>
        public Int32 CompareTo(Connector Connector)
        {

            if (Connector is null)
                throw new ArgumentNullException("The given Connector must not be null!");

            return Id.CompareTo(Connector.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Connector> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Connector connector &&
                   Equals(connector);

        #endregion

        #region Equals(Connector)

        /// <summary>
        /// Compares two Connectors for equality.
        /// </summary>
        /// <param name="Connector">An Connector to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Connector Connector)

            => (!(Connector is null)) &&
                   Id.Equals(Connector.Id);

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
