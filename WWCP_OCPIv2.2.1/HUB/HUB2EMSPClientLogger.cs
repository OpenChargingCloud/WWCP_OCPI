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
    public partial class HUB2EMSPClient : IHTTPClient
    {

        /// <summary>
        /// The OCPI HUB HTTP client logger.
        /// </summary>
        public new sealed class Logger : CommonClient.Logger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public new const String  DefaultContext   = $"OCPI{Version.String}_HUBClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached HUB client.
            /// </summary>
            public HUB2EMSPClient  HUBClient    { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new HUB client logger using the default logging delegates.
            /// </summary>
            /// <param name="HUBClient">A HUB client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public Logger(HUB2EMSPClient                    HUBClient,
                          String?                      LoggingPath,
                          String?                      Context          = DefaultContext,
                          OCPILogfileCreatorDelegate?  LogfileCreator   = null)

                : base(HUBClient,
                       LoggingPath,
                       Context ?? DefaultContext,
                       LogfileCreator)

            {

                this.HUBClient = HUBClient ?? throw new ArgumentNullException(nameof(HUBClient), "The given HUB client must not be null!");

                #region Location

                RegisterEvent("GetLocationRequest",
                              handler => HUBClient.OnGetLocationHTTPRequest += handler,
                              handler => HUBClient.OnGetLocationHTTPRequest -= handler,
                              "GetLocation", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationResponse",
                              handler => HUBClient.OnGetLocationHTTPResponse += handler,
                              handler => HUBClient.OnGetLocationHTTPResponse -= handler,
                              "GetLocation", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutLocationRequest",
                              handler => HUBClient.OnPutLocationHTTPRequest += handler,
                              handler => HUBClient.OnPutLocationHTTPRequest -= handler,
                              "PutLocation", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutLocationResponse",
                              handler => HUBClient.OnPutLocationHTTPResponse += handler,
                              handler => HUBClient.OnPutLocationHTTPResponse -= handler,
                              "PutLocation", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchLocationRequest",
                              handler => HUBClient.OnPatchLocationHTTPRequest += handler,
                              handler => HUBClient.OnPatchLocationHTTPRequest -= handler,
                              "PatchLocation", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchLocationResponse",
                              handler => HUBClient.OnPatchLocationHTTPResponse += handler,
                              handler => HUBClient.OnPatchLocationHTTPResponse -= handler,
                              "PatchLocation", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region EVSE

                RegisterEvent("GetEVSERequest",
                              handler => HUBClient.OnGetEVSEHTTPRequest += handler,
                              handler => HUBClient.OnGetEVSEHTTPRequest -= handler,
                              "GetEVSE", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetEVSEResponse",
                              handler => HUBClient.OnGetEVSEHTTPResponse += handler,
                              handler => HUBClient.OnGetEVSEHTTPResponse -= handler,
                              "GetEVSE", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutEVSERequest",
                              handler => HUBClient.OnPutEVSEHTTPRequest += handler,
                              handler => HUBClient.OnPutEVSEHTTPRequest -= handler,
                              "PutEVSE", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutEVSEResponse",
                              handler => HUBClient.OnPutEVSEHTTPResponse += handler,
                              handler => HUBClient.OnPutEVSEHTTPResponse -= handler,
                              "PutEVSE", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchEVSERequest",
                              handler => HUBClient.OnPatchEVSEHTTPRequest += handler,
                              handler => HUBClient.OnPatchEVSEHTTPRequest -= handler,
                              "PatchEVSE", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchEVSEResponse",
                              handler => HUBClient.OnPatchEVSEHTTPResponse += handler,
                              handler => HUBClient.OnPatchEVSEHTTPResponse -= handler,
                              "PatchEVSE", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Connector

                RegisterEvent("GetConnectorRequest",
                              handler => HUBClient.OnGetConnectorHTTPRequest += handler,
                              handler => HUBClient.OnGetConnectorHTTPRequest -= handler,
                              "GetConnector", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetConnectorResponse",
                              handler => HUBClient.OnGetConnectorHTTPResponse += handler,
                              handler => HUBClient.OnGetConnectorHTTPResponse -= handler,
                              "GetConnector", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutConnectorRequest",
                              handler => HUBClient.OnPutConnectorHTTPRequest += handler,
                              handler => HUBClient.OnPutConnectorHTTPRequest -= handler,
                              "PutConnector", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutConnectorResponse",
                              handler => HUBClient.OnPutConnectorHTTPResponse += handler,
                              handler => HUBClient.OnPutConnectorHTTPResponse -= handler,
                              "PutConnector", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchConnectorRequest",
                              handler => HUBClient.OnPatchConnectorHTTPRequest += handler,
                              handler => HUBClient.OnPatchConnectorHTTPRequest -= handler,
                              "PatchConnector", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchConnectorResponse",
                              handler => HUBClient.OnPatchConnectorHTTPResponse += handler,
                              handler => HUBClient.OnPatchConnectorHTTPResponse -= handler,
                              "PatchConnector", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Tariff

                RegisterEvent("GetTariffRequest",
                              handler => HUBClient.OnGetTariffHTTPRequest += handler,
                              handler => HUBClient.OnGetTariffHTTPRequest -= handler,
                              "GetTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTariffResponse",
                              handler => HUBClient.OnGetTariffHTTPResponse += handler,
                              handler => HUBClient.OnGetTariffHTTPResponse -= handler,
                              "GetTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutTariffRequest",
                              handler => HUBClient.OnPutTariffHTTPRequest += handler,
                              handler => HUBClient.OnPutTariffHTTPRequest -= handler,
                              "PutTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutTariffResponse",
                              handler => HUBClient.OnPutTariffHTTPResponse += handler,
                              handler => HUBClient.OnPutTariffHTTPResponse -= handler,
                              "PutTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchTariffRequest",
                              handler => HUBClient.OnPatchTariffHTTPRequest += handler,
                              handler => HUBClient.OnPatchTariffHTTPRequest -= handler,
                              "PatchTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchTariffResponse",
                              handler => HUBClient.OnPatchTariffHTTPResponse += handler,
                              handler => HUBClient.OnPatchTariffHTTPResponse -= handler,
                              "PatchTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("DeleteTariffRequest",
                              handler => HUBClient.OnDeleteTariffHTTPRequest += handler,
                              handler => HUBClient.OnDeleteTariffHTTPRequest -= handler,
                              "DeleteTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("DeleteTariffResponse",
                              handler => HUBClient.OnDeleteTariffHTTPResponse += handler,
                              handler => HUBClient.OnDeleteTariffHTTPResponse -= handler,
                              "DeleteTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Session

                RegisterEvent("GetSessionRequest",
                              handler => HUBClient.OnGetSessionHTTPRequest += handler,
                              handler => HUBClient.OnGetSessionHTTPRequest -= handler,
                              "GetSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionResponse",
                              handler => HUBClient.OnGetSessionHTTPResponse += handler,
                              handler => HUBClient.OnGetSessionHTTPResponse -= handler,
                              "GetSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutSessionRequest",
                              handler => HUBClient.OnPutSessionHTTPRequest += handler,
                              handler => HUBClient.OnPutSessionHTTPRequest -= handler,
                              "PutSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutSessionResponse",
                              handler => HUBClient.OnPutSessionHTTPResponse += handler,
                              handler => HUBClient.OnPutSessionHTTPResponse -= handler,
                              "PutSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchSessionRequest",
                              handler => HUBClient.OnPatchSessionHTTPRequest += handler,
                              handler => HUBClient.OnPatchSessionHTTPRequest -= handler,
                              "PatchSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchSessionResponse",
                              handler => HUBClient.OnPatchSessionHTTPResponse += handler,
                              handler => HUBClient.OnPatchSessionHTTPResponse -= handler,
                              "PatchSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("DeleteSessionRequest",
                              handler => HUBClient.OnDeleteSessionHTTPRequest += handler,
                              handler => HUBClient.OnDeleteSessionHTTPRequest -= handler,
                              "DeleteSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("DeleteSessionResponse",
                              handler => HUBClient.OnDeleteSessionHTTPResponse += handler,
                              handler => HUBClient.OnDeleteSessionHTTPResponse -= handler,
                              "DeleteSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region CDR

                RegisterEvent("GetCDRRequest",
                              handler => HUBClient.OnGetCDRHTTPRequest += handler,
                              handler => HUBClient.OnGetCDRHTTPRequest -= handler,
                              "GetCDR", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCDRResponse",
                              handler => HUBClient.OnGetCDRHTTPResponse += handler,
                              handler => HUBClient.OnGetCDRHTTPResponse -= handler,
                              "GetCDR", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PostCDRRequest",
                              handler => HUBClient.OnPostCDRHTTPRequest += handler,
                              handler => HUBClient.OnPostCDRHTTPRequest -= handler,
                              "PostCDR", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PostCDRResponse",
                              handler => HUBClient.OnPostCDRHTTPResponse += handler,
                              handler => HUBClient.OnPostCDRHTTPResponse -= handler,
                              "PostCDR", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Token(s)

                RegisterEvent("GetTokensRequest",
                              handler => HUBClient.OnGetTokensHTTPRequest += handler,
                              handler => HUBClient.OnGetTokensHTTPRequest -= handler,
                              "GetTokens", "token", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTokensResponse",
                              handler => HUBClient.OnGetTokensHTTPResponse += handler,
                              handler => HUBClient.OnGetTokensHTTPResponse -= handler,
                              "GetTokens", "token", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PostTokenRequest",
                              handler => HUBClient.OnPostTokenHTTPRequest += handler,
                              handler => HUBClient.OnPostTokenHTTPRequest -= handler,
                              "PostToken", "token", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PostTokenResponse",
                              handler => HUBClient.OnPostTokenHTTPResponse += handler,
                              handler => HUBClient.OnPostTokenHTTPResponse -= handler,
                              "PostToken", "token", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

            }

            #endregion

        }

     }

}
