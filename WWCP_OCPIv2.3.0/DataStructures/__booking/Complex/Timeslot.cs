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
    /// A timeslot.
    /// </summary>
    public class Timeslot : IEquatable<Timeslot>,
                            IComparable<Timeslot>,
                            IComparable
    {

        #region Properties

        /// <summary>
        /// Start time of this timeslot.
        /// </summary>
        [Mandatory]
        public DateTimeOffset  Start                 { get; }

        /// <summary>
        /// End time of this timeslot.
        /// </summary>
        [Mandatory]
        public DateTimeOffset  End                   { get; }

        /// <summary>
        /// The optional minimum power guaranteed during this timeslot.
        /// </summary>
        [Optional]
        public Watt?           MinPower              { get; }

        /// <summary>
        /// The optional maximum power guaranteed during this timeslot.
        /// Can be requested lower.
        /// </summary>
        [Optional]
        public Watt?           MaxPower              { get; }

        /// <summary>
        /// Optional indication wheather green energy is available during this timeslot.
        /// </summary>
        [Optional]
        public Boolean?        GreenEnergySupport    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Start">The start time of this timeslot.</param>
        /// <param name="End">The end time of this timeslot.</param>
        /// <param name="MinPower">An optional minimum power guaranteed during this timeslot.</param>
        /// <param name="MaxPower">An optional maximum power guaranteed during this timeslot. Can be requested lower.</param>
        /// <param name="GreenEnergySupport">Optional indication wheather green energy is available during this timeslot.</param>
        public Timeslot(DateTimeOffset  Start,
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


        #region (static) Parse   (JSON, CustomTimeslotParser = null)

        /// <summary>
        /// Parse the given JSON representation of a timeslot.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTimeslotParser">A delegate to parse custom timeslot JSON objects.</param>
        public static Timeslot Parse(JObject                                 JSON,
                                     CustomJObjectParserDelegate<Timeslot>?  CustomTimeslotParser   = null)
        {

            if (TryParse(JSON,
                         out var timeslot,
                         out var errorResponse,
                         CustomTimeslotParser))
            {
                return timeslot;
            }

            throw new ArgumentException("The given JSON representation of a timeslot is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Timeslot, out ErrorResponse, CustomTimeslotParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a timeslot.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Timeslot">The parsed timeslot.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out Timeslot?  Timeslot,
                                       [NotNullWhen(false)] out String?    ErrorResponse)

            => TryParse(JSON,
                        out Timeslot,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a timeslot.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Timeslot">The parsed timeslot.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTimeslotParser">A delegate to parse custom timeslot JSON objects.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out Timeslot?      Timeslot,
                                       [NotNullWhen(false)] out String?        ErrorResponse,
                                       CustomJObjectParserDelegate<Timeslot>?  CustomTimeslotParser   = null)
        {

            try
            {

                Timeslot = null;

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



                Timeslot = new Timeslot(
                               start,
                               end,
                               minPower,
                               maxPower,
                               greenEnergySupport
                           );


                if (CustomTimeslotParser is not null)
                    Timeslot = CustomTimeslotParser(JSON,
                                                    Timeslot);

                return true;

            }
            catch (Exception e)
            {
                Timeslot       = default;
                ErrorResponse  = "The given JSON representation of a timeslot is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTimeslotSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTimeslotSerializer">A delegate to serialize custom timeslot JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Timeslot>? CustomTimeslotSerializer = null)
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

            return CustomTimeslotSerializer is not null
                       ? CustomTimeslotSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this timeslot.
        /// </summary>
        public Timeslot Clone()

            => new (
                   Start,
                   End,
                   MinPower?.Clone(),
                   MaxPower?.Clone(),
                   GreenEnergySupport
               );

        #endregion


        #region Operator overloading

        #region Operator == (Timeslot1, Timeslot2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timeslot1">A timeslot.</param>
        /// <param name="Timeslot2">Another timeslot.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Timeslot? Timeslot1,
                                           Timeslot? Timeslot2)
        {

            if (Object.ReferenceEquals(Timeslot1, Timeslot2))
                return true;

            if (Timeslot1 is null || Timeslot2 is null)
                return false;

            return Timeslot1.Equals(Timeslot2);

        }

        #endregion

        #region Operator != (Timeslot1, Timeslot2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timeslot1">A timeslot.</param>
        /// <param name="Timeslot2">Another timeslot.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Timeslot? Timeslot1,
                                           Timeslot? Timeslot2)

            => !(Timeslot1 == Timeslot2);

        #endregion

        #region Operator <  (Timeslot1, Timeslot2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timeslot1">A timeslot.</param>
        /// <param name="Timeslot2">Another timeslot.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Timeslot? Timeslot1,
                                          Timeslot? Timeslot2)

            => Timeslot1 is null
                   ? throw new ArgumentNullException(nameof(Timeslot1), "The given timeslot must not be null!")
                   : Timeslot1.CompareTo(Timeslot2) < 0;

        #endregion

        #region Operator <= (Timeslot1, Timeslot2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timeslot1">A timeslot.</param>
        /// <param name="Timeslot2">Another timeslot.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Timeslot? Timeslot1,
                                           Timeslot? Timeslot2)

            => !(Timeslot1 > Timeslot2);

        #endregion

        #region Operator >  (Timeslot1, Timeslot2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timeslot1">A timeslot.</param>
        /// <param name="Timeslot2">Another timeslot.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Timeslot? Timeslot1,
                                          Timeslot? Timeslot2)

            => Timeslot1 is null
                   ? throw new ArgumentNullException(nameof(Timeslot1), "The given timeslot must not be null!")
                   : Timeslot1.CompareTo(Timeslot2) > 0;

        #endregion

        #region Operator >= (Timeslot1, Timeslot2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timeslot1">A timeslot.</param>
        /// <param name="Timeslot2">Another timeslot.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Timeslot? Timeslot1,
                                           Timeslot? Timeslot2)

            => !(Timeslot1 < Timeslot2);

        #endregion

        #endregion

        #region IComparable<Timeslot> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two timeslots.
        /// </summary>
        /// <param name="Object">A timeslot to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Timeslot timeslot
                   ? CompareTo(timeslot)
                   : throw new ArgumentException("The given object is not a timeslot!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Timeslot)

        /// <summary>s
        /// Compares two timeslots.
        /// </summary>
        /// <param name="Object">A timeslot to compare with.</param>
        public Int32 CompareTo(Timeslot? Timeslot)
        {

            if (Timeslot is null)
                throw new ArgumentNullException(nameof(Timeslot), "The given timeslot must not be null!");

            var c = Start. CompareTo(Timeslot.Start);

            if (c == 0)
                c = End.     CompareTo(Timeslot.End);

            if (c == 0 && MinPower.          HasValue && Timeslot.MinPower.          HasValue)
                c = MinPower.          Value.CompareTo(Timeslot.MinPower.          Value);

            if (c == 0 && MaxPower.          HasValue && Timeslot.MaxPower.          HasValue)
                c = MaxPower.          Value.CompareTo(Timeslot.MaxPower.          Value);

            if (c == 0 && GreenEnergySupport.HasValue && Timeslot.GreenEnergySupport.HasValue)
                c = GreenEnergySupport.Value.CompareTo(Timeslot.GreenEnergySupport.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Timeslot> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two timeslots for equality.
        /// </summary>
        /// <param name="Object">A timeslot to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Timeslot timeslot &&
                   Equals(timeslot);

        #endregion

        #region Equals(Timeslot)

        /// <summary>
        /// Compares two timeslots for equality.
        /// </summary>
        /// <param name="Timeslot">A timeslot to compare with.</param>
        public Boolean Equals(Timeslot? Timeslot)

            => Timeslot is not null &&

               Start.Equals(Timeslot.Start) &&
               End.  Equals(Timeslot.End)   &&

            ((!MinPower.          HasValue && !Timeslot.MinPower.          HasValue) ||
              (MinPower.          HasValue &&  Timeslot.MinPower.          HasValue && MinPower.          Value.Equals(Timeslot.MinPower.          Value))) &&

            ((!MaxPower.          HasValue && !Timeslot.MaxPower.          HasValue) ||
              (MaxPower.          HasValue &&  Timeslot.MaxPower.          HasValue && MaxPower.          Value.Equals(Timeslot.MaxPower.          Value))) &&

            ((!GreenEnergySupport.HasValue && !Timeslot.GreenEnergySupport.HasValue)  ||
              (GreenEnergySupport.HasValue &&  Timeslot.GreenEnergySupport.HasValue && GreenEnergySupport.Value.Equals(Timeslot.GreenEnergySupport.Value)));

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
