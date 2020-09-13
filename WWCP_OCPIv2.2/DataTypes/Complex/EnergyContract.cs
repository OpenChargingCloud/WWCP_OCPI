/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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
    /// An energy delivery contract.
    /// </summary>
    public readonly struct EnergyContract : IEquatable<EnergyContract>,
                                            IComparable<EnergyContract>,
                                            IComparable
    {

        #region Properties

        /// <summary>
        /// The name of the energy supplier for this token.
        /// </summary>
        public String              SupplierName    { get; }

        /// <summary>
        /// The optional contract identification at the energy supplier, that belongs
        /// to the owner of this token.
        /// </summary>
        public EnergyContract_Id?  ContractId      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new energy delivery contracts for an EV driver.
        /// </summary>
        /// <param name="SupplierName">The name of the energy supplier for this token.</param>
        /// <param name="ContractId">The optional contract identification at the energy supplier, that belongs to the owner of this token.</param>
        public EnergyContract(String              SupplierName,
                              EnergyContract_Id?  ContractId)
        {

            #region Initial checks

            if (SupplierName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(SupplierName), "The given energy supplier name must not be null or empty!");

            #endregion

            this.SupplierName  = SupplierName;
            this.ContractId    = ContractId;

        }

        #endregion


        #region Operator overloading

        #region Operator == (EnergyContract1, EnergyContract2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContract1">An energy delivery contract.</param>
        /// <param name="EnergyContract2">Another energy delivery contract.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnergyContract EnergyContract1,
                                           EnergyContract EnergyContract2)

            => EnergyContract1.Equals(EnergyContract2);

        #endregion

        #region Operator != (EnergyContract1, EnergyContract2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContract1">An energy delivery contract.</param>
        /// <param name="EnergyContract2">Another energy delivery contract.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnergyContract EnergyContract1,
                                           EnergyContract EnergyContract2)

            => !(EnergyContract1 == EnergyContract2);

        #endregion

        #region Operator <  (EnergyContract1, EnergyContract2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContract1">An energy delivery contract.</param>
        /// <param name="EnergyContract2">Another energy delivery contract.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EnergyContract EnergyContract1,
                                          EnergyContract EnergyContract2)

            => EnergyContract1.CompareTo(EnergyContract2) < 0;

        #endregion

        #region Operator <= (EnergyContract1, EnergyContract2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContract1">An energy delivery contract.</param>
        /// <param name="EnergyContract2">Another energy delivery contract.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EnergyContract EnergyContract1,
                                           EnergyContract EnergyContract2)

            => !(EnergyContract1 > EnergyContract2);

        #endregion

        #region Operator >  (EnergyContract1, EnergyContract2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContract1">An energy delivery contract.</param>
        /// <param name="EnergyContract2">Another energy delivery contract.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EnergyContract EnergyContract1,
                                          EnergyContract EnergyContract2)

            => EnergyContract1.CompareTo(EnergyContract2) > 0;

        #endregion

        #region Operator >= (EnergyContract1, EnergyContract2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContract1">An energy delivery contract.</param>
        /// <param name="EnergyContract2">Another energy delivery contract.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EnergyContract EnergyContract1,
                                           EnergyContract EnergyContract2)

            => !(EnergyContract1 < EnergyContract2);

        #endregion

        #endregion

        #region IComparable<EnergyContract> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is EnergyContract energyContract
                   ? CompareTo(energyContract)
                   : throw new ArgumentException("The given object is not a energy delivery contract!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnergyContract)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContract">An object to compare with.</param>
        public Int32 CompareTo(EnergyContract EnergyContract)
        {

            var c = SupplierName.CompareTo(EnergyContract.SupplierName);

            if (c == 0 && ContractId.HasValue && EnergyContract.ContractId.HasValue)
                c = ContractId.Value.CompareTo(EnergyContract.ContractId.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EnergyContract> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is EnergyContract energyContract &&
                   Equals(energyContract);

        #endregion

        #region Equals(EnergyContract)

        /// <summary>
        /// Compares two energy delivery contracts for equality.
        /// </summary>
        /// <param name="EnergyContract">An energy delivery contract to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EnergyContract EnergyContract)

            => SupplierName.Equals(EnergyContract.SupplierName) &&

            ((!ContractId.HasValue && !EnergyContract.ContractId.HasValue) ||
              (ContractId.HasValue &&  EnergyContract.ContractId.HasValue && ContractId.Value.Equals(EnergyContract.ContractId.Value)));

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return SupplierName.GetHashCode() ^

                       (ContractId.HasValue
                            ? ContractId.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(SupplierName,
                             ContractId.HasValue
                                 ? " (" + ContractId + ")"
                                 : "");

        #endregion

    }

}
