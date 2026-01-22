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

using System.Collections.Concurrent;

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


        #region Matches(Facilities, Text)

        /// <summary>
        /// Checks whether the given enumeration of facilities matches the given text.
        /// </summary>
        /// <param name="Facilities">An enumeration of facilities.</param>
        /// <param name="Text">A text to match.</param>
        public static Boolean Matches(this IEnumerable<Role>  Facilities,
                                      String                  Text)

            => Facilities.Any(facilitiy => facilitiy.Value.Contains(Text, StringComparison.OrdinalIgnoreCase));

        #endregion


    }


    /// <summary>
    /// A role comparer.
    /// </summary>
    public sealed class RoleComparer : IComparer<Role>
    {

        /// <summary>
        /// The default role comparer.
        /// </summary>
        public static readonly RoleComparer OrdinalIgnoreCase = new();

        /// <summary>
        /// Compares two facilities.
        /// </summary>
        /// <param name="Role1">A role to compare with.</param>
        /// <param name="Role2">A role to compare with.</param>
        public Int32 Compare(Role Role1,
                             Role Role2)

            => StringComparer.OrdinalIgnoreCase.Compare(
                   Role1.Value,
                   Role2.Value
               );

    }


    /// <summary>
    /// A role.
    /// </summary>
    public readonly struct Role : IId<Role>
    {

        #region Static Lookup

        private readonly static ConcurrentDictionary<String, Role> lookup = new (StringComparer.OrdinalIgnoreCase);

        private static Role Register(String Text)

            => lookup.GetOrAdd(
                   Text,
                   static text => new Role(text)
               );

        /// <summary>
        /// All registered roles.
        /// </summary>
        public static IEnumerable<Role> All
            => lookup.Values;

        #endregion

        #region Properties

        /// <summary>
        /// The text representation of the role.
        /// </summary>
        public String   Value    { get; }

        /// <summary>
        /// Indicates whether this role is null or empty.
        /// </summary>
        public Boolean  IsNullOrEmpty
            => Value.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this role is NOT null or empty.
        /// </summary>
        public Boolean  IsNotNullOrEmpty
            => Value.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the role.
        /// </summary>
        public UInt64   Length
            => (UInt64) (Value?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new role based on the given text representation.
        /// </summary>
        /// <param name="Text">The text representation of a role.</param>
        private Role(String Text)
        {
            this.Value = Text;
        }

        #endregion


        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a role.
        /// </summary>
        /// <param name="Text">A text representation of a role.</param>
        public static Role Parse(String Text)
        {

            if (TryParse(Text, out var role))
                return role;

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

            if (TryParse(Text, out var role))
                return role;

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
                    Role = Register(Text);
                    return true;
                }
                catch
                { }
            }

            Role = default;
            return false;

        }

        #endregion


        #region Static definitions

        /// <summary>
        /// Unknown
        /// </summary>
        public static Role  Unknown     { get; }
            = Register("Unknown");

        /// <summary>
        /// OpenData
        /// </summary>
        public static Role  OpenData    { get; }
            = Register("OpenData");

        /// <summary>
        /// A charge point operator operates a network of charging stations.
        /// </summary>
        public static Role  CPO         { get; }
            = Register("CPO");

        /// <summary>
        /// An E-Mobility Service Provider gives electric vehicle drivers access to charging services.
        /// </summary>
        public static Role  EMSP        { get; }
            = Register("EMSP");

        /// <summary>
        /// A payment terminal provider.
        /// </summary>
        public static Role  PTP         { get; }
            = Register("PTP");

        /// <summary>
        /// A roaming hub can connect multiple CPOs to multiple eMSPs.
        /// </summary>
        public static Role  HUB         { get; }
            = Register("HUB");

        /// <summary>
        /// National Access Point: National database with all location information of a country.
        /// </summary>
        public static Role  NAP         { get; }
            = Register("NAP");

        /// <summary>
        /// Navigation Service Provider: Like an eMSP, but probably only interested in location information.
        /// </summary>
        public static Role  NSP         { get; }
            = Register("NSP");

        /// <summary>
        /// Smart Charging Service Provider
        /// </summary>
        public static Role  SCSP        { get; }
            = Register("SCSP");

        /// <summary>
        /// Other
        /// </summary>
        public static Role  OTHER       { get; }
            = Register("OTHER");

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

            => Object is Role role
                   ? CompareTo(role)
                   : throw new ArgumentException("The given object is not a role!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Role)

        /// <summary>
        /// Compares two roles.
        /// </summary>
        /// <param name="Role">A role to compare with.</param>
        public Int32 CompareTo(Role Role)

            => String.Compare(Value,
                              Role.Value,
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

            => Object is Role role &&
                   Equals(role);

        #endregion

        #region Equals(Role)

        /// <summary>
        /// Compares two roles for equality.
        /// </summary>
        /// <param name="Role">A role to compare with.</param>
        public Boolean Equals(Role Role)

            => String.Equals(Value,
                             Role.Value,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => Value is not null
                   ? StringComparer.OrdinalIgnoreCase.GetHashCode(Value) 
                   : 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Value ?? "";

        #endregion

    }

}
