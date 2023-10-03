/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Charging profile period structure defines a time period in a charging profile, as used in: ChargingProfile
    /// </summary>
    public readonly struct ChargingProfilePeriod : IEquatable<ChargingProfilePeriod>,
                                                   IComparable<ChargingProfilePeriod>,
                                                   IComparable
    {

        #region Properties

        /// <summary>
        /// Start of the period, in seconds from the start of profile. The value of StartPeriod also defines the stop time of the previous period.
        /// </summary>
        [Mandatory]
        public UInt32  StartPeriod    { get; }

        /// <summary>
        /// Charging rate limit during the profile period, in the applicable chargingRateUnit, for example in Amperes (A) or Watts (W). Accepts at most one digit fraction (e.g. 8.1).
        /// </summary>
        [Mandatory]
        public Single  Limit          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new charging profile period command.
        /// </summary>
        /// <param name="StartPeriod">Start of the period, in seconds from the start of profile. The value of StartPeriod also defines the stop time of the previous period.</param>
        /// <param name="Limit">Charging rate limit during the profile period, in the applicable chargingRateUnit, for example in Amperes (A) or Watts (W). Accepts at most one digit fraction (e.g. 8.1).</param>
        public ChargingProfilePeriod(UInt32  StartPeriod,
                                     Single  Limit)
        {

            this.StartPeriod  = StartPeriod;
            this.Limit        = Limit;

        }

        #endregion


        #region (static) Parse   (JSON, CustomChargingProfilePeriodParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging profile period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomChargingProfilePeriodParser">A delegate to parse custom charging profile period JSON objects.</param>
        public static ChargingProfilePeriod Parse(JObject                                             JSON,
                                                  CustomJObjectParserDelegate<ChargingProfilePeriod>  CustomChargingProfilePeriodParser   = null)
        {

            if (TryParse(JSON,
                         out ChargingProfilePeriod  chargingProfilePeriod,
                         out String                 ErrorResponse,
                         CustomChargingProfilePeriodParser))
            {
                return chargingProfilePeriod;
            }

            throw new ArgumentException("The given JSON representation of a charging profile period is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomChargingProfilePeriodParser = null)

        /// <summary>
        /// Parse the given text representation of a charging profile period.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomChargingProfilePeriodParser">A delegate to parse custom charging profile period JSON objects.</param>
        public static ChargingProfilePeriod Parse(String                                              Text,
                                                  CustomJObjectParserDelegate<ChargingProfilePeriod>  CustomChargingProfilePeriodParser   = null)
        {

            if (TryParse(Text,
                         out ChargingProfilePeriod  chargingProfilePeriod,
                         out String                 ErrorResponse,
                         CustomChargingProfilePeriodParser))
            {
                return chargingProfilePeriod;
            }

            throw new ArgumentException("The given text representation of a charging profile period is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomChargingProfilePeriodParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charging profile period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomChargingProfilePeriodParser">A delegate to parse custom charging profile period JSON objects.</param>
        public static ChargingProfilePeriod? TryParse(JObject                                             JSON,
                                                      CustomJObjectParserDelegate<ChargingProfilePeriod>  CustomChargingProfilePeriodParser   = null)
        {

            if (TryParse(JSON,
                         out ChargingProfilePeriod  chargingProfilePeriod,
                         out String                 ErrorResponse,
                         CustomChargingProfilePeriodParser))
            {
                return chargingProfilePeriod;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomChargingProfilePeriodParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charging profile period.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomChargingProfilePeriodParser">A delegate to parse custom charging profile period JSON objects.</param>
        public static ChargingProfilePeriod? TryParse(String                                              Text,
                                                      CustomJObjectParserDelegate<ChargingProfilePeriod>  CustomChargingProfilePeriodParser   = null)
        {

            if (TryParse(Text,
                         out ChargingProfilePeriod  chargingProfilePeriod,
                         out String                 ErrorResponse,
                         CustomChargingProfilePeriodParser))
            {
                return chargingProfilePeriod;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingProfilePeriod, out ErrorResponse, CustomChargingProfilePeriodParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging profile period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingProfilePeriod">The parsed charging profile period.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                    JSON,
                                       out ChargingProfilePeriod  ChargingProfilePeriod,
                                       out String                 ErrorResponse)

            => TryParse(JSON,
                        out ChargingProfilePeriod,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging profile period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingProfilePeriod">The parsed charging profile period.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingProfilePeriodParser">A delegate to parse custom charging profile period JSON objects.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       out ChargingProfilePeriod                           ChargingProfilePeriod,
                                       out String                                          ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingProfilePeriod>  CustomChargingProfilePeriodParser   = null)
        {

            try
            {

                ChargingProfilePeriod = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse StartPeriod   [mandatory]

                if (!JSON.ParseMandatory("start_period",
                                         "start period",
                                         out UInt32 StartPeriod,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Limit         [mandatory]

                if (!JSON.ParseMandatory("limit",
                                         "limit",
                                         out Single Limit,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ChargingProfilePeriod = new ChargingProfilePeriod(StartPeriod,
                                                                  Limit);

                if (CustomChargingProfilePeriodParser is not null)
                    ChargingProfilePeriod = CustomChargingProfilePeriodParser(JSON,
                                                                              ChargingProfilePeriod);

                return true;

            }
            catch (Exception e)
            {
                ChargingProfilePeriod  = default;
                ErrorResponse          = "The given JSON representation of a charging profile period is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out ChargingProfilePeriod, out ErrorResponse, CustomChargingProfilePeriodParser = null)

        /// <summary>
        /// Try to parse the given text representation of a charging profile period.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="ChargingProfilePeriod">The parsed chargingProfilePeriod.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingProfilePeriodParser">A delegate to parse custom charging profile period JSON objects.</param>
        public static Boolean TryParse(String                                              Text,
                                       out ChargingProfilePeriod                           ChargingProfilePeriod,
                                       out String                                          ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingProfilePeriod>  CustomChargingProfilePeriodParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out ChargingProfilePeriod,
                                out ErrorResponse,
                                CustomChargingProfilePeriodParser);

            }
            catch (Exception e)
            {
                ChargingProfilePeriod  = default;
                ErrorResponse          = "The given text representation of a charging profile period is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChargingProfilePeriodSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingProfilePeriodSerializer">A delegate to serialize custom charging profile period JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingProfilePeriod> CustomChargingProfilePeriodSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("start_period",  StartPeriod),
                           new JProperty("Limit",         Math.Round(Limit, 1))
                       );

            return CustomChargingProfilePeriodSerializer is not null
                       ? CustomChargingProfilePeriodSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingProfilePeriod1, ChargingProfilePeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfilePeriod1">A charging profile period.</param>
        /// <param name="ChargingProfilePeriod2">Another charging profile period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingProfilePeriod ChargingProfilePeriod1,
                                           ChargingProfilePeriod ChargingProfilePeriod2)

            => ChargingProfilePeriod1.Equals(ChargingProfilePeriod2);

        #endregion

        #region Operator != (ChargingProfilePeriod1, ChargingProfilePeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfilePeriod1">A charging profile period.</param>
        /// <param name="ChargingProfilePeriod2">Another charging profile period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingProfilePeriod ChargingProfilePeriod1,
                                           ChargingProfilePeriod ChargingProfilePeriod2)

            => !(ChargingProfilePeriod1 == ChargingProfilePeriod2);

        #endregion

        #region Operator <  (ChargingProfilePeriod1, ChargingProfilePeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfilePeriod1">A charging profile period.</param>
        /// <param name="ChargingProfilePeriod2">Another charging profile period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingProfilePeriod ChargingProfilePeriod1,
                                          ChargingProfilePeriod ChargingProfilePeriod2)

            => ChargingProfilePeriod1.CompareTo(ChargingProfilePeriod2) < 0;

        #endregion

        #region Operator <= (ChargingProfilePeriod1, ChargingProfilePeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfilePeriod1">A charging profile period.</param>
        /// <param name="ChargingProfilePeriod2">Another charging profile period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingProfilePeriod ChargingProfilePeriod1,
                                           ChargingProfilePeriod ChargingProfilePeriod2)

            => !(ChargingProfilePeriod1 > ChargingProfilePeriod2);

        #endregion

        #region Operator >  (ChargingProfilePeriod1, ChargingProfilePeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfilePeriod1">A charging profile period.</param>
        /// <param name="ChargingProfilePeriod2">Another charging profile period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingProfilePeriod ChargingProfilePeriod1,
                                          ChargingProfilePeriod ChargingProfilePeriod2)

            => ChargingProfilePeriod1.CompareTo(ChargingProfilePeriod2) > 0;

        #endregion

        #region Operator >= (ChargingProfilePeriod1, ChargingProfilePeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfilePeriod1">A charging profile period.</param>
        /// <param name="ChargingProfilePeriod2">Another charging profile period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingProfilePeriod ChargingProfilePeriod1,
                                           ChargingProfilePeriod ChargingProfilePeriod2)

            => !(ChargingProfilePeriod1 < ChargingProfilePeriod2);

        #endregion

        #endregion

        #region IComparable<ChargingProfilePeriod> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is ChargingProfilePeriod chargingProfilePeriod
                   ? CompareTo(chargingProfilePeriod)
                   : throw new ArgumentException("The given object is not a charging profile period!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingProfilePeriod)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfilePeriod">An object to compare with.</param>
        public Int32 CompareTo(ChargingProfilePeriod ChargingProfilePeriod)
        {

            var c = StartPeriod.CompareTo(ChargingProfilePeriod.StartPeriod);

            if (c == 0)
                c = Limit.CompareTo(ChargingProfilePeriod.Limit);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ChargingProfilePeriod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ChargingProfilePeriod chargingProfilePeriod &&
                   Equals(chargingProfilePeriod);

        #endregion

        #region Equals(ChargingProfilePeriod)

        /// <summary>
        /// Compares two charging profile periods for equality.
        /// </summary>
        /// <param name="ChargingProfilePeriod">A charging profile period to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargingProfilePeriod ChargingProfilePeriod)

            => StartPeriod.Equals(ChargingProfilePeriod.StartPeriod) &&
               Limit.      Equals(ChargingProfilePeriod.Limit);

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

                return StartPeriod.GetHashCode() * 3 ^
                       Limit.      GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(StartPeriod, ", ",
                             Limit);

        #endregion

    }

}
