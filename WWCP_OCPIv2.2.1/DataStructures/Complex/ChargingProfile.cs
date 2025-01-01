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

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// The charging profile defines an enumeration of charging periods.
    /// </summary>
    public class ChargingProfile : IEquatable<ChargingProfile>,
                                   IComparable<ChargingProfile>,
                                   IComparable
    {

        #region Properties

        /// <summary>
        /// The unit of measure.
        /// </summary>
        [Mandatory]
        public ChargingRateUnits                   ChargingRateUnit           { get; }

        /// <summary>
        /// Starting point of an absolute profile. If absent the profile will be relative to start of charging.
        /// </summary>
        [Optional]
        public DateTime?                           Start                      { get; }

        /// <summary>
        /// Duration of the charging profile in seconds. If the duration is left empty, the last period will
        /// continue indefinitely or until end of the transaction in case startProfile is absent.
        /// </summary>
        [Optional]
        public TimeSpan?                           Duration                   { get; }

        /// <summary>
        /// Duration of the charging profile in seconds. If the duration is left empty, the last period will
        /// continue indefinitely or until end of the transaction in case startProfile is absent.
        /// </summary>
        [Optional]
        public Decimal?                            MinChargingRate            { get; }

        /// <summary>
        /// Duration of the charging profile in seconds. If the duration is left empty, the last period will
        /// continue indefinitely or until end of the transaction in case startProfile is absent.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingProfilePeriod>  ChargingProfilePeriods     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new charging profile.
        /// </summary>
        /// <param name="ChargingRateUnit">The unit of measure.</param>
        /// <param name="Start">Starting point of an absolute profile. If absent the profile will be relative to start of charging.</param>
        /// <param name="Duration">Duration of the charging profile in seconds. If the duration is left empty, the last period will continue indefinitely or until end of the transaction in case startProfile is absent.</param>
        /// <param name="MinChargingRate">Duration of the charging profile in seconds. If the duration is left empty, the last period will continue indefinitely or until end of the transaction in case startProfile is absent.</param>
        /// <param name="ChargingProfilePeriods">Duration of the charging profile in seconds. If the duration is left empty, the last period will continue indefinitely or until end of the transaction in case startProfile is absent.</param>
        public ChargingProfile(ChargingRateUnits                    ChargingRateUnit,
                               DateTime?                            Start                    = null,
                               TimeSpan?                            Duration                 = null,
                               Decimal?                             MinChargingRate          = null,
                               IEnumerable<ChargingProfilePeriod>?  ChargingProfilePeriods   = null)
        {

            this.ChargingRateUnit        = ChargingRateUnit;
            this.Start                   = Start;
            this.Duration                = Duration;
            this.MinChargingRate         = MinChargingRate;
            this.ChargingProfilePeriods  = ChargingProfilePeriods?.Distinct() ?? [];

            unchecked
            {

                hashCode = this.ChargingRateUnit.      GetHashCode()       * 11 ^
                          (this.Start?.                GetHashCode() ?? 0) *  7 ^
                          (this.Duration?.             GetHashCode() ?? 0) *  5 ^
                          (this.MinChargingRate?.      GetHashCode() ?? 0) *  3 ^
                           this.ChargingProfilePeriods.CalcHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomChargingProfileParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging profile.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomChargingProfileParser">A delegate to parse custom charging profile JSON objects.</param>
        public static ChargingProfile Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<ChargingProfile>?  CustomChargingProfileParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingProfile,
                         out var errorResponse,
                         CustomChargingProfileParser))
            {
                return chargingProfile;
            }

            throw new ArgumentException("The given JSON representation of a charging profile is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingProfile, out ErrorResponse, CustomChargingProfileParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging profile.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingProfile">The parsed charging profile.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out ChargingProfile?  ChargingProfile,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out ChargingProfile,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging profile.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingProfile">The parsed charging profile.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingProfileParser">A delegate to parse custom charging profile JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out ChargingProfile?      ChargingProfile,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingProfile>?  CustomChargingProfileParser   = null)
        {

            try
            {

                ChargingProfile = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ChargingRateUnit          [mandatory]

                if (!JSON.ParseMandatoryEnum("charging_rate_unit",
                                             "charging rate unit",
                                             out ChargingRateUnits ChargingRateUnit,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Start                     [optional]

                if (JSON.ParseOptional("start_date_time",
                                       "start timestamp",
                                       out DateTime? Start,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse Duration                  [optional]

                if (JSON.ParseOptional("duration",
                                       "duration",
                                       TimeSpanExtensions.TryParseSeconds,
                                       out TimeSpan? Duration,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MinChargingRate           [optional]

                if (JSON.ParseOptional("min_charging_rate",
                                       "min charging rate",
                                       out Decimal? MinChargingRate,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse ChargingProfilePeriod     [optional]

                if (JSON.ParseOptional("charging_profile_period",
                                       "charging profile period",
                                       out DateTime? ChargingProfilePeriod,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                ChargingProfile = new ChargingProfile(
                                      ChargingRateUnit,
                                      Start,
                                      Duration,
                                      MinChargingRate
                                  );

                if (CustomChargingProfileParser is not null)
                    ChargingProfile = CustomChargingProfileParser(JSON,
                                                                  ChargingProfile);

                return true;

            }
            catch (Exception e)
            {
                ChargingProfile  = default;
                ErrorResponse    = "The given JSON representation of a charging profile is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChargingProfileSerializer = null, CustomChargingProfilePeriodSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profile JSON objects.</param>
        /// <param name="CustomChargingProfilePeriodSerializer">A delegate to serialize custom charging profile period JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingProfile>?        CustomChargingProfileSerializer         = null,
                              CustomJObjectSerializerDelegate<ChargingProfilePeriod>?  CustomChargingProfilePeriodSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("charging_rate_unit",        ChargingRateUnit.     ToString()),

                           Start.HasValue
                               ? new JProperty("start_date_time",           Start.          Value.ToIso8601())
                               : null,

                           Duration.HasValue
                               ? new JProperty("duration",                  Duration.       Value.TotalSeconds)
                               : null,

                           MinChargingRate.HasValue
                               ? new JProperty("min_charging_rate",         MinChargingRate.Value.ToString("0.0"))
                               : null,

                           ChargingProfilePeriods.SafeAny()
                               ? new JProperty("charging_profile_period",   new JArray(ChargingProfilePeriods.Select(chargingProfilePeriods => chargingProfilePeriods.ToJSON(CustomChargingProfilePeriodSerializer))))
                               : null


                       );

            return CustomChargingProfileSerializer is not null
                       ? CustomChargingProfileSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingProfile1, ChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfile1">A charging profile.</param>
        /// <param name="ChargingProfile2">Another charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingProfile ChargingProfile1,
                                           ChargingProfile ChargingProfile2)

            => ChargingProfile1.Equals(ChargingProfile2);

        #endregion

        #region Operator != (ChargingProfile1, ChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfile1">A charging profile.</param>
        /// <param name="ChargingProfile2">Another charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingProfile ChargingProfile1,
                                           ChargingProfile ChargingProfile2)

            => !(ChargingProfile1 == ChargingProfile2);

        #endregion

        #region Operator <  (ChargingProfile1, ChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfile1">A charging profile.</param>
        /// <param name="ChargingProfile2">Another charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingProfile ChargingProfile1,
                                          ChargingProfile ChargingProfile2)

            => ChargingProfile1.CompareTo(ChargingProfile2) < 0;

        #endregion

        #region Operator <= (ChargingProfile1, ChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfile1">A charging profile.</param>
        /// <param name="ChargingProfile2">Another charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingProfile ChargingProfile1,
                                           ChargingProfile ChargingProfile2)

            => !(ChargingProfile1 > ChargingProfile2);

        #endregion

        #region Operator >  (ChargingProfile1, ChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfile1">A charging profile.</param>
        /// <param name="ChargingProfile2">Another charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingProfile ChargingProfile1,
                                          ChargingProfile ChargingProfile2)

            => ChargingProfile1.CompareTo(ChargingProfile2) > 0;

        #endregion

        #region Operator >= (ChargingProfile1, ChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfile1">A charging profile.</param>
        /// <param name="ChargingProfile2">Another charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingProfile ChargingProfile1,
                                           ChargingProfile ChargingProfile2)

            => !(ChargingProfile1 < ChargingProfile2);

        #endregion

        #endregion

        #region IComparable<ChargingProfile> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging profiles.
        /// </summary>
        /// <param name="Object">A charging profile to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingProfile chargingProfile
                   ? CompareTo(chargingProfile)
                   : throw new ArgumentException("The given object is not a charging profile!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingProfile)

        /// <summary>
        /// Compares two charging profiles.
        /// </summary>
        /// <param name="ChargingProfile">A charging profile to compare with.</param>
        public Int32 CompareTo(ChargingProfile? ChargingProfile)
        {

            if (ChargingProfile is null)
                throw new ArgumentNullException(nameof(ChargingProfile), "The given charging profile must not be null!");

            var c = 0;

            if (c == 0 && Start.          HasValue && ChargingProfile.Start.          HasValue)
                c = Start.          Value.CompareTo(ChargingProfile.Start.          Value);

            if (c == 0 && Duration.       HasValue && ChargingProfile.Duration.       HasValue)
                c = Duration.       Value.CompareTo(ChargingProfile.Duration.       Value);

            if (c == 0)
                c = ChargingRateUnit.CompareTo(ChargingProfile.ChargingRateUnit);

            if (c == 0 && MinChargingRate.HasValue && ChargingProfile.MinChargingRate.HasValue)
                c = MinChargingRate.Value.CompareTo(ChargingProfile.MinChargingRate.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ChargingProfile> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging profiles for equality.
        /// </summary>
        /// <param name="Object">A charging profile to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingProfile chargingProfile &&
                   Equals(chargingProfile);

        #endregion

        #region Equals(ChargingProfile)

        /// <summary>
        /// Compares two charging profiles for equality.
        /// </summary>
        /// <param name="ChargingProfile">A charging profile to compare with.</param>
        public Boolean Equals(ChargingProfile? ChargingProfile)

            => ChargingProfile is not null &&

               ChargingRateUnit.Equals(ChargingProfile.ChargingRateUnit) &&
               Start.           Equals(ChargingProfile.Start)            &&
               Duration.        Equals(ChargingProfile.Duration)         &&
               MinChargingRate. Equals(ChargingProfile.MinChargingRate)  &&

               ChargingProfilePeriods.Count().Equals(ChargingProfile.ChargingProfilePeriods.Count()) &&
               ChargingProfilePeriods.All(profilePeriod => ChargingProfile.ChargingProfilePeriods.Contains(profilePeriod));

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

                   Start.HasValue
                       ? Start.ToString()
                       : "",

                   Duration.HasValue
                       ? ", " + Duration + " secs"
                       : "",

                   MinChargingRate.HasValue
                       ? ", " + MinChargingRate + " "
                       : "",

                   ChargingRateUnit

               );

        #endregion

    }

}
