/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// Extension methods for parking types.
    /// </summary>
    public static class ParkingTypeExtensions
    {

        /// <summary>
        /// Indicates whether this parking type is null or empty.
        /// </summary>
        /// <param name="ParkingType">A parking type.</param>
        public static Boolean IsNullOrEmpty(this ParkingType? ParkingType)
            => !ParkingType.HasValue || ParkingType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this parking type is NOT null or empty.
        /// </summary>
        /// <param name="ParkingType">A parking type.</param>
        public static Boolean IsNotNullOrEmpty(this ParkingType? ParkingType)
            => ParkingType.HasValue && ParkingType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The parking type.
    /// </summary>
    public readonly struct ParkingType : IId<ParkingType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this parking type is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this parking type is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the parking type.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new parking type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a parking type.</param>
        private ParkingType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a parking type.
        /// </summary>
        /// <param name="Text">A text representation of a parking type.</param>
        public static ParkingType Parse(String Text)
        {

            if (TryParse(Text, out var parkingType))
                return parkingType;

            throw new ArgumentException($"Invalid text representation of a parking type: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a parking type.
        /// </summary>
        /// <param name="Text">A text representation of a parking type.</param>
        public static ParkingType? TryParse(String Text)
        {

            if (TryParse(Text, out var parkingType))
                return parkingType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ParkingType)

        /// <summary>
        /// Try to parse the given text as a parking type.
        /// </summary>
        /// <param name="Text">A text representation of a parking type.</param>
        /// <param name="ParkingType">The parsed parking type.</param>
        public static Boolean TryParse(String Text, out ParkingType ParkingType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ParkingType = new ParkingType(Text);
                    return true;
                }
                catch
                { }
            }

            ParkingType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this parking type.
        /// </summary>
        public ParkingType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Parking location type is not known by the operator (default).
        /// </summary>
        public static ParkingType UNKNOWN               { get; }
            = new ("UNKNOWN");

        /// <summary>
        /// Location on a parking facility/rest area along a motorway, freeway, interstate, highway etc.
        /// </summary>
        public static ParkingType ALONG_MOTORWAY        { get; }
            = new ("ALONG_MOTORWAY");

        /// <summary>
        /// Multistorey car park.
        /// </summary>
        public static ParkingType PARKING_GARAGE        { get; }
            = new ("PARKING_GARAGE");

        /// <summary>
        /// A cleared area that is intended for parking vehicles, i.e.at super markets, bars, etc.
        /// </summary>
        public static ParkingType PARKING_LOT           { get; }
            = new ("PARKING_LOT");

        /// <summary>
        /// Location is on the driveway of a house/building.
        /// </summary>
        public static ParkingType ON_DRIVEWAY           { get; }
            = new ("ON_DRIVEWAY");

        /// <summary>
        /// Parking in public space.
        /// </summary>
        public static ParkingType ON_STREET             { get; }
            = new ("ON_STREET");

        /// <summary>
        /// Multistorey car park, mainly underground.
        /// </summary>
        public static ParkingType UNDERGROUND_GARAGE    { get; }
            = new ("UNDERGROUND_GARAGE");

        #endregion


        #region Operator overloading

        #region Operator == (ParkingType1, ParkingType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingType1">A parking type.</param>
        /// <param name="ParkingType2">Another parking type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ParkingType ParkingType1,
                                           ParkingType ParkingType2)

            => ParkingType1.Equals(ParkingType2);

        #endregion

        #region Operator != (ParkingType1, ParkingType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingType1">A parking type.</param>
        /// <param name="ParkingType2">Another parking type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ParkingType ParkingType1,
                                           ParkingType ParkingType2)

            => !ParkingType1.Equals(ParkingType2);

        #endregion

        #region Operator <  (ParkingType1, ParkingType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingType1">A parking type.</param>
        /// <param name="ParkingType2">Another parking type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ParkingType ParkingType1,
                                          ParkingType ParkingType2)

            => ParkingType1.CompareTo(ParkingType2) < 0;

        #endregion

        #region Operator <= (ParkingType1, ParkingType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingType1">A parking type.</param>
        /// <param name="ParkingType2">Another parking type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ParkingType ParkingType1,
                                           ParkingType ParkingType2)

            => ParkingType1.CompareTo(ParkingType2) <= 0;

        #endregion

        #region Operator >  (ParkingType1, ParkingType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingType1">A parking type.</param>
        /// <param name="ParkingType2">Another parking type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ParkingType ParkingType1,
                                          ParkingType ParkingType2)

            => ParkingType1.CompareTo(ParkingType2) > 0;

        #endregion

        #region Operator >= (ParkingType1, ParkingType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingType1">A parking type.</param>
        /// <param name="ParkingType2">Another parking type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ParkingType ParkingType1,
                                           ParkingType ParkingType2)

            => ParkingType1.CompareTo(ParkingType2) >= 0;

        #endregion

        #endregion

        #region IComparable<ParkingType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two parking types.
        /// </summary>
        /// <param name="Object">A parking type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ParkingType parkingType
                   ? CompareTo(parkingType)
                   : throw new ArgumentException("The given object is not a parking type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ParkingType)

        /// <summary>
        /// Compares two parking types.
        /// </summary>
        /// <param name="ParkingType">A parking type to compare with.</param>
        public Int32 CompareTo(ParkingType ParkingType)

            => String.Compare(InternalId,
                              ParkingType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ParkingType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two parking types for equality.
        /// </summary>
        /// <param name="Object">A parking type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ParkingType parkingType &&
                   Equals(parkingType);

        #endregion

        #region Equals(ParkingType)

        /// <summary>
        /// Compares two parking types for equality.
        /// </summary>
        /// <param name="ParkingType">A parking type to compare with.</param>
        public Boolean Equals(ParkingType ParkingType)

            => String.Equals(InternalId,
                             ParkingType.InternalId,
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
