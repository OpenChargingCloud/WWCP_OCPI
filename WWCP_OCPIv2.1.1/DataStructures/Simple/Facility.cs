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

using System.Collections.Concurrent;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Extension methods for facilities.
    /// </summary>
    public static class FacilityExtensions
    {

        /// <summary>
        /// Indicates whether this facility is null or empty.
        /// </summary>
        /// <param name="Facility">A facility.</param>
        public static Boolean IsNullOrEmpty(this Facility? Facility)
            => !Facility.HasValue || Facility.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this facility is NOT null or empty.
        /// </summary>
        /// <param name="Facility">A facility.</param>
        public static Boolean IsNotNullOrEmpty(this Facility? Facility)
            => Facility.HasValue && Facility.Value.IsNotNullOrEmpty;


        #region Matches(Facilities, Text)

        /// <summary>
        /// Checks whether the given enumeration of facilities matches the given text.
        /// </summary>
        /// <param name="Facilities">An enumeration of facilities.</param>
        /// <param name="Text">A text to match.</param>
        public static Boolean Matches(this IEnumerable<Facility>  Facilities,
                                      String                      Text)

            => Facilities.Any(facilitiy => facilitiy.Value.Contains(Text, StringComparison.OrdinalIgnoreCase));

        #endregion


    }


    /// <summary>
    /// A facility comparer.
    /// </summary>
    public sealed class FacilityComparer : IComparer<Facility>
    {

        /// <summary>
        /// The default facility comparer.
        /// </summary>
        public static readonly FacilityComparer OrdinalIgnoreCase = new();

        /// <summary>
        /// Compares two facilities.
        /// </summary>
        /// <param name="Facility1">A facility to compare with.</param>
        /// <param name="Facility2">A facility to compare with.</param>
        public Int32 Compare(Facility Facility1,
                             Facility Facility2)

            => StringComparer.OrdinalIgnoreCase.Compare(
                   Facility1.Value,
                   Facility2.Value
               );

    }


    /// <summary>
    /// The unique identification of a facility.
    /// </summary>
    public readonly struct Facility : IId<Facility>
    {

        #region Static Lookup

        private readonly static ConcurrentDictionary<String, Facility> lookup = new (StringComparer.OrdinalIgnoreCase);

        private static Facility Register(String Text)

            => lookup.GetOrAdd(
                   Text,
                   static text => new Facility(text)
               );

        /// <summary>
        /// All registered facilities.
        /// </summary>
        public static IEnumerable<Facility> All
            => lookup.Values;

        #endregion

        #region Properties

        /// <summary>
        /// The text representation of the facility.
        /// </summary>
        public String   Value    { get; }

        /// <summary>
        /// Indicates whether this facility is null or empty.
        /// </summary>
        public Boolean  IsNullOrEmpty
            => Value.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this facility is NOT null or empty.
        /// </summary>
        public Boolean  IsNotNullOrEmpty
            => Value.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the facility.
        /// </summary>
        public UInt64   Length
            => (UInt64) (Value?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new facility based on the given text representation.
        /// </summary>
        /// <param name="Text">The text representation of a facility.</param>
        private Facility(String Text)
        {
            this.Value = Text;
        }

        #endregion


        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a facility.
        /// </summary>
        /// <param name="Text">A text representation of a facility.</param>
        public static Facility Parse(String Text)
        {

            if (TryParse(Text, out var facility))
                return facility;

            throw new ArgumentException($"Invalid text representation of a facility: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a facility.
        /// </summary>
        /// <param name="Text">A text representation of a facility.</param>
        public static Facility? TryParse(String Text)
        {

            if (TryParse(Text, out var facility))
                return facility;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out Facility)

        /// <summary>
        /// Try to parse the given text as a facility.
        /// </summary>
        /// <param name="Text">A text representation of a facility.</param>
        /// <param name="Facility">The parsed facility.</param>
        public static Boolean TryParse(String Text, out Facility Facility)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                Facility = Register(Text);
                return true;
            }

            Facility = default;
            return false;

        }

        #endregion


        #region Static definitions

        /// <summary>
        /// A hotel.
        /// </summary>
        public static Facility  HOTEL              { get; }
            = Register("HOTEL");

        /// <summary>
        /// A restaurant.
        /// </summary>
        public static Facility  RESTAURANT         { get; }
            = Register("RESTAURANT");

        /// <summary>
        /// A cafe.
        /// </summary>
        public static Facility  CAFE               { get; }
            = Register("CAFE");

        /// <summary>
        /// A mall or shopping center.
        /// </summary>
        public static Facility  MALL               { get; }
            = Register("MALL");

        /// <summary>
        /// A supermarket.
        /// </summary>
        public static Facility  SUPERMARKET        { get; }
            = Register("SUPERMARKET");

        /// <summary>
        /// Sport facilities: gym, field etc.
        /// </summary>
        public static Facility  SPORT              { get; }
            = Register("SPORT");

        /// <summary>
        /// A recreation area.
        /// </summary>
        public static Facility  RECREATION_AREA    { get; }
            = Register("RECREATION_AREA");

        /// <summary>
        /// Located in, or close to, a park, nature reserve etc.
        /// </summary>
        public static Facility  NATURE             { get; }
            = Register("NATURE");

        /// <summary>
        /// A museum.
        /// </summary>
        public static Facility  MUSEUM             { get; }
            = Register("MUSEUM");

        /// <summary>
        /// A bike/e-bike/e-scooter sharing location.
        /// </summary>
        public static Facility  BIKE_SHARING       { get; }
            = Register("BIKE_SHARING");

        /// <summary>
        /// A bus stop.
        /// </summary>
        public static Facility  BUS_STOP           { get; }
            = Register("BUS_STOP");

        /// <summary>
        /// A taxi stand.
        /// </summary>
        public static Facility  TAXI_STAND         { get; }
            = Register("TAXI_STAND");

        /// <summary>
        /// A tram stop/station.
        /// </summary>
        public static Facility  TRAM_STOP          { get; }
            = Register("TRAM_STOP");

        /// <summary>
        /// A metro station.
        /// </summary>
        public static Facility  METRO_STATION      { get; }
            = Register("METRO_STATION");

        /// <summary>
        /// A train station.
        /// </summary>
        public static Facility  TRAIN_STATION      { get; }
            = Register("TRAIN_STATION");

        /// <summary>
        /// An airport.
        /// </summary>
        public static Facility  AIRPORT            { get; }
            = Register("AIRPORT");

        /// <summary>
        /// A parking lot.
        /// </summary>
        public static Facility  PARKING_LOT        { get; }
            = Register("PARKING_LOT");

        /// <summary>
        /// A carpool parking.
        /// </summary>
        public static Facility  CARPOOL_PARKING    { get; }
            = Register("CARPOOL_PARKING");

        /// <summary>
        /// A Fuel station.
        /// </summary>
        public static Facility  FUEL_STATION       { get; }
            = Register("FUEL_STATION");

        /// <summary>
        /// Wifi or other type of internet available.
        /// </summary>
        public static Facility  WIFI               { get; }
            = Register("WIFI");

        #endregion


        #region Operator overloading

        #region Operator == (Facility1, Facility2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Facility1">A facility.</param>
        /// <param name="Facility2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Facility Facility1,
                                           Facility Facility2)

            => Facility1.Equals(Facility2);

        #endregion

        #region Operator != (Facility1, Facility2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Facility1">A facility.</param>
        /// <param name="Facility2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Facility Facility1,
                                           Facility Facility2)

            => !Facility1.Equals(Facility2);

        #endregion

        #region Operator <  (Facility1, Facility2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Facility1">A facility.</param>
        /// <param name="Facility2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Facility Facility1,
                                          Facility Facility2)

            => Facility1.CompareTo(Facility2) < 0;

        #endregion

        #region Operator <= (Facility1, Facility2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Facility1">A facility.</param>
        /// <param name="Facility2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Facility Facility1,
                                           Facility Facility2)

            => Facility1.CompareTo(Facility2) <= 0;

        #endregion

        #region Operator >  (Facility1, Facility2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Facility1">A facility.</param>
        /// <param name="Facility2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Facility Facility1,
                                          Facility Facility2)

            => Facility1.CompareTo(Facility2) > 0;

        #endregion

        #region Operator >= (Facility1, Facility2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Facility1">A facility.</param>
        /// <param name="Facility2">Another facility.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Facility Facility1,
                                           Facility Facility2)

            => Facility1.CompareTo(Facility2) >= 0;

        #endregion

        #endregion

        #region IComparable<Facility> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two facilities.
        /// </summary>
        /// <param name="Object">A facility to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Facility facility
                   ? CompareTo(facility)
                   : throw new ArgumentException("The given object is not a facility!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Facility)

        /// <summary>
        /// Compares two facilities.
        /// </summary>
        /// <param name="Facility">A facility to compare with.</param>
        public Int32 CompareTo(Facility Facility)

            => String.Compare(Value,
                              Facility.Value,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Facility> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two facilities for equality.
        /// </summary>
        /// <param name="Object">A facility to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Facility facility &&
                   Equals(facility);

        #endregion

        #region Equals(Facility)

        /// <summary>
        /// Compares two facilities for equality.
        /// </summary>
        /// <param name="Facility">A facility to compare with.</param>
        public Boolean Equals(Facility Facility)

            => String.Equals(Value,
                             Facility.Value,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Value is not null
                   ? StringComparer.OrdinalIgnoreCase.GetHashCode(Value)
                   : 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Value ?? "";

        #endregion

    }

}
