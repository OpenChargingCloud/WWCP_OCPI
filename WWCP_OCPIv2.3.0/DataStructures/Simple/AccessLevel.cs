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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Extension methods for access levels.
    /// </summary>
    public static class AccessLevelExtensions
    {

        /// <summary>
        /// Indicates whether this access level is null or empty.
        /// </summary>
        /// <param name="AccessLevel">A access level.</param>
        public static Boolean IsNullOrEmpty(this AccessLevel? AccessLevel)
            => !AccessLevel.HasValue || AccessLevel.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this access level is NOT null or empty.
        /// </summary>
        /// <param name="AccessLevel">A access level.</param>
        public static Boolean IsNotNullOrEmpty(this AccessLevel? AccessLevel)
            => AccessLevel.HasValue && AccessLevel.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An access level: whether the parking bay and the charging station are located on the same
    /// level, providing step-free or otherwise accessible access between them
    /// (OCPI Accessibility Extension 1.0.0).
    /// </summary>
    public readonly struct AccessLevel : IId<AccessLevel>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this access level is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this access level is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the access level.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new access level based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an access level.</param>
        private AccessLevel(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an access level.
        /// </summary>
        /// <param name="Text">A text representation of an access level.</param>
        public static AccessLevel Parse(String Text)
        {

            ArgumentNullException.ThrowIfNull(Text);

            if (TryParse(Text, out var accessLevel))
                return accessLevel;

            throw new ArgumentException($"Invalid text representation of an access level: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an access level.
        /// </summary>
        /// <param name="Text">A text representation of an access level.</param>
        public static AccessLevel? TryParse(String Text)
        {

            if (TryParse(Text, out var accessLevel))
                return accessLevel;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out AccessLevel)

        /// <summary>
        /// Try to parse the given text as an access level.
        /// </summary>
        /// <param name="Text">A text representation of an access level.</param>
        /// <param name="AccessLevel">The parsed access level.</param>
        public static Boolean TryParse(String Text, out AccessLevel AccessLevel)
        {

            Text = Text?.Trim() ?? "";

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    AccessLevel = new AccessLevel(Text);
                    return true;
                }
                catch
                { }
            }

            AccessLevel = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this access level.
        /// </summary>
        public AccessLevel Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Parking bay and charging station are on the same level; no step, curb, or height difference.
        /// </summary>
        public static AccessLevel  SAME_LEVEL               { get; }
            = new ("SAME_LEVEL");

        /// <summary>
        /// Small height difference bridged by a ramp, beveled edge, or dropped curb, providing
        /// an accessible transition between the parking bay and the charge point.
        /// </summary>
        public static AccessLevel  ACCESSIBLE_TRANSITION    { get; }
            = new ("ACCESSIBLE_TRANSITION");

        /// <summary>
        /// Step, curb, or significant height difference without an accessible transition.
        /// </summary>
        public static AccessLevel  LEVEL_DIFFERENCE         { get; }
            = new ("LEVEL_DIFFERENCE");

        #endregion


        #region Operator overloading

        #region Operator == (AccessLevel1, AccessLevel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessLevel1">A access level.</param>
        /// <param name="AccessLevel2">Another access level.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AccessLevel AccessLevel1,
                                           AccessLevel AccessLevel2)

            => AccessLevel1.Equals(AccessLevel2);

        #endregion

        #region Operator != (AccessLevel1, AccessLevel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessLevel1">A access level.</param>
        /// <param name="AccessLevel2">Another access level.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AccessLevel AccessLevel1,
                                           AccessLevel AccessLevel2)

            => !AccessLevel1.Equals(AccessLevel2);

        #endregion

        #region Operator <  (AccessLevel1, AccessLevel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessLevel1">A access level.</param>
        /// <param name="AccessLevel2">Another access level.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AccessLevel AccessLevel1,
                                          AccessLevel AccessLevel2)

            => AccessLevel1.CompareTo(AccessLevel2) < 0;

        #endregion

        #region Operator <= (AccessLevel1, AccessLevel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessLevel1">A access level.</param>
        /// <param name="AccessLevel2">Another access level.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AccessLevel AccessLevel1,
                                           AccessLevel AccessLevel2)

            => AccessLevel1.CompareTo(AccessLevel2) <= 0;

        #endregion

        #region Operator >  (AccessLevel1, AccessLevel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessLevel1">A access level.</param>
        /// <param name="AccessLevel2">Another access level.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AccessLevel AccessLevel1,
                                          AccessLevel AccessLevel2)

            => AccessLevel1.CompareTo(AccessLevel2) > 0;

        #endregion

        #region Operator >= (AccessLevel1, AccessLevel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessLevel1">A access level.</param>
        /// <param name="AccessLevel2">Another access level.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AccessLevel AccessLevel1,
                                           AccessLevel AccessLevel2)

            => AccessLevel1.CompareTo(AccessLevel2) >= 0;

        #endregion

        #endregion

        #region IComparable<AccessLevel> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two access levels.
        /// </summary>
        /// <param name="Object">A access level to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AccessLevel accessLevel
                   ? CompareTo(accessLevel)
                   : throw new ArgumentException("The given object is not an access level!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AccessLevel)

        /// <summary>
        /// Compares two access levels.
        /// </summary>
        /// <param name="AccessLevel">A access level to compare with.</param>
        public Int32 CompareTo(AccessLevel AccessLevel)

            => String.Compare(InternalId,
                              AccessLevel.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<AccessLevel> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two access levels for equality.
        /// </summary>
        /// <param name="Object">A access level to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AccessLevel accessLevel &&
                   Equals(accessLevel);

        #endregion

        #region Equals(AccessLevel)

        /// <summary>
        /// Compares two access levels for equality.
        /// </summary>
        /// <param name="AccessLevel">A access level to compare with.</param>
        public Boolean Equals(AccessLevel AccessLevel)

            => String.Equals(InternalId,
                             AccessLevel.InternalId,
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
