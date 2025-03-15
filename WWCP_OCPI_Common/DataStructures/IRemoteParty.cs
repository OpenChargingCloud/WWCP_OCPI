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

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// A remote party.
    /// In OCPI v2.1 this is a single CPO or EMSP.
    /// In OCPI v2.2 this is a roaming network operator serving multiple CPOs and/or EMSPs.
    /// </summary>
    public interface IRemoteParty : IHasId<RemoteParty_Id>,
                                    IComparable
    {

        /// <summary>
        /// The current status of the party.
        /// </summary>
        [Mandatory]
        PartyStatus                                                Status                        { get; }

        /// <summary>
        /// Timestamp when this remote party was last updated (or created).
        /// </summary>
        [Mandatory]
        DateTime                                                   LastUpdated                   { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this remote party.
        /// </summary>
        [Mandatory]
        String                                                     ETag                          { get; }

        /// <summary>
        /// Local access information.
        /// </summary>
        IEnumerable<LocalAccessInfo>                               LocalAccessInfos              { get; }

        /// <summary>
        /// Remote access information.
        /// </summary>
        IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos             { get; }



        /// <summary>
        /// A delegate to select a TLS client certificate.
        /// </summary>
        RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator    { get; }

        /// <summary>
        /// A delegate to select a TLS client certificate.
        /// </summary>
        LocalCertificateSelectionHandler?                          LocalCertificateSelector      { get; }

        /// <summary>
        /// The TLS client certificate to use of HTTP authentication.
        /// </summary>
        X509Certificate?                                           ClientCert                    { get; }

        /// <summary>
        /// The TLS protocol to use.
        /// </summary>
        SslProtocols?                                              TLSProtocol                   { get; }

        /// <summary>
        /// Prefer IPv4 instead of IPv6.
        /// </summary>
        Boolean?                                                   PreferIPv4                    { get; }

        /// <summary>
        /// The HTTP user agent identification.
        /// </summary>
        String?                                                    HTTPUserAgent                 { get; }

        /// <summary>
        /// The optional HTTP authentication to use.
        /// </summary>
        IHTTPAuthentication?                                       HTTPAuthentication            { get; }

        /// <summary>
        /// The timeout for upstream requests.
        /// </summary>
        TimeSpan?                                                  RequestTimeout                { get; set; }

        /// <summary>
        /// The delay between transmission retries.
        /// </summary>
        TransmissionRetryDelayDelegate?                            TransmissionRetryDelay        { get; }

        /// <summary>
        /// The maximum number of retries when communicating with the remote HTTP service.
        /// </summary>
        UInt16?                                                    MaxNumberOfRetries            { get; }

        /// <summary>
        /// The size of the internal buffers of HTTP clients.
        /// </summary>
        UInt32?                                                    InternalBufferSize            { get; }

        /// <summary>
        /// Whether to pipeline multiple HTTP request through a single HTTP/TCP connection.
        /// </summary>
        Boolean?                                                   UseHTTPPipelining             { get; }


    }

}
