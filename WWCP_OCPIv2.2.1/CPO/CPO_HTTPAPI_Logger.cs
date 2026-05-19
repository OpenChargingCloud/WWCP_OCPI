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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// A CPO API HTTP logger.
    /// </summary>
    public sealed class CPO_HTTPAPI_Logger : CommonAPILogger
    {

        #region Data

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public new const String DefaultContext = "CPOAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked CPO HTTP API.
        /// </summary>
        public CPO_HTTPAPI  CPO_HTTPAPI    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CPO HTTP API logger using the default logging delegates.
        /// </summary>
        /// <param name="CPO_HTTPAPI">An CPO HTTP API.</param>
        /// <param name="Context">A context of this HTTP API.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public CPO_HTTPAPI_Logger(CPO_HTTPAPI                  CPO_HTTPAPI,
                                  String?                      Context          = DefaultContext,
                                  String?                      LoggingPath      = null,
                                  OCPILogfileCreatorDelegate?  LogfileCreator   = null)

            : base(CPO_HTTPAPI.CommonAPI,
                   Context ?? DefaultContext,
                   LoggingPath,
                   LogfileCreator)

        {

            this.CPO_HTTPAPI = CPO_HTTPAPI ?? throw new ArgumentNullException(nameof(CPO_HTTPAPI), "The given CPO HTTP API must not be null!");

            #region Location(s)

            RegisterEvent("GetLocationsRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetLocationsHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetLocationsHTTPRequest -= handler,
                          "GetLocations", "Locations", "Get", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetLocationsResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetLocationsHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetLocationsHTTPResponse -= handler,
                          "GetLocations", "Locations", "Get", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Location

            RegisterEvent("GetLocationRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetLocationHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetLocationHTTPRequest -= handler,
                          "GetLocation", "Locations", "Get", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetLocationResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetLocationHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetLocationHTTPResponse -= handler,
                          "GetLocation", "Locations", "Get", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region EVSE

            RegisterEvent("GetEVSERequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetEVSEHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetEVSEHTTPRequest -= handler,
                          "GetEVSE", "EVSEs", "Get", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetEVSEResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetEVSEHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetEVSEHTTPResponse -= handler,
                          "GetEVSE", "EVSEs", "Get", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Connector

            RegisterEvent("GetConnectorRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetConnectorHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetConnectorHTTPRequest -= handler,
                          "GetConnector", "Connectors", "Get", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetConnectorResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetConnectorHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetConnectorHTTPResponse -= handler,
                          "GetConnector", "Connectors", "Get", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Tariff(s)

            RegisterEvent("GetTariffsRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTariffsHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTariffsHTTPRequest -= handler,
                          "GetTariffs", "Tariffs", "Get", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTariffsResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTariffsHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTariffsHTTPResponse -= handler,
                          "GetTariffs", "Tariffs", "Get", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Tariff

            RegisterEvent("GetTariffRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTariffHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTariffHTTPRequest -= handler,
                          "GetTariff", "Tariffs", "Get", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTariffResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTariffHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTariffHTTPResponse -= handler,
                          "GetTariff", "Tariffs", "Get", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Session(s)

            RegisterEvent("GetSessionsRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetSessionsHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetSessionsHTTPRequest -= handler,
                          "GetSessions", "Sessions", "Get", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetSessionsResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetSessionsHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetSessionsHTTPResponse -= handler,
                          "GetSessions", "Sessions", "Get", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Session

            RegisterEvent("GetSessionRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetSessionHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetSessionHTTPRequest -= handler,
                          "GetSession", "Sessions", "Get", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetSessionResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetSessionHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetSessionHTTPResponse -= handler,
                          "GetSession", "Sessions", "Get", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region CDR(s)

            RegisterEvent("GetCDRsRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetCDRsHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetCDRsHTTPRequest -= handler,
                          "GetCDRs", "CDRs", "Get", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetCDRsResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetCDRsHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetCDRsHTTPResponse -= handler,
                          "GetCDRs", "CDRs", "Get", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // CDR

            RegisterEvent("GetCDRRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetCDRHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetCDRHTTPRequest -= handler,
                          "GetCDR", "CDRs", "Get", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetCDRResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetCDRHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetCDRHTTPResponse -= handler,
                          "GetCDR", "CDRs", "Get", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Token(s)

            RegisterEvent("GetTokensRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTokensHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTokensHTTPRequest -= handler,
                          "GetTokens", "Tokens", "Get", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTokensResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTokensHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTokensHTTPResponse -= handler,
                          "GetTokens", "Tokens", "Get", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteTokensRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnDeleteTokensHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnDeleteTokensHTTPRequest -= handler,
                          "DeleteTokens", "Tokens", "Delete", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteTokensResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnDeleteTokensHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnDeleteTokensHTTPResponse -= handler,
                          "DeleteTokens", "Tokens", "Delete", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Token

            RegisterEvent("GetTokenRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTokenHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTokenHTTPRequest -= handler,
                          "GetToken", "Tokens", "Get", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTokenResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTokenHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnGetTokenHTTPResponse -= handler,
                          "GetToken", "Tokens", "Get", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutTokenRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnPutTokenHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnPutTokenHTTPRequest -= handler,
                          "PutToken", "Tokens", "Put", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutTokenResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnPutTokenHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnPutTokenHTTPResponse -= handler,
                          "PutToken", "Tokens", "Put", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchTokenRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnPatchTokenHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnPatchTokenHTTPRequest -= handler,
                          "PatchToken", "Tokens", "Patch", "Requests", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchTokenResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnPatchTokenHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnPatchTokenHTTPResponse -= handler,
                          "PatchToken", "Tokens", "Patch", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteTokenRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnDeleteTokenHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnDeleteTokenHTTPRequest -= handler,
                          "DeleteToken", "Tokens", "Delete", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteTokenResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnDeleteTokenHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnDeleteTokenHTTPResponse -= handler,
                          "DeleteToken", "Tokens", "Delete", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion


            // Commands

            #region ReserveNow-/CancelReservation

            RegisterEvent("ReserveNowRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnReserveNowHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnReserveNowHTTPRequest -= handler,
                          "ReserveNow", "Reservations", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("ReserveNowResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnReserveNowHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnReserveNowHTTPResponse -= handler,
                          "ReserveNow", "Reservations", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);



            RegisterEvent("CancelReservationRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnCancelReservationHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnCancelReservationHTTPRequest -= handler,
                          "CancelReservation", "Reservations", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CancelReservationResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnCancelReservationHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnCancelReservationHTTPResponse -= handler,
                          "CancelReservation", "Reservations", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Start-/StopSession

            RegisterEvent("StartSessionRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnStartSessionHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnStartSessionHTTPRequest -= handler,
                          "StartSession", "Sessions", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("StartSessionResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnStartSessionHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnStartSessionHTTPResponse -= handler,
                          "StartSession", "Sessions", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);



            RegisterEvent("StopSessionRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnStopSessionHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnStopSessionHTTPRequest -= handler,
                          "StopSession", "Sessions", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("StopSessionResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnStopSessionHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnStopSessionHTTPResponse -= handler,
                          "StopSession", "Sessions", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region UnlockConnector

            RegisterEvent("UnlockConnectorRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnUnlockConnectorHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnUnlockConnectorHTTPRequest -= handler,
                          "UnlockConnector", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("UnlockConnectorResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnUnlockConnectorHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnUnlockConnectorHTTPResponse -= handler,
                          "UnlockConnector", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion


            // Open Charging Cloud Extensions

            #region NotifyWebPaymentsStarted-/Failed

            RegisterEvent("NotifyWebPaymentsStartedRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnNotifyWebPaymentsStartedHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnNotifyWebPaymentsStartedHTTPRequest -= handler,
                          "NotifyWebPaymentsStarted", "NotifyWebPayments", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("NotifyWebPaymentsStartedResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnNotifyWebPaymentsStartedHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnNotifyWebPaymentsStartedHTTPResponse -= handler,
                          "NotifyWebPaymentsStarted", "NotifyWebPayments", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);



            RegisterEvent("NotifyWebPaymentsFailedRequest",
                          handler => CPO_HTTPAPI.HTTPEvents.OnNotifyWebPaymentsFailedHTTPRequest += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnNotifyWebPaymentsFailedHTTPRequest -= handler,
                          "NotifyWebPaymentsFailed", "NotifyWebPayments", "Requests",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("NotifyWebPaymentsFailedResponse",
                          handler => CPO_HTTPAPI.HTTPEvents.OnNotifyWebPaymentsFailedHTTPResponse += handler,
                          handler => CPO_HTTPAPI.HTTPEvents.OnNotifyWebPaymentsFailedHTTPResponse -= handler,
                          "NotifyWebPaymentsFailed", "NotifyWebPayments", "Responses", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion


        }

        #endregion

    }

}
