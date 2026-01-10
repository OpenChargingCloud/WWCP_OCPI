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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Extension methods for facilities.
    /// </summary>
    public static class FacilitiesExtensions
    {

        /// <summary>
        /// Indicates whether this facility is null or empty.
        /// </summary>
        /// <param name="Facilities">A facility.</param>
        public static Boolean IsNullOrEmpty(this Facilities? Facilities)
            => !Facilities.HasValue || Facilities.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this facility is NOT null or empty.
        /// </summary>
        /// <param name="Facilities">A facility.</param>
        public static Boolean IsNotNullOrEmpty(this Facilities? Facilities)
            => Facilities.HasValue && Facilities.Value.IsNotNullOrEmpty;


        #region Matches(DisplayTexts, Match, IgnoreCase = true)

        /// <summary>
        /// Checks whether the given enumeration of facilities matches the given text.
        /// </summary>
        /// <param name="Facilities">An enumeration of facilities.</param>
        /// <param name="Match">A text to match.</param>
        /// <param name="IgnoreCase">Whether to ignore the case of the text.</param>
        public static Boolean Matches(this IEnumerable<Facilities>  Facilities,
                                      String                        Match,
                                      Boolean                       IgnoreCase  = true)

            => Facilities.Any(facilitiy => IgnoreCase
                                               ? facilitiy.ToString().Contains(Match, StringComparison.OrdinalIgnoreCase)
                                               : facilitiy.ToString().Contains(Match, StringComparison.Ordinal));

        #endregion


    }


    /// <summary>
    /// The unique identification of a facility.
    /// </summary>
    public readonly struct Facilities : IId<Facilities>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this facility is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this facility is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the facility.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new facility based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a facility.</param>
        private Facilities(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a facility.
        /// </summary>
        /// <param name="Text">A text representation of a facility.</param>
        public static Facilities Parse(String Text)
        {

            if (TryParse(Text, out var facilityId))
                return facilityId;

            throw new ArgumentException($"Invalid text representation of a facility: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a facility.
        /// </summary>
        /// <param name="Text">A text representation of a facility.</param>
        public static Facilities? TryParse(String Text)
        {

            if (TryParse(Text, out var facilityId))
                return facilityId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Facilities)

        /// <summary>
        /// Try to parse the given text as a facility.
        /// </summary>
        /// <param name="Text">A text representation of a facility.</param>
        /// <param name="Facilities">The parsed facility.</param>
        public static Boolean TryParse(String Text, out Facilities Facilities)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    Facilities = new Facilities(Text);
                    return true;
                }
                catch
                { }
            }

            Facilities = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this facility.
        /// </summary>
        public Facilities Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definition

        /// <summary>
        /// A hotel.
        /// </summary>
        public static Facilities HOTEL
            => new ("HOTEL");

        /// <summary>
        /// A restaurant.
        /// </summary>
        public static Facilities RESTAURANT
            => new ("RESTAURANT");

        /// <summary>
        /// A cafe.
        /// </summary>
        public static Facilities CAFE
            => new ("CAFE");

        /// <summary>
        /// A mall or shopping center.
        /// </summary>
        public static Facilities MALL
            => new ("MALL");

        /// <summary>
        /// A supermarket.
        /// </summary>
        public static Facilities SUPERMARKET
            => new ("SUPERMARKET");

        /// <summary>
        /// Sport facilities: gym, field etc.
        /// </summary>
        public static Facilities SPORT
            => new ("SPORT");

        /// <summary>
        /// A recreation area.
        /// </summary>
        public static Facilities RECREATION_AREA
            => new ("RECREATION_AREA");

        /// <summary>
        /// Located in, or close to, a park, nature reserve etc.
        /// </summary>
        public static Facilities NATURE
            => new ("NATURE");

        /// <summary>
        /// A museum.
        /// </summary>
        public static Facilities MUSEUM
            => new ("MUSEUM");

        /// <summary>
        /// A bike/e-bike/e-scooter sharing location.
        /// </summary>
        public static Facilities BIKE_SHARING
            => new ("BIKE_SHARING");

        /// <summary>
        /// A bus stop.
        /// </summary>
        public static Facilities BUS_STOP
            => new ("BUS_STOP");

        /// <summary>
        /// A taxi stand.
        /// </summary>
        public static Facilities TAXI_STAND
            => new ("TAXI_STAND");

        /// <summary>
        /// A tram stop/station.
        /// </summary>
        public static Facilities TRAM_STOP
            => new ("TRAM_STOP");

        /// <summary>
        /// A metro station.
        /// </summary>
        public static Facilities METRO_STATION
            => new ("METRO_STATION");

        /// <summary>
        /// A train station.
        /// </summary>
        public static Facilities TRAIN_STATION
            => new ("TRAIN_STATION");

        /// <summary>
        /// An airport.
        /// </summary>
        public static Facilities AIRPORT
            => new ("AIRPORT");

        /// <summary>
        /// A parking lot.
        /// </summary>
        public static Facilities PARKING_LOT
            => new ("PARKING_LOT");

        /// <summary>
        /// A carpool parking.
        /// </summary>
        public static Facilities CARPOOL_PARKING
            => new ("CARPOOL_PARKING");

        /// <summary>
        /// A Fuel station.
        /// </summary>
        public static Facilities FUEL_STATION
            => new ("FUEL_STATION");

        /// <summary>
        /// Wifi or other type of internet available.
        /// </summary>
        public static Facilities WIFI
            => new ("WIFI");

        #endregion


        #region Operator overloading

        #region Operator == (Facilities1, Facilities2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Facilities1">A facility.</param>
        /// <param name="Facilities2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Facilities Facilities1,
                                           Facilities Facilities2)

            => Facilities1.Equals(Facilities2);

        #endregion

        #region Operator != (Facilities1, Facilities2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Facilities1">A facility.</param>
        /// <param name="Facilities2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Facilities Facilities1,
                                           Facilities Facilities2)

            => !Facilities1.Equals(Facilities2);

        #endregion

        #region Operator <  (Facilities1, Facilities2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Facilities1">A facility.</param>
        /// <param name="Facilities2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Facilities Facilities1,
                                          Facilities Facilities2)

            => Facilities1.CompareTo(Facilities2) < 0;

        #endregion

        #region Operator <= (Facilities1, Facilities2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Facilities1">A facility.</param>
        /// <param name="Facilities2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Facilities Facilities1,
                                           Facilities Facilities2)

            => Facilities1.CompareTo(Facilities2) <= 0;

        #endregion

        #region Operator >  (Facilities1, Facilities2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Facilities1">A facility.</param>
        /// <param name="Facilities2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Facilities Facilities1,
                                          Facilities Facilities2)

            => Facilities1.CompareTo(Facilities2) > 0;

        #endregion

        #region Operator >= (Facilities1, Facilities2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Facilities1">A facility.</param>
        /// <param name="Facilities2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Facilities Facilities1,
                                           Facilities Facilities2)

            => Facilities1.CompareTo(Facilities2) >= 0;

        #endregion

        #endregion

        #region IComparable<Facilities> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two facilities.
        /// </summary>
        /// <param name="Object">A facility to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Facilities facilityId
                   ? CompareTo(facilityId)
                   : throw new ArgumentException("The given object is not a facility!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Facilities)

        /// <summary>
        /// Compares two facilities.
        /// </summary>
        /// <param name="Facilities">A facility to compare with.</param>
        public Int32 CompareTo(Facilities Facilities)

            => String.Compare(InternalId,
                              Facilities.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Facilities> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two facilities for equality.
        /// </summary>
        /// <param name="Object">A facility to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Facilities facilityId &&
                   Equals(facilityId);

        #endregion

        #region Equals(Facilities)

        /// <summary>
        /// Compares two facilities for equality.
        /// </summary>
        /// <param name="Facilities">A facility to compare with.</param>
        public Boolean Equals(Facilities Facilities)

            => String.Equals(InternalId,
                             Facilities.InternalId,
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
