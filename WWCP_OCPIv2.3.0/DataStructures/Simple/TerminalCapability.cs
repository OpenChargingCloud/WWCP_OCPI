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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Extension methods for capabilities.
    /// </summary>
    public static class TerminalCapabilityExtensions
    {

        /// <summary>
        /// Indicates whether this terminal capability is null or empty.
        /// </summary>
        /// <param name="TerminalCapability">A terminal capability.</param>
        public static Boolean IsNullOrEmpty(this TerminalCapability? TerminalCapability)
            => !TerminalCapability.HasValue || TerminalCapability.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this terminal capability is NOT null or empty.
        /// </summary>
        /// <param name="TerminalCapability">A terminal capability.</param>
        public static Boolean IsNotNullOrEmpty(this TerminalCapability? TerminalCapability)
            => TerminalCapability.HasValue && TerminalCapability.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// Capabilities or functionalities of an Terminal.
    /// </summary>
    [VendorExtension(VE.GraphDefined)]
    public readonly struct TerminalCapability : IId<TerminalCapability>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this terminal capability is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this terminal capability is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the terminal capability.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new terminal capability based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a terminal capability.</param>
        private TerminalCapability(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a terminal capability.
        /// </summary>
        /// <param name="Text">A text representation of a terminal capability.</param>
        public static TerminalCapability Parse(String Text)
        {

            if (TryParse(Text, out var terminalCapability))
                return terminalCapability;

            throw new ArgumentException($"Invalid text representation of a terminal capability: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a terminal capability.
        /// </summary>
        /// <param name="Text">A text representation of a terminal capability.</param>
        public static TerminalCapability? TryParse(String Text)
        {

            if (TryParse(Text, out var terminalCapability))
                return terminalCapability;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TerminalCapability)

        /// <summary>
        /// Try to parse the given text as a terminal capability.
        /// </summary>
        /// <param name="Text">A text representation of a terminal capability.</param>
        /// <param name="TerminalCapability">The parsed terminal capability.</param>
        public static Boolean TryParse(String Text, out TerminalCapability TerminalCapability)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    TerminalCapability = new TerminalCapability(Text);
                    return true;
                }
                catch
                { }
            }

            TerminalCapability = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this terminal capability.
        /// </summary>
        public TerminalCapability Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The terminal supports price based charging limits.
        /// </summary>
        public static TerminalCapability  CHARGING_LIMIT_PRICE                { get; }
            = new ("CHARGING_LIMIT_PRICE");

        /// <summary>
        /// The terminal supports kWh based charging limits.
        /// </summary>
        public static TerminalCapability  CHARGING_LIMIT_KWH                  { get; }
            = new ("CHARGING_LIMIT_KWH");

        /// <summary>
        /// The terminal supports time based charging limits.
        /// </summary>
        public static TerminalCapability  CHARGING_LIMIT_TIME                 { get; }
            = new ("CHARGING_LIMIT_TIME");


        /// <summary>
        /// The terminal supports chip cards.
        /// </summary>
        public static TerminalCapability  CHIP_CARD_SUPPORT                   { get; }
            = new ("CHIP_CARD_SUPPORT");

        /// <summary>
        /// The terminal supports contactless cards.
        /// </summary>
        public static TerminalCapability  CONTACTLESS_CARD_SUPPORT            { get; }
            = new ("CONTACTLESS_CARD_SUPPORT");

        /// <summary>
        /// The terminal supports contactless payments via smartphone NFC.
        /// </summary>
        public static TerminalCapability  SMARTPHONE_NFC_PAYMENTS_SUPPORT     { get; }
            = new ("SMARTPHONE_NFC_PAYMENTS_SUPPORT");

        /// <summary>
        /// The terminal makes it possible to pay for charging using a credit card.
        /// </summary>
        public static TerminalCapability  CREDIT_CARD_PAYABLE                 { get; }
            = new ("CREDIT_CARD_PAYABLE");

        /// <summary>
        /// The terminal makes it possible to pay for charging using a debit card.
        /// </summary>
        public static TerminalCapability  DEBIT_CARD_PAYABLE                  { get; }
            = new ("DEBIT_CARD_PAYABLE");

        /// <summary>
        /// The terminal has a payment terminal with a pin-code entry device.
        /// </summary>
        public static TerminalCapability  PED_TERMINAL                        { get; }
            = new ("PED_TERMINAL");


        /// <summary>
        /// The terminal supports a remote display, e.g. on your smart phone.
        /// </summary>
        public static TerminalCapability  REMOTE_DISPLAY                      { get; }
            = new ("REMOTE_DISPLAY");

        #endregion


        #region Operator overloading

        #region Operator == (TerminalCapability1, TerminalCapability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TerminalCapability1">A terminal capability.</param>
        /// <param name="TerminalCapability2">Another terminal capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TerminalCapability TerminalCapability1,
                                           TerminalCapability TerminalCapability2)

            => TerminalCapability1.Equals(TerminalCapability2);

        #endregion

        #region Operator != (TerminalCapability1, TerminalCapability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TerminalCapability1">A terminal capability.</param>
        /// <param name="TerminalCapability2">Another terminal capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TerminalCapability TerminalCapability1,
                                           TerminalCapability TerminalCapability2)

            => !TerminalCapability1.Equals(TerminalCapability2);

        #endregion

        #region Operator <  (TerminalCapability1, TerminalCapability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TerminalCapability1">A terminal capability.</param>
        /// <param name="TerminalCapability2">Another terminal capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TerminalCapability TerminalCapability1,
                                          TerminalCapability TerminalCapability2)

            => TerminalCapability1.CompareTo(TerminalCapability2) < 0;

        #endregion

        #region Operator <= (TerminalCapability1, TerminalCapability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TerminalCapability1">A terminal capability.</param>
        /// <param name="TerminalCapability2">Another terminal capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TerminalCapability TerminalCapability1,
                                           TerminalCapability TerminalCapability2)

            => TerminalCapability1.CompareTo(TerminalCapability2) <= 0;

        #endregion

        #region Operator >  (TerminalCapability1, TerminalCapability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TerminalCapability1">A terminal capability.</param>
        /// <param name="TerminalCapability2">Another terminal capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TerminalCapability TerminalCapability1,
                                          TerminalCapability TerminalCapability2)

            => TerminalCapability1.CompareTo(TerminalCapability2) > 0;

        #endregion

        #region Operator >= (TerminalCapability1, TerminalCapability2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TerminalCapability1">A terminal capability.</param>
        /// <param name="TerminalCapability2">Another terminal capability.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TerminalCapability TerminalCapability1,
                                           TerminalCapability TerminalCapability2)

            => TerminalCapability1.CompareTo(TerminalCapability2) >= 0;

        #endregion

        #endregion

        #region IComparable<TerminalCapability> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two capabilities.
        /// </summary>
        /// <param name="Object">A terminal capability to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TerminalCapability terminalCapability
                   ? CompareTo(terminalCapability)
                   : throw new ArgumentException("The given object is not a terminal capability!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TerminalCapability)

        /// <summary>
        /// Compares two capabilities.
        /// </summary>
        /// <param name="TerminalCapability">A terminal capability to compare with.</param>
        public Int32 CompareTo(TerminalCapability TerminalCapability)

            => String.Compare(InternalId,
                              TerminalCapability.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TerminalCapability> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two capabilities for equality.
        /// </summary>
        /// <param name="Object">A terminal capability to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TerminalCapability terminalCapability &&
                   Equals(terminalCapability);

        #endregion

        #region Equals(TerminalCapability)

        /// <summary>
        /// Compares two capabilities for equality.
        /// </summary>
        /// <param name="TerminalCapability">A terminal capability to compare with.</param>
        public Boolean Equals(TerminalCapability TerminalCapability)

            => String.Equals(InternalId,
                             TerminalCapability.InternalId,
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
