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

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    public delegate Boolean RemotePartyProviderDelegate(RemoteParty_Id RemotePartyId, out RemoteParty RemoteParty);

    public delegate JObject RemotePartyToJSONDelegate(RemoteParty                                         RemoteParty,
                                                      Boolean                                             Embedded                           = false,
                                                      CustomJObjectSerializerDelegate<RemoteParty>?       CustomRemotePartySerializer        = null,
                                                      CustomJObjectSerializerDelegate<CredentialsRole>?   CustomCredentialsRoleSerializer    = null,
                                                      CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer    = null,
                                                      CustomJObjectSerializerDelegate<Image>?             CustomImageSerializer              = null,
                                                      CustomJObjectSerializerDelegate<LocalAccessInfo>?   CustomLocalAccessInfoSerializer    = null,
                                                      CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null);


    /// <summary>
    /// Extension methods for remote parties.
    /// </summary>
    public static partial class RemotePartyExtensions
    {

        #region ToJSON(this RemoteParties, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of remote parties.
        /// </summary>
        /// <param name="RemoteParties">An enumeration of remote parties.</param>
        /// <param name="Skip">The optional number of remote parties to skip.</param>
        /// <param name="Take">The optional number of remote parties to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a remote party.</param>
        public static JArray ToJSON(this IEnumerable<RemoteParty>                       RemoteParties,
                                    UInt64?                                             Skip                               = null,
                                    UInt64?                                             Take                               = null,
                                    Boolean                                             Embedded                           = false,
                                    CustomJObjectSerializerDelegate<RemoteParty>?       CustomRemotePartySerializer        = null,
                                    CustomJObjectSerializerDelegate<CredentialsRole>?   CustomCredentialsRoleSerializer    = null,
                                    CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer    = null,
                                    CustomJObjectSerializerDelegate<Image>?             CustomImageSerializer              = null,
                                    CustomJObjectSerializerDelegate<LocalAccessInfo>?   CustomLocalAccessInfoSerializer    = null,
                                    CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null,
                                    RemotePartyToJSONDelegate?                          RemotePartyToJSON                  = null)


            => RemoteParties?.Any() != true

                   ? new JArray()

                   : new JArray(RemoteParties.
                                    Where         (remoteParty => remoteParty is not null).
                                    OrderBy       (remoteParty => remoteParty.Id).
                                    SkipTakeFilter(Skip, Take).
                                    Select        (remoteParty => RemotePartyToJSON is not null
                                                                      ? RemotePartyToJSON (remoteParty,
                                                                                           Embedded,
                                                                                           CustomRemotePartySerializer,
                                                                                           CustomCredentialsRoleSerializer,
                                                                                           CustomBusinessDetailsSerializer,
                                                                                           CustomImageSerializer,
                                                                                           CustomLocalAccessInfoSerializer,
                                                                                           CustomRemoteAccessInfoSerializer)

                                                                      : remoteParty.ToJSON(Embedded,
                                                                                           CustomRemotePartySerializer,
                                                                                           CustomCredentialsRoleSerializer,
                                                                                           CustomBusinessDetailsSerializer,
                                                                                           CustomImageSerializer,
                                                                                           CustomLocalAccessInfoSerializer,
                                                                                           CustomRemoteAccessInfoSerializer)));

        #endregion

    }


    /// <summary>
    /// A remote party serving multiple CPOs and/or EMSPs.
    /// </summary>
    public class RemoteParty : IRemoteParty,
                               IEquatable<RemoteParty>,
                               IComparable<RemoteParty>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPIv2.3/remoteParty");

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this remote party
        /// (country code + party identification + role).
        /// </summary>
        [Mandatory]
        public RemoteParty_Id                       Id                            { get; }

        /// <summary>
        /// The current status of the party.
        /// </summary>
        [Mandatory]
        public PartyStatus                          Status                        { get; }

        /// <summary>
        /// The enumeration of credential roles.
        /// </summary>
        [Mandatory]
        public IEnumerable<CredentialsRole>         Roles                         { get; }

        /// <summary>
        /// The timestamp when this remote party was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public DateTimeOffset                       Created                       { get; }

        /// <summary>
        /// Timestamp when this remote party was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTimeOffset                       LastUpdated                   { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this remote party.
        /// </summary>
        [Mandatory]
        public String                               ETag                          { get; private set; }



        /// <summary>
        /// The remote TLS certificate validator.
        /// </summary>
        public RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator    { get; }

        /// <summary>
        /// A delegate to select a TLS client certificate.
        /// </summary>
        public LocalCertificateSelectionHandler?    LocalCertificateSelector     { get; }

        /// <summary>
        /// The TLS client certificate to use of HTTP authentication.
        /// </summary>
        public X509Certificate?                     ClientCert                    { get; }

        /// <summary>
        /// The TLS protocol to use.
        /// </summary>
        public SslProtocols?                        TLSProtocol                   { get; }

        /// <summary>
        /// Prefer IPv4 instead of IPv6.
        /// </summary>
        public Boolean?                             PreferIPv4                    { get; }

        /// <summary>
        /// The optional HTTP accept header.
        /// </summary>
        public AcceptTypes?                         Accept                        { get; }

        /// <summary>
        /// The optional HTTP authentication to use.
        /// </summary>
        public IHTTPAuthentication?                 HTTPAuthentication            { get; }

        /// <summary>
        /// The HTTP user agent identification.
        /// </summary>
        public String?                              HTTPUserAgent                 { get; }

        /// <summary>
        /// The timeout for upstream requests.
        /// </summary>
        public TimeSpan?                            RequestTimeout                { get; set; }

        /// <summary>
        /// The delay between transmission retries.
        /// </summary>
        public TransmissionRetryDelayDelegate?      TransmissionRetryDelay        { get; }

        /// <summary>
        /// The maximum number of retries when communicating with the remote HTTP service.
        /// </summary>
        public UInt16?                              MaxNumberOfRetries            { get; }

        /// <summary>
        /// The size of the internal buffers of HTTP clients.
        /// </summary>
        public UInt32?                              InternalBufferSize            { get; }

        /// <summary>
        /// Whether to pipeline multiple HTTP request through a single HTTP/TCP connection.
        /// </summary>
        public Boolean?                             UseHTTPPipelining             { get; }



        private readonly List<LocalAccessInfo> localAccessInfos;

        /// <summary>
        /// Local access information.
        /// </summary>
        public IEnumerable<LocalAccessInfo> LocalAccessInfos
            => localAccessInfos;



        private readonly List<RemoteAccessInfo> remoteAccessInfos;

        /// <summary>
        /// Remote access information.
        /// </summary>
        public IEnumerable<RemoteAccessInfo> RemoteAccessInfos
            => remoteAccessInfos;

        #endregion

        #region Constructor(s)

        #region RemoteParty(..., AccessToken, AccessStatus = ALLOWED, ...)

        public RemoteParty(RemoteParty_Id                                             Id,
                           IEnumerable<CredentialsRole>                               Roles,

                           AccessToken                                                AccessToken,
                           Boolean?                                                   AccessTokenBase64Encoding    = null,
                           Boolean?                                                   AllowDowngrades              = false,
                           AccessStatus?                                              AccessStatus                 = AccessStatus.ALLOWED,
                           PartyStatus?                                               Status                       = PartyStatus.ENABLED,

                           TimeSpan?                                                  LocalTOTP_ValidityTime       = null,
                           UInt32?                                                    LocalTOTP_Length             = null,
                           String?                                                    LocalTOTP_Alphabet           = null,
                           DateTimeOffset?                                            LocalAccessNotBefore         = null,
                           DateTimeOffset?                                            LocalAccessNotAfter          = null,

                           Boolean?                                                   PreferIPv4                   = null,
                           RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                           LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                           X509Certificate?                                           ClientCert                   = null,
                           SslProtocols?                                              TLSProtocol                  = null,
                           String?                                                    HTTPUserAgent                = null,
                           TimeSpan?                                                  RequestTimeout               = null,
                           TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                           UInt16?                                                    MaxNumberOfRetries           = null,
                           UInt32?                                                    InternalBufferSize           = null,
                           Boolean?                                                   UseHTTPPipelining            = null,

                           DateTimeOffset?                                            LastUpdated                  = null)

            : this(Id,
                   Roles,

                   [
                       new LocalAccessInfo(
                           AccessToken,
                           AccessStatus ?? OCPI.AccessStatus.ALLOWED,
                           LocalTOTP_ValidityTime,
                           LocalTOTP_Length,
                           LocalTOTP_Alphabet,
                           LocalAccessNotBefore,
                           LocalAccessNotAfter,
                           AccessTokenBase64Encoding,
                           AllowDowngrades
                       )
                   ],
                   [],
                   Status ?? PartyStatus.ENABLED,

                   PreferIPv4,
                   RemoteCertificateValidator,
                   LocalCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   HTTPUserAgent,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,
                   UseHTTPPipelining,

                   LastUpdated)

        { }

        #endregion

        #region RemoteParty(..., RemoteAccessToken, RemoteVersionsURL, ...)

        public RemoteParty(RemoteParty_Id                                             Id,
                           IEnumerable<CredentialsRole>                               Roles,

                           AccessToken                                                RemoteAccessToken,
                           URL                                                        RemoteVersionsURL,

                           TimeSpan?                                                  TOTP_ValidityTime            = null,
                           UInt32?                                                    TOTP_Length                  = null,
                           String?                                                    TOTP_Alphabet                = null,

                           IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                           Version_Id?                                                SelectedVersionId            = null,
                           Boolean?                                                   AccessTokenBase64Encoding    = null,
                           Boolean?                                                   AllowDowngrades              = null,

                           RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                           PartyStatus?                                               Status                       = PartyStatus.ENABLED,
                           DateTimeOffset?                                            RemoteAccessNotBefore        = null,
                           DateTimeOffset?                                            RemoteAccessNotAfter         = null,

                           Boolean?                                                   PreferIPv4                   = null,
                           RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                           LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                           X509Certificate?                                           ClientCert                   = null,
                           SslProtocols?                                              TLSProtocol                  = null,
                           String?                                                    HTTPUserAgent                = null,
                           TimeSpan?                                                  RequestTimeout               = null,
                           TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                           UInt16?                                                    MaxNumberOfRetries           = null,
                           UInt32?                                                    InternalBufferSize           = null,
                           Boolean?                                                   UseHTTPPipelining            = null,

                           DateTimeOffset?                                            LastUpdated                  = null)

            : this(Id,
                   Roles,

                   [],
                   [
                       new RemoteAccessInfo(
                           RemoteAccessToken,
                           RemoteVersionsURL,
                           TOTP_ValidityTime,
                           TOTP_Length,
                           TOTP_Alphabet,
                           RemoteVersionIds,
                           SelectedVersionId,
                           RemoteStatus,
                           RemoteAccessNotBefore,
                           RemoteAccessNotAfter,
                           AccessTokenBase64Encoding,
                           AllowDowngrades
                       )
                   ],
                   Status ?? PartyStatus.ENABLED,

                   PreferIPv4,
                   RemoteCertificateValidator,
                   LocalCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   HTTPUserAgent,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,
                   UseHTTPPipelining,

                   LastUpdated)

        { }

        #endregion

        #region RemoteParty(...)

        public RemoteParty(RemoteParty_Id                                             Id,
                           IEnumerable<CredentialsRole>                               Roles,

                           AccessToken                                                AccessToken,

                           AccessToken                                                RemoteAccessToken,
                           URL                                                        RemoteVersionsURL,

                           TimeSpan?                                                  RemoteTOTP_ValidityTime      = null,
                           UInt32?                                                    RemoteTOTP_Length            = null,
                           String?                                                    RemoteTOTP_Alphabet          = null,

                           IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                           Version_Id?                                                SelectedVersionId            = null,

                           TimeSpan?                                                  LocalTOTP_ValidityTime       = null,
                           UInt32?                                                    LocalTOTP_Length             = null,
                           String?                                                    LocalTOTP_Alphabet           = null,
                           DateTimeOffset?                                            LocalAccessNotBefore         = null,
                           DateTimeOffset?                                            LocalAccessNotAfter          = null,

                           Boolean?                                                   AccessTokenBase64Encoding    = null,
                           Boolean?                                                   AllowDowngrades              = false,
                           AccessStatus?                                              AccessStatus                 = AccessStatus.ALLOWED,
                           RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                           PartyStatus?                                               Status                       = PartyStatus.ENABLED,
                           DateTimeOffset?                                            RemoteAccessNotBefore        = null,
                           DateTimeOffset?                                            RemoteAccessNotAfter         = null,

                           Boolean?                                                   PreferIPv4                   = null,
                           RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                           LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                           X509Certificate?                                           ClientCert                   = null,
                           SslProtocols?                                              TLSProtocol                  = null,
                           String?                                                    HTTPUserAgent                = null,
                           TimeSpan?                                                  RequestTimeout               = null,
                           TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                           UInt16?                                                    MaxNumberOfRetries           = null,
                           UInt32?                                                    InternalBufferSize           = null,
                           Boolean?                                                   UseHTTPPipelining            = null,

                           DateTimeOffset?                                            LastUpdated                  = null)

            : this(Id,
                   Roles,

                   [
                       new LocalAccessInfo(
                           AccessToken,
                           AccessStatus ?? OCPI.AccessStatus.ALLOWED,
                           LocalTOTP_ValidityTime,
                           LocalTOTP_Length,
                           LocalTOTP_Alphabet,
                           LocalAccessNotBefore,
                           LocalAccessNotAfter,
                           AccessTokenBase64Encoding,
                           AllowDowngrades
                       )
                   ],
                   [
                       new RemoteAccessInfo(
                           RemoteAccessToken,
                           RemoteVersionsURL,
                           RemoteTOTP_ValidityTime,
                           RemoteTOTP_Length,
                           RemoteTOTP_Alphabet,
                           RemoteVersionIds,
                           SelectedVersionId,
                           RemoteStatus,
                           RemoteAccessNotBefore,
                           RemoteAccessNotAfter,
                           AccessTokenBase64Encoding,
                           AllowDowngrades
                       )
                   ],
                   Status ?? PartyStatus.ENABLED,

                   PreferIPv4,
                   RemoteCertificateValidator,
                   LocalCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   HTTPUserAgent,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,
                   UseHTTPPipelining,

                   LastUpdated)

        { }

        #endregion

        #region RemoteParty(...)

        public RemoteParty(RemoteParty_Id                                             Id,
                           IEnumerable<CredentialsRole>                               Roles,

                           IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                           IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                           PartyStatus?                                               Status                       = PartyStatus.ENABLED,

                           Boolean?                                                   PreferIPv4                   = null,
                           RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                           LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                           X509Certificate?                                           ClientCert                   = null,
                           SslProtocols?                                              TLSProtocol                  = null,
                           String?                                                    HTTPUserAgent                = null,
                           TimeSpan?                                                  RequestTimeout               = null,
                           TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                           UInt16?                                                    MaxNumberOfRetries           = null,
                           UInt32?                                                    InternalBufferSize           = null,
                           Boolean?                                                   UseHTTPPipelining            = null,

                           DateTimeOffset?                                            Created                      = null,
                           DateTimeOffset?                                            LastUpdated                  = null)

        {

            this.Id                          = Id;
            this.Roles                       = Roles;
            this.Status                      = Status                     ?? PartyStatus.ENABLED;

            this.PreferIPv4                  = PreferIPv4;
            this.RemoteCertificateValidator  = RemoteCertificateValidator ?? ((sender,
                                                                               certificate,
                                                                               certificateChain,
                                                                               tlsClient,
                                                                               policyErrors) => (true, Array.Empty<String>()));
            this.LocalCertificateSelector    = LocalCertificateSelector;
            this.ClientCert                  = ClientCert;
            this.TLSProtocol                 = TLSProtocol;
            this.HTTPUserAgent               = HTTPUserAgent;
            this.RequestTimeout              = RequestTimeout;
            this.TransmissionRetryDelay      = TransmissionRetryDelay;
            this.MaxNumberOfRetries          = MaxNumberOfRetries;
            this.InternalBufferSize          = InternalBufferSize;
            this.UseHTTPPipelining           = UseHTTPPipelining;

            this.Created                     = Created     ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated                 = LastUpdated ?? Created     ?? Timestamp.Now;

            this.localAccessInfos            = LocalAccessInfos. IsNeitherNullNorEmpty() ? [.. LocalAccessInfos]  : [];
            this.remoteAccessInfos           = RemoteAccessInfos.IsNeitherNullNorEmpty() ? [.. RemoteAccessInfos] : [];

            this.ETag                        = CalcSHA256Hash();

            unchecked
            {

                this.hashCode = Id.               GetHashCode()  * 17 ^
                                Roles.            CalcHashCode() * 13 ^
                                Status.           GetHashCode()  * 11 ^
                                LastUpdated.      GetHashCode()  *  7 ^
                                ETag.             GetHashCode()  *  5 ^
                                localAccessInfos. CalcHashCode() *  3 ^
                                remoteAccessInfos.CalcHashCode();

            }

        }

        #endregion

        #endregion


        #region (static) Parse   (JSON, CustomRemotePartyParser = null)

        /// <summary>
        /// Parse the given JSON representation of a remote party.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomRemotePartyParser">A delegate to parse custom remote party JSON objects.</param>
        public static RemoteParty Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<RemoteParty>?  CustomRemotePartyParser   = null)
        {

            if (TryParse(JSON,
                         out var remoteParty,
                         out var errorResponse,
                         CustomRemotePartyParser))
            {
                return remoteParty;
            }

            throw new ArgumentException("The given JSON representation of a remote party is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out RemoteParty, out ErrorResponse, CustomRemotePartyParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a remote party.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="RemoteParty">The parsed remote party.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRemotePartyParser">A delegate to parse custom remote party JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out RemoteParty?      RemoteParty,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<RemoteParty>?  CustomRemotePartyParser   = null)
        {

            try
            {

                RemoteParty = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                   [mandatory]

                if (!JSON.ParseMandatory("@id",
                                         "remote party identification",
                                         RemoteParty_Id.TryParse,
                                         out RemoteParty_Id id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Roles                [optional]

                if (!JSON.ParseMandatoryJSON("roles",
                                             "credentials roles",
                                             CredentialsRole.TryParse,
                                             out IEnumerable<CredentialsRole> roles,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse LocalAccessInfos     [optional]

                if (JSON.ParseOptionalJSON("localAccessInfos",
                                           "local access infos",
                                           LocalAccessInfo.TryParse,
                                           out IEnumerable<LocalAccessInfo> localAccessInfos,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse RemoteAccessInfos    [optional]

                if (JSON.ParseOptionalJSON("remoteAccessInfos",
                                           "remote access infos",
                                           RemoteAccessInfo.TryParse,
                                           out IEnumerable<RemoteAccessInfo> remoteAccessInfos,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Status               [mandatory]

                if (!JSON.ParseMandatory("partyStatus",
                                         "party status",
                                         PartyStatus.TryParse,
                                         out PartyStatus status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                // ...

                #region Parse Created              [optional, NonStandard]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTime? created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated          [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime lastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                RemoteParty = new RemoteParty(

                                  Id:                           id,
                                  Roles:                        roles,

                                  LocalAccessInfos:             localAccessInfos,
                                  RemoteAccessInfos:            remoteAccessInfos,

                                  Status:                       status,

                                  PreferIPv4:                   null,
                                  RemoteCertificateValidator:   null,
                                  LocalCertificateSelector:     null,
                                  ClientCert:                   null,
                                  TLSProtocol:                  null,
                                  HTTPUserAgent:                null,
                                  RequestTimeout:               null,
                                  TransmissionRetryDelay:       null,
                                  MaxNumberOfRetries:           null,
                                  InternalBufferSize:           null,
                                  UseHTTPPipelining:            null,

                                  LastUpdated:                  lastUpdated

                              );


                if (CustomRemotePartyParser is not null)
                    RemoteParty = CustomRemotePartyParser(JSON,
                                                          RemoteParty);

                return true;

            }
            catch (Exception e)
            {
                RemoteParty    = default;
                ErrorResponse  = "The given JSON representation of a remote party is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(Embedded, CustomRemotePartySerializer = null, CustomBusinessDetailsSerializer = null, IncludeCryptoHash = false)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="CustomRemotePartySerializer">A delegate to serialize custom remote party JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomLocalAccessInfoSerializer">A delegate to serialize custom local access information JSON objects.</param>
        /// <param name="CustomRemoteAccessInfoSerializer">A delegate to serialize custom remote access information JSON objects.</param>
        public JObject ToJSON(Boolean                                             Embedded,
                              CustomJObjectSerializerDelegate<RemoteParty>?       CustomRemotePartySerializer        = null,
                              CustomJObjectSerializerDelegate<CredentialsRole>?   CustomCredentialsRoleSerializer    = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer    = null,
                              CustomJObjectSerializerDelegate<Image>?             CustomImageSerializer              = null,
                              CustomJObjectSerializerDelegate<LocalAccessInfo>?   CustomLocalAccessInfoSerializer    = null,
                              CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("@id",                Id.ToString()),

                           Embedded
                               ? new JProperty("@context",           DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("partyStatus",        Status.ToString()),

                                 new JProperty("roles",              new JArray(Roles.Select(role => role.ToJSON(CustomCredentialsRoleSerializer,
                                                                                                                 CustomBusinessDetailsSerializer,
                                                                                                                 CustomImageSerializer)))),

                           localAccessInfos. Count != 0
                               ? new JProperty("accessInfos",        new JArray(localAccessInfos.Select(localAccessInfo => localAccessInfo.ToJSON(CustomLocalAccessInfoSerializer))))
                               : null,

                           remoteAccessInfos.Count != 0
                               ? new JProperty("remoteAccessInfos",  new JArray(remoteAccessInfos.Select(remoteAccessInfo => remoteAccessInfo.ToJSON(CustomRemoteAccessInfoSerializer))))
                               : null,

                                 new JProperty("last_updated",       LastUpdated.ToISO8601())

                       );

            return CustomRemotePartySerializer is not null
                       ? CustomRemotePartySerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this remote party.
        /// </summary>
        public RemoteParty Clone()

            => new(

                   Id.Clone(),
                   Roles.            Select(credentialsRole   => credentialsRole.  Clone()),
                   LocalAccessInfos. Select(accessInfoStatus  => accessInfoStatus. Clone()),
                   RemoteAccessInfos.Select(remoteAccessInfos => remoteAccessInfos.Clone()),
                   Status,

                   PreferIPv4,
                   RemoteCertificateValidator,
                   LocalCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   HTTPUserAgent,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,
                   UseHTTPPipelining,

                   LastUpdated

               );

        #endregion


        #region CalcSHA256Hash(CustomRemotePartySerializer = null, CustomCredentialsRoleSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this remote party in HEX.
        /// </summary>
        /// <param name="CustomRemotePartySerializer">A delegate to serialize custom remote party JSON objects.</param>
        /// <param name="CustomCredentialsRoleSerializer">A delegate to serialize custom credentials roles JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomLocalAccessInfoSerializer">A delegate to serialize custom local access information JSON objects.</param>
        /// <param name="CustomRemoteAccessInfoSerializer">A delegate to serialize custom remote access information JSON objects.</param>
        public String CalcSHA256Hash(CustomJObjectSerializerDelegate<RemoteParty>?       CustomRemotePartySerializer        = null,
                                     CustomJObjectSerializerDelegate<CredentialsRole>?   CustomCredentialsRoleSerializer    = null,
                                     CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer    = null,
                                     CustomJObjectSerializerDelegate<Image>?             CustomImageSerializer              = null,
                                     CustomJObjectSerializerDelegate<LocalAccessInfo>?   CustomLocalAccessInfoSerializer    = null,
                                     CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null)
        {

            this.ETag = SHA256.HashData(ToJSON(false, // always with @context!
                                               CustomRemotePartySerializer,
                                               CustomCredentialsRoleSerializer,
                                               CustomBusinessDetailsSerializer,
                                               CustomImageSerializer,
                                               CustomLocalAccessInfoSerializer,
                                               CustomRemoteAccessInfoSerializer).ToUTF8Bytes()).ToBase64();

            return this.ETag;

        }

        #endregion


        #region Operator overloading

        #region Operator == (RemoteParty1, RemoteParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteParty1">A remote party identification.</param>
        /// <param name="RemoteParty2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RemoteParty RemoteParty1,
                                           RemoteParty RemoteParty2)
        {

            if (Object.ReferenceEquals(RemoteParty1, RemoteParty2))
                return true;

            if ((RemoteParty1 is null) || (RemoteParty2 is null))
                return false;

            return RemoteParty1.Equals(RemoteParty2);

        }

        #endregion

        #region Operator != (RemoteParty1, RemoteParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteParty1">A remote party identification.</param>
        /// <param name="RemoteParty2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RemoteParty RemoteParty1,
                                           RemoteParty RemoteParty2)

            => !(RemoteParty1 == RemoteParty2);

        #endregion

        #region Operator <  (RemoteParty1, RemoteParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteParty1">A remote party identification.</param>
        /// <param name="RemoteParty2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RemoteParty RemoteParty1,
                                          RemoteParty RemoteParty2)

            => RemoteParty1 is null
                   ? throw new ArgumentNullException(nameof(RemoteParty1), "The given remote party must not be null!")
                   : RemoteParty1.CompareTo(RemoteParty2) < 0;

        #endregion

        #region Operator <= (RemoteParty1, RemoteParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteParty1">A remote party identification.</param>
        /// <param name="RemoteParty2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RemoteParty RemoteParty1,
                                           RemoteParty RemoteParty2)

            => !(RemoteParty1 > RemoteParty2);

        #endregion

        #region Operator >  (RemoteParty1, RemoteParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteParty1">A remote party identification.</param>
        /// <param name="RemoteParty2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RemoteParty RemoteParty1,
                                          RemoteParty RemoteParty2)

            => RemoteParty1 is null
                   ? throw new ArgumentNullException(nameof(RemoteParty1), "The given remote party must not be null!")
                   : RemoteParty1.CompareTo(RemoteParty2) > 0;

        #endregion

        #region Operator >= (RemoteParty1, RemoteParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteParty1">A remote party identification.</param>
        /// <param name="RemoteParty2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RemoteParty RemoteParty1,
                                           RemoteParty RemoteParty2)

            => !(RemoteParty1 < RemoteParty2);

        #endregion

        #endregion

        #region IComparable<RemoteParty> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two remote parties.
        /// </summary>
        /// <param name="Object">A remote party to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RemoteParty remoteParty
                   ? CompareTo(remoteParty)
                   : throw new ArgumentException("The given object is not a remote party!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RemoteParty)

        /// <summary>
        /// Compares two remote parties.
        /// </summary>
        /// <param name="RemoteParty">A remote party to compare with.</param>
        public Int32 CompareTo(RemoteParty? RemoteParty)
        {

            if (RemoteParty is null)
                throw new ArgumentNullException(nameof(RemoteParty), "The given remote party must not be null!");

            var c = Id.CompareTo(RemoteParty.Id);

            if (c == 0)
                c = Status.CompareTo(RemoteParty.Status);

            if (c == 0)
                c = Roles.Count().CompareTo(RemoteParty.Roles.Count());

            if (c == 0)
            {
                for (var i = 0; i < Roles.Count(); i++)
                {

                    c = Roles.ElementAt(i).CompareTo(RemoteParty.Roles.ElementAt(i));

                    if (c != 0)
                        break;

                }
            }

            if (c == 0)
                c = LastUpdated.CompareTo(RemoteParty.LastUpdated);

            if (c == 0)
                c = ETag.CompareTo(RemoteParty.ETag);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<RemoteParty> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two remote parties for equality.
        /// </summary>
        /// <param name="Object">A remote party to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoteParty remoteParty &&
                   Equals(remoteParty);

        #endregion

        #region Equals(RemoteParty)

        /// <summary>
        /// Compares two remote parties for equality.
        /// </summary>
        /// <param name="RemoteParty">A remote party to compare with.</param>
        public Boolean Equals(RemoteParty? RemoteParty)

            => RemoteParty is not null &&

               Id.Equals(RemoteParty.Id) &&
               Status.Equals(RemoteParty.Status) &&

               Roles.Count().Equals(RemoteParty.Roles.Count()) &&
               Roles.All(RemoteParty.Roles.Contains) &&

               LastUpdated.Equals(RemoteParty.LastUpdated) &&
               ETag.Equals(RemoteParty.ETag) &&

               localAccessInfos.Count.Equals(RemoteParty.localAccessInfos.Count) &&
               localAccessInfos.All(RemoteParty.localAccessInfos.Contains) &&

               remoteAccessInfos.Count.Equals(RemoteParty.remoteAccessInfos.Count) &&
               remoteAccessInfos.All(RemoteParty.remoteAccessInfos.Contains);

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
            => Id.ToString();

        #endregion

    }

}
