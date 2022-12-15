/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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
    /// The unique identification of a contract.
    /// </summary>
    public readonly struct Contract_Id : IId<Contract_Id>

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
        /// The length of the contract identification.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new contract identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a contract identification.</param>
        private Contract_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a contract identification.
        /// </summary>
        /// <param name="Text">A text representation of a contract identification.</param>
        public static Contract_Id Parse(String Text)
        {

            if (TryParse(Text, out Contract_Id contractId))
                return contractId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a contract identification must not be null or empty!");

            throw new ArgumentException("The given text representation of a contract identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a contract identification.
        /// </summary>
        /// <param name="Text">A text representation of a contract identification.</param>
        public static Contract_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Contract_Id contractId))
                return contractId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ContractId)

        /// <summary>
        /// Try to parse the given text as a contract identification.
        /// </summary>
        /// <param name="Text">A text representation of a contract identification.</param>
        /// <param name="ContractId">The parsed contract identification.</param>
        public static Boolean TryParse(String Text, out Contract_Id ContractId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ContractId = new Contract_Id(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            ContractId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this contract identification.
        /// </summary>
        public Contract_Id Clone

            => new Contract_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (ContractId1, ContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ContractId1">A contract identification.</param>
        /// <param name="ContractId2">Another contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Contract_Id ContractId1,
                                           Contract_Id ContractId2)

            => ContractId1.Equals(ContractId2);

        #endregion

        #region Operator != (ContractId1, ContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ContractId1">A contract identification.</param>
        /// <param name="ContractId2">Another contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Contract_Id ContractId1,
                                           Contract_Id ContractId2)

            => !(ContractId1 == ContractId2);

        #endregion

        #region Operator <  (ContractId1, ContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ContractId1">A contract identification.</param>
        /// <param name="ContractId2">Another contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Contract_Id ContractId1,
                                          Contract_Id ContractId2)

            => ContractId1.CompareTo(ContractId2) < 0;

        #endregion

        #region Operator <= (ContractId1, ContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ContractId1">A contract identification.</param>
        /// <param name="ContractId2">Another contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Contract_Id ContractId1,
                                           Contract_Id ContractId2)

            => !(ContractId1 > ContractId2);

        #endregion

        #region Operator >  (ContractId1, ContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ContractId1">A contract identification.</param>
        /// <param name="ContractId2">Another contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Contract_Id ContractId1,
                                          Contract_Id ContractId2)

            => ContractId1.CompareTo(ContractId2) > 0;

        #endregion

        #region Operator >= (ContractId1, ContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ContractId1">A contract identification.</param>
        /// <param name="ContractId2">Another contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Contract_Id ContractId1,
                                           Contract_Id ContractId2)

            => !(ContractId1 < ContractId2);

        #endregion

        #endregion

        #region IComparable<ContractId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object is Contract_Id contractId)
                return CompareTo(contractId);

            throw new ArgumentException("The given object is not a contract identification!",
                                        nameof(Object));

        }

        #endregion

        #region CompareTo(ContractId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ContractId">An object to compare with.</param>
        public Int32 CompareTo(Contract_Id ContractId)

            => String.Compare(InternalId,
                              ContractId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ContractId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is Contract_Id contractId)
                return Equals(contractId);

            return false;

        }

        #endregion

        #region Equals(ContractId)

        /// <summary>
        /// Compares two contract identifications for equality.
        /// </summary>
        /// <param name="ContractId">An contract identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Contract_Id ContractId)

            => String.Equals(InternalId,
                             ContractId.InternalId,
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
