/*
 * Copyright (c) 2014--2021 GraphDefined GmbH
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

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The unique identification of an invoice reference.
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
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty

            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the invoice reference.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new invoice reference based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the invoice reference.</param>
        private InvoiceReference_Id(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an invoice reference.
        /// </summary>
        /// <param name="Text">A text representation of an invoice reference.</param>
        public static InvoiceReference_Id Parse(String Text)
        {

            if (TryParse(Text, out InvoiceReference_Id invoiceReferenceId))
                return invoiceReferenceId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an invoice reference must not be null or empty!");

            throw new ArgumentException("The given text representation of an invoice reference is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an invoice reference.
        /// </summary>
        /// <param name="Text">A text representation of an invoice reference.</param>
        public static InvoiceReference_Id? TryParse(String Text)
        {

            if (TryParse(Text, out InvoiceReference_Id invoiceReferenceId))
                return invoiceReferenceId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out InvoiceReference_Id)

        /// <summary>
        /// Try to parse the given text as an invoice reference.
        /// </summary>
        /// <param name="Text">A text representation of an invoice reference.</param>
        /// <param name="InvoiceReference_Id">The parsed invoice reference.</param>
        public static Boolean TryParse(String Text, out InvoiceReference_Id InvoiceReference_Id)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    InvoiceReference_Id = new InvoiceReference_Id(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            InvoiceReference_Id = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this invoice reference.
        /// </summary>
        public InvoiceReference_Id Clone

            => new InvoiceReference_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (InvoiceReference_Id1, InvoiceReference_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReference_Id1">A invoice reference.</param>
        /// <param name="InvoiceReference_Id2">Another invoice reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (InvoiceReference_Id InvoiceReference_Id1,
                                           InvoiceReference_Id InvoiceReference_Id2)

            => InvoiceReference_Id1.Equals(InvoiceReference_Id2);

        #endregion

        #region Operator != (InvoiceReference_Id1, InvoiceReference_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReference_Id1">A invoice reference.</param>
        /// <param name="InvoiceReference_Id2">Another invoice reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (InvoiceReference_Id InvoiceReference_Id1,
                                           InvoiceReference_Id InvoiceReference_Id2)

            => !(InvoiceReference_Id1 == InvoiceReference_Id2);

        #endregion

        #region Operator <  (InvoiceReference_Id1, InvoiceReference_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReference_Id1">A invoice reference.</param>
        /// <param name="InvoiceReference_Id2">Another invoice reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (InvoiceReference_Id InvoiceReference_Id1,
                                          InvoiceReference_Id InvoiceReference_Id2)

            => InvoiceReference_Id1.CompareTo(InvoiceReference_Id2) < 0;

        #endregion

        #region Operator <= (InvoiceReference_Id1, InvoiceReference_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReference_Id1">A invoice reference.</param>
        /// <param name="InvoiceReference_Id2">Another invoice reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (InvoiceReference_Id InvoiceReference_Id1,
                                           InvoiceReference_Id InvoiceReference_Id2)

            => !(InvoiceReference_Id1 > InvoiceReference_Id2);

        #endregion

        #region Operator >  (InvoiceReference_Id1, InvoiceReference_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReference_Id1">A invoice reference.</param>
        /// <param name="InvoiceReference_Id2">Another invoice reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (InvoiceReference_Id InvoiceReference_Id1,
                                          InvoiceReference_Id InvoiceReference_Id2)

            => InvoiceReference_Id1.CompareTo(InvoiceReference_Id2) > 0;

        #endregion

        #region Operator >= (InvoiceReference_Id1, InvoiceReference_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReference_Id1">A invoice reference.</param>
        /// <param name="InvoiceReference_Id2">Another invoice reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (InvoiceReference_Id InvoiceReference_Id1,
                                           InvoiceReference_Id InvoiceReference_Id2)

            => !(InvoiceReference_Id1 < InvoiceReference_Id2);

        #endregion

        #endregion

        #region IComparable<InvoiceReference_Id> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is InvoiceReference_Id invoiceReferenceId
                   ? CompareTo(invoiceReferenceId)
                   : throw new ArgumentException("The given object is not an invoice reference identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(InvoiceReference_Id)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceReference_Id">An object to compare with.</param>
        public Int32 CompareTo(InvoiceReference_Id InvoiceReference_Id)

            => String.Compare(InternalId,
                              InvoiceReference_Id.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<InvoiceReference_Id> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is InvoiceReference_Id invoiceReferenceId &&
                   Equals(invoiceReferenceId);

        #endregion

        #region Equals(InvoiceReference_Id)

        /// <summary>
        /// Compares two invoice references for equality.
        /// </summary>
        /// <param name="InvoiceReference_Id">An invoice reference to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(InvoiceReference_Id InvoiceReference_Id)

            => String.Equals(InternalId,
                             InvoiceReference_Id.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

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
