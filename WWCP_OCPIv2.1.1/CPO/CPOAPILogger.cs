/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
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

#region Usings

using org.GraphDefined.Vanaheimr.Hermod.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.HTTP
{

    /// <summary>
    /// A CPO API logger.
    /// </summary>
    public class CPOAPILogger : CommonAPILogger
    {

        #region Data

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public new const String DefaultContext = "CPOAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked CPO API.
        /// </summary>
        public CPOAPI  CPOAPI  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CPO API logger using the default logging delegates.
        /// </summary>
        /// <param name="CPOAPI">An CPO API.</param>
        /// <param name="Context">A context of this API.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public CPOAPILogger(CPOAPI                   CPOAPI,
                            String                   LoggingPath,
                            String                   Context          = DefaultContext,
                            LogfileCreatorDelegate?  LogfileCreator   = null)

            : base(CPOAPI.CommonAPI,
                   LoggingPath,
                   Context,
                   LogfileCreator)

        {

            this.CPOAPI = CPOAPI ?? throw new ArgumentNullException(nameof(CPOAPI), "The given CPO API must not be null!");

            #region Location(s)

            RegisterEvent("GetLocationsRequest",
                          handler => CPOAPI.OnGetLocationsRequest += handler,
                          handler => CPOAPI.OnGetLocationsRequest -= handler,
                          "GetLocations", "Locations", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetLocationsResponse",
                          handler => CPOAPI.OnGetLocationsResponse += handler,
                          handler => CPOAPI.OnGetLocationsResponse -= handler,
                          "GetLocations", "Locations", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Location

            RegisterEvent("GetLocationRequest",
                          handler => CPOAPI.OnGetLocationRequest += handler,
                          handler => CPOAPI.OnGetLocationRequest -= handler,
                          "GetLocation", "Locations", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetLocationResponse",
                          handler => CPOAPI.OnGetLocationResponse += handler,
                          handler => CPOAPI.OnGetLocationResponse -= handler,
                          "GetLocation", "Locations", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region EVSE

            RegisterEvent("GetEVSERequest",
                          handler => CPOAPI.OnGetEVSERequest += handler,
                          handler => CPOAPI.OnGetEVSERequest -= handler,
                          "GetEVSE", "EVSEs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetEVSEResponse",
                          handler => CPOAPI.OnGetEVSEResponse += handler,
                          handler => CPOAPI.OnGetEVSEResponse -= handler,
                          "GetEVSE", "EVSEs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Connector

            RegisterEvent("GetConnectorRequest",
                          handler => CPOAPI.OnGetConnectorRequest += handler,
                          handler => CPOAPI.OnGetConnectorRequest -= handler,
                          "GetConnector", "Connectors", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetConnectorResponse",
                          handler => CPOAPI.OnGetConnectorResponse += handler,
                          handler => CPOAPI.OnGetConnectorResponse -= handler,
                          "GetConnector", "Connectors", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Tariff(s)

            RegisterEvent("GetTariffsRequest",
                          handler => CPOAPI.OnGetTariffsRequest += handler,
                          handler => CPOAPI.OnGetTariffsRequest -= handler,
                          "GetTariffs", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTariffsResponse",
                          handler => CPOAPI.OnGetTariffsResponse += handler,
                          handler => CPOAPI.OnGetTariffsResponse -= handler,
                          "GetTariffs", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Tariff

            RegisterEvent("GetTariffRequest",
                          handler => CPOAPI.OnGetTariffRequest += handler,
                          handler => CPOAPI.OnGetTariffRequest -= handler,
                          "GetTariff", "Tariffs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTariffResponse",
                          handler => CPOAPI.OnGetTariffResponse += handler,
                          handler => CPOAPI.OnGetTariffResponse -= handler,
                          "GetTariff", "Tariffs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Session(s)

            RegisterEvent("GetSessionsRequest",
                          handler => CPOAPI.OnGetSessionsRequest += handler,
                          handler => CPOAPI.OnGetSessionsRequest -= handler,
                          "GetSessions", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetSessionsResponse",
                          handler => CPOAPI.OnGetSessionsResponse += handler,
                          handler => CPOAPI.OnGetSessionsResponse -= handler,
                          "GetSessions", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Session

            RegisterEvent("GetSessionRequest",
                          handler => CPOAPI.OnGetSessionRequest += handler,
                          handler => CPOAPI.OnGetSessionRequest -= handler,
                          "GetSession", "Sessions", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetSessionResponse",
                          handler => CPOAPI.OnGetSessionResponse += handler,
                          handler => CPOAPI.OnGetSessionResponse -= handler,
                          "GetSession", "Sessions", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region CDR(s)

            RegisterEvent("GetCDRsRequest",
                          handler => CPOAPI.OnGetCDRsRequest += handler,
                          handler => CPOAPI.OnGetCDRsRequest -= handler,
                          "GetCDRs", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetCDRsResponse",
                          handler => CPOAPI.OnGetCDRsResponse += handler,
                          handler => CPOAPI.OnGetCDRsResponse -= handler,
                          "GetCDRs", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // CDR

            RegisterEvent("GetCDRRequest",
                          handler => CPOAPI.OnGetCDRRequest += handler,
                          handler => CPOAPI.OnGetCDRRequest -= handler,
                          "GetCDR", "CDRs", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetCDRResponse",
                          handler => CPOAPI.OnGetCDRResponse += handler,
                          handler => CPOAPI.OnGetCDRResponse -= handler,
                          "GetCDR", "CDRs", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Token(s)

            RegisterEvent("GetTokensRequest",
                          handler => CPOAPI.OnGetTokensRequest += handler,
                          handler => CPOAPI.OnGetTokensRequest -= handler,
                          "GetTokens", "Tokens", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTokensResponse",
                          handler => CPOAPI.OnGetTokensResponse += handler,
                          handler => CPOAPI.OnGetTokensResponse -= handler,
                          "GetTokens", "Tokens", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteTokensRequest",
                          handler => CPOAPI.OnDeleteTokensRequest += handler,
                          handler => CPOAPI.OnDeleteTokensRequest -= handler,
                          "DeleteTokens", "Tokens", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteTokensResponse",
                          handler => CPOAPI.OnDeleteTokensResponse += handler,
                          handler => CPOAPI.OnDeleteTokensResponse -= handler,
                          "DeleteTokens", "Tokens", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            // Token

            RegisterEvent("GetTokenRequest",
                          handler => CPOAPI.OnGetTokenRequest += handler,
                          handler => CPOAPI.OnGetTokenRequest -= handler,
                          "GetToken", "Tokens", "Get", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetTokenResponse",
                          handler => CPOAPI.OnGetTokenResponse += handler,
                          handler => CPOAPI.OnGetTokenResponse -= handler,
                          "GetToken", "Tokens", "Get", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutTokenRequest",
                          handler => CPOAPI.OnPutTokenRequest += handler,
                          handler => CPOAPI.OnPutTokenRequest -= handler,
                          "PutToken", "Tokens", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutTokenResponse",
                          handler => CPOAPI.OnPutTokenResponse += handler,
                          handler => CPOAPI.OnPutTokenResponse -= handler,
                          "PutToken", "Tokens", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchTokenRequest",
                          handler => CPOAPI.OnPatchTokenRequest += handler,
                          handler => CPOAPI.OnPatchTokenRequest -= handler,
                          "PatchToken", "Tokens", "Patch", "Request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchTokenResponse",
                          handler => CPOAPI.OnPatchTokenResponse += handler,
                          handler => CPOAPI.OnPatchTokenResponse -= handler,
                          "PatchToken", "Tokens", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteTokenRequest",
                          handler => CPOAPI.OnDeleteTokenRequest += handler,
                          handler => CPOAPI.OnDeleteTokenRequest -= handler,
                          "DeleteToken", "Tokens", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteTokenResponse",
                          handler => CPOAPI.OnDeleteTokenResponse += handler,
                          handler => CPOAPI.OnDeleteTokenResponse -= handler,
                          "DeleteToken", "Tokens", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

    }

}
