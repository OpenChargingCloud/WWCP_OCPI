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

using System.Text;
using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Illias.Logging;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;
using System.Diagnostics.CodeAnalysis;
using org.GraphDefined.Vanaheimr.Hermod;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    public delegate String OCPILogfileCreatorDelegate(String         LoggingPath,
                                                      IRemoteParty?  RemoteParty,
                                                      String         Context,
                                                      String         LogfileName);


    /// <summary>
    /// The OCPI Common HTTP API.
    /// </summary>
    public class CommonHTTPAPI : AHTTPExtAPIXExtension<HTTPExtAPIX>
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String         DefaultHTTPServerName           = "GraphDefined OCPI Common HTTP API";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String         DefaultHTTPServiceName          = "GraphDefined OCPI Common HTTP API";

        ///// <summary>
        ///// The default HTTP server TCP port.  
        ///// </summary>
        //public new static readonly IPPort         DefaultHTTPServerPort           = IPPort.Parse(8080);

        ///// <summary>
        ///// The default HTTP URL path prefix.  
        ///// </summary>
        //public new static readonly HTTPPath       DefaultURLPathPrefix            = HTTPPath.Parse("io/OCPI/");

        /// <summary>
        /// The default database file name for all remote party configuration.
        /// </summary>
        public const               String         DefaultRemotePartyDBFileName    = "RemoteParties.db";

        /// <summary>
        /// The default database file name for all OCPI assets.
        /// </summary>
        public const               String         DefaultAssetsDBFileName         = "Assets.db";

        private readonly           LogFileWriter  logFileWriter                   = new (10000);

        #region Commands

        public const String addRemoteParty                     = "addRemoteParty";
        public const String addRemotePartyIfNotExists          = "addRemotePartyIfNotExists";
        public const String addOrUpdateRemoteParty             = "addOrUpdateRemoteParty";
        public const String updateRemoteParty                  = "updateRemoteParty";
        public const String removeRemoteParty                  = "removeRemoteParty";
        public const String removeAllRemoteParties             = "removeAllRemoteParties";

        public const String addLocation                        = "addLocation";
        public const String addLocationIfNotExists             = "addLocationIfNotExists";
        public const String addOrUpdateLocation                = "addOrUpdateLocation";
        public const String updateLocation                     = "updateLocation";
        public const String removeLocation                     = "removeLocation";
        public const String removeAllLocations                 = "removeAllLocations";

        public const String addEVSE                            = "addEVSE";
        public const String addEVSEIfNotExists                 = "addEVSEIfNotExists";
        public const String addOrUpdateEVSE                    = "addOrUpdateEVSE";
        public const String updateEVSE                         = "updateEVSE";
        public const String removeEVSE                         = "removeEVSE";
        public const String removeAllEVSEs                     = "removeAllEVSEs";

        public const String addTariff                          = "addTariff";
        public const String addTariffIfNotExists               = "addTariffIfNotExists";
        public const String addOrUpdateTariff                  = "addOrUpdateTariff";
        public const String updateTariff                       = "updateTariff";
        public const String removeTariff                       = "removeTariff";
        public const String removeAllTariffs                   = "removeAllTariffs";

        public const String addSession                         = "addSession";
        public const String addSessionIfNotExists              = "addSessionIfNotExists";
        public const String addOrUpdateSession                 = "addOrUpdateSession";
        public const String updateSession                      = "updateSession";
        public const String removeSession                      = "removeSession";
        public const String removeAllSessions                  = "removeAllSessions";

        public const String addToken                           = "addToken";
        public const String addTokenIfNotExists                = "addTokenIfNotExists";
        public const String addOrUpdateToken                   = "addOrUpdateToken";
        public const String updateToken                        = "updateToken";
        public const String removeToken                        = "removeToken";
        public const String removeAllTokens                    = "removeAllTokens";

        public const String addTokenStatus                     = "addTokenStatus";
        public const String addTokenStatusIfNotExists          = "addTokenStatusIfNotExists";
        public const String addOrUpdateTokenStatus             = "addOrUpdateTokenStatus";
        public const String updateTokenStatus                  = "updateTokenStatus";
        public const String removeTokenStatus                  = "removeTokenStatus";
        public const String removeAllTokenStatus               = "removeAllTokenStatus";

        public const String addChargeDetailRecord              = "addChargeDetailRecord";
        public const String addChargeDetailRecordIfNotExists   = "addChargeDetailRecordIfNotExists";
        public const String addOrUpdateChargeDetailRecord      = "addOrUpdateChargeDetailRecord";
        public const String updateChargeDetailRecord           = "updateChargeDetailRecord";
        public const String removeChargeDetailRecord           = "removeChargeDetailRecord";
        public const String removeAllChargeDetailRecords       = "removeAllChargeDetailRecords";

        public const String addTerminal                        = "addTerminal";
        public const String addTerminalIfNotExists             = "addTerminalIfNotExists";
        public const String addOrUpdateTerminal                = "addOrUpdateTerminal";
        public const String updateTerminal                     = "updateTerminal";
        public const String removeTerminal                     = "removeTerminal";
        public const String removeAllTerminals                 = "removeAllTerminals";

        public const String addBooking                         = "addBooking";
        public const String addBookingIfNotExists              = "addBookingIfNotExists";
        public const String addOrUpdateBooking                 = "addOrUpdateBooking";
        public const String updateBooking                      = "updateBooking";
        public const String removeBooking                      = "removeBooking";
        public const String removeAllBookings                  = "removeAllBookings";

        public const String addBookingLocation                 = "addBookingLocation";
        public const String addBookingLocationIfNotExists      = "addBookingLocationIfNotExists";
        public const String addOrUpdateBookingLocation         = "addOrUpdateBookingLocation";
        public const String updateBookingLocation              = "updateBookingLocation";
        public const String removeBookingLocation              = "removeBookingLocation";
        public const String removeAllBookingLocations          = "removeAllBookingLocations";

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// The URL for your API endpoint.
        /// </summary>
        public URL                      OurBaseURL                 { get; }

        /// <summary>
        /// The URL to your OCPI /versions endpoint.
        /// </summary>
        [Mandatory]
        public URL                      OurVersionsURL             { get; }

        /// <summary>
        /// An optional additional URL path prefix.
        /// </summary>
        public HTTPPath?                AdditionalURLPathPrefix    { get; }

        /// <summary>
        /// Allow anonymous access to locations as Open Data.
        /// </summary>
        public Boolean                  LocationsAsOpenData        { get; }

        /// <summary>
        /// Allow anonymous access to tariffs as Open Data.
        /// </summary>
        public Boolean                  TariffsAsOpenData          { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?                 AllowDowngrades            { get; }

        ///// <summary>
        ///// The logging context.
        ///// </summary>
        //public String?                  LoggingContext             { get; }

        /// <summary>
        /// A template for OCPI client configurations.
        /// </summary>
        public ClientConfigurator       ClientConfigurations       { get; }

        /// <summary>
        /// The OCPI Common HTTP API logger.
        /// </summary>
        public CommonHTTPAPILogger?     Logger                     { get; set; }



        /// <summary>
        /// The database file name for all remote party configuration.
        /// </summary>
        public String                   RemotePartyDBFileName      { get; protected set; }

        #endregion

        #region Events

        #region (protected internal) GetRootRequest      (Request)

        /// <summary>
        /// An event sent whenever a GET / request was received.
        /// </summary>
        public HTTPRequestLogEventX OnGetRootRequest = new();

        /// <summary>
        /// An event sent whenever a GET / request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task GetRootRequest(DateTimeOffset     Timestamp,
                                               HTTPAPIX           API,
                                               HTTPRequest        Request,
                                               CancellationToken  CancellationToken)

            => OnGetRootRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetRootResponse     (Response)

        /// <summary>
        /// An event sent whenever a GET / response was sent.
        /// </summary>
        public HTTPResponseLogEventX OnGetRootResponse = new();

        /// <summary>
        /// An event sent whenever a GET / response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task GetRootResponse(DateTimeOffset     Timestamp,
                                                HTTPAPIX           API,
                                                HTTPRequest        Request,
                                                HTTPResponse       Response,
                                                CancellationToken  CancellationToken)

            => OnGetRootResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) GetVersionsRequest  (Request)

        /// <summary>
        /// An event sent whenever a GET versions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetVersionsRequest = new();

        /// <summary>
        /// An event sent whenever a GET versions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task GetVersionsRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnGetVersionsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetVersionsResponse (Response)

        /// <summary>
        /// An event sent whenever a GET versions response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetVersionsResponse = new();

        /// <summary>
        /// An event sent whenever a GET versions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task GetVersionsResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    OCPIResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnGetVersionsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<VersionInformation>?           CustomVersionInformationSerializer            { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CommonBaseAPI.
        /// </summary>
        /// <param name="OurBaseURL">The URL for your API endpoint.</param>
        /// <param name="OurVersionsURL">The URL of our VERSIONS endpoint.</param>
        /// 
        /// <param name="AdditionalURLPathPrefix"></param>
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="TariffsAsOpenData">Allow anonymous access to tariffs as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// 
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServerName">The default HTTP server name, used whenever no HTTP Host-header has been given.</param>
        /// 
        /// <param name="URLPathPrefix">A common prefix for all URLs.</param>
        /// <param name="HTTPServiceName">The name of the HTTP service.</param>
        /// <param name="APIVersionHashes">The API version hashes (git commit hash values).</param>
        /// 
        /// <param name="IsDevelopment">This HTTP API runs in development mode.</param>
        /// <param name="DevelopmentServers">An enumeration of server names which will imply to run this service in development mode.</param>
        /// <param name="DisableLogging">Disable any logging.</param>
        /// <param name="LoggingPath">The path for all logfiles.</param>
        /// <param name="LogfileName">The name of the logfile.</param>
        /// <param name="LogfileCreator">A delegate for creating the name of the logfile for this API.</param>
        public CommonHTTPAPI(HTTPExtAPIX                    HTTPAPI,
                             URL                            OurBaseURL,
                             URL                            OurVersionsURL,

                             IEnumerable<HTTPHostname>?     Hostnames                 = null,
                             HTTPPath?                      RootPath                  = null,
                             IEnumerable<HTTPContentType>?  HTTPContentTypes          = null,
                             I18NString?                    Description               = null,

                             HTTPPath?                      BasePath                  = null,  // For URL prefixes in HTML!

                             String?                        ExternalDNSName           = null,
                             String?                        HTTPServerName            = DefaultHTTPServerName,
                             String?                        HTTPServiceName           = DefaultHTTPServiceName,
                             String?                        APIVersionHash            = null,
                             JObject?                       APIVersionHashes          = null,

                             EMailAddress?                  APIRobotEMailAddress      = null,
                             String?                        APIRobotGPGPassphrase     = null,
                             ISMTPClient?                   SMTPClient                = null,

                             HTTPPath?                      AdditionalURLPathPrefix   = null,
                             Boolean                        LocationsAsOpenData       = true,
                             Boolean                        TariffsAsOpenData         = false,
                             Boolean?                       AllowDowngrades           = null,

                             Boolean?                       IsDevelopment             = null,
                             IEnumerable<String>?           DevelopmentServers        = null,
                             //Boolean?                       SkipURLTemplates          = false,
                             String?                        DatabaseFileName          = DefaultAssetsDBFileName,
                             Boolean?                       DisableNotifications      = false,

                             Boolean?                       DisableLogging            = null,
                             String?                        LoggingContext            = null,
                             String?                        LoggingPath               = null,
                             String?                        LogfileName               = null,
                             OCPILogfileCreatorDelegate?    LogfileCreator            = null)

            : base(Description ?? I18NString.Create("OCPI Common HTTP API"),
                   HTTPAPI,
                   RootPath,
                   BasePath,

                   ExternalDNSName,
                   HTTPServerName,
                   HTTPServiceName,
                   APIVersionHash,
                   APIVersionHashes,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : (loggingPath, context, logfileName) => String.Concat(
                                                                    loggingPath + Path.DirectorySeparatorChar,
                                                                 //   remoteParty is not null
                                                                 //       ? remoteParty.Id.ToString() + Path.DirectorySeparatorChar
                                                                 //       : null,
                                                                    context is not null ? context + "_" : "",
                                                                    logfileName, "_",
                                                                    Timestamp.Now.Year, "-",
                                                                    Timestamp.Now.Month.ToString("D2"),
                                                                    ".log"
                                                                ))

        {

            this.OurBaseURL               = OurBaseURL;
            this.OurVersionsURL           = OurVersionsURL;

            this.AdditionalURLPathPrefix  = AdditionalURLPathPrefix;
            this.LocationsAsOpenData      = LocationsAsOpenData;
            this.TariffsAsOpenData        = TariffsAsOpenData;
            this.AllowDowngrades          = AllowDowngrades;

            if (this.LoggingPath.IsNotNullOrEmpty())
                Directory.CreateDirectory(this.LoggingPath);

            this.RemotePartyDBFileName    = Path.Combine(this.LoggingPath,
                                                         RemotePartyDBFileName ?? DefaultRemotePartyDBFileName);

            this.ClientConfigurations     = new ClientConfigurator();

            RegisterURLTemplates();

            if (!this.DisableLogging)
                Logger                    = new CommonHTTPAPILogger(
                                                this,
                                                LoggingPath ?? AppContext.BaseDirectory,
                                                LoggingContext,
                                                LogfileCreator:   LogfileCreator is not null
                                                                      ? LogfileCreator
                                                                      : (loggingPath,
                                                                         remoteParty,
                                                                         context,
                                                                         logfileName) => String.Concat(
                                                                                             loggingPath + Path.DirectorySeparatorChar,
                                                                                             remoteParty is not null
                                                                                                 ? remoteParty.Id.ToString() + Path.DirectorySeparatorChar
                                                                                                 : null,
                                                                                             context is not null
                                                                                                 ? context + "_"
                                                                                                 : "",
                                                                                             logfileName, "_",
                                                                                             Timestamp.Now.Year, "-",
                                                                                             Timestamp.Now.Month.ToString("D2"),
                                                                                             ".log"
                                                                                         )
                                            );

        }

        #endregion



        #region AccessTokens

        private readonly ConcurrentDictionary<AccessToken, AccessStatus> accessTokens = [];

        public AccessStatus DefaultAccessStatus { get; set; } = AccessStatus.ALLOWED;


        #region AddAccessToken    (Token, Status)

        public async Task AddAccessToken(AccessToken   Token,
                                         AccessStatus  Status)
        {

            accessTokens.AddOrUpdate(
                Token,
                Status,
                (a, b) => b
            );

            //ToDo: Persist!

        }

        #endregion

        #region RemoveAccessToken (Token)

        public async Task RemoveAccessToken(AccessToken Token)
        {

            accessTokens.TryRemove(Token, out _);

            //ToDo: Persist!

        }

        #endregion


        public Boolean AccessTokenIs(AccessToken   Token,
                                     AccessStatus  Status)
        {

            if (accessTokens.TryGetValue(Token, out var status))
                return status == Status;

            return DefaultAccessStatus ==  Status;

        }

        public Boolean AccessTokenIsAllowed(AccessToken Token)

            => AccessTokenIs(
                   Token,
                   AccessStatus.ALLOWED
               );

        public Boolean AccessTokenIsBlocked(AccessToken Token)

            => AccessTokenIs(
                   Token,
                   AccessStatus.BLOCKED
               );

        #endregion

        #region RemoteParties

        #region Data

        private readonly ConcurrentDictionary<RemoteParty_Id, RemoteParty> remoteParties = new();

        /// <summary>
        /// Return an enumeration of all remote parties.
        /// </summary>
        public IEnumerable<RemoteParty> RemoteParties
            => remoteParties.Values;

        #endregion


        #region AddRemoteParty            (Id, CredentialsRoles, LocalAccessToken, ...)

        /// <summary>
        /// Create a new Remote Party with local access only.
        /// The remote party will start the OCPI registration process afterwards.
        /// Maybe some remote access parameters need already to be set for a successful registration.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CredentialsRoles"></param>
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
        /// <param name="EventTrackingId"></param>
        /// <param name="CurrentUserId"></param>
        public async Task<AddResult<RemoteParty>>

            AddRemoteParty(RemoteParty_Id                                             Id,
                           //IEnumerable<CredentialsRole>                               CredentialsRoles,

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
                           DateTimeOffset?                                            LastUpdated                       = null,
                           EventTracking_Id?                                          EventTrackingId                   = null,
                           User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     //CredentialsRoles,

                                     LocalAccessToken,
                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
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
                                     RemoteStatus,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion

        #region AddRemoteParty            (Id, CredentialsRoles,                   RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<AddResult<RemoteParty>>

            AddRemoteParty(RemoteParty_Id                                             Id,
                           //IEnumerable<CredentialsRole>                               CredentialsRoles,

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

                           RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                           IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                           Version_Id?                                                SelectedVersionId                 = null,
                           DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                           DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                           Boolean?                                                   RemoteAllowDowngrades             = null,

                           PartyStatus?                                               Status                            = null,

                           DateTimeOffset?                                            Created                           = null,
                           DateTimeOffset?                                            LastUpdated                       = null,
                           EventTracking_Id?                                          EventTrackingId                   = null,
                           User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     //CredentialsRoles,

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

                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion

        #region AddRemoteParty            (Id, CredentialsRoles, LocalAccessToken, RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<AddResult<RemoteParty>>

            AddRemoteParty(RemoteParty_Id                                             Id,
                           //IEnumerable<CredentialsRole>                               CredentialsRoles,

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
                           RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                           IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                           Version_Id?                                                SelectedVersionId                 = null,
                           DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                           DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                           Boolean?                                                   RemoteAllowDowngrades             = null,

                           Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                           TOTPConfig?                                                LocalTOTPConfig                   = null,
                           DateTimeOffset?                                            LocalAccessNotBefore              = null,
                           DateTimeOffset?                                            LocalAccessNotAfter               = null,
                           Boolean?                                                   LocalAllowDowngrades              = false,
                           AccessStatus?                                              LocalAccessStatus                 = AccessStatus.ALLOWED,

                           PartyStatus?                                               Status                            = null,

                           DateTimeOffset?                                            Created                           = null,
                           DateTimeOffset?                                            LastUpdated                       = null,
                           EventTracking_Id?                                          EventTrackingId                   = null,
                           User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     //CredentialsRoles,

                                     LocalAccessToken,

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
                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty)) {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion

        #region AddRemoteParty            (Id, CredentialsRoles, LocalAccessInfos, RemoteAccessInfos, ...)

        public async Task<AddResult<RemoteParty>>

            AddRemoteParty(RemoteParty_Id                 Id,
                           //IEnumerable<CredentialsRole>   CredentialsRoles,

                           IEnumerable<LocalAccessInfo>   LocalAccessInfos,
                           IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                           PartyStatus?                   Status            = null,

                           DateTimeOffset?                Created           = null,
                           DateTimeOffset?                LastUpdated       = null,
                           EventTracking_Id?              EventTrackingId   = null,
                           User_Id?                       CurrentUserId     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     //CredentialsRoles,

                                     LocalAccessInfos,
                                     RemoteAccessInfos,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion


        #region AddRemotePartyIfNotExists (Id, CredentialsRoles, LocalAccessToken, ...)

        /// <summary>
        /// Create a new Remote Party with local access only, if it does not already exist.
        /// The remote party will start the OCPI registration process afterwards.
        /// Maybe some remote access parameters need already to be set for a successful registration.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CredentialsRoles"></param>
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
        /// <param name="EventTrackingId"></param>
        /// <param name="CurrentUserId"></param>
        public async Task<AddResult<RemoteParty>>

            AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                      //IEnumerable<CredentialsRole>                               CredentialsRoles,

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
                                      DateTimeOffset?                                            LastUpdated                       = null,
                                      EventTracking_Id?                                          EventTrackingId                   = null,
                                      User_Id?                                                   CurrentUserId                     = null)

        {

            if (remoteParties.TryGetValue(Id, out var existingRemoteParty))
                return AddResult<RemoteParty>.NoOperation(
                           EventTracking_Id.New,
                           existingRemoteParty,
                           "The remote party already exists!"
                       );

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     //CredentialsRoles,

                                     LocalAccessToken,
                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
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
                                     RemoteStatus,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemotePartyIfNotExists,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion

        #region AddRemotePartyIfNotExists (Id, CredentialsRoles,                   RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<AddResult<RemoteParty>>

            AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                      //IEnumerable<CredentialsRole>                               CredentialsRoles,

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

                                      RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                                      IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                                      Version_Id?                                                SelectedVersionId                 = null,
                                      DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                                      DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                                      Boolean?                                                   RemoteAllowDowngrades             = null,

                                      PartyStatus?                                               Status                            = null,

                                      DateTimeOffset?                                            Created                           = null,
                                      DateTimeOffset?                                            LastUpdated                       = null,
                                      EventTracking_Id?                                          EventTrackingId                   = null,
                                      User_Id?                                                   CurrentUserId                     = null)

        {

            if (remoteParties.TryGetValue(Id, out var existingRemoteParty))
                return AddResult<RemoteParty>.NoOperation(
                           EventTracking_Id.New,
                           existingRemoteParty,
                           "The remote party already exists!"
                       );

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     //CredentialsRoles,

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

                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemotePartyIfNotExists,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion

        #region AddRemotePartyIfNotExists (Id, CredentialsRoles, LocalAccessToken, RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<AddResult<RemoteParty>>

            AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                      //IEnumerable<CredentialsRole>                               CredentialsRoles,

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
                                      RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                                      IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                                      Version_Id?                                                SelectedVersionId                 = null,
                                      DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                                      DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                                      Boolean?                                                   RemoteAllowDowngrades             = null,

                                      Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                                      TOTPConfig?                                                LocalTOTPConfig                   = null,
                                      DateTimeOffset?                                            LocalAccessNotBefore              = null,
                                      DateTimeOffset?                                            LocalAccessNotAfter               = null,
                                      Boolean?                                                   LocalAllowDowngrades              = false,
                                      AccessStatus?                                              LocalAccessStatus                 = AccessStatus.ALLOWED,

                                      PartyStatus?                                               Status                            = null,

                                      DateTimeOffset?                                            Created                           = null,
                                      DateTimeOffset?                                            LastUpdated                       = null,
                                      EventTracking_Id?                                          EventTrackingId                   = null,
                                      User_Id?                                                   CurrentUserId                     = null)

        {

            if (remoteParties.TryGetValue(Id, out var existingRemoteParty))
                return AddResult<RemoteParty>.NoOperation(
                           EventTracking_Id.New,
                           existingRemoteParty,
                           "The remote party already exists!"
                       );

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     //CredentialsRoles,

                                     LocalAccessToken,

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
                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemotePartyIfNotExists,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion

        #region AddRemotePartyIfNotExists (Id, CredentialsRoles, LocalAccessInfos, RemoteAccessInfos, ...)

        public async Task<AddResult<RemoteParty>>

            AddRemotePartyIfNotExists(RemoteParty_Id                 Id,
                                      //IEnumerable<CredentialsRole>   CredentialsRoles,

                                      IEnumerable<LocalAccessInfo>   LocalAccessInfos,
                                      IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                                      PartyStatus?                   Status            = null,

                                      DateTimeOffset?                Created           = null,
                                      DateTimeOffset?                LastUpdated       = null,
                                      EventTracking_Id?              EventTrackingId   = null,
                                      User_Id?                       CurrentUserId     = null)

        {

            if (remoteParties.TryGetValue(Id, out var existingRemoteParty))
                return AddResult<RemoteParty>.NoOperation(
                           EventTracking_Id.New,
                           existingRemoteParty,
                           "The remote party already exists!"
                       );

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     //CredentialsRoles,

                                     LocalAccessInfos,
                                     RemoteAccessInfos,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemotePartyIfNotExists,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion


        #region AddOrUpdateRemoteParty    (Id, CredentialsRoles, LocalAccessToken, ...)

        /// <summary>
        /// Create or update a Remote Party with local access only.
        /// The remote party will start the OCPI registration process afterwards.
        /// Maybe some remote access parameters need already to be set for a successful registration.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CredentialsRoles"></param>
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
        /// <param name="EventTrackingId"></param>
        /// <param name="CurrentUserId"></param>
        public async Task<AddOrUpdateResult<RemoteParty>>

            AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                   //IEnumerable<CredentialsRole>                               CredentialsRoles,

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
                                   DateTimeOffset?                                            LastUpdated                       = null,
                                   EventTracking_Id?                                          EventTrackingId                   = null,
                                   User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     //CredentialsRoles,

                                     LocalAccessToken,
                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
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
                                     RemoteStatus,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            var added = false;

            remoteParties.AddOrUpdate(
                newRemoteParty.Id,

                // Add
                id => {
                    added = true;
                    return newRemoteParty;
                },

                // Update
                (id, oldRemoteParty) => {
                    return newRemoteParty;
                }
            );

            await LogRemoteParty(
                      CommonHTTPAPI.addOrUpdateRemoteParty,
                      newRemoteParty.ToJSON(true),
                      EventTrackingId ?? EventTracking_Id.New,
                      CurrentUserId
                  );

            return added
                       ? AddOrUpdateResult<RemoteParty>.Created(
                             EventTracking_Id.New,
                             newRemoteParty
                         )
                       : AddOrUpdateResult<RemoteParty>.Updated(
                             EventTracking_Id.New,
                             newRemoteParty
                         );

        }

        #endregion

        #region AddOrUpdateRemoteParty    (Id, CredentialsRoles,                   RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<AddOrUpdateResult<RemoteParty>>

            AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                   //IEnumerable<CredentialsRole>                               CredentialsRoles,

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

                                   RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                                   IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                                   Version_Id?                                                SelectedVersionId                 = null,
                                   DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                                   DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                                   Boolean?                                                   RemoteAllowDowngrades             = null,

                                   PartyStatus?                                               Status                            = null,

                                   DateTimeOffset?                                            Created                           = null,
                                   DateTimeOffset?                                            LastUpdated                       = null,
                                   EventTracking_Id?                                          EventTrackingId                   = null,
                                   User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     //CredentialsRoles,

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

                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            var added = false;

            remoteParties.AddOrUpdate(
                newRemoteParty.Id,

                // Add
                id => {
                    added = true;
                    return newRemoteParty;
                },

                // Update
                (id, oldRemoteParty) => {
                    return newRemoteParty;
                }
            );

            await LogRemoteParty(
                      CommonHTTPAPI.addOrUpdateRemoteParty,
                      newRemoteParty.ToJSON(true),
                      EventTrackingId ?? EventTracking_Id.New,
                      CurrentUserId
                  );

            return added
                       ? AddOrUpdateResult<RemoteParty>.Created(
                             EventTracking_Id.New,
                             newRemoteParty
                         )
                       : AddOrUpdateResult<RemoteParty>.Updated(
                             EventTracking_Id.New,
                             newRemoteParty
                         );

        }

        #endregion

        #region AddOrUpdateRemoteParty    (Id, CredentialsRoles, LocalAccessToken, RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<AddOrUpdateResult<RemoteParty>>

            AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                   //IEnumerable<CredentialsRole>                               CredentialsRoles,

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
                                   RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                                   IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                                   Version_Id?                                                SelectedVersionId                 = null,
                                   DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                                   DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                                   Boolean?                                                   RemoteAllowDowngrades             = null,

                                   Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                                   TOTPConfig?                                                LocalTOTPConfig                   = null,
                                   DateTimeOffset?                                            LocalAccessNotBefore              = null,
                                   DateTimeOffset?                                            LocalAccessNotAfter               = null,
                                   Boolean?                                                   LocalAllowDowngrades              = false,
                                   AccessStatus?                                              LocalAccessStatus                 = AccessStatus.ALLOWED,

                                   PartyStatus?                                               Status                            = null,

                                   DateTimeOffset?                                            Created                           = null,
                                   DateTimeOffset?                                            LastUpdated                       = null,
                                   EventTracking_Id?                                          EventTrackingId                   = null,
                                   User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     //CredentialsRoles,

                                     LocalAccessToken,

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
                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            var added = false;

            remoteParties.AddOrUpdate(
                newRemoteParty.Id,

                // Add
                id => {
                    added = true;
                    return newRemoteParty;
                },

                // Update
                (id, oldRemoteParty) => {
                    return newRemoteParty;
                }
            );

            await LogRemoteParty(
                      CommonHTTPAPI.addOrUpdateRemoteParty,
                      newRemoteParty.ToJSON(true),
                      EventTrackingId ?? EventTracking_Id.New,
                      CurrentUserId
                  );

            return added
                       ? AddOrUpdateResult<RemoteParty>.Created(
                             EventTracking_Id.New,
                             newRemoteParty
                         )
                       : AddOrUpdateResult<RemoteParty>.Updated(
                             EventTracking_Id.New,
                             newRemoteParty
                         );

        }

        #endregion

        #region AddOrUpdateRemoteParty    (Id, CredentialsRoles, LocalAccessInfos, RemoteAccessInfos, ...)

        public async Task<AddOrUpdateResult<RemoteParty>>

            AddOrUpdateRemoteParty(RemoteParty_Id                 Id,
                                   //IEnumerable<CredentialsRole>   CredentialsRoles,

                                   IEnumerable<LocalAccessInfo>   LocalAccessInfos,
                                   IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                                   PartyStatus?                   Status            = null,

                                   DateTimeOffset?                Created           = null,
                                   DateTimeOffset?                LastUpdated       = null,
                                   EventTracking_Id?              EventTrackingId   = null,
                                   User_Id?                       CurrentUserId     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     //CredentialsRoles,

                                     LocalAccessInfos,
                                     RemoteAccessInfos,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            var added = false;

            remoteParties.AddOrUpdate(
                newRemoteParty.Id,

                // Add
                id => {
                    added = true;
                    return newRemoteParty;
                },

                // Update
                (id, oldRemoteParty) => {
                    return newRemoteParty;
                }
            );

            await LogRemoteParty(
                      CommonHTTPAPI.addOrUpdateRemoteParty,
                      newRemoteParty.ToJSON(true),
                      EventTrackingId ?? EventTracking_Id.New,
                      CurrentUserId
                  );

            return added
                       ? AddOrUpdateResult<RemoteParty>.Created(
                             EventTracking_Id.New,
                             newRemoteParty
                         )
                       : AddOrUpdateResult<RemoteParty>.Updated(
                             EventTracking_Id.New,
                             newRemoteParty
                         );

        }

        #endregion


        #region UpdateRemoteParty         (Id, CredentialsRoles, LocalAccessToken, ...)

        /// <summary>
        /// Update a Remote Party with local access only.
        /// The remote party will start the OCPI registration process afterwards.
        /// Maybe some remote access parameters need already to be set for a successful registration.
        /// </summary>
        /// <param name="ExistingRemoteParty"></param>
        /// <param name="CredentialsRoles"></param>
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
        /// <param name="EventTrackingId"></param>
        /// <param name="CurrentUserId"></param>
        public async Task<UpdateResult<RemoteParty>>

            UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,
                              //IEnumerable<CredentialsRole>                               CredentialsRoles,

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
                              DateTimeOffset?                                            LastUpdated                       = null,
                              EventTracking_Id?                                          EventTrackingId                   = null,
                              User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     ExistingRemoteParty.Id,
                                     //CredentialsRoles,

                                     LocalAccessToken,
                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
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
                                     RemoteStatus,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.updateRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return UpdateResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return UpdateResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be updated!"
                   );

        }

        #endregion

        #region UpdateRemoteParty         (Id, CredentialsRoles,                   RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<UpdateResult<RemoteParty>>

            UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,
                              //IEnumerable<CredentialsRole>                               CredentialsRoles,

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

                              RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                              IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                              Version_Id?                                                SelectedVersionId                 = null,
                              DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                              DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                              Boolean?                                                   RemoteAllowDowngrades             = null,

                              PartyStatus?                                               Status                            = null,

                              DateTimeOffset?                                            Created                           = null,
                              DateTimeOffset?                                            LastUpdated                       = null,
                              EventTracking_Id?                                          EventTrackingId                   = null,
                              User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     ExistingRemoteParty.Id,
                                     //CredentialsRoles,

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

                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.updateRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return UpdateResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return UpdateResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be updated!"
                   );

        }

        #endregion

        #region UpdateRemoteParty         (Id, CredentialsRoles, LocalAccessToken, RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<UpdateResult<RemoteParty>>

            UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,
                              //IEnumerable<CredentialsRole>                               CredentialsRoles,

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
                              RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                              IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                              Version_Id?                                                SelectedVersionId                 = null,
                              DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                              DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                              Boolean?                                                   RemoteAllowDowngrades             = null,

                              Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                              TOTPConfig?                                                LocalTOTPConfig                   = null,
                              DateTimeOffset?                                            LocalAccessNotBefore              = null,
                              DateTimeOffset?                                            LocalAccessNotAfter               = null,
                              Boolean?                                                   LocalAllowDowngrades              = false,
                              AccessStatus?                                              LocalAccessStatus                 = AccessStatus.ALLOWED,

                              PartyStatus?                                               Status                            = null,

                              DateTimeOffset?                                            Created                           = null,
                              DateTimeOffset?                                            LastUpdated                       = null,
                              EventTracking_Id?                                          EventTrackingId                   = null,
                              User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     ExistingRemoteParty.Id,
                                     //CredentialsRoles,

                                     LocalAccessToken,

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
                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.updateRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return UpdateResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return UpdateResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be updated!"
                   );

        }

        #endregion

        #region UpdateRemoteParty         (Id, CredentialsRoles, LocalAccessInfos, RemoteAccessInfos, ...)

        public async Task<UpdateResult<RemoteParty>>

            UpdateRemoteParty(RemoteParty                    ExistingRemoteParty,
                              //IEnumerable<CredentialsRole>   CredentialsRoles,

                              IEnumerable<LocalAccessInfo>   LocalAccessInfos,
                              IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                              PartyStatus?                   Status            = null,

                              DateTimeOffset?                Created           = null,
                              DateTimeOffset?                LastUpdated       = null,
                              EventTracking_Id?              EventTrackingId   = null,
                              User_Id?                       CurrentUserId     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     ExistingRemoteParty.Id,
                                     //CredentialsRoles,

                                     LocalAccessInfos,
                                     RemoteAccessInfos,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.updateRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return UpdateResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return UpdateResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be updated!"
                   );

        }

        #endregion


        #region ContainsRemoteParty       (RemotePartyId)

        /// <summary>
        /// Whether this API contains a remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        public Boolean ContainsRemoteParty(RemoteParty_Id RemotePartyId)

            => remoteParties.ContainsKey(RemotePartyId);

        #endregion

        #region GetRemoteParty            (RemotePartyId)

        /// <summary>
        /// Get the remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        public RemoteParty? GetRemoteParty(RemoteParty_Id RemotePartyId)
        {

            if (remoteParties.TryGetValue(RemotePartyId, out var remoteParty))
                return remoteParty;

            return null;

        }

        #endregion

        #region TryGetRemoteParty         (RemotePartyId, out RemoteParty)

        /// <summary>
        /// Try to get the remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        /// <param name="RemoteParty">The remote party.</param>
        public Boolean TryGetRemoteParty(RemoteParty_Id                        RemotePartyId,
                                         [NotNullWhen(true)] out RemoteParty?  RemoteParty)

            => remoteParties.TryGetValue(
                   RemotePartyId,
                   out RemoteParty
               );

        #endregion

        #region GetRemoteParties          (IncludeFilter = null)

        ///// <summary>
        ///// Get all remote parties machting the given optional filter.
        ///// </summary>
        ///// <param name="IncludeFilter">A delegate for filtering remote parties.</param>
        //public IEnumerable<RemoteParty> GetRemoteParties(IncludeRemoteParty? IncludeFilter = null)

        //    => IncludeFilter is null
        //           ? remoteParties.Values
        //           : remoteParties.Values.
        //                           Where(remoteParty => IncludeFilter(remoteParty));

        #endregion

        #region GetRemoteParties          (CountryCode, PartyId)

        ///// <summary>
        ///// Get all remote parties having the given country code and party identification.
        ///// </summary>
        ///// <param name="CountryCode">A country code.</param>
        ///// <param name="PartyId">A party identification.</param>
        //public IEnumerable<RemoteParty> GetRemoteParties(CountryCode  CountryCode,
        //                                                 Party_Id     PartyId)

        //    => remoteParties.Values.
        //                     Where(remoteParty => remoteParty.Roles.Any(credentialsRole => credentialsRole.PartyId.CountryCode == CountryCode &&
        //                                                                                   credentialsRole.PartyId.Party       == PartyId));

        #endregion

        #region GetRemoteParties          (CountryCode, PartyId, Role)

        ///// <summary>
        ///// Get all remote parties having the given country code, party identification and role.
        ///// </summary>
        ///// <param name="CountryCode">A country code.</param>
        ///// <param name="PartyId">A party identification.</param>
        ///// <param name="Role">A role.</param>
        //public IEnumerable<RemoteParty> GetRemoteParties(CountryCode  CountryCode,
        //                                                 Party_Id     PartyId,
        //                                                 Role         Role)

        //    => remoteParties.Values.
        //                     Where(remoteParty => remoteParty.Roles.Any(credentialsRole => credentialsRole.PartyId.CountryCode == CountryCode &&
        //                                                                                   credentialsRole.PartyId.Party       == PartyId &&
        //                                                                                   credentialsRole.Role                == Role));

        #endregion

        #region GetRemoteParties          (Role)

        ///// <summary>
        ///// Get all remote parties having the given role.
        ///// </summary>
        ///// <param name="Role">The role of the remote parties.</param>
        //public IEnumerable<RemoteParty> GetRemoteParties(Role Role)

        //    => remoteParties.Values.
        //                     Where(remoteParty => remoteParty.Roles.Any(credentialsRole => credentialsRole.Role == Role));

        #endregion

        #region GetRemoteParties          (AccessToken)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken AccessToken)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken == AccessToken));

        #endregion

        #region GetRemoteParties          (AccessToken, AccessStatus)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken   AccessToken,
                                                         AccessStatus  AccessStatus)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken == AccessToken &&
                                                                                                      localAccessInfo.Status      == AccessStatus));

        #endregion

        #region TryGetRemoteParties       (AccessToken, TOTP, out RemoteParties, out ErrorMessage)

        public Boolean TryGetRemoteParties(AccessToken                                           AccessToken,
                                           String?                                               TOTP,
                                           out IEnumerable<Tuple<RemoteParty, LocalAccessInfo>>  RemoteParties,
                                           [NotNullWhen(false)] out String?                      ErrorMessage)
        {

            var remoteParties = new List<Tuple<RemoteParty, LocalAccessInfo>>();

            foreach (var remoteParty in this.remoteParties.Values)
            {
                foreach (var localAccessInfo in remoteParty.LocalAccessInfos)
                {

                    if (localAccessInfo.AccessToken == AccessToken)
                    {

                        if (localAccessInfo.TOTPConfig is not null)
                        {

                            var (previous,
                                 current,
                                 next,
                                 _,
                                 _) = TOTPGenerator.GenerateTOTPs(
                                          localAccessInfo.TOTPConfig.SharedSecret,
                                          localAccessInfo.TOTPConfig.ValidityTime,
                                          localAccessInfo.TOTPConfig.Length,
                                          localAccessInfo.TOTPConfig.Alphabet
                                      );

                            if (TOTP == current || TOTP == previous || TOTP == next)
                                remoteParties.Add(
                                    new Tuple<RemoteParty, LocalAccessInfo>(
                                        remoteParty,
                                        localAccessInfo
                                    )
                                );

                            else
                            {
                                RemoteParties  = [];
                                ErrorMessage   = "Invalid Time-based One-Time Password (TOTP)!";
                                return false;
                            }

                        }

                        else
                            remoteParties.Add(
                                new Tuple<RemoteParty, LocalAccessInfo>(
                                    remoteParty,
                                    localAccessInfo
                                )
                            );

                    }

                }
            }

            RemoteParties  = remoteParties;
            ErrorMessage   = remoteParties.Count == 0
                                 ? "Unknown access token!"
                                 : null;

            return remoteParties.Count > 0;

        }

        #endregion


        #region RemoveRemoteParty         (RemoteParty)

        public async Task<Boolean> RemoveRemoteParty(RemoteParty        RemoteParty,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            if (remoteParties.TryRemove(RemoteParty.Id, out var remoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.removeRemoteParty,
                          remoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return true;

            }

            return false;

        }

        #endregion

        #region RemoveRemoteParty         (RemotePartyId)

        public async Task<Boolean> RemoveRemoteParty(RemoteParty_Id     RemotePartyId,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            if (remoteParties.Remove(RemotePartyId, out var remoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.removeRemoteParty,
                          remoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return true;

            }

            return false;

        }

        #endregion

        #region RemoveRemoteParty         (CountryCode, PartyId, Role)

        //public async Task<Boolean> RemoveRemoteParty(CountryCode        CountryCode,
        //                                             Party_Id           PartyId,
        //                                             Role               Role,
        //                                             EventTracking_Id?  EventTrackingId   = null,
        //                                             User_Id?           CurrentUserId     = null)
        //{

        //    foreach (var remoteParty in GetRemoteParties(CountryCode,
        //                                                 PartyId,
        //                                                 Role))
        //    {

        //        remoteParties.TryRemove(remoteParty.Id, out _);

        //        await LogRemoteParty(
        //                  CommonHTTPAPI.removeRemoteParty,
        //                  remoteParty.ToJSON(true),
        //                  EventTrackingId ?? EventTracking_Id.New,
        //                  CurrentUserId
        //              );

        //    }

        //    return true;

        //}

        #endregion

        #region RemoveRemoteParty         (CountryCode, PartyId, Role, AccessToken)

        //public async Task<Boolean> RemoveRemoteParty(CountryCode        CountryCode,
        //                                             Party_Id           PartyId,
        //                                             Role               Role,
        //                                             AccessToken        AccessToken,
        //                                             EventTracking_Id?  EventTrackingId   = null,
        //                                             User_Id?           CurrentUserId     = null)
        //{

        //    foreach (var remoteParty in remoteParties.Values.
        //                                              Where(remoteParty => remoteParty.Roles.            Any(credentialsRole  => credentialsRole.PartyId.CountryCode == CountryCode &&
        //                                                                                                                         credentialsRole.PartyId.Party       == PartyId &&
        //                                                                                                                         credentialsRole.Role                == Role) &&

        //                                                                   remoteParty.RemoteAccessInfos.Any(remoteAccessInfo => remoteAccessInfo.AccessToken == AccessToken)))
        //    {

        //        remoteParties.TryRemove(remoteParty.Id, out _);

        //        await LogRemoteParty(
        //                  CommonHTTPAPI.removeRemoteParty,
        //                  remoteParty.ToJSON(true),
        //                  EventTrackingId ?? EventTracking_Id.New,
        //                  CurrentUserId
        //              );

        //    }

        //    return true;

        //}

        #endregion

        #region RemoveAllRemoteParties    ()

        public async Task RemoveAllRemoteParties(EventTracking_Id?  EventTrackingId   = null,
                                                 User_Id?           CurrentUserId     = null)
        {

            remoteParties.Clear();

            await LogRemoteParty("removeAllRemoteParties",
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

        }

        #endregion

        #endregion



        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region OPTIONS  ~/

            HTTPBaseAPI.AddHandler(
                HTTPMethod.OPTIONS,
                URLPathPrefix,
                HTTPDelegate: request =>

                    Task.FromResult(
                        new HTTPResponse.Builder(request) {
                            HTTPStatusCode             = HTTPStatusCode.OK,
                            Server                     = HTTPServiceName,
                            Date                       = Timestamp.Now,
                            AccessControlAllowOrigin   = "*",
                            AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                            Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                            AccessControlAllowHeaders  = [ "Authorization" ],
                            Connection                 = ConnectionType.KeepAlive
                        }.AsImmutable)

            );

            #endregion

            #region GET      ~/

            HTTPBaseAPI.AddHandler(
                HTTPMethod.GET,
                URLPathPrefix,
                HTTPContentType.Text.PLAIN,
                HTTPRequestLogger:   GetRootRequest,
                HTTPResponseLogger:  GetRootResponse,
                HTTPDelegate:        request =>

                    Task.FromResult(
                        new HTTPResponse.Builder(request) {
                            HTTPStatusCode             = HTTPStatusCode.OK,
                            Server                     = HTTPServiceName,
                            Date                       = Timestamp.Now,
                            AccessControlAllowOrigin   = "*",
                            AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                            AccessControlAllowHeaders  = [ "Authorization" ],
                            ContentType                = HTTPContentType.Text.PLAIN,
                            Content                    = "This is an Open Charge Point Interface v2.x HTTP service!\r\nPlease check ~/versions!".ToUTF8Bytes(),
                            Connection                 = ConnectionType.KeepAlive,
                            Vary                       = "Accept"
                        }.AsImmutable)

            );

            HTTPBaseAPI.AddHandler(
                HTTPMethod.GET,
                URLPathPrefix,
                HTTPContentType.Application.JSON_UTF8,
                HTTPRequestLogger:   GetRootRequest,
                HTTPResponseLogger:  GetRootResponse,
                HTTPDelegate:        request =>

                    Task.FromResult(
                        new HTTPResponse.Builder(request) {
                            HTTPStatusCode             = HTTPStatusCode.OK,
                            Server                     = HTTPServiceName,
                            Date                       = Timestamp.Now,
                            AccessControlAllowOrigin   = "*",
                            AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                            AccessControlAllowHeaders  = [ "Authorization" ],
                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                            Content                    = JSONObject.Create(
                                                             new JProperty(
                                                                 "message",
                                                                 "This is an Open Charge Point Interface v2.x HTTP service! Please check ~/versions!"
                                                             )
                                                         ).ToUTF8Bytes(),
                            Connection                 = ConnectionType.KeepAlive,
                            Vary                       = "Accept"
                        }.AsImmutable),

                AllowReplacement: URLReplacement.Allow

            );

            #endregion


            #region OPTIONS  ~/versions

            // ----------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions
            // ----------------------------------------------------
            HTTPBaseAPI.AddHandler(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "versions",
                request =>

                    Task.FromResult(
                        new HTTPResponse.Builder(request) {
                            HTTPStatusCode             = HTTPStatusCode.OK,
                            Server                     = HTTPServiceName,
                            Date                       = Timestamp.Now,
                            AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                            Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                            AccessControlAllowHeaders  = [ "Authorization" ],
                            Vary                       = "Accept"
                        }.AsImmutable)

            );

            #endregion

            #region GET      ~/versions

            // ----------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions
            // ----------------------------------------------------------------------
            this.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "versions",
                //HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   GetVersionsRequest,
                OCPIResponseLogger:  GetVersionsResponse,
                OCPIRequestHandler:  request => {

                    var requestId        = request.HTTPRequest.TryParseHeaderField<Request_Id>    ("X-Request-ID",           Request_Id.    TryParse) ?? Request_Id.    NewRandom(IsLocal: true);
                    var correlationId    = request.HTTPRequest.TryParseHeaderField<Correlation_Id>("X-Correlation-ID",       Correlation_Id.TryParse) ?? Correlation_Id.NewRandom(IsLocal: true);
                    var toCountryCode    = request.HTTPRequest.TryParseHeaderField<CountryCode>   ("OCPI-to-country-code",   CountryCode.   TryParse);
                    var toPartyId        = request.HTTPRequest.TryParseHeaderField<Party_Id>      ("OCPI-to-party-id",       Party_Id.      TryParse);
                    var fromCountryCode  = request.HTTPRequest.TryParseHeaderField<CountryCode>   ("OCPI-from-country-code", CountryCode.   TryParse);
                    var fromPartyId      = request.HTTPRequest.TryParseHeaderField<Party_Id>      ("OCPI-from-party-id",     Party_Id.      TryParse);


                    #region Check access token

                    AccessToken? accessToken1 = null;

                    if (request.HTTPRequest.Authorization is HTTPTokenAuthentication TokenAuth &&
                        //TokenAuth.Token.TryParseBASE64_UTF8(out var decodedToken, out var errorResponse) &&
                        AccessToken.TryParse(TokenAuth.Token, out var accessToken))
                    {
                        accessToken1 = accessToken;
                    }

                    else if (request.HTTPRequest.Authorization is HTTPBasicAuthentication BasicAuth &&
                        AccessToken.TryParse(BasicAuth.Username, out accessToken))
                    {
                        accessToken1 = accessToken;
                    }

                    if (accessToken1.HasValue)
                    {

                        if (AccessTokenIsBlocked(accessToken1.Value))
                        {

                          //  var httpResponseBuilder2 =
                          //      new HTTPResponse.Builder(request.HTTPRequest) {
                          //          HTTPStatusCode             = HTTPStatusCode.OK,
                          //          Server                     = HTTPServiceName,
                          //          Date                       = Timestamp.Now,
                          //          AccessControlAllowOrigin   = "*",
                          //          AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                          //          Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                          //          AccessControlAllowHeaders  = [ "Authorization" ],
                          //          ContentType                = HTTPContentType.Application.JSON_UTF8,
                          //          Content                    = JSONObject.Create(
                          //                                           new JProperty("status_code",     2000),
                          //                                           new JProperty("status_message", "Invalid or blocked access token!")
                          //                                       ).ToUTF8Bytes(),
                          //          Connection                 = ConnectionType.KeepAlive,
                          //          Vary                       = "Accept"
                          //      };
                          //
                          //  httpResponseBuilder2.Set("X-Request-ID",      requestId);
                          //  httpResponseBuilder2.Set("X-Correlation-ID",  correlationId);
                          //
                          //  return Task.FromResult(httpResponseBuilder2.AsImmutable);

                            return Task.FromResult(
                                       new OCPIResponse.Builder(request) {
                                           StatusCode           = 2000,
                                           StatusMessage        = "Invalid or blocked access token!",
                                           HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               Server                     = HTTPServiceName,
                                               Date                       = Timestamp.Now,
                                               AccessControlAllowOrigin   = "*",
                                               AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                               Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                               AccessControlAllowHeaders  = [ "Authorization" ],
                                               ContentType                = HTTPContentType.Application.JSON_UTF8,
                                               Connection                 = ConnectionType.KeepAlive,
                                               Vary                       = "Accept"
                                           }
                                       }
                                   );

                        }

                    }

                    #endregion


                    //var httpResponseBuilder =
                    //    new HTTPResponse.Builder(request.HTTPRequest) {
                    //        HTTPStatusCode             = HTTPStatusCode.OK,
                    //        Server                     = HTTPServiceName,
                    //        Date                       = Timestamp.Now,
                    //        AccessControlAllowOrigin   = "*",
                    //        AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                    //        Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                    //        AccessControlAllowHeaders  = [ "Authorization" ],
                    //        ContentType                = HTTPContentType.Application.JSON_UTF8,
                    //        Content                    = JSONObject.Create(
                    //                                         new JProperty("timestamp",        Timestamp.Now.ToISO8601()),
                    //                                         new JProperty("status_code",      1000),
                    //                                         new JProperty("status_message",  "Hello world!"),
                    //                                         new JProperty("data",             new JArray(
                    //                                                                               VersionInformations.
                    //                                                                                   OrderBy(versionInformation => versionInformation.Id).
                    //                                                                                   Select (versionInformation => versionInformation.ToJSON(CustomVersionInformationSerializer))
                    //                                                                           ))
                    //                                     ).ToUTF8Bytes(),
                    //        Connection                 = ConnectionType.KeepAlive,
                    //        Vary                       = "Accept"
                    //    };

                    //httpResponseBuilder.Set("X-Request-ID",      requestId);
                    //httpResponseBuilder.Set("X-Correlation-ID",  correlationId);

                    //return Task.FromResult(httpResponseBuilder.AsImmutable);

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = new JArray(
                                                              VersionInformations.
                                                                  OrderBy(versionInformation => versionInformation.Id).
                                                                  Select (versionInformation => versionInformation.ToJSON(CustomVersionInformationSerializer))
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       Server                     = HTTPServiceName,
                                       Date                       = Timestamp.Now,
                                       AccessControlAllowOrigin   = "*",
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                       Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       ContentType                = HTTPContentType.Application.JSON_UTF8,
                                       Connection                 = ConnectionType.KeepAlive,
                                       Vary                       = "Accept"
                                   }
                               }
                           );

                }

            );

            #endregion


            #region GET      ~/support

            HTTPBaseAPI.AddHandler(

                HTTPMethod.GET,
                URLPathPrefix + "/support",
                HTTPContentType.Text.PLAIN,
                request =>

                    Task.FromResult(
                        new HTTPResponse.Builder(request) {
                            HTTPStatusCode             = HTTPStatusCode.OK,
                            Server                     = HTTPServerName,
                            Date                       = Timestamp.Now,
                            AccessControlAllowOrigin   = "*",
                            AccessControlAllowMethods  = [ "GET" ],
                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                            ContentType                = HTTPContentType.Text.PLAIN,
                            Content                    = "https://github.com/OpenChargingCloud/WWCP_OCPI".ToUTF8Bytes(),
                            Connection                 = ConnectionType.KeepAlive,
                            Vary                       = "Accept"
                        }.AsImmutable
                    )

            );

            #endregion

        }

        #endregion



        #region OCPI Version Informations

        private readonly ConcurrentDictionary<Version_Id, VersionInformation>  versionInformations   = [];

        public IEnumerable<VersionInformation> VersionInformations
            => versionInformations.Values;


        #region AddVersionInformation            (VersionInformation, ...)

        public async Task<AddResult<VersionInformation>>

            AddVersionInformation(VersionInformation  VersionInformation,
                                  Boolean             SkipNotifications   = false,
                                  EventTracking_Id?   EventTrackingId     = null,
                                  User_Id?            CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (versionInformations.TryAdd(VersionInformation.Id, VersionInformation))
            {

                //await LogAsset(
                //          addLocation,
                //          Location.ToJSON(true,
                //                          true,
                //                          true,
                //                          CustomLocationSerializer,
                //                          CustomPublishTokenSerializer,
                //                          CustomAddressSerializer,
                //                          CustomAdditionalGeoLocationSerializer,
                //                          CustomChargingStationSerializer,
                //                          CustomEVSESerializer,
                //                          CustomStatusScheduleSerializer,
                //                          CustomConnectorSerializer,
                //                          CustomEnergyMeterSerializer,
                //                          CustomTransparencySoftwareStatusSerializer,
                //                          CustomTransparencySoftwareSerializer,
                //                          CustomDisplayTextSerializer,
                //                          CustomBusinessDetailsSerializer,
                //                          CustomHoursSerializer,
                //                          CustomImageSerializer,
                //                          CustomEnergyMixSerializer,
                //                          CustomEnergySourceSerializer,
                //                          CustomEnvironmentalImpactSerializer,
                //                          CustomLocationMaxPowerSerializer),
                //          EventTrackingId,
                //          CurrentUserId
                //      );

                //if (!SkipNotifications)
                //{

                //    var OnLocationAddedLocal = OnLocationAdded;
                //    if (OnLocationAddedLocal is not null)
                //    {
                //        try
                //        {
                //            await OnLocationAddedLocal(Location);
                //        }
                //        catch (Exception e)
                //        {
                //            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnLocationAdded), ": ",
                //                        Environment.NewLine, e.Message,
                //                        Environment.NewLine, e.StackTrace ?? "");
                //        }
                //    }

                //}

                return AddResult<VersionInformation>.Success(
                           EventTrackingId,
                           VersionInformation
                       );

            }

            return AddResult<VersionInformation>.Failed(
                       EventTrackingId,
                       VersionInformation,
                       "The given version information already exists!"
                   );

        }

        #endregion

        #endregion


        //ToDo: Wrap the following into a pluggable interface!

        #region Log/Read   Remote Parties

        #region LogRemoteParty        (Command,              ...)

        public ValueTask LogRemoteParty(String            Command,
                                        EventTracking_Id  EventTrackingId,
                                        User_Id?          CurrentUserId   = null)

            => WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   null,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region LogRemoteParty        (Command, Text = null, ...)

        public ValueTask LogRemoteParty(String            Command,
                                        String?           Text,
                                        EventTracking_Id  EventTrackingId,
                                        User_Id?          CurrentUserId   = null)

            => WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   Text is not null
                       ? JToken.Parse(Text)
                       : null,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region LogRemoteParty        (Command, JSON,        ...)

        public ValueTask LogRemoteParty(String            Command,
                                        JObject           JSON,
                                        EventTracking_Id  EventTrackingId,
                                        User_Id?          CurrentUserId   = null)

            => WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   JSON,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region LogRemoteParty        (Command, Number,      ...)

        public ValueTask Log(String            Command,
                             Int64             Number,
                             EventTracking_Id  EventTrackingId,
                             User_Id?          CurrentUserId   = null)

            => WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   Number,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region LogRemotePartyComment (Text,                 ...)

        public ValueTask LogRemotePartyComment(String            Text,
                                               EventTracking_Id  EventTrackingId,
                                               User_Id?          CurrentUserId   = null)

            => WriteCommentToDatabase(
                   RemotePartyDBFileName,
                   Text,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion


        #region ReadRemotePartyDatabaseFile (DatabaseFileName = null)

        public void ReadRemotePartyDatabaseFile(String? DatabaseFileName = null)
        {

            ProcessRemotePartyCommands(
                LoadCommandsFromDatabaseFile(DatabaseFileName ?? RemotePartyDBFileName)
            );

        }

        #endregion

        #region ProcessRemotePartyCommands  (Commands)

        public void ProcessRemotePartyCommands(IEnumerable<Command> Commands)
        {

            foreach (var command in Commands)
            {

                String?      errorResponse   = null;
                RemoteParty? remoteParty;

                var errorResponses = new List<Tuple<Command, String>>();

                switch (command.CommandName)
                {

                    #region addRemoteParty

                    case CommonHTTPAPI.addRemoteParty:
                        try
                        {
                            if (command.JSONObject is not null &&
                                RemoteParty.TryParse(command.JSONObject,
                                                     out remoteParty,
                                                     out errorResponse))
                            {
                                remoteParties.TryAdd(remoteParty.Id, remoteParty);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addRemotePartyIfNotExists

                    case CommonHTTPAPI.addRemotePartyIfNotExists:
                        try
                        {
                            if (command.JSONObject is not null &&
                                RemoteParty.TryParse(command.JSONObject,
                                                     out remoteParty,
                                                     out errorResponse))
                            {
                                remoteParties.TryAdd(remoteParty.Id, remoteParty);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addOrUpdateRemoteParty

                    case CommonHTTPAPI.addOrUpdateRemoteParty:
                        try
                        {
                            if (command.JSONObject is not null &&
                                RemoteParty.TryParse(command.JSONObject,
                                                     out remoteParty,
                                                     out errorResponse))
                            {

                                if (remoteParties.ContainsKey(remoteParty.Id))
                                    remoteParties.Remove(remoteParty.Id, out _);

                                remoteParties.TryAdd(remoteParty.Id, remoteParty);

                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateRemoteParty

                    case CommonHTTPAPI.updateRemoteParty:
                        try
                        {
                            if (command.JSONObject is not null &&
                                RemoteParty.TryParse(command.JSONObject,
                                                     out remoteParty,
                                                     out errorResponse))
                            {
                                remoteParties.Remove(remoteParty.Id, out _);
                                remoteParties.TryAdd(remoteParty.Id, remoteParty);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateRemoteParty

                    case CommonHTTPAPI.removeRemoteParty:
                        try
                        {
                            if (command.JSONObject is not null &&
                                RemoteParty.TryParse(command.JSONObject,
                                                     out remoteParty,
                                                     out errorResponse))
                            {
                                remoteParties.Remove(remoteParty.Id, out _);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region removeAllRemoteParties

                    case CommonHTTPAPI.removeAllRemoteParties:
                        remoteParties.Clear();
                        break;

                    #endregion

                }

            }

        }

        #endregion

        #endregion


        #region Read/Write Database Files

        #region WriteToDatabase                          (FileName, Text,   ...)

        public ValueTask WriteToDatabase(String             FileName,
                                         String             Text,
                                         CancellationToken  CancellationToken   = default)

          => logFileWriter.EnqueueAsync(
                 FileName,
                 Text,
                 CancellationToken
             );

        #endregion

        #region WriteToDatabase                          (FileName, JToken, ...)

        public ValueTask WriteToDatabase(String             FileName,
                                         String             Command,
                                         JToken?            JToken,
                                         EventTracking_Id   EventTrackingId,
                                         User_Id?           CurrentUserId       = null,
                                         CancellationToken  CancellationToken   = default)

            => WriteToDatabase(

                   FileName,

                   JSONObject.Create(

                             // Command is always the first property!
                             new JProperty(Command,            JToken),
                             new JProperty("timestamp",        Timestamp.Now.  ToISO8601()),
                             new JProperty("eventTrackingId",  EventTrackingId.ToString()),

                       CurrentUserId is not null
                           ? new JProperty("userId",           CurrentUserId)
                           : null).

                   ToString(Newtonsoft.Json.Formatting.None),

                   CancellationToken

               );

        #endregion

        #region WriteCommentToDatabase                   (FileName, Text,   ...)

        public ValueTask WriteCommentToDatabase(String             FileName,
                                                String             Text,
                                                EventTracking_Id   EventTrackingId,
                                                User_Id?           CurrentUserId       = null,
                                                CancellationToken  CancellationToken   = default)

            => WriteToDatabase(
                   FileName,
                   $"//{Timestamp.Now.ToISO8601()} {EventTrackingId} {(CurrentUserId is not null ? CurrentUserId : "-")}: {Text}{Environment.NewLine}",
                   CancellationToken
               );

        #endregion


        #region LoadCommandsFromDatabaseFile             (DBFileName)

        public IEnumerable<Command> LoadCommandsFromDatabaseFile(String DBFileName)
        {

            try
            {

                return ParseCommands(
                           File.ReadLines(
                               DBFileName,
                               Encoding.UTF8
                           )
                       );

            }
            catch (FileNotFoundException)
            { }
            catch (Exception e)
            {
                DebugX.LogException(e, $"OCPI.CommonAPIBase.ReadDatabaseFile({DBFileName})");
            }

            return [];

        }

        #endregion

        #region ParseCommands                            (Commands)

        public IEnumerable<Command> ParseCommands(IEnumerable<String> Commands)
        {

            try
            {

                var commands = new List<Command>();

                foreach (var line in Commands)
                {

                    try
                    {

                        if (line.StartsWith("//") || line.StartsWith('#'))
                            continue;

                        var json     = JObject.Parse(line);
                        var command  = json.Properties().First();

                        if (     command.Value.Type == JTokenType.Object)
                            commands.Add(
                                new Command(
                                    command.Name,
                                    command.Value as JObject
                                )
                            );

                        else if (command.Value.Type == JTokenType.Array)
                            commands.Add(
                                new Command(
                                    command.Name,
                                    command.Value as JArray
                                )
                            );

                        else if (command.Value.Type == JTokenType.String)
                            commands.Add(
                                new Command(
                                    command.Name,
                                    command.Value<String>()
                                )
                            );

                        else if (command.Value.Type == JTokenType.Integer)
                            commands.Add(
                                new Command(
                                    command.Name,
                                    command.Value<Int64>()
                                )
                            );

                        else if (command.Value.Type == JTokenType.Float)
                            commands.Add(
                                new Command(
                                    command.Name,
                                    command.Value<Single>()
                                )
                            );

                        else if (command.Value.Type == JTokenType.Boolean)
                            commands.Add(
                                new Command(
                                    command.Name,
                                    command.Value<Boolean>()
                                )
                            );

                    }
                    catch (Exception e)
                    {
                        DebugX.LogException(e, $"OCPI.CommonAPIBase.ProcessCommands()");
                    }

                }

                return commands;

            }
            catch
            {
                return [];
            }

        }

        #endregion

        #region LoadCommandsWithMetadataFromDatabaseFile (DBFileName)

        public IEnumerable<CommandWithMetadata> LoadCommandsWithMetadataFromDatabaseFile(String DBFileName)
        {

            try
            {

                return ParseCommandsWithMetadata(
                           File.ReadLines(
                               DBFileName,
                               Encoding.UTF8
                           )
                       );

            }
            catch (Exception e)
            {
                DebugX.LogException(e, $"OCPI.CommonAPIBase.ReadDatabaseFileWithMetadata({DBFileName})");
            }

            return [];

        }

        #endregion

        #region ParseCommandsWithMetadata                (Commands)

        public IEnumerable<CommandWithMetadata> ParseCommandsWithMetadata(IEnumerable<String> Commands)
        {

            try
            {

                var commands = new List<CommandWithMetadata>();

                foreach (var command in Commands)
                {

                    try
                    {

                        if (command.StartsWith("//"))
                            continue;

                        var json             = JObject.Parse(command);
                        var ocpiCommand      = json.Properties().First();
                        var timestamp        = json["timestamp"]?.      Value<DateTime>();
                        var eventtrackingid  = json["eventTrackingId"]?.Value<String>();
                        var eventTrackingId  = eventtrackingid is not null ? EventTracking_Id.Parse(eventtrackingid) : null;
                        var userid           = json["userId"]?.         Value<String>();
                        var userId           = userid          is not null ? User_Id.         Parse(userid)          : new User_Id?();

                        if (timestamp.HasValue &&
                            eventTrackingId is not null)
                        {

                            if (ocpiCommand.Value.Type == JTokenType.String)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value<String>(),
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.Object)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value as JObject,
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.Integer)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value<Int64>(),
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );


                            if (     ocpiCommand.Value.Type == JTokenType.Object)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value as JObject,
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.Array)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value as JArray,
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.String)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value<String>(),
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.Integer)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value<Int64>(),
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.Float)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value<Single>(),
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.Boolean)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value<Boolean>(),
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );


                            else
                                DebugX.Log($"OCPI.CommonAPIBase.ProcessCommandsWithMetadata(): Invalid command: '{command}'!");

                        }
                        else
                            DebugX.Log($"OCPI.CommonAPIBase.ProcessCommandsWithMetadata(): Invalid command: '{command}'!");

                    }
                    catch (Exception e)
                    {
                        DebugX.LogException(e, $"OCPI.CommonAPIBase.ProcessCommandsWithMetadata()");
                    }

                }

                return commands;

            }
            catch
            {
                return [];
            }

        }

        #endregion

        #endregion


    }

}
