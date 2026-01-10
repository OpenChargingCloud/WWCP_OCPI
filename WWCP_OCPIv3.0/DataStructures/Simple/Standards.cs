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

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for standards.
    /// </summary>
    public static class StandardExtensions
    {

        /// <summary>
        /// Indicates whether this standard is null or empty.
        /// </summary>
        /// <param name="Standard">A standard.</param>
        public static Boolean IsNullOrEmpty(this Standard? Standard)
            => !Standard.HasValue || Standard.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this standard is NOT null or empty.
        /// </summary>
        /// <param name="Standard">A standard.</param>
        public static Boolean IsNotNullOrEmpty(this Standard? Standard)
            => Standard.HasValue && Standard.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The standard.
    /// </summary>
    public readonly struct Standard : IId<Standard>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this standard is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this standard is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the standard.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new standard based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a standard.</param>
        private Standard(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a standard.
        /// </summary>
        /// <param name="Text">A text representation of a standard.</param>
        public static Standard Parse(String Text)
        {

            if (TryParse(Text, out var standard))
                return standard;

            throw new ArgumentException($"Invalid text representation of a standard: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a standard.
        /// </summary>
        /// <param name="Text">A text representation of a standard.</param>
        public static Standard? TryParse(String Text)
        {

            if (TryParse(Text, out var standard))
                return standard;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Standard)

        /// <summary>
        /// Try to parse the given text as a standard.
        /// </summary>
        /// <param name="Text">A text representation of a standard.</param>
        /// <param name="Standard">The parsed standard.</param>
        public static Boolean TryParse(String Text, out Standard Standard)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    Standard = new Standard(Text);
                    return true;
                }
                catch
                { }
            }

            Standard = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this standard.
        /// </summary>
        public Standard Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// PAS 1899:2022 Electric Vehicles Accessible Charging
        /// </summary>
        public static Standard  PAS1899_2022    { get; }
            = new ("PAS 1899:2022");

        #endregion


        #region Operator overloading

        #region Operator == (Standard1, Standard2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Standard1">A standard.</param>
        /// <param name="Standard2">Another standard.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Standard Standard1,
                                           Standard Standard2)

            => Standard1.Equals(Standard2);

        #endregion

        #region Operator != (Standard1, Standard2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Standard1">A standard.</param>
        /// <param name="Standard2">Another standard.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Standard Standard1,
                                           Standard Standard2)

            => !Standard1.Equals(Standard2);

        #endregion

        #region Operator <  (Standard1, Standard2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Standard1">A standard.</param>
        /// <param name="Standard2">Another standard.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Standard Standard1,
                                          Standard Standard2)

            => Standard1.CompareTo(Standard2) < 0;

        #endregion

        #region Operator <= (Standard1, Standard2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Standard1">A standard.</param>
        /// <param name="Standard2">Another standard.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Standard Standard1,
                                           Standard Standard2)

            => Standard1.CompareTo(Standard2) <= 0;

        #endregion

        #region Operator >  (Standard1, Standard2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Standard1">A standard.</param>
        /// <param name="Standard2">Another standard.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Standard Standard1,
                                          Standard Standard2)

            => Standard1.CompareTo(Standard2) > 0;

        #endregion

        #region Operator >= (Standard1, Standard2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Standard1">A standard.</param>
        /// <param name="Standard2">Another standard.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Standard Standard1,
                                           Standard Standard2)

            => Standard1.CompareTo(Standard2) >= 0;

        #endregion

        #endregion

        #region IComparable<Standard> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two standards.
        /// </summary>
        /// <param name="Object">A standard to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Standard standard
                   ? CompareTo(standard)
                   : throw new ArgumentException("The given object is not a standard!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Standard)

        /// <summary>
        /// Compares two standards.
        /// </summary>
        /// <param name="Standard">A standard to compare with.</param>
        public Int32 CompareTo(Standard Standard)

            => String.Compare(InternalId,
                              Standard.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Standard> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two standards for equality.
        /// </summary>
        /// <param name="Object">A standard to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Standard standard &&
                   Equals(standard);

        #endregion

        #region Equals(Standard)

        /// <summary>
        /// Compares two standards for equality.
        /// </summary>
        /// <param name="Standard">A standard to compare with.</param>
        public Boolean Equals(Standard Standard)

            => String.Equals(InternalId,
                             Standard.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToUpper().GetHashCode() ?? 0;

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
