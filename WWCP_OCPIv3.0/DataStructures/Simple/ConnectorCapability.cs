/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for capabilities.
    /// </summary>
    public static class ConnectorCapabilityExtensions
    {

        /// <summary>
        /// Indicates whether this connector capability is null or empty.
        /// </summary>
        /// <param name="ConnectorCapability">A connector capability.</param>
        public static Boolean IsNullOrEmpty(this ConnectorCapability? ConnectorCapability)
            => !ConnectorCapability.HasValue || ConnectorCapability.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this connector capability is NOT null or empty.
        /// </summary>
        /// <param name="ConnectorCapability">A connector capability.</param>
        public static Boolean IsNotNullOrEmpty(this ConnectorCapability? ConnectorCapability)
            => ConnectorCapability.HasValue && ConnectorCapability.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// Capabilities or functionalities of an EVSE.
    /// </summary>
    public readonly struct ConnectorCapability : IId<ConnectorCapability>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this connector capability is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this connector capability is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the connector capability.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new connector capability based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a connector capability.</param>
        private ConnectorCapability(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a connector capability.
        /// </summary>
        /// <param name="Text">A text representation of a connector capability.</param>
        public static ConnectorCapability Parse(String Text)
        {

            if (TryParse(Text, out var meterId))
                return meterId;

            throw new ArgumentException($"Invalid text representation of a connector capability: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a connector capability.
        /// </summary>
        /// <param name="Text">A text representation of a connector capability.</param>
        public static ConnectorCapability? TryParse(String Text)
        {

            if (TryParse(Text, out var meterId))
                return meterId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ConnectorCapability)

        /// <summary>
        /// Try to parse the given text as a connector capability.
        /// </summary>
        /// <param name="Text">A text representation of a connector capability.</param>
        /// <param name="ConnectorCapability">The parsed connector capability.</param>
        public static Boolean TryParse(String Text, out ConnectorCapability ConnectorCapability)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ConnectorCapability = new ConnectorCapability(Text);
                    return true;
                }
                catch
                { }
            }

            ConnectorCapability = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this connector capability.
        /// </summary>
        public ConnectorCapability Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The EVSE supports charging profiles.
        /// </summary>
        public static ConnectorCapability CHARGING_PROFILE_CAPABLE            { get; }
            = new ("CHARGING_PROFILE_CAPABLE");

        /// <summary>
        /// The EVSE supports charging preferences.
        /// </summary>
        public static ConnectorCapability CHARGING_PREFERENCES_CAPABLE        { get; }
            = new ("CHARGING_PREFERENCES_CAPABLE");

        /// <summary>
        /// EVSE has a payment terminal that supports chip cards.
        /// </summary>
        public static ConnectorCapability CHIP_CARD_SUPPORT                   { get; }
            = new ("CHIP_CARD_SUPPORT");

        /// <summary>
        /// EVSE has a payment terminal that supports contactless cards.
        /// </summary>
        public static ConnectorCapability CONTACTLESS_CARD_SUPPORT            { get; }
            = new ("CONTACTLESS_CARD_SUPPORT");

        /// <summary>
        /// EVSE has a payment terminal that makes it possible to pay for charging using a credit card.
        /// </summary>
        public static ConnectorCapability CREDIT_CARD_PAYABLE                 { get; }
            = new ("CREDIT_CARD_PAYABLE");

        /// <summary>
        /// EVSE has a payment terminal that makes it possible to pay for charging using a debit card.
        /// </summary>
        public static ConnectorCapability DEBIT_CARD_PAYABLE                  { get; }
            = new ("DEBIT_CARD_PAYABLE");

        /// <summary>
        /// EVSE has a payment terminal with a pin-code entry device.
        /// </summary>
        public static ConnectorCapability PED_TERMINAL                        { get; }
            = new ("PED_TERMINAL");

        /// <summary>
        /// The EVSE can remotely be started/stopped.
        /// </summary>
        public static ConnectorCapability REMOTE_START_STOP_CAPABLE           { get; }
            = new ("REMOTE_START_STOP_CAPABLE");

        /// <summary>
        /// The EVSE can be reserved.
        /// </summary>
        public static ConnectorCapability RESERVABLE                          { get; }
            = new ("RESERVABLE");

        /// <summary>
        /// Charging at this EVSE can be authorized with a RFID token.
        /// </summary>
        public static ConnectorCapability RFID_READER                         { get; }
            = new ("RFID_READER");

        /// <summary>
        /// When a StartSession is sent to this EVSE, the MSP is required to add the
        /// optional connector_id field in the StartSession object.
        /// </summary>
        public static ConnectorCapability START_SESSION_CONNECTOR_REQUIRED    { get; }
            = new ("START_SESSION_CONNECTOR_REQUIRED");

        /// <summary>
        /// This EVSE supports token groups, two or more tokens work as one, so that a session can be
        /// started with one token and stopped with another (handy when a card and key-fob are given
        /// to the EV-driver).
        /// </summary>
        public static ConnectorCapability TOKEN_GROUP_CAPABLE                 { get; }
            = new ("TOKEN_GROUP_CAPABLE");

        /// <summary>
        /// Connectors have mechanical lock that can be requested by the eMSP to be unlocked.
        /// </summary>
        public static ConnectorCapability UNLOCK_CAPABLE                      { get; }
            = new ("UNLOCK_CAPABLE");

        #endregion


        #region Operator overloading

        #region Operator == (ConnectorCapability1, ConnectorCapability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorCapability1">A connector capability.</param>
        /// <param name="ConnectorCapability2">Another connector capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ConnectorCapability ConnectorCapability1,
                                           ConnectorCapability ConnectorCapability2)

            => ConnectorCapability1.Equals(ConnectorCapability2);

        #endregion

        #region Operator != (ConnectorCapability1, ConnectorCapability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorCapability1">A connector capability.</param>
        /// <param name="ConnectorCapability2">Another connector capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ConnectorCapability ConnectorCapability1,
                                           ConnectorCapability ConnectorCapability2)

            => !ConnectorCapability1.Equals(ConnectorCapability2);

        #endregion

        #region Operator <  (ConnectorCapability1, ConnectorCapability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorCapability1">A connector capability.</param>
        /// <param name="ConnectorCapability2">Another connector capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ConnectorCapability ConnectorCapability1,
                                          ConnectorCapability ConnectorCapability2)

            => ConnectorCapability1.CompareTo(ConnectorCapability2) < 0;

        #endregion

        #region Operator <= (ConnectorCapability1, ConnectorCapability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorCapability1">A connector capability.</param>
        /// <param name="ConnectorCapability2">Another connector capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ConnectorCapability ConnectorCapability1,
                                           ConnectorCapability ConnectorCapability2)

            => ConnectorCapability1.CompareTo(ConnectorCapability2) <= 0;

        #endregion

        #region Operator >  (ConnectorCapability1, ConnectorCapability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorCapability1">A connector capability.</param>
        /// <param name="ConnectorCapability2">Another connector capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ConnectorCapability ConnectorCapability1,
                                          ConnectorCapability ConnectorCapability2)

            => ConnectorCapability1.CompareTo(ConnectorCapability2) > 0;

        #endregion

        #region Operator >= (ConnectorCapability1, ConnectorCapability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorCapability1">A connector capability.</param>
        /// <param name="ConnectorCapability2">Another connector capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ConnectorCapability ConnectorCapability1,
                                           ConnectorCapability ConnectorCapability2)

            => ConnectorCapability1.CompareTo(ConnectorCapability2) >= 0;

        #endregion

        #endregion

        #region IComparable<ConnectorCapability> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two capabilities.
        /// </summary>
        /// <param name="Object">A connector capability to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ConnectorCapability meterId
                   ? CompareTo(meterId)
                   : throw new ArgumentException("The given object is not a connector capability!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ConnectorCapability)

        /// <summary>
        /// Compares two capabilities.
        /// </summary>
        /// <param name="ConnectorCapability">A connector capability to compare with.</param>
        public Int32 CompareTo(ConnectorCapability ConnectorCapability)

            => String.Compare(InternalId,
                              ConnectorCapability.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ConnectorCapability> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two capabilities for equality.
        /// </summary>
        /// <param name="Object">A connector capability to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ConnectorCapability meterId &&
                   Equals(meterId);

        #endregion

        #region Equals(ConnectorCapability)

        /// <summary>
        /// Compares two capabilities for equality.
        /// </summary>
        /// <param name="ConnectorCapability">A connector capability to compare with.</param>
        public Boolean Equals(ConnectorCapability ConnectorCapability)

            => String.Equals(InternalId,
                             ConnectorCapability.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToUpper().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
