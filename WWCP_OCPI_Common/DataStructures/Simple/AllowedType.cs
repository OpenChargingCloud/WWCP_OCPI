﻿/*
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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for allowed types.
    /// </summary>
    public static class AllowedTypeExtensions
    {

        /// <summary>
        /// Indicates whether this allowed type is null or empty.
        /// </summary>
        /// <param name="AllowedType">An allowed type.</param>
        public static Boolean IsNullOrEmpty(this AllowedType? AllowedType)
            => !AllowedType.HasValue || AllowedType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this allowed type is NOT null or empty.
        /// </summary>
        /// <param name="AllowedType">An allowed type.</param>
        public static Boolean IsNotNullOrEmpty(this AllowedType? AllowedType)
            => AllowedType.HasValue && AllowedType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An allowed type.
    /// </summary>
    public readonly struct AllowedType : IId<AllowedType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this allowed type is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this allowed type is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the allowed type.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new allowed type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an allowed type.</param>
        private AllowedType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an allowed type.
        /// </summary>
        /// <param name="Text">A text representation of an allowed type.</param>
        public static AllowedType Parse(String Text)
        {

            if (TryParse(Text, out var allowedType))
                return allowedType;

            throw new ArgumentException($"Invalid text representation of an allowed type: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an allowed type.
        /// </summary>
        /// <param name="Text">A text representation of an allowed type.</param>
        public static AllowedType? TryParse(String Text)
        {

            if (TryParse(Text, out var allowedType))
                return allowedType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out AllowedType)

        /// <summary>
        /// Try to parse the given text as an allowed type.
        /// </summary>
        /// <param name="Text">A text representation of an allowed type.</param>
        /// <param name="AllowedType">The parsed allowed type.</param>
        public static Boolean TryParse(String Text, out AllowedType AllowedType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    AllowedType = new AllowedType(Text);
                    return true;
                }
                catch
                { }
            }

            AllowedType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this allowed type.
        /// </summary>
        public AllowedType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// This Token is allowed to charge (at the given Location).
        /// </summary>
        public static AllowedType  ALLOWED              { get; }
            = new ("ALLOWED");

        /// <summary>
        /// This Token is blocked.
        /// </summary>
        public static AllowedType  BLOCKED              { get; }
            = new ("BLOCKED");

        /// <summary>
        /// This Token has expired.
        /// </summary>
        public static AllowedType  EXPIRED              { get; }
            = new ("EXPIRED");

        /// <summary>
        /// This Token belongs to an account that has not enough credits to charge (at the given Location).
        /// </summary>
        public static AllowedType  NO_CREDIT            { get; }
            = new ("NO_CREDIT");

        /// <summary>
        /// This Token is not allowed to charge at the whole Location or all of the Connectors in the
        /// request, but may be authorized to charge on a subset of the Connectors that authorization
        /// was requested for.
        /// </summary>
        public static AllowedType  REQUEST_TOO_BROAD    { get; }
            = new ("REQUEST_TOO_BROAD");

        /// <summary>
        /// The Token not allowed to charge at the Location or at any of the connectors that
        /// authorization was requested for.
        /// </summary>
        public static AllowedType  NOT_ALLOWED          { get; }
            = new ("NOT_ALLOWED");

        #endregion


        #region Operator overloading

        #region Operator == (AllowedType1, AllowedType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AllowedType1">An allowed type.</param>
        /// <param name="AllowedType2">Another allowed type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AllowedType AllowedType1,
                                           AllowedType AllowedType2)

            => AllowedType1.Equals(AllowedType2);

        #endregion

        #region Operator != (AllowedType1, AllowedType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AllowedType1">An allowed type.</param>
        /// <param name="AllowedType2">Another allowed type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AllowedType AllowedType1,
                                           AllowedType AllowedType2)

            => !AllowedType1.Equals(AllowedType2);

        #endregion

        #region Operator <  (AllowedType1, AllowedType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AllowedType1">An allowed type.</param>
        /// <param name="AllowedType2">Another allowed type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AllowedType AllowedType1,
                                          AllowedType AllowedType2)

            => AllowedType1.CompareTo(AllowedType2) < 0;

        #endregion

        #region Operator <= (AllowedType1, AllowedType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AllowedType1">An allowed type.</param>
        /// <param name="AllowedType2">Another allowed type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AllowedType AllowedType1,
                                           AllowedType AllowedType2)

            => AllowedType1.CompareTo(AllowedType2) <= 0;

        #endregion

        #region Operator >  (AllowedType1, AllowedType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AllowedType1">An allowed type.</param>
        /// <param name="AllowedType2">Another allowed type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AllowedType AllowedType1,
                                          AllowedType AllowedType2)

            => AllowedType1.CompareTo(AllowedType2) > 0;

        #endregion

        #region Operator >= (AllowedType1, AllowedType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AllowedType1">An allowed type.</param>
        /// <param name="AllowedType2">Another allowed type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AllowedType AllowedType1,
                                           AllowedType AllowedType2)

            => AllowedType1.CompareTo(AllowedType2) >= 0;

        #endregion

        #endregion

        #region IComparable<AllowedType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two allowed types.
        /// </summary>
        /// <param name="Object">An allowed type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AllowedType allowedType
                   ? CompareTo(allowedType)
                   : throw new ArgumentException("The given object is not an allowed type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AllowedType)

        /// <summary>
        /// Compares two allowed types.
        /// </summary>
        /// <param name="AllowedType">An allowed type to compare with.</param>
        public Int32 CompareTo(AllowedType AllowedType)

            => String.Compare(InternalId,
                              AllowedType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<AllowedType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two allowed types for equality.
        /// </summary>
        /// <param name="Object">An allowed type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AllowedType allowedType &&
                   Equals(allowedType);

        #endregion

        #region Equals(AllowedType)

        /// <summary>
        /// Compares two allowed types for equality.
        /// </summary>
        /// <param name="AllowedType">An allowed type to compare with.</param>
        public Boolean Equals(AllowedType AllowedType)

            => String.Equals(InternalId,
                             AllowedType.InternalId,
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
