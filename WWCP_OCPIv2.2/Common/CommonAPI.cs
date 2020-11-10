/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// Extention methods for the Common API.
    /// </summary>
    public static class CommonAPIExtentions
    {

    }


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


        public Boolean                       VersionsURLusesHTTPS       { get; }

        public HTTPPath?                     AdditionalURLPathPrefix    { get; }

        /// <summary>
        /// Allow anonymous access to locations as Open Data.
        /// </summary>
        public Boolean                       LocationsAsOpenData        { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?                      AllowDowngrades            { get; }

        #endregion

        #region Events

        #region (protected internal) PostCredentialsRequest   (Request)

        /// <summary>
        /// An event sent whenever a post credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostCredentialsRequest = new OCPIRequestLogEvent();

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
        public OCPIResponseLogEvent OnPostCredentialsResponse = new OCPIResponseLogEvent();

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
        public OCPIRequestLogEvent OnPutCredentialsRequest = new OCPIRequestLogEvent();

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
        public OCPIResponseLogEvent OnPutCredentialsResponse = new OCPIResponseLogEvent();

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
        public OCPIRequestLogEvent OnDeleteCredentialsRequest = new OCPIRequestLogEvent();

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
        public OCPIResponseLogEvent OnDeleteCredentialsResponse = new OCPIResponseLogEvent();

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
        /// 
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="HTTPServerPort">An optional HTTP TCP port.</param>
        /// <param name="HTTPServerName">An optional HTTP server name.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional HTTP URL path prefix.</param>
        /// <param name="ServiceName">An optional HTTP service name.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        /// 
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        public CommonAPI(URL                           OurVersionsURL,
                         IEnumerable<CredentialsRole>  OurCredentialRoles,

                         HTTPHostname?                 HTTPHostname              = null,
                         IPPort?                       HTTPServerPort            = null,
                         String                        HTTPServerName            = DefaultHTTPServerName,
                         String                        ExternalDNSName           = null,
                         HTTPPath?                     URLPathPrefix             = null,
                         String                        ServiceName               = DefaultHTTPServiceName,
                         DNSClient                     DNSClient                 = null,

                         Boolean                       VersionsURLusesHTTPS      = true,
                         HTTPPath?                     AdditionalURLPathPrefix   = null,
                         Boolean                       LocationsAsOpenData       = true,
                         Boolean?                      AllowDowngrades           = null)

            : base(HTTPHostname,
                   HTTPServerPort ?? DefaultHTTPServerPort,
                   HTTPServerName ?? DefaultHTTPServerName,
                   ExternalDNSName,
                   URLPathPrefix  ?? DefaultURLPathPrefix,
                   ServiceName    ?? DefaultHTTPServiceName,
                   DNSClient)

        {

            if (!OurCredentialRoles.SafeAny())
                throw new ArgumentNullException(nameof(OurCredentialRoles), "The given credential roles must not be null or empty!");

            this.OurVersionsURL           = OurVersionsURL;
            this.OurCredentialRoles       = OurCredentialRoles?.Distinct();
            this.VersionsURLusesHTTPS     = VersionsURLusesHTTPS;
            this.AdditionalURLPathPrefix  = AdditionalURLPathPrefix;
            this.LocationsAsOpenData      = LocationsAsOpenData;
            this.AllowDowngrades          = AllowDowngrades;

            this.AccessTokens             = new Dictionary<AccessToken, AccessInfo>();
            this.RemoteAccessInfos        = new List<RemoteAccessInfo>();
            this.Locations                = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id, Location>>>();
            this.Tariffs                  = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Tariff_Id,   Tariff>>>();
            this.Sessions                 = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Session_Id,  Session>>>();
            this.Tokens                   = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Token_Id,    TokenStatus>>>();
            this.CDRs                     = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<CDR_Id,      CDR>>>();

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
        /// <param name="ServiceName">An optional name of the HTTP API service.</param>
        /// 
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        public CommonAPI(URL                           OurVersionsURL,
                         IEnumerable<CredentialsRole>  OurCredentialRoles,

                         HTTPServer                    HTTPServer,
                         HTTPHostname?                 HTTPHostname              = null,
                         String                        ExternalDNSName           = null,
                         HTTPPath?                     URLPathPrefix             = null,
                         String                        ServiceName               = DefaultHTTPServerName,

                         Boolean                       VersionsURLusesHTTPS      = true,
                         HTTPPath?                     AdditionalURLPathPrefix   = null,
                         Boolean                       LocationsAsOpenData       = true,
                         Boolean?                      AllowDowngrades           = null)

            : base(HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   URLPathPrefix,
                   ServiceName)

        {

            if (!OurCredentialRoles.SafeAny())
                throw new ArgumentNullException(nameof(OurCredentialRoles), "The given credential roles must not be null or empty!");

            this.OurVersionsURL           = OurVersionsURL;
            this.OurCredentialRoles       = OurCredentialRoles?.Distinct();
            this.VersionsURLusesHTTPS     = VersionsURLusesHTTPS;
            this.AdditionalURLPathPrefix  = AdditionalURLPathPrefix;
            this.LocationsAsOpenData      = LocationsAsOpenData;
            this.AllowDowngrades          = AllowDowngrades;

            this.AccessTokens             = new Dictionary<AccessToken, AccessInfo>();
            this.RemoteAccessInfos        = new List<RemoteAccessInfo>();
            this.Locations                = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id, Location>>>();
            this.Tariffs                  = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Tariff_Id,   Tariff>>>();
            this.Sessions                 = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Session_Id,  Session>>>();
            this.Tokens                   = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Token_Id,    TokenStatus>>>();
            this.CDRs                     = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<CDR_Id,      CDR>>>();

            // Link HTTP events...
            HTTPServer.RequestLog        += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog       += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog          += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            RegisterURLTemplates();

        }

        #endregion

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region OPTIONS     ~/

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix,
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = DateTime.UtcNow,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "OPTIONS, GET",
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
            //                                   URLPrefix + "/", "cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CommonAPI.HTTPRoot",
            //                                   Assembly.GetCallingAssembly());

            //HTTPServer.AddMethodCallback(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             new HTTPPath[] {
            //                                 URLPrefix + "/index.html",
            //                                 URLPrefix + "/"
            //                             },
            //                             HTTPContentType.HTML_UTF8,
            //                             HTTPDelegate: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CommonAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CommonAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

            //                                 return new HTTPResponse.Builder(Request) {
            //                                     HTTPStatusCode  = HTTPStatusCode.OK,
            //                                     ContentType     = HTTPContentType.HTML_UTF8,
            //                                     Content         = _MemoryStream.ToArray(),
            //                                     Connection      = "close"
            //                                 };

            //                             });

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix,
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = DateTime.UtcNow,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "OPTIONS, GET",
                                                     AccessControlAllowHeaders  = "Authorization",
                                                     ContentType                = HTTPContentType.TEXT_UTF8,
                                                     Content                    = "This is an Open Charge Point Interface HTTP service!\r\nPlease check ~/versions!".ToUTF8Bytes(),
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion


            #region OPTIONS     ~/versions

            // ----------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions
            // ----------------------------------------------------
            HTTPServer.AddOCPIMethod(Hostname,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "versions",
                                     OCPIRequest: Request => {

                                         return Task.FromResult(
                                             new OCPIResponse.Builder(Request) {
                                                 HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     AccessControlAllowMethods  = "OPTIONS, GET",
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.GET
                                                                                  },
                                                     AccessControlAllowHeaders  = "Authorization"
                                                 }
                                             });

                                     });

            #endregion

            #region GET         ~/versions

            // ----------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions
            // ----------------------------------------------------------------------
            HTTPServer.AddOCPIMethod(Hostname,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "versions",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check access token

                                         if (Request.AccessToken.HasValue &&
                                             AccessTokens.TryGetValue(Request.AccessToken.Value, out AccessInfo accessInfo) &&
                                             accessInfo.Status != AccessStatus.ALLOWED)
                                         {

                                             return Task.FromResult(
                                                 new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
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
                                                                            new Version[] {
                                                                                new Version(
                                                                                    Version_Id.Parse("2.2"),
                                                                                    URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                              (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "/versions/2.2").Replace("//", "/"))
                                                                                )
                                                                            }.Select(version => version.ToJSON())
                                                                        ),
                                                 HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     AccessControlAllowMethods  = "OPTIONS, GET",
                                                     AccessControlAllowHeaders  = "Authorization"
                                                 }
                                             });

                                     });

            #endregion


            #region OPTIONS     ~/versions/{id}

            // --------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions/{id}
            // --------------------------------------------------------
            HTTPServer.AddOCPIMethod(Hostname,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "versions/{id}",
                                     OCPIRequest: Request => {

                                         return Task.FromResult(
                                             new OCPIResponse.Builder(Request) {
                                                 HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     AccessControlAllowMethods  = "OPTIONS, GET",
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.GET
                                                                                  },
                                                     AccessControlAllowHeaders  = "Authorization"
                                                 }
                                             });
                                     });

            #endregion

            #region GET         ~/versions/{id}

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions/{id}
            // ---------------------------------------------------------------------------
            HTTPServer.AddOCPIMethod(Hostname,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "versions/{id}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check access token

                                         AccessInfo accessInfo = default;

                                         if (Request.AccessToken.HasValue &&
                                             AccessTokens.TryGetValue(Request.AccessToken.Value, out accessInfo) &&
                                             accessInfo.Status != AccessStatus.ALLOWED)
                                         {

                                             return Task.FromResult(
                                                 new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
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
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                         }

                                         if (!Version_Id.TryParse(Request.ParsedURLParameters[0], out Version_Id versionId))
                                         {

                                             return Task.FromResult(
                                                 new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Version identification is invalid!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                         }

                                         #endregion

                                         #region Only allow versionId == "2.2"

                                         if (versionId.ToString() != "2.2")
                                         {

                                             return Task.FromResult(
                                                 new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Version identification is unknown!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                         }

                                         #endregion


                                         #region Common credential endpoints...

                                         var endpoints  = new List<VersionEndpoint>() {

                                                              new VersionEndpoint(ModuleIDs.Credentials,
                                                                                  InterfaceRoles.SENDER,
                                                                                  URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                            (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/credentials").Replace("//", "/"))),

                                                              new VersionEndpoint(ModuleIDs.Credentials,
                                                                                  InterfaceRoles.RECEIVER,
                                                                                  URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                            (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/credentials").Replace("//", "/")))

                                                          };

                                         #endregion


                                         #region The other side is a CPO...

                                         if (accessInfo.Roles?.Any(role  => role.Role == Roles.CPO) == true)
                                         {

                                             endpoints.Add(new VersionEndpoint(ModuleIDs.Locations,
                                                                               InterfaceRoles.RECEIVER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") + 
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/emsp/locations").Replace("//", "/"))));

                                             endpoints.Add(new VersionEndpoint(ModuleIDs.Tariffs,
                                                                               InterfaceRoles.RECEIVER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/emsp/tariffs").  Replace("//", "/"))));

                                             endpoints.Add(new VersionEndpoint(ModuleIDs.Sessions,
                                                                               InterfaceRoles.RECEIVER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/emsp/sessions"). Replace("//", "/"))));

                                             endpoints.Add(new VersionEndpoint(ModuleIDs.CDRs,
                                                                               InterfaceRoles.RECEIVER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/emsp/cdrs").     Replace("//", "/"))));


                                             endpoints.Add(new VersionEndpoint(ModuleIDs.Commands,
                                                                               InterfaceRoles.SENDER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/emsp/commands"). Replace("//", "/"))));

                                             endpoints.Add(new VersionEndpoint(ModuleIDs.Tokens,
                                                                               InterfaceRoles.SENDER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/emsp/tokens").   Replace("//", "/"))));

                                             // hubclientinfo

                                         }

                                         #endregion

                                         #region The other side is an EMP or unauthenticated (Open Data Access)...

                                         if (accessInfo.Roles?.Any(role => role.Role == Roles.EMSP)     == true ||
                                             accessInfo.Roles?.Any(role => role.Role == Roles.OpenData) == true ||
                                            (accessInfo.Roles == null && LocationsAsOpenData))
                                         {

                                             endpoints.Add(new VersionEndpoint(ModuleIDs.Locations,
                                                                               InterfaceRoles.SENDER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/locations").Replace("//", "/"))));

                                         }

                                         #endregion

                                         #region The other side is an EMP...

                                         if (accessInfo.Roles?.Any(role => role.Role == Roles.EMSP) == true)
                                         {

                                             endpoints.Add(new VersionEndpoint(ModuleIDs.CDRs,
                                                                               InterfaceRoles.SENDER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/cdrs").            Replace("//", "/"))));

                                             endpoints.Add(new VersionEndpoint(ModuleIDs.Sessions,
                                                                               InterfaceRoles.SENDER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/sessions").        Replace("//", "/"))));


                                             endpoints.Add(new VersionEndpoint(ModuleIDs.Locations,
                                                                               InterfaceRoles.SENDER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/locations").       Replace("//", "/"))));

                                             endpoints.Add(new VersionEndpoint(ModuleIDs.Tariffs,
                                                                               InterfaceRoles.SENDER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/tariffs").         Replace("//", "/"))));

                                             endpoints.Add(new VersionEndpoint(ModuleIDs.Sessions,
                                                                               InterfaceRoles.SENDER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/sessions").        Replace("//", "/"))));

                                             endpoints.Add(new VersionEndpoint(ModuleIDs.ChargingProfiles,
                                                                               InterfaceRoles.SENDER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/chargingprofiles").Replace("//", "/"))));

                                             endpoints.Add(new VersionEndpoint(ModuleIDs.CDRs,
                                                                               InterfaceRoles.SENDER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/cdrs").            Replace("//", "/"))));


                                             endpoints.Add(new VersionEndpoint(ModuleIDs.Commands,
                                                                               InterfaceRoles.RECEIVER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/commands").        Replace("//", "/"))));

                                             endpoints.Add(new VersionEndpoint(ModuleIDs.Tokens,
                                                                               InterfaceRoles.RECEIVER,
                                                                               URL.Parse((VersionsURLusesHTTPS ? "https://" : "http://") +
                                                                                         (Request.Host + URLPathPrefix + AdditionalURLPathPrefix + "2.2/cpo/tokens").          Replace("//", "/"))));

                                             // hubclientinfo

                                         }

                                         #endregion


                                         return Task.FromResult(
                                             new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 1000,
                                                    StatusMessage        = "Hello world!",
                                                    Data                 = new VersionDetail(
                                                                               Version_Id.Parse("2.2"),
                                                                               endpoints
                                                                           ).ToJSON(),
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                     });

            #endregion


            #region OPTIONS     ~/2.2/credentials

            // ----------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/2.2/credentials
            // ----------------------------------------------------------
            HTTPServer.AddOCPIMethod(Hostname,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "2.2/credentials",
                                     OCPIRequest: Request => {

                                         return Task.FromResult(
                                             new OCPIResponse.Builder(Request) {
                                                 HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
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

            #region GET         ~/2.2/credentials

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.2/credentials
            // ---------------------------------------------------------------------------------
            HTTPServer.AddOCPIMethod(Hostname,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "2.2/credentials",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         if (Request.AccessToken.HasValue &&
                                             AccessTokens.TryGetValue(Request.AccessToken.Value, out AccessInfo accessInfo) &&
                                             accessInfo.VersionsURL.HasValue &&
                                             accessInfo.Status == AccessStatus.ALLOWED)
                                         {

                                             return Task.FromResult(
                                                 new OCPIResponse.Builder(Request) {
                                                     StatusCode           = 1000,
                                                     StatusMessage        = "Hello world!",
                                                     Data                 = accessInfo.AsCredentials().ToJSON(),
                                                     HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                         AccessControlAllowHeaders  = "Authorization"
                                                     }
                                                 });

                                         }

                                         return Task.FromResult(
                                             new OCPIResponse.Builder(Request) {
                                                 StatusCode           = 2000,
                                                 StatusMessage        = "You need to be registered before trying to invoke this protected method.",
                                                 HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                     HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                     AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                     AccessControlAllowHeaders  = "Authorization"
                                                 }
                                             });

                                     });

            #endregion

            #region POST        ~/2.2/credentials

            // REGISTER new OCPI party!

            // -----------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.2/credentials
            // -----------------------------------------------------------------------------
            HTTPServer.AddOCPIMethod(Hostname,
                                     HTTPMethod.POST,
                                     URLPathPrefix + "2.2/credentials",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   PostCredentialsRequest,
                                     OCPIResponseLogger:  PostCredentialsResponse,
                                     OCPIRequest:   async Request => {

                                         var CREDENTIALS_TOKEN_A = Request.AccessToken;

                                         if (CREDENTIALS_TOKEN_A.HasValue &&
                                             AccessTokens.TryGetValue(CREDENTIALS_TOKEN_A.Value, out AccessInfo accessInfo) &&
                                             accessInfo.Status == AccessStatus.ALLOWED)
                                         {

                                             if (accessInfo.VersionsURL.HasValue)
                                                 return new OCPIResponse.Builder(Request) {
                                                            StatusCode           = 2000,
                                                            StatusMessage        = "The given access token '" + CREDENTIALS_TOKEN_A.Value.ToString() + "' is already registered!",
                                                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                                AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                                AccessControlAllowHeaders  = "Authorization"
                                                            }
                                                        };


                                             return await POSTOrPUTCredentials(Request);


                                         }

                                         return new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "You need to be registered before trying to invoke this protected method.",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                };

                                     });

            #endregion

            #region PUT         ~/2.2/credentials

            // UPDATE the registration of an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.2/credentials
            // ---------------------------------------------------------------------------------
            HTTPServer.AddOCPIMethod(Hostname,
                                     HTTPMethod.PUT,
                                     URLPathPrefix + "2.2/credentials",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   PutCredentialsRequest,
                                     OCPIResponseLogger:  PutCredentialsResponse,
                                     OCPIRequest:   async Request => {

                                         if (Request.AccessToken.HasValue &&
                                             AccessTokens.TryGetValue(Request.AccessToken.Value, out AccessInfo accessInfo) &&
                                             accessInfo.Status == AccessStatus.ALLOWED)
                                         {

                                             if (!accessInfo.VersionsURL.HasValue)
                                                 return new OCPIResponse.Builder(Request) {
                                                            StatusCode           = 2000,
                                                            StatusMessage        = "The given access token '" + Request.AccessToken.Value.ToString() + "' is not yet registered!",
                                                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                                AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                                AccessControlAllowHeaders  = "Authorization"
                                                            }
                                                        };

                                             return await POSTOrPUTCredentials(Request);


                                         }

                                         return new OCPIResponse.Builder(Request) {
                                                        StatusCode           = 2000,
                                                        StatusMessage        = "You need to be registered before trying to invoke this protected method.",
                                                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                            AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                            AccessControlAllowHeaders  = "Authorization"
                                                        }
                                                    };

                                     });

            #endregion

            #region DELETE      ~/2.2/credentials

            // UNREGISTER an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.2/credentials
            // ---------------------------------------------------------------------------------
            HTTPServer.AddOCPIMethod(Hostname,
                                     HTTPMethod.DELETE,
                                     URLPathPrefix + "2.2/credentials",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   DeleteCredentialsRequest,
                                     OCPIResponseLogger:  DeleteCredentialsResponse,
                                     OCPIRequest:   async Request => {

                                         if (Request.AccessToken.HasValue &&
                                             AccessTokens.TryGetValue(Request.AccessToken.Value, out AccessInfo accessInfo) &&
                                             accessInfo.Status == AccessStatus.ALLOWED)
                                         {

                                             #region Validations

                                             if (!accessInfo.VersionsURL.HasValue)
                                                 return new OCPIResponse.Builder(Request) {
                                                            StatusCode           = 2000,
                                                            StatusMessage        = "The given access token '" + Request.AccessToken.Value.ToString() + "' is not registered!",
                                                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                                AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
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
                                                            AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                            AccessControlAllowHeaders  = "Authorization"
                                                        }
                                                    };

                                         }

                                         return new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "You need to be registered before trying to invoke this protected method.",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
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

            var CREDENTIALS_TOKEN_A = Request.AccessToken;

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
                               AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                               AccessControlAllowHeaders  = "Authorization"
                           }
                       };

            }

            #endregion

            #region Additional security checks... (Non-Standard)

            lock (AccessTokens)
            {
                foreach (var credentialsRole in receivedCredentials.Roles)
                {

                    var result = AccessTokens.Values.Where(accessToken => accessToken.Roles.Any(role => role.CountryCode == credentialsRole.CountryCode &&
                                                                                                        role.PartyId     == credentialsRole.PartyId &&
                                                                                                        role.Role        == credentialsRole.Role)).ToArray();

                    if (result.Length == 0)
                    {

                        return new OCPIResponse.Builder(Request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "The given combination of country code, party identification and role is unknown!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                       AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                       AccessControlAllowHeaders  = "Authorization"
                                   }
                               };

                    }

                    if (result.Length > 0 &&
                        result.First().VersionsURL.HasValue)
                    {

                        return new OCPIResponse.Builder(Request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "The given combination of country code, party identification and role is already registered!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                       AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                       AccessControlAllowHeaders  = "Authorization"
                                   }
                               };

                    }

                }
            }

            #endregion


            var commonClient            = new CommonClient(receivedCredentials.Token,  // CREDENTIALS_TOKEN_B
                                                           receivedCredentials.URL,
                                                           URL.Parse("https://localhost:1234/commands"),
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
                               AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
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
                               AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
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
                               AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                               AccessControlAllowHeaders  = "Authorization"
                           }
                       };

            }

            #endregion


            // Store credential of the other side!
            foreach (var role in receivedCredentials.Roles)
                SetRemoteAccessInfo(role.CountryCode,
                                    role.PartyId,
                                    role.Role,
                                    role.BusinessDetails,
                                    receivedCredentials.Token,
                                    receivedCredentials.URL,
                                    otherVersions.Data.Select(version => version.Id),
                                    RemoteAccessStatus.ONLINE);


            var CREDENTIALS_TOKEN_C = AccessToken.Random();

            SetAccessToken(CREDENTIALS_TOKEN_C,
                           receivedCredentials.URL,
                           receivedCredentials.Roles,
                           AccessStatus.ALLOWED);


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
                               AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                               AccessControlAllowHeaders  = "Authorization"
                           }
                       };

        }

        #endregion


        //ToDo: Wrap the following into a plugable interface!

        #region AccessTokens

        private readonly Dictionary<AccessToken, AccessInfo> AccessTokens;

        public CommonAPI SetAccessToken(AccessToken                   AccessToken,
                                        URL                           VersionsURL,
                                        IEnumerable<CredentialsRole>  Roles,
                                        AccessStatus                  AccessStatus   = AccessStatus.ALLOWED)
        {
            lock (AccessTokens)
            {

                if (AccessTokens.ContainsKey(AccessToken))
                    AccessTokens.Remove(AccessToken);

                AccessTokens.Add(AccessToken,
                                 new AccessInfo(AccessToken,
                                                AccessStatus,
                                                VersionsURL,
                                                Roles));

                return this;

            }
        }

        public Boolean SetAccessToken(AccessToken   AccessToken,
                                      CountryCode   CountryCode,
                                      Party_Id      PartyId,
                                      Roles         Role,
                                      String        BusinessName   = null,
                                      AccessStatus  AccessStatus   = AccessStatus.ALLOWED)
        {
            lock (AccessTokens)
            {

                if (AccessTokens.ContainsKey(AccessToken))
                    AccessTokens.Remove(AccessToken);

                var alreadyExists = AccessTokens.Where(accessToken => accessToken.Value.Roles.Any(role => role.CountryCode == CountryCode &&
                                                                                                          role.PartyId     == PartyId &&
                                                                                                          role.Role        == Role)).ToArray();

                if (alreadyExists.Length > 0)
                    return false;

                AccessTokens.Add(AccessToken,
                                 new AccessInfo(AccessToken,
                                                AccessStatus,
                                                null,
                                                new CredentialsRole[] {
                                                    new CredentialsRole(CountryCode,
                                                                        PartyId,
                                                                        Role,
                                                                        BusinessName.IsNotNullOrEmpty()
                                                                            ? new BusinessDetails(BusinessName)
                                                                            : null)
                                                }));

                return true;

            }
        }

        public CommonAPI RemoveAccessToken(AccessToken AccessToken)
        {
            lock (AccessTokens)
            {
                AccessTokens.Remove(AccessToken);
                return this;
            }
        }

        #endregion

        #region RemoteAccessInfos

        private readonly List<RemoteAccessInfo> RemoteAccessInfos;

        public Boolean SetRemoteAccessInfo(CountryCode              CountryCode,
                                           Party_Id                 PartyId,
                                           Roles                    Role,
                                           BusinessDetails          BusinessDetails,
                                           AccessToken              Token,
                                           URL?                     VersionsURL,
                                           IEnumerable<Version_Id>  VersionIds   = null,
                                           RemoteAccessStatus?      Status       = RemoteAccessStatus.ONLINE)
        {
            lock (RemoteAccessInfos)
            {

                var existingAccessInfos = RemoteAccessInfos.Where(info => info.CountryCode == CountryCode &&
                                                                          info.PartyId     == PartyId &&
                                                                          info.Role        == Role).ToArray();

                foreach (var info in existingAccessInfos)
                    RemoteAccessInfos.Remove(info);


                RemoteAccessInfos.Add(new RemoteAccessInfo(CountryCode,
                                                           PartyId,
                                                           Role,
                                                           BusinessDetails,
                                                           Token,
                                                           VersionsURL,
                                                           VersionIds,
                                                           Status));

                return true;

            }
        }

        public Boolean RemoveRemoteAccessInfo(RemoteAccessInfo RemoteAccessInfo)
        {
            lock (RemoteAccessInfos)
            {
                return RemoteAccessInfos.Remove(RemoteAccessInfo);
            }
        }

        public Boolean RemoveRemoteAccessInfos(CountryCode  CountryCode,
                                               Party_Id     PartyId,
                                               Roles        Role)
        {
            lock (RemoteAccessInfos)
            {

                var existingAccessInfos = RemoteAccessInfos.Where(info => info.CountryCode == CountryCode &&
                                                                          info.PartyId     == PartyId &&
                                                                          info.Role        == Role).ToArray();

                foreach (var info in existingAccessInfos)
                    RemoteAccessInfos.Remove(info);

                return true;

            }
        }

        #endregion


        // Add last modified timestamp to locations!

        #region Locations

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id , Location>>> Locations;


        #region AddLocation           (Location)

        public Location AddLocation(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (Locations)
            {

                if (!Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                    Locations.Add(Location.CountryCode, parties);
                }

                if (!parties.TryGetValue(Location.PartyId, out Dictionary<Location_Id, Location> locations))
                {
                    locations = new Dictionary<Location_Id, Location>();
                    parties.Add(Location.PartyId, locations);
                }

                if (!locations.ContainsKey(Location.Id))
                {
                    locations.Add(Location.Id, Location);
                    return Location;
                }

                throw new ArgumentException("The given location already exists!");

            }

        }

        #endregion

        #region AddLocationIfNotExists(Location)

        public Location AddLocationIfNotExists(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (Locations)
            {

                if (!Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                    Locations.Add(Location.CountryCode, parties);
                }

                if (!parties.TryGetValue(Location.PartyId, out Dictionary<Location_Id, Location> locations))
                {
                    locations = new Dictionary<Location_Id, Location>();
                    parties.Add(Location.PartyId, locations);
                }

                if (!locations.ContainsKey(Location.Id))
                    locations.Add(Location.Id, Location);

                return Location;

            }

        }

        #endregion

        #region AddOrUpdateLocation   (newOrUpdatedLocation)

        private AddOrUpdateResult<Location> __addOrUpdateLocation(Location  newOrUpdatedLocation,
                                                                  Boolean?  AllowDowngrades = false)
        {

            if (newOrUpdatedLocation is null)
                return AddOrUpdateResult<Location>.Failed(newOrUpdatedLocation,
                                                          "The given location must not be null!");


            if (!Locations.TryGetValue(newOrUpdatedLocation.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
            {
                parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                Locations.Add(newOrUpdatedLocation.CountryCode, parties);
            }

            if (!parties.TryGetValue(newOrUpdatedLocation.PartyId, out Dictionary<Location_Id, Location> locations))
            {
                locations = new Dictionary<Location_Id, Location>();
                parties.Add(newOrUpdatedLocation.PartyId, locations);
            }

            if (locations.TryGetValue(newOrUpdatedLocation.Id, out Location existingLocation))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    newOrUpdatedLocation.LastUpdated < existingLocation.LastUpdated)
                {
                    return AddOrUpdateResult<Location>.Failed(newOrUpdatedLocation,
                                                              "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!");
                }

                locations[newOrUpdatedLocation.Id] = newOrUpdatedLocation;

                return AddOrUpdateResult<Location>.Success(newOrUpdatedLocation,
                                                           WasCreated: false);

            }

            locations.Add(newOrUpdatedLocation.Id, newOrUpdatedLocation);

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

        public Location UpdateLocation(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (Locations)
            {

                if (Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties)   &&
                    parties.  TryGetValue(Location.PartyId,     out                      Dictionary<Location_Id, Location>  locations) &&
                    locations.ContainsKey(Location.Id))
                {
                    locations[Location.Id] = Location;
                    return Location;
                }

                return null;

            }

        }

        #endregion


        #region TryPatchLocation(Location,                  LocationPatch, AllowDowngrades = false)

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

                if (Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties)   &&
                    parties.  TryGetValue(Location.PartyId,     out                      Dictionary<Location_Id, Location>  locations) &&
                    locations.TryGetValue(Location.Id,          out Location                                                existingLocation))
                {

                    var patchResult = existingLocation.TryPatch(LocationPatch,
                                                                AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                        locations[Location.Id] = patchResult.PatchedData;

                    return patchResult;

                }

                else
                    return PatchResult<Location>.Failed(Location,
                                                        "The given location does not exist!");

            }

        }

        #endregion


        #region AddOrUpdateEVSE    (Location, newOrUpdatedEVSE, AllowDowngrades = false)

        private AddOrUpdateResult<EVSE> __addOrUpdateEVSE(Location  Location,
                                                          EVSE      newOrUpdatedEVSE,
                                                          Boolean?  AllowDowngrades = false)
        {

            if (Location is null)
                return AddOrUpdateResult<EVSE>.Failed(newOrUpdatedEVSE,
                                                      "The given location must not be null!");

            if (newOrUpdatedEVSE is null)
                return AddOrUpdateResult<EVSE>.Failed(newOrUpdatedEVSE,
                                                      "The given EVSE must not be null!");


            var EVSEExistedBefore = Location.TryGetEVSE(newOrUpdatedEVSE.UId, out EVSE existingEVSE);

            if (existingEVSE != null)
            {
                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    newOrUpdatedEVSE.LastUpdated < existingEVSE.LastUpdated)
                {
                    return AddOrUpdateResult<EVSE>.Failed(newOrUpdatedEVSE,
                                                          "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!");
                }
            }


            Location.SetEVSE(newOrUpdatedEVSE);

            // Update location timestamp!
            var builder = Location.ToBuilder();
            builder.LastUpdated = newOrUpdatedEVSE.LastUpdated;
            __addOrUpdateLocation(builder, (AllowDowngrades ?? this.AllowDowngrades) == false);


            return AddOrUpdateResult<EVSE>.Success(newOrUpdatedEVSE,
                                                   WasCreated: !EVSEExistedBefore);

        }

        public async Task<AddOrUpdateResult<EVSE>> AddOrUpdateEVSE(Location  Location,
                                                                   EVSE      newOrUpdatedEVSE,
                                                                   Boolean?  AllowDowngrades = false)
        {

            if (Location is null)
                return AddOrUpdateResult<EVSE>.Failed(newOrUpdatedEVSE,
                                                      "The given location must not be null!");

            if (newOrUpdatedEVSE is null)
                return AddOrUpdateResult<EVSE>.Failed(newOrUpdatedEVSE,
                                                      "The given EVSE must not be null!");

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

        #region TryPatchEVSE    (Location, EVSE,            EVSEPatch, AllowDowngrades = false)

        public async Task<PatchResult<EVSE>> TryPatchEVSE(Location  Location,
                                                          EVSE      EVSE,
                                                          JObject   EVSEPatch,
                                                          Boolean?  AllowDowngrades = false)
        {

            if (Location is null)
                return PatchResult<EVSE>.Failed(EVSE,
                                                "The given location must not be null!");

            if (EVSE is null)
                return PatchResult<EVSE>.Failed(EVSE,
                                                "The given EVSE must not be null!");

            if (EVSEPatch is null || !EVSEPatch.HasValues)
                return PatchResult<EVSE>.Failed(EVSE,
                                                "The given EVSE patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (Locations)
            {

                var patchResult = EVSE.TryPatch(EVSEPatch,
                                                AllowDowngrades ?? this.AllowDowngrades ?? false);

                if (patchResult.IsSuccess)
                {

                    Location.SetEVSE(patchResult.PatchedData);

                    // Update location timestamp!
                    var builder = Location.ToBuilder();
                    builder.LastUpdated = patchResult.PatchedData.LastUpdated;
                    __addOrUpdateLocation(builder, (AllowDowngrades ?? this.AllowDowngrades) == false);

                }

                return patchResult;

            }

        }

        #endregion


        #region AddOrUpdateConnector    (Location, EVSE, newOrUpdatedConnector, AllowDowngrades = false)

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

                var ConnectorExistedBefore = EVSE.TryGetConnector(newOrUpdatedConnector.Id, out Connector existingConnector);

                if (existingConnector != null)
                {
                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        newOrUpdatedConnector.LastUpdated < existingConnector.LastUpdated)
                    {
                        return AddOrUpdateResult<Connector>.Failed(newOrUpdatedConnector,
                                                                   "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");
                    }
                }

                EVSE.SetConnector(newOrUpdatedConnector);

                // Update EVSE/location timestamps!
                var evseBuilder     = EVSE.ToBuilder();
                evseBuilder.LastUpdated = newOrUpdatedConnector.LastUpdated;
                __addOrUpdateEVSE    (Location, evseBuilder,     (AllowDowngrades ?? this.AllowDowngrades) == false);


                return AddOrUpdateResult<Connector>.Success(newOrUpdatedConnector,
                                                            WasCreated: !ConnectorExistedBefore);

            }

        }

        #endregion

        #region TryPatchConnector  (Location, EVSE, Connector, ConnectorPatch, AllowDowngrades = false)

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

                    EVSE.SetConnector(patchResult.PatchedData);

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

                if (Locations.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Location_Id, Location> locations))
                    {
                        return locations.ContainsKey(LocationId);
                    }
                }

                return false;

            }

        }

        #endregion

        #region TryGetLocation(CountryCode, PartyId, LocationId, out Location)

        public Boolean TryGetLocation(CountryCode   CountryCode,
                                      Party_Id      PartyId,
                                      Location_Id   LocationId,
                                      out Location  Location)
        {

            lock (Locations)
            {

                if (Locations.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Location_Id, Location> locations))
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

        #region GetLocations  (CountryCode = null, PartyId = null)

        public IEnumerable<Location> GetLocations(CountryCode? CountryCode  = null,
                                                  Party_Id?    PartyId      = null)
        {

            lock (Locations)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (Locations.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out Dictionary<Location_Id, Location> locations))
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
                        if (party.TryGetValue(PartyId.Value, out Dictionary<Location_Id, Location> locations))
                        {
                            allLocations.AddRange(locations.Values);
                        }
                    }

                    return allLocations;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (Locations.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
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

                return new Location[0];

            }

        }

        #endregion


        #region RemoveLocation(Location)

        public Location RemoveLocation(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (Locations)
            {

                if (Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {

                    if (parties.TryGetValue(Location.PartyId, out Dictionary<Location_Id, Location> locations))
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

                if (Locations.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Location_Id, Location> locations))
                    {
                        locations.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion

        #region Tariffs

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Tariff_Id , Tariff>>> Tariffs;


        #region AddTariff           (Tariff)

        public Tariff AddTariff(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (Tariffs)
            {

                if (!Tariffs.TryGetValue(Tariff.CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>>();
                    Tariffs.Add(Tariff.CountryCode, parties);
                }

                if (!parties.TryGetValue(Tariff.PartyId, out Dictionary<Tariff_Id, Tariff> tariffs))
                {
                    tariffs = new Dictionary<Tariff_Id, Tariff>();
                    parties.Add(Tariff.PartyId, tariffs);
                }

                if (!tariffs.ContainsKey(Tariff.Id))
                {
                    tariffs.Add(Tariff.Id, Tariff);
                    return Tariff;
                }

                throw new ArgumentException("The given tariff already exists!");

            }

        }

        #endregion

        #region AddTariffIfNotExists(Tariff)

        public Tariff AddTariffIfNotExists(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (Tariffs)
            {

                if (!Tariffs.TryGetValue(Tariff.CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>>();
                    Tariffs.Add(Tariff.CountryCode, parties);
                }

                if (!parties.TryGetValue(Tariff.PartyId, out Dictionary<Tariff_Id, Tariff> tariffs))
                {
                    tariffs = new Dictionary<Tariff_Id, Tariff>();
                    parties.Add(Tariff.PartyId, tariffs);
                }

                if (!tariffs.ContainsKey(Tariff.Id))
                    tariffs.Add(Tariff.Id, Tariff);

                return Tariff;

            }

        }

        #endregion

        #region AddOrUpdateTariff   (Tariff)

        public Tariff AddOrUpdateTariff(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (Tariffs)
            {

                if (!Tariffs.TryGetValue(Tariff.CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>>();
                    Tariffs.Add(Tariff.CountryCode, parties);
                }

                if (!parties.TryGetValue(Tariff.PartyId, out Dictionary<Tariff_Id, Tariff> tariffs))
                {
                    tariffs = new Dictionary<Tariff_Id, Tariff>();
                    parties.Add(Tariff.PartyId, tariffs);
                }

                if (tariffs.ContainsKey(Tariff.Id))
                {
                    tariffs.Remove(Tariff.Id);
                }

                tariffs.Add(Tariff.Id, Tariff);
                return Tariff;

            }

        }

        #endregion

        #region UpdateTariff        (Tariff)

        public Tariff UpdateTariff(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (Tariffs)
            {

                if (Tariffs.TryGetValue(Tariff.CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties) &&
                    parties. TryGetValue(Tariff.PartyId,     out                      Dictionary<Tariff_Id, Tariff>  tariffs) &&
                    tariffs.ContainsKey(Tariff.Id))
                {
                    tariffs[Tariff.Id] = Tariff;
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

            lock (Tariffs)
            {

                if (Tariffs.TryGetValue(Tariff.CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties) &&
                    parties.TryGetValue(Tariff.PartyId,     out                      Dictionary<Tariff_Id, Tariff>  tariffs) &&
                    tariffs.TryGetValue(Tariff.Id,          out Tariff                                              tariff))
                {

                    var patchResult = tariff.TryPatch(TariffPatch,
                                                      AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                        tariffs[Tariff.Id] = patchResult.PatchedData;

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

            lock (Tariffs)
            {

                if (Tariffs.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Tariff_Id, Tariff> tariffs))
                    {
                        return tariffs.ContainsKey(TariffId);
                    }
                }

                return false;

            }

        }

        #endregion

        #region TryGetTariff(CountryCode, PartyId, TariffId, out Tariff)

        public Boolean TryGetTariff(CountryCode   CountryCode,
                                      Party_Id      PartyId,
                                      Tariff_Id   TariffId,
                                      out Tariff  Tariff)
        {

            lock (Tariffs)
            {

                if (Tariffs.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Tariff_Id, Tariff> tariffs))
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

        #region GetTariffs  (CountryCode = null, PartyId = null)

        public IEnumerable<Tariff> GetTariffs(CountryCode? CountryCode  = null,
                                                  Party_Id?    PartyId      = null)
        {

            lock (Tariffs)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (Tariffs.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out Dictionary<Tariff_Id, Tariff> tariffs))
                        {
                            return tariffs.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allTariffs = new List<Tariff>();

                    foreach (var party in Tariffs.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out Dictionary<Tariff_Id, Tariff> tariffs))
                        {
                            allTariffs.AddRange(tariffs.Values);
                        }
                    }

                    return allTariffs;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (Tariffs.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
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

                    foreach (var party in Tariffs.Values)
                    {
                        foreach (var tariffs in party.Values)
                        {
                            allTariffs.AddRange(tariffs.Values);
                        }
                    }

                    return allTariffs;

                }

                return new Tariff[0];

            }

        }

        #endregion


        #region RemoveTariff(Tariff)

        public Tariff RemoveTariff(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (Tariffs)
            {

                if (Tariffs.TryGetValue(Tariff.CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                {

                    if (parties.TryGetValue(Tariff.PartyId, out Dictionary<Tariff_Id, Tariff> tariffs))
                    {

                        if (tariffs.ContainsKey(Tariff.Id))
                        {
                            tariffs.Remove(Tariff.Id);
                        }

                        if (!tariffs.Any())
                            parties.Remove(Tariff.PartyId);

                    }

                    if (!parties.Any())
                        Tariffs.Remove(Tariff.CountryCode);

                }

                return Tariff;

            }

        }

        #endregion

        #region RemoveAllTariffs()

        /// <summary>
        /// Remove all tariffs.
        /// </summary>
        public void RemoveAllTariffs()
        {

            lock (Tariffs)
            {
                Tariffs.Clear();
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

            lock (Tariffs)
            {

                if (Tariffs.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Tariff_Id, Tariff>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Tariff_Id, Tariff> tariffs))
                    {
                        tariffs.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion

        #region Sessions

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Session_Id , Session>>> Sessions;


        #region AddSession           (Session)

        public Session AddSession(Session Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (Sessions)
            {

                if (!Sessions.TryGetValue(Session.CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Session_Id, Session>>();
                    Sessions.Add(Session.CountryCode, parties);
                }

                if (!parties.TryGetValue(Session.PartyId, out Dictionary<Session_Id, Session> sessions))
                {
                    sessions = new Dictionary<Session_Id, Session>();
                    parties.Add(Session.PartyId, sessions);
                }

                if (!sessions.ContainsKey(Session.Id))
                {
                    sessions.Add(Session.Id, Session);
                    return Session;
                }

                throw new ArgumentException("The given session already exists!");

            }

        }

        #endregion

        #region AddSessionIfNotExists(Session)

        public Session AddSessionIfNotExists(Session Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (Sessions)
            {

                if (!Sessions.TryGetValue(Session.CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Session_Id, Session>>();
                    Sessions.Add(Session.CountryCode, parties);
                }

                if (!parties.TryGetValue(Session.PartyId, out Dictionary<Session_Id, Session> sessions))
                {
                    sessions = new Dictionary<Session_Id, Session>();
                    parties.Add(Session.PartyId, sessions);
                }

                if (!sessions.ContainsKey(Session.Id))
                    sessions.Add(Session.Id, Session);

                return Session;

            }

        }

        #endregion

        #region AddOrUpdateSession   (Session)

        public Session AddOrUpdateSession(Session Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (Sessions)
            {

                if (!Sessions.TryGetValue(Session.CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Session_Id, Session>>();
                    Sessions.Add(Session.CountryCode, parties);
                }

                if (!parties.TryGetValue(Session.PartyId, out Dictionary<Session_Id, Session> sessions))
                {
                    sessions = new Dictionary<Session_Id, Session>();
                    parties.Add(Session.PartyId, sessions);
                }

                if (sessions.ContainsKey(Session.Id))
                {
                    sessions.Remove(Session.Id);
                }

                sessions.Add(Session.Id, Session);
                return Session;

            }

        }

        #endregion

        #region UpdateSession        (Session)

        public Session UpdateSession(Session Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (Sessions)
            {

                if (Sessions.TryGetValue(Session.CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties)  &&
                    parties.  TryGetValue(Session.PartyId,     out                      Dictionary<Session_Id, Session>  sessions) &&
                    sessions.ContainsKey(Session.Id))
                {
                    sessions[Session.Id] = Session;
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

            lock (Sessions)
            {

                if (Sessions.TryGetValue(Session.CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties)  &&
                    parties. TryGetValue(Session.PartyId,     out                      Dictionary<Session_Id, Session>  sessions) &&
                    sessions.TryGetValue(Session.Id,          out Session                                               session))
                {

                    var patchResult = session.TryPatch(SessionPatch,
                                                       AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                        sessions[Session.Id] = patchResult.PatchedData;

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

            lock (Sessions)
            {

                if (Sessions.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Session_Id, Session> sessions))
                    {
                        return sessions.ContainsKey(SessionId);
                    }
                }

                return false;

            }

        }

        #endregion

        #region TryGetSession(CountryCode, PartyId, SessionId, out Session)

        public Boolean TryGetSession(CountryCode  CountryCode,
                                     Party_Id     PartyId,
                                     Session_Id   SessionId,
                                     out Session  Session)
        {

            lock (Sessions)
            {

                if (Sessions.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Session_Id, Session> sessions))
                    {
                        if (sessions.TryGetValue(SessionId, out Session))
                            return true;
                    }
                }

                Session = null;
                return false;

            }

        }

        #endregion

        #region GetSessions  (CountryCode = null, PartyId = null)

        public IEnumerable<Session> GetSessions(CountryCode? CountryCode  = null,
                                                  Party_Id?    PartyId      = null)
        {

            lock (Sessions)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (Sessions.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out Dictionary<Session_Id, Session> sessions))
                        {
                            return sessions.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allSessions = new List<Session>();

                    foreach (var party in Sessions.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out Dictionary<Session_Id, Session> sessions))
                        {
                            allSessions.AddRange(sessions.Values);
                        }
                    }

                    return allSessions;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (Sessions.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
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

                    foreach (var party in Sessions.Values)
                    {
                        foreach (var sessions in party.Values)
                        {
                            allSessions.AddRange(sessions.Values);
                        }
                    }

                    return allSessions;

                }

                return new Session[0];

            }

        }

        #endregion


        #region RemoveSession(Session)

        public Session RemoveSession(Session Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (Sessions)
            {

                if (Sessions.TryGetValue(Session.CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                {

                    if (parties.TryGetValue(Session.PartyId, out Dictionary<Session_Id, Session> sessions))
                    {

                        if (sessions.ContainsKey(Session.Id))
                        {
                            sessions.Remove(Session.Id);
                        }

                        if (!sessions.Any())
                            parties.Remove(Session.PartyId);

                    }

                    if (!parties.Any())
                        Sessions.Remove(Session.CountryCode);

                }

                return Session;

            }

        }

        #endregion

        #region RemoveAllSessions()

        /// <summary>
        /// Remove all sessions.
        /// </summary>
        public void RemoveAllSessions()
        {

            lock (Sessions)
            {
                Sessions.Clear();
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

            lock (Sessions)
            {

                if (Sessions.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Session_Id, Session>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Session_Id, Session> sessions))
                    {
                        sessions.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion

        #region Tokens

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>>> Tokens;


        #region AddToken           (Token, Status = AllowedTypes.ALLOWED)

        public Token AddToken(Token         Token,
                              AllowedTypes  Status = AllowedTypes.ALLOWED)
        {

            if (Token is null)
                throw new ArgumentNullException(nameof(Token), "The given token must not be null!");

            lock (Tokens)
            {

                if (!Tokens.TryGetValue(Token.CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>>();
                    Tokens.Add(Token.CountryCode, parties);
                }

                if (!parties.TryGetValue(Token.PartyId, out Dictionary<Token_Id, TokenStatus> tokens))
                {
                    tokens = new Dictionary<Token_Id, TokenStatus>();
                    parties.Add(Token.PartyId, tokens);
                }

                if (!tokens.ContainsKey(Token.Id))
                {
                    tokens.Add(Token.Id, new TokenStatus(Token, Status));
                    return Token;
                }

                throw new ArgumentException("The given token already exists!");

            }

        }

        #endregion

        #region AddTokenIfNotExists(Token, Status = AllowedTypes.ALLOWED)

        public Token AddTokenIfNotExists(Token         Token,
                                         AllowedTypes  Status = AllowedTypes.ALLOWED)
        {

            if (Token is null)
                throw new ArgumentNullException(nameof(Token), "The given token must not be null!");

            lock (Tokens)
            {

                if (!Tokens.TryGetValue(Token.CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>>();
                    Tokens.Add(Token.CountryCode, parties);
                }

                if (!parties.TryGetValue(Token.PartyId, out Dictionary<Token_Id, TokenStatus> tokens))
                {
                    tokens = new Dictionary<Token_Id, TokenStatus>();
                    parties.Add(Token.PartyId, tokens);
                }

                if (!tokens.ContainsKey(Token.Id))
                    tokens.Add(Token.Id, new TokenStatus(Token, Status));

                return Token;

            }

        }

        #endregion

        #region AddOrUpdateToken   (Token, Status = AllowedTypes.ALLOWED)

        public Token AddOrUpdateToken(Token         Token,
                                      AllowedTypes  Status = AllowedTypes.ALLOWED)
        {

            if (Token is null)
                throw new ArgumentNullException(nameof(Token), "The given token must not be null!");

            lock (Tokens)
            {

                if (!Tokens.TryGetValue(Token.CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>>();
                    Tokens.Add(Token.CountryCode, parties);
                }

                if (!parties.TryGetValue(Token.PartyId, out Dictionary<Token_Id, TokenStatus> tokens))
                {
                    tokens = new Dictionary<Token_Id, TokenStatus>();
                    parties.Add(Token.PartyId, tokens);
                }

                if (tokens.ContainsKey(Token.Id))
                {
                    tokens.Remove(Token.Id);
                }

                tokens.Add(Token.Id, new TokenStatus(Token, Status));
                return Token;

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

            lock (Tokens)
            {

                if (Tokens. TryGetValue(Token.CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties) &&
                    parties.TryGetValue(Token.PartyId,     out                      Dictionary<Token_Id, TokenStatus>  tokens) &&
                    tokens. TryGetValue(Token.Id,          out TokenStatus                                             tokenStatus))
                {

                    var patchResult = tokenStatus.Token.TryPatch(TokenPatch,
                                                                 AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                        tokens[Token.Id] = new TokenStatus(patchResult.PatchedData,
                                                           tokenStatus.Status);

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

            lock (Tokens)
            {

                if (Tokens.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Token_Id, TokenStatus> tokens))
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

            lock (Tokens)
            {

                if (Tokens.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Token_Id, TokenStatus> tokens))
                    {
                        if (tokens.TryGetValue(TokenId, out TokenWithStatus))
                            return true;
                    }
                }

                TokenWithStatus = default;
                return false;

            }

        }

        #endregion

        #region GetTokens  (CountryCode = null, PartyId = null)

        public IEnumerable<TokenStatus> GetTokens(CountryCode?  CountryCode   = null,
                                                  Party_Id?     PartyId       = null)
        {

            lock (Tokens)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (Tokens.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out Dictionary<Token_Id, TokenStatus> tokens))
                        {
                            return tokens.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allTokens = new List<TokenStatus>();

                    foreach (var party in Tokens.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out Dictionary<Token_Id, TokenStatus> tokens))
                        {
                            allTokens.AddRange(tokens.Values);
                        }
                    }

                    return allTokens;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (Tokens.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
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

                    foreach (var party in Tokens.Values)
                    {
                        foreach (var tokens in party.Values)
                        {
                            allTokens.AddRange(tokens.Values);
                        }
                    }

                    return allTokens;

                }

                return new TokenStatus[0];

            }

        }

        #endregion


        #region RemoveToken(TokenId)

        public Token RemoveToken(Token_Id TokenId)
        {

            lock (Tokens)
            {

                Token foundToken = null;

                foreach (var parties in Tokens.Values)
                {

                    foreach (var tokens in parties.Values)
                    {
                        if (tokens.TryGetValue(TokenId, out TokenStatus tokenStatus))
                        {
                            foundToken = tokenStatus.Token;
                            break;
                        }
                    }

                    if (foundToken != null)
                        break;

                }

                return foundToken != null
                           ? RemoveToken(foundToken)
                           : null;

            }

        }

        #endregion

        #region RemoveToken(Token)

        public Token RemoveToken(Token Token)
        {

            if (Token is null)
                throw new ArgumentNullException(nameof(Token), "The given token must not be null!");

            lock (Tokens)
            {

                if (Tokens.TryGetValue(Token.CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                {

                    if (parties.TryGetValue(Token.PartyId, out Dictionary<Token_Id, TokenStatus> tokens))
                    {

                        if (tokens.ContainsKey(Token.Id))
                        {
                            tokens.Remove(Token.Id);
                        }

                        if (!tokens.Any())
                            parties.Remove(Token.PartyId);

                    }

                    if (!parties.Any())
                        Tokens.Remove(Token.CountryCode);

                }

                return Token;

            }

        }

        #endregion

        #region RemoveAllTokens()

        /// <summary>
        /// Remove all tokens.
        /// </summary>
        public void RemoveAllTokens()
        {

            lock (Tokens)
            {
                Tokens.Clear();
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

            lock (Tokens)
            {

                if (Tokens.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Token_Id, TokenStatus> tokens))
                    {
                        tokens.Clear();
                    }
                }

            }

        }

        #endregion

        #endregion

        #region CDRs

        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<CDR_Id, CDR>>> CDRs;


        #region AddCDR           (CDR)

        public CDR AddCDR(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (CDRs)
            {

                if (!CDRs.TryGetValue(CDR.CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<CDR_Id, CDR>>();
                    CDRs.Add(CDR.CountryCode, parties);
                }

                if (!parties.TryGetValue(CDR.PartyId, out Dictionary<CDR_Id, CDR> partyCDRs))
                {
                    partyCDRs = new Dictionary<CDR_Id, CDR>();
                    parties.Add(CDR.PartyId, partyCDRs);
                }

                if (!partyCDRs.ContainsKey(CDR.Id))
                {
                    partyCDRs.Add(CDR.Id, CDR);
                    return CDR;
                }

                throw new ArgumentException("The given charge detail record already exists!");

            }

        }

        #endregion

        #region AddCDRIfNotExists(CDR)

        public CDR AddCDRIfNotExists(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (CDRs)
            {

                if (!CDRs.TryGetValue(CDR.CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<CDR_Id, CDR>>();
                    CDRs.Add(CDR.CountryCode, parties);
                }

                if (!parties.TryGetValue(CDR.PartyId, out Dictionary<CDR_Id, CDR> partyCDRs))
                {
                    partyCDRs = new Dictionary<CDR_Id, CDR>();
                    parties.Add(CDR.PartyId, partyCDRs);
                }

                if (!partyCDRs.ContainsKey(CDR.Id))
                    partyCDRs.Add(CDR.Id, CDR);

                return CDR;

            }

        }

        #endregion

        #region AddOrUpdateCDR   (CDR)

        public CDR AddOrUpdateCDR(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (CDRs)
            {

                if (!CDRs.TryGetValue(CDR.CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<CDR_Id, CDR>>();
                    CDRs.Add(CDR.CountryCode, parties);
                }

                if (!parties.TryGetValue(CDR.PartyId, out Dictionary<CDR_Id, CDR> partyCDRs))
                {
                    partyCDRs = new Dictionary<CDR_Id, CDR>();
                    parties.Add(CDR.PartyId, partyCDRs);
                }

                if (partyCDRs.ContainsKey(CDR.Id))
                {
                    partyCDRs.Remove(CDR.Id);
                }

                partyCDRs.Add(CDR.Id, CDR);
                return CDR;

            }

        }

        #endregion

        #region UpdateCDR        (CDR)

        public CDR UpdateCDR(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (CDRs)
            {

                if (CDRs.     TryGetValue(CDR.CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties)   &&
                    parties.  TryGetValue(CDR.PartyId,     out                      Dictionary<CDR_Id, CDR>  partyCDRs) &&
                    partyCDRs.ContainsKey(CDR.Id))
                {
                    partyCDRs[CDR.Id] = CDR;
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

            lock (CDRs)
            {

                if (CDRs.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<CDR_Id, CDR> partyCDRs))
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
                                 out CDR      CDR)
        {

            lock (CDRs)
            {

                if (CDRs.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<CDR_Id, CDR> partyCDRs))
                    {
                        if (partyCDRs.TryGetValue(CDRId, out CDR))
                            return true;
                    }
                }

                CDR = null;
                return false;

            }

        }

        #endregion

        #region GetCDRs  (CountryCode = null, PartyId = null)

        public IEnumerable<CDR> GetCDRs(CountryCode?  CountryCode   = null,
                                        Party_Id?     PartyId       = null)
        {

            lock (CDRs)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (CDRs.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out Dictionary<CDR_Id, CDR> partyCDRs))
                        {
                            return partyCDRs.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allCDRs = new List<CDR>();

                    foreach (var party in CDRs.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out Dictionary<CDR_Id, CDR> partyCDRs))
                        {
                            allCDRs.AddRange(partyCDRs.Values);
                        }
                    }

                    return allCDRs;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (CDRs.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
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

                    foreach (var party in CDRs.Values)
                    {
                        foreach (var partyCDRs in party.Values)
                        {
                            allCDRs.AddRange(partyCDRs.Values);
                        }
                    }

                    return allCDRs;

                }

                return new CDR[0];

            }

        }

        #endregion


        #region RemoveCDR(CDR)

        public CDR RemoveCDR(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (CDRs)
            {

                if (CDRs.TryGetValue(CDR.CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                {

                    if (parties.TryGetValue(CDR.PartyId, out Dictionary<CDR_Id, CDR> partyCDRs))
                    {

                        if (partyCDRs.ContainsKey(CDR.Id))
                        {
                            partyCDRs.Remove(CDR.Id);
                        }

                        if (!partyCDRs.Any())
                            parties.Remove(CDR.PartyId);

                    }

                    if (!parties.Any())
                        CDRs.Remove(CDR.CountryCode);

                }

                return CDR;

            }

        }

        #endregion

        #region RemoveAllCDRs()

        /// <summary>
        /// Remove all CDRs.
        /// </summary>
        public void RemoveAllCDRs()
        {

            lock (CDRs)
            {
                CDRs.Clear();
            }

        }

        #endregion

        #region RemoveAllCDRs(CountryCode, PartyId)

        /// <summary>
        /// Remove all CDRs owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllCDRs(CountryCode  CountryCode,
                                  Party_Id     PartyId)
        {

            lock (CDRs)
            {

                if (CDRs.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<CDR_Id, CDR>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<CDR_Id, CDR> partyCDRs))
                        partyCDRs.Clear();
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
                //    Subject     = "Open Data API '" + _ServiceName + "' restarted! at " + DateTime.Now.ToString(),
                //    PlainText   = "Open Data API '" + _ServiceName + "' restarted! at " + DateTime.Now.ToString(),
                //    HTMLText    = "Open Data API <b>'" + _ServiceName + "'</b> restarted! at " + DateTime.Now.ToString(),
                //    Passphrase  = _APIPassphrase
                //};
                //
                //var SMTPTask = _APISMTPClient.Send(Message0);
                //SMTPTask.Wait();

                //var r = SMTPTask.Result;

                #endregion

                //SendStarted(this, DateTime.Now);

            }

        }

        #endregion

        #region Shutdown(Message = null, Wait = true)

        public void Shutdown(String Message = null, Boolean Wait = true)
        {

            lock (HTTPServer)
            {

                HTTPServer.Shutdown(Message, Wait);

                //SendCompleted(this, DateTime.UtcNow, Message);

            }

        }

        #endregion

    }

}
