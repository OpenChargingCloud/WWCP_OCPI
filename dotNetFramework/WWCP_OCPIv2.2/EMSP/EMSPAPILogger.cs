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

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// A EMSP API logger.
    /// </summary>
    public class EMSPAPILogger : OCPIAPILogger
    {

        #region Data

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public const String DefaultContext = "EMSPAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked EMSP API.
        /// </summary>
        public EMSPAPI  EMSPAPI  { get; }

        #endregion

        #region Constructor(s)

        #region EMSPAPILogger(EMSPAPI, Context = DefaultContext, LogFileCreator = null)

        /// <summary>
        /// Create a new EMSP API logger using the default logging delegates.
        /// </summary>
        /// <param name="EMSPAPI">An EMSP API.</param>
        /// <param name="Context">A context of this API.</param>
        /// <param name="LogFileCreator">A delegate to create a log file from the given context and log file name.</param>
        public EMSPAPILogger(EMSPAPI                EMSPAPI,
                             String                  Context         = DefaultContext,
                             LogfileCreatorDelegate  LogFileCreator  = null)

            : this(EMSPAPI,
                   Context,
                   null,
                   null,
                   null,
                   null,
                   LogFileCreator: LogFileCreator)

        { }

        #endregion

        #region EMSPAPILogger(EMSPAPI, Context, ... Logging delegates ...)

        /// <summary>
        /// Create a new EMSP API logger using the given logging delegates.
        /// </summary>
        /// <param name="EMSPAPI">An EMSP API.</param>
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
        public EMSPAPILogger(EMSPAPI                     EMSPAPI,
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

            : base(EMSPAPI.HTTPServer,
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

            this.EMSPAPI = EMSPAPI ?? throw new ArgumentNullException(nameof(EMSPAPI), "The given EMSP API must not be null!");

            #region Locations

            RegisterEvent("DeleteLocationsRequest",
                          handler => EMSPAPI.OnDeleteLocationsRequest += handler,
                          handler => EMSPAPI.OnDeleteLocationsRequest -= handler,
                          "DeleteLocations", "Locations", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteLocationsResponse",
                          handler => EMSPAPI.OnDeleteLocationsResponse += handler,
                          handler => EMSPAPI.OnDeleteLocationsResponse -= handler,
                          "DeleteLocations", "Locations", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutLocationRequest",
                          handler => EMSPAPI.OnPutLocationRequest += handler,
                          handler => EMSPAPI.OnPutLocationRequest -= handler,
                          "PutLocation", "Locations", "Put", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutLocationResponse",
                          handler => EMSPAPI.OnPutLocationResponse += handler,
                          handler => EMSPAPI.OnPutLocationResponse -= handler,
                          "PutLocation", "Locations", "Put", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchLocationRequest",
                          handler => EMSPAPI.OnPatchLocationRequest += handler,
                          handler => EMSPAPI.OnPatchLocationRequest -= handler,
                          "PatchLocation", "Locations", "Patch", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchLocationResponse",
                          handler => EMSPAPI.OnPatchLocationResponse += handler,
                          handler => EMSPAPI.OnPatchLocationResponse -= handler,
                          "PatchLocation", "Locations", "Patch", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteLocationRequest",
                          handler => EMSPAPI.OnDeleteLocationRequest += handler,
                          handler => EMSPAPI.OnDeleteLocationRequest -= handler,
                          "DeleteLocation", "Locations", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteLocationResponse",
                          handler => EMSPAPI.OnDeleteLocationResponse += handler,
                          handler => EMSPAPI.OnDeleteLocationResponse -= handler,
                          "DeleteLocation", "Locations", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region EVSEs

            RegisterEvent("PutEVSERequest",
                          handler => EMSPAPI.OnPutEVSERequest += handler,
                          handler => EMSPAPI.OnPutEVSERequest -= handler,
                          "PutEVSE", "EVSEs", "Put", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutEVSEResponse",
                          handler => EMSPAPI.OnPutEVSEResponse += handler,
                          handler => EMSPAPI.OnPutEVSEResponse -= handler,
                          "PutEVSE", "EVSEs", "Put", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchEVSERequest",
                          handler => EMSPAPI.OnPatchEVSERequest += handler,
                          handler => EMSPAPI.OnPatchEVSERequest -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchEVSEResponse",
                          handler => EMSPAPI.OnPatchEVSEResponse += handler,
                          handler => EMSPAPI.OnPatchEVSEResponse -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteEVSERequest",
                          handler => EMSPAPI.OnDeleteEVSERequest += handler,
                          handler => EMSPAPI.OnDeleteEVSERequest -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteEVSEResponse",
                          handler => EMSPAPI.OnDeleteEVSEResponse += handler,
                          handler => EMSPAPI.OnDeleteEVSEResponse -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Connectors

            RegisterEvent("PutConnectorRequest",
                          handler => EMSPAPI.OnPutConnectorRequest += handler,
                          handler => EMSPAPI.OnPutConnectorRequest -= handler,
                          "PutConnector", "Connectors", "Put", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutConnectorResponse",
                          handler => EMSPAPI.OnPutConnectorResponse += handler,
                          handler => EMSPAPI.OnPutConnectorResponse -= handler,
                          "PutConnector", "Connectors", "Put", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchConnectorRequest",
                          handler => EMSPAPI.OnPatchConnectorRequest += handler,
                          handler => EMSPAPI.OnPatchConnectorRequest -= handler,
                          "PatchConnector", "Connectors", "Patch", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchConnectorResponse",
                          handler => EMSPAPI.OnPatchConnectorResponse += handler,
                          handler => EMSPAPI.OnPatchConnectorResponse -= handler,
                          "PatchConnector", "Connectors", "Patch", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteConnectorRequest",
                          handler => EMSPAPI.OnDeleteConnectorRequest += handler,
                          handler => EMSPAPI.OnDeleteConnectorRequest -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteConnectorResponse",
                          handler => EMSPAPI.OnDeleteConnectorResponse += handler,
                          handler => EMSPAPI.OnDeleteConnectorResponse -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Tariffs

            RegisterEvent("PutTariffRequest",
                          handler => EMSPAPI.OnPutTariffRequest += handler,
                          handler => EMSPAPI.OnPutTariffRequest -= handler,
                          "PutTariff", "Tariffs", "Put", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutTariffResponse",
                          handler => EMSPAPI.OnPutTariffResponse += handler,
                          handler => EMSPAPI.OnPutTariffResponse -= handler,
                          "PutTariff", "Tariffs", "Put", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteTariffRequest",
                          handler => EMSPAPI.OnDeleteTariffRequest += handler,
                          handler => EMSPAPI.OnDeleteTariffRequest -= handler,
                          "DeleteTariff", "Tariffs", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteTariffResponse",
                          handler => EMSPAPI.OnDeleteTariffResponse += handler,
                          handler => EMSPAPI.OnDeleteTariffResponse -= handler,
                          "DeleteTariff", "Tariffs", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Sessions

            RegisterEvent("DeleteSessionsRequest",
                          handler => EMSPAPI.OnDeleteSessionsRequest += handler,
                          handler => EMSPAPI.OnDeleteSessionsRequest -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteSessionsResponse",
                          handler => EMSPAPI.OnDeleteSessionsResponse += handler,
                          handler => EMSPAPI.OnDeleteSessionsResponse -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutSessionRequest",
                          handler => EMSPAPI.OnPutSessionRequest += handler,
                          handler => EMSPAPI.OnPutSessionRequest -= handler,
                          "PutSession", "Sessions", "Put", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutSessionResponse",
                          handler => EMSPAPI.OnPutSessionResponse += handler,
                          handler => EMSPAPI.OnPutSessionResponse -= handler,
                          "PutSession", "Sessions", "Put", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchSessionRequest",
                          handler => EMSPAPI.OnPatchSessionRequest += handler,
                          handler => EMSPAPI.OnPatchSessionRequest -= handler,
                          "PatchSession", "Sessions", "Patch", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchSessionResponse",
                          handler => EMSPAPI.OnPatchSessionResponse += handler,
                          handler => EMSPAPI.OnPatchSessionResponse -= handler,
                          "PatchSession", "Sessions", "Patch", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteSessionRequest",
                          handler => EMSPAPI.OnDeleteSessionRequest += handler,
                          handler => EMSPAPI.OnDeleteSessionRequest -= handler,
                          "DeleteSession", "Sessions", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteSessionResponse",
                          handler => EMSPAPI.OnDeleteSessionResponse += handler,
                          handler => EMSPAPI.OnDeleteSessionResponse -= handler,
                          "DeleteSession", "Sessions", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region CDRs

            RegisterEvent("DeleteCDRsRequest",
                          handler => EMSPAPI.OnDeleteCDRsRequest += handler,
                          handler => EMSPAPI.OnDeleteCDRsRequest -= handler,
                          "DeleteCDRs", "CDRs", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteCDRsResponse",
                          handler => EMSPAPI.OnDeleteCDRsResponse += handler,
                          handler => EMSPAPI.OnDeleteCDRsResponse -= handler,
                          "DeleteCDRs", "CDRs", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PostCDRRequest",
                          handler => EMSPAPI.OnPostCDRRequest += handler,
                          handler => EMSPAPI.OnPostCDRRequest -= handler,
                          "PostCDR", "CDRs", "Post", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PostCDRResponse",
                          handler => EMSPAPI.OnPostCDRResponse += handler,
                          handler => EMSPAPI.OnPostCDRResponse -= handler,
                          "PostCDR", "CDRs", "Post", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteCDRRequest",
                          handler => EMSPAPI.OnDeleteCDRRequest += handler,
                          handler => EMSPAPI.OnDeleteCDRRequest -= handler,
                          "DeleteCDR", "CDRs", "Delete", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteCDRResponse",
                          handler => EMSPAPI.OnDeleteCDRResponse += handler,
                          handler => EMSPAPI.OnDeleteCDRResponse -= handler,
                          "DeleteCDR", "CDRs", "Delete", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("GetCDRRequest",
                          handler => EMSPAPI.OnGetCDRRequest += handler,
                          handler => EMSPAPI.OnGetCDRRequest -= handler,
                          "GetCDR", "CDRs", "Get", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetCDRResponse",
                          handler => EMSPAPI.OnGetCDRResponse += handler,
                          handler => EMSPAPI.OnGetCDRResponse -= handler,
                          "GetCDR", "CDRs", "Get", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Tokens

            RegisterEvent("PostTokenRequest",
                          handler => EMSPAPI.OnPostTokenRequest += handler,
                          handler => EMSPAPI.OnPostTokenRequest -= handler,
                          "PostToken", "Tokens", "Post", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PostTokenResponse",
                          handler => EMSPAPI.OnPostTokenResponse += handler,
                          handler => EMSPAPI.OnPostTokenResponse -= handler,
                          "PostToken", "Tokens", "Post", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Command callbacks

            RegisterEvent("ReserveNowCallbackRequest",
                          handler => EMSPAPI.OnReserveNowCallbackRequest += handler,
                          handler => EMSPAPI.OnReserveNowCallbackRequest -= handler,
                          "ReserveNowCallback", "ReservationCallbacks", "Callbacks", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("ReserveNowCallbackResponse",
                          handler => EMSPAPI.OnReserveNowCallbackResponse += handler,
                          handler => EMSPAPI.OnReserveNowCallbackResponse -= handler,
                          "ReserveNowCallback", "ReservationCallbacks", "Callbacks", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("CancelReservationCallbackRequest",
                          handler => EMSPAPI.OnCancelReservationCallbackRequest += handler,
                          handler => EMSPAPI.OnCancelReservationCallbackRequest -= handler,
                          "CancelReservationCallback", "ReservationCallbacks", "Callbacks", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CancelReservationCallbackResponse",
                          handler => EMSPAPI.OnCancelReservationCallbackResponse += handler,
                          handler => EMSPAPI.OnCancelReservationCallbackResponse -= handler,
                          "CancelReservationCallback", "ReservationCallbacks", "Callbacks", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("StartSessionCallbackRequest",
                          handler => EMSPAPI.OnStartSessionCallbackRequest += handler,
                          handler => EMSPAPI.OnStartSessionCallbackRequest -= handler,
                          "StartSessionCallback", "SessionCallbacks", "Callbacks", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("StartSessionCallbackResponse",
                          handler => EMSPAPI.OnStartSessionCallbackResponse += handler,
                          handler => EMSPAPI.OnStartSessionCallbackResponse -= handler,
                          "StartSessionCallback", "SessionCallbacks", "Callbacks", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("StopSessionCallbackRequest",
                          handler => EMSPAPI.OnStopSessionCallbackRequest += handler,
                          handler => EMSPAPI.OnStopSessionCallbackRequest -= handler,
                          "StopSessionCallback", "SessionCallbacks", "Callbacks", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("StopSessionCallbackResponse",
                          handler => EMSPAPI.OnStopSessionCallbackResponse += handler,
                          handler => EMSPAPI.OnStopSessionCallbackResponse -= handler,
                          "StopSessionCallback", "SessionCallbacks", "CaCallbackslback", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("UnlockConnectorCallbackRequest",
                          handler => EMSPAPI.OnUnlockConnectorCallbackRequest += handler,
                          handler => EMSPAPI.OnUnlockConnectorCallbackRequest -= handler,
                          "UnlockConnectorCallback", "Callbacks", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("UnlockConnectorCallbackResponse",
                          handler => EMSPAPI.OnUnlockConnectorCallbackResponse += handler,
                          handler => EMSPAPI.OnUnlockConnectorCallbackResponse -= handler,
                          "UnlockConnectorCallback", "Callbacks", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

        #endregion

    }

}
