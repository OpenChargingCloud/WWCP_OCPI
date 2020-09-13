/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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
    /// Charging tariff restrictions.
    /// </summary>
    public class TariffRestrictions : IEquatable<TariffRestrictions>
    {

        #region Properties

        /// <summary>
        /// Start time of day, for example "13:30", valid from this time of the day.
        /// </summary>
        public Time?                         StartTime           { get; }

        /// <summary>
        /// End time of day, for example "19:45", valid from this time of the day.
        /// </summary>
        public Time?                         EndTime             { get; }

        /// <summary>
        /// Start date, for example: 2015-12-24, valid from this day until that day (excluding that day).
        /// </summary>
        public DateTime?                     StartDate           { get; }

        /// <summary>
        /// End date, for example: 2015-12-24, valid from this day until that day (excluding that day).
        /// </summary>
        public DateTime?                     EndDate             { get; }

        /// <summary>
        /// Minimum consumed energy in kWh, for example 20, valid from this amount of energy
        /// (inclusive) being used.
        /// </summary>
        public Decimal?                      MinkWh              { get; }

        /// <summary>
        /// Maximum consumed energy in kWh, for example 50, valid until this amount of energy
        /// (exclusive) being used.
        /// </summary>
        public Decimal?                      MaxkWh              { get; }

        /// <summary>
        /// Sum of the minimum current (in Amperes) over all phases, for example 5. When the EV is
        /// charging with more than, or equal to, the defined amount of current, this TariffElement
        /// is/becomes active. If the charging current is or becomes lower, this TariffElement is
        /// not or no longer valid and becomes inactive. This describes NOT the minimum current over
        /// the entire charging session. This restriction can make a TariffElement become active
        /// when the charging current is above the defined value, but the TariffElement MUST no
        /// longer be active when the charging current drops below the defined value.
        /// </summary>
        public Decimal?                      MinCurrent          { get; }

        /// <summary>
        /// Sum of the maximum current (in Amperes) over all phases, for example 20. When the EV is
        /// charging with less than the defined amount of current, this TariffElement becomes/is
        /// active. If the charging current is or becomes higher, this TariffElement is not or no
        /// longer valid and becomes inactive. This describes NOT the maximum current over the
        /// entire charging session. This restriction can make a TariffElement become active when
        /// the charging current is below this value, but the TariffElement MUST no longer be
        /// active when the charging current raises above the defined value.
        /// </summary>
        public Decimal?                      MaxCurrent          { get; }

        /// <summary>
        /// Minimum power in kW, for example 5. When the EV is charging with more than, or equal to,
        /// the defined amount of power, this TariffElement is/becomes active. If the charging power
        /// is or becomes lower, this TariffElement is not or no longer valid and becomes inactive.
        /// This describes NOT the minimum power over the entire charging session. This restriction
        /// can make a TariffElement become active when the charging power is above this value, but
        /// the TariffElement MUST no longer be active when the charging power drops below the
        /// defined value.
        /// </summary>
        public Decimal?                      MinPower            { get; }

        /// <summary>
        /// Maximum power in kW, for example 20. When the EV is charging with less than the defined
        /// amount of power, this TariffElement becomes/is active. If the charging power is or
        /// becomes higher, this TariffElement is not or no longer valid and becomes inactive. This
        /// describes NOT the maximum power over the entire Charging Session. This restriction can
        /// make a TariffElement become active when the charging power is below this value, but the
        /// TariffElement MUST no longer be active when the charging power raises above the defined
        /// value.
        /// </summary>
        public Decimal?                      MaxPower            { get; }

        /// <summary>
        /// Minimum duration in seconds the Charging Session MUST last (inclusive). When the
        /// duration of a Charging Session is longer than the defined value, this TariffElement is
        /// or becomes active. Before that moment, this TariffElement is not yet active.
        /// </summary>
        public TimeSpan?                     MinDuration         { get; }

        /// <summary>
        /// Maximum duration in seconds the Charging Session MUST last (exclusive). When the
        /// duration of a Charging Session is shorter than the defined value, this TariffElement
        /// is or becomes active. After that moment, this TariffElement is no longer active.
        /// </summary>
        public TimeSpan?                     MaxDuration         { get; }

        /// <summary>
        /// Minimum/Maximum duration in seconds, valid for a duration from x seconds.
        /// </summary>
        public IEnumerable<DayOfWeek>        DayOfWeek           { get; }

        /// <summary>
        /// When this field is present, the TariffElement describes reservation costs.
        /// A reservation starts when the reservation is made, and ends when the driver
        /// starts charging on the reserved EVSE/charging location, or when the reservation
        /// expires. A reservation can only have: FLAT and TIME TariffDimensions, where TIME is
        /// for the duration of the reservation.
        /// </summary>
        public ReservationRestrictionTypes?  Reservation         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging tariff restrictions.
        /// </summary>
        /// <param name="Time">Start/end time of day, for example "13:30 - 19:45", valid from this time of the day.</param>
        /// <param name="Date">Start/end date, for example: 2015-12-24, valid from this day until that day (excluding that day).</param>
        /// <param name="kWh">Minimum/Maximum used energy in kWh, for example 20, valid from this amount of energy is used.</param>
        /// <param name="Power">Minimum/Maximum power in kW, for example 0, valid from this charging speed.</param>
        /// <param name="Duration">Minimum/Maximum duration in seconds, valid for a duration from x seconds.</param>
        /// <param name="DayOfWeek">Minimum/Maximum duration in seconds, valid for a duration from x seconds.</param>
        public TariffRestrictions(Time?                         StartTime     = null,
                                  Time?                         EndTime       = null,
                                  DateTime?                     StartDate     = null,
                                  DateTime?                     EndDate       = null,
                                  Decimal?                      MinkWh        = null,
                                  Decimal?                      MaxkWh        = null,
                                  Decimal?                      MinPower      = null,
                                  Decimal?                      MaxPower      = null,
                                  TimeSpan?                     MinDuration   = null,
                                  TimeSpan?                     MaxDuration   = null,
                                  IEnumerable<DayOfWeek>        DayOfWeek     = null,
                                  ReservationRestrictionTypes?  Reservation   = null)
        {



            this.StartTime     = StartTime;
            this.EndTime       = EndTime;
            this.StartDate     = StartDate;
            this.EndDate       = EndDate;
            this.MinkWh        = MinkWh;
            this.MaxkWh        = MaxkWh;
            this.MinPower      = MinPower;
            this.MaxPower      = MaxPower;
            this.MinDuration   = MinDuration;
            this.MaxDuration   = MaxDuration;
            this.DayOfWeek     = DayOfWeek?.Distinct() ?? new DayOfWeek[0]; ;
            this.Reservation   = Reservation;

        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()
        {

            var JSON = JSONObject.Create(
                           //new JProperty("type",       _Type.    ToString()),
                           //new JProperty("type", _Type.ToString()),
                           //new JProperty("price",      _Price.   ToString()),

                           StartTime.HasValue ? new JProperty("start_time",  StartTime.Value.ToString())       : null,
                           EndTime.  HasValue ? new JProperty("end_time",    EndTime.  Value.ToString())       : null,

                           MinkWh.   HasValue ? new JProperty("min_kWh",     MinkWh.   Value.ToString("0.00")) : null,
                           MaxkWh.   HasValue ? new JProperty("max_kWh",     MaxkWh.   Value.ToString("0.00")) : null,

                           MinPower. HasValue ? new JProperty("min_power",   MinPower. Value.ToString("0.00")) : null,
                           MaxPower. HasValue ? new JProperty("max_power",   MaxPower. Value.ToString("0.00")) : null,

                           DayOfWeek.Any() ? new JProperty("day_of_week", new JArray(DayOfWeek.Select(day => day.ToString().ToUpper()))) : null);

            return JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (TariffRestriction1, TariffRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffRestriction1">A specification of charging tariff restrictions.</param>
        /// <param name="TariffRestriction2">Another specification of charging tariff restrictions.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffRestrictions TariffRestriction1,
                                           TariffRestrictions TariffRestriction2)

            => TariffRestriction1.Equals(TariffRestriction2);

        #endregion

        #region Operator != (TariffRestriction1, TariffRestriction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffRestriction1">A specification of charging tariff restrictions.</param>
        /// <param name="TariffRestriction2">Another specification of charging tariff restrictions.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffRestrictions TariffRestriction1,
                                           TariffRestrictions TariffRestriction2)

            => !(TariffRestriction1 == TariffRestriction2);

        #endregion

        #endregion

        #region IEquatable<TariffRestriction> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is TariffRestrictions tariffRestrictions &&
                   Equals(tariffRestrictions);

        #endregion

        #region Equals(TariffRestriction)

        /// <summary>
        /// Compares two charging tariff restrictions for equality.
        /// </summary>
        /// <param name="TariffRestrictions">A charging tariff restriction to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(TariffRestrictions TariffRestrictions)

            => !(TariffRestrictions is null)                                                                                                            &&

                ((!StartTime.  HasValue && !TariffRestrictions.StartTime.  HasValue) ||
                  (StartTime.  HasValue &&  TariffRestrictions.StartTime.  HasValue && StartTime.  Value.Equals(TariffRestrictions.StartTime.  Value))) &&

                ((!EndTime.    HasValue && !TariffRestrictions.EndTime.    HasValue) ||
                  (EndTime.    HasValue &&  TariffRestrictions.EndTime.    HasValue && EndTime.    Value.Equals(TariffRestrictions.EndTime.    Value))) &&

                (( StartDate.  HasValue &&  TariffRestrictions.StartDate.  HasValue) ||
                 ( StartDate.  HasValue &&  TariffRestrictions.StartDate.  HasValue && StartDate.  Value.Equals(TariffRestrictions.StartDate.  Value))) &&

                (( EndDate.    HasValue &&  TariffRestrictions.EndDate.    HasValue) ||
                 ( EndDate.    HasValue &&  TariffRestrictions.EndDate.    HasValue && EndDate.    Value.Equals(TariffRestrictions.EndDate.    Value))) &&

                ((!MinkWh.     HasValue && !TariffRestrictions.MinkWh.     HasValue) ||
                  (MinkWh.     HasValue &&  TariffRestrictions.MinkWh.     HasValue && MinkWh.     Value.Equals(TariffRestrictions.MinkWh.     Value))) &&

                ((!MaxkWh.     HasValue && !TariffRestrictions.MaxkWh.     HasValue) ||
                  (MaxkWh.     HasValue &&  TariffRestrictions.MaxkWh.     HasValue && MaxkWh.     Value.Equals(TariffRestrictions.MaxkWh.     Value))) &&

                ((!MinPower.   HasValue && !TariffRestrictions.MinPower.   HasValue) ||
                  (MinPower.   HasValue &&  TariffRestrictions.MinPower.   HasValue && MinPower.   Value.Equals(TariffRestrictions.MinPower.   Value))) &&

                ((!MaxPower.   HasValue && !TariffRestrictions.MaxPower.   HasValue) ||
                  (MaxPower.   HasValue &&  TariffRestrictions.MaxPower.   HasValue && MaxPower.   Value.Equals(TariffRestrictions.MaxPower.   Value))) &&

                ((!MinDuration.HasValue && !TariffRestrictions.MinDuration.HasValue) ||
                  (MinDuration.HasValue &&  TariffRestrictions.MinDuration.HasValue && MinDuration.Value.Equals(TariffRestrictions.MinDuration.Value))) &&

                ((!MaxDuration.HasValue && !TariffRestrictions.MaxDuration.HasValue) ||
                  (MaxDuration.HasValue &&  TariffRestrictions.MaxDuration.HasValue && MaxDuration.Value.Equals(TariffRestrictions.MaxDuration.Value))) &&

                DayOfWeek.Count().Equals(TariffRestrictions.DayOfWeek.Count()) &&
                DayOfWeek.All(day => TariffRestrictions.DayOfWeek.Contains(day));

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return (StartTime.HasValue
                            ? StartTime.  GetHashCode() * 29
                            : 0) ^

                       (EndTime.HasValue
                            ? EndTime.    GetHashCode() * 27
                            : 0) ^

                       (StartDate.HasValue
                            ? StartDate.  GetHashCode() * 23
                            : 0) ^

                       (EndDate.HasValue
                            ? EndDate.    GetHashCode() * 21
                            : 0) ^

                       (MinkWh.HasValue
                            ? MinkWh.     GetHashCode() * 17
                            : 0) ^

                       (MaxkWh.HasValue
                            ? MaxkWh.     GetHashCode() * 13
                            : 0) ^

                       (MinPower.HasValue
                            ? MinPower.   GetHashCode() * 11
                            : 0) ^

                       (MaxPower.HasValue
                            ? MaxPower.   GetHashCode() *  7
                            : 0) ^

                       (MinDuration.HasValue
                            ? MinDuration.GetHashCode() *  5
                            : 0) ^

                       (MaxDuration.HasValue
                            ? MaxDuration.GetHashCode() *  3
                            : 0) ^

                       (DayOfWeek.SafeAny()
                            ? DayOfWeek.Aggregate(0, (hashCode, day) => hashCode ^ day.GetHashCode())
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(StartTime.HasValue
                                 ?            StartTime.Value.ToString()
                                 : "",
                             EndTime.HasValue
                                 ? " - "    + EndTime.  Value.ToString()
                                 : "",
                             StartDate.HasValue
                                 ? " from " + StartDate.Value.ToString()
                                 : "",
                             EndDate.HasValue
                                 ? " to "   + EndDate.  Value.ToString()
                                 : "",

                             MinkWh.HasValue
                                 ? "; > "   + MinkWh.   Value.ToString() + " kWh"
                                 : "",

                             MaxkWh.HasValue
                                 ? "; < "   + MaxkWh.   Value.ToString() + " kWh"
                                 : "",

                             MinPower.HasValue
                                 ? "; > "   + MinPower. Value.ToString() + "kW"
                                 : "",

                             MaxPower.HasValue
                                 ? "; < "   + MaxPower. Value.ToString() + "kW"
                                 : "",

                             MinDuration.HasValue
                                 ? "; > "   + MinDuration.Value.TotalMinutes.ToString("0.00") + " min"
                                 : "",

                             MaxDuration.HasValue
                                 ? "; < "   + MaxDuration.Value.TotalMinutes.ToString("0.00") + " min"
                                 : "",

                             DayOfWeek.SafeAny()
                                 ? "; "     + DayOfWeek.AggregateWith("|")
                                 : "");

        #endregion

    }

}
