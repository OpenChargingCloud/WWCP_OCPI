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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Calendar
    /// </summary>
    public class Calendar : IEquatable<Calendar>,
                            IComparable<Calendar>,
                            IComparable
    {

        #region Properties

        /// <summary>
        /// The identification of the calendar.
        /// </summary>
        [Mandatory]
        public Calendar_Id            Id                    { get; }

        /// <summary>
        /// The start time of a calendar.
        /// </summary>
        [Mandatory]
        public DateTimeOffset         BeginFrom             { get; }

        /// <summary>
        /// The end time of a calendar.
        /// </summary>
        [Mandatory]
        public DateTimeOffset         EndBefore             { get; }

        /// <summary>
        /// The minimum allowed booking increment within available timeslot.
        /// </summary>
        [Mandatory]
        public UInt32?                StepSize              { get; }

        /// <summary>
        /// The enumeration of available time slots.
        /// </summary>
        [Mandatory]
        public IEnumerable<TimeSlot>  AvailableTimeSlots    { get; }

        /// <summary>
        /// The timestamp for the last calendar change has been made.
        /// </summary>
        [Mandatory]
        public DateTime               LastUpdated           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new calendar.
        /// </summary>
        /// <param name="Id">The identification of the calendar.</param>
        /// <param name="BeginFrom">The start time of a calendar.</param>
        /// <param name="EndBefore">The end time of a calendar.</param>
        /// <param name="AvailableTimeSlots">The enumeration of available time slots.</param>
        /// <param name="LastUpdated">The timestamp for the last calendar change has been made.</param>
        /// <param name="StepSize">The minimum allowed booking increment within available timeslot.</param>
        public Calendar(Calendar_Id            Id,
                        DateTimeOffset         BeginFrom,
                        DateTimeOffset         EndBefore,
                        IEnumerable<TimeSlot>  AvailableTimeSlots,
                        DateTime               LastUpdated,
                        UInt32?                StepSize   = null)
        {

            this.Id                  = Id;
            this.BeginFrom           = BeginFrom;
            this.EndBefore           = EndBefore;
            this.AvailableTimeSlots  = AvailableTimeSlots.Distinct();
            this.LastUpdated         = LastUpdated;
            this.StepSize            = StepSize;

            unchecked
            {

                hashCode = this.Id.                GetHashCode()  * 13 ^
                           this.BeginFrom.         GetHashCode()  * 11 ^
                           this.EndBefore.         GetHashCode()  *  7 ^
                           this.AvailableTimeSlots.CalcHashCode() *  5 ^
                           this.LastUpdated.       GetHashCode()  *  3 ^
                           this.StepSize?.         GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomCalendarParser = null)

        /// <summary>
        /// Parse the given JSON representation of a calendar.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCalendarParser">A delegate to parse custom calendar JSON objects.</param>
        public static Calendar Parse(JObject                                 JSON,
                                     CustomJObjectParserDelegate<Calendar>?  CustomCalendarParser   = null)
        {

            if (TryParse(JSON,
                         out var calendar,
                         out var errorResponse,
                         CustomCalendarParser))
            {
                return calendar;
            }

            throw new ArgumentException("The given JSON representation of a calendar is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Calendar, out ErrorResponse, CustomCalendarParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a calendar.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Calendar">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out Calendar?  Calendar,
                                       [NotNullWhen(false)] out String?    ErrorResponse)

            => TryParse(JSON,
                        out Calendar,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a calendar.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Calendar">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCalendarParser">A delegate to parse custom calendar JSON objects.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out Calendar?      Calendar,
                                       [NotNullWhen(false)] out String?        ErrorResponse,
                                       CustomJObjectParserDelegate<Calendar>?  CustomCalendarParser)
        {

            try
            {

                Calendar = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                    [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "calendar identification",
                                         Calendar_Id.TryParse,
                                         out Calendar_Id id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse BeginFrom             [mandatory]

                if (!JSON.ParseMandatory("begin_from",
                                         "begin from",
                                         out DateTimeOffset beginFrom,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EndBefore             [mandatory]

                if (!JSON.ParseMandatory("end_before",
                                         "end before",
                                         out DateTimeOffset endBefore,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AvailableTimeSlots    [mandatory]

                if (!JSON.ParseMandatoryList("available_timeslots",
                                             "available time slots",
                                             TimeSlot.TryParse,
                                             out List<TimeSlot> availableTimeSlots,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EndBefore             [mandatory]

                if (!JSON.ParseMandatory("end_before",
                                         "end before",
                                         out DateTime lastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse StepSize              [optional]

                if (!JSON.ParseOptional("step_size",
                                        "step size",
                                        out UInt32? stepSize,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Calendar = new Calendar(
                               id,
                               beginFrom,
                               endBefore,
                               availableTimeSlots,
                               lastUpdated,
                               stepSize
                           );

                if (CustomCalendarParser is not null)
                    Calendar = CustomCalendarParser(JSON,
                                                    Calendar);

                return true;

            }
            catch (Exception e)
            {
                Calendar       = default;
                ErrorResponse  = "The given JSON representation of a calendar is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCalendarSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCalendarSerializer">A delegate to serialize custom calendar JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Calendar>? CustomCalendarSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                    Id.         ToString()),
                                 new JProperty("begin_from",            BeginFrom.  ToISO8601()),
                                 new JProperty("end_before",            EndBefore.  ToISO8601()),
                                 new JProperty("available_timeslots",   new JArray(AvailableTimeSlots.Select(timeSlot => timeSlot.ToJSON()))),
                                 new JProperty("last_updated",          LastUpdated.ToISO8601()),

                           StepSize.HasValue
                               ? new JProperty("step_size",             StepSize.Value)
                               : null

                       );

            return CustomCalendarSerializer is not null
                       ? CustomCalendarSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this calendar.
        /// </summary>
        public Calendar Clone()

            => new (
                   Id.Clone(),
                   BeginFrom,
                   EndBefore,
                   AvailableTimeSlots.Select(timeSlot => timeSlot.Clone()),
                   LastUpdated,
                   StepSize
               );

        #endregion


        #region Operator overloading

        #region Operator == (Calendar1, Calendar2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Calendar1">A calendar.</param>
        /// <param name="Calendar2">Another calendar.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Calendar? Calendar1,
                                           Calendar? Calendar2)
        {

            if (Object.ReferenceEquals(Calendar1, Calendar2))
                return true;

            if (Calendar1 is null || Calendar2 is null)
                return false;

            return Calendar1.Equals(Calendar2);

        }

        #endregion

        #region Operator != (Calendar1, Calendar2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Calendar1">A calendar.</param>
        /// <param name="Calendar2">Another calendar.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Calendar? Calendar1,
                                           Calendar? Calendar2)

            => !(Calendar1 == Calendar2);

        #endregion

        #region Operator <  (Calendar1, Calendar2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Calendar1">A calendar.</param>
        /// <param name="Calendar2">Another calendar.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Calendar? Calendar1,
                                          Calendar? Calendar2)

            => Calendar1 is null
                   ? throw new ArgumentNullException(nameof(Calendar1), "The given calendar must not be null!")
                   : Calendar1.CompareTo(Calendar2) < 0;

        #endregion

        #region Operator <= (Calendar1, Calendar2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Calendar1">A calendar.</param>
        /// <param name="Calendar2">Another calendar.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Calendar? Calendar1,
                                           Calendar? Calendar2)

            => !(Calendar1 > Calendar2);

        #endregion

        #region Operator >  (Calendar1, Calendar2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Calendar1">A calendar.</param>
        /// <param name="Calendar2">Another calendar.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Calendar? Calendar1,
                                          Calendar? Calendar2)

            => Calendar1 is null
                   ? throw new ArgumentNullException(nameof(Calendar1), "The given calendar must not be null!")
                   : Calendar1.CompareTo(Calendar2) > 0;

        #endregion

        #region Operator >= (Calendar1, Calendar2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Calendar1">A calendar.</param>
        /// <param name="Calendar2">Another calendar.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Calendar? Calendar1,
                                           Calendar? Calendar2)

            => !(Calendar1 < Calendar2);

        #endregion

        #endregion

        #region IComparable<Calendar> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two calendars.
        /// </summary>
        /// <param name="Object">A calendar to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Calendar calendar
                   ? CompareTo(calendar)
                   : throw new ArgumentException("The given object is not a calendar!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Calendar)

        /// <summary>s
        /// Compares two calendars.
        /// </summary>
        /// <param name="Object">A calendar to compare with.</param>
        public Int32 CompareTo(Calendar? Calendar)
        {

            if (Calendar is null)
                throw new ArgumentNullException(nameof(Calendar), "The given calendar must not be null!");

            var c = Id.         CompareTo(Calendar.Id);

            if (c == 0)
                c = BeginFrom.  CompareTo(Calendar.BeginFrom);

            if (c == 0)
                c = EndBefore.  CompareTo(Calendar.EndBefore);

            if (c == 0)
                c = LastUpdated.CompareTo(Calendar.LastUpdated);

            if (c == 0)
                c = AvailableTimeSlots.Order().AggregateWith(",").CompareTo(Calendar.AvailableTimeSlots.Order().AggregateWith(","));

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Calendar> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two calendar for equality.
        /// </summary>
        /// <param name="Object">Calendar to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Calendar calendar &&
                   Equals(calendar);

        #endregion

        #region Equals(Calendar)

        /// <summary>
        /// Compares two calendar for equality.
        /// </summary>
        /// <param name="Calendar">Calendar to compare with.</param>
        public Boolean Equals(Calendar? Calendar)

            => Calendar is not null &&

               Id.                        Equals       (Calendar.Id)          &&
               BeginFrom.                 Equals       (Calendar.BeginFrom)   &&
               EndBefore.                 Equals       (Calendar.EndBefore)   &&
               AvailableTimeSlots.Order().SequenceEqual(Calendar.AvailableTimeSlots.Order()) &&
               LastUpdated.               Equals       (Calendar.LastUpdated) &&

            ((!StepSize.HasValue && !Calendar.StepSize.HasValue) ||
              (StepSize.HasValue &&  Calendar.StepSize.HasValue && StepSize.Value.Equals(Calendar.StepSize.Value)));

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

                   $"{Id}: {BeginFrom} -> {EndBefore}",

                   StepSize.HasValue
                       ? ", step size: " + StepSize.Value
                       : ""

               );

        #endregion

    }

}
