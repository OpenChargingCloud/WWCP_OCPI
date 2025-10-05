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
using cloud.charging.open.protocols.OCPIv2_3_0;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.PTP.HTTP
{

    /// <summary>
    /// The OCPI PTP2CPO client.
    /// </summary>
    public partial class PTP2CPOClient : IHTTPClient
    {

        /// <summary>
        /// The OCPI PTP2CPO HTTP client logger.
        /// </summary>
        public new sealed class Logger : CommonClient.Logger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public new const String  DefaultContext   = $"OCPI{Version.String}_PTP2CPOClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached PTP2CPO client.
            /// </summary>
            public PTP2CPOClient  PTP2CPOClient    { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new PTP2CPO client logger using the default logging delegates.
            /// </summary>
            /// <param name="PTP2CPOClient">A PTP2CPO client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public Logger(PTP2CPOClient                   PTP2CPOClient,
                          String?                      LoggingPath,
                          String?                      Context          = DefaultContext,
                          OCPILogfileCreatorDelegate?  LogfileCreator   = null)

                : base(PTP2CPOClient,
                       LoggingPath,
                       Context ?? DefaultContext,
                       LogfileCreator)

            {

                this.PTP2CPOClient = PTP2CPOClient ?? throw new ArgumentNullException(nameof(PTP2CPOClient), "The given PTP2CPO client must not be null!");

                #region Locations

                RegisterEvent("GetLocationsRequest",
                              handler => PTP2CPOClient.OnGetLocationsHTTPRequest += handler,
                              handler => PTP2CPOClient.OnGetLocationsHTTPRequest -= handler,
                              "GetLocations", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationsResponse",
                              handler => PTP2CPOClient.OnGetLocationsHTTPResponse += handler,
                              handler => PTP2CPOClient.OnGetLocationsHTTPResponse -= handler,
                              "GetLocations", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetLocationRequest",
                              handler => PTP2CPOClient.OnGetLocationHTTPRequest += handler,
                              handler => PTP2CPOClient.OnGetLocationHTTPRequest -= handler,
                              "GetLocation", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationResponse",
                              handler => PTP2CPOClient.OnGetLocationHTTPResponse += handler,
                              handler => PTP2CPOClient.OnGetLocationHTTPResponse -= handler,
                              "GetLocation", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetEVSERequest",
                              handler => PTP2CPOClient.OnGetEVSEHTTPRequest += handler,
                              handler => PTP2CPOClient.OnGetEVSEHTTPRequest -= handler,
                              "GetEVSE", "EVSEs", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetEVSEResponse",
                              handler => PTP2CPOClient.OnGetEVSEHTTPResponse += handler,
                              handler => PTP2CPOClient.OnGetEVSEHTTPResponse -= handler,
                              "GetEVSE", "EVSEs", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetConnectorRequest",
                              handler => PTP2CPOClient.OnGetConnectorHTTPRequest += handler,
                              handler => PTP2CPOClient.OnGetConnectorHTTPRequest -= handler,
                              "GetConnector", "connectors", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetConnectorResponse",
                              handler => PTP2CPOClient.OnGetConnectorHTTPResponse += handler,
                              handler => PTP2CPOClient.OnGetConnectorHTTPResponse -= handler,
                              "GetConnector", "connectors", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Tariffs

                RegisterEvent("GetTariffsRequest",
                              handler => PTP2CPOClient.OnGetTariffsHTTPRequest += handler,
                              handler => PTP2CPOClient.OnGetTariffsHTTPRequest -= handler,
                              "GetTariffs", "tariffs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTariffsResponse",
                              handler => PTP2CPOClient.OnGetTariffsHTTPResponse += handler,
                              handler => PTP2CPOClient.OnGetTariffsHTTPResponse -= handler,
                              "GetTariffs", "tariffs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetTariffRequest",
                              handler => PTP2CPOClient.OnGetTariffHTTPRequest += handler,
                              handler => PTP2CPOClient.OnGetTariffHTTPRequest -= handler,
                              "GetTariff", "tariffs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTariffResponse",
                              handler => PTP2CPOClient.OnGetTariffHTTPResponse += handler,
                              handler => PTP2CPOClient.OnGetTariffHTTPResponse -= handler,
                              "GetTariff", "tariffs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Sessions

                RegisterEvent("GetSessionsRequest",
                              handler => PTP2CPOClient.OnGetSessionsHTTPRequest += handler,
                              handler => PTP2CPOClient.OnGetSessionsHTTPRequest -= handler,
                              "GetSessions", "sessions", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionsResponse",
                              handler => PTP2CPOClient.OnGetSessionsHTTPResponse += handler,
                              handler => PTP2CPOClient.OnGetSessionsHTTPResponse -= handler,
                              "GetSessions", "sessions", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetSessionRequest",
                              handler => PTP2CPOClient.OnGetSessionHTTPRequest += handler,
                              handler => PTP2CPOClient.OnGetSessionHTTPRequest -= handler,
                              "GetSession", "sessions", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionResponse",
                              handler => PTP2CPOClient.OnGetSessionHTTPResponse += handler,
                              handler => PTP2CPOClient.OnGetSessionHTTPResponse -= handler,
                              "GetSession", "sessions", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region CDRs

                RegisterEvent("GetCDRsRequest",
                              handler => PTP2CPOClient.OnGetCDRsHTTPRequest += handler,
                              handler => PTP2CPOClient.OnGetCDRsHTTPRequest -= handler,
                              "GetCDRs", "CDRs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCDRsResponse",
                              handler => PTP2CPOClient.OnGetCDRsHTTPResponse += handler,
                              handler => PTP2CPOClient.OnGetCDRsHTTPResponse -= handler,
                              "GetCDRs", "CDRs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetCDRRequest",
                              handler => PTP2CPOClient.OnGetCDRHTTPRequest += handler,
                              handler => PTP2CPOClient.OnGetCDRHTTPRequest -= handler,
                              "GetCDR", "CDRs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCDRResponse",
                              handler => PTP2CPOClient.OnGetCDRHTTPResponse += handler,
                              handler => PTP2CPOClient.OnGetCDRHTTPResponse -= handler,
                              "GetCDR", "CDRs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Tokens

                RegisterEvent("GetTokenRequest",
                              handler => PTP2CPOClient.OnGetTokenHTTPRequest += handler,
                              handler => PTP2CPOClient.OnGetTokenHTTPRequest -= handler,
                              "GetToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTokenResponse",
                              handler => PTP2CPOClient.OnGetTokenHTTPResponse += handler,
                              handler => PTP2CPOClient.OnGetTokenHTTPResponse -= handler,
                              "GetToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutTokenRequest",
                              handler => PTP2CPOClient.OnPutTokenHTTPRequest += handler,
                              handler => PTP2CPOClient.OnPutTokenHTTPRequest -= handler,
                              "PutToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutTokenResponse",
                              handler => PTP2CPOClient.OnPutTokenHTTPResponse += handler,
                              handler => PTP2CPOClient.OnPutTokenHTTPResponse -= handler,
                              "PutToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchTokenRequest",
                              handler => PTP2CPOClient.OnPatchTokenHTTPRequest += handler,
                              handler => PTP2CPOClient.OnPatchTokenHTTPRequest -= handler,
                              "PatchToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchTokenResponse",
                              handler => PTP2CPOClient.OnPatchTokenHTTPResponse += handler,
                              handler => PTP2CPOClient.OnPatchTokenHTTPResponse -= handler,
                              "PatchToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Commands

                RegisterEvent("ReserveNowRequest",
                              handler => PTP2CPOClient.OnReserveNowHTTPRequest += handler,
                              handler => PTP2CPOClient.OnReserveNowHTTPRequest -= handler,
                              "ReserveNow", "reservations", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("ReserveNowResponse",
                              handler => PTP2CPOClient.OnReserveNowHTTPResponse += handler,
                              handler => PTP2CPOClient.OnReserveNowHTTPResponse -= handler,
                              "ReserveNow", "reservations", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("CancelReservationRequest",
                              handler => PTP2CPOClient.OnCancelReservationHTTPRequest += handler,
                              handler => PTP2CPOClient.OnCancelReservationHTTPRequest -= handler,
                              "CancelReservation", "reservations", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("CancelReservationResponse",
                              handler => PTP2CPOClient.OnCancelReservationHTTPResponse += handler,
                              handler => PTP2CPOClient.OnCancelReservationHTTPResponse -= handler,
                              "CancelReservation", "reservations", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);



                RegisterEvent("StartSessionRequest",
                              handler => PTP2CPOClient.OnStartSessionHTTPRequest += handler,
                              handler => PTP2CPOClient.OnStartSessionHTTPRequest -= handler,
                              "StartSession", "StartStopSessions", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("StartSessionResponse",
                              handler => PTP2CPOClient.OnStartSessionHTTPResponse += handler,
                              handler => PTP2CPOClient.OnStartSessionHTTPResponse -= handler,
                              "StartSession", "StartStopSessions", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("StopSessionRequest",
                              handler => PTP2CPOClient.OnStopSessionHTTPRequest += handler,
                              handler => PTP2CPOClient.OnStopSessionHTTPRequest -= handler,
                              "StopSession", "StopStopSessions", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("StopSessionResponse",
                              handler => PTP2CPOClient.OnStopSessionHTTPResponse += handler,
                              handler => PTP2CPOClient.OnStopSessionHTTPResponse -= handler,
                              "StopSession", "StopStopSessions", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);



                RegisterEvent("UnlockConnectorRequest",
                              handler => PTP2CPOClient.OnUnlockConnectorHTTPRequest += handler,
                              handler => PTP2CPOClient.OnUnlockConnectorHTTPRequest -= handler,
                              "UnlockConnector", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("UnlockConnectorResponse",
                              handler => PTP2CPOClient.OnUnlockConnectorHTTPResponse += handler,
                              handler => PTP2CPOClient.OnUnlockConnectorHTTPResponse -= handler,
                              "UnlockConnector", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

            }

            #endregion

        }

     }

}
