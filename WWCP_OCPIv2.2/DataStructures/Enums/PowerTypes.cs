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

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// Extensions methods for power types.
    /// </summary>
    public static class PowerTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a power type.
        /// </summary>
        /// <param name="Text">A text representation of a power type.</param>
        public static PowerTypes Parse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return PowerTypes.UNKNOWN;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a power type.
        /// </summary>
        /// <param name="Text">A text representation of a power type.</param>
        public static PowerTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return null;

        }

        #endregion

        #region TryParse(Text, out PowerType)

        /// <summary>
        /// Try to parse the given text as a power type.
        /// </summary>
        /// <param name="Text">A text representation of a power type.</param>
        /// <param name="PowerType">The parsed power type.</param>
        public static Boolean TryParse(String Text, out PowerTypes PowerType)
        {
            switch (Text.Trim().ToUpper())
            {

                case "AC_1_PHASE":
                    PowerType = PowerTypes.AC_1_PHASE;
                    return true;

                case "AC_3_PHASE":
                    PowerType = PowerTypes.AC_3_PHASE;
                    return true;

                case "DC":
                    PowerType = PowerTypes.DC;
                    return true;

                default:
                    PowerType = PowerTypes.UNKNOWN;
                    return true;

            }
        }

        #endregion

        #region AsText(this PowerType)

        public static String AsText(this PowerTypes PowerType)

            => PowerType switch {
                   PowerTypes.AC_1_PHASE  => "AC_1_PHASE",
                   PowerTypes.AC_3_PHASE  => "AC_3_PHASE",
                   PowerTypes.DC          => "DC",
                   _                      => "UNKNOWN"
               };

        #endregion

    }


    /// <summary>
    /// The power type of the power.
    /// </summary>
    public enum PowerTypes
    {

        /// <summary>
        /// Unknown power type.
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// AC single phase.
        /// </summary>
        AC_1_PHASE,

        /// <summary>
        /// AC two phases, only two of the three available phases connected.
        /// </summary>
        AC_2_PHASE,

        /// <summary>
        /// AC two phases using split phase system.
        /// </summary>
        AC_2_PHASE_SPLIT,

        /// <summary>
        /// AC three phases.
        /// </summary>
        AC_3_PHASE,

        /// <summary>
        /// Direct Current.
        /// </summary>
        DC

    }

}
