/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// The OCPI common client.
    /// </summary>
    public partial class CommonClient : OCPI.ACommonHTTPClient
    {

        /// <summary>
        /// The OCPI common client (HTTP client) logger.
        /// </summary>
        public class Logger : HTTPClientLogger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public const String  DefaultContext   = $"OCPI{Version.String}_CommonClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached common client.
            /// </summary>
            public CommonClient CommonClient { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new common client logger using the default logging delegates.
            /// </summary>
            /// <param name="CommonClient">A common client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public Logger(CommonClient                 CommonClient,
                          String?                      LoggingPath,
                          String?                      Context          = DefaultContext,
                          OCPILogfileCreatorDelegate?  LogfileCreator   = null)

                : base(CommonClient,
                       LoggingPath,
                       Context ?? DefaultContext,

                       null,
                       null,
                       null,
                       null,

                       null,
                       null,
                       null,
                       null,

                       null,
                       null,
                       null,
                       null,

                       LogfileCreator is not null
                           ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                           : null)

            {

                this.CommonClient = CommonClient ?? throw new ArgumentNullException(nameof(CommonClient), "The given common client must not be null!");

                #region Versions

                RegisterEvent("GetVersionsRequest",
                              handler => CommonClient.OnGetVersionsHTTPRequest += handler,
                              handler => CommonClient.OnGetVersionsHTTPRequest -= handler,
                              "GetVersions", "versions", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetVersionsResponse",
                              handler => CommonClient.OnGetVersionsHTTPResponse += handler,
                              handler => CommonClient.OnGetVersionsHTTPResponse -= handler,
                              "GetVersions", "versions", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("GetVersionDetailsRequest",
                              handler => CommonClient.OnGetVersionDetailsHTTPRequest += handler,
                              handler => CommonClient.OnGetVersionDetailsHTTPRequest -= handler,
                              "GetVersionDetails", "versions", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetVersionDetailsResponse",
                              handler => CommonClient.OnGetVersionDetailsHTTPRequest += handler,
                              handler => CommonClient.OnGetVersionDetailsHTTPRequest -= handler,
                              "GetVersionDetails", "versions", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

                #region Credentials

                RegisterEvent("GetCredentialsRequest",
                              handler => CommonClient.OnGetCredentialsHTTPRequest += handler,
                              handler => CommonClient.OnGetCredentialsHTTPRequest -= handler,
                              "GetCredentials", "credentials", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("GetCredentialsResponse",
                              handler => CommonClient.OnGetCredentialsHTTPResponse += handler,
                              handler => CommonClient.OnGetCredentialsHTTPResponse -= handler,
                              "GetCredentials", "credentials", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PostCredentialsRequest",
                              handler => CommonClient.OnPostCredentialsHTTPRequest += handler,
                              handler => CommonClient.OnPostCredentialsHTTPRequest -= handler,
                              "PostCredentials", "credentials", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PostCredentialsResponse",
                              handler => CommonClient.OnPostCredentialsHTTPResponse += handler,
                              handler => CommonClient.OnPostCredentialsHTTPResponse -= handler,
                              "PostCredentials", "credentials", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("PutCredentialsRequest",
                              handler => CommonClient.OnPutCredentialsHTTPRequest += handler,
                              handler => CommonClient.OnPutCredentialsHTTPRequest -= handler,
                              "PutCredentials", "credentials", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("PutCredentialsResponse",
                              handler => CommonClient.OnPutCredentialsHTTPResponse += handler,
                              handler => CommonClient.OnPutCredentialsHTTPResponse -= handler,
                              "PutCredentials", "credentials", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("DeleteCredentialsRequest",
                              handler => CommonClient.OnDeleteCredentialsHTTPRequest += handler,
                              handler => CommonClient.OnDeleteCredentialsHTTPRequest -= handler,
                              "DeleteCredentials", "credentials", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("DeleteCredentialsResponse",
                              handler => CommonClient.OnDeleteCredentialsHTTPResponse += handler,
                              handler => CommonClient.OnDeleteCredentialsHTTPResponse -= handler,
                              "DeleteCredentials", "credentials", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);


                RegisterEvent("RegisterRequest",
                              handler => CommonClient.OnRegisterHTTPRequest += handler,
                              handler => CommonClient.OnRegisterHTTPRequest -= handler,
                              "Register", "credentials", "requests", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                RegisterEvent("RegisterResponse",
                              handler => CommonClient.OnRegisterHTTPResponse += handler,
                              handler => CommonClient.OnRegisterHTTPResponse -= handler,
                              "Register", "credentials", "responses", "all").
                    RegisterDefaultConsoleLogTarget(this).
                    RegisterDefaultDiscLogTarget(this);

                #endregion

            }

            #endregion

        }

     }

}
