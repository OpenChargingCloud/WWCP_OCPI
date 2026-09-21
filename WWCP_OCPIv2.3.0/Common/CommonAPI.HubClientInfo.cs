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

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// The HubClientInfo module: who is connected to this hub, and whether
    /// they are reachable right now.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A configuration module and not a functional one, which is why it lives
    /// here beside "credentials" and "versions" rather than in the HUB HTTP
    /// API beside "locations" and "sessions". The practical consequence is
    /// that the message routing headers do not apply to it: a call here is
    /// between a peer and the hub itself, never between two peers through it.
    /// </para>
    /// <para>
    /// Two interfaces, and this is the sender half - the hub's own. A peer
    /// asks it for the list at a start, or whenever it doubts what it holds.
    /// The other half is the receiver, which every peer implements and the hub
    /// pushes to; that is what keeps them up to date between those two asks.
    /// </para>
    /// <para>
    /// Nothing here is ever deleted. The specification is explicit about it: a
    /// party that is no longer to be talked to is set to SUSPENDED, because a
    /// peer that never learns of a deletion would go on holding a party that
    /// the hub has forgotten, and neither side could tell that from a party
    /// which is merely quiet.
    /// </para>
    /// </remarks>
    public partial class CommonAPI
    {

        #region Data

        /// <summary>
        /// Who is on this hub, by party.
        /// </summary>
        /// <remarks>
        /// In memory: a hub reads its peers back from the remote party store
        /// at every start, and what it knew of their connection status a
        /// moment before it went down is worthless the moment it comes back.
        /// Everything here is re-established by asking.
        /// </remarks>
        private readonly ConcurrentDictionary<Party_Idv3, ClientInfo> clientInfos = [];

        #endregion

        #region Properties

        /// <summary>
        /// Everyone this hub knows about, whatever their connection status.
        /// </summary>
        public IEnumerable<ClientInfo>  ClientInfos
            => clientInfos.Values;

        /// <summary>
        /// Whether this platform serves the HubClientInfo module at all, i.e.
        /// whether one of its own parties is a hub.
        /// </summary>
        public Boolean                  IsHub
            => parties.Values.Any(partyData => partyData.Role == Role.HUB);

        #endregion

        #region Events

        /// <summary>
        /// A party was added to this hub, or its connection status changed.
        /// </summary>
        /// <remarks>
        /// Raised only on an actual change, so that whoever listens - the push
        /// to the other peers, a log, a web interface - is not woken by a
        /// keepalive that found everything exactly as it was.
        /// </remarks>
        public event Action<ClientInfo>? OnClientInfoChanged;

        #endregion


        #region SetClientInfo   (PartyId, Role, Status, LastUpdated = null)

        /// <summary>
        /// Put a party on this hub, or move it to another connection status,
        /// and answer with what it now is.
        /// </summary>
        /// <remarks>
        /// Idempotent on purpose: the keepalive calls this every time it finds
        /// a peer where it left it, and a hub that raised a change and pushed
        /// to everybody on every such call would spend its bandwidth telling
        /// its peers that nothing happened.
        /// </remarks>
        /// <param name="PartyId">The party this is about.</param>
        /// <param name="Role">What that party is.</param>
        /// <param name="Status">Whether it can be reached.</param>
        /// <param name="LastUpdated">When this was decided; now by default.</param>
        public ClientInfo SetClientInfo(Party_Idv3        PartyId,
                                        Role              Role,
                                        ConnectionStatus  Status,
                                        DateTimeOffset?   LastUpdated   = null)
        {

            var changed     = !clientInfos.TryGetValue(PartyId, out var existing) ||
                               existing.Status != Status ||
                               existing.Role   != Role;

            // The timestamp moves only when something else did. A
            // "last_updated" that ticks on every keepalive would make every
            // peer's date_from filter useless: everything would always be new.
            var clientInfo  = changed
                                  ? new ClientInfo(
                                        PartyId.CountryCode,
                                        PartyId.PartyId,
                                        Role,
                                        Status,
                                        LastUpdated ?? Timestamp.Now
                                    )
                                  : existing;

            clientInfos[PartyId] = clientInfo;

            if (changed)
            {
                try
                {
                    OnClientInfoChanged?.Invoke(clientInfo);
                }
                catch (Exception e)
                {
                    DebugX.LogException(e, $"OCPI {Version.String} {nameof(CommonAPI)}.{nameof(SetClientInfo)}");
                }
            }

            return clientInfo;

        }

        #endregion

        #region TryGetClientInfo(PartyId, out ClientInfo)

        /// <summary>
        /// What this hub knows about one party.
        /// </summary>
        public Boolean TryGetClientInfo(Party_Idv3      PartyId,
                                        out ClientInfo  ClientInfo)

            => clientInfos.TryGetValue(PartyId, out ClientInfo);

        #endregion

        #region GetClientInfos  (From = null, To = null)

        /// <summary>
        /// Everyone on this hub whose entry was last touched within the given
        /// window, oldest first.
        /// </summary>
        /// <remarks>
        /// The window is how a peer catches up without asking for everything:
        /// it remembers when it last asked and hands that back as
        /// "date_from". Exclusive at the lower end and inclusive at the upper,
        /// the way the other modules of this library filter.
        /// </remarks>
        /// <param name="From">Only what was updated after this.</param>
        /// <param name="To">Only what was updated up to and including this.</param>
        public IEnumerable<ClientInfo> GetClientInfos(DateTimeOffset?  From   = null,
                                                      DateTimeOffset?  To     = null)

            => clientInfos.Values.
                   Where  (clientInfo => !From.HasValue || clientInfo.LastUpdated >  From.Value).
                   Where  (clientInfo => !To.  HasValue || clientInfo.LastUpdated <= To.  Value).
                   OrderBy(clientInfo => clientInfo.LastUpdated).
                   ToArray();

        #endregion


        #region CredentialsRoles()

        /// <summary>
        /// The roles this platform hands a peer in the credentials.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Its own, and - new in OCPI 2.3.0, where a hub is concerned - every
        /// party that is reachable through it. In 2.2 and 2.2.1 a hub named
        /// only itself here, and a peer learned who else was on the hub from
        /// the HubClientInfo module or not at all.
        /// </para>
        /// <para>
        /// A party this hub has been told to stop talking to is left out.
        /// SUSPENDED is how the specification says "no longer in play" -
        /// there is no deletion - and a hub that went on advertising such a
        /// party in its credentials would be inviting its peers to address
        /// somebody it will not forward to.
        /// </para>
        /// </remarks>
        public IEnumerable<CredentialsRole> CredentialsRoles()
        {

            var roles = parties.Values.Select(partyData => partyData.ToCredentialsRole()).ToList();

            if (!IsHub)
                return roles;

            foreach (var remoteParty in remoteParties.Values)
            {
                foreach (var role in remoteParty.Roles)
                {

                    if (TryGetClientInfo(role.PartyId, out var clientInfo) &&
                        clientInfo.Status == ConnectionStatus.SUSPENDED)
                    {
                        continue;
                    }

                    // A party can be peered on more than one version, and a
                    // hub is not a second copy of anybody: named once.
                    if (!roles.Any(existing => existing.PartyId == role.PartyId &&
                                               existing.Role    == role.Role))
                    {
                        roles.Add(role);
                    }

                }
            }

            return roles;

        }

        #endregion

        #region (private) RegisterHubClientInfoURLs()

        /// <summary>
        /// The sender interface of the HubClientInfo module, which only a hub
        /// has: GET ~/{version}/hubclientinfo
        /// </summary>
        /// <remarks>
        /// Registered only where one of our own parties is a hub. A CPO or an
        /// EMSP answering this path would be claiming to be something it is
        /// not, and the endpoint is not advertised for them either.
        /// </remarks>
        private void RegisterHubClientInfoURLs()
        {

            if (!IsHub)
                return;

            // -----------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/v2.3.0/hubclientinfo
            // -----------------------------------------------------------------------------------
            this.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + $"{Version.String}/hubclientinfo",
                request => {

                    #region Check access token

                    // Who is on this hub is the business of the parties that
                    // are on it, and of nobody else: a stranger asking gets
                    // the same answer here as anywhere else in OCPI.
                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = StatusCode.ClientErrors.GenericClientError,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    Server                     = HTTPServiceName,
                                    Date                       = Timestamp.Now,
                                    AccessControlAllowOrigin   = "*",
                                    AccessControlAllowMethods  = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                    Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                    AccessControlAllowHeaders  = [ "Authorization" ],
                                    Connection                 = ConnectionType.KeepAlive,
                                    Vary                       = "Accept"
                                }
                            }
                        );

                    }

                    #endregion

                    var filters             = request.GetDateAndPaginationFilters();

                    var allClientInfos      = GetClientInfos().ToArray();

                    var filteredClientInfos = GetClientInfos(
                                                  filters.From,
                                                  filters.To
                                              ).ToArray();

                    var pagedClientInfos    = filteredClientInfos.
                                                  Skip((Int32) (filters.Offset ?? 0)).
                                                  Take((Int32) (filters.Limit  ?? (UInt64) filteredClientInfos.Length)).
                                                  ToArray();

                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            StatusCode           = StatusCode.Success,
                            StatusMessage        = "Hello world!",
                            Data                 = new JArray(pagedClientInfos.Select(clientInfo => clientInfo.ToJSON())),
                            HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode              = HTTPStatusCode.OK,
                                Server                      = HTTPServiceName,
                                Date                        = Timestamp.Now,
                                AccessControlAllowOrigin    = "*",
                                AccessControlAllowMethods   = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                AccessControlAllowHeaders   = [ "Authorization" ],
                                AccessControlExposeHeaders  = [ HTTPHeaders.X_Request_ID, HTTPHeaders.X_Correlation_ID, "X-Total-Count", "X-Filtered-Count" ],
                                Connection                  = ConnectionType.KeepAlive,
                                Vary                        = "Accept"
                            }.
                            Set("X-Total-Count",     allClientInfos.     Length).
                            Set("X-Filtered-Count",  filteredClientInfos.Length)
                        }
                    );

                }

            );

        }

        #endregion

    }

}
