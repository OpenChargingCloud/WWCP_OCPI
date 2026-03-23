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
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.CPO.HUB.HTTP
{

    /// <summary>
    /// The CPO2HUB client is used by CPOs to talk to HUBs.
    /// </summary>
    public partial class CPO2HUB_HTTPClient : IHTTPClient
    {

        public void LinkEventsToHTTPSSE(HTTPEventSource<JObject> HTTPSSE)
        {
            EventsToJSON(
                //async (txt, json, ct) => await HTTPSSE.SubmitEvent(txt, json, ct)
                HTTPSSE.SubmitEvent
            );
        }

        public void EventsToJSON(Func<String, JObject, CancellationToken, Task> Processor)
        {

            #region OnPutLocationRequest

            OnPutLocationRequest += (timestamp,
                                     sender,
                                     eventTrackingId,
                                     remotePartyId,
                                     from,
                                     to,
                                     requestId,
                                     correlationId,
                                     requestTimeout,

                                     //countryCode,
                                     //partyId,
                                     location,

                                     cancellationToken) =>

                Processor(
                    "OnPutLocationRequest",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              //new JProperty("countryCode",       countryCode.ToString()),
                              //new JProperty("partyId",           partyId.    ToString()),
                              new JProperty("location",          location.       ToJSON())

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPutLocationResponse

            OnPutLocationResponse += (timestamp,
                                      sender,
                                      eventTrackingId,
                                      remotePartyId,
                                      from,
                                      to,
                                      requestId,
                                      correlationId,
                                      requestTimeout,

                                      //countryCode,
                                      //partyId,
                                      location,

                                      response,
                                      runtime,
                                      cancellationToken) =>

                Processor(
                    "OnPutLocationResponse",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              new JProperty("location",          location.       ToJSON()),

                              new JProperty("response",          response.       ToJSON()),
                              new JProperty("runtime",           runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPatchLocationRequest

            OnPatchLocationRequest += (timestamp,
                                       sender,
                                       eventTrackingId,
                                       remotePartyId,
                                       from,
                                       to,
                                       requestId,
                                       correlationId,
                                       requestTimeout,

                                       //countryCode,
                                       //partyId,
                                       locationId,
                                       locationPatch,

                                       cancellationToken) =>

                Processor(
                    "OnPatchLocationRequest",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              //new JProperty("countryCode",       countryCode.    ToString()),
                              //new JProperty("partyId",           partyId.        ToString()),
                              new JProperty("locationId",        locationId.     ToString()),
                              new JProperty("locationPatch",     locationPatch)

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPatchLocationResponse

            OnPatchLocationResponse += (timestamp,
                                        sender,
                                        eventTrackingId,
                                        remotePartyId,
                                        from,
                                        to,
                                        requestId,
                                        correlationId,
                                        requestTimeout,

                                        //countryCode,
                                        //partyId,
                                        locationId,
                                        locationPatch,

                                        response,
                                        runtime,
                                        cancellationToken) =>

                Processor(
                    "OnPatchLocationResponse",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              //new JProperty("countryCode",       countryCode.    ToString()),
                              //new JProperty("partyId",           partyId.        ToString()),
                              new JProperty("locationId",        locationId.     ToString()),
                              new JProperty("locationPatch",     locationPatch),

                              new JProperty("response",          response.       ToJSON()),
                              new JProperty("runtime",           runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            #endregion


            #region OnPutEVSERequest

            OnPutEVSERequest += (timestamp,
                                 sender,
                                 eventTrackingId,
                                 remotePartyId,
                                 from,
                                 to,
                                 requestId,
                                 correlationId,
                                 requestTimeout,

                                 evse,
                                 locationId,
                                 countryCode,
                                 partyId,

                                 cancellationToken) =>

                Processor(
                    "OnPutEVSERequest",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              new JProperty("evse",              evse.           ToJSON())

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPutEVSEResponse

            OnPutEVSEResponse += (timestamp,
                                  sender,
                                  eventTrackingId,
                                  remotePartyId,
                                  from,
                                  to,
                                  requestId,
                                  correlationId,
                                  requestTimeout,

                                  evse,
                                  locationId,
                                  countryCode,
                                  partyId,

                                  response,
                                  runtime,
                                  cancellationToken) =>

                Processor(
                    "OnPutEVSEResponse",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              new JProperty("evse",              evse.           ToJSON()),

                              new JProperty("response",          response.       ToJSON()),
                              new JProperty("runtime",           runtime.TotalMilliseconds)

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPatchEVSERequest

            OnPatchEVSERequest += (timestamp,
                                   sender,
                                   eventTrackingId,
                                   remotePartyId,
                                   from,
                                   to,
                                   requestId,
                                   correlationId,
                                   requestTimeout,

                                   countryCode,
                                   partyId,
                                   locationId,
                                   evseId,
                                   evsePatch,

                                   cancellationToken) =>

                Processor(
                    "OnPatchEVSERequest",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              //new JProperty("countryCode",     countryCode.ToString()),
                              //new JProperty("partyId",         partyId.    ToString()),
                              new JProperty("locationId",   locationId.ToString()),
                              new JProperty("evseId",       evseId.    ToString()),
                              new JProperty("evsePatch",    evsePatch)

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPatchEVSEResponse

            OnPatchEVSEResponse += (timestamp,
                                    sender,
                                    eventTrackingId,
                                    remotePartyId,
                                    from,
                                    to,
                                    requestId,
                                    correlationId,
                                    requestTimeout,

                                    countryCode,
                                    partyId,
                                    locationId,
                                    evseId,
                                    evsePatch,

                                    response,
                                    runtime,
                                    cancellationToken) =>

                Processor(
                    "OnPatchEVSEResponse",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              //new JProperty("countryCode",     countryCode.ToString()),
                              //new JProperty("partyId",         partyId.    ToString()),
                              new JProperty("locationId",   locationId.ToString()),
                              new JProperty("evseId",       evseId.    ToString()),
                              new JProperty("evsePatch",    evsePatch),

                              new JProperty("response",     response.  ToJSON()),
                              new JProperty("runtime",      runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            #endregion


            #region OnPutConnectorRequest

            OnPutConnectorRequest += (timestamp,
                                      sender,
                                      eventTrackingId,
                                      remotePartyId,
                                      from,
                                      to,
                                      requestId,
                                      correlationId,
                                      requestTimeout,

                                      connector,
                                      //locationId,
                                      //countryCode,
                                      //partyId,

                                      cancellationToken) =>

                Processor(
                    "OnPutConnectorRequest",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              new JProperty("connector",         connector.      ToJSON())

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPutConnectorResponse

            OnPutConnectorResponse += (timestamp,
                                       sender,
                                       eventTrackingId,
                                       remotePartyId,
                                       from,
                                       to,
                                       requestId,
                                       correlationId,
                                       requestTimeout,

                                       connector,
                                       //locationId,
                                       //countryCode,
                                       //partyId,

                                       response,
                                       runtime,
                                       cancellationToken) =>

                Processor(
                    "OnPutConnectorResponse",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              new JProperty("connector",        connector.       ToJSON()),

                              new JProperty("response",         response.        ToJSON()),
                              new JProperty("runtime",          runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPatchConnectorRequest

            OnPatchConnectorRequest += (timestamp,
                                        sender,
                                        eventTrackingId,
                                        remotePartyId,
                                        from,
                                        to,
                                        requestId,
                                        correlationId,
                                        requestTimeout,

                                        countryCode,
                                        partyId,
                                        locationId,
                                        evseUId,
                                        connectorId,
                                        connectorPatch,

                                        cancellationToken) =>

                Processor(
                    "OnPatchConnectorRequest",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              //new JProperty("countryCode",       countryCode.    ToString()),
                              //new JProperty("partyId",           partyId.        ToString()),
                              new JProperty("locationId",        locationId.     ToString()),
                              new JProperty("connectorId",       connectorId.    ToString()),
                              new JProperty("connectorPatch",    connectorPatch)

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPatchConnectorResponse

            OnPatchConnectorResponse += (timestamp,
                                         sender,
                                         eventTrackingId,
                                         remotePartyId,
                                         from,
                                         to,
                                         requestId,
                                         correlationId,
                                         requestTimeout,

                                         countryCode,
                                         partyId,
                                         locationId,
                                         evseUId,
                                         connectorId,
                                         connectorPatch,

                                         response,
                                         runtime,
                                         cancellationToken) =>

                Processor(
                    "OnPatchConnectorResponse",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              //new JProperty("countryCode",       countryCode.    ToString()),
                              //new JProperty("partyId",           partyId.        ToString()),
                              new JProperty("locationId",        locationId.     ToString()),
                              new JProperty("evseUId",           evseUId.        ToString()),
                              new JProperty("connectorId",       connectorId.    ToString()),
                              new JProperty("connectorPatch",    connectorPatch),

                              new JProperty("response",          response.       ToJSON()),
                              new JProperty("runtime",           runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            #endregion


            #region OnPutTariffRequest

            OnPutTariffRequest += (timestamp,
                                   sender,
                                   eventTrackingId,
                                   remotePartyId,
                                   from,
                                   to,
                                   requestId,
                                   correlationId,
                                   requestTimeout,

                                   tariff,

                                   cancellationToken) =>

                Processor(
                    "OnPutTariffRequest",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              new JProperty("tariff",            tariff.         ToJSON())

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPutTariffResponse

            OnPutTariffResponse += (timestamp,
                                    sender,
                                    eventTrackingId,
                                    remotePartyId,
                                    from,
                                    to,
                                    requestId,
                                    correlationId,
                                    requestTimeout,

                                    tariff,

                                    response,
                                    runtime,
                                    cancellationToken) =>

                Processor(
                    "OnPutTariffResponse",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              new JProperty("tariff",            tariff.         ToJSON()),

                              new JProperty("response",          response.       ToJSON()),
                              new JProperty("runtime",           runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            #endregion


            #region OnPutSessionRequest

            OnPutSessionRequest += (timestamp,
                                    sender,
                                    eventTrackingId,
                                    remotePartyId,
                                    from,
                                    to,
                                    requestId,
                                    correlationId,
                                    requestTimeout,

                                    session,

                                    cancellationToken) =>

                Processor(
                    "OnPutSessionRequest",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              new JProperty("session",           session.        ToJSON())

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPutSessionResponse

            OnPutSessionResponse += (timestamp,
                                     sender,
                                     eventTrackingId,
                                     remotePartyId,
                                     from,
                                     to,
                                     requestId,
                                     correlationId,
                                     requestTimeout,

                                     session,

                                     response,
                                     runtime,
                                     cancellationToken) =>

                Processor(
                    "OnPutSessionResponse",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              new JProperty("session",           session.        ToJSON()),

                              new JProperty("response",          response.       ToJSON()),
                              new JProperty("runtime",           runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPatchSessionRequest

            OnPatchSessionRequest += (timestamp,
                                      sender,
                                      eventTrackingId,
                                      remotePartyId,
                                      from,
                                      to,
                                      requestId,
                                      correlationId,
                                      requestTimeout,

                                      countryCode,
                                      partyId,
                                      sessionId,
                                      sessionPatch,

                                      cancellationToken) =>

                Processor(
                    "OnPatchSessionRequest",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              new JProperty("countryCode",       countryCode.    ToString()),
                              new JProperty("partyId",           partyId.        ToString()),
                              new JProperty("sessionId",         sessionId.      ToString()),
                              new JProperty("sessionPatch",      sessionPatch)

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPatchSessionResponse

            OnPatchSessionResponse += (timestamp,
                                       sender,
                                       eventTrackingId,
                                       remotePartyId,
                                       from,
                                       to,
                                       requestId,
                                       correlationId,
                                       requestTimeout,

                                       countryCode,
                                       partyId,
                                       sessionId,
                                       sessionPatch,

                                       response,
                                       runtime,
                                       cancellationToken) =>

                Processor(
                    "OnPatchSessionResponse",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              new JProperty("countryCode",       countryCode.    ToString()),
                              new JProperty("partyId",           partyId.        ToString()),
                              new JProperty("sessionId",         sessionId.      ToString()),
                              new JProperty("sessionPatch",      sessionPatch),

                              new JProperty("response",          response.       ToJSON()),
                              new JProperty("runtime",           runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            #endregion


            #region OnPostCDRRequest

            OnPostCDRRequest += (timestamp,
                                 sender,
                                 eventTrackingId,
                                 remotePartyId,
                                 from,
                                 to,
                                 requestId,
                                 correlationId,
                                 requestTimeout,

                                 cdr,

                                 cancellationToken) =>

                Processor(
                    "OnPostCDRRequest",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              new JProperty("cdr",               cdr.            ToJSON())

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPostCDRResponse

            OnPostCDRResponse += (timestamp,
                                  sender,
                                  eventTrackingId,
                                  remotePartyId,
                                  from,
                                  to,
                                  requestId,
                                  correlationId,
                                  requestTimeout,

                                  cdr,

                                  response,
                                  runtime,
                                  cancellationToken) =>

                Processor(
                    "OnPostCDRResponse",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.      ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.ToString()),
                              new JProperty("remotePartyId",     remotePartyId.  ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.     Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.       Value.ToString())
                            : null,

                              new JProperty("cdr",               cdr.            ToJSON()),

                              new JProperty("response",          response.       ToJSON()),
                              new JProperty("runtime",           runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            #endregion


            #region OnPostTokenRequest

            OnPostTokenRequest += (timestamp,
                                   sender,
                                   eventTrackingId,
                                   remotePartyId,
                                   from,
                                   to,
                                   requestId,
                                   correlationId,
                                   requestTimeout,

                                   tokenId,
                                   tokenType,
                                   locationReference,

                                   cancellationToken) =>

                Processor(
                    "OnPostTokenRequest",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.              ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.        ToString()),
                              new JProperty("remotePartyId",     remotePartyId.          ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.             Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.               Value.ToString())
                            : null,

                              new JProperty("tokenId",           tokenId.                ToString()),
                              new JProperty("tokenType",         tokenType.              ToString()),

                        locationReference.HasValue

                            ? new JProperty("tokenType",         locationReference.Value.ToJSON())
                            : null

                    ),
                    cancellationToken
                );

            #endregion

            #region OnPostTokenResponse

            OnPostTokenResponse += (timestamp,
                                    sender,
                                    eventTrackingId,
                                    remotePartyId,
                                    from,
                                    to,
                                    requestId,
                                    correlationId,
                                    requestTimeout,

                                    tokenId,
                                    tokenType,
                                    locationReference,

                                    response,
                                    runtime,
                                    cancellationToken) =>

                Processor(
                    "OnPostTokenResponse",
                    JSONObject.Create(

                              new JProperty("timestamp",         timestamp.              ToISO8601()),
                              new JProperty("eventTrackingId",   eventTrackingId.        ToString()),
                              new JProperty("remotePartyId",     remotePartyId.          ToString()),

                        from.HasValue
                            ? new JProperty("from",              from.             Value.ToString())
                            : null,

                        to.  HasValue
                            ? new JProperty("to",                to.               Value.ToString())
                            : null,

                              new JProperty("tokenId",           tokenId.                ToString()),
                              new JProperty("tokenType",         tokenType.              ToString()),

                        locationReference.HasValue

                            ? new JProperty("tokenType",         locationReference.Value.ToJSON(
                                                                     CustomLocationReferenceSerializer
                                                                 ))
                            : null,

                              new JProperty("response",          response.               ToJSON()),
                              new JProperty("runtime",           runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            #endregion


        }

    }

}
