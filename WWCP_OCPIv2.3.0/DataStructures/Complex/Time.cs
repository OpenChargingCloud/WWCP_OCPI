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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

/// <summary>
    /// A structure to store a simple time.
    /// </summary>
    public readonly struct Time : IEquatable<Time>,
                                  IComparable<Time>,
                                  IComparable
    {

        #region Properties

        /// <summary>
        /// The hour.
        /// </summary>
        public Byte   Hour      { get; }

        /// <summary>
        /// The minute.
        /// </summary>
        public Byte   Minute    { get; }

        /// <summary>
        /// The second.
        /// </summary>
        public Byte?  Second    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a simple time.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute.</param>
        /// <param name="Second">The second.</param>
        private Time(Byte   Hour,
                     Byte   Minute  = 0,
                     Byte?  Second  = null)
        {

            if (Hour   > 23)
                throw new ArgumentException("The hour value is invalid!",    nameof(Hour));

            if (Minute > 59)
                throw new ArgumentException("The minute value is invalid!",  nameof(Minute));

            if (Second > 59)
                throw new ArgumentException("The second value is invalid!",  nameof(Second));


            this.Hour    = Hour;
            this.Minute  = Minute;
            this.Second  = Second;

        }

        #endregion


        #region FromHour      (Hour)

        /// <summary>
        /// Create a new time based on the given hour.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        public static Time FromHour(Byte Hour)

            => new Time(Hour);

        #endregion

        #region FromHourMin   (Hour, Minute)

        /// <summary>
        /// Create a new time based on the given hour and minute.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute</param>
        public static Time FromHourMin(Byte Hour, Byte Minute)

            => new Time(Hour,
                        Minute);

        #endregion

        #region FromHourMinSec(Hour, Minute, Second)

        /// <summary>
        /// Create a new time based on the given hour and minute.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute</param>
        /// <param name="Second">The second.</param>
        public static Time FromHourMinSec(Byte Hour, Byte Minute, Byte Second)

            => new Time(Hour,
                        Minute,
                        Second);

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as time.
        /// </summary>
        /// <param name="Text">A text representation of the time.</param>
        public static Time Parse(String Text)
        {

            if (TryParse(Text, out var time))
                return time;

            throw new ArgumentException("The given JSON representation of tariff restrictions is invalid: " + Text,
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as time.
        /// </summary>
        /// <param name="Text">A text representation of the time.</param>
        public static Time? TryParse(String Text)
        {

            if (TryParse(Text, out var time))
                return time;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Time)

        /// <summary>
        /// Try to parse the given text as time.
        /// </summary>
        /// <param name="Text">A text representation of the time.</param>
        /// <param name="Time">The parsed time.</param>
        public static Boolean TryParse(String Text, out Time Time)
        {

            Time = default;

            var Fragments = Text?.Trim()?.Split(':');

            if (Fragments == null)
                return false;

            else if (Fragments.Length == 1)
            {

                if (!Byte.TryParse(Fragments[0], out var Hour))
                    return false;

                Time = Time.FromHour(Hour);
                return true;

            }

            else if (Fragments.Length == 2)
            {

                if (!Byte.TryParse(Fragments[0], out var Hour))
                    return false;

                if (!Byte.TryParse(Fragments[1], out var Minute))
                    return false;

                Time = Time.FromHourMin(Hour, Minute);
                return true;

            }

            else if (Fragments.Length == 3)
            {

                if (!Byte.TryParse(Fragments[0], out var Hour))
                    return false;

                if (!Byte.TryParse(Fragments[1], out var Minute))
                    return false;

                if (!Byte.TryParse(Fragments[2], out var Second))
                    return false;

                Time = Time.FromHourMinSec(Hour, Minute, Second);
                return true;

            }

            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Time Clone()

            => new (Hour,
                    Minute,
                    Second);

        #endregion


        #region Operator overloading

        #region Operator == (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Time Time1,
                                           Time Time2)

            => Time1.Equals(Time2);

        #endregion

        #region Operator != (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Time Time1,
                                           Time Time2)

            => !Time1.Equals(Time2);

        #endregion

        #region Operator <  (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Time Time1,
                                          Time Time2)

            => Time1.CompareTo(Time2) < 0;

        #endregion

        #region Operator <= (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Time Time1,
                                           Time Time2)

            => Time1.CompareTo(Time2) <= 0;

        #endregion

        #region Operator >  (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Time Time1,
                                          Time Time2)

            => Time1.CompareTo(Time2) > 0;

        #endregion

        #region Operator >= (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Time Time1,
                                           Time Time2)

            => Time1.CompareTo(Time2) >= 0;

        #endregion

        #region Operator +  (Time1, Time2)

        /// <summary>
        /// Operator +
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        public static TimeSpan operator +  (Time Time1,
                                            Time Time2)
        {

            var days     = 0;

            var hours    = Time1.Hour   + Time2.Hour;

            var minutes  = Time1.Minute + Time2.Minute;

            var seconds  = Time1.Second.HasValue || Time2.Second.HasValue
                               ? new Int32?((Time1.Second ?? 0) + (Time2.Second ?? 0))
                               : null;


            if (seconds > 59)
            {
                seconds -= 59;
                minutes++;
            }

            if (minutes > 59)
            {
                minutes -= 59;
                hours++;
            }

            if (hours > 23)
            {
                hours -= 23;
                days++;
            }

            return new TimeSpan(hours, minutes, seconds ?? 0).
                       Add(TimeSpan.FromDays(days));

        }

        #endregion

        #region Operator -  (Time1, Time2)

        /// <summary>
        /// Operator -
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        public static TimeSpan operator -  (Time Time1,
                                            Time Time2)
        {

            var hours    = Time1.Hour   - Time2.Hour;

            var minutes  = Time1.Minute - Time2.Minute;

            var seconds  = Time1.Second.HasValue || Time2.Second.HasValue
                               ? new Int32?((Time1.Second ?? 0) - (Time2.Second ?? 0))
                               : null;

            return new TimeSpan(

                       hours   >= 0 ? hours   : 0,
                       minutes >= 0 ? minutes : 0,

                       seconds.HasValue && seconds.Value >= 0
                           ? seconds.Value
                           : 0

                   );

        }

        #endregion

        #endregion

        #region IComparable<Time> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two times.
        /// </summary>
        /// <param name="Object">A time to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Time time
                   ? CompareTo(time)
                   : throw new ArgumentException("The given object is not a time!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Time)

        /// <summary>
        /// Compares two times.
        /// </summary>
        /// <param name="Time">A time to compare with.</param>
        public Int32 CompareTo(Time Time)
        {

            var c = Hour.        CompareTo(Time.Hour);

            if (c == 0)
                c = Minute.      CompareTo(Time.Minute);

            if (c == 0 && Second.HasValue && Time.Second.HasValue)
                c = Second.Value.CompareTo(Time.Second.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Time> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two times for equality.
        /// </summary>
        /// <param name="Object">A time to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Time time &&
                   Equals(time);

        #endregion

        #region Equals(Time)

        /// <summary>
        /// Compares two times for equality.
        /// </summary>
        /// <param name="Time">A time to compare with.</param>
        public Boolean Equals(Time Time)

            => Hour.  Equals(Time.Hour)   &&
               Minute.Equals(Time.Minute) &&
               Second.Equals(Time.Second);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Hour.   GetHashCode() * 5 ^
                       Minute. GetHashCode() * 3 ^
                       Second?.GetHashCode() ?? 0;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Hour.  ToString("D2"), ":",
                   Minute.ToString("D2"),

                   Second.HasValue
                       ? ":" + Second.Value.ToString("D2")
                       : ""

               );

        #endregion

    }

}
