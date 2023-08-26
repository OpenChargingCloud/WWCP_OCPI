/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Extension methods for location types.
    /// </summary>
    public static class LocationTypeExtensions
    {

        /// <summary>
        /// Indicates whether this location type is null or empty.
        /// </summary>
        /// <param name="LocationType">A location type.</param>
        public static Boolean IsNullOrEmpty(this LocationType? LocationType)
            => !LocationType.HasValue || LocationType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this location type is NOT null or empty.
        /// </summary>
        /// <param name="LocationType">A location type.</param>
        public static Boolean IsNotNullOrEmpty(this LocationType? LocationType)
            => LocationType.HasValue && LocationType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The location type.
    /// </summary>
    public readonly struct LocationType : IId<LocationType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this location type is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this location type is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the location type.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new location type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a location type.</param>
        private LocationType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a location type.
        /// </summary>
        /// <param name="Text">A text representation of a location type.</param>
        public static LocationType Parse(String Text)
        {

            if (TryParse(Text, out var locationType))
                return locationType;

            throw new ArgumentException($"Invalid text representation of a location type: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a location type.
        /// </summary>
        /// <param name="Text">A text representation of a location type.</param>
        public static LocationType? TryParse(String Text)
        {

            if (TryParse(Text, out var locationType))
                return locationType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out LocationType)

        /// <summary>
        /// Try to parse the given text as a location type.
        /// </summary>
        /// <param name="Text">A text representation of a location type.</param>
        /// <param name="LocationType">The parsed location type.</param>
        public static Boolean TryParse(String Text, out LocationType LocationType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    LocationType = new LocationType(Text);
                    return true;
                }
                catch
                { }
            }

            LocationType = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this location type.
        /// </summary>
        public LocationType Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Parking location type is not known by the operator (default).
        /// </summary>
        public static LocationType UNKNOWN
            => new("UNKNOWN");

        /// <summary>
        /// Parking in public space.
        /// </summary>
        public static LocationType ON_STREET
            => new("ON_STREET");

        /// <summary>
        /// Multistorey car park.
        /// </summary>
        public static LocationType PARKING_GARAGE
            => new("PARKING_GARAGE");

        /// <summary>
        /// Multistorey car park, mainly underground.
        /// </summary>
        public static LocationType UNDERGROUND_GARAGE
            => new("UNDERGROUND_GARAGE");

        /// <summary>
        /// A cleared area that is intended for location vehicles, i.e.at super markets, bars, etc.
        /// </summary>
        public static LocationType PARKING_LOT
            => new("PARKING_LOT");

        /// <summary>
        /// None of the given possibilities.
        /// </summary>
        public static LocationType OTHER
            => new("OTHER");

        #endregion


        #region Operator overloading

        #region Operator == (LocationType1, LocationType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationType1">A location type.</param>
        /// <param name="LocationType2">Another location type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (LocationType LocationType1,
                                           LocationType LocationType2)

            => LocationType1.Equals(LocationType2);

        #endregion

        #region Operator != (LocationType1, LocationType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationType1">A location type.</param>
        /// <param name="LocationType2">Another location type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (LocationType LocationType1,
                                           LocationType LocationType2)

            => !LocationType1.Equals(LocationType2);

        #endregion

        #region Operator <  (LocationType1, LocationType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationType1">A location type.</param>
        /// <param name="LocationType2">Another location type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (LocationType LocationType1,
                                          LocationType LocationType2)

            => LocationType1.CompareTo(LocationType2) < 0;

        #endregion

        #region Operator <= (LocationType1, LocationType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationType1">A location type.</param>
        /// <param name="LocationType2">Another location type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (LocationType LocationType1,
                                           LocationType LocationType2)

            => LocationType1.CompareTo(LocationType2) <= 0;

        #endregion

        #region Operator >  (LocationType1, LocationType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationType1">A location type.</param>
        /// <param name="LocationType2">Another location type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (LocationType LocationType1,
                                          LocationType LocationType2)

            => LocationType1.CompareTo(LocationType2) > 0;

        #endregion

        #region Operator >= (LocationType1, LocationType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationType1">A location type.</param>
        /// <param name="LocationType2">Another location type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (LocationType LocationType1,
                                           LocationType LocationType2)

            => LocationType1.CompareTo(LocationType2) >= 0;

        #endregion

        #endregion

        #region IComparable<LocationType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two location types.
        /// </summary>
        /// <param name="Object">A location type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is LocationType locationType
                   ? CompareTo(locationType)
                   : throw new ArgumentException("The given object is not a location type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LocationType)

        /// <summary>
        /// Compares two location types.
        /// </summary>
        /// <param name="LocationType">A location type to compare with.</param>
        public Int32 CompareTo(LocationType LocationType)

            => String.Compare(InternalId,
                              LocationType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<LocationType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two location types for equality.
        /// </summary>
        /// <param name="Object">A location type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LocationType locationType &&
                   Equals(locationType);

        #endregion

        #region Equals(LocationType)

        /// <summary>
        /// Compares two location types for equality.
        /// </summary>
        /// <param name="LocationType">A location type to compare with.</param>
        public Boolean Equals(LocationType LocationType)

            => String.Equals(InternalId,
                             LocationType.InternalId,
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
