/*
 * Copyright (c) 2014--2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The unique identification of a credit reference.
    /// </summary>
    public readonly struct CreditReference_Id : IId<CreditReference_Id>
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
        /// The length of the credit reference.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new credit reference based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the credit reference.</param>
        private CreditReference_Id(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a credit reference.
        /// </summary>
        /// <param name="Text">A text representation of a credit reference.</param>
        public static CreditReference_Id Parse(String Text)
        {

            if (TryParse(Text, out CreditReference_Id creditReferenceId))
                return creditReferenceId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a credit reference must not be null or empty!");

            throw new ArgumentException("The given text representation of a credit reference is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a credit reference.
        /// </summary>
        /// <param name="Text">A text representation of a credit reference.</param>
        public static CreditReference_Id? TryParse(String Text)
        {

            if (TryParse(Text, out CreditReference_Id creditReferenceId))
                return creditReferenceId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CreditReference_Id)

        /// <summary>
        /// Try to parse the given text as a credit reference.
        /// </summary>
        /// <param name="Text">A text representation of a credit reference.</param>
        /// <param name="CreditReference_Id">The parsed credit reference.</param>
        public static Boolean TryParse(String Text, out CreditReference_Id CreditReference_Id)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CreditReference_Id = new CreditReference_Id(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            CreditReference_Id = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this credit reference.
        /// </summary>
        public CreditReference_Id Clone

            => new CreditReference_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (CreditReference_Id1, CreditReference_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CreditReference_Id1">A credit reference.</param>
        /// <param name="CreditReference_Id2">Another credit reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CreditReference_Id CreditReference_Id1,
                                           CreditReference_Id CreditReference_Id2)

            => CreditReference_Id1.Equals(CreditReference_Id2);

        #endregion

        #region Operator != (CreditReference_Id1, CreditReference_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CreditReference_Id1">A credit reference.</param>
        /// <param name="CreditReference_Id2">Another credit reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CreditReference_Id CreditReference_Id1,
                                           CreditReference_Id CreditReference_Id2)

            => !(CreditReference_Id1 == CreditReference_Id2);

        #endregion

        #region Operator <  (CreditReference_Id1, CreditReference_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CreditReference_Id1">A credit reference.</param>
        /// <param name="CreditReference_Id2">Another credit reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CreditReference_Id CreditReference_Id1,
                                          CreditReference_Id CreditReference_Id2)

            => CreditReference_Id1.CompareTo(CreditReference_Id2) < 0;

        #endregion

        #region Operator <= (CreditReference_Id1, CreditReference_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CreditReference_Id1">A credit reference.</param>
        /// <param name="CreditReference_Id2">Another credit reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CreditReference_Id CreditReference_Id1,
                                           CreditReference_Id CreditReference_Id2)

            => !(CreditReference_Id1 > CreditReference_Id2);

        #endregion

        #region Operator >  (CreditReference_Id1, CreditReference_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CreditReference_Id1">A credit reference.</param>
        /// <param name="CreditReference_Id2">Another credit reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CreditReference_Id CreditReference_Id1,
                                          CreditReference_Id CreditReference_Id2)

            => CreditReference_Id1.CompareTo(CreditReference_Id2) > 0;

        #endregion

        #region Operator >= (CreditReference_Id1, CreditReference_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CreditReference_Id1">A credit reference.</param>
        /// <param name="CreditReference_Id2">Another credit reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CreditReference_Id CreditReference_Id1,
                                           CreditReference_Id CreditReference_Id2)

            => !(CreditReference_Id1 < CreditReference_Id2);

        #endregion

        #endregion

        #region IComparable<CreditReference_Id> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is CreditReference_Id creditReferenceId
                   ? CompareTo(creditReferenceId)
                   : throw new ArgumentException("The given object is not a credit reference identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CreditReference_Id)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CreditReference_Id">An object to compare with.</param>
        public Int32 CompareTo(CreditReference_Id CreditReference_Id)

            => String.Compare(InternalId,
                              CreditReference_Id.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CreditReference_Id> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is CreditReference_Id creditReferenceId &&
                   Equals(creditReferenceId);

        #endregion

        #region Equals(CreditReference_Id)

        /// <summary>
        /// Compares two credit references for equality.
        /// </summary>
        /// <param name="CreditReference_Id">An credit reference to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CreditReference_Id CreditReference_Id)

            => String.Equals(InternalId,
                             CreditReference_Id.InternalId,
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
