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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A meter sample.
    /// </summary>
    public class MeterSample : IEquatable<MeterSample>,
                               IComparable<MeterSample>,
                               IComparable
    {

        #region Properties

        /// <summary>
        /// The time at which all the measurements were taken.
        /// </summary>
        [Mandatory]
        public DateTime                   Timestamp            { get; }

        /// <summary>
        /// The UID of the EVSE to which these measurements apply, if any.
        /// </summary>
        [Optional]
        public EVSE_UId?                  EVSEUId              { get; }

        /// <summary>
        /// The ID of the Charging Session to which these measurements apply, if any.
        /// </summary>
        [Optional]
        public Session_Id?                SessionId            { get; }

        /// <summary>
        /// The Charging Profile for which these measurements are sent.
        /// </summary>
        [Optional]
        public ChargingProfile_Id?        ChargingProfileId    { get; }

        /// <summary>
        /// The enumeration of meter readings.
        /// </summary>
        [Mandatory]
        public IEnumerable<MeterReading>  Readings             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new meter sample.
        /// </summary>
        /// <param name="Timestamp">The time at which all the measurements were taken.</param>
        /// <param name="Readings">An enumeration of meter readings.</param>
        /// <param name="EVSEUId">The UID of the EVSE to which these measurements apply, if any.</param>
        /// <param name="SessionId">The ID of the Charging Session to which these measurements apply, if any.</param>
        /// <param name="ChargingProfileId">The Charging Profile for which these measurements are sent.</param>
        public MeterSample(DateTime                   Timestamp,
                           IEnumerable<MeterReading>  Readings,
                           EVSE_UId?                  EVSEUId             = null,
                           Session_Id?                SessionId           = null,
                           ChargingProfile_Id?        ChargingProfileId   = null)
        {

            if (!Readings.Any())
                throw new ArgumentNullException(nameof(Readings), "The given enumeration of meter readings must not be empty!");

            this.Timestamp          = Timestamp;
            this.Readings           = Readings.Distinct();
            this.EVSEUId            = EVSEUId;
            this.SessionId          = SessionId;
            this.ChargingProfileId  = ChargingProfileId;

            unchecked
            {

                hashCode = this.Timestamp.         GetHashCode()       * 11 ^
                           this.Readings.          CalcHashCode()      *  7 ^
                          (this.EVSEUId?.          GetHashCode() ?? 0) *  5 ^
                          (this.SessionId?.        GetHashCode() ?? 0) *  3 ^
                           this.ChargingProfileId?.GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomMeterSampleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a meter sample.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomMeterSampleParser">A delegate to parse custom meter sample JSON objects.</param>
        public static MeterSample Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<MeterSample>?  CustomMeterSampleParser   = null)
        {

            if (TryParse(JSON,
                         out var meterSample,
                         out var errorResponse,
                         CustomMeterSampleParser))
            {
                return meterSample!;
            }

            throw new ArgumentException("The given JSON representation of a meter sample is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out MeterSample, out ErrorResponse, CustomMeterSampleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a meter sample.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="MeterSample">The parsed meter sample.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out MeterSample?  MeterSample,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out MeterSample,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a meter sample.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="MeterSample">The parsed meter sample.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomMeterSampleParser">A delegate to parse custom meter sample JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out MeterSample?      MeterSample,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<MeterSample>?  CustomMeterSampleParser   = null)
        {

            try
            {

                MeterSample = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Timestamp            [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "measurement(s) timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEUId              [optional]

                if (JSON.ParseOptional("evse_id",
                                       "EVSE identification",
                                       EVSE_UId.TryParse,
                                       out EVSE_UId? EVSEUId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse SessionId            [optional]

                if (JSON.ParseOptional("session_id",
                                       "session identification",
                                       Session_Id.TryParse,
                                       out Session_Id? SessionId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ChargingProfileId    [optional]

                if (JSON.ParseOptional("charging_profile_id",
                                       "charging profile identification",
                                       ChargingProfile_Id.TryParse,
                                       out ChargingProfile_Id? ChargingProfileId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Readings             [optional]

                if (!JSON.ParseMandatoryJSON("readings",
                                             "meter readings",
                                             MeterReading.TryParse,
                                             out IEnumerable<MeterReading> Readings,
                                             out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                MeterSample = new MeterSample(
                                  Timestamp,
                                  Readings,
                                  EVSEUId,
                                  SessionId,
                                  ChargingProfileId
                              );

                if (CustomMeterSampleParser is not null)
                    MeterSample = CustomMeterSampleParser(JSON,
                                                          MeterSample);

                return true;

            }
            catch (Exception e)
            {
                MeterSample    = default;
                ErrorResponse  = "The given JSON representation of a meter sample is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomMeterSampleSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMeterSampleSerializer">A delegate to serialize custom meter sample JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MeterSample>?   CustomMeterSampleSerializer    = null,
                              CustomJObjectSerializerDelegate<MeterReading>?  CustomMeterReadingSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("timestamp",             Timestamp),

                           EVSEUId.          HasValue
                               ? new JProperty("evse_id",               EVSEUId.          ToString())
                               : null,

                           SessionId.        HasValue
                               ? new JProperty("session_id",            SessionId.        ToString())
                               : null,

                           ChargingProfileId.HasValue
                               ? new JProperty("charging_profile_id",   ChargingProfileId.ToString())
                               : null,

                                 new JProperty("readings",              new JArray(Readings.Select(meterSample => meterSample.ToJSON(CustomMeterReadingSerializer))))

                       );

            return CustomMeterSampleSerializer is not null
                       ? CustomMeterSampleSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this meter sample.
        /// </summary>
        public MeterSample Clone()

            => new (

                   Timestamp,
                   Readings.Select(meterSample => meterSample.Clone()),
                   EVSEUId?.          Clone(),
                   SessionId?.        Clone(),
                   ChargingProfileId?.Clone()

               );

        #endregion


        #region Operator overloading

        #region Operator == (MeterSample1, MeterSample2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterSample1">A meter sample.</param>
        /// <param name="MeterSample2">Another meter sample.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MeterSample MeterSample1,
                                           MeterSample MeterSample2)

            => MeterSample1.Equals(MeterSample2);

        #endregion

        #region Operator != (MeterSample1, MeterSample2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterSample1">A meter sample.</param>
        /// <param name="MeterSample2">Another meter sample.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MeterSample MeterSample1,
                                           MeterSample MeterSample2)

            => !(MeterSample1 == MeterSample2);

        #endregion

        #region Operator <  (MeterSample1, MeterSample2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterSample1">A meter sample.</param>
        /// <param name="MeterSample2">Another meter sample.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (MeterSample MeterSample1,
                                          MeterSample MeterSample2)

            => MeterSample1.CompareTo(MeterSample2) < 0;

        #endregion

        #region Operator <= (MeterSample1, MeterSample2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterSample1">A meter sample.</param>
        /// <param name="MeterSample2">Another meter sample.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (MeterSample MeterSample1,
                                           MeterSample MeterSample2)

            => !(MeterSample1 > MeterSample2);

        #endregion

        #region Operator >  (MeterSample1, MeterSample2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterSample1">A meter sample.</param>
        /// <param name="MeterSample2">Another meter sample.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (MeterSample MeterSample1,
                                          MeterSample MeterSample2)

            => MeterSample1.CompareTo(MeterSample2) > 0;

        #endregion

        #region Operator >= (MeterSample1, MeterSample2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterSample1">A meter sample.</param>
        /// <param name="MeterSample2">Another meter sample.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (MeterSample MeterSample1,
                                           MeterSample MeterSample2)

            => !(MeterSample1 < MeterSample2);

        #endregion

        #endregion

        #region IComparable<MeterSample> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two meter samples.
        /// </summary>
        /// <param name="Object">A meter sample to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is MeterSample meterSample
                   ? CompareTo(meterSample)
                   : throw new ArgumentException("The given object is not a meter sample!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MeterSample)

        /// <summary>
        /// Compares two meter samples.
        /// </summary>
        /// <param name="MeterSample">A meter sample to compare with.</param>
        public Int32 CompareTo(MeterSample? MeterSample)
        {

            if (MeterSample is null)
                throw new ArgumentNullException(nameof(MeterSample), "The given meter sample must not be null!");

            var c = Timestamp.ToIso8601().CompareTo(MeterSample.Timestamp.ToIso8601());
            if (c != 0)
                return c;

            c = EVSEUId?.                 CompareTo(MeterSample.EVSEUId)           ?? (!MeterSample.EVSEUId.          HasValue ? 0 : -1);
            if (c != 0)
                return c;

            c = SessionId?.               CompareTo(MeterSample.SessionId)         ?? (!MeterSample.SessionId.        HasValue ? 0 : -1);
            if (c != 0)
                return c;

            c = ChargingProfileId?.       CompareTo(MeterSample.ChargingProfileId) ?? (!MeterSample.ChargingProfileId.HasValue ? 0 : -1);
            if (c != 0)
                return c;

            var readings1 =             Readings.OrderBy(r => r.Measurand).ThenBy(r => r.Unit).ToArray();
            var readings2 = MeterSample.Readings.OrderBy(r => r.Measurand).ThenBy(r => r.Unit).ToArray();

            for (var i = 0; i < Math.Min(readings1.Length, readings2.Length); i++)
            {
                c = readings1[i].CompareTo(readings2[i]);
                if (c != 0)
                    return c;
            }

            return readings1.Length.CompareTo(readings2.Length);

        }

        #endregion

        #endregion

        #region IEquatable<MeterSample> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two meter samples for equality.
        /// </summary>
        /// <param name="Object">A meter sample to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MeterSample meterSample &&
                   Equals(meterSample);

        #endregion

        #region Equals(MeterSample)

        /// <summary>
        /// Compares two meter samples for equality.
        /// </summary>
        /// <param name="MeterSample">A meter sample to compare with.</param>
        public Boolean Equals(MeterSample? MeterSample)

            => MeterSample is not null &&

               Timestamp.         Equals(MeterSample.Timestamp) &&

              (EVSEUId?.          Equals(MeterSample.EVSEUId)           ?? !MeterSample.EVSEUId.HasValue)           &&
              (SessionId?.        Equals(MeterSample.SessionId)         ?? !MeterSample.SessionId.HasValue)         &&
              (ChargingProfileId?.Equals(MeterSample.ChargingProfileId) ?? !MeterSample.ChargingProfileId.HasValue) &&

               Readings.OrderBy(r => r.Measurand).ThenBy(r => r.Unit).SequenceEqual(MeterSample.Readings.OrderBy(r => r.Measurand).ThenBy(r => r.Unit));

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

                   $"{Timestamp}: {Readings.Count()} meter reading(s)",

                   EVSEUId.          HasValue
                       ? $", for EVSE '{EVSEUId}'"
                       : "",

                   SessionId.        HasValue
                       ? $", for session '{SessionId}'"
                       : "",

                   ChargingProfileId.HasValue
                       ? $", for charging profile '{ChargingProfileId}'"
                       : ""

               );

        #endregion

    }

}
