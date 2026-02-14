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

namespace cloud.charging.open.protocols.OCPIv2_3_0
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
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetLocationsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetLocationsHTTPRequest -= handler,
                          "GetLocations", "Locations", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetLocationsResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetLocationsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetLocationsHTTPResponse -= handler,
                          "GetLocations", "Locations", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteLocationsRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteLocationsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteLocationsHTTPRequest -= handler,
                          "DeleteLocations", "Locations", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteLocationsResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteLocationsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteLocationsHTTPResponse -= handler,
                          "DeleteLocations", "Locations", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Location
            RegisterEvent("GetLocationRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetLocationHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetLocationHTTPRequest -= handler,
                          "GetLocation", "Location", "Get", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetLocationResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetLocationHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetLocationHTTPResponse -= handler,
                          "GetLocation", "Location", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutLocationRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutLocationHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutLocationHTTPRequest -= handler,
                          "PutLocation", "Locations", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutLocationResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutLocationHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutLocationHTTPResponse -= handler,
                          "PutLocation", "Locations", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchLocationRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchLocationHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchLocationHTTPRequest -= handler,
                          "PatchLocation", "Locations", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchLocationResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchLocationHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchLocationHTTPResponse -= handler,
                          "PatchLocation", "Locations", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteLocationRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteLocationHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteLocationHTTPRequest -= handler,
                          "DeleteLocation", "Locations", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteLocationResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteLocationHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteLocationHTTPResponse -= handler,
                          "DeleteLocation", "Locations", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region EVSE/EVSE status

            RegisterEvent("GetEVSERequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetEVSEHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetEVSEHTTPRequest -= handler,
                          "GetEVSE", "EVSEs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetEVSEResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetEVSEHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetEVSEHTTPResponse -= handler,
                          "GetEVSE", "EVSEs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutEVSERequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutEVSEHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutEVSEHTTPRequest -= handler,
                          "PutEVSE", "EVSEs", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutEVSEResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutEVSEHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutEVSEHTTPResponse -= handler,
                          "PutEVSE", "EVSEs", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchEVSERequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchEVSEHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchEVSEHTTPRequest -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchEVSEResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchEVSEHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchEVSEHTTPResponse -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteEVSERequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteEVSEHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteEVSEHTTPRequest -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteEVSEResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteEVSEHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteEVSEHTTPResponse -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // EVSE status

            RegisterEvent("PostEVSEStatusRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPostEVSEStatusHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPostEVSEStatusHTTPRequest -= handler,
                          "PostEVSEStatus", "EVSEs", "Post", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PostEVSEStatusResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPostEVSEStatusHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPostEVSEStatusHTTPResponse -= handler,
                          "PostEVSEStatus", "EVSEs", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);
            #endregion

            #region Connector

            RegisterEvent("GetConnectorRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetConnectorHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetConnectorHTTPRequest -= handler,
                          "GetConnector", "Connectors", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetConnectorResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetConnectorHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetConnectorHTTPResponse -= handler,
                          "GetConnector", "Connectors", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutConnectorRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutConnectorHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutConnectorHTTPRequest -= handler,
                          "PutConnector", "Connectors", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutConnectorResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutConnectorHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutConnectorHTTPResponse -= handler,
                          "PutConnector", "Connectors", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchConnectorRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchConnectorHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchConnectorHTTPRequest -= handler,
                          "PatchConnector", "Connectors", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchConnectorResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchConnectorHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchConnectorHTTPResponse -= handler,
                          "PatchConnector", "Connectors", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteConnectorRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteConnectorHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteConnectorHTTPRequest -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteConnectorResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteConnectorHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteConnectorHTTPResponse -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Tariff(s)

            RegisterEvent("GetTariffsRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetTariffsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetTariffsHTTPRequest -= handler,
                          "GetTariffs", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTariffsResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetTariffsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetTariffsHTTPResponse -= handler,
                          "GetTariffs", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteTariffsRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteTariffsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteTariffsHTTPRequest -= handler,
                          "DeleteTariffs", "Tariffs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteTariffsResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteTariffsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteTariffsHTTPResponse -= handler,
                          "DeleteTariffs", "Tariffs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Tariff

            RegisterEvent("GetTariffRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetTariffHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetTariffHTTPRequest -= handler,
                          "GetTariff", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTariffResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetTariffHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetTariffHTTPResponse -= handler,
                          "GetTariff", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutTariffRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutTariffHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutTariffHTTPRequest -= handler,
                          "PutTariff", "Tariffs", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutTariffResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutTariffHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutTariffHTTPResponse -= handler,
                          "PutTariff", "Tariffs", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchTariffRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchTariffHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchTariffHTTPRequest -= handler,
                          "PatchTariff", "Tariffs", "Patch", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchTariffResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchTariffHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchTariffHTTPResponse -= handler,
                          "PatchTariff", "Tariffs", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteTariffRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteTariffHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteTariffHTTPRequest -= handler,
                          "DeleteTariff", "Tariffs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteTariffResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteTariffHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteTariffHTTPResponse -= handler,
                          "DeleteTariff", "Tariffs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Session(s)

            RegisterEvent("GetSessionsRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetSessionsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetSessionsHTTPRequest -= handler,
                          "GetSessions", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetSessionsResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetSessionsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetSessionsHTTPResponse -= handler,
                          "GetSessions", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteSessionsRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteSessionsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteSessionsHTTPRequest -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteSessionsResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteSessionsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteSessionsHTTPResponse -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Session

            RegisterEvent("GetSessionRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetSessionHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetSessionHTTPRequest -= handler,
                          "GetSession", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetSessionResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetSessionHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetSessionHTTPResponse -= handler,
                          "GetSession", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutSessionRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutSessionHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutSessionHTTPRequest -= handler,
                          "PutSession", "Sessions", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutSessionResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutSessionHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPutSessionHTTPResponse -= handler,
                          "PutSession", "Sessions", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchSessionRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchSessionHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchSessionHTTPRequest -= handler,
                          "PatchSession", "Sessions", "Patch", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchSessionResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchSessionHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPatchSessionHTTPResponse -= handler,
                          "PatchSession", "Sessions", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteSessionRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteSessionHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteSessionHTTPRequest -= handler,
                          "DeleteSession", "Sessions", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteSessionResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteSessionHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteSessionHTTPResponse -= handler,
                          "DeleteSession", "Sessions", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region CDR(s)

            RegisterEvent("GetCDRsRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetCDRsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetCDRsHTTPRequest -= handler,
                          "GetCDRs", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetCDRsResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetCDRsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetCDRsHTTPResponse -= handler,
                          "GetCDRs", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PostCDRRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPostCDRHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPostCDRHTTPRequest -= handler,
                          "PostCDR", "CDRs", "Post", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PostCDRResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPostCDRHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPostCDRHTTPResponse -= handler,
                          "PostCDR", "CDRs", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteCDRsRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteCDRsHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteCDRsHTTPRequest -= handler,
                          "DeleteCDRs", "CDRs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteCDRsResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteCDRsHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteCDRsHTTPResponse -= handler,
                          "DeleteCDRs", "CDRs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);



            // CDR

            RegisterEvent("GetCDRRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetCDRHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetCDRHTTPRequest -= handler,
                          "GetCDR", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetCDRResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetCDRHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetCDRHTTPResponse -= handler,
                          "GetCDR", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteCDRRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteCDRHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteCDRHTTPRequest -= handler,
                          "DeleteCDR", "CDRs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteCDRResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteCDRHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnDeleteCDRHTTPResponse -= handler,
                          "DeleteCDR", "CDRs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Token(s)

            RegisterEvent("GetTokensRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetTokensHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetTokensHTTPRequest -= handler,
                          "GetTokens", "Tokens", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTokensResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetTokensHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnGetTokensHTTPResponse -= handler,
                          "GetTokens", "Tokens", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Token

            RegisterEvent("PostTokenRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPostTokenHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPostTokenHTTPRequest -= handler,
                          "PostToken", "Tokens", "Post", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PostTokenResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPostTokenHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnPostTokenHTTPResponse -= handler,
                          "PostToken", "Tokens", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Command callbacks

            RegisterEvent("ReserveNowCallbackRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnReserveNowCallbackHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnReserveNowCallbackHTTPRequest -= handler,
                          "ReserveNowCallback", "ReservationCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("ReserveNowCallbackResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnReserveNowCallbackHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnReserveNowCallbackHTTPResponse -= handler,
                          "ReserveNowCallback", "ReservationCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("CancelReservationCallbackRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnCancelReservationCallbackHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnCancelReservationCallbackHTTPRequest -= handler,
                          "CancelReservationCallback", "ReservationCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CancelReservationCallbackResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnCancelReservationCallbackHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnCancelReservationCallbackHTTPResponse -= handler,
                          "CancelReservationCallback", "ReservationCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("StartSessionCallbackRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnStartSessionCallbackHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnStartSessionCallbackHTTPRequest -= handler,
                          "StartSessionCallback", "SessionCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("StartSessionCallbackResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnStartSessionCallbackHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnStartSessionCallbackHTTPResponse -= handler,
                          "StartSessionCallback", "SessionCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("StopSessionCallbackRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnStopSessionCallbackHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnStopSessionCallbackHTTPRequest -= handler,
                          "StopSessionCallback", "SessionCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("StopSessionCallbackResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnStopSessionCallbackHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnStopSessionCallbackHTTPResponse -= handler,
                          "StopSessionCallback", "SessionCallbacks", "CaCallbackslback", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("UnlockConnectorCallbackRequest",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnUnlockConnectorCallbackHTTPRequest += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnUnlockConnectorCallbackHTTPRequest -= handler,
                          "UnlockConnectorCallback", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("UnlockConnectorCallbackResponse",
                          handler => EMSP_HTTPAPI.HTTPEvents.OnUnlockConnectorCallbackHTTPResponse += handler,
                          handler => EMSP_HTTPAPI.HTTPEvents.OnUnlockConnectorCallbackHTTPResponse -= handler,
                          "UnlockConnectorCallback", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

    }

}
