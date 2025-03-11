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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for PaymentMethods.
    /// </summary>
    public static class PaymentMethodExtensions
    {

        /// <summary>
        /// Indicates whether this PaymentMethod is null or empty.
        /// </summary>
        /// <param name="PaymentMethod">A PaymentMethod.</param>
        public static Boolean IsNullOrEmpty(this PaymentMethod? PaymentMethod)
            => !PaymentMethod.HasValue || PaymentMethod.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this PaymentMethod is null or empty.
        /// </summary>
        /// <param name="PaymentMethod">A PaymentMethod.</param>
        public static Boolean IsNotNullOrEmpty(this PaymentMethod? PaymentMethod)
            => PaymentMethod.HasValue && PaymentMethod.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A PaymentMethod.
    /// </summary>
    public readonly struct PaymentMethod : IId,
                                           IEquatable<PaymentMethod>,
                                           IComparable<PaymentMethod>
    {

        #region Data

        private readonly static Dictionary<String, PaymentMethod>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                              InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this PaymentMethod is null or empty.
        /// </summary>
        public readonly  Boolean                     IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this PaymentMethod is NOT null or empty.
        /// </summary>
        public readonly  Boolean                     IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the PaymentMethod.
        /// </summary>
        public readonly  UInt64                      Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered PaymentMethods.
        /// </summary>
        public static    IEnumerable<PaymentMethod>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new PaymentMethod based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a PaymentMethod.</param>
        private PaymentMethod(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static PaymentMethod Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new PaymentMethod(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a PaymentMethod.
        /// </summary>
        /// <param name="Text">A text representation of a PaymentMethod.</param>
        public static PaymentMethod Parse(String Text)
        {

            if (TryParse(Text, out var paymentMethod))
                return paymentMethod;

            throw new ArgumentException($"Invalid text representation of a PaymentMethod: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a PaymentMethod.
        /// </summary>
        /// <param name="Text">A text representation of a PaymentMethod.</param>
        public static PaymentMethod? TryParse(String Text)
        {

            if (TryParse(Text, out var paymentMethod))
                return paymentMethod;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out PaymentMethod)

        /// <summary>
        /// Try to parse the given text as a PaymentMethod.
        /// </summary>
        /// <param name="Text">A text representation of a PaymentMethod.</param>
        /// <param name="PaymentMethod">The parsed PaymentMethod.</param>
        public static Boolean TryParse(String Text, out PaymentMethod PaymentMethod)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out PaymentMethod))
                    PaymentMethod = Register(Text);

                return true;

            }

            PaymentMethod = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this PaymentMethod.
        /// </summary>
        public PaymentMethod Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Payment Credit Card
        /// </summary>
        public static PaymentMethod  PaymentCreditCard    { get; }
            = Register("PaymentCreditCard");

        /// <summary>
        /// Cash Bills Only
        /// </summary>
        public static PaymentMethod  CashBillsOnly        { get; }
            = Register("CashBillsOnly");

        /// <summary>
        /// Cash Coins Only
        /// </summary>
        public static PaymentMethod  CashCoinsOnly        { get; }
            = Register("CashCoinsOnly");

        /// <summary>
        /// Toll Tag
        /// </summary>
        public static PaymentMethod  TollTag              { get; }
            = Register("TollTag");

        /// <summary>
        /// Mobile Account
        /// </summary>
        public static PaymentMethod  MobileAccount        { get; }
            = Register("MobileAccount");

        /// <summary>
        /// Cash, Coins and Bills
        /// </summary>
        public static PaymentMethod  CashCoinsAndBills    { get; }
            = Register("CashCoinsAndBills");

        /// <summary>
        /// Prepay
        /// </summary>
        public static PaymentMethod  Prepay               { get; }
            = Register("Prepay");

        /// <summary>
        /// Payment Debit Card
        /// </summary>
        public static PaymentMethod  PaymentDebitCard     { get; }
            = Register("PaymentDebitCard");

        /// <summary>
        /// Payment Value Card
        /// </summary>
        public static PaymentMethod  PaymentValueCard     { get; }
            = Register("PaymentValueCard");

        /// <summary>
        /// Near Field Communication
        /// </summary>
        public static PaymentMethod  NFC                  { get; }
            = Register("NFC");

        /// <summary>
        /// EMV
        /// </summary>
        public static PaymentMethod  EMV                  { get; }
            = Register("EMV");

        /// <summary>
        /// QR-Code
        /// </summary>
        public static PaymentMethod  QRCode               { get; }
            = Register("QRCode");

        /// <summary>
        /// Website
        /// </summary>
        public static PaymentMethod  Website              { get; }
            = Register("Website");

        /// <summary>
        /// Unknown
        /// </summary>
        public static PaymentMethod  Unknown              { get; }
            = Register("unknown");

        #endregion


        #region Operator overloading

        #region Operator == (PaymentMethod1, PaymentMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentMethod1">A PaymentMethod.</param>
        /// <param name="PaymentMethod2">Another PaymentMethod.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PaymentMethod PaymentMethod1,
                                           PaymentMethod PaymentMethod2)

            => PaymentMethod1.Equals(PaymentMethod2);

        #endregion

        #region Operator != (PaymentMethod1, PaymentMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentMethod1">A PaymentMethod.</param>
        /// <param name="PaymentMethod2">Another PaymentMethod.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PaymentMethod PaymentMethod1,
                                           PaymentMethod PaymentMethod2)

            => !PaymentMethod1.Equals(PaymentMethod2);

        #endregion

        #region Operator <  (PaymentMethod1, PaymentMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentMethod1">A PaymentMethod.</param>
        /// <param name="PaymentMethod2">Another PaymentMethod.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PaymentMethod PaymentMethod1,
                                          PaymentMethod PaymentMethod2)

            => PaymentMethod1.CompareTo(PaymentMethod2) < 0;

        #endregion

        #region Operator <= (PaymentMethod1, PaymentMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentMethod1">A PaymentMethod.</param>
        /// <param name="PaymentMethod2">Another PaymentMethod.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PaymentMethod PaymentMethod1,
                                           PaymentMethod PaymentMethod2)

            => PaymentMethod1.CompareTo(PaymentMethod2) <= 0;

        #endregion

        #region Operator >  (PaymentMethod1, PaymentMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentMethod1">A PaymentMethod.</param>
        /// <param name="PaymentMethod2">Another PaymentMethod.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PaymentMethod PaymentMethod1,
                                          PaymentMethod PaymentMethod2)

            => PaymentMethod1.CompareTo(PaymentMethod2) > 0;

        #endregion

        #region Operator >= (PaymentMethod1, PaymentMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentMethod1">A PaymentMethod.</param>
        /// <param name="PaymentMethod2">Another PaymentMethod.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PaymentMethod PaymentMethod1,
                                           PaymentMethod PaymentMethod2)

            => PaymentMethod1.CompareTo(PaymentMethod2) >= 0;

        #endregion

        #endregion

        #region IComparable<PaymentMethod> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two PaymentMethods.
        /// </summary>
        /// <param name="Object">A PaymentMethod to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PaymentMethod paymentMethod
                   ? CompareTo(paymentMethod)
                   : throw new ArgumentException("The given object is not a PaymentMethod!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PaymentMethod)

        /// <summary>
        /// Compares two PaymentMethods.
        /// </summary>
        /// <param name="PaymentMethod">A PaymentMethod to compare with.</param>
        public Int32 CompareTo(PaymentMethod PaymentMethod)

            => String.Compare(InternalId,
                              PaymentMethod.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<PaymentMethod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two PaymentMethods for equality.
        /// </summary>
        /// <param name="Object">A PaymentMethod to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PaymentMethod paymentMethod &&
                   Equals(paymentMethod);

        #endregion

        #region Equals(PaymentMethod)

        /// <summary>
        /// Compares two PaymentMethods for equality.
        /// </summary>
        /// <param name="PaymentMethod">A PaymentMethod to compare with.</param>
        public Boolean Equals(PaymentMethod PaymentMethod)

            => String.Equals(InternalId,
                             PaymentMethod.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

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
