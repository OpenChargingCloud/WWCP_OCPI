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

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extensions methods for profile types.
    /// </summary>
    public static class ProfileTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a profile type.
        /// </summary>
        /// <param name="Text">A text representation of a profile type.</param>
        public static ProfileTypes Parse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return ProfileTypes.REGULAR;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a profile type.
        /// </summary>
        /// <param name="Text">A text representation of a profile type.</param>
        public static ProfileTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return null;

        }

        #endregion

        #region TryParse(Text, out ProfileType)

        /// <summary>
        /// Try to parse the given text as a profile type.
        /// </summary>
        /// <param name="Text">A text representation of a profile type.</param>
        /// <param name="ProfileType">The parsed profile type.</param>
        public static Boolean TryParse(String Text, out ProfileTypes ProfileType)
        {
            switch (Text.Trim().ToUpper())
            {

                case "CHEAP":
                    ProfileType = ProfileTypes.CHEAP;
                    return true;

                case "FAST":
                    ProfileType = ProfileTypes.FAST;
                    return true;

                case "GREEN":
                    ProfileType = ProfileTypes.GREEN;
                    return true;

                default:
                    ProfileType = ProfileTypes.REGULAR;
                    return true;

            }
        }

        #endregion

        #region AsText(this ProfileType)

        public static String AsText(this ProfileTypes ProfileType)

            => ProfileType switch {
                   ProfileTypes.CHEAP  => "CHEAP",
                   ProfileTypes.FAST   => "FAST",
                   ProfileTypes.GREEN  => "GREEN",
                   _                   => "REGULAR"
               };

        #endregion

    }


    /// <summary>
    /// Different smart charging profile types.
    /// </summary>
    public enum ProfileTypes
    {

        /// <summary>
        /// Driver wants to use the cheapest charging profile possible.
        /// </summary>
        CHEAP,

        /// <summary>
        /// Driver wants his EV charged as quickly as possible and is willing
        /// to pay a premium for this, if needed.
        /// </summary>
        FAST,

        /// <summary>
        /// Driver wants his EV charged with as much regenerative (green) energy as possible.
        /// </summary>
        GREEN,

        /// <summary>
        /// Driver does not have special preferences.
        /// </summary>
        REGULAR

    }

}
