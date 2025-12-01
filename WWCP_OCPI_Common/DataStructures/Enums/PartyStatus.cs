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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extensions methods for party status.
    /// </summary>
    public static class PartyStatusExtensions
    {

        #region Parse    (Text)

        /// <summary>
        /// Parse the given text as a party status.
        /// </summary>
        /// <param name="Text">A text representation of a party status.</param>
        public static PartyStatus Parse(String Text)
        {

            if (TryParse(Text, out var partyStatus))
                return partyStatus;

            return PartyStatus.UNKNOWN;

        }

        #endregion

        #region TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a party status.
        /// </summary>
        /// <param name="Text">A text representation of a party status.</param>
        public static PartyStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var partyStatus))
                return partyStatus;

            return null;

        }

        #endregion

        #region TryParse (Text, out PartyStatus)

        /// <summary>
        /// Try to parse the given text as a party status.
        /// </summary>
        /// <param name="Text">A text representation of a party status.</param>
        /// <param name="PartyStatus">The parsed party status.</param>
        public static Boolean TryParse(String Text, out PartyStatus PartyStatus)
        {
            switch (Text.Trim().ToUpper())
            {

                case "PRE_LOCAL_REGISTRATION":
                    PartyStatus = PartyStatus.PRE_LOCAL_REGISTRATION;
                    return true;

                case "PRE_REMOTE_REGISTRATION":
                    PartyStatus = PartyStatus.PRE_REMOTE_REGISTRATION;
                    return true;

                case "DISABLED":
                    PartyStatus = PartyStatus.DISABLED;
                    return true;

                case "ENABLED":
                    PartyStatus = PartyStatus.ENABLED;
                    return true;

                default:
                    PartyStatus = PartyStatus.UNKNOWN;
                    return false;

            }
        }

        #endregion

        #region AsText   (this PartyStatus)

        public static String AsText(this PartyStatus PartyStatus)

            => PartyStatus switch {
                   PartyStatus.PRE_LOCAL_REGISTRATION   => "PRE_LOCAL_REGISTRATION",
                   PartyStatus.PRE_REMOTE_REGISTRATION  => "PRE_REMOTE_REGISTRATION",
                   PartyStatus.DISABLED                 => "DISABLED",
                   PartyStatus.ENABLED                  => "ENABLED",
                   _                                    => "UNKNOWN"
               };

        #endregion

    }


    /// <summary>
    /// The status of a (remote) party.
    /// </summary>
    public enum PartyStatus
    {

        /// <summary>
        /// Unknown party status.
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// The party is waiting for a local registration.
        /// </summary>
        PRE_LOCAL_REGISTRATION,

        /// <summary>
        /// The party is waiting for a remote registration.
        /// </summary>
        PRE_REMOTE_REGISTRATION,

        /// <summary>
        /// The party is disabled.
        /// </summary>
        DISABLED,

        /// <summary>
        /// The party is enabled.
        /// </summary>
        ENABLED

    }

}
