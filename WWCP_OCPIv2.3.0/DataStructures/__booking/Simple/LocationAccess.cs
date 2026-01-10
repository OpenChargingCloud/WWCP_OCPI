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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Extension methods for location access.
    /// </summary>
    public static class LocationAccessExtensions
    {

        /// <summary>
        /// Indicates whether this location access is null or empty.
        /// </summary>
        /// <param name="LocationAccess">A location access.</param>
        public static Boolean IsNullOrEmpty(this LocationAccess? LocationAccess)
            => !LocationAccess.HasValue || LocationAccess.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this location access is NOT null or empty.
        /// </summary>
        /// <param name="LocationAccess">A location access.</param>
        public static Boolean IsNotNullOrEmpty(this LocationAccess? LocationAccess)
            => LocationAccess.HasValue && LocationAccess.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The location access.
    /// </summary>
    public readonly struct LocationAccess : IId<LocationAccess>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this location access is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this location access is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the location access.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new location access based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a location access.</param>
        private LocationAccess(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a location access.
        /// </summary>
        /// <param name="Text">A text representation of a location access.</param>
        public static LocationAccess Parse(String Text)
        {

            if (TryParse(Text, out var locationAccess))
                return locationAccess;

            throw new ArgumentException($"Invalid text representation of a location access: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a location access.
        /// </summary>
        /// <param name="Text">A text representation of a location access.</param>
        public static LocationAccess? TryParse(String Text)
        {

            if (TryParse(Text, out var locationAccess))
                return locationAccess;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out LocationAccess)

        /// <summary>
        /// Try to parse the given text as a location access.
        /// </summary>
        /// <param name="Text">A text representation of a location access.</param>
        /// <param name="LocationAccess">The parsed location access.</param>
        public static Boolean TryParse(String Text, out LocationAccess LocationAccess)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    LocationAccess = new LocationAccess(Text);
                    return true;
                }
                catch
                { }
            }

            LocationAccess = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this location access.
        /// </summary>
        public LocationAccess Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Open access to the site.
        /// </summary>
        public static LocationAccess  OPEN              { get; }
            = new ("OPEN");

        /// <summary>
        /// Using a token in that was sent in the booking.
        /// </summary>
        public static LocationAccess  TOKEN             { get; }
            = new ("TOKEN");

        /// <summary>
        /// The license plate(s) of the vehicle that wants to charge.
        /// </summary>
        public static LocationAccess  LICENSE_PLATE     { get; }
            = new ("LICENSE_PLATE");

        /// <summary>
        /// The access code provided.
        /// </summary>
        public static LocationAccess  ACCESS_CODE       { get; }
            = new ("ACCESS_CODE");

        /// <summary>
        /// Get access to the charging station by ringing the intercom.
        /// </summary>
        public static LocationAccess  INTERCOM          { get; }
            = new ("INTERCOM");

        /// <summary>
        /// Parking ticket required.
        /// </summary>
        public static LocationAccess  PARKING_TICKET    { get; }
            = new ("PENDING");

        #endregion


        #region Operator overloading

        #region Operator == (LocationAccess1, LocationAccess2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationAccess1">A location access.</param>
        /// <param name="LocationAccess2">Another location access.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (LocationAccess LocationAccess1,
                                           LocationAccess LocationAccess2)

            => LocationAccess1.Equals(LocationAccess2);

        #endregion

        #region Operator != (LocationAccess1, LocationAccess2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationAccess1">A location access.</param>
        /// <param name="LocationAccess2">Another location access.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (LocationAccess LocationAccess1,
                                           LocationAccess LocationAccess2)

            => !LocationAccess1.Equals(LocationAccess2);

        #endregion

        #region Operator <  (LocationAccess1, LocationAccess2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationAccess1">A location access.</param>
        /// <param name="LocationAccess2">Another location access.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (LocationAccess LocationAccess1,
                                          LocationAccess LocationAccess2)

            => LocationAccess1.CompareTo(LocationAccess2) < 0;

        #endregion

        #region Operator <= (LocationAccess1, LocationAccess2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationAccess1">A location access.</param>
        /// <param name="LocationAccess2">Another location access.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (LocationAccess LocationAccess1,
                                           LocationAccess LocationAccess2)

            => LocationAccess1.CompareTo(LocationAccess2) <= 0;

        #endregion

        #region Operator >  (LocationAccess1, LocationAccess2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationAccess1">A location access.</param>
        /// <param name="LocationAccess2">Another location access.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (LocationAccess LocationAccess1,
                                          LocationAccess LocationAccess2)

            => LocationAccess1.CompareTo(LocationAccess2) > 0;

        #endregion

        #region Operator >= (LocationAccess1, LocationAccess2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationAccess1">A location access.</param>
        /// <param name="LocationAccess2">Another location access.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (LocationAccess LocationAccess1,
                                           LocationAccess LocationAccess2)

            => LocationAccess1.CompareTo(LocationAccess2) >= 0;

        #endregion

        #endregion

        #region IComparable<LocationAccess> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two location access.
        /// </summary>
        /// <param name="Object">A location access to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is LocationAccess locationAccess
                   ? CompareTo(locationAccess)
                   : throw new ArgumentException("The given object is not a location access!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LocationAccess)

        /// <summary>
        /// Compares two location access.
        /// </summary>
        /// <param name="LocationAccess">A location access to compare with.</param>
        public Int32 CompareTo(LocationAccess LocationAccess)

            => String.Compare(InternalId,
                              LocationAccess.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<LocationAccess> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two location access for equality.
        /// </summary>
        /// <param name="Object">A location access to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LocationAccess locationAccess &&
                   Equals(locationAccess);

        #endregion

        #region Equals(LocationAccess)

        /// <summary>
        /// Compares two location access for equality.
        /// </summary>
        /// <param name="LocationAccess">A location access to compare with.</param>
        public Boolean Equals(LocationAccess LocationAccess)

            => String.Equals(InternalId,
                             LocationAccess.InternalId,
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
