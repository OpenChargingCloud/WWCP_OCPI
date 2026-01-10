/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extension methods for PaymentBrands.
    /// </summary>
    public static class PaymentBrandExtensions
    {

        /// <summary>
        /// Indicates whether this PaymentBrand is null or empty.
        /// </summary>
        /// <param name="PaymentBrand">A PaymentBrand.</param>
        public static Boolean IsNullOrEmpty(this PaymentBrand? PaymentBrand)
            => !PaymentBrand.HasValue || PaymentBrand.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this PaymentBrand is null or empty.
        /// </summary>
        /// <param name="PaymentBrand">A PaymentBrand.</param>
        public static Boolean IsNotNullOrEmpty(this PaymentBrand? PaymentBrand)
            => PaymentBrand.HasValue && PaymentBrand.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A PaymentBrand.
    /// </summary>
    public readonly struct PaymentBrand : IId,
                                          IEquatable<PaymentBrand>,
                                          IComparable<PaymentBrand>
    {

        #region Data

        private readonly static Dictionary<String, PaymentBrand>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                            InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this PaymentBrand is null or empty.
        /// </summary>
        public readonly  Boolean                    IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this PaymentBrand is NOT null or empty.
        /// </summary>
        public readonly  Boolean                    IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the PaymentBrand.
        /// </summary>
        public readonly  UInt64                     Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered PaymentBrands.
        /// </summary>
        public static    IEnumerable<PaymentBrand>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new PaymentBrand based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a PaymentBrand.</param>
        private PaymentBrand(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static PaymentBrand Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new PaymentBrand(Text)
               );

        #endregion


        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given string as a PaymentBrand.
        /// </summary>
        /// <param name="Text">A text representation of a PaymentBrand.</param>
        public static PaymentBrand Parse(String Text)
        {

            if (TryParse(Text, out var paymentBrand))
                return paymentBrand;

            throw new ArgumentException($"Invalid text representation of a PaymentBrand: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a PaymentBrand.
        /// </summary>
        /// <param name="Text">A text representation of a PaymentBrand.</param>
        public static PaymentBrand? TryParse(String Text)
        {

            if (TryParse(Text, out var paymentBrand))
                return paymentBrand;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out PaymentBrand)

        /// <summary>
        /// Try to parse the given text as a PaymentBrand.
        /// </summary>
        /// <param name="Text">A text representation of a PaymentBrand.</param>
        /// <param name="PaymentBrand">The parsed PaymentBrand.</param>
        public static Boolean TryParse(String Text, out PaymentBrand PaymentBrand)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out PaymentBrand))
                    PaymentBrand = Register(Text);

                return true;

            }

            PaymentBrand = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this PaymentBrand.
        /// </summary>
        public PaymentBrand Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// American Express
        /// </summary>
        public static PaymentBrand  AmericanExpress    { get; }
            = Register("American Express");

        /// <summary>
        /// Apple pay
        /// </summary>
        public static PaymentBrand  ApplePay           { get; }
            = Register("ApplePay");

        /// <summary>
        /// Cirrus
        /// </summary>
        public static PaymentBrand  Cirrus             { get; }
            = Register("Cirrus");

        /// <summary>
        /// Diners Club
        /// </summary>
        public static PaymentBrand  DinersClub         { get; }
            = Register("Diners Club");

        /// <summary>
        /// Discover Card
        /// </summary>
        public static PaymentBrand  DiscoverCard       { get; }
            = Register("Discover Card");

        /// <summary>
        /// Giro card
        /// </summary>
        public static PaymentBrand  GiroCard           { get; }
            = Register("GiroCard");

        /// <summary>
        /// Maestro
        /// </summary>
        public static PaymentBrand  Maestro            { get; }
            = Register("Maestro");

        /// <summary>
        /// Master Card
        /// </summary>
        public static PaymentBrand  MasterCard         { get; }
            = Register("MasterCard");

        /// <summary>
        /// Visa
        /// </summary>
        public static PaymentBrand  Visa               { get; }
            = Register("VISA");

        /// <summary>
        /// VPay
        /// </summary>
        public static PaymentBrand  VPay               { get; }
            = Register("VPay");

        /// <summary>
        /// Another brand
        /// </summary>
        public static PaymentBrand  Other              { get; }
            = Register("other");

        #endregion


        #region Operator overloading

        #region Operator == (PaymentBrand1, PaymentBrand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentBrand1">A PaymentBrand.</param>
        /// <param name="PaymentBrand2">Another PaymentBrand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PaymentBrand PaymentBrand1,
                                           PaymentBrand PaymentBrand2)

            => PaymentBrand1.Equals(PaymentBrand2);

        #endregion

        #region Operator != (PaymentBrand1, PaymentBrand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentBrand1">A PaymentBrand.</param>
        /// <param name="PaymentBrand2">Another PaymentBrand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PaymentBrand PaymentBrand1,
                                           PaymentBrand PaymentBrand2)

            => !PaymentBrand1.Equals(PaymentBrand2);

        #endregion

        #region Operator <  (PaymentBrand1, PaymentBrand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentBrand1">A PaymentBrand.</param>
        /// <param name="PaymentBrand2">Another PaymentBrand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PaymentBrand PaymentBrand1,
                                          PaymentBrand PaymentBrand2)

            => PaymentBrand1.CompareTo(PaymentBrand2) < 0;

        #endregion

        #region Operator <= (PaymentBrand1, PaymentBrand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentBrand1">A PaymentBrand.</param>
        /// <param name="PaymentBrand2">Another PaymentBrand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PaymentBrand PaymentBrand1,
                                           PaymentBrand PaymentBrand2)

            => PaymentBrand1.CompareTo(PaymentBrand2) <= 0;

        #endregion

        #region Operator >  (PaymentBrand1, PaymentBrand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentBrand1">A PaymentBrand.</param>
        /// <param name="PaymentBrand2">Another PaymentBrand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PaymentBrand PaymentBrand1,
                                          PaymentBrand PaymentBrand2)

            => PaymentBrand1.CompareTo(PaymentBrand2) > 0;

        #endregion

        #region Operator >= (PaymentBrand1, PaymentBrand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentBrand1">A PaymentBrand.</param>
        /// <param name="PaymentBrand2">Another PaymentBrand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PaymentBrand PaymentBrand1,
                                           PaymentBrand PaymentBrand2)

            => PaymentBrand1.CompareTo(PaymentBrand2) >= 0;

        #endregion

        #endregion

        #region IComparable<PaymentBrand> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two PaymentBrands.
        /// </summary>
        /// <param name="Object">A PaymentBrand to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PaymentBrand paymentBrand
                   ? CompareTo(paymentBrand)
                   : throw new ArgumentException("The given object is not a PaymentBrand!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PaymentBrand)

        /// <summary>
        /// Compares two PaymentBrands.
        /// </summary>
        /// <param name="PaymentBrand">A PaymentBrand to compare with.</param>
        public Int32 CompareTo(PaymentBrand PaymentBrand)

            => String.Compare(InternalId,
                              PaymentBrand.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<PaymentBrand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two PaymentBrands for equality.
        /// </summary>
        /// <param name="Object">A PaymentBrand to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PaymentBrand paymentBrand &&
                   Equals(paymentBrand);

        #endregion

        #region Equals(PaymentBrand)

        /// <summary>
        /// Compares two PaymentBrands for equality.
        /// </summary>
        /// <param name="PaymentBrand">A PaymentBrand to compare with.</param>
        public Boolean Equals(PaymentBrand PaymentBrand)

            => String.Equals(InternalId,
                             PaymentBrand.InternalId,
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
