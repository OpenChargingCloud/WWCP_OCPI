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

using org.GraphDefined.Vanaheimr.Hermod.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.HTTP
{

    /// <summary>
    /// A Common API logger.
    /// </summary>
    public class CommonAPILogger : OCPIAPILogger
    {

        #region Data

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public const String DefaultContext = "CommonAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked Common API.
        /// </summary>
        public CommonAPI  CommonAPI  { get; }

        #endregion

        #region Constructor(s)

        #region CommonAPILogger(CommonAPI, Context = DefaultContext, LogfileCreator = null)

        /// <summary>
        /// Create a new Common API logger using the default logging delegates.
        /// </summary>
        /// <param name="CommonAPI">An Common API.</param>
        /// <param name="Context">A context of this API.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public CommonAPILogger(CommonAPI                CommonAPI,
                               String                   LoggingPath,
                               String                   Context          = DefaultContext,
                               LogfileCreatorDelegate?  LogfileCreator   = null)

            : this(CommonAPI,
                   LoggingPath,
                   Context ?? DefaultContext,
                   null,
                   null,
                   null,
                   null,
                   LogfileCreator: LogfileCreator)

        { }

        #endregion

        #region CommonAPILogger(CommonAPI, Context, ... Logging delegates ...)

        /// <summary>
        /// Create a new Common API logger using the given logging delegates.
        /// </summary>
        /// <param name="CommonAPI">An Common API.</param>
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
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public CommonAPILogger(CommonAPI                    CommonAPI,
                               String                       LoggingPath,
                               String                       Context,

                               OCPIRequestLoggerDelegate?   LogHTTPRequest_toConsole    = null,
                               OCPIResponseLoggerDelegate?  LogHTTPResponse_toConsole   = null,
                               OCPIRequestLoggerDelegate?   LogHTTPRequest_toDisc       = null,
                               OCPIResponseLoggerDelegate?  LogHTTPResponse_toDisc      = null,

                               OCPIRequestLoggerDelegate?   LogHTTPRequest_toNetwork    = null,
                               OCPIResponseLoggerDelegate?  LogHTTPResponse_toNetwork   = null,
                               OCPIRequestLoggerDelegate?   LogHTTPRequest_toHTTPSSE    = null,
                               OCPIResponseLoggerDelegate?  LogHTTPResponse_toHTTPSSE   = null,

                               OCPIResponseLoggerDelegate?  LogHTTPError_toConsole      = null,
                               OCPIResponseLoggerDelegate?  LogHTTPError_toDisc         = null,
                               OCPIResponseLoggerDelegate?  LogHTTPError_toNetwork      = null,
                               OCPIResponseLoggerDelegate?  LogHTTPError_toHTTPSSE      = null,

                               LogfileCreatorDelegate?      LogfileCreator              = null)

            : base(CommonAPI.HTTPServer,
                   LoggingPath,
                   Context ?? DefaultContext,

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

                   LogfileCreator)

        {

            this.CommonAPI = CommonAPI ?? throw new ArgumentNullException(nameof(CommonAPI), "The given Common API must not be null!");

            #region Credentials

            RegisterEvent("PostCredentialsRequest",
                          handler => CommonAPI.OnPostCredentialsRequest += handler,
                          handler => CommonAPI.OnPostCredentialsRequest -= handler,
                          "Credentials", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PostCredentialsResponse",
                          handler => CommonAPI.OnPostCredentialsResponse += handler,
                          handler => CommonAPI.OnPostCredentialsResponse -= handler,
                          "Credentials", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutCredentialsRequest",
                          handler => CommonAPI.OnPutCredentialsRequest += handler,
                          handler => CommonAPI.OnPutCredentialsRequest -= handler,
                          "Credentials", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutCredentialsResponse",
                          handler => CommonAPI.OnPutCredentialsResponse += handler,
                          handler => CommonAPI.OnPutCredentialsResponse -= handler,
                          "Credentials", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteCredentialsRequest",
                          handler => CommonAPI.OnDeleteCredentialsRequest += handler,
                          handler => CommonAPI.OnDeleteCredentialsRequest -= handler,
                          "Credentials", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteCredentialsResponse",
                          handler => CommonAPI.OnDeleteCredentialsResponse += handler,
                          handler => CommonAPI.OnDeleteCredentialsResponse -= handler,
                          "Credentials", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

        #endregion

    }

}
