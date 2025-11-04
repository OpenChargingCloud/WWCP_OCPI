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
using cloud.charging.open.protocols.OCPIv3_0;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.EMSP.HTTP
{

    /// <summary>
    /// The OCPI EMSP client.
    /// </summary>
    public partial class EMSP2CPOClient : IHTTPClient
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
            public EMSP2CPOClient  EMSP2CPOClient    { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new EMSP client logger using the default logging delegates.
            /// </summary>
            /// <param name="EMSP2CPOClient">A EMSP client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public Logger(EMSP2CPOClient               EMSP2CPOClient,
                          String?                      LoggingPath,
                          String?                      Context          = DefaultContext,
                          OCPILogfileCreatorDelegate?  LogfileCreator   = null)

                : base(EMSP2CPOClient,
                       LoggingPath,
                       Context ?? DefaultContext,
                       LogfileCreator)

            {

                this.EMSP2CPOClient = EMSP2CPOClient ?? throw new ArgumentNullException(nameof(EMSP2CPOClient), "The given EMSP client must not be null!");

                #region Locations

                RegisterEvent("GetLocationsRequest",
                              handler => EMSP2CPOClient.OnGetLocationsHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnGetLocationsHTTPRequest -= handler,
                              "GetLocations", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationsResponse",
                              handler => EMSP2CPOClient.OnGetLocationsHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnGetLocationsHTTPResponse -= handler,
                              "GetLocations", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetLocationRequest",
                              handler => EMSP2CPOClient.OnGetLocationHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnGetLocationHTTPRequest -= handler,
                              "GetLocation", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationResponse",
                              handler => EMSP2CPOClient.OnGetLocationHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnGetLocationHTTPResponse -= handler,
                              "GetLocation", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetEVSERequest",
                              handler => EMSP2CPOClient.OnGetEVSEHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnGetEVSEHTTPRequest -= handler,
                              "GetEVSE", "EVSEs", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetEVSEResponse",
                              handler => EMSP2CPOClient.OnGetEVSEHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnGetEVSEHTTPResponse -= handler,
                              "GetEVSE", "EVSEs", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetConnectorRequest",
                              handler => EMSP2CPOClient.OnGetConnectorHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnGetConnectorHTTPRequest -= handler,
                              "GetConnector", "connectors", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetConnectorResponse",
                              handler => EMSP2CPOClient.OnGetConnectorHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnGetConnectorHTTPResponse -= handler,
                              "GetConnector", "connectors", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Tariffs

                RegisterEvent("GetTariffsRequest",
                              handler => EMSP2CPOClient.OnGetTariffsHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnGetTariffsHTTPRequest -= handler,
                              "GetTariffs", "tariffs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTariffsResponse",
                              handler => EMSP2CPOClient.OnGetTariffsHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnGetTariffsHTTPResponse -= handler,
                              "GetTariffs", "tariffs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetTariffRequest",
                              handler => EMSP2CPOClient.OnGetTariffHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnGetTariffHTTPRequest -= handler,
                              "GetTariff", "tariffs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTariffResponse",
                              handler => EMSP2CPOClient.OnGetTariffHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnGetTariffHTTPResponse -= handler,
                              "GetTariff", "tariffs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Sessions

                RegisterEvent("GetSessionsRequest",
                              handler => EMSP2CPOClient.OnGetSessionsHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnGetSessionsHTTPRequest -= handler,
                              "GetSessions", "sessions", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionsResponse",
                              handler => EMSP2CPOClient.OnGetSessionsHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnGetSessionsHTTPResponse -= handler,
                              "GetSessions", "sessions", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetSessionRequest",
                              handler => EMSP2CPOClient.OnGetSessionHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnGetSessionHTTPRequest -= handler,
                              "GetSession", "sessions", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionResponse",
                              handler => EMSP2CPOClient.OnGetSessionHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnGetSessionHTTPResponse -= handler,
                              "GetSession", "sessions", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region CDRs

                RegisterEvent("GetCDRsRequest",
                              handler => EMSP2CPOClient.OnGetCDRsHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnGetCDRsHTTPRequest -= handler,
                              "GetCDRs", "CDRs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCDRsResponse",
                              handler => EMSP2CPOClient.OnGetCDRsHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnGetCDRsHTTPResponse -= handler,
                              "GetCDRs", "CDRs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetCDRRequest",
                              handler => EMSP2CPOClient.OnGetCDRHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnGetCDRHTTPRequest -= handler,
                              "GetCDR", "CDRs", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCDRResponse",
                              handler => EMSP2CPOClient.OnGetCDRHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnGetCDRHTTPResponse -= handler,
                              "GetCDR", "CDRs", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Tokens

                RegisterEvent("GetTokenRequest",
                              handler => EMSP2CPOClient.OnGetTokenHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnGetTokenHTTPRequest -= handler,
                              "GetToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTokenResponse",
                              handler => EMSP2CPOClient.OnGetTokenHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnGetTokenHTTPResponse -= handler,
                              "GetToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutTokenRequest",
                              handler => EMSP2CPOClient.OnPutTokenHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnPutTokenHTTPRequest -= handler,
                              "PutToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutTokenResponse",
                              handler => EMSP2CPOClient.OnPutTokenHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnPutTokenHTTPResponse -= handler,
                              "PutToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchTokenRequest",
                              handler => EMSP2CPOClient.OnPatchTokenHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnPatchTokenHTTPRequest -= handler,
                              "PatchToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchTokenResponse",
                              handler => EMSP2CPOClient.OnPatchTokenHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnPatchTokenHTTPResponse -= handler,
                              "PatchToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Commands

                RegisterEvent("ReserveNowRequest",
                              handler => EMSP2CPOClient.OnReserveNowHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnReserveNowHTTPRequest -= handler,
                              "ReserveNow", "reservations", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("ReserveNowResponse",
                              handler => EMSP2CPOClient.OnReserveNowHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnReserveNowHTTPResponse -= handler,
                              "ReserveNow", "reservations", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("CancelReservationRequest",
                              handler => EMSP2CPOClient.OnCancelReservationHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnCancelReservationHTTPRequest -= handler,
                              "CancelReservation", "reservations", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("CancelReservationResponse",
                              handler => EMSP2CPOClient.OnCancelReservationHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnCancelReservationHTTPResponse -= handler,
                              "CancelReservation", "reservations", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);



                RegisterEvent("StartSessionRequest",
                              handler => EMSP2CPOClient.OnStartSessionHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnStartSessionHTTPRequest -= handler,
                              "StartSession", "StartStopSessions", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("StartSessionResponse",
                              handler => EMSP2CPOClient.OnStartSessionHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnStartSessionHTTPResponse -= handler,
                              "StartSession", "StartStopSessions", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("StopSessionRequest",
                              handler => EMSP2CPOClient.OnStopSessionHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnStopSessionHTTPRequest -= handler,
                              "StopSession", "StopStopSessions", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("StopSessionResponse",
                              handler => EMSP2CPOClient.OnStopSessionHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnStopSessionHTTPResponse -= handler,
                              "StopSession", "StopStopSessions", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);



                RegisterEvent("UnlockConnectorRequest",
                              handler => EMSP2CPOClient.OnUnlockConnectorHTTPRequest += handler,
                              handler => EMSP2CPOClient.OnUnlockConnectorHTTPRequest -= handler,
                              "UnlockConnector", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("UnlockConnectorResponse",
                              handler => EMSP2CPOClient.OnUnlockConnectorHTTPResponse += handler,
                              handler => EMSP2CPOClient.OnUnlockConnectorHTTPResponse -= handler,
                              "UnlockConnector", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

            }

            #endregion

        }

     }

}
