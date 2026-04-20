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

namespace cloud.charging.open.protocols.OCPIv2_2_1.EMSP.HUB.HTTP
{

    /// <summary>
    /// The OCPI EMSP-2-HUB HTTP client.
    /// </summary>
    public partial class EMSP2HUB_HTTPClient : IHTTPClient
    {

        /// <summary>
        /// The OCPI EMSP-2-HUB HTTP client logger.
        /// </summary>
        public new sealed class HTTPClientLogger : CommonHTTPClient.HTTPClientLogger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public new const String  DefaultContext   = $"OCPI{Version.String}_EMSP-2-HUBClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached EMSP-2-HUB client.
            /// </summary>
            public EMSP2HUB_HTTPClient  EMSP2HUBClient    { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new EMSP-2-HUB client logger using the default logging delegates.
            /// </summary>
            /// <param name="EMSP2HUBClient">An EMSP-2-HUB client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public HTTPClientLogger(EMSP2HUB_HTTPClient               EMSP2HUBClient,
                                    String?                      LoggingPath,
                                    String?                      Context          = DefaultContext,
                                    OCPILogfileCreatorDelegate?  LogfileCreator   = null)

                : base(EMSP2HUBClient,
                       LoggingPath,
                       Context ?? DefaultContext,
                       LogfileCreator)

