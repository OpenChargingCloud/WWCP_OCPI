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
    /// The unique identification of an energy contract.
    /// </summary>
    public readonly struct EnergyContract_Id : IId<EnergyContract_Id>

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
        private EnergyContract_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an energy contract identification.
        /// </summary>
        /// <param name="Text">A text representation of an energy contract identification.</param>
        public static EnergyContract_Id Parse(String Text)
        {

            if (TryParse(Text, out EnergyContract_Id energyContractId))
                return energyContractId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an energy contract identification must not be null or empty!");

            throw new ArgumentException("The given text representation of an energy contract identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an energy contract identification.
        /// </summary>
        /// <param name="Text">A text representation of an energy contract identification.</param>
        public static EnergyContract_Id? TryParse(String Text)
        {

            if (TryParse(Text, out EnergyContract_Id energyContractId))
                return energyContractId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EnergyContractId)

        /// <summary>
        /// Try to parse the given text as an energy contract identification.
        /// </summary>
        /// <param name="Text">A text representation of an energy contract identification.</param>
        /// <param name="EnergyContractId">The parsed contract identification.</param>
        public static Boolean TryParse(String Text, out EnergyContract_Id EnergyContractId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    EnergyContractId = new EnergyContract_Id(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            EnergyContractId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this contract identification.
        /// </summary>
        public EnergyContract_Id Clone

            => new EnergyContract_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (EnergyContractId1, EnergyContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContractId1">A contract identification.</param>
        /// <param name="EnergyContractId2">Another contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnergyContract_Id EnergyContractId1,
                                           EnergyContract_Id EnergyContractId2)

            => EnergyContractId1.Equals(EnergyContractId2);

        #endregion

        #region Operator != (EnergyContractId1, EnergyContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContractId1">A contract identification.</param>
        /// <param name="EnergyContractId2">Another contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnergyContract_Id EnergyContractId1,
                                           EnergyContract_Id EnergyContractId2)

            => !(EnergyContractId1 == EnergyContractId2);

        #endregion

        #region Operator <  (EnergyContractId1, EnergyContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContractId1">A contract identification.</param>
        /// <param name="EnergyContractId2">Another contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EnergyContract_Id EnergyContractId1,
                                          EnergyContract_Id EnergyContractId2)

            => EnergyContractId1.CompareTo(EnergyContractId2) < 0;

        #endregion

        #region Operator <= (EnergyContractId1, EnergyContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContractId1">A contract identification.</param>
        /// <param name="EnergyContractId2">Another contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EnergyContract_Id EnergyContractId1,
                                           EnergyContract_Id EnergyContractId2)

            => !(EnergyContractId1 > EnergyContractId2);

        #endregion

        #region Operator >  (EnergyContractId1, EnergyContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContractId1">A contract identification.</param>
        /// <param name="EnergyContractId2">Another contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EnergyContract_Id EnergyContractId1,
                                          EnergyContract_Id EnergyContractId2)

            => EnergyContractId1.CompareTo(EnergyContractId2) > 0;

        #endregion

        #region Operator >= (EnergyContractId1, EnergyContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContractId1">A contract identification.</param>
        /// <param name="EnergyContractId2">Another contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EnergyContract_Id EnergyContractId1,
                                           EnergyContract_Id EnergyContractId2)

            => !(EnergyContractId1 < EnergyContractId2);

        #endregion

        #endregion

        #region IComparable<EnergyContractId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object is EnergyContract_Id energyContractId)
                return CompareTo(energyContractId);

            throw new ArgumentException("The given object is not an energy contract identification!",
                                        nameof(Object));

        }

        #endregion

        #region CompareTo(EnergyContractId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContractId">An object to compare with.</param>
        public Int32 CompareTo(EnergyContract_Id EnergyContractId)

            => String.Compare(InternalId,
                              EnergyContractId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<EnergyContractId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is EnergyContract_Id energyContractId)
                return Equals(energyContractId);

            return false;

        }

        #endregion

        #region Equals(EnergyContractId)

        /// <summary>
        /// Compares two contract identifications for equality.
        /// </summary>
        /// <param name="EnergyContractId">An contract identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EnergyContract_Id EnergyContractId)

            => String.Equals(InternalId,
                             EnergyContractId.InternalId,
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
