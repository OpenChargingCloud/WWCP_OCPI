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

using org.GraphDefined.Vanaheimr.Hermod.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// A SCSP API logger.
    /// </summary>
    public class SCSPAPILogger : OCPIAPILogger
    {

        #region Data

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public const String DefaultContext = "SCSPAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked SCSP API.
        /// </summary>
        public SCSPAPI  SCSPAPI  { get; }

        #endregion

        #region Constructor(s)

        #region SCSPAPILogger(SCSPAPI, Context = DefaultContext, LogfileCreator = null)

        /// <summary>
        /// Create a new SCSP API logger using the default logging delegates.
        /// </summary>
        /// <param name="SCSPAPI">An SCSP API.</param>
        /// <param name="Context">A context of this API.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public SCSPAPILogger(SCSPAPI                  SCSPAPI,
                             String                   LoggingPath,
                             String?                  Context          = DefaultContext,
                             LogfileCreatorDelegate?  LogfileCreator   = null)

            : this(SCSPAPI,
                   LoggingPath,
                   Context ?? DefaultContext,
                   null,
                   null,
                   null,
                   null,
                   LogfileCreator: LogfileCreator)

        { }

        #endregion

        #region SCSPAPILogger(SCSPAPI, Context, ... Logging delegates ...)

        /// <summary>
        /// Create a new SCSP API logger using the given logging delegates.
        /// </summary>
        /// <param name="SCSPAPI">An SCSP API.</param>
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
        public SCSPAPILogger(SCSPAPI                      SCSPAPI,
                             String                       LoggingPath,
                             String                       Context,

                             OCPIRequestLoggerDelegate?   LogHTTPRequest_toConsole,
                             OCPIResponseLoggerDelegate?  LogHTTPResponse_toConsole,
                             OCPIRequestLoggerDelegate?   LogHTTPRequest_toDisc,
                             OCPIResponseLoggerDelegate?  LogHTTPResponse_toDisc,

                             OCPIRequestLoggerDelegate?   LogHTTPRequest_toNetwork    = null,
                             OCPIResponseLoggerDelegate?  LogHTTPResponse_toNetwork   = null,
                             OCPIRequestLoggerDelegate?   LogHTTPRequest_toHTTPSSE    = null,
                             OCPIResponseLoggerDelegate?  LogHTTPResponse_toHTTPSSE   = null,

                             OCPIResponseLoggerDelegate?  LogHTTPError_toConsole      = null,
                             OCPIResponseLoggerDelegate?  LogHTTPError_toDisc         = null,
                             OCPIResponseLoggerDelegate?  LogHTTPError_toNetwork      = null,
                             OCPIResponseLoggerDelegate?  LogHTTPError_toHTTPSSE      = null,

                             LogfileCreatorDelegate?      LogfileCreator              = null)

            : base(SCSPAPI.HTTPServer,
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

            this.SCSPAPI = SCSPAPI ?? throw new ArgumentNullException(nameof(SCSPAPI), "The given SCSP API must not be null!");

            #region Locations

            RegisterEvent("DeleteLocationsRequest",
                          handler => SCSPAPI.OnDeleteLocationsRequest += handler,
                          handler => SCSPAPI.OnDeleteLocationsRequest -= handler,
                          "DeleteLocations", "Locations", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteLocationsResponse",
                          handler => SCSPAPI.OnDeleteLocationsResponse += handler,
                          handler => SCSPAPI.OnDeleteLocationsResponse -= handler,
                          "DeleteLocations", "Locations", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutLocationRequest",
                          handler => SCSPAPI.OnPutLocationRequest += handler,
                          handler => SCSPAPI.OnPutLocationRequest -= handler,
                          "PutLocation", "Locations", "Put", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutLocationResponse",
                          handler => SCSPAPI.OnPutLocationResponse += handler,
                          handler => SCSPAPI.OnPutLocationResponse -= handler,
                          "PutLocation", "Locations", "Put", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchLocationRequest",
                          handler => SCSPAPI.OnPatchLocationRequest += handler,
                          handler => SCSPAPI.OnPatchLocationRequest -= handler,
                          "PatchLocation", "Locations", "Patch", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchLocationResponse",
                          handler => SCSPAPI.OnPatchLocationResponse += handler,
                          handler => SCSPAPI.OnPatchLocationResponse -= handler,
                          "PatchLocation", "Locations", "Patch", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteLocationRequest",
                          handler => SCSPAPI.OnDeleteLocationRequest += handler,
                          handler => SCSPAPI.OnDeleteLocationRequest -= handler,
                          "DeleteLocation", "Locations", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteLocationResponse",
                          handler => SCSPAPI.OnDeleteLocationResponse += handler,
                          handler => SCSPAPI.OnDeleteLocationResponse -= handler,
                          "DeleteLocation", "Locations", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region EVSEs

            RegisterEvent("PutEVSERequest",
                          handler => SCSPAPI.OnPutEVSERequest += handler,
                          handler => SCSPAPI.OnPutEVSERequest -= handler,
                          "PutEVSE", "EVSEs", "Put", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutEVSEResponse",
                          handler => SCSPAPI.OnPutEVSEResponse += handler,
                          handler => SCSPAPI.OnPutEVSEResponse -= handler,
                          "PutEVSE", "EVSEs", "Put", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchEVSERequest",
                          handler => SCSPAPI.OnPatchEVSERequest += handler,
                          handler => SCSPAPI.OnPatchEVSERequest -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchEVSEResponse",
                          handler => SCSPAPI.OnPatchEVSEResponse += handler,
                          handler => SCSPAPI.OnPatchEVSEResponse -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteEVSERequest",
                          handler => SCSPAPI.OnDeleteEVSERequest += handler,
                          handler => SCSPAPI.OnDeleteEVSERequest -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteEVSEResponse",
                          handler => SCSPAPI.OnDeleteEVSEResponse += handler,
                          handler => SCSPAPI.OnDeleteEVSEResponse -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Connectors

            RegisterEvent("PutConnectorRequest",
                          handler => SCSPAPI.OnPutConnectorRequest += handler,
                          handler => SCSPAPI.OnPutConnectorRequest -= handler,
                          "PutConnector", "Connectors", "Put", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutConnectorResponse",
                          handler => SCSPAPI.OnPutConnectorResponse += handler,
                          handler => SCSPAPI.OnPutConnectorResponse -= handler,
                          "PutConnector", "Connectors", "Put", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchConnectorRequest",
                          handler => SCSPAPI.OnPatchConnectorRequest += handler,
                          handler => SCSPAPI.OnPatchConnectorRequest -= handler,
                          "PatchConnector", "Connectors", "Patch", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchConnectorResponse",
                          handler => SCSPAPI.OnPatchConnectorResponse += handler,
                          handler => SCSPAPI.OnPatchConnectorResponse -= handler,
                          "PatchConnector", "Connectors", "Patch", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteConnectorRequest",
                          handler => SCSPAPI.OnDeleteConnectorRequest += handler,
                          handler => SCSPAPI.OnDeleteConnectorRequest -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteConnectorResponse",
                          handler => SCSPAPI.OnDeleteConnectorResponse += handler,
                          handler => SCSPAPI.OnDeleteConnectorResponse -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Sessions

            RegisterEvent("DeleteSessionsRequest",
                          handler => SCSPAPI.OnDeleteSessionsRequest += handler,
                          handler => SCSPAPI.OnDeleteSessionsRequest -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteSessionsResponse",
                          handler => SCSPAPI.OnDeleteSessionsResponse += handler,
                          handler => SCSPAPI.OnDeleteSessionsResponse -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutSessionRequest",
                          handler => SCSPAPI.OnPutSessionRequest += handler,
                          handler => SCSPAPI.OnPutSessionRequest -= handler,
                          "PutSession", "Sessions", "Put", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutSessionResponse",
                          handler => SCSPAPI.OnPutSessionResponse += handler,
                          handler => SCSPAPI.OnPutSessionResponse -= handler,
                          "PutSession", "Sessions", "Put", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchSessionRequest",
                          handler => SCSPAPI.OnPatchSessionRequest += handler,
                          handler => SCSPAPI.OnPatchSessionRequest -= handler,
                          "PatchSession", "Sessions", "Patch", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchSessionResponse",
                          handler => SCSPAPI.OnPatchSessionResponse += handler,
                          handler => SCSPAPI.OnPatchSessionResponse -= handler,
                          "PatchSession", "Sessions", "Patch", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteSessionRequest",
                          handler => SCSPAPI.OnDeleteSessionRequest += handler,
                          handler => SCSPAPI.OnDeleteSessionRequest -= handler,
                          "DeleteSession", "Sessions", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteSessionResponse",
                          handler => SCSPAPI.OnDeleteSessionResponse += handler,
                          handler => SCSPAPI.OnDeleteSessionResponse -= handler,
                          "DeleteSession", "Sessions", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

        #endregion

    }

}
