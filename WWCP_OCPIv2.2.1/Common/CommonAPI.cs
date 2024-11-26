/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1.CPO.HTTP;
using cloud.charging.open.protocols.OCPIv2_2_1.EMSP.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.HTTP
{

    /// <summary>
    /// A delegate for filtering remote parties.
    /// </summary>
    public delegate Boolean IncludeRemoteParty(RemoteParty RemoteParty);

    public delegate IEnumerable<Tariff_Id>  GetTariffIds2_Delegate(CountryCode    CPOCountryCode,
                                                                   Party_Id       CPOPartyId,
                                                                   Location_Id?   Location      = null,
                                                                   EVSE_UId?      EVSEUId       = null,
                                                                   Connector_Id?  ConnectorId   = null,
                                                                   EMSP_Id?       EMSPId        = null);


    /// <summary>
    /// The Common API.
    /// </summary>
    public class CommonAPI : CommonAPIBase
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServerName     = "GraphDefined OCPI HTTP API v0.1";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName    = "GraphDefined OCPI HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort     = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public new static readonly HTTPPath  DefaultURLPathPrefix      = HTTPPath.Parse("io/OCPI/");

        /// <summary>
        /// The default log file name.
        /// </summary>
        public static readonly     String    DefaultLogfileName        = $"OCPI{Version.Id}-CommonAPI.log";

        /// <summary>
        /// The command values store.
        /// </summary>
        public readonly ConcurrentDictionary<Command_Id, CommandValues> CommandValueStore = new ConcurrentDictionary<Command_Id, CommandValues>();

        #endregion

        #region Properties

        /// <summary>
        /// All our credential roles.
        /// </summary>
        public IEnumerable<CredentialsRole>  OurCredentialRoles         { get; }

        /// <summary>
        /// Whether to keep or delete EVSEs marked as "REMOVED".
        /// </summary>
        public Func<EVSE, Boolean>           KeepRemovedEVSEs           { get; }

        /// <summary>
        /// Disable OCPI v2.1.1.
        /// </summary>
        public Boolean                       Disable_OCPIv2_1_1         { get; }

        /// <summary>
        /// The Common API logger.
        /// </summary>
        public CommonAPILogger?              CommonAPILogger            { get; }

        #endregion

        #region Events

        #region (protected internal) GetVersionsRequest       (Request)

        /// <summary>
        /// An event sent whenever a GET versions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetVersionsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET versions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetVersionsRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnGetVersionsRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) GetVersionsResponse      (Response)

        /// <summary>
        /// An event sent whenever a GET versions response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetVersionsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET versions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetVersionsResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnGetVersionsResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) GetVersionRequest        (Request)

        /// <summary>
        /// An event sent whenever a GET version request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetVersionRequest = new ();

        /// <summary>
        /// An event sent whenever a GET version request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetVersionRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnGetVersionRequest.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) GetVersionResponse       (Response)

        /// <summary>
        /// An event sent whenever a GET version response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetVersionResponse = new ();

        /// <summary>
        /// An event sent whenever a GET version response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetVersionResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnGetVersionResponse.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) GetCredentialsRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCredentialsRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnGetCredentialsRequest.WhenAll(Timestamp,
                                               API ?? this,
                                               Request);

        #endregion

        #region (protected internal) GetCredentialsResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCredentialsResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnGetCredentialsResponse.WhenAll(Timestamp,
                                                API ?? this,
                                                Request,
                                                Response);

        #endregion


        #region (protected internal) PostCredentialsRequest   (Request)

        /// <summary>
        /// An event sent whenever a POST credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a POST credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostCredentialsRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnPostCredentialsRequest.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region (protected internal) PostCredentialsResponse  (Response)

        /// <summary>
        /// An event sent whenever a POST credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a POST credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostCredentialsResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        OCPIResponse  Response)

            => OnPostCredentialsResponse.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion


        #region (protected internal) PutCredentialsRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a PUT credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutCredentialsRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnPutCredentialsRequest.WhenAll(Timestamp,
                                               API ?? this,
                                               Request) ?? Task.CompletedTask;

        #endregion

        #region (protected internal) PutCredentialsResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a PUT credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutCredentialsResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnPutCredentialsResponse.WhenAll(Timestamp,
                                                API ?? this,
                                                Request,
                                                Response) ?? Task.CompletedTask;

        #endregion


        #region (protected internal) DeleteCredentialsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCredentialsRequest(DateTime     Timestamp,
                                                         HTTPAPI      API,
                                                         OCPIRequest  Request)

            => OnDeleteCredentialsRequest.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request);

        #endregion

        #region (protected internal) DeleteCredentialsResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCredentialsResponse(DateTime      Timestamp,
                                                          HTTPAPI       API,
                                                          OCPIRequest   Request,
                                                          OCPIResponse  Response)

            => OnDeleteCredentialsResponse.WhenAll(Timestamp,
                                                   API ?? this,
                                                   Request,
                                                   Response);

        #endregion

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<Tariff>?               CustomTariffSerializer                 { get; }
        public CustomJObjectSerializerDelegate<DisplayText>?          CustomDisplayTextSerializer            { get; }
        public CustomJObjectSerializerDelegate<Price>?                CustomPriceSerializer                  { get; }
        public CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer          { get; }
        public CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer         { get; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?   CustomTariffRestrictionsSerializer     { get; }
        public CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer              { get; }
        public CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer           { get; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer    { get; }

        #endregion

        #region Constructor(s)

        #region CommonAPI(HTTPServerName, ...)

        /// <summary>
        /// Create a new common HTTP API.
        /// </summary>
        /// <param name="OurVersionsURL">The URL of our VERSIONS endpoint.</param>
        /// <param name="OurCredentialRoles">All our credential roles.</param>
        /// <param name="APIVersionHashes">The API version hashes (git commit hash values).</param>
        /// 
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="HTTPServerPort">An optional HTTP TCP port.</param>
        /// <param name="HTTPServerName">An optional HTTP server name.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional HTTP URL path prefix.</param>
        /// <param name="HTTPServiceName">An optional HTTP service name.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        /// 
        /// <param name="KeepRemovedEVSEs">Whether to keep or delete EVSEs marked as "REMOVED" (default: keep).</param>
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        public CommonAPI(URL                                                        OurBaseURL,
                         URL                                                        OurVersionsURL,
                         IEnumerable<CredentialsRole>                               OurCredentialRoles,

                         HTTPPath?                                                  AdditionalURLPathPrefix      = null,
                         Func<EVSE, Boolean>?                                       KeepRemovedEVSEs             = null,
                         Boolean                                                    LocationsAsOpenData          = true,
                         Boolean?                                                   AllowDowngrades              = null,
                         Boolean                                                    Disable_RootServices         = true,
                         Boolean                                                    Disable_OCPIv2_1_1           = true,

                         HTTPHostname?                                              HTTPHostname                 = null,
                         String?                                                    ExternalDNSName              = null,
                         IPPort?                                                    HTTPServerPort               = null,
                         HTTPPath?                                                  BasePath                     = null,
                         String?                                                    HTTPServerName               = DefaultHTTPServerName,

                         HTTPPath?                                                  URLPathPrefix                = null,
                         String?                                                    HTTPServiceName              = DefaultHTTPServiceName,
                         JObject?                                                   APIVersionHashes             = null,

                         ServerCertificateSelectorDelegate?                         ServerCertificateSelector    = null,
                         RemoteTLSClientCertificateValidationHandler<IHTTPServer>?  ClientCertificateValidator   = null,
                         LocalCertificateSelectionHandler?                          LocalCertificateSelector    = null,
                         SslProtocols?                                              AllowedTLSProtocols          = null,
                         Boolean?                                                   ClientCertificateRequired    = null,
                         Boolean?                                                   CheckCertificateRevocation   = null,

                         ServerThreadNameCreatorDelegate?                           ServerThreadNameCreator      = null,
                         ServerThreadPriorityDelegate?                              ServerThreadPrioritySetter   = null,
                         Boolean?                                                   ServerThreadIsBackground     = null,
                         ConnectionIdBuilder?                                       ConnectionIdBuilder          = null,
                         TimeSpan?                                                  ConnectionTimeout            = null,
                         UInt32?                                                    MaxClientConnections         = null,

                         Boolean?                                                   DisableMaintenanceTasks      = null,
                         TimeSpan?                                                  MaintenanceInitialDelay      = null,
                         TimeSpan?                                                  MaintenanceEvery             = null,

                         Boolean?                                                   DisableWardenTasks           = null,
                         TimeSpan?                                                  WardenInitialDelay           = null,
                         TimeSpan?                                                  WardenCheckEvery             = null,

                         Boolean?                                                   IsDevelopment                = null,
                         IEnumerable<String>?                                       DevelopmentServers           = null,
                         Boolean?                                                   DisableLogging               = null,
                         String?                                                    LoggingContext               = null,
                         String?                                                    LoggingPath                  = null,
                         String?                                                    LogfileName                  = null,
                         OCPILogfileCreatorDelegate?                                LogfileCreator               = null,
                         String?                                                    DatabaseFilePath             = null,
                         String?                                                    RemotePartyDBFileName        = null,
                         String?                                                    AssetsDBFileName             = null,
                         DNSClient?                                                 DNSClient                    = null,
                         Boolean                                                    AutoStart                    = false)


            : base(Version.Id,
                   OurBaseURL,
                   OurVersionsURL,

                   AdditionalURLPathPrefix,
                   LocationsAsOpenData,
                   AllowDowngrades,
                   Disable_RootServices,

                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServerPort,
                   BasePath,
                   HTTPServerName,

                   URLPathPrefix,
                   HTTPServiceName,
                   APIVersionHashes,

                   ServerCertificateSelector,
                   ClientCertificateValidator,
                   LocalCertificateSelector,
                   AllowedTLSProtocols,
                   ClientCertificateRequired,
                   CheckCertificateRevocation,

                   ServerThreadNameCreator,
                   ServerThreadPrioritySetter,
                   ServerThreadIsBackground,
                   ConnectionIdBuilder,
                   ConnectionTimeout,
                   MaxClientConnections,

                   DisableMaintenanceTasks,
                   MaintenanceInitialDelay,
                   MaintenanceEvery,

                   DisableWardenTasks,
                   WardenInitialDelay,
                   WardenCheckEvery,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingContext,
                   LoggingPath,
                   LogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, remotePartyId, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null,
                   DatabaseFilePath,
                   RemotePartyDBFileName,
                   AssetsDBFileName,
                   DNSClient,
                   AutoStart)

        {

            if (!OurCredentialRoles.SafeAny())
                throw new ArgumentNullException(nameof(OurCredentialRoles), "The given credential roles must not be null or empty!");

            this.OurCredentialRoles    = OurCredentialRoles.Distinct();

            this.KeepRemovedEVSEs      = KeepRemovedEVSEs ?? (evse => true);

            this.Disable_OCPIv2_1_1    = Disable_OCPIv2_1_1;

            this.locations             = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id, Location>>>();
            this.chargingSessions      = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Session_Id,  Session>>>();
            this.tokenStatus           = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Token_Id,    TokenStatus>>>();
            this.ChargeDetailRecords   = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<CDR_Id,      CDR>>>();

            this.CommonAPILogger       = this.DisableLogging == false
                                             ? null
                                             : new CommonAPILogger(
                                                   this,
                                                   LoggingContext,
                                                   LoggingPath,
                                                   LogfileCreator
                                               );

            if (!this.DisableLogging)
                ReadRemotePartyDatabaseFile();

            RegisterURLTemplates();

        }

        #endregion

        #region CommonAPI(HTTPServer, ...)

        /// <summary>
        /// Create a new common HTTP API.
        /// </summary>
        /// <param name="OurVersionsURL">The URL of our VERSIONS endpoint.</param>
        /// <param name="OurCredentialRoles">All our credential roles.</param>
        /// 
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        /// 
        /// <param name="KeepRemovedEVSEs">Whether to keep or delete EVSEs marked as "REMOVED" (default: keep).</param>
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        public CommonAPI(URL                           OurBaseURL,
                         URL                           OurVersionsURL,
                         IEnumerable<CredentialsRole>  OurCredentialRoles,

                         HTTPServer                    HTTPServer,
                         HTTPHostname?                 HTTPHostname              = null,
                         String?                       ExternalDNSName           = null,
                         HTTPPath?                     URLPathPrefix             = null,
                         HTTPPath?                     BasePath                  = null,
                         String?                       HTTPServiceName           = DefaultHTTPServerName,

                         HTTPPath?                     AdditionalURLPathPrefix   = null,
                         Func<EVSE, Boolean>?          KeepRemovedEVSEs          = null,
                         Boolean                       LocationsAsOpenData       = true,
                         Boolean?                      AllowDowngrades           = null,
                         Boolean                       Disable_RootServices      = false,
                         Boolean                       Disable_OCPIv2_1_1        = true,

                         JObject?                      APIVersionHashes          = null,

                         Boolean?                      DisableMaintenanceTasks   = false,
                         TimeSpan?                     MaintenanceInitialDelay   = null,
                         TimeSpan?                     MaintenanceEvery          = null,

                         Boolean?                      DisableWardenTasks        = false,
                         TimeSpan?                     WardenInitialDelay        = null,
                         TimeSpan?                     WardenCheckEvery          = null,

                         Boolean?                      IsDevelopment             = false,
                         IEnumerable<String>?          DevelopmentServers        = null,
                         Boolean?                      DisableLogging            = false,
                         String?                       LoggingContext            = null,
                         String?                       LoggingPath               = null,
                         String?                       LogfileName               = null,
                         OCPILogfileCreatorDelegate?   LogfileCreator            = null,
                         String?                       DatabaseFilePath          = null,
                         String?                       RemotePartyDBFileName     = null,
                         String?                       AssetsDBFileName          = null,
                         Boolean                       AutoStart                 = false)

            : base(Version.Id,
                   OurBaseURL,
                   OurVersionsURL,
                   HTTPServer,

                   AdditionalURLPathPrefix,
                   LocationsAsOpenData,
                   AllowDowngrades,
                   Disable_RootServices,

                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServiceName,
                   BasePath,

                   URLPathPrefix,
                   APIVersionHashes,

                   DisableMaintenanceTasks,
                   MaintenanceInitialDelay,
                   MaintenanceEvery,

                   DisableWardenTasks,
                   WardenInitialDelay,
                   WardenCheckEvery,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingContext,
                   LoggingPath,
                   LogfileName ?? DefaultLogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, remotePartyId, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null,
                   DatabaseFilePath,
                   RemotePartyDBFileName,
                   AssetsDBFileName,
                   AutoStart)

        {

            if (!OurCredentialRoles.SafeAny())
                throw new ArgumentNullException(nameof(OurCredentialRoles), "The given credential roles must not be null or empty!");

            this.OurCredentialRoles    = OurCredentialRoles?.Distinct() ?? Array.Empty<CredentialsRole>();

            this.KeepRemovedEVSEs      = KeepRemovedEVSEs ?? (evse => true);

            this.Disable_OCPIv2_1_1    = Disable_OCPIv2_1_1;

            this.locations             = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id, Location>>>();
            this.chargingSessions      = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Session_Id,  Session>>>();
            this.tokenStatus           = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Token_Id,    TokenStatus>>>();
            this.ChargeDetailRecords   = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<CDR_Id,      CDR>>>();

            // Link HTTP events...
            HTTPServer.RequestLog     += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog    += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog       += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            this.CommonAPILogger       = this.DisableLogging == false
                                             ? new CommonAPILogger(
                                                   this,
                                                   LoggingContext,
                                                   LoggingPath,
                                                   LogfileCreator
                                               )
                                             : null;

            if (!this.DisableLogging)
                ReadRemotePartyDatabaseFile();

            RegisterURLTemplates();

        }

        #endregion

        #endregion


        #region GetModuleURL(ModuleId, Prefix = "")

        /// <summary>
        /// Return the URL of an OCPI module.
        /// </summary>
        /// <param name="ModuleId">The identification of an OCPI module.</param>
        /// <param name="Prefix">An optional prefix.</param>
        public URL GetModuleURL(Module_Id  ModuleId,
                                String     Prefix   = "")

            => OurBaseURL + Prefix + ModuleId.ToString();

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region OPTIONS     ~/

            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix,
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.GET
                                                                                  },
                                                     AccessControlAllowHeaders  = new[] { "Authorization" },
                                                     Connection                 = ConnectionType.Close
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET         ~/

            //HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
            //                                   URLPrefix + "/", "cloud.charging.open.protocols.OCPIv2_2_1.HTTPAPI.CommonAPI.HTTPRoot",
            //                                   Assembly.GetCallingAssembly());

            //this.AddMethodCallback(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             new HTTPPath[] {
            //                                 URLPrefix + "/index.html",
            //                                 URLPrefix + "/"
            //                             },
            //                             HTTPContentType.Text.HTML_UTF8,
            //                             HTTPDelegate: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2_1.HTTPAPI.CommonAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2_1.HTTPAPI.CommonAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

            //                                 return new HTTPResponse.Builder(Request) {
            //                                     HTTPStatusCode  = HTTPStatusCode.OK,
            //                                     ContentType     = HTTPContentType.Text.HTML_UTF8,
            //                                     Content         = _MemoryStream.ToArray(),
            //                                     Connection      = ConnectionType.Close
            //                                 };

            //                             });

            #region Text

            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix,
                                         HTTPContentType.Text.PLAIN,
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                     AccessControlAllowHeaders  = new[] { "Authorization" },
                                                     ContentType                = HTTPContentType.Text.PLAIN,
                                                     Content                    = "This is an Open Charge Point Interface HTTP service!\r\nPlease check ~/versions!".ToUTF8Bytes(),
                                                     Connection                 = ConnectionType.Close
                                                 }.AsImmutable);

                                         });

            #endregion

            #endregion


            #region OPTIONS     ~/versions

            // ----------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions
            // ----------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.OPTIONS,
                               URLPathPrefix + "versions",
                               OCPIRequestHandler: Request => {

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                               Allow                      = new List<HTTPMethod> {
                                                                                HTTPMethod.OPTIONS,
                                                                                HTTPMethod.GET
                                                                            },
                                               AccessControlAllowHeaders  = new[] { "Authorization" },
                                               Vary                       = "Accept"
                                           }
                                       });

                               });

            #endregion

            #region GET         ~/versions

            // ----------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions
            // ----------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.GET,
                               URLPathPrefix + "versions",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   GetVersionsRequest,
                               OCPIResponseLogger:  GetVersionsResponse,
                               OCPIRequestHandler:  Request => {

                                   #region Check access token

                                   if (Request.LocalAccessInfo is not null &&
                                       Request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Invalid or blocked access token!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                  AccessControlAllowHeaders  = new[] { "Authorization" }
                                              }
                                          });

                                   }

                                   #endregion

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           StatusCode           = 1000,
                                           StatusMessage        = "Hello world!",
                                           Data                 = new JArray(
                                                                      new VersionInformation?[] {
                                                                          Disable_OCPIv2_1_1
                                                                              ? null
                                                                              : new VersionInformation(
                                                                                    Version_Id.Parse("2.1.1"),
                                                                                    URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                              (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + "/versions/2.1.1")).Replace("//", "/"))
                                                                                ),
                                                                          new VersionInformation(
                                                                              Version.Id,
                                                                              URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                        (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + $"/versions/{Version.Id}")).Replace("//", "/"))
                                                                          )
                                                                      }.Where (version => version is not null).
                                                                        Select(version => version?.ToJSON())
                                                                  ),
                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                               AccessControlAllowHeaders  = new[] { "Authorization" },
                                               Vary                       = "Accept"
                                           }
                                       });

                               });

            #endregion


            #region OPTIONS     ~/versions/{versionId}

            // --------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions/{id}
            // --------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.OPTIONS,
                               URLPathPrefix + "versions/{versionId}",
                               OCPIRequestHandler: Request => {

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                               Allow                      = new List<HTTPMethod> {
                                                                                HTTPMethod.OPTIONS,
                                                                                HTTPMethod.GET
                                                                            },
                                               AccessControlAllowHeaders  = new[] { "Authorization" },
                                               Vary                       = "Accept"
                                           }
                                       });
                               });

            #endregion

            #region GET         ~/versions/{versionId}

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions/{id}
            // ---------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.GET,
                               URLPathPrefix + "versions/{versionId}",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   GetVersionRequest,
                               OCPIResponseLogger:  GetVersionResponse,
                               OCPIRequestHandler:  Request => {

                                   #region Check access token

                                   if (Request.LocalAccessInfo is not null &&
                                       Request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Invalid or blocked access token!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                  AccessControlAllowHeaders  = new[] { "Authorization" }
                                              }
                                          });

                                   }

                                   #endregion

                                   #region Get version identification URL parameter

                                   if (Request.ParsedURLParameters.Length < 1)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Version identification is missing!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                  AccessControlAllowHeaders  = new[] { "Authorization" }
                                              }
                                          });

                                   }

                                   if (!Version_Id.TryParse(Request.ParsedURLParameters[0], out var versionId))
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Version identification is invalid!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                  AccessControlAllowHeaders  = new[] { "Authorization" }
                                              }
                                          });

                                   }

                                   #endregion

                                   #region Only allow OCPI version v2.2.1

                                   if (versionId.ToString() != Version.Id.ToString())
                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "This OCPI version is not supported!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                  AccessControlAllowHeaders  = new[] { "Authorization" }
                                              }
                                          });

                                   #endregion


                                   var prefix = URLPathPrefix + AdditionalURLPathPrefix + $"v{versionId}";

                                   #region Common credential endpoints...

                                   var endpoints  = new List<VersionEndpoint>() {

                                                        new VersionEndpoint(Module_Id.Credentials,
                                                                            InterfaceRoles.SENDER,
                                                                            URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                      (Request.Host + (prefix + "credentials")).Replace("//", "/"))),

                                                        new VersionEndpoint(Module_Id.Credentials,
                                                                            InterfaceRoles.RECEIVER,
                                                                            URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                      (Request.Host + (prefix + "credentials")).Replace("//", "/")))

                                                    };

                                   #endregion


                                   #region The other side is a CPO...

                                   if (Request.RemoteParty?.Roles.Any(credentialsRole => credentialsRole.Role == Roles.CPO) == true)
                                   {

                                       endpoints.Add(new VersionEndpoint(Module_Id.Locations,
                                                                         InterfaceRoles.RECEIVER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") + 
                                                                                   (Request.Host + (prefix + "emsp/locations")).Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tariffs,
                                                                         InterfaceRoles.RECEIVER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/tariffs")).  Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Sessions,
                                                                         InterfaceRoles.RECEIVER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/sessions")). Replace("//", "/"))));

                                       // When the EMSP acts as smart charging receiver so that a SCSP can talk to him!
                                       endpoints.Add(new VersionEndpoint(Module_Id.ChargingProfiles,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/chargingprofiles")).Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.CDRs,
                                                                         InterfaceRoles.RECEIVER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/cdrs")).     Replace("//", "/"))));


                                       endpoints.Add(new VersionEndpoint(Module_Id.Commands,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/commands")). Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tokens,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/tokens")).   Replace("//", "/"))));

                                       // hubclientinfo

                                   }

                                   #endregion

                                   #region We are a CPO, the other side is unauthenticated and we export locations as Open Data...

                                   if (OurCredentialRoles.Any(credentialRole => credentialRole.Role == Roles.CPO) &&
                                       LocationsAsOpenData &&
                                       Request.RemoteParty is null)
                                   {

                                       endpoints.Add(new VersionEndpoint(Module_Id.Locations,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/locations")).Replace("//", "/"))));

                                   }

                                   #endregion

                                   #region The other side is an EMSP...

                                   if (Request.RemoteParty?.Roles.Any(credentialsRole => credentialsRole.Role == Roles.EMSP) == true)
                                   {

                                       endpoints.Add(new VersionEndpoint(Module_Id.Locations,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/locations")).       Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tariffs,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/tariffs")).         Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Sessions,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/sessions")).        Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.ChargingProfiles,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/chargingprofiles")).Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.CDRs,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/cdrs")).            Replace("//", "/"))));


                                       endpoints.Add(new VersionEndpoint(Module_Id.Commands,
                                                                         InterfaceRoles.RECEIVER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/commands")).        Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tokens,
                                                                         InterfaceRoles.RECEIVER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/tokens")).          Replace("//", "/"))));

                                       // hubclientinfo

                                   }

                                   #endregion


                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                               StatusCode           = 1000,
                                               StatusMessage        = "Hello world!",
                                               Data                 = new VersionDetail(
                                                                          versionId,
                                                                          endpoints
                                                                      ).ToJSON(),
                                               HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                                   AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                   AccessControlAllowHeaders  = new[] { "Authorization" },
                                                   Vary                       = "Accept"
                                               }
                                           });

                               });

            #endregion


            #region OPTIONS     ~/{versionId}/credentials

            // ----------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/2.2/credentials
            // ----------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.OPTIONS,
                               URLPathPrefix + "{versionId}/credentials",
                               OCPIRequestHandler: Request => {

                                   #region Defaults

                                   var accessControlAllowMethods  = new List<String> {
                                                                        "OPTIONS",
                                                                        "GET"
                                                                    };

                                   var allow                      = new List<HTTPMethod> {
                                                                        HTTPMethod.OPTIONS,
                                                                        HTTPMethod.GET
                                                                    };

                                   #endregion

                                   #region Check the access token whether the client is known, and its access is allowed!

                                   if (Request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                                   {

                                       accessControlAllowMethods.Add("POST");

                                       allow.Add(HTTPMethod.POST);

                                       // Only when the party is fully registered!
                                       if (Request.LocalAccessInfo?.VersionsURL.HasValue == true)
                                       {

                                           accessControlAllowMethods.Add("PUT");
                                           accessControlAllowMethods.Add("DELETE");

                                           allow.Add(HTTPMethod.PUT);
                                           allow.Add(HTTPMethod.DELETE);

                                       }

                                    }

                                   #endregion


                                   return Task.FromResult(
                                              new OCPIResponse.Builder(Request) {
                                                  StatusCode           = 1000,
                                                  StatusMessage        = "Hello world!",
                                                  HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                      HTTPStatusCode             = HTTPStatusCode.OK,
                                                      AccessControlAllowHeaders  = new[] { "Authorization" },
                                                      AccessControlAllowMethods  = accessControlAllowMethods,
                                                      Allow                      = allow
                                                  }
                                              });

                               });

            #endregion

            #region GET         ~/{versionId}/credentials

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.2/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.GET,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   GetCredentialsRequest,
                               OCPIResponseLogger:  GetCredentialsResponse,
                               OCPIRequestHandler:  Request => {

                                   #region Check access token

                                   if (Request.LocalAccessInfo is not null &&
                                       Request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Invalid or blocked access token!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                  AccessControlAllowHeaders  = new[] { "Authorization" }
                                              }
                                          });

                                   }

                                   #endregion

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           StatusCode           = 1000,
                                           StatusMessage        = "Hello world!",
                                           Data                 = new Credentials(
                                                                      Request.LocalAccessInfo?.AccessToken ?? AccessToken.Parse("<any>"),
                                                                      OurVersionsURL,
                                                                      OurCredentialRoles
                                                                  ).ToJSON(),
                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                               AccessControlAllowHeaders  = new[] { "Authorization" }
                                           }
                                       });

                               });

            #endregion

            #region POST        ~/{versionId}/credentials

            // REGISTER new OCPI party!

            // -----------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.2/credentials
            // -----------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.POST,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   PostCredentialsRequest,
                               OCPIResponseLogger:  PostCredentialsResponse,
                               OCPIRequestHandler:  async Request => {

                                   if (Request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                                   {

                                       if (Request.LocalAccessInfo.VersionsURL.HasValue)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,                                              // CREDENTIALS_TOKEN_A
                                                      StatusMessage        = $"The given access token '{Request.LocalAccessInfo.AccessToken}' is already registered!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                          AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                          AccessControlAllowHeaders  = new[] { "Authorization" }
                                                      }
                                                  };

                                       return await POSTOrPUTCredentials(Request);

                                   }

                                   return new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                  AccessControlAllowHeaders  = new[] { "Authorization" }
                                              }
                                          };

                               });

            #endregion

            #region PUT         ~/{versionId}/credentials

            // UPDATE the registration of an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.2/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.PUT,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   PutCredentialsRequest,
                               OCPIResponseLogger:  PutCredentialsResponse,
                               OCPIRequestHandler:  async Request => {

                                   #region The access token is known...

                                   if (Request.LocalAccessInfo is not null)
                                   {

                                       #region ...but access is blocked!

                                       if (Request.LocalAccessInfo?.Status == AccessStatus.BLOCKED)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,
                                                      StatusMessage        = "The given access token '" + (Request.AccessToken?.ToString() ?? "") + "' is blocked!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                          AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                          AccessControlAllowHeaders  = new[] { "Authorization" }
                                                      }
                                                  };

                                       #endregion

                                       #region ...and access is allowed, but maybe not yet full registered!

                                       if (Request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                                       {

                                           // The party is not yet fully registered!
                                           if (!Request.LocalAccessInfo?.VersionsURL.HasValue == true)
                                               return new OCPIResponse.Builder(Request) {
                                                          StatusCode           = 2000,
                                                          StatusMessage        = "The given access token '" + (Request.AccessToken?.ToString() ?? "") + "' is not yet registered!",
                                                          HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                              HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                              AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST" },
                                                              AccessControlAllowHeaders  = new[] { "Authorization" }
                                                          }
                                                      };

                                           return await POSTOrPUTCredentials(Request);

                                       }

                                       #endregion

                                   }

                                   #endregion

                                   return new OCPIResponse.Builder(Request) {
                                                  StatusCode           = 2000,
                                                  StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                                  HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                      HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                      AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                      AccessControlAllowHeaders  = new[] { "Authorization" }
                                                  }
                                              };

                               });

            #endregion

            #region DELETE      ~/{versionId}/credentials

            // UNREGISTER an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.2/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.DELETE,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   DeleteCredentialsRequest,
                               OCPIResponseLogger:  DeleteCredentialsResponse,
                               OCPIRequestHandler:  async Request => {

                                   if (Request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                                   {

                                       #region Validations

                                       if (!Request.LocalAccessInfo.VersionsURL.HasValue)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,
                                                      StatusMessage        = $"The given access token '{Request.LocalAccessInfo.AccessToken}' is not fully registered!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                          AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                          AccessControlAllowHeaders  = new[] { "Authorization" }
                                                      }
                                                  };

                                       #endregion

                                       await RemoveAccessToken(Request.LocalAccessInfo.AccessToken);

                                       return new OCPIResponse.Builder(Request) {
                                                  StatusCode           = 1000,
                                                  StatusMessage        = $"The given access token '{Request.LocalAccessInfo.AccessToken}' was deleted!",
                                                  HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                      HTTPStatusCode             = HTTPStatusCode.OK,
                                                      AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                      AccessControlAllowHeaders  = new[] { "Authorization" }
                                                  }
                                              };

                                   }

                                   return new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                  AccessControlAllowHeaders  = new[] { "Authorization" }
                                              }
                                          };

                               });

            #endregion

        }

        #endregion


        #region (private) POSTOrPUTCredentials(Request)

        private async Task<OCPIResponse.Builder> POSTOrPUTCredentials(OCPIRequest Request)
        {

            #region Validate CREDENTIALS_TOKEN_A

            var CREDENTIALS_TOKEN_A     = Request.AccessToken;

            if (!CREDENTIALS_TOKEN_A.HasValue)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = "The received credential token must not be null!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = new[] { "Authorization" }
                           }
                       };

            #endregion

            #region Validate old remote party

            var oldRemoteParty = GetRemoteParties(remoteParty => remoteParty.LocalAccessInfos.Any(accessInfoStatus => accessInfoStatus.AccessToken == CREDENTIALS_TOKEN_A.Value)).FirstOrDefault();

            if (oldRemoteParty is null)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = $"There is no remote party having the given access token '{CREDENTIALS_TOKEN_A}'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = new[] { "Authorization" }
                           }
                       };

            #endregion

            #region Parse JSON

            var errorResponse = String.Empty;

            if (!Request.TryParseJObjectRequestBody(out var JSON,
                                                    out var responseBuilder,
                                                    AllowEmptyHTTPBody: false))
            {
                return responseBuilder;
            }

            if (!Credentials.TryParse(JSON,
                                      out var receivedCredentials,
                                      out errorResponse) ||
                receivedCredentials is null)
            {

                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = "Could not parse the received credentials JSON object: " + errorResponse,
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = new[] { "Authorization" }
                           }
                       };

            }

            #endregion

            #region Additional security checks... (Non-Standard)

            //lock (AccessTokens)
            //{
            //    foreach (var credentialsRole in receivedCredentials.Roles)
            //    {

            //        var result = AccessTokens.Values.Where(accessToken => accessToken.Roles.Any(role => role.CountryCode == credentialsRole.CountryCode &&
            //                                                                                            role.PartyId     == credentialsRole.PartyId &&
            //                                                                                            role.Role        == credentialsRole.Role)).ToArray();

            //        if (result.Length == 0)
            //        {

            //            return new OCPIResponse.Builder(Request) {
            //                       StatusCode           = 2000,
            //                       StatusMessage        = "The given combination of country code, party identification and role is unknown!",
            //                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
            //                           HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
            //                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
            //                           AccessControlAllowHeaders  = new[] { "Authorization" }
            //                       }
            //                   };

            //        }

            //        if (result.Length > 0 &&
            //            result.First().VersionsURL.HasValue)
            //        {

            //            return new OCPIResponse.Builder(Request) {
            //                       StatusCode           = 2000,
            //                       StatusMessage        = "The given combination of country code, party identification and role is already registered!",
            //                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
            //                           HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
            //                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
            //                           AccessControlAllowHeaders  = new[] { "Authorization" }
            //                       }
            //                   };

            //        }

            //    }
            //}

            #endregion


            var commonClient              = new CommonClient(this,
                                                             receivedCredentials.URL,
                                                             receivedCredentials.Token,  // CREDENTIALS_TOKEN_B
                                                             DNSClient: HTTPServer.DNSClient);

            var otherVersions             = await commonClient.GetVersions();

            #region ...or send error!

            if (otherVersions.StatusCode != 1000)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = "Could not fetch VERSIONS information from '" + receivedCredentials.URL + "'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = new[] { "Authorization" }
                           }
                       };

            #endregion

            var justMySupportedVersion    = otherVersions.Data?.Where(version => version.Id == Version.Id).ToArray() ?? Array.Empty<VersionInformation>();

            #region ...or send error!

            if (justMySupportedVersion.Length == 0)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 3003,
                           StatusMessage        = $"Could not find {Version.String} at '{receivedCredentials.URL}'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = new[] { "Authorization" }
                           }
                       };

            #endregion

            var otherVersion2_2_1Details  = await commonClient.GetVersionDetails(Version.Id);

            #region ...or send error!

            if (otherVersion2_2_1Details.StatusCode != 1000)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 3001,
                           StatusMessage        = $"Could not fetch {Version.String} information from '{justMySupportedVersion.First().URL}'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = new[] { "Authorization" }
                           }
                       };

            #endregion


            #region Validate, that neither the credentials roles had not been changed!

            if (oldRemoteParty.Roles.Count() != receivedCredentials.Roles.Count())
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = $"Updating the number of credentials roles from '{oldRemoteParty.Roles.Count()}' to '{receivedCredentials.Roles.Count()}' is not allowed!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = new[] { "Authorization" }
                           }
                       };

            foreach (var receivedCredentialsRole in receivedCredentials.Roles)
            {

                CredentialsRole? existingCredentialsRole = null;

                foreach (var oldCredentialsRole in oldRemoteParty.Roles)
                {
                    if (oldCredentialsRole.CountryCode == receivedCredentialsRole.CountryCode &&
                        oldCredentialsRole.PartyId     == receivedCredentialsRole.PartyId &&
                        oldCredentialsRole.Role        == receivedCredentialsRole.Role)
                    {
                        existingCredentialsRole = receivedCredentialsRole;
                        break;
                    }
                }

                if (existingCredentialsRole is null)
                    return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = $"Updating the credentials roles is not allowed!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = new[] { "Authorization" }
                           }
                       };

            }

            #endregion

            var CREDENTIALS_TOKEN_C       = AccessToken.NewRandom();

            // Remove the old access token
            await RemoveAccessToken     (CREDENTIALS_TOKEN_A.Value);

            // Store credential of the other side!
            await AddOrUpdateRemoteParty(oldRemoteParty.Id,
                                         receivedCredentials.Roles,

                                         CREDENTIALS_TOKEN_C,

                                         receivedCredentials.Token,
                                         receivedCredentials.URL,
                                         otherVersions.Data?.Select(version => version.Id) ?? Array.Empty<Version_Id>(),
                                         Version.Id,

                                         null,
                                         null,
                                         null,
                                         null,
                                         AccessStatus.      ALLOWED,
                                         RemoteAccessStatus.ONLINE,
                                         PartyStatus.       ENABLED);


            return new OCPIResponse.Builder(Request) {
                           StatusCode           = 1000,
                           StatusMessage        = "Hello world!",
                           Data                 = new Credentials(
                                                      CREDENTIALS_TOKEN_C,
                                                      OurVersionsURL,
                                                      OurCredentialRoles
                                                  ).ToJSON(),
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.OK,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = new[] { "Authorization" }
                           }
                       };

        }

        #endregion


        //ToDo: Wrap the following into a plugable interface!

        #region AccessTokens

        // An access token might be used by more than one CountryCode + PartyId + Role combination!

        #region RemoveAccessToken(AccessToken)

        public async Task<CommonAPI> RemoveAccessToken(AccessToken        AccessToken,
                                                       EventTracking_Id?  EventTrackingId   = null,
                                                       User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in remoteParties.Values.Where(party => party.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken == AccessToken)))
            {

                #region The remote party has only a single local access token, or...

                if (remoteParty.LocalAccessInfos.Count() <= 1)
                {

                    remoteParties.TryRemove(remoteParty.Id, out _);

                    await LogRemoteParty(removeRemoteParty,
                                         remoteParty.ToJSON(true),
                                         EventTrackingId ?? EventTracking_Id.New,
                                         CurrentUserId);

                }

                #endregion

                #region ...the remote party has multiple local access tokens!

                else
                {

                    remoteParties.TryRemove(remoteParty.Id, out _);

                    var newRemoteParty = new RemoteParty(
                                                remoteParty.Id,
                                                remoteParty.Roles,
                                                remoteParty.LocalAccessInfos.Where(localAccessInfo => localAccessInfo.AccessToken != AccessToken),
                                                remoteParty.RemoteAccessInfos,
                                                remoteParty.Status
                                            );

                    if (remoteParties.TryAdd(newRemoteParty.Id,
                                             newRemoteParty))
                    {

                        await LogRemoteParty(updateRemoteParty,
                                             newRemoteParty.ToJSON(true),
                                             EventTrackingId ?? EventTracking_Id.New,
                                             CurrentUserId);

                    }

                }

                #endregion

            }

            return this;

        }

        #endregion

        #region TryGetLocalAccessInfo(AccessToken, out LocalAccessInfo)

        public Boolean TryGetLocalAccessInfo(AccessToken AccessToken, out LocalAccessInfo? LocalAccessInfo)
        {

            var accessInfos = remoteParties.Values.Where     (remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken == AccessToken)).
                                                   SelectMany(remoteParty => remoteParty.LocalAccessInfos).
                                                   ToArray();

            if (accessInfos.Length == 1)
            {
                LocalAccessInfo = accessInfos.First();
                return true;
            }

            LocalAccessInfo = default;
            return false;

        }

        #endregion

        #region GetLocalAccessInfos(AccessToken)

        public IEnumerable<LocalAccessInfo> GetLocalAccessInfos(AccessToken AccessToken)

            => remoteParties.Values.Where     (remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken == AccessToken)).
                                    SelectMany(remoteParty => remoteParty.LocalAccessInfos);

        #endregion

        #region GetLocalAccessInfos(AccessToken, AccessStatus)

        public IEnumerable<LocalAccessInfo> GetLocalAccessInfos(AccessToken   AccessToken,
                                                                AccessStatus  AccessStatus)

            => remoteParties.Values.Where     (remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken  == AccessToken &&
                                                                                                             accessInfo.Status       == AccessStatus)).
                                    SelectMany(remoteParty => remoteParty.LocalAccessInfos);

        #endregion

        #endregion

        #region RemoteParties

        #region Data

        private readonly ConcurrentDictionary<RemoteParty_Id, RemoteParty> remoteParties = new ();

        /// <summary>
        /// Return an enumeration of all remote parties.
        /// </summary>
        public IEnumerable<RemoteParty> RemoteParties
            => remoteParties.Values;

        #endregion


        #region AddRemoteParty(...)

        public async Task<Boolean> AddRemoteParty(RemoteParty_Id                                             Id,
                                                  IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                  AccessToken                                                AccessToken,

                                                  AccessToken                                                RemoteAccessToken,
                                                  URL                                                        RemoteVersionsURL,
                                                  IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                  Version_Id?                                                SelectedVersionId            = null,

                                                  DateTime?                                                  LocalAccessNotBefore         = null,
                                                  DateTime?                                                  LocalAccessNotAfter          = null,

                                                  Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                  Boolean?                                                   AllowDowngrades              = false,
                                                  AccessStatus                                               AccessStatus                 = AccessStatus.      ALLOWED,
                                                  RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                                  PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                  DateTime?                                                  RemoteAccessNotBefore        = null,
                                                  DateTime?                                                  RemoteAccessNotAfter         = null,

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

                                                  EventTracking_Id?                                          EventTrackingId              = null,
                                                  User_Id?                                                   CurrentUserId                = null)

        {

            var newRemoteParty = new RemoteParty(
                                     Id,
                                     CredentialsRoles,

                                     AccessToken,

                                     RemoteAccessToken,
                                     RemoteVersionsURL,
                                     RemoteVersionIds,
                                     SelectedVersionId,

                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,

                                     AccessTokenBase64Encoding,
                                     AllowDowngrades,
                                     AccessStatus,
                                     RemoteStatus,
                                     PartyStatus,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,

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
                                     UseHTTPPipelining
                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty)) {

                await LogRemoteParty(addRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemoteParty(...)

        public async Task<Boolean> AddRemoteParty(RemoteParty_Id                                             Id,
                                                  IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                  AccessToken                                                AccessToken,
                                                  DateTime?                                                  LocalAccessNotBefore         = null,
                                                  DateTime?                                                  LocalAccessNotAfter          = null,
                                                  Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                  Boolean?                                                   AllowDowngrades              = false,
                                                  AccessStatus                                               AccessStatus                 = AccessStatus.ALLOWED,

                                                  PartyStatus                                                PartyStatus                  = PartyStatus. ENABLED,

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

                                                  EventTracking_Id?                                          EventTrackingId              = null,
                                                  User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(
                                     Id,
                                     CredentialsRoles,

                                     AccessToken,
                                     AccessTokenBase64Encoding,
                                     AllowDowngrades,
                                     AccessStatus,

                                     PartyStatus,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,

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
                                     UseHTTPPipelining
                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(addRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemoteParty(...)

        public async Task<Boolean> AddRemoteParty(RemoteParty_Id                                             Id,
                                                  IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                  AccessToken                                                RemoteAccessToken,
                                                  URL                                                        RemoteVersionsURL,
                                                  IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                  Version_Id?                                                SelectedVersionId            = null,

                                                  Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                  Boolean?                                                   AllowDowngrades              = null,
                                                  RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                                  PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                  DateTime?                                                  RemoteAccessNotBefore        = null,
                                                  DateTime?                                                  RemoteAccessNotAfter         = null,

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

                                                  EventTracking_Id?                                          EventTrackingId              = null,
                                                  User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(
                                     Id,
                                     CredentialsRoles,

                                     RemoteAccessToken,
                                     RemoteVersionsURL,
                                     RemoteVersionIds,
                                     SelectedVersionId,

                                     AccessTokenBase64Encoding,
                                     AllowDowngrades,
                                     RemoteStatus,
                                     PartyStatus,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,

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
                                     UseHTTPPipelining
                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(addRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemoteParty(...)

        public async Task<Boolean> AddRemoteParty(RemoteParty_Id                                             Id,
                                                  IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                  IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                                                  IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                                                  PartyStatus                                                Status                       = PartyStatus.ENABLED,

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

                                                  DateTime?                                                  LastUpdated                  = null,

                                                  EventTracking_Id?                                          EventTrackingId              = null,
                                                  User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(
                                     Id,
                                     CredentialsRoles,

                                     LocalAccessInfos,
                                     RemoteAccessInfos,

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

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(addRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion


        #region AddRemotePartyIfNotExists(...)

        public async Task<Boolean> AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                                             IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                             AccessToken                                                AccessToken,

                                                             AccessToken                                                RemoteAccessToken,
                                                             URL                                                        RemoteVersionsURL,
                                                             IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                             Version_Id?                                                SelectedVersionId            = null,

                                                             DateTime?                                                  LocalAccessNotBefore         = null,
                                                             DateTime?                                                  LocalAccessNotAfter          = null,

                                                             Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                             Boolean?                                                   AllowDowngrades              = false,
                                                             AccessStatus                                               AccessStatus                 = AccessStatus.      ALLOWED,
                                                             RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                                             PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                             DateTime?                                                  RemoteAccessNotBefore        = null,
                                                             DateTime?                                                  RemoteAccessNotAfter         = null,

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

                                                             EventTracking_Id?                                          EventTrackingId              = null,
                                                             User_Id?                                                   CurrentUserId                = null)

        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 AccessToken,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

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
                                                 UseHTTPPipelining);

            var result = remoteParties.GetOrAdd(newRemoteParty.Id,
                                                value => newRemoteParty);

            if (result == newRemoteParty)
            {

                await LogRemoteParty(addRemotePartyIfNotExists,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemotePartyIfNotExists(...)

        public async Task<Boolean> AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                                             IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                             AccessToken                                                AccessToken,
                                                             DateTime?                                                  LocalAccessNotBefore         = null,
                                                             DateTime?                                                  LocalAccessNotAfter          = null,
                                                             Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                             Boolean?                                                   AllowDowngrades              = false,
                                                             AccessStatus                                               AccessStatus                 = AccessStatus.ALLOWED,

                                                             PartyStatus                                                PartyStatus                  = PartyStatus. ENABLED,

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

                                                             EventTracking_Id?                                          EventTrackingId              = null,
                                                             User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 AccessToken,
                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,

                                                 PartyStatus,
                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

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
                                                 UseHTTPPipelining);

            var result = remoteParties.GetOrAdd(newRemoteParty.Id,
                                                value => newRemoteParty);

            if (result == newRemoteParty)
            {

                await LogRemoteParty(addRemotePartyIfNotExists,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemotePartyIfNotExists(...)

        public async Task<Boolean> AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                                             IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                             AccessToken                                                RemoteAccessToken,
                                                             URL                                                        RemoteVersionsURL,
                                                             IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                             Version_Id?                                                SelectedVersionId            = null,

                                                             Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                             Boolean?                                                   AllowDowngrades              = null,
                                                             RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                                             PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                             DateTime?                                                  RemoteAccessNotBefore        = null,
                                                             DateTime?                                                  RemoteAccessNotAfter         = null,

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

                                                             EventTracking_Id?                                          EventTrackingId              = null,
                                                             User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

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
                                                 UseHTTPPipelining);

            var result = remoteParties.GetOrAdd(newRemoteParty.Id,
                                                value => newRemoteParty);

            if (result == newRemoteParty)
            {

                await LogRemoteParty(addRemotePartyIfNotExists,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemotePartyIfNotExists(...)

        public async Task<Boolean> AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                                             IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                             IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                                                             IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                                                             PartyStatus                                                Status                       = PartyStatus.ENABLED,

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

                                                             DateTime?                                                  LastUpdated                  = null,

                                                             EventTracking_Id?                                          EventTrackingId              = null,
                                                             User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 LocalAccessInfos,
                                                 RemoteAccessInfos,

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

                                                 LastUpdated);

            var result = remoteParties.GetOrAdd(newRemoteParty.Id,
                                                value => newRemoteParty);

            if (result == newRemoteParty)
            {

                await LogRemoteParty(addRemotePartyIfNotExists,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion


        #region AddOrUpdateRemoteParty(...)

        public async Task<Boolean> AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                                          IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                          AccessToken                                                AccessToken,

                                                          AccessToken                                                RemoteAccessToken,
                                                          URL                                                        RemoteVersionsURL,
                                                          IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                          Version_Id?                                                SelectedVersionId            = null,

                                                          DateTime?                                                  LocalAccessNotBefore         = null,
                                                          DateTime?                                                  LocalAccessNotAfter          = null,

                                                          Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                          Boolean?                                                   AllowDowngrades              = false,
                                                          AccessStatus                                               AccessStatus                 = AccessStatus.      ALLOWED,
                                                          RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                                          PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                          DateTime?                                                  RemoteAccessNotBefore        = null,
                                                          DateTime?                                                  RemoteAccessNotAfter         = null,

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

                                                          EventTracking_Id?                                          EventTrackingId              = null,
                                                          User_Id?                                                   CurrentUserId                = null)

        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 AccessToken,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

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
                                                 UseHTTPPipelining);

            var added = false;

            remoteParties.AddOrUpdate(newRemoteParty.Id,

                                      // Add
                                      id => {
                                          added = true;
                                          return newRemoteParty;
                                      },

                                      // Update
                                      (id, oldRemoteParty) => {
                                          return newRemoteParty;
                                      });

            await LogRemoteParty(addOrUpdateRemoteParty,
                                 newRemoteParty.ToJSON(true),
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

            return added;

        }

        #endregion

        #region AddOrUpdateRemoteParty(...)

        public async Task<Boolean> AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                                          IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                          AccessToken                                                AccessToken,
                                                          DateTime?                                                  LocalAccessNotBefore         = null,
                                                          DateTime?                                                  LocalAccessNotAfter          = null,
                                                          Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                          Boolean?                                                   AllowDowngrades              = false,
                                                          AccessStatus                                               AccessStatus                 = AccessStatus.ALLOWED,

                                                          PartyStatus                                                PartyStatus                  = PartyStatus. ENABLED,

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

                                                          EventTracking_Id?                                          EventTrackingId              = null,
                                                          User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 AccessToken,
                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,

                                                 PartyStatus,
                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

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
                                                 UseHTTPPipelining);

            var added = false;

            remoteParties.AddOrUpdate(newRemoteParty.Id,

                                      // Add
                                      id => {
                                          added = true;
                                          return newRemoteParty;
                                      },

                                      // Update
                                      (id, oldRemoteParty) => {
                                          return newRemoteParty;
                                      });

            await LogRemoteParty(addOrUpdateRemoteParty,
                                 newRemoteParty.ToJSON(true),
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

            return added;

        }

        #endregion

        #region AddOrUpdateRemoteParty(...)

        public async Task<Boolean> AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                                          IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                          AccessToken                                                RemoteAccessToken,
                                                          URL                                                        RemoteVersionsURL,
                                                          IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                          Version_Id?                                                SelectedVersionId            = null,

                                                          Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                          Boolean?                                                   AllowDowngrades              = null,
                                                          RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                                          PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                          DateTime?                                                  RemoteAccessNotBefore        = null,
                                                          DateTime?                                                  RemoteAccessNotAfter         = null,

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

                                                          EventTracking_Id?                                          EventTrackingId              = null,
                                                          User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

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
                                                 UseHTTPPipelining);

            var added = false;

            remoteParties.AddOrUpdate(newRemoteParty.Id,

                                      // Add
                                      id => {
                                          added = true;
                                          return newRemoteParty;
                                      },

                                      // Update
                                      (id, oldRemoteParty) => {
                                          return newRemoteParty;
                                      });

            await LogRemoteParty(addOrUpdateRemoteParty,
                                 newRemoteParty.ToJSON(true),
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

            return added;

        }

        #endregion

        #region AddOrUpdateRemoteParty(...)

        public async Task<Boolean> AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                                          IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                          IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                                                          IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                                                          PartyStatus                                                Status                       = PartyStatus.ENABLED,

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

                                                          DateTime?                                                  LastUpdated                  = null,

                                                          EventTracking_Id?                                          EventTrackingId              = null,
                                                          User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 LocalAccessInfos,
                                                 RemoteAccessInfos,

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

                                                 LastUpdated);

            var added = false;

            remoteParties.AddOrUpdate(newRemoteParty.Id,

                                      // Add
                                      id => {
                                          added = true;
                                          return newRemoteParty;
                                      },

                                      // Update
                                      (id, oldRemoteParty) => {
                                          return newRemoteParty;
                                      });

            await LogRemoteParty(addOrUpdateRemoteParty,
                                 newRemoteParty.ToJSON(true),
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

            return added;

        }

        #endregion


        #region UpdateRemoteParty(...)

        public async Task<Boolean> UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,

                                                     AccessToken                                                AccessToken,

                                                     AccessToken                                                RemoteAccessToken,
                                                     URL                                                        RemoteVersionsURL,
                                                     IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                     Version_Id?                                                SelectedVersionId            = null,

                                                     DateTime?                                                  LocalAccessNotBefore         = null,
                                                     DateTime?                                                  LocalAccessNotAfter          = null,

                                                     Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                     Boolean?                                                   AllowDowngrades              = false,
                                                     AccessStatus                                               AccessStatus                 = AccessStatus.      ALLOWED,
                                                     RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                                     PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                     DateTime?                                                  RemoteAccessNotBefore        = null,
                                                     DateTime?                                                  RemoteAccessNotAfter         = null,

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

                                                     EventTracking_Id?                                          EventTrackingId              = null,
                                                     User_Id?                                                   CurrentUserId                = null)

        {

            var newRemoteParty = new RemoteParty(ExistingRemoteParty.Id,
                                                 ExistingRemoteParty.Roles,

                                                 AccessToken,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

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
                                                 UseHTTPPipelining);

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(updateRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region UpdateRemoteParty(...)

        public async Task<Boolean> UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,

                                                     AccessToken                                                AccessToken,
                                                     DateTime?                                                  LocalAccessNotBefore         = null,
                                                     DateTime?                                                  LocalAccessNotAfter          = null,
                                                     Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                     Boolean?                                                   AllowDowngrades              = false,
                                                     AccessStatus                                               AccessStatus                 = AccessStatus.ALLOWED,

                                                     PartyStatus                                                PartyStatus                  = PartyStatus. ENABLED,

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

                                                     EventTracking_Id?                                          EventTrackingId              = null,
                                                     User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(ExistingRemoteParty.Id,
                                                 ExistingRemoteParty.Roles,

                                                 AccessToken,
                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,

                                                 PartyStatus,
                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

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
                                                 UseHTTPPipelining);

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(updateRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region UpdateRemoteParty(...)

        public async Task<Boolean> UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,

                                                     AccessToken                                                RemoteAccessToken,
                                                     URL                                                        RemoteVersionsURL,
                                                     IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                     Version_Id?                                                SelectedVersionId            = null,

                                                     Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                     Boolean?                                                   AllowDowngrades              = null,
                                                     RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                                     PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                     DateTime?                                                  RemoteAccessNotBefore        = null,
                                                     DateTime?                                                  RemoteAccessNotAfter         = null,

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

                                                     EventTracking_Id?                                          EventTrackingId              = null,
                                                     User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(ExistingRemoteParty.Id,
                                                 ExistingRemoteParty.Roles,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

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
                                                 UseHTTPPipelining);

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(updateRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region UpdateRemoteParty(...)

        public async Task<Boolean> UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,

                                                     IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                                                     IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                                                     PartyStatus                                                Status                       = PartyStatus.ENABLED,

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

                                                     DateTime?                                                  LastUpdated                  = null,

                                                     EventTracking_Id?                                          EventTrackingId              = null,
                                                     User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(ExistingRemoteParty.Id,
                                                 ExistingRemoteParty.Roles,

                                                 LocalAccessInfos,
                                                 RemoteAccessInfos,

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

                                                 LastUpdated);

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(updateRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion


        #region ContainsRemoteParty(RemotePartyId)

        /// <summary>
        /// Whether this API contains a remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        public Boolean ContainsRemoteParty(RemoteParty_Id RemotePartyId)

            => remoteParties.ContainsKey(RemotePartyId);

        #endregion

        #region GetRemoteParty     (RemotePartyId)

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

        #region TryGetRemoteParty  (RemotePartyId, out RemoteParty)

        /// <summary>
        /// Try to get the remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        /// <param name="RemoteParty">The remote party.</param>
        public Boolean TryGetRemoteParty(RemoteParty_Id    RemotePartyId,
                                         out RemoteParty?  RemoteParty)

            => remoteParties.TryGetValue(RemotePartyId,
                                         out RemoteParty);

        #endregion

        #region GetRemoteParties   (IncludeFilter = null)

        /// <summary>
        /// Get all remote parties machting the given optional filter.
        /// </summary>
        /// <param name="IncludeFilter">A delegate for filtering remote parties.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(IncludeRemoteParty? IncludeFilter = null)

            => IncludeFilter is null
                   ? remoteParties.Values
                   : remoteParties.Values.
                                   Where(remoteParty => IncludeFilter(remoteParty));

        #endregion

        #region GetRemoteParties   (CountryCode, PartyId)

        /// <summary>
        /// Get all remote parties having the given country code and party identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(CountryCode  CountryCode,
                                                         Party_Id     PartyId)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.Roles.Any(credentialsRole => credentialsRole.CountryCode == CountryCode &&
                                                                                           credentialsRole.PartyId     == PartyId));

        #endregion

        #region GetRemoteParties   (CountryCode, PartyId, Role)

        /// <summary>
        /// Get all remote parties having the given country code, party identification and role.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        /// <param name="Role">A role.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(CountryCode  CountryCode,
                                                         Party_Id     PartyId,
                                                         Roles        Role)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.Roles.Any(credentialsRole => credentialsRole.CountryCode == CountryCode &&
                                                                                           credentialsRole.PartyId     == PartyId &&
                                                                                           credentialsRole.Role        == Role));

        #endregion

        #region GetRemoteParties   (Role)

        /// <summary>
        /// Get all remote parties having the given role.
        /// </summary>
        /// <param name="Role">The role of the remote parties.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(Roles Role)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.Roles.Any(credentialsRole => credentialsRole.Role == Role));

        #endregion

        #region GetRemoteParties   (AccessToken)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken AccessToken)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken == AccessToken));

        #endregion

        #region GetRemoteParties   (AccessToken, AccessStatus)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken   AccessToken,
                                                         AccessStatus  AccessStatus)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken == AccessToken &&
                                                                                                      localAccessInfo.Status      == AccessStatus));

        #endregion

        #region GetRemoteParties   (AccessToken, out RemoteParties)

        public Boolean TryGetRemoteParties(AccessToken                   AccessToken,
                                           out IEnumerable<RemoteParty>  RemoteParties)
        {

            RemoteParties = remoteParties.Values.
                                          Where(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken == AccessToken));

            return RemoteParties.Any();

        }

        #endregion


        #region RemoveRemoteParty(RemoteParty)

        public async Task<Boolean> RemoveRemoteParty(RemoteParty        RemoteParty,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            if (remoteParties.TryRemove(RemoteParty.Id, out var remoteParty))
            {

                await LogRemoteParty(removeRemoteParty,
                                     remoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region RemoveRemoteParty(RemotePartyId)

        public async Task<Boolean> RemoveRemoteParty(RemoteParty_Id     RemotePartyId,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            if (remoteParties.Remove(RemotePartyId, out var remoteParty))
            {

                await LogRemoteParty(removeRemoteParty,
                                     remoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region RemoveRemoteParty(CountryCode, PartyId, Role)

        public async Task<Boolean> RemoveRemoteParty(CountryCode        CountryCode,
                                                     Party_Id           PartyId,
                                                     Roles              Role,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in GetRemoteParties(CountryCode,
                                                         PartyId,
                                                         Role))
            {

                remoteParties.TryRemove(remoteParty.Id, out _);

                await LogRemoteParty(removeRemoteParty,
                                     remoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

            }

            return true;

        }

        #endregion

        #region RemoveRemoteParty(CountryCode, PartyId, Role, AccessToken)

        public async Task<Boolean> RemoveRemoteParty(CountryCode        CountryCode,
                                                     Party_Id           PartyId,
                                                     Roles              Role,
                                                     AccessToken        AccessToken,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in remoteParties.Values.
                                                      Where(remoteParty => remoteParty.Roles.            Any(credentialsRole  => credentialsRole.CountryCode == CountryCode &&
                                                                                                                                 credentialsRole.PartyId     == PartyId &&
                                                                                                                                 credentialsRole.Role        == Role) &&

                                                                           remoteParty.RemoteAccessInfos.Any(remoteAccessInfo => remoteAccessInfo.AccessToken == AccessToken)))
            {

                remoteParties.TryRemove(remoteParty.Id, out _);

                await LogRemoteParty(removeRemoteParty,
                                     remoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

            }

            return true;

        }

        #endregion

        #region RemoveAllRemoteParties()

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

        #region CPOClients

        private readonly ConcurrentDictionary<EMSP_Id, CPOClient> cpoClients = new ();

        /// <summary>
        /// Return an enumeration of all CPO clients.
        /// </summary>
        public IEnumerable<CPOClient> CPOClients
            => cpoClients.Values;


        #region GetCPOClient(CountryCode, PartyId, Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote EMSP.</param>
        /// <param name="PartyId">The party identification of the remote EMSP.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPOClient? GetCPOClient(CountryCode  CountryCode,
                                       Party_Id     PartyId,
                                       I18NString?  Description          = null,
                                       Boolean      AllowCachedClients   = true)
        {

            var emspId         = EMSP_Id.       Parse(CountryCode, PartyId);
            var remotePartyId  = RemoteParty_Id.From (emspId);

            if (AllowCachedClients &&
                cpoClients.TryGetValue(emspId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (remoteParties.TryGetValue(remotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPOClient(
                                    this,
                                    remoteParty,
                                    null,
                                    Description ?? ClientConfigurations.Description?.Invoke(remotePartyId),
                                    null,
                                    ClientConfigurations.DisableLogging?.Invoke(remotePartyId),
                                    ClientConfigurations.LoggingPath?.   Invoke(remotePartyId),
                                    ClientConfigurations.LoggingContext?.Invoke(remotePartyId),
                                    ClientConfigurations.LogfileCreator,
                                    DNSClient
                                );

                cpoClients.TryAdd(emspId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #region GetCPOClient(RemoteParty,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPOClient? GetCPOClient(RemoteParty  RemoteParty,
                                       I18NString?  Description          = null,
                                       Boolean      AllowCachedClients   = true)
        {

            var emspId = EMSP_Id.From(RemoteParty.Id);

            if (AllowCachedClients &&
                cpoClients.TryGetValue(emspId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (RemoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPOClient(
                                    this,
                                    RemoteParty,
                                    null,
                                    Description ?? ClientConfigurations.Description?.Invoke(RemoteParty.Id),
                                    null,
                                    ClientConfigurations.DisableLogging?.Invoke(RemoteParty.Id),
                                    ClientConfigurations.LoggingPath?.   Invoke(RemoteParty.Id),
                                    ClientConfigurations.LoggingContext?.Invoke(RemoteParty.Id),
                                    ClientConfigurations.LogfileCreator,
                                    DNSClient
                                );

                cpoClients.TryAdd(emspId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #region GetCPOClient(RemotePartyId,        Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPOClient? GetCPOClient(RemoteParty_Id  RemotePartyId,
                                       I18NString?     Description          = null,
                                       Boolean         AllowCachedClients   = true)
        {

            var emspId = EMSP_Id.From(RemotePartyId);

            if (AllowCachedClients &&
                cpoClients.TryGetValue(emspId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (remoteParties.TryGetValue(RemotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPOClient(
                                    this,
                                    remoteParty,
                                    null,
                                    Description ?? ClientConfigurations.Description?.Invoke(RemotePartyId),
                                    null,
                                    ClientConfigurations.DisableLogging?.Invoke(RemotePartyId),
                                    ClientConfigurations.LoggingPath?.   Invoke(RemotePartyId),
                                    ClientConfigurations.LoggingContext?.Invoke(RemotePartyId),
                                    ClientConfigurations.LogfileCreator,
                                    DNSClient
                                );

                cpoClients.TryAdd(emspId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #endregion

        #region EMSPClients

        private readonly ConcurrentDictionary<CPO_Id, EMSPClient> emspClients = new ();

        /// <summary>
        /// Return an enumeration of all EMSP clients.
        /// </summary>
        public IEnumerable<EMSPClient> EMSPClients
            => emspClients.Values;


        #region GetEMSPClient(CountryCode, PartyId, Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote EMSP.</param>
        /// <param name="PartyId">The party identification of the remote EMSP.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached EMSP clients.</param>
        public EMSPClient? GetEMSPClient(CountryCode  CountryCode,
                                         Party_Id     PartyId,
                                         I18NString?  Description          = null,
                                         Boolean      AllowCachedClients   = true)
        {

            var cpoId          = CPO_Id.        Parse(CountryCode, PartyId);
            var remotePartyId  = RemoteParty_Id.From (cpoId);

            if (AllowCachedClients &&
                emspClients.TryGetValue(cpoId, out var cachedEMSPClient))
            {
                return cachedEMSPClient;
            }

            if (remoteParties.TryGetValue(remotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var emspClient = new EMSPClient(
                                     this,
                                     remoteParty,
                                     null,
                                     Description ?? ClientConfigurations.Description?.Invoke(remotePartyId),
                                     null,
                                     ClientConfigurations.DisableLogging?.Invoke(remotePartyId),
                                     ClientConfigurations.LoggingPath?.   Invoke(remotePartyId),
                                     ClientConfigurations.LoggingContext?.Invoke(remotePartyId),
                                     ClientConfigurations.LogfileCreator,
                                     DNSClient
                                 );

                emspClients.TryAdd(cpoId, emspClient);

                return emspClient;

            }

            return null;

        }

        #endregion

        #region GetEMSPClient(RemoteParty,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached EMSP clients.</param>
        public EMSPClient? GetEMSPClient(RemoteParty  RemoteParty,
                                         I18NString?  Description          = null,
                                         Boolean      AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemoteParty.Id);

            if (AllowCachedClients &&
                emspClients.TryGetValue(cpoId, out var cachedEMSPClient))
            {
                return cachedEMSPClient;
            }

            if (RemoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var emspClient = new EMSPClient(
                                     this,
                                     RemoteParty,
                                     null,
                                     Description ?? ClientConfigurations.Description?.Invoke(RemoteParty.Id),
                                     null,
                                     ClientConfigurations.DisableLogging?.Invoke(RemoteParty.Id),
                                     ClientConfigurations.LoggingPath?.   Invoke(RemoteParty.Id),
                                     ClientConfigurations.LoggingContext?.Invoke(RemoteParty.Id),
                                     ClientConfigurations.LogfileCreator,
                                     DNSClient
                                 );

                emspClients.TryAdd(cpoId, emspClient);

                return emspClient;

            }

            return null;

        }

        #endregion

        #region GetEMSPClient(RemotePartyId,        Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached EMSP clients.</param>
        public EMSPClient? GetEMSPClient(RemoteParty_Id  RemotePartyId,
                                         I18NString?     Description          = null,
                                         Boolean         AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemotePartyId);

            if (AllowCachedClients &&
                emspClients.TryGetValue(cpoId, out var cachedEMSPClient))
            {
                return cachedEMSPClient;
            }

            if (remoteParties.TryGetValue(RemotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var emspClient = new EMSPClient(
                                     this,
                                     remoteParty,
                                     null,
                                     Description ?? ClientConfigurations.Description?.Invoke(RemotePartyId),
                                     null,
                                     ClientConfigurations.DisableLogging?.Invoke(RemotePartyId),
                                     ClientConfigurations.LoggingPath?.   Invoke(RemotePartyId),
                                     ClientConfigurations.LoggingContext?.Invoke(RemotePartyId),
                                     ClientConfigurations.LogfileCreator,
                                     DNSClient
                                 );

                emspClients.TryAdd(cpoId, emspClient);

                return emspClient;

            }

            return null;

        }

        #endregion

        #endregion


        //ToDo: Add last modified timestamp to locations!
        //ToDo: Refactor async!
        //ToDo: Refactor result data structures!

        #region Locations

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id , Location>>> locations;


        public delegate Task OnLocationAddedDelegate(Location Location);

        public event OnLocationAddedDelegate? OnLocationAdded;


        public delegate Task OnLocationChangedDelegate(Location Location);

        public event OnLocationChangedDelegate? OnLocationChanged;


        #region AddLocation           (Location, SkipNotifications = false)

        public Location AddLocation(Location           Location,
                                    Boolean            SkipNotifications   = false,
                                    EventTracking_Id?  EventTrackingId     = null,
                                    User_Id?           CurrentUserId       = null)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (locations)
            {

                if (!this.locations.TryGetValue(Location.CountryCode, out var parties))
                {
                    parties = [];
                    this.locations.Add(Location.CountryCode, parties);
                }

                if (!parties.TryGetValue(Location.PartyId, out var locations))
                {
                    locations = [];
                    parties.Add(Location.PartyId, locations);
                }

                if (!locations.ContainsKey(Location.Id))
                {

                    locations.Add(Location.Id, Location);
                    Location.CommonAPI = this;

                    if (!SkipNotifications)
                    {

                        var OnLocationAddedLocal = OnLocationAdded;
                        if (OnLocationAddedLocal is not null)
                        {
                            try
                            {
                                OnLocationAddedLocal(Location).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnLocationAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return Location;

                }

                return null;
                throw new ArgumentException("The given location already exists!");

            }

        }

        #endregion

        #region AddLocationIfNotExists(Location, SkipNotifications = false)

        public Location AddLocationIfNotExists(Location           Location,
                                               Boolean            SkipNotifications   = false,
                                               EventTracking_Id?  EventTrackingId     = null,
                                               User_Id?           CurrentUserId       = null)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (locations)
            {

                if (!this.locations.TryGetValue(Location.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                    this.locations.Add(Location.CountryCode, parties);
                }

                if (!parties.TryGetValue(Location.PartyId, out var locations))
                {
                    locations = new Dictionary<Location_Id, Location>();
                    parties.Add(Location.PartyId, locations);
                }

                if (!locations.ContainsKey(Location.Id))
                {

                    locations.Add(Location.Id, Location);
                    Location.CommonAPI = this;

                    if (!SkipNotifications)
                    {

                        var OnLocationAddedLocal = OnLocationAdded;
                        if (OnLocationAddedLocal is not null)
                        {
                            try
                            {
                                OnLocationAddedLocal(Location).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocationIfNotExists), " ", nameof(OnLocationAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                }

                return Location;

            }

        }

        #endregion

        #region AddOrUpdateLocation   (newOrUpdatedLocation, AllowDowngrades = false)

        private AddOrUpdateResult<Location> __addOrUpdateLocation(Location           Location,
                                                                  Boolean?           AllowDowngrades   = false,
                                                                  EventTracking_Id?  EventTrackingId   = null,
                                                                  User_Id?           CurrentUserId     = null)
        {

            if (!this.locations.TryGetValue(Location.CountryCode, out var parties))
            {
                parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                this.locations.Add(Location.CountryCode, parties);
            }

            if (!parties.TryGetValue(Location.PartyId, out var locations))
            {
                locations = new Dictionary<Location_Id, Location>();
                parties.Add(Location.PartyId, locations);
            }

            if (locations.TryGetValue(Location.Id, out var existingLocation))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Location.LastUpdated <= existingLocation.LastUpdated)
                {
                    return AddOrUpdateResult<Location>.Failed     (Location,
                                                                   "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!");
                }

                if (Location.LastUpdated.ToIso8601() == existingLocation.LastUpdated.ToIso8601())
                    return AddOrUpdateResult<Location>.NoOperation(Location,
                                                                   "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!");

                locations[Location.Id] = Location;
                Location.CommonAPI = this;

                var OnLocationChangedLocal = OnLocationChanged;
                if (OnLocationChangedLocal is not null)
                {
                    try
                    {
                        OnLocationChangedLocal(Location).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnLocationChanged), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

                var OnEVSEChangedLocal = OnEVSEChanged;
                if (OnEVSEChangedLocal is not null)
                {
                    try
                    {
                        foreach (var evse in Location.EVSEs)
                            OnEVSEChangedLocal(evse).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

                return AddOrUpdateResult<Location>.Success(Location,
                                                           WasCreated: false);

            }

            locations.Add(Location.Id, Location);
            Location.CommonAPI = this;

            var OnLocationAddedLocal = OnLocationAdded;
            if (OnLocationAddedLocal is not null)
            {
                try
                {
                    OnLocationAddedLocal(Location).Wait();
                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnLocationAdded), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            return AddOrUpdateResult<Location>.Success(Location,
                                                       WasCreated: true);

        }

        public async Task<AddOrUpdateResult<Location>> AddOrUpdateLocation(Location  newOrUpdatedLocation,
                                                                           Boolean?  AllowDowngrades = false)
        {

            if (newOrUpdatedLocation is null)
                return AddOrUpdateResult<Location>.Failed(newOrUpdatedLocation,
                                                          "The given location must not be null!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (locations)
            {
                return __addOrUpdateLocation(newOrUpdatedLocation,
                                             AllowDowngrades);
            }

        }

        #endregion

        #region UpdateLocation        (Location)

        public Location? UpdateLocation(Location           Location,
                                        EventTracking_Id?  EventTrackingId   = null,
                                        User_Id?           CurrentUserId     = null)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (locations)
            {

                if (locations.TryGetValue(Location.CountryCode, out var parties)   &&
                    parties.  TryGetValue(Location.PartyId,     out var _locations) &&
                    _locations.ContainsKey(Location.Id))
                {

                    _locations[Location.Id] = Location;
                    Location.CommonAPI = this;

                    var OnEVSEChangedLocal = OnEVSEChanged;
                    if (OnEVSEChangedLocal is not null)
                    {
                        try
                        {
                            foreach (var evse in Location.EVSEs)
                                OnEVSEChangedLocal(evse).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    return Location;

                }

                return null;

            }

        }

        #endregion


        #region TryPatchLocation      (Location, LocationPatch, AllowDowngrades = false)

        public async Task<PatchResult<Location>> TryPatchLocation(Location           Location,
                                                                  JObject            LocationPatch,
                                                                  Boolean?           AllowDowngrades   = false,
                                                                  EventTracking_Id?  EventTrackingId   = null,
                                                                  User_Id?           CurrentUserId     = null)
        {

            if (Location is null)
                return PatchResult<Location>.Failed(Location,
                                                    "The given location must not be null!");

            if (LocationPatch is null || !LocationPatch.HasValues)
                return PatchResult<Location>.Failed(Location,
                                                    "The given location patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (locations)
            {

                if (locations. TryGetValue(Location.CountryCode, out var parties)   &&
                    parties.   TryGetValue(Location.PartyId,     out var _locations) &&
                    _locations.TryGetValue(Location.Id,          out var existingLocation))
                {

                    var patchResult = existingLocation.TryPatch(LocationPatch,
                                                                AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                    {

                        _locations[Location.Id] = patchResult.PatchedData;

                        var OnLocationChangedLocal = OnLocationChanged;
                        if (OnLocationChangedLocal is not null)
                        {
                            try
                            {
                                OnLocationChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchLocation), " ", nameof(OnLocationChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                        //ToDo: MayBe nothing changed here... perhaps test for changes before sending events!
                        var OnEVSEChangedLocal = OnEVSEChanged;
                        if (OnEVSEChangedLocal is not null)
                        {
                            try
                            {
                                foreach (var evse in patchResult.PatchedData.EVSEs)
                                    OnEVSEChangedLocal(evse).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchLocation), " ", nameof(OnEVSEChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return patchResult;

                }

                else
                    return PatchResult<Location>.Failed(Location,
                                                        "The given location does not exist!");

            }

        }

        #endregion



        public delegate Task OnEVSEAddedDelegate(EVSE EVSE);

        public event OnEVSEAddedDelegate? OnEVSEAdded;


        public delegate Task OnEVSEChangedDelegate(EVSE EVSE);

        public event OnEVSEChangedDelegate? OnEVSEChanged;

        public delegate Task OnEVSEStatusChangedDelegate(DateTime Timestamp, EVSE EVSE, StatusType OldEVSEStatus, StatusType NewEVSEStatus);

        public event OnEVSEStatusChangedDelegate? OnEVSEStatusChanged;


        public delegate Task OnEVSERemovedDelegate(EVSE EVSE);

        public event OnEVSERemovedDelegate? OnEVSERemoved;


        #region AddOrUpdateEVSE       (Location, newOrUpdatedEVSE, AllowDowngrades = false)

        private AddOrUpdateResult<EVSE> __addOrUpdateEVSE(Location           Location,
                                                          EVSE               newOrUpdatedEVSE,
                                                          Boolean?           AllowDowngrades   = false,
                                                          EventTracking_Id?  EventTrackingId   = null,
                                                          User_Id?           CurrentUserId     = null)
        {

            Location.TryGetEVSE(newOrUpdatedEVSE.UId, out var existingEVSE);

            if (existingEVSE is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    newOrUpdatedEVSE.LastUpdated < existingEVSE.LastUpdated)
                {
                    return AddOrUpdateResult<EVSE>.Failed     (newOrUpdatedEVSE,
                                                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!");
                }

                if (newOrUpdatedEVSE.LastUpdated.ToIso8601() == existingEVSE.LastUpdated.ToIso8601())
                    return AddOrUpdateResult<EVSE>.NoOperation(newOrUpdatedEVSE,
                                                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!");

            }


            Location.SetEVSE(newOrUpdatedEVSE);

            // Update location timestamp!
            var builder = Location.ToBuilder();
            builder.LastUpdated = newOrUpdatedEVSE.LastUpdated;
            __addOrUpdateLocation(builder, (AllowDowngrades ?? this.AllowDowngrades) == false);

            var OnLocationChangedLocal = OnLocationChanged;
            if (OnLocationChangedLocal is not null)
            {
                try
                {
                    OnLocationChangedLocal(newOrUpdatedEVSE.ParentLocation).Wait();
                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnLocationChanged), ": ",
                                Environment.NewLine, e.Message,
                                e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                }
            }


            if (existingEVSE is not null)
            {

                if (existingEVSE.Status != StatusType.REMOVED)
                {

                    var OnEVSEChangedLocal = OnEVSEChanged;
                    if (OnEVSEChangedLocal is not null)
                    {
                        try
                        {
                            OnEVSEChangedLocal(newOrUpdatedEVSE).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                        }
                    }


                    if (existingEVSE.Status != newOrUpdatedEVSE.Status)
                    {
                        var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                        if (OnEVSEStatusChangedLocal is not null)
                        {
                            try
                            {

                                OnEVSEStatusChangedLocal(Timestamp.Now,
                                                         newOrUpdatedEVSE,
                                                         existingEVSE.Status,
                                                         newOrUpdatedEVSE.Status).Wait();

                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEStatusChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            e.StackTrace is not null
                                                ? Environment.NewLine + e.StackTrace
                                                : String.Empty);
                            }
                        }
                    }

                }
                else
                {

                    if (!KeepRemovedEVSEs(newOrUpdatedEVSE))
                        Location.RemoveEVSE(newOrUpdatedEVSE);

                    var OnEVSERemovedLocal = OnEVSERemoved;
                    if (OnEVSERemovedLocal is not null)
                    {
                        try
                        {
                            OnEVSERemovedLocal(newOrUpdatedEVSE).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSERemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                        }
                    }

                }
            }
            else
            {
                var OnEVSEAddedLocal = OnEVSEAdded;
                if (OnEVSEAddedLocal is not null)
                {
                    try
                    {
                        OnEVSEAddedLocal(newOrUpdatedEVSE).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEAdded), ": ",
                                    Environment.NewLine, e.Message,
                                    e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                    }
                }
            }

            return AddOrUpdateResult<EVSE>.Success(newOrUpdatedEVSE,
                                                   WasCreated: existingEVSE is null);

        }

        public async Task<AddOrUpdateResult<EVSE>> AddOrUpdateEVSE(Location  Location,
                                                                   EVSE      newOrUpdatedEVSE,
                                                                   Boolean?  AllowDowngrades = false)
        {

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (locations)
            {
                return __addOrUpdateEVSE(Location,
                                         newOrUpdatedEVSE,
                                         (AllowDowngrades ?? this.AllowDowngrades) == false);
            }

        }

        #endregion

        #region TryPatchEVSE          (Location, EVSE, EVSEPatch,  AllowDowngrades = false)

        public async Task<PatchResult<EVSE>> TryPatchEVSE(Location           Location,
                                                          EVSE               EVSE,
                                                          JObject            EVSEPatch,
                                                          Boolean?           AllowDowngrades   = false,
                                                          EventTracking_Id?  EventTrackingId   = null,
                                                          User_Id?           CurrentUserId     = null)
        {

            if (!EVSEPatch.HasValues)
                return PatchResult<EVSE>.Failed(EVSE,
                                                "The given EVSE patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (locations)
            {

                var patchResult        = EVSE.TryPatch(EVSEPatch,
                                                       AllowDowngrades ?? this.AllowDowngrades ?? false);

                var justAStatusChange  = EVSEPatch.Children().Count() == 2 && EVSEPatch.ContainsKey("status") && EVSEPatch.ContainsKey("last_updated");

                if (patchResult.IsSuccess)
                {

                    if (patchResult.PatchedData.Status != StatusType.REMOVED || KeepRemovedEVSEs(EVSE))
                        Location.SetEVSE   (patchResult.PatchedData);
                    else
                        Location.RemoveEVSE(patchResult.PatchedData);

                    // Update location timestamp!
                    var builder = Location.ToBuilder();
                    builder.LastUpdated = patchResult.PatchedData.LastUpdated;
                    __addOrUpdateLocation(builder, (AllowDowngrades ?? this.AllowDowngrades) == false);


                    if (EVSE.Status != StatusType.REMOVED)
                    {

                        if (justAStatusChange)
                        {

                            DebugX.Log("EVSE status change: " + EVSE.EVSEId + " => " + patchResult.PatchedData.Status);

                            var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                            if (OnEVSEStatusChangedLocal is not null)
                            {
                                try
                                {

                                    OnEVSEStatusChangedLocal(patchResult.PatchedData.LastUpdated,
                                                             EVSE,
                                                             EVSE.Status,
                                                             patchResult.PatchedData.Status).Wait();

                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchEVSE), " ", nameof(OnEVSEStatusChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                e.StackTrace is not null
                                                    ? Environment.NewLine + e.StackTrace
                                                    : String.Empty);
                                }
                            }

                        }
                        else
                        {

                            var OnEVSEChangedLocal = OnEVSEChanged;
                            if (OnEVSEChangedLocal is not null)
                            {
                                try
                                {
                                    OnEVSEChangedLocal(patchResult.PatchedData).Wait();
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchEVSE), " ", nameof(OnEVSEChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                e.StackTrace is not null
                                                    ? Environment.NewLine + e.StackTrace
                                                    : String.Empty);
                                }
                            }

                        }

                    }
                    else
                    {

                        var OnEVSERemovedLocal = OnEVSERemoved;
                        if (OnEVSERemovedLocal is not null)
                        {
                            try
                            {
                                OnEVSERemovedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchEVSE), " ", nameof(OnEVSERemoved), ": ",
                                            Environment.NewLine, e.Message,
                                            e.StackTrace is not null
                                                ? Environment.NewLine + e.StackTrace
                                                : String.Empty);
                            }
                        }

                    }

                }

                return patchResult;

            }

        }

        #endregion


        #region AddOrUpdateConnector  (Location, EVSE, newOrUpdatedConnector,     AllowDowngrades = false)

        public async Task<AddOrUpdateResult<Connector>> AddOrUpdateConnector(Location           Location,
                                                                             EVSE               EVSE,
                                                                             Connector          newOrUpdatedConnector,
                                                                             Boolean?           AllowDowngrades   = false,
                                                                             EventTracking_Id?  EventTrackingId   = null,
                                                                             User_Id?           CurrentUserId     = null)
        {

            if (Location is null)
                return AddOrUpdateResult<Connector>.Failed(newOrUpdatedConnector,
                                                           "The given location must not be null!");

            if (EVSE     is null)
                return AddOrUpdateResult<Connector>.Failed(newOrUpdatedConnector,
                                                           "The given EVSE must not be null!");

            if (newOrUpdatedConnector is null)
                return AddOrUpdateResult<Connector>.Failed(newOrUpdatedConnector,
                                                           "The given connector must not be null!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (locations)
            {

                var ConnectorExistedBefore = EVSE.TryGetConnector(newOrUpdatedConnector.Id, out var existingConnector);

                if (existingConnector is not null)
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        newOrUpdatedConnector.LastUpdated < existingConnector.LastUpdated)
                    {
                        return AddOrUpdateResult<Connector>.Failed     (newOrUpdatedConnector,
                                                                        "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");
                    }

                    if (newOrUpdatedConnector.LastUpdated.ToIso8601() == existingConnector.LastUpdated.ToIso8601())
                        return AddOrUpdateResult<Connector>.NoOperation(newOrUpdatedConnector,
                                                                        "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");

                }

                EVSE.UpdateConnector(newOrUpdatedConnector);

                // Update EVSE/location timestamps!
                var evseBuilder     = EVSE.ToBuilder();
                evseBuilder.LastUpdated = newOrUpdatedConnector.LastUpdated;
                __addOrUpdateEVSE    (Location, evseBuilder,     (AllowDowngrades ?? this.AllowDowngrades) == false);


                var OnLocationChangedLocal = OnLocationChanged;
                if (OnLocationChangedLocal is not null)
                {
                    try
                    {
                        OnLocationChangedLocal(newOrUpdatedConnector.ParentEVSE.ParentLocation).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateConnector), " ", nameof(OnLocationChanged), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }


                return AddOrUpdateResult<Connector>.Success(newOrUpdatedConnector,
                                                            WasCreated: !ConnectorExistedBefore);

            }

        }

        #endregion

        #region TryPatchConnector     (Location, EVSE, Connector, ConnectorPatch, AllowDowngrades = false)

        public async Task<PatchResult<Connector>> TryPatchConnector(Location           Location,
                                                                    EVSE               EVSE,
                                                                    Connector          Connector,
                                                                    JObject            ConnectorPatch,
                                                                    Boolean?           AllowDowngrades   = false,
                                                                    EventTracking_Id?  EventTrackingId   = null,
                                                                    User_Id?           CurrentUserId     = null)
        {

            if (Location is null)
                return PatchResult<Connector>.Failed(Connector,
                                                     "The given location must not be null!");

            if (EVSE is null)
                return PatchResult<Connector>.Failed(Connector,
                                                     "The given EVSE must not be null!");

            if (Connector is null)
                return PatchResult<Connector>.Failed(Connector,
                                                     "The given connector must not be null!");

            if (ConnectorPatch is null || !ConnectorPatch.HasValues)
                return PatchResult<Connector>.Failed(Connector,
                                                     "The given connector patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (locations)
            {

                var patchResult = Connector.TryPatch(ConnectorPatch,
                                                     AllowDowngrades ?? this.AllowDowngrades ?? false);

                if (patchResult.IsSuccess)
                {

                    EVSE.UpdateConnector(patchResult.PatchedData);

                    // Update EVSE/location timestamps!
                    var evseBuilder     = EVSE.ToBuilder();
                    evseBuilder.LastUpdated = patchResult.PatchedData.LastUpdated;
                    __addOrUpdateEVSE    (Location, evseBuilder,     (AllowDowngrades ?? this.AllowDowngrades) == false);

                }

                return patchResult;

            }

        }

        #endregion


        #region LocationExists(CountryCode, PartyId, LocationId)

        public Boolean LocationExists(CountryCode   CountryCode,
                                      Party_Id      PartyId,
                                      Location_Id   LocationId)
        {

            lock (locations)
            {

                if (locations.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var locations))
                    {
                        return locations.ContainsKey(LocationId);
                    }
                }

                return false;

            }

        }

        #endregion

        #region TryGetLocation(CountryCode, PartyId, LocationId, out Location)

        public Boolean TryGetLocation(CountryCode    CountryCode,
                                      Party_Id       PartyId,
                                      Location_Id    LocationId,
                                      out Location?  Location)
        {

            lock (locations)
            {

                if (locations.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var locations))
                    {
                        if (locations.TryGetValue(LocationId, out Location))
                            return true;
                    }
                }

                Location = null;
                return false;

            }

        }

        #endregion

        #region GetLocations  (IncludeLocation)

        public IEnumerable<Location> GetLocations(Func<Location, Boolean> IncludeLocation)
        {

            lock (locations)
            {

                var allLocations = new List<Location>();

                foreach (var party in locations.Values)
                {
                    foreach (var partyLocations in party.Values)
                    {
                        foreach (var location in partyLocations.Values)
                        {
                            if (location is not null &&
                                IncludeLocation(location))
                            {
                                allLocations.Add(location);
                            }
                        }
                    }
                }

                return allLocations;

            }

        }

        #endregion

        #region GetLocations  (CountryCode = null, PartyId = null)

        public IEnumerable<Location> GetLocations(CountryCode? CountryCode   = null,
                                                  Party_Id?    PartyId       = null)
        {

            lock (locations)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (locations.TryGetValue(CountryCode.Value, out var parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out var locations))
                        {
                            return locations.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allLocations = new List<Location>();

                    foreach (var party in locations.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out var locations))
                        {
                            allLocations.AddRange(locations.Values);
                        }
                    }

                    return allLocations;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (locations.TryGetValue(CountryCode.Value, out var parties))
                    {

                        var allLocations = new List<Location>();

                        foreach (var locations in parties.Values)
                        {
                            allLocations.AddRange(locations.Values);
                        }

                        return allLocations;

                    }
                }

                else
                {

                    var allLocations = new List<Location>();

                    foreach (var party in locations.Values)
                    {
                        foreach (var locations in party.Values)
                        {
                            allLocations.AddRange(locations.Values);
                        }
                    }

                    return allLocations;

                }

                return Array.Empty<Location>();

            }

        }

        #endregion


        #region RemoveLocation    (Location)

        public Location RemoveLocation(Location           Location,
                                       EventTracking_Id?  EventTrackingId   = null,
                                       User_Id?           CurrentUserId     = null)
        {

            lock (locations)
            {

                if (locations.TryGetValue(Location.CountryCode, out var parties))
                {

                    if (parties.TryGetValue(Location.PartyId, out var locations))
                    {

                        if (locations.ContainsKey(Location.Id))
                        {
                            locations.Remove(Location.Id);
                        }

                        if (!locations.Any())
                            parties.Remove(Location.PartyId);

                    }

                    if (!parties.Any())
                        this.locations.Remove(Location.CountryCode);

                }

                return Location;

            }

        }

        #endregion

        #region RemoveAllLocations()

        /// <summary>
        /// Remove all locations.
        /// </summary>
        public void RemoveAllLocations(EventTracking_Id?  EventTrackingId   = null,
                                       User_Id?           CurrentUserId     = null)
        {

            lock (locations)
            {
                locations.Clear();
            }

        }

        #endregion

        #region RemoveAllLocations(CountryCode, PartyId)

        /// <summary>
        /// Remove all locations owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllLocations(CountryCode        CountryCode,
                                       Party_Id           PartyId,
                                       EventTracking_Id?  EventTrackingId   = null,
                                       User_Id?           CurrentUserId     = null)
        {

            lock (locations)
            {

                if (locations.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var locations))
                    {
                        locations.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion

        #region Tariffs

        #region Data

        private readonly ConcurrentDictionary<CountryCode, ConcurrentDictionary<Party_Id, TimeRangeDictionary<Tariff_Id , Tariff>>> tariffs = new();


        public delegate Task OnTariffAddedDelegate(Tariff Tariff);

        public event OnTariffAddedDelegate? OnTariffAdded;


        public delegate Task OnTariffChangedDelegate(Tariff Tariff);

        public event OnTariffChangedDelegate? OnTariffChanged;

        #endregion


        public GetTariffIds2_Delegate?        GetTariffIdsDelegate       { get; set; }


        #region AddTariff            (Tariff,                                       SkipNotifications = false, ...)

        public async Task<AddResult<Tariff>> AddTariff(Tariff             Tariff,
                                                       Boolean            SkipNotifications   = false,
                                                       EventTracking_Id?  EventTrackingId     = null,
                                                       User_Id?           CurrentUserId       = null)
        {

            if (!this.tariffs.TryGetValue(Tariff.CountryCode, out var parties))
            {
                parties = new ConcurrentDictionary<Party_Id, TimeRangeDictionary<Tariff_Id, Tariff>>();
                this.tariffs.TryAdd(Tariff.CountryCode, parties);
            }

            if (!parties.TryGetValue(Tariff.PartyId, out var tariffs))
            {
                tariffs = new TimeRangeDictionary<Tariff_Id, Tariff>();
                parties.TryAdd(Tariff.PartyId, tariffs);
            }

            if (tariffs.TryAdd(Tariff.Id, Tariff))
            {

                Tariff.CommonAPI = this;

                await LogAsset(addTariff,
                               Tariff.ToJSON(true,
                                             true,
                                             CustomTariffSerializer,
                                             CustomDisplayTextSerializer,
                                             CustomPriceSerializer,
                                             CustomTariffElementSerializer,
                                             CustomPriceComponentSerializer,
                                             CustomTariffRestrictionsSerializer,
                                             CustomEnergyMixSerializer,
                                             CustomEnergySourceSerializer,
                                             CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTariffAddedLocal = OnTariffAdded;
                    if (OnTariffAddedLocal is not null)
                    {
                        try
                        {
                            OnTariffAddedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddTariff), " ", nameof(OnTariffAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Tariff>.Success(Tariff);

            }

            return AddResult<Tariff>.Failed(Tariff,
                                            "TryAdd(Tariff.Id, Tariff) failed!");

        }

        #endregion

        #region AddTariffIfNotExists (Tariff,                                       SkipNotifications = false, ...)

        public async Task<AddResult<Tariff>> AddTariffIfNotExists(Tariff             Tariff,
                                                                  Boolean            SkipNotifications   = false,
                                                                  EventTracking_Id?  EventTrackingId     = null,
                                                                  User_Id?           CurrentUserId       = null)
        {

            if (!this.tariffs.TryGetValue(Tariff.CountryCode, out var parties))
            {
                parties = new ConcurrentDictionary<Party_Id, TimeRangeDictionary<Tariff_Id, Tariff>>();
                this.tariffs.TryAdd(Tariff.CountryCode, parties);
            }

            if (!parties.TryGetValue(Tariff.PartyId, out var tariffs))
            {
                tariffs = new TimeRangeDictionary<Tariff_Id, Tariff>();
                parties.TryAdd(Tariff.PartyId, tariffs);
            }

            if (tariffs.TryAdd(Tariff.Id, Tariff))
            {

                Tariff.CommonAPI = this;

                await LogAsset(addTariffIfNotExists,
                               Tariff.ToJSON(true,
                                             true,
                                             CustomTariffSerializer,
                                             CustomDisplayTextSerializer,
                                             CustomPriceSerializer,
                                             CustomTariffElementSerializer,
                                             CustomPriceComponentSerializer,
                                             CustomTariffRestrictionsSerializer,
                                             CustomEnergyMixSerializer,
                                             CustomEnergySourceSerializer,
                                             CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTariffAddedLocal = OnTariffAdded;
                    if (OnTariffAddedLocal is not null)
                    {
                        try
                        {
                            OnTariffAddedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddTariffIfNotExists), " ", nameof(OnTariffAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Tariff>.Success(Tariff);

            }

            return AddResult<Tariff>.NoOperation(Tariff);

        }

        #endregion

        #region AddOrUpdateTariff    (Tariff,              AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<AddOrUpdateResult<Tariff>> AddOrUpdateTariff(Tariff             Tariff,
                                                                       Boolean?           AllowDowngrades     = false,
                                                                       Boolean            SkipNotifications   = false,
                                                                       EventTracking_Id?  EventTrackingId     = null,
                                                                       User_Id?           CurrentUserId       = null)
        {

            if (!this.tariffs.TryGetValue(Tariff.CountryCode, out var parties))
            {
                parties = new ConcurrentDictionary<Party_Id, TimeRangeDictionary<Tariff_Id, Tariff>>();
                this.tariffs.TryAdd(Tariff.CountryCode, parties);
            }

            if (!parties.TryGetValue(Tariff.PartyId, out var tariffs))
            {
                tariffs = new TimeRangeDictionary<Tariff_Id, Tariff>();
                parties.TryAdd(Tariff.PartyId, tariffs);
            }

            #region Update an existing tariff

            if (tariffs.TryGetValue(Tariff.Id,
                                    out var existingTariff,
                                    Tariff.NotBefore ?? DateTime.MinValue))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Tariff.LastUpdated <= existingTariff.LastUpdated)
                {
                    return AddOrUpdateResult<Tariff>.Failed(Tariff,
                                                            "The 'lastUpdated' timestamp of the new charging tariff must be newer then the timestamp of the existing tariff!");
                }

                tariffs.AddOrUpdate(Tariff.Id, Tariff);
                Tariff.CommonAPI = this;

                await LogAsset(addOrUpdateTariff,
                               Tariff.ToJSON(true,
                                             true,
                                             CustomTariffSerializer,
                                             CustomDisplayTextSerializer,
                                             CustomPriceSerializer,
                                             CustomTariffElementSerializer,
                                             CustomPriceComponentSerializer,
                                             CustomTariffRestrictionsSerializer,
                                             CustomEnergyMixSerializer,
                                             CustomEnergySourceSerializer,
                                             CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTariffChangedLocal = OnTariffChanged;
                    if (OnTariffChangedLocal is not null)
                    {
                        try
                        {
                            OnTariffChangedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateTariff), " ", nameof(OnTariffChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Tariff>.Success(Tariff,
                                                         WasCreated: false);

            }

            #endregion

            #region Add a new tariff

            if (tariffs.TryAdd(Tariff.Id, Tariff))
            {

                Tariff.CommonAPI = this;

                await LogAsset(addOrUpdateTariff,
                               Tariff.ToJSON(true,
                                             true,
                                             CustomTariffSerializer,
                                             CustomDisplayTextSerializer,
                                             CustomPriceSerializer,
                                             CustomTariffElementSerializer,
                                             CustomPriceComponentSerializer,
                                             CustomTariffRestrictionsSerializer,
                                             CustomEnergyMixSerializer,
                                             CustomEnergySourceSerializer,
                                             CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTariffAddedLocal = OnTariffAdded;
                    if (OnTariffAddedLocal is not null)
                    {
                        try
                        {
                            OnTariffAddedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateTariff), " ", nameof(OnTariffAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Tariff>.Success(Tariff,
                                                         WasCreated: true);

            }

            return AddOrUpdateResult<Tariff>.Failed(Tariff,
                                                    "AddOrUpdateTariff(Tariff.Id, Tariff) failed!");

            #endregion

        }

        #endregion

        #region UpdateTariff         (Tariff,              AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<UpdateResult<Tariff>> UpdateTariff(Tariff             Tariff,
                                                             Boolean?           AllowDowngrades     = false,
                                                             Boolean            SkipNotifications   = false,
                                                             EventTracking_Id?  EventTrackingId     = null,
                                                             User_Id?           CurrentUserId       = null)
        {

            if (!this.tariffs.TryGetValue(Tariff.CountryCode, out var parties))
            {
                parties = new ConcurrentDictionary<Party_Id, TimeRangeDictionary<Tariff_Id, Tariff>>();
                this.tariffs.TryAdd(Tariff.CountryCode, parties);
            }

            if (!parties.TryGetValue(Tariff.PartyId, out var tariffs))
            {
                tariffs = new TimeRangeDictionary<Tariff_Id, Tariff>();
                parties.TryAdd(Tariff.PartyId, tariffs);
            }

            #region Validate AllowDowngrades

            if (tariffs.TryGetValue(Tariff.Id, out var existingTariff, Timestamp.Now))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Tariff.LastUpdated <= existingTariff.LastUpdated)
                {

                    return UpdateResult<Tariff>.Failed(Tariff,
                                                       "The 'lastUpdated' timestamp of the new charging tariff must be newer then the timestamp of the existing tariff!");

                }

            }
            else
                return UpdateResult<Tariff>.Failed(Tariff,
                                                   $"Unknown tariff identification '{Tariff.Id}'!");

            #endregion

            if (tariffs.TryUpdate(Tariff.Id, Tariff, existingTariff))
            {

                Tariff.CommonAPI = this;

                await LogAsset(updateTariff,
                               Tariff.ToJSON(true,
                                             true,
                                             CustomTariffSerializer,
                                             CustomDisplayTextSerializer,
                                             CustomPriceSerializer,
                                             CustomTariffElementSerializer,
                                             CustomPriceComponentSerializer,
                                             CustomTariffRestrictionsSerializer,
                                             CustomEnergyMixSerializer,
                                             CustomEnergySourceSerializer,
                                             CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTariffChangedLocal = OnTariffChanged;
                    if (OnTariffChangedLocal is not null)
                    {
                        try
                        {
                            OnTariffChangedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateTariff), " ", nameof(OnTariffChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return UpdateResult<Tariff>.Success(Tariff);

            }

            return UpdateResult<Tariff>.Failed(Tariff,
                                               "UpdateTariff(Tariff.Id, Tariff, Tariff) failed!");

        }

        #endregion

        #region TryPatchTariff       (Tariff, TariffPatch, AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<PatchResult<Tariff>> TryPatchTariff(Tariff             Tariff,
                                                              JObject            TariffPatch,
                                                              Boolean?           AllowDowngrades     = false,
                                                              Boolean            SkipNotifications   = false,
                                                              EventTracking_Id?  EventTrackingId     = null,
                                                              User_Id?           CurrentUserId       = null)
        {

            if (!TariffPatch.HasValues)
                return PatchResult<Tariff>.Failed(Tariff,
                                                  "The given charging tariff patch must not be null or empty!");

            if (!this.tariffs.TryGetValue(Tariff.CountryCode, out var parties))
            {
                parties = new ConcurrentDictionary<Party_Id, TimeRangeDictionary<Tariff_Id, Tariff>>();
                this.tariffs.TryAdd(Tariff.CountryCode, parties);
            }

            if (!parties.TryGetValue(Tariff.PartyId, out var tariffs))
            {
                tariffs = new TimeRangeDictionary<Tariff_Id, Tariff>();
                parties.TryAdd(Tariff.PartyId, tariffs);
            }


            if (tariffs.TryGetValue(Tariff.Id, out var existingTariff, Timestamp.Now))
            {

                var patchResult = existingTariff.TryPatch(TariffPatch,
                                                          AllowDowngrades ?? this.AllowDowngrades ?? false);

                if (patchResult.IsSuccess &&
                    patchResult.PatchedData is not null)
                {

                    tariffs.TryUpdate(Tariff.Id, Tariff, patchResult.PatchedData);

                    await LogAsset(updateTariff,
                                   Tariff.ToJSON(true,
                                                 true,
                                                 CustomTariffSerializer,
                                                 CustomDisplayTextSerializer,
                                                 CustomPriceSerializer,
                                                 CustomTariffElementSerializer,
                                                 CustomPriceComponentSerializer,
                                                 CustomTariffRestrictionsSerializer,
                                                 CustomEnergyMixSerializer,
                                                 CustomEnergySourceSerializer,
                                                 CustomEnvironmentalImpactSerializer),
                                   EventTrackingId ?? EventTracking_Id.New,
                                   CurrentUserId);

                    if (!SkipNotifications)
                    {

                        var OnTariffChangedLocal = OnTariffChanged;
                        if (OnTariffChangedLocal is not null)
                        {
                            try
                            {
                                OnTariffChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchTariff), " ", nameof(OnTariffChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                }

                return patchResult;

            }

            else
                return PatchResult<Tariff>.Failed(Tariff,
                                                  "The given charging tariff does not exist!");

        }

        #endregion

        #region RemoveTariff         (Tariff,                                       SkipNotifications = false, ...)

        /// <summary>
        /// Remove the given charging tariff.
        /// </summary>
        /// <param name="Tariff">A charging tariff.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveTariff(Tariff             Tariff,
                                                                          Boolean            SkipNotifications   = false,
                                                                          EventTracking_Id?  EventTrackingId     = null,
                                                                          User_Id?           CurrentUserId       = null)
        {

            IEnumerable<Tariff> removedTariffs = [];
            var success = false;

            if (this.tariffs.TryGetValue(Tariff.CountryCode, out var parties))
            {

                if (parties.TryGetValue(Tariff.PartyId, out var tariffs))
                {

                    if (tariffs.TryRemove(Tariff.Id, out removedTariffs))
                    {

                        if (removedTariffs.Any())
                            await LogAsset(removeTariff,
                                           new JArray(
                                               removedTariffs.Select(tariff => tariff.ToJSON(
                                                                                   true,
                                                                                   true,
                                                                                   CustomTariffSerializer,
                                                                                   CustomDisplayTextSerializer,
                                                                                   CustomPriceSerializer,
                                                                                   CustomTariffElementSerializer,
                                                                                   CustomPriceComponentSerializer,
                                                                                   CustomTariffRestrictionsSerializer,
                                                                                   CustomEnergyMixSerializer,
                                                                                   CustomEnergySourceSerializer,
                                                                                   CustomEnvironmentalImpactSerializer
                                                                               )
                                                                    )
                                           ),
                                           EventTrackingId ?? EventTracking_Id.New,
                                           CurrentUserId);

                        success = true;

                    }

                    if (!tariffs.Any())
                        parties.Remove(Tariff.PartyId, out _);

                }

                if (parties.IsEmpty)
                    this.tariffs.Remove(Tariff.CountryCode, out _);

            }

            return success
                       ? RemoveResult<IEnumerable<Tariff>>.Success(removedTariffs)
                       : RemoveResult<IEnumerable<Tariff>>.Failed (null, "RemoveTariff(TariffId, ...) failed!");

        }

        #endregion

        #region RemoveTariff         (TariffId,                                     SkipNotifications = false, ...)

        /// <summary>
        /// Remove the given charging tariff.
        /// </summary>
        /// <param name="TariffId">An unique charging tariff identification.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveTariff(Tariff_Id          TariffId,
                                                                          Boolean            SkipNotifications   = false,
                                                                          EventTracking_Id?  EventTrackingId     = null,
                                                                          User_Id?           CurrentUserId       = null)
        {

            CountryCode? countryCode   = default;
            Party_Id?    partyId       = default;

            foreach (var parties in tariffs.Values)
            {
                foreach (var tariffs in parties.Values)
                {
                    if (tariffs.TryGetValue(TariffId, out var tariff))
                    {
                        countryCode  = tariff.CountryCode;
                        partyId      = tariff.PartyId;
                    }
                }
            }

            if (countryCode.HasValue &&
                partyId.    HasValue)
            {

                var success = this.tariffs[countryCode.Value][partyId.Value].TryRemove(TariffId, out var removedTariffs);

                if (success)
                {

                    if (removedTariffs.Any())
                        await LogAsset(removeTariff,
                                       new JArray(
                                           removedTariffs.Select(tariff => tariff.ToJSON(
                                                                               true,
                                                                               true,
                                                                               CustomTariffSerializer,
                                                                               CustomDisplayTextSerializer,
                                                                               CustomPriceSerializer,
                                                                               CustomTariffElementSerializer,
                                                                               CustomPriceComponentSerializer,
                                                                               CustomTariffRestrictionsSerializer,
                                                                               CustomEnergyMixSerializer,
                                                                               CustomEnergySourceSerializer,
                                                                               CustomEnvironmentalImpactSerializer
                                                                           )
                                                                )
                                       ),
                                       EventTrackingId ?? EventTracking_Id.New,
                                       CurrentUserId);

                    if (!this.tariffs[countryCode.Value][partyId.Value].Any())
                        this.tariffs[countryCode.Value].Remove(partyId.Value, out _);

                    if (!this.tariffs[countryCode.Value].Any())
                        this.tariffs.Remove(countryCode.Value, out _);

                }

                return RemoveResult<IEnumerable<Tariff>>.Success(removedTariffs);

            }

            return RemoveResult<IEnumerable<Tariff>>.Failed(null, "RemoveTariff(TariffId, ...) failed!");

        }

        #endregion

        #region RemoveAllTariffs     (                                              SkipNotifications = false, ...)

        /// <summary>
        /// Remove all charging tariffs.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveAllTariffs(Boolean            SkipNotifications   = false,
                                                                              EventTracking_Id?  EventTrackingId     = null,
                                                                              User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTariffs = new List<Tariff>();

            foreach (var parties in tariffs.Values)
            {
                foreach (var tariffs in parties.Values)
                {
                    removedTariffs.AddRange(tariffs.Values());
                    tariffs.Clear();
                }
                parties.Clear();
            }

            tariffs.Clear();

            await LogAsset(removeAllTariffs,
                           new JArray(
                               removedTariffs.Select(tariff => tariff.ToJSON(
                                                                   true,
                                                                   true,
                                                                   CustomTariffSerializer,
                                                                   CustomDisplayTextSerializer,
                                                                   CustomPriceSerializer,
                                                                   CustomTariffElementSerializer,
                                                                   CustomPriceComponentSerializer,
                                                                   CustomTariffRestrictionsSerializer,
                                                                   CustomEnergyMixSerializer,
                                                                   CustomEnergySourceSerializer,
                                                                   CustomEnvironmentalImpactSerializer
                                                               )
                                                    )
                           ),
                           EventTrackingId,
                           CurrentUserId);

            return RemoveResult<IEnumerable<Tariff>>.Success(removedTariffs);

        }

        #endregion

        #region RemoveAllTariffs     (IncludeTariffs,                               SkipNotifications = false, ...)

        /// <summary>
        /// Remove all matching charging tariffs.
        /// </summary>
        /// <param name="IncludeTariffs">A charging tariff filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveAllTariffs(Func<Tariff, Boolean>  IncludeTariffs,
                                                                              Boolean                SkipNotifications   = false,
                                                                              EventTracking_Id?      EventTrackingId     = null,
                                                                              User_Id?               CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTariffs = new List<Tariff>();

            foreach (var parties in tariffs.Values)
            {

                foreach (var tariffs in parties.Values)
                {

                    foreach (var tariff in tariffs.Values())
                    {
                        if (IncludeTariffs(tariff))
                        {
                            tariffs.TryRemove(tariff.Id, out _);
                            removedTariffs.Add(tariff);
                        }
                    }

                    if (!tariffs.Any())
                        tariffs.Clear();

                }

                if (parties.IsEmpty)
                    tariffs.Clear();

            }

            await LogAsset(removeAllTariffs,
                           new JArray(
                               removedTariffs.Select(tariff => tariff.ToJSON(
                                                                   true,
                                                                   true,
                                                                   CustomTariffSerializer,
                                                                   CustomDisplayTextSerializer,
                                                                   CustomPriceSerializer,
                                                                   CustomTariffElementSerializer,
                                                                   CustomPriceComponentSerializer,
                                                                   CustomTariffRestrictionsSerializer,
                                                                   CustomEnergyMixSerializer,
                                                                   CustomEnergySourceSerializer,
                                                                   CustomEnvironmentalImpactSerializer
                                                               )
                                                    )
                           ),
                           EventTrackingId,
                           CurrentUserId);

            return RemoveResult<IEnumerable<Tariff>>.Success(removedTariffs);

        }

        #endregion

        #region RemoveAllTariffs     (IncludeTariffIds,                             SkipNotifications = false, ...)

        /// <summary>
        /// Remove all matching charging tariffs.
        /// </summary>
        /// <param name="IncludeTariffIds">A charging tariff identification filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveAllTariffs(Func<Tariff_Id, Boolean>  IncludeTariffIds,
                                                                              Boolean                   SkipNotifications   = false,
                                                                              EventTracking_Id?         EventTrackingId     = null,
                                                                              User_Id?                  CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTariffs = new List<Tariff>();

            foreach (var parties in tariffs.Values)
            {

                foreach (var tariffs in parties.Values)
                {

                    foreach (var tariff in tariffs.Values())
                    {
                        if (IncludeTariffIds(tariff.Id))
                        {
                            tariffs.TryRemove(tariff.Id, out _);
                            removedTariffs.Add(tariff);
                        }
                    }

                    if (!tariffs.Any())
                        tariffs.Clear();

                }

                if (parties.IsEmpty)
                    tariffs.Clear();

            }

            await LogAsset(removeAllTariffs,
                           new JArray(
                               removedTariffs.Select(tariff => tariff.ToJSON(
                                                                   true,
                                                                   true,
                                                                   CustomTariffSerializer,
                                                                   CustomDisplayTextSerializer,
                                                                   CustomPriceSerializer,
                                                                   CustomTariffElementSerializer,
                                                                   CustomPriceComponentSerializer,
                                                                   CustomTariffRestrictionsSerializer,
                                                                   CustomEnergyMixSerializer,
                                                                   CustomEnergySourceSerializer,
                                                                   CustomEnvironmentalImpactSerializer
                                                               )
                                                    )
                           ),
                           EventTrackingId,
                           CurrentUserId);

            return RemoveResult<IEnumerable<Tariff>>.Success(removedTariffs);

        }

        #endregion

        #region RemoveAllTariffs     (CountryCode, PartyId,                         SkipNotifications = false, ...)

        /// <summary>
        /// Remove all charging tariffs owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveAllTariffs(CountryCode        CountryCode,
                                                                              Party_Id           PartyId,
                                                                              Boolean            SkipNotifications   = false,
                                                                              EventTracking_Id?  EventTrackingId     = null,
                                                                              User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            await LogAssetComment($"{removeAllTariffs}: {CountryCode} {PartyId}",
                                  EventTrackingId,
                                  CurrentUserId);

            var removedTariffs = new List<Tariff>();

            if (tariffs.TryGetValue(CountryCode, out var parties))
            {
                if (parties.TryGetValue(PartyId, out var tariffs))
                {
                    removedTariffs.AddRange(tariffs.Values());
                    tariffs.Clear();
                }
            }

            await LogAsset(removeAllTariffs,
                           new JArray(
                               removedTariffs.Select(tariff => tariff.ToJSON(
                                                                   true,
                                                                   true,
                                                                   CustomTariffSerializer,
                                                                   CustomDisplayTextSerializer,
                                                                   CustomPriceSerializer,
                                                                   CustomTariffElementSerializer,
                                                                   CustomPriceComponentSerializer,
                                                                   CustomTariffRestrictionsSerializer,
                                                                   CustomEnergyMixSerializer,
                                                                   CustomEnergySourceSerializer,
                                                                   CustomEnvironmentalImpactSerializer
                                                               )
                                                    )
                           ),
                           EventTrackingId,
                           CurrentUserId);

            return RemoveResult<IEnumerable<Tariff>>.Success(removedTariffs);

        }

        #endregion


        #region TariffExists(CountryCode, PartyId, TariffId)

        public Boolean TariffExists(CountryCode  CountryCode,
                                    Party_Id     PartyId,
                                    Tariff_Id    TariffId)
        {

            lock (tariffs)
            {

                if (tariffs.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var tariffs))
                    {
                        return tariffs.ContainsKey(TariffId);
                    }
                }

                return false;

            }

        }

        #endregion

        #region TryGetTariff(CountryCode, PartyId, TariffId, out Tariff)

        public Boolean TryGetTariff(CountryCode  CountryCode,
                                    Party_Id     PartyId,
                                    Tariff_Id    TariffId,
                                    out Tariff?  Tariff)
        {

            lock (tariffs)
            {

                if (tariffs.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var tariffs))
                    {
                        if (tariffs.TryGetValue(TariffId, out Tariff))
                            return true;
                    }
                }

                Tariff = null;
                return false;

            }

        }

        #endregion

        #region GetTariffs  (IncludeTariff)

        public IEnumerable<Tariff> GetTariffs(Func<Tariff, Boolean>  IncludeTariff)
        {

            lock (tariffs)
            {

                var allTariffs = new List<Tariff>();

                foreach (var party in tariffs.Values)
                {
                    foreach (var partyTariffs in party.Values)
                    {
                        foreach (var tariff in partyTariffs.Values())
                        {
                            if (tariff is not null &&
                                IncludeTariff(tariff))
                            {
                                allTariffs.Add(tariff);
                            }
                        }
                    }
                }

                return allTariffs;

            }

        }

        #endregion

        #region GetTariffs  (CountryCode = null, PartyId = null)

        public IEnumerable<Tariff> GetTariffs(CountryCode?  CountryCode   = null,
                                              Party_Id?     PartyId       = null)
        {

            lock (tariffs)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (tariffs.TryGetValue(CountryCode.Value, out var parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out var tariffs))
                        {
                            return tariffs.Values().ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allTariffs = new List<Tariff>();

                    foreach (var party in tariffs.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out var tariffs))
                        {
                            allTariffs.AddRange(tariffs.Values());
                        }
                    }

                    return allTariffs;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (tariffs.TryGetValue(CountryCode.Value, out var parties))
                    {

                        var allTariffs = new List<Tariff>();

                        foreach (var tariffs in parties.Values)
                        {
                            allTariffs.AddRange(tariffs.Values());
                        }

                        return allTariffs;

                    }
                }

                else
                {

                    var allTariffs = new List<Tariff>();

                    foreach (var party in tariffs.Values)
                    {
                        foreach (var tariffs in party.Values)
                        {
                            allTariffs.AddRange(tariffs.Values());
                        }
                    }

                    return allTariffs;

                }

                return [];

            }

        }

        #endregion

        #region GetTariffIds(CountryCode?, PartyId?, LocationId?, EVSEUId?, ConnectorId?, EMSPId?)

        public IEnumerable<Tariff_Id> GetTariffIds(CountryCode    CountryCode,
                                                   Party_Id       PartyId,
                                                   Location_Id?   LocationId,
                                                   EVSE_UId?      EVSEUId,
                                                   Connector_Id?  ConnectorId,
                                                   EMSP_Id?       EMSPId)

            => GetTariffIdsDelegate?.Invoke(CountryCode,
                                            PartyId,
                                            LocationId,
                                            EVSEUId,
                                            ConnectorId,
                                            EMSPId) ?? [];

        #endregion

        #endregion

        #region Sessions

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Session_Id , Session>>> chargingSessions;


        public delegate Task OnSessionCreatedDelegate(Session Session);

        public event OnSessionCreatedDelegate? OnSessionCreated;

        public delegate Task OnSessionChangedDelegate(Session Session);

        public event OnSessionChangedDelegate? OnSessionChanged;


        #region AddSession           (Session, SkipNotifications = false)

        public Session AddSession(Session            Session,
                                  Boolean            SkipNotifications   = false,
                                  EventTracking_Id?  EventTrackingId     = null,
                                  User_Id?           CurrentUserId       = null)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (chargingSessions)
            {

                if (!this.chargingSessions.TryGetValue(Session.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Session_Id, Session>>();
                    this.chargingSessions.Add(Session.CountryCode, parties);
                }

                if (!parties.TryGetValue(Session.PartyId, out var sessions))
                {
                    sessions = new Dictionary<Session_Id, Session>();
                    parties.Add(Session.PartyId, sessions);
                }

                if (!sessions.ContainsKey(Session.Id))
                {

                    sessions.Add(Session.Id, Session);
                    Session.CommonAPI = this;

                    if (!SkipNotifications)
                    {

                        var OnSessionCreatedLocal = OnSessionCreated;
                        if (OnSessionCreatedLocal is not null)
                        {
                            try
                            {
                                OnSessionCreatedLocal(Session).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddSession), " ", nameof(OnSessionCreated), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return Session;

                }

                throw new ArgumentException("The given session already exists!");

            }

        }

        #endregion

        #region AddSessionIfNotExists(Session, SkipNotifications = false)

        public Session AddSessionIfNotExists(Session            Session,
                                             Boolean            SkipNotifications   = false,
                                             EventTracking_Id?  EventTrackingId     = null,
                                             User_Id?           CurrentUserId       = null)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (chargingSessions)
            {

                if (!this.chargingSessions.TryGetValue(Session.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Session_Id, Session>>();
                    this.chargingSessions.Add(Session.CountryCode, parties);
                }

                if (!parties.TryGetValue(Session.PartyId, out var sessions))
                {
                    sessions = new Dictionary<Session_Id, Session>();
                    parties.Add(Session.PartyId, sessions);
                }

                if (!sessions.ContainsKey(Session.Id))
                {

                    sessions.Add(Session.Id, Session);
                    Session.CommonAPI = this;

                    if (!SkipNotifications)
                    {

                        var OnSessionCreatedLocal = OnSessionCreated;
                        if (OnSessionCreatedLocal is not null)
                        {
                            try
                            {
                                OnSessionCreatedLocal(Session).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddSessionIfNotExists), " ", nameof(OnSessionCreated), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                }

                return Session;

            }

        }

        #endregion

        #region AddOrUpdateSession   (newOrUpdatedSession, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<Session>> AddOrUpdateSession(Session            newOrUpdatedSession,
                                                                         Boolean?           AllowDowngrades   = false,
                                                                         EventTracking_Id?  EventTrackingId   = null,
                                                                         User_Id?           CurrentUserId     = null)
        {

            if (newOrUpdatedSession is null)
                throw new ArgumentNullException(nameof(newOrUpdatedSession), "The given charging session must not be null!");

            lock (chargingSessions)
            {

                if (!this.chargingSessions.TryGetValue(newOrUpdatedSession.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Session_Id, Session>>();
                    this.chargingSessions.Add(newOrUpdatedSession.CountryCode, parties);
                }

                if (!parties.TryGetValue(newOrUpdatedSession.PartyId, out var sessions))
                {
                    sessions = new Dictionary<Session_Id, Session>();
                    parties.Add(newOrUpdatedSession.PartyId, sessions);
                }

                if (sessions.TryGetValue(newOrUpdatedSession.Id, out var existingSession))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        newOrUpdatedSession.LastUpdated <= existingSession.LastUpdated)
                    {
                        return AddOrUpdateResult<Session>.Failed(newOrUpdatedSession,
                                                                 "The 'lastUpdated' timestamp of the new charging session must be newer then the timestamp of the existing session!");
                    }

                    sessions[newOrUpdatedSession.Id] = newOrUpdatedSession;

                    var OnSessionChangedLocal = OnSessionChanged;
                    if (OnSessionChangedLocal is not null)
                    {
                        try
                        {
                            OnSessionChangedLocal(newOrUpdatedSession).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateSession), " ", nameof(OnSessionChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    return AddOrUpdateResult<Session>.Success(newOrUpdatedSession,
                                                              WasCreated: false);

                }

                sessions.Add(newOrUpdatedSession.Id, newOrUpdatedSession);

                var OnSessionCreatedLocal = OnSessionCreated;
                if (OnSessionCreatedLocal is not null)
                {
                    try
                    {
                        OnSessionCreatedLocal(newOrUpdatedSession).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateSession), " ", nameof(OnSessionCreated), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

                return AddOrUpdateResult<Session>.Success(newOrUpdatedSession,
                                                          WasCreated: true);

            }

        }

        #endregion

        #region UpdateSession        (Session)

        public Session? UpdateSession(Session            Session,
                                      EventTracking_Id?  EventTrackingId   = null,
                                      User_Id?           CurrentUserId     = null)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (chargingSessions)
            {

                if (chargingSessions.TryGetValue(Session.CountryCode, out var parties)  &&
                    parties. TryGetValue(Session.PartyId,     out var _sessions) &&
                    _sessions.ContainsKey(Session.Id))
                {

                    _sessions[Session.Id] = Session;

                    var OnSessionChangedLocal = OnSessionChanged;
                    if (OnSessionChangedLocal is not null)
                    {
                        try
                        {
                            OnSessionChangedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateSession), " ", nameof(OnSessionChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    return Session;

                }

                return null;

            }

        }

        #endregion


        #region TryPatchSession      (Session, SessionPatch, AllowDowngrades = false)

        public async Task<PatchResult<Session>> TryPatchSession(Session            Session,
                                                                JObject            SessionPatch,
                                                                Boolean?           AllowDowngrades   = false,
                                                                EventTracking_Id?  EventTrackingId   = null,
                                                                User_Id?           CurrentUserId     = null)
        {

            if (Session is null)
                return PatchResult<Session>.Failed(Session,
                                                   "The given charging session must not be null!");

            if (SessionPatch is null || !SessionPatch.HasValues)
                return PatchResult<Session>.Failed(Session,
                                                   "The given charging session patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (chargingSessions)
            {

                if (chargingSessions.TryGetValue(Session.CountryCode, out var parties)  &&
                    parties.         TryGetValue(Session.PartyId,     out var sessions) &&
                    sessions.        TryGetValue(Session.Id,          out var session))
                {

                    var patchResult = session.TryPatch(SessionPatch,
                                                       AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                    {

                        sessions[Session.Id] = patchResult.PatchedData;

                        var OnSessionChangedLocal = OnSessionChanged;
                        if (OnSessionChangedLocal is not null)
                        {
                            try
                            {
                                OnSessionChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchSession), " ", nameof(OnSessionChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return patchResult;

                }

                else
                    return PatchResult<Session>.Failed(Session,
                                                       "The given charging session does not exist!");

            }

        }

        #endregion


        #region SessionExists(CountryCode, PartyId, SessionId)

        public Boolean SessionExists(CountryCode  CountryCode,
                                     Party_Id     PartyId,
                                     Session_Id   SessionId)
        {

            lock (chargingSessions)
            {

                if (chargingSessions.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var _sessions))
                    {
                        return _sessions.ContainsKey(SessionId);
                    }
                }

                return false;

            }

        }

        #endregion

        #region TryGetSession(CountryCode, PartyId, SessionId, out Session)

        public Boolean TryGetSession(CountryCode   CountryCode,
                                     Party_Id      PartyId,
                                     Session_Id    SessionId,
                                     out Session?  Session)
        {

            lock (chargingSessions)
            {

                if (chargingSessions.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var _sessions))
                    {
                        if (_sessions.TryGetValue(SessionId, out Session))
                            return true;
                    }
                }

                Session = null;
                return false;

            }

        }

        #endregion

        #region GetSessions  (IncludeSession)

        public IEnumerable<Session> GetSessions(Func<Session, Boolean> IncludeSession)
        {

            lock (chargingSessions)
            {

                var allSessions = new List<Session>();

                foreach (var party in chargingSessions.Values)
                {
                    foreach (var partySessions in party.Values)
                    {
                        foreach (var session in partySessions.Values)
                        {
                            if (session is not null &&
                                IncludeSession(session))
                            {
                                allSessions.Add(session);
                            }
                        }
                    }
                }

                return allSessions;

            }

        }

        #endregion

        #region GetSessions  (CountryCode = null, PartyId = null)

        public IEnumerable<Session> GetSessions(CountryCode? CountryCode   = null,
                                                Party_Id?    PartyId       = null)
        {

            lock (chargingSessions)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (chargingSessions.TryGetValue(CountryCode.Value, out var parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out var _sessions))
                        {
                            return _sessions.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allSessions = new List<Session>();

                    foreach (var party in chargingSessions.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out var sessions))
                        {
                            allSessions.AddRange(sessions.Values);
                        }
                    }

                    return allSessions;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (chargingSessions.TryGetValue(CountryCode.Value, out var parties))
                    {

                        var allSessions = new List<Session>();

                        foreach (var sessions in parties.Values)
                        {
                            allSessions.AddRange(sessions.Values);
                        }

                        return allSessions;

                    }
                }

                else
                {

                    var allSessions = new List<Session>();

                    foreach (var party in chargingSessions.Values)
                    {
                        foreach (var sessions in party.Values)
                        {
                            allSessions.AddRange(sessions.Values);
                        }
                    }

                    return allSessions;

                }

                return Array.Empty<Session>();

            }

        }

        #endregion


        #region RemoveSession    (Session)

        public Boolean RemoveSession(Session            Session,
                                     EventTracking_Id?  EventTrackingId   = null,
                                     User_Id?           CurrentUserId     = null)
        {

            lock (chargingSessions)
            {

                var success = false;

                if (chargingSessions.TryGetValue(Session.CountryCode, out var parties))
                {

                    if (parties.TryGetValue(Session.PartyId, out var _sessions))
                    {

                        if (_sessions.ContainsKey(Session.Id))
                        {
                            success = _sessions.Remove(Session.Id);
                        }

                        if (!_sessions.Any())
                            parties.Remove(Session.PartyId);

                    }

                    if (!parties.Any())
                        chargingSessions.Remove(Session.CountryCode);

                }

                return success;

            }

        }

        #endregion

        #region RemoveSession    (SessionId)

        public Boolean RemoveSession(Session_Id         SessionId,
                                     EventTracking_Id?  EventTrackingId   = null,
                                     User_Id?           CurrentUserId     = null)
        {

            lock (chargingSessions)
            {

                CountryCode? countryCode   = default;
                Party_Id?    partyId       = default;

                foreach (var parties in chargingSessions.Values)
                {
                    foreach (var sessions in parties.Values)
                    {
                        if (sessions.TryGetValue(SessionId, out var session))
                        {
                            countryCode  = session.CountryCode;
                            partyId      = session.PartyId;
                        }
                    }
                }

                if (countryCode.HasValue &&
                    partyId.    HasValue)
                {
                    return chargingSessions[countryCode.Value][partyId.Value].Remove(SessionId);
                }

                return false;

            }

        }

        #endregion

        #region RemoveAllSessions(IncludeSessions = null)

        /// <summary>
        /// Remove all matching sessions.
        /// </summary>
        /// <param name="IncludeSessions">An optional charging session filter.</param>
        public void RemoveAllSessions(Func<Session, Boolean>?  IncludeSessions   = null,
                                      EventTracking_Id?        EventTrackingId   = null,
                                      User_Id?                 CurrentUserId     = null)
        {

            lock (chargingSessions)
            {

                if (IncludeSessions is null)
                    chargingSessions.Clear();

                else
                {

                    var sessionsToDelete = chargingSessions.Values.SelectMany(xx => xx.Values).
                                                                   SelectMany(yy => yy.Values).
                                                                   Where     (IncludeSessions).
                                                                   ToArray   ();

                    foreach (var session in sessionsToDelete)
                        chargingSessions[session.CountryCode][session.PartyId].Remove(session.Id);

                }

            }

        }

        #endregion

        #region RemoveAllSessions(CountryCode, PartyId)

        /// <summary>
        /// Remove all sessions owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllSessions(CountryCode        CountryCode,
                                      Party_Id           PartyId,
                                      EventTracking_Id?  EventTrackingId   = null,
                                      User_Id?           CurrentUserId     = null)
        {

            lock (chargingSessions)
            {

                if (chargingSessions.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var _sessions))
                    {
                        _sessions.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion

        #region Tokens

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>>> tokenStatus;


        public delegate Task OnTokenAddedDelegate(Token Token);

        public event OnTokenAddedDelegate? OnTokenAdded;


        public delegate Task OnTokenChangedDelegate(Token Token);

        public event OnTokenChangedDelegate? OnTokenChanged;


        public delegate Task<TokenStatus> OnVerifyTokenDelegate(CountryCode  CountryCode,
                                                                Party_Id     PartyId,
                                                                Token_Id     TokenId);

        public event OnVerifyTokenDelegate? OnVerifyToken;


        #region AddToken           (Token, Status = AllowedType.ALLOWED, SkipNotifications = false)

        public Token AddToken(Token              Token,
                              AllowedType?       Status              = null,
                              Boolean            SkipNotifications   = false,
                              EventTracking_Id?  EventTrackingId     = null,
                              User_Id?           CurrentUserId       = null)
        {

            Status ??= AllowedType.ALLOWED;

            lock (tokenStatus)
            {

                if (!tokenStatus.TryGetValue(Token.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>>();
                    tokenStatus.Add(Token.CountryCode, parties);
                }

                if (!parties.TryGetValue(Token.PartyId, out var tokens))
                {
                    tokens = new Dictionary<Token_Id, TokenStatus>();
                    parties.Add(Token.PartyId, tokens);
                }

                if (!tokens.ContainsKey(Token.Id))
                {

                    tokens.Add(Token.Id, new TokenStatus(Token, Status.Value));
                    Token.CommonAPI = this;

                    if (!SkipNotifications)
                    {

                        var OnTokenAddedLocal = OnTokenAdded;
                        if (OnTokenAddedLocal is not null)
                        {
                            try
                            {
                                OnTokenAddedLocal(Token).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddToken), " ", nameof(OnTokenAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return Token;

                }

                throw new ArgumentException("The given token already exists!");

            }

        }

        #endregion

        #region AddTokenIfNotExists(Token, Status = AllowedType.ALLOWED, SkipNotifications = false)

        public Token AddTokenIfNotExists(Token              Token,
                                         AllowedType?       Status              = null,
                                         Boolean            SkipNotifications   = false,
                                         EventTracking_Id?  EventTrackingId     = null,
                                         User_Id?           CurrentUserId       = null)
        {

            Status ??= AllowedType.ALLOWED;

            lock (tokenStatus)
            {

                if (!tokenStatus.TryGetValue(Token.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>>();
                    tokenStatus.Add(Token.CountryCode, parties);
                }

                if (!parties.TryGetValue(Token.PartyId, out var tokens))
                {
                    tokens = new Dictionary<Token_Id, TokenStatus>();
                    parties.Add(Token.PartyId, tokens);
                }

                if (!tokens.ContainsKey(Token.Id))
                {

                    tokens.Add(Token.Id, new TokenStatus(Token, Status.Value));
                    Token.CommonAPI = this;

                    if (!SkipNotifications)
                    {

                        var OnTokenAddedLocal = OnTokenAdded;
                        if (OnTokenAddedLocal is not null)
                        {
                            try
                            {
                                OnTokenAddedLocal(Token).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddTokenIfNotExists), " ", nameof(OnTokenAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                }

                return Token;

            }

        }

        #endregion

        #region AddOrUpdateToken   (newOrUpdatedToken, Status = AllowedTypes.ALLOWED, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<Token>> AddOrUpdateToken(Token              newOrUpdatedToken,
                                                                     AllowedType?       Status            = null,
                                                                     Boolean?           AllowDowngrades   = false,
                                                                     EventTracking_Id?  EventTrackingId   = null,
                                                                     User_Id?           CurrentUserId     = null)
        {

            Status ??= AllowedType.ALLOWED;

            if (newOrUpdatedToken is null)
                throw new ArgumentNullException(nameof(newOrUpdatedToken), "The given token must not be null!");

            lock (tokenStatus)
            {

                if (!tokenStatus.TryGetValue(newOrUpdatedToken.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>>();
                    tokenStatus.Add(newOrUpdatedToken.CountryCode, parties);
                }

                if (!parties.TryGetValue(newOrUpdatedToken.PartyId, out var _tokenStatus))
                {
                    _tokenStatus = new Dictionary<Token_Id, TokenStatus>();
                    parties.Add(newOrUpdatedToken.PartyId, _tokenStatus);
                }

                if (_tokenStatus.TryGetValue(newOrUpdatedToken.Id, out var existingTokenStatus))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        newOrUpdatedToken.LastUpdated <= existingTokenStatus.Token.LastUpdated)
                    {
                        return AddOrUpdateResult<Token>.Failed(newOrUpdatedToken,
                                                               "The 'lastUpdated' timestamp of the new charging token must be newer then the timestamp of the existing token!");
                    }

                    _tokenStatus[newOrUpdatedToken.Id] = new TokenStatus(newOrUpdatedToken,
                                                                         Status.Value);

                    var OnTokenChangedLocal = OnTokenChanged;
                    if (OnTokenChangedLocal is not null)
                    {
                        try
                        {
                            OnTokenChangedLocal(newOrUpdatedToken).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateToken), " ", nameof(OnTokenChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    return AddOrUpdateResult<Token>.Success(newOrUpdatedToken,
                                                            WasCreated: false);

                }

                _tokenStatus.Add(newOrUpdatedToken.Id, new TokenStatus(newOrUpdatedToken,
                                                                       Status.Value));

                var OnTokenAddedLocal = OnTokenAdded;
                if (OnTokenAddedLocal is not null)
                {
                    try
                    {
                        OnTokenAddedLocal(newOrUpdatedToken).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateToken), " ", nameof(OnTokenAdded), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

                return AddOrUpdateResult<Token>.Success(newOrUpdatedToken,
                                                        WasCreated: true);

            }

        }

        #endregion


        #region TryPatchToken      (Token, TokenPatch, AllowDowngrades = false)

        public async Task<PatchResult<Token>> TryPatchToken(Token              Token,
                                                            JObject            TokenPatch,
                                                            Boolean?           AllowDowngrades   = false,
                                                            EventTracking_Id?  EventTrackingId   = null,
                                                            User_Id?           CurrentUserId     = null)
        {

            if (Token is null)
                return PatchResult<Token>.Failed(Token,
                                                 "The given token must not be null!");

            if (TokenPatch is null || !TokenPatch.HasValues)
                return PatchResult<Token>.Failed(Token,
                                                 "The given token patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (tokenStatus)
            {

                if (tokenStatus.TryGetValue(Token.CountryCode, out var parties) &&
                    parties.    TryGetValue(Token.PartyId,     out var tokens)  &&
                    tokens.     TryGetValue(Token.Id,          out var _tokenStatus))
                {

                    var patchResult = _tokenStatus.Token.TryPatch(TokenPatch,
                                                                  AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                    {

                        tokens[Token.Id] = new TokenStatus(patchResult.PatchedData,
                                                           _tokenStatus.Status);

                        var OnTokenChangedLocal = OnTokenChanged;
                        if (OnTokenChangedLocal is not null)
                        {
                            try
                            {
                                OnTokenChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchToken), " ", nameof(OnTokenChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return patchResult;

                }

                else
                    return PatchResult<Token>.Failed(Token,
                                                      "The given charging token does not exist!");

            }

        }

        #endregion


        #region TokenExists(CountryCode, PartyId, TokenId)

        public Boolean TokenExists(CountryCode  CountryCode,
                                   Party_Id     PartyId,
                                   Token_Id     TokenId)
        {

            lock (tokenStatus)
            {

                if (tokenStatus.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var tokens))
                    {
                        return tokens.ContainsKey(TokenId);
                    }
                }

                return false;

            }

        }

        #endregion

        #region TryGetToken(CountryCode, PartyId, TokenId, out TokenWithStatus)

        public Boolean TryGetToken(CountryCode      CountryCode,
                                   Party_Id         PartyId,
                                   Token_Id         TokenId,
                                   out TokenStatus  TokenWithStatus)
        {

            lock (tokenStatus)
            {

                if (tokenStatus.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var tokens))
                    {
                        if (tokens.TryGetValue(TokenId, out TokenWithStatus))
                            return true;
                    }
                }

                var VerifyTokenLocal = OnVerifyToken;
                if (VerifyTokenLocal is not null)
                {

                    try
                    {

                        var result = VerifyTokenLocal(CountryCode,
                                                      PartyId,
                                                      TokenId).Result;

                        TokenWithStatus = result;

                        return true;

                    } catch (Exception e)
                    {

                    }

                }

                TokenWithStatus = default;
                return false;

            }

        }

        #endregion

        #region GetTokens  (IncludeToken)

        public IEnumerable<TokenStatus> GetTokens(Func<Token, Boolean> IncludeToken)
        {

            lock (tokenStatus)
            {

                var allTokenStatus = new List<TokenStatus>();

                foreach (var party in tokenStatus.Values)
                {
                    foreach (var partyTokens in party.Values)
                    {
                        foreach (var tokenStatus in partyTokens.Values)
                        {
                            if (IncludeToken(tokenStatus.Token))
                            {
                                allTokenStatus.Add(tokenStatus);
                            }
                        }
                    }
                }

                return allTokenStatus;

            }

        }

        #endregion

        #region GetTokens  (IncludeTokenStatus)

        public IEnumerable<TokenStatus> GetTokens(Func<TokenStatus, Boolean> IncludeTokenStatus)
        {

            lock (tokenStatus)
            {

                var allTokenStatus = new List<TokenStatus>();

                foreach (var party in tokenStatus.Values)
                {
                    foreach (var partyTokens in party.Values)
                    {
                        foreach (var tokenStatus in partyTokens.Values)
                        {
                            if (IncludeTokenStatus(tokenStatus))
                            {
                                allTokenStatus.Add(tokenStatus);
                            }
                        }
                    }
                }

                return allTokenStatus;

            }

        }

        #endregion

        #region GetTokens  (CountryCode = null, PartyId = null)

        public IEnumerable<TokenStatus> GetTokens(CountryCode?  CountryCode   = null,
                                                  Party_Id?     PartyId       = null)
        {

            lock (tokenStatus)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (tokenStatus.TryGetValue(CountryCode.Value, out var parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out var tokens))
                        {
                            return tokens.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allTokens = new List<TokenStatus>();

                    foreach (var party in tokenStatus.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out var tokens))
                        {
                            allTokens.AddRange(tokens.Values);
                        }
                    }

                    return allTokens;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (tokenStatus.TryGetValue(CountryCode.Value, out var parties))
                    {

                        var allTokens = new List<TokenStatus>();

                        foreach (var tokens in parties.Values)
                        {
                            allTokens.AddRange(tokens.Values);
                        }

                        return allTokens;

                    }
                }

                else
                {

                    var allTokens = new List<TokenStatus>();

                    foreach (var party in tokenStatus.Values)
                    {
                        foreach (var tokens in party.Values)
                        {
                            allTokens.AddRange(tokens.Values);
                        }
                    }

                    return allTokens;

                }

                return Array.Empty<TokenStatus>();

            }

        }

        #endregion


        #region RemoveToken    (Token)

        public Boolean RemoveToken(Token              Token,
                                   EventTracking_Id?  EventTrackingId   = null,
                                   User_Id?           CurrentUserId     = null)
        {

            lock (tokenStatus)
            {

                var success = false;

                if (tokenStatus.TryGetValue(Token.CountryCode, out var parties))
                {

                    if (parties.TryGetValue(Token.PartyId, out var _tokenStatus))
                    {

                        if (_tokenStatus.ContainsKey(Token.Id))
                        {
                            success = _tokenStatus.Remove(Token.Id);
                        }

                        if (!_tokenStatus.Any())
                            parties.Remove(Token.PartyId);

                    }

                    if (!parties.Any())
                        chargingSessions.Remove(Token.CountryCode);

                }

                return success;

            }

        }

        #endregion

        #region RemoveToken    (TokenId)

        public Boolean RemoveToken(Token_Id           TokenId,
                                   EventTracking_Id?  EventTrackingId   = null,
                                   User_Id?           CurrentUserId     = null)
        {

            lock (tokenStatus)
            {

                CountryCode? countryCode  = default;
                Party_Id?    partyId      = default;

                foreach (var parties in tokenStatus.Values)
                {
                    foreach (var _tokenStatus in parties.Values)
                    {
                        if (_tokenStatus.TryGetValue(TokenId, out var __tokenStatus))
                        {
                            countryCode = __tokenStatus.Token.CountryCode;
                            partyId     = __tokenStatus.Token.PartyId;
                        }
                    }
                }

                if (countryCode.HasValue &&
                    partyId.    HasValue)
                {
                    return tokenStatus[countryCode.Value][partyId.Value].Remove(TokenId);
                }

                return false;

            }

        }

        #endregion

        #region RemoveAllTokens(IncludeTokens = null)

        /// <summary>
        /// Remove all matching tokens.
        /// </summary>
        /// <param name="IncludeSessions">An optional token filter.</param>
        public void RemoveAllTokens(Func<Token, Boolean>? IncludeTokens     = null,
                                    EventTracking_Id?     EventTrackingId   = null,
                                    User_Id?              CurrentUserId     = null)
        {

            lock (tokenStatus)
            {

                if (IncludeTokens is null)
                    tokenStatus.Clear();

                else
                {

                    var tokensToDelete = tokenStatus.Values.SelectMany(xx => xx.Values).
                                                            SelectMany(yy => yy.Values).
                                                            Where     (tokenStatus => IncludeTokens(tokenStatus.Token)).
                                                            Select    (tokenStatus => tokenStatus.Token).
                                                            ToArray   ();

                    foreach (var token in tokensToDelete)
                        tokenStatus[token.CountryCode][token.PartyId].Remove(token.Id);

                }

            }

        }

        #endregion

        #region RemoveAllTokens(CountryCode, PartyId)

        /// <summary>
        /// Remove all tokens owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllTokens(CountryCode        CountryCode,
                                    Party_Id           PartyId,
                                    EventTracking_Id?  EventTrackingId   = null,
                                    User_Id?           CurrentUserId     = null)
        {

            lock (tokenStatus)
            {

                if (tokenStatus.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var tokens))
                    {
                        tokens.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion

        #region ChargeDetailRecords

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<CDR_Id, CDR>>> ChargeDetailRecords;


        public delegate Task OnChargeDetailRecordAddedDelegate(CDR CDR);

        public event OnChargeDetailRecordAddedDelegate? OnChargeDetailRecordAdded;


        public delegate Task OnChargeDetailRecordChangedDelegate(CDR CDR);

        public event OnChargeDetailRecordChangedDelegate? OnChargeDetailRecordChanged;


        public delegate Task<CDR> OnChargeDetailRecordLookupDelegate(CountryCode  CountryCode,
                                                                     Party_Id     PartyId,
                                                                     CDR_Id       CDRId);

        public event OnChargeDetailRecordLookupDelegate? OnChargeDetailRecordLookup;


        #region AddCDR           (CDR, SkipNotifications = false)

        public CDR AddCDR(CDR                CDR,
                          Boolean            SkipNotifications   = false,
                          EventTracking_Id?  EventTrackingId     = null,
                          User_Id?           CurrentUserId       = null)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (ChargeDetailRecords)
            {

                if (!ChargeDetailRecords.TryGetValue(CDR.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<CDR_Id, CDR>>();
                    ChargeDetailRecords.Add(CDR.CountryCode, parties);
                }

                if (!parties.TryGetValue(CDR.PartyId, out var partyCDRs))
                {
                    partyCDRs = new Dictionary<CDR_Id, CDR>();
                    parties.Add(CDR.PartyId, partyCDRs);
                }

                if (!partyCDRs.ContainsKey(CDR.Id))
                {

                    partyCDRs.Add(CDR.Id, CDR);
                    CDR.CommonAPI = this;

                    if (!SkipNotifications)
                    {

                        var OnChargeDetailRecordAddedLocal = OnChargeDetailRecordAdded;
                        if (OnChargeDetailRecordAddedLocal is not null)
                        {
                            try
                            {
                                OnChargeDetailRecordAddedLocal(CDR).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddCDR), " ", nameof(OnChargeDetailRecordAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return CDR;

                }

                throw new ArgumentException("The given charge detail record already exists!");

            }

        }

        #endregion

        #region AddCDRIfNotExists(CDR, SkipNotifications = false)

        public CDR AddCDRIfNotExists(CDR                CDR,
                                     Boolean            SkipNotifications   = false,
                                     EventTracking_Id?  EventTrackingId     = null,
                                     User_Id?           CurrentUserId       = null)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (ChargeDetailRecords)
            {

                if (!ChargeDetailRecords.TryGetValue(CDR.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<CDR_Id, CDR>>();
                    ChargeDetailRecords.Add(CDR.CountryCode, parties);
                }

                if (!parties.TryGetValue(CDR.PartyId, out var partyCDRs))
                {
                    partyCDRs = new Dictionary<CDR_Id, CDR>();
                    parties.Add(CDR.PartyId, partyCDRs);
                }

                if (!partyCDRs.ContainsKey(CDR.Id))
                {

                    partyCDRs.Add(CDR.Id, CDR);
                    CDR.CommonAPI = this;

                    if (!SkipNotifications)
                    {

                        var OnChargeDetailRecordAddedLocal = OnChargeDetailRecordAdded;
                        if (OnChargeDetailRecordAddedLocal is not null)
                        {
                            try
                            {
                                OnChargeDetailRecordAddedLocal(CDR).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddCDRIfNotExists), " ", nameof(OnChargeDetailRecordAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                }

                return CDR;

            }

        }

        #endregion

        #region AddOrUpdateCDR   (newOrUpdatedCDR, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<CDR>> AddOrUpdateCDR(CDR                newOrUpdatedCDR,
                                                                 Boolean?           AllowDowngrades   = false,
                                                                 EventTracking_Id?  EventTrackingId   = null,
                                                                 User_Id?           CurrentUserId     = null)
        {

            if (newOrUpdatedCDR is null)
                throw new ArgumentNullException(nameof(newOrUpdatedCDR), "The given charge detail record must not be null!");

            lock (ChargeDetailRecords)
            {

                if (!ChargeDetailRecords.TryGetValue(newOrUpdatedCDR.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<CDR_Id, CDR>>();
                    ChargeDetailRecords.Add(newOrUpdatedCDR.CountryCode, parties);
                }

                if (!parties.TryGetValue(newOrUpdatedCDR.PartyId, out var CDRs))
                {
                    CDRs = new Dictionary<CDR_Id, CDR>();
                    parties.Add(newOrUpdatedCDR.PartyId, CDRs);
                }

                if (CDRs.TryGetValue(newOrUpdatedCDR.Id, out var existingCDR))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        newOrUpdatedCDR.LastUpdated <= existingCDR.LastUpdated)
                    {
                        return AddOrUpdateResult<CDR>.Failed(newOrUpdatedCDR,
                                                             "The 'lastUpdated' timestamp of the new charge detail record must be newer then the timestamp of the existing charge detail record!");
                    }

                    CDRs[newOrUpdatedCDR.Id] = newOrUpdatedCDR;

                    var OnChargeDetailRecordChangedLocal = OnChargeDetailRecordChanged;
                    if (OnChargeDetailRecordChangedLocal is not null)
                    {
                        try
                        {
                            OnChargeDetailRecordChangedLocal(newOrUpdatedCDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateCDR), " ", nameof(OnChargeDetailRecordChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    return AddOrUpdateResult<CDR>.Success(newOrUpdatedCDR,
                                                          WasCreated: false);

                }

                CDRs.Add(newOrUpdatedCDR.Id, newOrUpdatedCDR);

                var OnChargeDetailRecordAddedLocal = OnChargeDetailRecordAdded;
                if (OnChargeDetailRecordAddedLocal is not null)
                {
                    try
                    {
                        OnChargeDetailRecordAddedLocal(newOrUpdatedCDR).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateCDR), " ", nameof(OnChargeDetailRecordAdded), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

                return AddOrUpdateResult<CDR>.Success(newOrUpdatedCDR,
                                                      WasCreated: true);

            }

        }

        #endregion

        #region UpdateCDR        (CDR)

        public CDR? UpdateCDR(CDR                CDR,
                              EventTracking_Id?  EventTrackingId   = null,
                              User_Id?           CurrentUserId     = null)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (ChargeDetailRecords)
            {

                if (ChargeDetailRecords.TryGetValue(CDR.CountryCode, out var parties)   &&
                    parties.            TryGetValue(CDR.PartyId,     out var partyCDRs) &&
                    partyCDRs.          ContainsKey(CDR.Id))
                {

                    partyCDRs[CDR.Id] = CDR;

                    var OnChargeDetailRecordChangedLocal = OnChargeDetailRecordChanged;
                    if (OnChargeDetailRecordChangedLocal is not null)
                    {
                        try
                        {
                            OnChargeDetailRecordChangedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateCDR), " ", nameof(OnChargeDetailRecordChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    return CDR;

                }

                return null;

            }

        }

        #endregion


        #region CDRExists(CountryCode, PartyId, CDRId)

        public Boolean CDRExists(CountryCode  CountryCode,
                                 Party_Id     PartyId,
                                 CDR_Id       CDRId)
        {

            lock (ChargeDetailRecords)
            {

                if (ChargeDetailRecords.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var partyCDRs))
                    {
                        return partyCDRs.ContainsKey(CDRId);
                    }
                }

                return false;

            }

        }

        #endregion

        #region TryGetCDR(CountryCode, PartyId, CDRId, out CDR)

        public Boolean TryGetCDR(CountryCode  CountryCode,
                                 Party_Id     PartyId,
                                 CDR_Id       CDRId,
                                 out CDR?     CDR)
        {

            lock (ChargeDetailRecords)
            {

                if (ChargeDetailRecords.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var partyCDRs))
                    {
                        if (partyCDRs.TryGetValue(CDRId, out CDR))
                            return true;
                    }
                }

                var OnChargeDetailRecordLookupLocal = OnChargeDetailRecordLookup;
                if (OnChargeDetailRecordLookupLocal is not null)
                {
                    try
                    {

                        var cdr = OnChargeDetailRecordLookupLocal(CountryCode,
                                                                  PartyId,
                                                                  CDRId).Result;

                        if (cdr is not null)
                        {
                            CDR = cdr;
                            return true;
                        }

                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryGetCDR), " ", nameof(OnChargeDetailRecordLookup), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }


                CDR = null;
                return false;

            }

        }

        #endregion

        #region GetCDRs  (IncludeCDR)

        public IEnumerable<CDR> GetCDRs(Func<CDR, Boolean> IncludeCDR)
        {

            lock (ChargeDetailRecords)
            {

                var allCDRs = new List<CDR>();

                foreach (var party in ChargeDetailRecords.Values)
                {
                    foreach (var partyCDRs in party.Values)
                    {
                        foreach (var cdr in partyCDRs.Values)
                        {
                            if (cdr is not null &&
                                IncludeCDR(cdr))
                            {
                                allCDRs.Add(cdr);
                            }
                        }
                    }
                }

                return allCDRs;

            }

        }

        #endregion

        #region GetCDRs  (CountryCode = null, PartyId = null)

        public IEnumerable<CDR> GetCDRs(CountryCode?  CountryCode   = null,
                                        Party_Id?     PartyId       = null)
        {

            lock (ChargeDetailRecords)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (ChargeDetailRecords.TryGetValue(CountryCode.Value, out var parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out var partyCDRs))
                        {
                            return partyCDRs.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allCDRs = new List<CDR>();

                    foreach (var party in ChargeDetailRecords.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out var partyCDRs))
                        {
                            allCDRs.AddRange(partyCDRs.Values);
                        }
                    }

                    return allCDRs;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (ChargeDetailRecords.TryGetValue(CountryCode.Value, out var parties))
                    {

                        var allCDRs = new List<CDR>();

                        foreach (var partyCDRs in parties.Values)
                        {
                            allCDRs.AddRange(partyCDRs.Values);
                        }

                        return allCDRs;

                    }
                }

                else
                {

                    var allCDRs = new List<CDR>();

                    foreach (var party in ChargeDetailRecords.Values)
                    {
                        foreach (var partyCDRs in party.Values)
                        {
                            allCDRs.AddRange(partyCDRs.Values);
                        }
                    }

                    return allCDRs;

                }

                return Array.Empty<CDR>();

            }

        }

        #endregion


        #region RemoveCDR(CDR)

        /// <summary>
        /// Remove the given charge detail record.
        /// </summary>
        /// <param name="CDR">A charge detail record.</param>
        public Boolean RemoveCDR(CDR                CDR,
                                 EventTracking_Id?  EventTrackingId   = null,
                                 User_Id?           CurrentUserId     = null)
        {

            lock (ChargeDetailRecords)
            {

                var success = false;

                if (ChargeDetailRecords.TryGetValue(CDR.CountryCode, out var parties))
                {

                    if (parties.TryGetValue(CDR.PartyId, out var partyCDRs))
                    {

                        if (partyCDRs.ContainsKey(CDR.Id))
                        {
                            success = partyCDRs.Remove(CDR.Id);
                        }

                        if (!partyCDRs.Any())
                            parties.Remove(CDR.PartyId);

                    }

                    if (!parties.Any())
                        ChargeDetailRecords.Remove(CDR.CountryCode);

                }

                return success;

            }

        }

        #endregion

        #region RemoveCDR  (CDRId)

        public Boolean RemoveCDR(CDR_Id             CDRId,
                                 EventTracking_Id?  EventTrackingId   = null,
                                 User_Id?           CurrentUserId     = null)
        {

            lock (ChargeDetailRecords)
            {

                CountryCode? countryCode   = default;
                Party_Id?    partyId       = default;

                foreach (var parties in ChargeDetailRecords.Values)
                {
                    foreach (var cdrs in parties.Values)
                    {
                        if (cdrs.TryGetValue(CDRId, out var cdr))
                        {
                            countryCode  = cdr.CountryCode;
                            partyId      = cdr.PartyId;
                        }
                    }
                }

                if (countryCode.HasValue &&
                    partyId.    HasValue)
                {
                    return ChargeDetailRecords[countryCode.Value][partyId.Value].Remove(CDRId);
                }

                return false;

            }

        }

        #endregion

        #region RemoveAllCDRs(IncludeCDRs = null)

        /// <summary>
        /// Remove all matching charge detail records.
        /// </summary>
        /// <param name="IncludeCDRs">An optional charge detail record filter.</param>
        public void RemoveAllCDRs(Func<CDR, Boolean>?  IncludeCDRs       = null,
                                  EventTracking_Id?    EventTrackingId   = null,
                                  User_Id?             CurrentUserId     = null)
        {

            lock (ChargeDetailRecords)
            {

                if (IncludeCDRs is null)
                    ChargeDetailRecords.Clear();

                else
                {

                    var cdrsToDelete = ChargeDetailRecords.Values.SelectMany(xx => xx.Values).
                                                                  SelectMany(yy => yy.Values).
                                                                  Where     (IncludeCDRs).
                                                                  ToArray   ();

                    foreach (var cdr in cdrsToDelete)
                        ChargeDetailRecords[cdr.CountryCode][cdr.PartyId].Remove(cdr.Id);

                }

            }

        }

        #endregion

        #region RemoveAllCDRs(CountryCode, PartyId)

        /// <summary>
        /// Remove all charge detail records owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllCDRs(CountryCode        CountryCode,
                                  Party_Id           PartyId,
                                  EventTracking_Id?  EventTrackingId   = null,
                                  User_Id?           CurrentUserId     = null)
        {

            lock (ChargeDetailRecords)
            {

                if (ChargeDetailRecords.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var partyCDRs))
                    {
                        partyCDRs.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion


    }

}
