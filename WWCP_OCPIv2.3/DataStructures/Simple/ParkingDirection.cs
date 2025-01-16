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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Extension methods for parking directions.
    /// </summary>
    public static class ParkingDirectionExtensions
    {

        /// <summary>
        /// Indicates whether this parking direction is null or empty.
        /// </summary>
        /// <param name="ParkingDirection">A parking direction.</param>
        public static Boolean IsNullOrEmpty(this ParkingDirection? ParkingDirection)
            => !ParkingDirection.HasValue || ParkingDirection.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this parking direction is NOT null or empty.
        /// </summary>
        /// <param name="ParkingDirection">A parking direction.</param>
        public static Boolean IsNotNullOrEmpty(this ParkingDirection? ParkingDirection)
            => ParkingDirection.HasValue && ParkingDirection.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A parking direction.
    /// </summary>
    public readonly struct ParkingDirection : IId<ParkingDirection>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this parking direction is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this parking direction is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the parking direction.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new parking direction based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a parking direction.</param>
        private ParkingDirection(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a parking direction.
        /// </summary>
        /// <param name="Text">A text representation of a parking direction.</param>
        public static ParkingDirection Parse(String Text)
        {

            if (TryParse(Text, out var parkingDirection))
                return parkingDirection;

            throw new ArgumentException($"Invalid text representation of a parking direction: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a parking direction.
        /// </summary>
        /// <param name="Text">A text representation of a parking direction.</param>
        public static ParkingDirection? TryParse(String Text)
        {

            if (TryParse(Text, out var parkingDirection))
                return parkingDirection;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ParkingDirection)

        /// <summary>
        /// Try to parse the given text as a parking direction.
        /// </summary>
        /// <param name="Text">A text representation of a parking direction.</param>
        /// <param name="ParkingDirection">The parsed parking direction.</param>
        public static Boolean TryParse(String Text, out ParkingDirection ParkingDirection)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ParkingDirection = new ParkingDirection(Text);
                    return true;
                }
                catch
                { }
            }

            ParkingDirection = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this parking direction.
        /// </summary>
        public ParkingDirection Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Parking happens parallel to the roadway on which vehicles approach the EVSE.
        /// </summary>
        public static ParkingDirection  PARALLEL         { get; }
            = new ("PARALLEL");

        /// <summary>
        /// Parking happens perpendicular to the roadway on which vehicles approach the EVSE.
        /// </summary>
        public static ParkingDirection  PERPENDICULAR    { get; }
            = new ("PERPENDICULAR");

        /// <summary>
        /// Parking happens at an angle to the roadway on which vehicles approach the EVSE (i.e. echelon parking).
        /// </summary>
        public static ParkingDirection  ANGLE            { get; }
            = new ("ANGLE");

        #endregion


        #region Operator overloading

        #region Operator == (ParkingDirection1, ParkingDirection2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingDirection1">A parking direction.</param>
        /// <param name="ParkingDirection2">Another parking direction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ParkingDirection ParkingDirection1,
                                           ParkingDirection ParkingDirection2)

            => ParkingDirection1.Equals(ParkingDirection2);

        #endregion

        #region Operator != (ParkingDirection1, ParkingDirection2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingDirection1">A parking direction.</param>
        /// <param name="ParkingDirection2">Another parking direction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ParkingDirection ParkingDirection1,
                                           ParkingDirection ParkingDirection2)

            => !ParkingDirection1.Equals(ParkingDirection2);

        #endregion

        #region Operator <  (ParkingDirection1, ParkingDirection2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingDirection1">A parking direction.</param>
        /// <param name="ParkingDirection2">Another parking direction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ParkingDirection ParkingDirection1,
                                          ParkingDirection ParkingDirection2)

            => ParkingDirection1.CompareTo(ParkingDirection2) < 0;

        #endregion

        #region Operator <= (ParkingDirection1, ParkingDirection2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingDirection1">A parking direction.</param>
        /// <param name="ParkingDirection2">Another parking direction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ParkingDirection ParkingDirection1,
                                           ParkingDirection ParkingDirection2)

            => ParkingDirection1.CompareTo(ParkingDirection2) <= 0;

        #endregion

        #region Operator >  (ParkingDirection1, ParkingDirection2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingDirection1">A parking direction.</param>
        /// <param name="ParkingDirection2">Another parking direction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ParkingDirection ParkingDirection1,
                                          ParkingDirection ParkingDirection2)

            => ParkingDirection1.CompareTo(ParkingDirection2) > 0;

        #endregion

        #region Operator >= (ParkingDirection1, ParkingDirection2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingDirection1">A parking direction.</param>
        /// <param name="ParkingDirection2">Another parking direction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ParkingDirection ParkingDirection1,
                                           ParkingDirection ParkingDirection2)

            => ParkingDirection1.CompareTo(ParkingDirection2) >= 0;

        #endregion

        #endregion

        #region IComparable<ParkingDirection> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two parking directions.
        /// </summary>
        /// <param name="Object">A parking direction to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ParkingDirection parkingDirection
                   ? CompareTo(parkingDirection)
                   : throw new ArgumentException("The given object is not a parking direction!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ParkingDirection)

        /// <summary>
        /// Compares two parking directions.
        /// </summary>
        /// <param name="ParkingDirection">A parking direction to compare with.</param>
        public Int32 CompareTo(ParkingDirection ParkingDirection)

            => String.Compare(InternalId,
                              ParkingDirection.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ParkingDirection> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two parking directions for equality.
        /// </summary>
        /// <param name="Object">A parking direction to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ParkingDirection parkingDirection &&
                   Equals(parkingDirection);

        #endregion

        #region Equals(ParkingDirection)

        /// <summary>
        /// Compares two parking directions for equality.
        /// </summary>
        /// <param name="ParkingDirection">A parking direction to compare with.</param>
        public Boolean Equals(ParkingDirection ParkingDirection)

            => String.Equals(InternalId,
                             ParkingDirection.InternalId,
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
