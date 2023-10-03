/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Opening and access hours.
    /// </summary>
    public class Hours : IEquatable<Hours>
    {

        #region Properties

        /// <summary>
        /// True to represent 24 hours per day and 7 days per week, except the given exceptions.
        /// </summary>
        [Mandatory]
        public Boolean                         IsTwentyFourSevenOpen    { get; }

        /// <summary>
        /// Regular hours, weekday based. Should not be set for representing 24/7 as this is the most common case.
        /// </summary>
        [Optional]
        public IEnumerable<RegularHours>       RegularHours             { get; }

        /// <summary>
        /// Exceptions for specified calendar dates, time-range based.
        /// Periods the station is operating/accessible.
        /// Additional to regular hours. May overlap regular rules.
        /// </summary>
        [Optional]
        public IEnumerable<ExceptionalPeriod>  ExceptionalOpenings      { get; }

        /// <summary>
        /// Exceptions for specified calendar dates, time-range based.
        /// Periods the station is not operating/accessible.
        /// Overwriting regularHours and exceptionalOpenings.
        /// Should not overlap exceptionalOpenings.
        /// </summary>
        [Optional]
        public IEnumerable<ExceptionalPeriod>  ExceptionalClosings      { get; }

        #endregion

        #region Constructor(s)

        #region (private) Hours(IsTwentyFourSevenOpen, RegularHours = null, ExceptionalOpenings = null, ExceptionalClosings = null)

        /// <summary>
        /// Create a new hours class having regular hours, weekday based.
        /// </summary>
        /// <param name="IsTwentyFourSevenOpen">True to represent 24 hours a day and 7 days a week, except the given exceptions.</param>
        /// <param name="RegularHours">Regular hours, weekday based. Should not be set for representing 24/7 as this is the most common case.</param>
        /// <param name="ExceptionalOpenings">Exceptions for specified calendar dates, time-range based. Periods the station is operating/accessible. Additional to regular hours. May overlap regular rules.</param>
        /// <param name="ExceptionalClosings">Exceptions for specified calendar dates, time-range based. Periods the station is not operating/accessible. Overwriting regularHours and exceptionalOpenings. Should not overlap exceptionalOpenings.</param>
        private Hours(Boolean                          IsTwentyFourSevenOpen,
                      IEnumerable<RegularHours>?       RegularHours          = null,
                      IEnumerable<ExceptionalPeriod>?  ExceptionalOpenings   = null,
                      IEnumerable<ExceptionalPeriod>?  ExceptionalClosings   = null)
        {

            this.IsTwentyFourSevenOpen  = IsTwentyFourSevenOpen;
            this.RegularHours           = RegularHours?.       Distinct() ?? Array.Empty<RegularHours>();
            this.ExceptionalOpenings    = ExceptionalOpenings?.Distinct() ?? Array.Empty<ExceptionalPeriod>();
            this.ExceptionalClosings    = ExceptionalClosings?.Distinct() ?? Array.Empty<ExceptionalPeriod>();


            unchecked
            {

                hashCode = IsTwentyFourSevenOpen.GetHashCode()        * 7 ^

                           (RegularHours?.       CalcHashCode() ?? 0) * 5 ^
                           (ExceptionalOpenings?.CalcHashCode() ?? 0) * 3 ^
                           (ExceptionalClosings?.CalcHashCode() ?? 0);

            }

        }

        #endregion

        #region Hours(RegularHours = null, ExceptionalOpenings = null, ExceptionalClosings = null)

        /// <summary>
        /// Create a new hours class having regular hours, weekday based.
        /// Should not be used for representing 24/7 as this is the most common case.
        /// </summary>
        /// <param name="RegularHours">Regular hours, weekday based. Should not be set for representing 24/7 as this is the most common case.</param>
        /// <param name="ExceptionalOpenings">Exceptions for specified calendar dates, time-range based. Periods the station is operating/accessible. Additional to regular hours. May overlap regular rules.</param>
        /// <param name="ExceptionalClosings">Exceptions for specified calendar dates, time-range based. Periods the station is not operating/accessible. Overwriting regularHours and exceptionalOpenings. Should not overlap exceptionalOpenings.</param>
        public Hours(IEnumerable<RegularHours>?       RegularHours          = null,
                     IEnumerable<ExceptionalPeriod>?  ExceptionalOpenings   = null,
                     IEnumerable<ExceptionalPeriod>?  ExceptionalClosings   = null)

            : this(false,
                   RegularHours,
                   ExceptionalOpenings,
                   ExceptionalClosings)

        { }

        #endregion

        #endregion


        #region (static) TwentyFourSevenOpen(ExceptionalOpenings = null, ExceptionalClosings = null)

        /// <summary>
        /// Create a new hours class representing 24 hours per day and 7 days per week, except the given exceptions.
        /// </summary>
        /// <param name="ExceptionalOpenings">Exceptions for specified calendar dates, time-range based. Periods the station is operating/accessible. Additional to regular hours. May overlap regular rules.</param>
        /// <param name="ExceptionalClosings">Exceptions for specified calendar dates, time-range based. Periods the station is not operating/accessible. Overwriting regularHours and exceptionalOpenings. Should not overlap exceptionalOpenings.</param>
        public static Hours TwentyFourSevenOpen(IEnumerable<ExceptionalPeriod>?  ExceptionalOpenings   = null,
                                                IEnumerable<ExceptionalPeriod>?  ExceptionalClosings   = null)

            => new (true,
                    Array.Empty<RegularHours>(),
                    ExceptionalOpenings,
                    ExceptionalClosings);

        #endregion


        #region (static) Parse   (JSON, CustomHoursParser = null)

        /// <summary>
        /// Parse the given JSON representation of a hour.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomHoursParser">A delegate to parse custom hours JSON objects.</param>
        public static Hours? Parse(JObject                              JSON,
                                   CustomJObjectParserDelegate<Hours>?  CustomHoursParser   = null)
        {

            if (TryParse(JSON,
                         out var hours,
                         out var errorResponse,
                         CustomHoursParser))
            {
                return hours;
            }

            throw new ArgumentException("The given JSON representation of a hours object is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Hours, out ErrorResponse, CustomHoursParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a hour.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Hours">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject      JSON,
                                       out Hours?   Hours,
                                       out String?  ErrorResponse)

            => TryParse(JSON,
                        out Hours,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a hour.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Hours">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomHoursParser">A delegate to parse custom hours JSON objects.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       out Hours?                           Hours,
                                       out String?                          ErrorResponse,
                                       CustomJObjectParserDelegate<Hours>?  CustomHoursParser)
        {

            try
            {

                Hours = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse TwentyFourSeven        [mandatory]

                if (!JSON.ParseMandatory("twentyfourseven",
                                         "24/7",
                                         out Boolean TwentyFourSeven,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse RegularHours           [optional]

                if (JSON.ParseOptionalHashSet("regular_hours",
                                              "regular hours",
                                              OCPI.RegularHours.TryParse,
                                              out HashSet<RegularHours> RegularHours,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ExceptionalOpenings    [optional]

                if (JSON.ParseOptionalHashSet("exceptional_openings",
                                              "exceptional openings",
                                              ExceptionalPeriod.TryParse,
                                              out HashSet<ExceptionalPeriod> ExceptionalOpenings,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ExceptionalClosings    [optional]

                if (JSON.ParseOptionalHashSet("exceptional_closings",
                                              "exceptional closings",
                                              ExceptionalPeriod.TryParse,
                                              out HashSet<ExceptionalPeriod> ExceptionalClosings,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Hours = new Hours(TwentyFourSeven,
                                  RegularHours,
                                  ExceptionalOpenings,
                                  ExceptionalClosings);


                if (CustomHoursParser is not null)
                    Hours = CustomHoursParser(JSON,
                                              Hours);

                return true;

            }
            catch (Exception e)
            {
                Hours          = default;
                ErrorResponse  = "The given JSON representation of a hour is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomHoursSerializer = null, CustomRegularHoursSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomRegularHoursSerializer">A delegate to serialize custom regular hours JSON objects.</param>
        /// <param name="CustomExceptionalPeriodSerializer">A delegate to serialize custom exceptional period JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Hours>?              CustomHoursSerializer               = null,
                              CustomJObjectSerializerDelegate<RegularHours>?       CustomRegularHoursSerializer        = null,
                              CustomJObjectSerializerDelegate<ExceptionalPeriod>?  CustomExceptionalPeriodSerializer   = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("twentyfourseven",       IsTwentyFourSevenOpen),

                           RegularHours.       Any()
                               ? new JProperty("regular_hours",         new JArray(RegularHours.       Select(regularHours     => regularHours.    ToJSON(CustomRegularHoursSerializer))))
                               : null,

                           ExceptionalOpenings.Any()
                               ? new JProperty("exceptional_openings",  new JArray(ExceptionalOpenings.Select(eceptionalPeriod => eceptionalPeriod.ToJSON(CustomExceptionalPeriodSerializer))))
                               : null,

                           ExceptionalClosings.Any()
                               ? new JProperty("exceptional_closings",  new JArray(ExceptionalClosings.Select(eceptionalPeriod => eceptionalPeriod.ToJSON(CustomExceptionalPeriodSerializer))))
                               : null

                       );

            return CustomHoursSerializer is not null
                       ? CustomHoursSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Hours Clone()

            => new (IsTwentyFourSevenOpen,
                    RegularHours.       Select(regularHour         => regularHour.        Clone()).ToArray(),
                    ExceptionalOpenings.Select(exceptionalOpening  => exceptionalOpening. Clone()).ToArray(),
                    ExceptionalClosings.Select(exceptionalClosings => exceptionalClosings.Clone()).ToArray());

        #endregion


        #region Operator overloading

        #region Operator == (Hours1, Hours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hours1">A specification of opening and access hours.</param>
        /// <param name="Hours2">Another specification of opening and access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Hours Hours1,
                                           Hours Hours2)
        {

            if (Object.ReferenceEquals(Hours1, Hours2))
                return true;

            if (Hours1 is null || Hours2 is null)
                return false;

            return Hours1.Equals(Hours2);

        }

        #endregion

        #region Operator != (Hours1, Hours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hours1">A specification of opening and access hours.</param>
        /// <param name="Hours2">Another specification of opening and access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Hours Hours1,
                                           Hours Hours2)

            => !(Hours1 == Hours2);

        #endregion

        #endregion

        #region IEquatable<Hours> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two hours objects for equality.
        /// </summary>
        /// <param name="Object">An hours object to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Hours hours &&
                   Equals(hours);

        #endregion

        #region Equals(Hours)

        /// <summary>
        /// Compares two hours objects for equality.
        /// </summary>
        /// <param name="Hours">An hours object to compare with.</param>
        public Boolean Equals(Hours? Hours)

            => Hours is not null &&

               IsTwentyFourSevenOpen.Equals(Hours.IsTwentyFourSevenOpen)                               &&

               RegularHours.       Count().Equals(Hours.RegularHours.       Count())                   &&
               ExceptionalOpenings.Count().Equals(Hours.ExceptionalOpenings.Count())                   &&
               ExceptionalClosings.Count().Equals(Hours.ExceptionalClosings.Count())                   &&

               RegularHours.       All(regularHour => Hours.RegularHours.       Contains(regularHour)) &&
               ExceptionalOpenings.All(regularHour => Hours.ExceptionalOpenings.Contains(regularHour)) &&
               ExceptionalClosings.All(regularHour => Hours.ExceptionalClosings.Contains(regularHour));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   IsTwentyFourSevenOpen
                       ? "24/7, "
                       : "..., ",

                   RegularHours.       Count(), " regular hour(s), ",
                   ExceptionalOpenings.Count(), " exceptional opening(s), ",
                   ExceptionalClosings.Count(), " exceptional closing(s)"

               );

        #endregion


    }

}
