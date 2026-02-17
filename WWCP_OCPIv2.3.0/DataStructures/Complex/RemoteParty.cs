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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0.CPO.HTTP;
using cloud.charging.open.protocols.OCPIv2_3_0.EMSP.HTTP;

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

                   ? []

                   : new JArray(
                         RemoteParties.
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
                                                                                    CustomRemoteAccessInfoSerializer))
                     );

        #endregion

    }


    /// <summary>
    /// A remote party serving multiple CPOs and/or EMSPs.
    /// </summary>
    public class RemoteParty : ARemotePartyV2x,
                               IEquatable<RemoteParty>,
                               IComparable<RemoteParty>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse($"https://open.charging.cloud/contexts/OCPI/remoteParty/{Version.String}");

        #endregion

        #region Properties

        /// <summary>
        /// An optional persistent CPO to EMSP client.
        /// </summary>
        public CPO2EMSP_HTTPClient?  CPO2EMSPClient    { get; set; }

        /// <summary>
        /// An optional persistent EMSP to CPO client.
        /// </summary>
        public EMSP2CPOClient?  EMSP2CPOClient    { get; set; }

        /// <summary>
        /// Optional incoming request and response modifiers.
        /// </summary>
        public IOModifiers?     IN                { get; set; }

        /// <summary>
        /// Optional outgoing request and response modifiers.
        /// </summary>
        public IOModifiers?     OUT               { get; set; }

        #endregion

        #region Constructor(s)

        #region RemoteParty (Id, Roles, LocalAccessToken,                                       ..., Status = PRE_REMOTE_REGISTRATION, ...)

        /// <summary>
        /// Create a new Remote Party with local access only.
        /// The remote party will start the OCPI registration process afterwards!
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Roles"></param>
        /// 
        /// <param name="LocalAccessToken"></param>
        /// <param name="LocalAccessTokenBase64Encoding"></param>
        /// <param name="LocalTOTPConfig"></param>
        /// <param name="LocalAccessNotBefore"></param>
        /// <param name="LocalAccessNotAfter"></param>
        /// <param name="LocalAllowDowngrades"></param>
        /// <param name="LocalAccessStatus"></param>
        /// 
        /// <param name="RemoteAccessTokenBase64Encoding"></param>
        /// <param name="RemoteTOTPConfig"></param>
        /// <param name="RemoteAccessNotBefore"></param>
        /// <param name="RemoteAccessNotAfter"></param>
        /// <param name="PreferIPv4"></param>
        /// <param name="RemoteCertificateValidator"></param>
        /// <param name="LocalCertificateSelector"></param>
        /// <param name="ClientCertificates"></param>
        /// <param name="ClientCertificateContext"></param>
        /// <param name="ClientCertificateChain"></param>
        /// <param name="TLSProtocols"></param>
        /// <param name="ContentType"></param>
        /// <param name="Accept"></param>
        /// <param name="HTTPUserAgent"></param>
        /// <param name="RequestTimeout"></param>
        /// <param name="TransmissionRetryDelay"></param>
        /// <param name="MaxNumberOfRetries"></param>
        /// <param name="InternalBufferSize"></param>
        /// <param name="UseHTTPPipelining"></param>
        /// <param name="RemoteStatus"></param>
        /// <param name="RemoteAllowDowngrades"></param>
        /// 
        /// <param name="Status"></param>
        /// 
        /// <param name="Created"></param>
        /// <param name="LastUpdated"></param>
        public RemoteParty(RemoteParty_Id                                             Id,
                           IEnumerable<CredentialsRole>                               Roles,

                           AccessToken                                                LocalAccessToken,
                           Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                           TOTPConfig?                                                LocalTOTPConfig                   = null,
                           DateTimeOffset?                                            LocalAccessNotBefore              = null,
                           DateTimeOffset?                                            LocalAccessNotAfter               = null,
                           Boolean?                                                   LocalAllowDowngrades              = null,
                           AccessStatus?                                              LocalAccessStatus                 = null,

                           Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                           TOTPConfig?                                                RemoteTOTPConfig                  = null,
                           DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                           DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                           Boolean?                                                   PreferIPv4                        = null,
                           RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                           LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                           SslStreamCertificateContext?                               ClientCertificateContext          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                           SslProtocols?                                              TLSProtocols                      = null,
                           HTTPContentType?                                           ContentType                       = null,
                           AcceptTypes?                                               Accept                            = null,
                           String?                                                    HTTPUserAgent                     = null,
                           TimeSpan?                                                  RequestTimeout                    = null,
                           TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                           UInt16?                                                    MaxNumberOfRetries                = null,
                           UInt32?                                                    InternalBufferSize                = null,
                           Boolean?                                                   UseHTTPPipelining                 = null,
                           RemoteAccessStatus?                                        RemoteStatus                      = null,
                           Boolean?                                                   RemoteAllowDowngrades             = null,

                           PartyStatus?                                               Status                            = null,

                           DateTimeOffset?                                            Created                           = null,
                           DateTimeOffset?                                            LastUpdated                       = null)

            : this(Id,
                   Roles,

                   [
                       new LocalAccessInfo(
                           LocalAccessToken,
                           LocalAccessStatus,
                           LocalTOTPConfig,
                           LocalAccessNotBefore,
                           LocalAccessNotAfter,
                           LocalAccessTokenBase64Encoding,
                           LocalAllowDowngrades
                       )
                   ],
                   (RemoteAccessTokenBase64Encoding is not null ||
                    RemoteTOTPConfig                is not null ||
                    RemoteAccessNotBefore           is not null ||
                    RemoteAccessNotAfter            is not null ||
                    PreferIPv4                      is not null ||
                    RemoteCertificateValidator      is not null ||
                    LocalCertificateSelector        is not null ||
                    ClientCertificates              is not null ||
                    ClientCertificateContext        is not null ||
                    ClientCertificateChain          is not null ||
                    TLSProtocols                    is not null ||
                    ContentType                     is not null ||
                    Accept                          is not null ||
                    HTTPUserAgent                   is not null ||
                    RequestTimeout                  is not null ||
                    TransmissionRetryDelay          is not null ||
                    MaxNumberOfRetries              is not null ||
                    InternalBufferSize              is not null ||
                    UseHTTPPipelining               is not null ||
                    RemoteStatus                    is not null ||
                    RemoteAllowDowngrades           is not null)
                        ? [
                              new RemoteAccessInfo(

                                  RemoteStatus,

                                  null,  // We do not know the RemoteVersionsURL yet!
                                  null,  // We do not know the RemoteAccessToken yet!
                                  RemoteAccessTokenBase64Encoding,
                                  RemoteTOTPConfig,

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

                                  null,  // We do not know the RemoteVersionIds yet!
                                  null,  // We do not know the SelectedVersionId yet!
                                  RemoteAccessNotBefore,
                                  RemoteAccessNotAfter,
                                  RemoteAllowDowngrades

                              )
                          ]
                        : [],
                   Status ?? PartyStatus.PRE_REMOTE_REGISTRATION,

                   Created,
                   LastUpdated)

        { }

        #endregion

        #region RemoteParty (Id, Roles,                   RemoteVersionsURL, RemoteAccessToken, ..., Status = PRE_LOCAL_REGISTRATION,  ...)

        /// <summary>
        /// Create a new Remote Party with remote access only.
        /// _WE_ will start the OCPI registration process afterwards!
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Roles"></param>
        /// 
        /// <param name="RemoteVersionsURL"></param>
        /// <param name="RemoteAccessToken"></param>
        /// <param name="RemoteAccessTokenBase64Encoding"></param>
        /// <param name="RemoteTOTPConfig"></param>
        /// 
        /// <param name="PreferIPv4"></param>
        /// <param name="RemoteCertificateValidator"></param>
        /// <param name="LocalCertificateSelector"></param>
        /// <param name="ClientCertificates"></param>
        /// <param name="ClientCertificateContext"></param>
        /// <param name="ClientCertificateChain"></param>
        /// <param name="TLSProtocols"></param>
        /// <param name="ContentType"></param>
        /// <param name="Accept"></param>
        /// <param name="HTTPUserAgent"></param>
        /// <param name="RequestTimeout"></param>
        /// <param name="TransmissionRetryDelay"></param>
        /// <param name="MaxNumberOfRetries"></param>
        /// <param name="InternalBufferSize"></param>
        /// <param name="UseHTTPPipelining"></param>
        /// 
        /// <param name="RemoteStatus"></param>
        /// 
        /// <param name="RemoteVersionIds"></param>
        /// <param name="SelectedVersionId"></param>
        /// <param name="RemoteAccessNotBefore"></param>
        /// <param name="RemoteAccessNotAfter"></param>
        /// <param name="RemoteAllowDowngrades"></param>
        /// 
        /// <param name="Status"></param>
        /// 
        /// <param name="Created"></param>
        /// <param name="LastUpdated"></param>
        public RemoteParty(RemoteParty_Id                                             Id,
                           IEnumerable<CredentialsRole>                               Roles,

                           URL                                                        RemoteVersionsURL,
                           AccessToken?                                               RemoteAccessToken                 = null,
                           Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                           TOTPConfig?                                                RemoteTOTPConfig                  = null,

                           Boolean?                                                   PreferIPv4                        = null,
                           RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                           LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                           SslStreamCertificateContext?                               ClientCertificateContext          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                           SslProtocols?                                              TLSProtocols                      = null,
                           HTTPContentType?                                           ContentType                       = null,
                           AcceptTypes?                                               Accept                            = null,
                           String?                                                    HTTPUserAgent                     = null,
                           TimeSpan?                                                  RequestTimeout                    = null,
                           TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                           UInt16?                                                    MaxNumberOfRetries                = null,
                           UInt32?                                                    InternalBufferSize                = null,
                           Boolean?                                                   UseHTTPPipelining                 = null,

                           RemoteAccessStatus?                                        RemoteStatus                      = null,

                           IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                           Version_Id?                                                SelectedVersionId                 = null,
                           DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                           DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                           Boolean?                                                   RemoteAllowDowngrades             = null,

                           PartyStatus?                                               Status                            = null,

                           DateTimeOffset?                                            Created                           = null,
                           DateTimeOffset?                                            LastUpdated                       = null)

            : this(Id,
                   Roles,

                   [],
                   [
                       new RemoteAccessInfo(

                           RemoteStatus,

                           RemoteVersionsURL,
                           RemoteAccessToken,
                           RemoteAccessTokenBase64Encoding,
                           RemoteTOTPConfig,

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

                           RemoteVersionIds,
                           SelectedVersionId,
                           RemoteAccessNotBefore,
                           RemoteAccessNotAfter,
                           RemoteAllowDowngrades

                       )
                   ],
                   Status ?? PartyStatus.PRE_LOCAL_REGISTRATION,

                   Created,
                   LastUpdated)

        { }

        #endregion

        #region RemoteParty (Id, Roles, LocalAccessToken, RemoteVersionsURL, RemoteAccessToken, ..., Status = ENABLED, ...)

        /// <summary>
        /// Create a new Remote Party with a manual configuration of local and remote access.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Roles"></param>
        /// 
        /// <param name="LocalAccessToken"></param>
        /// 
        /// <param name="RemoteVersionsURL"></param>
        /// <param name="RemoteAccessToken"></param>
        /// <param name="RemoteAccessTokenBase64Encoding"></param>
        /// <param name="RemoteTOTPConfig"></param>
        /// 
        /// <param name="PreferIPv4"></param>
        /// <param name="RemoteCertificateValidator"></param>
        /// <param name="LocalCertificateSelector"></param>
        /// <param name="ClientCertificates"></param>
        /// <param name="ClientCertificateContext"></param>
        /// <param name="ClientCertificateChain"></param>
        /// <param name="TLSProtocols"></param>
        /// <param name="ContentType"></param>
        /// <param name="Accept"></param>
        /// <param name="HTTPUserAgent"></param>
        /// <param name="RequestTimeout"></param>
        /// <param name="TransmissionRetryDelay"></param>
        /// <param name="MaxNumberOfRetries"></param>
        /// <param name="InternalBufferSize"></param>
        /// <param name="UseHTTPPipelining"></param>
        /// <param name="RemoteStatus"></param>
        /// <param name="RemoteVersionIds"></param>
        /// <param name="SelectedVersionId"></param>
        /// <param name="RemoteAccessNotBefore"></param>
        /// <param name="RemoteAccessNotAfter"></param>
        /// <param name="RemoteAllowDowngrades"></param>
        /// 
        /// <param name="LocalAccessTokenBase64Encoding"></param>
        /// <param name="LocalTOTPConfig"></param>
        /// <param name="LocalAccessNotBefore"></param>
        /// <param name="LocalAccessNotAfter"></param>
        /// <param name="LocalAllowDowngrades"></param>
        /// <param name="LocalAccessStatus"></param>
        /// 
        /// <param name="Status"></param>
        /// <param name="Created"></param>
        /// <param name="LastUpdated"></param>
        public RemoteParty(RemoteParty_Id                                             Id,
                           IEnumerable<CredentialsRole>                               Roles,

                           AccessToken                                                LocalAccessToken,

                           URL                                                        RemoteVersionsURL,
                           AccessToken                                                RemoteAccessToken,
                           Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                           TOTPConfig?                                                RemoteTOTPConfig                  = null,

                           Boolean?                                                   PreferIPv4                        = null,
                           RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                           LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                           SslStreamCertificateContext?                               ClientCertificateContext          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                           SslProtocols?                                              TLSProtocols                      = null,
                           HTTPContentType?                                           ContentType                       = null,
                           AcceptTypes?                                               Accept                            = null,
                           String?                                                    HTTPUserAgent                     = null,
                           TimeSpan?                                                  RequestTimeout                    = null,
                           TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                           UInt16?                                                    MaxNumberOfRetries                = null,
                           UInt32?                                                    InternalBufferSize                = null,
                           Boolean?                                                   UseHTTPPipelining                 = null,
                           RemoteAccessStatus?                                        RemoteStatus                      = null,
                           IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                           Version_Id?                                                SelectedVersionId                 = null,
                           DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                           DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                           Boolean?                                                   RemoteAllowDowngrades             = null,

                           Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                           TOTPConfig?                                                LocalTOTPConfig                   = null,
                           DateTimeOffset?                                            LocalAccessNotBefore              = null,
                           DateTimeOffset?                                            LocalAccessNotAfter               = null,
                           Boolean?                                                   LocalAllowDowngrades              = null,
                           AccessStatus?                                              LocalAccessStatus                 = null,

                           PartyStatus?                                               Status                            = null,

                           DateTimeOffset?                                            Created                           = null,
                           DateTimeOffset?                                            LastUpdated                       = null)

            : this(Id,
                   Roles,

                   [
                       new LocalAccessInfo(
                           LocalAccessToken,
                           LocalAccessStatus,
                           LocalTOTPConfig,
                           LocalAccessNotBefore,
                           LocalAccessNotAfter,
                           LocalAccessTokenBase64Encoding,
                           LocalAllowDowngrades
                       )
                   ],
                   [
                       new RemoteAccessInfo(

                           RemoteStatus,

                           RemoteVersionsURL,
                           RemoteAccessToken,
                           RemoteAccessTokenBase64Encoding,
                           RemoteTOTPConfig,

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

                           RemoteVersionIds,
                           SelectedVersionId,
                           RemoteAccessNotBefore,
                           RemoteAccessNotAfter,
                           RemoteAllowDowngrades

                       )
                   ],
                   Status ?? PartyStatus.ENABLED,

                   Created,
                   LastUpdated)

        { }

        #endregion

        #region RemoteParty (Id, Roles, LocalAccessInfos,                    RemoteAccessInfos, ..., Status = ENABLED, ...)

        public RemoteParty(RemoteParty_Id                 Id,
                           IEnumerable<CredentialsRole>   Roles,

                           IEnumerable<LocalAccessInfo>   LocalAccessInfos,
                           IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                           PartyStatus?                   Status        = null,

                           DateTimeOffset?                Created       = null,
                           DateTimeOffset?                LastUpdated   = null)

            : base(Id,

                   Roles,
                   LocalAccessInfos,
                   RemoteAccessInfos,
                   Status ?? PartyStatus.ENABLED,

                   Created,
                   LastUpdated)

        {

            this.ETag  = CalcSHA256Hash();

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


                #region Parse Created              [optional, NonStandard]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTimeOffset? created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated          [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTimeOffset lastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                RemoteParty = new RemoteParty(

                                  id,
                                  roles,

                                  localAccessInfos,
                                  remoteAccessInfos,

                                  status,

                                  created,
                                  lastUpdated

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

        #region ToJSON(Embedded, CustomRemotePartySerializer = null, CustomCredentialsRoleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="CustomRemotePartySerializer">A delegate to serialize custom remote party JSON objects.</param>
        /// <param name="CustomCredentialsRoleSerializer">A delegate to serialize custom credentials roles JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        /// <param name="CustomLocalAccessInfoSerializer">A delegate to serialize custom local access information JSON objects.</param>
        /// <param name="CustomRemoteAccessInfoSerializer">A delegate to serialize custom remote access information JSON objects.</param>
        /// <param name="CustomTOTPConfigSerializerDelegate">A delegate to serialize custom TOTP configuration JSON objects.</param>
        public JObject ToJSON(Boolean                                             Embedded,
                              CustomJObjectSerializerDelegate<RemoteParty>?       CustomRemotePartySerializer          = null,
                              CustomJObjectSerializerDelegate<CredentialsRole>?   CustomCredentialsRoleSerializer      = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer      = null,
                              CustomJObjectSerializerDelegate<Image>?             CustomImageSerializer                = null,
                              CustomJObjectSerializerDelegate<LocalAccessInfo>?   CustomLocalAccessInfoSerializer      = null,
                              CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer     = null,
                              CustomJObjectSerializerDelegate<TOTPConfig>?        CustomTOTPConfigSerializerDelegate   = null)
        {

            var json = base.ToJSON(

                           Embedded
                               ? DefaultJSONLDContext
                               : null,

                           CustomCredentialsRoleSerializer,
                           CustomBusinessDetailsSerializer,
                           CustomImageSerializer,
                           CustomLocalAccessInfoSerializer,
                           CustomRemoteAccessInfoSerializer,
                           CustomTOTPConfigSerializerDelegate

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

            => new (

                   Id.Clone(),
                   Roles.            Select(credentialsRole   => credentialsRole.  Clone()),

                   LocalAccessInfos. Select(accessInfoStatus  => accessInfoStatus. Clone()),
                   RemoteAccessInfos.Select(remoteAccessInfos => remoteAccessInfos.Clone()),

                   Status,

                   Created,
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
        public new Int32 CompareTo(Object? Object)

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

            var c = Id.    CompareTo(RemoteParty.Id);

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

    }

}
