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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.EMSP.HTTP
{

    /// <summary>
    /// The OCPI EMSP-2-CPO HTTP client.
    /// </summary>
    public partial class EMSP2CPO_HTTPClient : IHTTPClient
    {


        public void LinkClientEventsToHTTPSSE(HTTPEventSource<JObject> HTTPSSE)
        {
            ClientEventsToJSON(
                HTTPSSE.SubmitEvent
            );
        }

        public void ClientEventsToJSON(Func<String, JObject, CancellationToken, Task> Processor)
        {

            #region OnStartSessionRequest/-Response

            OnStartSessionRequest += (timestamp,
                                      sender,
                                      eventTrackingId,
                                      remotePartyId,
                                      from,
                                      to,
                                      requestId,
                                      correlationId,
                                      requestTimeout,

                                      token,
                                      locationId,
                                      evseUId,
                                      connectorId,
                                      authorizationReference,

                                      cancellationToken) =>

                Processor(
                    "OnStartSessionRequest",
                    JSONObject.Create(

                              new JProperty("timestamp",                timestamp.                   ToISO8601()),
                              new JProperty("eventTrackingId",          eventTrackingId.             ToString()),
                              new JProperty("remotePartyId",            remotePartyId.               ToString()),

                        from.                  HasValue
                            ? new JProperty("from",                     from.                  Value.ToString())
                            : null,

                        to.                    HasValue
                            ? new JProperty("to",                       to.                    Value.ToString())
                            : null,

                              new JProperty("requestId",                requestId.                   ToString()),
                              new JProperty("correlationId",            correlationId.               ToString()),
                              new JProperty("requestTimeout",           requestTimeout.              TotalSeconds),

                              new JProperty("token",                    token.                       ToJSON()),
                              new JProperty("locationId",               locationId.                  ToString()),

                        evseUId.               HasValue
                            ? new JProperty("evseUId",                  evseUId.               Value.ToString())
                            : null,

                        connectorId.           HasValue
                            ? new JProperty("connectorId",              connectorId.           Value.ToString())
                            : null,

                        authorizationReference.HasValue
                            ? new JProperty("authorizationReference",   authorizationReference.Value.ToString())
                            : null

                    ),
                    cancellationToken
                );


            OnStartSessionResponse += (timestamp,
                                       sender,
                                       eventTrackingId,
                                       remotePartyId,
                                       from,
                                       to,
                                       requestId,
                                       correlationId,
                                       requestTimeout,

                                       token,
                                       locationId,
                                       evseUId,
                                       connectorId,
                                       authorizationReference,

                                       response,
                                       runtime,
                                       cancellationToken) =>

                Processor(
                    "OnStartSessionResponse",
                    JSONObject.Create(

                              new JProperty("timestamp",                timestamp.                   ToISO8601()),
                              new JProperty("eventTrackingId",          eventTrackingId.             ToString()),
                              new JProperty("remotePartyId",            remotePartyId.               ToString()),

                        from.                  HasValue
                            ? new JProperty("from",                     from.                  Value.ToString())
                            : null,

                        to.                    HasValue
                            ? new JProperty("to",                       to.                    Value.ToString())
                            : null,

                              new JProperty("requestId",                requestId.                   ToString()),
                              new JProperty("correlationId",            correlationId.               ToString()),
                              new JProperty("requestTimeout",           requestTimeout.              TotalSeconds),

                              new JProperty("token",                    token.                       ToJSON()),
                              new JProperty("locationId",               locationId.                  ToString()),

                        evseUId.               HasValue
                            ? new JProperty("evseUId",                  evseUId.               Value.ToString())
                            : null,

                        connectorId.           HasValue
                            ? new JProperty("connectorId",              connectorId.           Value.ToString())
                            : null,

                        authorizationReference.HasValue
                            ? new JProperty("authorizationReference",   authorizationReference.Value.ToString())
                            : null,

                              new JProperty("response",                 response.                    ToJSON(commandResponse => commandResponse.ToJSON())),

                              new JProperty("runtime",                  runtime.                     TotalMilliseconds)

                    ),
                    cancellationToken
                );

            #endregion

            #region OnStopSessionRequest/-Response

            OnStopSessionRequest += (timestamp,
                                     sender,
                                     eventTrackingId,
                                     remotePartyId,
                                     from,
                                     to,
                                     requestId,
                                     correlationId,
                                     requestTimeout,

                                     sessionId,

                                     cancellationToken) =>

                Processor(
                    "OnStopSessionRequest",
                    JSONObject.Create(

                              new JProperty("timestamp",        timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",  eventTrackingId.ToString()),
                              new JProperty("remotePartyId",    remotePartyId.  ToString()),

                        from.                  HasValue
                            ? new JProperty("from",             from.     Value.ToString())
                            : null,

                        to.                    HasValue
                            ? new JProperty("to",               to.       Value.ToString())
                            : null,

                              new JProperty("requestId",        requestId.      ToString()),
                              new JProperty("correlationId",    correlationId.  ToString()),
                              new JProperty("requestTimeout",   requestTimeout. TotalSeconds),

                              new JProperty("sessionId",        sessionId.      ToString())

                    ),
                    cancellationToken
                );


            OnStopSessionResponse += (timestamp,
                                      sender,
                                      eventTrackingId,
                                      remotePartyId,
                                      from,
                                      to,
                                      requestId,
                                      correlationId,
                                      requestTimeout,

                                      sessionId,

                                      response,
                                      runtime,
                                      cancellationToken) =>

                Processor(
                    "OnStopSessionResponse",
                    JSONObject.Create(

                              new JProperty("timestamp",        timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",  eventTrackingId.ToString()),
                              new JProperty("remotePartyId",    remotePartyId.  ToString()),

                        from.                  HasValue
                            ? new JProperty("from",             from.     Value.ToString())
                            : null,

                        to.                    HasValue
                            ? new JProperty("to",               to.       Value.ToString())
                            : null,

                              new JProperty("requestId",        requestId.      ToString()),
                              new JProperty("correlationId",    correlationId.  ToString()),
                              new JProperty("requestTimeout",   requestTimeout. TotalSeconds),

                              new JProperty("sessionId",        sessionId.      ToString()),

                              new JProperty("response",         response.       ToJSON(commandResponse => commandResponse.ToJSON())),

                              new JProperty("runtime",          runtime.        TotalMilliseconds)

                    ),
                    cancellationToken
                );

            #endregion

        }


        public void LinkClientEventsToDebugText()
        {
            ClientEventsToDebugText(
                txt => {
                    DebugX.LogT(txt);
                    return Task.CompletedTask;
                }
            );
        }

        public void ClientEventsToDebugText(Func<String, Task> Processor)
        {

            #region OnStartSessionRequest/-Response

            OnStartSessionRequest += (timestamp,
                                      sender,
                                      eventTrackingId,
                                      remotePartyId,
                                      from,
                                      to,
                                      requestId,
                                      correlationId,
                                      requestTimeout,

                                      token,
                                      locationId,
                                      evseUId,
                                      connectorId,
                                      authorizationReference,

                                      cancellationToken) =>

                Processor(
                    $"StartSession request request for '{token.ContractId} / {authorizationReference?.ToString() ?? "-"}' at '{locationId} / {evseUId?.ToString() ?? "-"} / {connectorId?.ToString() ?? "-"}'"
                );


            OnStartSessionResponse += (timestamp,
                                       sender,
                                       eventTrackingId,
                                       remotePartyId,
                                       from,
                                       to,
                                       requestId,
                                       correlationId,
                                       requestTimeout,

                                       token,
                                       locationId,
                                       evseUId,
                                       connectorId,
                                       authorizationReference,

                                       response,
                                       runtime,
                                       cancellationToken) =>

                Processor(
                    $"StartSession response for '{token.ContractId} / {authorizationReference?.ToString() ?? "-"}' at '{locationId}' / {evseUId?.ToString() ?? "-"} / {connectorId?.ToString() ?? "-"}'" +
                    $" => {response.StatusCode} [{runtime.TotalMilliseconds} ms]"
                );

            #endregion

            #region OnStopSessionRequest/-Response

            OnStopSessionRequest += (timestamp,
                                     sender,
                                     eventTrackingId,
                                     remotePartyId,
                                     from,
                                     to,
                                     requestId,
                                     correlationId,
                                     requestTimeout,

                                     sessionId,

                                     cancellationToken) =>

                Processor(
                    $"StopSession request request for session '{sessionId}'"
                );


            OnStopSessionResponse += (timestamp,
                                      sender,
                                      eventTrackingId,
                                      remotePartyId,
                                      from,
                                      to,
                                      requestId,
                                      correlationId,
                                      requestTimeout,

                                      sessionId,

                                      response,
                                      runtime,
                                      cancellationToken) =>

                Processor(
                    $"StopSession response for session '{sessionId}'" +
                    $" => {response.StatusCode} [{runtime.TotalMilliseconds} ms]"
                );

            #endregion

        }


    }

}
