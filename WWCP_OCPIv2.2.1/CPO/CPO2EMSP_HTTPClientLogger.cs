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

namespace cloud.charging.open.protocols.OCPIv2_2_1.CPO.HTTP
{

    /// <summary>
    /// The CPO2EMSP client is used by CPOs to talk to EMSPs.
    /// </summary>
    public partial class CPO2EMSP_HTTPClient : IHTTPClient
    {

        /// <summary>
        /// The CPO2EMSP HTTP client logger.
        /// </summary>
        public new sealed class HTTPClientLogger : CommonHTTPClient.HTTPClientLogger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public new const String  DefaultContext   = $"OCPI{Version.String}_CPO2EMSP_HTTPClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached CPO2EMSP HTTP client.
            /// </summary>
            public CPO2EMSP_HTTPClient  Client    { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new CPO2EMSP HTTP client logger using the default logging delegates.
            /// </summary>
            /// <param name="Client">A CPO2EMSP HTTP client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public HTTPClientLogger(CPO2EMSP_HTTPClient          Client,
                                    String?                      LoggingPath,
                                    String?                      Context          = DefaultContext,
                                    OCPILogfileCreatorDelegate?  LogfileCreator   = null)

                : base(Client,
                       LoggingPath,
                       Context ?? DefaultContext,
                       LogfileCreator)

