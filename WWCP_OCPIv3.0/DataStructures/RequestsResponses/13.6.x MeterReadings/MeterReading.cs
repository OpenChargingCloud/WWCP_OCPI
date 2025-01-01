/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A meter reading.
    /// </summary>
    public class MeterReading : IEquatable<MeterReading>,
                                IComparable<MeterReading>,
                                IComparable
    {

        #region Properties

        /// <summary>
        /// The measured value, in the unit given by the unit field.
        /// </summary>
        [Mandatory]
        public Decimal            Value             { get; }

        /// <summary>
        /// The quantity that was measured.
        /// </summary>
        [Mandatory]
        public Measurand          Measurand         { get; }

        /// <summary>
        /// The unit in which this meter reading is given.
        /// </summary>
        [Mandatory]
        public MeterReadingUnit   Unit              { get; }

        /// <summary>
        /// The level of grouping for which the meter reading is given.
        /// </summary>
        [Mandatory]
        public ComponentLevel     ComponentLevel    { get; }

        /// <summary>
        /// At which point in the energy flow relative to the component the meter reading was obtained.
        /// </summary>
        [Mandatory]
        public ComponentLocation  Location          { get; }

        /// <summary>
        /// Which phase the reading applies to. When this field is not given, the measured value is interpreted as an overall value.
        /// </summary>
        [Optional]
        public Phase?             Phase             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new meter reading.
        /// </summary>
        /// <param name="Value">The measured value, in the unit given by the unit field.</param>
        /// <param name="Measurand">The quantity that was measured.</param>
        /// <param name="Unit">The unit in which this meter reading is given.</param>
        /// <param name="ComponentLevel">The level of grouping for which the meter reading is given.</param>
        /// <param name="Location">At which point in the energy flow relative to the component the meter reading was obtained.</param>
        /// <param name="Phase">Which phase the reading applies to. When this field is not given, the measured value is interpreted as an overall value.</param>
        public MeterReading(Decimal            Value,
                            Measurand          Measurand,
                            MeterReadingUnit   Unit,
                            ComponentLevel     ComponentLevel,
                            ComponentLocation  Location,
                            Phase?             Phase   = null)
        {

            this.Value           = Value;
            this.Measurand       = Measurand;
            this.Unit            = Unit;
            this.ComponentLevel  = ComponentLevel;
            this.Location        = Location;
            this.Phase           = Phase;

            unchecked
            {

                hashCode = this.Value.         GetHashCode() * 13 ^
                           this.Measurand.     GetHashCode() * 11 ^
                           this.Unit.          GetHashCode() *  7 ^
                           this.ComponentLevel.GetHashCode() *  5 ^
                           this.Location.      GetHashCode() *  3 ^
                           this.Phase?.        GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomMeterReadingParser = null)

        /// <summary>
        /// Parse the given JSON representation of a meter reading.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomMeterReadingParser">A delegate to parse custom meter reading JSON objects.</param>
        public static MeterReading Parse(JObject                                     JSON,
                                         CustomJObjectParserDelegate<MeterReading>?  CustomMeterReadingParser   = null)
        {

            if (TryParse(JSON,
                         out var meterReading,
                         out var errorResponse,
                         CustomMeterReadingParser))
            {
                return meterReading!;
            }

            throw new ArgumentException("The given JSON representation of a meter reading is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out MeterReading, out ErrorResponse, CustomMeterReadingParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a meter reading.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="MeterReading">The parsed meter reading.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out MeterReading?  MeterReading,
                                       [NotNullWhen(false)] out String?        ErrorResponse)

            => TryParse(JSON,
                        out MeterReading,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a meter reading.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="MeterReading">The parsed meter reading.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomMeterReadingParser">A delegate to parse custom meter reading JSON objects.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out MeterReading?      MeterReading,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomJObjectParserDelegate<MeterReading>?  CustomMeterReadingParser   = null)
        {

            try
            {

                MeterReading = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Value             [mandatory]

                if (!JSON.ParseMandatory("value",
                                         "measured value",
                                         out Decimal Value,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Measurand         [mandatory]

                if (!JSON.ParseMandatory("measurand",
                                         "measurand",
                                         OCPIv3_0.Measurand.TryParse,
                                         out Measurand Measurand,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Unit              [mandatory]

                if (!JSON.ParseMandatory("unit",
                                         "meter reading unit",
                                         MeterReadingUnit.TryParse,
                                         out MeterReadingUnit Unit,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ComponentLevel    [mandatory]

                if (!JSON.ParseMandatory("component_level",
                                         "component level",
                                         OCPIv3_0.ComponentLevel.TryParse,
                                         out ComponentLevel ComponentLevel,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Location          [mandatory]

                if (!JSON.ParseMandatory("location",
                                         "component location",
                                         ComponentLocation.TryParse,
                                         out ComponentLocation Location,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Phase             [optional]

                if (JSON.ParseOptional("phase",
                                       "phase",
                                       OCPIv3_0.Phase.TryParse,
                                       out Phase? Phase,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                MeterReading = new MeterReading(
                                   Value,
                                   Measurand,
                                   Unit,
                                   ComponentLevel,
                                   Location,
                                   Phase
                               );

                if (CustomMeterReadingParser is not null)
                    MeterReading = CustomMeterReadingParser(JSON,
                                                            MeterReading);

                return true;

            }
            catch (Exception e)
            {
                MeterReading   = default;
                ErrorResponse  = "The given JSON representation of a meter reading is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomMeterReadingSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMeterReadingSerializer">A delegate to serialize custom meter reading JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MeterReading>? CustomMeterReadingSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("value",             Value),
                                 new JProperty("measurand",         Measurand.     ToString()),
                                 new JProperty("unit",              Unit.          ToString()),
                                 new JProperty("component_level",   ComponentLevel.ToString()),
                                 new JProperty("location",          Location.      ToString()),

                           Phase.HasValue
                               ? new JProperty("phase",             Phase.         ToString())
                               : null

                       );

            return CustomMeterReadingSerializer is not null
                       ? CustomMeterReadingSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this meter reading.
        /// </summary>
        public MeterReading Clone()

            => new(

                   Value,
                   Measurand.     Clone(),
                   Unit.          Clone(),
                   ComponentLevel.Clone(),
                   Location.      Clone(),
                   Phase?.        Clone()

               );

        #endregion


        #region Operator overloading

        #region Operator == (MeterReading1, MeterReading2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterReading1">A meter reading.</param>
        /// <param name="MeterReading2">Another meter reading.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MeterReading MeterReading1,
                                           MeterReading MeterReading2)

            => MeterReading1.Equals(MeterReading2);

        #endregion

        #region Operator != (MeterReading1, MeterReading2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterReading1">A meter reading.</param>
        /// <param name="MeterReading2">Another meter reading.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MeterReading MeterReading1,
                                           MeterReading MeterReading2)

            => !(MeterReading1 == MeterReading2);

        #endregion

        #region Operator <  (MeterReading1, MeterReading2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterReading1">A meter reading.</param>
        /// <param name="MeterReading2">Another meter reading.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (MeterReading MeterReading1,
                                          MeterReading MeterReading2)

            => MeterReading1.CompareTo(MeterReading2) < 0;

        #endregion

        #region Operator <= (MeterReading1, MeterReading2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterReading1">A meter reading.</param>
        /// <param name="MeterReading2">Another meter reading.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (MeterReading MeterReading1,
                                           MeterReading MeterReading2)

            => !(MeterReading1 > MeterReading2);

        #endregion

        #region Operator >  (MeterReading1, MeterReading2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterReading1">A meter reading.</param>
        /// <param name="MeterReading2">Another meter reading.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (MeterReading MeterReading1,
                                          MeterReading MeterReading2)

            => MeterReading1.CompareTo(MeterReading2) > 0;

        #endregion

        #region Operator >= (MeterReading1, MeterReading2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterReading1">A meter reading.</param>
        /// <param name="MeterReading2">Another meter reading.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (MeterReading MeterReading1,
                                           MeterReading MeterReading2)

            => !(MeterReading1 < MeterReading2);

        #endregion

        #endregion

        #region IComparable<MeterReading> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two meter readings.
        /// </summary>
        /// <param name="Object">A meter reading to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is MeterReading meterReading
                   ? CompareTo(meterReading)
                   : throw new ArgumentException("The given object is not a meter reading!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MeterReading)

        /// <summary>
        /// Compares two meter readings.
        /// </summary>
        /// <param name="MeterReading">A meter reading to compare with.</param>
        public Int32 CompareTo(MeterReading? MeterReading)
        {

            if (MeterReading is null)
                throw new ArgumentNullException(nameof(MeterReading), "The given meter reading must not be null!");

            var c = Value.     CompareTo(MeterReading.Value);
            if (c != 0)
                return c;

            c = Measurand.     CompareTo(MeterReading.Measurand);
            if (c != 0)
                return c;

            c = Unit.          CompareTo(MeterReading.Unit);
            if (c != 0)
                return c;

            c = ComponentLevel.CompareTo(MeterReading.ComponentLevel);
            if (c != 0)
                return c;

            c = Location.      CompareTo(MeterReading.Location);
            if (c != 0)
                return c;

            if (Phase.HasValue && MeterReading.Phase.HasValue)
                return Phase.Value.CompareTo(MeterReading.Phase.Value);

            else if (Phase.HasValue)
                return 1;

            else if (MeterReading.Phase.HasValue)
                return -1;

            return 0;

        }

        #endregion

        #endregion

        #region IEquatable<MeterReading> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two meter readings for equality.
        /// </summary>
        /// <param name="Object">A meter reading to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MeterReading meterReading &&
                   Equals(meterReading);

        #endregion

        #region Equals(MeterReading)

        /// <summary>
        /// Compares two meter readings for equality.
        /// </summary>
        /// <param name="MeterReading">A meter reading to compare with.</param>
        public Boolean Equals(MeterReading? MeterReading)

            => MeterReading is not null &&

               Value.         Equals(MeterReading.Value)          &&
               Measurand.     Equals(MeterReading.Measurand)      &&
               Unit.          Equals(MeterReading.Unit)           &&
               ComponentLevel.Equals(MeterReading.ComponentLevel) &&
               Location.      Equals(MeterReading.Location)       &&
               Phase.         Equals(MeterReading.Phase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   $"{Value} {Unit} of {Measurand}",

                   Phase.HasValue
                       ? $" in {Phase} phase"
                       : "",

                   $" at {Location}/{ComponentLevel}"

               );

        #endregion

    }

}
