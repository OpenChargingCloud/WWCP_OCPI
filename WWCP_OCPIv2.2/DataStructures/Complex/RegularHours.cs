/*
 * Copyright (c) 2015-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The regular hours specify regular and recurring access or operations.
    /// </summary>
    public readonly struct RegularHours : IEquatable<RegularHours>,
                                          IComparable<RegularHours>,
                                          IComparable
    {

        #region Properties

        /// <summary>
        /// Day of the week, from Monday till Sunday.
        /// </summary>
        [Mandatory]
        public DayOfWeek  Weekday    { get; }

        /// <summary>
        /// Begin of the regular period given in regular hours and minutes. Must be in 24h format.
        /// </summary>
        [Mandatory]
        public HourMin    Begin      { get; }

        /// <summary>
        /// End of the regular period, syntax as for period_begin. Must be later than the begin.
        /// </summary>
        [Mandatory]
        public HourMin    End        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new regular hours specification.
        /// </summary>
        /// <param name="Weekday">Day of the week, from Monday till Sunday.</param>
        /// <param name="Begin">Begin of the regular period given in regular hours and minutes. Must be in 24h format.</param>
        /// <param name="End">End of the regular period, syntax as for period_begin. Must be later than the begin.</param>
        public RegularHours(DayOfWeek  Weekday,
                            HourMin    Begin,
                            HourMin    End)
        {

            if (Begin >= End)
                throw new ArgumentException("Begin time must be before the end time!");

            this.Weekday  = Weekday;
            this.Begin    = Begin;
            this.End      = End;

        }

        #endregion


        #region (static) Parse   (JSON, CustomRegularHoursParser = null)

        /// <summary>
        /// Parse the given JSON representation of a regular hours specification.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomRegularHoursParser">A delegate to parse custom regular hours specification JSON objects.</param>
        public static RegularHours Parse(JObject                                     JSON,
                                         CustomJObjectParserDelegate<RegularHours>?  CustomRegularHoursParser   = null)
        {

            if (TryParse(JSON,
                         out var regularHours,
                         out var errorResponse,
                         CustomRegularHoursParser))
            {
                return regularHours;
            }

            throw new ArgumentException("The given JSON representation of a regular hours specification is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out RegularHours, out ErrorResponse, CustomRegularHoursParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a regular hours specification.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="RegularHours">The parsed regular hours specification.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject           JSON,
                                       out RegularHours  RegularHours,
                                       out String?       ErrorResponse)

            => TryParse(JSON,
                        out RegularHours,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a regular hour.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="RegularHours">The parsed regular hours specification.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRegularHoursParser">A delegate to parse custom regular hours specification JSON objects.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       out RegularHours                            RegularHours,
                                       out String                                  ErrorResponse,
                                       CustomJObjectParserDelegate<RegularHours>?  CustomRegularHoursParser)
        {

            try
            {

                RegularHours = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse WeekDay        [mandatory]

                if (!JSON.ParseMandatory("weekday",
                                         "weekday",
                                         out Int32 WeekDayInt32,
                                         out ErrorResponse))
                {
                    return false;
                }

                // https://github.com/ocpi/ocpi/blob/master/mod_locations.asciidoc#mod_locations_regularhours_class
                // "Number of day in the week, from Monday (1) till Sunday (7)" <= OCPI is crazy!

                var WeekDay = (DayOfWeek) (WeekDayInt32 % 7);

                #endregion

                #region Parse PeriodBegin    [mandatory]

                if (!JSON.ParseMandatory("period_begin",
                                         "period begin",
                                         HourMin.TryParse,
                                         out HourMin PeriodBegin,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PeriodEnd      [mandatory]

                if (!JSON.ParseMandatory("period_end",
                                         "period end",
                                         HourMin.TryParse,
                                         out HourMin PeriodEnd,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                RegularHours = new RegularHours(WeekDay,
                                                PeriodBegin,
                                                PeriodEnd);


                if (CustomRegularHoursParser is not null)
                    RegularHours = CustomRegularHoursParser(JSON,
                                                            RegularHours);

                return true;

            }
            catch (Exception e)
            {
                RegularHours   = default;
                ErrorResponse  = "The given JSON representation of a regular hours specification is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomRegularHoursSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRegularHoursSerializer">A delegate to serialize custom regular hours specifications JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RegularHours>? CustomRegularHoursSerializer = null)
        {

            var JSON = JSONObject.Create(

                           // https://github.com/ocpi/ocpi/blob/master/mod_locations.asciidoc#mod_locations_regularhours_class
                           // "Number of day in the week, from Monday (1) till Sunday (7)" <= OCPI is crazy!
                           new JProperty("weekday",       Weekday == DayOfWeek.Sunday ? 7 : (UInt32) Weekday),

                           new JProperty("period_begin",  Begin.ToString()),
                           new JProperty("period_end",    End.  ToString())

                       );

            return CustomRegularHoursSerializer is not null
                       ? CustomRegularHoursSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (RegularHours1, RegularHours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegularHours1">A regular hours specification.</param>
        /// <param name="RegularHours2">Another regular hours specification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RegularHours RegularHours1,
                                           RegularHours RegularHours2)

            => RegularHours1.Equals(RegularHours2);

        #endregion

        #region Operator != (RegularHours1, RegularHours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegularHours1">A regular hours specification.</param>
        /// <param name="RegularHours2">Another regular hours specification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RegularHours RegularHours1,
                                           RegularHours RegularHours2)

            => !RegularHours1.Equals(RegularHours2);

        #endregion

        #region Operator <  (RegularHours1, RegularHours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegularHours1">A regular hours specification.</param>
        /// <param name="RegularHours2">Another regular hours specification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RegularHours RegularHours1,
                                          RegularHours RegularHours2)

            => RegularHours1.CompareTo(RegularHours2) < 0;

        #endregion

        #region Operator <= (RegularHours1, RegularHours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegularHours1">A regular hours specification.</param>
        /// <param name="RegularHours2">Another regular hours specification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RegularHours RegularHours1,
                                           RegularHours RegularHours2)

            => RegularHours1.CompareTo(RegularHours2) <= 0;

        #endregion

        #region Operator >  (RegularHours1, RegularHours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegularHours1">A regular hours specification.</param>
        /// <param name="RegularHours2">Another regular hours specification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RegularHours RegularHours1,
                                          RegularHours RegularHours2)

            => RegularHours1.CompareTo(RegularHours2) > 0;

        #endregion

        #region Operator >= (RegularHours1, RegularHours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegularHours1">A regular hours specification.</param>
        /// <param name="RegularHours2">Another regular hours specification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RegularHours RegularHours1,
                                           RegularHours RegularHours2)

            => RegularHours1.CompareTo(RegularHours2) >= 0;

        #endregion

        #endregion

        #region IComparable<RegularHours> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two regular hours specifications.
        /// </summary>
        /// <param name="Object">A regular hours specification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RegularHours regularHours
                   ? CompareTo(regularHours)
                   : throw new ArgumentException("The given object is not a regular hours specification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RegularHours)

        /// <summary>
        /// Compares two regular hours specifications.
        /// </summary>
        /// <param name="RegularHours">A regular hours specification to compare with.</param>
        public Int32 CompareTo(RegularHours RegularHours)
        {

            var c = Weekday.CompareTo(RegularHours.Weekday);

            if (c == 0)
                c = Begin.  CompareTo(RegularHours.Begin);

            if (c == 0)
                c = End.    CompareTo(RegularHours.End);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<RegularHours> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two regular hours specifications for equality.
        /// </summary>
        /// <param name="Object">A regular hours specification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RegularHours RegularHours &&
                   Equals(RegularHours);

        #endregion

        #region Equals(RegularHours)

        /// <summary>
        /// Compares two regular hours specifications for equality.
        /// </summary>
        /// <param name="RegularHours">A regular hours specification to compare with.</param>
        public Boolean Equals(RegularHours RegularHours)

            => Weekday.Equals(RegularHours.Weekday) &&
               Begin.  Equals(RegularHours.Begin)   &&
               End.    Equals(RegularHours.End);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Weekday.GetHashCode() * 5 ^
                       Begin.  GetHashCode() * 3 ^
                       End.    GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Weekday.ToString(),
                   "s from ",
                   Begin.  ToString(),
                   " to ",
                   End.    ToString()

               );

        #endregion

    }

}
