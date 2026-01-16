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

using System.Net.Security;
using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// The common OCPI HTTP Client.
    /// </summary>
    public abstract class ACommonHTTPClient : AHTTPClient
    {

        #region Data

        protected readonly  AcceptTypes                            ocpiAcceptTypes   = AcceptTypes.FromHTTPContentTypes(HTTPContentType.Application.JSON_UTF8);
        protected readonly  HTTPContentType                        ocpiContentType   = HTTPContentType.Application.JSON_UTF8;

        protected readonly  ConcurrentDictionary<Version_Id, URL>  versions          = [];

        #endregion

        #region Properties

        /// <summary>
        /// The current remote URL of the remote VERSIONS endpoint to connect to.
        /// The remote party might define multiple for H/A reasons.
        /// </summary>
        public     URL                       RemoteVersionsURL             { get; }

        /// <summary>
        /// The current access token.
        /// The remote party might define different access token for each VersionsAPI endpoint for H/A and/or security reasons.
        /// The access token might be updated during the REGISTRATION process!
        /// </summary>
        public     AccessToken?              RemoteAccessToken             { get; protected set; }

        public     Boolean?                  AccessTokenIsBase64Encoded    { get; protected set; }

        /// <summary>
        /// The current HTTP Token Authentication based on the current access token.
        /// </summary>
        public     HTTPTokenAuthentication?  TokenAuth                     { get; protected set; }


        /// <summary>
        /// The selected OCPI version.
        /// The selected OCPI version might be updated during the REGISTRATION process!
        /// </summary>
        public     Version_Id?               SelectedOCPIVersionId         { get; set; }

        /// <summary>
        /// The JSON formatting to use.
        /// </summary>
        public     Formatting                JSONFormatting                { get; set; } = Formatting.None;

        /// <summary>
        /// The HTTP client.
        /// </summary>
        public     HTTPTestClient            NewHTTPClient;

        /// <summary>
        /// A HTTP client pool for low-latency HTTP requests.
        /// </summary>
        public     HTTPClientPool            NewHTTPClientPool;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new common OCPI HTTP client.
        /// </summary>
        /// <param name="VersionsURL">The remote URL of the OCPI versions endpoint to connect to.</param>
        /// <param name="AccessToken">The optional OCPI access token.</param>
        /// <param name="AccessTokenIsBase64Encoded">Whether the access token is Base64 encoded.</param>
        /// <param name="TOTPConfig">An optional TOTP configuration for 2nd factor authentication.</param>
        /// 
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this client.</param>
        /// <param name="MaxNumberOfPooledClients">An optional maximum number of pooled HTTP clients.</param>
        /// <param name="PreferIPv4">Prefer IPv4 instead of IPv6.</param>
        /// <param name="RemoteCertificateValidator">The remote TLS certificate validator.</param>
        /// <param name="LocalCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="ClientCertificates">An optional enumeration of TLS client certificates to use for HTTP authentication.</param>
        /// <param name="ClientCertificateContext">An optional TLS client certificate context.</param>
        /// <param name="ClientCertificateChain">An optional TLS client certificate chain.</param>
        /// <param name="TLSProtocols">The TLS protocol to use.</param>
        /// 
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="Accept">The optional HTTP accept header.</param>
        /// <param name="ContentType">An optional HTTP content type.</param>
        /// <param name="Connection">The optional HTTP connection type.</param>
        /// 
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">An optional maximum number of transmission retries for HTTP request.</param>
        /// <param name="InternalBufferSize">An optional size of the internal HTTP client buffers.</param>
        /// <param name="UseHTTPPipelining">Whether to pipeline multiple HTTP request through a single HTTP/TCP connection.</param>
        /// <param name="DisableLogging">Whether to disable all logging.</param>
        /// <param name="HTTPLogger">An optional delegate to log HTTP(S) requests and responses.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public ACommonHTTPClient(URL                                                        VersionsURL,
                                 AccessToken?                                               AccessToken                  = null,
                                 Boolean?                                                   AccessTokenIsBase64Encoded   = null,
                                 TOTPConfig?                                                TOTPConfig                   = null,

                                 HTTPHostname?                                              VirtualHostname              = null,
                                 I18NString?                                                Description                  = null,
                                 UInt16?                                                    MaxNumberOfPooledClients     = null,
                                 Boolean?                                                   PreferIPv4                   = null,
                                 RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                 LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                 IEnumerable<X509Certificate2>?                             ClientCertificates           = null,
                                 SslStreamCertificateContext?                               ClientCertificateContext     = null,
                                 IEnumerable<X509Certificate2>?                             ClientCertificateChain       = null,
                                 SslProtocols?                                              TLSProtocols                 = null,

                                 String?                                                    HTTPUserAgent                = DefaultHTTPUserAgent,
                                 AcceptTypes?                                               Accept                       = null,
                                 HTTPContentType?                                           ContentType                  = null,
                                 ConnectionType?                                            Connection                   = null,

                                 TimeSpan?                                                  RequestTimeout               = null,
                                 TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                 UInt16?                                                    MaxNumberOfRetries           = DefaultMaxNumberOfRetries,
                                 UInt32?                                                    InternalBufferSize           = DefaultInternalBufferSize,
                                 Boolean?                                                   UseHTTPPipelining            = null,
                                 Boolean?                                                   DisableLogging               = false,
                                 HTTPClientLogger?                                          HTTPLogger                   = null,
                                 IDNSClient?                                                DNSClient                    = null)

            : base(VersionsURL,
                   VirtualHostname,
                   Description,
                   PreferIPv4,
                   RemoteCertificateValidator,
                   LocalCertificateSelector,
                   ClientCertificates,
                   ClientCertificateContext,
                   ClientCertificateChain,
                   TLSProtocols,
                   ContentType    ?? HTTPContentType.Application.JSON_UTF8,
                   Accept         ?? AcceptTypes.FromHTTPContentTypes(HTTPContentType.Application.JSON_UTF8),
                   AccessToken.HasValue
                       ? AccessTokenIsBase64Encoded ?? true
                             ? HTTPTokenAuthentication.Parse(AccessToken.Value.ToString().ToBase64())
                             : HTTPTokenAuthentication.Parse(AccessToken.Value.ToString())
                       : null,
                   TOTPConfig,
                   HTTPUserAgent  ?? DefaultHTTPUserAgent,
                   Connection     ?? ConnectionType.KeepAlive,
                   RequestTimeout ?? DefaultRequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,
                   UseHTTPPipelining,
                   DisableLogging,
                   HTTPLogger,
                   DNSClient)

        {

            this.RemoteVersionsURL  = VersionsURL;
            this.RemoteAccessToken  = AccessToken;
            this.TokenAuth          = HTTPAuthentication as HTTPTokenAuthentication;

            this.NewHTTPClient      = new HTTPTestClient(

                                          URL:                                   this.RemoteURL,
                                          Description:                           this.Description,

                                          HTTPUserAgent:                         this.HTTPUserAgent,
                                          Accept:                                ocpiAcceptTypes,
                                          ContentType:                           ocpiContentType,
                                          Connection:                            this.Connection ?? ConnectionType.KeepAlive,
                                          DefaultRequestBuilder:                 () => new HTTPRequest.Builder(this, CancellationToken.None) {
                                                                                           Host         = this.RemoteURL.Hostname,
                                                                                           Accept       = ocpiAcceptTypes,
                                                                                           ContentType  = ocpiContentType,
                                                                                           UserAgent    = this.HTTPUserAgent ?? DefaultHTTPUserAgent,
                                                                                           Connection   = this.Connection    ?? ConnectionType.KeepAlive
                                                                                       },

                                          RemoteCertificateValidator:            this.RemoteCertificateValidator is not null
                                                                                     ? (sender,
                                                                                        certificate,
                                                                                        certificateChain,
                                                                                        httpTestClient,
                                                                                        policyErrors) => this.RemoteCertificateValidator.Invoke(
                                                                                                             sender,
                                                                                                             certificate,
                                                                                                             certificateChain,
                                                                                                             this,
                                                                                                             policyErrors
                                                                                                         )
                                                                                     :  null,
                                          LocalCertificateSelector:              this.LocalCertificateSelector,
                                          ClientCertificates:                    this.ClientCertificates,
                                          ClientCertificateContext:              this.ClientCertificateContext,
                                          ClientCertificateChain:                this.ClientCertificateChain,
                                          TLSProtocols:                          this.TLSProtocols,
                                          CertificateRevocationCheckMode:        X509RevocationMode.NoCheck,
                                          ApplicationProtocols:                  null,
                                          AllowRenegotiation:                    null,
                                          AllowTLSResume:                        null,
                                          TOTPConfig:                            TOTPConfig,

                                          ConsumeRequestChunkedTEImmediately:    true,
                                          ConsumeResponseChunkedTEImmediately:   true,

                                          PreferIPv4:                            this.PreferIPv4,
                                          ConnectTimeout:                        null,
                                          ReceiveTimeout:                        null,
                                          SendTimeout:                           null,
                                          BufferSize:                            null,

                                          DNSClient:                             this.DNSClient

                                      );

            this.NewHTTPClientPool     = new HTTPClientPool(

                                          URL:                                   this.RemoteURL,
                                          Description:                           this.Description,

                                          HTTPUserAgent:                         this.HTTPUserAgent,
                                          Accept:                                ocpiAcceptTypes,
                                          ContentType:                           ocpiContentType,
                                          Connection:                            this.Connection ?? ConnectionType.KeepAlive,
                                          DefaultRequestBuilder:                 () => new HTTPRequest.Builder(this, CancellationToken.None) {
                                                                                           Host         = this.RemoteURL.Hostname,
                                                                                           Accept       = ocpiAcceptTypes,
                                                                                           ContentType  = ocpiContentType,
                                                                                           UserAgent    = this.HTTPUserAgent ?? DefaultHTTPUserAgent,
                                                                                           Connection   = this.Connection    ?? ConnectionType.KeepAlive
                                                                                       },

                                          RemoteCertificateValidator:            this.RemoteCertificateValidator is not null
                                                                                     ? (sender,
                                                                                        certificate,
                                                                                        certificateChain,
                                                                                        httpTestClient,
                                                                                        policyErrors) => this.RemoteCertificateValidator.Invoke(
                                                                                                             sender,
                                                                                                             certificate,
                                                                                                             certificateChain,
                                                                                                             this,
                                                                                                             policyErrors
                                                                                                         )
                                                                                     :  null,
                                          LocalCertificateSelector:              this.LocalCertificateSelector,
                                          ClientCertificates:                    this.ClientCertificates,
                                          ClientCertificateContext:              this.ClientCertificateContext,
                                          ClientCertificateChain:                this.ClientCertificateChain,
                                          TLSProtocols:                          this.TLSProtocols,
                                          CertificateRevocationCheckMode:        X509RevocationMode.NoCheck,
                                          ApplicationProtocols:                  null,
                                          AllowRenegotiation:                    null,
                                          AllowTLSResume:                        null,
                                          //TOTPConfig:                            TOTPConfig,

                                          MaxNumberOfClients:                    MaxNumberOfPooledClients ?? 6,

                                          PreferIPv4:                            this.PreferIPv4,
                                          ConnectTimeout:                        null,
                                          ReceiveTimeout:                        null,
                                          SendTimeout:                           null,
                                          BufferSize:                            null,

                                          DNSClient:                             this.DNSClient

                                      );

        }

        #endregion


        #region Dispose()

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion


    }

}
