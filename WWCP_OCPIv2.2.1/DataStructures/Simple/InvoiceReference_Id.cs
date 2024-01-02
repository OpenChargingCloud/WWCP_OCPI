/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, InvoiceReference 2.0 (the "License");
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

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// Extension methods for invoice reference identifications.
    /// </summary>
    public static class InvoiceReferenceIdExtensions
    {

        /// <summary>
        /// Indicates whether this invoice reference identification is null or empty.
        /// </summary>
        /// <param name="InvoiceReferenceId">An invoice reference identification.</param>
        public static Boolean IsNullOrEmpty(this InvoiceReference_Id? InvoiceReferenceId)
            => !InvoiceReferenceId.HasValue || InvoiceReferenceId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this invoice reference identification is NOT null or empty.
        /// </summary>
        /// <param name="InvoiceReferenceId">An invoice reference identification.</param>
        public static Boolean IsNotNullOrEmpty(this InvoiceReference_Id? InvoiceReferenceId)
            => InvoiceReferenceId.HasValue && InvoiceReferenceId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of an invoice reference.
    /// CiString(39)
    /// </summary>
    public readonly struct InvoiceReference_Id : IId<InvoiceReference_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this invoice reference identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this invoice reference identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the invoice reference identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new invoice reference identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an invoice reference identification.</param>
        private InvoiceReference_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an invoice reference identification.
        /// </summary>
        /// <param name="Text">A text representation of an invoice reference identification.</param>
        public static InvoiceReference_Id Parse(String Text)
        {

            if (TryParse(Text, out var invoiceReferenceId))
                return invoiceReferenceId;

            throw new ArgumentException($"Invalid text representation of an invoice reference identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an invoice reference identification.
        /// </summary>
        /// <param name="Text">A text representation of an invoice reference identification.</param>
        public static InvoiceReference_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var invoiceReferenceId))
                return invoiceReferenceId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out InvoiceReferenceId)

        /// <summary>
        /// Try to parse the given text as an invoice reference identification.
        /// </summary>
        /// <param name="Text">A text representation of an invoice reference identification.</param>
        /// <param name="InvoiceReferenceId">The parsed invoice reference identification.</param>
        public static Boolean TryParse(String Text, out InvoiceReference_Id InvoiceReferenceId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    InvoiceReferenceId = new InvoiceReference_Id(Text);
                    return true;
                }
                catch
                { }
            }

            InvoiceReferenceId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this invoice reference identification.
        /// </summary>
        public InvoiceReference_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (InvoiceReferenceId1, InvoiceReferenceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReferenceId1">An invoice reference identification.</param>
        /// <param name="InvoiceReferenceId2">Another invoice reference identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (InvoiceReference_Id InvoiceReferenceId1,
                                           InvoiceReference_Id InvoiceReferenceId2)

            => InvoiceReferenceId1.Equals(InvoiceReferenceId2);

        #endregion

        #region Operator != (InvoiceReferenceId1, InvoiceReferenceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReferenceId1">An invoice reference identification.</param>
        /// <param name="InvoiceReferenceId2">Another invoice reference identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (InvoiceReference_Id InvoiceReferenceId1,
                                           InvoiceReference_Id InvoiceReferenceId2)

            => !InvoiceReferenceId1.Equals(InvoiceReferenceId2);

        #endregion

        #region Operator <  (InvoiceReferenceId1, InvoiceReferenceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReferenceId1">An invoice reference identification.</param>
        /// <param name="InvoiceReferenceId2">Another invoice reference identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (InvoiceReference_Id InvoiceReferenceId1,
                                          InvoiceReference_Id InvoiceReferenceId2)

            => InvoiceReferenceId1.CompareTo(InvoiceReferenceId2) < 0;

        #endregion

        #region Operator <= (InvoiceReferenceId1, InvoiceReferenceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReferenceId1">An invoice reference identification.</param>
        /// <param name="InvoiceReferenceId2">Another invoice reference identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (InvoiceReference_Id InvoiceReferenceId1,
                                           InvoiceReference_Id InvoiceReferenceId2)

            => InvoiceReferenceId1.CompareTo(InvoiceReferenceId2) <= 0;

        #endregion

        #region Operator >  (InvoiceReferenceId1, InvoiceReferenceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReferenceId1">An invoice reference identification.</param>
        /// <param name="InvoiceReferenceId2">Another invoice reference identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (InvoiceReference_Id InvoiceReferenceId1,
                                          InvoiceReference_Id InvoiceReferenceId2)

            => InvoiceReferenceId1.CompareTo(InvoiceReferenceId2) > 0;

        #endregion

        #region Operator >= (InvoiceReferenceId1, InvoiceReferenceId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReferenceId1">An invoice reference identification.</param>
        /// <param name="InvoiceReferenceId2">Another invoice reference identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (InvoiceReference_Id InvoiceReferenceId1,
                                           InvoiceReference_Id InvoiceReferenceId2)

            => InvoiceReferenceId1.CompareTo(InvoiceReferenceId2) >= 0;

        #endregion

        #endregion

        #region IComparable<InvoiceReferenceId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two invoice reference identifications.
        /// </summary>
        /// <param name="Object">An invoice reference identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is InvoiceReference_Id invoiceReferenceId
                   ? CompareTo(invoiceReferenceId)
                   : throw new ArgumentException("The given object is not an invoice reference identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(InvoiceReferenceId)

        /// <summary>
        /// Compares two invoice reference identifications.
        /// </summary>
        /// <param name="InvoiceReferenceId">An invoice reference identification to compare with.</param>
        public Int32 CompareTo(InvoiceReference_Id InvoiceReferenceId)

            => String.Compare(InternalId,
                              InvoiceReferenceId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<InvoiceReferenceId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two invoice reference identifications for equality.
        /// </summary>
        /// <param name="Object">An invoice reference identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is InvoiceReference_Id invoiceReferenceId &&
                   Equals(invoiceReferenceId);

        #endregion

        #region Equals(InvoiceReferenceId)

        /// <summary>
        /// Compares two invoice reference identifications for equality.
        /// </summary>
        /// <param name="InvoiceReferenceId">An invoice reference identification to compare with.</param>
        public Boolean Equals(InvoiceReference_Id InvoiceReferenceId)

            => String.Equals(InternalId,
                             InvoiceReferenceId.InternalId,
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
