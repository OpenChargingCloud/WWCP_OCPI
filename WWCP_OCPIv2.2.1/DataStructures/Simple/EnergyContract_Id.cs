/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, EnergyContract 2.0 (the "License");
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

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// Extension methods for energy contract identifications.
    /// </summary>
    public static class EnergyContractIdExtensions
    {

        /// <summary>
        /// Indicates whether this energy contract identification is null or empty.
        /// </summary>
        /// <param name="EnergyContractId">An energy contract identification.</param>
        public static Boolean IsNullOrEmpty(this EnergyContract_Id? EnergyContractId)
            => !EnergyContractId.HasValue || EnergyContractId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this energy contract identification is NOT null or empty.
        /// </summary>
        /// <param name="EnergyContractId">An energy contract identification.</param>
        public static Boolean IsNotNullOrEmpty(this EnergyContract_Id? EnergyContractId)
            => EnergyContractId.HasValue && EnergyContractId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of an energy contract.
    /// string(64)
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
        /// Indicates whether this energy contract identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this energy contract identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the energy contract identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new energy contract identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an energy contract identification.</param>
        private EnergyContract_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an energy contract identification.
        /// </summary>
        /// <param name="Text">A text representation of an energy contract identification.</param>
        public static EnergyContract_Id Parse(String Text)
        {

            if (TryParse(Text, out var energyContractId))
                return energyContractId;

            throw new ArgumentException($"Invalid text representation of an energy contract identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an energy contract identification.
        /// </summary>
        /// <param name="Text">A text representation of an energy contract identification.</param>
        public static EnergyContract_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var energyContractId))
                return energyContractId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EnergyContractId)

        /// <summary>
        /// Try to parse the given text as an energy contract identification.
        /// </summary>
        /// <param name="Text">A text representation of an energy contract identification.</param>
        /// <param name="EnergyContractId">The parsed energy contract identification.</param>
        public static Boolean TryParse(String Text, out EnergyContract_Id EnergyContractId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    EnergyContractId = new EnergyContract_Id(Text);
                    return true;
                }
                catch
                { }
            }

            EnergyContractId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this energy contract identification.
        /// </summary>
        public EnergyContract_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (EnergyContractId1, EnergyContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContractId1">An energy contract identification.</param>
        /// <param name="EnergyContractId2">Another energy contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnergyContract_Id EnergyContractId1,
                                           EnergyContract_Id EnergyContractId2)

            => EnergyContractId1.Equals(EnergyContractId2);

        #endregion

        #region Operator != (EnergyContractId1, EnergyContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContractId1">An energy contract identification.</param>
        /// <param name="EnergyContractId2">Another energy contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnergyContract_Id EnergyContractId1,
                                           EnergyContract_Id EnergyContractId2)

            => !EnergyContractId1.Equals(EnergyContractId2);

        #endregion

        #region Operator <  (EnergyContractId1, EnergyContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContractId1">An energy contract identification.</param>
        /// <param name="EnergyContractId2">Another energy contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EnergyContract_Id EnergyContractId1,
                                          EnergyContract_Id EnergyContractId2)

            => EnergyContractId1.CompareTo(EnergyContractId2) < 0;

        #endregion

        #region Operator <= (EnergyContractId1, EnergyContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContractId1">An energy contract identification.</param>
        /// <param name="EnergyContractId2">Another energy contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EnergyContract_Id EnergyContractId1,
                                           EnergyContract_Id EnergyContractId2)

            => EnergyContractId1.CompareTo(EnergyContractId2) <= 0;

        #endregion

        #region Operator >  (EnergyContractId1, EnergyContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContractId1">An energy contract identification.</param>
        /// <param name="EnergyContractId2">Another energy contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EnergyContract_Id EnergyContractId1,
                                          EnergyContract_Id EnergyContractId2)

            => EnergyContractId1.CompareTo(EnergyContractId2) > 0;

        #endregion

        #region Operator >= (EnergyContractId1, EnergyContractId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContractId1">An energy contract identification.</param>
        /// <param name="EnergyContractId2">Another energy contract identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EnergyContract_Id EnergyContractId1,
                                           EnergyContract_Id EnergyContractId2)

            => EnergyContractId1.CompareTo(EnergyContractId2) >= 0;

        #endregion

        #endregion

        #region IComparable<EnergyContractId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two energy contract identifications.
        /// </summary>
        /// <param name="Object">An energy contract identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EnergyContract_Id energyContractId
                   ? CompareTo(energyContractId)
                   : throw new ArgumentException("The given object is not an energy contract identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnergyContractId)

        /// <summary>
        /// Compares two energy contract identifications.
        /// </summary>
        /// <param name="EnergyContractId">An energy contract identification to compare with.</param>
        public Int32 CompareTo(EnergyContract_Id EnergyContractId)

            => String.Compare(InternalId,
                              EnergyContractId.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<EnergyContractId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two energy contract identifications for equality.
        /// </summary>
        /// <param name="Object">An energy contract identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EnergyContract_Id energyContractId &&
                   Equals(energyContractId);

        #endregion

        #region Equals(EnergyContractId)

        /// <summary>
        /// Compares two energy contract identifications for equality.
        /// </summary>
        /// <param name="EnergyContractId">An energy contract identification to compare with.</param>
        public Boolean Equals(EnergyContract_Id EnergyContractId)

            => String.Equals(InternalId,
                             EnergyContractId.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

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
