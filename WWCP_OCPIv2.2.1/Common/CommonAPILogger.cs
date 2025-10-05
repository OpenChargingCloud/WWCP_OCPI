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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// A CommonAPI logger.
    /// </summary>
    public class CommonAPILogger : OCPIAPILogger
    {

        #region Data

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public new const String  DefaultContext   = $"OCPI{Version.String}_CommonAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked CommonAPI.
        /// </summary>
        public CommonAPI  CommonAPI    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CommonAPI logger using the default logging delegates.
        /// </summary>
        /// <param name="CommonAPI">An CommonAPI.</param>
        /// <param name="Context">A context of this API.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public CommonAPILogger(CommonAPI                    CommonAPI,
                               String?                      Context          = DefaultContext,
                               String?                      LoggingPath      = null,
                               OCPILogfileCreatorDelegate?  LogfileCreator   = null)

            : base(CommonAPI.HTTPBaseAPI.HTTPServer,
                   Context ?? DefaultContext,
                   LoggingPath,
                   LogfileCreator)

        {

            this.CommonAPI = CommonAPI ?? throw new ArgumentNullException(nameof(CommonAPI), "The given CommonAPI must not be null!");

            #region Version(s)

            RegisterEvent("GetVersionsRequest",
                          handler => CommonAPI.OnGetVersionsRequest += handler,
                          handler => CommonAPI.OnGetVersionsRequest -= handler,
                          "GetVersions", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetVersionsResponse",
                          handler => CommonAPI.OnGetVersionsResponse += handler,
                          handler => CommonAPI.OnGetVersionsResponse -= handler,
                          "GetVersions", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("GetVersionRequest",
                          handler => CommonAPI.OnGetVersionRequest += handler,
                          handler => CommonAPI.OnGetVersionRequest -= handler,
                          "GetVersion", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetVersionResponse",
                          handler => CommonAPI.OnGetVersionResponse += handler,
                          handler => CommonAPI.OnGetVersionResponse -= handler,
                          "GetVersion", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Credentials

            RegisterEvent("GetCredentialsRequest",
                          handler => CommonAPI.OnGetCredentialsRequest += handler,
                          handler => CommonAPI.OnGetCredentialsRequest -= handler,
                          "GetCredentials", "Credentials", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("GetCredentialsResponse",
                          handler => CommonAPI.OnGetCredentialsResponse += handler,
                          handler => CommonAPI.OnGetCredentialsResponse -= handler,
                          "GetCredentials", "Credentials", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PostCredentialsRequest",
                          handler => CommonAPI.OnPostCredentialsRequest += handler,
                          handler => CommonAPI.OnPostCredentialsRequest -= handler,
                          "PostCredentials", "Credentials", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PostCredentialsResponse",
                          handler => CommonAPI.OnPostCredentialsResponse += handler,
                          handler => CommonAPI.OnPostCredentialsResponse -= handler,
                          "PostCredentials", "Credentials", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutCredentialsRequest",
                          handler => CommonAPI.OnPutCredentialsRequest += handler,
                          handler => CommonAPI.OnPutCredentialsRequest -= handler,
                          "PutCredentials", "Credentials", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutCredentialsResponse",
                          handler => CommonAPI.OnPutCredentialsResponse += handler,
                          handler => CommonAPI.OnPutCredentialsResponse -= handler,
                          "PutCredentials", "Credentials", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteCredentialsRequest",
                          handler => CommonAPI.OnDeleteCredentialsRequest += handler,
                          handler => CommonAPI.OnDeleteCredentialsRequest -= handler,
                          "DeleteCredentials", "Credentials", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteCredentialsResponse",
                          handler => CommonAPI.OnDeleteCredentialsResponse += handler,
                          handler => CommonAPI.OnDeleteCredentialsResponse -= handler,
                          "DeleteCredentials", "Credentials", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

    }

}
