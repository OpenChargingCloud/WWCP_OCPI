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
    public static class AccessStatusExtensions
    {

        #region Parse    (Text)

        /// <summary>
        /// Parse the given text as a party status.
        /// </summary>
        /// <param name="Text">A text representation of a party status.</param>
        public static AccessStatus Parse(String Text)
        {

            if (TryParse(Text, out var accessStatus))
                return accessStatus;

            return AccessStatus.UNKNOWN;

        }

        #endregion

        #region TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a party status.
        /// </summary>
        /// <param name="Text">A text representation of a party status.</param>
        public static AccessStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var accessStatus))
                return accessStatus;

            return null;

        }

        #endregion

        #region TryParse (Text, out AccessStatus)

        /// <summary>
        /// Try to parse the given text as a party status.
        /// </summary>
        /// <param name="Text">A text representation of a party status.</param>
        /// <param name="AccessStatus">The parsed party status.</param>
        public static Boolean TryParse(String Text, out AccessStatus AccessStatus)
        {
            switch (Text.Trim().ToUpper())
            {

                case "BLOCKED":
                    AccessStatus = AccessStatus.BLOCKED;
                    return true;

                case "ALLOWED":
                    AccessStatus = AccessStatus.ALLOWED;
                    return true;

                default:
                    AccessStatus = AccessStatus.UNKNOWN;
                    return false;

            }
        }

        #endregion

        #region AsText   (this AccessStatus)

        public static String AsText(this AccessStatus AccessStatus)

            => AccessStatus switch {
                   AccessStatus.BLOCKED  => "BLOCKED",
                   AccessStatus.ALLOWED  => "ALLOWED",
                   _                     => "UNKNOWN"
               };

        #endregion

    }


    /// <summary>
    /// The access status of an OCPI party.
    /// </summary>
    public enum AccessStatus
    {

        /// <summary>
        /// Unknown access status.
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// The other party is blocked.
        /// </summary>
        BLOCKED,

        /// <summary>
        /// The other party is allowed to connect.
        /// </summary>
        ALLOWED

    }

}
