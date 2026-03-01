/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Tax 2.0 (the "License");
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
    /// Extension methods for tax identifications.
    /// </summary>
    public static class TaxIdExtensions
    {

        /// <summary>
        /// Indicates whether this tax identification is null or empty.
        /// </summary>
        /// <param name="TaxId">A tax identification.</param>
        public static Boolean IsNullOrEmpty(this Tax_Id? TaxId)
            => !TaxId.HasValue || TaxId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this tax identification is NOT null or empty.
        /// </summary>
        /// <param name="TaxId">A tax identification.</param>
        public static Boolean IsNotNullOrEmpty(this Tax_Id? TaxId)
            => TaxId.HasValue && TaxId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a tax.
    /// </summary>
    public readonly struct Tax_Id : IId<Tax_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this tax identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this tax identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the tax identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tax identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a tax identification.</param>
        private Tax_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a tax identification.
        /// </summary>
        /// <param name="Text">A text representation of a tax identification.</param>
        public static Tax_Id Parse(String Text)
        {

            if (TryParse(Text, out var TaxId))
                return TaxId;

            throw new ArgumentException($"Invalid text representation of a tax identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a tax identification.
        /// </summary>
        /// <param name="Text">A text representation of a tax identification.</param>
        public static Tax_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var TaxId))
                return TaxId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out TaxId)

        /// <summary>
        /// Try to parse the given text as a tax identification.
        /// </summary>
        /// <param name="Text">A text representation of a tax identification.</param>
        /// <param name="TaxId">The parsed tax identification.</param>
        public static Boolean TryParse(String Text, out Tax_Id  TaxId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    TaxId = new Tax_Id(Text);
                    return true;
                }
                catch
                { }
            }

            TaxId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this tax identification.
        /// </summary>
        public Tax_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Value Added Tax (VAT).
        /// </summary>
        public static Tax_Id  VAT    { get; }
            = new ("VAT");

        #endregion


        #region Operator overloading

        #region Operator == (TaxId1, TaxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxId1">A tax identification.</param>
        /// <param name="TaxId2">Another tax identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Tax_Id TaxId1,
                                           Tax_Id TaxId2)

            => TaxId1.Equals(TaxId2);

        #endregion

        #region Operator != (TaxId1, TaxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxId1">A tax identification.</param>
        /// <param name="TaxId2">Another tax identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Tax_Id TaxId1,
                                           Tax_Id TaxId2)

            => !TaxId1.Equals(TaxId2);

        #endregion

        #region Operator <  (TaxId1, TaxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxId1">A tax identification.</param>
        /// <param name="TaxId2">Another tax identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Tax_Id TaxId1,
                                          Tax_Id TaxId2)

            => TaxId1.CompareTo(TaxId2) < 0;

        #endregion

        #region Operator <= (TaxId1, TaxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxId1">A tax identification.</param>
        /// <param name="TaxId2">Another tax identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Tax_Id TaxId1,
                                           Tax_Id TaxId2)

            => TaxId1.CompareTo(TaxId2) <= 0;

        #endregion

        #region Operator >  (TaxId1, TaxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxId1">A tax identification.</param>
        /// <param name="TaxId2">Another tax identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Tax_Id TaxId1,
                                          Tax_Id TaxId2)

            => TaxId1.CompareTo(TaxId2) > 0;

        #endregion

        #region Operator >= (TaxId1, TaxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxId1">A tax identification.</param>
        /// <param name="TaxId2">Another tax identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Tax_Id TaxId1,
                                           Tax_Id TaxId2)

            => TaxId1.CompareTo(TaxId2) >= 0;

        #endregion

        #endregion

        #region IComparable<TaxId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tax identifications.
        /// </summary>
        /// <param name="Object">A tax identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Tax_Id TaxId
                   ? CompareTo(TaxId)
                   : throw new ArgumentException("The given object is not a tax identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TaxId)

        /// <summary>
        /// Compares two tax identifications.
        /// </summary>
        /// <param name="TaxId">A tax identification to compare with.</param>
        public Int32 CompareTo(Tax_Id TaxId)

            => String.Compare(InternalId,
                              TaxId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TaxId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tax identifications for equality.
        /// </summary>
        /// <param name="Object">A tax identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Tax_Id TaxId &&
                   Equals(TaxId);

        #endregion

        #region Equals(TaxId)

        /// <summary>
        /// Compares two tax identifications for equality.
        /// </summary>
        /// <param name="TaxId">A tax identification to compare with.</param>
        public Boolean Equals(Tax_Id TaxId)

            => String.Equals(InternalId,
                             TaxId.InternalId,
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
