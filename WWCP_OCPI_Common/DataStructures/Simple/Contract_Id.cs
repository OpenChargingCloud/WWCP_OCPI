/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Contract 2.0 (the "License");
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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for contract identifications.
    /// </summary>
    public static class ContractIdExtensions
    {

        /// <summary>
        /// Indicates whether this contract identification is null or empty.
        /// </summary>
        /// <param name="ContractId">A contract identification.</param>
        public static Boolean IsNullOrEmpty(this Contract_Id? ContractId)
            => !ContractId.HasValue || ContractId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this contract identification is NOT null or empty.
        /// </summary>
        /// <param name="ContractId">A contract identification.</param>
        public static Boolean IsNotNullOrEmpty(this Contract_Id? ContractId)
            => ContractId.HasValue && ContractId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a contract (eMAId).
    /// CiString(36)
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
        /// Indicates whether this contract identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this contract identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

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
        /// Parse the given text as a contract identification.
        /// </summary>
        /// <param name="Text">A text representation of a contract identification.</param>
        public static Contract_Id Parse(String Text)
        {

            if (TryParse(Text, out var contractId))
                return contractId;

            throw new ArgumentException($"Invalid text representation of a contract identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a contract identification.
        /// </summary>
        /// <param name="Text">A text representation of a contract identification.</param>
        public static Contract_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var contractId))
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

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ContractId = new Contract_Id(Text);
                    return true;
                }
                catch
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

            => new (
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

            => !ContractId1.Equals(ContractId2);

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

            => ContractId1.CompareTo(ContractId2) <= 0;

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

            => ContractId1.CompareTo(ContractId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ContractId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two contract identifications.
        /// </summary>
        /// <param name="Object">A contract identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Contract_Id contractId
                   ? CompareTo(contractId)
                   : throw new ArgumentException("The given object is not a contract identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ContractId)

        /// <summary>
        /// Compares two contract identifications.
        /// </summary>
        /// <param name="ContractId">A contract identification to compare with.</param>
        public Int32 CompareTo(Contract_Id ContractId)

            => String.Compare(InternalId,
                              ContractId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ContractId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two contract identifications for equality.
        /// </summary>
        /// <param name="Object">A contract identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Contract_Id contractId &&
                   Equals(contractId);

        #endregion

        #region Equals(ContractId)

        /// <summary>
        /// Compares two contract identifications for equality.
        /// </summary>
        /// <param name="ContractId">A contract identification to compare with.</param>
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
