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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.EMSP.HTTP
{

    /// <summary>
    /// The OCPI EMSP client.
    /// </summary>
    public partial class EMSPClient : IHTTPClient
    {

        /// <summary>
        /// The OCPI EMSP HTTP client logger.
        /// </summary>
        public new sealed class Logger : CommonClient.Logger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public new const String  DefaultContext   = $"OCPI{Version.String}_EMSPClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached EMSP client.
            /// </summary>
            public EMSPClient  EMSPClient    { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new EMSP client logger using the default logging delegates.
            /// </summary>
            /// <param name="EMSPClient">A EMSP client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public Logger(EMSPClient                   EMSPClient,
                          String?                      LoggingPath,
                          String?                      Context          = DefaultContext,
                          OCPILogfileCreatorDelegate?  LogfileCreator   = null)

                : base(EMSPClient,
                       LoggingPath,
                       Context ?? DefaultContext,
                       LogfileCreator is not null
                           ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                           : null)

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


                RegisterEvent("GetLocationRequest",
                              handler => EMSPClient.OnGetLocationHTTPRequest += handler,
                              handler => EMSPClient.OnGetLocationHTTPRequest -= handler,
                              "GetLocation", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationResponse",
                              handler => EMSPClient.OnGetLocationHTTPResponse += handler,
                              handler => EMSPClient.OnGetLocationHTTPResponse -= handler,
                              "GetLocation", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetEVSERequest",
                              handler => EMSPClient.OnGetEVSEHTTPRequest += handler,
                              handler => EMSPClient.OnGetEVSEHTTPRequest -= handler,
                              "GetEVSE", "EVSEs", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetEVSEResponse",
                              handler => EMSPClient.OnGetEVSEHTTPResponse += handler,
                              handler => EMSPClient.OnGetEVSEHTTPResponse -= handler,
                              "GetEVSE", "EVSEs", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetConnectorRequest",
                              handler => EMSPClient.OnGetConnectorHTTPRequest += handler,
                              handler => EMSPClient.OnGetConnectorHTTPRequest -= handler,
                              "GetConnector", "connectors", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetConnectorResponse",
                              handler => EMSPClient.OnGetConnectorHTTPResponse += handler,
                              handler => EMSPClient.OnGetConnectorHTTPResponse -= handler,
                              "GetConnector", "connectors", "locations", "responses", "all").
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


                RegisterEvent("GetTariffRequest",
                              handler => EMSPClient.OnGetTariffHTTPRequest += handler,
                              handler => EMSPClient.OnGetTariffHTTPRequest -= handler,
                              "GetTariff", "tariffs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTariffResponse",
                              handler => EMSPClient.OnGetTariffHTTPResponse += handler,
                              handler => EMSPClient.OnGetTariffHTTPResponse -= handler,
                              "GetTariff", "tariffs", "responses", "all").
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


                RegisterEvent("GetSessionRequest",
                              handler => EMSPClient.OnGetSessionHTTPRequest += handler,
                              handler => EMSPClient.OnGetSessionHTTPRequest -= handler,
                              "GetSession", "sessions", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionResponse",
                              handler => EMSPClient.OnGetSessionHTTPResponse += handler,
                              handler => EMSPClient.OnGetSessionHTTPResponse -= handler,
                              "GetSession", "sessions", "responses", "all").
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


                RegisterEvent("GetCDRRequest",
                              handler => EMSPClient.OnGetCDRHTTPRequest += handler,
                              handler => EMSPClient.OnGetCDRHTTPRequest -= handler,
                              "GetCDR", "CDRs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCDRResponse",
                              handler => EMSPClient.OnGetCDRHTTPResponse += handler,
                              handler => EMSPClient.OnGetCDRHTTPResponse -= handler,
                              "GetCDR", "CDRs", "responses", "all").
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

        }

     }

}
