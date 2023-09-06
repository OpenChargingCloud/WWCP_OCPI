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

namespace cloud.charging.open.protocols.OCPIv2_1_1.HTTP
{

    /// <summary>
    /// A EMSP API logger.
    /// </summary>
    public sealed class EMSPAPILogger : CommonAPILogger
    {

        #region Data

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public new const String  DefaultContext   = $"OCPI{Version.String}_EMSPAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked EMSP API.
        /// </summary>
        public EMSPAPI  EMSPAPI  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EMSP API logger using the default logging delegates.
        /// </summary>
        /// <param name="EMSPAPI">An EMSP API.</param>
        /// <param name="Context">A context of this API.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public EMSPAPILogger(EMSPAPI                      EMSPAPI,
                             String?                      Context          = DefaultContext,
                             String?                      LoggingPath      = null,
                             OCPILogfileCreatorDelegate?  LogfileCreator   = null)

            : base(EMSPAPI.CommonAPI,
                   Context ?? DefaultContext,
                   LoggingPath,
                   LogfileCreator)

        {

            this.EMSPAPI = EMSPAPI ?? throw new ArgumentNullException(nameof(EMSPAPI), "The given EMSP API must not be null!");

            #region Location(s)

            RegisterEvent("GetLocationsRequest",
                          handler => EMSPAPI.OnGetLocationsRequest += handler,
                          handler => EMSPAPI.OnGetLocationsRequest -= handler,
                          "GetLocations", "Locations", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetLocationsResponse",
                          handler => EMSPAPI.OnGetLocationsResponse += handler,
                          handler => EMSPAPI.OnGetLocationsResponse -= handler,
                          "GetLocations", "Locations", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteLocationsRequest",
                          handler => EMSPAPI.OnDeleteLocationsRequest += handler,
                          handler => EMSPAPI.OnDeleteLocationsRequest -= handler,
                          "DeleteLocations", "Locations", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteLocationsResponse",
                          handler => EMSPAPI.OnDeleteLocationsResponse += handler,
                          handler => EMSPAPI.OnDeleteLocationsResponse -= handler,
                          "DeleteLocations", "Locations", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Location
            RegisterEvent("GetLocationRequest",
                          handler => EMSPAPI.OnGetLocationRequest += handler,
                          handler => EMSPAPI.OnGetLocationRequest -= handler,
                          "GetLocation", "Location", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetLocationResponse",
                          handler => EMSPAPI.OnGetLocationResponse += handler,
                          handler => EMSPAPI.OnGetLocationResponse -= handler,
                          "GetLocation", "Location", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutLocationRequest",
                          handler => EMSPAPI.OnPutLocationRequest += handler,
                          handler => EMSPAPI.OnPutLocationRequest -= handler,
                          "PutLocation", "Locations", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutLocationResponse",
                          handler => EMSPAPI.OnPutLocationResponse += handler,
                          handler => EMSPAPI.OnPutLocationResponse -= handler,
                          "PutLocation", "Locations", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchLocationRequest",
                          handler => EMSPAPI.OnPatchLocationRequest += handler,
                          handler => EMSPAPI.OnPatchLocationRequest -= handler,
                          "PatchLocation", "Locations", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchLocationResponse",
                          handler => EMSPAPI.OnPatchLocationResponse += handler,
                          handler => EMSPAPI.OnPatchLocationResponse -= handler,
                          "PatchLocation", "Locations", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteLocationRequest",
                          handler => EMSPAPI.OnDeleteLocationRequest += handler,
                          handler => EMSPAPI.OnDeleteLocationRequest -= handler,
                          "DeleteLocation", "Locations", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteLocationResponse",
                          handler => EMSPAPI.OnDeleteLocationResponse += handler,
                          handler => EMSPAPI.OnDeleteLocationResponse -= handler,
                          "DeleteLocation", "Locations", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region EVSE/EVSE status

            RegisterEvent("GetEVSERequest",
                          handler => EMSPAPI.OnGetEVSERequest += handler,
                          handler => EMSPAPI.OnGetEVSERequest -= handler,
                          "GetEVSE", "EVSEs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetEVSEResponse",
                          handler => EMSPAPI.OnGetEVSEResponse += handler,
                          handler => EMSPAPI.OnGetEVSEResponse -= handler,
                          "GetEVSE", "EVSEs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutEVSERequest",
                          handler => EMSPAPI.OnPutEVSERequest += handler,
                          handler => EMSPAPI.OnPutEVSERequest -= handler,
                          "PutEVSE", "EVSEs", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutEVSEResponse",
                          handler => EMSPAPI.OnPutEVSEResponse += handler,
                          handler => EMSPAPI.OnPutEVSEResponse -= handler,
                          "PutEVSE", "EVSEs", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchEVSERequest",
                          handler => EMSPAPI.OnPatchEVSERequest += handler,
                          handler => EMSPAPI.OnPatchEVSERequest -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchEVSEResponse",
                          handler => EMSPAPI.OnPatchEVSEResponse += handler,
                          handler => EMSPAPI.OnPatchEVSEResponse -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteEVSERequest",
                          handler => EMSPAPI.OnDeleteEVSERequest += handler,
                          handler => EMSPAPI.OnDeleteEVSERequest -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteEVSEResponse",
                          handler => EMSPAPI.OnDeleteEVSEResponse += handler,
                          handler => EMSPAPI.OnDeleteEVSEResponse -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // EVSE status

            RegisterEvent("PostEVSEStatusRequest",
                          handler => EMSPAPI.OnPostEVSEStatusRequest += handler,
                          handler => EMSPAPI.OnPostEVSEStatusRequest -= handler,
                          "PostEVSEStatus", "EVSEs", "Post", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PostEVSEStatusResponse",
                          handler => EMSPAPI.OnPostEVSEStatusResponse += handler,
                          handler => EMSPAPI.OnPostEVSEStatusResponse -= handler,
                          "PostEVSEStatus", "EVSEs", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Connector

            RegisterEvent("GetConnectorRequest",
                          handler => EMSPAPI.OnGetConnectorRequest += handler,
                          handler => EMSPAPI.OnGetConnectorRequest -= handler,
                          "GetConnector", "Connectors", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetConnectorResponse",
                          handler => EMSPAPI.OnGetConnectorResponse += handler,
                          handler => EMSPAPI.OnGetConnectorResponse -= handler,
                          "GetConnector", "Connectors", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutConnectorRequest",
                          handler => EMSPAPI.OnPutConnectorRequest += handler,
                          handler => EMSPAPI.OnPutConnectorRequest -= handler,
                          "PutConnector", "Connectors", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutConnectorResponse",
                          handler => EMSPAPI.OnPutConnectorResponse += handler,
                          handler => EMSPAPI.OnPutConnectorResponse -= handler,
                          "PutConnector", "Connectors", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchConnectorRequest",
                          handler => EMSPAPI.OnPatchConnectorRequest += handler,
                          handler => EMSPAPI.OnPatchConnectorRequest -= handler,
                          "PatchConnector", "Connectors", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchConnectorResponse",
                          handler => EMSPAPI.OnPatchConnectorResponse += handler,
                          handler => EMSPAPI.OnPatchConnectorResponse -= handler,
                          "PatchConnector", "Connectors", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteConnectorRequest",
                          handler => EMSPAPI.OnDeleteConnectorRequest += handler,
                          handler => EMSPAPI.OnDeleteConnectorRequest -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteConnectorResponse",
                          handler => EMSPAPI.OnDeleteConnectorResponse += handler,
                          handler => EMSPAPI.OnDeleteConnectorResponse -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Tariff(s)

            RegisterEvent("GetTariffsRequest",
                          handler => EMSPAPI.OnGetTariffsRequest += handler,
                          handler => EMSPAPI.OnGetTariffsRequest -= handler,
                          "GetTariffs", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTariffsResponse",
                          handler => EMSPAPI.OnGetTariffsResponse += handler,
                          handler => EMSPAPI.OnGetTariffsResponse -= handler,
                          "GetTariffs", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteTariffsRequest",
                          handler => EMSPAPI.OnDeleteTariffsRequest += handler,
                          handler => EMSPAPI.OnDeleteTariffsRequest -= handler,
                          "DeleteTariffs", "Tariffs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteTariffsResponse",
                          handler => EMSPAPI.OnDeleteTariffsResponse += handler,
                          handler => EMSPAPI.OnDeleteTariffsResponse -= handler,
                          "DeleteTariffs", "Tariffs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Tariff

            RegisterEvent("GetTariffRequest",
                          handler => EMSPAPI.OnGetTariffRequest += handler,
                          handler => EMSPAPI.OnGetTariffRequest -= handler,
                          "GetTariff", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTariffResponse",
                          handler => EMSPAPI.OnGetTariffResponse += handler,
                          handler => EMSPAPI.OnGetTariffResponse -= handler,
                          "GetTariff", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutTariffRequest",
                          handler => EMSPAPI.OnPutTariffRequest += handler,
                          handler => EMSPAPI.OnPutTariffRequest -= handler,
                          "PutTariff", "Tariffs", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutTariffResponse",
                          handler => EMSPAPI.OnPutTariffResponse += handler,
                          handler => EMSPAPI.OnPutTariffResponse -= handler,
                          "PutTariff", "Tariffs", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchTariffRequest",
                          handler => EMSPAPI.OnPatchTariffRequest += handler,
                          handler => EMSPAPI.OnPatchTariffRequest -= handler,
                          "PatchTariff", "Tariffs", "Patch", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchTariffResponse",
                          handler => EMSPAPI.OnPatchTariffResponse += handler,
                          handler => EMSPAPI.OnPatchTariffResponse -= handler,
                          "PatchTariff", "Tariffs", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteTariffRequest",
                          handler => EMSPAPI.OnDeleteTariffRequest += handler,
                          handler => EMSPAPI.OnDeleteTariffRequest -= handler,
                          "DeleteTariff", "Tariffs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteTariffResponse",
                          handler => EMSPAPI.OnDeleteTariffResponse += handler,
                          handler => EMSPAPI.OnDeleteTariffResponse -= handler,
                          "DeleteTariff", "Tariffs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Session(s)

            RegisterEvent("GetSessionsRequest",
                          handler => EMSPAPI.OnGetSessionsRequest += handler,
                          handler => EMSPAPI.OnGetSessionsRequest -= handler,
                          "GetSessions", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetSessionsResponse",
                          handler => EMSPAPI.OnGetSessionsResponse += handler,
                          handler => EMSPAPI.OnGetSessionsResponse -= handler,
                          "GetSessions", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteSessionsRequest",
                          handler => EMSPAPI.OnDeleteSessionsRequest += handler,
                          handler => EMSPAPI.OnDeleteSessionsRequest -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteSessionsResponse",
                          handler => EMSPAPI.OnDeleteSessionsResponse += handler,
                          handler => EMSPAPI.OnDeleteSessionsResponse -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Session

            RegisterEvent("GetSessionRequest",
                          handler => EMSPAPI.OnGetSessionRequest += handler,
                          handler => EMSPAPI.OnGetSessionRequest -= handler,
                          "GetSession", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetSessionResponse",
                          handler => EMSPAPI.OnGetSessionResponse += handler,
                          handler => EMSPAPI.OnGetSessionResponse -= handler,
                          "GetSession", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutSessionRequest",
                          handler => EMSPAPI.OnPutSessionRequest += handler,
                          handler => EMSPAPI.OnPutSessionRequest -= handler,
                          "PutSession", "Sessions", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutSessionResponse",
                          handler => EMSPAPI.OnPutSessionResponse += handler,
                          handler => EMSPAPI.OnPutSessionResponse -= handler,
                          "PutSession", "Sessions", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchSessionRequest",
                          handler => EMSPAPI.OnPatchSessionRequest += handler,
                          handler => EMSPAPI.OnPatchSessionRequest -= handler,
                          "PatchSession", "Sessions", "Patch", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchSessionResponse",
                          handler => EMSPAPI.OnPatchSessionResponse += handler,
                          handler => EMSPAPI.OnPatchSessionResponse -= handler,
                          "PatchSession", "Sessions", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteSessionRequest",
                          handler => EMSPAPI.OnDeleteSessionRequest += handler,
                          handler => EMSPAPI.OnDeleteSessionRequest -= handler,
                          "DeleteSession", "Sessions", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteSessionResponse",
                          handler => EMSPAPI.OnDeleteSessionResponse += handler,
                          handler => EMSPAPI.OnDeleteSessionResponse -= handler,
                          "DeleteSession", "Sessions", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region CDR(s)

            RegisterEvent("GetCDRsRequest",
                          handler => EMSPAPI.OnGetCDRsRequest += handler,
                          handler => EMSPAPI.OnGetCDRsRequest -= handler,
                          "GetCDRs", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetCDRsResponse",
                          handler => EMSPAPI.OnGetCDRsResponse += handler,
                          handler => EMSPAPI.OnGetCDRsResponse -= handler,
                          "GetCDRs", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PostCDRRequest",
                          handler => EMSPAPI.OnPostCDRRequest += handler,
                          handler => EMSPAPI.OnPostCDRRequest -= handler,
                          "PostCDR", "CDRs", "Post", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PostCDRResponse",
                          handler => EMSPAPI.OnPostCDRResponse += handler,
                          handler => EMSPAPI.OnPostCDRResponse -= handler,
                          "PostCDR", "CDRs", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteCDRsRequest",
                          handler => EMSPAPI.OnDeleteCDRsRequest += handler,
                          handler => EMSPAPI.OnDeleteCDRsRequest -= handler,
                          "DeleteCDRs", "CDRs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteCDRsResponse",
                          handler => EMSPAPI.OnDeleteCDRsResponse += handler,
                          handler => EMSPAPI.OnDeleteCDRsResponse -= handler,
                          "DeleteCDRs", "CDRs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);



            // CDR

            RegisterEvent("GetCDRRequest",
                          handler => EMSPAPI.OnGetCDRRequest += handler,
                          handler => EMSPAPI.OnGetCDRRequest -= handler,
                          "GetCDR", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetCDRResponse",
                          handler => EMSPAPI.OnGetCDRResponse += handler,
                          handler => EMSPAPI.OnGetCDRResponse -= handler,
                          "GetCDR", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteCDRRequest",
                          handler => EMSPAPI.OnDeleteCDRRequest += handler,
                          handler => EMSPAPI.OnDeleteCDRRequest -= handler,
                          "DeleteCDR", "CDRs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteCDRResponse",
                          handler => EMSPAPI.OnDeleteCDRResponse += handler,
                          handler => EMSPAPI.OnDeleteCDRResponse -= handler,
                          "DeleteCDR", "CDRs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Token(s)

            RegisterEvent("GetTokensRequest",
                          handler => EMSPAPI.OnGetTokensRequest += handler,
                          handler => EMSPAPI.OnGetTokensRequest -= handler,
                          "GetTokens", "Tokens", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTokensResponse",
                          handler => EMSPAPI.OnGetTokensResponse += handler,
                          handler => EMSPAPI.OnGetTokensResponse -= handler,
                          "GetTokens", "Tokens", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Token

            RegisterEvent("PostTokenRequest",
                          handler => EMSPAPI.OnPostTokenRequest += handler,
                          handler => EMSPAPI.OnPostTokenRequest -= handler,
                          "PostToken", "Tokens", "Post", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PostTokenResponse",
                          handler => EMSPAPI.OnPostTokenResponse += handler,
                          handler => EMSPAPI.OnPostTokenResponse -= handler,
                          "PostToken", "Tokens", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Command callbacks

            RegisterEvent("ReserveNowCallbackRequest",
                          handler => EMSPAPI.OnReserveNowCallbackRequest += handler,
                          handler => EMSPAPI.OnReserveNowCallbackRequest -= handler,
                          "ReserveNowCallback", "ReservationCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("ReserveNowCallbackResponse",
                          handler => EMSPAPI.OnReserveNowCallbackResponse += handler,
                          handler => EMSPAPI.OnReserveNowCallbackResponse -= handler,
                          "ReserveNowCallback", "ReservationCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("CancelReservationCallbackRequest",
                          handler => EMSPAPI.OnCancelReservationCallbackRequest += handler,
                          handler => EMSPAPI.OnCancelReservationCallbackRequest -= handler,
                          "CancelReservationCallback", "ReservationCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CancelReservationCallbackResponse",
                          handler => EMSPAPI.OnCancelReservationCallbackResponse += handler,
                          handler => EMSPAPI.OnCancelReservationCallbackResponse -= handler,
                          "CancelReservationCallback", "ReservationCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("StartSessionCallbackRequest",
                          handler => EMSPAPI.OnStartSessionCallbackRequest += handler,
                          handler => EMSPAPI.OnStartSessionCallbackRequest -= handler,
                          "StartSessionCallback", "SessionCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("StartSessionCallbackResponse",
                          handler => EMSPAPI.OnStartSessionCallbackResponse += handler,
                          handler => EMSPAPI.OnStartSessionCallbackResponse -= handler,
                          "StartSessionCallback", "SessionCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("StopSessionCallbackRequest",
                          handler => EMSPAPI.OnStopSessionCallbackRequest += handler,
                          handler => EMSPAPI.OnStopSessionCallbackRequest -= handler,
                          "StopSessionCallback", "SessionCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("StopSessionCallbackResponse",
                          handler => EMSPAPI.OnStopSessionCallbackResponse += handler,
                          handler => EMSPAPI.OnStopSessionCallbackResponse -= handler,
                          "StopSessionCallback", "SessionCallbacks", "CaCallbackslback", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("UnlockConnectorCallbackRequest",
                          handler => EMSPAPI.OnUnlockConnectorCallbackRequest += handler,
                          handler => EMSPAPI.OnUnlockConnectorCallbackRequest -= handler,
                          "UnlockConnectorCallback", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("UnlockConnectorCallbackResponse",
                          handler => EMSPAPI.OnUnlockConnectorCallbackResponse += handler,
                          handler => EMSPAPI.OnUnlockConnectorCallbackResponse -= handler,
                          "UnlockConnectorCallback", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

    }

}
