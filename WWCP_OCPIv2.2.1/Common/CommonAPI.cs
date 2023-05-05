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

using System.Text;
using System.Net.Security;
using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;

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
                                                                   EMP_Id?        EMPId         = null);


    /// <summary>
    /// The Common API.
    /// </summary>
    public class CommonAPI : HTTPAPI
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

        private readonly URL OurBaseURL;


        /// <summary>
        /// The command values store.
        /// </summary>
        public readonly ConcurrentDictionary<Command_Id, CommandValues> CommandValueStore = new ConcurrentDictionary<Command_Id, CommandValues>();

        #endregion

        #region Properties

        /// <summary>
        /// The URL of our VERSIONS endpoint.
        /// </summary>
        public URL                           OurVersionsURL             { get; }

        /// <summary>
        /// All our credential roles.
        /// </summary>
        public IEnumerable<CredentialsRole>  OurCredentialRoles         { get; }


        public HTTPPath?                     AdditionalURLPathPrefix    { get; }

        /// <summary>
        /// Whether to keep or delete EVSEs marked as "REMOVED".
        /// </summary>
        public Func<EVSE, Boolean>           KeepRemovedEVSEs           { get; }

        /// <summary>
        /// Allow anonymous access to locations as Open Data.
        /// </summary>
        public Boolean                       LocationsAsOpenData        { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?                      AllowDowngrades            { get; }

        public Boolean                       Disable_RootServices       { get; }


        /// <summary>
        /// Disable OCPI v2.1.1.
        /// </summary>
        public Boolean                       Disable_OCPIv2_1_1         { get; }

        #endregion

        #region Events

        #region (protected internal) PostCredentialsRequest   (Request)

        /// <summary>
        /// An event sent whenever a post credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a post credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostCredentialsRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnPostCredentialsRequest?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request);

        #endregion

        #region (protected internal) PostCredentialsResponse  (Response)

        /// <summary>
        /// An event sent whenever a post credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a post credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostCredentialsResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        OCPIResponse  Response)

            => OnPostCredentialsResponse?.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request,
                                                  Response);

        #endregion


        #region (protected internal) PutCredentialsRequest    (Request)

        /// <summary>
        /// An event sent whenever a put credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a put credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutCredentialsRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnPutCredentialsRequest?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region (protected internal) PutCredentialsResponse   (Response)

        /// <summary>
        /// An event sent whenever a put credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a put credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutCredentialsResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnPutCredentialsResponse?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion


        #region (protected internal) DeleteCredentialsRequest (Request)

        /// <summary>
        /// An event sent whenever a delete credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a delete credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCredentialsRequest(DateTime     Timestamp,
                                                         HTTPAPI      API,
                                                         OCPIRequest  Request)

            => OnDeleteCredentialsRequest?.WhenAll(Timestamp,
                                                   API ?? this,
                                                   Request);

        #endregion

        #region (protected internal) DeleteCredentialsResponse(Response)

        /// <summary>
        /// An event sent whenever a delete credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a delete credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCredentialsResponse(DateTime      Timestamp,
                                                          HTTPAPI       API,
                                                          OCPIRequest   Request,
                                                          OCPIResponse  Response)

            => OnDeleteCredentialsResponse?.WhenAll(Timestamp,
                                                    API ?? this,
                                                    Request,
                                                    Response);

        #endregion

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
        public CommonAPI(URL                                    OurVersionsURL,
                         IEnumerable<CredentialsRole>           OurCredentialRoles,

                         HTTPPath?                              AdditionalURLPathPrefix            = null,
                         Func<EVSE, Boolean>?                   KeepRemovedEVSEs                   = null,
                         Boolean                                LocationsAsOpenData                = true,
                         Boolean?                               AllowDowngrades                    = null,
                         Boolean                                Disable_RootServices               = true,
                         Boolean                                Disable_OCPIv2_1_1                 = true,

                         HTTPHostname?                          HTTPHostname                       = null,
                         String?                                ExternalDNSName                    = null,
                         IPPort?                                HTTPServerPort                     = null,
                         HTTPPath?                              BasePath                           = null,
                         String?                                HTTPServerName                     = DefaultHTTPServerName,

                         HTTPPath?                              URLPathPrefix                      = null,
                         String?                                HTTPServiceName                    = DefaultHTTPServiceName,
                         //String?                                HTMLTemplate                       = null,
                         JObject?                               APIVersionHashes                   = null,

                         ServerCertificateSelectorDelegate?     ServerCertificateSelector          = null,
                         RemoteCertificateValidationCallback?   ClientCertificateValidator         = null,
                         LocalCertificateSelectionCallback?     ClientCertificateSelector          = null,
                         SslProtocols?                          AllowedTLSProtocols                = null,
                         Boolean?                               ClientCertificateRequired          = null,
                         Boolean?                               CheckCertificateRevocation         = null,

                         String?                                ServerThreadName                   = null,
                         ThreadPriority?                        ServerThreadPriority               = null,
                         Boolean?                               ServerThreadIsBackground           = null,

                         ConnectionIdBuilder?                   ConnectionIdBuilder                = null,
                         TimeSpan?                              ConnectionTimeout                  = null,
                         UInt32?                                MaxClientConnections               = null,

                         Boolean?                               DisableMaintenanceTasks            = null,
                         TimeSpan?                              MaintenanceInitialDelay            = null,
                         TimeSpan?                              MaintenanceEvery                   = null,

                         Boolean?                               DisableWardenTasks                 = null,
                         TimeSpan?                              WardenInitialDelay                 = null,
                         TimeSpan?                              WardenCheckEvery                   = null,

                         Boolean?                               IsDevelopment                      = null,
                         IEnumerable<String>?                   DevelopmentServers                 = null,
                         Boolean?                               DisableLogging                     = null,
                         String?                                LoggingPath                        = null,
                         String?                                LogfileName                        = null,
                         LogfileCreatorDelegate?                LogfileCreator                     = null,
                         DNSClient?                             DNSClient                          = null,
                         Boolean                                Autostart                          = false)


            : base(HTTPHostname,
                   ExternalDNSName,
                   HTTPServerPort  ?? DefaultHTTPServerPort,
                   BasePath,
                   HTTPServerName  ?? DefaultHTTPServerName,

                   URLPathPrefix   ?? DefaultURLPathPrefix,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   null, // HTMLTemplate,
                   APIVersionHashes,

                   ServerCertificateSelector,
                   ClientCertificateValidator,
                   ClientCertificateSelector,
                   AllowedTLSProtocols,
                   ClientCertificateRequired,
                   CheckCertificateRevocation,

                   ServerThreadName,
                   ServerThreadPriority,
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
                   LoggingPath,
                   LogfileName,
                   LogfileCreator,
                   DNSClient,
                   Autostart)

        {

            if (!OurCredentialRoles.SafeAny())
                throw new ArgumentNullException(nameof(OurCredentialRoles), "The given credential roles must not be null or empty!");

            this.OurVersionsURL           = OurVersionsURL;
            this.OurBaseURL               = URL.Parse(OurVersionsURL.ToString().Replace("/versions", ""));
            this.OurCredentialRoles       = OurCredentialRoles.Distinct();
            this.AdditionalURLPathPrefix  = AdditionalURLPathPrefix;
            this.KeepRemovedEVSEs         = KeepRemovedEVSEs ?? (evse => true);
            this.LocationsAsOpenData      = LocationsAsOpenData;
            this.AllowDowngrades          = AllowDowngrades;

            this.Disable_OCPIv2_1_1       = Disable_OCPIv2_1_1;

            this.remoteParties            = new Dictionary<RemoteParty_Id, RemoteParty>();
            this.Locations                = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id, Location>>>();
            this.tariffs                  = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Tariff_Id,   Tariff>>>();
            this.chargingSessions         = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Session_Id,  Session>>>();
            this.tokenStatus              = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Token_Id,    TokenStatus>>>();
            this.ChargeDetailRecords      = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<CDR_Id,      CDR>>>();

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
        public CommonAPI(URL                           OurVersionsURL,
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
                         String?                       LoggingPath               = null,
                         String?                       LogfileName               = null,
                         LogfileCreatorDelegate?       LogfileCreator            = null,
                         Boolean                       Autostart                 = false)

            : base(HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServiceName,
                   BasePath,

                   URLPathPrefix,
                   null, // HTMLTemplate,
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
                   LoggingPath,
                   LogfileName,
                   LogfileCreator,
                   Autostart)

        {

            if (!OurCredentialRoles.SafeAny())
                throw new ArgumentNullException(nameof(OurCredentialRoles), "The given credential roles must not be null or empty!");

            this.OurVersionsURL           = OurVersionsURL;
            this.OurBaseURL               = URL.Parse(OurVersionsURL.ToString().Replace("/versions", ""));
            this.OurCredentialRoles       = OurCredentialRoles?.Distinct() ?? Array.Empty<CredentialsRole>();
            this.AdditionalURLPathPrefix  = AdditionalURLPathPrefix;
            this.KeepRemovedEVSEs         = KeepRemovedEVSEs ?? (evse => true);
            this.LocationsAsOpenData      = LocationsAsOpenData;
            this.AllowDowngrades          = AllowDowngrades;
            this.Disable_RootServices     = Disable_RootServices;

            this.Disable_OCPIv2_1_1       = Disable_OCPIv2_1_1;

            this.remoteParties            = new Dictionary<RemoteParty_Id, RemoteParty>();
            this.Locations                = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id, Location>>>();
            this.tariffs                  = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Tariff_Id,   Tariff>>>();
            this.chargingSessions         = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Session_Id,  Session>>>();
            this.tokenStatus              = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Token_Id,    TokenStatus>>>();
            this.ChargeDetailRecords      = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<CDR_Id,      CDR>>>();

            // Link HTTP events...
            HTTPServer.RequestLog        += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog       += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog          += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            RegisterURLTemplates();

        }

        #endregion

        #endregion


        #region GetModuleURL(ModuleId, Stuff = "")

        public URL GetModuleURL(Module_Id  ModuleId,
                                String     Stuff   = "")
        {

            return OurBaseURL + Stuff + ModuleId.ToString();

            //switch (ModuleId)
            //{

            //    case Module_Id.CDRs:
            //        return OurBaseURL + Stuff + "cdrs";

            //    case Module_Id.ChargingProfiles:
            //        return OurBaseURL + Stuff + "chargingprofiles";

            //    case Module_Id.Commands:
            //        return OurBaseURL + Stuff + "commands";

            //    case Module_Id.Credentials:
            //        return OurBaseURL + Stuff + "credentials";

            //    case Module_Id.HubClientInfo:
            //        return OurBaseURL + Stuff + "hubclientinfo";

            //    case Module_Id.Locations:
            //        return OurBaseURL + Stuff + "locations";

            //    case Module_Id.Sessions:
            //        return OurBaseURL + Stuff + "sessions";

            //    case Module_Id.Tariffs:
            //        return OurBaseURL + Stuff + "tariffs";

            //    case Module_Id.Tokens:
            //        return OurBaseURL + Stuff + "tokens";

            //    default:
            //        return OurVersionsURL;

            //}

        }

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
                                                     AccessControlAllowHeaders  = "Authorization",
                                                     Connection                 = "close"
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
            //                             HTTPContentType.HTML_UTF8,
            //                             HTTPDelegate: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2_1.HTTPAPI.CommonAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2_1.HTTPAPI.CommonAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

            //                                 return new HTTPResponse.Builder(Request) {
            //                                     HTTPStatusCode  = HTTPStatusCode.OK,
            //                                     ContentType     = HTTPContentType.HTML_UTF8,
            //                                     Content         = _MemoryStream.ToArray(),
            //                                     Connection      = "close"
            //                                 };

            //                             });

            #region Text

            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix,
                                         HTTPContentType.TEXT_UTF8,
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                     AccessControlAllowHeaders  = "Authorization",
                                                     ContentType                = HTTPContentType.TEXT_UTF8,
                                                     Content                    = "This is an Open Charge Point Interface HTTP service!\r\nPlease check ~/versions!".ToUTF8Bytes(),
                                                     Connection                 = "close"
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
                                               AccessControlAllowHeaders  = "Authorization",
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
                               HTTPContentType.JSON_UTF8,
                               OCPIRequestHandler: Request => {

                                   #region Check access token

                                   if (Request.LocalAccessInfo.HasValue &&
                                       Request.LocalAccessInfo.Value.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Invalid or blocked access token!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                  AccessControlAllowHeaders  = "Authorization"
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
                                               AccessControlAllowHeaders  = "Authorization",
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
                                               AccessControlAllowHeaders  = "Authorization",
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
                               HTTPContentType.JSON_UTF8,
                               OCPIRequestHandler: Request => {

                                   #region Check access token

                                   if (Request.LocalAccessInfo.HasValue &&
                                       Request.LocalAccessInfo.Value.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Invalid or blocked access token!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                  AccessControlAllowHeaders  = "Authorization"
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
                                                  AccessControlAllowHeaders  = "Authorization"
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
                                                  AccessControlAllowHeaders  = "Authorization"
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
                                                  AccessControlAllowHeaders  = "Authorization"
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

                                   if (Request.RemoteParty?.Role == Roles.CPO)
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

                                   if (Request.RemoteParty?.Role == Roles.EMSP)
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
                                                   AccessControlAllowHeaders  = "Authorization",
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

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                               Allow                      = new List<HTTPMethod> {
                                                                                HTTPMethod.OPTIONS,
                                                                                HTTPMethod.GET,
                                                                                HTTPMethod.POST,
                                                                                HTTPMethod.PUT,
                                                                                HTTPMethod.DELETE
                                                                            },
                                               AccessControlAllowHeaders  = "Authorization"
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
                               HTTPContentType.JSON_UTF8,
                               OCPIRequestHandler: Request => {

                                   #region Check access token

                                   if (Request.LocalAccessInfo.HasValue &&
                                       Request.LocalAccessInfo.Value.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Invalid or blocked access token!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                  AccessControlAllowHeaders  = "Authorization"
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
                                               AccessControlAllowHeaders  = "Authorization"
                                           }
                                       });


                                   //return Task.FromResult(
                                   //    new OCPIResponse.Builder(Request) {
                                   //        StatusCode           = 2000,
                                   //        StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                   //        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                   //            HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                   //            AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                   //            AccessControlAllowHeaders  = "Authorization"
                                   //        }
                                   //    });

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
                               HTTPContentType.JSON_UTF8,
                               OCPIRequestLogger:   PostCredentialsRequest,
                               OCPIResponseLogger:  PostCredentialsResponse,
                               OCPIRequestHandler:   async Request => {

                                   var CREDENTIALS_TOKEN_A = Request.AccessToken;

                                   if (Request.RemoteParty is not null &&
                                       Request.LocalAccessInfo.HasValue     &&
                                       Request.LocalAccessInfo.Value.Status == AccessStatus.ALLOWED)
                                   {

                                       if (Request.LocalAccessInfo.Value.VersionsURL.HasValue)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,
                                                      StatusMessage        = "The given access token '" + CREDENTIALS_TOKEN_A.Value.ToString() + "' is already registered!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                          AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                          AccessControlAllowHeaders  = "Authorization"
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
                                                  AccessControlAllowHeaders  = "Authorization"
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
                               HTTPContentType.JSON_UTF8,
                               OCPIRequestLogger:  PutCredentialsRequest,
                               OCPIResponseLogger: PutCredentialsResponse,
                               OCPIRequestHandler:        async Request => {

                                   if (Request.LocalAccessInfo.HasValue &&
                                       Request.LocalAccessInfo.Value.Status == AccessStatus.ALLOWED)
                                   {

                                       if (!Request.LocalAccessInfo.Value.VersionsURL.HasValue)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,
                                                      StatusMessage        = "The given access token '" + (Request.AccessToken?.ToString() ?? "") + "' is not yet registered!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                          AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                          AccessControlAllowHeaders  = "Authorization"
                                                      }
                                                  };

                                       return await POSTOrPUTCredentials(Request);


                                   }

                                   return new OCPIResponse.Builder(Request) {
                                                  StatusCode           = 2000,
                                                  StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                                  HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                      HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                      AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                      AccessControlAllowHeaders  = "Authorization"
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
                               HTTPContentType.JSON_UTF8,
                               OCPIRequestLogger:  DeleteCredentialsRequest,
                               OCPIResponseLogger: DeleteCredentialsResponse,
                               OCPIRequestHandler:        async Request => {

                                   if (Request.LocalAccessInfo.HasValue &&
                                       Request.LocalAccessInfo.Value.Status == AccessStatus.ALLOWED)
                                   {

                                       #region Validations

                                       if (!Request.LocalAccessInfo.Value.VersionsURL.HasValue)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,
                                                      StatusMessage        = "The given access token '" + Request.AccessToken.Value.ToString() + "' is not registered!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                          AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                          AccessControlAllowHeaders  = "Authorization"
                                                      }
                                                  };

                                       #endregion


                                       //ToDo: await...
                                       RemoveAccessToken(Request.AccessToken.Value);


                                       return new OCPIResponse.Builder(Request) {
                                                  StatusCode           = 1000,
                                                  StatusMessage        = "The given access token was deleted!",
                                                  HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                      HTTPStatusCode             = HTTPStatusCode.OK,
                                                      AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                      AccessControlAllowHeaders  = "Authorization"
                                                  }
                                              };

                                   }

                                   return new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                  AccessControlAllowHeaders  = "Authorization"
                                              }
                                          };

                               });

            #endregion

        }

        #endregion


        #region (private) POSTOrPUTCredentials(Request)

        private async Task<OCPIResponse.Builder> POSTOrPUTCredentials(OCPIRequest Request)
        {

            var CREDENTIALS_TOKEN_A     = Request.AccessToken;

            #region Parse JSON

            var ErrorResponse = String.Empty;

            if (!Request.TryParseJObjectRequestBody(out JObject JSON, out OCPIResponse.Builder ResponseBuilder, AllowEmptyHTTPBody: false) ||
                !Credentials.TryParse(JSON, out Credentials receivedCredentials, out ErrorResponse))
            {

                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = "Could not parse the credentials JSON object! " + ErrorResponse,
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = "Authorization"
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
            //                           AccessControlAllowHeaders  = "Authorization"
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
            //                           AccessControlAllowHeaders  = "Authorization"
            //                       }
            //                   };

            //        }

            //    }
            //}

            #endregion


            var commonClient            = new CommonClient(//receivedCredentials.Roles.First().CountryCode,
                                                           //receivedCredentials.Roles.First().PartyId,
                                                           //receivedCredentials.Roles.First().Role,
                                                           receivedCredentials.URL,
                                                           receivedCredentials.Token,  // CREDENTIALS_TOKEN_B
                                                           this,
                                                           DNSClient: HTTPServer.DNSClient);

            var otherVersions           = await commonClient.GetVersions();

            #region ...or send error!

            if (otherVersions.StatusCode != 1000)
            {

                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = "Could not fetch VERSIONS information from '" + receivedCredentials.URL + "'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = "Authorization"
                           }
                       };

            }

            #endregion

            var version2_2              = Version_Id.Parse("2.2");
            var justVersion2_2          = otherVersions.Data.Where(version => version.Id == version2_2).ToArray();

            #region ...or send error!

            if (justVersion2_2.Length == 0)
            {

                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 3003,
                           StatusMessage        = "Could not find OCPI v2.2 at '" + receivedCredentials.URL + "'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = "Authorization"
                           }
                       };

            }

            #endregion

            var otherVersion2_2Details  = await commonClient.GetVersionDetails(version2_2);

            #region ...or send error!

            if (otherVersion2_2Details.StatusCode != 1000)
            {

                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 3001,
                           StatusMessage        = "Could not fetch v2.2 information from '" + justVersion2_2.First().URL + "'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = "Authorization"
                           }
                       };

            }

            #endregion



            var CREDENTIALS_TOKEN_C = AccessToken.NewRandom();

            // Store credential of the other side!
            foreach (var role in receivedCredentials.Roles)
                AddOrUpdateRemoteParty(role.CountryCode,
                                       role.PartyId,
                                       role.Role,
                                       role.BusinessDetails,

                                       CREDENTIALS_TOKEN_C, // -------------------------------------------------- !!!

                                       receivedCredentials.Token,
                                       receivedCredentials.URL,
                                       otherVersions.Data.Select(version => version.Id),
                                       version2_2,

                                       null,
                                       AccessStatus.ALLOWED,
                                       RemoteAccessStatus.ONLINE,
                                       PartyStatus.ENABLED);

            //SetIncomingAccessToken(CREDENTIALS_TOKEN_C,
            //                       receivedCredentials.URL,
            //                       receivedCredentials.Roles,
            //                       AccessStatus.ALLOWED);


            RemoveAccessToken(CREDENTIALS_TOKEN_A.Value);



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
                               AccessControlAllowHeaders  = "Authorization"
                           }
                       };

        }

        #endregion


        //ToDo: Wrap the following into a plugable interface!

        #region AccessTokens

        // An access token might be used by more than one CountryCode + PartyId + Role combination!

        #region RemoveAccessToken(AccessToken)

        public CommonAPI RemoveAccessToken(AccessToken AccessToken)
        {
            lock (remoteParties)
            {

                var _remoteParties = remoteParties.Values.Where(party => party.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken == AccessToken)).ToArray();

                foreach (var remoteParty in _remoteParties)
                {

                    if (remoteParty.LocalAccessInfos.Count() <= 1)
                    {
                        this.remoteParties.Remove((RemoteParty_Id)remoteParty.Id);
                        File.AppendAllText(LogfileName,
                                           new JObject(new JProperty("removeRemoteParty", remoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                           Encoding.UTF8);
                    }

                    else
                    {

                        this.remoteParties.Remove((RemoteParty_Id)remoteParty.Id);

                        var newRemoteParty = new RemoteParty(
                                                 remoteParty.CountryCode,
                                                 remoteParty.PartyId,
                                                 remoteParty.Role,
                                                 remoteParty.BusinessDetails,
                                                 remoteParty.LocalAccessInfos.Where(accessInfo => accessInfo.AccessToken != AccessToken),
                                                 remoteParty.RemoteAccessInfos,
                                                 remoteParty.Status
                                             );

                        this.remoteParties.Add(newRemoteParty.Id, newRemoteParty);

                        File.AppendAllText(LogfileName,
                                           new JObject(new JProperty("updateRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                           Encoding.UTF8);

                    }

                }

                return this;

            }
        }

        #endregion

        #region TryGetLocalAccessInfo(AccessToken, out LocalAccessInfo)

        public Boolean TryGetLocalAccessInfo(AccessToken AccessToken, out LocalAccessInfo LocalAccessInfo)
        {
            lock (remoteParties)
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
        }

        #endregion

        #region GetLocalAccessInfos(AccessToken)

        public IEnumerable<LocalAccessInfo> GetLocalAccessInfos(AccessToken AccessToken)
        {
            lock (remoteParties)
            {

                return remoteParties.Values.Where     (remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken == AccessToken)).
                                            SelectMany(remoteParty => remoteParty.LocalAccessInfos).
                                            ToArray();

            }
        }

        #endregion

        #region GetAccessInfos(AccessToken, AccessStatus)

        public IEnumerable<LocalAccessInfo> GetAccessInfos(AccessToken   AccessToken,
                                                           AccessStatus  AccessStatus)
        {
            lock (remoteParties)
            {

                return remoteParties.Values.Where     (remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken  == AccessToken &&
                                                                                                                     accessInfo.Status == AccessStatus)).
                                            SelectMany(remoteParty => remoteParty.LocalAccessInfos).
                                            ToArray();

            }
        }

        #endregion

        #endregion

        #region RemoteParties

        #region Data

        private readonly Dictionary<RemoteParty_Id, RemoteParty> remoteParties;

        public IEnumerable<RemoteParty> RemoteParties
            => remoteParties.Values;

        #endregion

        #region (private) GetRemotePartySerializator(Request, User)

        //private RemotePartyToJSONDelegate GetRemotePartySerializator(HTTPRequest  Request,
        //                                                             User         User)
        //{

        //    return (RemoteParty,
        //            Embedded,
        //            CustomRemotePartySerializer,
        //            CustomBusinessDetailsSerializer,
        //            IncludeCryptoHash)

        //            => RemoteParty.ToJSON(Embedded,
        //                                  CustomRemotePartySerializer,
        //                                  CustomBusinessDetailsSerializer,
        //                                  IncludeCryptoHash);

        //}

        #endregion


        #region AddRemoteParty(...)

        public Boolean AddRemoteParty(CountryCode                           CountryCode,
                                      Party_Id                              PartyId,
                                      Roles                                 Role,
                                      BusinessDetails                       BusinessDetails,

                                      AccessToken                           AccessToken,

                                      AccessToken                           RemoteAccessToken,
                                      URL                                   RemoteVersionsURL,
                                      IEnumerable<Version_Id>?              RemoteVersionIds             = null,
                                      Version_Id?                           SelectedVersionId            = null,

                                      Boolean?                              AccessTokenBase64Encoding    = null,
                                      AccessStatus                          AccessStatus                 = AccessStatus.ALLOWED,
                                      RemoteAccessStatus?                   RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                      PartyStatus                           PartyStatus                  = PartyStatus.ENABLED,

                                      RemoteCertificateValidationCallback?  RemoteCertificateValidator   = null,
                                      LocalCertificateSelectionCallback?    ClientCertificateSelector    = null,
                                      X509Certificate?                      ClientCert                   = null,
                                      SslProtocols?                         TLSProtocol                  = null,
                                      Boolean?                              PreferIPv4                   = null,
                                      String?                               HTTPUserAgent                = null,
                                      TimeSpan?                             RequestTimeout               = null,
                                      TransmissionRetryDelayDelegate?       TransmissionRetryDelay       = null,
                                      UInt16?                               MaxNumberOfRetries           = null,
                                      Boolean?                              UseHTTPPipelining            = null)

        {
            lock (remoteParties)
            {

                //var remoteParties = _RemoteParties.Values.Where(party => party.CountryCode == CountryCode &&
                //                                                         party.PartyId     == PartyId     &&
                //                                                         party.Role        == Role).ToArray();

                //foreach (var remoteParty in remoteParties)
                //    _RemoteParties.Remove(remoteParty.Id);


                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role,
                                                     BusinessDetails,

                                                     AccessToken,

                                                     RemoteAccessToken,
                                                     RemoteVersionsURL,
                                                     RemoteVersionIds,
                                                     SelectedVersionId,

                                                     AccessTokenBase64Encoding,
                                                     AccessStatus,
                                                     RemoteStatus,
                                                     PartyStatus,

                                                     RemoteCertificateValidator,
                                                     ClientCertificateSelector,
                                                     ClientCert,
                                                     TLSProtocol,
                                                     PreferIPv4,
                                                     HTTPUserAgent,
                                                     RequestTimeout,
                                                     TransmissionRetryDelay,
                                                     MaxNumberOfRetries,
                                                     UseHTTPPipelining);

                remoteParties.Add(newRemoteParty.Id, newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }
        }

        #endregion

        #region AddRemoteParty(...)

        public Boolean AddRemoteParty(CountryCode                           CountryCode,
                                      Party_Id                              PartyId,
                                      Roles                                 Role,
                                      BusinessDetails                       BusinessDetails,

                                      AccessToken                           AccessToken,
                                      Boolean?                              AccessTokenBase64Encoding    = null,
                                      AccessStatus                          AccessStatus                 = AccessStatus.ALLOWED,

                                      PartyStatus                           PartyStatus                  = PartyStatus.ENABLED,

                                      RemoteCertificateValidationCallback?  RemoteCertificateValidator   = null,
                                      LocalCertificateSelectionCallback?    ClientCertificateSelector    = null,
                                      X509Certificate?                      ClientCert                   = null,
                                      SslProtocols?                         TLSProtocol                  = null,
                                      Boolean?                              PreferIPv4                   = null,
                                      String?                               HTTPUserAgent                = null,
                                      TimeSpan?                             RequestTimeout               = null,
                                      TransmissionRetryDelayDelegate?       TransmissionRetryDelay       = null,
                                      UInt16?                               MaxNumberOfRetries           = null,
                                      Boolean?                              UseHTTPPipelining            = null)
        {
            lock (remoteParties)
            {

                //var remoteParties = _RemoteParties.Values.Where(party => party.CountryCode == CountryCode &&
                //                                                         party.PartyId     == PartyId     &&
                //                                                         party.Role        == Role).ToArray();

                //foreach (var remoteParty in remoteParties)
                //    _RemoteParties.Remove(remoteParty.Id);


                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role,
                                                     BusinessDetails,

                                                     AccessToken,
                                                     AccessTokenBase64Encoding,
                                                     AccessStatus,

                                                     PartyStatus,

                                                     RemoteCertificateValidator,
                                                     ClientCertificateSelector,
                                                     ClientCert,
                                                     TLSProtocol,
                                                     PreferIPv4,
                                                     HTTPUserAgent,
                                                     RequestTimeout,
                                                     TransmissionRetryDelay,
                                                     MaxNumberOfRetries,
                                                     UseHTTPPipelining);

                remoteParties.Add(newRemoteParty.Id, newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }
        }

        #endregion

        #region AddRemoteParty(...)

        public Boolean AddRemoteParty(CountryCode                           CountryCode,
                                      Party_Id                              PartyId,
                                      Roles                                 Role,
                                      BusinessDetails                       BusinessDetails,

                                      AccessToken                           RemoteAccessToken,
                                      URL                                   RemoteVersionsURL,
                                      IEnumerable<Version_Id>?              RemoteVersionIds             = null,
                                      Version_Id?                           SelectedVersionId            = null,

                                      Boolean?                              AccessTokenBase64Encoding    = null,
                                      RemoteAccessStatus?                   RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                      PartyStatus                           PartyStatus                  = PartyStatus.ENABLED,

                                      RemoteCertificateValidationCallback?  RemoteCertificateValidator   = null,
                                      LocalCertificateSelectionCallback?    ClientCertificateSelector    = null,
                                      X509Certificate?                      ClientCert                   = null,
                                      SslProtocols?                         TLSProtocol                  = null,
                                      Boolean?                              PreferIPv4                   = null,
                                      String?                               HTTPUserAgent                = null,
                                      TimeSpan?                             RequestTimeout               = null,
                                      TransmissionRetryDelayDelegate?       TransmissionRetryDelay       = null,
                                      UInt16?                               MaxNumberOfRetries           = null,
                                      Boolean?                              UseHTTPPipelining            = null)
        {
            lock (remoteParties)
            {

                //var remoteParties = _RemoteParties.Values.Where(party => party.CountryCode == CountryCode &&
                //                                                         party.PartyId     == PartyId     &&
                //                                                         party.Role        == Role).ToArray();

                //foreach (var remoteParty in remoteParties)
                //    _RemoteParties.Remove(remoteParty.Id);


                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role,
                                                     BusinessDetails,

                                                     RemoteAccessToken,
                                                     RemoteVersionsURL,
                                                     RemoteVersionIds,
                                                     SelectedVersionId,

                                                     AccessTokenBase64Encoding,
                                                     RemoteStatus,
                                                     PartyStatus,

                                                     RemoteCertificateValidator,
                                                     ClientCertificateSelector,
                                                     ClientCert,
                                                     TLSProtocol,
                                                     PreferIPv4,
                                                     HTTPUserAgent,
                                                     RequestTimeout,
                                                     TransmissionRetryDelay,
                                                     MaxNumberOfRetries,
                                                     UseHTTPPipelining);

                remoteParties.Add(newRemoteParty.Id, newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }
        }

        #endregion

        #region AddRemoteParty(...)

        public Boolean AddRemoteParty(CountryCode                           CountryCode,
                                      Party_Id                              PartyId,
                                      Roles                                 Role,
                                      BusinessDetails                       BusinessDetails,

                                      IEnumerable<LocalAccessInfo>          LocalAccessInfos,
                                      IEnumerable<RemoteAccessInfo>         RemoteAccessInfos,

                                      PartyStatus                           Status                       = PartyStatus.ENABLED,

                                      RemoteCertificateValidationCallback?  RemoteCertificateValidator   = null,
                                      LocalCertificateSelectionCallback?    ClientCertificateSelector    = null,
                                      X509Certificate?                      ClientCert                   = null,
                                      SslProtocols?                         TLSProtocol                  = null,
                                      Boolean?                              PreferIPv4                   = null,
                                      String?                               HTTPUserAgent                = null,
                                      TimeSpan?                             RequestTimeout               = null,
                                      TransmissionRetryDelayDelegate?       TransmissionRetryDelay       = null,
                                      UInt16?                               MaxNumberOfRetries           = null,
                                      Boolean?                              UseHTTPPipelining            = null,

                                      DateTime?                             LastUpdated                  = null)
        {
            lock (remoteParties)
            {

                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role,
                                                     BusinessDetails,

                                                     LocalAccessInfos,
                                                     RemoteAccessInfos,

                                                     Status,

                                                     RemoteCertificateValidator,
                                                     ClientCertificateSelector,
                                                     ClientCert,
                                                     TLSProtocol,
                                                     PreferIPv4,
                                                     HTTPUserAgent,
                                                     RequestTimeout,
                                                     TransmissionRetryDelay,
                                                     MaxNumberOfRetries,
                                                     UseHTTPPipelining,

                                                     LastUpdated);

                remoteParties.Add(newRemoteParty.Id, newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }
        }

        #endregion


        #region AddOrUpdateRemoteParty(...)

        public Boolean AddOrUpdateRemoteParty(CountryCode                           CountryCode,
                                              Party_Id                              PartyId,
                                              Roles                                 Role,
                                              BusinessDetails                       BusinessDetails,

                                              AccessToken                           AccessToken,

                                              AccessToken                           RemoteAccessToken,
                                              URL                                   RemoteVersionsURL,
                                              IEnumerable<Version_Id>?              RemoteVersionIds             = null,
                                              Version_Id?                           SelectedVersionId            = null,

                                              Boolean?                              AccessTokenBase64Encoding    = null,
                                              AccessStatus                          AccessStatus                 = AccessStatus.ALLOWED,
                                              RemoteAccessStatus?                   RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                              PartyStatus                           PartyStatus                  = PartyStatus.ENABLED,

                                              RemoteCertificateValidationCallback?  RemoteCertificateValidator   = null,
                                              LocalCertificateSelectionCallback?    ClientCertificateSelector    = null,
                                              X509Certificate?                      ClientCert                   = null,
                                              SslProtocols?                         TLSProtocol                  = null,
                                              Boolean?                              PreferIPv4                   = null,
                                              String?                               HTTPUserAgent                = null,
                                              TimeSpan?                             RequestTimeout               = null,
                                              TransmissionRetryDelayDelegate?       TransmissionRetryDelay       = null,
                                              UInt16?                               MaxNumberOfRetries           = null,
                                              Boolean?                              UseHTTPPipelining            = null)
        {
            lock (remoteParties)
            {

                var _remoteParties = remoteParties.Values.Where(party => party.CountryCode == CountryCode &&
                                                                         party.PartyId     == PartyId     &&
                                                                         party.Role        == Role).ToArray();

                foreach (var remoteParty in _remoteParties)
                    this.remoteParties.Remove((RemoteParty_Id)remoteParty.Id);


                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role,
                                                     BusinessDetails,

                                                     AccessToken,

                                                     RemoteAccessToken,
                                                     RemoteVersionsURL,
                                                     RemoteVersionIds,
                                                     SelectedVersionId,

                                                     AccessTokenBase64Encoding,
                                                     AccessStatus,
                                                     RemoteStatus,
                                                     PartyStatus,

                                                     RemoteCertificateValidator,
                                                     ClientCertificateSelector,
                                                     ClientCert,
                                                     TLSProtocol,
                                                     PreferIPv4,
                                                     HTTPUserAgent,
                                                     RequestTimeout,
                                                     TransmissionRetryDelay,
                                                     MaxNumberOfRetries,
                                                     UseHTTPPipelining);

                this.remoteParties.Add(newRemoteParty.Id, newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addOrUpdateRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }
        }

        #endregion

        #region AddOrUpdateRemoteParty(...)

        public Boolean AddOrUpdateRemoteParty(CountryCode                           CountryCode,
                                              Party_Id                              PartyId,
                                              Roles                                 Role,
                                              BusinessDetails                       BusinessDetails,

                                              AccessToken                           AccessToken,
                                              Boolean?                              AccessTokenBase64Encoding    = null,
                                              AccessStatus                          AccessStatus                 = AccessStatus.ALLOWED,

                                              PartyStatus                           PartyStatus                  = PartyStatus.ENABLED,

                                              RemoteCertificateValidationCallback?  RemoteCertificateValidator   = null,
                                              LocalCertificateSelectionCallback?    ClientCertificateSelector    = null,
                                              X509Certificate?                      ClientCert                   = null,
                                              SslProtocols?                         TLSProtocol                  = null,
                                              Boolean?                              PreferIPv4                   = null,
                                              String?                               HTTPUserAgent                = null,
                                              TimeSpan?                             RequestTimeout               = null,
                                              TransmissionRetryDelayDelegate?       TransmissionRetryDelay       = null,
                                              UInt16?                               MaxNumberOfRetries           = null,
                                              Boolean?                              UseHTTPPipelining            = null)
        {
            lock (remoteParties)
            {

                var _remoteParties = remoteParties.Values.Where(party => party.CountryCode == CountryCode &&
                                                                         party.PartyId     == PartyId     &&
                                                                         party.Role        == Role).ToArray();

                foreach (var remoteParty in _remoteParties)
                    this.remoteParties.Remove((RemoteParty_Id)remoteParty.Id);


                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role,
                                                     BusinessDetails,

                                                     AccessToken,
                                                     AccessTokenBase64Encoding,
                                                     AccessStatus,

                                                     PartyStatus,

                                                     RemoteCertificateValidator,
                                                     ClientCertificateSelector,
                                                     ClientCert,
                                                     TLSProtocol,
                                                     PreferIPv4,
                                                     HTTPUserAgent,
                                                     RequestTimeout,
                                                     TransmissionRetryDelay,
                                                     MaxNumberOfRetries,
                                                     UseHTTPPipelining);

                this.remoteParties.Add(newRemoteParty.Id, newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addOrUpdateRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }
        }

        #endregion

        #region AddOrUpdateRemoteParty(...)

        public Boolean AddOrUpdateRemoteParty(CountryCode                           CountryCode,
                                              Party_Id                              PartyId,
                                              Roles                                 Role,
                                              BusinessDetails                       BusinessDetails,

                                              AccessToken                           RemoteAccessToken,
                                              URL                                   RemoteVersionsURL,
                                              IEnumerable<Version_Id>?              RemoteVersionIds             = null,
                                              Version_Id?                           SelectedVersionId            = null,

                                              Boolean?                              AccessTokenBase64Encoding    = null,
                                              RemoteAccessStatus?                   RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                              PartyStatus                           PartyStatus                  = PartyStatus.ENABLED,

                                              RemoteCertificateValidationCallback?  RemoteCertificateValidator   = null,
                                              LocalCertificateSelectionCallback?    ClientCertificateSelector    = null,
                                              X509Certificate?                      ClientCert                   = null,
                                              SslProtocols?                         TLSProtocol                  = null,
                                              Boolean?                              PreferIPv4                   = null,
                                              String?                               HTTPUserAgent                = null,
                                              TimeSpan?                             RequestTimeout               = null,
                                              TransmissionRetryDelayDelegate?       TransmissionRetryDelay       = null,
                                              UInt16?                               MaxNumberOfRetries           = null,
                                              Boolean?                              UseHTTPPipelining            = null)

        {
            lock (remoteParties)
            {

                var _remoteParties = remoteParties.Values.Where(party => party.CountryCode == CountryCode &&
                                                                         party.PartyId     == PartyId     &&
                                                                         party.Role        == Role).ToArray();

                foreach (var remoteParty in _remoteParties)
                    this.remoteParties.Remove((RemoteParty_Id)remoteParty.Id);


                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role,
                                                     BusinessDetails,

                                                     RemoteAccessToken,
                                                     RemoteVersionsURL,
                                                     RemoteVersionIds,
                                                     SelectedVersionId,

                                                     AccessTokenBase64Encoding,
                                                     RemoteStatus,
                                                     PartyStatus,

                                                     RemoteCertificateValidator,
                                                     ClientCertificateSelector,
                                                     ClientCert,
                                                     TLSProtocol,
                                                     PreferIPv4,
                                                     HTTPUserAgent,
                                                     RequestTimeout,
                                                     TransmissionRetryDelay,
                                                     MaxNumberOfRetries,
                                                     UseHTTPPipelining);

                this.remoteParties.Add(newRemoteParty.Id, newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addOrUpdateRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }
        }

        #endregion


        #region ContainsRemoteParty(RemotePartyId)

        /// <summary>
        /// Whether this API contains a remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        public Boolean ContainsRemoteParty(RemoteParty_Id RemotePartyId)
        {

            try
            {

                //RemotePartiesSemaphore.Wait();

                return remoteParties.ContainsKey(RemotePartyId);

            }
            finally
            {
                //RemotePartiesSemaphore.Release();
            }

        }

        #endregion

        #region GetRemoteParty     (RemotePartyId)

        /// <summary>
        /// Get the remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        public RemoteParty? GetRemoteParty(RemoteParty_Id RemotePartyId)
        {

            try
            {

                //await RemotePartiesSemaphore.WaitAsync();

                if (remoteParties.TryGetValue(RemotePartyId, out var remoteParty))
                    return remoteParty;

                return null;

            }
            finally
            {
                //RemotePartiesSemaphore.Release();
            }

        }

        #endregion

        #region TryGetRemoteParty  (RemotePartyId, out RemoteParty)

        /// <summary>
        /// Try to get the remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        /// <param name="RemoteParty">The defibrillator.</param>
        public Boolean TryGetRemoteParty(RemoteParty_Id    RemotePartyId,
                                         out RemoteParty?  RemoteParty)
        {

            try
            {

                //RemotePartiesSemaphore.Wait();

                if (remoteParties.TryGetValue(RemotePartyId, out RemoteParty))
                    return true;

                RemoteParty = null;
                return false;

            }
            finally
            {
                //RemotePartiesSemaphore.Release();
            }

        }

        #endregion

        #region GetRemoteParties   (IncludeFilter = null)

        /// <summary>
        /// Get all remote parties machting the given optional filter.
        /// </summary>
        /// <param name="IncludeFilter">A delegate for filtering remote parties.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(IncludeRemoteParty? IncludeFilter = null)
        {

            try
            {

                //await RemotePartiesSemaphore.WaitAsync();

                return IncludeFilter is null
                           ? remoteParties.Values.ToArray()
                           : remoteParties.Values.Where(remoteParty => IncludeFilter(remoteParty)).ToArray();

            }
            finally
            {
                //RemotePartiesSemaphore.Release();
            }

        }

        #endregion

        #region GetRemoteParties   (Role)

        /// <summary>
        /// Get all remote parties having the given role.
        /// </summary>
        /// <param name="Role">The role of the remote parties.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(Roles Role)
        {

            try
            {

                //await RemotePartiesSemaphore.WaitAsync();

                return remoteParties.Values.Where(remoteParty => remoteParty.Role == Role).ToArray();

            }
            finally
            {
                //RemotePartiesSemaphore.Release();
            }

        }

        #endregion

        #region GetRemoteParties(AccessToken)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken AccessToken)
        {
            lock (remoteParties)
            {

                return remoteParties.Values.Where(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken == AccessToken)).
                                            ToArray();

            }
        }

        #endregion

        #region GetRemoteParties(AccessToken, AccessStatus)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken   AccessToken,
                                                         AccessStatus  AccessStatus)
        {
            lock (remoteParties)
            {

                return remoteParties.Values.Where(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken  == AccessToken &&
                                                                                                                     localAccessInfo.Status == AccessStatus)).
                                            ToArray();

            }
        }

        #endregion

        #region TryGetRemoteParties(AccessToken, out RemoteParties)

        public Boolean TryGetRemoteParties(AccessToken AccessToken, out IEnumerable<RemoteParty> RemoteParties)
        {
            lock (this.remoteParties)
            {

                RemoteParties = this.remoteParties.Values.Where(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken == AccessToken)).
                                                          ToArray();

                return RemoteParties.Any();

            }
        }

        #endregion


        #region RemoveRemoteParty(RemoteParty)

        public Boolean RemoveRemoteParty(RemoteParty RemoteParty)
        {

            lock (remoteParties)
            {

                if (remoteParties.Remove(RemoteParty.Id, out var remoteParty))
                {

                    File.AppendAllText(LogfileName,
                                       new JObject(new JProperty("removeRemoteParty", remoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                       Encoding.UTF8);

                    return true;

                }

                return false;

            }

        }

        #endregion

        #region RemoveRemoteParty(RemotePartyId)

        public Boolean RemoveRemoteParty(RemoteParty_Id RemotePartyId)
        {

            lock (remoteParties)
            {

                if (remoteParties.Remove(RemotePartyId, out var remoteParty))
                {

                    File.AppendAllText(LogfileName,
                                       new JObject(new JProperty("removeRemoteParty", remoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                       Encoding.UTF8);

                    return true;

                }

                return false;

            }

        }

        #endregion

        #region RemoveRemoteParty(CountryCode, PartyId)

        public Boolean RemoveRemoteParty(CountryCode  CountryCode,
                                         Party_Id     PartyId)
        {

            lock (remoteParties)
            {

                foreach (var remoteParty in remoteParties.Values.Where(remoteParty => remoteParty.CountryCode == CountryCode &&
                                                                                      remoteParty.PartyId     == PartyId))
                {

                    remoteParties.Remove(remoteParty.Id);

                    File.AppendAllText(LogfileName,
                                       new JObject(new JProperty("removeRemoteParty", remoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                       Encoding.UTF8);

                }

                return true;

            }

        }

        #endregion

        #region RemoveRemoteParty(CountryCode, PartyId, AccessToken)

        public Boolean RemoveRemoteParty(CountryCode  CountryCode,
                                         Party_Id     PartyId,
                                         AccessToken  AccessToken)
        {

            lock (remoteParties)
            {

                foreach (var remoteParty in remoteParties.Values.Where(remoteParty => remoteParty.CountryCode == CountryCode &&
                                                                                      remoteParty.PartyId     == PartyId     &&
                                                                                      remoteParty.RemoteAccessInfos.Any(remoteAccessInfo => remoteAccessInfo.AccessToken == AccessToken)))
                {

                    remoteParties.Remove(remoteParty.Id);

                    File.AppendAllText(LogfileName,
                                       new JObject(new JProperty("removeRemoteParty", remoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                       Encoding.UTF8);

                }

                return true;

            }

        }

        #endregion

        #region RemoveAllRemoteParties()

        public void RemoveAllRemoteParties()
        {
            lock (remoteParties)
            {
                remoteParties.Clear();
            }
        }

        #endregion

        #endregion

        #region CPOClients

        private readonly List<CPOClient> cpoClients = new();

        public IEnumerable<CPOClient> CPOClients
            => cpoClients;

        #region GetCPOClient (CountryCode, PartyId)

        /// <summary>
        /// As a CPO create a client to access a remote EMSP.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote EMSP.</param>
        /// <param name="PartyId">The party identification of the remote EMSP.</param>
        public CPOClient? GetCPOClient(CountryCode  CountryCode,
                                       Party_Id     PartyId)
        {

            var remoteParty = RemoteParties.FirstOrDefault(remoteparty => remoteparty.CountryCode == CountryCode &&
                                                                          remoteparty.PartyId     == PartyId);

            if (remoteParty?.RemoteAccessInfos?.Any() == true)
                return cpoClients.AddAndReturnElement(
                    new CPOClient(
                        remoteParty,
                        this
                    ));

            return null;

        }

        #endregion

        #region GetCPOClient(RemoteParty)

        public CPOClient? GetCPOClient(RemoteParty  RemoteParty)
        {

            if (RemoteParty is null)
                return null;

            var cpoClient = CPOClients.FirstOrDefault(cpoclient => cpoclient.RemoteVersionsURL == RemoteParty.RemoteAccessInfos.First().VersionsURL &&
                                                                   cpoclient.AccessToken       == RemoteParty.RemoteAccessInfos.First().AccessToken);

            if (cpoClient is not null)
                return cpoClient;

            if (RemoteParty.RemoteAccessInfos?.Any() == true)
                return cpoClients.AddAndReturnElement(
                    new CPOClient(
                        RemoteParty,
                        this
                    ));

            return null;

        }

        #endregion

        #region GetCPOClient(RemotePartyId)

        public CPOClient? GetCPOClient(RemoteParty_Id  RemotePartyId)
        {

            var remoteParty  = RemoteParties.FirstOrDefault(remoteparty => remoteparty.CountryCode == RemotePartyId.CountryCode &&
                                                                           remoteparty.PartyId     == RemotePartyId.PartyId);

            var cpoClient   = remoteParty is not null
                                   ? CPOClients.FirstOrDefault(cpoclient => cpoclient.RemoteVersionsURL == remoteParty.RemoteAccessInfos.First().VersionsURL &&
                                                                            cpoclient.AccessToken       == remoteParty.RemoteAccessInfos.First().AccessToken)
                                   : null;

            if (cpoClient is not null)
                return cpoClient;

            if (remoteParty?.RemoteAccessInfos?.Any() == true)
                return cpoClients.AddAndReturnElement(
                    new CPOClient(
                        remoteParty,
                        this
                    ));

            return null;

        }

        #endregion

        #endregion

        #region EMSPClients

        private readonly List<EMSPClient> emspClients = new();
        public IEnumerable<EMSPClient> EMSPClients
            => emspClients;

        #region GetEMSPClient(CountryCode, PartyId)

        /// <summary>
        /// As an EMSP create a client to access a remote CPO.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote CPO.</param>
        /// <param name="PartyId">The party identification of the remote CPO.</param>
        public EMSPClient? GetEMSPClient(CountryCode  CountryCode,
                                         Party_Id     PartyId)
        {

            var remoteParty = RemoteParties.FirstOrDefault(remoteparty => remoteparty.CountryCode == CountryCode &&
                                                                          remoteparty.PartyId     == PartyId);

            if (remoteParty?.RemoteAccessInfos?.Any() == true)
                return emspClients.AddAndReturnElement(
                    new EMSPClient(
                        remoteParty,
                        this
                    ));

            return null;

        }

        #endregion

        #region GetEMSPClient(RemoteParty)

        public EMSPClient? GetEMSPClient(RemoteParty  RemoteParty)
        {

            if (RemoteParty is null)
                return null;

            var emspClient = EMSPClients.FirstOrDefault(emspclient => emspclient.RemoteVersionsURL == RemoteParty.RemoteAccessInfos.First().VersionsURL &&
                                                                      emspclient.AccessToken       == RemoteParty.RemoteAccessInfos.First().AccessToken);

            if (emspClient is not null)
                return emspClient;

            if (RemoteParty.RemoteAccessInfos?.Any() == true)
                return emspClients.AddAndReturnElement(
                    new EMSPClient(
                        RemoteParty,
                        this
                    ));

            return null;

        }

        #endregion

        #region GetEMSPClient(RemotePartyId)

        public EMSPClient? GetEMSPClient(RemoteParty_Id  RemotePartyId)
        {

            var remoteParty  = RemoteParties.FirstOrDefault(remoteparty => remoteparty.CountryCode == RemotePartyId.CountryCode &&
                                                                           remoteparty.PartyId     == RemotePartyId.PartyId);

            var emspClient   = remoteParty is not null
                                   ? EMSPClients.FirstOrDefault(emspclient => emspclient.RemoteVersionsURL == remoteParty.RemoteAccessInfos.First().VersionsURL &&
                                                                              emspclient.AccessToken       == remoteParty.RemoteAccessInfos.First().AccessToken)
                                   : null;

            if (emspClient is not null)
                return emspClient;

            if (remoteParty?.RemoteAccessInfos?.Any() == true)
                return emspClients.AddAndReturnElement(
                    new EMSPClient(
                        remoteParty,
                        this
                    ));

            return null;

        }

        #endregion

        #endregion


        // Add last modified timestamp to locations!

        #region Locations

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id , Location>>> Locations;


        public delegate Task OnLocationAddedDelegate(Location Location);

        public event OnLocationAddedDelegate? OnLocationAdded;


        public delegate Task OnLocationChangedDelegate(Location Location);

        public event OnLocationChangedDelegate? OnLocationChanged;


        #region AddLocation           (Location, SkipNotifications = false)

        public Location AddLocation(Location  Location,
                                    Boolean   SkipNotifications   = false)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (Locations)
            {

                if (!Locations.TryGetValue(Location.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                    Locations.Add(Location.CountryCode, parties);
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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddLocation), " ", nameof(OnLocationAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }

                    return Location;

                }

                throw new ArgumentException("The given location already exists!");

            }

        }

        #endregion

        #region AddLocationIfNotExists(Location, SkipNotifications = false)

        public Location AddLocationIfNotExists(Location  Location,
                                               Boolean   SkipNotifications   = false)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (Locations)
            {

                if (!Locations.TryGetValue(Location.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                    Locations.Add(Location.CountryCode, parties);
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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddLocationIfNotExists), " ", nameof(OnLocationAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }

                }

                return Location;

            }

        }

        #endregion

        #region AddOrUpdateLocation   (newOrUpdatedLocation, AllowDowngrades = false)

        private AddOrUpdateResult<Location> __addOrUpdateLocation(Location  newOrUpdatedLocation,
                                                                  Boolean?  AllowDowngrades = false)
        {

            if (newOrUpdatedLocation is null)
                return AddOrUpdateResult<Location>.Failed(newOrUpdatedLocation,
                                                          "The given location must not be null!");


            if (!Locations.TryGetValue(newOrUpdatedLocation.CountryCode, out var parties))
            {
                parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                Locations.Add(newOrUpdatedLocation.CountryCode, parties);
            }

            if (!parties.TryGetValue(newOrUpdatedLocation.PartyId, out var locations))
            {
                locations = new Dictionary<Location_Id, Location>();
                parties.Add(newOrUpdatedLocation.PartyId, locations);
            }

            if (locations.TryGetValue(newOrUpdatedLocation.Id, out var existingLocation))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    newOrUpdatedLocation.LastUpdated <= existingLocation.LastUpdated)
                {
                    return AddOrUpdateResult<Location>.Failed     (newOrUpdatedLocation,
                                                                   "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!");
                }

                if (newOrUpdatedLocation.LastUpdated.ToIso8601() == existingLocation.LastUpdated.ToIso8601())
                    return AddOrUpdateResult<Location>.NoOperation(newOrUpdatedLocation,
                                                                   "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!");

                locations[newOrUpdatedLocation.Id] = newOrUpdatedLocation;
                newOrUpdatedLocation.CommonAPI = this;

                var OnLocationChangedLocal = OnLocationChanged;
                if (OnLocationChangedLocal is not null)
                {
                    try
                    {
                        OnLocationChangedLocal(newOrUpdatedLocation).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateLocation), " ", nameof(OnLocationChanged), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }

                var OnEVSEChangedLocal = OnEVSEChanged;
                if (OnEVSEChangedLocal is not null)
                {
                    try
                    {
                        foreach (var evse in newOrUpdatedLocation.EVSEs)
                            OnEVSEChangedLocal(evse).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }

                return AddOrUpdateResult<Location>.Success(newOrUpdatedLocation,
                                                           WasCreated: false);

            }

            locations.Add(newOrUpdatedLocation.Id, newOrUpdatedLocation);
            newOrUpdatedLocation.CommonAPI = this;

            var OnLocationAddedLocal = OnLocationAdded;
            if (OnLocationAddedLocal is not null)
            {
                try
                {
                    OnLocationAddedLocal(newOrUpdatedLocation).Wait();
                }
                catch (Exception e)
                {
                    DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateLocation), " ", nameof(OnLocationAdded), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace);
                }
            }

            return AddOrUpdateResult<Location>.Success(newOrUpdatedLocation,
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

            lock (Locations)
            {
                return __addOrUpdateLocation(newOrUpdatedLocation,
                                             AllowDowngrades);
            }

        }

        #endregion

        #region UpdateLocation        (Location)

        public Location? UpdateLocation(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (Locations)
            {

                if (Locations.TryGetValue(Location.CountryCode, out var parties)   &&
                    parties.  TryGetValue(Location.PartyId,     out var locations) &&
                    locations.ContainsKey(Location.Id))
                {

                    locations[Location.Id] = Location;
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
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return Location;

                }

                return null;

            }

        }

        #endregion


        #region TryPatchLocation      (Location, LocationPatch, AllowDowngrades = false)

        public async Task<PatchResult<Location>> TryPatchLocation(Location  Location,
                                                                  JObject   LocationPatch,
                                                                  Boolean?  AllowDowngrades = false)
        {

            if (Location is null)
                return PatchResult<Location>.Failed(Location,
                                                    "The given location must not be null!");

            if (LocationPatch is null || !LocationPatch.HasValues)
                return PatchResult<Location>.Failed(Location,
                                                    "The given location patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (Locations)
            {

                if (Locations.TryGetValue(Location.CountryCode, out var parties)   &&
                    parties.  TryGetValue(Location.PartyId,     out var locations) &&
                    locations.TryGetValue(Location.Id,          out var existingLocation))
                {

                    var patchResult = existingLocation.TryPatch(LocationPatch,
                                                                AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                    {

                        locations[Location.Id] = patchResult.PatchedData;

                        var OnLocationChangedLocal = OnLocationChanged;
                        if (OnLocationChangedLocal is not null)
                        {
                            try
                            {
                                OnLocationChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchLocation), " ", nameof(OnLocationChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchLocation), " ", nameof(OnEVSEChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
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

        private AddOrUpdateResult<EVSE> __addOrUpdateEVSE(Location  Location,
                                                          EVSE      newOrUpdatedEVSE,
                                                          Boolean?  AllowDowngrades = false)
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
                    DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateEVSE), " ", nameof(OnLocationChanged), ": ",
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
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEChanged), ": ",
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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEStatusChanged), ": ",
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
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSERemoved), ": ",
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
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEAdded), ": ",
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

            lock (Locations)
            {
                return __addOrUpdateEVSE(Location,
                                         newOrUpdatedEVSE,
                                         (AllowDowngrades ?? this.AllowDowngrades) == false);
            }

        }

        #endregion

        #region TryPatchEVSE          (Location, EVSE, EVSEPatch,  AllowDowngrades = false)

        public async Task<PatchResult<EVSE>> TryPatchEVSE(Location  Location,
                                                          EVSE      EVSE,
                                                          JObject   EVSEPatch,
                                                          Boolean?  AllowDowngrades = false)
        {

            if (!EVSEPatch.HasValues)
                return PatchResult<EVSE>.Failed(EVSE,
                                                "The given EVSE patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (Locations)
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
                                    DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchEVSE), " ", nameof(OnEVSEStatusChanged), ": ",
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
                                    DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchEVSE), " ", nameof(OnEVSEChanged), ": ",
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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchEVSE), " ", nameof(OnEVSERemoved), ": ",
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

        public async Task<AddOrUpdateResult<Connector>> AddOrUpdateConnector(Location   Location,
                                                                             EVSE       EVSE,
                                                                             Connector  newOrUpdatedConnector,
                                                                             Boolean?   AllowDowngrades = false)
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

            lock (Locations)
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
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateConnector), " ", nameof(OnLocationChanged), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }


                return AddOrUpdateResult<Connector>.Success(newOrUpdatedConnector,
                                                            WasCreated: !ConnectorExistedBefore);

            }

        }

        #endregion

        #region TryPatchConnector     (Location, EVSE, Connector, ConnectorPatch, AllowDowngrades = false)

        public async Task<PatchResult<Connector>> TryPatchConnector(Location   Location,
                                                                    EVSE       EVSE,
                                                                    Connector  Connector,
                                                                    JObject    ConnectorPatch,
                                                                    Boolean?   AllowDowngrades = false)
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

            lock (Locations)
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

            lock (Locations)
            {

                if (Locations.TryGetValue(CountryCode, out var parties))
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

            lock (Locations)
            {

                if (Locations.TryGetValue(CountryCode, out var parties))
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

            lock (Locations)
            {

                var allLocations = new List<Location>();

                foreach (var party in Locations.Values)
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

            lock (Locations)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (Locations.TryGetValue(CountryCode.Value, out var parties))
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

                    foreach (var party in Locations.Values)
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
                    if (Locations.TryGetValue(CountryCode.Value, out var parties))
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

                    foreach (var party in Locations.Values)
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

        public Location RemoveLocation(Location Location)
        {

            lock (Locations)
            {

                if (Locations.TryGetValue(Location.CountryCode, out var parties))
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
                        Locations.Remove(Location.CountryCode);

                }

                return Location;

            }

        }

        #endregion

        #region RemoveAllLocations()

        /// <summary>
        /// Remove all locations.
        /// </summary>
        public void RemoveAllLocations()
        {

            lock (Locations)
            {
                Locations.Clear();
            }

        }

        #endregion

        #region RemoveAllLocations(CountryCode, PartyId)

        /// <summary>
        /// Remove all locations owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllLocations(CountryCode  CountryCode,
                                       Party_Id     PartyId)
        {

            lock (Locations)
            {

                if (Locations.TryGetValue(CountryCode, out var parties))
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

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Tariff_Id , Tariff>>> tariffs;


        public delegate Task OnTariffAddedDelegate(Tariff Tariff);

        public event OnTariffAddedDelegate? OnTariffAdded;


        public delegate Task OnTariffChangedDelegate(Tariff Tariff);

        public event OnTariffChangedDelegate? OnTariffChanged;


        public GetTariffIds2_Delegate?        GetTariffIdsDelegate       { get; set; }


        #region AddTariff           (Tariff, SkipNotifications = false)

        public Tariff AddTariff(Tariff   Tariff,
                                Boolean  SkipNotifications   = false)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (tariffs)
            {

                if (!this.tariffs.TryGetValue(Tariff.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>>();
                    this.tariffs.Add(Tariff.CountryCode, parties);
                }

                if (!parties.TryGetValue(Tariff.PartyId, out var tariffs))
                {
                    tariffs = new Dictionary<Tariff_Id, Tariff>();
                    parties.Add(Tariff.PartyId, tariffs);
                }

                if (!tariffs.ContainsKey(Tariff.Id))
                {

                    tariffs.Add(Tariff.Id, Tariff);
                    Tariff.CommonAPI = this;

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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddTariff), " ", nameof(OnTariffAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }

                    return Tariff;

                }

                throw new ArgumentException("The given tariff already exists!");

            }

        }

        #endregion

        #region AddTariffIfNotExists(Tariff, SkipNotifications = false)

        public Tariff AddTariffIfNotExists(Tariff   Tariff,
                                           Boolean  SkipNotifications   = false)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (tariffs)
            {

                if (!this.tariffs.TryGetValue(Tariff.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>>();
                    this.tariffs.Add(Tariff.CountryCode, parties);
                }

                if (!parties.TryGetValue(Tariff.PartyId, out var tariffs))
                {
                    tariffs = new Dictionary<Tariff_Id, Tariff>();
                    parties.Add(Tariff.PartyId, tariffs);
                }

                if (!tariffs.ContainsKey(Tariff.Id))
                {

                    tariffs.Add(Tariff.Id, Tariff);
                    Tariff.CommonAPI = this;

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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddTariffIfNotExists), " ", nameof(OnTariffAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }

                }

                return Tariff;

            }

        }

        #endregion

        #region AddOrUpdateTariff   (newOrUpdatedTariff, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<Tariff>> AddOrUpdateTariff(Tariff    newOrUpdatedTariff,
                                                                       Boolean?  AllowDowngrades = false)
        {

            if (newOrUpdatedTariff is null)
                throw new ArgumentNullException(nameof(newOrUpdatedTariff), "The given charging tariff must not be null!");

            lock (tariffs)
            {

                if (!this.tariffs.TryGetValue(newOrUpdatedTariff.CountryCode, out var parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>>();
                    this.tariffs.Add(newOrUpdatedTariff.CountryCode, parties);
                }

                if (!parties.TryGetValue(newOrUpdatedTariff.PartyId, out var tariffs))
                {
                    tariffs = new Dictionary<Tariff_Id, Tariff>();
                    parties.Add(newOrUpdatedTariff.PartyId, tariffs);
                }

                if (tariffs.TryGetValue(newOrUpdatedTariff.Id, out var existingTariff))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        newOrUpdatedTariff.LastUpdated <= existingTariff.LastUpdated)
                    {
                        return AddOrUpdateResult<Tariff>.Failed(newOrUpdatedTariff,
                                                                "The 'lastUpdated' timestamp of the new charging tariff must be newer then the timestamp of the existing tariff!");
                    }

                    tariffs[newOrUpdatedTariff.Id] = newOrUpdatedTariff;

                    var OnTariffChangedLocal = OnTariffChanged;
                    if (OnTariffChangedLocal is not null)
                    {
                        try
                        {
                            OnTariffChangedLocal(newOrUpdatedTariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateTariff), " ", nameof(OnTariffChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return AddOrUpdateResult<Tariff>.Success(newOrUpdatedTariff,
                                                             WasCreated: false);

                }

                tariffs.Add(newOrUpdatedTariff.Id, newOrUpdatedTariff);

                var OnTariffAddedLocal = OnTariffAdded;
                if (OnTariffAddedLocal is not null)
                {
                    try
                    {
                        OnTariffAddedLocal(newOrUpdatedTariff).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateTariff), " ", nameof(OnTariffAdded), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }

                return AddOrUpdateResult<Tariff>.Success(newOrUpdatedTariff,
                                                         WasCreated: true);

            }

        }

        #endregion

        #region UpdateTariff        (Tariff)

        public Tariff? UpdateTariff(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (tariffs)
            {

                if (tariffs.TryGetValue(Tariff.CountryCode, out var parties) &&
                    parties.TryGetValue(Tariff.PartyId,     out var _tariffs) &&
                    _tariffs.ContainsKey(Tariff.Id))
                {

                    _tariffs[Tariff.Id] = Tariff;

                    var OnTariffChangedLocal = OnTariffChanged;
                    if (OnTariffChangedLocal is not null)
                    {
                        try
                        {
                            OnTariffChangedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(UpdateTariff), " ", nameof(OnTariffChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return Tariff;

                }

                return null;

            }

        }

        #endregion


        #region TryPatchTariff      (Tariff, TariffPatch, AllowDowngrades = false)

        public async Task<PatchResult<Tariff>> TryPatchTariff(Tariff    Tariff,
                                                              JObject   TariffPatch,
                                                              Boolean?  AllowDowngrades = false)
        {

            if (Tariff is null)
                return PatchResult<Tariff>.Failed(Tariff,
                                                  "The given charging tariff must not be null!");

            if (TariffPatch is null || !TariffPatch.HasValues)
                return PatchResult<Tariff>.Failed(Tariff,
                                                  "The given charging tariff patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (tariffs)
            {

                if (tariffs. TryGetValue(Tariff.CountryCode, out var parties)  &&
                    parties. TryGetValue(Tariff.PartyId,     out var _tariffs) &&
                    _tariffs.TryGetValue(Tariff.Id,          out var tariff))
                {

                    var patchResult = tariff.TryPatch(TariffPatch,
                                                      AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                    {

                        _tariffs[Tariff.Id] = patchResult.PatchedData;

                        var OnTariffChangedLocal = OnTariffChanged;
                        if (OnTariffChangedLocal is not null)
                        {
                            try
                            {
                                OnTariffChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchTariff), " ", nameof(OnTariffChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }


                    return patchResult;

                }

                else
                    return PatchResult<Tariff>.Failed(Tariff,
                                                      "The given charging tariff does not exist!");

            }

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

        public IEnumerable<Tariff> GetTariffs(Func<Tariff, Boolean> IncludeTariff)
        {

            lock (tariffs)
            {

                var allTariffs = new List<Tariff>();

                foreach (var party in tariffs.Values)
                {
                    foreach (var partyTariffs in party.Values)
                    {
                        foreach (var tariff in partyTariffs.Values)
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

        public IEnumerable<Tariff> GetTariffs(CountryCode? CountryCode  = null,
                                              Party_Id?    PartyId      = null)
        {

            lock (tariffs)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (tariffs.TryGetValue(CountryCode.Value, out var parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out var tariffs))
                        {
                            return tariffs.Values.ToArray();
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
                            allTariffs.AddRange(tariffs.Values);
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
                            allTariffs.AddRange(tariffs.Values);
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
                            allTariffs.AddRange(tariffs.Values);
                        }
                    }

                    return allTariffs;

                }

                return Array.Empty<Tariff>();

            }

        }

        #endregion

        #region GetTariffIds(CountryCode?, PartyId?, LocationId?, EVSEUId?, ConnectorId?, EMPId?)

        public IEnumerable<Tariff_Id> GetTariffIds(CountryCode    CountryCode,
                                                   Party_Id       PartyId,
                                                   Location_Id?   LocationId,
                                                   EVSE_UId?      EVSEUId,
                                                   Connector_Id?  ConnectorId,
                                                   EMP_Id?        EMPId)

            => GetTariffIdsDelegate?.Invoke(CountryCode,
                                            PartyId,
                                            LocationId,
                                            EVSEUId,
                                            ConnectorId,
                                            EMPId) ?? Array.Empty<Tariff_Id>();

        #endregion


        #region RemoveTariff    (Tariff)

        public Boolean RemoveTariff(Tariff Tariff)
        {

            lock (tariffs)
            {

                var success = false;

                if (tariffs.TryGetValue(Tariff.CountryCode, out var parties))
                {

                    if (parties.TryGetValue(Tariff.PartyId, out var _tariffs))
                    {

                        if (_tariffs.ContainsKey(Tariff.Id))
                        {
                            success = _tariffs.Remove(Tariff.Id);
                        }

                        if (!_tariffs.Any())
                            parties.Remove(Tariff.PartyId);

                    }

                    if (!parties.Any())
                        chargingSessions.Remove(Tariff.CountryCode);

                }

                return success;

            }

        }

        #endregion

        #region RemoveTariff    (TariffId)

        public Boolean RemoveTariff(Tariff_Id TariffId)
        {

            lock (tariffs)
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
                    return tariffs[countryCode.Value][partyId.Value].Remove(TariffId);
                }

                return false;

            }

        }

        #endregion

        #region RemoveAllTariffs(IncludeTariffs = null)

        /// <summary>
        /// Remove all matching charging tariffs.
        /// </summary>
        /// <param name="IncludeSessions">An optional charging tariff filter.</param>
        public void RemoveAllTariffs(Func<Tariff, Boolean>? IncludeTariffs = null)
        {

            lock (tariffs)
            {

                if (IncludeTariffs is null)
                    tariffs.Clear();

                else
                {

                    var tariffsToDelete = tariffs.Values.SelectMany(xx => xx.Values).
                                                         SelectMany(yy => yy.Values).
                                                         Where(IncludeTariffs).
                                                         ToArray();

                    foreach (var tariff in tariffsToDelete)
                        tariffs[tariff.CountryCode][tariff.PartyId].Remove(tariff.Id);

                }

            }

        }

        #endregion

        #region RemoveAllTariffs(CountryCode, PartyId)

        /// <summary>
        /// Remove all tariffs owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllTariffs(CountryCode  CountryCode,
                                     Party_Id     PartyId)
        {

            lock (tariffs)
            {

                if (tariffs.TryGetValue(CountryCode, out var parties))
                {
                    if (parties.TryGetValue(PartyId, out var tariffs))
                    {
                        tariffs.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion

        #region Sessions

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Session_Id , Session>>> chargingSessions;


        public delegate Task OnSessionCreatedDelegate(Session Session);

        public event OnSessionCreatedDelegate? OnSessionCreated;

        public delegate Task OnSessionChangedDelegate(Session Session);

        public event OnSessionChangedDelegate? OnSessionChanged;


        #region AddSession           (Session, SkipNotifications = false)

        public Session AddSession(Session  Session,
                                  Boolean  SkipNotifications   = false)
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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddSession), " ", nameof(OnSessionCreated), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
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

        public Session AddSessionIfNotExists(Session  Session,
                                             Boolean  SkipNotifications   = false)
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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddSessionIfNotExists), " ", nameof(OnSessionCreated), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }

                }

                return Session;

            }

        }

        #endregion

        #region AddOrUpdateSession   (newOrUpdatedSession, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<Session>> AddOrUpdateSession(Session   newOrUpdatedSession,
                                                                         Boolean?  AllowDowngrades = false)
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
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateSession), " ", nameof(OnSessionChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
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
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateSession), " ", nameof(OnSessionCreated), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }

                return AddOrUpdateResult<Session>.Success(newOrUpdatedSession,
                                                          WasCreated: true);

            }

        }

        #endregion

        #region UpdateSession        (Session)

        public Session? UpdateSession(Session Session)
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
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(UpdateSession), " ", nameof(OnSessionChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return Session;

                }

                return null;

            }

        }

        #endregion


        #region TryPatchSession      (Session, SessionPatch, AllowDowngrades = false)

        public async Task<PatchResult<Session>> TryPatchSession(Session   Session,
                                                                JObject   SessionPatch,
                                                                Boolean?  AllowDowngrades = false)
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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchSession), " ", nameof(OnSessionChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
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

        public Boolean RemoveSession(Session Session)
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

        public Boolean RemoveSession(Session_Id SessionId)
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
        public void RemoveAllSessions(Func<Session, Boolean>? IncludeSessions = null)
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
        public void RemoveAllSessions(CountryCode  CountryCode,
                                      Party_Id     PartyId)
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

        public Token AddToken(Token         Token,
                              AllowedType?  Status              = null,
                              Boolean       SkipNotifications   = false)
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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddToken), " ", nameof(OnTokenAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
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

        public Token AddTokenIfNotExists(Token         Token,
                                         AllowedType?  Status              = null,
                                         Boolean       SkipNotifications   = false)
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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddTokenIfNotExists), " ", nameof(OnTokenAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }

                }

                return Token;

            }

        }

        #endregion

        #region AddOrUpdateToken   (newOrUpdatedToken, Status = AllowedTypes.ALLOWED, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<Token>> AddOrUpdateToken(Token         newOrUpdatedToken,
                                                                     AllowedType?  Status            = null,
                                                                     Boolean?      AllowDowngrades   = false)
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
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateToken), " ", nameof(OnTokenChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
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
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateToken), " ", nameof(OnTokenAdded), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }

                return AddOrUpdateResult<Token>.Success(newOrUpdatedToken,
                                                        WasCreated: true);

            }

        }

        #endregion


        #region TryPatchToken      (Token, TokenPatch, AllowDowngrades = false)

        public async Task<PatchResult<Token>> TryPatchToken(Token     Token,
                                                            JObject   TokenPatch,
                                                            Boolean?  AllowDowngrades = false)
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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchToken), " ", nameof(OnTokenChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
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

        public Boolean RemoveToken(Token Token)
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

        public Boolean RemoveToken(Token_Id TokenId)
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
        public void RemoveAllTokens(Func<Token, Boolean>? IncludeTokens = null)
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
        public void RemoveAllTokens(CountryCode  CountryCode,
                                    Party_Id     PartyId)
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

        public CDR AddCDR(CDR      CDR,
                          Boolean  SkipNotifications   = false)
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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddCDR), " ", nameof(OnChargeDetailRecordAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
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

        public CDR AddCDRIfNotExists(CDR      CDR,
                                     Boolean  SkipNotifications   = false)
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
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddCDRIfNotExists), " ", nameof(OnChargeDetailRecordAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }

                }

                return CDR;

            }

        }

        #endregion

        #region AddOrUpdateCDR   (newOrUpdatedCDR, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<CDR>> AddOrUpdateCDR(CDR       newOrUpdatedCDR,
                                                                 Boolean?  AllowDowngrades = false)
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
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateCDR), " ", nameof(OnChargeDetailRecordChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
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
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateCDR), " ", nameof(OnChargeDetailRecordAdded), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }

                return AddOrUpdateResult<CDR>.Success(newOrUpdatedCDR,
                                                      WasCreated: true);

            }

        }

        #endregion

        #region UpdateCDR        (CDR)

        public CDR? UpdateCDR(CDR CDR)
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
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(UpdateCDR), " ", nameof(OnChargeDetailRecordChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
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
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryGetCDR), " ", nameof(OnChargeDetailRecordLookup), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
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
        public Boolean RemoveCDR(CDR CDR)
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

        public Boolean RemoveCDR(CDR_Id CDRId)
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
        public void RemoveAllCDRs(Func<CDR, Boolean>? IncludeCDRs = null)
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
        public void RemoveAllCDRs(CountryCode  CountryCode,
                                  Party_Id     PartyId)
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


        #region Start()

        public void Start()
        {

            lock (HTTPServer)
            {

                if (!HTTPServer.IsStarted)
                    HTTPServer.Start();

                #region Send 'Open Data API restarted'-e-mail...

                //var Message0 = new HTMLEMailBuilder() {
                //    From        = _APIEMailAddress,
                //    To          = _APIAdminEMail,
                //    Subject     = "Open Data API '" + _ServiceName + "' restarted! at " + Timestamp.Now.ToString(),
                //    PlainText   = "Open Data API '" + _ServiceName + "' restarted! at " + Timestamp.Now.ToString(),
                //    HTMLText    = "Open Data API <b>'" + _ServiceName + "'</b> restarted! at " + Timestamp.Now.ToString(),
                //    Passphrase  = _APIPassphrase
                //};
                //
                //var SMTPTask = _APISMTPClient.Send(Message0);
                //SMTPTask.Wait();

                //var r = SMTPTask.Result;

                #endregion

                //SendStarted(this, Timestamp.Now);

            }

        }

        #endregion

        #region Shutdown(Message = null, Wait = true)

        public void Shutdown(String Message = null, Boolean Wait = true)
        {

            lock (HTTPServer)
            {

                HTTPServer.Shutdown(Message, Wait);

                //SendCompleted(this, Timestamp.Now, Message);

            }

        }

        #endregion

    }

}
