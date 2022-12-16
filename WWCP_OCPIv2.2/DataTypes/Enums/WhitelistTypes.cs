/*
 * Copyright (c) 2015-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extensions methods for whitelist types.
    /// </summary>
    public static class WhitelistTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a whitelist type.
        /// </summary>
        /// <param name="Text">A text representation of a whitelist type.</param>
        public static WhitelistTypes Parse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return WhitelistTypes.NEVER;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a whitelist type.
        /// </summary>
        /// <param name="Text">A text representation of a whitelist type.</param>
        public static WhitelistTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return null;

        }

        #endregion

        #region TryParse(Text, out WhitelistType)

        /// <summary>
        /// Try to parse the given text as a whitelist type.
        /// </summary>
        /// <param name="Text">A text representation of a whitelist type.</param>
        /// <param name="WhitelistType">The parsed whitelist type.</param>
        public static Boolean TryParse(String Text, out WhitelistTypes WhitelistType)
        {
            switch (Text.Trim().ToUpper())
            {

                case "ALWAYS":
                    WhitelistType = WhitelistTypes.ALWAYS;
                    return true;

                case "ALLOWED":
                    WhitelistType = WhitelistTypes.ALLOWED;
                    return true;

                case "ALLOWED_OFFLINE":
                    WhitelistType = WhitelistTypes.ALLOWED_OFFLINE;
                    return true;

                default:
                    WhitelistType = WhitelistTypes.NEVER;
                    return true;

            }
        }

        #endregion

        #region AsText(this WhitelistType)

        public static String AsText(this WhitelistTypes WhitelistType)

            => WhitelistType switch {
                   WhitelistTypes.ALWAYS           => "ALWAYS",
                   WhitelistTypes.ALLOWED          => "ALLOWED",
                   WhitelistTypes.ALLOWED_OFFLINE  => "ALLOWED_OFFLINE",
                   _                               => "NEVER"
               };

        #endregion

    }


    /// <summary>
    /// Defines when authorization of a whitelist by the CPO is allowed.
    /// </summary>
    public enum WhitelistTypes
    {

        /// <summary>
        /// Token always has to be whitelisted, realtime authorization is not possible/allowed.
        /// CPO shall always allow any use of this whitelist.
        /// </summary>
        ALWAYS,

        /// <summary>
        /// It is allowed to whitelist the whitelist, realtime authorization is also allowed.
        /// The CPO may choose which version of authorization to use.
        /// </summary>
        ALLOWED,

        /// <summary>
        /// In normal situations realtime authorization shall be used. But when the CPO cannot
        /// get a response from the eMSP (communication between CPO and eMSP is offline),
        /// the CPO shall allow this Token to be used.
        /// </summary>
        ALLOWED_OFFLINE,

        /// <summary>
        /// Whitelisting is forbidden, only realtime authorization is allowed. CPO shall always
        /// send a realtime authorization for any use of this Token to the eMSP.
        /// </summary>
        NEVER

    }

}
