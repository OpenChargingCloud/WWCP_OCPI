/*
 * Copyright (c) 2015-2021 GraphDefined GmbH
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
    /// The OCPI EMSP client.
    /// </summary>
    public partial class EMSPClient : IHTTPClient
    {

        /// <summary>
        /// The OCPI EMSP HTTP client logger.
        /// </summary>
        public new class Logger : CommonClient.Logger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public const String  DefaultContext  = "OCPIEMSPClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached EMSP client.
            /// </summary>
            public EMSPClient  EMSPClient    { get; }

            #endregion

            #region Constructor(s)

            #region Logger(EMSPClient, Context = DefaultContext, LogfileCreator = null)

            /// <summary>
            /// Create a new EMSP client logger using the default logging delegates.
            /// </summary>
            /// <param name="EMSPClient">A EMSP client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public Logger(EMSPClient              EMSPClient,
                          String                  LoggingPath,
                          String                  Context         = DefaultContext,
                          LogfileCreatorDelegate  LogfileCreator  = null)

                : this(EMSPClient,
                       LoggingPath,
                       Context.IsNotNullOrEmpty() ? Context : DefaultContext,
                       null,
                       null,
                       null,
                       null,

                       LogfileCreator: LogfileCreator)

            { }

            #endregion

            #region Logger(EMSPClient, Context, ... Logging delegates ...)

            /// <summary>
            /// Create a new EMSP client logger using the given logging delegates.
            /// </summary>
            /// <param name="EMSPClient">A EMSP client.</param>
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
            public Logger(EMSPClient                  EMSPClient,
                          String                      LoggingPath,
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

                : base(EMSPClient,
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

                this.EMSPClient = EMSPClient ?? throw new ArgumentNullException(nameof(EMSPClient), "The given EMSP client must not be null!");

                #region Locations

                RegisterEvent("GetLocationsRequest",
                              handler => EMSPClient.OnGetLocationsHTTPRequest += handler,
                              handler => EMSPClient.OnGetLocationsHTTPRequest -= handler,
                              "GetLocations", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationsResponse",
                              handler => EMSPClient.OnGetLocationsHTTPResponse += handler,
                              handler => EMSPClient.OnGetLocationsHTTPResponse -= handler,
                              "GetLocations", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetLocationByIdRequest",
                              handler => EMSPClient.OnGetLocationByIdHTTPRequest += handler,
                              handler => EMSPClient.OnGetLocationByIdHTTPRequest -= handler,
                              "GetLocationById", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationByIdResponse",
                              handler => EMSPClient.OnGetLocationByIdHTTPResponse += handler,
                              handler => EMSPClient.OnGetLocationByIdHTTPResponse -= handler,
                              "GetLocationById", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetEVSEByUIdRequest",
                              handler => EMSPClient.OnGetEVSEByUIdHTTPRequest += handler,
                              handler => EMSPClient.OnGetEVSEByUIdHTTPRequest -= handler,
                              "GetEVSEByUId", "EVSEs", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetEVSEByUIdResponse",
                              handler => EMSPClient.OnGetEVSEByUIdHTTPResponse += handler,
                              handler => EMSPClient.OnGetEVSEByUIdHTTPResponse -= handler,
                              "GetEVSEByUId", "EVSEs", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetConnectorByIdRequest",
                              handler => EMSPClient.OnGetConnectorByIdHTTPRequest += handler,
                              handler => EMSPClient.OnGetConnectorByIdHTTPRequest -= handler,
                              "GetConnectorById", "connectors", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetConnectorByIdResponse",
                              handler => EMSPClient.OnGetConnectorByIdHTTPResponse += handler,
                              handler => EMSPClient.OnGetConnectorByIdHTTPResponse -= handler,
                              "GetConnectorById", "connectors", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Tariffs

                RegisterEvent("GetTariffsRequest",
                              handler => EMSPClient.OnGetTariffsHTTPRequest += handler,
                              handler => EMSPClient.OnGetTariffsHTTPRequest -= handler,
                              "GetTariffs", "tariffs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTariffsResponse",
                              handler => EMSPClient.OnGetTariffsHTTPResponse += handler,
                              handler => EMSPClient.OnGetTariffsHTTPResponse -= handler,
                              "GetTariffs", "tariffs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetTariffByIdRequest",
                              handler => EMSPClient.OnGetTariffByIdHTTPRequest += handler,
                              handler => EMSPClient.OnGetTariffByIdHTTPRequest -= handler,
                              "GetTariffById", "tariffs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTariffByIdResponse",
                              handler => EMSPClient.OnGetTariffByIdHTTPResponse += handler,
                              handler => EMSPClient.OnGetTariffByIdHTTPResponse -= handler,
                              "GetTariffById", "tariffs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Sessions

                RegisterEvent("GetSessionsRequest",
                              handler => EMSPClient.OnGetSessionsHTTPRequest += handler,
                              handler => EMSPClient.OnGetSessionsHTTPRequest -= handler,
                              "GetSessions", "sessions", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionsResponse",
                              handler => EMSPClient.OnGetSessionsHTTPResponse += handler,
                              handler => EMSPClient.OnGetSessionsHTTPResponse -= handler,
                              "GetSessions", "sessions", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetSessionByIdRequest",
                              handler => EMSPClient.OnGetSessionByIdHTTPRequest += handler,
                              handler => EMSPClient.OnGetSessionByIdHTTPRequest -= handler,
                              "GetSessionById", "sessions", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionByIdResponse",
                              handler => EMSPClient.OnGetSessionByIdHTTPResponse += handler,
                              handler => EMSPClient.OnGetSessionByIdHTTPResponse -= handler,
                              "GetSessionById", "sessions", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region CDRs

                RegisterEvent("GetCDRsRequest",
                              handler => EMSPClient.OnGetCDRsHTTPRequest += handler,
                              handler => EMSPClient.OnGetCDRsHTTPRequest -= handler,
                              "GetCDRs", "CDRs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCDRsResponse",
                              handler => EMSPClient.OnGetCDRsHTTPResponse += handler,
                              handler => EMSPClient.OnGetCDRsHTTPResponse -= handler,
                              "GetCDRs", "CDRs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetCDRByIdRequest",
                              handler => EMSPClient.OnGetCDRByIdHTTPRequest += handler,
                              handler => EMSPClient.OnGetCDRByIdHTTPRequest -= handler,
                              "GetCDRById", "CDRs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCDRByIdResponse",
                              handler => EMSPClient.OnGetCDRByIdHTTPResponse += handler,
                              handler => EMSPClient.OnGetCDRByIdHTTPResponse -= handler,
                              "GetCDRById", "CDRs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Tokens

                RegisterEvent("GetTokenRequest",
                              handler => EMSPClient.OnGetTokenHTTPRequest += handler,
                              handler => EMSPClient.OnGetTokenHTTPRequest -= handler,
                              "GetToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTokenResponse",
                              handler => EMSPClient.OnGetTokenHTTPResponse += handler,
                              handler => EMSPClient.OnGetTokenHTTPResponse -= handler,
                              "GetToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutTokenRequest",
                              handler => EMSPClient.OnPutTokenHTTPRequest += handler,
                              handler => EMSPClient.OnPutTokenHTTPRequest -= handler,
                              "PutToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutTokenResponse",
                              handler => EMSPClient.OnPutTokenHTTPResponse += handler,
                              handler => EMSPClient.OnPutTokenHTTPResponse -= handler,
                              "PutToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchTokenRequest",
                              handler => EMSPClient.OnPatchTokenHTTPRequest += handler,
                              handler => EMSPClient.OnPatchTokenHTTPRequest -= handler,
                              "PatchToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchTokenResponse",
                              handler => EMSPClient.OnPatchTokenHTTPResponse += handler,
                              handler => EMSPClient.OnPatchTokenHTTPResponse -= handler,
                              "PatchToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Commands

                RegisterEvent("ReserveNowRequest",
                              handler => EMSPClient.OnReserveNowHTTPRequest += handler,
                              handler => EMSPClient.OnReserveNowHTTPRequest -= handler,
                              "ReserveNow", "reservations", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("ReserveNowResponse",
                              handler => EMSPClient.OnReserveNowHTTPResponse += handler,
                              handler => EMSPClient.OnReserveNowHTTPResponse -= handler,
                              "ReserveNow", "reservations", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("CancelReservationRequest",
                              handler => EMSPClient.OnCancelReservationHTTPRequest += handler,
                              handler => EMSPClient.OnCancelReservationHTTPRequest -= handler,
                              "CancelReservation", "reservations", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("CancelReservationResponse",
                              handler => EMSPClient.OnCancelReservationHTTPResponse += handler,
                              handler => EMSPClient.OnCancelReservationHTTPResponse -= handler,
                              "CancelReservation", "reservations", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);



                RegisterEvent("StartSessionRequest",
                              handler => EMSPClient.OnStartSessionHTTPRequest += handler,
                              handler => EMSPClient.OnStartSessionHTTPRequest -= handler,
                              "StartSession", "StartStopSessions", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("StartSessionResponse",
                              handler => EMSPClient.OnStartSessionHTTPResponse += handler,
                              handler => EMSPClient.OnStartSessionHTTPResponse -= handler,
                              "StartSession", "StartStopSessions", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("StopSessionRequest",
                              handler => EMSPClient.OnStopSessionHTTPRequest += handler,
                              handler => EMSPClient.OnStopSessionHTTPRequest -= handler,
                              "StopSession", "StopStopSessions", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("StopSessionResponse",
                              handler => EMSPClient.OnStopSessionHTTPResponse += handler,
                              handler => EMSPClient.OnStopSessionHTTPResponse -= handler,
                              "StopSession", "StopStopSessions", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);



                RegisterEvent("UnlockConnectorRequest",
                              handler => EMSPClient.OnUnlockConnectorHTTPRequest += handler,
                              handler => EMSPClient.OnUnlockConnectorHTTPRequest -= handler,
                              "UnlockConnector", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("UnlockConnectorResponse",
                              handler => EMSPClient.OnUnlockConnectorHTTPResponse += handler,
                              handler => EMSPClient.OnUnlockConnectorHTTPResponse -= handler,
                              "UnlockConnector", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

            }

            #endregion

            #endregion

        }

     }

}