            {

                this.Client = Client ?? throw new ArgumentNullException(nameof(Client), "The given CPO2EMSP HTTP client must not be null!");

                #region Location

                RegisterEvent("GetLocationRequest",
                              handler => Client.OnGetLocationHTTPRequest += handler,
                              handler => Client.OnGetLocationHTTPRequest -= handler,
                              "GetLocation", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetLocationResponse",
                              handler => Client.OnGetLocationHTTPResponse += handler,
                              handler => Client.OnGetLocationHTTPResponse -= handler,
                              "GetLocation", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutLocationRequest",
                              handler => Client.OnPutLocationHTTPRequest += handler,
                              handler => Client.OnPutLocationHTTPRequest -= handler,
                              "PutLocation", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutLocationResponse",
                              handler => Client.OnPutLocationHTTPResponse += handler,
                              handler => Client.OnPutLocationHTTPResponse -= handler,
                              "PutLocation", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchLocationRequest",
                              handler => Client.OnPatchLocationHTTPRequest += handler,
                              handler => Client.OnPatchLocationHTTPRequest -= handler,
                              "PatchLocation", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchLocationResponse",
                              handler => Client.OnPatchLocationHTTPResponse += handler,
                              handler => Client.OnPatchLocationHTTPResponse -= handler,
                              "PatchLocation", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region EVSE

                RegisterEvent("GetEVSERequest",
                              handler => Client.OnGetEVSEHTTPRequest += handler,
                              handler => Client.OnGetEVSEHTTPRequest -= handler,
                              "GetEVSE", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetEVSEResponse",
                              handler => Client.OnGetEVSEHTTPResponse += handler,
                              handler => Client.OnGetEVSEHTTPResponse -= handler,
                              "GetEVSE", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutEVSERequest",
                              handler => Client.OnPutEVSEHTTPRequest += handler,
                              handler => Client.OnPutEVSEHTTPRequest -= handler,
                              "PutEVSE", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutEVSEResponse",
                              handler => Client.OnPutEVSEHTTPResponse += handler,
                              handler => Client.OnPutEVSEHTTPResponse -= handler,
                              "PutEVSE", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchEVSERequest",
                              handler => Client.OnPatchEVSEHTTPRequest += handler,
                              handler => Client.OnPatchEVSEHTTPRequest -= handler,
                              "PatchEVSE", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchEVSEResponse",
                              handler => Client.OnPatchEVSEHTTPResponse += handler,
                              handler => Client.OnPatchEVSEHTTPResponse -= handler,
                              "PatchEVSE", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Connector

                RegisterEvent("GetConnectorRequest",
                              handler => Client.OnGetConnectorHTTPRequest += handler,
                              handler => Client.OnGetConnectorHTTPRequest -= handler,
                              "GetConnector", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetConnectorResponse",
                              handler => Client.OnGetConnectorHTTPResponse += handler,
                              handler => Client.OnGetConnectorHTTPResponse -= handler,
                              "GetConnector", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutConnectorRequest",
                              handler => Client.OnPutConnectorHTTPRequest += handler,
                              handler => Client.OnPutConnectorHTTPRequest -= handler,
                              "PutConnector", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutConnectorResponse",
                              handler => Client.OnPutConnectorHTTPResponse += handler,
                              handler => Client.OnPutConnectorHTTPResponse -= handler,
                              "PutConnector", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchConnectorRequest",
                              handler => Client.OnPatchConnectorHTTPRequest += handler,
                              handler => Client.OnPatchConnectorHTTPRequest -= handler,
                              "PatchConnector", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchConnectorResponse",
                              handler => Client.OnPatchConnectorHTTPResponse += handler,
                              handler => Client.OnPatchConnectorHTTPResponse -= handler,
                              "PatchConnector", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Tariff

                RegisterEvent("GetTariffRequest",
                              handler => Client.OnGetTariffHTTPRequest += handler,
                              handler => Client.OnGetTariffHTTPRequest -= handler,
                              "GetTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTariffResponse",
                              handler => Client.OnGetTariffHTTPResponse += handler,
                              handler => Client.OnGetTariffHTTPResponse -= handler,
                              "GetTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutTariffRequest",
                              handler => Client.OnPutTariffHTTPRequest += handler,
                              handler => Client.OnPutTariffHTTPRequest -= handler,
                              "PutTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutTariffResponse",
                              handler => Client.OnPutTariffHTTPResponse += handler,
                              handler => Client.OnPutTariffHTTPResponse -= handler,
                              "PutTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchTariffRequest",
                              handler => Client.OnPatchTariffHTTPRequest += handler,
                              handler => Client.OnPatchTariffHTTPRequest -= handler,
                              "PatchTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchTariffResponse",
                              handler => Client.OnPatchTariffHTTPResponse += handler,
                              handler => Client.OnPatchTariffHTTPResponse -= handler,
                              "PatchTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("DeleteTariffRequest",
                              handler => Client.OnDeleteTariffHTTPRequest += handler,
                              handler => Client.OnDeleteTariffHTTPRequest -= handler,
                              "DeleteTariff", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("DeleteTariffResponse",
                              handler => Client.OnDeleteTariffHTTPResponse += handler,
                              handler => Client.OnDeleteTariffHTTPResponse -= handler,
                              "DeleteTariff", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Session

                RegisterEvent("GetSessionRequest",
                              handler => Client.OnGetSessionHTTPRequest += handler,
                              handler => Client.OnGetSessionHTTPRequest -= handler,
                              "GetSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetSessionResponse",
                              handler => Client.OnGetSessionHTTPResponse += handler,
                              handler => Client.OnGetSessionHTTPResponse -= handler,
                              "GetSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutSessionRequest",
                              handler => Client.OnPutSessionHTTPRequest += handler,
                              handler => Client.OnPutSessionHTTPRequest -= handler,
                              "PutSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutSessionResponse",
                              handler => Client.OnPutSessionHTTPResponse += handler,
                              handler => Client.OnPutSessionHTTPResponse -= handler,
                              "PutSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PatchSessionRequest",
                              handler => Client.OnPatchSessionHTTPRequest += handler,
                              handler => Client.OnPatchSessionHTTPRequest -= handler,
                              "PatchSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PatchSessionResponse",
                              handler => Client.OnPatchSessionHTTPResponse += handler,
                              handler => Client.OnPatchSessionHTTPResponse -= handler,
                              "PatchSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("DeleteSessionRequest",
                              handler => Client.OnDeleteSessionHTTPRequest += handler,
                              handler => Client.OnDeleteSessionHTTPRequest -= handler,
                              "DeleteSession", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("DeleteSessionResponse",
                              handler => Client.OnDeleteSessionHTTPResponse += handler,
                              handler => Client.OnDeleteSessionHTTPResponse -= handler,
                              "DeleteSession", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region CDR

                RegisterEvent("GetCDRRequest",
                              handler => Client.OnGetCDRHTTPRequest += handler,
                              handler => Client.OnGetCDRHTTPRequest -= handler,
                              "GetCDR", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCDRResponse",
                              handler => Client.OnGetCDRHTTPResponse += handler,
                              handler => Client.OnGetCDRHTTPResponse -= handler,
                              "GetCDR", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PostCDRRequest",
                              handler => Client.OnPostCDRHTTPRequest += handler,
                              handler => Client.OnPostCDRHTTPRequest -= handler,
                              "PostCDR", "cdr", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PostCDRResponse",
                              handler => Client.OnPostCDRHTTPResponse += handler,
                              handler => Client.OnPostCDRHTTPResponse -= handler,
                              "PostCDR", "cdr", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Token(s)

                RegisterEvent("GetTokensRequest",
                              handler => Client.OnGetTokensHTTPRequest += handler,
                              handler => Client.OnGetTokensHTTPRequest -= handler,
                              "GetTokens", "token", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetTokensResponse",
                              handler => Client.OnGetTokensHTTPResponse += handler,
                              handler => Client.OnGetTokensHTTPResponse -= handler,
                              "GetTokens", "token", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PostTokenRequest",
                              handler => Client.OnPostTokenHTTPRequest += handler,
                              handler => Client.OnPostTokenHTTPRequest -= handler,
                              "PostToken", "token", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PostTokenResponse",
                              handler => Client.OnPostTokenHTTPResponse += handler,
                              handler => Client.OnPostTokenHTTPResponse -= handler,
                              "PostToken", "token", "response", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

            }

            #endregion

        }

     }

}
