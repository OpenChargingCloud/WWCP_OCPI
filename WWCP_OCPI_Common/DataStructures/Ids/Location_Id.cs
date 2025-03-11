/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Location 2.0 (the "License");
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
    /// Extension methods for location identifications.
    /// </summary>
    public static class LocationIdExtensions
    {

        /// <summary>
        /// Indicates whether this location identification is null or empty.
        /// </summary>
        /// <param name="LocationId">A location identification.</param>
        public static Boolean IsNullOrEmpty(this Location_Id? LocationId)
            => !LocationId.HasValue || LocationId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this location identification is NOT null or empty.
        /// </summary>
        /// <param name="LocationId">A location identification.</param>
        public static Boolean IsNotNullOrEmpty(this Location_Id? LocationId)
            => LocationId.HasValue && LocationId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a location.
    /// CiString(36)
    /// </summary>
    public readonly struct Location_Id : IId<Location_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this location identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this location identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the location identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new location identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a location identification.</param>
        private Location_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom(Length = 20)

        /// <summary>
        /// Create a new random location identification.
        /// </summary>
        /// <param name="Length">The expected length of the location identification.</param>
        public static Location_Id NewRandom(Byte Length = 30)

            => new (RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a location identification.
        /// </summary>
        /// <param name="Text">A text representation of a location identification.</param>
        public static Location_Id Parse(String Text)
        {

            if (TryParse(Text, out var locationId))
                return locationId;

            throw new ArgumentException($"Invalid text representation of a location identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a location identification.
        /// </summary>
        /// <param name="Text">A text representation of a location identification.</param>
        public static Location_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var locationId))
                return locationId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out LocationId)

        /// <summary>
        /// Try to parse the given text as a location identification.
        /// </summary>
        /// <param name="Text">A text representation of a location identification.</param>
        /// <param name="LocationId">The parsed location identification.</param>
        public static Boolean TryParse(String Text, out Location_Id LocationId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    LocationId = new Location_Id(Text);
                    return true;
                }
                catch
                { }
            }

            LocationId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this location identification.
        /// </summary>
        public Location_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A location identification.</param>
        /// <param name="LocationId2">Another location identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Location_Id LocationId1,
                                           Location_Id LocationId2)

            => LocationId1.Equals(LocationId2);

        #endregion

        #region Operator != (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A location identification.</param>
        /// <param name="LocationId2">Another location identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Location_Id LocationId1,
                                           Location_Id LocationId2)

            => !LocationId1.Equals(LocationId2);

        #endregion

        #region Operator <  (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A location identification.</param>
        /// <param name="LocationId2">Another location identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Location_Id LocationId1,
                                          Location_Id LocationId2)

            => LocationId1.CompareTo(LocationId2) < 0;

        #endregion

        #region Operator <= (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A location identification.</param>
        /// <param name="LocationId2">Another location identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Location_Id LocationId1,
                                           Location_Id LocationId2)

            => LocationId1.CompareTo(LocationId2) <= 0;

        #endregion

        #region Operator >  (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A location identification.</param>
        /// <param name="LocationId2">Another location identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Location_Id LocationId1,
                                          Location_Id LocationId2)

            => LocationId1.CompareTo(LocationId2) > 0;

        #endregion

        #region Operator >= (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A location identification.</param>
        /// <param name="LocationId2">Another location identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Location_Id LocationId1,
                                           Location_Id LocationId2)

            => LocationId1.CompareTo(LocationId2) >= 0;

        #endregion

        #endregion

        #region IComparable<LocationId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two location identifications.
        /// </summary>
        /// <param name="Object">A location identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Location_Id locationId
                   ? CompareTo(locationId)
                   : throw new ArgumentException("The given object is not a location identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LocationId)

        /// <summary>
        /// Compares two location identifications.
        /// </summary>
        /// <param name="LocationId">A location identification to compare with.</param>
        public Int32 CompareTo(Location_Id LocationId)

            => String.Compare(InternalId,
                              LocationId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<LocationId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two location identifications for equality.
        /// </summary>
        /// <param name="Object">A location identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Location_Id locationId &&
                   Equals(locationId);

        #endregion

        #region Equals(LocationId)

        /// <summary>
        /// Compares two location identifications for equality.
        /// </summary>
        /// <param name="LocationId">A location identification to compare with.</param>
        public Boolean Equals(Location_Id LocationId)

            => String.Equals(InternalId,
                             LocationId.InternalId,
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
