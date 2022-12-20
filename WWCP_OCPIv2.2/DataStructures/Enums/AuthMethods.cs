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
    /// Extensions methods for authentication methods.
    /// </summary>
    public static class AuthMethodsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an authentication method.
        /// </summary>
        /// <param name="Text">A text representation of an authentication method.</param>
        public static AuthMethods Parse(String Text)
        {

            if (TryParse(Text, out var method))
                return method;

            return AuthMethods.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an authentication method.
        /// </summary>
        /// <param name="Text">A text representation of an authentication method.</param>
        public static AuthMethods? TryParse(String Text)
        {

            if (TryParse(Text, out var method))
                return method;

            return null;

        }

        #endregion

        #region TryParse(Text, out AuthMethod)

        /// <summary>
        /// Try to parse the given text as an authentication method.
        /// </summary>
        /// <param name="Text">A text representation of an authentication method.</param>
        /// <param name="AuthMethod">The parsed authentication method.</param>
        public static Boolean TryParse(String Text, out AuthMethods AuthMethod)
        {
            switch (Text.Trim().ToUpper())
            {

                case "AUTH_REQUEST":
                    AuthMethod = AuthMethods.AUTH_REQUEST;
                    return true;

                case "COMMAND":
                    AuthMethod = AuthMethods.COMMAND;
                    return true;

                case "WHITELIST":
                    AuthMethod = AuthMethods.WHITELIST;
                    return true;

                default:
                    AuthMethod = AuthMethods.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText(this AuthMethod)

        public static String AsText(this AuthMethods AuthMethod)

            => AuthMethod switch {
                   AuthMethods.AUTH_REQUEST  => "AUTH_REQUEST",
                   AuthMethods.COMMAND       => "COMMAND",
                   AuthMethods.WHITELIST     => "WHITELIST",
                   _                         => "unknown"
               };

        #endregion

    }


    /// <summary>
    /// Authentication methods.
    /// </summary>
    public enum AuthMethods
    {

        /// <summary>
        /// Unknown authentication method.
        /// </summary>
        Unknown,

        /// <summary>
        /// An authentication request has been sent to the eMSP.
        /// </summary>
        AUTH_REQUEST,

        /// <summary>
        /// A command like 'StartSession' or 'ReserveNow' was used to start the charging session, the token provided in the command was used as authorization.
        /// </summary>
        COMMAND,

        /// <summary>
        /// A whitelist was used for authentication, no request to the eMSP has been performed.
        /// </summary>
        WHITELIST

    }

}
