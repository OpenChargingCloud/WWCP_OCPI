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

namespace cloud.charging.open.protocols.OCPIv2_2_1.HTTP
{

    /// <summary>
    /// A SCSP API logger.
    /// </summary>
    public sealed class SCSPAPILogger : CommonAPILogger
    {

        #region Data

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public new const String  DefaultContext   = $"OCPI{Version.String}_SCSPAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked SCSP API.
        /// </summary>
        public SCSPAPI  SCSPAPI  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SCSP API logger using the default logging delegates.
        /// </summary>
        /// <param name="SCSPAPI">An SCSP API.</param>
        /// <param name="Context">A context of this API.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public SCSPAPILogger(SCSPAPI                      SCSPAPI,
                             String?                      Context          = DefaultContext,
                             String?                      LoggingPath      = null,
                             OCPILogfileCreatorDelegate?  LogfileCreator   = null)

            : base(SCSPAPI.CommonAPI,
                   Context ?? DefaultContext,
                   LoggingPath,
                   LogfileCreator)

        {

            this.SCSPAPI = SCSPAPI ?? throw new ArgumentNullException(nameof(SCSPAPI), "The given SCSP API must not be null!");

            #region Locations

            RegisterEvent("DeleteLocationsRequest",
                          handler => SCSPAPI.OnDeleteLocationsRequest += handler,
                          handler => SCSPAPI.OnDeleteLocationsRequest -= handler,
                          "DeleteLocations", "Locations", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteLocationsResponse",
                          handler => SCSPAPI.OnDeleteLocationsResponse += handler,
                          handler => SCSPAPI.OnDeleteLocationsResponse -= handler,
                          "DeleteLocations", "Locations", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutLocationRequest",
                          handler => SCSPAPI.OnPutLocationRequest += handler,
                          handler => SCSPAPI.OnPutLocationRequest -= handler,
                          "PutLocation", "Locations", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutLocationResponse",
                          handler => SCSPAPI.OnPutLocationResponse += handler,
                          handler => SCSPAPI.OnPutLocationResponse -= handler,
                          "PutLocation", "Locations", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchLocationRequest",
                          handler => SCSPAPI.OnPatchLocationRequest += handler,
                          handler => SCSPAPI.OnPatchLocationRequest -= handler,
                          "PatchLocation", "Locations", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchLocationResponse",
                          handler => SCSPAPI.OnPatchLocationResponse += handler,
                          handler => SCSPAPI.OnPatchLocationResponse -= handler,
                          "PatchLocation", "Locations", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteLocationRequest",
                          handler => SCSPAPI.OnDeleteLocationRequest += handler,
                          handler => SCSPAPI.OnDeleteLocationRequest -= handler,
                          "DeleteLocation", "Locations", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteLocationResponse",
                          handler => SCSPAPI.OnDeleteLocationResponse += handler,
                          handler => SCSPAPI.OnDeleteLocationResponse -= handler,
                          "DeleteLocation", "Locations", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region EVSEs

            RegisterEvent("PutEVSERequest",
                          handler => SCSPAPI.OnPutEVSERequest += handler,
                          handler => SCSPAPI.OnPutEVSERequest -= handler,
                          "PutEVSE", "EVSEs", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutEVSEResponse",
                          handler => SCSPAPI.OnPutEVSEResponse += handler,
                          handler => SCSPAPI.OnPutEVSEResponse -= handler,
                          "PutEVSE", "EVSEs", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchEVSERequest",
                          handler => SCSPAPI.OnPatchEVSERequest += handler,
                          handler => SCSPAPI.OnPatchEVSERequest -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchEVSEResponse",
                          handler => SCSPAPI.OnPatchEVSEResponse += handler,
                          handler => SCSPAPI.OnPatchEVSEResponse -= handler,
                          "PatchEVSE", "EVSEs", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteEVSERequest",
                          handler => SCSPAPI.OnDeleteEVSERequest += handler,
                          handler => SCSPAPI.OnDeleteEVSERequest -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteEVSEResponse",
                          handler => SCSPAPI.OnDeleteEVSEResponse += handler,
                          handler => SCSPAPI.OnDeleteEVSEResponse -= handler,
                          "DeleteEVSE", "EVSEs", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Connectors

            RegisterEvent("PutConnectorRequest",
                          handler => SCSPAPI.OnPutConnectorRequest += handler,
                          handler => SCSPAPI.OnPutConnectorRequest -= handler,
                          "PutConnector", "Connectors", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutConnectorResponse",
                          handler => SCSPAPI.OnPutConnectorResponse += handler,
                          handler => SCSPAPI.OnPutConnectorResponse -= handler,
                          "PutConnector", "Connectors", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchConnectorRequest",
                          handler => SCSPAPI.OnPatchConnectorRequest += handler,
                          handler => SCSPAPI.OnPatchConnectorRequest -= handler,
                          "PatchConnector", "Connectors", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchConnectorResponse",
                          handler => SCSPAPI.OnPatchConnectorResponse += handler,
                          handler => SCSPAPI.OnPatchConnectorResponse -= handler,
                          "PatchConnector", "Connectors", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteConnectorRequest",
                          handler => SCSPAPI.OnDeleteConnectorRequest += handler,
                          handler => SCSPAPI.OnDeleteConnectorRequest -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteConnectorResponse",
                          handler => SCSPAPI.OnDeleteConnectorResponse += handler,
                          handler => SCSPAPI.OnDeleteConnectorResponse -= handler,
                          "DeleteConnector", "Connectors", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Sessions

            RegisterEvent("DeleteSessionsRequest",
                          handler => SCSPAPI.OnDeleteSessionsRequest += handler,
                          handler => SCSPAPI.OnDeleteSessionsRequest -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteSessionsResponse",
                          handler => SCSPAPI.OnDeleteSessionsResponse += handler,
                          handler => SCSPAPI.OnDeleteSessionsResponse -= handler,
                          "DeleteSessions", "Sessions", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PutSessionRequest",
                          handler => SCSPAPI.OnPutSessionRequest += handler,
                          handler => SCSPAPI.OnPutSessionRequest -= handler,
                          "PutSession", "Sessions", "Put", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PutSessionResponse",
                          handler => SCSPAPI.OnPutSessionResponse += handler,
                          handler => SCSPAPI.OnPutSessionResponse -= handler,
                          "PutSession", "Sessions", "Put", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("PatchSessionRequest",
                          handler => SCSPAPI.OnPatchSessionRequest += handler,
                          handler => SCSPAPI.OnPatchSessionRequest -= handler,
                          "PatchSession", "Sessions", "Patch", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("PatchSessionResponse",
                          handler => SCSPAPI.OnPatchSessionResponse += handler,
                          handler => SCSPAPI.OnPatchSessionResponse -= handler,
                          "PatchSession", "Sessions", "Patch", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent("DeleteSessionRequest",
                          handler => SCSPAPI.OnDeleteSessionRequest += handler,
                          handler => SCSPAPI.OnDeleteSessionRequest -= handler,
                          "DeleteSession", "Sessions", "Delete", "Request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent("DeleteSessionResponse",
                          handler => SCSPAPI.OnDeleteSessionResponse += handler,
                          handler => SCSPAPI.OnDeleteSessionResponse -= handler,
                          "DeleteSession", "Sessions", "Delete", "Response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

    }

}
