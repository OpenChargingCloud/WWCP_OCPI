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
    public sealed class HUBAPILogger : CommonAPILogger
    {

        #region Data

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public new const String DefaultContext = "HUBAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked HUB API.
        /// </summary>
        public HUBAPI  HUBAPI  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new HUB API logger using the default logging delegates.
        /// </summary>
        /// <param name="HUBAPI">An HUB API.</param>
        /// <param name="Context">A context of this API.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public HUBAPILogger(HUBAPI                       HUBAPI,
                            String?                      Context          = DefaultContext,
                            String?                      LoggingPath      = null,
                            OCPILogfileCreatorDelegate?  LogfileCreator   = null)

            : base(HUBAPI.CommonAPI,
                   Context ?? DefaultContext,
                   LoggingPath,
                   LogfileCreator)

        {

            this.HUBAPI = HUBAPI ?? throw new ArgumentNullException(nameof(HUBAPI), "The given HUB API must not be null!");

            // CPO Events

            #region Location(s)

            RegisterEvent("CPO.GetLocationsRequest",
                          handler => HUBAPI.CPOEvents.OnGetLocationsRequest += handler,
                          handler => HUBAPI.CPOEvents.OnGetLocationsRequest -= handler,
                          "GetLocations", "Locations", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetLocationsResponse",
                          handler => HUBAPI.CPOEvents.OnGetLocationsResponse += handler,
                          handler => HUBAPI.CPOEvents.OnGetLocationsResponse -= handler,
                          "GetLocations", "Locations", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Location

            RegisterEvent("CPO.GetLocationRequest",
                          handler => HUBAPI.CPOEvents.OnGetLocationRequest += handler,
                          handler => HUBAPI.CPOEvents.OnGetLocationRequest -= handler,
                          "GetLocation", "Locations", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetLocationResponse",
                          handler => HUBAPI.CPOEvents.OnGetLocationResponse += handler,
                          handler => HUBAPI.CPOEvents.OnGetLocationResponse -= handler,
                          "GetLocation", "Locations", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region EVSE

            RegisterEvent("CPO.GetEVSERequest",
                          handler => HUBAPI.CPOEvents.OnGetEVSERequest += handler,
                          handler => HUBAPI.CPOEvents.OnGetEVSERequest -= handler,
                          "GetEVSE", "EVSEs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetEVSEResponse",
                          handler => HUBAPI.CPOEvents.OnGetEVSEResponse += handler,
                          handler => HUBAPI.CPOEvents.OnGetEVSEResponse -= handler,
                          "GetEVSE", "EVSEs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Connector

            RegisterEvent("CPO.GetConnectorRequest",
                          handler => HUBAPI.CPOEvents.OnGetConnectorRequest += handler,
                          handler => HUBAPI.CPOEvents.OnGetConnectorRequest -= handler,
                          "GetConnector", "Connectors", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetConnectorResponse",
                          handler => HUBAPI.CPOEvents.OnGetConnectorResponse += handler,
                          handler => HUBAPI.CPOEvents.OnGetConnectorResponse -= handler,
                          "GetConnector", "Connectors", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Tariff(s)

            RegisterEvent("CPO.GetTariffsRequest",
                          handler => HUBAPI.CPOEvents.OnGetTariffsRequest += handler,
                          handler => HUBAPI.CPOEvents.OnGetTariffsRequest -= handler,
                          "GetTariffs", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetTariffsResponse",
                          handler => HUBAPI.CPOEvents.OnGetTariffsResponse += handler,
                          handler => HUBAPI.CPOEvents.OnGetTariffsResponse -= handler,
                          "GetTariffs", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Tariff

            RegisterEvent("CPO.GetTariffRequest",
                          handler => HUBAPI.CPOEvents.OnGetTariffRequest += handler,
                          handler => HUBAPI.CPOEvents.OnGetTariffRequest -= handler,
                          "GetTariff", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetTariffResponse",
                          handler => HUBAPI.CPOEvents.OnGetTariffResponse += handler,
                          handler => HUBAPI.CPOEvents.OnGetTariffResponse -= handler,
                          "GetTariff", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Session(s)

            RegisterEvent("CPO.GetSessionsRequest",
                          handler => HUBAPI.CPOEvents.OnGetSessionsRequest += handler,
                          handler => HUBAPI.CPOEvents.OnGetSessionsRequest -= handler,
                          "GetSessions", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetSessionsResponse",
                          handler => HUBAPI.CPOEvents.OnGetSessionsResponse += handler,
                          handler => HUBAPI.CPOEvents.OnGetSessionsResponse -= handler,
                          "GetSessions", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Session

            RegisterEvent("CPO.GetSessionRequest",
                          handler => HUBAPI.CPOEvents.OnGetSessionRequest += handler,
                          handler => HUBAPI.CPOEvents.OnGetSessionRequest -= handler,
                          "GetSession", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetSessionResponse",
                          handler => HUBAPI.CPOEvents.OnGetSessionResponse += handler,
                          handler => HUBAPI.CPOEvents.OnGetSessionResponse -= handler,
                          "GetSession", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region CDR(s)

            RegisterEvent("CPO.GetCDRsRequest",
                          handler => HUBAPI.CPOEvents.OnGetCDRsRequest += handler,
                          handler => HUBAPI.CPOEvents.OnGetCDRsRequest -= handler,
                          "GetCDRs", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetCDRsResponse",
                          handler => HUBAPI.CPOEvents.OnGetCDRsResponse += handler,
                          handler => HUBAPI.CPOEvents.OnGetCDRsResponse -= handler,
                          "GetCDRs", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // CDR

            RegisterEvent("CPO.GetCDRRequest",
                          handler => HUBAPI.CPOEvents.OnGetCDRRequest += handler,
                          handler => HUBAPI.CPOEvents.OnGetCDRRequest -= handler,
                          "GetCDR", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetCDRResponse",
                          handler => HUBAPI.CPOEvents.OnGetCDRResponse += handler,
                          handler => HUBAPI.CPOEvents.OnGetCDRResponse -= handler,
                          "GetCDR", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Token(s)

            RegisterEvent("CPO.GetTokensRequest",
                          handler => HUBAPI.CPOEvents.OnGetTokensRequest += handler,
                          handler => HUBAPI.CPOEvents.OnGetTokensRequest -= handler,
                          "GetTokens", "Tokens", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetTokensResponse",
                          handler => HUBAPI.CPOEvents.OnGetTokensResponse += handler,
                          handler => HUBAPI.CPOEvents.OnGetTokensResponse -= handler,
                          "GetTokens", "Tokens", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("CPO.DeleteTokensRequest",
                          handler => HUBAPI.CPOEvents.OnDeleteTokensRequest += handler,
                          handler => HUBAPI.CPOEvents.OnDeleteTokensRequest -= handler,
                          "DeleteTokens", "Tokens", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.DeleteTokensResponse",
                          handler => HUBAPI.CPOEvents.OnDeleteTokensResponse += handler,
                          handler => HUBAPI.CPOEvents.OnDeleteTokensResponse -= handler,
                          "DeleteTokens", "Tokens", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Token

            RegisterEvent("CPO.GetTokenRequest",
                          handler => HUBAPI.CPOEvents.OnGetTokenRequest += handler,
                          handler => HUBAPI.CPOEvents.OnGetTokenRequest -= handler,
                          "GetToken", "Tokens", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.GetTokenResponse",
                          handler => HUBAPI.CPOEvents.OnGetTokenResponse += handler,
                          handler => HUBAPI.CPOEvents.OnGetTokenResponse -= handler,
                          "GetToken", "Tokens", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("CPO.PutTokenRequest",
                          handler => HUBAPI.CPOEvents.OnPutTokenRequest += handler,
                          handler => HUBAPI.CPOEvents.OnPutTokenRequest -= handler,
                          "PutToken", "Tokens", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.PutTokenResponse",
                          handler => HUBAPI.CPOEvents.OnPutTokenResponse += handler,
                          handler => HUBAPI.CPOEvents.OnPutTokenResponse -= handler,
                          "PutToken", "Tokens", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("CPO.PatchTokenRequest",
                          handler => HUBAPI.CPOEvents.OnPatchTokenRequest += handler,
                          handler => HUBAPI.CPOEvents.OnPatchTokenRequest -= handler,
                          "PatchToken", "Tokens", "Patch", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.PatchTokenResponse",
                          handler => HUBAPI.CPOEvents.OnPatchTokenResponse += handler,
                          handler => HUBAPI.CPOEvents.OnPatchTokenResponse -= handler,
                          "PatchToken", "Tokens", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("CPO.DeleteTokenRequest",
                          handler => HUBAPI.CPOEvents.OnDeleteTokenRequest += handler,
                          handler => HUBAPI.CPOEvents.OnDeleteTokenRequest -= handler,
                          "DeleteToken", "Tokens", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("CPO.DeleteTokenResponse",
                          handler => HUBAPI.CPOEvents.OnDeleteTokenResponse += handler,
                          handler => HUBAPI.CPOEvents.OnDeleteTokenResponse -= handler,
                          "DeleteToken", "Tokens", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion


            // EMSP Events

            #region Location(s)

            RegisterEvent("EMSP.GetLocationsRequest",
                          handler => HUBAPI.EMSPEvents.OnGetLocationsRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnGetLocationsRequest -= handler,
                          "GetLocations", "Locations", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetLocationsResponse",
                          handler => HUBAPI.EMSPEvents.OnGetLocationsResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnGetLocationsResponse -= handler,
                          "GetLocations", "Locations", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteLocationsRequest",
                          handler => HUBAPI.EMSPEvents.OnDeleteLocationsRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteLocationsRequest -= handler,
                          "DeleteLocations", "Locations", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteLocationsResponse",
                          handler => HUBAPI.EMSPEvents.OnDeleteLocationsResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteLocationsResponse -= handler,
                          "DeleteLocations", "Locations", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Location
            RegisterEvent("EMSP.GetLocationRequest",
                          handler => HUBAPI.EMSPEvents.OnGetLocationRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnGetLocationRequest -= handler,
                          "GetLocation", "Location", "Get", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetLocationResponse",
                          handler => HUBAPI.EMSPEvents.OnGetLocationResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnGetLocationResponse -= handler,
                          "GetLocation", "Location", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PutLocationRequest",
                          handler => HUBAPI.EMSPEvents.OnPutLocationRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnPutLocationRequest -= handler,
                          "PutLocation", "Locations", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PutLocationResponse",
                          handler => HUBAPI.EMSPEvents.OnPutLocationResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnPutLocationResponse -= handler,
                          "PutLocation", "Locations", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PatchLocationRequest",
                          handler => HUBAPI.EMSPEvents.OnPatchLocationRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnPatchLocationRequest -= handler,
                          "PatchLocation", "Locations", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PatchLocationResponse",
                          handler => HUBAPI.EMSPEvents.OnPatchLocationResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnPatchLocationResponse -= handler,
                          "PatchLocation", "Locations", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteLocationRequest",
                          handler => HUBAPI.EMSPEvents.OnDeleteLocationRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteLocationRequest -= handler,
                          "DeleteLocation", "Locations", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteLocationResponse",
                          handler => HUBAPI.EMSPEvents.OnDeleteLocationResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteLocationResponse -= handler,
                          "DeleteLocation", "Locations", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region EVSE/EVSE status

            RegisterEvent("EMSP.GetEVSERequest",
                          handler => HUBAPI.EMSPEvents.OnGetEVSERequest += handler,
                          handler => HUBAPI.EMSPEvents.OnGetEVSERequest -= handler,
                          "GetEVSE", "EVSEs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetEVSEResponse",
                          handler => HUBAPI.EMSPEvents.OnGetEVSEResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnGetEVSEResponse -= handler,
                          "GetEVSE", "EVSEs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PutEVSERequest",
                          handler => HUBAPI.EMSPEvents.OnPutEVSERequest += handler,
                          handler => HUBAPI.EMSPEvents.OnPutEVSERequest -= handler,
                          "PutEVSE", "EVSEs", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PutEVSEResponse",
                          handler => HUBAPI.EMSPEvents.OnPutEVSEResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnPutEVSEResponse -= handler,
                          "PutEVSE", "EVSEs", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PatchEVSERequest",
                          handler => HUBAPI.EMSPEvents.OnPatchEVSERequest += handler,
                          handler => HUBAPI.EMSPEvents.OnPatchEVSERequest -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PatchEVSEResponse",
                          handler => HUBAPI.EMSPEvents.OnPatchEVSEResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnPatchEVSEResponse -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteEVSERequest",
                          handler => HUBAPI.EMSPEvents.OnDeleteEVSERequest += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteEVSERequest -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteEVSEResponse",
                          handler => HUBAPI.EMSPEvents.OnDeleteEVSEResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteEVSEResponse -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // EVSE status

            RegisterEvent("EMSP.PostEVSEStatusRequest",
                          handler => HUBAPI.EMSPEvents.OnPostEVSEStatusRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnPostEVSEStatusRequest -= handler,
                          "PostEVSEStatus", "EVSEs", "Post", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PostEVSEStatusResponse",
                          handler => HUBAPI.EMSPEvents.OnPostEVSEStatusResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnPostEVSEStatusResponse -= handler,
                          "PostEVSEStatus", "EVSEs", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);
            #endregion

            #region Connector

            RegisterEvent("EMSP.GetConnectorRequest",
                          handler => HUBAPI.EMSPEvents.OnGetConnectorRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnGetConnectorRequest -= handler,
                          "GetConnector", "Connectors", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetConnectorResponse",
                          handler => HUBAPI.EMSPEvents.OnGetConnectorResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnGetConnectorResponse -= handler,
                          "GetConnector", "Connectors", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PutConnectorRequest",
                          handler => HUBAPI.EMSPEvents.OnPutConnectorRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnPutConnectorRequest -= handler,
                          "PutConnector", "Connectors", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PutConnectorResponse",
                          handler => HUBAPI.EMSPEvents.OnPutConnectorResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnPutConnectorResponse -= handler,
                          "PutConnector", "Connectors", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PatchConnectorRequest",
                          handler => HUBAPI.EMSPEvents.OnPatchConnectorRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnPatchConnectorRequest -= handler,
                          "PatchConnector", "Connectors", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PatchConnectorResponse",
                          handler => HUBAPI.EMSPEvents.OnPatchConnectorResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnPatchConnectorResponse -= handler,
                          "PatchConnector", "Connectors", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteConnectorRequest",
                          handler => HUBAPI.EMSPEvents.OnDeleteConnectorRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteConnectorRequest -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteConnectorResponse",
                          handler => HUBAPI.EMSPEvents.OnDeleteConnectorResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteConnectorResponse -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Tariff(s)

            RegisterEvent("EMSP.GetTariffsRequest",
                          handler => HUBAPI.EMSPEvents.OnGetTariffsRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnGetTariffsRequest -= handler,
                          "GetTariffs", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetTariffsResponse",
                          handler => HUBAPI.EMSPEvents.OnGetTariffsResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnGetTariffsResponse -= handler,
                          "GetTariffs", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteTariffsRequest",
                          handler => HUBAPI.EMSPEvents.OnDeleteTariffsRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteTariffsRequest -= handler,
                          "DeleteTariffs", "Tariffs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteTariffsResponse",
                          handler => HUBAPI.EMSPEvents.OnDeleteTariffsResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteTariffsResponse -= handler,
                          "DeleteTariffs", "Tariffs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Tariff

            RegisterEvent("EMSP.GetTariffRequest",
                          handler => HUBAPI.EMSPEvents.OnGetTariffRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnGetTariffRequest -= handler,
                          "GetTariff", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetTariffResponse",
                          handler => HUBAPI.EMSPEvents.OnGetTariffResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnGetTariffResponse -= handler,
                          "GetTariff", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PutTariffRequest",
                          handler => HUBAPI.EMSPEvents.OnPutTariffRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnPutTariffRequest -= handler,
                          "PutTariff", "Tariffs", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PutTariffResponse",
                          handler => HUBAPI.EMSPEvents.OnPutTariffResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnPutTariffResponse -= handler,
                          "PutTariff", "Tariffs", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PatchTariffRequest",
                          handler => HUBAPI.EMSPEvents.OnPatchTariffRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnPatchTariffRequest -= handler,
                          "PatchTariff", "Tariffs", "Patch", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PatchTariffResponse",
                          handler => HUBAPI.EMSPEvents.OnPatchTariffResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnPatchTariffResponse -= handler,
                          "PatchTariff", "Tariffs", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteTariffRequest",
                          handler => HUBAPI.EMSPEvents.OnDeleteTariffRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteTariffRequest -= handler,
                          "DeleteTariff", "Tariffs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteTariffResponse",
                          handler => HUBAPI.EMSPEvents.OnDeleteTariffResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteTariffResponse -= handler,
                          "DeleteTariff", "Tariffs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Session(s)

            RegisterEvent("EMSP.GetSessionsRequest",
                          handler => HUBAPI.EMSPEvents.OnGetSessionsRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnGetSessionsRequest -= handler,
                          "GetSessions", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetSessionsResponse",
                          handler => HUBAPI.EMSPEvents.OnGetSessionsResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnGetSessionsResponse -= handler,
                          "GetSessions", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteSessionsRequest",
                          handler => HUBAPI.EMSPEvents.OnDeleteSessionsRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteSessionsRequest -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteSessionsResponse",
                          handler => HUBAPI.EMSPEvents.OnDeleteSessionsResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteSessionsResponse -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Session

            RegisterEvent("EMSP.GetSessionRequest",
                          handler => HUBAPI.EMSPEvents.OnGetSessionRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnGetSessionRequest -= handler,
                          "GetSession", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetSessionResponse",
                          handler => HUBAPI.EMSPEvents.OnGetSessionResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnGetSessionResponse -= handler,
                          "GetSession", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PutSessionRequest",
                          handler => HUBAPI.EMSPEvents.OnPutSessionRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnPutSessionRequest -= handler,
                          "PutSession", "Sessions", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PutSessionResponse",
                          handler => HUBAPI.EMSPEvents.OnPutSessionResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnPutSessionResponse -= handler,
                          "PutSession", "Sessions", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PatchSessionRequest",
                          handler => HUBAPI.EMSPEvents.OnPatchSessionRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnPatchSessionRequest -= handler,
                          "PatchSession", "Sessions", "Patch", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PatchSessionResponse",
                          handler => HUBAPI.EMSPEvents.OnPatchSessionResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnPatchSessionResponse -= handler,
                          "PatchSession", "Sessions", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteSessionRequest",
                          handler => HUBAPI.EMSPEvents.OnDeleteSessionRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteSessionRequest -= handler,
                          "DeleteSession", "Sessions", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteSessionResponse",
                          handler => HUBAPI.EMSPEvents.OnDeleteSessionResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteSessionResponse -= handler,
                          "DeleteSession", "Sessions", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region CDR(s)

            RegisterEvent("EMSP.GetCDRsRequest",
                          handler => HUBAPI.EMSPEvents.OnGetCDRsRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnGetCDRsRequest -= handler,
                          "GetCDRs", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetCDRsResponse",
                          handler => HUBAPI.EMSPEvents.OnGetCDRsResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnGetCDRsResponse -= handler,
                          "GetCDRs", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.PostCDRRequest",
                          handler => HUBAPI.EMSPEvents.OnPostCDRRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnPostCDRRequest -= handler,
                          "PostCDR", "CDRs", "Post", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PostCDRResponse",
                          handler => HUBAPI.EMSPEvents.OnPostCDRResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnPostCDRResponse -= handler,
                          "PostCDR", "CDRs", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteCDRsRequest",
                          handler => HUBAPI.EMSPEvents.OnDeleteCDRsRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteCDRsRequest -= handler,
                          "DeleteCDRs", "CDRs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteCDRsResponse",
                          handler => HUBAPI.EMSPEvents.OnDeleteCDRsResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteCDRsResponse -= handler,
                          "DeleteCDRs", "CDRs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);



            // CDR

            RegisterEvent("EMSP.GetCDRRequest",
                          handler => HUBAPI.EMSPEvents.OnGetCDRRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnGetCDRRequest -= handler,
                          "GetCDR", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetCDRResponse",
                          handler => HUBAPI.EMSPEvents.OnGetCDRResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnGetCDRResponse -= handler,
                          "GetCDR", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.DeleteCDRRequest",
                          handler => HUBAPI.EMSPEvents.OnDeleteCDRRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteCDRRequest -= handler,
                          "DeleteCDR", "CDRs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.DeleteCDRResponse",
                          handler => HUBAPI.EMSPEvents.OnDeleteCDRResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnDeleteCDRResponse -= handler,
                          "DeleteCDR", "CDRs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Token(s)

            RegisterEvent("EMSP.GetTokensRequest",
                          handler => HUBAPI.EMSPEvents.OnGetTokensRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnGetTokensRequest -= handler,
                          "GetTokens", "Tokens", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.GetTokensResponse",
                          handler => HUBAPI.EMSPEvents.OnGetTokensResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnGetTokensResponse -= handler,
                          "GetTokens", "Tokens", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Token

            RegisterEvent("EMSP.PostTokenRequest",
                          handler => HUBAPI.EMSPEvents.OnPostTokenRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnPostTokenRequest -= handler,
                          "PostToken", "Tokens", "Post", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.PostTokenResponse",
                          handler => HUBAPI.EMSPEvents.OnPostTokenResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnPostTokenResponse -= handler,
                          "PostToken", "Tokens", "Post", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Command callbacks

            RegisterEvent("EMSP.ReserveNowCallbackRequest",
                          handler => HUBAPI.EMSPEvents.OnReserveNowCallbackRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnReserveNowCallbackRequest -= handler,
                          "ReserveNowCallback", "ReservationCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.ReserveNowCallbackResponse",
                          handler => HUBAPI.EMSPEvents.OnReserveNowCallbackResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnReserveNowCallbackResponse -= handler,
                          "ReserveNowCallback", "ReservationCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.CancelReservationCallbackRequest",
                          handler => HUBAPI.EMSPEvents.OnCancelReservationCallbackRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnCancelReservationCallbackRequest -= handler,
                          "CancelReservationCallback", "ReservationCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.CancelReservationCallbackResponse",
                          handler => HUBAPI.EMSPEvents.OnCancelReservationCallbackResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnCancelReservationCallbackResponse -= handler,
                          "CancelReservationCallback", "ReservationCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.StartSessionCallbackRequest",
                          handler => HUBAPI.EMSPEvents.OnStartSessionCallbackRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnStartSessionCallbackRequest -= handler,
                          "StartSessionCallback", "SessionCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.StartSessionCallbackResponse",
                          handler => HUBAPI.EMSPEvents.OnStartSessionCallbackResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnStartSessionCallbackResponse -= handler,
                          "StartSessionCallback", "SessionCallbacks", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.StopSessionCallbackRequest",
                          handler => HUBAPI.EMSPEvents.OnStopSessionCallbackRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnStopSessionCallbackRequest -= handler,
                          "StopSessionCallback", "SessionCallbacks", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.StopSessionCallbackResponse",
                          handler => HUBAPI.EMSPEvents.OnStopSessionCallbackResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnStopSessionCallbackResponse -= handler,
                          "StopSessionCallback", "SessionCallbacks", "CaCallbackslback", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("EMSP.UnlockConnectorCallbackRequest",
                          handler => HUBAPI.EMSPEvents.OnUnlockConnectorCallbackRequest += handler,
                          handler => HUBAPI.EMSPEvents.OnUnlockConnectorCallbackRequest -= handler,
                          "UnlockConnectorCallback", "Callbacks", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("EMSP.UnlockConnectorCallbackResponse",
                          handler => HUBAPI.EMSPEvents.OnUnlockConnectorCallbackResponse += handler,
                          handler => HUBAPI.EMSPEvents.OnUnlockConnectorCallbackResponse -= handler,
                          "UnlockConnectorCallback", "Callbacks", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

    }

}
