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
    /// Extensions methods for remote access status.
    /// </summary>
    public static class RemoteAccessStatusExtensions
    {

        #region Parse    (Text)

        /// <summary>
        /// Parse the given text as a remote access status.
        /// </summary>
        /// <param name="Text">A text representation of a remote access status.</param>
        public static RemoteAccessStatus Parse(String Text)
        {

            if (TryParse(Text, out var remoteAccessStatus))
                return remoteAccessStatus;

            return RemoteAccessStatus.UNKNOWN;

        }

        #endregion

        #region TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a remote access status.
        /// </summary>
        /// <param name="Text">A text representation of a remote access status.</param>
        public static RemoteAccessStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var remoteAccessStatus))
                return remoteAccessStatus;

            return null;

        }

        #endregion

        #region TryParse (Text, out RemoteAccessStatus)

        /// <summary>
        /// Try to parse the given text as a remote access status.
        /// </summary>
        /// <param name="Text">A text representation of a remote access status.</param>
        /// <param name="RemoteAccessStatus">The parsed remote access status.</param>
        public static Boolean TryParse(String Text, out RemoteAccessStatus RemoteAccessStatus)
        {
            switch (Text.Trim().ToUpper())
            {

                case "PRE_LOCAL_REGISTRATION":
                    RemoteAccessStatus = RemoteAccessStatus.PRE_LOCAL_REGISTRATION;
                    return true;

                case "PRE_REMOTE_REGISTRATION":
                    RemoteAccessStatus = RemoteAccessStatus.PRE_REMOTE_REGISTRATION;
                    return true;

                case "OFFLINE":
                    RemoteAccessStatus = RemoteAccessStatus.OFFLINE;
                    return true;

                case "BANNED":
                    RemoteAccessStatus = RemoteAccessStatus.BANNED;
                    return true;

                case "ONLINE":
                    RemoteAccessStatus = RemoteAccessStatus.ONLINE;
                    return true;

                default:
                    RemoteAccessStatus = RemoteAccessStatus.UNKNOWN;
                    return false;

            }
        }

        #endregion

        #region AsText   (this RemoteAccessStatus)

        public static String AsText(this RemoteAccessStatus RemoteAccessStatus)

            => RemoteAccessStatus switch {
                   RemoteAccessStatus.PRE_LOCAL_REGISTRATION   => "PRE_LOCAL_REGISTRATION",
                   RemoteAccessStatus.PRE_REMOTE_REGISTRATION  => "PRE_REMOTE_REGISTRATION",
                   RemoteAccessStatus.OFFLINE                  => "OFFLINE",
                   RemoteAccessStatus.BANNED                   => "BANNED",
                   RemoteAccessStatus.ONLINE                   => "ONLINE",
                   _                                           => "UNKNOWN"
               };

        #endregion

    }


    /// <summary>
    /// The access status of a remote party.
    /// </summary>
    public enum RemoteAccessStatus
    {

        /// <summary>
        /// The remote access status is unknown.
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// The remote party is waiting for our registration.
        /// </summary>
        PRE_LOCAL_REGISTRATION,

        /// <summary>
        /// The remote party will soon start a registration.
        /// </summary>
        PRE_REMOTE_REGISTRATION,

        /// <summary>
        /// The remote access status is offline.
        /// </summary>
        OFFLINE,

        /// <summary>
        /// The remote access status is banned.
        /// </summary>
        BANNED,

        /// <summary>
        /// The remote access status is online.
        /// </summary>
        ONLINE

    }

}
