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

using cloud.charging.open.protocols.WWCP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPI.Transport
{

    /// <summary>
    /// What OCPI is called on the wire where it goes over a WebSocket or over
    /// SSH rather than over HTTP: the WebSocket subprotocols of its versions,
    /// and the SSH subsystem.
    /// </summary>
    /// <remarks>
    /// <para>
    /// As OCPP names its own: "ocpp2.1", and "ocpp2.1cbor" for the same version
    /// with every message as CBOR in a binary frame. Here "ocpi2.2.1" and
    /// "ocpi2.2.1cbor" - the version as OCPI labels it, behind "ocpi" - for each
    /// of 2.1.1, 2.2.1 and 2.3.0.
    /// </para>
    /// <para>
    /// The SSH subsystem is one for every version, as OCPP's is: which version
    /// is spoken is offered in the HELLO, by these subprotocols.
    /// </para>
    /// </remarks>
    public static class OCPIWireNames
    {

        #region Data

        /// <summary>
        /// The SSH subsystem OCPI goes over: "ocpi@open.charging.cloud".
        /// </summary>
        public const String SshSubsystem = "ocpi@open.charging.cloud";

        /// <summary>
        /// What the name of a subprotocol starts with: "ocpi".
        /// </summary>
        public const String Prefix       = "ocpi";

        /// <summary>
        /// What the name of a subprotocol ends in when its messages are CBOR,
        /// as OCPP's do: "cbor".
        /// </summary>
        public const String CBORSuffix   = "cbor";

        /// <summary>
        /// The versions of OCPI that go over a WebSocket or over SSH, as OCPI
        /// labels them.
        /// </summary>
        public static readonly IReadOnlyList<String> Versions = [ "2.1.1", "2.2.1", "2.3.0" ];

        #endregion


        #region SubprotocolOf(Version) / CBOROf(Version) / SubprotocolsOf(Version)

        /// <summary>
        /// The subprotocol of the given version of OCPI, with its messages as
        /// JSON: "ocpi2.2.1".
        /// </summary>
        /// <param name="Version">The version, as OCPI labels it: "2.2.1".</param>
        public static String SubprotocolOf(String Version)

            => Prefix + Version;

        /// <summary>
        /// The subprotocol of the given version of OCPI, with its messages as
        /// CBOR: "ocpi2.2.1cbor".
        /// </summary>
        /// <param name="Version">The version, as OCPI labels it: "2.2.1".</param>
        public static String CBOROf(String Version)

            => Prefix + Version + CBORSuffix;

        /// <summary>
        /// Both subprotocols of the given version of OCPI - JSON, then CBOR -:
        /// what a server of that version speaks.
        /// </summary>
        /// <param name="Version">The version, as OCPI labels it: "2.2.1".</param>
        public static IReadOnlyList<String> SubprotocolsOf(String Version)

            => [ SubprotocolOf(Version), CBOROf(Version) ];

        #endregion

        #region Offer(Version, Encoding)

        /// <summary>
        /// What a connection of the given version of OCPI offers, by preference,
        /// for the way its messages shall be written - as a connection of OCPP
        /// offers it: JSON the subprotocol as it is, CBOR its CBOR form alone,
        /// so that a far end without CBOR refuses the connection, and "auto"
        /// the CBOR form first and JSON after it.
        /// </summary>
        /// <param name="Version">The version, as OCPI labels it: "2.2.1".</param>
        /// <param name="Encoding">How the messages shall be written.</param>
        public static IReadOnlyList<String> Offer(String            Version,
                                                  MessageEncodings  Encoding)

            => Encoding switch {
                   MessageEncodings.CBOR  => [ CBOROf(Version) ],
                   MessageEncodings.Auto  => [ CBOROf(Version), SubprotocolOf(Version) ],
                   _                      => [ SubprotocolOf(Version) ]
               };

        #endregion

        #region IsCBOR(Subprotocol) / VersionOf(Subprotocol)

        /// <summary>
        /// Whether the messages of the given subprotocol of OCPI are CBOR in
        /// binary frames.
        /// </summary>
        /// <param name="Subprotocol">The subprotocol a connection agreed on, if any.</param>
        public static Boolean IsCBOR(String? Subprotocol)

            => VersionOf(Subprotocol) is not null &&
               Subprotocol!.EndsWith(CBORSuffix, StringComparison.Ordinal);

        /// <summary>
        /// The version of OCPI the given subprotocol speaks - "2.2.1" of
        /// "ocpi2.2.1" and of "ocpi2.2.1cbor" - or null, where it is none of OCPI.
        /// </summary>
        /// <param name="Subprotocol">A subprotocol.</param>
        public static String? VersionOf(String? Subprotocol)

            => Subprotocol is null
                   ? null
                   : Versions.FirstOrDefault(version => Subprotocol == SubprotocolOf(version) ||
                                                        Subprotocol == CBOROf       (version));

        #endregion

    }

}
