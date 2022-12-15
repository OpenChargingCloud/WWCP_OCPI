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

using System;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// This type is used to schedule EVSE/connector status periods in the future.
    /// </summary>
    public readonly struct StatusSchedule : IEquatable<StatusSchedule>,
                                            IComparable<StatusSchedule>,
                                            IComparable
    {

        #region Properties

        /// <summary>
        /// Begin of the scheduled period.
        /// </summary>
        [Mandatory]
        public DateTime     Begin     { get; }

        /// <summary>
        /// Optional end of the scheduled period.
        /// </summary>
        [Optional]
        public DateTime?    End       { get; }

        /// <summary>
        /// EVSE status value during the scheduled period.
        /// </summary>
        [Mandatory]
        public StatusTypes  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// This type is used to schedule EVSE status periods in the future.
        /// </summary>
        /// <param name="Status">EVSE status value during the scheduled period.</param>
        /// <param name="Begin">Begin of the scheduled period.</param>
        /// <param name="End">Optional end of the scheduled period.</param>
        public StatusSchedule(StatusTypes  Status,
                              DateTime     Begin,
                              DateTime?    End   = null)
        {

            this.Status  = Status;
            this.Begin   = Begin;
            this.End     = End;

        }

        #endregion


        #region (static) Parse   (JSON, CustomStatusScheduleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a status schedule.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomStatusScheduleParser">A delegate to parse custom status schedule JSON objects.</param>
        public static StatusSchedule Parse(JObject                                      JSON,
                                           CustomJObjectParserDelegate<StatusSchedule>  CustomStatusScheduleParser   = null)
        {

            if (TryParse(JSON,
                         out StatusSchedule statusSchedule,
                         out String         ErrorResponse,
                         CustomStatusScheduleParser))
            {
                return statusSchedule;
            }

            throw new ArgumentException("The given JSON representation of a status schedule is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomStatusScheduleParser = null)

        /// <summary>
        /// Parse the given text representation of a status schedule.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomStatusScheduleParser">A delegate to parse custom status schedule JSON objects.</param>
        public static StatusSchedule Parse(String                                       Text,
                                           CustomJObjectParserDelegate<StatusSchedule>  CustomStatusScheduleParser   = null)
        {

            if (TryParse(Text,
                         out StatusSchedule statusSchedule,
                         out String         ErrorResponse,
                         CustomStatusScheduleParser))
            {
                return statusSchedule;
            }

            throw new ArgumentException("The given text representation of a status schedule is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out StatusSchedule, out ErrorResponse, CustomStatusScheduleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a status schedule.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="StatusSchedule">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       out StatusSchedule  StatusSchedule,
                                       out String          ErrorResponse)

            => TryParse(JSON,
                        out StatusSchedule,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a status schedule.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="StatusSchedule">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStatusScheduleParser">A delegate to parse custom status schedule JSON objects.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       out StatusSchedule                           StatusSchedule,
                                       out String                                   ErrorResponse,
                                       CustomJObjectParserDelegate<StatusSchedule>  CustomStatusScheduleParser   = null)
        {

            try
            {

                StatusSchedule = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Status    [mandatory]

                if (!JSON.ParseMandatoryEnum("status",
                                             "status",
                                             out StatusTypes Status,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Begin     [mandatory]

                if (!JSON.ParseMandatory("period_begin",
                                         "period begin",
                                         out DateTime Begin,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse End       [optional]

                if (JSON.ParseOptional("period_end",
                                       "period end",
                                       out DateTime? End,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                StatusSchedule = new StatusSchedule(Status,
                                                    Begin,
                                                    End);


                if (CustomStatusScheduleParser is not null)
                    StatusSchedule = CustomStatusScheduleParser(JSON,
                                                                StatusSchedule);

                return true;

            }
            catch (Exception e)
            {
                StatusSchedule  = default;
                ErrorResponse   = "The given JSON representation of a status schedule is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out StatusSchedule, out ErrorResponse, CustomStatusScheduleParser = null)

        /// <summary>
        /// Try to parse the given text representation of a status schedule.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="StatusSchedule">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStatusScheduleParser">A delegate to parse custom status schedule JSON objects.</param>
        public static Boolean TryParse(String                                       Text,
                                       out StatusSchedule                           StatusSchedule,
                                       out String                                   ErrorResponse,
                                       CustomJObjectParserDelegate<StatusSchedule>  CustomStatusScheduleParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out StatusSchedule,
                                out ErrorResponse,
                                CustomStatusScheduleParser);

            }
            catch (Exception e)
            {
                StatusSchedule = default;
                ErrorResponse  = "The given text representation of a status schedule is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomStatusScheduleSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StatusSchedule> CustomStatusScheduleSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("period_begin",      Begin.    ToIso8601()),

                           End.HasValue
                               ? new JProperty("period_end",  End.Value.ToIso8601())
                               : null,

                           new JProperty("status",            Status.   ToString())

                       );

            return CustomStatusScheduleSerializer is not null
                       ? CustomStatusScheduleSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StatusSchedule1, StatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule1">A status schedule.</param>
        /// <param name="StatusSchedule2">Another status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StatusSchedule StatusSchedule1,
                                           StatusSchedule StatusSchedule2)

            => StatusSchedule1.Equals(StatusSchedule2);

        #endregion

        #region Operator != (StatusSchedule1, StatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule1">A status schedule.</param>
        /// <param name="StatusSchedule2">Another status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (StatusSchedule StatusSchedule1,
                                           StatusSchedule StatusSchedule2)

            => !(StatusSchedule1 == StatusSchedule2);

        #endregion

        #region Operator <  (StatusSchedule1, StatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule1">A status schedule.</param>
        /// <param name="StatusSchedule2">Another status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (StatusSchedule StatusSchedule1,
                                          StatusSchedule StatusSchedule2)

            => StatusSchedule1.CompareTo(StatusSchedule2) < 0;

        #endregion

        #region Operator <= (StatusSchedule1, StatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule1">A status schedule.</param>
        /// <param name="StatusSchedule2">Another status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (StatusSchedule StatusSchedule1,
                                           StatusSchedule StatusSchedule2)

            => !(StatusSchedule1 > StatusSchedule2);

        #endregion

        #region Operator >  (StatusSchedule1, StatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule1">A status schedule.</param>
        /// <param name="StatusSchedule2">Another status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (StatusSchedule StatusSchedule1,
                                          StatusSchedule StatusSchedule2)

            => StatusSchedule1.CompareTo(StatusSchedule2) > 0;

        #endregion

        #region Operator >= (StatusSchedule1, StatusSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule1">A status schedule.</param>
        /// <param name="StatusSchedule2">Another status schedule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (StatusSchedule StatusSchedule1,
                                           StatusSchedule StatusSchedule2)

            => !(StatusSchedule1 < StatusSchedule2);

        #endregion

        #endregion

        #region IComparable<StatusSchedule> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is StatusSchedule statusSchedule
                   ? CompareTo(statusSchedule)
                   : throw new ArgumentException("The given object is not a status schedule!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(StatusSchedule)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusSchedule">An object to compare with.</param>
        public Int32 CompareTo(StatusSchedule StatusSchedule)
        {

            var c = Begin.ToIso8601().CompareTo(StatusSchedule.Begin.ToIso8601());

            if (c == 0)
                c = Status.           CompareTo(StatusSchedule.Status);

            if (c == 0)
                c = End.HasValue && StatusSchedule.End.HasValue
                        ? End.Value.ToIso8601().CompareTo(StatusSchedule.End.Value.ToIso8601())
                        : 0;

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<StatusSchedule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is StatusSchedule statusSchedule &&
                   Equals(statusSchedule);

        #endregion

        #region Equals(StatusSchedule)

        /// <summary>
        /// Compares two status schedules for equality.
        /// </summary>
        /// <param name="StatusSchedule">A status schedule to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(StatusSchedule StatusSchedule)

            => Begin. ToIso8601().Equals(StatusSchedule.Begin.ToIso8601()) &&
               Status.            Equals(StatusSchedule.Status)            &&

               ((!End.HasValue && !StatusSchedule.End.HasValue) ||
                 (End.HasValue &&  StatusSchedule.End.HasValue && End.Value.ToIso8601().Equals(StatusSchedule.End.Value.ToIso8601())));

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

                return Begin. ToIso8601().GetHashCode() * 5 ^
                       Status.            GetHashCode() * 3 ^

                       (End.HasValue
                            ? End.Value.ToIso8601().GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Begin,
                             " -> ",
                             End.HasValue
                                 ? End.Value.ToString()
                                 : "...",
                             " : ",
                             Status);

        #endregion

    }

}
