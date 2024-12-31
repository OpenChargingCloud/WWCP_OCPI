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
    /// Extension methods for invoice identifications.
    /// </summary>
    public static class InvoiceIdExtensions
    {

        /// <summary>
        /// Indicates whether this invoice identification is null or empty.
        /// </summary>
        /// <param name="InvoiceId">An invoice identification.</param>
        public static Boolean IsNullOrEmpty(this Invoice_Id? InvoiceId)
            => !InvoiceId.HasValue || InvoiceId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this invoice identification is NOT null or empty.
        /// </summary>
        /// <param name="InvoiceId">An invoice identification.</param>
        public static Boolean IsNotNullOrEmpty(this Invoice_Id? InvoiceId)
            => InvoiceId.HasValue && InvoiceId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of an invoice.
    /// </summary>
    public readonly struct Invoice_Id : IId<Invoice_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this invoice identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this invoice identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the invoice identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new invoice identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an invoice identification.</param>
        private Invoice_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an invoice identification.
        /// </summary>
        /// <param name="Text">A text representation of an invoice identification.</param>
        public static Invoice_Id Parse(String Text)
        {

            if (TryParse(Text, out var invoiceId))
                return invoiceId;

            throw new ArgumentException($"Invalid text representation of an invoice identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an invoice identification.
        /// </summary>
        /// <param name="Text">A text representation of an invoice identification.</param>
        public static Invoice_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var invoiceId))
                return invoiceId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out InvoiceId)

        /// <summary>
        /// Try to parse the given text as an invoice identification.
        /// </summary>
        /// <param name="Text">A text representation of an invoice identification.</param>
        /// <param name="InvoiceId">The parsed invoice identification.</param>
        public static Boolean TryParse(String Text, out Invoice_Id InvoiceId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    InvoiceId = new Invoice_Id(Text);
                    return true;
                }
                catch
                { }
            }

            InvoiceId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this invoice identification.
        /// </summary>
        public Invoice_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (InvoiceId1, InvoiceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceId1">An invoice identification.</param>
        /// <param name="InvoiceId2">Another invoice identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Invoice_Id InvoiceId1,
                                           Invoice_Id InvoiceId2)

            => InvoiceId1.Equals(InvoiceId2);

        #endregion

        #region Operator != (InvoiceId1, InvoiceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceId1">An invoice identification.</param>
        /// <param name="InvoiceId2">Another invoice identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Invoice_Id InvoiceId1,
                                           Invoice_Id InvoiceId2)

            => !InvoiceId1.Equals(InvoiceId2);

        #endregion

        #region Operator <  (InvoiceId1, InvoiceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceId1">An invoice identification.</param>
        /// <param name="InvoiceId2">Another invoice identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Invoice_Id InvoiceId1,
                                          Invoice_Id InvoiceId2)

            => InvoiceId1.CompareTo(InvoiceId2) < 0;

        #endregion

        #region Operator <= (InvoiceId1, InvoiceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceId1">An invoice identification.</param>
        /// <param name="InvoiceId2">Another invoice identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Invoice_Id InvoiceId1,
                                           Invoice_Id InvoiceId2)

            => InvoiceId1.CompareTo(InvoiceId2) <= 0;

        #endregion

        #region Operator >  (InvoiceId1, InvoiceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceId1">An invoice identification.</param>
        /// <param name="InvoiceId2">Another invoice identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Invoice_Id InvoiceId1,
                                          Invoice_Id InvoiceId2)

            => InvoiceId1.CompareTo(InvoiceId2) > 0;

        #endregion

        #region Operator >= (InvoiceId1, InvoiceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceId1">An invoice identification.</param>
        /// <param name="InvoiceId2">Another invoice identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Invoice_Id InvoiceId1,
                                           Invoice_Id InvoiceId2)

            => InvoiceId1.CompareTo(InvoiceId2) >= 0;

        #endregion

        #endregion

        #region IComparable<InvoiceId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two invoice identifications.
        /// </summary>
        /// <param name="Object">An invoice identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Invoice_Id invoiceId
                   ? CompareTo(invoiceId)
                   : throw new ArgumentException("The given object is not an invoice identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(InvoiceId)

        /// <summary>
        /// Compares two invoice identifications.
        /// </summary>
        /// <param name="InvoiceId">An invoice identification to compare with.</param>
        public Int32 CompareTo(Invoice_Id InvoiceId)

            => String.Compare(InternalId,
                              InvoiceId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<InvoiceId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two invoice identifications for equality.
        /// </summary>
        /// <param name="Object">An invoice identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Invoice_Id invoiceId &&
                   Equals(invoiceId);

        #endregion

        #region Equals(InvoiceId)

        /// <summary>
        /// Compares two invoice identifications for equality.
        /// </summary>
        /// <param name="InvoiceId">An invoice identification to compare with.</param>
        public Boolean Equals(Invoice_Id InvoiceId)

            => String.Equals(InternalId,
                             InvoiceId.InternalId,
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
