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

namespace cloud.charging.open.protocols.OCPIv2_2_1.CPO.HTTP
{

    /// <summary>
    /// The CPO2EMSP client is used by CPOs to talk to EMSPs.
    /// </summary>
    public partial class CPO2EMSP_HTTPClient : IHTTPClient
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

            OnPutLocationRequest += async (timestamp,
                                           sender,
                                           eventTrackingId,
                                           requestId,
                                           correlationId,
                                           requestTimeout,

                                           //countryCode,
                                           //partyId,
                                           location,

                                           cancellationToken) => {

                await Processor(
                    "OnPutLocationRequest",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              //new JProperty("countryCode",     countryCode.ToString()),
                              //new JProperty("partyId",         partyId.    ToString()),
                              new JProperty("location",   location.ToJSON())

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPutLocationResponse

            OnPutLocationResponse += async (timestamp,
                                            sender,
                                            eventTrackingId,
                                            requestId,
                                            correlationId,
                                            requestTimeout,

                                            //countryCode,
                                            //partyId,
                                            location,

                                            response,
                                            runtime,
                                            cancellationToken) => {

                await Processor(
                    "OnPutLocationResponse",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              //new JProperty("countryCode",     countryCode.ToString()),
                              //new JProperty("partyId",         partyId.    ToString()),
                              new JProperty("location",    location.ToJSON()),

                              new JProperty("response",    response.ToJSON()),
                              new JProperty("runtime",     runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchLocationRequest

            OnPatchLocationRequest += async (timestamp,
                                             sender,
                                             eventTrackingId,
                                             requestId,
                                             correlationId,
                                             requestTimeout,

                                             //countryCode,
                                             //partyId,
                                             locationId,
                                             locationPatch,

                                             cancellationToken) => {

                await Processor(
                    "OnPatchLocationRequest",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              //new JProperty("countryCode",     countryCode.ToString()),
                              //new JProperty("partyId",         partyId.    ToString()),
                              new JProperty("locationId",      locationId. ToString()),
                              new JProperty("locationPatch",   locationPatch)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchLocationResponse

            OnPatchLocationResponse += async (timestamp,
                                              sender,
                                              eventTrackingId,
                                              requestId,
                                              correlationId,
                                              requestTimeout,

                                              //countryCode,
                                              //partyId,
                                              locationId,
                                              locationPatch,

                                              response,
                                              runtime,
                                              cancellationToken) => {

                await Processor(
                    "OnPatchLocationResponse",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              //new JProperty("countryCode",     countryCode.ToString()),
                              //new JProperty("partyId",         partyId.    ToString()),
                              new JProperty("locationId",      locationId. ToString()),
                              new JProperty("locationPatch",   locationPatch),

                              new JProperty("response",        response.   ToJSON()),
                              new JProperty("runtime",         runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion


            #region OnPutEVSERequest

            OnPutEVSERequest += async (timestamp,
                                       sender,
                                       eventTrackingId,
                                       requestId,
                                       correlationId,
                                       requestTimeout,

                                       evse,
                                       locationId,
                                       countryCode,
                                       partyId,

                                       cancellationToken) => {

                await Processor(
                    "OnPutEVSERequest",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("evse",   evse.ToJSON())

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPutEVSEResponse

            OnPutEVSEResponse += async (timestamp,
                                        sender,
                                        eventTrackingId,
                                        requestId,
                                        correlationId,
                                        requestTimeout,

                                        evse,
                                        locationId,
                                        countryCode,
                                        partyId,

                                        response,
                                        runtime,
                                        cancellationToken) => {

                await Processor(
                    "OnPutEVSEResponse",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("evse",        evse.    ToJSON()),

                              new JProperty("response",    response.ToJSON()),
                              new JProperty("runtime",     runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchEVSERequest

            OnPatchEVSERequest += async (timestamp,
                                         sender,
                                         eventTrackingId,
                                         requestId,
                                         correlationId,
                                         requestTimeout,

                                         countryCode,
                                         partyId,
                                         locationId,
                                         evseId,
                                         evsePatch,

                                         cancellationToken) => {

                await Processor(
                    "OnPatchEVSERequest",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              //new JProperty("countryCode",     countryCode.ToString()),
                              //new JProperty("partyId",         partyId.    ToString()),
                              new JProperty("locationId",   locationId.ToString()),
                              new JProperty("evseId",       evseId.    ToString()),
                              new JProperty("evsePatch",    evsePatch)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchEVSEResponse

            OnPatchEVSEResponse += async (timestamp,
                                          sender,
                                          eventTrackingId,
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
                                          cancellationToken) => {

                await Processor(
                    "OnPatchEVSEResponse",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

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

            };

            #endregion


            #region OnPutConnectorRequest

            OnPutConnectorRequest += async (timestamp,
                                            sender,
                                            eventTrackingId,
                                            requestId,
                                            correlationId,
                                            requestTimeout,

                                            connector,
                                            //locationId,
                                            //countryCode,
                                            //partyId,

                                            cancellationToken) => {

                await Processor(
                    "OnPutConnectorRequest",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("connector",   connector.ToJSON())

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPutConnectorResponse

            OnPutConnectorResponse += async (timestamp,
                                             sender,
                                             eventTrackingId,
                                             requestId,
                                             correlationId,
                                             requestTimeout,

                                             connector,
                                             //locationId,
                                             //countryCode,
                                             //partyId,

                                             response,
                                             runtime,
                                             cancellationToken) => {

                await Processor(
                    "OnPutConnectorResponse",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("connector",        connector.    ToJSON()),

                              new JProperty("response",    response.ToJSON()),
                              new JProperty("runtime",     runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchConnectorRequest

            OnPatchConnectorRequest += async (timestamp,
                                              sender,
                                              eventTrackingId,
                                              requestId,
                                              correlationId,
                                              requestTimeout,

                                              countryCode,
                                              partyId,
                                              locationId,
                                              evseUId,
                                              connectorId,
                                              connectorPatch,

                                              cancellationToken) => {

                await Processor(
                    "OnPatchConnectorRequest",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              //new JProperty("countryCode",     countryCode.ToString()),
                              //new JProperty("partyId",         partyId.    ToString()),
                              new JProperty("locationId",       locationId. ToString()),
                              new JProperty("connectorId",      connectorId.ToString()),
                              new JProperty("connectorPatch",   connectorPatch)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchConnectorResponse

            OnPatchConnectorResponse += async (timestamp,
                                               sender,
                                               eventTrackingId,
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
                                               cancellationToken) => {

                await Processor(
                    "OnPatchConnectorResponse",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              //new JProperty("countryCode",      countryCode.ToString()),
                              //new JProperty("partyId",          partyId.    ToString()),
                              new JProperty("locationId",       locationId. ToString()),
                              new JProperty("evseUId",          evseUId.    ToString()),
                              new JProperty("connectorId",      connectorId.ToString()),
                              new JProperty("connectorPatch",   connectorPatch),

                              new JProperty("response",         response.  ToJSON()),
                              new JProperty("runtime",          runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion


            #region OnPutTariffRequest

            OnPutTariffRequest += async (timestamp,
                                         sender,
                                         eventTrackingId,
                                         requestId,
                                         correlationId,
                                         requestTimeout,

                                         tariff,

                                         cancellationToken) => {

                await Processor(
                    "OnPutTariffRequest",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("tariff",   tariff.ToJSON())

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPutTariffResponse

            OnPutTariffResponse += async (timestamp,
                                          sender,
                                          eventTrackingId,
                                          requestId,
                                          correlationId,
                                          requestTimeout,

                                          tariff,

                                          response,
                                          runtime,
                                          cancellationToken) => {

                await Processor(
                    "OnPutTariffResponse",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("tariff",     tariff. ToJSON()),

                              new JProperty("response",   response.ToJSON()),
                              new JProperty("runtime",    runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion


            #region OnPutSessionRequest

            OnPutSessionRequest += async (timestamp,
                                          sender,
                                          eventTrackingId,
                                          requestId,
                                          correlationId,
                                          requestTimeout,

                                          session,

                                          cancellationToken) => {

                await Processor(
                    "OnPutSessionRequest",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("session",   session.ToJSON())

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPutSessionResponse

            OnPutSessionResponse += async (timestamp,
                                           sender,
                                           eventTrackingId,
                                           requestId,
                                           correlationId,
                                           requestTimeout,

                                           session,

                                           response,
                                           runtime,
                                           cancellationToken) => {

                await Processor(
                    "OnPutSessionResponse",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("session",     session. ToJSON()),

                              new JProperty("response",    response.ToJSON()),
                              new JProperty("runtime",     runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchSessionRequest

            OnPatchSessionRequest += async (timestamp,
                                            sender,
                                            eventTrackingId,
                                            requestId,
                                            correlationId,
                                            requestTimeout,

                                            countryCode,
                                            partyId,
                                            sessionId,
                                            sessionPatch,

                                            cancellationToken) => {

                await Processor(
                    "OnPatchSessionRequest",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("countryCode",    countryCode.ToString()),
                              new JProperty("partyId",        partyId.    ToString()),
                              new JProperty("sessionId",      sessionId.  ToString()),
                              new JProperty("sessionPatch",   sessionPatch)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchSessionResponse

            OnPatchSessionResponse += async (timestamp,
                                             sender,
                                             eventTrackingId,
                                             requestId,
                                             correlationId,
                                             requestTimeout,

                                             countryCode,
                                             partyId,
                                             sessionId,
                                             sessionPatch,

                                             response,
                                             runtime,
                                             cancellationToken) => {

                await Processor(
                    "OnPatchSessionResponse",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("countryCode",    countryCode.ToString()),
                              new JProperty("partyId",        partyId.    ToString()),
                              new JProperty("sessionId",      sessionId.  ToString()),
                              new JProperty("sessionPatch",   sessionPatch),

                              new JProperty("response",       response.   ToJSON()),
                              new JProperty("runtime",        runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion


            #region OnPostCDRRequest

            OnPostCDRRequest += async (timestamp,
                                       sender,
                                       eventTrackingId,
                                       requestId,
                                       correlationId,
                                       requestTimeout,

                                       cdr,

                                       cancellationToken) => {

                await Processor(
                    "OnPostCDRRequest",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("cdr",   cdr.              ToJSON())

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPostCDRResponse

            OnPostCDRResponse += async (timestamp,
                                        sender,
                                        eventTrackingId,
                                        requestId,
                                        correlationId,
                                        requestTimeout,

                                        cdr,

                                        response,
                                        runtime,
                                        cancellationToken) => {

                await Processor(
                    "OnPostCDRResponse",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("cdr",         cdr.                    ToJSON()),

                              new JProperty("response",    response.               ToJSON()),
                              new JProperty("runtime",     runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion


            #region OnPostTokenRequest

            OnPostTokenRequest += async (timestamp,
                                         sender,
                                         eventTrackingId,
                                         requestId,
                                         correlationId,
                                         requestTimeout,

                                         tokenId,
                                         tokenType,
                                         locationReference,

                                         cancellationToken) => {

                await Processor(
                    "OnPostTokenRequest",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("tokenId",     tokenId.                ToString()),
                              new JProperty("tokenType",   tokenType.              ToString()),

                        locationReference.HasValue

                            ? new JProperty("tokenType",   locationReference.Value.ToJSON())
                            : null

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPostTokenResponse

            OnPostTokenResponse += async (timestamp,
                                          sender,
                                          eventTrackingId,
                                          requestId,
                                          correlationId,
                                          requestTimeout,

                                          tokenId,
                                          tokenType,
                                          locationReference,

                                          response,
                                          runtime,
                                          cancellationToken) => {

                await Processor(
                    "OnPostTokenResponse",
                    JSONObject.Create(

                        //from_CountryCode.HasValue && from_PartyId.HasValue
                        //    ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                        //    : null,

                        //to_CountryCode.  HasValue && to_PartyId.  HasValue
                        //    ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                        //    : null,

                              new JProperty("tokenId",     tokenId.                ToString()),
                              new JProperty("tokenType",   tokenType.              ToString()),

                        locationReference.HasValue

                            ? new JProperty("tokenType",   locationReference.Value.ToJSON(
                                                               CustomLocationReferenceSerializer
                                                           ))
                            : null,

                              new JProperty("response",    response.               ToJSON()),
                              new JProperty("runtime",     runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion

        }

    }

}
