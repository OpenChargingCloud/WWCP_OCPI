/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for tariff association identifications.
    /// </summary>
    public static class TariffAssociationIdExtensions
    {

        /// <summary>
        /// Indicates whether this tariff association identification is null or empty.
        /// </summary>
        /// <param name="TariffAssociationId">An tariff association identification.</param>
        public static Boolean IsNullOrEmpty(this TariffAssociation_Id? TariffAssociationId)
            => !TariffAssociationId.HasValue || TariffAssociationId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this tariff association identification is NOT null or empty.
        /// </summary>
        /// <param name="TariffAssociationId">An tariff association identification.</param>
        public static Boolean IsNotNullOrEmpty(this TariffAssociation_Id? TariffAssociationId)
            => TariffAssociationId.HasValue && TariffAssociationId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a tariff association.
    /// </summary>
    public readonly struct TariffAssociation_Id : IId<TariffAssociation_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this tariff association identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this tariff association identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the tariff association identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tariff association identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a tariff association identification.</param>
        private TariffAssociation_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a tariff association identification.
        /// </summary>
        /// <param name="Text">A text representation of a tariff association identification.</param>
        public static TariffAssociation_Id Parse(String Text)
        {

            if (TryParse(Text, out var tariffAssociationId))
                return tariffAssociationId;

            throw new ArgumentException($"Invalid text representation of a tariff association identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a tariff association identification.
        /// </summary>
        /// <param name="Text">A text representation of a tariff association identification.</param>
        public static TariffAssociation_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffAssociationId))
                return tariffAssociationId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TariffAssociationId)

        /// <summary>
        /// Try to parse the given text as a tariff association identification.
        /// </summary>
        /// <param name="Text">A text representation of a tariff association identification.</param>
        /// <param name="TariffAssociationId">The parsed tariff association identification.</param>
        public static Boolean TryParse(String Text, out TariffAssociation_Id TariffAssociationId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    TariffAssociationId = new TariffAssociation_Id(Text);
                    return true;
                }
                catch
                { }
            }

            TariffAssociationId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this tariff association identification.
        /// </summary>
        public TariffAssociation_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (TariffAssociationId1, TariffAssociationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssociationId1">An tariff association identification.</param>
        /// <param name="TariffAssociationId2">Another tariff association identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffAssociation_Id TariffAssociationId1,
                                           TariffAssociation_Id TariffAssociationId2)

            => TariffAssociationId1.Equals(TariffAssociationId2);

        #endregion

        #region Operator != (TariffAssociationId1, TariffAssociationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssociationId1">An tariff association identification.</param>
        /// <param name="TariffAssociationId2">Another tariff association identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffAssociation_Id TariffAssociationId1,
                                           TariffAssociation_Id TariffAssociationId2)

            => !TariffAssociationId1.Equals(TariffAssociationId2);

        #endregion

        #region Operator <  (TariffAssociationId1, TariffAssociationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssociationId1">An tariff association identification.</param>
        /// <param name="TariffAssociationId2">Another tariff association identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TariffAssociation_Id TariffAssociationId1,
                                          TariffAssociation_Id TariffAssociationId2)

            => TariffAssociationId1.CompareTo(TariffAssociationId2) < 0;

        #endregion

        #region Operator <= (TariffAssociationId1, TariffAssociationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssociationId1">An tariff association identification.</param>
        /// <param name="TariffAssociationId2">Another tariff association identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TariffAssociation_Id TariffAssociationId1,
                                           TariffAssociation_Id TariffAssociationId2)

            => TariffAssociationId1.CompareTo(TariffAssociationId2) <= 0;

        #endregion

        #region Operator >  (TariffAssociationId1, TariffAssociationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssociationId1">An tariff association identification.</param>
        /// <param name="TariffAssociationId2">Another tariff association identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TariffAssociation_Id TariffAssociationId1,
                                          TariffAssociation_Id TariffAssociationId2)

            => TariffAssociationId1.CompareTo(TariffAssociationId2) > 0;

        #endregion

        #region Operator >= (TariffAssociationId1, TariffAssociationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssociationId1">An tariff association identification.</param>
        /// <param name="TariffAssociationId2">Another tariff association identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TariffAssociation_Id TariffAssociationId1,
                                           TariffAssociation_Id TariffAssociationId2)

            => TariffAssociationId1.CompareTo(TariffAssociationId2) >= 0;

        #endregion

        #endregion

        #region IComparable<TariffAssociationId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tariff association identifications.
        /// </summary>
        /// <param name="Object">An tariff association identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TariffAssociation_Id tariffAssociationId
                   ? CompareTo(tariffAssociationId)
                   : throw new ArgumentException("The given object is not a tariff association identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TariffAssociationId)

        /// <summary>
        /// Compares two tariff association identifications.
        /// </summary>
        /// <param name="TariffAssociationId">An tariff association identification to compare with.</param>
        public Int32 CompareTo(TariffAssociation_Id TariffAssociationId)

            => String.Compare(InternalId,
                              TariffAssociationId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TariffAssociationId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tariff association identifications for equality.
        /// </summary>
        /// <param name="Object">An tariff association identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffAssociation_Id tariffAssociationId &&
                   Equals(tariffAssociationId);

        #endregion

        #region Equals(TariffAssociationId)

        /// <summary>
        /// Compares two tariff association identifications for equality.
        /// </summary>
        /// <param name="TariffAssociationId">An tariff association identification to compare with.</param>
        public Boolean Equals(TariffAssociation_Id TariffAssociationId)

            => String.Equals(InternalId,
                             TariffAssociationId.InternalId,
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
