/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    public delegate Boolean RemotePartyProviderDelegate(RemoteParty_Id RemotePartyId, out RemoteParty RemoteParty);

    public delegate JObject RemotePartyToJSONDelegate(RemoteParty                                         RemoteParty,
                                                      Boolean                                             Embedded                           = false,
                                                      CustomJObjectSerializerDelegate<RemoteParty>?       CustomRemotePartySerializer        = null,
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
                                                                                           CustomBusinessDetailsSerializer,
                                                                                           CustomImageSerializer,
                                                                                           CustomLocalAccessInfoSerializer,
                                                                                           CustomRemoteAccessInfoSerializer)

                                                                      : remoteParty.ToJSON(Embedded,
                                                                                           CustomRemotePartySerializer,
                                                                                           CustomBusinessDetailsSerializer,
                                                                                           CustomImageSerializer,
                                                                                           CustomLocalAccessInfoSerializer,
                                                                                           CustomRemoteAccessInfoSerializer)));

        #endregion

    }


    /// <summary>
    /// A remote party.
    /// In OCPI v2.1 this is a single CPO or EMSP.
    /// </summary>
    public class RemoteParty : IRemoteParty,
                               IEquatable<RemoteParty>,
                               IComparable<RemoteParty>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/remoteParty");

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this remote party
        /// (country code + party identification + role).
        /// </summary>
        [Mandatory]
        public RemoteParty_Id                        Id                            { get; }

        /// <summary>
        /// ISO-3166 alpha-2 country code of the country this party is operating in.
        /// </summary>
        [Mandatory]
        public CountryCode                           CountryCode                   { get; }

        /// <summary>
        /// CPO, eMSP (or other role) ID of this party (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id                              PartyId                       { get; }

        /// <summary>
        /// The type of the role.
        /// </summary>
        [Mandatory]
        public Roles                                 Role                          { get; }

        /// <summary>
        /// Business details of this party.
        /// </summary>
        [Mandatory]
        public BusinessDetails                       BusinessDetails               { get; }

        /// <summary>
        /// The current status of the party.
        /// </summary>
        [Mandatory]
        public PartyStatus                           Status                        { get; }

        /// <summary>
        /// Timestamp when this remote party was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                              LastUpdated                   { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this remote party.
        /// </summary>
        [Mandatory]
        public String                                ETag                          { get; private set; }



        /// <summary>
        /// The remote TLS certificate validator.
        /// </summary>
        public RemoteCertificateValidationHandler?   RemoteCertificateValidator    { get; }

        /// <summary>
        /// A delegate to select a TLS client certificate.
        /// </summary>
        public LocalCertificateSelectionHandler?     ClientCertificateSelector     { get; }

        /// <summary>
        /// The TLS client certificate to use of HTTP authentication.
        /// </summary>
        public X509Certificate?                      ClientCert                    { get; }

        /// <summary>
        /// The TLS protocol to use.
        /// </summary>
        public SslProtocols?                         TLSProtocol                   { get; }

        /// <summary>
        /// Prefer IPv4 instead of IPv6.
        /// </summary>
        public Boolean?                              PreferIPv4                    { get; }

        /// <summary>
        /// The HTTP user agent identification.
        /// </summary>
        public String?                               HTTPUserAgent                 { get; }

        /// <summary>
        /// The optional HTTP authentication to use.
        /// </summary>
        public IHTTPAuthentication?                  HTTPAuthentication            { get; }

        /// <summary>
        /// The timeout for upstream requests.
        /// </summary>
        public TimeSpan?                             RequestTimeout                { get; set; }

        /// <summary>
        /// The delay between transmission retries.
        /// </summary>
        public TransmissionRetryDelayDelegate?       TransmissionRetryDelay        { get; }

        /// <summary>
        /// The maximum number of retries when communicationg with the remote HTTP service.
        /// </summary>
        public UInt16?                               MaxNumberOfRetries            { get; }

        /// <summary>
        /// The size of the internal buffers of HTTP clients.
        /// </summary>
        public UInt32?                               InternalBufferSize            { get; }

        /// <summary>
        /// Whether to pipeline multiple HTTP request through a single HTTP/TCP connection.
        /// </summary>
        public Boolean?                              UseHTTPPipelining             { get; }





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

        public RemoteParty(CountryCode                          CountryCode,
                           Party_Id                             PartyId,
                           Roles                                Role,
                           BusinessDetails                      BusinessDetails,

                           AccessToken                          AccessToken,
                           Boolean?                             AccessTokenBase64Encoding    = null,
                           Boolean?                             AllowDowngrades              = false,
                           AccessStatus                         AccessStatus                 = AccessStatus.ALLOWED,
                           PartyStatus                          Status                       = PartyStatus. ENABLED,
                           DateTime?                            LocalAccessNotBefore         = null,
                           DateTime?                            LocalAccessNotAfter          = null,

                           Boolean?                             PreferIPv4                   = null,
                           RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
                           LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                           X509Certificate?                     ClientCert                   = null,
                           SslProtocols?                        TLSProtocol                  = null,
                           String?                              HTTPUserAgent                = null,
                           TimeSpan?                            RequestTimeout               = null,
                           TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                           UInt16?                              MaxNumberOfRetries           = null,
                           UInt32?                              InternalBufferSize           = null,
                           Boolean?                             UseHTTPPipelining            = null,

                           DateTime?                            LastUpdated                  = null)

            : this(CountryCode,
                   PartyId,
                   Role,
                   BusinessDetails,

                   new[] {
                       new LocalAccessInfo(
                           AccessToken,
                           AccessStatus,
                           LocalAccessNotBefore,
                           LocalAccessNotAfter,
                           AccessTokenBase64Encoding,
                           AllowDowngrades
                       )
                   },
                   Array.Empty<RemoteAccessInfo>(),
                   Status,

                   PreferIPv4,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
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

        public RemoteParty(CountryCode                          CountryCode,
                           Party_Id                             PartyId,
                           Roles                                Role,
                           BusinessDetails                      BusinessDetails,

                           AccessToken                          RemoteAccessToken,
                           URL                                  RemoteVersionsURL,
                           IEnumerable<Version_Id>?             RemoteVersionIds             = null,
                           Version_Id?                          SelectedVersionId            = null,
                           Boolean?                             AccessTokenBase64Encoding    = null,
                           Boolean?                             AllowDowngrades              = null,

                           RemoteAccessStatus?                  RemoteStatus                 = RemoteAccessStatus.ONLINE,
                           PartyStatus                          Status                       = PartyStatus.       ENABLED,
                           DateTime?                            RemoteAccessNotBefore        = null,
                           DateTime?                            RemoteAccessNotAfter         = null,

                           Boolean?                             PreferIPv4                   = null,
                           RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
                           LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                           X509Certificate?                     ClientCert                   = null,
                           SslProtocols?                        TLSProtocol                  = null,
                           String?                              HTTPUserAgent                = null,
                           TimeSpan?                            RequestTimeout               = null,
                           TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                           UInt16?                              MaxNumberOfRetries           = null,
                           UInt32?                              InternalBufferSize           = null,
                           Boolean?                             UseHTTPPipelining            = null,

                           DateTime?                            LastUpdated                  = null)

            : this(CountryCode,
                   PartyId,
                   Role,
                   BusinessDetails,

                   Array.Empty<LocalAccessInfo>(),
                   new[] {
                       new RemoteAccessInfo(
                           RemoteAccessToken,
                           RemoteVersionsURL,
                           RemoteVersionIds,
                           SelectedVersionId,
                           RemoteStatus,
                           RemoteAccessNotBefore,
                           RemoteAccessNotAfter,
                           AccessTokenBase64Encoding,
                           AllowDowngrades
                       )
                   },
                   Status,

                   PreferIPv4,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
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

        public RemoteParty(CountryCode                          CountryCode,
                           Party_Id                             PartyId,
                           Roles                                Role,
                           BusinessDetails                      BusinessDetails,

                           AccessToken                          AccessToken,

                           AccessToken                          RemoteAccessToken,
                           URL                                  RemoteVersionsURL,
                           IEnumerable<Version_Id>?             RemoteVersionIds             = null,
                           Version_Id?                          SelectedVersionId            = null,

                           DateTime?                            LocalAccessNotBefore         = null,
                           DateTime?                            LocalAccessNotAfter          = null,

                           Boolean?                             AccessTokenBase64Encoding    = null,
                           Boolean?                             AllowDowngrades              = false,
                           AccessStatus                         AccessStatus                 = AccessStatus.      ALLOWED,
                           RemoteAccessStatus?                  RemoteStatus                 = RemoteAccessStatus.ONLINE,
                           PartyStatus                          Status                       = PartyStatus.       ENABLED,
                           DateTime?                            RemoteAccessNotBefore        = null,
                           DateTime?                            RemoteAccessNotAfter         = null,

                           Boolean?                             PreferIPv4                   = null,
                           RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
                           LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                           X509Certificate?                     ClientCert                   = null,
                           SslProtocols?                        TLSProtocol                  = null,
                           String?                              HTTPUserAgent                = null,
                           TimeSpan?                            RequestTimeout               = null,
                           TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                           UInt16?                              MaxNumberOfRetries           = null,
                           UInt32?                              InternalBufferSize           = null,
                           Boolean?                             UseHTTPPipelining            = null,

                           DateTime?                            LastUpdated                  = null)

            : this(CountryCode,
                   PartyId,
                   Role,
                   BusinessDetails,

                   new[] {
                       new LocalAccessInfo(
                           AccessToken,
                           AccessStatus,
                           LocalAccessNotBefore,
                           LocalAccessNotAfter,
                           AccessTokenBase64Encoding,
                           AllowDowngrades
                       )
                   },
                   new[] {
                       new RemoteAccessInfo(
                           RemoteAccessToken,
                           RemoteVersionsURL,
                           RemoteVersionIds,
                           SelectedVersionId,
                           RemoteStatus,
                           RemoteAccessNotBefore,
                           RemoteAccessNotAfter,
                           AccessTokenBase64Encoding,
                           AllowDowngrades
                       )
                   },
                   Status,

                   PreferIPv4,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
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

        public RemoteParty(CountryCode                          CountryCode,
                           Party_Id                             PartyId,
                           Roles                                Role,
                           BusinessDetails                      BusinessDetails,

                           IEnumerable<LocalAccessInfo>         LocalAccessInfos,
                           IEnumerable<RemoteAccessInfo>        RemoteAccessInfos,
                           PartyStatus                          Status                       = PartyStatus.ENABLED,

                           Boolean?                             PreferIPv4                   = null,
                           RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
                           LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                           X509Certificate?                     ClientCert                   = null,
                           SslProtocols?                        TLSProtocol                  = null,
                           String?                              HTTPUserAgent                = null,
                           TimeSpan?                            RequestTimeout               = null,
                           TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                           UInt16?                              MaxNumberOfRetries           = null,
                           UInt32?                              InternalBufferSize           = null,
                           Boolean?                             UseHTTPPipelining            = null,

                           DateTime?                            LastUpdated                  = null)

        {

            this.Id                          = RemoteParty_Id.Parse(
                                                   String.Concat(CountryCode,
                                                                 "-",
                                                                 PartyId,
                                                                 "_",
                                                                 Role));

            this.CountryCode                 = CountryCode;
            this.PartyId                     = PartyId;
            this.Role                        = Role;
            this.BusinessDetails             = BusinessDetails;
            this.Status                      = Status;

            this.RemoteCertificateValidator  = RemoteCertificateValidator ?? ((sender, certificate, chain, sslPolicyErrors) => (true, Array.Empty<String>()));
            this.ClientCertificateSelector   = ClientCertificateSelector;
            this.ClientCert                  = ClientCert;
            this.TLSProtocol                 = TLSProtocol;
            this.PreferIPv4                  = PreferIPv4;
            this.HTTPUserAgent               = HTTPUserAgent;
            this.RequestTimeout              = RequestTimeout;
            this.TransmissionRetryDelay      = TransmissionRetryDelay;
            this.MaxNumberOfRetries          = MaxNumberOfRetries;
            this.InternalBufferSize          = InternalBufferSize;
            this.UseHTTPPipelining           = UseHTTPPipelining;

            this.LastUpdated                 = LastUpdated ?? Timestamp.Now;

            this.localAccessInfos            = LocalAccessInfos. IsNeitherNullNorEmpty() ? new List<LocalAccessInfo> (LocalAccessInfos)  : new List<LocalAccessInfo>();
            this.remoteAccessInfos           = RemoteAccessInfos.IsNeitherNullNorEmpty() ? new List<RemoteAccessInfo>(RemoteAccessInfos) : new List<RemoteAccessInfo>();

            this.ETag                        = CalcSHA256Hash();

            unchecked
            {

                this.hashCode = Id.               GetHashCode()  * 29 ^
                                CountryCode.      GetHashCode()  * 23 ^
                                PartyId.          GetHashCode()  * 19 ^
                                Role.             GetHashCode()  * 17 ^
                                BusinessDetails.  GetHashCode()  * 13 ^
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
                return remoteParty!;
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
                                       out RemoteParty?                           RemoteParty,
                                       out String?                                ErrorResponse,
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

                #region Parse CountryCode          [mandatory]

                if (!JSON.ParseMandatory("countryCode",
                                         "country code",
                                         OCPI.CountryCode.TryParse,
                                         out CountryCode CountryCode,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PartyId              [mandatory]

                if (!JSON.ParseMandatory("partyId",
                                         "party identification",
                                         Party_Id.TryParse,
                                         out Party_Id PartyId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Role                 [mandatory]

                if (!JSON.ParseMandatory("role",
                                         "party role",
                                         RolesExtensions.TryParse,
                                         out Roles Role,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse BusinessDetails      [mandatory]

                if (!JSON.ParseMandatoryJSON("businessDetails",
                                             "business details",
                                             OCPI.BusinessDetails.TryParse,
                                             out BusinessDetails? BusinessDetails,
                                             out ErrorResponse) ||
                    BusinessDetails is null)
                {
                    return false;
                }

                #endregion


                #region Parse LocalAccessInfos     [optional]

                if (JSON.ParseOptionalJSON("localAccessInfos",
                                           "local access infos",
                                           LocalAccessInfo.TryParse,
                                           out IEnumerable<LocalAccessInfo> LocalAccessInfos,
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
                                           out IEnumerable<RemoteAccessInfo> RemoteAccessInfos,
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
                                         out PartyStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                // ...

                #region Parse LastUpdated          [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                RemoteParty = new RemoteParty(CountryCode,
                                              PartyId,
                                              Role,
                                              BusinessDetails,

                                              LocalAccessInfos,
                                              RemoteAccessInfos,

                                              Status,
                                              LastUpdated: LastUpdated);


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

        #region ToJSON(Embedded, CustomRemotePartySerializer = null, CustomBusinessDetailsSerializer = null, ...)

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
                              CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer    = null,
                              CustomJObjectSerializerDelegate<Image>?             CustomImageSerializer              = null,
                              CustomJObjectSerializerDelegate<LocalAccessInfo>?   CustomLocalAccessInfoSerializer    = null,
                              CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("@id",                 Id.                  ToString()),

                           Embedded
                               ? new JProperty("@context",            DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("countryCode",         CountryCode.         ToString()),
                                 new JProperty("partyId",             PartyId.             ToString()),
                                 new JProperty("role",                Role.                ToString()),
                                 new JProperty("partyStatus",         Status.              ToString()),

                           BusinessDetails is not null
                               ? new JProperty("businessDetails",     BusinessDetails.     ToJSON(CustomBusinessDetailsSerializer,
                                                                                                  CustomImageSerializer))
                               : null,

                           localAccessInfos.Any()
                               ? new JProperty("localAccessInfos",    new JArray(localAccessInfos. Select(localAccessInfo  => localAccessInfo. ToJSON(CustomLocalAccessInfoSerializer))))
                               : null,

                           remoteAccessInfos.Any()
                               ? new JProperty("remoteAccessInfos",   new JArray(remoteAccessInfos.Select(remoteAccessInfo => remoteAccessInfo.ToJSON(CustomRemoteAccessInfoSerializer))))
                               : null,

                                 new JProperty("last_updated",        LastUpdated.         ToIso8601())

                       );

            return CustomRemotePartySerializer is not null
                       ? CustomRemotePartySerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public RemoteParty Clone()

            => new (CountryCode. Clone,
                    PartyId.     Clone,
                    Role,
                    BusinessDetails.Clone(),

                    LocalAccessInfos. Select(localAccesssInfo  => localAccesssInfo. Clone()),
                    RemoteAccessInfos.Select(remoteAccessInfos => remoteAccessInfos.Clone()),
                    Status,

                    PreferIPv4,
                    RemoteCertificateValidator,
                    ClientCertificateSelector,
                    ClientCert,
                    TLSProtocol,
                    HTTPUserAgent,
                    RequestTimeout,
                    TransmissionRetryDelay,
                    MaxNumberOfRetries,
                    InternalBufferSize,
                    UseHTTPPipelining,

                    LastUpdated);

        #endregion


        #region CalcSHA256Hash(CustomRemotePartySerializer = null, CustomBusinessDetailsSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this remote party in HEX.
        /// </summary>
        /// <param name="CustomRemotePartySerializer">A delegate to serialize custom remote party JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomLocalAccessInfoSerializer">A delegate to serialize custom local access information JSON objects.</param>
        /// <param name="CustomRemoteAccessInfoSerializer">A delegate to serialize custom remote access information JSON objects.</param>
        public String CalcSHA256Hash(CustomJObjectSerializerDelegate<RemoteParty>?       CustomRemotePartySerializer        = null,
                                     CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer    = null,
                                     CustomJObjectSerializerDelegate<Image>?             CustomImageSerializer              = null,
                                     CustomJObjectSerializerDelegate<LocalAccessInfo>?   CustomLocalAccessInfoSerializer    = null,
                                     CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer   = null)
        {

            this.ETag = SHA256.HashData(ToJSON(false, // always with @context!
                                               CustomRemotePartySerializer,
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

            var c = Id.             CompareTo(RemoteParty.Id);

            if (c == 0)
                c = CountryCode.    CompareTo(RemoteParty.CountryCode);

            if (c == 0)
                c = PartyId.        CompareTo(RemoteParty.PartyId);

            if (c == 0)
                c = Role.           CompareTo(RemoteParty.Role);

            if (c == 0)
                c = BusinessDetails.CompareTo(RemoteParty.BusinessDetails);

            if (c == 0)
                c = Status.         CompareTo(RemoteParty.Status);

            if (c == 0)
                c = LastUpdated.    CompareTo(RemoteParty.LastUpdated);

            if (c == 0)
                c = ETag.           CompareTo(RemoteParty.ETag);

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

               Id.             Equals(RemoteParty.Id)              &&
               Status.         Equals(RemoteParty.Status)          &&

               CountryCode.    Equals(RemoteParty.CountryCode)     &&
               PartyId.        Equals(RemoteParty.PartyId)         &&
               Role.           Equals(RemoteParty.Role)            &&
               BusinessDetails.Equals(RemoteParty.BusinessDetails) &&

               LastUpdated.    Equals(RemoteParty.LastUpdated)     &&
               ETag.           Equals(RemoteParty.ETag)            &&

               localAccessInfos.Count.Equals(RemoteParty.localAccessInfos.Count)   &&
               localAccessInfos. All(RemoteParty.localAccessInfos. Contains)       &&

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
