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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The 'set charging profile' command.
    /// </summary>
    public readonly struct SetChargingProfile : IEquatable<SetChargingProfile>,
                                                IComparable<SetChargingProfile>,
                                                IComparable
    {

        #region Properties

        /// <summary>
        /// The charging profile containing limits for the available power or current over time.
        /// </summary>
        [Mandatory]
        public ChargingProfile  ChargingProfile    { get; }

        /// <summary>
        /// URL that the ChargingProfileResult POST should be send to. This URL might contain an
        /// unique identification to be able to distinguish between GET ActiveChargingProfile requests.
        /// </summary>
        [Mandatory]
        public URL              ResponseURL        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new 'set charging profile' command command.
        /// </summary>
        /// <param name="ChargingProfile">Session identification of the charging session that is requested to be stopped.</param>
        /// <param name="ResponseURL">URL that the CommandResult POST should be sent to. This URL might contain an unique identification to be able to distinguish between 'set charging profile' command requests.</param>
        public SetChargingProfile(ChargingProfile  ChargingProfile,
                                  URL              ResponseURL)
        {

            this.ChargingProfile  = ChargingProfile;
            this.ResponseURL      = ResponseURL;

        }

        #endregion


        #region (static) Parse   (JSON, CustomSetChargingProfileParser = null)

        /// <summary>
        /// Parse the given JSON representation of a 'set charging profile' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSetChargingProfileParser">A delegate to parse custom 'set charging profile' command JSON objects.</param>
        public static SetChargingProfile Parse(JObject                                          JSON,
                                               CustomJObjectParserDelegate<SetChargingProfile>  CustomSetChargingProfileParser   = null)
        {

            if (TryParse(JSON,
                         out SetChargingProfile  setChargingProfile,
                         out String              ErrorResponse,
                         CustomSetChargingProfileParser))
            {
                return setChargingProfile;
            }

            throw new ArgumentException("The given JSON representation of a 'set charging profile' command is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomSetChargingProfileParser = null)

        /// <summary>
        /// Parse the given text representation of a 'set charging profile' command.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomSetChargingProfileParser">A delegate to parse custom 'set charging profile' command JSON objects.</param>
        public static SetChargingProfile Parse(String                                           Text,
                                               CustomJObjectParserDelegate<SetChargingProfile>  CustomSetChargingProfileParser   = null)
        {

            if (TryParse(Text,
                         out SetChargingProfile  setChargingProfile,
                         out String              ErrorResponse,
                         CustomSetChargingProfileParser))
            {
                return setChargingProfile;
            }

            throw new ArgumentException("The given text representation of a 'set charging profile' command is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomSetChargingProfileParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a 'set charging profile' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSetChargingProfileParser">A delegate to parse custom 'set charging profile' command JSON objects.</param>
        public static SetChargingProfile? TryParse(JObject                                          JSON,
                                                   CustomJObjectParserDelegate<SetChargingProfile>  CustomSetChargingProfileParser   = null)
        {

            if (TryParse(JSON,
                         out SetChargingProfile  setChargingProfile,
                         out String              ErrorResponse,
                         CustomSetChargingProfileParser))
            {
                return setChargingProfile;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomSetChargingProfileParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a 'set charging profile' command.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomSetChargingProfileParser">A delegate to parse custom 'set charging profile' command JSON objects.</param>
        public static SetChargingProfile? TryParse(String                                           Text,
                                                   CustomJObjectParserDelegate<SetChargingProfile>  CustomSetChargingProfileParser   = null)
        {

            if (TryParse(Text,
                         out SetChargingProfile  setChargingProfile,
                         out String              ErrorResponse,
                         CustomSetChargingProfileParser))
            {
                return setChargingProfile;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out SetChargingProfile, out ErrorResponse, CustomSetChargingProfileParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a 'set charging profile' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SetChargingProfile">The parsed 'set charging profile' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                 JSON,
                                       out SetChargingProfile  SetChargingProfile,
                                       out String              ErrorResponse)

            => TryParse(JSON,
                        out SetChargingProfile,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a 'set charging profile' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SetChargingProfile">The parsed 'set charging profile' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetChargingProfileParser">A delegate to parse custom 'set charging profile' command JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       out SetChargingProfile                           SetChargingProfile,
                                       out String                                       ErrorResponse,
                                       CustomJObjectParserDelegate<SetChargingProfile>  CustomSetChargingProfileParser   = null)
        {

            try
            {

                SetChargingProfile = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ChargingProfile   [mandatory]

                if (!JSON.ParseMandatory("charging_profile",
                                         "charging profile",
                                         OCPIv2_2.ChargingProfile.TryParse,
                                         out ChargingProfile ChargingProfile,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ResponseURL       [mandatory]

                if (!JSON.ParseMandatory("response_url",
                                         "response URL",
                                         URL.TryParse,
                                         out URL ResponseURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                SetChargingProfile = new SetChargingProfile(ChargingProfile,
                                                            ResponseURL);

                if (CustomSetChargingProfileParser != null)
                    SetChargingProfile = CustomSetChargingProfileParser(JSON,
                                                                        SetChargingProfile);

                return true;

            }
            catch (Exception e)
            {
                SetChargingProfile  = default;
                ErrorResponse       = "The given JSON representation of a 'set charging profile' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out SetChargingProfile, out ErrorResponse, CustomSetChargingProfileParser = null)

        /// <summary>
        /// Try to parse the given text representation of a 'set charging profile' command.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="SetChargingProfile">The parsed setChargingProfile.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetChargingProfileParser">A delegate to parse custom 'set charging profile' command JSON objects.</param>
        public static Boolean TryParse(String                                           Text,
                                       out SetChargingProfile                           SetChargingProfile,
                                       out String                                       ErrorResponse,
                                       CustomJObjectParserDelegate<SetChargingProfile>  CustomSetChargingProfileParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out SetChargingProfile,
                                out ErrorResponse,
                                CustomSetChargingProfileParser);

            }
            catch (Exception e)
            {
                SetChargingProfile  = default;
                ErrorResponse       = "The given text representation of a 'set charging profile' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetChargingProfileSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetChargingProfileSerializer">A delegate to serialize custom 'set charging profile' command JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetChargingProfile> CustomSetChargingProfileSerializer = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("charging_profile",  ChargingProfile.ToString()),
                           new JProperty("response_url",      ResponseURL.    ToString())
                       );

            return CustomSetChargingProfileSerializer != null
                       ? CustomSetChargingProfileSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetChargingProfile1, SetChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfile1">An 'set charging profile' command.</param>
        /// <param name="SetChargingProfile2">Another 'set charging profile' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SetChargingProfile SetChargingProfile1,
                                           SetChargingProfile SetChargingProfile2)

            => SetChargingProfile1.Equals(SetChargingProfile2);

        #endregion

        #region Operator != (SetChargingProfile1, SetChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfile1">An 'set charging profile' command.</param>
        /// <param name="SetChargingProfile2">Another 'set charging profile' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SetChargingProfile SetChargingProfile1,
                                           SetChargingProfile SetChargingProfile2)

            => !(SetChargingProfile1 == SetChargingProfile2);

        #endregion

        #region Operator <  (SetChargingProfile1, SetChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfile1">An 'set charging profile' command.</param>
        /// <param name="SetChargingProfile2">Another 'set charging profile' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SetChargingProfile SetChargingProfile1,
                                          SetChargingProfile SetChargingProfile2)

            => SetChargingProfile1.CompareTo(SetChargingProfile2) < 0;

        #endregion

        #region Operator <= (SetChargingProfile1, SetChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfile1">An 'set charging profile' command.</param>
        /// <param name="SetChargingProfile2">Another 'set charging profile' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SetChargingProfile SetChargingProfile1,
                                           SetChargingProfile SetChargingProfile2)

            => !(SetChargingProfile1 > SetChargingProfile2);

        #endregion

        #region Operator >  (SetChargingProfile1, SetChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfile1">An 'set charging profile' command.</param>
        /// <param name="SetChargingProfile2">Another 'set charging profile' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SetChargingProfile SetChargingProfile1,
                                          SetChargingProfile SetChargingProfile2)

            => SetChargingProfile1.CompareTo(SetChargingProfile2) > 0;

        #endregion

        #region Operator >= (SetChargingProfile1, SetChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfile1">An 'set charging profile' command.</param>
        /// <param name="SetChargingProfile2">Another 'set charging profile' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SetChargingProfile SetChargingProfile1,
                                           SetChargingProfile SetChargingProfile2)

            => !(SetChargingProfile1 < SetChargingProfile2);

        #endregion

        #endregion

        #region IComparable<SetChargingProfile> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is SetChargingProfile setChargingProfile
                   ? CompareTo(setChargingProfile)
                   : throw new ArgumentException("The given object is not a 'set charging profile' command!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SetChargingProfile)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfile">An object to compare with.</param>
        public Int32 CompareTo(SetChargingProfile SetChargingProfile)
        {

            var c = ChargingProfile.CompareTo(SetChargingProfile.ChargingProfile);

            if (c == 0)
                c = ResponseURL.    CompareTo(SetChargingProfile.ResponseURL);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<SetChargingProfile> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is SetChargingProfile setChargingProfile &&
                   Equals(setChargingProfile);

        #endregion

        #region Equals(SetChargingProfile)

        /// <summary>
        /// Compares two 'set charging profile' commands for equality.
        /// </summary>
        /// <param name="SetChargingProfile">An 'set charging profile' command to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(SetChargingProfile SetChargingProfile)

            => ChargingProfile.Equals(SetChargingProfile.ChargingProfile) &&
               ResponseURL.    Equals(SetChargingProfile.ResponseURL);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return ChargingProfile.GetHashCode() * 3 ^
                       ResponseURL.    GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ChargingProfile.ToString(), " => ",
                             ResponseURL);

        #endregion

    }

}
