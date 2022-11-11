/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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
    /// A charging period consists of a start timestamp and a list of
    /// possible values that influence this period, for example:
    /// Amount of energy charged this period, maximum current during
    /// this period etc.
    /// </summary>
    public readonly struct ChargingPeriod : IEquatable<ChargingPeriod>,
                                            IComparable<ChargingPeriod>,
                                            IComparable
    {

        #region Properties

        /// <summary>
        /// Start timestamp of the charging period.
        /// This period ends when a next period starts,
        /// the last period ends when the session ends.
        /// </summary>
        [Mandatory]
        public DateTime                   StartTimestamp     { get; }

        /// <summary>
        /// List of relevant values for this charging period.
        /// </summary>
        [Mandatory]
        public IEnumerable<CDRDimension>  Dimensions         { get; }

        /// <summary>
        /// Unique identifier of the tariff that is relevant for this charging period.
        /// If not provided, no tariff is relevant during this period.
        /// </summary>
        [Optional]
        public Tariff_Id?                 TariffId           { get;}

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A charging period consists of a start timestamp and a
        /// list of possible values that influence this period.
        /// </summary>
        /// <param name="StartTimestamp">Start timestamp of the charging period.</param>
        /// <param name="Dimensions">List of relevant values for this charging period.</param>
        /// <param name="TariffId">Unique identifier of the tariff that is relevant for this charging period.</param>
        public ChargingPeriod(DateTime                   StartTimestamp,
                              IEnumerable<CDRDimension>  Dimensions,
                              Tariff_Id?                 TariffId   = null)
        {

            if (!Dimensions.SafeAny())
                throw new ArgumentNullException(nameof(Dimensions), "The given enumeration of relevant values for this charging period must not be null or empty!");

            this.StartTimestamp  = StartTimestamp;
            this.Dimensions      = Dimensions?.Distinct();
            this.TariffId        = TariffId;

        }

        #endregion


        #region (static) Parse   (JSON, CustomChargingPeriodParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomChargingPeriodParser">A delegate to parse custom charging period JSON objects.</param>
        public static ChargingPeriod Parse(JObject                                      JSON,
                                           CustomJObjectParserDelegate<ChargingPeriod>  CustomChargingPeriodParser   = null)
        {

            if (TryParse(JSON,
                         out ChargingPeriod  chargingPeriod,
                         out String          ErrorResponse,
                         CustomChargingPeriodParser))
            {
                return chargingPeriod;
            }

            throw new ArgumentException("The given JSON representation of a charging period is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomChargingPeriodParser = null)

        /// <summary>
        /// Parse the given text representation of a charging period.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomChargingPeriodParser">A delegate to parse custom charging period JSON objects.</param>
        public static ChargingPeriod Parse(String                                       Text,
                                           CustomJObjectParserDelegate<ChargingPeriod>  CustomChargingPeriodParser   = null)
        {

            if (TryParse(Text,
                         out ChargingPeriod  chargingPeriod,
                         out String          ErrorResponse,
                         CustomChargingPeriodParser))
            {
                return chargingPeriod;
            }

            throw new ArgumentException("The given text representation of a charging period is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomChargingPeriodParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charging period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomChargingPeriodParser">A delegate to parse custom charging period JSON objects.</param>
        public static ChargingPeriod? TryParse(JObject                                         JSON,
                                                  CustomJObjectParserDelegate<ChargingPeriod>  CustomChargingPeriodParser   = null)
        {

            if (TryParse(JSON,
                         out ChargingPeriod  chargingPeriod,
                         out String          ErrorResponse,
                         CustomChargingPeriodParser))
            {
                return chargingPeriod;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomChargingPeriodParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charging period.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomChargingPeriodParser">A delegate to parse custom charging period JSON objects.</param>
        public static ChargingPeriod? TryParse(String                                       Text,
                                               CustomJObjectParserDelegate<ChargingPeriod>  CustomChargingPeriodParser   = null)
        {

            if (TryParse(Text,
                         out ChargingPeriod  chargingPeriod,
                         out String          ErrorResponse,
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
        public static Boolean TryParse(JObject             JSON,
                                       out ChargingPeriod  ChargingPeriod,
                                       out String          ErrorResponse)

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
        public static Boolean TryParse(JObject                                      JSON,
                                       out ChargingPeriod                           ChargingPeriod,
                                       out String                                   ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingPeriod>  CustomChargingPeriodParser   = null)
        {

            try
            {

                ChargingPeriod = default;

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

                #region Parse TariffId          [optional]

                if (JSON.ParseOptional("tariff_id",
                                       "tariff identification",
                                       Tariff_Id.TryParse,
                                       out Tariff_Id? TariffId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ChargingPeriod = new ChargingPeriod(StartTimestamp,
                                                    Dimensions,
                                                    TariffId);


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

        #region (static) TryParse(Text, out ChargingPeriod, out ErrorResponse, CustomChargingPeriodParser = null)

        /// <summary>
        /// Try to parse the given text representation of an chargingPeriod.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="ChargingPeriod">The parsed chargingPeriod.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingPeriodParser">A delegate to parse custom charging period JSON objects.</param>
        public static Boolean TryParse(String                                       Text,
                                       out ChargingPeriod                           ChargingPeriod,
                                       out String                                   ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingPeriod>  CustomChargingPeriodParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out ChargingPeriod,
                                out ErrorResponse,
                                CustomChargingPeriodParser);

            }
            catch (Exception e)
            {
                ChargingPeriod  = default;
                ErrorResponse   = "The given text representation of a charging period is invalid: " + e.Message;
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
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingPeriod>  CustomChargingPeriodSerializer   = null,
                              CustomJObjectSerializerDelegate<CDRDimension>    CustomCDRDimensionSerializer     = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("start_date_time",  StartTimestamp.ToIso8601()),
                           new JProperty("dimensions",       new JArray(Dimensions.Select(dimension => dimension.ToJSON(CustomCDRDimensionSerializer)))),

                           TariffId.HasValue
                               ? new JProperty("tariff_id",  TariffId.Value.ToString())
                               : null
                       );

            return CustomChargingPeriodSerializer is not null
                       ? CustomChargingPeriodSerializer(this, JSON)
                       : JSON;

        }

        #endregion


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

            => !(ChargingPeriod1 == ChargingPeriod2);

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

            => !(ChargingPeriod1 > ChargingPeriod2);

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

            => !(ChargingPeriod1 < ChargingPeriod2);

        #endregion

        #endregion

        #region IComparable<ChargingPeriod> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is ChargingPeriod chargingPeriod
                   ? CompareTo(chargingPeriod)
                   : throw new ArgumentException("The given object is not a charging period!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingPeriod)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod">An object to compare with.</param>
        public Int32 CompareTo(ChargingPeriod ChargingPeriod)
        {

            var c = StartTimestamp.CompareTo(ChargingPeriod.StartTimestamp);

            if (c == 0 && TariffId.HasValue && ChargingPeriod.TariffId.HasValue)
                c = TariffId.Value.CompareTo(ChargingPeriod.TariffId.HasValue);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ChargingPeriod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ChargingPeriod chargingPeriod &&
                   Equals(chargingPeriod);

        #endregion

        #region Equals(ChargingPeriod)

        /// <summary>
        /// Compares two charging periods for equality.
        /// </summary>
        /// <param name="ChargingPeriod">A charging period to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargingPeriod ChargingPeriod)

            => StartTimestamp.ToIso8601().Equals(ChargingPeriod.StartTimestamp.ToIso8601())         &&

               Dimensions.Count().        Equals(ChargingPeriod.Dimensions.    Count())             &&
               Dimensions.All(dimension =>       ChargingPeriod.Dimensions.    Contains(dimension)) &&

            ((!TariffId.HasValue && !ChargingPeriod.TariffId.HasValue) ||
              (TariffId.HasValue &&  ChargingPeriod.TariffId.HasValue && TariffId.Value.Equals(ChargingPeriod.TariffId.Value)));

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

                return StartTimestamp.GetHashCode() * 3 ^

                       Dimensions.Aggregate(0, (hashCode, dimension) => hashCode ^ dimension.GetHashCode()) ^

                       (TariffId.HasValue
                            ? TariffId.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(StartTimestamp,

                             " -> ",

                             Dimensions.OrderBy(dimension => dimension.Type).
                                        AggregateWith(", "),

                             TariffId.HasValue
                                 ? ", tariff: " + TariffId.ToString()
                                 : "");

        #endregion

    }

}
