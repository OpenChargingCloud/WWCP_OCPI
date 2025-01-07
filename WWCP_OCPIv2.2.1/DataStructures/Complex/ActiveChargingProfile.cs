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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// Defines an active charging profile.
    /// </summary>
    public class ActiveChargingProfile : IEquatable<ActiveChargingProfile>,
                                         IComparable<ActiveChargingProfile>,
                                         IComparable
    {

        #region Properties

        /// <summary>
        /// Date and time at which the charge point has calculated this active charging profile.
        /// All time measurements within the profile are relative to this timestamp.
        /// </summary>
        [Mandatory]
        public DateTime         Start              { get; }

        /// <summary>
        /// The active charging profile including an enumeration of charging periods.
        /// </summary>
        [Mandatory]
        public ChargingProfile  ChargingProfile    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new active charging profile.
        /// </summary>
        /// <param name="Start">Date and time at which the charge point has calculated this active charging profile. All time measurements within the profile are relative to this timestamp.</param>
        /// <param name="ChargingProfile">The active charging profile including an enumeration of charging periods.</param>
        public ActiveChargingProfile(DateTime         Start,
                                     ChargingProfile  ChargingProfile)
        {

            this.Start            = Start;
            this.ChargingProfile  = ChargingProfile;

        }

        #endregion


        #region (static) Parse   (JSON, CustomActiveChargingProfileParser = null)

        /// <summary>
        /// Parse the given JSON representation of an active charging profile.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomActiveChargingProfileParser">A delegate to parse custom active charging profile JSON objects.</param>
        public static ActiveChargingProfile Parse(JObject                                              JSON,
                                                  CustomJObjectParserDelegate<ActiveChargingProfile>?  CustomActiveChargingProfileParser   = null)
        {

            if (TryParse(JSON,
                         out var activeChargingProfile,
                         out var errorResponse,
                         CustomActiveChargingProfileParser))
            {
                return activeChargingProfile!;
            }

            throw new ArgumentException("The given JSON representation of an active charging profile is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomActiveChargingProfileParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an active charging profile.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomActiveChargingProfileParser">A delegate to parse custom active charging profile JSON objects.</param>
        public static ActiveChargingProfile? TryParse(JObject                                              JSON,
                                                      CustomJObjectParserDelegate<ActiveChargingProfile>?  CustomActiveChargingProfileParser   = null)
        {

            if (TryParse(JSON,
                         out var activeChargingProfile,
                         out var errorResponse,
                         CustomActiveChargingProfileParser))
            {
                return activeChargingProfile;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out ActiveChargingProfile, out ErrorResponse, CustomActiveChargingProfileParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an active charging profile.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ActiveChargingProfile">The parsed active charging profile.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                     JSON,
                                       out ActiveChargingProfile?  ActiveChargingProfile,
                                       out String?                 ErrorResponse)

            => TryParse(JSON,
                        out ActiveChargingProfile,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an active charging profile.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ActiveChargingProfile">The parsed active charging profile.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomActiveChargingProfileParser">A delegate to parse custom active charging profile JSON objects.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       out ActiveChargingProfile?                           ActiveChargingProfile,
                                       out String?                                          ErrorResponse,
                                       CustomJObjectParserDelegate<ActiveChargingProfile>?  CustomActiveChargingProfileParser   = null)
        {

            try
            {

                ActiveChargingProfile = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Start             [mandatory]

                if (!JSON.ParseMandatory("start",
                                         "start timestamp",
                                         out DateTime Start,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ChargingProfile   [mandatory]

                if (!JSON.ParseMandatoryJSON("charging_profile",
                                             "charging profile",
                                             OCPIv2_2_1.ChargingProfile.TryParse,
                                             out ChargingProfile? ChargingProfile,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (ChargingProfile is null)
                    return false;

                #endregion


                ActiveChargingProfile = new ActiveChargingProfile(Start,
                                                                  ChargingProfile);

                if (CustomActiveChargingProfileParser is not null)
                    ActiveChargingProfile = CustomActiveChargingProfileParser(JSON,
                                                                              ActiveChargingProfile);

                return true;

            }
            catch (Exception e)
            {
                ActiveChargingProfile  = default;
                ErrorResponse          = "The given JSON representation of an active charging profile is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomActiveChargingProfileSerializer = null, CustomChargingProfileSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomActiveChargingProfileSerializer">A delegate to serialize custom active charging profile JSON objects.</param>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profile JSON objects.</param>
        /// <param name="CustomChargingProfilePeriodSerializer">A delegate to serialize custom charging profile period JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ActiveChargingProfile>?  CustomActiveChargingProfileSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingProfile>?        CustomChargingProfileSerializer         = null,
                              CustomJObjectSerializerDelegate<ChargingProfilePeriod>?  CustomChargingProfilePeriodSerializer   = null)
        {

            var json = JSONObject.Create(
                           new JProperty("start_date_time",   Start.          ToIso8601()),
                           new JProperty("charging_profile",  ChargingProfile.ToJSON   (CustomChargingProfileSerializer,
                                                                                        CustomChargingProfilePeriodSerializer))
                       );

            return CustomActiveChargingProfileSerializer is not null
                       ? CustomActiveChargingProfileSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ActiveChargingProfile1, ActiveChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ActiveChargingProfile1">An active charging profile.</param>
        /// <param name="ActiveChargingProfile2">Another active charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ActiveChargingProfile ActiveChargingProfile1,
                                           ActiveChargingProfile ActiveChargingProfile2)

            => ActiveChargingProfile1.Equals(ActiveChargingProfile2);

        #endregion

        #region Operator != (ActiveChargingProfile1, ActiveChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ActiveChargingProfile1">An active charging profile.</param>
        /// <param name="ActiveChargingProfile2">Another active charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ActiveChargingProfile ActiveChargingProfile1,
                                           ActiveChargingProfile ActiveChargingProfile2)

            => !(ActiveChargingProfile1 == ActiveChargingProfile2);

        #endregion

        #region Operator <  (ActiveChargingProfile1, ActiveChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ActiveChargingProfile1">An active charging profile.</param>
        /// <param name="ActiveChargingProfile2">Another active charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ActiveChargingProfile ActiveChargingProfile1,
                                          ActiveChargingProfile ActiveChargingProfile2)

            => ActiveChargingProfile1.CompareTo(ActiveChargingProfile2) < 0;

        #endregion

        #region Operator <= (ActiveChargingProfile1, ActiveChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ActiveChargingProfile1">An active charging profile.</param>
        /// <param name="ActiveChargingProfile2">Another active charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ActiveChargingProfile ActiveChargingProfile1,
                                           ActiveChargingProfile ActiveChargingProfile2)

            => !(ActiveChargingProfile1 > ActiveChargingProfile2);

        #endregion

        #region Operator >  (ActiveChargingProfile1, ActiveChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ActiveChargingProfile1">An active charging profile.</param>
        /// <param name="ActiveChargingProfile2">Another active charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ActiveChargingProfile ActiveChargingProfile1,
                                          ActiveChargingProfile ActiveChargingProfile2)

            => ActiveChargingProfile1.CompareTo(ActiveChargingProfile2) > 0;

        #endregion

        #region Operator >= (ActiveChargingProfile1, ActiveChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ActiveChargingProfile1">An active charging profile.</param>
        /// <param name="ActiveChargingProfile2">Another active charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ActiveChargingProfile ActiveChargingProfile1,
                                           ActiveChargingProfile ActiveChargingProfile2)

            => !(ActiveChargingProfile1 < ActiveChargingProfile2);

        #endregion

        #endregion

        #region IComparable<ActiveChargingProfile> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two active charging profiles.
        /// </summary>
        /// <param name="Object">An active charging profile to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ActiveChargingProfile activeChargingProfile
                   ? CompareTo(activeChargingProfile)
                   : throw new ArgumentException("The given object is not an active charging profile!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ActiveChargingProfile)

        /// <summary>
        /// Compares two active charging profiles.
        /// </summary>
        /// <param name="ActiveChargingProfile">An active charging profile to compare with.</param>
        public Int32 CompareTo(ActiveChargingProfile? ActiveChargingProfile)
        {

            if (ActiveChargingProfile is null)
                throw new ArgumentNullException(nameof(ActiveChargingProfile), "The given active charging profile must not be null!");

            var c = Start.          CompareTo(ActiveChargingProfile.Start);

            if (c == 0)
                c = ChargingProfile.CompareTo(ActiveChargingProfile.ChargingProfile);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ActiveChargingProfile> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two active charging profiles for equality.
        /// </summary>
        /// <param name="Object">An active charging profile to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ActiveChargingProfile activeChargingProfile &&
                   Equals(activeChargingProfile);

        #endregion

        #region Equals(ActiveChargingProfile)

        /// <summary>
        /// Compares two active charging profiles for equality.
        /// </summary>
        /// <param name="ActiveChargingProfile">An active charging profile to compare with.</param>
        public Boolean Equals(ActiveChargingProfile? ActiveChargingProfile)

            => ActiveChargingProfile is not null &&

               Start.          Equals(ActiveChargingProfile.Start) &&
               ChargingProfile.Equals(ActiveChargingProfile.ChargingProfile);

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

                return Start.          GetHashCode() * 3 ^
                       ChargingProfile.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Start,
                   ": ",
                   ChargingProfile.ToString()

               );

        #endregion

    }

}
