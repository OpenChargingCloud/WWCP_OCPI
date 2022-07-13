/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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

#region Usings

using System;

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.HTTP
{

    /// <summary>
    /// A CPO API logger.
    /// </summary>
    public class CPOAPILogger : OCPIAPILogger
    {

        #region Data

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public const String DefaultContext = "CPOAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked CPO API.
        /// </summary>
        public CPOAPI  CPOAPI  { get; }

        #endregion

        #region Constructor(s)

        #region CPOAPILogger(CPOAPI, Context = DefaultContext, LogFileCreator = null)

        /// <summary>
        /// Create a new CPO API logger using the default logging delegates.
        /// </summary>
        /// <param name="CPOAPI">An CPO API.</param>
        /// <param name="Context">A context of this API.</param>
        /// <param name="LogFileCreator">A delegate to create a log file from the given context and log file name.</param>
        public CPOAPILogger(CPOAPI                CPOAPI,
                             String                  Context         = DefaultContext,
                             LogfileCreatorDelegate  LogFileCreator  = null)

            : this(CPOAPI,
                   Context,
                   null,
                   null,
                   null,
                   null,
                   LogFileCreator: LogFileCreator)

        { }

        #endregion

        #region CPOAPILogger(CPOAPI, Context, ... Logging delegates ...)

        /// <summary>
        /// Create a new CPO API logger using the given logging delegates.
        /// </summary>
        /// <param name="CPOAPI">An CPO API.</param>
        /// <param name="Context">A context of this API.</param>
        /// 
        /// <param name="LogHTTPRequest_toConsole">A delegate to log incoming HTTP requests to console.</param>
        /// <param name="LogHTTPResponse_toConsole">A delegate to log HTTP requests/responses to console.</param>
        /// <param name="LogHTTPRequest_toDisc">A delegate to log incoming HTTP requests to disc.</param>
        /// <param name="LogHTTPResponse_toDisc">A delegate to log HTTP requests/responses to disc.</param>
        /// 
        /// <param name="LogHTTPRequest_toNetwork">A delegate to log incoming HTTP requests to a network target.</param>
        /// <param name="LogHTTPResponse_toNetwork">A delegate to log HTTP requests/responses to a network target.</param>
        /// <param name="LogHTTPRequest_toHTTPSSE">A delegate to log incoming HTTP requests to a HTTP server sent events source.</param>
        /// <param name="LogHTTPResponse_toHTTPSSE">A delegate to log HTTP requests/responses to a HTTP server sent events source.</param>
        /// 
        /// <param name="LogHTTPError_toConsole">A delegate to log HTTP errors to console.</param>
        /// <param name="LogHTTPError_toDisc">A delegate to log HTTP errors to disc.</param>
        /// <param name="LogHTTPError_toNetwork">A delegate to log HTTP errors to a network target.</param>
        /// <param name="LogHTTPError_toHTTPSSE">A delegate to log HTTP errors to a HTTP server sent events source.</param>
        /// 
        /// <param name="LogFileCreator">A delegate to create a log file from the given context and log file name.</param>
        public CPOAPILogger(CPOAPI                     CPOAPI,
                             String                      Context,

                             OCPIRequestLoggerDelegate   LogHTTPRequest_toConsole,
                             OCPIResponseLoggerDelegate  LogHTTPResponse_toConsole,
                             OCPIRequestLoggerDelegate   LogHTTPRequest_toDisc,
                             OCPIResponseLoggerDelegate  LogHTTPResponse_toDisc,

                             OCPIRequestLoggerDelegate   LogHTTPRequest_toNetwork    = null,
                             OCPIResponseLoggerDelegate  LogHTTPResponse_toNetwork   = null,
                             OCPIRequestLoggerDelegate   LogHTTPRequest_toHTTPSSE    = null,
                             OCPIResponseLoggerDelegate  LogHTTPResponse_toHTTPSSE   = null,

                             OCPIResponseLoggerDelegate  LogHTTPError_toConsole      = null,
                             OCPIResponseLoggerDelegate  LogHTTPError_toDisc         = null,
                             OCPIResponseLoggerDelegate  LogHTTPError_toNetwork      = null,
                             OCPIResponseLoggerDelegate  LogHTTPError_toHTTPSSE      = null,

                             LogfileCreatorDelegate      LogFileCreator              = null)

            : base(CPOAPI.HTTPServer,
                   Context,

                   LogHTTPRequest_toConsole,
                   LogHTTPResponse_toConsole,
                   LogHTTPRequest_toDisc,
                   LogHTTPResponse_toDisc,

                   LogHTTPRequest_toNetwork,
                   LogHTTPResponse_toNetwork,
                   LogHTTPRequest_toHTTPSSE,
                   LogHTTPResponse_toHTTPSSE,

                   LogHTTPError_toConsole,
                   LogHTTPError_toDisc,
                   LogHTTPError_toNetwork,
                   LogHTTPError_toHTTPSSE,

                   LogFileCreator)

        {

            this.CPOAPI = CPOAPI ?? throw new ArgumentNullException(nameof(CPOAPI), "The given CPO API must not be null!");

            #region Tokens

            RegisterEvent("PutTokenRequest",
                          handler => CPOAPI.OnPutTokenRequest += handler,
                          handler => CPOAPI.OnPutTokenRequest -= handler,
                          "PutToken", "Tokens", "Put", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutTokenResponse",
                          handler => CPOAPI.OnPutTokenResponse += handler,
                          handler => CPOAPI.OnPutTokenResponse -= handler,
                          "PutToken", "Tokens", "Put", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchTokenRequest",
                          handler => CPOAPI.OnPatchTokenRequest += handler,
                          handler => CPOAPI.OnPatchTokenRequest -= handler,
                          "PatchToken", "Tokens", "Patch", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchTokenResponse",
                          handler => CPOAPI.OnPatchTokenResponse += handler,
                          handler => CPOAPI.OnPatchTokenResponse -= handler,
                          "PatchToken", "Tokens", "Patch", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteTokenRequest",
                          handler => CPOAPI.OnDeleteTokenRequest += handler,
                          handler => CPOAPI.OnDeleteTokenRequest -= handler,
                          "DeleteToken", "Tokens", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteTokenResponse",
                          handler => CPOAPI.OnDeleteTokenResponse += handler,
                          handler => CPOAPI.OnDeleteTokenResponse -= handler,
                          "DeleteToken", "Tokens", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

        #endregion

    }

}
