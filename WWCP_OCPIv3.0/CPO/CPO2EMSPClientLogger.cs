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
using cloud.charging.open.protocols.OCPIv3_0;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.CPO.HTTP
{

    /// <summary>
    /// The OCPI CPO client.
    /// </summary>
    public partial class CPO2EMSPClient : IHTTPClient
    {

        /// <summary>
        /// The OCPI CPO HTTP client logger.
        /// </summary>
        public new sealed class Logger : CommonClient.Logger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public new const String  DefaultContext   = $"OCPI{Version.String}_CPOClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached CPO client.
            /// </summary>
            public CPO2EMSPClient  CPO2EMSPClient    { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new CPO client logger using the default logging delegates.
            /// </summary>
            /// <param name="CPO2EMSPClient">A CPO client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public Logger(CPO2EMSPClient               CPO2EMSPClient,
                          String?                      LoggingPath,
                          String?                      Context          = DefaultContext,
                          OCPILogfileCreatorDelegate?  LogfileCreator   = null)

                : base(CPO2EMSPClient,
                       LoggingPath,
                       Context ?? DefaultContext,
                       LogfileCreator)

            {

                this.CPO2EMSPClient = CPO2EMSPClient ?? throw new ArgumentNullException(nameof(CPO2EMSPClient), "The given CPO client must not be null!");

                #region Location

                RegisterEvent("GetLocationRequest",
                              handler => CPO2EMSPClient.OnGetLocationHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnGetLocationHTTPRequest -= handler,
                              "GetLocation", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationResponse",
                              handler => CPO2EMSPClient.OnGetLocationHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnGetLocationHTTPResponse -= handler,
                              "GetLocation", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutLocationRequest",
                              handler => CPO2EMSPClient.OnPutLocationHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnPutLocationHTTPRequest -= handler,
                              "PutLocation", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutLocationResponse",
                              handler => CPO2EMSPClient.OnPutLocationHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnPutLocationHTTPResponse -= handler,
                              "PutLocation", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchLocationRequest",
                              handler => CPO2EMSPClient.OnPatchLocationHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnPatchLocationHTTPRequest -= handler,
                              "PatchLocation", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchLocationResponse",
                              handler => CPO2EMSPClient.OnPatchLocationHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnPatchLocationHTTPResponse -= handler,
                              "PatchLocation", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region EVSE

                RegisterEvent("GetEVSERequest",
                              handler => CPO2EMSPClient.OnGetEVSEHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnGetEVSEHTTPRequest -= handler,
                              "GetEVSE", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetEVSEResponse",
                              handler => CPO2EMSPClient.OnGetEVSEHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnGetEVSEHTTPResponse -= handler,
                              "GetEVSE", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutEVSERequest",
                              handler => CPO2EMSPClient.OnPutEVSEHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnPutEVSEHTTPRequest -= handler,
                              "PutEVSE", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutEVSEResponse",
                              handler => CPO2EMSPClient.OnPutEVSEHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnPutEVSEHTTPResponse -= handler,
                              "PutEVSE", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchEVSERequest",
                              handler => CPO2EMSPClient.OnPatchEVSEHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnPatchEVSEHTTPRequest -= handler,
                              "PatchEVSE", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchEVSEResponse",
                              handler => CPO2EMSPClient.OnPatchEVSEHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnPatchEVSEHTTPResponse -= handler,
                              "PatchEVSE", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Connector

                RegisterEvent("GetConnectorRequest",
                              handler => CPO2EMSPClient.OnGetConnectorHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnGetConnectorHTTPRequest -= handler,
                              "GetConnector", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetConnectorResponse",
                              handler => CPO2EMSPClient.OnGetConnectorHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnGetConnectorHTTPResponse -= handler,
                              "GetConnector", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutConnectorRequest",
                              handler => CPO2EMSPClient.OnPutConnectorHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnPutConnectorHTTPRequest -= handler,
                              "PutConnector", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutConnectorResponse",
                              handler => CPO2EMSPClient.OnPutConnectorHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnPutConnectorHTTPResponse -= handler,
                              "PutConnector", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchConnectorRequest",
                              handler => CPO2EMSPClient.OnPatchConnectorHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnPatchConnectorHTTPRequest -= handler,
                              "PatchConnector", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchConnectorResponse",
                              handler => CPO2EMSPClient.OnPatchConnectorHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnPatchConnectorHTTPResponse -= handler,
                              "PatchConnector", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Tariff

                RegisterEvent("GetTariffRequest",
                              handler => CPO2EMSPClient.OnGetTariffHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnGetTariffHTTPRequest -= handler,
                              "GetTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTariffResponse",
                              handler => CPO2EMSPClient.OnGetTariffHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnGetTariffHTTPResponse -= handler,
                              "GetTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutTariffRequest",
                              handler => CPO2EMSPClient.OnPutTariffHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnPutTariffHTTPRequest -= handler,
                              "PutTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutTariffResponse",
                              handler => CPO2EMSPClient.OnPutTariffHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnPutTariffHTTPResponse -= handler,
                              "PutTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchTariffRequest",
                              handler => CPO2EMSPClient.OnPatchTariffHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnPatchTariffHTTPRequest -= handler,
                              "PatchTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchTariffResponse",
                              handler => CPO2EMSPClient.OnPatchTariffHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnPatchTariffHTTPResponse -= handler,
                              "PatchTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("DeleteTariffRequest",
                              handler => CPO2EMSPClient.OnDeleteTariffHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnDeleteTariffHTTPRequest -= handler,
                              "DeleteTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("DeleteTariffResponse",
                              handler => CPO2EMSPClient.OnDeleteTariffHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnDeleteTariffHTTPResponse -= handler,
                              "DeleteTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Session

                RegisterEvent("GetSessionRequest",
                              handler => CPO2EMSPClient.OnGetSessionHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnGetSessionHTTPRequest -= handler,
                              "GetSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionResponse",
                              handler => CPO2EMSPClient.OnGetSessionHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnGetSessionHTTPResponse -= handler,
                              "GetSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutSessionRequest",
                              handler => CPO2EMSPClient.OnPutSessionHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnPutSessionHTTPRequest -= handler,
                              "PutSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutSessionResponse",
                              handler => CPO2EMSPClient.OnPutSessionHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnPutSessionHTTPResponse -= handler,
                              "PutSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchSessionRequest",
                              handler => CPO2EMSPClient.OnPatchSessionHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnPatchSessionHTTPRequest -= handler,
                              "PatchSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchSessionResponse",
                              handler => CPO2EMSPClient.OnPatchSessionHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnPatchSessionHTTPResponse -= handler,
                              "PatchSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("DeleteSessionRequest",
                              handler => CPO2EMSPClient.OnDeleteSessionHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnDeleteSessionHTTPRequest -= handler,
                              "DeleteSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("DeleteSessionResponse",
                              handler => CPO2EMSPClient.OnDeleteSessionHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnDeleteSessionHTTPResponse -= handler,
                              "DeleteSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region CDR

                RegisterEvent("GetCDRRequest",
                              handler => CPO2EMSPClient.OnGetCDRHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnGetCDRHTTPRequest -= handler,
                              "GetCDR", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCDRResponse",
                              handler => CPO2EMSPClient.OnGetCDRHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnGetCDRHTTPResponse -= handler,
                              "GetCDR", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PostCDRRequest",
                              handler => CPO2EMSPClient.OnPostCDRHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnPostCDRHTTPRequest -= handler,
                              "PostCDR", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PostCDRResponse",
                              handler => CPO2EMSPClient.OnPostCDRHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnPostCDRHTTPResponse -= handler,
                              "PostCDR", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Token(s)

                RegisterEvent("GetTokensRequest",
                              handler => CPO2EMSPClient.OnGetTokensHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnGetTokensHTTPRequest -= handler,
                              "GetTokens", "token", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTokensResponse",
                              handler => CPO2EMSPClient.OnGetTokensHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnGetTokensHTTPResponse -= handler,
                              "GetTokens", "token", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PostTokenRequest",
                              handler => CPO2EMSPClient.OnPostTokenHTTPRequest += handler,
                              handler => CPO2EMSPClient.OnPostTokenHTTPRequest -= handler,
                              "PostToken", "token", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PostTokenResponse",
                              handler => CPO2EMSPClient.OnPostTokenHTTPResponse += handler,
                              handler => CPO2EMSPClient.OnPostTokenHTTPResponse -= handler,
                              "PostToken", "token", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

            }

            #endregion

        }

     }

}
