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

#region Usings

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// A PTP API logger.
    /// </summary>
    public sealed class PTPAPILogger : CommonAPILogger
    {

        #region Data

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public new const String DefaultContext = "PTPAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked PTP API.
        /// </summary>
        public PTPAPI  PTPAPI  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new PTP API logger using the default logging delegates.
        /// </summary>
        /// <param name="PTPAPI">An PTP API.</param>
        /// <param name="Context">A context of this API.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public PTPAPILogger(PTPAPI                       PTPAPI,
                            String?                      Context          = DefaultContext,
                            String?                      LoggingPath      = null,
                            OCPILogfileCreatorDelegate?  LogfileCreator   = null)

            : base(PTPAPI.CommonAPI,
                   Context ?? DefaultContext,
                   LoggingPath,
                   LogfileCreator)

        {

            this.PTPAPI = PTPAPI ?? throw new ArgumentNullException(nameof(PTPAPI), "The given PTP API must not be null!");

            #region Terminal(s)

            RegisterEvent("GetTerminalsRequest",
                          handler => PTPAPI.OnGetTerminalsRequest += handler,
                          handler => PTPAPI.OnGetTerminalsRequest -= handler,
                          "GetTerminals", "Terminals", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTerminalsResponse",
                          handler => PTPAPI.OnGetTerminalsResponse += handler,
                          handler => PTPAPI.OnGetTerminalsResponse -= handler,
                          "GetTerminals", "Terminals", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Terminal

            RegisterEvent("GetTerminalRequest",
                          handler => PTPAPI.OnGetTerminalRequest += handler,
                          handler => PTPAPI.OnGetTerminalRequest -= handler,
                          "GetTerminal", "Terminals", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTerminalResponse",
                          handler => PTPAPI.OnGetTerminalResponse += handler,
                          handler => PTPAPI.OnGetTerminalResponse -= handler,
                          "GetTerminal", "Terminals", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

    }

}
