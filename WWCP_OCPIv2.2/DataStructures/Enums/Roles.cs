/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// Extensions methods for roles.
    /// </summary>
    public static class RolesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a role.
        /// </summary>
        /// <param name="Text">A text representation of a role.</param>
        public static Roles Parse(String Text)
        {

            if (TryParse(Text, out var role))
                return role;

            return Roles.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a role.
        /// </summary>
        /// <param name="Text">A text representation of a role.</param>
        public static Roles? TryParse(String Text)
        {

            if (TryParse(Text, out var role))
                return role;

            return null;

        }

        #endregion

        #region TryParse(Text, out Role)

        /// <summary>
        /// Try to parse the given text as a role.
        /// </summary>
        /// <param name="Text">A text representation of a role.</param>
        /// <param name="Role">The parsed role.</param>
        public static Boolean TryParse(String Text, out Roles Role)
        {
            switch (Text.Trim().ToUpper())
            {

                case "OPENDATA":
                    Role = Roles.OpenData;
                    return true;

                case "CPO":
                    Role = Roles.CPO;
                    return true;

                case "EMSP":
                    Role = Roles.EMSP;
                    return true;

                case "HUB":
                    Role = Roles.HUB;
                    return true;

                case "NAP":
                    Role = Roles.NAP;
                    return true;

                case "NSP":
                    Role = Roles.NSP;
                    return true;

                case "OTHER":
                    Role = Roles.OTHER;
                    return true;

                case "SCSP":
                    Role = Roles.SCSP;
                    return true;

                default:
                    Role = Roles.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText(this Role)

        public static String AsText(this Roles Role)

            => Role switch {
                   Roles.OpenData  => "OPENDATA",
                   Roles.CPO       => "CPO",
                   Roles.EMSP      => "EMSP",
                   Roles.HUB       => "HUB",
                   Roles.NAP       => "NAP",
                   Roles.NSP       => "NSP",
                   Roles.OTHER     => "OTHER",
                   Roles.SCSP      => "SCSP",
                   _               => "unknown"
               };

        #endregion

    }


    /// <summary>
    /// The role of a party.
    /// </summary>
    public enum Roles
    {

        /// <summary>
        /// Unknown role.
        /// </summary>
        Unknown,

        OpenData,

        /// <summary>
        /// A charge point operator operates a network of charging stations.
        /// </summary>
        CPO,

        /// <summary>
        /// An E-Mobility Service Provider gives electric vehicle drivers access to charging services.
        /// </summary>
        EMSP,

        /// <summary>
        /// A roaming hub can connect multiple CPOs to multiple eMSPs.
        /// </summary>
        HUB,

        /// <summary>
        /// National Access Point: National database with all location information of a country.
        /// </summary>
        NAP,

        /// <summary>
        /// Navigation Service Provider: Like an eMSP, but probably only interested in location information.
        /// </summary>
        NSP,

        /// <summary>
        /// Other.
        /// </summary>
        OTHER,

        /// <summary>
        /// Smart Charging Service Provider.
        /// </summary>
        SCSP

    }

}
