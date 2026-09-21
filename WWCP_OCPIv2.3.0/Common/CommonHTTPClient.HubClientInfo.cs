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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// The push half of the HubClientInfo module: a hub telling one of its
    /// peers what became of another.
    /// </summary>
    /// <remarks>
    /// On the common client rather than on HUB2CPOClient and HUB2EMSPClient
    /// both, because it is the same request either way. A hub pushes to a CPO
    /// exactly what it pushes to an EMSP - who is on the hub - and unlike the
    /// functional modules there is no direction in which the two differ.
    /// </remarks>
    public partial class CommonHTTPClient
    {

        #region PutClientInfo(ClientInfo, VersionId = null, ...)

        /// <summary>
        /// Push one ClientInfo object to this peer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The receiver interface, which is the peer's half: PUT to its
        /// clientinfo endpoint with the party in the path. Where that endpoint
        /// is comes out of the peer's own version details, so a peer that does
        /// not offer the module is simply told nothing - see the answer below,
        /// which says so rather than throwing.
        /// </para>
        /// <para>
        /// PUT and never DELETE, however final the change: the specification
        /// has no deletion in this module, and a party that is no longer to be
        /// talked to is pushed with the status SUSPENDED instead.
        /// </para>
        /// </remarks>
        /// <param name="ClientInfo">What became of a party on this hub.</param>
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// <param name="RequestTimestamp">The timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<OCPIResponse<ClientInfo>>

            PutClientInfo(ClientInfo         ClientInfo,
                          Version_Id?        VersionId           = null,

                          Request_Id?        RequestId           = null,
                          Correlation_Id?    CorrelationId       = null,

                          DateTimeOffset?    RequestTimestamp    = null,
                          EventTracking_Id?  EventTrackingId     = null,
                          TimeSpan?          RequestTimeout      = null,
                          CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;

            OCPIResponse<ClientInfo> response;

            #endregion

            try
            {

                var remoteURL = await GetModuleRemoteURL(
                                          Module_Id.HubClientInfo,
                                          InterfaceRoles.RECEIVER,
                                          VersionId,
                                          eventTrackingId,
                                          CancellationToken
                                      );

                if (!remoteURL.HasValue)

                    // Not an error to be retried, and not an exception either:
                    // a peer that does not offer the receiver interface has
                    // made a choice, and a hub that shouted about it once per
                    // status change would fill its own log with somebody
                    // else's decision.
                    response = OCPIResponse<ClientInfo>.Error("This peer offers no HubClientInfo receiver endpoint!");

                else
                {

                    #region Upstream HTTP request...

                    var httpResponse = await NewHTTPClient.PUT(

                                                 Path:                  remoteURL.Value.Path + ClientInfo.CountryCode.ToString() +
                                                                                               ClientInfo.PartyId.    ToString(),

                                                 Content:               ClientInfo.ToJSON().ToUTF8Bytes(JSONFormatting),
                                                 ContentType:           HTTPContentType.Application.JSON_UTF8,
                                                 Accept:                ocpiAcceptTypes,
                                                 Authentication:        TokenAuth,
                                                 Connection:            ConnectionType.Close,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set(HTTPHeaders.X_Request_ID,     requestId);
                                                                            requestBuilder.Set(HTTPHeaders.X_Correlation_ID, correlationId);
                                                                        },
                                                 EventTrackingId:       eventTrackingId,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken

                                             ).ConfigureAwait(false);

                    #endregion

                    // A receiver may answer with the object or with nothing at
                    // all, and both are a success: what matters is that it was
                    // told. So the parser is only asked where there is
                    // something to parse.
                    response = OCPIResponse<ClientInfo>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => ClientInfo.Parse(json)
                               );

                }

            }
            catch (Exception e)
            {
                response = OCPIResponse<String, ClientInfo>.Exception(e);
            }

            return response;

        }

        #endregion

    }

}
