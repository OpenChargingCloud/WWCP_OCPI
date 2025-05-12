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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// A time slot.
    /// </summary>
    public class TimeSlot : IEquatable<TimeSlot>,
                            IComparable<TimeSlot>,
                            IComparable
    {

        #region Properties

        /// <summary>
        /// Start time of this time slot.
        /// </summary>
        [Mandatory]
        public DateTimeOffset  Start                 { get; }

        /// <summary>
        /// End time of this time slot.
        /// </summary>
        [Mandatory]
        public DateTimeOffset  End                   { get; }

        /// <summary>
        /// The optional minimum power guaranteed during this time slot.
        /// </summary>
        [Optional]
        public Watt?           MinPower              { get; }

        /// <summary>
        /// The optional maximum power guaranteed during this time slot.
        /// Can be requested lower.
        /// </summary>
        [Optional]
        public Watt?           MaxPower              { get; }

        /// <summary>
        /// Optional indication wheather green energy is available during this time slot.
        /// </summary>
        [Optional]
        public Boolean?        GreenEnergySupport    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Start">The start time of this time slot.</param>
        /// <param name="End">The end time of this time slot.</param>
        /// <param name="MinPower">An optional minimum power guaranteed during this time slot.</param>
        /// <param name="MaxPower">An optional maximum power guaranteed during this time slot. Can be requested lower.</param>
        /// <param name="GreenEnergySupport">Optional indication wheather green energy is available during this time slot.</param>
        public TimeSlot(DateTimeOffset  Start,
                        DateTimeOffset  End,
                        Watt?           MinPower             = null,
                        Watt?           MaxPower             = null,
                        Boolean?        GreenEnergySupport   = null)
        {

            this.Start               = Start;
            this.End                 = End;
            this.MinPower            = MinPower;
            this.MaxPower            = MaxPower;
            this.GreenEnergySupport  = GreenEnergySupport;

            unchecked
            {

                hashCode = this.Start.              GetHashCode()       * 11 ^
                           this.End.                GetHashCode()       *  7 ^
                          (this.MinPower?.          GetHashCode() ?? 0) *  5 ^
                          (this.MaxPower?.          GetHashCode() ?? 0) *  3 ^
                           this.GreenEnergySupport?.GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomTimeSlotParser = null)

        /// <summary>
        /// Parse the given JSON representation of a time slot.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTimeSlotParser">A delegate to parse custom time slot JSON objects.</param>
        public static TimeSlot Parse(JObject                                 JSON,
                                     CustomJObjectParserDelegate<TimeSlot>?  CustomTimeSlotParser   = null)
        {

            if (TryParse(JSON,
                         out var timeSlot,
                         out var errorResponse,
                         CustomTimeSlotParser))
            {
                return timeSlot;
            }

            throw new ArgumentException("The given JSON representation of a time slot is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TimeSlot, out ErrorResponse, CustomTimeSlotParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a time slot.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TimeSlot">The parsed time slot.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out TimeSlot?  TimeSlot,
                                       [NotNullWhen(false)] out String?    ErrorResponse)

            => TryParse(JSON,
                        out TimeSlot,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a time slot.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TimeSlot">The parsed time slot.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTimeSlotParser">A delegate to parse custom time slot JSON objects.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out TimeSlot?      TimeSlot,
                                       [NotNullWhen(false)] out String?        ErrorResponse,
                                       CustomJObjectParserDelegate<TimeSlot>?  CustomTimeSlotParser   = null)
        {

            try
            {

                TimeSlot = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Start                 [mandatory]

                if (!JSON.ParseMandatory("start_from",
                                         "start from",
                                         out DateTimeOffset start,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse End                   [mandatory]

                if (!JSON.ParseMandatory("end_before",
                                         "end before",
                                         out DateTimeOffset end,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MinPower              [optional]

                if (JSON.ParseOptional("min_power",
                                       "minimum power",
                                       Watt.TryParse,
                                       out Watt? minPower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxPower              [optional]

                if (JSON.ParseOptional("max_power",
                                       "maximum power",
                                       Watt.TryParse,
                                       out Watt? maxPower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse GreenEnergySupport    [optional]

                if (JSON.ParseOptional("green_energy_support",
                                       "green energy support",
                                       out Boolean? greenEnergySupport,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion



                TimeSlot = new TimeSlot(
                               start,
                               end,
                               minPower,
                               maxPower,
                               greenEnergySupport
                           );


                if (CustomTimeSlotParser is not null)
                    TimeSlot = CustomTimeSlotParser(JSON,
                                                    TimeSlot);

                return true;

            }
            catch (Exception e)
            {
                TimeSlot       = default;
                ErrorResponse  = "The given JSON representation of a time slot is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTimeSlotSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTimeSlotSerializer">A delegate to serialize custom time slot JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TimeSlot>? CustomTimeSlotSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("start_from",             Start.ToISO8601()),
                                 new JProperty("end_before",             End.  ToISO8601()),

                           MinPower.HasValue
                               ? new JProperty("min_power",              MinPower.Value.    Value)
                               : null,

                           MaxPower.HasValue
                               ? new JProperty("max_power",              MaxPower.Value.    Value)
                               : null,

                           GreenEnergySupport.HasValue
                               ? new JProperty("green_energy_support",   GreenEnergySupport.Value)
                               : null

                       );

            return CustomTimeSlotSerializer is not null
                       ? CustomTimeSlotSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this time slot.
        /// </summary>
        public TimeSlot Clone()

            => new (
                   Start,
                   End,
                   MinPower?.Clone(),
                   MaxPower?.Clone(),
                   GreenEnergySupport
               );

        #endregion


        #region Operator overloading

        #region Operator == (TimeSlot1, TimeSlot2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeSlot1">A time slot.</param>
        /// <param name="TimeSlot2">Another time slot.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TimeSlot? TimeSlot1,
                                           TimeSlot? TimeSlot2)
        {

            if (Object.ReferenceEquals(TimeSlot1, TimeSlot2))
                return true;

            if (TimeSlot1 is null || TimeSlot2 is null)
                return false;

            return TimeSlot1.Equals(TimeSlot2);

        }

        #endregion

        #region Operator != (TimeSlot1, TimeSlot2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeSlot1">A time slot.</param>
        /// <param name="TimeSlot2">Another time slot.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TimeSlot? TimeSlot1,
                                           TimeSlot? TimeSlot2)

            => !(TimeSlot1 == TimeSlot2);

        #endregion

        #region Operator <  (TimeSlot1, TimeSlot2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeSlot1">A time slot.</param>
        /// <param name="TimeSlot2">Another time slot.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TimeSlot? TimeSlot1,
                                          TimeSlot? TimeSlot2)

            => TimeSlot1 is null
                   ? throw new ArgumentNullException(nameof(TimeSlot1), "The given time slot must not be null!")
                   : TimeSlot1.CompareTo(TimeSlot2) < 0;

        #endregion

        #region Operator <= (TimeSlot1, TimeSlot2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeSlot1">A time slot.</param>
        /// <param name="TimeSlot2">Another time slot.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TimeSlot? TimeSlot1,
                                           TimeSlot? TimeSlot2)

            => !(TimeSlot1 > TimeSlot2);

        #endregion

        #region Operator >  (TimeSlot1, TimeSlot2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeSlot1">A time slot.</param>
        /// <param name="TimeSlot2">Another time slot.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TimeSlot? TimeSlot1,
                                          TimeSlot? TimeSlot2)

            => TimeSlot1 is null
                   ? throw new ArgumentNullException(nameof(TimeSlot1), "The given time slot must not be null!")
                   : TimeSlot1.CompareTo(TimeSlot2) > 0;

        #endregion

        #region Operator >= (TimeSlot1, TimeSlot2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeSlot1">A time slot.</param>
        /// <param name="TimeSlot2">Another time slot.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TimeSlot? TimeSlot1,
                                           TimeSlot? TimeSlot2)

            => !(TimeSlot1 < TimeSlot2);

        #endregion

        #endregion

        #region IComparable<TimeSlot> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two time slots.
        /// </summary>
        /// <param name="Object">A time slot to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TimeSlot timeSlot
                   ? CompareTo(timeSlot)
                   : throw new ArgumentException("The given object is not a time slot!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TimeSlot)

        /// <summary>s
        /// Compares two time slots.
        /// </summary>
        /// <param name="Object">A time slot to compare with.</param>
        public Int32 CompareTo(TimeSlot? TimeSlot)
        {

            if (TimeSlot is null)
                throw new ArgumentNullException(nameof(TimeSlot), "The given time slot must not be null!");

            var c = Start. CompareTo(TimeSlot.Start);

            if (c == 0)
                c = End.     CompareTo(TimeSlot.End);

            if (c == 0 && MinPower.          HasValue && TimeSlot.MinPower.          HasValue)
                c = MinPower.          Value.CompareTo(TimeSlot.MinPower.          Value);

            if (c == 0 && MaxPower.          HasValue && TimeSlot.MaxPower.          HasValue)
                c = MaxPower.          Value.CompareTo(TimeSlot.MaxPower.          Value);

            if (c == 0 && GreenEnergySupport.HasValue && TimeSlot.GreenEnergySupport.HasValue)
                c = GreenEnergySupport.Value.CompareTo(TimeSlot.GreenEnergySupport.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<TimeSlot> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two time slots for equality.
        /// </summary>
        /// <param name="Object">A time slot to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TimeSlot timeSlot &&
                   Equals(timeSlot);

        #endregion

        #region Equals(TimeSlot)

        /// <summary>
        /// Compares two time slots for equality.
        /// </summary>
        /// <param name="TimeSlot">A time slot to compare with.</param>
        public Boolean Equals(TimeSlot? TimeSlot)

            => TimeSlot is not null &&

               Start.Equals(TimeSlot.Start) &&
               End.  Equals(TimeSlot.End)   &&

            ((!MinPower.          HasValue && !TimeSlot.MinPower.          HasValue) ||
              (MinPower.          HasValue &&  TimeSlot.MinPower.          HasValue && MinPower.          Value.Equals(TimeSlot.MinPower.          Value))) &&

            ((!MaxPower.          HasValue && !TimeSlot.MaxPower.          HasValue) ||
              (MaxPower.          HasValue &&  TimeSlot.MaxPower.          HasValue && MaxPower.          Value.Equals(TimeSlot.MaxPower.          Value))) &&

            ((!GreenEnergySupport.HasValue && !TimeSlot.GreenEnergySupport.HasValue)  ||
              (GreenEnergySupport.HasValue &&  TimeSlot.GreenEnergySupport.HasValue && GreenEnergySupport.Value.Equals(TimeSlot.GreenEnergySupport.Value)));

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

                   $"'{Start}' -> '{End}'",

                   MinPower.HasValue
                       ? $", min power: {MinPower.Value}"
                       : "",

                   MaxPower.HasValue
                       ? $", max power: {MaxPower.Value}"
                       : "",

                   GreenEnergySupport.HasValue
                       ? $", green energy: {GreenEnergySupport.Value}"
                       : ""

               );

        #endregion

    }

}
