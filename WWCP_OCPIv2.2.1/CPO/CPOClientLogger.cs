/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPIv2_2_1.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.CPO.HTTP
{

    /// <summary>
    /// The OCPI CPO client.
    /// </summary>
    public partial class CPOClient : IHTTPClient
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
            public CPOClient  CPOClient    { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new CPO client logger using the default logging delegates.
            /// </summary>
            /// <param name="CPOClient">A CPO client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public Logger(CPOClient                CPOClient,
                          String?                  LoggingPath,
                          String?                  Context          = DefaultContext,
                          LogfileCreatorDelegate?  LogfileCreator   = null)

                : base(CPOClient,
                       LoggingPath,
                       Context ?? DefaultContext,
                       LogfileCreator)

            {

                this.CPOClient = CPOClient ?? throw new ArgumentNullException(nameof(CPOClient), "The given CPO client must not be null!");

                #region Location

                RegisterEvent("GetLocationRequest",
                              handler => CPOClient.OnGetLocationHTTPRequest += handler,
                              handler => CPOClient.OnGetLocationHTTPRequest -= handler,
                              "GetLocation", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationResponse",
                              handler => CPOClient.OnGetLocationHTTPResponse += handler,
                              handler => CPOClient.OnGetLocationHTTPResponse -= handler,
                              "GetLocation", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutLocationRequest",
                              handler => CPOClient.OnPutLocationHTTPRequest += handler,
                              handler => CPOClient.OnPutLocationHTTPRequest -= handler,
                              "PutLocation", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutLocationResponse",
                              handler => CPOClient.OnPutLocationHTTPResponse += handler,
                              handler => CPOClient.OnPutLocationHTTPResponse -= handler,
                              "PutLocation", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchLocationRequest",
                              handler => CPOClient.OnPatchLocationHTTPRequest += handler,
                              handler => CPOClient.OnPatchLocationHTTPRequest -= handler,
                              "PatchLocation", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchLocationResponse",
                              handler => CPOClient.OnPatchLocationHTTPResponse += handler,
                              handler => CPOClient.OnPatchLocationHTTPResponse -= handler,
                              "PatchLocation", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region EVSE

                RegisterEvent("GetEVSERequest",
                              handler => CPOClient.OnGetEVSEHTTPRequest += handler,
                              handler => CPOClient.OnGetEVSEHTTPRequest -= handler,
                              "GetEVSE", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetEVSEResponse",
                              handler => CPOClient.OnGetEVSEHTTPResponse += handler,
                              handler => CPOClient.OnGetEVSEHTTPResponse -= handler,
                              "GetEVSE", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutEVSERequest",
                              handler => CPOClient.OnPutEVSEHTTPRequest += handler,
                              handler => CPOClient.OnPutEVSEHTTPRequest -= handler,
                              "PutEVSE", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutEVSEResponse",
                              handler => CPOClient.OnPutEVSEHTTPResponse += handler,
                              handler => CPOClient.OnPutEVSEHTTPResponse -= handler,
                              "PutEVSE", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchEVSERequest",
                              handler => CPOClient.OnPatchEVSEHTTPRequest += handler,
                              handler => CPOClient.OnPatchEVSEHTTPRequest -= handler,
                              "PatchEVSE", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchEVSEResponse",
                              handler => CPOClient.OnPatchEVSEHTTPResponse += handler,
                              handler => CPOClient.OnPatchEVSEHTTPResponse -= handler,
                              "PatchEVSE", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Connector

                RegisterEvent("GetConnectorRequest",
                              handler => CPOClient.OnGetConnectorHTTPRequest += handler,
                              handler => CPOClient.OnGetConnectorHTTPRequest -= handler,
                              "GetConnector", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetConnectorResponse",
                              handler => CPOClient.OnGetConnectorHTTPResponse += handler,
                              handler => CPOClient.OnGetConnectorHTTPResponse -= handler,
                              "GetConnector", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutConnectorRequest",
                              handler => CPOClient.OnPutConnectorHTTPRequest += handler,
                              handler => CPOClient.OnPutConnectorHTTPRequest -= handler,
                              "PutConnector", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutConnectorResponse",
                              handler => CPOClient.OnPutConnectorHTTPResponse += handler,
                              handler => CPOClient.OnPutConnectorHTTPResponse -= handler,
                              "PutConnector", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchConnectorRequest",
                              handler => CPOClient.OnPatchConnectorHTTPRequest += handler,
                              handler => CPOClient.OnPatchConnectorHTTPRequest -= handler,
                              "PatchConnector", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchConnectorResponse",
                              handler => CPOClient.OnPatchConnectorHTTPResponse += handler,
                              handler => CPOClient.OnPatchConnectorHTTPResponse -= handler,
                              "PatchConnector", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Tariff

                RegisterEvent("GetTariffRequest",
                              handler => CPOClient.OnGetTariffHTTPRequest += handler,
                              handler => CPOClient.OnGetTariffHTTPRequest -= handler,
                              "GetTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTariffResponse",
                              handler => CPOClient.OnGetTariffHTTPResponse += handler,
                              handler => CPOClient.OnGetTariffHTTPResponse -= handler,
                              "GetTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutTariffRequest",
                              handler => CPOClient.OnPutTariffHTTPRequest += handler,
                              handler => CPOClient.OnPutTariffHTTPRequest -= handler,
                              "PutTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutTariffResponse",
                              handler => CPOClient.OnPutTariffHTTPResponse += handler,
                              handler => CPOClient.OnPutTariffHTTPResponse -= handler,
                              "PutTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchTariffRequest",
                              handler => CPOClient.OnPatchTariffHTTPRequest += handler,
                              handler => CPOClient.OnPatchTariffHTTPRequest -= handler,
                              "PatchTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchTariffResponse",
                              handler => CPOClient.OnPatchTariffHTTPResponse += handler,
                              handler => CPOClient.OnPatchTariffHTTPResponse -= handler,
                              "PatchTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("DeleteTariffRequest",
                              handler => CPOClient.OnDeleteTariffHTTPRequest += handler,
                              handler => CPOClient.OnDeleteTariffHTTPRequest -= handler,
                              "DeleteTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("DeleteTariffResponse",
                              handler => CPOClient.OnDeleteTariffHTTPResponse += handler,
                              handler => CPOClient.OnDeleteTariffHTTPResponse -= handler,
                              "DeleteTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Session

                RegisterEvent("GetSessionRequest",
                              handler => CPOClient.OnGetSessionHTTPRequest += handler,
                              handler => CPOClient.OnGetSessionHTTPRequest -= handler,
                              "GetSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionResponse",
                              handler => CPOClient.OnGetSessionHTTPResponse += handler,
                              handler => CPOClient.OnGetSessionHTTPResponse -= handler,
                              "GetSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutSessionRequest",
                              handler => CPOClient.OnPutSessionHTTPRequest += handler,
                              handler => CPOClient.OnPutSessionHTTPRequest -= handler,
                              "PutSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutSessionResponse",
                              handler => CPOClient.OnPutSessionHTTPResponse += handler,
                              handler => CPOClient.OnPutSessionHTTPResponse -= handler,
                              "PutSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchSessionRequest",
                              handler => CPOClient.OnPatchSessionHTTPRequest += handler,
                              handler => CPOClient.OnPatchSessionHTTPRequest -= handler,
                              "PatchSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchSessionResponse",
                              handler => CPOClient.OnPatchSessionHTTPResponse += handler,
                              handler => CPOClient.OnPatchSessionHTTPResponse -= handler,
                              "PatchSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("DeleteSessionRequest",
                              handler => CPOClient.OnDeleteSessionHTTPRequest += handler,
                              handler => CPOClient.OnDeleteSessionHTTPRequest -= handler,
                              "DeleteSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("DeleteSessionResponse",
                              handler => CPOClient.OnDeleteSessionHTTPResponse += handler,
                              handler => CPOClient.OnDeleteSessionHTTPResponse -= handler,
                              "DeleteSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region CDR

                RegisterEvent("GetCDRRequest",
                              handler => CPOClient.OnGetCDRHTTPRequest += handler,
                              handler => CPOClient.OnGetCDRHTTPRequest -= handler,
                              "GetCDR", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCDRResponse",
                              handler => CPOClient.OnGetCDRHTTPResponse += handler,
                              handler => CPOClient.OnGetCDRHTTPResponse -= handler,
                              "GetCDR", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PostCDRRequest",
                              handler => CPOClient.OnPostCDRHTTPRequest += handler,
                              handler => CPOClient.OnPostCDRHTTPRequest -= handler,
                              "PostCDR", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PostCDRResponse",
                              handler => CPOClient.OnPostCDRHTTPResponse += handler,
                              handler => CPOClient.OnPostCDRHTTPResponse -= handler,
                              "PostCDR", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Token(s)

                RegisterEvent("GetTokensRequest",
                              handler => CPOClient.OnGetTokensHTTPRequest += handler,
                              handler => CPOClient.OnGetTokensHTTPRequest -= handler,
                              "GetTokens", "token", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTokensResponse",
                              handler => CPOClient.OnGetTokensHTTPResponse += handler,
                              handler => CPOClient.OnGetTokensHTTPResponse -= handler,
                              "GetTokens", "token", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PostTokenRequest",
                              handler => CPOClient.OnPostTokenHTTPRequest += handler,
                              handler => CPOClient.OnPostTokenHTTPRequest -= handler,
                              "PostToken", "token", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PostTokenResponse",
                              handler => CPOClient.OnPostTokenHTTPResponse += handler,
                              handler => CPOClient.OnPostTokenHTTPResponse -= handler,
                              "PostToken", "token", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

            }

            #endregion

        }

     }

}
