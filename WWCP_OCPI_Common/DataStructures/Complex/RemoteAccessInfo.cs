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

using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.PKI;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// The remote access information.
    /// </summary>
    public sealed class RemoteAccessInfo : IEquatable<RemoteAccessInfo>,
                                           IComparable<RemoteAccessInfo>,
                                           IComparable
    {

        #region Properties

        /// <summary>
        /// The versions URL of the remote party.
        /// </summary>
        [Mandatory]
        public URL?                     VersionsURL                   { get; }

        /// <summary>
        /// The access token for accessing the remote party.
        /// Null, when remote data is publicly accessible as Open Data.
        /// </summary>
        [Optional]
        public AccessToken?             AccessToken                   { get; }

        /// <summary>
        /// Whether the access token is base64 encoded or not.
        /// </summary>
        [Optional]
        public Boolean?                 AccessTokenIsBase64Encoded    { get; }

        /// <summary>
        /// Configuration of the Time-Based One-Time Password (TOTP) 2nd factor authentication.
        /// </summary>
        public TOTPConfig?              TOTPConfig                    { get; }






        /// <summary>
        /// Prefer IPv4 instead of IPv6.
        /// </summary>
        public Boolean?                                                   PreferIPv4                    { get; }


        public UInt16?                                                    MaxNumberOfPooledClients      { get; }

        /// <summary>
        /// The remote TLS certificate validator.
        /// </summary>
        public RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator    { get; protected set; }

        /// <summary>
        /// A delegate to select a TLS client certificate.
        /// </summary>
        public LocalCertificateSelectionHandler?                          LocalCertificateSelector      { get; }

        /// <summary>
        /// The TLS client certificates with private key to use for HTTP authentication.
        /// </summary>
        public IEnumerable<X509Certificate2>                              ClientCertificates            { get; }

        public SslStreamCertificateContext?                               ClientCertificateContext      { get; }

        public IEnumerable<X509Certificate2>                              ClientCertificateChain        { get; }


        /// <summary>
        /// The TLS protocol to use.
        /// </summary>
        public SslProtocols?                                              TLSProtocols                  { get; }

        /// <summary>
        /// The optional HTTP accept header.
        /// </summary>
        public HTTPContentType?                                           ContentType                   { get; }

        /// <summary>
        /// The optional HTTP accept header.
        /// </summary>
        public AcceptTypes?                                               Accept                        { get; }

        /// <summary>
        /// The HTTP user agent identification.
        /// </summary>
        public String?                                                    HTTPUserAgent                 { get; }

        /// <summary>
        /// The timeout for upstream requests.
        /// </summary>
        public TimeSpan?                                                  RequestTimeout                { get; set; }


        public ConnectionType?                                            ConnectionType                { get; }


        /// <summary>
        /// The delay between transmission retries.
        /// </summary>
        public TransmissionRetryDelayDelegate?                            TransmissionRetryDelay        { get; }

        /// <summary>
        /// The maximum number of retries when communicating with the remote HTTP service.
        /// </summary>
        public UInt16?                                                    MaxNumberOfRetries            { get; }

        /// <summary>
        /// The size of the internal buffers of HTTP clients.
        /// </summary>
        public UInt32?                                                    InternalBufferSize            { get; }

        /// <summary>
        /// Whether to pipeline multiple HTTP request through a single HTTP/TCP connection.
        /// </summary>
        public Boolean?                                                   UseHTTPPipelining             { get; }




        /// <summary>
        /// The remote access status.
        /// </summary>
        [Mandatory]
        public RemoteAccessStatus       Status                        { get; }


        /// <summary>
        /// All available OCPI versions of the remote party.
        /// </summary>
        [Optional]
        public IEnumerable<Version_Id>  VersionIds                    { get; }

        /// <summary>
        /// The selected OCPI version at the remote party.
        /// </summary>
        [Optional]
        public Version_Id?              SelectedVersionId             { get; }

        /// <summary>
        /// This remote access information should not be used before this timestamp.
        /// </summary>
        [Optional]
        public DateTimeOffset?          NotBefore                     { get; }

        /// <summary>
        /// This remote access information should not be used after this timestamp.
        /// </summary>
        [Optional]
        public DateTimeOffset?          NotAfter                      { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI does not define any behaviour for this.
        /// </summary>
        [Mandatory]
        public Boolean                  AllowDowngrades               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new remote access information.
        /// </summary>
        /// <param name="VersionsURL">An OCPI versions URL.</param>
        /// <param name="AccessToken">A remote access token.</param>
        /// 
        /// <param name="Status">A remote access status.</param>
        /// <param name="TOTPConfig">An optional configuration of the Time-Based One-Time Password (TOTP) 2nd factor authentication.</param>
        /// <param name="VersionIds">An optional enumeration of version identifications.</param>
        /// <param name="SelectedVersionId">A optional selected version identification.</param>
        /// <param name="NotBefore">This remote access information should not be used before this timestamp.</param>
        /// <param name="NotAfter">This remote access information should not be used after this timestamp.</param>
        /// <param name="AccessTokenIsBase64Encoded">Whether the access token is base64 encoded or not.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        public RemoteAccessInfo(URL?                                                       VersionsURL                  = null,
                                AccessToken?                                               AccessToken                  = null,
                                Boolean?                                                   AccessTokenIsBase64Encoded   = null,
                                TOTPConfig?                                                TOTPConfig                   = null,

                                Boolean?                                                   PreferIPv4                   = null,
                                RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                IEnumerable<X509Certificate2>?                             ClientCertificates           = null,
                                SslStreamCertificateContext?                               ClientCertificateContext     = null,
                                IEnumerable<X509Certificate2>?                             ClientCertificateChain       = null,
                                SslProtocols?                                              TLSProtocols                 = null,

                                HTTPContentType?                                           ContentType                  = null,
                                AcceptTypes?                                               Accept                       = null,
                                String?                                                    HTTPUserAgent                = null,
                                TimeSpan?                                                  RequestTimeout               = null,
                                TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                UInt16?                                                    MaxNumberOfRetries           = null,
                                UInt32?                                                    InternalBufferSize           = null,
                                Boolean?                                                   UseHTTPPipelining            = null,

                                RemoteAccessStatus?                                        Status                       = RemoteAccessStatus.ONLINE,

                                IEnumerable<Version_Id>?                                   VersionIds                   = null,
                                Version_Id?                                                SelectedVersionId            = null,
                                DateTimeOffset?                                            NotBefore                    = null,
                                DateTimeOffset?                                            NotAfter                     = null,
                                Boolean?                                                   AllowDowngrades              = false)
        {

            this.VersionsURL                 = VersionsURL;
            this.AccessToken                 = AccessToken;
            this.AccessTokenIsBase64Encoded  = AccessTokenIsBase64Encoded;
            this.TOTPConfig                  = TOTPConfig;

            this.PreferIPv4                  = PreferIPv4;
            this.RemoteCertificateValidator  = RemoteCertificateValidator ?? ((sender,
                                                                               certificate,
                                                                               certificateChain,
                                                                               tlsClient,
                                                                               policyErrors) => {
                                                                                    return (false, [ $"The default behavior within {nameof(ARemoteParty)} is to reject all remote TLS server certificates!" ] );
                                                                               });
            this.LocalCertificateSelector    = LocalCertificateSelector;
            this.ClientCertificates          = ClientCertificates         ?? [];
            this.ClientCertificateContext    = ClientCertificateContext;
            this.ClientCertificateChain      = ClientCertificateChain     ?? [];
            this.TLSProtocols                = TLSProtocols;
            this.ContentType                 = ContentType;//                ?? HTTPContentType.Application.JSON_UTF8;
            this.Accept                      = Accept;//                     ?? AcceptTypes.FromHTTPContentTypes(HTTPContentType.Application.JSON_UTF8);
            this.HTTPUserAgent               = HTTPUserAgent;
            this.RequestTimeout              = RequestTimeout;
            this.TransmissionRetryDelay      = TransmissionRetryDelay;
            this.MaxNumberOfRetries          = MaxNumberOfRetries;
            this.InternalBufferSize          = InternalBufferSize;
            this.UseHTTPPipelining           = UseHTTPPipelining;

            this.Status                      = Status                     ?? (VersionsURL.HasValue
                                                                                  ? RemoteAccessStatus.ONLINE
                                                                                  : RemoteAccessStatus.PRE_REMOTE_REGISTRATION);

            this.VersionIds                  = VersionIds?.Distinct()     ?? [];
            this.SelectedVersionId           = SelectedVersionId;
            this.NotBefore                   = NotBefore;
            this.NotAfter                    = NotAfter;
            this.AllowDowngrades             = AllowDowngrades            ?? false;

            unchecked
            {

                this.hashCode = (this.VersionsURL?.               GetHashCode() ?? 0) * 29 ^
                                (this.AccessToken?.               GetHashCode() ?? 0) * 23 ^
                                (this.AccessTokenIsBase64Encoded?.GetHashCode() ?? 0) * 19 ^
                                (this.TOTPConfig?.                GetHashCode() ?? 0) * 17 ^

                                 this.Status.                     GetHashCode()       * 13 ^

                                 this.VersionIds.                 CalcHashCode()      * 11 ^
                                (this.SelectedVersionId?.         GetHashCode() ?? 0) *  7 ^
                                (this.NotBefore?.                 GetHashCode() ?? 0) *  5 ^
                                (this.NotAfter?.                  GetHashCode() ?? 0) *  3 ^
                                 this.AllowDowngrades.            GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomRemoteAccessInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of a remote access information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomRemoteAccessInfoParser">A delegate to parse custom remote access information JSON objects.</param>
        public static RemoteAccessInfo Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoParser   = null)
        {
            if (TryParse(JSON,
                         out var remoteAccessInfo,
                         out var errorResponse,
                         CustomRemoteAccessInfoParser))
            {
                return remoteAccessInfo;
            }

            throw new ArgumentException("The given JSON representation of a remote access information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out RemoteAccessInfo, out ErrorResponse, CustomRemoteAccessInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a remote access information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="RemoteAccessInfo">The parsed remote access information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out RemoteAccessInfo?  RemoteAccessInfo,
                                       [NotNullWhen(false)] out String?            ErrorResponse)

            => TryParse(JSON,
                        out RemoteAccessInfo,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a remote access information.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="RemoteAccessInfo">The parsed remote access information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRemoteAccessInfoParser">A delegate to parse custom remote access information JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out RemoteAccessInfo?      RemoteAccessInfo,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoParser   = null)
        {

            try
            {

                RemoteAccessInfo = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse VersionsURL                   [mandatory]

                if (!JSON.ParseMandatory("versionsURL",
                                         "versions URL",
                                         URL.TryParse,
                                         out URL VersionsURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AccessToken                   [optional]

                if (JSON.ParseOptional("accessToken",
                                       "access token",
                                       OCPI.AccessToken.TryParse,
                                       out AccessToken? AccessToken,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse AccessTokenIsBase64Encoded    [optional]

                if (JSON.ParseOptional("accessTokenIsBase64Encoded",
                                       "access token is base64 encoded",
                                       out Boolean? AccessTokenIsBase64Encoded,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TOTPConfig                    [optional]

                if (JSON.ParseOptionalJSON("totpConfig",
                                           "time-based one-time password configuration",
                                           TOTPConfig.TryParse,
                                           out TOTPConfig? totpConfig,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                // ...

                #region Parse ClientCertificates            [optional]

                var clientCertificates = new List<X509Certificate2>();

                if (JSON.ParseOptionalTexts("clientCertificates",
                                            "remote TLS client certificates with private keys (PEM)",
                                            out IEnumerable<String> clientCertificatesPEM,
                                            out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    try
                    {
                        foreach (var clientCertPEM in clientCertificatesPEM)
                        {
                            clientCertificates.Add(X509CertificateLoader.LoadCertificate(clientCertPEM.ToUTF8Bytes()));
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorResponse = "The given remote TLS client certificate chain is invalid: " + e.Message;
                        return false;
                    }

                }

                #endregion

                #region Parse ClientCertificateChain        [optional]

                var clientCertificateChain = new List<X509Certificate2>();

                if (JSON.ParseOptionalTexts("clientCertificateChain",
                                            "remote TLS client certificate chain (PEM)",
                                            out IEnumerable<String> clientCertificateChainPEM,
                                            out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    try
                    {
                        foreach (var certificatePEM in clientCertificateChainPEM)
                        {
                            clientCertificateChain.Add(X509CertificateLoader.LoadCertificate(certificatePEM.ToUTF8Bytes()));
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorResponse = "The given remote TLS client certificate chain is invalid: " + e.Message;
                        return false;
                    }

                }

                #endregion

                #region Parse HTTPUserAgent                 [optional]

                if (JSON.ParseOptionalText("httpUserAgent",
                                           "remote HTTP UserAgent",
                                           out String? httpUserAgent,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                // ...


                #region Parse Status                        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "remote access status",
                                         RemoteAccessStatusExtensions.TryParse,
                                         out RemoteAccessStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse SelectedVersionId             [optional]

                if (JSON.ParseOptional("selectedVersionId",
                                       "selected version identification",
                                       Version_Id.TryParse,
                                       out Version_Id? SelectedVersionId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NotBefore                     [optional]

                if (JSON.ParseOptional("notBefore",
                                       "not before",
                                       out DateTime? NotBefore,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NotAfter                      [optional]

                if (JSON.ParseOptional("notAfter",
                                       "not after",
                                       out DateTime? NotAfter,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse AllowDowngrades               [mandatory]

                if (!JSON.ParseMandatory("allowDowngrades",
                                         "allow downgrades",
                                         out Boolean AllowDowngrades,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                RemoteAccessInfo = new RemoteAccessInfo(

                                       VersionsURL,
                                       AccessToken,
                                       AccessTokenIsBase64Encoded,
                                       totpConfig,

                                       null, //preferIPv4,
                                       null, //remoteCertificateValidator,
                                       null, //LocalCertificateSelector,
                                       clientCertificates,
                                       null, //ClientCertificateContext,
                                       clientCertificateChain,
                                       null, //TLSProtocols,
                                       null, //ContentType,
                                       null, //Accept,
                                       httpUserAgent,
                                       null, //RequestTimeout,
                                       null, //TransmissionRetryDelay,
                                       null, //MaxNumberOfRetries,
                                       null, //InternalBufferSize,
                                       null, //UseHTTPPipelining,

                                       Status,

                                       null, // VersionIds
                                       SelectedVersionId,
                                       NotBefore,
                                       NotAfter,
                                       AllowDowngrades

                                   );


                if (CustomRemoteAccessInfoParser is not null)
                    RemoteAccessInfo = CustomRemoteAccessInfoParser(JSON,
                                                                    RemoteAccessInfo);

                return true;

            }
            catch (Exception e)
            {
                RemoteAccessInfo  = default;
                ErrorResponse     = "The given JSON representation of a remote access information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBusinessDetailsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoteAccessInfoSerializer">A delegate to serialize custom remote access information JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null)
        {

            var clientPrivateKeys = new List<AsymmetricAlgorithm>();

            foreach (var clientCertificate in ClientCertificates)
            {
                if (clientCertificate.HasPrivateKey)
                {

                    var privateRSAKey = clientCertificate.GetRSAPrivateKey();
                    if (privateRSAKey is not null)
                        clientPrivateKeys.Add(privateRSAKey);

                    var privateDSAKey = clientCertificate.GetDSAPrivateKey();
                    if (privateDSAKey is not null)
                        clientPrivateKeys.Add(privateDSAKey);

                }
            }

            var json = JSONObject.Create(

                           VersionsURL.HasValue
                               ? new JProperty("versionsURL",                  VersionsURL.      Value.ToString())
                               : null,

                           AccessToken.HasValue
                               ? new JProperty("accessToken",                  AccessToken.            ToString())
                               : null,

                           AccessTokenIsBase64Encoded.HasValue
                               ? new JProperty("accessTokenIsBase64Encoded",   AccessTokenIsBase64Encoded.Value)
                               : null,

                           TOTPConfig is not null
                               ? new JProperty("totpConfig",                   TOTPConfig.             ToJSON())
                               : null,


                           PreferIPv4.HasValue
                               ? new JProperty("preferIPv4",                   PreferIPv4.       Value)
                               : null,

                           ClientCertificates.Any()
                               ? new JProperty("clientCertificates",           new JArray(ClientCertificates.    Select(clientCertificate => clientCertificate.ExportCertificateAndPrivateKeyPEM())))
                               : null,

                           //clientPrivateKeys.Count > 0
                           //    ? new JProperty("clientPrivateKeys",            new JArray(clientPrivateKeys.     Select(clientPrivateKey  => clientPrivateKey. ExportPkcs8PrivateKeyPem())))
                           //    : null,

                           ClientCertificateChain.Any()
                               ? new JProperty("clientCertificateChain",       new JArray(ClientCertificateChain.Select(certificate       => certificate.      ExportCertificatePem())))
                               : null,

                           TLSProtocols.HasValue
                               ? new JProperty("tlsProtocols",                 TLSProtocols.     Value.ToString())
                               : null,

                           ContentType is not null
                               ? new JProperty("contentType",                  ContentType.            ToString())
                               : null,

                           Accept is not null
                               ? new JProperty("accept",                       Accept.                 ToString())
                               : null,

                           HTTPUserAgent.IsNotNullOrEmpty()
                               ? new JProperty("httpUserAgent",                HTTPUserAgent)
                               : null,

                           RequestTimeout.HasValue
                               ? new JProperty("requestTimeout",               (Int32) RequestTimeout.   Value.TotalMilliseconds)
                               : null,

                           MaxNumberOfRetries.HasValue
                               ? new JProperty("maxNumberOfRetries",           MaxNumberOfRetries.       Value)
                               : null,

                           // ...

                                 new JProperty("status",                       Status.           AsText()),


                           VersionIds.IsNeitherNullNorEmpty()
                               ? new JProperty("versionIds",                   new JArray(VersionIds.Select(versionId => versionId.ToString())))
                               : null,

                           SelectedVersionId.HasValue
                               ? new JProperty("selectedVersionId",            SelectedVersionId.ToString())
                               : null,

                           NotBefore.HasValue
                               ? new JProperty("notBefore",                    NotBefore.Value.ToISO8601())
                               : null,

                           NotAfter.HasValue
                               ? new JProperty("notAfter",                     NotAfter. Value.ToISO8601())
                               : null,

                                 new JProperty("allowDowngrades",              AllowDowngrades)

                       );

            return CustomRemoteAccessInfoSerializer is not null
                       ? CustomRemoteAccessInfoSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this remote access information.
        /// </summary>
        public RemoteAccessInfo Clone()

            => new (

                   VersionsURL?.Clone(),
                   AccessToken?.Clone(),
                   AccessTokenIsBase64Encoded,
                   TOTPConfig?. Clone(),

                   PreferIPv4,
                   RemoteCertificateValidator,
                   LocalCertificateSelector,
                   ClientCertificates,
                   ClientCertificateContext,
                   ClientCertificateChain,
                   TLSProtocols,
                   ContentType,
                   Accept,
                   HTTPUserAgent,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,
                   UseHTTPPipelining,

                   Status,

                   VersionIds.Select(versionId => versionId.Clone()),
                   SelectedVersionId,
                   NotBefore,
                   NotAfter,
                   AllowDowngrades

               );

        #endregion


        public void SetRemoteCertificateValidator(RemoteTLSServerCertificateValidationHandler<IHTTPClient> RemoteCertificateValidator)
        {
            this.RemoteCertificateValidator = RemoteCertificateValidator;
        }


        #region Operator overloading

        #region Operator == (RemoteAccessInfo1, RemoteAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteAccessInfo1">A remote access information.</param>
        /// <param name="RemoteAccessInfo2">Another remote access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RemoteAccessInfo RemoteAccessInfo1,
                                           RemoteAccessInfo RemoteAccessInfo2)
        {

            if (Object.ReferenceEquals(RemoteAccessInfo1, RemoteAccessInfo2))
                return true;

            if ((RemoteAccessInfo1 is null) || (RemoteAccessInfo2 is null))
                return false;

            return RemoteAccessInfo1.Equals(RemoteAccessInfo2);

        }

        #endregion

        #region Operator != (RemoteAccessInfo1, RemoteAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteAccessInfo1">A remote access information.</param>
        /// <param name="RemoteAccessInfo2">Another remote access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RemoteAccessInfo RemoteAccessInfo1,
                                           RemoteAccessInfo RemoteAccessInfo2)

            => !(RemoteAccessInfo1 == RemoteAccessInfo2);

        #endregion

        #region Operator <  (RemoteAccessInfo1, RemoteAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteAccessInfo1">A remote access information.</param>
        /// <param name="RemoteAccessInfo2">Another remote access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RemoteAccessInfo RemoteAccessInfo1,
                                          RemoteAccessInfo RemoteAccessInfo2)

            => RemoteAccessInfo1 is null
                   ? throw new ArgumentNullException(nameof(RemoteAccessInfo1), "The given remote access information must not be null!")
                   : RemoteAccessInfo1.CompareTo(RemoteAccessInfo2) < 0;

        #endregion

        #region Operator <= (RemoteAccessInfo1, RemoteAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteAccessInfo1">A remote access information.</param>
        /// <param name="RemoteAccessInfo2">Another remote access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RemoteAccessInfo RemoteAccessInfo1,
                                           RemoteAccessInfo RemoteAccessInfo2)

            => !(RemoteAccessInfo1 > RemoteAccessInfo2);

        #endregion

        #region Operator >  (RemoteAccessInfo1, RemoteAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteAccessInfo1">A remote access information.</param>
        /// <param name="RemoteAccessInfo2">Another remote access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RemoteAccessInfo RemoteAccessInfo1,
                                          RemoteAccessInfo RemoteAccessInfo2)

            => RemoteAccessInfo1 is null
                   ? throw new ArgumentNullException(nameof(RemoteAccessInfo1), "The given remote access information must not be null!")
                   : RemoteAccessInfo1.CompareTo(RemoteAccessInfo2) > 0;

        #endregion

        #region Operator >= (RemoteAccessInfo1, RemoteAccessInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteAccessInfo1">A remote access information.</param>
        /// <param name="RemoteAccessInfo2">Another remote access information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RemoteAccessInfo RemoteAccessInfo1,
                                           RemoteAccessInfo RemoteAccessInfo2)

            => !(RemoteAccessInfo1 < RemoteAccessInfo2);

        #endregion

        #endregion

        #region IComparable<RemoteAccessInfo> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two remote access information.
        /// </summary>
        /// <param name="Object">A remote access information to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RemoteAccessInfo remoteAccessInfo
                   ? CompareTo(remoteAccessInfo)
                   : throw new ArgumentException("The given object is not a remote access information!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RemoteAccessInfo)

        /// <summary>
        /// Compares two remote access information.
        /// </summary>
        /// <param name="RemoteAccessInfo">A remote access information to compare with.</param>
        public Int32 CompareTo(RemoteAccessInfo? RemoteAccessInfo)
        {

            if (RemoteAccessInfo is null)
                throw new ArgumentNullException(nameof(RemoteAccessInfo), "The given remote access information must not be null!");

            var c = 0;

            if (c == 0 && VersionsURL.               HasValue && RemoteAccessInfo.VersionsURL.               HasValue)
                c = VersionsURL.Value.               CompareTo(RemoteAccessInfo.VersionsURL.Value);

            if (c == 0 && AccessToken.               HasValue && RemoteAccessInfo.AccessToken.               HasValue)
                c = AccessToken.Value.               CompareTo(RemoteAccessInfo.AccessToken.Value);

            if (c == 0 && AccessTokenIsBase64Encoded.HasValue && RemoteAccessInfo.AccessTokenIsBase64Encoded.HasValue)
                c = AccessTokenIsBase64Encoded.Value.CompareTo(RemoteAccessInfo.AccessTokenIsBase64Encoded.Value);

            if (c == 0 && TOTPConfig    is not null && RemoteAccessInfo.TOTPConfig is not null)
                c = TOTPConfig.                      CompareTo(RemoteAccessInfo.TOTPConfig);


            if (c == 0)
                c = Status.                          CompareTo(RemoteAccessInfo.Status);


            if (c == 0)
                c = AllowDowngrades.                 CompareTo(RemoteAccessInfo.AllowDowngrades);

            if (c == 0 && NotBefore.HasValue && RemoteAccessInfo.NotBefore.HasValue)
                c = NotBefore.        Value.         CompareTo(RemoteAccessInfo.NotBefore.        Value);

            if (c == 0 && NotAfter. HasValue && RemoteAccessInfo.NotAfter. HasValue)
                c = NotAfter.         Value.         CompareTo(RemoteAccessInfo.NotAfter.         Value);

            if (c == 0 && SelectedVersionId.HasValue && RemoteAccessInfo.SelectedVersionId.HasValue)
                c = SelectedVersionId.Value.         CompareTo(RemoteAccessInfo.SelectedVersionId.Value);

            if (c == 0)
                c = VersionIds.Count().              CompareTo(RemoteAccessInfo.VersionIds.Count());

            if (c == 0)
                c = VersionIds.OrderBy(versionId => versionId.ToString()).AggregateWith(",").CompareTo(RemoteAccessInfo.VersionIds.OrderBy(versionId => versionId.ToString()).AggregateWith(","));

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<RemoteAccessInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two remote access information for equality.
        /// </summary>
        /// <param name="Object">A remote access information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoteAccessInfo remoteAccessInfo &&
                   Equals(remoteAccessInfo);

        #endregion

        #region Equals(RemoteAccessInfo)

        /// <summary>
        /// Compares two remote access information for equality.
        /// </summary>
        /// <param name="RemoteAccessInfo">A remote access information to compare with.</param>
        public Boolean Equals(RemoteAccessInfo? RemoteAccessInfo)

            => RemoteAccessInfo is not null &&

               VersionsURL.               Equals(RemoteAccessInfo.VersionsURL)                &&

            ((!AccessToken.               HasValue && !RemoteAccessInfo.AccessToken.               HasValue) ||
              (AccessToken.               HasValue &&  RemoteAccessInfo.AccessToken.               HasValue && AccessToken.               Value.Equals(RemoteAccessInfo.AccessToken.               Value))) &&

            ((!AccessTokenIsBase64Encoded.HasValue && !RemoteAccessInfo.AccessTokenIsBase64Encoded.HasValue) ||
              (AccessTokenIsBase64Encoded.HasValue &&  RemoteAccessInfo.AccessTokenIsBase64Encoded.HasValue && AccessTokenIsBase64Encoded.Value.Equals(RemoteAccessInfo.AccessTokenIsBase64Encoded.Value))) &&

               Status.                    Equals(RemoteAccessInfo.Status)                     &&

               AllowDowngrades.           Equals(RemoteAccessInfo.AllowDowngrades)            &&

            ((!NotBefore.                 HasValue && !RemoteAccessInfo.NotBefore.                 HasValue) ||
              (NotBefore.                 HasValue &&  RemoteAccessInfo.NotBefore.                 HasValue && NotBefore.                 Value.Equals(RemoteAccessInfo.NotBefore.                 Value))) &&

            ((!NotAfter.                  HasValue && !RemoteAccessInfo.NotAfter.                  HasValue) ||
              (NotAfter.                  HasValue &&  RemoteAccessInfo.NotAfter.                  HasValue && NotAfter.                  Value.Equals(RemoteAccessInfo.NotAfter.                  Value))) &&

            ((!SelectedVersionId.         HasValue && !RemoteAccessInfo.SelectedVersionId.         HasValue) ||
              (SelectedVersionId.         HasValue &&  RemoteAccessInfo.SelectedVersionId.         HasValue && SelectedVersionId.         Value.Equals(RemoteAccessInfo.SelectedVersionId.         Value))) &&

             ((TOTPConfig is null         &&  RemoteAccessInfo.TOTPConfig is null)         ||
              (TOTPConfig is not null     &&  RemoteAccessInfo.TOTPConfig is not null     && TOTPConfig.             Equals(RemoteAccessInfo.TOTPConfig             ))) &&

               VersionIds.Count().Equals(RemoteAccessInfo.VersionIds.Count()) &&
               VersionIds.All(RemoteAccessInfo.VersionIds.Contains);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{AccessToken}{(AccessTokenIsBase64Encoded.HasValue && AccessTokenIsBase64Encoded.Value ? " [base64]" : "")} @ {VersionsURL} {Status}";

        #endregion


    }

}
