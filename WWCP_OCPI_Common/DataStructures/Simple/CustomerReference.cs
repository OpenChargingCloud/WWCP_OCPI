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
    /// Extension methods for customer references.
    /// </summary>
    public static class CustomerReferenceExtensions
    {

        /// <summary>
        /// Indicates whether this customer reference is null or empty.
        /// </summary>
        /// <param name="CustomerReference">A customer reference.</param>
        public static Boolean IsNullOrEmpty(this Customer_Reference? CustomerReference)
            => !CustomerReference.HasValue || CustomerReference.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this customer reference is NOT null or empty.
        /// </summary>
        /// <param name="CustomerReference">A customer reference.</param>
        public static Boolean IsNotNullOrEmpty(this Customer_Reference? CustomerReference)
            => CustomerReference.HasValue && CustomerReference.Value.IsNotNullOrEmpty;


        #region Matches(CustomerReferences, Match, IgnoreCase = true)

        /// <summary>
        /// Checks whether the given enumeration of EVSE identifications matches the given text.
        /// </summary>
        /// <param name="CustomerReferences">An enumeration of EVSE identifications.</param>
        /// <param name="Match">A text to match.</param>
        /// <param name="IgnoreCase">Whether to ignore the case of the text.</param>
        public static Boolean Matches(this IEnumerable<Customer_Reference>  CustomerReferences,
                                      String                                Match,
                                      Boolean                               IgnoreCase  = true)

            => CustomerReferences.Any(customerReference => IgnoreCase
                                          ? customerReference.ToString().Contains(Match, StringComparison.OrdinalIgnoreCase)
                                          : customerReference.ToString().Contains(Match, StringComparison.Ordinal));

        #endregion


    }


    /// <summary>
    /// The unique official identification of an EVSE.
    /// CiString(36)
    /// </summary>
    public readonly struct Customer_Reference : IId<Customer_Reference>
    {

        #region Data

        /// <summary>
        /// The official identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this customer reference is null or empty.
        /// </summary>
        public Boolean  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this customer reference is NOT null or empty.
        /// </summary>
        public Boolean  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the customer reference.
        /// </summary>
        public UInt64   Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new customer reference based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a customer reference.</param>
        private Customer_Reference(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a customer reference.
        /// </summary>
        /// <param name="Text">A text representation of a customer reference.</param>
        public static Customer_Reference Parse(String Text)
        {

            if (TryParse(Text, out var customerReference))
                return customerReference;

            throw new ArgumentException($"Invalid text representation of a customer reference: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a customer reference.
        /// </summary>
        /// <param name="Text">A text representation of a customer reference.</param>
        public static Customer_Reference? TryParse(String Text)
        {

            if (TryParse(Text, out var customerReference))
                return customerReference;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CustomerReference)

        /// <summary>
        /// Try to parse the given text as a customer reference.
        /// </summary>
        /// <param name="Text">A text representation of a customer reference.</param>
        /// <param name="CustomerReference">The parsed customer reference.</param>
        public static Boolean TryParse(String Text, out Customer_Reference CustomerReference)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CustomerReference = new Customer_Reference(Text);
                    return true;
                }
                catch
                { }
            }

            CustomerReference = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this customer reference.
        /// </summary>
        public Customer_Reference Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (CustomerReference1, CustomerReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerReference1">A customer reference.</param>
        /// <param name="CustomerReference2">Another customer reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Customer_Reference CustomerReference1,
                                           Customer_Reference CustomerReference2)

            => CustomerReference1.Equals(CustomerReference2);

        #endregion

        #region Operator != (CustomerReference1, CustomerReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerReference1">A customer reference.</param>
        /// <param name="CustomerReference2">Another customer reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Customer_Reference CustomerReference1,
                                           Customer_Reference CustomerReference2)

            => !CustomerReference1.Equals(CustomerReference2);

        #endregion

        #region Operator <  (CustomerReference1, CustomerReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerReference1">A customer reference.</param>
        /// <param name="CustomerReference2">Another customer reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Customer_Reference CustomerReference1,
                                          Customer_Reference CustomerReference2)

            => CustomerReference1.CompareTo(CustomerReference2) < 0;

        #endregion

        #region Operator <= (CustomerReference1, CustomerReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerReference1">A customer reference.</param>
        /// <param name="CustomerReference2">Another customer reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Customer_Reference CustomerReference1,
                                           Customer_Reference CustomerReference2)

            => CustomerReference1.CompareTo(CustomerReference2) <= 0;

        #endregion

        #region Operator >  (CustomerReference1, CustomerReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerReference1">A customer reference.</param>
        /// <param name="CustomerReference2">Another customer reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Customer_Reference CustomerReference1,
                                          Customer_Reference CustomerReference2)

            => CustomerReference1.CompareTo(CustomerReference2) > 0;

        #endregion

        #region Operator >= (CustomerReference1, CustomerReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerReference1">A customer reference.</param>
        /// <param name="CustomerReference2">Another customer reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Customer_Reference CustomerReference1,
                                           Customer_Reference CustomerReference2)

            => CustomerReference1.CompareTo(CustomerReference2) >= 0;

        #endregion

        #endregion

        #region IComparable<CustomerReference> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two customer references.
        /// </summary>
        /// <param name="Object">A customer reference to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Customer_Reference CustomerReference
                   ? CompareTo(CustomerReference)
                   : throw new ArgumentException("The given object is not a customer reference!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CustomerReference)

        /// <summary>
        /// Compares two customer references.
        /// </summary>
        /// <param name="CustomerReference">A customer reference to compare with.</param>
        public Int32 CompareTo(Customer_Reference CustomerReference)

            => String.Compare(InternalId,
                              CustomerReference.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CustomerReference> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two customer references for equality.
        /// </summary>
        /// <param name="Object">A customer reference to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Customer_Reference CustomerReference &&
                   Equals(CustomerReference);

        #endregion

        #region Equals(CustomerReference)

        /// <summary>
        /// Compares two customer references for equality.
        /// </summary>
        /// <param name="CustomerReference">A customer reference to compare with.</param>
        public Boolean Equals(Customer_Reference CustomerReference)

            => String.Equals(InternalId,
                             CustomerReference.InternalId,
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