            {

                this.EMSP2HUBClient = EMSP2HUBClient ?? throw new ArgumentNullException(nameof(EMSP2HUBClient), "The given EMSP-2-HUB client must not be null!");

                #region Locations

                RegisterEvent("GetLocationsRequest",
                              handler => EMSP2HUBClient.OnGetLocationsHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnGetLocationsHTTPRequest -= handler,
                              "GetLocations", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("GetLocationsResponse",
                              handler => EMSP2HUBClient.OnGetLocationsHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnGetLocationsHTTPResponse -= handler,
                              "GetLocations", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);


                RegisterEvent("GetLocationRequest",
                              handler => EMSP2HUBClient.OnGetLocationHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnGetLocationHTTPRequest -= handler,
                              "GetLocation", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("GetLocationResponse",
                              handler => EMSP2HUBClient.OnGetLocationHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnGetLocationHTTPResponse -= handler,
                              "GetLocation", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);


                RegisterEvent("GetEVSERequest",
                              handler => EMSP2HUBClient.OnGetEVSEHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnGetEVSEHTTPRequest -= handler,
                              "GetEVSE", "EVSEs", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("GetEVSEResponse",
                              handler => EMSP2HUBClient.OnGetEVSEHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnGetEVSEHTTPResponse -= handler,
                              "GetEVSE", "EVSEs", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);


                RegisterEvent("GetConnectorRequest",
                              handler => EMSP2HUBClient.OnGetConnectorHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnGetConnectorHTTPRequest -= handler,
                              "GetConnector", "connectors", "locations", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("GetConnectorResponse",
                              handler => EMSP2HUBClient.OnGetConnectorHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnGetConnectorHTTPResponse -= handler,
                              "GetConnector", "connectors", "locations", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                #endregion

                #region Tariffs

                RegisterEvent("GetTariffsRequest",
                              handler => EMSP2HUBClient.OnGetTariffsHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnGetTariffsHTTPRequest -= handler,
                              "GetTariffs", "tariffs", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("GetTariffsResponse",
                              handler => EMSP2HUBClient.OnGetTariffsHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnGetTariffsHTTPResponse -= handler,
                              "GetTariffs", "tariffs", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);


                RegisterEvent("GetTariffRequest",
                              handler => EMSP2HUBClient.OnGetTariffHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnGetTariffHTTPRequest -= handler,
                              "GetTariff", "tariffs", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("GetTariffResponse",
                              handler => EMSP2HUBClient.OnGetTariffHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnGetTariffHTTPResponse -= handler,
                              "GetTariff", "tariffs", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                #endregion

                #region Sessions

                RegisterEvent("GetSessionsRequest",
                              handler => EMSP2HUBClient.OnGetSessionsHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnGetSessionsHTTPRequest -= handler,
                              "GetSessions", "sessions", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("GetSessionsResponse",
                              handler => EMSP2HUBClient.OnGetSessionsHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnGetSessionsHTTPResponse -= handler,
                              "GetSessions", "sessions", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);


                RegisterEvent("GetSessionRequest",
                              handler => EMSP2HUBClient.OnGetSessionHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnGetSessionHTTPRequest -= handler,
                              "GetSession", "sessions", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("GetSessionResponse",
                              handler => EMSP2HUBClient.OnGetSessionHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnGetSessionHTTPResponse -= handler,
                              "GetSession", "sessions", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                #endregion

                #region CDRs

                RegisterEvent("GetCDRsRequest",
                              handler => EMSP2HUBClient.OnGetCDRsHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnGetCDRsHTTPRequest -= handler,
                              "GetCDRs", "CDRs", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("GetCDRsResponse",
                              handler => EMSP2HUBClient.OnGetCDRsHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnGetCDRsHTTPResponse -= handler,
                              "GetCDRs", "CDRs", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);


                RegisterEvent("GetCDRRequest",
                              handler => EMSP2HUBClient.OnGetCDRHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnGetCDRHTTPRequest -= handler,
                              "GetCDR", "CDRs", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("GetCDRResponse",
                              handler => EMSP2HUBClient.OnGetCDRHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnGetCDRHTTPResponse -= handler,
                              "GetCDR", "CDRs", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                #endregion

                #region Tokens

                RegisterEvent("GetTokenRequest",
                              handler => EMSP2HUBClient.OnGetTokenHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnGetTokenHTTPRequest -= handler,
                              "GetToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("GetTokenResponse",
                              handler => EMSP2HUBClient.OnGetTokenHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnGetTokenHTTPResponse -= handler,
                              "GetToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);


                RegisterEvent("PutTokenRequest",
                              handler => EMSP2HUBClient.OnPutTokenHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnPutTokenHTTPRequest -= handler,
                              "PutToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("PutTokenResponse",
                              handler => EMSP2HUBClient.OnPutTokenHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnPutTokenHTTPResponse -= handler,
                              "PutToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);


                RegisterEvent("PatchTokenRequest",
                              handler => EMSP2HUBClient.OnPatchTokenHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnPatchTokenHTTPRequest -= handler,
                              "PatchToken", "tokens", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("PatchTokenResponse",
                              handler => EMSP2HUBClient.OnPatchTokenHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnPatchTokenHTTPResponse -= handler,
                              "PatchToken", "tokens", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                #endregion

                #region Commands

                RegisterEvent("ReserveNowRequest",
                              handler => EMSP2HUBClient.OnReserveNowHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnReserveNowHTTPRequest -= handler,
                              "ReserveNow", "reservations", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("ReserveNowResponse",
                              handler => EMSP2HUBClient.OnReserveNowHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnReserveNowHTTPResponse -= handler,
                              "ReserveNow", "reservations", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);


                RegisterEvent("CancelReservationRequest",
                              handler => EMSP2HUBClient.OnCancelReservationHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnCancelReservationHTTPRequest -= handler,
                              "CancelReservation", "reservations", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("CancelReservationResponse",
                              handler => EMSP2HUBClient.OnCancelReservationHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnCancelReservationHTTPResponse -= handler,
                              "CancelReservation", "reservations", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);



                RegisterEvent("StartSessionRequest",
                              handler => EMSP2HUBClient.OnStartSessionHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnStartSessionHTTPRequest -= handler,
                              "StartSession", "StartStopSessions", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("StartSessionResponse",
                              handler => EMSP2HUBClient.OnStartSessionHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnStartSessionHTTPResponse -= handler,
                              "StartSession", "StartStopSessions", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);


                RegisterEvent("StopSessionRequest",
                              handler => EMSP2HUBClient.OnStopSessionHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnStopSessionHTTPRequest -= handler,
                              "StopSession", "StopStopSessions", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("StopSessionResponse",
                              handler => EMSP2HUBClient.OnStopSessionHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnStopSessionHTTPResponse -= handler,
                              "StopSession", "StopStopSessions", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);



                RegisterEvent("UnlockConnectorRequest",
                              handler => EMSP2HUBClient.OnUnlockConnectorHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnUnlockConnectorHTTPRequest -= handler,
                              "UnlockConnector", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("UnlockConnectorResponse",
                              handler => EMSP2HUBClient.OnUnlockConnectorHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnUnlockConnectorHTTPResponse -= handler,
                              "UnlockConnector", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);


                // Open Charging Cloud Extensions

                RegisterEvent("NotifyWebPaymentsStartedRequest",
                              handler => EMSP2HUBClient.OnNotifyWebPaymentsStartedHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnNotifyWebPaymentsStartedHTTPRequest -= handler,
                              "NotifyWebPaymentsStarted", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("NotifyWebPaymentsStartedResponse",
                              handler => EMSP2HUBClient.OnNotifyWebPaymentsStartedHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnNotifyWebPaymentsStartedHTTPResponse -= handler,
                              "NotifyWebPaymentsStarted", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);


                RegisterEvent("NotifyWebPaymentsFailedRequest",
                              handler => EMSP2HUBClient.OnNotifyWebPaymentsFailedHTTPRequest += handler,
                              handler => EMSP2HUBClient.OnNotifyWebPaymentsFailedHTTPRequest -= handler,
                              "NotifyWebPaymentsFailed", "commands", "requests", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                RegisterEvent("NotifyWebPaymentsFailedResponse",
                              handler => EMSP2HUBClient.OnNotifyWebPaymentsFailedHTTPResponse += handler,
                              handler => EMSP2HUBClient.OnNotifyWebPaymentsFailedHTTPResponse -= handler,
                              "NotifyWebPaymentsFailed", "commands", "responses", "all").
                    RegisterDefaultConsoleLogTargetX(this).
                    RegisterDefaultDiscLogTargetX(this);

                #endregion

            }

            #endregion

        }

     }

}
