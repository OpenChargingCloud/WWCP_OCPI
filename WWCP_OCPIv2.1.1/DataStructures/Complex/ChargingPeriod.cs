/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    public enum MeteringValueSource
    {
        Measured,
        Imputed
    }

    public class MeteringValue(DateTime             Timestamp,
                               Decimal              Value,
                               MeteringValueSource  Source = MeteringValueSource.Measured)
    {

        public DateTime             Timestamp    { get; }  = Timestamp;
        public Decimal              Value        { get; }  = Value;
        public MeteringValueSource  Source       { get; }  = Source;


        public static MeteringValue Measured(DateTime Timestamp, Decimal Value)
            => new (Timestamp, Value, MeteringValueSource.Measured);

        public static MeteringValue Imputed(DateTime Timestamp, Decimal Value)
            => new (Timestamp, Value, MeteringValueSource.Imputed);

    }


    /// <summary>
    /// A charging period consists of a start timestamp and a list of
    /// possible values that influence this period, for example:
    /// Amount of energy charged this period, maximum current during
    /// this period etc.
    /// This implementation has many extensions to simplify the calculation
    /// of charging costs in combination with charging tariffs.
    /// </summary>
    public class ChargingPeriod
    {

        #region Properties

        public  UInt32                                       Id                    { get; set; }

        /// <summary>
        /// Start timestamp of the charging period.
        /// This period ends when a next period starts,
        /// the last period ends when the session ends.
        /// </summary>
        [Mandatory]
        public  DateTime                                     StartTimestamp        { get; }

        /// <summary>
        /// List of relevant values for this charging period.
        /// </summary>
        [Mandatory]
        public  HashSet<CDRDimension>                        Dimensions            { get; } = [];




        #region Our extensions!!!

        /// <summary>
        /// The final stop timestamp when this charging period and
        /// with it the entire charging session ends.
        /// Thus there is no 'next' charging period having a start
        /// timestamp.
        /// </summary>
        private  DateTime?                                   endTimestamp;

        public DateTime? SetEndTimestamp(DateTime? Timestamp)
        {
            endTimestamp = Timestamp;
            return endTimestamp;
        }

        /// <summary>
        /// Stop timestamp of the charging period.
        /// This period ends when a next period starts,
        /// the last period ends when the session ends.
        /// </summary>
        [Optional]
        public  DateTime?                                    StopTimestamp
            => Next is not null
                   ? Next.StartTimestamp
                   : endTimestamp;



        public  Decimal   FlatPrice         { get; set; }



        public  WattHour  Energy            { get; set; }
        public  Decimal   EnergyPrice       { get; set; }
        public  UInt32    EnergyStepSize    { get; set; }



        public  TimeSpan  Duration
             => StopTimestamp.HasValue
                    ? StopTimestamp.Value - StartTimestamp
                    : Timestamp.Now       - StartTimestamp;

        public  Decimal   TimePrice         { get; set; }
        public  UInt32    TimeStepSize      { get; set; }

        public  TimeSpan                                     TotalDuration
            => TimeSpan.FromSeconds(GetThisAndAllPrevious.Sum(chargingPeriods => chargingPeriods.Duration.TotalSeconds));




        public  Watt       PowerAverage        { get; set; }




        public  Dictionary<TariffDimension, PriceComponent>  PriceComponents       { get; } = [];





        public  MeteringValue?                               StartMeteringValue    { get; set; }


        private MeteringValue?                               stopMeteringValue;
        public  MeteringValue?                               StopMeteringValue {

            get
            {

                if (next is not null)
                    return next.StartMeteringValue;

                return stopMeteringValue;

            }

            set
            {
                stopMeteringValue = value;
            }

        }



        public  ChargingPeriod                               First
        {
            get
            {

                var current = this;

                while (current.Previous is not null)
                    current = current.Previous;

                return current;

            }
        }
        public  ChargingPeriod?                              Previous              { get; set; }

        private ChargingPeriod? next;
        public ChargingPeriod?                               Next {

            get
            {
                return next;
            }

            set
            {
                next          = value;
                endTimestamp  = null;
            }

        }

        public  IEnumerable<ChargingPeriod>                  GetAllPrevious
        {
            get
            {

                var allPrevious = new List<ChargingPeriod>();

                var current = this;

                while (current.Previous is not null)
                {
                    allPrevious.Add(current.Previous);
                    current = current.Previous;
                }

                return allPrevious;

            }
        }

        public  IEnumerable<ChargingPeriod>                  GetThisAndAllPrevious
        {
            get
            {

                var allPrevious = new List<ChargingPeriod>() { this };

                var current = this;

                while (current.Previous is not null)
                {
                    allPrevious.Add(current.Previous);
                    current = current.Previous;
                }

                return allPrevious;

            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A charging period consists of a start timestamp and a
        /// list of possible values that influence this period.
        /// </summary>
        /// <param name="StartTimestamp">Start timestamp of the charging period.</param>
        /// <param name="Dimensions">List of relevant values for this charging period.</param>
        public ChargingPeriod(DateTime                    StartTimestamp,
                              IEnumerable<CDRDimension>?  Dimensions   = null)
        {

            this.StartTimestamp  = StartTimestamp;
            this.Dimensions      = Dimensions is not null
                                       ? new HashSet<CDRDimension>(Dimensions)
                                       : [];

        }


        /// <summary>
        /// A charging period consists of a start timestamp and a
        /// list of possible values that influence this period.
        /// </summary>
        /// <param name="StartTimestamp">Start timestamp of the charging period.</param>
        /// <param name="Dimensions">List of relevant values for this charging period.</param>
        public ChargingPeriod(DateTime                    StartTimestamp,
                              ChargingPeriod              Previous,
                              ChargingPeriod              Next,
                              IEnumerable<CDRDimension>?  Dimensions   = null)
        {

            this.StartTimestamp  = StartTimestamp;
            this.Dimensions      = Dimensions is not null
                                       ? new HashSet<CDRDimension>(Dimensions)
                                       : [];

            this.Previous        = Previous;
            this.next            = Next;

            Previous.next        = this;
            next.Previous        = this;

        }


        /// <summary>
        /// A charging period consists of a start timestamp and a
        /// list of possible values that influence this period.
        /// </summary>
        /// <param name="StartTimestamp">Start timestamp of the charging period.</param>
        /// <param name="Dimensions">List of relevant values for this charging period.</param>
        public ChargingPeriod(DateTime                    StartTimestamp,
                              DateTime                    EndTimestamp,
                              ChargingPeriod              Previous,
                              IEnumerable<CDRDimension>?  Dimensions   = null)
        {

            this.StartTimestamp     = StartTimestamp;
            this.endTimestamp       = EndTimestamp;
            this.Dimensions         = Dimensions is not null
                                          ? new HashSet<CDRDimension>(Dimensions)
                                          : [];

            this.Previous           = Previous;
            this.next               = Next;

            Previous.next           = this;

        }

        #endregion


        #region (static) Parse   (JSON, CustomChargingPeriodParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomChargingPeriodParser">A delegate to parse custom charging period JSON objects.</param>
        public static ChargingPeriod Parse(JObject                                       JSON,
                                           CustomJObjectParserDelegate<ChargingPeriod>?  CustomChargingPeriodParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingPeriod,
                         out var errorResponse,
                         CustomChargingPeriodParser))
            {
                return chargingPeriod;
            }

            throw new ArgumentException("The given JSON representation of a charging period is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomChargingPeriodParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charging period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomChargingPeriodParser">A delegate to parse custom charging period JSON objects.</param>
        public static ChargingPeriod? TryParse(JObject                                       JSON,
                                               CustomJObjectParserDelegate<ChargingPeriod>?  CustomChargingPeriodParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingPeriod,
                         out var errorResponse,
                         CustomChargingPeriodParser))
            {
                return chargingPeriod;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingPeriod, out ErrorResponse, CustomChargingPeriodParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingPeriod">The parsed charging period.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out ChargingPeriod?  ChargingPeriod,
                                       [NotNullWhen(false)] out String?          ErrorResponse)

            => TryParse(JSON,
                        out ChargingPeriod,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingPeriod">The parsed charging period.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingPeriodParser">A delegate to parse custom charging period JSON objects.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out ChargingPeriod?      ChargingPeriod,
                                       [NotNullWhen(false)] out String?              ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingPeriod>?  CustomChargingPeriodParser   = null)
        {

            try
            {

                ChargingPeriod = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse StartTimestamp    [mandatory]

                if (!JSON.ParseMandatory("start_date_time",
                                         "energy supplier name",
                                         out DateTime StartTimestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Dimensions        [mandatory]

                if (!JSON.ParseMandatoryJSON("dimensions",
                                             "charging dimensions",
                                             CDRDimension.TryParse,
                                             out IEnumerable<CDRDimension> Dimensions,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ChargingPeriod = new ChargingPeriod(
                                     StartTimestamp,
                                     Dimensions
                                 );


                if (CustomChargingPeriodParser is not null)
                    ChargingPeriod = CustomChargingPeriodParser(JSON,
                                                                ChargingPeriod);

                return true;

            }
            catch (Exception e)
            {
                ChargingPeriod  = default;
                ErrorResponse   = "The given JSON representation of a charging period is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChargingPeriodSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingPeriod>?  CustomChargingPeriodSerializer   = null,
                              CustomJObjectSerializerDelegate<CDRDimension>?    CustomCDRDimensionSerializer     = null)
        {

            var json = JSONObject.Create(

                           new JProperty("start_date_time",   StartTimestamp.ToIso8601()),

                           new JProperty("dimensions",        new JArray(Dimensions.Select(cdrDimension => cdrDimension.ToJSON(CustomCDRDimensionSerializer))))

                       );

            return CustomChargingPeriodSerializer is not null
                       ? CustomChargingPeriodSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public ChargingPeriod Clone()

            => new (StartTimestamp,
                    Dimensions.Select(cdrDimension => cdrDimension.Clone()).ToArray());

        #endregion


        public ChargingPeriod AsChargingPeriod()

            => new (
                   StartTimestamp,
                   Dimensions
               );


        #region Operator overloading

        #region Operator == (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingPeriod ChargingPeriod1,
                                           ChargingPeriod ChargingPeriod2)

            => ChargingPeriod1.Equals(ChargingPeriod2);

        #endregion

        #region Operator != (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingPeriod ChargingPeriod1,
                                           ChargingPeriod ChargingPeriod2)

            => !ChargingPeriod1.Equals(ChargingPeriod2);

        #endregion

        #region Operator <  (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingPeriod ChargingPeriod1,
                                          ChargingPeriod ChargingPeriod2)

            => ChargingPeriod1.CompareTo(ChargingPeriod2) < 0;

        #endregion

        #region Operator <= (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingPeriod ChargingPeriod1,
                                           ChargingPeriod ChargingPeriod2)

            => ChargingPeriod1.CompareTo(ChargingPeriod2) <= 0;

        #endregion

        #region Operator >  (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingPeriod ChargingPeriod1,
                                          ChargingPeriod ChargingPeriod2)

            => ChargingPeriod1.CompareTo(ChargingPeriod2) > 0;

        #endregion

        #region Operator >= (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingPeriod ChargingPeriod1,
                                           ChargingPeriod ChargingPeriod2)

            => ChargingPeriod1.CompareTo(ChargingPeriod2) >= 0;

        #endregion

        #endregion

        #region IComparable<ChargingPeriod> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging periods.
        /// </summary>
        /// <param name="Object">A charging period to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingPeriod chargingPeriod
                   ? CompareTo(chargingPeriod)
                   : throw new ArgumentException("The given object is not a charging period!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingPeriod)

        /// <summary>
        /// Compares two charging periods.
        /// </summary>
        /// <param name="ChargingPeriod">A charging period to compare with.</param>
        public Int32 CompareTo(ChargingPeriod ChargingPeriod)
        {

            var c = StartTimestamp.CompareTo(ChargingPeriod.StartTimestamp);

            // Dimensions

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ChargingPeriod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging periods for equality.
        /// </summary>
        /// <param name="Object">A charging period to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingPeriod chargingPeriod &&
                   Equals(chargingPeriod);

        #endregion

        #region Equals(ChargingPeriod)

        /// <summary>
        /// Compares two charging periods for equality.
        /// </summary>
        /// <param name="ChargingPeriod">A charging period to compare with.</param>
        public Boolean Equals(ChargingPeriod ChargingPeriod)

            => StartTimestamp.ToIso8601().Equals(ChargingPeriod.StartTimestamp.ToIso8601()) &&

               Dimensions.Count.          Equals(ChargingPeriod.Dimensions.    Count) &&
               Dimensions.All(ChargingPeriod.Dimensions.Contains);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return StartTimestamp.GetHashCode() * 3 ^
                       Dimensions.    CalcHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   $"[{Id}] {StartTimestamp}: {Dimensions.Count} charging dimension(s): ",

                   Dimensions.AggregateWith(", ")

               );

        #endregion

    }

}
