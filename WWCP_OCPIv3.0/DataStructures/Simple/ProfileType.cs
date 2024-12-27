/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for profile types.
    /// </summary>
    public static class ProfileTypeExtensions
    {

        /// <summary>
        /// Indicates whether this profile type is null or empty.
        /// </summary>
        /// <param name="ProfileType">A profile type.</param>
        public static Boolean IsNullOrEmpty(this ProfileType? ProfileType)
            => !ProfileType.HasValue || ProfileType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this profile type is NOT null or empty.
        /// </summary>
        /// <param name="ProfileType">A profile type.</param>
        public static Boolean IsNotNullOrEmpty(this ProfileType? ProfileType)
            => ProfileType.HasValue && ProfileType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The profile of an EVSE/connector.
    /// </summary>
    public readonly struct ProfileType : IId<ProfileType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this profile type is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this profile type is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the profile type.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new profile type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a profile type.</param>
        private ProfileType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a profile type.
        /// </summary>
        /// <param name="Text">A text representation of a profile type.</param>
        public static ProfileType Parse(String Text)
        {

            if (TryParse(Text, out var profileType))
                return profileType;

            throw new ArgumentException($"Invalid text representation of a profile type: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a profile type.
        /// </summary>
        /// <param name="Text">A text representation of a profile type.</param>
        public static ProfileType? TryParse(String Text)
        {

            if (TryParse(Text, out var profileType))
                return profileType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ProfileType)

        /// <summary>
        /// Try to parse the given text as a profile type.
        /// </summary>
        /// <param name="Text">A text representation of a profile type.</param>
        /// <param name="ProfileType">The parsed profile type.</param>
        public static Boolean TryParse(String Text, out ProfileType ProfileType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ProfileType = new ProfileType(Text);
                    return true;
                }
                catch
                { }
            }

            ProfileType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this profile type.
        /// </summary>
        public ProfileType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// Driver wants to use the cheapest charging profile possible.
        /// </summary>
        public static ProfileType  CHEAP      { get; }
            = new ("CHEAP");

        /// <summary>
        /// Driver wants his EV charged as quickly as possible and is willing
        /// to pay a premium for this, if needed.
        /// </summary>
        public static ProfileType  FAST       { get; }
            = new ("FAST");

        /// <summary>
        /// Driver wants his EV charged with as much regenerative (green) energy as possible.
        /// </summary>
        public static ProfileType  GREEN      { get; }
            = new ("GREEN");

        /// <summary>
        /// Driver does not have special preferences.
        /// </summary>
        public static ProfileType  REGULAR    { get; }
            = new ("REGULAR");

        #endregion


        #region Operator overloading

        #region Operator == (ProfileType1, ProfileType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ProfileType1">A profile type.</param>
        /// <param name="ProfileType2">Another profile type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ProfileType ProfileType1,
                                           ProfileType ProfileType2)

            => ProfileType1.Equals(ProfileType2);

        #endregion

        #region Operator != (ProfileType1, ProfileType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ProfileType1">A profile type.</param>
        /// <param name="ProfileType2">Another profile type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ProfileType ProfileType1,
                                           ProfileType ProfileType2)

            => !ProfileType1.Equals(ProfileType2);

        #endregion

        #region Operator <  (ProfileType1, ProfileType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ProfileType1">A profile type.</param>
        /// <param name="ProfileType2">Another profile type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ProfileType ProfileType1,
                                          ProfileType ProfileType2)

            => ProfileType1.CompareTo(ProfileType2) < 0;

        #endregion

        #region Operator <= (ProfileType1, ProfileType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ProfileType1">A profile type.</param>
        /// <param name="ProfileType2">Another profile type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ProfileType ProfileType1,
                                           ProfileType ProfileType2)

            => ProfileType1.CompareTo(ProfileType2) <= 0;

        #endregion

        #region Operator >  (ProfileType1, ProfileType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ProfileType1">A profile type.</param>
        /// <param name="ProfileType2">Another profile type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ProfileType ProfileType1,
                                          ProfileType ProfileType2)

            => ProfileType1.CompareTo(ProfileType2) > 0;

        #endregion

        #region Operator >= (ProfileType1, ProfileType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ProfileType1">A profile type.</param>
        /// <param name="ProfileType2">Another profile type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ProfileType ProfileType1,
                                           ProfileType ProfileType2)

            => ProfileType1.CompareTo(ProfileType2) >= 0;

        #endregion

        #endregion

        #region IComparable<ProfileType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two profile types.
        /// </summary>
        /// <param name="Object">A profile type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ProfileType profileType
                   ? CompareTo(profileType)
                   : throw new ArgumentException("The given object is not a profile type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ProfileType)

        /// <summary>
        /// Compares two profile types.
        /// </summary>
        /// <param name="ProfileType">A profile type to compare with.</param>
        public Int32 CompareTo(ProfileType ProfileType)

            => String.Compare(InternalId,
                              ProfileType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ProfileType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two profile types for equality.
        /// </summary>
        /// <param name="Object">A profile type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ProfileType profileType &&
                   Equals(profileType);

        #endregion

        #region Equals(ProfileType)

        /// <summary>
        /// Compares two profile types for equality.
        /// </summary>
        /// <param name="ProfileType">A profile type to compare with.</param>
        public Boolean Equals(ProfileType ProfileType)

            => String.Equals(InternalId,
                             ProfileType.InternalId,
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
