/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPIv2_2_1.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.SCSP.HTTP
{

    /// <summary>
    /// The OCPI SCSP client.
    /// </summary>
    public partial class SCSPClient : IHTTPClient
    {

        /// <summary>
        /// The OCPI SCSP HTTP client logger.
        /// </summary>
        public new class Logger : CommonClient.Logger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public const String  DefaultContext  = "OCPISCSPClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached SCSP client.
            /// </summary>
            public SCSPClient  SCSPClient    { get; }

            #endregion

            #region Constructor(s)

            #region Logger(SCSPClient, Context = DefaultContext, LogfileCreator = null)

            /// <summary>
            /// Create a new SCSP client logger using the default logging delegates.
            /// </summary>
            /// <param name="SCSPClient">A SCSP client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public Logger(SCSPClient               SCSPClient,
                          String                   LoggingPath,
                          String                   Context          = DefaultContext,
                          LogfileCreatorDelegate?  LogfileCreator   = null)

                : this(SCSPClient,
                       LoggingPath,
                       Context.IsNotNullOrEmpty() ? Context : DefaultContext,
                       null,
                       null,
                       null,
                       null,

                       LogfileCreator: LogfileCreator)

            { }

            #endregion

            #region Logger(SCSPClient, Context, ... Logging delegates ...)

            /// <summary>
            /// Create a new SCSP client logger using the given logging delegates.
            /// </summary>
            /// <param name="SCSPClient">A SCSP client.</param>
            /// <param name="LoggingPath">The logging path.</param>
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
            public Logger(SCSPClient                   SCSPClient,
                          String                       LoggingPath,
                          String                       Context,

                          HTTPRequestLoggerDelegate?   LogHTTPRequest_toConsole    = null,
                          HTTPResponseLoggerDelegate?  LogHTTPResponse_toConsole   = null,
                          HTTPRequestLoggerDelegate?   LogHTTPRequest_toDisc       = null,
                          HTTPResponseLoggerDelegate?  LogHTTPResponse_toDisc      = null,

                          HTTPRequestLoggerDelegate?   LogHTTPRequest_toNetwork    = null,
                          HTTPResponseLoggerDelegate?  LogHTTPResponse_toNetwork   = null,
                          HTTPRequestLoggerDelegate?   LogHTTPRequest_toHTTPSSE    = null,
                          HTTPResponseLoggerDelegate?  LogHTTPResponse_toHTTPSSE   = null,

                          HTTPResponseLoggerDelegate?  LogHTTPError_toConsole      = null,
                          HTTPResponseLoggerDelegate?  LogHTTPError_toDisc         = null,
                          HTTPResponseLoggerDelegate?  LogHTTPError_toNetwork      = null,
                          HTTPResponseLoggerDelegate?  LogHTTPError_toHTTPSSE      = null,

                          LogfileCreatorDelegate?      LogfileCreator              = null)

                : base(SCSPClient,
                       LoggingPath,
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

                this.SCSPClient = SCSPClient ?? throw new ArgumentNullException(nameof(SCSPClient), "The given SCSP client must not be null!");

                #region Locations

                RegisterEvent("GetLocationsRequest",
                              handler => SCSPClient.OnGetLocationsHTTPRequest += handler,
                              handler => SCSPClient.OnGetLocationsHTTPRequest -= handler,
                              "GetLocations", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationsResponse",
                              handler => SCSPClient.OnGetLocationsHTTPResponse += handler,
                              handler => SCSPClient.OnGetLocationsHTTPResponse -= handler,
                              "GetLocations", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetLocationRequest",
                              handler => SCSPClient.OnGetLocationHTTPRequest += handler,
                              handler => SCSPClient.OnGetLocationHTTPRequest -= handler,
                              "GetLocation", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationResponse",
                              handler => SCSPClient.OnGetLocationHTTPResponse += handler,
                              handler => SCSPClient.OnGetLocationHTTPResponse -= handler,
                              "GetLocation", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetEVSERequest",
                              handler => SCSPClient.OnGetEVSEHTTPRequest += handler,
                              handler => SCSPClient.OnGetEVSEHTTPRequest -= handler,
                              "GetEVSE", "EVSEs", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetEVSEResponse",
                              handler => SCSPClient.OnGetEVSEHTTPResponse += handler,
                              handler => SCSPClient.OnGetEVSEHTTPResponse -= handler,
                              "GetEVSE", "EVSEs", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetConnectorRequest",
                              handler => SCSPClient.OnGetConnectorHTTPRequest += handler,
                              handler => SCSPClient.OnGetConnectorHTTPRequest -= handler,
                              "GetConnector", "connectors", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetConnectorResponse",
                              handler => SCSPClient.OnGetConnectorHTTPResponse += handler,
                              handler => SCSPClient.OnGetConnectorHTTPResponse -= handler,
                              "GetConnector", "connectors", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Sessions

                RegisterEvent("GetSessionsRequest",
                              handler => SCSPClient.OnGetSessionsHTTPRequest += handler,
                              handler => SCSPClient.OnGetSessionsHTTPRequest -= handler,
                              "GetSessions", "sessions", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionsResponse",
                              handler => SCSPClient.OnGetSessionsHTTPResponse += handler,
                              handler => SCSPClient.OnGetSessionsHTTPResponse -= handler,
                              "GetSessions", "sessions", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetSessionRequest",
                              handler => SCSPClient.OnGetSessionHTTPRequest += handler,
                              handler => SCSPClient.OnGetSessionHTTPRequest -= handler,
                              "GetSession", "sessions", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionResponse",
                              handler => SCSPClient.OnGetSessionHTTPResponse += handler,
                              handler => SCSPClient.OnGetSessionHTTPResponse -= handler,
                              "GetSession", "sessions", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

            }

            #endregion

            #endregion

        }

     }

}
