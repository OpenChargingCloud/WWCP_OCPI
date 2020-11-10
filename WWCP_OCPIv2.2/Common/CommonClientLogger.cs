/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// The OCPI common client.
    /// </summary>
    public partial class CommonClient : IHTTPClient
    {

        /// <summary>
        /// The OCPI common client (HTTP client) logger.
        /// </summary>
        public class Logger : HTTPClientLogger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public const String DefaultContext = "OCPICommonClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached common client.
            /// </summary>
            public CommonClient CommonClient { get; }

            #endregion

            #region Constructor(s)

            #region Logger(CommonClient, Context = DefaultContext, LogfileCreator = null)

            /// <summary>
            /// Create a new common client logger using the default logging delegates.
            /// </summary>
            /// <param name="CommonClient">A common client.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public Logger(CommonClient            CommonClient,
                          String                  Context         = DefaultContext,
                          LogfileCreatorDelegate  LogfileCreator  = null)

                : this(CommonClient,
                       Context.IsNotNullOrEmpty() ? Context : DefaultContext,
                       null,
                       null,
                       null,
                       null,

                       LogfileCreator: LogfileCreator)

            { }

            #endregion

            #region Logger(CommonClient, Context, ... Logging delegates ...)

            /// <summary>
            /// Create a new common client logger using the given logging delegates.
            /// </summary>
            /// <param name="CommonClient">A common client.</param>
            /// <param name="Context">A context of this API.</param>
            /// 
            /// <param name="LogHTTPRequest_toConsole">A delegate to log incoming HTTP requests to console.</param>
            /// <param name="LogHTTPResponse_toConsole">A delegate to log HTTP requests/responses to console.</param>
            /// <param name="LogHTTPRequest_toDisc">A delegate to log incoming HTTP requests to disc.</param>
            /// <param name="LogHTTPResponse_toDisc">A delegate to log HTTP requests/responses to disc.</param>
            /// 
            /// <param name="LogHTTPRequest_toNetwork">A delegate to log incoming HTTP requests to a network target.</param>
            /// <param name="LogHTTPResponse_toNetwork">A delegate to log HTTP requests/responses to a network target.</param>
            /// <param name="LogHTTPRequest_toHTTPSSE">A delegate to log incoming HTTP requests to a HTTP client sent events source.</param>
            /// <param name="LogHTTPResponse_toHTTPSSE">A delegate to log HTTP requests/responses to a HTTP client sent events source.</param>
            /// 
            /// <param name="LogHTTPError_toConsole">A delegate to log HTTP errors to console.</param>
            /// <param name="LogHTTPError_toDisc">A delegate to log HTTP errors to disc.</param>
            /// <param name="LogHTTPError_toNetwork">A delegate to log HTTP errors to a network target.</param>
            /// <param name="LogHTTPError_toHTTPSSE">A delegate to log HTTP errors to a HTTP client sent events source.</param>
            /// 
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public Logger(CommonClient                CommonClient,
                          String                      Context,

                          HTTPRequestLoggerDelegate   LogHTTPRequest_toConsole,
                          HTTPResponseLoggerDelegate  LogHTTPResponse_toConsole,
                          HTTPRequestLoggerDelegate   LogHTTPRequest_toDisc,
                          HTTPResponseLoggerDelegate  LogHTTPResponse_toDisc,

                          HTTPRequestLoggerDelegate   LogHTTPRequest_toNetwork    = null,
                          HTTPResponseLoggerDelegate  LogHTTPResponse_toNetwork   = null,
                          HTTPRequestLoggerDelegate   LogHTTPRequest_toHTTPSSE    = null,
                          HTTPResponseLoggerDelegate  LogHTTPResponse_toHTTPSSE   = null,

                          HTTPResponseLoggerDelegate  LogHTTPError_toConsole      = null,
                          HTTPResponseLoggerDelegate  LogHTTPError_toDisc         = null,
                          HTTPResponseLoggerDelegate  LogHTTPError_toNetwork      = null,
                          HTTPResponseLoggerDelegate  LogHTTPError_toHTTPSSE      = null,

                          LogfileCreatorDelegate      LogfileCreator              = null)

                : base(CommonClient,
                       Context.IsNotNullOrEmpty() ? Context : DefaultContext,

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

                this.CommonClient = CommonClient ?? throw new ArgumentNullException(nameof(CommonClient), "The given common client must not be null!");

                #region Versions

                RegisterEvent("GetVersionsRequest",
                              handler => CommonClient.OnGetVersionsHTTPRequest += handler,
                              handler => CommonClient.OnGetVersionsHTTPRequest -= handler,
                              "GetVersions", "Versions", "Request", "All").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetVersionsResponse",
                              handler => CommonClient.OnGetVersionsHTTPResponse += handler,
                              handler => CommonClient.OnGetVersionsHTTPResponse -= handler,
                              "GetVersions", "Versions", "Response", "All").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetVersionDetailsRequest",
                              handler => CommonClient.OnGetVersionDetailsHTTPRequest += handler,
                              handler => CommonClient.OnGetVersionDetailsHTTPRequest -= handler,
                              "GetVersionDetails", "Versions", "Request", "All").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetVersionDetailsResponse",
                              handler => CommonClient.OnGetVersionDetailsHTTPRequest += handler,
                              handler => CommonClient.OnGetVersionDetailsHTTPRequest -= handler,
                              "GetVersionDetails", "Versions", "Response", "All").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Credentials

                RegisterEvent("RegisterRequest",
                              handler => CommonClient.OnRegisterHTTPRequest += handler,
                              handler => CommonClient.OnRegisterHTTPRequest -= handler,
                              "Register", "Credentials", "Request", "All").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("RegisterResponse",
                              handler => CommonClient.OnRegisterHTTPResponse += handler,
                              handler => CommonClient.OnRegisterHTTPResponse -= handler,
                              "Register", "Credentials", "Response", "All").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

            }

            #endregion

            #endregion

        }

     }

}
