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
    /// Extension methods for parking restriction groups.
    /// </summary>
    public static class ParkingRestrictionGroupExtensions
    {

        /// <summary>
        /// Indicates whether this parking restriction group is null or empty.
        /// </summary>
        /// <param name="ParkingRestrictionGroup">A parking restriction group.</param>
        public static Boolean IsNullOrEmpty(this ParkingRestrictionGroup? ParkingRestrictionGroup)
            => !ParkingRestrictionGroup.HasValue || ParkingRestrictionGroup.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this parking restriction group is NOT null or empty.
        /// </summary>
        /// <param name="ParkingRestrictionGroup">A parking restriction group.</param>
        public static Boolean IsNotNullOrEmpty(this ParkingRestrictionGroup? ParkingRestrictionGroup)
            => ParkingRestrictionGroup.HasValue && ParkingRestrictionGroup.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The parking restriction group.
    /// </summary>
    public readonly struct ParkingRestrictionGroup : IId<ParkingRestrictionGroup>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this parking restriction group is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this parking restriction group is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the parking restriction group.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new parking restriction group based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a parking restriction group.</param>
        private ParkingRestrictionGroup(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a parking restriction group.
        /// </summary>
        /// <param name="Text">A text representation of a parking restriction group.</param>
        public static ParkingRestrictionGroup Parse(String Text)
        {

            if (TryParse(Text, out var parkingRestrictionGroup))
                return parkingRestrictionGroup;

            throw new ArgumentException($"Invalid text representation of a parking restriction group: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a parking restriction group.
        /// </summary>
        /// <param name="Text">A text representation of a parking restriction group.</param>
        public static ParkingRestrictionGroup? TryParse(String Text)
        {

            if (TryParse(Text, out var parkingRestrictionGroup))
                return parkingRestrictionGroup;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ParkingRestrictionGroup)

        /// <summary>
        /// Try to parse the given text as a parking restriction group.
        /// </summary>
        /// <param name="Text">A text representation of a parking restriction group.</param>
        /// <param name="ParkingRestrictionGroup">The parsed parking restriction group.</param>
        public static Boolean TryParse(String Text, out ParkingRestrictionGroup ParkingRestrictionGroup)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ParkingRestrictionGroup = new ParkingRestrictionGroup(Text);
                    return true;
                }
                catch
                { }
            }

            ParkingRestrictionGroup = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this parking restriction group.
        /// </summary>
        public ParkingRestrictionGroup Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Reserved parking spot for electric vehicles.
        /// </summary>
        public static ParkingRestrictionGroup EV_ONLY        { get; }
            = new ("EV_ONLY");

        /// <summary>
        /// Parking allowed only while plugged in (charging).
        /// </summary>
        public static ParkingRestrictionGroup PLUGGED        { get; }
            = new ("PLUGGED");

        /// <summary>
        /// Reserved parking spot for disabled people with valid ID.
        /// </summary>
        public static ParkingRestrictionGroup DISABLED        { get; }
            = new ("DISABLED");

        /// <summary>
        /// Parking spot for customers/guests only, for example in case of a hotel or shop.
        /// </summary>
        public static ParkingRestrictionGroup CUSTOMERS        { get; }
            = new ("CUSTOMERS");

        /// <summary>
        /// Parking spot only suitable for (electric) motorcycles or scooters.
        /// </summary>
        public static ParkingRestrictionGroup MOTORCYCLES        { get; }
            = new ("MOTORCYCLES");

        #endregion


        #region Operator overloading

        #region Operator == (ParkingRestrictionGroup1, ParkingRestrictionGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestrictionGroup1">A parking restriction group.</param>
        /// <param name="ParkingRestrictionGroup2">Another parking restriction group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ParkingRestrictionGroup ParkingRestrictionGroup1,
                                           ParkingRestrictionGroup ParkingRestrictionGroup2)

            => ParkingRestrictionGroup1.Equals(ParkingRestrictionGroup2);

        #endregion

        #region Operator != (ParkingRestrictionGroup1, ParkingRestrictionGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestrictionGroup1">A parking restriction group.</param>
        /// <param name="ParkingRestrictionGroup2">Another parking restriction group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ParkingRestrictionGroup ParkingRestrictionGroup1,
                                           ParkingRestrictionGroup ParkingRestrictionGroup2)

            => !ParkingRestrictionGroup1.Equals(ParkingRestrictionGroup2);

        #endregion

        #region Operator <  (ParkingRestrictionGroup1, ParkingRestrictionGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestrictionGroup1">A parking restriction group.</param>
        /// <param name="ParkingRestrictionGroup2">Another parking restriction group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ParkingRestrictionGroup ParkingRestrictionGroup1,
                                          ParkingRestrictionGroup ParkingRestrictionGroup2)

            => ParkingRestrictionGroup1.CompareTo(ParkingRestrictionGroup2) < 0;

        #endregion

        #region Operator <= (ParkingRestrictionGroup1, ParkingRestrictionGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestrictionGroup1">A parking restriction group.</param>
        /// <param name="ParkingRestrictionGroup2">Another parking restriction group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ParkingRestrictionGroup ParkingRestrictionGroup1,
                                           ParkingRestrictionGroup ParkingRestrictionGroup2)

            => ParkingRestrictionGroup1.CompareTo(ParkingRestrictionGroup2) <= 0;

        #endregion

        #region Operator >  (ParkingRestrictionGroup1, ParkingRestrictionGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestrictionGroup1">A parking restriction group.</param>
        /// <param name="ParkingRestrictionGroup2">Another parking restriction group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ParkingRestrictionGroup ParkingRestrictionGroup1,
                                          ParkingRestrictionGroup ParkingRestrictionGroup2)

            => ParkingRestrictionGroup1.CompareTo(ParkingRestrictionGroup2) > 0;

        #endregion

        #region Operator >= (ParkingRestrictionGroup1, ParkingRestrictionGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestrictionGroup1">A parking restriction group.</param>
        /// <param name="ParkingRestrictionGroup2">Another parking restriction group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ParkingRestrictionGroup ParkingRestrictionGroup1,
                                           ParkingRestrictionGroup ParkingRestrictionGroup2)

            => ParkingRestrictionGroup1.CompareTo(ParkingRestrictionGroup2) >= 0;

        #endregion

        #endregion

        #region IComparable<ParkingRestrictionGroup> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two parking restriction groups.
        /// </summary>
        /// <param name="Object">A parking restriction group to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ParkingRestrictionGroup parkingRestrictionGroup
                   ? CompareTo(parkingRestrictionGroup)
                   : throw new ArgumentException("The given object is not a parking restriction group!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ParkingRestrictionGroup)

        /// <summary>
        /// Compares two parking restriction groups.
        /// </summary>
        /// <param name="ParkingRestrictionGroup">A parking restriction group to compare with.</param>
        public Int32 CompareTo(ParkingRestrictionGroup ParkingRestrictionGroup)

            => String.Compare(InternalId,
                              ParkingRestrictionGroup.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ParkingRestrictionGroup> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two parking restriction groups for equality.
        /// </summary>
        /// <param name="Object">A parking restriction group to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ParkingRestrictionGroup parkingRestrictionGroup &&
                   Equals(parkingRestrictionGroup);

        #endregion

        #region Equals(ParkingRestrictionGroup)

        /// <summary>
        /// Compares two parking restriction groups for equality.
        /// </summary>
        /// <param name="ParkingRestrictionGroup">A parking restriction group to compare with.</param>
        public Boolean Equals(ParkingRestrictionGroup ParkingRestrictionGroup)

            => String.Equals(InternalId,
                             ParkingRestrictionGroup.InternalId,
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
