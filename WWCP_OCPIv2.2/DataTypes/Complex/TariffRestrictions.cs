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
using org.GraphDefined.Vanaheimr.Hermod;

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
        /// Start/end time of day, for example "13:30 - 19:45", valid from this time of the day.
        /// </summary>
        public TimeRange?              Time         { get; }

        /// <summary>
        /// Start/end date, for example: 2015-12-24, valid from this day until that day (excluding that day).
        /// </summary>
        public StartEndDateTime        Date         { get; }

        /// <summary>
        /// Minimum/Maximum used energy in kWh, for example 20, valid from this amount of energy is used.
        /// </summary>
        public DecimalMinMax?          kWh          { get; }

        /// <summary>
        /// Minimum/Maximum power in kW, for example 0, valid from this charging speed.
        /// </summary>
        public DecimalMinMax?          Power        { get; }

        /// <summary>
        /// Minimum/Maximum duration in seconds, valid for a duration from x seconds.
        /// </summary>
        public TimeSpanMinMax?         Duration     { get; }

        /// <summary>
        /// Minimum/Maximum duration in seconds, valid for a duration from x seconds.
        /// </summary>
        public IEnumerable<DayOfWeek>  DayOfWeek    { get; }

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
        public TariffRestrictions(TimeRange?              Time        = null,
                                  StartEndDateTime        Date        = null,
                                  DecimalMinMax?          kWh         = null,
                                  DecimalMinMax?          Power       = null,
                                  TimeSpanMinMax?         Duration    = null,
                                  IEnumerable<DayOfWeek>  DayOfWeek   = null)
        {

            #region Initial checks

            if (!Time.   HasValue &&
                 Date    != null  &&
                !kWh.    HasValue &&
                Power.   HasValue &&
                Duration.HasValue &&
                DayOfWeek == null)
            {
                throw new ArgumentNullException("All given parameter equals null is invalid!");
            }

            #endregion

            this.Time       = Time;
            this.Date       = Date;
            this.kWh        = kWh;
            this.Power      = Power;
            this.Duration   = Duration;
            this.DayOfWeek  = DayOfWeek?.Distinct() ?? new DayOfWeek[0];

        }

        #endregion


        #region (static) MinkWh(MinkWh)

        /// <summary>
        /// Create a new MinkWh tariff restriction.
        /// </summary>
        /// <param name="MinkWh">The minimum kWh value.</param>
        public static TariffRestrictions MinkWh(Decimal MinkWh)
        {
            return new TariffRestrictions(kWh: DecimalMinMax.FromMin(MinkWh));
        }

        #endregion

        #region (static) MinkWh(MaxkWh)

        /// <summary>
        /// Create a new MaxkWh tariff restriction.
        /// </summary>
        /// <param name="MaxkWh">The maximum kWh value.</param>
        public static TariffRestrictions MaxkWh(Decimal MaxkWh)
        {
            return new TariffRestrictions(kWh: DecimalMinMax.FromMax(MaxkWh));
        }

        #endregion

        #region (static) MinPower(MinPower)

        /// <summary>
        /// Create a new MinPower tariff restriction.
        /// </summary>
        /// <param name="MinPower">The minimum power value.</param>
        public static TariffRestrictions MinPower(Decimal MinPower)
        {
            return new TariffRestrictions(Power: DecimalMinMax.FromMin(MinPower));
        }

        #endregion

        #region (static) MinPower(MaxPower)

        /// <summary>
        /// Create a new MaxPower tariff restriction.
        /// </summary>
        /// <param name="MaxPower">The maximum power value.</param>
        public static TariffRestrictions MaxPower(Decimal MaxPower)
        {
            return new TariffRestrictions(Power: DecimalMinMax.FromMax(MaxPower));
        }

        #endregion

        #region (static) MinDuration(MinDuration)

        /// <summary>
        /// Create a new MinDuration tariff restriction.
        /// </summary>
        /// <param name="MinDuration">The minimum Duration value.</param>
        public static TariffRestrictions MinDuration(TimeSpan MinDuration)
        {
            return new TariffRestrictions(Duration: TimeSpanMinMax.FromMin(MinDuration));
        }

        #endregion

        #region (static) MinDuration(MaxDuration)

        /// <summary>
        /// Create a new MaxDuration tariff restriction.
        /// </summary>
        /// <param name="MaxDuration">The maximum Duration value.</param>
        public static TariffRestrictions MaxDuration(TimeSpan MaxDuration)
        {
            return new TariffRestrictions(Duration: TimeSpanMinMax.FromMax(MaxDuration));
        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()
        {

            return JSONObject.Create(//new JProperty("type",       _Type.    ToString()),
                                     //new JProperty("type", _Type.ToString()),
                                     //new JProperty("price",      _Price.   ToString()),

                                     Time. HasValue && Time. Value.StartTime.HasValue ? new JProperty("start_time",   Time.Value.StartTime.Value.ToString())       : null,
                                     Time. HasValue && Time. Value.EndTime.  HasValue ? new JProperty("end_time",     Time.Value.EndTime.  Value.ToString())       : null,

                                     kWh.  HasValue && kWh.  Value.Min.      HasValue  ? new JProperty("min_kWh",      kWh.Value.Min.      Value.ToString("0.00")) : null,
                                     kWh.  HasValue && kWh.  Value.Max.      HasValue  ? new JProperty("max_kWh",      kWh.Value.Max.      Value.ToString("0.00")) : null,

                                     Power.HasValue && Power.Value.Min.      HasValue ? new JProperty("min_power",   Power.Value.Min.      Value.ToString("0.00")) : null,
                                     Power.HasValue && Power.Value.Max.      HasValue ? new JProperty("max_power",   Power.Value.Max.      Value.ToString("0.00")) : null,

                                     DayOfWeek.Any()                                  ? new JProperty("day_of_week", new JArray(DayOfWeek.Select(day => day.ToString().ToUpper()))) : null);

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

            => !(TariffRestrictions is null)                                                                                                &&

                ((!Time.    HasValue && !TariffRestrictions.Time.    HasValue) ||
                  (Time.    HasValue &&  TariffRestrictions.Time.    HasValue && Time.    Value.Equals(TariffRestrictions.Time.    Value))) &&

                (( Date     == null  &&  TariffRestrictions.Date     == null) ||
                 ( Date     != null  &&  TariffRestrictions.Date     != null  && Date.          Equals(TariffRestrictions.Date)))           &&

                ((!kWh.     HasValue && !TariffRestrictions.kWh.     HasValue) ||
                  (kWh.     HasValue &&  TariffRestrictions.kWh.     HasValue && kWh.     Value.Equals(TariffRestrictions.kWh.     Value))) &&

                ((!Power.   HasValue && !TariffRestrictions.Power.   HasValue) ||
                  (Power.   HasValue &&  TariffRestrictions.Power.   HasValue && Power.   Value.Equals(TariffRestrictions.Power.   Value))) &&

                ((!Duration.HasValue && !TariffRestrictions.Duration.HasValue) ||
                  (Duration.HasValue &&  TariffRestrictions.Duration.HasValue && Duration.Value.Equals(TariffRestrictions.Duration.Value))) &&

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

                return (Time.HasValue
                            ? Time.    GetHashCode() * 13
                            : 0) ^

                       (Date != null
                            ? Date.    GetHashCode() * 11
                            : 0) ^

                       (kWh.HasValue
                            ? kWh.     GetHashCode() *  7
                            : 0) ^

                       (Power.HasValue
                            ? Power.   GetHashCode() *  5
                            : 0) ^

                       (Duration.HasValue
                            ? Duration.GetHashCode() *  3
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

            => String.Concat(Time.HasValue
                                 ? Time.           Value.ToString()
                                 : "",

                             Date != null
                                 ? "; " + Date.          ToString()
                                 : "",

                             kWh.HasValue
                                 ? "; " + kWh.     Value.ToString()
                                 : "",

                             Power.HasValue
                                 ? "; " + Power.   Value.ToString()
                                 : "",

                             Duration.HasValue
                                 ? "; " + Duration.Value.ToString()
                                 : "",

                             DayOfWeek.SafeAny()
                                 ? "; " + DayOfWeek.AggregateWith(", ")
                                 : "");

        #endregion

    }

}
