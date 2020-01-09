/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

#endregion

namespace org.GraphDefined.WWCP.OCPIv2_2
{

    /// <summary>
    /// A connector is the socket or cable available for the EV to make use of.
    /// </summary>
    public class Connector : IEquatable<Connector>, IComparable<Connector>, IComparable
    {

        #region Properties

        #region Id

        private readonly Connector_Id _Id;

        /// <summary>
        /// Identifier of the connector within the EVSE.
        /// Two connectors may have the same id as long as they do not belong to the same EVSE object.
        /// </summary>
        public Connector_Id Id
        {
            get
            {
                return _Id;
            }
        }

        #endregion

        #region Standard

        private readonly ConnectorType _Standard;

        /// <summary>
        /// The standard of the installed connector.
        /// </summary>
        public ConnectorType Standard
        {
            get
            {
                return _Standard;
            }
        }

        #endregion

        #region Format

        private readonly ConnectorFormatType _Format;

        /// <summary>
        /// The format (socket/cable) of the installed connector.
        /// </summary>
        public ConnectorFormatType Format
        {
            get
            {
                return _Format;
            }
        }

        #endregion

        #region PowerType

        private readonly PowerType _PowerType;

        /// <summary>
        /// The type of powert at the connector.
        /// </summary>
        public PowerType PowerType
        {
            get
            {
                return _PowerType;
            }
        }

        #endregion

        #region Voltage

        private readonly UInt16 _Voltage;

        /// <summary>
        /// Voltage of the connector (line to neutral for AC_3_PHASE), in volt [V].
        /// </summary>
        public UInt16 Voltage
        {
            get
            {
                return _Voltage;
            }
        }

        #endregion

        #region Amperage

        private readonly UInt16 _Amperage;

        /// <summary>
        /// Maximum amperage of the connector, in ampere [A].
        /// </summary>
        public UInt16 Amperage
        {
            get
            {
                return _Amperage;
            }
        }

        #endregion

        #region TariffId

        private readonly Tariff_Id _TariffId;

        /// <summary>
        /// Optional identifier of the current charging tariff structure.
        /// </summary>
        public Tariff_Id TariffId
        {
            get
            {
                return _TariffId;
            }
        }

        #endregion

        #region TermsAndConditions

        private readonly Uri _TermsAndConditions;

        /// <summary>
        /// Optional URL to the operator's terms and conditions.
        /// </summary>
        public Uri TermsAndConditions
        {
            get
            {
                return _TermsAndConditions;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A connector is the socket or cable available for the EV to make use of.
        /// </summary>
        /// <param name="ConnectorId">Identifier of the connector within the EVSE. Two connectors may have the same id as long as they do not belong to the same EVSE object.</param>
        /// <param name="Standard">The standard of the installed connector.</param>
        /// <param name="Format">The format (socket/cable) of the installed connector.</param>
        /// <param name="PowerType">The type of powert at the connector.</param>
        /// <param name="Voltage">Voltage of the connector (line to neutral for AC_3_PHASE), in volt [V].</param>
        /// <param name="Amperage">Maximum amperage of the connector, in ampere [A].</param>
        /// <param name="TariffId">Optional identifier of the current charging tariff structure.</param>
        /// <param name="TermsAndConditions">Optional URL to the operator's terms and conditions.</param>
        public Connector(Connector_Id         ConnectorId,
                         ConnectorType        Standard,
                         ConnectorFormatType  Format,
                         PowerType            PowerType,
                         UInt16               Voltage,
                         UInt16               Amperage,
                         Tariff_Id            TariffId            = null,
                         Uri                  TermsAndConditions  = null)
        {

            #region Initial checks

            if (ConnectorId == null)
                throw new ArgumentNullException("ConnectorId", "The given parameter must not be null!");

            #endregion

            this._Id                  = ConnectorId;
            this._Standard            = Standard;
            this._Format              = Format;
            this._PowerType           = PowerType;
            this._Voltage             = Voltage;
            this._Amperage            = Amperage;
            this._TariffId            = TariffId;
            this._TermsAndConditions  = TermsAndConditions;

        }

        #endregion


        #region IComparable<Connector> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an Connector.
            var Connector = Object as Connector;
            if ((Object) Connector == null)
                throw new ArgumentException("The given object is not an Connector!");

            return CompareTo(Connector);

        }

        #endregion

        #region CompareTo(Connector)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Connector">An Connector to compare with.</param>
        public Int32 CompareTo(Connector Connector)
        {

            if ((Object) Connector == null)
                throw new ArgumentNullException("The given Connector must not be null!");

            return _Id.CompareTo(Connector._Id);

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
        {

            if (Object == null)
                return false;

            // Check if the given object is an Connector.
            var Connector = Object as Connector;
            if ((Object) Connector == null)
                return false;

            return this.Equals(Connector);

        }

        #endregion

        #region Equals(Connector)

        /// <summary>
        /// Compares two Connectors for equality.
        /// </summary>
        /// <param name="Connector">An Connector to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Connector Connector)
        {

            if ((Object) Connector == null)
                return false;

            return _Id.Equals(Connector._Id);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            return _Id.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return _Id.ToString();
        }

        #endregion

    }

}
