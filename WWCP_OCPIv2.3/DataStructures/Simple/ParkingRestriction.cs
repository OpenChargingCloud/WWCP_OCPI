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

namespace cloud.charging.open.protocols.OCPIv2_3
{

    /// <summary>
    /// Extension methods for parking restrictions.
    /// </summary>
    public static class ParkingRestrictionExtensions
    {

        /// <summary>
        /// Indicates whether this parking restriction is null or empty.
        /// </summary>
        /// <param name="ParkingRestriction">A parking restriction.</param>
        public static Boolean IsNullOrEmpty(this ParkingRestriction? ParkingRestriction)
            => !ParkingRestriction.HasValue || ParkingRestriction.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this parking restriction is NOT null or empty.
        /// </summary>
        /// <param name="ParkingRestriction">A parking restriction.</param>
        public static Boolean IsNotNullOrEmpty(this ParkingRestriction? ParkingRestriction)
            => ParkingRestriction.HasValue && ParkingRestriction.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The parking restriction.
    /// </summary>
    public readonly struct ParkingRestriction : IId<ParkingRestriction>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this parking restriction is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this parking restriction is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the parking restriction.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new parking restriction based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a parking restriction.</param>
        private ParkingRestriction(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a parking restriction.
        /// </summary>
        /// <param name="Text">A text representation of a parking restriction.</param>
        public static ParkingRestriction Parse(String Text)
        {

            if (TryParse(Text, out var parkingType))
                return parkingType;

            throw new ArgumentException($"Invalid text representation of a parking restriction: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a parking restriction.
        /// </summary>
        /// <param name="Text">A text representation of a parking restriction.</param>
        public static ParkingRestriction? TryParse(String Text)
        {

            if (TryParse(Text, out var parkingType))
                return parkingType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ParkingRestriction)

        /// <summary>
        /// Try to parse the given text as a parking restriction.
        /// </summary>
        /// <param name="Text">A text representation of a parking restriction.</param>
        /// <param name="ParkingRestriction">The parsed parking restriction.</param>
        public static Boolean TryParse(String Text, out ParkingRestriction ParkingRestriction)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ParkingRestriction = new ParkingRestriction(Text);
                    return true;
                }
                catch
                { }
            }

            ParkingRestriction = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this parking restriction.
        /// </summary>
        public ParkingRestriction Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Parking only for people who work at a site, building, or complex that the Location belongs to.
        /// </summary>
        public static ParkingRestriction EMPLOYEES      { get; }
            = new ("EMPLOYEES");

        /// <summary>
        /// Reserved parking spot for electric vehicles.
        /// </summary>
        public static ParkingRestriction EV_ONLY        { get; }
            = new ("EV_ONLY");

        /// <summary>
        /// Parking allowed only while plugged in (charging).
        /// </summary>
        public static ParkingRestriction PLUGGED        { get; }
            = new ("PLUGGED");

        /// <summary>
        /// Reserved parking spot for disabled people with valid ID.
        /// </summary>
        public static ParkingRestriction DISABLED       { get; }
            = new ("DISABLED");

        /// <summary>
        /// Parking spot for customers/guests only, for example in case of a hotel or shop.
        /// </summary>
        public static ParkingRestriction CUSTOMERS      { get; }
            = new ("CUSTOMERS");

        /// <summary>
        /// Parking only for taxi vehicles.
        /// </summary>
        public static ParkingRestriction TAXI           { get; }
            = new ("TAXI");

        /// <summary>
        /// Parking only for people who live in a complex that the Location belongs to.
        /// </summary>
        public static ParkingRestriction TENANTS        { get; }
            = new ("TENANTS");

        #endregion


        #region Operator overloading

        #region Operator == (ParkingRestriction1, ParkingRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestriction1">A parking restriction.</param>
        /// <param name="ParkingRestriction2">Another parking restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ParkingRestriction ParkingRestriction1,
                                           ParkingRestriction ParkingRestriction2)

            => ParkingRestriction1.Equals(ParkingRestriction2);

        #endregion

        #region Operator != (ParkingRestriction1, ParkingRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestriction1">A parking restriction.</param>
        /// <param name="ParkingRestriction2">Another parking restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ParkingRestriction ParkingRestriction1,
                                           ParkingRestriction ParkingRestriction2)

            => !ParkingRestriction1.Equals(ParkingRestriction2);

        #endregion

        #region Operator <  (ParkingRestriction1, ParkingRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestriction1">A parking restriction.</param>
        /// <param name="ParkingRestriction2">Another parking restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ParkingRestriction ParkingRestriction1,
                                          ParkingRestriction ParkingRestriction2)

            => ParkingRestriction1.CompareTo(ParkingRestriction2) < 0;

        #endregion

        #region Operator <= (ParkingRestriction1, ParkingRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestriction1">A parking restriction.</param>
        /// <param name="ParkingRestriction2">Another parking restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ParkingRestriction ParkingRestriction1,
                                           ParkingRestriction ParkingRestriction2)

            => ParkingRestriction1.CompareTo(ParkingRestriction2) <= 0;

        #endregion

        #region Operator >  (ParkingRestriction1, ParkingRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestriction1">A parking restriction.</param>
        /// <param name="ParkingRestriction2">Another parking restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ParkingRestriction ParkingRestriction1,
                                          ParkingRestriction ParkingRestriction2)

            => ParkingRestriction1.CompareTo(ParkingRestriction2) > 0;

        #endregion

        #region Operator >= (ParkingRestriction1, ParkingRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingRestriction1">A parking restriction.</param>
        /// <param name="ParkingRestriction2">Another parking restriction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ParkingRestriction ParkingRestriction1,
                                           ParkingRestriction ParkingRestriction2)

            => ParkingRestriction1.CompareTo(ParkingRestriction2) >= 0;

        #endregion

        #endregion

        #region IComparable<ParkingRestriction> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two parking restrictions.
        /// </summary>
        /// <param name="Object">A parking restriction to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ParkingRestriction parkingType
                   ? CompareTo(parkingType)
                   : throw new ArgumentException("The given object is not a parking restriction!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ParkingRestriction)

        /// <summary>
        /// Compares two parking restrictions.
        /// </summary>
        /// <param name="ParkingRestriction">A parking restriction to compare with.</param>
        public Int32 CompareTo(ParkingRestriction ParkingRestriction)

            => String.Compare(InternalId,
                              ParkingRestriction.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ParkingRestriction> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two parking restrictions for equality.
        /// </summary>
        /// <param name="Object">A parking restriction to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ParkingRestriction parkingType &&
                   Equals(parkingType);

        #endregion

        #region Equals(ParkingRestriction)

        /// <summary>
        /// Compares two parking restrictions for equality.
        /// </summary>
        /// <param name="ParkingRestriction">A parking restriction to compare with.</param>
        public Boolean Equals(ParkingRestriction ParkingRestriction)

            => String.Equals(InternalId,
                             ParkingRestriction.InternalId,
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
