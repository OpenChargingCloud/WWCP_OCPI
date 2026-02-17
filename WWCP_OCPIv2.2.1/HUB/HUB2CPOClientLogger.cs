/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using cloud.charging.open.protocols.OCPIv2_2_1;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.HUB.HTTP
{

    /// <summary>
    /// The OCPI HUB client.
    /// </summary>
    public partial class HUB2CPOClient : IHTTPClient
    {

        /// <summary>
        /// The OCPI HUB HTTP client logger.
        /// </summary>
        public new sealed class Logger : CommonHTTPClient.HTTPClientLogger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public new const String  DefaultContext   = $"OCPI{Version.String}_HUB-2-CPOClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached HUB-2-CPO client.
            /// </summary>
            public HUB2CPOClient  HUB2CPOClient    { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new HUB-2-CPO client logger using the default logging delegates.
            /// </summary>
            /// <param name="HUB2CPOClient">An HUB-2-CPO client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public Logger(HUB2CPOClient                HUB2CPOClient,
                          String?                      LoggingPath,
                          String?                      Context          = DefaultContext,
                          OCPILogfileCreatorDelegate?  LogfileCreator   = null)

                : base(HUB2CPOClient,
                       LoggingPath,
                       Context ?? DefaultContext,
                       LogfileCreator)

            {

                this.HUB2CPOClient = HUB2CPOClient ?? throw new ArgumentNullException(nameof(HUB2CPOClient), "The given HUB-2-CPO client must not be null!");

                #region Locations

                RegisterEvent("GetLocationsRequest",
                              handler => HUB2CPOClient.OnGetLocationsHTTPRequest += handler,
                              handler => HUB2CPOClient.OnGetLocationsHTTPRequest -= handler,
                              "GetLocations", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationsResponse",
                              handler => HUB2CPOClient.OnGetLocationsHTTPResponse += handler,
                              handler => HUB2CPOClient.OnGetLocationsHTTPResponse -= handler,
                              "GetLocations", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetLocationRequest",
                              handler => HUB2CPOClient.OnGetLocationHTTPRequest += handler,
                              handler => HUB2CPOClient.OnGetLocationHTTPRequest -= handler,
                              "GetLocation", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationResponse",
                              handler => HUB2CPOClient.OnGetLocationHTTPResponse += handler,
                              handler => HUB2CPOClient.OnGetLocationHTTPResponse -= handler,
                              "GetLocation", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetEVSERequest",
                              handler => HUB2CPOClient.OnGetEVSEHTTPRequest += handler,
                              handler => HUB2CPOClient.OnGetEVSEHTTPRequest -= handler,
                              "GetEVSE", "EVSEs", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetEVSEResponse",
                              handler => HUB2CPOClient.OnGetEVSEHTTPResponse += handler,
                              handler => HUB2CPOClient.OnGetEVSEHTTPResponse -= handler,
                              "GetEVSE", "EVSEs", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetConnectorRequest",
                              handler => HUB2CPOClient.OnGetConnectorHTTPRequest += handler,
                              handler => HUB2CPOClient.OnGetConnectorHTTPRequest -= handler,
                              "GetConnector", "connectors", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetConnectorResponse",
                              handler => HUB2CPOClient.OnGetConnectorHTTPResponse += handler,
                              handler => HUB2CPOClient.OnGetConnectorHTTPResponse -= handler,
                              "GetConnector", "connectors", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Tariffs

                RegisterEvent("GetTariffsRequest",
                              handler => HUB2CPOClient.OnGetTariffsHTTPRequest += handler,
                              handler => HUB2CPOClient.OnGetTariffsHTTPRequest -= handler,
                              "GetTariffs", "tariffs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTariffsResponse",
                              handler => HUB2CPOClient.OnGetTariffsHTTPResponse += handler,
                              handler => HUB2CPOClient.OnGetTariffsHTTPResponse -= handler,
                              "GetTariffs", "tariffs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetTariffRequest",
                              handler => HUB2CPOClient.OnGetTariffHTTPRequest += handler,
                              handler => HUB2CPOClient.OnGetTariffHTTPRequest -= handler,
                              "GetTariff", "tariffs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTariffResponse",
                              handler => HUB2CPOClient.OnGetTariffHTTPResponse += handler,
                              handler => HUB2CPOClient.OnGetTariffHTTPResponse -= handler,
                              "GetTariff", "tariffs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Sessions

                RegisterEvent("GetSessionsRequest",
                              handler => HUB2CPOClient.OnGetSessionsHTTPRequest += handler,
                              handler => HUB2CPOClient.OnGetSessionsHTTPRequest -= handler,
                              "GetSessions", "sessions", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionsResponse",
                              handler => HUB2CPOClient.OnGetSessionsHTTPResponse += handler,
                              handler => HUB2CPOClient.OnGetSessionsHTTPResponse -= handler,
                              "GetSessions", "sessions", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetSessionRequest",
                              handler => HUB2CPOClient.OnGetSessionHTTPRequest += handler,
                              handler => HUB2CPOClient.OnGetSessionHTTPRequest -= handler,
                              "GetSession", "sessions", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionResponse",
                              handler => HUB2CPOClient.OnGetSessionHTTPResponse += handler,
                              handler => HUB2CPOClient.OnGetSessionHTTPResponse -= handler,
                              "GetSession", "sessions", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region CDRs

                RegisterEvent("GetCDRsRequest",
                              handler => HUB2CPOClient.OnGetCDRsHTTPRequest += handler,
                              handler => HUB2CPOClient.OnGetCDRsHTTPRequest -= handler,
                              "GetCDRs", "CDRs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCDRsResponse",
                              handler => HUB2CPOClient.OnGetCDRsHTTPResponse += handler,
                              handler => HUB2CPOClient.OnGetCDRsHTTPResponse -= handler,
                              "GetCDRs", "CDRs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetCDRRequest",
                              handler => HUB2CPOClient.OnGetCDRHTTPRequest += handler,
                              handler => HUB2CPOClient.OnGetCDRHTTPRequest -= handler,
                              "GetCDR", "CDRs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCDRResponse",
                              handler => HUB2CPOClient.OnGetCDRHTTPResponse += handler,
                              handler => HUB2CPOClient.OnGetCDRHTTPResponse -= handler,
                              "GetCDR", "CDRs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Tokens

                RegisterEvent("GetTokenRequest",
                              handler => HUB2CPOClient.OnGetTokenHTTPRequest += handler,
                              handler => HUB2CPOClient.OnGetTokenHTTPRequest -= handler,
                              "GetToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTokenResponse",
                              handler => HUB2CPOClient.OnGetTokenHTTPResponse += handler,
                              handler => HUB2CPOClient.OnGetTokenHTTPResponse -= handler,
                              "GetToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutTokenRequest",
                              handler => HUB2CPOClient.OnPutTokenHTTPRequest += handler,
                              handler => HUB2CPOClient.OnPutTokenHTTPRequest -= handler,
                              "PutToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutTokenResponse",
                              handler => HUB2CPOClient.OnPutTokenHTTPResponse += handler,
                              handler => HUB2CPOClient.OnPutTokenHTTPResponse -= handler,
                              "PutToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchTokenRequest",
                              handler => HUB2CPOClient.OnPatchTokenHTTPRequest += handler,
                              handler => HUB2CPOClient.OnPatchTokenHTTPRequest -= handler,
                              "PatchToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchTokenResponse",
                              handler => HUB2CPOClient.OnPatchTokenHTTPResponse += handler,
                              handler => HUB2CPOClient.OnPatchTokenHTTPResponse -= handler,
                              "PatchToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Commands

                RegisterEvent("ReserveNowRequest",
                              handler => HUB2CPOClient.OnReserveNowHTTPRequest += handler,
                              handler => HUB2CPOClient.OnReserveNowHTTPRequest -= handler,
                              "ReserveNow", "reservations", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("ReserveNowResponse",
                              handler => HUB2CPOClient.OnReserveNowHTTPResponse += handler,
                              handler => HUB2CPOClient.OnReserveNowHTTPResponse -= handler,
                              "ReserveNow", "reservations", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("CancelReservationRequest",
                              handler => HUB2CPOClient.OnCancelReservationHTTPRequest += handler,
                              handler => HUB2CPOClient.OnCancelReservationHTTPRequest -= handler,
                              "CancelReservation", "reservations", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("CancelReservationResponse",
                              handler => HUB2CPOClient.OnCancelReservationHTTPResponse += handler,
                              handler => HUB2CPOClient.OnCancelReservationHTTPResponse -= handler,
                              "CancelReservation", "reservations", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);



                RegisterEvent("StartSessionRequest",
                              handler => HUB2CPOClient.OnStartSessionHTTPRequest += handler,
                              handler => HUB2CPOClient.OnStartSessionHTTPRequest -= handler,
                              "StartSession", "StartStopSessions", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("StartSessionResponse",
                              handler => HUB2CPOClient.OnStartSessionHTTPResponse += handler,
                              handler => HUB2CPOClient.OnStartSessionHTTPResponse -= handler,
                              "StartSession", "StartStopSessions", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("StopSessionRequest",
                              handler => HUB2CPOClient.OnStopSessionHTTPRequest += handler,
                              handler => HUB2CPOClient.OnStopSessionHTTPRequest -= handler,
                              "StopSession", "StopStopSessions", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("StopSessionResponse",
                              handler => HUB2CPOClient.OnStopSessionHTTPResponse += handler,
                              handler => HUB2CPOClient.OnStopSessionHTTPResponse -= handler,
                              "StopSession", "StopStopSessions", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);



                RegisterEvent("UnlockConnectorRequest",
                              handler => HUB2CPOClient.OnUnlockConnectorHTTPRequest += handler,
                              handler => HUB2CPOClient.OnUnlockConnectorHTTPRequest -= handler,
                              "UnlockConnector", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("UnlockConnectorResponse",
                              handler => HUB2CPOClient.OnUnlockConnectorHTTPResponse += handler,
                              handler => HUB2CPOClient.OnUnlockConnectorHTTPResponse -= handler,
                              "UnlockConnector", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

            }

            #endregion

        }

     }

}
