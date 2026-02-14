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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// A EMSP HTTP API logger.
    /// </summary>
    public sealed class EMSP_HTTPAPI_Logger : CommonAPILogger
    {

        #region Data

        /// <summary>
        /// The default context of this HTTP logger.
        /// </summary>
        public new const String DefaultContext = "EMSPAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked EMSP HTTP API.
        /// </summary>
        public EMSP_HTTPAPI  EMSP_HTTPAPI    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EMSP HTTP API logger using the default logging delegates.
        /// </summary>
        /// <param name="EMSP_HTTPAPI">The EMSP HTTP API.</param>
        /// <param name="Context">A context of this HTTP API.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public EMSP_HTTPAPI_Logger(EMSP_HTTPAPI                 EMSP_HTTPAPI,
                                   String?                      Context          = DefaultContext,
                                   String?                      LoggingPath      = null,
                                   OCPILogfileCreatorDelegate?  LogfileCreator   = null)

            : base(EMSP_HTTPAPI.CommonAPI,
                   Context ?? DefaultContext,
                   LoggingPath,
                   LogfileCreator)

        {

            this.EMSP_HTTPAPI = EMSP_HTTPAPI ?? throw new ArgumentNullException(nameof(EMSP_HTTPAPI), "The given EMSP HTTP API must not be null!");

            #region Location(s)

            RegisterEvent("GetLocationsRequest",
                          handler => EMSP_HTTPAPI.OnGetLocationsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnGetLocationsHTTPRequest -= handler,
                          "GetLocations", "Locations", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetLocationsResponse",
                          handler => EMSP_HTTPAPI.OnGetLocationsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnGetLocationsHTTPResponse -= handler,
                          "GetLocations", "Locations", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteLocationsRequest",
                          handler => EMSP_HTTPAPI.OnDeleteLocationsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnDeleteLocationsHTTPRequest -= handler,
                          "DeleteLocations", "Locations", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteLocationsResponse",
                          handler => EMSP_HTTPAPI.OnDeleteLocationsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnDeleteLocationsHTTPResponse -= handler,
                          "DeleteLocations", "Locations", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Location
            RegisterEvent("GetLocationRequest",
                          handler => EMSP_HTTPAPI.OnGetLocationHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnGetLocationHTTPRequest -= handler,
                          "GetLocation", "Location", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetLocationResponse",
                          handler => EMSP_HTTPAPI.OnGetLocationHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnGetLocationHTTPResponse -= handler,
                          "GetLocation", "Location", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutLocationRequest",
                          handler => EMSP_HTTPAPI.OnPutLocationHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnPutLocationHTTPRequest -= handler,
                          "PutLocation", "Locations", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutLocationResponse",
                          handler => EMSP_HTTPAPI.OnPutLocationHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnPutLocationHTTPResponse -= handler,
                          "PutLocation", "Locations", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchLocationRequest",
                          handler => EMSP_HTTPAPI.OnPatchLocationHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnPatchLocationHTTPRequest -= handler,
                          "PatchLocation", "Locations", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchLocationResponse",
                          handler => EMSP_HTTPAPI.OnPatchLocationHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnPatchLocationHTTPResponse -= handler,
                          "PatchLocation", "Locations", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteLocationRequest",
                          handler => EMSP_HTTPAPI.OnDeleteLocationHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnDeleteLocationHTTPRequest -= handler,
                          "DeleteLocation", "Locations", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteLocationResponse",
                          handler => EMSP_HTTPAPI.OnDeleteLocationHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnDeleteLocationHTTPResponse -= handler,
                          "DeleteLocation", "Locations", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region EVSE/EVSE status

            RegisterEvent("GetEVSERequest",
                          handler => EMSP_HTTPAPI.OnGetEVSEHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnGetEVSEHTTPRequest -= handler,
                          "GetEVSE", "EVSEs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetEVSEResponse",
                          handler => EMSP_HTTPAPI.OnGetEVSEHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnGetEVSEHTTPResponse -= handler,
                          "GetEVSE", "EVSEs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutEVSERequest",
                          handler => EMSP_HTTPAPI.OnPutEVSEHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnPutEVSEHTTPRequest -= handler,
                          "PutEVSE", "EVSEs", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutEVSEResponse",
                          handler => EMSP_HTTPAPI.OnPutEVSEHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnPutEVSEHTTPResponse -= handler,
                          "PutEVSE", "EVSEs", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchEVSERequest",
                          handler => EMSP_HTTPAPI.OnPatchEVSEHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnPatchEVSEHTTPRequest -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchEVSEResponse",
                          handler => EMSP_HTTPAPI.OnPatchEVSEHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnPatchEVSEHTTPResponse -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteEVSERequest",
                          handler => EMSP_HTTPAPI.OnDeleteEVSEHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnDeleteEVSEHTTPRequest -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteEVSEResponse",
                          handler => EMSP_HTTPAPI.OnDeleteEVSEHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnDeleteEVSEHTTPResponse -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // EVSE status

            RegisterEvent("PostEVSEStatusRequest",
                          handler => EMSP_HTTPAPI.OnPostEVSEStatusHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnPostEVSEStatusHTTPRequest -= handler,
                          "PostEVSEStatus", "EVSEs", "Post", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PostEVSEStatusResponse",
                          handler => EMSP_HTTPAPI.OnPostEVSEStatusHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnPostEVSEStatusHTTPResponse -= handler,
                          "PostEVSEStatus", "EVSEs", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Connector

            RegisterEvent("GetConnectorRequest",
                          handler => EMSP_HTTPAPI.OnGetConnectorHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnGetConnectorHTTPRequest -= handler,
                          "GetConnector", "Connectors", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetConnectorResponse",
                          handler => EMSP_HTTPAPI.OnGetConnectorHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnGetConnectorHTTPResponse -= handler,
                          "GetConnector", "Connectors", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutConnectorRequest",
                          handler => EMSP_HTTPAPI.OnPutConnectorHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnPutConnectorHTTPRequest -= handler,
                          "PutConnector", "Connectors", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutConnectorResponse",
                          handler => EMSP_HTTPAPI.OnPutConnectorHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnPutConnectorHTTPResponse -= handler,
                          "PutConnector", "Connectors", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchConnectorRequest",
                          handler => EMSP_HTTPAPI.OnPatchConnectorHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnPatchConnectorHTTPRequest -= handler,
                          "PatchConnector", "Connectors", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchConnectorResponse",
                          handler => EMSP_HTTPAPI.OnPatchConnectorHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnPatchConnectorHTTPResponse -= handler,
                          "PatchConnector", "Connectors", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteConnectorRequest",
                          handler => EMSP_HTTPAPI.OnDeleteConnectorHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnDeleteConnectorHTTPRequest -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteConnectorResponse",
                          handler => EMSP_HTTPAPI.OnDeleteConnectorHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnDeleteConnectorHTTPResponse -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Tariff(s)

            RegisterEvent("GetTariffsRequest",
                          handler => EMSP_HTTPAPI.OnGetTariffsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnGetTariffsHTTPRequest -= handler,
                          "GetTariffs", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTariffsResponse",
                          handler => EMSP_HTTPAPI.OnGetTariffsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnGetTariffsHTTPResponse -= handler,
                          "GetTariffs", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteTariffsRequest",
                          handler => EMSP_HTTPAPI.OnDeleteTariffsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnDeleteTariffsHTTPRequest -= handler,
                          "DeleteTariffs", "Tariffs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteTariffsResponse",
                          handler => EMSP_HTTPAPI.OnDeleteTariffsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnDeleteTariffsHTTPResponse -= handler,
                          "DeleteTariffs", "Tariffs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Tariff

            RegisterEvent("GetTariffRequest",
                          handler => EMSP_HTTPAPI.OnGetTariffHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnGetTariffHTTPRequest -= handler,
                          "GetTariff", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTariffResponse",
                          handler => EMSP_HTTPAPI.OnGetTariffHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnGetTariffHTTPResponse -= handler,
                          "GetTariff", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutTariffRequest",
                          handler => EMSP_HTTPAPI.OnPutTariffHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnPutTariffHTTPRequest -= handler,
                          "PutTariff", "Tariffs", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutTariffResponse",
                          handler => EMSP_HTTPAPI.OnPutTariffHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnPutTariffHTTPResponse -= handler,
                          "PutTariff", "Tariffs", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchTariffRequest",
                          handler => EMSP_HTTPAPI.OnPatchTariffHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnPatchTariffHTTPRequest -= handler,
                          "PatchTariff", "Tariffs", "Patch", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchTariffResponse",
                          handler => EMSP_HTTPAPI.OnPatchTariffHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnPatchTariffHTTPResponse -= handler,
                          "PatchTariff", "Tariffs", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteTariffRequest",
                          handler => EMSP_HTTPAPI.OnDeleteTariffHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnDeleteTariffHTTPRequest -= handler,
                          "DeleteTariff", "Tariffs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteTariffResponse",
                          handler => EMSP_HTTPAPI.OnDeleteTariffHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnDeleteTariffHTTPResponse -= handler,
                          "DeleteTariff", "Tariffs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Session(s)

            RegisterEvent("GetSessionsRequest",
                          handler => EMSP_HTTPAPI.OnGetSessionsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnGetSessionsHTTPRequest -= handler,
                          "GetSessions", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetSessionsResponse",
                          handler => EMSP_HTTPAPI.OnGetSessionsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnGetSessionsHTTPResponse -= handler,
                          "GetSessions", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteSessionsRequest",
                          handler => EMSP_HTTPAPI.OnDeleteSessionsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnDeleteSessionsHTTPRequest -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteSessionsResponse",
                          handler => EMSP_HTTPAPI.OnDeleteSessionsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnDeleteSessionsHTTPResponse -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Session

            RegisterEvent("GetSessionRequest",
                          handler => EMSP_HTTPAPI.OnGetSessionHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnGetSessionHTTPRequest -= handler,
                          "GetSession", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetSessionResponse",
                          handler => EMSP_HTTPAPI.OnGetSessionHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnGetSessionHTTPResponse -= handler,
                          "GetSession", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutSessionRequest",
                          handler => EMSP_HTTPAPI.OnPutSessionHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnPutSessionHTTPRequest -= handler,
                          "PutSession", "Sessions", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutSessionResponse",
                          handler => EMSP_HTTPAPI.OnPutSessionHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnPutSessionHTTPResponse -= handler,
                          "PutSession", "Sessions", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchSessionRequest",
                          handler => EMSP_HTTPAPI.OnPatchSessionHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnPatchSessionHTTPRequest -= handler,
                          "PatchSession", "Sessions", "Patch", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchSessionResponse",
                          handler => EMSP_HTTPAPI.OnPatchSessionHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnPatchSessionHTTPResponse -= handler,
                          "PatchSession", "Sessions", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteSessionRequest",
                          handler => EMSP_HTTPAPI.OnDeleteSessionHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnDeleteSessionHTTPRequest -= handler,
                          "DeleteSession", "Sessions", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteSessionResponse",
                          handler => EMSP_HTTPAPI.OnDeleteSessionHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnDeleteSessionHTTPResponse -= handler,
                          "DeleteSession", "Sessions", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region CDR(s)

            RegisterEvent("GetCDRsRequest",
                          handler => EMSP_HTTPAPI.OnGetCDRsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnGetCDRsHTTPRequest -= handler,
                          "GetCDRs", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetCDRsResponse",
                          handler => EMSP_HTTPAPI.OnGetCDRsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnGetCDRsHTTPResponse -= handler,
                          "GetCDRs", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PostCDRRequest",
                          handler => EMSP_HTTPAPI.OnPostCDRHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnPostCDRHTTPRequest -= handler,
                          "PostCDR", "CDRs", "Post", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PostCDRResponse",
                          handler => EMSP_HTTPAPI.OnPostCDRHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnPostCDRHTTPResponse -= handler,
                          "PostCDR", "CDRs", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteCDRsRequest",
                          handler => EMSP_HTTPAPI.OnDeleteCDRsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnDeleteCDRsHTTPRequest -= handler,
                          "DeleteCDRs", "CDRs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteCDRsResponse",
                          handler => EMSP_HTTPAPI.OnDeleteCDRsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnDeleteCDRsHTTPResponse -= handler,
                          "DeleteCDRs", "CDRs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);



            // CDR

            RegisterEvent("GetCDRRequest",
                          handler => EMSP_HTTPAPI.OnGetCDRHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnGetCDRHTTPRequest -= handler,
                          "GetCDR", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetCDRResponse",
                          handler => EMSP_HTTPAPI.OnGetCDRHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnGetCDRHTTPResponse -= handler,
                          "GetCDR", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteCDRRequest",
                          handler => EMSP_HTTPAPI.OnDeleteCDRHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnDeleteCDRHTTPRequest -= handler,
                          "DeleteCDR", "CDRs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteCDRResponse",
                          handler => EMSP_HTTPAPI.OnDeleteCDRHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnDeleteCDRHTTPResponse -= handler,
                          "DeleteCDR", "CDRs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Token(s)

            RegisterEvent("GetTokensRequest",
                          handler => EMSP_HTTPAPI.OnGetTokensHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnGetTokensHTTPRequest -= handler,
                          "GetTokens", "Tokens", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTokensResponse",
                          handler => EMSP_HTTPAPI.OnGetTokensHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnGetTokensHTTPResponse -= handler,
                          "GetTokens", "Tokens", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Token

            RegisterEvent("PostTokenRequest",
                          handler => EMSP_HTTPAPI.OnPostTokenHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnPostTokenHTTPRequest -= handler,
                          "PostToken", "Tokens", "Post", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PostTokenResponse",
                          handler => EMSP_HTTPAPI.OnPostTokenHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnPostTokenHTTPResponse -= handler,
                          "PostToken", "Tokens", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Command callbacks

            RegisterEvent("ReserveNowCallbackRequest",
                          handler => EMSP_HTTPAPI.OnReserveNowCallbackHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnReserveNowCallbackHTTPRequest -= handler,
                          "ReserveNowCallback", "ReservationCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("ReserveNowCallbackResponse",
                          handler => EMSP_HTTPAPI.OnReserveNowCallbackHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnReserveNowCallbackHTTPResponse -= handler,
                          "ReserveNowCallback", "ReservationCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("CancelReservationCallbackRequest",
                          handler => EMSP_HTTPAPI.OnCancelReservationCallbackHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnCancelReservationCallbackHTTPRequest -= handler,
                          "CancelReservationCallback", "ReservationCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CancelReservationCallbackResponse",
                          handler => EMSP_HTTPAPI.OnCancelReservationCallbackHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnCancelReservationCallbackHTTPResponse -= handler,
                          "CancelReservationCallback", "ReservationCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("StartSessionCallbackRequest",
                          handler => EMSP_HTTPAPI.OnStartSessionCallbackHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnStartSessionCallbackHTTPRequest -= handler,
                          "StartSessionCallback", "SessionCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("StartSessionCallbackResponse",
                          handler => EMSP_HTTPAPI.OnStartSessionCallbackHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnStartSessionCallbackHTTPResponse -= handler,
                          "StartSessionCallback", "SessionCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("StopSessionCallbackRequest",
                          handler => EMSP_HTTPAPI.OnStopSessionCallbackHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnStopSessionCallbackHTTPRequest -= handler,
                          "StopSessionCallback", "SessionCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("StopSessionCallbackResponse",
                          handler => EMSP_HTTPAPI.OnStopSessionCallbackHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnStopSessionCallbackHTTPResponse -= handler,
                          "StopSessionCallback", "SessionCallbacks", "CaCallbackslback", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("UnlockConnectorCallbackRequest",
                          handler => EMSP_HTTPAPI.OnUnlockConnectorCallbackHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.OnUnlockConnectorCallbackHTTPRequest -= handler,
                          "UnlockConnectorCallback", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("UnlockConnectorCallbackResponse",
                          handler => EMSP_HTTPAPI.OnUnlockConnectorCallbackHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.OnUnlockConnectorCallbackHTTPResponse -= handler,
                          "UnlockConnectorCallback", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

    }

}
