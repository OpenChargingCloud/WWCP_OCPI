/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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
    public static class CapabilityExtensions
    {

        /// <summary>
        /// Indicates whether this capability is null or empty.
        /// </summary>
        /// <param name="Capability">A capability.</param>
        public static Boolean IsNullOrEmpty(this Capability? Capability)
            => !Capability.HasValue || Capability.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this capability is NOT null or empty.
        /// </summary>
        /// <param name="Capability">A capability.</param>
        public static Boolean IsNotNullOrEmpty(this Capability? Capability)
            => Capability.HasValue && Capability.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// Capabilities or functionalities of an EVSE.
    /// </summary>
    public readonly struct Capability : IId<Capability>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this capability is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this capability is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the capability.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new capability based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a capability.</param>
        private Capability(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a capability.
        /// </summary>
        /// <param name="Text">A text representation of a capability.</param>
        public static Capability Parse(String Text)
        {

            if (TryParse(Text, out var meterId))
                return meterId;

            throw new ArgumentException($"Invalid text representation of a capability: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a capability.
        /// </summary>
        /// <param name="Text">A text representation of a capability.</param>
        public static Capability? TryParse(String Text)
        {

            if (TryParse(Text, out var meterId))
                return meterId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Capability)

        /// <summary>
        /// Try to parse the given text as a capability.
        /// </summary>
        /// <param name="Text">A text representation of a capability.</param>
        /// <param name="Capability">The parsed capability.</param>
        public static Boolean TryParse(String Text, out Capability Capability)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    Capability = new Capability(Text);
                    return true;
                }
                catch
                { }
            }

            Capability = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this capability.
        /// </summary>
        public Capability Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The EVSE supports charging profiles.
        /// </summary>
        public static Capability CHARGING_PROFILE_CAPABLE            { get; }
            = new ("CHARGING_PROFILE_CAPABLE");

        /// <summary>
        /// The EVSE supports charging preferences.
        /// </summary>
        public static Capability CHARGING_PREFERENCES_CAPABLE        { get; }
            = new ("CHARGING_PREFERENCES_CAPABLE");

        /// <summary>
        /// EVSE has a payment terminal that supports chip cards.
        /// </summary>
        public static Capability CHIP_CARD_SUPPORT                   { get; }
            = new ("CHIP_CARD_SUPPORT");

        /// <summary>
        /// EVSE has a payment terminal that supports contactless cards.
        /// </summary>
        public static Capability CONTACTLESS_CARD_SUPPORT            { get; }
            = new ("CONTACTLESS_CARD_SUPPORT");

        /// <summary>
        /// EVSE has a payment terminal that makes it possible to pay for charging using a credit card.
        /// </summary>
        public static Capability CREDIT_CARD_PAYABLE                 { get; }
            = new ("CREDIT_CARD_PAYABLE");

        /// <summary>
        /// EVSE has a payment terminal that makes it possible to pay for charging using a debit card.
        /// </summary>
        public static Capability DEBIT_CARD_PAYABLE                  { get; }
            = new ("DEBIT_CARD_PAYABLE");

        /// <summary>
        /// EVSE has a payment terminal with a pin-code entry device.
        /// </summary>
        public static Capability PED_TERMINAL                        { get; }
            = new ("PED_TERMINAL");

        /// <summary>
        /// The EVSE can remotely be started/stopped.
        /// </summary>
        public static Capability REMOTE_START_STOP_CAPABLE           { get; }
            = new ("REMOTE_START_STOP_CAPABLE");

        /// <summary>
        /// The EVSE can be reserved.
        /// </summary>
        public static Capability RESERVABLE                          { get; }
            = new ("RESERVABLE");

        /// <summary>
        /// Charging at this EVSE can be authorized with a RFID token.
        /// </summary>
        public static Capability RFID_READER                         { get; }
            = new ("RFID_READER");

        /// <summary>
        /// When a StartSession is sent to this EVSE, the MSP is required to add the
        /// optional connector_id field in the StartSession object.
        /// </summary>
        public static Capability START_SESSION_CONNECTOR_REQUIRED    { get; }
            = new ("START_SESSION_CONNECTOR_REQUIRED");

        /// <summary>
        /// This EVSE supports token groups, two or more tokens work as one, so that a session can be
        /// started with one token and stopped with another (handy when a card and key-fob are given
        /// to the EV-driver).
        /// </summary>
        public static Capability TOKEN_GROUP_CAPABLE                 { get; }
            = new ("TOKEN_GROUP_CAPABLE");

        /// <summary>
        /// Connectors have mechanical lock that can be requested by the eMSP to be unlocked.
        /// </summary>
        public static Capability UNLOCK_CAPABLE                      { get; }
            = new ("UNLOCK_CAPABLE");

        #endregion


        #region Operator overloading

        #region Operator == (Capability1, Capability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Capability1">A capability.</param>
        /// <param name="Capability2">Another capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Capability Capability1,
                                           Capability Capability2)

            => Capability1.Equals(Capability2);

        #endregion

        #region Operator != (Capability1, Capability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Capability1">A capability.</param>
        /// <param name="Capability2">Another capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Capability Capability1,
                                           Capability Capability2)

            => !Capability1.Equals(Capability2);

        #endregion

        #region Operator <  (Capability1, Capability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Capability1">A capability.</param>
        /// <param name="Capability2">Another capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Capability Capability1,
                                          Capability Capability2)

            => Capability1.CompareTo(Capability2) < 0;

        #endregion

        #region Operator <= (Capability1, Capability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Capability1">A capability.</param>
        /// <param name="Capability2">Another capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Capability Capability1,
                                           Capability Capability2)

            => Capability1.CompareTo(Capability2) <= 0;

        #endregion

        #region Operator >  (Capability1, Capability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Capability1">A capability.</param>
        /// <param name="Capability2">Another capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Capability Capability1,
                                          Capability Capability2)

            => Capability1.CompareTo(Capability2) > 0;

        #endregion

        #region Operator >= (Capability1, Capability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Capability1">A capability.</param>
        /// <param name="Capability2">Another capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Capability Capability1,
                                           Capability Capability2)

            => Capability1.CompareTo(Capability2) >= 0;

        #endregion

        #endregion

        #region IComparable<Capability> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two capabilities.
        /// </summary>
        /// <param name="Object">A capability to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Capability meterId
                   ? CompareTo(meterId)
                   : throw new ArgumentException("The given object is not a capability!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Capability)

        /// <summary>
        /// Compares two capabilities.
        /// </summary>
        /// <param name="Capability">A capability to compare with.</param>
        public Int32 CompareTo(Capability Capability)

            => String.Compare(InternalId,
                              Capability.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Capability> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two capabilities for equality.
        /// </summary>
        /// <param name="Object">A capability to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Capability meterId &&
                   Equals(meterId);

        #endregion

        #region Equals(Capability)

        /// <summary>
        /// Compares two capabilities for equality.
        /// </summary>
        /// <param name="Capability">A capability to compare with.</param>
        public Boolean Equals(Capability Capability)

            => String.Equals(InternalId,
                             Capability.InternalId,
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
