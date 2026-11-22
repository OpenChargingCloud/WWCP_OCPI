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

using System.Security.Authentication;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.TCP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.SSH;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPI.Transport
{

    /// <summary>
    /// The networking node OCPI goes over where it goes over a WebSocket or over
    /// SSH rather than over HTTP: of a CSMS, which connects to an EMSP, and of an
    /// EMSP, which a CPO connects to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For now it holds the line and nothing more: it connects and is
    /// connected to, over the subprotocols and the SSH subsystem of
    /// <see cref="OCPIWireNames"/>, and both ends see the connection as up.
    /// What goes over it - the modules of OCPI as messages rather than as calls
    /// over HTTP - is what it is to carry next.
    /// </para>
    /// <para>
    /// A networking node of WWCP as OCPP's are, and routed as they are: what
    /// connects to it is reached by the name it came under, and what it
    /// connects to by the name of the connection.
    /// </para>
    /// </remarks>
    public class OCPINetworkingNode : AWWCPNetworkingNode
    {

        #region Constructor(s)

        /// <summary>
        /// The networking node of OCPI of a node.
        /// </summary>
        /// <param name="Id">What it is called in the network: the node's own name.</param>
        /// <param name="Description">What it is, in as many languages as it is said in.</param>
        /// <param name="DNSClient">The DNS client the names of what it connects to are resolved with.</param>
        /// <param name="Clock">The clock it reads the time from.</param>
        public OCPINetworkingNode(NetworkingNode_Id  Id,
                                  I18NString?        Description   = null,
                                  IDNSClient?        DNSClient     = null,
                                  TimeProvider?      Clock         = null)

            : base(Id:                     Id,
                   Description:            Description,
                   DisableSendHeartbeats:  true,
                   DNSClient:              DNSClient,
                   Clock:                  Clock)

        { }

        #endregion


        #region AttachWebSocketServer(...)

        /// <summary>
        /// A WebSocket server of this node, speaking the given subprotocols of
        /// OCPI - which may listen on a port of its own, or be handed its
        /// WebSockets by another server that listens (see
        /// org.GraphDefined.Vanaheimr.Hermod.WebSocket.AWebSocketServer.RouteBySubprotocol).
        /// Nothing listens yet.
        /// </summary>
        /// <param name="HTTPServiceName">What the server calls itself.</param>
        /// <param name="IPAddress">The address it listens on.</param>
        /// <param name="TCPPort">The port it listens on.</param>
        /// <param name="Description">What it is.</param>
        /// <param name="RequireAuthentication">Whether the server itself asks for HTTP Basic: no, where a door in front of it decides who comes in.</param>
        /// <param name="SecWebSocketProtocols">The subprotocols it speaks: those of one version of OCPI, see <see cref="OCPIWireNames.SubprotocolsOf"/>.</param>
        /// <param name="WebSocketPingEvery">How often a silent connection is pinged.</param>
        /// <param name="ClientCertificateValidator">Whether a client certificate shown in the TLS handshake is believed.</param>
        /// <param name="AllowedTLSProtocols">The versions of TLS it speaks.</param>
        /// <param name="ClientCertificateRequired">Whether a client is asked for its certificate.</param>
        /// <param name="CheckCertificateRevocation">Whether a client certificate is checked for revocation.</param>
        /// <param name="MaxClientConnections">How many may be connected at once.</param>
        public WWCPWebSocketServer AttachWebSocketServer(String?                                                   HTTPServiceName              = null,
                                                         IPAddress?                                                IPAddress                    = null,
                                                         IPPort?                                                   TCPPort                      = null,
                                                         I18NString?                                               Description                  = null,
                                                         Boolean                                                   RequireAuthentication        = false,
                                                         IEnumerable<String>?                                      SecWebSocketProtocols        = null,
                                                         TimeSpan?                                                 WebSocketPingEvery           = null,
                                                         RemoteTLSClientCertificateValidationHandler<ITCPServer>?  ClientCertificateValidator   = null,
                                                         SslProtocols?                                             AllowedTLSProtocols          = null,
                                                         Boolean?                                                  ClientCertificateRequired    = null,
                                                         Boolean?                                                  CheckCertificateRevocation   = null,
                                                         UInt32?                                                   MaxClientConnections         = null)
        {

            var webSocketServer = new WWCPWebSocketServer(

                                      this,

                                      IPAddress,
                                      TCPPort,
                                      HTTPServiceName,
                                      Description,

                                      RequireAuthentication:       RequireAuthentication,
                                      SecWebSocketProtocols:       SecWebSocketProtocols,
                                      WebSocketPingEvery:          WebSocketPingEvery,

                                      ClientCertificateValidator:  ClientCertificateValidator,
                                      AllowedTLSProtocols:         AllowedTLSProtocols,
                                      ClientCertificateRequired:   ClientCertificateRequired,
                                      CheckCertificateRevocation:  CheckCertificateRevocation,

                                      MaxClientConnections:        MaxClientConnections,
                                      DNSClient:                   DNSClient,
                                      AutoStart:                   false

                                  );

            WireWebSocketServer(webSocketServer);

            return webSocketServer;

        }

        #endregion

        #region AttachSshServer(Version, MaxMessageSize = null, PingEvery = null)

        /// <summary>
        /// Serve the given version of OCPI over the SSH subsystem of OCPI. The SSH
        /// server is not this node's: hand the returned server's
        /// <see cref="WWCPSshServer.Handler"/> to it - or, for several versions on
        /// the one subsystem, the handler of their message servers routed by
        /// subprotocol.
        /// </summary>
        /// <param name="Version">The version, as OCPI labels it: "2.2.1".</param>
        /// <param name="MaxMessageSize">The largest message taken and sent.</param>
        /// <param name="PingEvery">Every how long a PING is sent on every connection; null for none.</param>
        public WWCPSshServer AttachSshServer(String     Version,
                                             UInt64?    MaxMessageSize   = null,
                                             TimeSpan?  PingEvery        = null)

            => AttachSshServer(
                   new WWCPSshServer(
                       this,
                       OCPIWireNames.SshSubsystem,
                       OCPIWireNames.SubprotocolsOf(Version),
                       MaxMessageSize,
                       PingEvery
                   )
               );

        #endregion

    }

}
