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
    /// Extension methods for whitelist types.
    /// </summary>
    public static class WhitelistTypeExtensions
    {

        /// <summary>
        /// Indicates whether this whitelist type is null or empty.
        /// </summary>
        /// <param name="WhitelistType">A whitelist type.</param>
        public static Boolean IsNullOrEmpty(this WhitelistType? WhitelistType)
            => !WhitelistType.HasValue || WhitelistType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this whitelist type is NOT null or empty.
        /// </summary>
        /// <param name="WhitelistType">A whitelist type.</param>
        public static Boolean IsNotNullOrEmpty(this WhitelistType? WhitelistType)
            => WhitelistType.HasValue && WhitelistType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The whitelist type.
    /// </summary>
    public readonly struct WhitelistType : IId<WhitelistType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this whitelist type is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this whitelist type is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the whitelist type.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new whitelist type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a whitelist type.</param>
        private WhitelistType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a whitelist type.
        /// </summary>
        /// <param name="Text">A text representation of a whitelist type.</param>
        public static WhitelistType Parse(String Text)
        {

            if (TryParse(Text, out var whitelistType))
                return whitelistType;

            throw new ArgumentException($"Invalid text representation of a whitelist type: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a whitelist type.
        /// </summary>
        /// <param name="Text">A text representation of a whitelist type.</param>
        public static WhitelistType? TryParse(String Text)
        {

            if (TryParse(Text, out var whitelistType))
                return whitelistType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out WhitelistType)

        /// <summary>
        /// Try to parse the given text as a whitelist type.
        /// </summary>
        /// <param name="Text">A text representation of a whitelist type.</param>
        /// <param name="WhitelistType">The parsed whitelist type.</param>
        public static Boolean TryParse(String Text, out WhitelistType WhitelistType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    WhitelistType = new WhitelistType(Text);
                    return true;
                }
                catch
                { }
            }

            WhitelistType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this whitelist type.
        /// </summary>
        public WhitelistType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Token always has to be whitelisted, realtime authorization is not possible/allowed.
        /// CPO shall always allow any use of this whitelist.
        /// </summary>
        public static WhitelistType  ALWAYS             { get; }
            = new ("ALWAYS");

        /// <summary>
        /// It is allowed to whitelist the whitelist, realtime authorization is also allowed.
        /// The CPO may choose which version of authorization to use.
        /// </summary>
        public static WhitelistType  ALLOWED            { get; }
            = new ("ALLOWED");

        /// <summary>
        /// In normal situations realtime authorization shall be used. But when the CPO cannot
        /// get a response from the eMSP (communication between CPO and eMSP is offline),
        /// the CPO shall allow this Token to be used.
        /// </summary>
        public static WhitelistType  ALLOWED_OFFLINE    { get; }
            = new ("ALLOWED_OFFLINE");

        /// <summary>
        /// Whitelisting is forbidden, only realtime authorization is allowed. CPO shall always
        /// send a realtime authorization for any use of this Token to the eMSP.
        /// </summary>
        public static WhitelistType  NEVER              { get; }
            = new ("NEVER");

        #endregion


        #region Operator overloading

        #region Operator == (WhitelistType1, WhitelistType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="WhitelistType1">A whitelist type.</param>
        /// <param name="WhitelistType2">Another whitelist type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (WhitelistType WhitelistType1,
                                           WhitelistType WhitelistType2)

            => WhitelistType1.Equals(WhitelistType2);

        #endregion

        #region Operator != (WhitelistType1, WhitelistType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="WhitelistType1">A whitelist type.</param>
        /// <param name="WhitelistType2">Another whitelist type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (WhitelistType WhitelistType1,
                                           WhitelistType WhitelistType2)

            => !WhitelistType1.Equals(WhitelistType2);

        #endregion

        #region Operator <  (WhitelistType1, WhitelistType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="WhitelistType1">A whitelist type.</param>
        /// <param name="WhitelistType2">Another whitelist type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (WhitelistType WhitelistType1,
                                          WhitelistType WhitelistType2)

            => WhitelistType1.CompareTo(WhitelistType2) < 0;

        #endregion

        #region Operator <= (WhitelistType1, WhitelistType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="WhitelistType1">A whitelist type.</param>
        /// <param name="WhitelistType2">Another whitelist type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (WhitelistType WhitelistType1,
                                           WhitelistType WhitelistType2)

            => WhitelistType1.CompareTo(WhitelistType2) <= 0;

        #endregion

        #region Operator >  (WhitelistType1, WhitelistType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="WhitelistType1">A whitelist type.</param>
        /// <param name="WhitelistType2">Another whitelist type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (WhitelistType WhitelistType1,
                                          WhitelistType WhitelistType2)

            => WhitelistType1.CompareTo(WhitelistType2) > 0;

        #endregion

        #region Operator >= (WhitelistType1, WhitelistType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="WhitelistType1">A whitelist type.</param>
        /// <param name="WhitelistType2">Another whitelist type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (WhitelistType WhitelistType1,
                                           WhitelistType WhitelistType2)

            => WhitelistType1.CompareTo(WhitelistType2) >= 0;

        #endregion

        #endregion

        #region IComparable<WhitelistType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two whitelist types.
        /// </summary>
        /// <param name="Object">A whitelist type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is WhitelistType whitelistType
                   ? CompareTo(whitelistType)
                   : throw new ArgumentException("The given object is not a whitelist type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(WhitelistType)

        /// <summary>
        /// Compares two whitelist types.
        /// </summary>
        /// <param name="WhitelistType">A whitelist type to compare with.</param>
        public Int32 CompareTo(WhitelistType WhitelistType)

            => String.Compare(InternalId,
                              WhitelistType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<WhitelistType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two whitelist types for equality.
        /// </summary>
        /// <param name="Object">A whitelist type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is WhitelistType whitelistType &&
                   Equals(whitelistType);

        #endregion

        #region Equals(WhitelistType)

        /// <summary>
        /// Compares two whitelist types for equality.
        /// </summary>
        /// <param name="WhitelistType">A whitelist type to compare with.</param>
        public Boolean Equals(WhitelistType WhitelistType)

            => String.Equals(InternalId,
                             WhitelistType.InternalId,
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
