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
    /// Extension methods for invoice creators.
    /// </summary>
    public static class InvoiceCreatorExtensions
    {

        /// <summary>
        /// Indicates whether this invoice creator is null or empty.
        /// </summary>
        /// <param name="InvoiceCreator">An invoice creator.</param>
        public static Boolean IsNullOrEmpty(this Invoice_Creator? InvoiceCreator)
            => !InvoiceCreator.HasValue || InvoiceCreator.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this invoice creator is NOT null or empty.
        /// </summary>
        /// <param name="InvoiceCreator">An invoice creator.</param>
        public static Boolean IsNotNullOrEmpty(this Invoice_Creator? InvoiceCreator)
            => InvoiceCreator.HasValue && InvoiceCreator.Value.IsNotNullOrEmpty;


        #region Matches(InvoiceCreators, Match, IgnoreCase = true)

        /// <summary>
        /// Checks whether the given enumeration of EVSE identifications matches the given text.
        /// </summary>
        /// <param name="InvoiceCreators">An enumeration of EVSE identifications.</param>
        /// <param name="Match">A text to match.</param>
        /// <param name="IgnoreCase">Whether to ignore the case of the text.</param>
        public static Boolean Matches(this IEnumerable<Invoice_Creator>  InvoiceCreators,
                                      String                                Match,
                                      Boolean                               IgnoreCase  = true)

            => InvoiceCreators.Any(invoiceCreator => IgnoreCase
                                          ? invoiceCreator.ToString().Contains(Match, StringComparison.OrdinalIgnoreCase)
                                          : invoiceCreator.ToString().Contains(Match, StringComparison.Ordinal));

        #endregion


    }


    /// <summary>
    /// The unique official identification of an EVSE.
    /// CiString(36)
    /// </summary>
    public readonly struct Invoice_Creator : IId<Invoice_Creator>
    {

        #region Data

        /// <summary>
        /// The official identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this invoice creator is null or empty.
        /// </summary>
        public Boolean  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this invoice creator is NOT null or empty.
        /// </summary>
        public Boolean  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the invoice creator.
        /// </summary>
        public UInt64   Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new invoice creator based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an invoice creator.</param>
        private Invoice_Creator(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an invoice creator.
        /// </summary>
        /// <param name="Text">A text representation of an invoice creator.</param>
        public static Invoice_Creator Parse(String Text)
        {

            if (TryParse(Text, out var invoiceCreator))
                return invoiceCreator;

            throw new ArgumentException($"Invalid text representation of an invoice creator: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an invoice creator.
        /// </summary>
        /// <param name="Text">A text representation of an invoice creator.</param>
        public static Invoice_Creator? TryParse(String Text)
        {

            if (TryParse(Text, out var invoiceCreator))
                return invoiceCreator;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out InvoiceCreator)

        /// <summary>
        /// Try to parse the given text as an invoice creator.
        /// </summary>
        /// <param name="Text">A text representation of an invoice creator.</param>
        /// <param name="InvoiceCreator">The parsed invoice creator.</param>
        public static Boolean TryParse(String Text, out Invoice_Creator InvoiceCreator)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    InvoiceCreator = new Invoice_Creator(Text);
                    return true;
                }
                catch
                { }
            }

            InvoiceCreator = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this invoice creator.
        /// </summary>
        public Invoice_Creator Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The CPO issues the invoice and provides it via the invoice_base_url + authorization_reference.
        /// </summary>
        public static Invoice_Creator  CPO    { get; }
            = new ("CPO");

        /// <summary>
        /// The PTP issues the invoice and directly shows/provides it the eDriver via the payment terminal.
        /// </summary>
        public static Invoice_Creator  PTP    { get; }
            = new ("PTP");

        #endregion


        #region Operator overloading

        #region Operator == (InvoiceCreator1, InvoiceCreator2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceCreator1">An invoice creator.</param>
        /// <param name="InvoiceCreator2">Another invoice creator.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Invoice_Creator InvoiceCreator1,
                                           Invoice_Creator InvoiceCreator2)

            => InvoiceCreator1.Equals(InvoiceCreator2);

        #endregion

        #region Operator != (InvoiceCreator1, InvoiceCreator2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceCreator1">An invoice creator.</param>
        /// <param name="InvoiceCreator2">Another invoice creator.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Invoice_Creator InvoiceCreator1,
                                           Invoice_Creator InvoiceCreator2)

            => !InvoiceCreator1.Equals(InvoiceCreator2);

        #endregion

        #region Operator <  (InvoiceCreator1, InvoiceCreator2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceCreator1">An invoice creator.</param>
        /// <param name="InvoiceCreator2">Another invoice creator.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Invoice_Creator InvoiceCreator1,
                                          Invoice_Creator InvoiceCreator2)

            => InvoiceCreator1.CompareTo(InvoiceCreator2) < 0;

        #endregion

        #region Operator <= (InvoiceCreator1, InvoiceCreator2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceCreator1">An invoice creator.</param>
        /// <param name="InvoiceCreator2">Another invoice creator.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Invoice_Creator InvoiceCreator1,
                                           Invoice_Creator InvoiceCreator2)

            => InvoiceCreator1.CompareTo(InvoiceCreator2) <= 0;

        #endregion

        #region Operator >  (InvoiceCreator1, InvoiceCreator2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceCreator1">An invoice creator.</param>
        /// <param name="InvoiceCreator2">Another invoice creator.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Invoice_Creator InvoiceCreator1,
                                          Invoice_Creator InvoiceCreator2)

            => InvoiceCreator1.CompareTo(InvoiceCreator2) > 0;

        #endregion

        #region Operator >= (InvoiceCreator1, InvoiceCreator2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceCreator1">An invoice creator.</param>
        /// <param name="InvoiceCreator2">Another invoice creator.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Invoice_Creator InvoiceCreator1,
                                           Invoice_Creator InvoiceCreator2)

            => InvoiceCreator1.CompareTo(InvoiceCreator2) >= 0;

        #endregion

        #endregion

        #region IComparable<InvoiceCreator> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two invoice creators.
        /// </summary>
        /// <param name="Object">An invoice creator to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Invoice_Creator InvoiceCreator
                   ? CompareTo(InvoiceCreator)
                   : throw new ArgumentException("The given object is not an invoice creator!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(InvoiceCreator)

        /// <summary>
        /// Compares two invoice creators.
        /// </summary>
        /// <param name="InvoiceCreator">An invoice creator to compare with.</param>
        public Int32 CompareTo(Invoice_Creator InvoiceCreator)

            => String.Compare(InternalId,
                              InvoiceCreator.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<InvoiceCreator> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two invoice creators for equality.
        /// </summary>
        /// <param name="Object">An invoice creator to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Invoice_Creator InvoiceCreator &&
                   Equals(InvoiceCreator);

        #endregion

        #region Equals(InvoiceCreator)

        /// <summary>
        /// Compares two invoice creators for equality.
        /// </summary>
        /// <param name="InvoiceCreator">An invoice creator to compare with.</param>
        public Boolean Equals(Invoice_Creator InvoiceCreator)

            => String.Equals(InternalId,
                             InvoiceCreator.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
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
