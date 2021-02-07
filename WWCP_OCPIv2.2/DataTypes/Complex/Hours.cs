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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
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

        /// <summary>
        /// Create a new hours class having regular hours, weekday based.
        /// </summary>
        /// <param name="IsTwentyFourSevenOpen">True to represent 24 hours a day and 7 days a week, except the given exceptions.</param>
        /// <param name="RegularHours">Regular hours, weekday based. Should not be set for representing 24/7 as this is the most common case.</param>
        /// <param name="ExceptionalOpenings">Exceptions for specified calendar dates, time-range based. Periods the station is operating/accessible. Additional to regular hours. May overlap regular rules.</param>
        /// <param name="ExceptionalClosings">Exceptions for specified calendar dates, time-range based. Periods the station is not operating/accessible. Overwriting regularHours and exceptionalOpenings. Should not overlap exceptionalOpenings.</param>
        private Hours(Boolean                         IsTwentyFourSevenOpen,
                      IEnumerable<RegularHours>       RegularHours          = null,
                      IEnumerable<ExceptionalPeriod>  ExceptionalOpenings   = null,
                      IEnumerable<ExceptionalPeriod>  ExceptionalClosings   = null)
        {

            this.IsTwentyFourSevenOpen  = IsTwentyFourSevenOpen;
            this.RegularHours           = RegularHours?.       Distinct() ?? new RegularHours[0];
            this.ExceptionalOpenings    = ExceptionalOpenings?.Distinct() ?? new ExceptionalPeriod[0];
            this.ExceptionalClosings    = ExceptionalClosings?.Distinct() ?? new ExceptionalPeriod[0];

        }

        /// <summary>
        /// Create a new hours class having regular hours, weekday based.
        /// Should not be used for representing 24/7 as this is the most common case.
        /// </summary>
        /// <param name="RegularHours">Regular hours, weekday based. Should not be set for representing 24/7 as this is the most common case.</param>
        /// <param name="ExceptionalOpenings">Exceptions for specified calendar dates, time-range based. Periods the station is operating/accessible. Additional to regular hours. May overlap regular rules.</param>
        /// <param name="ExceptionalClosings">Exceptions for specified calendar dates, time-range based. Periods the station is not operating/accessible. Overwriting regularHours and exceptionalOpenings. Should not overlap exceptionalOpenings.</param>
        public Hours(IEnumerable<RegularHours>       RegularHours          = null,
                     IEnumerable<ExceptionalPeriod>  ExceptionalOpenings   = null,
                     IEnumerable<ExceptionalPeriod>  ExceptionalClosings   = null)

            : this(false,
                   RegularHours,
                   ExceptionalOpenings,
                   ExceptionalClosings)

        { }

        /// <summary>
        /// Create a new hours class representing 24 hours per day and 7 days per week, except the given exceptions.
        /// </summary>
        /// <param name="ExceptionalOpenings">Exceptions for specified calendar dates, time-range based. Periods the station is operating/accessible. Additional to regular hours. May overlap regular rules.</param>
        /// <param name="ExceptionalClosings">Exceptions for specified calendar dates, time-range based. Periods the station is not operating/accessible. Overwriting regularHours and exceptionalOpenings. Should not overlap exceptionalOpenings.</param>
        public Hours(IEnumerable<ExceptionalPeriod>  ExceptionalOpenings   = null,
                     IEnumerable<ExceptionalPeriod>  ExceptionalClosings   = null)

            : this(true,
                   new RegularHours[0],
                   ExceptionalOpenings,
                   ExceptionalClosings)

        { }

        #endregion


        #region (static) TwentyFourSevenOpen(ExceptionalOpenings = null, ExceptionalClosings = null)

        /// <summary>
        /// Create a new hours class representing 24 hours per day and 7 days per week, except the given exceptions.
        /// </summary>
        /// <param name="ExceptionalOpenings">Exceptions for specified calendar dates, time-range based. Periods the station is operating/accessible. Additional to regular hours. May overlap regular rules.</param>
        /// <param name="ExceptionalClosings">Exceptions for specified calendar dates, time-range based. Periods the station is not operating/accessible. Overwriting regularHours and exceptionalOpenings. Should not overlap exceptionalOpenings.</param>
        public static Hours TwentyFourSevenOpen(IEnumerable<ExceptionalPeriod>  ExceptionalOpenings   = null,
                                                IEnumerable<ExceptionalPeriod>  ExceptionalClosings   = null)

            => new Hours(ExceptionalOpenings,
                         ExceptionalClosings);

        #endregion


        #region (static) Parse   (JSON, CustomHoursParser = null)

        /// <summary>
        /// Parse the given JSON representation of a hour.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomHoursParser">A delegate to parse custom hours JSON objects.</param>
        public static Hours Parse(JObject                                        JSON,
                                             CustomJObjectParserDelegate<Hours>  CustomHoursParser   = null)
        {

            if (TryParse(JSON,
                         out Hours hours,
                         out String           ErrorResponse,
                         CustomHoursParser))
            {
                return hours;
            }

            throw new ArgumentException("The given JSON representation of a hour is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomHoursParser = null)

        /// <summary>
        /// Parse the given text representation of a hour.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomHoursParser">A delegate to parse custom hours JSON objects.</param>
        public static Hours Parse(String                                         Text,
                                             CustomJObjectParserDelegate<Hours>  CustomHoursParser   = null)
        {

            if (TryParse(Text,
                         out Hours hours,
                         out String         ErrorResponse,
                         CustomHoursParser))
            {
                return hours;
            }

            throw new ArgumentException("The given text representation of a hour is invalid: " + ErrorResponse, nameof(Text));

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
        public static Boolean TryParse(JObject     JSON,
                                       out Hours   Hours,
                                       out String  ErrorResponse)

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
        public static Boolean TryParse(JObject                             JSON,
                                       out Hours                           Hours,
                                       out String                          ErrorResponse,
                                       CustomJObjectParserDelegate<Hours>  CustomHoursParser)
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

                if (JSON.ParseOptionalJSON("regular_hours",
                                           "regular hours",
                                           OCPIv2_2.RegularHours.TryParse,
                                           out IEnumerable<RegularHours> RegularHours,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse ExceptionalOpenings    [optional]

                if (JSON.ParseOptionalJSON("exceptional_openings",
                                           "exceptional openings",
                                           ExceptionalPeriod.TryParse,
                                           out IEnumerable<ExceptionalPeriod> ExceptionalOpenings,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse ExceptionalClosings    [optional]

                if (JSON.ParseOptionalJSON("exceptional_closings",
                                           "exceptional closings",
                                           ExceptionalPeriod.TryParse,
                                           out IEnumerable<ExceptionalPeriod> ExceptionalClosings,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                Hours = new Hours(TwentyFourSeven,
                                  RegularHours,
                                  ExceptionalOpenings,
                                  ExceptionalClosings);


                if (CustomHoursParser != null)
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

        #region (static) TryParse(Text, out Hours, out ErrorResponse, CustomHoursParser = null)

        /// <summary>
        /// Try to parse the given text representation of a hour.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="Hours">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomHoursParser">A delegate to parse custom hours JSON objects.</param>
        public static Boolean TryParse(String                              Text,
                                       out Hours                           Hours,
                                       out String                          ErrorResponse,
                                       CustomJObjectParserDelegate<Hours>  CustomHoursParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out Hours,
                                out ErrorResponse,
                                CustomHoursParser);

            }
            catch (Exception e)
            {
                Hours = default;
                ErrorResponse  = "The given text representation of a hour is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomHoursSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Hours> CustomHoursSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("twentyfourseven",             IsTwentyFourSevenOpen),

                           RegularHours.SafeAny()
                               ? new JProperty("regular_hours",         new JArray(RegularHours.       Select(hours   => hours.  ToJSON())))
                               : null,

                           ExceptionalOpenings.SafeAny()
                               ? new JProperty("exceptional_openings",  new JArray(ExceptionalOpenings.Select(opening => opening.ToJSON())))
                               : null,

                           ExceptionalClosings.SafeAny()
                               ? new JProperty("exceptional_closings",  new JArray(ExceptionalClosings.Select(closing => closing.ToJSON())))
                               : null

                       );

            return CustomHoursSerializer != null
                       ? CustomHoursSerializer(this, JSON)
                       : JSON;

        }

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
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Hours Hours &&
                   Equals(Hours);

        #endregion

        #region Equals(Hours)

        /// <summary>
        /// Compares two Hourss for equality.
        /// </summary>
        /// <param name="Hours">A Hours to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Hours Hours)

            => !(Hours is null) &&

               IsTwentyFourSevenOpen.Equals(Hours.IsTwentyFourSevenOpen)                               &&
               RegularHours.         Equals(Hours.RegularHours.       Count())                         &&
               ExceptionalOpenings.  Equals(Hours.ExceptionalOpenings.Count())                         &&
               ExceptionalClosings.  Equals(Hours.ExceptionalClosings.Count())                         &&

               RegularHours.       All(regularHour => Hours.RegularHours.Contains(regularHour))        &&
               ExceptionalOpenings.All(regularHour => Hours.ExceptionalOpenings.Contains(regularHour)) &&
               ExceptionalClosings.All(regularHour => Hours.ExceptionalClosings.Contains(regularHour));

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

                return IsTwentyFourSevenOpen.GetHashCode() * 3 ^
                       RegularHours.Aggregate(0, (hashCode, dimension) => hashCode ^ dimension.GetHashCode()) ^
                       ExceptionalOpenings.Aggregate(0, (hashCode, period)    => hashCode ^ period.   GetHashCode()) ^
                       ExceptionalClosings.Aggregate(0, (hashCode, period)    => hashCode ^ period.   GetHashCode());

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(IsTwentyFourSevenOpen ? "24/7" : "...");

        #endregion

    }

}
