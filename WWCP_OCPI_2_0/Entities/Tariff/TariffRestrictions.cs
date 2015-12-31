/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace org.GraphDefined.WWCP.OCPI_2_0
{

    /// <summary>
    /// A tariff restrictions class.
    /// </summary>
    public class TariffRestriction
    {

        #region Properties

        #region Time

        private readonly TimeRange? _Time;

        /// <summary>
        /// Start/end time of day, for example "13:30 - 19:45", valid from this time of the day.
        /// </summary>
        public TimeRange? Time
        {
            get
            {
                return _Time;
            }
        }

        #endregion

        #region Date

        private readonly StartEndDateTime? _Date;

        /// <summary>
        /// Start/end date, for example: 2015-12-24, valid from this day until that day (excluding that day).
        /// </summary>
        public StartEndDateTime? Date
        {
            get
            {
                return _Date;
            }
        }

        #endregion

        #region kWh

        private readonly DecimalMinMax? _kWh;

        /// <summary>
        /// Minimum/Maximum used energy in kWh, for example 20, valid from this amount of energy is used.
        /// </summary>
        public DecimalMinMax? kWh
        {
            get
            {
                return _kWh;
            }
        }

        #endregion

        #region Power

        private readonly DecimalMinMax? _Power;

        /// <summary>
        /// Minimum/Maximum power in kW, for example 0, valid from this charging speed.
        /// </summary>
        public DecimalMinMax? Power
        {
            get
            {
                return _Power;
            }
        }

        #endregion

        #region Duration

        private readonly TimeSpanMinMax? _Duration;

        /// <summary>
        /// Minimum/Maximum duration in seconds, valid for a duration from x seconds.
        /// </summary>
        public TimeSpanMinMax? Duration
        {
            get
            {
                return _Duration;
            }
        }

        #endregion

        #region DayOfWeek

        private readonly IEnumerable<DayOfWeek> _DayOfWeek;

        /// <summary>
        /// Minimum/Maximum duration in seconds, valid for a duration from x seconds.
        /// </summary>
        public IEnumerable<DayOfWeek> DayOfWeek
        {
            get
            {
                return _DayOfWeek;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tariff restrictions class.
        /// </summary>
        /// <param name="Time">Start/end time of day, for example "13:30 - 19:45", valid from this time of the day.</param>
        /// <param name="Date">Start/end date, for example: 2015-12-24, valid from this day until that day (excluding that day).</param>
        /// <param name="kWh">Minimum/Maximum used energy in kWh, for example 20, valid from this amount of energy is used.</param>
        /// <param name="Power">Minimum/Maximum power in kW, for example 0, valid from this charging speed.</param>
        /// <param name="Duration">Minimum/Maximum duration in seconds, valid for a duration from x seconds.</param>
        /// <param name="DayOfWeek">Minimum/Maximum duration in seconds, valid for a duration from x seconds.</param>
        public TariffRestriction(TimeRange?              Time       = null,
                                 StartEndDateTime?       Date       = null,
                                 DecimalMinMax?          kWh        = null,
                                 DecimalMinMax?          Power      = null,
                                 TimeSpanMinMax?         Duration   = null,
                                 IEnumerable<DayOfWeek>  DayOfWeek  = null)
        {

            #region Initial checks

            if (!Time.   HasValue &&
                !Date.   HasValue &&
                !kWh.    HasValue &&
                Power.   HasValue &&
                Duration.HasValue &&
                DayOfWeek == null)
                throw new ArgumentNullException("All given parameter equals null is invalid!");

            #endregion

            this._Time       = Time;
            this._Date       = Date;
            this._kWh        = kWh;
            this._Power      = Power;
            this._Duration   = Duration;
            this._DayOfWeek  = DayOfWeek != null ? DayOfWeek.Distinct() : new DayOfWeek[0];

        }

        #endregion


        #region (static) MinkWh(MinkWh)

        /// <summary>
        /// Create a new MinkWh tariff restriction.
        /// </summary>
        /// <param name="MinkWh">The minimum kWh value.</param>
        public static TariffRestriction MinkWh(Decimal MinkWh)
        {
            return new TariffRestriction(kWh: DecimalMinMax.FromMin(MinkWh));
        }

        #endregion

        #region (static) MinkWh(MaxkWh)

        /// <summary>
        /// Create a new MaxkWh tariff restriction.
        /// </summary>
        /// <param name="MaxkWh">The maximum kWh value.</param>
        public static TariffRestriction MaxkWh(Decimal MaxkWh)
        {
            return new TariffRestriction(kWh: DecimalMinMax.FromMax(MaxkWh));
        }

        #endregion

        #region (static) MinPower(MinPower)

        /// <summary>
        /// Create a new MinPower tariff restriction.
        /// </summary>
        /// <param name="MinPower">The minimum power value.</param>
        public static TariffRestriction MinPower(Decimal MinPower)
        {
            return new TariffRestriction(Power: DecimalMinMax.FromMin(MinPower));
        }

        #endregion

        #region (static) MinPower(MaxPower)

        /// <summary>
        /// Create a new MaxPower tariff restriction.
        /// </summary>
        /// <param name="MaxPower">The maximum power value.</param>
        public static TariffRestriction MaxPower(Decimal MaxPower)
        {
            return new TariffRestriction(Power: DecimalMinMax.FromMax(MaxPower));
        }

        #endregion

        #region (static) MinDuration(MinDuration)

        /// <summary>
        /// Create a new MinDuration tariff restriction.
        /// </summary>
        /// <param name="MinDuration">The minimum Duration value.</param>
        public static TariffRestriction MinDuration(TimeSpan MinDuration)
        {
            return new TariffRestriction(Duration: TimeSpanMinMax.FromMin(MinDuration));
        }

        #endregion

        #region (static) MinDuration(MaxDuration)

        /// <summary>
        /// Create a new MaxDuration tariff restriction.
        /// </summary>
        /// <param name="MaxDuration">The maximum Duration value.</param>
        public static TariffRestriction MaxDuration(TimeSpan MaxDuration)
        {
            return new TariffRestriction(Duration: TimeSpanMinMax.FromMax(MaxDuration));
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

                                     _Time.HasValue && _Time.Value.StartTime.HasValue ? new JProperty("start_time", _Time.Value.StartTime.Value.ToString()) : null,
                                     _Time.HasValue && _Time.Value.EndTime.  HasValue ? new JProperty("end_time",   _Time.Value.EndTime.  Value.ToString()) : null,

                                     _kWh.  HasValue && _kWh. Value.Min.HasValue ? new JProperty("min_kWh",    _kWh.  Value.Min.Value.ToString("0.00")) : null,
                                     _kWh.  HasValue && _kWh. Value.Max.HasValue ? new JProperty("max_kWh",    _kWh.  Value.Max.Value.ToString("0.00")) : null,

                                     _Power.HasValue && Power.Value.Min.HasValue ? new JProperty("min_power",  _Power.Value.Min.Value.ToString("0.00")) : null,
                                     _Power.HasValue && Power.Value.Max.HasValue ? new JProperty("max_power",  _Power.Value.Max.Value.ToString("0.00")) : null,

                                     _DayOfWeek.Any() ? new JProperty("day_of_week", new JArray(_DayOfWeek.Select(day => day.ToString().ToUpper()))) : null);

        }

        #endregion


        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return _Time.     GetHashCode() * 41 ^
                       _Date.     GetHashCode() * 37 ^
                       _kWh.      GetHashCode() * 31 ^
                       _Power.    GetHashCode() * 23 ^
                       _Duration. GetHashCode() * 17 ^
                       _DayOfWeek.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        ///// <summary>
        ///// Get a string representation of this object.
        ///// </summary>
        //public override String ToString()
        //{
        //    return String.Concat("type: ", _Type.ToString(), ", price: ", _Price.ToString(), ", step size:", _StepSize.ToString());
        //}

        #endregion

    }

}
