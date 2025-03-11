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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for roles.
    /// </summary>
    public static class RoleExtensions
    {

        /// <summary>
        /// Indicates whether this role is null or empty.
        /// </summary>
        /// <param name="Role">A role.</param>
        public static Boolean IsNullOrEmpty(this Role? Role)
            => !Role.HasValue || Role.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this role is NOT null or empty.
        /// </summary>
        /// <param name="Role">A role.</param>
        public static Boolean IsNotNullOrEmpty(this Role? Role)
            => Role.HasValue && Role.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A role.
    /// </summary>
    public readonly struct Role : IId<Role>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this role is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this role is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the role.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new role based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a role.</param>
        private Role(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom(Length = 30, IsLocal = false)

        /// <summary>
        /// Create a new random role.
        /// </summary>
        /// <param name="Length">The expected length of the role.</param>
        /// <param name="IsLocal">The role was generated locally and not received via network.</param>
        public static Role NewRandom(Byte      Length    = 30,
                                           Boolean?  IsLocal   = false)

            => new ((IsLocal == true ? "Local:" : "") +
                    RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a role.
        /// </summary>
        /// <param name="Text">A text representation of a role.</param>
        public static Role Parse(String Text)
        {

            if (TryParse(Text, out var requestId))
                return requestId;

            throw new ArgumentException($"Invalid text representation of a role: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a role.
        /// </summary>
        /// <param name="Text">A text representation of a role.</param>
        public static Role? TryParse(String Text)
        {

            if (TryParse(Text, out var requestId))
                return requestId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out Role)

        /// <summary>
        /// Try to parse the given text as a role.
        /// </summary>
        /// <param name="Text">A text representation of a role.</param>
        /// <param name="Role">The parsed role.</param>
        public static Boolean TryParse(String Text, out Role Role)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    Role = new Role(Text);
                    return true;
                }
                catch
                { }
            }

            Role = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this role.
        /// </summary>
        public Role Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Unknown
        /// </summary>
        public static Role  Unknown     { get; }
            = new ("Unknown");

        /// <summary>
        /// OpenData
        /// </summary>
        public static Role  OpenData    { get; }
            = new ("OpenData");

        /// <summary>
        /// A charge point operator operates a network of charging stations.
        /// </summary>
        public static Role  CPO         { get; }
            = new ("CPO");

        /// <summary>
        /// An E-Mobility Service Provider gives electric vehicle drivers access to charging services.
        /// </summary>
        public static Role  EMSP        { get; }
            = new ("EMSP");

        /// <summary>
        /// A payment terminal provider.
        /// </summary>
        public static Role  PTP         { get; }
            = new ("PTP");

        /// <summary>
        /// A roaming hub can connect multiple CPOs to multiple eMSPs.
        /// </summary>
        public static Role  HUB         { get; }
            = new ("HUB");

        /// <summary>
        /// National Access Point: National database with all location information of a country.
        /// </summary>
        public static Role  NAP         { get; }
            = new ("NAP");

        /// <summary>
        /// Navigation Service Provider: Like an eMSP, but probably only interested in location information.
        /// </summary>
        public static Role  NSP         { get; }
            = new ("NSP");

        /// <summary>
        /// Smart Charging Service Provider
        /// </summary>
        public static Role  SCSP        { get; }
            = new ("SCSP");

        /// <summary>
        /// Other
        /// </summary>
        public static Role  OTHER       { get; }
            = new ("OTHER");

        #endregion


        #region Operator overloading

        #region Operator == (Role1, Role2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Role1">A role.</param>
        /// <param name="Role2">Another role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Role Role1,
                                           Role Role2)

            => Role1.Equals(Role2);

        #endregion

        #region Operator != (Role1, Role2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Role1">A role.</param>
        /// <param name="Role2">Another role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Role Role1,
                                           Role Role2)

            => !Role1.Equals(Role2);

        #endregion

        #region Operator <  (Role1, Role2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Role1">A role.</param>
        /// <param name="Role2">Another role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Role Role1,
                                          Role Role2)

            => Role1.CompareTo(Role2) < 0;

        #endregion

        #region Operator <= (Role1, Role2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Role1">A role.</param>
        /// <param name="Role2">Another role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Role Role1,
                                           Role Role2)

            => Role1.CompareTo(Role2) <= 0;

        #endregion

        #region Operator >  (Role1, Role2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Role1">A role.</param>
        /// <param name="Role2">Another role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Role Role1,
                                          Role Role2)

            => Role1.CompareTo(Role2) > 0;

        #endregion

        #region Operator >= (Role1, Role2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Role1">A role.</param>
        /// <param name="Role2">Another role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Role Role1,
                                           Role Role2)

            => Role1.CompareTo(Role2) >= 0;

        #endregion

        #endregion

        #region IComparable<Role> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two roles.
        /// </summary>
        /// <param name="Object">A role to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Role requestId
                   ? CompareTo(requestId)
                   : throw new ArgumentException("The given object is not a role!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Role)

        /// <summary>
        /// Compares two roles.
        /// </summary>
        /// <param name="Role">A role to compare with.</param>
        public Int32 CompareTo(Role Role)

            => String.Compare(InternalId,
                              Role.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Role> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two roles for equality.
        /// </summary>
        /// <param name="Object">A role to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Role requestId &&
                   Equals(requestId);

        #endregion

        #region Equals(Role)

        /// <summary>
        /// Compares two roles for equality.
        /// </summary>
        /// <param name="Role">A role to compare with.</param>
        public Boolean Equals(Role Role)

            => String.Equals(InternalId,
                             Role.InternalId,
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
