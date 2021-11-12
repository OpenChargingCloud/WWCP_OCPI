/*
 * Copyright (c) 2015-2021 GraphDefined GmbH
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

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Facilities at a charge point location.
    /// May be used for user information.
    /// </summary>
    public struct Facilities : IId<Facilities>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty

            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the facility.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new facility based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the facility.</param>
        private Facilities(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a facility.
        /// </summary>
        /// <param name="Text">A text representation of a facility.</param>
        public static Facilities Parse(String Text)
        {

            if (TryParse(Text, out Facilities locationId))
                return locationId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a facility must not be null or empty!");

            throw new ArgumentException("The given text representation of a facility is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a facility.
        /// </summary>
        /// <param name="Text">A text representation of a facility.</param>
        public static Facilities? TryParse(String Text)
        {

            if (TryParse(Text, out Facilities locationId))
                return locationId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out VersionId)

        /// <summary>
        /// Try to parse the given text as a facility.
        /// </summary>
        /// <param name="Text">A text representation of a facility.</param>
        /// <param name="VersionId">The parsed facility.</param>
        public static Boolean TryParse(String Text, out Facilities VersionId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    VersionId = new Facilities(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            VersionId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this facility.
        /// </summary>
        public Facilities Clone

            => new Facilities(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        /// <summary>
        /// A hotel.
        /// </summary>
        public static Facilities HOTEL            = new Facilities("HOTEL");

        /// <summary>
        /// A restaurant.
        /// </summary>
        public static Facilities RESTAURANT       = new Facilities("RESTAURANT");

        /// <summary>
        /// A cafe.
        /// </summary>
        public static Facilities CAFE             = new Facilities("CAFE");

        /// <summary>
        /// A mall or shopping center.
        /// </summary>
        public static Facilities MALL             = new Facilities("MALL");

        /// <summary>
        /// A supermarket.
        /// </summary>
        public static Facilities SUPERMARKET      = new Facilities("SUPERMARKET");

        /// <summary>
        /// Sport facilities: gym, field etc.
        /// </summary>
        public static Facilities SPORT            = new Facilities("XXSPORTX");

        /// <summary>
        /// A recreation area.
        /// </summary>
        public static Facilities RECREATION_AREA  = new Facilities("RECREATION_AREA");

        /// <summary>
        /// Located in, or close to, a park, nature reserve etc.
        /// </summary>
        public static Facilities NATURE           = new Facilities("NATURE");

        /// <summary>
        /// A museum.
        /// </summary>
        public static Facilities MUSEUM           = new Facilities("MUSEUM");

        /// <summary>
        /// A bike/e-bike/e-scooter sharing location.
        /// </summary>
        public static Facilities BIKE_SHARING     = new Facilities("BIKE_SHARING");

        /// <summary>
        /// A bus stop.
        /// </summary>
        public static Facilities BUS_STOP         = new Facilities("BUS_STOP");

        /// <summary>
        /// A taxi stand.
        /// </summary>
        public static Facilities TAXI_STAND       = new Facilities("TAXI_STAND");

        /// <summary>
        /// A tram stop/station.
        /// </summary>
        public static Facilities TRAM_STOP        = new Facilities("TRAM_STOP");

        /// <summary>
        /// A metro station.
        /// </summary>
        public static Facilities METRO_STATION    = new Facilities("METRO_STATION");

        /// <summary>
        /// A train station.
        /// </summary>
        public static Facilities TRAIN_STATION    = new Facilities("TRAIN_STATION");

        /// <summary>
        /// An airport.
        /// </summary>
        public static Facilities AIRPORT          = new Facilities("AIRPORT");

        /// <summary>
        /// A parking lot.
        /// </summary>
        public static Facilities PARKING_LOT      = new Facilities("PARKING_LOT");

        /// <summary>
        /// A carpool parking.
        /// </summary>
        public static Facilities CARPOOL_PARKING  = new Facilities("CARPOOL_PARKING");

        /// <summary>
        /// A Fuel station.
        /// </summary>
        public static Facilities FUEL_STATION     = new Facilities("FUEL_STATION");

        /// <summary>
        /// Wifi or other type of internet available.
        /// </summary>
        public static Facilities WIFI             = new Facilities("WIFI");


        #region Operator overloading

        #region Operator == (VersionId1, VersionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionId1">A facility.</param>
        /// <param name="VersionId2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Facilities VersionId1,
                                           Facilities VersionId2)

            => VersionId1.Equals(VersionId2);

        #endregion

        #region Operator != (VersionId1, VersionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionId1">A facility.</param>
        /// <param name="VersionId2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Facilities VersionId1,
                                           Facilities VersionId2)

            => !(VersionId1 == VersionId2);

        #endregion

        #region Operator <  (VersionId1, VersionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionId1">A facility.</param>
        /// <param name="VersionId2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Facilities VersionId1,
                                          Facilities VersionId2)

            => VersionId1.CompareTo(VersionId2) < 0;

        #endregion

        #region Operator <= (VersionId1, VersionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionId1">A facility.</param>
        /// <param name="VersionId2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Facilities VersionId1,
                                           Facilities VersionId2)

            => !(VersionId1 > VersionId2);

        #endregion

        #region Operator >  (VersionId1, VersionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionId1">A facility.</param>
        /// <param name="VersionId2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Facilities VersionId1,
                                          Facilities VersionId2)

            => VersionId1.CompareTo(VersionId2) > 0;

        #endregion

        #region Operator >= (VersionId1, VersionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionId1">A facility.</param>
        /// <param name="VersionId2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Facilities VersionId1,
                                           Facilities VersionId2)

            => !(VersionId1 < VersionId2);

        #endregion

        #endregion

        #region IComparable<VersionId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Facilities locationId
                   ? CompareTo(locationId)
                   : throw new ArgumentException("The given object is not a facility!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(VersionId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionId">An object to compare with.</param>
        public Int32 CompareTo(Facilities VersionId)

            => String.Compare(InternalId,
                              VersionId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<VersionId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Facilities locationId &&
                   Equals(locationId);

        #endregion

        #region Equals(VersionId)

        /// <summary>
        /// Compares two facilitys for equality.
        /// </summary>
        /// <param name="VersionId">An facility to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Facilities VersionId)

            => String.Equals(InternalId,
                             VersionId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

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
