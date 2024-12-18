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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Extensions methods for interface roles.
    /// </summary>
    public static class InterfaceRolesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an interface role.
        /// </summary>
        /// <param name="Text">A text representation of an interface role.</param>
        public static InterfaceRoles Parse(String Text)
        {

            if (TryParse(Text, out var role))
                return role;

            return InterfaceRoles.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an interface role.
        /// </summary>
        /// <param name="Text">A text representation of an interface role.</param>
        public static InterfaceRoles? TryParse(String Text)
        {

            if (TryParse(Text, out var role))
                return role;

            return null;

        }

        #endregion

        #region TryParse(Text, out InterfaceRole)

        /// <summary>
        /// Try to parse the given text as an interface role.
        /// </summary>
        /// <param name="Text">A text representation of an interface role.</param>
        /// <param name="InterfaceRole">The parsed interface role.</param>
        public static Boolean TryParse(String Text, out InterfaceRoles InterfaceRole)
        {
            switch (Text.Trim().ToUpper())
            {

                case "SENDER":
                    InterfaceRole = InterfaceRoles.SENDER;
                    return true;

                case "RECEIVER":
                    InterfaceRole = InterfaceRoles.RECEIVER;
                    return true;

                default:
                    InterfaceRole = InterfaceRoles.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText(this InterfaceRole)

        public static String AsText(this InterfaceRoles InterfaceRole)

            => InterfaceRole switch {
                   InterfaceRoles.SENDER    => "SENDER",
                   InterfaceRoles.RECEIVER  => "RECEIVER",
                _                           => "unknown"
               };

        #endregion

    }


    /// <summary>
    /// The interface role of a party.
    /// </summary>
    public enum InterfaceRoles
    {

        /// <summary>
        /// Unknown interface role.
        /// </summary>
        Unknown,

        /// <summary>
        /// Sender Interface implementation. Interface implemented by the owner of data, so the Receiver can Pull information from the data Sender/owner.
        /// </summary>
        SENDER,

        /// <summary>
        /// Receiver Interface implementation. Interface implemented by the receiver of data, so the Sender/owner can Push information to the Receiver.
        /// </summary>
        RECEIVER

    }

}
