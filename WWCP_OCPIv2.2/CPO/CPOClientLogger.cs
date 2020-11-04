/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using org.GraphDefined.WWCP;

using cloud.charging.open.protocols.OCPIv2_2.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The CPO client.
    /// </summary>
    public partial class CPOClient : IHTTPClient
    {

        /// <summary>
        /// The CPO client (HTTP client) logger.
        /// </summary>
        public class CPOClientLogger : HTTPClientLogger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public const String DefaultContext = "OCPICPOClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached CPO client.
            /// </summary>
            public CPOClient  CPOClient    { get; }

            #endregion

            #region Constructor(s)

            #region CPOClientLogger(CPOClient, Context = DefaultContext, LogfileCreator = null)

            /// <summary>
            /// Create a new CPO client logger using the default logging delegates.
            /// </summary>
            /// <param name="CPOClient">A CPO client.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public CPOClientLogger(CPOClient               CPOClient,
                                   String                  Context         = DefaultContext,
                                   LogfileCreatorDelegate  LogfileCreator  = null)

                : this(CPOClient,
                       Context.IsNotNullOrEmpty() ? Context : DefaultContext,
                       null,
                       null,
                       null,
                       null,

                       LogfileCreator: LogfileCreator)

            { }

            #endregion

            #region CPOClientLogger(CPOClient, Context, ... Logging delegates ...)

            /// <summary>
            /// Create a new CPO client logger using the given logging delegates.
            /// </summary>
            /// <param name="CPOClient">A CPO client.</param>
            /// <param name="Context">A context of this API.</param>
            /// 
            /// <param name="LogHTTPRequest_toConsole">A delegate to log incoming HTTP requests to console.</param>
            /// <param name="LogHTTPResponse_toConsole">A delegate to log HTTP requests/responses to console.</param>
            /// <param name="LogHTTPRequest_toDisc">A delegate to log incoming HTTP requests to disc.</param>
            /// <param name="LogHTTPResponse_toDisc">A delegate to log HTTP requests/responses to disc.</param>
            /// 
            /// <param name="LogHTTPRequest_toNetwork">A delegate to log incoming HTTP requests to a network target.</param>
            /// <param name="LogHTTPResponse_toNetwork">A delegate to log HTTP requests/responses to a network target.</param>
            /// <param name="LogHTTPRequest_toHTTPSSE">A delegate to log incoming HTTP requests to a HTTP client sent events source.</param>
            /// <param name="LogHTTPResponse_toHTTPSSE">A delegate to log HTTP requests/responses to a HTTP client sent events source.</param>
            /// 
            /// <param name="LogHTTPError_toConsole">A delegate to log HTTP errors to console.</param>
            /// <param name="LogHTTPError_toDisc">A delegate to log HTTP errors to disc.</param>
            /// <param name="LogHTTPError_toNetwork">A delegate to log HTTP errors to a network target.</param>
            /// <param name="LogHTTPError_toHTTPSSE">A delegate to log HTTP errors to a HTTP client sent events source.</param>
            /// 
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public CPOClientLogger(CPOClient                   CPOClient,
                                   String                      Context,

                                   HTTPRequestLoggerDelegate   LogHTTPRequest_toConsole,
                                   HTTPResponseLoggerDelegate  LogHTTPResponse_toConsole,
                                   HTTPRequestLoggerDelegate   LogHTTPRequest_toDisc,
                                   HTTPResponseLoggerDelegate  LogHTTPResponse_toDisc,

                                   HTTPRequestLoggerDelegate   LogHTTPRequest_toNetwork    = null,
                                   HTTPResponseLoggerDelegate  LogHTTPResponse_toNetwork   = null,
                                   HTTPRequestLoggerDelegate   LogHTTPRequest_toHTTPSSE    = null,
                                   HTTPResponseLoggerDelegate  LogHTTPResponse_toHTTPSSE   = null,

                                   HTTPResponseLoggerDelegate  LogHTTPError_toConsole      = null,
                                   HTTPResponseLoggerDelegate  LogHTTPError_toDisc         = null,
                                   HTTPResponseLoggerDelegate  LogHTTPError_toNetwork      = null,
                                   HTTPResponseLoggerDelegate  LogHTTPError_toHTTPSSE      = null,

                                   LogfileCreatorDelegate      LogfileCreator              = null)

                : base(CPOClient,
                       Context.IsNotNullOrEmpty() ? Context : DefaultContext,

                       LogHTTPRequest_toConsole,
                       LogHTTPResponse_toConsole,
                       LogHTTPRequest_toDisc,
                       LogHTTPResponse_toDisc,

                       LogHTTPRequest_toNetwork,
                       LogHTTPResponse_toNetwork,
                       LogHTTPRequest_toHTTPSSE,
                       LogHTTPResponse_toHTTPSSE,

                       LogHTTPError_toConsole,
                       LogHTTPError_toDisc,
                       LogHTTPError_toNetwork,
                       LogHTTPError_toHTTPSSE,

                       LogfileCreator)

            {

                #region Initial checks

                this.CPOClient = CPOClient ?? throw new ArgumentNullException(nameof(CPOClient), "The given CPO client must not be null!");

                #endregion

                #region Register log events

                //RegisterEvent("SendHeartbeatRequest",
                //              handler => CPOClient.OnSendHeartbeatSOAPRequest  += handler,
                //              handler => CPOClient.OnSendHeartbeatSOAPRequest  -= handler,
                //              "SendHeartbeat", "Heartbeat", "Request", "All").
                //    RegisterDefaultConsoleLogTarget(this).
                //    RegisterDefaultDiscLogTarget(this);

                //RegisterEvent("SendHeartbeatResponse",
                //              handler => CPOClient.OnSendHeartbeatSOAPResponse += handler,
                //              handler => CPOClient.OnSendHeartbeatSOAPResponse -= handler,
                //              "SendHeartbeat", "Heartbeat", "Response", "All").
                //    RegisterDefaultConsoleLogTarget(this).
                //    RegisterDefaultDiscLogTarget(this);


                #endregion

            }

            #endregion

            #endregion

        }

     }

}
