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
    /// A HUB API logger.
    /// </summary>
    public sealed class HUB_HTTPAPI_Logger : CommonAPILogger
    {

        #region Data

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public new const String DefaultContext = "HUBAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked HUB HTTP API.
        /// </summary>
        public HUB_HTTPAPI  HUB_HTTPAPI  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new HUB HTTP API logger using the default logging delegates.
        /// </summary>
        /// <param name="HUB_HTTPAPI">An HUB HTTP API.</param>
        /// <param name="Context">A context of this HTTP API.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public HUB_HTTPAPI_Logger(HUB_HTTPAPI                  HUB_HTTPAPI,
                                  String?                      Context          = DefaultContext,
                                  String?                      LoggingPath      = null,
                                  OCPILogfileCreatorDelegate?  LogfileCreator   = null)

            : base(HUB_HTTPAPI.CommonAPI,
                   Context ?? DefaultContext,
                   LoggingPath,
                   LogfileCreator)

        {

            this.HUB_HTTPAPI = HUB_HTTPAPI ?? throw new ArgumentNullException(nameof(HUB_HTTPAPI), "The given HUB HTTP API must not be null!");

            // CPO Events

            #region Location(s)

            RegisterEvent("CPO.GetLocationsRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetLocationsHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetLocationsHTTPRequest -= handler,
                          "GetLocations", "Locations", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetLocationsResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetLocationsHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetLocationsHTTPResponse -= handler,
                          "GetLocations", "Locations", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Location

            RegisterEvent("CPO.GetLocationRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetLocationHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetLocationHTTPRequest -= handler,
                          "GetLocation", "Locations", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetLocationResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetLocationHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetLocationHTTPResponse -= handler,
                          "GetLocation", "Locations", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region EVSE

            RegisterEvent("CPO.GetEVSERequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetEVSEHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetEVSEHTTPRequest -= handler,
                          "GetEVSE", "EVSEs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetEVSEResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetEVSEHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetEVSEHTTPResponse -= handler,
                          "GetEVSE", "EVSEs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Connector

            RegisterEvent("CPO.GetConnectorRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetConnectorHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetConnectorHTTPRequest -= handler,
                          "GetConnector", "Connectors", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetConnectorResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetConnectorHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetConnectorHTTPResponse -= handler,
                          "GetConnector", "Connectors", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Tariff(s)

            RegisterEvent("CPO.GetTariffsRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTariffsHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTariffsHTTPRequest -= handler,
                          "GetTariffs", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetTariffsResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTariffsHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTariffsHTTPResponse -= handler,
                          "GetTariffs", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Tariff

            RegisterEvent("CPO.GetTariffRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTariffHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTariffHTTPRequest -= handler,
                          "GetTariff", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetTariffResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTariffHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTariffHTTPResponse -= handler,
                          "GetTariff", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Session(s)

            RegisterEvent("CPO.GetSessionsRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetSessionsHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetSessionsHTTPRequest -= handler,
                          "GetSessions", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetSessionsResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetSessionsHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetSessionsHTTPResponse -= handler,
                          "GetSessions", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Session

            RegisterEvent("CPO.GetSessionRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetSessionHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetSessionHTTPRequest -= handler,
                          "GetSession", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetSessionResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetSessionHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetSessionHTTPResponse -= handler,
                          "GetSession", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region CDR(s)

            RegisterEvent("CPO.GetCDRsRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetCDRsHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetCDRsHTTPRequest -= handler,
                          "GetCDRs", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetCDRsResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetCDRsHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetCDRsHTTPResponse -= handler,
                          "GetCDRs", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // CDR

            RegisterEvent("CPO.GetCDRRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetCDRHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetCDRHTTPRequest -= handler,
                          "GetCDR", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetCDRResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetCDRHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetCDRHTTPResponse -= handler,
                          "GetCDR", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Token(s)

            RegisterEvent("CPO.GetTokensRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTokensHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTokensHTTPRequest -= handler,
                          "GetTokens", "Tokens", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetTokensResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTokensHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTokensHTTPResponse -= handler,
                          "GetTokens", "Tokens", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("CPO.DeleteTokensRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnDeleteTokensHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnDeleteTokensHTTPRequest -= handler,
                          "DeleteTokens", "Tokens", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.DeleteTokensResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnDeleteTokensHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnDeleteTokensHTTPResponse -= handler,
                          "DeleteTokens", "Tokens", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Token

            RegisterEvent("CPO.GetTokenRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTokenHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTokenHTTPRequest -= handler,
                          "GetToken", "Tokens", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetTokenResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTokenHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnGetTokenHTTPResponse -= handler,
                          "GetToken", "Tokens", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("CPO.PutTokenRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnPutTokenHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnPutTokenHTTPRequest -= handler,
                          "PutToken", "Tokens", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.PutTokenResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnPutTokenHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnPutTokenHTTPResponse -= handler,
                          "PutToken", "Tokens", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("CPO.PatchTokenRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnPatchTokenHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnPatchTokenHTTPRequest -= handler,
                          "PatchToken", "Tokens", "Patch", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.PatchTokenResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnPatchTokenHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnPatchTokenHTTPResponse -= handler,
                          "PatchToken", "Tokens", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("CPO.DeleteTokenRequest",
                          handler => HUB_HTTPAPI.CPOEvents.OnDeleteTokenHTTPRequest += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnDeleteTokenHTTPRequest -= handler,
                          "DeleteToken", "Tokens", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.DeleteTokenResponse",
                          handler => HUB_HTTPAPI.CPOEvents.OnDeleteTokenHTTPResponse += handler,
                          handler => HUB_HTTPAPI.CPOEvents.OnDeleteTokenHTTPResponse -= handler,
                          "DeleteToken", "Tokens", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion


            // EMSP Events

            #region Location(s)

            RegisterEvent("EMSP.GetLocationsRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetLocationsHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetLocationsHTTPRequest -= handler,
                          "GetLocations", "Locations", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetLocationsResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetLocationsHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetLocationsHTTPResponse -= handler,
                          "GetLocations", "Locations", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteLocationsRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteLocationsHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteLocationsHTTPRequest -= handler,
                          "DeleteLocations", "Locations", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteLocationsResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteLocationsHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteLocationsHTTPResponse -= handler,
                          "DeleteLocations", "Locations", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Location
            RegisterEvent("EMSP.GetLocationRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetLocationHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetLocationHTTPRequest -= handler,
                          "GetLocation", "Location", "Get", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetLocationResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetLocationHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetLocationHTTPResponse -= handler,
                          "GetLocation", "Location", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PutLocationRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutLocationHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutLocationHTTPRequest -= handler,
                          "PutLocation", "Locations", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PutLocationResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutLocationHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutLocationHTTPResponse -= handler,
                          "PutLocation", "Locations", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PatchLocationRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchLocationHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchLocationHTTPRequest -= handler,
                          "PatchLocation", "Locations", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PatchLocationResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchLocationHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchLocationHTTPResponse -= handler,
                          "PatchLocation", "Locations", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteLocationRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteLocationHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteLocationHTTPRequest -= handler,
                          "DeleteLocation", "Locations", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteLocationResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteLocationHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteLocationHTTPResponse -= handler,
                          "DeleteLocation", "Locations", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region EVSE/EVSE status

            RegisterEvent("EMSP.GetEVSERequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetEVSEHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetEVSEHTTPRequest -= handler,
                          "GetEVSE", "EVSEs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetEVSEResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetEVSEHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetEVSEHTTPResponse -= handler,
                          "GetEVSE", "EVSEs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PutEVSERequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutEVSEHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutEVSEHTTPRequest -= handler,
                          "PutEVSE", "EVSEs", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PutEVSEResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutEVSEHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutEVSEHTTPResponse -= handler,
                          "PutEVSE", "EVSEs", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PatchEVSERequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchEVSEHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchEVSEHTTPRequest -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PatchEVSEResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchEVSEHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchEVSEHTTPResponse -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteEVSERequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteEVSEHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteEVSEHTTPRequest -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteEVSEResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteEVSEHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteEVSEHTTPResponse -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // EVSE status

            RegisterEvent("EMSP.PostEVSEStatusRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPostEVSEStatusHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPostEVSEStatusHTTPRequest -= handler,
                          "PostEVSEStatus", "EVSEs", "Post", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PostEVSEStatusResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPostEVSEStatusHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPostEVSEStatusHTTPResponse -= handler,
                          "PostEVSEStatus", "EVSEs", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);
            #endregion

            #region Connector

            RegisterEvent("EMSP.GetConnectorRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetConnectorHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetConnectorHTTPRequest -= handler,
                          "GetConnector", "Connectors", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetConnectorResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetConnectorHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetConnectorHTTPResponse -= handler,
                          "GetConnector", "Connectors", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PutConnectorRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutConnectorHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutConnectorHTTPRequest -= handler,
                          "PutConnector", "Connectors", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PutConnectorResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutConnectorHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutConnectorHTTPResponse -= handler,
                          "PutConnector", "Connectors", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PatchConnectorRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchConnectorHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchConnectorHTTPRequest -= handler,
                          "PatchConnector", "Connectors", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PatchConnectorResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchConnectorHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchConnectorHTTPResponse -= handler,
                          "PatchConnector", "Connectors", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteConnectorRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteConnectorHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteConnectorHTTPRequest -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteConnectorResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteConnectorHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteConnectorHTTPResponse -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Tariff(s)

            RegisterEvent("EMSP.GetTariffsRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetTariffsHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetTariffsHTTPRequest -= handler,
                          "GetTariffs", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetTariffsResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetTariffsHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetTariffsHTTPResponse -= handler,
                          "GetTariffs", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteTariffsRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteTariffsHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteTariffsHTTPRequest -= handler,
                          "DeleteTariffs", "Tariffs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteTariffsResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteTariffsHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteTariffsHTTPResponse -= handler,
                          "DeleteTariffs", "Tariffs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Tariff

            RegisterEvent("EMSP.GetTariffRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetTariffHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetTariffHTTPRequest -= handler,
                          "GetTariff", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetTariffResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetTariffHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetTariffHTTPResponse -= handler,
                          "GetTariff", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PutTariffRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutTariffHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutTariffHTTPRequest -= handler,
                          "PutTariff", "Tariffs", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PutTariffResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutTariffHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutTariffHTTPResponse -= handler,
                          "PutTariff", "Tariffs", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PatchTariffRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchTariffHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchTariffHTTPRequest -= handler,
                          "PatchTariff", "Tariffs", "Patch", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PatchTariffResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchTariffHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchTariffHTTPResponse -= handler,
                          "PatchTariff", "Tariffs", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteTariffRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteTariffHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteTariffHTTPRequest -= handler,
                          "DeleteTariff", "Tariffs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteTariffResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteTariffHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteTariffHTTPResponse -= handler,
                          "DeleteTariff", "Tariffs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Session(s)

            RegisterEvent("EMSP.GetSessionsRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetSessionsHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetSessionsHTTPRequest -= handler,
                          "GetSessions", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetSessionsResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetSessionsHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetSessionsHTTPResponse -= handler,
                          "GetSessions", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteSessionsRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteSessionsHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteSessionsHTTPRequest -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteSessionsResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteSessionsHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteSessionsHTTPResponse -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Session

            RegisterEvent("EMSP.GetSessionRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetSessionHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetSessionHTTPRequest -= handler,
                          "GetSession", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetSessionResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetSessionHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetSessionHTTPResponse -= handler,
                          "GetSession", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PutSessionRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutSessionHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutSessionHTTPRequest -= handler,
                          "PutSession", "Sessions", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PutSessionResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutSessionHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPutSessionHTTPResponse -= handler,
                          "PutSession", "Sessions", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PatchSessionRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchSessionHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchSessionHTTPRequest -= handler,
                          "PatchSession", "Sessions", "Patch", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PatchSessionResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchSessionHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPatchSessionHTTPResponse -= handler,
                          "PatchSession", "Sessions", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteSessionRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteSessionHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteSessionHTTPRequest -= handler,
                          "DeleteSession", "Sessions", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteSessionResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteSessionHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteSessionHTTPResponse -= handler,
                          "DeleteSession", "Sessions", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region CDR(s)

            RegisterEvent("EMSP.GetCDRsRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetCDRsHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetCDRsHTTPRequest -= handler,
                          "GetCDRs", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetCDRsResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetCDRsHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetCDRsHTTPResponse -= handler,
                          "GetCDRs", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PostCDRRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPostCDRHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPostCDRHTTPRequest -= handler,
                          "PostCDR", "CDRs", "Post", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PostCDRResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPostCDRHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPostCDRHTTPResponse -= handler,
                          "PostCDR", "CDRs", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteCDRsRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteCDRsHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteCDRsHTTPRequest -= handler,
                          "DeleteCDRs", "CDRs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteCDRsResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteCDRsHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteCDRsHTTPResponse -= handler,
                          "DeleteCDRs", "CDRs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);



            // CDR

            RegisterEvent("EMSP.GetCDRRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetCDRHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetCDRHTTPRequest -= handler,
                          "GetCDR", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetCDRResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetCDRHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetCDRHTTPResponse -= handler,
                          "GetCDR", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteCDRRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteCDRHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteCDRHTTPRequest -= handler,
                          "DeleteCDR", "CDRs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteCDRResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteCDRHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnDeleteCDRHTTPResponse -= handler,
                          "DeleteCDR", "CDRs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Token(s)

            RegisterEvent("EMSP.GetTokensRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetTokensHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetTokensHTTPRequest -= handler,
                          "GetTokens", "Tokens", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetTokensResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetTokensHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnGetTokensHTTPResponse -= handler,
                          "GetTokens", "Tokens", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Token

            RegisterEvent("EMSP.PostTokenRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPostTokenHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPostTokenHTTPRequest -= handler,
                          "PostToken", "Tokens", "Post", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PostTokenResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnPostTokenHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnPostTokenHTTPResponse -= handler,
                          "PostToken", "Tokens", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Command callbacks

            RegisterEvent("EMSP.ReserveNowCallbackRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnReserveNowCallbackHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnReserveNowCallbackHTTPRequest -= handler,
                          "ReserveNowCallback", "ReservationCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.ReserveNowCallbackResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnReserveNowCallbackHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnReserveNowCallbackHTTPResponse -= handler,
                          "ReserveNowCallback", "ReservationCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.CancelReservationCallbackRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnCancelReservationCallbackHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnCancelReservationCallbackHTTPRequest -= handler,
                          "CancelReservationCallback", "ReservationCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.CancelReservationCallbackResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnCancelReservationCallbackHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnCancelReservationCallbackHTTPResponse -= handler,
                          "CancelReservationCallback", "ReservationCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.StartSessionCallbackRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnStartSessionCallbackHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnStartSessionCallbackHTTPRequest -= handler,
                          "StartSessionCallback", "SessionCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.StartSessionCallbackResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnStartSessionCallbackHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnStartSessionCallbackHTTPResponse -= handler,
                          "StartSessionCallback", "SessionCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.StopSessionCallbackRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnStopSessionCallbackHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnStopSessionCallbackHTTPRequest -= handler,
                          "StopSessionCallback", "SessionCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.StopSessionCallbackResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnStopSessionCallbackHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnStopSessionCallbackHTTPResponse -= handler,
                          "StopSessionCallback", "SessionCallbacks", "CaCallbackslback", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.UnlockConnectorCallbackRequest",
                          handler => HUB_HTTPAPI.EMSPEvents.OnUnlockConnectorCallbackHTTPRequest += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnUnlockConnectorCallbackHTTPRequest -= handler,
                          "UnlockConnectorCallback", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.UnlockConnectorCallbackResponse",
                          handler => HUB_HTTPAPI.EMSPEvents.OnUnlockConnectorCallbackHTTPResponse += handler,
                          handler => HUB_HTTPAPI.EMSPEvents.OnUnlockConnectorCallbackHTTPResponse -= handler,
                          "UnlockConnectorCallback", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

    }

}
