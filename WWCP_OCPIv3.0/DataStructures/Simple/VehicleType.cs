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

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for vehicle types.
    /// </summary>
    public static class VehicleTypeExtensions
    {

        /// <summary>
        /// Indicates whether this vehicle type is null or empty.
        /// </summary>
        /// <param name="VehicleType">A vehicle type.</param>
        public static Boolean IsNullOrEmpty(this VehicleType? VehicleType)
            => !VehicleType.HasValue || VehicleType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this vehicle type is NOT null or empty.
        /// </summary>
        /// <param name="VehicleType">A vehicle type.</param>
        public static Boolean IsNotNullOrEmpty(this VehicleType? VehicleType)
            => VehicleType.HasValue && VehicleType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A vehicle type.
    /// </summary>
    public readonly struct VehicleType : IId<VehicleType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this vehicle type is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this vehicle type is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the vehicle type.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new vehicle type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a vehicle type.</param>
        private VehicleType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a vehicle type.
        /// </summary>
        /// <param name="Text">A text representation of a vehicle type.</param>
        public static VehicleType Parse(String Text)
        {

            if (TryParse(Text, out var vehicleType))
                return vehicleType;

            throw new ArgumentException($"Invalid text representation of a vehicle type: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a vehicle type.
        /// </summary>
        /// <param name="Text">A text representation of a vehicle type.</param>
        public static VehicleType? TryParse(String Text)
        {

            if (TryParse(Text, out var vehicleType))
                return vehicleType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out VehicleType)

        /// <summary>
        /// Try to parse the given text as a vehicle type.
        /// </summary>
        /// <param name="Text">A text representation of a vehicle type.</param>
        /// <param name="VehicleType">The parsed vehicle type.</param>
        public static Boolean TryParse(String Text, out VehicleType VehicleType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    VehicleType = new VehicleType(Text);
                    return true;
                }
                catch
                { }
            }

            VehicleType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this vehicle type.
        /// </summary>
        public VehicleType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// A motorcycle.
        /// </summary>
        public static VehicleType  MOTORCYCLE                       { get; }
            = new ("MOTORCYCLE");

        /// <summary>
        /// A personal vehicle, a passenger car.
        /// </summary>
        public static VehicleType  PERSONAL_VEHICLE                 { get; }
            = new ("PERSONAL_VEHICLE");

        /// <summary>
        /// A personal vehicle with a trailer attached.
        /// </summary>
        public static VehicleType  PERSONAL_VEHICLE_WITH_TRAILER    { get; }
            = new ("PERSONAL_VEHICLE_WITH_TRAILER");

        /// <summary>
        /// A light-duty van with a height smaller than 275 cm.
        /// </summary>
        public static VehicleType  VAN                              { get; }
            = new ("VAN");

        /// <summary>
        /// A heavy-duty truck without a trailer.
        /// </summary>
        public static VehicleType  TRUCK                            { get; }
            = new ("TRUCK");

        /// <summary>
        /// A heavy-duty truck with a trailer attached.
        /// </summary>
        public static VehicleType  TRUCK_WITH_TRAILER               { get; }
            = new ("TRUCK_WITH_TRAILER");

        /// <summary>
        /// A bus or a motor coach.
        /// </summary>
        public static VehicleType  BUS                              { get; }
            = new ("BUS");

        /// <summary>
        /// A vehicle with a permit for parking spaces for people with disabilities.
        /// </summary>
        public static VehicleType  DISABLED                         { get; }
            = new ("DISABLED");

        #endregion


        #region Operator overloading

        #region Operator == (VehicleType1, VehicleType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VehicleType1">A vehicle type.</param>
        /// <param name="VehicleType2">Another vehicle type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VehicleType VehicleType1,
                                           VehicleType VehicleType2)

            => VehicleType1.Equals(VehicleType2);

        #endregion

        #region Operator != (VehicleType1, VehicleType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VehicleType1">A vehicle type.</param>
        /// <param name="VehicleType2">Another vehicle type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VehicleType VehicleType1,
                                           VehicleType VehicleType2)

            => !VehicleType1.Equals(VehicleType2);

        #endregion

        #region Operator <  (VehicleType1, VehicleType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VehicleType1">A vehicle type.</param>
        /// <param name="VehicleType2">Another vehicle type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (VehicleType VehicleType1,
                                          VehicleType VehicleType2)

            => VehicleType1.CompareTo(VehicleType2) < 0;

        #endregion

        #region Operator <= (VehicleType1, VehicleType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VehicleType1">A vehicle type.</param>
        /// <param name="VehicleType2">Another vehicle type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (VehicleType VehicleType1,
                                           VehicleType VehicleType2)

            => VehicleType1.CompareTo(VehicleType2) <= 0;

        #endregion

        #region Operator >  (VehicleType1, VehicleType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VehicleType1">A vehicle type.</param>
        /// <param name="VehicleType2">Another vehicle type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (VehicleType VehicleType1,
                                          VehicleType VehicleType2)

            => VehicleType1.CompareTo(VehicleType2) > 0;

        #endregion

        #region Operator >= (VehicleType1, VehicleType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VehicleType1">A vehicle type.</param>
        /// <param name="VehicleType2">Another vehicle type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (VehicleType VehicleType1,
                                           VehicleType VehicleType2)

            => VehicleType1.CompareTo(VehicleType2) >= 0;

        #endregion

        #endregion

        #region IComparable<VehicleType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two vehicle types.
        /// </summary>
        /// <param name="Object">A vehicle type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is VehicleType vehicleType
                   ? CompareTo(vehicleType)
                   : throw new ArgumentException("The given object is not a vehicle type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(VehicleType)

        /// <summary>
        /// Compares two vehicle types.
        /// </summary>
        /// <param name="VehicleType">A vehicle type to compare with.</param>
        public Int32 CompareTo(VehicleType VehicleType)

            => String.Compare(InternalId,
                              VehicleType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<VehicleType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two vehicle types for equality.
        /// </summary>
        /// <param name="Object">A vehicle type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VehicleType vehicleType &&
                   Equals(vehicleType);

        #endregion

        #region Equals(VehicleType)

        /// <summary>
        /// Compares two vehicle types for equality.
        /// </summary>
        /// <param name="VehicleType">A vehicle type to compare with.</param>
        public Boolean Equals(VehicleType VehicleType)

            => String.Equals(InternalId,
                             VehicleType.InternalId,
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
