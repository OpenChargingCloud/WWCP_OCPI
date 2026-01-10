/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

#region Usings

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// A OCPI client configurator.
    /// </summary>
    public sealed class ClientConfigurator
    {

        /// <summary>
        /// The description of the OCPI client.
        /// </summary>
        public Func<RemoteParty_Id, I18NString>?  Description       { get; set; }

        /// <summary>
        /// Whether logging is disabled for this OCPI client.
        /// </summary>
        public Func<RemoteParty_Id, Boolean>?     DisableLogging    { get; set; }

        /// <summary>
        /// The logging path for this OCPI client.
        /// </summary>
        public Func<RemoteParty_Id, String>?      LoggingPath       { get; set; }

        /// <summary>
        /// The logging context for this OCPI client.
        /// </summary>
        public Func<RemoteParty_Id, String>?      LoggingContext    { get; set; }

        /// <summary>
        /// The logfile creator for this OCPI client.
        /// </summary>
        public OCPILogfileCreatorDelegate?        LogfileCreator    { get; set; }

    }

}
